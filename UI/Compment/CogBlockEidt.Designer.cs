namespace UI
{
    partial class CogBlockEidt
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
            this.btn_triger = new System.Windows.Forms.Button();
            this.btn_save = new System.Windows.Forms.Button();
            this.btn_load = new System.Windows.Forms.Button();
            this.btn_run_image = new System.Windows.Forms.Button();
            this.Editor = new Cognex.VisionPro.ToolBlock.CogToolBlockEditV2();
            this.openFileDlg = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.Editor)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_triger
            // 
            this.btn_triger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_triger.Location = new System.Drawing.Point(580, 2);
            this.btn_triger.Name = "btn_triger";
            this.btn_triger.Size = new System.Drawing.Size(75, 23);
            this.btn_triger.TabIndex = 1;
            this.btn_triger.Text = "触发";
            this.btn_triger.UseVisualStyleBackColor = true;
            this.btn_triger.Click += new System.EventHandler(this.btn_triger_Click);
            // 
            // btn_save
            // 
            this.btn_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save.Location = new System.Drawing.Point(760, 2);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(75, 23);
            this.btn_save.TabIndex = 2;
            this.btn_save.Text = "保存";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // btn_load
            // 
            this.btn_load.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_load.Location = new System.Drawing.Point(680, 2);
            this.btn_load.Name = "btn_load";
            this.btn_load.Size = new System.Drawing.Size(75, 23);
            this.btn_load.TabIndex = 3;
            this.btn_load.Text = "加载";
            this.btn_load.UseVisualStyleBackColor = true;
            this.btn_load.Click += new System.EventHandler(this.btn_load_Click);
            // 
            // btn_run_image
            // 
            this.btn_run_image.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_run_image.Location = new System.Drawing.Point(499, 2);
            this.btn_run_image.Name = "btn_run_image";
            this.btn_run_image.Size = new System.Drawing.Size(75, 23);
            this.btn_run_image.TabIndex = 4;
            this.btn_run_image.Text = "图片";
            this.btn_run_image.UseVisualStyleBackColor = true;
            this.btn_run_image.Click += new System.EventHandler(this.button1_Click);
            // 
            // Editor
            // 
            this.Editor.AllowDrop = true;
            this.Editor.ContextMenuCustomizer = null;
            this.Editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Editor.Location = new System.Drawing.Point(0, 0);
            this.Editor.Margin = new System.Windows.Forms.Padding(0);
            this.Editor.MinimumSize = new System.Drawing.Size(489, 0);
            this.Editor.Name = "Editor";
            this.Editor.ShowNodeToolTips = true;
            this.Editor.Size = new System.Drawing.Size(853, 542);
            this.Editor.SuspendElectricRuns = false;
            this.Editor.TabIndex = 5;
            // 
            // openFileDlg
            // 
            this.openFileDlg.FileName = "openFileDialog1";
            // 
            // CogBlockEidt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_run_image);
            this.Controls.Add(this.btn_load);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.btn_triger);
            this.Controls.Add(this.Editor);
            this.Name = "CogBlockEidt";
            this.Size = new System.Drawing.Size(853, 542);
            ((System.ComponentModel.ISupportInitialize)(this.Editor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btn_triger;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Button btn_load;
        private System.Windows.Forms.Button btn_run_image;
        private Cognex.VisionPro.ToolBlock.CogToolBlockEditV2 Editor;
        private System.Windows.Forms.OpenFileDialog openFileDlg;
    }
}
