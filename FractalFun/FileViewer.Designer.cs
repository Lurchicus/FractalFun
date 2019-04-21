namespace FractalFun
{
    partial class FileViewer
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
            this.TxtView = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TxtView
            // 
            this.TxtView.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.TxtView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TxtView.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtView.ForeColor = System.Drawing.SystemColors.Info;
            this.TxtView.Location = new System.Drawing.Point(0, 0);
            this.TxtView.Multiline = true;
            this.TxtView.Name = "TxtView";
            this.TxtView.ReadOnly = true;
            this.TxtView.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtView.Size = new System.Drawing.Size(662, 450);
            this.TxtView.TabIndex = 0;
            // 
            // FileViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 450);
            this.Controls.Add(this.TxtView);
            this.Name = "FileViewer";
            this.Text = "FileViewer";
            this.Load += new System.EventHandler(this.FileViewer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TxtView;
    }
}