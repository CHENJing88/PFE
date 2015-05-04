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
using System.Windows;
using System.Xml;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;

using AForge.Imaging;
using AForge.Imaging.Filters;




namespace RetroUtil
{
    /// <summary>
    /// Effective clustering from raw data location
    /// </summary>
    public class ClusteringTool2
    {
        #region Attributes

        private bool bIllustrationClustering;
        private TemplateMatchingParameters oTemplateMatchingParameters;
        private String imagesDir;
        private String altoDir;
        private String clustersDir;
        private long clusterNb;
        private XmlDocument xmlDocument = new XmlDocument();
        private List<String> representatives = new List<String>();

        private String tmpPath;
        private String outputpath;

        // Aforge filters
        private Grayscale filterG = new Grayscale(0.2125, 0.7154, 0.0721);
        private SISThreshold filterB = new SISThreshold();
        private Invert filterI = new Invert();
        private ResizeNearestNeighbor filterResize;
        private Crop filterCrop;

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="agoraProjectDir">Path of the agora project, must contains alto/ and images/ subfolders</param>
        /// <param name="existingModelsDir">Path of the directory of Models to considerer during the clustering (existence must been check by the caller)</param>
        /// <param name="clustersDir">Path of the output directory for clusterNNNNN.xml files</param>
        /// <param name="parameters">TemplateMacthing algorithm parameter</param>
        /// <param name="bIllustrationClustering">Define if illustration or text EoC are being considered</param>
        public ClusteringTool2(String agoraProjectDir, String existingModelsDir, String clustersDir, TemplateMatchingParameters parameters, bool bIllustrationClustering)
        {
            // Check if directory exists
            if (this.CheckDirectoryExistence(agoraProjectDir, clustersDir))
                return;

            // Remove existing clusters files
            Array.ForEach(Directory.GetFiles(clustersDir), delegate(string path) { File.Delete(path); });

            // Build temp directory
            this.tmpPath = clustersDir + @"\tmpclstImages\";
            if (Directory.Exists(tmpPath)) { Directory.Delete(tmpPath, true); System.Threading.Thread.Sleep(500); }
            Directory.CreateDirectory(tmpPath);
            this.outputpath = clustersDir + @"\tmpclst";
            if (Directory.Exists(outputpath)) { Directory.Delete(outputpath, true); System.Threading.Thread.Sleep(500); }
            Directory.CreateDirectory(outputpath);

            // Get attributes
            this.bIllustrationClustering = bIllustrationClustering;
            this.oTemplateMatchingParameters = parameters;
            this.imagesDir = agoraProjectDir + @"\images\";
            this.altoDir = agoraProjectDir + @"\alto\";
            this.clustersDir = clustersDir;
            this.clusterNb = 0;

            // Handle initial existing models
            this.HandleInitialModels(existingModelsDir);

            // Get the Alto.String images to cluster from directory
            List<String> images = new List<String>(Directory.GetFiles(agoraProjectDir + @"\images"));

            // Create DynamicSplashScreenNotification
            DynamicSplashScreenNotification dynamicSplashScreenNotification = new DynamicSplashScreenNotification();
            dynamicSplashScreenNotification.Message = "Starting Automatic Transcription process";

            // Create DynamicSplashScreen thread
            System.Threading.Thread dynamicSplashScreenThread = new System.Threading.Thread((object parameter) =>
            {
                List<String> processName = new List<String>();
                processName.Add("Clustering");
                processName.Add("Process");
                DynamicSplashScreen dynamicSplashScreen = new DynamicSplashScreen(processName, (DynamicSplashScreenNotification)parameter);
                dynamicSplashScreen.ShowDialog();
            });

            // Start DynamicSplashScreen thread
            dynamicSplashScreenThread.SetApartmentState(ApartmentState.STA);
            dynamicSplashScreenThread.Start(dynamicSplashScreenNotification);

            DateTime startTime = DateTime.Now;

            // Call cluster method 
            int currentImage = 1;
            foreach (String image in images)
            {
                // Notify the user
                dynamicSplashScreenNotification.Message = "Processing Image " + currentImage + "/" + images.Count + "\n";
                dynamicSplashScreenNotification.Message += clusterNb + " computed clusters for now ...";
                
                // Call Cluster method on the current image
                if (this.bIllustrationClustering)
                    this.FR_Illustration_Cluster(image);
                else
                    this.FR_Characters_Cluster(image);

                // Update current image iterator
                currentImage++;
            }

            // Export from internal files to xml files
            dynamicSplashScreenNotification.Message = "Exporting " + clusterNb.ToString() + " clusters";
            ExportTool.Export2XML_JY(clustersDir); 
            
            // Create algorithms.xml
            ExportTool.CreateAlgorithmsXml(clustersDir, parameters);

            // Compute processing duration
            DateTime stopTime = DateTime.Now;
            TimeSpan duration = stopTime - startTime;

            // End the DynamicSplashScreen Thread
            dynamicSplashScreenNotification.Message = "End of the Clustering. \nProcessing time: " + duration;
            dynamicSplashScreenThread.Abort();


            // Display notification
            var result = MessageBox.Show("End of the Clustering. \nProcessing time: " + duration);
            
            //var result = MessageBox.Show("Do you want to load Clusters now ?", "End of Process Clusters", MessageBoxButton.YesNo);
            //if (result == MessageBoxResult.Yes)
            //{
            //    //Click_Clustering_LoadClusters(sender, e);
            //}
            //else if (result == MessageBoxResult.No)
            //{

            //}

        }


