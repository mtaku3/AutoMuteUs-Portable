use std::{
    fs::File,
    io::Write,
    path::PathBuf,
    process::{exit, Command},
};

use anyhow::{anyhow, Context, Result};
use fork::daemon;
#[cfg(windows)]
use std::os::windows::process::CommandExt;

pub struct Daemonizer {
    pid_file: PathBuf,
}

impl Daemonizer {
    pub fn new() -> Self {
        Self {
            pid_file: std::env::temp_dir().join("amup_core.pid"),
        }
    }

    pub fn is_running(&self) -> Result<bool> {
        Ok(self.pid_file.try_exists()?)
    }

    pub fn spawn_daemon(&self) -> Result<()> {
        if self.is_running()? {
            Err(anyhow!("Daemon is already running"))?
        }

        #[cfg(unix)]
        {
            match daemon(false, false) {
                Ok(fork::Fork::Parent(pid)) => {
                    self.create_pidfile(pid)?;
                    exit(0);
                },
                Err(_) => Err(anyhow!("Couldn't spawn daemon process"))?,
                _ => {},
            }
        }
        #[cfg(windows)]
        {
            let mut args = std::env::args().collect::<Vec<_>>();
            args.remove(0);
            args.push("--no-daemon".to_string());
            let mut proc = Command::new(std::env::current_exe().unwrap())
                .args(args)
                .creation_flags(8 /* DETACHED_PROCESS */)
                .spawn()
                .context("Couldn't spawn daemon process")?;
            let pid = proc.id;
            self.create_pidfile(pid)?;
            exit(0);
        }

        Ok(())
    }

    pub fn i_am_daemon(&self) -> Result<()> {
        if self.is_running()? {
            Err(anyhow!("Daemon is already running"))?
        }

        let pid = std::process::id() as i32;
        self.create_pidfile(pid).unwrap();
        Ok(())
    }

    fn create_pidfile(&self, pid: i32) -> Result<()> {
        let mut file = File::create(self.pid_file.as_path())?;
        write!(file, "{}", pid)?;
        Ok(())
    }

    pub fn get_bomb(&self) -> DaemonBomb {
        self.into()
    }
}

pub struct DaemonBomb {
    pid_file: PathBuf,
}

impl From<&Daemonizer> for DaemonBomb {
    fn from(value: &Daemonizer) -> Self {
        Self {
            pid_file: value.pid_file.clone(),
        }
    }
}

impl Drop for DaemonBomb {
    fn drop(&mut self) {
        std::fs::remove_file(self.pid_file.as_path()).unwrap();
    }
}
