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

            public Guid Id { get { return _Id; } }
            public Bug MyBug { get { return _MyBug; } }
            public DateTime DateTimeCreated { get { return _DateTimeCreated; } }
            public TagType Type { get { return _Type; } }

            internal Tag(Guid pId)
            {

            }

            internal Tag(string pTagType)
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

            public bool Delete()
            {
                bool _Success = false;

                return _Success;
            }
        }
    }
    
}
