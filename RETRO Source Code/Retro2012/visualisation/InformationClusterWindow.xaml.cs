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
using System.IO;
using System.Collections.ObjectModel;

using Retro.Model;
using Polytech.Clustering.Plugin;

namespace RetroGUI.visualisation
{
    /// <summary>
    /// Logique d'interaction pour InformationClusterWindow.xaml
    /// </summary>
    public partial class InformationClusterWindow : UserControl
    {

        #region Attributes

        private int index = 0;

        private ObservableCollection<APattern> AllShapes = new ObservableCollection<APattern>();

        private List<Cluster> _OriginalClusters = new List<Cluster>();

        private List<Cluster> _Clusters = new List<Cluster>();

        private List<APattern> _Shapes;
        /// <summary>
        /// ShapeEoC list
        /// </summary>
        public List<APattern> Shapes
        {
            get { return _Shapes; }
            set { _Shapes = value; }

        }

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

        private Cluster _CurrentCluster;
        /// <summary>
        /// Current selected Cluster
        /// </summary>
        public Cluster CurrentCluster
        {
            get { return _CurrentCluster; }
            set { _CurrentCluster = value; }

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
        public InformationClusterWindow()
        {
            InitializeComponent();

        }

        #endregion



        /// <summary>
        /// Update the view regarding the current cluster that is considered
        /// </summary>
        public void UpdateView()
        {
            this.labelClusterno.Content = this.CurrentCluster.Id;
            this.labelNbshapes.Content = this.CurrentCluster.Patterns.Count;
            this.textBoxLabel.Text = (this.CurrentCluster.LabelList.Count != 0) ? this.CurrentCluster.LabelList.ElementAt(0) : "[" + this.CurrentCluster.Id + "]";
            this.textBoxLabel.Focus();
            this.textBoxLabel.SelectAll();

            // Update representative thumbnail
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            ShapeEoC tempShapeEoC = (ShapeEoC)this.CurrentCluster.Representatives[0];
            //Console.Out.WriteLine(tempShapeEoC.PathToFullImage);
            if (File.Exists(tempShapeEoC.PathToFullImage))
                src.UriSource = new Uri(tempShapeEoC.PathToFullImage, UriKind.Absolute);
            //if (File.Exists(this.CurrentCluster.Patterns[0].ToString()))
                //src.UriSource = new Uri(this.CurrentCluster.Patterns[0].ToString(), UriKind.Absolute);
            else
                src.UriSource = new Uri("/resources/LogoLI.png", UriKind.Relative);
            src.EndInit();

            this.imageClusterRepresentative.Source = src;
            this.imageClusterRepresentative.Stretch = Stretch.Uniform;

            //debu jy ,this.Shapes = this.CurrentCluster.Patterns;
            this.listBoxShapeCluster.ItemsSource = AllShapes;

            this.Shapes = this.CurrentCluster.Patterns;
            index = 0;
            UpdateBindingAllShapes();

        }


        public void AddView()
        {
            InformationClusterWindow ICW = new InformationClusterWindow();
            ICW.labelClusterno.Content = this.CurrentCluster.Id;
            ICW.labelNbshapes.Content = this.CurrentCluster.Patterns.Count;
            ICW.textBoxLabel.Text = (this.CurrentCluster.LabelList.Count != 0) ? this.CurrentCluster.LabelList.ElementAt(0) : "[" + this.CurrentCluster.Id + "]";
            ICW.textBoxLabel.Focus();
            ICW.textBoxLabel.SelectAll();

            // Update representative thumbnail
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            string pathh = "ICW.CurrentCluster.RepresentativePathToBitmap";
            if (File.Exists(pathh))
                src.UriSource = new Uri(pathh, UriKind.Absolute);
            else
                src.UriSource = new Uri("/resources/LogoLI.png", UriKind.Relative);
            src.EndInit();

            ICW.imageClusterRepresentative.Source = src;
            ICW.imageClusterRepresentative.Stretch = Stretch.Uniform;

            ICW.Shapes = this.CurrentCluster.Patterns;
            ICW.listBoxShapeCluster.ItemsSource = AllShapes;

            this.Shapes = this.CurrentCluster.Patterns;
            index = 0;
            UpdateBindingAllShapes();
            ICW.Visibility = Visibility.Visible;
            

        }

        ///// <summary>
        ///// Update the 12 shapes of the cluster that are displayed
        ///// </summary>
        //private void UpdateBindingTwelveShapes()
        //{
        //    AllShapes.Clear();
        //    for (int i = index; i < Math.Min(Shapes.Count, index + 12); i++)
        //    {
        //        AllShapes.Add(Shapes.ElementAt(i));
        //    }
        //}

        /// <summary>
        /// Update the all shapes of the cluster that are displayed
        /// </summary>
        private void UpdateBindingAllShapes()
        {
            AllShapes.Clear();
            for (int i = index; i <Shapes.Count; i++)
            {
                AllShapes.Add(Shapes.ElementAt(i));
            }
        }


       
    }
}
