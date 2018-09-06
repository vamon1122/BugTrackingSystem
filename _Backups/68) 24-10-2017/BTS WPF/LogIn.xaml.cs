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
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            input_UsernameOrEMail.Focus();
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            Style style_TextBoxError = Application.Current.FindResource("TextBoxError") as Style;
            Style style_PasswordBoxError = Application.Current.FindResource("PasswordBoxError") as Style;

            bool ValidateSuccess = true;

            if (input_UsernameOrEMail.Text == "")
            {
                ValidateSuccess = false;
                input_UsernameOrEMail.Style = style_TextBoxError;
                MessageBox.Show("You cannot leave the username / e-mail field blank", "Blank Field",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (input_Password.Password == "")
            {
                ValidateSuccess = false;
                input_Password.Style = style_PasswordBoxError;
                MessageBox.Show("You cannot leave the password field blank", "Blank Field",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (ValidateSuccess)
            {
                User MyUser = new User();
                if (input_UsernameOrEMail.Text.Contains("@"))
                {
                    MyUser.EMail = input_UsernameOrEMail.Text;
                }
                else
                {
                    MyUser.Username = input_UsernameOrEMail.Text;
                }

                MyUser.Password = input_Password.Password;

                Mouse.OverrideCursor = Cursors.Wait;
                if (MyUser.LogIn())
                {
                    Mouse.OverrideCursor = null;
                    Home HomeWindow = new Home();
                    HomeWindow.Show();
                    this.Close();
                    MessageBox.Show("Logged in successfully!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    MessageBox.Show("Could not log in: " + MyUser.ErrMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                };
            }
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            CreateUser CreateUserWindow = new CreateUser();
            CreateUserWindow.Show();
            //this.Close();
        }

        private void RememberMe_Checked(object sender, RoutedEventArgs e)
        {

        }

        

    }
}
