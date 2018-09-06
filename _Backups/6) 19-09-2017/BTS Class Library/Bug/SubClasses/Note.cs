using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS_Class_Library
{
    public partial class Bug
    {
        public class Note
        {
            private Guid _Id;
            private Bug _MyBug;
            private User _MyUser;
            private DateTime _DateTimeCreated;
            private string _MyNote;

            internal Note(Guid pId)
            {
                _Id = pId;
                Get();
            }

            internal Note(User pUser)
            {
                _Id = new Guid();
                _DateTimeCreated = DateTime.Now;
                _MyUser = pUser;
                Create();
            }

            public Guid Id { get { return _Id; } }
            public Bug MyBug { get { return _MyBug; } }
            public User MyUser { get { return _MyUser; } }
            public DateTime DateTimeCreated { get { return _DateTimeCreated; } }
            public string MyNote
            {
                get { return MyNote; }

                set
                {
                    if (value.Length < 1001){ 
                        _MyNote = value;
                    }
                    else
                    {
                        throw new Exception("Note exceeds 1000 characters");
                    }
                }
            }

            public bool Create()
            {
                bool _Success = false;

                return _Success;
            }

            public bool Update()
            {
                bool _Success = false;

                return _Success;
            }

            private bool Get()
            {
                bool _Success = false;

                return _Success;
            }

            public bool Delete()
            {
                bool _Success = false;

                return _Success;
            }
        }
    }
    
}
