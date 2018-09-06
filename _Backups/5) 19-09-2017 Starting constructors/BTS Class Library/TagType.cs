using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS_Class_Library
{
    public class TagType
    {
        private string _Type;

        public string TagName { get { return _Type; } }

        public TagType(string pTagType)
        {

        }

        private bool Create()
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

        private bool OfflineDbDelete()
        {
            bool _Success = false;

            return _Success;
        }
    }
}
