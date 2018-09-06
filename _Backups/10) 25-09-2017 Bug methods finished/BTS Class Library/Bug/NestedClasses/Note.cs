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
            private DateTime _DateTimeUpdated;
            private string _MyNote;
            private bool _Uploaded;

            internal Note(Guid pId)
            {
                _Id = pId;
                Get();
            }

            internal Note(Bug pBug, User pUser)
            {
                _Id = Guid.NewGuid();
                _DateTimeCreated = DateTime.Now;
                _MyBug = pBug;
                _MyUser = pUser;
                Create();
            }

            public Guid Id { get { return _Id; } }
            public Bug MyBug { get { return _MyBug; } }
            public User MyUser { get { return _MyUser; } }
            public DateTime DateTimeCreated { get { return _DateTimeCreated; } }
            public DateTime DateTimeUpdated { get { return _DateTimeUpdated; } }
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
            public bool Uploaded { get { return _Uploaded; } }

            public bool Create()
            {
                bool _Success = false;

                MyBug._ErrorMsg = "TEST ERROR MSG";

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
