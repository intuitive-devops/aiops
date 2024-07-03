using System;
using System.Collections.Generic;
using System.IO;
using SoftAgent.Core;
using SoftAgent.Indices;
using SoftAgent.Utilities;

namespace SoftAgent.Trend
{
    public sealed class PredictorManager
    {
        private List<OpenIndex> _openIndex = new List<OpenIndex>();
        private List<HighIndex> _highIndex = new List<HighIndex>();
        private List<LowIndex> _lowIndex = new List<LowIndex>();
        private List<CloseIndex> _closeIndex = new List<CloseIndex>();
        private readonly List<ForexIndices> _samples = new List<ForexIndices>();

        private readonly int _inputSize;
        private readonly int _outputSize;
        private const string DateHeader = "date";
        private const string OpenHeader = "open";
        private const string HighHeader = "high";
        private const string LowHeader = "low";
        private const string CloseHeader = "close";

        public double MaxOpen { get; private set; }
        public double MinOpen { get; private set; }
        public double MaxHigh { get; private set; }
        public double MinHigh { get; private set; }
        public double MaxLow { get; private set; }
        public double MinLow { get; private set; }
        public double MaxClose { get; private set; }
        public double MinClose { get; private set; }
        public double MaxVolume { get; private set; }
        public double MinVolume { get; private set; }
        public DateTime MaxDate { get; private set; }
        public DateTime MinDate { get; private set; }

        public IList<ForexIndices> Samples
        {
            get { return _samples; }
        }
        public int Size
        {
            get { return _samples.Count; }
        }

        public PredictorManager(int inputSize, int outputSize)
        {
            if (inputSize <= 0)
                throw new ArgumentException(@"inputSize cannot be less than 0");
            if (outputSize <= 0)
                throw new ArgumentException(@"outputSize cannot be less than 0");
            _inputSize = inputSize;
            _outputSize = outputSize;
            MaxOpen = MaxHigh = MaxClose = MaxLow = Double.MinValue;
            MinOpen = MinHigh = MinClose = MinLow = Double.MaxValue;
            MaxDate = DateTime.MaxValue;
            MinDate = DateTime.MinValue;
            // Intialize storage objects.
            Rates.StoredRates.OpenData = new List<double>();
            Rates.StoredRates.HighData = new List<double>();
            Rates.StoredRates.LowData = new List<double>();
            Rates.StoredRates.CloseData = new List<double>();
        }

