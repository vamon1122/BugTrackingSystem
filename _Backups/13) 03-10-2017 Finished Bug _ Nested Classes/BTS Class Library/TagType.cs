using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS_Class_Library
{
    public class TagType
    {
        private Guid _Id;
        private Organisation _MyOrg;
        private string _Value;
        public bool Uploaded;

        public TagType(Guid pId)
        {
            _Id = pId;
            Get();
        }

        public TagType(Organisation pOrg)
        {
            _Id = Guid.NewGuid();
            _MyOrg = pOrg;
            Uploaded = false;
        }


        public Guid Id { get { return _Id; } }
        public Organisation MyOrg { get { return _MyOrg; } }
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (value.Length > 50)
                {
                    throw new Exception("Tag value exceeds 50 characters");
                }
                else
                {
                    _Value = value;
                }
            }
        }

        public bool Create()
        {
            return false;
        }

        public bool Get()
        {
            return false;
        }

        public bool Delete()
        {
            return false;
        }
    }
}
