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
    public class ZernikeConfig : IConfig
    {
        /// <summary>
        /// Nombre maximum de CFEntries pouvant être contenues dans un noeud non feuille
        /// </summary>
        private int m_maxOrder = -1;
        public int MaxOrder
        {
            set { m_maxOrder = value; }
            get { return m_maxOrder; }
        }

        /// <summary>
        /// Indique le type de distance à utiliser pour placer un pattern dans un CFTree
        /// </summary>
        //   private CFTree.DistanceType m_distanceType;

        /// <summary>
        /// Maximum de CFENtries pouvant être contenues dans un noeud feuille
        /// </summary>
        //   [XmlElement("NbMaxEntriesLeaf")]
        private int m_squareHeight = -1;
        public int SquareHeight 
        {
            get { return m_squareHeight; } 
            set{m_squareHeight = value;} 
        }

        /// <summary>
        /// Seuil à appliquer pour la fusion des CFEntries
        /// </summary>
        //  [XmlElement("Threshold")]
        private int m_squareWidth = -1;
        public int SquareWidth
        {
            get { return m_squareWidth; } 
            set { m_squareWidth = value; } 
        } 

        private ZernikeConfig()
        {
        }

        public ZernikeConfig(int maxOrder, int squareWidth, int squareHeight)
        {
            m_maxOrder = maxOrder;
            m_squareHeight = squareHeight;
            m_squareWidth = squareWidth;
        }


        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void DeserializeFromXml(string path)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(ZernikeConfig) );
            TextReader textReader = new StreamReader(path);
            ZernikeConfig conf = (ZernikeConfig) deserializer.Deserialize(textReader);
            textReader.Close();

            m_maxOrder = conf.MaxOrder;
            m_squareHeight = conf.SquareHeight;
            m_squareWidth = conf.SquareWidth;
            conf = null;
        }

       public void SerializeToXml(string path)
       {
            XmlSerializer serializer = new XmlSerializer(typeof(ZernikeConfig));
            TextWriter textWriter = new StreamWriter(path);
             serializer.Serialize(textWriter, this);
             textWriter.Close();
       }    
    }
}
