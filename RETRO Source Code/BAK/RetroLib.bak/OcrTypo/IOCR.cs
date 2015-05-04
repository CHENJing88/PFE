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
using Retro.Model.core;
using System.IO;

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

    }
}