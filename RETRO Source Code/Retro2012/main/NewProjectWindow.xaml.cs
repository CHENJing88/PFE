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
using System.Diagnostics;
using System.Windows.Forms;
using Retro.ViewModel;
using System.IO;
using System.Threading;
using RetroGUI.util;        //TODO: For FolderBrowserDialog, Find a libray that will allow to avoid use of WinForms!

namespace RetroGUI.main
{
    /// <summary>
    /// Define New Project Window
    /// </summary>
    public partial class NewProjectWindow : Window
    {
        public static string pathClusterDirc;
        /// <summary>
        ///Instance of the ViewModel of the application
        /// </summary> 
        private RetroViewModel retroVM;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="retroVM">Instance of Retro ViewModel</param>
        public NewProjectWindow(RetroViewModel retroVM)
        {
            InitializeComponent();
            this.retroVM = retroVM;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            pathClusterDirc = null;
        }


        #region Event handlers

        /// <summary>
        /// Handle Browse Folder buttons Click Events
        /// Display a FolderBrowserDialog
        /// and update the associated textbox with the selected directory
        /// </summary>
        /*private void Click_Browse_Folder(object sender, RoutedEventArgs e)
        {
            // Get the sender
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
            String buttonName = button.Name;
            String paramName = buttonName.Substring(6, buttonName.Length - 6);

            // Open a dialog for forlder selection
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select the directory for this new Retro project).";
            fbd.ShowNewFolderButton = true;
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            
            fbd.ShowDialog();
            if ((fbd.SelectedPath.Length != 0) && (Directory.Exists(fbd.SelectedPath)))
            {
                // Update textbox with the selected directory
                System.Windows.Controls.TextBox textbox = (System.Windows.Controls.TextBox)GridLayout.FindName("textBox" + paramName);
                textbox.Text = fbd.SelectedPath;
            }
        }
        */

        /// <summary>
        /// Handle Browse File buttons Click Events
        /// Display a FolderDialog
        /// and update the associated textbox with the selected directory
        /// </summary>
        private void Click_Browse_Agora(object sender, RoutedEventArgs e)
        {
            // Get the sender
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
            String buttonName = button.Name;
            String paramName = buttonName.Substring(6, buttonName.Length - 6);

            // Open a dialog for forlder selection
            FolderBrowserDialog fdd = new FolderBrowserDialog();
            fdd.Description = "Select the directory that contains the data to process (that contains the Alto and Images folders).";
            fdd.ShowNewFolderButton = false;
            fdd.RootFolder = Environment.SpecialFolder.Desktop;           
            
            DialogResult result = fdd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                String folderName = fdd.SelectedPath;
                // Update textbox with the selected directory
                System.Windows.Controls.TextBox textbox = (System.Windows.Controls.TextBox)GridLayout.FindName("textBox" + paramName);
                textbox.Text = folderName;
            }
        }


        /// <summary>
        ///  Handle Ok button Click Event
        ///  TODO Use Validation data for user input data!
        /// </summary>
        private void Click_Ok(object sender, RoutedEventArgs e)
        {

            if ((this.textBoxProjectName.Text.Length == 0) || (this.textBoxRetroProjectFolder.Text.Length == 0) || (this.textBoxAgoraProjectFolder.Text.Length == 0))
            {
                System.Windows.MessageBox.Show("Please fill all the paths", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if ((!Directory.Exists(this.textBoxAgoraProjectFolder.Text)) || (!Directory.Exists(this.textBoxRetroProjectFolder.Text)))
            {
                System.Windows.MessageBox.Show("One of the file/folder doen't exists ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Create a new project
                this.retroVM.NewProject(this.textBoxProjectName.Text, this.textBoxRetroProjectFolder.Text, this.textBoxAgoraProjectFolder.Text);
                pathClusterDirc = this.textBoxRetroProjectFolder.Text;
                this.Close();

            }
        }


        /// <summary>
        ///  Handle Cancel button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        private void buttonRetroProjectFile_Click(object sender, RoutedEventArgs e)
        {
            // Get the sender
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
            String buttonName = button.Name;
            String paramName = buttonName.Substring(6, buttonName.Length - 6);

            // Open a dialog for forlder selection
            FolderBrowserDialog fdd = new FolderBrowserDialog();
            fdd.Description = "Select the directory that contains the output data (cluster files and retro project file).";
            fdd.ShowNewFolderButton = true;
            fdd.RootFolder = Environment.SpecialFolder.Desktop;

            DialogResult result = fdd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                String folderName = fdd.SelectedPath;
                // Update textbox with the selected directory
                System.Windows.Controls.TextBox textbox = (System.Windows.Controls.TextBox)GridLayout.FindName("textBox" + paramName);
                textbox.Text = folderName;
            }

        }

    }
}
