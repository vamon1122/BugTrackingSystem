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
    /// Interaction logic for Ctrl_OrgMember.xaml
    /// </summary>
    public partial class Ctrl_OrgMember : UserControl
    {
        OrgMember MyOrgMember;
        Win_Org MyOrgWin;

        public Ctrl_OrgMember(OrgMember pOrgMember, Win_Org pOrgWin)
        {
            InitializeComponent();
            MyOrgMember = pOrgMember;
            MyOrgWin = pOrgWin;
            butt_OrgMember.Content = MyOrgMember.MyUser.FullName;
        }

        private void butt_OrgMember_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Result = MessageBox.Show(String.Format("Are you sure you want to " +
                "remove {0} from the organisation?", MyOrgMember.MyUser.FullName),
                "Un-assign", MessageBoxButton.YesNo);

            if (Result == MessageBoxResult.Yes)
            {
                if (!MyOrgMember.Delete())
                {
                    throw new Exception(MyOrgMember.ErrMsg);
                }
                Data.OrgMembers.Remove(MyOrgMember);
                MyOrgWin.UpdateOrgMembers();
                MessageBox.Show("{0} was successfully removed from the organisation!", "Success");

            }
            else
            {

            }
        }
    }
}
