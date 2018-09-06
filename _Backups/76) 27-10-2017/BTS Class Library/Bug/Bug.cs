using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BenLog;

namespace BTS_Class_Library
{
    public partial class Bug
    {
        private Guid _Id;
        private Guid _ProductId;
        private User _RaisedBy;
        private string _Title;
        private string _Description;
        private int _Severity;
        private DateTime _DateTimeCreated;
        //private DateTime _ResolvedDateTime;
        private string _ErrMsg;
        public bool Uploaded;
        private static string OnlineConnStr = Data.OnlineConnStr;
        private static string LocalConnStr = Data.LocalConnStr;

        public Guid Id { get { return _Id; } }
        public User RaisedBy { get { return _RaisedBy; } }

        public string Title
        {
            get { return _Title; }

            set
            {
                if (value.Length < 51)
                {
                    Uploaded = false;
                    _Title = value;
                }
                else
                {
                    throw new Exception("Title exceeds 50 characters");
                }
            }
        }

        public string Description
        {
            get { return _Description; }

            set
            {
                Uploaded = false;
                if (value.Length < 4001)
                {
                    _Description = value;
                }
                else
                {
                    throw new Exception("Description exceeds 4000 characters");
                }
            }
        }

        public int Severity { get { return _Severity; } set { Uploaded = false; _Severity = value; } } //Need to decide on some limits for this


        public List<Assignee> Assignees { get {
                AppLog.Info("BUG ASSIGNEES (Getter) - Attempting to retrieve bug's assignees from local database...");

                List<Assignee> MyAssignees = new List<Assignee>();

                int DownloadedAssignees = 0;

                if (Data.OfflineMode)
                {
                    using (SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        AppLog.Info("GET BUG - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("GET BUG - Connection to local database opened successfully");

                        AppLog.Info("GET BUG - Attempting to retrieve bug's assignees from local database...");
                        SqlCommand DownloadBug_Assignees = new SqlCommand("SELECT UserId FROM Assignees WHERE BugId = @BugId",
                            conn);
                        DownloadBug_Assignees.Parameters.Add(new SqlParameter("BugId", _Id));

                        using (SqlDataReader reader = DownloadBug_Assignees.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MyAssignees.Add(new Assignee(this, new User(new Guid(reader[0].ToString()))));
                                DownloadedAssignees++;
                            }
                        }
                        AppLog.Info(String.Format("GET BUG - {0} assignees were retrieved for bug from local database " +
                            "successfully", DownloadedAssignees));
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        AppLog.Info("GET BUG - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("GET BUG - Connection to online database opened successfully");

                        AppLog.Info("GET BUG - Attempting to retrieve bug's assignees from online database...");
                        SqlCommand DownloadBug_Assignees = new SqlCommand("SELECT UserId FROM t_BTS_Assignees WHERE " +
                            "BugId = @BugId", conn);
                        DownloadBug_Assignees.Parameters.Add(new SqlParameter("BugId", _Id));

                        using (SqlDataReader reader = DownloadBug_Assignees.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Assignee TempAssignee = new Assignee(this, new User(new Guid(reader[0].ToString())));
                                if (!TempAssignee.Get())
                                {
                                    _ErrMsg = "Error while downloading assignee for bug: " + TempAssignee.ErrMsg;
                                    throw new Exception(_ErrMsg);
                                }
                                MyAssignees.Add(TempAssignee);
                                DownloadedAssignees++;
                            }
                        }
                        AppLog.Info(String.Format("GET BUG - {0} assignees were retrieved for bug from online database " +
                            "successfully", DownloadedAssignees));

                    }
                }
                return MyAssignees;
            }
        }


