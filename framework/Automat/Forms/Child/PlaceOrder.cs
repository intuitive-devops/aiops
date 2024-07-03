using System;
using System.Globalization;
using System.Windows.Forms;
using SoftAgent.Core;

namespace SoftAgent.Automat.Forms.Child
{
    public partial class PlaceOrder : Form
    {
        private Form _owner;
        public static bool Instance { get; set; }

        public PlaceOrder(Form mOwner)
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
        }
        /// <summary>
        /// Formats the instrument pair for the REST server.
        /// </summary>
        /// <returns>A formatted string.</returns>
        public string FormatInstrumentPair()
        {
            var firstPair = firstInstrumentBox.Text;
            var secondPair = secondInstrumentBox.Text;
            return firstPair + "_" + secondPair;
        }

        #region Events
        private void placeOrderButton_Click(object sender, EventArgs e)
        {
            var result = Orders.PostMarketOrder(int.Parse(accountIdBox.Text), FormatInstrumentPair(), int.Parse(numberOfUnitsBox.Text),
                orderSideBox.Text);
            orderStatusBox.Text = result;
        }
        public void NewOrderOnClosing(object sender, FormClosingEventArgs e)
        {
            Instance = false;
        }
        #endregion
    }
}
