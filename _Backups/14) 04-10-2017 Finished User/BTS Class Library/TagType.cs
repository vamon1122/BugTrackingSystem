using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace BTS_Class_Library
{
    public class TagType
    {
        private Guid _Id;
        private Organisation _MyOrg;
        private string _Value;
        public bool Uploaded;
        private string _ErrMsg;

        public TagType(Guid pId)
        {
            _Id = pId;
            Get();
        }

        public TagType(Organisation pOrg)
        {
            _Id = Guid.NewGuid();
            _MyOrg = pOrg;
            Uploaded = false;
        }


        public Guid Id { get { return _Id; } }
        public Organisation MyOrg { get { return _MyOrg; } }
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (value.Length > 50)
                {
                    throw new Exception("Tag value exceeds 50 characters");
                }
                else
                {
                    _Value = value;
                }
            }
        }

        public string ErrMsg { get { return _ErrMsg; } }

        public bool Create()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    conn.Open();

                    SqlCommand CreateTagType = new SqlCommand("INSERT INTO TagTypes VALUES(Id, OrgId, Value, " +
                        "Uploaded)", conn);
                    CreateTagType.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateTagType.Parameters.Add(new SqlParameter("OrgId", _MyOrg.Id));
                    CreateTagType.Parameters.Add(new SqlParameter("Value", _Value));
                    CreateTagType.Parameters.Add(new SqlParameter("Uploaded", Uploaded));

                    CreateTagType.ExecuteNonQuery();
                }
            }
            catch(SqlException e)
            {
                _ErrMsg = "Error while creating TagType on local database: " + e;
                return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    conn.Open();

                    SqlCommand CreateTagType = new SqlCommand("INSERT INTO t_BTS_TagTypes VALUES(Id, OrgId, Value, " +
                        "Uploaded)", conn);
                    CreateTagType.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateTagType.Parameters.Add(new SqlParameter("OrgId", _MyOrg.Id));
                    CreateTagType.Parameters.Add(new SqlParameter("Value", _Value));

                    CreateTagType.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while creating TagType on online database: " + e;
                return false;
            }

            return true;
        }

        public bool Get()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    conn.Open();

                    SqlCommand GetTagType = new SqlCommand("SELECT * FROM TagTypes WHERE Id = @Id;", conn);
                    GetTagType.Parameters.Add(new SqlParameter("Id", _Id));

                    GetTagType.ExecuteNonQuery();

                    using(SqlDataReader reader = GetTagType.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _MyOrg = new Organisation(reader[1].ToString());
                            _Value = reader[2].ToString();
                            Uploaded = Convert.ToBoolean(reader[3]);
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while getting TagType from local database: " + e;
                return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    conn.Open();

                    SqlCommand DownloadTagType = new SqlCommand("SELECT * FROM t_BTS_TagTypes WHERE Id = @Id;", conn);
                    DownloadTagType.Parameters.Add(new SqlParameter("Id", _Id));

                    DownloadTagType.ExecuteNonQuery();

                    using (SqlDataReader reader = DownloadTagType.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _MyOrg = new Organisation(reader[1].ToString());
                            _Value = reader[2].ToString();
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while downloading TagType from online database: " + e;
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

                    SqlCommand DeleteTagType = new SqlCommand("DELETE FROM TagTypes WHERE Id = @Id;", conn);
                    DeleteTagType.Parameters.Add(new SqlParameter("Id", _Id));

                    DeleteTagType.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting TagType from local database: " + e;
                return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    conn.Open();

                    SqlCommand DeleteTagType = new SqlCommand("DELETE FROM TagTypes WHERE Id = @Id;", conn);
                    DeleteTagType.Parameters.Add(new SqlParameter("Id", _Id));

                    DeleteTagType.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting TagType from online database: " + e;
                return false;
            }

            return true;
        }
    }
}
