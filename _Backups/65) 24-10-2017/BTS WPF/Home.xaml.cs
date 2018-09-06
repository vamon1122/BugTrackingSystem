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
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        public Home()
        {
            InitializeComponent();
            if(Data.ActiveUser != null)
            {
                //combo_ActiveUser.Items.Add(Data.ActiveUser);

                label_ActiveUser.Content = Data.ActiveUser.FullName;
                UpdateOrgList();
                //UpdateProductList();

                
                
            }
        }

        private void combo_ActiveOrg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            combo_ActiveProduct.Items.Clear();
            BugStack.Children.Clear();
            Data.ActiveProduct = null;

            Organisation Temp = new Organisation(new Guid(combo_ActiveOrg.SelectedValue.ToString()));
            if (Temp.Get())
            {
                //MessageBox.Show("Success!", "Success");
            }
            else
            {
                //MessageBox.Show(Temp.ErrMsg, "Error");
            }
            Data.ActiveOrg = Temp;
            UpdateProductList();
            Mouse.OverrideCursor = null;
        }

        private void UpdateOrgList()
        {
            
            foreach (Organisation Org in Data.ActiveUser.Organisations)
            {
                combo_ActiveOrg.Items.Add(Org);
            }
        }

        private void UpdateProductList()
        {
            
            combo_ActiveProduct.Items.Clear();

            foreach (Product MyProduct in Data.ActiveOrg.Products)
            {
                combo_ActiveProduct.Items.Add(MyProduct);
            }
        }

        private void UpdateBugList()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            BugStack.Children.Clear();

            foreach (Bug MyBug in Data.ActiveProduct.Bugs)
            {
                BugControl MyBugControl = new BugControl(MyBug);

                BugStack.Children.Add(MyBugControl);
            }
            Mouse.OverrideCursor = null;
        }

        private void combo_ActiveProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combo_ActiveProduct.SelectedValue != null)
            {
                Product TempProduct = new Product(new Guid(combo_ActiveProduct.SelectedValue.ToString()));
                TempProduct.Get();
                Data.ActiveProduct = TempProduct;
            }

            UpdateBugList();
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            new LogIn().Show();
            this.Close();
        }
    }
}
