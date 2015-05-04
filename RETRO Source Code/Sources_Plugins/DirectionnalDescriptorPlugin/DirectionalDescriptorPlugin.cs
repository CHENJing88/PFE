using Polytech.Clustering.Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using AForge.Imaging.Filters;
using System.Collections.Concurrent;

namespace Polytech.Clustering.Plugin
{
    public partial class DirectionalDescriptorPlugin : Form, IDescriptorPlugin
    {
        private bool[] m_checkedDirections = new bool[8];
        private Bitmap m_initialImage = null;

        /// <summary>
        /// Référence vers la classe de configuration du plugin
        /// </summary>
        private IConfig m_config;

        public DirectionalDescriptorPlugin()
        {
            InitializeComponent();
            //sauvegarde de l'image initiale
            m_initialImage = (Bitmap) imagePanel.BackgroundImage;
            //Par défaut, uniquement N,S,E,O
            checkedListBox1.SetItemChecked(0, true);
            checkedListBox1.SetItemChecked(1, true);
            checkedListBox1.SetItemChecked(2, true);
            checkedListBox1.SetItemChecked(3, true);
            checkedListBox1.SetItemChecked(4, true);
            checkedListBox1.SetItemChecked(5, true);
            checkedListBox1.SetItemChecked(6, true);
            checkedListBox1.SetItemChecked(7, true);
            checkedListBox1.SetItemChecked(8, true);

            UpdateImage();
        }

        public List<string> GetInfoList()
        {
            List<string> infoList = new List<string>();

            infoList.Add("Nom : " + GetName());
            infoList.Add("Direction(s) : à compléter");

            return infoList;
        }

        public string GetName()
        {
            return "Signature directionnelle";
        }

        public string GetAuthor()
        {
            return "Ludovic Esperce";
        }

        public Form GetConfigWindow()
        {
            return this;
        }

        //Update image dans form
        private void UpdateImage()
        {
            int xCenter = imagePanel.BackgroundImage.Width/2;
            int yCenter = imagePanel.BackgroundImage.Height/2;

            //création d'une nouvelle image à afficher 
            Bitmap newImage = (Bitmap)m_initialImage.Clone();
            //récuparation d'une référence vers l'objet Graphics lié à l'image à modifer
            Graphics g = Graphics.FromImage(newImage);
            Pen newPen = new Pen(Color.Red);
            newPen.Width = 2;

            Point center = new Point(xCenter, yCenter);
            //on passe en revue les directions sélectionnées et on dessine les lignes qui nous intéressent
            foreach (int index in checkedListBox1.CheckedIndices)
            {
                switch (index)
                {
                    case 8:
                        g.DrawLine(newPen, center, new Point(xCenter, 0));
                        g.DrawLine(newPen, center, new Point(xCenter, m_initialImage.Height - 1));
                        g.DrawLine(newPen, center, new Point(m_initialImage.Width - 1, yCenter));
                        g.DrawLine(newPen, center, new Point(0, yCenter));
                        g.DrawLine(newPen, center, new Point(m_initialImage.Width - 1, 0));
                        g.DrawLine(newPen, center, new Point(0, 0));
                        g.DrawLine(newPen, center, new Point(m_initialImage.Width - 1, m_initialImage.Height - 1));
                        g.DrawLine(newPen, center, new Point(0, m_initialImage.Height - 1));
                        //màj des valeurs des checkboxes
                        checkedListBox1.SetItemChecked(0, true);
                        checkedListBox1.SetItemChecked(1, true);
                        checkedListBox1.SetItemChecked(2, true);
                        checkedListBox1.SetItemChecked(3, true);
                        checkedListBox1.SetItemChecked(4, true);
                        checkedListBox1.SetItemChecked(5, true);
                        checkedListBox1.SetItemChecked(6, true);
                        checkedListBox1.SetItemChecked(7, true);
                        break;
                    case 0: //N
                        g.DrawLine(newPen, center, new Point(xCenter, 0));
                        break;
                    case 1: //S
                        g.DrawLine(newPen, center, new Point(xCenter, m_initialImage.Height-1));
                        break;
                    case 2: // E
                        g.DrawLine(newPen, center, new Point(m_initialImage.Width-1, yCenter));
                        break;
                    case 3: // O
                        g.DrawLine(newPen, center, new Point(0, yCenter));
                        break;
                    case 4: // NE
                        g.DrawLine(newPen, center, new Point(m_initialImage.Width-1, 0));
                        break;
                    case 5: //NO
                        g.DrawLine(newPen, center, new Point(0, 0));
                        break;
                    case 6: //SE
                        g.DrawLine(newPen, center, new Point(m_initialImage.Width-1, m_initialImage.Height-1));
                        break;
                    case 7:  //SO
                        g.DrawLine(newPen, center, new Point(0, m_initialImage.Height-1));
                        break;
                }
            }
            //une fois la nouvelle image créée on l'ajoute au panel
            imagePanel.BackgroundImage = newImage;

            imagePanel.Update();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateImage();
        }


        public void CalculateSignature(APattern toModify)
        {
            Bitmap imageThumbnail = toModify.ImageRepresentation;

            //conversion en dimension carrée
            Bitmap squared = ImageNormalisationTools.ConvertToSquareImage(imageThumbnail);
            //   squared.Save(@"F:\scolaire\PFE2\TestData\squared" + hpos + vpos);

            //normalisation
            Bitmap resized = ImageNormalisationTools.ResizeImage(squared, int.Parse(normalisationTextBox1.Text), int.Parse(normalisationTextBox1.Text));
            //   resized.Save(@"F:\scolaire\PFE2\TestData\resized" + hpos + vpos);
            squared = null;

            //binarisation
            Bitmap binarised = ImageNormalisationTools.Binarize(resized);

            bool[] checkedOptions = new bool[8] { true, true, true, true, true, true, true, true };
            //filtrage terminé, on calcule les signatures directionnelles
            ASignature directionalSign = DirectionalCalculator.CalculateFeatures(binarised, checkedOptions);
            resized = null;
            toModify.AddSignature(directionalSign);
            binarised = null;

          //  imageThumbnail.Dispose();
        }


        public IConfig GetConfig()
        {
            return null;
        }

        private void validateButton_Click(object sender, EventArgs e)
        {
            //on applique les modifications à la configuration
           // ((ZernikeConfig)m_config).MaxOrder = int.Parse((maxOrderTextBox.Text));
           // ((ZernikeConfig)m_config).SquareHeight = int.Parse((normalisationTextBox1.Text));
           // ((ZernikeConfig)m_config).SquareWidth = int.Parse((normalisationTextBox1.Text));
            this.Hide();
        }
    }
}
