using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BenLog;

namespace BTS_Class_Library
{
    public partial class Bug
    {
        public class Tag
        {
            private Guid _Id;
            private Bug _MyBug;
            private DateTime _DateTimeCreated;
            private TagType _MyTagType;
            //public bool Uploaded; Tag properties cannot be changed
            private string _ErrMsg;

            public Guid Id { get { return _Id; } }
            public Bug MyBug { get { return _MyBug; } }
            public DateTime DateTimeCreated { get { return _DateTimeCreated; } }
            public TagType Type { get { return _MyTagType; } }
            public string ErrMsg { get { return _ErrMsg; } }

            internal Tag(Guid pId)
            {
                _Id = pId;
                if (!Get()) { Data.UserFriendlyError(_ErrMsg); };
            }

            internal Tag()
            {
                _Id = Guid.NewGuid();
                _DateTimeCreated = DateTime.Now;
            }

            public bool Create()
            {
                if (!Data.OfflineMode)
                {
                    AppLog.Info("CREATE TAG - Attempting to create tag on online database...");
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                        {
                            AppLog.Info("CREATE TAG - Attempting to open connection to online database...");
                            conn.Open();
                            AppLog.Info("CREATE TAG - Connection to online database opened successfully");
                            
                            SqlCommand CreateTag = new SqlCommand("INSERT INTO t_BTS_Tags (@Id, @BugId, " +
                                "@DateTimeCreated, @Type);");
                            CreateTag.Parameters.Add(new SqlParameter("Id", _Id));
                            CreateTag.Parameters.Add(new SqlParameter("BugId", MyBug.Id));
                            CreateTag.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));
                            CreateTag.Parameters.Add(new SqlParameter("Type", _MyTagType));

                            CreateTag.ExecuteNonQuery();
                        }
                        //Uploaded = true; Tag properties cannot be changed
                        AppLog.Info(String.Format("CREATE TAG - Tag {0} created on online database successfully", 
                            Type.Value));
                    }
                    catch (SqlException e)
                    {
                        if (Data.OfflineMode) //If we are offline the only copy is local. Else it  
                        {                     //doesn't matter if this fails.
                            _ErrMsg = "Error while creating tag on local database. Changes were not saved";
                            AppLog.Error(_ErrMsg + ": " + e);
                            return false;
                        }
                        else
                        {
                            _ErrMsg = "Error while creating tag on local database. Changes were saved online so " +
                                "no action required. Continuing... ";
                            AppLog.Error(_ErrMsg + ": " + e);
                        }
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
                    using (SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        AppLog.Info("CREATE TAG - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("CREATE TAG - Connection to local database opened successfully");

                        SqlCommand CreateTag = new SqlCommand("INSERT INTO Tags (@Id, @BugId, @DateTimeCreated, " +
                            "@Type;");
                        CreateTag.Parameters.Add(new SqlParameter("Id", _Id));
                        CreateTag.Parameters.Add(new SqlParameter("BugId", MyBug.Id));
                        CreateTag.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));
                        CreateTag.Parameters.Add(new SqlParameter("Type", _MyTagType));

                        CreateTag.ExecuteNonQuery();
                    }
                    AppLog.Info(String.Format("CREATE TAG - Tag {0} created on local database successfully", Type.Value));
                }
                catch(SqlException e)
                {
                    _ErrMsg = "Error while creating tag on local database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }


                AppLog.Info("CREATE TAG - Success!");
                return true;
            }

            private bool Get()
            {
                AppLog.Info("GET TAG - Starting...");

                if (Data.OfflineMode)
                {
                    AppLog.Info("GET TAG - Offline mode is ON, retrieving cached tag data from local database");
                    AppLog.Info("GET TAG - Attempting to download tag from local database...");
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(LocalConnStr))
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
                                    _MyBug = new Bug(new Guid(reader[1].ToString()));
                                    _DateTimeCreated = Convert.ToDateTime(reader[2]);
                                    _MyTagType = new TagType(new Guid(reader[3].ToString()));
                                }
                            }
                        }
                        AppLog.Info(String.Format("GET TAG - <Class> {0} <function> on <online/local> database successfully", Type.Value));
                    }
                    catch (SqlException e)
                    {
                        _ErrMsg = "Error while getting tag from local database";
                        AppLog.Error(_ErrMsg + ": " + e);
                        return false;
                    }
                }
                else
                {
                    AppLog.Info("GET TAG - Offline mode is OFF, retrieving tag data from online database");
                    AppLog.Info("GET TAG - Attempting to retrieve tag from online database...");
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(OnlineConnStr))
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
                                    _MyBug = new Bug(new Guid(reader[1].ToString()));
                                    _DateTimeCreated = Convert.ToDateTime(reader[2]);
                                    _MyTagType = new TagType(new Guid(reader[3].ToString()));
                                }
                            }
                        }
                        AppLog.Info(String.Format("GET TAG - Tag {0} retrieved from local database successfully", Type.Value));
                    }
                    catch (SqlException e)
                    {
                        _ErrMsg = "Error while getting tag from online database";
                        AppLog.Error(_ErrMsg + ": " + e);
                        return false;
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
                    using (SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        AppLog.Info("DELETE TAG - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("DELETE TAG - Connection to <online/local> database opened successfully");
                        
                        SqlCommand DeleteTag = new SqlCommand("DELETE FROM Tags WHERE Id = @Id;", conn);
                        DeleteTag.Parameters.Add(new SqlParameter("Id", _Id));

                        DeleteTag.ExecuteNonQuery();
                    }
                    AppLog.Info(String.Format("DELETE TAG - Tag {0} deleted from local database successfully",Type.Value));
                }
                catch(SqlException e)
                {
                    _ErrMsg = "Error while deleting tag from local database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }

                AppLog.Info("DELETE TAG - Attempting to delete tag on online database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
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
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
                AppLog.Info("DELETE TAG - Success!");
                return true;
            }
        }
    }
    
}
