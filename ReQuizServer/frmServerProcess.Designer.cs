namespace ReQuizServer
{
    partial class frmServerProcess
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmServerProcess));
            this.btnFinish = new System.Windows.Forms.Button();
            this.lvLog = new System.Windows.Forms.ListView();
            this.chTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chEvent = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ttfrmServerProcess = new System.Windows.Forms.ToolTip(this.components);
            this.lblNoCompletedTest = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnFinish
            // 
            this.btnFinish.Location = new System.Drawing.Point(12, 393);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size(490, 47);
            this.btnFinish.TabIndex = 2;
            this.btnFinish.Text = "Finish the test";
            this.btnFinish.UseVisualStyleBackColor = true;
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            // 
            // lvLog
            // 
            this.lvLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chTime,
            this.chEvent});
            this.lvLog.GridLines = true;
            this.lvLog.Location = new System.Drawing.Point(12, 12);
            this.lvLog.Name = "lvLog";
            this.lvLog.Size = new System.Drawing.Size(490, 331);
            this.lvLog.TabIndex = 3;
            this.lvLog.UseCompatibleStateImageBehavior = false;
            this.lvLog.View = System.Windows.Forms.View.Details;
            // 
            // chTime
            // 
            this.chTime.Text = "Time";
            this.chTime.Width = 100;
            // 
            // chEvent
            // 
            this.chEvent.Text = "Event";
            this.chEvent.Width = 385;
            // 
            // lblNoCompletedTest
            // 
            this.lblNoCompletedTest.AutoSize = true;
            this.lblNoCompletedTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoCompletedTest.Location = new System.Drawing.Point(12, 358);
            this.lblNoCompletedTest.Name = "lblNoCompletedTest";
            this.lblNoCompletedTest.Size = new System.Drawing.Size(161, 20);
            this.lblNoCompletedTest.TabIndex = 4;
            this.lblNoCompletedTest.Text = "Completed the test: 0";
            // 
            // frmServerProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 452);
            this.Controls.Add(this.lblNoCompletedTest);
            this.Controls.Add(this.lvLog);
            this.Controls.Add(this.btnFinish);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmServerProcess";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ReQuiz Server - Main Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmServerProcess_FormClosing);
            this.Load += new System.EventHandler(this.frmServerProcess_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFinish;
        private System.Windows.Forms.ListView lvLog;
        private System.Windows.Forms.ColumnHeader chTime;
        private System.Windows.Forms.ColumnHeader chEvent;
        private System.Windows.Forms.ToolTip ttfrmServerProcess;
        private System.Windows.Forms.Label lblNoCompletedTest;
    }
}