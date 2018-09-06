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
            Organisation NewOrg = new Organisation();
            NewOrg.Name = "Stress Test Org";

            if (NewOrg.Create())
            {
                Console.WriteLine("Created organisation");
            }
            else
            {
                Console.WriteLine("Failed to create organisation: " + NewOrg.ErrMsg);
                Console.ReadLine();
            }

            User Me = new User(new Guid("78d3c158-01a9-49ff-82c8-d823ba27d214"));
            if (Me.Get())
            {
                Console.WriteLine("Got your user");
            }
            else
            {
                Console.WriteLine("Failed to get your user");
                Console.ReadLine();
            }

            Data.ActiveUser = Me;

            OrgMember MeMember = NewOrg.NewOrgMember(Me);
            if (!MeMember.Create())
            {
                Console.Write("Failed to make org member");
                Console.ReadLine();
            }

            Product MyProduct = NewOrg.NewProduct();
            MyProduct.Name = "Stress Test Product";

            if (MyProduct.Create())
            {
                Console.WriteLine("Created your product");
            }
            else
            {
                Console.WriteLine("Failed to create your product");
                Console.ReadLine();
            }

            for(int i = 0; i < 1000; i++)
            {
                Bug TempBug = MyProduct.NewBug();
                TempBug.Title = "Auto Generated Bug #" + i;
                TempBug.Description = "Lorem ipsum dolor sit amet, prima pertinacia no has. " +
                    "Ad sea velit audiam phaedrum. Te efficiendi omittantur consequuntur cum, " +
                    "usu timeam imperdiet liberavisse cu. Cu idque oporteat scribentur vel, " +
                    "bonorum probatus cu vim, quando liberavisse ut duo. Audiam ornatus sententiae " +
                    "eos te.His id nobis veniam, eos eu eripuit sapientem argumentum, nec no erant " +
                    "legimus postulant. Wisi admodum deseruisse eu vim.Ridens timeam mea ad. Cu sit " +
                    "populo oblique, ut fugit dicunt indoctum usu.";

                TempBug.Severity = 1;

                if (TempBug.Create())
                {
                    Console.Write("\rBug {0}/1000 created", i + 1);
                }
                else
                {
                    Console.Write("Failed while creating bug: " + TempBug.ErrMsg);
                }
            }
            Console.WriteLine();
            Console.WriteLine("Done");
            Console.ReadLine();






        }
    }
}
