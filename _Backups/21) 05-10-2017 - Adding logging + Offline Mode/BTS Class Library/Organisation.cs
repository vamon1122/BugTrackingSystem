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
            if (!Get()) { Data.UserFriendlyError(_ErrMsg); };
        }

        public Organisation()
        {
            _Id = Guid.NewGuid();
            _DateTimeCreated = DateTime.Now;
        }

        public bool Create()
        {
            AppLog.Info("CREATE ORG - Starting...");

            if (Data.OfflineMode)
            {
                _ErrMsg = "Cannot create organisations while in offline mode";
                AppLog.Info(String.Format("CREATE ORG - Organisation {0} was not created because offline mode is on", _Name));
                return false;
            }

            AppLog.Info("CREATE ORG - Attempting to create organisation on online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("CREATE ORG - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("CREATE ORG - Connection to online database opened successfully");

                    SqlCommand CreateOrg = new SqlCommand("INSERT INTO t_BTS_Organisations VALUES(@Id, @Name," +
                        "@DateTimeCreated);", conn);
                    CreateOrg.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateOrg.Parameters.Add(new SqlParameter("Name", _Name));
                    CreateOrg.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));

                    CreateOrg.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while creating organisation on online database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info(String.Format("CREATE ORG - Organisation {0} created on online database successfully", _Name));

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

                    CreateOrg.ExecuteNonQuery();
                }
            }
            catch(SqlException e)
            {
                _ErrMsg = "Error while creating organisation on local database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info(String.Format("CREATE ORG - Organisation {0} created on local database successfully", _Name));
            AppLog.Info(String.Format("CREATE ORG - Success!"));
            return true;
        }

        public bool Update()
        {
            AppLog.Info("UPDATE ORG - Starting...");

            if (Data.OfflineMode)
            {
                _ErrMsg = "Cannot update organisations while in offline mode";
                AppLog.Info(String.Format("UPDATE ORG - Organisation {0} was not updated because offline mode is on", _Name));
                return false;
            }
            AppLog.Info("CREATE ORG - Attempting to update organisation on online database...");

            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("UPDATE ORG - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("UPDATE ORG - Connection to local database opened successfully");

                    SqlCommand UpdateOrg = new SqlCommand("UPDATE t_BTS_Organisations SET Name = @Name WHERE Id = @Id;",conn);
                    UpdateOrg.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateOrg.Parameters.Add(new SqlParameter("Name", _Name));

                    UpdateOrg.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while updating organisation on online database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info(String.Format("UPDATE ORG - Organisation {0} updated on online database successfully", _Name));

            try
            {
                AppLog.Info("CREATE ORG - Attempting to update organisation on local database...");
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("UPDATE ORG - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("UPDATE ORG - Connection to local database opened successfully");

                    SqlCommand UpdateOrg = new SqlCommand("UPDATE Organisations SET Name = @Name WHERE Id = @Id;",conn);
                    UpdateOrg.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateOrg.Parameters.Add(new SqlParameter("Name", _Name));

                     UpdateOrg.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while updating organisation on local database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info(String.Format("UPDATE ORG - Organisation {0} updated on local database successfully", _Name));

            
            AppLog.Info(String.Format("UPDATE ORG - Success!"));
            return true;
        }

        public bool Get()
        {
            AppLog.Info("GET ORG - Starting...");

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

                        GetOrganisation.ExecuteNonQuery();

                        using (SqlDataReader reader = GetOrganisation.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _Name = reader[1].ToString();
                                _DateTimeCreated = Convert.ToDateTime(reader[2]);
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while getting organisation from local database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
                AppLog.Info(String.Format("GET ORG - Organisation {0} downloaded from local database successfully", _Name));
            }
            else
            {
                AppLog.Info("GET ORG - Offline mode is OFF. Attempting to get organisations from online database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("GET ORG - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("GET ORG - Connection to local database opened successfully");

                        SqlCommand GetOrganisation = new SqlCommand("SELECT * FROM t_BTS_Organisations WHERE Id = @Id;", conn);
                        GetOrganisation.Parameters.Add(new SqlParameter("Id", _Id));

                        GetOrganisation.ExecuteNonQuery();

                        using (SqlDataReader reader = GetOrganisation.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _Name = reader[1].ToString();
                                _DateTimeCreated = Convert.ToDateTime(reader[2]);
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while getting organisation from online database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
                AppLog.Info(String.Format("GET ORG - Organisation {0} downloaded from online database successfully", _Name));
            }

            AppLog.Info(String.Format("GET ORG - Success!"));

            return true;
        }

        public bool Delete()
        {
            AppLog.Info("DELETE ORG - Starting...");

            if (Data.OfflineMode)
            {
                _ErrMsg = "Cannot delete organisations while in offline mode";
                AppLog.Info(String.Format("DELETE ORG - Organisation {0} was not deleted because offline mode is on", _Name));
                return false;
            }

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

                    DeleteOrg.ExecuteNonQuery();
                }

            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting organisation from local database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info(String.Format("DELETE ORG - Organisation {0} deleted from local database successfully", _Name));

            AppLog.Info("DELETE ORG - Attempting to delete organisation from online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("DELETE ORG - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("DELETE ORG - Connection to online database opened successfully");

                    SqlCommand DeleteOrg = new SqlCommand("DELETE FROM t_BTS_Organisations WHERE Id = @Id;", conn);
                    DeleteOrg.Parameters.Add(new SqlParameter("Id", _Id));

                    DeleteOrg.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting organisation from online database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info(String.Format("DELETE ORG - Organisation {0} deleted from online database successfully", _Name));

            return true;
        }
    }
}
