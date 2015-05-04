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
using System.ComponentModel;

namespace RetroGUI.main
{
    /// <summary>
    /// Define PropertyGrid Control
    /// </summary>
    public partial class PropertyGridControl : UserControl
    {
        /// <summary>
        /// Object selected in the grid
        /// </summary>
        private object selectedObject = null;


        /// <summary>
        /// Constructor
        /// </summary>
        public PropertyGridControl()
        {
            InitializeComponent();
            
        }


        /// <summary>
        /// Get/Set Object selected in the grid
        /// </summary>
        public object SelectedObject{
            get { return selectedObject; }
            set { selectedObject = value; SelectedObjectHelper(selectedObject,null); }
        }


        /// <summary>
        /// Handle sleected object behaviour
        /// </summary>
        public void SelectedObjectHelper(object value,EventArgs e) {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                                           new EventHandler(SelectedObjectHelper), value, e);
            }
            else
            {
                this.PropertyPanel.Children.Clear(); //clear propertypanel
                
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value))
                {
                    if (!property.IsBrowsable) continue; //could also check for browsableattribute, but this one's shorter

                    PropertyItemControl currentProperty = new PropertyItemControl();
                    currentProperty.PropertyName = property.Name;        
                    Binding b = new Binding(property.Name);
                    b.Source = selectedObject;
                    b.Mode = property.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;

                    currentProperty.SetBinding(PropertyItemControl.PropertyValueProperty, b);
                    currentProperty.OnActive += new EventHandler<DescriptionEventArgs>(currentProperty_OnActive);
                    
                    foreach (Attribute attribute in property.Attributes)
                    {
                        if (attribute.GetType() == typeof(DescriptionAttribute))
                        {
                            currentProperty.PropertyDescription = ((DescriptionAttribute)attribute).Description;
                        }
                       if (attribute.GetType() == typeof(CategoryAttribute)) {
                            currentProperty.PropertyCategory = ((CategoryAttribute)attribute).Category;
                        }
                    }      
                    PropertyPanel.Children.Add(currentProperty); //add the propertyitem
                }
            }            
        }

        /// <summary>
        /// Display text of the selected object
        /// </summary>
        void currentProperty_OnActive(object sender, DescriptionEventArgs e)
        {
            if (!Application.Current.Dispatcher.CheckAccess()){
                Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                                           new EventHandler<DescriptionEventArgs>(currentProperty_OnActive),sender, e);
            }else{
                this.descriptionTextBlock.Text = e.Description;
            }
        }

        /// <summary>
        /// Reset method
        /// </summary>
        public void Reset()
        {
            // Set selectedObject to null
            this.selectedObject = null;

            // Clear the list of properties
            this.PropertyPanel.Children.Clear();

            // Clear the prperty text description
            this.descriptionTextBlock.Text = "";
        }


        /// <summary>
        /// Add manually a PropetyItem (without binding)
        /// </summary>
        /// <param name="propertyName">Name of the prperty</param>
        /// <param name="propertyValue">Value of the property</param>
        /// <param name="propertyDescription">Description of the property</param>
        public void AddPropertyItem(String propertyName, String propertyValue, String propertyDescription)
        {
            // Create a new PropertyItemControl
            PropertyItemControl property = new PropertyItemControl();
            property.PropertyName = propertyName;
            property.PropertyValue = propertyValue;
            property.PropertyDescription = propertyDescription;

            // Add a event handler on activation
            property.OnActive += new EventHandler<DescriptionEventArgs>(currentProperty_OnActive);

            // Add the PropertyItem to the panel
            this.PropertyPanel.Children.Add(property); 
        }

    }
}