        public List<Tag> Tags { get {
                AppLog.Info("BUG ASSIGNEES (Getter) - Attempting to retrieve bug's tags from local database...");

                List<Tag> MyTags = new List<Tag>();
                int DownloadedTags = 0;

                if (Data.OfflineMode)
                {
                    using (SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        AppLog.Info("GET BUG - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("GET BUG - Connection to local database opened successfully");

                        AppLog.Info("GET BUG - Attempting to retrieve bug's tags from local database...");
                        SqlCommand DownloadBug_Tags = new SqlCommand("SELECT Id FROM Tags WHERE BugId = @BugId", conn);
                        DownloadBug_Tags.Parameters.Add(new SqlParameter("BugId", _Id));
                        using (SqlDataReader reader = DownloadBug_Tags.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Tag TempTag = new Tag(new Guid(reader[0].ToString()));
                                if (!TempTag.Get())
                                {
                                    _ErrMsg = "Error while downloading tag for bug: " + TempTag.ErrMsg;
                                    throw new Exception(_ErrMsg);
                                    
                                }
                                MyTags.Add(TempTag);
                                DownloadedTags++;
                            }
                        }
                        AppLog.Info(String.Format("GET BUG - {0} tags were retrieved for bug from local database " +
                            "successfully", DownloadedTags));

                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        AppLog.Info("GET BUG - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("GET BUG - Connection to online database opened successfully");

                        AppLog.Info("GET BUG - Attempting to retrieve bug's tags from online database...");
                        SqlCommand DownloadBug_Tags = new SqlCommand("SELECT Id FROM t_BTS_Tags WHERE BugId = @BugId",
                            conn);
                        DownloadBug_Tags.Parameters.Add(new SqlParameter("BugId", _Id));
                        using (SqlDataReader reader = DownloadBug_Tags.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Tag TempTag = new Tag(new Guid(reader[0].ToString()));
                                TempTag.Get();
                                if (!TempTag.Get())
                                {
                                    _ErrMsg = "Error while downloading tag for bug: " + TempTag.ErrMsg;
                                    throw new Exception(_ErrMsg);
                                }
                                MyTags.Add(TempTag);
                                DownloadedTags++;
                            }
                        }
                        AppLog.Info(String.Format("GET BUG - {0} tags were retrieved for bug from online database " +
                            "successfully", DownloadedTags));

                    }
                }
                return MyTags;
            }
        }


        public List<Note> Notes { get {
                AppLog.Info("BUG NOTES (Getter) - Attempting to retrieve bug's notes from local database...");

                List<Note> MyNotes = new List<Note>();
                int DownloadedNotes = 0;

                if (Data.OfflineMode)
                {
                    using (SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        AppLog.Info("GET BUG - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("GET BUG - Connection to local database opened successfully");

                        AppLog.Info("GET BUG - Attempting to retrieve bug's notes from local database...");
                        SqlCommand DownloadBug_Notes = new SqlCommand("SELECT Id FROM Notes WHERE BugId = @BugId", conn);
                        DownloadBug_Notes.Parameters.Add(new SqlParameter("BugId", _Id));
                        using (SqlDataReader reader = DownloadBug_Notes.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MyNotes.Add(new Note(new Guid(reader[0].ToString())));
                                DownloadedNotes++;
                            }
                        }
                        AppLog.Info(String.Format("GET BUG - {0} notes were retrieved for bug from local database " +
                            "successfully", DownloadedNotes));

                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        AppLog.Info("GET BUG - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("GET BUG - Connection to online database opened successfully");


                        AppLog.Info("GET BUG - Attempting to retrieve bug's notes from online database...");
                        SqlCommand DownloadBug_Notes = new SqlCommand("SELECT Id FROM t_BTS_Notes WHERE BugId = @BugId", conn);
                        DownloadBug_Notes.Parameters.Add(new SqlParameter("BugId", _Id));
                        using (SqlDataReader reader = DownloadBug_Notes.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Note TempNote = new Note(new Guid(reader[0].ToString()));
                                TempNote.Get();
                                if (!TempNote.Get())
                                {
                                    _ErrMsg = "Error while downloading note for bug: " + TempNote.ErrMsg;
                                    throw new Exception(_ErrMsg);
                                }
                                MyNotes.Add(TempNote);
                                DownloadedNotes++;
                            }
                        }
                        AppLog.Info(String.Format("GET BUG - {0} notes were retrieved for bug from online database " +
                            "successfully", DownloadedNotes));
                    }
                }
                return MyNotes;
            }
        }


        public DateTime CreatedDateTime { get { return _DateTimeCreated; } }
        //public DateTime ResolvedDateTime { get { return _ResolvedDateTime; } }

