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
        public Bug(Guid pId)
        {
            _Id = pId;
            if (!Get()) { Data.UserFriendlyError(_ErrMsg); };
        }

        public Bug(User pRaisedBy)
        {
            _Id = Guid.NewGuid();
            _RaisedBy = pRaisedBy;
            Uploaded = false;
            _CreatedDateTime = DateTime.Now;
        }

        public bool Create()
        {
            AppLog.Info("CREATE BUG - Starting...");
            
            AppLog.Info("CREATE BUG - Attempting to CREATE BUG on online database...");
            try //Online
            {
                using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                {
                    AppLog.Info("CREATE BUG - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("CREATE BUG - Connection to online database opened successfully");

                    SqlCommand CreateBug = new SqlCommand("INSERT INTO Bugs VALUES (@Id, @RaisedBy, @Title, " +
                        "@Description, @Severity, @CreatedDateTime, @ResolvedDateTime, @ReOpenedDateTime;");
                    CreateBug.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateBug.Parameters.Add(new SqlParameter("RaisedBy", _RaisedBy));
                    CreateBug.Parameters.Add(new SqlParameter("Title", _Title));
                    CreateBug.Parameters.Add(new SqlParameter("Description", _Description));
                    CreateBug.Parameters.Add(new SqlParameter("Severity", _Severity));
                    CreateBug.Parameters.Add(new SqlParameter("CreatedDateTime", Data.DateTimeToSql(_CreatedDateTime)));
                    CreateBug.Parameters.Add(new SqlParameter("ResolvedDateTime", Data.DateTimeToSql(_ResolvedDateTime)));
                    CreateBug.Parameters.Add(new SqlParameter("ReOpenedDateTime", Data.DateTimeToSql(_ReOpenedDateTime)));

                    CreateBug.ExecuteNonQuery();
                }
                Uploaded = true;
                AppLog.Info(String.Format("CREATE BUG - Bug {0} created on online database successfully", Title));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error whilst creating user on the online database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }

            AppLog.Info("CREATE BUG - Attempting to create bug on local database...");
            try //Local
            {
                using (SqlConnection conn = new SqlConnection(LocalConnStr))
                {
                    AppLog.Info("CREATE BUG - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("CREATE BUG - Connection to local database opened successfully");

                    SqlCommand CreateBug = new SqlCommand("INSERT INTO Bugs VALUES (@Id, @RaisedBy, @Title, " +
                        "@Description, @Severity, @CreatedDateTime, @ResolvedDateTime, @ReOpenedDateTime, @Uploaded);");
                    CreateBug.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateBug.Parameters.Add(new SqlParameter("RaisedBy", _RaisedBy));
                    CreateBug.Parameters.Add(new SqlParameter("Title", _Title));
                    CreateBug.Parameters.Add(new SqlParameter("Description", _Description));
                    CreateBug.Parameters.Add(new SqlParameter("Severity", _Severity));
                    CreateBug.Parameters.Add(new SqlParameter("CreatedDateTime", Data.DateTimeToSql(_CreatedDateTime)));
                    CreateBug.Parameters.Add(new SqlParameter("ResolvedDateTime", Data.DateTimeToSql(_ResolvedDateTime)));
                    CreateBug.Parameters.Add(new SqlParameter("ReOpenedDateTime", Data.DateTimeToSql(_ReOpenedDateTime)));
                    CreateBug.Parameters.Add(new SqlParameter("Uploaded", BitConverter.GetBytes(Uploaded)));

                    CreateBug.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("CREATE BUG - Bug {0} created on local database successfully", Title));
            }
            catch (SqlException e)
            {
                /*_ErrMsg = "Error whilst creating bug on the local database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;*/
                if (Data.OfflineMode) //If we are offline the only copy is local. Else it doesn't matter if this fails.
                {
                    _ErrMsg = "Error while creating bug on local database. Changes were not saved";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
                else
                {
                    _ErrMsg = "Error while creating bug on local database. Changes were saved online so " +
                        "no action required. Continuing... ";
                    AppLog.Error(_ErrMsg + ": " + e);
                }
            }    

            /*AppLog.Info("CREATE BUG - Attempting to CREATE BUG on <online/local> database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(LocalConnStr))
                {
                    AppLog.Info("CREATE BUG - Attempting to open connection to <online/local> database...");
                    AppLog.Info("CREATE BUG - Connection to <online/local> database opened successfully");
                    conn.Open();

                    SqlCommand CreateBug = new SqlCommand("INSERT INTO Bugs (Uploaded) VALUES (@Uploaded);");
                    CreateBug.Parameters.Add(new SqlParameter("Uploaded", BitConverter.GetBytes(Uploaded)));

                    CreateBug.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("CREATE BUG - Bug {0} <function> on <online/local> database successfully", Title));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error whilst setting uploaded indicator";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }*/

            AppLog.Info("CREATE BUG - Success!");
            return true;
        }

        public bool Update()
        {
            AppLog.Info("UPDATE BUG - Starting...");

            AppLog.Info("UPDATE BUG - Attempting to update bug on online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                {
                    AppLog.Info("UPDATE BUG - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("UPDATE BUG - Connection to online database opened successfully");
                    
                    SqlCommand UpdateBug = new SqlCommand("UPDATE t_BTS_Bugs SET Title = @Title, Description = " +
                        "@Description, Severity = @Severity, ResolvedDateTime = @ResolvedDateTime, ReOpenedDateTime = " +
                        "@ReOpenedDateTime WHERE Id = @Id", conn);

                    UpdateBug.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateBug.Parameters.Add(new SqlParameter("Title", _Title));
                    UpdateBug.Parameters.Add(new SqlParameter("Description", _Description));
                    UpdateBug.Parameters.Add(new SqlParameter("Severity", _Severity));
                    UpdateBug.Parameters.Add(new SqlParameter("ResolvedDateTime", _ResolvedDateTime));
                    UpdateBug.Parameters.Add(new SqlParameter("ReOpenedDateTime", _ReOpenedDateTime));
                }
                Uploaded = true;
                AppLog.Info(String.Format("UPDATE BUG - Bug {0} updated on online database successfully", Title));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error updating bug on the online database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }

            AppLog.Info("UPDATE BUG - Attempting to update bug on local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(LocalConnStr))
                {
                    AppLog.Info("UPDATE BUG - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("UPDATE BUG - Connection to local database opened successfully");

                    SqlCommand UpdateBug = new SqlCommand("UPDATE Bugs SET Title = @Title, Description = @Description," +
                        "Severity = @Severity, ResolvedDateTime = @ResolvedDateTime, ReOpenedDateTime = " +
                        "@ReOpenedDateTime WHERE Id = @Id", conn);

                    UpdateBug.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateBug.Parameters.Add(new SqlParameter("Title", _Title));
                    UpdateBug.Parameters.Add(new SqlParameter("Description", _Description));
                    UpdateBug.Parameters.Add(new SqlParameter("Severity", _Severity));
                    UpdateBug.Parameters.Add(new SqlParameter("ResolvedDateTime", _ResolvedDateTime));
                    UpdateBug.Parameters.Add(new SqlParameter("ReOpenedDateTime", _ReOpenedDateTime));
                    UpdateBug.Parameters.Add(new SqlParameter("Uploaded", BitConverter.GetBytes(Uploaded)));

                    UpdateBug.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("UPDATE BUG - Bug {0} updated on local database successfully", Title));
            }
            catch(SqlException e)
            {
                if (Data.OfflineMode) //If we are offline the only copy is local. Else it doesn't matter if this fails.
                {
                    _ErrMsg = "Error while updating bug on local database. Changes were not saved";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
                else
                {
                    _ErrMsg = "Error while updating bug on local database. Changes were saved online so " +
                        "no action required. Continuing... ";
                    AppLog.Error(_ErrMsg + ": " + e);
                }
            }

            /*AppLog.Info("UPDATE BUG - Attempting to <function> bug on <online/local> database...");
            try
            {
                using(SqlConnection conn = new SqlConnection(LocalConnStr))
                {
                    AppLog.Info("UPDATE BUG - Attempting to open connection to <online/local> database...");
                    AppLog.Info("UPDATE BUG - Connection to <online/local> database opened successfully");
                    conn.Open();

                    SqlCommand UpdateBug = new SqlCommand("UPDATE Bugs SET Uploaded = @Uploaded WHERE Id = @Id", conn);

                    UpdateBug.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateBug.Parameters.Add(new SqlParameter("Uploaded", Uploaded));
                }
                AppLog.Info(String.Format("UPDATE BUG - Bug {0} <function> on <online/local> database successfully", Title));
            }
            catch(SqlException e)
            {
                _ErrMsg = "Error whilst setting uploaded indicator";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }*/
            AppLog.Info("UPDATE BUG - Success!");
            return true;
        }

        /// <summary>
        /// This Get() method downloads the bug, the it's assignees, notes and tags
        /// </summary>
        /// <returns></returns>
        private bool Get()
        {
            AppLog.Info("GET BUG - Starting...");

            if (Data.OfflineMode)
            {
                AppLog.Info("GET BUG - Offline mode is ON, retrieving cached bug data from local database");
                AppLog.Info("GET BUG - Attempting to retrieve bug from local database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        AppLog.Info("GET BUG - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("GET BUG - Connection to local database opened successfully");

                        AppLog.Info("GET BUG - Attempting to retrieve core bug from local database...");
                        SqlCommand DownloadBug = new SqlCommand("SELECT * FROM Bugs WHERE Id = @Id ", conn);

                        DownloadBug.Parameters.Add(new SqlParameter("Id", _Id));

                        using (SqlDataReader reader = DownloadBug.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _Id = new Guid(reader[0].ToString());
                                _RaisedBy = new User(new Guid(reader[1].ToString()));
                                _Title = reader[2].ToString().Trim();
                                _Description = reader[3].ToString();
                                _Severity = Convert.ToUInt16(reader[4]);
                                _CreatedDateTime = Convert.ToDateTime(reader[5]);
                                _CreatedDateTime = Convert.ToDateTime(reader[6]);
                                _CreatedDateTime = Convert.ToDateTime(reader[7]);
                                Uploaded = true;
                            }
                        }
                        AppLog.Info("GET BUG - Core bug retrieved from local database successfully");

                        AppLog.Info("GET BUG - Attempting to retrieve bug's assignees from local database...");
                        SqlCommand DownloadAssignees = new SqlCommand("", conn);
                        DownloadAssignees.Parameters.Add(new SqlParameter("BugId", _Id));

                        using (SqlDataReader reader = DownloadAssignees.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _Assignees.Add(new Assignee(this, new User(new Guid(reader[1].ToString()))));
                            }
                        }
                        AppLog.Info("GET BUG - Bug's assignees retrieved from local database successfully");

                        AppLog.Info("GET BUG - Attempting to retrieve bug's notes from local database...");
                        SqlCommand DownloadNotes = new SqlCommand("", conn);
                        DownloadNotes.Parameters.Add(new SqlParameter("BugId", _Id));
                        using (SqlDataReader reader = DownloadNotes.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _Notes.Add(new Note(new Guid(reader[0].ToString())));
                            }
                        }
                        AppLog.Info("GET BUG - Bug's notes retrieved from local database successfully");

                        AppLog.Info("GET BUG - Attempting to retrieve bug's tags from local database...");
                        SqlCommand DownloadTags = new SqlCommand("", conn);
                        DownloadTags.Parameters.Add(new SqlParameter("BugId", _Id));
                        using (SqlDataReader reader = DownloadTags.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _Tags.Add(new Tag(new Guid(reader[0].ToString())));
                            }
                        }
                        AppLog.Info("GET BUG - Bug's tags retrieved from local database successfully");
                    }
                    AppLog.Info(String.Format("GET BUG - Bug {0} retrieved from local database successfully", Title));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error downloading bug from the local database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
            }
            else
            {
                AppLog.Info("GET BUG - Offline mode is OFF, retrieving bug data from online database");
                AppLog.Info("GET BUG - Attempting to retrieve bug on local database...");
                try
                {
                    //Download from online
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        AppLog.Info("GET BUG - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("GET BUG - Connection to online database opened successfully");

                        AppLog.Info("GET BUG - Attempting to retrieve core bug from online database...");
                        SqlCommand DownloadBug = new SqlCommand("SELECT * FROM t_BTS_Bugs WHERE Id = @Id ", conn);

                        DownloadBug.Parameters.Add(new SqlParameter("Id", _Id));

                        using (SqlDataReader reader = DownloadBug.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _Id = new Guid(reader[0].ToString());
                                _RaisedBy = new User(new Guid(reader[1].ToString()));
                                _Title = reader[2].ToString().Trim();
                                _Description = reader[3].ToString();
                                _Severity = Convert.ToUInt16(reader[4]);
                                _CreatedDateTime = Convert.ToDateTime(reader[5]);
                                _CreatedDateTime = Convert.ToDateTime(reader[6]);
                                _CreatedDateTime = Convert.ToDateTime(reader[7]);
                                Uploaded = true;
                            }
                        }
                        AppLog.Info("GET BUG - Core bug retrieved from online database successfully");

                        AppLog.Info("GET BUG - Attempting to retrieve bug's assignees from local database...");
                        SqlCommand DownloadAssignees = new SqlCommand("", conn);
                        DownloadAssignees.Parameters.Add(new SqlParameter("BugId", _Id));

                        using (SqlDataReader reader = DownloadAssignees.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _Assignees.Add(new Assignee(this, new User(new Guid(reader[1].ToString()))));
                            }
                        }
                        AppLog.Info("GET BUG - Bug's assignees retrieved from online database successfully");

                        AppLog.Info("GET BUG - Attempting to retrieve bug's notes from local database...");
                        SqlCommand DownloadNotes = new SqlCommand("", conn);
                        DownloadNotes.Parameters.Add(new SqlParameter("BugId", _Id));
                        using (SqlDataReader reader = DownloadNotes.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _Notes.Add(new Note(new Guid(reader[0].ToString())));
                            }
                        }
                        AppLog.Info("GET BUG - Bug's notes retrieved from online database successfully");

                        AppLog.Info("GET BUG - Attempting to retrieve bug's tags from local database...");
                        SqlCommand DownloadTags = new SqlCommand("", conn);
                        DownloadTags.Parameters.Add(new SqlParameter("BugId", _Id));
                        using (SqlDataReader reader = DownloadTags.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _Tags.Add(new Tag(new Guid(reader[0].ToString())));
                            }
                        }
                        AppLog.Info("GET BUG - Bug's tags retrieved from online database successfully");
                    }
                    AppLog.Info(String.Format("GET BUG - Bug {0} retrieved on online database successfully", Title));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error downloading bug from the online database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
            }
            AppLog.Info("GET BUG - Success!");
            return true;
        }

        public bool Delete()
        {
            AppLog.Info("DELETE BUG - Starting...");

            AppLog.Info("DELETE BUG - Attempting to delete bug on local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(LocalConnStr))
                {
                    AppLog.Info("DELETE BUG - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("DELETE BUG - Connection to local database opened successfully");

                    AppLog.Info("DELETE BUG - Attempting to delete core bug from local database...");
                    SqlCommand DeleteBug = new SqlCommand("DELETE FROM Bugs WHERE Id = @Id;", conn);
                    DeleteBug.Parameters.Add(new SqlParameter("Id", _Id));
                    DeleteBug.ExecuteNonQuery();
                    AppLog.Info("DELETE BUG - Core bug deleted from local database successfully");

                    AppLog.Info("DELETE BUG - Attempting to delete bug's notes from local database...");
                    SqlCommand DeleteNotes = new SqlCommand("DELETE FROM Notes WHERE BugId = @BugId;", conn);
                    DeleteNotes.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteNotes.ExecuteNonQuery();
                    AppLog.Info("DELETE BUG - Bug's notes deleted from local database successfully");

                    AppLog.Info("DELETE BUG - Attempting to delete bug's assignees from online database...");
                    SqlCommand DeleteAssignees = new SqlCommand("DELETE FROM Assignees WHERE BugId = @BugId;", conn);
                    DeleteAssignees.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteAssignees.ExecuteNonQuery();
                    AppLog.Info("DELETE BUG - Bug's assignees deleted from online database successfully");

                    AppLog.Info("DELETE BUG - Attempting to delete bug's tags from online database...");
                    SqlCommand DeleteTags = new SqlCommand("DELETE FROM Tags WHERE BugId = @BubId;", conn);
                    DeleteTags.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteTags.ExecuteNonQuery();
                    AppLog.Info("DELETE BUG - Bug's tags deleted from online database successfully");
                }
                AppLog.Info(String.Format("DELETE BUG - Bug {0} <function> on <online/local> database successfully", Title));
            }
            catch(SqlException e)
            {
                _ErrMsg = "Error deleting bug from the local database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }

            AppLog.Info("DELETE BUG - Attempting to delete bug on online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                {
                    AppLog.Info("DELETE BUG - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("DELETE BUG - Connection to online database opened successfully");
                    
                    AppLog.Info("DELETE BUG - Attempting to delete core bug from online database...");
                    SqlCommand DeleteBug = new SqlCommand("DELETE FROM t_BTS_Bugs WHERE Id = @Id;", conn);
                    DeleteBug.Parameters.Add(new SqlParameter("Id", _Id));
                    DeleteBug.ExecuteNonQuery();
                    AppLog.Info("DELETE BUG - Core bug deleted from online database successfully");

                    AppLog.Info("DELETE BUG - Attempting to delete bug's notes from online database...");
                    SqlCommand DeleteNotes = new SqlCommand("DELETE FROM t_BTS_Notes WHERE BugId = @BugId;", conn);
                    DeleteNotes.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteNotes.ExecuteNonQuery();
                    AppLog.Info("DELETE BUG - Bug's notes deleted from online database successfully");

                    AppLog.Info("DELETE BUG - Attempting to delete bug's assignees from online database...");
                    SqlCommand DeleteAssignees = new SqlCommand("DELETE FROM t_BTS_Assignees WHERE BugId = @BugId;", conn);
                    DeleteAssignees.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteAssignees.ExecuteNonQuery();
                    AppLog.Info("DELETE BUG - Bug's assignees deleted from online database successfully");

                    AppLog.Info("DELETE BUG - Attempting to delete bug's tags from online database...");
                    SqlCommand DeleteTags = new SqlCommand("DELETE FROM Tags WHERE t_BTS_BugId = @BugId;", conn);
                    DeleteTags.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteTags.ExecuteNonQuery();
                    AppLog.Info("DELETE BUG - Bug's tags deleted from online database successfully");
                }
                AppLog.Info(String.Format("DELETE BUG - Bug {0} deleted from online database successfully", Title));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error deleting bug from the online database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info("DELETE BUG - Success!");
            return true;
        }

        public Tag CreateTag()
        {
            return new Tag();
        }

        public Assignee CreateAssignee()
        {
            return new Assignee(this);
        }

        public Note CreateNote(User pUser)
        {
            return new Note(this, pUser);
        }

        public void Resolve()
        {
            _ResolvedDateTime = DateTime.Now;
        }

        public void ReOpen()
        {
            _ReOpenedDateTime = DateTime.Now;
        }

    }
}
