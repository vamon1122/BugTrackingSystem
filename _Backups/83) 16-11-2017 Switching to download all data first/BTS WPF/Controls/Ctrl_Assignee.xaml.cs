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
    /// Interaction logic for Ctrl_Assignee.xaml
    /// </summary>
    public partial class Ctrl_Assignee : UserControl
    {
        Assignee MyAssignee;
        Win_Bug MyBugWin;

        public Ctrl_Assignee(Assignee pAssignee, Win_Bug pBugWin)
        {

            InitializeComponent();
            MyAssignee = pAssignee;
            MyBugWin = pBugWin;
            butt_Assignee.Content = MyAssignee.MyUser.FullName;
        }

        private void butt_Assignee_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Result = MessageBox.Show(String.Format("Are you sure you want to un-assign {0}", MyAssignee.MyUser.FullName), "Un-assign", MessageBoxButton.YesNo) ;

            if(Result == MessageBoxResult.Yes)
            {
                if (!MyAssignee.Delete())
                {
                    throw new Exception(MyAssignee.ErrMsg);
                }
                MyBugWin.UpdateAssignees();
                MessageBox.Show("Assignee was removed successfully!", "Success");
                
            }
            else
            {

            }
        }
    }
}
