using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace BTS_Class_Library
{
    public class User
    {
        private Guid _Id;
        private string _FName;
        private string _SName;
        private string _Username;
        private string _JobTitle;
        private List<Organisation> _Organisations;
        private string _EMail;
        private string _Password;
        private DateTime _DateTimeCreated;
        private string _ErrMsg;

        public Guid Id { get { return _Id; } }

        public string FName {
            get {
                return FName;
            }
            set
            {
                if(value.Length < 51)
                {
                    _FName = value;
                }
                else
                {
                    throw new Exception("Forename exceeds 50 characters");
                }
            }
        }

        public string SName {
            get {
                return _SName;
            }
            set {
                if(value.Length < 51)
                {
                    _SName = value;
                }
                else
                {
                    throw new Exception("Surname exceeds 50 characters");
                }
            }
        }

        public string FullName { get { return FName + " " + SName; } }

        public string Username
        {
            get { return _Username; }
            set
            {
                if (value.Length < 51)
                {
                    _Username = value;
                }
            }
        }

        public string JobTitle {
            get {
                return _JobTitle;
            }
            set
            { if (value.Length < 51)
                {
                    _JobTitle = value;
                } else {
                    throw new Exception("Job title exceeds 50 characters");
                }
            }
        }

        public List<Organisation> Organisations { get { return _Organisations; } }

        public string EMail { get {
                return _EMail;
            }
            set {
                if (value.Contains("@") && value.Contains("."))
                {
                    if (value.Length < 255)
                    {
                        _EMail = value;
                    }
                    else
                    {
                        throw new Exception("Email address exceeds 254 characters");
                    }
                }
                else
                {
                    throw new Exception("Invalid email address");
                }
            }
        }

        public string Password { get { return _Password; }
            set {
                if(value.Length < 51)
                {
                    if(value.Length > 6)
                    {
                        _Password = value;
                    }
                    else
                    {
                        throw new Exception("Password is too short (less than 6 characters)");
                    }
                }
                else
                {
                    throw new Exception("Password exceeds 50 characters");
                }
            }
        }

        public DateTime DateTimeCreated { get { return _DateTimeCreated; } }

        public string ErrMsg { get { return _ErrMsg; } }

        public User(Guid pId)
        {
            _Id = pId;
            Get();
        }

        public User()
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

                    SqlCommand CreateUser = new SqlCommand("INSERT INTO Users VALUES(Id, FName, SName, Username, JobTitle," +
                        "EMail, Password, DateTimeCreated);");
                    CreateUser.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateUser.Parameters.Add(new SqlParameter("FName", _FName));
                    CreateUser.Parameters.Add(new SqlParameter("SName", _SName));
                    CreateUser.Parameters.Add(new SqlParameter("Username", _Username));
                    CreateUser.Parameters.Add(new SqlParameter("JobTitle", _JobTitle));
                    CreateUser.Parameters.Add(new SqlParameter("EMail", _EMail));
                    CreateUser.Parameters.Add(new SqlParameter("Password", _Password));
                    CreateUser.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));

                    CreateUser.ExecuteNonQuery();
                }
            }
            catch(SqlException e)
            {
                _ErrMsg = "Error while creating user on local database: " + e;
                return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    conn.Open();

                    SqlCommand CreateUser = new SqlCommand("INSERT INTO t_BTS_Users VALUES(Id, FName, SName, Username, " +
                        "JobTitle, EMail, Password, DateTimeCreated);");
                    CreateUser.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateUser.Parameters.Add(new SqlParameter("FName", _FName));
                    CreateUser.Parameters.Add(new SqlParameter("SName", _SName));
                    CreateUser.Parameters.Add(new SqlParameter("Username", _Username));
                    CreateUser.Parameters.Add(new SqlParameter("JobTitle", _JobTitle));
                    CreateUser.Parameters.Add(new SqlParameter("EMail", _EMail));
                    CreateUser.Parameters.Add(new SqlParameter("Password", _Password));
                    CreateUser.Parameters.Add(new SqlParameter("DateTimeCreated", _DateTimeCreated));

                    CreateUser.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while creating user on online database: " + e;
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

                    SqlCommand UpdateUser = new SqlCommand("UPDATE Users SET FName = @FName, SName = @SName," +
                        "Username = @Username, JobTitle = @JobTitle, EMail = @EMail, Password = @Password" +
                        "WHERE Id = @Id;", conn);

                    UpdateUser.Parameters.Add(new SqlParameter("FName", _FName));
                    UpdateUser.Parameters.Add(new SqlParameter("SName", _SName));
                    UpdateUser.Parameters.Add(new SqlParameter("Username", _Username));
                    UpdateUser.Parameters.Add(new SqlParameter("JobTitle", _JobTitle));
                    UpdateUser.Parameters.Add(new SqlParameter("EMail", _EMail));
                    UpdateUser.Parameters.Add(new SqlParameter("Password", _Password));

                    UpdateUser.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while updating user on local database: " + e;
                return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    conn.Open();

                    SqlCommand UpdateUser = new SqlCommand("UPDATE t_BTS_Users SET FName = @FName, SName = @SName," +
                        "Username = @Username, JobTitle = @JobTitle, EMail = @EMail, Password = @Password" +
                        "WHERE Id = @Id;", conn);

                    UpdateUser.Parameters.Add(new SqlParameter("FName", _FName));
                    UpdateUser.Parameters.Add(new SqlParameter("SName", _SName));
                    UpdateUser.Parameters.Add(new SqlParameter("Username", _Username));
                    UpdateUser.Parameters.Add(new SqlParameter("JobTitle", _JobTitle));
                    UpdateUser.Parameters.Add(new SqlParameter("EMail", _EMail));
                    UpdateUser.Parameters.Add(new SqlParameter("Password", _Password));

                    UpdateUser.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while updating user on online database: " + e;
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

                    SqlCommand GetUser = new SqlCommand("SELECT * FROM Users WHERE Id = @Id;", conn);

                    GetUser.Parameters.Add(new SqlParameter("Id", _Id));

                    GetUser.ExecuteNonQuery();

                    using (SqlDataReader reader = GetUser.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _FName = reader[1].ToString().Trim();
                            _SName = reader[2].ToString().Trim();
                            _Username = reader[3].ToString().Trim();
                            _JobTitle = reader[4].ToString().Trim();
                            _EMail = reader[5].ToString().Trim();
                            _Password = reader[6].ToString().Trim();
                            _DateTimeCreated = Convert.ToDateTime(reader[7]);
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while downloading user from local database: " + e;
                return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    conn.Open();

                    SqlCommand DownloadUser = new SqlCommand("SELECT * FROM t_BTS_Users WHERE Id = @Id;", conn);

                    DownloadUser.Parameters.Add(new SqlParameter("Id", _Id));

                    DownloadUser.ExecuteNonQuery();

                    using (SqlDataReader reader = DownloadUser.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _FName = reader[1].ToString().Trim();
                            _SName = reader[2].ToString().Trim();
                            _Username = reader[3].ToString().Trim();
                            _JobTitle = reader[4].ToString().Trim();
                            _EMail = reader[5].ToString().Trim();
                            _Password = reader[6].ToString().Trim();
                            _DateTimeCreated = Convert.ToDateTime(reader[7]);
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while downloading user from online database: " + e;
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

                    SqlCommand DeleteUser = new SqlCommand("DELETE FROM Users WHERE Id = @Id;", conn);
                    DeleteUser.Parameters.Add(new SqlParameter("Id", _Id));

                    DeleteUser.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting user on local database: " + e;
                return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    conn.Open();

                    SqlCommand DeleteUser = new SqlCommand("DELETE FROM t_BTS_Users WHERE Id = @Id;", conn);
                    DeleteUser.Parameters.Add(new SqlParameter("Id", _Id));

                    DeleteUser.ExecuteNonQuery();
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting user on online database: " + e;
                return false;
            }
            return true;
        }
    }
}
