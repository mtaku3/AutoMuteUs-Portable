package broker

import (
	"encoding/json"
	galactus_client "github.com/automuteus/galactus/pkg/client"
	"github.com/automuteus/utils/pkg/capture"
	"github.com/automuteus/utils/pkg/game"
	socketio "github.com/googollee/go-socket.io"
	"github.com/gorilla/mux"
	"go.uber.org/zap"
	"net/http"
	"strconv"
	"sync"
	"time"
)

const ConnectCodeLength = 8

type Broker struct {
	client *galactus_client.GalactusClient

	// map of socket IDs to connection codes
	connections map[string]string

	ackKillChannels map[string]chan bool
	connectionsLock sync.RWMutex

	logger *zap.Logger
}

func NewBroker(galactusAddr string, logger *zap.Logger) *Broker {
	client, err := galactus_client.NewGalactusClient(galactusAddr, logger)
	for err != nil {
		logger.Error("error connecting to galactus. Retrying every second until it is reachable",
			zap.Error(err))
		time.Sleep(time.Second)
		client, err = galactus_client.NewGalactusClient(galactusAddr, logger)
	}
	return &Broker{
		client:          client,
		connections:     map[string]string{},
		ackKillChannels: map[string]chan bool{},
		connectionsLock: sync.RWMutex{},
		logger:          logger,
	}
}

