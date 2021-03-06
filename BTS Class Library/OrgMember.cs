﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BenLog;

namespace BTS_Class_Library
{
    public class OrgMember
    {
        private Guid _OrgId;
        private Guid _UserId;
        private DateTime _DateTimeJoined;
        private int _AccessLevel;
        private string _ErrMsg;

        public Guid OrgId { get { return _OrgId; } }
        public Guid UserId { get { return _UserId; } }

        public User MyUser { get
            {
                bool has = Data.Users.Any(user => user.Id.ToString() == _UserId.ToString());
                return Data.Users.Single(user => user.Id.ToString() == _UserId.ToString());
            }
        }

        public Organisation MyOrg
        {
            get
            {
                bool has = Data.Organisations.Any(org => org.Id.ToString() == OrgId.ToString());
                return Data.Organisations.Single(org => org.Id.ToString() == OrgId.ToString());
            }
        }


        public DateTime DateTimeJoined { get { return _DateTimeJoined; } }
        public int AccessLevel
        {
            get { return _AccessLevel; }
            set { _AccessLevel = value; }
        }
        public string ErrMsg { get { return _ErrMsg; } }


        internal OrgMember(Guid pUserId, Guid pOrgId)
        {
            _OrgId = pOrgId;
            _UserId = pUserId;
            //_DateTimeJoined = DateTime.Now;
        }

        public OrgMember(Guid pUserId, Guid pOrgId, DateTime pDateTimeJoined)
        {
            _UserId = pUserId;
            _OrgId = pOrgId;
            _DateTimeJoined = pDateTimeJoined;
        }



        public bool Create()
        {
            //Print identifiers to log
            AppLog.Info("CREATE ORGMEMBER - Starting...");
            AppLog.Info("CREATE ORGMEMBER - OrgMember's Org's Id: " + _OrgId);
            AppLog.Info("CREATE ORGMEMBER - OrgMember's User's Id: " + _UserId);
            AppLog.Info("CREATE ORGMEMBER - OrgMember's User's FullName: " + MyUser.FullName);
            AppLog.Info("CREATE ORGMEMBER - OrgMember's User's Username: " + MyUser.Username);

            //Checks offline mode (can't create users in offline mode)
            if (Data.OfflineMode)
            {
                _ErrMsg = "Cannot create organisation members while in offline mode";
                AppLog.Info(String.Format("CREATE ORGMEMBER - Organisation member {0} was not created because " +
                    "offline mode is on", MyUser.Username));
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

            _DateTimeJoined = DateTime.Now;

            AppLog.Info("CREATE ORGMEMBER - Attempting to create organisation member on online database...");
            //If offline mode is on, try and create org member on online database
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("CREATE ORGMEMBER - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("CREATE ORGMEMBER - Connection to online database opened successfully");

                    //This is a check to see weather the org member already exists on the database. Obviously
                    //if it's already there, it doesn't need creating again, but this might be called
                    //if for example the org member did not exist on the local database, so the Create() function
                    //needed to be able to account for that.
                    AppLog.Info("CREATE ORGMEMBER - Checking that org member doesn't already exist on online database");
                    bool OnlineOrgMemberExists;

                    SqlCommand CheckOnlineDb = new SqlCommand("SELECT * FROM t_OrgMembers WHERE OrgId = @OrgId AND " +
                        "UserId = @UserId;", conn);
                    CheckOnlineDb.Parameters.Add(new SqlParameter("OrgId", _OrgId));
                    CheckOnlineDb.Parameters.Add(new SqlParameter("UserId", MyUser.Id));
                    using (SqlDataReader reader = CheckOnlineDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            OnlineOrgMemberExists = true;
                            AppLog.Info("CREATE ORGMEMBER - OrgMember already exists in online database!");
                        }
                        else
                        {
                            OnlineOrgMemberExists = false;
                            AppLog.Info("CREATE ORGMEMBER - OrgMember does not exist in online database. Creating org member on online database");
                        }
                    }
                    if (!OnlineOrgMemberExists)
                    {
                        SqlCommand CreateOrgMember = new SqlCommand("INSERT INTO t_OrgMembers VALUES(@OrgId," +
                        "@UserId, @DateTimeJoined, @AccessLevel);", conn);
                        CreateOrgMember.Parameters.Add(new SqlParameter("OrgId", _OrgId));
                        CreateOrgMember.Parameters.Add(new SqlParameter("UserId", MyUser.Id));
                        CreateOrgMember.Parameters.Add(new SqlParameter("DateTimeJoined", _DateTimeJoined));
                        CreateOrgMember.Parameters.Add(new SqlParameter("AccessLevel", _AccessLevel));

                        CreateOrgMember.ExecuteNonQuery();

                        AppLog.Info(String.Format("CREATE ORGMEMBER - Organisation member {0} created on online " +
                            "database successfully", MyUser.FullName));
                    }
                }
            }
            catch (SqlException e)
            {
                //Nothing really needs to happen here, neither of the databases have been affected
                _ErrMsg = "Error while creating organisation member on online database";
                AppLog.Error("CREATE ORGMEMBER - " + _ErrMsg + ": " + e);
                return false;
            }



