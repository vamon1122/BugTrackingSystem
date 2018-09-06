using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace BTS_Class_Library
{
    public partial class Bug
    {
        public Bug(Guid pId)
        {
            _Id = pId;
            if (!Get()) { Data.Error(_ErrMsg); };
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
            try //Local
            {
                using (SqlConnection conn = new SqlConnection(LocalConnStr))
                {
                    conn.Open();

                    SqlCommand CreateBug = new SqlCommand("INSERT INTO Bugs VALUES (@Id, @RaisedBy, @Title, @Description," +
                        "@Severity, @CreatedDateTime, @ResolvedDateTime, @ReOpenedDateTime, @Uploaded);");
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
            }
            catch (SqlException e)
            {

                _ErrMsg = "Error whilst creating user on the local database: " + e;
                return false;
            }

            try //Online
            {
                using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                {
                    conn.Open();

                    SqlCommand CreateBug = new SqlCommand("INSERT INTO Bugs VALUES (@Id, @RaisedBy, @Title, @Description," +
                        "@Severity, @CreatedDateTime, @ResolvedDateTime, @ReOpenedDateTime;");
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
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error whilst creating user on the online database: " + e;
                return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(LocalConnStr))
                {
                    conn.Open();

                    SqlCommand CreateBug = new SqlCommand("INSERT INTO Bugs (Uploaded) VALUES (@Uploaded);");
                    CreateBug.Parameters.Add(new SqlParameter("Uploaded", BitConverter.GetBytes(Uploaded)));

                    CreateBug.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error whilst setting uploaded indicator: " + e;
                return false;
            }

            return true;
        }

        public bool Update()
        {
            Uploaded = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(LocalConnStr))
                {
                    conn.Open();

                    SqlCommand UpdateBug = new SqlCommand("UPDATE Bugs SET Title = @Title, Description = @Description," +
                        "Severity = @Severity, ResolvedDateTime = @ResolvedDateTime, ReOpenedDateTime = @ReOpenedDateTime " +
                        "WHERE Id = @Id", conn);

                    UpdateBug.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateBug.Parameters.Add(new SqlParameter("Title", _Title));
                    UpdateBug.Parameters.Add(new SqlParameter("Description", _Description));
                    UpdateBug.Parameters.Add(new SqlParameter("Severity", _Severity));
                    UpdateBug.Parameters.Add(new SqlParameter("ResolvedDateTime", _ResolvedDateTime));
                    UpdateBug.Parameters.Add(new SqlParameter("ReOpenedDateTime", _ReOpenedDateTime));
                    UpdateBug.Parameters.Add(new SqlParameter("Uploaded", BitConverter.GetBytes(Uploaded)));

                    UpdateBug.ExecuteNonQuery();
                }
            }
            catch(SqlException e)
            {
                _ErrMsg = "Error updating bug on the local database: " + e;
                return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                {
                    conn.Open();

                    SqlCommand UpdateBug = new SqlCommand("UPDATE t_BTS_Bugs SET Title = @Title, Description = @Description," +
                        "Severity = @Severity, ResolvedDateTime = @ResolvedDateTime, ReOpenedDateTime = @ReOpenedDateTime " +
                        "WHERE Id = @Id", conn);

                    UpdateBug.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateBug.Parameters.Add(new SqlParameter("Title", _Title));
                    UpdateBug.Parameters.Add(new SqlParameter("Description", _Description));
                    UpdateBug.Parameters.Add(new SqlParameter("Severity", _Severity));
                    UpdateBug.Parameters.Add(new SqlParameter("ResolvedDateTime", _ResolvedDateTime));
                    UpdateBug.Parameters.Add(new SqlParameter("ReOpenedDateTime", _ReOpenedDateTime));
                }
            }
            catch(SqlException e)
            {
                _ErrMsg = "Error updating bug on the online database: " + e;
                return false;
            }

            Uploaded = true;

            try
            {
                using(SqlConnection conn = new SqlConnection(LocalConnStr))
                {
                    conn.Open();

                    SqlCommand UpdateBug = new SqlCommand("UPDATE Bugs SET Uploaded = @Uploaded WHERE Id = @Id", conn);

                    UpdateBug.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateBug.Parameters.Add(new SqlParameter("Uploaded", Uploaded));
                }
            }
            catch(SqlException e)
            {
                _ErrMsg = "Error whilst setting uploaded indicator: " + e;
                return false;
            }

            return true;
        }

        /// <summary>
        /// This Get() method downloads the bug, the it's assignees, notes and tags
        /// </summary>
        /// <returns></returns>
        private bool Get()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(LocalConnStr))
                {
                    conn.Open();

                    SqlCommand DownloadBug = new SqlCommand("SELECT * FROM Bugs WHERE Id = @Id ", conn);

                    DownloadBug.Parameters.Add(new SqlParameter("Id", _Id));

                    using(SqlDataReader reader = DownloadBug.ExecuteReader())
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

                    SqlCommand DownloadAssignees = new SqlCommand("", conn);
                    DownloadAssignees.Parameters.Add(new SqlParameter("BugId", _Id));

                    using (SqlDataReader reader = DownloadAssignees.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _Assignees.Add(new Assignee(this, new User(new Guid(reader[1].ToString()))));
                        }
                    }

                    SqlCommand DownloadNotes = new SqlCommand("", conn);
                    DownloadNotes.Parameters.Add(new SqlParameter("BugId", _Id));
                    using (SqlDataReader reader = DownloadNotes.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _Notes.Add(new Note(new Guid(reader[0].ToString())));
                        }
                    }

                    SqlCommand DownloadTags = new SqlCommand("", conn);
                    DownloadTags.Parameters.Add(new SqlParameter("BugId", _Id));
                    using (SqlDataReader reader = DownloadTags.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _Tags.Add(new Tag(new Guid(reader[0].ToString())));
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error downloading bug from the local database: " + e;
                return false;
            }

            try
            {
                //Download from online
                using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                {
                    conn.Open();

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

                    SqlCommand DownloadAssignees = new SqlCommand("", conn);
                    DownloadAssignees.Parameters.Add(new SqlParameter("BugId", _Id));

                    using (SqlDataReader reader = DownloadAssignees.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _Assignees.Add(new Assignee(this, new User(new Guid(reader[1].ToString()))));
                        }
                    }

                    SqlCommand DownloadNotes = new SqlCommand("", conn);
                    DownloadNotes.Parameters.Add(new SqlParameter("BugId", _Id));
                    using (SqlDataReader reader = DownloadNotes.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _Notes.Add(new Note(new Guid(reader[0].ToString())));
                        }
                    }

                    SqlCommand DownloadTags = new SqlCommand("", conn);
                    DownloadTags.Parameters.Add(new SqlParameter("BugId", _Id));
                    using (SqlDataReader reader = DownloadTags.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _Tags.Add(new Tag(new Guid(reader[0].ToString())));
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error downloading bug from the online database: " + e;
                return false;
            }

            return true;
        }

        public bool Delete()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(LocalConnStr))
                {
                    conn.Open();

                    SqlCommand DeleteBug = new SqlCommand("DELETE FROM Bugs WHERE Id = @Id;", conn);
                    DeleteBug.Parameters.Add(new SqlParameter("Id", _Id));
                    DeleteBug.ExecuteNonQuery();

                    SqlCommand DeleteNotes = new SqlCommand("DELETE FROM Notes WHERE BugId = @BugId;", conn);
                    DeleteNotes.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteNotes.ExecuteNonQuery();

                    SqlCommand DeleteAssignees = new SqlCommand("DELETE FROM Assignees WHERE BugId = @BugId;", conn);
                    DeleteAssignees.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteAssignees.ExecuteNonQuery();

                    SqlCommand DeleteTags = new SqlCommand("DELETE FROM Tags WHERE BugId = @BubId;", conn);
                    DeleteTags.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteTags.ExecuteNonQuery();
                }
            }
            catch(SqlException e)
            {
                _ErrMsg = "Error deleting bug from the local database: " + e;
                return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                {
                    conn.Open();

                    SqlCommand DeleteBug = new SqlCommand("DELETE FROM t_BTS_Bugs WHERE Id = @Id;", conn);
                    DeleteBug.Parameters.Add(new SqlParameter("Id", _Id));
                    DeleteBug.ExecuteNonQuery();

                    SqlCommand DeleteNotes = new SqlCommand("DELETE FROM t_BTS_Notes WHERE BugId = @BugId;", conn);
                    DeleteNotes.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteNotes.ExecuteNonQuery();

                    SqlCommand DeleteAssignees = new SqlCommand("DELETE FROM t_BTS_Assignees WHERE BugId = @BugId;", conn);
                    DeleteAssignees.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteAssignees.ExecuteNonQuery();

                    SqlCommand DeleteTags = new SqlCommand("DELETE FROM Tags WHERE t_BTS_BugId = @BugId;", conn);
                    DeleteTags.Parameters.Add(new SqlParameter("BugId", _Id));
                    DeleteTags.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error deleting bug from the online database: " + e;
                return false;
            }

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
