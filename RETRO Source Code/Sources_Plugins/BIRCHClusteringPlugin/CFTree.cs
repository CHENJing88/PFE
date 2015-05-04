using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polytech.Clustering.Plugin
{
    /// <summary>
    /// Classe représentant un CFTree de la méthode BIRCH
    /// </summary>
    [Serializable]
    public class CFTree
    {
         
        /// <summary>
        /// Référence vers la racine du CFTree
        /// </summary>
        Node m_root = null; 

        /// <summary>
        /// Référence vers les première feuille du CFTree
        /// </summary>
        private Node m_firstLeaf = null;
        public Node FirstLeaf
        {
            get{return m_firstLeaf;}
            set { m_firstLeaf = value; }
        }

        /// <summary>
        /// Le seuil à utiliser pour la fusion des CFEntries
        /// </summary>
        private double m_threshold = 0.0;
        public double Threshold
        {
            get {return m_threshold;}
        }

        /// <summary>
        /// Nombre maximal d'entrées pouvant être contenues dans un noeud
        /// </summary>
        private int m_maxInternalNodeEntries = 0;
        public double MaxInternalNodeEntries
        {
            get{return m_maxInternalNodeEntries;}
        }


        /// <summary>
        /// Nombre maximal d'entrées pouvant être contenues dans une feuille
        /// </summary>
        private int m_maxLeafEntries = 0;
        public double MaxLeafEntries
        {
            get{return m_maxLeafEntries;}
        }

        /// <summary>
        /// Le type de distance à calculer
        /// </summary>
        private DistanceType m_distance;
        public DistanceType Distance
        {
            get { return m_distance;}
        }



        /// <summary>
        /// Enumération indiquant les fonctions distance pouvant être utilisées
        /// </summary>
        public enum DistanceType { D0 = 0, D1, D2, D3, D4 };


        /// <summary>
        /// Constructeur particulier d'un objet de CFTree
        /// </summary>
        /// <param name="maxInternalNodeEntries">Nombre maximal de CFEntries qu'un noeud non feuille peut accueillir</param>
        /// <param name="maxLeafEntries">Nombre maximal de CFEntries qu'un noeud feuille peut accueillir</param>
        public CFTree(int maxInternalNodeEntries, int maxLeafEntries, CFTree.DistanceType distanceType, double threshold)
        {
            m_root = new Node(this);
            //initialement le premier noeud correspond à la première feuille
            m_firstLeaf = m_root;
            m_maxInternalNodeEntries = maxInternalNodeEntries;
            m_maxLeafEntries = maxLeafEntries;
            m_distance = distanceType;
            m_threshold = threshold;
        }

        /// <summary>
        /// Calcule la valeur du nouveau seuil pour la construction d'un arbre.
        /// Heuristique utilisé : valeur moyenne des distances entre les CFEntries les plus proches pour chaque noeud feuille
        /// </summary>
        /// <returns>Le nouveau seuil</returns>
        public double ComputeNewThreshold()
        {
            double newThreshold = 0.0;
            Node currentNode = m_firstLeaf;
            int meanCount = 0;
            List<CFEntry> listClosest = null;

            while (currentNode != null) //on passe en revue tous les noeuds feuilles
            {
                listClosest = currentNode.ClosestEntries();

                if (listClosest.Count == 2)
                {
                    meanCount++;
                    newThreshold += listClosest[0].Distance(listClosest[1], m_distance);
                }
                currentNode = currentNode.NextLeaf;
            }

            return newThreshold/meanCount;
        }


        /// <summary>
        /// Construit, à partir d'un abre existant, une nouvelle structure utilisant le seuil indiqué en paramètre
        /// </summary>
        /// <param name="newThreshold">Nouveau seuil à utiliser pour la construction de l'arbre</param>
        /// <returns></returns>
        public CFTree Rebuild(double newThreshold)
        {
            //création du nouvel arbre 
            CFTree newTree = new CFTree(m_maxInternalNodeEntries, m_maxLeafEntries, m_distance, newThreshold);
		
            //récupération des feuilles de l'ancien arbre
		    Node leafNode = m_firstLeaf;
		
		    while(leafNode!=null) //on passe en revue toutes les feuilles
            {
                foreach (CFEntry ent in leafNode.Entries) //redistribution de chaque entrée avec le nouveau seuil
                {
                    newTree.InsertEntry(ent);
                }
			    leafNode = leafNode.NextLeaf; //on passeà à la feuille suivante
		    }
		    return newTree;
        }


        /// <summary>
        /// Déplace un pattern d'une CFEntry à une autre
        /// </summary>
        /// <param name="toMove">Le pattern à déplacer</param>
        /// <param name="source">Le cluster contenant le patter à déplacer</param>
        /// <param name="destination">Le cluster censé accueillir le patter à déplacer</param>
        public void MovePattern(APattern toMove, Cluster source, Cluster destination)
        {
            //récupération de la CFEntry dans laquelle l'élément à déplacer est contenu
            CFEntry entrySource = this.GetCFEntry(source);

            //on retire le pattern de la CFEntry source
            entrySource.RemovePattern(toMove);

            //on l'ajoute au cluster destination
            //récup d'une référence vers la CFEntry destination
            CFEntry entryDest = this.GetCFEntry(destination);
            CFEntry newEntry = new CFEntry(toMove); //instanciation d'un CFEntry comprenant uniquement le patter à déplacer
            entryDest.AddEntry(newEntry);            
        }

        /// <summary>
        /// Renvoie une référence vers la CFEntry accueillant un cluster donné
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private CFEntry GetCFEntry(Cluster source)
        {
            //on passe en revue les CFEntries contenues dans les noeuds feuilles
            Node currentNode = this.m_firstLeaf;
            while (currentNode != null)
            {
                //on retrouve l'entrée contenant le cluster donné
                foreach (CFEntry entry in currentNode.Entries)
                {
                    if (source.Equals(entry.Cluster))  //on a retrouvé la CFEntry
                        return entry;

                    currentNode = currentNode.NextLeaf;
                }
            }

            return null;
        }

        /// <summary>
        /// Ajoute une entrée contenant un ou plusieurs patterns
        /// </summary>
        /// <param name="newEntry">L'entrée à ajouter</param>
        public void InsertEntry(CFEntry newEntry)
        {
            List<CFEntry> splitResult = m_root.InsertEntry(newEntry); //appel récurisif

            if (splitResult != null) //arbre modifié
            {
                //Création d'un nouvelle racine
                m_root = new Node(this);
                m_root.Entries.Add(splitResult[0]);
                m_root.Entries.Add(splitResult[1]);
            }
        }

        /// <summary>
        /// Retourne l'integralité des clusters constitués par la méthode.
        /// </summary>
        /// <returns>La liste des clusters</returns>
        public List<Cluster> GetClusters()
        {
            //instanciation d'une liste de clusters
            List<Cluster> results = new List<Cluster>();

            Node currentNode = m_firstLeaf;
            while (currentNode != null) //on passe en revue tous les noeuds feuilles
            {
                if(currentNode.Entries.Count >0 )
                {
                    int indexCluster = 0;
                    foreach(CFEntry entry in currentNode.Entries) //et toutes ses entrées
                    {
                        results.Add(entry.Cluster);

                        indexCluster++;
                    }
                } //fin vérif de toutes les entrées
                currentNode = currentNode.NextLeaf;                
            }
            return results;
        }
    }
}
