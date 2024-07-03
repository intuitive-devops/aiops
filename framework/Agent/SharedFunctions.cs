using System;
using System.Reflection;

namespace SoftAgent
{
    /// <summary>
    /// A static class containing commonly-used (shared) functions.
    /// </summary>
    public static class SharedFunctions
    {
        /// <summary>
        /// The application version information.
        /// </summary>
        public static string ApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        /// <summary>
        /// The path to the folder containing the files based on Visual Studio build configuration.
        /// </summary>
        public static string RootFolderPath = Environment.CurrentDirectory;

    }
}
