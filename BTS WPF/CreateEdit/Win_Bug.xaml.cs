using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BTS_Class_Library;

namespace BTS_WPF
{
    /// <summary>
    /// Interaction logic for BugWindow.xaml
    /// </summary>
    public partial class Win_Bug : Window
    {
        public Bug MyBug;
        public Ctrl_Bug MyBugControl;
        public Win_Home MyHomeWindow;

        
        public List<Note> MyNoteCopies = new List<Note>();
        public List<Note> MyDeletedNoteCopies = new List<Note>();

        public List<Tag> MyTagCopies = new List<Tag>();
        public List<Tag> MyDeletedTagCopies = new List<Tag>();

        public List<Assignee> MyAssigneeCopies = new List<Assignee>();
        public List<Assignee> MyDeletedAssigneeCopies = new List<Assignee>();


        //This is for editing bugs
        public Win_Bug(Ctrl_Bug pBugControl)//Much faster to pass tags rather than re-download them. Also
        {                                                        //helps with data integrity (everything is updated at once)
            InitializeComponent();
            MyBug = pBugControl.MyBug;
            MyBugControl = pBugControl;
            MyHomeWindow = pBugControl.MyHomeWindow;
            User TempUser = new User(pBugControl.MyBug.RaisedBy.Id);
            TempUser.Get();
            label_RaisedBy.Content = TempUser.FullName;
            input_Title.Text = pBugControl.MyBug.Title;
            input_Description.Text = pBugControl.MyBug.Description;
            combo_Severity.Text = pBugControl.MyBug.Severity.ToString();

            BackupNotes();
            BackupTags();
            BackupAssignees();
            UpdateTags();
            UpdateNotes();
            UpdateAssignees();
        }

        public Win_Bug(Win_Home pHomeWindow)             //This is for new bugs
        {                            //helps with data integrity (everything is updated at once)
            InitializeComponent();
            MyBug = Data.ActiveProduct.NewBug();
            MyBugControl = null;
            MyHomeWindow = pHomeWindow;
            User TempUser = Data.ActiveUser;
            TempUser.Get();
            label_RaisedBy.Content = TempUser.FullName;
                        
            /*UpdateTags();
            UpdateNotes();*/
        }

        public void UpdateAssignees()
        {
            AssigneesView.Children.Clear();
            foreach(Assignee TempAssignee in MyAssigneeCopies)
            {
                if (TempAssignee.BugId.ToString() == MyBug.Id.ToString())
                {
                    Ctrl_Assignee TempAssigneeControl = new Ctrl_Assignee(TempAssignee, this);
                    AssigneesView.Children.Add(TempAssigneeControl);
                }
            } 
        }

        public void UpdateTags()
        {
            //MessageBox.Show("Updating tags...");
            TagsView.Children.Clear();

            foreach (Tag TempTag in MyTagCopies)
            {
                //MessageBox.Show("Tag {0} found!", TempTag.Type.Value);
                TagsView.Children.Add(new Ctrl_Tag(TempTag, this));
            }

            //MessageBox.Show("All tags have been added");

            if (MyBugControl != null)
            {
                MyBugControl.UpdateUi();
            }
            else
            {
                //This bug is being created (not edited)
            }

            /*foreach (Tag TempTag in Data.Tags)
            {
                if (TempTag.BugId.ToString() == MyBug.Id.ToString())
                {
                    TagsView.Children.Add(new Ctrl_Tag(TempTag, this));
                }
            }

            if (MyBugControl != null)
            {
                MyBugControl.UpdateUi();
            }
            else
            {
                //This bug is being created (not edited)
            }*/

        }

        
        private void BackupNotes()
        {
            foreach (Note TempNote in Data.Notes)
            {
                if (TempNote.BugId.ToString() == MyBug.Id.ToString())
                {
                    Note CopyOfTempNote = new Note(TempNote.Id);
                    CopyOfTempNote.Title = TempNote.Title;
                    CopyOfTempNote.Body = TempNote.Body;

                    MyNoteCopies.Add(CopyOfTempNote);
                }
            }
        }

