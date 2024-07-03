using System;

namespace SoftAgent.Indices
{
    /// <summary>
    /// SnP 500 index
    /// </summary>
    public class Snp : IComparable<Snp>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Snp"/> class.
        /// </summary>
        /// <param name="amount">SP index</param>
        /// <param name="date">Date of the index</param>
        public Snp(double amount, DateTime date)
        {
            SnpIndex = amount;
            Date = date;
        }
        /// <summary>
        /// SnP500 Index
        /// </summary>
        public double SnpIndex { get; set; }
        /// <summary>
        /// Corresponding date
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Compare the indexes by date
        /// </summary>
        /// <param name="other">Other SnP index</param>
        /// <returns>Comparison result</returns>
        public int CompareTo(Snp other)
        {
            return Date.CompareTo(other.Date);
        }
    }
}
