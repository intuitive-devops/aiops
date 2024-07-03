using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Boagaphish;
using Encog.Engine.Network.Activation;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Networks.Training.Propagation.SCG;
using Encog.Utilities;

namespace SoftAgent.Trend
{
    /// <summary>
    /// Training algorithm
    /// </summary>
    public enum TrainingAlgorithm
    {
        /// <summary>
        /// Backpropagation learning
        /// </summary>
        Resilient,
        /// <summary>
        /// Simulated annealing
        /// </summary>
        Annealing,
        /// <summary>
        /// Evolutionary learning
        /// </summary>
        Evolutionary,
        ScaledConjugateGradient
    }
    /// <summary>
    /// Training status delegate
    /// </summary>
    /// <param name="iteration">Epoch number</param>
    /// <param name="error">Error</param>
    /// <param name="algorithm">Training algorithm</param>
    public delegate void TrainingStatus(int iteration, double error, TrainingAlgorithm algorithm);
    /// <summary>
    /// Class for prediction
    /// </summary>
    public sealed class PredictIndicators
    {
        /// <summary>
        /// Indexes to consider
        /// </summary>
        /// <remarks>
        /// Dow index, Prime interest rate, Nasdaq index, SnP500 index
        /// </remarks>
        private const int IndexesToConsider = 4;
        /// <summary>
        /// Input Tuples. Each tuple consist of a pair: <c>SnP500</c> index and prime interest rate PIR
        /// </summary>
        /// <remarks>
        /// The total amount of input synapses equals <c>InputTupples * IndexesToConsider</c>
        /// </remarks>
        private const int InputTuples = 10;
        /// <summary>
        /// The size of network's output
        /// </summary>
        private const int OutputSize = 4;
        /// <summary>
        /// Maximum allowable training error. Default is 5E-4.
        /// </summary>
        public double MaxTrainingError { get; set; }
        public double TrainingError { get; private set; }
        public double PredictionError { get; private set; }
        public int SampleSize { get; private set; }
        /// <summary>
        /// Network to be trained
        /// </summary>
        private BasicNetwork _network;
        /// <summary>
        /// Input data SnP, Prime Interest Rate, Nasdaq, Dow indexes
        /// </summary>
        private double[][] _input;
        /// <summary>
        /// Desired output
        /// </summary>
        private double[][] _ideal;
        private PredictorManager _manager;
        private Thread _trainThread;
        private string _pathToLow;
        private string _pathToClose;
        private string _pathToOpen;
        private string _pathToHigh;
        public int TrainingSize { get; set; }

        public PredictIndicators(string pathToLow, string pathToClose, string pathToOpen, string pathToHigh, int hiddenUnits, int hiddenLayers)
        {
            if (!File.Exists(pathToLow))
                throw new ArgumentException("pathToLow targets an invalid file");
            if (!File.Exists(pathToClose))
                throw new ArgumentException("pathToClose targets an invalid file");
            if (!File.Exists(pathToOpen))
                throw new ArgumentException("pathToOpen targets an invalid file");
            if (!File.Exists(pathToHigh))
                throw new ArgumentException("pathToHigh targets an invalid file");

            _pathToLow = pathToLow;
            _pathToClose = pathToClose;
            _pathToOpen = pathToOpen;
            _pathToHigh = pathToHigh;

            CreateNetwork(hiddenUnits, hiddenLayers);                                                       /*Create new network*/
            _manager = new PredictorManager(InputTuples, OutputSize);     /*Create new financial predictor manager*/
            _manager.Load(_pathToLow, _pathToClose, _pathToOpen, _pathToHigh);     /*Load SnP 500 and prime interest rates*/
            Loaded = true;
            HiddenLayers = hiddenLayers;
            HiddenUnits = hiddenUnits;
            SampleSize = _manager.Size;
        }
        public bool Loaded { get; private set; }
        public bool TrainingCompleted { get; private set; }
        public bool PredictionCompleted { get; private set; }
        /// <summary>
        /// Hidden layers
        /// </summary>
        public int HiddenLayers { get; private set; }
        /// <summary>
        /// Hidden units
        /// </summary>
        public int HiddenUnits { get; private set; }
        /// <summary>
        /// Maximum date for training and prediction
        /// </summary>
        public DateTime MaxIndexDate
        {
            get
            {
                return _manager == null ? DateTime.MinValue : _manager.MaxDate;
            }
        }
        /// <summary>
        /// Minimum date for training and prediction
        /// </summary>
        public DateTime MinIndexDate
        {
            get
            {
                return _manager == null ? DateTime.MaxValue : _manager.MinDate;
            }
        }

