using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Polytech.Clustering.Plugin
{
    /// <summary>
    /// Interface for Clustering plugin
    /// </summary>
    public interface IClusteringPlugin
    {
       
        /// <summary>
        /// Need to be implemented to Perform a clustering on the loaded dataset of documents containing Patterns by using cluster Representatives. 
        /// </summary>
        /// <param name="refPatterns">Representatives of the desired or existing clusters  (null  if not necessary)</param>
        /// <param name="indexSignature">Index of the signature to use for the clustering clustering (-1 to use all signatures / -2 for Pixel Matching )</param>
        /// <param name="updateActualClusters"> To know if it is a new clustering or if we should update the current clusters</param>
        /// <returns>Resulting list of clusters or "null" if not implemented in the plugin</returns>
        List<Cluster> PerformClustering(List<APattern> refPatterns = null, bool updateActualClusters = false, int indexSignature = -1);

        
        /// <summary>
        /// Need to be implemented to Perform a clustering on Patterns that have been put in a unique initial cluster with cluster representatives. 
        /// </summary>
        /// <param name="clusterToProcess">Cluster to analyze again</param>
        /// <param name="refPatterns">Representatives of the desired or existing clusters (null  if not necessary)</param>
        /// <param name="indexSignature">Index of the signature to use for the clustering clustering (-1 to use all signatures / -2 for Pixel Matching )</param>
        /// <param name="updateActualClusters"> To know if it is a new clustering or if we should update the current clusters</param>
        /// <returns>Resulting list of clusters or "null" if not implemented in the plugin</returns>
        List<Cluster> PerformClustering(Cluster clusterToProcess, List<APattern> refPatterns, bool updateActualClusters = false, int indexSignature = -1);


        /// <summary>
        /// To define the Path to clusters files
        /// </summary>
        void SetClustersDir(string clustersDir);
        
        /// <summary>
        /// To define the database to analyze
        /// </summary>
        /// <param name="db">Database to use</param>
        void SetDatabase(Database db);

        /// <summary>
        /// Get the name of the Plugin
        /// </summary>
        /// <returns>Name of the plugin</returns>
        string GetName();

        /// <summary>
        /// Names of the authors
        /// </summary>
        /// <returns>Names of the authors</returns>
        string GetAuthor();

        /// <summary>
        /// Reference to the configuration Form
        /// </summary>
        /// <returns>Reference to the configuration Form</returns>
        Form GetConfigWindow();


        /// <summary>
        /// To get information about the plugin (for exemple, its configuration)
        /// </summary>
        /// <returns>List of strings with information about the plugin</returns>
        List<String> GetInfoList();

        /// <summary>
        /// Reference on a Class embedding the configuration of the plugin
        /// </summary>
        /// <returns>Reference on a Class embedding the configuration du plugin</returns>
        IConfig GetConfig();

        /// <summary>
        /// Processing time
        /// </summary>
        /// <returns>Processing time</returns>
        TimeSpan GetProcessingTime();
    }
}
