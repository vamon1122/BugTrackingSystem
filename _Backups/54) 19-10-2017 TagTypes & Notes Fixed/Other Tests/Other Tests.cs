using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTS_Class_Library;
using System.Data.SqlClient;

namespace Other_Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            int Downloaded = 0;
            using(SqlConnection conn = new SqlConnection(Data.OnlineConnStr))
            {
                conn.Open();
                SqlCommand FindAllBugs = new SqlCommand("SELECT Id FROM t_BTS_Bugs;",conn);
                
                using(SqlDataReader reader = FindAllBugs.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        
                        new Bug(new Guid(reader[0].ToString())).Get();
                        Downloaded++;
                        Console.Write("\rDownloaded {0} bug(s)", Downloaded);
                    }
                }
            }
            Console.WriteLine("Fin");
            Console.ReadLine();
        }
    }
}
