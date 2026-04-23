using System;
using System.Threading;
using Bph;
using Boagaphish.Settings;
using System.IO;
using Boagaphish;
using System.Xml;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Cartheur.Demo
{
    class Program
    {
        static XmlDocument _document;
        const string FileType = ".xml";
        static System.Timers.Timer _whirlTimer;
        static double Lifetime { get; set; }
        static int Duration { get; set; }
        static int MatrixLoops { get; set; }
        static SettingsDictionary GlobalSettings;
        static string Task { get; set; }
        static string DecisionLogPath { get; set; }
        static string RunId { get; } = Guid.NewGuid().ToString("N");

        static void Initalize()
        {
            GlobalSettings = new SettingsDictionary();
            LoadSettings();

            Lifetime = ReadDouble("lifetime", 2);
            Duration = ReadInt("duration", 3000);
            Task = ReadString("task", "demo-sequence");
            MatrixLoops = ReadInt("matrixloops", 10000);

            var configuredLogLocation = ReadString("logfile", "logs");
            DecisionLogPath = DecisionLog.ResolveLogPath(configuredLogLocation);
        }

        static void Main()
        {
            Initalize();
            var startTime = DateTime.Now;

            if (Task == "demo-sequence")
            {
                Console.WriteLine("Running the demonstration sequence...");
            }

            Console.WriteLine("Begin an agent sequence that lasts for " + Duration.ToString() + " milliseconds. The agent has a lifespan of " + Lifetime.ToString(CultureInfo.InvariantCulture) + " minutes.");
            Console.WriteLine("Decision log path: " + DecisionLogPath);

            BeginToRueTheWhirl(Duration, "Run demo");
            Thread.Sleep(Duration);

            while (((TimeSpan)(DateTime.Now - startTime)).TotalMinutes < Lifetime)
            {
                Thread.Sleep(Math.Max(250, Duration / 5));
            }

            _whirlTimer?.Stop();
            Console.WriteLine("The agent has exhausted its time.");
            Thread.Sleep(2000);
            Environment.Exit(0);
        }

        #region The Novel Whirl
        public static void BeginToRueTheWhirl(double duration, string curiousity)
        {
            _whirlTimer = new System.Timers.Timer
            {
                Interval = duration
            };
            _whirlTimer.Elapsed += WhirlTimerElapsed;
            _whirlTimer.Start();
            RueTheWhirl.CurrentState = "Zero";

            var decision = RueTheWhirl.ActionController('0');
            Console.WriteLine(decision);
            WriteDecision("Zero", decision);
        }

        public static void WhirlTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string nextState;
            object actionArg;

            switch (RueTheWhirl.CurrentState)
            {
                case "Zero":
                    nextState = "One";
                    actionArg = MatrixLoops;
                    break;
                case "One":
                    nextState = "Two";
                    actionArg = '0';
                    break;
                case "Two":
                    nextState = "Three";
                    actionArg = '0';
                    break;
                case "Three":
                    nextState = "Four";
                    actionArg = '0';
                    break;
                case "Four":
                    nextState = "Zero";
                    actionArg = '0';
                    break;
                default:
                    nextState = "Zero";
                    actionArg = '0';
                    break;
            }

            RueTheWhirl.CurrentState = nextState;
            var decision = RueTheWhirl.ActionController(actionArg);
            Console.WriteLine(decision);
            WriteDecision(nextState, decision);

            if (decision == "Discover the average value of a noisy container.")
            {
                // Placeholder for sampling and taxonomy logic.
            }
        }
        #endregion

        #region Utilities
        public static void LoadSettings()
        {
            var candidates = new[]
            {
                Path.Combine(Environment.CurrentDirectory, "config", "Settings.xml"),
                Path.Combine(AppContext.BaseDirectory, "config", "Settings.xml"),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "config", "Settings.xml")
            };

            var path = candidates.FirstOrDefault(File.Exists);
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new FileNotFoundException("Unable to find Settings.xml. Looked in: " + string.Join(", ", candidates));
            }

            GlobalSettings.LoadSettings(Path.GetFullPath(path));
        }

        static int ReadInt(string settingName, int fallback)
        {
            var envName = "AIOPS_" + settingName.ToUpperInvariant();
            var envValue = Environment.GetEnvironmentVariable(envName);
            if (int.TryParse(envValue, out var envParsed))
            {
                return envParsed;
            }

            var configured = GlobalSettings.GrabSetting(settingName);
            if (int.TryParse(configured, out var parsed))
            {
                return parsed;
            }

            return fallback;
        }

        static double ReadDouble(string settingName, double fallback)
        {
            var envName = "AIOPS_" + settingName.ToUpperInvariant();
            var envValue = Environment.GetEnvironmentVariable(envName);
            if (double.TryParse(envValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var envParsed))
            {
                return envParsed;
            }

            var configured = GlobalSettings.GrabSetting(settingName);
            if (double.TryParse(configured, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed;
            }

            return fallback;
        }

        static string ReadString(string settingName, string fallback)
        {
            var envName = "AIOPS_" + settingName.ToUpperInvariant();
            var envValue = Environment.GetEnvironmentVariable(envName);
            if (!string.IsNullOrWhiteSpace(envValue))
            {
                return envValue;
            }

            var configured = GlobalSettings.GrabSetting(settingName);
            return string.IsNullOrWhiteSpace(configured) ? fallback : configured;
        }

        static void WriteDecision(string state, string decision)
        {
            var (signal, confidence, action) = BuildDecisionMetadata(state, decision);
            DecisionLog.Write(DecisionLogPath, RunId, state, signal, decision, confidence, action);
        }

        static (string signal, double confidence, string action) BuildDecisionMetadata(string state, string decision)
        {
            if (state == "One")
            {
                return ("cpu_noise_sustained", 0.64, "run_matrix_probe");
            }
            if (state == "Two")
            {
                return ("probe_complete", 0.72, "spawn_agent");
            }
            if (state == "Three")
            {
                return ("agent_ready", 0.81, "build_mitigation_recommendation");
            }
            if (state == "Four")
            {
                return ("recommendation_ready", 0.88, "publish_decision");
            }

            if (decision.Contains("Resting", StringComparison.OrdinalIgnoreCase))
            {
                return ("stable_window", 0.55, "continue_monitoring");
            }

            return ("sequence_bootstrap", 0.60, "begin_sequence");
        }

        private static string XPathValue(string xPath)
        {
            var node = _document.SelectSingleNode((xPath));
            if (node == null)
                Logging.WriteLog(@"Cannot find the specified node.", Logging.LogType.Error, Logging.LogCaller.AgentCore, "XPathValue");
            return node != null ? node.InnerText : "";
        }
        #endregion
    }
}
