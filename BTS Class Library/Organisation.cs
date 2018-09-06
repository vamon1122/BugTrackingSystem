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
        private string _ErrMsg;
        private static string CodeFileName = "Organisation.cs";

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

        public List<OrgMember> Members { get {


                string MethodName = "public List<OrgMember> Members get;";
                string InfoPrefix = String.Format("[{0} {1}]", CodeFileName, MethodName);

                List<OrgMember> TempOrgMembers = new List<OrgMember>();

                AppLog.Debug(String.Format("{1} BEN!!! There are {0} records in Data.OrgMembers", Data.OrgMembers.Count, InfoPrefix));

                int i = 0;
                foreach (OrgMember TempOrgMember in Data.OrgMembers)
                {
                    AppLog.Debug(String.Format("{2} Data.OrgMembers[{0}].FullName = {1}", i, TempOrgMember.MyUser.FullName, InfoPrefix));
                    if (TempOrgMember.OrgId.ToString() == Id.ToString())
                    {
                        AppLog.Debug(InfoPrefix + " BEN!!! TempOrgMember.OrgId.ToString() == Id.ToString()");
                        AppLog.Debug(String.Format("{2} BEN!!! {0} == {1}", TempOrgMember.OrgId.ToString(), Id.ToString(), InfoPrefix));
                        AppLog.Debug(String.Format("{2} BEN!!! {0} IS a member of {1}", TempOrgMember.MyUser.FullName, TempOrgMember.OrgId, InfoPrefix));
                        TempOrgMembers.Add(TempOrgMember);
                    }
                    else
                    {
                        AppLog.Debug(InfoPrefix + "BEN!!! TempOrgMember.OrgId.ToString() != Id.ToString()");
                        AppLog.Debug(String.Format("{2} BEN!!! {0} != {1}", TempOrgMember.OrgId.ToString(), Id.ToString(), InfoPrefix));
                        AppLog.Debug(String.Format("{2} BEN!!! {0} IS not a member of {1}", TempOrgMember.MyUser.FullName, TempOrgMember.OrgId, InfoPrefix));
                    }
                    i++;
                }

                return TempOrgMembers;
            }
        }
        //public int NoOfMembers { get { return _Members.Count; } }
        public List<Product> Products { get {

                List<Product> TempProducts = new List<Product>();

                foreach (Product TempProduct in Data.Products)
                {
                    if (TempProduct.MyOrg.Id.ToString() == Id.ToString())
                    {
                        TempProducts.Add(TempProduct);
                    }
                }

                return TempProducts;
            }
        }

        public List<TagType> TagTypes { get {
                List<TagType> TempTagTypes = new List<TagType>();

                foreach (TagType TempTagType in Data.TagTypes)
                {
                    if (TempTagType.MyOrg.Id.ToString() == Id.ToString())
                    {
                        TempTagTypes.Add(TempTagType);
                    }
                }

                return TempTagTypes;
            }
        }

        public Organisation(Guid pId)
        {
            _Id = pId;
        }

        public Organisation()
        {
            _Id = Guid.NewGuid();
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

                    //This is a check to see weather the org already exists on the database. Obviously
                    //if it's already there, it doesn't need creating again, but this might be called
                    //if for example the org did not exist on the local database, so the Create() function
                    //needed to be able to account for that.
                    AppLog.Info("CREATE ORG - Checking that org doesn't already exist on online database");
                    bool OnlineOrgExists;

                    SqlCommand CheckOnlineDb = new SqlCommand("SELECT * FROM t_Organisations WHERE Id = @Id", conn);
                    CheckOnlineDb.Parameters.Add(new SqlParameter("Id", Id));
                    using (SqlDataReader reader = CheckOnlineDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            OnlineOrgExists = true;
                            AppLog.Info("CREATE ORG - Org already exists in online database!");
                        }
                        else
                        {
                            OnlineOrgExists = false;
                            AppLog.Info("CREATE ORG - Org does not exist in online database. Creating org on online database");
                        }
                    }
                    if(!OnlineOrgExists)
                    {
                        SqlCommand CreateOrg = new SqlCommand("INSERT INTO t_Organisations VALUES(@Id, @Name," +
                        "@DateTimeCreated);", conn);
                        CreateOrg.Parameters.Add(new SqlParameter("Id", _Id));
                        CreateOrg.Parameters.Add(new SqlParameter("Name", _Name));
                        CreateOrg.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));

                        AffectedRows = CreateOrg.ExecuteNonQuery();

                        AppLog.Info(String.Format("CREATE ORG - Organisation {0} created on online database successfully. " +
                    "{1} row(s) affected", _Name, AffectedRows));
                    }
                }
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

                    //This is a check to see weather the organisation already exists on the database. Obviously
                    //if it's already there, it doesn't need creating again, but this might be called
                    //if for example the organisation did not exist on the local database, so the Create() function
                    //needed to be able to account for that.
                    AppLog.Info("CREATE ORG - Checking that organisation doesn't already exist on local database");
                    bool LocalOrgExists;

                    SqlCommand CheckLocalDb = new SqlCommand("SELECT * FROM Organisations WHERE Id = @Id", conn);
                    CheckLocalDb.Parameters.Add(new SqlParameter("Id", Id));
                    using (SqlDataReader reader = CheckLocalDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            LocalOrgExists = true;
                            AppLog.Info("CREATE ORG - Organisation already exists in local database!");
                        }
                        else
                        {
                            LocalOrgExists = false;
                            AppLog.Info("CREATE ORG - Organisation does not exist in local database. Creating organisation on local database");
                        }
                    }
                    if (!LocalOrgExists)
                    {
                        SqlCommand CreateOrg = new SqlCommand("INSERT INTO Organisations VALUES(@Id, @Name, " +
                        "@DateTimeCreated);",
                        conn);
                        CreateOrg.Parameters.Add(new SqlParameter("Id", _Id));
                        CreateOrg.Parameters.Add(new SqlParameter("Name", _Name));
                        CreateOrg.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));

                        AffectedRows = CreateOrg.ExecuteNonQuery();

                        AppLog.Info(String.Format("CREATE ORG - Organisation {0} created on local database successfully. " +
                            "{1} row(s) affected", _Name, AffectedRows));
                    }
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while creating organisation on local database. Changes were saved online so " +
                    "no action required. Continuing... ";
                AppLog.Error("CREATE ORG - " + _ErrMsg + ": " + e);
            }

            AppLog.Debug("CREATE ORG - Attempting to add organisation to DATA...");

            if (Data.Organisations.GroupBy(x => x.Id).ToString() == this.Id.ToString())
            {
                AppLog.Debug("BEN!!! The organisation already exists!!!");
            }
            else
            {
                AppLog.Debug("BEN!!! The organisation does not exist!!!");
            }

            Data.Organisations.Add(this);
            AppLog.Debug("CREATE ORG - ...Success! Added organisation to DATA");

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
                        AppLog.Info(String.Format("GET ORG - Organisation {0} downloaded from local database successfully",
                       _Name));
                    }
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
                        AppLog.Info("GET ORG - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("GET ORG - Connection to online database opened successfully");

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

                        AppLog.Info(String.Format("GET ORG - Organisation {0} downloaded from online database successfully", _Name));

                        
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while getting organisation from online database";
                    AppLog.Error("GET ORG - " + _ErrMsg + ": " + e);
                    return false;
                }
                //Finally, check if organisation exists in the local database. If not, ADD THEM!!! If so, UPDATE THEM!!!
                AppLog.Info("GET ORGANISATION - Checking whether organisation exists in local database");

                bool ExistsOnLocalDb;

                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("GET ORGANISATION - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("GET ORGANISATION - Connection to local database opened successfully");



                    SqlCommand CheckLocalDb = new SqlCommand("SELECT * FROM Organisations WHERE Id = @Id;", conn);
                    CheckLocalDb.Parameters.Add(new SqlParameter("Id", Id));

                    using (SqlDataReader reader = CheckLocalDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ExistsOnLocalDb = true;
                            AppLog.Info("GET ORGANISATION - Organisation already exists in the local database!");
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
                        AppLog.Info("GET ORGANISATION - Updated organisation on local db successfully");
                    }
                    else
                    {
                        AppLog.Info("GET ORGANISATION - Failed to update organisation: " + _ErrMsg);
                        return false;
                    }
                }
                else
                {
                    if (Create())
                    {
                        AppLog.Info("GET ORGANISATION - Created organisation on local db successfully");
                    }
                    else
                    {
                        AppLog.Info("GET ORGANISATION - Failed to create organisation: " + _ErrMsg);
                        return false;
                    }
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

                    AppLog.Info("DELETE ORG - Attempting to delete org members from local database...");
                    SqlCommand DeleteOrgMembers = new SqlCommand("DELETE FROM OrgMembers WHERE OrgId = @OrgId;", 
                        conn);
                    DeleteOrgMembers.Parameters.Add(new SqlParameter("OrgId", _Id));
                    DeleteOrgMembers.ExecuteNonQuery();
                    AppLog.Info("DELETE ORG - Deleted org members from local database successfully");

                    AppLog.Info("DELETE ORG - Attempting to delete organisation from local database...");
                    SqlCommand DeleteOrg = new SqlCommand("DELETE FROM Organisations WHERE Id = @Id;", conn);
                    DeleteOrg.Parameters.Add(new SqlParameter("Id", _Id));
                    AppLog.Info("DELETE ORG - Deleted organisation from local database successfully");

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

                    AppLog.Info("DELETE ORG - Attempting to delete org members from online database...");
                    SqlCommand DeleteOrgMembers = new SqlCommand("DELETE FROM t_OrgMembers WHERE OrgId = @OrgId;",
                        conn);
                    DeleteOrgMembers.Parameters.Add(new SqlParameter("OrgId", _Id));
                    DeleteOrgMembers.ExecuteNonQuery();
                    AppLog.Info("DELETE ORG - Deleted org members from online database successfully");

                    AppLog.Info("DELETE ORG - Attempting to delete organisation from online database...");
                    SqlCommand DeleteOrg = new SqlCommand("DELETE FROM t_Organisations WHERE Id = @Id;", conn);
                    DeleteOrg.Parameters.Add(new SqlParameter("Id", _Id));

                    AffectedRows = DeleteOrg.ExecuteNonQuery();
                    AppLog.Info("DELETE ORG - Deleted organisation from online database successfully");
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
