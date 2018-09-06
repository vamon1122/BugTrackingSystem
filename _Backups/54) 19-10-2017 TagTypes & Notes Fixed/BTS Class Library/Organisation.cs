using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BenLog;

namespace BTS_Class_Library
{
    public partial class Organisation
    {
        private Guid _Id;
        private string _Name;
        private DateTime _DateTimeCreated;
        private List<OrgMember> _Members;
        private string _ErrMsg;

        public Guid Id { get { return _Id; } }
        public string Name {
            get {
                return _Name;
            }
            set {
                if(value.Length < 51)
                {
                    _Name = value;
                }
                else
                {
                    throw new Exception("Organisation name exceeds 50 characters");
                }
            }
        }
        public DateTime DateTimeCreated { get { return _DateTimeCreated; } }
        public string ErrMsg { get { return _ErrMsg; } }
        public List<OrgMember> Members { get { return _Members; } }
        public int NoOfMembers { get { return _Members.Count; } }

        public Organisation(Guid pId)
        {
            _Id = pId;
            //if (!Get()) { Data.UserFriendlyError(_ErrMsg); };
        }

        public Organisation()
        {
            _Id = Guid.NewGuid();
            //_DateTimeCreated = DateTime.Now;
        }

        public bool Create()
        {
            //Print identifiers in log
            AppLog.Info("CREATE ORG - Starting...");
            AppLog.Info("CREATE ORG - Organisation's Id: " + Id);
            AppLog.Info("CREATE ORG - Organisation's Name: " + Name);

            int AffectedRows = 0;

            //Checks offline mode (can't create organisations in offline mode)
            if (Data.OfflineMode)
            {
                _ErrMsg = "Cannot create organisations while in offline mode";
                AppLog.Info(String.Format("CREATE ORG - Organisation {0} was not created because offline mode is on", 
                    _Name));
                return false;
            }

            //Checks that data is valid before attempting upload
            AppLog.Info("CREATE ORG - Validating...");
            if (!Validate())
            {
                AppLog.Info("CREATE ORG - Organisation failed validation");
                return false;
            }
            AppLog.Info("CREATE ORG - Organisation validated successfully");

            _DateTimeCreated = DateTime.Now;

            //Try creating user online first
            AppLog.Info("CREATE ORG - Attempting to create organisation on online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("CREATE ORG - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("CREATE ORG - Connection to online database opened successfully");

                    SqlCommand CreateOrg = new SqlCommand("INSERT INTO t_Organisations VALUES(@Id, @Name," +
                        "@DateTimeCreated);", conn);
                    CreateOrg.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateOrg.Parameters.Add(new SqlParameter("Name", _Name));
                    CreateOrg.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));

