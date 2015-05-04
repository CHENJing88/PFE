using Polytech.Clustering.Plugin;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Retro.Treatment;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using System.Collections.ObjectModel;
using RetroGUI.util;
using Retro.ViewModel;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;

namespace RetroGUI.clustering
{
    /// <summary>
    /// Logique d'interaction pour AnalyseCluster.xaml
    /// </summary>
    public partial class AnalyseClusterPanel : UserControl, INotifyPropertyChanged
    {
        #region Attributes
        
        /// <summary>
        /// AnalyseViewModel
        /// </summary>
        public AnalyseViewModel Analyse { get; private set; }
        /// <summary>
        /// RetroViewModel
        /// </summary>
        private RetroViewModel _retroVM;
        /// <summary>
        /// DataSource of PCA plan
        /// </summary>
        private EnumerableDataSource<System.Windows.Point> m_d3DataSource;
        public EnumerableDataSource<System.Windows.Point> D3DataSource
        {
            get
            {
                return m_d3DataSource;
            }
            set
            {
                //you can set your mapping inside the set block as well             
                m_d3DataSource = value;
                NotifyPropertyChanged("D3DataSource");
            }
        }

        #endregion

        #region Constructor

        public AnalyseClusterPanel()
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cluster">Cluster</param>
        /// <param name="retroVM">RetroViewModel</param>
        public AnalyseClusterPanel(Cluster cluster, RetroViewModel retroVM)
        {
            InitializeComponent();
            //set attributes
            Analyse = new AnalyseViewModel(cluster, retroVM);
            _retroVM =retroVM;

            //upload the 3D chart of PCA
            Loaded += new RoutedEventHandler(PlotterPCA_Loaded);

            //upload all the DataGrid
            UpLoadData();
        }

        /// <summary>
        /// Upload all the dataGrid (Cluster Resultat, Info of Patterns)
        /// </summary>
        private void UpLoadData()
        {
            //upload the info of analyse cluster result
            this.DataGridAnalyseResult.ItemsSource = Analyse.ClusterAnalyseData;
            
            //upload the info of Pattern
            UpLoadPatternsInfo();
        }

        #endregion

        #region Override Handler
        /// <summary>
        /// Override MouseLeftButtonUp event of point on the plan PCA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void marker_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point mousePos = mouseTrack.Position;
            var transform = this.PlotterACP.Viewport.Transform;
            System.Windows.Point mousePosInData = mousePos.ScreenToData(transform);
            
            //research the point of PlotterPCA in the DataGridInfoPatterns
            var patternItem=Analyse.Plotter_PatternItem_Conn(mousePosInData);

            //set selected item visible in the DataGridInfoPatterns
            if (patternItem != null)
            {
                this.DataGridInfoPatterns.UpdateLayout();
                this.TabItemPatternsInfo.Visibility = Visibility.Visible;
                this.DataGridInfoPatterns.SelectedItem = patternItem;
                this.DataGridInfoPatterns.ScrollIntoView(patternItem);
            }
        }

        #endregion

        #region PCA Plotter
        /// <summary>
        /// load the 3D plan PCA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlotterPCA_Loaded(object sender, RoutedEventArgs e)
        {
            //load data of plotter
            LoadPlotter();

            // update the MenuItem of Plotter 3D chart
            PlotterACP.ContextMenu.ItemsSource = Analyse.PlotterMenuItem((ObservableCollection<object>)PlotterACP.ContextMenu.ItemsSource);
            
            //attach mouse actions
            PlotterAttached();
        }
        /// <summary>
        /// load data of plotter PCA
        /// </summary>
        private void LoadPlotter()
        {
            // Add the graph. Colors are not specified and chosen random
            /*PlotterACP.AddLineGraph(D3DataSource,
              new System.Windows.Media.Pen(System.Windows.Media.Brushes.White, 0),
              PointsMarker,//CirclePointMarker TrianglePointMarker CircleElementPointMarker
              null//new PenDescription("Patterns Projection")
              );*/
            //graphPatterns = plotter.AddLineGraph(D3DataSource, Colors.Blue, 2, "Patterns");
            D3DataSource = Analyse.PlotterPCA_DataSource();
            ElementMarkerPointsGraph PointsGraph = new ElementMarkerPointsGraph();
            PointsGraph.DataSource = D3DataSource;
            ACPElementPointMarker PointsMarker = new ACPElementPointMarker 
            { 
                Size = 5.0, 
                Fill = System.Windows.Media.Brushes.Red,
            };
           
            PointsGraph.Marker = PointsMarker;
            
            PlotterACP.Children.Add(PointsGraph);
            // Force everyting to fit in view
            PlotterACP.Viewport.FitToView();

            
        }

