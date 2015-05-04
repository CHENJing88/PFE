using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Polytech.Clustering.Plugin;
using Retro.Treatment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Retro.ViewModel
{
    /// <summary>
    /// AnalyseViewModel is the ViewModel for the panel AnalyseClusterPanel
    /// </summary>
    public class AnalyseViewModel : INotifyPropertyChanged
    {
        #region Attributes
       
        /// <summary>
        /// RetroViewModel
        /// </summary>
        private RetroViewModel _retroViewModel;
        /// <summary>
        /// Instance of the current used RetroProject
        /// </summary>
        public RetroViewModel RetroViewModel
        {
            get { return _retroViewModel; }
            set { _retroViewModel = value; }
        }
        /// <summary>
        /// the class for the method of AnalyseCluster
        /// </summary>
        public AnalyseCluster AnalyseMethod{ get; set; }

        /// <summary>
        /// the data list of point show on the plan PCA
        /// </summary>
        private List<MapInfo> mapInfoList;

        /// <summary>
        /// the cluster in use
        /// </summary>
        private Cluster m_clusterCurrent;
        public Cluster ClusterCurrent
        {
            get { return m_clusterCurrent; }
            set
            {
                m_clusterCurrent = value;
                NotifyPropertyChanged("ClusterCurrent");
                //AnalyseMethod.ClusterCurrent = m_clusterCurrent;
            }
        }
        
        /// <summary>
        /// save the original cluster 
        /// </summary>
        private Cluster m_clusterOrigin;
        public Cluster ClusterOrigin
        {
            get { return m_clusterOrigin; }
            set
            {
                m_clusterOrigin = value;
                NotifyPropertyChanged("ClusterOrigin");
            }
        }
        /// <summary>
        /// ClusterAnalyseData for the datagrid of analyse cluster info
        /// </summary>
        private ObservableCollection<AnalyseItem> m_clusterAnalyseData;
        public ObservableCollection<AnalyseItem> ClusterAnalyseData
        {
            get { return m_clusterAnalyseData; }
            set
            {
                m_clusterAnalyseData = value;
                NotifyPropertyChanged("ClusterAnalyseData");
            }
        }
        /// <summary>
        /// PatternAnalyseData for the datagrid of pattern info
        /// </summary>
        private ObservableCollection<PatternItem> m_patternAnalyseData;
        public ObservableCollection<PatternItem> PatternAnalyseData 
        {
            get { return m_patternAnalyseData; }
            set
            {
                m_patternAnalyseData = value;
                NotifyPropertyChanged("PatternAnalyseData");
            }
        }

        #endregion

        #region Constructor

        public AnalyseViewModel()
        {
            AnalyseMethod = new AnalyseCluster();
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cluster">cluster for analyse</param>
        /// <param name="retroViewModel">RetroViewModel</param>
        public AnalyseViewModel(Cluster cluster, RetroViewModel retroViewModel)
        {
            this.RetroViewModel = retroViewModel;
            this.ClusterCurrent = cluster;
            this.ClusterOrigin = cluster;
            AnalyseMethod = new AnalyseCluster(cluster);

            Initialization();
        }
        /// <summary>
        /// initialize the data of dynamic figure PCA, load the data of pattern list datagrid and load the data of analyse cluster
        /// </summary>
        private void Initialization()
        {
            mapInfoList = new List<MapInfo>();
            PatternAnalyseData = new ObservableCollection<PatternItem>();
            ClusterAnalyseData = new ObservableCollection<AnalyseItem>();
            
            LoadMapInfo(AnalyseMethod.ACP());
            LoadPatternInfo();
            LoadClusterInfo();
        }

        #endregion

        #region PCA Plotter 
        /// <summary>
        /// this structure for the PCA 3d chater(data show on the plan PCA)
        /// </summary>
        public class MapInfo
        {
            public int id { get; set; }
            public System.Windows.Point point { get; set; }


            public MapInfo(int id, System.Windows.Point point)
            {
                this.id = id;
                this.point = point;
            }
        }

        /// <summary>
        /// add the data of PCA projection to the MapInfo(data show on the plan PCA) structure
        /// </summary>
        /// <param name="ComponentMatrix"></param>
        private void LoadMapInfo(double[,] ComponentMatrix)
        {
            for (int i = 0; i < ComponentMatrix.GetLength(0); i++)
            {
                int id = i;
               
                MapInfo mi = new MapInfo(id, new System.Windows.Point(ComponentMatrix[i, 0], ComponentMatrix[i, 1]));
                mapInfoList.Add(mi);
            }
        }
        /// <summary>
        /// load the point data from mapInfoList to the EnumerableDataSource
        /// </summary>
        /// <returns>the data source show on the plan PCA</returns>
        public EnumerableDataSource<System.Windows.Point> PlotterPCA_DataSource()
        {

            //List<MapInfo> mapInfoList = LoadMapInfo("D:\\RETRO2014_Test\\MapInfo.txt");
            //mapInfoList = LoadMapInfo(AnalyseMethod.ACP());
            //LoadMapInfo(AnalyseMethod.ACP());
            /*int[] ids = new int[mapInfoList.Count];
            double[] xs = new double[mapInfoList.Count];
            double[] ys = new double[mapInfoList.Count];

            for (int i = 0; i < mapInfoList.Count; ++i)
            {
                  //ids[i] = mapInfoList[i].id;
                  xs[i] = mapInfoList[i].lon;
                  ys[i] = mapInfoList[i].lat;
            }*/
            System.Windows.Point[] dataCollection = new System.Windows.Point[mapInfoList.Count];
            for (int i = 0; i < mapInfoList.Count; ++i)
            {
                dataCollection[i] = mapInfoList[i].point;
            }
            /*var idsDataSource = new EnumerableDataSource<int>(ids);
            idsDataSource.SetXMapping(x => x);

            var xsDataSource = new EnumerableDataSource<double>(xs);
            xsDataSource.SetXMapping(x => x);

            var ysDataSource = new EnumerableDataSource<double>(ys);
            ysDataSource.SetYMapping(y => y);
            
            CompositeDataSource compositeDataSource = new
              CompositeDataSource(xsDataSource,ysDataSource);*/

            //D3DataSource = new ObservableDataSource<System.Windows.Point>(dataCollection);
            EnumerableDataSource<System.Windows.Point> D3DataSource = new EnumerableDataSource<System.Windows.Point>(dataCollection);
            //mapping the element of dataCollection to plan PCA
            D3DataSource.SetXMapping(x => x.X);
            D3DataSource.SetYMapping(y => y.Y);
            D3DataSource.SetXYMapping(p => p);

            //add the tooltip text of point
            D3DataSource.AddMapping(CircleElementPointMarker.ToolTipTextProperty, s => String.Format("Id: {0}\nX-Data : {1}\nY-Data : {2}", GetIdByPosition(s.X, s.Y), s.X, s.Y));
            
            return D3DataSource;
        }
        /// <summary>
        /// get the id of pattern in the list of mapInfoList(data show on the plan) by its position on the plan PCA
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <returns>id in the list of mapInfoList</returns>
        private string GetIdByPosition(double x, double y)
        {
            int Id = 0;
            Parallel.For(0, mapInfoList.Count, i =>
            //for (int i = 0; i < mapInfoList.Count; i++)
            {
                if ((mapInfoList[i].point.X == x) && (mapInfoList[i].point.Y == y))
                    Id = mapInfoList[i].id;
            });

            return Id.ToString();
        }
        /// <summary>
        /// add a flot menu of PCA plan
        /// </summary>
        /// <param name="observableCollection">flot menu exist of PCA plan</param>
        /// <returns>flot menu</returns>
        public ObservableCollection<object> PlotterMenuItem(System.Collections.ObjectModel.ObservableCollection<object> observableCollection)
        {
            ObservableCollection<object> StaticMenuItems = observableCollection;
            MenuItem SaveMenuItem = new MenuItem
            {
                Header = "Save as .txt",
                IsCheckable = true,

            };
            StaticMenuItems.Add(SaveMenuItem);
            SaveMenuItem.Click += new System.Windows.RoutedEventHandler(this.MenuItemPlotterSaveTxt_Click);

            return StaticMenuItems;
        }
        /// <summary>
        /// the action of click the menuItem Save as .txt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemPlotterSaveTxt_Click(System.Object sender, EventArgs e)
        {
           
        }

        /*private static List<MapInfo> LoadMapInfo(string fileName)
        {
            List<MapInfo> result = new List<MapInfo>();
            FileStream fs = new FileStream(fileName, FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("//"))
                    continue;
                else
                {
                    string[] pieces = line.Split(' ');
                    int id = int.Parse(pieces[0]);
                    double lat = double.Parse(pieces[1]);
                    double lon = -1.0 * double.Parse(pieces[2]);
                    MapInfo mi = new MapInfo(id, lat, lon);
                    result.Add(mi);
                }
            }
            sr.Close();
            fs.Close();
            return result;
        }*/

        #endregion

        #region Analyse Cluster
        /// <summary>
        /// this structure is for the datagrid of panel analyse info
        /// it descripts all the parametters of cluster
        /// </summary>
        public class AnalyseItem
        {
            /// <summary>
            /// id of cluster
            /// </summary>
            public String Id { get; set; }
            /// <summary>
            /// nombre of patterns
            /// </summary>
            public int NbPatterns { get; set; }
            /// <summary>
            /// label of cluster
            /// </summary>
            public String Label { get; set; }
            /// <summary>
            /// average of signature (not fonction!)
            /// </summary>
            public double AverageSignature { get; set; }
            /// <summary>
            /// discriptor of signature
            /// </summary>
            public String DiscriptorSignature { get; set; }

            public AnalyseItem(String id, int nbPattern, String label, double avgSign, String discripSign)
            {
                this.Id = id;
                this.NbPatterns = nbPattern;
                this.Label = label;
                this.AverageSignature = avgSign;
                this.DiscriptorSignature = discripSign;
            }

        }
        /// <summary>
        /// upload the info of Analyse Cluster. Fill the list of ClusterAnalyseData.
        /// </summary>
        /// <returns>ClusterAnalyseData</returns>
        public System.Collections.IEnumerable LoadClusterInfo()
        {
            //ClusterAnalyseData = new ObservableCollection<AnalyseItem>();
            
            ClusterAnalyseData.Add(new AnalyseItem(
                ClusterCurrent.Id,
                ClusterCurrent.NbPatterns,
                (ClusterCurrent.LabelList.Count != 0) ? ClusterCurrent.LabelList.ElementAt(0) : "[" + ClusterCurrent.Id + "]",
                AnalyseMethod.AverageClusterSignature(),
                AnalyseMethod.DescriptorSignature
            ));

            return ClusterAnalyseData;
        }
        
        #endregion

        #region Patterns Info
        /// <summary>
        /// the structure of pattern list in the cluster
        /// </summary>
        public class PatternItem
        {
            /// <summary>
            /// id of pattern
            /// </summary>
            public String Id { get; set; }
            /// <summary>
            /// PCA projection coordinate x
            /// </summary>
            public double ProjectionX { get; set; }
            /// <summary>
            /// PCA projection coordinate y
            /// </summary>
            public double ProjectionY { get; set; }
            /// <summary>
            /// the pattern(for show the image in the panel)
            /// </summary>
            public APattern Pattern { get; set; }

            public PatternItem(String id, double x, double y, APattern pattern)
            {
                this.Id = id;
                this.ProjectionX = x;
                this.ProjectionY = y;
                this.Pattern = pattern;
            }
        }
        
        /// <summary>
        /// load the pattern list info. Fill the list of PatternAnalyseData.
        /// </summary>
        public void LoadPatternInfo()
        {
            //PatternAnalyseData = new ObservableCollection<PatternItem>();
            Parallel.For(0,mapInfoList.Count,i=>
                //(int i = 0; i < mapInfoList.Count; i++)
            {
                PatternAnalyseData.Add(new PatternItem(
                   i.ToString(),
                   mapInfoList[i].point.X,
                   mapInfoList[i].point.Y,
                   this.ClusterCurrent.Patterns[i]
               ))
               ;
            });
            
           // return PatternAnalyseData;
        }


        /// <summary>
        /// connect the point in the plan PCA and the pattern
        /// </summary>
        /// <param name="mousePosInData">the point mouse clicked in the PCA plan</param>
        /// <returns>the nearest patternItem</returns>
        public object Plotter_PatternItem_Conn(Point mousePosInData)
        {
            double offset = 0.5;
            object objectConn=null;

            foreach (PatternItem patternItem in PatternAnalyseData)
            {
                //return the patternItem in the square area 1.0*1.0
                if ((Math.Abs(patternItem.ProjectionX - mousePosInData.X) <= offset) && (Math.Abs(patternItem.ProjectionY - mousePosInData.Y) <= offset))
                {
                    objectConn = patternItem;break;
                }
            }

            return objectConn;
        }

        #endregion

        #region Buttons
        /// <summary>
        /// delet the pattern in the panel of pattern info, the point in the PCA Plotter 
        /// and update the current cluster file .xml
        /// </summary>
        /// <param name="SelectObjet">selected pattern the pattern list</param>
        public void DeletPattern(Object SelectObjet)
        {
            String Id=((PatternItem)SelectObjet).Id;
            
            //delet Pattern in the list of CurrentCluster.Patterns
            ClusterCurrent.RemovePattern(((PatternItem)SelectObjet).Pattern);
            //delet Pattern in the PCA Plotter
               //list of MapInfo
            foreach(MapInfo point in mapInfoList)
            {
                if (point.id.ToString() == Id)
                {
                    mapInfoList.Remove(point); break;
                }
            }
                //list of PatternAnalyseData,D3DataSource,
            PatternAnalyseData.Remove((PatternItem)SelectObjet);

            //update the patterns in the file .xml
            if (ClusterCurrent.NbPatterns != 0)
            {
                ClusterCurrent.LoadRepresentativePath();
                ClusterCurrent.SaveClusterToXml();
            }
            else
            {
                File.Delete(RetroViewModel.RetroInstance.ClusteringPath + @"\cluster" + ClusterCurrent.Id + ".xml");
                RetroViewModel.RetroInstance.ClustersList.Remove(ClusterCurrent);
            }

        }
        /// <summary>
        /// send the seleced pattern in the list of pattern to cluster cited
        /// </summary>
        /// <param name="IdClusterSend">the id and label of cluster</param>
        /// <param name="NewClusterId">the id of new cluster</param>
        /// <param name="NewCluster">the label of new cluster</param>
        /// <param name="PatternSend">selected PatternItem</param>
        /// <param name="ClusteringPath">folder of file cluster .xml</param>
        public void SendPattern(String IdClusterSend,String NewClusterId, String NewCluster, Object PatternSend,String ClusteringPath)
        {
            //delet the pattern in the panel of analyse cluster
            DeletPattern(PatternSend);
            //add the pattern in another Cluster
            String[] ArrayId = IdClusterSend.Split(')');
            List<Cluster> Clusters = RetroViewModel.GetClusters();
            if (IdClusterSend != "NewCluster")
            {
                Parallel.ForEach(Clusters, cluster =>
                {
                    if ((cluster.Id == ArrayId[0]))//&& (cluster.LabelList[0] == ArrayId[1])
                    {
                        cluster.AddPattern(((PatternItem)PatternSend).Pattern);
                        //update the patterns in the file .xml
                        cluster.SaveClusterToXml();
                        
                    }
                });
            }
            else
            {
                Parallel.ForEach(Clusters,cluster=>
                    {
                        if (cluster.Id == NewClusterId)
                        {
                            MessageBox.Show("The Id of new Cluster has exist, please select another Id!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }  
                    });

                //create a new cluster and add the pattern in this cluster
                Cluster newCluster = new Cluster(NewClusterId,  ClusteringPath);
                newCluster.AddPattern(((PatternItem)PatternSend).Pattern);
                newCluster.Representatives.Add(((PatternItem)PatternSend).Pattern);//for the image show in the Cluster->Display
                newCluster.LoadRepresentativePath();//for the image show in the list of Cluster_listBox in the panel Cluster->RESULTS
                newCluster.SaveClusterToXml();
                newCluster.AddNewLabel("MANUAL", NewCluster, 1.0);

                RetroViewModel.RetroInstance.ClustersList.Add(newCluster);
            }
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
