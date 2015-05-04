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
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Xml;

using Retro.Model;
using Polytech.Clustering.Plugin;

namespace RetroGUI.visualisation
{
    /// <summary>
    /// Generate a view for a cluster in order to modify it in the Modify Cluster View
    /// </summary>
    public partial class DisplayDetailsClusterForModification : System.Windows.Controls.UserControl
    {
        #region Attributes
        //public static List<object> listClustersAddToWorkspace;
        public static List<ShapeEoC> listShapesAddToWorkspace;
        public static ShapeEoC shapeAddtoWorkspace;

        private ComboBoxItem itemNull = new ComboBoxItem();

        ShapeEoC currentShape;
        System.Windows.Controls.ListBox dragSource;

        //ListBoxItem currentShape;
        private static ShapeEoC _shapeToChange;
        private static ListBoxItem _dragged;
        //private static Retro.Model.core.ShapeEoC _dragged;
        private int index = 0;
        private ObservableCollection<APattern> AllShapesOfCurrentCluster = new ObservableCollection<APattern>();

        private Cluster _CurrentCluster;
        /// <summary>
        /// Current selected Cluster
        /// </summary>
        public Cluster CurrentCluster
        {
            get { return _CurrentCluster; }
            set { _CurrentCluster = value; }

        }


        private List<APattern> _Shapes;
        /// <summary>
        /// ShapeEoC list
        /// </summary>
        public List<APattern> Shapes
        {
            get { return _Shapes; }
            set { _Shapes = value; }

        }

        //Lists of ClusteringMethods and Descriptors Names to fill comboBoxes
        List<string> listClusteringMethodsName = new List<string>();
        List<string> listDescritporsName = new List<string>();

        #endregion

        #region constructor
        public DisplayDetailsClusterForModification()
        {
            InitializeComponent();
            /*
            currentShape = new ShapeEoC();
            _shapeToChange = new ShapeEoC();
            //listClustersAddToWorkspace = new List<object>();
            listShapesAddToWorkspace = new List<ShapeEoC>();
            dragSource = null;
            itemNull.Content = null;
            fillComboBoxes();
            shapeAddtoWorkspace = new ShapeEoC();
            */
        }
        #endregion

        #region View
        /// <summary>
        /// Update the view regarding the current cluster that is considered
        /// </summary>
        public void UpdateView()
        {
            // Update representative thumbnail
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            string pathh = "jy to debug";
            if (File.Exists(pathh))
                src.UriSource = new Uri(pathh, UriKind.Absolute);
            else
                src.UriSource = new Uri("/resources/LogoLI.png", UriKind.Relative);
            src.EndInit();


            this.Shapes = this.CurrentCluster.Patterns;
            this.listBoxShapeCluster.ItemsSource = AllShapesOfCurrentCluster;

            index = 0;

            UpdateBindingAllShapes();
        }



        /// <summary>
        /// Update the all shapes of the cluster that are displayed
        /// </summary>
        private void UpdateBindingAllShapes()
        {
            try
            {
                AllShapesOfCurrentCluster.Clear();
                for (int i = index; i < Shapes.Count; i++)
                {
                    AllShapesOfCurrentCluster.Add(Shapes.ElementAt(i));
                }
            }
            catch (NullReferenceException e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }


        }
        #endregion

