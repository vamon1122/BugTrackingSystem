using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTS_Class_Library;

namespace BTS_Class_Lib___Con_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            User MyUser = new User();
            MyUser.FName = "Joseph";
            MyUser.SName = "Bloggs";
            MyUser.Username = "Joe B";
            MyUser.JobTitle = "Some job";
            MyUser.EMail = "joebloggs@gmail.com";
            MyUser.Password = "P4ssword";

            if (MyUser.Create()) { Console.WriteLine("User created successfully!"); } else { Data.Error(MyUser.ErrMsg); }

            Console.WriteLine();

            Organisation MyOrg = new Organisation();
            MyOrg.Name = "Mega Corp";

            

            if (MyOrg.Create()) { Console.WriteLine("Organisation created successfully!"); } else { Data.Error(MyOrg.ErrMsg); }

            Console.ReadLine();
        }
    }
}
