using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenLog;
using System.Data.SqlClient;

namespace BTS_Class_Library
{
    static class PrivateData
    {
        internal static List<Bug> Bugs = new List<Bug>();
        internal static List<Tag> Tags = new List<Tag>();
        internal static List<Note> Notes = new List<Note>();
        internal static List<Assignee> Assignees = new List<Assignee>();
        internal static List<TagType> TagTypes = new List<TagType>();
        internal static List<Product> Products = new List<Product>();
        internal static List<OrgMember> OrgMembers = new List<OrgMember>();
        internal static List<Organisation> Organisations = new List<Organisation>();
        internal static List<User> Users = new List<User>();
    }

    public static class Data
    {
        #region Private Data
        public static void AddBug(Bug pBug)
        {
            PrivateData.Bugs.Add(pBug);
        }

        public static List<Bug> Bugs { get { return PrivateData.Bugs; } }

        public static void AddTag(Tag pTag)
        {
            PrivateData.Tags.Add(pTag);
        }

        public static List<Tag> Tags { get { return PrivateData.Tags; } }

        public static void AddNote(Note pNote)
        {
            PrivateData.Notes.Add(pNote);
        }

        public static List<Note> Notes { get { return PrivateData.Notes; } }

        public static void AddAssignee(Assignee pAssignee)
        {
            PrivateData.Assignees.Add(pAssignee);
        }

        public static List<Assignee> Assignees { get { return PrivateData.Assignees; } }

        public static void AddTagType(TagType pTagType)
        {
            PrivateData.TagTypes.Add(pTagType);
        }

        public static List<TagType> TagTypes { get { return PrivateData.TagTypes; } }

        public static void AddProduct(Product pProduct)
        {
            PrivateData.Products.Add(pProduct);
        }

        public static List<Product> Products { get { return PrivateData.Products; } }

        public static void AddOrgMember(OrgMember pOrgMember)
        {
            PrivateData.OrgMembers.Add(pOrgMember);
        }

        public static List<OrgMember> OrgMembers { get { return PrivateData.OrgMembers; } }

        public static void AddOrganisation(Organisation pOrganisation)
        {
            PrivateData.Organisations.Add(pOrganisation);
        }

        public static List<Organisation> Organisations { get { return PrivateData.Organisations; } }

        public static void AddUser(User pUser)
        {
            PrivateData.Users.Add(pUser);
        }

        public static List<User> Users { get { return PrivateData.Users; } }


        #endregion

        private static User _ActiveUser;

