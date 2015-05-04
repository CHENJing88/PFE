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
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;

using Retro.Model;
using Polytech.Clustering.Plugin;


namespace RetroGUI.visualisation
{
    /// <summary>
    /// Define Cluster Window
    /// </summary>
    public partial class ClusterWindow : Window
    {
        #region Attributes

        private int index = 0;
        private ObservableCollection<APattern> TwelveShapes = new ObservableCollection<APattern>();

        private Cluster _CurrentCluster;
        /// <summary>
        /// Current selected Cluster
        /// </summary>
        public Cluster CurrentCluster
        {
            get { return _CurrentCluster; }
            set { _CurrentCluster = value; }

        }

        private List<APattern> _Shapes;
        /// <summary>
        /// APattern list
        /// </summary>
        public List<APattern> Shapes
        {
            get { return _Shapes; }
            set { _Shapes = value; }

        }

        private bool _HasBeenClosed;
        /// <summary>
        /// To avoid multiple instince of ClusterWindows ans handle it's closing
        /// </summary>
        public bool HasBeenClosed
        { 
            get { return _HasBeenClosed; }
            set { _HasBeenClosed = value; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ClusterWindow()
        {
            InitializeComponent();
            this.HasBeenClosed = false;
        }
        #endregion

        /// <summary>
        /// Update the view regarding the current cluster that is considered
        /// </summary>
        public void UpdateView()
        {
            this.label_clusterno.Content = this.CurrentCluster.Id;
            this.label_nbshapes.Content = this.CurrentCluster.Patterns.Count;
            this.textBox_label.Text = (this.CurrentCluster.LabelList.Count != 0) ? this.CurrentCluster.LabelList.ElementAt(0) : "[" + this.CurrentCluster.Id + "]";
            this.textBox_label.Focus();
            this.textBox_label.SelectAll();

            // Update representative thumbnail
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            if (File.Exists(this.CurrentCluster.Patterns[0].ToString())) //fichier image
                src.UriSource = new Uri(this.CurrentCluster.Patterns[0].ToString(), UriKind.Absolute);
            else
                src.UriSource = new Uri("/resources/LogoLI.png", UriKind.Relative);
            src.EndInit();

            this.RepresentativeImage.Source = src;
            this.RepresentativeImage.Stretch = Stretch.Uniform;

            
            this.Shapes = this.CurrentCluster.Patterns;
            this.Shape_listBox.ItemsSource = TwelveShapes;

            index = 0;
            UpdateBinding();
            
        }


        /// <summary>
        /// Update the 12 shapes of the cluster that are displayed
        /// </summary>
        private void UpdateBinding()
        {
            TwelveShapes.Clear();
            for (int i = index; i < Math.Min(Shapes.Count, index + 12); i++)
            {
                TwelveShapes.Add(Shapes.ElementAt(i));
            }
        }


        /// <summary>
        /// Handler for TextBox label change
        /// </summary>
        private void ClusterWindow_TextBox_Label_TextChange(object sender, TextChangedEventArgs e)
        {
            if (this.textBox_label.Text != null)
            {
                if (this.CurrentCluster.LabelList.Count > 0)
                {
                    if (this.textBox_label.Text.CompareTo(this.CurrentCluster.LabelList[0]) == 0)
                    {
                        // Don't add a new manual label if it already exist!
                    }
                    else
                    {
                        this.CurrentCluster.AddNewLabel("MANUAL", this.textBox_label.Text, 1.0);
                        this.CurrentCluster.IsLabelized = true;
                    }
                }
                else 
                {
                    if ((this.textBox_label.Text.StartsWith("[")) && (this.textBox_label.Text.EndsWith("]")))
                    {
                        this.CurrentCluster.AddNewLabel("DEFAULT", this.textBox_label.Text, 0.0);
                    }
                    else
                    {
                        this.CurrentCluster.AddNewLabel("MANUAL", this.textBox_label.Text, 1.0);
                        this.CurrentCluster.IsLabelized = true;
                    }
                }

            }
        }


        /// <summary>
        /// Handler for Previous Button Click
        /// </summary>
        private void PreviousShapes_Click(object sender, System.EventArgs e)
        {
            index = (index >= 12) ? index - 12 : 0;
            UpdateBinding();
        }


        /// <summary>
        /// Handler for Next Button Click
        /// </summary>
        private void NextShapes_Click(object sender, System.EventArgs e)
        {
            index = (index < this.Shapes.Count - 12) ? index + 12 : index;
            UpdateBinding();
        }


        /// <summary>
        /// Override ClearClustersList event to set a flag in order manage the ClusterWindow Lifetime
        /// </summary>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            this.HasBeenClosed = true;
        }
        
    }
}
