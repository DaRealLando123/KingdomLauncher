using System.Drawing;
using System.Windows.Forms;

namespace KingdomLauncher {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btn_InstallPlay = new System.Windows.Forms.Button();
            this.btn_dir = new System.Windows.Forms.Button();
            this.box_version = new System.Windows.Forms.ComboBox();
            this.btn_del = new System.Windows.Forms.Button();
            this.btn_help = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(9, 153);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(329, 34);
            this.label1.TabIndex = 2;
            this.label1.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.BackColor = System.Drawing.Color.Black;
            this.progressBar1.Location = new System.Drawing.Point(9, 187);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(329, 20);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 1;
            this.progressBar1.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::KingdomLauncher.Properties.Resources.Logo;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(82, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(196, 149);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // btn_InstallPlay
            // 
            this.btn_InstallPlay.BackColor = System.Drawing.Color.Transparent;
            this.btn_InstallPlay.BackgroundImage = global::KingdomLauncher.Properties.Resources.Roxas;
            this.btn_InstallPlay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btn_InstallPlay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_InstallPlay.FlatAppearance.BorderSize = 0;
            this.btn_InstallPlay.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_InstallPlay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_InstallPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_InstallPlay.Font = new System.Drawing.Font("Palatino Linotype", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_InstallPlay.ForeColor = System.Drawing.Color.White;
            this.btn_InstallPlay.Location = new System.Drawing.Point(105, 138);
            this.btn_InstallPlay.Name = "btn_InstallPlay";
            this.btn_InstallPlay.Size = new System.Drawing.Size(151, 37);
            this.btn_InstallPlay.TabIndex = 0;
            this.btn_InstallPlay.TabStop = false;
            this.btn_InstallPlay.Text = "Install";
            this.btn_InstallPlay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_InstallPlay.UseVisualStyleBackColor = false;
            this.btn_InstallPlay.Visible = false;
            this.btn_InstallPlay.Click += new System.EventHandler(this.btn_InstallPlay_Click);
            // 
            // btn_dir
            // 
            this.btn_dir.BackColor = System.Drawing.Color.Transparent;
            this.btn_dir.Cursor = System.Windows.Forms.Cursors.Help;
            this.btn_dir.FlatAppearance.BorderSize = 0;
            this.btn_dir.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btn_dir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_dir.Font = new System.Drawing.Font("Palatino Linotype", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_dir.ForeColor = System.Drawing.SystemColors.Control;
            this.btn_dir.Location = new System.Drawing.Point(227, 176);
            this.btn_dir.Name = "btn_dir";
            this.btn_dir.Size = new System.Drawing.Size(23, 23);
            this.btn_dir.TabIndex = 6;
            this.btn_dir.Text = "📁";
            this.btn_dir.UseVisualStyleBackColor = false;
            this.btn_dir.Visible = false;
            this.btn_dir.Click += new System.EventHandler(this.btn_dir_Click);
            // 
            // box_version
            // 
            this.box_version.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.box_version.FormattingEnabled = true;
            this.box_version.ItemHeight = 13;
            this.box_version.Location = new System.Drawing.Point(137, 177);
            this.box_version.Name = "box_version";
            this.box_version.Size = new System.Drawing.Size(87, 21);
            this.box_version.TabIndex = 7;
            this.box_version.Visible = false;
            this.box_version.SelectedIndexChanged += new System.EventHandler(this.box_version_Changed);
            // 
            // btn_del
            // 
            this.btn_del.BackColor = System.Drawing.Color.Transparent;
            this.btn_del.Cursor = System.Windows.Forms.Cursors.Help;
            this.btn_del.FlatAppearance.BorderSize = 0;
            this.btn_del.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btn_del.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_del.Font = new System.Drawing.Font("Palatino Linotype", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_del.ForeColor = System.Drawing.SystemColors.Control;
            this.btn_del.Location = new System.Drawing.Point(110, 176);
            this.btn_del.Name = "btn_del";
            this.btn_del.Size = new System.Drawing.Size(23, 23);
            this.btn_del.TabIndex = 6;
            this.btn_del.Text = "🗑️";
            this.btn_del.UseVisualStyleBackColor = false;
            this.btn_del.Visible = false;
            this.btn_del.Click += new System.EventHandler(this.btn_del_Click);
            // 
            // btn_help
            // 
            this.btn_help.BackColor = System.Drawing.Color.Transparent;
            this.btn_help.Cursor = System.Windows.Forms.Cursors.Help;
            this.btn_help.FlatAppearance.BorderSize = 0;
            this.btn_help.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btn_help.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_help.Font = new System.Drawing.Font("Palatino Linotype", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_help.ForeColor = System.Drawing.SystemColors.Control;
            this.btn_help.Location = new System.Drawing.Point(315, 12);
            this.btn_help.Name = "btn_help";
            this.btn_help.Size = new System.Drawing.Size(23, 23);
            this.btn_help.TabIndex = 6;
            this.btn_help.Text = "?";
            this.btn_help.UseVisualStyleBackColor = false;
            this.btn_help.Click += new System.EventHandler(this.btn_help_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::KingdomLauncher.Properties.Resources.BackgroundImage;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(349, 214);
            this.Controls.Add(this.box_version);
            this.Controls.Add(this.btn_del);
            this.Controls.Add(this.btn_help);
            this.Controls.Add(this.btn_dir);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btn_InstallPlay);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "KingdomLauncher";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private PictureBox pictureBox1;
        private Button btn_InstallPlay;
        private Button btn_dir;
        private Button btn_del;
        private ComboBox box_version;
        private Button btn_help;
    }
}

