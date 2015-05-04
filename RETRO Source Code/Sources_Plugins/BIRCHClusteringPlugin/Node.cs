using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polytech.Clustering.Plugin
{
    /// <summary>
    /// Représente un noeud (feuille ou non) d'un CFTree
    /// </summary>
    [Serializable]
    public class Node
    {
        /// <summary>
        /// La liste des CFEntries contenues dans le noeud
        /// </summary>
        List<CFEntry> m_entries = new List<CFEntry>();

        /// <summary>
        /// Référence vers l'arbre parent, permet de récupérer les informations relatives à la construction de l'arbre (distance à utiliser, nombre max d'entrées/noeud...)
        /// </summary>
        CFTree m_parentTree = null;

        /// <summary>
        /// Pointeur vers la CFEntry parent
        /// </summary>
        CFEntry m_parentEntry = null;
        public CFEntry ParentEntry
        {
            get { return m_parentEntry; }
        }

        /// <summary>
        /// Référence sur la feuille suivante dans l'arbre, utilisé uniquement dans le cas d'un noeud feuille
        /// </summary>
        Node m_nextLeaf = null;
        public Node NextLeaf
        {
            get{ return m_nextLeaf;}
            set{ m_nextLeaf = value;}
        }

        /// <summary>
        /// Référence sur la feuille précédente de l'arbre, utilisé uniquement dans le cas d'un noeud feuille
        /// </summary>
        Node m_prevLeaf = null;
        public Node PreviousLeaf
        {
            get {return m_prevLeaf;}
            set{ m_prevLeaf = value;}
        }

        public List<CFEntry> Entries
        {
            get{return m_entries;}
        }


        public Node(CFTree parentTree)
        {
            m_parentTree = parentTree;
        }

        /// <summary>
        /// Retourne le couple de CFEntries les plus proches (la métrique utilisée est celle configurée dans CFTree)
        /// </summary>
        /// <returns>Le couple des CFEntries les plus proches</returns>
        public List<CFEntry> ClosestEntries()
        {
            List<CFEntry> listEntries = new List<CFEntry>(); //liste contenant le résultat de la recherche
            if (m_entries.Count >= 2) //si au moins 2 entrées
            {
                double distMin = double.MaxValue, distTemp = double.MaxValue; //variable stockant la distance maximale entre deux entrées
                int index1 = -1, index2 = -1;
                for (int i = 0; i < m_entries.Count - 1; i++) //pour chaque CFEntry du noeud
                {
                    for (int j = i + 1; j < m_entries.Count; j++)
                    {
                        distTemp = m_entries[i].Distance(m_entries[j], m_parentTree.Distance);

                        if (distTemp < distMin)
                        {
                            distMin = distTemp;
                            index1 = i;
                            index2 = j;                            
                        }
                    }
                }
                listEntries.Add(m_entries[index1]);
                listEntries.Add(m_entries[index2]);
            }

            return listEntries;
        }
 

        /// <summary>
        /// Retourne le couple de CFEntries le plus distant (la métrique utilisée est celle configurée dans CFTree)
        /// </summary>
        /// <returns>Le couple des CFEntries les plus distants</returns>
        public List<CFEntry> FarthestEntries()
        {
            List<CFEntry> listEntries = new List<CFEntry>();
            if (m_entries.Count >= 2)
            {
                double distMax = -1.0, distTemp = -1.0; //variable stockant la distance maximale entre deux entrées
                int index1 = -1, index2 = -1;

                for (int i = 0; i < m_entries.Count - 1; i++)
                {
                    for (int j = i + 1; j < m_entries.Count; j++)
                    {
                        distTemp = m_entries[i].Distance(m_entries[j], m_parentTree.Distance);

                        if (distTemp > distMax)
                        {
                            distMax = distTemp;
                            index1 = i;
                            index2 = j;
                        }
                    }
                }
                listEntries.Add(m_entries[index1]);
                listEntries.Add(m_entries[index2]);
            }
            return listEntries;
        }

        /// <summary>
        /// Version de InsertEntry dans laquelle la division de noeud se fait au niveau concerné
        /// </summary>
        /// <param name="toInsert"></param>
        /// <returns>Liste d'entrées nouvellement créés dans le cas d'une séparation</returns>
        public List<CFEntry> InsertEntry(CFEntry toInsert)
        {
            if (m_entries.Count > 0)
            {
                //Récupération de la CFEntry la plus proche
                CFEntry closest = null;
                double distanceClosest = GetClosestEntry(toInsert, ref closest);

                //test si l'entrée la plus proche a un fils
                if (closest.ChildNode != null)
                {
                    //si oui, appel récursif
                    List<CFEntry> newEntriesAfterSplit = closest.ChildNode.InsertEntry(toInsert);

                    if (newEntriesAfterSplit == null) //pas de split, mise à jour du CFEntry
                        closest.AddEntry(toInsert);
                    else //insertion impossible, on ajoute les nouvelles CFENtries le 
                    {
                        //supression du CFEntry superflue
                        Entries.Remove(closest);

                        //ajout des nouvelles entrées
                        Entries.Add(newEntriesAfterSplit[0]);
                        Entries.Add(newEntriesAfterSplit[1]);

                        //+ màj des références vers le noeud parent
                        //this.m_parentEntry

                        if (Entries.Count > m_parentTree.MaxInternalNodeEntries) //division du noeud si le besoin s'en fait sentir
                            return Split(); //split du noeud
                    }
                    return null;
                }
                else //noeud feuille
                {
                    //on vérifie qu'il est possible d'intégrer la nouvelle entrée (utilisation du seuil)
                    //double newRadius = closest.Radius(toInsert);


                 //   double dist = closest.Distance(toInsert, this.m_parentTree.Distance);

                    if (distanceClosest < m_parentTree.Threshold) //absorption possible par la CFEntry
                    {
                        //mise à jour de l'entrée
                        closest.AddEntry(toInsert);
                        return null;
                    }
                    else //absorption impossible par l'entrée la plus proche,on ajoute la CFEntry à la suite
                    {
                        //ajout de la CFEntry à la suite
                        m_entries.Add(toInsert);
                        //màj de la référence de l'entrée sur son noeud conteneur
                        toInsert.ContainerNode = this;

                        if (m_entries.Count > m_parentTree.MaxLeafEntries)  // max de CFEntries atteint, Division du noeud
                            return Split(); //on renvoie les CFEntries nouvellement formées

                        return null; //pas de split
                    }
                }
            }
            else
            {
                //on force l'insertion
                m_entries.Add(toInsert);
                toInsert.ContainerNode = this;
                return null;
            }
        }

        private double GetClosestEntry(CFEntry ent2,ref CFEntry closestToReturn)
        {
            double distMin = Double.MaxValue, distTmp = 0;
           // CFEntry entMin = null;
            //on passe en revue les entrées du noeud
            foreach (CFEntry ent in m_entries)
            {
                distTmp = ent.Distance(ent2, m_parentTree.Distance);

                if (distTmp < distMin)
                {
                    distMin = distTmp;
                    closestToReturn = ent;
                }
            }
            return distMin;
        }

        private CFEntry GetClosestEntry(CFEntry ent2)
        {
            double distMin = Double.MaxValue, distTmp = 0;
            CFEntry entMin = null;
            //on passe en revue les entrées du noeud
            foreach (CFEntry ent in m_entries)
            {
                distTmp = ent.Distance(ent2, m_parentTree.Distance);

                if (distTmp < distMin)
                {
                    distMin = distTmp;
                    entMin = ent;
                }
            }
            return entMin;
        }

        /// <summary>
        /// Divise un noeud en deux, renvoie des CFEntries pointant vers les noeuds créés
        /// </summary>
        /// <returns></returns>
        private List<CFEntry> Split()
        {
            List<CFEntry> retEntries = new List<CFEntry>();

            //récupération de la liste des CFEntries contenues dans le noeud à séparer
            List<CFEntry> listEntries = Entries;
            //récupération des deux entrées les plus distantes
            List<CFEntry> farthestEntries = FarthestEntries();

            //On teste si le noeud est une feuille
            bool isLeaf = (Entries[0].ChildNode == null);

            //création de deux nouveaux noeuds qui vont accueillir les entrées de 'toSplit'
            Node newNode1 = new Node(m_parentTree);
            Node newNode2 = new Node(m_parentTree);

            //Màj des entrées contenues dans chaque noeud
            newNode1.InsertEntry(farthestEntries[0]);
            newNode2.InsertEntry(farthestEntries[1]);

            //création de deux nouvelles entrées pour la nouvelle racine
            CFEntry newRootEntry1 = new CFEntry(newNode1);
            CFEntry newRootEntry2 = new CFEntry(newNode2);

            //ajout des nouvelles entrées dans la liste de retour
            retEntries.Add(newRootEntry1);
            retEntries.Add(newRootEntry2);

            //dans le cas où le noeud est feuille il est nécessaire de mettre à jour les références vers les CFEntries permettant de rapidemment retrouver l'information sur la composition des clusters
            if (isLeaf)
            {
                //mise à jour des références
                //Node 1
                newNode1.m_prevLeaf = this.m_prevLeaf;
                newNode1.m_nextLeaf = newNode2;

                //changement des références du noeud précédent
                if (newNode1.m_prevLeaf != null) //si possible
                    newNode1.m_prevLeaf.m_nextLeaf = newNode1;

                //Node 2
                newNode2.m_prevLeaf = newNode1;
                newNode2.m_nextLeaf = this.m_nextLeaf;
                
                //changement de référence du noeud suivant
                if (newNode2.m_nextLeaf != null)
                    newNode2.m_nextLeaf.m_prevLeaf = newNode2;

                //màj de la référence sur le premier noeud feuille
                if (newNode1.m_prevLeaf == null)
                    m_parentTree.FirstLeaf = newNode1;
            }

            //distribution des CFEntries dans les nouveaux noeuds
            DistributeEntries(newNode1, newNode2, farthestEntries, newRootEntry1, newRootEntry2);
            return retEntries;
        }


        /// <summary>
        /// Permet de redistribuer les CFEntries d'un noeud dans deux nouveaux noeuds.
        /// Attention : Les CFEntries de la liste "farthestEntries" ne sont pas ajoutées !
        /// </summary>
        /// <param name="node1">Premier noeud à remplir</param>
        /// <param name="node2">Second noeud à remplir</param>
        public void DistributeEntries(Node node1, Node node2, List<CFEntry> farthestEntries)
        {
            //On positionne les entrées ne faisant pas partie de la liste "farthestEntries"
            foreach(CFEntry ent in this.Entries)
            {
                if (!ent.Equals(farthestEntries[0]) && !ent.Equals(farthestEntries[1]))
                {
                    //insertion dans le noeud présentant le plus de similarité, on calcule 2 distances   
                    double distNode1 = farthestEntries[0].Distance(ent, m_parentTree.Distance);
                    double distNode2 = farthestEntries[1].Distance(ent, m_parentTree.Distance);

                    if (distNode1 > distNode2)//insertion dans le noeud 2
                        node2.Entries.Add(ent);
                    else //noeud 1
                        node1.Entries.Add(ent);
                }
            }
        }

        /// <summary>
        /// Permet de redistribuer les CFEntries d'un noeud dans deux nouveaux noeuds.
        /// Attention : Les CFEntries de la liste "farthestEntries" ne sont pas ajoutées !
        /// </summary>
        /// <param name="node1">Premier noeud à remplir</param>
        /// <param name="node2">Second noeud à remplir</param>
        /// <param name="farthestEntries">Liste des entrées les plus éloignées (déjà contenues dans les noeuds)</param>
        /// <param name="parent1">Parent du noeud 1 à mettre à jour</param>
        /// <param name="parent2">Parent du noeud 2 à mettre à jour</param>
        public void DistributeEntries(Node node1, Node node2, List<CFEntry> farthestEntries, CFEntry parent1, CFEntry parent2)
        {
            //màj des référence vers les CFENtry parents
            node1.m_parentEntry = parent1;
            node2.m_parentEntry = parent2;

            //On positionne les entrées ne faisant pas partie de la liste "farthestEntries"
            foreach(CFEntry ent in this.Entries)
            {
                if (!ent.Equals(farthestEntries[0]) && !ent.Equals(farthestEntries[1]))
                {
                    //insertion dans le noeud présentant le plus de similarité, on calcule 2 distances   
                    double distNode1 = farthestEntries[0].Distance(ent, m_parentTree.Distance);
                    double distNode2 = farthestEntries[1].Distance(ent, m_parentTree.Distance);

                    if (distNode1 > distNode2)//insertion dans le noeud 2
                    {
                        node2.Entries.Add(ent);
                        ent.ContainerNode = node2;
                        parent2.AddEntry(ent);
                    }
                    else //noeud 1
                    {
                        node1.Entries.Add(ent);
                        ent.ContainerNode = node1;
                        parent1.AddEntry(ent);
                    }
                }
            }
        }
    }
}
