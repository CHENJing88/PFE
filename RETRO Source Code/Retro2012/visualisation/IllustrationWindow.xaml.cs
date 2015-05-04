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

using System.IO;
using Retro.ViewModel;
using RetroGUI.typography;
using System.Drawing;
using RetroGUI.util;
using System.Xml;
using AForge.Imaging.Filters;
using RetroUtil;


namespace RetroGUI.visualisation
{
    /// <summary>
    /// Define Illustration Window
    /// </summary>
    public partial class IllustrationWindow : Window
    {

        #region Attributes

        private RetroViewModel retroVM = null;
        private List<String> imagesList = new List<String>();
        private List<String> altoList = new List<String>();
        private int currentImageIndex;
        private System.Windows.Media.ImageSourceConverter converter = new System.Windows.Media.ImageSourceConverter();

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="retroVM">Instance of the View Model</param>
        public IllustrationWindow(RetroViewModel retroVM)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Assign ModelView
            this.retroVM = retroVM;

            // Get the list of alto files corresponding to pages that contains illustration
            this.BuildList();

            // Set cursor to the first file
            currentImageIndex = 0;

            // Update window
            if (this.altoList.Count != 0)
                this.UpdateWindow();
        }


        /// <summary>
        /// Get the list of images that contains illustration
        /// Get the list of alto files corresponding to images that contains illustration
        /// </summary>
        private void BuildList()
        {
            // Get the images/ and alto/ directory path
            String imagesDir = this.retroVM.RetroInstance.FullImagesPath;
            String altoDir = this.retroVM.RetroInstance.AgoraAltoPath;
            String[] directoryList = new String[0];

            // Get list of subdirectory (correspond to each image where an illustration has been extracted)
            if (Directory.Exists(this.retroVM.RetroInstance.AgoraAltoPath + @"images_illustration"))
            {
                directoryList = Directory.GetDirectories(this.retroVM.RetroInstance.AgoraAltoPath + @"images_illustration");
            }
            
            // Fill the list of alto files corresponding to pages that contains illustration
            foreach (String directory in directoryList)
            {
                String dirname = System.IO.Path.GetFileName(directory);
                this.imagesList.Add(imagesDir + dirname + ".png");
                this.altoList.Add(altoDir + dirname + ".xml");
            }
        }


        /// <summary>
        /// Update window view regarding the current image index considered
        /// </summary>
        private void UpdateWindow()
        {
            // Get current image and alto files
            String altofilepath = this.altoList.ElementAt(this.currentImageIndex);
            String imagefilepath = this.imagesList.ElementAt(this.currentImageIndex);

            // Update the imagepath label
            this.IllustrationWindow_ImagenameLabel.Content = imagefilepath;

            // Get list of illustrations
            List<System.Drawing.Rectangle> illustrationsList = this.GetIllustrationBoundaryBoxes(altofilepath);

            // Open image
            Bitmap bitmap = (Bitmap)Bitmap.FromFile(imagefilepath);

            // Convert to color image if grayscale
            if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                GrayscaleToRGB filter = new GrayscaleToRGB();
                bitmap = filter.Apply(bitmap);
            }

            // Draw Blobs
            if (illustrationsList.Count != 0)
                this.DrawIllustrationBoundingBoxes(ref bitmap, ref illustrationsList);

            // Update View
            this.IllustrationWindow_Image.Source = RetroUtil.Image2DisplayTool.Bitmap2BitmapSource(ref bitmap);

