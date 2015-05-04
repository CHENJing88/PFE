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
using System.Xml.Serialization;
using System.ComponentModel;

using Retro.Model;
using RetroUtil;

namespace Retro.ViewModel
{
    /// <summary>
    /// Define the ViewModel Element of Retro in the MVVM architectural pattern
    /// </summary>
    public class RetroViewModel
    {

        private RetroProject _RetroInstance;
        /// <summary>
        /// Instance of the current used RetroProject
        /// </summary>
        public RetroProject RetroInstance
        {
            get { return _RetroInstance; }
            set { _RetroInstance = value; }
        }


        /// <summary>
        /// Default Constructor
        /// </summary>
        public RetroViewModel()
        {
        }


        /// <summary>
        /// Create a new Retro project
        /// </summary>
        /// <param name="projectName">Name of the current project</param>
        /// <param name="agoraProjectFile">Agora project file</param>
        public void NewProject(String projectName, String agoraProjectFile)
        {
            RetroProject _retro = new RetroProject();
            RetroProject.New(projectName, agoraProjectFile, ref _retro);
            this._RetroInstance = _retro;
        }


        /// <summary>
        /// Load an existing Retro project
        /// </summary>
        /// <param name="filepath">Path of the project file</param>
        /// <returns>ReturnValues.OpenProject value</returns>
        public ReturnValues.OpenProject OpenProject(String filepath)
        {
            RetroProject _retro = new RetroProject();
            ReturnValues.OpenProject result = RetroProject.Open(filepath, ref _retro);

            // Assign RetroInstance
            if (result == ReturnValues.OpenProject.Ok)
                this._RetroInstance = _retro;
            else
                this._RetroInstance = null;

            return result;
        }


        /// <summary>
        /// Save the current project
        /// </summary>
        /// <param name="filepath">Path of the project file</param>
        public void SaveProject(String filepath)
        {
            this._RetroInstance.Save(filepath);
        }


        /// <summary>
        /// ClearClustersList the current project
        /// </summary>
        public void CloseProject()
        {
            this._RetroInstance.ClearClustersList();
        }

        /// <summary>
        /// Export Transcription as Alto
        /// </summary>
        /// <param name="dynamicSplashScreenNotification">Dynamic Splashscreen Notification</param>
        public void ExportAsAlto(DynamicSplashScreenNotification dynamicSplashScreenNotification)
        {
            this._RetroInstance.ExportAsAlto(dynamicSplashScreenNotification);
        }


        /// <summary>
        /// Load clusters entities of the current project
        /// </summary>
        /// <param name="bIllustrationClustering">Indicate if we considerate illustration clusters</param>
        public void LoadClusters(bool bIllustrationClustering)
        {
            this._RetroInstance.LoadClusters(bIllustrationClustering);
        }


        /// <summary>
        /// Get the list of clusters of the current project
        /// </summary>
        /// <returns>List of clusters</returns>
        public List<Cluster> GetClusters()
        {
            if (this.RetroInstance == null)
            {
                return null;
            }
            else
            {
                return this._RetroInstance.ClustersList;
            }
        }


        /// <summary>
        /// Clear current list of clusters
        /// </summary>
        public void ClearClustersList()
        {
            this._RetroInstance.ClustersList.Clear();
        }

    }
}
