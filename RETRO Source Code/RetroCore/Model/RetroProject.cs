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
using System.IO;
using System.Xml.Serialization;
using System.Windows;

using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.Xml;

using Retro.ViewModel;
using RetroUtil;
using Polytech.Clustering.Plugin;

namespace Retro.Model
{
    /// <summary>
    /// Model of a Retro Project
    /// </summary>
    public class RetroProject
    {

        #region Attributs
        
        private String _RetroProjectName;
        /// <summary>
        /// Name of the RETRO project
        /// </summary>
        [Description("Retro Project name")]
        public String RetroProjectName
        {
            get { return _RetroProjectName; }
            set { _RetroProjectName = value; }
        }


        private String _RetroProjectFilePath;
        /// <summary>
        /// Path and fileName of the RETRO project file
        /// </summary>
        [Description("Retro project file path")]
        public String RetroProjectFilePath
        {
            get { return _RetroProjectFilePath; }
            set { _RetroProjectFilePath = value; }
        }


        private String _AgoraAltoPath;
        /// <summary>
        /// Path of the Agora project Alto directory
        /// </summary>
        [Description("Agora Alto path")]
        public String AgoraAltoPath
        {
            get { return _AgoraAltoPath; }
            set { _AgoraAltoPath = value; }
        }


        private String _FullImagesPath;
        /// <summary>
        /// Path of the images considered by Agora
        /// AgoraPath + @"images/"
        /// </summary>
        [Description("Full Images path")]
        public String FullImagesPath
        {
            get { return _FullImagesPath; }
            set { _FullImagesPath = value; }
        }


        private String _ClusteringRetroResultPath;
        /// <summary>
        /// Path where the clusters.xml files will be saved
        /// </summary>
        [Description("Clusters path")]
        public String ClusteringPath
        {
            get { return _ClusteringRetroResultPath; }
            set { _ClusteringRetroResultPath = value; }
        }


        private List<Cluster> _ClustersList = new List<Cluster>();
        /// <summary>
        /// Clusters List 
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public List<Cluster> ClustersList
        {
            get { return _ClustersList; }
            set { _ClustersList = value; }
        }

        
        private int _TotalNbClusters;
        /// <summary>
        /// Total number of clusters
        /// </summary>
        [Description("Total clusters number")]
        public int TotalNbClusters
        {
            get { return _TotalNbClusters; }
            set { _TotalNbClusters = value; }
        }


        private int _TotalNbShapes;
        /// <summary>
        /// Total number of shapes of this project's clusters
        /// </summary>
        [Description("Total shapes number")]
        public int TotalNbShapes
        {
            get { return _TotalNbShapes; }
            set { _TotalNbShapes = value; }
        }

        private String _driveName;
        /// <summary>
        /// driveName where data are stored ("E:\\" for example)
        /// </summary>
        [Description("Name of the drive (letter)")]
        public String DriveName
        {
            get { return _driveName; }
            set { _driveName = value; }
        }


        #endregion



        /// <summary>
        /// Default Constructor
        /// </summary>
        public RetroProject()
        {
        }


        #region Static project related methods

