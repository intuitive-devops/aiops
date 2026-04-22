## Podman Desktop Kubernetes (kind-compatible)

Use this repo with a local Kubernetes cluster from Podman Desktop.

### 1) Verify cluster context

- `kubectl config current-context`
- `kubectl get nodes`

### 2) Deploy AiOps namespace and baseline workloads

- `kubectl create namespace aiops --dry-run=client -o yaml | kubectl apply -f -`
- `kubectl apply -f aiops-noise.configmap.yaml`
- `kubectl apply -f aiops-level-1.deployment.yaml`
- `kubectl apply -f aiops-level-1.service.yaml`
- `kubectl apply -f diagnostics.yaml -n aiops`
- `kubectl get pods -n aiops`

### 3) Optional monitoring commands

- `kubectl --namespace monitoring get pods -l "release=kube-prometheus-stack"`
- `kubectl port-forward -n monitoring svc/kube-prometheus-stack-prometheus 9090:9090`
- `kubectl port-forward -n monitoring svc/kube-prometheus-stack-grafana 31000:80`

### 4) Cleanup

- `kubectl delete namespace aiops`
