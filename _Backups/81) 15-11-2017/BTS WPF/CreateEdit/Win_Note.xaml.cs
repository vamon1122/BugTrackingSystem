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
                if (TempNote.Create())
                {
                    Data.ActiveProductBugNoteList.Add(TempNote);
                    MessageBox.Show("Note created successfully");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to create note");
                }
            }
            else
            {
                //This is an existing note, it needs to be updated
                MyNoteControl.MyNote.Title = input_Title.Text;
                MyNoteControl.MyNote.Body = input_Body.Text;
                if (MyNoteControl.MyNote.Update())
                {
                    MyBugWin.UpdateNotes();
                    MessageBox.Show("Note updated successfully");
                }
                else
                {
                    MessageBox.Show("Failed to update note");
                }
            }
            
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
