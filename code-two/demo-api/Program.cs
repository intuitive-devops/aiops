using System.Globalization;
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
