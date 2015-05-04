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
using System.Xml;

namespace RetroGUI.util
{
    /// <summary>
    /// Define various functions as ont shot scripts
    /// </summary>
    public static class Scripts
    {
        /// <summary>
        /// Update alto xml file:
        ///   _ add ":alto" to one of the xmlns in the metadata
        ///   _ add WIDTH and HEIGHT Attributes in the Page tag
        /// </summary>
        public static void AddAltoAttributes()
        {
            // Directory paths
            String altoDirectory = @"C:\Users\frédéric.rayar\Documents\tmp\ClusteringTestData2\alto";
            String imageDirectory = @"C:\Users\frédéric.rayar\Documents\tmp\ClusteringTestData2\images";
            String imageExtension = ".jpg";
            

            if ((Directory.Exists(altoDirectory)) && (Directory.Exists(imageDirectory)))
            {
                // Get list of alto xml file
                String[] altoFiles = Directory.GetFiles(altoDirectory, "*.xml");

                foreach (String altoFile in altoFiles)
                {
                    // Get the name of the associated image file
                    String imagename = Path.GetFileNameWithoutExtension(altoFile);
                    Bitmap bitmap = (Bitmap)Bitmap.FromFile(imageDirectory + @"\" + imagename + imageExtension);

                    // Get Dimensions of the image
                    int imageWidth = bitmap.Width;
                    int imageHeight = bitmap.Height;

                    // Release image
                    bitmap.Dispose();

                    // Open alto xml file
                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();

                    // Check if XML file exist
                    if (File.Exists(altoFile))
                    {
                        // Get text
                        String text = File.ReadAllText(altoFile);

                        // Update xmlns attibutes in metadata
                        int pos = text.LastIndexOf("xmlns") + 5;
                        text = text.Insert(pos, ":alto");

                        // Update Page dimensions
                        pos = text.IndexOf("<Page") + 5;
                        text = text.Insert(pos, " WIDTH=\"" + imageWidth + "\" HEIGHT=\"" + imageHeight + "\"");

                        // Save text
                        File.WriteAllText(altoFile, text);
                    }
                }
            }
        }

    }
}
