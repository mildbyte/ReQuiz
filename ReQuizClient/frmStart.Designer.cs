namespace ReQuizClient
{
    partial class frmStart
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmStart));
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblIPAddress = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.lblConnectionProgress = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.ttfrmStart = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(424, 78);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Welcome to ReQuiz!";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblIPAddress
            // 
            this.lblIPAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIPAddress.Location = new System.Drawing.Point(12, 70);
            this.lblIPAddress.Name = "lblIPAddress";
            this.lblIPAddress.Size = new System.Drawing.Size(276, 69);
            this.lblIPAddress.TabIndex = 0;
            this.lblIPAddress.Text = "Server IP address:";
            this.lblIPAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 209);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(429, 37);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtAddress
            // 
            this.txtAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddress.Location = new System.Drawing.Point(230, 93);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(206, 26);
            this.txtAddress.TabIndex = 2;
            // 
            // lblConnectionProgress
            // 
            this.lblConnectionProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectionProgress.Location = new System.Drawing.Point(12, 70);
            this.lblConnectionProgress.Name = "lblConnectionProgress";
            this.lblConnectionProgress.Size = new System.Drawing.Size(429, 103);
            this.lblConnectionProgress.TabIndex = 3;
            this.lblConnectionProgress.Text = "Connecting...";
            this.lblConnectionProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblConnectionProgress.Visible = false;
            // 
            // lblPort
            // 
            this.lblPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPort.Location = new System.Drawing.Point(12, 121);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(212, 69);
            this.lblPort.TabIndex = 0;
            this.lblPort.Text = "Port:";
            this.lblPort.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPort
            // 
            this.txtPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPort.Location = new System.Drawing.Point(230, 144);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(206, 26);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "1234";
            // 
            // frmStart
            // 
            this.AcceptButton = this.btnConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 258);
            this.Controls.Add(this.lblConnectionProgress);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.lblIPAddress);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmStart";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ReQuiz Client - Startup";
            this.Load += new System.EventHandler(this.frmStart_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblIPAddress;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label lblConnectionProgress;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.ToolTip ttfrmStart;
    }
}

