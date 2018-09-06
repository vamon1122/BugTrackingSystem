using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BenLog;


namespace BTS_Class_Library
{
    public class User
    {
        private Guid _Id;
        private string _FName;
        private string _SName;
        private string _Username;
        private string _JobTitle;
        private List<Organisation> _Organisations;
        private string _EMail;
        private string _Password;
        private DateTime _DateTimeCreated;
        private string _ErrMsg;

        public Guid Id { get { return _Id; } }

        public string FName {
            get {
                return _FName;
            }
            set
            {
                if (value.Length < 51)
                {
                    _FName = value;
                }
                else
                {
                    string msg = "Set forename failed. Forename must not exceed 50 characters";
                    AppLog.Error(msg);
                    throw new Exception(msg);
                }
            }
        }

        public string SName {
            get {
                return _SName;
            }
            set {
                if(value.Length < 51)
                {
                    _SName = value;
                }
                else
                {
                    string msg = "Set surname failed. Surname must not exceed 50 characters";
                    AppLog.Error(msg);
                    throw new Exception(msg);
                }
            }
        }

        public string FullName { get { return FName + " " + SName; } }

        public string Username
        {
            get { return _Username; }
            set
            {
                if (value.Length < 51)
                {
                    _Username = value;
                }
            }
        }

        public string JobTitle {
            get {
                return _JobTitle;
            }
            set
            { if (value.Length < 51)
                {
                    _JobTitle = value;
                } else {
                    string msg = "Set job title failed. Job title must not exceed 50 characters";
                    AppLog.Error(msg);
                    throw new Exception(msg);
                }
            }
        }

        public List<Organisation> Organisations { get { return _Organisations; } }

        public string EMail { get {
                return _EMail;
            }
            set {
                if (value.Contains("@") && value.Contains("."))
                {
                    if (value.Length < 255)
                    {
                        _EMail = value;
                    }
                    else
                    {
                        string msg = "Set email address failed. Email address must not exceed 254 characters";
                        AppLog.Error(msg);
                        throw new Exception(msg);
                    }
                }
                else
                {
                    string msg = "Set email address failed. Email address must be valid";
                    AppLog.Error(msg);
                    throw new Exception(msg);
                }
            }
        }

        public string Password { get { return _Password; }
            set {
                if(value.Length < 51)
                {
                    if(value.Length > 6)
                    {
                        _Password = value;
                    }
                    else
                    {
                        string msg = "Set password failed. Password must contain at least 6 characters";
                        AppLog.Error(msg);
                        throw new Exception(msg);
                    }
                }
                else
                {
                    string msg = "Set password failed. Password must not exceed 50 characters";
                    AppLog.Error(msg);
                    throw new Exception(msg);
                }
            }
        }

        public DateTime DateTimeCreated { get { return _DateTimeCreated; } }

        public string ErrMsg { get { return _ErrMsg; } }

        public User(Guid pId)
        {
            _Id = pId;
        }

        public User()
        {
            _Id = Guid.NewGuid();
            _DateTimeCreated = DateTime.Now;
        }

        public bool Create()
        {
            //Show identifiers in log
            AppLog.Info("CREATE USER - Starting...");
            AppLog.Info("CREATE USER - User Id: " + _Id);
            AppLog.Info("CREATE USER - User's Full Name: " + FullName);
            AppLog.Info("CREATE USER - User's Username: " + _Username);

            //Checks offline mode (can't create users in offline mode)
            if (Data.OfflineMode)
            {
                _ErrMsg = "Cannot create users while in offline mode";
                AppLog.Info(String.Format("CREATE USER - User {0} was not created because offline mode is on", _Username));
                return false;
            }

            AppLog.Info("CREATE USER - OfflineMode is OFF. Attempting to create user on online database...");

            //Try creating user online first
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("CREATE USER - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("CREATE USER - Connection to online database opened successfully");

                    SqlCommand CreateUser = new SqlCommand("INSERT INTO t_BTS_Users VALUES(@Id, @FName, @SName, " +
                        "@Username, @JobTitle, @EMail, @Password, @DateTimeCreated);", conn);
                    CreateUser.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateUser.Parameters.Add(new SqlParameter("FName", _FName));
                    CreateUser.Parameters.Add(new SqlParameter("SName", _SName));
                    CreateUser.Parameters.Add(new SqlParameter("Username", _Username));
                    CreateUser.Parameters.Add(new SqlParameter("JobTitle", _JobTitle));
                    CreateUser.Parameters.Add(new SqlParameter("EMail", _EMail));
                    CreateUser.Parameters.Add(new SqlParameter("Password", _Password));
                    CreateUser.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));

