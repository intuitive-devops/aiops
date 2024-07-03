using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Windows.Forms;
using Boagaphish;
using SoftAgent.Automat.Properties;
using SoftAgent.Trend;
using SoftAgent.Utilities;

namespace SoftAgent.Automat.Forms
{
    /// <summary>
    /// From https://www.codeproject.com/Articles/175777/Financial-predictor-via-neural-network#fpredictor
    /// </summary>
    public partial class TrendForm : Form
    {      
        /// <summary>
        /// Default path to SnP csv
        /// </summary>
        private string _pathToSp = "SP500.csv";
        /// <summary>
        /// Default path to Prime interest rates csv
        /// </summary>
        private string _pathToPrimeRates = "rates.csv";
        /// <summary>
        /// Default path to Nasdaq indexes csv
        /// </summary>
        private string _pathToNasdaq = "nasdaq.csv";
        /// <summary>
        /// Default path to Dow indexes csv
        /// </summary>
        private string _pathToDow = "dow.csv";
        /// <summary>
        /// Predictor
        /// </summary>
        private PredictIndicators _predictor;
        // Prediction time-range
        private readonly DateTime _predictFrom = CsvReader.ParseDate("2017-08-11 14:02:00");
        private readonly DateTime _predictTo = CsvReader.ParseDate("2017-08-11 14:05:00");
        // Learning time-range
        private readonly DateTime _learnFrom = CsvReader.ParseDate("2017-08-11 12:02:00");
        private readonly DateTime _learnTo = CsvReader.ParseDate("2017-08-11 14:02:00");
        /// <summary>
        /// Default parameter for hidden layers
        /// </summary>
        private int _hiddenLayers = 2;
        /// <summary>
        /// Default parameter for hidden units
        /// </summary>
        private int _hiddenUnits = 41;
        /// <summary>
        /// Check if there is a need in reloading the files
        /// </summary>
        private bool _reloadFiles;
        List<PredictionResults> _results;
        /// <summary>
        /// Initializes a new instance of the <see cref="TrendForm"/> class.
        /// </summary>
        public TrendForm()
        {
            DateTime minDate;
            DateTime maxDate;
            InitializeComponent();
            _btnStop.Enabled = false;
            _btnExport.Enabled = false;
            try
            {
                maxDate = CsvReader.ParseDate(ConfigurationManager.AppSettings["MaxDate"]);
                minDate = CsvReader.ParseDate(ConfigurationManager.AppSettings["MinDate"]);
            }
            catch
            {
                maxDate = DateTime.Now;                        /*Maximum specified in the csv files*/
                minDate = CsvReader.ParseDate("2017-08-11 12:02:00");   /*Minimum specified in the csv files*/
            }

            /*Set some reasonable default values*/
            _dtpTrainFrom.Value = _learnFrom;
            _dtpTrainUntil.Value = _learnTo;
            _dtpPredictFrom.Value = _predictFrom;
            _dtpPredictTo.Value = _predictTo;

            _dtpTrainFrom.MaxDate = _dtpTrainUntil.MaxDate = _dtpPredictFrom.MaxDate = _dtpPredictTo.MaxDate = maxDate;
            _dtpTrainFrom.MinDate = _dtpTrainUntil.MinDate = _dtpPredictFrom.MinDate = _dtpPredictTo.MinDate = minDate;

            _nudHiddenLayers.Value = _hiddenLayers;
            _nudHiddenUnits.Value = _hiddenUnits;

        }
        private void MainFormLoad(object sender, EventArgs e)
        {
            SetPathsInTextBoxes();
        }
        private void SetPathsInTextBoxes()
        {
            if (File.Exists(Path.GetFullPath(_pathToSp)))
                _tbPathToSp.Text = Path.GetFileName(_pathToSp);
            if (File.Exists(Path.GetFullPath(_pathToPrimeRates)))
                _tbPathToPR.Text = Path.GetFileName(_pathToPrimeRates);
            if (File.Exists(Path.GetFullPath(_pathToDow)))
                _tbPathToDow.Text = Path.GetFileName(_pathToDow);
            if (File.Exists(Path.GetFullPath(_pathToNasdaq)))
                _tbPathToNasdaq.Text = Path.GetFileName(_pathToNasdaq);
        }
        /// <summary>
        /// Training callback, invoked at each iteration.
        /// </summary>
        /// <param name="epoch">Epoch number</param>
        /// <param name="error">Current error</param>
        /// <param name="algorithm">Training algorithm</param>
        private void TrainingCallback(int epoch, double error, TrainingAlgorithm algorithm)
        {
            Invoke(addAction, epoch, error, algorithm, _dgvTrainingResults);
        }
        /// <summary>
        /// Start training button pressed.
        /// </summary>
        private void BtnStartTrainingClick(object sender, EventArgs e)
        {
            if (_dgvTrainingResults.Rows.Count != 0)
                _dgvTrainingResults.Rows.Clear();

            if (_predictor == null)
            {
                _reloadFiles = false;
                if (!File.Exists(_tbPathToDow.Text) || !File.Exists(_tbPathToNasdaq.Text) ||
                    !File.Exists(_tbPathToPR.Text) || !File.Exists(_tbPathToSp.Text))
                {
                    MessageBox.Show(Resources.InputMissing, Resources.FileMissing, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            DateTime trainFrom = _dtpTrainFrom.Value;
            DateTime trainTo = _dtpTrainUntil.Value;

            if (trainFrom > trainTo)
            {
                MessageBox.Show(Resources.TrainFromTrainTo, Resources.BadParameters, MessageBoxButtons.OK, MessageBoxIcon.Information);
                _dtpTrainFrom.Focus();
                return;
            }
            FadeControls(true);
            if (_predictor == null)
            {
                _pathToSp = _tbPathToSp.Text;
                _pathToDow = _tbPathToDow.Text;
                _pathToNasdaq = _tbPathToNasdaq.Text;
                _pathToPrimeRates = _tbPathToPR.Text;
                 Cursor = Cursors.WaitCursor;
                _hiddenLayers = (int)_nudHiddenLayers.Value;
                _hiddenUnits = (int)_nudHiddenUnits.Value;
                try
                {
                    _predictor = new PredictIndicators(_pathToSp, _pathToPrimeRates, _pathToDow, _pathToNasdaq, _hiddenUnits, _hiddenLayers);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Resources.Exception, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.TrendGui, "StartTraining");
                    _predictor = null;
                    return;
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
            else if (_reloadFiles) /*Reload training sets*/
            {
                _pathToSp = _tbPathToSp.Text;
                _pathToDow = _tbPathToDow.Text;
                _pathToNasdaq = _tbPathToNasdaq.Text;
                _pathToPrimeRates = _tbPathToPR.Text;
                _predictor.ReloadFiles(_pathToSp, _pathToPrimeRates, _pathToDow, _pathToNasdaq);
                _dtpTrainFrom.MinDate = _predictor.MinIndexDate;
                _dtpTrainUntil.MaxDate = _predictor.MaxIndexDate;
            }
            /*Verify if dates do conform with the min/max ranges*/
            if (trainFrom < _predictor.MinIndexDate)
                _dtpTrainFrom.MinDate = _dtpTrainFrom.Value = trainFrom = _predictor.MinIndexDate;
            if (trainTo > _predictor.MaxIndexDate)
                _dtpTrainUntil.MaxDate = _dtpTrainUntil.Value = trainTo = _predictor.MaxIndexDate;
            TrainingStatus callback = TrainingCallback;
            _predictor.TrainNetworkAsync(trainFrom, trainTo, callback);
        }
        /// <summary>
        /// Predict the values
        /// </summary>
        private void BtnPredictClick(object sender, EventArgs e)
        {
            if (_dgvPredictionResults.Rows.Count != 0)
                _dgvPredictionResults.Rows.Clear();

            if (_predictor == null)         /*The network is untrained*/
            {
                _reloadFiles = false;
                if (!File.Exists(_tbPathToDow.Text) || !File.Exists(_tbPathToNasdaq.Text) ||
                    !File.Exists(_tbPathToPR.Text) || !File.Exists(_tbPathToSp.Text))
                {
                    MessageBox.Show(Resources.InputMissing, Resources.FileMissing, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Logging.WriteLog(Resources.InputMissing, Logging.LogType.Error, Logging.LogCaller.TrendGui, "Predict");
                    return;
                }
                switch (MessageBox.Show(Resources.UntrainedPredictorWarning, Resources.NoNetwork, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information))
                {
                    case DialogResult.Yes:
                        break;
                    case DialogResult.No:
                        /*Load the network*/
                        Cursor = Cursors.WaitCursor;
                        _hiddenLayers = (int)_nudHiddenLayers.Value;
                        _hiddenUnits = (int)_nudHiddenUnits.Value;
                        try
                        {
                            _predictor = new PredictIndicators(_pathToSp, _pathToPrimeRates, _pathToDow, _pathToNasdaq, _hiddenUnits, _hiddenLayers);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, Resources.Exception, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.TrendGui, "RunPredictor");
                            _predictor = null;
                            return;
                        }
                        finally
                        {
                            Cursor = Cursors.Default;
                        }
                        using (OpenFileDialog ofd = new OpenFileDialog() { FileName = "predictor.ntwrk", Filter = Resources.NtwrkFilter })
                        {
                            if (ofd.ShowDialog() == DialogResult.OK)
                            {
                                try
                                {
                                    _predictor.LoadNeuralNetwork(Path.GetFullPath(ofd.FileName));
                                }
                                catch
                                {
                                    MessageBox.Show(Resources.ExceptionMessage, Resources.Exception, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Logging.WriteLog(Resources.ExceptionMessage, Logging.LogType.Error, Logging.LogCaller.TrendGui, "");
                                    return;
                                }
                            }
                        }
                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }
            DateTime predictFrom = _dtpPredictFrom.Value;
            DateTime predictTo = _dtpPredictTo.Value;
            if (predictFrom > predictTo)
            {
                MessageBox.Show(Resources.PredictFromToWarning, Resources.BadParameters, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Logging.WriteLog(Resources.PredictFromToWarning, Logging.LogType.Error, Logging.LogCaller.TrendGui, "");
                _dtpPredictFrom.Focus();
                return;
            }

            if (_predictor == null)
            {
                _pathToSp = _tbPathToSp.Text;
                _pathToDow = _tbPathToDow.Text;
                _pathToNasdaq = _tbPathToNasdaq.Text;
                _pathToPrimeRates = _tbPathToPR.Text;
                 Cursor = Cursors.WaitCursor;
                _hiddenLayers = (int)_nudHiddenLayers.Value;
                _hiddenUnits = (int)_nudHiddenUnits.Value;
                try
                {
                    _predictor = new PredictIndicators(_pathToSp, _pathToPrimeRates, _pathToDow, _pathToNasdaq, _hiddenUnits, _hiddenLayers);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Resources.Exception, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.TrendGui, "StartTraining");
                    _predictor = null;
                    return;
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
            
            try
            {
                if (_reloadFiles)
                {
                    _pathToSp = _tbPathToSp.Text;
                    _pathToDow = _tbPathToDow.Text;
                    _pathToNasdaq = _tbPathToNasdaq.Text;
                    _pathToPrimeRates = _tbPathToPR.Text;
                    _predictor.ReloadFiles(_pathToSp, _pathToPrimeRates, _pathToDow, _pathToNasdaq);
                }
                _results = _predictor.Predict(predictFrom, predictTo);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.Exception, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.TrendGui, "StartTraining");
                return;
            }
            foreach (var item in _results)
            {
                _dgvPredictionResults.Rows.Add(item.Date.ToShortDateString(), item.ActualLow,
                                               item.PredictedLow.ToString("F2", CultureInfo.InvariantCulture), item.ActualHigh, item.PredictedHigh.ToString("F2", CultureInfo.InvariantCulture), item.ActualOpen,
                                               item.PredictedOpen.ToString("F2", CultureInfo.InvariantCulture), item.ActualClose, item.PredictedClose.ToString("F2", CultureInfo.InvariantCulture), item.Error.ToString("F4", CultureInfo.InvariantCulture));
            }
        }
        /// <summary>
        /// Form closing event
        /// </summary>
        private void WinFinancialMarketPredictorFormClosing(object sender, FormClosingEventArgs e)
        {
            if(_predictor != null)
                _predictor.AbortTraining();
        }
        /// <summary>
        /// Stop training
        /// </summary>
        private void BtnStopClick(object sender, EventArgs e)
        {
            FadeControls(false);
            _predictor.AbortTraining();
            _btnExport.Enabled = true;
        }
        /// <summary>
        /// New path to SnP500 index is selected
        /// </summary>
        private void TbPathToSpMouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { FileName = "sp500.csv", Filter = Resources.CsvFilter };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _tbPathToSp.Text = Path.GetFullPath(ofd.FileName);
                _reloadFiles = true;
            }
        }
        /// <summary>
        /// New path to Prime interest rate is selected
        /// </summary>
        private void TbPathToPrMouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { FileName = "pir.csv", Filter = Resources.CsvFilter };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _tbPathToPR.Text = Path.GetFullPath(ofd.FileName);
                _reloadFiles = true;
            }
        }
        /// <summary>
        /// New path to Dow index is selected
        /// </summary>
        private void TbPathToDowMouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { FileName = "dow.csv", Filter = Resources.CsvFilter };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _tbPathToDow.Text = Path.GetFullPath(ofd.FileName);
                _reloadFiles = true;
            }
        }
        /// <summary>
        /// Path to Nasdaq index is selected
        /// </summary>
        private void TbPathToNasdaqMouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { FileName = "nasdaq.csv", Filter = Resources.CsvFilter };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _tbPathToNasdaq.Text = Path.GetFullPath(ofd.FileName);
                _reloadFiles = true;
            }
        }
        /// <summary>
        /// Fade controls from the main form
        /// </summary>
        /// <param name="fade">If true - fade, otherwise - restore</param>
        private void FadeControls(bool fade)
        {
            Action<bool> action = (param) =>
                                  {
                                      _tbPathToSp.Enabled = param;
                                      _tbPathToPR.Enabled = param;
                                      _tbPathToDow.Enabled = param;
                                      _tbPathToNasdaq.Enabled = param;
                                      _btnStartTraining.Enabled = param;
                                      _btnStop.Enabled = !param;
                                      _dtpPredictFrom.Enabled = param;
                                      _dtpPredictTo.Enabled = param;
                                      _dtpTrainFrom.Enabled = param;
                                      _dtpTrainUntil.Enabled = param;
                                      _nudHiddenLayers.Enabled = param;
                                      _nudHiddenUnits.Enabled = param;
                                  };
            Invoke(action, !fade);
        }
        /// <summary>
        /// Save the network, for later reuse
        /// </summary>
        private void BtnExportClick(object sender, EventArgs e)
        {
            using(SaveFileDialog sfd = new SaveFileDialog() { FileName = "predictor.ntwrk", Filter = Resources.NtwrkFilter })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    FileIOPermission perm = new FileIOPermission(FileIOPermissionAccess.Write, Path.GetFullPath(sfd.FileName));
                    try
                    {
                        perm.Demand();
                    }
                    catch (System.Security.SecurityException)
                    {
                        MessageBox.Show(Resources.SecurityExceptionMessage, Resources.SecurityException, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Logging.WriteLog(Resources.SecurityExceptionMessage, Logging.LogType.Error, Logging.LogCaller.TrendGui, "");
                        return;
                    }
                    _predictor.ExportNeuralNetwork(Path.GetFullPath(sfd.FileName));
                }
            }
        }
        /// <summary>
        /// Load previously saved network
        /// </summary>
        private void BtnLoadClick(object sender, EventArgs e)
        {
            if (!File.Exists(_tbPathToDow.Text) || !File.Exists(_tbPathToNasdaq.Text) ||
                    !File.Exists(_tbPathToPR.Text) || !File.Exists(_tbPathToSp.Text))
            {
                MessageBox.Show(Resources.InputMissing, Resources.FileMissing, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (_predictor == null || _predictor.Loaded == false)
            {
                /*Load the network*/
                Cursor = Cursors.WaitCursor;
                _hiddenLayers = (int)_nudHiddenLayers.Value;
                _hiddenUnits = (int)_nudHiddenUnits.Value;
                try
                {
                    _predictor = new PredictIndicators(_pathToSp, _pathToPrimeRates, _pathToDow, _pathToNasdaq, _hiddenUnits, _hiddenLayers);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Resources.Exception, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.TrendGui, "PredictorLoading");
                    _predictor = null;
                    return;
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
            using (OpenFileDialog ofd = new OpenFileDialog() { FileName = "predictor.ntwrk", Filter = Resources.NtwrkFilter })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _predictor.LoadNeuralNetwork(Path.GetFullPath(ofd.FileName));
                        _nudHiddenLayers.Value = _predictor.HiddenLayers;
                        _nudHiddenUnits.Value = _predictor.HiddenUnits;
                    }
                    catch (System.Security.SecurityException)
                    {
                        MessageBox.Show(Resources.SecurityExceptionFolderLevel, Resources.Exception, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Logging.WriteLog(Resources.SecurityExceptionFolderLevel, Logging.LogType.Error, Logging.LogCaller.TrendGui, "");
                    }
                    catch
                    {
                        MessageBox.Show(Resources.ExceptionMessage, Resources.Exception, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Logging.WriteLog(Resources.ExceptionMessage, Logging.LogType.Error, Logging.LogCaller.TrendGui, "");
                    }
                }
            }
        }
        /// <summary>
        /// Save results
        /// </summary>
        private void BtnSaveResultsClick(object sender, EventArgs e)
        {
            var dgvResults = _dgvPredictionResults;
            SaveFileDialog ofd = new SaveFileDialog {Filter = Resources.CsvFilter, FileName = "results.csv"};
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CsvWriter writer;
                try
                {
                    writer = new CsvWriter(ofd.FileName);
                }
                catch (System.Security.SecurityException)
                {
                    MessageBox.Show(Resources.SecurityExceptionFolderLevel, Resources.Exception, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Logging.WriteLog(Resources.SecurityExceptionFolderLevel, Logging.LogType.Error, Logging.LogCaller.TrendGui, "StartTraining");
                    return;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, Resources.Exception, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.TrendGui, "StartTraining");
                    return;
                }
                object[,] values = new object[dgvResults.Rows.Count + 2, dgvResults.Columns.Count];
                int rowIndex = 0;
                int colIndex = 0;
                foreach (DataGridViewColumn col in dgvResults.Columns) /*Writing Column Headers*/
                {
                    values[rowIndex, colIndex] = col.HeaderText;
                    colIndex++;
                }
                rowIndex++; /*1*/

                foreach (DataGridViewRow row in dgvResults.Rows) /*Writing the values*/
                {
                    colIndex = 0;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        values[rowIndex, colIndex] = cell.Value;
                        colIndex++;
                    }
                    rowIndex++;
                }

                /*Writing the results in the last row*/
                writer.Write(values);
            }
        }
        /// <summary>
        /// Number of hidden units changed
        /// </summary>
        private void NudHiddenUnitsValueChanged(object sender, EventArgs e)
        {
            if(_predictor != null)
            {
                if(MessageBox.Show(Resources.ChangedNetwork, Resources.Warning, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    _predictor = null;
                }
            }
        }
        /// <summary>
        /// Number of hidden layers changed
        /// </summary>
        private void NudHiddenLayersValueChanged(object sender, EventArgs e)
        {
            if (_predictor != null)
            {
                if (MessageBox.Show(Resources.ChangedNetwork, Resources.Warning, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    _predictor = null;
                }
            }
        }

    }
}
