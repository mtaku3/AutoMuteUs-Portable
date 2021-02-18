# Portable AutoMuteUs For Windows

![GitHub release (latest by SemVer including pre-releases)](https://img.shields.io/github/downloads-pre/mtaku3/AutoMuteUs-Portable/latest/total?color=green&sort=semver)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/mtaku3/AutoMuteUs-Portable/releaser-v1)
![GitHub](https://img.shields.io/github/license/mtaku3/AutoMuteUs-Portable)

### What is this?
This is portable version of [AutoMuteUs(self-hosting)](https://github.com/automuteus). So that no need to install anything.

### Installation Guide
First of all, this supports only windows

1.  Download the latest version here <a href="https://github.com/mtaku3/AutoMuteUs-Portable/releases/latest/download/AutoMuteUs-Portable.zip"><img alt="GitHub release (latest SemVer including pre-releases)" src="https://img.shields.io/github/v/release/mtaku3/AutoMuteUs-Portable?color=blue&include_prereleases&label=download&sort=semver"></a>
2.  Extract the zip downloaded in step No.1 wherever you want.
3.  Create Discord Bot and get the token. Instruction [here](https://github.com/denverquane/automuteus/blob/master/BOT_README.md)
4.  Go into the folder which named `AutoMuteUs-Portable` and is extracted, then open up `.env` file.
5.  Add your Discord Bot Token which you got in step No.4 after `DISCORD_BOT_TOKEN=`. Something like this `DISCORD_BOT_TOKEN=kjdiosajdiosjaiojfxcmniok903iu219034uj920#21`. Then save and close the file.
6.  Run `run.bat` which is inside the folder.
7.  Many CLI(Command Prompt) will appear. If you've followed the instruction correctly, now you get the Self-Hosted AutoMuteUs Bot. Congrats :partying_face:

At the first time to run `run.bat`, `initdb.bat` will be run automatically too. So you don't need to run `initdb.bat` manually.

### Wait.. How to close the application..
Just focus Command Prompt then `Ctrl+C` to exit the CLI.
Repeat for all Command Prompts.

## Structure
This supports automuteus-v7 architecture.

Used binaries below.

- automuteus.exe: built using [here](https://github.com/denverquane/automuteus)
- galactus.exe: built using [here](https://github.com/automuteus/galactus)
- wingman.exe: built using [here](https://github.com/automuteus/wingman)
- postgres: binary using [here](https://www.enterprisedb.com/download-postgresql-binaries)
- redis: binary using [here](https://github.com/tporadowski/redis)