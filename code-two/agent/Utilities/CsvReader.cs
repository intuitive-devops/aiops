using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SoftAgent.Utilities
{
    /// <summary>
    /// A reader class for csv files.
    /// </summary>
    public class CsvReader : IDisposable
    {
        private readonly string[] _data;
        private readonly TextReader _reader;
        /// <summary>
        /// The names of all of the columns, read from the first line of the file.
        /// </summary>
        private readonly IDictionary<string, int> _columns = new Dictionary<string, int>();
        /// <summary>
        /// Format a date/time object to the same format that we parse in.
        /// </summary>
        /// <param name="date">The date to format.</param>
        /// <returns>A formatted date and time.</returns>
        public static string DisplayDate(DateTime date)
        {
            return date.ToString(CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Parse a date using the specified format.
        /// </summary>
        /// <param name="when">A string that contains a date in the specified format.</param>
        /// <returns>A DateTime that was parsed.</returns>
        public static DateTime ParseDate(string when)
        {
            try
            {
                return DateTime.ParseExact(when, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                return default(DateTime);
            }
        }
        /// <summary>
        /// Construct an object to read the specified CSV file.
        /// </summary>
        /// <param name="filename">The filename to read.</param>
        public CsvReader(string filename)
        {
            _reader = new StreamReader(filename);
            // Read the column headers.
            var line = _reader.ReadLine();
            if (line != null)
            {
                var token = line.Split(',');
                for (var index = 0; index < token.Length; index++)
                {
                    var header = token[index];
                    _columns.Add(header.ToLower().Trim(), index);
                }
                _data = new string[token.Length];
            }
        }
        /// <summary>
        /// Close the file.
        /// </summary>
        public void Close()
        {
            _reader.Close();
        }
        /// <summary>
        /// Get the specified column using an index.
        /// </summary>
        /// <param name="i">The zero based index of the column to read.</param>
        /// <returns>The specified column as a string.</returns>
        public string Get(int i)
        {
            return _data[i];
        }
        /// <summary>
        /// Get the specified column as a string.
        /// </summary>
        /// <param name="column">The specified column.</param>
        /// <returns>The specified column as a string.</returns>
        public string Get(string column)
        {
            if (!_columns.ContainsKey(column.ToLower()))
            {
                return null;
            }
            int i = _columns[column.ToLower()];

            return _data[i];
        }
        /// <summary>
        /// Read the specified column as a date.
        /// </summary>
        /// <param name="column">The specified column.</param>
        /// <returns>The specified column as a DateTime.</returns>
        public DateTime GetDate(string column)
        {
            string str = Get(column);
            return DateTime.Parse(str, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Get the specified column as a double.
        /// </summary>
        /// <param name="column">The column to read.</param>
        /// <returns>The specified column as a double.</returns>
        public double GetDouble(string column)
        {
            string str = Get(column);
            return double.Parse(str, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Get an integer that has the specified name.
        /// </summary>
        /// <param name="col">The column name to read.</param>
        /// <returns>The specified column as an int.</returns>
        public int GetInt(string col)
        {
            string str = Get(col);
            try
            {
                return int.Parse(str, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                return 0;
            }
        }
        /// <summary>
        /// Read the next line.
        /// </summary>
        /// <returns>Return false if there are no more lines in the file.</returns>
        public bool Next()
        {
            string line = _reader.ReadLine();
            if (line == null)
            {
                return false;
            }

            string[] tok = line.Split(',');

            for (int i = 0; i < tok.Length; i++)
            {
                string str = tok[i];
                if (i < _data.Length)
                {
                    _data[i] = str;
                }
            }

            return true;
        }

        #region IDisposable Members
        private bool _alreadydisposed;
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            _alreadydisposed = true;
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (_alreadydisposed) return;
            // Release managed resources.
            if(isDisposing)
            {
                _reader.Dispose();
            }
        }
        /// <summary>
        /// Finalizes an instance of the <see cref="CsvReader"/> class.
        /// </summary>
        ~CsvReader()
        {
            Dispose(false);
        }
        #endregion
    }
}
