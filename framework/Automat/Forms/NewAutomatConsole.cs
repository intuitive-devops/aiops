using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Boagaphish;
using Boagaphish.Settings;
using SoftAgent.Automat.Forms.Child;
using SoftAgent.Core;
using SoftAgent.Display;
using SoftAgent.Trend;

namespace SoftAgent.Automat.Forms
{
    public partial class NewAutomatConsole : Form
    {
        private DateTime _agentTaskStartedOn;
        private DateTime _agentTaskStoppedOn;
        private AgentCore HardCoreAgent { get; set; }
        private PredictIndicators _predictor;
        private List<PredictionResults> _results;
        private int PredictionSize { get; set; }
        private int SampleSize { get; set; }
        private double ErrorTolerance { get; set; }
        private int NumberOfUnitsToTransact { get; set; }
        private int SessionType { get; set; }
        public static bool PortfolioDatasetsLoaded { get; set; }
        public static int NumberOfCandles { get; set; }
        public SettingsDictionary GlobalSettings;
        public string CommandRequest;
        public TrainingAlgorithm TrainAlgorithm { get; set; }
        public string PathToXms
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory + @"\data\xms\", xmsFilepathBox.Text);
            }
        }

        public NewAutomatConsole()
        {
            InitializeComponent();
            PopulateListBox(xmsFilepathBox, PathToXms, "*.xml");
            // Initialize global settings.
            GlobalSettings = new SettingsDictionary();
            LoadSettings();
            // Account and instrument parameters
            Decision.Authorization = Convert.ToBoolean(GlobalSettings.GrabSetting("authorization"));
            SessionType = (int)MonitoringSession.MonitoringSessionAccount.Practice;
            Rates.HoldingCurrency = GlobalSettings.GrabSetting("holdingcurrency");
            Rates.TradingCurrency = GlobalSettings.GrabSetting("tradingcurrency");
            sessionTypeBox.DataSource = Enum.GetValues(typeof(MonitoringSession.MonitoringSessionAccount));
            // Data and training parameters
            var algo = GlobalSettings.GrabSetting("trainingalgorithm");
            switch (algo)
            {
                case "Annealing":
                    TrainAlgorithm = Trend.TrainingAlgorithm.Annealing;
                    break;
                case "Evolutionary":
                    TrainAlgorithm = Trend.TrainingAlgorithm.Evolutionary;
                    break;
                case "Resilient":
                    TrainAlgorithm = Trend.TrainingAlgorithm.Resilient;
                    break;
                case "ScaledConjugateGradient":
                    TrainAlgorithm = Trend.TrainingAlgorithm.ScaledConjugateGradient;
                    break;
            }
            // Set the desired settings
            _results = new List<PredictionResults>();
            ErrorTolerance = Convert.ToDouble(GlobalSettings.GrabSetting("errortolerance"));
            PredictionSize = int.Parse(GlobalSettings.GrabSetting("predictionsize"));
            NumberOfUnitsToTransact = int.Parse(GlobalSettings.GrabSetting("numberofunitstotransact"));
            Rates.PredictedRates.OpenPrediction = new double[PredictionSize];
            Rates.PredictedRates.HighPrediction = new double[PredictionSize];
            Rates.PredictedRates.LowPrediction = new double[PredictionSize];
            Rates.PredictedRates.ClosePrediction = new double[PredictionSize];
            HardCoreAgent = new AgentCore();
            // Create required form elements
            numberOfCandlesBox.Text = GlobalSettings.GrabSetting("numberofcandles");
            numberOfAnalyticCandles.Text = GlobalSettings.GrabSetting("numberofcandles");
            NumberOfCandles = int.Parse(numberOfCandlesBox.Text);
        }

        protected void HaveAuthorization()
        {
            Logging.WriteLog("I have authorization.", Logging.LogType.Information, Logging.LogCaller.Automat, "HaveAuthorization");
            if (Decision.Sell)
            {
                try
                {
                    var result = Orders.PostMarketOrder(SessionType, FormatInstrumentPair(), NumberOfUnitsToTransact, "sell");
                    AeonOutput.Text += @"Sell order completed with the result: " + result + Environment.NewLine;
                    Logging.WriteLog(@"Sell order completed with the result: " + result, Logging.LogType.Information, Logging.LogCaller.Automat);
                }
                catch (Exception ex)
                {
                    AeonOutput.Text += @"Sell order failed: " + ex.Message + Environment.NewLine;
                    Logging.WriteLog("An error occured: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Automat, "Place order sell");
                }
            }
            if (Decision.Buy)
            {
                try
                {
                    var result = Orders.PostMarketOrder(SessionType, FormatInstrumentPair(), NumberOfUnitsToTransact, "buy");
                    AeonOutput.Text += @"Buy order completed with the result: " + result + Environment.NewLine;
                    Logging.WriteLog(@"Buy order completed with the result: " + result, Logging.LogType.Information, Logging.LogCaller.Automat);
                }
                catch (Exception ex)
                {
                    AeonOutput.Text += @"Buy order failed: " + ex.Message + Environment.NewLine;
                    Logging.WriteLog("An error occured: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Automat, "Place order buy");
                }
            }
        }
        protected void TradeAutonomously()
        {
            Refresh();
            // Step: Train on data retrieved from a sample.
            AeonOutput.Text += @"Starting training." + Environment.NewLine;
            Logging.WriteLog("Starting training.", Logging.LogType.Information, Logging.LogCaller.Automat);
            StartTraining();
            // Step: Predict the next sequential value from the last data point in the sample.
            AeonOutput.Text += @"Computing a prediction." + Environment.NewLine;
            Logging.WriteLog("Computing a prediction.", Logging.LogType.Information, Logging.LogCaller.Automat);
            RunPrediction();
            // Step: Decide whether or not to trade based on the prediction.
            AeonOutput.Text += @"Deciding whether or not to trade." + Environment.NewLine;
            Logging.WriteLog("Deciding whether or not to trade.", Logging.LogType.Information, Logging.LogCaller.Automat);
            var decision = new Decision(Rates.StoredRates.HighData[SampleSize - 1], Rates.PredictedRates.HighPrediction[0]);
            // Step: Act on the decision whether or not to buy or sell the instrument position.
            if (Decision.Sell)
            {
                AeonOutput.Text += @"I've found a value with enough magnitude that we should sell a set of units. Would you like me to place the order?" + Environment.NewLine;
                Logging.WriteLog("I've found a value with enough magnitude that we should sell a set of units. Would you like me to place the order?", Logging.LogType.Information, Logging.LogCaller.Automat);
                AeonOutput.Text += @"Would you like me to buy?" + Environment.NewLine;
                if (Decision.Authorization)
                    HaveAuthorization();

                else
                {
                    AeonOutput.Text += @"I am not allowed to process this buy." + Environment.NewLine;
                    Logging.WriteLog("I am not allowed to process this sell.", Logging.LogType.Information, Logging.LogCaller.Automat);
                    AeonOutput.Text += @"I am told I cannot sell." + Environment.NewLine;
                }
                    
            }
            if (Decision.Buy)
            {
                AeonOutput.Text += @"I've found a value with enough magnitude that we should buy a set of units. Would you like me to place the order?" + Environment.NewLine;
                Logging.WriteLog("I've found a value with enough magnitude that we should buy a set of units. Would you like me to place the order?", Logging.LogType.Information, Logging.LogCaller.Automat);
                AeonOutput.Text += @"Would you like me to buy?" + Environment.NewLine;
                if (Decision.Authorization)
                    HaveAuthorization();
                else
                {
                    AeonOutput.Text += @"I am not allowed to process this buy." + Environment.NewLine;
                    Logging.WriteLog("I am not allowed to process this autonomously decided buy.", Logging.LogType.Information, Logging.LogCaller.Automat);
                    AeonOutput.Text += @"I am told I cannot buy." + Environment.NewLine;
                }
                    
            }
            if (Decision.Cheese)
            {
                AeonOutput.Text += @"Not enough difference to substantiate the trade." + Environment.NewLine;
                Logging.WriteLog(@"Not enough difference to substantiate the trade.", Logging.LogType.Information, Logging.LogCaller.Automat);
                AeonOutput.Text += @"Will cheese on a trade for now." + Environment.NewLine;
            }
            // Step: Perform post-processing to see how effective the trade was. How is your portfolio doing?

            // Step: How to keep this running for a particular amount of time?
        }

        #region Deep learning elements for Encog (might be useful for tuning my networks)
        private void StartTraining()
        {
            _agentTaskStartedOn = DateTime.Now;

            if (_predictor == null)
            {
                if (!File.Exists(TradeElements.ReturnFilePath(TradeElements.PathToLow)) || !File.Exists(TradeElements.ReturnFilePath(TradeElements.PathToClose)) ||
                    !File.Exists(TradeElements.ReturnFilePath(TradeElements.PathToHigh)) || !File.Exists(TradeElements.ReturnFilePath(TradeElements.PathToOpen)))
                {
                    Logging.WriteLog("Elemental trading files cannot be found.", Logging.LogType.Error, Logging.LogCaller.Automat, "StartTraining");
                    return;
                }
            }
            try
            {
                // Create an indicator.
                _predictor = new PredictIndicators(TradeElements.ReturnFilePath(TradeElements.PathToLow), TradeElements.ReturnFilePath(TradeElements.PathToClose), TradeElements.ReturnFilePath(TradeElements.PathToOpen), TradeElements.ReturnFilePath(TradeElements.PathToHigh), TradeElements.HiddenUnits, TradeElements.HiddenLayers);
                dataStateLabel.Text = @"Csv data loaded into memory,";
            }
            catch (Exception ex)
            {
                Logging.WriteLog("Indicator creation failed. " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Automat, "StartTraining");
                _predictor = null;
                return;
            }
            // Set predictor parameters.
            _predictor.MaxTrainingError = double.Parse(GlobalSettings.GrabSetting("maxtrainingerror"));
            _predictor.TrainingSize = int.Parse(GlobalSettings.GrabSetting("trainingsize"));
            // First strategy is to train on the entire set of data.
            TradeElements.TrainFrom = _predictor.MinIndexDate;
            TradeElements.TrainTo = _predictor.MaxIndexDate;
            // Report the sample size.
            SampleSize = _predictor.SampleSize;
            sampleSizeLabel.Text = @"Sample size: " + SampleSize.ToString(CultureInfo.InvariantCulture);
            Refresh();

            #region train async
            //TrainingStatus callback = TrainingCallback;
            //Predictor.TrainNetworkAsync(_trainFrom, _trainTo, callback);
            #endregion

            _predictor.TrainNetwork(TradeElements.TrainFrom, TradeElements.TrainTo, TrainAlgorithm);
            // Verify data processing and report.
            if (_predictor.TrainingCompleted)
            {
                _agentTaskStoppedOn = DateTime.Now;
                AeonOutput.Text += @"Training error: " + _predictor.TrainingError + Environment.NewLine;
                AeonOutput.Text += @"Training duration: " + ComputeTaskDuration() + Environment.NewLine;
                AeonOutput.Text += @"Training is complete." + Environment.NewLine;
            }
        }
        private void RunPrediction()
        {
            // First strategy is to train on the entire set of data.
            TradeElements.PredictFrom = _predictor.MaxIndexDate;
            TradeElements.PredictTo = TradeElements.PredictFrom.AddMinutes(PredictionSize);
            // Set the error resolution.
            _predictor.MaxTrainingError = ErrorTolerance;
            _agentTaskStartedOn = DateTime.Now;

            try
            {
                _results = _predictor.Predict(TradeElements.PredictFrom, TradeElements.PredictTo);
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.Automat, "RunPrediction");
            }
            // Collect the predicted value which you will use.
            var i = 0;
            foreach (var item in _results)
            {
                Rates.PredictedRates.OpenPrediction[i] = item.PredictedOpen;
                Rates.PredictedRates.HighPrediction[i] = item.PredictedHigh;
                Rates.PredictedRates.LowPrediction[i] = item.PredictedLow;
                Rates.PredictedRates.ClosePrediction[i] = item.PredictedClose;
                i++;
            }
            // Verify data processing and report.
            if (_predictor.PredictionCompleted)
            {
                _agentTaskStoppedOn = DateTime.Now;
                AeonOutput.Text += @"Prediction error: " + _predictor.PredictionError + Environment.NewLine;
                AeonOutput.Text += @"Prediction duration: " + ComputeTaskDuration() + Environment.NewLine;
                AeonOutput.Text += @"Next value for high will be: " + Rates.PredictedRates.HighPrediction[0] + Environment.NewLine;
                AeonOutput.Text += @"Prediction is complete." + Environment.NewLine;
            }
        }
        #endregion

        #region Utilities
        public void LoadSettings()
        {
            var path = Path.Combine(Environment.CurrentDirectory, Path.Combine("config", "Settings.xml"));
            GlobalSettings.LoadSettings(path);
        }
        private TimeSpan ComputeTaskDuration()
        {
            return _agentTaskStoppedOn - _agentTaskStartedOn;
        }
        private static string FormatInstrumentPair()
        {
            var firstPair = Rates.HoldingCurrency;
            var secondPair = Rates.TradingCurrency;
            return firstPair + "_" + secondPair;
        }
        #endregion

        #region Events
        private static void PopulateListBox(ComboBox lsb, string folder, string fileType)
        {
            try
            {
                var dinfo = new DirectoryInfo(folder);
                var files = dinfo.GetFiles(fileType);
                foreach (var file in files)
                {
                    lsb.Items.Add(file.Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Listbox error", MessageBoxButtons.OK, MessageBoxIcon.Error,MessageBoxDefaultButton.Button1);
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.Automat);
            }
            
        }
        private void refreshListButton_Click(object sender, EventArgs e)
        {
            xmsFilepathBox.Items.Clear();
            PopulateListBox(xmsFilepathBox, PathToXms, "*.xml");
        }
        private void AeonOutput_TextChanged(object sender, EventArgs e)
        {
            // How can this be creatively used?
        }
        private void loadLiveDataButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            //AeonOutput.Text += HardCoreAgent.ProcessLiveData(int.Parse(numberOfCandlesBox.Text)) + Environment.NewLine;
            Cursor.Current = Cursors.Default;
        }
        private void loadChartFormButton_Click(object sender, EventArgs e)
        {
            if (xmsFilepathBox.Text == "")
                xmsFilepathBox.Text = @"LiveCandles_200.xml";
            if (AnalyticChart.Instance == false)
            {
                var form = new AnalyticChart(this, HardCoreAgent, ChartProperties.ChartType.Candles, xmsFilepathBox.Text);
                form.Show(this);
                AnalyticChart.Instance = true;
            }
            else if (AnalyticChart.Instance)
            {
                // Do nothing.
            }
        }
        private void sessionTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MonitoringSession.MonitoringSessionType = sessionTypeBox.SelectedItem.ToString();
        }
        private void xmsFilepathBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var file = PathToXms.Split('_');
            var number = file[1].Split('.');
            numberOfAnalyticCandles.Text = number[0];
        }
        private void accountsFormButton_Click(object sender, EventArgs e)
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
        #endregion 

    }
}