        public void ReloadFiles(string pathToSp500, string pathToPrimeRates, string pathToDow, string pathToNasdaq)
        {
            if (!File.Exists(pathToSp500))
                throw new ArgumentException("pathToSP500 targets an invalid file");
            if (!File.Exists(pathToPrimeRates))
                throw new ArgumentException("pathToPrimeRates targets an invalid file");
            if (!File.Exists(pathToDow))
                throw new ArgumentException("pathToDow targets an invalid file");
            if (!File.Exists(pathToNasdaq))
                throw new ArgumentException("pathToNasdaq targets an invalid file");
            Loaded = false;
            _pathToLow = pathToSp500;
            _pathToClose = pathToPrimeRates;
            _pathToOpen = pathToDow;
            _pathToHigh = pathToNasdaq;
            _manager = new PredictorManager(InputTuples, OutputSize);     /*Create new financial predictor manager*/
            _manager.Load(_pathToLow, _pathToClose, _pathToOpen, _pathToHigh);     /*Load SnP 500 and prime interest rates*/
            _ideal = _input = null;
            Loaded = true;
        }
        /// <summary>
        /// Create a new network
        /// </summary>
        private void CreateNetwork(int hiddenUnits, int hiddenLayers)
        {
            _network = new BasicNetwork(); //{ Name = "Financial Predictor", Description = "Network for prediction analysis" };
            _network.AddLayer(new BasicLayer(InputTuples * IndexesToConsider));                             /*Input*/
            for (int i = 0; i < hiddenLayers; i++)
                _network.AddLayer(new BasicLayer(new ActivationTANH(), true, hiddenUnits));                 /*Hidden layer*/
            _network.AddLayer(new BasicLayer(new ActivationTANH(), true, OutputSize));                      /*Output of the network*/
            _network.Structure.FinalizeStructure();                                                         /*Finalize network structure*/
            _network.Reset();                                                                               /*Randomize*/
        }
        /// <summary>
        /// Create Training sets for the neural network to be trained
        /// </summary>
        /// <param name="trainFrom">Initial date, from which to gather indexes</param>
        /// <param name="trainTo">Final date, to which to gather indexes</param>
        public void CreateTrainingSets(DateTime trainFrom, DateTime trainTo)
        {
            // find where we are starting from
            int startIndex = -1;
            int endIndex = -1;
            foreach (var sample in _manager.Samples)
            {
                if (sample.Date.CompareTo(trainFrom) < 0)
                    startIndex++;
                if (sample.Date.CompareTo(trainTo) < 0)
                    endIndex++;
            }
            // Create a sample across the training area.
            TrainingSize = Math.Abs(startIndex - endIndex);// Swapped these: endIndex - startIndex since the result is negative.
            _input = new double[TrainingSize][];
            _ideal = new double[TrainingSize][];

            // Grab the actual training data from that point.
            for (var i = startIndex; i < endIndex; i++)
            {
                _input[i - startIndex] = new double[InputTuples * IndexesToConsider];
                _ideal[i - startIndex] = new double[OutputSize];
                _manager.GetInputData(i, _input[i - startIndex]);
                _manager.GetOutputData(i, _ideal[i - startIndex]);
            }
#if LOG_DATASET
            using (StreamWriter writer = new StreamWriter("dataset.csv"), ideal = new StreamWriter("ideal.csv"))
            {
                for (int i = 0; i < _input.Length; i++)
                {
                    StringBuilder builder = new StringBuilder();
                    for (int j = 0; j < _input[0].Length; j++)
                    {
                        builder.Append(_input[i][j]);
                        if (j != _input[0].Length - 1)
                            builder.Append(",");
                    }
                    writer.WriteLine(builder.ToString());

                    StringBuilder idealData = new StringBuilder();
                    for (int j = 0; j < _ideal[0].Length; j++)
                    {
                        idealData.Append(_ideal[i][j]);
                        if (j != _ideal[0].Length - 1)
                            idealData.Append(",");
                    }
                    ideal.WriteLine(idealData.ToString());
                }
            }
#endif

        }
        /// <summary>
        /// Train the network using Backpropagation and SimulatedAnnealing methods asynchronously.
        /// </summary>
        /// <param name="trainTo">Train until a specific date</param>
        /// <param name="status">Callback function invoked on each _epoch</param>
        /// <param name="trainFrom">Initial date, from which to gather training data</param>
        public void TrainNetworkAsync(DateTime trainFrom, DateTime trainTo, TrainingStatus status)
        {
            Action<DateTime, DateTime, TrainingStatus> action = TrainNetwork;
            action.BeginInvoke(trainFrom, trainTo, status, action.EndInvoke, action);
        }
        private void TrainNetwork(DateTime trainFrom, DateTime trainTo, TrainingStatus status)
        {
            if (_input == null || _ideal == null)
                CreateTrainingSets(trainFrom, trainTo);         /*Create training sets, according to input parameters*/
            _trainThread = Thread.CurrentThread;
            int epoch = 1;
            ITrain train = null;
            try
            {
                var trainSet = new BasicNeuralDataSet(_input, _ideal);// these are null.
                train = new ResilientPropagation(_network, trainSet);
                double error;
                do
                {
                    train.Iteration();
                    error = train.Error;
                    if (status != null)
                        status.Invoke(epoch, error, TrainingAlgorithm.Resilient);
                    epoch++;
                } while (error > MaxTrainingError);
            }
            catch (ThreadAbortException) {/*Training aborted*/ _trainThread = null; }
            finally
            {
                if (train != null) train.FinishTraining();
                TrainingCompleted = true;
            }
            _trainThread = null;
        }
        /// <summary>
        /// Trains the network.
        /// </summary>
        /// <param name="trainFrom">The train from time.</param>
        /// <param name="trainTo">The train to time.</param>
        /// <param name="algorithm">The type of training algorithm.</param>
        public void TrainNetwork(DateTime trainFrom, DateTime trainTo, TrainingAlgorithm algorithm)
        {
            if (_input == null || _ideal == null)
                CreateTrainingSets(trainFrom, trainTo);// Create training sets, according to input parameters.
            int epoch = 1;
            ITrain train = null;
            try
            {
                var trainSet = new BasicNeuralDataSet(_input, _ideal);
                switch (algorithm)
                {
                    case TrainingAlgorithm.Resilient:
                        train = new ResilientPropagation(_network, trainSet);
                        break;
                    case TrainingAlgorithm.ScaledConjugateGradient:
                        train = new ScaledConjugateGradient(_network, trainSet);
                        break;
                    // Add more as you learn.
                }
                
                do
                {
                    train.Iteration();
                    TrainingError = train.Error;
                    epoch++;
                } while (TrainingError > MaxTrainingError);
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.Accounts, "TrainNetwork(non-async)");
            }
            finally
            {
                if (train != null) train.FinishTraining();
                TrainingCompleted = true;
            }
        }

