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
using System.ComponentModel;

namespace RetroUtil
{
    /// <summary>
    /// Define notification for the Dynamic splash screen
    /// </summary>
    public class DynamicSplashScreenNotification : INotifyPropertyChanged
    {

        private String _Message;

        /// <summary>
        /// Message to display
        /// </summary>
        public String Message
        {
            get { return _Message; }
            set
            {
                _Message = value;
                OnPropertyChanged("Message");
            }
        }


        /// <summary>
        /// Allow a dynamic splash screen
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Property changed
        /// </summary>
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
