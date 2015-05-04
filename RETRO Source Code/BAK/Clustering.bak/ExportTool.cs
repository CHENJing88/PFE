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
using System.Xml;

namespace TestModule
{
    /// <summary>
    /// Export functionalities
    /// </summary>
    public class ExportTool
    {

        /// <summary>
        /// Export from temp file to PaRADIIT cluster xml files
        /// </summary>
        /// <param name="clusterdir">Path of the output directory</param>
        public static void Export2XML_JY(String clusterdir)
        {
            String internaltxtdir = clusterdir + @"\tmpclst";
            String internalimgdir = clusterdir + @"\tmpclstImages";
            int currentcluster = 0, lineNumber = 0;
            String shape, label;

            List<String> txtfiles = new List<String>(Directory.GetFiles(internaltxtdir, "*.txt", SearchOption.AllDirectories));

            foreach (String txtfile in txtfiles)
            {
                currentcluster++;
                label = "";
                lineNumber = 0;
                
                // Read the tmp cluster txt file
                StreamReader txtInput = new StreamReader(txtfile);

                // Read first line to check if it a cluster build from an existing Model
                shape = txtInput.ReadLine();
                bool existingModelCluster =  shape.StartsWith("EXISTING");

                // Get the associated label if existing model cluster
                if (existingModelCluster)
                    label = shape.Substring(shape.IndexOf('=') + 2);
                else
                    lineNumber++;

                // Get the number of lines/shapes
                while (!txtInput.EndOfStream)
                {
                    shape = txtInput.ReadLine();
                    lineNumber++;
                }
                txtInput.Close(); 
                txtInput.Dispose();


                if (existingModelCluster && (lineNumber == 1))
                {
                    // Don't export the cluster
                }
                else
                {
                    // Reopen
                    txtInput = new StreamReader(txtfile);

                    // Create the xml file
                    StreamWriter xmlOutput = new StreamWriter(clusterdir + @"\" + "cluster" + currentcluster + ".xml", false);
                    xmlOutput.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    xmlOutput.WriteLine("<cluster size=\"" + lineNumber.ToString() + "\" label=\"" + label + "\"" + ">");

                    // Read first line if existing model
                    if (existingModelCluster)
                        shape = txtInput.ReadLine();

                    // Process the rest of the txt file and upadte the xml output
                    while (!txtInput.EndOfStream)
                    {
                        shape = txtInput.ReadLine();
                        int pStart = shape.LastIndexOf('\\') + 1;
                        int pEnd = shape.LastIndexOf('.');
                        String id = (pEnd - pStart > 0)? shape.Substring(pStart, pEnd - pStart) : shape;

                        xmlOutput.WriteLine("\t<cc id=\"" + id + "\"/>");
                    }

                    xmlOutput.WriteLine("</cluster>");
                    xmlOutput.Close();
                    txtInput.Close();
                }
            }

            //Remove tmpclst directory
            if (Directory.Exists(internaltxtdir)) Directory.Delete(internaltxtdir, true);
            if (Directory.Exists(internalimgdir)) Directory.Delete(internalimgdir, true); 

        }


        /// <summary>
        /// Generate algorithms.xml output
        /// </summary>
        /// <param name="clusterdir">Path of the output directory</param>
        /// <param name="templateMatchingParameters">Parameters of the Template Matching algorithm</param>
        public static void CreateAlgorithmsXml(String clusterdir, TemplateMatchingParameters templateMatchingParameters)
        {
            StreamWriter xmlOutput = new StreamWriter(clusterdir + @"\" + "algorithms.xml");

            xmlOutput.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlOutput.WriteLine("<root>");
            xmlOutput.WriteLine("\t<algo id=\"1\" name=\"incremental\">");
            xmlOutput.WriteLine("\t\t<param name=\"binarization\" value=\"" + templateMatchingParameters.BINARIZE_FOR_COMPARISON + "\" desc=\"SIS binarization before template matching\" />");
            xmlOutput.WriteLine("\t\t<param name=\"normalization\" value=\"" + templateMatchingParameters.NORMALIZE_BEFORE_COMPARISON + "\" desc=\"20x20 normalization before template matching\" />");
            xmlOutput.WriteLine("\t\t<param name=\"denoising\" value=\"" + templateMatchingParameters.DENOISE_BOUNDING_BOX + "\" desc=\"denoising before template matching\" />");
            xmlOutput.WriteLine("\t\t<param name=\"threshold\" value=\"" + templateMatchingParameters.TEMPLATE_MATCHING_THRESHOLD + "\" desc=\"template matching threshold\" />");
            xmlOutput.WriteLine("\t</algo>");
            xmlOutput.WriteLine("</root>");
            xmlOutput.Close();
        }


        /// <summary>
        /// Generate stats.xml output
        /// </summary>
        /// <param name="clusterdir">Path of the output directory</param>
        public static void CreateStatsXml(String clustersdir)
        {
            // Create the dictionary
            Dictionary<int, int> histogram = new Dictionary<int, int>();

            // Create abscisse segments of the histogram
            histogram.Add(1, 0);
            histogram.Add(2, 0);
            histogram.Add(3, 0);
            histogram.Add(4, 0);
            histogram.Add(5, 0);
            histogram.Add(10, 0);
            histogram.Add(50, 0);
            histogram.Add(100, 0);

            // Get the Cluster files from directory
            List<String> clusterList = new List<String>(Directory.GetFiles(clustersdir, "*.xml"));
            clusterList.RemoveAll(notClusterXML);
            XmlDocument xmlDoc = new XmlDocument();
            int nbShapesTotal = 0;

            // Parse clusters files and fill the dictionary
            foreach (String cluster in clusterList)
            {
                //Console.WriteLine(cluster);

                // Load the xml file
                xmlDoc.Load(cluster);
                // Get the <cluster> node
                XmlElement clusterElement = (XmlElement)xmlDoc.SelectSingleNode("cluster");
                // Get the number of shapes of this cluster
                int nbShapes = Convert.ToInt32(clusterElement.Attributes["size"].Value);

                // Update the total shapes number
                nbShapesTotal += nbShapes;

                // Naive insertion in the histogram
                switch (nbShapes)
                {
                    case 1:
                        histogram[1]++;
                        break;
                    case 2:
                        histogram[2]++;
                        break;
                    case 3:
                        histogram[3]++;
                        break;
                    case 4:
                        histogram[4]++;
                        break;
                    case 5:
                        histogram[5]++;
                        break;
                    default:
                        if (nbShapes >= 100)
                        {
                            histogram[100]++;
                        }
                        else if (nbShapes >= 50)
                        {
                            histogram[50]++;
                        }
                        else
                        {
                            histogram[10]++;
                        }
                        break;
                }
            }

            // Export results in a txt file
            StreamWriter xmlOutput = new StreamWriter(clustersdir + @"\" + "stats.xml", false);

            xmlOutput.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlOutput.WriteLine("<stats>");
            xmlOutput.WriteLine("\t<clusters count=\"" + clusterList.Count + "\" />");
            xmlOutput.WriteLine("\t<shapes count=\"" + nbShapesTotal + "\" />");
            xmlOutput.WriteLine("\t<histogram>");
            foreach (KeyValuePair<int, int> pair in histogram)
                xmlOutput.WriteLine("\t\t<bin nbItems=\"" + pair.Key + "\" nbClusters=\"" + pair.Value + "\" />");
            xmlOutput.WriteLine("\t</histogram>");
            xmlOutput.WriteLine("</stats>");
            xmlOutput.Close();
        }


        /// <summary>
        /// Check if the xml file is a clusterXXXX.xml 
        /// </summary>
        /// <param name="s">Name of the file</param>
        /// <returns>True is the file isn't a cluster xml file, esle False</returns>
        private static bool notClusterXML(String s)
        {
            return !Path.GetFileNameWithoutExtension(s).StartsWith("cluster");
        }
    }
}
