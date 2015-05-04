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
using System.Windows.Shapes;

using RetroGUI.main;
using System.Xml;
using RetroGUI.util;
using System.Windows.Threading;
using System.Windows.Media.Effects;
using Microsoft.Win32;
using Retro.ViewModel;
using System.Threading;
using RetroGUI.typography;
using System.IO;
using System.Drawing;
using RetroUtil;
using AForge.Imaging;

namespace RetroGUI.visualisation
{
    /// <summary>
    /// Define Page Window
    /// This version only exploit alto and imageSource
    /// and therefore doens't need the AGORA exported thumbnails anymore
    /// </summary>
    public partial class PageWindow : Window
    {

        #region struct EoCAttributes definition

        /// <summary>
        /// Defne an EoCAttributes structure
        /// in order to store in memory list of EoC
        /// </summary>
        struct EoCAttributes
        {
            // Attributes
            public String ID;
            public int HPOS;
            public int VPOS;
            public int WIDTH;
            public int HEIGHT;
            public String CONTENT;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="id">EoC ID</param>
            /// <param name="hpos">EoC horizontal position</param>
            /// <param name="vpos">EoC vertical position</param>
            /// <param name="width">EoC width</param>
            /// <param name="height">EoC height</param>
            /// <param name="content">EoC textual content</param>
            public EoCAttributes(String id, int hpos, int vpos, int width, int height, String content)
            {
                this.ID = id;
                this.HPOS = hpos;
                this.VPOS = vpos;
                this.WIDTH = width;
                this.HEIGHT = height;
                this.CONTENT = content;
            }
        }

        #endregion


        #region Attributes

        // Other window related attributes
        private EoCWindow eocw = new EoCWindow();
        private bool displayMetadata = true;
        private ModelMetaData modelMetadata;
        private AddModelDataWindow amdw;

        // Images & alto files Attributes
        private String imagesDirectory;
        private String altoDirectory;
        private List<String> imagesList = null;
        private List<String> altoList = null;
        private int currentAltoIndex = 0;
        private ImageSourceConverter converter = new ImageSourceConverter();

        // EoC Attributes
        private int eocGranularity = 1; // Textline
        private List<EoCAttributes> textEoCList = new List<EoCAttributes>();
        private List<EoCAttributes> illustrationsEoCList = new List<EoCAttributes>();

        // Current eoc attribute
        private EoCAttributes currentEoCAttributes = new EoCAttributes();
        private Bitmap currentEoCBitmap = null;
        private String currentImageSourceOriginalFilename = "";

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="retroVM">Instance of Retro View Model</param>
        public PageWindow(RetroViewModel retroVM)
        {
            InitializeComponent();
            this.eocw.Visibility = Visibility.Hidden;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Get the list of alto file linked to the current project
            this.altoDirectory = retroVM.RetroInstance.AgoraAltoPath;
            this.imagesDirectory = retroVM.RetroInstance.FullImagesPath;
            if (Directory.Exists(this.altoDirectory))
            {
                imagesList = new List<String>(System.IO.Directory.GetFiles(imagesDirectory));
                altoList = new List<String>(System.IO.Directory.GetFiles(altoDirectory, "*.xml"));
                if (altoList.Count != 0)
                {
                    // Initialize owned windows
                    this.amdw = new AddModelDataWindow();
                    this.amdw.SetProvider();
                    this.amdw.Visibility = Visibility.Hidden;

                    // Update the resources
                    currentAltoIndex = 0;
                    this.UpdateWindow();
                }
            }
            else
            {
                // Disable controls
                this.pagePreviousButton.IsEnabled = false;
                this.pageNextButton.IsEnabled = false;
                this.PageWindow_zoomSlider.IsEnabled = false;
                this.Page_comboBox.IsEnabled = false;
            }
        }

        #endregion


        #region Update methods

        /// <summary>
        /// Update the window display
        /// </summary>
        private void UpdateWindow()
        {
            // Display a splashScreen during Canvas refresh
            SplashScreen splashScreen = new SplashScreen("resources/loading.png");
            splashScreen.Show(true);

            // Update the resources
            this.UpdateResources();

            // Update the controls
            this.UpdateControls();

            // Reset EoCWindow if it was visible
            if (this.eocw.IsVisible)
                eocw.ResetEoCWindow();
        }


