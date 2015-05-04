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
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RetroUtil
{
    /// <summary>
    /// Image conversion functions
    /// </summary>
    public class Image2DisplayTool
    {

        /// <summary>
        /// External function to to free the memory used by the GDI bitmap object.
        /// </summary>
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);


        /// <summary>
        /// Convert an image from System.Drawing.Bitmap to System.Windows.Media.Imaging.BitmapSource
        /// Allow to juggle between image processing format and wpf display format
        /// </summary>
        /// <param name="bitmap">Bitmap to convert</param>
        /// <returns>BitmapSource representation of the Bitmap</returns>
        public static System.Windows.Media.Imaging.BitmapSource Bitmap2BitmapSource(ref Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();

            System.Windows.Media.Imaging.BitmapSource bitmapSource =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            // Free memory
            DeleteObject(hBitmap);

            return bitmapSource;

        }
    }
}
