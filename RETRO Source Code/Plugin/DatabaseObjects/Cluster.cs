/*
 * RETRO 2014 - JYR
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
 * Copyright © RFAI, LI Tours, 2011-2014
 * Contacts : rayar@univ-tours.fr
 *            ramel@univ-tours.fr
 * 
 */

using System;
using System.Xml.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
namespace Polytech.Clustering.Plugin
{
    /// <summary>
    /// Important Class that allow to manage and store information about the clusters (To avoid memory problem some information can be stored in a file instead of in memory if not needed - see list of patterns)
    /// </summary>
    public class Cluster : INotifyPropertyChanged
    {
        #region Attributs

        private String m_id;
        /// <summary>
        /// Identifier of the Cluster (name of the xml file)        
        /// </summary>        
        public String Id
        {
            get { return m_id; }
            set
            {
                m_id = value;
                NotifyPropertyChanged("Id");
            }
        }


        /// <summary>
        /// To get/set the list of Patterns in memory for this cluster
        /// </summary>
        /// <returns>returns the List of available pattern in this cluster (can be not present here but only in the associated xml file) </returns>
        public List<APattern> Patterns 
        { 
            get { return m_listPatterns; } 
            set 
            { 
                m_listPatterns = value;
                NotifyPropertyChanged("Patterns");
            } 
        }

        private List<APattern> m_listPatterns = new List<APattern>();    //List of Patterns in memory
        
        private bool m_patternsAreLoaded = true;    // should be set to false in ClearPatterns() - should be set to true in LoadPatternsFromFile()
        private String m_pathClusterFilename;

        /// <summary>
        /// To know if the patterns have been cleared from memory
        /// </summary>
        /// <returns>returns false if ClearPatterns() have been used - true if LoadPatternsFromFile() have been used</returns>returns>
        public bool ShapesAreLoaded 
        {
            get { return m_patternsAreLoaded; }
            set
            {
                m_patternsAreLoaded = value;
                NotifyPropertyChanged("ShapesAreLoaded");
            }  
        }


        private bool m_IsLabelized = false;
        /// <summary>
        /// Indicate if the cluster has been labelized
        /// </summary>
        public bool IsLabelized
        {
            get { return m_IsLabelized; }
            set
            {
                m_IsLabelized = value;
                NotifyPropertyChanged("IsLabelized");
            }
        }


        private int m_NbPatterns = 0;
        /// <summary>
        /// To know the Number of the shapes of this cluster 
        /// </summary>
        public int NbPatterns
        {
            get { return m_NbPatterns; }
            set
            {
                m_NbPatterns = value;
                NotifyPropertyChanged("NbPatterns");
            }
        }


        private String _RepresentativePath;
        /// <summary>
        /// Path of the representative of this cluster
        /// </summary>
        public String RepresentativePath
        {
            get { return _RepresentativePath; }
            set
            {
                _RepresentativePath = value;
                NotifyPropertyChanged("RepresentativePath");
            }
        }

        /// <summary>
        /// load the path of representativePath who is the first ShapeEoC in the list of Representatives
        /// </summary>
        public void LoadRepresentativePath()
        {
            ShapeEoC representativeShape = (ShapeEoC)Representatives[0];// need to continue modifing
            RepresentativePath = representativeShape.PathToFullImage;
        } 

        private List<APattern> m_listRepresentatives = new List<APattern>();   //List of Representatives of the patterns (centroids)
        /// <summary>
        /// list of Representatives of this cluster
        /// </summary>
        public List<APattern> Representatives
        {
            get { return m_listRepresentatives; }
            set
            {
                m_listRepresentatives = value;
                NotifyPropertyChanged("Representatives");
            }
        }

        /// <summary>
        /// Add a Representative for this cluster
        /// </summary>
        public void AddRepresentative(APattern pattern)
        {            
            m_listRepresentatives.Add(pattern);
        }

        private List<String> m_LabelList = new List<String>();
        /// <summary>
        /// Ordered List of label for this cluster (see Confidencelist and MethoList) 
        /// </summary>
        public List<String> LabelList
        {
            get { return m_LabelList; }
            set
            {
                m_LabelList = value;
                NotifyPropertyChanged("LabelList");
            }
        }

        private List<String> m_MethodList = new List<String>();
        /// <summary>
        /// Method used to set the label at this index in the LabelList
        /// </summary>
        public List<String> MethodList
        {
            get { return m_MethodList; }
            set
            {
                m_MethodList = value;
                NotifyPropertyChanged("MethodList");
            }
        }
        
