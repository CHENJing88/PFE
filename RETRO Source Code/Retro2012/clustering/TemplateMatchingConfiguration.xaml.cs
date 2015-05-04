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

using Retro.Model;
using System.IO; 

namespace RetroGUI.clustering
{
    /// <summary>
    /// Logique d'interaction pour TemplateMatchingConfiguration.xaml
    /// </summary>
    public partial class TemplateMatchingConfiguration : Window
    {
        //Attributes
        private String agoraProjectDir;
        private String outputClustersDir;

        public TemplateMatchingConfiguration()
        {
            InitializeComponent();
        }

        private void SliderThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //this.parameters.TEMPLATE_MATCHING_THRESHOLD = (float)(e.NewValue / 100.0);
        }
        
        /// <summary>
        /// Handler for Cancel Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancelTemplateMatchingConfig_Click(object sender, RoutedEventArgs e)
        {
            //this.ClearClustersList();
            this.Hide();

        }

        /// <summary>
        /// Handler for binarization checkbox
        /// </summary>
        private void CheckBoxBinarize_Changed(object sender, RoutedEventArgs e)
        {
            //Recovery of CheckBoxBinarize state (checked or unchecked)
            //this.parameters.BINARIZE_FOR_COMPARISON = (bool)this.CheckBoxBinarize.IsChecked;
            
            // If CheckBoxBinarize is uncheked, disable denoising
            if (this.CheckBoxBinarize.IsChecked == false)
            {
                //this.parameters.BINARIZE_FOR_COMPARISON = false;
                this.CheckBoxDenoise.IsChecked = false;
                this.CheckBoxDenoise.IsEnabled = false;
                //this.parameters.DENOISE_BOUNDING_BOX = false;
            }
            else
            {
                if (this.CheckBoxBinarize != null)
                {
                    //this.parameters.BINARIZE_FOR_COMPARISON = true;
                    if (this.CheckBoxDenoise != null)
                    {
                        this.CheckBoxDenoise.IsEnabled = true;
                    }
                }
            }            
        }

        /// <summary>
        /// Handler for normalization checkbox
        /// </summary>
        private void CheckBoxNormalize_Changed(object sender, RoutedEventArgs e)
        {
            //this.parameters.NORMALIZE_BEFORE_COMPARISON = (bool)this.CheckBoxNormalize.IsChecked;
        }

        /// <summary>
        /// Handler for Denoising checkbox (binary images mandatory)
        /// </summary>
        private void CheckBoxDenoise_Changed(object sender, RoutedEventArgs e)
        {
            //this.parameters.DENOISE_BOUNDING_BOX = (bool)this.CheckBoxDenoise.IsChecked;
        }

        /// <summary>
        /// Handler for Ok button : recovery of the differents items states
        /// </summary>
        private void ButtonOKTemplateMatchingConfig_Click(object sender, RoutedEventArgs e)
        {
         /*   this.parameters.TEMPLATE_MATCHING_THRESHOLD = (float)(this.SliderThreshold.Value / 100.0);
            this.parameters.BINARIZE_FOR_COMPARISON     = (bool)this.CheckBoxBinarize.IsChecked;
            this.parameters.NORMALIZE_BEFORE_COMPARISON = (bool)this.CheckBoxNormalize.IsChecked;
            this.parameters.DENOISE_BOUNDING_BOX        = (bool)this.CheckBoxDenoise.IsChecked;
         */
            this.Hide();
          
        }
    }
}
