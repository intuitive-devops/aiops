# bunnyhop Monitoring (Grafana + Kubernetes Dashboard)

This folder contains a ready monitoring setup for local clusters (minikube):

- `kube-prometheus-stack.values.yaml`
  - enables Grafana dashboard sidecar loading from ConfigMaps
  - adds Prometheus scrape target for demo API metrics (`192.168.49.2:8787/metrics`)
- `bunnyhop-aiops-dashboard.json`
  - 4 panels for demo KPIs and decision telemetry
- `bunnyhop-grafana-dashboard.configmap.yaml`
  - makes the dashboard auto-import into Grafana
- `bunnyhop-demo-api.deployment.yaml`
  - deploys the demo API in-cluster on host network for resilient local scraping

## Setup

From repo root:

```bash
scripts/demo/setup-monitoring.sh
```

Open monitoring views:

```bash
scripts/demo/open-monitoring.sh
```

## Dashboard panels

- Workload CPU Trend
- Anomaly Window
- Mitigation Decision Confidence
- Decision Volume by State/Action
