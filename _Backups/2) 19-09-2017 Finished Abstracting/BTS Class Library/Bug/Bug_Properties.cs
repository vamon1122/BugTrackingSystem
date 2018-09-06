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
        private List<Note> _Notes;
        private DateTime _CreatedDateTime;
        private DateTime _ResolvedDateTime;
        private DateTime _ReOpenedDateTime;
        private string _ErrorMsg;
        private OnlineDbClass OnlineDb;
        private OfflineDbClass OfflineDb;

        public Guid Id { get; set; }
        public User RaisedBy { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Severity { get; set; }
        public List<Assignee> Assignees { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Note> Notes { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ResolvedDateTime { get; set; }
        public DateTime ReOpenedDateTime { get; set; }
        public bool Resolved { get; set; }
        public int TotalTags { get; set; }
        public int TotalAssignees { get; set; }
        public int TotalNotes { get; set; }
        public string ErrorMsg { get; set; }
    }
}
