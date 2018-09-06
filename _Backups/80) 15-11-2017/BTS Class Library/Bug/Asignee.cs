using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BenLog;

namespace BTS_Class_Library
{

    public class Assignee
    {
        private Guid _BugId;
        private User _MyUser;
        private TimeSpan _TimeSpent;
        private int _AccessLevel;
        private DateTime _DateTimeCreated;
        public bool Uploaded;
        private string _ErrMsg;

        public Guid BugId { get { return _BugId; } }
        public User MyUser { get { return _MyUser; } }
        public TimeSpan TimeSpent { get { return _TimeSpent; } set { Uploaded = false; _TimeSpent += value; } }
        public int AccessLevel { get { return _AccessLevel; } set { Uploaded = false; AccessLevel = value; } }
        public DateTime DateTimeCreated { get { return _DateTimeCreated; } }
        public string ErrMsg { get { return _ErrMsg; } }

        

        internal Assignee(Bug pBug, User pUser)
        {
            _BugId = pBug.Id;
            _MyUser = pUser;
        }

        public bool Create()
        {
            AppLog.Info("CREATE ASSIGNEE - Starting...");

            //Checks that data is valid before attempting upload
            AppLog.Info("CREATE ASSIGNEE - Validating...");
            if (!Validate())
            {
                AppLog.Info("CREATE ASSIGNEE - Assignee failed validation");
                return false;
            }
            AppLog.Info("CREATE ASSIGNEE - Assignee validated successfully");

            _DateTimeCreated = DateTime.Now;

            if (!Data.OfflineMode)
            {
                AppLog.Info(String.Format("CREATE ASSIGNEE - Offline mode is OFF"));

                AppLog.Info("CREATE ASSIGNEE - Attempting to create assignee on online database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("CREATE ASSIGNEE - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("CREATE ASSIGNEE - Connection to online database opened successfully");

                        //This is a check to see weather the assignee already exists on the database. Obviously
                        //if it's already there, it doesn't need creating again, but this might be called
                        //if for example the assignee did not exist on the local database, so the Create() function
                        //needed to be able to account for that.
                        AppLog.Info("CREATE ASSIGNEE - Checking that assignee doesn't already exist on online database");
                        bool OnlineAssigneeExists;

                        SqlCommand CheckOnlineDb = new SqlCommand("SELECT * FROM t_BTS_Assignees WHERE BugId = @BugId AND " +
                            "UserId = @UserId;", conn);
                        CheckOnlineDb.Parameters.Add(new SqlParameter("BugId", BugId));
                        CheckOnlineDb.Parameters.Add(new SqlParameter("UserId", MyUser.Id));
                        using (SqlDataReader reader = CheckOnlineDb.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                OnlineAssigneeExists = true;
                                AppLog.Info("CREATE ASSIGNEE - Assignee already exists in online database!");
                            }
                            else
                            {
                                OnlineAssigneeExists = false;
                                AppLog.Info("CREATE ASSIGNEE - Assignee does not exist in online database. Creating assignee on online database");
                            }
                        }

                        if (!OnlineAssigneeExists)
                        {
                            SqlCommand CreateAssignee = new SqlCommand("INSERT INTO t_BTS_Assignees VALUES (@BugId, " +
                                                        "@UserId, @TimeSpent, @AccessLevel, @DateTimeCreated);", conn);

                            CreateAssignee.Parameters.Add(new SqlParameter("BugId", _BugId));
                            CreateAssignee.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));
                            CreateAssignee.Parameters.Add(new SqlParameter("TimeSpent", _TimeSpent.Ticks));
                            CreateAssignee.Parameters.Add(new SqlParameter("Accesslevel", _AccessLevel));
                            CreateAssignee.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));

