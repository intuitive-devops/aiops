using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var logPath = ResolveDecisionLogPath();
var alertsLogPath = ResolveAlertsLogPath(logPath);
var slackWebhookUrl = Environment.GetEnvironmentVariable("AIOPS_SLACK_WEBHOOK_URL");
var httpClient = new HttpClient();
long alertsReceivedTotal = 0;
long alertsForwardedTotal = 0;
long alertsForwardFailureTotal = 0;
long replayTriggeredTotal = 0;

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    utc = DateTimeOffset.UtcNow,
    decisionLog = logPath
}));

app.MapGet("/api/status", () =>
{
    var latest = ReadEntries(logPath, 1).FirstOrDefault();
    if (latest is null)
    {
        return Results.Ok(new
        {
            status = "waiting_for_decisions",
            decisionLog = logPath,
            message = "No decision entries found yet. Run scripts/demo/run-demo-sequence.sh first."
        });
    }

    return Results.Ok(new
    {
        status = "ready",
        decisionLog = logPath,
        latest
    });
});

app.MapGet("/api/decisions", (int? limit) =>
{
    var capped = Math.Clamp(limit ?? 25, 1, 200);
    var entries = ReadEntries(logPath, capped);
    return Results.Ok(new
    {
        count = entries.Count,
        limit = capped,
        items = entries
    });
});

app.MapGet("/api/replay", (int? cycles) =>
{
    var replayCycles = Math.Clamp(cycles ?? 1, 1, 10);
    var runId = $"replay-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}";
    AppendReplayEntries(logPath, runId, replayCycles);
    Interlocked.Add(ref replayTriggeredTotal, replayCycles);

    return Results.Ok(new
    {
        status = "replay_triggered",
        runId,
        cycles = replayCycles,
        decisionLog = logPath
    });
});

app.MapPost("/api/alerts", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    var payload = await reader.ReadToEndAsync();
    var summary = SummarizeAlertPayload(payload);
    var now = DateTimeOffset.UtcNow;

    var line = JsonSerializer.Serialize(new
    {
        timestamp = now.ToString("O", CultureInfo.InvariantCulture),
        summary.status,
        summary.alertCount,
        summary.severity,
        summary.alertNames
    });
    AppendLine(alertsLogPath, line);
    Interlocked.Increment(ref alertsReceivedTotal);

    if (!string.IsNullOrWhiteSpace(slackWebhookUrl))
    {
        var slackText = $"bunnyhop alert [{summary.status}] severity={summary.severity} count={summary.alertCount} alerts={string.Join(",", summary.alertNames)}";
        var slackPayload = JsonSerializer.Serialize(new { text = slackText });
        using var content = new StringContent(slackPayload, Encoding.UTF8, "application/json");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        try
        {
            var response = await httpClient.PostAsync(slackWebhookUrl, content);
            if (response.IsSuccessStatusCode)
            {
                Interlocked.Increment(ref alertsForwardedTotal);
            }
            else
            {
                Interlocked.Increment(ref alertsForwardFailureTotal);
            }
        }
        catch
        {
            Interlocked.Increment(ref alertsForwardFailureTotal);
        }
    }

    return Results.Ok(new
    {
        status = "alert_received",
        alertStatus = summary.status,
        summary.alertCount
    });
});

app.MapGet("/metrics", () =>
{
    var entries = ReadEntries(logPath, 5000);
    var text = BuildPrometheusMetrics(
        entries,
        Interlocked.Read(ref alertsReceivedTotal),
        Interlocked.Read(ref alertsForwardedTotal),
        Interlocked.Read(ref alertsForwardFailureTotal),
        Interlocked.Read(ref replayTriggeredTotal));
    return Results.Text(text, "text/plain; version=0.0.4; charset=utf-8");
});

app.Run();

static string ResolveDecisionLogPath()
{
    var envPath = Environment.GetEnvironmentVariable("AIOPS_DECISION_LOG");
    if (!string.IsNullOrWhiteSpace(envPath))
    {
        return Path.GetFullPath(envPath);
    }

    var candidates = new[]
    {
        Path.Combine(Environment.CurrentDirectory, "code-two", "run", "logs", "decision-log.jsonl"),
        Path.Combine(Environment.CurrentDirectory, "run", "logs", "decision-log.jsonl"),
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "run", "logs", "decision-log.jsonl")
    };

    var existing = candidates
        .Select(Path.GetFullPath)
        .FirstOrDefault(File.Exists);

    return !string.IsNullOrWhiteSpace(existing)
        ? existing
        : Path.GetFullPath(candidates[0]);
}

