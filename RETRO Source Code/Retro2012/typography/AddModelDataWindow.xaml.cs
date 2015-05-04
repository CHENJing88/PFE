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
using Microsoft.Win32;
using AForge.Imaging;
using System.Drawing;
using AForge.Imaging.Filters;
using System.IO;
using System.Xml;
using System.Windows.Forms; //TODO: For FolderBrowserDialog, Find a libray that will allow to avoid use of WinForms!

using RetroGUI.util;
using RetroGUI.visualisation;
using RetroUtil;

namespace RetroGUI.typography
{
    /// <summary>
    /// Display a form with character-related datas to fill during the creation of a font model 
    /// </summary>
    public partial class AddModelDataWindow : Window
    {
        #region Attributes

        // Grayscale mage
        private Bitmap modelBitmap;
        private System.Drawing.Rectangle roi;
        private String imageSourceName;
        private String _width;
        private String _height;
        private String _hpos;
        private String _vpos;
        private String _resolution;
        private XmlDataProvider provider;

        #endregion


        private bool _ColorizeBlob;
        /// <summary>
        /// For drawing green bouding boxes
        /// </summary>
        public bool ColorizeBlob
        {
            get { return _ColorizeBlob; }
            set { _ColorizeBlob = value; }
        }


        private bool _HasBeenClosed;
        /// <summary>
        /// To avoid multiple instince of ClusterWindows ans handle it's closing
        /// </summary>
        public bool HasBeenClosed
        {
            get { return _HasBeenClosed; }
            set { _HasBeenClosed = value; }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public AddModelDataWindow()
        {
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.HasBeenClosed = false;

            this.createModelButton.IsEnabled = false;
            this.createModelButton.ToolTip = "Give an output directory";
            
        }


        /// <summary>
        /// Set XML Provider for typographic resources
        /// </summary>
        public void SetProvider()
        {
            this.provider = (XmlDataProvider)this.LayoutRoot.FindResource("TypoFeed");
            String typoResourcesPath = "/resources/Typographic_Resources.xml";
            this.provider.Source = new Uri(typoResourcesPath , UriKind.RelativeOrAbsolute);
            this.provider.Refresh();
        }


        /// <summary>
        /// Set Image preview of the selected EoC
        /// </summary>
        public void SetImage(String imagepath, System.Drawing.Rectangle roi)
        {
            // Open the image
            this.imageSourceName = System.IO.Path.GetFileName(imagepath);
            Bitmap bitmap = new Bitmap(imagepath);

            // Update attributes
            this.roi = roi;
            this._width = "" + roi.Width;
            this._height = "" + roi.Height;
            this._hpos = "" + roi.X;
            this._vpos = "" + roi.Y;
            this._resolution = "" + bitmap.HorizontalResolution;  // We suppose that resolution will be the same for the 2 dimensions

            // Crop the image
            if (modelBitmap != null)
                modelBitmap.Dispose();
            Crop filter = new Crop(roi);
            modelBitmap = filter.Apply(bitmap);

            // Display the image
            this.modelImage.Source = RetroUtil.Image2DisplayTool.Bitmap2BitmapSource(ref modelBitmap);
                
            // Free the image
            bitmap.Dispose();
        }


        /// <summary>
        /// Set Image preview of the selected EoC
        /// Used in PageWindow
        /// </summary>
        public void SetImage(ImageSource thumbnailSource, String imageSourcePath, String width, String height, String hpos, String vpos)
        {
            // Assign the page image source
            this._width = width;
            this._height = height;
            this._hpos = hpos;
            this._vpos = vpos;
            this.imageSourceName = System.IO.Path.GetFileName(imageSourcePath);

            // Create the modelBitmap
            Uri imagepath = new Uri(thumbnailSource.ToString());
            this.modelBitmap = (Bitmap)Bitmap.FromFile(imagepath.LocalPath);

            // Set resolution atributes
            this._resolution = "" + this.modelBitmap.HorizontalResolution;

            // Display the image
            this.modelImage.Source = thumbnailSource;
        }


        /// <summary>
        /// Set Image preview of the selected EoC
        /// Used in PageWindow2
        /// </summary>
        public void SetImage(ref Bitmap thumbnailBitmap, String imageSourcePath, String width, String height, String hpos, String vpos)
        {
            // Assign the page image source
            this._width = width;
            this._height = height;
            this._hpos = hpos;
            this._vpos = vpos;
            this.imageSourceName = System.IO.Path.GetFileName(imageSourcePath);

            // Create the modelBitmap
            if (this.modelBitmap != null)
                this.modelBitmap.Dispose();
            this.modelBitmap = thumbnailBitmap;

            // Set resolution atributes
            this._resolution = "" + this.modelBitmap.HorizontalResolution;

            // Display the image
            this.modelImage.Source = RetroUtil.Image2DisplayTool.Bitmap2BitmapSource(ref thumbnailBitmap); ;
        }


        /// <summary>
        /// Handler for output dir textbox
        /// </summary>
        private void AddModelData_TextBox_OutputDir_TextChange(object sender, TextChangedEventArgs e)
        {
            if ((this.outputDirTextBox != null) && !(this.outputDirTextBox.Text == ""))
            {
                // Disable the create button
                this.createModelButton.IsEnabled = true;
                this.createModelButton.ToolTip = null;
            }
            else
            {
                // Disable the create button
                this.createModelButton.IsEnabled = false;
                this.createModelButton.ToolTip = "Give an output directory";
            }

        }


        /// <summary>
        /// Handler for a click on the select output directory button
        /// </summary>
        private void AddModelData_Button_Open_Click(object sender, RoutedEventArgs e)
        {
            // Open Show Image Dialog
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath.Length != 0)
            {
                // Update textbox
                this.outputDirTextBox.Text = fbd.SelectedPath;
            }

        }


