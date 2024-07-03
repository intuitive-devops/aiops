using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Boagaphish;
using Boagaphish.Controls;
using Boagaphish.Numeric;
using SoftAgent.Core;
using SoftAgent.Display;

namespace SoftAgent.Automat.Forms.Child
{
    // Todo: First draft is using historical rates class.
    public partial class AnalyticChart : Form
    {
        private readonly object _lock = new object();
        private bool _threadRunning;
        private bool _abortRequested;
        private Form _owner;
        private readonly ChartProperties.ChartType _chartType;
        private AgentCore ChartAgent { get; set; }
        private XmlDocument _document;
        private double[,] _dataToShow;
        private readonly double[,] _windowSizeDelimiter = { { 0, 0 }, { 0, 0 } };
        private readonly double[,] _forecastSizeDelimiter = { { 0, 0 }, { 0, 0 } };
        private TransferFunction _transferFunction;

        public static bool Instance { get; set; }
        public string XmsFile { get; set; }
        public int[] MarketVolume { get; set; }
        public static string TrajectoryProcessingTime { get; set; }
        public string AnalysisTrajectory { get; set; }
        public double[] RateTrajectory { get; set; }
        public string NumberOfCandles { get; set; }
        public DateTime StartedOn;
        public TimeSpan Duration;
        public string CurrencyHolding { get; set; }
        public string CurrencyTrading { get; set; }
        public int WindowSize { get; set; }
        public int ForecastSize { get; set; }
        public TransferFunction TransferFunction { get; set; }
        public string PathToXms
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory + @"\data\xms\", XmsFile);
            }
        }

        public AnalyticChart(Form mOwner, AgentCore core, ChartProperties.ChartType chartType, string xmsFile)
        {
            InitializeComponent();
            _owner = mOwner;
            _chartType = chartType;
            XmsFile = xmsFile;
            ChartAgent = core;
            WindowSize = ChartAgent.WindowSize;
            ForecastSize = ChartAgent.ForecastSize;
            numberOfCandlesBox.Text = ReturnNumberOfCandles();
            CurrencyHolding = "EUR";
            CurrencyTrading = "GBP";
            //candleFormatBox.SelectedItem = "midpoint";
            //granularityBox.SelectedItem = "M1";
            //dailyAlignmentBox.SelectedItem = "0";
            analyticIterationsBox.Text = @"10";
            analysisTrajectoryBox.DataSource = Enum.GetValues(typeof(Rates.HistoricalRates.Trajectory));
            transferFunctionSelectionBox.DataSource = Enum.GetValues(typeof (TransferFunction));
            displayChart.AddDataSeries("data", Color.Black, Chart.SeriesType.ConnectedDots, 4);
            displayChart.AddDataSeries("window", Color.LightGray, Chart.SeriesType.Line, 1, false);
            displayChart.AddDataSeries("solution", Color.Blue, Chart.SeriesType.ConnectedDots, 5);
            displayChart.AddDataSeries("forecastDelimiter", Color.Green, Chart.SeriesType.Line, 1, false);
            displayChart.AddDataSeries("forecast", Color.Goldenrod, Chart.SeriesType.ConnectedDots, 4);
            Instance = true;
        }
        public void ChartFormLoad(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            switch (_chartType)
            {
                    case ChartProperties.ChartType.Candles:
                    Enum.TryParse(transferFunctionSelectionBox.SelectedItem.ToString(), out _transferFunction);
                    TransferFunction = _transferFunction;
                    Cursor.Current = Cursors.WaitCursor;
                    NumberOfCandles = numberOfCandlesBox.Text;

                    LoadLocalDataBuildChart();

                    analysisTrajectoryBox.SelectedItem = "High";
                    AnalysisTrajectory = analysisTrajectoryBox.SelectedItem.ToString();
                    granularityBox.Text = Rates.HistoricalRates.Granularity;
                    transferFunctionSelectionBox.SelectedItem = TransferFunction.BipolarSigmoid;
                    Cursor.Current = Cursors.Default;
                    break;
            }
        }

        #region Events
        private void analyzeDatasetButton_Click(object sender, EventArgs e)
        {
            ClearChart("solution");
            //ClearChart("data");
            ClearChart("forecast");
            //displayChart.AddDataSeries("data", Color.Black, Chart.SeriesType.ConnectedDots, 4);
            displayChart.AddDataSeries("solution", Color.Blue, Chart.SeriesType.ConnectedDots, 4);
            displayChart.AddDataSeries("forecast", Color.Goldenrod, Chart.SeriesType.ConnectedDots, 4);
            processingTimeBox.Text = "";
            Refresh();
            if (_transferFunction == TransferFunction.None)
            {
                processingTimeBox.Text = @"T-func none";
                return;
            }

            RunAnalysis();
        }
        private void forecastButton_Click(object sender, EventArgs e)
        {
            XmsFile = "ForecastNetwork.xml";
            ClearChart("forecast");
            displayChart.AddDataSeries("forecast", Color.Goldenrod, Chart.SeriesType.ConnectedDots, 4);
            if (Rates.HistoricalRates.RatesLoaded)
                StartForecast();
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
            //        ThreadPool.QueueUserWorkItem(StartForecast);
            //    }
            //}
            else
            {
                MessageBox.Show(
                    @"Cannot forecast the future without knowing the past, therefore no future trend is to be found.",
                    @"Empty forecase", MessageBoxButtons.OK,
                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }
        private void prospectButton_Click(object sender, EventArgs e)
        {
            decisionBox.Text = ChartAgent.ProspectTrade();
        }
        public void ChartFormClosing(object sender, FormClosingEventArgs e)
        {
            Instance = false;
        }
        private void ClearChart(string series)
        {
            displayChart.RemoveDataSeries(series);
        }
        private void analysisTrajectoryBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RateTrajectory == null)
                return;
            var selection = analysisTrajectoryBox.SelectedItem.ToString();
            //ClearChart("data");
            ClearChart("solution");
            ClearChart("forecast");
            //displayChart.AddDataSeries("data", Color.Black, Chart.SeriesType.ConnectedDots, 4);
            displayChart.AddDataSeries("solution", Color.Blue, Chart.SeriesType.ConnectedDots, 4);
            displayChart.AddDataSeries("forecast", Color.Goldenrod, Chart.SeriesType.ConnectedDots, 4);
            RateTrajectory = new double[Rates.HistoricalRates.NumberOfCandles];
            switch (selection)
            {
                case "High":
                    RateTrajectory = Rates.HistoricalRates.HighData;
                    BuildChart();
                    RunAnalysis();
                    break;
                case "Open":
                    RateTrajectory = Rates.HistoricalRates.OpenData;
                    BuildChart();
                    RunAnalysis();
                    break;
                case "Close":
                    RateTrajectory = Rates.HistoricalRates.CloseData;
                    BuildChart();
                    RunAnalysis();
                    break;
                case "Low":
                    RateTrajectory = Rates.HistoricalRates.LowData;
                    BuildChart();
                    RunAnalysis();
                    break;
            }
            MarketVolume = Rates.HistoricalRates.VolumeData;
        }
        private void transferFunctionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Enum.TryParse(transferFunctionSelectionBox.SelectedItem.ToString(), out _transferFunction);
            TransferFunction = _transferFunction;
        }
        #endregion

        #region Methods (which should be collected)

        private void ChangeTrajectory()
        {
            var selection = analysisTrajectoryBox.SelectedItem.ToString();
            switch (selection)
            {
                case "High":
                    RateTrajectory = Rates.HistoricalRates.HighData;
                    break;
                case "Open":
                    RateTrajectory = Rates.HistoricalRates.OpenData;
                    break;
                case "Close":
                    RateTrajectory = Rates.HistoricalRates.CloseData;
                    break;
                case "Low":
                    RateTrajectory = Rates.HistoricalRates.LowData;
                    break;
            }
            MarketVolume = Rates.HistoricalRates.VolumeData;
        }
        public void LoadLocalDataBuildChart()
        {
            Cursor.Current = Cursors.WaitCursor;
            var constructData = ChartAgent.LoadLocalData(PathToXms);
            if (Rates.HistoricalRates.RatesLoaded)
            {
                var build = BuildChart();
            }
            Cursor.Current = Cursors.Default;
        }
        public bool BuildChart()
        {
            if (Rates.HistoricalRates.RatesLoaded)
            {
                StartedOn = DateTime.Now;
                _document = new XmlDocument();
                try
                {
                    _document.Load(PathToXms);
                }
                catch
                {
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
            RateTrajectory = new double[numberOfCandles];

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
            ChangeTrajectory();
            // Release.
            _document = null;
            // Process the data.
            // Allocate and set the data.
            ChartAgent.Data = new double[numberOfCandles];
            _dataToShow = new double[numberOfCandles, 2];
            Array.Copy(RateTrajectory, 0, ChartAgent.Data, 0, numberOfCandles);
            for (var j = 0; j < numberOfCandles; j++)
            {
                _dataToShow[j, 0] = j;
                _dataToShow[j, 1] = ChartAgent.Data[j];
            }
            // Update chart.
            displayChart.RangeX = new DoubleRange(0, ChartAgent.Data.Length - 1 + ForecastSize);
            displayChart.UpdateDataSeries("data", _dataToShow);
            // Record performance vectors for the operation.
            Duration = DateTime.Now - StartedOn;
            TrajectoryProcessingTime = Duration.Seconds + @"." + Duration.Milliseconds;
            processingTimeBox.Text = TrajectoryProcessingTime;
            UpdateDelimiters();
            return true;
        }
        private void RunAnalysis()
        {
            var iterations = int.Parse(analyticIterationsBox.Text);
            StartedOn = DateTime.Now;
            Cursor.Current = Cursors.WaitCursor;
            ChartAgent.TrainNetwork(TransferFunction);
            switch (transferFunctionSelectionBox.SelectedItem.ToString())
            {
                case "BipolarSigmoid":
                    _transferFunction = TransferFunction.BipolarSigmoid;
                    break;
                case "Gaussian":
                    _transferFunction = TransferFunction.Gaussian;
                    break;
                case "Linear":
                    _transferFunction = TransferFunction.Linear;
                    break;
                case "NormalizedExponent":
                    _transferFunction = TransferFunction.NormalizedExponent;
                    break;
                case "RationalSigmoid":
                    _transferFunction = TransferFunction.RationalSigmoid;
                    break;
                case "Sigmoid":
                    _transferFunction = TransferFunction.Sigmoid;
                    break;
            }
            for (var i = 0; i < iterations; i++)
            {
                displayChart.UpdateDataSeries("solution", ChartAgent.SearchSolution(displayChart.RangeY.Min, displayChart.RangeY.Length));
            }
            Duration = DateTime.Now - StartedOn;
            TrajectoryProcessingTime = Duration.Seconds + @"." + Duration.Milliseconds;
            processingTimeBox.Text = TrajectoryProcessingTime;
            Cursor.Current = Cursors.Default;
        }
        public string FormatInstrument()
        {
            var firstPair = CurrencyHolding;
            var secondPair = CurrencyTrading;
            return firstPair + "_" + secondPair;
        }
        private string XPathValue(string xPath)
        {
            var node = _document.SelectSingleNode((xPath));
            if (node == null)
                Logging.WriteLog(@"Cannot find the specified node.", Logging.LogType.Error, Logging.LogCaller.AgentCore, "XPathValue");
            return node != null ? node.InnerText : "";
        }
        private void UpdateDelimiters()
        {
            if (ChartAgent.Data == null) return;
            // The window delimiter.
            _windowSizeDelimiter[0, 0] = _windowSizeDelimiter[1, 0] = WindowSize;
            _windowSizeDelimiter[0, 1] = displayChart.RangeY.Min;
            _windowSizeDelimiter[1, 1] = displayChart.RangeY.Max;
            displayChart.UpdateDataSeries("window", _windowSizeDelimiter);
            // The prediction delimiter.
            _forecastSizeDelimiter[0, 0] = _forecastSizeDelimiter[1, 0] = ChartAgent.Data.Length - 1 - ForecastSize;
            _forecastSizeDelimiter[0, 1] = displayChart.RangeY.Min;
            _forecastSizeDelimiter[1, 1] = displayChart.RangeY.Max;
            displayChart.UpdateDataSeries("forecastDelimiter", _forecastSizeDelimiter);
        }
        private string ReturnNumberOfCandles()
        {
            var file = PathToXms.Split('_');
            var number = file[1].Split('.');
            return number[0];
        }
        private void StartForecast(/*object state*/)
        {
            try
            {
                var iterations = int.Parse(analyticIterationsBox.Text);
                StartedOn = DateTime.Now;
                Cursor.Current = Cursors.WaitCursor;
                for (var i = 0; i < iterations; i++)
                {
                    displayChart.UpdateDataSeries("forecast", ChartAgent.SearchForecastSolution(displayChart.RangeY.Min, displayChart.RangeY.Length, true));
                }
                Duration = DateTime.Now - StartedOn;
                TrajectoryProcessingTime = Duration.Seconds + @"." + Duration.Milliseconds;
                //nextForecastValueBox.Text = ChartAgent.ForecastValueNext.ToString(CultureInfo.InvariantCulture);
                Cursor.Current = Cursors.Default;
                //SearchForecastUsingCustomPropagationNetwork();
            }
            finally
            {
                lock (_lock)
                {
                    _threadRunning = false;
                }
            }
        }

        #endregion

        

    }
}
