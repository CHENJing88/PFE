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

using Retro.Model.core;
using Retro.ViewModel;
using RetroUtil;

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
        /// Path of the RETRO project file
        /// </summary>
        [Description("Retro project file path")]
        public String RetroProjectFilePath
        {
            get { return _RetroProjectFilePath; }
            set { _RetroProjectFilePath = value; }
        }


        private String _AgoraPath;
        /// <summary>
        /// Path of the Agora project
        /// </summary>
        [Description("Agora path")]
        public String AgoraPath
        {
            get { return _AgoraPath; }
            set { _AgoraPath = value; }
        }


        private String _AltoPathPrivate;
        /// <summary>
        /// Path of the Alto xml file. 
        /// AgoraPath + @"alto/"
        /// </summary>
        [Description("Alto path")]
        public String AltoPath
        {
            get { return _AltoPathPrivate; }
            set { _AltoPathPrivate = value; }
        }


        private String _TextThumbnailsPath;
        /// <summary>
        /// Path of the Text Thumbnails.
        /// AgoraPath + @"alto/images_text/"
        /// </summary>
        [Description("Text Thumbnails path")]
        public String TextThumbnailsPath
        {
            get { return _TextThumbnailsPath; }
            set { _TextThumbnailsPath = value; }
        }


        private String _IllustrationThumbnailsPath;
        /// <summary>
        /// Path of the Illustration Thumbnails
        /// AgoraPath + @"alto/images_illustration/"
        /// </summary>
        [Description("Illustration Thumbnails path")]
        public String IllustrationThumbnailsPath
        {
            get { return _IllustrationThumbnailsPath; }
            set { _IllustrationThumbnailsPath = value; }
        }


        private String _ImagesPathPrivate;
        /// <summary>
        /// Path of the images considered by Agora
        /// AgoraPath + @"images/"
        /// </summary>
        [Description("Images path")]
        public String ImagesPath
        {
            get { return _ImagesPathPrivate; }
            set { _ImagesPathPrivate = value; }
        }


        private String _ClusteringRetroResultPath;
        /// <summary>
        /// Path of the Clustering project
        /// </summary>
        [Description("Clustering path")]
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

        #endregion



        /// <summary>
        /// Dfault Constructor
        /// </summary>
        public RetroProject()
        {
        }


        #region Static project related methods

        /// <summary>
        /// Create a new Retro project
        /// </summary>
        /// <param name="projectName">Name of the project</param>
        /// <param name="agoraProjectFile">Agora project file</param>
        /// <param name="_retro">Instance of RetroProject to fill</param>
        public static void New(String projectName, String agoraProjectFile, ref RetroProject _retro)
        {
            String agoraEnvironmentPath = Path.GetDirectoryName(agoraProjectFile);
            // Create directory
            if (!Directory.Exists(agoraEnvironmentPath + @"\clustering"))
                Directory.CreateDirectory(agoraEnvironmentPath + @"\clustering");
            if (!Directory.Exists(agoraEnvironmentPath + @"\retro"))
                Directory.CreateDirectory(agoraEnvironmentPath + @"\retro");

            // Fill RetroProject attributes
            _retro._AgoraPath = agoraEnvironmentPath;
            _retro._RetroProjectName = projectName;
            _retro._RetroProjectFilePath = agoraEnvironmentPath + @"\retro\" + projectName + ".xml";
            _retro._AltoPathPrivate = agoraEnvironmentPath + @"\alto\";
            _retro._TextThumbnailsPath = agoraEnvironmentPath + @"\alto\images_text\";
            _retro._IllustrationThumbnailsPath = agoraEnvironmentPath + @"\alto\images_illustration\";
            _retro._ImagesPathPrivate = agoraEnvironmentPath + @"\images\";
            _retro._ClusteringRetroResultPath = agoraEnvironmentPath + @"\clustering\";
            _retro._ClustersList = new List<Cluster>();


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

            // Check if BIN file exist
            if (!File.Exists(filename.Substring(0,filename.Length-4) + ".bin"))
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

            // Deserialize the BIN file
            try
            {
                FileStream fs = new FileStream(filename.Substring(0, filename.Length - 4) + ".bin", FileMode.Open);
                BinaryFormatter sf = new BinaryFormatter();
                _retro.ClustersList = (List<Cluster>) sf.Deserialize(fs);
                fs.Close();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
                return ReturnValues.OpenProject.XmlDeserializeError;
            }

            return ReturnValues.OpenProject.Ok;
        }


        /// <summary>
        /// Save a Retro project
        /// </summary>
        /// <param name="filepath">Path of the xml file where the project will be saved</param>
        public void Save(String filepath)
        {
            // Update attribute if SaveAs case
            if (filepath.CompareTo(this._RetroProjectFilePath) != 0)
            {
                this._RetroProjectName = Path.GetFileNameWithoutExtension(filepath);
                this._RetroProjectFilePath = filepath;
            }

            // Save the Retro project xml file
            XmlSerializer xs = new XmlSerializer(typeof(RetroProject));
            StreamWriter wr = new StreamWriter(filepath);
            xs.Serialize(wr, this);
            wr.Close();

            // Save the Clusters in a *.bin extension file
            FileStream fs = new FileStream(filepath.Substring(0, filepath.Length - 4) + ".bin", FileMode.Create);
            BinaryFormatter sf = new BinaryFormatter();
            sf.Serialize(fs, this._ClustersList);
            fs.Close();
        }


        /// <summary>
        /// ClearClustersList of the current project
        /// </summary>
        public void ClearClustersList()
        {
            // Clear Clusters list
            foreach (Cluster cluster in this._ClustersList)
                cluster.Reset();

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

            // Compute nb of shapes that will be consider
            int nbShapesToProcess = 0;
            foreach (Cluster cluster in labelizedClusters)
                nbShapesToProcess += cluster.NbShapes;

            // Naive way: open all the alto files for all the shapes of all the labelized clusters
            // TODO: Improve and Optimize!
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            int shapeCpt = 0;
            foreach (Cluster cluster in labelizedClusters)
            {
                foreach (ShapeEoC shape in cluster.ShapesList)
                {
                    // Notify the ViewModel
                    shapeCpt++;
                    dynamicSplashScreenNotification.Message = "Processing shape " + shapeCpt + "/" + nbShapesToProcess;

                    String altoFile = this._AltoPathPrivate + shape.ImageSourceID + ".xml";

                    // Check if XML file exist
                    if (File.Exists(altoFile))
                    {
                        // Load the XML File
                        xmlDoc.Load(altoFile);

                        // Create an XmlNamespaceManager to resolve the default namespace.
                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                        nsmgr.AddNamespace("alto", "http://www.loc.gov/standards/alto/ns-v2#");
                            
                        // Find the current shape
                        String query = String.Format("//alto:String[@ID='{0}']", shape.ShapeId);  // or "//*[@id='{0}']" if we don't want to precise the tag name
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
        /// Load the Clusters files and build store the list in memory
        /// </summary>
        public void LoadClusters(bool bIllustrationClustering)
        {
            int nshapes = 0;
            Cluster currentcluster = null;
            String thumbnailsPath = "";

            // Check if the directory exists
            if (!Directory.Exists(this._ClusteringRetroResultPath))
            {
                MessageBox.Show("ERROR: " + this._ClusteringRetroResultPath + " doesn't exist!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Get list of xml filenames
                String[] clusterFiles = Directory.GetFiles(this._ClusteringRetroResultPath, "*.xml");

                // Set the thumbnail path
                thumbnailsPath = (bIllustrationClustering) ? this._IllustrationThumbnailsPath : this._TextThumbnailsPath;

                foreach (String filename in clusterFiles)
                {
                    // Check if the xml is a cluster file   
                    if ((Path.GetFileNameWithoutExtension(filename)).Contains("cluster"))
                        currentcluster = new Cluster(filename, thumbnailsPath, this._AltoPathPrivate);
                    else
                        currentcluster = null;

                    // Add the currentcluster to the list
                    if (currentcluster != null && (currentcluster.NbShapes != 0))
                    {
                        currentcluster.RepresentativePathToBitmap = thumbnailsPath + @"\" + currentcluster.ShapesList.ElementAt(0).ImageSourceID + @"\" + currentcluster.ShapesList.ElementAt(0).ShapeId + ".png";
                        this._ClustersList.Add(currentcluster);
                        nshapes = nshapes + currentcluster.NbShapes;

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
