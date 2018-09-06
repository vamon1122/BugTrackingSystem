using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BenLog;

namespace BTS_Class_Library
{

    public class Note
    {
        private Guid _Id;
        private Guid _BugId;
        private User _CreatedBy;
        private DateTime _DateTimeCreated;
        private User _EditedBy;
        private DateTime _DateTimeEdited;
        private string _Title;
        private string _Body;
        public bool Uploaded;
        private string _ErrMsg;

        public Note(Guid pId)
        {
            _Id = pId;
        }

        internal Note(Bug pBug)
        {
            _Id = Guid.NewGuid();
            _BugId = pBug.Id;

            if (Data.ActiveUser == null)
            {
                throw new Exception("Fatal error while creating note! Active user does not exist!");
            }

            _CreatedBy = Data.ActiveUser;
            _EditedBy = Data.ActiveUser;
        }

        public Guid Id { get { return _Id; } }
        public Guid BugId { get { return _BugId; } }
        public User CreatedBy { get { return _CreatedBy; } }
        public DateTime DateTimeCreated { get { return _DateTimeCreated; } }
        public User EditedBy { get; }
        public DateTime DateTimeEdited { get { return _DateTimeEdited; } }
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
        public string Body
        {
            get { return _Body; }

            set
            {
                if (value.Length < 1001)
                {
                    Uploaded = false;
                    _Body = value;
                }
                else
                {
                    throw new Exception("Body exceeds 1000 characters");
                }
            }
        }
        public string ErrMsg { get { return _ErrMsg; } }

