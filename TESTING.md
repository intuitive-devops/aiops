# Testing Strategy

## Purpose

Validate both engineering quality and customer-demo effectiveness.

## Test layers

1. Unit tests
- Target deterministic orchestration and decision-support logic.
- Current scope: whirl state transitions, task idempotency, API URL builders.

2. Integration tests
- Validate Podman Desktop Kubernetes deployment flow.
- Verify manifests apply cleanly and core pods become ready.

3. Demo effectiveness checks
- Measure operational value shown to prospects:
  - detection latency
  - decision consistency across repeated runs
  - false positive rate during normal load

## Unit test project

- Project: `code-two/tests/Cartheur.Demo.Tests/Cartheur.Demo.Tests.csproj`
- Test files:
  - `WhirlAndTasksTests.cs`
  - `UrlBuilderTests.cs`
  - `KpiAssertionsTests.cs`

Run:

```bash
dotnet test code-two/tests/Cartheur.Demo.Tests/Cartheur.Demo.Tests.csproj -v minimal
```

## Kubernetes integration checklist (manual)

1. `kubectl config current-context`
2. `kubectl get nodes`
3. `kubectl create namespace aiops --dry-run=client -o yaml | kubectl apply -f -`
4. `kubectl apply -f deployment/aiops-noise.configmap.yaml`
5. `kubectl apply -f deployment/aiops-level-1.deployment.yaml`
6. `kubectl apply -f deployment/aiops-level-1.service.yaml`
7. `kubectl get pods -n aiops`
8. `kubectl delete namespace aiops`

## Kubernetes integration tests (automated, opt-in)

- Project: `code-two/tests/Cartheur.Demo.IntegrationTests/Cartheur.Demo.IntegrationTests.csproj`
- Test file:
  - `KubernetesDeploymentIntegrationTests.cs`
- Enable with:
  - `RUN_K8S_INTEGRATION=1`
- Optional repo root override:
  - `AIOPS_REPO_ROOT=/absolute/path/to/repo`

Run:

```bash
RUN_K8S_INTEGRATION=1 dotnet test code-two/tests/Cartheur.Demo.IntegrationTests/Cartheur.Demo.IntegrationTests.csproj -v minimal
```

## Quality gate script

- Script: `scripts/quality-gate.sh`
- Enforces:
  - unit test pass
  - coverage thresholds for `run/` and `agent/` code
  - optional Kubernetes integration test execution

Run:

```bash
./scripts/quality-gate.sh
```

## Release gate for customer demos

- Unit tests pass.
- Kubernetes manifest deploy/teardown verified in Podman Desktop.
- One rehearsed 10-minute run recorded.
- Decision output captured and archived for the rehearsal.
