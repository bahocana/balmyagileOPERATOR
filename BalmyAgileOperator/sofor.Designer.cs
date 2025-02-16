namespace BalmyAgilev1
{
    partial class sofor
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
            this.lbl_uretimemrino = new System.Windows.Forms.Label();
            this.dvg_soforlist = new Zuby.ADGV.AdvancedDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dvg_soforlist)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_uretimemrino
            // 
            this.lbl_uretimemrino.AutoSize = true;
            this.lbl_uretimemrino.Font = new System.Drawing.Font("Microsoft YaHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lbl_uretimemrino.Location = new System.Drawing.Point(139, 9);
            this.lbl_uretimemrino.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbl_uretimemrino.Name = "lbl_uretimemrino";
            this.lbl_uretimemrino.Size = new System.Drawing.Size(99, 25);
            this.lbl_uretimemrino.TabIndex = 54;
            this.lbl_uretimemrino.Text = "Şöför Seç";
            // 
            // dvg_soforlist
            // 
            this.dvg_soforlist.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Tai Le", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dvg_soforlist.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dvg_soforlist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dvg_soforlist.FilterAndSortEnabled = true;
            this.dvg_soforlist.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.dvg_soforlist.Location = new System.Drawing.Point(51, 80);
            this.dvg_soforlist.Margin = new System.Windows.Forms.Padding(4);
            this.dvg_soforlist.MaxFilterButtonImageHeight = 23;
            this.dvg_soforlist.Name = "dvg_soforlist";
            this.dvg_soforlist.RightToLeft = System.Windows.Forms.RightToLeft.No;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Tai Le", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dvg_soforlist.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dvg_soforlist.RowHeadersWidth = 51;
            this.dvg_soforlist.RowTemplate.Height = 24;
            this.dvg_soforlist.Size = new System.Drawing.Size(513, 576);
            this.dvg_soforlist.SortStringChangedInvokeBeforeDatasourceUpdate = true;
            this.dvg_soforlist.TabIndex = 53;
            this.dvg_soforlist.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dvg_soforlist_CellDoubleClick);
            // 
            // sofor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 865);
            this.Controls.Add(this.lbl_uretimemrino);
            this.Controls.Add(this.dvg_soforlist);
            this.Name = "sofor";
            this.Text = "sofor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.sofor_FormClosed);
            this.Load += new System.EventHandler(this.sofor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dvg_soforlist)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_uretimemrino;
        public Zuby.ADGV.AdvancedDataGridView dvg_soforlist;
    }
}