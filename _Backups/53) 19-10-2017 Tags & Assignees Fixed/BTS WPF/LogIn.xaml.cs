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

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            User MyUser = new User();
            if (input_Username.Text.Contains("@"))
            {
                MyUser.EMail = input_Username.Text;
            }
            else
            {
                MyUser.Username = input_Username.Text;
            }
            
            MyUser.Password = input_Password.Password;
            if (MyUser.LogIn()) {
                Home HomeWindow = new Home();
                HomeWindow.Show();
                this.Close();
                MessageBox.Show("Logged in successfully!", "Success!", MessageBoxButton.OK,MessageBoxImage.Information);
            }
            else {
                MessageBox.Show("Could not log in: " + MyUser.ErrMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            };
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            CreateUser CreateUserWindow = new CreateUser();
            CreateUserWindow.Show();
            this.Close();
        }

        private void RememberMe_Checked(object sender, RoutedEventArgs e)
        {

        }

        

    }
}
