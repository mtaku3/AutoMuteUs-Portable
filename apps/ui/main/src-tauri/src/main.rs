// Prevents additional console window on Windows in release, DO NOT REMOVE!!
#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

mod ipc;

use ipc::connect_to_ui_ipc_server;
use tauri::Manager;
use tokio::sync::mpsc;

#[tokio::main]
async fn main() {
  env_logger::init();
  let (tx, mut rx) = mpsc::channel::<String>(100);
  let rpc = connect_to_ui_ipc_server(&tx).await.unwrap();

  tauri::Builder::default()
    .setup(|app| {
      let main_window = app.get_window("main").unwrap();
      tokio::spawn(async move {
        loop {
          if let Some(msg) = rx.recv().await {
            main_window
              .emit("test", msg)
              .expect("failed to emit message");
          }
        };
      });

      Ok(())
    })
    .run(tauri::generate_context!())
    .expect("error while running tauri application");
}
