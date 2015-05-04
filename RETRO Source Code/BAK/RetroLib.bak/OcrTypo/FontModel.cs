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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using System.ComponentModel;

namespace Retro.ocr
{
    /// <summary>
    /// Represent a font that is being considered as a model
    /// </summary>
    [Serializable]
    public class FontModel : INotifyPropertyChanged
    {

        #region Attributes

        private String _Directory;
        /// <summary>
        /// Directory of the Font Model
        /// </summary>
        public String Directory
        {
            get { return _Directory; }
            set
            {
                _Directory = value;
                NotifyPropertyChanged("Directory");
            }
        }


        private String _NormalizedName;
        /// <summary>
        /// Normalized Name of the Font Model
        /// </summary>
        public String NormalizedName
        {
            get { return _NormalizedName; }
            set
            {
                _NormalizedName = value;
                NotifyPropertyChanged("Normalized Name");
            }
        }


        private String _PublicationAuthor;
        /// <summary>
        /// Font Model Publication Author
        /// </summary>
        public String PublicationAuthor
        {
            get { return _PublicationAuthor; }
            set
            {
                _PublicationAuthor = value;
                NotifyPropertyChanged("Publication Author");
            }
        }


        private String _PublicationTitle;
        /// <summary>
        /// Font Model Publication Title
        /// </summary>
        public String PublicationTitle
        {
            get { return _PublicationTitle; }
            set
            {
                _PublicationTitle = value;
                NotifyPropertyChanged("Publication Title");
            }
        }


        private String _PublicationPlace;
        /// <summary>
        /// Font Model Publication Place
        /// </summary>
        public String PublicationPlace
        {
            get { return _PublicationPlace; }
            set
            {
                _PublicationPlace = value;
                NotifyPropertyChanged("Publication Place");
            }
        }


        private String _PublicationPrinterOrPublisher;
        /// <summary>
        /// Font Model Publication PrinterOrPublisher
        /// </summary>
        public String PublicationPrinterOrPublisher
        {
            get { return _PublicationPrinterOrPublisher; }
            set
            {
                _PublicationPrinterOrPublisher = value;
                NotifyPropertyChanged("Publication Printer or Publisher");
            }
        }


        private String _PublicationDate;
        /// <summary>
        /// Font Model Publication Date
        /// </summary>
        public String PublicationDate
        {
            get { return _PublicationDate; }
            set
            {
                _PublicationDate = value;
                NotifyPropertyChanged("Publication Date");
            }
        }


        private String _PublicationFormat;
        /// <summary>
        /// Font Model Publication Format
        /// </summary>
        public String PublicationFormat
        {
            get { return _PublicationFormat; }
            set
            {
                _PublicationFormat = value;
                NotifyPropertyChanged("Publication Format");
            }
        }


        private String _CopyLibrary;
        /// <summary>
        /// Font Model Copy Library
        /// </summary>
        public String CopyLibrary
        {
            get { return _CopyLibrary; }
            set
            {
                _CopyLibrary = value;
                NotifyPropertyChanged("Copy Library");
            }
        }


        private String _CopyCallNumber;
        /// <summary>
        /// Font Model Copy CallNumber
        /// </summary>
        public String CopyCallNumber
        {
            get { return _CopyCallNumber; }
            set
            {
                _CopyCallNumber = value;
                NotifyPropertyChanged("Copy CallNumber");
            }
        }

       
        private String _CopyDigitization;
        /// <summary>
        /// Font Model Copy Digitization
        /// </summary>
        public String CopyDigitization
        {
            get { return _CopyDigitization; }
            set
            {
                _CopyDigitization = value;
                NotifyPropertyChanged("Copy Digitization");
            }
        }


        private String _CopyCopyright;
        /// <summary>
        /// Font Model Copy Copyright
        /// </summary>
        public String CopyCopyright
        {
            get { return _CopyCopyright; }
            set
            {
                _CopyCopyright = value;
                NotifyPropertyChanged("Copy Copyright");
            }
        }


