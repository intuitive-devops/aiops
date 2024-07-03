using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Boagaphish;
using Newtonsoft.Json.Linq;
using SoftAgent.Server;

namespace SoftAgent.Core
{
    #region Trade notes
    // "stopLoss=1.6" "takeProfit=1.7" "trailingStop=50" "https://api-fxtrade.oanda.com/v1/accounts/12345/trades/43211" | SEE: http://developer.oanda.com/rest-live/trades/#getInformationSpecificTrade
    #endregion

    public static class Trades
    {
        public static string Url { get; set; }
        public static string CredentialHeader { get; set; }
        public static string[] TradeID { get; set; }
        public static int[] Units { get; set; }
        public static string[] Side { get; set; }
        public static string[] Instrument { get; set; }
        public static string[] Time { get; set; }
        public static double[] Price { get; set; }
        public static double[] TakeProfit { get; set; }
        public static double[] StopLoss { get; set; }
        public static int[] TrailingStop { get; set; }
        public static double[] TrailingAmount { get; set; }
        public static bool TradesLoaded { get; set; }
        public static string Verbose { get; set; }

        public static bool GetOpenTrades(int accountId, string instrument)
        {
            try
            {
                switch (TradingSession.TradingSessionType)
                {
                    case "Practice":
                        CredentialHeader = String.Format(Settings.UrlTokenPractice);
                        Url = UrlPractice.ReturnOpenTrades(accountId, instrument);
                        break;
                    case "Live":
                        CredentialHeader = String.Format(Settings.UrlTokenLive);
                        Url = UrlLive.ReturnOpenTrades(accountId, instrument);
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
                var data = JObject.Parse(json);
                IList<JToken> trades = data["trades"].Children().ToList();
                int i = 0;
                // Initialize the arrays.
                TradeID = new string[trades.Count];
                Units = new int[trades.Count];
                Side = new string[trades.Count];
                Instrument = new string[trades.Count];
                Time = new string[trades.Count];
                Price = new double[trades.Count];
                TakeProfit = new double[trades.Count];
                StopLoss = new double[trades.Count];
                TrailingStop = new int[trades.Count];
                TrailingAmount = new double[trades.Count];
                foreach (var trade in trades)
                {
                    TradeID[i] = trade["id"].ToString();
                    Units[i] = int.Parse(trade["units"].ToString());
                    Side[i] = (string)trade["side"];
                    Instrument[i] = (string)trade["instrument"];
                    Time[i] = (string)trade["time"];
                    Price[i] = double.Parse(trade["price"].ToString());
                    TakeProfit[i] = int.Parse(trade["takeProfit"].ToString());
                    StopLoss[i] = int.Parse(trade["stopLoss"].ToString());
                    TrailingStop[i] = int.Parse(trade["trailingStop"].ToString());
                    TrailingAmount[i] = double.Parse(trade["trailingAmount"].ToString());
                    i++;
                }
                TradesLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(@"Server connection error: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Trades, "GetOpenTrades");
            }
            TradesLoaded = false;
            return false;
        }
        public static bool GetOpenTrades(int accountId, string instrument, int count)
        {
            try
            {
                switch (TradingSession.TradingSessionType)
                {
                    case "Practice":
                        CredentialHeader = String.Format(Settings.UrlTokenPractice);
                        Url = UrlPractice.ReturnOpenTrades(accountId, instrument, count);
                        break;
                    case "Live":
                        CredentialHeader = String.Format(Settings.UrlTokenLive);
                        Url = UrlLive.ReturnOpenTrades(accountId, instrument, count);
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
                var data = JObject.Parse(json);
                IList<JToken> trades = data["trades"].Children().ToList();
                var i = 0;
                // Initialize the arrays.
                TradeID = new string[trades.Count];
                Units = new int[trades.Count];
                Side = new string[trades.Count];
                Instrument = new string[trades.Count];
                Time = new string[trades.Count];
                Price = new double[trades.Count];
                TakeProfit = new double[trades.Count];
                StopLoss = new double[trades.Count];
                TrailingStop = new int[trades.Count];
                TrailingAmount = new double[trades.Count];
                foreach (var trade in trades)
                {
                    TradeID[i] = trade["id"].ToString();
                    Units[i] = int.Parse(trade["units"].ToString());
                    Side[i] = (string)trade["side"];
                    Instrument[i] = (string)trade["instrument"];
                    Time[i] = (string)trade["time"];
                    Price[i] = double.Parse(trade["price"].ToString());
                    TakeProfit[i] = int.Parse(trade["takeProfit"].ToString());
                    StopLoss[i] = int.Parse(trade["stopLoss"].ToString());
                    TrailingStop[i] = int.Parse(trade["trailingStop"].ToString());
                    TrailingAmount[i] = double.Parse(trade["trailingAmount"].ToString());
                    i++;
                }
                TradesLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(@"Server connection error: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Trades, "GetOpenTrades(count)");
            }
            TradesLoaded = false;
            return false;
        }
        public static bool DeleteTrade(int accountId, string tradeId)
        {
            try
            {
                switch (TradingSession.TradingSessionType)
                {
                    case "Practice":
                        CredentialHeader = String.Format(Settings.UrlTokenPractice);
                        Url = UrlPractice.DeleteTrade(accountId, tradeId);
                        break;
                    case "Live":
                        CredentialHeader = String.Format(Settings.UrlTokenLive);
                        Url = UrlLive.DeleteTrade(accountId, tradeId);
                        break;
                }
                var request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "DELETE";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", CredentialHeader);
                var response = (HttpWebResponse)request.GetResponse();
                var stream = new StreamReader(response.GetResponseStream(), Encoding.ASCII);
                var json = stream.ReadToEnd();
                stream.Close();
                stream.Dispose();
                var data = JObject.Parse(json);
                // Return the entire json as string
                Verbose = data.ToString();
                return true;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(@"Server connection error: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Trades, "DeleteTrade");
            }
            return false;
        }
    }

    public static class TradeInformation
    {
        public static string Url { get; set; }
        public static string CredentialHeader { get; set; }
        public static string TradeID { get; set; }
        public static int Units { get; set; }
        public static string Side { get; set; }
        public static string Instrument { get; set; }
        public static string Time { get; set; }
        public static double Price { get; set; }
        public static double TakeProfit { get; set; }
        public static double StopLoss { get; set; }
        public static int TrailingStop { get; set; }
        public static double TrailingAmount { get; set; }
        public static bool TradeInformationLoaded { get; set; }

        public static bool GetTradeInformation(int accountId, string tradeId)
        {
            try
            {
                switch (TradingSession.TradingSessionType)
                {
                    case "Practice":
                        CredentialHeader = String.Format(Settings.UrlTokenPractice);
                        Url = UrlPractice.ReturnTradeInformation(accountId, tradeId);
                        break;
                    case "Live":
                        CredentialHeader = String.Format(Settings.UrlTokenLive);
                        Url = UrlLive.ReturnTradeInformation(accountId, tradeId);
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
                var trade = JObject.Parse(json);
                TradeID = trade["id"].ToString();
                Units = int.Parse(trade["units"].ToString());
                Side = (string)trade["side"];
                Instrument = (string)trade["instrument"];
                Time = (string)trade["time"];
                Price = double.Parse(trade["price"].ToString());
                Side = (string)trade["side"];
                Price = double.Parse(trade["price"].ToString());
                TakeProfit = double.Parse(trade["takeProfit"].ToString());
                StopLoss = double.Parse(trade["stopLoss"].ToString());
                TrailingStop = int.Parse(trade["trailingStop"].ToString());
                TrailingAmount = int.Parse(trade["trailingStop"].ToString());
                TradeInformationLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(@"Server connection error: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Trades, "GetTradeInformation");
            }
            TradeInformationLoaded = false;
            return false;
        }
    }

    public static class TradingSession
    {
        public enum TradingSessionAccount { Practice = 475324, Live = 6825462 }
        public static string TradingSessionType { get; set; }
    }
}
