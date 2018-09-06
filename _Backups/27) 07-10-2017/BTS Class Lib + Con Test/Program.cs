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
            MyUser.JobTitle = "Apprentice Software Developer";
            MyUser.EMail = "joebloggs@gmail.com";
            MyUser.Password = "P4ssword";

            if (MyUser.Create()) {
                Console.WriteLine("User created successfully! {0}", MyUser.Id);
            }
            else {
                Data.UserFriendlyError(MyUser.ErrMsg);
            }

            User MyDownloadedUser = new User(MyUser.Id);

            if (MyDownloadedUser.Get()) {
                Console.WriteLine("User downloaded successfully!");
            }
            else {
                Data.UserFriendlyError(MyDownloadedUser.ErrMsg);
            }

            void Match(string p1, string p2)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("MATCH ");
                DispName(p1);
                Console.Write(" == ");
                DispName(p2);
                Console.WriteLine();
            }

            void NoMatch(string p1, string p2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("NO MATCH ");
                DispName(p1);
                Console.Write(" != ");
                DispName(p2);
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("---------- Validating Downloaded User's Details ----------");
            Console.ResetColor();



            Console.Write("FName "); if (MyUser.FName == MyDownloadedUser.FName) {
                Match(MyUser.FName, MyDownloadedUser.FName);
            }
            else {
                NoMatch(MyUser.FName, MyDownloadedUser.FName);
            }

            Console.Write("SName "); if (MyUser.SName == MyDownloadedUser.SName)
            {
                Match(MyUser.SName, MyDownloadedUser.SName);
            }
            else
            {
                NoMatch(MyUser.SName, MyDownloadedUser.SName);
            }

            Console.Write("Username "); if (MyUser.Username == MyDownloadedUser.Username)
            {
                Match(MyUser.Username, MyDownloadedUser.Username);
            }
            else
            {
                NoMatch(MyUser.Username, MyDownloadedUser.Username);
            }

            Console.Write("JobTitle "); if (MyUser.JobTitle == MyDownloadedUser.JobTitle)
            {
                Match(MyUser.JobTitle, MyDownloadedUser.JobTitle);
            }
            else
            {
                NoMatch(MyUser.JobTitle, MyDownloadedUser.JobTitle);
            }

            Console.Write("EMail "); if (MyUser.EMail == MyDownloadedUser.EMail)
            {
                Match(MyUser.EMail, MyDownloadedUser.EMail);
            }
            else
            {
                NoMatch(MyUser.EMail, MyDownloadedUser.EMail);
            }

            Console.Write("Password "); if (MyUser.Password == MyDownloadedUser.Password)
            {
                Match(MyUser.Password, MyDownloadedUser.Password);
            }
            else
            {
                NoMatch(MyUser.Password, MyDownloadedUser.Password);
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("----------------------------------------------------------");
            Console.ResetColor();
            Console.WriteLine();

            MyUser.FName = "Joe";

            if (MyUser.Update()) {
                Console.WriteLine("User updated successfully!");
            }
            else {
                Data.UserFriendlyError(MyUser.ErrMsg);
            }         

            

            ////////// This section tests the functionality of the Organisation class in online mode //////////

            Organisation MyOrg = new Organisation();
            MyOrg.Name = "2Eskimos";

            if (MyOrg.Create()) {
                Console.WriteLine("Organisation created successfully! " + MyOrg.Id);
            }
            else {
                Data.UserFriendlyError(MyOrg.ErrMsg);
            }

            Organisation DownloadedOrg = new Organisation(MyOrg.Id);
            if (DownloadedOrg.Get()) {
                Console.WriteLine("Organisation downloaded successfully!");
            }
            else {
                Data.UserFriendlyError(DownloadedOrg.ErrMsg);
            }


            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("---------- Validating Downloaded Organisation's Details ----------");
            Console.ResetColor();

            Console.Write("Name "); if (MyOrg.Name == DownloadedOrg.Name)
            {
                Match(MyOrg.Name, DownloadedOrg.Name);
            }
            else
            {
                NoMatch(MyOrg.Name, DownloadedOrg.Name);
            }

            Console.Write("DateTimeCreated ");
            if (MyOrg.DateTimeCreated.ToString() == DownloadedOrg.DateTimeCreated.ToString())
            {
                Match(MyOrg.DateTimeCreated.ToString(), DownloadedOrg.DateTimeCreated.ToString());
            }
            else
            {
                NoMatch(Convert.ToString(MyOrg.DateTimeCreated), Convert.ToString(DownloadedOrg.DateTimeCreated));
            }

            Console.Write("Downloaded organisation's Name = \""); DispName(DownloadedOrg.Name);
            Console.Write("\" EXPECTED: \""); DispName(MyOrg.Name); Console.WriteLine("\"");

            Console.Write("Downloaded organisation's DateTimeCreated = \""); DispName(DownloadedOrg.DateTimeCreated.ToString());
            Console.Write("\" EXPECTED: \""); DispName(MyOrg.DateTimeCreated.ToString()); Console.WriteLine("\"");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("------------------------------------------------------------------");
            Console.ResetColor();
            Console.WriteLine();

            MyOrg.Name = "2Simple";

            if (MyOrg.Update()) {
                Console.WriteLine("Organisation updated successfully! " + MyOrg.Id);
            }
            else {
                Data.UserFriendlyError(MyOrg.ErrMsg);
            }

            ////////// This section tests the functionality of the OrgMember class in online mode //////////
            Organisation.OrgMember MyOrgMember = new Organisation.OrgMember(MyUser, MyOrg);
            MyOrgMember.AccessLevel = 1;

            if (MyOrgMember.Create()) {
                Console.WriteLine("Organisation member created successfully!");
            }
            else {
                Data.UserFriendlyError(MyOrgMember.ErrMsg);
            }

            Organisation.OrgMember DownloadedOrgMember = new Organisation.OrgMember(MyUser, MyOrg);
            if (DownloadedOrgMember.Get()) {
                Console.WriteLine("Organisation member downloaded successfully!");
            }
            else {
                Data.UserFriendlyError(DownloadedOrgMember.ErrMsg);
            }


            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("---------- Validating Downloaded Organisation Member's Details ----------");
            Console.ResetColor();

            Console.Write("DateTimeJoined ");
            if (MyOrgMember.DateTimeJoined.ToString() == DownloadedOrgMember.DateTimeJoined.ToString())
            {
                Match(MyOrgMember.DateTimeJoined.ToString(), DownloadedOrgMember.DateTimeJoined.ToString());
            }
            else
            {
                NoMatch(MyOrgMember.DateTimeJoined.ToString(), DownloadedOrgMember.DateTimeJoined.ToString());
            }

            Console.Write("AccessLevel "); if (MyOrgMember.AccessLevel.ToString() == DownloadedOrgMember.AccessLevel.ToString())
            {
                Match(MyOrgMember.AccessLevel.ToString(), DownloadedOrgMember.AccessLevel.ToString());
            }
            else
            {
                NoMatch(MyOrgMember.AccessLevel.ToString(), DownloadedOrgMember.AccessLevel.ToString());
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("-------------------------------------------------------------------------");
            Console.ResetColor();

            ////////// This section tests the functionality of various classes in offline mode //////////
            InfoDismiss("Testing Offline Mode");

            Data.OfflineMode = true;

            Info("OFFLINE MODE WAS TURNED ON");

            Console.WriteLine();

            User MyDownloadedUser2 = new User(MyUser.Id);
            if (MyDownloadedUser2.Get()) {
                Console.WriteLine("User retrieved from local database successfully!");
            }
            else {
                Data.UserFriendlyError(MyDownloadedUser2.ErrMsg);
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("---------- Validating Downloaded User's Details ----------");
            Console.ResetColor();



            Console.Write("FName "); if (MyUser.FName == MyDownloadedUser2.FName)
            {
                Match(MyUser.FName, MyDownloadedUser2.FName);
            }
            else
            {
                NoMatch(MyUser.FName, MyDownloadedUser2.FName);
            }

            Console.Write("SName "); if (MyUser.SName == MyDownloadedUser2.SName)
            {
                Match(MyUser.SName, MyDownloadedUser2.SName);
            }
            else
            {
                NoMatch(MyUser.SName, MyDownloadedUser2.SName);
            }

            Console.Write("Username "); if (MyUser.Username == MyDownloadedUser2.Username)
            {
                Match(MyUser.Username, MyDownloadedUser2.Username);
            }
            else
            {
                NoMatch(MyUser.Username, MyDownloadedUser2.Username);
            }

            Console.Write("JobTitle "); if (MyUser.JobTitle == MyDownloadedUser2.JobTitle)
            {
                Match(MyUser.JobTitle, MyDownloadedUser2.JobTitle);
            }
            else
            {
                NoMatch(MyUser.JobTitle, MyDownloadedUser2.JobTitle);
            }

            Console.Write("EMail "); if (MyUser.EMail == MyDownloadedUser2.EMail)
            {
                Match(MyUser.EMail, MyDownloadedUser2.EMail);
            }
            else
            {
                NoMatch(MyUser.EMail, MyDownloadedUser2.EMail);
            }

            Console.Write("Password "); if (MyUser.Password == MyDownloadedUser2.Password)
            {
                Match(MyUser.Password, MyDownloadedUser2.Password);
            }
            else
            {
                NoMatch(MyUser.Password, MyDownloadedUser2.Password);
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("----------------------------------------------------------");
            Console.ResetColor();
            Console.WriteLine();

            Organisation DownloadedOrg2 = new Organisation(MyOrg.Id);
            if (DownloadedOrg2.Get()) {
                Console.WriteLine("Organisation retrieved from local database successfully!");
            }
            else {
                Data.UserFriendlyError(DownloadedOrg2.ErrMsg);
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("---------- Validating Downloaded Organisation's Details ----------");
            Console.ResetColor();

            Console.Write("Name "); if (MyOrg.Name == DownloadedOrg2.Name)
            {
                Match(MyOrg.Name, DownloadedOrg2.Name);
            }
            else
            {
                NoMatch(MyOrg.Name, DownloadedOrg2.Name);
            }

            Console.Write("DateTimeCreated "); if (MyOrg.DateTimeCreated.ToString() == DownloadedOrg2.DateTimeCreated.ToString())
            {
                Match(MyOrg.DateTimeCreated.ToString(), DownloadedOrg2.DateTimeCreated.ToString());
            }
            else
            {
                NoMatch(Convert.ToString(MyOrg.DateTimeCreated), Convert.ToString(DownloadedOrg2.DateTimeCreated));
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("------------------------------------------------------------------");
            Console.ResetColor();
            Console.WriteLine();

            Organisation.OrgMember DownloadedOrgMember2 = new Organisation.OrgMember(MyOrgMember.MyUser, MyOrgMember.MyOrg);
            if (DownloadedOrgMember2.Get()) {
                Console.WriteLine("Organisation member downloaded successfully!");
            }
            else {
                Data.UserFriendlyError(DownloadedOrgMember2.ErrMsg);
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("---------- Validating Downloaded Organisation Member's Details ----------");
            Console.ResetColor();

            Console.Write("DateTimeJoined ");
            if (MyOrgMember.DateTimeJoined.ToString() == DownloadedOrgMember2.DateTimeJoined.ToString())
            {
                Match(MyOrgMember.DateTimeJoined.ToString(), DownloadedOrgMember2.DateTimeJoined.ToString());
            }
            else
            {
                NoMatch(MyOrgMember.DateTimeJoined.ToString(), DownloadedOrgMember2.DateTimeJoined.ToString());
            }

            Console.Write("AccessLevel "); if (MyOrgMember.AccessLevel.ToString() == DownloadedOrgMember2.AccessLevel.ToString())
            {
                Match(MyOrgMember.AccessLevel.ToString(), DownloadedOrgMember2.AccessLevel.ToString());
            }
            else
            {
                NoMatch(MyOrgMember.AccessLevel.ToString(), DownloadedOrgMember2.AccessLevel.ToString());
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("-------------------------------------------------------------------------");
            Console.ResetColor();
            Console.WriteLine();

            Data.OfflineMode = false;

            Info("OFFLINE MODE WAS TURNED OFF");


            InfoDismiss("Testing Delete Functions");

            if (MyUser.Delete()) {
                Console.WriteLine("User deleted successfully!");
            }
            else {
                Data.UserFriendlyError(MyUser.ErrMsg);
            }

            if (MyOrg.Delete()) {
                Console.WriteLine("Organisation deleted successfully!");
            }
            else {
                Data.UserFriendlyError(MyOrg.ErrMsg);
            }

            if (MyOrgMember.Delete()) {
                Console.WriteLine("Organisation member deleted successfully!");
            }
            else {
                Data.UserFriendlyError(MyOrgMember.ErrMsg);
            }

            InfoDismiss("Attempting to download some records which don't exist");

            if (MyUser.Get()) {
                Console.WriteLine("User was somehow downloaded successfully...");
            }
            else {
                Data.UserFriendlyError(MyUser.ErrMsg);
            }

            if (MyOrg.Get()) {
                Console.WriteLine("Organisation was somehow downloaded successfully...");
            }
            else {
                Data.UserFriendlyError(MyOrg.ErrMsg);
            }

            if (MyOrgMember.Get()) {
                Console.WriteLine("Organisation member was somehow downloaded successfully...");
            }
            else {
                Data.UserFriendlyError(MyOrgMember.ErrMsg);
            }

            InfoDismiss("End of test");
        }
    }
}
