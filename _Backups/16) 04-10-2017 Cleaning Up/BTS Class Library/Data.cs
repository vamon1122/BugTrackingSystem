using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//if (!/*Some Function*/) { Data.Error(_ErrMsg); };

namespace BTS_Class_Library
{
    public static class Data
    {
        public static string DateTimeToSql(DateTime pNetDateTime)
        {
            string ConvertedDateTime = "CHANGE ME";

            return ConvertedDateTime;
        }

        public static void Error(string pString)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(pString);
            Console.ResetColor();
            Console.Write("(Press enter to dismiss)");
            Console.ReadLine();
        }

        public static string OnlineConnStr = "";
        public static string LocalConnStr = "";
    }
}
