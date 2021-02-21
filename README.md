# Portable AutoMuteUs For Windows

![GitHub release (latest by SemVer including pre-releases)](https://img.shields.io/github/downloads-pre/mtaku3/AutoMuteUs-Portable/latest/total?color=green&sort=semver)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/mtaku3/AutoMuteUs-Portable/releaser-v1)
![GitHub](https://img.shields.io/github/license/mtaku3/AutoMuteUs-Portable)

## What is this?
This is portable version of [AutoMuteUs(self-hosting)](https://github.com/automuteus). So that no need to install anything.

Note that this is not AmongUsCapture. this is its server side ✊

## Installation Guide
First of all, this supports only windows

1.  Download the latest version here <a href="https://github.com/mtaku3/AutoMuteUs-Portable/releases/latest/download/AutoMuteUs-Portable.exe"><img alt="GitHub release (latest SemVer including pre-releases)" src="https://img.shields.io/github/v/release/mtaku3/AutoMuteUs-Portable?color=blue&include_prereleases&label=download&sort=semver"></a>
2.  Create Discord Bot and get the token. Instruction [here](https://github.com/denverquane/automuteus/blob/master/BOT_README.md)
3.  Run `AutoMuteUs-Portable.exe`. `Settings` window will appear at first launch.
4.  Add your Discord Bot Token which you got in step No.2 in `DISCORD_BOT_TOKEN` setting. Then click save. 

🔔 After you clicked save button, all required binaries will be downloaded from <a href="#binary-repositories">Binary Repositories</a> in the `EnvPath` directory.

5.  Logs will appear in MainWindow's log text box. If you've followed the instruction correctly, now you get the Self-Hosted AutoMuteUs Bot. Congrats :partying_face:

## Binary Repositories
Required binaries will be downloaded from release of repositories below 👇

- automuteus.exe : https://github.com/AutoMuteUs-Portable/automuteus/releases
- galactus.exe : https://github.com/AutoMuteUs-Portable/galactus/releases
- wingman.exe : https://github.com/AutoMuteUs-Portable/wingman
- postgres.zip : https://github.com/AutoMuteUs-Portable/postgres/releases
- redis.zip : https://github.com/AutoMuteUs-Portable/redis/releases

postgres.zip and redis.zip will be extracted after loaded binaries.

wingman.exe will be downloaded if `ARCHITECTURE` setting is set to `v7`.