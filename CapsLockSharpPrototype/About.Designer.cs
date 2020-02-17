using System.ComponentModel;
using System.Windows.Forms;

namespace CapsLockSharpPrototype
{
    partial class About
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.label1 = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.linkToGithub = new System.Windows.Forms.LinkLabel();
            this.linkToDefaultShortcuts = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.picLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(86, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "CaploS";
            // 
            // lblVersion
            // 
            this.lblVersion.Location = new System.Drawing.Point(240, 14);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(148, 17);
            this.lblVersion.TabIndex = 1;
            this.lblVersion.Text = "version xxx by Pluveto";
            // 
            // linkToGithub
            // 
            this.linkToGithub.AutoSize = true;
            this.linkToGithub.Location = new System.Drawing.Point(87, 96);
            this.linkToGithub.Name = "linkToGithub";
            this.linkToGithub.Size = new System.Drawing.Size(177, 17);
            this.linkToGithub.TabIndex = 2;
            this.linkToGithub.TabStop = true;
            this.linkToGithub.Text = "Github Repo（手动检查更新）";
            this.linkToGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkToGithub_LinkClicked);
            // 
            // linkToDefaultShortcuts
            // 
            this.linkToDefaultShortcuts.AutoSize = true;
            this.linkToDefaultShortcuts.Location = new System.Drawing.Point(320, 96);
            this.linkToDefaultShortcuts.Name = "linkToDefaultShortcuts";
            this.linkToDefaultShortcuts.Size = new System.Drawing.Size(68, 17);
            this.linkToDefaultShortcuts.TabIndex = 2;
            this.linkToDefaultShortcuts.TabStop = true;
            this.linkToDefaultShortcuts.Text = "默认键位表";
            this.linkToDefaultShortcuts.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkToDefaultShortcuts_LinkClicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(149, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 17);
            this.label4.TabIndex = 1;
            this.label4.Text = " - CapsLock#";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(87, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(301, 48);
            this.label5.TabIndex = 1;
            this.label5.Text = "　　如果本软件对你有用，请考虑在下方 Github 网页给一颗小星星~（点击右上角 star 即可）";
            // 
            // picLogo
            // 
            this.picLogo.Image = ((System.Drawing.Image)(resources.GetObject("picLogo.Image")));
            this.picLogo.Location = new System.Drawing.Point(12, 9);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(64, 64);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 3;
            this.picLogo.TabStop = false;
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 122);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.linkToDefaultShortcuts);
            this.Controls.Add(this.linkToGithub);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "About";
            this.Text = "关于";
            this.Load += new System.EventHandler(this.About_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private Label lblVersion;
        private LinkLabel linkToGithub;
        private LinkLabel linkToDefaultShortcuts;
        private Label label4;
        private Label label5;
        private PictureBox picLogo;
    }
}