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
            User Me = new User();
            Me.Username = "benbartontablet";
            Me.Password = "BBTbbt1704";
            Me.LogIn();

            Organisation TwoEskimos = new Organisation();
            TwoEskimos.Name = "2Eskimos";
            TwoEskimos.Create();

            Product TwoBAP = TwoEskimos.NewProduct();
            TwoBAP.Name = "2BAP";
            if (!TwoBAP.Create())
            {
                Console.WriteLine(TwoBAP.ErrMsg);
            }

            Product TwoERA = TwoEskimos.NewProduct();
            TwoERA.Name = "2ERA";
            TwoERA.Create();

            Product CollinsRA = TwoEskimos.NewProduct();
            CollinsRA.Name = "Collins RA";
            CollinsRA.Create();

            OrgMember MeMember = TwoEskimos.NewOrgMember(Me);
            MeMember.Create();

            Organisation Google = new Organisation();
            Google.Name = "Google";
            Google.Create();

            Product GoogleSearch = Google.NewProduct();
            GoogleSearch.Name = "Google Search";
            GoogleSearch.Create();

            Product GoogleTranslate = Google.NewProduct();
            GoogleTranslate.Name = "Google Translate";
            GoogleTranslate.Create();

            Product GoogleEarth = Google.NewProduct();
            GoogleEarth.Name = "Google Earth";
            GoogleEarth.Create();

            OrgMember MeMemberTwo = Google.NewOrgMember(Me);
            MeMemberTwo.Create();

            Console.WriteLine("Success!");
            Console.ReadLine();
        }
    }
}
