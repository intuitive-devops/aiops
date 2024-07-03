using System;

namespace SoftAgent.Automat.Forms
{
    public static class TradeElements
    {
        // Paths to the data required for the trade.
        public static string PathToLow = "low.csv";
        public static string PathToClose = "close.csv";
        public static string PathToHigh = "high.csv";
        public static string PathToOpen = "open.csv";
        // Training time-range.
        public static DateTime TrainFrom;
        public static DateTime TrainTo;
        // Learning time-range.
        public static DateTime LearnFrom;
        public static DateTime LearnTo;
        // Prediction time-range.
        public static DateTime PredictFrom;
        public static DateTime PredictTo;
        // Network properties (with defaults).
        public static int HiddenLayers = 2;
        public static int HiddenUnits = 41;

        public static string ReturnFilePath(string file)
        {
            return Environment.CurrentDirectory + @"/data/csv/" + file;
        }
    }
}
