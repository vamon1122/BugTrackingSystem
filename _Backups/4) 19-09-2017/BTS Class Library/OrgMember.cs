using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS_Class_Library
{
    public partial class Organisation
    {
        public class OrgMember
        {
            private Organisation _MyOrg;
            private User _MyUser;
            private DateTime _DateTimeJoined;
            private int _AccessLevel;

            public Organisation MyOrg { get { return _MyOrg; } }
            public User MyUser { get { return _MyUser; } }
            public DateTime DateTimeJoined { get { return DateTimeJoined; } }
            public int AccessLevel { get { return _AccessLevel; }
                set { _AccessLevel = value; OfflineDbUpdate(); OnlineDbUpdate(); } }

            internal OrgMember(User pUser, Organisation pOrg)
            {

            }

            private bool Create()
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
