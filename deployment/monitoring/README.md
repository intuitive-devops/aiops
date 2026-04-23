# bunnyhop Monitoring (Grafana + Kubernetes Dashboard)

This folder contains a ready monitoring setup for local clusters (minikube):

- `kube-prometheus-stack.values.yaml`
  - enables Grafana dashboard sidecar loading from ConfigMaps
  - adds Prometheus scrape target for demo API metrics (`192.168.49.2:8787/metrics`)
  - adds bunnyhop Prometheus alert rules and Alertmanager webhook routing
- `bunnyhop-aiops-dashboard.json`
  - operations + executive panels and incident replay trigger
- `bunnyhop-grafana-dashboard.configmap.yaml`
  - makes the dashboard auto-import into Grafana
- `bunnyhop-demo-api.deployment.yaml`
  - deploys the demo API in-cluster on host network for resilient local scraping
  - supports optional Slack forwarding via `AIOPS_SLACK_WEBHOOK_URL`

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
- Executive SLO Risk
- Executive Recommended Action
- Executive Confidence
- Executive Time Saved
- Incident Replay Trigger

## Alerting + Slack

- Alert rules:
  - `BunnyhopAnomalyActive`
  - `BunnyhopSloRiskHigh`
  - `BunnyhopReplayBurst`
- Alertmanager sends webhook notifications to demo API endpoint:
  - `http://192.168.49.2:8787/api/alerts`
- Optional Slack forwarding:
  - configure Kubernetes secret and rollout restart:
  - `scripts/demo/configure-slack-webhook.sh 'https://hooks.slack.com/services/T000/B000/XXXX'`
