namespace SoftAgent.Automat.Forms.Child
{
    partial class ForecastChartForm
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
            this.displayChart = new Boagaphish.Controls.Chart();
            this.solveForecastButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.transferFunctionSelectionBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.granularityBox = new System.Windows.Forms.TextBox();
            this.analyticIterationsBox = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.processingTimeBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.analysisTrajectoryBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numberOfCandlesBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // displayChart
            // 
            this.displayChart.Location = new System.Drawing.Point(13, 23);
            this.displayChart.Name = "displayChart";
            this.displayChart.Size = new System.Drawing.Size(980, 469);
            this.displayChart.TabIndex = 0;
            // 
            // solveForecastButton
            // 
            this.solveForecastButton.Location = new System.Drawing.Point(837, 520);
            this.solveForecastButton.Name = "solveForecastButton";
            this.solveForecastButton.Size = new System.Drawing.Size(75, 31);
            this.solveForecastButton.TabIndex = 1;
            this.solveForecastButton.Text = "Solve";
            this.solveForecastButton.UseVisualStyleBackColor = true;
            this.solveForecastButton.Click += new System.EventHandler(this.solveForecastButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(918, 520);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 31);
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.transferFunctionSelectionBox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.granularityBox);
            this.groupBox1.Controls.Add(this.analyticIterationsBox);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.processingTimeBox);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.analysisTrajectoryBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numberOfCandlesBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(13, 498);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(818, 103);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Trajectory properties";
            // 
            // transferFunctionSelectionBox
            // 
            this.transferFunctionSelectionBox.FormattingEnabled = true;
            this.transferFunctionSelectionBox.Items.AddRange(new object[] {
            "Sigmoid",
            "RationalSigmoid",
            "BipolarSigmoid",
            "Gaussian"});
            this.transferFunctionSelectionBox.Location = new System.Drawing.Point(561, 58);
            this.transferFunctionSelectionBox.Name = "transferFunctionSelectionBox";
            this.transferFunctionSelectionBox.Size = new System.Drawing.Size(138, 24);
            this.transferFunctionSelectionBox.TabIndex = 31;
            this.transferFunctionSelectionBox.SelectedIndexChanged += new System.EventHandler(this.transferFunctionBox_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(558, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 17);
            this.label5.TabIndex = 19;
            this.label5.Text = "Transfer function";
            // 
            // granularityBox
            // 
            this.granularityBox.Location = new System.Drawing.Point(6, 58);
            this.granularityBox.Name = "granularityBox";
            this.granularityBox.Size = new System.Drawing.Size(116, 22);
            this.granularityBox.TabIndex = 20;
            // 
            // analyticIterationsBox
            // 
            this.analyticIterationsBox.Location = new System.Drawing.Point(429, 59);
            this.analyticIterationsBox.Name = "analyticIterationsBox";
            this.analyticIterationsBox.Size = new System.Drawing.Size(116, 22);
            this.analyticIterationsBox.TabIndex = 19;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(698, 36);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(110, 17);
            this.label16.TabIndex = 18;
            this.label16.Text = "Process time (s)";
            // 
            // processingTimeBox
            // 
            this.processingTimeBox.Location = new System.Drawing.Point(705, 60);
            this.processingTimeBox.Name = "processingTimeBox";
            this.processingTimeBox.Size = new System.Drawing.Size(103, 22);
            this.processingTimeBox.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(287, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(123, 17);
            this.label7.TabIndex = 15;
            this.label7.Text = "Analysis trajectory";
            // 
            // analysisTrajectoryBox
            // 
            this.analysisTrajectoryBox.FormattingEnabled = true;
            this.analysisTrajectoryBox.Location = new System.Drawing.Point(289, 58);
            this.analysisTrajectoryBox.Name = "analysisTrajectoryBox";
            this.analysisTrajectoryBox.Size = new System.Drawing.Size(121, 24);
            this.analysisTrajectoryBox.TabIndex = 16;
            this.analysisTrajectoryBox.SelectedIndexChanged += new System.EventHandler(this.analysisTrajectoryBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Granularity";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(140, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Number of candles";
            // 
            // numberOfCandlesBox
            // 
            this.numberOfCandlesBox.Location = new System.Drawing.Point(143, 58);
            this.numberOfCandlesBox.Name = "numberOfCandlesBox";
            this.numberOfCandlesBox.Size = new System.Drawing.Size(124, 22);
            this.numberOfCandlesBox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(426, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "Analytic iterations";
            // 
            // ForecastChartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 616);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.solveForecastButton);
            this.Controls.Add(this.displayChart);
            this.MaximizeBox = false;
            this.Name = "ForecastChartForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ForecastFormClosing);
            this.Load += new System.EventHandler(this.ChartFormLoad);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Boagaphish.Controls.Chart displayChart;
        private System.Windows.Forms.Button solveForecastButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox processingTimeBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox analysisTrajectoryBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox numberOfCandlesBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox transferFunctionSelectionBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox analyticIterationsBox;
        private System.Windows.Forms.TextBox granularityBox;
    }
}