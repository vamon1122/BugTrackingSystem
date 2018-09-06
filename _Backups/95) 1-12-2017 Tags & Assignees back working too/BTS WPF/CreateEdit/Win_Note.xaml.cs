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
    /// Interaction logic for Win_Note.xaml
    /// </summary>
    public partial class Win_Note : Window
    {
        Ctrl_Note MyNoteControl;
        Win_Bug MyBugWin;

        public Win_Note(Win_Bug pBugWin)
        {
            InitializeComponent();
            MyNoteControl = null;
            MyBugWin = pBugWin;
        }

        public Win_Note(Win_Bug pBugWin, Ctrl_Note pCtrlNote)
        {
            InitializeComponent();
            MyNoteControl = pCtrlNote;
            MyBugWin = pBugWin;
            input_Title.Text = MyNoteControl.MyNote.Title;
            input_Body.Text = MyNoteControl.MyNote.Body;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            //This is a new note, it needs to be created
            if(MyNoteControl == null)
            {
                Note TempNote = MyBugWin.MyBug.CreateNote();
                TempNote.Title = input_Title.Text;
                TempNote.Body = input_Body.Text;

                MyBugWin.MyNoteCopies.Add(TempNote);
                //if (TempNote.Create())
                {
                    /*Data.Notes.Add(TempNote);
                    */
                    
                    MyBugWin.UpdateNotes();
                    MessageBox.Show("Note created successfully");
                    this.Close();
                }
                /*else
                {
                    MessageBox.Show("Failed to create note");
                }*/
            }
            else
            {
                //This is an existing note, it needs to be updated
                MyNoteControl.MyNote.Title = input_Title.Text;
                MyNoteControl.MyNote.Body = input_Body.Text;
                //if (MyNoteControl.MyNote.Update())
                {
                    //I don't think that this could ever be in Data.Notes. MyNoteControl.MyNote is a copy of the note
                    //which is in Data.Notes, it is not the same note.

                    /*if (Data.Notes.Contains(MyNoteControl.MyNote))
                    {
                        Data.Notes.Remove(MyNoteControl.MyNote);
                    }*/

                    //When would it not be???
                    /*if (!MyBugWin.MyNoteCopies.Contains(MyNoteControl.MyNote))
                    {
                        MyBugWin.MyNoteCopies.Add(MyNoteControl.MyNote);
                    }*/
                    

                    MyBugWin.UpdateNotes();
                    MessageBox.Show("Note updated successfully");
                }
                /*else
                {
                    MessageBox.Show("Failed to update note");
                }*/
            }
            
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Result = MessageBox.Show("Are you sure you want to delete this note?",
                "Delete Note", MessageBoxButton.YesNo);

            if (Result == MessageBoxResult.Yes)
            {
                

                Note NoteCopy = MyBugWin.MyNoteCopies.Find(x => x.Id.ToString() == MyNoteControl.MyNote.Id.ToString());

                if(NoteCopy != null)
                {
                    MyBugWin.MyNoteCopies.Remove(NoteCopy);
                }
                else
                {
                    MessageBox.Show("###Note not found 2");
                }

                //I don't think that this check is necessary
                /*if (!MyBugWin.MyDeletedNoteCopies.Contains(MyNoteControl.MyNote))
                {*/
                    if(MyNoteControl.MyNote.DateTimeCreated != DateTime.MinValue)
                    {
                    //It only needs to be added to the 'for deletion' list if it has already
                    //been created on the database.
                    MyBugWin.MyDeletedNoteCopies.Add(MyNoteControl.MyNote);
                    }
                    
                //}

                MyBugWin.UpdateNotes();
                MessageBox.Show("Note deleted successfully");

                /*if (MyNoteControl.MyNote.Delete())
                {
                    Data.Notes.Remove(MyNoteControl.MyNote);
                    MyNoteControl.MyBugWindow.UpdateNotes();

                    MessageBox.Show("Note deleted successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("There was an error deleting this note. Note was not deleted", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }*/
            }
        }
    }
}