        /// <summary>
        /// Create a new Retro project
        /// </summary>
        /// <param name="projectName">Name of the retro project</param>
        /// <param name="retroProjectDir">Retro project dir</param>
        /// <param name="agoraDataPath">Agora Data Path</param>
        /// <param name="_retro">Instance of RetroProject to fill</param>
        public static void New(String projectName, string retroProjectDir, String agoraDataPath, ref RetroProject _retro)
        {
            String agoraAltoPath = agoraDataPath + @"\alto\";             // Add "/Alto" from the string 
                //agoraDataPath.Substring(0, agoraDataPath.Length - 5);      // remove "Alto" from the string
            // Create Retro Projet directory and Clusters directory
            if (!Directory.Exists(retroProjectDir))
                Directory.CreateDirectory(retroProjectDir);
            if (!Directory.Exists(retroProjectDir + @"\clusters"))
                Directory.CreateDirectory(retroProjectDir + @"\clusters");

            // Fill RetroProject attributes            
            _retro._AgoraAltoPath = agoraAltoPath;
            _retro._RetroProjectName = projectName;
            _retro._RetroProjectFilePath = retroProjectDir + @"\" + projectName + ".xml";            
            _retro._FullImagesPath = agoraDataPath + @"\images\";
            _retro._ClusteringRetroResultPath = retroProjectDir + @"\clusters\";
            _retro._ClustersList = new List<Cluster>();
            _retro.TotalNbClusters = 0;
            _retro.TotalNbShapes = 0;

            //Extract the driveName from AgoraAlto Path
            _retro._driveName = agoraAltoPath.Substring(0, 2);

            // Save project
            _retro.Save(_retro._RetroProjectFilePath);
        }


        /// <summary>
        /// Open an existing Retro project
        /// </summary>
        /// <param name="filename">Path of the project xml file</param>
        /// <param name="_retro">Instance of RetroProject</param>
        /// <returns>ReturnValues.OpenProject value</returns>
        public static ReturnValues.OpenProject Open(String filename, ref RetroProject _retro)
        {
            // Check if XML file
            if (!Path.GetExtension(filename).Equals(".xml"))
                return ReturnValues.OpenProject.NotXmlFile;

            // Check if XML file exist
            if (!File.Exists(filename))
                return ReturnValues.OpenProject.FileDoesNotExist;

            // Desezialize the XML file
            XmlSerializer xs = new XmlSerializer(typeof(RetroProject));
            try
            {
                StreamReader rd = new StreamReader(filename);
                _retro = (RetroProject)xs.Deserialize(rd);
                rd.Close();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
                return ReturnValues.OpenProject.XmlDeserializeError;
            }

            //Extract the driveName from AgoraAlto Path
            _retro._driveName = _retro.AgoraAltoPath.Substring(0, 2);

            return ReturnValues.OpenProject.Ok;
        }


        /// <summary>
        /// Save a Retro project
        /// </summary>
        /// <param name="filepath">Path of the xml file where the project will be saved</param>
        public void Save(String filepath)
        {
            // Save the Retro project xml file
            XmlSerializer xs = new XmlSerializer(typeof(RetroProject));
            StreamWriter wr = new StreamWriter(filepath);
            xs.Serialize(wr, this);
            wr.Close();

            // We actually suppose that the Cluster list have been saved with there new labels during their processing => so nothing to do here !
            
        }


        /// <summary>
        /// Clear Clusters List from the current project => Number of cluster will be zero
        /// </summary>
        public void ClearClustersList()
        {
            // Clear Clusters list
            foreach (Cluster cluster in this._ClustersList)
                cluster.ClearPatternsFromMemory();

            this._ClustersList.Clear();
            this._ClustersList = null;            
        }

        #endregion


