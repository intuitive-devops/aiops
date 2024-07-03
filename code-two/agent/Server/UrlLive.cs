namespace SoftAgent.Server
{
    public class UrlLive
    {
        public static string Pair { get; set; }
        private readonly string _url;

        public override string ToString()
        {
            return _url + Pair;
        }

        /// <summary>
        /// Returns the a set of candles by query.
        /// </summary>
        /// <param name="instrument">The instrument.</param>
        /// <param name="count">The number of points.</param>
        /// <param name="candleFormat">The candle format.</param>
        /// <param name="granularity">The granularity.</param>
        /// <param name="dailyAlignment">The daily alignment.</param>
        /// <param name="alignmentTimezone">The alignment timezone.</param>
        /// <returns></returns>
        public static string ReturnRates(string instrument, string count, string candleFormat, string granularity,
            string dailyAlignment, string alignmentTimezone)
        {
            return "https://api-fxtrade.oanda.com/v1/candles?instrument=" + instrument + "&count=" + count + "&candleFormat=" + candleFormat + "&granularity=" + granularity + "&dailyAlignment=" + dailyAlignment + "&alignmentTimezone=" + alignmentTimezone;
        }
        /// <summary>
        /// Returns the accounts.
        /// </summary>
        /// <returns></returns>
        public static string ReturnAccounts()
        {
            return "https://api-fxtrade.oanda.com/v1/accounts";
        }
        /// <summary>
        /// Returns the account information.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns></returns>
        public static string ReturnAccountInformation(int accountId)
        {
            return "https://api-fxtrade.oanda.com/v1/accounts/" + accountId;
        }
        /// <summary>
        /// Returns the positions for instrument.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="instrument">The instrument.</param>
        /// <returns></returns>
        public static string ReturnInstrumentPosition(int accountId, string instrument)
        {
            return "https://api-fxtrade.oanda.com/v1/accounts/" + accountId + "/positions/" + instrument;
        }
        /// <summary>
        /// Returns the open positions.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <returns></returns>
        public static string ReturnOpenPositions(int accountId)
        {
            return "https://api-fxtrade.oanda.com/v1/accounts/" + accountId + "/positions";
        }
        /// <summary>
        /// Returns the get transaction history.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="instrument">The instrument.</param>
        /// <returns></returns>
        public static string ReturnGetTransactionHistory(int accountId, string instrument)
        {
            return "https://api-fxpractice.oanda.com/v1/accounts/" + accountId + "/transactions?instrument=" + instrument;
        }
        /// <summary>
        /// Returns the transaction history.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="instrument">The instrument.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static string ReturnGetTransactionHistory(int accountId, string instrument, int count)
        {
            return "https://api-fxtrade.oanda.com/v1/accounts/" + accountId + "/transactions?instrument=" + instrument + "&count=" + count;
        }
        /// <summary>
        /// Returns the transaction information.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="transactionId">The transaction identifier.</param>
        /// <returns></returns>
        public static string ReturnTransactionInformation(int accountId, string transactionId)
        {
            return "https://api-fxtrade.oanda.com/v1/accounts/" + accountId + "/transactions/" + transactionId;
        }
        /// <summary>
        /// Returns the open trades.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="instrument">The instrument.</param>
        /// <returns></returns>
        public static string ReturnOpenTrades(int accountId, string instrument)
        {
            return "https://api-fxtrade.oanda.com/v1/accounts/" + accountId + "/trades?instrument=" + instrument;
        }
        /// <summary>
        /// Returns the open trades.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="instrument">The instrument.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static string ReturnOpenTrades(int accountId, string instrument, int count)
        {
            return "https://api-fxtrade.oanda.com/v1/accounts/" + accountId + "/trades?instrument=" + instrument + "&count=" + count;
        }
        /// <summary>
        /// Returns the trade information.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="tradeId">The trade identifier.</param>
        /// <returns></returns>
        public static string ReturnTradeInformation(int accountId, string tradeId)
        {
            return "https://api-fxtrade.oanda.com/v1/accounts/" + accountId + "/trades/" + tradeId;
        }
        /// <summary>
        /// Returns the orders.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="instrument">The instrument.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static string ReturnOrders(int accountId, string instrument, int count)
        {
            return "https://api-fxtrade.oanda.com/v1/accounts/" + accountId + "/orders?instrument=" + instrument + "&count=" + count;
        }
        /// <summary>
        /// Returns the order information.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        public static string ReturnOrderInformation(int accountId, int orderId)
        {
            return "https://api-fxtrade.oanda.com/v1/accounts/" + accountId + "/orders/" + orderId;
        }
        /// <summary>
        /// Places a market order.
        /// </summary>
        /// <param name="accoundId">The accound identifier.</param>
        /// <param name="instrument">The instrument to open the order on.</param>
        /// <param name="units">The number of units to open order for.</param>
        /// <param name="side">Direction of the order, either 'buy' or 'sell'.</param>
        /// <returns></returns>
        public static string PlaceMarketOrder(int accoundId, string instrument, int units, string side)
        {
            //const string type = "market";
            return "https://api-fxtrade.oanda.com/v1/accounts/" + accoundId + "/orders"; //instrument=" + instrument + "&units=" + units + "&side=" + side + "&type=" + type;
        }
        /// <summary>
        /// Places an order.
        /// </summary>
        /// <param name="instrument">The instrument to open the order on.</param>
        /// <param name="units">The number of units to open order for.</param>
        /// <param name="side">Direction of the order, either 'buy' or 'sell'.</param>
        /// <param name="type">The type of the order 'limit', 'stop', 'marketIfTouched' or 'market'.</param>
        /// <param name="expiry">If order type is 'limit', 'stop', or 'marketIfTouched'. The order expiration time in UTC. The value specified must be in a valid datetime format.</param>
        /// <param name="price">If order type is 'limit', 'stop', or 'marketIfTouched'. The price where the order is set to trigger at.</param>
        /// <param name="lowerBound">Optional: The minimum execution price.</param>
        /// <param name="upperBound">Optional: The maximum execution price.</param>
        /// <param name="stopLoss">Optional: The stop loss price.</param>
        /// <param name="takeProfit">Optional: The take profit price.</param>
        /// <param name="trailingStop">Optional: The trailing stop distance in pips, up to one decimal place. </param>
        /// <returns></returns>
        public static string PlaceOrder(string instrument, int units, string side, string type, string expiry,
            string price, string lowerBound, string upperBound, string stopLoss, string takeProfit, string trailingStop)
        {
            return "Not implemented.";
        }
        /// <summary>
        /// Deletes the trade.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="tradeId">The trade identifier.</param>
        /// <returns></returns>
        public static string DeleteTrade(int accountId, string tradeId)
        {
            return "https://api-fxtrade.oanda.com/v1/accounts/" + accountId + "/trades/" + tradeId;
        }
    }
}
