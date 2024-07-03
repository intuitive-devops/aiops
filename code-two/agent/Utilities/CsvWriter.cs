using System.IO;
using System.Text;

namespace SoftAgent.Utilities
{
    /// <summary>
    /// Class for writing any object values in comma separated file
    /// </summary>
    public class CsvWriter
    {
        /// <summary>
        /// Char separator
        /// </summary>
        private const char Separator = ',';

        /// <summary>
        /// Path to file to be written
        /// </summary>
        private readonly string _pathToFile;

        /// <summary>
        /// Stream writer
        /// </summary>
        private StreamWriter _writer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pathToFile">Path to filename</param>
        public CsvWriter(string pathToFile)
        {
            _pathToFile = pathToFile;
        }
        /// <summary>
        /// Write the data into csv
        /// </summary>
        /// <param name="data">Data to be written</param>
        public void Write(object[,] data)
        {
            using (_writer = new StreamWriter(_pathToFile))
            {
                int cols = data.GetLength(1);
                for (int i = 0, n = data.GetLength(0); i < n; i++)
                {
                    var builder = new StringBuilder();
                    for (int j = 0; j < cols; j++)
                    {
                        builder.Append(data[i, j]);
                        if (j != cols - 1)
                            builder.Append(Separator);
                    }
                    _writer.WriteLine(builder.ToString());
                }
                _writer.Close();
            }
        }
    }
}
