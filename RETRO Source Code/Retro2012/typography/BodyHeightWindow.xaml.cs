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
using System.Drawing;
using Microsoft.Win32;
using System.IO;
using System.Xml;

namespace RetroGUI.typography
{
    /// <summary>
    /// Display the BodyHeight assisted measurement tool
    /// </summary>
    public partial class BodyHeightWindow : Window
    {
        #region Attributes

        private String imagepath;
        private int linewidth = 500;

        private List<Line> lines = new List<Line>();
        private Line line_20_top = new Line();
        private Line line_20_bottom = new Line();
        private Line line_x_top = new Line();
        private Line line_x_bottom = new Line();
        private Line line_colon_top = new Line();
        private Line line_colon_bottom = new Line();
        private Line currentLine = null;

        private List<TextBox> textBoxes = new List<TextBox>();
        private TextBox currentTextBox = null;

        private BodyHeightManager.bodyHeight bh_20 = new BodyHeightManager.bodyHeight();
        private BodyHeightManager.bodyHeight bh_x = new BodyHeightManager.bodyHeight();
        private BodyHeightManager.bodyHeight bh_colon = new BodyHeightManager.bodyHeight();

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        public BodyHeightWindow()
        {   
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Initialization
            BodyHeightManager bh_infos = new BodyHeightManager();
            this.InitLines(this.linewidth);
            this.InitTextBoxes();
        }


        /// <summary>
        /// Set image
        /// </summary>
        private void InitializeView()
        {
            // Assign the image
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(imagepath, UriKind.RelativeOrAbsolute);
            src.EndInit();
            this.BH_imagename.Content = this.imagepath;
            this.BH_image.Source = src;
            this.BH_canvas.Width = src.Width;
            this.BH_canvas.Height = src.Height;
            this.linewidth = (int)src.Width;

        }


        /// <summary>
        /// Initialize textbox values
        /// </summary>
        private void InitTextBoxes()
        {
            this.textBoxes.Add(this.BH_textBox_20);
            this.textBoxes.Add(this.BH_textBox_20_linespace);
            this.textBoxes.Add(this.BH_textBox_20_top);
            this.textBoxes.Add(this.BH_textBox_20_bottom);
            this.textBoxes.Add(this.BH_textBox_x);
            this.textBoxes.Add(this.BH_textBox_x_top);
            this.textBoxes.Add(this.BH_textBox_x_bottom);
            this.textBoxes.Add(this.BH_textBox_colon);
            this.textBoxes.Add(this.BH_textBox_colon_top);
            this.textBoxes.Add(this.BH_textBox_colon_bottom);
        }


        /// <summary>
        /// Initialize lines
        /// </summary>
        /// <param name="width">width of the line to draw</param>
        private void InitLines(double width)
        {
            // Initialize list of lines
            this.lines.Add(this.line_20_top);
            this.lines.Add(this.line_20_bottom);
            this.lines.Add(this.line_x_top);
            this.lines.Add(this.line_x_bottom);
            this.lines.Add(this.line_colon_top);
            this.lines.Add(this.line_colon_bottom);

            // Initialize lines common final attributes
            foreach (Line line in lines)
            {
                line.X1 = 0;
                line.X2 = width;
                line.Visibility = Visibility.Hidden;
                this.BH_canvas.Children.Add(line);
            }

            // Define color for each line
            line_20_top.Stroke = new SolidColorBrush(Colors.Red);
            line_20_bottom.Stroke = new SolidColorBrush(Colors.Red);
            line_x_top.Stroke = new SolidColorBrush(Colors.Green);
            line_x_bottom.Stroke = new SolidColorBrush(Colors.Green);
            line_colon_top.Stroke = new SolidColorBrush(Colors.Blue);
            line_colon_bottom.Stroke = new SolidColorBrush(Colors.Blue);

            // Define StrokeArray for Bottom lines
            line_20_bottom.StrokeDashArray = new DoubleCollection(new double[] { 5 });
            line_x_bottom.StrokeDashArray = new DoubleCollection(new double[] { 5 });
            line_colon_bottom.StrokeDashArray = new DoubleCollection(new double[] { 5 });

        }


        /// <summary>
        /// Reset lines positions
        /// </summary>
        private void ResetLines()
        {
            foreach (Line line in lines)
            {
                line.X1 = 0;
                line.X2 = this.linewidth;
                line.Y1 = 0;
                line.Y2 = 0;
                line.Visibility = Visibility.Hidden;
            }
        }


        /// <summary>
        /// Reset textbox values
        /// </summary>
        private void ResetTextBoxes()
        {
            foreach (TextBox tb in textBoxes)
            {
                tb.Text = "0";
            }
        }


        /// <summary>
        /// Reset BodyHeight evaluated values
        /// </summary>
        private void ResetBHResults()
        {
            this.BH_result_20.Content = "- Not Defined -";
            this.BH_result_x.Content = "- Not Defined -";
            this.BH_result_colon.Content = "- Not Defined -";
        }


