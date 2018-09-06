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
        public class Note
        {
            private Guid _Id;
            private Bug _MyBug;
            private User _MyUser;
            private DateTime _DateTimeCreated;
            private DateTime _DateTimeUpdated;
            private string _Title;
            private string _Body;
            public bool Uploaded;
            private string _ErrMsg;

            internal Note(Guid pId)
            {
                _Id = pId;
                if (!Get()) { Data.UserFriendlyError(_ErrMsg); };
            }

            internal Note(Bug pBug, User pUser)
            {
                _Id = Guid.NewGuid();
                _DateTimeCreated = DateTime.Now;
                _MyBug = pBug;
                _MyUser = pUser;
            }

            public Guid Id { get { return _Id; } }
            public Bug MyBug { get { return _MyBug; } }
            public User MyUser { get { return _MyUser; } }
            public DateTime DateTimeCreated { get { return _DateTimeCreated; } }
            public DateTime DateTimeUpdated { get { return _DateTimeUpdated; } }
            public string Title
            {
                get { return _Title; }

                set
                {
                    if (value.Length < 51){
                        Uploaded = false;
                        _Body = value;
                    }
                    else
                    {
                        throw new Exception("Title exceeds 50 characters");
                    }
                }
            }
            public string Body
            {
                get { return Body; }

                set
                {
                    if (value.Length < 1001)
                    {
                        Uploaded = false;
                        _Body = value;
                    }
                    else
                    {
                        throw new Exception("Body exceeds 1000 characters");
                    }
                }
            }
            public string ErrMsg { get { return _ErrMsg; } }

            public bool Create()
            {
                AppLog.Info("CREATE NOTE - Starting...");

                /*if (Data.OfflineMode)
                {
                    _ErrMsg = "Cannot create notes while in offline mode";
                    AppLog.Info(String.Format("CREATE NOTE - Note {0} was not created because offline mode is on", 
                        MyUser.Username));
                    return false;
                }*/
                
                _DateTimeUpdated = DateTime.Now;

                if (!Data.OfflineMode)
                {
                    AppLog.Info("CREATE NOTE - Attempting to create note on online database...");
                    try
                    {
                        AppLog.Info("CREATE NOTE - Attempting to create note on online database...");
                        using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                        {
                            AppLog.Info("CREATE NOTE - Attempting to open connection to online database...");
                            conn.Open();
                            AppLog.Info("CREATE NOTE - Connection to online database opened successfully");

                            SqlCommand CreateNote = new SqlCommand("INSERT INTO t_BTS_Notes VALUES (@Id, @BugId, " +
                                "@UserId, @DateTimeCreated, @DateTimeUpdated, @Title, @Body", conn);
                            CreateNote.Parameters.Add(new SqlParameter("Id", _Id));
                            CreateNote.Parameters.Add(new SqlParameter("BugId", _MyBug.Id));
                            CreateNote.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));
                            CreateNote.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));
                            CreateNote.Parameters.Add(new SqlParameter("DateTimeUpdated", _DateTimeUpdated));
                            CreateNote.Parameters.Add(new SqlParameter("Title", _Title));
                            CreateNote.Parameters.Add(new SqlParameter("Body", _Body));

                            CreateNote.ExecuteNonQuery();
                        }
                        Uploaded = true;
                        AppLog.Info(String.Format("CREATE NOTE - Note {0} created on online database successfully", Body));
                    }
                    catch (SqlException e)
                    {
                        _ErrMsg = "Error creating note on online database";
                        AppLog.Error(_ErrMsg + ": " + e);
                        return false;
                    }
                }
                else
                {
                    AppLog.Info(String.Format("CREATE NOTE - Offline mode is ON. Skipping create note on" +
                        "online database"));
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        AppLog.Info("CREATE NOTE - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("CREATE NOTE - Connection to local database opened successfully");

                        SqlCommand CreateNote = new SqlCommand("INSERT INTO Notes VALUES (@Id, @BugId, @UserId," +
                            "@DateTimeCreated, @DateTimeUpdated, @Title, @Body, @Uploaded", conn);
                        CreateNote.Parameters.Add(new SqlParameter("Id", _Id));
                        CreateNote.Parameters.Add(new SqlParameter("BugId", _MyBug.Id));
                        CreateNote.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));
                        CreateNote.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));
                        CreateNote.Parameters.Add(new SqlParameter("DateTimeUpdated", _DateTimeUpdated));
                        CreateNote.Parameters.Add(new SqlParameter("Title", _Title));
                        CreateNote.Parameters.Add(new SqlParameter("Body", _Body));
                        CreateNote.Parameters.Add(new SqlParameter("Uploaded", Uploaded));

                        CreateNote.ExecuteNonQuery();
                    }
                    AppLog.Info(String.Format("CREATE NOTE - Note {0} created on local database successfully", this.Title));
                }
                catch (SqlException e)
                {
                    /*_ErrMsg = "Error creating note on local database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;*/

                    if (Data.OfflineMode) //If we are offline the only copy is local. Else it doesn't matter if this fails.
                    {
                        _ErrMsg = "Error while creating note on local database. Changes were not saved";
                        AppLog.Error(_ErrMsg + ": " + e);
                        return false;
                    }
                    else
                    {
                        _ErrMsg = "Error while creating note on local database. Changes were saved online so " +
                            "no action required. Continuing... ";
                        AppLog.Error(_ErrMsg + ": " + e);
                    }
                }
                
                AppLog.Info("CREATE NOTE - Success!");
                return true;
            }

            public bool Update()
            {
                AppLog.Info("UPDATE NOTE - Starting...");
                if (!Data.OfflineMode)
                {
                    AppLog.Info("UPDATE NOTE - Offline mode is OFF");

                    AppLog.Info("UPDATE NOTE - Attempting to update note on online database...");
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                        {
                            AppLog.Info("UPDATE NOTE - Attempting to open connection to online database...");
                            conn.Open();
                            AppLog.Info("UPDATE NOTE - Connection to online> database opened successfully");
                            
                            SqlCommand UpdateNote = new SqlCommand("UPDATE t_BTS_Notes SET DateTimeUpdated = " +
                                "@DateTimeUpdated, Title = @Title, Body = @Body WHERE Id = @Id", conn);
                            UpdateNote.Parameters.Add(new SqlParameter("Id", _Id));
                            UpdateNote.Parameters.Add(new SqlParameter("DateTimeUpdated", _DateTimeUpdated));
                            UpdateNote.Parameters.Add(new SqlParameter("Title", _Title));
                            UpdateNote.Parameters.Add(new SqlParameter("Body", _Body));

                            UpdateNote.ExecuteNonQuery();
                        }
                        Uploaded = true;
                        AppLog.Info(String.Format("UPDATE NOTE - Note {0} updated on online database successfully",Title));
                    }
                    catch (SqlException e)
                    {
                        _ErrMsg = "Error updating note on online database";
                        AppLog.Error(_ErrMsg + ": " + e);
                        return false;
                    }
                }
                else
                {
                    AppLog.Info("UPDATE NOTE - Offline mode is ON. Skipping update note on online database");
                }

                AppLog.Info("UPDATE NOTE - Attempting to update note on local database...");
                try
                {
                    using(SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        AppLog.Info("UPDATE NOTE - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("UPDATE NOTE - Connection to online> database local successfully");

                        SqlCommand UpdateNote = new SqlCommand("UPDATE Notes SET DateTimeUpdated = @DateTimeUpdated," +
                            "Title = @Title, Body = @Body WHERE Id = @Id", conn);
                        UpdateNote.Parameters.Add(new SqlParameter("Id", _Id));
                        UpdateNote.Parameters.Add(new SqlParameter("DateTimeUpdated", _DateTimeUpdated));
                        UpdateNote.Parameters.Add(new SqlParameter("Title", _Title));
                        UpdateNote.Parameters.Add(new SqlParameter("Body", _Body));

                        UpdateNote.ExecuteNonQuery();
                    }
                    AppLog.Info(String.Format("UPDATE NOTE - Note {0} updated on local database successfully", Title));
                }
                catch(SqlException e)
                {
                    /*_ErrMsg = "Error updating note on local database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;*/

                    if (Data.OfflineMode)
                    {
                        _ErrMsg = "Error while updating note on local database. Changes were not saved.";
                        AppLog.Error(_ErrMsg + ": " + e);
                        return false;
                    }
                    else
                    {
                        _ErrMsg = "Error while updating note on local database. Changes were saved online so " +
                            "no action required. Continuing...";
                        AppLog.Error(_ErrMsg + ": " + e);
                    }
                }
                AppLog.Info("UPDATE NOTE - Success!");
                return true;
            }

            private bool Get()
            {
                AppLog.Info("GET NOTE - Starting...");
                if (Data.OfflineMode)
                {
                    AppLog.Info("GET NOTE - Offline mode is ON, retrieving cached note data from local database");
                    AppLog.Info("GET NOTE - Attempting to get note from local database...");
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(LocalConnStr))
                        {
                            AppLog.Info("GET NOTE - Attempting to open connection to local database...");
                            conn.Open();
                            AppLog.Info("GET NOTE - Connection to local database opened successfully");
                            
                            SqlCommand GetNote = new SqlCommand("SELECT * FROM Notes WHERE Id = @Id;;", conn);
                            GetNote.Parameters.Add(new SqlParameter("Id", _Id));

                            using (SqlDataReader reader = GetNote.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    _MyBug = new Bug(new Guid(reader[1].ToString()));
                                    _MyUser = new User(new Guid(reader[2].ToString()));
                                    _DateTimeCreated = Convert.ToDateTime(reader[3]);
                                    _DateTimeUpdated = Convert.ToDateTime(reader[4]);
                                    _Title = reader[5].ToString();
                                    _Body = reader[6].ToString();
                                    Uploaded = Convert.ToBoolean(reader[7]);
                                }
                            }
                        }
                        AppLog.Info(String.Format("GET NOTE - Note {0} downloaded from local database successfully", 
                            Title));
                    }
                    catch (SqlException e)
                    {
                        _ErrMsg = "Error getting note from local database";
                        AppLog.Error(_ErrMsg + ": " + e);
                        return false;
                    }
                }
                else
                {
                    AppLog.Info("GET NOTE - Offline mode is OFF, retrieving note data from online database");
                    AppLog.Info("GET NOTE - Attempting to download note from online database...");
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                        {
                            AppLog.Info("GET NOTE - Attempting to open connection to online database...");
                            conn.Open();
                            AppLog.Info("GET NOTE - Connection to online database opened successfully");
                            
                            SqlCommand DownloadNote = new SqlCommand("SELECT * FROM t_BTS_Notes WHERE Id = @Id;", conn);
                            DownloadNote.Parameters.Add(new SqlParameter("Id", _Id));

                            using (SqlDataReader reader = DownloadNote.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    _MyBug = new Bug(new Guid(reader[1].ToString()));
                                    _MyUser = new User(new Guid(reader[2].ToString()));
                                    _DateTimeCreated = Convert.ToDateTime(reader[3]);
                                    _DateTimeUpdated = Convert.ToDateTime(reader[4]);
                                    _Title = reader[5].ToString();
                                    _Body = reader[6].ToString();
                                }
                            }
                        }
                        AppLog.Info(String.Format("GET NOTE - Note {0} downloaded from online database successfully", 
                            Title));
                    }
                    catch (SqlException e)
                    {
                        _ErrMsg = "Error downloading note from online database";
                        AppLog.Error(_ErrMsg + ": " + e);
                        return false;
                    }
                }
                AppLog.Info("GET NOTE - Success!");
                return true;
            }

            public bool Delete()
            {
                AppLog.Info("<FUNCTION> <CLASS> - Starting...");
                AppLog.Info("DELETE NOTE - Attempting to delete note from local database...");
                try
                {
                    using(SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        AppLog.Info("DELETE NOTE - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("DELETE NOTE - Connection to local database opened successfully");
                        SqlCommand DeleteNote = new SqlCommand("DELETE FROM Notes WHERE Id = @Id;", conn);
                        DeleteNote.Parameters.Add(new SqlParameter("Id", _Id));

                        DeleteNote.ExecuteNonQuery();
                    }
                    AppLog.Info(String.Format("DELETE NOTE - Note {0} deleted from local database successfully", Title));
                }
                catch(SqlException e)
                {
                    _ErrMsg = "Error deleting note from local database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }

                AppLog.Info("DELETE NOTE - Attempting to delete note from online database...");
                try
                {
                    AppLog.Info("DELETE NOTE - Attempting to delete note from online database...");
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        AppLog.Info("DELETE NOTE - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("DELETE NOTE - Connection to online database opened successfully");

                        SqlCommand DeleteNote = new SqlCommand("DELETE FROM t_BTS_Notes WHERE Id = @Id;", conn);
                        DeleteNote.Parameters.Add(new SqlParameter("Id", _Id));

                        DeleteNote.ExecuteNonQuery();
                    }
                    AppLog.Info(String.Format("DELETE NOTE - Note {0} deleted from online database successfully", 
                        Title));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error deleting note from online database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
                AppLog.Info("DELETE NOTE - Success!");
                return true;
            }
        }
    }
    
}