static string ResolveAlertsLogPath(string decisionLogPath)
{
    var directory = Path.GetDirectoryName(decisionLogPath) ?? AppContext.BaseDirectory;
    return Path.Combine(directory, "alert-events.jsonl");
}

static List<DecisionLogEntry> ReadEntries(string path, int limit)
{
    if (!File.Exists(path))
    {
        return new List<DecisionLogEntry>();
    }

    return File.ReadLines(path)
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Reverse()
        .Take(limit)
        .Select(TryParse)
        .Where(entry => entry is not null)
        .Cast<DecisionLogEntry>()
        .OrderBy(entry => entry.Timestamp)
        .ToList();
}

static DecisionLogEntry? TryParse(string line)
{
    try
    {
        return JsonSerializer.Deserialize<DecisionLogEntry>(line);
    }
    catch
    {
        return null;
    }
}

static void AppendLine(string path, string line)
{
    var directory = Path.GetDirectoryName(path);
    if (!string.IsNullOrWhiteSpace(directory))
    {
        Directory.CreateDirectory(directory);
    }

    File.AppendAllText(path, line + Environment.NewLine);
}

static void AppendReplayEntries(string logPath, string runId, int cycles)
{
    var template = new (string State, string Signal, string Decision, double Confidence, string Action)[]
    {
        ("Zero", "sequence_bootstrap", "Initialize baseline", 0.82, "begin_sequence"),
        ("Zero", "stable_window", "Continue monitoring", 0.77, "continue_monitoring"),
        ("One", "cpu_noise_sustained", "Anomaly detected", 0.91, "run_matrix_probe"),
        ("Two", "probe_complete", "Agent activated", 0.88, "spawn_agent"),
        ("Three", "agent_ready", "Mitigation prepared", 0.84, "build_mitigation_recommendation"),
        ("Four", "recommendation_ready", "Publish recommendation", 0.86, "publish_decision")
    };

    for (var cycle = 0; cycle < cycles; cycle++)
    {
        foreach (var item in template)
        {
            var entry = new DecisionLogEntry
            {
                Timestamp = DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture),
                Signal = item.Signal,
                Decision = item.Decision,
                Confidence = item.Confidence,
                Action = item.Action,
                State = item.State,
                RunId = $"{runId}-{cycle + 1}"
            };

            AppendLine(logPath, JsonSerializer.Serialize(entry));
        }
    }
}

static (string status, int alertCount, string severity, List<string> alertNames) SummarizeAlertPayload(string payload)
{
    try
    {
        using var doc = JsonDocument.Parse(payload);
        var root = doc.RootElement;
        var status = root.TryGetProperty("status", out var statusEl) ? (statusEl.GetString() ?? "unknown") : "unknown";
        var names = new List<string>();
        var severity = "unknown";

        if (root.TryGetProperty("alerts", out var alertsEl) && alertsEl.ValueKind == JsonValueKind.Array)
        {
            foreach (var alert in alertsEl.EnumerateArray())
            {
                if (alert.TryGetProperty("labels", out var labels) && labels.ValueKind == JsonValueKind.Object)
                {
                    if (labels.TryGetProperty("alertname", out var alertNameEl))
                    {
                        names.Add(alertNameEl.GetString() ?? "unknown");
                    }

                    if (severity == "unknown" && labels.TryGetProperty("severity", out var severityEl))
                    {
                        severity = severityEl.GetString() ?? "unknown";
                    }
                }
            }
        }

        if (names.Count == 0)
        {
            names.Add("unknown");
        }

        return (status, names.Count, severity, names.Distinct(StringComparer.Ordinal).ToList());
    }
    catch
    {
        return ("unknown", 0, "unknown", new List<string> { "parse_error" });
    }
}

