package main

import (
	"github.com/automuteus/wingman/pkg/broker"
	"go.uber.org/zap"
	"log"
	"os"
	"os/signal"
	"syscall"
)

const DefaultWingmanPort = "8123"

func main() {
	logger, err := zap.NewProduction()
	if err != nil {
		log.Fatal(err)
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
