using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polytech.Clustering.Plugin
{
    /// <summary>
    /// Class containing all the information about the data to process
    /// </summary>    
    public class Database
    {
        private List<Document> m_listDocuments = new List<Document>();

        // Variables used for the normalisation of the signatures according to the actual context

        /// List of double to save the averages of the features of the signatures
        private List<double> m_avgSignatures = null;
        /// List of list of averages (one for each signature of the pattern - a pattern can have several signature)
        private List<List<double>> m_listMeans = new List<List<double>>();

        /// Standard deviation of the data
        private List<double> m_variance = new List<double>();
        /// List of list of variance (one for each signature of the pattern - a pattern can have several signature)
        private List<List<double>> m_listVariances = new List<List<double>>();

        /// 2 List of doubles to save Min and Max values of each feature (used for MinMax normalization).
        private List<List<double>> m_listMaxValues = new List<List<double>>();
        private List<List<double>> m_listMinValue = new List<List<double>>();

        /// Total number of patterns in the data to process
        private int m_nbPatterns = 0;
        /// <summary>
        /// Get/Set Total number of patterns in the data to process
        /// </summary>
        public int NbPatterns
        {
            get { return m_nbPatterns; }
            set
            {
                m_nbPatterns = value;
            }
        }

        /// <summary>
        /// List of documents (images) containing the patterns to process 
        /// </summary>    
        public List<Document> Documents
        {
            get
            {
                return m_listDocuments;
            }
            set
            {
                m_listDocuments = value;
            }

        }

        /// <summary>
        /// To add a document into the dataset to process
        /// </summary>    
        public void AddDocument(Document newDoc)
        {
            m_listDocuments.Add(newDoc);
        }

        /// <summary>
        /// Méthod to normalized the data of the dataset by using Z-SCORE
        /// (See : http://stn.spotfire.com/spotfire_client_help/norm/norm_z_score.htm)
        /// </summary>
        public void ZScoreNormalization()
        {
            //Normlisation de l'integralité des signatures de la base de données
            foreach (Document doc in m_listDocuments)
            { 
                foreach(APattern pattern in doc.Patterns)
                {
                    for(int i = 0 ; i< pattern.GetSignatures.Count; i++)
                    {
                        List<double> nonNormalisedValues = pattern.GetSignatures[i].GetNormalisedFeatures();
                        // ATTENTION : GetNormalisedValue returns the non normalised values if no normalisation has been done before

                        List<double> normalisedValues = new List<double>();
                        // Normalisation of the data for each signature
                        for (int j = 0; j < nonNormalisedValues.Count; j++)
                        {
                            //double normalised = (radiuspattern.GetSignatures[i].Features[i].GetDoubleValue() - m_mean[i]) / Math.Sqrt(m_variance[i]);
                            normalisedValues.Add ( (nonNormalisedValues[j] - m_listMeans[i][j]) / Math.Sqrt(m_listVariances[i][j]) );

                              // For each signature, computation of the normalised value with ZScore
                        }
                        //Update the normalized values
                        pattern.GetSignatures[i].SetNormalisedFeatures(normalisedValues); //TODO surcharge d'opérateurs
                    }
                }
            }
        }

        /// <summary>
        /// Normalisation of the data by MinMax method
        /// </summary>
        public void MinMaxNormalisation()
        {
            //Normlisation of all the signatures of the database
            foreach (Document doc in m_listDocuments)
            {
                foreach (APattern pattern in doc.Patterns)
                {
                    for (int i = 0; i < pattern.GetSignatures.Count; i++)
                    {
                        // ATTENTION : GetNormalisedValue returns the none normalisez data if no normalisation has been done before
                        List<double> nonNormalizedValues = pattern.GetSignatures[i].GetNormalisedFeatures();

                        List<double> normalisedValues = new List<double>();
                        // On effectue la normalisation des données pour chaque signature
                        for (int j = 0; j < nonNormalizedValues.Count; j++)
                        {
                            //double normalised = (radiuspattern.GetSignatures[i].Features[i].GetDoubleValue() - m_mean[i]) / Math.Sqrt(m_variance[i]);
                            //normalisedValues.Add((nonNormalisedValues[j] - m_listMeans[i][j]) / Math.Sqrt(m_listVariances[i][j]));
                            normalisedValues.Add( (nonNormalizedValues[j] - m_listMinValue[i][j]) / (m_listMaxValues[i][j] - m_listMinValue[i][j]) );
                            // On calcule, pour chaque caractéristique de la signature, sa valeur normalisée grâce à la méthode ZScore
                        }
                        //Update of the normalized values
                        pattern.GetSignatures[i].SetNormalisedFeatures(normalisedValues); //TODO surcharge d'opérateurs
                    }
                }
            }
        }


        /// <summary>
        /// Updating of the normalized data when adding a new pattern
        /// </summary>
        /// <param name="newPattern">Added Pattern</param>
        public void UpdateNormalizationData(APattern newPattern)
        {
            double newMean = 0.0;

            if (m_nbPatterns == 0) //si No element in the dataset 
            {
                for (int i = 0; i < newPattern.GetSignatures.Count; i++) //enregistrement des valeurs pour chaque signature
                {
                    List<double> newMaxValues = new List<double>(); // max values
                    List<double> newMinValues = new List<double>(); // min values

                    List<double> newMeansList = new List<double>();
                    List<double> newVariancesList= new List<double>();

                    m_listMeans.Add(newMeansList); //Add a list for the averages of the signature
                    m_listVariances.Add(newVariancesList);
                    m_listMaxValues.Add(newMaxValues);
                    m_listMinValue.Add(newMinValues);
                    //Getting the features + storage in the list
                    List<double> features = newPattern.GetSignatures[i].GetNormalisedFeatures(); //Getting the non normalized features
                    for (int j = 0; j < features.Count; j++) // for eaach feature*
                    {
                        newMaxValues.Add(features[j]); //Fisrt value = max
                        newMinValues.Add(features[j]); // first value = min

                        newMeansList.Add(features[j]); //Fisrt value = Mean
                        newVariancesList.Add(0.0); // variance = 0
                    }
                }
            }
            else
            {
                //Modification of the average     Xmoy(n)=n−1[Xn+(n−1)X¯n−1].
                for (int i = 0; i < newPattern.GetSignatures.Count; i++)
                {
                    //Getting the values of the signature
                    List<double> features = newPattern.GetSignatures[i].GetNormalisedFeatures();
                    for (int j = 0; j < features.Count; j++) //Updating of means + variances for the signature
                    {
                        double featureValue = features[j];
                        // Updating of the mean for the signature i 
                        newMean = (featureValue + m_nbPatterns * m_listMeans[i][j]) / (m_nbPatterns + 1);

                        //màj écart-type
                        m_listVariances[i][j] = (m_nbPatterns - 1) * m_listVariances[i][j] / m_nbPatterns + Math.Pow((featureValue - m_listMeans[i][j]), 2) / (m_nbPatterns + 1);
                       // m_variance[i] = (m_nbPatterns - 1) * m_variance[i] / m_nbPatterns + Math.Pow((featureValue - m_mean[i]), 2) / (m_nbPatterns + 1);
                        //m_mean[i][j] = newMean;
                        m_listMeans[i][j] = newMean;

                        //màj des min-max
                        if (m_listMaxValues[i][j] < features[j])
                            m_listMaxValues[i][j] = features[j];
                        else if (m_listMinValue[i][j] > features[j])
                            m_listMinValue[i][j] = features[j];
                    }
                }
            }
            m_nbPatterns++;
        }
    }
}
