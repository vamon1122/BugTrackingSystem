﻿using System;
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
            private Guid _UserId;
            private TimeSpan _TimeSpent;
            private int _AccessLevel;
            private DateTime _DateTimeCreated;

            public Guid Id { get; set; }
            public User MyUser { get; set; }
            public TimeSpan TimeSpend { get; set; }
            public int AccessLevel { get; set; }
            public DateTime DateTimeCreated { get; set; }

            internal Assignee(Guid pId)
            {

            }

            internal Assignee(User pUser)
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

            private bool OfflineDbCreate()
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
