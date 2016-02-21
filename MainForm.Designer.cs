namespace FolderCompare
{
    partial class MainForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.folderDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.logListBox = new System.Windows.Forms.ListBox();
            this.baseFoldertextBox = new System.Windows.Forms.TextBox();
            this.compareFoldersTextBox = new System.Windows.Forms.TextBox();
            this.statusTextBox = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.fastRadio = new System.Windows.Forms.RadioButton();
            this.normalRadio = new System.Windows.Forms.RadioButton();
            this.strictRadio = new System.Windows.Forms.RadioButton();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(16, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(221, 39);
            this.button1.TabIndex = 0;
            this.button1.Text = "Load Base Folder";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(245, 13);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(221, 39);
            this.button2.TabIndex = 1;
            this.button2.Text = "Add Folder to compare";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.button3.Enabled = false;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(935, 712);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(221, 52);
            this.button3.TabIndex = 2;
            this.button3.Text = "Compare Folders and delete Duplicates";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // folderDialog1
            // 
            this.folderDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(473, 13);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(221, 39);
            this.button4.TabIndex = 4;
            this.button4.Text = "Clear Compare List";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(701, 13);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(221, 39);
            this.button5.TabIndex = 5;
            this.button5.Text = "Find Corrupted JPG";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.Location = new System.Drawing.Point(928, 13);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(221, 39);
            this.button6.TabIndex = 6;
            this.button6.Text = "Delete Empty Files/Folders";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // logListBox
            // 
            this.logListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logListBox.FormattingEnabled = true;
            this.logListBox.ItemHeight = 17;
            this.logListBox.Location = new System.Drawing.Point(16, 290);
            this.logListBox.Name = "logListBox";
            this.logListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.logListBox.Size = new System.Drawing.Size(1136, 395);
            this.logListBox.TabIndex = 7;
            // 
            // baseFoldertextBox
            // 
            this.baseFoldertextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.baseFoldertextBox.Location = new System.Drawing.Point(16, 68);
            this.baseFoldertextBox.Name = "baseFoldertextBox";
            this.baseFoldertextBox.ReadOnly = true;
            this.baseFoldertextBox.Size = new System.Drawing.Size(1133, 24);
            this.baseFoldertextBox.TabIndex = 8;
            // 
            // compareFoldersTextBox
            // 
            this.compareFoldersTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.compareFoldersTextBox.Location = new System.Drawing.Point(16, 111);
            this.compareFoldersTextBox.Multiline = true;
            this.compareFoldersTextBox.Name = "compareFoldersTextBox";
            this.compareFoldersTextBox.ReadOnly = true;
            this.compareFoldersTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.compareFoldersTextBox.Size = new System.Drawing.Size(1133, 161);
            this.compareFoldersTextBox.TabIndex = 9;
            // 
            // statusTextBox
            // 
            this.statusTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusTextBox.Location = new System.Drawing.Point(16, 712);
            this.statusTextBox.Multiline = true;
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.ReadOnly = true;
            this.statusTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.statusTextBox.Size = new System.Drawing.Size(902, 97);
            this.statusTextBox.TabIndex = 10;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(935, 770);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(220, 39);
            this.progressBar1.TabIndex = 11;
            // 
            // fastRadio
            // 
            this.fastRadio.AutoSize = true;
            this.fastRadio.Location = new System.Drawing.Point(607, 688);
            this.fastRadio.Name = "fastRadio";
            this.fastRadio.Size = new System.Drawing.Size(155, 22);
            this.fastRadio.TabIndex = 13;
            this.fastRadio.Text = "Fast (Size + Name)";
            this.fastRadio.UseVisualStyleBackColor = true;
            // 
            // normalRadio
            // 
            this.normalRadio.AutoSize = true;
            this.normalRadio.Checked = true;
            this.normalRadio.Location = new System.Drawing.Point(768, 688);
            this.normalRadio.Name = "normalRadio";
            this.normalRadio.Size = new System.Drawing.Size(170, 22);
            this.normalRadio.TabIndex = 14;
            this.normalRadio.TabStop = true;
            this.normalRadio.Text = "Normal (Size + Hash)";
            this.normalRadio.UseVisualStyleBackColor = true;
            // 
            // strictRadio
            // 
            this.strictRadio.AutoSize = true;
            this.strictRadio.Location = new System.Drawing.Point(944, 688);
            this.strictRadio.Name = "strictRadio";
            this.strictRadio.Size = new System.Drawing.Size(212, 22);
            this.strictRadio.TabIndex = 15;
            this.strictRadio.Text = "Strict (Size + Name + Hash)";
            this.strictRadio.UseVisualStyleBackColor = true;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1170, 824);
            this.Controls.Add(this.strictRadio);
            this.Controls.Add(this.normalRadio);
            this.Controls.Add(this.fastRadio);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.statusTextBox);
            this.Controls.Add(this.compareFoldersTextBox);
            this.Controls.Add(this.baseFoldertextBox);
            this.Controls.Add(this.logListBox);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1186, 274);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Folder Compare";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.FolderBrowserDialog folderDialog1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.ListBox logListBox;
        private System.Windows.Forms.TextBox baseFoldertextBox;
        private System.Windows.Forms.TextBox compareFoldersTextBox;
        private System.Windows.Forms.TextBox statusTextBox;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RadioButton fastRadio;
        private System.Windows.Forms.RadioButton normalRadio;
        private System.Windows.Forms.RadioButton strictRadio;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}

