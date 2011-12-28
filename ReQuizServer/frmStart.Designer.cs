namespace ReQuizServer
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
            this.button1 = new System.Windows.Forms.Button();
            this.gbSettings = new System.Windows.Forms.GroupBox();
            this.nudPortNumber = new System.Windows.Forms.NumericUpDown();
            this.nudNoHints = new System.Windows.Forms.NumericUpDown();
            this.nudNoQuestions = new System.Windows.Forms.NumericUpDown();
            this.lblServerPort = new System.Windows.Forms.Label();
            this.lblNoHints = new System.Windows.Forms.Label();
            this.lblNoQuestions = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.gbSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPortNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNoHints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNoQuestions)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 251);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(377, 47);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // gbSettings
            // 
            this.gbSettings.Controls.Add(this.nudPortNumber);
            this.gbSettings.Controls.Add(this.nudNoHints);
            this.gbSettings.Controls.Add(this.nudNoQuestions);
            this.gbSettings.Controls.Add(this.lblServerPort);
            this.gbSettings.Controls.Add(this.lblNoHints);
            this.gbSettings.Controls.Add(this.lblNoQuestions);
            this.gbSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbSettings.Location = new System.Drawing.Point(12, 90);
            this.gbSettings.Name = "gbSettings";
            this.gbSettings.Size = new System.Drawing.Size(377, 154);
            this.gbSettings.TabIndex = 2;
            this.gbSettings.TabStop = false;
            this.gbSettings.Text = "Settings";
            // 
            // nudPortNumber
            // 
            this.nudPortNumber.Location = new System.Drawing.Point(251, 113);
            this.nudPortNumber.Maximum = new decimal(new int[] {
            49151,
            0,
            0,
            0});
            this.nudPortNumber.Minimum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.nudPortNumber.Name = "nudPortNumber";
            this.nudPortNumber.Size = new System.Drawing.Size(120, 26);
            this.nudPortNumber.TabIndex = 1;
            this.nudPortNumber.Value = new decimal(new int[] {
            1234,
            0,
            0,
            0});
            // 
            // nudNoHints
            // 
            this.nudNoHints.Location = new System.Drawing.Point(251, 71);
            this.nudNoHints.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudNoHints.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudNoHints.Name = "nudNoHints";
            this.nudNoHints.Size = new System.Drawing.Size(120, 26);
            this.nudNoHints.TabIndex = 1;
            this.nudNoHints.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // nudNoQuestions
            // 
            this.nudNoQuestions.Location = new System.Drawing.Point(251, 29);
            this.nudNoQuestions.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudNoQuestions.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudNoQuestions.Name = "nudNoQuestions";
            this.nudNoQuestions.Size = new System.Drawing.Size(120, 26);
            this.nudNoQuestions.TabIndex = 1;
            this.nudNoQuestions.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudNoQuestions.ValueChanged += new System.EventHandler(this.nudNoQuestions_ValueChanged);
            // 
            // lblServerPort
            // 
            this.lblServerPort.AutoSize = true;
            this.lblServerPort.Location = new System.Drawing.Point(6, 115);
            this.lblServerPort.Name = "lblServerPort";
            this.lblServerPort.Size = new System.Drawing.Size(191, 20);
            this.lblServerPort.TabIndex = 0;
            this.lblServerPort.Text = "Server port (1024-49151):";
            // 
            // lblNoHints
            // 
            this.lblNoHints.AutoSize = true;
            this.lblNoHints.Location = new System.Drawing.Point(6, 71);
            this.lblNoHints.Name = "lblNoHints";
            this.lblNoHints.Size = new System.Drawing.Size(182, 20);
            this.lblNoHints.TabIndex = 0;
            this.lblNoHints.Text = "Number of hints allowed:";
            // 
            // lblNoQuestions
            // 
            this.lblNoQuestions.AutoSize = true;
            this.lblNoQuestions.Location = new System.Drawing.Point(6, 31);
            this.lblNoQuestions.Name = "lblNoQuestions";
            this.lblNoQuestions.Size = new System.Drawing.Size(210, 20);
            this.lblNoQuestions.TabIndex = 0;
            this.lblNoQuestions.Text = "Number of questions (1-50): ";
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(377, 78);
            this.lblTitle.TabIndex = 3;
            this.lblTitle.Text = "Welcome to ReQuiz!";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 310);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.gbSettings);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmStart";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ReQuiz Server";
            this.gbSettings.ResumeLayout(false);
            this.gbSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPortNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNoHints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNoQuestions)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox gbSettings;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblNoQuestions;
        private System.Windows.Forms.NumericUpDown nudNoQuestions;
        private System.Windows.Forms.Label lblNoHints;
        private System.Windows.Forms.NumericUpDown nudNoHints;
        private System.Windows.Forms.Label lblServerPort;
        private System.Windows.Forms.NumericUpDown nudPortNumber;
    }
}