static string BuildPrometheusMetrics(
    List<DecisionLogEntry> entries,
    long alertsReceivedTotal,
    long alertsForwardedTotal,
    long alertsForwardFailureTotal,
    long replayTriggeredTotal)
{
    var sb = new StringBuilder();
    sb.AppendLine("# HELP bunnyhop_decision_total Total number of bunnyhop decisions grouped by state/action/signal.");
    sb.AppendLine("# TYPE bunnyhop_decision_total counter");

    var knownCombinations = new[]
    {
        new { State = "Zero", Action = "begin_sequence", Signal = "sequence_bootstrap" },
        new { State = "Zero", Action = "continue_monitoring", Signal = "stable_window" },
        new { State = "One", Action = "run_matrix_probe", Signal = "cpu_noise_sustained" },
        new { State = "Two", Action = "spawn_agent", Signal = "probe_complete" },
        new { State = "Three", Action = "build_mitigation_recommendation", Signal = "agent_ready" },
        new { State = "Four", Action = "publish_decision", Signal = "recommendation_ready" }
    };

    var grouped = entries
        .GroupBy(entry => new { entry.State, entry.Action, entry.Signal })
        .OrderBy(group => group.Key.State)
        .ThenBy(group => group.Key.Action)
        .ThenBy(group => group.Key.Signal);

    var groupedLookup = grouped.ToDictionary(
        group => $"{group.Key.State}|{group.Key.Action}|{group.Key.Signal}",
        group => group.Count());

    foreach (var combo in knownCombinations)
    {
        var key = $"{combo.State}|{combo.Action}|{combo.Signal}";
        var count = groupedLookup.TryGetValue(key, out var existing) ? existing : 0;
        sb.Append("bunnyhop_decision_total");
        sb.Append("{state=\"").Append(EscapeLabelValue(combo.State)).Append("\",");
        sb.Append("action=\"").Append(EscapeLabelValue(combo.Action)).Append("\",");
        sb.Append("signal=\"").Append(EscapeLabelValue(combo.Signal)).Append("\"} ");
        sb.AppendLine(count.ToString(CultureInfo.InvariantCulture));
    }

    foreach (var group in grouped.Where(group =>
                 !knownCombinations.Any(combo =>
                     combo.State == group.Key.State &&
                     combo.Action == group.Key.Action &&
                     combo.Signal == group.Key.Signal)))
    {
        sb.Append("bunnyhop_decision_total");
        sb.Append("{state=\"").Append(EscapeLabelValue(group.Key.State)).Append("\",");
        sb.Append("action=\"").Append(EscapeLabelValue(group.Key.Action)).Append("\",");
        sb.Append("signal=\"").Append(EscapeLabelValue(group.Key.Signal)).Append("\"} ");
        sb.AppendLine(group.Count().ToString(CultureInfo.InvariantCulture));
    }

    sb.AppendLine("# HELP bunnyhop_latest_decision_confidence Latest confidence value observed for each whirl state.");
    sb.AppendLine("# TYPE bunnyhop_latest_decision_confidence gauge");

    var knownStates = new[] { "Zero", "One", "Two", "Three", "Four" };
    var latestByState = entries
        .GroupBy(entry => entry.State)
        .ToDictionary(
            group => group.Key,
            group => group.OrderByDescending(entry => ParseTimestamp(entry.Timestamp)).First());

    foreach (var state in knownStates)
    {
        var confidence = latestByState.TryGetValue(state, out var latestEntry) ? latestEntry.Confidence : 0.0;
        sb.Append("bunnyhop_latest_decision_confidence");
        sb.Append("{state=\"").Append(EscapeLabelValue(state)).Append("\"} ");
        sb.AppendLine(confidence.ToString(CultureInfo.InvariantCulture));
    }

    foreach (var state in latestByState.Keys.Where(state => !knownStates.Contains(state, StringComparer.Ordinal)).OrderBy(state => state))
    {
        var entry = latestByState[state];
        sb.Append("bunnyhop_latest_decision_confidence");
        sb.Append("{state=\"").Append(EscapeLabelValue(state)).Append("\"} ");
        sb.AppendLine(entry.Confidence.ToString(CultureInfo.InvariantCulture));
    }

    var latest = entries
        .OrderByDescending(entry => ParseTimestamp(entry.Timestamp))
        .FirstOrDefault();

    sb.AppendLine("# HELP bunnyhop_latest_decision_timestamp_seconds Unix timestamp of the most recent decision.");
    sb.AppendLine("# TYPE bunnyhop_latest_decision_timestamp_seconds gauge");
    var latestUnix = latest is null ? 0 : ParseTimestamp(latest.Timestamp).ToUnixTimeSeconds();
    sb.AppendLine("bunnyhop_latest_decision_timestamp_seconds " + latestUnix.ToString(CultureInfo.InvariantCulture));

    sb.AppendLine("# HELP bunnyhop_anomaly_active 1 when latest signal indicates sustained anomaly, otherwise 0.");
    sb.AppendLine("# TYPE bunnyhop_anomaly_active gauge");
    var anomalyActive = latest?.Signal == "cpu_noise_sustained" ? 1 : 0;
    sb.AppendLine("bunnyhop_anomaly_active " + anomalyActive.ToString(CultureInfo.InvariantCulture));

    sb.AppendLine("# HELP bunnyhop_slo_risk_score Composite SLO risk score in range [0,1].");
    sb.AppendLine("# TYPE bunnyhop_slo_risk_score gauge");
    var latestConfidence = latest?.Confidence ?? 0.0;
    var sloRisk = Math.Clamp((anomalyActive * 0.7) + ((1.0 - latestConfidence) * 0.3), 0.0, 1.0);
    sb.AppendLine("bunnyhop_slo_risk_score " + sloRisk.ToString(CultureInfo.InvariantCulture));

    sb.AppendLine("# HELP bunnyhop_recommended_action_code Encoded recommended action based on latest decision.");
    sb.AppendLine("# TYPE bunnyhop_recommended_action_code gauge");
    var actionCode = latest?.Action switch
    {
        "continue_monitoring" => 1,
        "run_matrix_probe" => 2,
        "spawn_agent" => 3,
        "build_mitigation_recommendation" => 4,
        "publish_decision" => 5,
        "begin_sequence" => 6,
        _ => 0
    };
    sb.AppendLine("bunnyhop_recommended_action_code " + actionCode.ToString(CultureInfo.InvariantCulture));

    sb.AppendLine("# HELP bunnyhop_time_saved_minutes Estimated operator minutes saved by automated decisions.");
    sb.AppendLine("# TYPE bunnyhop_time_saved_minutes gauge");
    var decisionCount = entries.Count;
    var timeSavedMinutes = decisionCount * 2.5;
    sb.AppendLine("bunnyhop_time_saved_minutes " + timeSavedMinutes.ToString(CultureInfo.InvariantCulture));

    sb.AppendLine("# HELP bunnyhop_replay_trigger_total Number of incident replay triggers.");
    sb.AppendLine("# TYPE bunnyhop_replay_trigger_total counter");
    sb.AppendLine("bunnyhop_replay_trigger_total " + replayTriggeredTotal.ToString(CultureInfo.InvariantCulture));

    sb.AppendLine("# HELP bunnyhop_alerts_received_total Total alerts received from Alertmanager webhook.");
    sb.AppendLine("# TYPE bunnyhop_alerts_received_total counter");
    sb.AppendLine("bunnyhop_alerts_received_total " + alertsReceivedTotal.ToString(CultureInfo.InvariantCulture));

    sb.AppendLine("# HELP bunnyhop_alerts_forwarded_total Total alert messages forwarded to Slack webhook.");
    sb.AppendLine("# TYPE bunnyhop_alerts_forwarded_total counter");
    sb.AppendLine("bunnyhop_alerts_forwarded_total " + alertsForwardedTotal.ToString(CultureInfo.InvariantCulture));

    sb.AppendLine("# HELP bunnyhop_alerts_forward_failures_total Total Slack webhook forward failures.");
    sb.AppendLine("# TYPE bunnyhop_alerts_forward_failures_total counter");
    sb.AppendLine("bunnyhop_alerts_forward_failures_total " + alertsForwardFailureTotal.ToString(CultureInfo.InvariantCulture));

    return sb.ToString();
}

static DateTimeOffset ParseTimestamp(string? value)
{
    if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
    {
        return parsed;
    }
    return DateTimeOffset.UnixEpoch;
}

static string EscapeLabelValue(string? value)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        return "unknown";
    }

    return value
        .Replace("\\", "\\\\", StringComparison.Ordinal)
        .Replace("\"", "\\\"", StringComparison.Ordinal)
        .Replace("\n", "\\n", StringComparison.Ordinal);
}

public sealed class DecisionLogEntry
{
    public string Timestamp { get; set; } = DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture);
    public string Signal { get; set; } = string.Empty;
    public string Decision { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string Action { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string RunId { get; set; } = string.Empty;
}