        /// <summary>
        /// Update the resources of the window
        /// </summary>
        private void UpdateResources()
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(this.altoList[this.currentAltoIndex]);

            if (doc.DocumentElement.Name.CompareTo("alto:alto") == 0)
            {
                // Get the original name of the current image
                // WARNING: Assume that RenameOriginalImage is the first processing step!
                XmlNodeList processingStepSettingsXMLElement = doc.GetElementsByTagName("alto:processingStepSettings");
                XmlNode node = processingStepSettingsXMLElement.Item(0);

                // Set the current page originale filename (for FontModel Attributes exportation)
                String tmpString = node.InnerText;
                tmpString = tmpString.Substring(0, tmpString.IndexOf(','));
                this.currentImageSourceOriginalFilename = System.IO.Path.GetFileName(tmpString);

                // Get the EoC List
                this.GetTextEoCList(ref doc);
                this.GetIllustrationEoCList(ref doc);

                // Load the image
                Bitmap bitmap = (Bitmap)Bitmap.FromFile(this.imagesList[this.currentAltoIndex]);
                if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    AForge.Imaging.Filters.GrayscaleToRGB filter = new AForge.Imaging.Filters.GrayscaleToRGB();
                    bitmap = filter.Apply(bitmap);
                }

                // Draw the Eoc (Text & Illustration)
                this.DrawEoCBoundingBoxes(ref bitmap);

