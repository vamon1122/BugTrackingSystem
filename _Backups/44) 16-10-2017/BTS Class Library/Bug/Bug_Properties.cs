﻿using System;
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
        private DateTime _DateTimeCreated;
        //private DateTime _ResolvedDateTime;
        private string _ErrMsg;
        public bool Uploaded;
        private static string OnlineConnStr = Data.OnlineConnStr;
        private static string LocalConnStr = Data.LocalConnStr;

        public Guid Id { get { return _Id; } }
        public User RaisedBy { get { return _RaisedBy; } }

        public string Title
        {
            get { return _Title; }

            set
            {
                if (value.Length < 51)
                {
                    Uploaded = false;
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
                Uploaded = false;
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

        public int Severity { get { return _Severity; } set { Uploaded = false; _Severity = value; } } //Need to decide on some limits for this
        public List<Assignee> Assignees { get { return _Assignees; } }
        public List<Tag> Tags { get { return _Tags; } }
        public List<Note> Notes { get { return _Notes; } }
        public DateTime CreatedDateTime { get { return _DateTimeCreated; } }
        //public DateTime ResolvedDateTime { get { return _ResolvedDateTime; } }
        
        /*public bool Resolved
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
        }*/

        public int TotalTags { get { return _Tags.Count; } }
        public int TotalAssignees { get { return _Assignees.Count; } }
        public int TotalNotes { get { return _Notes.Count; } }
        public string ErrMsg { get { return _ErrMsg;  } }
    }
}