using System;
using System.Windows.Forms;
using SoftAgent.Trend;

namespace SoftAgent.Automat.Forms
{
    partial class AutomatConsole
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutomatConsole));
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
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.liveSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.networkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solverNetworkLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forecastNetworkLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.predictorNetworkLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solverTrainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forecastTrainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.solutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.castToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autonomousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialogAeon = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialogDump = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogDump = new System.Windows.Forms.SaveFileDialog();
            this.AeonOutput = new System.Windows.Forms.RichTextBox();
            this.notificationLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.placeOrderButton = new System.Windows.Forms.Button();
            this.portfolioManagerButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.boagaphishTradeOnceButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.xmsFilepathBox = new System.Windows.Forms.ComboBox();
            this.numberOfCandlesBox = new System.Windows.Forms.TextBox();
            this.castSolutionButton = new System.Windows.Forms.Button();
            this.trainForecastButton = new System.Windows.Forms.Button();
            this.trainSolverButton = new System.Windows.Forms.Button();
            this.loadLiveDataButton = new System.Windows.Forms.Button();
            this.loadFileDataButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.displayArrayButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.encogTradeOnceButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.sampleSizeLabel = new System.Windows.Forms.Label();
            this.dataStateLabel = new System.Windows.Forms.Label();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.getButton = new System.Windows.Forms.Button();
            this.containerNameComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this._dgvTrainingResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgvPredictionResults)).BeginInit();
            this.menuStripMain.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox16.SuspendLayout();
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
            // menuStripMain
            // 
            this.menuStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(463, 24);
            this.menuStripMain.TabIndex = 2;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.trainToolStripMenuItem,
            this.toolStripSeparator1,
            this.solutionToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataToolStripMenuItem,
            this.networkToolStripMenuItem});
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.loadToolStripMenuItem.Text = "Load...";
            // 
            // dataToolStripMenuItem
            // 
            this.dataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromFileToolStripMenuItem,
            this.liveSourceToolStripMenuItem,
            this.generateToolStripMenuItem});
            this.dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            this.dataToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.dataToolStripMenuItem.Text = "Data...";
            // 
            // fromFileToolStripMenuItem
            // 
            this.fromFileToolStripMenuItem.Name = "fromFileToolStripMenuItem";
            this.fromFileToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.fromFileToolStripMenuItem.Text = "From file";
            this.fromFileToolStripMenuItem.Click += new System.EventHandler(this.fromFileToolStripMenuItem_Click);
            // 
            // liveSourceToolStripMenuItem
            // 
            this.liveSourceToolStripMenuItem.Name = "liveSourceToolStripMenuItem";
            this.liveSourceToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.liveSourceToolStripMenuItem.Text = "Live source";
            this.liveSourceToolStripMenuItem.Click += new System.EventHandler(this.liveSourceToolStripMenuItem_Click);
            // 
            // generateToolStripMenuItem
            // 
            this.generateToolStripMenuItem.Name = "generateToolStripMenuItem";
            this.generateToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.generateToolStripMenuItem.Text = "Generate";
            this.generateToolStripMenuItem.Click += new System.EventHandler(this.generateToolStripMenuItem_Click);
            // 
            // networkToolStripMenuItem
            // 
            this.networkToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.solverNetworkLoadToolStripMenuItem,
            this.forecastNetworkLoadToolStripMenuItem,
            this.predictorNetworkLoadToolStripMenuItem});
            this.networkToolStripMenuItem.Name = "networkToolStripMenuItem";
            this.networkToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.networkToolStripMenuItem.Text = "Network...";
            // 
            // solverNetworkLoadToolStripMenuItem
            // 
            this.solverNetworkLoadToolStripMenuItem.Name = "solverNetworkLoadToolStripMenuItem";
            this.solverNetworkLoadToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.solverNetworkLoadToolStripMenuItem.Text = "Solver";
            this.solverNetworkLoadToolStripMenuItem.Click += new System.EventHandler(this.solverNetworkLoadToolStripMenuItem_Click);
            // 
            // forecastNetworkLoadToolStripMenuItem
            // 
            this.forecastNetworkLoadToolStripMenuItem.Name = "forecastNetworkLoadToolStripMenuItem";
            this.forecastNetworkLoadToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.forecastNetworkLoadToolStripMenuItem.Text = "Forecast";
            this.forecastNetworkLoadToolStripMenuItem.Click += new System.EventHandler(this.forecastNetworkLoadToolStripMenuItem_Click);
            // 
            // predictorNetworkLoadToolStripMenuItem
            // 
            this.predictorNetworkLoadToolStripMenuItem.Name = "predictorNetworkLoadToolStripMenuItem";
            this.predictorNetworkLoadToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.predictorNetworkLoadToolStripMenuItem.Text = "Predictor";
            this.predictorNetworkLoadToolStripMenuItem.Click += new System.EventHandler(this.predictorNetworkLoadToolStripMenuItem_Click);
            // 
            // trainToolStripMenuItem
            // 
            this.trainToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.solverTrainToolStripMenuItem,
            this.forecastTrainToolStripMenuItem});
            this.trainToolStripMenuItem.Name = "trainToolStripMenuItem";
            this.trainToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.trainToolStripMenuItem.Text = "Train...";
            // 
            // solverTrainToolStripMenuItem
            // 
            this.solverTrainToolStripMenuItem.Name = "solverTrainToolStripMenuItem";
            this.solverTrainToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.solverTrainToolStripMenuItem.Text = "Solver";
            this.solverTrainToolStripMenuItem.Click += new System.EventHandler(this.solverTrainToolStripMenuItem_Click);
            // 
            // forecastTrainToolStripMenuItem
            // 
            this.forecastTrainToolStripMenuItem.Name = "forecastTrainToolStripMenuItem";
            this.forecastTrainToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.forecastTrainToolStripMenuItem.Text = "Forecast";
            this.forecastTrainToolStripMenuItem.Click += new System.EventHandler(this.forecastTrainToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(124, 6);
            // 
            // solutionToolStripMenuItem
            // 
            this.solutionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.castToolStripMenuItem,
            this.autonomousToolStripMenuItem});
            this.solutionToolStripMenuItem.Name = "solutionToolStripMenuItem";
            this.solutionToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.solutionToolStripMenuItem.Text = "Solution...";
            // 
            // castToolStripMenuItem
            // 
            this.castToolStripMenuItem.Name = "castToolStripMenuItem";
            this.castToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.castToolStripMenuItem.Text = "Cast";
            this.castToolStripMenuItem.Click += new System.EventHandler(this.castToolStripMenuItem_Click);
            // 
            // autonomousToolStripMenuItem
            // 
            this.autonomousToolStripMenuItem.Name = "autonomousToolStripMenuItem";
            this.autonomousToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.autonomousToolStripMenuItem.Text = "Autonomous";
            this.autonomousToolStripMenuItem.Click += new System.EventHandler(this.autonomousToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(124, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearConsoleToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // clearConsoleToolStripMenuItem
            // 
            this.clearConsoleToolStripMenuItem.Name = "clearConsoleToolStripMenuItem";
            this.clearConsoleToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.clearConsoleToolStripMenuItem.Text = "Clear console";
            this.clearConsoleToolStripMenuItem.Click += new System.EventHandler(this.clearConsoleToolStripMenuItem_Click_1);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.licenseToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // licenseToolStripMenuItem
            // 
            this.licenseToolStripMenuItem.Name = "licenseToolStripMenuItem";
            this.licenseToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.licenseToolStripMenuItem.Text = "License";
            this.licenseToolStripMenuItem.Click += new System.EventHandler(this.licenseToolStripMenuItem_Click);
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
            this.AeonOutput.Location = new System.Drawing.Point(9, 30);
            this.AeonOutput.Margin = new System.Windows.Forms.Padding(2);
            this.AeonOutput.Name = "AeonOutput";
            this.AeonOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.AeonOutput.Size = new System.Drawing.Size(234, 452);
            this.AeonOutput.TabIndex = 3;
            this.AeonOutput.Text = "";
            this.AeonOutput.TextChanged += new System.EventHandler(this.AeonOutput_TextChanged);
            // 
            // notificationLabel
            // 
            this.notificationLabel.AutoSize = true;
            this.notificationLabel.Location = new System.Drawing.Point(126, 6);
            this.notificationLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.notificationLabel.Name = "notificationLabel";
            this.notificationLabel.Size = new System.Drawing.Size(0, 13);
            this.notificationLabel.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.placeOrderButton);
            this.groupBox1.Controls.Add(this.portfolioManagerButton);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.boagaphishTradeOnceButton);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.xmsFilepathBox);
            this.groupBox1.Controls.Add(this.numberOfCandlesBox);
            this.groupBox1.Controls.Add(this.castSolutionButton);
            this.groupBox1.Controls.Add(this.trainForecastButton);
            this.groupBox1.Controls.Add(this.trainSolverButton);
            this.groupBox1.Controls.Add(this.loadLiveDataButton);
            this.groupBox1.Controls.Add(this.loadFileDataButton);
            this.groupBox1.Location = new System.Drawing.Point(247, 79);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(206, 240);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Boagaphish";
            // 
            // placeOrderButton
            // 
            this.placeOrderButton.Location = new System.Drawing.Point(132, 201);
            this.placeOrderButton.Margin = new System.Windows.Forms.Padding(2);
            this.placeOrderButton.Name = "placeOrderButton";
            this.placeOrderButton.Size = new System.Drawing.Size(69, 32);
            this.placeOrderButton.TabIndex = 10;
            this.placeOrderButton.Text = "Place order";
            this.placeOrderButton.UseVisualStyleBackColor = true;
            this.placeOrderButton.Click += new System.EventHandler(this.placeOrderButton_Click);
            // 
            // portfolioManagerButton
            // 
            this.portfolioManagerButton.Location = new System.Drawing.Point(146, 162);
            this.portfolioManagerButton.Margin = new System.Windows.Forms.Padding(2);
            this.portfolioManagerButton.Name = "portfolioManagerButton";
            this.portfolioManagerButton.Size = new System.Drawing.Size(55, 34);
            this.portfolioManagerButton.TabIndex = 9;
            this.portfolioManagerButton.Text = "Portfolio";
            this.portfolioManagerButton.UseVisualStyleBackColor = true;
            this.portfolioManagerButton.Click += new System.EventHandler(this.portfolioManagerButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 21);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "From pure xms";
            // 
            // boagaphishTradeOnceButton
            // 
            this.boagaphishTradeOnceButton.Location = new System.Drawing.Point(5, 204);
            this.boagaphishTradeOnceButton.Margin = new System.Windows.Forms.Padding(2);
            this.boagaphishTradeOnceButton.Name = "boagaphishTradeOnceButton";
            this.boagaphishTradeOnceButton.Size = new System.Drawing.Size(80, 24);
            this.boagaphishTradeOnceButton.TabIndex = 7;
            this.boagaphishTradeOnceButton.Text = "Trade once";
            this.boagaphishTradeOnceButton.UseVisualStyleBackColor = true;
            this.boagaphishTradeOnceButton.Click += new System.EventHandler(this.boagaphishTradeOnceButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(111, 76);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Metrics:";
            // 
            // xmsFilepathBox
            // 
            this.xmsFilepathBox.FormattingEnabled = true;
            this.xmsFilepathBox.Location = new System.Drawing.Point(91, 43);
            this.xmsFilepathBox.Margin = new System.Windows.Forms.Padding(2);
            this.xmsFilepathBox.Name = "xmsFilepathBox";
            this.xmsFilepathBox.Size = new System.Drawing.Size(111, 21);
            this.xmsFilepathBox.TabIndex = 6;
            // 
            // numberOfCandlesBox
            // 
            this.numberOfCandlesBox.Location = new System.Drawing.Point(163, 76);
            this.numberOfCandlesBox.Margin = new System.Windows.Forms.Padding(2);
            this.numberOfCandlesBox.Name = "numberOfCandlesBox";
            this.numberOfCandlesBox.Size = new System.Drawing.Size(39, 20);
            this.numberOfCandlesBox.TabIndex = 5;
            // 
            // castSolutionButton
            // 
            this.castSolutionButton.Location = new System.Drawing.Point(5, 167);
            this.castSolutionButton.Margin = new System.Windows.Forms.Padding(2);
            this.castSolutionButton.Name = "castSolutionButton";
            this.castSolutionButton.Size = new System.Drawing.Size(80, 24);
            this.castSolutionButton.TabIndex = 4;
            this.castSolutionButton.Text = "Cast solution";
            this.castSolutionButton.UseVisualStyleBackColor = true;
            this.castSolutionButton.Click += new System.EventHandler(this.castSolutionButton_Click);
            // 
            // trainForecastButton
            // 
            this.trainForecastButton.Location = new System.Drawing.Point(4, 137);
            this.trainForecastButton.Margin = new System.Windows.Forms.Padding(2);
            this.trainForecastButton.Name = "trainForecastButton";
            this.trainForecastButton.Size = new System.Drawing.Size(80, 24);
            this.trainForecastButton.TabIndex = 3;
            this.trainForecastButton.Text = "Train forecast";
            this.trainForecastButton.UseVisualStyleBackColor = true;
            this.trainForecastButton.Click += new System.EventHandler(this.trainForecastButton_Click);
            // 
            // trainSolverButton
            // 
            this.trainSolverButton.Location = new System.Drawing.Point(4, 108);
            this.trainSolverButton.Margin = new System.Windows.Forms.Padding(2);
            this.trainSolverButton.Name = "trainSolverButton";
            this.trainSolverButton.Size = new System.Drawing.Size(72, 24);
            this.trainSolverButton.TabIndex = 2;
            this.trainSolverButton.Text = "Train solver";
            this.trainSolverButton.UseVisualStyleBackColor = true;
            this.trainSolverButton.Click += new System.EventHandler(this.trainSolverButton_Click);
            // 
            // loadLiveDataButton
            // 
            this.loadLiveDataButton.Location = new System.Drawing.Point(4, 72);
            this.loadLiveDataButton.Margin = new System.Windows.Forms.Padding(2);
            this.loadLiveDataButton.Name = "loadLiveDataButton";
            this.loadLiveDataButton.Size = new System.Drawing.Size(99, 24);
            this.loadLiveDataButton.TabIndex = 1;
            this.loadLiveDataButton.Text = "Container data";
            this.loadLiveDataButton.UseVisualStyleBackColor = true;
            this.loadLiveDataButton.Click += new System.EventHandler(this.loadLiveDataButton_Click);
            // 
            // loadFileDataButton
            // 
            this.loadFileDataButton.Location = new System.Drawing.Point(4, 41);
            this.loadFileDataButton.Margin = new System.Windows.Forms.Padding(2);
            this.loadFileDataButton.Name = "loadFileDataButton";
            this.loadFileDataButton.Size = new System.Drawing.Size(80, 24);
            this.loadFileDataButton.TabIndex = 0;
            this.loadFileDataButton.Text = "Load file data";
            this.loadFileDataButton.UseVisualStyleBackColor = true;
            this.loadFileDataButton.Click += new System.EventHandler(this.loadFileDataButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.displayArrayButton);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.encogTradeOnceButton);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(248, 324);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(205, 89);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Encog";
            // 
            // displayArrayButton
            // 
            this.displayArrayButton.Location = new System.Drawing.Point(144, 13);
            this.displayArrayButton.Margin = new System.Windows.Forms.Padding(2);
            this.displayArrayButton.Name = "displayArrayButton";
            this.displayArrayButton.Size = new System.Drawing.Size(56, 24);
            this.displayArrayButton.TabIndex = 8;
            this.displayArrayButton.Text = "Display";
            this.displayArrayButton.UseVisualStyleBackColor = true;
            this.displayArrayButton.Click += new System.EventHandler(this.displayArrayButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(98, 62);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "incl train/predict";
            // 
            // encogTradeOnceButton
            // 
            this.encogTradeOnceButton.Location = new System.Drawing.Point(4, 57);
            this.encogTradeOnceButton.Margin = new System.Windows.Forms.Padding(2);
            this.encogTradeOnceButton.Name = "encogTradeOnceButton";
            this.encogTradeOnceButton.Size = new System.Drawing.Size(80, 24);
            this.encogTradeOnceButton.TabIndex = 6;
            this.encogTradeOnceButton.Text = "Trade once";
            this.encogTradeOnceButton.UseVisualStyleBackColor = true;
            this.encogTradeOnceButton.Click += new System.EventHandler(this.encogTradeOnceButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 24);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "From xms split into csv";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.sampleSizeLabel);
            this.groupBox3.Controls.Add(this.dataStateLabel);
            this.groupBox3.Location = new System.Drawing.Point(247, 419);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(206, 62);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Details";
            // 
            // sampleSizeLabel
            // 
            this.sampleSizeLabel.AutoSize = true;
            this.sampleSizeLabel.Location = new System.Drawing.Point(13, 39);
            this.sampleSizeLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.sampleSizeLabel.Name = "sampleSizeLabel";
            this.sampleSizeLabel.Size = new System.Drawing.Size(66, 13);
            this.sampleSizeLabel.TabIndex = 1;
            this.sampleSizeLabel.Text = "Sample size:";
            // 
            // dataStateLabel
            // 
            this.dataStateLabel.AutoSize = true;
            this.dataStateLabel.Location = new System.Drawing.Point(13, 23);
            this.dataStateLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.dataStateLabel.Name = "dataStateLabel";
            this.dataStateLabel.Size = new System.Drawing.Size(80, 13);
            this.dataStateLabel.TabIndex = 0;
            this.dataStateLabel.Text = "No data loaded";
            // 
            // groupBox16
            // 
            this.groupBox16.Controls.Add(this.containerNameComboBox);
            this.groupBox16.Controls.Add(this.getButton);
            this.groupBox16.Location = new System.Drawing.Point(256, 34);
            this.groupBox16.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox16.Size = new System.Drawing.Size(191, 40);
            this.groupBox16.TabIndex = 34;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "Container monitoring";
            // 
            // getButton
            // 
            this.getButton.Location = new System.Drawing.Point(18, 15);
            this.getButton.Margin = new System.Windows.Forms.Padding(2);
            this.getButton.Name = "getButton";
            this.getButton.Size = new System.Drawing.Size(58, 25);
            this.getButton.TabIndex = 11;
            this.getButton.Text = "Get";
            this.getButton.UseVisualStyleBackColor = true;
            this.getButton.Click += new System.EventHandler(this.GetButtonClick);
            // 
            // containerNameComboBox
            // 
            this.containerNameComboBox.FormattingEnabled = true;
            this.containerNameComboBox.Items.AddRange(new object[] {
            "demo-flow",
            "noise"});
            this.containerNameComboBox.Location = new System.Drawing.Point(93, 18);
            this.containerNameComboBox.Name = "containerNameComboBox";
            this.containerNameComboBox.Size = new System.Drawing.Size(83, 21);
            this.containerNameComboBox.TabIndex = 35;
            // 
            // AutomatConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 496);
            this.Controls.Add(this.groupBox16);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.notificationLabel);
            this.Controls.Add(this.AeonOutput);
            this.Controls.Add(this.menuStripMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(479, 535);
            this.MinimumSize = new System.Drawing.Size(479, 535);
            this.Name = "AutomatConsole";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Soft agent: Automat";
            ((System.ComponentModel.ISupportInitialize)(this._dgvTrainingResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgvPredictionResults)).EndInit();
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox16.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem solverTrainToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem licenseToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogAeon;
        private System.Windows.Forms.OpenFileDialog openFileDialogDump;
        private System.Windows.Forms.SaveFileDialog saveFileDialogDump;
        private System.Windows.Forms.RichTextBox AeonOutput;
        private System.Windows.Forms.Label notificationLabel;
        private System.Windows.Forms.ToolStripMenuItem forecastTrainToolStripMenuItem;
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
        private GroupBox groupBox2;
        private Button loadFileDataButton;
        private Button castSolutionButton;
        private Button trainForecastButton;
        private Button trainSolverButton;
        private Button loadLiveDataButton;
        private GroupBox groupBox3;
        private TextBox numberOfCandlesBox;
        private ToolStripMenuItem loadToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem solutionToolStripMenuItem;
        private ToolStripMenuItem castToolStripMenuItem;
        private ToolStripMenuItem autonomousToolStripMenuItem;
        private ToolStripMenuItem networkToolStripMenuItem;
        private ToolStripMenuItem solverNetworkLoadToolStripMenuItem;
        private ToolStripMenuItem forecastNetworkLoadToolStripMenuItem;
        private ToolStripMenuItem predictorNetworkLoadToolStripMenuItem;
        private ToolStripMenuItem dataToolStripMenuItem;
        private ToolStripMenuItem fromFileToolStripMenuItem;
        private ToolStripMenuItem liveSourceToolStripMenuItem;
        private Label dataStateLabel;
        private Label sampleSizeLabel;
        private ComboBox xmsFilepathBox;
        private Label label1;
        private Button boagaphishTradeOnceButton;
        private Button encogTradeOnceButton;
        private Label label2;
        private Label label4;
        private Label label3;
        private Button portfolioManagerButton;
        private GroupBox groupBox16;
        private Button placeOrderButton;
        private Button displayArrayButton;
        private ToolStripMenuItem generateToolStripMenuItem;
        private Button getButton;
        private ComboBox containerNameComboBox;
    }
}