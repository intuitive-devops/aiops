# Customer Demo Runbook (Podman Desktop Kubernetes)

## Goal

Show a full mini-loop on Kubernetes:
1. Start/verify a local cluster in Podman Desktop (kind-compatible).
2. Deploy demo workloads.
3. Run agent sequence.
4. Produce mitigation recommendation.

## Preconditions

- Podman Desktop with Kubernetes enabled.
- `kubectl` configured to Podman Desktop context.
- .NET SDK installed (`dotnet --version`).
- Local cluster can pull required images, or images are preloaded.

## Step 1: Verify Kubernetes context

From repo root:

```bash
kubectl config current-context
kubectl get nodes
```

Expected:
- A local Podman Desktop Kubernetes context.
- At least one node in `Ready` state.

## Step 2: Deploy baseline manifests

```bash
kubectl create namespace aiops --dry-run=client -o yaml | kubectl apply -f -
kubectl apply -f deployment/aiops-noise.configmap.yaml
kubectl apply -f deployment/aiops-level-1.deployment.yaml
kubectl apply -f deployment/aiops-level-1.service.yaml
kubectl apply -f deployment/diagnostics.yaml -n aiops
kubectl get pods -n aiops
```

Expected:
- `aipos-level-1` pod running.
- `diagnostics` pod running.

## Step 3: Configure fast demo settings

Edit `code-two/run/config/Settings.xml` for demo speed:

- `lifetime`: `2`
- `duration`: `3000`
- `matrixloops`: `10000`

## Step 4: Build and run the sequence

```bash
dotnet build code-two/run/run.csproj -v minimal
dotnet run --project code-two/run/run.csproj
```

Expected console states:

- "Beginning my sequences."
- computation task activation
- agent spawn/task messages

## Step 5: Customer-facing narration

- "This demo is running on a local Kubernetes cluster in Podman Desktop."
- "The agent watches runtime behavior and moves through a decision sequence."
- "In production mode, this can drive recommendation-only or guarded auto-remediation."

## Step 6: Clean up

```bash
kubectl delete namespace aiops
```

## Optional: Podman Compose fallback (non-Kubernetes)

Use this only for local component testing:

```bash
cd code-two/scripts
podman compose -f compose.yml up -d
podman ps
podman compose -f compose.yml down
```

## Troubleshooting

- `ImagePullBackOff` in Kubernetes:
  - Verify image references in deployment YAML.
  - Push images to a registry your local cluster can access, or preload images into the cluster runtime.
- Build failure in `run.csproj`:
  - Confirm `code-two/agent/agent.csproj` does not reference `../encog/encog.csproj`.
- Pod scheduling issues:
  - Remove cloud-specific tolerations for local demos if they block scheduling.
