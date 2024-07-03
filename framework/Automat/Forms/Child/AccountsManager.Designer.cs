namespace SoftAgent.Automat.Forms.Child
{
    partial class AccountsManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountsManager));
            this.retrieveAccountsInformationButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.accountsInformationBox = new System.Windows.Forms.TextBox();
            this.accountIdBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.sessionTypeBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.balanceBox = new System.Windows.Forms.TextBox();
            this.unrealizedBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.realizedBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.marginUsedBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.marginAvailableBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.marginRateBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.openTradesBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.stopMonitorButton = new System.Windows.Forms.Button();
            this.nomenSelectionBox = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.intervalValueBox = new System.Windows.Forms.TextBox();
            this.balanceCheckBox = new System.Windows.Forms.CheckBox();
            this.startMonitorButton = new System.Windows.Forms.Button();
            this.marginUsedCheckBox = new System.Windows.Forms.CheckBox();
            this.unrealizedCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // retrieveAccountsInformationButton
            // 
            this.retrieveAccountsInformationButton.Location = new System.Drawing.Point(498, 109);
            this.retrieveAccountsInformationButton.Name = "retrieveAccountsInformationButton";
            this.retrieveAccountsInformationButton.Size = new System.Drawing.Size(88, 33);
            this.retrieveAccountsInformationButton.TabIndex = 1;
            this.retrieveAccountsInformationButton.Text = "Retrieve";
            this.retrieveAccountsInformationButton.UseVisualStyleBackColor = true;
            this.retrieveAccountsInformationButton.Click += new System.EventHandler(this.retrieveAccountsInformationButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.accountsInformationBox);
            this.groupBox2.Location = new System.Drawing.Point(12, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(470, 263);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Information";
            // 
            // accountsInformationBox
            // 
            this.accountsInformationBox.Location = new System.Drawing.Point(6, 21);
            this.accountsInformationBox.Multiline = true;
            this.accountsInformationBox.Name = "accountsInformationBox";
            this.accountsInformationBox.Size = new System.Drawing.Size(458, 236);
            this.accountsInformationBox.TabIndex = 1;
            // 
            // accountIdBox
            // 
            this.accountIdBox.Location = new System.Drawing.Point(498, 81);
            this.accountIdBox.Name = "accountIdBox";
            this.accountIdBox.Size = new System.Drawing.Size(100, 22);
            this.accountIdBox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(489, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Account ID:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(489, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 17);
            this.label4.TabIndex = 66;
            this.label4.Text = "Trading session";
            // 
            // sessionTypeBox
            // 
            this.sessionTypeBox.Location = new System.Drawing.Point(495, 33);
            this.sessionTypeBox.Name = "sessionTypeBox";
            this.sessionTypeBox.Size = new System.Drawing.Size(103, 22);
            this.sessionTypeBox.TabIndex = 65;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 299);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 17);
            this.label2.TabIndex = 67;
            this.label2.Text = "Balance";
            // 
            // balanceBox
            // 
            this.balanceBox.Location = new System.Drawing.Point(35, 320);
            this.balanceBox.Name = "balanceBox";
            this.balanceBox.Size = new System.Drawing.Size(100, 22);
            this.balanceBox.TabIndex = 68;
            // 
            // unrealizedBox
            // 
            this.unrealizedBox.Location = new System.Drawing.Point(247, 320);
            this.unrealizedBox.Name = "unrealizedBox";
            this.unrealizedBox.Size = new System.Drawing.Size(100, 22);
            this.unrealizedBox.TabIndex = 70;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(138, 299);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 17);
            this.label3.TabIndex = 69;
            this.label3.Text = "Realized";
            // 
            // realizedBox
            // 
            this.realizedBox.Location = new System.Drawing.Point(141, 320);
            this.realizedBox.Name = "realizedBox";
            this.realizedBox.Size = new System.Drawing.Size(100, 22);
            this.realizedBox.TabIndex = 72;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(244, 299);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 17);
            this.label5.TabIndex = 71;
            this.label5.Text = "Unrealized";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(350, 299);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 17);
            this.label6.TabIndex = 74;
            this.label6.Text = "Margin used";
            // 
            // marginUsedBox
            // 
            this.marginUsedBox.Location = new System.Drawing.Point(353, 320);
            this.marginUsedBox.Name = "marginUsedBox";
            this.marginUsedBox.Size = new System.Drawing.Size(100, 22);
            this.marginUsedBox.TabIndex = 73;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(456, 299);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(111, 17);
            this.label7.TabIndex = 76;
            this.label7.Text = "Margin available";
            // 
            // marginAvailableBox
            // 
            this.marginAvailableBox.Location = new System.Drawing.Point(459, 320);
            this.marginAvailableBox.Name = "marginAvailableBox";
            this.marginAvailableBox.Size = new System.Drawing.Size(100, 22);
            this.marginAvailableBox.TabIndex = 75;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(350, 351);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 17);
            this.label8.TabIndex = 78;
            this.label8.Text = "Margin rate";
            // 
            // marginRateBox
            // 
            this.marginRateBox.Location = new System.Drawing.Point(353, 372);
            this.marginRateBox.Name = "marginRateBox";
            this.marginRateBox.Size = new System.Drawing.Size(100, 22);
            this.marginRateBox.TabIndex = 77;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(32, 351);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(87, 17);
            this.label9.TabIndex = 80;
            this.label9.Text = "Open trades";
            // 
            // openTradesBox
            // 
            this.openTradesBox.Location = new System.Drawing.Point(35, 372);
            this.openTradesBox.Name = "openTradesBox";
            this.openTradesBox.Size = new System.Drawing.Size(100, 22);
            this.openTradesBox.TabIndex = 79;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.stopMonitorButton);
            this.groupBox1.Controls.Add(this.nomenSelectionBox);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.intervalValueBox);
            this.groupBox1.Controls.Add(this.balanceCheckBox);
            this.groupBox1.Controls.Add(this.startMonitorButton);
            this.groupBox1.Controls.Add(this.marginUsedCheckBox);
            this.groupBox1.Controls.Add(this.unrealizedCheckBox);
            this.groupBox1.Location = new System.Drawing.Point(18, 414);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(358, 132);
            this.groupBox1.TabIndex = 81;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Monitor variables";
            // 
            // stopMonitorButton
            // 
            this.stopMonitorButton.Location = new System.Drawing.Point(290, 75);
            this.stopMonitorButton.Name = "stopMonitorButton";
            this.stopMonitorButton.Size = new System.Drawing.Size(57, 34);
            this.stopMonitorButton.TabIndex = 84;
            this.stopMonitorButton.Text = "Stop";
            this.stopMonitorButton.UseVisualStyleBackColor = true;
            this.stopMonitorButton.Click += new System.EventHandler(this.stopMonitorButton_Click);
            // 
            // nomenSelectionBox
            // 
            this.nomenSelectionBox.FormattingEnabled = true;
            this.nomenSelectionBox.Items.AddRange(new object[] {
            "s",
            "m",
            "h",
            "d"});
            this.nomenSelectionBox.Location = new System.Drawing.Point(155, 81);
            this.nomenSelectionBox.Name = "nomenSelectionBox";
            this.nomenSelectionBox.Size = new System.Drawing.Size(52, 24);
            this.nomenSelectionBox.TabIndex = 83;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(29, 81);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(54, 17);
            this.label10.TabIndex = 82;
            this.label10.Text = "Interval";
            // 
            // intervalValueBox
            // 
            this.intervalValueBox.Location = new System.Drawing.Point(89, 81);
            this.intervalValueBox.Name = "intervalValueBox";
            this.intervalValueBox.Size = new System.Drawing.Size(60, 22);
            this.intervalValueBox.TabIndex = 4;
            // 
            // balanceCheckBox
            // 
            this.balanceCheckBox.AutoSize = true;
            this.balanceCheckBox.Location = new System.Drawing.Point(237, 32);
            this.balanceCheckBox.Name = "balanceCheckBox";
            this.balanceCheckBox.Size = new System.Drawing.Size(81, 21);
            this.balanceCheckBox.TabIndex = 2;
            this.balanceCheckBox.Text = "Balance";
            this.balanceCheckBox.UseVisualStyleBackColor = true;
            // 
            // startMonitorButton
            // 
            this.startMonitorButton.Location = new System.Drawing.Point(227, 75);
            this.startMonitorButton.Name = "startMonitorButton";
            this.startMonitorButton.Size = new System.Drawing.Size(57, 34);
            this.startMonitorButton.TabIndex = 1;
            this.startMonitorButton.Text = "Start";
            this.startMonitorButton.UseVisualStyleBackColor = true;
            this.startMonitorButton.Click += new System.EventHandler(this.startMonitorButton_Click);
            // 
            // marginUsedCheckBox
            // 
            this.marginUsedCheckBox.AutoSize = true;
            this.marginUsedCheckBox.Location = new System.Drawing.Point(123, 32);
            this.marginUsedCheckBox.Name = "marginUsedCheckBox";
            this.marginUsedCheckBox.Size = new System.Drawing.Size(108, 21);
            this.marginUsedCheckBox.TabIndex = 3;
            this.marginUsedCheckBox.Text = "Margin used";
            this.marginUsedCheckBox.UseVisualStyleBackColor = true;
            // 
            // unrealizedCheckBox
            // 
            this.unrealizedCheckBox.AutoSize = true;
            this.unrealizedCheckBox.Location = new System.Drawing.Point(17, 32);
            this.unrealizedCheckBox.Name = "unrealizedCheckBox";
            this.unrealizedCheckBox.Size = new System.Drawing.Size(98, 21);
            this.unrealizedCheckBox.TabIndex = 0;
            this.unrealizedCheckBox.Text = "Unrealized";
            this.unrealizedCheckBox.UseVisualStyleBackColor = true;
            // 
            // AccountsManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 568);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.openTradesBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.marginRateBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.marginAvailableBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.marginUsedBox);
            this.Controls.Add(this.realizedBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.unrealizedBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.balanceBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.sessionTypeBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.accountIdBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.retrieveAccountsInformationButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AccountsManager";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Account variables";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AccountsManagerOnClosing);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button retrieveAccountsInformationButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox accountsInformationBox;
        private System.Windows.Forms.TextBox accountIdBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox sessionTypeBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox balanceBox;
        private System.Windows.Forms.TextBox unrealizedBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox realizedBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox marginUsedBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox marginAvailableBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox marginRateBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox openTradesBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button startMonitorButton;
        private System.Windows.Forms.CheckBox unrealizedCheckBox;
        private System.Windows.Forms.CheckBox balanceCheckBox;
        private System.Windows.Forms.CheckBox marginUsedCheckBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox intervalValueBox;
        private System.Windows.Forms.ComboBox nomenSelectionBox;
        private System.Windows.Forms.Button stopMonitorButton;
    }
}