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
using System.Configuration;

namespace BTS_WPF
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Win_Home : Window
    {
        

        public Win_Home()
        {

            InitializeComponent();
            if(Data.ActiveUser != null)
            {
                //combo_ActiveUser.Items.Add(Data.ActiveUser);

                label_ActiveUser.Content = Data.ActiveUser.FullName;
                UpdateOrgList();
                //UpdateProductList();
                combo_ActiveProduct.Text = "Placeholder";
                
                
            }
        }

        private void combo_ActiveOrg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            combo_ActiveProduct.Items.Clear();
            //Data.ActiveOrgTagTypeList.Clear();
            BugStack.Children.Clear();
            Data.ActiveProduct = null;

            if (combo_ActiveOrg.SelectedValue != null)
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

                /*foreach(TagType MyTagType in Data.ActiveOrg.TagTypes)
                {
                    Data.TagTypes.Add(MyTagType);
                }*/

                UpdateProductList();
            }
            
            Mouse.OverrideCursor = null;
        }

        private void UpdateOrgList()
        {
            combo_ActiveOrg.Items.Clear();
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

        

        public void UpdateBugList()
        {
            BugStack.Children.Clear();

            foreach (Bug MyBug in Data.Bugs)
            {
                if(MyBug.ProductId.ToString() == Data.ActiveProduct.Id.ToString())
                {
                    BugStack.Children.Add(new Ctrl_Bug(MyBug,this));
                }
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
            Configuration config = ConfigurationManager.OpenExeConfiguration(AppDomain.CurrentDomain.BaseDirectory + "BTS WPF.exe");
            //config.AppSettings.Settings.Add("UsernameOrEmail", input_UsernameOrEMail.Text);
            //config.AppSettings.Settings.Add("Password", input_Password.Password);
            config.AppSettings.Settings["UsernameOrEmail"].Value = null;
            config.AppSettings.Settings["Password"].Value = null;
            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection("appSettings");

            Data.ActiveOrg = null;
            Data.ActiveProduct = null;
            Data.ActiveUser = null;
            Data.Assignees.Clear();
            Data.Bugs.Clear();
            Data.Notes.Clear();
            Data.Organisations.Clear();
            Data.Products.Clear();
            Data.Tags.Clear();
            Data.TagTypes.Clear();
            Data.OrgMembers.Clear();

            win_LogIn MyLogInPage = new win_LogIn();

            //MyLogInPage.Show();
            MyLogInPage.Show();
            this.Close();
            /*foreach (Window MyWindow in App.Current.Windows)
            {
                if(MyWindow != MyLogInPage)
                {
                    MyWindow.Close();
                }
            }*/
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            BugStack.Children.Clear();

            List<Bug> AddedBugs = new List<Bug>(); //It is quite resource intensive to check the stack pannel each time
                                                   //because I would have to loop through the whole stack every time. It's
                                                   //much quicker to create a temporary list of bugs which have been added
                                                   //to the stack and use that instead. So update/check this instead of the
                                                   //stack pannel!
            
            foreach (Bug MyBug in Data.Bugs)
            {
                if(MyBug.ProductId.ToString() == Data.ActiveProduct.Id.ToString() && MyBug.Title.Contains(SearchBox.Text))
                {
                    AddedBugs.Add(MyBug);
                    BugStack.Children.Add(new Ctrl_Bug(MyBug,this));
                }
            }

            /*if(BugStack.Children.Contains)
            foreach(BugControl MyBugControl in BugStack.Children)*/

            foreach (Tag MyTag in Data.Tags)
            {
                /*MessageBox.Show("Value: " + MyTag.Type.Value);
                MessageBox.Show("Search: " + SearchBox.Text);*/
                if (MyTag.Type.Value.Contains(SearchBox.Text)) //If searchbox contains tag name
                {
                    foreach (Bug MyBug in Data.Bugs) //For each bug in the active product list
                    {
                        if(MyTag.BugId == MyBug.Id) //If the tag is associated with this bug...
                        {
                            if (!AddedBugs.Contains(MyBug) && MyBug.ProductId.ToString() == Data.ActiveProduct.Id.ToString()) //... and the bug isnt alread in the *pretend* stack and is a bug for the active product
                            {
                                BugStack.Children.Add(new Ctrl_Bug(MyBug,this)); //Add the bug to the stack
                            }
                        }
                    }
                }
            }
            Mouse.OverrideCursor = null;
        }

        private void Home_Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Window MyWindow in App.Current.Windows)
            {
                if(MyWindow != this && !(MyWindow is win_LogIn))
                {
                    MyWindow.Close();
                }
            }
        }

        private void MouseOverChange(object sender, DependencyPropertyChangedEventArgs e)
        {
           
        }

        private void butt_NewOrg_Click(object sender, RoutedEventArgs e)
        {
            new Win_Org().ShowDialog();
            UpdateOrgList();
        }

        private void butt_NewBug_Click(object sender, RoutedEventArgs e)
        {
            if (Data.ActiveOrg == null)
            {
                MessageBox.Show("You must select the organisation & product which you want to create a bug for first!", "Select Organisation", MessageBoxButton.OK);
            }
            else if(Data.ActiveProduct == null)
            {
                MessageBox.Show("You must select the product which you want to create a bug for first!", "Select Product", MessageBoxButton.OK);
            }
            else
            {
                new Win_Bug(this).ShowDialog();
                UpdateBugList();
            }
        }

        private void butt_NewProduct_Click(object sender, RoutedEventArgs e)
        {
            if (Data.ActiveOrg == null)
            {
                MessageBox.Show("You must select the organisation which you want to create a product for first!", "Select Organisation", MessageBoxButton.OK);
            }
            else
            {
                new Win_CreateProduct().ShowDialog();
                UpdateProductList();
            }
            
        }

        private void Click_EditOrg(object sender, RoutedEventArgs e)
        {
            if(Data.ActiveOrg == null)
            {
                MessageBox.Show("You must select the organisation which you want to edit first!", "Select Organisation", MessageBoxButton.OK);
            }
            else
            {
                new Win_Org(Data.ActiveOrg).ShowDialog();
                UpdateOrgList();
            }
            
        }
    }
}
