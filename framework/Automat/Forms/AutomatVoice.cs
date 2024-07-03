using System;
using System.Collections.Generic;
using System.IO;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Windows.Forms;
using Boagaphish;
using Boagaphish.Settings;
using SoftAgent.Core;
using SoftAgent.Trend;

namespace SoftAgent.Automat.Forms
{
    public partial class AutomatVoice : Form
    {
        private DateTime _agentTaskStartedOn;
        private DateTime _agentTaskStoppedOn;
        private static bool SpeechInit { get; set; }
        private bool _ableToListen;
        private AgentCore HardCoreAgent { get; set; }
        //private SoundPlayer ForexActive { get; set; }

        static readonly SpeechSynthesizer SpeechSynth = new SpeechSynthesizer();
        static readonly PromptBuilder PromptBuilder = new PromptBuilder();
        static readonly SpeechRecognitionEngine Recognizer = new SpeechRecognitionEngine();
        static readonly GrammarBuilder GrammarBuilder = new GrammarBuilder();

        private PredictIndicators _predictor;
        private List<PredictionResults> _results;
        private int PredictionSize { get; set; }
        private int SampleSize { get; set; }
        private double ErrorResolution { get; set; }
        private int NumberOfUnitsToTransact { get; set; }
        private int SessionType { get; set; }

        public SettingsDictionary GlobalSettings;
        public string CommandRequest;
        public TrainingAlgorithm TrainAlgorithm { get; set; }

        public AutomatVoice()
        {
            InitializeComponent();
            // Initialize global settings.
            GlobalSettings = new SettingsDictionary();
            LoadSettings();
            // Account and instrument parameters
            Decision.Authorization = Convert.ToBoolean(GlobalSettings.GrabSetting("authorization"));
            SessionType = (int)TradingSession.TradingSessionAccount.Practice;
            Rates.HoldingCurrency = GlobalSettings.GrabSetting("holdingcurrency");
            Rates.TradingCurrency = GlobalSettings.GrabSetting("tradingcurrency");
            //TrainAlgorithm = GlobalSettings.GrabSetting("trainingalgorithm");
            TrainAlgorithm = Trend.TrainingAlgorithm.Resilient;
            _results = new List<PredictionResults>();
            ErrorResolution = Convert.ToDouble(GlobalSettings.GrabSetting("errorresolution"));
            PredictionSize = int.Parse(GlobalSettings.GrabSetting("predictionsize"));
            NumberOfUnitsToTransact = int.Parse(GlobalSettings.GrabSetting("numberofunitstotransact"));
            Rates.PredictedRates.OpenPrediction = new double[PredictionSize];
            Rates.PredictedRates.HighPrediction = new double[PredictionSize];
            Rates.PredictedRates.LowPrediction = new double[PredictionSize];
            Rates.PredictedRates.ClosePrediction = new double[PredictionSize];
            HardCoreAgent = new AgentCore();
            toolStripMenuItemSpeech.Checked = true;
            SpeechInit = InitializeSpeechEngine();
            ProcessInput("This soft agent is initialized.");
            //ForexActive = new SoundPlayer(Environment.CurrentDirectory + @"\sounds\introduction.wav");
            //ForexActive.Play();
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
                    ProcessInput("Order placed for sell.");
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
                    ProcessInput("Order placed for buy.");
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
                ProcessInput("Would you like me to buy?");
                if (Decision.Authorization)
                    HaveAuthorization();

                else
                {
                    AeonOutput.Text += @"I am not allowed to process this buy." + Environment.NewLine;
                    Logging.WriteLog("I am not allowed to process this sell.", Logging.LogType.Information, Logging.LogCaller.Automat);
                    ProcessInput("I am told I cannot sell.");
                }
                    
            }
            if (Decision.Buy)
            {
                AeonOutput.Text += @"I've found a value with enough magnitude that we should buy a set of units. Would you like me to place the order?" + Environment.NewLine;
                Logging.WriteLog("I've found a value with enough magnitude that we should buy a set of units. Would you like me to place the order?", Logging.LogType.Information, Logging.LogCaller.Automat);
                ProcessInput("Would you like me to buy?");
                if (Decision.Authorization)
                    HaveAuthorization();
                else
                {
                    AeonOutput.Text += @"I am not allowed to process this buy." + Environment.NewLine;
                    Logging.WriteLog("I am not allowed to process this autonomously decided buy.", Logging.LogType.Information, Logging.LogCaller.Automat);
                    ProcessInput("I am told I cannot buy.");
                }
                    
            }
            if (Decision.Cheese)
            {
                AeonOutput.Text += @"Not enough difference to substantiate the trade." + Environment.NewLine;
                Logging.WriteLog(@"Not enough difference to substantiate the trade.", Logging.LogType.Information, Logging.LogCaller.Automat);
                ProcessInput("Will cheese on a trade for now.");
            }
            // Step: Perform post-processing to see how effective the trade was. How is your portfolio doing?