        private String _CopyCataloguerName;
        /// <summary>
        /// Font Model Copy CataloguerName
        /// </summary>
        public String CopyCataloguerName
        {
            get { return _CopyCataloguerName; }
            set
            {
                _CopyCataloguerName = value;
                NotifyPropertyChanged("Copy CataloguerName");
            }
        }


        private String _TranscriptionCharacter;
        /// <summary>
        /// Font Model Transcription Character
        /// </summary>
        public String TranscriptionCharacter
        {
            get { return _TranscriptionCharacter; }
            set
            {
                _TranscriptionCharacter = value;
                NotifyPropertyChanged("Transcription Character");
            }
        }


        private String _TranscriptionUnicode;
        /// <summary>
        /// Font Model Transcription Unicode
        /// </summary>
        public String TranscriptionUnicode
        {
            get { return _TranscriptionUnicode; }
            set
            {
                _TranscriptionUnicode = value;
                NotifyPropertyChanged("Transcription Unicode");
            }
        }


        private String _ImageFilename;
        /// <summary>
        /// Font Model (Grayscale) Image Filename
        /// </summary>
        public String ImageFilename
        {
            get { return _ImageFilename; }
            set
            {
                _ImageFilename = value;
                NotifyPropertyChanged("Image Filename");
            }
        }


        private String _ImageFolder;
        /// <summary>
        /// Font Model Image Folder
        /// </summary>
        public String ImageFolder
        {
            get { return _ImageFolder; }
            set
            {
                _ImageFolder = value;
                NotifyPropertyChanged("Image Folder");
            }
        }


        private String _ImagePage;
        /// <summary>
        /// Font Model Image Page
        /// </summary>
        public String ImagePage
        {
            get { return _ImagePage; }
            set
            {
                _ImagePage = value;
                NotifyPropertyChanged("Image Page");
            }
        }


        private String _ImageResolution;
        /// <summary>
        /// Font Model Image Resolution
        /// </summary>
        public String ImageResolution
        {
            get { return _ImageResolution; }
            set
            {
                _ImageResolution = value;
                NotifyPropertyChanged("Image Resolution");
            }
        }


        private String _ThumbnailName;
        /// <summary>
        /// Font Model Thumbnail Name
        /// </summary>
        public String ThumbnailName
        {
            get { return _ThumbnailName; }
            set
            {
                _ThumbnailName = value;
                NotifyPropertyChanged("Thumbnail Name");
            }
        }


        private String _ThumbnailWidth;
        /// <summary>
        /// Font Model Thumbnail Width
        /// </summary>
        public String ThumbnailWidth
        {
            get { return _ThumbnailWidth; }
            set
            {
                _ThumbnailWidth = value;
                NotifyPropertyChanged("Thumbnail Width");
            }
        }


        private String _ThumbnailHeight;
        /// <summary>
        /// Font Model Thumbnail Height
        /// </summary>
        public String ThumbnailHeight
        {
            get { return _ThumbnailHeight; }
            set
            {
                _ThumbnailHeight = value;
                NotifyPropertyChanged("Thumbnail Height");
            }
        }


        private String _ThumbnailPositionX;
        /// <summary>
        /// Font Model Thumbnail PositionX
        /// </summary>
        public String ThumbnailPositionX
        {
            get { return _ThumbnailPositionX; }
            set
            {
                _ThumbnailPositionX = value;
                NotifyPropertyChanged("Thumbnail PositionX");
            }
        }


        private String _ThumbnailPositionY;
        /// <summary>
        /// Font Model Thumbnail PositionY
        /// </summary>
        public String ThumbnailPositionY
        {
            get { return _ThumbnailPositionY; }
            set
            {
                _ThumbnailPositionY = value;
                NotifyPropertyChanged("Thumbnail PositionY");
            }
        }


