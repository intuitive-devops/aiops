using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using Boagaphish;
using Boagaphish.ActivationFunctions;
using Boagaphish.Core.Learning;
using Boagaphish.Core.Networks;
using Boagaphish.Custom;
using Boagaphish.Numeric;
using Boagaphish.Settings;
using Cartheur.Animals.Core;
using SoftAgent.Core;
using Timer = System.Timers.Timer;

namespace SoftAgent
{
    // Todo: First draft is using historical rates class.
    public class AgentCore
    {
        #region Left here in case multi-threading is relevant.
        //private Thread _workerThread;
        //private volatile bool _stopThread;
        //private bool _abortRequested;
        #endregion

        private bool _needToStop;
        private XmlDocument _document;
        private const string FileType = ".xml";

        public enum AnalysisTrajectory
        {
            CpuPercentage, MemoryUsage, MemoryLimit, MemoryPercentage, NetworkBandwidth
        }
        public enum NetworkType
        {
            Activation, Custom, Solver
        }
        // Core objects.
        public double[] Data { get; set; }
        public double[,] Solution { get; set; }
        public double[,] Forecast { get; set; }
        public int SolutionSize { get; set; }
        public List<double> ForecastData { get; set; }
        public static bool LocalDataLoaded { get; set; }
        public static bool LiveDataProcessed { get; set; }
        public static bool SolverTrainingDone { get; set; }
        public ActivationNetwork Activation { get; set; }
        public BackPropagation SolverNetwork { get; set; }

        public List<double> PredictionValuesRawData { get; set; }
        public List<double> PredictionValuesSolution { get; set; }
        public List<bool> TrendSolution { get; set; }
        public List<bool> TrendRawData { get; set; }
        public static string HoldingCurrency { get; set; }
        public static string TradingCurrency { get; set; }
        public static string TrajectoryProcessingTime { get; set; }
        public static string CandleFormat { get; set; }
        public static string Granularity { get; set; }
        public static string DailyAlignment { get; set; }
        public string Name { get; set; } //= "OandaForecast";
        public static string XmsFileName { get; set; } //= "xml_candles.xml";
        private const char Separator = ',';
        public static string TrainedNetworkName { get; set; }
        public AnalysisTrajectory Trajectory { get; set; }
        public NetworkType TypeOfNetwork { get; set; }
        public int NumberOfLiveMetrics { get; set; }
        public DateTime StartedOn;
        public TimeSpan Duration;
        public Timer AutolivePollingTimer;
        // Configuration-set properties.
        public int SampleSize { get; set; }
        public int Iterations { get; set; }
        public int TrainingCycles { get; set; }
        public double SigmoidAlpha { get; set; }
        public double Momentum { get; set; }
        public double LearningRate { get; set; }
        public int NumberOfLayers { get; set; }
        public int[] LayerSize { get; set; }
        public double[][] Bias;
        public double[][][] Weight;
        public int WindowSize { get; set; }
        public int PredictionSize { get; set; }
        public int PredictionSizeTrend { get; set; }
        public double Epoch { get; private set; }
        public double LearningError { get; private set; }
        public double PredictionError { get; private set; }
        public double ForecastError { get; private set; }
        /// <summary>
        /// The forecast difference tuple: Data, forecast, difference.
        /// </summary>
        public Tuple<double, double, double> ForecastDifference { get; set; }
        public List<Tuple<double, double, double>> ForecastDifferences { get; set; }
        public double ForecastValueNext { get; set; }
        public int TrailingDigits { get; set; }
        
        public double DataMagnitudeUpper { get; set; }
        public double DataMagnitudeLower { get; set; }
        
        public List<string> TrendFromDataStrings { get; set; }
        public List<string> TrendFromSolutionStrings { get; set; }
        public List<bool> TrendFromData { get; set; }
        public List<bool> TrendFromSolution { get; set; }
        public double TrainingRate { get; set; }
        public double ErrorTolerance { get; set; }
        public TransferFunction TransferFunction { get; set; }
        public Boagaphish.Settings.SettingsDictionary GlobalSettings;
        public double TradingDifferenceMagnitude { get; set; }

        //Todo: // Finished up, to be completed below.
        // 1. Implement softmax, UNDONE.
        // 2. Finish methods, DONE.
        // 3. The decision a preference elicitation for a trade, DONE with truth table and frequency of sequence.
        // 4. Automat. DONE.
        // 5. Incorporate dataset magnitudes, e.g., the maximum and minimum value of a dataset. DONE
        // 6. Softmax/scale DONE