            // Step: How to keep this running for a particular amount of time?
        }

        #region Deep learning elements
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
            }
            catch (Exception ex)
            {
                Logging.WriteLog("Indicator creation failed. " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Automat, "StartTraining");
                _predictor = null;
                return;
            }
            // First strategy is to train on the entire set of data.
            TradeElements.TrainFrom = _predictor.MinIndexDate;
            TradeElements.TrainTo = _predictor.MaxIndexDate;
            // Set the error resolution.
            _predictor.MaxTrainingError = ErrorResolution;
            // Report the sample size.
            SampleSize = _predictor.SampleSize;

            #region train async
            //TrainingStatus callback = TrainingCallback;
            //Predictor.TrainNetworkAsync(_trainFrom, _trainTo, callback);
            #endregion

            _predictor.TrainNetwork(TradeElements.TrainFrom, TradeElements.TrainTo, TrainAlgorithm);
            // Verify data processing and report.
            if (_predictor.TrainingCompleted)
            {
                _agentTaskStoppedOn = DateTime.Now;
                AeonOutput.Text += @"Training is completed." + Environment.NewLine;
                AeonOutput.Text += @"Training error: " + _predictor.TrainingError + Environment.NewLine;
                AeonOutput.Text += @"Training duration: " + ComputeTaskDuration() + Environment.NewLine;
                ProcessInput("Training is complete.");
            }
        }
        private void RunPrediction()
        {
            // First strategy is to train on the entire set of data.
            TradeElements.PredictFrom = _predictor.MaxIndexDate;
            TradeElements.PredictTo = TradeElements.PredictFrom.AddMinutes(PredictionSize);
            // Set the error resolution.
            _predictor.MaxTrainingError = ErrorResolution;
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
                AeonOutput.Text += @"Prediction is completed." + Environment.NewLine;
                AeonOutput.Text += @"Prediction error: " + _predictor.PredictionError + Environment.NewLine;
                AeonOutput.Text += @"Prediction duration: " + ComputeTaskDuration() + Environment.NewLine;
                AeonOutput.Text += @"Next value for high will be: " + Rates.PredictedRates.HighPrediction[0] + Environment.NewLine;
                ProcessInput("Prediction is complete.");
            }
        }
        #endregion

        #region Conversation processing
        private static string ProcessInput(string input)
        {
            if (!SpeechInit) return input;
            PromptBuilder.ClearContent();
            PromptBuilder.AppendText(input);
            SpeechSynth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
            SpeechSynth.Speak(PromptBuilder);
            return input;
        }
        private void ProcessCommand()
        {
            switch (CommandRequest)
            {
                case "aeon":
                    ProcessInput("my name");
                    break;
                case "load file data":
                    //AeonOutput.Text += ProcessInput(HardCoreAgent.LoadLocalData()) + Environment.NewLine;
                    break;
                case "load live data":
                    break;
                case "list available commands":
                    ProcessInput("The commands available are:" + GrammarBuilder.DebugShowPhrases);
                    break;
                case "train solver network":
                    //if (AgentCore.LocalDataLoaded)
                    //    AeonOutput.Text += ProcessInput(HardCoreAgent.TrainSolverNetwork()) + Environment.NewLine;
                    //else
                        ProcessInput("Load a dataset first.");
                    break;
                case "train forecast network":
                    break;
                case "cast solution":
                    ProcessInput("Please wait.");
                    //if (AgentCore.LocalDataLoaded && AgentCore.SolverTrainingDone)
                        //AeonOutput.Text += ProcessInput(HardCoreAgent.CastSolution()) + Environment.NewLine;
                    //else
                        ProcessInput("Load a dataset and do some training before trying to find a solution.");
                    break;
                case "aeon auto trade":
                    ProcessInput("Intializing a trading session.");
                    _ableToListen = false;
                    TradeAutonomously();
                    _ableToListen = true;
                    break;
                case "exit":
                    //ProcessInput("Exiting the application. Goodbye.");
                    ProcessInput("Voice activated exit is disabled.");
                    //Application.Exit();
                    break;
                case "stop listening":
                    ProcessInput("Listening is disabled.");
                    _ableToListen = false; // Stop listening.
                    break;
                case "how many commands":
                    ProcessInput("There are" + GrammarBuilder.DebugShowPhrases.Length + "commands available.");
                    break;
            }
        }
        private void RecognizerSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Here is where the logic of the hardware connects to the software.
            CommandRequest = e.Result.Text;
            if (CommandRequest == "start listening")
            {
                _ableToListen = true; // Resume listening.
                ProcessInput("Resuming listening.");
            }
            if (!_ableToListen) return;
            if (CommandRequest == "I agree aeon go ahead")
            {
                HaveAuthorization();
            }
            ProcessCommand();
            Logging.WriteLog(CommandRequest, Logging.LogType.Information, Logging.LogCaller.AgentVoice);
        }
        private bool InitializeSpeechEngine()
        {
            try
            {
                // Read in the list of phrases that the speech engine will recognise when it detects it being spoken.
                GrammarBuilder.Append(
                    new Choices(File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, Path.Combine("grammar", "forex-grammar.txt")))));
            }
            catch (Exception ex)
            {
                Logging.WriteLog("Failure in intializing the speech engine: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.AgentVoice, "GrammarBuilder.Append");
                return false;
            }
            var gr = new Grammar(GrammarBuilder);
            try
            {
                Recognizer.UnloadAllGrammars();
                Recognizer.RecognizeAsyncCancel();
                Recognizer.RequestRecognizerUpdate();
                Recognizer.LoadGrammar(gr);
                Recognizer.SpeechRecognized += RecognizerSpeechRecognized;
                Recognizer.SetInputToDefaultAudioDevice();
                Recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.AgentVoice);
                //Logging.DumpToConsole(ex.Message);
            }
            AeonOutput.Text += @"This soft agent is ready for commands." + Environment.NewLine;
            _ableToListen = true;
            return true;
        }
        #endregion

        #region Utilities
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

        #region Add expansion to the program at runtime
        private void addExtraCodeFragmentsMenuItem_Click(object sender, EventArgs e)
        {
            // Add new personality traits from code fragments without relaunching the application.
            try
            {
                //Cursor.Current = Cursors.WaitCursor;
                //AeonLoader loader = new AeonLoader(_myAeon);
                //_myAeon.IsAcceptingUserInput = false;
                //loader.LoadAeon(_myAeon.PathToExtras);
                //Logging.RecordTranscript("---Extras loaded---");
                //_myAeon.IsAcceptingUserInput = true;
                //Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.AgentGui);
                MessageBox.Show(@"Unable to load extras feature code fragment(s).", @"Fragment loading error", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                //Cursor.Current = Cursors.Default;
            }

        }
        private void addExtraAssembliesMenuItem_Click(object sender, EventArgs e)
        {
            // Add new personality traits from full-blow assemblies without relaunching the application.
            try
            {
                //Cursor.Current = Cursors.WaitCursor;
                //AeonLoader loader = new AeonLoader(_myAeon);
                //_myAeon.IsAcceptingUserInput = false;
                //loader.LoadAeon(_myAeon.PathToExtras);
                //Logging.RecordTranscript("---Extras loaded---");
                //_myAeon.IsAcceptingUserInput = true;
                //Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.AgentGui);
                MessageBox.Show(@"Unable to load extras feature assembly.", @"Assembly loading error", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                //Cursor.Current = Cursors.Default;
            }
        }
        #endregion

        #region Events
        public void LoadSettings()
        {
            var path = Path.Combine(Environment.CurrentDirectory, Path.Combine("config", "Settings.xml"));
            GlobalSettings.LoadSettings(path);
        }
        //private void TrainingCallback(int epoch, double error, TrainingAlgorithm algorithm)
        //{
        //    Invoke(addAction, epoch, error, algorithm, _dgvTrainingResults);
        //}
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Cartheur presents: forex algorithm, version " + SharedFunctions.ApplicationVersion + @". Copyright 2008-2017 Cartheur Robotics, spol. s r.o., All rights reserved.", @"About this program", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }
        private void licenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"There is a substantive license for this program. Plunge into the abstract void, but don't be a piff.", @"License", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }
        private void clearConsoleToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            AeonOutput.Text = "";
            Refresh();
        }
        //private void MyAeonWrittenToLog()
        //{
        //    //AlgorithmOutput.Text += _thisAeon.LastLogMessage + Environment.NewLine + Environment.NewLine;
        //    AeonOutput.ScrollToCaret();
        //}
        private void AeonOutput_TextChanged(object sender, EventArgs e)
        {
            // How can this be creatively used?
        }
        #endregion
    }
}
