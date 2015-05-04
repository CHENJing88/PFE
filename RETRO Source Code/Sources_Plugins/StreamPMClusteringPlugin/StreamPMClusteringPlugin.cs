using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

using AForge.Imaging;
using AForge.Imaging.Filters;


namespace Polytech.Clustering.Plugin
{
    /// <summary>
    /// Pixel Matching stream clustering Plugin 
    /// </summary>        
    public partial class StreamPMClusteringPlugin : Form, IClusteringPlugin
    {
        // Aforge filters
        private Grayscale filterG = new Grayscale(0.2125, 0.7154, 0.0721);
        private SISThreshold filterB = new SISThreshold();
        private Invert filterI = new Invert();
        private ResizeNearestNeighbor filterResize;
 
        /// <summary>
        /// Database of documents containing Patterns to cluster
        /// </summary>
        Database m_db = null;

        /// <summary>
        /// Class that embbed the configuration of theis PM clustering plugin
        /// </summary>
        IConfig m_config = null;

        /// <summary>
        /// Processing time of the clustering
        /// </summary>
        TimeSpan m_elapsedTime;

        /// <summary>
        /// Path to clusters files
        /// </summary>
        string m_clustersDir;

        /// <summary>
        /// Path to clusters Files
        /// </summary>
        public void SetClustersDir(string path)
        {
            m_clustersDir = path;
        }
        
        /// <summary>
        /// Database of documents containing Patterns to cluster
        /// </summary>
        public void SetDatabase(Database db)
        {
            m_db = db;
        }

        //To select the id of the new clusters
        private int lastId = 0;

        /// <summary>
        /// Constructor for the Main Class of the Plugin (the Form and Iconfig object)
        /// </summary>
        public StreamPMClusteringPlugin()
        {
            InitializeComponent();

            //Get configuration of plugin from GUI default values
            double thresholdValue = double.Parse(textBoxThreshold.Text, System.Globalization.CultureInfo.InvariantCulture);
            m_config = new StreamPMConfig(thresholdValue, checkBoxNoiseRemoval.Checked, int.Parse(textBoxSize.Text));
            
            //Set default dir for clusters files - Need to be updated !!!
            m_clustersDir = System.Windows.Forms.Application.StartupPath + "\\CLUSTERS";
        }

        
        /// <summary>
        /// Perform clustering on the patterns in the dataset using....
        /// </summary>
        /// <param name="refPatterns">Representative Patterns of the desired clusters (null if none)</param>
        /// <param name="updateActualClusters">Should be set to True if you want to update the actual clusters</param>
        /// <param name="indexSignature">Should be set to -2 for Pixel Matching clustering</param>
        /// <returns></returns>
        public List<Cluster> PerformClustering(List<APattern> refPatterns = null, bool updateActualClusters = false, int indexSignature = -1)
        {
            DateTime start = DateTime.Now;
            Cluster clusMin = null;
            //int indexMin;
            List<Cluster> listClusters = new List<Cluster>();

            // Check directory
            if (!Directory.Exists(m_clustersDir)) Directory.CreateDirectory(m_clustersDir);

            //Initialisation of the resulting clusters with the previous clusters if requested            
            if (updateActualClusters)
            {
                String[] Files = Directory.GetFiles(m_clustersDir);

                //Instanciation of a shared list (thread safe)
                BlockingCollection<Cluster> oldclusters = new BlockingCollection<Cluster>();
                Task loadClTask = Task.Factory.StartNew(() =>
                {
                    Parallel.ForEach(Files, st =>
                    {
                        //Load the existing cluster
                        Cluster newCluster = new Cluster(st);

                        // Set the representative Pattern
                        APattern pattern = newCluster.Patterns[0];
                        pattern.ImageRepresentation = preprocessing(pattern);
                        newCluster.AddRepresentative(pattern);

                        // Free memory by removing the old patterns
                        newCluster.ClearPatternsFromMemory();
                        oldclusters.Add(newCluster);
                    });
                    //complétion of the updating of the list
                    oldclusters.CompleteAdding();
                });

                loadClTask.Wait();
                //Copy from shared list to normal list
                foreach (Cluster cl in oldclusters)
                {
                    listClusters.Add(cl);
                }
                oldclusters.Dispose();
            }
            else
            {
                String[] Files = Directory.GetFiles(m_clustersDir);
                foreach (String st in Files)
                {
                    //Delete the existing cluster files
                    File.Delete(st);
                }
            }

            //Initialisation of the new clusters with the reference patterns if provided
            if (refPatterns != null)
            {
                foreach (APattern pattern in refPatterns)
                {
                    //instanciation of a new cluster
                    int newid = GetNewClusterId();
                    Cluster newCluster = new Cluster(newid.ToString(), m_clustersDir);

                    //Add representative
                    pattern.ImageRepresentation = preprocessing(pattern);
                    newCluster.AddRepresentative(pattern);
                    listClusters.Add(newCluster);
                }
            }

            //End of initialisation

            //Clustering of the patterns 
            foreach (Document docu in m_db.Documents)
            {
                foreach (APattern pattern in docu.Patterns)
                {
                    // Get the bitmap of the patern to analyze and compare it to the existing clusters
                    Bitmap bitmap_pattern = preprocessing(pattern);
                    //indexMin = GetClosest(bitmap_pattern, listClusters);
                    clusMin = GetClosest(bitmap_pattern, listClusters);

                    //Add the current pattern into a cluster
                    if (clusMin != null)
                    {
                        //The pattern is similar to a existing cluster according to the similarity Threshold
                        //listClusters[indexMin].AddPattern(pattern);
                        clusMin.AddPattern(pattern);
                    }
                    else
                    {
                        //The pattern is not similar enough to the existing cluster => Creation of a new cluster
                        int newid = GetNewClusterId();
                        Cluster newCluster = new Cluster(newid.ToString(), m_clustersDir);

                        // Creation of the representative
                        pattern.ImageRepresentation = bitmap_pattern;
                        newCluster.AddRepresentative(pattern);

                        // Add this first pattern into the new cluster
                        newCluster.AddPattern(pattern);
                        listClusters.Add(newCluster);
                    }

                    //Free memory and update info windows every 500 patterns ?
                }
            }

            //Save or update new clusters into files ?

            m_elapsedTime = start - DateTime.Now;

            return listClusters;
        }
        
