pub mod commands;
pub mod notifications;

#[cfg(unix)]
pub const UI_IPC_SERVER_PATH: &str = "/tmp/amup_core_ui_ipc.sock";
#[cfg(windows)]
pub const UI_IPC_SERVER_PATH: &str = "127.0.0.1:7777";

pub const UI_IPC_HOST_NAME: &str = "host";

pub const UI_IPC_CLIENT_NAME: &str = "client";
