namespace UI
{
    partial class Station
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
            this.lb_status = new System.Windows.Forms.Label();
            this.lb_disc = new System.Windows.Forms.Label();
            this.chk_b_close = new System.Windows.Forms.CheckBox();
            this.chk_b_on = new System.Windows.Forms.CheckBox();
            this.chk_f_on = new System.Windows.Forms.CheckBox();
            this.chk_f_close = new System.Windows.Forms.CheckBox();
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
            this.tl_pnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tl_pnl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tl_pnl.Size = new System.Drawing.Size(537, 339);
            this.tl_pnl.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 8;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.Controls.Add(this.lb_pos_idx, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lb_status, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lb_disc, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.chk_b_close, 7, 0);
            this.tableLayoutPanel1.Controls.Add(this.chk_b_on, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this.chk_f_on, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.chk_f_close, 4, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(531, 14);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // lb_status
            // 
            this.lb_status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_status.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_status.ForeColor = System.Drawing.Color.DimGray;
            this.lb_status.Location = new System.Drawing.Point(179, 0);
            this.lb_status.Name = "lb_status";
            this.lb_status.Size = new System.Drawing.Size(264, 14);
            this.lb_status.TabIndex = 8;
            this.lb_status.Text = "STATUS";
            this.lb_status.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb_disc
            // 
            this.lb_disc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_disc.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_disc.ForeColor = System.Drawing.Color.DimGray;
            this.lb_disc.Location = new System.Drawing.Point(3, 0);
            this.lb_disc.Name = "lb_disc";
            this.lb_disc.Size = new System.Drawing.Size(110, 14);
            this.lb_disc.TabIndex = 3;
            this.lb_disc.Text = "DISC";
            this.lb_disc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chk_b_close
            // 
            this.chk_b_close.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_b_close.Checked = true;
            this.chk_b_close.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_b_close.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chk_b_close.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.chk_b_close.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gold;
            this.chk_b_close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chk_b_close.Font = new System.Drawing.Font("宋体", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chk_b_close.Location = new System.Drawing.Point(517, 3);
            this.chk_b_close.Name = "chk_b_close";
            this.chk_b_close.Size = new System.Drawing.Size(11, 8);
            this.chk_b_close.TabIndex = 15;
            this.chk_b_close.Text = " ";
            this.chk_b_close.UseVisualStyleBackColor = true;
            // 
            // chk_b_on
            // 
            this.chk_b_on.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_b_on.Checked = true;
            this.chk_b_on.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_b_on.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chk_b_on.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.chk_b_on.FlatAppearance.CheckedBackColor = System.Drawing.Color.PaleGreen;
            this.chk_b_on.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chk_b_on.Font = new System.Drawing.Font("宋体", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chk_b_on.Location = new System.Drawing.Point(501, 3);
            this.chk_b_on.Name = "chk_b_on";
            this.chk_b_on.Size = new System.Drawing.Size(10, 8);
            this.chk_b_on.TabIndex = 16;
            this.chk_b_on.Text = " ";
            this.chk_b_on.UseVisualStyleBackColor = true;
            // 
            // chk_f_on
            // 
            this.chk_f_on.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_f_on.Checked = true;
            this.chk_f_on.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_f_on.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chk_f_on.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.chk_f_on.FlatAppearance.CheckedBackColor = System.Drawing.Color.PaleGreen;
            this.chk_f_on.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chk_f_on.Font = new System.Drawing.Font("宋体", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chk_f_on.Location = new System.Drawing.Point(449, 3);
            this.chk_f_on.Name = "chk_f_on";
            this.chk_f_on.Size = new System.Drawing.Size(10, 8);
            this.chk_f_on.TabIndex = 13;
            this.chk_f_on.UseVisualStyleBackColor = true;
            // 
            // chk_f_close
            // 
            this.chk_f_close.Appearance = System.Windows.Forms.Appearance.Button;
            this.chk_f_close.Checked = true;
            this.chk_f_close.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_f_close.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chk_f_close.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.chk_f_close.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gold;
            this.chk_f_close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chk_f_close.Font = new System.Drawing.Font("宋体", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chk_f_close.Location = new System.Drawing.Point(465, 3);
            this.chk_f_close.Name = "chk_f_close";
            this.chk_f_close.Size = new System.Drawing.Size(10, 8);
            this.chk_f_close.TabIndex = 14;
            this.chk_f_close.Text = " ";
            this.chk_f_close.UseVisualStyleBackColor = true;
            // 
            // pnl_ws
            // 
            this.pnl_ws.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnl_ws.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_ws.Location = new System.Drawing.Point(3, 23);
            this.pnl_ws.Name = "pnl_ws";
            this.pnl_ws.Size = new System.Drawing.Size(531, 313);
            this.pnl_ws.TabIndex = 4;
            this.pnl_ws.Paint += new System.Windows.Forms.PaintEventHandler(this.pnl_ws_Paint);
            // 
            // lb_pos_idx
            // 
            this.lb_pos_idx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_pos_idx.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_pos_idx.ForeColor = System.Drawing.Color.DimGray;
            this.lb_pos_idx.Location = new System.Drawing.Point(119, 0);
            this.lb_pos_idx.Name = "lb_pos_idx";
            this.lb_pos_idx.Size = new System.Drawing.Size(54, 14);
            this.lb_pos_idx.TabIndex = 17;
            this.lb_pos_idx.Text = "[0]";
            this.lb_pos_idx.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // WorkStation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tl_pnl);
            this.DoubleBuffered = true;
            this.Name = "WorkStation";
            this.Size = new System.Drawing.Size(537, 339);
            this.tl_pnl.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tl_pnl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lb_disc;
        private System.Windows.Forms.Label lb_status;
        private System.Windows.Forms.CheckBox chk_f_on;
        private System.Windows.Forms.CheckBox chk_f_close;
        private System.Windows.Forms.Panel pnl_ws;
        private System.Windows.Forms.CheckBox chk_b_close;
        private System.Windows.Forms.CheckBox chk_b_on;
        private System.Windows.Forms.Label lb_pos_idx;
    }
}
