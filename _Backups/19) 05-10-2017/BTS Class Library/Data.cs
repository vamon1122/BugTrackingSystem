using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextLogger;

//if (!/*Some Function*/) { Data.Error(_ErrMsg); };

namespace BTS_Class_Library
{
    public static class Data
    {

        public static bool OfflineMode = false;


        public static string DateTimeToSql(DateTime pNetDateTime)
        {
            string ConvertedDateTime = "CHANGE ME";

            return ConvertedDateTime;
        }

        public static void UserFriendlyError(string pString)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(pString);
            Console.ResetColor();
            Console.Write(" (Press enter to dismiss)");
            Console.ReadLine();
        }

        public static string OnlineConnStr = "Server=tcp:ben.database.windows.net,1433;Initial Catalog=BenDB;Persist Security Info=False;User ID=ben;Password=BBTbbt1704;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public static string LocalConnStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\Ben\Desktop\BTS Class Lib + Con Test (Friday 5PM)\BTS Class Library\BTS_Local_Db.mdf';Integrated Security = True;";
    }
}
