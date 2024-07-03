//
// This autonomous intelligent system software is the property of Cartheur Research B.V. Copyright 2022, all rights reserved.
//
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Boagaphish.Numeric;

namespace Boagaphish.Controls
{
    /// <summary>
    /// A chart control to display data using double[].
    /// </summary>
    /// <remarks>The chart control allows to display multiple charts of different types: dots, lines, connected dots.</remarks>
    public class ChartSingle : Control
    {
        /// <summary>
        /// Chart series type.
        /// </summary>
        public enum SeriesType { Line, Dots, ConnectedDots }
        readonly Hashtable _dataSeriesTable = new Hashtable();
        private readonly Pen _blackPen = new Pen(Color.Black);
        private readonly Brush _whiteBrush = new SolidBrush(Color.White);
        private DoubleRange _rangeX = new DoubleRange(0, 1);
        private DoubleRange _rangeY = new DoubleRange(0, 1);
        // Series data.
        private class DataSeries
        {
            public double[][] Data;
            public Color Color = Color.Blue;
            public SeriesType Type = SeriesType.Line;
            public int width = 1;
            public bool updateYRange = true;
        }
        /// <summary>
        /// Chart's x-range
        /// </summary>
        /// <value>The range in the x-coordinate.</value>
        /// <remarks>The value sets the x-range of data to be displayed on the chart.</remarks>
        public DoubleRange RangeX
        {
            get { return _rangeX; }
            set
            {
                _rangeX = value;
                Invalidate();
            }
        }
        /// <summary>
        /// Chart's y-range.
        /// </summary>
        /// <value>The range in the y-coordinate.</value>
        /// <remarks>The value sets the y-range of data to be displayed on the chart.</remarks>
        public DoubleRange RangeY
        {
            get { return _rangeY; }
            set
            {
                _rangeY = value;
                Invalidate();
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Chart"/> class.
        /// </summary>
        public ChartSingle()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // Update control style
            //SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true);
        }
        /// <summary>
        /// Paint the control with all available data series.
        /// </summary>
        /// <param name="args">Data for Paint event.</param>
        protected override void OnPaint(PaintEventArgs args)
        {
            Graphics g = args.Graphics;
            int clientWidth = ClientRectangle.Width;
            int clientHeight = ClientRectangle.Height;

            // fill with white background
            g.FillRectangle(_whiteBrush, 0, 0, clientWidth - 1, clientHeight - 1);

            // draw a black rectangle
            g.DrawRectangle(_blackPen, 0, 0, clientWidth - 1, clientHeight - 1);

            // check if there are any series
            if (_rangeY != null)
            {
                double xFactor = (clientWidth - 10) / (_rangeX.Length);
                double yFactor = (clientHeight - 10) / (_rangeY.Length);

                // walk through all data series
                IDictionaryEnumerator en = _dataSeriesTable.GetEnumerator();
                while (en.MoveNext())
                {
                    var series = (DataSeries)en.Value;
                    // get data of the series
                    double[][] data = series.Data;

                    // check for available data
                    if (data == null)
                        continue;

                    // check series type
                    if (series.Type == SeriesType.Dots)
                    {
                        // draw dots
                        Brush brush = new SolidBrush(series.Color);
                        int width = series.width;
                        int r = width >> 1;

                        // draw all points
                        for (int i = 0, n = data.GetLength(0); i < n; i++)
                        {
                            int x = (int)((data[i][0] - _rangeX.Min) * xFactor);
                            int y = (int)((data[i][1] - _rangeY.Min) * yFactor);

                            x += 5;
                            y = clientHeight - 6 - y;

                            g.FillRectangle(brush, x - r, y - r, width, width);
                        }
                        brush.Dispose();
                    }
                    else if (series.Type == SeriesType.ConnectedDots)
                    {
                        // draw dots connected with 1-pixel width line
                        Brush brush = new SolidBrush(series.Color);
                        Pen pen = new Pen(series.Color, 1);
                        int width = series.width;
                        int r = width >> 1;

                        int x1 = (int)((data[0][0] - _rangeX.Min) * xFactor);
                        int y1 = (int)((data[0][1] - _rangeY.Min) * yFactor);

                        x1 += 5;
                        y1 = clientHeight - 6 - y1;
                        g.FillRectangle(brush, x1 - r, y1 - r, width, width);

                        // draw all lines
                        for (int i = 1, n = data.GetLength(0); i < n; i++)
                        {
                            int x2 = (int)((data[i][0] - _rangeX.Min) * xFactor);
                            int y2 = (int)((data[i][1] - _rangeY.Min) * yFactor);

                            x2 += 5;
                            y2 = clientHeight - 6 - y2;
                            g.FillRectangle(brush, x2 - r, y2 - r, width, width);

                            g.DrawLine(pen, x1, y1, x2, y2);

                            x1 = x2;
                            y1 = y2;
                        }

                        pen.Dispose();
                        brush.Dispose();
                    }
                    else if (series.Type == SeriesType.Line)
                    {
                        // draw line
                        Pen pen = new Pen(series.Color, series.width);

                        int x1 = (int)((data[0][0] - _rangeX.Min) * xFactor);
                        int y1 = (int)((data[0][1] - _rangeY.Min) * yFactor);

                        x1 += 5;
                        y1 = clientHeight - 6 - y1;

                        // draw all lines
                        for (int i = 1, n = data.GetLength(0); i < n; i++)
                        {
                            int x2 = (int)((data[i][0] - _rangeX.Min) * xFactor);
                            int y2 = (int)((data[i][1] - _rangeY.Min) * yFactor);

                            x2 += 5;
                            y2 = clientHeight - 6 - y2;

                            g.DrawLine(pen, x1, y1, x2, y2);

                            x1 = x2;
                            y1 = y2;
                        }
                        pen.Dispose();
                    }
                }
            }

            // Calling the base class OnPaint.
            base.OnPaint(args);
        }
        /// <summary>
        /// Add data series to the chart
        /// </summary>
        /// <param name="name">Data series name</param>
        /// <param name="color">Data series color</param>
        /// <param name="type">Data series type</param>
        /// <param name="width">Width (depends on the data series type, see remarks)</param>
        /// <remarks>Adds new empty data series to the collection of data series. To update this
        /// series the <see cref="UpdateDataSeries"/> method should be used.<br/><br/>
        /// The meaning of the width parameter depends on the data series type:
        /// <list type="bullet">
        /// 		<item><b>Line</b> - width of the line;</item>
        /// 		<item><b>Dots</b> - size of dots (rectangular dots with specified width and the same height);</item>
        /// 		<item><b>Connected dots</b> - size of dots (dots are connected with one pixel width line).</item>
        /// 	</list>
        /// </remarks>
        public void AddDataSeries(string name, Color color, SeriesType type, int width)
        {
            AddDataSeries(name, color, type, width, true);
        }
        /// <summary>
        /// Add data series to the chart.
        /// </summary>
        /// <param name="name">Data series name.</param>
        /// <param name="color">Data series color.</param>
        /// <param name="type">Data series type.</param>
        /// <param name="width">Width (depends on the data series type, see remarks).</param>
        /// <param name="updateYRange">Specifies if <see cref="RangeY"/> should be updated.</param>
        /// <remarks>Adds new empty data series to the collection. The <b>updateYRange</b> parameter specifies if the data series can affect displayable y-range.</remarks>
        public void AddDataSeries(string name, Color color, SeriesType type, int width, bool updateYRange)
        {
            // create new series definition ...
            DataSeries series = new DataSeries();
            // ... add fill it
            series.Color = color;
            series.Type = type;
            series.width = width;
            series.updateYRange = updateYRange;
            // add to series table
            _dataSeriesTable.Add(name, series);
        }
        /// <summary>
        /// Update data series on the chart using double[].
        /// </summary>
        /// <param name="name">Data series name to update.</param>
        /// <param name="data">Data series values.</param>
        public void UpdateDataSeries(string name, double[][] data)
        {
            // Get the data series.
            var series = (DataSeries)_dataSeriesTable[name];
            // Update data.
            series.Data = data;
            // Update Y-range.
            if (series.updateYRange)
                UpdateYRange();
            // Invalidate the control.
            Invalidate();
        }
        /// <summary>
        /// Remove data series from the chart.
        /// </summary>
        /// <param name="name">Data series name.</param>
        public void RemoveDataSeries(string name)
        {
            // remove data series from table
            _dataSeriesTable.Remove(name);
            // invalidate the control
            Invalidate();
        }
        /// <summary>
        /// Remove all data series from the chart.
        /// </summary>
        public void RemoveAllDataSeries()
        {
            _dataSeriesTable.Clear();
            Invalidate();
        }
        /// <summary>
        /// Updates the y-range.
        /// </summary>
        private void UpdateYRange()
        {
            double minY = double.MaxValue;
            double maxY = double.MinValue;

            // Walk through all data series.
            IDictionaryEnumerator en = _dataSeriesTable.GetEnumerator();
            while (en.MoveNext())
            {
                DataSeries series = (DataSeries)en.Value;
                // Get data of the series.
                double[][] data = series.Data;

                if ((series.updateYRange) && (data != null))
                {
                    for (int i = 0, n = data.GetLength(0); i < n; i++)
                    {
                        double v = data[i][1];
                        // Check for max.
                        if (v > maxY)
                            maxY = v;
                        // Check for min.
                        if (v < minY)
                            minY = v;
                    }
                }
            }

            // Update y-range, if there is any data.
            if ((minY != double.MaxValue) || (maxY != double.MinValue))
            {
                _rangeY = new DoubleRange(minY, maxY);
            }
        }

        #region Component Designer generated code
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();

                // free graphics resources
                _blackPen.Dispose();
                _whiteBrush.Dispose();
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion
    }
}
