using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Polytech.Clustering.Plugin
{
    /// <summary>
    /// Interface for the document reader plugins
    /// </summary>
    public interface IDocumentReaderPlugin
    {

        /// <summary>
        /// Load a database in memory and compute the requested signatures at the same time
        /// </summary>
        /// <param name="descriptors">List of the Descriptor plugins to use</param>
        /// <param name="path">Path of the input data</param>
        /// <returns>The resulting dataset</returns>
        Database LoadDatabase(List<IDescriptorPlugin> descriptors, string path);


        /// <summary>
        /// Load a database in memory and compute the requested signature at the same time"
        /// </summary>
        /// <param name="descriptors">List of the Descriptor plugins to use</param>
        /// <param name="path">Path of the input data</param>
        /// <param name="mainWindow">Main form</param>
        /// <param name="changeState">State</param>
        /// <returns>The resulting dataset</returns>
        Database LoadDatabase(List<IDescriptorPlugin> descriptors, string path, Form mainWindow, System.Delegate changeState);


        /// <summary>
        /// Returns the processing time to load the dataset
        /// </summary>
        /// <returns>Time</returns>
        TimeSpan GetProcessingTime();

        /// <summary>
        /// Returns the processed dataset
        /// </summary>
        /// <returns>The dataset</returns>
        Database GetLoadedDatabase();

        /// <summary>
        /// Reference to the configuration windows
        /// </summary>
        /// <returns>the config form</returns>
        Form GetConfigWindow();

        /// <summary>
        /// To get the configuration of the plugin
        /// </summary>
        /// <returns>Configuration of the plugin</returns>
        IConfig GetConfig();
        
        /// <summary>
        /// Names of the authors
        /// </summary>
        /// <returns>Names of the authors</returns>
        string GetAuthor();

        /// <summary>
        /// Name of the Plugin
        /// </summary>
        /// <returns>The name of the Doc Reader Plugin </returns>
        string GetName();
    }
}
