﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS_Class_Library
{
    public partial class Bug
    {
        public Bug(Guid pId)
        {
            OnlineDb = new OnlineDbClass(this);
            OfflineDb = new OfflineDbClass(this);
        }

        public Bug(User pRaisedBy, User pAssignedTo, string pTitle, string pDescription, int pSeverity)
        {
            OnlineDb = new OnlineDbClass(this);
            OfflineDb = new OfflineDbClass(this);
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

        private bool Download()
        {
            bool _Success = false;

            return _Success;
        }

        public bool Delete()
        {
            bool _Success = false;

            return _Success;
        }

        public bool CreateTag()
        {
            bool _Success = false;

            return _Success;
        }

        public bool CreateAssignee()
        {
            bool _Success = false;

            return _Success;
        }

        public bool CreateNote()
        {
            bool _Success = false;

            return _Success;
        }

        public bool Resolve()
        {
            bool _Success = false;

            return _Success;
        }

        public bool ReOpen()
        {
            bool _Success = false;

            return _Success;
        }

        public bool SetPassword()
        {
            bool _Success = false;

            return _Success;
        }
    }
}