/** 
 * 
 * Retro 2011
 * Namespace RetroGUI.typography
 * Class BodyHeightManager
 * 
 * 
 * 
 * @author F. RAYAR (rayar@univ-tours.fr)
 * @version 2.0
 * @date 2012
 *
 */
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RetroGUI.typography
{
    /// <summary>
    /// BodyHeight assisted measurement tool core structure and method
    /// </summary>
    public class BodyHeightManager
    {

        protected static class BodyHeightValues
        {


            public static int NB_BODY_HEIGHT = 26;

            public static List<String> bh_French_Name = new List<string>()
                                                {   "Parisienne",
                                                    "Nonpareille",
                                                    "Mignonne",
                                                    "Petit-texte",
                                                    "Gaillarde",
                                                    "Petit-romain",
                                                    "Philosophie",
                                                    "Cicero",
                                                    "Saint-augustin",
                                                    "Gros-texte",
                                                    "Gros-romain",
                                                    "Petit-parangon",
                                                    "Gros-parangon",
                                                    "Palestine",
                                                    "Petit-canon",
                                                    "Trismégiste",
                                                    "Gros-canon",
                                                    "Double-canon",
                                                    "-",
                                                    "Triple-canon",
                                                    "Grosse-nompareille",
                                                    "-",
                                                    "-",
                                                    "-",
                                                    "-",
                                                    "-"
                                                };

            public static List<String> bh_French_Code = new List<string>()
                                                {   "PEA",
                                                    "NPA",
                                                    "MIN",
                                                    "BRV",
                                                    "BRG",
                                                    "LPR",
                                                    "SMP",
                                                    "PIC",
                                                    "ENG",
                                                    "LEN",
                                                    "GPR",
                                                    "PAR",
                                                    "DBP",
                                                    "02P",
                                                    "2LE",
                                                    "2LG",
                                                    "2LD",
                                                    "CAN",
                                                    "05P",
                                                    "06P",
                                                    "07P",
                                                    "08P",
                                                    "09P",
                                                    "10P",
                                                    "11P",
                                                    "12P",
                                                };

            public static List<String> bh_English_Name = new List<string>()
                                                {   "Pearl",
                                                    "Nonpareil",
                                                    "Minion",
                                                    "Brevier",
                                                    "Bourgeois",
                                                    "Long Primer",
                                                    "Small Pica",
                                                    "Pica",
                                                    "English",
                                                    "Large English",
                                                    "Great Primer",
                                                    "Paragon",
                                                    "Double Pica",
                                                    "Two-line Pica",
                                                    "Two-line English",
                                                    "Two-line Great Primer",
                                                    "Two-line Double Pica",
                                                    "Canon",
                                                    "Five-line Pica",
                                                    "Six-line Pica",
                                                    "Seven-line Pica",
                                                    "Eight-line Pica",
                                                    "Nine-line Pica",
                                                    "Ten-line Pica",
                                                    "Eleven-line Pica",
                                                    "Twelve-line Pica"};

            public static List<int> bh_min_20 = new List<int>() { 30, 40, 44, 50, 57, 65, 70, 77, 89, 101, 110, 128, 136, 146, 180, 220, 270, 330, 380, 480, 580, 680, 780, 880, 980, 1080};
            public static List<int> bh_max_20 = new List<int>() { 39, 43, 49, 56, 64, 69, 76, 88, 100, 109, 127, 135, 145, 179, 219, 269, 329, 379, 479, 579, 679, 779, 879, 979, 1079, 1179};
            public static List<double> bh_min_x = new List<double>() { 0.1, 0.8, 0.8, 1, 1.2, 1.1, 1.2, 1.5, 1.8, 2.1, 2, 2.5, 2.5, 2.8, 3.4, 4, 4.5, 5, 5, 10, 12.5, 13.8, 13.8, 13.8, 13.8, 13.8};
            public static List<double> bh_max_x = new List<double>() { 0.7, 1, 1, 1.2, 1.5, 1.5, 1.8, 2, 2.2, 2.1, 2.5, 2.9, 3, 3, 4, 4.5, 5.5, 10, 10, 13, 13.8, 56, 56, 56, 56, 56};
            public static List<double> bh_min_colon = new List<double>() { 0.1, 1.1, 1.1, 1.3, 1.4, 1.5, 1.9, 1.9, 2.4, 3.5, 3, 4, 4, 4.7, 6, 7, 8, 9, 9, 16, 21, 21, 21, 21, 21, 21};
            public static List<double> bh_max_colon = new List<double>() { 1, 1.4, 1.6, 1.8, 2.1, 2.4, 2.5, 3, 3.2, 3.5, 3.7, 4, 4.7, 5, 7, 8, 9.8, 16, 16, 16, 21, 56, 56, 56, 56, 56};


        }

        /// <summary>
        /// Bofy Height metadata structure
        /// </summary>
        public struct bodyHeight
        {
            // Attributes
            public String french_name;
            public String french_code;
            public String english_name;
            public int min_20;
            public int max_20;
            public double min_x;
            public double max_x;
            public double min_colon;
            public double max_colon;
            public bool initialized;

            // Constructors
            // Condition (index < NB_BODY_HEIGHT) must be verified by the caller
            public bodyHeight(int index)
            {
                french_name = BodyHeightValues.bh_French_Name[index];
                french_code = BodyHeightValues.bh_French_Code[index];
                english_name = BodyHeightValues.bh_English_Name[index];
                min_20 = BodyHeightValues.bh_min_20[index];
                max_20 = BodyHeightValues.bh_max_20[index];
                min_x = BodyHeightValues.bh_min_x[index];
                max_x = BodyHeightValues.bh_max_x[index];
                min_colon = BodyHeightValues.bh_min_colon[index];
                max_colon = BodyHeightValues.bh_max_colon[index];
                initialized = true;
            }

                   
        };

        // Get the Body Height estimated to the [20] input value
        public static bodyHeight GetBHFrom20(double value)
        {
            bodyHeight bhResult = new bodyHeight();
            bhResult.initialized = false;

            if ((value < BodyHeightValues.bh_min_20[0]) || value > BodyHeightValues.bh_max_20[BodyHeightValues.NB_BODY_HEIGHT - 1])
                return bhResult;

            int index = 0;
            while (value > BodyHeightValues.bh_max_20[index])
                index++;

            if (value >= BodyHeightValues.bh_min_20[index])
                bhResult = new bodyHeight(index);

            return bhResult;

        }

        // Get the Body Height estimated to the [x] input value
        // Intervals for [x] values overlap sometimes, so the first match is returned
        public static bodyHeight GetBHFromX(double value)
        {
            bodyHeight bhResult = new bodyHeight();
            bhResult.initialized = false;

            if ((value < BodyHeightValues.bh_min_x[0]) || value > BodyHeightValues.bh_max_x[BodyHeightValues.NB_BODY_HEIGHT - 1])
                return bhResult;

            int index = 0;
            while (value > BodyHeightValues.bh_max_x[index])
                index++;

            if (value >= BodyHeightValues.bh_min_x[index])
                bhResult = new bodyHeight(index);

            return bhResult;

        }

        // Get the Body Height estimated to the [:] input value
        // Intervals for [:] values overlap sometimes, so the first match is returned
        public static bodyHeight GetBHFromColon(double value)
        {
            bodyHeight bhResult = new bodyHeight();
            bhResult.initialized = false;

            if ((value < BodyHeightValues.bh_min_colon[0]) || value > BodyHeightValues.bh_max_colon[BodyHeightValues.NB_BODY_HEIGHT - 1])
                return bhResult;

            int index = 0;
            while (value > BodyHeightValues.bh_max_colon[index])
                index++;

            if (value >= BodyHeightValues.bh_min_colon[index])
                bhResult = new bodyHeight(index);

            return bhResult;

        }

    }
}
