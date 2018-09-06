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

        internal Product(Organisation pOrg)
        {
            _Id = Guid.NewGuid();
            _MyOrg = pOrg;
            Uploaded = false;
        }

        public Product(Guid pId) //Need this to be public for when i'm creating lists for the UI;
        {
            _Id = pId;
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
                try
                {
                    using (SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
                    {
                        AppLog.Info("CREATE PRODUCT - Attempting to open connection to online database...");
                        conn.Open();
                        AppLog.Info("CREATE PRODUCT - Connection to online database opened successfully");

                        //This is a check to see weather the product already exists on the database. Obviously
                        //if it's already there, it doesn't need creating again, but this might be called
                        //if for example the product did not exist on the Online database, so the Create() function
                        //needed to be able to account for that.
                        AppLog.Info("CREATE PRODUCT - Checking that product doesn't already exist on Online database");
                        bool OnlineProductExists;

                        SqlCommand CheckOnlineDb = new SqlCommand("SELECT * FROM t_Products WHERE Id = @Id", conn);
                        CheckOnlineDb.Parameters.Add(new SqlParameter("Id", Id));
                        using (SqlDataReader reader = CheckOnlineDb.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                OnlineProductExists = true;
                                AppLog.Info("CREATE PRODUCT - Product already exists in online database!");
                            }
                            else
                            {
                                OnlineProductExists = false;
                                AppLog.Info("CREATE PRODUCT - Product does not exist in online database. Creating product on online database");
                            }
                        }
                        if (!OnlineProductExists)
                        {
                            SqlCommand CreateProduct = new SqlCommand("INSERT INTO t_Products VALUES(@Id, @OrgId, @Name);",
                            conn);
                            CreateProduct.Parameters.Add(new SqlParameter("Id", _Id));
                            CreateProduct.Parameters.Add(new SqlParameter("OrgId", _MyOrg.Id));
                            CreateProduct.Parameters.Add(new SqlParameter("Name", _Name));

                            CreateProduct.ExecuteNonQuery();

                            AppLog.Info(String.Format("CREATE PRODUCT - Product {0} created on online database successfully",
                            Name));
                        }
                    }
                }
                catch (SqlException e)
                {
                    _ErrMsg = "Error while creating product on online database";
                    AppLog.Error(_ErrMsg + ": " + e);
                    return false;
                }
                
            }
            else
            {
                AppLog.Info(String.Format("CREATE PRODUCT - Offline mode is ON. Skipping create product on" +
                        "online database"));
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("CREATE PRODUCT - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("CREATE PRODUCT - Connection to local database opened successfully");

                    //This is a check to see weather the product already exists on the database. Obviously
                    //if it's already there, it doesn't need creating again, but this might be called
                    //if for example the product did not exist on the local database, so the Create() function
                    //needed to be able to account for that.
                    AppLog.Info("CREATE PRODUCT - Checking that product doesn't already exist on local database");
                    bool LocalProductExists;

                    SqlCommand CheckLocalDb = new SqlCommand("SELECT * FROM Products WHERE Id = @Id", conn);
                    CheckLocalDb.Parameters.Add(new SqlParameter("Id", Id));
                    using (SqlDataReader reader = CheckLocalDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            LocalProductExists = true;
                            AppLog.Info("CREATE PRODUCT - Product already exists in local database!");
                        }
                        else
                        {
                            LocalProductExists = false;
                            AppLog.Info("CREATE PRODUCT - Product does not exist in local database. Creating product on local database");
                        }
                    }
                    if (!LocalProductExists)
                    {
                        SqlCommand CreateProduct = new SqlCommand("INSERT INTO Products VALUES(@Id, @OrgId, @Name)", conn);
                        CreateProduct.Parameters.Add(new SqlParameter("Id", _Id));
                        CreateProduct.Parameters.Add(new SqlParameter("OrgId", _MyOrg.Id));
                        CreateProduct.Parameters.Add(new SqlParameter("Name", _Name));

                        CreateProduct.ExecuteNonQuery();

                        Uploaded = true;
                        AppLog.Info(String.Format("CREATE PRODUCT - Product {0} created on local database successfully",
                            Name));
                    }
                }
            }
            catch (SqlException e)
            {
                _ErrMsg = "Error while creating product on local database";
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

                        SqlCommand UpdateProduct = new SqlCommand("UPDATE t_Products SET Name = @Name WHERE " +
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

                    SqlCommand UpdateProduct = new SqlCommand("UPDATE Products SET Name = @Name WHERE Id = @Id;", conn);

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
                _ErrMsg = "Error while updating product on local database. Changes were not saved";
                AppLog.Error(_ErrMsg + ": " + e);
                return false;
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
                                _Name = reader[2].ToString().Trim();
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

                        SqlCommand DownloadProduct = new SqlCommand("SELECT * FROM t_Products WHERE Id = @Id;", conn);
                        DownloadProduct.Parameters.Add(new SqlParameter("Id", _Id));

                        DownloadProduct.ExecuteNonQuery();

                        using (SqlDataReader reader = DownloadProduct.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _MyOrg = new Organisation(new Guid(reader[1].ToString()));
                                _Name = reader[2].ToString().Trim();
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
                //Finally, check if product exists in the local database. If not, ADD THEM!!! If so, UPDATE THEM!!!
                AppLog.Info("GET PRODUCT - Checking whether product exists in local database");

                bool ExistsOnLocalDb;

                using (SqlConnection conn = new SqlConnection(Data.LocalConnStr))
                {
                    AppLog.Info("GET PRODUCT - Attempting to open connection to local database...");
                    conn.Open();
                    AppLog.Info("GET PRODUCT - Connection to local database opened successfully");



                    SqlCommand CheckLocalDb = new SqlCommand("SELECT * FROM Products WHERE Id = @Id;", conn);
                    CheckLocalDb.Parameters.Add(new SqlParameter("Id", Id));

                    using (SqlDataReader reader = CheckLocalDb.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ExistsOnLocalDb = true;
                            AppLog.Info("GET PRODUCT - Product already exists in the local database!");
                        }
                        else
                        {
                            ExistsOnLocalDb = false;
                        }
                    }
                }
                if (ExistsOnLocalDb)
                {
                    if (Update())
                    {
                        AppLog.Info("GET PRODUCT - Updated product on local db successfully");
                    }
                    else
                    {
                        AppLog.Info("GET PRODUCT - Failed to update product: " + _ErrMsg);
                        return false;
                    }
                }
                else
                {
                    if (Create())
                    {
                        AppLog.Info("GET PRODUCT - Created product on local db successfully");
                    }
                    else
                    {
                        AppLog.Info("GET PRODUCT - Failed to create product: " + _ErrMsg);
                        return false;
                    }
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
