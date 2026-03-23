#!/usr/bin/env bash
# deploy-server.sh — Full DedicatedServer deploy pipeline.
#
# Run this after every git pull that touches DedicatedServer or web-client.
# It always rebuilds both frontend and backend so bundle hashes never drift.
#
# Usage:
#   ./scripts/deploy-server.sh          # build only, do not restart server
#   ./scripts/deploy-server.sh --restart # build + kill existing server + launch new one
#
# The script must be run from the repo root.

set -euo pipefail
REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$REPO_ROOT"

BINDIR="src/DedicatedServer/bin/Debug/net48"
WEBCLIENT="src/DedicatedServer/web-client"
DOTNET="$HOME/.dotnet/dotnet"
MONO="$HOME/.mono-x64/6.12.0/bin/mono"
SERVER_LOG="/private/tmp/dedicated-server.log"
ONI_STREAMING_ASSETS="$HOME/Library/Application Support/Steam/steamapps/common/OxygenNotIncluded/OxygenNotIncluded.app/Contents/Resources/Data/StreamingAssets"

RESTART=false
for arg in "$@"; do
  [[ "$arg" == "--restart" ]] && RESTART=true
done

echo "=== [1/4] Frontend build (npm run build → wwwroot) ==="
# vite.config.ts: outDir = '../wwwroot', emptyOutDir = true
# This always regenerates bundle hashes so index.html stays in sync.
(cd "$WEBCLIENT" && npm run build)

echo ""
echo "=== [2/4] .NET build (OniMod.sln Debug) ==="
"$DOTNET" build OniMod.sln -c Debug

echo ""
echo "=== [3/4] Restore CoreModule.dll (DedicatedServer project rebuild) ==="
# Full solution build zeroes out UnityEngine.CoreModule.dll in the output dir.
# Rebuilding just DedicatedServer.csproj restores the 1.3 MB file from lib/exposed.
"$DOTNET" build src/DedicatedServer/DedicatedServer.csproj -c Debug

echo ""
echo "=== [4/4] PatchInternalCalls (Cecil InternalCall stubs → UnityRuntime) ==="
"$DOTNET" run --project src/PatchInternalCalls/PatchInternalCalls.csproj -- \
  "$BINDIR/UnityEngine.CoreModule.dll" \
  "$BINDIR/UnityEngine.CoreModule.dll" \
  "$BINDIR/DedicatedServer.exe" \
  "$BINDIR/Assembly-CSharp.dll"

echo ""
echo "=== Deploy complete ==="
echo "  Frontend : built → $BINDIR/../wwwroot"
echo "  Backend  : $BINDIR/DedicatedServer.exe"
echo "  Patched  : $BINDIR/UnityEngine.CoreModule.dll + Assembly-CSharp.dll"

if [[ "$RESTART" == "true" ]]; then
  echo ""
  echo "=== Restarting server ==="
  # Kill existing server by finding the ServerLauncher mono process
  EXISTING_PID=$(pgrep -f "mono.*ServerLauncher.exe" || true)
  if [[ -n "$EXISTING_PID" ]]; then
    echo "  Killing PID $EXISTING_PID"
    kill "$EXISTING_PID"
    sleep 2
  fi

  rm -f "$SERVER_LOG"
  echo "  Launching ServerLauncher.exe → $SERVER_LOG"
  ONI_STREAMING_ASSETS="$ONI_STREAMING_ASSETS" \
    arch -x86_64 "$MONO" "$BINDIR/ServerLauncher.exe" \
    > "$SERVER_LOG" 2>&1 &
  NEW_PID=$!
  echo "  New PID: $NEW_PID"

  # Wait up to 45s for "World ready"
  for i in $(seq 1 45); do
    sleep 1
    if grep -qa "World ready" "$SERVER_LOG" 2>/dev/null; then
      echo "  ✓ World ready (${i}s)"
      break
    fi
    if ! kill -0 "$NEW_PID" 2>/dev/null; then
      echo "  ✗ Server exited early — check $SERVER_LOG"
      exit 1
    fi
  done
fi
