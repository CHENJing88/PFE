using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Polytech.Clustering.Plugin
{

    /// <summary>
    /// Cette classe représente une entrée contenue dans un noeud du CFTree
    /// Chaque CFEntry correspond à un résumé d'un ou plusieurs clusters
    /// CF = {N, LS, SS} avec N le nombre d'éléments d'un (ou plusieurs) cluster(s), LS le centroid et SS la somme au carré des données
    /// Pour plus d'informations se référer à l'article de ZHang, Ramakrishnan et Livny intitulé "BIRCH: A New Data Clustering ALgorithm and Its application"
    /// </summary>
    [Serializable]
    public class CFEntry
    {
        /// <summary>
        /// Nombre d'éléments contenus dans les sous-clusters représentés par la CFEntry
        /// </summary>
        int m_nbPoints = 0;

        /// <summary>
        /// Le rayon d'un cluster représenté par le CFEntry
        /// </summary>
       // double m_radius = 0.0;

        /// <summary>
        /// Somme linéaire des caractéristiques de m_nbPoints représentés par la CFEntry
        /// </summary>
      //  List<double> m_linearSumSignature = null;

        /// <summary>
        /// Somme linéaire des caractéristiques de m_nbPoints représentés par la CFEntry, enregistré sous la forme d'une liste de ASignature
        /// </summary>
        List<ASignature> m_linearSumSignatures = new List<ASignature>();
        public List<ASignature> LinearSumSignatures
        {
            get {return m_linearSumSignatures;}
            set{m_linearSumSignatures = value;}
        }


        /// <summary>
        /// Somme des carrés des points de la base de données
        /// </summary>
        double m_squareSum = 0.0;

        /// <summary>
        /// Référence vers le noeud fils de l'entrée. Null si la CFEntry est contenue dans un noeud feuille
        /// </summary>
        private Node m_childNode = null;

        /// <summary>
        /// Référence vers le noeud contenant cette CFEntry
        /// </summary>
        private Node m_containerNode = null;
        public Node ContainerNode
        {
            get { return m_containerNode; }
            set { m_containerNode = value; }
        }

        /// <summary>
        /// Référence vers le cluster contenant les élements regroupés dans la CFEntry
        /// </summary>
        [NonSerialized]
        private Cluster m_cluster = new Cluster("CFTree","NoPath");
        public Cluster Cluster
        {
            get { return m_cluster; }
        }

        public Node ChildNode
        {
            get {return m_childNode; }
            set{m_childNode = value;}
        }


        /// <summary>
        /// Instancie un CFENtry en prenant en compte un noeud fils
        /// </summary>
        /// <param name="node"></param>
        public CFEntry(Node node)
        {
            m_childNode = node;
            if (node.Entries.Count > 0) //on passe en revue chaque CFEntry du fils si possible
            {
                //nouvelle somme linéaire des signatures
                //enregistrment de la première somme linéaires
                //instanciation de la liste représentant la somme linéaire
                m_linearSumSignatures.Add( (ASignature) node.Entries[0].m_linearSumSignatures[0].Clone());

                //ajout des autres signatures
                for (int i = 1; i < node.Entries[0].m_linearSumSignatures.Count; i++)
                {
                    m_linearSumSignatures.Add( (ASignature) node.Entries[0].m_linearSumSignatures[i].Clone() );
                   // m_linearSumSignature.Add(node.Entries[0].m_linearSumSignature[i]);
                }

                //enregistrement du nombe d'éléments de la base de données du noeud
                m_nbPoints = node.Entries[0].m_nbPoints;
                m_squareSum = node.Entries[0].m_squareSum;

                //Ajout des données contenues dans les autres CFEntries
                for (int i = 1; i < node.Entries.Count; i++)
                {
                    m_nbPoints += node.Entries[i].m_nbPoints; //ajout du nombre de points

                    //ajout de la somme linéaire
                    for (int j = 0; j < node.Entries[i].m_linearSumSignatures.Count; j++)
                    {
                        m_linearSumSignatures[j].SignatureSum(node.Entries[i].m_linearSumSignatures[j]); 
                    }

                    //Màj de SS
                    m_squareSum += node.Entries[i].m_squareSum;
                }
            }
        }

        /// <summary>
        /// Instancie un CFENtry ne contenant qu'un seul pattern
        /// </summary>
        /// <param name="pattern">Le pattern contenu dans la CFEntry</param>
        public CFEntry(APattern pattern)
        {
            m_nbPoints = 1; //initialement, un seul point de la base de données
            m_squareSum = 0.0;

            //on passe en revue l'intégralité des signatures
             m_linearSumSignatures.Add( (ASignature) pattern.GetSignatures[0].Clone());  //On utilise que la première signature 
            for (int i = 1; i < pattern.GetSignatures.Count ;i++ ) //ajout des autres signatures
            {	
                //ajout successif des signatures
                m_linearSumSignatures.Add( (ASignature) pattern.GetSignatures[i].Clone());

                foreach (double value in pattern.GetSignatures[i].GetNormalisedFeatures())
                {
                    m_squareSum += value * value;
                }
            }
            // ajout du pattern au cluster formé par la CFEntry
            m_cluster.AddPattern(pattern);
        }


        /// <summary>
        /// Permet d'ajouter les informations d'une CFEntry à une autre
        /// </summary>
        /// <param name="ent">La CFEntry à ajouter</param>
        public void AddEntry(CFEntry ent)
        {
            m_nbPoints += ent.m_nbPoints;

            //on somme les valeurs de toutes les signatures
            for (int i = 0; i < m_linearSumSignatures.Count; i++)
            {
                m_linearSumSignatures[i].SignatureSum( ent.m_linearSumSignatures[i]);
            }
            
            //ajout de la sommme au carré
            m_squareSum += ent.m_squareSum;

            //on ajoute les patterns à la suite
            m_cluster.AddPatterns(ent.m_cluster.Patterns);
        }

        public double Distance(CFEntry entry2, Polytech.Clustering.Plugin.CFTree.DistanceType dist)
        {
            switch (dist)
            {
                case Polytech.Clustering.Plugin.CFTree.DistanceType.D0:
                    return CentroidEuclidianDistance(entry2);
                case Polytech.Clustering.Plugin.CFTree.DistanceType.D1:
                    return CentroidManhattanDistance(entry2);
                case Polytech.Clustering.Plugin.CFTree.DistanceType.D2:
                    return AverageIntraClusterDistance(entry2);
            }
            return -1;
        }

        private double AverageIntraClusterDistance(CFEntry entry2)
        {
            /*double dotProduct = 0.0; //variable stockant le produit scalaire
            for(int i =0;i<m_linearSumSignature.Features.Count;i++) //pour chaque caractéristique
            {
                dotProduct += m_linearSumSignature.Features[i].GetDoubleValue() * entry2.m_linearSumSignature.Features[i].GetDoubleValue();
            }

            return (m_nbPoints*m_squareSum + entry2.m_nbPoints*entry2.m_squareSum - 2*dotProduct) / (m_nbPoints*entry2.m_nbPoints);*/
            return 0.0;
        }

        /// <summary>
        /// Calcul la distance euclidienne entre les centroïdes de de this et une autre CFEntry
        /// </summary>
        /// <param name="entry2"></param>
        /// <returns></returns>
        private double CentroidManhattanDistance(CFEntry entry2)
        {
            /*double dist = 0; //la distance finale à renvoyer
            double diff = 0; //différence entre deux caractéristiques
            int nbFeatures = m_linearSumSignature.NbFeatures;

            for (int i = 0; i < nbFeatures; i++) //pour chaque caractéristique de la CFEntry
            {
                //calcul de la différence entre chaque caractéristique
                diff = Math.Abs((this.m_linearSumSignature.Features[i].GetDoubleValue() / m_nbPoints) - (entry2.m_linearSumSignature.Features[i].GetDoubleValue() / entry2.m_nbPoints));
                dist += diff;
            }
            return dist;*/
            return 0.0;
        }

        /// <summary>
        /// Calcul la distance euclidienne entre les centroïdes de la CFEntry courant et "entry2"
        /// </summary>
        /// <param name="entry2">La CFEntry avec laquelle calculer la distance</param>
        /// <returns>LA distance calculée par la fonction</returns>
        private double CentroidEuclidianDistance(CFEntry entry2)
        {
            //différence entre deux caractéristiques
            double diff = 0.0;
            double dist = 0.0;
            //distance euclidienne entre chaque centroide
            for (int i = 0;i<m_linearSumSignatures.Count;i++ )
            {
                //distance euclidienne entre chaque signature
                diff = Math.Pow( (m_linearSumSignatures[i] / m_nbPoints).EuclidianDistance( (entry2.m_linearSumSignatures[i] / entry2.m_nbPoints) ), 2);
                dist += diff;
            }

            return Math.Sqrt(dist);
        }


        /// <summary>
        /// Calcul le rayon du cluster dans le cas où l'on intégre la CFEntry "toInsert"
        /// </summary>
        /// <param name="toInsert">CFEntry qui est hypothétiquement à insérer</param>
        /// <returns>Le rayon du cluster en prenant en compte la nouvelle CFEntry</returns>
        internal double Radius(CFEntry toInsert)
        {

            double radius = 0.0;
            //calcul du rayon en considérant l'integralité des éléments contenus dans le cluster
            //pour chaque signature
            for (int i = 0; i < m_linearSumSignatures.Count; i++) //ASignature sign in this.m_linearSumSignatures)
            {
                //signature du centroide 
                ASignature centroidSign = ((m_linearSumSignatures[i] + toInsert.m_linearSumSignatures[i]) / (m_nbPoints+toInsert.m_nbPoints));

                //ajout de la signature des patterns de "toInsert"
                foreach (APattern pattern in toInsert.m_cluster.Patterns)
                {
                    radius += pattern.GetSignatures[i].EuclidianDistance(centroidSign);
                }

                // calcul de la distance centroïde - élément
                foreach (APattern pattern in m_cluster.Patterns)
                {
                    radius += pattern.GetSignatures[i].EuclidianDistance(centroidSign);
                }
            }

            return radius / (m_nbPoints + toInsert.m_nbPoints);
        }

        /// <summary>
        /// Rayon d'une CFentry. Il s'agit de la somme des différences entre les éléments du cluster et le centroïde. 
        /// </summary>
        /// <returns>Le rayon du périmètre</returns>
        public double Radius()
        {
            //si qu'une seul élément
            if (this.m_nbPoints <= 1)
                return 0.0;

            double radius = 0.0;
            //calcul du rayon en considérant l'integralité des éléments contenus dans le cluster
            //pour chaque signature
            for(int i = 0; i < m_linearSumSignatures.Count; i++) //ASignature sign in this.m_linearSumSignatures)
            {
                //signature du centroide 
                ASignature centroidSign = m_linearSumSignatures[i] / m_nbPoints;

                // calcul de la distance centroïde - élément
                foreach (APattern pattern in m_cluster.Patterns)
                {
                    radius += pattern.GetSignatures[i].EuclidianDistance(centroidSign);
                }
            }

            return radius / m_nbPoints;
        }

        /// <summary>
        /// Retourne le diamètre d'une CFEntry. La diamètre correspond à la somme des différences entre chaque élément d'un cluster
        /// </summary>
        /// <returns></returns>
        public double Diameter()
        {
            //si qu'une seul élément
            if (this.m_nbPoints <= 1)
                return 0.0;

            return 0.0;
        }

        /// <summary>
        /// Enléve un pattern de la CFEntry. Met à jour les informations SS, LS
        /// </summary>
        /// <param name="toRemove">Le pattern à retirer</param>
        internal void RemovePattern(APattern toRemove)
        {
            //on enlève un point
            m_nbPoints--;
            
            //Màj de LS et SS
            for (int i = 0; i < m_linearSumSignatures.Count; i++) //opur chaque signature
            {
                m_linearSumSignatures[i].SignatureSub(toRemove.GetSignatures[i]);

                
                List<double> normalisedFeatures = toRemove.GetSignatures[i].GetNormalisedFeatures();
                foreach(double feature in normalisedFeatures)
                {
                    m_squareSum -= feature * feature;
                }
            }

            //suppression du pattern du cluster si noeud feuille
            if(m_childNode == null)
                m_cluster.RemovePattern(toRemove);

            if (m_containerNode.ParentEntry != null) //appel réccursif pour la suppression du pattern dans les CFENtries parents
                m_containerNode.ParentEntry.RemovePattern(toRemove);
        }
    }
}
