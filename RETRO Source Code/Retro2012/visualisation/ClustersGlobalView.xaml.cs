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

using AvalonDock;
using RetroGUI.clustering;
using RetroGUI.transcription;
using RetroGUI.typography;
using RetroGUI.util;
using RetroGUI.main;
using RetroUtil;
using Polytech.Clustering.Plugin;

using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using Retro.Model;
using RetroGUI.export;
using System.ComponentModel;
using System.Windows.Forms;


namespace RetroGUI.visualisation
{
    /// <summary>
    /// Logique d'interaction pour ClustersGlobalView.xaml
    /// </summary>
    public partial class ClustersGlobalView : Window
    {

        #region Attributes

        //Attributes
        private DocumentContent clusteringDocumentContent = null;
        private DocumentContent manualTranscriptionDocumentContent = null;
        private DocumentContent exportEoCTranscriptionDocumentContent = null;
        public bool bIllustrationClustering = false;
        


        #endregion

        #region Constructor
        public ClustersGlobalView()
        {
            InitializeComponent();
        }
        #endregion

        #region Project Information & Data Display Methods
        /// <summary>
        /// Display the Clusters of this project
        /// </summary>
        public void DisplayClusters()
        {
            //Enable the view
            this.Show();
            // Assign the clusters list
            List<Cluster> clustersList = MainWindow._retroVM.GetClusters();
            this.ClustersDisplay.Clusters = clustersList;
            this.ClustersDisplay.DisplayClusters();
            
        }

        #endregion

        #region Utils
        /// <summary>
        /// Redefine closing function to hide the window instead of close it (in order to permit to reopen it)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClustersGlobalView_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
        #endregion

    }
}
