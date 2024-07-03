using System;
using System.Globalization;
using System.Windows.Forms;
using SoftAgent.Core;

namespace SoftAgent.Automat.Forms.Child
{
    public partial class PortfolioManager : Form
    {
        private Form _owner;
        public static bool Instance { get; set; }
        public int NumberOfOrdersToDisplay { get; set; }

        public bool GetAllOpenPositions { get; private set; }
        public bool GetAccountInformation { get; private set; }
        public bool GetTrades { get; private set; }
        public bool GetOrders { get; private set; }
        public bool GetTransactionHistory { get; private set; }

        public PortfolioManager(Form mOwner)
        {
            _owner = mOwner;
            InitializeComponent();
            firstInstrumentBox.Text = Rates.HoldingCurrency;
            secondInstrumentBox.Text = Rates.TradingCurrency;
            sessionTypeBox.Text = MonitoringSession.MonitoringSessionType;
            if (MonitoringSession.MonitoringSessionType == MonitoringSession.MonitoringSessionAccount.Practice.ToString())
            {
                const int session = (int)MonitoringSession.MonitoringSessionAccount.Practice;
                accountIdBox.Text = session.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                const int session = (int)MonitoringSession.MonitoringSessionAccount.Live;
                accountIdBox.Text = session.ToString(CultureInfo.InvariantCulture);
            }
            NumberOfOrdersToDisplay = 5;
            if (NewAutomatConsole.PortfolioDatasetsLoaded)
            {
                GetAllOpenPositions = true;
                GetAccountInformation = true;
                GetTrades = true;
                GetOrders = true;
                GetTransactionHistory = true;
                FillColumms();
            }
            Instance = true;
        }

        public string FormatInstrumentPair()
        {
            var firstPair = firstInstrumentBox.Text;
            var secondPair = secondInstrumentBox.Text;
            return firstPair + "_" + secondPair;
        }
        private void LoadDatasets()
        {
            if (!NewAutomatConsole.PortfolioDatasetsLoaded)
            {
                try
                {
                    GetAllOpenPositions = Positions.GetOpenPositions(int.Parse(accountIdBox.Text));
                    GetAccountInformation = AccountInformation.GetAccountInformation(int.Parse(accountIdBox.Text));
                    GetTrades = Trades.GetOpenTrades(int.Parse(accountIdBox.Text), FormatInstrumentPair());
                    GetOrders = Orders.GetOrders(int.Parse(accountIdBox.Text), FormatInstrumentPair(), NumberOfOrdersToDisplay);
                    GetTransactionHistory = Transactions.GetTransactionHistory(int.Parse(accountIdBox.Text), FormatInstrumentPair());
                    if (!GetAllOpenPositions && !GetAccountInformation && !GetTrades && !GetOrders &&
                        !GetTransactionHistory)
                    {
                        NewAutomatConsole.PortfolioDatasetsLoaded = false;
                        MessageBox.Show(@"An error occurred while trying to obtain the requisite data. Check the computer's network connection.", @"Dataset error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }  
                    else
                    {
                        NewAutomatConsole.PortfolioDatasetsLoaded = true;
                        FillColumms();
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }
        private void FillColumms()
        {
            if(GetAccountInformation)
            accountsGridView.Rows.Add(AccountInformation.AccountName, AccountInformation.MarginRate, AccountInformation.Balance, AccountInformation.UnrealizedPl, AccountInformation.RealizedPl, AccountInformation.MarginUsed, AccountInformation.MarginAvailable, AccountInformation.OpenOrders, AccountInformation.OpenTrades);
            for (var i = 0; i < Positions.Side.Length; i++)
            {
                if(GetAllOpenPositions)
                positionsGridView.Rows.Add(Positions.Side[i], Positions.Instrument[i], Positions.Units[i], Positions.AveragePrice[i]);
            }
            for (var i = 0; i < Trades.TradeID.Length; i++)
            {
                if(GetTrades)
                tradesGridView.Rows.Add(Trades.TradeID[i], Trades.Units[i], Trades.Instrument[i], Trades.Time[i], Trades.Price[i], Trades.TakeProfit[i], Trades.StopLoss[i], Trades.TrailingStop[i], Trades.TrailingAmount[i]);
            }
            for (var i = 0; i < Orders.OrderID.Length; i++)
            {
                if (GetOrders)
                    ordersGridView.Rows.Add(Orders.OrderID, Orders.Side, Orders.Instrument, Orders.Price, Orders.Expiry, Orders.Type, Orders.Units, Orders.StopLoss, Orders.TakeProfit, Orders.TrailingStop);
            }
            for (var i = 0; i < Transactions.TransactionID.Length; i++)
            {
                if (GetTransactionHistory)
                transactionsGridView.Rows.Add(Transactions.TransactionID[i], Transactions.AccountID[i], Transactions.Time[i], Transactions.Type[i], Transactions.Instrument[i], Transactions.Interest[i], Transactions.AccountBalance[i]);
            }

        }

        #region Events
        public void TradePortfolioFormClosing(object sender, FormClosingEventArgs e)
        {
            Instance = false;
        }
        private void closeFormButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void refreshDataGridViewButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            LoadDatasets();
            Cursor.Current = Cursors.Default;
        }
        #endregion

    }
}
