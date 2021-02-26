# Portable AutoMuteUs For Windows

![GitHub release (latest by SemVer including pre-releases)](https://img.shields.io/github/downloads-pre/mtaku3/AutoMuteUs-Portable/latest/total?color=green&sort=semver)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/mtaku3/AutoMuteUs-Portable/releaser-v1)

## 概要
これは[AutoMuteUs(セルフホスト)](https://github.com/automuteus)のポータブル版です。 そのためインストールは不要です。

※ これはAmong Us Captureではありません。Among Us Captureのサーバーサイドです。 ✊

noteに詳細の記事を作成しました！[こちら](https://note.com/mtaku3/n/nd1419c9138c7)もどうぞ。

## 注意

全てのバイナリを最新版に修正したため、AutoMuteUs Portableはv2.0.0以下のバージョンでは動作しません。

v2.1.0以降をご使用ください。

### Among Us CaptureとAutoMuteUsの違い
- Among Us Capture: Among Usからプレイヤーの名前などを読み取り、サーバーに送信するアプリケーション
- AutoMuteUs: Among Us Captureから受信したデータを元に、Discordのミュート等を実行するアプリケーション

AutoMuteUsは現在のバージョンでは、複数のコンポーネントで構成されている上、インストールには`Docker`のインストールが必要でした。しかし、この`AutoMuteUs Portable`では、Windowsの`.exe`ファイル1つの実行のみで、これらの複数の必要コンポーネントを自動で実行します。

## インストール手順
noteに図解付き徹底解説作成しました！[こちら](https://note.com/mtaku3/n/n07fc4a2e9617)もどうぞ。

まず初めに、これはWindows専用です。

1.  最新版をダウンロードしてください。 <a href="https://github.com/mtaku3/AutoMuteUs-Portable/releases/latest/download/AutoMuteUs-Portable-x64.exe"><img alt="GitHub release (latest SemVer including pre-releases)" src="https://img.shields.io/github/v/release/mtaku3/AutoMuteUs-Portable?color=blue&include_prereleases&label=download&sort=semver"></a>
2.  Discordボットを作成してください。 手順は[こちら(英語)](https://github.com/denverquane/automuteus/blob/master/BOT_README.md)
3.  `AutoMuteUs-Portable.exe`を起動してください。 最初の起動では、`Settings`画面が立ち上がります。
4.  Windows Defenderの注意画面が出たら、`詳細情報`を押した後、`実行`をクリックしてください。
5.  `Settings`画面の`DISCORD_BOT_TOKEN`欄に手順2で取得したトークンを入力してください。
6.  `Settings`画面の`POSTGRES_PASS`欄に好きなパスワードを入力してください。※ パスワードは空にはできません。
7.  `Settings`画面、下の`save`ボタンを押してください。

🔔 saveボタンを押した後、必要なファイルが<a href="#バイナリレポジトリ">バイナリレポジトリ</a>からダウンロードされます。

8.  手順通りに操作ができれば、メイン画面のログテキストボックスにたくさんのログが現れます。AutoMuteUsボットが実行され、利用可能になります。 おめでとう!! 🥳

## 上手くいかない場合
トラブルがあれば、リセット機能をお試しください 👍

1.  `AutoMuteUs-Portable.exe`を起動してください。 `AutoMuteUs Portable`という名前のメイン画面が立ち上がります。
2.  メイン画面の右下にある`RESET`ボタンをクリックしてください。 `Reset`画面が現れます。
3.  `RESET ALL PROPERTIES`と`DELETE ALL BINARIES`を押してください。 それぞれのボタンを押した後、メッセージボックスが立ち上がりますが、注意書きですので、`Yes` ▶️ `OK` と進めてください。
4.  `Reset`画面の`X`ボタンを押して、画面を閉じてください。 自動的にアプリが終了します。
5.  `AutoMuteUs-Portable.exe`をもう一度起動してください。 <a href="#インストール手順">インストール手順</a>に従って再設定してください。

## バイナリレポジトリ
必要なファイルは以下のレポジトリからダウンロードされます。 👇

- automuteus.exe : https://github.com/AutoMuteUs-Portable/automuteus/releases
- galactus.exe : https://github.com/AutoMuteUs-Portable/galactus/releases
- wingman.exe : https://github.com/AutoMuteUs-Portable/wingman
- postgres.zip : https://github.com/AutoMuteUs-Portable/postgres/releases
- redis.zip : https://github.com/AutoMuteUs-Portable/redis/releases

postgres.zipとredis.zipはダウンロード後解凍されます。

wingman.exeは`ARCHITECTURE`が`v7`の場合のみダウンロードされます。