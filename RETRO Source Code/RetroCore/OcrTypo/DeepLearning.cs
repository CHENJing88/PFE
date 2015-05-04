using Accord.Neuro;
using Accord.Neuro.ActivationFunctions;
using Accord.Neuro.Learning;
using Accord.Neuro.Networks;
using Accord.Statistics.Analysis;
using Accord.Math;
using AForge.Imaging.Filters;
using AForge.Neuro;
using AForge.Neuro.Learning;
using Polytech.Clustering.Plugin;
using Retro.ocr;
using RetroUtil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Retro.OcrTypo
{
    public class DeepLearning : IOCR, INotifyPropertyChanged
    {
        #region Attributes

        private string FontModelFolderPath;
        private string SelectDescriptor;

        public DeepBeliefNetwork Network { get; private set; }

        // Network parameters
        public int NewLayerNeurons { get; set; }
        private int Classes;
        private int inputLength;
        private int nbLayer;
        public bool CanClassify { get { return Network != null && Network.OutputCount ==Classes; } }
        public bool CanGenerate { get { return Network != null && Network.Layers.Length > 0; } }

        /// <summary>Indicates whether a background thread is busy loading data.</summary>
        public bool IsDataLoading { get; set; }

        /// <summary>Indicates whether training data has already finished loading.</summary>
        public bool HasDataLoaded { get; private set; }

        /// <summary>Indicates the learning procedure is starting.</summary>
        public bool IsStarting { get; set; }

        /// <summary>Indicates the learning procedure is currently being run.</summary>
        public bool IsLearning { get; private set; }

        public bool HasLearned { get; private set; }

        public string CurrentTask
        {
            get
            {
                if (ShouldLearnEntireNetwork)
                    return "Fine-tuning entire network";
                return "Learn network layer " + SelectedLayerIndex + " of " + Network.Layers.Length;
            }
        }

        // Training parameters
        public int SelectedLayerIndex { get; set; }// for the use of training each layer 
        public bool ShouldLayerBeSupervised { get; set; }
        public bool ShouldLearnEntireNetwork { get; set; }

        public double LearningRate { get; set; }
        public double Momentum { get; set; }
        public double WeightDecay { get; set; }
        public int Epochs { get; set; }
        public int BatchSize { get; set; }

        // Training session information
        public int CurrentEpoch { get; private set; }
        public double CurrentError { get; private set; }
        
        // Training controls
        public bool CanStart
        {
            get
            {
                return (HasDataLoaded && !IsLearning) &&
                    ((!ShouldLearnEntireNetwork && !ShouldLayerBeSupervised && CanGenerate) ||
                    ((ShouldLearnEntireNetwork || ShouldLayerBeSupervised) && CanClassify));
            }
        }
        
        public bool CanLearnUnsupervised { get { return Network != null; } }
        public bool CanLayerBeSupervised
        {
            get
            {
                return CanLearnUnsupervised && !ShouldLearnEntireNetwork &&
                    Network.Layers[SelectedLayerIndex - 1].Neurons.Length == Classes;
            }
        }

        public bool CanNetworkBeSupervised
        {
            get { return CanClassify; }
        }
        /// <summary>
        ///   Gets the collection of training instances.
        /// </summary>
        private List<FontModel> Training;
        private List<TrainSample> TrainSet;

        /// <summary>
        ///   Gets the colleection of testing instances.
        /// </summary>
        private List<Cluster> Testing { set; get; }
        
        #endregion

        #region Constructor & Starters
        /// <summary>
        /// Default Constructor
        /// </summary>
        public DeepLearning()
        { }
        /// <summary>
        /// start the processus of deep learning OCR with encoding the image(input is the signature of image)
        /// </summary>
        /// <param name="clustersList"></param>
        /// <param name="directory"></param>
        /// <param name="SelectDescripMethod"></param>
        /// <param name="dynamicSplashScreenNotification"></param>
        /// <returns></returns>
        internal int RunDeepLearningOCR(List<Cluster> clustersList, string directory, string SelectDescripMethod, DynamicSplashScreenNotification dynamicSplashScreenNotification)
        {
            // Complete member initialization
            this.FontModelFolderPath = directory;
            this.SelectDescriptor = SelectDescripMethod;
            //set the testing set
            this.Testing = clustersList;
            //set the training set
            Training = new List<FontModel>();
            TrainSet = new List<TrainSample>();

            int nbTranscribedClusters = 0;
            DateTime dtPrepB = DateTime.Now; 
            //=========== get the TrainSet of network(data of learning) =============
            //the structure TrainSet extraire the model's feature as training set and the model's classe
            String[] files = Directory.GetFiles(directory, "*.xml");
            List<String> caraList = new List<String>();
            Dictionary<int, String> caraClass = new Dictionary<int, string>();
            Classes = 0;

            // Process each xml file
            for (int i = 0; i < files.Length; i++)
            {
                
                String filename = Path.GetFileNameWithoutExtension(files[i]);
                if ((File.Exists(directory + @"\" + filename + ".png")) && (File.Exists(directory + @"\" + filename + "_bw.png")))
                {
                    // Create a new FontModel from the xml path
                    FontModel fontmodel = new FontModel(files[i]);
                    // Add the newly created FontModel in the list
                    Training.Add(fontmodel);
                    //filled the output and compte the class
                    if (i == 0)
                    {
                        caraList.Add(fontmodel.TranscriptionCharacter);
                        caraClass.Add(Classes, fontmodel.TranscriptionCharacter);
                        Classes++;
                    }
                    else
                    {
                        if (!caraList.Contains(fontmodel.TranscriptionCharacter))
                        {
                            caraList.Add(fontmodel.TranscriptionCharacter);
                            caraClass.Add(Classes, fontmodel.TranscriptionCharacter);
                            Classes++;
                        }
                    }
                    
                    //get the signatures of image and filled the array of inputs
                    ShapeEoC currentshape;                                 // quite strange to be obliged to use ShapeEoC instead of APattern
                    currentshape = new ShapeEoC(i.ToString(), fontmodel.Directory);       // quite strange to be obliged to use ShapeEoC instead of APattern
                    currentshape.PathToFullImage = directory + @"\" + filename + "_bw.png";
                    currentshape.LoadEoCImage();
                    LoadPatternSignature(currentshape);
                    //set the features data of model
                    double[] featrues = currentshape.GetSignature(SelectDescripMethod).GetNormalisedFeatures().ToArray();
                    currentshape.Dispose();
                    //set the features,label,class of model
                    TrainSet.Add(new TrainSample(featrues, fontmodel.TranscriptionCharacter, Classes-1, -1));
                }
            }
            //set the input size of network
            inputLength = TrainSet[0].Features.Length;

            //create and initialize the parameters of deep learning network
            initialization();
            DateTime dtPrepE = DateTime.Now;
            Console.WriteLine("Time of preparation：{0}ms", (dtPrepE - dtPrepB).TotalMilliseconds); 

            //=========== Process of training the network with the inputs ===========
            DateTime dtTrainB = DateTime.Now;
            // pre-training the network for init weights of network
            PreTraining();
            //fine-tune the entire network
            learnNetworkSupervised();

            DateTime dtTrainE = DateTime.Now;
            Console.WriteLine("Time of training：{0}ms", (dtTrainE - dtTrainB).TotalMilliseconds); 
            //=========== Process of recognize for each cluster ===========
            DateTime dtTestB = DateTime.Now;
            foreach (Cluster cluster in Testing)
            {
                // Notify the ViewModel
                dynamicSplashScreenNotification.Message = "Processing cluster " + cluster.Id + "/" + Testing.Count;

                // Init loop attributes
                int mostSililarFontModelIndex = -1;

                cluster.LoadPatternsFromFile(true);
                ShapeEoC shape = null;
                shape = (ShapeEoC)((cluster.Representatives[0]).Clone());
                shape.LoadEoCImage();
                cluster.ClearPatternsFromMemory();

                if (shape.ImageRepresentation != null)
                {
                    
                    // regonize the known cluster representatives
                    mostSililarFontModelIndex = Recognize(shape);

                    // Free representative image
                    shape.Dispose();

                    // Assign the matched FontModel label to the cluster
                    if ((mostSililarFontModelIndex > 0))
                    {
                        cluster.AddNewLabel("DEEP_LEARNING", caraClass[mostSililarFontModelIndex], 0.1);
                        cluster.IsLabelized = true;
                        nbTranscribedClusters++;
                    }
                }
            }
            DateTime dtTestE = DateTime.Now;
            Console.WriteLine("Time of testing：{0}ms", (dtTestE - dtTestB).TotalMilliseconds);
            Console.WriteLine("Time total：{0}ms", (dtTestE - dtPrepB).TotalMilliseconds);
            System.Windows.MessageBox.Show("Time of preparation：" + (dtPrepE - dtPrepB).TotalMilliseconds + "ms.\nTime of testing：" + (dtTestE - dtTestB).TotalMilliseconds + "ms.\nTime total：" + (dtTestE - dtPrepB).TotalMilliseconds + "ms.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            return nbTranscribedClusters;

        }

       
        /// <summary>
        /// initialize the network, the network parameters
        /// </summary>
        public void initialization()
        {
            var function = new BernoulliFunction(alpha: 0.5);

            // Create a Restricted Boltzmann Machine with different inputs and 1 hidden neuron
            var rbm1 = new RestrictedBoltzmannMachine(function, inputsCount: inputLength, hiddenNeurons: 500);//input layer
            var rbm2 = new RestrictedBoltzmannMachine(function, inputsCount: 500, hiddenNeurons: 500);
            var rbm3 = new RestrictedBoltzmannMachine(function, inputsCount: 500, hiddenNeurons: 2000);
            var rbm4 = new RestrictedBoltzmannMachine(function, inputsCount: 2000, hiddenNeurons: Classes);//output layer

            RestrictedBoltzmannMachine[] layers = { rbm1, rbm2, rbm3, rbm4 };

            // Create settings for dataset
            Network = new DeepBeliefNetwork(inputLength, layers);

            //Network = new DeepBeliefNetwork(new BernoulliFunction(), 1024, 50, Classes);
            NewLayerNeurons = Classes;
            nbLayer = 4;
            //StackNewLayer();

            new GaussianWeights(Network).Randomize();
            Network.UpdateVisibleWeights();

            
            SelectedLayerIndex = 1;

            LearningRate = 0.1;
            WeightDecay = 0.001;
            Momentum = 0.9;
            Epochs = 50;
            BatchSize = 100;

            IsLearning = true;
            IsStarting = true;

            CurrentEpoch = 0;
            CurrentError = 0;

            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                HasLearned = true;

            this.PropertyChanged += new PropertyChangedEventHandler(DeepLearn_PropertyChanged);
        }

        /// <summary>
        /// start the processus of deep learning OCR without encoding the image(input is the value of pixel)
        /// </summary>
        public int RunDeepLearningOCR(DynamicSplashScreenNotification dynamicSplashScreenNotification)
        {
            //if (!CanStart) return;

            //initialize the parameters of deep learning network
            initialization();

            List<String> tmpFontModelImagesPath = new List<String>();
            int nbTranscribedClusters = 0;

            // Build temp directory
            String tmpPath = @"\tmpModelsImages_DeepLearning";
            if (Directory.Exists(tmpPath)) { Directory.Delete(tmpPath, true); System.Threading.Thread.Sleep(500); }
            Directory.CreateDirectory(tmpPath);

            // Create tmp font models images
            foreach (FontModel model in Training)
            {
                // Open model
                //String modelPath = model.Directory + @"\" + model.ThumbnailName;
                String modelPath = model.Directory + @"\" + model.NormalizedName + ".png";
                Bitmap modelBitmap = (Bitmap)Bitmap.FromFile(modelPath);

                // Preprocess image
                PreprocessImage(ref modelBitmap);
                // Save image in tmp directory
                //modelBitmap.Save(tmpPath + @"\" + model.ThumbnailName, ImageFormat.Png);
                modelBitmap.Save(tmpPath + @"\" + model.NormalizedName + "."+ImageFormat.Png, ImageFormat.Png);
                modelBitmap.Dispose();

                // Add in the list of tmp model image
                //tmpFontModelImagesPath.Add(tmpPath + @"\" + model.ThumbnailName);
                tmpFontModelImagesPath.Add(tmpPath + @"\" + model.NormalizedName + ".png");
            }
            
            //LoadTrainSet(tmpFontModelImagesPath);

            //%%%%%%%%%%%%%%%%%%%%%%% Process of training the network with the FontModel %%%%%%%%%%%%%%%%%%%%%%%
            // pre-training the network for init weights of network
            PreTraining();
            //fine-tune the entire network
            learnNetworkSupervised();

            //%%%%%%%%%%%%%%%%%%%%%%% Process of recognize for each cluster %%%%%%%%%%%%%%%%%%%%%%%
            foreach (Cluster cluster in Testing)
            {
                // Notify the ViewModel
                dynamicSplashScreenNotification.Message = "Processing cluster " + cluster.Id + "/" + Testing.Count;

                // Init loop attributes
                int mostSililarFontModelIndex = -1;

                cluster.LoadPatternsFromFile(true);
                ShapeEoC shape = null;
                shape = (ShapeEoC)((cluster.Representatives[0]).Clone());
                shape.LoadEoCImage();
                cluster.ClearPatternsFromMemory();
                
                if (shape.ImageRepresentation != null)
                {
                    Bitmap representativeBitmap = shape.ImageRepresentation;
                    this.PreprocessImage(ref representativeBitmap);

                    // regonize the known cluster representatives
                    mostSililarFontModelIndex = Recognize(representativeBitmap);
                    
                    // Free representative image
                    representativeBitmap.Dispose();

                    // Assign the matched FontModel label to the cluster
                    if ((mostSililarFontModelIndex > 0))
                    {
                        cluster.AddNewLabel("DEEP_LEARNING", Training[mostSililarFontModelIndex].TranscriptionCharacter, -1);
                        cluster.IsLabelized = true;
                        nbTranscribedClusters++;
                    }
                }
            }

            // Remove tmps directory
            Directory.Delete(tmpPath, true);

            return nbTranscribedClusters;
        }

        private void DeepLearn_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Network")
            {
                PropertyChanged(this, new PropertyChangedEventArgs("CurrentTask"));
                PropertyChanged(this, new PropertyChangedEventArgs("CanNetworkBeSupervised"));
            }
            if (e.PropertyName == "CanLayerBeSupervised")
                if (!CanLayerBeSupervised) ShouldLayerBeSupervised = ShouldLearnEntireNetwork;
        }

        /// <summary>
        /// add a new layer to the network
        /// </summary>
        public void StackNewLayer()
        {
            if (Network.Layers.Length == 0)
            {
                Network.Push(NewLayerNeurons,
                    visibleFunction: new GaussianFunction(),
                    hiddenFunction: new BernoulliFunction());
            }
            else Network.Push(NewLayerNeurons, new BernoulliFunction());

            PropertyChanged(this, new PropertyChangedEventArgs("Network"));
        }

        /// <summary>
        /// remove the last layer of the network
        /// </summary>
        public void RemoveLastLayer()
        {
            Network.Pop();
            PropertyChanged(this, new PropertyChangedEventArgs("Network"));
        }
        #endregion

        #region Training
        /// <summary>
        /// initialization the weight by RBM(Restricted Boltzmann Machines)
        /// train the network layer by layer
        /// </summary>
        private void PreTraining()
        {
            //training the hidden layer in order
            while(SelectedLayerIndex < nbLayer)
            {
                learnLayerUnsupervised(); 
                SelectedLayerIndex++;
            }
            //training the last layer 
            learnLayerSupervised(); 
        }

        /// <summary>
        /// fine-tune the entire network by backprogration
        /// </summary>
        private void learnNetworkSupervised()
        {
            if (!CanGenerate) return;
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            double error;
            /*new Task(() =>
            {*/
                var teacher = new BackPropagationLearning(Network)
                {
                    LearningRate = LearningRate,
                    Momentum = Momentum
                };

                double[][] inputs, outputs;
                GetInstances(out inputs, out outputs);
                
                // Start running the learning procedure
                for (int i = 0; i < Epochs; i++)
                {
                    error = teacher.RunEpoch(inputs, outputs);

                    dispatcher.BeginInvoke((Action<int, double>)updateError,
                        DispatcherPriority.ContextIdle, i + 1, error);
                }

                Network.UpdateVisibleWeights();
                IsLearning = false;

            //}).Start();
        }
        /// <summary>
        /// training the layer hidden 
        /// </summary>
        private void learnLayerUnsupervised()
        {
            if (!CanGenerate) return;
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            double error;
            /*new Task(() =>
            {*/
                DeepBeliefNetworkLearning teacher = new DeepBeliefNetworkLearning(Network)
                {
                    // Create the learning algorithm for RBMs
                    Algorithm = (h, v, i) => new ContrastiveDivergenceLearning(h, v)
                    {
                        LearningRate = LearningRate,
                        Momentum = 0.5,
                        Decay = WeightDecay,
                    },

                    LayerIndex = SelectedLayerIndex - 1,
                };

                double[][] inputs;
                inputs= GetInstances();
                int batchCount = Math.Max(1, inputs.Length / BatchSize);

                // Create mini-batches to speed learning
                int[] groups = Accord.Statistics.Tools
                    .RandomGroups(inputs.Length, batchCount);
                double[][][] batches = inputs.Subgroups(groups);

                // Gather learning data for the layer
                double[][][] layerData = teacher.GetLayerInput(batches);
                var cd = teacher.GetLayerAlgorithm(teacher.LayerIndex) as ContrastiveDivergenceLearning;

                // Start running the learning procedure
                for (int i = 0; i < Epochs ; i++)
                {
                    error = teacher.RunEpoch(layerData) / inputs.Length;

                    dispatcher.BeginInvoke((Action<int, double>)updateError,
                        DispatcherPriority.ContextIdle, i + 1, error);

                    if (i == 10)
                        cd.Momentum = Momentum;
                }

                IsLearning = false;

            //}).Start();
        }
        /// <summary>
        /// training the last layer of network for supervising the layer output
        /// </summary>
        private void learnLayerSupervised()
        {
            if (!CanClassify) return;
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            double error;
            /*new Task(() =>
            {*/
                DeepNeuralNetworkLearning teacher = new DeepNeuralNetworkLearning(Network)
                {
                    Algorithm = (ann, i) => new ParallelResilientBackpropagationLearning(ann),
                    LayerIndex = Network.Layers.Length - 1,
                };

                double[][] inputs, outputs;
                GetInstances(out inputs, out outputs);

                // Gather learning data for the layer
                double[][] layerData = teacher.GetLayerInput(inputs);

                // Start running the learning procedure
                for (int i = 0; i < Epochs; i++)
                {
                    error = teacher.RunEpoch(layerData, outputs);

                    dispatcher.BeginInvoke((Action<int, double>)updateError,
                        DispatcherPriority.ContextIdle, i + 1, error);
                }

                Network.UpdateVisibleWeights();
                IsLearning = false;

           // }).Start();
        }
        /// <summary>
        /// update error each epoch
        /// </summary>
        /// <param name="epoch"></param>
        /// <param name="error"></param>
        private void updateError(int epoch, double error)
        {
            IsStarting = false;
            CurrentEpoch = epoch;
            CurrentError = error;
            HasLearned = true;
        }
        #endregion

        #region Recognize
        /// <summary>
        /// recognize the image with the features of image
        /// </summary>
        /// <param name="shape">ShapeEoC</param>
        /// <returns>the index of classe in the outputs</returns>
        private int Recognize(ShapeEoC shape)
        {
            int Classification = 0;
            LoadPatternSignature(shape);
            // for network input
            double[] input = shape.GetSignature(this.SelectDescriptor).GetNormalisedFeatures().ToArray();

            // compute network and get it's ouput
            double[] output = Network.Compute(input);

            // find the maximum from output
            /*double max = output[0];
            for (int i = 1; i < output.Length; i++)
            {
                if (output[i] > max)
                {
                    max = output[i];
                    Classification = i;
                }
            }*/
            int imax; output.Max(out imax);
            Classification = imax;

            return Classification;
        }

        /// <summary>
        /// recognize the image with the pixel of image
        /// </summary>
        /// <param name="representativeBitmap">image must be the image after process(binarisation, normalisation..) </param>
        /// <returns>the index of classe in the outputs </returns>
        public int Recognize(Bitmap representativeBitmap)
        {
            int Classification = 0;
            
            // for network input
            double[] input = ToFeatures(representativeBitmap);

            // compute network and get it's ouput
            double[] output = Network.Compute(input);

            // find the maximum from output
            /*double max = output[0];
            for (int i = 1; i < output.Length; i++)
            {
                if (output[i] > max)
                {
                    max = output[i];
                    Classification = i;
                }
            }*/
            int imax; output.Max(out imax);
            Classification = imax;

            return Classification;
        }
        
        #endregion

        #region Calculations
        /// <summary>
        /// get the input data for learnLayerUnsupervised
        /// </summary>
        /// <returns></returns>
        public double[][] GetInstances()
        {
            double[][] input = new double[Training.Count][];
            /*for (int i = 0; i < input.Length; i++)
                input[i] = TrainSet[i].Features;*/
            Parallel.For(0, Training.Count, i =>
            {
                input[i] = TrainSet[i].Features;
            });

            return input;
        }
        /// <summary>
        /// get the input and output data for learnLayerSupervised
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void GetInstances(out double[][] input, out int[] output)
        {
            input = new double[Training.Count][];
            output = new int[Training.Count];
            double[][] tempInput = new double[Training.Count][];
            int[] tempOutput = new int[Training.Count];

            Parallel.For(0, Training.Count, i =>
            {
                tempInput[i] = TrainSet[i].Features;
                tempOutput[i] = TrainSet[i].Class;
            });

            input = tempInput;
            output = tempOutput;
           
        }
        /// <summary>
        /// get the input data and output data for learnLayerUnsupervised
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void GetInstances(out double[][] input, out double[][] output)
        {
            input = new double[Training.Count][];
            output = new double[Training.Count][];
            double[][] tempInput = new double[Training.Count][];
            double[][] tempOutput = new double[Training.Count][];

            Parallel.For(0, Training.Count, i =>
            {
                int total = Classes;

                tempInput[i] = TrainSet[i].Features;
                tempOutput[i] = new double[total];
                tempOutput[i][TrainSet[i].Class] = 1;
            });
            input = tempInput;
            output = tempOutput;

        }
        /// <summary>
        /// this structure save the data for training 
        /// </summary>
        public class TrainSample
        {
            /// <summary>
            /// siganture of pattern
            /// </summary>
            public double[] Features { get; set; }
            /// <summary>
            /// label of pattern
            /// </summary>
            public String Label { get; set; }
            /// <summary>
            /// class of pattern
            /// </summary>
            public int Class { get; set; }
            /// <summary>
            /// if the pattern is tested, the Result is the class output
            /// </summary>
            public int Result { get; set; }
            public bool? Match
            {
                get
                {
                    if (Result == -1) return null;
                    else return Result == Class;
                }
            }
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="features"></param>
            /// <param name="label"></param>
            /// <param name="classe"></param>
            /// <param name="result"></param>
            public TrainSample(double[] features, String label, int classe, int result)
            {
                this.Features = features;
                this.Result = result;
                this.Class = classe;
                this.Label = label;
            }
        }
        /// <summary>
        /// GetFontModels and set the signature by the value pixel of image 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fontModelList"></param>
        /// <param name="TrainSet"></param>
        /// <param name="nbClass"></param>
        public void GetFontModels(String directory, out List<FontModel> fontModelList, out List<TrainSample> TrainSet, out int nbClass)
        {
            fontModelList = new List<FontModel>();
            TrainSet = new List<TrainSample>();
            nbClass = 0;
            List<String> caraList = new List<String>();
            // Get the models (images + xml)
            String[] files = Directory.GetFiles(directory, "*.xml");

            // Process each xml file
            for (int i = 0; i < files.Length; i++)//String file in files)
            {
                String filename = Path.GetFileNameWithoutExtension(files[i]);
                if ((File.Exists(directory + @"\" + filename + ".png")) && (File.Exists(directory + @"\" + filename + "_bw.png")))
                {
                    // Create a new FontModel from the xml path
                    FontModel fontmodel = new FontModel(files[i]);
                    if (i == 0)
                    {
                        caraList.Add(fontmodel.TranscriptionCharacter);
                        nbClass++;
                    }
                    else
                        if (!caraList.Contains(fontmodel.TranscriptionCharacter))
                        {
                            caraList.Add(fontmodel.TranscriptionCharacter);
                            nbClass++;
                        }
                    Bitmap fontmodelBitmap = (Bitmap)Bitmap.FromFile(fontmodel.Directory + @"\" + fontmodel.NormalizedName + ".png");
                    PreprocessImage(ref fontmodelBitmap);
                    TrainSample trainSample = new TrainSample(ToFeatures(fontmodelBitmap), fontmodel.TranscriptionCharacter, nbClass - 1, -1);
                    TrainSet.Add(trainSample);

                    // Add the newly created FontModel in the list
                    fontModelList.Add(fontmodel);
                }
            }

        }

        public double[] ToFeatures(Bitmap bmp)
        {
            double[] features = new double[32 * 32];
            for (int i = 0; i < bmp.Height; i++)
                for (int j = 0; j < bmp.Width; j++)
                    features[i * bmp.Width + j] = (bmp.GetPixel(j, i).R > 0) ? 0 : 1;

            return features;
        }
        #endregion

        
        // The PropertyChanged event doesn't needs to be explicitly raised
        // from this application. The event raising is handled automatically
        // by the NotifyPropertyWeaver VS extension using IL injection.
        //
#pragma warning disable 0067
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

        
    }
}
