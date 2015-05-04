using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Polytech.Clustering.Plugin
{
    /// <summary>
    /// Class embedding the plugin configuration
    /// </summary>        
    public interface IConfig //: IXmlSerializable
    {
        /// <summary>
        /// XML serialiser
        /// </summary>
        /// <param name="path"> File to save the configuration</param>
        void SerializeToXml(string path);

        /// <summary>
        /// XML deserialiser 
        /// </summary>
        /// <param name="path">File containing the serialisation</param>
        void DeserializeFromXml(string path);
    }
}
