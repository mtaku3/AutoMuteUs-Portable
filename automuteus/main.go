package main

import (
	"errors"
	galactus_client "github.com/automuteus/galactus/pkg/client"
	"github.com/automuteus/utils/pkg/settings"
	"go.uber.org/zap"
	"log"
	"math/rand"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/denverquane/amongusdiscord/storage"

	"github.com/denverquane/amongusdiscord/discord"
)

var (
	version = "7.0.0"
	commit  = "none"
	date    = "unknown"
)

const DefaultURL = "http://localhost:8123"
const DefaultPrefix = ".au"

func main() {
	// seed the rand generator (used for making connection codes)
	rand.Seed(time.Now().Unix())

	logger, err := zap.NewProduction()
	if err != nil {
		log.Fatal(err)
	}

	err = discordMainWrapper(logger)
	if err != nil {
		logger.Fatal("program exited with error",
			zap.Error(err))
		return
	}
}

func discordMainWrapper(logger *zap.Logger) error {
	emojiGuildID := os.Getenv("EMOJI_GUILD_ID")
	if emojiGuildID == "" {
		log.Println("No EMOJI_GUILD_ID specified!!! Emojis will be added to a RANDOM guild, I recommend you specify " +
			"which guild you'd like me to add emojis to by specifying EMOJI_GUILD_ID")
	}

	log.Println(version + "-" + commit)

	url := os.Getenv("HOST")
	if url == "" {
		log.Printf("[Info] No valid HOST provided. Defaulting to %s\n", DefaultURL)
		url = DefaultURL
	}

	var redisClient discord.RedisInterface
	var storageInterface storage.StorageInterface

	redisAddr := os.Getenv("REDIS_ADDR")
	redisPassword := os.Getenv("REDIS_PASS")
	if redisAddr != "" {
		err := redisClient.Init(storage.RedisParameters{
			Addr:     redisAddr,
			Username: "",
			Password: redisPassword,
		})
		if err != nil {
			log.Println(err)
		}
		prefix := os.Getenv("AUTOMUTEUS_GLOBAL_PREFIX")
		if prefix == "" {
			prefix = DefaultPrefix
		}
		err = storageInterface.Init(prefix, storage.RedisParameters{
			Addr:     redisAddr,
			Username: "",
			Password: redisPassword,
		})
		if err != nil {
			log.Println(err)
		}
	} else {
		return errors.New("no REDIS_ADDR specified; exiting")
	}

	galactusAddr := os.Getenv("GALACTUS_ADDR")
	if galactusAddr == "" {
		return errors.New("no GALACTUS_ADDR specified; exiting")
	}

	galactusClient, err := galactus_client.NewGalactusClient(galactusAddr, logger)
	for err != nil {
		logger.Error("error connecting to galactus. Retrying every second until it is reachable",
			zap.Error(err))
		time.Sleep(time.Second)
		galactusClient, err = galactus_client.NewGalactusClient(galactusAddr, logger)
	}

	settings.InitLang(os.Getenv("LOCALE_PATH"), os.Getenv("BOT_LANG"))

	psql := storage.PsqlInterface{}
	pAddr := os.Getenv("POSTGRES_ADDR")
	if pAddr == "" {
		return errors.New("no POSTGRES_ADDR specified; exiting")
	}

	pUser := os.Getenv("POSTGRES_USER")
	if pUser == "" {
		return errors.New("no POSTGRES_USER specified; exiting")
	}

	pPass := os.Getenv("POSTGRES_PASS")
	if pPass == "" {
		return errors.New("no POSTGRES_PASS specified; exiting")
	}

	err = psql.Init(storage.ConstructPsqlConnectURL(pAddr, pUser, pPass))
	if err != nil {
		return err
	}

	// on the official bot, we don't want to accidentally clobber schemas
	if os.Getenv("AUTOMUTEUS_OFFICIAL") == "" {
		go psql.LoadAndExecFromFile("./storage/postgres.sql")
	}

	log.Println("Bot is now running.  Press CTRL-C to exit.")
	sc := make(chan os.Signal, 1)
	signal.Notify(sc, syscall.SIGINT, syscall.SIGTERM, os.Interrupt, os.Kill)

	redisClient.SetVersionAndCommit(version, commit)

	bot := discord.MakeAndStartBot(url, emojiGuildID, &redisClient, &storageInterface, &psql, galactusClient, logger)

	<-sc
	log.Printf("Received Sigterm or Kill signal. Bot will terminate in 1 second")
	time.Sleep(time.Second)

	bot.Close()
	return nil
}
