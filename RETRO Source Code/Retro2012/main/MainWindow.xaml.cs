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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

using System.Drawing;
using System.ComponentModel;
using System.Threading;
using System.IO;
using AvalonDock;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

using RetroGUI.export;
using RetroGUI.clustering;
using RetroGUI.transcription;
using RetroGUI.typography;
using RetroGUI.util;
using RetroGUI.visualisation;
using RetroUtil;
using Retro.ViewModel;
using Polytech.Clustering.Plugin;
using Retro.ocr;



namespace RetroGUI.main
{
    /// <summary>
    /// Define Main Window of the RETRO
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Attributes

        //Attributes

        /// <summary>
        /// Define retro View-Model
        /// </summary>
        public static RetroViewModel _retroVM;

        private DocumentContent clusteringDocumentContent = null;
        private DocumentContent manualTranscriptionDocumentContent = null;
        private DocumentContent autoTranscriptionDocumentContent = null;
        private DocumentContent exportEoCTranscriptionDocumentContent = null;
        private DocumentContent analyseClusterDocumentContent = null;
       
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _retroVM = new RetroViewModel();
            this.RetroContent.Close();
            this.ClustersContent.Close();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        }
        #endregion


        #region Project Menu Events Handler

        /// <summary>
        /// Handler for File/NewProject menu item
        /// </summary>
        private void Click_File_NewProject(object sender, RoutedEventArgs e)
        {
            // ClearClustersList eventuel open projects
            bool createNewProject = true;
            if (_retroVM.RetroInstance != null)
                createNewProject = this.CloseOpenProject();

            if (createNewProject)
            {
                // Display the new project windows 
                NewProjectWindow npw = new NewProjectWindow(_retroVM);
                npw.Owner = this;
                npw.ShowDialog();

                if (_retroVM.RetroInstance != null)
                {
                    // Add to rencent file list
                    //RecentFileList.InsertFile(_retroVM.RetroInstance.RetroProjectFilePath);

                    // Display informations and datas
                    this.Display();
                }
            }

        }


        /// <summary>
        /// Handler for File/OpenProject menu item
        /// </summary>
        private void Click_File_OpenProject(object sender, RoutedEventArgs e)
        {
            // ClearClustersList eventuel open projects
            bool openNewProject = true;
            if (_retroVM.RetroInstance != null)
                openNewProject = this.CloseOpenProject();

            // If user has validate the opening
            if (openNewProject)
            {
                // Open Dialog
                OpenFileDialog ofd = new OpenFileDialog();                
                ofd.InitialDirectory = Environment.SpecialFolder.Personal.ToString();
                ofd.Filter = "Retro Files|*.xml;|All Files|*.*";
                ofd.ShowDialog();

                // Open Project
                if (ofd.SafeFileName.Length != 0)
                    this.OpenProject(ofd.FileName);
            }
        }


        /// <summary>
        /// Handler for File/SaveProject menu item
        /// </summary>
        private void Click_File_SaveProject(object sender, RoutedEventArgs e)
        {
            if (_retroVM.RetroInstance != null)
            {
                this.SaveProject("");
            }
        }


        /*
        // <summary>
        /// Handler for File/SaveProjectAs menu item
        /// </summary>
        private void Click_File_SaveProjectAs(object sender, RoutedEventArgs e)
        {
            if (_retroVM.RetroInstance != null)
            {
                // Open FileDialog
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "XML Files | *.xml";
                sfd.DefaultExt = "xml";
                sfd.ShowDialog();

                // Save project
                if (sfd.SafeFileName.Length != 0)
                    this.SaveProject(sfd.FileName);
            }
        }
         */ 


        /// <summary>
        /// Handler for File/Exit menu item
        /// </summary>
        private void Click_File_CloseProject(object sender, RoutedEventArgs e)
        {
            if (_retroVM.RetroInstance != null)
            {
                this.CloseProject();
            }
        }


        /// <summary>
        /// Handler for File/Recent projects menu item
        /// </summary>
        private void Click_File_RecentProject(object sender, RetroGUI.util.RecentFileList.MenuClickEventArgs e)
        {
            // ClearClustersList eventuel open projects
            bool openNewProject = true;
            if (_retroVM.RetroInstance != null)
                openNewProject = this.CloseOpenProject();

            // If user has validate the opening
            if (openNewProject)
            {
                // Open project
                this.OpenProject(e.Filepath);
            }
        }


        /// <summary>
        /// Handler for File/Exit menu item
        /// </summary>
        private void Click_File_Exit(object sender, RoutedEventArgs e)
        {
            // ClearClustersList the Application and its window
            Close();
        }

        #endregion


        #region EoCs Menu Events Handler

        /// <summary>
        /// Override compute signatures event
        /// </summary>
        private void ComputeSignatures_Click(object sender, RoutedEventArgs e)
        {
            
        }

        #endregion


        #region PAGE Menu Events Handler

        /// <summary>
        /// Handler for Page/OpenPage menu item
        /// </summary>
        private void Click_Page_OpenPage(object sender, RoutedEventArgs e)
        {
            if (_retroVM.RetroInstance != null)
            {
                PageWindow pw = new PageWindow(_retroVM);
                pw.Owner = this;
                pw.ShowDialog();
            }
        }


        /// <summary>
        /// Handler for Page/ViewIllustrations menu item
        /// </summary>
        private void Click_Page_ViewIllustrations(object sender, RoutedEventArgs e)
        {
            if (_retroVM.RetroInstance != null)
            {
                IllustrationWindow iw = new IllustrationWindow(_retroVM);
                iw.Owner = this;
                iw.ShowDialog();
            }
        }

        #endregion


        #region CLUSTERS Menu Events Handler

        /// <summary>
        /// Handler for TestModule/Process TestModule menu item
        /// </summary>
        private void Click_Clustering_ProcessClustering(object sender, RoutedEventArgs e)
        {
            if (_retroVM.RetroInstance != null)
            {
                // Create and display a new tab (only if not shown yet)
                if (this.clusteringDocumentContent == null)
                {
                    // Process TestModule
                    ClusteringPanel cp = new ClusteringPanel(
                        _retroVM.RetroInstance.AgoraAltoPath,
                        _retroVM.RetroInstance.ClusteringPath
                        );

                    clusteringDocumentContent = new DocumentContent() 
                    { 
                        Name = "ClusteringDocumentContent", 
                        Title = "Clustering", 
                        Content = cp 
                    };
                    clusteringDocumentContent.Show(dockManager);
                    clusteringDocumentContent.Activate();
                }
            }
        }


        /// <summary>
        /// Handler for Load Clusters menu item
        /// </summary>
        private void Click_Clustering_LoadClusters(object sender, RoutedEventArgs e)
        {
            if (_retroVM.RetroInstance != null)
            {
                // Remove current clusters
                // Load Clusters in memory
                // Create loading windows thread
                System.Threading.Thread loadingThread = new System.Threading.Thread(() =>
                {
                    // Dynamic SplashScreen
                    LoadingWindow splashScreen = new LoadingWindow();
                    splashScreen.ShowDialog();
                });

                // Start the loading window thread
                loadingThread.SetApartmentState(ApartmentState.STA);
                loadingThread.Start();

                // Create the loading thread
                System.Threading.Thread startupThread = new System.Threading.Thread(() =>
                {
                    // Open the selected project - may be time consuming
                    _retroVM.LoadClusters();
                    //System.Windows.MessageBox.Show("" + _retroVM.GetClusters().Count);
                });

                // Start the loading thread 
                startupThread.Start();

                // Wait the end of the loading thread
                startupThread.Join();

                // ClearClustersList splash screen
                loadingThread.Abort();

                // Save the project
                this.DisplayRetroInfos();
                this.SaveProject("");

                //System.Windows.MessageBox.Show(""+_retroVM.GetClusters().Count);

            }
        }


        /// <summary>
        /// Handler for TestModule/View Clusters menu item
        /// </summary>
        private void Click_Clustering_ViewClusters(object sender, RoutedEventArgs e)
        {
            if (_retroVM.RetroInstance != null)
            {
                // Display the Clusters
                this.DisplayClusters();
            }
            if (clustering.ClusteringPanel.clusteringMethodPluginUsedForClustering != null)
                this.ClusteringResultView.listBoxClusteringMethodInformation.ItemsSource = clustering.ClusteringPanel.clusteringMethodPluginUsedForClustering.GetInfoList();
            if (clustering.ClusteringPanel.descriptorPluginUsedForClustering != null)
                this.ClusteringResultView.listBoxDescriptorInformations.ItemsSource = clustering.ClusteringPanel.descriptorPluginUsedForClustering.GetInfoList();
        }


        /// <summary>
        /// Handler for TestModule/Generaye Stats menu item
        /// </summary>
        private void Click_Clustering_GenerateStats(object sender, RoutedEventArgs e)
        {
            if (_retroVM.RetroInstance != null)
            {
                //ExportTool.CreateStatsXml(_retroVM.RetroInstance.ClusteringPath);
                MessageBox.Show("The file stats.xml has been created.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Handler for Analyse Clusters menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Clustering_AnalyseClusters(object sender, RoutedEventArgs e)
        {
            if (_retroVM.RetroInstance != null)
            {
                //anaylse the Cluster selected in the window ClusteringResultView
                if (this.ClusteringResultView.SelectedCluster != null)
                {
                    //init the window of AnalyseClusterPanel
                    AnalyseClusterPanel analyseClusterPanel = new AnalyseClusterPanel(this.ClusteringResultView.SelectedCluster, _retroVM);
    
                        if (this.analyseClusterDocumentContent == null)
                        {
                                analyseClusterDocumentContent = new DocumentContent() 
                                { 
                                    Name = "AnalyseClusterDocumentContent", 
                                    Title = "Analyze Cluster " + this.ClusteringResultView.SelectedCluster.Id, 
                                    Content = analyseClusterPanel 
                                };
                        }else
                        {
                            //((AnalyseClusterPanel)analyseClusterDocumentContent.Content).ClusterCurrent = this.ClusteringResultView.SelectedCluster;
                            MessageBoxResult result=MessageBox.Show("Please save analyse resultat first before start another analyse.\nDo you want to start a new analysis ? ", "Warnning", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                analyseClusterDocumentContent.Close();
                                analyseClusterDocumentContent = new DocumentContent()
                                {
                                    Name = "AnalyseClusterDocumentContent",
                                    Title = "Analyze Cluster " + this.ClusteringResultView.SelectedCluster.Id,
                                    Content = analyseClusterPanel
                                };
                            }
                            
                        }
                        analyseClusterDocumentContent.Show(dockManager);
                        analyseClusterDocumentContent.Activate();
                  
                }
                    
            }
            else 
            {
                MessageBox.Show("Please choose a Cluster in the window 'Cluster'->'View / Edit' first. ", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
       
        #endregion


        #region TRANSCRIPTION Menu Events Handler

        /// <summary>
        /// Handler for Transcription/ManualTranscription menu item
        /// </summary>
        private void Click_Transcription_ManualTranscription(object sender, RoutedEventArgs e)
        {
            if (_retroVM.RetroInstance != null)
            {
                // Create and display a new tab (only uif not shown yet)
                if (this.manualTranscriptionDocumentContent == null)
                {
                    // Process TestModule
                    ManualTranscriptionPanel mtp = new ManualTranscriptionPanel(_retroVM);

                    manualTranscriptionDocumentContent = new DocumentContent() { Name = "ManualTranscriptionDocumentContent", Title = "Manual Transcription", Content = mtp };
                    manualTranscriptionDocumentContent.Closing += new EventHandler<CancelEventArgs>(ManualTranscritpionPanel_OnClosing);
                    manualTranscriptionDocumentContent.Show(dockManager);
                    manualTranscriptionDocumentContent.Activate();
                }
            }
        }


        /// <summary>
        /// Handler for Transcription/AutomaticTranscription menu item
        /// </summary>
        private void Click_Transcription_AutomaticTranscription(object sender, RoutedEventArgs e)
        {
            // Create and display a new tab (only uif not shown yet)
            if (this.autoTranscriptionDocumentContent == null)
            {
                // Process TestModule
                AutoTranscriptionPanel atp = new AutoTranscriptionPanel(_retroVM);

                autoTranscriptionDocumentContent = new DocumentContent() { Name = "AutoTranscriptionDocumentContent", Title = "Auto Transcription", Content = atp };
                //autoTranscriptionDocumentContent.Closing += new EventHandler<CancelEventArgs>(ManualTranscritpionPanel_OnClosing);
                autoTranscriptionDocumentContent.Show(dockManager);
                autoTranscriptionDocumentContent.Activate();
            }
            
            /*if (_retroVM.RetroInstance != null)
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
                }*/

                // Give instruction to user 
            /*MessageBox.Show("Please select the folder of the FontModels.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);

            // Open a dialog for folder selection
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowDialog();

            if ((fbd.SelectedPath.Length != 0) && (Directory.Exists(fbd.SelectedPath)))
            {
                    
                // Get list of Font Models
                TemplateMatchingOCREngine tm = new TemplateMatchingOCREngine();
                List<FontModel> fontModelsList = tm.GetFontModels(fbd.SelectedPath);

                // Display the found number of FontModels, and ask if the user want to proceed
                MessageBoxResult result = MessageBox.Show(fontModelsList.Count + " Font Models have been found in the selected folder.\nProceed the Automatic Transcription?", "Notification", MessageBoxButton.YesNo, MessageBoxImage.Question);

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
                        nbTranscribedClusters = tm.RunOCR(clustersList, fontModelsList, dynamicSplashScreenNotification);
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
         }*/
        }

        #endregion


        #region BOOKS Menu Events Handler

        /// <summary>
        /// Handler for Results/ExportAsAlto menu item
        /// </summary>
        private void Click_Books_ExportAsAlto(object sender, RoutedEventArgs e)
        {

            if (_retroVM.RetroInstance != null)
            {
                DynamicSplashScreenNotification dynamicSplashScreenNotification = new DynamicSplashScreenNotification();
                dynamicSplashScreenNotification.Message = "Starting Export to alto files process";

                // Create DynamicSplashScreen thread
                System.Threading.Thread dynamicSplashScreenThread = new System.Threading.Thread((object parameter) =>
                {
                    List<String> processName = new List<String>();
                    processName.Add("Export Results");
                    processName.Add("in Alto files");
                    DynamicSplashScreen dynamicSplashScreen = new DynamicSplashScreen(processName, (DynamicSplashScreenNotification)parameter);
                    //dynamicSplashScreen.Owner = this;
                    dynamicSplashScreen.ShowDialog();
                });

                // Start DynamicSplashScreen thread
                dynamicSplashScreenThread.SetApartmentState(ApartmentState.STA);
                dynamicSplashScreenThread.Start(dynamicSplashScreenNotification);

                // Run Export process
                _retroVM.ExportAsAlto(dynamicSplashScreenNotification);

                // End the DynamicSplashScreen Thread
                dynamicSplashScreenThread.Abort();

                // Notify user
                MessageBox.Show("The results has been exported as in the alto files.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }


        /// <summary>
        /// Handler for Results/ExportEoCAnnotation menu item
        /// </summary>
        private void Click_Export_EoC_Annotation(object sender, RoutedEventArgs e)
        {
            // Create and display a new tab (only if not shown yet)
            if (this.exportEoCTranscriptionDocumentContent == null)
            {
                ExportEoCTranscriptionPanel eetp = null;
                if (_retroVM.RetroInstance != null)
                {
                    String annotationPath = _retroVM.RetroInstance.ClusteringPath + @"\annotations\";
                    if (Directory.Exists(annotationPath))
                    {
                        MessageBoxResult result = MessageBox.Show("The annotation/ folder already exists for this project. If you continue, its content will be erased.\n Do you want to continue?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.No)
                            return;
                        else
                            Directory.Delete(annotationPath, true);

                        Directory.CreateDirectory(annotationPath);
                    }

                    eetp = new ExportEoCTranscriptionPanel(_retroVM.RetroInstance.AgoraAltoPath, annotationPath);
                }
                else
                {
                    eetp = new ExportEoCTranscriptionPanel("", "");
                }

                exportEoCTranscriptionDocumentContent = new DocumentContent() { Name = "ExportEoCTranscriptionDocumentContent", Title = "Export EoC Transcription", Content = eetp };
                exportEoCTranscriptionDocumentContent.Show(dockManager);
                exportEoCTranscriptionDocumentContent.Activate();
            }
        }

        #endregion


        #region TYPOGRAPHY Menu Events Handler

        /// <summary>
        /// Handler for Typography/AddModel menu item
        /// </summary>
        private void Click_Typography_AddModel(object sender, RoutedEventArgs e)
        {
            AddModelWindow amw = new AddModelWindow();
            amw.Owner = this;
            amw.Show();
        }


        /// <summary>
        /// Handler for Typography/BodyHeight menu item
        /// </summary>
        private void Click_Typography_BodyHeight(object sender, RoutedEventArgs e)
        {
            BodyHeightWindow bhw = new BodyHeightWindow();
            bhw.Owner = this;
            bhw.Show();
        }


        /// <summary>
        /// Handler for TestModule/Clusters to Models menu item
        /// </summary>
        private void Click_Typo_ClustersToModels(object sender, RoutedEventArgs e)
        {
            if (_retroVM.RetroInstance != null)
            {
                // Get labelized cluster
                List<Cluster> clustersList = _retroVM.GetClusters().FindAll(
                                                    delegate(Cluster cluster)
                                                    {
                                                        return cluster.IsLabelized;
                                                    }
                                                );

                if (clustersList.Count == 0)
                {
                    MessageBox.Show("There is no (labelized) Clusters to convert as Model.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Give instruction to user 
                MessageBox.Show("Please select the folder where models will be exported.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);

                // Open a dialog for folder selection
                System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                fbd.ShowDialog();

                if ((fbd.SelectedPath.Length != 0) && (Directory.Exists(fbd.SelectedPath)))
                {
                    // Create Models from Clusters
                    int cpt = 0;
                    foreach (Cluster cluster in clustersList)
                    {
                        // Build name
                        String filename = fbd.SelectedPath + @"\" + cluster.LabelList[0] + cpt + "_XX_X_X_XX_XXX";

                        // Save grayscale image
                        Bitmap bitmap = cluster.Representatives[0].ImageRepresentation;
                        bitmap.Save(filename + ".png", ImageFormat.Png);

                        // Save Black and white image
                        if (bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                        {
                            Grayscale filterG = new Grayscale(0.2125, 0.7154, 0.0721);
                            bitmap = filterG.Apply(bitmap);
                        }
                        SISThreshold filterB = new SISThreshold();
                        bitmap = filterB.Apply(bitmap);
                        bitmap.Save(filename + "_bw.png", System.Drawing.Imaging.ImageFormat.Png);

                        // Free bitmap
                        bitmap.Dispose();

                        // Export to XML
                        AddModelDataWindow.DefaultCreateModel(filename + ".xml", cluster.LabelList[0]);

                        // Increase model count
                        cpt++;
                    }

                    // Norification
                    MessageBox.Show("Clusters representatives have been exported as Models.\nWARNING: XML information of the Models have to be filled manually.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
        }

        #endregion


        #region HELP Menu Events Handler

        /// <summary>
        /// Handler for Help/About menu item
        /// </summary>
        private void Click_Help_About(object sender, RoutedEventArgs e)
        {
            AboutWindow aw = new AboutWindow();
            aw.Owner = this;
            aw.ShowDialog();
        }

        #endregion


        #region Project management related Methods

        /// <summary>
        /// If a project is already open
        /// Confirm with the user how he want to proceed
        /// and close the current project if asked
        /// </summary>
        private bool CloseOpenProject()
        {
            bool proceedCloseProject = true;

            MessageBoxResult mbResult = MessageBox.Show("A project is already opened.\n Do you want to continue?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (mbResult == MessageBoxResult.Yes)
                this.CloseProject();                // Save and close the current project
            else
                proceedCloseProject = false;        // Notify the calling method not to proceed

            //RestartApplication();

            return proceedCloseProject;
        }

        //private void RestartApplication()
        //{
        //    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
        //    Application.Current.Shutdown();
        //}


        /// <summary>
        /// Open a RETRO project
        /// All project must be closed before
        /// </summary>
        /// <param name="filepath">Path of the project to open</param>
        private void OpenProject(String filepath)
        {
            // Open File
            Retro.Model.ReturnValues.OpenProject result = Retro.Model.ReturnValues.OpenProject.Ok;
            if (filepath.Length != 0)
            {
                // Create loading windows thread
                System.Threading.Thread loadingThread = new System.Threading.Thread(() =>
                {
                    // Dynamic SplashScreen
                    LoadingWindow splashScreen = new LoadingWindow();
                    splashScreen.ShowDialog();
                });

                // Start the loading window thread
                loadingThread.SetApartmentState(ApartmentState.STA);
                loadingThread.Start();

                // Create the loading thread
                System.Threading.Thread startupThread = new System.Threading.Thread(() =>
                {
                    // Open the selected project - may be time consuming
                    result = _retroVM.OpenProject(filepath);
                });

                // Start the loading thread 
                startupThread.Start();

                // Wait the end of the loading thread
                startupThread.Join();

                // ClearClustersList splash screen
                loadingThread.Abort();
                
                 //result = _retroVM.OpenProject(filepath);
                
                // Continue the work
                if (result != Retro.Model.ReturnValues.OpenProject.Ok)
                {
                    // Display error message
                    MessageBox.Show(Retro.Model.ReturnValues.OpenProjectErrorMessage[(int)result], "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    // Remove from recent file list
                    //RecentFileList.RemoveFile(filepath);

                    return;
                }
                
                
                // Add to rencent file list
                //RecentFileList.InsertFile(_retroVM.RetroInstance.RetroProjectFilePath);

                // Display informations and datas
                this.Display();
            }
        }


        /// <summary>
        /// Save a RETRO project
        /// </summary>
        /// <param name="filepath">Path of the file where to save the project</param>
        private void SaveProject(String filepath)
        {
            // Save As
            if (filepath.Length != 0)
            {
                _retroVM.SaveProject(filepath);

                // Update Retro panel info
                this.DisplayRetroInfos();

                // Add to rencent file list
                //RecentFileList.InsertFile(filepath);
            }
            // Save
            else
            {
                _retroVM.SaveProject(_retroVM.RetroInstance.RetroProjectFilePath);
            }

            // Display notification
            MessageBox.Show("The project has been saved.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        /// <summary>
        /// ClearClustersList a RETRO project
        /// Confirm if the project has to be saved before
        /// </summary>
        private void CloseProject()
        {

            MessageBoxResult result = MessageBox.Show("Do you want to save the project before closing?",
                "Close project", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _retroVM.SaveProject(_retroVM.RetroInstance.RetroProjectFilePath);
                MessageBox.Show("The project has been saved.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Free Retro Instance
            _retroVM.CloseProject();
            _retroVM.RetroInstance = null;

            // Free Left Information panel
            this.retroPropertyGrid.Reset();

            // Free Clusters
            //this.ClustersDisplay.Reset();


            // Display autoclosing notification
            // Useful to force main GUI refresh
            ToastWindow notification = new ToastWindow("Closed", 1);
            notification.Owner = this;
            notification.ShowDialog();


        }

        #endregion


        #region Project Information & Data Display Methods

        /// <summary>
        /// Display information and datas of the project
        /// </summary>
        private void Display()
        {
            this.DisplayRetroInfos();
            //this.DisplayClusters();
        }


        /// <summary>
        /// Display the Retro Info
        /// </summary>
        private void DisplayRetroInfos()
        {
            this.RetroContent.Show();
            this.retroPropertyGrid.descriptionTextBlock.Background = System.Windows.Media.Brushes.LightSteelBlue;
            this.retroPropertyGrid.SelectedObject = _retroVM.RetroInstance;
            this.InfomationDocumentPane.SelectedIndex = 0;
        }


        /// <summary>
        /// Display the Clusters of this project
        /// </summary>
        private void DisplayClusters()
        {
            // Enable the view
            this.ClustersContent.Show();
            this.ClustersContent.Activate();

            // Assign the clusters list
            List<Cluster> clustersList = _retroVM.GetClusters();
            this.ClusteringResultView.Clusters = clustersList;
            this.ClusteringResultView.DisplayClusters();
        }

        #endregion


        #region Closing Methods

        /// <summary>
        /// Override ClearClustersList event to assure that all the windows are closed
        /// </summary>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {

            if (_retroVM.RetroInstance != null)
            {
                MessageBoxResult sure = MessageBox.Show("Do you really want to close the program ?", "Close projet", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (sure == MessageBoxResult.Yes)
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to save the project before leaving?", "Exit RETRO", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _retroVM.SaveProject(_retroVM.RetroInstance.RetroProjectFilePath);
                        MessageBox.Show("The project has been saved.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }

            // Assure that all the windows of the current application are closed
            for (int intCounter = App.Current.Windows.Count - 1; intCounter > 0; intCounter--)
                App.Current.Windows[intCounter].Close();

            //App.Current.Shutdown();
            base.OnClosing(e);

            Environment.Exit(0);
        }


        /// <summary>
        /// Override ClearClustersList event to set a flag in order manage the ClusterWindow Lifetime
        /// </summary>
        private void ManualTranscritpionPanel_OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            this.manualTranscriptionDocumentContent = null;
        }

        #endregion

    }

}
