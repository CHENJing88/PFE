/*
 * RETRO 2012 - v2.3
 * 
 * PaRADIIT Project
 * https://sites.google.com/site/paradiitproject/
 * 
 * This software is provided under LGPL v.3 license, 
 * which exact definition can be found at the following link:
 * http://www.gnu.org/licenses/lgpl.html
 * 
 * Please, contact us for any offers, remarks, ideas, etc.
 * 
 * Copyright © RFAI, LI Tours, 2011-2012
 * Contacts : rayar@univ-tours.fr
 *            ramel@univ-tours.fr
 * 
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Retro.Model;
using Polytech.Clustering.Plugin;


namespace RetroGUI.util
{

    /// <summary>
    /// Comparison methods for Cluster ArrayList according id number (asc)
    /// </summary>
    public class SortbyIdAsc : IComparer<Cluster>
    {
        /// <summary>
        /// Compare according id number (asc)
        /// </summary>
        /// <param name="x">Cluster 1</param>
        /// <param name="y">Cluster 2</param>
        /// <returns> 1 if x id greater than y id, -1 if y one is greater, 0 if same id </returns>
        public int Compare(Cluster x, Cluster y)
        {
            int idDiff = Convert.ToInt32(x.Id) - Convert.ToInt32(y.Id);
            if (idDiff  > 0)
                return (1);
            else
                if (idDiff < 0)
                    return (-1);
                else return (0);
        }

    }

    /// <summary>
    /// Comparison methods for Cluster ArrayList according id number (descs)
    /// </summary>
    public class SortbyIdDesc : IComparer<Cluster>
    {
        /// <summary>
        /// Compare according id number (desc)
        /// Calls SortbyShapeNumberAsc.Compare with the parameters reversed.
        /// </summary>
        /// <param name="x">Cluster 1</param>
        /// <param name="y">Cluster 2</param>
        /// <returns> 1 if y has more shapes than x, -1 if x has more, 0 if same number of shapes </returns>
        public int Compare(Cluster x, Cluster y)
        {
            return (new SortbyIdAsc()).Compare(y, x);
        }
    }

    /// <summary>
    /// Comparison methods for Cluster ArrayList according shape number (asc)
    /// </summary>
    public class SortbyShapeNumberAsc : IComparer<Cluster>
    {
        /// <summary>
        /// Compare according shape number (asc)
        /// </summary>
        /// <param name="x">Cluster 1</param>
        /// <param name="y">Cluster 2</param>
        /// <returns> 1 if x has more shapes than y, -1 if y has more, 0 if same number of shapes </returns>
        public int Compare(Cluster x, Cluster y)
        {
            if (x.Patterns.Count - y.Patterns.Count > 0)
                return (1);
            else
                if (x.Patterns.Count - y.Patterns.Count < 0)
                    return (-1);
                else return (0); 
        }

    }

    /// <summary>
    /// Comparison methods for Cluster ArrayList according shape number (desc)
    /// </summary>
    public class SortbyShapeNumberDesc : IComparer<Cluster>
    {
        /// <summary>
        /// Compare according id number (desc)
        /// Calls SortbyShapeNumberAsc.Compare with the parameters reversed.
        /// </summary>
        /// <param name="x">Cluster 1</param>
        /// <param name="y">Cluster 2</param>
        /// <returns> 1 if y has more shapes than x, -1 if x has more, 0 if same number of shapes </returns>
        public int Compare(Cluster x, Cluster y)
        {
            return (new SortbyShapeNumberAsc()).Compare(y, x);
        }
    }


    /*
    /// <summary>
    /// Comparison methods for ClusterEntity ArrayList according to confidence rate (asc)
    /// </summary>
    public class SortbyConfidenceRateAsc : IComparer
    {
        /// <returns> 1 if x has a greater confidence rate than y, -1 if y's is greater, 0 if same confidence rate </returns>
        public int Compare(Object x, Object y)
        {
            if (((ClusterEntity)x).getConfidenceRates()[0] - ((ClusterEntity)y).getConfidenceRates()[0] > 0)
                return (1);
            else
                if (((ClusterEntity)x).getConfidenceRates()[0] - ((ClusterEntity)y).getConfidenceRates()[0] < 0)
                    return (-1);
                else return (0);
        }
    }
    */

    /*
    /// <summary>
    /// Comparison methods for ClusterEntity ArrayList according to confidence rate (desc)
    /// </summary>
    public class SortbyConfidenceRateDesc : IComparer
    {

        /// <summary>
        /// Calls SortbyConfidenceRateAsc.Compare with the parameters reversed.
        /// </summary>
        /// <returns> 1 if x has a greater confidence rate than y, -1 if y's is greater, 0 if same confidence rate </returns>
        public int Compare(Object x, Object y)
        {
            return (new SortbyConfidenceRateAsc()).Compare(y, x);
        }

    }
    */
}

    
    
