using System.IO;

namespace Traker.Services
{
    /// <summary>
    /// Creates log file as "Log YYY-MM-dd.txt".
    /// It also shows the log level.
    /// </summary>
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

        private static readonly object _logLock = new();
        #endregion

        #region Public Static Functions
        public static void LogActivity(string level, string text)
        {
            try
            {
                string fileName = $"Log {DateTime.Now:yyyy-MM-dd}.txt"; // file name "Log YYYY-MM-dd.txt"
                string LogDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Log");
                Directory.CreateDirectory(LogDirectory);

                string filePath = Path.Combine(LogDirectory, fileName); // Set file path directory

                lock (_logLock)
                {
                    // add file path, and the message with timestamp
                    File.AppendAllText(filePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} [{level}] {text}{Environment.NewLine}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogActivity(Logger.ERROR, $"Logger: LogActivity() FAIL\n\t{ex.Message}");
            }
        }
        #endregion
    }
}