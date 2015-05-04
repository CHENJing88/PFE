using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plugins.Interfaces
{
    public interface IClusteringPlugin
    {

        /// <summary>
        /// Renvoie une référence sur la fenêtre de configuration du module
        /// </summary>
        /// <returns></returns>
        Form GetConfigWindow();

        /// <summary>
        /// Do the clustering
        /// </summary>
        void PerformClustering(String agoraProjectDir, String clustersDir);


    }
}
