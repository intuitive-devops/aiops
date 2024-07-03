using System;
using System.Threading;
using Bph;
using Boagaphish.Settings;
using System.IO;
using Boagaphish;
using System.Xml;
using SweetPolynomial;
using System.Collections;
using SoftAgent.Core;

namespace Cartheur.Demo
{
    /// <summary>
    /// For this program to work, a docker-compose yml must first be run.
    /// </summary>
    class Program
    {
        // Statistics of Interest
        static string ContainerID { get; set; }
        static string ContainerName { get; set; }
        static string CpuPercentage { get; set; }
        static string MemoryUsage { get; set; }
        static string MemoryLimit { get; set; }
        static string MemoryPercentage { get; set; }
        static string NetworkBandwidth { get; set; }
        static string NumberOfPoints { get; set; }
        static int SequenceNumber { get; set; }
        static string Result { get; set; }
        static XmlDocument _document;
        const string FileType = ".xml";
        static System.Timers.Timer _whirlTimer;
        // static List<string> Reports { get; set; } Reports = new List<string>();
        static double Lifetime { get; set; }
        static int Duration { get; set; }
        static bool RunSanityCheck { get; set; }
        static SettingsDictionary GlobalSettings;
        static string Task { get; set; }
        static Noise NoiseSource { get; set; }
        static string MatrixRaw { get; set; }
        static ArrayList Matrices { get; set; }
        static ArrayList MatriceResults { get; set; }
        static Matrix MatrixResult { get; set; }
        static void Initalize()
        {
            GlobalSettings = new SettingsDictionary();
            LoadSettings();
            // Set program parameters.
            Lifetime = Convert.ToDouble(GlobalSettings.GrabSetting("lifetime"));
            Duration = Convert.ToInt32(GlobalSettings.GrabSetting("duration"));
            Task = GlobalSettings.GrabSetting("task");
            RunSanityCheck = Convert.ToBoolean(GlobalSettings.GrabSetting("runsanitycheck"));
            NumberOfPoints = GlobalSettings.GrabSetting("iterations");
            // Run sanity-checks, if set in the Settings.xml file.
            Statistics.XmsFileName = GlobalSettings.GrabSetting("xmsfilename");
            if (RunSanityCheck)
            {
                Console.WriteLine("Sanity check being conducted.");
                MatrixRaw = GlobalSettings.GrabSetting("matrices");
                Matrices = new ArrayList();
                MatriceResults = new ArrayList();
                TestDocker(NumberOfPoints);
                for (int i = 0; i < 100; i++)
                {
                    TestNoisiness();
                }
            }
            Console.WriteLine("Intialization complete. Hit a key to continue.");
            Console.ReadLine();
        }
        static void Main()
        {
            // Begin.
            Initalize();
            var startTime = DateTime.Now;
            // Here is the noise source, but what to do with it?
            NoiseSource = new Noise();
            //
            // Begin.
            if (Task == "demo-sequence")
                Console.WriteLine("Running the demonstration sequence...");
            Console.WriteLine("Begin an agent sequence that lasts for " + Duration.ToString() + " milliseconds. The agent has a lifespan of " + Lifetime.ToString() + " minutes.");
            // 1. Create a whirl that considers a period of interest (as a curiousity-problem)
            BeginToRueTheWhirl(Duration, "Run demo");
            Thread.Sleep(Duration);
            do
                // PART A
            {   // 2a. Have running a noisy container/pod with the default set of yaml values - DONE
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
            Console.WriteLine(RueTheWhirl.ActionController()); // Set the action for the whirl.
        }
        public static void WhirlTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            switch (RueTheWhirl.CurrentState)
            {
                case "Zero":
                    RueTheWhirl.CurrentState = "One";
                    //Reports.Add(RueTheWhirl.ActionController());
                    Console.WriteLine(RueTheWhirl.ActionController());
                    break;
                case "One":
                    RueTheWhirl.CurrentState = "Two";
                    //Reports.Add(RueTheWhirl.ActionController());
                    Console.WriteLine(RueTheWhirl.ActionController());
                    break;
                case "Two":
                    RueTheWhirl.CurrentState = "Three";
                    //Reports.Add(RueTheWhirl.ActionController());
                    Console.WriteLine(RueTheWhirl.ActionController());
                    break;
                case "Three":
                    RueTheWhirl.CurrentState = "Four";
                    //Reports.Add(RueTheWhirl.ActionController());
                    Console.WriteLine(RueTheWhirl.ActionController());
                    break;
                case "Four":
                    RueTheWhirl.CurrentState = "Zero";
                    //Reports.Add(RueTheWhirl.ActionController());
                    Console.WriteLine(RueTheWhirl.ActionController());
                    break;
                default:
                    //Reports.Add("--------");
                    break;
            }
            if (RueTheWhirl.ActionController().Contains("Discover") | RueTheWhirl.ActionController() == "Discover the average value of a noisy container.")
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
        public static void TestDocker(string iterations)
        {
            var command = @"run -d -it --name aiTest -v /var/run/docker.sock:/var/run/docker.sock docker";
            //Tasks.TransmitDocker(command, Tasks.CommandType.docker);
            Result = Tasks.ReturnStatistic("noise");
            SequenceNumber = 1;
            for (int i = 0; i < int.Parse(iterations); i++)
            {
                Result = Tasks.ReturnStatistic("noise");
                ParseStatistic(Result, false);
                Thread.Sleep(1000);
            }
            Statistics.SaveStatisticData();
        }
        /// <summary>
        /// Method to determine how to loop complex matrix computations to trigger more CPU and Memory actions.
        /// </summary>
        public static void TestNoisiness()
        {
            var split = MatrixRaw.Split(':');

            for (int i = 0; i < split.Length; i++)
            {
                Matrix M = new Matrix(split[i]);
                Matrices.Add(M);
                Complex det = M.Determinant(); // det = 1 
                Matrix Minv = M.Inverse(); // Minv = [2, -1; -1, 1] 
                
            }
            foreach (Matrix matrix in Matrices)
            {
                Matrix matrixCopy = matrix;
                MatrixResult = matrixCopy * matrix;
                MatriceResults.Add(MatrixResult);
            }
        }

        #endregion

        #region Utilities
        public static void ParseStatistic(string result, bool storage)
        {
            var split = result.Split(' ');
            ContainerID = split[0];
            ContainerName = split[3];
            CpuPercentage = split[8];
            MemoryUsage = split[13];
            MemoryLimit = split[15];
            MemoryPercentage = split[18];
            NetworkBandwidth = split[23];
            if (!storage)
            {
                // Store as arrays.
                Statistics.ProcessMetrics(SequenceNumber, NumberOfPoints, CpuPercentage, MemoryUsage, MemoryLimit, MemoryPercentage, NetworkBandwidth);
                Statistics.TimeStamp = DateTime.Now;
                Console.WriteLine("Processed sequence: " + SequenceNumber + " of " + NumberOfPoints);
            }
            if (storage)
            {
                // Save as xml files. What is the periodicity?
            }
            SequenceNumber++;;
        }
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
        static void SmellPerlin()
        {
            Noise perlin = new Noise();

            //Standard noise, provide the x, y and z coordinate to sample the noise function. 
            //y and z are optional parameters.
            double noise = perlin.Noise(1);

            //Creates noise that will seamlessly tile in all directions in all dimensions. 
            //tileRegion specifies the size of the region to "tile over", a larger value 
            //means it will take longer for the noise to repeat. y, z and tileRedion are optional parameters.
            //double noiseTiled = perlin.NoiseTiled(x, y, z, tileRegion);

            //Samples the standard noise functions multiple times and adds the results together. 
            //Each sample is sampled at a higher frequency and lower amplitude, adding more 
            //and smaller features to the noise. numberOfOctaves determines how many times to 
            //sample the standard noise function. lacunarity specifies how quickly the frequency increases. 
            //persistence specifies how quickly the amplitude of consecutive samples decreases. 
            //All parameters except x are optional.
            //double noiseOctaves = perlin.NoiseOctaves(x, y, z, numberOfOctaves, lacunarity, persistence);

            //Combines the Octaves and Tiled nosise functions to create tilable noise that 
            //consists of multiple octaves.
            //double noiseTiledOctaves = perlin.NoiseTiledOctaves(x, y, z, tileRegion, numberOfOctaves, lacunarity, persistence);
        }
        //static void SumPerlin()
        //{
        //    Bitmap bitmap = new Bitmap(size, size);
        //    var perlin = new Perlin();
        //    for (int x = 0; x < size; x++)
        //    {
        //        for (int y = 0; y < size; y++)
        //        {
        //            //GetColor() takes in a double in the range 0..1 and returns the proper color
        //            Color color = GetColor((1d + perlin.NoiseOctaves(
        //                (double)x / 32,
        //                (double)y / 32,
        //                0.5d)) / 2d);
        //            bitmap.SetPixel(x, y, color);
        //        }
        //    }
        //    bitmap.Save("images/WorldMap-image.png");

        //    Color GetColor = (color) => {
        //        if (color < 0.35d)
        //        {//water
        //            return Color.FromArgb(60, 110, 200);
        //        }
        //        else if (color < 0.45d)
        //        {   //shallow water
        //            return Color.FromArgb(64, 104, 192);
        //        }
        //        else if (color < 0.48d)
        //        { //sand
        //            return Color.FromArgb(208, 207, 130);
        //        }
        //        else if (color < 0.55d)
        //        { //grass
        //            return Color.FromArgb(84, 150, 29);
        //        }
        //        else if (color < 0.6d)
        //        { //forrest
        //            return Color.FromArgb(61, 105, 22);
        //        }
        //        else if (color < 0.7d)
        //        { //mountain
        //            return Color.FromArgb(91, 68, 61);
        //        }
        //        else if (color < 0.87d)
        //        { //high mountain
        //            return Color.FromArgb(75, 58, 54);
        //        }
        //        else
        //        { //snow
        //            return Color.FromArgb(255, 254, 255);
        //        }
        //    };
        //}

        #endregion
    }
}
