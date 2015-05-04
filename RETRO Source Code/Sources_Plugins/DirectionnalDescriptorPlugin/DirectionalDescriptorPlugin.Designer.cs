namespace Polytech.Clustering.Plugin
{
    partial class DirectionalDescriptorPlugin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DirectionalDescriptorPlugin));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.closeButton = new System.Windows.Forms.Button();
            this.validateButton = new System.Windows.Forms.Button();
            this.directionsPanel = new System.Windows.Forms.Panel();
            this.imagePanel = new System.Windows.Forms.Panel();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.normalisationTextBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.normalisationTextBox1 = new System.Windows.Forms.TextBox();
            this.normalizeCheckbox = new System.Windows.Forms.CheckBox();
            this.binarizeCheckBox = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.directionsPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.closeButton);
            this.flowLayoutPanel1.Controls.Add(this.validateButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 268);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(397, 33);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(319, 3);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Fermer";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // validateButton
            // 
            this.validateButton.Location = new System.Drawing.Point(238, 3);
            this.validateButton.Name = "validateButton";
            this.validateButton.Size = new System.Drawing.Size(75, 23);
            this.validateButton.TabIndex = 1;
            this.validateButton.Text = "Valider";
            this.validateButton.UseVisualStyleBackColor = true;
            this.validateButton.Click += new System.EventHandler(this.validateButton_Click);
            // 
            // directionsPanel
            // 
            this.directionsPanel.Controls.Add(this.imagePanel);
            this.directionsPanel.Controls.Add(this.checkedListBox1);
            this.directionsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.directionsPanel.Location = new System.Drawing.Point(0, 0);
            this.directionsPanel.Name = "directionsPanel";
            this.directionsPanel.Size = new System.Drawing.Size(397, 166);
            this.directionsPanel.TabIndex = 2;
            // 
            // imagePanel
            // 
            this.imagePanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("imagePanel.BackgroundImage")));
            this.imagePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.imagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imagePanel.Location = new System.Drawing.Point(0, 0);
            this.imagePanel.Name = "imagePanel";
            this.imagePanel.Size = new System.Drawing.Size(186, 166);
            this.imagePanel.TabIndex = 2;
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.BackColor = System.Drawing.SystemColors.Control;
            this.checkedListBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Items.AddRange(new object[] {
            "N",
            "S",
            "E",
            "O",
            "NE",
            "NO",
            "SE",
            "SO",
            "Toutes directions"});
            this.checkedListBox1.Location = new System.Drawing.Point(186, 0);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(211, 166);
            this.checkedListBox1.TabIndex = 1;
            this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.checkedListBox1_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 166);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(397, 102);
            this.panel2.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.normalisationTextBox2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.normalisationTextBox1);
            this.groupBox1.Controls.Add(this.normalizeCheckbox);
            this.groupBox1.Controls.Add(this.binarizeCheckBox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(397, 102);
            this.groupBox1.TabIndex = 5;
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
            this.normalisationTextBox2.Text = "50";
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
            this.normalisationTextBox1.Text = "50";
            // 
            // normalizeCheckbox
            // 
            this.normalizeCheckbox.AutoSize = true;
            this.normalizeCheckbox.Checked = true;
            this.normalizeCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.normalizeCheckbox.Location = new System.Drawing.Point(9, 42);
            this.normalizeCheckbox.Name = "normalizeCheckbox";
            this.normalizeCheckbox.Size = new System.Drawing.Size(89, 17);
            this.normalizeCheckbox.TabIndex = 1;
            this.normalizeCheckbox.Text = "Normalisation";
            this.normalizeCheckbox.UseVisualStyleBackColor = true;
            // 
            // binarizeCheckBox
            // 
            this.binarizeCheckBox.AutoSize = true;
            this.binarizeCheckBox.Checked = true;
            this.binarizeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.binarizeCheckBox.Location = new System.Drawing.Point(9, 19);
            this.binarizeCheckBox.Name = "binarizeCheckBox";
            this.binarizeCheckBox.Size = new System.Drawing.Size(80, 17);
            this.binarizeCheckBox.TabIndex = 0;
            this.binarizeCheckBox.Text = "Binarisation";
            this.binarizeCheckBox.UseVisualStyleBackColor = true;
            // 
            // DirectionalDescriptorPlugin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 301);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.directionsPanel);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "DirectionalDescriptorPlugin";
            this.Text = "Signature directionnelle - Configuration";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.directionsPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button validateButton;
        private System.Windows.Forms.Panel directionsPanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox normalisationTextBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox normalisationTextBox1;
        private System.Windows.Forms.CheckBox normalizeCheckbox;
        private System.Windows.Forms.CheckBox binarizeCheckBox;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Panel imagePanel;
    }
}