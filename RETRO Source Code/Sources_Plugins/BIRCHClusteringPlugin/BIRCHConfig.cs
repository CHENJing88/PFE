using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Polytech.Clustering.Plugin
{
    public class BIRCHConfig : IConfig
    {
        /// <summary>
        /// Nombre maximum de CFEntries pouvant être contenues dans un noeud non feuille
        /// </summary>
        private int m_maxEntriesInternal = -1 ;
        public int MaxEntriesInternalNode
        {
            get { return m_maxEntriesInternal; }
            set { m_maxEntriesInternal = value; }
        }

        /// <summary>
        /// Indique le type de distance à utiliser pour placer un pattern dans un CFTree
        /// </summary>
     //   private CFTree.DistanceType m_distanceType;

        /// <summary>
        /// Maximum de CFENtries pouvant être contenues dans un noeud feuille
        /// </summary>
        private int m_maxEntriesLeaf = -1;
        public int MaxEntriesLeafNode
        {
            get { return m_maxEntriesLeaf; }
            set { m_maxEntriesLeaf = value; }
        }

        /// <summary>
        /// Seuil à appliquer pour la fusion des CFEntries
        /// </summary>
        private double m_threshold = -1;
        public double Threshold
        {
            get { return m_threshold; }
            set { m_threshold = value; }
        }


        private BIRCHConfig()
        {
        }

        public BIRCHConfig(int maxEntriesInternal, int maxEntriesLeaf, double threshold, CFTree.DistanceType distanceType)
        {
            m_maxEntriesInternal = maxEntriesInternal;
            m_maxEntriesLeaf = maxEntriesLeaf;
            m_threshold = threshold;
        }


        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void DeserializeFromXml(string path)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(BIRCHConfig));
            TextReader textReader = new StreamReader(path);
            BIRCHConfig conf = (BIRCHConfig) deserializer.Deserialize(textReader);
            textReader.Close();

            m_maxEntriesInternal = conf.m_maxEntriesInternal;
            m_maxEntriesLeaf = conf.m_maxEntriesLeaf;
            m_threshold = conf.m_threshold;
            conf = null;
        }

        public void SerializeToXml(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BIRCHConfig));
            TextWriter textWriter = new StreamWriter(path);
            serializer.Serialize(textWriter, this);
            textWriter.Close();
        }    
    }
}