        public static User ActiveUser {
            get
            {
                return _ActiveUser;
            }
            set {
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

        public static bool OfflineMode = false;

        public static string InitialiseProgress;

        public static void Initialise()
        {
            if (!Data.Users.Any(user => user.Id.ToString() == _ActiveUser.Id.ToString()))
            {
                AddUser(_ActiveUser);
            }

            using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
            {
                conn.Open();



                GetUserOrgs();
                GetOrgMembers();
                GetOrgMemberUsers();
                GetOrgTagTypes();
                GetOrgProducts();
                GetOrgBugs();
                GetBugAssignees();
                GetBugTags();
                GetBugNotes();

                bool GetUserOrgs()
                {

                    SqlCommand SelectUsersOrgMembersips = new SqlCommand("SELECT OrgId FROM t_OrgMembers WHERE " +
                        "UserId = @UserId", conn);

                    SelectUsersOrgMembersips.Parameters.Add(new SqlParameter("UserId", ActiveUser.Id));

                    int NoOfOrgMemberships = 0;
                    List<Guid> UsersOrgIds = new List<Guid>();

                    using (SqlDataReader UsersOrgMembershipsReader = SelectUsersOrgMembersips.ExecuteReader())
                    {
                        while (UsersOrgMembershipsReader.Read())
                        {
                            NoOfOrgMemberships++;
                            UsersOrgIds.Add(Guid.Parse(UsersOrgMembershipsReader[0].ToString()));
                        }
                    }

                    AppLog.Debug(String.Format("{0} is a member of {1} organisation(s). Attempting to download these organisations...", ActiveUser.FullName, NoOfOrgMemberships));

                    int NoOfOrgsDownloaded = 0;

                    foreach (Guid UserOrgId in UsersOrgIds)
                    {
                        SqlCommand SelectOrganisations = new SqlCommand("SELECT * FROM t_Organisations WHERE Id = @Id;", conn);
                        SelectOrganisations.Parameters.Add(new SqlParameter("Id", UserOrgId.ToString()));

                        Organisation TempOrg;

                        using (SqlDataReader OrgReader = SelectOrganisations.ExecuteReader())
                        {
                            if (OrgReader.Read())
                            {
                                AddOrganisation(new Organisation(new Guid(UserOrgId.ToString()), OrgReader[1].ToString().Trim(), Convert.ToDateTime(OrgReader[2])));
                                NoOfOrgsDownloaded++;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }

                    AppLog.Debug(String.Format("Success! The {0}/{1} of the organisations which {2} is a member of were downloaded.", NoOfOrgsDownloaded, NoOfOrgMemberships, ActiveUser.FullName));
                    return true;
                }

                bool GetOrgMembers()
                {
                    foreach (Organisation org in Organisations)
                    {
                        SqlCommand GetOrganisationMembers = new SqlCommand("SELECT * FROM t_OrgMembers WHERE OrgId = @OrgId", conn);
                        GetOrganisationMembers.Parameters.Add(new SqlParameter("OrgId", org.Id));

                        int NoOfOrgMembersDownloaded = 0;

                        using (SqlDataReader ReadOrgansiationMembers = GetOrganisationMembers.ExecuteReader())
                        {
                            while (ReadOrgansiationMembers.Read())
                            {
                                AddOrgMember(new OrgMember(Guid.Parse(ReadOrgansiationMembers[1].ToString()), org.Id, Convert.ToDateTime(ReadOrgansiationMembers[2])));
                            }
                        }
                        NoOfOrgMembersDownloaded++;
                        AppLog.Debug(String.Format("Found {0} member(s) of {1}", NoOfOrgMembersDownloaded, org.Name));
                    }
                    return true;
                }

                bool GetOrgMemberUsers()
                {
                    foreach (OrgMember orgmemb in OrgMembers)
                    {
                        SqlCommand GetUsers = new SqlCommand("SELECT * FROM t_Users WHERE Id = @UserId", conn);
                        GetUsers.Parameters.Add(new SqlParameter("UserId", orgmemb.UserId));

                        int NoOfOrgMemberUsersDownloaded = 0;

                        using (SqlDataReader ReadOrgansiationMemberUsers = GetUsers.ExecuteReader())
                        {
                            while (ReadOrgansiationMemberUsers.Read())
                            {
                                User TempUser = new User(new Guid(ReadOrgansiationMemberUsers[0].ToString()), ReadOrgansiationMemberUsers[1].ToString().Trim(), ReadOrgansiationMemberUsers[2].ToString().Trim(), ReadOrgansiationMemberUsers[5].ToString().Trim(), ReadOrgansiationMemberUsers[6].ToString().Trim(), ReadOrgansiationMemberUsers[7].ToString().Trim(), ReadOrgansiationMemberUsers[8].ToString().Trim(), Convert.ToDateTime(ReadOrgansiationMemberUsers[12]));

                                if (ReadOrgansiationMemberUsers[3] != DBNull.Value)
                                {
                                    TempUser.DOB = Convert.ToDateTime(ReadOrgansiationMemberUsers[3]);
                                }

                                if (ReadOrgansiationMemberUsers[4] != DBNull.Value)
                                {
                                    TempUser.Gender = ReadOrgansiationMemberUsers[4].ToString().ToCharArray()[0];
                                }

                                if (ReadOrgansiationMemberUsers[9] != DBNull.Value)
                                {
                                    TempUser.Phone = ReadOrgansiationMemberUsers[9].ToString().Trim();
                                }

                                if (ReadOrgansiationMemberUsers[10] != DBNull.Value)
                                {
                                    TempUser.Phone = ReadOrgansiationMemberUsers[10].ToString().Trim();
                                }

                                if (ReadOrgansiationMemberUsers[11] != DBNull.Value)
                                {
                                    TempUser.Phone = ReadOrgansiationMemberUsers[11].ToString().Trim();
                                }

                                if(!Data.Users.Any(user => user.Id.ToString() == TempUser.Id.ToString()))
                                {
                                    AddUser(TempUser);
                                }
                            }
                        }
                        NoOfOrgMemberUsersDownloaded++;
                        AppLog.Debug(String.Format("Found {0} users(s)", NoOfOrgMemberUsersDownloaded));
                    }
                    AppLog.Debug("There are " + Data.Users.Count() + "users!");
                    return true;
                }

                bool GetOrgTagTypes()
                {
                    foreach (Organisation org in Organisations)
                    {
                        SqlCommand GetTagTypes = new SqlCommand("SELECT * FROM t_TagTypes WHERE OrgId = @OrgId", conn);
                        GetTagTypes.Parameters.Add(new SqlParameter("OrgId", org.Id));

                        int NoOfDownloadedTagTypes = 0;

                        using (SqlDataReader ReadTagTypes = GetTagTypes.ExecuteReader())
                        {
                            while (ReadTagTypes.Read())
                            {
                                AddTagType(new TagType(new Guid(ReadTagTypes[0].ToString()), org.Id, ReadTagTypes[2].ToString().Trim()));
                                NoOfDownloadedTagTypes++;
                            }
                        }
                        AppLog.Debug(String.Format("Found {0} tagtypes(s) for {1}", NoOfDownloadedTagTypes, org.Name));
                    }
                    return true;
                }

                bool GetOrgProducts()
                {
                    foreach (Organisation org in Organisations)
                    {
                        int NoOfDownloadedProducts = 0;

                        SqlCommand GetProducts = new SqlCommand("SELECT * FROM t_Products WHERE OrgId = @OrgId", conn);
                        GetProducts.Parameters.Add(new SqlParameter("OrgId", org.Id));

                        using (SqlDataReader ProductReader = GetProducts.ExecuteReader())
                        {
                            while (ProductReader.Read())
                            {
                                AddProduct(new Product(new Guid(ProductReader[0].ToString()), org.Id, ProductReader[2].ToString()));
                                NoOfDownloadedProducts++;
                            }
                        }
                        AppLog.Debug(String.Format("Found {0} products(s) for {1}", NoOfDownloadedProducts, org.Name));
                    }
                    return true;
                }

                bool GetOrgBugs()
                {
                    AppLog.Debug("BEN !!! Getting organisation's bugs");
                    foreach(Product prod in Products)
                    {
                        SqlCommand GetBugs = new SqlCommand("SELECT * FROM t_Bugs WHERE ProductId = @ProductId", conn);
                        GetBugs.Parameters.Add(new SqlParameter("ProductId", prod.Id));

                        AppLog.Debug(String.Format("Searching for bugs for Product ({0} {1})", prod.Name.Trim(), prod.Id));

                        using (SqlDataReader reader = GetBugs.ExecuteReader())
                        {
                            int NoOfDownloadedBugs = 0;
                            while (reader.Read())
                            {
                                Bug TempBug = new Bug(new Guid(reader[0].ToString()), new Guid(reader[1].ToString()), new Guid(reader[2].ToString()), reader[3].ToString(), "", Convert.ToInt32(reader[5]), Convert.ToDateTime(reader[6]), DateTime.MinValue, Convert.ToDateTime(reader[8]), new Guid(reader[9].ToString()));
                                
                                if(reader[4] != DBNull.Value)
                                {
                                    TempBug.Description = reader[4].ToString();
                                }

                                /*if (reader[7] != DBNull.Value)
                                {
                                    TempBug.ResolvedDateTime = reader[7].ToString();
                                }*/

                                AddBug(TempBug);
                                NoOfDownloadedBugs++;
                            }
                            AppLog.Debug(String.Format("Found {0} bug(s) for {1}", NoOfDownloadedBugs, prod.Name));
                        }
                    }
                    return true;
                }

                bool GetBugAssignees()
                {
                    foreach (Bug mybug in Bugs)
                    {
                        SqlCommand DownloadBug_Assignees = new SqlCommand("SELECT * FROM t_Assignees WHERE " +
                            "BugId = @BugId", conn);
                        DownloadBug_Assignees.Parameters.Add(new SqlParameter("BugId", mybug.Id));

                        using (SqlDataReader reader = DownloadBug_Assignees.ExecuteReader())
                        {
                            int NoOfDownloadedAssignees = 0;
                            while (reader.Read())
                            {
                                Assignee TempAssignee = new Assignee(new Guid(reader[0].ToString()), new Guid(reader[1].ToString()), Convert.ToInt16(reader[2]), Convert.ToInt16(reader[3]), Convert.ToDateTime(reader[4]));

                                AddAssignee(TempAssignee);
                                NoOfDownloadedAssignees++;
                            }
                            AppLog.Debug(String.Format("Found {0} assignees(s) for {1}", NoOfDownloadedAssignees, mybug.Title));
                        }
                        
                    }
                    return true;
                }

                bool GetBugTags()
                {
                    foreach (Bug mybug in Bugs)
                    {
                        SqlCommand DownloadBug_Tags = new SqlCommand("SELECT * FROM t_Tags WHERE BugId = @BugId",
                            conn);

                        int NoOfDownloadedTags = 0;

                        DownloadBug_Tags.Parameters.Add(new SqlParameter("BugId", mybug.Id));
                        using (SqlDataReader reader = DownloadBug_Tags.ExecuteReader())
                        {
                            
                            while (reader.Read())
                            {
                                Tag TempTag = new Tag(new Guid(reader[0].ToString()), mybug.Id, Convert.ToDateTime(reader[2]), new Guid(reader[3].ToString()));
                                
                                AddTag(TempTag);
                                NoOfDownloadedTags++;
                            }
                        }
                        AppLog.Debug(String.Format("Found {0} tag(s) for {1}", NoOfDownloadedTags, mybug.Title));
                    }
                    return true;
                }

                bool GetBugNotes()
                {
                    foreach (Bug mybug in Bugs)
                    {
                        SqlCommand DownloadBug_Notes = new SqlCommand("SELECT * FROM t_Notes WHERE BugId = @BugId", conn);
                        DownloadBug_Notes.Parameters.Add(new SqlParameter("BugId", mybug.Id));

                        int NoOfDownloadedNotes = 0;

                        using (SqlDataReader reader = DownloadBug_Notes.ExecuteReader())
                        {
                            
                            while (reader.Read())
                            {
                                AddNote(new Note(new Guid(reader[0].ToString()), new Guid(reader[1].ToString()), new Guid(reader[2].ToString()), Convert.ToDateTime(reader[3]), new Guid(reader[4].ToString()), Convert.ToDateTime(reader[5].ToString()), reader[6].ToString(), reader[7].ToString()));
                                NoOfDownloadedNotes++;
                            }
                        }
                        AppLog.Debug(String.Format("Found {0} notes(s) for {1}", NoOfDownloadedNotes, mybug.Title));
                    }
                    return true;
                }
            }
        }

        public static string OnlineConnStr = "Server=198.38.83.33;Database=vamon112_bugtrackingsystem;Uid=vamon112_ben;Password=ccjO07JT";

        public static string LocalConnStr = "Data Source = (LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"" +
            AppDomain.CurrentDomain.BaseDirectory + "BTS_Local_Db.mdf\"; Integrated Security = True";
    }
}
