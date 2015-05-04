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
    public class KMedoidConfig : IConfig
    {
        /// <summary>
        /// Le nombre de clusters à constituer
        /// </summary>
        private int m_nbClusters = -1;
        public int NbClusters
        {
            set { m_nbClusters= value; }
            get { return m_nbClusters; }
        }


        public KMedoidConfig(int nbClusters)
        {
            m_nbClusters = nbClusters;
        }


        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void DeserializeFromXml(string path)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(KMedoidConfig));
            TextReader textReader = new StreamReader(path);
            KMedoidConfig conf = (KMedoidConfig) deserializer.Deserialize(textReader);
            textReader.Close();

            m_nbClusters = conf.m_nbClusters;
            conf = null;
        }

       public void SerializeToXml(string path)
       {
            XmlSerializer serializer = new XmlSerializer(typeof(KMedoidConfig));
            TextWriter textWriter = new StreamWriter(path);
             serializer.Serialize(textWriter, this);
             textWriter.Close();
       }    
    }
}
