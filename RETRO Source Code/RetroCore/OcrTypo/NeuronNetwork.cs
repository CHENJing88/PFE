using AForge.Neuro;
using AForge.Neuro.Learning;
using Retro.ocr;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Retro.OcrTypo
{
    public class NeuronNetwork : IOCR, INotifyPropertyChanged
    {
        #region Attributes
        private int[,][] data;
        private Receptors receptors = new Receptors();
        private int initialReceptorsCount = 500;
        private int receptorsCount = 100;
        private Network neuralNet;

        private float learningRate1 = 1.0f;
        private float errorLimit1 = 1.0f;
        private float learningRate2 = 0.2f;
        private float errorLimit2 = 0.1f;
        private int learningEpoch = 0;

        private float error = 0.0f;
        private int misclassified = 0;

        private Thread workerThread;
        private ManualResetEvent stopEvent = null;
        private int workType;

        #endregion

        #region Constructor
        public NeuronNetwork()
        {
        }
        public NeuronNetwork(String FontFolderPath)
        {

        }

        #endregion



        // On "Generate" button click - generate receptors
        private void generateReceptorsButton_Click(object sender, System.EventArgs e)
        {
            GetReceptorsCount();

            // generate
            GenerateReceptors();
        }

        // Get recpetors count
        private void GetReceptorsCount()
        {
           
            initialReceptorsCount = 500;
            receptorsCount = 100;
        }

        // Generate receptors
        private void GenerateReceptors()
        {
            // remove previous receptors
            receptors.Clear();
            // set reception area size
            receptors.AreaSize = paintBoard.AreaSize;
            // generate new receptors
            receptors.Generate(initialReceptorsCount);
            
        }

        // On "Generate Data" button click
        private void generateDataButton_Click(object sender, System.EventArgs e)
        {
            workType = 0;
            // start work
            StartWork(false);

            // set status message
            //statusBox.Text = "Generating data ...";

            // create and start new thread
            workerThread = new Thread(new ThreadStart(GenerateLearningData));
            // start thread
            workerThread.Start();
        }

        // On "Filter Data" button click
        private void filterDataButton_Click(object sender, System.EventArgs e)
        {
            
            GetReceptorsCount();

            // filter data
            RemoveLearningDuplicates();
            FilterLearningData();

            // set receptors for image
            paintBoard.Receptors = receptors;
            paintBoard.Invalidate();
        }

        // Generate data for learning
        private void GenerateLearningData()
        {
            int objectsCount = 26;
            int featuresCount = receptors.Count;
            int variantsCount = 0;
            int fontsCount = fonts.Length * 2;
            int i, j, k, font, v = 0, step = 0;
            bool italic;

            // count variants
            for (i = 0; i < fonts.Length; i++)
            {
                variantsCount += (regularFonts[i]) ? 1 : 0;
                variantsCount += (italicFonts[i]) ? 1 : 0;
            }

            if (variantsCount == 0)
                return;

            // set progress size
            //progressBar.Maximum = objectsCount * variantsCount;

            // create data array
            data = new int[objectsCount, featuresCount][];
            // init each data element
            for (i = 0; i < objectsCount; i++)
            {
                for (j = 0; j < featuresCount; j++)
                {
                    data[i, j] = new int[variantsCount];
                }
            }

            // fill data ...

            // for all fonts
            for (j = 0; j < fontsCount; j++)
            {
                font = j >> 1;
                italic = ((j & 1) != 0);

                // skip disabled fonts
                

                // for all objects
                for (i = 0; i < objectsCount; i++)
                {
                    // draw letter
                    paintBoard.DrawLetter((char)((int)'A' + i), fonts[font], 90, italic, false);

                    // get receptors state
                    int[] state = receptors.GetReceptorsState(paintBoard.GetImage(false));

                    // copy receptors state
                    for (k = 0; k < featuresCount; k++)
                    {
                        data[i, k][v] = state[k];
                    }

                    // show progress
                    ReportProgress(++step, null);
                }

                v++;
            }

        }
        // Remove duplicates from learning data and receptors,
        // which produced these duplicates
        private void RemoveLearningDuplicates()
        {
            if (data == null)
                return;

            int objectsCount = data.GetLength(0);
            int featuresCount = data.GetLength(1);
            int variantsCount = data[0, 0].Length;

            int i, j, k, s;
            int[] item;

            // calculate checksum of each object for each receptor
            int[,] checkSum = new int[objectsCount, featuresCount];

            // for each object
            for (i = 0; i < objectsCount; i++)
            {
                // for each receptor
                for (j = 0; j < featuresCount; j++)
                {
                    item = data[i, j];
                    s = 0;

                    // for each variant
                    for (k = 0; k < variantsCount; k++)
                    {
                        s |= item[k] << k;
                    }

                    checkSum[i, j] = s;
                }
            }

            // find which receptors should be removed
            bool[] remove = new bool[featuresCount];

            // walk through all receptors ...
            for (i = 0; i < featuresCount - 1; i++)
            {
                // skip receptors alredy marked as deleted
                if (remove[i] == true)
                    continue;

                // ... and compare each receptor with others
                for (j = i + 1; j < featuresCount; j++)
                {
                    // remove by default
                    remove[j] = true;

                    // compare cheksums of all objects
                    for (k = 0; k < objectsCount; k++)
                    {
                        if (checkSum[k, i] != checkSum[k, j])
                        {
                            // ups, they are different, do not delete it
                            remove[j] = false;
                            break;
                        }
                    }
                }
            }

            // count receptors to save
            int receptorsToSave = 0;
            for (i = 0; i < featuresCount; i++)
                receptorsToSave += (remove[i]) ? 0 : 1;

            // filter data removing receptors with usability below acceptable
            int[,][] newData = new int[objectsCount, receptorsToSave][];
            Receptors newReceptors = new Receptors();

            k = 0;
            // for all receptors
            for (j = 0; j < featuresCount; j++)
            {
                if (remove[j])
                    continue;

                // for all objects
                for (i = 0; i < objectsCount; i++)
                {
                    newData[i, k] = data[i, j];
                }
                newReceptors.Add(receptors[j]);
                k++;
            }

            // set new data
            data = newData;
            receptors = newReceptors;
        }

        // Filter learning data
        private void FilterLearningData()
        {
            if (data == null)
                return;

            // data filtering is performed by removing bad receptors

            int objectsCount = data.GetLength(0);
            int featuresCount = data.GetLength(1);
            int variantsCount = data[0, 0].Length;
            int i, j, k, v;
            int[] item;

            // maybe we already filtered ?
            // so check that new receptors count is not greater than we have
            if (receptorsCount >= featuresCount)
                return;

            int[] outerCounters = new int[2];
            int[] innerCounters = new int[2];
            double ie, oe;

            double[] usabilities = new double[featuresCount];

            // for all receptors
            for (j = 0; j < featuresCount; j++)
            {
                // clear outer counters
                Array.Clear(outerCounters, 0, 2);

                ie = 0;
                // for all objects
                for (i = 0; i < objectsCount; i++)
                {
                    // clear inner counters
                    Array.Clear(innerCounters, 0, 2);
                    // get variants item
                    item = data[i, j];

                    // for all variants
                    for (k = 0; k < variantsCount; k++)
                    {
                        v = item[k];

                        innerCounters[v]++;
                        outerCounters[v]++;
                    }

                    // callculate inner entropy of receptor for current object
                    ie += Statistics.Entropy(innerCounters, variantsCount);
                }

                // average inner entropy
                ie /= objectsCount;
                // outer entropy
                oe = Statistics.Entropy(outerCounters, objectsCount * variantsCount);
                // receptors usability
                usabilities[j] = (1.0 - ie) * oe;
            }

            // create usabilities copy and sort it
            double[] temp = (double[])usabilities.Clone();
            Array.Sort(temp);
            // get acceptable usability for receptor
            double accaptableUsability = temp[featuresCount - receptorsCount];

            // filter data removing receptors with usability below acceptable
            int[,][] newData = new int[objectsCount, receptorsCount][];
            Receptors newReceptors = new Receptors();

            k = 0;
            // for all receptors
            for (j = 0; j < featuresCount; j++)
            {
                if (usabilities[j] < accaptableUsability)
                    continue;

                // for all objects
                for (i = 0; i < objectsCount; i++)
                {
                    newData[i, k] = data[i, j];
                }
                newReceptors.Add(receptors[j]);

                if (++k == receptorsCount)
                    break;
            }

            // set new data
            data = newData;
            receptors = newReceptors;
        }

        // Create Network
        private void CreateNetwork()
        {
            if (data == null)
                return;

            int objectsCount = data.GetLength(0);
            int featuresCount = data.GetLength(1);
            float alfa;

            // get alfa value
            alfa = 1.0f;
            
            

            // creare network
            if (layersCombo.SelectedIndex == 0)
            {
                neuralNet = new Network(new BipolarSigmoidFunction(alfa), featuresCount, objectsCount);
            }
            else
            {
                neuralNet = new Network(new BipolarSigmoidFunction(alfa), featuresCount, objectsCount, objectsCount);
            }

            // randomize network`s weights
            neuralNet.Randomize();
        }

        #region Training
        // On "Traing" button click
        private void traintNetworkButton_Click(object sender, System.EventArgs e)
        {
            outputBox.Text = string.Empty;

            // get parameters
            
            learningEpoch = 0;
            learningRate1 = 1.0f;
            errorLimit1 = 1.0f;
            learningRate2 = 0.2f;
            errorLimit2 = 0.1f;

            //
            workType = 1;
            progressBar.Hide();

            // start work
            StartWork(true);

            // set status message
            statusBox.Text = "Training network ...";

            // create and start new thread
            workerThread = new Thread(new ThreadStart(TrainNetwork));
            // start thread
            workerThread.Start();
        }

        /// <summary>
        /// Traing neural network to recognize our training set
        /// </summary>
        private void TrainNetwork()
        {
            if (data == null)
                return;

            int objectsCount = data.GetLength(0);
            int featuresCount = data.GetLength(1);
            int variantsCount = data[0, 0].Length;
            int i, j, k, n;

            // generate possible outputs
            float[][] possibleOutputs = new float[objectsCount][];

            for (i = 0; i < objectsCount; i++)
            {
                possibleOutputs[i] = new float[objectsCount];
                for (j = 0; j < objectsCount; j++)
                {
                    possibleOutputs[i][j] = (i == j) ? 0.5f : -0.5f;
                }
            }

            // generate network training data
            float[][] input = new float[objectsCount * variantsCount][];
            float[][] output = new float[objectsCount * variantsCount][];
            float[] ins;

            // for all varaints
            for (j = 0, n = 0; j < variantsCount; j++)
            {
                // for all objects
                for (i = 0; i < objectsCount; i++, n++)
                {
                    // prepare input
                    input[n] = ins = new float[featuresCount];

                    // for each receptor
                    for (k = 0; k < featuresCount; k++)
                    {
                        ins[k] = (float)data[i, k][j] - 0.5f;
                    }

                    // set output
                    output[n] = possibleOutputs[i];
                }
            }

            System.Diagnostics.Debug.WriteLine("--- learning started");
            // create network teacher
            BackPropagationLearning teacher = new BackPropagationLearning(neuralNet);

            // First pass
            teacher.LearningLimit = errorLimit1;
            teacher.LearningRate = learningRate1;

            i = 0;
            // learn
            do
            {
                error = teacher.LearnEpoch(input, output);
                i++;

                // report status
                if ((i % 100) == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Learning, 1st pass ... (iterations: {0}, error: {1})",i, error);
                }

                // need to stop ?
                if (stopEvent.WaitOne(0, true))
                    break;
            }
            while (((learningEpoch == 0) && (!teacher.IsConverged)) ||
                ((learningEpoch != 0) && (i < learningEpoch)));

            System.Diagnostics.Debug.WriteLine("first pass: " + i + ", error = " + error);

            // skip second pass, if learning epoch number was specified
            if (learningEpoch == 0)
            {
                // Second pass
                teacher = new BackPropagationLearning(neuralNet);

                teacher.Momentum = errorLimit2;
                teacher.LearningRate = learningRate2;

                // learn
                do
                {
                    error = teacher.LearnEpoch(input, output);
                    i++;

                    // report status
                    if ((i % 100) == 0)
                    {
                         System.Diagnostics.Debug.WriteLine("Learning, 2nd pass ... (iterations: {0}, error: {1})",i, error);
                    }

                    // need to stop ?
                    if (stopEvent.WaitOne(0, true))
                        break;
                }
                while (!teacher.IsConverged);

                System.Diagnostics.Debug.WriteLine("second pass: " + i + ", error = " + error);
            }

            // get the misclassified value
            misclassified = 0;
            // for all training patterns
            for (i = 0, n = input.Length; i < n; i++)
            {
                float[] realOutput = neuralNet.Compute(input[i]);
                float[] desiredOutput = output[i];
                int maxIndex1 = 0;
                int maxIndex2 = 0;
                float max1 = realOutput[0];
                float max2 = desiredOutput[0];

                for (j = 1, k = realOutput.Length; j < k; j++)
                {
                    if (realOutput[j] > max1)
                    {
                        max1 = realOutput[j];
                        maxIndex1 = j;
                    }
                    if (desiredOutput[j] > max2)
                    {
                        max2 = desiredOutput[j];
                        maxIndex2 = j;
                    }
                }

                if (maxIndex1 != maxIndex2)
                    misclassified++;
            }
        }
        #endregion

        #region Recognize
        private void Recognize()
        {
            int i, n, maxIndex = 0;

            // get current receptors state
            int[] state = receptors.GetReceptorsState(paintBoard.GetImage());

            // for network input
            float[] input = new float[state.Length];

            for (i = 0; i < state.Length; i++)
                input[i] = (float)state[i] - 0.5f;

            // compute network and get it's ouput
            float[] output = neuralNet.Compute(input);

            // find the maximum from output
            float max = output[0];
            for (i = 1, n = output.Length; i < n; i++)
            {
                if (output[i] > max)
                {
                    max = output[i];
                    maxIndex = i;
                }
            }

            //
            outputBox.Text = string.Format("{0}", (char)((int)'A' + maxIndex));
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
