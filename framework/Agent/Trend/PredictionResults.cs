using System;

namespace SoftAgent.Trend
{
    public class PredictionResults
    {
        // NOTE: Going on the assumption that one cat predict volume but it would be meaningless as you cannot read minds.
        /// <summary>
        /// Date of the prediction.
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Actual low value.
        /// </summary>
        public double ActualLow {get; set; }
        /// <summary>
        /// Predicted low value move.
        /// </summary>
        public double PredictedLow {get; set; }
        /// <summary>
        /// Actual open value.
        /// </summary>
        public double ActualOpen { get; set; }
        /// <summary>
        /// Predicted open value move.
        /// </summary>
        public double PredictedOpen { get; set; }
        /// <summary>
        /// Actual high value.
        /// </summary>
        public double ActualHigh { get; set; }
        /// <summary>
        /// Predicted high value move.
        /// </summary>
        public double PredictedHigh { get; set; }
        /// <summary>
        /// Actual close value.
        /// </summary>
        public double ActualClose { get; set; }
        /// <summary>
        /// Predicted close value move.
        /// </summary>
        public double PredictedClose { get; set; }
        /// <summary>
        /// Error between predicted and actual values.
        /// </summary>
        public double Error { get; set; }
    }
}
