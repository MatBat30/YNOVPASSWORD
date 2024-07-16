#nullable enable
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LoggerLibrary
{
    public static class Logger
    {
        private static StreamWriter? _logWriter;
        private static readonly CultureInfo Culture = new("fr-FR"); // Changed from "fr_FR" to "fr-FR"
        private static string LogFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "YnovPassword",
            "logs",
            DateTime.Now.ToString("yyyy-MM"),
            DateTime.Now.ToString("yyyy-MM-dd_HH") + ".txt"
        );

        public static async Task InitializeAsync()
        {
            var logDir = Path.GetDirectoryName(LogFilePath) ?? throw new InvalidOperationException("Log directory path cannot be null.");
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
                Console.WriteLine($"Created directory: {logDir}");
            }

            _logWriter = new StreamWriter(new FileStream(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                AutoFlush = true
            };

            // Log the path of the log file to the console
            Console.WriteLine($"Log file created at: {LogFilePath}");

            await Task.CompletedTask;  // To handle the async warning
        }

        public static void Debug(string message)
        {
            if (Debugger.IsAttached)
            {
                WriteLog("D", message);
            }
        }

        public static void Error(string message)
        {
            WriteLog("E", message);
            WriteEventLog("Error", message);
        }

        public static void Warning(string message)
        {
             WriteLog("W", message);
            WriteEventLog("Warning", message);
        }

        public static void Info(string message)
        {
            WriteLog("I", message);
        }

        private static void WriteLog(string type, string message)
        {
            var timestamp = DateTime.Now.ToString("yyyy/MM/dd_HH-mm-ss", Culture);
            var logMessage = $"{timestamp}: <{type}>: {message}";

            Console.WriteLine($"Writing log message: {logMessage}");

            // Vérifier si le fichier de log actuel a changé
            if (_logWriter?.BaseStream is FileStream fs && fs.Name != LogFilePath)
            {
                _logWriter?.Dispose();
                InitializeAsync().Wait();
            }

            _logWriter?.WriteLine(logMessage);
        }

        private static void WriteEventLog(string type, string message)
        {
            #if WINDOWS
            const string source = ".NET Runtime";
            const string log = "Application";

            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, log);
            }

            EventLogEntryType entryType = type switch
            {
                "Error" => EventLogEntryType.Error,
                "Warning" => EventLogEntryType.Warning,
                _ => EventLogEntryType.Information
            };

            EventLog.WriteEntry(source, message, entryType, 1000);
            #else
            Console.WriteLine($"EventLog: {type} - {message}");
            #endif
        }

        public static async Task DisposeAsync()
        {
            if (_logWriter != null)
            {
                await _logWriter.DisposeAsync();
            }
        }

        public static void DeleteLogs(int days)
        {
            var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "YnovPassword", "logs");

            if (Directory.Exists(logDir))
            {
                var files = Directory.GetFiles(logDir, "*", SearchOption.AllDirectories)
                    .Select(f => new FileInfo(f))
                    .Where(f => f.LastWriteTime < DateTime.Now.AddDays(-days));

                foreach (var file in files)
                {
                    file.Delete();
                }

                // Optionally, delete empty directories
                foreach (var dir in Directory.GetDirectories(logDir, "*", SearchOption.AllDirectories))
                {
                    if (!Directory.EnumerateFileSystemEntries(dir).Any())
                    {
                        Directory.Delete(dir, false);
                    }
                }
            }
        }
    }
}
