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
using System.Timers;

namespace RetroGUI.util
{
    /// <summary>
    /// Define Toast Window
    /// Auto-closing notification windows
    /// Useful to force GUI refresh
    /// </summary>
    public partial class ToastWindow : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="duration">time during the toast is visible</param>
        public ToastWindow(String message, int duration)
        {
            InitializeComponent();
            this.messageLabel.Content = message;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Auto closing method
            Timer t = new Timer();
            t.Interval = duration;
            t.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            t.Start();

        }

        /// <summary>
        /// Handler for Timer
        /// </summary>
        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Close();
            }), null);
        }
    }
}
