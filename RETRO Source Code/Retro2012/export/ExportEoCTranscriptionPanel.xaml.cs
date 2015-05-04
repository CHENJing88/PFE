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
using System.Windows.Forms;
using System.IO;     // NOT WPF, BUT NEEDED FOR FolderBrowserDialog

namespace RetroGUI.export
{
    /// <summary>
    /// Define EoC TranscriptionPanel Exportation Panel
    /// </summary>
    public partial class ExportEoCTranscriptionPanel : System.Windows.Controls.UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="altoFilesDir">Path of alto files directory</param>
        /// <param name="outputAnnotationsTopDir">Output annotations topDirectory</param>
        public ExportEoCTranscriptionPanel(String altoFilesDir, String outputAnnotationsTopDir)
        {
            InitializeComponent();

            this.AltoXMLFilesDirectoryTextBox.Text = altoFilesDir;
            this.AnnotationFilesDirectoryTextBox.Text = outputAnnotationsTopDir;
        }


        #region Events Handlers

        /// <summary>
        /// Handler for Select Directory button
        /// </summary>
        private void Click_Browse_Folder(object sender, RoutedEventArgs e)
        {
            // Open a dialog for forlder selection
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();

            if ((fbd.SelectedPath.Length != 0) && (Directory.Exists(fbd.SelectedPath)))
            {
                // Get the sender
                System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
                String buttonName = button.Name;
                String senderName = buttonName.Substring(0, buttonName.Length - 6) + "TextBox";

                // Update textbox with the selected directory
                System.Windows.Controls.TextBox textbox = (System.Windows.Controls.TextBox)GridLayout.FindName(senderName);
                textbox.Text = fbd.SelectedPath;
            }
        }


        /// <summary>
        /// Handler for Export button
        /// </summary>
        private void Click_Export(object sender, RoutedEventArgs e)
        {
            if ((this.AltoXMLFilesDirectoryTextBox.Text.CompareTo("") == 0) || (this.AnnotationFilesDirectoryTextBox.Text.CompareTo("") == 0))
                System.Windows.MessageBox.Show("Please fill in both required path.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            else if (!Directory.Exists(this.AltoXMLFilesDirectoryTextBox.Text))
                System.Windows.MessageBox.Show("The alto xml files directory does not exists.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            else if (!Directory.Exists(this.AnnotationFilesDirectoryTextBox.Text))
                System.Windows.MessageBox.Show("The output annotations directory does not exists.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                ExportEoCTranscriptionTool.ExportEoCTranscription(this.AltoXMLFilesDirectoryTextBox.Text, this.AnnotationFilesDirectoryTextBox.Text);

            // Notify user
            System.Windows.MessageBox.Show("The EoC transcription has been exported as annotations", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
       
        }

        #endregion
    }
}
