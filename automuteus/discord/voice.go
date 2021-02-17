package discord

import (
	galactus_client "github.com/automuteus/galactus/pkg/client"
	"github.com/automuteus/utils/pkg/discord"
	"github.com/automuteus/utils/pkg/premium"
	"github.com/automuteus/utils/pkg/settings"
	"log"
	"strconv"
	"time"
)

type HandlePriority int

const (
	NoPriority    HandlePriority = 0
	AlivePriority HandlePriority = 1
	DeadPriority  HandlePriority = 2
)

func (bot *Bot) applyToSingle(dgs *GameState, userID string, mute, deaf bool) {
	log.Println("Forcibly applying mute/deaf to " + userID)
	premTier := premium.FreeTier
	premiumRecord, err := bot.GalactusClient.GetGuildPremium(dgs.GuildID)
	if err == nil && !premium.IsExpired(premiumRecord.Tier, premiumRecord.Days) {
		premTier = premiumRecord.Tier
	}
	uid, _ := strconv.ParseUint(userID, 10, 64)
	req := discord.UserModifyRequest{
		Premium: premTier,
		Users: []discord.UserModify{
			{
				UserID: uid,
				Mute:   mute,
				Deaf:   deaf,
			},
		},
	}
	// nil lock because this is an override; we don't care about legitimately obtaining the lock
	mdsc := bot.GalactusClient.ModifyUsers(dgs.GuildID, dgs.ConnectCode, req)
	if mdsc == nil {
		log.Println("Nil response from modifyUsers, probably not good...")
	}
}

func (bot *Bot) applyToAll(dgs *GameState, mute, deaf bool) {
	g, err := bot.GalactusClient.GetGuild(dgs.GuildID)
	if err != nil {
		log.Println(err)
		return
	}

	users := []discord.UserModify{}

	for _, voiceState := range g.VoiceStates {
		userData, err := dgs.GetUser(voiceState.UserID)
		if err != nil {
			// the User doesn't exist in our userdata cache; add them
			added := false
			userData, added = dgs.checkCacheAndAddUser(g, bot.GalactusClient, voiceState.UserID)
			if !added {
				continue
			}
		}

		tracked := voiceState.ChannelID != "" && dgs.Tracking.ChannelID == voiceState.ChannelID

		_, linked := dgs.AmongUsData.GetByName(userData.InGameName)
		// only actually tracked if we're in a tracked channel AND linked to a player
		tracked = tracked && linked

		if tracked {
			uid, _ := strconv.ParseUint(userData.User.UserID, 10, 64)
			users = append(users, discord.UserModify{
				UserID: uid,
				Mute:   mute,
				Deaf:   deaf,
			})
			log.Println("Forcibly applying mute/deaf to " + userData.User.UserID)
		}
	}
	if len(users) > 0 {
		premTier := premium.FreeTier
		premiumRecord, err := bot.GalactusClient.GetGuildPremium(dgs.GuildID)
		if err == nil && !premium.IsExpired(premiumRecord.Tier, premiumRecord.Days) {
			premTier = premiumRecord.Tier
		}
		req := discord.UserModifyRequest{
			Premium: premTier,
			Users:   users,
		}

		mdsc := bot.GalactusClient.ModifyUsers(dgs.GuildID, dgs.ConnectCode, req)
		if mdsc == nil {
			log.Println("Nil response from modifyUsers, probably not good...")
		}
	}
}

