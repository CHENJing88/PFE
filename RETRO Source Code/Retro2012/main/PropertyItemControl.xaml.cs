/*
 * RETRO 2012 - v2.3
 * 
 * PaRADIIT Project
 * https://sites.google.com/site/paradiitproject/
 * 
 * This software is provided under LGPL v.3 license, 
 * which exact definition can be found at the following link:
 * http://www.gnu.org/licenses/lgpl.html
 * 
 * Please, contact us for any offers, remarks, ideas, etc.
 * 
 * Copyright © RFAI, LI Tours, 2011-2012
 * Contacts : rayar@univ-tours.fr
 *            ramel@univ-tours.fr
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RetroGUI.main
{
    /// <summary>
    /// Define the PropertyItem Control
    /// </summary>
    public partial class PropertyItemControl : UserControl
    {

        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(PropertyItemControl));
        public static readonly DependencyProperty PropertyValueProperty = DependencyProperty.Register("PropertyValue", typeof(object), typeof(PropertyItemControl));
        public static readonly DependencyProperty PropertyDescriptionProperty = DependencyProperty.Register("PropertyDescription", typeof(string), typeof(PropertyItemControl));
        public static readonly DependencyProperty PropertyCategoryProperty = DependencyProperty.Register("PropertyCategory", typeof(string), typeof(PropertyItemControl));
        
        public EventHandler<DescriptionEventArgs> DescriptionEventHandler;
        public event EventHandler<DescriptionEventArgs> OnActive;


        public PropertyItemControl()
        {
            InitializeComponent();          
            PropertyItemGrid.DataContext = this;
        }

        public string PropertyName {

            get { return (string)GetValue(PropertyItemControl.PropertyNameProperty); }
            set { SetValue(PropertyItemControl.PropertyNameProperty, value); }
        }

        public object PropertyValue
        {
            get { return (string)GetValue(PropertyItemControl.PropertyValueProperty); }
            set { SetValue(PropertyItemControl.PropertyValueProperty, value); }
        }

        public string PropertyDescription
        {
            get { return (string)GetValue(PropertyItemControl.PropertyDescriptionProperty); }
            set { SetValue(PropertyItemControl.PropertyDescriptionProperty, value); }
        }

        public string PropertyCategory
        {
            get { return (string)GetValue(PropertyItemControl.PropertyCategoryProperty); }
            set { SetValue(PropertyItemControl.PropertyCategoryProperty, value); }
        }

        #region events
        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (OnActive != null) {
                OnActive(this, new DescriptionEventArgs(PropertyDescription));
            }
        }
        #endregion
    }

    public class DescriptionEventArgs : EventArgs{
        public string Description { get; set;}

        public DescriptionEventArgs(string descr){
            this.Description = descr;
        }
    }
}
