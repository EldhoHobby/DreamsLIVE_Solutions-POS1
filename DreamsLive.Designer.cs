using System;

namespace DreamsLIVE_Solutions_POS1
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.SQL_Status = new System.Windows.Forms.Label();
            this.Check_SQL_Status = new System.Windows.Forms.Button();
            this.LottoCloseing = new System.Windows.Forms.Button();
            this.LottoBalanceing = new System.Windows.Forms.Button();
            this.lblTime = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.LT_button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // SQL_Status
            // 
            resources.ApplyResources(this.SQL_Status, "SQL_Status");
            this.SQL_Status.Name = "SQL_Status";
            // 
            // Check_SQL_Status
            // 
            resources.ApplyResources(this.Check_SQL_Status, "Check_SQL_Status");
            this.Check_SQL_Status.Name = "Check_SQL_Status";
            this.Check_SQL_Status.TabStop = false;
            this.Check_SQL_Status.UseVisualStyleBackColor = true;
            // 
            // LottoCloseing
            // 
            this.LottoCloseing.BackColor = System.Drawing.Color.SandyBrown;
            resources.ApplyResources(this.LottoCloseing, "LottoCloseing");
            this.LottoCloseing.Name = "LottoCloseing";
            this.LottoCloseing.UseVisualStyleBackColor = false;
            this.LottoCloseing.Click += new System.EventHandler(this.LottoCloseing_Click);
            // 
            // LottoBalanceing
            // 
            this.LottoBalanceing.BackColor = System.Drawing.Color.Turquoise;
            resources.ApplyResources(this.LottoBalanceing, "LottoBalanceing");
            this.LottoBalanceing.Name = "LottoBalanceing";
            this.LottoBalanceing.UseVisualStyleBackColor = false;
            this.LottoBalanceing.Click += new System.EventHandler(this.LottoBalanceing_Click);
            // 
            // lblTime
            // 
            resources.ApplyResources(this.lblTime, "lblTime");
            this.lblTime.ForeColor = System.Drawing.Color.Blue;
            this.lblTime.Name = "lblTime";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // LT_button
            // 
            this.LT_button.BackColor = System.Drawing.SystemColors.Info;
            resources.ApplyResources(this.LT_button, "LT_button");
            this.LT_button.Name = "LT_button";
            this.LT_button.TabStop = false;
            this.LT_button.UseVisualStyleBackColor = false;
            this.LT_button.Click += new System.EventHandler(this.LT_button_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.Color.IndianRed;
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.Color.IndianRed;
            this.label4.Name = "label4";
            // 
            // Main
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LT_button);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.LottoBalanceing);
            this.Controls.Add(this.LottoCloseing);
            this.Controls.Add(this.Check_SQL_Status);
            this.Controls.Add(this.SQL_Status);
            this.Name = "Main";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
        #endregion

        private System.Windows.Forms.Label SQL_Status;
        private System.Windows.Forms.Button Check_SQL_Status;
        private System.Windows.Forms.Button LottoCloseing;
        private System.Windows.Forms.Button LottoBalanceing;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button LT_button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

