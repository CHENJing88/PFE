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

using AForge.Imaging;
using System.Drawing;
using AForge.Imaging.Filters;



namespace Polytech.Clustering.Plugin
{
    /// <summary>
    ///  Define Bounding Box Denoising functionalities
    ///  Same as Clustering.BoundingBoxNoiseRemoval
    /// </summary>
    public class BoundingBoxNoiseRemoval
    {
        /// <summary>
        /// Isolate main Connected Component in a Bounding Box of a character
        /// Requirement: White foreground and black background
        /// </summary>
        /// <param name="img">Bitmap of a character</param> 
        public static void RemoveNoise(Bitmap img)
        {
            // Get Blobs
            BlobCounter bc = new BlobCounter(); 
            bc.ProcessImage(img);
            Rectangle[] listCC = bc.GetObjectsRectangles();
            if (listCC.Length > 1)
            {
                foreach (Rectangle childCC in listCC)
                {
                    // Lancement du nettoyage récursif des CC parasites
                    if (img.Size != childCC.Size)
                    {
                        RemoveCC(img, childCC);
                    }
                }
            }

        }

        /// <summary>
        /// Remove a specified CC in the whole image
        /// </summary>
        /// <param name="img">Whole image of the Character</param>
        /// <param name="cc">Current RoI to consider</param>
        public static void RemoveCC(System.Drawing.Bitmap img, Rectangle cc)
        {
            CanvasFill filterFill;
            CanvasCrop filterCrop;

            // Patch, because when CC is too small, issues with AForge algo
            if ((cc.Width <= 1) && (cc.Height <= 1))
            {
                // We set the CC to black
                filterFill = new CanvasFill(cc, (byte)0);
                filterFill.ApplyInPlace(img);
                return;
            }

            // Clone original image to work on it and remove everything around the considered CC
            Bitmap tmp = (Bitmap)img.Clone();
            filterCrop = new CanvasCrop(cc, (byte)0);
            filterCrop.ApplyInPlace(tmp);

            // Get remaining CC
            BlobCounter bc = new BlobCounter(); 
            bc.ProcessImage(tmp);

            Rectangle[] listCC = bc.GetObjectsRectangles();
            foreach (Rectangle childCC in listCC)
            {
                if (cc.Size != childCC.Size)
                {
                    // Recursive noise removal
                    RemoveCC(tmp, childCC);
                }
            }

            // Get cleaned image
            Subtract filterSub = new Subtract(tmp);
            filterSub.ApplyInPlace(img);

            //img.Save(@"C:\WordSpottingTemp\cleaned.jpg");
        }

        
    }
}
