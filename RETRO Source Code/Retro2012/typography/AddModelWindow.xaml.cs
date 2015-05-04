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
using AForge.Imaging.Filters;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using RetroGUI.util;
using RetroUtil;


namespace RetroGUI.typography
{
    /// <summary>
    /// Display a page of the book with extracted letters for font creation purpose 
    /// </summary>
    public partial class AddModelWindow : Window
    {
        #region Attributes

        private String imagepath;
        private Bitmap image;
        private Bitmap imageDisplayed;

        private BaseInPlacePartialFilter filter = null;
        private Blob[] blobs;
        private System.Windows.Media.Imaging.BitmapSource bitmapSource;

        private AddModelDataWindow amdw;
        private ModelMetaData modelMetadata;

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        public AddModelWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            modelMetadata = new ModelMetaData();

            // Initialize owned windows
            this.amdw = new AddModelDataWindow();
            this.amdw.SetProvider();
            this.amdw.Visibility = Visibility.Hidden;

        }


        /// <summary>
        /// Binarization
        /// </summary>
        /// <param name="image">Bitmap of the image to process</param>
        private void Binarize(ref Bitmap image)
        {
            if (image != null)
            {
                // grayscale filter (BT709)
                if (image.PixelFormat != System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    Grayscale filterG = new Grayscale(0.2125, 0.7154, 0.0721);
                    image = filterG.Apply(image);
                }

                // binarize
                if (this.filter == null)
                    this.filter = new SISThreshold();

                image = this.filter.Apply(image);
            }
        }


        /// <summary>
        /// Extraction of blobs.
        /// Image must be binarized.
        /// </summary>
        /// <param name="image">Bitmap of the image to process</param>
        private void ExtractBlob(Bitmap image)
        {
            // Invert image
            Invert filterI = new Invert();
            image = filterI.Apply(image);

            // Search blobs
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinWidth = 5;
            blobCounter.MinHeight = 5;
            blobCounter.MaxWidth = 200;
            blobCounter.MaxHeight = 200;
            blobCounter.CoupledSizeFiltering = true;
            blobCounter.ProcessImage(image);
            blobCounter.ObjectsOrder = ObjectsOrder.Size;

            // Convert blob list in eoc list
            this.blobs = blobCounter.GetObjects(image, false);
            //rblob = (tmpblobs[i]).Rectangle;

            // Re-invert image
            //image = filterI.Apply(image);
        }


        /// <summary>
        /// Sort Blobs
        /// </summary>
        private void SortBlob()
        {
            Array.Sort(this.blobs, delegate(Blob blob1, Blob blob2)
            {
                return Math.Min((blob1.Rectangle.Width - blob2.Rectangle.Width), (blob1.Rectangle.Height - blob2.Rectangle.Height));
            });
        }


