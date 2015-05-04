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
using System.Windows.Threading;
using System.ComponentModel;
using RetroGUI.main;

namespace RetroGUI.visualisation
{
    /// <summary>
    /// Define EoC Window
    /// </summary>
    public partial class EoCWindow : Window
    {
        private bool _HasBeenClosed;
        /// <summary>
        /// To avoid multiple instince of ClusterWindows ans handle it's closing
        /// </summary>
        public bool HasBeenClosed
        {
            get { return _HasBeenClosed; }
            set { _HasBeenClosed = value; }
        }

 
        /// <summary>
        /// Constructor
        /// </summary>
        public EoCWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.HasBeenClosed = false;
        }


        /// <summary>
        /// Set EoC Attributes
        /// Used in PageWindow(2)
        /// </summary>
        public void SetEoCAttributes(String eocID, ImageSource imageSource, String eocDimensions)
        {
            this.EoCIDLabel.Content = eocID;
            this.EoCImage.Source = imageSource;
            this.EoCDimensionsLabel.Content = eocDimensions;
            this.EoCTranscriptionTextBlock.Text = "";
        }


        /// <summary>
        /// Set EoC Transcription
        /// </summary>
        /// <param name="eocTranscription">EoC Transcription</param>
        public void SetEoCTranscription(String eocTranscription)
        {
            this.EoCTranscriptionTextBlock.Text = eocTranscription;
        }


        /// <summary>
        /// Reset EoC Attributes
        /// </summary>
        public void ResetEoCWindow()
        {
            this.EoCIDLabel.Content = "";
            this.EoCImage.Source = null;
            this.EoCDimensionsLabel.Content = "";
            this.EoCTranscriptionTextBlock.Text = "";
        }


        /// <summary>
        /// Override ClearClustersList event to set a flag in order manage the ClusterWindow Lifetime
        /// </summary>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            this.HasBeenClosed = true;
        }

    }
}
