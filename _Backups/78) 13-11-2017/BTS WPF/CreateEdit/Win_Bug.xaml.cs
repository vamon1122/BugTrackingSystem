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

        //This is for editing bugs
        public Win_Bug(Ctrl_Bug pBugControl)//Much faster to pass tags rather than re-download them. Also
        {                                                        //helps with data integrity (everything is updated at once)
            InitializeComponent();
            MyBug = pBugControl.MyBug;
            MyBugControl = pBugControl;
            User TempUser = new User(pBugControl.MyBug.RaisedBy.Id);
            TempUser.Get();
            label_RaisedBy.Content = TempUser.FullName;
            input_Title.Text = pBugControl.MyBug.Title;
            input_Description.Text = pBugControl.MyBug.Description;
            combo_Severity.Text = pBugControl.MyBug.Severity.ToString();

            UpdateTags();
            UpdateNotes();
        }

        public Win_Bug()//This is for new bugs
        {                                                        //helps with data integrity (everything is updated at once)
            InitializeComponent();
            MyBug = Data.ActiveProduct.NewBug();
            MyBugControl = null;
            User TempUser = Data.ActiveUser;
            TempUser.Get();
            label_RaisedBy.Content = TempUser.FullName;
                        
            /*UpdateTags();
            UpdateNotes();*/
        }

        public void UpdateTags()
        {
            TagsView.Children.Clear();
            foreach (Tag TempTag in Data.ActiveProductBugTagList)
            {
                if (TempTag.BugId == MyBug.Id)
                {
                    TagsView.Children.Add(new Ctrl_Tag(TempTag, this));
                }
            }
            if(MyBugControl != null)
            {
                MyBugControl.UpdateUi();
            }
            else
            {
                //This bug is being created (not edited)
            }
            
        }

        public void UpdateNotes()
        {
            NotesView.Children.Clear();
            foreach (Note TempNote in Data.ActiveProductBugNoteList)
            {
                if (TempNote.BugId == MyBug.Id)
                {
                    NotesView.Children.Add(new Ctrl_Note(TempNote,this));
                }
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

            if (ValidateSuccess)
            {
                MyBug.Title = input_Title.Text;
                MyBug.Description = input_Description.Text;
                MyBug.Severity = Convert.ToInt16(combo_Severity.Text);

                if (MyBugControl == null) //If it equals null, this is a new bug (needs to be created not updated)
                {
                    if (MyBug.Create())
                    {

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
            new Win_BugAddTag(this).Show();
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddAssignee_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