        /// <summary>
        /// Convert WPF measure to millimeters
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double WPFUnit2mm(double value)
        {
            return (value * 25.4 / 90);
        }


        /// <summary>
        /// Estimate BodyHeight regarding current lines positions
        /// </summary>
        private void UpdateBodyHeight()
        {
            // Conversion Rules
            // Mouse Position, Width and Height are given in independant unit
            // 1 unit = 1/96 inch
            // 1 inch = 25.4mm

            // Update [20] value
            double d20_top = Convert.ToDouble(this.BH_textBox_20_top.Text);
            double d20_bottom = Convert.ToDouble(this.BH_textBox_20_bottom.Text);
            double d20_value = d20_bottom - d20_top;
            d20_value = d20_value * 25.4 / 96;                                          // convert in mm 
            d20_value = Math.Round(d20_value, 2, MidpointRounding.AwayFromZero);        // round to 2 decimals 
            if (d20_value > 0)
            {
                this.BH_textBox_20.Text = Convert.ToString(d20_value);
            }
            // Update [20] estimated BH
            this.bh_20 = BodyHeightManager.GetBHFrom20(d20_value);
            if (this.bh_20.initialized)
            {
                this.BH_result_20.Content = this.bh_20.english_name + " (" + (int)d20_value + ")";
            }
            else
            {
                this.BH_result_20.Content = "- Not Defined -";
            }


            // Update [x] value
            double x_top = Convert.ToDouble(this.BH_textBox_x_top.Text);
            double x_bottom = Convert.ToDouble(this.BH_textBox_x_bottom.Text);
            double x_value = x_bottom - x_top;
            x_value = x_value * 25.4 / 96;                                          // convert in mm 
            x_value = Math.Round(x_value, 2, MidpointRounding.AwayFromZero);        // round to 2 decimals 
            if (x_value > 0)
            {
                this.BH_textBox_x.Text = Convert.ToString(x_value);
            }
            // Update [x] estimated BH
            this.bh_x = BodyHeightManager.GetBHFromX(x_value);
            if (this.bh_x.initialized)
            {
                this.BH_result_x.Content = this.bh_x.english_name + " (" + (int)x_value + ")";
            }
            else
            {
                this.BH_result_x.Content = "- Not Defined -";
            }


            // Update [:] value
            double colon_top = Convert.ToDouble(this.BH_textBox_colon_top.Text);
            double colon_bottom = Convert.ToDouble(this.BH_textBox_colon_bottom.Text);
            double colon_value = colon_bottom - colon_top;
            colon_value = colon_value * 25.4 / 96;                                          // convert in mm 
            colon_value = Math.Round(colon_value, 2, MidpointRounding.AwayFromZero);        // round to 2 decimals 
            if (colon_value > 0)
            {
                this.BH_textBox_colon.Text = Convert.ToString(colon_value);
            }
            // Update [:] estimated BH
            this.bh_colon = BodyHeightManager.GetBHFromColon(colon_value);
            if (this.bh_colon.initialized)
            {
                this.BH_result_colon.Content = this.bh_colon.english_name + " (" + (int)colon_value + ")";
            }
            else
            {
                this.BH_result_colon.Content = "- Not Defined -";
            }

        }


