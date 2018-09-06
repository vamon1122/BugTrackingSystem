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
        private string _Organisations;
        private string _EMail;
        private string _Password;
        private DateTime _DateTimeCreated;

        public Guid Id { get; set; }
        public string FName { get; set; }
        public string SName { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string JobTitle { get; set; }
        public List<Organisation> Organisations { get; set; }
        public string EMail { get; set; }
        public string Password { get; set; }
        public DateTime DateTimeCreated { get; set; }
        
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
