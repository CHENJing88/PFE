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

namespace RetroGUI.typography
{
    /// <summary>
    /// Display a form with book-related metadatas to fill during the creation of a font model 
    /// </summary>
    public partial class AddModelMetadataWindow : Window
    {

        /// <summary>
        /// Model MetaData structure
        /// </summary>
        private ModelMetaData modelMetadata;


        /// <summary>
        /// Constructor
        /// </summary>
        public AddModelMetadataWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.modelMetadata = new ModelMetaData();
        }


        /// <summary>
        /// Handler for a click on the next button
        /// </summary>
        private void AddModelMetadata_Button_Next_Click(object sender, RoutedEventArgs e)
        {
            // Update metamodel attributes regarding filled information
            this.modelMetadata.PublicationAuthor = this.authorTextBox.Text;
            this.modelMetadata.PublicationTitle = this.tileTextBox.Text;
            this.modelMetadata.PublicationPublicationSite = this.placeTextBox.Text;
            this.modelMetadata.PublicationPrinter = this.printerTextBox.Text;
            this.modelMetadata.PublicationDate = this.dateTextBox.Text;
            this.modelMetadata.PublicationFormat = this.formatTextBox.Text;
            this.modelMetadata.CopyLibrary = this.libraryTextBox.Text;
            this.modelMetadata.CopyPressmark = this.callNumberTextBox.Text;
            this.modelMetadata.CopyDigitization = this.digitizationTextBox.Text;
            this.modelMetadata.CopyLicense = this.licenseTextBox.Text;
            this.modelMetadata.CopyCataloguer = this.cataloguerTextBox.Text;

            // ClearClustersList this window
            this.Close();
        }


        /// <summary>
        /// Model metadata getter for the caller
        /// </summary>
        public ModelMetaData GetModelMetaData()
        {
            return this.modelMetadata;
        }
    }
}