        /// <summary>
        /// Perform clustering on the patterns into the provided cluster using ...
        /// </summary>
        /// <param name="clusterToProcess">Cluster containing the data (patterns) to cluster</param>
        /// <param name="refPatterns">Representative Patterns of the desired clusters (null if none)</param>
        /// <param name="updateActualClusters">Should be set to True if you want to update the actual clusters</param>
        /// <param name="indexSignature">Should be set to -2 for Pixel Matching clustering</param>
        /// <returns></returns>
        public List<Cluster> PerformClustering(Cluster clusterToProcess, List<APattern> refPatterns = null, bool updateActualClusters = false, int indexSignature = -1)
        {
            DateTime start = DateTime.Now;
            Cluster clusMin = null;             
            //int indexMin;
            List<Cluster> listClusters = new List<Cluster>();
            
            // Check directory
            if (!Directory.Exists(m_clustersDir)) Directory.CreateDirectory(m_clustersDir);
            
            //Initialisation of the resulting clusters with the previous clusters if requested            
            if (updateActualClusters)
            {
                String[] Files = Directory.GetFiles(m_clustersDir);

                //Instanciation of a shared list (thread safe)
                BlockingCollection<Cluster> oldclusters = new BlockingCollection<Cluster>();
                Task loadClTask = Task.Factory.StartNew(() =>
                {
                    Parallel.ForEach(Files, st =>
                    {
                        //Load the existing cluster
                        Cluster newCluster = new Cluster(st);

                        // Set the representative Pattern
                        APattern pattern = newCluster.Patterns[0];
                        pattern.ImageRepresentation = preprocessing(pattern);
                        newCluster.AddRepresentative(pattern);

                        // Free memory by removing the old patterns
                        newCluster.ClearPatternsFromMemory();
                        oldclusters.Add(newCluster);
                    });
                    //complétion of the updating of the list
                    oldclusters.CompleteAdding();
                });

                loadClTask.Wait();
                //Copy from shared list to normal list
                foreach (Cluster cl in oldclusters)
                {
                    listClusters.Add(cl);
                }
                oldclusters.Dispose();
            }
            else
            {
                    String[] Files = Directory.GetFiles(m_clustersDir);
                    foreach (String st in Files)
                    {
                        //Delete the existing cluster files
                        File.Delete(st);
                    }
            }
            
            //Initialisation of the new clusters with the reference patterns if provided
            if (refPatterns != null)
            {
                foreach (APattern pattern in refPatterns)
                {
                    //instanciation of a new cluster
                    int newid = GetNewClusterId();
                    Cluster newCluster = new Cluster(newid.ToString(), m_clustersDir);

                    //Add representative
                    pattern.ImageRepresentation = preprocessing(pattern);
                    newCluster.AddRepresentative(pattern); 
                    listClusters.Add(newCluster);                                     
                }
            }

            //End of initialisation

            //Clustering of the patterns            
            foreach(APattern pattern in clusterToProcess.Patterns)
            {
                // Get the bitmap of the patern to analyze and compare it to the existing clusters
                Bitmap bitmap_pattern = preprocessing(pattern);
                clusMin = GetClosest(bitmap_pattern,listClusters);
                
                //Add the current pattern into a cluster
                if (clusMin != null)
                {
                    //The pattern is similar to a existing cluster according to the similarity Threshold
                    //listClusters[indexMin].AddPattern(pattern);
                    clusMin.AddPattern(pattern);
                }
                else
                {
                    //The pattern is not similar enough to the existing cluster => Creation of a new cluster
                    int newid = GetNewClusterId();
                    Cluster newCluster = new Cluster(newid.ToString(), m_clustersDir);

                    // Creation of the representative
                    pattern.ImageRepresentation = bitmap_pattern;
                    newCluster.AddRepresentative(pattern);

                    // Add this first pattern into the new cluster
                    newCluster.AddPattern(pattern);
                    listClusters.Add(newCluster);
                }

                //Free memory and update info windows every 500 patterns ?
            }

            //Save or update new clusters into files ?

            m_elapsedTime = start - DateTime.Now;

            return listClusters;
        }


