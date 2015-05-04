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
using System.IO;
using System.Xml;

namespace RetroGUI.export
{
    /// <summary>
    /// Define EoC TranscriptionPanel Exportation Method.
    /// Not in Retro Lib to allow the use of this functionaliy outside of a Retro project.
    /// </summary>
    public class ExportEoCTranscriptionTool
    {

        /// <summary>
        /// Export EoC Transcription method.
        /// Existence of the directories set as parameters must be checked by the calling method.
        /// Only TextBlock EoC are considered by default, g_ExportTextLine should be set to true for TextLine consideration.
        /// </summary>
        /// <param name="altoFilesDir">Path of alto files directory</param>
        /// <param name="outputAnnotationsTopDir">Output annotations topDirectory</param>
        public static void ExportEoCTranscription(String altoFilesDir, String outputAnnotationsTopDir)
        {
            // Put the following boolean to true, if you want to export TextLine transcription in separate files
            bool g_ExportTextLine = false;

             // Get the Alto xml files from directory
            List<String> altoFiles = new List<String>(Directory.GetFiles(altoFilesDir, "*.xml"));

            foreach (String alto in altoFiles)
            {
                // Create page subfolder
                String ID = Path.GetFileNameWithoutExtension(alto);
                Directory.CreateDirectory(outputAnnotationsTopDir + @"/" + ID);

                // Load alto file
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(alto);

                // Check if the xml file is an alto xml file
                if (doc.DocumentElement.Name.CompareTo("alto:alto") == 0)
                {
                    // Get the TextBlock list
                    XmlNodeList textblockList = doc.GetElementsByTagName("alto:TextBlock");

                    // Process each TextBlock
                    foreach (XmlNode textblock in textblockList)
                    {
                        String textblockTranscription = "";

                        // Get the TextLine child
                        XmlNodeList textlineList = textblock.ChildNodes;
                        foreach (XmlNode textline in textlineList)
                        {
                            if (textline.Name.CompareTo("alto:TextLine") != 0)
                                continue;

                            String textlineTranscription = "";

                            XmlNodeList stringList = textline.ChildNodes;
                            int currentWord = 0;
                            foreach (XmlNode _string in stringList)
                            {
                                if (_string.Name.CompareTo("alto:String") != 0)
                                    continue;

                                // Get the numero of the word in the line
                                String _stringID = _string.Attributes["ID"].Value;
                                _stringID = _stringID.Substring(0, _stringID.LastIndexOf('.'));
                                _stringID = _stringID.Substring(_stringID.LastIndexOf('.') + 1);
                                if (Convert.ToInt32(_stringID) != currentWord)
                                {
                                    // Add space between words
                                    textlineTranscription += " ";
                                    currentWord++;
                                }

                                textlineTranscription += _string.Attributes["CONTENT"].Value;
                            }

                            // Export TextLine transcription
                            textlineTranscription += "\n";
                            if (g_ExportTextLine)
                                ExportEoCTranscriptionTool.Export(outputAnnotationsTopDir + @"/" + ID, textline.Attributes["ID"].Value, textlineTranscription);

                            // Add current TextLine transcription to the current TextBlock
                            textblockTranscription += textlineTranscription;
                        }

                        // Export TextBlock transcription
                        ExportEoCTranscriptionTool.Export(outputAnnotationsTopDir + @"/" + ID, textblock.Attributes["ID"].Value, textblockTranscription);
                    }
                }
            }
        } 


        /// <summary>
        /// Write EoC Transcription in file
        /// </summary>
        /// <param name="outputdir">Path of the current EoC annotation directory</param>
        /// <param name="eocID">Id of the current EoC</param>
        /// <param name="eocTranscription">Transcription of the currnt EoC</param>
        public static void Export(String outputdir, String eocID, String eocTranscription)
        {
            // create a writer and open the file
            TextWriter tw = new StreamWriter(outputdir + @"/" + eocID + "_transcription.txt");
            tw.WriteLine(eocTranscription);
            tw.Close();
        }
    }
}
