namespace MythicalLauncher
{
    partial class FrmLoading
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLoading));
            this.DragControl = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.logo = new System.Windows.Forms.PictureBox();
            this.lblappname = new System.Windows.Forms.Label();
            this.lblexit = new System.Windows.Forms.Label();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.logo)).BeginInit();
            this.SuspendLayout();
            // 
            // DragControl
            // 
            this.DragControl.TargetControl = this;
            // 
            // logo
            // 
            this.logo.Image = ((System.Drawing.Image)(resources.GetObject("logo.Image")));
            this.logo.Location = new System.Drawing.Point(65, 30);
            this.logo.Name = "logo";
            this.logo.Size = new System.Drawing.Size(118, 110);
            this.logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.logo.TabIndex = 0;
            this.logo.TabStop = false;
            // 
            // lblappname
            // 
            this.lblappname.AutoSize = true;
            this.lblappname.Font = new System.Drawing.Font("Microsoft Sans Serif", 19F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblappname.ForeColor = System.Drawing.Color.White;
            this.lblappname.Location = new System.Drawing.Point(13, 156);
            this.lblappname.Name = "lblappname";
            this.lblappname.Size = new System.Drawing.Size(222, 30);
            this.lblappname.TabIndex = 1;
            this.lblappname.Text = "MythicalSystems";
            // 
            // lblexit
            // 
            this.lblexit.AutoSize = true;
            this.lblexit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblexit.ForeColor = System.Drawing.Color.White;
            this.lblexit.Location = new System.Drawing.Point(218, 9);
            this.lblexit.Name = "lblexit";
            this.lblexit.Size = new System.Drawing.Size(18, 17);
            this.lblexit.TabIndex = 2;
            this.lblexit.Text = "X";
            this.lblexit.Click += new System.EventHandler(this.lblexit_Click);
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 25;
            this.guna2Elipse1.TargetControl = this;
            // 
            // FrmLoading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(248, 215);
            this.Controls.Add(this.lblexit);
            this.Controls.Add(this.lblappname);
            this.Controls.Add(this.logo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmLoading";
            this.Opacity = 0.8D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MythicalLauncher";
            ((System.ComponentModel.ISupportInitialize)(this.logo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2DragControl DragControl;
        private System.Windows.Forms.Label lblexit;
        private System.Windows.Forms.Label lblappname;
        private System.Windows.Forms.PictureBox logo;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
    }
}

