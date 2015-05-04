using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using System.Drawing.Imaging;
using System.Numerics;

namespace Polytech.Clustering.Plugin
{
    /// <summary>
    /// Classe offrant des méthodes permettant de calculer les différents moments de Zernike associés à une image
    /// </summary>
    static class ZernikeCalculator
    {
        // factorial table
        static Int64[] factors64 = { 0, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880, 3628800, 39916800, 479001600, 6227020800, 87178291200, 1307674368000, 20922789888000, 355687428096000, 6402373705728000, 121645100408832000, 2432902008176640000 };

        static Int64 Factorial(int factor)
        {
            if (factor <= 0)
                return 1;
            else
                return factors64[factor];

        }
        
        // Radial function 
        /// <summary>
        /// Calculates Radial function value for a pixel
        /// 
        ///  Input :
        ///  Radius of pixel from orgin
        ///  order n
        ///  repititon m
        /// </summary>
        static private double RadialFunction(double radius, int n, int m)
        {
            double radial = 0;
            double b;
            for (int s = 0; s <= (n - m) / 2; s++)
            {
                b = Factorial(n - s);
                b = b / (Factorial(s) * Factorial((n + System.Math.Abs(m)) / 2 - s) * Factorial((n - System.Math.Abs(m)) / 2 - s));
                b = System.Math.Pow(-1, s) * b;
                radial += b * System.Math.Pow(radius, n - 2 * s);
            }
            return radial;
        }

        /// <summary>
        /// Calcul du moment d'ordre n
        /// </summary>
        /// <param name="n"></param>
        /// <param name="zernikeSign"></param>
        /// <param name="img">Image 8bits à traiter</param>
        static public void OrderN(int n, ZernikeSignature zernikeSign, Bitmap img)          
        {
            int m;                          //m = repeition;
            if (n % 2 == 0) m = 0;
            else m = 1;

            //on accéde à l'image en utilisant des pointeurs
            //Lock the entire bitmap (the rectangle argument) once
            BitmapData locked = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                                                ImageLockMode.ReadOnly,
                                               img.PixelFormat);
            //
            //Utilisation de pointeurs
           unsafe
           {
                for (; m <= n; m = m + 2)
                {
                    double zr = 0;
                    double zi = 0;
                    int count = 0;
                    int dimension = img.Width;
                    if (dimension > img.Height)
                        dimension = img.Height;


                    //on part du point haut-gauche
                    byte* pixelPtr = (byte*)locked.Scan0;
                    for (int y = 0; y < dimension; y++)
                    {
                        for (int x = 0; x < dimension; x++)
                        {
                            double xi = (2 * x - dimension + 1);
                            double yi = (dimension - 2 * y - 1);

                            double radius = System.Math.Sqrt(System.Math.Pow(xi, 2) + System.Math.Pow(yi, 2));
                            radius = radius / dimension;
                            if (radius <= 1)
                            {
                                double radial = RadialFunction(radius, n, m);
                                double theta = System.Math.Atan2(yi, xi);

                                zr += (*(pixelPtr + y * locked.Stride + x)) * radial * System.Math.Cos(m * theta);
                                zi += (*(pixelPtr + y * locked.Stride + x)) * radial * System.Math.Sin(m * theta);

                                count++;
                            }
                        }
                    }

                    zr = (n + 1) * zr / count;
                    zi = (n + 1) * zi / count;


                    // nouveau résultat complexe
                    //ComplexFeature newFeature = new ComplexFeature(zr, zi);
                    Complex newFeature = new Complex(zr, zi);

                    //ajout du résultat à la signature
                    zernikeSign.AddFeature(newFeature);
                }
            }

            //on libére le bitmap
            img.UnlockBits(locked); 
        }

        /// <summary>
        /// Calcul les moments de Zernike pour les "order" premiers ordres
        /// </summary>
        /// <param name="img">L'image concernée par le calcul</param>
        /// <param name="order">Ordre maximale</param>
        /// <returns></returns>
        static public ZernikeSignature CalculateFeatures(Bitmap img, int order)
        {
            ZernikeSignature zernikeSign = new ZernikeSignature();

                for( int i = 1; i <= order; i++ ) // pour les ordres de 1 à "order"
                {
                    //calcul des signatures pour l'ordre i
                    OrderN(i, zernikeSign, img);
                }
                return zernikeSign;
        }

    }
}