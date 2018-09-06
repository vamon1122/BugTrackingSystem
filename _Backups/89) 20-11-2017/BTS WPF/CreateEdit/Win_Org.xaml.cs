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
    /// Interaction logic for win_CreateOrg.xaml
    /// </summary>
    public partial class Win_Org : Window
    {
        Organisation MyOrg;

        public Win_Org()
        {
            InitializeComponent();
            MyOrg = new Organisation();
            Butt_Update.Visibility = Visibility.Hidden;
        }

        public Win_Org(Organisation pOrg)
        {
            InitializeComponent();
            
            MyOrg = pOrg;
            
            input_OrgName.Text = MyOrg.Name;
            Butt_Create.Visibility = Visibility.Hidden;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            MyOrg.Name = input_OrgName.Text;
            if (!MyOrg.Create())
            {
                MessageBox.Show("Could not create your new organisation");
            }

            OrgMember TempMember = MyOrg.NewOrgMember(Data.ActiveUser);
            if (TempMember.Create())
            {
                MessageBox.Show("Organisation created successfully!");
                this.Close();
            }
            else
            {
                if (MyOrg.Delete())
                {
                    MessageBox.Show("You could not be added as a member to your organisation. " +
                        "Your organisation has been deleted.");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Your organisation was created but you could not be added as a  member. " +
                        "Changes could not be reverted.");
                    this.Close();
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Result = MessageBox.Show("Are you sure you want to delete this organisation?",
                "Delete Organisation", MessageBoxButton.YesNo);

            if (Result == MessageBoxResult.Yes)
            {
                if (MyOrg.Delete())
                {
                    Data.Organisations.Remove(MyOrg);

                    MessageBox.Show("Organisation deleted successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("There was an error deleting this organisation. Note was not deleted", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            MyOrg.Name = input_OrgName.Text.ToString();
            if (MyOrg.Update())
            {
                MessageBox.Show("Organisation was updated successfully!");
            }
            else
            {
                MessageBox.Show("There was an error whilst saving changes to the organisation. " +
                    "Changes were not saved");
                
            }
        }
    }
}
