using AForge.Imaging.Filters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Polytech.Clustering.Plugin
{
    /// <summary>
    /// Alto Generic Reader Plugin 
    /// </summary>    
    public class AltoGenericReaderPlugin : IDocumentReaderPlugin
    {
        /// <summary>
        /// Database of documentsSafe containing Patterns to cluster
        /// </summary>
        Database m_database = null;

        /// <summary>
        /// Class that embbed the configuration of this plugin
        /// </summary>
        AGRConfig m_config = null;

        /// <summary>
        /// Load the database in memory
        /// </summary>        
        public Database LoadDatabase(List<IDescriptorPlugin> descriptors, string agoraPath, Form mainWindow, System.Delegate changeState)
        {
            //Performemce and stats
            Stopwatch newWatch = new Stopwatch();
            newWatch.Start();

            m_database = new Database();

            //Create the list of string with the name of the Alto files to read in the agoraPath directory
            string[] xmlPaths = Directory.GetFiles(agoraPath + "\\alto", "*.xml");
            
            //XML Alto Document list
            List<XmlDocument> xmlDocumentList = new List<XmlDocument>();
            
            //Shared list of documentsSafe to put in the database (thread safe)
            BlockingCollection<Document> documentsSafe = new BlockingCollection<Document>();
            Task loadDbTask = Task.Factory.StartNew(() =>
            {
                    Parallel.ForEach(xmlPaths, xmlPath =>
                    {
                        //Document creation in the shared list
                        Document newDoc = new Document(xmlPath);
                        documentsSafe.Add(newDoc);

                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocumentList.Add(xmlDocument);
      
                        //Get the name of the file without path and extension corresponding to the alto file
                        string fName = Path.GetFileName(xmlPath);
                        fName = Path.ChangeExtension(fName, null); //extension suppression
                                                                        
                        //Load Alto file in XmlDocument
                        xmlDocument.Load(xmlPath);

                        //Get the desired XML elements
                        XmlNodeList elementList = xmlDocument.GetElementsByTagName(m_config.ElementTag);
                        
                        //Get the paths of the corresponding full images (png and jpg)
                        string[] imgPaths = Directory.GetFiles(agoraPath + "\\images\\", fName + ".*");
                        
                        //For the pattern creation
                        int hpos, vpos, width, height;
                        string eocId1;
                        
                        foreach (XmlNode node in elementList) //pour chaque TAG string
                        {
                            if (node.Attributes["STYLEREF"].Value == m_config.StyleRefTag)
                            {
                                //We get the pattern / node
                                hpos = Convert.ToInt32(node.Attributes["HPOS"].Value);
                                vpos = Convert.ToInt32(node.Attributes["VPOS"].Value);
                                width = Convert.ToInt32(node.Attributes["WIDTH"].Value);
                                height = Convert.ToInt32(node.Attributes["HEIGHT"].Value);
                                eocId1 = node.Attributes["ID"].Value;

                                //Pattern creation
                                ShapeEoC newPattern = null;
                                newPattern = new ShapeEoC(eocId1, agoraPath + "\\alto", hpos, vpos, width, height, imgPaths[0]); //0 can correspond to either png or jpg version of the same full image)

                                //Computation of the requested signatures (null => no signature to compute)
                                foreach (IDescriptorPlugin plugin in descriptors)
                                {
                                    plugin.CalculateSignature(newPattern);
                                }

                                //Add pattern into the document
                                newDoc.AddPattern(newPattern);

                                newPattern.Dispose(); //Free its image from memory
                            }

                            mainWindow.Invoke(changeState/*new SetToolStripStateDelegate(m_mainWindow.ChangeToolstripState) */, new object[] { true, "Loading : " + (xmlPaths.Length - documentsSafe.Count) });
                        }
                    });
                    //complétion of the shared document list 
                    documentsSafe.CompleteAdding();
            });

            loadDbTask.Wait();

            //Copy the shared list into the final list
            foreach (Document doc in documentsSafe)
            {
                    m_database.AddDocument(doc);
            }

            newWatch.Stop();

            //Save time in stats file
            System.IO.StreamWriter file = new System.IO.StreamWriter(agoraPath+"\\time.txt");
            file.WriteLine("DATABASE : " + newWatch.Elapsed + "--");
            file.Close();

            return m_database;
        }

         /// <summary>
        /// Constructor for the Main Class of the Plugin 
        /// </summary>
        public AltoGenericReaderPlugin(string elementtag,string styletag)
        {
            m_config = new AGRConfig(elementtag,styletag);
        }

        /// <summary>
        /// Loading Time management
        /// </summary>
        public TimeSpan GetProcessingTime()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// To get the loaded database
        /// </summary>       
        public Database GetLoadedDatabase()
        {
            return m_database;
        }

        /// <summary>
        /// To get the author of the plugin
        /// </summary>        
        public string GetAuthor()
        {
            return "JY Ramel";
        }

        /// <summary>
        /// Form to configure the plugin
        /// </summary>       
        Form IDocumentReaderPlugin.GetConfigWindow()
        {
            return null;
        }

        /// <summary>
        /// Class that embbed the configuration of this plugin
        /// </summary>        
        public IConfig GetConfig()
        {
            return m_config;
        }

        /// <summary>
        /// Get the name of this plugin
        /// </summary>        
        public string GetName()
        {
            return "AltoGenericReader";
        }

        /// <summary>
        /// Not implemented yet
        /// </summary>        
        public Database LoadDatabase(List<IDescriptorPlugin> descriptors, string path)
        {
            throw new NotImplementedException();
        }
        
    }
}