                            CreateAssignee.ExecuteNonQuery();
                        }
                        Uploaded = true;
                        AppLog.Info(String.Format("CREATE ASSIGNEE - Assignee {0} created on online database successfully",
                        _MyUser.Username));
                    }
                        
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while creating assignee on online database";
                    AppLog.Error("CREATE ASSIGNEE - " + _ErrMsg + ": " + e); ;
                    return false;
                }

            }
            else
            {
                AppLog.Info(String.Format("CREATE ASSIGNEE - Offline mode is ON. Skipping create user on" +
                    "online database"));
            }

            AppLog.Info("CREATE ASSIGNEE - Attempting to create assignee on local database...");
            try
            {

                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("CREATE ASSIGNEE - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("CREATE ASSIGNEE - Connection to local database opened successfully");

                    //This is a check to see weather the assignee already exists on the database. Obviously
                    //if it's already there, it doesn't need creating again, but this might be called
                    //if for example the assignee did not exist on the local database, so the Create() function
                    //needed to be able to account for that.
                    AppLog.Info("CREATE ASSIGNEE - Checking that assignee doesn't already exist on local database");
                    bool LocalAssigneeExists;

                    SqlCommand CheckLocalDb = new SqlCommand("SELECT * FROM Assignees WHERE BugId = @BugId AND " +
                        "UserId = @UserId;", conn);
                    CheckLocalDb.Parameters.Add(new SqlParameter("BugId", BugId));
                    CheckLocalDb.Parameters.Add(new SqlParameter("UserId", MyUser.Id));
                    using (SqlDataReader reader = CheckLocalDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            LocalAssigneeExists = true;
                            AppLog.Info("CREATE ASSIGNEE - Assignee already exists in local database!");
                        }
                        else
                        {
                            LocalAssigneeExists = false;
                            AppLog.Info("CREATE ASSIGNEE - Assignee does not exist in local database. Creating assignee on local database");
                        }
                    }

                    if (!LocalAssigneeExists)
                    {
                        SqlCommand CreateAssignee = new SqlCommand("INSERT INTO Assignees VALUES (@BugId, @UserId, " +
                        "@TimeSpent, @AccessLevel, @DateTimeCreated, @Uploaded);", conn);

                        CreateAssignee.Parameters.Add(new SqlParameter("BugId", _BugId));
                        CreateAssignee.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));
                        CreateAssignee.Parameters.Add(new SqlParameter("TimeSpent", _TimeSpent.Ticks));
                        CreateAssignee.Parameters.Add(new SqlParameter("Accesslevel", _AccessLevel));
                        CreateAssignee.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));
                        CreateAssignee.Parameters.Add(new SqlParameter("Uploaded", Uploaded));

                        CreateAssignee.ExecuteNonQuery();
                    }
                    AppLog.Info(String.Format("CREATE ASSIGNEE - Assignee {0} created on local database successfully",
                    _MyUser.Username));
                }
            }
            catch (SqlException e)
            {
                    _ErrMsg = "Error while creating assignee on local database. Changes were not saved";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
            }
            AppLog.Info(String.Format("CREATE ASSIGNEE - Success!"));
            return true;
        }

        public bool Update()
        {
            AppLog.Info("UPDATE ASSIGNEE - Starting...");

            if (!Data.OfflineMode)
            {
                AppLog.Info("UPDATE ASSIGNEE - Offline mode is OFF");

                AppLog.Info("UPDATE ASSIGNEE - Attempting to update assignee on online database...");
                try
                {

                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("UPDATE ASSIGNEE - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("UPDATE ASSIGNEE - Connection to online database opened successfully");

                        SqlCommand UpdateAssignee = new SqlCommand("UPDATE t_BTS_Assignees SET TimeSpent = " +
                            "@TimeSpent, AccessLevel = @AccessLevel WHERE BugId = @BugId AND UserId = @UserId;", conn);
                        UpdateAssignee.Parameters.Add(new SqlParameter("BugId", _BugId));
                        UpdateAssignee.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));
                        UpdateAssignee.Parameters.Add(new SqlParameter("TimeSpent", _TimeSpent.Ticks));
                        UpdateAssignee.Parameters.Add(new SqlParameter("Accesslevel", _AccessLevel));

                        UpdateAssignee.ExecuteNonQuery();
                    }
                    Uploaded = true;
                    AppLog.Info(String.Format("UPDATE ASSIGNEE - Assignee {0} updated on online database " +
                    "successfully", _MyUser.Username));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while updating assignee on online database";
                    AppLog.Error("UPDATE ASSIGNEE - " + _ErrMsg + ": " + e); ;
                    return false;
                }

            }
            else
            {
                AppLog.Info("UPDATE ASSIGNEE - Offline mode is ON. Skipping update assignee on online database");
            }

            AppLog.Info("UPDATE ASSIGNEE - Attempting to update assignee on local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("UPDATE ASSIGNEE - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("UPDATE ASSIGNEE - Connection to local database opened successfully");

                    SqlCommand UpdateAssignee = new SqlCommand("UPDATE Assignees SET TimeSpent = @TimeSpent, " +
                        "AccessLevel = @AccessLevel WHERE BugId = @BugId AND UserId = @UserId;", conn);
                    UpdateAssignee.Parameters.Add(new SqlParameter("BugId", _BugId));
                    UpdateAssignee.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));
                    UpdateAssignee.Parameters.Add(new SqlParameter("TimeSpent", _TimeSpent.Ticks));
                    UpdateAssignee.Parameters.Add(new SqlParameter("Accesslevel", _AccessLevel));

                    UpdateAssignee.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("UPDATE ASSIGNEE - Assignee {0} updated on local database successfully",
                _MyUser.FullName));
            }
            catch (SqlException e)
            {
                if (Data.OfflineMode)
                {
                    _ErrMsg = "Error while updating assignee on local database. Changes were not saved.";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
            }
            AppLog.Info("UPDATE ASSIGNEE - Success!");
            return true;
        }

        public bool Get()
        {
            AppLog.Info("GET ASSIGNEE - Starting...");
            if (Data.OfflineMode)
            {
                AppLog.Info("GET ASSIGNEE - Attempting to get assignee from local database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                    {
                        AppLog.Info("GET ASSIGNEE - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("GET ASSIGNEE - Connection to local database opened successfully");


                        SqlCommand GetAssignee = new SqlCommand("SELECT * FROM Assignees WHERE BugId = @BugId AND " +
                            "UserId = UserId;", conn);
                        GetAssignee.Parameters.Add(new SqlParameter("BugId", _BugId));
                        GetAssignee.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));

                        using (SqlDataReader reader = GetAssignee.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _TimeSpent = TimeSpan.FromTicks(Convert.ToInt64(reader[2]));
                                _AccessLevel = Convert.ToInt32(reader[3]);
                                _DateTimeCreated = Convert.ToDateTime(reader[4]);
                                Uploaded = Convert.ToBoolean(reader[5]);
                            }
                        }
                    }
                    AppLog.Info(String.Format("GET ASSIGNEE - Assignee {0} downloaded from local database " +
                        "successfully", MyUser.Username));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while getting assignee from local database";
                    AppLog.Error("GET ASSIGNEE - " + _ErrMsg + ": " + e); ;
                    return false;
                }
            }
            else
            {
                AppLog.Info("GET ASSIGNEE - Attempting to get assignee from online database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("GET ASSIGNEE - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("GET ASSIGNEE - Connection to online database opened successfully");


                        SqlCommand GetAssignee = new SqlCommand("SELECT * FROM t_BTS_Assignees WHERE BugId = " +
                            "@BugId AND UserId = UserId;", conn);
                        GetAssignee.Parameters.Add(new SqlParameter("BugId", _BugId));
                        GetAssignee.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));

                        using (SqlDataReader reader = GetAssignee.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _TimeSpent = TimeSpan.FromTicks(Convert.ToInt64(reader[2]));
                                _AccessLevel = Convert.ToInt32(reader[3]);
                                _DateTimeCreated = Convert.ToDateTime(reader[4]);
                            }
                        }
                    }
                    AppLog.Info(String.Format("GET ASSIGNEE - Assignee {0} downloaded from online database " +
                        "successfully", _MyUser.Username));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while getting assignee from online database";
                    AppLog.Error("GET ASSIGNEE - " + _ErrMsg + ": " + e); ;
                    return false;
                }
                //Finally, check if assignee exists in the local database. If not, ADD THEM!!! If so, UPDATE THEM!!!
                AppLog.Info("GET ASSIGNEE - Checking whether assignee exists in local database");

                bool ExistsOnLocalDb;

                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("GET ASSIGNEE - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("GET ASSIGNEE - Connection to local database opened successfully");



                    SqlCommand CheckLocalDb = new SqlCommand("SELECT * FROM Assignees WHERE BugId = @BugId AND " +
                        "UserId = @UserId;", conn);
                    CheckLocalDb.Parameters.Add(new SqlParameter("BugId", BugId));
                    CheckLocalDb.Parameters.Add(new SqlParameter("UserId", MyUser.Id));

                    using (SqlDataReader reader = CheckLocalDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ExistsOnLocalDb = true;
                            AppLog.Info("GET ASSIGNEE - Assignee already exists in the local database!");
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
                        AppLog.Info("GET ASSIGNEE - Updated assignee on local db successfully");
                    }
                    else
                    {
                        AppLog.Info("GET ASSIGNEE - Failed to update assignee: " + _ErrMsg);
                        return false;
                    }
                }
                else
                {
                    if (Create())
                    {
                        AppLog.Info("GET ASSIGNEE - Created assignee on local db successfully");
                    }
                    else
                    {
                        AppLog.Info("GET ASSIGNEE - Failed to create assignee: " + _ErrMsg);
                        return false;
                    }
                }
            }
            AppLog.Info("GET ASSIGNEE - Success!");
            return true;
        }

        public bool Delete()
        {
            AppLog.Info("DELETE ASSIGNEE - Starting...");
            AppLog.Info("DELETE ASSIGNEE - Attempting to delete assignee on online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("DELETE ASSIGNEE - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("DELETE ASSIGNEE - Connection to online database opened successfully");

                    SqlCommand DeleteAssignee = new SqlCommand("DETETE FROM t_BTS_Assignees WHERE BugId = @BugId " +
                        "AND Assignee = @Assignee;", conn);
                    DeleteAssignee.Parameters.Add(new SqlParameter("BugId", _BugId));
                    DeleteAssignee.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));

                    DeleteAssignee.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("DELETE ASSIGNEE - <Class> {0} deleted from online database successfully",
                    _MyUser.Username));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting assignee from online database";
                AppLog.Error("DELETE ASSIGNEE - " + _ErrMsg + ": " + e); ;
                return false;
            }

            AppLog.Info("DELETE ASSIGNEE - Attempting to delete assignee on local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("DELETE ASSIGNEE - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("DELETE ASSIGNEE - Connection to local database opened successfully");

                    SqlCommand DeleteAssignee = new SqlCommand("DETETE FROM Assignees WHERE BugId = @BugId AND " +
                        "Assignee = @Assignee;", conn);
                    DeleteAssignee.Parameters.Add(new SqlParameter("BugId", _BugId));
                    DeleteAssignee.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));

                    DeleteAssignee.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("DELETE ASSIGNEE - Assignee {0} deleted from local database successfully",
                    _MyUser.Username));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting assignee from local database";
                AppLog.Error("DELETE ASSIGNEE - " + _ErrMsg + ": " + e); ;
                return false;
            }
            AppLog.Info("DELETE ASSIGNEE - Success!");
            return true;
        }

        private bool Validate()
        {
            AppLog.Info("VALIDATE USER - Starting...");
            try
            {
                //Check that access level is within range (not decided what the range is yet)
                AppLog.Info("VALIDATE USER - Validation has not been implemented for assignee yet");
            }
            catch (Exception e)
            {
                _ErrMsg = e.ToString();
                AppLog.Error("VALIDATE USER - Validation failed: " + _ErrMsg);
                return false;
            }
            AppLog.Info("VALIDATE USER - Success!");
            return true;
        }
    }
}

