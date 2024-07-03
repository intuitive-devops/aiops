using System;
using System.Windows.Forms;
using SoftAgent.Trend;

namespace SoftAgent.Automat.Forms
{
    partial class AutomatVoice
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
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extrasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.codeFragmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assembliesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSpeech = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastResultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastRequestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialogAeon = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialogDump = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogDump = new System.Windows.Forms.SaveFileDialog();
            this.AeonOutput = new System.Windows.Forms.RichTextBox();
            this.notificationLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._dgvTrainingResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgvPredictionResults)).BeginInit();
            this.menuStripMain.SuspendLayout();
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
            this.menuStripMain.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStripMain.Size = new System.Drawing.Size(332, 28);
            this.menuStripMain.TabIndex = 2;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripMenuItemSpeech,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extrasToolStripMenuItem,
            this.codeFragmentsToolStripMenuItem,
            this.assembliesToolStripMenuItem});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(197, 26);
            this.addToolStripMenuItem.Text = "Add...";
            // 
            // extrasToolStripMenuItem
            // 
            this.extrasToolStripMenuItem.Name = "extrasToolStripMenuItem";
            this.extrasToolStripMenuItem.Size = new System.Drawing.Size(184, 24);
            this.extrasToolStripMenuItem.Text = "Extras";
            // 
            // codeFragmentsToolStripMenuItem
            // 
            this.codeFragmentsToolStripMenuItem.Name = "codeFragmentsToolStripMenuItem";
            this.codeFragmentsToolStripMenuItem.Size = new System.Drawing.Size(184, 24);
            this.codeFragmentsToolStripMenuItem.Text = "Code fragments";
            this.codeFragmentsToolStripMenuItem.Click += new System.EventHandler(this.addExtraCodeFragmentsMenuItem_Click);
            // 
            // assembliesToolStripMenuItem
            // 
            this.assembliesToolStripMenuItem.Name = "assembliesToolStripMenuItem";
            this.assembliesToolStripMenuItem.Size = new System.Drawing.Size(184, 24);
            this.assembliesToolStripMenuItem.Text = "Assemblies";
            this.assembliesToolStripMenuItem.Click += new System.EventHandler(this.addExtraAssembliesMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(194, 6);
            // 
            // toolStripMenuItemSpeech
            // 
            this.toolStripMenuItemSpeech.Checked = true;
            this.toolStripMenuItemSpeech.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItemSpeech.Name = "toolStripMenuItemSpeech";
            this.toolStripMenuItemSpeech.Size = new System.Drawing.Size(197, 26);
            this.toolStripMenuItemSpeech.Text = "Synthesize speech";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(194, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(197, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearConsoleToolStripMenuItem,
            this.lastResultToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.userToolStripMenuItem,
            this.lastRequestToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // clearConsoleToolStripMenuItem
            // 
            this.clearConsoleToolStripMenuItem.Name = "clearConsoleToolStripMenuItem";
            this.clearConsoleToolStripMenuItem.Size = new System.Drawing.Size(167, 24);
            this.clearConsoleToolStripMenuItem.Text = "Clear console";
            this.clearConsoleToolStripMenuItem.Click += new System.EventHandler(this.clearConsoleToolStripMenuItem_Click_1);
            // 
            // lastResultToolStripMenuItem
            // 
            this.lastResultToolStripMenuItem.Name = "lastResultToolStripMenuItem";
            this.lastResultToolStripMenuItem.Size = new System.Drawing.Size(167, 24);
            this.lastResultToolStripMenuItem.Text = "Last result";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(167, 24);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // userToolStripMenuItem
            // 
            this.userToolStripMenuItem.Name = "userToolStripMenuItem";
            this.userToolStripMenuItem.Size = new System.Drawing.Size(167, 24);
            this.userToolStripMenuItem.Text = "User";
            // 
            // lastRequestToolStripMenuItem
            // 
            this.lastRequestToolStripMenuItem.Name = "lastRequestToolStripMenuItem";
            this.lastRequestToolStripMenuItem.Size = new System.Drawing.Size(167, 24);
            this.lastRequestToolStripMenuItem.Text = "Last request";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.licenseToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(126, 24);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // licenseToolStripMenuItem
            // 
            this.licenseToolStripMenuItem.Name = "licenseToolStripMenuItem";
            this.licenseToolStripMenuItem.Size = new System.Drawing.Size(126, 24);
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
            this.AeonOutput.Location = new System.Drawing.Point(12, 37);
            this.AeonOutput.Name = "AeonOutput";
            this.AeonOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.AeonOutput.Size = new System.Drawing.Size(311, 198);
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
            // AutomatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 245);
            this.Controls.Add(this.notificationLabel);
            this.Controls.Add(this.AeonOutput);
            this.Controls.Add(this.menuStripMain);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(350, 290);
            this.MinimumSize = new System.Drawing.Size(350, 290);
            this.Name = "AutomatForm";
            this.ShowIcon = false;
            this.Text = "Interactive automat";
            ((System.ComponentModel.ISupportInitialize)(this._dgvTrainingResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dgvPredictionResults)).EndInit();
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extrasToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSpeech;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lastResultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem userToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lastRequestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem licenseToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogAeon;
        private System.Windows.Forms.OpenFileDialog openFileDialogDump;
        private System.Windows.Forms.SaveFileDialog saveFileDialogDump;
        private System.Windows.Forms.RichTextBox AeonOutput;
        private System.Windows.Forms.Label notificationLabel;
        private System.Windows.Forms.ToolStripMenuItem codeFragmentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem assembliesToolStripMenuItem;
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
    }
}