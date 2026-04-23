#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$ROOT_DIR"

echo "[demo-api-cluster] Building image in minikube daemon..."
minikube image build -t bunnyhop/demo-api:local -f code-two/demo-api/Dockerfile .

echo "[demo-api-cluster] Applying deployment/service..."
kubectl apply -f deployment/monitoring/bunnyhop-demo-api.deployment.yaml
kubectl -n monitoring rollout status deployment/bunnyhop-demo-api --timeout=180s

echo "[demo-api-cluster] Running pod:"
kubectl get pods -n monitoring -l app=bunnyhop-demo-api
