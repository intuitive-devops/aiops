using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Boagaphish;
using Boagaphish.Format;
using Cartheur.Json.Linq;
using SoftAgent.Server;

namespace SoftAgent.Core
{
    public static class Rates
    {
        public static string HoldingCurrency { get; set; }
        public static string TradingCurrency { get; set; }

        public static class LiveMetrics
        {
            public static string Url { get; set; }
            public static string CredentialHeader { get; set; }
            // Enumerations for operations in code.
            public enum Trajectory { High, Open, Close, Low, Volume }
            public enum CurrenciesFrom { EUR, GBP, USD, CNY, CAD, JPY, AUD, INR }
            public enum CurrenciesTo { GBP, USD, CNY, EUR, CAD, JPY, AUD, INR }
            // Point arrays from the json formatted for the algorithm.
            public static double[] MarketTimeData { get; set; }
            public static XDate[] MarketXTimeDate { get; set; }
            public static double[] OpenData { get; set; }
            public static double[] CloseData { get; set; }
            public static double[] HighData { get; set; }
            public static double[] LowData { get; set; }
            public static int[] VolumeData { get; set; }
            // Properties for the candle retrieval.
            public static int NumberOfCandles { get; set; }
            public static string CandleFormat { get; set; }
            public static string Granularity { get; set; }
            public static int DailyAlignment { get; set; }
            public static bool RatesLoaded { get; set; }
 
            /// <summary>
            /// Clears the candle data arrays.
            /// </summary>
            /// <returns>True if successful, otherwise false.</returns>
            public static bool ClearCandles()
            {
                try
                {
                    MarketTimeData = new double[0];
                    OpenData = new double[0];
                    CloseData = new double[0];
                    HighData = new double[0];
                    LowData = new double[0];
                    VolumeData = new int[0];
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }
        public static class HistoricalRates
        {
            public enum Trajectory { High, Open, Close, Low, Volume }
            public static string Name { get; set; }
            public static double[] MarketTimeData { get; set; }
            public static double[] OpenData { get; set; }
            public static double[] CloseData { get; set; }
            public static double[] HighData { get; set; }
            public static double[] LowData { get; set; }
            public static int[] VolumeData { get; set; }
            public static int NumberOfCandles { get; set; }
            public static string CandleFormat { get; set; }
            public static string Granularity { get; set; }
            public static int DailyAlignment { get; set; }
            public static int WindowSize { get; set; }
            public static bool RatesLoaded { get; set; }
            /// <summary>
            /// Clears the data arrays.
            /// </summary>
            /// <returns>True if successful, otherwise false.</returns>
            public static bool ClearHistoricalRates()
            {
                try
                {
                    MarketTimeData = new double[0];
                    OpenData = new double[0];
                    CloseData = new double[0];
                    HighData = new double[0];
                    LowData = new double[0];
                    VolumeData = new int[0];
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }
        public static class StoredRates
        {
            public enum Trajectory { High, Open, Close, Low, Volume }
            public static string Name { get; set; }
            public static double[] MarketTimeData { get; set; }
            public static List<double> OpenData { get; set; }
            public static List<double> CloseData { get; set; }
            public static List<double> HighData { get; set; }
            public static List<double> LowData { get; set; }
            public static int[] VolumeData { get; set; }
            public static int NumberOfCandles { get; set; }
            public static string CandleFormat { get; set; }
            public static string Granularity { get; set; }
            public static int DailyAlignment { get; set; }
            public static int WindowSize { get; set; }
            public static bool RatesLoaded { get; set; }
            /// <summary>
            /// Clears the data arrays.
            /// </summary>
            /// <returns>True if successful, otherwise false.</returns>
            public static bool ClearStoredRates()
            {
                try
                {
                    MarketTimeData = new double[0];
                    OpenData = new List<double>();
                    CloseData = new List<double>();
                    HighData = new List<double>();
                    LowData = new List<double>();
                    VolumeData = new int[0];
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }
        public static class PredictedRates
        {
            public enum Trajectory { High, Open, Close, Low, Volume }
            public static string Name { get; set; }
            public static double[] OpenPrediction { get; set; }
            public static double[] ClosePrediction { get; set; }
            public static double[] HighPrediction { get; set; }
            public static double[] LowPrediction { get; set; }
            public static int[] VolumePrediction { get; set; }
            /// <summary>
            /// Clears the data arrays.
            /// </summary>
            /// <returns>True if successful, otherwise false.</returns>
            public static bool ClearPredictedRates()
            {
                try
                {
                    OpenPrediction = new double[0];
                    ClosePrediction = new double[0];
                    HighPrediction = new double[0];
                    LowPrediction = new double[0];
                    VolumePrediction = new int[0];
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }
    }

}
