using Polytech.Clustering.Plugin;
using Retro.ocr;
using Retro.ViewModel;
using RetroUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RetroGUI.transcription
{
    /// <summary>
    /// Logical interaction for AutoTranscriptionPanel.xaml
    /// </summary>
    public partial class AutoTranscriptionPanel : UserControl
    {
        #region Attributes
        /// <summary>
        /// AutoTransVewModel
        /// </summary>
        public AutoTransVewModel AutoTrans { get; private set; }
        /// <summary>
        /// Description method selected
        /// </summary>
        private String SelectDescripMethod;
        /// <summary>
        /// Transcription method selected
        /// </summary>
        private String SelectTranscripMethod;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor par defaut
        /// </summary>
        public AutoTranscriptionPanel() {}

        /// <summary>
        /// Constructor with RetroViewModel
        /// </summary>
        /// <param name="retroVM"></param>
        public AutoTranscriptionPanel(Retro.ViewModel.RetroViewModel retroVM)
        {
            InitializeComponent();
            AutoTrans = new AutoTransVewModel(retroVM);

            LoadTranscripMethod_Combox();
            LoadDescriptorMethod_Combox();
        }
        #endregion

        #region ComboBox
        /// <summary>
        /// Load the method of descriptor 
        /// </summary>
        private void LoadDescriptorMethod_Combox()
        {
            //add the list of transcription method
            List<String> DescriptorMethod = AutoTrans.DescriptorMethodList();
            //add the combobox Transcription method
            for (int i = 0; i < DescriptorMethod.Count; i++)
            {
                this.ComboBoxDescriptor.Items.Add(DescriptorMethod[i]);
            }
            this.ComboBoxDescriptor.SelectedIndex = 0;
        }
        /// <summary>
        /// Load the method of transcription 
        /// </summary>
        private void LoadTranscripMethod_Combox()
        {
            //add the list of transcription method
            List<String> TranscripMethod = AutoTrans.TranscripMethodList();
            //add the combobox Transcription method
            for (int i = 0; i < TranscripMethod.Count; i++)
            {
                this.ComboBoxTransMethode.Items.Add(TranscripMethod[i]);
            }
            this.ComboBoxTransMethode.SelectedIndex = 0;
        }
        /// <summary>
        /// comboBox of transcription method selection changed action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxTransMethode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectTranscripMethod = this.ComboBoxTransMethode.SelectedValue.ToString();
        }
        /// <summary>
        /// comboBox of descriptor method selection changed action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxDescriptor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectDescripMethod = this.ComboBoxDescriptor.SelectedValue.ToString();
        }
        #endregion 

        #region Buttons
        /// <summary>
        /// the button of select the floder of FontModel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonFloderFontModel_Click(object sender, RoutedEventArgs e)
        {
            // Open a dialog for folder selection
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowDialog();
            if ((fbd.SelectedPath.Length != 0) && (Directory.Exists(fbd.SelectedPath)))
            {
                // Update textbox with the selected directory
                this.TextBoxFontMondelFolder.Text = fbd.SelectedPath;
            }
        }
        /// <summary>
        /// run the processus of auto transcription
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            AutoTrans.Run(SelectDescripMethod,SelectTranscripMethod, this.TextBoxFontMondelFolder.Text);
        }

        #endregion


    }
}