func (broker *Broker) Start(port string) {
	logger := broker.logger
	server, err := socketio.NewServer(nil)
	if err != nil {
		logger.Fatal("failed to start socket server with error",
			zap.Error(err),
		)
	}

	server.OnConnect("/", func(s socketio.Conn) error {
		s.SetContext("")
		logger.Info("socket.io client connected",
			zap.String("socketID", s.ID()))
		return nil
	})

	server.OnEvent("/", "connectCode", func(s socketio.Conn, msg string) {
		if len(msg) != ConnectCodeLength {
			logger.Info("closing socket connection because connectCode is invalid",
				zap.String("socketID", s.ID()),
				zap.String("connectCode", msg),
			)
			s.Close()
		} else {
			killChannel := make(chan bool)

			logger.Info("received connection code",
				zap.String("connectCode", msg))

			broker.connectionsLock.Lock()
			broker.connections[s.ID()] = msg
			broker.ackKillChannels[s.ID()] = killChannel
			broker.connectionsLock.Unlock()

			event := capture.Event{
				EventType: capture.Connection,
				Payload:   []byte("true"),
			}
			logger.Info("sending connection message to galactus")

			err := broker.client.AddCaptureEvent(msg, event)
			if err != nil {
				logger.Error("error sending connection capture message to galactus",
					zap.Error(err),
					zap.String("socketID", s.ID()),
					zap.String("connectCode", msg),
					zap.Int("eventType", int(event.EventType)),
					zap.ByteString("payload", event.Payload),
				)
			} else {
				logger.Info("successfully sent connection capture event to galactus",
					zap.String("socketID", s.ID()),
					zap.String("connectCode", msg),
					zap.Int("eventType", int(event.EventType)),
					zap.ByteString("payload", event.Payload),
				)
			}
		}
	})

	// only join the room for the connect code once we ensure that the bot actually connects with a valid discord session
	server.OnEvent("/", "botID", func(s socketio.Conn, msg int64) {
		broker.connectionsLock.RLock()
		if cCode, ok := broker.connections[s.ID()]; ok {
			logger.Info("received BotID for Socket client",
				zap.String("socketID", s.ID()),
				zap.Int64("botID", msg))
			// this socket is now listening for mutes that can be applied via that connect code
			s.Join(cCode)
			killChan := broker.ackKillChannels[s.ID()]
			if killChan != nil {
				go broker.tasksListener(server, cCode, killChan)
			} else {
				logger.Error("null killchannel for connectCode (received botID before connectCode message)",
					zap.String("connectCode", cCode),
				)
			}
		}
		broker.connectionsLock.RUnlock()
	})

	server.OnEvent("/", "taskFailed", func(s socketio.Conn, msg string) {
		err := broker.client.SetCaptureTaskStatus(msg, "false")
		if err != nil {
			logger.Error("error marking task as unsuccessful",
				zap.Error(err),
				zap.String("task", msg),
			)
		}
	})

	server.OnEvent("/", "taskComplete", func(s socketio.Conn, msg string) {
		err := broker.client.SetCaptureTaskStatus(msg, "true")
		if err != nil {
			logger.Error("error marking task as successful",
				zap.Error(err),
				zap.String("task", msg),
			)
		}
	})

	server.OnEvent("/", "lobby", func(s socketio.Conn, msg string) {
		// validation
		var lobby game.Lobby
		err := json.Unmarshal([]byte(msg), &lobby)
		if err != nil {
			logger.Error("error unmarshalling lobby message",
				zap.Error(err),
				zap.String("socketID", s.ID()),
				zap.String("payload", msg),
			)
		} else {
			broker.connectionsLock.RLock()
			if cCode, ok := broker.connections[s.ID()]; ok {
				event := capture.Event{
					EventType: capture.Lobby,
					Payload:   []byte(msg),
				}
				err := broker.client.AddCaptureEvent(cCode, event)
				if err != nil {
					logger.Error("error sending lobby capture message to galactus",
						zap.Error(err),
						zap.String("socketID", s.ID()),
						zap.String("connectCode", cCode),
						zap.Int("eventType", int(event.EventType)),
						zap.ByteString("payload", event.Payload),
					)
				} else {
					logger.Info("successfully sent lobby capture event to galactus",
						zap.String("socketID", s.ID()),
						zap.String("connectCode", cCode),
						zap.Int("eventType", int(event.EventType)),
						zap.ByteString("payload", event.Payload),
					)
				}
			}
			broker.connectionsLock.RUnlock()
		}
	})
	server.OnEvent("/", "state", func(s socketio.Conn, msg string) {

		_, err := strconv.Atoi(msg)
		if err != nil {
			logger.Error("error unmarshalling state message",
				zap.Error(err),
				zap.String("socketID", s.ID()),
				zap.String("payload", msg),
			)
		} else {
			broker.connectionsLock.RLock()
			if cCode, ok := broker.connections[s.ID()]; ok {
				event := capture.Event{
					EventType: capture.State,
					Payload:   []byte(msg),
				}
				err := broker.client.AddCaptureEvent(cCode, event)
				if err != nil {
					logger.Error("error sending state capture message to galactus",
						zap.Error(err),
						zap.String("socketID", s.ID()),
						zap.String("connectCode", cCode),
						zap.Int("eventType", int(event.EventType)),
						zap.ByteString("payload", event.Payload),
					)
				} else {
					logger.Info("successfully sent state capture event to galactus",
						zap.String("socketID", s.ID()),
						zap.String("connectCode", cCode),
						zap.Int("eventType", int(event.EventType)),
						zap.ByteString("payload", event.Payload),
					)
				}
			}
			broker.connectionsLock.RUnlock()
		}
	})
	server.OnEvent("/", "player", func(s socketio.Conn, msg string) {

		// validation
		var player game.Player
		err := json.Unmarshal([]byte(msg), &player)
		if err != nil {
			logger.Error("error unmarshalling player message",
				zap.Error(err),
				zap.String("socketID", s.ID()),
				zap.String("connectCode", msg),
				zap.String("payload", msg),
			)
		} else {
			broker.connectionsLock.RLock()
			if cCode, ok := broker.connections[s.ID()]; ok {
				event := capture.Event{
					EventType: capture.Player,
					Payload:   []byte(msg),
				}
				err := broker.client.AddCaptureEvent(cCode, event)
				if err != nil {
					logger.Error("error sending player capture message to galactus",
						zap.Error(err),
						zap.String("socketID", s.ID()),
						zap.String("connectCode", cCode),
						zap.Int("eventType", int(event.EventType)),
						zap.ByteString("payload", event.Payload),
					)
				} else {
					logger.Info("successfully sent player capture event to galactus",
						zap.String("socketID", s.ID()),
						zap.String("connectCode", cCode),
						zap.Int("eventType", int(event.EventType)),
						zap.ByteString("payload", event.Payload),
					)
				}
			}
			broker.connectionsLock.RUnlock()
		}
	})
	server.OnEvent("/", "gameover", func(s socketio.Conn, msg string) {

		// TODO validate gameover message

		broker.connectionsLock.RLock()
		if cCode, ok := broker.connections[s.ID()]; ok {
			event := capture.Event{
				EventType: capture.GameOver,
				Payload:   []byte(msg),
			}
			err := broker.client.AddCaptureEvent(cCode, event)
			if err != nil {
				logger.Error("error sending gameover capture message to galactus",
					zap.Error(err),
					zap.String("socketID", s.ID()),
					zap.String("connectCode", cCode),
					zap.Int("eventType", int(event.EventType)),
					zap.ByteString("payload", event.Payload),
				)
			} else {
				logger.Info("successfully sent gameover capture event to galactus",
					zap.String("socketID", s.ID()),
					zap.String("connectCode", cCode),
					zap.Int("eventType", int(event.EventType)),
					zap.ByteString("payload", event.Payload),
				)
			}
		}
		broker.connectionsLock.RUnlock()
	})
	server.OnError("/", func(s socketio.Conn, e error) {
		logger.Error("socket error",
			zap.Error(e),
		)
	})
	server.OnDisconnect("/", func(s socketio.Conn, reason string) {
		logger.Info("client connection closed",
			zap.String("socketID", s.ID()),
			zap.String("reason", reason),
		)

		broker.connectionsLock.RLock()
		if cCode, ok := broker.connections[s.ID()]; ok {
			event := capture.Event{
				EventType: capture.Connection,
				Payload:   []byte("false"),
			}
			err := broker.client.AddCaptureEvent(cCode, event)
			if err != nil {
				logger.Error("error sending connection capture message to galactus",
					zap.Error(err),
					zap.String("socketID", s.ID()),
					zap.String("connectCode", cCode),
					zap.Int("eventType", int(event.EventType)),
					zap.ByteString("payload", event.Payload),
				)
			} else {
				logger.Info("successfully sent connection capture event to galactus",
					zap.String("socketID", s.ID()),
					zap.String("connectCode", cCode),
					zap.Int("eventType", int(event.EventType)),
					zap.ByteString("payload", event.Payload),
				)
			}
			server.ClearRoom("/", cCode)
		}
		broker.connectionsLock.RUnlock()

		broker.connectionsLock.Lock()
		if c, ok := broker.ackKillChannels[s.ID()]; ok {
			c <- true
		}
		delete(broker.ackKillChannels, s.ID())
		delete(broker.connections, s.ID())
		broker.connectionsLock.Unlock()
	})
	go server.Serve()
	defer server.Close()

	router := mux.NewRouter()
	router.Handle("/socket.io/", server)

	router.HandleFunc("/", func(w http.ResponseWriter, r *http.Request) {
		w.WriteHeader(http.StatusOK)
		w.Write([]byte("ok"))
	})
	logger.Info("message broker is now running",
		zap.String("port", port))

	err = http.ListenAndServe(":"+port, router)
	logger.Fatal("message broker terminated with error",
		zap.Error(err))
}
