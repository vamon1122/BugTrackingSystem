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

        private static User _ActiveUser;
        public static User ActiveUser { get { return _ActiveUser;
            } set {
                _ActiveOrg = null; /*This will also set ActiveProduct to null*/
                _ActiveUser = value;
            }
        }

        private static Organisation _ActiveOrg;
        public static Organisation ActiveOrg { get { return _ActiveOrg; } set { AppLog.Info("Active organisation was set to: " + value.Name); _ActiveProduct = null; _ActiveOrg = value; } }

        private static Product _ActiveProduct;
        public static Product ActiveProduct { get; set; }
        public static List<Bug> ActiveProductBugList = new List<Bug>();
        public static List<Tag> ActiveProductBugTagList = new List<Tag>();
        public static List<Note> ActiveProductBugNoteList = new List<Note>();
        public static List<TagType> ActiveOrgTagTypeList = new List<TagType>();

        public static bool OfflineMode = false;

        

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

        //Home
        /*public static string LocalConnStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='M:\Users\benba\" + 
            @"My Documents\Production\Code Projects\C#\Bug Tracking System\BTS Class Lib + Con Test\" + 
            @"BTS Class Library\bin\Debug\BTS_Local_Db.mdf';Integrated Security = True;";*/

        //Work
        /*public static string LocalConnStr = "Data Source = (LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\" +
            "Ben\\Desktop\\BTS Class Lib + Con Test (Friday 5PM)\\BTS Class Library\\BTS_Local_Db.mdf\";" +
            "Integrated Security = True";*/

        public static string LocalConnStr = "Data Source = (LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"" +
            AppDomain.CurrentDomain.BaseDirectory + "BTS_Local_Db.mdf\"; Integrated Security = True";

        
    }
}
