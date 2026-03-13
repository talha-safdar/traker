using System.IO;

namespace Traker.Services
{
    /// <summary>
    /// Provides static methods and constants for logging application events with various severity levels to a local log
    /// file.
    /// </summary>
    /// <remarks>The <see cref="Logger"/> class defines standard log level constants and a method for writing
    /// log entries to a file in the user's local application data folder. Log entries are timestamped and categorized
    /// by severity. This class is thread-safe for typical usage scenarios.</remarks>
    public static class Logger
    {
        #region Public Static Field Variables
        // these are log levels to indiciate the severity of a log
        public static readonly string FATAL = "FTL"; // for crashes etc.
        public static readonly string ERROR = "ERR"; // for incorrect use of data (e.g. wrong type) etc.
        public static readonly string WARNING = "WRG"; // for something affecting the usability (e.g. network issue) etc.
        public static readonly string INFO = "INF"; // for successful loads up of initialisations, services etc.
        public static readonly string TRACE = "TRC"; // for tracing user inputs etc.
        public static readonly string DEBUG = "DBG"; // for debugging purpose
        #endregion

        #region Public Static Functions
        public static void LogActivity(string level, string text)
        {
            try
            {
                string fileName = $"Log {DateTime.Now:yyyy-MM-dd}.txt"; // file name "Log YYYY-MM-dd.txt"
                string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Traker");
                string logFolder = Path.Combine(basePath, "Logs");
                Directory.CreateDirectory(logFolder);
                string filePath = Path.Combine(logFolder, fileName); // Set file path directory
                File.AppendAllText(filePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} [{level}] {text}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Logger.LogActivity(Logger.ERROR, $"Logger: LogActivity() FAIL\n\t{ex.Message}");
            }
        }
        #endregion
    }
}