                    AffectedRows = CreateOrg.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("CREATE ORG - Organisation {0} created on online database successfully. " +
                "{1} row(s) affected", _Name, AffectedRows));
            }
            catch (SqlException e)
            {
                //Nothing really needs to happen here, neither of the databases have been affected
                _ErrMsg = "Error while creating organisation on online database";
                AppLog.Error("GET ORG - " + _ErrMsg + ": " + e);
                return false;
            }

            AffectedRows = 0;

            //If the organisation was created on the online database without throwing any errors,
            //try and create it on the local database
            AppLog.Info("CREATE ORG - Attempting to create organisation on local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("CREATE ORG - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("CREATE ORG - Connection to local database opened successfully");

                    SqlCommand CreateOrg = new SqlCommand("INSERT INTO Organisations VALUES(@Id, @Name, " +
                        "@DateTimeCreated);",
                        conn);
                    CreateOrg.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateOrg.Parameters.Add(new SqlParameter("Name", _Name));
                    CreateOrg.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));

                    AffectedRows = CreateOrg.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("CREATE ORG - Organisation {0} created on local database successfully. " +
                "{1} row(s) affected", _Name, AffectedRows));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while creating organisation on local database. Changes were saved online so " +
                    "no action required. Continuing... ";
                AppLog.Error("CREATE ORG - " + _ErrMsg + ": " + e);
            }
            AppLog.Info(String.Format("CREATE ORG - Success!"));
            return true;
        }

        public bool Update()
        {
            //Print identifiers to log
            AppLog.Info("UPDATE ORG - Starting...");
            AppLog.Info("UPDATE ORG - Organisation's Id: " + Id);
            AppLog.Info("UPDATE ORG - Organisation's Name: " + Name);

            int AffectedRows = 0;

            //Orgs can't be updated in offline mode
            if (Data.OfflineMode)
            {
                _ErrMsg = "Cannot update organisations while in offline mode";
                AppLog.Info(String.Format("UPDATE ORG - Organisation {0} was not updated because offline mode is on", _Name));
                return false;
            }

            /*
            //If offline mode is off, back up organisation

            //Creates a backup of the organisation (from online database) before edit is attempted
            AppLog.Info("UPDATE ORG - Creating a backup of the current organisation...");
            AppLog.Break();
            AppLog.Info(@"////////// Backup \\\\\\\\\\"); //This is just to make it display in the log
            Organisation BackupOrg = new Organisation(Id);               //a bit nicer

            if (!BackupOrg.Get())
            {
                _ErrMsg = "Error while backing up organisation";
                AppLog.Error(_ErrMsg);
                return false;
            }

            AppLog.Info(@"//////////////\\\\\\\\\\\\\\");
            AppLog.Break();
            AppLog.Info("UPDATE ORG - Organisation backed up successfully!");*/

            //try update online database
            AppLog.Info("UPDATE ORG - Attempting to update organisation on online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("UPDATE ORG - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("UPDATE ORG - Connection to local database opened successfully");

                    SqlCommand UpdateOrg = new SqlCommand("UPDATE t_Organisations SET Name = @Name WHERE Id = @Id;",conn);
                    UpdateOrg.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateOrg.Parameters.Add(new SqlParameter("Name", _Name));

                    AffectedRows = UpdateOrg.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("UPDATE ORG - Organisation {0} updated on online database successfully. " +
                "{1} row(s) affected", _Name, AffectedRows));
            }
            catch (SqlException e)
            {
                //Database has not been affected, no handling required
                _ErrMsg = "Error while updating organisation on online database";
                AppLog.Error("UPDATE ORG - " + _ErrMsg + ": " + e);
                return false;
            }
            

            AffectedRows = 0;

            //If online database was updated successfully, try updating local database
            try
            {
                AppLog.Info("UPDATE ORG - Attempting to update organisation on local database...");
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("UPDATE ORG - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("UPDATE ORG - Connection to local database opened successfully");

                    SqlCommand UpdateOrg = new SqlCommand("UPDATE Organisations SET Name = @Name WHERE Id = @Id;", conn);
                    UpdateOrg.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateOrg.Parameters.Add(new SqlParameter("Name", _Name));

                    AffectedRows = UpdateOrg.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("UPDATE ORG - Organisation {0} updated on local database successfully. " +
                "{1} row(s) affected", _Name, AffectedRows));
            }
            catch (SqlException e)
            {
                /*
                //Need to revert changes which were made on the online database
                AppLog.Break();
                AppLog.Info(@"////////// Restore \\\\\\\\\\");

                AppLog.Info("UPDATE ORG - Attempting to delete organisation...");

                if (!Delete())
                {
                    _ErrMsg = "UPDATE ORG - Error while trying to delete organisation";
                    AppLog.Info(_ErrMsg);
                    return false;
                }

                AppLog.Info("UPDATE ORG - Organisation deleted successfully!");

                AppLog.Info("UPDATE ORG - Restoring values from backup...");

                this._Id = BackupOrg._Id;
                this._Name = BackupOrg.Name;
                this._DateTimeCreated = BackupOrg.DateTimeCreated;
                this._Members = BackupOrg.Members;

                AppLog.Info("UPDATE ORG - Values restored successfully!");
                AppLog.Info("UPDATE ORG - Attempting upload...");
                if (!Delete())
                {
                    _ErrMsg = "UPDATE ORG - Error while trying to upload organisation";
                    AppLog.Info(_ErrMsg);
                    return false;
                }
                AppLog.Info("UPDATE ORG - Organisation uploaded successfully!");

                AppLog.Info(@"///////////////\\\\\\\\\\\\\\\");
                _ErrMsg = "Error while updating organisation on local database. Changes have been reverted";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;*/
                
                _ErrMsg = "Error while updating user on local database. Changes were saved online so " +
                    "no action required. Continuing...";
                AppLog.Error("UPDATE ORG - " + _ErrMsg + ": " + e);
            }
            AppLog.Info(String.Format("UPDATE ORG - Success!"));
            return true;
        }

        public bool Get()
        {
            AppLog.Info("GET ORG - Starting...");
            AppLog.Info("GET ORG - Organisation's Id: " + Id);

            //Get from local
            if (Data.OfflineMode)
            {
                AppLog.Info("GET ORG - Offline mode is ON. Attempting to download organisation from local database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                    {
                        AppLog.Info("GET ORG - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("GET ORG - Connection to local database opened successfully");
                        
                        SqlCommand GetOrganisation = new SqlCommand("SELECT * FROM Organisations WHERE Id = @Id;", conn);
                        GetOrganisation.Parameters.Add(new SqlParameter("Id", _Id));

                        using (SqlDataReader reader = GetOrganisation.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _Name = reader[1].ToString().Trim();
                                _DateTimeCreated = Convert.ToDateTime(reader[2]);
                            }
                            else
                            {
                                //If reader.Read() returns false, no data was returned
                                _ErrMsg = "Error while downloading organisation from local database. No data" +
                                    " was returned";
                                AppLog.Error("GET ORG - " + _ErrMsg);
                                return false;
                            }
                        }
                    }
                    AppLog.Info(String.Format("GET ORG - Organisation {0} downloaded from local database successfully",
                        _Name));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while getting organisation from local database";
                    AppLog.Error("GET ORG - " + _ErrMsg + ": " + e);
                    return false;
                }
                
            }
            else//Get from online
            {
                AppLog.Info("GET ORG - Offline mode is OFF. Attempting to get organisations from online database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("GET ORG - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("GET ORG - Connection to local database opened successfully");

                        SqlCommand GetOrganisation = new SqlCommand("SELECT * FROM t_Organisations WHERE Id = @Id;", conn);
                        GetOrganisation.Parameters.Add(new SqlParameter("Id", _Id));

                        GetOrganisation.ExecuteNonQuery();

                        using (SqlDataReader reader = GetOrganisation.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _Name = reader[1].ToString().Trim();
                                _DateTimeCreated = Convert.ToDateTime(reader[2]);
                            }
                            else
                            {
                                //If reader.Read() returns false, no data was returned
                                _ErrMsg = "Error while downloading organisation from online database. No data was returned";
                                AppLog.Error("GET ORG - " + _ErrMsg);
                                return false;
                            }
                        }
                    }
                    AppLog.Info(String.Format("GET ORG - Organisation {0} downloaded from online database successfully", _Name));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while getting organisation from online database";
                    AppLog.Error("GET ORG - " + _ErrMsg + ": " + e);
                    return false;
                }
                
            }

            AppLog.Info(String.Format("GET ORG - Success!"));

            return true;
        }

        public bool Delete()
        {
            AppLog.Info("DELETE ORG - Starting...");
            AppLog.Info("DELETE ORG - Organisation's Id: " + Id);
            AppLog.Info("DELETE ORG - Organisation's Name: " + Name);

            int AffectedRows = 0;

            if (Data.OfflineMode)
            {
                _ErrMsg = "Cannot delete organisations while in offline mode";
                AppLog.Info(String.Format("DELETE ORG - Organisation {0} was not deleted because offline mode is on", _Name));
                return false;
            }
            //If offline mode is off, first try deleting from online database
            AppLog.Info("DELETE ORG - Attempting to delete organisation from local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("DELETE ORG - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("DELETE ORG - Connection to local database opened successfully");

                    SqlCommand DeleteOrg = new SqlCommand("DELETE FROM Organisations WHERE Id = @Id;", conn);
                    DeleteOrg.Parameters.Add(new SqlParameter("Id", _Id));

                    AffectedRows = DeleteOrg.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("DELETE ORG - Organisation {0} deleted from local database successfully. " +
                "{1} row(s) affected", _Name, AffectedRows));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting organisation from local database";
                AppLog.Error("DELETE ORG - " + _ErrMsg + ": " + e);
                return false;
            }
            

            AffectedRows = 0;

            //If deleted from online database successfully, try local database
            AppLog.Info("DELETE ORG - Attempting to delete organisation from online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("DELETE ORG - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("DELETE ORG - Connection to online database opened successfully");

                    SqlCommand DeleteOrg = new SqlCommand("DELETE FROM t_Organisations WHERE Id = @Id;", conn);
                    DeleteOrg.Parameters.Add(new SqlParameter("Id", _Id));

                    AffectedRows = DeleteOrg.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("DELETE ORG - Organisation {0} deleted from online database successfully. " +
                "{1} row(s) affected", _Name, AffectedRows));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting organisation from online database";
                AppLog.Error("DELETE ORG - " + _ErrMsg + ": " + e);
                return false;
            }
            return true;
        }

        private bool Validate()
        {
            AppLog.Info("VALIDATE ORGANISATION - Starting...");
            try
            {
                if (Name == null) { _ErrMsg = "Organisation has not been given a name"; throw new Exception(); }
                if (Name.Length > 50) { _ErrMsg = "Organisation name exceeds 50 characters"; throw new Exception(); }
            }
            catch
            {
                AppLog.Error("VALIDATE ORGANISATION - Validation failed: " + _ErrMsg);
                return false;
            }
            AppLog.Info("VALIDATE USER - Success!");
            return true;
        }

        public TagType NewTagType()
        {
            return new TagType(this);
        }

        public OrgMember NewOrgMember(User pUser)
        {
            return new OrgMember(pUser, this);
        }

        public Product NewProduct()
        {
            return new Product(this);
        }
    }
}
