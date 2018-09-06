using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS_Class_Library
{
    public partial class Bug
    {
        class Note
        {
            private Guid _Id;
            private Guid _BugId;
            private Guid _UserId;
            private DateTime _DateTimeCreated;
            private string _Note;

            public Note(Guid pId)
            {

            }

            public Note(User pUser, string pNoteContent)
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
