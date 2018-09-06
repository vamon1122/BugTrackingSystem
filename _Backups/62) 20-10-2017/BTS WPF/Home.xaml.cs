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
        }

        private void UpdateOrgList()
        {
            foreach (Organisation Org in Data.ActiveUser.Organisations)
            {
                Org.Get();

                combo_ActiveOrg.Items.Add(Org);
            }
        }

        private void UpdateProductList()
        {
            foreach (Product MyProduct in Data.ActiveOrg.Products)
            {
                MyProduct.Get();

                combo_ActiveOrg.Items.Add(MyProduct);
            }
        }

        private void combo_ActiveProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Product TempProduct = new Product(new Guid(combo_ActiveProduct.SelectedValue.ToString()));
            TempProduct.Get();
            Data.ActiveProduct = TempProduct;
        }
    }
}
