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
        public class Note
        {
            private Guid _Id;
            private Bug _MyBug;
            private User _MyUser;
            private DateTime _DateTimeCreated;
            private DateTime _DateTimeUpdated;
            private string _MyNote;
            public bool Uploaded;

            internal Note(Guid pId)
            {
                _Id = pId;
                Get();
            }

            internal Note(Bug pBug, User pUser)
            {
                _Id = Guid.NewGuid();
                _DateTimeCreated = DateTime.Now;
                _MyBug = pBug;
                _MyUser = pUser;
                Create();
            }

            public Guid Id { get { return _Id; } }
            public Bug MyBug { get { return _MyBug; } }
            public User MyUser { get { return _MyUser; } }
            public DateTime DateTimeCreated { get { return _DateTimeCreated; } }
            public DateTime DateTimeUpdated { get { return _DateTimeUpdated; } }
            public string MyNote
            {
                get { return MyNote; }

                set
                {
                    if (value.Length < 1001){ 
                        _MyNote = value;
                    }
                    else
                    {
                        throw new Exception("Note exceeds 1000 characters");
                    }
                }
            }

            public bool Create()
            {
                _DateTimeUpdated = DateTime.Now;

                try
                {
                    using (SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        conn.Open();

                        SqlCommand CreateNote = new SqlCommand("INSERT INTO Notes VALUES (@Id, @BugId, @UserId," +
                            "@DateTimeCreated, @DateTimeUpdated, @Note, @Uploaded", conn);
                        CreateNote.Parameters.Add(new SqlParameter("Id", _Id));
                        CreateNote.Parameters.Add(new SqlParameter("BugId", _MyBug.Id));
                        CreateNote.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));
                        CreateNote.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));
                        CreateNote.Parameters.Add(new SqlParameter("DateTimeUpdated", _DateTimeUpdated));
                        CreateNote.Parameters.Add(new SqlParameter("Note", _MyNote));
                        CreateNote.Parameters.Add(new SqlParameter("Uploaded", Uploaded));

                        CreateNote.ExecuteNonQuery();
                    }
                }
                catch(SqlException e)
                {
                    MyBug._ErrorMsg = "Error creating note on local databade: " + e;
                    return false;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        conn.Open();

                        SqlCommand CreateNote = new SqlCommand("INSERT INTO t_BTS_Notes VALUES (@Id, @BugId, @UserId," +
                            "@DateTimeCreated, @DateTimeUpdated, @Note", conn);
                        CreateNote.Parameters.Add(new SqlParameter("Id", _Id));
                        CreateNote.Parameters.Add(new SqlParameter("BugId", _MyBug.Id));
                        CreateNote.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));
                        CreateNote.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));
                        CreateNote.Parameters.Add(new SqlParameter("DateTimeUpdated", _DateTimeUpdated));
                        CreateNote.Parameters.Add(new SqlParameter("Note", _MyNote));

                        CreateNote.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    MyBug._ErrorMsg = "Error creating note on online databade: " + e;
                    return false;
                }

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
                            "Note = @Note WHERE Id = @Id", conn);
                        UpdateNote.Parameters.Add(new SqlParameter("Id", _Id));
                        UpdateNote.Parameters.Add(new SqlParameter("DateTimeUpdated", _DateTimeUpdated));
                        UpdateNote.Parameters.Add(new SqlParameter("Note", _MyNote));

                        UpdateNote.ExecuteNonQuery();
                    }
                }
                catch(SqlException e)
                {
                    MyBug._ErrorMsg = "Error updating note on local database: " + e;
                    return false;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        conn.Open();

                        SqlCommand UpdateNote = new SqlCommand("UPDATE t_BTS_Notes SET DateTimeUpdated = @DateTimeUpdated," +
                            "Note = @Note WHERE Id = @Id", conn);
                        UpdateNote.Parameters.Add(new SqlParameter("Id", _Id));
                        UpdateNote.Parameters.Add(new SqlParameter("DateTimeUpdated", _DateTimeUpdated));
                        UpdateNote.Parameters.Add(new SqlParameter("Note", _MyNote));

                        UpdateNote.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    MyBug._ErrorMsg = "Error updating note on online database: " + e;
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

                        GetNote.ExecuteNonQuery();

                        using (SqlDataReader reader = GetNote.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _MyBug = new Bug(new Guid(reader[1].ToString()));
                                _MyUser = new User(new Guid(reader[2].ToString()));
                                _DateTimeCreated = Convert.ToDateTime(reader[3]);
                                _DateTimeUpdated = Convert.ToDateTime(reader[4]);
                                _MyNote = reader[5].ToString();
                                Uploaded = Convert.ToBoolean(reader[6]);
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    MyBug._ErrorMsg = "Error getting note from local database: " + e;
                    return false;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        conn.Open();

                        SqlCommand DownloadNote = new SqlCommand("SELECT * FROM t_BTS_Notes WHERE Id = @Id;", conn);
                        DownloadNote.Parameters.Add(new SqlParameter("Id", _Id));

                        DownloadNote.ExecuteNonQuery();

                        using(SqlDataReader reader = DownloadNote.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _MyBug = new Bug(new Guid(reader[1].ToString()));
                                _MyUser = new User(new Guid(reader[2].ToString()));
                                _DateTimeCreated = Convert.ToDateTime(reader[3]);
                                _DateTimeUpdated = Convert.ToDateTime(reader[4]);
                                _MyNote = reader[5].ToString();
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    MyBug._ErrorMsg = "Error downloading note from online database: " + e;
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
                    MyBug._ErrorMsg = "Error deleting note from local database: " + e;
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
                    MyBug._ErrorMsg = "Error deleting note from online database: " + e;
                    return false;
                }

                return true;
            }
        }
    }
    
}
