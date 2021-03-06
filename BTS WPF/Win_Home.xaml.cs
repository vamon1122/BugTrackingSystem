﻿using System;
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
using BenLog;

namespace BTS_WPF
{
    
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Win_Home : Window
    {
        static Log UiLog;
        private static string CodeFileName = "Win_Home.xaml.cs";

        public Win_Home()
        {

            InitializeComponent();

            UiLog = new Log("UiLog.txt");

            UiLog.Debug("Initialising home page...");

            if (Data.ActiveUser != null)
            {
                //combo_ActiveUser.Items.Add(Data.ActiveUser);

                label_ActiveUser.Content = Data.ActiveUser.FullName;
                UpdateOrgList();
                //UpdateProductList();
                combo_ActiveProduct.Text = "Placeholder";
                
                
            }
            UiLog.Debug("...Success! Home page was initialised");
        }

        private void combo_ActiveOrg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UiLog.Info("ActiveOrg selection was changed! Attempting to do 'combo_ActiveOrg_SelectionChanged' event...");
            Mouse.OverrideCursor = Cursors.Wait;
            combo_ActiveProduct.Items.Clear();
            //Data.ActiveOrgTagTypeList.Clear();
            BugStack.Children.Clear();
            Data.ActiveProduct = null;

            if (combo_ActiveOrg.SelectedValue != null)
            {
                //Organisation Temp = new Organisation(new Guid(combo_ActiveOrg.SelectedValue.ToString()));
                Organisation Temp = Data.Organisations.Single(org => org.Id.ToString() == combo_ActiveOrg.SelectedValue.ToString());

                Data.ActiveOrg = Temp;

                /*foreach(TagType MyTagType in Data.ActiveOrg.TagTypes)
                {
                    Data.TagTypes.Add(MyTagType);
                }*/

                UpdateProductList();
            }
            
            Mouse.OverrideCursor = null;
            UiLog.Info("...Success! 'combo_ActiveOrg_SelectionChanged' event was completed");
        }

        private void UpdateOrgList()
        {
            string MethodName = "private void UpdateOrgList()";
            string InfoPrefix = String.Format("[{0} {1}]", CodeFileName, MethodName);

            UiLog.Info(InfoPrefix + " Attempting to update UI 'organisations' drop-down list from DATA 'organisations' list...");
            combo_ActiveOrg.Items.Clear();

            UiLog.Debug(String.Format("{1} There are {0} items in Data.ActiveUser.Organisations", Data.ActiveUser.Organisations.Count(), InfoPrefix));

            foreach (Organisation Org in Data.ActiveUser.Organisations)
            {
                UiLog.Debug(String.Format("{1} BEN!!! Organisation {0} found in ActiveUser.Organisations. {0} was added to list", Org.Name, InfoPrefix));
                combo_ActiveOrg.Items.Add(Org);
            }
            UiLog.Info(InfoPrefix + " Success! UI 'organisations' drop-down list was updated from DATA 'organisations' list");
        }

        private void UpdateProductList()
        {
            UiLog.Info("Attempting to update UI 'products' drop-down list from DATA 'products' list...");
            combo_ActiveProduct.Items.Clear();

            UiLog.Debug(String.Format("There are {0} items in Data.ActiveOrg.Products", Data.ActiveOrg.Products.Count()));

            foreach (Product MyProduct in Data.ActiveOrg.Products)
            {
                combo_ActiveProduct.Items.Add(MyProduct);
            }
            UiLog.Info("Success! UI 'products' drop-down list was updated from DATA 'products' list");
        }

        

        public void UpdateBugList()
        {
            UiLog.Info("Attempting to update UI 'bugs' scroll viewer list from DATA 'bugs' list...");
            BugStack.Children.Clear();

            UiLog.Debug(String.Format("There are {0} bugs in Data.Bugs", Data.Bugs.Count()));

            int Matches = 0;

            foreach (Bug MyBug in Data.Bugs)
            {
                if(MyBug.ProductId.ToString() == Data.ActiveProduct.Id.ToString())
                {
                    BugStack.Children.Add(new Ctrl_Bug(MyBug,this));
                    Matches++;
                }
            }

            UiLog.Debug(String.Format("{0} bugs were added to the stack", Matches));

            Mouse.OverrideCursor = null;
            UiLog.Info("Success! UI 'bugs' scroll viewer list was updated from DATA 'bugs' list");
        }

        private void combo_ActiveProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UiLog.Info("ActiveProduct selection was changed! Attempting to do 'combo_ActiveProduct_SelectionChanged' event...");
            if (combo_ActiveProduct.SelectedValue != null)
            {
                Product TempProduct = new Product(new Guid(combo_ActiveProduct.SelectedValue.ToString()));
                TempProduct.Get();
                Data.ActiveProduct = TempProduct;
                UiLog.Debug("Active product is now = " + TempProduct.Name);
            }

            UpdateBugList();
            UiLog.Info("...Success! 'combo_ActiveProduct_SelectionChanged' event was completed");
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            UiLog.Info("LogOut was clicked! Attempting to log user out...");
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
            UiLog.Info("...Success! User was logged out");
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
            UiLog.Info("NewOrg was clicked!");
            new Win_Org().ShowDialog();
            UpdateOrgList();
        }

        private void butt_NewBug_Click(object sender, RoutedEventArgs e)
        {
            UiLog.Info("NewBug was clicked!");
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
            UiLog.Info("NewProduct was clicked!");
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
            UiLog.Info("EditOrg was clicked!");
            if (Data.ActiveOrg == null)
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
