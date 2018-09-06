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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LogIn : Window
    {
        public LogIn()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            User MyUser = new User();
            MyUser.Username = input_Username.Text;
            MyUser.Password = input_Password.Password;
            if (MyUser.LogIn()) { MessageBox.Show("Logged in successfully!", "Success!", MessageBoxButton.OK); } else { MessageBox.Show(MyUser.ErrMsg, "Error", MessageBoxButton.OK); };
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CreateUser CreateUserWindow = new CreateUser();
            CreateUserWindow.Show();
            this.Close();
        }
    }
}
