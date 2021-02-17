package broker

import (
	"context"
	"encoding/json"
	socketio "github.com/googollee/go-socket.io"
	"go.uber.org/zap"
)

// tasksListener is a worker that waits for incoming mute/deafen tasks for a particular game (HTTP long-polling with Galactus)
// This worker broadcasts the task to the Socket.io room associated with the connectcode, to see if any Capture Bots can
// issue the task. This worker doesn't care about the response; the response is handled/passed on by the socket.io server
// itself in broker.go
func (broker *Broker) tasksListener(server *socketio.Server, connectCode string, killchan <-chan bool) {
	broker.logger.Info("client is now listening for mute/deafen tasks",
		zap.String("connectCode", connectCode),
	)
	ctx, cancelRequest := context.WithCancel(context.Background())

	go func() {
		<-killchan
		broker.logger.Info("stopping task listener for client",
			zap.String("connectCode", connectCode),
		)
		cancelRequest()
		return
	}()

	for {
		task, err := broker.client.GetCaptureTask(ctx, connectCode)
		if err != nil {
			broker.logger.Error("error fetching capture task from galactus",
				zap.Error(err),
				zap.String("connectCode", connectCode),
			)
			// TODO fix the return error being non-nil when there are no tasks available (it's not an "error")
		} else if task != nil {
			jBytes, err := json.Marshal(task)
			if err != nil {
				broker.logger.Error("failed to marshal task to json with error",
					zap.Error(err),
					zap.String("connectCode", connectCode),
				)
			} else {
				broker.logger.Info("broadcasting task message to room",
					zap.ByteString("message", jBytes),
					zap.String("connectCode", connectCode),
				)
				server.BroadcastToRoom("/", connectCode, "modify", jBytes)
			}
		}
	}
}

//
//// ackWorker functions as a healthcheck for the bot, if the bot resumes a game (by connectcode) when it starts up after
//// being down/offline. This worker receives the ack, and, if the connection is still active, responds with a
//// connection=true message. If the client has terminated the connection, then this worker is also terminated, and thus
//// the bot never receives a healthy response for the connectcode/game in question
//func (broker *Broker) ackWorker(ctx context.Context, connCode string, killChan <-chan bool) {
//	pubsub := task.AckSubscribe(ctx, broker.client, connCode)
//	channel := pubsub.Channel()
//	defer pubsub.Close()
//
//	for {
//		select {
//		case <-killChan:
//			return
//		case <-channel:
//			err := task.PushJob(ctx, broker.client, connCode, task.ConnectionJob, "true")
//			if err != nil {
//				log.Println(err)
//			}
//			break
//		}
//	}
//}
