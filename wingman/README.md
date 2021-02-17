# wingman
Helping your capture client find the perfect match

## Description
Wingman is the externally-facing Socket.io broker for automuteus.

Wingman receives messages from your capture client, and relays that information to Galactus directly (over HTTP).

Wingman also coordinates sending mute/deafen requests (that are provided by Galactus) to any available capture clients
that are running a Discord bot that can be used for mute/deafen.

## Environment Variables

### Required

- `GALACTUS_ADDR`: The host and port at which your Galactus is accessible. Ex: `http://localhost:5858`

### Optional
- `WINGMAN_PORT`: The port at which Wingman can be reached. This is the externally-facing Socket.io port that capture clients will connect to.
Defaults to 8123.
