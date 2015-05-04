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
using System.Collections.ObjectModel;

using Retro.Model;
using RetroGUI.util;
using Polytech.Clustering.Plugin;

namespace RetroGUI.visualisation
{
    /// <summary>
    /// Define Clusters Panel
    /// </summary>
    public partial class ClustersPanel : UserControl
    {

        #region Attributes

        private int index = 0;
        //List of Clusters from the project
        private List<Cluster> _OriginalClusters = new List<Cluster>();
        private ObservableCollection<Cluster> TwentyEightClusters = new ObservableCollection<Cluster>();


        private List<Cluster> _Clusters = new List<Cluster>();
        /// <summary>
        /// List of Clusters to consider
        /// </summary>
        public List<Cluster> Clusters
        {
            get { return _Clusters; }
            set {
                _OriginalClusters = value;
                _Clusters = value; }
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
        private ClusterWindow cw;


        #endregion


        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ClustersPanel()
        {
            InitializeComponent();

            this.cw = new ClusterWindow();
            this.cw.Visibility = Visibility.Hidden;

        }


        #endregion


        #region Events Handlers

        /// <summary>
        /// Handler for listbox selection change
        /// </summary>
        private void Cluster_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            Object obj = (sender as ListBox).SelectedItem ;
            Cluster cluster = (Cluster)obj;

            if (cluster != null)
            {
                if (cw.HasBeenClosed)
                    this.cw = new ClusterWindow();

                if (cluster != null)
                {
                    this.cw.CurrentCluster = cluster;
                    cw.UpdateView();
                }

                cw.Show();
                cw.Topmost = true;
            }
        }


        /// <summary>
        /// Handler for Labelized/NotLabelized ComboBoxes OnCheck/OnUnchecked
        /// </summary>
        private void ClusterPanel_ClusterSelection_CheckBoxes_Handler(object sender, RoutedEventArgs e)
        {
            if ((this.ClusterPanelLabelizedCheckBox != null) && (this.ClusterPanelNotLabelizedCheckBox != null))
            {
                this.UpdateClusters((bool)this.ClusterPanelLabelizedCheckBox.IsChecked, (bool)this.ClusterPanelNotLabelizedCheckBox.IsChecked);
            }
        }


        /// <summary>
        /// Handler for combo box selection change   
        /// </summary>
        private void SortMethod_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            this.SortClusters();
            this.UpdateBinding();
        }


        /// <summary>
        /// Handler for Previous Button Click
        /// </summary>
        private void PreviousClusters_Click(object sender, System.EventArgs e)
        {
            index = (index >= 28) ? index - 28 : 0;
            UpdateBinding();
        }


        /// <summary>
        /// Handler for Next Button Click
        /// </summary>
        private void NextClusters_Click(object sender, System.EventArgs e)
        {
            index = (index < this.Clusters.Count - 28) ? index + 28 : index;
            UpdateBinding();
        }

        #endregion


        #region Cluster manipulation methods

        /// <summary>
        /// Update the clusters list
        /// </summary>
        /// <param name="labelized">Indicate if labelized clusters have to be displayed</param>
        /// <param name="notLabelized">Indicate if not labelized clusters have to be displayed</param>
        private void UpdateClusters(bool labelized, bool notLabelized)
        {
            // Update clusters list
            if (labelized)
            {
                if(notLabelized)
                {
                    // Labelized + NotLabalized
                    this._Clusters = this._OriginalClusters;
                }
                else
                {
                    // Only Labelized
                    this._Clusters = this._OriginalClusters.FindAll(
                        delegate(Cluster cluster)
                        {
                            return cluster.IsLabelized;
                        }
                    );
                }
            }
            else
            {
                if (notLabelized)
                {
                    // Only NotLabelized
                    this._Clusters = this._OriginalClusters.FindAll(
                        delegate(Cluster cluster)
                        {
                            return !cluster.IsLabelized;
                        }
                    );
                }
                else
                {
                    // None
                    this._Clusters.Clear();
                }
            }

            // sort Computed list
            this.SortClusters();

            // Update binding
            this.UpdateBinding();

        }


        /// <summary>
        /// Sort the clusters
        /// </summary>
        private void SortClusters()
        {
            int sortMethod = 0;
            IComparer<Cluster> comparer = null;

            if (this.Clusters_comboBox != null)
                sortMethod = this.Clusters_comboBox.Items.IndexOf(this.Clusters_comboBox.SelectedItem);

            switch (sortMethod)
            {
                case 0:
                    comparer = new SortbyIdAsc();
                    break;
                case 1:
                    comparer = new SortbyIdDesc();
                    break;
                case 2:
                    comparer = new SortbyShapeNumberAsc();
                    break;
                case 3:
                    comparer = new SortbyShapeNumberDesc();
                    break;
                default:
                    break;
            }

            this._Clusters.Sort(comparer);
        }


        /// <summary>
        /// Display the first 28 clusters representative thumbnail
        /// </summary>
        public void DisplayClusters()
        {
            // Default sort by Asc ID
            this.Clusters.Sort(new SortbyIdAsc());

            this.index = 0;
            this.Cluster_listBox.ItemsSource = this.TwentyEightClusters;
            this.UpdateBinding();


            // Enable bottom controls
            this.ClusterPanelLabelizedCheckBox.IsEnabled = true;
            this.ClusterPanelNotLabelizedCheckBox.IsEnabled = true;
            this.Clusters_comboBox.IsEnabled = true;
            this.button_previous_clusters.IsEnabled = true;
            this.button_next_clusters.IsEnabled = true;

        }


        /// <summary>
        /// Update the 28 cluster that are displayed
        /// </summary>
        private void UpdateBinding()
        {
            TwentyEightClusters.Clear();
            for (int i = this.index; i < Math.Min(this.Clusters.Count, this.index + 28); i++)
            {
                TwentyEightClusters.Add(Clusters.ElementAt(i));
            }

            // Update buttons tooltip
            if (this.button_previous_clusters != null)
            {
                int page = (int)this.index / 28 + 1;
                int pages = (int)this.Clusters.Count / 28 + 1;

                this.button_previous_clusters.ToolTip = (page > 1)? Convert.ToString(page-1) + "/" + pages : "-";
                this.button_next_clusters.ToolTip = (page < pages)? Convert.ToString(page+1) + "/" + pages : "-";
            }
        }

        #endregion


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
            this.TwentyEightClusters.Clear();

            // Disable ClustersPanel Controls
            this.ClusterPanelLabelizedCheckBox.IsEnabled = false;
            this.ClusterPanelNotLabelizedCheckBox.IsEnabled = false;
            this.Clusters_comboBox.IsEnabled = false;
            this.button_previous_clusters.IsEnabled = false;
            this.button_next_clusters.IsEnabled = false;
            this.button_previous_clusters.ToolTip = "";
            this.button_previous_clusters.ToolTip = "";

        }
    }
}