                // Set the Image source
                this.Page_Image.Source = RetroUtil.Image2DisplayTool.Bitmap2BitmapSource(ref bitmap);
                bitmap.Dispose();
            }
            else
            {
                MessageBox.Show("The selected xml file is not an alto file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Update the visiblity of the window's controls
        /// </summary>
        private void UpdateControls()
        {
            this.pageLabel.Content = this.altoList.ElementAt(this.currentAltoIndex);
            this.pagePreviousButton.IsEnabled = (this.currentAltoIndex == 0) ? false : true;
            this.pageNextButton.IsEnabled = (this.currentAltoIndex == this.altoList.Count - 1) ? false : true;
        }


        /// <summary>
        /// Get the list of the Text EoC of this image in the associated alto file
        /// </summary>
        /// <param name="doc">Alto xml</param>
        private void GetTextEoCList(ref System.Xml.XmlDocument doc)
        {
            // Clear the list of Text EoC
            this.textEoCList.Clear();

            // Set the tagname regarding the granularity
            String tagname = "alto:";
            switch (this.eocGranularity)
            {
                case 0:
                    tagname += "TextBlock";
                    break;
                case 1:
                    tagname += "TextLine";
                    break;
                case 2:
                    tagname += "String";
                    break;
                default:
                    tagname = "";
                    break;
            }
    
            if (tagname.CompareTo("") != 0)
            {
                XmlNodeList xmlElementList = doc.GetElementsByTagName(tagname);
                foreach (XmlNode node in xmlElementList)
                {
                    // Get Element attributes
                    EoCAttributes elementAttributes = new EoCAttributes();
                    elementAttributes.ID = node.Attributes["ID"].Value;
                    elementAttributes.HPOS = Convert.ToInt32(node.Attributes["HPOS"].Value);
                    elementAttributes.VPOS = Convert.ToInt32(node.Attributes["VPOS"].Value);
                    elementAttributes.WIDTH = Convert.ToInt32(node.Attributes["WIDTH"].Value);
                    elementAttributes.HEIGHT = Convert.ToInt32(node.Attributes["HEIGHT"].Value);
                    elementAttributes.CONTENT = this.getAltoContent(node);

                    // Add to the list
                    this.textEoCList.Add(elementAttributes);
                }
            }
        }


        /// <summary>
        /// Get the list of the Illustration EoC of this image in the associated alto file
        /// (regarding the current selected granularity)
        /// </summary>
        /// <param name="doc">Alto xml</param>
        private void GetIllustrationEoCList(ref System.Xml.XmlDocument doc)
        {
            // Clear the list of Illustration EoC
            this.illustrationsEoCList.Clear();

            XmlNodeList xmlElementList = doc.GetElementsByTagName("alto:Illustration");
            foreach (XmlNode element in xmlElementList)
            {
                // Get Element attributes
                EoCAttributes elementAttributes = new EoCAttributes();
                elementAttributes.ID = element.Attributes["ID"].Value;
                elementAttributes.HPOS = Convert.ToInt32(element.Attributes["HPOS"].Value);
                elementAttributes.VPOS = Convert.ToInt32(element.Attributes["VPOS"].Value);
                elementAttributes.WIDTH = Convert.ToInt32(element.Attributes["WIDTH"].Value);
                elementAttributes.HEIGHT = Convert.ToInt32(element.Attributes["HEIGHT"].Value);
                elementAttributes.CONTENT = "";

                // Add to the list
                this.illustrationsEoCList.Add(elementAttributes);
            }
        }


        /// <summary>
        /// Draw the computed TexBlock bounding boxes in the images
        /// </summary>
        /// <param name="bitmap">List of rectangles to draw in the image</param>
        private void DrawEoCBoundingBoxes(ref Bitmap bitmap)
        {
            // Lock bits
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
            System.Drawing.Imaging.BitmapData bitmapData =
                bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bitmap.PixelFormat);

            // Draw Illustration bounding boxes
            foreach (EoCAttributes eoc in this.illustrationsEoCList)
            {
                // Workaround for a thick rectangle: draw 2 rectangles ...
                AForge.Imaging.Drawing.Rectangle(bitmapData, new System.Drawing.Rectangle(eoc.HPOS - 1, eoc.VPOS - 1, eoc.WIDTH + 2, eoc.HEIGHT + 2), System.Drawing.Color.Red);
                AForge.Imaging.Drawing.Rectangle(bitmapData, new System.Drawing.Rectangle(eoc.HPOS, eoc.VPOS, eoc.WIDTH, eoc.HEIGHT), System.Drawing.Color.Red);
            }

            // Draw Text bounding boxes
            foreach (EoCAttributes eoc in this.textEoCList)
            {
                // Workaround for a thick rectangle: draw 2 rectangles ...
                AForge.Imaging.Drawing.Rectangle(bitmapData, new System.Drawing.Rectangle(eoc.HPOS - 1, eoc.VPOS - 1, eoc.WIDTH + 2, eoc.HEIGHT + 2), System.Drawing.Color.Red);
                AForge.Imaging.Drawing.Rectangle(bitmapData, new System.Drawing.Rectangle(eoc.HPOS, eoc.VPOS, eoc.WIDTH, eoc.HEIGHT), System.Drawing.Color.Red);
            }

            // Unlock bits
            bitmap.UnlockBits(bitmapData);
        }

        #endregion


        #region Events handler methods

        /// <summary>
        /// Handler for a click on the open button
        /// Allow user to open an precise xml in the alto directory of this project
        /// </summary>
        private void PageWindow_Button_Open_Click(object sender, RoutedEventArgs e)
        {
            // Open Show Image Dialog
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = this.altoDirectory;
            ofd.Filter = "Alto Files|*.xml|All Files|*.*";
            bool result = (bool)ofd.ShowDialog();
            bool correctResult = false;

            // Workaround to impose the user to stay on the alto directory of this project
            while (result && !correctResult)
            {
                if ((System.IO.Path.GetDirectoryName(ofd.FileName)).CompareTo(this.altoDirectory) != 0)
                {
                    MessageBox.Show("Please select an ALTO XML file in the alto directory of this project.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    result = (bool)ofd.ShowDialog();
                }
                else
                {
                    correctResult = true;
                }
            }

            if (result && (ofd.SafeFileName.Length != 0))
            {
                altoDirectory = System.IO.Path.GetDirectoryName(ofd.FileName);
                altoList = new List<String>(System.IO.Directory.GetFiles(altoDirectory));
                currentAltoIndex = altoList.IndexOf(ofd.FileName);

                // Update the window
                this.UpdateWindow();
            }
        }


        /// <summary>
        /// Handler for combo box selection change
        /// </summary>
        public void Page_EoC_Granularity_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {

            if ( this.eocGranularity != this.Page_comboBox.Items.IndexOf(this.Page_comboBox.SelectedItem) )
            {
                // Get the selected granularity
                this.eocGranularity = this.Page_comboBox.Items.IndexOf(this.Page_comboBox.SelectedItem);

                // Display a splashScreen during Canvas refresh
                SplashScreen splashScreen = new SplashScreen("resources/loading.png");
                if (this.altoDirectory != null)
                    splashScreen.Show(true);

                // Update the window
                this.UpdateWindow();

                // Reset EoCWindow if it was visible
                if (this.eocw.IsVisible)
                    eocw.ResetEoCWindow();
            }
        }


        /// <summary>
        /// Handler for the mouse wheel in the image  
        /// Increase/Decrease zoom
        /// </summary>
        void Page_EoC_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((this.PageWindow_zoomSlider.IsEnabled) && (Keyboard.Modifiers == ModifierKeys.Control))
            {
                if (e.Delta > 0) // UP
                    this.PageWindow_zoomSlider.Value = (this.PageWindow_zoomSlider.Value < 5) ? this.PageWindow_zoomSlider.Value + 0.5 : this.PageWindow_zoomSlider.Value;
                else // DOWN
                    this.PageWindow_zoomSlider.Value = (this.PageWindow_zoomSlider.Value > 0.5) ? this.PageWindow_zoomSlider.Value - 0.5 : this.PageWindow_zoomSlider.Value;
            }
        }


        /// <summary>
        /// Handler for a left clik on a EoC in the image
        /// Display ContextMenu of an EoC
        /// </summary>
        private void Page_EoC_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Reset the ContextMenu
            this.Page_Image.ContextMenu = null;

            // Get the position of the cursor
            System.Windows.Point p = e.GetPosition(sender as UIElement);

            // Search the selected EoC in the illustration list
            bool found = this.FindSelectedEoC(p, true);
            if (found)
                this.Page_Image.ContextMenu = (ContextMenu)this.LayoutRoot.FindResource("IllustrationtEoCContextMenu");
            else
            {
                // Search the selected EoC in the illustration list
                found = this.FindSelectedEoC(p, false);
                if (found)
                {
                    this.Page_Image.ContextMenu = (ContextMenu)this.LayoutRoot.FindResource("TextEoCContextMenu");
                    // Enabled the SaveAsFontModel only if String granularity is selected
                    if ((this.Page_comboBoxItem_3 != null) && (this.Page_Image.ContextMenu != null))
                        ((MenuItem)this.Page_Image.ContextMenu.Items[4]).IsEnabled = this.Page_comboBoxItem_3.IsSelected;
                }
            }

            if (found)
            {
                // Highlight the selected EoC
                this.HighLightSelectedEoC();

                // Release the eocBitmap if exist
                if (this.currentEoCBitmap != null)
                    this.currentEoCBitmap.Dispose();

                // Get the EoC Ilage by cropping the global image
                this.currentEoCBitmap = (Bitmap)Bitmap.FromFile(this.imagesList[this.currentAltoIndex]);
                System.Drawing.Rectangle roi = new System.Drawing.Rectangle(this.currentEoCAttributes.HPOS, this.currentEoCAttributes.VPOS,
                    this.currentEoCAttributes.WIDTH, this.currentEoCAttributes.HEIGHT);
                AForge.Imaging.Filters.Crop filterC = new AForge.Imaging.Filters.Crop(roi);
                this.currentEoCBitmap = filterC.Apply(this.currentEoCBitmap);
            }
        }


