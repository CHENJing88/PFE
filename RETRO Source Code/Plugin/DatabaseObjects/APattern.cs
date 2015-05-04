using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Polytech.Clustering.Plugin
{
    /// <summary>
    /// Abstract Class representing a generic pattern that need to be clutered or visualized  
    /// </summary>
    public abstract class APattern : IDisposable, ICloneable, INotifyPropertyChanged
    {

        private List<ASignature> m_listSignatures = new List<ASignature>();
        //The dictionary of the average signature according to the discriptor <NomDiscriptor, average signature>
        //private Dictionary<string, object> m_listSignAvg = new Dictionary<string, object>(); 
        /// <summary>
        /// Returns the list of the signatures available (in memory) for the pattern 
        /// </summary>        
        /// <returns>Returns the list of the signatures available (in memory) for the pattern</returns>
        public List<ASignature> GetSignatures
        {
            get
            {
                return m_listSignatures;
            }
            set
            {
                m_listSignatures = value;
                NotifyPropertyChanged("GetSignatures");
            }
        }

        /// <summary>
        /// Return the requested signature if available (in memory) for the pattern 
        /// </summary>
        /// <param name="signatureName">Name of the desired signature (ZERNIKE for exemple) </param>
        /// <returns>The requested signature or null if not found</returns>
        public ASignature GetSignature(String signatureName)
        {            
            ASignature sign = null; 
            
            //Look in all the signatures in memory
            for (int i = 0; i < m_listSignatures.Count; i++)
            {                
                if(m_listSignatures[i].GetName() == signatureName) sign = m_listSignatures[i];
            }           
 
            return sign;
        }

        private String m_idPart1;
        /// <summary>
        /// Identifier of the pattern composed of 2 parts : Alto EoC identifier = name of the file without extension (for exemple 0000.0000.0001) + Path to the Agora output directory without the initial letter (for exemple //mylibrary//agoraresult//Book1//Alto)        
        /// </summary>        
        public String IdPart1
        {
            get { return m_idPart1; }
            set
            {
                m_idPart1 = value;
                NotifyPropertyChanged("IdPart1");
            }
        }

        private String m_idPart2;
        /// <summary>
        /// Identifier of the pattern composed of 2 parts : Alto EoC identifier = name of the file without extension (for exemple 0000.0000.0001) + Path to the Agora output directory without the initial letter (for exemple //mylibrary//agoraresult//Book1//Alto)
        /// </summary>                
        public String IdPart2
        {
            get { return m_idPart2; }
            set
            {
                m_idPart2 = value;
                NotifyPropertyChanged("IdPart2");
            }
        }

        private Bitmap m_ImageRepresentation = null;
        /// <summary>
        /// To allow to associate a bitmap to a pattern ("null" if not available)
        /// </summary>
        /// <returns></returns>
        public Bitmap ImageRepresentation
        {
            get { return m_ImageRepresentation; }
            set
            {
                m_ImageRepresentation = value;
                NotifyPropertyChanged("ImageRepresentation");
            }
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="idPart1">L'Part 1 of the Pattern identifier (Agora Alto id) </param>
        /// <param name="idPart2">L'Part 2 of the Pattern identifier (Agora output Path) </param>
        /// <param name="listSignatures">List of signatures for thi pattern</param>
        public APattern(string idPart1, string idPart2, List<ASignature> listSignatures)
        {
            m_idPart1 = idPart1;
            m_idPart2 = idPart2;
            m_listSignatures = listSignatures;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="idPart1">L'Part 1 of the Pattern identifier (Agora Alto id) </param>
        /// <param name="idPart2">L'Part 2 of the Pattern identifier (Agora output Path) </param>
        public APattern(string idPart1, string idPart2)
        {
            m_idPart1 = idPart1;
            m_idPart2 = idPart2;            
        }
        
        /// <summary>
        /// Add a signature in the list of available signature (be careful that the order of the signature can be important)
        /// </summary>
        /// <param name="newSign">signature to add</param>
        public void AddSignature(ASignature newSign)
        {
            m_listSignatures.Add(newSign);
        }

        
        /// <summary>
        /// Save the requested signature in a file according to the pattern Id 
        /// </summary>
        /// <param name="signatureName">Name of the desired signature (ZERNIKE for exemple) </param>
        /// <returns>Return true if the requested signature has been saved in a file</returns>
        public bool SaveSignature(String signatureName)
        {
            ASignature sign = GetSignature(signatureName); 
            if(sign == null) return false;

            String filepath = this.IdPart2 + @"\" + signatureName + @"\" + this.IdPart1 + ".xml";

            // Save the signature in xml file
            XmlSerializer xs = new XmlSerializer(typeof(ASignature));
            StreamWriter wr = new StreamWriter(filepath);
            xs.Serialize(wr, sign);
            wr.Close();
         
            return true;
        }

        /// <summary>
        /// Load the requested signature from a file according to the name and pattern Id 
        /// </summary>
        /// <param name="signatureName">Name of the desired signature (ZERNIKE for exemple) </param>
        /// <returns>Return true if the requested signature has been loaded from a file</returns>
        public bool LoadSignature(String signatureName)
        {

            ASignature sign = null;
            
            String filepath = this.IdPart2 + @"\" + signatureName + @"\" + this.IdPart1 + ".xml";

            // Load the signature from xml file
            XmlSerializer xs = new XmlSerializer(typeof(ASignature));
            StreamReader wr = new StreamReader(filepath);
            sign = (ASignature)xs.Deserialize(wr);
            wr.Close();

            if (sign == null)
                return false;
            else
            {
                AddSignature(sign);
                return true;
            }
        }

        /// <summary>
        /// Computation of euclideian distance between 2 pattern using all the available signatures (in memory)
        /// </summary>
        /// <param name="pattern2">Pattrern to compare with</param>
        /// <returns>euclidian distance from pattern 2</returns>
        public double EuclidianDistance(APattern pattern2)
        {
            //double distance = 0.0;
            double diff = 0.0;
            double dist = 0.0;
            //Use all the signatures
            for (int i = 0; i<m_listSignatures.Count ; i++)
            {
                //distance for one signature
                diff = Math.Pow( GetSignatures[i].EuclidianDistance(pattern2.GetSignatures[i]), 2);
                dist += diff;

                //distance += m_listSignatures[i].EuclidianDistance(pattern2.m_listSignatures[i]);
            }
            //return distance / m_listSignatures.Count;

            return Math.Sqrt(dist);
        }

        /// <summary>
        /// Compute euclidian distance between 2 pattern using only one specifi signature
        /// </summary>
        /// <param name="pattern2">pattern to compare with</param>
        /// <param name="indexSignature">L'index of signature to use (be careful that order can be important)</param>
        /// <returns>the euclidian distance</returns>
        public double EuclidianDistance(APattern pattern2, int indexSignature)
        {
            return m_listSignatures[indexSignature].EuclidianDistance(pattern2.m_listSignatures[indexSignature]);
        }

        /// <summary>
        /// Provide info about the pattern
        /// </summary>
        /// <returns>List of Object (string, bitmap, ...)</returns>
        public abstract List<object> GetInfo();
        
        /// <summary>
        /// To free the memory with non useful data for clustering
        /// </summary>
        public void Dispose()
        {
            m_ImageRepresentation.Dispose();
            m_ImageRepresentation = null;
        }

        /// <summary>
        /// Héritage of the interface "ICloneable", to duplicate a pattern
        /// </summary>
        public abstract object Clone();

        /// <summary>
        /// Addition of the signatures in pattern2 to signatures of this pattern
        /// </summary>
        /// <param name="pattern2"> pattern to add</param>
        public abstract void SumPattern(APattern pattern2);

        /// <summary>
        /// Division of all the signatures by an integer
        /// </summary>
        /// <param name="p">integer to use for the division</param>
        /// <returns>the pattern with the new values of signatures</returns>
        internal APattern DivideSignatures(int p)
        {
            if(p!=0)
            {
                for (int i = 0; i< GetSignatures.Count;i++ )
                {
                    GetSignatures[i] = GetSignatures[i] / p;
                }
            }
            return this;
        }

        /// <summary>
        /// Remove the last signature in the list (in memory).
        /// </summary>
        public void RemoveLastSignature()
        {
            m_listSignatures.RemoveAt(m_listSignatures.Count -1);
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