        public AgentCore()
        {
            // Initialize with global settings from the config file.
            GlobalSettings = new Boagaphish.Settings.SettingsDictionary();
            LoadSettings();
            // Set instrument parameters.
            HoldingCurrency = GlobalSettings.GrabSetting("holdingcurrency");
            TradingCurrency = GlobalSettings.GrabSetting("tradingcurrency");
            var function = GlobalSettings.GrabSetting("transferfunction");

            switch (function)
            {
                case "BipolarSigmoid":
                    TransferFunction = TransferFunction.BipolarSigmoid;
                    break;
                case "Gaussian":
                    TransferFunction = TransferFunction.Gaussian;
                    break;
                case "Linear":
                    TransferFunction = TransferFunction.Linear;
                    break;
                case "None":
                    TransferFunction = TransferFunction.None;
                    break;
                case "NormalizedExponent":
                    TransferFunction = TransferFunction.NormalizedExponent;
                    break;
                case "RationalSigmoid":
                    TransferFunction = TransferFunction.RationalSigmoid;
                    break;
                case "Sigmoid":
                    TransferFunction = TransferFunction.Sigmoid;
                    break;
            }
            _document = new XmlDocument();

            WindowSize = int.Parse(GlobalSettings.GrabSetting("windowsize"));
            PredictionSize = int.Parse(GlobalSettings.GrabSetting("predictionsize"));
            PredictionSizeTrend = int.Parse(GlobalSettings.GrabSetting("predictionsizetrend"));
            ForecastSize = int.Parse(GlobalSettings.GrabSetting("forecastsize"));
            TrainingRate = double.Parse(GlobalSettings.GrabSetting("trainingrate"));
            ErrorTolerance = double.Parse(GlobalSettings.GrabSetting("errortolerance"));
            Iterations = int.Parse(GlobalSettings.GrabSetting("iterations"));
            LearningRate = double.Parse(GlobalSettings.GrabSetting("learningrate"));
            SigmoidAlpha = double.Parse(GlobalSettings.GrabSetting("sigmoidalpha"));
            Momentum = int.Parse(GlobalSettings.GrabSetting("momentum"));
            TrailingDigits = int.Parse(GlobalSettings.GrabSetting("trailingdigits"));
            NetworkCorrection = double.Parse(GlobalSettings.GrabSetting("networkcorrection"));
            PredictionSizeTrend = int.Parse(GlobalSettings.GrabSetting("predictionsizetrend"));
            NumberOfLiveMetrics = int.Parse(GlobalSettings.GrabSetting("numberofcandles"));
            TradingDifferenceMagnitude = double.Parse(GlobalSettings.GrabSetting("differencemagnitude"));
            Rates.HistoricalRates.WindowSize = WindowSize;
            ForecastDifferences = new List<Tuple<double, double, double>>();
            Logging.WriteLog(@"An agent core has been initialized, loaded with parameters from the config file.", Logging.LogType.Information, Logging.LogCaller.AgentCore);
        }
        /// <summary>
        /// Gets the path for candle data.
        /// </summary>
        /// <value>
        /// The path for candle data.
        /// </value>
        public string PathForCandleData
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory + @"\data\xms\", XmsFileName);
            }
        }
        public BackPropagationNetwork BackPropagationNetwork { get; set; }
        public BackPropagationNetwork ForecastNetwork { get; set; }
        public BackPropagationNetwork CustomNetwork { get; set; }
        public double BackPropagationError { get; set; }
        public double[][] Input { get; set; }
        public double[][] Desired { get; set; }
        public double NetworkCorrection { get; set; }
        public double[] NetworkInput { get; set; }
        protected int HiddenLayer { get; set; }
        public int OutputLayer { get; set; }
        public int ForecastSize { get; set; }
        protected int SolutionOffset { get; set; }
        public string BuyOrSell { get; set; }
        static int SequenceNumber { get; set; }
        static string Result { get; set; }
        //public string BuyOrSellTrendRawData { get; set; }
        // ----------- Data manipulation
        public string LoadLocalData(string fileName)
        {
            XmsFileName = fileName;// + fileCandles + FileType;
            if (PathForCandleData == null)
                return "There is no path listed for the candle data.";
            StartedOn = DateTime.Now;
            _document = new XmlDocument();
            try
            {
                _document.Load(PathForCandleData);
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.AgentCore);
                return "No local files could be found.";
            }

            // Load from xms.
            const string basePath = "RatesData/Data";
            if (XPathValue("RatesData/@Source") != "Oanda")
                return "Xpath failure.";
            int numberOfCandles;
            int.TryParse(XPathValue("RatesData/@NumberOfCandles"), out numberOfCandles);
            Rates.HistoricalRates.NumberOfCandles = numberOfCandles;
            Rates.HistoricalRates.CandleFormat = XPathValue("RatesData/@CandleFormat");
            int dailyAlignment;
            int.TryParse(XPathValue("RatesData/@DailyAlignment"), out dailyAlignment);
            Rates.HistoricalRates.DailyAlignment = dailyAlignment;
            Rates.HistoricalRates.Granularity = XPathValue("RatesData/@Granularity");
            Rates.HistoricalRates.MarketTimeData = new double[numberOfCandles];
            Rates.HistoricalRates.OpenData = new double[numberOfCandles];
            Rates.HistoricalRates.LowData = new double[numberOfCandles];
            Rates.HistoricalRates.HighData = new double[numberOfCandles];
            Rates.HistoricalRates.CloseData = new double[numberOfCandles];
            Rates.HistoricalRates.VolumeData = new int[numberOfCandles];

            for (var l = 0; l < numberOfCandles; l++)
            {
                double.TryParse(
                    XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@MarketTime"),
                    out Rates.HistoricalRates.MarketTimeData[l]);
                double.TryParse(
                    XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@Open"),
                    out Rates.HistoricalRates.OpenData[l]);
                double.TryParse(
                    XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@Low"),
                    out Rates.HistoricalRates.LowData[l]);
                double.TryParse(
                    XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@High"),
                    out Rates.HistoricalRates.HighData[l]);
                double.TryParse(
                    XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@Close"),
                    out Rates.HistoricalRates.CloseData[l]);
                int.TryParse(
                    XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@Volume"),
                    out Rates.HistoricalRates.VolumeData[l]);
            }
            Rates.HistoricalRates.RatesLoaded = true;
            // Release.
            _document = null;
            // Record performance for the operation.
            LocalDataLoaded = true;
            Duration = DateTime.Now - StartedOn;
            TrajectoryProcessingTime = Duration.Seconds + @"." + Duration.Milliseconds;
            return numberOfCandles.ToString(CultureInfo.InvariantCulture);
        }
        public string ProcessMetricalData(int numberOfLiveMetrics, string name = "LiveMetrics", string outputFileName = "LiveMetrics_")
        {
            if (numberOfLiveMetrics == 0)
                return "Cannot have zero metrics.";
            Name = name;
            XmsFileName = outputFileName + numberOfLiveMetrics + FileType;
            MonitoringSession.MonitoringSessionType = "Metric";
            StartedOn = DateTime.Now;
            NumberOfLiveMetrics = numberOfLiveMetrics;
            var analysisTrajectory = GlobalSettings.GrabSetting("analysistrajectory");
            switch (analysisTrajectory)
            {
                case "cpupercentage":
                    Trajectory = AnalysisTrajectory.CpuPercentage;
                    break;
                case "memorylimit":
                    Trajectory = AnalysisTrajectory.MemoryLimit;
                    break;
                case "memoryusage":
                    Trajectory = AnalysisTrajectory.MemoryUsage;
                    break;
                case "memorypercentage":
                    Trajectory = AnalysisTrajectory.MemoryPercentage;
                    break;
                case "networkbandwidth":
                    Trajectory = AnalysisTrajectory.NetworkBandwidth;
                    break;
            }
            CandleFormat = GlobalSettings.GrabSetting("candleformat");
            Granularity = GlobalSettings.GrabSetting("granularity");
            DailyAlignment = GlobalSettings.GrabSetting("dailyalignment");
            var result = true;
            //var result = Statistics.ProcessMetrics(NumberOfLiveMetrics.ToString(CultureInfo.InvariantCulture), CandleFormat, Granularity, DailyAlignment, "Europe%2FAmsterdam");
            Logging.WriteLog("Retrieved live candles.", Logging.LogType.Information, Logging.LogCaller.AgentCore);
            Logging.WriteLog("Data trajectory to analyze is: " + Trajectory, Logging.LogType.Information, Logging.LogCaller.AgentCore);
            if (!result) return "Generating the result has failed.";
            switch (Trajectory)
            {
                case AnalysisTrajectory.NetworkBandwidth:
                {
                    Data = new double[NumberOfLiveMetrics];
                    Array.Copy(Statistics.NetworkBandwidthData, 0, Data, 0, NumberOfLiveMetrics);
                }
                    break;
                case AnalysisTrajectory.CpuPercentage:
                {
                    Data = new double[NumberOfLiveMetrics];
                    Array.Copy(Statistics.CpuPercentageData, 0, Data, 0, NumberOfLiveMetrics);
                }
                    break;
                case AnalysisTrajectory.MemoryLimit:
                {
                    Data = new double[NumberOfLiveMetrics];
                    Array.Copy(Statistics.MemoryLimitData, 0, Data, 0, NumberOfLiveMetrics);
                }
                    break;
                case AnalysisTrajectory.MemoryUsage:
                {
                    Data = new double[NumberOfLiveMetrics];
                    Array.Copy(Statistics.MemoryUsageData, 0, Data, 0, NumberOfLiveMetrics);
                }
                    break;
                case AnalysisTrajectory.MemoryPercentage:
                {
                    Data = new double[NumberOfLiveMetrics];
                    Array.Copy(Statistics.MemoryPercentageData, 0, Data, 0, NumberOfLiveMetrics);
                }
                    break;
            }
            try
            {
                SaveStatisticData("Wayward");
            }
            catch (Exception ex)
            {
                Logging.WriteLog("Error in saving candle data (xms). " + ex.Message, Logging.LogType.Error, Logging.LogCaller.AgentCore);
                return "Unable to save the candle data as xms.";
            }
            // Save into separated csv files for Encog for exploration of other types of networks/algorithms/models.
            try
            {
                SaveToCsv();
            }
            catch (Exception ex)
            {
                Logging.WriteLog("Error in saving candle data (csv). " + ex.Message, Logging.LogType.Error, Logging.LogCaller.AgentCore);
                return "Unable to save the candle data as csv.";
            }
            // Record performance vectors.
            LiveDataProcessed = true;
            Duration = DateTime.Now - StartedOn;
            TrajectoryProcessingTime = Duration.Seconds + @"." + Duration.Milliseconds.ToString(CultureInfo.InvariantCulture);
            Logging.WriteLog("Processing time for live data was: " + TrajectoryProcessingTime, Logging.LogType.Information, Logging.LogCaller.AgentCore);
            return "I have processed the requested live data.";
        }
        private void SaveCandleData()
        {
            if (PathForCandleData == null)
                return;
            var writer = XmlWriter.Create(PathForCandleData);
            // Data properties.
            writer.WriteStartElement("RatesData");
            writer.WriteAttributeString("Source", "Oanda");
            writer.WriteAttributeString("NumberOfCandles", Rates.LiveMetrics.NumberOfCandles.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("CandleFormat", Rates.LiveMetrics.CandleFormat);
            writer.WriteAttributeString("DailyAlignment", Rates.LiveMetrics.DailyAlignment.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("Granularity", Rates.LiveMetrics.Granularity);
            // Data elements.
            for (var i = 0; i < Rates.LiveMetrics.NumberOfCandles; i++)
            {
                writer.WriteStartElement("Data");
                writer.WriteAttributeString("Index", i.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("MarketTime", Rates.LiveMetrics.MarketTimeData[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Open", Rates.LiveMetrics.OpenData[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Low", Rates.LiveMetrics.LowData[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("High", Rates.LiveMetrics.HighData[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Close", Rates.LiveMetrics.CloseData[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Volume", Rates.LiveMetrics.VolumeData[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
        }
        public bool RetrieveContainerData(string containerName, string iterations)
        {
            Result = Tasks.ReturnStatistic(containerName);
            if (iterations == "0")
                Iterations = int.Parse(GlobalSettings.GrabSetting("iterations"));
            Iterations = int.Parse(iterations);
            Statistics.NumberOfPoints = Iterations;
            SequenceNumber = 1;
            
            for (int i = 0; i < Iterations; i++)
            {
                Result = Tasks.ReturnStatistic(containerName);
                Statistics.ParseStatistic(Result, false);
                Thread.Sleep(500);
            }
            var result = SaveStatisticData("LiveMetrics_" + containerName);
            if (result)
                return true;

            return false;
        }
        public bool SaveStatisticData(string outputFileName)
        {
            XmsFileName = outputFileName + Iterations + FileType;
            if (PathForCandleData == null)
                return false;
            try
            {
                var writer = XmlWriter.Create(PathForCandleData);
                // Data properties.
                writer.WriteStartElement("StatisticData");
                writer.WriteAttributeString("Source", "Demo");
                writer.WriteAttributeString("Iterations", Iterations.ToString());
                //writer.WriteAttributeString("CandleFormat", Rates.LiveMetrics.CandleFormat);
                // Data elements.
                for (var i = 0; i < Iterations; i++)
                {
                    writer.WriteStartElement("Data");
                    writer.WriteAttributeString("Index", i.ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("StatisticTime", Statistics.TimeStamp.ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("CpuPercentage", Statistics.CpuPercentageData[i].ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("MemoryLimit", Statistics.MemoryLimitData[i].ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("MemoryPercentage", Statistics.MemoryPercentageData[i].ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("MemoryUsage", Statistics.MemoryUsageData[i].ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("NetworkBandwidth", Statistics.NetworkBandwidthData[i].ToString(CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.Flush();
                writer.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.AgentCore);
                return false;
            }

        }
        private static void SaveToCsv()
        {
            try
            {
                var files = new[] { "open", "high", "low", "close", "volume" };
                foreach (var file in files)
                {
                    var writer = new StreamWriter(Path.Combine(Environment.CurrentDirectory + @"\data\csv\", file + ".csv"));
                    var builder = new StringBuilder();
                    builder.Append("date, " + file);
                    writer.WriteLine(builder.ToString());
                    builder.Clear();
                    for (var i = 0; i < Rates.LiveMetrics.NumberOfCandles; i++)
                    {
                        builder.Append(Rates.LiveMetrics.MarketXTimeDate[i].ToString("yyyy-MM-dd HH:mm:ss"));
                        builder.Append(Separator);
                        switch (file)
                        {
                            case "open":
                                builder.Append(Rates.LiveMetrics.OpenData[i].ToString(CultureInfo.InvariantCulture));
                                break;
                            case "high":
                                builder.Append(Rates.LiveMetrics.HighData[i].ToString(CultureInfo.InvariantCulture));
                                break;
                            case "low":
                                builder.Append(Rates.LiveMetrics.LowData[i].ToString(CultureInfo.InvariantCulture));
                                break;
                            case "close":
                                builder.Append(Rates.LiveMetrics.CloseData[i].ToString(CultureInfo.InvariantCulture));
                                break;
                            case "volume":
                                builder.Append(Rates.LiveMetrics.VolumeData[i].ToString(CultureInfo.InvariantCulture));
                                break;
                        }
                        writer.WriteLine(builder.ToString());
                        builder.Clear();
                    }
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.AgentCore);
            }

        }
        // ----------- Create a visual representation
        public bool BuildChart(string fileName)
        {
            if (!Rates.HistoricalRates.RatesLoaded)
            {
                XmsFileName = fileName;// + fileCandles + FileType;
                if (PathForCandleData == null)
                    return false;
                StartedOn = DateTime.Now;
                _document = new XmlDocument();
                try
                {
                    _document.Load(PathForCandleData);
                }
                catch (Exception ex)
                {
                    Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.AgentCore);
                    return false;
                }
            }
            // Load data from xms file.
            const string basePath = "RatesData/Data";
            if (XPathValue("RatesData/@Source") != "Oanda")
                return false;
            int numberOfCandles;
            int.TryParse(XPathValue("RatesData/@NumberOfCandles"), out numberOfCandles);
            Rates.HistoricalRates.NumberOfCandles = numberOfCandles;
            Rates.HistoricalRates.CandleFormat = XPathValue("RatesData/@CandleFormat");
            int dailyAlignment;
            int.TryParse(XPathValue("RatesData/@DailyAlignment"), out dailyAlignment);
            Rates.HistoricalRates.DailyAlignment = dailyAlignment;
            Rates.HistoricalRates.Granularity = XPathValue("RatesData/@Granularity");
            Rates.HistoricalRates.MarketTimeData = new double[numberOfCandles];
            Rates.HistoricalRates.OpenData = new double[numberOfCandles];
            Rates.HistoricalRates.LowData = new double[numberOfCandles];
            Rates.HistoricalRates.HighData = new double[numberOfCandles];
            Rates.HistoricalRates.CloseData = new double[numberOfCandles];
            Rates.HistoricalRates.VolumeData = new int[numberOfCandles];

            for (var l = 0; l < numberOfCandles; l++)
            {
                double.TryParse(
                    XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@MarketTime"),
                    out Rates.HistoricalRates.MarketTimeData[l]);
                double.TryParse(
                    XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@Open"),
                    out Rates.HistoricalRates.OpenData[l]);
                double.TryParse(
                    XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@Low"),
                    out Rates.HistoricalRates.LowData[l]);
                double.TryParse(
                    XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@High"),
                    out Rates.HistoricalRates.HighData[l]);
                double.TryParse(
                    XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@Close"),
                    out Rates.HistoricalRates.CloseData[l]);
                int.TryParse(
                    XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@Volume"),
                    out Rates.HistoricalRates.VolumeData[l]);
            }
            Rates.HistoricalRates.RatesLoaded = true;
            // Release.
            _document = null;
            // Process the data.
            Data = new double[numberOfCandles];
            Array.Copy(Rates.HistoricalRates.HighData, 0, Data, 0, numberOfCandles);
            // Record performance vectors for the operation.
            LocalDataLoaded = true;
            Duration = DateTime.Now - StartedOn;
            TrajectoryProcessingTime = Duration.Seconds + @"." + Duration.Milliseconds;
            return true;
        }
        // ----------- Train deep-solver network
        public string TrainNetwork(TransferFunction transferFunction, string outputFileName = "SolverNetwork")
        {
            TransferFunction = transferFunction;
            Logging.WriteLog(@"Based on the data provided, now training the (solver) network.", Logging.LogType.Information, Logging.LogCaller.AgentCore);
            TrainedNetworkName = outputFileName + FileType;
            // Create performace measure point.
            StartedOn = DateTime.Now;
            // Create the network if it does not already exist.
            if (BackPropagationNetwork == null)
            {
                var layerSizes = new[] { WindowSize, WindowSize * 2, 1 };
                var tFuncs = new[] { TransferFunction.None, TransferFunction, TransferFunction.Linear };
                BackPropagationNetwork = new BackPropagationNetwork(layerSizes, tFuncs) { Name = "SolverNetwork" };
            }
            // Prepare the number of learning samples.
            SampleSize = Data.Length - PredictionSize - WindowSize;
            Input = new double[SampleSize][];
            Desired = new double[SampleSize][];
            // Loop.
            for (var i = 0; i < SampleSize; i++)
            {
                Input[i] = new double[WindowSize];
                Desired[i] = new double[1];
                // Set the input.
                for (var j = 0; j < WindowSize; j++)
                {
                    Input[i][j] = Data[i + j];
                }
                // Set the desired values.
                Desired[i][0] = Data[i + WindowSize];
            }
            // Train the network.
            var count = 0;
            do
            {
                // Prepare for training epoch.
                count++;
                // Train the network.
                for (var i = 0; i < Input.Length; i++)
                {
                    BackPropagationError += BackPropagationNetwork.Train(ref Input[i], ref Desired[i], TrainingRate, Momentum);
                }

                Bias = BackPropagationNetwork.Bias;
                Weight = BackPropagationNetwork.Weight;
                Logging.WriteLog(@"Backpropagation error: " + BackPropagationError.ToString(CultureInfo.InvariantCulture), Logging.LogType.Statistics, Logging.LogCaller.AgentCore);

                for (var i = 0; i < WindowSize; i++)
                    BackPropagationError += BackPropagationNetwork.Train(ref Input[i], ref Desired[i], TrainingRate, Momentum);

                Logging.WriteLog(@"Backpropagation residuals: " + BackPropagationError.ToString(CultureInfo.InvariantCulture), Logging.LogType.Statistics, Logging.LogCaller.AgentCore);

            } while (BackPropagationError > ErrorTolerance && count <= TrainingCycles);

            BackPropagationNetwork.SaveNetworkXml(Environment.CurrentDirectory + @"\data\networks\" + TrainedNetworkName);
            // Record performance vectors for the operation.
            SolverTrainingDone = true;
            Duration = DateTime.Now - StartedOn;
            Logging.WriteLog("Solver training processing time: " + Duration.Seconds + @"." + Duration.Milliseconds.ToString(CultureInfo.InvariantCulture), Logging.LogType.Statistics, Logging.LogCaller.AgentCore);

            return "I have trained the solver network.";
        }
        // ----------- Discover the solution by applying the algorithm to the problem
        public double[,] SearchSolution(double chartRangeYmin, double chartRangeYlength)
        {
            _needToStop = false;
            //_workerThread = new Thread(SearchSolution) { Name = "SearchSolution Thread" };
            StartedOn = DateTime.Now;
            // Set the network properties.
            HiddenLayer = WindowSize * 2;
            OutputLayer = 1;
            // Implement data transformation factors for the chart.
            var factor = 1.7 / chartRangeYlength; //0.00405499999999992
            var yMin = chartRangeYmin; //0.852045
            NetworkInput = new double[WindowSize];

            for (var i = 0; i < SampleSize; i++)
            {
                for (var j = 0; j < WindowSize; j++)
                {
                    Input[i][j] = (Data[i + j] - yMin) * factor - NetworkCorrection;
                }

                Desired[i][0] = (Data[i + WindowSize] - yMin) * factor - NetworkCorrection;
            }
            // Todo: Add the new code to the Bph library starting at these points below.
            switch (TransferFunction.ToString())
            {
                case "BipolarSigmoid":
                    Activation = new ActivationNetwork(new BipolarSigmoidFunction(SigmoidAlpha), WindowSize, HiddenLayer, OutputLayer);
                    break;
                case "Gaussian":
                    break;
                case "Linear":
                    break;
                case "NormalizedExponent":
                    break;
                case "RationalSigmoid":
                    break;
                case "Sigmoid":
                    Activation = new ActivationNetwork(new SigmoidFunction(SigmoidAlpha), WindowSize, HiddenLayer, OutputLayer);
                    break;
                case "VanderPol":
                    break;
            }
            NumberOfLayers = 2;
            LayerSize = new int[NumberOfLayers];
            LayerSize[0] = HiddenLayer;
            LayerSize[1] = OutputLayer;
            // One set of classes containing activation functions, from the first iteration of this code.
            SolverNetwork = new BackPropagation(Activation) { LearningRate = LearningRate, Momentum = Momentum };
            // This is here but have no historical trace. What send an array of zeros to the algorithm?
            Activation.Compute(NetworkInput); // This is an array of zeros. Can we feed the algorithm's correction here?
            LearningError += Math.Abs(Data[1] - ((Activation.Compute(NetworkInput)[0] + NetworkCorrection) / factor + yMin));
            // Iterations.
            var iteration = 1;
            // Solution array.
            SolutionSize = Data.Length - WindowSize;
            Solution = new double[SolutionSize, 2];
            // Calculate x-values to be used with solution function.
            for (var j = 0; j < SolutionSize; j++)
            {
                Solution[j, 0] = j + WindowSize;
            }
            // Loop.
            while (!_needToStop)
            {
                // Run epoch of learning procedure.
                Epoch = SolverNetwork.RunEpoch(Input, Desired) / SampleSize;
                // Calculate solution, learning, and prediction errors by iterating the data.
                for (int i = 0, n = Data.Length - WindowSize; i < n; i++)
                {
                    // Assign values from current window as network's input.
                    for (var j = 0; j < WindowSize; j++)
                    {
                        NetworkInput[j] = (Data[i + j] - yMin) * factor - NetworkCorrection;
                    }
                    // Evaluate the function.
                    Solution[i, 1] = (Activation.Compute(NetworkInput)[0] + NetworkCorrection) / factor + yMin;
                    // Truncate to a pre-chosen set of decimal places.
                    Solution[i, 1] = Math.Round(Solution[i, 1], TrailingDigits);
                    // Compute the prediction and learning error.
                    if (i >= n - PredictionSizeTrend)
                    {
                        PredictionError += Math.Abs(Solution[i, 1] - Data[WindowSize + i]);
                    }
                    else
                    {
                        LearningError += Math.Abs(Solution[i, 1] - Data[WindowSize + i]);
                    }
                }
                // Set the dataset maximum and minimum values.
                DataMagnitudeUpper = Data.Max();
                DataMagnitudeLower = Data.Min();
                // Increase the current iteration.
                iteration++;
                // Check if we need to stop.
                if ((Iterations != 0) && (iteration > Iterations))
                {
                    //_workerThread.Abort();
                    _needToStop = true;
                    //break;
                }
            }
            // Record performance vectors for the operation and logfile.
            Duration = DateTime.Now - StartedOn;
            Logging.WriteLog("Neural network processed its given task in " + Duration.Seconds + @"." + Duration.Milliseconds + " seconds", Logging.LogType.Statistics, Logging.LogCaller.AgentCore);
            Logging.WriteLog("Program metrics: At an epoch of " + Epoch + ", " + Iterations + " iterations, " + LearningRate + " learning rate, " + PredictionSizeTrend + " prediction size for the trend computation; a sigmoid alpha value of " + SigmoidAlpha + " and a momentum of " + Momentum + " resulting in a learning error of " + LearningError + " and a prediction error of " + PredictionError, Logging.LogType.Statistics, Logging.LogCaller.AgentCore);
            return Solution;
        }
        public double[,] SearchForecastSolution(double chartRangeYmin, double chartRangeYlength, bool combineWithSolution = false)
        {
            // Todo: Rewrite this method leveraging the knowledge gained by the agent on the problem space from the solver network.
            StartedOn = DateTime.Now;
            // Data transformation factors for the chart.
            var factor = 1.7 / chartRangeYlength;
            var yMin = chartRangeYmin;
            // Create a fresh network input.
            NetworkInput = new double[ForecastSize];
            Forecast = new double[ForecastSize, 2];
            // Calculate x-values to be used with forecast function.
            for (var j = 0; j < ForecastSize; j++)
            {
                Forecast[j, 0] = j;
            }
            if (Solution == null)
                return null;
            // Take the last elements from the solution of a size matched by the future size.
            ForecastData = Solution.GetLastValuesOf(ForecastSize);
            // Calculate forecast assigning values from current forcast size as network's input.
            for (var i = 0; i < ForecastSize; i++)
            {
                NetworkInput[i] = (ForecastData[i] - yMin) * factor - NetworkCorrection;
                // Evaluate the function.
                Forecast[i, 1] = (Activation.Compute(NetworkInput)[0] + NetworkCorrection) / factor + yMin;
                ForecastError += Math.Abs(Forecast[i, 1] - ForecastData[i]);
                ForecastDifference = new Tuple<double, double, double>(ForecastData[i], Forecast[i, 1],
                    Math.Abs(ForecastData[i] - Forecast[i, 1]));
                ForecastDifferences.Add(ForecastDifference);
            }
            //var averageMean = ForecastDifferences.AverageMean();
            ForecastValueNext = ForecastDifferences.PickLastForecast();
            if (combineWithSolution)
            {
                // Combine the forecast with the solution data and replot on the chart.
                Forecast = Solution.CombineArray(Forecast);
            }
            // Record performance vectors for the operation.
            Duration = DateTime.Now - StartedOn;
            Logging.WriteLog("The search for the forecast solution completed in " + Duration.Seconds + @"." + Duration.Milliseconds + " seconds", Logging.LogType.Statistics, Logging.LogCaller.AgentCore);
            return Forecast;

        }
        // ----------- Trading
        public string ProspectTrade()
        {
            BuyOrSell = Decision.MakeDecision(ForecastValueNext, TradingDifferenceMagnitude);
            //DataMagnitudeUpper = Math.Round(DataMagnitudeUpper, 5);
            //DataMagnitudeLower = Math.Round(DataMagnitudeLower, 5);

            // Step 4: Decide whether or not to buy, sell, or keep the instrument position.
            if (Decision.Buy)
            {
                //Logging.WriteLog("Trend-solution says: " + BuyOrSellTrendSolution + " while the Raw data-trend says " + BuyOrSellTrendRawData + ".", Logging.LogType.Statistics, Logging.LogCaller.AgentCore);
                //var differential = DataMagnitudeUpper - DataMagnitudeLower;
                //Logging.WriteLog(differential.ToString(CultureInfo.InvariantCulture), Logging.LogType.Statistics, Logging.LogCaller.AgentCore);
                return "Buy";
                //PlaceOrder();
            }
            if (Decision.Sell)
            {
                //Logging.WriteLog("Trend-solution says: " + BuyOrSellTrendSolution + " while the Raw data-trend says " + BuyOrSellTrendRawData + ".", Logging.LogType.Statistics, Logging.LogCaller.AgentCore);
                //var differential = DataMagnitudeUpper - DataMagnitudeLower;
                //Logging.WriteLog(differential.ToString(CultureInfo.InvariantCulture), Logging.LogType.Statistics, Logging.LogCaller.AgentCore);
                return "Sell";
                //PlaceOrder("sell");
            }
            if (Decision.Keep)
            {
                //Logging.WriteLog(@"Not enough difference to substantiate the trade.", Logging.LogType.Statistics, Logging.LogCaller.AgentCore);
                return "Not enough difference to substantiate the trade.";
            }
            // Step 5: Perform post-processing to see how effective the trade was in terms of (+) or (-) magnitudes.
            //DataMagnitudeUpper and DataMagnitudeLower are serving this function now.
            return BuyOrSell;

        }
        public void PlaceOrder(int numberOfUnits, string side = "buy")
        {
            try
            {
                var result = Orders.PostMarketOrder(ReturnAccountID(), FormatInstrument(), numberOfUnits, side);
                Logging.WriteLog("PlaceOrder" + side + ": " + result, Logging.LogType.Information, Logging.LogCaller.AgentCore, "Orders.PostMarketOrder");
            }
            catch (Exception ex)
            {
                Logging.WriteLog("Orders.PostMarketOrder failed: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.AgentCore);
            }
            
        }
        public void SellInstrument()
        {
            
        }

        #region Utilities
        public void LoadSettings()
        {
            var path = Path.Combine(Environment.CurrentDirectory, Path.Combine("config", "Settings.xml"));
            GlobalSettings.LoadSettings(path);
        }
        private string XPathValue(string xPath)
        {
            var node = _document.SelectSingleNode((xPath));
            if (node == null)
                Logging.WriteLog(@"Cannot find the specified node.", Logging.LogType.Error, Logging.LogCaller.AgentCore, "XPathValue");
            return node != null ? node.InnerText : "";
        }
        public string FormatInstrument()
        {
            var firstPair = HoldingCurrency;
            var secondPair = TradingCurrency;
            return firstPair + "_" + secondPair;
        }
        public int ReturnAccountID()
        {
            int session;
            if (MonitoringSession.MonitoringSessionType == MonitoringSession.MonitoringSessionAccount.Practice.ToString())
            {
                session = (int)MonitoringSession.MonitoringSessionAccount.Practice;
            }
            else
            {
                session = (int)MonitoringSession.MonitoringSessionAccount.Live;
            }
            return session;
        }
        #endregion

        #region To be removed - Check relevance of search using softmax (i think currently on the call to the compute method is correct, otherwise, not).
        //public string ProspectTrade()
        //{
        //    // Assumption #1: When the Epoch is less than 0.0050, the prediction is fairly accurate on the first iteration of a solution.

        //    // Preparation: Create the objects for the autonomous session.
        //    TrendFromDataStrings = new List<string>();
        //    TrendFromSolutionStrings = new List<string>();
        //    BuyOrSellTrendSolution = "";
        //    BuyOrSellTrendRawData = "";
        //    // Step 1: Check the automat data is loaded. Run the solution.
        //    if (Rates.HistoricalRates.RatesLoaded | Rates.LiveRates.RatesLoaded)
        //    // Check against the rates, be them live or historical.
        //    {
        //        if (Solution == null)
        //        {
        //            //TrainSolverNetwork();
        //            //CastSolution();
        //        }
        //        // Step 2: Analyze the points within the prediction size window and check for an upward or downward trend in the given data point series length.
        //        // a: Get the values from the prediction series.
        //        PredictionValuesSolution = Solution.GetLastValuesOf(PredictionSizeTrend);
        //        PredictionValuesRawData = Data.GetPredictionWindowValues(PredictionSizeTrend);
        //        // b: Compile a list of the trend: true if value greater than previous, false if not.
        //        TrendSolution = PredictionValuesSolution.IsGreaterThanPrevious(PredictionSizeTrend);
        //        TrendRawData = PredictionValuesRawData.IsGreaterThanPrevious(PredictionSizeTrend);
        //        // c: Send the trend to the decision engine.
        //        BuyOrSellTrendSolution = Decision.MakeDecision(TrendSolution, Decision.DataStreamType.Solution);
        //        // Decision based on the solution.
        //        BuyOrSellTrendRawData = Decision.MakeDecision(TrendRawData, Decision.DataStreamType.Raw);
        //        // Decision based on raw data.
        //        // d: Trim the magnitudes of the dataset.
        //        DataMagnitudeUpper = Math.Round(DataMagnitudeUpper, 5);
        //        DataMagnitudeLower = Math.Round(DataMagnitudeLower, 5);
        //    }
        //    else
        //    {
        //        Logging.WriteLog(@"Candle data has not been loaded, therefore no solution is to be found.", Logging.LogType.Error, Logging.LogCaller.AgentCore);
        //        return "Candle data has not been loaded, therefore no solution is to be found.";
        //    }
        //    // Step 4: Decide whether or not to buy or sell the instrument position.
        //    if (Decision.Buy)
        //    {
        //        Logging.WriteLog("Trend-solution says: " + BuyOrSellTrendSolution + " while the Raw data-trend says " + BuyOrSellTrendRawData + ".", Logging.LogType.Statistics, Logging.LogCaller.AgentCore);
        //        var differential = DataMagnitudeUpper - DataMagnitudeLower;
        //        Logging.WriteLog(differential.ToString(CultureInfo.InvariantCulture), Logging.LogType.Statistics, Logging.LogCaller.AgentCore);
        //        return "Trend-solution says: " + BuyOrSellTrendSolution + " while the Raw data-trend says " +
        //               BuyOrSellTrendRawData + ".";
        //        //PlaceOrder();
        //    }
        //    if (Decision.Sell)
        //    {
        //        Logging.WriteLog("Trend-solution says: " + BuyOrSellTrendSolution + " while the Raw data-trend says " + BuyOrSellTrendRawData + ".", Logging.LogType.Statistics, Logging.LogCaller.AgentCore);
        //        var differential = DataMagnitudeUpper - DataMagnitudeLower;
        //        Logging.WriteLog(differential.ToString(CultureInfo.InvariantCulture), Logging.LogType.Statistics, Logging.LogCaller.AgentCore);
        //        return "Trend-solution says: " + BuyOrSellTrendSolution + " while the Raw data-trend says " +
        //               BuyOrSellTrendRawData + ".";
        //        //PlaceOrder("sell");
        //    }
        //    if (Decision.Cheese)
        //    {
        //        Logging.WriteLog(@"Not enough difference to substantiate the trade.", Logging.LogType.Statistics, Logging.LogCaller.AgentCore);
        //        return "Not enough difference to substantiate the trade.";
        //    }
        //    // Step 5: Perform post-processing to see how effective the trade was in terms of (+) or (-) magnitudes.
        //    //DataMagnitudeUpper and DataMagnitudeLower are serving this function now.
        //    return "Trade prospect complete.";

        //}
        //public void SearchForecastUsingSoftmax()
        //{
        //    // Todo: Needs to be completed.
        //    StartedOn = DateTime.Now;
        //    // Forecast array.
        //    var forecastSize = ForecastSize;
        //    var forecastSolution = new double[forecastSize, 2];
        //    // Parse the number of iterations.
        //    //Iterations = 2000;
        //    // Create the network if it does not already exist.

        //    // Calculate x-values to be used with forecast function.
        //    for (var j = 0; j < forecastSize; j++)
        //    {
        //        forecastSolution[j, 0] = j;
        //    }
        //    //TrainForecastNetwork("SoftmaxForecastNetwork");
        //    var output = new double[1];
        //    var networkInput = NetworkInput;
        //    var networkOutput = new List<double[]>();
        //    for (var i = 0; i < ForecastSize; i++)
        //    {
        //        CustomNetwork.RunEvaluation(ref networkInput, out output);
        //        networkOutput.Add(output);
        //    }
        //    // Compute the softmax.
        //    var softmaxFirst = CustomNetwork.ComputeSoftmax(Boagaphish.Core.Networks.Network.CrossLayer.InputHidden, NetworkInput);
        //    var softmaxSecond = CustomNetwork.ComputeSoftmax(Boagaphish.Core.Networks.Network.CrossLayer.HiddenOutput, NetworkInput);
        //    // This is where the design stops. Continue on.
        //    //var hold = 0;

        //}
        //public string TrainForecastNetwork(string outputFileName = "ForecastNetwork")
        //{
        //    Logging.WriteLog(@"Training the forecast network.", Logging.LogType.Information, Logging.LogCaller.AgentCore);
        //    TrainedNetworkName = outputFileName + FileType;
        //    StartedOn = DateTime.Now;
        //    if (ForecastNetwork == null)
        //    {
        //        var layerSizes = new[] { WindowSize, WindowSize * 2, 1 };
        //        var functions = new[] { TransferFunction.None, TransferFunction, TransferFunction.Linear };
        //        ForecastNetwork = new BackPropagationNetwork(layerSizes, functions) { Name = "ForecastNetwork" };
        //    }
        //    // Prepare the number of learning samples.
        //    SampleSize = Data.Length - PredictionSize - WindowSize;
        //    Input = new double[SampleSize][];
        //    Desired = new double[SampleSize][];
        //    // Loop.
        //    for (var i = 0; i < SampleSize; i++)
        //    {
        //        Input[i] = new double[WindowSize];
        //        Desired[i] = new double[1];
        //        // Set the input.
        //        for (var j = 0; j < WindowSize; j++)
        //        {
        //            Input[i][j] = Data[i + j];
        //        }
        //        // Set the desired values.
        //        Desired[i][0] = Data[i + WindowSize];
        //    }
        //    // Train the network.
        //    var count = 0;
        //    do
        //    {
        //        // Prepare for training epoch.
        //        count++;
        //        // Train the network.
        //        for (var i = 0; i < Input.Length; i++)
        //        {
        //            ForecastError += ForecastNetwork.Train(ref Input[i], ref Desired[i], TrainingRate, Momentum);
        //        }

        //    } while (BackPropagationError > ErrorTolerance && count <= TrainingCycles);

        //    ForecastNetwork.SaveNetworkXml(Environment.CurrentDirectory + @"\data\xms\" + TrainedNetworkName);
        //    // Record performance vectors for the operation.
        //    ForecastTrainingDone = true;
        //    Duration = DateTime.Now - StartedOn;
        //    Logging.WriteLog("Forecast training processing time: " + Duration.Seconds + @"." + Duration.Milliseconds.ToString(CultureInfo.InvariantCulture), Logging.LogType.Statistics, Logging.LogCaller.AgentCore);
        //    return "I have trained the forecast network.";
        //}
        //private void SearchSolution()
        //{
        //    //WindowSize = 5;
        //    PredictionSizeTrend = 7;
        //    StartedOn = DateTime.Now;
        //    // Set the network properties.
        //    HiddenLayer = WindowSize * 2;
        //    OutputLayer = 1;
        //    // Implement data transformation factors for the chart.
        //    var factor = 1.7 / _trajectoryChart.RangeY.Length; //0.00405499999999992
        //    var yMin = _trajectoryChart.RangeY.Min; //0.852045
        //    // Prepare the network.
        //    var networkInput = new double[WindowSize];
        //    // Loop.
        //    for (var i = 0; i < SampleSize; i++)
        //    {
        //        // Set the input.
        //        for (var j = 0; j < WindowSize; j++)
        //        {
        //            Input[i][j] = (_data[i + j] - yMin) * factor - NetworkCorrection;
        //        }
        //        // Set the output.
        //        Desired[i][0] = (_data[i + WindowSize] - yMin) * factor - NetworkCorrection;
        //    }
        //    // Create a multi-layer neural network.
        //    Network = new ActivationNetwork(new BipolarSigmoidFunction(SigmoidAlpha), WindowSize, HiddenLayer,
        //        OutputLayer);
        //    NumberOfLayers = 2;
        //    LayerSize = new int[NumberOfLayers];
        //    LayerSize[0] = HiddenLayer;
        //    LayerSize[1] = OutputLayer;
        //    // Create a teacher. Set the learning rate and momentum.
        //    LearningNetwork = new BackPropagation(Network) { LearningRate = LearningRate, Momentum = Momentum };
        //    // This is here but have no historical trace.
        //    Network.Compute(networkInput); // This is an array of zeros. Can we feed the algorithm's correction here?
        //    LearningError += Math.Abs(_data[1] - ((Network.Compute(networkInput)[0] + NetworkCorrection) / factor + yMin));
        //    // Iterations.
        //    var iteration = 1;
        //    // Solution array.
        //    var solutionSize = _data.Length - WindowSize;
        //    var solution = new double[solutionSize, 2];
        //    // Calculate x-values to be used with solution function.
        //    for (var j = 0; j < solutionSize; j++)
        //    {
        //        solution[j, 0] = j + WindowSize;
        //    }
        //    // Loop.
        //    while (!_needToStop)
        //    {
        //        // Run epoch of learning procedure.
        //        Epoch = LearningNetwork.RunEpoch(Input, Desired) / SampleSize;
        //        // Calculate solution, learning, and prediction errors by iterating the data.
        //        for (int i = 0, n = _data.Length - WindowSize; i < n; i++)
        //        {
        //            // Assign values from current window as network's input.
        //            for (var j = 0; j < WindowSize; j++)
        //            {
        //                networkInput[j] = (_data[i + j] - yMin) * factor - NetworkCorrection;
        //            }
        //            // Evaluate the function.
        //            solution[i, 1] = (Network.Compute(networkInput)[0] + NetworkCorrection) / factor + yMin;
        //            // Truncate to a pre-chosen set of decimal places.
        //            solution[i, 1] = Math.Round(solution[i, 1], TrailingDigits);
        //            // Compute the prediction and learning error.
        //            if (i >= n - PredictionSizeTrend)
        //            {
        //                PredictionError += Math.Abs(solution[i, 1] - _data[WindowSize + i]);
        //            }
        //            else
        //            {
        //                LearningError += Math.Abs(solution[i, 1] - _data[WindowSize + i]);
        //            }
        //        }
        //        // Update the solution on the chart object (off-thread).
        //        _trajectoryChart.UpdateDataSeries("solution", solution);
        //        Solution = solution;
        //        Data = _data;
        //        // Set the dataset maximum and minimum values.
        //        DataMagnitudeUpper = _data.Max();
        //        DataMagnitudeLower = _data.Min();
        //        // Increase the current iteration.
        //        iteration++;
        //        // Check if we need to stop.
        //        if ((Iterations != 0) && (iteration > Iterations))
        //        {
        //            _workerThread.Abort();
        //            _needToStop = true;
        //            //break;
        //        }
        //    }
        //    // Report the behavioural attributes.

        //    // Record performance vectors for the operation and logfile.
        //    Duration = DateTime.Now - StartedOn;
        //    Logging.WriteLog("Neural network processed its given task in " + Duration.Seconds + @"." + Duration.Milliseconds + " seconds", Logging.LogType.Statistics, Logging.LogCaller.AgentCore);
        //    Logging.WriteLog("Program metrics: At an epoch of " + Epoch + ", " + Iterations + " iterations, " + LearningRate + " learning rate, " + PredictionSizeTrend + " prediction size for the trend computation; a sigmoid alpha value of " + SigmoidAlpha + " and a momentum of " + Momentum + " resulting in a learning error of " + LearningError + " and a prediction error of " + PredictionError, Logging.LogType.Statistics, Logging.LogCaller.AgentCore);
        //}
        // ----------- Chart-related operations (GPU)
        //private void UpdateDelimiters()
        //{
        //    if (_data == null) return;
        //    // The window delimiter.
        //    _windowDelimiter[0, 0] = _windowDelimiter[1, 0] = WindowSize;
        //    _windowDelimiter[0, 1] = _trajectoryChart.RangeY.Min;
        //    _windowDelimiter[1, 1] = _trajectoryChart.RangeY.Max;
        //    _trajectoryChart.UpdateDataSeries("window", _windowDelimiter);
        //    // The prediction delimiter.
        //    _predictionDelimiter[0, 0] = _predictionDelimiter[1, 0] = _data.Length - 1 - PredictionSize;
        //    _predictionDelimiter[0, 1] = _trajectoryChart.RangeY.Min;
        //    _predictionDelimiter[1, 1] = _trajectoryChart.RangeY.Max;
        //    _trajectoryChart.UpdateDataSeries("prediction", _predictionDelimiter);
        //    // The future delimiter.
        //    _futureDelimiter[0, 0] = _futureDelimiter[1, 0] = _data.Length - 1;
        //    _futureDelimiter[0, 1] = _trajectoryChart.RangeY.Min;
        //    _futureDelimiter[1, 1] = _trajectoryChart.RangeY.Max;
        //    _trajectoryChart.UpdateDataSeries("future", _futureDelimiter);
        //}
        //public string TrainForecastNetwork(string outputFileName = "ForecastNetwork")
        //{
        //    Logging.WriteLog(@"Training the forecast network.", Logging.LogType.Information, Logging.LogCaller.AgentCore);
        //    TrainedNetworkName = outputFileName + FileType;
        //    StartedOn = DateTime.Now;
        //    if (ForecastNetwork == null)
        //    {
        //        var layerSizes = new[] { WindowSize, WindowSize * 2, 1 };
        //        var functions = new[] { TransferFunction.None, TransferFunction, TransferFunction.Linear };
        //        ForecastNetwork = new BackPropagationNetwork(layerSizes, functions) { Name = "ForecastNetwork" };
        //    }
        //    // Prepare the number of learning samples.
        //    SampleSize = _data.Length - PredictionSize - WindowSize;
        //    Input = new double[SampleSize][];
        //    Desired = new double[SampleSize][];
        //    // Loop.
        //    for (var i = 0; i < SampleSize; i++)
        //    {
        //        Input[i] = new double[WindowSize];
        //        Desired[i] = new double[1];
        //        // Set the input.
        //        for (var j = 0; j < WindowSize; j++)
        //        {
        //            Input[i][j] = _data[i + j];
        //        }
        //        // Set the desired values.
        //        Desired[i][0] = _data[i + WindowSize];
        //    }
        //    // Train the network.
        //    var count = 0;
        //    do
        //    {
        //        // Prepare for training epoch.
        //        count++;
        //        // Train the network.
        //        for (var i = 0; i < Input.Length; i++)
        //        {
        //            ForecastError += ForecastNetwork.Train(ref Input[i], ref Desired[i], TrainingRate, Momentum);
        //        }

        //    } while (BackPropagationError > ErrorTolerance && count <= TrainingCycles);

        //    ForecastNetwork.SaveNetworkXml(Environment.CurrentDirectory + @"\data\xms\" + TrainedNetworkName);
        //    // Record performance vectors for the operation.
        //    ForecastTrainingDone = true;
        //    Duration = DateTime.Now - StartedOn;
        //    Logging.WriteLog("Forecast training processing time: " + Duration.Seconds + @"." + Duration.Milliseconds.ToString(CultureInfo.InvariantCulture), Logging.LogType.Statistics, Logging.LogCaller.AgentCore);
        //    return "I have trained the forecast network.";
        //}
        // ----------- Search for a solution
        //public string CastSolution()
        //{
        //    Logging.WriteLog(@"Casting the solution.", Logging.LogType.Information, Logging.LogCaller.AgentCore);
        //    if (_abortRequested)
        //        return "Abort has been requesting. Stopping solution search.";
        //    if (BackPropagationNetwork == null)
        //        return "Empty network, cannot continue.";
        //    StartedOn = DateTime.Now;
        //    // Initialize properties based on the current selection.
        //    //UpdateDelimiters();
        //    _needToStop = false;
        //    //_workerThread = new Thread(SearchSolution) { Name = "SearchSolution Thread" };
        //    //_workerThread = new Thread(SearchOldSolution) { Name = "SearchAlternateSolution Thread" };
        //    //SearchAlternateSolution(); // Stored in the boneyard. Spent a lot of time working it out, but have acquired the training methods and the xms parts. I will need to optimise training in the future.
        //    try
        //    {
        //        SearchSolution();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logging.WriteLog("Cast the solution has failed: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.AgentCore);
        //        return "Cast solution has failed.";
        //    }

        //    return "A solution has been found";
        //    //_workerThread.Start();
        //}
        //public string TrainSolverNetwork(string outputFileName = "SolverNetwork")
        //{
        //    Logging.WriteLog(@"Training the solver network.", Logging.LogType.Information, Logging.LogCaller.AgentCore);
        //    TrainedNetworkName = outputFileName + FileType;
        //    // Create performace measure point.
        //    StartedOn = DateTime.Now;
        //    // Create the network if it does not already exist.
        //    if (BackPropagationNetwork == null)
        //    {
        //        var layerSizes = new[] { WindowSize, WindowSize * 2, 1 };
        //        var tFuncs = new[] { TransferFunction.None, TransferFunction, TransferFunction.Linear };
        //        BackPropagationNetwork = new BackPropagationNetwork(layerSizes, tFuncs) { Name = "SolverNetwork" };
        //    }
        //    // Prepare the number of learning samples.
        //    SampleSize = _data.Length - PredictionSize - WindowSize;
        //    Input = new double[SampleSize][];
        //    Desired = new double[SampleSize][];
        //    // Loop.
        //    for (var i = 0; i < SampleSize; i++)
        //    {
        //        Input[i] = new double[WindowSize];
        //        Desired[i] = new double[1];
        //        // Set the input.
        //        for (var j = 0; j < WindowSize; j++)
        //        {
        //            Input[i][j] = _data[i + j];
        //        }
        //        // Set the desired values.
        //        Desired[i][0] = _data[i + WindowSize];
        //    }
        //    // Train the network.
        //    var count = 0;
        //    do
        //    {
        //        // Prepare for training epoch.
        //        count++;
        //        // Train the network.
        //        for (var i = 0; i < Input.Length; i++)
        //        {
        //            BackPropagationError += BackPropagationNetwork.Train(ref Input[i], ref Desired[i], TrainingRate, Momentum);
        //        }

        //        Bias = BackPropagationNetwork.Bias;
        //        Weight = BackPropagationNetwork.Weight;
        //        Logging.WriteLog(@"Backpropagation error: " + BackPropagationError.ToString(CultureInfo.InvariantCulture), Logging.LogType.Statistics, Logging.LogCaller.AgentCore);

        //        for (var i = 0; i < WindowSize; i++)
        //            BackPropagationError += BackPropagationNetwork.Train(ref Input[i], ref Desired[i], TrainingRate, Momentum);

        //        Logging.WriteLog(@"Backpropagation residuals: " + BackPropagationError.ToString(CultureInfo.InvariantCulture), Logging.LogType.Statistics, Logging.LogCaller.AgentCore);

        //    } while (BackPropagationError > ErrorTolerance && count <= TrainingCycles);

        //    BackPropagationNetwork.SaveNetworkXml(Environment.CurrentDirectory + @"\data\xms\" + TrainedNetworkName);
        //    // Record performance vectors for the operation.
        //    SolverTrainingDone = true;
        //    Duration = DateTime.Now - StartedOn;
        //    Logging.WriteLog("Solver training processing time: " + Duration.Seconds + @"." + Duration.Milliseconds.ToString(CultureInfo.InvariantCulture), Logging.LogType.Statistics, Logging.LogCaller.AgentCore);

        //    return "I have trained the solver network.";
        //}
        #endregion
    }
}
