# Portable AutoMuteUs For Windows

![GitHub all releases](https://img.shields.io/github/downloads/mtaku3/AutoMuteUs-Portable/total?label=%E5%90%88%E8%A8%88%E3%83%80%E3%82%A6%E3%83%B3%E3%83%AD%E3%83%BC%E3%83%89%E6%95%B0)
![GitHub release (latest by SemVer including pre-releases)](https://img.shields.io/github/downloads-pre/mtaku3/AutoMuteUs-Portable/latest/total?label=%E6%9C%80%E6%96%B0%E7%89%88%E3%83%80%E3%82%A6%E3%83%B3%E3%83%AD%E3%83%BC%E3%83%89%E6%95%B0&sort=semver)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/mtaku3/AutoMuteUs-Portable/releaser-v2?label=%E3%83%93%E3%83%AB%E3%83%89)

## 重要なお知らせ

AutoMuteUs Portable は v3 から v4 のメジャーアップデートを計画しています。このアップデートではアーキテクチャの大幅な変更があるため v4 のアップデートと同時に v3.1.0 以前のバージョンは動作しなくなります。v3.1.0 以前のバージョンをご利用の方はお早めに v3.1.0 以降にアップデートするようお願いいたします。

## 概要

これは[AutoMuteUs(セルフホスト)](https://github.com/automuteus)のポータブル版です。 そのためインストールは不要です。

※ これは Among Us Capture ではありません。Among Us Capture のサーバーサイドです。 ✊

note に詳細の記事を作成しました！[こちら](https://note.com/mtaku3/n/nd1419c9138c7)もどうぞ。

## アップデート情報

[Twitter(@DYZdK2oir7Pm)](https://twitter.com/DYZdK2oir7Pm)にて随時アップデート情報を配信しています。是非フォローお願いいたします。

また、質問や不具合についてもお気軽にお問い合わせください。

## 注意

v2.2.4 以前のバージョンで、Postgres データベースが正常に初期化されないバグを発見しました。それにより、AutoMuteUs が正しくプレイヤー戦績を保存しません。

既に v2.2.5 にて修正済みですので、v2.2.5 以降のバージョンをお使いください。

また、v2.2.4 以前のバージョンを使用していた方は、Postgres の初期化をおすすめします。

### Among Us Capture と AutoMuteUs の違い

- Among Us Capture: Among Us からプレイヤーの名前などを読み取り、サーバーに送信するアプリケーション
- AutoMuteUs: Among Us Capture から受信したデータを元に、Discord のミュート等を実行するアプリケーション

AutoMuteUs は現在のバージョンでは、複数のコンポーネントで構成されている上、インストールには`Docker`のインストールが必要でした。しかし、この`AutoMuteUs Portable`では、Windows の`.exe`ファイル 1 つの実行のみで、これらの複数の必要コンポーネントを自動で実行します。

## インストール手順

note に図解付き徹底解説作成しました！[こちら](https://note.com/mtaku3/n/n07fc4a2e9617)もどうぞ。

まず初めに、これは Windows 専用です。

1.  最新版をダウンロードしてください。 <a href="https://github.com/mtaku3/AutoMuteUs-Portable/releases/latest/download/AutoMuteUs-Portable-x64.exe"><img alt="GitHub release (latest SemVer)" src="https://img.shields.io/github/v/release/mtaku3/AutoMuteUs-Portable?label=%E3%83%80%E3%82%A6%E3%83%B3%E3%83%AD%E3%83%BC%E3%83%89&sort=semver"></a>

    32 ビット版のダウンロードリンクは[こちら](https://github.com/mtaku3/AutoMuteUs-Portable/releases/latest/download/AutoMuteUs-Portable-x86.exe)

2.  Discord ボットを作成してください。 手順は[こちら(英語)](https://github.com/denverquane/automuteus/blob/master/BOT_README.md)
3.  `AutoMuteUs-Portable.exe`を起動してください。 最初の起動では、`Settings`画面が立ち上がります。
4.  Windows Defender の注意画面が出たら、`詳細情報`を押した後、`実行`をクリックしてください。
5.  `Settings`画面の`DISCORD_BOT_TOKEN`欄に手順 2 で取得したトークンを入力してください。
6.  `Settings`画面の`POSTGRES_PASS`欄に好きなパスワードを入力してください。※ パスワードは空にはできません。
7.  `Settings`画面、下の`save`ボタンを押してください。

🔔 save ボタンを押した後、必要なファイルが<a href="#バイナリレポジトリ">バイナリレポジトリ</a>からダウンロードされます。

8.  手順通りに操作ができれば、メイン画面のログテキストボックスにたくさんのログが現れます。AutoMuteUs ボットが実行され、利用可能になります。 おめでとう!! 🥳

## 上手くいかない場合

トラブルがあれば、リセット機能をお試しください 👍

1.  `AutoMuteUs-Portable.exe`を起動してください。 `AutoMuteUs Portable`という名前のメイン画面が立ち上がります。
2.  メイン画面の右下にある`RESET`ボタンをクリックしてください。 `Reset`画面が現れます。
3.  `RESET ALL PROPERTIES`と`DELETE ALL BINARIES`を押してください。 それぞれのボタンを押した後、メッセージボックスが立ち上がりますが、注意書きですので、`Yes` ▶️ `OK` と進めてください。
4.  `Reset`画面の`X`ボタンを押して、画面を閉じてください。 自動的にアプリが終了します。
5.  `AutoMuteUs-Portable.exe`をもう一度起動してください。 <a href="#インストール手順">インストール手順</a>に従って再設定してください。

## バイナリレポジトリ

必要なファイルは以下のレポジトリからダウンロードされます。 👇

- automuteus.exe : https://github.com/AutoMuteUs-Portable/old.automuteus/releases
- galactus.exe : https://github.com/AutoMuteUs-Portable/old.galactus/releases
- wingman.exe : https://github.com/AutoMuteUs-Portable/old.wingman/releases
- postgres.zip : https://github.com/AutoMuteUs-Portable/old.postgres/releases
- redis.zip : https://github.com/AutoMuteUs-Portable/old.redis/releases

postgres.zip と redis.zip はダウンロード後解凍されます。

wingman.exe は`ARCHITECTURE`が`v7`の場合のみダウンロードされます。
