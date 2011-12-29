namespace ReQuizClient
{
    partial class frmQuestions
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
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnNextQuestion = new System.Windows.Forms.Button();
            this.btnPrevQuestion = new System.Windows.Forms.Button();
            this.lblQuestionNumber = new System.Windows.Forms.Label();
            this.btnHint = new System.Windows.Forms.Button();
            this.lblHints = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(386, 181);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(138, 25);
            this.btnSubmit.TabIndex = 0;
            this.btnSubmit.Text = "&Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnNextQuestion
            // 
            this.btnNextQuestion.Location = new System.Drawing.Point(156, 181);
            this.btnNextQuestion.Name = "btnNextQuestion";
            this.btnNextQuestion.Size = new System.Drawing.Size(138, 25);
            this.btnNextQuestion.TabIndex = 0;
            this.btnNextQuestion.Text = "&Next >>";
            this.btnNextQuestion.UseVisualStyleBackColor = true;
            this.btnNextQuestion.Click += new System.EventHandler(this.btnNextQuestion_Click);
            // 
            // btnPrevQuestion
            // 
            this.btnPrevQuestion.Location = new System.Drawing.Point(12, 181);
            this.btnPrevQuestion.Name = "btnPrevQuestion";
            this.btnPrevQuestion.Size = new System.Drawing.Size(138, 25);
            this.btnPrevQuestion.TabIndex = 0;
            this.btnPrevQuestion.Text = "<< &Previous";
            this.btnPrevQuestion.UseVisualStyleBackColor = true;
            this.btnPrevQuestion.Click += new System.EventHandler(this.btnPrevQuestion_Click);
            // 
            // lblQuestionNumber
            // 
            this.lblQuestionNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQuestionNumber.Location = new System.Drawing.Point(300, 181);
            this.lblQuestionNumber.Name = "lblQuestionNumber";
            this.lblQuestionNumber.Size = new System.Drawing.Size(80, 25);
            this.lblQuestionNumber.TabIndex = 1;
            this.lblQuestionNumber.Text = "1/15";
            this.lblQuestionNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnHint
            // 
            this.btnHint.Location = new System.Drawing.Point(386, 150);
            this.btnHint.Name = "btnHint";
            this.btnHint.Size = new System.Drawing.Size(138, 25);
            this.btnHint.TabIndex = 0;
            this.btnHint.Text = "&Hint";
            this.btnHint.UseVisualStyleBackColor = true;
            this.btnHint.Click += new System.EventHandler(this.btnHint_Click);
            // 
            // lblHints
            // 
            this.lblHints.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHints.Location = new System.Drawing.Point(12, 149);
            this.lblHints.Name = "lblHints";
            this.lblHints.Size = new System.Drawing.Size(368, 25);
            this.lblHints.TabIndex = 1;
            this.lblHints.Text = "Hints: 10";
            this.lblHints.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // frmQuestions
            // 
            this.AcceptButton = this.btnNextQuestion;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 218);
            this.Controls.Add(this.btnHint);
            this.Controls.Add(this.lblHints);
            this.Controls.Add(this.lblQuestionNumber);
            this.Controls.Add(this.btnPrevQuestion);
            this.Controls.Add(this.btnNextQuestion);
            this.Controls.Add(this.btnSubmit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmQuestions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ReQuiz";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmQuestions_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnNextQuestion;
        private System.Windows.Forms.Button btnPrevQuestion;
        private System.Windows.Forms.Label lblQuestionNumber;
        private System.Windows.Forms.Button btnHint;
        private System.Windows.Forms.Label lblHints;

    }
}