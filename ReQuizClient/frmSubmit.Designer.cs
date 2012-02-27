namespace ReQuizClient
{
    partial class frmSubmit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSubmit));
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblConnectionProgress = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.ttfrmSubmit = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(467, 78);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Results";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblConnectionProgress
            // 
            this.lblConnectionProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectionProgress.Location = new System.Drawing.Point(15, 87);
            this.lblConnectionProgress.Name = "lblConnectionProgress";
            this.lblConnectionProgress.Size = new System.Drawing.Size(464, 236);
            this.lblConnectionProgress.TabIndex = 4;
            this.lblConnectionProgress.Text = "Connecting...";
            this.lblConnectionProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblResult
            // 
            this.lblResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult.Location = new System.Drawing.Point(12, 87);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(467, 236);
            this.lblResult.TabIndex = 5;
            this.lblResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblResult.Visible = false;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(12, 326);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(467, 40);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Visible = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmSubmit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 378);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblConnectionProgress);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblResult);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSubmit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ReQuiz Client - Results";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSubmit_FormClosed);
            this.Load += new System.EventHandler(this.frmSubmit_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblConnectionProgress;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ToolTip ttfrmSubmit;
    }
}