        /// <summary>
        /// Abort training
        /// </summary>
        public void AbortTraining()
        {
            if (_trainThread != null) _trainThread.Abort();
        }
        /// <summary>
        /// Export neural network
        /// </summary>
        /// <param name="path"></param>
        public void ExportNeuralNetwork(string path)
        {
            if (_network == null)
                throw new NullReferenceException("Network reference is set to null. Nothing to export.");
            SerializeObject.Save(path, _network);
        }
        /// <summary>
        /// Load neural network
        /// </summary>
        /// <param name="path">Path to previously serialized object</param>
        public void LoadNeuralNetwork(string path)
        {
            _network = (BasicNetwork)SerializeObject.Load(path);
            HiddenLayers = _network.Structure.Layers.Count - 2 /*1 input, 1 output*/;
            HiddenUnits = _network.Structure.Layers[1].NeuronCount;
        }
        /// <summary>
        /// Predict the results
        /// </summary>
        /// <returns>List with the prediction results</returns>
        public List<PredictionResults> Predict(DateTime predictFrom, DateTime predictTo)
        {
            var results = new List<PredictionResults>();
            var present = new double[InputTuples * IndexesToConsider];
            var actualOutput = new double[OutputSize];
            var index = 0;
            foreach (var sample in _manager.Samples)
            {
                if (sample.Date.CompareTo(predictFrom) >= 0 && sample.Date.CompareTo(predictTo) < 0)
                {// Set >= from only > since I want to use the last data point inclusive to the prediction which steps away from it.
                    var result = new PredictionResults();
                    _manager.GetInputData(index - InputTuples, present);
                    _manager.GetOutputData(index - InputTuples, actualOutput);
                    var data = new BasicNeuralData(present);
                    var predict = _network.Compute(data);
                    result.ActualLow = actualOutput[0] * (_manager.MaxLow - _manager.MinLow) + _manager.MinLow;
                    result.PredictedLow = predict[0] * (_manager.MaxLow - _manager.MinLow) + _manager.MinLow;
                    result.ActualClose = actualOutput[1] * (_manager.MaxClose - _manager.MinClose) + _manager.MinClose;
                    result.PredictedClose = predict[1] * (_manager.MaxClose - _manager.MinClose) + _manager.MinClose;
                    result.ActualOpen = actualOutput[2] * (_manager.MaxOpen - _manager.MinOpen) + _manager.MinOpen;
                    result.PredictedOpen = predict[2] * (_manager.MaxOpen - _manager.MinOpen) + _manager.MinOpen;
                    result.ActualHigh = actualOutput[3] * (_manager.MaxHigh - _manager.MinHigh) + _manager.MinHigh;
                    result.PredictedHigh = predict[3] * (_manager.MaxHigh - _manager.MinHigh) + _manager.MinHigh;
                    result.Date = sample.Date;
                    var error = new Error();
                    error.UpdateError(actualOutput, predict);
                    result.Error = error.CalculateRms();
                    PredictionError = result.Error;
                    results.Add(result);
                }
                index++;
            }
            PredictionCompleted = true;
            return results;
        }
        public List<PredictionResults> PredictTimeInsensitive(DateTime predictFrom, DateTime predictTo)
        {
            var results = new List<PredictionResults>();
            var present = new double[InputTuples * IndexesToConsider];
            var actualOutput = new double[OutputSize];
            var index = 0;
            foreach (var sample in _manager.Samples)
            {
                if (sample.Date.CompareTo(predictFrom) > 0 && sample.Date.CompareTo(predictTo) < 0)
                {
                    var result = new PredictionResults();
                    _manager.GetInputData(index - InputTuples, present);
                    _manager.GetOutputData(index - InputTuples, actualOutput);
                    var data = new BasicNeuralData(present);
                    var predict = _network.Compute(data);
                    result.ActualLow = actualOutput[0] * (_manager.MaxLow - _manager.MinLow) + _manager.MinLow;
                    result.PredictedLow = predict[0] * (_manager.MaxLow - _manager.MinLow) + _manager.MinLow;
                    result.ActualClose = actualOutput[1] * (_manager.MaxClose - _manager.MinClose) + _manager.MinClose;
                    result.PredictedClose = predict[1] * (_manager.MaxClose - _manager.MinClose) + _manager.MinClose;
                    result.ActualOpen = actualOutput[2] * (_manager.MaxOpen - _manager.MinOpen) + _manager.MinOpen;
                    result.PredictedOpen = predict[2] * (_manager.MaxOpen - _manager.MinOpen) + _manager.MinOpen;
                    result.ActualHigh = actualOutput[3] * (_manager.MaxHigh - _manager.MinHigh) + _manager.MinHigh;
                    result.PredictedHigh = predict[3] * (_manager.MaxHigh - _manager.MinHigh) + _manager.MinHigh;
                    result.Date = sample.Date;
                    //var error = new Error();
                    //error.UpdateError(actualOutput, predict);
                    //result.Error = error.CalculateRms();
                    PredictionError = result.Error;
                    results.Add(result);
                }
                index++;
            }
            PredictionCompleted = true;
            return results;
        }
    }
}
