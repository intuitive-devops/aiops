using System;
using System.Collections.Generic;
using System.Threading;
using Bph;
using Boagaphish.Settings;
using System.IO;
using Boagaphish;
using System.Xml;

namespace Cartheur.Demo
{
    class Program
    {
        static XmlDocument _document;
        const string FileType = ".xml";
        static System.Timers.Timer _whirlTimer;
        //static List<string> Reports { get; set; }
        static double Lifetime { get; set; }
        static int Duration { get; set; }
        static int MatrixLoops { get; set; }
        static SettingsDictionary GlobalSettings;
        static string Task { get; set; }
        static void Initalize()
        {
            GlobalSettings = new SettingsDictionary();
            LoadSettings();
            // Set program parameters.
            Lifetime = Convert.ToDouble(GlobalSettings.GrabSetting("lifetime"));
            Duration = Convert.ToInt32(GlobalSettings.GrabSetting("duration"));
            Task = GlobalSettings.GrabSetting("task");
            MatrixLoops = Convert.ToInt32(GlobalSettings.GrabSetting("matrixloops"));
        }
        static void Main()
        {
            // Begin.
            Initalize();
            var startTime = DateTime.Now;
            //Reports = new List<string>();
            // Begin.
            if (Task == "demo-sequence")
                Console.WriteLine("Running the demonstration sequence...");
            Console.WriteLine("Begin an agent sequence that lasts for " + Duration.ToString() + " milliseconds. The agent has a lifespan of " + Lifetime.ToString() + " minutes.");
            // 1. Create a whirl that considers a period of interest (as a curiousity-problem)
            BeginToRueTheWhirl(Duration, "Run demo");
            Thread.Sleep(Duration);
            do
            {   // PART A
                // 2a. Have running a noisy container/pod with the default set of yaml values - DONE
                // 2b. Have running a prometheus server that tracks the metrics-of-interest (using API C#) - DONE (docker-compose up) BUT NOT IN DEMO
                // 2c. Format the output from the API appropriate for the soft agent (use API like LiveServer) -  DONE
                // 3a. Create a (soft) agent that works with the statistical data, that leverages bph-indications-trends to analyze the patterns - DONE
                // 3b. Structure a windows form around the behaviour of the algorithm on time-series data, the analysis, and the decision (pop-up accept/decline) -DONE
                // 4. Sends the decision to the whirl. - DONE
                // 5. Generates a yaml (docker) file and sends to the controller container (debian-bazel-dockercli). - TODO

                // really, though...what is the target output for an aiops platform? Finishing on this step for this first demo.

                // PART B - Manuipulate the container with new settings
                // 6. Revive archived debian-bazel use its docker-cli (the controller container) - DONE
                // 7. Run either "docker-in-docker" or remote daemon (using the latter) - DONE
                //
                // 6. Build a new container using the generated Dockerfile - POSSIBLE
                // 7. Stop the old and run the replacement version - POSSIBLE
                //
                // 8. The agent controls the noisy container as: docker attach [OPTIONS] noisy - X
                // 8a. Networking between the pods?
                // 9. How to acually deploy the decision?
                //
                // FINIS
                //
                // Extra credit:
                //      a. Spawns a copy of the first pod with different yaml values (Helm)
                //      b. Sends a message to the agent to peform an analysis on the timespan of (prometheus) metrics
                //      c. The whirl produces a report of what has happened in the curiousity period of interest
                //

            } while (((TimeSpan)(DateTime.Now - startTime)).TotalMinutes < Lifetime) ;

            Console.WriteLine("The agent has exhausted its time.");
            Thread.Sleep(2000);
            Environment.Exit(0);
        }

        #region The Novel Whirl
        public static void BeginToRueTheWhirl(double duration, string curiousity)
        {
            _whirlTimer = new System.Timers.Timer
            {
                Interval = duration // Duration of time in each state (in ms)
            };
            _whirlTimer.Elapsed += WhirlTimerElapsed;
            _whirlTimer.Start(); // Start whirl-duration timer.
            RueTheWhirl.CurrentState = "Zero"; // Start the whirl.
            //Reports.Add(RueTheWhirl.ActionController()); 
            Console.WriteLine(RueTheWhirl.ActionController('0')); // Set the action for the whirl.
        }
        public static void WhirlTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            switch (RueTheWhirl.CurrentState)
            {
                case "Zero":
                    RueTheWhirl.CurrentState = "One";
                    //Reports.Add(RueTheWhirl.ActionController());
                    Console.WriteLine(RueTheWhirl.ActionController('0'));
                    break;
                case "One":
                    RueTheWhirl.CurrentState = "Two";
                    //Reports.Add(RueTheWhirl.ActionController());
                    Console.WriteLine(RueTheWhirl.ActionController(MatrixLoops));
                    break;
                case "Two":
                    RueTheWhirl.CurrentState = "Three";
                    //Reports.Add(RueTheWhirl.ActionController());
                    Console.WriteLine(RueTheWhirl.ActionController('0'));
                    break;
                case "Three":
                    RueTheWhirl.CurrentState = "Four";
                    //Reports.Add(RueTheWhirl.ActionController());
                    Console.WriteLine(RueTheWhirl.ActionController('0'));
                    break;
                case "Four":
                    RueTheWhirl.CurrentState = "Zero";
                    //Reports.Add(RueTheWhirl.ActionController());
                    Console.WriteLine(RueTheWhirl.ActionController('0'));
                    break;
                default:
                    //Reports.Add("--------");
                    break;
            }
            if (RueTheWhirl.ActionController('0').Contains("Discover") | RueTheWhirl.ActionController('0') == "Discover the average value of a noisy container.")
            {
                // Sample the data of the Prometheus instance.
                //SensorTemporalValueDifference();
                // Process taxonomy.
                //ProcessTaxonomy();
                // Start as a find of a peek.
                //PeekValue();
                // Move toward equilibrium.
                //IncrementLeft();


            }
        }

        #endregion

        #region Utilities
        public static void LoadSettings()
        {
            var path = Path.Combine(Environment.CurrentDirectory, Path.Combine("config", "Settings.xml"));
            GlobalSettings.LoadSettings(path);
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
