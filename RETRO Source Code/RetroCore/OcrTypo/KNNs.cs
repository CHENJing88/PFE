using Accord.MachineLearning;
using Polytech.Clustering.Plugin;
using Retro.ocr;
using RetroUtil;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Retro.OcrTypo
{
    /// <summary>
    /// KNearestNeighbors, one of machine learning algo, for auto transcription cluster
    /// </summary>
    public class KNNs:IOCR
    {
        //parametre KNNs
        private int K=1;

        /// <summary>
        /// Constructor
        /// </summary>
        public KNNs() {}

        
        /// <summary>
        /// start the processus of KNNs OCR
        /// </summary>
        /// <param name="clusterList">List of non labeled clusters</param>
        /// <param name="directory">List og labeled Font Model</param>
        /// <param name="SelectDescripMethod">a description of signature</param>
        /// <param name="dynamicSplashScreenNotification">Dynamic Splashscreen Notification</param>
        /// <returns>Number of cluster automatically transcripted</returns>
        public int RunKNNsOCR(List<Cluster> clusterList, string directory, string SelectDescripMethod, DynamicSplashScreenNotification dynamicSplashScreenNotification)
        {
            int nbTranscribedClusters = 0;
            DateTime dtPrepB = DateTime.Now; 
            //%%%%%%%%%%%%%%%%%%%%%%% Process of getting the inputs and output of the network with the directory of FontModel %%%%%%%%%%%%%%%%%%%%%%%
            // Get list of Font Models and init TrainSet(inputs) & Classes(nb of total class):
            // extraire the training FontModel's feature and classe attached

            // Get the models (images + xml)
            String[] files = Directory.GetFiles(directory, "*.xml");
            //extraire the model's feature as training set
            double[][] inputs = new double[files.Length][];
            //extraire the model's classe
            int[] outputs = new int[files.Length];
            List<String> caraList = new List<String>();
            int Classes = 0;
            Dictionary<int, String> caraClass = new Dictionary<int, string>();
            // Process each xml file
            for (int i = 0; i < files.Length; i++)
            {
                String filename = Path.GetFileNameWithoutExtension(files[i]);
                if ((File.Exists(directory + @"\" + filename + ".png")) && (File.Exists(directory + @"\" + filename + "_bw.png")))
                {
                    // Create a new FontModel from the xml path
                    FontModel fontmodel = new FontModel(files[i]);

                    //filled the output and compte the class
                    
                    if (!caraList.Contains(fontmodel.TranscriptionCharacter))
                    {
                        caraList.Add(fontmodel.TranscriptionCharacter);
                        caraClass.Add(Classes, fontmodel.TranscriptionCharacter);
                        Classes++;
                    }
                    
                    outputs[i] = Classes - 1;
                    
                    //get the signatures of image and filled the array of inputs
                    ShapeEoC currentshape;                                 // quite strange to be obliged to use ShapeEoC instead of APattern
                    currentshape = new ShapeEoC(i.ToString(), fontmodel.Directory);       // quite strange to be obliged to use ShapeEoC instead of APattern
                    currentshape.PathToFullImage = directory + @"\" + filename + "_bw.png";
                    currentshape.LoadEoCImage();
                    LoadPatternSignature(currentshape);
                    //set the input with the signature of pattern
                    inputs[i] = currentshape.GetSignature(SelectDescripMethod).GetNormalisedFeatures().ToArray();
                    //release the memory of pattern
                    currentshape.Dispose();
                }
            }
           //create the KNNs structure
            KNearestNeighbors knn = new KNearestNeighbors(k: K, classes: Classes, inputs: inputs, outputs: outputs);
            DateTime dtPrepE = DateTime.Now;
            Console.WriteLine("Time of preparation：{0}ms", (dtPrepE - dtPrepB).TotalMilliseconds); 
            //%%%%%%%%%%%%%%%%%%%%%%% Process of recognize for each cluster %%%%%%%%%%%%%%%%%%%%%%%
            DateTime dtTestB = DateTime.Now;
            foreach (Cluster cluster in clusterList)
            {
                // Notify the ViewModel
                dynamicSplashScreenNotification.Message = "Processing cluster " + cluster.Id + "/" + clusterList.Count;

                // Init loop attributes
                cluster.LoadPatternsFromFile(true);
                ShapeEoC shape = null;
                shape = (ShapeEoC)((cluster.Representatives[0]).Clone());
                shape.LoadEoCImage();
                cluster.ClearPatternsFromMemory();
                
                // After the algorithm has been created, we can classify a new instance:
                int answer = knn.Compute(shape.GetSignature(SelectDescripMethod).GetNormalisedFeatures().ToArray()); // answer will be 2.

                // Assign the matched FontModel label to the cluster
                if ((answer > 0))
                {
                    cluster.AddNewLabel("KNNs", caraClass[answer], 0.5);
                    cluster.IsLabelized = true;
                    nbTranscribedClusters++;
                }
            }
            DateTime dtTestE = DateTime.Now;
            Console.WriteLine("Time of testing：{0}ms", (dtTestE - dtTestB).TotalMilliseconds);
            Console.WriteLine("Time total：{0}ms", (dtTestE - dtPrepB).TotalMilliseconds);
            System.Windows.MessageBox.Show("Time of preparation：" + (dtPrepE - dtPrepB).TotalMilliseconds + "ms.\nTime of testing：" + (dtTestE - dtTestB).TotalMilliseconds + "ms.\nTime total："+(dtTestE - dtPrepB).TotalMilliseconds+"ms.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            return nbTranscribedClusters;
        }
        
    }
}