        public bool Create()
        {
            AppLog.Info("CREATE NOTE - Starting...");

            //Checks that data is valid before attempting upload
            AppLog.Info("CREATE NOTE - Validating...");
            if (!Validate())
            {
                AppLog.Info("CREATE NOTE - Note failed validation");
                return false;
            }
            AppLog.Info("CREATE NOTE - Note validated successfully");

            _DateTimeCreated = DateTime.Now;
            _DateTimeEdited = DateTime.Now;

            if (!Data.OfflineMode)
            {
                AppLog.Info("CREATE NOTE - OfflineMode is OFF. Attempting to create note on online database...");
                AppLog.Info("CREATE NOTE - Attempting to create note on online database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("CREATE NOTE - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("CREATE NOTE - Connection to online database opened successfully");

                        //This is a check to see weather the note already exists on the database. Obviously
                        //if it's already there, it doesn't need creating again, but this might be called
                        //if for example the note did not exist on the local database, so the Create() function
                        //needed to be able to account for that.
                        AppLog.Info("CREATE NOTE - Checking that note doesn't already exist on online database");
                        bool OnlineNoteExists;

                        SqlCommand CheckOnlineDb = new SqlCommand("SELECT * FROM t_Notes WHERE Id = @Id", conn);
                        CheckOnlineDb.Parameters.Add(new SqlParameter("Id", Id));
                        using (SqlDataReader reader = CheckOnlineDb.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                OnlineNoteExists = true;
                                AppLog.Info("CREATE NOTE - Note already exists in online database!");
                            }
                            else
                            {
                                OnlineNoteExists = false;
                                AppLog.Info("CREATE NOTE - Note does not exist in online database. Creating note on online database");
                            }
                        }
                        if (!OnlineNoteExists)
                        {
                            SqlCommand CreateNote = new SqlCommand("INSERT INTO t_Notes VALUES (@Id, @BugId, " +
                            "@CreatedById, @DateTimeCreated, @EditedById, @DateTimeEdited, @Title, @Body);", conn);
                            CreateNote.Parameters.Add(new SqlParameter("Id", _Id));
                            CreateNote.Parameters.Add(new SqlParameter("BugId", _BugId));
                            CreateNote.Parameters.Add(new SqlParameter("CreatedById", _CreatedBy.Id));
                            CreateNote.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));
                            CreateNote.Parameters.Add(new SqlParameter("EditedById", _EditedBy.Id));
                            CreateNote.Parameters.Add(new SqlParameter("DateTimeEdited", _DateTimeEdited));
                            CreateNote.Parameters.Add(new SqlParameter("Title", _Title));
                            CreateNote.Parameters.Add(new SqlParameter("Body", _Body));

                            CreateNote.ExecuteNonQuery();

                            Uploaded = true;
                            AppLog.Info(String.Format("CREATE NOTE - Note {0} created on online database successfully", Body));
                        }
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error creating note on online database";
                    AppLog.Error("CREATE NOTE - " + _ErrMsg + ": " + e);
                    return false;
                }
            }
            else
            {
                AppLog.Info(String.Format("CREATE NOTE - Offline mode is ON. Skipping create note on" +
                    "online database"));
            }

            AppLog.Info("CREATE NOTE - Attempting to create note on local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("CREATE NOTE - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("CREATE NOTE - Connection to local database opened successfully");

                    //This is a check to see weather the note already exists on the database. Obviously
                    //if it's already there, it doesn't need creating again, but this might be called
                    //if for example the note did not exist on the local database, so the Create() function
                    //needed to be able to account for that.
                    AppLog.Info("CREATE NOTE - Checking that note doesn't already exist on local database");
                    bool LocalNoteExists;

                    SqlCommand CheckLocalDb = new SqlCommand("SELECT * FROM Notes WHERE Id = @Id", conn);
                    CheckLocalDb.Parameters.Add(new SqlParameter("Id", Id));
                    using (SqlDataReader reader = CheckLocalDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            LocalNoteExists = true;
                            AppLog.Info("CREATE NOTE - Note already exists in local database!");
                        }
                        else
                        {
                            LocalNoteExists = false;
                            AppLog.Info("CREATE NOTE - Note does not exist in local database. Creating note on local database");
                        }
                    }
                    if (!LocalNoteExists)
                    {
                        SqlCommand CreateNote = new SqlCommand("INSERT INTO Notes VALUES (@Id, @BugId, @CreatedById," +
                        "@DateTimeCreated, @EditedById, @DateTimeEdited, @Title, @Body, @Uploaded);", conn);
                        CreateNote.Parameters.Add(new SqlParameter("Id", _Id));
                        CreateNote.Parameters.Add(new SqlParameter("BugId", _BugId));
                        CreateNote.Parameters.Add(new SqlParameter("CreatedById", _CreatedBy.Id));
                        CreateNote.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));
                        CreateNote.Parameters.Add(new SqlParameter("EditedById", _EditedBy.Id));
                        CreateNote.Parameters.Add(new SqlParameter("DateTimeEdited", _DateTimeEdited));
                        CreateNote.Parameters.Add(new SqlParameter("Title", _Title));
                        CreateNote.Parameters.Add(new SqlParameter("Body", _Body));
                        CreateNote.Parameters.Add(new SqlParameter("Uploaded", Uploaded));

                        CreateNote.ExecuteNonQuery();

                        AppLog.Info(String.Format("CREATE NOTE - Note {0} created on local database successfully", this.Title));
                    }
                }
            }
            catch (SqlException e)
            {
                    _ErrMsg = "Error while creating note on local database. Changes were not saved";
                    AppLog.Error("CREATE NOTE - " + _ErrMsg + ": " + e);
                    return false;
            }

            AppLog.Info("CREATE NOTE - Success!");
            return true;
        }

        public bool Update()
        {
            AppLog.Info("UPDATE NOTE - Starting...");

            _DateTimeEdited = DateTime.Now;
            _EditedBy = Data.ActiveUser;

            if (!Data.OfflineMode)
            {
                AppLog.Info("UPDATE NOTE - Offline mode is OFF");

                AppLog.Info("UPDATE NOTE - Attempting to update note on online database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("UPDATE NOTE - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("UPDATE NOTE - Connection to online> database opened successfully");

                        SqlCommand UpdateNote = new SqlCommand("UPDATE t_Notes SET EditedById = @EditedById, " +
                            "DateTimeEdited = @DateTimeEdited, Title = @Title, Body = @Body WHERE Id = @Id", conn);
                        UpdateNote.Parameters.Add(new SqlParameter("Id", _Id));
                        UpdateNote.Parameters.Add(new SqlParameter("EditedById", _EditedBy.Id));
                        UpdateNote.Parameters.Add(new SqlParameter("DateTimeEdited", _DateTimeEdited));
                        UpdateNote.Parameters.Add(new SqlParameter("Title", _Title));
                        UpdateNote.Parameters.Add(new SqlParameter("Body", _Body));

                        UpdateNote.ExecuteNonQuery();
                    }
                    Uploaded = true;
                    AppLog.Info(String.Format("UPDATE NOTE - Note {0} updated on online database successfully", Title));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error updating note on online database";
                    AppLog.Error("UPDATE NOTE - " + _ErrMsg + ": " + e);
                    return false;
                }
            }
            else
            {
                AppLog.Info("UPDATE NOTE - Offline mode is ON. Skipping update note on online database");
            }

            AppLog.Info("UPDATE NOTE - Attempting to update note on local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("UPDATE NOTE - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("UPDATE NOTE - Connection to online> database local successfully");

                    SqlCommand UpdateNote = new SqlCommand("UPDATE Notes SET EditedById = @EditedById, " +
                        "DateTimeEdited = @DateTimeEdited, Title = @Title, Body = @Body WHERE Id = @Id", conn);
                    UpdateNote.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateNote.Parameters.Add(new SqlParameter("EditedById", _EditedBy.Id));
                    UpdateNote.Parameters.Add(new SqlParameter("DateTimeEdited", _DateTimeEdited));
                    UpdateNote.Parameters.Add(new SqlParameter("Title", _Title));
                    UpdateNote.Parameters.Add(new SqlParameter("Body", _Body));

                    UpdateNote.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("UPDATE NOTE - Note {0} updated on local database successfully", Title));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while updating note on local database. Changes were not saved.";
                AppLog.Error("UPDATE NOTE - " + _ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info("UPDATE NOTE - Success!");
            return true;
        }

        public bool Get()
        {
            AppLog.Info("GET NOTE - Starting...");
            if (Data.OfflineMode)
            {
                AppLog.Info("GET NOTE - Offline mode is ON, retrieving cached note data from local database");
                AppLog.Info("GET NOTE - Attempting to get note from local database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                    {
                        AppLog.Info("GET NOTE - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("GET NOTE - Connection to local database opened successfully");

                        SqlCommand GetNote = new SqlCommand("SELECT * FROM Notes WHERE Id = @Id;;", conn);
                        GetNote.Parameters.Add(new SqlParameter("Id", _Id));

                        using (SqlDataReader reader = GetNote.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _BugId = new Guid(reader[1].ToString());
                                _CreatedBy = new User(new Guid(reader[2].ToString()));
                                _DateTimeCreated = Convert.ToDateTime(reader[3]);
                                _EditedBy = new User(new Guid(reader[4].ToString()));
                                _DateTimeEdited = Convert.ToDateTime(reader[5]);
                                _Title = reader[6].ToString();
                                _Body = reader[7].ToString();
                                Uploaded = Convert.ToBoolean(reader[8]);
                            }
                        }
                    }
                    AppLog.Info(String.Format("GET NOTE - Note {0} downloaded from local database successfully",
                        Title));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error getting note from local database";
                    AppLog.Error("GET NOTE - " + _ErrMsg + ": " + e);
                    return false;
                }
            }
            else
            {
                AppLog.Info("GET NOTE - Offline mode is OFF, retrieving note data from online database");
                AppLog.Info("GET NOTE - Attempting to download note from online database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("GET NOTE - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("GET NOTE - Connection to online database opened successfully");

                        SqlCommand DownloadNote = new SqlCommand("SELECT * FROM t_Notes WHERE Id = @Id;", conn);
                        DownloadNote.Parameters.Add(new SqlParameter("Id", _Id));

                        using (SqlDataReader reader = DownloadNote.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _BugId = new Guid(reader[1].ToString());
                                _CreatedBy = new User(new Guid(reader[2].ToString()));
                                _DateTimeCreated = Convert.ToDateTime(reader[3]);
                                _EditedBy = new User(new Guid(reader[4].ToString()));
                                _DateTimeEdited = Convert.ToDateTime(reader[5]);
                                _Title = reader[6].ToString();
                                _Body = reader[7].ToString();
                            }
                        }
                    }
                    AppLog.Info(String.Format("GET NOTE - Note {0} downloaded from online database successfully",
                        Title));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error downloading note from online database";
                    AppLog.Error("GET NOTE - " + _ErrMsg + ": " + e);
                    return false;
                }
                
                //Finally, check if note exists in the local database. If not, ADD IT!!! If so, UPDATE IT!!!
                /*AppLog.Info("GET NOTE - Checking whether note exists in local database");

                bool ExistsOnLocalDb;

                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("GET NOTE - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("GET NOTE - Connection to local database opened successfully");



                    SqlCommand CheckLocalDb = new SqlCommand("SELECT * FROM Notes WHERE Id = @Id;", conn);
                    CheckLocalDb.Parameters.Add(new SqlParameter("Id", Id));

                    using (SqlDataReader reader = CheckLocalDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ExistsOnLocalDb = true;
                            AppLog.Info("GET NOTE - Note already exists in the local database!");
                        }
                        else
                        {
                            ExistsOnLocalDb = false;
                        }
                    }
                }
                if (ExistsOnLocalDb)
                {
                    if (Update())
                    {
                        AppLog.Info("GET NOTE - Updated note on local db successfully");
                    }
                    else
                    {
                        AppLog.Info("GET NOTE - Failed to update note: " + _ErrMsg);
                        return false;
                    }
                }
                else
                {
                    if (Create())
                    {
                        AppLog.Info("GET NOTE - Created note on local db successfully");
                    }
                    else
                    {
                        AppLog.Info("GET NOTE - Failed to create note: " + _ErrMsg);
                        return false;
                    }
                }*/
            }
            AppLog.Info("GET NOTE - Success!");
            return true;
        }

        public bool Delete()
        {
            AppLog.Info("<FUNCTION> <CLASS> - Starting...");
            AppLog.Info("DELETE NOTE - Attempting to delete note from local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("DELETE NOTE - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("DELETE NOTE - Connection to local database opened successfully");
                    SqlCommand DeleteNote = new SqlCommand("DELETE FROM Notes WHERE Id = @Id;", conn);
                    DeleteNote.Parameters.Add(new SqlParameter("Id", _Id));

                    DeleteNote.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("DELETE NOTE - Note {0} deleted from local database successfully", Title));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error deleting note from local database";
                AppLog.Error("DELETE NOTE - " + _ErrMsg + ": " + e);
                return false;
            }

            AppLog.Info("DELETE NOTE - Attempting to delete note from online database...");
            try
            {
                AppLog.Info("DELETE NOTE - Attempting to delete note from online database...");
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("DELETE NOTE - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("DELETE NOTE - Connection to online database opened successfully");

                    SqlCommand DeleteNote = new SqlCommand("DELETE FROM t_Notes WHERE Id = @Id;", conn);
                    DeleteNote.Parameters.Add(new SqlParameter("Id", _Id));

                    DeleteNote.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("DELETE NOTE - Note {0} deleted from online database successfully",
                    Title));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error deleting note from online database";
                AppLog.Error("DELETE NOTE - " + _ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info("DELETE NOTE - Success!");
            return true;
        }

        private bool Validate()
        {
            AppLog.Info("VALIDATE NOTE - Starting...");
            try
            {
                if (Title == null) { throw new Exception("Note has not been given a title"); }
                if (Title.Length > 50) { throw new Exception("Title exceeds 50 characters"); }
                if (Body == null) { throw new Exception("Note has not been given a body"); }
                if (Body.Length > 1000) { throw new Exception("Note exceeds 1000 characters"); }
            }
            catch (Exception e)
            {
                _ErrMsg = e.ToString();
                AppLog.Error("VALIDATE NOTE - Validation failed: " + _ErrMsg);
                return false;
            }
            AppLog.Info("VALIDATE NOTE - Success!");
            return true;
        }
    }
}
    

