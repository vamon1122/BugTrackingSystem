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
            Get();
        }

        public Bug(User pRaisedBy)
        {
            _Id = Guid.NewGuid();
            _RaisedBy = pRaisedBy;
            Create();
        }

        public bool Create()
        {
            bool _Success = true;

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
                    CreateBug.Parameters.Add(new SqlParameter("Uploaded", BitConverter.GetBytes(_Uploaded)));

                    CreateBug.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _Success = false;
                _ErrorMsg = "Error whilst creating user on the local database: " + e;
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
                _ErrorMsg = "Error whilst creating user on the online database: " + e;
                return false;
            }

            _Uploaded = true;

            try
            {
                using (SqlConnection conn = new SqlConnection(LocalConnStr))
                {
                    conn.Open();

                    SqlCommand CreateBug = new SqlCommand("INSERT INTO Bugs (Uploaded) VALUES (@Uploaded);");
                    CreateBug.Parameters.Add(new SqlParameter("Uploaded", BitConverter.GetBytes(_Uploaded)));

                    CreateBug.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _Success = false;
                _ErrorMsg = "Error whilst setting uploaded indicator: " + e;
            }

            return _Success;
        }

        public bool Update()
        {
            bool _Success = true;

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

                    UpdateBug.ExecuteNonQuery();
                }
            }
            catch(SqlException e)
            {
                _Success = false;
                _ErrorMsg = "Error updating bug on the local database: " + e;
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
                _Success = false;
                _ErrorMsg = "Error updating bug on the online database: " + e;
            }

            try
            {
                using(SqlConnection conn = new SqlConnection(LocalConnStr))
                {
                    conn.Open();

                    SqlCommand UpdateBug = new SqlCommand("UPDATE Bugs SET Uploaded = @Uploaded WHERE Id = @Id", conn);

                    UpdateBug.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateBug.Parameters.Add(new SqlParameter("Uploaded", _Uploaded));
                }
            }
            catch(SqlException e)
            {
                _Success = false;
                _ErrorMsg = "Error whilst setting uploaded indicator: " + e;
            }

            return _Success;
        }

        private bool Get()
        {
            bool _Success = true;

            try
            {
                using (SqlConnection conn = new SqlConnection(LocalConnStr))
                {
                    conn.Open();

                    SqlCommand DownloadBug = new SqlCommand("SELECT * FROM Bugs WHERE Id = @Id ", conn);

                    DownloadBug.Parameters.Add(new SqlParameter("Id", _Id));

                    DownloadBug.ExecuteNonQuery();

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
                            _Uploaded = true;
                        }
                    }

                    SqlCommand DownloadAssignees = new SqlCommand("", conn);
                    DownloadAssignees.Parameters.Add(new SqlParameter("BugId", _Id));

                    using (SqlDataReader reader = DownloadAssignees.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Assignee MyAssignee = new Assignee(this);

                            MyAssignee._MyUser = new User(new Guid(reader[1].ToString()));
                            MyAssignee.TimeSpent = Convert.ToInt16 
                                (reader[2]);
                            _Assignees.Add(MyAssignee);
                        }
                    }

                    SqlCommand DownloadNotes = new SqlCommand("", conn);
                    DownloadNotes.Parameters.Add(new SqlParameter("BugId", _Id));

                    SqlCommand DownloadTags = new SqlCommand("", conn);
                    DownloadTags.Parameters.Add(new SqlParameter("BugId", _Id));



                }
            }
            catch (SqlException e)
            {
                _Success = false;
                _ErrorMsg = "Error updating bug on the local database: " + e;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                {
                    conn.Open();

                    SqlCommand DownloadBug = new SqlCommand("UPDATE t_BTS_Bugs SET Title = @Title, Description = @Description," +
                        "Severity = @Severity, ResolvedDateTime = @ResolvedDateTime, ReOpenedDateTime = @ReOpenedDateTime " +
                        "WHERE Id = @Id", conn);

                    DownloadBug.Parameters.Add(new SqlParameter("Id", _Id));
                    DownloadBug.Parameters.Add(new SqlParameter("Title", _Title));
                    DownloadBug.Parameters.Add(new SqlParameter("Description", _Description));
                    DownloadBug.Parameters.Add(new SqlParameter("Severity", _Severity));
                    DownloadBug.Parameters.Add(new SqlParameter("ResolvedDateTime", _ResolvedDateTime));
                    DownloadBug.Parameters.Add(new SqlParameter("ReOpenedDateTime", _ReOpenedDateTime));

                    SqlCommand DownloadAssignees = new SqlCommand("", conn);

                    SqlCommand DownloadNotes = new SqlCommand("", conn);

                    SqlCommand DownloadTags = new SqlCommand("", conn);

                }
            }
            catch (SqlException e)
            {
                _Success = false;
                _ErrorMsg = "Error updating bug on the online database: " + e;
            }

            return _Success;
        }

        public bool Delete()
        {
            bool _Success = false;

            return _Success;
        }

        public bool CreateTag()
        {
            bool _Success = false;

            return _Success;
        }

        public bool CreateAssignee()
        {
            bool _Success = false;

            return _Success;
        }

        public bool CreateNote()
        {
            bool _Success = false;

            return _Success;
        }

        public bool Resolve()
        {
            bool _Success = false;

            return _Success;
        }

        public bool ReOpen()
        {
            bool _Success = false;

            return _Success;
        }

        public bool SetPassword()
        {
            bool _Success = false;

            return _Success;
        }
    }
}
