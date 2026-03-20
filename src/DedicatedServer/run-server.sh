#!/bin/bash
# ONI Dedicated Server launcher
# Uses dotnet test runner as host (required for Harmony on Mono)
set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"

# Auto-detect x86_64 Mono and dotnet
MONO_HOME="${MONO_HOME:-$HOME/.mono-x64/6.12.0}"
DOTNET_X64="${DOTNET_X64:-$HOME/.dotnet-x64/dotnet}"

if [ ! -f "$MONO_HOME/bin/mono" ]; then
    echo "ERROR: x86_64 Mono not found at $MONO_HOME"
    echo "Install: https://www.mono-project.com/download/stable/"
    exit 1
fi

if [ ! -f "$DOTNET_X64" ]; then
    echo "ERROR: x86_64 dotnet not found at $DOTNET_X64"
    exit 1
fi

# Symlink SimDLL if not present
OUTPUT_DIR="$SCRIPT_DIR/bin/Debug/net48"
if [ ! -f "$OUTPUT_DIR/libSimDLL.dylib" ]; then
    SIM_DLL="$HOME/Library/Application Support/Steam/steamapps/common/OxygenNotIncluded/OxygenNotIncluded.app/Contents/PlugIns/SimDLL.bundle/Contents/MacOS/SimDLL"
    if [ -f "$SIM_DLL" ]; then
        ln -sf "$SIM_DLL" "$OUTPUT_DIR/libSimDLL.dylib"
        echo "SimDLL symlinked"
    else
        echo "WARNING: SimDLL not found — physics will be disabled"
    fi
fi

echo "Starting ONI Dedicated Server..."
export MONO_HOME PATH="$MONO_HOME/bin:$PATH"
exec arch -x86_64 "$DOTNET_X64" test "$SCRIPT_DIR/DedicatedServer.csproj" \
    --filter ServerBoot --no-build -c Debug
