using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS_Class_Library
{
    public class User
    {
        private Guid _Id;
        private string _FName;
        private string _SName;
        private string _Username;
        private string _JobTitle;
        private List<Organisation> _Organisations;
        private string _EMail;
        private string _Password;
        private DateTime _DateTimeCreated;

        public Guid Id { get { return _Id; } }

        public string FName {
            get {
                return FName;
            }
            set
            {
                if(value.Length < 51)
                {
                    _FName = value;
                }
                else
                {
                    throw new Exception("Forename exceeds 50 characters");
                }
            }
        }

        public string SName {
            get {
                return _SName;
            }
            set {
                if(value.Length < 51)
                {
                    _SName = value;
                }
                else
                {
                    throw new Exception("Surname exceeds 50 characters");
                }
            }
        }

        public string FullName { get { return FName + " " + SName; } }

        public string Username
        {
            get { return _Username; }
            set
            {
                if (value.Length < 51)
                {
                    _Username = value;
                }
            }
        }

        public string JobTitle {
            get {
                return _JobTitle;
            }
            set
            { if (value.Length < 51)
                {
                    _JobTitle = value;
                } else {
                    throw new Exception("Job title exceeds 50 characters");
                }
            }
        }

        public List<Organisation> Organisations { get { return _Organisations; } }

        public string EMail { get {
                return _EMail;
            }
            set {
                if (value.Contains("@") && value.Contains("."))
                {
                    if (value.Length < 255)
                    {
                        _EMail = value;
                    }
                    else
                    {
                        throw new Exception("Email address exceeds 254 characters");
                    }
                }
                else
                {
                    throw new Exception("Invalid email address");
                }
            }
        }

        public string Password { get { return _Password; } }
        public DateTime DateTimeCreated { get { return _DateTimeCreated; } }
        
        public User(Guid pId)
        {

        }

        public User(string pFName, string pSName, string pUsername, string pJobTitle, Organisation pOrganisation, string pEMail, string pPassword)
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

        private bool OfflineDbGetOrganisations()
        {
            bool _Success = false;

            return _Success;
        }

        private bool OnlineDbDownloadOrganisations()
        {
            bool _Success = false;

            return _Success;
        }
    }
}
