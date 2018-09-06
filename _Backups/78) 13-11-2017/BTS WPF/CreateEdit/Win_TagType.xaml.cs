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
    /// Interaction logic for Win_CreateTAg.xaml
    /// </summary>
    public partial class Win_TagType : Window
    {
        public Win_TagType()
        {
            InitializeComponent();
        }

        private void Click_CreateUpdateTag(object sender, RoutedEventArgs e)
        {
            TagType Temp = Data.ActiveOrg.NewTagType();
            //NEED SOME VALIDATION HERE ###
            Temp.Value = TagTypeField.Text;

            if (Temp.Create())
            {
                MessageBox.Show("Success", "Tag type created successfully");
                //### Need to update list. Pass in 'Win_BugAddTag'?
                
            }
            else
            {
                MessageBox.Show("Error", "There was an error creating tagtype");
            }
        }
    }
}
