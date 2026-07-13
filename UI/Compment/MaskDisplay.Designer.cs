namespace UI.Compment
{
     partial class MaskDisplay
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
            this.CogImageMask = new Cognex.VisionPro.CogImageMaskEditV2();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btn_cancal = new System.Windows.Forms.Button();
            this.btn_ok = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CogImageMask
            // 
            this.CogImageMask.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CogImageMask.Location = new System.Drawing.Point(0, 0);
            this.CogImageMask.Name = "CogImageMask";
            this.CogImageMask.Size = new System.Drawing.Size(700, 582);
            this.CogImageMask.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btn_cancal, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btn_ok, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(486, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(202, 40);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // btn_cancal
            // 
            this.btn_cancal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_cancal.Image = global::UI.Properties.Resources.delete_23_6px_1122573_easyicon_net;
            this.btn_cancal.Location = new System.Drawing.Point(104, 3);
            this.btn_cancal.Name = "btn_cancal";
            this.btn_cancal.Size = new System.Drawing.Size(95, 34);
            this.btn_cancal.TabIndex = 1;
            this.btn_cancal.Text = "取消";
            this.btn_cancal.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_cancal.UseVisualStyleBackColor = true;
            this.btn_cancal.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btn_ok
            // 
            this.btn_ok.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_ok.Image = global::UI.Properties.Resources.YES_23_6px_1122562_easyicon_net;
            this.btn_ok.Location = new System.Drawing.Point(3, 3);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(95, 34);
            this.btn_ok.TabIndex = 0;
            this.btn_ok.Text = "确认";
            this.btn_ok.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.BtnApply_Click);
            // 
            // MaskDisplay
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.CogImageMask);
            this.Name = "MaskDisplay";
            this.Size = new System.Drawing.Size(700, 582);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
     
        private Cognex.VisionPro.CogImageMaskEditV2 CogImageMask;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Button btn_cancal;
        public System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
