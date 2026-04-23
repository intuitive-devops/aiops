# bunnyhop aiops startup-readme

## Purpose

This document summarizes the implementation and startup analysis for the `bunnyhop aiops` demo environment.

## What is now implemented

- Deterministic demo runner with decision logging (`code-two/run`).
- Demo HTTP API exposing:
  - `GET /health`
  - `GET /api/status`
  - `GET /api/decisions`
  - `GET /metrics` (Prometheus format for decision telemetry)
- Cloudflare Tunnel scripts for publishing the demo API to `bunnyhop.work`.
- Local monitoring bundle with:
  - `kube-prometheus-stack` install values
  - prebuilt Grafana dashboard (`bunnyhop aiops overview`)
  - dashboard auto-import ConfigMap
- Kubernetes integration test namespace handling fixed for both quoted/unquoted YAML namespace values.

## Startup sequence (local)

1. Confirm local Kubernetes context:
   - `kubectl config current-context`
   - `kubectl get nodes`
2. Run demo sequence:
   - `scripts/demo/run-demo-sequence.sh`
3. Run demo API:
   - `scripts/demo/run-demo-api.sh`
4. Install monitoring stack:
   - `scripts/demo/setup-monitoring.sh`
5. Open monitoring endpoints:
   - `scripts/demo/open-monitoring.sh`
6. Optional public exposure:
   - `scripts/cloudflare/setup-tunnel.sh bunnyhop-demo bunnyhop.work http://127.0.0.1:8787`
   - `scripts/cloudflare/run-tunnel.sh bunnyhop-demo`

## Monitoring analysis

- Grafana is the primary customer-facing view (KPIs + mitigation story).
- Kubernetes Dashboard is best used as an operator/health/troubleshooting view.
- Prometheus scrapes the local demo API metrics from `host.minikube.internal:8787/metrics`.
- Included dashboard panels:
  - Workload CPU Trend
  - Anomaly Window
  - Mitigation Decision Confidence
  - Decision Volume by State/Action

## Minikube findings

- Common startup failures were driver/rootless mismatches and missing contexts.
- Working state confirmed when:
  - `minikube status` shows host/kubelet/apiserver `Running`
  - `kubectl get nodes` shows `minikube` in `Ready`
- Integration test now passes against running minikube when `RUN_K8S_INTEGRATION=1`.

## Validation summary

- `dotnet build code-two/demo-api/demo-api.csproj` passed.
- `/metrics` endpoint returns valid Prometheus-formatted metrics.
- Monitoring scripts pass shell syntax checks.
- Kubernetes integration test passed after namespace rewrite fix.

## Known practical dependencies

- `helm` required for monitoring setup script.
- `cloudflared` required for tunnel setup.
- A running local cluster context (`minikube` in current workflow) is required before Kubernetes integration tests.
