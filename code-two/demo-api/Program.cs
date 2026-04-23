using System.Globalization;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var logPath = ResolveDecisionLogPath();

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

app.MapGet("/metrics", () =>
{
    var entries = ReadEntries(logPath, 5000);
    var text = BuildPrometheusMetrics(entries);
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

static string BuildPrometheusMetrics(List<DecisionLogEntry> entries)
{
    var sb = new StringBuilder();
    sb.AppendLine("# HELP bunnyhop_decision_total Total number of bunnyhop decisions grouped by state/action/signal.");
    sb.AppendLine("# TYPE bunnyhop_decision_total counter");

    var grouped = entries
        .GroupBy(entry => new { entry.State, entry.Action, entry.Signal })
        .OrderBy(group => group.Key.State)
        .ThenBy(group => group.Key.Action)
        .ThenBy(group => group.Key.Signal);

    foreach (var group in grouped)
    {
        sb.Append("bunnyhop_decision_total");
        sb.Append("{state=\"").Append(EscapeLabelValue(group.Key.State)).Append("\",");
        sb.Append("action=\"").Append(EscapeLabelValue(group.Key.Action)).Append("\",");
        sb.Append("signal=\"").Append(EscapeLabelValue(group.Key.Signal)).Append("\"} ");
        sb.AppendLine(group.Count().ToString(CultureInfo.InvariantCulture));
    }

    sb.AppendLine("# HELP bunnyhop_latest_decision_confidence Latest confidence value observed for each whirl state.");
    sb.AppendLine("# TYPE bunnyhop_latest_decision_confidence gauge");
    var latestByState = entries
        .GroupBy(entry => entry.State)
        .Select(group => group.OrderByDescending(entry => ParseTimestamp(entry.Timestamp)).First());

    foreach (var entry in latestByState.OrderBy(entry => entry.State))
    {
        sb.Append("bunnyhop_latest_decision_confidence");
        sb.Append("{state=\"").Append(EscapeLabelValue(entry.State)).Append("\"} ");
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