// handleTrackedMembers moves/mutes players according to the current game state
func (bot *Bot) handleTrackedMembers(galactus *galactus_client.GalactusClient, sett *settings.GuildSettings, delay int, handlePriority HandlePriority, gsr GameStateRequest) {

	lock, dgs := bot.RedisInterface.GetDiscordGameStateAndLock(gsr)
	for lock == nil {
		lock, dgs = bot.RedisInterface.GetDiscordGameStateAndLock(gsr)
	}

	g, err := galactus.GetGuild(dgs.GuildID)
	if err != nil || g == nil {
		lock.Unlock()
		return
	}

	users := []discord.UserModify{}

	priorityRequests := 0
	for _, voiceState := range g.VoiceStates {
		userData, err := dgs.GetUser(voiceState.UserID)
		if err != nil {
			// the User doesn't exist in our userdata cache; add them
			added := false
			userData, added = dgs.checkCacheAndAddUser(g, bot.GalactusClient, voiceState.UserID)
			if !added {
				continue
			}
		}

		tracked := voiceState.ChannelID != "" && dgs.Tracking.ChannelID == voiceState.ChannelID

		auData, found := dgs.AmongUsData.GetByName(userData.InGameName)
		// only actually tracked if we're in a tracked channel AND linked to a player
		var isAlive bool

		// only actually tracked if we're in a tracked channel AND linked to a player
		if !sett.GetMuteSpectator() {
			tracked = tracked && found
			isAlive = auData.IsAlive
		} else {
			if !found {
				// we just assume the spectator is dead
				isAlive = false
			} else {
				isAlive = auData.IsAlive
			}
		}
		shouldMute, shouldDeaf := sett.GetVoiceState(isAlive, tracked, dgs.AmongUsData.GetPhase())

		incorrectMuteDeafenState := shouldMute != userData.ShouldBeMute || shouldDeaf != userData.ShouldBeDeaf

		// only issue a change if the User isn't in the right state already
		// nicksmatch can only be false if the in-game data is != nil, so the reference to .audata below is safe
		// check the userdata is linked here to not accidentally undeafen music bots, for example
		if incorrectMuteDeafenState && (found || sett.GetMuteSpectator()) {
			uid, _ := strconv.ParseUint(userData.User.UserID, 10, 64)
			userModify := discord.UserModify{
				UserID: uid,
				Mute:   shouldMute,
				Deaf:   shouldDeaf,
			}

			if handlePriority != NoPriority && ((handlePriority == AlivePriority && isAlive) || (handlePriority == DeadPriority && !isAlive)) {
				users = append([]discord.UserModify{userModify}, users...)
				priorityRequests++ // counter of how many elements on the front of the arr should be sent first
			} else {
				users = append(users, userModify)
			}
			userData.SetShouldBeMuteDeaf(shouldMute, shouldDeaf)
			dgs.UpdateUserData(userData.User.UserID, userData)
		}
	}

	// we relinquish the lock while we wait
	bot.RedisInterface.SetDiscordGameState(dgs, lock)

	voiceLock, err := bot.RedisInterface.LockVoiceChanges(dgs.ConnectCode)
	if err != nil || voiceLock == nil {
		return
	}
	defer voiceLock.Unlock()

	if delay > 0 {
		log.Printf("Sleeping for %d seconds before applying changes to users\n", delay)
		time.Sleep(time.Second * time.Duration(delay))
	}

	if dgs.Running && len(users) > 0 {
		premTier := premium.FreeTier
		premiumRecord, err := bot.GalactusClient.GetGuildPremium(dgs.GuildID)
		if err == nil && !premium.IsExpired(premiumRecord.Tier, premiumRecord.Days) {
			premTier = premiumRecord.Tier
		}

		if priorityRequests > 0 {
			req := discord.UserModifyRequest{
				Premium: premTier,
				Users:   users[:priorityRequests],
			}
			bot.issueMutesAndRecord(dgs.GuildID, dgs.ConnectCode, req)
			log.Println("Finished issuing high priority mutes")
			rem := users[priorityRequests:]
			if len(rem) > 0 {
				req = discord.UserModifyRequest{
					Premium: premTier,
					Users:   rem,
				}
				bot.issueMutesAndRecord(dgs.GuildID, dgs.ConnectCode, req)
			}
		} else {
			// no priority; issue all at once
			log.Println("Issuing mutes/deafens with no particular priority")
			req := discord.UserModifyRequest{
				Premium: premTier,
				Users:   users,
			}
			bot.issueMutesAndRecord(dgs.GuildID, dgs.ConnectCode, req)
		}
	}
}

func (bot *Bot) issueMutesAndRecord(guildID, connectCode string, req discord.UserModifyRequest) {
	mdsc := bot.GalactusClient.ModifyUsers(guildID, connectCode, req)
	if mdsc == nil {
		log.Println("Nil response from modifyUsers, probably not good...")
	}
}
