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
                        OfflineDbUpdate();
                        OnlineDbUpdate();
                    }
                    else
                    {
                        throw new Exception("Note exceeds 1000 characters");
                    }
                }
            }

            internal Note(Guid pId)
            {

            }

            internal Note(User pUser, string pNoteContent)
            {

            }

            private bool Create()
            {
                bool _Success = false;

                return _Success;
            }

            private bool Update()
            {
                bool _Success = false;

                return _Success;
            }

            private bool Get()
            {
                bool _Success = false;

                return _Success;
            }

            private bool Delete()
            {
                bool _Success = false;

                return _Success;
            }

            private bool OnlineDbCreate()
            {
                bool _Success = false;

                return _Success;
            }

            private bool OnlineDbUpdate()
            {
                bool _Success = false;

                return _Success;
            }

            private bool OnlineDbDownload()
            {
                bool _Success = false;

                return _Success;
            }

            private bool OnlineDbDelete()
            {
                bool _Success = false;

                return _Success;
            }

            private bool OfflineDbCreate()
            {
                bool _Success = false;

                return _Success;
            }

            private bool OfflineDbUpdate()
            {
                bool _Success = false;

                return _Success;
            }

            private bool OfflineDbGet()
            {
                bool _Success = false;

                return _Success;
            }

            private bool OfflineDbDelete()
            {
                bool _Success = false;

                return _Success;
            }
        }
    }
    
}
