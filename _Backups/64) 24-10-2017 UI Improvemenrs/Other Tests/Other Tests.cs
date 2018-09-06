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
            string Username = "benbartontablet";
            string Password = "BBTbbt1704";

            string Org1 = "2Eskimos";
                string Org1_Prod1 = "2BAP";
                string Org1_Prod2 = "2ERA";
                string Org1_Prod3 = "Collins RA";

            string Org2 = "Google";
                string Org2_Prod1 = "Google Search";
                string Org2_Prod2 = "Google Translate";
                string Org2_Prod3 = "Google Earth";

            User Me = new User();
            Me.Username = Username;
            Me.Password = Password;
            Me.LogIn();

            /*
            Organisation TwoEskimos = new Organisation();
            TwoEskimos.Name = Org1;
            TwoEskimos.Create();

            Product TwoBAP = TwoEskimos.NewProduct();
            TwoBAP.Name = Org1_Prod1;
            if (!TwoBAP.Create())
            {
                Console.WriteLine(TwoBAP.ErrMsg);
            }

            Product TwoERA = TwoEskimos.NewProduct();
            TwoERA.Name = Org1_Prod2;
            TwoERA.Create();

            Product CollinsRA = TwoEskimos.NewProduct();
            CollinsRA.Name = Org1_Prod3;
            CollinsRA.Create();

            OrgMember MeMember = TwoEskimos.NewOrgMember(Me);
            MeMember.Create();

            Organisation Google = new Organisation();
            Google.Name = Org2;
            Google.Create();

            Product GoogleSearch = Google.NewProduct();
            GoogleSearch.Name = Org2_Prod1;
            GoogleSearch.Create();

            Product GoogleTranslate = Google.NewProduct();
            GoogleTranslate.Name = Org2_Prod2;
            GoogleTranslate.Create();

            Product GoogleEarth = Google.NewProduct();
            GoogleEarth.Name = Org2_Prod3;
            GoogleEarth.Create();

            OrgMember MeMemberTwo = Google.NewOrgMember(Me);
            MeMemberTwo.Create();

            Console.WriteLine("Success!");
            Console.ReadLine();*/

            Product MyProduct = new Product(new Guid("2499fd22-0453-4b5a-9c57-7eb0f7fa3410"));
            MyProduct.Get();

            Bug MyBug = MyProduct.NewBug();
            MyBug.Title = "earth-issues - issue #1809";
            MyBug.Description = "I reported this problem in french discussion long time ago, maybe five years, and it was confirmed. But the problem is still here. When there are placemarks in the Temporary places folder, when exiting Google Earth, the window about saving items appears. But in French language, there are two \"Annuler\" buttons, the middle one (Discard) and the right one (Cancel)."; 
                
            MyBug.Severity = 3;

            if (!MyBug.Create())
            {
                Console.WriteLine(MyBug.ErrMsg);
                Console.ReadLine();
            }

            Console.WriteLine("Success!");
            Console.ReadLine();
        }
    }
}
