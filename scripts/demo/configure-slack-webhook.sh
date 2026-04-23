#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$ROOT_DIR"

if ! command -v kubectl >/dev/null 2>&1; then
  echo "kubectl is required."
  exit 1
fi

if [ "${1:-}" = "" ]; then
  cat <<'EOF'
Usage:
  scripts/demo/configure-slack-webhook.sh <slack-webhook-url>

Example:
  scripts/demo/configure-slack-webhook.sh 'https://hooks.slack.com/services/T000/B000/XXXX'
EOF
  exit 1
fi

SLACK_WEBHOOK_URL="$1"

echo "[slack] Creating/updating secret: monitoring/bunnyhop-secrets"
kubectl -n monitoring create secret generic bunnyhop-secrets \
  --from-literal=slack_webhook_url="$SLACK_WEBHOOK_URL" \
  --dry-run=client -o yaml | kubectl apply -f -

echo "[slack] Restarting demo API deployment to pick up new env var..."
kubectl -n monitoring rollout restart deployment/bunnyhop-demo-api
kubectl -n monitoring rollout status deployment/bunnyhop-demo-api --timeout=180s

echo "[slack] Done. bunnyhop alerts can now forward to Slack."
