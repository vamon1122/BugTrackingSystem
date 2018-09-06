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

            OrgMember MeMember = TwoEskimos.NewOrgMember(Me);
            MeMember.Create();

            Organisation Google = new Organisation();
            Google.Name = "Google";
            Google.Create();

            OrgMember MeMemberTwo = Google.NewOrgMember(Me);
            MeMemberTwo.Create();

            Console.WriteLine("Success!");
        }
    }
}
