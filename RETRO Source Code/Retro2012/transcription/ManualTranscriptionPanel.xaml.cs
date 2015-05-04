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
using System.IO;
using AForge.Imaging.Filters;
using System.Drawing;

using Retro.Model;
using Polytech.Clustering.Plugin;
using Retro.ViewModel;

namespace RetroGUI.transcription
{
    /// <summary>
    /// Define the ManualTranscription Panel
    /// </summary>
    public partial class ManualTranscriptionPanel : UserControl
    {

        #region Attributes

        /// <summary>
        /// Instance of the ViewModel of the application
        /// </summary>
        private RetroViewModel retroVM;


        /// <summary>
        /// List of the cluster to transcript
        /// </summary>
        private List<Cluster> clustersList;


        /// <summary>
        /// Current displayed cluster
        /// </summary>
        private Cluster currentCluster;


        /// <summary>
        /// Current displayed cluster index
        /// </summary>
        private int currentClusterIndex = 0;


        /// <summary>
        /// List of Image Control for Context preview
        /// For UpdateContextImages()
        /// TODO: Handle without this attributes
        /// </summary>
        private List<System.Windows.Controls.Image> clusterShapeContextImages;

        
        /// <summary>
        /// Current shape index of the cluster
        /// </summary>
        private int currentShapeIndex = 0;

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="retroVM">Instance of the ViewModel</param>
        public ManualTranscriptionPanel(RetroViewModel retroVM)
        {
            //TODO: Maybe retroVM isn't need, just the list of clusters!
            //      Check if two way binding! 
            //      Imbricates the if or manage return errors values!


            if (retroVM !=null)
                this.retroVM = retroVM;

            // Get the clusters that aren't labelized yet
            if (this.retroVM.RetroInstance != null)
            {
                this.clustersList =
                    this.retroVM.RetroInstance.ClustersList.FindAll(
                    delegate(Cluster cluster)
                    {
                        return !cluster.IsLabelized;
                    }
                );
            }
                    

            if (this.clustersList.Count > 0)
                this.currentCluster = this.clustersList[0];

            InitializeComponent();

            this.InitClusterShapeContextImageList();
            this.UpdateView();

        }


        /// <summary>
        /// Initialize the list of cluster shape context image Control
        /// </summary>
        private void InitClusterShapeContextImageList()
        {
            this.clusterShapeContextImages = new List<System.Windows.Controls.Image>();
            this.clusterShapeContextImages.Add(this.clusterContextImage1);
            this.clusterShapeContextImages.Add(this.clusterContextImage2);
            this.clusterShapeContextImages.Add(this.clusterContextImage3);
            this.clusterShapeContextImages.Add(this.clusterContextImage4);
        }


        /// <summary>
        /// Update the left part of the window
        /// </summary>
        private void UpdateView()
        {
            // TODO: make a direct binding in the XAML file!
            if (this.currentCluster != null)
            {
                this.clusterNumberLabel.Content = this.currentCluster.Id;
                this.clusterTranscriptionTextBox.Text = (this.currentCluster.LabelList.Count > 0) ? this.currentCluster.LabelList[0] : "[" + this.currentCluster.Id + "]";
                this.clusterTranscriptionTextBox.Focus();
                this.clusterTranscriptionTextBox.SelectAll();
                this.clusterShapeNumberLabel.Content = this.currentCluster.Patterns.Count;

                // Update representative thumbnail
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                string pathh = "debug jy this.currentCluster.RepresentativePathToBitmap";
                if (File.Exists(pathh))
                    bitmap.UriSource = new Uri(pathh, UriKind.Absolute);
                else
                    bitmap.UriSource = new Uri("/resources/LogoLI.png", UriKind.Relative);
                bitmap.EndInit();
                this.clusterRepresentativeImage.Source = bitmap;

                // Update the contextImages
                this.UpdateContextImages();
            }
        }


        /// <summary>
        /// Update the right part of the window 
        /// </summary>
        private void UpdateContextImages()
        {
            // TODO: use Invoke and Delegate???
            // TODO: use binding instead of 4 copy-paste the same code

            for (int i = this.currentShapeIndex; i < this.currentShapeIndex + 4 ; i++)
            {
                if (i < this.currentCluster.Patterns.Count) 
                {
                    APattern shape = this.currentCluster.Patterns[i];
                    BitmapSource bitmapSource = this.GetShapeContextImage(shape);
                    System.Windows.Controls.Image imageControl = (System.Windows.Controls.Image)this.clusterShapeContextImages[i % 4];
                    if (bitmapSource != null) 
                        imageControl.Source = bitmapSource;
                    else
                        imageControl.Source = null;
                }
                else
                {
                    System.Windows.Controls.Image imageControl = (System.Windows.Controls.Image)this.clusterShapeContextImages[i % 4];
                    imageControl.Source = null;
                }
            }
        }


