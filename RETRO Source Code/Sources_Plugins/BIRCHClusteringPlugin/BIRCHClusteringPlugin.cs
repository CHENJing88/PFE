using System.Windows.Forms;
using Polytech.Clustering.Plugin;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;


namespace Polytech.Clustering.Plugin
{
    public class BIRCHClusteringPlugin : Form, IClusteringPlugin
    {
        /// <summary>
        /// Référence vers la base de données à partitionner
        /// </summary>
        Database m_db = null;

        /// <summary>
        /// Référence vers la classe encapsulant la configuration du plugin
        /// </summary>
        IConfig m_config = null;

        /// <summary>
        /// Temps de traitement de la méthode de clustering
        /// </summary>
        TimeSpan m_elapsedTime;


        /// <summary>
        /// Référence vers le dernier CFTree créé
        /// </summary>
        CFTree m_lastTree = null;

        /// <summary>
        /// Liste des clusters résultants de l'opération
        /// </summary>
        List<Cluster> m_results = null;

        private TableLayoutPanel tableLayoutPanel1;
        private Label label1;
        private Label label2;
        private TextBox maxEntries1TextBox;
        private TextBox maxEntries2TextBox;
        private Label label3;
        private GroupBox groupBox1;
        private FlowLayoutPanel flowLayoutPanel2;
        private Button closeButton;
        private Button okButton;
        private FlowLayoutPanel flowLayoutPanel1;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
        private FlowLayoutPanel flowLayoutPanel3;
        private Button saveCFTreeButton;
        private Button loadCFTreeButton;
        private Label label4;
        private TextBox rebuildTextBox;
        private Label label5;
        private TextBox stepRebuildTextBox;
        private CheckBox checkBoxReuseTree;
        private TextBox thresholdTextBox;


        public void SetDatabase(Database db)
        {
            m_db = db;
        }

        /// <summary>
        /// Effectue une opération de clustering sur le cluster passé en paramètre
        /// </summary>
        /// <param name="cluster"></param>
        /// <returns></returns>
        public List<Cluster> PerformClustering(Cluster cluster, List<APattern> refPatterns = null, bool updateActualClusters = false, int indexSignature = -1)
        {
            //on débute le clustering
            // Récupération du type de distance à utiliser pour l'insertion dans un arbre
            int distanceType = -1;
            for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)
            {
                if (flowLayoutPanel1.Controls[i].GetType().Name == "RadioButton") //si il s'agit d'un radio button
                {
                    if (((RadioButton)flowLayoutPanel1.Controls[i]).Checked)
                    {
                        distanceType = i;
                        break;
                    }
                }
            }
            distanceType = 0;
            double thresholdValue = double.Parse(thresholdTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);

            //Création/réutilisation du CFTree
            CFTree tree =null;
            if (checkBoxReuseTree.Checked == true)
                tree = m_lastTree;
            else
                tree = new CFTree(((BIRCHConfig)m_config).MaxEntriesInternalNode, ((BIRCHConfig)m_config).MaxEntriesLeafNode, (CFTree.DistanceType)distanceType, ((BIRCHConfig)m_config).Threshold);
           
            //Phase 1 : Construction d'une première version de l'arbre
            //On passe en revue les éléments du cluster
            int indexPattern = 1;
            foreach (APattern pattern in cluster.Patterns)
            {
                //création d'une entrée contenant uniquement 1 pattern
                CFEntry newEntry = new CFEntry(pattern);
                //insertion de la nouvelle entrée dans l'arbre
                tree.InsertEntry(newEntry);
                indexPattern++;
            }

            //Phase 2 - Reconstruction de l'arbre pour obtenir une version moins dense
            CFTree newTree = tree;
            double threshold = tree.Threshold;
            for (int i = 0; i <  int.Parse(rebuildTextBox.Text) ; i++)
            {
                if (!stepRebuildTextBox.Text.Equals(""))
                {
                    threshold = threshold + Convert.ToDouble(stepRebuildTextBox.Text);
                }
                newTree = newTree.Rebuild(threshold);
            }

            //Phase 3 - Clustering des CFEntries des feuilles
            tree = newTree;
            m_results = tree.GetClusters();
            //enregistrement de la référence sur l'arbre
            m_lastTree = tree;
            return m_results;
        }

