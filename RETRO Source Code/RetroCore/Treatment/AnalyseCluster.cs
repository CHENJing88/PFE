
using Accord.Statistics.Analysis;
using Polytech.Clustering.Plugin;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;

namespace Retro.Treatment
{
    /// <summary>
    /// Analyse a cluster via visual data of patterns in the cluster and modify(delet, send to another cluster) the patterns 
    /// </summary>
    public class AnalyseCluster
    {
        #region Attributes
        /// <summary>
        /// PrincipalComponentAnalysis for PCA
        /// </summary>
        private PrincipalComponentAnalysis PCA;
        /// <summary>
        /// DescriptiveAnalysiscfor PCA
        /// </summary>
        private DescriptiveAnalysis SDA;
        /// <summary>
        /// cluster to analyse
        /// </summary>
        private Cluster m_clusterCurrent;
        public Cluster ClusterCurrent
        {
            get { return m_clusterCurrent; }
            set 
            { 
                m_clusterCurrent = value; 
            }
        }
        /// <summary>
        /// the descriptor of signature
        /// </summary>
        private String m_descriptorSignature;
        public String DescriptorSignature
        {
            get { return m_descriptorSignature; }
            set
            {
                m_descriptorSignature = value;
            }
        }

        #endregion

        #region Constructor

        public AnalyseCluster(){}

        /// <summary>
        /// Constructor with cluster
        /// </summary>
        /// <param name="ClusterAnalyse"></param>
        public AnalyseCluster(Cluster ClusterAnalyse)
        {
            m_clusterCurrent = ClusterAnalyse;
            m_descriptorSignature = "Zernike";
        }
        #endregion

        #region Analyse Compronant Principale
        /// <summary>
        /// PCA method traits the signature of cluster, the descriptor of signature is "Zernike".
        /// </summary>
        /// <returns>the matrix of projection result</returns>
        public double[,] ACP()
        {
            /*DataTable DataTableFeatures = new DataTable();
            for (int i = 0; i < FeaturesPattern[0].Count; i++)
            {
                DataTableFeatures.Columns.Add();
            }
            
            //each line is a individus
            foreach (var Features in FeaturesPattern)
            {
                DataRow dr = DataTableFeatures.NewRow();
                //dr["cFeature"] = i.ToString();
                DataTableFeatures.Rows.Add(Features.ToArray());
               
            }*/
            List<List<double>> FeaturesPattern = new List<List<double>>();
            List<APattern> PatternsClusterCurrent = ClusterCurrent.Patterns;

            foreach (APattern Pattern in PatternsClusterCurrent)
            {
                FeaturesPattern.Add(Pattern.GetSignature(DescriptorSignature).GetNormalisedFeatures());
            }
            //int p = PatternsClusterCurrent.ElementAt(0).GetSignature(DiscriptorSignature).GetNormalisedFeatures().Count;
            // Create the data source of PCA, matrix(n*p)
            double[,] sourceMatrix = new double[PatternsClusterCurrent.Count, FeaturesPattern[0].Count];
            
            //each line is a individus
            for (int i = 0; i < FeaturesPattern.Count; i++)
            {
                //each colonme is a feature
                for (int j = 0; j < FeaturesPattern[i].Count; j++)
                {
                    sourceMatrix[i,j] = FeaturesPattern[i][j];
                }
            }
            double[,] projection;
            if (ClusterCurrent.Patterns.Count != 1)
            {
                // Create and compute a new Simple Descriptive Analysis
                SDA = new DescriptiveAnalysis(sourceMatrix);//

                SDA.Compute();

                // Create the Principal Component Analysis of the data 
                PCA = new PrincipalComponentAnalysis(SDA.Source, AnalysisMethod.Center);
                PCA.Compute();

                //set the nombre of Principal Component 
                int components = 2;
                //double[,] projectionSource = (dgvProjectionSource.DataSource as DataTable).ToMatrix(out colNames);

                // Compute the projection
                projection = PCA.Transform(sourceMatrix, components);
            }
            else
            {
                projection = new double[1, 2];
                projection[0, 0] = 0;
                projection[0, 1] = 0;
            }

            return projection;
        }

        #endregion

        /// <summary>
        /// calcul the average signature of cluster(not finished)
        /// </summary>
        /// <returns>the average signature of cluster</returns>
        public double AverageClusterSignature()
        {
            double AvergSign=0.0;

            return AvergSign;
        }
    }
}