        /// <summary>
        /// Get shape context image 
        /// </summary>
        private BitmapSource GetShapeContextImage(APattern shape)
        {
            // Get the image source
            String imagepath = Path.GetDirectoryName(this.retroVM.RetroInstance.FullImagesPath) + @"\" + shape.ImageRepresentation + ".png";

            if (File.Exists(imagepath))
            {
                // Open the image
                Bitmap bitmap = new Bitmap(imagepath);

                // Convert to color image if grayscale
                if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    GrayscaleToRGB filter = new GrayscaleToRGB();
                    bitmap = filter.Apply(bitmap);
                }

                // Get the RoI of the image source corresponding to the context shape                
                Rectangle roi = new Rectangle(0, 0, shape.ImageRepresentation.Width, shape.ImageRepresentation.Height);

                // Crop the image
                Crop filter_ = new Crop(roi);
                Bitmap croppedBitmap = filter_.Apply(bitmap);

                // Free the source bitmap
                bitmap.Dispose();

                // Lock bits of the cropped image
                Rectangle rect = new Rectangle(0, 0, croppedBitmap.Width, croppedBitmap.Height);
                System.Drawing.Imaging.BitmapData bitmapData =
                    croppedBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    croppedBitmap.PixelFormat);

                // Draw the bounding box of the shape
                // Workaround for a thick rectangle: draw 2 rectangles ...
                //System.Drawing.Rectangle shapeBB = new System.Drawing.Rectangle(shape.X - roi.X, shape.Y - roi.Y, shape.Width, shape.Height);
                System.Drawing.Rectangle shapeBB = new System.Drawing.Rectangle(0, 0, shape.ImageRepresentation.Width, shape.ImageRepresentation.Height);
                AForge.Imaging.Drawing.Rectangle(bitmapData, new System.Drawing.Rectangle(shapeBB.X - 1, shapeBB.Y - 1, shapeBB.Width + 2, shapeBB.Height + 2), System.Drawing.Color.Red);
                AForge.Imaging.Drawing.Rectangle(bitmapData, shapeBB, System.Drawing.Color.Red);

                // Unlock bits
                croppedBitmap.UnlockBits(bitmapData);

                // Display the image
                BitmapSource bitmapSource =
                    System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        croppedBitmap.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                // Free the cropped bitmap
                croppedBitmap.Dispose();

                return bitmapSource;
            }
            else
            {
                return null;
            }
        }


        #region Events handlers

        /// <summary>
        /// Handler for a click on the previous cluster button
        /// </summary>
        private void ManualTranscription_Previous_Cluster_Click(object sender, RoutedEventArgs e)
        {
            this.currentClusterIndex = (this.currentClusterIndex > 0) ? this.currentClusterIndex - 1 : this.currentClusterIndex;
            this.currentCluster = this.clustersList[this.currentClusterIndex];
            this.currentShapeIndex = 0;
            UpdateView();
        }


        /// <summary>
        /// Handler for a click on the next cluster button
        /// </summary>
        private void ManualTranscription_Next_Cluster_Click(object sender, RoutedEventArgs e)
        {
            this.currentClusterIndex = (this.currentClusterIndex < this.clustersList.Count - 1) ? this.currentClusterIndex + 1 : this.currentClusterIndex;
            this.currentCluster = this.clustersList[this.currentClusterIndex];
            this.currentShapeIndex = 0;
            UpdateView();
        }


        /// <summary>
        /// Handler for a Enter key event
        /// </summary>
        private void ManualTranscription_Enter_KeyPressed(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((e.Key == Key.Enter) || (e.Key == Key.N))
            {
                if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.N))
                {
                    this.currentCluster.AddNewLabel("MANUAL", "[NOISE]", 1.0);
                }
                else
                {
                    this.currentCluster.AddNewLabel("MANUAL", this.clusterTranscriptionTextBox.Text, 1.0);
                }

                this.currentCluster.IsLabelized = true;
                this.currentClusterIndex = (this.currentClusterIndex < this.clustersList.Count - 1) ? this.currentClusterIndex + 1 : this.currentClusterIndex;
                this.currentCluster = this.clustersList[this.currentClusterIndex];
                this.currentShapeIndex = 0;
                UpdateView();
            }
        }
        

        /// <summary>
        /// Handler for a Noise button click
        /// </summary>
        private void ManualTranscription_Noise_Click(object sender, RoutedEventArgs e)
        {
            // Assign label
            this.currentCluster.AddNewLabel("MANUAL", "[NOISE]", 1.0);

            this.currentClusterIndex = (this.currentClusterIndex < this.clustersList.Count - 1) ? this.currentClusterIndex + 1 : this.currentClusterIndex;
            this.currentCluster = this.clustersList[this.currentClusterIndex];
            this.currentShapeIndex = 0;
            UpdateView();
        }


        /// <summary>
        /// Handler for a click on the previous shapes button
        /// </summary>
        private void ManualTranscription_Previous_Shapes_Click(object sender, RoutedEventArgs e)
        {
            this.currentShapeIndex = (this.currentShapeIndex >= 4) ? this.currentShapeIndex - 4 : this.currentShapeIndex;
            UpdateContextImages();
        }


        /// <summary>
        /// Handler for a click on the next shapes button
        /// </summary>
        ///
        private void ManualTranscription_Next_Shapes_Click(object sender, RoutedEventArgs e)
        {
            this.currentShapeIndex = (this.currentShapeIndex < this.currentCluster.Patterns.Count - 4) ? this.currentShapeIndex + 4 : this.currentShapeIndex;
            UpdateContextImages();
        }

        #endregion


    }
}
