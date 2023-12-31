use std::fs::{self, File};
use std::path::PathBuf;
use std::process::exit;
use std::sync::{Arc, mpsc};
use std::sync::atomic::{AtomicBool, Ordering};

use abi_stable::std_types::RResult::RErr;
use abi_stable::{library::RootModule, std_types::RResult::ROk};
use amup_core_comp_mod_intf::ComponentMod_Ref;
use anyhow::{Result, Context};
use busrt::client::AsyncClient;
use busrt::rpc::{RpcClient, Rpc};
use clap::{Parser, Subcommand};
use daemonize::{Daemonizer, DaemonBomb};
use fd_lock::RwLock;
use tokio::runtime::Runtime;
use tokio::signal;
use ui_ipc::UIIPCServer;

mod ui_ipc;
mod daemonize;

#[derive(Parser)]
struct Cli {
    #[arg(long, short = 'c', value_name = "FILE", help = "Path to config file")]
    config: Option<PathBuf>,

    #[arg(long, help = "Run in the foreground")]
    no_daemon: bool,
}

#[tokio::main]
async fn main() {
    let cli = Cli::parse();
    env_logger::init();

    let daemonizer = Daemonizer::new();
    if daemonizer.is_running().unwrap() {
        panic!("Daemon is already running");
    }
    if !cli.no_daemon {
        daemonizer.spawn_daemon().unwrap();
    } else {
        daemonizer.i_am_daemon().unwrap();
    }
    let _daemon_bomb = daemonizer.get_bomb();

    let mut ui_ipc_server = UIIPCServer::new();
    let rpc = ui_ipc_server.start().await.unwrap();

    let (tx, rx) = mpsc::channel();

    ctrlc::set_handler(move || {
        let _ = tx.send(0);
    }).context("Couldn't set Ctrl-C handler").unwrap();

    loop {
        rpc.notify(amup_core_ui_ipc_intf::UI_IPC_CLIENT_NAME, "123".as_bytes().into(), busrt::QoS::Realtime).await;
        if rx.try_recv().is_ok() {
            break;
        }
        tokio::time::sleep(tokio::time::Duration::from_millis(1000)).await;
    }
}

// fn main() {
//     let mods = ComponentMod_Ref::load_from_file(
//         "../component_modules/automuteus/target/debug/libautomuteus.so".as_ref(),
//     )
//     .unwrap_or_else(|e| panic!("{}", e));
//     let mut comp = mods.new()();
//     let config = toml::from_str(
//         r#"
//         discord_bot_token = "token"
//
//         [components.automuteus]
//         port = 5000
//     "#,
//     )
//     .unwrap();
//     match comp.start(&config) {
//         ROk(_) => (),
//         RErr(_) => (),
//     };
//     println!("Success!");
// }
