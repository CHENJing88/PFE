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
using System.Windows.Forms;
using System.ComponentModel;

using RetroGUI.visualisation;
using Retro.Model;
using Polytech.Clustering.Plugin;


namespace RetroGUI.main
{
    /// <summary>
    /// Define the window for select clusters to modify
    /// </summary>
    public partial class SelectClustersToModify : Window
    {
        #region Attributes

        int numberOfClustersToModify;
        double ScreenWidth;
        public static List<Cluster> listClustersToModify = new List<Cluster>();
        private List<object> test = new List<object>();

        #endregion

        #region Constructor

        public SelectClustersToModify()
        {
            InitializeComponent();
            numberOfClustersToModify = 0;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            listBoxAllClusters.Items.Clear();
            listBoxClustersToModify.Items.Clear();
            listClustersToModify.Clear();
        }
        #endregion

        #region Buttons

        /// <summary>
        /// Permit to select the clusters we want to modify
        /// Button " >> "
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddToModify_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxAllClusters.SelectedItem != null)
            {
                if (listBoxClustersToModify.Items.Count < 4)
                {
                    listBoxClustersToModify.Items.Add(listBoxAllClusters.SelectedItem);
                    listBoxAllClusters.Items.Remove(listBoxAllClusters.SelectedItem);
                }
                if (listBoxClustersToModify.Items.Count == 4 && listBoxAllClusters.SelectedItem != null)
                {
                    System.Windows.Forms.MessageBox.Show("You cannot choose more of 4 clusters to modify");
                }

            }
        }


        /// <summary>
        /// Permit to cancel a select of a clusters we finally don't want to modify
        /// Button " << "
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemoveToModify_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxClustersToModify.SelectedItem != null)
            {
                listBoxAllClusters.Items.Add(listBoxClustersToModify.SelectedItem);
                listBoxClustersToModify.Items.Remove(listBoxClustersToModify.SelectedItem);
            }

        }

        /// <summary>
        /// Action for cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.listBoxClustersToModify.Items.Clear();
            //this.Visibility = Visibility.Hidden;
            this.Hide();
        }

        /// <summary>
        /// Action for OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            int numberOfClustersToModify = 0;

            if (this.listBoxClustersToModify.Items.Count != 0 && listBoxClustersToModify.Items.Count <= 4)
            {
                foreach (Cluster clusterElement in listBoxClustersToModify.Items)
                {
                    listClustersToModify.Add(clusterElement);
                }

                //Genrate the view for the Modify Cluster window
                ModifyClusters viewModifyClusters = new ModifyClusters();

                numberOfClustersToModify = listBoxClustersToModify.Items.Count;
                viewModifyClusters.createViewForClustersToModify(numberOfClustersToModify);
            }
            listClustersToModify.Clear();
            this.Hide();



        }

        protected virtual void OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        #endregion

    }
        
}