        /// <summary>
        /// Add the action of mouse on the 3D chart
        /// </summary>
        private CursorCoordinateGraph mouseTrack = new CursorCoordinateGraph();
        private void PlotterAttached()
        {
            if (!PlotterACP.Children.Contains(mouseTrack))
            {
                PlotterACP.Children.Add(mouseTrack);
                PlotterACP.MouseLeftButtonUp += new MouseButtonEventHandler(marker_MouseLeftButtonUp);
            }
            //.MouseMove += new MouseEventHandler(GridVisualCluster_MouseMove);
        }
        #endregion
       
        #region Analyse Info of Cluster
        /// <summary>
        /// generate the vertical columns of analyse info datagrid 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new RotateTransform(90));
            foreach (DataGridColumn dataGridColumn in this.DataGridAnalyseResult.Columns)
            {
                if (dataGridColumn is DataGridTextColumn)
                {
                    DataGridTextColumn dataGridTextColumn = dataGridColumn as DataGridTextColumn;

                    Style style = new Style(dataGridTextColumn.ElementStyle.TargetType, dataGridTextColumn.ElementStyle.BasedOn);
                    style.Setters.Add(new Setter(TextBlock.MarginProperty, new Thickness(0, 2, 0, 2)));
                    style.Setters.Add(new Setter(TextBlock.LayoutTransformProperty, transformGroup));
                    style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center));

                    Style editingStyle = new Style(dataGridTextColumn.EditingElementStyle.TargetType, dataGridTextColumn.EditingElementStyle.BasedOn);
                    editingStyle.Setters.Add(new Setter(TextBox.MarginProperty, new Thickness(0, 2, 0, 2)));
                    editingStyle.Setters.Add(new Setter(TextBox.LayoutTransformProperty, transformGroup));
                    editingStyle.Setters.Add(new Setter(TextBox.HorizontalAlignmentProperty, HorizontalAlignment.Center));

