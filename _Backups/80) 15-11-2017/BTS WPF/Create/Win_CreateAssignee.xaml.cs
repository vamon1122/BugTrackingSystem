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
    /// Interaction logic for Win_CreateAssignee.xaml
    /// </summary>
    public partial class Win_CreateAssignee : Window
    {
        Win_Bug MyBugWin;
        public Win_CreateAssignee(Win_Bug pBugWin)
        {
            InitializeComponent();

            MyBugWin = pBugWin;

            foreach(OrgMember TempOrgMember in Data.ActiveOrg.Members)
            {
                /*MessageBox.Show("Name: " + TempOrgMember.MyUser.FullName + "Id: " + 
                    TempOrgMember.MyUser.Id, "Org Member Downloaded!");*/
                OrgMembers.Items.Add(TempOrgMember);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show( "Id = " + OrgMembers.SelectedValue, "Id");
            User TempUser = new User(new Guid(OrgMembers.SelectedValue.ToString()));
            if (!TempUser.Get()) { MessageBox.Show("Couldn't get user: " + TempUser.ErrMsg, "Error"); }
                
            Assignee TempAssignee = MyBugWin.MyBug.CreateAssignee(TempUser);
            if (TempAssignee.Create())
            {
                MessageBox.Show("Created assignee", "Success");
            }
            else
            {
                MessageBox.Show("Failed to create assignee", "Error");
            }
        }
    }
}