                    CreateUser.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                //Nothing really needs to happen here, neither of the databases have been affected
                _ErrMsg = "Error while creating user on online database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }

            AppLog.Info(String.Format("CREATE USER - User {0} created on online database successfully", _Username));
            AppLog.Info("CREATE USER - Attempting to create user on local database...");

            //If user is created on online database without throwing errors, 
            //try and create user offline
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("CREATE USER - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("CREATE USER - Connection to local database opened successfully");

                    SqlCommand CreateUser = new SqlCommand("INSERT INTO Users VALUES(@Id, @FName, @SName, @Username, " +
                        "@JobTitle, @EMail, @Password, @DateTimeCreated);", conn);
                    CreateUser.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateUser.Parameters.Add(new SqlParameter("FName", _FName));
                    CreateUser.Parameters.Add(new SqlParameter("SName", _SName));
                    CreateUser.Parameters.Add(new SqlParameter("Username", _Username));
                    CreateUser.Parameters.Add(new SqlParameter("JobTitle", _JobTitle));
                    CreateUser.Parameters.Add(new SqlParameter("EMail", _EMail));
                    CreateUser.Parameters.Add(new SqlParameter("Password", _Password));
                    CreateUser.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));

                    CreateUser.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                //If an exception is thrown while creating user on local database,
                //delete the user to reset changes.

