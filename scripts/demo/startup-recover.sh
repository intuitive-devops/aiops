#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$ROOT_DIR"

require_cmd() {
  if ! command -v "$1" >/dev/null 2>&1; then
    echo "[startup] missing required command: $1"
    exit 1
  fi
}

require_cmd minikube
require_cmd kubectl
require_cmd helm

echo "[startup] ensuring minikube is running..."
if ! minikube status --format '{{.Host}} {{.Kubelet}} {{.APIServer}}' 2>/dev/null | grep -Eq '^Running Running Running$'; then
  minikube start
else
  echo "[startup] minikube already running."
fi

echo "[startup] restoring monitoring stack..."
scripts/demo/setup-monitoring.sh

LOG_DIR="/tmp/bunnyhop"
mkdir -p "$LOG_DIR"

# Stop old forwards so reboot scripts are idempotent.
pkill -f "kubectl port-forward -n monitoring svc/kube-prometheus-stack-grafana 31000:80" >/dev/null 2>&1 || true
pkill -f "kubectl port-forward -n monitoring svc/kube-prometheus-stack-prometheus 9090:9090" >/dev/null 2>&1 || true

echo "[startup] starting Grafana port-forward..."
nohup kubectl port-forward -n monitoring svc/kube-prometheus-stack-grafana 31000:80 \
  >"$LOG_DIR/grafana-port-forward.log" 2>&1 &
echo $! >"$LOG_DIR/grafana-port-forward.pid"

echo "[startup] starting Prometheus port-forward..."
nohup kubectl port-forward -n monitoring svc/kube-prometheus-stack-prometheus 9090:9090 \
  >"$LOG_DIR/prometheus-port-forward.log" 2>&1 &
echo $! >"$LOG_DIR/prometheus-port-forward.pid"

echo "[startup] done."
echo "[startup] Grafana:    http://127.0.0.1:31000  (admin / bunnyhop)"
echo "[startup] Prometheus: http://127.0.0.1:9090"
echo "[startup] logs:       $LOG_DIR"
