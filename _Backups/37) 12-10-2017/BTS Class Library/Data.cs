using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenLog;

//if (!/*Some Function*/) { Data.Error(_ErrMsg); };

namespace BTS_Class_Library
{
    public static class Data
    {

        //public static Log MyLog = 

        public static bool OfflineMode = false;

        public static void UserFriendlyError(string pString)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(pString);
            Console.ResetColor();
            Console.Write(" (Press enter to dismiss)");
            Console.ReadLine();
        }

        /*public static string DateTimeToSql(DateTime pDateTime)
        {
            string Day = pDateTime.ToString().Substring(0,2);
            string Month = pDateTime.ToString().Substring(3,2);
            string Year = pDateTime.ToString().Substring(6, 4);

            Console.WriteLine(pDateTime);
            Console.WriteLine("Day = " + Day);
            Console.WriteLine("Month = " + Month);
            Console.WriteLine("Year = " + Year);
            string Time;

            return "";
        }*/

        public static string OnlineConnStr = "Server=tcp:ben.database.windows.net,1433;Initial Catalog=BenDB;" +
            "Persist Security Info=False;User ID=ben;Password=BBTbbt1704;MultipleActiveResultSets=False;" +
            "Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        /*public static string LocalConnStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='M:\Users\benba\" + 
            @"My Documents\Production\Code Projects\C#\Bug Tracking System\BTS Class Lib + Con Test\" + 
            @"BTS Class Library\bin\Debug\BTS_Local_Db.mdf';Integrated Security = True;";*/


        public static string LocalConnStr = "Data Source = (LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\" +
            "Ben\\Desktop\\BTS Class Lib + Con Test (Friday 5PM)\\BTS Class Library\\BTS_Local_Db.mdf\";" +
            "Integrated Security = True";
    }
}
