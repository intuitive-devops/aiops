#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$ROOT_DIR"

export AIOPS_DECISION_LOG="${AIOPS_DECISION_LOG:-$ROOT_DIR/code-two/run/logs/decision-log.jsonl}"
export AIOPS_API_URLS="${AIOPS_API_URLS:-http://127.0.0.1:8787}"

echo "[demo-api] Serving ${AIOPS_DECISION_LOG}"
echo "[demo-api] URL: ${AIOPS_API_URLS}"

dotnet run --project code-two/demo-api/demo-api.csproj --urls "$AIOPS_API_URLS"
