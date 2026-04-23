#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$ROOT_DIR"

if ! command -v helm >/dev/null 2>&1; then
  echo "helm is required. Install helm first: https://helm.sh/docs/intro/install/"
  exit 1
fi

if ! command -v kubectl >/dev/null 2>&1; then
  echo "kubectl is required."
  exit 1
fi

echo "[monitoring] Ensuring namespace exists..."
kubectl create namespace monitoring --dry-run=client -o yaml | kubectl apply -f -

echo "[monitoring] Installing kube-prometheus-stack..."
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts >/dev/null 2>&1 || true
helm repo update >/dev/null
helm upgrade --install kube-prometheus-stack prometheus-community/kube-prometheus-stack \
  --namespace monitoring \
  --values deployment/monitoring/kube-prometheus-stack.values.yaml

echo "[monitoring] Applying bunnyhop dashboard configmap..."
kubectl apply -f deployment/monitoring/bunnyhop-grafana-dashboard.configmap.yaml

echo "[monitoring] Building/deploying bunnyhop demo API in-cluster..."
scripts/demo/deploy-demo-api-in-cluster.sh

echo "[monitoring] Waiting for Grafana rollout..."
kubectl -n monitoring rollout status deployment/kube-prometheus-stack-grafana --timeout=180s

echo "[monitoring] Monitoring stack ready."
echo "- Grafana user: admin"
echo "- Grafana password: bunnyhop"
echo "- Start port-forward: scripts/demo/open-monitoring.sh"
