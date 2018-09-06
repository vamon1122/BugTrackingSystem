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
    public partial class Win_CreateOrg : Window
    {
        public Win_CreateOrg()
        {
            InitializeComponent();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            Organisation TempOrg = new Organisation();
            TempOrg.Name = input_OrgName.Text;
            if (!TempOrg.Create())
            {
                MessageBox.Show("Could not create your new organisation");
            }

            OrgMember TempMember = TempOrg.NewOrgMember(Data.ActiveUser);
            if (TempMember.Create())
            {
                MessageBox.Show("Organisation created successfully!");
                this.Close();
            }
            else
            {
                if (TempOrg.Delete())
                {
                    MessageBox.Show("You could not be added as a  member to your organisation. Your organisation has been deleted.");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Your organisation was created but you could not be added as a  member. Changes could not be reverted.");
                    this.Close();
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
