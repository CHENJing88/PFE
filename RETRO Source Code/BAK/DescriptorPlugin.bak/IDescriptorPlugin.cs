using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polytech.Clustering.Plugin
{
    public interface IDescriptorPlugin
    {
        /// <summary>
        /// Chemin vers le dossier contenant les formes à classifier
        /// </summary>
        //  private string m_path;

        /// <summary>
        /// Liste des formes de la base de données à partionner
        /// </summary>
        //private List<Pattern> m_listPatterns;

        /// <summary>
        /// Constructeur d'un plugin de type descripteur
        /// </summary>
        /// <param name="path">Chemin vers le dossier contenant les éléments de la base de données</param>
        // public IDescriptorPlugin(String path);

        /// <summary>
        /// Permet de calculer les signatures de la liste de formes associée au plugin
        /// </summary>
        void CalculateSignatures();

        /// <summary>
        /// Renvoie sous formes de string les informations relatives au descripteur (par exemple, sa configuration)
        /// </summary>
        /// <returns>Une liste de chaînes de caractères représentant toutes les informations relatives au descripteur</returns>
        List<String> GetInfoList();
    }
}