        private List<Double> m_ConfidenceList = new List<Double>();
        /// <summary>
        /// Confidence rate for the labling
        /// </summary>
        public List<Double> ConfidenceList
        {
            get { return m_ConfidenceList; }
            set
            {
                m_ConfidenceList = value;
                NotifyPropertyChanged("ConfidenceList");
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor to create an empty cluster (and set the associated filename according to path and id)
        /// </summary>
        /// <param name="id">Name of the cluster file to use </param>
        /// <param name="pathToClusterDir">L'Define where to put the cluster file (path) </param>
        public Cluster(string id, string pathToClusterDir)
        {
            m_id = id;
            m_NbPatterns = 0;
            m_pathClusterFilename = pathToClusterDir + "//cluster" + id + ".xml";            
        }


        /// <summary>
        /// Constructor by loading the content of a Xml file)
        /// </summary>
        /// <param name="pathToClusterFilename">Path to cluster xml file </param>
        public Cluster(string pathToClusterFilename)
        {
            // Check if the file exists
            m_pathClusterFilename = pathToClusterFilename;
            if (!System.IO.File.Exists(m_pathClusterFilename))
            {
                MessageBox.Show("ERROR: " + m_pathClusterFilename + " doesn't exist", "Error");
                return;
            }
            else
            {
                //Extraction of Cluster ID from the filename
                String filename = Path.GetFileNameWithoutExtension(m_pathClusterFilename);
                filename = filename.Substring(7);
                this.m_id = filename;
               
                // Load the patterns and the potential label
                LoadPatternsFromFile(true);
            }
        }

        #endregion


        /// <summary>
        /// Save Cluster to XML file
        /// </summary>
        public void SaveClusterToXml()
        {
            //File creation
            StreamWriter xmlOutput = new StreamWriter(m_pathClusterFilename);
            xmlOutput.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            //xmlOutput.WriteLine("<cluster size=\"" + this.Patterns.Count + "\" label=\"" + this.LabelList[0] + "\">");
            xmlOutput.WriteLine("<cluster size=\"" + this.Patterns.Count + "\" label=\"" + ((this.LabelList.Count != 0) ? this.LabelList.ElementAt(0) : "") + "\">");
            
            //insertion of the patterns
            foreach (APattern pattern in this.Patterns)
                xmlOutput.WriteLine("\t<cc id=\"" + pattern.IdPart1 + "\" path=\"" + pattern.IdPart2 + "\"/>");

            //end of xml file
            xmlOutput.WriteLine("</cluster>");
            xmlOutput.Close();
        }

        
        /// <summary>
        /// TODO - Update the patterns list in the XML file with the actual content - TODO
        /// </summary>
        public void UpdatePatternsinXmlFile()
        {
            // Open the cluster xml file
            XDocument xmlDoc = XDocument.Load(this.m_pathClusterFilename);
            XElement clusterElement = xmlDoc.Element("cluster");
                       
            if (clusterElement != null)
                clusterElement.Attribute("size").Value = clusterElement.Attribute("size").Value + Patterns.Count;
                        
            //Update list of paterns in XMLTree
            foreach (APattern pattern in this.Patterns)
            {
                XElement ccElement = xmlDoc.Element("cc");
                ccElement.SetAttributeValue("id",pattern.IdPart1);
                ccElement.SetAttributeValue("path",pattern.IdPart2);
                clusterElement.Add(ccElement);
            }

            // Save it
            xmlDoc.Save(this.m_pathClusterFilename);
        }


        /// <summary>
        /// Load patterns from xml file
        /// </summary>
        /// <param name="updateLabel">true if Label should be read in the xml file - false to read only patterns </param>
        public void LoadPatternsFromFile(bool updateLabel)
        {
            // Check if the file exists
            if (!System.IO.File.Exists(m_pathClusterFilename))
            {
                MessageBox.Show("ERROR: " + m_pathClusterFilename + " doesn't exist", "Error");
                return;
            }
            else
            {
                //Clear the list of patterns
                ClearPatternsFromMemory();
                
                // Load the cluster xml file
                XDocument xmlDoc = XDocument.Load(m_pathClusterFilename);

                // Assign Nb of patterns
                XElement xCluster = xmlDoc.Element("cluster");
                // this.m_NbPatterns = Convert.ToInt32(xCluster.Attribute("size").Value);     // Not usefull to Read it from XML because the list will be filled just after

                // Assign label if exist and requested
                String label = xCluster.Attribute("label").Value;
                if (updateLabel == true &&  label.CompareTo("") != 0)
                {
                    this.AddNewLabel("FROMXMLFILE", label, 0.85);
                    this.IsLabelized = true;
                }

                // Fill the cluster with its EoCShapes
                ShapeEoC currentshape;                                 // quite strange to be obliged to use ShapeEoC instead of APattern
                this.m_NbPatterns = 0;                                 // List is empty for the moment
                

                foreach (XElement xCC in xmlDoc.Descendants("cc"))
                {
                    String ccID = xCC.Attribute("id").Value;
                    String altoPath = xCC.Attribute("path").Value;
                    currentshape = new ShapeEoC(ccID, altoPath);       // quite strange to be obliged to use ShapeEoC instead of APattern
                    currentshape.PathToFullImage = altoPath + "images\\" + ccID + ".png";
                    currentshape.LoadEoCImage();
                    LoadPatternSignature(currentshape);
                    this.AddPattern(currentshape);
                }
                m_NbPatterns = m_listPatterns.Count;                    // probably redundant because it has been incremented during each AddPattern()
                m_patternsAreLoaded = true;
            }
        }

        private void LoadPatternSignature(APattern Pattern)
        {
            String exePath = System.Windows.Forms.Application.StartupPath;
            List<string> descriptorNames = getClusteringMethodOrDescriptorsNamesFromXMLFile(exePath + "\\XML_Files\\Descriptors.xml");
            IDescriptorPlugin descriptorPluginObj;
            for (int i = 0; i < descriptorNames.Count; i++)
            {
                string pathDescriptor = getPathDescriptorFromXMLFile(exePath + "\\XML_Files\\Descriptors.xml", descriptorNames[i]);
                string[] splittedPathDescriptor = pathDescriptor.Split('\\');
                descriptorPluginObj = (IDescriptorPlugin)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(pathDescriptor, "Polytech.Clustering.Plugin." + splittedPathDescriptor[splittedPathDescriptor.Length - 1].Split('.')[0]);
                descriptorPluginObj.CalculateSignature(Pattern);
            }
        }
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
        /// Insert a new label for the current cluster 
        /// </summary>
        /// <param name="methodname">method of the labelling</param>
        /// <param name="label">New label</param>
        /// <param name="confidenceRate">Confidence of the new label, in [0,1]</param>
        public void AddNewLabel(String methodname, String label, Double confidenceRate)
        {
            // Get the correct insert index regarding condifance rate DESC!
            int index = 0;
            while ((index < this.m_ConfidenceList.Count) && (confidenceRate < this.m_ConfidenceList[index]))
                index++;

            // Add a the end or Insert methodname, label, and confidence in the 3 List at the computed index
            if (index == this.m_ConfidenceList.Count)
            {
                this.m_LabelList.Add(label);
                this.m_ConfidenceList.Add(confidenceRate);
                this.MethodList.Insert(index, methodname);
            }
            else
            {
                this.m_LabelList.Insert(index, label);
                this.m_ConfidenceList.Insert(index, confidenceRate);
                this.MethodList.Insert(index, methodname);
            }

            IsLabelized = true;
            
            //update Xml file if necessary (better confidence for the label)
            if (index == 0)
                this.UpdateLabelinXmlFile(label);
        }


        /// <summary>
        /// Update the label attribute in the cluster xml file regarding the new assigned label
        /// </summary>
        /// <param name="newLabel">New label</param>
        private void UpdateLabelinXmlFile(String newLabel)
        {
            // Load the cluster xml file
            XDocument xmlDoc = XDocument.Load(this.m_pathClusterFilename);
            XElement clusterElement = xmlDoc.Element("cluster");

            if (clusterElement != null)
                clusterElement.Attribute("label").Value = newLabel;

            xmlDoc.Save(this.m_pathClusterFilename);
        }


        /// <summary>
        /// Clear all the cluster content
        /// </summary>
        public void ResetPatternListandLabels()
        {
            this.m_listPatterns.Clear();
            this.m_listPatterns = null;

            this.m_listRepresentatives.Clear();
            this.m_listRepresentatives = null;
         
            this.m_LabelList.Clear();
            this.m_LabelList = null;
            
            this.m_ConfidenceList.Clear();
            this.m_ConfidenceList = null;
            
            m_IsLabelized = false;
            m_NbPatterns = 0;
            m_patternsAreLoaded = true;

        }

        /// <summary>
        /// Remove patterns from the memory
        /// </summary>
        public void ClearPatternsFromMemory()
        {
            this.m_listPatterns.Clear();
            //this.m_listPatterns = null;       remove patterns from the list but keep it as empty

            m_patternsAreLoaded = false;            
        }


        
        /// <summary>
        /// Compute the centroid of the cluster according to patterns in the memory
        /// </summary>
        /// <returns>Référence to the centroid radiuspattern. Null if no radiuspattern in memory</returns>
        public APattern GetCentroid()
        {
            if(Patterns.Count >0)
            {
                APattern centroid = (APattern)m_listPatterns[0].Clone();
                
                for (int i = 1; i < m_listPatterns.Count; i++)
                {
                    //ajout des signatures des patterns
                    centroid.SumPattern(m_listPatterns[i]);
                }
                return (centroid.DivideSignatures(m_listPatterns.Count) );
            }
           
            return null;
        }

        /// <summary>
        /// Compute the radius of the cluster - average distance from the centroïd to the rest of the patterns
        /// </summary>
        /// <returns>return the radius</returns>
        public double GetRadius()
        {
            double radius = 0;
            //// Get the centroid
            APattern radiuspattern = GetCentroid();
            
            //Compute the radius
            foreach(APattern pat in Patterns)
            {
                radius += radiuspattern.EuclidianDistance(pat);
            }
            return radius/Patterns.Count;
        }

        /// <summary>
        /// Compute the diameter of the cluster - average distance between all the patterns
        /// </summary>
        /// <returns>return the diameter</returns>
        public double GetDiameter()
        {
            double diameter = 0.0;

            //Calcul des distance entre chaque élement 2 à 2
            for (int i = 0; i < Patterns.Count - 1; i++)
            {
                for(int j = i+1; j<Patterns.Count;j++)
                {
                    diameter += Patterns[i].EuclidianDistance(Patterns[j]);
                }
            }
            return diameter / (Patterns.Count * (Patterns.Count-1));
        }

        /// <summary>
        /// Compte the list of average features of all the Patterns in the Cluster
        /// </summary>
        /// <returns>the list of average feature of Cluster</returns>
        public List<double> GetAvgFeatures(string nameFeature)
        {
            List<List<ASignature> > FeaturesList = new List<List<ASignature> >();
            foreach (APattern Pattern in Patterns)
            {
                FeaturesList.Add(Pattern.GetSignatures);
                
            }

            List<double> MoyFeatures = null;

            foreach (List<ASignature> Features in FeaturesList)
            {
                List<ASignature> FeaturesMoys=null;
                for (int i = 0; i < Features.Count; i++)
                {
                        FeaturesMoys[i].SignatureSum(Features[i]);
                        Console.Out.WriteLine(FeaturesMoys[i].ToString());
                }

            }
            //each number in the array is divided by the nombre of Partterns of Cluster
            MoyFeatures = System.Linq.Enumerable.Select(MoyFeatures, c => c / FeaturesList.Count).ToList();

            return MoyFeatures;
        }

        /// <summary>
        /// Add a pattern into the cluster
        /// </summary>
        /// <param name="pattern">Pattern to add</param>
        public void AddPattern(APattern pattern)
        {
            m_listPatterns.Add(pattern);
            
            m_NbPatterns++;
        }


        /// <summary>
        /// Add a list of patterns
        /// </summary>
        /// <param name="patternsToAdd">List of patterns to add</param>
        public void AddPatterns(List<APattern> patternsToAdd)
        {
            foreach(APattern pattern in patternsToAdd)
            {
                m_listPatterns.Add(pattern);
                m_NbPatterns++;
            }
        }

        /// <summary>
        /// Remove a pattern from the cluster (in memory)
        /// </summary>
        /// <param name="toRemove">Pattern to remove</param>
        /// <returns>true -> done - false -> pattern not found</returns>
        public bool RemovePattern(APattern toRemove)
        {
            // localisation  of the pattern 
            foreach (APattern pattern in Patterns)
            {
                if (pattern.Equals(toRemove)) 
                {
                    Patterns.Remove(pattern);
                    m_NbPatterns--;
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Sort the list of patterns according to distance to the reference pattern
        /// </summary>
        /// <param name="centroid">reference pattern </param>
        public void Order(APattern centroid)
        {
            //Reorganisatino of the list
            m_listPatterns.Sort(
                delegate(APattern pattern1, APattern pattern2)
                    {
                        //computation of distances from the centroïde
                        return (int)(pattern2.EuclidianDistance(centroid) - pattern1.EuclidianDistance(centroid));
                       //negatif -> pattern1 before pattern 2
                    }
                );
        }

        /// <summary>
        /// For Display purpose in GUI ListBox
        /// </summary>
        public override String ToString()
        {
            String description = "ID\t\t" + this.Id + "\n"
                + "Nb Shapes\t" + this.m_NbPatterns + "\n"
                + "Transcription\t";

            description += (this.m_IsLabelized) ? this.m_LabelList[0] : "-";

            return description;
        }

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
