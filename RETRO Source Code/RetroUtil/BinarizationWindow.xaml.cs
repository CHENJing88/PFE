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
using AForge.Imaging.Filters;
using System.Drawing;

namespace RetroUtil
{
    /// <summary>
    /// Display a window for the binarization process 
    /// </summary>
    public partial class BinarizationWindow : Window
    {

        // Attributes
        private Bitmap grayscaleImage = null;
        private Bitmap binaryImage = null;
        private BaseInPlacePartialFilter filter = null;
        private bool filterSelected = false;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="image">Bitmap image to consider</param>
        public BinarizationWindow(Bitmap image)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.originalImage.Source = Image2DisplayTool.Bitmap2BitmapSource(ref image);
            
            Grayscale filterG = new Grayscale(0.2125, 0.7154, 0.0721);
            this.grayscaleImage = filterG.Apply(image);

            this.filter = new SISThreshold();
            this.binaryImage = filter.Apply(this.grayscaleImage);

            // Update the preview display
            this.UpdatePreview();
        }


        /// <summary>
        /// Update the preview frame
        /// </summary>
        public void UpdatePreview()
        {
            if (this.binaryImage != null)
            {
                this.binaryImage.Dispose();
                this.binaryImage = filter.Apply(this.grayscaleImage);
                this.previewImage.Source = Image2DisplayTool.Bitmap2BitmapSource(ref this.binaryImage);
            }
        }


        /// <summary>
        /// Handler for the radio button
        /// </summary>
        private void Binarization_RadioButton_Check(object sender, RoutedEventArgs e)
        {
            // Diasable the threshold slider
            if(this.thresholdSlider != null)
                this.thresholdSlider.IsEnabled = false;

            // Update the filter regarding selected radioButton
            if ((bool)this.sisRadioButton.IsChecked)
            {
                filter = new SISThreshold();   
            }
            else if ((bool)this.otsuRadioButton.IsChecked)
            {
                this.thresholdSlider.IsEnabled = false;
                filter = new OtsuThreshold();               
            }
            else if ((bool)this.thresholdRadioButton.IsChecked)
            {
                this.thresholdSlider.IsEnabled = true;
                filter = new Threshold((int)this.thresholdSlider.Value);
            }

            // Update the preview display
            this.UpdatePreview();
        }


        /// <summary>
        /// Handler for threshold slider
        /// </summary>
        private void Binarization_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.grayscaleImage != null)
            {
                ((Threshold)this.filter).ThresholdValue = (int)e.NewValue;
                this.UpdatePreview();
            }
        }


        /// <summary>
        /// Handler for the Apply button
        /// </summary>
        private void Binarization_Button_Apply_Click(object sender, RoutedEventArgs e)
        {
            this.filterSelected = true;
            this.Close();
        }


        /// <summary>
        /// Filter Getter (for the caller)
        /// </summary>
        public BaseInPlacePartialFilter GetFilter()
        {
            return this.filter;
        }


        /// <summary>
        /// Selected Filter Getter (for the caller)
        /// </summary>
        public bool GetFilterSelected()
        {
            return this.filterSelected;
        }
 
    }
}
