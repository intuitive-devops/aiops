# AiOps Product And Demo Plan

## 1) What this repo can already support

- Simulate noisy workload behavior (`network-noise`, `code-two/matrix`).
- Collect metrics with Prometheus/cAdvisor on a Kubernetes cluster running in Podman Desktop (kind-compatible).
- Run a .NET orchestration sequence ("whirl") that triggers compute/agent actions (`code-two/run/Program.cs`).
- Deploy baseline workloads to Kubernetes (`deployment/*.yaml`).

This is enough to build a credible "v1 customer demo" if we package it around one clear outcome.

## 2) Product definition (v1)

### Product name (working)

- Cartheur AiOps Autopilot

### One-line value proposition

- Detect abnormal workload behavior and recommend (or apply) safer runtime settings before incidents escalate.

### Ideal first customer

- Platform/SRE teams running Kubernetes clusters with recurring CPU or latency incidents.
- Teams without dedicated ML staff that still want practical optimization signals.

### Narrow v1 use case

- "Noise overload protection": detect sustained high CPU/noise behavior and trigger a controlled response recommendation.

## 3) Demo outcome to sell

### Before

- Workload shows unstable/high-noise utilization.

### During

- Metrics are ingested and analyzed.
- Agent sequence produces a decision.

### After

- Customer sees either:
1. A recommendation ("reduce loop intensity / adjust deployment values"), or
2. An automated config change in a controlled namespace.

Keep the story around "faster mean time to mitigation" rather than "general AI."

## 4) 10-minute live demo structure

1. Problem framing (1 min): recurring noisy workload incidents in K8s.
2. Environment bring-up (2 min): start demo stack, show workload + metrics.
3. Detection and analysis (3 min): run sequence, show metrics trend and agent state transition.
4. Decision and action (2 min): show recommended or applied change.
5. Business close (2 min): expected impact, integration path, next pilot milestone.

## 5) MVP backlog (customer-ready)

### P0 (must-have)

- Single command startup script for local demo.
- Podman Desktop Kubernetes (kind-compatible) startup/teardown workflow with `kubectl`.
- Stable deterministic demo data path (no surprises in live session).
- One dashboard view (Grafana or CLI summary) with 3 KPIs:
  - workload CPU trend
  - anomaly window
  - mitigation decision
- Decision log artifact saved to file (`timestamp`, `signal`, `decision`, `confidence`, `action`).
- README rewrite focused on customer scenario, not internal notes.

### P1 (high-value)

- "recommend-only" and "auto-apply" modes.
- Helm chart values patch generation from decision output.
- Basic HTTP API for status and decision history.

### P2 (later)

- Multi-workload support.
- Policy guardrails and approval workflow.
- Tenant-aware dashboards.

## 6) Commercial packaging for customer conversations

### Offer

- 2-week pilot in one namespace, one workload class.

### Success criteria

- Detect and flag target anomaly class.
- Produce actionable decision in less than 2 minutes from threshold breach.
- Demonstrate at least one successful mitigation loop in controlled conditions.

### Suggested pilot outputs

- Pilot report (incident timeline + recommendations).
- Operational runbook.
- Rollout proposal for production guardrailed mode.

## 7) Immediate next steps (this repo)

1. Create a deterministic demo script that starts stack, runs sequence, and captures outputs.
2. Add a small decision log writer in `code-two/run`.
3. Add a concise "Customer Demo Quickstart" to README.
4. Rehearse a 10-minute script with strict timing.
