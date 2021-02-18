package main

import (
	"log"
	"os"
	"os/signal"
	"syscall"

	"github.com/automuteus/wingman/pkg/broker"
	"go.uber.org/zap"

	"github.com/joho/godotenv"
)

const DefaultWingmanPort = "8123"

func main() {
	logger, err := zap.NewProduction()
	if err != nil {
		log.Fatal(err)
	}

	// Load .env file
	err = godotenv.Load(".env")
	if err != nil {
		log.Fatal(err)
		return
	}

	galactusAddr := os.Getenv("GALACTUS_ADDR")
	if galactusAddr == "" {
		logger.Fatal("no GALACTUS_ADDR specified")
	}

	brokerPort := os.Getenv("WINGMAN_PORT")
	if brokerPort == "" {
		logger.Info("no WINGMAN_PORT provided. Using default value",
			zap.String("default", DefaultWingmanPort))
		brokerPort = DefaultWingmanPort
	}

	socketBroker := broker.NewBroker(galactusAddr, logger)

	sc := make(chan os.Signal, 1)
	signal.Notify(sc, syscall.SIGINT, syscall.SIGTERM, os.Interrupt, os.Kill)

	go socketBroker.Start(brokerPort)

	<-sc
	logger.Info("wingman received a kill/term signal and is not exiting")
}
