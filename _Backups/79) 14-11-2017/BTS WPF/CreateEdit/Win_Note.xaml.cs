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

namespace BTS_WPF
{
    /// <summary>
    /// Interaction logic for Win_Note.xaml
    /// </summary>
    public partial class Win_Note : Window
    {
        Ctrl_Note MyNoteControl;

        public Win_Note()
        {
            InitializeComponent();
            MyNoteControl = null;
        }

        public Win_Note(Ctrl_Note pCtrlNote)
        {
            InitializeComponent();
            MyNoteControl = pCtrlNote;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
