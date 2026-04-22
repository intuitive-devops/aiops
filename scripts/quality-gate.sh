#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

UNIT_TEST_PROJECT="code-two/tests/Cartheur.Demo.Tests/Cartheur.Demo.Tests.csproj"
INTEGRATION_TEST_PROJECT="code-two/tests/Cartheur.Demo.IntegrationTests/Cartheur.Demo.IntegrationTests.csproj"

RUN_MIN_COVERAGE="${RUN_MIN_COVERAGE:-40.0}"
AGENT_MIN_COVERAGE="${AGENT_MIN_COVERAGE:-0.3}"

echo "[quality-gate] Running unit tests with coverage..."
dotnet test "$UNIT_TEST_PROJECT" --collect:"XPlat Code Coverage" -v minimal

COVERAGE_FILE="$(find code-two/tests/Cartheur.Demo.Tests/TestResults -name coverage.cobertura.xml -printf '%T@ %p\n' | sort -nr | head -1 | cut -d' ' -f2-)"
if [[ -z "${COVERAGE_FILE}" || ! -f "${COVERAGE_FILE}" ]]; then
  echo "[quality-gate] ERROR: coverage.cobertura.xml not found."
  exit 1
fi

METRICS="$(sed -n 's/.*filename="\([^"]*\)"[^>]*line-rate="\([0-9.]*\)".*/\1 \2/p' "$COVERAGE_FILE" | awk '{f=$1;r=$2+0;if(index(f,"run/")==1){sr+=r;cr++} if(index(f,"agent/")==1){sa+=r;ca++}} END{printf("%.4f %.4f %d %d", cr?100*sr/cr:0, ca?100*sa/ca:0, cr, ca)}')"

RUN_COVERAGE="$(echo "$METRICS" | awk '{print $1}')"
AGENT_COVERAGE="$(echo "$METRICS" | awk '{print $2}')"
RUN_CLASS_COUNT="$(echo "$METRICS" | awk '{print $3}')"
AGENT_CLASS_COUNT="$(echo "$METRICS" | awk '{print $4}')"

echo "[quality-gate] run/ average class line coverage: ${RUN_COVERAGE}% (${RUN_CLASS_COUNT} classes)"
echo "[quality-gate] agent/ average class line coverage: ${AGENT_COVERAGE}% (${AGENT_CLASS_COUNT} classes)"

awk -v actual="$RUN_COVERAGE" -v required="$RUN_MIN_COVERAGE" 'BEGIN{ if (actual+0 < required+0) exit 1 }' || {
  echo "[quality-gate] ERROR: run/ coverage ${RUN_COVERAGE}% is below threshold ${RUN_MIN_COVERAGE}%"
  exit 1
}

awk -v actual="$AGENT_COVERAGE" -v required="$AGENT_MIN_COVERAGE" 'BEGIN{ if (actual+0 < required+0) exit 1 }' || {
  echo "[quality-gate] ERROR: agent/ coverage ${AGENT_COVERAGE}% is below threshold ${AGENT_MIN_COVERAGE}%"
  exit 1
}

if [[ "${RUN_K8S_INTEGRATION:-0}" == "1" ]]; then
  echo "[quality-gate] Running Kubernetes integration tests..."
  dotnet test "$INTEGRATION_TEST_PROJECT" -v minimal
else
  echo "[quality-gate] Skipping Kubernetes integration tests (set RUN_K8S_INTEGRATION=1 to enable)."
fi

echo "[quality-gate] PASS"
