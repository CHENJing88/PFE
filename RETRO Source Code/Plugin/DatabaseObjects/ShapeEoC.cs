using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AForge.Imaging.Filters;
using System.ComponentModel;


namespace Polytech.Clustering.Plugin
{
    /// <summary>
    /// Represent an EoC extracted from a document image. Herited from APattern Class with additional information like position and Bitmap
    /// </summary>
    public class ShapeEoC : APattern, INotifyPropertyChanged
    {
        private string m_pathToFullImage = null;
        /// <summary>
        /// Path of the associated complete image (not always set)
        /// </summary>
        public string PathToFullImage
        {
            set 
            { 
                m_pathToFullImage = value;
                NotifyPropertyChanged("PathToFullImage");
            }
            get { return m_pathToFullImage; }
        }

        /// <summary>
        /// Horizontal position  of the Eoc (not always set)
        /// </summary>
        int m_hpos = 0;

        /// <summary>
        /// Vertical Position of the EoC (not always set)
        /// </summary>
        int m_vpos = 0;

        /// <summary>
        /// Width of the EoC (not always set)
        /// </summary>
        int m_width = 0;

        /// <summary>
        /// Height of the EoC (not always set)
        /// </summary>
        int m_height = 0;

        /// <summary>
        /// Crop the EoC image in the associated document obtained from "m_pathToFullImage" 
        /// </summary>
        public void LoadEoCImage(bool withcrop=false)
        {
            try
            {
                //Loading Eoc image in the EoC bitmap
                Bitmap imageSource = (Bitmap)Bitmap.FromFile(m_pathToFullImage);
                if (withcrop)
                {
                    Crop filterCrop = new Crop(new Rectangle(m_hpos, m_vpos, m_width, m_height));
                    imageSource = filterCrop.Apply(imageSource);
                }
                ImageRepresentation = imageSource;
            }
            catch(Exception ex)
            {
                //in case of pb do nothing
                ImageRepresentation = null ;
            }
            
        }

        /// <summary>
        /// Get an object list describing the eoc
        /// </summary>
        /// <returns>List of info</returns>
        public override List<object> GetInfo()
        {
            //Get the contextual image of the eoc
            List<object> info = new List<object>();
            Bitmap contextImage = GetContext(40, 40);
            //contextImage.Save(@"F:\scolaire\PFE2\TestData\bla");
            info.Add(contextImage);

            //General information about EoC
            List<string> strInfo = new List<string>();
            strInfo.Add("Alto Identifier : " + IdPart1);
            strInfo.Add("Vertical pos : " + m_vpos + " px");
            strInfo.Add("Horizontal pos : " + m_vpos + " px");
            strInfo.Add("Length : " + m_width + " px");
            strInfo.Add("Height: " + m_height + " px");
            info.Add(strInfo);

            //Addition of info from Signature part
            foreach(ASignature sign in GetSignatures)
            {
                List<string> strSignature = new List<string>();
                strSignature.Add("Signature : " + sign.GetName());
                
                //Get list of normalised signatures
                List<String> features = sign.ToStringList();
                int i = 1;
                foreach (String feature in features)
                {
                    strSignature.Add("Carac. " +  i + " : " + feature.ToString());
                    i++;
                }
                info.Add(strSignature);
            }
            return info;
        }


        /// <summary>
        /// Return image of the EoC in his surrounding context
        /// </summary>
        /// <param name="pxWidth">Nb of pixels to add horizontally</param>        
        /// <param name="pxHeight">nb of pixels to add vertically</param>        
        /// <returns>Bitmap of the context</returns>
        public Bitmap GetContext(int pxWidth, int pxHeight)
        {
            Bitmap imageSource = (Bitmap)Bitmap.FromFile(m_pathToFullImage);

            Bitmap context = null;
            
            int contextHPos = 0, contextVPos = 0;
            //int contextHSize = 0, contextVSize = 0;

            contextHPos = m_hpos - pxWidth;
            if (contextHPos < 0 )
                contextHPos = 0;

            contextVPos = m_vpos - pxHeight;
            if (contextVPos < 0)
                contextVPos = 0;
           
            //Crop filterCrop2 = new Crop(...)
            Point upperLeft = new Point(contextHPos, contextVPos);
            Crop filterCrop = new Crop(new Rectangle(upperLeft, new Size(2 * pxWidth + m_width, 2 * pxHeight + m_height)));
            context = filterCrop.Apply(imageSource);
            //context.Save(@"F:\test");
            return context;
        }

        /// <summary>
        /// Constructor by default herited from Appettern
        /// </summary>
        /// <param name="id1">part 1 of Pattern id (AgoraAlto id)</param>        
        /// <param name="id2">part 2 of pattern id (path to alto directory)</param>     
        public ShapeEoC(string id1,string id2) : base(id1,id2)
        {
        }

        /// <summary>
        /// Constructor with signatures insertion
        /// </summary>
        /// <param name="idPart1">L'Part 1 of the Pattern identifier (Agora Alto id) </param>
        /// <param name="idPart2">L'Part 2 of the Pattern identifier (Agora output Path) </param>
        /// <param name="listSignatures">List of signatures for thi pattern</param>
        public ShapeEoC(string idPart1, string idPart2, List<ASignature> listSignatures) : base(idPart1,idPart2, listSignatures)
        {
        }

        /// <summary>
        /// Constuctor with Additional parameters   
        /// </summary>
        /// <param name="idPart1">Identifier of the EoC = Name of the image file of the pattern</param>
        /// <param name="idPart2">Identifier of the EoC = Name of the image file of the pattern</param>
        /// <param name="hpos">x position</param>        
        /// <param name="vpos">y position</param>        
        /// <param name="height">nb of pixels to add vertically</param>     
        /// <param name="width">nb of pixels to add horizontally</param>     
        /// <param name="pathToFullImage">Path to the full image</param>     
        public ShapeEoC(string idPart1, string idPart2, int hpos, int vpos, int width, int height, string pathToFullImage)
            : base(idPart1,idPart2)
        {
            m_pathToFullImage = pathToFullImage;
            m_hpos = hpos;
            m_vpos = vpos;
            m_width = width;
            m_height = height;
        }


        /// <summary>
        /// To intantiate a new EoC
        /// </summary>
        public override object Clone()
        {
            //Instanciation 
            ShapeEoC clone = new ShapeEoC(IdPart1,IdPart2,m_hpos, m_vpos, m_width, m_height, m_pathToFullImage);
            //clonage of the signature
            foreach(ASignature sign in GetSignatures)
            {
                clone.AddSignature( (ASignature) sign.Clone() );
            }
            
            return clone;
        }

        /// <summary>
        /// Add signature of pattern2 with signature of the current pattern
        /// </summary>
        public override void SumPattern(APattern pattern2)
        {
            for(int i = 0; i< GetSignatures.Count;i++) //on somme chaque signature
            {
                GetSignatures[i].SignatureSum(pattern2.GetSignatures[i]);
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
