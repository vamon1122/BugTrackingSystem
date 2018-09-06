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

            if (MyUser.Create()) { Console.WriteLine("User created successfully! {0}", MyUser.Id); } else { Data.UserFriendlyError(MyUser.ErrMsg); }

            User MyDownloadedUser = new User(MyUser.Id);

            if (MyDownloadedUser.Get()) { Console.WriteLine("User downloaded successfully!"); } else { Data.UserFriendlyError(MyDownloadedUser.ErrMsg); }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("---------- Downloaded User's Details ----------");
            Console.ResetColor();



            Console.Write("Downloaded user's FName = \""); DispName(MyDownloadedUser.FName);
            Console.Write("\" EXPECTED: \""); DispName(MyUser.FName); Console.WriteLine("\"");

            Console.Write("Downloaded user's SName = \""); DispName(MyDownloadedUser.SName);
            Console.Write("\" EXPECTED: \""); DispName(MyUser.SName); Console.WriteLine("\"");

            Console.Write("Downloaded user's Username = \""); DispName(MyDownloadedUser.Username);
            Console.Write("\" EXPECTED: \""); DispName(MyUser.Username); Console.WriteLine("\"");

            Console.Write("Downloaded user's JobTitle = \""); DispName(MyDownloadedUser.JobTitle);
            Console.Write("\" EXPECTED: \""); DispName(MyUser.JobTitle); Console.WriteLine("\"");

            Console.Write("Downloaded user's EMail = \""); DispName(MyDownloadedUser.EMail);
            Console.Write("\" EXPECTED: \""); DispName(MyUser.EMail); Console.WriteLine("\"");

            Console.Write("Downloaded user's Password = \""); DispName(MyDownloadedUser.Password);
            Console.Write("\" EXPECTED: \""); DispName(MyUser.Password); Console.WriteLine("\"");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("-----------------------------------------------");
            Console.ResetColor();
            Console.WriteLine();

            MyUser.FName = "Joe";

            if (MyUser.Update()) { Console.WriteLine("User updated successfully!"); } else { Data.UserFriendlyError(MyUser.ErrMsg); }         

            

            ////////// This section tests the functionality of the Organisation class in online mode //////////

            Organisation MyOrg = new Organisation();
            MyOrg.Name = "Mega Corp";

            if (MyOrg.Create()) { Console.WriteLine("Organisation created successfully! " + MyOrg.Id); } else { Data.UserFriendlyError(MyOrg.ErrMsg); }

            Organisation DownloadedOrg = new Organisation(MyOrg.Id);
            if (DownloadedOrg.Get()) { Console.WriteLine("Organisation downloaded successfully!"); } else { Data.UserFriendlyError(DownloadedOrg.ErrMsg); }


            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("---------- Downloaded Organisation's Details ----------");
            Console.ResetColor();



            Console.Write("Downloaded organisation's Name = \""); DispName(DownloadedOrg.Name);
            Console.Write("\" EXPECTED: \""); DispName(MyOrg.Name); Console.WriteLine("\"");

            Console.Write("Downloaded organisation's DateTimeCreated = \""); DispName(DownloadedOrg.DateTimeCreated.ToString());
            Console.Write("\" EXPECTED: \""); DispName(MyOrg.DateTimeCreated.ToString()); Console.WriteLine("\"");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("-------------------------------------------------------");
            Console.ResetColor();
            Console.WriteLine();

            MyOrg.Name = "Mega Corp (Renamed)";

            if (MyOrg.Update()) { Console.WriteLine("Organisation updated successfully! " + MyOrg.Id); } else { Data.UserFriendlyError(MyOrg.ErrMsg); }

            ////////// This section tests the functionality of the OrgMember class in online mode //////////
            Organisation.OrgMember MyOrgMember = new Organisation.OrgMember(MyUser, MyOrg);
            MyOrgMember.AccessLevel = 1;

            if (MyOrgMember.Create()) { Console.WriteLine("Organisation member created successfully!"); } else { Data.UserFriendlyError(MyOrgMember.ErrMsg); }

            Organisation.OrgMember DownloadedOrgMember = new Organisation.OrgMember(MyUser, MyOrg);
            DownloadedOrgMember.Get();
            if (DownloadedOrgMember.Get()) { Console.WriteLine("Organisation member downloaded successfully!"); } else { Data.UserFriendlyError(DownloadedOrgMember.ErrMsg); }



            ////////// This section tests the functionality of various classes in offline mode //////////
            InfoDismiss("Testing Offline Mode");

            Data.OfflineMode = true;

            Info("OFFLINE MODE WAS TURNED ON");

            Console.WriteLine();

            User MyDownloadedUser2 = new User(MyUser.Id);
            if (MyDownloadedUser2.Get()) { Console.WriteLine("User retrieved from local database successfully!"); } else { Data.UserFriendlyError(MyDownloadedUser2.ErrMsg); }

            Organisation MyDownloadedOrg = new Organisation(MyOrg.Id);
            if (MyDownloadedOrg.Get()) { Console.WriteLine("Organisation retrieved from local database successfully!"); } else { Data.UserFriendlyError(MyDownloadedOrg.ErrMsg); }
            Console.WriteLine();

            Data.OfflineMode = false;

            Info("OFFLINE MODE WAS TURNED OFF");


            InfoDismiss("Testing Delete Functions");

            if (MyUser.Delete()) { Console.WriteLine("User deleted successfully!"); } else { Data.UserFriendlyError(MyUser.ErrMsg); }

            if (MyOrg.Delete()) { Console.WriteLine("Organisation deleted successfully!"); } else { Data.UserFriendlyError(MyOrg.ErrMsg); }

            InfoDismiss("Attempting to download some records which don't exist");

            if (MyUser.Get()) { Console.WriteLine("User was somehow downloaded successfully..."); } else { Data.UserFriendlyError(MyUser.ErrMsg); }

            if (MyOrg.Get()) { Console.WriteLine("Organisation was somehow downloaded successfully..."); } else { Data.UserFriendlyError(MyOrg.ErrMsg); }



            InfoDismiss("End of test");
            
        }
    }
}
