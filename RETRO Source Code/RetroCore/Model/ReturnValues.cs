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

namespace Retro.Model
{
    /// <summary>
    /// Define all the Return Values of the Retro Application
    /// </summary>
    [Serializable]
    public static class ReturnValues
    {
        /// <summary>
        /// Enumeration {Ok, FileDoesNotExist, NotXmlFile, XmlDeserializeError}
        /// </summary>
        public enum OpenProject
        {
            Ok = 0,
            FileDoesNotExist,
            NotXmlFile,
            XmlDeserializeError,

        }


        /// <summary>
        /// Human-readable message associated to the return values
        /// </summary>
        public static String[] OpenProjectErrorMessage =
        {
            "",
            "File doesn't exist",
            "Not an xml file",
            "Error deserialiazing xml file",
        };

    }
}