        public List<Cluster> PerformClustering()
        {
            if (m_db == null)
            {
                MessageBox.Show("Aucune base de données disponible",
                    "Base de données manquante",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                return null;
            }
            else
            {
                //enregistrement temps de début clustering
                Stopwatch sw = new Stopwatch();
                sw.Start();

                //on débute le clustering
               // Récupération du type de distance à utiliser pour l'insertion dans un arbre
                int distanceType = -1;
                for(int i = 0;i< flowLayoutPanel1.Controls.Count;i++)
                {
                       if (flowLayoutPanel1.Controls[i].GetType().Name == "RadioButton") //si il s'agit d'un radio button
                       {
                           if (((RadioButton)flowLayoutPanel1.Controls[i]).Checked)
                           {
                               distanceType = i;
                               break;
                           }
                       }
                }
                distanceType = 0;
                double thresholdValue = double.Parse(thresholdTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);

                //Création/réutilisation du CFTree
                CFTree tree = null;
                if (checkBoxReuseTree.Checked == true)
                    tree = m_lastTree;
                else
                    tree = new CFTree(((BIRCHConfig)m_config).MaxEntriesInternalNode, ((BIRCHConfig)m_config).MaxEntriesLeafNode, (CFTree.DistanceType)distanceType, ((BIRCHConfig)m_config).Threshold);

                //Phase 1 : Construction d'une première version de l'arbre
                //On passe en revue la base de données
                //récupération de la liste des documents
                List<Document> docs = m_db.Documents;
                foreach(Document doc in docs)
                {
                    int i = 1;
                    //pour chaque pattern
                    foreach (APattern pattern in doc.Patterns)
                    {
                        //création d'une entrée contenant uniquement 1 pattern
                        CFEntry newEntry = new CFEntry(pattern);
                        //insertion de la nouvelle entrée dans l'arbre
                        tree.InsertEntry(newEntry);
                        i++;
                    }
                }

                //Phase 2 - Reconstruction de l'arbre pour obtenir une version moins dense
                CFTree newTree = tree;
                double threshold = tree.Threshold;

                for (int i = 0; i < Convert.ToInt32(rebuildTextBox.Text); i++)
                {
                    if ( !stepRebuildTextBox.Text.Equals("") )
                    {
                        threshold = threshold + Convert.ToDouble(stepRebuildTextBox.Text);
                    }
                    newTree = newTree.Rebuild(threshold);
                    // }//
                }

                //Phase 3 - Clustering des CFEntries des feuilles
               tree = newTree;
                m_results = tree.GetClusters();
                  
                sw.Stop();
                //retiré NG
                //System.IO.StreamWriter file = new System.IO.StreamWriter("F:\\scolaire\\PFE2\\TestData\\time2.txt");
                //m_elapsedTime = sw.Elapsed;
                //file.WriteLine("BIRCH Clustering : " + m_elapsedTime + "--");
                //file.Close();
                m_lastTree = tree;
                return m_results;
            }
        }

       public BIRCHClusteringPlugin()
       {
           InitializeComponent();
           //Instanciation de l'objet encapsulant la configuration du plugin
           double thresholdValue = double.Parse(thresholdTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
           int distanceType = 0;
           m_config = new BIRCHConfig(int.Parse(maxEntries1TextBox.Text), int.Parse(maxEntries2TextBox.Text), thresholdValue, (CFTree.DistanceType) distanceType);
       }

        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.maxEntries1TextBox = new System.Windows.Forms.TextBox();
            this.maxEntries2TextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.thresholdTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.rebuildTextBox = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.saveCFTreeButton = new System.Windows.Forms.Button();
            this.loadCFTreeButton = new System.Windows.Forms.Button();
            this.checkBoxReuseTree = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.closeButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.stepRebuildTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 182F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.maxEntries1TextBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.maxEntries2TextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.thresholdTextBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.rebuildTextBox, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.stepRebuildTextBox, 1, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.3871F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.6129F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 126F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(523, 306);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Entrées max. (non feuilles) :";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Entrées max. (feuilles) :";
            // 
            // maxEntries1TextBox
            // 
            this.maxEntries1TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.maxEntries1TextBox.Location = new System.Drawing.Point(344, 4);
            this.maxEntries1TextBox.Name = "maxEntries1TextBox";
            this.maxEntries1TextBox.Size = new System.Drawing.Size(176, 20);
            this.maxEntries1TextBox.TabIndex = 2;
            this.maxEntries1TextBox.Text = "70";
            // 
            // maxEntries2TextBox
            // 
            this.maxEntries2TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.maxEntries2TextBox.Location = new System.Drawing.Point(344, 34);
            this.maxEntries2TextBox.Name = "maxEntries2TextBox";
            this.maxEntries2TextBox.Size = new System.Drawing.Size(176, 20);
            this.maxEntries2TextBox.TabIndex = 3;
            this.maxEntries2TextBox.Text = "70";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 90);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(335, 120);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Métrique de fusion";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.radioButton1);
            this.flowLayoutPanel1.Controls.Add(this.radioButton2);
            this.flowLayoutPanel1.Controls.Add(this.radioButton3);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(329, 101);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(3, 3);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(142, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Distance inter-centroides";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(3, 26);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(56, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Rayon";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(3, 49);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(67, 17);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Diamètre";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // thresholdTextBox
            // 
            this.thresholdTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.thresholdTextBox.Location = new System.Drawing.Point(344, 63);
            this.thresholdTextBox.Name = "thresholdTextBox";
            this.thresholdTextBox.Size = new System.Drawing.Size(176, 20);
            this.thresholdTextBox.TabIndex = 6;
            this.thresholdTextBox.Text = "75";
            this.thresholdTextBox.TextChanged += new System.EventHandler(this.thresholdTextBox_TextChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Seuil :";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 221);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(140, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Nombre de reconstructions :";
            // 
            // rebuildTextBox
            // 
            this.rebuildTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.rebuildTextBox.Location = new System.Drawing.Point(344, 217);
            this.rebuildTextBox.Name = "rebuildTextBox";
            this.rebuildTextBox.Size = new System.Drawing.Size(176, 20);
            this.rebuildTextBox.TabIndex = 10;
            this.rebuildTextBox.Text = "0";
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.Controls.Add(this.saveCFTreeButton);
            this.flowLayoutPanel3.Controls.Add(this.loadCFTreeButton);
            this.flowLayoutPanel3.Controls.Add(this.checkBoxReuseTree);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 272);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(333, 29);
            this.flowLayoutPanel3.TabIndex = 8;
            // 
            // saveCFTreeButton
            // 
            this.saveCFTreeButton.AutoSize = true;
            this.saveCFTreeButton.Enabled = false;
            this.saveCFTreeButton.Location = new System.Drawing.Point(3, 3);
            this.saveCFTreeButton.Name = "saveCFTreeButton";
            this.saveCFTreeButton.Size = new System.Drawing.Size(116, 23);
            this.saveCFTreeButton.TabIndex = 0;
            this.saveCFTreeButton.Text = "Sauvegarder CFTree";
            this.saveCFTreeButton.UseVisualStyleBackColor = true;
            this.saveCFTreeButton.Click += new System.EventHandler(this.saveCFTreeButton_Click);
            // 
            // loadCFTreeButton
            // 
            this.loadCFTreeButton.AutoSize = true;
            this.loadCFTreeButton.Location = new System.Drawing.Point(125, 3);
            this.loadCFTreeButton.Name = "loadCFTreeButton";
            this.loadCFTreeButton.Size = new System.Drawing.Size(92, 23);
            this.loadCFTreeButton.TabIndex = 1;
            this.loadCFTreeButton.Text = "Charger CFTree";
            this.loadCFTreeButton.UseVisualStyleBackColor = true;
            this.loadCFTreeButton.Click += new System.EventHandler(this.loadCFTreeButton_Click);
            // 
            // checkBoxReuseTree
            // 
            this.checkBoxReuseTree.AutoSize = true;
            this.checkBoxReuseTree.Location = new System.Drawing.Point(223, 3);
            this.checkBoxReuseTree.MinimumSize = new System.Drawing.Size(107, 17);
            this.checkBoxReuseTree.Name = "checkBoxReuseTree";
            this.checkBoxReuseTree.Size = new System.Drawing.Size(107, 17);
            this.checkBoxReuseTree.TabIndex = 2;
            this.checkBoxReuseTree.Text = "Réutiliser CFTree";
            this.checkBoxReuseTree.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.closeButton);
            this.flowLayoutPanel2.Controls.Add(this.okButton);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(344, 272);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(162, 29);
            this.flowLayoutPanel2.TabIndex = 7;
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(84, 3);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Fermer";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(3, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "Valider";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 249);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(172, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Pas du seuil entre reconstructions :";
            // 
            // stepRebuildTextBox
            // 
            this.stepRebuildTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.stepRebuildTextBox.Location = new System.Drawing.Point(344, 245);
            this.stepRebuildTextBox.Name = "stepRebuildTextBox";
            this.stepRebuildTextBox.Size = new System.Drawing.Size(176, 20);
            this.stepRebuildTextBox.TabIndex = 12;
            // 
            // BIRCHClusteringPlugin
            // 
            this.ClientSize = new System.Drawing.Size(523, 306);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(421, 304);
            this.Name = "BIRCHClusteringPlugin";
            this.Load += new System.EventHandler(this.BIRCHClusteringPlugin_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        public string GetName()
        {
            return "BIRCH";
        }

        public string GetAuthor()
        {
            return "Zhang, Ramakrishnan et Livny";
        }


        public Form GetConfigWindow()
        {
            return this;
        }

        public List<string> GetInfoList()
        {
            List<string> infoList = new List<string>();

            infoList.Add("Configuration");
            infoList.Add("Nombre d'entrées max. (noeuds non feuilles) : " + ((BIRCHConfig) m_config).MaxEntriesInternalNode );
            infoList.Add("Nombre d'entrées max. (noeuds feuilles) : " + ((BIRCHConfig)m_config).MaxEntriesLeafNode);
            infoList.Add("Seuil : " + ((BIRCHConfig)m_config).Threshold);

            string distanceName = null;
            //Récupération du texte associé au radiobutton permettant de sélectionner le type de distance à utiliser
            for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)
            {
                if (flowLayoutPanel1.Controls[i].GetType().Name == "RadioButton") //si il s'agit d'un radio button
                {
                    if (((RadioButton)flowLayoutPanel1.Controls[i]).Checked)
                    {
                        distanceName = ((RadioButton)flowLayoutPanel1.Controls[i]).Text;
                        break;
                    }
                }
            }
            infoList.Add("Fonction distance (pour insertion) : " + distanceName);
   
            return infoList;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void closeButton_Click(object sender, System.EventArgs e)
        {
            this.Hide();
        }

        /// <summary>
        /// Action à effectuer sur clique du bouton "ok"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, System.EventArgs e)
        {
            //Modification de la configuration
            ((BIRCHConfig)m_config).MaxEntriesInternalNode = int.Parse(maxEntries1TextBox.Text);
            ((BIRCHConfig)m_config).MaxEntriesInternalNode = int.Parse(maxEntries2TextBox.Text);
            ((BIRCHConfig)m_config).Threshold = int.Parse(thresholdTextBox.Text);
            
            this.Hide();
        }

        private void thresholdTextBox_TextChanged(object sender, System.EventArgs e)
        {
        }

        public IConfig GetConfig()
        {
            return m_config;
        }


        public System.TimeSpan GetProcessingTime()
        {
            return m_elapsedTime;
        }

        private void BIRCHClusteringPlugin_Load(object sender, EventArgs e)
        {
            //màj de l'affichege
            maxEntries1TextBox.Text = ((BIRCHConfig)m_config).MaxEntriesInternalNode.ToString() ;
            maxEntries1TextBox.Text = ((BIRCHConfig)m_config).MaxEntriesLeafNode.ToString();
            thresholdTextBox.Text = ((BIRCHConfig)m_config).Threshold.ToString();

            if (m_lastTree != null)
                saveCFTreeButton.Enabled = true;
        }

        private void saveCFTreeButton_Click(object sender, EventArgs e)
        {
            if( m_lastTree != null) // on teste si un arbre a été construit
            {
                FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
                //si oui, on demande à l'utilisateur dans quel dossier il souhaite enregistrer son arbre
                if(folderBrowserDialog1.ShowDialog() == DialogResult.OK) 
                {
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new FileStream(folderBrowserDialog1.SelectedPath + "CFTree.tree", FileMode.Create, FileAccess.Write, FileShare.None);
                    formatter.Serialize(stream, this.m_lastTree);
                    stream.Close();

                    MessageBox.Show("Enregistrement effectué avec succès");
                }
            }
        }

        private void loadCFTreeButton_Click(object sender, EventArgs e)
        {
            //désérialization du fichier correspondant cf CFTree
            //on demande à l'utilsateur de sélectionner le fichier qui l'intéresse
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
                //on essaie de désérializer l'arbre
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(file.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                m_lastTree = (CFTree) formatter.Deserialize(stream);

                stream.Close();

                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            }
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
             try
            {
                if (args.Name.Split(',')[0] == "ZernikeDescriptorPlugin")
                {
                    //Load my Assembly 
                    Assembly assem = Assembly.LoadFrom(@"F:\workspace\visual\Clustering\Clustering\bin\Debug\plugins\descriptors\ZernikeDescriptorPlugin.dll");
                    if(assem != null)
                        return assem;
                }
            }
            catch { ;}

           return Assembly.GetExecutingAssembly();
        }

        public void saveCFTreeTemp()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(@"F:\CFTree.tree", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this.m_lastTree);
            stream.Close();
        }

        public void loadCFTreeTemp()
        {
            //on essaie de désérializer l'arbre
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(@"F:\CFTree.tree", FileMode.Open, FileAccess.Read, FileShare.Read);
            m_lastTree = (CFTree)formatter.Deserialize(stream);
            stream.Close();
        }

        public List<Cluster> PerformClustering(List<APattern> refPatterns = null, bool updateActualClusters = false, int indexSignature = -1)
        {
            throw new NotImplementedException(); 
        }
    }
}
