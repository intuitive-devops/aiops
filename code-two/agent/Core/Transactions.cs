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
    public static class Transactions
    {
        public enum TransactionTypes { DailyInterest, Fee, LimitOrderCreate, MarginCallEnter, MarginCallExit, MarginCloseout, MarketIfTouchedOrderCreate, MarketOrderCreate, MigrateTradeClosed, MigrateTradeOpen, OrderCancel, OrderFilled, OrderUpdate, SetMarginRate, StopLossFilled, StopOrderCreate, TakeProfitFilled, TradeClose, TradeUpdate, TrailingStopFilled, TransferFunds }

        public static string Url { get; set; }
        public static string CredentialHeader { get; set; }
        public static string[] TransactionID { get; set; }
        public static int[] AccountID { get; set; }
        public static string[] Time { get; set; }
        public static string[] Type { get; set; }
        public static string[] Instrument { get; set; }
        public static string[] Interest { get; set; }
        public static string[] AccountBalance { get; set; }
        public static string Verbose { get; set; }
        public static bool TransactionHistoryLoaded { get; set; }

        public static bool GetTransactionHistory(int accountId, string instrument)
        {
            try
            {
                switch (TradingSession.TradingSessionType)
                {
                    case "Practice":
                        CredentialHeader = String.Format(Settings.UrlTokenPractice);
                        Url = UrlPractice.ReturnGetTransactionHistory(accountId, instrument);
                        break;
                    case "Live":
                        CredentialHeader = String.Format(Settings.UrlTokenLive);
                        Url = UrlLive.ReturnGetTransactionHistory(accountId, instrument);
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
                IList<JToken> transactions = data["transactions"].Children().ToList();
                var i = 0;
                // Return the entire json as string
                Verbose = data.ToString();
                // Initialize the arrays.
                TransactionID = new string[transactions.Count];
                AccountID = new int[transactions.Count];
                Time = new string[transactions.Count];
                Type = new string[transactions.Count];
                Instrument = new string[transactions.Count];
                Interest = new string[transactions.Count];
                AccountBalance = new string[transactions.Count];
                foreach (var transaction in transactions)
                {
                    TransactionID[i] = transaction["id"].ToString();
                    AccountID[i] = int.Parse(transaction["accountId"].ToString());
                    Time[i] = (string)transaction["time"];
                    Type[i] = (string)transaction["type"];
                    Instrument[i] = transaction["instrument"].ToString();
                    Interest[i] = transaction["interest"].ToString();
                    AccountBalance[i] = transaction["accountBalance"].ToString();
                    i++;
                }
                TransactionHistoryLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(@"Server connection error: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Transactions, "GetTransactionHistory");
            }
            TransactionHistoryLoaded = false;
            return false;
        }
        public static bool GetTransactionHistory(int accountId, string instrument, int count)
        {
            try
            {
                switch (TradingSession.TradingSessionType)
                {
                    case "Practice":
                        CredentialHeader = String.Format(Settings.UrlTokenPractice);
                        Url = UrlPractice.ReturnGetTransactionHistory(accountId, instrument, count);
                        break;
                    case "Live":
                        CredentialHeader = String.Format(Settings.UrlTokenLive);
                        Url = UrlLive.ReturnGetTransactionHistory(accountId, instrument, count);
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
                IList<JToken> transactions = data["transactions"].Children().ToList();
                var i = 0;
                // Initialize the arrays.
                TransactionID = new string[transactions.Count];
                AccountID = new int[transactions.Count];
                Time = new string[transactions.Count];
                Type = new string[transactions.Count];
                Instrument = new string[transactions.Count];
                Interest = new string[transactions.Count];
                AccountBalance = new string[transactions.Count];
                foreach (var transaction in transactions)
                {
                    TransactionID[i] = transaction["id"].ToString();
                    AccountID[i] = int.Parse(transaction["accountId"].ToString());
                    Time[i] = (string)transaction["time"];
                    Type[i] = (string)transaction["type"];
                    Instrument[i] = transaction["instrument"].ToString();
                    Interest[i] = (string)transaction["interest"];
                    AccountBalance[i] = (string)transaction["accountBalance"];
                    i++;
                }
                TransactionHistoryLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(@"Server connection error: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Transactions, "GetTransactionHistory(count)");
            }
            TransactionHistoryLoaded = false;
            return false;
        }
    }

    public static class TransactionInformation
    {
        public static string Url { get; set; }
        public static string CredentialHeader { get; set; }
        public static string TransactionID { get; set; }
        public static int AccountID { get; set; }
        public static string Time { get; set; }
        public static string Type { get; set; }
        public static string Instrument { get; set; }
        public static int Units { get; set; }
        public static string Side { get; set; }
        public static double Price { get; set; }
        public static double Pl { get; set; }
        public static double Interest { get; set; }
        public static double AccountBalance { get; set; }
        public static bool TransactionInformationLoaded { get; set; }

        public static bool GetTransactionInformation(int accountId, string transactionId)
        {
            try
            {
                switch (TradingSession.TradingSessionType)
                {
                    case "Practice":
                        CredentialHeader = String.Format(Settings.UrlTokenPractice);
                        Url = UrlPractice.ReturnTransactionInformation(accountId, transactionId);
                        break;
                    case "Live":
                        CredentialHeader = String.Format(Settings.UrlTokenLive);
                        Url = UrlLive.ReturnTransactionInformation(accountId, transactionId);
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
                var transaction = JObject.Parse(json);
                TransactionID = transaction["id"].ToString();
                AccountID = int.Parse(transaction["accountId"].ToString());
                Time = (string)transaction["time"];
                Type = (string)transaction["type"];
                Instrument = (string)transaction["instrument"];
                Units = int.Parse(transaction["units"].ToString());
                Side = (string)transaction["side"];
                Price = double.Parse(transaction["price"].ToString());
                Pl = double.Parse(transaction["pl"].ToString());
                Interest = double.Parse(transaction["interest"].ToString());
                AccountBalance = double.Parse(transaction["accountBalance"].ToString());
                TransactionInformationLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(@"Server connection error: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Transactions, "GetTransactionInformation");
            }
            TransactionInformationLoaded = false;
            return false;
        }
    }
}
