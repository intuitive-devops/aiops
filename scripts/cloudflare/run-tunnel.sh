#!/usr/bin/env bash
set -euo pipefail

if [[ $# -ne 1 ]]; then
  echo "Usage: $0 <tunnel-name>"
  echo "Example: $0 bunnyhop-demo"
  exit 1
fi

TUNNEL_NAME="$1"
CONFIG_PATH="${HOME}/.cloudflared/config-${TUNNEL_NAME}.yml"

if [[ ! -f "$CONFIG_PATH" ]]; then
  echo "Missing config: ${CONFIG_PATH}"
  echo "Run scripts/cloudflare/setup-tunnel.sh first."
  exit 1
fi

cloudflared tunnel --config "$CONFIG_PATH" run "$TUNNEL_NAME"