        /// <summary>
        /// Return the index of the closest cluster according to pixel matching between the bitmaps (return -1 if dissimilarity > Threshold)
        /// </summary>
        /// <param name="image">Butmap of the pattern to use for pixel matching</param>
        /// <param name="lstClusters">List of clusters to compare with</param>
        /// <returns>Most similar cluster or null if no similar enough cluster has been found</returns>
        private Cluster GetClosest(Bitmap image, List<Cluster> lstClusters)
        {
            Object thisLock = new Object(); //To lock the access to Min in Parallel version
            
            double tmin, tmax;
            double distanceMin = double.MaxValue;
            double distanceTemp = 0.0;
            Cluster clusMin = null;
            Bitmap ref_image;

            //Version seq:for (int i = 0; i < lstClusters.Count; i++)
            Parallel.ForEach(lstClusters, clus =>
            {
                //Version seq: ref_image = ((lstClusters[i]).Representatives[0]).ImageRepresentation;
                ref_image = (clus.Representatives[0]).ImageRepresentation;

                //Compute similarity between images only if necessary (Threshold = 1.2)
                tmax = (double)Math.Max(ref_image.Width * ref_image.Height, image.Width * image.Height);
                tmin = (double)Math.Min(ref_image.Width * ref_image.Height, image.Width * image.Height);
                if (tmax < 1.2 * tmin)
                {
                    // Compute Similarity value 
                    distanceTemp = ComputeSimilarity(ref image, ref ref_image);

                    //Lock access in parallel version
                    lock (thisLock)
                    {
                        //compare similarities
                        if (distanceTemp < distanceMin)
                        {
                            distanceMin = distanceTemp;
                            clusMin = clus;
                        }
                    }
                }
            });

            if ( ((StreamPMConfig)m_config).Threshold > distanceMin ) 
                return clusMin;
            else
                return null;
        }

        /// <summary>
        /// Preprocessing of the Bitmap of the Pattern according to the parameters of the plugin
        /// </summary>
        /// <param name="pattern">pattern to preprocess before comparison</param>
        /// <returns>Return the Bitmap of the Pattern to use for the Pixel Matching (according to the parameters)</returns>
        private Bitmap preprocessing(APattern pattern)
        {
            // Get the image of the pattern and free memory
            ((ShapeEoC)pattern).LoadEoCImage();
            Bitmap image = pattern.ImageRepresentation;
            pattern.Dispose();
            
            // Size Normalization
            if( ((StreamPMConfig)m_config).NormalisationSize != -1)
            {
                //preserve aspect ratio
                filterResize = new ResizeNearestNeighbor((int)Math.Round(((float)image.Width / (float)image.Height) * ((StreamPMConfig)m_config).NormalisationSize), ((StreamPMConfig)m_config).NormalisationSize);
                image = filterResize.Apply(image);
            }

            //Convert to Greyscale
            if (image.PixelFormat != PixelFormat.Format8bppIndexed) image = filterG.Apply(image);

            // Binarisation
            image = filterB.Apply(image);

            //Denoising
            if ( ((StreamPMConfig)m_config).NoiseRemoval )
            {
                    filterI.ApplyInPlace(image);
                    BoundingBoxNoiseRemoval.RemoveNoise(image);
            }
            
            return image;
        }


