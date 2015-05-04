using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polytech.Clustering.Plugin
{
    /// <summary>
    /// Static Class with image processing tools
    /// </summary>
    public static class ImageNormalisationTools
    {
        /// <summary>
        /// Structure for a pixel 32bits argb 
        /// </summary>
        public struct PixelData32argb
        {
            public byte B;
            public byte G;
            public byte R;
            public byte A;

            /// <summary>
            /// Structure for a pixel 32bits argb 
            /// </summary>
            public PixelData32argb(byte r, byte g, byte b, byte a)
            {
                this.R = r;
                this.G = g;
                this.B = b;
                this.A = a;
            }
        }

        /// <summary>
        /// Structure for a pixel 24bits argb 
        /// </summary>
        public struct PixelData24rgb
        {
            public byte B;
            public byte G;
            public byte R;

            /// <summary>
            /// Structure for a pixel 24bits argb 
            /// </summary>
            public PixelData24rgb(byte r, byte g, byte b)
            {
                this.R = r;
                this.G = g;
                this.B = b;
            }
        }


        /// <summary>
        /// Structure for a pixel 8bits 
        /// </summary>
        public struct PixelData8
        {
            public byte intensity;

            /// <summary>
            /// Structure for a pixel 8bits argb 
            /// </summary>
            public PixelData8(byte intensity)
            {
                this.intensity = intensity;
            }
        }


        /// <summary>
        /// Binarization of the image
        /// </summary>
        /// <param name="toProcess">Image to process</param>
        /// <returns>Resulting image</returns>
        public static Bitmap Binarize(Bitmap toProcess)
        {
            Bitmap binarised; //Binarised image
           
            if ((toProcess.PixelFormat == PixelFormat.Format16bppGrayScale || toProcess.PixelFormat == PixelFormat.Format8bppIndexed))
            {
                // create binarisation filter
                SISThreshold filter2 = new SISThreshold();
                // apply filter
                binarised = filter2.Apply(toProcess);
                binarised = new Bitmap(toProcess);
                //filter2.Apply(UnmanagedImage.FromManagedImage(toProcess), UnmanagedImage.FromManagedImage(binarised));
            }
            else
            {
                // apply greyscale filter BT709
                Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
                System.Drawing.Bitmap binaryImage = filter.Apply(toProcess);  

                // Binarisation 
                SISThreshold filter2 = new SISThreshold();
                // on applique le filtre
                //binarised = filter2.Apply(binaryImage);
                binarised = new Bitmap(binaryImage);
                //filter2.Apply(UnmanagedImage.FromManagedImage(binaryImage), UnmanagedImage.FromManagedImage(binarised));
            }
            return binarised;
        }

        /// <summary>
        /// Resize  with square dimension an image
        /// </summary>
        /// <param name="toProcess">Image to resize</param>
        /// <param name="squareDimension">Desired Size</param>
        /// <returns></returns>
        static public Bitmap NormalizeImageAsSquare(Bitmap toProcess, int squareDimension)
        {
            //create filter
            ResizeBicubic filter2 = new ResizeBicubic(squareDimension, squareDimension);
            //apply filter to normalize image
            Bitmap newImage = filter2.Apply(toProcess);
            return newImage;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            //a holder for the result
            Bitmap result = new Bitmap(width, height);
            // set the resolutions the same to avoid cropping due to resolution differences
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            //return the resulting bitmap
            return result;
        }

        /// <summary>
        /// Convertit une image en dimension carrée
        /// </summary>
        static public Bitmap ConvertToSquareImage(Bitmap toProcess)
        {
            Bitmap backgroundImage;

            //récupération de la taille max 
            int dimension = toProcess.Width;                                          
            if (dimension < toProcess.Height)
                dimension = toProcess.Height;

            backgroundImage = DrawBackgroundImage(toProcess, dimension); //récupération d'une image correspondant au background
           // backgroundImage.Save();
            //on copie l'image sur le background créé
            Graphics g = Graphics.FromImage(backgroundImage);
            //calcul des bits de padding pour recentrer l'image
            //en hauteur
            double heightPadding = Math.Round((double)(dimension - toProcess.Height) / 2, MidpointRounding.AwayFromZero);
            double widthPadding = Math.Round((double)(dimension - toProcess.Width) / 2, MidpointRounding.AwayFromZero);

            g.DrawImage(toProcess, (int)widthPadding, (int)heightPadding, toProcess.Width, toProcess.Height);
            g.Dispose();
            return backgroundImage;
        }

        

        /// <summary>
        /// Draw the background of images converted in square with the mean grey value - TODO
        /// </summary>
        /// <param name="image">image to process</param>
        /// <param name="dimension">Square size</param>
        /// <returns>L'Background image</returns>
        static private Bitmap DrawBackgroundImage(Bitmap image, int dimension)
        {
            //background image
            Bitmap backgroundImage = new Bitmap(dimension, dimension, PixelFormat.Format24bppRgb);

            Graphics imgGraphic = Graphics.FromImage(backgroundImage);
            SolidBrush brush = new SolidBrush(Color.White);                //White for the meoment : TODO!
            imgGraphic.FillRectangle(brush, 0, 0, dimension, dimension);

            //free memory
            imgGraphic.Dispose();
            brush.Dispose();
        
            return backgroundImage;
        }
    }
}
