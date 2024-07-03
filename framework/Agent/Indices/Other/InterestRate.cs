using System;

namespace SoftAgent.Indices
{
    /// <summary>
    /// Entity object which represent interest rate value.
    /// </summary>
    public class InterestRate : IComparable<InterestRate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterestRate"/> class.
        /// </summary>
        /// <param name="effectiveDate">Effective date with corresponding interest rate.</param>
        /// <param name="rate">Interest rate value.</param>
        public InterestRate(DateTime effectiveDate, double rate)
        {
            Date = effectiveDate;
            Rate = rate;
        }
        /// <summary>
        /// Effective date.
        /// </summary>
        public DateTime Date {get; set;}
        /// <summary>
        /// Interest rate value.
        /// </summary>
        public double Rate {get; set;}
        /// <summary>
        /// Compare 2 interest rates indexes by date.
        /// </summary>
        /// <param name="other">Other interest rate index.</param>
        /// <returns>Date comparison result.</returns>
        public int CompareTo(InterestRate other)
        {
            return Date.CompareTo(other.Date);
        }
    }
}