        /// <summary>
        /// Export Transcription as Alto
        /// </summary>
        /// <param name="dynamicSplashScreenNotification">Dynamic Splashscreen Notification</param>
        public void ExportAsAlto(DynamicSplashScreenNotification dynamicSplashScreenNotification)
        {
            // Get labelized clusters
            // TODO: find a way to consider only clusters labelized during this session
            List<Cluster> labelizedClusters = this.ClustersList.FindAll(
                delegate(Cluster cluster)
                {
                    return cluster.IsLabelized;
                }
            );

            // Load and Compute nb of shapes that will be consider
            int nbShapesToProcess = 0;
            foreach (Cluster cluster in labelizedClusters)
            {
                cluster.LoadPatternsFromFile(true);
                nbShapesToProcess += cluster.Patterns.Count;
            }

            // Naive way: open all the alto files for all the shapes of all the labelized clusters
            // TODO: Improve and Optimize!
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            int shapeCpt = 0;
            foreach (Cluster cluster in labelizedClusters)
            {
                foreach (ShapeEoC shape in cluster.Patterns)
                {
                    // Notify the ViewModel
                    shapeCpt++;
                    dynamicSplashScreenNotification.Message = "Processing shape " + shapeCpt + "/" + nbShapesToProcess;

                    string [] altoname = shape.IdPart1.Split('.');
                    String altoFile = this._AgoraAltoPath + altoname[0] + ".xml";

                    // Check if XML file exist
                    if (File.Exists(altoFile))
                    {
                        // Load the XML File
                        xmlDoc.Load(altoFile);

                        // Create an XmlNamespaceManager to resolve the default namespace.
                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                        nsmgr.AddNamespace("alto", "http://www.loc.gov/standards/alto/ns-v2#");
                            
                        // Find the current shape
                        String query = String.Format("//alto:String[@ID='{0}']", shape.IdPart1);  // or "//*[@id='{0}']" if we don't want to precise the tag name
                        XmlElement stringElement = (XmlElement)xmlDoc.SelectSingleNode(query, nsmgr);

                        // Update the content of the selected shape in the alto xml
                        stringElement.Attributes["CONTENT"].InnerText = cluster.LabelList[0];

                        // Save the modification
                        bool done = false;
                        while (!done)
                        {
                            try
                            {
                                StreamWriter writer = new StreamWriter(altoFile, false, Encoding.UTF8);
                                xmlDoc.Save(writer);
                                writer.Close();
                                done = true;

                            }
                            catch (IOException e)
                            {
                                System.Windows.MessageBox.Show(e.ToString());
                                // Do nothing
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Load information about the Clusters from the XML files and store the list in memory. Patterns are cleared from memury
        /// </summary>
        public void LoadClusters()
        {
            int nshapes = 0;
            Cluster currentcluster = null;
            ShapeEoC currentShape = null;
            
            // Check if the directory exists
            if (!Directory.Exists(this._ClusteringRetroResultPath))
            {
                MessageBox.Show("ERROR: " + this._ClusteringRetroResultPath + " doesn't exist!");
                
            }
            else
            {
                // Get list of xml filenames
                String[] clusterFiles = Directory.GetFiles(this._ClusteringRetroResultPath, "*.xml");

                // Load the clusters
                foreach (String filename in clusterFiles)
                {
                    // Check if the xml is a cluster file   
                    if ((Path.GetFileNameWithoutExtension(filename)).Contains("cluster"))
                    {
                        currentcluster = new Cluster(filename);  // Load the Cluster from xml file and all the patterns
                    }
                    else 
                        currentcluster = null;

                    // Add the currentcluster to the list
                    if (currentcluster != null && (currentcluster.Patterns.Count != 0))
                    {
                        if (!currentcluster.ShapesAreLoaded) currentcluster.LoadPatternsFromFile(true);
                        currentShape = ((ShapeEoC)currentcluster.Patterns.ElementAt(0));
                        
                        //currentShape.PathToFullImage = this.DriveName + currentShape.IdPart2 + @"images\" + currentShape.IdPart1 + ".png";;
                        currentShape.PathToFullImage = currentShape.IdPart2 + @"images\" + currentShape.IdPart1 + ".png";
                        currentShape.LoadEoCImage();
                        
                        //add the representative image of cluster and its path
                        currentcluster.AddRepresentative(currentShape);
                        currentcluster.LoadRepresentativePath();

                        this._ClustersList.Add(currentcluster);
                        nshapes = nshapes + currentcluster.Patterns.Count;
                        //currentcluster.ClearPatternsFromMemory();
                    }
                    
                }
                
                // Sort the list of cluster
                // Default Sorting function is Desc ShapeEoC Number
                //IComparer myComparer = new SortbyShapeNumberDesc();
                //this.clusters.Sort(myComparer);

                this._TotalNbShapes = nshapes;
                this._TotalNbClusters = this.ClustersList.Count;
            }
        }


    }
}
