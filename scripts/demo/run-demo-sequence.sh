#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$ROOT_DIR"

export AIOPS_LIFETIME="${AIOPS_LIFETIME:-2}"
export AIOPS_DURATION="${AIOPS_DURATION:-3000}"
export AIOPS_MATRIXLOOPS="${AIOPS_MATRIXLOOPS:-10000}"
export AIOPS_DECISION_LOG="${AIOPS_DECISION_LOG:-$ROOT_DIR/code-two/run/logs/decision-log.jsonl}"

echo "[demo] Running sequence with lifetime=${AIOPS_LIFETIME} min, duration=${AIOPS_DURATION} ms, matrixloops=${AIOPS_MATRIXLOOPS}"
echo "[demo] Decision log => ${AIOPS_DECISION_LOG}"

dotnet run --project code-two/run/run.csproj
