using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenLog;
using System.Data.SqlClient;

namespace BTS_Class_Library
{
    public class Product 
    {
        private Guid _Id;
        private Organisation _MyOrg;
        private string _Name;
        public bool Uploaded;
        private string _ErrMsg;

        internal Product(Guid pId)
        {
            _Id = pId;
        }

        internal Product(Organisation pOrg)
        {
            _Id = Guid.NewGuid();
            _MyOrg = pOrg;
            Uploaded = false;
        }
        
        public Guid Id { get { return _Id; } }
        public Organisation MyOrg { get { return _MyOrg; } }
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value.Length > 50)
                {
                    throw new Exception("Product value exceeds 50 characters");
                }
                else
                {
                    Uploaded = false;
                    _Name = value;
                }
            }
        }

        public string ErrMsg { get { return _ErrMsg; } }

        public bool Create()
        {
            AppLog.Info("CREATE PRODUCT - Starting...");

            //Checks that data is valid before attempting upload
            AppLog.Info("CREATE PRODUCT - Validating...");
            if (!Validate())
            {
                AppLog.Info("CREATE PRODUCT - Product failed validation");
                return false;
            }
            AppLog.Info("CREATE PRODUCT - Product validated successfully");

            if (!Data.OfflineMode)
            {
                AppLog.Info("CREATE PRODUCT - Attempting to create product on online database...");

                AppLog.Info("CREATE PRODUCT - Attempting to create product on <online/local> database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                    {
                        AppLog.Info("CREATE PRODUCT - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("CREATE PRODUCT - Connection to local database opened successfully");

                        SqlCommand CreateProduct = new SqlCommand("INSERT INTO Products VALUES(@Id, @OrgId, @Name, " +
                            "@Uploaded)", conn);
                        CreateProduct.Parameters.Add(new SqlParameter("Id", _Id));
                        CreateProduct.Parameters.Add(new SqlParameter("OrgId", _MyOrg.Id));
                        CreateProduct.Parameters.Add(new SqlParameter("Name", _Name));
                        CreateProduct.Parameters.Add(new SqlParameter("Uploaded", Uploaded));

                        CreateProduct.ExecuteNonQuery();
                    }
                    Uploaded = true;
                    AppLog.Info(String.Format("CREATE PRODUCT - Product {0} created on local database successfully",
                        Name));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while creating product on local database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
            }
            else
            {
                AppLog.Info(String.Format("CREATE PRODUCT - Offline mode is ON. Skipping create product on" +
                        "online database"));
            }

            AppLog.Info("CREATE PRODUCT - Attempting to create product on online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("CREATE PRODUCT - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("CREATE PRODUCT - Connection to online database opened successfully");

                    SqlCommand CreateProduct = new SqlCommand("INSERT INTO t_BTS_Products VALUES(@Id, @OrgId, @Name);",
                        conn);
                    CreateProduct.Parameters.Add(new SqlParameter("Id", _Id));
                    CreateProduct.Parameters.Add(new SqlParameter("OrgId", _MyOrg.Id));
                    CreateProduct.Parameters.Add(new SqlParameter("Name", _Name));

                    CreateProduct.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("CREATE PRODUCT - Product {0} created on online database successfully",
                    Name));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while creating product on online database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info("CREATE PRODUCT - Success!");
            return true;
        }

        public bool Update()
        {
            AppLog.Info("UPDATE PRODUCT - Starting...");
            if (!Data.OfflineMode)
            {
                AppLog.Info("UPDATE PRODUCT - Offline mode is OFF");

                AppLog.Info("UPDATE PRODUCT - Attempting to update product on online database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("UPDATE PRODUCT - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("UPDATE PRODUCT - Connection to online database opened successfully");

                        SqlCommand UpdateProduct = new SqlCommand("UPDATE t_BTS_Products SET Name = @Name WHERE " +
                            "Id = @Id;", conn);

                        UpdateProduct.Parameters.Add(new SqlParameter("Name", _Name));
                        UpdateProduct.Parameters.Add(new SqlParameter("Id", _Id));

                        UpdateProduct.ExecuteNonQuery();
                    }
                    Uploaded = true;
                    AppLog.Info(String.Format("UPDATE PRODUCT - Product {0} updated on online database successfully",
                        Name));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while updating product on online database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
            }
            else
            {
                AppLog.Info("UPDATE PRODUCT - Offline mode is ON. Skipping update product on online database");
            }

            AppLog.Info("UPDATE PRODUCT - Attempting to update product on local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("UPDATE PRODUCT - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("UPDATE PRODUCT - Connection to local database opened successfully");

                    SqlCommand UpdateProduct = new SqlCommand("UPDATE Products SET Name = @Name, " +
                        "Uploaded = @Uploaded WHERE Id = @Id;", conn);

                    UpdateProduct.Parameters.Add(new SqlParameter("Name", _Name));
                    UpdateProduct.Parameters.Add(new SqlParameter("Id", _Id));
                    UpdateProduct.Parameters.Add(new SqlParameter("Uploaded", Uploaded));

                    UpdateProduct.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("UPDATE PRODUCT - Product {0} created on local database successfully",
                    Name));
            }
            catch (SqlException e)
            {
                if (Data.OfflineMode) //If we are offline the only copy is local. Else it doesn't matter if this fails.
                {
                    _ErrMsg = "Error while updating product on local database. Changes were not saved";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
                else
                {
                    _ErrMsg = "Error while updating product on local database. Changes were saved online so " +
                        "no action required. Continuing... ";
                    AppLog.Error(_ErrMsg + ": " + e);
                }
            }

            AppLog.Info("UPDATE PRODUCT - Success!");
            return true;
        }

        public bool Get()
        {
            AppLog.Info("GET PRODUCT - Starting...");
            if (Data.OfflineMode)
            {
                AppLog.Info("GET PRODUCT - Attempting to retrieve product from local database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                    {
                        AppLog.Info("GET PRODUCT - Attempting to open connection to local database...");
                        conn.Open();
                        AppLog.Info("GET PRODUCT - Connection to local database opened successfully");

                        SqlCommand GetProduct = new SqlCommand("SELECT * FROM Products WHERE Id = @Id;", conn);
                        GetProduct.Parameters.Add(new SqlParameter("Id", _Id));

                        GetProduct.ExecuteNonQuery();

                        using (SqlDataReader reader = GetProduct.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _MyOrg = new Organisation(new Guid(reader[1].ToString()));
                                _Name = reader[2].ToString();
                                Uploaded = Convert.ToBoolean(reader[3]);
                            }
                        }
                    }
                    AppLog.Info(String.Format("GET PRODUCT - Product {0} retrieved from local database successfully",
                        Name));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while getting product from local database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
            }
            else
            {
                AppLog.Info("GET PRODUCT - Attempting to retrieve product from online database...");
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("GET PRODUCT - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("GET PRODUCT - Connection to online database opened successfully");

                        SqlCommand DownloadProduct = new SqlCommand("SELECT * FROM t_BTS_Products WHERE Id = @Id;", conn);
                        DownloadProduct.Parameters.Add(new SqlParameter("Id", _Id));

                        DownloadProduct.ExecuteNonQuery();

                        using (SqlDataReader reader = DownloadProduct.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _MyOrg = new Organisation(new Guid(reader[1].ToString()));
                                _Name = reader[2].ToString();
                            }
                        }
                    }
                    AppLog.Info(String.Format("GET PRODUCT - Product {0} retrieved fron online database successfully",
                        Name));
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while downloading product from online database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
            }
            AppLog.Info("GET PRODUCT - Success!");
            return true;
        }

        public bool Delete()
        {
            AppLog.Info("DELETE PRODUCT - Starting...");

            AppLog.Info("DELETE PRODUCT - Attempting to delete product from local database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("DELETE PRODUCT - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("DELETE PRODUCT - Connection to local database opened successfully");

                    SqlCommand DeleteProduct = new SqlCommand("DELETE FROM Products WHERE Id = @Id;", conn);
                    DeleteProduct.Parameters.Add(new SqlParameter("Id", _Id));

                    DeleteProduct.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("DELETE PRODUCT - Product {0} deleted from local database successfully",
                    Name));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting product from local database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }

            AppLog.Info("DELETE PRODUCT - Attempting to delete product from online database...");
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                {
                    AppLog.Info("DELETE PRODUCT - Attempting to open connection to online database...");
                    conn.Open();
                    AppLog.Info("DELETE PRODUCT - Connection to online database opened successfully");

                    SqlCommand DeleteProduct = new SqlCommand("DELETE FROM Products WHERE Id = @Id;", conn);
                    DeleteProduct.Parameters.Add(new SqlParameter("Id", _Id));

                    DeleteProduct.ExecuteNonQuery();
                }
                AppLog.Info(String.Format("DELETE PRODUCT - Product {0} deleted from online database successfully",
                    Name));
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while deleting product from online database";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
            }
            AppLog.Info("DELETE PRODUCT - Success!");
            return true;
        }
        private bool Validate()
        {
            AppLog.Info("VALIDATE PRODUCT - Starting...");
            try
            {
                if (Name == null) { _ErrMsg = "Product has not been given a value"; throw new Exception(); }
                if (Name.Length > 50) { _ErrMsg = "Value of product exceeds 50 characters"; throw new Exception(); }
            }
            catch (Exception e)
            {
                AppLog.Error("VALIDATE PRODUCT - Validation failed: " + _ErrMsg);
                return false;
            }
            AppLog.Info("VALIDATE PRODUCT - Success!");
            return true;
        }
    }
}
