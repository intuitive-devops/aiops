//
// This autonomous intelligent system software is the property of Cartheur Research B.V. Copyright 2022, all rights reserved.
//
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Boagaphish.Numeric;

namespace Boagaphish.Controls
{
    /// <summary>
    /// A chart control to display data.
    /// </summary>
    /// <remarks>The chart control allows to display multiple charts of different types: dots, lines, connected dots.</remarks>
    public class Chart : Control
    {
        private const double Epsilon = 1E-5;
        private readonly Hashtable _dataSeriesTable = new Hashtable();
        private readonly Pen _blackPen = new Pen(Color.Black);
        private readonly Brush _whiteBrush = new SolidBrush(Color.White);
        private DoubleRange _rangeX = new DoubleRange(0, 1);
        private DoubleRange _rangeY = new DoubleRange(0, 1);
        /// <summary>
        /// Chart series type.
        /// </summary>
        public enum SeriesType { Line, Dots, ConnectedDots }
        public int WindowSize { get; set; }
        public int ForecastSize { get; set; }
        // Series data.
        private class DataSeries
        {
            public double[,] Data;
            public Color Color = Color.Blue;
            public SeriesType Type = SeriesType.Line;
            public int Width = 1;
            public bool UpdateYRange = true;
        }
        /// <summary>
        /// The chart's x-range
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
        /// The chart's y-range.
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
        public Chart()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true);
        }
        /// <summary>
        /// Add a data series to the chart
        /// </summary>
        /// <param name="name">The data series name</param>
        /// <param name="color">The data series color</param>
        /// <param name="type">The data series type</param>
        /// <param name="width">The width of the data series delimiter.</param>
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
        /// Add a data series to the chart.
        /// </summary>
        /// <param name="name">The data series name.</param>
        /// <param name="color">The data series color.</param>
        /// <param name="type">The data series type.</param>
        /// <param name="width">The width of the data series delimiter.</param>
        /// <param name="updateYRange">Specifies if <see cref="RangeY"/> should be updated.</param>
        /// <remarks>Adds new empty data series to the collection. The <b>updateYRange</b> parameter specifies if the data series can affect displayable y-range.</remarks>
        public void AddDataSeries(string name, Color color, SeriesType type, int width, bool updateYRange)
        {
            var series = new DataSeries {Color = color, Type = type, Width = width, UpdateYRange = updateYRange};
            _dataSeriesTable.Add(name, series);
        }
        /// <summary>
        /// Update data series on the chart.
        /// </summary>
        /// <param name="name">Data series name to update.</param>
        /// <param name="data">Data series values for the given name.</param>
        public void UpdateDataSeries(string name, double[,] data)
        {
            // Get the data series.
            var series = (DataSeries)_dataSeriesTable[name];
            // Update data.
            series.Data = data;
            // Update Y-range.
            if (series.UpdateYRange)
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
            _dataSeriesTable.Remove(name);
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
        private void UpdateYRange()
        {
            var minY = double.MaxValue;
            var maxY = double.MinValue;
            var en = _dataSeriesTable.GetEnumerator();

            while (en.MoveNext())
            {
                var series = (DataSeries)en.Value;
                var data = series.Data;

                if ((series.UpdateYRange) && (data != null))
                {
                    for (int i = 0, n = data.GetLength(0); i < n; i++)
                    {
                        var v = data[i, 1];
                        if (v > maxY)
                            maxY = v;
                        if (v < minY)
                            minY = v;
                    }
                }
            }
            // Update the y-range if there is any data.
            if ((Math.Abs(minY - double.MaxValue) > Epsilon) || (Math.Abs(maxY - double.MinValue) > Epsilon))
            {
                _rangeY = new DoubleRange(minY, maxY);
            }
        }
        /// <summary>
        /// Paint the control with all available data series.
        /// </summary>
        /// <param name="args">Data for Paint event.</param>
        protected override void OnPaint(PaintEventArgs args)
        {
            var g = args.Graphics;
            var clientWidth = ClientRectangle.Width;
            var clientHeight = ClientRectangle.Height;
            // Fill with a white background.
            g.FillRectangle(_whiteBrush, 0, 0, clientWidth - 1, clientHeight - 1);
            // Draw a black rectangle.
            g.DrawRectangle(_blackPen, 0, 0, clientWidth - 1, clientHeight - 1);
            // Check if there are any series.
            if (_rangeY != null)
            {
                var xFactor = (clientWidth - 10) / (_rangeX.Length);
                var yFactor = (clientHeight - 10) / (_rangeY.Length);
                // Walk through all data series.
                var en = _dataSeriesTable.GetEnumerator();
                while (en.MoveNext())
                {
                    var series = (DataSeries)en.Value;
                    // Get data of the series.
                    var data = series.Data;
                    // Check for available data.
                    if (data == null)
                        continue;
                    // Check series type.
                    if (series.Type == SeriesType.Dots)
                    {
                        // Draw dots.
                        Brush brush = new SolidBrush(series.Color);
                        var width = series.Width;
                        var r = width >> 1;
                        // Draw all points.
                        for (int i = 0, n = data.GetLength(0); i < n; i++)
                        {
                            var x = (int)((data[i, 0] - _rangeX.Min) * xFactor);
                            var y = (int)((data[i, 1] - _rangeY.Min) * yFactor);
                            x += 5;
                            y = clientHeight - 6 - y;
                            g.FillRectangle(brush, x - r, y - r, width, width);
                        }
                        brush.Dispose();
                    }
                    else if (series.Type == SeriesType.ConnectedDots)
                    {
                        // Draw dots connected with 1-pixel width line.
                        Brush brush = new SolidBrush(series.Color);
                        var pen = new Pen(series.Color, 1);
                        var width = series.Width;
                        var r = width >> 1;
                        var x1 = (int)((data[0, 0] - _rangeX.Min) * xFactor);
                        var y1 = (int)((data[0, 1] - _rangeY.Min) * yFactor);
                        x1 += 5;
                        y1 = clientHeight - 6 - y1;
                        g.FillRectangle(brush, x1 - r, y1 - r, width, width);
                        // Draw all lines.
                        for (int i = 1, n = data.GetLength(0); i < n; i++)
                        {
                            var x2 = (int)((data[i, 0] - _rangeX.Min) * xFactor);
                            var y2 = (int)((data[i, 1] - _rangeY.Min) * yFactor);
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
                        // Draw line.
                        var pen = new Pen(series.Color, series.Width);
                        var x1 = (int)((data[0, 0] - _rangeX.Min) * xFactor);
                        var y1 = (int)((data[0, 1] - _rangeY.Min) * yFactor);
                        x1 += 5;
                        y1 = clientHeight - 6 - y1;
                        // Draw all lines.
                        for (int i = 1, n = data.GetLength(0); i < n; i++)
                        {
                            var x2 = (int)((data[i, 0] - _rangeX.Min) * xFactor);
                            var y2 = (int)((data[i, 1] - _rangeY.Min) * yFactor);
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
            // Calling the base class.
            base.OnPaint(args);
        }

        #region Component Designer generated code (VS7.1 created file)
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null)
                    _components.Dispose();

                // free graphics resources
                _blackPen.Dispose();
                _whiteBrush.Dispose();
            }
            base.Dispose(disposing);
        }
        private System.ComponentModel.Container _components;
        private void InitializeComponent()
        {
            _components = new System.ComponentModel.Container();
        }
        #endregion
    }
}
