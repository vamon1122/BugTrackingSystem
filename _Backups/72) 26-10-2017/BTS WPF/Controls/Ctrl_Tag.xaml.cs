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
    /// Interaction logic for ctrl_Tag.xaml
    /// </summary>
    public partial class Ctrl_Tag : UserControl
    {
        public Tag MyTag;
        public Ctrl_Tag(Tag pTag)
        {
            
            InitializeComponent();
            MyTag = pTag;
            butt_Name.Content = pTag.Type.Value;
        }

        private void butt_Name_Click(object sender, RoutedEventArgs e)
        {
            new Win_EditTag(this);
        }
    }
}