        /// <summary>
        /// Handler for a click on the create model button
        /// </summary>
        private void AddModelData_Button_CreateModel_Click(object sender, RoutedEventArgs e)
        {
            // Export image, B&W image and XML output
            if (this.CreateModel() == 0)
            {
                // Notify the user
                System.Windows.MessageBox.Show("A new model has been created.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);

                // Update the color of the current blob for the caller
                this.ColorizeBlob = true;

                // Reset some of the textboxes values
                this.transcriptionTextBox.Text = "";
                this.unicodeTextBox.Text = "";
                this.smallCapCheckBox.IsChecked = false;

                // Hide the windows!
                this.Visibility = Visibility.Hidden;
            }

        }


        /// <summary>
        /// Creation of the model output resources:
        ///  1/ Grayscale image
        ///  2/ Binarized image
        ///  3/ XML description file
        /// </summary>
        /// <returns>0 if success, 1 if failure</returns>
        private int CreateModel()
        {
            // Get the output directory
            String outputPath = this.outputDirTextBox.Text;
            if ( (outputPath!="") && (!Directory.Exists(outputPath)) )
                Directory.CreateDirectory(outputPath);

            // Build normalized filename
            String filename = this.BuildFilename(outputPath);

            // Save grayscale image
            try
            {
                this.modelBitmap.Save(filename + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString()+"ERROR: Font Model Creation has failed. \nTranscription should not contain the followig characters: \\ / : * ? \" < > |");
                return 1;
            }


            // Create B&W image
            Bitmap modelBitmapBW = this.modelBitmap;
            if (this.modelBitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                Grayscale filterG = new Grayscale(0.2125, 0.7154, 0.0721);
                modelBitmapBW = filterG.Apply(this.modelBitmap);
            }
            SISThreshold filterB = new SISThreshold();  
            modelBitmapBW = filterB.Apply(modelBitmapBW);

            // Save B&W image
            modelBitmapBW.Save(filename + "_bw.png", System.Drawing.Imaging.ImageFormat.Png);
            modelBitmapBW.Dispose();

            // Get metadata
            ModelMetaData metadata = new ModelMetaData();
            metadata.Clean();
            if (this.Owner is AddModelWindow)
                metadata = ((AddModelWindow)this.Owner).GetModelMetaData();
            else if (this.Owner is PageWindow)
                metadata = ((PageWindow)this.Owner).GetModelMetaData();

            // Save XML description file
            this.ExportToXML(filename, metadata);

            return 0;

        }


        /// <summary>
        /// Build normalized filename for exportation
        /// </summary>
        private String BuildFilename(String outputPath)
        {
                        
            String filename, label, type, alphabet, family, subfamily, bodyheight;

            // Get Label
            label = this.transcriptionTextBox.Text;

            // Get Type
            type = "XX";
            if (this.typeComboBox.SelectedItem != null)
                type = ((XmlElement)this.typeComboBox.SelectedItem).Attributes["Code"].Value;

            // Get Alphabet
            alphabet = "X";
            if (this.alphabetComboBox.SelectedItem != null)
                alphabet = ((XmlElement)this.alphabetComboBox.SelectedItem).Attributes["Code"].Value;

            // Get Family
            family = "X";
            if (this.familyComboBox.SelectedItem != null)
                family = ((XmlElement)this.familyComboBox.SelectedItem).Attributes["Code"].Value;

            // Get Subfamily
            subfamily = "XX";
            if (this.subfamilyComboBox.SelectedItem != null)
                subfamily = ((XmlElement)this.subfamilyComboBox.SelectedItem).Attributes["Code"].Value;

            // Get BodyHeight
            bodyheight = "XXX";
            if (this.bodyHeightComboBox.SelectedItem != null)
                bodyheight = ((XmlElement)this.bodyHeightComboBox.SelectedItem).Attributes["Code"].Value;

            // Selection of a file name
            int cpt = 0;
            do
            {
                cpt++;
                filename = outputPath + @"\" + label + cpt + "_" + type + "_" + alphabet + "_" + family + "_" + subfamily + "_" + bodyheight;
            }
            while (File.Exists(filename + ".xml"));

            return filename;
        }
 

        /// <summary>
        /// XML ouput export 
        /// </summary>
        private void ExportToXML(String outputPath, ModelMetaData metadata)
        {

            // Create FileStream
            FileStream fs = new FileStream(outputPath + ".xml", FileMode.Create);

            // Create XML writer
            XmlTextWriter xmlOut = new XmlTextWriter(fs, Encoding.Unicode);

            // use indenting for readability
            xmlOut.Formatting = Formatting.Indented;

            // start document
            xmlOut.WriteStartDocument();
            xmlOut.WriteComment("RETRO Model file");

            // main node
            xmlOut.WriteStartElement("Model");

            // Transcription node
            xmlOut.WriteStartElement("Metadata");
                // Book metadata node
                xmlOut.WriteStartElement("Publication");
                xmlOut.WriteAttributeString("Author", metadata.PublicationAuthor);
                xmlOut.WriteAttributeString("Title", metadata.PublicationTitle);
                xmlOut.WriteAttributeString("Place", metadata.PublicationPublicationSite);
                xmlOut.WriteAttributeString("PrinterOrPublisher", metadata.PublicationPrinter);
                xmlOut.WriteAttributeString("Date", metadata.PublicationDate);
                xmlOut.WriteAttributeString("Format", metadata.PublicationFormat);
                xmlOut.WriteEndElement();
                // Copy metadata node
                xmlOut.WriteStartElement("Copy");
                xmlOut.WriteAttributeString("Library", metadata.CopyLibrary);
                xmlOut.WriteAttributeString("CallNumber", metadata.CopyPressmark);
                xmlOut.WriteAttributeString("Digitization", metadata.CopyDigitization);
                xmlOut.WriteAttributeString("Copyright", metadata.CopyLicense);
                xmlOut.WriteAttributeString("CataloguerName", metadata.CopyCataloguer);
                xmlOut.WriteEndElement();
            xmlOut.WriteEndElement();

            // Transcription node
            xmlOut.WriteStartElement("Transcription");
            xmlOut.WriteAttributeString("Character", this.transcriptionTextBox.Text);
            xmlOut.WriteAttributeString("Unicode", this.unicodeTextBox.Text);
            xmlOut.WriteEndElement();

            // Image node
            xmlOut.WriteStartElement("Image");
            xmlOut.WriteAttributeString("Filename", "" + this.imageSourceName);
            int pos = this.imageSourceName.LastIndexOf('_');
            if (pos > 0)
            {
                xmlOut.WriteAttributeString("Folder", "" + this.imageSourceName.Substring(0, pos));
                xmlOut.WriteAttributeString("Page", "" + (System.IO.Path.GetFileNameWithoutExtension(this.imageSourceName)).Substring(this.imageSourceName.LastIndexOf('_') + 1));
            }
            else
            {
                xmlOut.WriteAttributeString("Folder", "");
                xmlOut.WriteAttributeString("Page", "");
            }
            xmlOut.WriteAttributeString("Resolution", "" + this._resolution);
            xmlOut.WriteEndElement();

            // Image node
            xmlOut.WriteStartElement("Thumbnail");
            xmlOut.WriteAttributeString("Name", "" + System.IO.Path.GetFileName(outputPath) + ".png");
            xmlOut.WriteAttributeString("Width", "" + this._width);
            xmlOut.WriteAttributeString("Height", "" + this._height);
            xmlOut.WriteAttributeString("PositionX", "" + this._hpos);
            xmlOut.WriteAttributeString("PositionY", "" + this._vpos);
            xmlOut.WriteEndElement();

            // Typography node
            String isSmallCap = "false";
            String type = "XX";
            String alphabet = "X";
            String family = "X";
            String subfamily = "XX";
            String bodyheight = "XXX";

            if (this.smallCapCheckBox != null)
                isSmallCap = ((bool)this.smallCapCheckBox.IsChecked).ToString();
            if (this.typeComboBox.SelectedItem != null)
                type = ((XmlElement)this.typeComboBox.SelectedItem).Attributes["Name"].Value;
            if (this.alphabetComboBox.SelectedItem != null)
                alphabet = ((XmlElement)this.alphabetComboBox.SelectedItem).Attributes["Name"].Value;
            if (this.familyComboBox.SelectedItem != null)
                family = ((XmlElement)this.familyComboBox.SelectedItem).Attributes["Name"].Value;
            if (this.subfamilyComboBox.SelectedItem != null)
                subfamily = ((XmlElement)this.subfamilyComboBox.SelectedItem).Attributes["Name"].Value;
            if (this.bodyHeightComboBox.SelectedItem != null)
                bodyheight = ((XmlElement)this.bodyHeightComboBox.SelectedItem).Attributes["EnglishName"].Value;

            xmlOut.WriteStartElement("Typography");
            xmlOut.WriteAttributeString("IsSmallCap", isSmallCap);
            xmlOut.WriteAttributeString("Type", type);
            xmlOut.WriteAttributeString("Alphabet", alphabet);
            xmlOut.WriteAttributeString("Family", family);
            xmlOut.WriteAttributeString("SubFamily", subfamily);
            xmlOut.WriteAttributeString("BodyHeight", bodyheight);
            xmlOut.WriteAttributeString("Thickness", this.thicknessTextBox.Text);
            xmlOut.WriteEndElement();

            // Description
            xmlOut.WriteStartElement("Description");
            xmlOut.WriteAttributeString("References", this.referenceTextBox.Text);
            xmlOut.WriteAttributeString("Engraver", this.graveurTextBox.Text);
            xmlOut.WriteAttributeString("Comments", this.commentTextBlock.Text);
            xmlOut.WriteEndElement();

            // close file
            xmlOut.Close();
            

        }


        /// <summary>
        /// Default XML ouput export 
        /// </summary>
        /// <param name="outputFilepath">Path of the xml file to be created</param>
        /// <param name="label">Label of the model</param>
        public static void DefaultCreateModel(String outputFilepath, String label)
        {
            // Create FileStream
            FileStream fs = new FileStream(outputFilepath, FileMode.Create);

            // Create XML writer
            XmlTextWriter xmlOut = new XmlTextWriter(fs, Encoding.Unicode);

            // use indenting for readability
            xmlOut.Formatting = Formatting.Indented;

            // start document
            xmlOut.WriteStartDocument();
            xmlOut.WriteComment("RETRO Model file");

            // main node
            xmlOut.WriteStartElement("Model");

            // Transcription node
            xmlOut.WriteStartElement("Metadata");
            // Book metadata node
            xmlOut.WriteStartElement("Publication");
            xmlOut.WriteAttributeString("Author", "");
            xmlOut.WriteAttributeString("Title", "");
            xmlOut.WriteAttributeString("Place", "");
            xmlOut.WriteAttributeString("PrinterOrPublisher", "");
            xmlOut.WriteAttributeString("Date", "");
            xmlOut.WriteAttributeString("Format", "");
            xmlOut.WriteEndElement();
            // Copy metadata node
            xmlOut.WriteStartElement("Copy");
            xmlOut.WriteAttributeString("Library", "");
            xmlOut.WriteAttributeString("CallNumber", "");
            xmlOut.WriteAttributeString("Digitization", "");
            xmlOut.WriteAttributeString("Copyright", "");
            xmlOut.WriteAttributeString("CataloguerName", "");
            xmlOut.WriteEndElement();
            xmlOut.WriteEndElement();

            // Transcription node
            xmlOut.WriteStartElement("Transcription");
            xmlOut.WriteAttributeString("Character", label);
            xmlOut.WriteAttributeString("Unicode", "");
            xmlOut.WriteEndElement();

            // Image node
            xmlOut.WriteStartElement("Image");
            xmlOut.WriteAttributeString("Filename", "");
            xmlOut.WriteAttributeString("Folder", "");
            xmlOut.WriteAttributeString("Page", "");
            xmlOut.WriteAttributeString("Resolution", "");
            xmlOut.WriteEndElement();

            // Image node
            xmlOut.WriteStartElement("Thumbnail");
            xmlOut.WriteAttributeString("Name", "");
            xmlOut.WriteAttributeString("Width", "");
            xmlOut.WriteAttributeString("Height", "");
            xmlOut.WriteAttributeString("PositionX", "");
            xmlOut.WriteAttributeString("PositionY", "");
            xmlOut.WriteEndElement();

            // Typography node
            xmlOut.WriteStartElement("Typography");
            xmlOut.WriteAttributeString("IsSmallCap", "");
            xmlOut.WriteAttributeString("Type", "");
            xmlOut.WriteAttributeString("Alphabet", "");
            xmlOut.WriteAttributeString("Family", "");
            xmlOut.WriteAttributeString("SubFamily", "");
            xmlOut.WriteAttributeString("BodyHeight", "");
            xmlOut.WriteAttributeString("Thickness", "");
            xmlOut.WriteEndElement();

            // Description
            xmlOut.WriteStartElement("Description");
            xmlOut.WriteAttributeString("References", "");
            xmlOut.WriteAttributeString("Engraver", "");
            xmlOut.WriteAttributeString("Comments", "");
            xmlOut.WriteEndElement();

            // close file
            xmlOut.Close();
        }


        /// <summary>
        /// Override ClearClustersList event to set a flag in order manage the ClusterWindow Lifetime
        /// </summary>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            this.HasBeenClosed = true;
        }

    }
}
