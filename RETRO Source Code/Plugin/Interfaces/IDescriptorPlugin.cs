using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Polytech.Clustering.Plugin
{
    /// <summary>
    /// Interface of the Descriptor Plugin
    /// </summary>        
    public interface IDescriptorPlugin
    {
/*
        /// <summary>
        /// Chemin vers le dossier contenant les formes à classifier
        /// </summary>
        // private string m_path;

        /// <summary>
        /// List of the Patterns to analyze
        /// </summary>
        //private List<Pattern> m_listPatterns;

        /// <summary>
        /// Constructeur d'un plugin de type descripteur
        /// </summary>
        /// <param name="path">Path of the data to analyse (not clear !!! jy !!!)</param>
        // public IDescriptorPlugin(String path);
*/

        /// <summary>
        /// Compute and append a signature to a pattern 
        /// <param name="toModify">Pattern to analyze and modify</param>
        /// </summary>
        void CalculateSignature(APattern toModify);

        /// <summary>
        /// Get the information about the plugin / descriptor (for exemple, its configuration)
        /// </summary>
        /// <returns>Information about the descriptor plugin</returns>
        List<String> GetInfoList();


        /// <summary>
        /// Name of the Descriptor Plugin
        /// </summary>
        /// <returns>Name of the Descriptor Plugin</returns>
        string GetName();

        /// <summary>
        /// Author of the plugin
        /// </summary>
        /// <returns>Author of the plugin</returns>
        string GetAuthor();

        /// <summary>
        /// To get the configuration of the plugin
        /// </summary>
        /// <returns>Configuration of the plugin</returns>
        IConfig GetConfig();
        
        /// <summary>
        /// Reference to the configuration Form
        /// </summary>
        /// <returns>Reference to the configuration Form</returns>
        Form GetConfigWindow();
    }
}
