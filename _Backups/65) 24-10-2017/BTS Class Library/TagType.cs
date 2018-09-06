using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BenLog;

namespace BTS_Class_Library
{
    public class TagType
    {
        private Guid _Id;
        private Organisation _MyOrg;
        private string _Value;
        public bool Uploaded;
        private string _ErrMsg;

        internal TagType(Guid pId)
        {
            _Id = pId;
        }

        internal TagType(Organisation pOrg)
        {
            _Id = Guid.NewGuid();
            _MyOrg = pOrg;
            Uploaded = false;
        }
        
        public Guid Id { get { return _Id; } }
        public Organisation MyOrg { get { return _MyOrg; } }
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (value.Length > 50)
                {
                    throw new Exception("Tag value exceeds 50 characters");
                }
                else
                {
                    Uploaded = false;
                    _Value = value;
                }
            }
        }

        public string ErrMsg { get { return _ErrMsg; } }

        public bool Create()
        {
            AppLog.Info("CREATE TAGTYPE - Starting...");

            //Checks that data is valid before attempting upload
            AppLog.Info("CREATE TAGTYPE - Validating...");
            if (!Validate())
            {
                AppLog.Info("CREATE TAGTYPE - Tag type failed validation");
                return false;
            }
            AppLog.Info("CREATE TAGTYPE - Tag type validated successfully");

            if (!Data.OfflineMode)
            {//Not in offline mode
                AppLog.Info("CREATE TAGTYPE - Attempting to create tag type on online database...");                
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("CREATE TAGTYPE - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("CREATE TAGTYPE - Connection to online database opened successfully");

                        SqlCommand CreateTagType = new SqlCommand("INSERT INTO t_BTS_TagTypes VALUES(@Id, @OrgId, @Value);",
                            conn);
                        CreateTagType.Parameters.Add(new SqlParameter("Id", _Id));
                        CreateTagType.Parameters.Add(new SqlParameter("OrgId", _MyOrg.Id));
                        CreateTagType.Parameters.Add(new SqlParameter("Value", _Value));

                        CreateTagType.ExecuteNonQuery();

                        AppLog.Info(String.Format("CREATE TAGTYPE - Tag type {0} created on online database successfully",
                        Value));
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while creating tag type on online database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
            }
            else
            {
                AppLog.Info(String.Format("CREATE TAGTYPE - Offline mode is ON. Skipping create tag type on" +
                        "online database"));
            }

            AppLog.Info("CREATE TAGTYPE - Attempting to create tag type on local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("CREATE TAGTYPE - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("CREATE TAGTYPE - Connection to local database opened successfully");

                    //This is a check to see weather the tag type already exists on the database. Obviously
                    //if it's already there, it doesn't need creating again, but this might be called
                    //if for example the tag type did not exist on the local database, so the Create() function
                    //needed to be able to account for that.
                    AppLog.Info("CREATE TAGTYPE - Checking that tag type doesn't already exist on local database");

                    bool LocalTagTypeExists;

                    SqlCommand CheckLocalDb = new SqlCommand("SELECT * FROM TagTypes WHERE Id = @Id", conn);
                    CheckLocalDb.Parameters.Add(new SqlParameter("Id", Id));
                    using (SqlDataReader reader = CheckLocalDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            LocalTagTypeExists = true;
                            AppLog.Info("CREATE TAGTYPE - TagType already exists in local database!");
                        }
                        else
                        {
                            LocalTagTypeExists = false;
                            AppLog.Info("CREATE TAGTYPE - TagType does not exist in local database. Creating tag type on local database");
                        }
                    }

                    if (!LocalTagTypeExists)
                    {
                        SqlCommand CreateTagType = new SqlCommand("INSERT INTO TagTypes VALUES(@Id, @OrgId, @Value, " +
                        "@Uploaded)", conn);
                        CreateTagType.Parameters.Add(new SqlParameter("Id", _Id));
                        CreateTagType.Parameters.Add(new SqlParameter("OrgId", _MyOrg.Id));
                        CreateTagType.Parameters.Add(new SqlParameter("Value", _Value));
                        CreateTagType.Parameters.Add(new SqlParameter("Uploaded", Uploaded));

                        CreateTagType.ExecuteNonQuery();

                        Uploaded = true;
                        AppLog.Info(String.Format("CREATE TAGTYPE - Tag type {0} created on local database successfully",
                            Value));
                    }
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while creating tag type on local database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info("CREATE TAGTYPE - Success!");
            return true;
        }

        public bool Update()
        {
            AppLog.Info("UPDATE TAGTYPE - Starting...");
            if (!Data.OfflineMode)
            {
                AppLog.Info("UPDATE TAGTYPE - Offline mode is OFF");

                AppLog.Info("UPDATE TAGTYPE - Attempting to update tag type on online database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("UPDATE TAGTYPE - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("UPDATE TAGTYPE - Connection to online database opened successfully");

                        SqlCommand UpdateTagType = new SqlCommand("UPDATE t_BTS_TagTypes SET Value = @Value WHERE " +
                            "Id = @Id;", conn);

                        UpdateTagType.Parameters.Add(new SqlParameter("Value", _Value));
                        UpdateTagType.Parameters.Add(new SqlParameter("Id", _Id));

                        UpdateTagType.ExecuteNonQuery();
                    }
                    Uploaded = true;
                    AppLog.Info(String.Format("UPDATE TAGTYPE - Tag type {0} updated on online database successfully", 
                        Value));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while updating tag type on online database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
            }
            else
            {
                AppLog.Info("UPDATE TAGTYPE - Offline mode is ON. Skipping update tag type on online database");
            }

                AppLog.Info("UPDATE TAGTYPE - Attempting to update tag type on local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("UPDATE TAGTYPE - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("UPDATE TAGTYPE - Connection to local database opened successfully");

                    SqlCommand UpdateTagType = new SqlCommand("UPDATE TagTypes SET Value = @Value, " +
                        "Uploaded = @Uploaded WHERE Id = @Id;", conn);

                    UpdateTagType.Parameters.Add(new SqlParameter("Value", _Value));
                    UpdateTagType.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateTagType.Parameters.Add(new SqlParameter("Uploaded", Uploaded));

                    UpdateTagType.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("UPDATE TAGTYPE - Tag type {0} created on local database successfully",
                    Value));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while updating tag type on local database. Changes were not saved";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }
            
            AppLog.Info("UPDATE TAGTYPE - Success!");
            return true;
        }

        public bool Get()
        {
            AppLog.Info("GET TAGTYPE - Starting...");
            if (Data.OfflineMode)
            {
                AppLog.Info("GET TAGTYPE - Attempting to retrieve tag type from local database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                    {
                        AppLog.Info("GET TAGTYPE - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("GET TAGTYPE - Connection to local database opened successfully");
                        
                        SqlCommand GetTagType = new SqlCommand("SELECT * FROM TagTypes WHERE Id = @Id;", conn);
                        GetTagType.Parameters.Add(new SqlParameter("Id", _Id));

                        GetTagType.ExecuteNonQuery();

                        using (SqlDataReader reader = GetTagType.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _MyOrg = new Organisation(new Guid(reader[1].ToString()));
                                _Value = reader[2].ToString();
                                Uploaded = Convert.ToBoolean(reader[3]);
                            }
                        }
                    }
                    AppLog.Info(String.Format("GET TAGTYPE - Tag type {0} retrieved from local database successfully", 
                        Value));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while getting TagType from local database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
            }
            else
            {
                AppLog.Info("GET TAGTYPE - Attempting to retrieve tag type from online database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("GET TAGTYPE - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("GET TAGTYPE - Connection to online database opened successfully");
                        
                        SqlCommand DownloadTagType = new SqlCommand("SELECT * FROM t_BTS_TagTypes WHERE Id = @Id;", conn);
                        DownloadTagType.Parameters.Add(new SqlParameter("Id", _Id));

                        DownloadTagType.ExecuteNonQuery();

                        using (SqlDataReader reader = DownloadTagType.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _MyOrg = new Organisation(new Guid(reader[1].ToString()));
                                _Value = reader[2].ToString();
                            }
                        }
                    }
                    AppLog.Info(String.Format("GET TAGTYPE - Tag type {0} retrieved fron online database successfully", 
                        Value));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while downloading TagType from online database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
                //Finally, check if tag type exists in the local database. If not, ADD THEM!!! If so, UPDATE THEM!!!
                AppLog.Info("GET TAGTYPE - Checking whether tag type exists in local database");

                bool ExistsOnLocalDb;

                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("GET TAGTYPE - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("GET TAGTYPE - Connection to local database opened successfully");



                    SqlCommand CheckLocalDb = new SqlCommand("SELECT * FROM TagTypes WHERE Id = @Id;", conn);
                    CheckLocalDb.Parameters.Add(new SqlParameter("Id", Id));

                    using (SqlDataReader reader = CheckLocalDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ExistsOnLocalDb = true;
                            AppLog.Info("GET TAGTYPE - TagType already exists in the local database!");
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
                        AppLog.Info("GET TAGTYPE - Updated tag type on local db successfully");
                    }
                    else
                    {
                        AppLog.Info("GET TAGTYPE - Failed to update tag type: " + _ErrMsg);
                        return false;
                    }
                }
                else
                {
                    if (Create())
                    {
                        AppLog.Info("GET TAGTYPE - Created tag type on local db successfully");
                    }
                    else
                    {
                        AppLog.Info("GET TAGTYPE - Failed to create tag type: " + _ErrMsg);
                        return false;
                    }
                }

            }
            AppLog.Info("GET TAGTYPE - Success!");
            return true;
        }

        public bool Delete()
        {
            AppLog.Info("DELETE TAGTYPE - Starting...");

            AppLog.Info("DELETE TAGTYPE - Attempting to delete tag type from local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("DELETE TAGTYPE - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("DELETE TAGTYPE - Connection to local database opened successfully");

                    SqlCommand DeleteTagType = new SqlCommand("DELETE FROM TagTypes WHERE Id = @Id;", conn);
                    DeleteTagType.Parameters.Add(new SqlParameter("Id", _Id));

                    DeleteTagType.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("DELETE TAGTYPE - Tag type {0} deleted from local database successfully", 
                    Value));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting TagType from local database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }

            AppLog.Info("DELETE TAGTYPE - Attempting to delete tag type from online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("DELETE TAGTYPE - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("DELETE TAGTYPE - Connection to online database opened successfully");

                    SqlCommand DeleteTagType = new SqlCommand("DELETE FROM TagTypes WHERE Id = @Id;", conn);
                    DeleteTagType.Parameters.Add(new SqlParameter("Id", _Id));

                    DeleteTagType.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("DELETE TAGTYPE - Tag type {0} deleted from online database successfully", 
                    Value));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting TagType from online database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info("DELETE TAGTYPE - Success!");
            return true;
        }
        private bool Validate()
        {
            AppLog.Info("VALIDATE TAGTYPE - Starting...");
            try
            {
                if (Value == null) { _ErrMsg = "TagType has not been given a value";  throw new Exception(); }
                if (Value.Length > 50) { _ErrMsg = "Value of tag type exceeds 50 characters"; throw new Exception(); }
            }
            catch (Exception e)
            {
                AppLog.Error("VALIDATE TAGTYPE - Validation failed: " + _ErrMsg);
                return false;
            }
            AppLog.Info("VALIDATE TAGTYPE - Success!");
            return true;
        }
    }
}
