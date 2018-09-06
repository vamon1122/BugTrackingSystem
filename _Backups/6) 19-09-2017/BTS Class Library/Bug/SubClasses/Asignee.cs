using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS_Class_Library
{
    public partial class Bug
    {
        public class Assignee
        {
            private Guid _BugId;
            private User _MyUser;
            private TimeSpan _TimeSpent;
            private int _AccessLevel;
            private DateTime _DateTimeCreated;

            public Guid BugId { get { return _BugId; } }
            public User MyUser { get { return _MyUser; } }
            public TimeSpan TimeSpent { get { return _TimeSpent; } set { _TimeSpent += value; } }
            public int AccessLevel { get { return _AccessLevel; } set { AccessLevel = value; } }
            public DateTime DateTimeCreated { get { return _DateTimeCreated; } }

            internal Assignee(Guid pId)
            {

            }

            internal Assignee(User pUser)
            {

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