        /*public bool Resolved
        {
            get
            {
                if (_ResolvedDateTime != null && _ReOpenedDateTime != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }*/



        /*These need re-implementing
         * 
         * public int TotalTags { get { return _Tags.Count; } }
        public int TotalAssignees { get { return _Assignees.Count; } }
        public int TotalNotes { get { return _Notes.Count; } }*/

        

        public string ErrMsg { get { return _ErrMsg; } }

        public Bug(Guid pId)
        {
            _Id = pId;
            //if (!Get()) { Data.UserFriendlyError(_ErrMsg); };
        }

        internal Bug(Product pProduct)
        {
            _Id = Guid.NewGuid();
            _ProductId = pProduct.Id;
            Uploaded = false;
            

            /*if (Data.ActiveProduct == null)
            {
                throw new Exception("Fatal error while creating bug! Active product does not exist!");
            }
            else
            {
                _ProductId = Data.ActiveProduct.Id;
            }*/

            if (Data.ActiveUser == null)
            {
                throw new Exception("Fatal error while creating bug! Active user does not exist!");   
            }
            else
            {
                _RaisedBy = Data.ActiveUser;
            }



        }

        public bool Create()
        {
            AppLog.Info("CREATE BUG - Starting...");
            

            //Checks that data is valid before attempting upload
            AppLog.Info("CREATE BUG - Validating...");
            if (!Validate())
            {
                AppLog.Info("CREATE BUG - Bug failed validation");
                return false;
            }
            AppLog.Info("CREATE BUG - Bug validated successfully");

            _DateTimeCreated = DateTime.Now;

            //Need to check for offline mode here. Also need to do GET for TagType

            AppLog.Info("CREATE BUG - Attempting to CREATE BUG on online database...");
            try //Online
            {
                using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                {
                    AppLog.Info("CREATE BUG - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("CREATE BUG - Connection to online database opened successfully");

                    //This is a check to see weather the bug already exists on the database. Obviously
                    //if it's already there, it doesn't need creating again, but this might be called
                    //if for example the bug did not exist on the local database, so the Create() function
                    //needed to be able to account for that.
                    AppLog.Info("CREATE BUG - Checking that bug doesn't already exist on online database");
                    bool OnlineBugExists;

                    SqlCommand CheckOnlineDb = new SqlCommand("SELECT * FROM t_BTS_Bugs WHERE Id = @Id", conn);
                    CheckOnlineDb.Parameters.Add(new SqlParameter("Id", Id));
                    using (SqlDataReader reader = CheckOnlineDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            OnlineBugExists = true;
                            AppLog.Info("CREATE BUG - Bug already exists in online database!");
                        }
                        else
                        {
                            OnlineBugExists = false;
                        }
                    }
                    if (!OnlineBugExists)
                    {
                        AppLog.Info("CREATE BUG - Bug does not exist in online database. Creating bug on online database");

                        SqlCommand CreateBug = new SqlCommand("INSERT INTO t_BTS_Bugs (Id, ProductId, RaisedBy, Title, " +
                            "Severity, CreatedDateTime) VALUES (@Id, @ProductId, @RaisedBy, @Title, @Severity, @CreatedDateTime);", conn);
                        CreateBug.Parameters.Add(new SqlParameter("Id", _Id));
                        CreateBug.Parameters.Add(new SqlParameter("ProductId", _ProductId));
                        CreateBug.Parameters.Add(new SqlParameter("RaisedBy", _RaisedBy.Id));
                        CreateBug.Parameters.Add(new SqlParameter("Title", _Title));
                        CreateBug.Parameters.Add(new SqlParameter("Severity", _Severity));
                        CreateBug.Parameters.Add(new SqlParameter("CreatedDateTime", _DateTimeCreated));

                        CreateBug.ExecuteNonQuery();

                        if (_Description != null && _Description != "")
                        {
                            SqlCommand CreateBug_Description = new SqlCommand("UPDATE t_BTS_Bugs SET Description = " +
                                "@Description WHERE Id = @Id",conn);
                            CreateBug_Description.Parameters.Add(new SqlParameter("Description", _Description));
                            CreateBug_Description.Parameters.Add(new SqlParameter("Id", _Id));

                            CreateBug_Description.ExecuteNonQuery();
                        }
                    }
                }
                Uploaded = true;
                AppLog.Info(String.Format("CREATE BUG - Bug {0} created on online database successfully", Title));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error whilst creating bug on the online database";
                AppLog.Error("CREATE BUG - " + _ErrMsg + ": " + e);
                return false;
            }

            AppLog.Info("CREATE BUG - Attempting to create bug on local database...");
            try //Local
            {
                using (SqlConnection conn = new SqlConnection(LocalConnStr))
                {
                    AppLog.Info("CREATE BUG - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("CREATE BUG - Connection to local database opened successfully");

                    //This is a check to see weather the bug already exists on the database. Obviously
                    //if it's already there, it doesn't need creating again, but this might be called
                    //if for example the bug did not exist on the local database, so the Create() function
                    //needed to be able to account for that.
                    AppLog.Info("CREATE BUG - Checking that bug doesn't already exist on local database");
                    bool LocalBugExists;

                    SqlCommand CheckLocalDb = new SqlCommand("SELECT * FROM Bugs WHERE Id = @Id", conn);
                    CheckLocalDb.Parameters.Add(new SqlParameter("Id", Id));
                    using (SqlDataReader reader = CheckLocalDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            LocalBugExists = true;
                            AppLog.Info("CREATE BUG - Bug already exists in local database!");
                        }
                        else
                        {
                            LocalBugExists = false;
                        }
                    }
                    if (!LocalBugExists)
                    {
                        SqlCommand CreateBug = new SqlCommand("INSERT INTO Bugs (Id, ProductId, RaisedBy, Title, Severity, " +
                            "CreatedDateTime, Uploaded) VALUES (@Id, @ProductId, @RaisedBy, @Title, @Severity, @CreatedDateTime, " +
                            "@Uploaded);", conn);
                        CreateBug.Parameters.Add(new SqlParameter("Id", _Id));
                        CreateBug.Parameters.Add(new SqlParameter("ProductId", _ProductId));
                        CreateBug.Parameters.Add(new SqlParameter("RaisedBy", _RaisedBy.Id));
                        CreateBug.Parameters.Add(new SqlParameter("Title", _Title));
                        CreateBug.Parameters.Add(new SqlParameter("Severity", _Severity));
                        CreateBug.Parameters.Add(new SqlParameter("CreatedDateTime", _DateTimeCreated));
                        CreateBug.Parameters.Add(new SqlParameter("Uploaded", BitConverter.GetBytes(Uploaded)));

                        CreateBug.ExecuteNonQuery();

                        if (_Description != null && _Description != "")
                        {
                            SqlCommand CreateBug_Description = new SqlCommand("UPDATE Bugs SET Description = @Description " +
                                "WHERE Id = @Id",conn);
                            CreateBug_Description.Parameters.Add(new SqlParameter("Description", _Description));
                            CreateBug_Description.Parameters.Add(new SqlParameter("Id", _Id));

                            CreateBug_Description.ExecuteNonQuery();
                        }
                    }
                }
                AppLog.Info(String.Format("CREATE BUG - Bug {0} created on local database successfully", Title));
            }
            catch (SqlException e)
            {
                    _ErrMsg = "Error while creating bug on local database. Changes were not saved";
                    AppLog.Error("CREATE BUG - " + _ErrMsg + ": " + e);
                    return false;
            }    

            AppLog.Info("CREATE BUG - Success!");
            return true;
        }

