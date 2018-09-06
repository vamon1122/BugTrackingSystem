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

                    SqlCommand CreateBug = new SqlCommand("INSERT INTO Bugs VALUES (@Id, @RaisedBy, @Title, @Description, @Severity, " +
                        "@CreatedDateTime, @ResolvedDateTime, @ReOpenedDateTime, @Uploaded);");
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
                _ErrorMsg = "Error whilst creating user on the local database: " + e.ToString();
            }

            try //Online
            {
                using (SqlConnection conn = new SqlConnection(OnlineConnStr))
                {
                    conn.Open();

                    SqlCommand CreateBug = new SqlCommand("INSERT INTO Bugs VALUES (@Id, @RaisedBy, @Title, @Description, @Severity, " +
                        "@CreatedDateTime, @ResolvedDateTime, @ReOpenedDateTime;");
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
                _ErrorMsg = "Error whilst creating user on the online database: " + e.ToString();
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
                _ErrorMsg = "Error whilst setting uploaded indicator: " + e.ToString();
            }



            return _Success;
        }

        public bool Update()
        {
            bool _Success = false;

            return _Success;
        }

        private bool Get()
        {
            bool _Success = false;

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
