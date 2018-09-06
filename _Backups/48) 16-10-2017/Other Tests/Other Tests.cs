using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTS_Class_Library;

namespace Other_Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            /*User MyUser = new User();
            MyUser.FName = "Peter";
            MyUser.SName = "Griffin";
            MyUser.EMail = "p.milnes@gmail.com";
            MyUser.Username = "pmilnes";
            MyUser.Password = "familyguy";

            Console.WriteLine(MyUser.Id);*/

            User MyUser = new User();
            MyUser.Username = "pmilnes";
            MyUser.Password = "familyguy";


            if (MyUser.LogIn())
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine(MyUser.ErrMsg);
            }

            Organisation MyOrg = new Organisation();
            MyOrg.Name = "Pawtucket Brewery";

            if (MyOrg.Create())
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine(MyOrg.ErrMsg);
            }

            OrgMember MyOrgMember = MyOrg.NewOrgMember(MyUser);

            if (MyOrgMember.Create())
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine(MyOrg.ErrMsg);
            }

            Console.WriteLine(MyUser.FullName + " works at " + MyUser.Organisations[0].Name);




            Console.ReadLine();
        }
    }
}