        /// <summary>
        /// Compute dissimilarity between two binary shapes (distance).
        /// May not have the same dimensions
        /// </summary>
        /// <param name="image1">Shape 1</param>
        /// <param name="image2">Shape 2</param>
        /// <returns>Dissimilary between the two shapes in [0,1]</returns>
        private float ComputeSimilarity(ref Bitmap image1, ref Bitmap image2)
        {
            int m_width = Math.Min(image1.Width, image2.Width);
            int m_height = Math.Min(image1.Height, image2.Height);
            int x, y, x1, y1, x2, y2;
            byte pimg_xy, pref_xy;
            long diff = 0;

            // lock images
            BitmapData imageData1 = image1.LockBits(new Rectangle(0, 0, image1.Width, image1.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            BitmapData imageData2 = image2.LockBits(new Rectangle(0, 0, image2.Width, image2.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
            UnmanagedImage u_image = new UnmanagedImage(imageData1);
            UnmanagedImage u_ref = new UnmanagedImage(imageData2);

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

                    for (x = 0; x < (m_width - 1); x++)
                    {
                        for (y = 0; y < (m_height - 1); y++)
                        {
                            x1 = offset_x1 + x;
                            y1 = offset_y1 + y;
                            x2 = offset_x2 + x;
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
                image1.UnlockBits(imageData1);
                image2.UnlockBits(imageData2);
            }

            return (float)diff / (float)(m_width * m_height * 255);
        }


        /// <summary>
        /// Name of the plugin
        /// </summary>
        /// <returns>Name of the clustering method</returns>
        public string GetName()
        {
            return "PixelMatchingClustering";
        }

        /// <summary>
        /// Name of the author of the plugin
        /// </summary>
        /// <returns>Name of the author of the clustering method</returns>
        public string GetAuthor()
        {
            return "JY Ramel - F. Rayar";
        }

        /// <summary>
        /// Configuration of the clustering method
        /// </summary>
        /// <returns>Form that allows the clustering method</returns>        
        public Form GetConfigWindow()
        {
            return this;
        }

        /// <summary>
        /// Ifo about the method and parameters
        /// </summary>
        /// <returns>list of string providing info</returns>
        public List<string> GetInfoList()
        {
            List<string> infoList = new List<string>();

            infoList.Add("Method : " + GetName());
            infoList.Add("Author : " + GetAuthor());

            infoList.Add("Configuration");
            infoList.Add("Threshold : " + (((StreamPMConfig) m_config).Threshold).ToString() );
            infoList.Add("NoiseRemoval : " + (((StreamPMConfig)m_config).NoiseRemoval).ToString());
            infoList.Add("Seuil : " + (((StreamPMConfig)m_config).NormalisationSize).ToString());
            
            return infoList;
        }

        /// <summary>
        /// Object embedding the config
        /// </summary>
        /// <returns>config object</returns>        
        public IConfig GetConfig()
        {
            return (StreamPMConfig)m_config;
        }

        /// <summary>
        /// Duration of the clustering
        /// </summary>
        /// <returns>duration of the clustering</returns>
        public TimeSpan GetProcessingTime()
        {
            return m_elapsedTime;
        }


        // Widget Events Management
 
        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void buttonValidate_Click(object sender, EventArgs e)
        {
            //Modification of the configuration
            ((StreamPMConfig)m_config).NoiseRemoval = checkBoxNoiseRemoval.Checked;
            ((StreamPMConfig)m_config).NormalisationSize = int.Parse(textBoxSize.Text);
            ((StreamPMConfig)m_config).Threshold = int.Parse(textBoxThreshold.Text);

            this.Hide();
        }

        private void textBoxThreshold_TextChanged(object sender, EventArgs e)
        {
            //Nothing to do
        }

        private void textBoxSize_TextChanged(object sender, EventArgs e)
        {
            //Nothing to do
        }

        private void checkBoxNoiseRemoval_CheckedChanged(object sender, EventArgs e)
        {
            //Nothing to do
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            //Load Config from file
            string filename = System.Windows.Forms.Application.StartupPath + "\\StreamPMClusteinrconfig.bin"; 
            
            IFormatter formatter = new BinaryFormatter();            
            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            m_config = (StreamPMConfig)formatter.Deserialize(stream);

            stream.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            //Save config to file
            string filename = System.Windows.Forms.Application.StartupPath + "\\StreamPMClusteinrconfig.bin";

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this.m_config);
            stream.Close();

            MessageBox.Show("Config saved in " + filename );
        }


        // Find a free ID for a new cluster (file) in the Clusters Dir
        private int GetNewClusterId()
        {
            String filename;
            bool ok = false;
            int id = lastId;

            while (!ok)
            {
                id++;
                filename = m_clustersDir + @"\" + "cluster" + id.ToString() + ".xml";  
                if (!File.Exists(filename)) ok = true;                    
            }

            lastId = id;
            return id;
        }

    }
}
