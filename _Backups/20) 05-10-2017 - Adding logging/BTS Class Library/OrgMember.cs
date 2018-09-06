using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace BTS_Class_Library
{
    public partial class Organisation
    {
        public class OrgMember
        {
            private Organisation _MyOrg;
            private User _MyUser;
            private DateTime _DateTimeJoined;
            private int _AccessLevel;
            private string _ErrMsg;

            public Organisation MyOrg { get { return _MyOrg; } }
            public User MyUser { get { return _MyUser; } }
            public DateTime DateTimeJoined { get { return DateTimeJoined; } }
            public int AccessLevel { get { return _AccessLevel; }
                set { _AccessLevel = value; } }
            public string ErrMsg { get { return _ErrMsg; } }


            public OrgMember(User pUser, Organisation pOrg)
            {
                _MyOrg = pOrg;
                _MyUser = pUser;
                _DateTimeJoined = DateTime.Now;
            }

            public bool Create()
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                    {
                        conn.Open();

                        SqlCommand CreateOrgMember = new SqlCommand("INSERT INTO OrgMembers VALUES()", conn);
                        CreateOrgMember.Parameters.Add(new SqlParameter("OrgId", _MyOrg.Id));
                        CreateOrgMember.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));
                        CreateOrgMember.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeJoined));
                        CreateOrgMember.Parameters.Add(new SqlParameter("AccessLevel", _AccessLevel));
                    }
                }
                catch(SqlException e)
                {
                    _ErrMsg = "Error while creating organisation member on local database: " + e;
                    return false;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        conn.Open();

                        SqlCommand CreateOrgMember = new SqlCommand("INSERT INTO t_BTS_OrgMembers VALUES()", conn);
                        CreateOrgMember.Parameters.Add(new SqlParameter("OrgId", _MyOrg.Id));
                        CreateOrgMember.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));
                        CreateOrgMember.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeJoined));
                        CreateOrgMember.Parameters.Add(new SqlParameter("AccessLevel", _AccessLevel));
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while creating organisation member on online database: " + e;
                    return false;
                }

                return true;
            }

            public bool Update()
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                    {
                        conn.Open();

                        SqlCommand UpdateOrgMember = new SqlCommand("UPDATE OrgMembers SET AccessLevel = @AccessLevel " +
                            "WHERE OrgId = @OrgId AND UserId = @UserId VAL", conn);
                        UpdateOrgMember.Parameters.Add(new SqlParameter("OrgId", _MyOrg.Id));
                        UpdateOrgMember.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));
                        UpdateOrgMember.Parameters.Add(new SqlParameter("AccessLevel", _AccessLevel));

                        UpdateOrgMember.ExecuteNonQuery();
                    }
                }
                catch(SqlException e)
                {
                    _ErrMsg = "Error while updating organisation member on local database: " + e;
                    return false;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        conn.Open();

                        SqlCommand UpdateOrgMember = new SqlCommand("UPDATE t_BTS_OrgMembers SET AccessLevel = @AccessLevel " +
                            "WHERE OrgId = @OrgId AND UserId = @UserId VAL", conn);
                        UpdateOrgMember.Parameters.Add(new SqlParameter("OrgId", _MyOrg.Id));
                        UpdateOrgMember.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));
                        UpdateOrgMember.Parameters.Add(new SqlParameter("AccessLevel", _AccessLevel));

                        UpdateOrgMember.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while updating organisation member on online database: " + e;
                    return false;
                }

                return true;
            }

            private bool Get()
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                    {
                        conn.Open();

                        SqlCommand GetOrgMember = new SqlCommand("SELECT AccessLevel FROM OrgMembers WHERE OrgId = @OrgId " +
                            "AND UserId = @UserId;", conn);
                        GetOrgMember.Parameters.Add(new SqlParameter("OrgId", _MyOrg.Id));
                        GetOrgMember.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));

                        using(SqlDataReader reader = GetOrgMember.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _AccessLevel = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while getting organisation member from local database: " + e;
                    return false;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        conn.Open();

                        SqlCommand GetOrgMember = new SqlCommand("SELECT AccessLevel FROM t_BTS_OrgMembers " +
                            "WHERE OrgId = @OrgId AND UserId = @UserId;", conn);
                        GetOrgMember.Parameters.Add(new SqlParameter("OrgId", _MyOrg.Id));
                        GetOrgMember.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));

                        using (SqlDataReader reader = GetOrgMember.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _AccessLevel = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while downloading organisation member from online database: " + e;
                    return false;
                }
                return true;
            }

            public bool Delete()
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                    {
                        conn.Open();

                        SqlCommand DeleteOrgMember = new SqlCommand("DELETE FROM OrgMembers WHERE OrgId = @OrgId " +
                            "AND UserId = @UserId;");

                        DeleteOrgMember.Parameters.Add(new SqlParameter("OrgId", _MyOrg.Id));
                        DeleteOrgMember.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));

                        DeleteOrgMember.ExecuteNonQuery();
                    }
                }
                catch(SqlException e)
                {
                    _ErrMsg = "Error while deleting organisation member from local database: " + e;
                    return false;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        conn.Open();

                        SqlCommand DeleteOrgMember = new SqlCommand("DELETE FROM t_BTS_OrgMembers WHERE OrgId = @OrgId " +
                            "AND UserId = @UserId;");

                        DeleteOrgMember.Parameters.Add(new SqlParameter("OrgId", _MyOrg.Id));
                        DeleteOrgMember.Parameters.Add(new SqlParameter("UserId", _MyUser.Id));

                        DeleteOrgMember.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while deleting organisation member from online database: " + e;
                    return false;
                }
                return true;
            }
        }
    }
}
