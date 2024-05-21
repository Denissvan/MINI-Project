namespace UI.Compment
{
    partial class CameraList
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
            this.dgv_cam = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.disc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.model = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.triger = new Cognex.VisionPro.Implementation.CogGridViewComboBoxColumn();
            this.exposure = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.live = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_cam)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgv_cam
            // 
            this.dgv_cam.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_cam.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.name,
            this.disc,
            this.model,
            this.sn,
            this.ip,
            this.triger,
            this.exposure,
            this.live});
            this.dgv_cam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_cam.Location = new System.Drawing.Point(3, 3);
            this.dgv_cam.Name = "dgv_cam";
            this.dgv_cam.RowTemplate.Height = 23;
            this.dgv_cam.Size = new System.Drawing.Size(847, 294);
            this.dgv_cam.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.dgv_cam, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 76F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(853, 395);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 303);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(847, 89);
            this.panel1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(626, 35);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "刷新";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(729, 35);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "保存";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // name
            // 
            this.name.HeaderText = "名称";
            this.name.Name = "name";
            this.name.ReadOnly = true;
            // 
            // disc
            // 
            this.disc.HeaderText = "描述";
            this.disc.Name = "disc";
            this.disc.ReadOnly = true;
            // 
            // model
            // 
            this.model.HeaderText = "型号";
            this.model.Name = "model";
            this.model.ReadOnly = true;
            // 
            // sn
            // 
            this.sn.HeaderText = "序列号";
            this.sn.Name = "sn";
            this.sn.ReadOnly = true;
            this.sn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.sn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ip
            // 
            this.ip.HeaderText = "IP";
            this.ip.Name = "ip";
            this.ip.ReadOnly = true;
            // 
            // triger
            // 
            this.triger.Electric = false;
            this.triger.ElectricIconAlignment = System.Windows.Forms.ErrorIconAlignment.TopLeft;
            this.triger.ElectricIconHidden = false;
            this.triger.ElectricIconPadding = 0;
            this.triger.ErrorIconAlignment = System.Windows.Forms.ErrorIconAlignment.MiddleRight;
            this.triger.ErrorIconPadding = -36;
            this.triger.HeaderText = "触发模式";
            this.triger.Name = "triger";
            this.triger.Path = null;
            this.triger.ReadOnly = true;
            this.triger.SubjectInUseMode = Cognex.VisionPro.CogSubjectInUseModeConstants.ReadOnly;
            // 
            // exposure
            // 
            this.exposure.HeaderText = "曝光值(ms)";
            this.exposure.Name = "exposure";
            this.exposure.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.exposure.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // live
            // 
            this.live.HeaderText = "实况";
            this.live.Name = "live";
            this.live.ReadOnly = true;
            // 
            // CameraList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CameraList";
            this.Size = new System.Drawing.Size(853, 395);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_cam)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv_cam;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn disc;
        private System.Windows.Forms.DataGridViewTextBoxColumn model;
        private System.Windows.Forms.DataGridViewTextBoxColumn sn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ip;
        private Cognex.VisionPro.Implementation.CogGridViewComboBoxColumn triger;
        private System.Windows.Forms.DataGridViewTextBoxColumn exposure;
        private System.Windows.Forms.DataGridViewButtonColumn live;
    }
}
