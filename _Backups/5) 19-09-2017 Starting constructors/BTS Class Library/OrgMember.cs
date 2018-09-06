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
                set { _AccessLevel = value; } }

            internal OrgMember(User pUser, Organisation pOrg)
            {

            }

            private bool Create()
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
