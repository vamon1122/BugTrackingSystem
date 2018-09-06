using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenLog
{
    public static class AppLog
    {
        private static Log DefaultLog = new Log("log.txt", AppDomain.CurrentDomain.BaseDirectory);
        
        //Work
        //private static Log DefaultLog = new Log(@"C:\Users\Ben\Desktop\BTS Class Lib + Con Test (Friday 5PM)\log.txt");

        //Home
        //private static Log DefaultLog = new Log(@"M:\Users\benba\My Documents\Production\Code Projects\C#\Bug Tracking System\BTS Class Lib + Con Test\log.txt");



        public static void Break()
        {
            
            DefaultLog.Break();
            
        }

        public static void Input(string value)
        {
            DefaultLog.Input(value);
            
        }

        public static void Output(string value)
        {
            DefaultLog.Output(value);
        }

        public static void Func(string value)
        {
            DefaultLog.Func(value);   
        }

        public static void Debug(string value)
        {
            DefaultLog.Debug(value);   
        }

        public static void Info(string value)
        {
            DefaultLog.Info(value);   
        }

        public static void Error(string value)
        {
            DefaultLog.Error(value);
        }
    }
}
