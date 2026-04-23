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
  - in-cluster demo API deployment for metrics (`deployment/monitoring/bunnyhop-demo-api.deployment.yaml`)
- Kubernetes integration test namespace handling fixed for both quoted/unquoted YAML namespace values.

## Startup sequence (local)

1. Confirm local Kubernetes context:
   - `kubectl config current-context`
   - `kubectl get nodes`
2. Run demo sequence:
   - `scripts/demo/run-demo-sequence.sh`
3. Install monitoring stack:
   - `scripts/demo/setup-monitoring.sh`
4. Open monitoring endpoints:
   - `scripts/demo/open-monitoring.sh`
5. Optional public exposure:
   - `scripts/cloudflare/setup-tunnel.sh bunnyhop-demo bunnyhop.work http://127.0.0.1:8787`
   - `scripts/cloudflare/run-tunnel.sh bunnyhop-demo`

## Monitoring analysis

- Grafana is the primary customer-facing view (KPIs + mitigation story).
- Kubernetes Dashboard is best used as an operator/health/troubleshooting view.
- Prometheus scrapes demo API metrics from `192.168.49.2:8787/metrics` (host-network fallback for unstable local pod DNS/networking).
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

## Troubleshooting no-data / high fan noise

- Symptom: Grafana panels show "No data", Prometheus target is `DOWN`, and machine fan runs hot.
- Observed root cause in this environment: pod-to-pod networking and cluster DNS intermittently timeout.
- Implemented mitigation:
  - deployed `bunnyhop-demo-api` with `hostNetwork: true`
  - switched Prometheus target to node IP `192.168.49.2:8787`
  - reduced synthetic CPU noise loop intensity in `deployment/aiops-level-1.deployment.yaml`
- Quick verification commands:
  - `kubectl get pods -A`
  - `kubectl -n monitoring exec prometheus-kube-prometheus-stack-prometheus-0 -c prometheus -- wget -qO- 'http://127.0.0.1:9090/api/v1/query?query=up%7Bjob%3D%22bunnyhop-demo-api%22%7D'`
  - `kubectl -n monitoring exec prometheus-kube-prometheus-stack-prometheus-0 -c prometheus -- wget -qO- 'http://127.0.0.1:9090/api/v1/query?query=sum%20by%20(state%2Caction)%20(bunnyhop_decision_total)'`

## Troubleshooting Grafana datasource 400 errors

- Symptom: Grafana dashboard opens, but panel requests fail and Grafana logs show `POST /api/ds/query status=400`.
- Observed root cause in this environment: Grafana pod cannot reliably resolve or reach the in-cluster Prometheus `ClusterIP` service.
- Implemented mitigation:
  - expose Prometheus service as NodePort `30090`
  - configure Grafana datasource URL to `http://192.168.49.2:30090/`
- Verification commands:
  - `kubectl -n monitoring get svc kube-prometheus-stack-prometheus`
  - `kubectl -n monitoring get configmap kube-prometheus-stack-grafana-datasource -o yaml | rg 'url:'`
  - `kubectl -n monitoring exec deploy/kube-prometheus-stack-grafana -c grafana -- wget -qO- --timeout=5 http://192.168.49.2:30090/-/ready`

## Validation summary

- `dotnet build code-two/demo-api/demo-api.csproj` passed.
- `/metrics` endpoint returns valid Prometheus-formatted metrics.
- Monitoring scripts pass shell syntax checks.
- Kubernetes integration test passed after namespace rewrite fix.

## Known practical dependencies

- `helm` required for monitoring setup script.
- `cloudflared` required for tunnel setup.
- A running local cluster context (`minikube` in current workflow) is required before Kubernetes integration tests.
