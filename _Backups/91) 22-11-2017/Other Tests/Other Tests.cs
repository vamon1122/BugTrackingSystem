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
            Organisation TwoEskimos = new Organisation(new Guid("3fab6188-b010-45ed-bbec-ee6976d2fec9"));

            if (TwoEskimos.Get())
            {
                Console.WriteLine("Got organisation");
            }
            else
            {
                Console.WriteLine("Failed to get organisation: " + TwoEskimos.ErrMsg);
                Console.ReadLine();
            }

            User MyUser = new User();
            MyUser.FName = "Caethan";
            MyUser.SName = "Wilde";
            MyUser.EMail = "caethan@2eskimos.com";
            MyUser.Username = "CaethanWilde";
            MyUser.Password = "password";

            if (MyUser.Create())
            {
                Console.WriteLine("User {0} created successfully", MyUser.FullName);
            }
            else
            {
                Console.WriteLine("Error creating user: " + MyUser.ErrMsg);
                Console.ReadLine();
            }

            OrgMember MyOrgMember = TwoEskimos.NewOrgMember(MyUser);

            if (MyOrgMember.Create())
            {
                Console.WriteLine("Org member {0} created successfully", MyUser.FullName);
            }
            else
            {
                Console.WriteLine("Error creating org member: " + MyOrgMember.ErrMsg);
                Console.ReadLine();
            }

            Console.ReadLine();

        }
    }
}