        #region GetDataFromListBox(ListBox,Point)
        private static object GetDataFromListBox(System.Windows.Controls.ListBox source, Point point)
        {
            UIElement element = source.InputHitTest(point) as UIElement;
            if (element != null)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);
                    if (data == DependencyProperty.UnsetValue)
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    }
                    if (element == source)
                    {
                        return null;
                    }
                }
                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
            }
            return null;
        }

        #endregion

        #region Drag&Drop

        /// <summary>
        /// Define the action when the left mouse button is pressed while the mouse pointer is over this element. Permit to get the shape selected by the clic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxShapeCluster_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.ListBox parent = (System.Windows.Controls.ListBox)sender;
            dragSource = parent;
            object data = GetDataFromListBox(dragSource, e.GetPosition(parent));

            if (data != null)
            {
                DragDrop.DoDragDrop(parent, data, System.Windows.DragDropEffects.Move);
            }

        }
       
        /// <summary>
        /// Define the action of the drag enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxShapeCluster_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            object data = e.Data.GetData(typeof(ShapeEoC));
            if (data != null)
            {
                AllShapesOfCurrentCluster.Remove((ShapeEoC)data);
            }

        }
        
        /// <summary>
        /// Define the action for drop a shape in a new cluster
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxShapeCluster_Drop(object sender, System.Windows.DragEventArgs e)
        {

            System.Windows.Controls.ListBox parent = (System.Windows.Controls.ListBox)sender;
            object data = e.Data.GetData(typeof(ShapeEoC));
            if (data != null)
            {
                AllShapesOfCurrentCluster.Add((ShapeEoC)data);
            }

        }
        #endregion

        #region RightClicOnShape
        /// <summary>
        /// Define the action for a click on the MenuItem "Add To Workspace"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemAddToWorkspace_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menuItem = e.Source as System.Windows.Controls.MenuItem;
            if (this.listBoxShapeCluster.SelectedItem != null)
            {
                object data = listBoxShapeCluster.SelectedItem;
               shapeAddtoWorkspace = (ShapeEoC)data;

                if (data != null)
                {
                    AllShapesOfCurrentCluster.Remove(shapeAddtoWorkspace);
                }

                this.FindPageControl(this).UpdateViewWorkspace(shapeAddtoWorkspace);
            }
            listShapesAddToWorkspace.Clear();

        }

        

        /// <summary>
        /// Define the action for a click on the MenuItem "Delete"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxShapeCluster.SelectedItems.Count > 0)
            {
                _shapeToChange = (ShapeEoC)listBoxShapeCluster.SelectedItems[0];
                MessageBoxResult sure = System.Windows.MessageBox.Show("Are you sure you want to delete this shape ? (shape n° " + _shapeToChange.IdPart1 + ")", "Delete shape", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (sure == MessageBoxResult.Yes)
                {
                    object data = listBoxShapeCluster.SelectedItem;
                    AllShapesOfCurrentCluster.Remove((ShapeEoC)data);

                }
            }


        }
        #endregion

        #region readXMLFile

        /// <summary>
        /// Permit to extract all the clusteringMethod names or descriptors names available
        /// </summary>
        /// <param name="path">path of the Xml File</param>
        /// <returns>a list of all the name</returns>
        public List<string> getClusteringMethodOrDescriptiorsNamesFromXMLFile(string path)
        {
            List<string> listNames = new List<string>();
            XDocument doc = XDocument.Load(path);
            var mehtodNames = doc.Descendants("Name");

            foreach (var mehtodName in mehtodNames)
            {
                listNames.Add(mehtodName.Value);
            }

            return listNames;
        }

        /// <summary>
        /// Permit to extract the path of a ClusteringMethod dll
        /// </summary>
        /// <param name="path">path of the XML file</param>
        /// <param name="name">name of the ClusteringMethod (the same that there is in the XML File</param>
        /// <returns>the path of the ClusteringMethod dll</returns>
        public string getPathClusteringMethodFromXMLFile(string path, string name)
        {
            string s = null;
            XmlDocument xml = new XmlDocument();
            xml.Load(path); // suppose that myXmlString contains "<Names>...</Names>"

            XmlNodeList xnList = xml.SelectNodes("/ClusteringMethods/Method");
            foreach (XmlNode xn in xnList)
            {
                if (xn["Name"].InnerText == name)
                {
                    string pathDll = xn["Path"].InnerText;
                    s = pathDll;
                }
            }

            return s;
        }

        /// <summary>
        /// Permit to extract the path of a Descriptor dll
        /// </summary>
        /// <param name="path">path of the XML file</param>
        /// <param name="name">name of the Descriptor (the same that there is in the XML File)</param>
        /// <returns>the path of the Descriptor dll</returns>
        public string getPathDescriptorFromXMLFile(string path, string name)
        {
            string s = null;
            XmlDocument xml = new XmlDocument();
            xml.Load(path); // suppose that myXmlString contains "<Names>...</Names>"

            XmlNodeList xnList = xml.SelectNodes("/Descriptors/Descriptor");
            foreach (XmlNode xn in xnList)
            {
                if (xn["Name"].InnerText == name)
                {
                    string pathDll = xn["Path"].InnerText;
                    s = pathDll;
                }
            }

            return s;
        }

        #endregion

        #region ComboBoxe

        /// <summary>
        /// Permit to fill the comboBoxes
        /// </summary>
        public void fillComboBoxes()
        {
            comboBoxDescripteur.Items.Add(itemNull);

            listClusteringMethodsName = getClusteringMethodOrDescriptiorsNamesFromXMLFile("..\\..\\..\\XML_Files\\ClusteringMethods.xml");
            listDescritporsName = getClusteringMethodOrDescriptiorsNamesFromXMLFile("..\\..\\..\\XML_Files\\Descriptors.xml");

            foreach (string methodNames in listClusteringMethodsName)
            {
                comboBoxClusteringMethod.Items.Add(methodNames);
            }

            foreach (string descriptorsNames in listDescritporsName)
            {
                comboBoxDescripteur.Items.Add(descriptorsNames);

            }

            comboBoxClusteringMethod.SelectedItem = null;
        }

        /// <summary>
        /// Permit to get the clustering method selected
        /// </summary>
        /// <returns></returns>
        public string getClusteringMethodSelected()
        {
            return (this.comboBoxClusteringMethod.Text);
        }

        /// <summary>
        /// Define the action when the clustering comboBox selection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxClusteringMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBoxDescripteur.IsEnabled = true;

            List<string> clusteringMethodNames = getClusteringMethodOrDescriptiorsNamesFromXMLFile("..\\..\\..\\XML_Files\\ClusteringMethods.xml");

            for (int i = 0; i < clusteringMethodNames.Count; i++)
            {
                if (comboBoxClusteringMethod.SelectedItem.ToString() == clusteringMethodNames[i])
                {
                    //chargement de la dll
                    IClusteringPlugin clusteringPluginObj;
                    string pathClustering = getPathClusteringMethodFromXMLFile("..\\..\\..\\XML_Files\\ClusteringMethods.xml", clusteringMethodNames[i]);
                    string[] splittedPath = pathClustering.Split('\\');
                    clusteringPluginObj = (IClusteringPlugin)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(pathClustering, "Polytech.Clustering.Plugin." + splittedPath[splittedPath.Length - 1].Split('.')[0]);

                    clusteringPluginObj.GetConfigWindow().ShowDialog();
                }

            }


        }

        /// <summary>
        /// Define the action when the descriptor comboBox selection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxDescripteur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            List<string> descriptorNames = getClusteringMethodOrDescriptiorsNamesFromXMLFile("..\\..\\..\\XML_Files\\Descriptors.xml");

            for (int i = 0; i < descriptorNames.Count; i++)
            {
                if (comboBoxDescripteur.SelectedItem.ToString() == descriptorNames[i])
                {
                    //chargement de la dll
                    IDescriptorPlugin descriptorPluginObj;
                    string pathDescriptor = getPathDescriptorFromXMLFile("..\\..\\..\\XML_Files\\Descriptors.xml", descriptorNames[i]);
                    string[] splittedPath = pathDescriptor.Split('\\');
                    descriptorPluginObj = (IDescriptorPlugin)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(pathDescriptor, "Polytech.Clustering.Plugin." + splittedPath[splittedPath.Length - 1].Split('.')[0]);

                    descriptorPluginObj.GetConfigWindow().ShowDialog();
                }
            }
        }

        #endregion

        #region Start TestModule on one Cluster

        // TO DO
        
        #endregion

        #region Tools
        /// <summary>
        /// Permit to get the current window parent of the userControl
        /// </summary>
        /// <param name="child">userControl</param>
        /// <returns></returns>
        public main.ModifyClusters FindPageControl(DependencyObject child)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            if (parent == null) return null;

            main.ModifyClusters page = parent as main.ModifyClusters;
            if (page != null)
            {
                return page;
            }
            else
            {
                return FindPageControl(parent);
            }
        }
        #endregion

    }

}



