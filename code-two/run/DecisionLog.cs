using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Cartheur.Demo
{
    public sealed class DecisionLogEntry
    {
        public string Timestamp { get; set; }
        public string Signal { get; set; }
        public string Decision { get; set; }
        public double Confidence { get; set; }
        public string Action { get; set; }
        public string State { get; set; }
        public string RunId { get; set; }
    }

    public static class DecisionLog
    {
        private static readonly object Sync = new object();

        public static string ResolveLogPath(string configuredLogLocation)
        {
            var envOverride = Environment.GetEnvironmentVariable("AIOPS_DECISION_LOG");
            if (!string.IsNullOrWhiteSpace(envOverride))
            {
                return Path.GetFullPath(envOverride);
            }

            var fileName = "decision-log.jsonl";
            var candidates = new List<string>();

            if (!string.IsNullOrWhiteSpace(configuredLogLocation))
            {
                candidates.Add(Path.Combine(Environment.CurrentDirectory, configuredLogLocation, fileName));
                candidates.Add(Path.Combine(AppContext.BaseDirectory, configuredLogLocation, fileName));
                candidates.Add(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", configuredLogLocation, fileName));
            }

            candidates.Add(Path.Combine(Environment.CurrentDirectory, "logs", fileName));
            candidates.Add(Path.Combine(AppContext.BaseDirectory, "logs", fileName));
            candidates.Add(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "logs", fileName));

            var existing = candidates
                .Select(Path.GetFullPath)
                .FirstOrDefault(File.Exists);

            if (!string.IsNullOrWhiteSpace(existing))
            {
                return existing;
            }

            return Path.GetFullPath(candidates.First());
        }

        public static void Write(string logPath, string runId, string state, string signal, string decision, double confidence, string action)
        {
            var entry = new DecisionLogEntry
            {
                Timestamp = DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture),
                Signal = signal,
                Decision = decision,
                Confidence = confidence,
                Action = action,
                State = state,
                RunId = runId
            };

            var directory = Path.GetDirectoryName(logPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var line = JsonSerializer.Serialize(entry);
            lock (Sync)
            {
                File.AppendAllText(logPath, line + Environment.NewLine);
            }
        }
    }
}
