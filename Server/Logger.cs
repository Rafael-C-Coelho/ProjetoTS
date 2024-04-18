using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Logger
    {
        public string logFilePath { get; set; }

        public Logger(string logFilename)
        {
            this.logFilePath = GetPathString(logFilename);
        }

        public static string GetPathString(string filename)
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ProjetoTS",
                "Logs",
                filename
            );
        }

        private void Log(string message, string type)
        {
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine($"[{DateTime.Now}] - [{type}] - {message}");
            }
        }

        public void Debug(string message)
        {
            Log(message, "DEBUG");
        }

        public void Info(string message)
        {
            Log(message, "INFO");
        }

        public void Warn(string message)
        {
            Log(message, "WARN");
        }

        public void Error(string message)
        {
            Log(message, "ERROR");
        }

        public void Fatal(string message)
        {
            Log(message, "FATAL");
        }

        public void Exception(Exception ex)
        {
            Log(ex.Message, "EXCEPTION");
        }
    }
}
