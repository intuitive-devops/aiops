## Launching containers to a kind k8s

* `kubectl apply -f diagnostics.yaml -n aiops`
* `kubectl exec -it diagnostics -n aiops -- bash`

More.

podman run -it --rm

podman run --name pdm-nginx -p 8080:80 nginx

`kubectl --namespace monitoring get pods -l "release=kube-prometheus-stack"`

`kubectl port-forward -n monitoring svc/kube-prometheus-stack-prometheus 9090:9090`

kubectl port-forward svc/kube-prometheus-stack-prometheus -n monitoring 9090:9090 --address=0.0.0.0 &
kubectl port-forward svc/kube-prometheus-stack-grafana -n monitoring 31000:80 --address=0.0.0.0 &

podman run --name prometheus -d -p 127.0.0.1:9090:9090 prom/prometheus