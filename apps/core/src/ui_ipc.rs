use amup_core_ui_ipc_intf::{UI_IPC_SERVER_PATH, UI_IPC_HOST_NAME};
use anyhow::Result;
use busrt::{broker::{Broker, ServerConfig}, rpc::{RpcClient, RpcHandlers}, client::AsyncClient, QoS, async_trait};

pub struct UIIPCServer {
    broker: Broker,
}

impl UIIPCServer {
    pub fn new() -> Self {
        Self {
            broker: Broker::new(),
        }
    }

    pub async fn start(&mut self) -> Result<RpcClient> {
        let server_config = ServerConfig::default();
        #[cfg(unix)]
        self.broker.spawn_unix_server(UI_IPC_SERVER_PATH, server_config).await?;
        #[cfg(windows)]
        self.broker.spawn_tcp_server(UI_IPC_SERVER_PATH, server_config).await?;

        let mut client = self.broker.register_client(UI_IPC_HOST_NAME).await?;
        client.subscribe("#", QoS::Realtime).await?;

        let handlers = UIIPCHandlers {};
        let rpc = RpcClient::new(client, handlers);
        Ok(rpc)
    }
}

struct UIIPCHandlers {}

#[async_trait]
impl RpcHandlers for UIIPCHandlers {}
