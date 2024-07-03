using System;

namespace SoftAgent.Indices
{
    /// <summary>
    /// This is the class which presents output from the algorithm for the future value of an index.
    /// </summary>
    public class FutureIndex : IComparable<FutureIndex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FutureIndex"/> class.
        /// </summary>
        /// <param name="predictionValue">The prediction value of the index.</param>
        /// <param name="date">The date of the index.</param>
        public FutureIndex(double predictionValue, DateTime date)
        {
            PredictionValue = predictionValue;
            Date = date;
        }
        /// <summary>
        /// The prediction value of the index.
        /// </summary>
        public double PredictionValue { get; set; }
        /// <summary>
        /// The corresponding date of the index.
        /// </summary>
        public DateTime Date {get; set;}
        /// <summary>
        /// Compare two indices by date.
        /// </summary>
        /// <param name="other">The other future index.</param>
        /// <returns>Date comparison result.</returns>
        public int CompareTo(FutureIndex other)
        {
            return Date.CompareTo(other.Date);
        }
    }
}