        /// <summary>
        /// Draw Blobs on image
        /// </summary>
        /// <param name="image">Bitmap image where blobs will be drawn</param>
        private void DrawBlob(Bitmap image)
        { 
            Blob currentBlob = null;
            if (this.blobs != null)
            {
                IEnumerator en = blobs.GetEnumerator();

                // Lock bits
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
                System.Drawing.Imaging.BitmapData bitmapData =
                    image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    image.PixelFormat);

                // Draw bounding boxes
                while (en.MoveNext())
                {
                    currentBlob = (Blob)en.Current;
                    // Workaround for a thick rectangle: draw 2 rectangles ...
                    AForge.Imaging.Drawing.Rectangle(bitmapData, new System.Drawing.Rectangle(currentBlob.Rectangle.X - 1, currentBlob.Rectangle.Y - 1, currentBlob.Rectangle.Width + 2, currentBlob.Rectangle.Height + 2), System.Drawing.Color.Red);
                    AForge.Imaging.Drawing.Rectangle(bitmapData, currentBlob.Rectangle, System.Drawing.Color.Red);
                }

                // Unlock bits
                image.UnlockBits(bitmapData);
            }
        }


        /// <summary>
        /// Update Image area
        /// </summary>
        private void UpdateView()
        {
            this.AddModel_image.Source = this.bitmapSource;
            this.AddModel_canvas.Width = this.bitmapSource.Width;
            this.AddModel_canvas.Height = this.bitmapSource.Height;
        }


        /// <summary>
        /// Handler for a click on the open button
        /// </summary>
        private void AddModel_Button_Open_Click(object sender, RoutedEventArgs e)
        {
            // Open Show Image Dialog
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = System.IO.Path.GetDirectoryName(this.imagepath);
            ofd.Filter = "Image Files|*.bmp;*.jpg;*.png;*.tiff|All Files|*.*";
            ofd.ShowDialog();

            // Update the AddModelWindow
            if (ofd.SafeFileName.Length != 0)
            { 
                this.imagepath = ofd.FileName;

                // Update image
                if (this.image != null)
                    this.image.Dispose();
                this.image = new Bitmap(imagepath);

                // Update image displayed
                if (this.imageDisplayed != null)
                    this.imageDisplayed.Dispose();
                this.imageDisplayed = new Bitmap(this.image);

                // Convert to color image if grayscale
                if (this.image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    GrayscaleToRGB filter = new GrayscaleToRGB();
                    this.image = filter.Apply(this.image);
                }

                // Update attributes
                this.bitmapSource = RetroUtil.Image2DisplayTool.Bitmap2BitmapSource(ref image);
                this.blobs = null;
                this.modelMetadata.Clean();

                // Update controls
                this.AddModel_binarizeButton.IsEnabled = true;
                this.AddModel_binarizeButton.ToolTip = null;
                this.AddModel_extractButton.IsEnabled = true;
                this.AddModel_extractButton.ToolTip = null;

                // Update View
                this.UpdateView();
                this.AddModel_imagename.Content = this.imagepath;
                this.AddModel_zoomSlider.Value = 1;

            }
        }


        /// <summary>
        /// Handler for a click on the binarize button
        /// Open a Binarization windows to manage a custom binarozation
        /// </summary>
        private void AddModel_Button_Binarize_Click(object sender, RoutedEventArgs e)
        {
            if (this.image != null)
            {
                BinarizationWindow bw = new BinarizationWindow(this.image);
                bw.Owner = this;
                bw.ShowDialog();

                if (bw.GetFilterSelected())
                {
                    // Get the selected filter
                    this.filter = bw.GetFilter();

                    // Binarize
                    Bitmap imageBinarized = new Bitmap(this.image);
                    this.Binarize(ref imageBinarized);

                    // Update View     
                    this.bitmapSource = RetroUtil.Image2DisplayTool.Bitmap2BitmapSource(ref imageBinarized);
                    this.UpdateView();
                }
                
            }

        }
        

        /// <summary>
        /// Handler for a click on the extract button
        /// Process a Blob extraction in EoC for typography analysis
        /// </summary>
        private void AddModel_Button_Extract_Click(object sender, RoutedEventArgs e)
        {
            if (this.image != null)
            {
                // Disable the binarization and extraction button
                this.AddModel_binarizeButton.IsEnabled = false;  
                this.AddModel_binarizeButton.ToolTip = "Reload Image first";
                this.AddModel_extractButton.IsEnabled = false;
                this.AddModel_extractButton.ToolTip = "Reload Image first";

                Bitmap imageBinarized = new Bitmap(this.image);

                // Binarize
                this.Binarize(ref imageBinarized);

                // Extract Blobs
                this.ExtractBlob(imageBinarized);

                // Free binarized bitmap
                imageBinarized.Dispose();

                // Sort Blobs
                this.SortBlob();

                // Draw Blobs
                this.DrawBlob(this.imageDisplayed); 

                // Update View
                this.bitmapSource = RetroUtil.Image2DisplayTool.Bitmap2BitmapSource(ref this.imageDisplayed);
                this.UpdateView();

                // Open dialog to get model book metadata
                AddModelMetadataWindow ammw = new AddModelMetadataWindow();
                ammw.Owner = this;
                ammw.ShowDialog();

                // Get model metadata
                this.modelMetadata = ammw.GetModelMetaData();

            }
        }


        /// <summary>
        /// Handler for a click on an EoC in the image
        /// </summary>
        private void AddModel_Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Only when CC extraction has been done and blobs computed
            if (blobs != null)
            {
                // Get the position of the mouse
                System.Windows.Point position = e.GetPosition((IInputElement)sender);

                IEnumerator en = blobs.GetEnumerator();
                Blob blob = null;

                bool found = false;

                // Search selected Blob
                while (en.MoveNext() && !found)
                {
                    blob = (Blob)en.Current;
                    if ((blob.Rectangle.Left < position.X) && (blob.Rectangle.Right > position.X) &&
                             (blob.Rectangle.Top < position.Y) && (blob.Rectangle.Bottom > position.Y))
                    {
                        found = true;

                        if (this.amdw.HasBeenClosed)
                        {
                            this.amdw = new AddModelDataWindow();
                            this.amdw.SetProvider();
                        }

                        amdw.SetImage(this.imagepath, blob.Rectangle);
                        amdw.ColorizeBlob = false;
                        amdw.Owner = this;
                        amdw.ShowDialog();

                        // Colorize the processed EoC
                        if (this.amdw.ColorizeBlob)
                        {
                            // Lock bits
                            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, this.imageDisplayed.Width, this.imageDisplayed.Height);
                            System.Drawing.Imaging.BitmapData bitmapData =
                                this.imageDisplayed.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                                this.imageDisplayed.PixelFormat);
                            // Workaround for a thick rectangle: draw 2 rectangles ...
                            AForge.Imaging.Drawing.Rectangle(bitmapData, new System.Drawing.Rectangle(blob.Rectangle.X - 1, blob.Rectangle.Y - 1, blob.Rectangle.Width + 2, blob.Rectangle.Height + 2), System.Drawing.Color.Lime);
                            AForge.Imaging.Drawing.Rectangle(bitmapData, blob.Rectangle, System.Drawing.Color.Lime);
                            // Unlock bits
                            this.imageDisplayed.UnlockBits(bitmapData);
                            // Update View
                            this.bitmapSource = RetroUtil.Image2DisplayTool.Bitmap2BitmapSource(ref this.imageDisplayed);
                            this.UpdateView();
                        }
                        
                    }
                }
            }
        }


        /// <summary>
        /// Handler for the mouse wheel in the image  
        /// Increase/Decrease zoom
        /// </summary>
        void AddModel_Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((this.AddModel_zoomSlider.IsEnabled) && (Keyboard.Modifiers == ModifierKeys.Control))
            {
                if (e.Delta > 0) // UP
                    this.AddModel_zoomSlider.Value = (this.AddModel_zoomSlider.Value < 5) ? this.AddModel_zoomSlider.Value + 0.5 : this.AddModel_zoomSlider.Value;
                else // DOWN
                    this.AddModel_zoomSlider.Value = (this.AddModel_zoomSlider.Value > 0.5) ? this.AddModel_zoomSlider.Value - 0.5 : this.AddModel_zoomSlider.Value;
            }
        }


        /// <summary>
        /// Model metadata getter for the AddModelDataWindow
        /// </summary>
        public ModelMetaData GetModelMetaData()
        {
            return this.modelMetadata;
        }



    }
}



