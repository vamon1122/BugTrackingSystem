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
        private List<Assignee> _Assignees = new List<Assignee>();
        private List<Tag> _Tags = new List<Tag>();
        private List<Note> _Notes = new List<Note>();
        private DateTime _CreatedDateTime;
        private DateTime _ResolvedDateTime;
        private DateTime _ReOpenedDateTime;
        private string _ErrorMsg;
        private OnlineDbClass OnlineDb;
        private OfflineDbClass OfflineDb;

        public Guid Id { get { return _Id; } }
        public User RaisedBy { get { return _RaisedBy; } }

        public string Title
        {
            get { return _Title; }

            set
            {
                if (value.Length < 51)
                {
                    _Title = value;
                    OfflineDb.UpdateBug();
                    OnlineDb.UpdateBug();
                }
                else
                {
                    throw new Exception("Title exceeds 50 characters");
                }
            }
        }

        public string Description
        {
            get { return _Description; }

            set
            {
                if (value.Length < 4001)
                {
                    _Description = value;
                    OfflineDb.UpdateBug();
                    OnlineDb.UpdateBug();
                }
                else
                {
                    throw new Exception("Description exceeds 4000 characters");
                }
            }
        }

        public int Severity { get { return _Severity; } set { _Severity = value; OfflineDb.UpdateBug(); OnlineDb.UpdateBug(); } } //Need to decide on some limits for this
        public List<Assignee> Assignees { get { OfflineDb.GetAssignees(); OnlineDb.DownloadAsignees(); return _Assignees; } }
        public List<Tag> Tags { get { OfflineDb.GetTags(); OnlineDb.DownloadTags(); return _Tags; } }
        public List<Note> Notes { get { OfflineDb.GetNotes(); OnlineDb.DownloadNotes(); return _Notes; } }
        public DateTime CreatedDateTime { get { return _CreatedDateTime; } }
        public DateTime ResolvedDateTime { get { return _ResolvedDateTime; } }
        public DateTime ReOpenedDateTime { get { return _ReOpenedDateTime; } }
        public bool Resolved { get { if (_ResolvedDateTime != null && _ReOpenedDateTime != null) { return true; } else { return false; } } }
        public int TotalTags { get { return OnlineDb.CountTags(); } }
        public int TotalAssignees { get { return OnlineDb.CountAssignees(); } }
        public int TotalNotes { get { return OnlineDb.CountNotes(); } }
        public string ErrorMsg { get { return _ErrorMsg;  } }
    }
}
