using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using TextLogger;


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
                if(value.Length < 51)
                {
                    _FName = value;
                }
                else
                {
                    string msg = "Set forename failed. Forename must not exceed 50 characters";
                    Log.Error(msg);
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
                    Log.Error(msg);
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
                    Log.Error(msg);
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
                        Log.Error(msg);
                        throw new Exception(msg);
                    }
                }
                else
                {
                    string msg = "Set email address failed. Email address must be valid";
                    Log.Error(msg);
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
                        Log.Error(msg);
                        throw new Exception(msg);
                    }
                }
                else
                {
                    string msg = "Set password failed. Password must not exceed 50 characters";
                    Log.Error(msg);
                    throw new Exception(msg);
                }
            }
        }

        public DateTime DateTimeCreated { get { return _DateTimeCreated; } }

        public string ErrMsg { get { return _ErrMsg; } }

        public User(Guid pId)
        {
            _Id = pId;
            //if (!Get()) { Data.UserFriendlyError(_ErrMsg); }
        }

        public User()
        {
            _Id = Guid.NewGuid();
            _DateTimeCreated = DateTime.Now;
        }

        public bool Create()
        {
            Log.Info("CREATE USER - Starting...");

            if (Data.OfflineMode)
            {
                _ErrMsg = "Cannot create users while in offline mode";
                Log.Info(String.Format("CREATE USER - User {0} was not created because offline mode is on", _Username));
                return false;
            }

            Log.Info("CREATE USER - OfflineMode is OFF. Attempting to create user on online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    Log.Info("CREATE USER - Attempting to open connection to online database...");
                    conn.Open();
                    Log.Info("CREATE USER - Connection to online database opened successfully");

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
                _ErrMsg = "Error while creating user on online database";
                Log.Error(_ErrMsg + ": " + e);
                return false;
            }

            Log.Info(String.Format("CREATE USER - User {0} created on online database successfully", _Username));
            Log.Info("CREATE USER - Attempting to create user on local database...");

            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    Log.Info("CREATE USER - Attempting to open connection to local database...");
                    conn.Open();
                    Log.Info("CREATE USER - Connection to local database opened successfully");

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
            catch (SqlException e)
            {
                _ErrMsg = "Error while creating user on local database";
                Log.Error(_ErrMsg + ": " + e);
                return false;
            }

            Log.Info(String.Format("CREATE USER - User {0} created on local database successfully", _Username));

            Log.Info(String.Format("CREATE USER - Success!", _Username));

            return true;
        }

        public bool Update()
        {
            Log.Info("UPDATE User - Starting...");

            if (Data.OfflineMode)
            {
                _ErrMsg = "Cannot edit users while in offline mode";
                Log.Info(String.Format("UPDATE USER - User {0} was not updated because offline mode is on", _Username));
                return false;
            }


            Log.Info("UPDATE USER - Attempting to update user on online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {

                    Log.Info("UPDATE USER - Attempting to open connection to online database...");
                    conn.Open();
                    Log.Info("UPDATE USER - Connection to online database opened successfully");


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

                    UpdateUser.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while updating user on online database";
                Log.Error(_ErrMsg + ": " + e);
                return false;
            }
            Log.Info(String.Format("UPDATE USER - User {0} created on online database successfully", _Username));

            Log.Info("UPDATE USER - Attempting to update user on local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    Log.Info("UPDATE USER - Attempting to open connection to local database...");
                    conn.Open();
                    Log.Info("UPDATE USER - Connection to local database opened successfully");

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

                    UpdateUser.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while updating user on local database";
                Log.Error(_ErrMsg + ": " + e);
                return false;
            }
            Log.Info(String.Format("UPDATE USER - User {0} created on local database successfully", _Username));
            return true;
        }

        public bool Get()
        {
            Log.Info("GET User - Starting...");

            if (Data.OfflineMode)
            {
                Log.Info("GET USER - Offline mode is ON, retrieving cached user data from local database");

                Log.Info("GET USER - Attempting to get user from local database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                    {
                        Log.Info("GET USER - Attempting to open connection to local database...");
                        conn.Open();
                        Log.Info("GET USER - Connection to local database opened successfully");
                        

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
                                _ErrMsg = "Error while downloading user from online database. No data was returned";
                                Log.Error(_ErrMsg);
                                return false;
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while downloading user from local database";
                    Log.Error(_ErrMsg + ": " + e);
                    return false;
                }
                Log.Info(String.Format("GET USER - Got {0} from local database successfully", _Username));

            }
            else
            {
                Log.Info("GET USER - Offline mode is OFF, retrieving user data from online database");
                Log.Info("GET USER - Attempting to <function> user on <online/local> database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        Log.Info("GET USER - Attempting to open connection to online database...");
                        
                        conn.Open();
                        Log.Info("GET USER - Connection to online database opened successfully");

                        SqlCommand DownloadUser = new SqlCommand("SELECT * FROM t_BTS_Users WHERE Id = @Id;", conn);

                        DownloadUser.Parameters.Add(new SqlParameter("Id", _Id));

                        //DownloadUser.ExecuteNonQuery();

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
                                _ErrMsg = "Error while downloading user from online database. No data was returned";
                                Log.Error(_ErrMsg);
                                return false;
                            }
                        }
                    }
                }

                
                catch (SqlException e)
                {
                    _ErrMsg = "Error while downloading user from online database";
                    Log.Error(_ErrMsg + ": " + e);
                    return false;
                }
                Log.Info(String.Format("GET USER - Got {0} from online database successfully", _Username));
            }


            Log.Info(String.Format("GET USER - Success!"));
            
            return true;
        }

        public bool Delete()
        {
            Log.Info("DELETE USER - Starting...");

            Log.Info("DELETE USER - Attempting to delete user on online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    Log.Info("DELETE USER - Attempting to open connection to online database...");
                    conn.Open();
                    Log.Info("DELETE USER - Connection to online database opened successfully");

                    SqlCommand DeleteUser = new SqlCommand("DELETE FROM t_BTS_Users WHERE Id = @Id;", conn);
                    DeleteUser.Parameters.Add(new SqlParameter("Id", _Id));

                    DeleteUser.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting user on online database";
                Log.Error(_ErrMsg + ": " + e);
                return false;
            }
            Log.Info(String.Format("DELETE USER - User {0} deleted from online database successfully", _Username));

            Log.Info("DELETE USER - Attempting to delete user from local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    
                    Log.Info("DELETE USER - Attempting to open connection to local database...");
                    conn.Open();
                    Log.Info("DELETE USER - Connection to local database opened successfully");

                    SqlCommand DeleteUser = new SqlCommand("DELETE FROM Users WHERE Id = @Id;", conn);
                    DeleteUser.Parameters.Add(new SqlParameter("Id", _Id));

                    DeleteUser.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting user from local database";
                Log.Error(_ErrMsg + ": " + e);
                return false;
            }
            Log.Info(String.Format("DELETE USER - User {0} deleted from local database successfully", _Username));

            Log.Info(String.Format("DELETE USER - Success!"));

            return true;
        }
    }
}