            // Free memory
            bitmap.Dispose();

        }

        
        /// <summary>
        /// Get Illustration BoundingBoxes list
        /// </summary>
        private List<System.Drawing.Rectangle> GetIllustrationBoundaryBoxes(String altofilepath)
        {
            // Build the result list
            List<System.Drawing.Rectangle> illustrationList = new List<System.Drawing.Rectangle>();

            // Open the alto file
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(altofilepath);

            if (doc.DocumentElement.Name.CompareTo("alto:alto") == 0)
            {
                // Get the illustration tag
                XmlNodeList illustrationNodes = doc.GetElementsByTagName("alto:Illustration");

                foreach (XmlNode illustrationNode in illustrationNodes)
                {
                    System.Drawing.Rectangle illustration = new System.Drawing.Rectangle(
                        Convert.ToInt32(illustrationNode.Attributes["HPOS"].Value),
                        Convert.ToInt32(illustrationNode.Attributes["VPOS"].Value),
                        Convert.ToInt32(illustrationNode.Attributes["WIDTH"].Value),
                        Convert.ToInt32(illustrationNode.Attributes["HEIGHT"].Value)
                        );
                    illustrationList.Add(illustration);
                }

                // Get image original dimensions while xml is loaded
                XmlNodeList pageXMLElement = doc.GetElementsByTagName("alto:Page");
                XmlNode node = pageXMLElement.Item(0);
                this.Resources["originalWidth"] = Convert.ToDouble(node.Attributes["WIDTH"].Value);
                this.Resources["originalHeight"] = Convert.ToDouble(node.Attributes["HEIGHT"].Value);
            }

            return illustrationList;
        }


        /// <summary>
        /// Draw Illustration BoundingBoxes
        /// </summary>
        private void DrawIllustrationBoundingBoxes(ref Bitmap bitmap, ref List<System.Drawing.Rectangle> illustrationsList)
        {

                // Lock bits
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                System.Drawing.Imaging.BitmapData bitmapData =
                    bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bitmap.PixelFormat);

                // Draw bounding boxes
                foreach (System.Drawing.Rectangle illustration in illustrationsList)
                {
                    // Workaround for a thick rectangle: draw 2 rectangles ...
                    AForge.Imaging.Drawing.Rectangle(bitmapData, 
                        new System.Drawing.Rectangle(illustration.X - 1, illustration.Y - 1, illustration.Width + 2, illustration.Height + 2), 
                        System.Drawing.Color.Red);
                    AForge.Imaging.Drawing.Rectangle(bitmapData, illustration, System.Drawing.Color.Red);
                }

                // Unlock bits
                bitmap.UnlockBits(bitmapData);
        }



        #region Events Handler

        /// <summary>
        /// Handler for the mouse wheel in the image  
        /// Increase/Decrease zoom
        /// </summary>
        private void IllustrationWindow_Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((this.IllustrationWindow_zoomSlider.IsEnabled) && (Keyboard.Modifiers == ModifierKeys.Control))
            {
                if (e.Delta > 0) // UP
                    this.IllustrationWindow_zoomSlider.Value = (this.IllustrationWindow_zoomSlider.Value < 5) ? this.IllustrationWindow_zoomSlider.Value + 0.5 : this.IllustrationWindow_zoomSlider.Value;
                else // DOWN
                    this.IllustrationWindow_zoomSlider.Value = (this.IllustrationWindow_zoomSlider.Value > 0.5) ? this.IllustrationWindow_zoomSlider.Value - 0.5 : this.IllustrationWindow_zoomSlider.Value;
            }
        }


        /// <summary>
        /// Handler for the previous page button  
        /// </summary>
        private void IllustrationWindow_PreviousButton(object sender, RoutedEventArgs e)
        {
            if (this.altoList != null)
            {
                if (this.currentImageIndex > 0)
                {
                    // Update regarding previous alto files in the directory
                    this.currentImageIndex--;
                    this.UpdateWindow();
                }
            }

        }


        /// <summary>
        /// Handler for the next page button  
        /// </summary>
        private void IllustrationWindow_NextButton(object sender, RoutedEventArgs e)
        {
            if (this.altoList != null)
            {
                if (this.currentImageIndex < this.altoList.Count - 1)
                {
                    // Update regarding next alto files in the directory
                    this.currentImageIndex++;
                    this.UpdateWindow();
                }
            }
        }

        #endregion 


    }
}
