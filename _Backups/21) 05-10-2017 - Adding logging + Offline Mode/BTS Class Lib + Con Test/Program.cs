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
            void DispName(string p)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(p);
                Console.ResetColor();
            }

            void InfoDismiss(string p)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(p);
                Console.ResetColor();
                Console.WriteLine(" (Press enter to dismiss)");
                Console.ReadLine();
            }

            void Info(string p)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(p);
                Console.ResetColor();
            }

            InfoDismiss("Testing Online Mode");
            Data.OfflineMode = false;
            Info("OFFLINE MODE WAS TURNED OFF");
            Console.WriteLine();

            ////////// This section tests the functionality of the User class in online mode //////////

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

            ////////// This section tests the functionality of the Organisation class in online mode //////////

            Organisation MyOrg = new Organisation();
            MyOrg.Name = "Mega Corp";

            if (MyOrg.Create()) { Console.WriteLine("Organisation created successfully! " + MyOrg.Id); } else { Data.UserFriendlyError(MyOrg.ErrMsg); }

            MyOrg.Name = "New Name!!!";

            if (MyOrg.Update()) { Console.WriteLine("Organisation updated successfully! " + MyOrg.Id); } else { Data.UserFriendlyError(MyOrg.ErrMsg); }

            InfoDismiss("Testing Offline Mode");

            Data.OfflineMode = true;

            Info("OFFLINE MODE WAS TURNED ON");

            Console.WriteLine();

            User MyDownloadedUser2 = new User(MyUserGuid);
            if (MyDownloadedUser2.Get()) { Console.WriteLine("User retrieved from local database successfully!"); } else { Data.UserFriendlyError(MyDownloadedUser2.ErrMsg); }

            Organisation MyDownloadedOrg = new Organisation(MyUserGuid);
            if (MyDownloadedOrg.Get()) { Console.WriteLine("Organisation retrieved from local database successfully!"); } else { Data.UserFriendlyError(MyDownloadedOrg.ErrMsg); }
            Console.WriteLine();

            Data.OfflineMode = false;

            Info("OFFLINE MODE WAS TURNED OFF");

            Console.WriteLine();

            if (MyUser.Delete()) { Console.WriteLine("User deleted successfully!"); } else { Data.UserFriendlyError(MyUser.ErrMsg); }

            if (MyOrg.Delete()) { Console.WriteLine("Organisation deleted successfully!"); } else { Data.UserFriendlyError(MyOrg.ErrMsg); }

            InfoDismiss("Attempting to download some records which don't exist");

            if (MyUser.Get()) { Console.WriteLine("User was somehow downloaded successfully..."); } else { Data.UserFriendlyError(MyUser.ErrMsg); }

            if (MyOrg.Get()) { Console.WriteLine("Organisation was somehow downloaded successfully..."); } else { Data.UserFriendlyError(MyOrg.ErrMsg); }



            InfoDismiss("End of test");
            
        }
    }
}
