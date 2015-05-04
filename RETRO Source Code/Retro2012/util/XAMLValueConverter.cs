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
using System.Windows.Data;

namespace RetroGUI.util
{
    /// <summary>
    /// XAML Value Converter Class
    /// Add the path and the extension to the name/ID of the EoC 
    /// </summary>
    class ImagePathConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            // Get values
            String eocID = (String)values[0];
            String eocType = ((System.Xml.XmlLinkedNode)values[1]).Name ;

            // Get parameters
            String[] parameters = parameter as String[];
            String imageSourceID = eocID.Substring(0, eocID.IndexOf('.'));

            // Build result string
            String imagePath = "";
            if (eocType.CompareTo("alto:Illustration") == 0)
                imagePath = (parameters[1] + imageSourceID + @"\" + eocID + parameters[2]);
            else
                imagePath = (parameters[0] + imageSourceID + @"\" + eocID + parameters[2]);

            // Return image source
            System.Windows.Media.ImageSourceConverter conv = new System.Windows.Media.ImageSourceConverter();
            return conv.ConvertFromString(imagePath);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Fit the EoC image dimensions and positions regarding the fixed size of the Canvas
    /// NOT USED ANYMORE
    /// </summary>
    public class FitCanvasConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String[] parameters = parameter as String[];
            int canvasDimension = System.Convert.ToInt32(parameters[0]);
            int imageDimension = System.Convert.ToInt32(parameters[1]);
            double input = System.Convert.ToDouble(value);
            return canvasDimension * input / imageDimension;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    
}