        /// <summary>
        /// Export in xml files
        /// </summary>
        private void WriteXMLOutput()
        {
            // Get Directory 
            String outputPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\BODY HEIGHT";
            if (!Directory.Exists(outputPath)) 
                Directory.CreateDirectory(outputPath);

            // Build filename
            int d20_value = (int)Convert.ToDouble(this.BH_textBox_20.Text);
            String outputName = System.IO.Path.GetFileNameWithoutExtension(this.imagepath) + "_" + this.bh_20.french_code + "_" + d20_value;
            //String outputName = "bhOutput.xml";

            // Save body height info in XML file            
            FileStream fs = new FileStream(outputPath + @"\" + outputName + ".xml", FileMode.Create);

            // Create XML writer
            XmlTextWriter xmlOut = new XmlTextWriter(fs, Encoding.Unicode);

            // use indenting for readability
            xmlOut.Formatting = Formatting.Indented;

            // start document
            xmlOut.WriteStartDocument();
            xmlOut.WriteComment("RETRO Body Height file");

            // main node
            xmlOut.WriteStartElement("BodyHeight");

                // Filename node
                xmlOut.WriteStartElement("Filename");
                xmlOut.WriteAttributeString("Path", this.imagepath);
                xmlOut.WriteEndElement();

                // Label node
                // Default: [20] information
                xmlOut.WriteStartElement("Typography");
                xmlOut.WriteAttributeString("FrenchName", this.bh_20.french_name);
                xmlOut.WriteAttributeString("FrenchCode", this.bh_20.french_code);
                xmlOut.WriteAttributeString("EnglishName", this.bh_20.english_name);
                    // [20] node
                    xmlOut.WriteStartElement("[20]");
                    xmlOut.WriteAttributeString("Value", this.BH_textBox_20.Text);
                    xmlOut.WriteEndElement();
                    // [x] node
                    xmlOut.WriteStartElement("[x]");
                    xmlOut.WriteAttributeString("Value", this.BH_textBox_x.Text);
                    xmlOut.WriteEndElement();
                    // [x] node
                    xmlOut.WriteStartElement("[:]");
                    xmlOut.WriteAttributeString("Value", this.BH_textBox_colon.Text);
                    xmlOut.WriteEndElement();
                xmlOut.WriteEndElement();

            xmlOut.WriteEndElement();

            // close file
            xmlOut.Close();
        }


        /// <summary>
        /// Handler fot BodyHeight radioboxex
        /// </summary>
        private void BH_RadioBox_Select(object sender, RoutedEventArgs e)
        {
            if (this.imagepath != null)
            {
                if (this.BH_radioButton_20_Top.IsChecked == true)
                {
                    this.currentLine = this.line_20_top;
                    this.currentTextBox = this.BH_textBox_20_top;
                }
                else if (this.BH_radioButton_20_Bottom.IsChecked == true)
                {
                    this.currentLine = this.line_20_bottom;
                    this.currentTextBox = this.BH_textBox_20_bottom;
                }
                else if (this.BH_radioButton_x_Top.IsChecked == true)
                {
                    this.currentLine = this.line_x_top;
                    this.currentTextBox = this.BH_textBox_x_top;
                }
                else if (this.BH_radioButton_x_Bottom.IsChecked == true)
                {
                    this.currentLine = this.line_x_bottom;
                    this.currentTextBox = this.BH_textBox_x_bottom;
                }
                else if (this.BH_radioButton_colon_Top.IsChecked == true)
                {
                    this.currentLine = this.line_colon_top;
                    this.currentTextBox = this.BH_textBox_colon_top;
                }
                else if (this.BH_radioButton_colon_Bottom.IsChecked == true)
                {
                    this.currentLine = this.line_colon_bottom;
                    this.currentTextBox = this.BH_textBox_colon_bottom;
                }
                else { }

                this.currentLine.Visibility = Visibility.Visible;
            }
        }


        /// <summary>
        /// Handler for MouseMove on the canvas
        /// </summary>
        private void BH_Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentLine != null)
            {
                // Get mouse position
                UIElement el = (UIElement)this.BH_canvas;
                System.Windows.Point point = e.MouseDevice.GetPosition(el);

                // Draw Line
                this.currentLine.Y1 = point.Y;
                this.currentLine.Y2 = point.Y;
                this.BH_canvas.UpdateLayout();

                this.currentTextBox.Text = Convert.ToString(point.Y);
            }
        }


        /// <summary>
        /// Handler for the Mouse LeftButton click on the canvas
        /// </summary>
        private void BH_Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Set the currentLine to null to freeze the lines
            this.currentLine = null;
            this.UpdateBodyHeight();
        }


        /// <summary>
        /// Handler for the mouse wheel in the image  
        /// Increase/Decrease zoom
        /// </summary>
        void BH_Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((this.BH_zoom_slider.IsEnabled) && (Keyboard.Modifiers == ModifierKeys.Control))
            {
                if (e.Delta > 0) // UP
                    this.BH_zoom_slider.Value = (this.BH_zoom_slider.Value < 5) ? this.BH_zoom_slider.Value + 0.5 : this.BH_zoom_slider.Value;
                else // DOWN
                    this.BH_zoom_slider.Value = (this.BH_zoom_slider.Value > 0.5) ? this.BH_zoom_slider.Value - 0.5 : this.BH_zoom_slider.Value;
            }
        }


        /// <summary>
        /// Handler for Open Button
        /// </summary>
        private void BH_Button_Open_Click(object sender, RoutedEventArgs e)
        {
            // Open Show Image Dialog
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = this.imagepath;
            ofd.Filter = "Image Files|*.bmp;*.jpg;*.png;*.tiff|All Files|*.*";
            ofd.ShowDialog();

            // Update the BodyHeightWindows
            if (ofd.SafeFileName.Length != 0)
            {
                this.imagepath = ofd.FileName;
                this.InitializeView();
                this.ResetLines();
                this.ResetTextBoxes();
                this.ResetBHResults();
                this.BH_zoom_slider.Value = 1;
            }
        }


        /// <summary>
        /// Handler for Reset Button
        /// </summary>
        private void BH_Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            this.ResetLines();
            this.ResetTextBoxes();
            this.ResetBHResults();
            this.BH_zoom_slider.Value = 1;
        }

        /// <summary>
        /// Handler for Export Button
        /// </summary>
        private void BH_Button_Export_Click(object sender, RoutedEventArgs e)
        {
            if (this.imagepath != null)
            {
                WriteXMLOutput();
                MessageBox.Show("Body Height Information Exported");
            }
        }

    }
}
