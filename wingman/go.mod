module github.com/automuteus/wingman

go 1.15

require (
	github.com/automuteus/galactus v1.2.2
	github.com/automuteus/utils v0.0.13
	github.com/googollee/go-socket.io v1.4.4
	github.com/gorilla/mux v1.8.0
	go.uber.org/zap v1.16.0
)

// TODO replace when V7 comes out
replace github.com/automuteus/galactus v1.2.2 => github.com/automuteus/galactus v1.2.3-0.20210209052631-1bd854dca0cf