        /// <summary>
        /// Find the selected EoC in the list of EoC
        /// </summary>
        /// <param name="p">position of the click</param>
        /// <param name="illustration">to select the EoC list to search in (Text or Illustration)</param>
        private bool FindSelectedEoC(System.Windows.Point p, bool illustration)
        {
            // Assign the list to search in
            List<EoCAttributes> eocList = null;
            if (illustration)
                eocList = this.illustrationsEoCList;
            else
                eocList = this.textEoCList;

            // Search
            foreach (EoCAttributes eoc in eocList)
            {
                if (p.Y < eoc.VPOS)
                    continue;

                if (p.Y < eoc.VPOS + eoc.HEIGHT)
                {
                    if (p.X < eoc.HPOS)
                        continue;

                    if (p.X < eoc.HPOS + eoc.WIDTH)
                    {
                        this.currentEoCAttributes = eoc;
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Highlight the selected EoC
        /// Not implemented yet
        /// </summary>
        private void HighLightSelectedEoC()
        {
            // TODO
        }


        /// <summary>
        /// Handler for the previous page button  
        /// </summary>
        private void Click_Previous_Page(object sender, RoutedEventArgs e)
        {
            if (this.altoList != null)
            {
                if (this.currentAltoIndex > 0)
                {
                    // Update regarding previous alto files in the directory
                    this.currentAltoIndex--;
                    this.UpdateWindow();
                }
            }
        }


        /// <summary>
        /// Handler for the next page button
        /// </summary>
        private void Click_Next_Page(object sender, RoutedEventArgs e)
        {
            if (this.altoList != null)
            {
                if (this.currentAltoIndex < this.altoList.Count - 1)
                {
                    // Update regarding next alto files in the directory
                    this.currentAltoIndex++;
                    this.UpdateWindow();
                }
            }
        }

        #endregion


        #region ContextMenu Events handler method

        /// <summary>
        /// Handler for ContextMenu/Open EoC Image MenuItem Click
        /// </summary>
        private void OpenEoCMetaData_Click(object sender, RoutedEventArgs e)
        {
            // Create a new EocWindow if needed
            if (this.eocw.HasBeenClosed)
                this.eocw = new EoCWindow();

            // Set the EoC Attributes
            this.eocw.SetEoCAttributes(this.currentEoCAttributes.ID, RetroUtil.Image2DisplayTool.Bitmap2BitmapSource(ref this.currentEoCBitmap), this.currentEoCAttributes.WIDTH + " x " + this.currentEoCAttributes.HEIGHT);

            // If Text Eoc, set the transcription
            if (this.currentEoCAttributes.CONTENT.CompareTo("") != 0)
                this.eocw.SetEoCTranscription(this.currentEoCAttributes.CONTENT);

            // Display the EoC Window
            this.eocw.Owner = this;
            this.eocw.Show();
        }


        /// <summary>
        /// Handler for ContextMenu/Copy EoC Image In Clipboard MenuItem Click
        /// </summary>
        private void CopyEoCImageInClipboard_Click(object sender, RoutedEventArgs e)
        {

            // Copy the transcription in the clipboard
            Clipboard.SetData(DataFormats.Bitmap, (Object)this.currentEoCBitmap);
        }


        /// <summary>
        /// Handler for ContextMenu/Save EoC Image MenuItem Click
        /// </summary>
        private void SaveEoCImage_Click(object sender, RoutedEventArgs e)
        {
            // Display SaveDialog
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "*.png";
            sfd.Filter = "Images (.png)|*.png"; // Filter files by extension
            sfd.ShowDialog();

            if (sfd.SafeFileName.Length != 0)
            {
                // Copy the image
                this.currentEoCBitmap.Save(sfd.FileName);

                // Display Notification
                MessageBox.Show("The image has been saved.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        /// <summary>
        /// Handler for ContextMenu/Copy EoC Transcription MenuItem Click
        /// </summary>
        private void CopyTextEoCTranscriptionInClipboard_Click(object sender, RoutedEventArgs e)
        {
            // Copy the transcription in the clipboard
            Clipboard.SetData(DataFormats.Text, (Object)this.currentEoCAttributes.CONTENT);
        }


        /// <summary>
        /// Handler for ContextMenu/Save as FontModel MenuItem Click
        /// </summary>
        private void SaveAsFontModel_Click(object sender, RoutedEventArgs e)
        {
            // First time display MetadataModelWindow
            if (this.displayMetadata)
            {
                this.modelMetadata = new ModelMetaData();

                // Open dialog to get model book metadata
                AddModelMetadataWindow ammw = new AddModelMetadataWindow();
                ammw.Owner = this;
                ammw.ShowDialog();

                // Get model metadata
                this.modelMetadata = ammw.GetModelMetaData();

                this.displayMetadata = false;
            }

            // Display the ModelDataWindow
            if (this.amdw.HasBeenClosed)
            {
                this.amdw = new AddModelDataWindow();
                this.amdw.SetProvider();
            }

            amdw.SetImage(ref this.currentEoCBitmap, this.currentImageSourceOriginalFilename,
                "" + this.currentEoCAttributes.WIDTH, "" + this.currentEoCAttributes.HEIGHT, "" + this.currentEoCAttributes.HPOS, "" + this.currentEoCAttributes.VPOS);
            amdw.ColorizeBlob = false;
            amdw.Owner = this;
            amdw.ShowDialog();
        }


        #endregion


        #region EoC Content method

        /// <summary>
        /// Get Alto content regarding selected EoC granularity
        /// </summary>
        private String getAltoContent(System.Xml.XmlNode node)
        {
            String altoContent = "";

            switch (this.eocGranularity)
            {
                case 0:     // TextBlock
                    altoContent = this.getTextBlockTagAltoContent(node);
                    break;
                case 1:     // TextLine
                    altoContent = this.GetTextLineTagAltoContent(node);
                    break;
                case 2:     // String
                    altoContent = node.Attributes["CONTENT"].Value;
                    break;
                default:
                    break;
            }

            return altoContent;
        }


        /// <summary>
        /// Get Alto Content of a TextLine tag
        /// </summary>
        /// <param name="node"> The XmlNode corresponding to the current alto:TextLine</param>
        /// <returns>Transcription of the TextLine (with space between words)</returns>
        private String GetTextLineTagAltoContent(XmlNode node)
        {
            String result = "";

            // Get the String chils
            XmlNodeList stringList = node.ChildNodes;
            int currentWord = 0;
            foreach (XmlNode _string in stringList)
            {
                if (_string.Name.CompareTo("alto:String") != 0)
                    continue;

                // Get the numero of the word in the line
                String _stringID = _string.Attributes["ID"].Value;
                _stringID = _stringID.Substring(0, _stringID.LastIndexOf('.'));
                _stringID = _stringID.Substring(_stringID.LastIndexOf('.') + 1);
                if (Convert.ToInt32(_stringID) != currentWord)
                {
                    // Add space between words
                    result += " ";
                    currentWord++;
                }

                result += _string.Attributes["CONTENT"].Value;
            }
            return result;
        }


        /// <summary>
        /// Get Alto Content of a TextBlock tag
        /// </summary>
        /// <param name="node">The XmlNode corresponding to the current alto:TextBlock</param>
        /// <returns>Transcription of the TextBlock (with space between words, linebreak between lines)</returns>
        private String getTextBlockTagAltoContent(XmlNode node)
        { 
            String result = "";

            // Get the TextLine child
            XmlNodeList textlineList = node.ChildNodes;
            foreach (XmlNode textline in textlineList)
            {
                if (textline.Name.CompareTo("alto:TextLine") != 0)
                    continue;

                XmlNodeList stringList = textline.ChildNodes;
                int currentWord = 0;
                foreach (XmlNode _string in stringList)
                {
                    if (_string.Name.CompareTo("alto:String") != 0)
                        continue;

                    // Get the numero of the word in the line
                    String _stringID = _string.Attributes["ID"].Value;
                    _stringID = _stringID.Substring(0, _stringID.LastIndexOf('.'));
                    _stringID = _stringID.Substring(_stringID.LastIndexOf('.') + 1);
                    if (Convert.ToInt32(_stringID) != currentWord)
                    {
                        // Add space between words
                        result += " ";
                        currentWord++;
                    }

                    result += _string.Attributes["CONTENT"].Value;
                }
                result += "\n";
            }
            return result;
        }

        #endregion


        /// <summary>
        /// Model metadata getter for the AddModelDataWindow
        /// </summary>
        public ModelMetaData GetModelMetaData()
        {
            return this.modelMetadata;
        }
    }
}
