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
    /// To store the parameters of the Plugin 
    /// </summary>        
    public class AGRConfig : IConfig
    {
        private string m_elementTag = "alto:String";
        /// <summary>
        /// Element to consider in Alto Xml files
        /// </summary>
        public string ElementTag
        {
            set { m_elementTag= value; }
            get { return m_elementTag; }
        }

        private string m_styleRefTag = "CHAR";
        /// <summary>
        /// StyleRef to consider in Alto Xml files
        /// </summary>
        public string StyleRefTag
        {
            set { m_styleRefTag = value; }
            get { return m_styleRefTag; }
        }

        /// <summary>
        /// Constructor
        /// </summary>        
        /// To decide which EoC to extract as patterns to cluster 
        public AGRConfig(string elementtag, string styletag)
        {
            m_elementTag = elementtag;
            m_styleRefTag = styletag;
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
            XmlSerializer deserializer = new XmlSerializer(typeof(AGRConfig));
            TextReader textReader = new StreamReader(filename);
            AGRConfig conf = (AGRConfig) deserializer.Deserialize(textReader);
            textReader.Close();

            m_elementTag = conf.ElementTag;
            m_styleRefTag = conf.StyleRefTag;            
            conf = null;
        }

        /// <summary>
        /// Save Parameters to file
        /// </summary>
        public void SerializeToXml(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AGRConfig));
            TextWriter textWriter = new StreamWriter(filename);
             serializer.Serialize(textWriter, this);
             textWriter.Close();
        }    
    }
}
