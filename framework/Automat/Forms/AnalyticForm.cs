using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using Boagaphish;
using Boagaphish.ActivationFunctions;
using Boagaphish.Controls;
using Boagaphish.Core;
using Boagaphish.Core.Learning;
using Boagaphish.Core.Networks;
using Boagaphish.Custom;
using Boagaphish.Numeric;
using SoftAgent.Core;
using Timer = System.Windows.Forms.Timer;

namespace Automat.Forms
{
    public partial class AnalyticForm : Form
    {
        private readonly ListView _dataListView;
        private Thread _workerThread;
        private volatile bool _stopThread;
        private readonly object _lock = new object();
        private bool _threadRunning;
        private bool _abortRequested;
        private bool _needToStop;
        private double[] _data;
        private double[,] _dataToShow;
        private const char Separator = ',';
        delegate void SetTrainingCallback(double backPropagationError);
        delegate void SetForecastCallback(double forecastError);
        delegate void SetEpochCallback(double epoch);
        delegate void SetLearningErrorCallback(double learningError);
        delegate void SetPredictionErrorCallback(double predictionError);
        delegate void SetAlgorithmPerformanceCallback(TimeSpan duration);
        delegate void SetIterationCallback(int iteration);
        private readonly double[,] _windowDelimiter = { { 0, 0 }, { 0, 0 } };
        private readonly double[,] _predictionDelimiter = { { 0, 0 }, { 0, 0 } };
        private readonly double[,] _futureDelimiter = { { 0, 0 }, { 0, 0 } };
        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public AnalyticForm()
        {
            InitializeComponent();
            // Default settings for the form.
            //PathToXms = Environment.GetFolderPath(Environment.CurrentDirectory);
            candleFormatBox.SelectedItem = "midpoint";
            numberOfCandlesBox.Text = @"120";
            granularityBox.SelectedItem = "M1";
            dailyAlignmentBox.SelectedItem = "0";
            predictionSizeBox.SelectedItem = "7";// Ten minute resolution on prediction.
            windowSizeBox.SelectedItem = "5";
            futureSizeBox.SelectedItem = windowSizeBox.SelectedItem;// 'Peer' the same time into the future as the window size.
            iterationsBox.Text = @"2000";
            trainingCyclesBox.Text = @"2000";
            learningRateBox.Text = @"0.1";
            momentumBox.Text = @"0";
            sigmoidAlphaBox.Text = @"2";
            runtimeDuration.SelectedItem = "5";
            TrainingRate = 0.15;
            ErrorTolerance = 1E-5;
            PopulateListBox(xmsFilepathBox, PathToXms, "*.xml");
            transferFunctionSelectionBox.SelectedItem = "Gaussian";
            transferFunctionSelectionBox.Enabled = true;
            TransferFunctions.Alpha = 2;
            // Set the value of the combo boxes to their respective enum values.
            analysisTrajectoryBox.DataSource = Enum.GetValues(typeof(Statistics.Trajectory));
            //currencyFromBox.DataSource = Enum.GetValues(typeof(LiveRates.CurrenciesFrom));
            //currencyToBox.DataSource = Enum.GetValues(typeof(LiveRates.CurrenciesTo));
            Rates.HoldingCurrency = currencyFromBox.SelectedItem.ToString();
            Rates.TradingCurrency = currencyToBox.SelectedItem.ToString();
            switchCurrencyFromToBox.Checked = false;
            sessionTypeBox.DataSource = Enum.GetValues(typeof(MonitoringSession.MonitoringSessionAccount));
            LearningError = 0.0;
            PredictionError = 0.0;
            _dataListView = new ListView();
            // Initialize a chart control.
            //trajectoryChart.AddDataSeries("xmsdata", Color.BlueViolet, Chart.SeriesType.Dots, 5);
            trajectoryChart.AddDataSeries("data", Color.Black, Chart.SeriesType.ConnectedDots, 5);
            trajectoryChart.AddDataSeries("solution", Color.Blue, Chart.SeriesType.ConnectedDots, 5);
            trajectoryChart.AddDataSeries("forecastFromSolution", Color.Chartreuse, Chart.SeriesType.ConnectedDots, 5);
            trajectoryChart.AddDataSeries("forecastCustom", Color.Crimson, Chart.SeriesType.ConnectedDots, 5);
            trajectoryChart.AddDataSeries("window", Color.LightGray, Chart.SeriesType.Line, 1, false);
            trajectoryChart.AddDataSeries("prediction", Color.Gray, Chart.SeriesType.Line, 1, false);
            trajectoryChart.AddDataSeries("future", Color.Green, Chart.SeriesType.Line, 2, false);
            // Initialize global settings.
            SigmoidAlpha = double.Parse(sigmoidAlphaBox.Text);
            Iterations = int.Parse(iterationsBox.Text);
            TrainingCycles = int.Parse(trainingCyclesBox.Text);
            LearningRate = double.Parse(learningRateBox.Text);
            Momentum = double.Parse(momentumBox.Text);
            PredictionSize = int.Parse(predictionSizeBox.SelectedItem.ToString());
            WindowSize = int.Parse(windowSizeBox.SelectedItem.ToString());
            ForecastSize = int.Parse(futureSizeBox.SelectedItem.ToString());
            //int.TryParse(windowSizeBox.SelectedItem.ToString(), WindowSize);
            HistoricalStatistics.WindowSize = WindowSize;
            NumberOfPoints = numberOfCandlesBox.Text;
            TrailingDigits = 8;
            NetworkCorrection = 0.85;
        }
        private void AlgorithmComputationLoad(object sender, EventArgs e)
        {
            var toolTip = new ToolTip { AutoPopDelay = 10000, InitialDelay = 400, ReshowDelay = 250, ShowAlways = true };
            toolTip.SetToolTip(okButton, "Clear notifications.");
            toolTip.SetToolTip(getLiveRatesButton, "Get the live rate trajectory.");
            toolTip.SetToolTip(getXmsRatesButton, "Get the historical rate trajectory.");
            toolTip.SetToolTip(trainNetworkButton, "Train the network on the data which is loaded.");
            toolTip.SetToolTip(candlesViewerXmsButton, "Open the candles viewer for the historical data.");
            toolTip.SetToolTip(saveXmsDataButton, "Save the current dataset into the xms repository.");
            toolTip.SetToolTip(candlesViewerLiveButton, "Open the candles viewer for the live data.");
            toolTip.SetToolTip(searchSolutionButton, "Search for the trend in the prediction window.");
            toolTip.SetToolTip(refreshChartButton, "Refresh the chart.");
            toolTip.SetToolTip(tradesManagerButton, "Open the form showing the trades which have been made.");
            toolTip.SetToolTip(executeDeciderButton, "Tell the algorithm to make a decision whether to buy or sell based on the prediction size data currently in the chart.");
            toolTip.SetToolTip(runtimeDuration, "Set the amount of time to trade autonomously (min).");
            toolTip.SetToolTip(autonomousTradeButton, "Start the autonomous trading procedure.");
            toolTip.SetToolTip(neuralNetworkOperationsButton, "Open the form to perform advanced operations with the neural network.");
            toolTip.SetToolTip(resetSolutionButton, "Destroy the neural network and reset the solution.");
            refreshNotificationLabel.Text = @"";

        }
        public string AnalysisTrajectory { get; set; }
        public string NumberOfPoints { get; set; }
        public DateTime StartedOn;
        public TimeSpan Duration;
        public Timer AutolivePollingTimer;
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
        public int PredictionSize { get; set; } //7
        public double Epoch { get; private set; }
        public double LearningError { get; private set; }
        public double PredictionError { get; private set; }
        public double ForecastError { get; private set; }
        public int TrailingDigits { get; set; }
        public double[,] Solution { get; set; }
        public double[,] Forecast { get; set; }
        public double[] Data { get; set; }
        public double DataMagnitudeUpper { get; set; }
        public double DataMagnitudeLower { get; set; }
        public double[] PredictionPoints { get; set; }
        public ActivationNetwork Network { get; set; }
        public List<ActivationNetwork> Networks { get; set; }
        public BackPropagation LearningNetwork { get; set; }
        public List<BackPropagation> LearningNetworks { get; set; }
        public List<string> TrendFromDataStrings { get; set; }
        public List<string> TrendFromSolutionStrings { get; set; }
        public List<bool> TrendFromData { get; set; }
        public List<bool> TrendFromSolution { get; set; }
        public double TrainingRate { get; set; }
        public double ErrorTolerance { get; set; }
        public TransferFunction TransferFunction { get; set; }
        public string PathToXms
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory + @"\xms\", xmsFilepathBox.Text);
            }
        }
        public string PathToCsv
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory + @"\csv\", xmsFilepathBox.Text);
            }
        }
        public string PathToAgentTemplate
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, "agent_network.xml");
            }
        }
        private XmlDocument _document;
        public BackPropagationNetwork BackPropagationNetwork { get; set; }
        public BackPropagationNetwork ForecastNetwork { get; set; }
        public BackPropagationNetwork CustomNetwork { get; set; }
        public double BackPropagationError { get; set; }
        public double[][] Input { get; set; }
        public double[][] Desired { get; set; }
        private TransferFunction _transferFunction;
        public double NetworkCorrection { get; set; }
        public double[] NetworkInput { get; set; }
        public int HiddenLayer { get; set; }
        public int OutputLayer { get; set; }
        public int ForecastSize { get; set; }
        public int SolutionOffset { get; set; }
        public string BuyOrSellTrendSolution { get; set; }
        public string BuyOrSellTrendRawData { get; set; }
        /// <summary>
        /// Formats the instrument for the REST server.
        /// </summary>
        /// <returns>A formatted string.</returns>
        public string FormatInstrument()
        {
            var firstPair = currencyFromBox.SelectedItem.ToString();
            var secondPair = currencyToBox.SelectedItem.ToString();
            return firstPair + "_" + secondPair;
        }
        /// <summary>
        /// Loads the data from xms.
        /// </summary>
        public void LoadXmsData()
        {
            if (PathToXms == null)
                return;
            StartedOn = DateTime.Now;
            _document = new XmlDocument();
            if (xmsFilepathBox.Text == "")
            {
                MessageBox.Show("Select a document to load from the 'Save Data' pane.", "", MessageBoxButtons.OK);
                return;
            }
            _document.Load(PathToXms);
            // Load from xms.
            const string basePath = "RatesData/Data";
            if (XPathValue("RatesData/@Source") != "Oanda")
                return;
            int numberOfCandles;
            int.TryParse(XPathValue("RatesData/@NumberOfCandles"), out numberOfCandles);
            HistoricalStatistics.NumberOfCandles = numberOfCandles;
            HistoricalStatistics.CandleFormat = XPathValue("RatesData/@CandleFormat");
            int dailyAlignment;
            int.TryParse(XPathValue("RatesData/@DailyAlignment"), out dailyAlignment);
            HistoricalStatistics.DailyAlignment = dailyAlignment;
            HistoricalStatistics.Granularity = XPathValue("RatesData/@Granularity");
            HistoricalStatistics.MarketTimeData = new double[numberOfCandles];
            HistoricalStatistics.OpenData = new double[numberOfCandles];
            HistoricalStatistics.LowData = new double[numberOfCandles];
            HistoricalStatistics.HighData = new double[numberOfCandles];
            HistoricalStatistics.CloseData = new double[numberOfCandles];
            HistoricalStatistics.VolumeData = new int[numberOfCandles];

            for (int l = 0; l < numberOfCandles; l++)
            {
                double.TryParse(XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@MarketTime"), out HistoricalStatistics.MarketTimeData[l]);
                double.TryParse(XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@Open"), out HistoricalStatistics.OpenData[l]);
                double.TryParse(XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@Low"), out HistoricalStatistics.LowData[l]);
                double.TryParse(XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@High"), out HistoricalStatistics.HighData[l]);
                double.TryParse(XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@Close"), out HistoricalStatistics.CloseData[l]);
                int.TryParse(XPathValue(basePath + "[@Index='" + l.ToString(CultureInfo.InvariantCulture) + "']/@Volume"), out HistoricalStatistics.VolumeData[l]);
            }
            HistoricalStatistics.RatesLoaded = true;
            // Release.
            _document = null;
            // Process the data.
            // Allocate and set the data.
            _data = new double[numberOfCandles];
            _dataToShow = new double[numberOfCandles, 2];
            Array.Copy(HistoricalStatistics.HighData, 0, _data, 0, numberOfCandles);
            for (var j = 0; j < numberOfCandles; j++)
            {
                _dataToShow[j, 0] = j;
                _dataToShow[j, 1] = _data[j];
            }
            // Update list and chart.
            //UpdateDataListView();
            trajectoryChart.RangeX = new DoubleRange(0, _data.Length - 1 + ForecastSize);
            trajectoryChart.UpdateDataSeries("data", _dataToShow);
            trajectoryChart.UpdateDataSeries("solution", null);
            // Set the delimiters.
            UpdateDelimiters();
            // Record performance vectors for the operation.
            Duration = DateTime.Now - StartedOn;
            trajectoryProcessingTimeBox.Text = Duration.Seconds + @"." + Duration.Milliseconds.ToString(CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Loads the metric data.
        /// </summary>
        public void LoadMetricData(string numberOfPoints, string cpuPercentage, string memoryUsage, string memoryLimit, string memoryPercentage, string networkBandwidth)
        {
            StartedOn = DateTime.Now;
            var _numberOfPoints = int.Parse(numberOfPoints);
            var result = Statistics.ProcessMetrics(_numberOfPoints, cpuPercentage, memoryUsage, memoryLimit, memoryPercentage, networkBandwidth);
            //var result = true;
            if (result)
            {
                switch (AnalysisTrajectory)
                {
                    case "CpuPercentage":
                        {
                            // Allocate and set the data.
                            _data = new double[_numberOfPoints];
                            _dataToShow = new double[_numberOfPoints, 2];
                            Array.Copy(Statistics.CpuPercentageData, 0, _data, 0, _numberOfPoints);
                            for (var j = 0; j < _numberOfPoints; j++)
                            {
                                _dataToShow[j, 0] = j;
                                _dataToShow[j, 1] = _data[j];
                            }
                        }
                        break;
                    case "MemoryUsage":
                        {
                            // Allocate and set the data.
                            _data = new double[_numberOfPoints];
                            _dataToShow = new double[_numberOfPoints, 2];
                            Array.Copy(Statistics.MemoryUsageData, 0, _data, 0, _numberOfPoints);
                            for (var j = 0; j < _numberOfPoints; j++)
                            {
                                _dataToShow[j, 0] = j;
                                _dataToShow[j, 1] = _data[j];
                            }
                        }
                        break;
                    case "MemoryLimit":
                        {
                            // Allocate and set the data.
                            _data = new double[_numberOfPoints];
                            _dataToShow = new double[_numberOfPoints, 2];
                            Array.Copy(Statistics.MemoryLimitData, 0, _data, 0, _numberOfPoints);
                            for (var j = 0; j < _numberOfPoints; j++)
                            {
                                _dataToShow[j, 0] = j;
                                _dataToShow[j, 1] = _data[j];
                            }
                        }
                        break;
                    case "MemoryPercentage":
                        {
                            // Allocate and set the data.
                            _data = new double[_numberOfPoints];
                            _dataToShow = new double[_numberOfPoints, 2];
                            Array.Copy(Statistics.MemoryPercentageData, 0, _data, 0, _numberOfPoints);
                            for (var j = 0; j < _numberOfPoints; j++)
                            {
                                _dataToShow[j, 0] = j;
                                _dataToShow[j, 1] = _data[j];
                            }
                        }
                        break;
                    case "NetworkBandwidth":
                        {
                            // Allocate and set the data.
                            _data = new double[_numberOfPoints];
                            _dataToShow = new double[_numberOfPoints, 2];
                            Array.Copy(Statistics.NetworkBandwidthData, 0, _data, 0, _numberOfPoints);
                            for (var j = 0; j < _numberOfPoints; j++)
                            {
                                _dataToShow[j, 0] = j;
                                _dataToShow[j, 1] = _data[j];
                            }
                        }
                        break;
                }
                // Update list and chart.
                //UpdateDataListView();
                trajectoryChart.RangeX = new DoubleRange(0, _data.Length - 1 + ForecastSize);
                trajectoryChart.UpdateDataSeries("data", _dataToShow);
                trajectoryChart.UpdateDataSeries("solution", null);
                // Set the delimiters.
                UpdateDelimiters();
                // Record performance vectors for the form.
                Duration = DateTime.Now - StartedOn;
                trajectoryProcessingTimeBox.Text = Duration.Seconds + @"." + Duration.Milliseconds.ToString(CultureInfo.InvariantCulture);
            }
        }
        /// <summary>
        /// Prepares the solution.
        /// </summary>
        public void PrepareSolution()
        {
            if (_abortRequested)
                return;
            if (BackPropagationNetwork == null)
                return;
            StartedOn = DateTime.Now;
            // Initialize properties based on the current selection.
            UpdateDelimiters();
            Iterations = int.Parse(iterationsBox.Text);
            LearningRate = double.Parse(learningRateBox.Text);
            SigmoidAlpha = double.Parse(sigmoidAlphaBox.Text);
            Momentum = double.Parse(momentumBox.Text);
            _needToStop = false;
            _workerThread = new Thread(SearchSolution) { Name = "SearchSolution Thread" };
            //_workerThread = new Thread(SearchOldSolution) { Name = "SearchAlternateSolution Thread" };
            Cursor.Current = Cursors.WaitCursor;
            //SearchAlternateSolution(); // Stored in the boneyard. Spent a lot of time working it out, but have acquired the training methods and the xms parts. I will need to optimise training in the future.
            SearchSolution();
            Cursor.Current = Cursors.Default;
            //_workerThread.Start();
        }
        private void SearchSolution()
        {
            StartedOn = DateTime.Now;
            // Set the network properties.
            HiddenLayer = WindowSize * 2;
            OutputLayer = 1;
            // Implement data transformation factors for the chart.
            double factor = 1.7 / trajectoryChart.RangeY.Length; //0.00405499999999992
            double yMin = trajectoryChart.RangeY.Min; //0.852045
            // Prepare the network.
            var networkInput = new double[WindowSize]; //5
            // Loop.
            for (int i = 0; i < SampleSize; i++)
            {
                // Set the input.
                for (int j = 0; j < WindowSize; j++)
                {
                    Input[i][j] = (_data[i + j] - yMin) * factor - NetworkCorrection;
                }
                // Set the output.
                Desired[i][0] = (_data[i + WindowSize] - yMin) * factor - NetworkCorrection;
            }
            // Create a multi-layer neural network.
            Network = new ActivationNetwork(new BipolarSigmoidFunction(SigmoidAlpha), WindowSize, HiddenLayer, OutputLayer);
            NumberOfLayers = 2;
            LayerSize = new int[NumberOfLayers];
            LayerSize[0] = HiddenLayer;
            LayerSize[1] = OutputLayer;
            // Create a teacher. Set the learning rate and momentum.
            LearningNetwork = new BackPropagation(Network) { LearningRate = LearningRate, Momentum = Momentum };
            // This is here but have no historical trace.
            Network.Compute(networkInput); // This is an array of zeros. Can we feed the algorithm's correction here?
            LearningError += Math.Abs(_data[1] - ((Network.Compute(networkInput)[0] + NetworkCorrection) / factor + yMin));
            // Iterations.
            var iteration = 1;
            // Solution array.
            var solutionSize = _data.Length - WindowSize;
            var solution = new double[solutionSize, 2];
            // Calculate x-values to be used with solution function.
            for (int j = 0; j < solutionSize; j++)
            {
                solution[j, 0] = j + WindowSize;
            }
            // Loop.
            while (!_needToStop)
            {
                // Run epoch of learning procedure.
                Epoch = LearningNetwork.RunEpoch(Input, Desired) / SampleSize;
                // Calculate solution, learning, and prediction errors by iterating the data.
                for (int i = 0, n = _data.Length - WindowSize; i < n; i++)
                {
                    // Assign values from current window as network's input.
                    for (var j = 0; j < WindowSize; j++)
                    {
                        networkInput[j] = (_data[i + j] - yMin) * factor - NetworkCorrection;
                    }
                    // Evaluate the function.
                    solution[i, 1] = (Network.Compute(networkInput)[0] + NetworkCorrection) / factor + yMin;
                    // Truncate to a pre-chosen set of decimal places.
                    solution[i, 1] = Math.Round(solution[i, 1], TrailingDigits);
                    // Compute the prediction and learning error.
                    if (i >= n - PredictionSize)
                    {
                        PredictionError += Math.Abs(solution[i, 1] - _data[WindowSize + i]);
                    }
                    else
                    {
                        LearningError += Math.Abs(solution[i, 1] - _data[WindowSize + i]);
                    }
                }
                // Update the solution on the chart object (off-thread).
                trajectoryChart.UpdateDataSeries("solution", solution);
                Solution = solution;
                Data = _data;
                // Set the dataset maximum and minimum values.
                DataMagnitudeUpper = _data.Max();
                DataMagnitudeLower = _data.Min();
                // Increase the current iteration.
                iteration++;
                // Check if we need to stop.
                if ((Iterations != 0) && (iteration > Iterations))
                {
                    _workerThread.Abort();
                    break;
                }
            }
            // Report the behavioural attributes.
            SetIteration(iteration);
            SetEpoch(Epoch);
            SetLearningError(LearningError);
            SetPredictionError(PredictionError);
            // Record performance vectors for the operation and logfile.
            Duration = DateTime.Now - StartedOn;
            SetAlgorithmPerformance(Duration);
            Logging.WriteLog("Neural network processed its given task in " + Duration.Seconds + @"." + Duration.Milliseconds + " seconds", Logging.LogType.Statistics, Logging.LogCaller.AgentGui);
            Logging.WriteLog("Program metrics: At an epoch of " + Epoch + ", " + Iterations + " iterations, " + LearningRate + " learning rate, " + PredictionSize + " prediction size; a sigmoid alpha value of " + SigmoidAlpha + " and a momentum of " + Momentum + " resulting in a learning error of " + LearningError + " and a prediction error of " + PredictionError + " captured by the " + Logging.LogCaller.AlgorithmicComputation + " form", Logging.LogType.Statistics, Logging.LogCaller.AlgorithmicComputation);
        }
        // Chart-related operations.
        private void UpdateDelimiters()
        {
            if (_data == null) return;
            // The window delimiter.
            _windowDelimiter[0, 0] = _windowDelimiter[1, 0] = WindowSize;
            _windowDelimiter[0, 1] = trajectoryChart.RangeY.Min;
            _windowDelimiter[1, 1] = trajectoryChart.RangeY.Max;
            trajectoryChart.UpdateDataSeries("window", _windowDelimiter);
            // The prediction delimiter.
            _predictionDelimiter[0, 0] = _predictionDelimiter[1, 0] = _data.Length - 1 - PredictionSize;
            _predictionDelimiter[0, 1] = trajectoryChart.RangeY.Min;
            _predictionDelimiter[1, 1] = trajectoryChart.RangeY.Max;
            trajectoryChart.UpdateDataSeries("prediction", _predictionDelimiter);
            // The future delimiter.
            _futureDelimiter[0, 0] = _futureDelimiter[1, 0] = _data.Length - 1;
            _futureDelimiter[0, 1] = trajectoryChart.RangeY.Min;
            _futureDelimiter[1, 1] = trajectoryChart.RangeY.Max;
            trajectoryChart.UpdateDataSeries("future", _futureDelimiter);
        }
        //private void UpdateDataListView()
        //{
        //    // remove all current records
        //    _dataListView.Items.Clear();
        //    // add new records
        //    for (int i = 0, n = _data.GetLength(0); i < n; i++)
        //    {
        //        _dataListView.Items.Add(new ListViewItem(_data[i].ToString(CultureInfo.InvariantCulture)));
        //    }
        //}
        // Will assume the training is sufficient at the present time.
        public void TrainNetwork()
        {
            StartedOn = DateTime.Now;
            Cursor.Current = Cursors.WaitCursor;
            // Parse the number of iterations.
            Iterations = int.Parse(iterationsBox.Text);
            // Create the network if it does not already exist.
            if (BackPropagationNetwork == null)
            {
                var layerSizes = new[] { WindowSize, WindowSize * 2, 1 };
                var tFuncs = new[] { TransferFunction.None, TransferFunction, TransferFunction.Linear };
                BackPropagationNetwork = new BackPropagationNetwork(layerSizes, tFuncs);
            }
            //if (BackPropagationNetwork != null)
            //{
            //    BackPropagationError = 0;
            //    SetBackPropagationError(BackPropagationError);
            //}
            // Prepare the number of learning samples.
            SampleSize = _data.Length - PredictionSize - WindowSize;
            Input = new double[SampleSize][];
            Desired = new double[SampleSize][];
            // Loop.
            for (int i = 0; i < SampleSize; i++)
            {
                Input[i] = new double[WindowSize];
                Desired[i] = new double[1];
                // Set the input.
                for (int j = 0; j < WindowSize; j++)
                {
                    Input[i][j] = _data[i + j];
                }
                // Set the desired values.
                Desired[i][0] = _data[i + WindowSize];
            }
            // Train the network.
            int count = 0;
            do
            {
                // Prepare for training epoch.
                count++;
                // Train the network.
                for (int i = 0; i < Input.Length; i++)
                {
                    BackPropagationError += BackPropagationNetwork.Train(ref Input[i], ref Desired[i], TrainingRate, Momentum);
                }
                SetBackPropagationError(BackPropagationError);
                //backpropagationErrorBox.Text = BackPropagationError.ToString(CultureInfo.InvariantCulture);
                //for (int i = 0; i < WindowSize; i++)
                //    BackPropagationError += BackPropagationNetwork.Train(ref Input[i], ref Desired[i], TrainingRate, Momentum);
                //networkErrorBox.Text = BackPropagationError.ToString(CultureInfo.InvariantCulture);

            } while (BackPropagationError > ErrorTolerance && count <= TrainingCycles);

            BackPropagationNetwork.SaveNetworkXml(Environment.CurrentDirectory + @"\files\" + "GuiAgentNetwork.xml");
            Cursor.Current = Cursors.Default;
            // Record performance vectors for the operation.
            Duration = DateTime.Now - StartedOn;
            //trajectoryProcessingTimeBox.Text = Duration.Seconds + @"." + Duration.Milliseconds.ToString(CultureInfo.InvariantCulture);
            // DELEGATE ME!
        }
        public void TrainForecastNetwork(string network)
        {
            StartedOn = DateTime.Now;
            Cursor.Current = Cursors.WaitCursor;
            // Check for the forecast-flavor network to train. Create the network if it does not already exist.
            network = network.ToLower().Trim();
            switch (network)
            {
                case "custom":
                    {
                        if (CustomNetwork == null)
                        {
                            var layerSizes = new[] { WindowSize, WindowSize * 2, 1 };
                            var tFuncs = new[] { TransferFunction.None, TransferFunction, TransferFunction.Linear };
                            CustomNetwork = new BackPropagationNetwork(layerSizes, tFuncs);
                        }
                    }
                    break;
                case "forecast":
                    {
                        if (ForecastNetwork == null)
                        {
                            var layerSizes = new[] { WindowSize, WindowSize * 2, 1 };
                            var functions = new[] { TransferFunction.None, TransferFunction, TransferFunction.Linear };
                            ForecastNetwork = new BackPropagationNetwork(layerSizes, functions) { Name = "Forecast network" };
                        }
                    }
                    break;
            }
            // Prepare the number of learning samples.
            SampleSize = _data.Length - PredictionSize - WindowSize;
            Input = new double[SampleSize][];
            Desired = new double[SampleSize][];
            // Loop.
            for (int i = 0; i < SampleSize; i++)
            {
                Input[i] = new double[WindowSize];
                Desired[i] = new double[1];
                // Set the input.
                for (int j = 0; j < WindowSize; j++)
                {
                    Input[i][j] = _data[i + j];
                }
                // Set the desired values.
                Desired[i][0] = _data[i + WindowSize];
            }
            // Train the network.
            int count = 0;
            do
            {
                switch (network)
                {
                    case "custom":
                        {
                            // Prepare for training epoch.
                            count++;
                            // Train the network.
                            for (int i = 0; i < Input.Length; i++)
                            {
                                ForecastError += CustomNetwork.Train(ref Input[i], ref Desired[i], TrainingRate, Momentum);
                            }
                            SetBackPropagationError(BackPropagationError);
                        }
                        break;
                    case "forecast":
                        {
                            // Prepare for training epoch.
                            count++;
                            // Train the network.
                            for (int i = 0; i < Input.Length; i++)
                            {
                                ForecastError += ForecastNetwork.Train(ref Input[i], ref Desired[i], TrainingRate, Momentum);
                            }
                            SetBackPropagationError(BackPropagationError);
                        }
                        break;
                }

            } while (BackPropagationError > ErrorTolerance && count <= TrainingCycles);

            switch (network)
            {
                case "custom":
                    {
                        CustomNetwork.SaveNetworkXml(Environment.CurrentDirectory + @"\files\" + "GuiCustomNetwork.xml");
                    }
                    break;
                case "forecast":
                    {
                        ForecastNetwork.SaveNetworkXml(Environment.CurrentDirectory + @"\files\" + "GuiForecastNetwork.xml");
                    }
                    break;
            }

            Cursor.Current = Cursors.Default;
        }
        // Finished up, to be completed below.
        // 1. Implement softmax, DONE.
        // 2. Finish methods, DONE.
        // 3. The decision a preference elicitation for a trade, DONE with truth table and frequency of sequence.
        // 4. Automat. DONE.
        // TODO: Incorporate dataset magnitudes, e.g., the maximum and minimum value of a dataset. DONE
        // This is what is being used now.
        private void SearchForecastCombinedWithSolution()
        {
            StartedOn = DateTime.Now;
            // Iterations.
            //var iteration = 1;
            // Implement data transformation factors for the chart.
            double factor = 1.7 / trajectoryChart.RangeY.Length;
            double yMin = trajectoryChart.RangeY.Min;
            // Prepare the network.
            NetworkInput = new double[ForecastSize];
            // Forecast array.
            var forecastSize = ForecastSize;
            var forecastSolution = new double[forecastSize, 2];
            // Calculate x-values to be used with forecast function.
            for (int j = 0; j < forecastSize; j++)
            {
                forecastSolution[j, 0] = j;
            }
            if (Solution == null)
                return;
            // Take the last elements from the solution of a size matched by the future size.
            var forecastData = Solution.GetLastValuesOf(ForecastSize);
            // Calculate forecast assigning values from current forcast size as network's input.
            for (int i = 0; i < ForecastSize; i++)
            {
                NetworkInput[i] = (forecastData[i] - yMin) * factor - NetworkCorrection;
                // Evaluate the function.
                forecastSolution[i, 1] = (Network.Compute(NetworkInput)[0] + NetworkCorrection) / factor + yMin;
                ForecastError += Math.Abs(forecastSolution[i, 1] - forecastData[i]);
            }
            // Combine the forcast with the solution data and replot on the chart.
            var forecast = Solution.CombineArray(forecastSolution);
            trajectoryChart.UpdateDataSeries("forecastFromSolution", forecast);
            Forecast = forecast;
            // Record performance vectors for the operation.
            SetForecastError(ForecastError);
            Duration = DateTime.Now - StartedOn;
            SetAlgorithmPerformance(Duration);
        }
        // Has softmax implementation. Unfinished.
        private void SearchForecastUsingCustomPropagationNetwork()
        {
            Cursor.Current = Cursors.WaitCursor;
            // Want to rewrite this as per the design which needs to be finished.
            StartedOn = DateTime.Now;
            // Forecast array.
            var forecastSize = ForecastSize;
            var forecastSolution = new double[forecastSize, 2];
            // Parse the number of iterations.
            Iterations = int.Parse(iterationsBox.Text);
            // Create the network if it does not already exist.

            // Calculate x-values to be used with forecast function.
            for (int j = 0; j < forecastSize; j++)
            {
                forecastSolution[j, 0] = j;
            }
            TrainForecastNetwork("custom");
            var output = new double[1];
            var networkInput = NetworkInput;
            var networkOutput = new List<double[]>();
            for (int i = 0; i < ForecastSize; i++)
            {
                CustomNetwork.RunEvaluation(ref networkInput, out output);
                networkOutput.Add(output);
            }
            // Compute the softmax.
            var softmaxFirst = CustomNetwork.ComputeSoftmax(Boagaphish.Core.Networks.Network.CrossLayer.InputHidden, NetworkInput);
            var softmaxSecond = CustomNetwork.ComputeSoftmax(Boagaphish.Core.Networks.Network.CrossLayer.HiddenOutput, NetworkInput);
            // This is where the design stops. Continue on.
            var hold = 0;

        }
        // Done.
        public void TradeAutonomously()
        {
            // Assumption #1: When the Epoch is less than 0.0050, the prediction is fairly accurate on the first iteration of a solution.
            autonomyStatusLabel.Text = @"Working...";
            Refresh();
            // Preparation: Create the objects for the autonomous session.
            TrendFromDataStrings = new List<string>();
            TrendFromSolutionStrings = new List<string>();
            // Step 1: Check the automat data is loaded. Run the solution.
            if (HistoricalStatistics.RatesLoaded | Statistics.MetricLoaded) // Check against the rates, be them live or historical.
            {
                if (Solution == null)
                {
                    TrainNetwork();
                    PrepareSolution();
                }
                // Step 2: Analyze the points within the prediction size window and check for an upward or downward trend in the given data point series length.
                // a: Get the values from the prediction series.
                var predictionValuesSolution = Solution.GetLastValuesOf(PredictionSize);
                var predictionValuesRawData = Data.GetPredictionWindowValues(PredictionSize);
                // b: Compile a list of the trend: true if value greater than previous, false if not.
                var trendSolution = predictionValuesSolution.IsGreaterThanPrevious(PredictionSize);
                var trendRawData = predictionValuesRawData.IsGreaterThanPrevious(PredictionSize);
                // c: Send the trend to the decision engine.
                BuyOrSellTrendSolution = Decision.MakeDecision(trendSolution, Decision.DataStreamType.Solution); // Decision based on the solution.
                BuyOrSellTrendRawData = Decision.MakeDecision(trendRawData, Decision.DataStreamType.Raw);// Decision based on raw data.
                // d: Trim the magnitudes of the dataset.
                DataMagnitudeUpper = Math.Round(DataMagnitudeUpper, 5);
                DataMagnitudeLower = Math.Round(DataMagnitudeLower, 5);
            }
            else
            {
                MessageBox.Show(@"Candle data has not been loaded, therefore no solution is to be found.", @"Empty solution", MessageBoxButtons.OK,
                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            // Step 3: Decide whether or not to buy or sell the instrument position.
            if (Decision.Buy)
            {
                //tradeIndicator.Image = Properties.Resources.red_indicator;
                notificationBox.Text = @"S-trend says: " + BuyOrSellTrendSolution + @". " + Environment.NewLine + @"RD-trend says " + BuyOrSellTrendRawData + @".";
                trendIndicationLabel.Text = (DataMagnitudeUpper - DataMagnitudeLower).ToString(CultureInfo.InvariantCulture);
                Refresh();
                Decision.Buy = false;
            }
            if (Decision.Sell)
            {
                //tradeIndicator.Image = Properties.Resources.green_indicator;
                notificationBox.Text = @"S-trend says: " + BuyOrSellTrendSolution + @". " + Environment.NewLine + @"RD-trend says " + BuyOrSellTrendRawData + @".";
                trendIndicationLabel.Text = (DataMagnitudeUpper - DataMagnitudeLower).ToString(CultureInfo.InvariantCulture);
                Refresh();
                Decision.Sell = false;
            }
            // Step 4: Perform post-processing to see how effective the trade was in terms of (+) or (-) magnitudes.
            // DataMagnitudeUpper and DataMagnitudeLower are serving this function now.
            autonomyStatusLabel.Text = @"Magnitude:";
            Refresh();
        }

        #region Multi-Threading
        private void StartTraining(object state)
        {
            try
            {
                TrainNetwork();
                TrainForecastNetwork("Forecast");
            }
            finally
            {
                lock (_lock)
                {
                    _threadRunning = false;
                }
            }
        }
        private void StartSolution(object state)
        {
            try
            {
                PrepareSolution();
            }
            finally
            {
                lock (_lock)
                {
                    _threadRunning = false;
                }
            }
        }
        private void TradeAutonomously(object state)
        {
            try
            {
                TradeAutonomously();
            }
            finally
            {
                lock (_lock)
                {
                    _threadRunning = false;
                }
            }
        }
        private void StartForecast(object state)
        {
            try
            {
                SearchForecastCombinedWithSolution();
                SearchForecastUsingCustomPropagationNetwork();
            }
            finally
            {
                lock (_lock)
                {
                    _threadRunning = false;
                }
            }
        }
        private void SetForecastError(double forecastError)
        {
            if (forecastErrorBox.InvokeRequired)
            {
                var d = new SetForecastCallback(SetForecastError);
                Invoke(d, new object[] { forecastError });
                ForecastError = forecastError;
            }
            else
            {
                forecastErrorBox.Text = ForecastError.ToString(CultureInfo.InvariantCulture);
            }
        }
        private void SetBackPropagationError(double backPropagationError)
        {
            if (backpropagationErrorBox.InvokeRequired)
            {
                var d = new SetTrainingCallback(SetBackPropagationError);
                Invoke(d, new object[] { backPropagationError });
                BackPropagationError = backPropagationError;
            }
            else
            {
                backpropagationErrorBox.Text = BackPropagationError.ToString(CultureInfo.InvariantCulture);
            }
        }
        private void SetEpoch(double epoch)
        {
            if (epochBox.InvokeRequired)
            {
                var d = new SetEpochCallback(SetEpoch);
                Invoke(d, new object[] { epoch });
                Epoch = epoch;
            }
            else
            {
                epochBox.Text = Epoch.ToString(CultureInfo.InvariantCulture);
            }
        }
        private void SetLearningError(double learningError)
        {
            if (learningErrorBox.InvokeRequired)
            {
                var d = new SetLearningErrorCallback(SetLearningError);
                Invoke(d, new object[] { learningError });
                LearningError = learningError;
            }
            else
            {
                learningErrorBox.Text = LearningError.ToString(CultureInfo.InvariantCulture);
            }
        }
        private void SetPredictionError(double predictionError)
        {
            if (predictionErrorBox.InvokeRequired)
            {
                var d = new SetPredictionErrorCallback(SetPredictionError);
                Invoke(d, new object[] { predictionError });
                PredictionError = predictionError;
            }
            else
            {
                predictionErrorBox.Text = PredictionError.ToString(CultureInfo.InvariantCulture);
            }
        }
        private void SetAlgorithmPerformance(TimeSpan duration)
        {
            if (algorithmProcessingTimeBox.InvokeRequired)
            {
                var d = new SetAlgorithmPerformanceCallback(SetAlgorithmPerformance);
                Invoke(d, new object[] { duration });
                Duration = duration;
            }
            else
            {
                algorithmProcessingTimeBox.Text = Duration.Seconds + @"." + Duration.Milliseconds.ToString(CultureInfo.InvariantCulture);
            }
        }
        private void SetIteration(int iteration)
        {
            if (notificationBox.InvokeRequired)
            {
                var d = new SetIterationCallback(SetIteration);
                Invoke(d, new object[] { iteration });
            }
            else
            {
                notificationBox.Text = iteration.ToString(CultureInfo.InvariantCulture);
            }
        }
        private void StartThread(string methodToCall)
        {
            if (_workerThread == null)
            {
                _stopThread = false;
                //_workerThread = new Thread(new ThreadStart(MethodToCall));// Sort the appropriate method to call.
                if (_workerThread != null) _workerThread.Start();
            }
        }
        private void StopThread()
        {
            if (_workerThread != null)
            {
                _stopThread = true;
                _workerThread.Join();
                _workerThread = null;
            }
        }
        #endregion

        #region X stuff
        /// <summary>
        /// Saves the data into xms format.
        /// </summary>
        public void SaveToXms()
        {
            if (PathToXms == null)
                return;
            var writer = XmlWriter.Create(PathToXms);
            // Begin document.
            writer.WriteStartElement("RatesData");
            writer.WriteAttributeString("Source", "Oanda");
            writer.WriteAttributeString("NumberOfCandles", LiveRates.NumberOfCandles.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("CandleFormat", LiveRates.CandleFormat);
            writer.WriteAttributeString("DailyAlignment", LiveRates.DailyAlignment.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("Granularity", LiveRates.Granularity);
            // Data element.
            for (int i = 0; i < LiveRates.NumberOfCandles; i++)
            {
                writer.WriteStartElement("Data");
                writer.WriteAttributeString("Index", i.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("MarketTime", LiveRates.MarketTimeData[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Open", LiveRates.OpenData[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Low", LiveRates.LowData[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("High", LiveRates.HighData[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Close", LiveRates.CloseData[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Volume", LiveRates.VolumeData[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteEndElement();// Data
            }
            writer.WriteEndElement();//RatesData
            writer.Flush();
            writer.Close();
        }
        /// <summary>
        /// Saves the data into csv format. Can split files for use in the predictor.
        /// </summary>
        public void SaveToCsv(bool separateFiles = false)
        {
            if (separateFiles)
            {
                var files = new[] {"open", "high", "low", "close", "volume"};
                foreach (var file in files)
                {
                    var writer = new StreamWriter(Path.Combine(Environment.CurrentDirectory + @"\csv\", file + ".csv"));
                    var builder = new StringBuilder();
                    builder.Append("date, " + file);
                    writer.WriteLine(builder.ToString());
                    builder.Clear();
                    for (var i = 0; i < LiveRates.NumberOfCandles; i++)
                    {
                        builder.Append(LiveRates.MarketXTimeDate[i].ToString("yyyy-MM-dd HH:mm:ss"));
                        builder.Append(Separator);
                        switch (file)
                        {
                            case "open":
                                builder.Append(LiveRates.OpenData[i].ToString(CultureInfo.InvariantCulture));
                                break;
                            case "high":
                                builder.Append(LiveRates.HighData[i].ToString(CultureInfo.InvariantCulture));
                                break;
                            case "low":
                                builder.Append(LiveRates.LowData[i].ToString(CultureInfo.InvariantCulture));
                                break;
                            case "close":
                                builder.Append(LiveRates.CloseData[i].ToString(CultureInfo.InvariantCulture));
                                break;
                            case "volume":
                                builder.Append(LiveRates.VolumeData[i].ToString(CultureInfo.InvariantCulture));
                                break;
                        }
                        writer.WriteLine(builder.ToString());
                        builder.Clear();
                    }
                    writer.Flush();
                    writer.Close();
                }
            }
            else if (PathToCsv != null)
            {
                var writer = new StreamWriter(PathToCsv);
                var builder = new StringBuilder();
                builder.Append("Date,Open,High,Low,Close,Volume,Adj Close");
                writer.WriteLine(builder.ToString());
                builder.Clear();
                for (int i = 0; i < LiveRates.NumberOfCandles; i++)
                {
                    builder.Append(LiveRates.MarketXTimeDate[i].ToString("yyyy-MM-dd HH:mm:ss"));
                    builder.Append(Separator);
                    builder.Append(LiveRates.OpenData[i].ToString(CultureInfo.InvariantCulture));
                    builder.Append(Separator);
                    builder.Append(LiveRates.LowData[i].ToString(CultureInfo.InvariantCulture));
                    builder.Append(Separator);
                    builder.Append(LiveRates.HighData[i].ToString(CultureInfo.InvariantCulture));
                    builder.Append(Separator);
                    builder.Append(LiveRates.CloseData[i].ToString(CultureInfo.InvariantCulture));
                    builder.Append(Separator);
                    builder.Append(LiveRates.VolumeData[i].ToString(CultureInfo.InvariantCulture));
                    builder.Append(Separator);
                    builder.Append(LiveRates.CloseData[i].ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine(builder.ToString());
                    builder.Clear();
                }
                writer.Flush();
                writer.Close();
            }
            
        }
        /// <summary>
        /// Saves the agent template.
        /// </summary>
        public void SaveAgentTemplate()
        {
            if (PathToAgentTemplate == null)
                return;
            var writer = XmlWriter.Create(PathToAgentTemplate);
            // Begin document
            writer.WriteStartElement("NeuralNetwork");
            writer.WriteAttributeString("Type", "BackPropagation");
            // Parameters element
            writer.WriteStartElement("Parameters");
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("WindowSize", WindowSize.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("layerCount", NumberOfLayers.ToString(CultureInfo.InvariantCulture));
            // Layer sizes
            writer.WriteStartElement("Layers");
            for (int l = 0; l < NumberOfLayers; l++)
            {
                writer.WriteStartElement("Layer");
                writer.WriteAttributeString("Index", l.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Size", LayerSize[l].ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Type", TransferFunction.ToString());

                writer.WriteEndElement();// Layer
            }

            writer.WriteEndElement();//Layers
            writer.WriteEndElement();//Parameters
            // Weights and biases
            writer.WriteStartElement("Weights");
            for (int l = 0; l < NumberOfLayers; l++)
            {
                writer.WriteStartElement("Layer");
                writer.WriteAttributeString("Index", l.ToString(CultureInfo.InvariantCulture));
                for (int j = 0; j < LayerSize[l]; j++)
                {
                    writer.WriteStartElement("Node");
                    writer.WriteAttributeString("Index", j.ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("Bias", Bias[l][j].ToString(CultureInfo.InvariantCulture));
                    for (int i = 0; i < (l == 0 ? WindowSize : LayerSize[l - 1]); i++)
                    {
                        writer.WriteStartElement("Axion");
                        writer.WriteAttributeString("Index", i.ToString(CultureInfo.InvariantCulture));
                        writer.WriteString(Weight[l][i][j].ToString(CultureInfo.InvariantCulture));
                        writer.WriteEndElement();// Axion
                    }
                    writer.WriteEndElement();// Node
                }
                writer.WriteEndElement();// Layer
            }
            writer.WriteEndElement();// Weights
            writer.WriteEndElement();//NeuralNetwork
            writer.Flush();
            writer.Close();
        }
        /// <summary>
        /// Returns the Xpath value.
        /// </summary>
        /// <param name="xPath">The X path.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Cannot find specified node</exception>
        private string XPathValue(string xPath)
        {
            var node = _document.SelectSingleNode((xPath));
            if (node == null)
                MessageBox.Show(@"Cannot find specified node.", @"Xms error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            if (node != null)
                return node.InnerText;
            return "";
        }
        #endregion

        #region Events
        // Click.
        private void autonomousTradeButton_Click(object sender, EventArgs e)
        {
            autonomyStatusLabel.Text = "";
            notificationBox.Text = "";
            Refresh();
            // Step 6: Automat processing.
            if (LiveRates.RatesLoaded | HistoricalRates.RatesLoaded)
                //lock (_lock)
                //{
                //    if (_threadRunning)
                //    {
                //        _abortRequested = true;
                //    }
                //    else
                //    {
                //        _abortRequested = false;
                //        _threadRunning = true;
                //        ThreadPool.QueueUserWorkItem(TradeAutonomously);
                //    }
                //}
                TradeAutonomously();

            else
            {
                MessageBox.Show(@"Candle data has not been loaded from a live source nor from xms, therefore no trade information can be detected.", @"Empty data sent to the ai", MessageBoxButtons.OK,
                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }
        private void candlesViewerXmsButton_Click(object sender, EventArgs e)
        {
            if (HistoricalRates.RatesLoaded)
            {
                if (RatesViewer.Instance == false)
                {
                    var form = new RatesViewer(this, currencyFromBox.SelectedItem.ToString(), currencyToBox.SelectedItem.ToString(), "Xms candles");
                    form.Show(this);
                    RatesViewer.Instance = true;
                }
                else if (RatesViewer.Instance)
                {
                    // Do nothing.
                }
            }
            else
            {
                MessageBox.Show(@"Candle data has not been loaded from xms.", @"Xms candle data loading error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }

        }
        private void candlesViewerLiveButton_Click(object sender, EventArgs e)
        {
            if (LiveRates.RatesLoaded)
            {
                if (RatesViewer.Instance == false)
                {
                    var form = new RatesViewer(this, currencyFromBox.SelectedItem.ToString(), currencyToBox.SelectedItem.ToString(), "Live candles");
                    form.Show(this);
                    RatesViewer.Instance = true;
                }
                else if (RatesViewer.Instance)
                {
                    // Do nothing.
                }
            }
            else
            {
                MessageBox.Show(@"Candle data has not been loaded from live source.", @"Candle data loading error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }
        private void getLiveRatesButton_Click(object sender, EventArgs e)
        {
            //refreshNotificationLabel.Text = "";
            // Parse for network training.
            Enum.TryParse(transferFunctionSelectionBox.SelectedItem.ToString(), out _transferFunction);
            TransferFunction = _transferFunction;
            Cursor.Current = Cursors.WaitCursor;
            // Step 1: Load automat data. Intialize the values for the algorithm first.
            AnalysisTrajectory = analysisTrajectoryBox.SelectedItem.ToString();
            NumberOfPoints = numberOfCandlesBox.Text;
            //LoadLiveData();
            LoadMetricData();
            Cursor.Current = Cursors.Default;
        }
        private void getXmsRatesButton_Click(object sender, EventArgs e)
        {
            // Load the data from xms.
            //refreshNotificationLabel.Text = "";
            // Parse for network training.
            Enum.TryParse(transferFunctionSelectionBox.SelectedItem.ToString(), out _transferFunction);
            TransferFunction = _transferFunction;
            Cursor.Current = Cursors.WaitCursor;
            // Step 1: Load automat data. Intialize the values for the algorithm first.
            AnalysisTrajectory = analysisTrajectoryBox.SelectedItem.ToString();
            NumberOfPoints = numberOfCandlesBox.Text;
            LoadXmsData();
            Cursor.Current = Cursors.Default;
        }
        private void setSolutionButton_Click(object sender, EventArgs e)
        {
            refreshNotificationLabel.Text = "";
            // Step 2: Algorithmic processing.
            if (LiveRates.RatesLoaded | HistoricalRates.RatesLoaded)
                lock (_lock)
                {
                    if (_threadRunning)
                    {
                        _abortRequested = true;
                    }
                    else
                    {
                        _abortRequested = false;
                        _threadRunning = true;
                        ThreadPool.QueueUserWorkItem(StartSolution);
                    }
                }

            else
            {
                MessageBox.Show(@"Candle data has not been loaded from a live source nor from xms, therefore no solution is to be found.", @"Empty solution", MessageBoxButtons.OK,
                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }
        private void forecastTrendButton_Click(object sender, EventArgs e)
        {
            // Step 5: Predict the future trend from algorithmic activity.
            if (LiveRates.RatesLoaded | HistoricalRates.RatesLoaded)
                lock (_lock)
                {
                    if (_threadRunning)
                    {
                        _abortRequested = true;
                    }
                    else
                    {
                        _abortRequested = false;
                        _threadRunning = true;
                        ThreadPool.QueueUserWorkItem(StartForecast);
                    }
                }
            else
            {
                MessageBox.Show(@"Cannot predict the future without knowing the past, therefore no future trend is to be found.", @"Empty prediction", MessageBoxButtons.OK,
                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }
        private void refreshChartButton_Click(object sender, EventArgs e)
        {
            refreshNotificationLabel.Text = "";
            PredictionSize = int.Parse(predictionSizeBox.Text);
            WindowSize = int.Parse(windowSizeBox.Text);
            ForecastSize = int.Parse(futureSizeBox.Text);
            //PopulateListBox(xmsFilepathBox, PathToXms, "*.xml");
            UpdateDelimiters();
        }
        private void tradesManagerButton_Click(object sender, EventArgs e)
        {
            if (TradesManager.Instance == false)
            {
                var form = new TradesManager(this);
                form.Show(this);
                TradesManager.Instance = true;
            }
            else if (TradesManager.Instance)
            {
                // Do nothing.
            }
        }
        private void accountsManagerButton_Click(object sender, EventArgs e)
        {
            if (AccountsManager.Instance == false)
            {
                var form = new AccountsManager(this);
                form.Show(this);
                AccountsManager.Instance = true;
            }
            else if (AccountsManager.Instance)
            {
                // Do nothing.
            }
        }
        private void ordersManagerButton_Click(object sender, EventArgs e)
        {
            if (OrdersManager.Instance == false)
            {
                var form = new OrdersManager(this);
                form.Show(this);
                OrdersManager.Instance = true;
            }
            else if (OrdersManager.Instance)
            {
                // Do nothing.
            }
        }
        private void positionsManagerButton_Click(object sender, EventArgs e)
        {
            if (PositionsManager.Instance == false)
            {
                var form = new PositionsManager(this);
                form.Show(this);
                PositionsManager.Instance = true;
            }
            else if (PositionsManager.Instance)
            {
                // Do nothing.
            }
        }
        private void transactionHistoryButton_Click(object sender, EventArgs e)
        {
            if (TransactionsManager.Instance == false)
            {
                var form = new TransactionsManager(this);
                form.Show(this);
                TransactionsManager.Instance = true;
            }
            else if (TransactionsManager.Instance)
            {
                // Do nothing.
            }
        }
        private void executeDeciderButton_Click(object sender, EventArgs e)
        {
            // What to do when a decision is rendered.

        }
        private void okButton_Click(object sender, EventArgs e)
        {
            refreshNotificationLabel.Text = "";
            notificationBox.Text = "";
            backpropagationErrorBox.Text = "";
            learningErrorBox.Text = "";
            predictionErrorBox.Text = "";
            epochBox.Text = "";
            forecastErrorBox.Text = "";

        }
        private void neuralNetworkOperationsButton_Click(object sender, EventArgs e)
        {
            if (NeuralNetworkOperations.Instance == false)
            {
                var form = new NeuralNetworkOperations(this);
                form.Show(this);
                NeuralNetworkOperations.Instance = true;
            }
            else if (NeuralNetworkOperations.Instance)
            {
                // Do nothing.
            }
        }
        private void saveXmsDataButton_Click(object sender, EventArgs e)
        {
            SaveToXms();
        }
        private void saveCsvDataButton_Click(object sender, EventArgs e)
        {
            SaveToCsv(true);
        }
        private void trainNetworkButton_Click(object sender, EventArgs e)
        {
            lock (_lock)
            {
                if (_threadRunning)
                {
                    _abortRequested = true;
                }
                else
                {
                    _abortRequested = false;
                    _threadRunning = true;
                    ThreadPool.QueueUserWorkItem(StartTraining);
                }
            }

        }
        private void resetSolutionButton_Click(object sender, EventArgs e)
        {
            BackPropagationNetwork = null;
        }
        private void placeOrderButton_Click(object sender, EventArgs e)
        {
            if (NewOrder.Instance == false)
            {
                var form = new NewOrder(this);
                form.Show(this);
                NewOrder.Instance = true;
            }
            else if (NewOrder.Instance)
            {
                // Do nothing.
            }
        }
        // Change.
        private void predictionSizeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //UpdateDelimiters();
            refreshNotificationLabel.Text = @"Refresh the chart!";
        }
        private void windowSizeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //UpdateDelimiters();
            refreshNotificationLabel.Text = @"Refresh the chart!";
        }
        private void currencyFromBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Rates.HoldingCurrency = currencyFromBox.SelectedItem.ToString();
        }
        private void currencyToBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Rates.TradingCurrency = currencyToBox.SelectedItem.ToString();
        }
        private void sessionTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TradingSession.TradingSessionType = sessionTypeBox.SelectedItem.ToString();
        }
        private void autoliveCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AutolivePollingTimer = new Timer();
            AutolivePollingTimer.Tick += AutolivePollingTimer_Tick;
            if (autoliveCheckBox.Checked)
            {
                // Start autopolling the live feed based on the granularily.
                var granularily = granularityBox.SelectedItem.ToString();
                if (granularily.Contains("S"))
                {
                    // Trim the letter
                    var newone = Regex.Replace(granularily, "[^0-9.]", "");
                    var duration = Convert.ToInt32(newone) * 1000;
                    AutolivePollingTimer.Interval = duration;
                }
                if (granularily.Contains("M"))
                {
                    // Trim the letter
                    var newone = Regex.Replace(granularily, "[^0-9.]", "");
                    var duration = Convert.ToInt32(newone) * 60 * 1000;
                    AutolivePollingTimer.Interval = duration;
                }
                if (granularily.Contains("H"))
                {
                    // Trim the letter
                    var newone = Regex.Replace(granularily, "[^0-9.]", "");
                    var duration = Convert.ToInt32(newone) * 3600 * 1000;
                    AutolivePollingTimer.Interval = duration;
                }
                AutolivePollingTimer.Enabled = true;
            }
            if (!autoliveCheckBox.Checked)
            {
                notificationBox.Text += @"Autolive feature off";
                AutolivePollingTimer.Tick -= AutolivePollingTimer_Tick;
                AutolivePollingTimer.Enabled = false;
            }
        }
        // Process.
        private void PopulateListBox(ComboBox lsb, string folder, string fileType)
        {
            DirectoryInfo dinfo = new DirectoryInfo(folder);
            FileInfo[] files = dinfo.GetFiles(fileType);
            foreach (FileInfo file in files)
            {
                lsb.Items.Add(file.Name);
            }
        }
        private void AutolivePollingTimer_Tick(object sender, EventArgs e)
        {
            AnalysisTrajectory = analysisTrajectoryBox.SelectedItem.ToString();
            NumberOfPoints = numberOfCandlesBox.Text;
            LoadLiveData();
        }
        #endregion

        #region Boneyard
        //private void SearchAlternateSolution()
        //{
        //    // Prepare the number of learning samples.
        //    SampleSize = _data.Length - PredictionSize - WindowSize;
        //    // Implement data transformation factor.
        //    double factor = 1.7 / trajectoryChart.RangeY.Length;
        //    double yMin = trajectoryChart.RangeY.Min;
        //    var networkInput = new double[WindowSize];
        //    // Solution array.
        //    var solutionSize = _data.Length - WindowSize;
        //    var solution = new double[solutionSize, 2];
        //    // Calculate x-values to be used with solution function.
        //    for (int j = 0; j < solutionSize; j++)
        //    {
        //        solution[j, 0] = j + WindowSize;
        //    }
        //    for (int i = 0, n = _data.Length - WindowSize; i < n; i++)
        //    {
        //        // Assign values from current window as network's input.
        //        for (var j = 0; j < WindowSize; j++)
        //        {
        //            networkInput[j] = (_data[i + j] - yMin) * factor - NetworkCorrection;
        //        }
        //        // Evaluate the function.
        //        solution[i, 1] = (BackPropagationNetwork.Compute(networkInput)[0] + NetworkCorrection) / factor + yMin;
        //        // Compute the prediction and learning error.
        //        if (i >= n - PredictionSize)
        //        {
        //            PredictionError += Math.Abs(solution[i, 1] - _data[WindowSize + i]);
        //        }
        //        else
        //        {
        //            LearningError += Math.Abs(solution[i, 1] - _data[WindowSize + i]);
        //        }
        //    }
        //    // Update the solution on the chart object (off-thread)
        //    trajectoryChart.UpdateDataSeries("solution", solution);

        //    Solution = solution;
        //    Data = _data;
        //    // Report the behavioural attributes.
        //    learningErrorBox.Text = LearningError.ToString(CultureInfo.InvariantCulture);
        //    predictionErrorBox.Text = PredictionError.ToString(CultureInfo.InvariantCulture);
        //    // Record performance vectors for the form and logfile.
        //    Duration = DateTime.Now - StartedOn;
        //    algorithmProcessingTimeBox.Text = Duration.Seconds + @"." + Duration.Milliseconds.ToString(CultureInfo.InvariantCulture);
        //    //Logging.WriteLog("Neural network processed its given task in " + Duration.Seconds + @"." + Duration.Milliseconds + " seconds", Logging.LogType.Statistics, Logging.LogCaller.AgentForm);
        //    //Logging.WriteLog("Program metrics: At an epoch of " + Epoch + ", " + Iterations + " iterations, " + LearningRate + " learning rate, " + PredictionSize + " prediction size; a sigmoid alpha value of " + SigmoidAlpha + " and a momentum of " + Momentum + " resulting in a learning error of " + LearningError + " and a prediction error of " + PredictionError + " captured by the " + Logging.LogCaller.AlgorithmicComputation + " form", Logging.LogType.Statistics, Logging.LogCaller.AlgorithmicComputation);

        //}

        // Create an array of those points only appearing in the prediction size window.
        // On raw data.
        //var decisionBlock = _data.SubArray(_data.Length - PredictionSize, PredictionSize);
        // On solution data.
        //var summy = Solution.GetUpperBound(0);
        //var solutionDecisionBlock = Solution.SubArray(Solution.Length - PredictionSize, PredictionSize, 2);
        // Send the raw data and the solution to the algorithm for trend analysis.
        // The trend using tanglible decisioning.

        //TrendFromData = tangibleDecision.TruthTable;
        // Assuming this to be the correct data sample. Formulate a decision between the sequences in the trend tables.
        //var buyOrSell = tangibleDecision.MakeDecisionBasedOnTrend(TrendFromData);
        // What is the output from the decider process?


        //trendIndicationLabel.Text = @"Upward. Buy!";
        //Refresh();
        // The trend using nebulous decisioning (used on solution data).
        //TrendFromSolution = solutionDecisionBlock.RenderTrend();
        // Formulate a decision between the sequences in the trend tables.
        // Or is the trend downward (of the last two!).
        //var downward = predictionWindowValues.IsLessThanPrevious();
        //trendIndicationLabel.Text = @"Downward. Sell!";
        //Refresh();


        //var next = "";
        #endregion
    }
}
