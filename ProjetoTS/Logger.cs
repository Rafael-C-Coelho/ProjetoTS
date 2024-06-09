using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Logger
    {
        public string logFilePath { get; set; }

        // Verifica se existe um directório.Caso não exista, cria-o.Este diretório servirá para armazenar o ficheiro de logs
        public Logger(string logFilename)
        {
            if (!Directory.Exists(Path.GetDirectoryName(GetPathString(logFilename))))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(GetPathString(logFilename)));
            }
            this.logFilePath = GetPathString(logFilename);
        }

        //o método recebe o nome do ficheiro e combina todo o caminho do diretório, bem como dos dois subdiretórios "ProjetoTS" e "Logs", e o nome do ficheiro log.
        //Por fim,retorna o caminho completo do ficheiro  
        public static string GetPathString(string filename)
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ProjetoTS",
                "Logs",
                filename
            );
        }

        //método que recebe a mensagem e o tipo de log, e acrescenta a data e hora no ficheiro do log 
        private void Log(string message, string type)
        {
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine($"[{DateTime.Now}] - [{type}] - {message}");
            }
        }

        //métodos de logging que permitem efetuar o registo de diferentes tipos de eventos e informações que ocorrem durante a execução da aplicação 
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

        public void Error(Exception ex, string message)
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
