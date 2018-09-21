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
            Console.WriteLine("Getting user from db...");
            Data.ActiveUser = new User(Guid.Parse("2b392eb1-3458-4f04-82f8-3b601ac5f271"));
            Data.ActiveUser.Get();
            Console.WriteLine("Successfully got user from db!");

            Console.WriteLine("Creating product on db...");
            Product MyProduct = new Product(Guid.NewGuid(), Guid.Parse("2e032dbe-943c-4cd8-b677-8ebf763ccca0"), "Lots Of Bugs");
            MyProduct.Create();
            Console.WriteLine("Successfully created product on db!");

            for (int i = 0; i < 100; i++)
            {
                Console.Write("\rCreating bug {0}/100 on db...", i+1);
                Bug MyBug = MyProduct.NewBug();
                MyBug.Title = String.Format("Bug {0} title", i);
                MyBug.Description = String.Format("Bug {0} description", i);
                MyBug.Severity = 2;
                MyBug.Create();
            }
            Console.WriteLine();
            Console.WriteLine("Successfully created bugs on db!");
            Console.ReadLine();
        }
    }
}