        private void BackupTags()
        {
            foreach (Tag TempTag in Data.Tags)
            {
                if (TempTag.BugId.ToString() == MyBug.Id.ToString())
                {
                    Tag CopyOfTempTag = new Tag(TempTag.Id, TempTag.BugId, TempTag.DateTimeCreated, TempTag.Type.Id);

                    MyTagCopies.Add(CopyOfTempTag);
                }
            }
        }

        private void BackupAssignees()
        {
            foreach (Assignee TempAssignee in Data.Assignees)
            {
                if (TempAssignee.BugId.ToString() == MyBug.Id.ToString())
                {
                    Assignee CopyOfTempAssignee = new Assignee(MyBug, TempAssignee.MyUser);

                    MyAssigneeCopies.Add(CopyOfTempAssignee);
                }
            }
        }

        public void UpdateNotes()
        {
            
            NotesView.Children.Clear();
            

            foreach(Note TempNote in MyNoteCopies)
            {
                NotesView.Children.Add(new Ctrl_Note(TempNote, this));
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Action_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            //Need to sort out notes, assignees and tags here

            foreach(Note TempNote in MyNoteCopies)
            {
                Note OriginalNote = Data.Notes.Find(x => x.Id.ToString() == TempNote.Id.ToString());

                if(OriginalNote != null)
                {
                    //MessageBox.Show("Original Note Title: " + OriginalNote.Title);
                    OriginalNote.Title = TempNote.Title;
                    //MessageBox.Show("Original Note Title Is Now: " + OriginalNote.Title);
                    OriginalNote.Body = TempNote.Body;

                    if (!OriginalNote.Update())
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    Note NewNote = MyBug.CreateNote();

                    //MessageBox.Show("Original Note Title: " + OriginalNote.Title);
                    NewNote.Title = TempNote.Title;
                    //MessageBox.Show("Original Note Title Is Now: " + OriginalNote.Title);
                    NewNote.Body = TempNote.Body;

                    Data.Notes.Add(NewNote);

                    if (!NewNote.Create())
                    {
                        throw new Exception();
                    }
                }
            }

            foreach (Note TempNote in MyDeletedNoteCopies)
            {
                Note OriginalNote = Data.Notes.Find(x => x.Id.ToString() == TempNote.Id.ToString());
                
                if (!OriginalNote.Delete())
                {
                    throw new Exception();
                }

                Data.Notes.Remove(OriginalNote);
            }

            foreach (Tag TempTag in MyTagCopies)
            {
                Tag OriginalTag = Data.Tags.Find(x => x.Id.ToString() == TempTag.Id.ToString());

                if (OriginalTag != null)
                {
                    /*//MessageBox.Show("Original Tag Title: " + OriginalTag.Title);
                    OriginalTag.Title = TempTag.Title;
                    //MessageBox.Show("Original Tag Title Is Now: " + OriginalTag.Title);
                    OriginalTag.Body = TempTag.Body;

                    if (!OriginalTag.Update())
                    {
                        throw new Exception();
                    }*/
                }
                else
                {
                    Tag NewTag = MyBug.CreateTag(TempTag.Type);
                    
                    if (!NewTag.Create())
                    {
                        throw new Exception();
                    }

                    Data.Tags.Add(NewTag);
                }
            }

            

            foreach (Tag TempTag in MyDeletedTagCopies)
            {
                Tag OriginalTag = Data.Tags.Find(x => x.Id.ToString() == TempTag.Id.ToString());

                if (!OriginalTag.Delete())
                {
                    throw new Exception();
                }
                else
                {
                    MessageBox.Show("DEBUG: Tag Deleted");
                }

                Data.Tags.Remove(OriginalTag);
            }

            foreach (Assignee TempAssignee in MyAssigneeCopies)
            {
                Assignee OriginalAssignee = Data.Assignees.Find(x => x.BugId.ToString() == TempAssignee.BugId.ToString() && x.MyUser.Id.ToString() == TempAssignee.MyUser.Id.ToString());

                if (OriginalAssignee != null)
                {
                    /*//MessageBox.Show("Original Assignee Title: " + OriginalAssignee.Title);
                    OriginalAssignee.Title = TempAssignee.Title;
                    //MessageBox.Show("Original Assignee Title Is Now: " + OriginalAssignee.Title);
                    OriginalAssignee.Body = TempAssignee.Body;

                    if (!OriginalAssignee.Update())
                    {
                        throw new Exception();
                    }*/
                }
                else
                {
                    Assignee NewAssignee = MyBug.CreateAssignee(TempAssignee.MyUser);

                    if (!NewAssignee.Create())
                    {
                        throw new Exception();
                    }

                    Data.Assignees.Add(NewAssignee);
                }
            }



            foreach (Assignee TempAssignee in MyDeletedAssigneeCopies)
            {
                Assignee OriginalAssignee = Data.Assignees.Find(x => x.BugId.ToString() == TempAssignee.BugId.ToString() && x.MyUser.Id.ToString() == TempAssignee.MyUser.Id.ToString());

                if (!OriginalAssignee.Delete())
                {
                    throw new Exception();
                }
                else
                {
                    MessageBox.Show("DEBUG: Assignee Deleted");
                }

                Data.Assignees.Remove(OriginalAssignee);
            }



            Style style_TextBoxError = Application.Current.FindResource("TextBoxError") as Style;
            Style style_PasswordBoxError = Application.Current.FindResource("PasswordBoxError") as Style;

            bool ValidateSuccess = true;

            if(input_Title.Text == "")
            {
                ValidateSuccess = false;
                input_Title.Style = style_TextBoxError;
                MessageBox.Show("You cannot leave the title field blank", "Blank Field",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            /*if (input_Description.Text == "")
            {
                ValidateSuccess = false;
                input_Description.Style = style_TextBoxError;
                MessageBox.Show("You cannot leave the description field blank", "Blank Field",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }*/

            if(combo_Severity.Text == "")
            {
                ValidateSuccess = false;
                MessageBox.Show("You cannot leave the severity field blank", "Blank Field", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            if (ValidateSuccess)
            {
                MyBug.Title = input_Title.Text;
                MyBug.Description = input_Description.Text;
                MyBug.Severity = Convert.ToInt16(combo_Severity.Text);

                if (MyBugControl == null) //If it equals null, this is a new bug (needs to be created not updated)
                {
                    if (MyBug.Create())
                    {
                        MyHomeWindow.UpdateBugList();
                        MessageBox.Show("Bug created successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Error while creating bug: " + MyBug.ErrMsg, "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    if (MyBug.Update())
                    {
                        MyBugControl.UpdateUi();
                        MessageBox.Show("Bug updated successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Error while updating bug: " + MyBug.ErrMsg, "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            
        }

        private void AddTag_Click(object sender, RoutedEventArgs e)
        {
            new Win_BugAddTag(this).ShowDialog();
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            new Win_Note(this).ShowDialog();
            UpdateNotes();
        }

        private void AddAssignee_Click(object sender, RoutedEventArgs e)
        {
            new Win_CreateAssignee(this).ShowDialog();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Result = MessageBox.Show("Are you sure you want to delete this bug?", 
                "Delete Bug", MessageBoxButton.YesNo);

            if (Result == MessageBoxResult.Yes)
            {
                if (MyBug.Delete())
                {
                    Data.Bugs.Remove(MyBugControl.MyBug);
                    MyBugControl.MyHomeWindow.UpdateBugList();

                    MessageBox.Show("Bug deleted successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("There was an error deleting this bug. Bug was not deleted", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error); 
                }
            }
        }

        void Win_Bug_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            /*foreach(Note TempNote in MyNotes)
            {
                if(TempNote.DateTimeCreated != DateTime.MinValue)
                {
                    Data.Notes.Add(TempNote);
                }
            }*/
        }
    }
}
