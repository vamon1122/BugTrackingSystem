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

            Guid MyUserGuid = MyUser.Id;

            if (MyUser.Create()) { Console.WriteLine("User created successfully! {0}", MyUserGuid); } else { Data.UserFriendlyError(MyUser.ErrMsg); }

            MyUser.FName = "Joe";

            if (MyUser.Update()) { Console.WriteLine("User updated successfully!"); } else { Data.UserFriendlyError(MyUser.ErrMsg); }

            

            

            User MyDownloadedUser = new User(MyUserGuid);

            if (MyDownloadedUser.Get()) { Console.WriteLine("User downloaded successfully!"); } else { Data.UserFriendlyError(MyDownloadedUser.ErrMsg); }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("---------- Downloaded User's Details ----------");
            Console.ResetColor();

            void DispName(string p)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(p);
                Console.ResetColor();
            }

            Console.Write("Downloaded user FName = \"");  DispName(MyDownloadedUser.FName);
            Console.Write("\" EXPECTED: \""); DispName("Joe"); Console.WriteLine("\"");

            Console.Write("Downloaded user SName = \""); DispName(MyDownloadedUser.SName);
            Console.Write("\" EXPECTED: \"");  DispName("Bloggs"); Console.WriteLine("\"");

            Console.Write("Downloaded user Username = \""); DispName(MyDownloadedUser.Username);
            Console.Write("\" EXPECTED: \""); DispName("Joe B"); Console.WriteLine("\"");

            Console.Write("Downloaded user JobTitle = \""); DispName(MyDownloadedUser.JobTitle);
            Console.Write("\" EXPECTED: \""); DispName("Some Job"); Console.WriteLine("\"");

            Console.Write("Downloaded user EMail = \""); DispName(MyDownloadedUser.EMail);
            Console.Write("\" EXPECTED: \""); DispName("joebloggs@gmail.com"); Console.WriteLine("\"");

            Console.Write("Downloaded user Password = \""); DispName(MyDownloadedUser.Password);
            Console.Write("\" EXPECTED: \""); DispName("P4ssword"); Console.WriteLine("\"");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("-----------------------------------------------");
            Console.ResetColor();
            Console.WriteLine();

            if (MyUser.Delete()) { Console.WriteLine("User deleted successfully!"); } else { Data.UserFriendlyError(MyUser.ErrMsg); }

            User MyDownloadedUser2 = new User(MyUserGuid);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Attempting to download a record which doesn't exist. An error SHOULD be produced" +
                " (Press enter to dismiss)");
            Console.ResetColor();
            Console.ReadLine();

            if (MyDownloadedUser2.Get()) { Console.WriteLine("User downloaded successfully!"); } else { Data.UserFriendlyError(MyDownloadedUser2.ErrMsg); }


            


            Console.WriteLine();

            Organisation MyOrg = new Organisation();
            MyOrg.Name = "Mega Corp";

            

            if (MyOrg.Create()) { Console.WriteLine("Organisation created successfully!"); } else { Data.UserFriendlyError(MyOrg.ErrMsg); }

            Console.ReadLine();
        }
    }
}
