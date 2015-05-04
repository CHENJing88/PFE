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

namespace TestModule
{

    /// <summary>
    /// Describe the parameters considered regarding the Template Matching Algorithm
    /// </summary>
    public class TemplateMatchingParameters
    {
        public string NAME = "Template Matching";
        public float TEMPLATE_MATCHING_THRESHOLD = 0.85f;
        public bool BINARIZE_FOR_COMPARISON = true;
        public bool NORMALIZE_BEFORE_COMPARISON = false;
        public bool DENOISE_BOUNDING_BOX = false;

        public List<string> getListOfParameters()
        {
            List<string> listOfTemplateMatchingParameters = new List<string>();
            listOfTemplateMatchingParameters.Add(NAME);
            listOfTemplateMatchingParameters.Add(TEMPLATE_MATCHING_THRESHOLD.ToString());
            listOfTemplateMatchingParameters.Add(BINARIZE_FOR_COMPARISON.ToString());
            listOfTemplateMatchingParameters.Add(NORMALIZE_BEFORE_COMPARISON.ToString());
            listOfTemplateMatchingParameters.Add(DENOISE_BOUNDING_BOX.ToString());

            return listOfTemplateMatchingParameters;
        }
    }
}
