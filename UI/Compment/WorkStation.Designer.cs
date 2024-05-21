namespace UI
{
    partial class WorkStation
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
            this.tl_pnl = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lb_NG = new System.Windows.Forms.Label();
            this.lb_status = new System.Windows.Forms.Label();
            this.lb_disc = new System.Windows.Forms.Label();
            this.chk_b_close = new System.Windows.Forms.CheckBox();
            this.chk_f_on = new System.Windows.Forms.CheckBox();
            this.chk_f_close = new System.Windows.Forms.CheckBox();
            this.chk_b_on = new System.Windows.Forms.CheckBox();
            this.pnl_ws = new System.Windows.Forms.Panel();
            this.lb_pos_idx = new System.Windows.Forms.Label();
            this.tl_pnl.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tl_pnl
            // 
            this.tl_pnl.ColumnCount = 1;
            this.tl_pnl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tl_pnl.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tl_pnl.Controls.Add(this.pnl_ws, 0, 1);
            this.tl_pnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tl_pnl.Location = new System.Drawing.Point(0, 0);
            this.tl_pnl.Name = "tl_pnl";
            this.tl_pnl.RowCount = 2;
            this.tl_pnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tl_pnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tl_pnl.Size = new System.Drawing.Size(380, 115);
            this.tl_pnl.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 9;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 155F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.Controls.Add(this.lb_pos_idx, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lb_NG, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lb_status, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.lb_disc, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.chk_b_close, 8, 0);
            this.tableLayoutPanel1.Controls.Add(this.chk_f_on, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.chk_f_close, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.chk_b_on, 7, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(380, 18);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // lb_NG
            // 
            this.lb_NG.BackColor = System.Drawing.Color.Lime;
            this.lb_NG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_NG.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_NG.ForeColor = System.Drawing.Color.DimGray;
            this.lb_NG.Location = new System.Drawing.Point(197, 0);
            this.lb_NG.Margin = new System.Windows.Forms.Padding(0);
            this.lb_NG.Name = "lb_NG";
            this.lb_NG.Size = new System.Drawing.Size(65, 18);
            this.lb_NG.TabIndex = 18;
            this.lb_NG.Text = "NG:00.0%";
            this.lb_NG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_status
            // 
            this.lb_status.AutoSize = true;
            this.lb_status.Dock = System.Windows.Forms.DockStyle.Right;
            this.lb_status.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_status.ForeColor = System.Drawing.Color.DimGray;
            this.lb_status.Location = new System.Drawing.Point(269, 0);
            this.lb_status.Margin = new System.Windows.Forms.Padding(3, 0, 6, 0);
            this.lb_status.Name = "lb_status";
            this.lb_status.Size = new System.Drawing.Size(63, 18);
            this.lb_status.TabIndex = 8;
            this.lb_status.Text = "联机错误";
            this.lb_status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_disc
            // 
            this.lb_disc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_disc.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_disc.ForeColor = System.Drawing.Color.DimGray;
            this.lb_disc.Location = new System.Drawing.Point(0, 0);
            this.lb_disc.Margin = new System.Windows.Forms.Padding(0);
            this.lb_disc.Name = "lb_disc";
            this.lb_disc.Size = new System.Drawing.Size(42, 18);
            this.lb_disc.TabIndex = 3;
            this.lb_disc.Text = "工站0";
            this.lb_disc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chk_b_close
            // 
            this.chk_b_close.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_b_close.Checked = true;
            this.chk_b_close.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_b_close.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.chk_b_close.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.chk_b_close.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gold;
            this.chk_b_close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chk_b_close.Font = new System.Drawing.Font("宋体", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chk_b_close.Location = new System.Drawing.Point(370, 6);
            this.chk_b_close.Margin = new System.Windows.Forms.Padding(0, 0, 1, 4);
            this.chk_b_close.Name = "chk_b_close";
            this.chk_b_close.Size = new System.Drawing.Size(9, 8);
            this.chk_b_close.TabIndex = 15;
            this.chk_b_close.Text = " ";
            this.chk_b_close.UseVisualStyleBackColor = true;
            // 
            // chk_f_on
            // 
            this.chk_f_on.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_f_on.Checked = true;
            this.chk_f_on.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_f_on.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.chk_f_on.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.chk_f_on.FlatAppearance.CheckedBackColor = System.Drawing.Color.PaleGreen;
            this.chk_f_on.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chk_f_on.Font = new System.Drawing.Font("宋体", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chk_f_on.Location = new System.Drawing.Point(338, 6);
            this.chk_f_on.Margin = new System.Windows.Forms.Padding(0, 0, 1, 4);
            this.chk_f_on.Name = "chk_f_on";
            this.chk_f_on.Size = new System.Drawing.Size(9, 8);
            this.chk_f_on.TabIndex = 13;
            this.chk_f_on.UseVisualStyleBackColor = true;
            // 
            // chk_f_close
            // 
            this.chk_f_close.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_f_close.Checked = true;
            this.chk_f_close.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_f_close.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.chk_f_close.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.chk_f_close.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gold;
            this.chk_f_close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chk_f_close.Font = new System.Drawing.Font("宋体", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chk_f_close.Location = new System.Drawing.Point(348, 6);
            this.chk_f_close.Margin = new System.Windows.Forms.Padding(0, 0, 1, 4);
            this.chk_f_close.Name = "chk_f_close";
            this.chk_f_close.Size = new System.Drawing.Size(9, 8);
            this.chk_f_close.TabIndex = 14;
            this.chk_f_close.Text = " ";
            this.chk_f_close.UseVisualStyleBackColor = true;
            // 
            // chk_b_on
            // 
            this.chk_b_on.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_b_on.Checked = true;
            this.chk_b_on.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_b_on.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.chk_b_on.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.chk_b_on.FlatAppearance.CheckedBackColor = System.Drawing.Color.PaleGreen;
            this.chk_b_on.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chk_b_on.Font = new System.Drawing.Font("宋体", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chk_b_on.Location = new System.Drawing.Point(360, 6);
            this.chk_b_on.Margin = new System.Windows.Forms.Padding(0, 0, 1, 4);
            this.chk_b_on.Name = "chk_b_on";
            this.chk_b_on.Size = new System.Drawing.Size(9, 8);
            this.chk_b_on.TabIndex = 16;
            this.chk_b_on.Text = " ";
            this.chk_b_on.UseVisualStyleBackColor = true;
            // 
            // pnl_ws
            // 
            this.pnl_ws.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnl_ws.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_ws.Location = new System.Drawing.Point(3, 21);
            this.pnl_ws.Name = "pnl_ws";
            this.pnl_ws.Size = new System.Drawing.Size(374, 91);
            this.pnl_ws.TabIndex = 4;
            this.pnl_ws.Paint += new System.Windows.Forms.PaintEventHandler(this.pnl_ws_Paint);
            this.pnl_ws.DoubleClick += new System.EventHandler(this.pnl_ws_DoubleClick);
            // 
            // lb_pos_idx
            // 
            this.lb_pos_idx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_pos_idx.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_pos_idx.ForeColor = System.Drawing.Color.DimGray;
            this.lb_pos_idx.Location = new System.Drawing.Point(42, 0);
            this.lb_pos_idx.Margin = new System.Windows.Forms.Padding(0);
            this.lb_pos_idx.Name = "lb_pos_idx";
            this.lb_pos_idx.Size = new System.Drawing.Size(155, 18);
            this.lb_pos_idx.TabIndex = 19;
            this.lb_pos_idx.Text = "位置[0]▼ [0]  000.0s";
            this.lb_pos_idx.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // WorkStation
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.tl_pnl);
            this.DoubleBuffered = true;
            this.Name = "WorkStation";
            this.Size = new System.Drawing.Size(380, 115);
            this.tl_pnl.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tl_pnl;
        private System.Windows.Forms.Label lb_disc;
        private System.Windows.Forms.Label lb_status;
        private System.Windows.Forms.CheckBox chk_f_on;
        private System.Windows.Forms.CheckBox chk_f_close;
        private System.Windows.Forms.CheckBox chk_b_close;
        private System.Windows.Forms.CheckBox chk_b_on;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel pnl_ws;
        private System.Windows.Forms.Label lb_NG;
        private System.Windows.Forms.Label lb_pos_idx;
    }
}
