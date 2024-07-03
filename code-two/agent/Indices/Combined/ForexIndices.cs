using System;

namespace SoftAgent.Indices
{
    public class ForexIndices : IComparable<ForexIndices>
    {
        public double OpenIndex { get; set; }
        public double HighIndex { get; set; }
        public double LowIndex { get; set; }
        public double CloseIndex { get; set; }
        //public double VolumeIndex { get; set; }
        public DateTime Date { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ForexIndices"/> class.
        /// </summary>
        /// <param name="open">The open.</param>
        /// <param name="high">The high.</param>
        /// <param name="low">The low.</param>
        /// <param name="close">The close.</param>
        /// <param name="volume">The volume.</param>
        /// <param name="date">The date.</param>
        public ForexIndices(double open, double high, double low, double close, /*double volume,*/ DateTime date)
        {
            OpenIndex = open;
            HighIndex = high;
            LowIndex = low;
            CloseIndex = close;
            //VolumeIndex = volume;
            Date = date;
        }
        /// <summary>
        /// Compare indices by date.
        /// </summary>
        /// <param name="another">Another forex index.</param>
        /// <returns></returns>
        public int CompareTo(ForexIndices another)
        {
            return Date.CompareTo(another.Date);
        }
    }
}