                    dataGridTextColumn.ElementStyle = style;
                    dataGridTextColumn.EditingElementStyle = editingStyle;
                }
            }

            List<DataGridColumn> dataGridColumns = new List<DataGridColumn>();
            foreach (DataGridColumn dataGridColumn in this.DataGridAnalyseResult.Columns)
            {
                dataGridColumns.Add(dataGridColumn);
            }
            this.DataGridAnalyseResult.Columns.Clear();
            dataGridColumns.Reverse();
            foreach (DataGridColumn dataGridColumn in dataGridColumns)
            {
                this.DataGridAnalyseResult.Columns.Add(dataGridColumn);
            }
        }


        #endregion

        #region Info of Patterns
        
        /// <summary>
        /// Upload the image and the info of all the pattern 
        /// </summary>
        private void UpLoadPatternsInfo()
        {
            this.DataGridInfoPatterns.ItemsSource = Analyse.PatternAnalyseData; // Analyse.LoadPatternInfo();
            this.DataGridInfoPatterns.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid_LoadingRow);
            if(this.ComboxClusterNb.Items.Count==0)
                LoadClusterNb_Combox();
        }

        /// <summary>
        /// add the automatically increased No. of row 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        void LoadClusterNb_Combox()
        {
            List<Cluster> clusterList = _retroVM.GetClusters();
            this.ComboxClusterNb.Items.Add("NewCluster");
            //add the combobox of cluster label
            for (int i = 0; i < clusterList.Count; i++)
            {
                this.ComboxClusterNb.Items.Add(clusterList[i].Id+")"+ ((clusterList[i].LabelList.Count != 0) ? clusterList[i].LabelList.ElementAt(0) : "[" + clusterList[i].Id + "]"));
            }
            
            this.ComboxClusterNb.SelectedIndex = -1;
        }
        /// <summary>
        /// combox of cluster id+label list change action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboxClusterNb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ComboxClusterNb.SelectedItem != null)
            {
                if (this.ComboxClusterNb.SelectedItem.ToString() == "NewCluster")
                {
                    this.LabelNewClusterId.Visibility = Visibility.Visible;
                    this.LabelNewClusterLabel.Visibility = Visibility.Visible;
                    this.TextBoxNewClusterLabel.Visibility = Visibility.Visible;
                    this.TextBoxId.Visibility = Visibility.Visible;
                    this.TextBoxNewClusterLabel.IsEnabled = true;
                    this.TextBoxId.IsEnabled = true;
                }
                else
                {
                    this.LabelNewClusterId.Visibility = Visibility.Hidden;
                    this.LabelNewClusterLabel.Visibility = Visibility.Hidden;
                    this.TextBoxNewClusterLabel.Visibility = Visibility.Hidden;
                    this.TextBoxId.Visibility = Visibility.Hidden;
                    this.TextBoxNewClusterLabel.IsEnabled = false;
                    this.TextBoxId.IsEnabled = false;
                }
            }
        }
        #endregion

        #region Buttons
        /// <summary>
        /// clear the panel Analyse Cluster: the plan, the datagride of AnalyseResult and InfoPatterns
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClearAnalyse_Click(object sender, RoutedEventArgs e)
        {
            //plotter.Children.RemoveAll<MarkerPointsGraph>(typeof(Point));
            ClearPlotter();

            this.DataGridAnalyseResult.DataContext = null;
            this.DataGridAnalyseResult.ItemsSource = null;
            //ClusterAnalyseData = new ObservableCollection<AnalyseItem>();

            this.DataGridInfoPatterns.DataContext = null;
            this.DataGridInfoPatterns.ItemsSource = null;
            //PatternAnalyseData = new ObservableCollection<PatternItem>();
            
        }
        /// <summary>
        /// Clear the component of PlotterACP
        /// </summary>
        private void ClearPlotter()
        {
            foreach (var elem in this.PlotterACP.Children.Where(e => e is LineGraph || e is MarkerPointsGraph||e is ElementMarkerPointsGraph).ToArray())
            {
                this.PlotterACP.Children.Remove(elem);
            }
            this.D3DataSource = null;
        }

        /// <summary>
        /// the button of Delet the pattern in the DataGrid of pattern list and Plotter of PCA
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDeletPattern_Click(object sender, RoutedEventArgs e)
        {
            var SelectObjet = DataGridInfoPatterns.SelectedCells[0].Item;
            if (SelectObjet != null)
            {
                MessageBoxResult result=MessageBox.Show("Do you really want to delet this pattern?", "Notification", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    Analyse.DeletPattern(SelectObjet);
                    //this.DataGridInfoPatterns.SelectedItems.Clear();
                    
                    ClearPlotter();

                    LoadPlotter();

                    UpLoadData();
                    MessageBox.Show("The Pattern has been deleted.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                
                }
            }
            else
                MessageBox.Show("Please select a Pattern.", "Notification", MessageBoxButton.OK, MessageBoxImage.Question);
       }
        /// <summary>
        /// the button of Send pattern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSendModify_Click(object sender, RoutedEventArgs e)
        {
            String IdClusterSend = this.ComboxClusterNb.SelectedItem.ToString();
            
            
            if (DataGridInfoPatterns.SelectedCells!= null)
            {
                var PatternSend = DataGridInfoPatterns.SelectedCells[0].Item;

                MessageBoxResult result=MessageBox.Show("Do you really want to send this pattern ?", "Notification", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    String NewClusterLabel; 
                    String NewClusterId;
                    if (IdClusterSend == "NewCluster")
                    {
                        NewClusterLabel = this.TextBoxNewClusterLabel.Text.Trim();
                        NewClusterId = this.TextBoxId.Text.Trim();
                    }
                    else
                    {
                        NewClusterLabel = null; NewClusterId = null;
                    }
                    Analyse.SendPattern(IdClusterSend, NewClusterId, NewClusterLabel, PatternSend, _retroVM.RetroInstance.ClusteringPath);
                    
                    ClearPlotter();

                    LoadPlotter();

                    UpLoadData();
                    MessageBox.Show("The Pattern has been sent.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
                MessageBox.Show("Please select a Pattern.", "Notification", MessageBoxButton.OK, MessageBoxImage.Question);
        }

        #endregion

       
        /// <summary>
        /// For binding purpose
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        

        /// <summary>
        /// For binding purpose
        /// </summary>
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

       
    }
}
