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
using System.Windows.Shapes;
using System.IO;

namespace RetroUtil
{
    /// <summary>
    /// Keep the user notified regarding the advance of the current process
    /// </summary>
    public partial class DynamicSplashScreen : Window
    {

        /// <summary>
        /// Massage displayed in the splash screen
        /// </summary>
        public String Message;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Process">Name of the current process</param>
        /// <param name="dynamicSplashScreenNotification">Displayed Notification</param>
        public DynamicSplashScreen(List<String> Process, DynamicSplashScreenNotification dynamicSplashScreenNotification)
        {
            InitializeComponent();

            //Defaults for splash screen
            this.ShowInTaskbar = false;
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.None;

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Update Process name
            this.Process1.Text = Process[0];
            this.Process2.Text = Process[1];

            // Bind notification
            Binding binding = new Binding();
            binding.Source = dynamicSplashScreenNotification;
            binding.Path = new PropertyPath("Message");
            BindingOperations.SetBinding(this.DynamicSplashScreenMessage, TextBlock.TextProperty, binding);
        }

    }
}
