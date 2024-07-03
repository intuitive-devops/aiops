using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Boagaphish.Core;

namespace SoftAgent.Core
{
    /// <summary>
    /// Collect all data before sending to the area where a decision is contemplated.
    /// </summary>
    public class Decision
    {
        public double DifferenceThreshold { get; set; } // The threshold which determines if the transaction is worth acting on.
        public enum DataStreamType { Solution, Raw }
        public static bool Buy { get; set; }
        public static bool Sell { get; set; }
        public static bool Keep { get; set; }
        public static bool Cheese { get; set; }
        public static bool Authorization { get; set; }
        public static double[] Inputs { get; set; }
        public static IList<bool> RawDataTruthTable { get; set; }
        public static IList<bool> SolutionTruthTable { get; set; }
        public List<string> TruthTableStrings { get; set; }
        public static int TrueCount { get; set; }
        public static int TrueInSequence { get; set; }
        public static int FalseCount { get; set; }
        public static int FalseInSequence { get; set; }
        static bool PositiveTrend { get; set; }
        static bool NegativeTrend { get; set; }
        public static bool UpwardForecastValue { get; set; }
        public static bool DownwardForecastValue { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Decision"/> class.
        /// </summary>
        /// <param name="trend">The trend.</param>
        /// <param name="dataType">Type of the data.</param>
        public Decision(List<bool> trend, DataStreamType dataType)
        {
            MakeDecision(trend, dataType);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Decision"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public Decision(double[] data)
        {
            RenderTrend(data);
        }
        /// <summary>
        /// Make a decision based on the forecast value and the minimum difference magnitude between it and the last solution value.
        /// </summary>
        /// <param name="forecastValue">The forecast value.</param>
        /// <param name="differenceMagnitude">The difference magnitude.</param>
        /// <returns></returns>
        public static string MakeDecision(double forecastValue, double differenceMagnitude)
        {
            const string output = "null";
            UpwardForecastValue = forecastValue.IsGreaterThanPreviousByMagnitude(differenceMagnitude);
            DownwardForecastValue = forecastValue.IsLessThanPreviousByMagnitude(differenceMagnitude);
            if (UpwardForecastValue)
                return "Buy";
            return DownwardForecastValue ? "Sell" : output;
        }
        public static string MakeDecision(List<bool> trend, DataStreamType dataType)
        {
            string output = "null";
            // Passing in two lists: one from solution and one from raw data.
            switch (dataType) // Does the solution suggest an upward or downward trend?
            {
                    case DataStreamType.Raw:
                {
                    RawDataTruthTable = trend;
                    break;
                }
                    case DataStreamType.Solution:
                {
                    SolutionTruthTable = trend;
                    break;
                }
            }
            // Does the solution suggest an upward or downward trend?
            TrueCount = trend.Count(c => c);
            TrueInSequence = trend.MaximumTrueInSequence();
            FalseCount = trend.Count(c => !c);
            FalseInSequence = trend.MaximumFalseInSequence();
            // Is the information indicative of a market response?
            if (FalseInSequence >= 2)
            {
                NegativeTrend = true;
            }
            if (TrueInSequence >= 2)
            {
                PositiveTrend = true;
            }
            if (PositiveTrend)
            {
                Buy = true;
                output = "Buy";
            }
            if (NegativeTrend)
            {
                Sell = true;
                output = "Sell";
            }
            if (!PositiveTrend && !NegativeTrend)
            {
                Cheese = true;
                output = "No trend";
            }
            // Reset the flags.
            NegativeTrend = false;
            PositiveTrend = false;
            return output;
        }
        /// <summary>
        /// Renders the trend.
        /// </summary>
        /// <param name="dataPoints">The data points.</param>
        /// <returns></returns>
        public IList<bool> RenderTrend(double[] dataPoints)
        {
            Inputs = new double[dataPoints.Length];
            Array.Copy(dataPoints, Inputs, dataPoints.Length);
            RawDataTruthTable = new List<bool>();
            // Build a parseable list.
            for (var i = 0; i < dataPoints.Length - 1; i++)
            {
                if (Inputs[i] < dataPoints[i + 1])
                {
                    RawDataTruthTable.Add(true);// An upward trend.
                }
                if (Inputs[i] > dataPoints[i + 1])
                {
                    RawDataTruthTable.Add(false);// A downward trend.
                }
            }
            return RawDataTruthTable;

        }
        /// <summary>
        /// Renders the trend strings.
        /// </summary>
        /// <param name="dataPoints">The data points.</param>
        /// <returns></returns>
        public List<string> RenderTrendStrings(double[] dataPoints)
        {
            Inputs = new double[dataPoints.Length];
            Array.Copy(dataPoints, Inputs, dataPoints.Length);
            TruthTableStrings = new List<string>();
            // Build a parseable list.
            for (var i = 0; i < dataPoints.Length - 1; i++)
            {
                if (Inputs[i] < dataPoints[i + 1])
                {
                    TruthTableStrings.Add("Point " + i + ": Trend upward.");
                }
                if (Inputs[i] > dataPoints[i + 1])
                {
                    TruthTableStrings.Add("Point " + i + ": Trend downward.");
                }
            }
            return TruthTableStrings;

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Decision"/> class.
        /// </summary>
        /// <param name="realLastValue">The real last value.</param>
        /// <param name="predictedValue">The predicted value.</param>
        /// <param name="differenceThreshold">The difference threshold. Default is 0.02 in the standard currency (EUR).</param>
        public Decision(double realLastValue, double predictedValue, double differenceThreshold = 0.02)
        {
            DifferenceThreshold = differenceThreshold;
            double difference = 0.0;
            double difference2 = 0.0;
            var comparison = predictedValue.IsGreaterThan(realLastValue);
            if (comparison)
                difference = predictedValue - realLastValue;
            if (difference > DifferenceThreshold)
                Buy = true;
            else
                Cheese = true;
            if (!comparison)
            {
                var comparison2 = predictedValue.IsLessThan(realLastValue);
                if (comparison2)
                    difference2 = realLastValue - predictedValue;
                if (difference2 > DifferenceThreshold)
                    Sell = true;
                else
                    Cheese = true;
            } 
        }
    }
}
