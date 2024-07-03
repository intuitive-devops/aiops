using System;
using System.Collections.Generic;

namespace SoftAgent.Core
{
    public static class Storage
    {
        public static Dictionary<double, double> IteratedValues { get; set; } 
        // t1, t2, t3, t4, t5.
        public static double TimePoint { get; set; }
        // Forecast and event window values.
        public static double ForecastValue { get; set; }
        public static double EventWindowValue { get; set; }
        // Should we stop and move to notify?
        public static double UnrealizedProfit { get; set; }
        public static double PortfolioValue { get; set; }
        public static double NotifyTakeProfit { get; set; }

        public static bool CreateDictionary()
        {
            IteratedValues = new Dictionary<double, double>
            {
                {TimePoint, QuotientDistance()},
                {TimePoint, QuotientDifference()},
                {TimePoint, UnrealizedProfit},
                {TimePoint, PortfolioValue}
            };
            return true;
        }

        public static double QuotientDistance()
        {
            return Math.Abs(ForecastValue - EventWindowValue);
        }

        public static double QuotientDifference()
        {
            return ForecastValue - EventWindowValue;
        }
    }
}
