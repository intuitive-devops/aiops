# bunnyhop aiops

Customer-facing demo stack for an AiOps mitigation loop:
1. Run deterministic sequence (`code-two/run`)
2. Persist decision artifacts (`decision-log.jsonl`)
3. Expose decision status over HTTP (`code-two/demo-api`)
4. Publish the API securely via Cloudflare Tunnel + DNS

## Prerequisites

- .NET SDK 9+
- `kubectl` (optional for Kubernetes portion)
- `cloudflared` (for public DNS exposure)

## Quickstart (local)

From repo root:

```bash
scripts/demo/run-demo-sequence.sh
```

This writes decision events to:

`code-two/run/logs/decision-log.jsonl`

In a second shell, run the API:

```bash
scripts/demo/run-demo-api.sh
```

API endpoints:
- `GET /health`
- `GET /api/status`
- `GET /api/decisions?limit=25`

Default local URL:

`http://127.0.0.1:8787/api/status`

## Cloudflare Tunnel + DNS

1. Authenticate `cloudflared` once:

```bash
cloudflared tunnel login
```

2. Create/update tunnel config and DNS route:

```bash
scripts/cloudflare/setup-tunnel.sh bunnyhop-demo bunnyhop.work http://127.0.0.1:8787
```

3. Run the tunnel:

```bash
scripts/cloudflare/run-tunnel.sh bunnyhop-demo
```

Your DNS hostname (`bunnyhop.work`) will route to the local demo API while the tunnel is running.

## Monitoring (Grafana + Kubernetes Dashboard)

Bring up the monitoring stack in your minikube cluster:

```bash
scripts/demo/setup-monitoring.sh
```

In another shell, run the demo API so Prometheus can scrape decision metrics:

```bash
scripts/demo/run-demo-api.sh
```

Then open Grafana/Prometheus and Kubernetes Dashboard:

```bash
scripts/demo/open-monitoring.sh
```

Grafana dashboard included: `bunnyhop aiops overview`  
Grafana login: `admin` / `bunnyhop`

## Demo tuning via env vars

Override runtime behavior without editing XML:

```bash
AIOPS_LIFETIME=2 AIOPS_DURATION=3000 AIOPS_MATRIXLOOPS=10000 scripts/demo/run-demo-sequence.sh
```

Override log location:

```bash
AIOPS_DECISION_LOG=/tmp/decision-log.jsonl scripts/demo/run-demo-sequence.sh
```

## Legacy notes

Original planning and delivery docs remain in:
- `PRODUCT_DEMO_PLAN.md`
- `DEMO_RUNBOOK.md`
