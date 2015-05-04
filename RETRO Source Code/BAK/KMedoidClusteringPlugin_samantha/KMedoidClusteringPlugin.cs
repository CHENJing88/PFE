using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plugins.Interfaces
{

    public partial class KMedoidClusteringPlugin : Form, IClusteringPlugin
    {
        public KMedoidClusteringPlugin()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Renvoie une référence sur la fenêtre de configuration du module
        /// </summary>
        /// <returns></returns>
        public Form GetConfigWindow()
        {
            return this;
        }


        public void PerformClustering(string agoraProjectDir, string clustersDir)
        {
            throw new NotImplementedException();
        }
    }
}
