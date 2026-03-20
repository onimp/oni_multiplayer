#!/bin/bash
# ONI Dedicated Server launcher
#
# Required environment:
#   ONI_STREAMING_ASSETS — path to game's StreamingAssets directory
#   MONO_HOME           — path to x86_64 Mono installation (default: ~/.mono-x64/6.12.0)
#
# Usage: ./run-server.sh [port]
set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
OUTPUT_DIR="$SCRIPT_DIR/bin/Debug/net48"
MONO_HOME="${MONO_HOME:-$HOME/.mono-x64/6.12.0}"
MONO="$MONO_HOME/bin/mono"

if [ -z "$ONI_STREAMING_ASSETS" ]; then
    echo "ERROR: ONI_STREAMING_ASSETS not set"
    echo "Set it to the game's StreamingAssets path, e.g.:"
    echo "  export ONI_STREAMING_ASSETS=~/Library/Application\\ Support/Steam/.../StreamingAssets"
    exit 1
fi

if [ ! -f "$MONO" ]; then
    echo "ERROR: x86_64 Mono not found at $MONO"
    exit 1
fi

# Build ServerLauncher if not present
if [ ! -f "$OUTPUT_DIR/ServerLauncher.exe" ]; then
    echo "Building ServerLauncher..."
    arch -x86_64 "$MONO" "$MONO_HOME/lib/mono/msbuild/Current/bin/Roslyn/csc.exe" \
        "$SCRIPT_DIR/ServerLauncher.cs" /out:"$OUTPUT_DIR/ServerLauncher.exe" /target:exe
fi

export MONO_HOME PATH="$MONO_HOME/bin:$PATH"
exec arch -x86_64 "$MONO" "$OUTPUT_DIR/ServerLauncher.exe" "$@"
