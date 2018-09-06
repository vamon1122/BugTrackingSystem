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
            for (int i = 0; i < 1000; i++)
            {
                /*
                User MyUser = new User();
                MyUser.FName = "Ben";
                MyUser.SName = "Barton";
                MyUser.Username = "vamon1122";
                MyUser.JobTitle = "Apprentice Software Developer";
                MyUser.EMail = "ben@2eskimos.com";
                MyUser.Password = "P4ssword";

                if (MyUser.Create())
                {
                    Console.Write("\r{0}/1000", i+1);
                }
                else
                {
                    Data.UserFriendlyError(MyUser.ErrMsg);
                }
                */
                User MyUser = new User();
                MyUser.FName = "Ben";
                MyUser.SName = "Barton";
                MyUser.Username = "vamon1122";
                MyUser.JobTitle = "Apprentice Software Developer";
                MyUser.EMail = "ben@2eskimos.com";
                MyUser.Password = "P4ssword";

                if (MyUser.Create())
                {
                    Console.Write("\r{0}/1000", i + 1);
                }
                else
                {
                    Data.UserFriendlyError(MyUser.ErrMsg);
                }


            }
            Console.ReadLine();
        }
    }
}
