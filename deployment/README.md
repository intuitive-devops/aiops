## Launching containers to a kind k8s

* `kubectl apply -f diagnostics.yaml -n aiops`
* `kubectl exec -it diagnostics -n aiops -- bash`

More.

podman run -it --rm

podman run --name pdm-nginx -p 8080:80 nginx