using System.Drawing;
using System.Windows.Forms;

namespace HoloLauncher {
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
            this.btn_Uninstall = new System.Windows.Forms.Button();
            this.btn_InstallPlay = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_dir = new System.Windows.Forms.Button();
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
            this.label1.Location = new System.Drawing.Point(8, 163);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(329, 29);
            this.label1.TabIndex = 2;
            this.label1.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(8, 197);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(329, 10);
            this.progressBar1.TabIndex = 1;
            this.progressBar1.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Image = global::HoloLauncher.Properties.Resources.Logo;
            this.pictureBox1.Location = new System.Drawing.Point(8, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(114, 81);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // btn_Uninstall
            // 
            this.btn_Uninstall.BackColor = System.Drawing.Color.Transparent;
            this.btn_Uninstall.BackgroundImage = global::HoloLauncher.Properties.Resources.Man;
            this.btn_Uninstall.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_Uninstall.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Uninstall.Enabled = false;
            this.btn_Uninstall.FlatAppearance.BorderSize = 0;
            this.btn_Uninstall.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_Uninstall.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_Uninstall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Uninstall.Font = new System.Drawing.Font("Palatino Linotype", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Uninstall.ForeColor = System.Drawing.Color.Transparent;
            this.btn_Uninstall.Location = new System.Drawing.Point(-5, 121);
            this.btn_Uninstall.Name = "btn_Uninstall";
            this.btn_Uninstall.Size = new System.Drawing.Size(138, 23);
            this.btn_Uninstall.TabIndex = 4;
            this.btn_Uninstall.TabStop = false;
            this.btn_Uninstall.Text = "Uninstall";
            this.btn_Uninstall.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_Uninstall.UseVisualStyleBackColor = false;
            this.btn_Uninstall.Click += new System.EventHandler(this.btn_Uninstall_Click);
            // 
            // btn_InstallPlay
            // 
            this.btn_InstallPlay.BackColor = System.Drawing.Color.Transparent;
            this.btn_InstallPlay.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_InstallPlay.BackgroundImage")));
            this.btn_InstallPlay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_InstallPlay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_InstallPlay.FlatAppearance.BorderSize = 0;
            this.btn_InstallPlay.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btn_InstallPlay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btn_InstallPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_InstallPlay.Font = new System.Drawing.Font("Palatino Linotype", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_InstallPlay.ForeColor = System.Drawing.Color.White;
            this.btn_InstallPlay.Location = new System.Drawing.Point(-5, 87);
            this.btn_InstallPlay.Name = "btn_InstallPlay";
            this.btn_InstallPlay.Size = new System.Drawing.Size(138, 32);
            this.btn_InstallPlay.TabIndex = 0;
            this.btn_InstallPlay.TabStop = false;
            this.btn_InstallPlay.Text = "Install";
            this.btn_InstallPlay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btn_InstallPlay.UseVisualStyleBackColor = false;
            this.btn_InstallPlay.Click += new System.EventHandler(this.btn_InstallPlay_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(150, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(187, 65);
            this.label2.TabIndex = 5;
            this.label2.Text = "Launcher created by\r\nDaRealLando123 and zpitolava22350\r\n\r\n358/2 Days Final Mix cr" +
    "eated by\r\nO’Shinobi ツ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btn_dir
            // 
            this.btn_dir.BackColor = System.Drawing.Color.Transparent;
            this.btn_dir.Cursor = System.Windows.Forms.Cursors.Help;
            this.btn_dir.FlatAppearance.BorderSize = 0;
            this.btn_dir.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btn_dir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_dir.ForeColor = System.Drawing.SystemColors.Control;
            this.btn_dir.Location = new System.Drawing.Point(125, 121);
            this.btn_dir.Name = "btn_dir";
            this.btn_dir.Size = new System.Drawing.Size(23, 23);
            this.btn_dir.TabIndex = 6;
            this.btn_dir.Text = "📂";
            this.btn_dir.UseVisualStyleBackColor = false;
            this.btn_dir.Visible = false;
            this.btn_dir.Click += new System.EventHandler(this.btn_dir_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::HoloLauncher.Properties.Resources.BackgroundImage;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(349, 214);
            this.Controls.Add(this.btn_dir);
            this.Controls.Add(this.btn_Uninstall);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_InstallPlay);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "KingdomLauncher";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private PictureBox pictureBox1;
        private Button btn_Uninstall;
        private Button btn_InstallPlay;
        private Label label2;
        private Button btn_dir;
    }
}

