#!/usr/bin/env bash
set -euo pipefail

echo "[monitoring] Grafana: http://127.0.0.1:31000 (admin / bunnyhop)"
echo "[monitoring] Prometheus: http://127.0.0.1:9090"
echo "[k8s-dashboard] starting via minikube (this prints a URL)..."

kubectl port-forward -n monitoring svc/kube-prometheus-stack-grafana 31000:80 >/tmp/bunnyhop-grafana-port-forward.log 2>&1 &
GRAFANA_PID=$!

kubectl port-forward -n monitoring svc/kube-prometheus-stack-prometheus 9090:9090 >/tmp/bunnyhop-prometheus-port-forward.log 2>&1 &
PROM_PID=$!

cleanup() {
  kill "$GRAFANA_PID" "$PROM_PID" >/dev/null 2>&1 || true
}
trap cleanup EXIT INT TERM

minikube dashboard --url
wait
