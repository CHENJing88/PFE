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
using System.Drawing;
using AForge.Imaging.Filters;
using System.Drawing.Imaging;
using Polytech.Clustering.Plugin;
using System.Xml;
using System.Xml.Linq;



namespace Retro.ocr
{
    /// <summary>
    /// Interface of OCR Engines that will be embedded in Retro
    /// </summary>
    public class IOCR
    {

        /// <summary>
        /// Default Constructor
        /// </summary>
        public IOCR()
        {

        }


        /// <summary>
        /// Get all the Font Model of a selected directory.
        /// A Font Model is a triplet {*.png, *.xml, *_bw.png}
        ///  Note: Same idea as Clustering.ClusteringTool2.HandleInitialModels() but not exactly the same processes
        /// </summary>
        /// <param name="directory"> Path of the existing models (TopDirectory only). Existence has been check by the caller</param>
        /// <returns>List of found Font Model</returns>
        public List<FontModel> GetFontModels(String directory)
        {
            List<FontModel> fontModelList = new List<FontModel>();

            // Get the models (images + xml)
            String[] files = Directory.GetFiles(directory, "*.xml");

            // Process each xml file
            foreach (String file in files)
            {
                String filename = Path.GetFileNameWithoutExtension(file);
                if ((File.Exists(directory + @"\" + filename + ".png")) && (File.Exists(directory + @"\" + filename + "_bw.png")))
                {
                    // Create a new FontModel from the xml path
                    FontModel fontmodel = new FontModel(file);

                    // Add the newly created FontModel in the list
                    fontModelList.Add(fontmodel);
                }
            }
            return fontModelList;
        }

        #region Processus Image

        private ResizeNearestNeighbor filterResize;
        private Grayscale filterG = new Grayscale(0.2125, 0.7154, 0.0721);
        private SISThreshold filterB = new SISThreshold();
        /// <summary>
        /// PreProcess the image: binarisation, and normalisation (h=20) with the aspect ratio kept
        /// </summary>
        /// <param name="image">Bitmap image to preprocess</param>
        protected void PreprocessImage(ref Bitmap image)
        {

            // Normalisation
            this.filterResize = new ResizeNearestNeighbor((int)Math.Round(((float)image.Width / (float)image.Height) * 20), 20);
            image = this.filterResize.Apply(image);

            // Convert to Greyscale if needed
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
                image = this.filterG.Apply(image);

            // Binarisation
            image = this.filterB.Apply(image);

            //Denoising
            //filterI.ApplyInPlace(image);
            //BoundingBoxNoiseRemoval.RemoveNoise(image);
        }
        #endregion

        #region Load signature of pattern

        public void LoadPatternSignature(APattern Pattern)
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
        #endregion
    }
}