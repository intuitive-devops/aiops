# bunnyhop Monitoring (Grafana + Kubernetes Dashboard)

This folder contains a ready monitoring setup for local clusters (minikube):

- `kube-prometheus-stack.values.yaml`
  - enables Grafana dashboard sidecar loading from ConfigMaps
  - adds Prometheus scrape target for local demo API metrics (`host.minikube.internal:8787/metrics`)
- `bunnyhop-aiops-dashboard.json`
  - 4 panels for demo KPIs and decision telemetry
- `bunnyhop-grafana-dashboard.configmap.yaml`
  - makes the dashboard auto-import into Grafana

## Setup

From repo root:

```bash
scripts/demo/setup-monitoring.sh
```

Start the demo API in a separate shell so Prometheus can scrape decision metrics:

```bash
scripts/demo/run-demo-api.sh
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
