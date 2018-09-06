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
        public class Assignee
        {
            private Bug _MyBug;
            private User _MyUser;
            private TimeSpan _TimeSpent;
            private int _AccessLevel;
            private DateTime _DateTimeCreated;
            public bool Uploaded;
            private string _ErrMsg;

            public Bug MyBug { get { return _MyBug; } }
            public User MyUser { get { return _MyUser; } }
            public TimeSpan TimeSpent { get { return _TimeSpent; } set { _TimeSpent += value; } }
            public int AccessLevel { get { return _AccessLevel; } set { AccessLevel = value; } }
            public DateTime DateTimeCreated { get { return _DateTimeCreated; } }
            public string ErrMsg { get { return _ErrMsg; } }

            internal Assignee(Bug pBug)
            {
                //You would do this when creating a new assignee
                _MyBug = pBug;
                _DateTimeCreated = DateTime.Now;
            }

            internal Assignee(Bug pBug, User pUser)
            {
                //You would do this when downloading an assignee
                
                _MyBug = pBug;
                _MyUser = pUser;
                if (!Get()) { Data.UserFriendlyError(_ErrMsg); };
            }

            public bool Create()
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        conn.Open();

                        SqlCommand CreateAssignee = new SqlCommand("INSERT INTO Assignees VALUES (BugId, UserId, " +
                            "TimeSpent, AccessLevel, DateTimeCreated, Uploaded);", conn);

                        CreateAssignee.Parameters.Add(new SqlParameter("BugId", _MyBug.Id));
                        CreateAssignee.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));
                        CreateAssignee.Parameters.Add(new SqlParameter("TimeSpent", _TimeSpent));
                        CreateAssignee.Parameters.Add(new SqlParameter("Accesslevel", _AccessLevel));
                        CreateAssignee.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));
                        CreateAssignee.Parameters.Add(new SqlParameter("Uploaded", Uploaded));

                        CreateAssignee.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while creating assignee on local database: " + e;
                    return false;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        conn.Open();

                        SqlCommand CreateAssignee = new SqlCommand("INSERT INTO t_BTS_Assignees VALUES (BugId, UserId, " +
                            "TimeSpent, AccessLevel, DateTimeCreated);", conn);

                        CreateAssignee.Parameters.Add(new SqlParameter("BugId", _MyBug.Id));
                        CreateAssignee.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));
                        CreateAssignee.Parameters.Add(new SqlParameter("TimeSpent", _TimeSpent));
                        CreateAssignee.Parameters.Add(new SqlParameter("Accesslevel", _AccessLevel));
                        CreateAssignee.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));

                        CreateAssignee.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while creating assignee on online database: " + e;
                    return false;
                }

                return true;
            }

            public bool Update()
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        conn.Open();

                        SqlCommand UpdateAssignee = new SqlCommand("UPDATE Assignees SET TimeSpent = @TimeSpent, " +
                            "AccessLevel = @AccessLevel WHERE BugId = @BugId AND UserId = @UserId;", conn);
                        UpdateAssignee.Parameters.Add(new SqlParameter("BugId", _MyBug.Id));
                        UpdateAssignee.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));
                        UpdateAssignee.Parameters.Add(new SqlParameter("TimeSpent", _TimeSpent));
                        UpdateAssignee.Parameters.Add(new SqlParameter("Accesslevel", _AccessLevel));

                        UpdateAssignee.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while updating assignee on local database: " + e;
                    return false;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        conn.Open();

                        SqlCommand UpdateAssignee = new SqlCommand("UPDATE t_BTS_Assignees SET TimeSpent = @TimeSpent, " +
                            "AccessLevel = @AccessLevel WHERE BugId = @BugId AND UserId = @UserId;", conn);
                        UpdateAssignee.Parameters.Add(new SqlParameter("BugId", _MyBug.Id));
                        UpdateAssignee.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));
                        UpdateAssignee.Parameters.Add(new SqlParameter("TimeSpent", _TimeSpent));
                        UpdateAssignee.Parameters.Add(new SqlParameter("Accesslevel", _AccessLevel));

                        UpdateAssignee.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while updating assignee on online database: " + e;
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

                        SqlCommand GetAssignee = new SqlCommand("SELECT * FROM Assignees WHERE BugId = @BugId AND " +
                            "UserId = UserId;", conn);
                        GetAssignee.Parameters.Add(new SqlParameter("BugId", _MyBug.Id));
                        GetAssignee.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));

                        using(SqlDataReader reader = GetAssignee.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _TimeSpent = new TimeSpan(Convert.ToInt32(reader[2]));
                                _AccessLevel = Convert.ToInt32(reader[3]);
                                _DateTimeCreated = Convert.ToDateTime(reader[4]);
                                Uploaded = Convert.ToBoolean(reader[5]);
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while getting assignee from local database: " + e;
                    return false;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        conn.Open();

                        SqlCommand GetAssignee = new SqlCommand("SELECT * FROM t_BTS_Assignees WHERE BugId = @BugId AND " +
                            "UserId = UserId;", conn);
                        GetAssignee.Parameters.Add(new SqlParameter("BugId", _MyBug.Id));
                        GetAssignee.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));

                        using (SqlDataReader reader = GetAssignee.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _TimeSpent = new TimeSpan(Convert.ToInt32(reader[2]));
                                _AccessLevel = Convert.ToInt32(reader[3]);
                                _DateTimeCreated = Convert.ToDateTime(reader[4]);
                                Uploaded = Convert.ToBoolean(reader[5]);
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while getting assignee from online database: " + e;
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

                        SqlCommand DeleteAssignee = new SqlCommand("DETETE FROM Assignees WHERE BugId = @BugId AND " +
                            "Assignee = @Assignee;", conn);
                        DeleteAssignee.Parameters.Add(new SqlParameter("BugId", _MyBug.Id));
                        DeleteAssignee.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));

                        DeleteAssignee.ExecuteNonQuery();
                    }
                }
                catch(SqlException e)
                {
                    _ErrMsg = "Error while deleting assignee from local database: " + e;
                    return false;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        conn.Open();

                        SqlCommand DeleteAssignee = new SqlCommand("DETETE FROM t_BTS_Assignees WHERE BugId = @BugId AND " +
                            "Assignee = @Assignee;", conn);
                        DeleteAssignee.Parameters.Add(new SqlParameter("BugId", _MyBug.Id));
                        DeleteAssignee.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));

                        DeleteAssignee.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while deleting assignee from online database: " + e;
                    return false;
                }
                
                return true;
            }
        }
    }
}
