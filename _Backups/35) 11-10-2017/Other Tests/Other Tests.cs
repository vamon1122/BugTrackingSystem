using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTS_Class_Library;

namespace Other_Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            User MyUser = new User();

            Bug MyBug = new Bug(MyUser);



            for (int i = 0; i < 10; i++)
            {
                Note MyNote = MyBug.CreateNote(MyUser);
                Assignee MyAssignee = MyBug.CreateAssignee();
                
            }
            Console.ReadLine();
        }
    }
}
