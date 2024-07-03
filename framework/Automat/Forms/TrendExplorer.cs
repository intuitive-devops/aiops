using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Boagaphish;
using SoftAgent.Automat.Properties;
using SoftAgent.Trend;
using SoftAgent.Utilities;

namespace SoftAgent.Automat.Forms
{
    public partial class TrendExplorer : Form
    {      
        private string _pathToLow = "low.csv";
        private string _pathToClose = "close.csv";
        private string _pathToHigh = "high.csv";
        private string _pathToOpen = "open.csv";
        private PredictIndicators _predictor;
        // Prediction time-range
        private readonly DateTime _predictFrom = CsvReader.ParseDate("2017-08-11 14:02:00");
        private readonly DateTime _predictTo = CsvReader.ParseDate("2017-08-26 14:05:00");
        // Learning time-range
        private readonly DateTime _learnFrom = CsvReader.ParseDate("2017-08-11 12:02:00");
        private readonly DateTime _learnTo = CsvReader.ParseDate("2017-08-26 14:02:00");

        public static string ReturnFilePath(string file)
        {
            return Environment.CurrentDirectory + @"/data/csv/" + file;
        }

        private int _hiddenLayers = 2;
        private int _hiddenUnits = 41;
        private bool _reloadFiles;
        private List<PredictionResults> _results;

