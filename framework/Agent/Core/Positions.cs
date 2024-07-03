using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Boagaphish;
using Cartheur.Json.Linq;
using SoftAgent.Server;

namespace SoftAgent.Core
{
    public static class Positions
    {
        public static string Url { get; set; }
        public static string CredentialHeader { get; set; }
        public static string[] Side { get; set; }
        public static string[] Instrument { get; set; }
        public static int[] Units { get; set; }
        public static double[] AveragePrice { get; set; }
        public static bool PositionsLoaded { get; set; }
        /// <summary>
        /// Gets all open positions.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns></returns>
        public static bool GetOpenPositions(int accountId)
        {
            try
            {
                switch (MonitoringSession.MonitoringSessionType)
                {
                    case "Practice":
                        CredentialHeader = String.Format(Settings.UrlTokenPractice);
                        Url = UrlPractice.ReturnOpenPositions(accountId);
                        break;
                    case "Live":
                        CredentialHeader = String.Format(Settings.UrlTokenLive);
                        Url = UrlLive.ReturnOpenPositions(accountId);
                        break;
                }
                var request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", CredentialHeader);
                var response = (HttpWebResponse)request.GetResponse();
                var stream = new StreamReader(response.GetResponseStream(), Encoding.ASCII);
                var json = stream.ReadToEnd();
                stream.Close();
                stream.Dispose();
                var openPositions = JObject.Parse(json);
                IList<JToken> positions = openPositions["positions"].Children().ToList();
                var i = 0;
                // Initialize the arrays.
                Side = new string[positions.Count];
                Instrument = new string[positions.Count];
                Units = new int[positions.Count];
                AveragePrice = new double[positions.Count];
                foreach (var position in positions)
                {
                    Side[i] = (string)position["side"];
                    Instrument[i] = (string)position["instrument"];
                    Units[i] = (int)position["units"];
                    AveragePrice[i] = (double)position["avgPrice"];
                    i++;
                }
                PositionsLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(@"Server connection error: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Positions, "GetOpenPositions");
            }
            PositionsLoaded = false;
            return false;
        }
    }

    public static class PositionInformation
    {
        public static string Url { get; set; }
        public static string CredentialHeader { get; set; }
        public static string Side { get; set; }
        public static string Instrument { get; set; }
        public static int Units { get; set; }
        public static double AveragePrice { get; set; }
        public static bool PositionInformationLoaded { get; set; }
        /// <summary>
        /// Retrieves the instrument position.
        /// </summary>
        /// <returns></returns>
        public static bool RetrieveInstrumentPosition(int accountId, string instrument)
        {
            try
            {
                switch (MonitoringSession.MonitoringSessionType)
                {
                    case "Practice":
                        CredentialHeader = String.Format(Settings.UrlTokenPractice);
                        Url = UrlPractice.ReturnInstrumentPosition(accountId, instrument);
                        break;
                    case "Live":
                        CredentialHeader = String.Format(Settings.UrlTokenLive);
                        Url = UrlLive.ReturnInstrumentPosition(accountId, instrument);
                        break;
                }
                var request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", CredentialHeader);
                var response = (HttpWebResponse)request.GetResponse();
                var stream = new StreamReader(response.GetResponseStream(), Encoding.ASCII);
                var json = stream.ReadToEnd();
                stream.Close();
                stream.Dispose();
                var position = JObject.Parse(json);
                Side = (string)position["side"];
                Instrument = (string)position["instrument"];
                Units = int.Parse(position["units"].ToString());
                AveragePrice = double.Parse(position["avgPrice"].ToString());
                PositionInformationLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(@"Server connection error: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Positions, "RetrieveInstrumentPosition");
            }
            PositionInformationLoaded = false;
            return false;
        }
    }
}
