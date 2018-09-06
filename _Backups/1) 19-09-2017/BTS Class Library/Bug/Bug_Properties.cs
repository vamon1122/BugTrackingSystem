using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS_Class_Library
{
    public partial class Bug
    {
        private Guid _Id;
        private User _RaisedBy;
        private string _Title;
        private string _Description;
        private int _Severity;
        private List<User> _Assignees;
        private List<Tag> _Tags;
        private DateTime _CreatedDateTime;
        private DateTime _ResolvedDateTime;
        private DateTime _ReopenedDateTime;
        private string _ErrorMsg;
        private OnlineDbClass OnlineDb;
        private OfflineDbClass OfflineDb;

        public Guid Id { get; set; }
        public int MyProperty { get; set; }
    }
}
