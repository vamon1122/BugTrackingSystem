using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BenLog;

namespace BTS_Class_Library
{

    public class Tag
    {
        private Guid _Id;
        private Guid _BugId;
        private DateTime _DateTimeCreated;
        private TagType _MyTagType;
        //public bool Uploaded; Tag properties cannot be changed
        private string _ErrMsg;

        public Guid Id { get { return _Id; } }
        public Guid BugId { get { return _BugId; } }
        public DateTime DateTimeCreated { get { return _DateTimeCreated; } }
        public TagType Type { get { return _MyTagType; } }
        public string ErrMsg { get { return _ErrMsg; } }

        internal Tag(Guid pId)
        {
            _Id = pId;
            //if (!Get()) { Data.UserFriendlyError(_ErrMsg); };
        }

        public Tag(Bug pBug, TagType pTagType)
        {
            _Id = Guid.NewGuid();
            _BugId = pBug.Id;
            _MyTagType = pTagType;
            //_DateTimeCreated = DateTime.Now;
        }

        public bool Create()
        {
            AppLog.Info("CREATE TAG - Starting...");

            _DateTimeCreated = DateTime.Now;

            //Tag does not need to be validated because it can't really be invalid

            if (!Data.OfflineMode)
            {

                AppLog.Info("CREATE TAG - Attempting to create tag on online database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("CREATE TAG - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("CREATE TAG - Connection to online database opened successfully");

                        //This is a check to see weather the tag already exists on the database. Obviously
                        //if it's already there, it doesn't need creating again, but this might be called
                        //if for example the tag did not exist on the local database, so the Create() function
                        //needed to be able to account for that.
                        AppLog.Info("CREATE TAG - Checking that tag doesn't already exist on online database");
                        bool OnlineTagExists;

                        SqlCommand CheckOnlineDb = new SqlCommand("SELECT * FROM t_BTS_Tags WHERE Id = @Id", conn);
                        CheckOnlineDb.Parameters.Add(new SqlParameter("Id", Id));
                        using (SqlDataReader reader = CheckOnlineDb.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                OnlineTagExists = true;
                                AppLog.Info("CREATE TAG - Tag already exists in online database!");
                            }
                            else
                            {
                                OnlineTagExists = false;
                                AppLog.Info("CREATE TAG - Tag does not exist in online database. Creating tag on online database");
                            }
                        }

                        if (!OnlineTagExists)
                        {
                            SqlCommand CreateTag = new SqlCommand("INSERT INTO t_BTS_Tags VALUES (@Id, @BugId, " +
                                                        "@DateTimeCreated, @TagTypeId);", conn);
                            CreateTag.Parameters.Add(new SqlParameter("Id", _Id));
                            CreateTag.Parameters.Add(new SqlParameter("BugId", BugId));
                            CreateTag.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));
                            CreateTag.Parameters.Add(new SqlParameter("TagTypeId", _MyTagType.Id));

                            CreateTag.ExecuteNonQuery();

                            AppLog.Info(String.Format("CREATE TAG - Tag {0} created on online database successfully",
                        Type.Value));
                        }
                    }
                    //Uploaded = true; Tag properties cannot be changed
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while creating tag on online database. Changes were not saved";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
            }
            else
            {
                AppLog.Info(String.Format("CREATE TAG - Offline mode is ON. Skipping create tag on" +
                    "online database"));
            }

            AppLog.Info("CREATE TAG - Attempting to create tag on local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("CREATE TAG - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("CREATE TAG - Connection to local database opened successfully");

                    //This is a check to see weather the tag already exists on the database. Obviously
                    //if it's already there, it doesn't need creating again, but this might be called
                    //if for example the tag did not exist on the local database, so the Create() function
                    //needed to be able to account for that.
                    AppLog.Info("CREATE TAG - Checking that tag doesn't already exist on local database");
                    bool LocalTagExists;

                    SqlCommand CheckLocalDb = new SqlCommand("SELECT * FROM Tags WHERE Id = @Id", conn);
                    CheckLocalDb.Parameters.Add(new SqlParameter("Id", Id));
                    using (SqlDataReader reader = CheckLocalDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            LocalTagExists = true;
                            AppLog.Info("CREATE TAG - Tag already exists in local database!");
                        }
                        else
                        {
                            LocalTagExists = false;
                            AppLog.Info("CREATE USER - Tag does not exist in local database. Creating tag on local database");
                        }
                    }

                    if (!LocalTagExists)
                    {
                        SqlCommand CreateTag = new SqlCommand("INSERT INTO Tags VALUES (@Id, @BugId, @DateTimeCreated, " +
                                                "@TagTypeId);",conn);
                        CreateTag.Parameters.Add(new SqlParameter("Id", _Id));
                        CreateTag.Parameters.Add(new SqlParameter("BugId", BugId));
                        CreateTag.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));
                        CreateTag.Parameters.Add(new SqlParameter("TagTypeId", _MyTagType.Id));

                        CreateTag.ExecuteNonQuery();
                        AppLog.Info(String.Format("CREATE TAG - Tag {0} created on local database successfully", Type.Value));
                    }
                    
                }
                
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while creating tag on local database";
                AppLog.Error("CREATE TAG - " + _ErrMsg + ": " + e);
                return false;
            }


            AppLog.Info("CREATE TAG - Success!");
            return true;
        }

        public bool Get()
        {
            AppLog.Info("GET TAG - Starting...");

            if (Data.OfflineMode)
            {
                AppLog.Info("GET TAG - Offline mode is ON, retrieving cached tag data from local database");
                AppLog.Info("GET TAG - Attempting to download tag from local database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                    {
                        AppLog.Info("GET TAG - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("GET TAG - Connection to local database opened successfully");

                        SqlCommand GetTag = new SqlCommand("SELECT * FROM Tags WHERE Id = @Id", conn);
                        GetTag.Parameters.Add(new SqlParameter("Id", _Id));

                        GetTag.ExecuteNonQuery();

                        using (SqlDataReader reader = GetTag.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _BugId = new Guid(reader[1].ToString());
                                _DateTimeCreated = Convert.ToDateTime(reader[2]);
                                TagType TempTagType = new TagType(new Guid(reader[3].ToString()));
                                if (!TempTagType.Get())
                                {
                                    throw new Exception("Error while downloading Tag's TagType: " + _ErrMsg);
                                }
                                _MyTagType = TempTagType;
                            }
                        }
                    }
                    AppLog.Info(String.Format("GET TAG - <Class> {0} <function> on <online/local> database successfully", Type.Value));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while getting tag from local database";
                    AppLog.Error("GET TAG - " + _ErrMsg + ": " + e);
                    return false;
                }
            }
            else
            {
                AppLog.Info("GET TAG - Offline mode is OFF, retrieving tag data from online database");
                AppLog.Info("GET TAG - Attempting to retrieve tag from online database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("GET TAG - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("GET TAG - Connection to online database opened successfully");

                        SqlCommand GetTag = new SqlCommand("SELECT * FROM t_BTS_Tags WHERE Id = @Id", conn);
                        GetTag.Parameters.Add(new SqlParameter("Id", _Id));

                        GetTag.ExecuteNonQuery();

                        using (SqlDataReader reader = GetTag.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _BugId = new Guid(reader[1].ToString());
                                _DateTimeCreated = Convert.ToDateTime(reader[2]);
                                TagType TempTagType = new TagType(new Guid(reader[3].ToString()));
                                if (!TempTagType.Get())
                                {
                                    throw new Exception("Error while downloading Tag's TagType: " + _ErrMsg);
                                }
                                _MyTagType = TempTagType;
                            }
                        }
                    }
                    AppLog.Info(String.Format("GET TAG - Tag {0} retrieved from local database successfully", Type.Value));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while getting tag from online database";
                    AppLog.Error("GET TAG - " + _ErrMsg + ": " + e);
                    return false;
                }

                //Finally, check if tag exists in the local database. If not, ADD THEM!!! If so, UPDATE THEM!!!
                AppLog.Info("GET TAG - Checking whether tag exists in local database");

                bool ExistsOnLocalDb;

                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("GET TAG - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("GET TAG - Connection to local database opened successfully");



                    SqlCommand CheckLocalDb = new SqlCommand("SELECT * FROM Tags WHERE Id = @Id;", conn);
                    CheckLocalDb.Parameters.Add(new SqlParameter("Id", Id));

                    using (SqlDataReader reader = CheckLocalDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ExistsOnLocalDb = true;
                            AppLog.Info("GET TAG - Tag already exists in the local database!");
                        }
                        else
                        {
                            ExistsOnLocalDb = false;
                        }
                    }
                }
                if (ExistsOnLocalDb)
                {
                    //Would usually update but tags cannot be updated
                }
                else
                {
                    if (Create())
                    {
                        AppLog.Info("GET TAG - Created tag on local db successfully");
                    }
                    else
                    {
                        AppLog.Info("GET TAG - Failed to create tag: " + _ErrMsg);
                        return false;
                    }
                }
            }
            AppLog.Info("GET TAG - Success!");
            return true;
        }

        public bool Delete()
        {
            AppLog.Info("DELETE TAG - Starting...");

            AppLog.Info("DELETE TAG - Attempting to delete tag from local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("DELETE TAG - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("DELETE TAG - Connection to <online/local> database opened successfully");

                    SqlCommand DeleteTag = new SqlCommand("DELETE FROM Tags WHERE Id = @Id;", conn);
                    DeleteTag.Parameters.Add(new SqlParameter("Id", _Id));

                    DeleteTag.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("DELETE TAG - Tag {0} deleted from local database successfully", Type.Value));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting tag from local database";
                AppLog.Error("DELETE TAG - " + _ErrMsg + ": " + e);
                return false;
            }

            AppLog.Info("DELETE TAG - Attempting to delete tag on online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("DELETE TAG - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("DELETE TAG - Connection to <online/local> database opened successfully");

                    SqlCommand DeleteTag = new SqlCommand("DELETE FROM t_BTS_Tags WHERE Id = @Id;", conn);
                    DeleteTag.Parameters.Add(new SqlParameter("Id", _Id));

                    DeleteTag.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("DELETE TAG - Tag {0} deleted on online database successfully", Type.Value));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting tag from online database";
                AppLog.Error("DELETE TAG - " + _ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info("DELETE TAG - Success!");
            return true;
        }
    }
}
