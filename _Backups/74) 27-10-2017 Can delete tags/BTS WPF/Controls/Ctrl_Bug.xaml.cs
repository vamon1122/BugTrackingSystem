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
    /// Interaction logic for BugControl.xaml
    /// </summary>
    public partial class Ctrl_Bug : UserControl
    {
        public Bug MyBug;

        public void UpdateUi()
        {
            BugTitle.Content = MyBug.Title;

            if (MyBug.Description != null && MyBug.Description != "")
            {
                Description.Text = MyBug.Description;
            }

            Severity.Content = MyBug.Severity;
        }

        public Ctrl_Bug(Bug pBug)
        {
            InitializeComponent();
            MyBug = pBug;
            BugTitle.Content = pBug.Title;

            if(pBug.Description != null && pBug.Description != "")
            {
                Description.Text = pBug.Description;
            }
            
            Severity.Content = pBug.Severity;

            Label_Tags.Content = "Tags: ";

            GetTags();   
        }

        public void GetTags()
        {
            Label_Tags.Content = "Tags: ";
            foreach (Tag TempTag in Data.ActiveProductBugTagList)
            {
                if (TempTag.BugId == MyBug.Id)
                {
                    Label_Tags.Content += TempTag.Type.Value + ", ";
                }
            }
        }

        private void BugClicked(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Bug \"" + MyBug.Title + "\" clicked!");
            new Win_EditBug(this).Show();
        }
    }
}
