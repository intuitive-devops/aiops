using System;

namespace SoftAgent.Indices
{
    public class MarketIndices : IComparable<MarketIndices>
    {
        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="dowIndex">Dow index</param>
        /// <param name="nasdaqIndex">Nasdaq index</param>
        /// <param name="snpIndex">SnP index</param>
        /// <param name="pirIndex">Prime interest rate index</param>
        /// <param name="date">Date</param>
        public MarketIndices(double dowIndex, double nasdaqIndex, double snpIndex, double pirIndex, DateTime date)
        {
            DowIndex = dowIndex;
            NasdaqIndex = nasdaqIndex;
            Snp = snpIndex;
            PrimeInterestRate = pirIndex;
            Date = date;
        }
        /// <summary>
        /// Dow Jones index
        /// </summary>
        public double DowIndex { get; set; }
        /// <summary>
        /// NASDAQ index
        /// </summary>
        public double NasdaqIndex { get; set; }
        /// <summary>
        /// The value of Adj Close of SnP500 index
        /// </summary>
        public double Snp { get; set; }
        /// <summary>
        /// Prime interest rate
        /// </summary>
        public double PrimeInterestRate { get; set; }
        /// <summary>
        /// Date with corresponding SnP500 Adj Close and Prime Interest PrimeInterestRate
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Compare by date
        /// </summary>
        /// <param name="other">Other financial pair</param>
        /// <returns></returns>
        public int CompareTo(MarketIndices other)
        {
            return Date.CompareTo(other.Date);
        }
    }
}