            AppLog.Info("CREATE ORGMEMBER - Attempting to create organisation member on local database...");
            //If org member was created on the online database successfully, try local database
            /*try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("CREATE ORGMEMBER - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("CREATE ORGMEMBER - Connection to local database opened successfully");

                    //This is a check to see weather the org member already exists on the database. Obviously
                    //if it's already there, it doesn't need creating again, but this might be called
                    //if for example the org member did not exist on the local database, so the Create() function
                    //needed to be able to account for that.
                    AppLog.Info("CREATE ORGMEMBER - Checking that org member doesn't already exist on local database");
                    bool LocalOrgMemberExists;

                    SqlCommand CheckLocalDb = new SqlCommand("SELECT * FROM OrgMembers WHERE OrgId = @OrgId AND " +
                        "UserId = @UserId;", conn);
                    CheckLocalDb.Parameters.Add(new SqlParameter("OrgId", _OrgId));
                    CheckLocalDb.Parameters.Add(new SqlParameter("UserId", _UserId));
                    using (SqlDataReader reader = CheckLocalDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            LocalOrgMemberExists = true;
                            AppLog.Info("CREATE ORGMEMBER - OrgMember already exists in local database!");
                        }
                        else
                        {
                            LocalOrgMemberExists = false;
                            AppLog.Info("CREATE ORGMEMBER - OrgMember does not exist in local database. Creating org member on local database");
                        }
                    }
                    if (LocalOrgMemberExists)
                    {
                        SqlCommand CreateOrgMember = new SqlCommand("INSERT INTO OrgMembers VALUES(@OrgId," +
                        "@UserId, @DateTimeJoined, @AccessLevel);", conn);
                        CreateOrgMember.Parameters.Add(new SqlParameter("OrgId", _OrgId));
                        CreateOrgMember.Parameters.Add(new SqlParameter("UserId", _UserId));
                        CreateOrgMember.Parameters.Add(new SqlParameter("DateTimeJoined", _DateTimeJoined));
                        CreateOrgMember.Parameters.Add(new SqlParameter("AccessLevel", _AccessLevel));

                        CreateOrgMember.ExecuteNonQuery();

                        AppLog.Info(String.Format("CREATE ORGMEMBER - Organisation member {0} created on local database " +
                        "successfully.", MyUser.FullName));
                    }
                }
            }
            catch (SqlException e)
            {
                
                //If this fails we need to reset the changes
                //_ErrMsg = "Error while creating organisation member on local database. Organisation member will " +
                //    "now be deleted.";
                //AppLog.Error(_ErrMsg + ": " + e);
                //
                //AppLog.Info("Attempting to resolve error by deleting organisation member...");
                //if (Delete())
                //{
                //    AppLog.Info("Organisation member deleted. Error resolved successfully!");
                //}
                //else
                //{
                    AppLog.Info("Organisation member not deleted. FATAL ERROR!!!");
                    throw new Exception("Fatal error while creating organisation. There was an error and " +
                        "changes could not be reverted.");
                }
                return false;

                _ErrMsg = "Error while creating organisation member on local database. Changes were saved " +
                    "online so no action required. Continuing... ";
                AppLog.Error("CREATE ORGMEMBER - " + _ErrMsg + ": " + e);
            }*/
            AppLog.Info(String.Format("CREATE ORGMEMBER - Success!"));
            return true;
        }

