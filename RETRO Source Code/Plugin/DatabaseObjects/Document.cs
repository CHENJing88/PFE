using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polytech.Clustering.Plugin
{
    /// <summary>
    /// A Document (image) corresponds to the element that contains the patterns to be proccessed  
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Reference to the Dataset containing all the documents 
        /// </summary>
        private Database m_dbParent = null;

        private List<APattern> m_listPatterns = new List<APattern>();
        /// <summary>
        /// A document contains a list of extracted patterns (EoC)
        /// </summary>        
        public List<APattern> Patterns
        {
            get
            {
                return m_listPatterns;
            }
        }


        private string m_documentPath = null;
        /// <summary>
        /// Path to the file corresponding to the document (alto or image or ...)
        /// </summary>      
        public Document(string path)
        {
            m_documentPath = path;
        }

        /// <summary>
        /// To add a pattern into the document
        /// </summary>      
        public void AddPattern(APattern newPattern)
        {
            m_listPatterns.Add(newPattern);  
            // On alerte la base de données afin qu'elle puisse mettre à jour ses listes permettant d'effectuer la normalisation des données
            //m_dbParent.PatternAdded(newPattern);
        }

    }
}
