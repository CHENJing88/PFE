using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Polytech.Clustering.Plugin
{
    public partial class KMedoidClusteringPlugin : Form, IClusteringPlugin
    {
        private int m_expectedNbClusters = 0;
        //List<int> m_indexSeeds = null;

        public KMedoidClusteringPlugin()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Effectue le clustering du cluster passé en paramètre
        /// </summary>
        /// <param name="clusterToProcess">Le cluster sur lequel appliquer le clustering</param>
        /// <param name="refPatterns">Les éventuels patterns de référence pour l'initialisation de l'algorithme</param>
        /// <param name="updateActualClusters"></param>
        /// <param name="indexSignature">Index de la signature à utiliser pour effectuer le clustering (-1 si non renseigné)</param>
        /// <returns></returns>
        public List<Cluster> PerformClustering(Cluster clusterToProcess, List<APattern> refPatterns = null, bool updateActualClusters = false, int indexSignature = -1)
        {
            List<Cluster> listClusters = new List<Cluster>();
            List<APattern> listMedoids = new List<APattern>();

            //Initialisation des clusters résultats en fonction de la configuration de l'algo
            if (refPatterns != null) //si l'utilisateur souhaite faire de la classfication, il a sélectionné les patterns qui vont servir de référence
            {

                //récupération des références vers ces pattern et initialisation des clusters*
                foreach (APattern pattern in refPatterns)
                {
                    //instanciation d'un nouveau cluster
                    Cluster newCluster = new Cluster("Kmed" + DateTime.Now.ToString() ,"NoPaath");
                    newCluster.AddPattern(pattern);
                    listClusters.Add(newCluster);
                    listMedoids.Add(pattern); //ajout du pattern à la liste des médoides
                }
            }
            else //sélection aléatoire de medoids
            {
                List<int> generatedNumbers = new List<int>();
                Random random = new Random();
                //génération de nombres en accord avec le nombre de clusters attendus par l'utilisateur
                for (int i = 0; i < m_expectedNbClusters; i++)
                {
                    //génération d'un index entre 0 et nombre de patterns - 1
                    int randomNumber = random.Next(0, clusterToProcess.Patterns.Count);
                    //l'index a t-il déjà été tiré ?
                    if (generatedNumbers.Contains(randomNumber))
                    {
                        i--;
                        continue; //on réitère 
                    }
                    generatedNumbers.Add(randomNumber);   //ajout du nombre à la liste des nombres déjà générés

                    //initialisation du cluster associé
                    Cluster newCluster = new Cluster("Kmed" + DateTime.Now.ToString(), "NoPaath");
                    newCluster.AddPattern(clusterToProcess.Patterns[randomNumber]);
                    listClusters.Add(newCluster);
                    listMedoids.Add(clusterToProcess.Patterns[randomNumber]); 
                }
                
            } //fin de l'initialisation

            //Répartition des patterns du cluster considéré
            //On passe en revue la liste des patterns contenus dans le cluster sur lequel le clustering est à appliquer
            foreach(APattern pattern in clusterToProcess.Patterns)
            {
                if (!listMedoids.Contains(pattern)) //si le pattern considéré n'est pas un élément de référence
                {
                    //on recherche l'élément le plus proche
                    int indexMin = GetClosest(listMedoids, pattern, indexSignature);

                    //on positionne le pattern dans le cluster représenté par l'élément le plus proche
                    listClusters[indexMin].AddPattern(pattern);
                }
            }

            return listClusters;
        }


        /// <summary>
        /// Renvoie l'index du pattern de la liste "listMedoids" le plus proche au sens de la distance euclidienne
        /// </summary>
        /// <returns>Index du pattern le plus proche</returns>
        private int GetClosest(List<APattern> listMedoids, APattern toInsert, int indexSignature = -1)
        {
            double distanceMin = double.MaxValue;
            double distanceTemp = 0.0;
            int indexMin = -1;
            for (int i = 0; i < listMedoids.Count; i++)
            {
                if(indexSignature == -1) //on considére l'intégralité des signatures
                    distanceTemp = listMedoids[i].EuclidianDistance(toInsert);
                else //on ne considére que la signature d'index "indexSignature"
                    distanceTemp = listMedoids[i].EuclidianDistance(toInsert, indexSignature);

                if (distanceTemp < distanceMin)
                {
                    distanceMin = distanceTemp;
                    indexMin = i;
                }
            }
            return indexMin;
        }


        public List<Cluster> PerformClustering( List<APattern> refPatterns = null, bool updateActualClusters = false, int indexSignature = -1)
        {
            throw new NotImplementedException();
        }

        
        public List<Cluster> PerformClustering()
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return "Partitioning Around Medoids";
        }

        public string GetAuthor()
        {
            return "Leonard Kaufman and Peter J. Rousseeuw";
        }

        public Form GetConfigWindow()
        {
            return this;
        }

        public List<string> GetInfoList()
        {
            List<String> infoList = new List<String>();

            infoList.Add("Méthode : " + GetName());
            infoList.Add("Auteur : " + GetAuthor());

            return infoList;
        }


        public IConfig GetConfig()
        {
            return null;
        }
        

        public TimeSpan GetProcessingTime()
        {
            throw new NotImplementedException();
        }


        public void SetDatabase(Database db)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
