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
            User MyTempUser = new User();
            MyTempUser.FName = input_Forename.Text;
            MyTempUser.SName = input_Surname.Text;
            MyTempUser.Username = input_Username.Text;
            MyTempUser.JobTitle = input_JobTitle.Text;
            MyTempUser.EMail = input_Email.Text;
            if(input_Password.Password == input_ConfirmPassword.Password)
            {
                MyTempUser.Password = input_Password.Password;
                if (MyTempUser.Create())
                {
                   MessageBoxResult MyResult = MessageBox.Show("User created successfully! Click the OK button to log in", "Success", MessageBoxButton.OK,MessageBoxImage.Information);
                    if(MyResult == MessageBoxResult.OK)
                    {
                        LogIn LogInWindow = new LogIn();
                        LogInWindow.Show();
                        this.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Passwords do not match, please try again", "Error", MessageBoxButton.OK,MessageBoxImage.Error);
            }
            
        }
    }
}