                _ErrMsg = "Error while creating user on local database. User will now be deleted.";
                AppLog.Error(_ErrMsg + ": " + e);
                AppLog.Info("Attempting to resolve error by deleting user...");
                if (Delete())
                {
                    AppLog.Info("User deleted. Error resolved successfully!");
                }
                else
                {
                    AppLog.Info("User not deleted. FATAL ERROR!!!");
                    throw new Exception("Fatal error while creating user. Could not revert changes.");
                }
                return false;
            }

            AppLog.Info(String.Format("CREATE USER - User {0} created on local database successfully", _Username));

            AppLog.Info(String.Format("CREATE USER - Success!", _Username));

            return true;
        }

        public bool Update()
        {
            //Shows identifiers in log
            AppLog.Info("UPDATE USER - Starting...");
            AppLog.Info("UPDATE USER - User Id: " + _Id);
            AppLog.Info("UPDATE USER - User's Full Name: " + FullName);
            AppLog.Info("UPDATE USER - User's Username: " + _Username);

            int AffectedRows = 0;

            if (Data.OfflineMode)
            {
                //Users cannot be edited in offline mode
                _ErrMsg = "Cannot edit users while in offline mode";
                AppLog.Info(String.Format("UPDATE USER - User {0} was not updated because offline mode is on", _Username));
                return false;
            }

            //Creates a backup of the user (from online database) before edit is attempted
            AppLog.Info("UPDATE USER - Creating a backup of the current user...");
            AppLog.Info("");
            AppLog.Info(@"////////// Backup \\\\\\\\\\"); //This is just to make it display in the log
            User BackupUser = new User(Id);               //a bit nicer
            if (!BackupUser.Get())
            {
                _ErrMsg = "Error while backing up user";
                AppLog.Error(_ErrMsg);
                return false;
            }
            
            AppLog.Info(@"//////////////\\\\\\\\\\\\\\");
            AppLog.Info("");
            AppLog.Info("UPDATE USER - User backed up successfully!");

            //Once the user is backed up, the update can begin starting with the online database
            AppLog.Info("UPDATE USER - Attempting to update user on online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {

                    AppLog.Info("UPDATE USER - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("UPDATE USER - Connection to online database opened successfully");


                    SqlCommand UpdateUser = new SqlCommand("UPDATE t_BTS_Users SET FName = @FName, SName = @SName," +
                        "Username = @Username, JobTitle = @JobTitle, EMail = @EMail, Password = @Password " +
                        "WHERE Id = @Id;", conn);

                    UpdateUser.Parameters.Add(new SqlParameter("FName", _FName));
                    UpdateUser.Parameters.Add(new SqlParameter("SName", _SName));
                    UpdateUser.Parameters.Add(new SqlParameter("Username", _Username));
                    UpdateUser.Parameters.Add(new SqlParameter("JobTitle", _JobTitle));
                    UpdateUser.Parameters.Add(new SqlParameter("EMail", _EMail));
                    UpdateUser.Parameters.Add(new SqlParameter("Password", _Password));
                    UpdateUser.Parameters.Add(new SqlParameter("Id", _Id));

                    AffectedRows = UpdateUser.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                //Nothing really needs to happen here because neither databases have been affected
                _ErrMsg = "Error while updating user on online database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info(String.Format("UPDATE USER - User {0} updated on online database successfully. " +
                "{1} row(s) affected", _Username, AffectedRows));

            AffectedRows = 0;

            //If user has been updated on the online database successfully, update the local database
            AppLog.Info("UPDATE USER - Attempting to update user on local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("UPDATE USER - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("UPDATE USER - Connection to local database opened successfully");

                    SqlCommand UpdateUser = new SqlCommand("UPDATE Users SET FName = @FName, SName = @SName," +
                        "Username = @Username, JobTitle = @JobTitle, EMail = @EMail, Password = @Password " +
                        "WHERE Id = @Id;", conn);

                    UpdateUser.Parameters.Add(new SqlParameter("FName", _FName));
                    UpdateUser.Parameters.Add(new SqlParameter("SName", _SName));
                    UpdateUser.Parameters.Add(new SqlParameter("Username", _Username));
                    UpdateUser.Parameters.Add(new SqlParameter("JobTitle", _JobTitle));
                    UpdateUser.Parameters.Add(new SqlParameter("EMail", _EMail));
                    UpdateUser.Parameters.Add(new SqlParameter("Password", _Password));
                    UpdateUser.Parameters.Add(new SqlParameter("Id", _Id));

                    AffectedRows = UpdateUser.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                //If creating user on the local database fails, changes to the online database
                //need to be reverted. This is done by deleting the user, then changing this' 
                //values to match the backup values, then 'Create' the user on database again.

                AppLog.Info("");
                AppLog.Info(@"////////// Restore \\\\\\\\\\");

                AppLog.Info("UPDATE USER - Attempting to delete user...");

                if (!Delete())
                {
                    _ErrMsg = "UPDATE USER - Error while trying to delete user";
                    AppLog.Info(_ErrMsg);
                    return false;
                }
                AppLog.Info("UPDATE USER - User deleted successfully!");

                AppLog.Info("UPDATE USER - Restoring values from backup...");
                this._Id = BackupUser._Id;
                this._FName = BackupUser._FName;
                this._SName = BackupUser.SName;
                this._Username = BackupUser.Username;
                this._JobTitle = BackupUser.JobTitle;
                this._Organisations = BackupUser.Organisations;
                this.EMail = BackupUser.EMail;
                this._Password = BackupUser._Password;
                this._DateTimeCreated = BackupUser.DateTimeCreated;
                AppLog.Info("UPDATE USER - Values restored successfully!");
                AppLog.Info("UPDATE USER - Attempting upload...");
                if (!Delete())
                {
                    _ErrMsg = "UPDATE USER - Error while trying to upload user";
                    AppLog.Info(_ErrMsg);
                    return false;
                }
                AppLog.Info("UPDATE USER - User uploaded successfully!");

                AppLog.Info(@"///////////////\\\\\\\\\\\\\\\");




                _ErrMsg = "Error while updating user on local database. Changes have been reverted";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info(String.Format("UPDATE USER - User {0} updated on local database successfully. " +
                "{1} row(s) affected", _Username, AffectedRows));
            return true;
        }

        public bool Get()
        {
            //Print identifiers to log
            AppLog.Info("GET USER - Starting...");
            AppLog.Info("GET USER - User Id: " + _Id);

            //If offline mode is on, cached user can be retrieved from the local database
            if (Data.OfflineMode)
            {
                AppLog.Info("GET USER - Offline mode is ON. Attempting to get user from local database...");

                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                    {
                        AppLog.Info("GET USER - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("GET USER - Connection to local database opened successfully");
                        

                        SqlCommand GetUser = new SqlCommand("SELECT * FROM Users WHERE Id = @Id;", conn);

                        GetUser.Parameters.Add(new SqlParameter("Id", _Id));

                        GetUser.ExecuteNonQuery();

                        using (SqlDataReader reader = GetUser.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _FName = reader[1].ToString().Trim();
                                _SName = reader[2].ToString().Trim();
                                _Username = reader[3].ToString().Trim();
                                _JobTitle = reader[4].ToString().Trim();
                                _EMail = reader[5].ToString().Trim();
                                _Password = reader[6].ToString().Trim();
                                _DateTimeCreated = Convert.ToDateTime(reader[7]);
                            }
                            else
                            {
                                //If reader.Read() returns false, no data was returned
                                _ErrMsg = "Error while downloading user from online database. No data was returned";
                                AppLog.Error(_ErrMsg);
                                return false;
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while downloading user from local database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
                AppLog.Info(String.Format("GET USER - Got {0} from local database successfully", _Username));

            }
            else //If offline mode is off, latest copy of user will be downloaded from the online database
            {
                AppLog.Info("GET USER - Offline mode is OFF. Attempting to download user from online database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("GET USER - Attempting to open connection to online database...");
                        
                        conn.Open();
                        AppLog.Info("GET USER - Connection to online database opened successfully");

                        SqlCommand DownloadUser = new SqlCommand("SELECT * FROM t_BTS_Users WHERE Id = @Id;", conn);

                        DownloadUser.Parameters.Add(new SqlParameter("Id", _Id));

                        using (SqlDataReader reader = DownloadUser.ExecuteReader())
                        {
                            if(reader.Read())
                            {
                                _FName = reader[1].ToString().Trim();
                                _SName = reader[2].ToString().Trim();
                                _Username = reader[3].ToString().Trim();
                                _JobTitle = reader[4].ToString().Trim();
                                _EMail = reader[5].ToString().Trim();
                                _Password = reader[6].ToString().Trim();
                                _DateTimeCreated = Convert.ToDateTime(reader[7]);
                            }
                            else
                            {
                                //If reader.Read() returns false, no data was returned.
                                _ErrMsg = "Error while downloading user from online database. No data was returned";
                                AppLog.Error(_ErrMsg);
                                return false;
                            }
                        }
                    }
                }

                
                catch (SqlException e)
                {
                    _ErrMsg = "Error while downloading user from online database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
                AppLog.Info(String.Format("GET USER - Got {0} from online database successfully", _Username));
            }


            AppLog.Info(String.Format("GET USER - Success!"));
            
            return true;
        }

        public bool Delete()
        {
            //Printing identifiers to log
            AppLog.Info("DELETE USER - Starting...");
            AppLog.Info("DELETE USER - User Id: " + _Id);
            AppLog.Info("DELETE USER - User's Full Name: " + FullName);
            AppLog.Info("DELETE USER - User's Username: " + _Username);

            int AffectedRows = 0;

            //Users can't be deleted in offline mode
            if (Data.OfflineMode)
            {
                _ErrMsg = "Cannot delete users while in offline mode";
                AppLog.Info(String.Format("DELETE USER - User {0} was not deleted because offline mode is on", _Username));
                return false;
            }

            AppLog.Info("DELETE USER - User Id: " + _Id);

            //If offline mode is off, first try deleting from online database
            AppLog.Info("DELETE USER - Attempting to delete user on online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("DELETE USER - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("DELETE USER - Connection to online database opened successfully");

                    SqlCommand DeleteUser = new SqlCommand("DELETE FROM t_BTS_Users WHERE Id = @Id;", conn);
                    DeleteUser.Parameters.Add(new SqlParameter("Id", _Id));

                    AffectedRows = DeleteUser.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting user on online database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info(String.Format("DELETE USER - User {0} deleted from online database successfully. " +
                "{1} row(s) affected", _Username,AffectedRows));

            AffectedRows = 0;

            AppLog.Info("DELETE USER - Attempting to delete user from local database...");

            //If deleted from online database successfully, try local database
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    
                    AppLog.Info("DELETE USER - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("DELETE USER - Connection to local database opened successfully");

                    SqlCommand DeleteUser = new SqlCommand("DELETE FROM Users WHERE Id = @Id;", conn);
                    DeleteUser.Parameters.Add(new SqlParameter("Id", _Id));

                    AffectedRows = DeleteUser.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting user from local database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info(String.Format("DELETE USER - User {0} deleted from local database successfully. " +
                "{1} row(s) affected", _Username, AffectedRows));

            AppLog.Info(String.Format("DELETE USER - Success!"));

            return true;
        }
    }
}
