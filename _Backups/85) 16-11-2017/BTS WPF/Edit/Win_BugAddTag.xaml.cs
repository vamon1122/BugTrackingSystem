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
    /// Interaction logic for Win_BugAddTag.xaml
    /// </summary>
    public partial class Win_BugAddTag : Window
    {
        public Win_Bug MyEditBugWindow;

        public Win_BugAddTag(Win_Bug pBugWindow)
        {
            InitializeComponent();
            MyEditBugWindow = pBugWindow;
            UpdateTagTypes();
        }

        public void UpdateTagTypes()
        {
            combo_TagTypes.Items.Clear();

            foreach (TagType TempTag in Data.TagTypes)
            {
                //MessageBox.Show("Adding tag: " + TempTag.Value);
                if(TempTag.MyOrg.Id.ToString() == Data.ActiveOrg.Id.ToString())
                {
                    combo_TagTypes.Items.Add(TempTag);
                }
                
            }
        }

        private void NewTag_Click(object sender, RoutedEventArgs e)
        {
            new Win_TagType().ShowDialog();
            UpdateTagTypes();
        }

        private void AddTag_Click(object sender, RoutedEventArgs e)
        {
            TagType MyTagType = new TagType(new Guid(combo_TagTypes.SelectedValue.ToString()));
            if (!MyTagType.Get()){
                throw new Exception("Failed to download tag type while creating tag");
            }
            Tag MyNewTag = new Tag(MyEditBugWindow.MyBug, MyTagType);
            if (!MyNewTag.Create()){
                throw new Exception("Failed to create tag");
            }
            Data.Tags.Add(MyNewTag);
            MyEditBugWindow.UpdateTags();
            this.Close();
        }
    }
}
