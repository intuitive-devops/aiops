using System;
using System.Globalization;
using System.Windows.Forms;
using Boagaphish;
using Boagaphish.Core.Variables;
using SoftAgent.Core;
using Timer = System.Windows.Forms.Timer;

namespace SoftAgent.Automat.Forms.Child
{
    public partial class AccountsManager : Form
    {
        private Form _owner;
        public static bool Instance { get; set; }
        public DataPoints.AccountDataPoint ActiveDataPoint { get; set; }
        public int Balance { get; set; }
        public int Interval { get; set; }
        public string Nomen { get; set; }
        public Timer MonitorTimer;

        public AccountsManager(Form mOwner)
        {
            _owner = mOwner;
            InitializeComponent();
            intervalValueBox.Text = @"0";
            unrealizedCheckBox.Checked = true;
            nomenSelectionBox.SelectedItem = "m";
            MonitorTimer = new Timer();
            stopMonitorButton.Enabled = false;
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

        public void FillAccountInformation()
        {
            balanceBox.Text = AccountInformation.Balance.ToString(CultureInfo.InvariantCulture);
            unrealizedBox.Text = AccountInformation.UnrealizedPl.ToString(CultureInfo.InvariantCulture);
            realizedBox.Text = AccountInformation.RealizedPl.ToString(CultureInfo.InvariantCulture);
            marginAvailableBox.Text = AccountInformation.MarginAvailable.ToString(CultureInfo.InvariantCulture);
            marginRateBox.Text = AccountInformation.MarginRate.ToString(CultureInfo.InvariantCulture);
            marginUsedBox.Text = AccountInformation.MarginUsed.ToString(CultureInfo.InvariantCulture);
            openTradesBox.Text = AccountInformation.OpenTrades.ToString(CultureInfo.InvariantCulture);
        }
        public void MonitorAccountDataPoint(DataPoints.AccountDataPoint accountDataPoint)
        {
            Nomen = nomenSelectionBox.Text;
            Interval = int.Parse(intervalValueBox.Text);
            switch (Nomen)
            {
                case "s":
                    MonitorTimer.Interval = Interval * 1000;
                    break;
                case "m":
                    MonitorTimer.Interval = Interval * 1000 * 60;
                    break;
                case "h":
                    MonitorTimer.Interval = Interval * 1000 * 3600;
                    break;
                case "d":
                    MonitorTimer.Interval = Interval * 1000 * 3600 * 24;
                    break;
            }
            MonitorTimer.Enabled = true;
            MonitorTimer.Start();
        }
        public void RetrieveAccountInformation()
        {
            Cursor.Current = Cursors.WaitCursor;
            var getAccountInformation = AccountInformation.GetAccountInformation(int.Parse(accountIdBox.Text));
            if (getAccountInformation)
            {
                accountsInformationBox.Text = @"AccountID: " + AccountInformation.AccountID + Environment.NewLine + @"Account name: " + AccountInformation.AccountName + Environment.NewLine + @"Account currency: " + AccountInformation.AccountCurrency + Environment.NewLine + @"Margin rate: " + AccountInformation.MarginRate + Environment.NewLine + @"Balance: " + AccountInformation.Balance + Environment.NewLine + @"Unrealized profit & loss: " + AccountInformation.UnrealizedPl + Environment.NewLine + @"Realized profit & loss: " + AccountInformation.RealizedPl + Environment.NewLine + @"Margin used: " + AccountInformation.MarginUsed + Environment.NewLine + @"Margin available: " + AccountInformation.MarginAvailable + Environment.NewLine + @"Open orders: " + AccountInformation.OpenOrders + Environment.NewLine + @"Open trades: " + AccountInformation.OpenTrades;
                FillAccountInformation();
            }
            Cursor.Current = Cursors.Default; 
        }

        #region Events
        public void AccountsManagerOnClosing(object sender, FormClosingEventArgs e)
        {
            MonitorTimer.Stop();
            Instance = false;
            MonitorTimer.Dispose();
        }
        private void retrieveAccountsInformationButton_Click(object sender, EventArgs e)
        {
            RetrieveAccountInformation();
        }
        private void startMonitorButton_Click(object sender, EventArgs e)
        {
            startMonitorButton.Enabled = false;
            stopMonitorButton.Enabled = true;
            MonitorTimer.Tick += OnMonitorIntervalElapsed;
            if (balanceCheckBox.Checked)
            {
                ActiveDataPoint = DataPoints.AccountDataPoint.Balance;
                MonitorAccountDataPoint(ActiveDataPoint);
            }
            if (marginUsedCheckBox.Checked)
            {
                ActiveDataPoint = DataPoints.AccountDataPoint.MarginUsed;
                MonitorAccountDataPoint(ActiveDataPoint);
            }
            if (unrealizedCheckBox.Checked)
            {
                ActiveDataPoint = DataPoints.AccountDataPoint.Unrealized;
                MonitorAccountDataPoint(ActiveDataPoint);
            }     
        }
        private void stopMonitorButton_Click(object sender, EventArgs e)
        {
            MonitorTimer.Tick -= OnMonitorIntervalElapsed;
            MonitorTimer.Stop();
            startMonitorButton.Enabled = true;
            stopMonitorButton.Enabled = false;
        }
        private void OnMonitorIntervalElapsed(object sender, EventArgs e)
        {
            AccountInformation.GetAccountInformation(int.Parse(accountIdBox.Text));
            switch (ActiveDataPoint)
            {
                case DataPoints.AccountDataPoint.Balance:
                    Logging.RecordEvent(ActiveDataPoint, AccountInformation.Balance, Interval, Nomen);
                    break;
                case DataPoints.AccountDataPoint.MarginUsed:
                    Logging.RecordEvent(ActiveDataPoint, AccountInformation.MarginUsed, Interval, Nomen);
                    break;
                case DataPoints.AccountDataPoint.Unrealized:
                    Logging.RecordEvent(ActiveDataPoint, AccountInformation.UnrealizedPl, Interval, Nomen);
                    break;
            }

        }
        #endregion

    }
}
