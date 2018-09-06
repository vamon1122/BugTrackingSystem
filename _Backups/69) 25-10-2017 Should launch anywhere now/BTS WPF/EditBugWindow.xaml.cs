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
    /// Interaction logic for BugWindow.xaml
    /// </summary>
    public partial class EditBugWindow : Window
    {
        Bug MyBug;
        BugControl MyBugControl;
        public EditBugWindow(Bug pBug, BugControl pBugControl)
        {
            InitializeComponent();
            MyBug = pBug;
            MyBugControl = pBugControl;
            User TempUser = new User(pBug.RaisedBy.Id);
            TempUser.Get();
            label_RaisedBy.Content = TempUser.FullName;
            input_Title.Text = pBug.Title;
            input_Description.Text = pBug.Description;
            combo_Severity.Text = pBug.Severity.ToString();

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Action_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Style style_TextBoxError = Application.Current.FindResource("TextBoxError") as Style;
            Style style_PasswordBoxError = Application.Current.FindResource("PasswordBoxError") as Style;

            bool ValidateSuccess = true;

            if(input_Title.Text == "")
            {
                ValidateSuccess = false;
                input_Title.Style = style_TextBoxError;
                MessageBox.Show("You cannot leave the title field blank", "Blank Field",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            /*if (input_Description.Text == "")
            {
                ValidateSuccess = false;
                input_Description.Style = style_TextBoxError;
                MessageBox.Show("You cannot leave the description field blank", "Blank Field",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }*/

            if (ValidateSuccess)
            {
                MyBug.Title = input_Title.Text;
                MyBug.Description = input_Description.Text;
                MyBug.Severity = Convert.ToInt16(combo_Severity.Text);
                if (MyBug.Update())
                {
                    if(MyBugControl != null)
                    {
                        MyBugControl.UpdateUi();
                    }
                    
                    MessageBox.Show("Bug updated successfully!");
                }
                else
                {
                    MessageBox.Show("Error while updating bug: " + MyBug.ErrMsg, "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            
        }
    }
}
