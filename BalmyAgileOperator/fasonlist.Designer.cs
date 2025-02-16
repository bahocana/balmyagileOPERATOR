namespace BalmyAgilev1
{
    partial class fasonlist
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dvg_fasonlist = new Zuby.ADGV.AdvancedDataGridView();
            this.lbl_uretimemrino = new System.Windows.Forms.Label();
            this.txt_fasonformisim = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dvg_fasonlist)).BeginInit();
            this.SuspendLayout();
            // 
            // dvg_fasonlist
            // 
            this.dvg_fasonlist.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Tai Le", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dvg_fasonlist.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dvg_fasonlist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dvg_fasonlist.FilterAndSortEnabled = true;
            this.dvg_fasonlist.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.dvg_fasonlist.Location = new System.Drawing.Point(31, 129);
            this.dvg_fasonlist.Margin = new System.Windows.Forms.Padding(4);
            this.dvg_fasonlist.MaxFilterButtonImageHeight = 23;
            this.dvg_fasonlist.Name = "dvg_fasonlist";
            this.dvg_fasonlist.RightToLeft = System.Windows.Forms.RightToLeft.No;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Tai Le", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dvg_fasonlist.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dvg_fasonlist.RowHeadersWidth = 51;
            this.dvg_fasonlist.RowTemplate.Height = 24;
            this.dvg_fasonlist.Size = new System.Drawing.Size(834, 580);
            this.dvg_fasonlist.SortStringChangedInvokeBeforeDatasourceUpdate = true;
            this.dvg_fasonlist.TabIndex = 51;
            this.dvg_fasonlist.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dvg_fasonlist_CellDoubleClick);
            // 
            // lbl_uretimemrino
            // 
            this.lbl_uretimemrino.AutoSize = true;
            this.lbl_uretimemrino.Font = new System.Drawing.Font("Microsoft YaHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_uretimemrino.Location = new System.Drawing.Point(97, 37);
            this.lbl_uretimemrino.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbl_uretimemrino.Name = "lbl_uretimemrino";
            this.lbl_uretimemrino.Size = new System.Drawing.Size(104, 25);
            this.lbl_uretimemrino.TabIndex = 52;
            this.lbl_uretimemrino.Text = "Fason Seç";
            // 
            // txt_fasonformisim
            // 
            this.txt_fasonformisim.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txt_fasonformisim.Location = new System.Drawing.Point(278, 33);
            this.txt_fasonformisim.Margin = new System.Windows.Forms.Padding(5);
            this.txt_fasonformisim.Name = "txt_fasonformisim";
            this.txt_fasonformisim.Size = new System.Drawing.Size(176, 29);
            this.txt_fasonformisim.TabIndex = 76;
            // 
            // fasonlist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 734);
            this.Controls.Add(this.txt_fasonformisim);
            this.Controls.Add(this.lbl_uretimemrino);
            this.Controls.Add(this.dvg_fasonlist);
            this.Name = "fasonlist";
            this.Text = "fasonlist";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.fasonlist_FormClosed);
            this.Load += new System.EventHandler(this.fasonlist_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dvg_fasonlist)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public Zuby.ADGV.AdvancedDataGridView dvg_fasonlist;
        private System.Windows.Forms.Label lbl_uretimemrino;
        public System.Windows.Forms.TextBox txt_fasonformisim;
    }
}