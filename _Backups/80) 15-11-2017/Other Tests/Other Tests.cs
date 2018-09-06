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
            Bug MyBug = new Bug(new Guid("b3e7eb2c-52bd-4acd-ab0f-e05258e09737"));
            if (MyBug.Get())
            {
                Console.WriteLine("Got Bug");
            }
            else
            {
                Console.WriteLine("Failed to get bug");
                Console.ReadLine();
            }

            User MyUser = new User(new Guid("8894ceb0-060f-45b5-a6d8-c165e9827948"));
            if(MyUser.Get())
            {
                Console.WriteLine("Got user");
            }
            else
            {
                Console.WriteLine("Failed to get user");
                Console.ReadLine();
            }

            Data.ActiveUser = MyUser;

            Note MyNote = MyBug.CreateNote();
            MyNote.Title = "My first note!";
            MyNote.Body = "This note mas made programatically!";

            if(MyNote.Create())
            {
                Console.WriteLine("Created note");
            }
            else
            {
                Console.WriteLine("Failed to create note");
                Console.ReadLine();
            }

            Console.ReadLine();

        }
    }
}
