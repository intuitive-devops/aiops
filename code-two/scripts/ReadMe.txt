podman compose -f compose.yml up

Query commands of interest to the curiosity-problem

rate(container_cpu_usage_seconds_total{name="noise"}[1m])

Getting the metrics into time-series format?
https://prometheus.io/docs/prometheus/latest/querying/api/#tsdb-admin-apis
