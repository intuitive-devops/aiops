#!/usr/bin/env bash
set -euo pipefail

if [[ $# -lt 2 || $# -gt 3 ]]; then
  echo "Usage: $0 <tunnel-name> <hostname> [origin-url]"
  echo "Example: $0 bunnyhop-demo bunnyhop.work http://127.0.0.1:8787"
  exit 1
fi

TUNNEL_NAME="$1"
HOSTNAME="$2"
ORIGIN_URL="${3:-http://127.0.0.1:8787}"

if ! command -v cloudflared >/dev/null 2>&1; then
  echo "cloudflared is not installed. Install it first: https://developers.cloudflare.com/cloudflare-one/connections/connect-networks/downloads/"
  exit 1
fi

echo "[cloudflare] Ensuring tunnel exists: ${TUNNEL_NAME}"
CREATE_OUTPUT="$(cloudflared tunnel create "$TUNNEL_NAME" 2>&1 || true)"

TUNNEL_ID="$(echo "$CREATE_OUTPUT" | grep -Eo '[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}' | head -n 1 || true)"
if [[ -z "$TUNNEL_ID" ]]; then
  TUNNEL_ID="$(cloudflared tunnel list | awk -v tunnel="$TUNNEL_NAME" '$0 ~ tunnel {print $1; exit}')"
fi

if [[ -z "$TUNNEL_ID" ]]; then
  echo "Could not determine tunnel id for ${TUNNEL_NAME}."
  echo "Output from cloudflared tunnel create:"
  echo "$CREATE_OUTPUT"
  echo "Tip: run 'cloudflared tunnel login' first, then re-run this script."
  exit 1
fi

CONFIG_DIR="${HOME}/.cloudflared"
CONFIG_PATH="${CONFIG_DIR}/config-${TUNNEL_NAME}.yml"
CREDENTIALS_PATH="${CONFIG_DIR}/${TUNNEL_ID}.json"

mkdir -p "$CONFIG_DIR"

cat > "$CONFIG_PATH" <<CFG
tunnel: ${TUNNEL_ID}
credentials-file: ${CREDENTIALS_PATH}

ingress:
  - hostname: ${HOSTNAME}
    service: ${ORIGIN_URL}
  - service: http_status:404
CFG

echo "[cloudflare] Creating DNS route: ${HOSTNAME}"
cloudflared tunnel route dns "$TUNNEL_NAME" "$HOSTNAME"

echo "[cloudflare] Tunnel configured."
echo "[cloudflare] Config file: ${CONFIG_PATH}"
echo "[cloudflare] Start with: cloudflared tunnel --config ${CONFIG_PATH} run ${TUNNEL_NAME}"
