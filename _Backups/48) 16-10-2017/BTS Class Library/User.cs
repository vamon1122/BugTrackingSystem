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
        private DateTime _DOB;
        private char _Gender;
        private string _Username;
        private string _JobTitle;
        private List<Organisation> _Organisations = new List<Organisation>();
        private string _EMail;
        private string _Password;
        private string _Phone;
        private string _PostCode;
        private string _Address;
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

        public DateTime DOB { get; set; }
        public char Gender { get; set; }

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
                    if(value.Length > 5)
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
            //_DateTimeCreated = DateTime.Now;
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
                AppLog.Info(String.Format("CREATE USER - User {0} was not created because offline mode is " +
                    "on", _Username));
                return false;
            }

            //Checks that data is valid before attempting upload
            AppLog.Info("CREATE USER - Validating...");
            if (!Validate())
            {
                AppLog.Info("CREATE USER - User failed validation");
                return false;
            }
            AppLog.Info("CREATE USER - User validated successfully");

            _DateTimeCreated = DateTime.Now;

            AppLog.Info("CREATE USER - OfflineMode is OFF. Attempting to create user on online database...");

            //Try creating user online first
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("CREATE USER - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("CREATE USER - Connection to online database opened successfully");

                    SqlCommand CreateUser = new SqlCommand("INSERT INTO t_Users (Id, FName, SName, Username, EMail, Password, " +
                        "DateTimeCreated) VALUES(@Id, @FName, @SName, @Username, @EMail, @Password, @DateTimeCreated);", conn);
                    CreateUser.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateUser.Parameters.Add(new SqlParameter("FName", _FName));
                    CreateUser.Parameters.Add(new SqlParameter("SName", _SName));
                    CreateUser.Parameters.Add(new SqlParameter("Username", _Username));
                    CreateUser.Parameters.Add(new SqlParameter("EMail", _EMail));
                    CreateUser.Parameters.Add(new SqlParameter("Password", _Password));
                    CreateUser.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));

                    CreateUser.ExecuteNonQuery();

                    if (_DOB != new DateTime())
                    {
                        SqlCommand CreateUser_DOB = new SqlCommand("UPDATE t_Users SET DOB = @DOB WHERE Id = @Id;",
                            conn);
                        CreateUser_DOB.Parameters.Add(new SqlParameter("DOB", _DOB));
                        CreateUser_DOB.Parameters.Add(new SqlParameter("Id", _Id));

                        CreateUser_DOB.ExecuteNonQuery();
                    }

                    if (_Gender != Convert.ToChar("\0"))
                    {
                        SqlCommand CreateUser_Gender = new SqlCommand("UPDATE t_Users SET Gender = @Gender WHERE Id = @Id;",
                            conn);
                        CreateUser_Gender.Parameters.Add(new SqlParameter("Gender", _Gender));
                        CreateUser_Gender.Parameters.Add(new SqlParameter("Id", _Id));

                        CreateUser_Gender.ExecuteNonQuery();
                    }

                    if (_JobTitle != null && _JobTitle != "")
                    {
                        SqlCommand CreateUser_JobTitle = new SqlCommand("UPDATE t_Users SET JobTitle = @JobTitle WHERE Id = @Id;",
                            conn);
                        CreateUser_JobTitle.Parameters.Add(new SqlParameter("JobTitle", _JobTitle));
                        CreateUser_JobTitle.Parameters.Add(new SqlParameter("Id", _Id));

                        CreateUser_JobTitle.ExecuteNonQuery();
                    }

                    if (_Phone != null && _Phone != "")
                    {
                        SqlCommand CreateUser_Phone = new SqlCommand("UPDATE t_Users SET Phone = @Phone WHERE Id = @Id;",
                            conn);
                        CreateUser_Phone.Parameters.Add(new SqlParameter("Phone", _Phone));
                        CreateUser_Phone.Parameters.Add(new SqlParameter("Id", _Id));

                        CreateUser_Phone.ExecuteNonQuery();
                    }

                    if (_PostCode != null && _PostCode != "")
                    {
                        SqlCommand CreateUser_PostCode = new SqlCommand("UPDATE t_Users SET PostCode = @PostCode WHERE Id = @Id;",
                            conn);
                        CreateUser_PostCode.Parameters.Add(new SqlParameter("PostCode", _PostCode));
                        CreateUser_PostCode.Parameters.Add(new SqlParameter("Id", _Id));

                        CreateUser_PostCode.ExecuteNonQuery();
                    }

                    if (_Address != null && _Address != "")
                    {
                        SqlCommand CreateUser_Address = new SqlCommand("UPDATE t_Users SET Address = @Address WHERE Id = @Id;",
                            conn);
                        CreateUser_Address.Parameters.Add(new SqlParameter("Address", _Address));
                        CreateUser_Address.Parameters.Add(new SqlParameter("Id", _Id));

                        CreateUser_Address.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException e)
            {
                //Nothing really needs to happen here, neither of the databases have been affected
                _ErrMsg = "Error while creating user on online database";
                AppLog.Error("CREATE USER - " + _ErrMsg + ": " + e);
                return false;
            }

            
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

                    SqlCommand CreateUser = new SqlCommand("INSERT INTO Users (Id, FName, SName, Username, EMail, Password, " +
                        "DateTimeCreated) VALUES(@Id, @FName, @SName, @Username, @EMail, @Password, @DateTimeCreated);", conn);
                    CreateUser.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateUser.Parameters.Add(new SqlParameter("FName", _FName));
                    CreateUser.Parameters.Add(new SqlParameter("SName", _SName));
                    CreateUser.Parameters.Add(new SqlParameter("Username", _Username));
                    CreateUser.Parameters.Add(new SqlParameter("EMail", _EMail));
                    CreateUser.Parameters.Add(new SqlParameter("Password", _Password));
                    CreateUser.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));

                    CreateUser.ExecuteNonQuery();

                    if (_DOB.ToString() != new DateTime().ToString())
                    {
                        AppLog.Info("BEN!!!");
                        AppLog.Info(_DOB.ToString() + " != " + new DateTime().ToString());
                        SqlCommand CreateUser_DOB = new SqlCommand("UPDATE Users SET DOB = @DOB WHERE Id = @Id;",
                            conn);
                        CreateUser_DOB.Parameters.Add(new SqlParameter("DOB", _DOB));
                        CreateUser_DOB.Parameters.Add(new SqlParameter("Id", _Id));

                        CreateUser_DOB.ExecuteNonQuery();
                    }

                    if (_Gender != Convert.ToChar("\0"))
                    {
                        SqlCommand CreateUser_Gender = new SqlCommand("UPDATE Users SET Gender = @Gender WHERE Id = @Id;",
                            conn);
                        CreateUser_Gender.Parameters.Add(new SqlParameter("Gender", _Gender));
                        CreateUser_Gender.Parameters.Add(new SqlParameter("Id", _Id));

                        CreateUser_Gender.ExecuteNonQuery();
                    }

                    if (_JobTitle != null && _JobTitle != "")
                    {
                        SqlCommand CreateUser_JobTitle = new SqlCommand("UPDATE Users SET JobTitle = @JobTitle WHERE Id = @Id;",
                            conn);
                        CreateUser_JobTitle.Parameters.Add(new SqlParameter("JobTitle", _JobTitle));
                        CreateUser_JobTitle.Parameters.Add(new SqlParameter("Id", _Id));

                        CreateUser_JobTitle.ExecuteNonQuery();
                    }

                    if (_Phone != null && _Phone != "")
                    {
                        SqlCommand CreateUser_Phone = new SqlCommand("UPDATE Users SET Phone = @Phone WHERE Id = @Id;",
                            conn);
                        CreateUser_Phone.Parameters.Add(new SqlParameter("Phone", _Phone));
                        CreateUser_Phone.Parameters.Add(new SqlParameter("Id", _Id));

                        CreateUser_Phone.ExecuteNonQuery();
                    }

                    if (_PostCode != null && _PostCode != "")
                    {
                        SqlCommand CreateUser_PostCode = new SqlCommand("UPDATE Users SET PostCode = @PostCode WHERE Id = @Id;",
                            conn);
                        CreateUser_PostCode.Parameters.Add(new SqlParameter("PostCode", _PostCode));
                        CreateUser_PostCode.Parameters.Add(new SqlParameter("Id", _Id));

                        CreateUser_PostCode.ExecuteNonQuery();
                    }

                    if (_Address != null && _Address != "")
                    {
                        SqlCommand CreateUser_Address = new SqlCommand("UPDATE Users SET Address = @Address WHERE Id = @Id;",
                            conn);
                        CreateUser_Address.Parameters.Add(new SqlParameter("Address", _Address));
                        CreateUser_Address.Parameters.Add(new SqlParameter("Id", _Id));

                        CreateUser_Address.ExecuteNonQuery();
                    }
                }
                AppLog.Info(String.Format("CREATE USER - User {0} created on online database successfully", _Username));
            }
            catch (SqlException e)
            {
                /*
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
                }*/


                if (Data.OfflineMode) //If we are offline the only copy is local. Else it doesn't matter if this fails.
                {
                    _ErrMsg = "Error while creating user on local database. Changes were not saved";
                    AppLog.Error("CREATE USER - " + _ErrMsg + ": " + e);
                    return false;
                }
                else
                {
                    _ErrMsg = "Error while creating user on local database. Changes were saved online so " +
                        "no action required. Continuing... ";
                    AppLog.Error("CREATE USER - " + _ErrMsg + ": " + e);
                }
            }

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

            /*
            //Creates a backup of the user (from online database) before edit is attempted
            AppLog.Info("UPDATE USER - Creating a backup of the current user...");
            AppLog.Break();
            AppLog.Info(@"////////// Backup \\\\\\\\\\"); //This is just to make it display in the log
            User BackupUser = new User(Id);               //a bit nicer
            if (!BackupUser.Get())
            {
                _ErrMsg = "Error while backing up user";
                AppLog.Error(_ErrMsg);
                return false;
            }
            
            AppLog.Info(@"//////////////\\\\\\\\\\\\\\");
            AppLog.Break();*/
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


                    SqlCommand UpdateUser = new SqlCommand("UPDATE t_Users SET FName = @FName, SName = @SName," +
                        "Username = @Username, EMail = @EMail, Password = @Password WHERE Id = @Id;", conn);

                    UpdateUser.Parameters.Add(new SqlParameter("FName", _FName));
                    UpdateUser.Parameters.Add(new SqlParameter("SName", _SName));
                    UpdateUser.Parameters.Add(new SqlParameter("Username", _Username));
                    UpdateUser.Parameters.Add(new SqlParameter("EMail", _EMail));
                    UpdateUser.Parameters.Add(new SqlParameter("Password", _Password));
                    UpdateUser.Parameters.Add(new SqlParameter("Id", _Id));

                    AffectedRows = UpdateUser.ExecuteNonQuery();

                    if (_DOB != new DateTime())
                    {
                        SqlCommand UpdateUser_DOB = new SqlCommand("UPDATE t_Users SET DOB = @DOB WHERE Id = @Id;",
                            conn);
                        UpdateUser_DOB.Parameters.Add(new SqlParameter("DOB", _DOB));
                        UpdateUser_DOB.Parameters.Add(new SqlParameter("Id", _Id));

                        UpdateUser_DOB.ExecuteNonQuery();
                    }

                    if (_Gender != Convert.ToChar("\0"))
                    {
                        SqlCommand UpdateUser_Gender = new SqlCommand("UPDATE t_Users SET Gender = @Gender WHERE Id = @Id;",
                            conn);
                        UpdateUser_Gender.Parameters.Add(new SqlParameter("Gender", _Gender));
                        UpdateUser_Gender.Parameters.Add(new SqlParameter("Id", _Id));

                        UpdateUser_Gender.ExecuteNonQuery();
                    }

                    if (_JobTitle != null && _JobTitle != "")
                    {
                        SqlCommand UpdateUser_JobTitle = new SqlCommand("UPDATE t_Users SET JobTitle = @JobTitle WHERE Id = @Id;",
                            conn);
                        UpdateUser_JobTitle.Parameters.Add(new SqlParameter("JobTitle", _JobTitle));
                        UpdateUser_JobTitle.Parameters.Add(new SqlParameter("Id", _Id));

                        UpdateUser_JobTitle.ExecuteNonQuery();
                    }

                    if (_Phone != null && _Phone != "")
                    {
                        SqlCommand UpdateUser_Phone = new SqlCommand("UPDATE t_Users SET Phone = @Phone WHERE Id = @Id;",
                            conn);
                        UpdateUser_Phone.Parameters.Add(new SqlParameter("Phone", _Phone));
                        UpdateUser_Phone.Parameters.Add(new SqlParameter("Id", _Id));

                        UpdateUser_Phone.ExecuteNonQuery();
                    }

                    if (_PostCode != null && _PostCode != "")
                    {
                        SqlCommand UpdateUser_PostCode = new SqlCommand("UPDATE t_Users SET PostCode = @PostCode WHERE Id = @Id;",
                            conn);
                        UpdateUser_PostCode.Parameters.Add(new SqlParameter("PostCode", _PostCode));
                        UpdateUser_PostCode.Parameters.Add(new SqlParameter("Id", _Id));

                        UpdateUser_PostCode.ExecuteNonQuery();
                    }

                    if (_Address != null && _Address != "")
                    {
                        SqlCommand UpdateUser_Address = new SqlCommand("UPDATE t_Users SET Address = @Address WHERE Id = @Id;",
                            conn);
                        UpdateUser_Address.Parameters.Add(new SqlParameter("Address", _Address));
                        UpdateUser_Address.Parameters.Add(new SqlParameter("Id", _Id));

                        UpdateUser_Address.ExecuteNonQuery();
                    }                    
                }

                AppLog.Info(String.Format("UPDATE USER - User {0} updated on online database successfully. " +
                "{1} row(s) affected", _Username, AffectedRows));
            }
            catch (SqlException e)
            {
                //Nothing really needs to happen here because neither databases have been affected
                _ErrMsg = "Error while updating user on online database";
                AppLog.Error("UPDATE USER - " + _ErrMsg + ": " + e);
                return false;
            }
            
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
                        "Username = @Username, EMail = @EMail, Password = @Password WHERE Id = @Id;", conn);

                    UpdateUser.Parameters.Add(new SqlParameter("FName", _FName));
                    UpdateUser.Parameters.Add(new SqlParameter("SName", _SName));
                    UpdateUser.Parameters.Add(new SqlParameter("Username", _Username));
                    UpdateUser.Parameters.Add(new SqlParameter("EMail", _EMail));
                    UpdateUser.Parameters.Add(new SqlParameter("Password", _Password));
                    UpdateUser.Parameters.Add(new SqlParameter("Id", _Id));

                    AffectedRows = UpdateUser.ExecuteNonQuery();

                    if (_DOB != new DateTime())
                    {
                        SqlCommand UpdateUser_DOB = new SqlCommand("UPDATE Users SET DOB = @DOB WHERE Id = @Id;",
                            conn);
                        UpdateUser_DOB.Parameters.Add(new SqlParameter("DOB", _DOB));
                        UpdateUser_DOB.Parameters.Add(new SqlParameter("Id", _Id));

                        UpdateUser_DOB.ExecuteNonQuery();
                    }

                    if (_Gender != Convert.ToChar("\0"))
                    {
                        SqlCommand UpdateUser_Gender = new SqlCommand("UPDATE Users SET Gender = @Gender WHERE Id = @Id;",
                            conn);
                        UpdateUser_Gender.Parameters.Add(new SqlParameter("Gender", _Gender));
                        UpdateUser_Gender.Parameters.Add(new SqlParameter("Id", _Id));

                        UpdateUser_Gender.ExecuteNonQuery();
                    }

                    if (_JobTitle != null && _JobTitle != "")
                    {
                        SqlCommand UpdateUser_JobTitle = new SqlCommand("UPDATE Users SET JobTitle = @JobTitle WHERE Id = @Id;",
                            conn);
                        UpdateUser_JobTitle.Parameters.Add(new SqlParameter("JobTitle", _JobTitle));
                        UpdateUser_JobTitle.Parameters.Add(new SqlParameter("Id", _Id));

                        UpdateUser_JobTitle.ExecuteNonQuery();
                    }

                    if (_Phone != null && _Phone != "")
                    {
                        SqlCommand UpdateUser_Phone = new SqlCommand("UPDATE Users SET Phone = @Phone WHERE Id = @Id;",
                            conn);
                        UpdateUser_Phone.Parameters.Add(new SqlParameter("Phone", _Phone));
                        UpdateUser_Phone.Parameters.Add(new SqlParameter("Id", _Id));

                        UpdateUser_Phone.ExecuteNonQuery();
                    }

                    if (_PostCode != null && _PostCode != "")
                    {
                        SqlCommand UpdateUser_PostCode = new SqlCommand("UPDATE Users SET PostCode = @PostCode WHERE Id = @Id;",
                            conn);
                        UpdateUser_PostCode.Parameters.Add(new SqlParameter("PostCode", _PostCode));
                        UpdateUser_PostCode.Parameters.Add(new SqlParameter("Id", _Id));

                        UpdateUser_PostCode.ExecuteNonQuery();
                    }

                    if (_Address != null && _Address != "")
                    {
                        SqlCommand UpdateUser_Address = new SqlCommand("UPDATE Users SET Address = @Address WHERE Id = @Id;",
                            conn);
                        UpdateUser_Address.Parameters.Add(new SqlParameter("Address", _Address));
                        UpdateUser_Address.Parameters.Add(new SqlParameter("Id", _Id));

                        UpdateUser_Address.ExecuteNonQuery();
                    }
                }
                AppLog.Info(String.Format("UPDATE USER - User {0} updated on local database successfully. " +
                "{1} row(s) affected", _Username, AffectedRows));
            }
            catch (SqlException e)
            {
                if (Data.OfflineMode)
                {
                    _ErrMsg = "Error while updating user on local database. Changes were not saved.";
                    AppLog.Error("UPDATE USER - " + _ErrMsg + ": " + e);
                    return false;
                }
                else
                {
                    _ErrMsg = "Error while updating user on local database. Changes were saved online so " +
                        "no action required. Continuing...";
                    AppLog.Error("UPDATE USER - " + _ErrMsg + ": " + e);
                }
            }
            return true;
        }

        public bool Get()
        {
            //Print identifiers to log
            AppLog.Info("GET USER - Starting...");
            AppLog.Info("GET USER - User Id: " + _Id);

            int DownloadedOrganisations = 0;

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
                                _Username = reader[5].ToString().Trim();
                                _EMail = reader[7].ToString().Trim();
                                _Password = reader[8].ToString().Trim();
                                _DateTimeCreated = Convert.ToDateTime(reader[9]);
                                
                                //Check if these are null
                                if (!reader.IsDBNull(3))
                                {
                                    _DOB = Convert.ToDateTime(reader[3]);
                                }

                                if (!reader.IsDBNull(4))
                                {
                                    _Gender = Convert.ToChar(reader[4]);
                                }
                                
                                if (!reader.IsDBNull(6))
                                {
                                    _JobTitle = reader[6].ToString().Trim();
                                }
                            }
                            else
                            {
                                //If reader.Read() returns false, no data was returned
                                _ErrMsg = "Error while downloading user from online database. No data was returned";
                                AppLog.Error("GET USER - " + _ErrMsg);
                                return false;
                            }
                        }

                        AppLog.Info("GET USER - Attempting to retrieve user's organisations from local database...");
                        SqlCommand DownloadUser_Organisations = new SqlCommand("SELECT OrgId FROM OrgMembers WHERE " +
                            "UserId = @UserId", conn);

                        DownloadUser_Organisations.Parameters.Add(new SqlParameter("UserId", _Id));

                        //DownloadUser_Organisations.ExecuteNonQuery();

                        using (SqlDataReader reader = DownloadUser_Organisations.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Organisation TempOrg = new Organisation(new Guid(reader[0].ToString()));
                                if (TempOrg.Get())
                                {
                                    _Organisations.Add(TempOrg);
                                }
                                else
                                {
                                    _ErrMsg = "Failed to download organisations for user";
                                    AppLog.Error("GET USER - Get failed while downloading organisations: " + TempOrg.ErrMsg);

                                    return false;
                                }

                            }
                        }
                        AppLog.Info(String.Format("GET USER - {0} organisations were retrieved for bug from local database " +
                            "successfully", DownloadedOrganisations));
                    }
                    AppLog.Info(String.Format("GET USER - Got {0} from local database successfully", _Username));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while downloading user from local database";
                    AppLog.Error("GET USER" + _ErrMsg + ": " + e);
                    return false;
                }
                

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

                        SqlCommand DownloadUser = new SqlCommand("SELECT * FROM t_Users WHERE Id = @Id;", conn);

                        DownloadUser.Parameters.Add(new SqlParameter("Id", _Id));

                        using (SqlDataReader reader = DownloadUser.ExecuteReader())
                        {
                            if(reader.Read())
                            {
                                _FName = reader[1].ToString().Trim();
                                _SName = reader[2].ToString().Trim();
                                _EMail = reader[5].ToString().Trim();
                                _Username = reader[7].ToString().Trim();
                                _Password = reader[8].ToString().Trim();
                                _DateTimeCreated = Convert.ToDateTime(reader[12]);

                                //Check if these are null
                                if (!reader.IsDBNull(3))
                                {
                                    _DOB = Convert.ToDateTime(reader[3]);
                                }

                                if (!reader.IsDBNull(4))
                                {
                                    _Gender = Convert.ToChar(reader[4]);
                                }
                                                                
                                if (!reader.IsDBNull(9))
                                {
                                    _JobTitle = reader[9].ToString().Trim();
                                }
                                
                            }
                            else
                            {
                                //If reader.Read() returns false, no data was returned.
                                _ErrMsg = "Error while downloading user from online database. No data was returned";
                                AppLog.Error("GET USER" + _ErrMsg);
                                return false;
                            }
                        }

                        

                        AppLog.Info("GET USER - Attempting to retrieve user's organisations from online database...");
                        SqlCommand DownloadUser_Organisations = new SqlCommand("SELECT OrgId FROM t_OrgMembers WHERE " +
                            "UserId = @UserId", conn);

                        DownloadUser_Organisations.Parameters.Add(new SqlParameter("UserId", _Id));

                        //DownloadUser_Organisations.ExecuteNonQuery();

                        using(SqlDataReader reader = DownloadUser_Organisations.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Organisation TempOrg = new Organisation(new Guid(reader[0].ToString()));
                                if (TempOrg.Get())
                                {
                                    this._Organisations.Add(TempOrg);
                                }
                                else
                                {
                                    _ErrMsg = "Failed to download organisations for user";
                                    AppLog.Error("GET USER - Get failed while downloading organisations: " + TempOrg.ErrMsg);
                                     
                                    return false;
                                }
                                
                            }
                        }
                        AppLog.Info(String.Format("GET USER - {0} organisations were retrieved for bug from online database " +
                            "successfully", DownloadedOrganisations));
                    }
                    

                    AppLog.Info(String.Format("GET USER - Got {0} from online database successfully", _Username));
                }

                
                catch (SqlException e)
                {
                    _ErrMsg = "Error while downloading user from online database";
                    AppLog.Error("GET USER - " + _ErrMsg + ": " + e);
                    return false;
                }
                
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

                    SqlCommand DeleteUser = new SqlCommand("DELETE FROM t_Users WHERE Id = @Id;", conn);
                    DeleteUser.Parameters.Add(new SqlParameter("Id", _Id));

                    AffectedRows = DeleteUser.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("DELETE USER - User {0} deleted from online database successfully. " +
                "{1} row(s) affected", _Username, AffectedRows));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting user on online database";
                AppLog.Error("DELETE USER" + _ErrMsg + ": " + e);
                return false;
            }
            

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
                AppLog.Info(String.Format("DELETE USER - User {0} deleted from local database successfully. " +
                "{1} row(s) affected", _Username, AffectedRows));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting user from local database";
                AppLog.Error("DELETE USER - " + _ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info(String.Format("DELETE USER - Success!"));

            return true;
        }

        private bool Validate()
        {
            AppLog.Info("VALIDATE USER - Starting...");

            try
            {

                if (FName == null || FName == "") { _ErrMsg = "User has not been given a forename"; throw new Exception(); }
                if (FName.Length > 50) { _ErrMsg = "Forename exceeds 50 characters"; throw new Exception(); }
                if (SName == null || SName == "") { _ErrMsg = "User has not been given a surname"; throw new Exception(); }
                if (SName.Length > 50) { _ErrMsg = "Surname exceeds 50 characters"; throw new Exception(); }

                if (Gender != Convert.ToChar("\0"))
                {
                    if (Gender.ToString() != "M" && Gender.ToString() != "F" && Gender.ToString() != "O")
                    { _ErrMsg = Gender.ToString() + " is not a valid gender"; throw new Exception(); }
                }

                if (Username == null || Username == "") { _ErrMsg = "User has not been given a username"; throw new Exception(); }
                if (Username.Length > 20) { _ErrMsg = "Username exceeds 50 characters"; throw new Exception(); }
                if (JobTitle != null)
                {
                    if (JobTitle.Length > 50) { _ErrMsg = "Job title exceeds 50 characters"; throw new Exception(); }
                }
                if (EMail == null || EMail == "") { _ErrMsg = "User has not been given a email address"; throw new Exception(); }
                if (EMail.Length > 254) { _ErrMsg = "Email address exceeds 254 characters"; throw new Exception(); }
                if (Password == null || Password == "") { _ErrMsg = "User has not been given a password"; throw new Exception(); }
                if (Password.Length < 6) { _ErrMsg = "Password is too short. Must be at least 6 characters"; throw new Exception(); }
                if (Password.Length > 50) { _ErrMsg = "Password exceeds 50 characters"; throw new Exception(); }
            }
            catch
            {
                AppLog.Error("VALIDATE USER - Validation failed: " + _ErrMsg);
                return false;
            }       


            AppLog.Info("VALIDATE USER - Success!");
            return true;
        }

        public bool LogIn()
        {
            //Print identifiers to log
            AppLog.Info("LOGIN USER - Starting...");
            AppLog.Info("LOGIN USER - Username: " + _Username);
            AppLog.Info("LOGIN USER - Password: " + _Password);

            //If offline mode is on, cached user can be retrieved from the local database
            if (Data.OfflineMode)
            {
                AppLog.Info("LOGIN USER - Offline mode is ON. Attempting to get user from local database...");

                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                    {
                        AppLog.Info("LOGIN USER - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("LOGIN USER - Connection to local database opened successfully");


                        SqlCommand GetUser = new SqlCommand("SELECT Id FROM Users WHERE Username = @Username " +
                            "AND Password = @Password;", conn);

                        GetUser.Parameters.Add(new SqlParameter("Username", _Username));
                        GetUser.Parameters.Add(new SqlParameter("Password", _Password));

                        GetUser.ExecuteNonQuery();

                        using (SqlDataReader reader = GetUser.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _Id = new Guid(reader[0].ToString());
                                Get();
                            }
                            else
                            {
                                //If reader.Read() returns false, no data was returned
                                _ErrMsg = "Error while downloading user from online database. No data was returned";
                                AppLog.Error("LOGIN USER - " + _ErrMsg);
                                return false;
                            }
                        }
                    }
                    AppLog.Info(String.Format("LOGIN USER - Got {0} from local database successfully", _Username));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while downloading user from local database";
                    AppLog.Error("LOGIN USER" + _ErrMsg + ": " + e);
                    return false;
                }


            }
            else //If offline mode is off, latest copy of user will be downloaded from the online database
            {
                AppLog.Info("LOGIN USER - Offline mode is OFF. Attempting to download user from online database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("LOGIN USER - Attempting to open connection to online database...");

                        conn.Open();
                        AppLog.Info("LOGIN USER - Connection to online database opened successfully");

                        SqlCommand DownloadUser = new SqlCommand("SELECT Id FROM t_Users WHERE Username = @Username " +
                            "AND Password = @Password;", conn);

                        DownloadUser.Parameters.Add(new SqlParameter("Username", _Username));
                        DownloadUser.Parameters.Add(new SqlParameter("Password", _Password));

                        using (SqlDataReader reader = DownloadUser.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _Id = new Guid(reader[0].ToString());
                                Get();
                            }
                            else
                            {
                                //If reader.Read() returns false, no data was returned.
                                _ErrMsg = "Error while downloading user from online database. No data was returned";
                                AppLog.Error("LOGIN USER" + _ErrMsg);
                                return false;
                            }
                        }
                    }
                    AppLog.Info(String.Format("LOGIN USER - Got {0} from online database successfully", _Username));
                }


                catch (SqlException e)
                {
                    _ErrMsg = "Error while downloading user from online database";
                    AppLog.Error("LOGIN USER - " + _ErrMsg + ": " + e);
                    return false;
                }

            }

            Data.ActiveUser = this;

            AppLog.Info(String.Format("LOGIN USER - Success!"));

            return true;
        }
    }
}
