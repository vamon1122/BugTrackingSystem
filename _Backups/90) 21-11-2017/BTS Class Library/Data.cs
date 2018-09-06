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
        public static Organisation ActiveOrg { get { return _ActiveOrg; }
            set {
                if (value == null) {
                    AppLog.Info("Active organisation was set to: null");
                }
                else {
                    AppLog.Info("Active organisation was set to: " + value.Name);
                    _ActiveProduct = null; _ActiveOrg = value;
                }
            }
        }

        private static Product _ActiveProduct;

        public static Product ActiveProduct { get; set; }
        /*public static List<Bug> ActiveProductBugList = new List<Bug>();
        public static List<Tag> ActiveProductBugTagList = new List<Tag>();
        public static List<Note> ActiveProductBugNoteList = new List<Note>();
        public static List<TagType> ActiveOrgTagTypeList = new List<TagType>();*/

        public static List<Organisation> Organisations = new List<Organisation>();
        public static List<Bug> Bugs = new List<Bug>();
        public static List<Tag> Tags = new List<Tag>();
        public static List<Note> Notes = new List<Note>();
        public static List<Assignee> Assignees = new List<Assignee>();
        public static List<TagType> TagTypes = new List<TagType>();
        public static List<Product> Products = new List<Product>();
        public static List<OrgMember> OrgMembers = new List<OrgMember>();

        public static bool OfflineMode = false;

        public static string InitialiseProgress;

        public static void Initialise()
        {
            foreach(Organisation TempOrg in ActiveUser.Organisations)
            {
                Organisations.Add(TempOrg);

                foreach(TagType TempTag in TempOrg.TagTypes)
                {
                    TagTypes.Add(TempTag);
                }

                foreach(OrgMember TempOrgMember in TempOrg.Members)
                {
                    OrgMembers.Add(TempOrgMember);
                }

                foreach(Product TempProduct in TempOrg.Products)
                {
                    Products.Add(TempProduct);

                    foreach(Bug TempBug in TempProduct.Bugs)
                    {
                        Bugs.Add(TempBug);

                        foreach(Note TempNote in TempBug.Notes)
                        {
                            Notes.Add(TempNote);
                        }

                        foreach(Assignee TempAssignee in TempBug.Assignees)
                        {
                            Assignees.Add(TempAssignee);
                        }

                        foreach(Tag TempTag in TempBug.Tags)
                        {
                            Tags.Add(TempTag);
                        }
                    }
                }
            }

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
