using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftAgent
{
    public static class Extensions
    {
        public static double LastMarketValue { get; set; }
        public static T[] Append<T>(this T[] array, T item)
        {
            List<T> list = new List<T>(array);
            list.Add(item);

            return list.ToArray();
        }
        public static string GetNumbers(this string text)
        {
            text = text ?? string.Empty;
            return new string(text.Where(p => char.IsDigit(p)).ToArray());
        }
        /// <summary>
        /// Determines whether this instance has Mega units.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        ///   <c>true</c> if the specified text is mega; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMega(this string text)
        {
            text = text ?? string.Empty;
            return text.Contains('M');
        }
        /// <summary>
        /// Determines whether this instance has Giga units.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        ///   <c>true</c> if the specified text is giga; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsGiga(this string text)
        {
            text = text ?? string.Empty;
            return text.Contains('G');
        }
        public static List<string> RenderTrend(this List<double> dataPoints)
        {
            //var inputs = new double[dataPoints.Count];
            //Array.Copy(dataPoints, inputs, dataPoints.Count);
            var inputs = dataPoints.ToList();
            var truthTable = new List<string>();
            // Build a parseable list.
            for (var i = 0; i < dataPoints.Count - 1; i++)
            {
                if (inputs[i] > dataPoints[i + 1])
                {
                    truthTable.Add("Point " + i + ": Trend upward.");
                }
                if (inputs[i] < dataPoints[i + 1])
                {
                    truthTable.Add("Point " + i + ": Trend downward.");
                }
            }
            return truthTable;
        }
        public static List<string> RenderTrendStrings(this double[,] dataPoints)
        {
            var inputs = new double[dataPoints.Length];
            Array.Copy(dataPoints, inputs, dataPoints.Length);
            var truthTable = new List<string>();
            // Build a parseable list.
            for (var i = 0; i < dataPoints.Length - 1; i++)
            {
                if (inputs[i] > dataPoints[i + 1, 1])
                {
                    truthTable.Add("Point " + i + ": Trend upward.");
                }
                if (inputs[i] < dataPoints[i + 1, 1])
                {
                    truthTable.Add("Point " + i + ": Trend downward.");
                }
            }
            return truthTable;
        }
        public static List<bool> RenderTrend(this double[] dataPoints)
        {
            var inputs = new double[dataPoints.Length];
            Array.Copy(dataPoints, inputs, dataPoints.Length);
            var truthTable = new List<bool>();
            // Build a parseable list.
            for (var i = 0; i < dataPoints.Length - 1; i++)
            {
                if (inputs[i] > dataPoints[i + 1])
                {
                    truthTable.Add(true);// An upward trend.
                }
                if (inputs[i] < dataPoints[i + 1])
                {
                    truthTable.Add(false);// A downward trend.
                }
            }
            return truthTable;

        }
        public static List<bool> RenderTrend(this double[,] dataPoints)
        {
            var inputs = new double[dataPoints.GetUpperBound(0), 2];
            Array.Copy(dataPoints, inputs, inputs.Length);
            var truthTable = new List<bool>();
            // Build a parseable list.
            var list = new List<double>();
            foreach (var point in inputs)
            {
                list.Add(point);
            }
            list.RemoveAt(0);

            for (var i = inputs.Length / 4; i < inputs.Length / 2; i--)
            {
                if (i > 0)
                {
                    if (inputs[i, 1] > dataPoints[i - 1, 1])
                    {
                        // Check values.
                        var first = inputs[i, 1];
                        var next = dataPoints[i - 1, 1];
                        var directionUpward = Boagaphish.Core.Extensions.IsGreaterThan(first, next);
                        if (directionUpward)
                            truthTable.Add(true);// An upward trend.
                    }
                    if (inputs[i, 1] < dataPoints[i - 1, 1])
                    {
                        // Check values.
                        var first = inputs[i, 1];
                        var next = dataPoints[i - 1, 1];
                        var directionDownward = Boagaphish.Core.Extensions.IsGreaterThan(first, next);
                        if (directionDownward)
                            truthTable.Add(false);// A downward trend.
                    }
                }
            }

            return truthTable;

        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        public static T[,] SubArray<T>(this T[,] solution, int index, int lengthI, int lengthJ)
        {
            var result = new T[lengthI, lengthJ];
            Array.Copy(solution, index, result, 0, lengthI);
            return result;
        }
        
        // Using now??
        public static List<double> GetLastValuesOf(this double[,] value, int predictionSize)
        {
            var output = new List<double>();
            var last = value.GetUpperBound(0);
            for (var i = last; i > last - predictionSize; i--)
            {
                output.Add(value[i, 1]);
            }
            return output;
        }
        public static List<bool> IsGreaterThanPrevious(this List<double> input, int numberOfPoints)
        {
            var result = new List<bool>();
            for (var i = 0; i < numberOfPoints - 1; i++)
            {
                var value = input[i];
                var previousValue = input[i + 1];
                if (value > previousValue)
                    result.Add(true);
                else
                {
                    result.Add(false);
                }
            }
            result.Reverse();
            return result;
        }
        public static bool IsGreaterThanPreviousByMagnitude(this double forecastValue, double differenceMagnitude)
        {
            var magnitude = Math.Abs(forecastValue - LastMarketValue);
            if (forecastValue > LastMarketValue)
            {
                if (magnitude >= differenceMagnitude)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            return false;
        }
        public static bool IsLessThanPreviousByMagnitude(this double forecastValue, double differenceMagnitude)
        {
            var magnitude = Math.Abs(forecastValue - LastMarketValue);
            if (forecastValue < LastMarketValue)
            {
                if (magnitude >= differenceMagnitude)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            return false;
        }
        public static T[,] CombineArray<T>(this T[,] first, T[,] second)
        {
            int j = 1;
            var newArray = new T[first.GetUpperBound(0) + second.GetUpperBound(0) + 2, first.GetUpperBound(1) + second.GetUpperBound(1)];
            Array.Copy(first, newArray, first.Length);
            Array.Copy(second, 0, newArray, first.Length, second.Length);
            var value = Convert.ToInt32(first[first.GetUpperBound(0), 0]);
            for (int i = first.GetUpperBound(0) + 1, n = newArray.GetUpperBound(0) + 1; i < n; i++)
            {
                newArray.SetValue(value + j, i, 0);
                j++;
            }
            return newArray;
        }
        public static List<double> GetPredictionWindowValues(this double[] value, int predictionSize)
        {
            var output = new List<double>();
            var last = value.GetUpperBound(0);
            for (var i = last; i > last - predictionSize; i--)
            {
                output.Add(value[i]);
            }

            return output;
        }
        public static int MaximumFalseInSequence(this IList<bool> value)
        {
            int maximumFalse = 0;
            for (int i = 0; i < value.Count - 1; i++)
            {
                var current = value[i];
                if (current == false && value[i + 1] == false)
                {
                    maximumFalse++;
                }
            }
            return maximumFalse;
        }
        public static int MaximumTrueInSequence(this IList<bool> value)
        {
            int maximumTrue = 0;
            for (int i = 0; i < value.Count - 1; i++)
            {
                var current = value[i];
                if (current && value[i + 1])
                {
                    maximumTrue++;
                }
            }
            return maximumTrue;
        }

        public static Tuple<int, int> CoordinatesOf<T>(this T[,] input, T value)
        {
            int w = input.GetLength(0); // width
            int h = input.GetLength(1); // height

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    if (input[x, y].Equals(value))
                        return Tuple.Create(x, y);
                }
            }

            return Tuple.Create(-1, -1);
        }
        public static List<Tuple<double[,], int, int>> GetEndPoints<T>(this T[,] solution, T numberOfPoints)
        {
            int w = solution.GetLength(0); // width
            int h = solution.GetLength(1); // height
            var output = new double[,] { };
            var endPoints = new List<Tuple<double[,], int, int>>();

            for (var i = 0; i < solution.Length; i++)
            {
                endPoints.Add(new Tuple<double[,], int, int>(output, Convert.ToInt32(solution.GetValue(w)), Convert.ToInt32(solution.GetValue(h))));
            }

            //for (int x = 0; x < w; ++x)
            //{
            //    for (int y = 0; y < h; ++y)
            //    {
            //        if (solution[x, y].Equals(numberOfPoints))
            //            return Tuple<>.Create(x, y);
            //    }
            //}

            return endPoints;
        }
        public static T[,] ResizeArray<T>(this T[,] original, int rows, int cols)
        {
            var newArray = new T[rows, cols];
            int minRows = Math.Min(rows, original.GetLength(0));
            int minCols = Math.Min(cols, original.GetLength(1));
            for (int i = 0; i < minRows; i++)
                for (int j = 0; j < minCols; j++)
                    newArray[i, j] = original[i, j];
            return newArray;
        }
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
        // Code purely for forecasting.
        public static double AverageMean(this IList<Tuple<double, double, double>> dataPoints)
        {
            // Assume for first draft that no datapoint is more important than the other. Order is data, forecast, absolute difference.
            var mean = 0.0;
            foreach (var dataPoint in dataPoints)
            {
                mean += dataPoint.Item2;
            }
            mean = mean/dataPoints.Count;
            return mean;
        }
        public static double PickLastForecast(this IList<Tuple<double, double, double>> dataPoints)
        {
            // Pick the last value on the list assuming it has the least difference. Can make more sophisicated as a optimization.
            var lastValue = dataPoints[dataPoints.Count - 1];
            var nextMean = lastValue.Item2;
            LastMarketValue = lastValue.Item1;

            return nextMean;
        }

        #region Idea
        //public static List<bool> RenderTrend(this double[,] dataPoints)
        //{
        //    var inputs = new double[dataPoints.GetUpperBound(0), 2];
        //    Array.Copy(dataPoints, inputs, inputs.Length);
        //    var truthTable = new List<bool>();
        //    // Build a parseable list.
        //    for (var i = inputs.Length / 4; i < inputs.Length / 2; i--)
        //    {
        //        if (i > 0)
        //        {
        //            if (inputs[i, 1] > dataPoints[i - 1, 1])
        //            {
        //                // Check values.
        //                var first = inputs[i, 1];
        //                var next = dataPoints[i - 1, 1];
        //                var directionUpward = IsGreaterThanPrevious(first, next);
        //                if (directionUpward)
        //                    truthTable.Add(true);// An upward trend.
        //            }
        //            if (inputs[i, 1] < dataPoints[i - 1, 1])
        //            {
        //                // Check values.
        //                var first = inputs[i, 1];
        //                var next = dataPoints[i - 1, 1];
        //                var directionDownward = IsGreaterThanPrevious(first, next);
        //                if (directionDownward)
        //                    truthTable.Add(false);// A downward trend.
        //            }
        //        }
        //    }

        //    return truthTable;

        //}
        #endregion
    }
}
