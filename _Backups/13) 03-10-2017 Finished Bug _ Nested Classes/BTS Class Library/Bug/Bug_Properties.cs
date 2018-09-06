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
        public bool Uploaded;
        private static string OnlineConnStr;
        private static string LocalConnStr;

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
                }
                else
                {
                    throw new Exception("Description exceeds 4000 characters");
                }
            }
        }

        public int Severity { get { return _Severity; } set { _Severity = value; } } //Need to decide on some limits for this
        public List<Assignee> Assignees { get { return _Assignees; } }
        public List<Tag> Tags { get { return _Tags; } }
        public List<Note> Notes { get { return _Notes; } }
        public DateTime CreatedDateTime { get { return _CreatedDateTime; } }
        public DateTime ResolvedDateTime { get { return _ResolvedDateTime; } }
        public DateTime ReOpenedDateTime { get { return _ReOpenedDateTime; } }

        public bool Resolved
        {
            get
            {
                if (_ResolvedDateTime != null && _ReOpenedDateTime != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int TotalTags { get { return _Tags.Count; } }
        public int TotalAssignees { get { return _Assignees.Count; } }
        public int TotalNotes { get { return _Notes.Count; } }
        public string ErrorMsg { get { return _ErrorMsg;  } }
    }
}
