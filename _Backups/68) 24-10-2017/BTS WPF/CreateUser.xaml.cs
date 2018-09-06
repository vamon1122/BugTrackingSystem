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
    /// Interaction logic for CreateUser.xaml
    /// </summary>
    public partial class CreateUser : Window
    {
        public CreateUser()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Style style_TextBoxError = Application.Current.FindResource("TextBoxError") as Style;
            Style style_PasswordBoxError = Application.Current.FindResource("PasswordBoxError") as Style;



            bool ValidateSuccess = true;

            if (input_Forename.Text == "")
            {
                ValidateSuccess = false;
                input_Forename.Style = style_TextBoxError;
                MessageBox.Show("You cannot leave the forename field blank", "Blank Field", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (input_Surname.Text == "")
            {
                ValidateSuccess = false;
                input_Surname.Style = style_TextBoxError;
                MessageBox.Show("You cannot leave the surname field blank", "Blank Field",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //Username is no longer a mandetory field
            /*if (input_Username.Text == "")
            {
                ValidateSuccess = false;
                input_Username.Style = style_TextBoxError;
                MessageBox.Show("You cannot leave the username field blank", "Blank Field",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }*/

            //EMAIL
            if(input_EMail.Text == "")
            {
                ValidateSuccess = false;
                input_EMail.Style = style_TextBoxError;
                MessageBox.Show("You cannot leave the email field blank", "Invalid Input",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (!input_EMail.Text.Contains("@") || !input_EMail.Text.Contains("."))
            {
                ValidateSuccess = false;
                input_EMail.Style = style_TextBoxError;
                MessageBox.Show("You must enter a valid email address", "Invalid Input",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            //PASSWORD
            if(input_Password.Password == "")
            {
                ValidateSuccess = false;
                input_Password.Style = style_PasswordBoxError;
                input_ConfirmPassword.Style = style_PasswordBoxError;
                MessageBox.Show("You cannot leave the password field blank", "Invalid Input",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if(input_Password.Password.Length < 6)
            {
                ValidateSuccess = false;
                input_Password.Style = style_TextBoxError;
                input_ConfirmPassword.Style = style_TextBoxError;
                MessageBox.Show("Password is too short. Passwords must be at least 6 characters long", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (input_Password.Password != input_ConfirmPassword.Password)
            {
                ValidateSuccess = false;
                input_Password.Style = style_TextBoxError;
                input_ConfirmPassword.Style = style_TextBoxError;
                MessageBox.Show("Passwords do not match, please try again", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (ValidateSuccess)
            {
                User MyTempUser = new User();
                MyTempUser.FName = input_Forename.Text;
                MyTempUser.SName = input_Surname.Text;
                MyTempUser.Username = input_Username.Text;
                MyTempUser.JobTitle = input_JobTitle.Text;
                MyTempUser.EMail = input_EMail.Text;
                MyTempUser.Password = input_Password.Password;
                
                if (MyTempUser.Create())
                {
                    MessageBoxResult MyResult = MessageBox.Show("User created successfully! Click the OK button to log in", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (MyResult == MessageBoxResult.OK)
                    {
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show(MyTempUser.ErrMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            

            

            



        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
