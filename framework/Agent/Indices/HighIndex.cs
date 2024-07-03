using System;

namespace SoftAgent.Indices
{
    public class HighIndex : IComparable<HighIndex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighIndex"/> class.
        /// </summary>
        /// <param name="amount">The amount of the index.</param>
        /// <param name="date">The date of the index.</param>
        public HighIndex(double amount, DateTime date)
        {
            Amount = amount;
            Date = date;
        }
        /// <summary>
        /// The amount of the index.
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// The corresponding date of the index.
        /// </summary>
        public DateTime Date {get; set;}
        /// <summary>
        /// Compare two indices by date.
        /// </summary>
        /// <param name="other">The other high index.</param>
        /// <returns>Date comparison result.</returns>
        public int CompareTo(HighIndex other)
        {
            return Date.CompareTo(other.Date);
        }
    }
}
