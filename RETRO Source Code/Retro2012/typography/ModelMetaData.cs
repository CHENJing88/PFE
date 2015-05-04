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

namespace RetroGUI.typography
{
    /// <summary>
    /// Describe metadatas of an image Publication
    /// </summary>
    public class ModelMetaData
    {

        private String _PublicationAuthor;
        /// <summary>
        /// Author
        /// </summary>
        public String PublicationAuthor
        {
            get { return _PublicationAuthor; }
            set { _PublicationAuthor = value; }
        }


        private String _PublicationTitle;
        /// <summary>
        /// Title
        /// </summary>
        public String PublicationTitle
        {
            get { return _PublicationTitle; }
            set { _PublicationTitle = value; }
        }


        private String _PublicationPublicationSite;
        /// <summary>
        /// Publication Site
        /// </summary>
        public String PublicationPublicationSite
        {
            get { return _PublicationPublicationSite; }
            set { _PublicationPublicationSite = value; }
        }

 
        private String _PublicationPrinter;
        /// <summary>
        /// Printer
        /// </summary>
        public String PublicationPrinter
        {
            get { return _PublicationPrinter; }
            set { _PublicationPrinter = value; }
        }


        private String _PublicationDate;
        /// <summary>
        /// Date
        /// </summary>
        public String PublicationDate
        {
            get { return _PublicationDate; }
            set { _PublicationDate = value; }
        }

 
        private String _PublicationFormat;
        /// <summary>
        /// Format
        /// </summary>
        public String PublicationFormat
        {
            get { return _PublicationFormat; }
            set { _PublicationFormat = value; }
        }


        private String _CopyLibrary;
        /// <summary>
        /// Library
        /// </summary>
        public String CopyLibrary
        {
            get { return _CopyLibrary; }
            set { _CopyLibrary = value; }
        }


        private String _CopyPressmark;
        /// <summary>
        /// Pressmark
        /// </summary>
        public String CopyPressmark
        {
            get { return _CopyPressmark; }
            set { _CopyPressmark = value; }
        }


        private String _CopyDigitization;
        /// <summary>
        /// Digitalization
        /// </summary>
        public String CopyDigitization
        {
            get { return _CopyDigitization; }
            set { _CopyDigitization = value; }
        }

 
        private String _CopyLicense;
        /// <summary>
        /// License
        /// </summary>
        public String CopyLicense
        {
            get { return _CopyLicense; }
            set { _CopyLicense = value; }
        }


        private String _CopyCataloguer;
        /// <summary>
        /// Cataloguer
        /// </summary>
        public String CopyCataloguer
        {
            get { return _CopyCataloguer; }
            set { _CopyCataloguer = value; }
        }


        /// <summary>
        /// Clean the attributes
        /// </summary>
        public void Clean()
        {
            this._PublicationAuthor = "";
            this._PublicationTitle = "";
            this._PublicationPublicationSite = "";
            this._PublicationPrinter = "";
            this._PublicationDate = "";
            this._PublicationFormat = "";
            this._CopyLibrary = "";
            this._CopyPressmark = "";
            this._CopyDigitization = "";
            this._CopyLicense = "";
            this._CopyCataloguer = "";
        }

    }
}
