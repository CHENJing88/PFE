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

using System.Windows.Forms;
using System.IO;
using System.Collections.ObjectModel;

using AvalonDock;

using RetroGUI.visualisation;
using Retro.Model;
using RetroGUI.clustering;
using Retro.ViewModel;
using Polytech.Clustering.Plugin;


namespace RetroGUI.main
{
    /// <summary>
    /// Define the Modify Cluster window and the associated actions
    /// </summary>
    public partial class ModifyClusters : Window
    {
        #region Attributes

        private List<System.Windows.Controls.ListBox> listOfListBoxInWindow;
        private List<Cluster> listOfClusterInWindow;
        private List<APattern> listOfShapeInCluster;
        public ObservableCollection<APattern> MyItems { get; set; }
        private static List<object> listStaticOfShapeInWorkspace = new List<object>();
        public static Window Instance = null;
        string workspaceFilePath;

        #endregion

        #region Constructor
        public ModifyClusters()
        {
            InitializeComponent();
            listOfListBoxInWindow = new List<System.Windows.Controls.ListBox>();
            listOfClusterInWindow = new List<Cluster>();
            listOfShapeInCluster = new List<APattern>();

            this.DataContext = this;
            Instance = this;
            workspaceFilePath = null;

            //Permit to not loose the cluster that are in wokspace if the user 
            //open/close/open/close/etc... the Modify Cluster window
            foreach (object el in listStaticOfShapeInWorkspace)
            {
                this.listBoxWorkspace.Items.Add(el);
            }
        }


        #endregion

        #region GenerateView

        /// <summary>
        /// Generate a specific view for each clusters to modify
        /// </summary>
        /// <param name="numberOfClustersToModify"></param>
        public void createViewForClustersToModify(int numberOfClustersToModify)
        {
            // Loop for each clusters to modify
            for (int i = 0; i < numberOfClustersToModify; i++)
            {
                //Create a new instance of a displayDetailSClusterForModification view
                DisplayDetailsClusterForModification ddcfm = new DisplayDetailsClusterForModification();

                //Fill the view with the cluster to modify
                ddcfm.CurrentCluster = SelectClustersToModify.listClustersToModify[i];
                ddcfm.labelClusterID.Content = "Cluster No : " + ddcfm.CurrentCluster.Id;
                ddcfm.UpdateView();

                listOfListBoxInWindow.Add(ddcfm.listBoxShapeCluster);
                listOfClusterInWindow.Add(ddcfm.CurrentCluster);

                //Create column to separate each clusters to modify
                ColumnDefinition col = new ColumnDefinition();
                gridFirstBorder.ColumnDefinitions.Add(col);

                Grid.SetColumn(ddcfm, i);
                gridFirstBorder.Children.Add(ddcfm);


            }

            this.Show();

        }
        #endregion

        #region Button
        /// <summary>
        /// Permit to save the modifications of the clusters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            List<APattern> listShapesInWorkspace = new List<APattern>();


            for (int i = 0; i < listOfListBoxInWindow.Count(); i++)
            {
                listOfClusterInWindow[i].Patterns.Clear();
                foreach (APattern shape in listOfListBoxInWindow[i].Items)
                {
                    listOfClusterInWindow[i].Patterns.Add(shape);
                }

                //Clear the xml file corresponding to the cluster
                string filePathOftheCurrentCluster = listOfClusterInWindow[i].ToString();
                if (System.IO.File.Exists(filePathOftheCurrentCluster))
                {
                    try
                    {
                        System.IO.File.Delete(filePathOftheCurrentCluster);
                    }
                    catch (System.IO.IOException exception)
                    {
                        Console.WriteLine(exception.Message);
                        return;
                    }
                }

                //Creation of a new xml file for a cluster
                StreamWriter xmlOutput = new StreamWriter(filePathOftheCurrentCluster);
                xmlOutput.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                xmlOutput.WriteLine("<cluster size=\"" + listOfClusterInWindow[i].Patterns.Count + "\" label=\"\">");

                //Add shapes in the cluster xml file
                foreach (APattern shape in listOfClusterInWindow[i].Patterns)
                    xmlOutput.WriteLine("\t<cc id=\"" + shape.IdPart1 + "\"/>");

                xmlOutput.WriteLine("</cluster>");// end of the cluster
                xmlOutput.Close();

            }

            //Management of the workspace
            //workspaceFilePath = new FileInfo(listOfClusterInWindow[0].ClusterFilepath).Directory.FullName + "\\workspace.xml";

            foreach (object element in listBoxWorkspace.Items)
            {
                listShapesInWorkspace.Add((APattern)element);
            }

            if (System.IO.File.Exists(workspaceFilePath))
            {
                try
                {
                    System.IO.File.Delete(workspaceFilePath);
                }
                catch (System.IO.IOException exception)
                {
                    Console.WriteLine(exception.Message);
                    return;
                }
            }

            StreamWriter xmlOutputWorkspace = new StreamWriter(workspaceFilePath);
            xmlOutputWorkspace.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlOutputWorkspace.WriteLine("<cluster size=\"" + listShapesInWorkspace.Count);
            //Shapes insertion
            foreach (APattern shape in listShapesInWorkspace)
            { xmlOutputWorkspace.WriteLine("\t<cc id=\"" + shape.IdPart1 + "\"/>"); }

            xmlOutputWorkspace.WriteLine("</cluster>");// end of workspace cluster
            xmlOutputWorkspace.Close();

            //Message
            System.Windows.MessageBox.Show("The clusters have been saved with success.", "Save done");

        }

        #endregion

        #region Workspace Management

        /// <summary>
        /// Update the view of the worspace regarding a list of shapes
        /// </summary>
        /// <param name="listShapes"></param>
        public void UpdateViewWorkspace(List<APattern> listShapes)
        {
            foreach (object element in listShapes)
            {
                this.listBoxWorkspace.Items.Add(element);
            }

        }

        /// <summary>
        /// Update the view of the workspace ragarding the add of a shape
        /// </summary>
        /// <param name="Shapes"></param>
        public void UpdateViewWorkspace(APattern Shapes)
        {
            this.listBoxWorkspace.Items.Add(Shapes);
            listStaticOfShapeInWorkspace.Add(Shapes);



        }



        /// <summary>
        /// Load the Clusters files and build store the list in memory
        /// </summary>
        public void LoadWorkspaceShapes(bool bIllustrationClustering)
        {
            // To complete

        }

        #endregion

        #region Closing
        /// <summary>
        /// Overloading closed method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void windowModifyClusters_Closed(object sender, EventArgs e)
        {
            this.gridFirstBorder.Children.Clear();
        }

        #endregion


    }
}
