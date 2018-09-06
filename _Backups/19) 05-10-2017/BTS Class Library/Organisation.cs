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
        private Guid _Id;
        private string _Name;
        private DateTime _DateTimeCreated;
        private List<OrgMember> _Members;
        private string _ErrMsg;

        public Guid Id { get { return _Id; } }
        public string Name {
            get {
                return _Name;
            }
            set {
                if(value.Length < 51)
                {
                    _Name = value;
                }
                else
                {
                    throw new Exception("Organisation name exceeds 50 characters");
                }
            }
        }
        public DateTime DateTimeCreated { get { return _DateTimeCreated; } }
        public string ErrMsg { get { return _ErrMsg; } }
        public List<OrgMember> Members { get { return _Members; } }
        public int NoOfMembers { get { return _Members.Count; } }

        public Organisation(Guid pId)
        {
            _Id = pId;
            if (!Get()) { Data.UserFriendlyError(_ErrMsg); };
        }

        public Organisation()
        {
            _Id = Guid.NewGuid();
            _DateTimeCreated = DateTime.Now;
        }

        public bool Create()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    conn.Open();

                    SqlCommand CreateOrg = new SqlCommand("INSERT INTO Organisations VALUES(@Id, @Name, " +
                        "@DateTimeCreated);",
                        conn);
                    CreateOrg.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateOrg.Parameters.Add(new SqlParameter("Name", _Name));
                    CreateOrg.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));

                    CreateOrg.ExecuteNonQuery();
                }
            }
            catch(SqlException e)
            {
                _ErrMsg = "Error while creating organisation on local database: " + e;
                return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    conn.Open();

                    SqlCommand CreateOrg = new SqlCommand("INSERT INTO t_BTS_Organisations VALUES(@Id, @Name," +
                        "@DateTimeCreated);", conn);
                    CreateOrg.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateOrg.Parameters.Add(new SqlParameter("Name", _Name));
                    CreateOrg.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));

                    CreateOrg.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while creating organisation on online database: " + e;
                return false;
            }

            return true;
        }

        public bool Update()
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    conn.Open();

                    SqlCommand UpdateOrg = new SqlCommand("UPDATE Organisations SET Name = @Name WHERE Id = @Id;");
                    UpdateOrg.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateOrg.Parameters.Add(new SqlParameter("Name", _Name));

                    UpdateOrg.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while updating organisation on local database: " + e;
                return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    conn.Open();

                    SqlCommand UpdateOrg = new SqlCommand("UPDATE t_BTS_Organisations SET Name = @Name WHERE Id = @Id;");
                    UpdateOrg.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateOrg.Parameters.Add(new SqlParameter("Name", _Name));

                    UpdateOrg.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while updating organisation on online database: " + e;
                return false;
            }
            return true;
        }

        private bool Get()
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    conn.Open();

                    SqlCommand GetOrganisation = new SqlCommand("SELECT * FROM Organisations WHERE Id = @Id;", conn);
                    GetOrganisation.Parameters.Add(new SqlParameter("Id", _Id));

                    GetOrganisation.ExecuteNonQuery();

                    using (SqlDataReader reader = GetOrganisation.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _Name = reader[1].ToString();
                            _DateTimeCreated = Convert.ToDateTime(reader[2]);
                        }
                    }
                }
            }
            catch(SqlException e)
            {
                _ErrMsg = "Error while getting organisation from local database: " + e;
                return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    conn.Open();

                    SqlCommand GetOrganisation = new SqlCommand("SELECT * FROM t_BTS_Organisations WHERE Id = @Id;", conn);
                    GetOrganisation.Parameters.Add(new SqlParameter("Id", _Id));

                    GetOrganisation.ExecuteNonQuery();

                    using (SqlDataReader reader = GetOrganisation.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _Name = reader[1].ToString();
                            _DateTimeCreated = Convert.ToDateTime(reader[2]);
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while getting organisation from online database: " + e;
                return false;
            }

            return true;
        }

        public bool Delete()
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    conn.Open();

                    SqlCommand DeleteOrg = new SqlCommand("DELETE FROM Organisations WHERE Id = @Id;", conn);
                    DeleteOrg.Parameters.Add(new SqlParameter("Id", _Id));

                    DeleteOrg.ExecuteNonQuery();
                }
            }
            catch(SqlException e)
            {
                _ErrMsg = "Error while deleting organisation from local database: " + e;
                return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    conn.Open();

                    SqlCommand DeleteOrg = new SqlCommand("DELETE FROM t_BTS_Organisations WHERE Id = @Id;", conn);
                    DeleteOrg.Parameters.Add(new SqlParameter("Id", _Id));

                    DeleteOrg.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting organisation from online database: " + e;
                return false;
            }
            return true;
        }
    }
}
