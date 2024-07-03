namespace SoftAgent.Automat.Forms.Child
{
    partial class PlaceOrder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlaceOrder));
            this.label1 = new System.Windows.Forms.Label();
            this.accountIdBox = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.secondInstrumentBox = new System.Windows.Forms.TextBox();
            this.firstInstrumentBox = new System.Windows.Forms.TextBox();
            this.placeOrderButton = new System.Windows.Forms.Button();
            this.orderSideBox = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.orderTypeBox = new System.Windows.Forms.ComboBox();
            this.numberOfUnitsBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.sessionTypeBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.orderStatusBox = new System.Windows.Forms.TextBox();
            this.groupBox6.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 17);
            this.label1.TabIndex = 53;
            this.label1.Text = "Account ID";
            // 
            // accountIdBox
            // 
            this.accountIdBox.Location = new System.Drawing.Point(12, 29);
            this.accountIdBox.Name = "accountIdBox";
            this.accountIdBox.Size = new System.Drawing.Size(100, 22);
            this.accountIdBox.TabIndex = 52;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.secondInstrumentBox);
            this.groupBox6.Controls.Add(this.firstInstrumentBox);
            this.groupBox6.Location = new System.Drawing.Point(15, 59);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(127, 103);
            this.groupBox6.TabIndex = 54;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Instrument";
            // 
            // secondInstrumentBox
            // 
            this.secondInstrumentBox.Location = new System.Drawing.Point(18, 57);
            this.secondInstrumentBox.Name = "secondInstrumentBox";
            this.secondInstrumentBox.Size = new System.Drawing.Size(88, 22);
            this.secondInstrumentBox.TabIndex = 1;
            // 
            // firstInstrumentBox
            // 
            this.firstInstrumentBox.Location = new System.Drawing.Point(18, 28);
            this.firstInstrumentBox.Name = "firstInstrumentBox";
            this.firstInstrumentBox.Size = new System.Drawing.Size(88, 22);
            this.firstInstrumentBox.TabIndex = 0;
            // 
            // placeOrderButton
            // 
            this.placeOrderButton.Location = new System.Drawing.Point(220, 175);
            this.placeOrderButton.Name = "placeOrderButton";
            this.placeOrderButton.Size = new System.Drawing.Size(72, 46);
            this.placeOrderButton.TabIndex = 1;
            this.placeOrderButton.Text = "Place order";
            this.placeOrderButton.UseVisualStyleBackColor = true;
            this.placeOrderButton.Click += new System.EventHandler(this.placeOrderButton_Click);
            // 
            // orderSideBox
            // 
            this.orderSideBox.FormattingEnabled = true;
            this.orderSideBox.Items.AddRange(new object[] {
            "buy",
            "sell"});
            this.orderSideBox.Location = new System.Drawing.Point(18, 56);
            this.orderSideBox.Name = "orderSideBox";
            this.orderSideBox.Size = new System.Drawing.Size(114, 24);
            this.orderSideBox.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.orderTypeBox);
            this.groupBox1.Controls.Add(this.orderSideBox);
            this.groupBox1.Location = new System.Drawing.Point(148, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(144, 103);
            this.groupBox1.TabIndex = 57;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Order type";
            // 
            // orderTypeBox
            // 
            this.orderTypeBox.FormattingEnabled = true;
            this.orderTypeBox.Items.AddRange(new object[] {
            "limit",
            "stop",
            "marketIfTouched",
            "market"});
            this.orderTypeBox.Location = new System.Drawing.Point(18, 26);
            this.orderTypeBox.Name = "orderTypeBox";
            this.orderTypeBox.Size = new System.Drawing.Size(114, 24);
            this.orderTypeBox.TabIndex = 0;
            // 
            // numberOfUnitsBox
            // 
            this.numberOfUnitsBox.Location = new System.Drawing.Point(183, 29);
            this.numberOfUnitsBox.Name = "numberOfUnitsBox";
            this.numberOfUnitsBox.Size = new System.Drawing.Size(97, 22);
            this.numberOfUnitsBox.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(177, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 17);
            this.label2.TabIndex = 60;
            this.label2.Text = "Number of units";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 167);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 17);
            this.label3.TabIndex = 62;
            this.label3.Text = "Trading session";
            // 
            // sessionTypeBox
            // 
            this.sessionTypeBox.Location = new System.Drawing.Point(15, 187);
            this.sessionTypeBox.Name = "sessionTypeBox";
            this.sessionTypeBox.Size = new System.Drawing.Size(103, 22);
            this.sessionTypeBox.TabIndex = 61;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.orderStatusBox);
            this.groupBox2.Location = new System.Drawing.Point(15, 227);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(277, 285);
            this.groupBox2.TabIndex = 63;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Server response";
            // 
            // orderStatusBox
            // 
            this.orderStatusBox.Location = new System.Drawing.Point(6, 21);
            this.orderStatusBox.Multiline = true;
            this.orderStatusBox.Name = "orderStatusBox";
            this.orderStatusBox.Size = new System.Drawing.Size(259, 258);
            this.orderStatusBox.TabIndex = 59;
            // 
            // PlaceOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 524);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.sessionTypeBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numberOfUnitsBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.placeOrderButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.accountIdBox);
            this.Controls.Add(this.groupBox6);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlaceOrder";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Place order";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewOrderOnClosing);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox accountIdBox;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button placeOrderButton;
        private System.Windows.Forms.ComboBox orderSideBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox numberOfUnitsBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox orderTypeBox;
        private System.Windows.Forms.TextBox firstInstrumentBox;
        private System.Windows.Forms.TextBox secondInstrumentBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox sessionTypeBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox orderStatusBox;
    }
}