        private String _TypographyIsSmallCap;
        /// <summary>
        /// Font Model Typography IsSmallCap
        /// </summary>
        public String TypographyIsSmallCap
        {
            get { return _TypographyIsSmallCap; }
            set
            {
                _TypographyIsSmallCap = value;
                NotifyPropertyChanged("Typography IsSmallCap");
            }
        }


        private String _TypographyType;
        /// <summary>
        /// Font Model Typography Type
        /// </summary>
        public String TypographyType
        {
            get { return _TypographyType; }
            set
            {
                _TypographyType = value;
                NotifyPropertyChanged("Typography Type");
            }
        }


        private String _TypographyAlphabet;
        /// <summary>
        /// Font Model Typography Alphabet
        /// </summary>
        public String TypographyAlphabet
        {
            get { return _TypographyAlphabet; }
            set
            {
                _TypographyAlphabet = value;
                NotifyPropertyChanged("Typography Alphabet");
            }
        }


        private String _TypographyFamily;
        /// <summary>
        /// Font Model Typography Family
        /// </summary>
        public String TypographyFamily
        {
            get { return _TypographyFamily; }
            set
            {
                _TypographyFamily = value;
                NotifyPropertyChanged("Typography Family");
            }
        }


        private String _TypographySubFamily;
        /// <summary>
        /// Font Model Typography SubFamily
        /// </summary>
        public String TypographySubFamily
        {
            get { return _TypographySubFamily; }
            set
            {
                _TypographySubFamily = value;
                NotifyPropertyChanged("Typography SubFamily");
            }
        }


        private String _TypographyBodyHeight;
        /// <summary>
        /// Font Model Typography BodyHeight
        /// </summary>
        public String TypographyBodyHeight
        {
            get { return _TypographyBodyHeight; }
            set
            {
                _TypographyBodyHeight = value;
                NotifyPropertyChanged("Typography BodyHeight");
            }
        }


        private String _TypographyThickness;
        /// <summary>
        /// Font Model Typography Thickness
        /// </summary>
        public String TypographyThickness
        {
            get { return _TypographyThickness; }
            set
            {
                _TypographyThickness = value;
                NotifyPropertyChanged("Typography Thickness");
            }
        }


        private String _DescriptionReferences;
        /// <summary>
        /// Font Model Description References
        /// </summary>
        public String DescriptionReferences
        {
            get { return _DescriptionReferences; }
            set
            {
                _DescriptionReferences = value;
                NotifyPropertyChanged("Description References");
            }
        }


        private String _DescriptionEngraver;
        /// <summary>
        /// Font Model Description Engraver
        /// </summary>
        public String DescriptionEngraver
        {
            get { return _DescriptionEngraver; }
            set
            {
                _DescriptionEngraver = value;
                NotifyPropertyChanged("Description Engraver");
            }
        }


        private String _DescriptionComments;
        /// <summary>
        /// Font Model Description Comments
        /// </summary>
        public String DescriptionComments
        {
            get { return _DescriptionComments; }
            set
            {
                _DescriptionComments = value;
                NotifyPropertyChanged("Description Comments");
            }
        }

