using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS_Class_Library
{
    public partial class Organisation
    {
        private Guid _Id;
        private string _Name;
        private DateTime _DateTimeCreated;
        private List<OrgMember> _Members;

        public Guid Id { get { return _Id; } }
        public string Name {
            get {
                return _Name;
            }
            set {
                if(value.Length < 51)
                {
                    _Name = value;
                    OfflineDbUpdate();
                    OnlineDbCreate();
                }
                else
                {
                    throw new Exception("Organisation name exceeds 50 characters");
                }
            }
        }
        public DateTime DateTimeCreated { get { return _DateTimeCreated; } }
        public List<OrgMember> Members { get { return _Members; } }
        public int NoOfMembers { get { return _Members.Count; } }

        public Organisation(Guid pId)
        {

        }

        public Organisation(string pName)
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

        private bool CountMembers()
        {
            bool _Success = false;

            return _Success;
        }
    }
}
