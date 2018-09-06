using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BenLog
{
    public class Log
    {
        private string _fileName;
        private string _filePath;

        public Log(string fileName)
        {
            _fileName = fileName;
        }

        public Log(string fileName, string fileDirectory)
        {
            _fileName = fileName;
            _filePath = fileDirectory;
        }

        private void WriteLog(string prefix, string value)
        {
            if (_filePath != null)
            {
                if (!System.IO.Directory.Exists(_filePath))
                {
                    System.IO.Directory.CreateDirectory(_filePath);
                }

                if (_filePath.Substring(_filePath.Length - 1, 1) != @"\")
                {
                    _filePath += @"\";
                }
            }

            if (File.Exists(_filePath + _fileName))
            {
                //Console.WriteLine("Log exists!");
                using (StreamWriter log = File.AppendText(_filePath + _fileName))
                {
                    log.WriteLine("<{0}> {1} - {2}", prefix, GetDateTime(), value);
                }
            }
            else
            {
                try
                {
                    using (FileStream fileStream = new FileStream(_filePath + _fileName, FileMode.OpenOrCreate))
                    {

                        using (StreamWriter log = new StreamWriter(fileStream))
                        {
                            log.WriteLine("<info> {0} - Log \"{1}\" did not exist. Log has been created", GetDateTime(), _fileName);
                            log.WriteLine("<{0}> {1} - {2}", prefix, GetDateTime(), value);
                        }
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine("Error while creating log. Directory does not exist!");
                    Console.ReadLine();
                }
                
            }
        }

        private void WriteLog()
        {
            if (_filePath != null)
            {
                if (!System.IO.Directory.Exists(_filePath))
                {
                    System.IO.Directory.CreateDirectory(_filePath);
                }

                if (_filePath.Substring(_filePath.Length - 1, 1) != @"\")
                {
                    _filePath += @"\";
                }
            }

            if (File.Exists(_filePath + _fileName))
            {
                //Console.WriteLine("Log exists!");
                using (StreamWriter log = File.AppendText(_filePath + _fileName))
                {
                    log.WriteLine("");
                }
            }
            else
            {
                using (FileStream fileStream = new FileStream(_filePath + _fileName, FileMode.OpenOrCreate))
                {

                    using (StreamWriter log = new StreamWriter(fileStream))
                    {
                        log.WriteLine("<info> {0} - Log \"{1}\" did not exist. Log has been created", GetDateTime(), _fileName);
                        log.WriteLine("");
                    }
                }
            }
        }

        private static string GetDateTime()
        {
            return System.DateTime.Now.ToString();
        }

        public void Break()
        {
            System.Diagnostics.Debug.WriteLine("");
            WriteLog();
        }

        public void Input(string value)
        {
            System.Diagnostics.Debug.WriteLine("[log - input] " + value);
            WriteLog("input", value);
        }

        public void Output(string value)
        {
            System.Diagnostics.Debug.WriteLine("[log - output] " + value);
            WriteLog("output", value);
        }

        public void Func(string value)
        {
            System.Diagnostics.Debug.WriteLine("[log - func] " + value);
            WriteLog("function", value);
        }

        public void Debug(string value)
        {
            System.Diagnostics.Debug.WriteLine("[log - debug] " + value);
            WriteLog("debug", value);
        }

        public void Info(string value)
        {
            System.Diagnostics.Debug.WriteLine("[log - info] " + value);
            WriteLog("info", value);
        }

        public void Error(string value)
        {
            System.Diagnostics.Debug.WriteLine("[log - error] " + value);
            WriteLog("error", value);
        }
    }

    
}