        public void GetInputData(int offset, double[] input)
        {
            if (offset == -1)
                offset = 0;// Added to reconcile the -1 from the start index.
            for (int i = 0; i < _inputSize; i++)
            {
                var sample = _samples[offset + i];
                input[i * 4] = sample.LowIndex;
                input[i * 4 + 1] = sample.CloseIndex;
                input[i * 4 + 2] = sample.OpenIndex;
                input[i * 4 + 3] = sample.HighIndex;
            }
        }
        public void GetOutputData(int offset, double[] output)
        {
            var sample = _samples[offset + _inputSize];
            output[0] = sample.LowIndex;
            output[1] = sample.CloseIndex;
            output[2] = sample.OpenIndex;
            output[3] = sample.HighIndex;
        }
        // Get indices
        public double GetLowIndex(DateTime date)
        {
            double currentsp = 0;

            foreach (var item in _lowIndex)
            {
                if (item.Date.CompareTo(date) >= 0)
                {
                    return currentsp;
                }
                currentsp = item.Amount;
            }
            return currentsp;
        }
        public double GetCloseIndex(DateTime date)
        {
            double currentRate = 0;

            foreach (var rate in _closeIndex)
            {
                if (rate.Date.CompareTo(date) >= 0)
                {
                    return currentRate;
                }
                currentRate = rate.Amount;
            }
            return currentRate;
        }
        public double GetOpenIndex(DateTime date)
        {
            double currentAmount = 0;

            foreach (var index in _openIndex)
            {
                if (index.Date.CompareTo(date) >= 0)
                {
                    return currentAmount;
                }
                currentAmount = index.Amount;
            }
            return currentAmount;
        }
        public double GetHighIndex(DateTime date)
        {
            double currentAmount = 0;

            foreach (var index in _highIndex)
            {
                if (index.Date.CompareTo(date) >= 0)
                {
                    return currentAmount;
                }
                currentAmount = index.Amount;
            }
            return currentAmount;
        }
        // Create the lists, stitch, and normalize.
        public void Load(string lowIndices, string closeIndices, string openIndices, string highIndices)
        {
            if (!File.Exists(lowIndices))
                throw new ArgumentException("lowIndexFilename targets an invalid file");
            if (!File.Exists(closeIndices))
                throw new ArgumentException("closeIndexFilename targets an invalid file");
            if (!File.Exists(openIndices))
                throw new ArgumentException("pathToOpen targets an invalid file");
            if (!File.Exists(highIndices))
                throw new ArgumentException("pathToHigh targets an invalid file");
            try
            {
                LoadLowIndices(lowIndices);
            }
            catch
            {
                throw new NotSupportedException("Loading low indices file failed. Not supported file format. Make sure \"date\" and \"low\" column headers are written in the file");
            }
            try
            {
                LoadCloseIndices(closeIndices);
            }
            catch
            {
                throw new NotSupportedException("Loading close indices data file failed. Not supported file format. Make sure \"date\" and \"close\" column headers are written in the file");
            }
            try
            {
                LoadOpenIndices(openIndices);
            }
            catch
            {
                throw new NotSupportedException("Loading open indices file failed. Not supported file format. Make sure \"date\" and \"open\" column headers are written in the file");
            }
            try
            {
                LoadHighIndices(highIndices);
            }
            catch
            {
                throw new NotSupportedException("Loading high indices file failed. Not supported file format. Make sure \"date\" and \"high\" column headers are written in the file");
            }
            MaxDate = MaxDate.Subtract(new TimeSpan(0, 0, _inputSize, 0)); /*Subtract 10 last minutes*/
            StitchFinancialIndexes();
            _samples.Sort();            /*Sort by date*/
            NormalizeData();
        }
        // Load *.csv files via (wait for it) Load!
        public void LoadOpenIndices(string pathToOpen)
        {
            if (_openIndex == null) _openIndex = new List<OpenIndex>();
            else if (_openIndex.Count > 0) _openIndex.Clear();
            using (var csv = new CsvReader(pathToOpen))
            {
                while (csv.Next())
                {
                    DateTime date = csv.GetDate(DateHeader);
                    double amount = csv.GetDouble(OpenHeader);
                    var sample = new OpenIndex(amount, date);
                    _openIndex.Add(sample);
                    Rates.StoredRates.OpenData.Add(sample.Amount);
                    if (amount > MaxOpen) MaxOpen = amount;
                    if (amount < MinOpen) MinOpen = amount;
                }
                csv.Close();
                _openIndex.Sort();
            }
            if (_openIndex.Count > 0)
            {
                if (MinDate < _openIndex[0].Date)                   //after sorting the indexes at 0 position is the lowest date in the range
                    MinDate = _openIndex[0].Date;
                if (MaxDate > _openIndex[_openIndex.Count - 1].Date) //Maximal date
                    MaxDate = _openIndex[_openIndex.Count - 1].Date;
            }
        }
        public void LoadHighIndices(string pathToHigh)
        {
            if (_highIndex == null) _highIndex = new List<HighIndex>();
            else if (_highIndex.Count > 0) _highIndex.Clear();
            using (var csv = new CsvReader(pathToHigh))
            {
                while (csv.Next())
                {
                    DateTime date = csv.GetDate(DateHeader);
                    double amount = csv.GetDouble(HighHeader);
                    var sample = new HighIndex(amount, date);
                    _highIndex.Add(sample);
                    Rates.StoredRates.HighData.Add(sample.Amount);
                    if (amount > MaxHigh) MaxHigh = amount;
                    if (amount < MinHigh) MinHigh = amount;
                }
                csv.Close();
                _highIndex.Sort();
            }
            if (_highIndex.Count > 0)
            {
                if (MinDate < _highIndex[0].Date)                   //after sorting the indexes at 0 position is the lowest date in the range
                    MinDate = _highIndex[0].Date;
                if (MaxDate > _highIndex[_highIndex.Count - 1].Date) //Maximal date
                    MaxDate = _highIndex[_highIndex.Count - 1].Date;
            }
        }
        public void LoadCloseIndices(string pathToClose)
        {
            if (_closeIndex == null) _closeIndex = new List<CloseIndex>();
            else if (_closeIndex.Count > 0) _closeIndex.Clear();
            using (var csv = new CsvReader(pathToClose))
            {
                while (csv.Next())
                {
                    var date = csv.GetDate(DateHeader);
                    var amount = csv.GetDouble(CloseHeader);
                    var sample = new CloseIndex(amount, date);
                    _closeIndex.Add(sample);
                    Rates.StoredRates.CloseData.Add(sample.Amount);
                    if (amount > MaxClose) MaxClose = amount;
                    if (amount < MinClose) MinClose = amount;
                }

                csv.Close();
                _closeIndex.Sort();
            }
            if (_closeIndex.Count > 0)
            {
                if (MinDate < _closeIndex[0].Date)                   //after sorting the indexes at 0 position is the lowest date in the range
                    MinDate = _closeIndex[0].Date;
                /*No need to infer the maximum date as the last PIR value will be used as a reference to current date*/
            }

        }
        public void LoadLowIndices(string pathToLow)
        {
            if (_lowIndex == null) _lowIndex = new List<LowIndex>();
            else if (_lowIndex.Count > 0) _lowIndex.Clear();
            using (var csv = new CsvReader(pathToLow))
            {
                while (csv.Next())
                {
                    var date = csv.GetDate(DateHeader);
                    var amount = csv.GetDouble(LowHeader);
                    var sample = new LowIndex(amount, date);
                    _lowIndex.Add(sample);
                    Rates.StoredRates.LowData.Add(sample.Amount);
                    if (amount > MaxLow) MaxLow = amount;
                    if (amount < MinLow) MinLow = amount;
                }
                csv.Close();
                _lowIndex.Sort();
            }
            if (_lowIndex.Count > 0)
            {
                if (MinDate < _lowIndex[0].Date)                //after sorting the indexes at 0 position is the lowest date in the range
                    MinDate = _lowIndex[0].Date;
                if (MaxDate > _lowIndex[_lowIndex.Count - 1].Date) //Maximal date
                    MaxDate = _lowIndex[_lowIndex.Count - 1].Date;
            }
        }
        // Post-processing.
        public void StitchFinancialIndexes()
        {
            foreach (var item in _lowIndex)
            {
                double close = GetCloseIndex(item.Date);
                double open = GetOpenIndex(item.Date);
                double high = GetHighIndex(item.Date);
                double low = GetLowIndex(item.Date);
                _samples.Add(new ForexIndices(open, high, low, close, item.Date));
            }
        }
        public void NormalizeData()
        {
            foreach (var t in _samples)
            {
                t.OpenIndex = (t.OpenIndex - MinOpen) / (MaxOpen - MinOpen);
                t.HighIndex = (t.HighIndex - MinHigh) / (MaxHigh - MinHigh);
                t.CloseIndex = (t.CloseIndex - MinClose) / (MaxClose - MinClose);
                t.LowIndex = (t.LowIndex - MinLow) / (MaxLow - MinLow);
            }
        }
    }
}
