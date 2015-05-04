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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using RetroUtil;
using Retro.Model.ocr;
using Retro.Model.core;
using Retro.ViewModel;
using AForge.Imaging;
using AForge.Imaging.Filters;


namespace Retro.ocr
{

    /// <summary>
    /// OCR Engine using Template Matching
    /// </summary>
    public class TemplateMatchingOCREngine : IOCR
    {
        // Attributes
        private ResizeNearestNeighbor filterResize;
        private Grayscale filterG = new Grayscale(0.2125, 0.7154, 0.0721);
        private SISThreshold filterB = new SISThreshold();


        /// <summary>
        /// Default Constructor
        /// </summary>
        public TemplateMatchingOCREngine()
        {
        }


        /// <summary>
        /// Run the OCR process
        /// </summary>
        /// <param name="clusterList">List of non labeled clusters</param>
        /// <param name="fontModelList">List og labeled Font Model</param>
        /// <param name="dynamicSplashScreenNotification">Dynamic Splashscreen Notification</param>
        /// <returns>Number of cluster automatically transcripted</returns>
        public int RunOCR(List<Cluster> clusterList, List<FontModel> fontModelList, DynamicSplashScreenNotification dynamicSplashScreenNotification)
        {
            List<String> tmpFontModelImagesPath = new List<String>();
            int nbTranscribedClusters = 0;
            float threshold = 0.85f;

            // Build temp directory
            String tmpPath = @"\tmpModelsImages\";
            if (Directory.Exists(tmpPath)) { Directory.Delete(tmpPath, true); System.Threading.Thread.Sleep(500); }
            Directory.CreateDirectory(tmpPath);

            // Create tmp font models images
            foreach(FontModel model in fontModelList)
            {
                // Open model
                String modelPath = model.Directory + @"\" + model.ThumbnailName;
                Bitmap modelBitmap = (Bitmap)Bitmap.FromFile(modelPath);
                // Preprocess image
                this.PreprocessImage(ref modelBitmap);
                // Save image in tmp directory
                modelBitmap.Save(tmpPath + @"\" + model.ThumbnailName, ImageFormat.Png);
                modelBitmap.Dispose();
                // Add in the list of tmp model image
                tmpFontModelImagesPath.Add(tmpPath + @"\" + model.ThumbnailName);
            }

            // Process each cluster
            foreach (Cluster cluster in clusterList)
            {
                // Notify the ViewModel
                dynamicSplashScreenNotification.Message = "Processing cluster " + cluster.ClusterId + "/" + clusterList.Count;

                // Init loop attributes
                float similarityMax = 0.0f;
                int mostSililarFontModelIndex = -1;

                if (File.Exists(cluster.RepresentativePathToBitmap))
                {
                    Bitmap representativeBitmap = (Bitmap)Bitmap.FromFile(cluster.RepresentativePathToBitmap);
                    this.PreprocessImage(ref representativeBitmap);

                    // Compare with the known cluster representatives
                    for (int it = 0; it < fontModelList.Count; it++)
                    {
                        Bitmap fontmodelBitmap = (Bitmap)Bitmap.FromFile(tmpFontModelImagesPath[it]);
                        float similarity = this.ComputeSimilarity(ref representativeBitmap, ref fontmodelBitmap);
                        if (similarity > similarityMax)
                        {
                            similarityMax = similarity;
                            mostSililarFontModelIndex = it;
                        }

                        fontmodelBitmap.Dispose();
                    }

                    // Free representative image
                    representativeBitmap.Dispose();

                    // Assign the matched FontModel label to the cluster
                    if ((similarityMax > threshold) && (mostSililarFontModelIndex > 0))
                    {
                        cluster.AddNewLabel("TEMPLATE_MATCHING", fontModelList[mostSililarFontModelIndex].TranscriptionCharacter, similarityMax);
                        cluster.IsLabelized = true;
                        nbTranscribedClusters++;
                    }
                }
            }

            // Remove tmps directory
            Directory.Delete(tmpPath, true);

            return nbTranscribedClusters;
        }


        /// <summary>
        /// PreProcess the image: binarisation, and normalisation (h=20) with the aspect ratio kept
        /// </summary>
        /// <param name="image">Bitmap image to preprocess</param>
        private void PreprocessImage(ref Bitmap image)
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


        /// <summary>
        /// Compute similarity between two binary shapes.
        /// May not have the same dimensions
        /// Note: Exactly the same function as in Clustering.ClusteringTool2
        /// </summary>
        /// <param name="image1">ShapeEoC 1</param>
        /// <param name="image2">ShapeEoC 2</param>
        /// <returns>Similary between the two shapes in [0,1]</returns>
        private float ComputeSimilarity(ref Bitmap image1,ref Bitmap image2)
        {
            int m_width = Math.Min(image1.Width, image2.Width);
            int m_height = Math.Min(image1.Height, image2.Height);
            int x,y,x1,y1,x2,y2;
            byte pimg_xy, pref_xy;
            long diff = 0;

            // lock images
            BitmapData imageData1 = image1.LockBits( new Rectangle( 0, 0, image1.Width, image1.Height ), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );
            BitmapData imageData2   = image2.LockBits( new Rectangle( 0, 0, image2.Width, image2.Height ), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );
            UnmanagedImage u_image = new UnmanagedImage( imageData1 );
            UnmanagedImage u_ref = new UnmanagedImage( imageData2 );

            int offset_x1 = image1.Width / 2 - m_width / 2;
            int offset_y1 = image1.Height / 2 - m_height / 2;
            int offset_x2 = image2.Width / 2 - m_width / 2;
            int offset_y2 = image2.Height / 2 - m_height / 2;

            try
            {
                // process the 2 images
                unsafe
                {
                    byte* ptr = (byte*)u_image.ImageData.ToPointer();
                    byte* ptr2 = (byte*)u_ref.ImageData.ToPointer();

                    for( x = 0 ; x < (m_width-1) ;x++)
                    {
                        for( y = 0 ; y < (m_height-1) ;y++)
                        {
                            x1 = offset_x1 + x;
                            y1 = offset_y1 + y;
                            x2 = offset_x2  + x;
                            y2 = offset_y2 + y;
                 
                            pimg_xy = (*(ptr + x1 + y1 * u_image.Stride));                           
                            pref_xy = (*(ptr2 + x2 + y2 * u_ref.Stride));

                            diff = diff + (long)Math.Abs(pimg_xy - pref_xy);
                        }
                    }
                }
           }
           finally
           {
                // unlock images
                image1.UnlockBits( imageData1 );
                image2.UnlockBits( imageData2 );                
           }

            return (1 - (float)diff / (float)(m_width * m_height * 255));
        }

    }
}