        /// <summary>
        /// Check mandatory directories existence
        /// </summary>
        /// <param name="agoraProjectDir">Path of the agora project scenario, must contains alto/ and images/ subfolders</param>
        /// <param name="clustersDir">Output directory for clusterNNNNN.xml files</param>
        /// <returns>True if a directory doens't exist, else False</returns>
        private bool CheckDirectoryExistence(String agoraProjectDir, String clustersDir)
        {
            if (!Directory.Exists(agoraProjectDir))
            {
                MessageBox.Show("ERROR: Directory " + agoraProjectDir + " doesn't exist");
                return true;
            }
            if (!Directory.Exists(agoraProjectDir + @"\images"))
            {
                MessageBox.Show("ERROR: Directory " + agoraProjectDir + @"\images" + " doesn't exist");
                return true;
            }
            if (!Directory.Exists(agoraProjectDir + @"\alto"))
            {
                MessageBox.Show("ERROR: Directory " + agoraProjectDir + @"\alto" + " doesn't exist");
                return true;
            }
            if (!Directory.Exists(clustersDir))
            {
                MessageBox.Show("ERROR: Directory " + clustersDir + " doesn't exist");
                return true;
            }
            return false;
        }


        /// <summary>
        /// Handle eventual initialModels.
        /// A Font Model is a triplet {*.png, *.xml, *_bw.png}
        /// </summary>
        /// <param name="existingModelsDir">Path of the existing models (TopDirectory only)</param>
        public void HandleInitialModels(String existingModelsDir)
        {
            if (existingModelsDir.CompareTo("") != 0)
            {
                // Initialize attributes
                String outputpath_ = this.outputpath + "\\clst_";
                TextWriter tw = new StreamWriter(this.outputpath + @"\delete.xml");
                tw.Close();
                File.Delete(this.outputpath + @"\delete.xml");

                // Get the models (images + xml)
                String[] files = Directory.GetFiles(existingModelsDir, "*.xml");

                // Process each xml file
                foreach (String file in files)
                {
                    String filename = Path.GetFileNameWithoutExtension(file);
                    if ( (File.Exists(existingModelsDir + @"\" + filename + ".png")) && (File.Exists(existingModelsDir + @"\" + filename + "_bw.png")) )
                    {
                        // Load model grayscale image 
                        Bitmap image = (Bitmap)Bitmap.FromFile(existingModelsDir + @"\" + filename + ".png");

                        // Preprocess the image
                        this.PreprocessImage(ref image);

                        // Save the preprocessed image  
                        String path_ = this.tmpPath + this.clusterNb + ".png";
                        image.Save(path_, ImageFormat.Png);

                        // Add as a new representative
                        this.representatives.Add(path_);

                        // Load the xml
                        XmlDocument modelXML = new XmlDocument();
                        modelXML.Load(file);

                        // Get the label
                        String label = modelXML.GetElementsByTagName("Transcription")[0].Attributes["Character"].Value;

                        // Assign the current image to the chosen cluster (file)                
                        tw = new StreamWriter(outputpath_ + this.clusterNb.ToString() + ".txt", true);
                        tw.WriteLine("EXISTING MODEL - LABEL = " + label , true);
                        //tw.WriteLine(filename + ".png" + " -  1");
                        tw.Close();
                        tw.Dispose();

                        // Increase Cluster count
                        this.clusterNb++;
                    }
                }
            }
        }

 
        /// <summary>
        /// Characters TestModule
        /// </summary>
        /// <param name="imagepath">Path of the document image to consider</param>
        private void FR_Characters_Cluster(String imagepath)
        {
            // Get the alto path of the image
            String altopath = this.altoDir + Path.GetFileNameWithoutExtension(imagepath) + ".xml";

            if (!File.Exists(altopath))
                return ;

            // Get the String elements
            this.xmlDocument.Load(altopath);
            XmlNodeList stringList = xmlDocument.GetElementsByTagName("alto:String");

            // Open the image
            Bitmap imageSource = (Bitmap)Bitmap.FromFile(imagepath);

            // TestModule attributes
            //long shapeCpt = 0;
            float similarityMax = 0.0f, similarity = 0.0f;
            long mostSililarFontModelIndex = -1;
            Bitmap image, ref_image;

            // Build tmp directory
            String outputpath_ = outputpath + "\\clst_";

            //Put first image in first cluster
            TextWriter tw = new StreamWriter(outputpath_+ "delete.xml");
            tw.Close();
            File.Delete(outputpath_ + "delete.xml");

            // Process each String
            String imageName;
            int width, height, hpos, vpos;
            foreach (XmlNode node in stringList)
            {
                // Init loop attributes
                similarityMax = 0.0f;
                mostSililarFontModelIndex = -1;
                similarity = 0.0f;

                // Get Bounding box
                hpos = Convert.ToInt32(node.Attributes["HPOS"].Value);
                vpos = Convert.ToInt32(node.Attributes["VPOS"].Value);
                width = Convert.ToInt32(node.Attributes["WIDTH"].Value);
                height = Convert.ToInt32(node.Attributes["HEIGHT"].Value);
                imageName = node.Attributes["ID"].Value;

                // Get cropped String
                filterCrop = new Crop(new Rectangle(hpos, vpos, width, height));
                image = filterCrop.Apply(imageSource);

                // Prepocess image
                this.PreprocessImage(ref image);

                // Compare current "image" with the known cluster representatives "ref_image"
                for (int it = 0; it < this.representatives.Count; it++)
                {
                    // Load and prepare the reference image
                    ref_image = (Bitmap)Bitmap.FromFile(this.representatives[it]);

                    //if ((ref_image.Width / image.Width > 0.2) && (ref_image.Width / image.Width < 2))
                    if (Math.Abs(ref_image.Width - image.Width) < 5)
                    {
                        // Compute Similarity value 
                        similarity = ComputeSimilarity(ref image, ref ref_image);

                        //compare similarities
                        if (similarity > similarityMax)
                        {
                            similarityMax = similarity;
                            mostSililarFontModelIndex = it;
                        }
                    }

                    // Free memory
                    ref_image.Dispose();
                }

                // Evaluate the find maximum similarity
                if ((similarityMax < this.oTemplateMatchingParameters.TEMPLATE_MATCHING_THRESHOLD) || (mostSililarFontModelIndex < 0))
                {
                    //Create a New cluster by adding new representative     
                    String path_ = this.tmpPath + this.clusterNb + ".png";
                    this.representatives.Add(path_);
                    image.Save(path_, ImageFormat.Png);

                    // And select this cluster as the most similar 
                    mostSililarFontModelIndex = this.clusterNb;

                    // Increase Cluster count
                    this.clusterNb++;
                }

                // Assign the current image to the chosen cluster (file)                
                tw = new StreamWriter(outputpath_ + mostSililarFontModelIndex.ToString() + ".txt", true);
                tw.WriteLine(imageName + ".png" + " - " + similarityMax.ToString());
                //if (imageName != images[0]) tw.WriteLine(imageName + " - " + similarityMax.ToString());

                tw.Close();
                tw.Dispose();

                // Free memory
                image.Dispose();

                // Close the stream
                tw.Close();
                tw.Dispose();
            }

            //Force Garbage Collector to free memory
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }


        /// <summary>
        /// PreProcess the cropped EoC regarding TemplateMatchong parameters
        /// </summary>
        /// <param name="image">Bitmap image to preprocess</param>
        private void PreprocessImage(ref Bitmap image)
        {
            // Normalization
            if (this.oTemplateMatchingParameters.NORMALIZE_BEFORE_COMPARISON)
            {
                //preserve aspect ration 
                filterResize = new ResizeNearestNeighbor((int)Math.Round(((float)image.Width / (float)image.Height) * 20), 20);
                image = filterResize.Apply(image);
            }

            //Convert to Greyscale
            if (image.PixelFormat != PixelFormat.Format8bppIndexed) image = filterG.Apply(image);

            // Binarisation
            if (this.oTemplateMatchingParameters.BINARIZE_FOR_COMPARISON)
            {
                image = filterB.Apply(image);

                //Denoising
                if (this.oTemplateMatchingParameters.DENOISE_BOUNDING_BOX)
                {
                    filterI.ApplyInPlace(image);
                    BoundingBoxNoiseRemoval.RemoveNoise(image);
                }
            }
        }


        /// <summary>
        /// Compute similarity between two binary shapes.
        /// May not have the same dimensions
        /// </summary>
        /// <param name="image1">Shape 1</param>
        /// <param name="image2">Shape 2</param>
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


        /// <summary>
        /// Illustration TestModule
        /// </summary>
        private void FR_Illustration_Cluster(String imagepath)
        {
            // Get the alto path of the image
            String altopath = this.altoDir + Path.GetFileNameWithoutExtension(imagepath) + ".xml";

            if (!File.Exists(altopath))
                return;

            // Get the String elements
            this.xmlDocument.Load(altopath);
            XmlNodeList stringList = xmlDocument.GetElementsByTagName("alto:Illustration");

            // Open the image
            Bitmap imageSource = (Bitmap)Bitmap.FromFile(imagepath);

            // TestModule attributes
            //long shapeCpt = 0;
            float similarityMax = 0.0f, similarity = 0.0f;
            long mostSililarFontModelIndex = -1;
            Bitmap image, ref_image;

            // Build tmp directory
            String outputpath_ = outputpath + "\\clst_";

            //Put first image in first cluster
            TextWriter tw = new StreamWriter(outputpath_ + "delete.xml");
            tw.Close();
            File.Delete(outputpath_ + "delete.xml");

            // Process each String
            String imageName;
            int width, height, hpos, vpos;
            foreach (XmlNode node in stringList)
            {
                // Init loop attributes
                similarityMax = 0.0f;
                mostSililarFontModelIndex = -1;
                similarity = 0.0f;

                // Get Bounding box
                hpos = Convert.ToInt32(node.Attributes["HPOS"].Value);
                vpos = Convert.ToInt32(node.Attributes["VPOS"].Value);
                width = Convert.ToInt32(node.Attributes["WIDTH"].Value);
                height = Convert.ToInt32(node.Attributes["HEIGHT"].Value);
                imageName = node.Attributes["ID"].Value;

                // Get cropped String
                filterCrop = new Crop(new Rectangle(hpos, vpos, width, height));
                image = filterCrop.Apply(imageSource);

                // Convert to Grayscale if needed
                if (image.PixelFormat != PixelFormat.Format8bppIndexed) image = filterG.Apply(image);

                // Binarisation
                if (this.oTemplateMatchingParameters.BINARIZE_FOR_COMPARISON)
                {
                    image = filterB.Apply(image);

                    //Denoising
                    if (this.oTemplateMatchingParameters.DENOISE_BOUNDING_BOX)
                    {
                        filterI.ApplyInPlace(image);
                        BoundingBoxNoiseRemoval.RemoveNoise(image);
                    }
                }

                // Compare current "image" with the known cluster representatives "ref_image"
                for (int it = 0; it < this.representatives.Count; it++)
                {
                    // Load and prepare the reference image
                    ref_image = (Bitmap)Bitmap.FromFile(representatives[it]);

                    //Convert to Greyscale
                    if (ref_image.PixelFormat != PixelFormat.Format8bppIndexed) ref_image = filterG.Apply(ref_image);

                    // Binarisation
                    if (this.oTemplateMatchingParameters.BINARIZE_FOR_COMPARISON)
                    {
                        ref_image = filterB.Apply(ref_image);

                        //Denoising
                        if (this.oTemplateMatchingParameters.DENOISE_BOUNDING_BOX)
                        {
                            filterI.ApplyInPlace(ref_image);
                            BoundingBoxNoiseRemoval.RemoveNoise(ref_image);
                        }
                    }

                    // Normalize ref_image regarding image largest dimension and preserve the aspect ratio
                    if (this.oTemplateMatchingParameters.NORMALIZE_BEFORE_COMPARISON)
                    {
                        if (image.Width > image.Height)
                            filterResize = new ResizeNearestNeighbor(image.Width, (int)Math.Round(((float)ref_image.Height / (float)ref_image.Width) * image.Width));
                        else
                            filterResize = new ResizeNearestNeighbor((int)Math.Round(((float)ref_image.Width / (float)ref_image.Height) * image.Height), image.Height);
                        ref_image = filterResize.Apply(ref_image);
                    }

                    //if ((ref_image.Width / image.Width > 0.2) && (ref_image.Width / image.Width < 2))
                    if (Math.Abs(ref_image.Width - image.Width) < 5)
                    {
                        // Compute Similarity value 
                        similarity = ComputeSimilarity(ref image, ref ref_image);

                        //compare similarities
                        if (similarity > similarityMax)
                        {
                            similarityMax = similarity;
                            mostSililarFontModelIndex = it;
                        }
                    }

                    // Free memory
                    ref_image.Dispose();
                }

                // Evaluate the find maximum similarity
                if ((similarityMax < this.oTemplateMatchingParameters.TEMPLATE_MATCHING_THRESHOLD) || (mostSililarFontModelIndex < 0))
                {
                    //Create a New cluster by adding new representative     
                    String path_ = this.tmpPath + this.clusterNb + ".png";
                    this.representatives.Add(path_);
                    image.Save(path_, ImageFormat.Png);

                    // And select this cluster as the most similar 
                    mostSililarFontModelIndex = this.clusterNb;

                    // Increase Cluster count
                    this.clusterNb++;
                }

                // Assign the current image to the chosen cluster (file)                
                tw = new StreamWriter(outputpath_ + mostSililarFontModelIndex.ToString() + ".txt", true);
                tw.WriteLine(imageName + ".png" + " - " + similarityMax.ToString());
                //if (imageName != images[0]) tw.WriteLine(imageName + " - " + similarityMax.ToString());

                tw.Close();
                tw.Dispose();

                // Free memory
                image.Dispose();

                // close the stream
                tw.Close();
                tw.Dispose();
            }

            //Force Garbage Collector to free memory
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }  

    }
}
