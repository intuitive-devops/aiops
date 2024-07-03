using System;
using System.Windows.Forms;
using SoftAgent.Trend;

namespace SoftAgent.Automat.Forms
{
    partial class NewAutomatConsole
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewAutomatConsole));
            this._dgvTrainingResults = new System.Windows.Forms.DataGridView();
            this.Epoch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Error = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrainingAlgorithm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._dgvPredictionResults = new System.Windows.Forms.DataGridView();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActualSP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PredictedSP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActualNasdaq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PredictedNasdaq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActualDow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PredictedDow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActualPIR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PredictedPIR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ErrorDifference = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialogAeon = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialogDump = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogDump = new System.Windows.Forms.SaveFileDialog();
            this.AeonOutput = new System.Windows.Forms.RichTextBox();
            this.notificationLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numberOfAnalyticCandles = new System.Windows.Forms.TextBox();
            this.refreshListButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.displayArrayButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.xmsFilepathBox = new System.Windows.Forms.ComboBox();
            this.numberOfCandlesBox = new System.Windows.Forms.TextBox();
            this.loadLiveDataButton = new System.Windows.Forms.Button();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.sessionTypeBox = new System.Windows.Forms.ComboBox();
            this.dataStateLabel = new System.Windows.Forms.Label();
            this.sampleSizeLabel = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.accountsFormButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._dgvTrainingResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgvPredictionResults)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox16.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // _dgvTrainingResults
            // 
            this._dgvTrainingResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Epoch,
            this.Error,
            this.TrainingAlgorithm});
            this._dgvTrainingResults.Location = new System.Drawing.Point(0, 0);
            this._dgvTrainingResults.Name = "_dgvTrainingResults";
            this._dgvTrainingResults.Size = new System.Drawing.Size(240, 150);
            this._dgvTrainingResults.TabIndex = 0;
            // 
            // Epoch
            // 
            this.Epoch.HeaderText = "Epoch";
            this.Epoch.Name = "Epoch";
            // 
            // Error
            // 
            this.Error.HeaderText = "Error";
            this.Error.Name = "Error";
            // 
            // TrainingAlgorithm
            // 
            this.TrainingAlgorithm.HeaderText = "Training Algorithm";
            this.TrainingAlgorithm.Name = "TrainingAlgorithm";
            // 
            // _dgvPredictionResults
            // 
            this._dgvPredictionResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Date,
            this.ActualSP,
            this.PredictedSP,
            this.ActualNasdaq,
            this.PredictedNasdaq,
            this.ActualDow,
            this.PredictedDow,
            this.ActualPIR,
            this.PredictedPIR,
            this.ErrorDifference});
            this._dgvPredictionResults.Location = new System.Drawing.Point(0, 0);
            this._dgvPredictionResults.Name = "_dgvPredictionResults";
            this._dgvPredictionResults.Size = new System.Drawing.Size(240, 150);
            this._dgvPredictionResults.TabIndex = 0;
            // 
            // Date
            // 
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            // 
            // ActualSP
            // 
            this.ActualSP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ActualSP.HeaderText = "Actual Low";
            this.ActualSP.Name = "ActualSP";
            // 
            // PredictedSP
            // 
            this.PredictedSP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PredictedSP.HeaderText = "Predicted Low";
            this.PredictedSP.Name = "PredictedSP";
            // 
            // ActualNasdaq
            // 
            this.ActualNasdaq.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ActualNasdaq.HeaderText = "Actual High";
            this.ActualNasdaq.Name = "ActualNasdaq";
            // 
            // PredictedNasdaq
            // 
            this.PredictedNasdaq.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PredictedNasdaq.HeaderText = "Predicted High";
            this.PredictedNasdaq.Name = "PredictedNasdaq";
            // 
            // ActualDow
            // 
            this.ActualDow.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ActualDow.HeaderText = "Actual Open";
            this.ActualDow.Name = "ActualDow";
            // 
            // PredictedDow
            // 
            this.PredictedDow.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PredictedDow.HeaderText = "Predicted Open";
            this.PredictedDow.Name = "PredictedDow";
            // 
            // ActualPIR
            // 
            this.ActualPIR.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ActualPIR.HeaderText = "Actual Close";
            this.ActualPIR.Name = "ActualPIR";
            // 
            // PredictedPIR
            // 
            this.PredictedPIR.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PredictedPIR.HeaderText = "Predicted Close";
            this.PredictedPIR.Name = "PredictedPIR";
            // 
            // ErrorDifference
            // 
            this.ErrorDifference.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ErrorDifference.HeaderText = "RMS Error";
            this.ErrorDifference.Name = "ErrorDifference";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // folderBrowserDialogAeon
            // 
            this.folderBrowserDialogAeon.Description = "Select code files folder";
            this.folderBrowserDialogAeon.RootFolder = System.Environment.SpecialFolder.ApplicationData;
            this.folderBrowserDialogAeon.ShowNewFolderButton = false;
            // 
            // openFileDialogDump
            // 
            this.openFileDialogDump.FileName = "Graphmaster.dat";
            this.openFileDialogDump.Title = "Select the binary file to load into memory";
            // 
            // saveFileDialogDump
            // 
            this.saveFileDialogDump.FileName = "Graphmaster.dat";
            this.saveFileDialogDump.Title = "Select location to save graphmaster";
            // 
            // AeonOutput
            // 
            this.AeonOutput.Location = new System.Drawing.Point(12, 12);
            this.AeonOutput.Name = "AeonOutput";
            this.AeonOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.AeonOutput.Size = new System.Drawing.Size(188, 580);
            this.AeonOutput.TabIndex = 3;
            this.AeonOutput.Text = "";
            this.AeonOutput.TextChanged += new System.EventHandler(this.AeonOutput_TextChanged);
            // 
            // notificationLabel
            // 
            this.notificationLabel.AutoSize = true;
            this.notificationLabel.Location = new System.Drawing.Point(168, 7);
            this.notificationLabel.Name = "notificationLabel";
            this.notificationLabel.Size = new System.Drawing.Size(0, 17);
            this.notificationLabel.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.accountsFormButton);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.numberOfAnalyticCandles);
            this.groupBox1.Controls.Add(this.refreshListButton);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.displayArrayButton);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.xmsFilepathBox);
            this.groupBox1.Controls.Add(this.numberOfCandlesBox);
            this.groupBox1.Controls.Add(this.loadLiveDataButton);
            this.groupBox1.Location = new System.Drawing.Point(206, 67);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(397, 443);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tools";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(200, 250);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 17);
            this.label6.TabIndex = 15;
            this.label6.Text = "# of Candles:";
            // 
            // numberOfAnalyticCandles
            // 
            this.numberOfAnalyticCandles.Location = new System.Drawing.Point(297, 247);
            this.numberOfAnalyticCandles.Name = "numberOfAnalyticCandles";
            this.numberOfAnalyticCandles.Size = new System.Drawing.Size(69, 22);
            this.numberOfAnalyticCandles.TabIndex = 14;
            // 
            // refreshListButton
            // 
            this.refreshListButton.Location = new System.Drawing.Point(244, 186);
            this.refreshListButton.Name = "refreshListButton";
            this.refreshListButton.Size = new System.Drawing.Size(99, 31);
            this.refreshListButton.TabIndex = 13;
            this.refreshListButton.Text = "Refresh list";
            this.refreshListButton.UseVisualStyleBackColor = true;
            this.refreshListButton.Click += new System.EventHandler(this.refreshListButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 157);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(278, 17);
            this.label5.TabIndex = 12;
            this.label5.Text = "Task: Apply algorithm to available datasets";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(254, 17);
            this.label4.TabIndex = 11;
            this.label4.Text = "2. Leverage algorithms against dataset";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(148, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "Task: Get candle data";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(211, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "1. Construct dataset for analysis";
            // 
            // displayArrayButton
            // 
            this.displayArrayButton.Location = new System.Drawing.Point(29, 242);
            this.displayArrayButton.Name = "displayArrayButton";
            this.displayArrayButton.Size = new System.Drawing.Size(155, 30);
            this.displayArrayButton.TabIndex = 8;
            this.displayArrayButton.Text = "Visualize data";
            this.displayArrayButton.UseVisualStyleBackColor = true;
            this.displayArrayButton.Click += new System.EventHandler(this.loadChartFormButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(200, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "# of Candles:";
            // 
            // xmsFilepathBox
            // 
            this.xmsFilepathBox.FormattingEnabled = true;
            this.xmsFilepathBox.Location = new System.Drawing.Point(29, 186);
            this.xmsFilepathBox.Name = "xmsFilepathBox";
            this.xmsFilepathBox.Size = new System.Drawing.Size(198, 24);
            this.xmsFilepathBox.TabIndex = 6;
            this.xmsFilepathBox.SelectedIndexChanged += new System.EventHandler(this.xmsFilepathBox_SelectedIndexChanged);
            // 
            // numberOfCandlesBox
            // 
            this.numberOfCandlesBox.Location = new System.Drawing.Point(297, 91);
            this.numberOfCandlesBox.Name = "numberOfCandlesBox";
            this.numberOfCandlesBox.Size = new System.Drawing.Size(69, 22);
            this.numberOfCandlesBox.TabIndex = 5;
            // 
            // loadLiveDataButton
            // 
            this.loadLiveDataButton.Location = new System.Drawing.Point(29, 87);
            this.loadLiveDataButton.Name = "loadLiveDataButton";
            this.loadLiveDataButton.Size = new System.Drawing.Size(155, 31);
            this.loadLiveDataButton.TabIndex = 1;
            this.loadLiveDataButton.Text = "Live candles to xms";
            this.loadLiveDataButton.UseVisualStyleBackColor = true;
            this.loadLiveDataButton.Click += new System.EventHandler(this.loadLiveDataButton_Click);
            // 
            // groupBox16
            // 
            this.groupBox16.Controls.Add(this.sessionTypeBox);
            this.groupBox16.Location = new System.Drawing.Point(342, 12);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(255, 49);
            this.groupBox16.TabIndex = 34;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "Session type";
            // 
            // sessionTypeBox
            // 
            this.sessionTypeBox.FormattingEnabled = true;
            this.sessionTypeBox.Location = new System.Drawing.Point(108, 19);
            this.sessionTypeBox.Name = "sessionTypeBox";
            this.sessionTypeBox.Size = new System.Drawing.Size(97, 24);
            this.sessionTypeBox.TabIndex = 21;
            this.sessionTypeBox.SelectedIndexChanged += new System.EventHandler(this.sessionTypeBox_SelectedIndexChanged);
            // 
            // dataStateLabel
            // 
            this.dataStateLabel.AutoSize = true;
            this.dataStateLabel.Location = new System.Drawing.Point(17, 28);
            this.dataStateLabel.Name = "dataStateLabel";
            this.dataStateLabel.Size = new System.Drawing.Size(105, 17);
            this.dataStateLabel.TabIndex = 0;
            this.dataStateLabel.Text = "No data loaded";
            // 
            // sampleSizeLabel
            // 
            this.sampleSizeLabel.AutoSize = true;
            this.sampleSizeLabel.Location = new System.Drawing.Point(17, 48);
            this.sampleSizeLabel.Name = "sampleSizeLabel";
            this.sampleSizeLabel.Size = new System.Drawing.Size(88, 17);
            this.sampleSizeLabel.TabIndex = 1;
            this.sampleSizeLabel.Text = "Sample size:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.sampleSizeLabel);
            this.groupBox3.Controls.Add(this.dataStateLabel);
            this.groupBox3.Location = new System.Drawing.Point(329, 516);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(274, 76);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Encog";
            // 
            // accountsFormButton
            // 
            this.accountsFormButton.Location = new System.Drawing.Point(29, 324);
            this.accountsFormButton.Name = "accountsFormButton";
            this.accountsFormButton.Size = new System.Drawing.Size(90, 30);
            this.accountsFormButton.TabIndex = 16;
            this.accountsFormButton.Text = "Accounts";
            this.accountsFormButton.UseVisualStyleBackColor = true;
            this.accountsFormButton.Click += new System.EventHandler(this.accountsFormButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 288);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(342, 17);
            this.label7.TabIndex = 17;
            this.label7.Text = "3. Create a table of most interesting trading variables";
            // 
            // NewAutomatConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 605);
            this.Controls.Add(this.groupBox16);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.notificationLabel);
            this.Controls.Add(this.AeonOutput);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(633, 650);
            this.MinimumSize = new System.Drawing.Size(633, 650);
            this.Name = "NewAutomatConsole";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Automat console";
            ((System.ComponentModel.ISupportInitialize)(this._dgvTrainingResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgvPredictionResults)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox16.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogAeon;
        private System.Windows.Forms.OpenFileDialog openFileDialogDump;
        private System.Windows.Forms.SaveFileDialog saveFileDialogDump;
        private System.Windows.Forms.RichTextBox AeonOutput;
        private System.Windows.Forms.Label notificationLabel;
        // Objects to host the data and space to write for computations.
        // 1. Training objects for the computational space.
        private DataGridView _dgvTrainingResults;
        private DataGridViewTextBoxColumn Epoch;
        private DataGridViewTextBoxColumn Error;
        private DataGridViewTextBoxColumn TrainingAlgorithm;
        // 2. Prediction objects for the computational space.
        private DataGridView _dgvPredictionResults;
        private DataGridViewTextBoxColumn Date;
        private DataGridViewTextBoxColumn ActualSP;
        private DataGridViewTextBoxColumn PredictedSP;
        private DataGridViewTextBoxColumn ActualNasdaq;
        private DataGridViewTextBoxColumn PredictedNasdaq;
        private DataGridViewTextBoxColumn ActualDow;
        private DataGridViewTextBoxColumn PredictedDow;
        private DataGridViewTextBoxColumn ActualPIR;
        private DataGridViewTextBoxColumn PredictedPIR;
        private DataGridViewTextBoxColumn ErrorDifference;

        private readonly Action<int, double, TrainingAlgorithm, DataGridView> addAction = new Action<int, double, TrainingAlgorithm, DataGridView>((epoch, error, algorithm, dgvTrainingResults) =>
        {
            int rowIndex = dgvTrainingResults.Rows.Add(epoch, error, algorithm.ToString());
            dgvTrainingResults.FirstDisplayedScrollingRowIndex = rowIndex;
        });
        private GroupBox groupBox1;
        private Button loadLiveDataButton;
        private TextBox numberOfCandlesBox;
        private ComboBox xmsFilepathBox;
        private Label label1;
        private GroupBox groupBox16;
        private ComboBox sessionTypeBox;
        private Button displayArrayButton;
        private Label label3;
        private Label label2;
        private Label label5;
        private Label label4;
        private Button refreshListButton;
        private Label label6;
        private TextBox numberOfAnalyticCandles;
        private Label dataStateLabel;
        private Label sampleSizeLabel;
        private GroupBox groupBox3;
        private Label label7;
        private Button accountsFormButton;
    }
}