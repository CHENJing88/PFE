using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System;
using AForge.Imaging.Filters;

using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace Polytech.Clustering.Plugin
{
    public class ZernikeDescriptorPlugin : Form, IDescriptorPlugin
    {
        private TableLayoutPanel tableLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label orderLabel;
        private GroupBox groupBox1;
        private FlowLayoutPanel flowLayoutPanel2;
        private Button closeButton;
        private Button okButton;
        private Label label1;
        private TextBox normalisationTextBox1;
        private CheckBox checkBox2;
        private CheckBox checkBox1;
        private Label label2;
        private TextBox normalisationTextBox2;
        private TextBox maxOrderTextBox;

        private ZernikeConfig m_config = null;

        public ZernikeDescriptorPlugin()
        {
            InitializeComponent();
            m_config = new ZernikeConfig(int.Parse(this.maxOrderTextBox.Text), int.Parse(this.normalisationTextBox1.Text), int.Parse(this.normalisationTextBox1.Text));
        }

        public List<string> GetInfoList()
        {
            List<string> infoList = new List<string>();

            infoList.Add("Nom : " + GetName());
            infoList.Add("Ordre max. : " + m_config.MaxOrder);
            return infoList;
        }

       

       private void InitializeComponent()
       {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.orderLabel = new System.Windows.Forms.Label();
            this.maxOrderTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.normalisationTextBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.normalisationTextBox1 = new System.Windows.Forms.TextBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.closeButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.75449F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 77.24551F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(286, 201);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.orderLabel);
            this.flowLayoutPanel1.Controls.Add(this.maxOrderTextBox);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(280, 32);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // orderLabel
            // 
            this.orderLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.orderLabel.AutoSize = true;
            this.orderLabel.Location = new System.Drawing.Point(3, 6);
            this.orderLabel.Name = "orderLabel";
            this.orderLabel.Size = new System.Drawing.Size(64, 13);
            this.orderLabel.TabIndex = 0;
            this.orderLabel.Text = "Ordre max. :";
            // 
            // maxOrderTextBox
            // 
            this.maxOrderTextBox.Location = new System.Drawing.Point(73, 3);
            this.maxOrderTextBox.Name = "maxOrderTextBox";
            this.maxOrderTextBox.Size = new System.Drawing.Size(100, 20);
            this.maxOrderTextBox.TabIndex = 1;
            this.maxOrderTextBox.Text = "10";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.normalisationTextBox2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.normalisationTextBox1);
            this.groupBox1.Controls.Add(this.checkBox2);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(280, 123);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pré-traitement";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(202, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "px";
            // 
            // normalisationTextBox2
            // 
            this.normalisationTextBox2.Location = new System.Drawing.Point(169, 40);
            this.normalisationTextBox2.Name = "normalisationTextBox2";
            this.normalisationTextBox2.ReadOnly = true;
            this.normalisationTextBox2.Size = new System.Drawing.Size(27, 20);
            this.normalisationTextBox2.TabIndex = 5;
            this.normalisationTextBox2.Text = "30";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(137, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "px  x";
            // 
            // normalisationTextBox1
            // 
            this.normalisationTextBox1.Location = new System.Drawing.Point(104, 40);
            this.normalisationTextBox1.Name = "normalisationTextBox1";
            this.normalisationTextBox1.Size = new System.Drawing.Size(27, 20);
            this.normalisationTextBox1.TabIndex = 3;
            this.normalisationTextBox1.Text = "30";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(9, 42);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(89, 17);
            this.checkBox2.TabIndex = 1;
            this.checkBox2.Text = "Normalisation";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(9, 19);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Binarisation";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.closeButton);
            this.flowLayoutPanel2.Controls.Add(this.okButton);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 170);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(280, 28);
            this.flowLayoutPanel2.TabIndex = 5;
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(202, 3);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Fermer";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(121, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "Valider";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // ZernikeDescriptorPlugin
            // 
            this.ClientSize = new System.Drawing.Size(286, 201);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(302, 240);
            this.Name = "ZernikeDescriptorPlugin";
            this.Text = "Zernike - Configuration";
            this.Load += new System.EventHandler(this.ZernikeDescriptorPlugin_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

       }


       public string GetName()
       {
           return "Zernike";
       }

       public string GetAuthor()
       {
           return "F. Zernike";
       }


       public Form GetConfigWindow()
       {
           return this;
       }

        /// <summary>
        /// Calcule la signature à partir du pattern fourni en paramètre
        /// Prérequis : le pattern doit être de type Image !
        /// </summary>
        /// <param name="toModify">Le pattern concerné par le calcul</param>
       public void CalculateSignature(APattern toModify)
       {
           Bitmap imageThumbnail = toModify.ImageRepresentation;
           //Début du calcul des moments de Zernike en fonction des informations renseignées dans la fenêtre de configuration
           Bitmap squared = ImageNormalisationTools.ConvertToSquareImage(imageThumbnail);
           //squared.Save(@"F:\scolaire\PFE2\TestData\squared" + toModify.Id);

           //normalisation
           Bitmap resized = ImageNormalisationTools.ResizeImage(squared, m_config.SquareHeight, m_config.SquareHeight);
           //resized.Save(@"F:\scolaire\PFE2\TestData\resized" + toModify.Id);
           squared = null;

           //binarisation
           Bitmap binarised = ImageNormalisationTools.Binarize(resized);
           //binarised.Save(@"F:\scolaire\PFE2\TestData\binarized" + toModify.Id);


           //filtrage terminé, on calcule les différents moments de Zernike
           ZernikeSignature zernikeSign = ZernikeCalculator.CalculateFeatures(binarised, m_config.MaxOrder);
           resized = null;
           toModify.AddSignature(zernikeSign);

           binarised = null;
       }

       protected override void OnClosing(CancelEventArgs e)
       {
           this.Hide();
           e.Cancel = true;
       }

       protected void closeButton_Click(object sender, EventArgs e)
       {
           this.Hide();
       }

       private void okButton_Click(object sender, EventArgs e)
       {
           
           //on applique les modifications à la configuration
           ((ZernikeConfig)m_config).MaxOrder = int.Parse((maxOrderTextBox.Text));
           ((ZernikeConfig)m_config).SquareHeight = int.Parse((normalisationTextBox1.Text));
           ((ZernikeConfig)m_config).SquareWidth = int.Parse((normalisationTextBox1.Text));
           this.Hide();
       }


       public IConfig GetConfig()
       {
           return m_config;
       }

       private void ZernikeDescriptorPlugin_Load(object sender, EventArgs e)
       {
           //màj de l'affichage
           this.maxOrderTextBox.Text = m_config.MaxOrder.ToString();
           this.normalisationTextBox1.Text = m_config.SquareHeight.ToString();
           this.normalisationTextBox2.Text = m_config.SquareWidth.ToString();
       }
    }
}
