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
    /// Interaction logic for Win_CreateProduct.xaml
    /// </summary>
    public partial class Win_CreateProduct : Window
    {
        public Win_CreateProduct()
        {
            InitializeComponent();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            Product TempProduct = Data.ActiveOrg.NewProduct();

            TempProduct.Name = input_ProductName.Text;

            if (TempProduct.Create())
            {
                MessageBox.Show("Product was created successfully!");

                this.Close();
            }
            else
            {
                MessageBox.Show("Could not create your new product");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
