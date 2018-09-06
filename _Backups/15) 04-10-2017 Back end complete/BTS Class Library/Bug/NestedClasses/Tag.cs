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
        public class Tag
        {
            private Guid _Id;
            private Bug _MyBug;
            private DateTime _DateTimeCreated;
            private TagType _MyTagType;
            public bool Uploaded;

            public Guid Id { get { return _Id; } }
            public Bug MyBug { get { return _MyBug; } }
            public DateTime DateTimeCreated { get { return _DateTimeCreated; } }
            public TagType Type { get { return _MyTagType; } }

            internal Tag(Guid pId)
            {
                _Id = pId;
                Get();
            }

            internal Tag()
            {
                _Id = Guid.NewGuid();
                _DateTimeCreated = DateTime.Now;
            }

            public bool Create()
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        conn.Open();

                        SqlCommand CreateTag = new SqlCommand("INSERT INTO Tags (@Id, @BugId, @DateTimeCreated, " +
                            "@Type, @Uploaded);");
                        CreateTag.Parameters.Add(new SqlParameter("Id", _Id));
                        CreateTag.Parameters.Add(new SqlParameter("BugId", MyBug.Id));
                        CreateTag.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));
                        CreateTag.Parameters.Add(new SqlParameter("Type", _MyTagType));
                        CreateTag.Parameters.Add(new SqlParameter("Uploaded", BitConverter.GetBytes(Uploaded)));

                        CreateTag.ExecuteNonQuery();
                    }
                }
                catch(SqlException e)
                {
                    _MyBug._ErrorMsg = "Error while creating tag on local database: " + e;
                    return false;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        conn.Open();

                        SqlCommand CreateTag = new SqlCommand("INSERT INTO t_BTS_Tags (@Id, @BugId, @DateTimeCreated, " +
                            "@Type);");
                        CreateTag.Parameters.Add(new SqlParameter("Id", _Id));
                        CreateTag.Parameters.Add(new SqlParameter("BugId", MyBug.Id));
                        CreateTag.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));
                        CreateTag.Parameters.Add(new SqlParameter("Type", _MyTagType));
                        
                        CreateTag.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    _MyBug._ErrorMsg = "Error while creating tag on online database: " + e;
                    return false;
                }

                return true;
            }

            private bool Get()
            {
                try
                {
                    using(SqlConnection conn = new SqlConnection(LocalConnStr))
                    {
                        conn.Open();

                        SqlCommand GetTag = new SqlCommand("SELECT * FROM Tags WHERE Id = @Id", conn);
                        GetTag.Parameters.Add(new SqlParameter("Id", _Id));

                        GetTag.ExecuteNonQuery();

                        using(SqlDataReader reader = GetTag.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _MyBug = new Bug(new Guid(reader[1].ToString()));
                                _DateTimeCreated = Convert.ToDateTime(reader[2]);
                                _MyTagType = new TagType(new Guid(reader[3].ToString()));
                            }
                        }
                    }
                }
                catch(SqlException e)
                {
                    _MyBug._ErrorMsg = "Error while getting tag from local database: " + e;
                    return false;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        conn.Open();

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
                }
                catch (SqlException e)
                {
                    _MyBug._ErrorMsg = "Error while getting tag from online database: " + e;
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

                        SqlCommand DeleteTag = new SqlCommand("DELETE FROM Tags WHERE Id = @Id;", conn);
                        DeleteTag.Parameters.Add(new SqlParameter("Id", _Id));

                        DeleteTag.ExecuteNonQuery();
                    }
                }
                catch(SqlException e)
                {
                    _MyBug._ErrorMsg = "Error while deleting tag from local database: " + e;
                    return false;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                    {
                        conn.Open();

                        SqlCommand DeleteTag = new SqlCommand("DELETE FROM t_BTS_Tags WHERE Id = @Id;", conn);
                        DeleteTag.Parameters.Add(new SqlParameter("Id", _Id));

                        DeleteTag.ExecuteNonQuery();
                    }
                }
                catch (SqlException e)
                {
                    _MyBug._ErrorMsg = "Error while deleting tag from online database: " + e;
                    return false;
                }

                return true;
            }
        }
    }
    
}
