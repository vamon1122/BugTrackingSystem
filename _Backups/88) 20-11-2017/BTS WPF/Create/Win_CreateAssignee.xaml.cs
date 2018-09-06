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
                OrgMembers.Items.Add(TempOrgMember);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show( "Id = " + OrgMembers.SelectedValue, "Id");
            User TempUser = new User(new Guid(OrgMembers.SelectedValue.ToString()));
            if (!TempUser.Get()) { MessageBox.Show("Couldn't get user: " + TempUser.ErrMsg, "Error"); }


                
            Assignee TempAssignee = MyBugWin.MyBug.CreateAssignee(TempUser);

            bool AssigneeExists = false;

            //Checks if assignee has been created (if user has been assigned to this bug already)
            //The if statement below the foreach loop had to be separate because it edits Data.Assignees
            //during the loop.
            foreach (Assignee MyAssignee in Data.Assignees)
            {
                if (MyAssignee.MyUser.Id.ToString() == TempAssignee.MyUser.Id.ToString() &&
                    MyAssignee.BugId.ToString() == TempAssignee.BugId.ToString())
                {
                    AssigneeExists = true;
                }
            }

            if (AssigneeExists)
            {
                MessageBox.Show(String.Format("You have already assigned {0} to this bug",TempUser.FullName, MyBugWin.MyBug.Title), "Already Assigned");
            }
            else
            {
                if (TempAssignee.Create())
                {
                    Data.Assignees.Add(TempAssignee);
                    MyBugWin.UpdateAssignees();

                    MessageBox.Show(String.Format("Assigned {0} to this bug", TempAssignee.MyUser.FullName), "Success");

                }
                else
                {
                    MessageBox.Show("Failed to create assignee", "Error");
                }
            }

            
        }
    }
}
