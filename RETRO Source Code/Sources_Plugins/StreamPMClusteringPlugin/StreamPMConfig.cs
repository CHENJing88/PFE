using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Polytech.Clustering.Plugin
{
    /// <summary>
    /// To store the parameters of the Pixel Matching Clustering 
    /// </summary>        
    public class StreamPMConfig : IConfig
    {
        private double m_similarityThreshold = 0.8;
        /// <summary>
        /// Threshold on similarity measure
        /// </summary>
        public double Threshold
        {
            set { m_similarityThreshold= value; }
            get { return m_similarityThreshold; }
        }

        private bool m_useNoiseRemoval = false;
        /// <summary>
        /// To decide if noise removal preprocessing should be done 
        /// </summary>
        public bool NoiseRemoval
        {
            set { m_useNoiseRemoval = value; }
            get { return m_useNoiseRemoval; }
        }

        private int m_sizeOfNormalisation = 20;
        /// <summary>
        /// To decide the size (width) of the normalisation (-1 if no normalisation is needed) 
        /// </summary>
        public int NormalisationSize
        {
            set { m_sizeOfNormalisation = value; }
            get { return m_sizeOfNormalisation; }
        }
        

        /// <summary>
        /// Constructor
        /// </summary>        
        /// To store the parameters of the Pixel Matching Clustering 
        /// To decide if noise removal preprocessing should be done 
        /// To decide the size (width) of the normalisation (-1 if no normalisation is needed) 
        public StreamPMConfig(double threshold, bool noiseremoval, int sizenormalisation)
        {
            m_similarityThreshold = threshold;
            m_useNoiseRemoval = noiseremoval;
            m_sizeOfNormalisation = sizenormalisation;
        }

        /// <summary>
        /// XML checker
        /// </summary>       
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Get the parameters from file
        /// </summary>
        public void DeserializeFromXml(string filename)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(StreamPMConfig));
            TextReader textReader = new StreamReader(filename);
            StreamPMConfig conf = (StreamPMConfig) deserializer.Deserialize(textReader);
            textReader.Close();

            m_similarityThreshold = conf.m_similarityThreshold;
            m_sizeOfNormalisation = conf.NormalisationSize;
            m_useNoiseRemoval = conf.NoiseRemoval;
            conf = null;
        }

        /// <summary>
        /// Save Parameters to file
        /// </summary>
        public void SerializeToXml(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(StreamPMConfig));
            TextWriter textWriter = new StreamWriter(filename);
             serializer.Serialize(textWriter, this);
             textWriter.Close();
        }    
    }
}
