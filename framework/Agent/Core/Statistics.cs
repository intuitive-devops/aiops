using Boagaphish;
using Cartheur.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Xml;

namespace SoftAgent.Core
{
    /// <summary>
    /// Class representing rates from a metric server such as Prometheus.
    /// </summary>
    public static class Statistics
    {
        public static string XmsFileName { get; set; }
        public static string PathForStatisticData
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory + @"\data\xms\", XmsFileName);
            }
        }
        public static string CredentialHeader { get; set; }
        // Enumerations for operations in code.
        public enum Trajectory { CpuPercentage, MemoryUsage, MemoryLimit, MemoryPercentage, NetworkPercentage }
        public static double[] SequenceNumberData { get; set; }
        public static double[] CpuPercentageData { get; set; }
        public static double[] MemoryUsageData { get; set; }
        public static string[] MemoryUsageUnitData { get; set; }
        public static double[] MemoryLimitData { get; set; }
        public static string[] MemoryLimitUnitData { get; set; }
        public static double[] MemoryPercentageData { get; set; }
        public static int[] NetworkBandwidthData { get; set; }
        // Properties for the data retrieval.
        public static string ContainerID { get; set; }
        public static string ContainerName { get; set; }
        public static int NumberOfPoints { get; set; }
        public static int SequenceNumber { get; set; }
        public static string CpuPercentage { get; set; }
        public static string MemoryUsage { get; set; }
        public static string MemoryUsageUnit { get; set; }
        public static string MemoryLimit { get; set; }
        public static string MemoryLimitUnit { get; set; }
        public static string MemoryPercentage { get; set; }
        public static string NetworkBandwidth { get; set; }
        public static bool MetricLoaded { get; set; }
        public static bool ArraysCreated { get; set; }
        public static DateTime TimeStamp { get; set; }
        static int MetricNumber { get; set; }
        public static List<string> Metrics { get; set; }

        /// <summary>
        /// Retrieves the metric data from the container.
        /// </summary>
        /// <param name="numberOfPoints">The number of points in the dataset.</param>
        /// <param name="cpuPercentage">The cpu percentage.</param>
        /// <param name="memoryUsage">The memory usage.</param>
        /// <param name="memoryLimit">The memory limit.</param>
        /// <param name="memoryPercentage">The memory percentage.</param>
        /// <param name="networkBandwidth">The network percentage.</param>
        public static bool ProcessMetrics(int sequenceNumber, string numberOfPoints, string cpuPercentage, string memoryUsage, string memoryLimit, string memoryPercentage, string networkBandwidth)
        {
            SequenceNumber = sequenceNumber;
            NumberOfPoints = Int32.Parse(numberOfPoints);
            CpuPercentage = cpuPercentage.GetNumbers();
            MemoryUsage = memoryUsage;
            if (memoryUsage.IsMega())
            {
                MemoryUsageUnit = "M";
            }
            if (memoryUsage.IsGiga())
            {
                MemoryUsageUnit = "G";
            }
            MemoryLimit = memoryLimit;
            if (memoryLimit.IsMega())
            {
                MemoryLimitUnit = "M";
            }
            if (memoryLimit.IsGiga())
            {
                MemoryLimitUnit = "G";
            }
            MemoryPercentage = memoryPercentage;
            NetworkBandwidth = networkBandwidth;

            if(!ArraysCreated)
            {
                SequenceNumberData = new double[NumberOfPoints];
                CpuPercentageData = new double[NumberOfPoints];
                MemoryUsageData = new double[NumberOfPoints];
                MemoryUsageUnitData = new string[NumberOfPoints];
                MemoryLimitData = new double[NumberOfPoints];
                MemoryLimitUnitData = new string[NumberOfPoints];
                MemoryPercentageData = new double[NumberOfPoints];
                NetworkBandwidthData = new int[NumberOfPoints];
                ArraysCreated = true;
            }
            try
            {
                SequenceNumberData[SequenceNumber] = SequenceNumber;
                CpuPercentageData[SequenceNumber] = double.Parse(CpuPercentage.GetNumbers());
                MemoryUsageData[SequenceNumber] = double.Parse(MemoryUsage.GetNumbers());
                MemoryUsageUnitData[SequenceNumber] = MemoryUsageUnit;
                MemoryLimitData[SequenceNumber] = double.Parse(MemoryLimit.GetNumbers());
                MemoryLimitUnitData[SequenceNumber] = MemoryLimitUnit;
                MemoryPercentageData[SequenceNumber] = double.Parse(MemoryPercentage.GetNumbers());
                NetworkBandwidthData[SequenceNumber] = int.Parse(NetworkBandwidth.GetNumbers());
                MetricLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.AgentCore);
            }

            return false;
        }
        public static void ParseStatistic(string result, bool storage)
        {
            var split = result.Split(' ');
            ContainerID = split[0];
            ContainerName = split[3];
            CpuPercentage = split[8];
            MemoryUsage = split[13];
            MemoryLimit = split[15];
            MemoryPercentage = split[18];
            NetworkBandwidth = split[23];
            if (!storage)
            {
                // Store as arrays.
                Statistics.ProcessMetrics(SequenceNumber, NumberOfPoints.ToString(), CpuPercentage, MemoryUsage, MemoryLimit, MemoryPercentage, NetworkBandwidth);
                Statistics.TimeStamp = DateTime.Now;
                Logging.WriteLog("Processed sequence: " + SequenceNumber + " of " + NumberOfPoints, Logging.LogType.Information, Logging.LogCaller.Statistics);
            }
            if (storage)
            {
                // Save as xml files. What is the periodicity?
            }
            SequenceNumber++; ;
        }
        public static void SaveStatisticData()
        {
            if (PathForStatisticData == null)
                return;
            var writer = XmlWriter.Create(PathForStatisticData);
            // Data properties.
            writer.WriteStartElement("StatisticData");
            writer.WriteAttributeString("Source", "Demo");
            writer.WriteAttributeString("NumberOfPoints", Statistics.NumberOfPoints.ToString());
            //writer.WriteAttributeString("CandleFormat", Rates.LiveMetrics.CandleFormat);
            // Data elements.
            for (var i = 0; i < Statistics.NumberOfPoints; i++)
            {
                writer.WriteStartElement("Data");
                writer.WriteAttributeString("Index", i.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("StatisticTime", Statistics.TimeStamp.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("CpuPercentage", Statistics.CpuPercentageData[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("MemoryLimit", Statistics.MemoryLimitData[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("MemoryPercentage", Statistics.MemoryPercentageData[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("MemoryUsage", Statistics.MemoryUsageData[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("NetworkBandwidth", Statistics.NetworkBandwidthData[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
        }
        /// <summary>
        /// Clears the candle data arrays.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        public static bool ClearStatistics()
        {
            try
            {
                SequenceNumberData = new double[0];
                CpuPercentageData = new double[0];
                MemoryUsageData = new double[0];
                MemoryLimitData = new double[0];
                MemoryPercentageData = new double[0];
                NetworkBandwidthData = new int[0];
            }
            catch
            {
                return false;
            }

            return true;
        }
    }

    public static class HistoricalStatistics
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
    public static class StoredStatistics
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
    public static class PredictedStatistics
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
