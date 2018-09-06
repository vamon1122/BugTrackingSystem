using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenLog;
using System.Data.SqlClient;

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
            /////Get active user's organisations start/////
            {
                AppLog.Info("USER ORGANISATIONS (Getter) - Attempting to retrieve user's organisations...");
                if (Data.OfflineMode)
                {
                    using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                    {
                        AppLog.Info("USER ORGANISATIONS (Getter) - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("USER ORGANISATIONS (Getter) - Connection to online database local successfully");
                        AppLog.Info("USER ORGANISATIONS (Getter) - Attempting to retrieve user's organisations from local database...");
                        SqlCommand DownloadUser_Organisations = new SqlCommand("SELECT OrgId FROM OrgMembers WHERE " +
                            "UserId = @UserId", conn);

                        DownloadUser_Organisations.Parameters.Add(new SqlParameter("UserId", ActiveUser.Id));

                        //DownloadUser_Organisations.ExecuteNonQuery();

                        using (SqlDataReader reader = DownloadUser_Organisations.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Organisation TempOrg = new Organisation(new Guid(reader[0].ToString()));
                                if (TempOrg.Get())
                                {
                                    Organisations.Add(TempOrg);
                                    //ActiveUserOrganisations.Add(TempOrg);
                                    //DownloadedOrganisations++;
                                }
                                else
                                {
                                    /*_ErrMsg = "Failed to download organisations for user: " + TempOrg.ErrMsg;
                                    AppLog.Error("USER ORGANISATIONS (Getter) - Get failed while downloading organisations: " + TempOrg.ErrMsg);
                                    */
                                   
                                }

                            }
                        }
                        /*AppLog.Info(String.Format("USER ORGANISATIONS (Getter) - {0} organisations were retrieved for user from local database " +
                            "successfully", DownloadedOrganisations));*/
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("USER ORGANISATIONS (Getter) - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("USER ORGANISATIONS (Getter) - Connection to online database online successfully");
                        AppLog.Info("USER ORGANISATIONS (Getter) - Attempting to retrieve user's organisations from online database...");
                        SqlCommand DownloadUser_Organisations = new SqlCommand("SELECT OrgId FROM t_OrgMembers WHERE " +
                            "UserId = @UserId", conn);

                        DownloadUser_Organisations.Parameters.Add(new SqlParameter("UserId", ActiveUser.Id));

                        //DownloadUser_Organisations.ExecuteNonQuery();

                        using (SqlDataReader reader = DownloadUser_Organisations.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Organisation TempOrg = new Organisation(new Guid(reader[0].ToString()));
                                if (TempOrg.Get())
                                {
                                    Organisations.Add(TempOrg);
                                    AppLog.Info("###Added organisation: " + TempOrg.Name);
                                    //ActiveUserOrganisations.Add(TempOrg);
                                    //DownloadedOrganisations++;
                                }
                                else
                                {
                                    /*_ErrMsg = "Failed to download organisations for user";
                                    AppLog.Error("USER ORGANISATIONS (Getter) " + _ErrMsg + ": " + TempOrg.ErrMsg);*/

                                    
                                }
                            }
                        }
                        /*AppLog.Info(String.Format("USER ORGANISATIONS (Getter) - {0} organisations were retrieved for user from online database " +
                            "successfully", DownloadedOrganisations));*/
                    }
                }
                
            }
            /////Get active user's organisations end/////
                        
            foreach (Organisation TempOrg in Organisations)
            {
                List<Product> TempOrgProducts = new List<Product>();
                
                /////Get organisation's tag types start/////
                {
                    

                    int DownloadedTagTypes = 0;

                    AppLog.Info("ORG TAGTYPES (Getter) - Attempting to retrieve organisation's tagtypes from local database...");

                    if (Data.OfflineMode)
                    {


                        using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                        {
                            AppLog.Info("ORG TAGTYPES (Getter) - Attempting to open connection to local database...");
                            conn.Open();
                            AppLog.Info("ORG TAGTYPES (Getter) - Connection to local database opened successfully");
                            SqlCommand GetTagTypes = new SqlCommand("SELECT Id FROM t_TagTypes WHERE OrgId = @OrgId", conn);
                            GetTagTypes.Parameters.Add(new SqlParameter("OrgId", TempOrg.Id));

                            using (SqlDataReader reader = GetTagTypes.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    TagType TempTagType = new TagType(new Guid(reader[0].ToString()));
                                    if (!TempTagType.Get())
                                    {
                                        //_ErrMsg = "Error while downloading product for organisation: " + TempTagType.ErrMsg;
                                        
                                    }

                                    DownloadedTagTypes++;

                                    TagTypes.Add(TempTagType);
                                }
                            }
                        }
                        AppLog.Info("ORG TAGTYPES (Getter) - TagTypes downloaded from local database successfully");
                    }
                    else
                    {
                        using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                        {
                            AppLog.Info("ORG TAGTYPES (Getter) - Attempting to open connection to online database...");
                            conn.Open();
                            AppLog.Info("ORG TAGTYPES (Getter) - Connection to online database opened successfully");
                            //AppLog.Info(String.Format("ORG TAGTYPES (Getter) - Organisation {0} downloaded from online database successfully", _Name));

                            AppLog.Info("ORG TAGTYPES (Getter) - Attempting to retrieve organisation's tagtypes from online database...");
                            SqlCommand GetTagTypes = new SqlCommand("SELECT Id FROM t_TagTypes WHERE OrgId = @OrgId", conn);
                            GetTagTypes.Parameters.Add(new SqlParameter("OrgId", TempOrg.Id));

                            using (SqlDataReader reader = GetTagTypes.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    TagType TempTagType = new TagType(new Guid(reader[0].ToString()));
                                    if (!TempTagType.Get())
                                    {
                                        //_ErrMsg = "Error while downloading product for organisation: " + TempTagType.ErrMsg;
                                        
                                    }
                                    TagTypes.Add(TempTagType);

                                    DownloadedTagTypes++;
                                }
                            }
                        }
                    }
                    AppLog.Info(String.Format("ORG TAGTYPES (Getter) - {0} product(s) downloaded from online database successfully", DownloadedTagTypes));

                    
                }
                /////Get organisation's tag types end/////



                /////Get organisation's org members start/////
                {

                    if (Data.OfflineMode)
                    {


                        AppLog.Info("ORG ORGMEMBERS (Getter) - Attempting to retrieve organisation's members from local database...");

                        using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                        {
                            AppLog.Info("ORG ORGMEMBERS (Getter) - Attempting to open connection to local database...");
                            conn.Open();
                            AppLog.Info("ORG ORGMEMBERS (Getter) - Connection to local database opened successfully");

                            SqlCommand GetOrgMembers = new SqlCommand("SELECT UserId FROM t_OrgMembers WHERE OrgId = @OrgId", conn);
                            GetOrgMembers.Parameters.Add(new SqlParameter("OrgId", TempOrg.Id));

                            using (SqlDataReader reader = GetOrgMembers.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    User TempUser = new User(new Guid(reader[0].ToString()));
                                    if (!TempUser.Get())
                                    {
                                        //_ErrMsg = "Error while downloading user for organisation: " + TempUser.ErrMsg;
                                        
                                    }
                                    OrgMember TempOrgMember = new OrgMember(TempUser, TempOrg);
                                    if (!TempOrgMember.Get())
                                    {
                                        //_ErrMsg = "Error while downloading OrgMember for organisation: " + TempOrgMember.ErrMsg;
                                        
                                    }
                                    OrgMembers.Add(TempOrgMember);
                                }
                            }
                            AppLog.Info("ORG ORGMEMBERS (Getter) - OrgMembers downloaded from local database successfully");
                        }
                    }
                    else
                    {
                        using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                        {
                            AppLog.Info("ORG PRODUCTS (Getter) - Attempting to open connection to online database...");
                            conn.Open();
                            AppLog.Info("ORG PRODUCTS (Getter) - Connection to online database opened successfully");
                            AppLog.Info("ORG PRODUCTS (Getter) - Attempting to retrieve organisation's members from online database...");
                            SqlCommand GetOrgMembers = new SqlCommand("SELECT UserId FROM t_OrgMembers WHERE OrgId = @OrgId", conn);
                            GetOrgMembers.Parameters.Add(new SqlParameter("OrgId", TempOrg.Id));

                            using (SqlDataReader reader = GetOrgMembers.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    User TempUser = new User(new Guid(reader[0].ToString()));
                                    if (!TempUser.Get())
                                    {
                                        //_ErrMsg = "Error while downloading user for organisation: " + TempUser.ErrMsg;
                                        
                                    }
                                    OrgMember TempOrgMember = new OrgMember(TempUser, TempOrg);
                                    if (!TempOrgMember.Get())
                                    {
                                        //_ErrMsg = "Error while downloading OrgMember for organisation: " + TempOrgMember.ErrMsg;
                                        
                                    }
                                    OrgMembers.Add(TempOrgMember);
                                }
                            }
                        }
                        AppLog.Info("ORG ORGMEMBERS (Getter) - OrgMembers downloaded from online database successfully");
                    }
                    
                }
                /////Get organisation's org members end/////
                


                /////Get organisation's products start/////
                {


                    AppLog.Info("ORG PRODUCTS (Getter) - Attempting to retrieve organisation's products from local database...");

                    if (Data.OfflineMode)
                    {


                        using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                        {
                            AppLog.Info("ORG PRODUCTS (Getter) - Attempting to open connection to local database...");
                            conn.Open();
                            AppLog.Info("ORG PRODUCTS (Getter) - Connection to local database opened successfully");
                            SqlCommand GetProducts = new SqlCommand("SELECT Id FROM t_Products WHERE OrgId = @OrgId", conn);
                            GetProducts.Parameters.Add(new SqlParameter("OrgId", TempOrg.Id));

                            using (SqlDataReader reader = GetProducts.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Product TempProduct = new Product(new Guid(reader[0].ToString()));
                                    if (!TempProduct.Get())
                                    {
                                        //_ErrMsg = "Error while downloading product for organisation: " + TempProduct.ErrMsg;

                                    }


                                    Products.Add(TempProduct);
                                    TempOrgProducts.Add(TempProduct);
                                }
                            }
                        }
                        AppLog.Info("ORG PRODUCTS (Getter) - Products downloaded from local database successfully");
                    }
                    else
                    {
                        using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                        {
                            AppLog.Info("ORG PRODUCTS (Getter) - Attempting to open connection to online database...");
                            conn.Open();
                            AppLog.Info("ORG PRODUCTS (Getter) - Connection to online database opened successfully");
                            //AppLog.Info(String.Format("ORG PRODUCTS (Getter) - Organisation {0} downloaded from online database successfully", _Name));

                            AppLog.Info("ORG PRODUCTS (Getter) - Attempting to retrieve organisation's products from online database...");
                            SqlCommand GetProducts = new SqlCommand("SELECT Id FROM t_Products WHERE OrgId = @OrgId", conn);
                            GetProducts.Parameters.Add(new SqlParameter("OrgId", TempOrg.Id));

                            using (SqlDataReader reader = GetProducts.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Product TempProduct = new Product(new Guid(reader[0].ToString()));
                                    if (!TempProduct.Get())
                                    {
                                        //_ErrMsg = "Error while downloading product for organisation: " + TempProduct.ErrMsg;

                                    }
                                    Products.Add(TempProduct);
                                    TempOrgProducts.Add(TempProduct);


                                }
                            }
                        }
                    }
                    //AppLog.Info(String.Format("ORG PRODUCTS (Getter) - {0} product(s) downloaded from online database successfully", DownloadedProducts));

                }
                /////Get organisation's products end/////

                foreach (Product TempProduct in TempOrgProducts)
                {

                    List<Bug> TempProductBugs = new List<Bug>();
                    /////Get product's bugs start/////
                    {
                        
                        if (Data.OfflineMode)
                        {
                            AppLog.Info("PRODUCT BUGS (Getter) - Attempting to retrieve product's bugs from local database...");

                            using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                            {
                                AppLog.Info("PRODUCT BUGS (Getter) - Attempting to open connection to local database...");
                                conn.Open();
                                AppLog.Info("PRODUCT BUGS (Getter) - Connection to local database opened successfully");

                                SqlCommand GetBugs = new SqlCommand("SELECT Id FROM t_Bugs WHERE ProductId = @ProductId", conn);
                                GetBugs.Parameters.Add(new SqlParameter("ProductId", TempProduct.Id));

                                using (SqlDataReader reader = GetBugs.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        Bug TempBug = new Bug(new Guid(reader[0].ToString()));
                                        if (!TempBug.Get())
                                        {
                                            //_ErrMsg = "Error while downloading bug for product: " + TempBug.ErrMsg;
                                           
                                        }
                                        //Bugs.Add(TempBug);
                                        Bugs.Add(TempBug);
                                        TempProductBugs.Add(TempBug);
                                    }
                                }
                                AppLog.Info("PRODUCT BUGS (Getter) - Bugs downloaded from local database successfully");
                            }
                        }
                        else
                        {
                            using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                            {
                                AppLog.Info("ORG PRODUCTS (Getter) - Attempting to open connection to online database...");
                                conn.Open();
                                AppLog.Info("ORG PRODUCTS (Getter) - Connection to online database opened successfully");
                                AppLog.Info("ORG PRODUCTS (Getter) - Attempting to retrieve organisation's members from online " +
                                    "database...");
                                SqlCommand GetBugs = new SqlCommand("SELECT Id FROM t_Bugs WHERE ProductId = @ProductId", conn);
                                GetBugs.Parameters.Add(new SqlParameter("ProductId", TempProduct.Id));

                                using (SqlDataReader reader = GetBugs.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        Bug TempBug = new Bug(new Guid(reader[0].ToString()));
                                        if (!TempBug.Get())
                                        {
                                            //_ErrMsg = "Error while downloading Bug for product: " + TempBug.ErrMsg;
                                            
                                        }
                                        //Bugs.Add(TempBug);
                                        Bugs.Add(TempBug);
                                        TempProductBugs.Add(TempBug);
                                    }
                                }
                            }
                            AppLog.Info("PRODUCT BUGS (Getter) - Bugs downloaded from online database successfully");
                        }
                        
                    }
                    /////Get product's bugs end/////

                    foreach(Bug TempBug in TempProductBugs)
                    {
                        /////Get bug's assignees start/////
                        {


                            AppLog.Info("INITIALISE - Attempting to retrieve bug's assignees from local database...");





                            if (Data.OfflineMode)
                            {
                                using (SqlConnection conn = new SqlConnection(LocalConnStr))
                                {
                                    AppLog.Info("INITIALISE - Attempting to open connection to local database...");
                                    conn.Open();
                                    AppLog.Info("INITIALISE - Connection to local database opened successfully");

                                    AppLog.Info("INITIALISE - Attempting to retrieve bug's assignees from local database...");
                                    SqlCommand DownloadBug_Assignees = new SqlCommand("SELECT UserId FROM Assignees WHERE BugId = @BugId",
                                        conn);
                                    DownloadBug_Assignees.Parameters.Add(new SqlParameter("BugId", TempBug.Id));

                                    using (SqlDataReader reader = DownloadBug_Assignees.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            User TempUser = new User(new Guid(reader[0].ToString()));

                                            if (!TempUser.Get())
                                            {
                                                throw new Exception("Could not download user from local database (Get assignees for bug)");
                                            }



                                            Assignee TempAssignee = new Assignee(TempBug, TempUser);

                                            if (!TempAssignee.Get())
                                            {
                                                //_ErrMsg = "Error while downloading assignee for bug from local database: " + TempAssignee.ErrMsg;

                                            }
                                            Assignees.Add(TempAssignee);
                                        }
                                    }
                                    /*AppLog.Info(String.Format("INITIALISE - {0} assignees were retrieved for bug from local database " +
                                        "successfully", DownloadedAssignees));*/
                                }
                            }
                            else
                            {
                                using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                                {
                                    AppLog.Info("INITIALISE - Attempting to open connection to online database...");
                                    conn.Open();
                                    AppLog.Info("INITIALISE - Connection to online database opened successfully");

                                    AppLog.Info("INITIALISE - Attempting to retrieve bug's assignees from online database...");
                                    SqlCommand DownloadBug_Assignees = new SqlCommand("SELECT UserId FROM t_Assignees WHERE " +
                                        "BugId = @BugId", conn);
                                    DownloadBug_Assignees.Parameters.Add(new SqlParameter("BugId", TempBug.Id));

                                    using (SqlDataReader reader = DownloadBug_Assignees.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            User TempUser = new User(new Guid(reader[0].ToString()));

                                            if (!TempUser.Get())
                                            {
                                                throw new Exception("Could not download user from online databse (Get assignees for bug)");
                                            }

                                            Assignee TempAssignee = new Assignee(TempBug, TempUser);

                                            if (!TempAssignee.Get())
                                            {
                                                //_ErrMsg = "Error while downloading assignee for bug from online database: " + TempAssignee.ErrMsg;

                                            }
                                            Assignees.Add(TempAssignee);
                                            //DownloadedAssignees++;
                                        }
                                    }
                                    /*AppLog.Info(String.Format("INITIALISE - {0} assignees were retrieved for bug from online database " +
                                        "successfully", DownloadedAssignees));*/

                                }
                            }
                            
                        }
                        /////Get bug's assignees end/////



                        /////Get bug's notes start/////
                        {
                            AppLog.Info("BUG NOTES (Getter) - Attempting to retrieve bug's notes from local database...");
                            

                            if (Data.OfflineMode)
                            {
                                using (SqlConnection conn = new SqlConnection(LocalConnStr))
                                {
                                    AppLog.Info("INITIALISE - Attempting to open connection to local database...");
                                    conn.Open();
                                    AppLog.Info("INITIALISE - Connection to local database opened successfully");

                                    AppLog.Info("INITIALISE - Attempting to retrieve bug's notes from local database...");
                                    SqlCommand DownloadBug_Notes = new SqlCommand("SELECT Id FROM Notes WHERE BugId = @BugId", conn);
                                    DownloadBug_Notes.Parameters.Add(new SqlParameter("BugId", TempBug.Id));
                                    using (SqlDataReader reader = DownloadBug_Notes.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            Notes.Add(new Note(new Guid(reader[0].ToString())));
                                            //DownloadedNotes++;
                                        }
                                    }
                                    /*AppLog.Info(String.Format("INITIALISE - {0} notes were retrieved for bug from local database " +
                                        "successfully", DownloadedNotes));*/

                                }
                            }
                            else
                            {
                                using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                                {
                                    AppLog.Info("INITIALISE - Attempting to open connection to online database...");
                                    conn.Open();
                                    AppLog.Info("INITIALISE - Connection to online database opened successfully");


                                    AppLog.Info("INITIALISE - Attempting to retrieve bug's notes from online database...");
                                    SqlCommand DownloadBug_Notes = new SqlCommand("SELECT Id FROM t_Notes WHERE BugId = @BugId", conn);
                                    DownloadBug_Notes.Parameters.Add(new SqlParameter("BugId", TempBug.Id));
                                    using (SqlDataReader reader = DownloadBug_Notes.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            Note TempNote = new Note(new Guid(reader[0].ToString()));
                                            TempNote.Get();
                                            if (!TempNote.Get())
                                            {
                                                /*_ErrMsg = "Error while downloading note for bug: " + TempNote.ErrMsg;
                                                throw new Exception(_ErrMsg);*/
                                            }
                                            Notes.Add(TempNote);
                                            //DownloadedNotes++;
                                        }
                                    }
                                    /*AppLog.Info(String.Format("INITIALISE - {0} notes were retrieved for bug from online database " +
                                        "successfully", DownloadedNotes));*/
                                }
                            }
                        }
                        /////Get bug's notes end/////



                        /////Get bug's tags start/////
                        {
                            AppLog.Info("BUG ASSIGNEES (Getter) - Attempting to retrieve bug's tags from local database...");

                            
                            

                            if (Data.OfflineMode)
                            {
                                using (SqlConnection conn = new SqlConnection(LocalConnStr))
                                {
                                    AppLog.Info("INITIALISE - Attempting to open connection to local database...");
                                    conn.Open();
                                    AppLog.Info("INITIALISE - Connection to local database opened successfully");

                                    AppLog.Info("INITIALISE - Attempting to retrieve bug's tags from local database...");
                                    SqlCommand DownloadBug_Tags = new SqlCommand("SELECT Id FROM Tags WHERE BugId = @BugId", conn);
                                    DownloadBug_Tags.Parameters.Add(new SqlParameter("BugId", TempBug.Id));
                                    using (SqlDataReader reader = DownloadBug_Tags.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            Tag TempTag = new Tag(new Guid(reader[0].ToString()));
                                            if (!TempTag.Get())
                                            {
                                                //_ErrMsg = "Error while downloading tag for bug: " + TempTag.ErrMsg;
                                                

                                            }
                                            Tags.Add(TempTag);

                                            //AppLog.Debug("###BEN! Temp tag {0} was added to Data.MyTags");
                                            //DownloadedTags++;
                                        }
                                    }
                                    /*AppLog.Info(String.Format("INITIALISE - {0} tags were retrieved for bug from local database " +
                                        "successfully", DownloadedTags));*/

                                }
                            }
                            else
                            {
                                using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                                {
                                    AppLog.Info("INITIALISE - Attempting to open connection to online database...");
                                    conn.Open();
                                    AppLog.Info("INITIALISE - Connection to online database opened successfully");

                                    AppLog.Info("INITIALISE - Attempting to retrieve bug's tags from online database...");
                                    SqlCommand DownloadBug_Tags = new SqlCommand("SELECT Id FROM t_Tags WHERE BugId = @BugId",
                                        conn);
                                    DownloadBug_Tags.Parameters.Add(new SqlParameter("BugId", TempBug.Id));
                                    using (SqlDataReader reader = DownloadBug_Tags.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            Tag TempTag = new Tag(new Guid(reader[0].ToString()));
                                            TempTag.Get();
                                            if (!TempTag.Get())
                                            {
                                                //_ErrMsg = "Error while downloading tag for bug: " + TempTag.ErrMsg;
                                                
                                            }
                                            Tags.Add(TempTag);
                                            //DownloadedTags++;
                                        }
                                    }
                                    /*AppLog.Info(String.Format("INITIALISE - {0} tags were retrieved for bug from online database " +
                                        "successfully", DownloadedTags));*/

                                }
                            }
                        }
                        /////Get bug's tags end/////
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

        /*public static string OnlineConnStr = "Server=tcp:ben.database.windows.net,1433;Initial Catalog=BenDB;" +
            "Persist Security Info=False;User ID=ben;Password=BBTbbt1704;MultipleActiveResultSets=False;" +
            "Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            */

        public static string OnlineConnStr = "Server=198.38.83.33;Database=vamon112_bugtrackingsystem;Uid=vamon112_ben;Password=ccjO07JT";

        //Home
        /*
        public static string LocalConnStr = "Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=\"D:\\Users\\benba" +
            "\\My Documents\\Production\\Coding\\BugTrackingSystem\\Other Tests\\BTS_Local_Db.mdf\";Integrated Security=True";
        */

        //Work
        /*public static string LocalConnStr = "Data Source = (LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\" +
            "Ben\\Desktop\\BTS Class Lib + Con Test (Friday 5PM)\\BTS Class Library\\BTS_Local_Db.mdf\";" +
            "Integrated Security = True";*/

        public static string LocalConnStr = "Data Source = (LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"" +
            AppDomain.CurrentDomain.BaseDirectory + "BTS_Local_Db.mdf\"; Integrated Security = True";

        
    }
}
