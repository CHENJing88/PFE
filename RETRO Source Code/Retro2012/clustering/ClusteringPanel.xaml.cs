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
using System.Windows.Forms;     // NOT WPF, BUT NEEDED FOR FolderBrowserDialog
using System.Linq.Expressions;

using System.IO;
using System.Xml.Linq;
using System.Xml;

using Retro.ViewModel;
using Polytech.Clustering.Plugin;
using RetroUtil;
using System.Threading;

namespace RetroGUI.clustering
{
    /// <summary>
    /// Define TestModule Panel
    /// </summary>
    public partial class ClusteringPanel : System.Windows.Controls.UserControl
    {

        #region Attributes

        
        public static IClusteringPlugin clusteringMethodPluginUsedForClustering;
        public static IDescriptorPlugin descriptorPluginUsedForClustering;

        private TemplateMatchingConfiguration templateMatchingConfigurationWindow = new TemplateMatchingConfiguration();
        private bool bIllustrationClustering;
        private String agoraProjectDir;
        private String outputClustersDir;
        private ComboBoxItem itemComboBoxMethodTemplateMatching = new ComboBoxItem();
        private ComboBoxItem itemNull = new ComboBoxItem();
        private string NameTemplateMatching = "Template Matching";

        public static string clustersDir;

        List<string> listClusteringMethodsName = new List<string>();
        List<string> listDescritporsName = new List<string>();
        System.Windows.Forms.MenuItem testItem = new System.Windows.Forms.MenuItem();

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="agoraProjectDir">Path of the AGORA project</param>
        /// <param name="outputClustersDir">Path of the output directory</param>
        public ClusteringPanel(String agoraProjectDir, String outputClustersDir)
        {
            InitializeComponent();
            this.agoraProjectDir = agoraProjectDir;
            this.outputClustersDir = outputClustersDir;
            clustersDir = this.outputClustersDir;
            itemNull.Content = null;
            //itemComboBoxMethodTemplateMatching.Content = templateMatchingParameters.NAME;
            fillComboBoxes();

            
            
            
        }


        public ClusteringPanel()
        {
            InitializeComponent();

        }
        #endregion

        #region Events Handlers

        /// <summary>
        /// Handler for Select Existing Models Directory button
        /// Display a FolderBrowserDialog
        /// and update the associated textbox with the selected directory
        /// </summary>
        private void Click_Browse_Folder(object sender, RoutedEventArgs e)
        {
            // Open a dialog for forlder selection
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();

            if ((fbd.SelectedPath.Length != 0) && (Directory.Exists(fbd.SelectedPath)))
            {
                // Update textbox with the selected directory
                ExistingModelsDirectoryTextBox.Text = fbd.SelectedPath;
            }
        }


        /// <summary>
        /// Handler for threshold slider
        /// </summary>
        private void Threshold_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //this.templateMatchingParameters.TEMPLATE_MATCHING_THRESHOLD = (float)(e.NewValue / 100.0);
        }

        /// <summary>
        /// Handler for ILLUSTRATION checkbox
        /// </summary>
        private void Illustration_CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            this.bIllustrationClustering = (bool)this.Illustration_CheckBox.IsChecked;