        public bool Update()
        {
            AppLog.Info("UPDATE ORGMEMBER - Starting...");
            AppLog.Info("UPDATE ORGMEMBER - OrgMember's Org's Id: " + _OrgId);
            AppLog.Info("UPDATE ORGMEMBER - OrgMember's User's Id: " + _UserId);
            AppLog.Info("UPDATE ORGMEMBER - OrgMember's User's FullName: " + MyUser.FullName);
            AppLog.Info("UPDATE ORGMEMBER - OrgMember's User's Username: " + MyUser.Username);

            //Checks offline mode (can't update users in offline mode)
            if (Data.OfflineMode)
            {
                _ErrMsg = "Cannot update organisation members while in offline mode";
                AppLog.Info(String.Format("UPDATE ORGMEMBER - Organisation member {0} was not updated because " +
                    "offline mode is on", MyUser.Username));
                return false;
            }

            /*
            //Creates a backup of the organisation member (from online database) before edit is attempted
            AppLog.Info("UPDATE ORGMEMBER - Creating a backup of the current organisation member...");
            AppLog.Break();
            AppLog.Info(@"////////// Backup \\\\\\\\\\"); //This is just to make it display in the log
            OrgMember BackupOrgMember = new OrgMember(_MyUser, MyOrg);               //a bit nicer
            if (!BackupOrgMember.Get())
            {
                _ErrMsg = "Error while backing up organisation member";
                AppLog.Error(_ErrMsg);
                return false;
            }

            AppLog.Info(@"//////////////\\\\\\\\\\\\\\");
            AppLog.Break();
            AppLog.Info("UPDATE ORGMEMBER - Organisation member backed up successfully!");
            */

            int AffectedRows = 0;

            AppLog.Info("UPDATE ORGMEMBER - Attempting to update organisation member on online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("UPDATE ORGMEMBER - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("UPDATE ORGMEMBER - Connection to online database opened successfully");

                    SqlCommand UpdateOrgMember = new SqlCommand("UPDATE t_OrgMembers SET AccessLevel = @AccessLevel " +
                        "WHERE OrgId = @OrgId AND UserId = @UserId;", conn);
                    UpdateOrgMember.Parameters.Add(new SqlParameter("OrgId", _OrgId));
                    UpdateOrgMember.Parameters.Add(new SqlParameter("UserId", _UserId));
                    UpdateOrgMember.Parameters.Add(new SqlParameter("AccessLevel", _AccessLevel));

                    AffectedRows = UpdateOrgMember.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("UPDATE ORGMEMBER - User {0} updated on online database " +
                "successfully. {1} row(s) affected", MyUser.FullName, AffectedRows));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while updating organisation member on online database";
                AppLog.Error("UPDATE ORGMEMBER - " + _ErrMsg + ": " + e);
                return false;
            }

            AffectedRows = 0;

