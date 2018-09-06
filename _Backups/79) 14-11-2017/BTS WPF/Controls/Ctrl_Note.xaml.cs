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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BTS_Class_Library;

namespace BTS_WPF
{
    /// <summary>
    /// Interaction logic for Ctrl_Note.xaml
    /// </summary>
    public partial class Ctrl_Note : UserControl
    {
        Note MyNote;
        Win_Bug MyEditBugWindow;
        public Ctrl_Note(Note pNote, Win_Bug pEditBugWindow)
        {
            MyNote = pNote;
            MyEditBugWindow = pEditBugWindow;

            InitializeComponent();

            Title.Content = MyNote.Title;
            Body.Text = MyNote.Body;
        }

        private void EditNote_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}