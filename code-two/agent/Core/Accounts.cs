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
    public static class Accounts
    {
        public static string Url { get; set; }
        public static string CredentialHeader { get; set; }
        public static int[] AccountID { get; set; }
        public static string[] AccountName { get; set; }
        public static string[] AccountCurrency { get; set; }
        public static double[] MarginRate { get; set; }
        public static bool AccountsLoaded { get; set; }
        /// <summary>
        /// Retrieves the user accounts.
        /// </summary>
        /// <returns></returns>
        public static bool RetrieveUserAccounts()
        {
            try
            {
                switch (TradingSession.TradingSessionType)
                {
                    case "Practice":
                        CredentialHeader = String.Format(Settings.UrlTokenPractice);
                        Url = UrlPractice.ReturnAccounts();
                        break;
                    case "Live":
                        CredentialHeader = String.Format(Settings.UrlTokenLive);
                        Url = UrlLive.ReturnAccounts();
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
                IList<JToken> accounts = data["accounts"].Children().ToList();
                var i = 0;
                // Initialize the arrays.
                AccountID = new int[accounts.Count];
                AccountName = new string[accounts.Count];
                AccountCurrency = new string[accounts.Count];
                MarginRate = new double[accounts.Count];
                foreach (var account in accounts)
                {
                    AccountID[i] = (int)account["accountId"];
                    AccountName[i] = (string)account["accountName"];
                    AccountCurrency[i] = (string)account["accountCurrency"];
                    MarginRate[i] = (double)account["marginRate"];
                    i++;
                }
                AccountsLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(@"Server connection error: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Accounts, "RetrieveUserAccounts");
            }
            AccountsLoaded = false;
            return false;
        }

    }

    public static class AccountInformation
    {
        public static string Url { get; set; }
        public static string CredentialHeader { get; set; }
        public static int AccountID { get; set; }
        public static string AccountName { get; set; }
        public static string AccountCurrency { get; set; }
        public static double MarginRate { get; set; }
        public static double Balance { get; set; }
        public static double UnrealizedPl { get; set; }
        public static double RealizedPl { get; set; }
        public static double MarginUsed { get; set; }
        public static double MarginAvailable { get; set; }
        public static double OpenTrades { get; set; }
        public static double OpenOrders { get; set; }
        public static bool AccountInformationLoaded { get; set; }
        /// <summary>
        /// Gets the account information.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns></returns>
        public static bool GetAccountInformation(int accountId)
        {
            try
            {
                switch (TradingSession.TradingSessionType)
                {
                    case "Practice":
                        CredentialHeader = String.Format(Settings.UrlTokenPractice);
                        Url = UrlPractice.ReturnAccountInformation(accountId);
                        break;
                    case "Live":
                        CredentialHeader = String.Format(Settings.UrlTokenLive);
                        Url = UrlLive.ReturnAccountInformation(accountId);
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
                var account = JObject.Parse(json);
                AccountID = (int)account["accountId"];
                AccountName = (string)account["accountName"];
                AccountCurrency = (string)account["accountCurrency"];
                MarginRate = double.Parse(account["marginRate"].ToString());
                Balance = double.Parse(account["balance"].ToString());
                UnrealizedPl = double.Parse(account["unrealizedPl"].ToString());
                RealizedPl = double.Parse(account["realizedPl"].ToString());
                MarginUsed = double.Parse(account["marginUsed"].ToString());
                MarginAvailable = double.Parse(account["marginAvail"].ToString());
                OpenOrders = double.Parse(account["openOrders"].ToString());
                OpenTrades = double.Parse(account["openTrades"].ToString());
                AccountInformationLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(@"Server connection error: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Accounts, "GetAccountInformation");
            }
            AccountInformationLoaded = false;
            return false;
        }
    }
}
