using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS_Class_Library
{
    public partial class Bug
    {
        public class Tag
        {
            private Guid _Id;
            private Bug _MyBug;
            private DateTime _DateTimeCreated;
            private TagType _Type;
            private bool _Uploaded;

            public Guid Id { get { return _Id; } }
            public Bug MyBug { get { return _MyBug; } }
            public DateTime DateTimeCreated { get { return _DateTimeCreated; } }
            public TagType Type { get { return _Type; } }
            public bool Uploaded { get { return _Uploaded; } }

            internal Tag(Guid pId)
            {
                _Id = pId;
                Get();
            }

            internal Tag(TagType pTagType)
            {
                _Id = Guid.NewGuid();
                _DateTimeCreated = DateTime.Now;
                _Type = pTagType;
            }

            public bool Create()
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
