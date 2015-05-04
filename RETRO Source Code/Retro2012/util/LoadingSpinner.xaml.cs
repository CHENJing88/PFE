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
using System.Windows.Media.Animation;

namespace RetroGUI.util
{
    /// <summary>
    /// Define Loading Spinner User Control
    /// Inspired by http://sharpfellows.com/post/WPF-Wait-Indicator-(aka-Spinner).aspx
    /// </summary>
    public partial class LoadingSpinner : UserControl
    {

        /// <summary>
        /// Animation Storyboard
        /// </summary>
        private Storyboard _storyboard;


        /// <summary>
        /// Constructor
        /// </summary>
        public LoadingSpinner()
        {
            InitializeComponent();

            this.IsVisibleChanged += OnVisibleChanged;
        }


        /// <summary>
        /// Handler for Visibility change
        /// </summary>
        private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                StartAnimation();
            }
            else
            {
                StopAnimation();
            }
        }


        /// <summary>
        /// Start the Loading Animation
        /// </summary>
        private void StartAnimation()
        {
            _storyboard = (Storyboard)FindResource("canvasAnimation");
            _storyboard.Begin(canvas, true);
        }


        /// <summary>
        /// Stop the loading Animation
        /// </summary>
        private void StopAnimation()
        {
            _storyboard.Remove(canvas);
        }
    }
}