            // Propagate this information to the main window in a really dirty way
            // mandatory to load the correct path of the thumbnail regarding wheter it's a illustration or not
            //((RetroGUI.main.MainWindow)App.Current.MainWindow).bIllustrationClustering = this.bIllustrationClustering;
        }


        /// <summary>
        /// Handler for Start button
        /// </summary>
        private void Click_Start(object sender, RoutedEventArgs e)
        {
            String exePath = System.Windows.Forms.Application.StartupPath;

            if ((this.ExistingModelsDirectoryTextBox.Text.CompareTo("") != 0) && (!Directory.Exists(this.ExistingModelsDirectoryTextBox.Text)))
            {
                System.Windows.MessageBox.Show("The existing models directory does not exists.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string methodeClustering = null;
                string descriptor = null;

                
                if (comboBoxSelectClusteringMethod.SelectedItem.ToString() == NameTemplateMatching)
                {
                    //new ClusteringTool2(this.agoraProjectDir, this.ExistingModelsDirectoryTextBox.Text, this.outputClustersDir, this.templateMatchingParameters, this.bIllustrationClustering);
                }
                else
                {
                    methodeClustering = comboBoxSelectClusteringMethod.SelectedItem.ToString();
                    descriptor = comboBoxSelectDescriptor.SelectedItem.ToString();
                    
                    List<string> clusteringMethodNames = getClusteringMethodOrDescriptorsNamesFromXMLFile(exePath + "\\XML_Files\\ClusteringMethods.xml");
                    List<string> descriptorNames = getClusteringMethodOrDescriptorsNamesFromXMLFile(exePath + "\\XML_Files\\Descriptors.xml");
                    for (int i = 0; i < clusteringMethodNames.Count; i++)
                    {
                        if (comboBoxSelectClusteringMethod.SelectedItem.ToString() == clusteringMethodNames[i])
                        {
                            for (int j = 0; j < descriptorNames.Count; j++)
                            {
                                if (comboBoxSelectDescriptor.SelectedItem.ToString() == descriptorNames[j])
                                {

                                    // Create DynamicSplashScreenNotification
                                    DynamicSplashScreenNotification dynamicSplashScreenNotification = new DynamicSplashScreenNotification();
                                    dynamicSplashScreenNotification.Message = "Clustering in process.\n Please wait.";

                                    // Create DynamicSplashScreen thread
                                    System.Threading.Thread dynamicSplashScreenThread = new System.Threading.Thread((object parameter) =>
                                    {
                                        List<String> processName = new List<String>();
                                        processName.Add("Clustering");
                                        processName.Add("Process");
                                        DynamicSplashScreen dynamicSplashScreen = new DynamicSplashScreen(processName, (DynamicSplashScreenNotification)parameter);
                                        dynamicSplashScreen.ShowDialog();
                                    });

                                    // Start DynamicSplashScreen thread
                                    dynamicSplashScreenThread.SetApartmentState(ApartmentState.STA);
                                    dynamicSplashScreenThread.Start(dynamicSplashScreenNotification);

                                    DateTime startTime = DateTime.Now;

                                    //StartClustering
                                    Database db = new Database();

                                    //Descriptor
                                    IDescriptorPlugin descriptorPluginObj;
                                    string pathDescriptor = getPathDescriptorFromXMLFile(exePath + "\\XML_Files\\Descriptors.xml", descriptorNames[j]);
                                    string[] splittedPathDescriptor = pathDescriptor.Split('\\');
                                    descriptorPluginObj = (IDescriptorPlugin)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(pathDescriptor, "Polytech.Clustering.Plugin." + splittedPathDescriptor[splittedPathDescriptor.Length - 1].Split('.')[0]);
                                    descriptorPluginUsedForClustering = descriptorPluginObj;

                                    //DocumentReader
                                    List<IDescriptorPlugin> listDescr = new List<IDescriptorPlugin>();
                                    listDescr.Add(descriptorPluginObj);
                                    IDocumentReaderPlugin docReaderPlugin;
                                    string pathdocReader = exePath + "\\Plugins\\DocumentReader\\AltoReaderPlugin.dll";
                                    string[] splittedPathDocReader = pathdocReader.Split('\\');
                                    docReaderPlugin = (IDocumentReaderPlugin)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(pathdocReader, "Polytech.Clustering.Plugin." + splittedPathDocReader[splittedPathDocReader.Length - 1].Split('.')[0]);
                                    
                                    //JY : Lien descripteurs - donnée ici => C'est load database qui lance le calcul des signatures ?????
                                    docReaderPlugin.LoadDatabase(listDescr, this.agoraProjectDir);
                                    db = docReaderPlugin.GetLoadedDatabase();

                                    //clusteringMethod
                                    //chargement de la dll
                                    IClusteringPlugin clusteringPluginObj;
                                    string pathClustering = getPathClusteringMethodFromXMLFile(exePath + "\\XML_Files\\ClusteringMethods.xml", clusteringMethodNames[i]);
                                    string[] splittedPathClusteringMethod = pathClustering.Split('\\');
                                    clusteringPluginObj = (IClusteringPlugin)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(pathClustering, "Polytech.Clustering.Plugin." + splittedPathClusteringMethod[splittedPathClusteringMethod.Length - 1].Split('.')[0]);

                                    clusteringPluginObj.SetDatabase(db);
                                    clusteringMethodPluginUsedForClustering = clusteringPluginObj;

                                    try
                                    {
                                        List<Cluster> clusterList = clusteringPluginObj.PerformClustering();

                                        // Compute processing duration
                                        DateTime stopTime = DateTime.Now;
                                        TimeSpan duration = stopTime - startTime;

                                        // End the DynamicSplashScreen Thread
                                        dynamicSplashScreenNotification.Message = "End of the Clustering. \nProcessing time: " + duration;
                                        dynamicSplashScreenThread.Abort();


                                        // Display notification
                                        var result = System.Windows.Forms.MessageBox.Show("End of the Clustering. \nProcessing time: " + duration);


                                        //sauvegarde des différents clusters dans fichiers xml séparés
                                        //Polytech.Clustering.Plugin.ExportTool.ExportClustersToXml(agoraProjectDir + @"\clustering", clusterList);
                                        //Polytech.Clustering.Plugin.ExportTool.ExportStatsToXml(agoraProjectDir + @"\clustering", clusterList);
                                    }
                                    catch (NotImplementedException exception)
                                    {
                                        if (exception.Source != null)
                                            System.Windows.MessageBox.Show("La méthode de clustering n'est pas implémentée, veuillez en choisir une autre", "Echec", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                                    }

                                   
                                }
                            }
                        }
                    }
                }

            }
            
            
        }
        #endregion

        #region ComboBoxClustering

       public void fillComboBoxes()
       {
           String exePath = System.Windows.Forms.Application.StartupPath;
           comboBoxSelectClusteringMethod.Items.Add(itemComboBoxMethodTemplateMatching.Content);
           comboBoxSelectDescriptor.Items.Add(itemNull);

           listClusteringMethodsName = getClusteringMethodOrDescriptorsNamesFromXMLFile(exePath + "\\XML_Files\\ClusteringMethods.xml");
           listDescritporsName = getClusteringMethodOrDescriptorsNamesFromXMLFile(exePath + "\\XML_Files\\Descriptors.xml");

           foreach (string methodNames in listClusteringMethodsName)
           {
               comboBoxSelectClusteringMethod.Items.Add(methodNames);
               
           }

           foreach (string descriptorsNames in listDescritporsName)
           {
               comboBoxSelectDescriptor.Items.Add(descriptorsNames);
               
           }
           
           comboBoxSelectClusteringMethod.SelectedItem = null;
       }

      


        public string getClusteringMethodSelected()
        {
            return (this.comboBoxSelectClusteringMethod.Text);
        }

        private void comboBoxSelectClusteringMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String exePath = System.Windows.Forms.Application.StartupPath;

            comboBoxSelectDescriptor.IsEnabled = true;
            if (comboBoxSelectClusteringMethod.SelectedItem.ToString() == itemComboBoxMethodTemplateMatching.Content)
            {
                //System.Windows.MessageBox.Show("Item = " + comboBoxSelectClusteringMethod.SelectedItem.ToString());
                templateMatchingConfigurationWindow.ShowDialog();
                comboBoxSelectDescriptor.IsEnabled = false;
            }
            else
            {
                List<string> clusteringMethodNames = getClusteringMethodOrDescriptorsNamesFromXMLFile(exePath + "\\XML_Files\\ClusteringMethods.xml");
               
                for (int i = 0; i < clusteringMethodNames.Count; i++)
                {
                    if (comboBoxSelectClusteringMethod.SelectedItem.ToString() == clusteringMethodNames[i])
                    {
                        //chargement de la dll
                        IClusteringPlugin clusteringPluginObj;
                        string pathClustering = getPathClusteringMethodFromXMLFile(exePath + "\\XML_Files\\ClusteringMethods.xml", clusteringMethodNames[i]);
                        string[] splittedPath = pathClustering.Split('\\');
                        clusteringPluginObj = (IClusteringPlugin)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(pathClustering, "Polytech.Clustering.Plugin." + splittedPath[splittedPath.Length - 1].Split('.')[0]);

                        clusteringPluginObj.GetConfigWindow().ShowDialog();
                        this.toolTipDescriptionClusteringMethod.Content = getDescriptionOfClusteringMethodFromXMLFile(exePath + "\\XML_Files\\ClusteringMethods.xml", clusteringMethodNames[i]);
                    
                    }
                }
            }
 

        }
        #endregion

        #region ComboBoxDescriptor
        private void comboBoxSelectDescriptor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String exePath = System.Windows.Forms.Application.StartupPath;

            List<string> descriptorNames = getClusteringMethodOrDescriptorsNamesFromXMLFile(exePath + "\\XML_Files\\Descriptors.xml");

            for (int i = 0; i < descriptorNames.Count; i++)
            {
                if (comboBoxSelectDescriptor.SelectedItem.ToString() == descriptorNames[i])
                {
                    //chargement de la dll
                    IDescriptorPlugin descriptorPluginObj;
                    string pathDescriptor = getPathDescriptorFromXMLFile(exePath + "\\XML_Files\\Descriptors.xml", descriptorNames[i]);
                    string[] splittedPath = pathDescriptor.Split('\\');
                    descriptorPluginObj = (IDescriptorPlugin)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(pathDescriptor, "Polytech.Clustering.Plugin." + splittedPath[splittedPath.Length - 1].Split('.')[0]);

                    descriptorPluginObj.GetConfigWindow().ShowDialog();

                    this.toolTipDescriptionDescriptor.Content = getDescriptionOfDescriptorFromXMLFile(exePath + "\\XML_Files\\Descriptors.xml", descriptorNames[i]);
                    
                }
            }

            //if (comboBoxSelectDescriptor.SelectedItem.ToString() == NameZernike)
            //{
            //    ////chargement de la dll
            //    IDescriptorPlugin descriptorPluginObj;
            //    string pathDescriptor = getPathDescriptorFromXMLFile(exePath + "\\XML_Files\\Descriptors.xml", "Zernike");
            //    string[] splittedPath = pathDescriptor.Split('\\');
            //    descriptorPluginObj = (IDescriptorPlugin)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(pathDescriptor, "Polytech.TestModule.Plugin." + splittedPath[splittedPath.Length - 1].Split('.')[0]);
                
            //    descriptorPluginObj.GetConfigWindow().ShowDialog();

            //}

        }
        #endregion

        #region readXMLFile

        /// <summary>
        /// Permit to extract all the clusteringMethod names or descriptors names available
        /// </summary>
        /// <param name="path">path of the Xml File</param>
        /// <returns>a list of all the name</returns>
        public List<string> getClusteringMethodOrDescriptorsNamesFromXMLFile(string path)
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
            string s= null;
            XmlDocument xml = new XmlDocument();
            xml.Load(path); // suppose that myXmlString contains "<Names>...</Names>"

            XmlNodeList xnList = xml.SelectNodes("/ClusteringMethods/Method");
            foreach (XmlNode xn in xnList)
            {
                if (xn["Name"].InnerText == name)
                {
                    s = xn["Path"].InnerText;                    
                }
            }

            return s;
        }

        public string getDescriptionOfClusteringMethodFromXMLFile(string path, string name)
        {
            string s = null;
            XmlDocument xml = new XmlDocument();
            xml.Load(path); // suppose that myXmlString contains "<Names>...</Names>"

            XmlNodeList xnList = xml.SelectNodes("/ClusteringMethods/Method");
            foreach (XmlNode xn in xnList)
            {
                if (xn["Name"].InnerText == name)
                {
                    s = xn["Description"].InnerText;                    
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
                    s = xn["Path"].InnerText;                    
                }
            }

            return s;
        }

        public string getDescriptionOfDescriptorFromXMLFile(string path, string name)
        {
            string s = null;
            XmlDocument xml = new XmlDocument();
            xml.Load(path); // suppose that myXmlString contains "<Names>...</Names>"

            XmlNodeList xnList = xml.SelectNodes("/Descriptors/Descriptor");
            foreach (XmlNode xn in xnList)
            {
                if (xn["Name"].InnerText == name)
                {
                    s = xn["Description"].InnerText;                    
                }
            }

            return s;
        }
        #endregion

       
        public string getOutputClusterDir(){

            return this.outputClustersDir;
        }

            
            
        }




    }

