👉 <a href="https://github.com/mtaku3/AutoMuteUs-Portable/blob/main/README_ja.md">日本語</a>

# Portable AutoMuteUs For Windows

![GitHub release (latest by SemVer including pre-releases)](https://img.shields.io/github/downloads-pre/mtaku3/AutoMuteUs-Portable/latest/total?color=green&sort=semver)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/mtaku3/AutoMuteUs-Portable/releaser-v1)

## What is this?
This is portable version of [AutoMuteUs(self-hosting)](https://github.com/automuteus). So that no need to install anything.

Note that this is not AmongUsCapture. this is its server side ✊

## Caution
Binaries has been updated for newer AutoMuteUs Portable version. So that AutoMuteUs Portable under v2.0.0 doesn't work properly.

Please use later than v2.1.0

## Installation Guide
First of all, this supports only windows

1.  Download the latest version here <a href="https://github.com/mtaku3/AutoMuteUs-Portable/releases/latest/download/AutoMuteUs-Portable-x64.exe"><img alt="GitHub release (latest SemVer including pre-releases)" src="https://img.shields.io/github/v/release/mtaku3/AutoMuteUs-Portable?color=blue&include_prereleases&label=download&sort=semver"></a>
2.  Create Discord Bot and get the token. Instruction [here](https://github.com/denverquane/automuteus/blob/master/BOT_README.md)
3.  Run `AutoMuteUs-Portable.exe`. `Settings` window will appear at first launch.
4.  If you see Windows Defender caution, click `More Info` and `Run Anyway`.
5.  Write your Discord Bot Token which you got in step No.2 in `DISCORD_BOT_TOKEN` setting.
6.  Write password whatever you want for Postgres in `POSTGRES_PASS` setting. Password can't be empty.
7.  Click `save` button.

🔔 After you clicked save button, all required binaries will be downloaded from <a href="#binary-repositories">Binary Repositories</a> in the `EnvPath` directory.

8.  Logs will appear in MainWindow's log text box. If you've followed the instruction correctly, now you get the Self-Hosted AutoMuteUs Bot. Congrats :partying_face:

## Something went wrong?
If there's some trouble, try RESET Feature 👍

1.  Run `AutoMuteUs-Portable.exe`. MainWindow named `AutoMuteUs Portable` will appear.
2.  Click `RESET` button at right bottom of MainWindow. `Reset` Window will appear.
3.  Click `RESET ALL PROPERTIES` and `DELETE ALL BINARIES`. Follow the instructions of message box appeared.
4.  After that, close `Reset` window by clicking `X` button of the window.
5.  Run `AutoMuteUs-Portable.exe` again. Follow <a href="#installation-guide">Installation Guide</a>

## Binary Repositories
Required binaries will be downloaded from release of repositories below 👇

- automuteus.exe : https://github.com/AutoMuteUs-Portable/automuteus/releases
- galactus.exe : https://github.com/AutoMuteUs-Portable/galactus/releases
- wingman.exe : https://github.com/AutoMuteUs-Portable/wingman
- postgres.zip : https://github.com/AutoMuteUs-Portable/postgres/releases
- redis.zip : https://github.com/AutoMuteUs-Portable/redis/releases

postgres.zip and redis.zip will be extracted after loaded binaries.

wingman.exe will be downloaded if `ARCHITECTURE` setting is set to `v7`.