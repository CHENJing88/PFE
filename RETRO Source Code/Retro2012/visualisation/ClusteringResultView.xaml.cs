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
using System.Collections.ObjectModel;

using RetroUtil;
using Polytech.Clustering.Plugin;
using RetroGUI.util;
using RetroGUI.main;




namespace RetroGUI.visualisation
{
    /// <summary>
    /// Generate the result view after a clustering process
    /// </summary>
    public partial class ClusteringResultView : UserControl
    {
        #region Attributes

        int numberofClusterPreSelectToModify;
        SelectClustersToModify viewSelectClustersToModify = new SelectClustersToModify();
        InformationClusterWindow ICW = new InformationClusterWindow();
        // TemplateMatchingParameters templateMatchingParameter = new TemplateMatchingParameters();
        private ClustersGlobalView clustersGlobalView = new ClustersGlobalView();

        private ObservableCollection<Cluster> allClusters = new ObservableCollection<Cluster>();


        public List<object> listAllcluster = new List<object>();
        public List<object> listClusterForListBoxClusterCandidateForModification = new List<object>();
        public List<object> listClustersForListBoxToModify = new List<object>();


        private int index = 0;

        //List of Clusters from the project
        private List<Cluster> _OriginalClusters = new List<Cluster>();

        private ObservableCollection<Cluster> EightClusters = new ObservableCollection<Cluster>();
        private List<Cluster> _Clusters = new List<Cluster>();

        /// <summary>
        /// List of Clusters to consider
        /// </summary>
        public List<Cluster> Clusters
        {
            get { return _Clusters; }
            set
            {
                _OriginalClusters = value;
                _Clusters = value;
            }
        }

        private Cluster _SelectedCluster;

        /// <summary>
        /// Selected Cluster
        /// </summary>
        public Cluster SelectedCluster
        {
            get { return _SelectedCluster; }
            set { _SelectedCluster = value; }
        }

        /// <summary>
        /// Cluster Windows
        /// </summary>
        // A modifier -> nouvelle fenetre en cliquant sur un cluster 
        private ClusterWindow cw;

        #endregion

        #region Constructor
        public ClusteringResultView()
        {
            InitializeComponent();
            this.cw = new ClusterWindow();
            this.cw.Visibility = Visibility.Hidden;
            numberofClusterPreSelectToModify = 0;
            this.groupBoxDisplay.Content = this.GridInGroupBoxDisplay;
            foreach (object obj in allClusters)
            {
                listClusterForListBoxClusterCandidateForModification.Add(obj);
            }

        }

        #endregion

        #region Buttons

        /// <summary>
        /// Permit to display the globla view of the clusters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGlobalView_Click(object sender, RoutedEventArgs e)
        {
            clustersGlobalView.DisplayClusters();
        }

        /// <summary>
        /// Permit to display a window in order to select the cluster to modify
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonModifyClusters_Click(object sender, RoutedEventArgs e)
        {
            viewSelectClustersToModify.listBoxAllClusters.Items.Clear();
            viewSelectClustersToModify.listBoxClustersToModify.Items.Clear();

            //Fill the respectives ListBoxc
            foreach (object element in listClusterForListBoxClusterCandidateForModification)
            {
                viewSelectClustersToModify.listBoxAllClusters.Items.Add(element);
            }
            foreach (object element in listClustersForListBoxToModify)
            {
                viewSelectClustersToModify.listBoxClustersToModify.Items.Add(element);
            }
            viewSelectClustersToModify.Visibility = Visibility.Visible;

        }

        #endregion

        #region ClusterDisplayManagement

        /// <summary>
        /// Handler for listbox selection change
        /// </summary>
        private void Cluster_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Object obj = (sender as ListBox).SelectedItem;
            Cluster cluster = (Cluster)obj;
            SelectedCluster = cluster;

            if (this.GridInGroupBoxDisplay.Children != null)
            {
                this.GridInGroupBoxDisplay.Children.Remove(ICW);
            }

            //if ((cluster != null)||cluster.NbPatterns!=0)
            if (cluster != null)
            {
                double sizeHeight = this.GridInGroupBoxDisplay.ActualHeight;
                double sizeWidth = this.GridInGroupBoxDisplay.ActualWidth;
                ICW.Height = sizeHeight;
                ICW.Width = sizeWidth;

                ICW.CurrentCluster = cluster;
                ICW.UpdateView();

                this.GridInGroupBoxDisplay.Children.Add(ICW);
            }

            
        }


        /// <summary>
        /// Display all clusters representative thumbnail
        /// </summary>
        public void DisplayClusters()
        {
            // Default sort by Asc ID
            this.Clusters.Sort(new SortbyIdAsc());

            this.index = 0;
            this.Cluster_listBox.ItemsSource = this.allClusters;
            this.UpdateBinding();
            foreach (object element in Cluster_listBox.Items)
            {
                listClusterForListBoxClusterCandidateForModification.Add(element);
            }


        }

        /// <summary>
        /// Update all the clusters
        /// </summary>
        private void UpdateBinding()
        {
            allClusters.Clear();
            for (int i = this.index; i < (this.Clusters.Count); i++)
            {
                allClusters.Add(Clusters.ElementAt(i));
            }

        }

        /// <summary>
        /// Reset the attributes of the panel
        /// </summary>
        public void Reset()
        {
            // ClearClustersList the ClusterWindow
            this.cw.Visibility = Visibility.Hidden;
            this.cw.Close();

            // Clear the cluster lists
            this.index = 0;
            this._OriginalClusters.Clear();
            this._OriginalClusters = null;
            this._Clusters.Clear();
            this._Clusters = null;
            this.allClusters.Clear();

        }

        #endregion

        #region RightClic on a cluster
        /// <summary>
        /// Action for the MenuItem "Add To Modify". Add a cluster to a list of "pré-select" cluster for modification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemAddToModify_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.Source as MenuItem;
            if (this.Cluster_listBox.SelectedItem != null)
            {
                if (menuItem.IsChecked == true)
                {
                    numberofClusterPreSelectToModify++;
                    if (numberofClusterPreSelectToModify <= 4)
                    {
                        listClustersForListBoxToModify.Add(Cluster_listBox.SelectedItem);
                        listClusterForListBoxClusterCandidateForModification.Remove(Cluster_listBox.SelectedItem);

                    }
                    else
                    {
                        MessageBox.Show("You can't select more than 4 clusters to modify.\n Please select only 4 clusters maximum for modification", "Warning");
                        menuItem.IsChecked = false;
                        numberofClusterPreSelectToModify--;
                    }
                }
                else
                {
                    numberofClusterPreSelectToModify--;
                    listClustersForListBoxToModify.Remove(Cluster_listBox.SelectedItem);
                    listClusterForListBoxClusterCandidateForModification.Add(Cluster_listBox.SelectedItem);
                }
            }

        }
        #endregion

    }
}
