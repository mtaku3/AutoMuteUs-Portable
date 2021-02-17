package storage

import (
	"context"
	"encoding/json"
	"errors"
	"github.com/alicebob/miniredis/v2"
	"github.com/automuteus/utils/pkg/rediskey"
	"github.com/automuteus/utils/pkg/settings"
	"github.com/go-redis/redis/v8"
	"log"
)

var ctx = context.Background()

type StorageInterface struct {
	client        *redis.Client
	defaultPrefix string
}

type RedisParameters struct {
	Addr     string
	Username string
	Password string
}

func (storageInterface *StorageInterface) InitMock() {
	mr, err := miniredis.Run()
	if err != nil {
		panic(err)
	}

	storageInterface.client = redis.NewClient(&redis.Options{
		Addr: mr.Addr(),
	})
}

func (storageInterface *StorageInterface) Init(prefix string, params interface{}) error {
	redisParams := params.(RedisParameters)
	rdb := redis.NewClient(&redis.Options{
		Addr:     redisParams.Addr,
		Username: redisParams.Username,
		Password: redisParams.Password,
		DB:       0, // use default DB
	})
	storageInterface.client = rdb
	storageInterface.defaultPrefix = prefix
	return nil
}

func (storageInterface *StorageInterface) NewGuildSettings() *settings.GuildSettings {
	return settings.MakeGuildSettings(storageInterface.defaultPrefix)
}

func (storageInterface *StorageInterface) GetGuildSettings(guildID string) *settings.GuildSettings {
	key := rediskey.GuildSettings(rediskey.HashGuildID(guildID))

	j, err := storageInterface.client.Get(ctx, key).Result()
	switch {
	case errors.Is(err, redis.Nil):
		s := settings.MakeGuildSettings(storageInterface.defaultPrefix)
		jBytes, err := json.MarshalIndent(s, "", "  ")
		if err != nil {
			log.Println(err)
			return settings.MakeGuildSettings(storageInterface.defaultPrefix)
		}
		err = storageInterface.client.Set(ctx, key, jBytes, 0).Err()
		if err != nil {
			log.Println(err)
		}
		return s
	case err != nil:
		log.Println(err)
		return settings.MakeGuildSettings(storageInterface.defaultPrefix)
	default:
		s := settings.GuildSettings{}
		err := json.Unmarshal([]byte(j), &s)
		if err != nil {
			log.Println(err)
			return settings.MakeGuildSettings(storageInterface.defaultPrefix)
		}
		return &s
	}
}

func (storageInterface *StorageInterface) SetGuildSettings(guildID string, guildSettings *settings.GuildSettings) error {
	key := rediskey.GuildSettings(rediskey.HashGuildID(guildID))

	jbytes, err := json.MarshalIndent(guildSettings, "", "  ")
	if err != nil {
		return err
	}
	err = storageInterface.client.Set(ctx, key, jbytes, 0).Err()
	return err
}

func (storageInterface *StorageInterface) DeleteGuildSettings(guildID string) error {
	key := rediskey.GuildSettings(rediskey.HashGuildID(guildID))

	err := storageInterface.client.Del(ctx, key).Err()
	return err
}

func (storageInterface *StorageInterface) Close() error {
	return storageInterface.client.Close()
}
