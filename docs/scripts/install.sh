#!/bin/bash
cd "$(dirname "$0")"

if [[ "$OSTYPE" == "darwin"* ]]; then
    # macOS
    MOD_PATH=~/Library/Application\ Support/unity.Klei.Oxygen\ Not\ Included/mods/Local/MultiplayerMod
else
    # Linux
    MOD_PATH=~/.config/unity3d/Klei/Oxygen\ Not\ Included/mods/Local/MultiplayerMod
fi

mkdir -p "$MOD_PATH"
cp -r ./MultiplayerMod/. "$MOD_PATH"
