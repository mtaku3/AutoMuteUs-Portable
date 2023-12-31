use amup_core_ui_ipc_intf::{UI_IPC_SERVER_PATH, UI_IPC_CLIENT_NAME};
use anyhow::Result;
use busrt::{ipc as busrt_ipc, client::AsyncClient, QoS, rpc::{RpcClient, RpcHandlers, RpcEvent, Rpc}, async_trait};
use tokio::sync::mpsc::Sender;

pub async fn connect_to_ui_ipc_server(tx: &Sender<String>) -> Result<RpcClient> {
  let config = busrt_ipc::Config::new(UI_IPC_SERVER_PATH,UI_IPC_CLIENT_NAME);
  let mut client = busrt_ipc::Client::connect(&config).await?;
  client.subscribe("#", QoS::Realtime).await?;
  let handlers = UIIPCHandlers {
    tx: tx.clone(),
  };
  let rpc = RpcClient::new(client, handlers);
  Ok(rpc)
}

struct UIIPCHandlers {
  tx: Sender<String>,
}

#[async_trait]
impl RpcHandlers for UIIPCHandlers {
  async fn handle_notification(&self, event: RpcEvent) {
    let payload = std::str::from_utf8(event.payload()).unwrap().to_string();
    self.tx.send(payload).await.unwrap();
  }
}
