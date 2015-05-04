using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polytech.Clustering.Plugin
{
    static class DirectionalCalculator
    {
        static public ASignature CalculateFeatures(Bitmap imgToProcess, Boolean[] directions)
        {
            DirectionnalSignature newSign = new DirectionnalSignature();

            int byteStep = 0;
            //on récupère le nombre de bytes à lire pour un pixel donné
            switch(imgToProcess.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    byteStep = 1;
                    break;
                case PixelFormat.Format24bppRgb:
                    byteStep = 3;
                    break;
                case PixelFormat.Format16bppGrayScale:
                    byteStep = 2;
                    break;
                case PixelFormat.Format32bppArgb:
                    byteStep = 4;
                    break;
            }

            int imgHeight = imgToProcess.Height;
            int imgWidth = imgToProcess.Width;
            //on accéde à l'image en utilisant des pointeurs
            //Lock the entire bitmap (the rectangle argument) once
            BitmapData locked = imgToProcess.LockBits(new Rectangle(0, 0, imgToProcess.Width, imgToProcess.Height),
                                                ImageLockMode.ReadOnly,
                                               imgToProcess.PixelFormat);
            

            //Utilisation de pointeurs
            unsafe
            {
                //on stock l'adresse du pixel correspondant au centre de l'image
                byte* pixelPtr = (byte*)locked.Scan0;
                byte* centerPixel = (pixelPtr + (imgHeight/2) *locked.Stride + (imgWidth/2)*byteStep);

                //on effectue le calcul de la signature dans les direction choisies par l'utilisateur
                byte* currentPixel = null;
                int xMove = 0, yMove = 0;
                for (int i = 0; i < 8; i++)
                {
                    if(directions[i])
                    {
                        //on adapte le comportement du calcul en fonction des directions choisies
                        switch (i)
                        {
                            case 0: // N
                                xMove = 0;
                                yMove = -1;
                                break;
                            case 1: // S
                                xMove = 0;
                                yMove = 1;
                                break;
                            case 2: // Est
                                xMove = 1;
                                yMove = 0;
                                break;
                            case 3: //O
                                xMove = -1;
                                yMove = 0;
                                break;
                            case 4: //NE
                                xMove = 1;
                                yMove = -1;
                                break;
                            case 5: //NO
                                xMove = -1;
                                yMove = -1;
                                break;
                            case 6: // SE
                                xMove = 1;
                                yMove = 1;
                                break;
                            case 7: //SO
                                xMove = -1;
                                yMove = 1;
                                break;
                        }

                        double value = 0.0;
                        //positionnement des index sur le pixel au centre de l'image
                        int y = imgHeight / 2;
                        int x = imgWidth / 2;
                        double valueTmp = 0;
                        while ((y >-1 && y < imgHeight) && (x>-1 && x < imgWidth)) //boucle sur les lignes et les colonnes
                        {
                            currentPixel = (pixelPtr + y * locked.Stride + x * byteStep);
                            //on somme les niveaux de gris lus
                            if (imgToProcess.PixelFormat == PixelFormat.Format24bppRgb)
                            {
                                valueTmp =(currentPixel[0] + currentPixel[1] + currentPixel[2]) / 3;; 
                                //on récupère une intensité de gris en fonction des valeurs RGB
                                
                            }
                            else if (imgToProcess.PixelFormat == PixelFormat.Format8bppIndexed)
                            {
                                valueTmp = *currentPixel;

                            }
                            else if (imgToProcess.PixelFormat == PixelFormat.Format32bppArgb)
                            {

                                valueTmp = (currentPixel[0] + currentPixel[1] + currentPixel[2]) / 3; //on ne prend pas en compte alpha
                            }

                            //si pixel noir
                            if (valueTmp < 50)                                 
                                value++;

                            //on avance le pointeur sur le pixel d'après
                            x += xMove;
                            y += yMove;
                        }
                        //ajour de la valeur lue dans une direction donnée à la signature
                        newSign.AddFeature(value);
                    }
                }
            }//fin unsafe
            imgToProcess.UnlockBits(locked);
            return newSign;
        }
    }
}
