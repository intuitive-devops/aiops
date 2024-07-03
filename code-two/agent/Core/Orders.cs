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
    public static class Orders
    {
        public enum OrderType { limit, stop, marketIfTouched, market }
        public enum OrderSide { buy, sell }
        public static string Url { get; set; }
        public static string CredentialHeader { get; set; }
        public static string[] OrderID { get; set; }
        public static string[] Instrument { get; set; }
        public static int[] Units { get; set; }
        public static string[] Side { get; set; }
        public static string[] Type { get; set; }
        public static string[] Time { get; set; }
        public static double[] Price { get; set; }
        public static double[] TakeProfit { get; set; }
        public static double[] StopLoss { get; set; }
        public static string[] Expiry { get; set; }
        public static double[] UpperBound { get; set; }
        public static double[] LowerBound { get; set; }
        public static int[] TrailingStop { get; set; }
        public static bool OrdersLoaded { get; set; }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="instrument">The instrument.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static bool GetOrders(int accountId, string instrument, int count)
        {
            try
            {
                switch (TradingSession.TradingSessionType)
                {
                    case "Practice":
                        CredentialHeader = String.Format(Settings.UrlTokenPractice);
                        Url = UrlPractice.ReturnOrders(accountId, instrument, count);
                        break;
                    case "Live":
                        CredentialHeader = String.Format(Settings.UrlTokenLive);
                        Url = UrlLive.ReturnOrders(accountId, instrument, count);
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
                IList<JToken> orders = data["orders"].Children().ToList();
                var i = 0;
                // Initialize the arrays.
                OrderID = new string[orders.Count];
                Instrument = new string[orders.Count];
                Units = new int[orders.Count];
                Side = new string[orders.Count];
                Type = new string[orders.Count];
                Time = new string[orders.Count];
                Price = new double[orders.Count];
                TakeProfit = new double[orders.Count];
                StopLoss = new double[orders.Count];
                Expiry = new string[orders.Count];
                UpperBound = new double[orders.Count];
                LowerBound = new double[orders.Count];
                TrailingStop = new int[orders.Count];
                
                foreach (var order in orders)
                {
                    OrderID[i] = order["id"].ToString();
                    Instrument[i] = (string)order["instrument"];
                    Units[i] = int.Parse(order["units"].ToString());
                    Side[i] = (string)order["side"];
                    Type[i] = (string)order["type"];
                    Time[i] = (string)order["time"];
                    Price[i] = double.Parse(order["price"].ToString());
                    TakeProfit[i] = int.Parse(order["takeProfit"].ToString());
                    StopLoss[i] = int.Parse(order["stopLoss"].ToString());
                    Expiry[i] = (string)order["expiry"];
                    UpperBound[i] = double.Parse(order["upperBound"].ToString());
                    LowerBound[i] = double.Parse(order["lowerBound"].ToString());
                    TrailingStop[i] = int.Parse(order["trailingStop"].ToString());
                    i++;
                }
                OrdersLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(@"Server connection error: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Orders, "GetOrders");
            }
            OrdersLoaded = false;
            return false;
        }
        /// <summary>
        /// Posts the market order.
        /// </summary>
        /// <param name="accoundId">The accound identifier.</param>
        /// <param name="instrument">The instrument.</param>
        /// <param name="units">The units.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        public static string PostMarketOrder(int accoundId, string instrument, int units, string side)
        {
            try
            {
                switch (TradingSession.TradingSessionType)
                {
                    case "Practice":
                        CredentialHeader = String.Format(Settings.UrlTokenPractice);
                        Url = UrlPractice.PlaceMarketOrder(accoundId, instrument, units, side);
                        break;
                    case "Live":
                        CredentialHeader = String.Format(Settings.UrlTokenLive);
                        Url = UrlLive.PlaceMarketOrder(accoundId, instrument, units, side);
                        break;
                }
                // Create the post data stream.
                var postData = "instrument=" + instrument + "&units=" + units + "&side=" + side + "&type=market";
                var request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                var data = Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Authorization", CredentialHeader);
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                using (var response = request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseString = reader.ReadToEnd().Trim();
                        Logging.WriteLog(responseString, Logging.LogType.Information, Logging.LogCaller.Orders, "PostMarketOrder");
                        return responseString;
                    } 
                }
                
            }
            catch (Exception ex)
            {
                Logging.WriteLog(@"Server connection error: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Orders, "PostMarketOrder");
            }
            return "Response error";
        }
        /// <summary>
        /// Posts the order.
        /// </summary>
        /// <param name="instrument">The instrument.</param>
        /// <param name="units">The units.</param>
        /// <param name="side">The side.</param>
        /// <param name="type">The type.</param>
        /// <param name="expiry">The expiry.</param>
        /// <param name="price">The price.</param>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="stopLoss">The stop loss.</param>
        /// <param name="takeProfit">The take profit.</param>
        /// <param name="trailingStop">The trailing stop.</param>
        /// <returns></returns>
        public static bool PostOrder(string instrument, int units, string side, string type, string expiry, string price, string lowerBound, string upperBound, string stopLoss, string takeProfit, string trailingStop)
        {
            try
            {
                switch (TradingSession.TradingSessionType)
                {
                    case "Practice":
                        CredentialHeader = String.Format(Settings.UrlTokenPractice);
                        //Url = UrlPractice.ReturnOrders(accountId, instrument, count);
                        break;
                    case "Live":
                        CredentialHeader = String.Format(Settings.UrlTokenLive);
                        //Url = UrlLive.ReturnOrders(accountId, instrument, count);
                        break;
                }
                //var request = (HttpWebRequest)WebRequest.Create(Url);
                //request.Method = "POST";
                //request.ContentType = "application/json";
                //request.Headers.Add("Authorization", CredentialHeader);
                //var response = (HttpWebResponse)request.GetResponse();
                //var stream = new StreamReader(response.GetResponseStream(), Encoding.ASCII);
                //var json = stream.ReadToEnd();
                //stream.Close();
                //stream.Dispose();
                //Logging.WriteLog(responseString, Logging.LogType.Information, Logging.LogCaller.Orders, "PostOrder");
            }
            catch (Exception ex)
            {
                Logging.WriteLog(@"Server connection error: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.Orders, "PostOrder");
            }
            return false;
        }
    }

    public static class OrderInformation
    {
        public static string Url { get; set; }
        public static string CredentialHeader { get; set; }
        public static int OrderID { get; set; }
        public static string Instrument { get; set; }
        public static int Units { get; set; }
        public static string Side { get; set; }
        public static string Type { get; set; }
        public static string Time { get; set; }
        public static double Price { get; set; }
        public static double TakeProfit { get; set; }
        public static double StopLoss { get; set; }
        public static string Expiry { get; set; }
        public static double UpperBound { get; set; }
        public static double LowerBound { get; set; }
        public static int TrailingStop { get; set; }
        public static bool OrderInformationLoaded { get; set; }
        /// <summary>
        /// Gets the order information.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        public static bool GetOrderInformation(int accountId, int orderId)
        {
            try
            {
                switch (TradingSession.TradingSessionType)
                {
                    case "Practice":
                        CredentialHeader = String.Format(Settings.UrlTokenPractice);
                        Url = UrlPractice.ReturnOrderInformation(accountId, orderId);
                        break;
                    case "Live":
                        CredentialHeader = String.Format(Settings.UrlTokenLive);
                        Url = UrlLive.ReturnOrderInformation(accountId, orderId);
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
                var order = JObject.Parse(json);
                OrderID = int.Parse(order["id"].ToString());
                Instrument = (string)order["instrument"];
                Units = int.Parse(order["units"].ToString());
                Side = (string)order["side"];
                Type = (string)order["type"];
                Time = (string)order["time"];
                Price = double.Parse(order["price"].ToString());
                TakeProfit = double.Parse(order["takeProfit"].ToString());
                StopLoss = double.Parse(order["stopLoss"].ToString());
                Expiry = (string)order["expiry"];
                UpperBound = double.Parse(order["upperBound"].ToString());
                LowerBound = double.Parse(order["lowerBound"].ToString());
                TrailingStop = int.Parse(order["trailingStop"].ToString());
                OrderInformationLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(@"Server connection error: " + ex.Message, Logging.LogType.Error, Logging.LogCaller.OrderInformation, "GetOrderInformation");
            }
            OrderInformationLoaded = false;
            return false;
        }
    }
}
