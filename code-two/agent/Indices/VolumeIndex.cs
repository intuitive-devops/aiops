using System;

namespace SoftAgent.Indices
{
    public class VolumeIndex : IComparable<VolumeIndex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VolumeIndex"/> class.
        /// </summary>
        /// <param name="amount">The amount of the index.</param>
        /// <param name="date">The date of the index.</param>
        public VolumeIndex(int amount, DateTime date)
        {
            Amount = amount;
            Date = date;
        }
        /// <summary>
        /// The amount of the index.
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// The corresponding date of the index.
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Compare indices by date.
        /// </summary>
        /// <param name="other">The other volume index.</param>
        /// <returns>Date comparison result.</returns>
        public int CompareTo(VolumeIndex other)
        {
            return Date.CompareTo(other.Date);
        }
    }
}
