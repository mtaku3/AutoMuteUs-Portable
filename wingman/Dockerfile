FROM golang:1.15-alpine AS builder

# Git is required for getting the dependencies.
# hadolint ignore=DL3018
RUN apk add --no-cache git

WORKDIR /src

# Fetch dependencies first; they are less susceptible to change on every build
# and will therefore be cached for speeding up the next build
COPY ./go.mod ./go.sum ./
RUN go mod download

# Import the code from the context.
COPY ./ ./

# Build the executable to `/app`. Mark the build as statically linked.
# hadolint ignore=SC2155
RUN CGO_ENABLED=0 \
    go build -installsuffix 'static' \
    -o /app ./cmd/main.go

FROM alpine:3.12.1 AS final

# Set up non-root user and app directory
# * Non-root because of the principle of least privlege
# * App directory to allow mounting volumes
RUN addgroup -g 1000 wingman && \
    adduser -HD -u 1000 -G wingman wingman && \
    mkdir /app && \
    chown -R wingman:wingman /app
USER wingman

# Import the compiled executable from the first stage.
COPY --from=builder /app /app

# Default socket.io port
EXPOSE 8123

# Run the compiled binary.
ENTRYPOINT ["/app/app"]