            AppLog.Info("UPDATE ORGMEMBER - Attempting to update organisation member on local database...");
            //If online databaase was updated successfully, try local database
            /*try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("UPDATE ORGMEMBER - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("UPDATE ORGMEMBER - Connection to local database opened successfully");

                    SqlCommand UpdateOrgMember = new SqlCommand("UPDATE OrgMembers SET AccessLevel = @AccessLevel " +
                        "WHERE OrgId = @OrgId AND UserId = @UserId;", conn);
                    UpdateOrgMember.Parameters.Add(new SqlParameter("OrgId", _OrgId));
                    UpdateOrgMember.Parameters.Add(new SqlParameter("UserId", MyUser.Id));
                    UpdateOrgMember.Parameters.Add(new SqlParameter("AccessLevel", _AccessLevel));

                    AffectedRows = UpdateOrgMember.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("UPDATE ORGMEMBER - User {0} updated on local database " +
                "successfully. {1} row(s) affected", MyUser.FullName, AffectedRows));
            }
            catch (SqlException e)
            {
                //Need to revert changes here

                //AppLog.Break();
                //AppLog.Info(@"////////// Restore \\\\\\\\\\");
                //
                //AppLog.Info("UPDATE ORGMEMBER - Attempting to delete organisation member...");
                //
                //if (!Delete())
                //{
                //    _ErrMsg = "UPDATE ORGMEMBER - Error while trying to delete organisation member";
                //    AppLog.Info(_ErrMsg);
                //    return false;
                //}
                //AppLog.Info("UPDATE ORGMEMBER - Organisation member deleted successfully!");
                //
                //AppLog.Info("UPDATE ORGMEMBER - Restoring values from backup...");
                //this._MyOrg = BackupOrgMember.MyOrg;
                //this._MyUser = BackupOrgMember.MyUser;
                //this._DateTimeJoined = BackupOrgMember.DateTimeJoined;
                //this._AccessLevel = BackupOrgMember.AccessLevel;
                //this._ErrMsg = BackupOrgMember.ErrMsg;
                //AppLog.Info("UPDATE ORGMEMBER - Values restored successfully!");
                //AppLog.Info("UPDATE ORGMEMBER - Attempting upload...");
                //if (!Delete())
                //{
                //    _ErrMsg = "UPDATE ORGMEMBER - Error while trying to upload organisation member";
                //    AppLog.Info(_ErrMsg);
                //    return false;
                //}
                //AppLog.Info("UPDATE ORGMEMBER - Organisation member uploaded successfully!");

                //AppLog.Info(@"///////////////\\\\\\\\\\\\\\\");

                //_ErrMsg = "Error while updating organisation member on local database. Changes have been reverted.";
                //AppLog.Error(_ErrMsg + ": " + e);
                //return false;
                

                _ErrMsg = "Error while updating organisation member on local database. Changes were saved online " +
                    "so no action required. Continuing...";
                AppLog.Error("UPDATE ORGMEMBER - " + _ErrMsg + ": " + e);
            }*/
            AppLog.Info(String.Format("UPDATE ORGMEMBER - Success!"));
            return true;
        }

        public bool Get()
        {
            AppLog.Info("GET ORGMEMBER - Starting...");
            AppLog.Info("GET ORGMEMBER - OrgMember's Org's Id: " + _OrgId);
            AppLog.Info("GET ORGMEMBER - OrgMember's User's Id: " + _UserId);

            //Download from local db if offline mode is on
            if (Data.OfflineMode)
            {
                AppLog.Info("GET ORGMEMBER - Offline mode is ON. Attempting to download organisation member" +
                    " from local database...");
                /*try
                {
                    using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                    {
                        AppLog.Info("GET ORGMEMBER - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("GET ORGMEMBER - Connection to local database opened successfully");

                        SqlCommand GetOrgMember = new SqlCommand("SELECT * FROM OrgMembers " +
                            "WHERE OrgId = @OrgId AND UserId = @UserId;", conn);
                        GetOrgMember.Parameters.Add(new SqlParameter("OrgId", _OrgId));
                        GetOrgMember.Parameters.Add(new SqlParameter("UserId", _UserId));

                        using (SqlDataReader reader = GetOrgMember.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _DateTimeJoined = Convert.ToDateTime(reader[2]);
                                _AccessLevel = Convert.ToInt32(reader[3]);
                            }
                            else
                            {
                                //If reader.Read() returns false, no data was returned
                                _ErrMsg = "Error while downloading organisation member from local database. " +
                                    "No data was returned";
                                AppLog.Error("GET ORGMEMBER - " + _ErrMsg);
                                return false;
                            }
                        }
                    }
                    AppLog.Info(String.Format("GET ORGMEMBER - Got org member from local database successfully"));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while getting organisation member from local database";
                    AppLog.Error("GET ORGMEMBER - " + _ErrMsg + ": " + e);
                    return false;
                }*/

            }
            else
            {
                AppLog.Info("GET ORGMEMBER - Offline mode is OFF. Attempting to download organisation member " +
                    "from online database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("GET ORGMEMBER - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("GET ORGMEMBER - Connection to online database opened successfully");

                        SqlCommand GetOrgMember = new SqlCommand("SELECT * FROM t_OrgMembers " +
                            "WHERE OrgId = @OrgId AND UserId = @UserId;", conn);
                        GetOrgMember.Parameters.Add(new SqlParameter("OrgId", _OrgId));
                        GetOrgMember.Parameters.Add(new SqlParameter("UserId", _UserId));

                        using (SqlDataReader reader = GetOrgMember.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _DateTimeJoined = Convert.ToDateTime(reader[2]);
                                _AccessLevel = Convert.ToInt32(reader[3]);
                            }
                            else
                            {
                                //If reader.Read() returns false, no data was returned
                                _ErrMsg = "Error while downloading organisation member from online database. " +
                                    "No data was returned";
                                AppLog.Error("GET ORGMEMBER - " + _ErrMsg);
                                return false;
                            }
                        }
                    }
                    AppLog.Info(String.Format("GET ORGMEMBER - Got org member from online database successfully"));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while downloading organisation member from online database";
                    AppLog.Error("GET ORGMEMBER - " + _ErrMsg + ": " + e);
                    return false;
                }
                //Finally, check if org member exists in the local database. If not, ADD THEM!!! If so, UPDATE THEM!!!
                AppLog.Info("GET ORGMEMBER - Checking whether org member exists in local database");

                bool ExistsOnLocalDb;

                /*using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("GET ORGMEMBER - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("GET ORGMEMBER - Connection to local database opened successfully");



                    SqlCommand CheckLocalDb = new SqlCommand("SELECT * FROM OrgMembers WHERE OrgId = @OrgId AND " +
                        "UserId = @UserId;", conn);
                    CheckLocalDb.Parameters.Add(new SqlParameter("OrgId", _OrgId));
                    CheckLocalDb.Parameters.Add(new SqlParameter("UserId", _UserId));

                    using (SqlDataReader reader = CheckLocalDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ExistsOnLocalDb = true;
                            AppLog.Info("GET ORGMEMBER - OrgMember already exists in the local database!");
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
                        AppLog.Info("GET ORGMEMBER - Updated org member on local db successfully");
                    }
                    else
                    {
                        AppLog.Info("GET ORGMEMBER - Failed to update org member: " + _ErrMsg);
                        return false;
                    }
                }
                else
                {
                    if (Create())
                    {
                        AppLog.Info("GET ORGMEMBER - Created org member on local db successfully");
                    }
                    else
                    {
                        AppLog.Info("GET ORGMEMBER - Failed to create org member: " + _ErrMsg);
                        return false;
                    }
                }*/
            }
            AppLog.Info(String.Format("GET ORGMEMBER - Success!"));
            return true;
        }

        public bool Delete()
        {
            AppLog.Info("DELETE ORGMEMBER - Starting...");
            AppLog.Info("DELETE ORGMEMBER - OrgMember's Org's Id: " + _OrgId);
            AppLog.Info("DELETE ORGMEMBER - OrgMember's User's Id: " + _UserId);
            AppLog.Info("DELETE ORGMEMBER - OrgMember's User's FullName: " + MyUser.FullName);
            AppLog.Info("DELETE ORGMEMBER - OrgMember's User's Username: " + MyUser.Username);

            //Checks offline mode (can't delete users in offline mode)
            if (Data.OfflineMode)
            {
                _ErrMsg = "Cannot delete organisation members while in offline mode";
                AppLog.Info(String.Format("CREATE ORGMEMBER - Organisation member {0} was not deleted because " +
                    "offline mode is on", MyUser.Username));
                return false;
            }

            int AffectedRows = 0;

            //If offline mode is off, first try deleting from online database
            AppLog.Info("DELETE USER - Attempting to delete organisation member from online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("CREATE ORGMEMBER - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("CREATE ORGMEMBER - Connection to online database opened successfully");

                    SqlCommand DeleteOrgMember = new SqlCommand("DELETE FROM t_OrgMembers WHERE OrgId = @OrgId " +
                        "AND UserId = @UserId;", conn);

                    DeleteOrgMember.Parameters.Add(new SqlParameter("OrgId", _OrgId));
                    DeleteOrgMember.Parameters.Add(new SqlParameter("UserId", _UserId));

                    AffectedRows = DeleteOrgMember.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("DELETE ORGMEMBER - Organisation member {0} deleted from online database " +
                "successfully. {1} row(s) affected", MyUser.FullName, AffectedRows));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting organisation member from online database";
                AppLog.Error("DELETE ORGMEMBER - " + _ErrMsg + ": " + e);
                return false;
            }


            //If deleted from online database successfully, try local database
            AppLog.Info("DELETE USER - Attempting to delete organisation member from local database...");
            /*try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("DELETE ORGMEMBER - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("DELETE ORGMEMBER - Connection to local database opened successfully");

                    SqlCommand DeleteOrgMember = new SqlCommand("DELETE FROM OrgMembers WHERE OrgId = @OrgId " +
                        "AND UserId = @UserId;", conn);

                    DeleteOrgMember.Parameters.Add(new SqlParameter("OrgId", _OrgId));
                    DeleteOrgMember.Parameters.Add(new SqlParameter("UserId", _UserId));

                    AffectedRows = DeleteOrgMember.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("DELETE ORGMEMBER - Organisation member {0} deleted from local database " +
                "successfully. {1} row(s) affected", MyUser.FullName, AffectedRows));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting organisation member from local database";
                AppLog.Error("DELETE ORGMEMBER" + _ErrMsg + ": " + e);
                return false;
            }*/
            //AffectedRows = 0; Why did this need resetting?? 09/10/2017 15:50
            AppLog.Info(String.Format("DELETE ORGMEMBER - Success!"));
            return true;
        }

        private bool Validate()
        {
            AppLog.Info("VALIDATE ORGMEMBER - Starting...");
            try
            {
                //Put some access level validation here
                AppLog.Info("VALIDATE ORGMEMBER - Validation has not been implemented for organisation member yet");
            }
            catch (Exception e)
            {
                _ErrMsg = e.ToString();
                AppLog.Error("VALIDATE ORGMEMBER - Validation failed: " + _ErrMsg);
                return false;
            }
            AppLog.Info("VALIDATE ORGMEMBER - Success!");
            return true;
        }
    }
}
