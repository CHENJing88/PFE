using Polytech.Clustering.Plugin;
using Retro.ocr;
using Retro.OcrTypo;
using RetroUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Retro.ViewModel
{
    public class AutoTransVewModel
    {
        #region Attributes
        /// <summary>
        /// RetroViewModel
        /// </summary>
        public RetroViewModel _retroVM { get; private set; }
        /// <summary>
        /// machine learing Template Matching
        /// </summary>
        private TemplateMatchingOCREngine OCREngine;
        /// <summary>
        /// machine learing DeepLearning
        /// </summary>
        private DeepLearning DBNEngine;
        /// <summary>
        /// machine learing KNearestNeighbors
        /// </summary>
        private KNNs KNNsEngine;

        /// <summary>
        /// Transcription Method list
        /// </summary>
        private List<String> m_transcripMethod;
        public List<String> TranscripMethod
        {
            get { return m_transcripMethod; }
            set
            {
                m_transcripMethod = value;
            }
        }
        
        /// <summary>
        /// Descriptor Method list
        /// </summary>
        private List<String> m_descriptorMethod;
        public List<String> DescriptorMethod
        {
            get { return m_descriptorMethod; }
            set
            {
                m_descriptorMethod = value;
            }
        }

        #endregion

        #region Constructor
        public AutoTransVewModel(RetroViewModel retroViewModel)
        {
            this._retroVM = retroViewModel;
        }
        /// <summary>
        /// run transcription automatiquement
        /// </summary>
        /// <param name="SelectDescripMethod">description method</param>
        /// <param name="SelectTranscripMethod">transcription method</param>
        /// <param name="FontMondelFolderPath">the folder path of fontModel </param>
        public void Run(string SelectDescripMethod, string SelectTranscripMethod, string FontModelFolderPath)
        {
            if (_retroVM.RetroInstance != null)
            {
                // Get non labelized cluster
                List<Cluster> clustersList = _retroVM.GetClusters().FindAll(
                                                    delegate(Cluster cluster)
                                                    {
                                                        return !cluster.IsLabelized;
                                                    }
                                                );

                if (clustersList.Count == 0)
                {
                    MessageBox.Show("There is no non labelized Clusters to transcript.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if ((FontModelFolderPath.Length != 0) && (Directory.Exists(FontModelFolderPath)))
                {
                    // Display the found number of FontModels, and ask if the user want to proceed
                    MessageBoxResult result = MessageBox.Show(Directory.GetFiles(FontModelFolderPath, "*.xml").Length + " Font Models have been found in the selected folder.\nProceed the Automatic Transcription?", "Notification", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        DynamicSplashScreenNotification dynamicSplashScreenNotification = new DynamicSplashScreenNotification();
                        dynamicSplashScreenNotification.Message = "Starting Automatic Transcription process";

                        // Create DynamicSplashScreen thread
                        System.Threading.Thread dynamicSplashScreenThread = new System.Threading.Thread((object parameter) =>
                        {
                            List<String> processName = new List<String>();
                            processName.Add("Automatic");
                            processName.Add("Transcription");
                            DynamicSplashScreen dynamicSplashScreen = new DynamicSplashScreen(processName, (DynamicSplashScreenNotification)parameter);
                            dynamicSplashScreen.ShowDialog();
                        });

                        // Start DynamicSplashScreen thread
                        dynamicSplashScreenThread.SetApartmentState(ApartmentState.STA);
                        dynamicSplashScreenThread.Start(dynamicSplashScreenNotification);


                        // Create automatic OCR thread
                        int nbTranscribedClusters = 0;
                        System.Threading.Thread automaticOCRThread = new System.Threading.Thread((object parameter) =>
                        {
                            if (SelectTranscripMethod != null || FontModelFolderPath != null)
                                switch (SelectTranscripMethod)
                                {
                                    case "Template Matching":
                                        // Get list of Font Models
                                        OCREngine = new TemplateMatchingOCREngine();
                                        List<FontModel> fontModelsList = OCREngine.GetFontModels(FontModelFolderPath); 
                                        nbTranscribedClusters = OCREngine.RunOCR(clustersList, fontModelsList, dynamicSplashScreenNotification); break;

                                    case "Deep Belif Learning":
                                        DBNEngine = new DeepLearning();
                                        nbTranscribedClusters = DBNEngine.RunDeepLearningOCR(clustersList,FontModelFolderPath, SelectDescripMethod, dynamicSplashScreenNotification); break;

                                    case "K-Nearest Neighborhoods":
                                        KNNsEngine = new KNNs(); 
                                        nbTranscribedClusters =KNNsEngine.RunKNNsOCR(clustersList, FontModelFolderPath, SelectDescripMethod, dynamicSplashScreenNotification);break;

                                }
                        });

                        // Start automatic OCR thread
                        automaticOCRThread.Start(dynamicSplashScreenNotification);

                        // wait for automatic OCR thread end
                        automaticOCRThread.Join();

                        // End the DynamicSplashScreen Thread
                        dynamicSplashScreenThread.Abort();

                        // Notify user
                        MessageBox.Show("Automatic OCR has been processed.\n" + nbTranscribedClusters + " Clusters have been automatically transcribed.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                     }
                }
            }
        }

        
        #endregion

        #region Method List
        /// <summary>
        /// set the descriptor method list of the comobox
        /// </summary>
        /// <returns>the list of DescriptorMethod</returns>
        public List<String> DescriptorMethodList()
        {
            //add the list of discriptor method
            DescriptorMethod = new List<String>();
            DescriptorMethod.Add("Zernike");
            //DiscriptorMethod.Add("Directionnal");

            return DescriptorMethod;
        }
        /// <summary>
        /// set the transcription method list of the comobox
        /// </summary>
        /// <returns>the list of TranscripMethod</returns>
        public List<String> TranscripMethodList()
        {
            //add the list of transcription method
            TranscripMethod = new List<String>();
            TranscripMethod.Add("K-Nearest Neighborhoods");
            TranscripMethod.Add("Deep Belif Learning");
            TranscripMethod.Add("Template Matching");
            return TranscripMethod;
        }
        #endregion

        

        
    }
}
