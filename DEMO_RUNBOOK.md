# Customer Demo Runbook (Kubernetes + Cloudflare)

## Goal

Show a full mini-loop:
1. Start local demo sequence.
2. Publish KPI/decision history via HTTP API.
3. Expose the API on a customer-facing DNS hostname through Cloudflare Tunnel.

## Preconditions

- .NET SDK installed (`dotnet --version`)
- Optional Kubernetes context if showing cluster objects
- Cloudflare account with zone for your hostname
- `cloudflared` installed and authenticated

## Step 1: Run deterministic sequence

```bash
scripts/demo/run-demo-sequence.sh
```

Expected:
- Console shows whirl state transitions.
- Decision artifacts are appended to `code-two/run/logs/decision-log.jsonl`.

## Step 2: Start the demo API

In a second terminal:

```bash
scripts/demo/run-demo-api.sh
```

Validate locally:

```bash
curl -s http://127.0.0.1:8787/api/status
curl -s "http://127.0.0.1:8787/api/decisions?limit=5"
```

## Step 3: Create tunnel + DNS route

Authenticate once (interactive browser flow):

```bash
cloudflared tunnel login
```

Create tunnel config and DNS CNAME route:

```bash
scripts/cloudflare/setup-tunnel.sh bunnyhop-demo bunnyhop.work http://127.0.0.1:8787
```

Expected:
- Tunnel exists in Cloudflare.
- DNS route for `bunnyhop.work` exists.
- Config file created at `~/.cloudflared/config-bunnyhop-demo.yml`.

## Step 4: Run tunnel

In a third terminal:

```bash
scripts/cloudflare/run-tunnel.sh bunnyhop-demo
```

Then verify publicly:

```bash
curl -s https://bunnyhop.work/api/status
```

## Optional Kubernetes context checks

```bash
kubectl config current-context
kubectl get nodes
kubectl get pods -n aiops
```

## Troubleshooting

- `cloudflared tunnel create` fails:
  - Re-run `cloudflared tunnel login` and ensure you selected the correct zone.
- Public hostname returns 404:
  - Confirm `scripts/cloudflare/run-tunnel.sh` is still running.
  - Confirm ingress host in `~/.cloudflared/config-<tunnel-name>.yml` matches DNS hostname.
- API has no data:
  - Run `scripts/demo/run-demo-sequence.sh` first, then call `/api/status` again.
