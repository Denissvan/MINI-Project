
namespace UI
{
    partial class DataGrideSysInfo
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.MyDge = new UI.DefineDataGridView();
            this.Column1 = new UI.DataGridViewGroupColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.MyDge)).BeginInit();
            this.SuspendLayout();
            // 
            // MyDge
            // 
            this.MyDge.AllowUserToAddRows = false;
            this.MyDge.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MyDge.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            this.MyDge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MyDge.Location = new System.Drawing.Point(0, 0);
            this.MyDge.Name = "MyDge";
            this.MyDge.RowHeadersWidth = 30;
            this.MyDge.RowTemplate.Height = 23;
            this.MyDge.ShowRowHeaderNumbers = true;
            this.MyDge.Size = new System.Drawing.Size(868, 494);
            this.MyDge.TabIndex = 1;
            this.MyDge.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.MyDge_CellMouseDoubleClick);
            // 
            // Column1
            // 
            this.Column1.FillWeight = 200F;
            this.Column1.HeaderText = "名称";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column1.Width = 250;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "解释";
            this.Column2.Name = "Column2";
            this.Column2.Width = 250;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "值";
            this.Column3.Name = "Column3";
            this.Column3.Width = 200;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "备注";
            this.Column4.Name = "Column4";
            this.Column4.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // DataGrideSysInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MyDge);
            this.Name = "DataGrideSysInfo";
            this.Size = new System.Drawing.Size(868, 494);
            ((System.ComponentModel.ISupportInitialize)(this.MyDge)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DefineDataGridView MyDge;
        private DataGridViewGroupColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
    }
}