        public bool Update()
        {
            AppLog.Info("UPDATE BUG - Starting...");

            AppLog.Info("UPDATE BUG - Attempting to update bug on online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                {
                    AppLog.Info("UPDATE BUG - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("UPDATE BUG - Connection to online database opened successfully");
                    
                    SqlCommand UpdateBug = new SqlCommand("UPDATE t_BTS_Bugs SET Title = @Title, Severity = @Severity " +
                        "WHERE Id = @Id", conn);

                    UpdateBug.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateBug.Parameters.Add(new SqlParameter("Title", _Title));
                    UpdateBug.Parameters.Add(new SqlParameter("Severity", _Severity));

                    UpdateBug.ExecuteNonQuery();

                    if (_Description != null && _Description != "")
                    {
                        SqlCommand UpdateBug_Description = new SqlCommand("UPDATE t_BTS_Bugs SET Description = @Description " +
                            "WHERE Id = @Id", conn);
                        UpdateBug_Description.Parameters.Add(new SqlParameter("Description", _Description));
                        UpdateBug_Description.Parameters.Add(new SqlParameter("Id", _Id));

                        UpdateBug_Description.ExecuteNonQuery();
                    }
                }
                Uploaded = true;
                AppLog.Info(String.Format("UPDATE BUG - Bug {0} updated on online database successfully", Title));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error updating bug on the online database";
                AppLog.Error("UPDATE BUG - " + _ErrMsg + ": " + e);
                return false;
            }

            AppLog.Info("UPDATE BUG - Attempting to update bug on local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(LocalConnStr))
                {
                    AppLog.Info("UPDATE BUG - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("UPDATE BUG - Connection to local database opened successfully");

                    SqlCommand UpdateBug = new SqlCommand("UPDATE Bugs SET Title = @Title, Severity = @Severity WHERE " +
                        "Id = @Id", conn);

                    UpdateBug.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateBug.Parameters.Add(new SqlParameter("Title", _Title));
                    UpdateBug.Parameters.Add(new SqlParameter("Severity", _Severity));
                    UpdateBug.Parameters.Add(new SqlParameter("Uploaded", BitConverter.GetBytes(Uploaded)));

                    UpdateBug.ExecuteNonQuery();

                    if (_Description != null && _Description != "")
                    {
                        SqlCommand UpdateBug_Description = new SqlCommand("UPDATE Bugs SET Description = @Description " +
                            "WHERE Id = @Id", conn);
                        UpdateBug_Description.Parameters.Add(new SqlParameter("Description", _Description));
                        UpdateBug_Description.Parameters.Add(new SqlParameter("Id", _Id));

                        UpdateBug_Description.ExecuteNonQuery();
                    }
                }
                AppLog.Info(String.Format("UPDATE BUG - Bug {0} updated on local database successfully", Title));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while updating bug on local database. Changes were not saved";
                AppLog.Error("UPDATE BUG - " + _ErrMsg + ": " + e);
                return false;
            }
            
            AppLog.Info("UPDATE BUG - Success!");
            return true;
        }

        /// <summary>
        /// This Get() method downloads the bug, the it's assignees, notes and tags
        /// </summary>
        /// <returns></returns>
        public bool Get()
        {
            AppLog.Info("GET BUG - Starting...");

            if (Data.OfflineMode)
            {
                AppLog.Info("GET BUG - Offline mode is ON, retrieving cached bug data from local database");
                AppLog.Info("GET BUG - Attempting to retrieve bug from local database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        AppLog.Info("GET BUG - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("GET BUG - Connection to local database opened successfully");

                        AppLog.Info("GET BUG - Attempting to retrieve core bug from local database...");
                        SqlCommand DownloadBug = new SqlCommand("SELECT * FROM Bugs WHERE Id = @Id ", conn);

                        DownloadBug.Parameters.Add(new SqlParameter("Id", _Id));

                        using (SqlDataReader reader = DownloadBug.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //_Id = new Guid(reader[0].ToString());
                                _ProductId = new Guid(reader[1].ToString());
                                _RaisedBy = new User(new Guid(reader[2].ToString()));
                                _Title = reader[3].ToString().Trim();
                                _Severity = Convert.ToUInt16(reader[5]);
                                _DateTimeCreated = Convert.ToDateTime(reader[6]);
                                Uploaded = true;

                                if (!reader.IsDBNull(4))
                                {
                                    _Description = reader[4].ToString();
                                }
                            }
                        }
                        AppLog.Info("GET BUG - Bug retrieved from local database successfully");
                    }
                    AppLog.Info(String.Format("GET BUG - Bug {0} retrieved from local database successfully", Title));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error downloading bug from the local database";
                    AppLog.Error("GET BUG - " + _ErrMsg + ": " + e);
                    return false;
                }
            }
            else
            {
                AppLog.Info("GET BUG - Offline mode is OFF, retrieving bug data from online database");
                AppLog.Info("GET BUG - Attempting to retrieve bug on online database...");
                try
                {
                    //Download from online
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        AppLog.Info("GET BUG - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("GET BUG - Connection to online database opened successfully");

                        AppLog.Info("GET BUG - Attempting to retrieve core bug from online database...");
                        SqlCommand DownloadBug = new SqlCommand("SELECT * FROM t_BTS_Bugs WHERE Id = @Id ", conn);

                        DownloadBug.Parameters.Add(new SqlParameter("Id", _Id));

                        using (SqlDataReader reader = DownloadBug.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _Id = new Guid(reader[0].ToString());
                                _ProductId = new Guid(reader[1].ToString());
                                _RaisedBy = new User(new Guid(reader[2].ToString()));
                                _Title = reader[3].ToString().Trim();
                                _Severity = Convert.ToUInt16(reader[5]);
                                _DateTimeCreated = Convert.ToDateTime(reader[6]);
                                Uploaded = true;

                                if (!reader.IsDBNull(4))
                                {
                                    _Description = reader[4].ToString();
                                }
                            }
                        }
                        AppLog.Info("GET BUG - Bug retrieved from online database successfully");
                    }
                    AppLog.Info(String.Format("GET BUG - Bug {0} retrieved on online database successfully", Title));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error downloading bug from the online database";
                    AppLog.Error("GET BUG - " + _ErrMsg + ": " + e);
                    return false;
                }

