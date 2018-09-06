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
                    _ErrMsg = "Error creating note on local database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
                
                AppLog.Info("CREATE NOTE - Success!");
                return true;
            }

            public bool Update()
            {
                try
                {
                    using(SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        conn.Open();

                        SqlCommand UpdateNote = new SqlCommand("UPDATE Notes SET DateTimeUpdated = @DateTimeUpdated," +
                            "Title = @Title, Body = @Body WHERE Id = @Id", conn);
                        UpdateNote.Parameters.Add(new SqlParameter("Id", _Id));
                        UpdateNote.Parameters.Add(new SqlParameter("DateTimeUpdated", _DateTimeUpdated));
                        UpdateNote.Parameters.Add(new SqlParameter("Title", _Title));
                        UpdateNote.Parameters.Add(new SqlParameter("Body", _Body));

                        UpdateNote.ExecuteNonQuery();
                    }
                }
                catch(SqlException e)
                {
                    _ErrMsg = "Error updating note on local database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        conn.Open();

                        SqlCommand UpdateNote = new SqlCommand("UPDATE t_BTS_Notes SET DateTimeUpdated = @DateTimeUpdated," +
                            "Title = @Title, Body = @Body WHERE Id = @Id", conn);
                        UpdateNote.Parameters.Add(new SqlParameter("Id", _Id));
                        UpdateNote.Parameters.Add(new SqlParameter("DateTimeUpdated", _DateTimeUpdated));
                        UpdateNote.Parameters.Add(new SqlParameter("Title", _Title));
                        UpdateNote.Parameters.Add(new SqlParameter("Body", _Body));

                        UpdateNote.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error updating note on online database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
                return true;
            }

            private bool Get()
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        conn.Open();

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
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error getting note from local database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        conn.Open();

                        SqlCommand DownloadNote = new SqlCommand("SELECT * FROM t_BTS_Notes WHERE Id = @Id;", conn);
                        DownloadNote.Parameters.Add(new SqlParameter("Id", _Id));

                        using(SqlDataReader reader = DownloadNote.ExecuteReader())
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
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error downloading note from online database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
                return true;
            }

            public bool Delete()
            {
                try
                {
                    using(SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        SqlCommand DeleteNote = new SqlCommand("DELETE FROM Notes WHERE Id = @Id;", conn);
                        DeleteNote.Parameters.Add(new SqlParameter("Id", _Id));

                        DeleteNote.ExecuteNonQuery();
                    }
                }
                catch(SqlException e)
                {
                    _ErrMsg = "Error deleting note from local database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        SqlCommand DeleteNote = new SqlCommand("DELETE FROM t_BTS_Notes WHERE Id = @Id;", conn);
                        DeleteNote.Parameters.Add(new SqlParameter("Id", _Id));

                        DeleteNote.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error deleting note from online database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }

                return true;
            }
        }
    }
    
}