        #endregion



        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filename">Full path of the pattern xml file</param>
        public FontModel(string filename) 
        {
            try
            {
                // Load the Font Model xml file
                XmlDocument modelXML = new XmlDocument();
                modelXML.Load(filename);

                //Assign basic attributes
                this._Directory = Path.GetDirectoryName(filename);
                this._NormalizedName = Path.GetFileNameWithoutExtension(filename);

                // Assign Publication Metadata Attributes
                this._PublicationAuthor = modelXML.GetElementsByTagName("Publication")[0].Attributes["Author"].Value;
                this._PublicationTitle = modelXML.GetElementsByTagName("Publication")[0].Attributes["Title"].Value;
                this._PublicationPlace = modelXML.GetElementsByTagName("Publication")[0].Attributes["Place"].Value;
                this._PublicationPrinterOrPublisher = modelXML.GetElementsByTagName("Publication")[0].Attributes["PrinterOrPublisher"].Value;
                this._PublicationDate = modelXML.GetElementsByTagName("Publication")[0].Attributes["Date"].Value;
                this._PublicationFormat = modelXML.GetElementsByTagName("Publication")[0].Attributes["Format"].Value;

                // Assign Copy Metadata Attributes
                this._CopyLibrary = modelXML.GetElementsByTagName("Copy")[0].Attributes["Library"].Value;
                this._CopyCallNumber = modelXML.GetElementsByTagName("Copy")[0].Attributes["CallNumber"].Value;
                this._CopyDigitization = modelXML.GetElementsByTagName("Copy")[0].Attributes["Digitization"].Value;
                this._CopyCopyright = modelXML.GetElementsByTagName("Copy")[0].Attributes["Copyright"].Value;
                this._CopyCataloguerName = modelXML.GetElementsByTagName("Copy")[0].Attributes["CataloguerName"].Value;

                // Assign Transcription Attributes
                this._TranscriptionCharacter = modelXML.GetElementsByTagName("Transcription")[0].Attributes["Character"].Value;
                this._TranscriptionUnicode = modelXML.GetElementsByTagName("Transcription")[0].Attributes["Unicode"].Value;

                // Assign Image Attributes
                this._ImageFilename = modelXML.GetElementsByTagName("Image")[0].Attributes["Filename"].Value;
                this._ImageFolder = modelXML.GetElementsByTagName("Image")[0].Attributes["Folder"].Value;
                this._ImagePage = modelXML.GetElementsByTagName("Image")[0].Attributes["Page"].Value;
                this.ImageResolution = modelXML.GetElementsByTagName("Image")[0].Attributes["Resolution"].Value;

                // Assign Thumbnail Attributes
                this._ThumbnailName = modelXML.GetElementsByTagName("Thumbnail")[0].Attributes["Name"].Value;
                this._ThumbnailWidth = modelXML.GetElementsByTagName("Thumbnail")[0].Attributes["Width"].Value;
                this._ThumbnailHeight = modelXML.GetElementsByTagName("Thumbnail")[0].Attributes["Height"].Value;
                this.ThumbnailPositionX = modelXML.GetElementsByTagName("Thumbnail")[0].Attributes["PositionX"].Value;
                this._ThumbnailPositionY = modelXML.GetElementsByTagName("Thumbnail")[0].Attributes["PositionY"].Value;

                // Assign Typography Attributes
                this._TypographyIsSmallCap = modelXML.GetElementsByTagName("Typography")[0].Attributes["IsSmallCap"].Value;
                this._TypographyType = modelXML.GetElementsByTagName("Typography")[0].Attributes["Type"].Value;
                this._TypographyAlphabet = modelXML.GetElementsByTagName("Typography")[0].Attributes["Alphabet"].Value;
                this._TypographyFamily = modelXML.GetElementsByTagName("Typography")[0].Attributes["Family"].Value;
                this._TypographySubFamily = modelXML.GetElementsByTagName("Typography")[0].Attributes["SubFamily"].Value;
                this._TypographyBodyHeight = modelXML.GetElementsByTagName("Typography")[0].Attributes["BodyHeight"].Value;
                this._TypographyThickness = modelXML.GetElementsByTagName("Typography")[0].Attributes["Thickness"].Value;

                // Assign Description Attributes
                this._DescriptionReferences = modelXML.GetElementsByTagName("Description")[0].Attributes["References"].Value;
                this._DescriptionEngraver = modelXML.GetElementsByTagName("Description")[0].Attributes["Engraver"].Value;
                this._DescriptionComments = modelXML.GetElementsByTagName("Description")[0].Attributes["Comments"].Value;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString()+"ERROR: Exception raised during parsing in FontModel.FontModel()", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //MessageBox.Show("ERROR: Exception raised during parsing in FontModel.FontModel()", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }      
        }


        /// <summary>
        /// For binding purpose
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// For binding purpose
        /// </summary>
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    } 
}

