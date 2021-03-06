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

namespace BTS_WPF
{
    /// <summary>
    /// Interaction logic for Win_CreateTAg.xaml
    /// </summary>
    public partial class Win_TagType : Window
    {
        public Win_BugAddTag MyWin_BugAddTag;

        public Win_TagType(Win_BugAddTag pWin_BugAddTag)
        {
            InitializeComponent();
           MyWin_BugAddTag = pWin_BugAddTag;
        }

        private void Click_CreateUpdateTag(object sender, RoutedEventArgs e)
        {
            bool IsUnique = true;

            foreach(TagType TempTagType in Data.TagTypes)
            {
                if(TempTagType.MyOrg.Id.ToString() == Data.ActiveOrg.Id.ToString() && TempTagType.Value == TagTypeField.Text)
                {
                    IsUnique = false;
                }
            }

            if (IsUnique)
            {
                TagType Temp = Data.ActiveOrg.NewTagType();
                //NEED SOME VALIDATION HERE ###
                Temp.Value = TagTypeField.Text;

                if (Temp.Create())
                {
                    Data.TagTypes.Add(Temp);
                    MessageBox.Show("Tag type created successfully", "Success");
                    MyWin_BugAddTag.UpdateTagTypes();
                }
                else
                {
                    MessageBox.Show("There was an error creating tagtype", "Error");
                }
            }
            else
            {
                MessageBox.Show("A tag type with this name has already been made for this organisation", "Error");
            }

            
        }
    }
}