        public TrendExplorer()
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
            {//yyyy-MM-dd HH:mm:ss
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

        #region Events
        private void MainFormLoad(object sender, EventArgs e)
        {
            SetPathsInTextBoxes();
        }
        private void SetPathsInTextBoxes()
        {
            if (File.Exists(Path.GetFullPath(ReturnFilePath(_pathToLow))))
                _tbPathToSp.Text = Path.GetFileName(ReturnFilePath(_pathToLow));
            if (File.Exists(Path.GetFullPath(ReturnFilePath(_pathToClose))))
                _tbPathToPR.Text = Path.GetFileName(ReturnFilePath(_pathToClose));
            if (File.Exists(Path.GetFullPath(ReturnFilePath(_pathToOpen))))
                _tbPathToDow.Text = Path.GetFileName(ReturnFilePath(_pathToOpen));
            if (File.Exists(Path.GetFullPath(ReturnFilePath(_pathToHigh))))
                _tbPathToNasdaq.Text = Path.GetFileName(ReturnFilePath(_pathToHigh));
        }
        private void TrainingCallback(int epoch, double error, TrainingAlgorithm algorithm)
        {
            Invoke(addAction, epoch, error, algorithm, _dgvTrainingResults);
        }
        private void BtnStartTrainingClick(object sender, EventArgs e)
        {
            if (_dgvTrainingResults.Rows.Count != 0)
                _dgvTrainingResults.Rows.Clear();

            if (_predictor == null)
            {
                _reloadFiles = false;
                if (!File.Exists(ReturnFilePath(_tbPathToDow.Text)) || !File.Exists(ReturnFilePath(_tbPathToNasdaq.Text)) ||
                    !File.Exists(ReturnFilePath(_tbPathToPR.Text)) || !File.Exists(ReturnFilePath(_tbPathToSp.Text)))
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
            // To here are form checks.
            if (_predictor == null)
            {
                _pathToLow = _tbPathToSp.Text;
                _pathToOpen = _tbPathToDow.Text;
                _pathToHigh = _tbPathToNasdaq.Text;
                _pathToClose = _tbPathToPR.Text;
                 Cursor = Cursors.WaitCursor;
                _hiddenLayers = (int)_nudHiddenLayers.Value;
                _hiddenUnits = (int)_nudHiddenUnits.Value;
                try
                {
                    // Create an indicator.
                    _predictor = new PredictIndicators(ReturnFilePath(_pathToLow), ReturnFilePath(_pathToClose), ReturnFilePath(_pathToOpen), ReturnFilePath(_pathToHigh), _hiddenUnits, _hiddenLayers);
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
                _pathToLow = _tbPathToSp.Text;
                _pathToOpen = _tbPathToDow.Text;
                _pathToHigh = _tbPathToNasdaq.Text;
                _pathToClose = _tbPathToPR.Text;
                _predictor.ReloadFiles(ReturnFilePath(_pathToLow), ReturnFilePath(_pathToClose), ReturnFilePath(_pathToOpen), ReturnFilePath(_pathToHigh));
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
        private void BtnPredictClick(object sender, EventArgs e)
        {
            if (_dgvPredictionResults.Rows.Count != 0)
                _dgvPredictionResults.Rows.Clear();

            if (_predictor == null)         /*The network is untrained*/
            {
                _reloadFiles = false;
                if (!File.Exists(ReturnFilePath(_tbPathToDow.Text)) || !File.Exists(ReturnFilePath(_tbPathToNasdaq.Text)) ||
                    !File.Exists(ReturnFilePath(_tbPathToPR.Text)) || !File.Exists(ReturnFilePath(_tbPathToSp.Text)))
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
                            _predictor = new PredictIndicators(ReturnFilePath(_pathToLow), ReturnFilePath(_pathToClose), ReturnFilePath(_pathToOpen), ReturnFilePath(_pathToHigh), _hiddenUnits, _hiddenLayers);
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
                        using (var ofd = new OpenFileDialog() { FileName = "predictor.ntwrk", Filter = Resources.NtwrkFilter })
                        {
                            if (ofd.ShowDialog() == DialogResult.OK)
                            {
                                try
                                {
                                    _predictor.LoadNeuralNetwork(Path.GetFullPath(ReturnFilePath(ofd.FileName)));
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
                _pathToLow = _tbPathToSp.Text;
                _pathToOpen = _tbPathToDow.Text;
                _pathToHigh = _tbPathToNasdaq.Text;
                _pathToClose = _tbPathToPR.Text;
                 Cursor = Cursors.WaitCursor;
                _hiddenLayers = (int)_nudHiddenLayers.Value;
                _hiddenUnits = (int)_nudHiddenUnits.Value;
                try
                {
                    _predictor = new PredictIndicators(ReturnFilePath(_pathToLow), ReturnFilePath(_pathToClose), ReturnFilePath(_pathToOpen), ReturnFilePath(_pathToHigh), _hiddenUnits, _hiddenLayers);
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
                    _pathToLow = _tbPathToSp.Text;
                    _pathToOpen = _tbPathToDow.Text;
                    _pathToHigh = _tbPathToNasdaq.Text;
                    _pathToClose = _tbPathToPR.Text;
                    _predictor.ReloadFiles(ReturnFilePath(_pathToLow), ReturnFilePath(_pathToClose), ReturnFilePath(_pathToOpen), ReturnFilePath(_pathToHigh));
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
                _dgvPredictionResults.Rows.Add(item.Date.TimeOfDay, item.ActualLow,
                                               item.PredictedLow.ToString("F5", CultureInfo.InvariantCulture), item.ActualHigh, item.PredictedHigh.ToString("F5", CultureInfo.InvariantCulture), item.ActualOpen,
                                               item.PredictedOpen.ToString("F5", CultureInfo.InvariantCulture), item.ActualClose, item.PredictedClose.ToString("F5", CultureInfo.InvariantCulture), item.Error.ToString("F4", CultureInfo.InvariantCulture));
            }
        }
        private void MainFormClosing(object sender, FormClosingEventArgs e)
        {
            if(_predictor != null)
                _predictor.AbortTraining();
        }
        private void BtnStopClick(object sender, EventArgs e)
        {
            FadeControls(false);
            _predictor.AbortTraining();
            _btnExport.Enabled = true;
        }
        private void TbPathToLowMouseDoubleClick(object sender, MouseEventArgs e)
        {
            var ofd = new OpenFileDialog { FileName = "low.csv", Filter = Resources.CsvFilter };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            _tbPathToSp.Text = Path.GetFullPath(ofd.FileName);
            _reloadFiles = true;
        }
        private void TbPathToCloseMouseDoubleClick(object sender, MouseEventArgs e)
        {
            var ofd = new OpenFileDialog() { FileName = "close.csv", Filter = Resources.CsvFilter };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            _tbPathToPR.Text = Path.GetFullPath(ofd.FileName);
            _reloadFiles = true;
        }
        private void TbPathToOpenMouseDoubleClick(object sender, MouseEventArgs e)
        {
            var ofd = new OpenFileDialog() { FileName = "open.csv", Filter = Resources.CsvFilter };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            _tbPathToDow.Text = Path.GetFullPath(ofd.FileName);
            _reloadFiles = true;
        }
        private void TbPathToHighMouseDoubleClick(object sender, MouseEventArgs e)
        {
            var ofd = new OpenFileDialog() { FileName = "high.csv", Filter = Resources.CsvFilter };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            _tbPathToNasdaq.Text = Path.GetFullPath(ofd.FileName);
            _reloadFiles = true;
        }
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
        private void BtnExportClick(object sender, EventArgs e)
        {
            using(var sfd = new SaveFileDialog() { FileName = "predictor.ntwrk", Filter = Resources.NtwrkFilter })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _predictor.ExportNeuralNetwork(Path.GetFullPath(ReturnFilePath(sfd.FileName)));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, @"Sum exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.TrendGui, "");
                    }
                }
            }
        }
        private void BtnLoadClick(object sender, EventArgs e)
        {
            if (!File.Exists(ReturnFilePath(_tbPathToDow.Text)) || !File.Exists(ReturnFilePath(_tbPathToNasdaq.Text)) ||
                    !File.Exists(ReturnFilePath(_tbPathToPR.Text)) || !File.Exists(ReturnFilePath(_tbPathToSp.Text)))
            {
                MessageBox.Show(@"No input", @"darn files missing", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    _predictor = new PredictIndicators(ReturnFilePath(_pathToLow), ReturnFilePath(_pathToClose), ReturnFilePath(_pathToOpen), ReturnFilePath(_pathToHigh), _hiddenUnits, _hiddenLayers);
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
            using (var ofd = new OpenFileDialog() { FileName = "predictor.ntwrk", Filter = Resources.NtwrkFilter })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _predictor.LoadNeuralNetwork(Path.GetFullPath(ReturnFilePath(ofd.FileName)));
                        _nudHiddenLayers.Value = _predictor.HiddenLayers;
                        _nudHiddenUnits.Value = _predictor.HiddenUnits;
                    }
                    catch
                    {
                        MessageBox.Show(Resources.ExceptionMessage, Resources.Exception, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Logging.WriteLog(Resources.ExceptionMessage, Logging.LogType.Error, Logging.LogCaller.TrendGui, "");
                    }
                }
            }
        }
        private void BtnSaveResultsClick(object sender, EventArgs e)
        {
            var dgvResults = _dgvPredictionResults;
            var ofd = new SaveFileDialog {Filter = Resources.CsvFilter, FileName = "results.csv"};
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CsvWriter writer;
                try
                {
                    writer = new CsvWriter(ReturnFilePath(ofd.FileName));
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
        #endregion
    }
}
