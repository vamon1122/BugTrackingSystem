﻿using System;
using BTS_Class_Library;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using System.Reflection;


using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace BTS_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class win_LogIn : Window
    {
        bool UserRemembered = false;
        public win_LogIn()
        {
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            AppVersLabel.Content = String.Format("(Vers {0})", Assembly.GetExecutingAssembly().GetName().Version.ToString());

            if (ConfigurationManager.AppSettings["UsernameOrEmail"] != "")
            {
                input_UsernameOrEMail.Text = ConfigurationManager.AppSettings["UsernameOrEmail"];
                RememberMe.IsChecked = true;

                if (ConfigurationManager.AppSettings["Password"] != "")
                {
                    input_Password.Password = ConfigurationManager.AppSettings["Password"];
                }
                UserRemembered = true;
                LogIn();
            }
            else
            {
                input_UsernameOrEMail.Focus();
            }
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            LogIn();
        }

        private void LogIn()
        {
            //Styles are defined at the top of the function
            Style style_TextBoxError = Application.Current.FindResource("TextBoxError") as Style;
            Style style_PasswordBoxError = Application.Current.FindResource("PasswordBoxError") as Style;

            //Success bool will be set to false if there is an error
            bool ValidateSuccess = true;

            //Checks that username/email is not blank
            if (input_UsernameOrEMail.Text == "")
            {
                ValidateSuccess = false;
                input_UsernameOrEMail.Style = style_TextBoxError;
                MessageBox.Show("You cannot leave the username / e-mail field blank", "Blank Field",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //Checks password is not blank
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
                    if (RememberMe.IsChecked == true)
                    {
                        Configuration config = ConfigurationManager.OpenExeConfiguration(AppDomain.CurrentDomain.BaseDirectory + "BTS WPF.exe");
                        
                        try
                        {
                            config.AppSettings.Settings["UsernameOrEmail"].Value = input_UsernameOrEMail.Text;
                        }
                        catch (System.NullReferenceException)
                        {
                            config.AppSettings.Settings.Add("UsernameOrEmail", input_UsernameOrEMail.Text);
                        }

                        try
                        {
                            config.AppSettings.Settings["Password"].Value = input_Password.Password;
                        }
                        catch (System.NullReferenceException)
                        {
                            config.AppSettings.Settings.Add("Password", input_Password.Password);
                        }
                        
                        config.Save(ConfigurationSaveMode.Minimal);
                        ConfigurationManager.RefreshSection("appSettings");
                    }

                    //MessageBox.Show("Initialising");
                    Win_SplashScreen TempSplash = new Win_SplashScreen();
                    TempSplash.Show();

                    //This is a check to see if the window has been initialised. Had a problem where
                    //sometimes after Window.Show(), the window would be blank until Data.Initialise()
                    //had completed which made the splash screen a bit pointless.

                    Data.Initialise();

                    /*if (TempSplash.Ready()) {
                        Data.Initialise();
                    }
                    else {
                        throw new Exception("App failed to initialise");
                    };*/

                    TempSplash.Close();
                    //MessageBox.Show("Initialised");

                    Mouse.OverrideCursor = null;

                    Win_Home HomeWindow = new Win_Home();
                    HomeWindow.Show();
                    this.Close();

                    if (!UserRemembered) { MessageBox.Show("Logged in successfully!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information); }
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
            Win_CreateUser CreateUserWindow = new Win_CreateUser();
            CreateUserWindow.ShowDialog();
            //this.Close();
        }

        private void RememberMe_Unchecked(object sender, RoutedEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(AppDomain.CurrentDomain.BaseDirectory + "BTS WPF.exe");
            //config.AppSettings.Settings.Add("UsernameOrEmail", input_UsernameOrEMail.Text);
            //config.AppSettings.Settings.Add("Password", input_Password.Password);
            config.AppSettings.Settings["UsernameOrEmail"].Value = null;
            config.AppSettings.Settings["Password"].Value = null;
            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
