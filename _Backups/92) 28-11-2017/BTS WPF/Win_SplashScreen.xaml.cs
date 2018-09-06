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

namespace BTS_WPF
{
    /// <summary>
    /// Interaction logic for Win_SplashScreen.xaml
    /// </summary>
    public partial class Win_SplashScreen : Window
    {
        public Win_SplashScreen()
        {
            InitializeComponent();
        }

        public bool Ready()
        {
            //This is a check to see if the window has been initialised. Had a problem where
            //sometimes after Window.Show(), the window would be blank until Data.Initialise()
            //had completed which made the splash screen a bit pointless. See Win_LogIn().
            return true;
        }
    }
}
