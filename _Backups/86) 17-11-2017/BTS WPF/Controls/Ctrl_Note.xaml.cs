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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BTS_Class_Library;

namespace BTS_WPF
{
    /// <summary>
    /// Interaction logic for Ctrl_Note.xaml
    /// </summary>
    public partial class Ctrl_Note : UserControl
    {
        public Note MyNote;
        Win_Bug MyBugWindow;
        public Ctrl_Note(Note pNote, Win_Bug pBugWindow)
        {
            MyNote = pNote;
            MyBugWindow = pBugWindow;

            InitializeComponent();

            Title.Content = MyNote.Title;
            Body.Text = MyNote.Body;
        }

        private void EditNote_Click(object sender, RoutedEventArgs e)
        {
            new Win_Note(MyBugWindow, this).Show();
        }
    }
}