                //Finally, check if bug exists in the local database. If not, ADD IT!!!
                AppLog.Info("GET BUG - Checking whether bug exists in local database");

                bool LocalBugExists;

                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("GET BUG - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("GET BUG - Connection to local database opened successfully");

                    

                    SqlCommand CheckLocalDb = new SqlCommand("SELECT * FROM Bugs WHERE Id = @Id;", conn);
                    CheckLocalDb.Parameters.Add(new SqlParameter("Id", Id));

                    using (SqlDataReader reader = CheckLocalDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            LocalBugExists = true;
                            AppLog.Info("GET BUG - Bug already exists in the local database!");
                        }
                        else
                        {
                            LocalBugExists = false;
                        }
                    }
                }
                if (LocalBugExists)
                {
                    if (Update())
                    {
                        AppLog.Info("GET BUG - Updated bug on local db successfully");
                    }
                    else
                    {
                        AppLog.Info("GET BUG - Failed to update bug: " + _ErrMsg);
                        return false;
                    }
                }
                else
                {
                    if (Create())
                    {
                        AppLog.Info("GET BUG - Created bug on local db successfully");
                    }
                    else
                    {
                        AppLog.Info("GET BUG - Failed to create bug: " + _ErrMsg);
                        return false;
                    }
                }
            }
            AppLog.Info("GET BUG - Success!");
            return true;
        }

        public bool Delete()
        {
            AppLog.Info("DELETE BUG - Starting...");

            AppLog.Info("DELETE BUG - Attempting to delete bug on local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(LocalConnStr))
                {
                    AppLog.Info("DELETE BUG - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("DELETE BUG - Connection to local database opened successfully");

                    AppLog.Info("DELETE BUG - Attempting to delete core bug from local database...");
                    SqlCommand DeleteBug = new SqlCommand("DELETE FROM Bugs WHERE Id = @Id;", conn);
                    DeleteBug.Parameters.Add(new SqlParameter("Id", _Id));
                    DeleteBug.ExecuteNonQuery();
                    AppLog.Info("DELETE BUG - Core bug deleted from local database successfully");

                    AppLog.Info("DELETE BUG - Attempting to delete bug's notes from local database...");
                    SqlCommand DeleteNotes = new SqlCommand("DELETE FROM Notes WHERE BugId = @BugId;", conn);
                    DeleteNotes.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteNotes.ExecuteNonQuery();
                    AppLog.Info("DELETE BUG - Bug's notes deleted from local database successfully");

                    AppLog.Info("DELETE BUG - Attempting to delete bug's assignees from online database...");
                    SqlCommand DeleteAssignees = new SqlCommand("DELETE FROM Assignees WHERE BugId = @BugId;", conn);
                    DeleteAssignees.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteAssignees.ExecuteNonQuery();
                    AppLog.Info("DELETE BUG - Bug's assignees deleted from online database successfully");

                    AppLog.Info("DELETE BUG - Attempting to delete bug's tags from online database...");
                    SqlCommand DeleteTags = new SqlCommand("DELETE FROM Tags WHERE BugId = @BubId;", conn);
                    DeleteTags.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteTags.ExecuteNonQuery();
                    AppLog.Info("DELETE BUG - Bug's tags deleted from online database successfully");
                }
                AppLog.Info(String.Format("DELETE BUG - Bug {0} <function> on <online/local> database successfully", Title));
            }
            catch(SqlException e)
            {
                _ErrMsg = "Error deleting bug from the local database";
                AppLog.Error("DELETE BUG - " + _ErrMsg + ": " + e);
                return false;
            }

            AppLog.Info("DELETE BUG - Attempting to delete bug on online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                {
                    AppLog.Info("DELETE BUG - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("DELETE BUG - Connection to online database opened successfully");
                    
                    AppLog.Info("DELETE BUG - Attempting to delete core bug from online database...");
                    SqlCommand DeleteBug = new SqlCommand("DELETE FROM t_BTS_Bugs WHERE Id = @Id;", conn);
                    DeleteBug.Parameters.Add(new SqlParameter("Id", _Id));
                    DeleteBug.ExecuteNonQuery();
                    AppLog.Info("DELETE BUG - Core bug deleted from online database successfully");

                    AppLog.Info("DELETE BUG - Attempting to delete bug's notes from online database...");
                    SqlCommand DeleteNotes = new SqlCommand("DELETE FROM t_BTS_Notes WHERE BugId = @BugId;", conn);
                    DeleteNotes.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteNotes.ExecuteNonQuery();
                    AppLog.Info("DELETE BUG - Bug's notes deleted from online database successfully");

                    AppLog.Info("DELETE BUG - Attempting to delete bug's assignees from online database...");
                    SqlCommand DeleteAssignees = new SqlCommand("DELETE FROM t_BTS_Assignees WHERE BugId = @BugId;", conn);
                    DeleteAssignees.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteAssignees.ExecuteNonQuery();
                    AppLog.Info("DELETE BUG - Bug's assignees deleted from online database successfully");

                    AppLog.Info("DELETE BUG - Attempting to delete bug's tags from online database...");
                    SqlCommand DeleteTags = new SqlCommand("DELETE FROM Tags WHERE t_BTS_BugId = @BugId;", conn);
                    DeleteTags.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteTags.ExecuteNonQuery();
                    AppLog.Info("DELETE BUG - Bug's tags deleted from online database successfully");
                }
                AppLog.Info(String.Format("DELETE BUG - Bug {0} deleted from online database successfully", Title));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error deleting bug from the online database";
                AppLog.Error("DELETE BUG - " + _ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info("DELETE BUG - Success!");
            return true;
        }

        public Tag NewTag(TagType pTagType)
        {
            return new Tag(this, pTagType);
        }

        private bool Validate()
        {
            AppLog.Info("VALIDATE BUG - Starting...");
            try
            {
                if (_Title == null || _Title == "") { throw new Exception("Bug has not been given a title"); }
                if (_Title.Length > 50) { throw new Exception("Title exceeds 50 characters"); }
                if(Description != null)
                {
                    if (_Description.Length > 4000) { throw new Exception("Description exceeds 4000 characters"); }
                }
                if (_Severity == 0) { throw new Exception("Bug has not been given a severity"); }
                if (_Severity.ToString().Length > 1) { throw new Exception("Severity must be between 1 and 9"); }
            }
            catch(Exception e)
            {
                _ErrMsg = e.ToString();
                AppLog.Error("VALIDATE BUG - Validation failed: " + _ErrMsg);
                return false;
            }
            AppLog.Info("VALIDATE BUG - Success!");
            return true;
        }

        public Tag CreateTag(TagType pTagType)
        {
            return new Tag(this, pTagType);
        }

        public Assignee CreateAssignee(User pUser)
        {
            return new Assignee(this, pUser);
        }

        public Note CreateNote()
        {
            return new Note(this);
        }

        /*public void Resolve()
        {
            _ResolvedDateTime = DateTime.Now;
        }*/

       

    }
}
