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
    public partial class BugControl : UserControl
    {
        private Bug MyBug;
        public BugControl(Bug pBug)
        {
            InitializeComponent();
            MyBug = pBug;
            BugTitle.Content = pBug.Title;

            if(pBug.Description != null && pBug.Description != "")
            {
                Description.Text = pBug.Description;
            }
            
            Severity.Content = pBug.Severity;
            
        }

        private void BugClicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bug \"" + MyBug.Title + "\" clicked!");
        }
    }
}
