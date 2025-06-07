using System.Windows.Forms;

namespace DreamsLIVE_Solutions_POS1
{
    partial class LottoBalanceingHistory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LottoBalanceingHistory));
            this.LottoBalanceingHistView = new System.Windows.Forms.DataGridView();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnNextDay = new System.Windows.Forms.Button();
            this.btnPreviousDay = new System.Windows.Forms.Button();
            this.dateTimePickerHist = new System.Windows.Forms.DateTimePicker();
            this.ViewByDate = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.LottoBalanceingHistView)).BeginInit();
            this.SuspendLayout();
            // 
            // LottoBalanceingHistView
            // 
            this.LottoBalanceingHistView.AllowUserToAddRows = false;
            this.LottoBalanceingHistView.AllowUserToDeleteRows = false;
            this.LottoBalanceingHistView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.LottoBalanceingHistView.Dock = System.Windows.Forms.DockStyle.Top;
            this.LottoBalanceingHistView.Location = new System.Drawing.Point(0, 0);
            this.LottoBalanceingHistView.Name = "LottoBalanceingHistView";
            this.LottoBalanceingHistView.ReadOnly = true;
            this.LottoBalanceingHistView.Size = new System.Drawing.Size(1459, 395);
            this.LottoBalanceingHistView.TabIndex = 111;
            this.LottoBalanceingHistView.TabStop = false;
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Location = new System.Drawing.Point(1367, 434);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(75, 23);
            this.btnMoveUp.TabIndex = 0;
            this.btnMoveUp.Text = "Move Up";
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Location = new System.Drawing.Point(1367, 462);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(75, 23);
            this.btnMoveDown.TabIndex = 1;
            this.btnMoveDown.Text = "Move Down";
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnNextDay
            // 
            this.btnNextDay.Location = new System.Drawing.Point(1251, 434);
            this.btnNextDay.Name = "btnNextDay";
            this.btnNextDay.Size = new System.Drawing.Size(90, 23);
            this.btnNextDay.TabIndex = 3;
            this.btnNextDay.Text = "Next Day";
            this.btnNextDay.Click += new System.EventHandler(this.btnNextDay_Click);
            // 
            // btnPreviousDay
            // 
            this.btnPreviousDay.Location = new System.Drawing.Point(1251, 462);
            this.btnPreviousDay.Name = "btnPreviousDay";
            this.btnPreviousDay.Size = new System.Drawing.Size(90, 23);
            this.btnPreviousDay.TabIndex = 2;
            this.btnPreviousDay.Text = "Previous Day";
            this.btnPreviousDay.Click += new System.EventHandler(this.btnPreviousDay_Click);
            // 
            // dateTimePickerHist
            // 
            this.dateTimePickerHist.Location = new System.Drawing.Point(1014, 411);
            this.dateTimePickerHist.Name = "dateTimePickerHist";
            this.dateTimePickerHist.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerHist.TabIndex = 112;
            this.dateTimePickerHist.ValueChanged += new System.EventHandler(this.dateTimePickerHist_ValueChanged);
            // 
            // ViewByDate
            // 
            this.ViewByDate.AutoSize = true;
            this.ViewByDate.Location = new System.Drawing.Point(1251, 411);
            this.ViewByDate.Name = "ViewByDate";
            this.ViewByDate.Size = new System.Drawing.Size(90, 17);
            this.ViewByDate.TabIndex = 113;
            this.ViewByDate.Text = "View By Date";
            this.ViewByDate.UseVisualStyleBackColor = true;
            this.ViewByDate.CheckedChanged += new System.EventHandler(this.ViewByDate_CheckedChanged);
            // 
            // LottoBalanceingHistory
            // 
            this.ClientSize = new System.Drawing.Size(1459, 490);
            this.Controls.Add(this.ViewByDate);
            this.Controls.Add(this.dateTimePickerHist);
            this.Controls.Add(this.btnNextDay);
            this.Controls.Add(this.btnPreviousDay);
            this.Controls.Add(this.btnMoveDown);
            this.Controls.Add(this.btnMoveUp);
            this.Controls.Add(this.LottoBalanceingHistView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LottoBalanceingHistory";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lotto Balancing History";
            ((System.ComponentModel.ISupportInitialize)(this.LottoBalanceingHistView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DataGridView LottoBalanceingHistView;
        private Button btnMoveUp;
        private Button btnMoveDown;
        private Button btnNextDay;
        private Button btnPreviousDay;
        private DateTimePicker dateTimePickerHist;
        private CheckBox ViewByDate;
    }
}