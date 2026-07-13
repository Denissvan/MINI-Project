namespace UI
{
    partial class SQLSelector
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
            this.dtpicker_end = new System.Windows.Forms.DateTimePicker();
            this.dtpicker_from = new System.Windows.Forms.DateTimePicker();
            this.cb_num = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.cb_ws = new System.Windows.Forms.ComboBox();
            this.cb_pc = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.cb_testbox = new System.Windows.Forms.ComboBox();
            this.tb_barcode = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cb_res = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lb_data_status = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.btn_clear = new System.Windows.Forms.Button();
            this.btn_select = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbtn_RetProduct = new System.Windows.Forms.RadioButton();
            this.rbtn_NorProduct = new System.Windows.Forms.RadioButton();
            this.rbtn_AllProduct = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dtpicker_end
            // 
            this.dtpicker_end.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dtpicker_end.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtpicker_end.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtpicker_end.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpicker_end.Location = new System.Drawing.Point(3, 43);
            this.dtpicker_end.Name = "dtpicker_end";
            this.dtpicker_end.ShowUpDown = true;
            this.dtpicker_end.Size = new System.Drawing.Size(224, 35);
            this.dtpicker_end.TabIndex = 92;
            this.dtpicker_end.Value = new System.DateTime(2018, 10, 23, 10, 1, 59, 0);
            // 
            // dtpicker_from
            // 
            this.dtpicker_from.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dtpicker_from.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtpicker_from.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtpicker_from.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpicker_from.Location = new System.Drawing.Point(3, 3);
            this.dtpicker_from.Name = "dtpicker_from";
            this.dtpicker_from.ShowUpDown = true;
            this.dtpicker_from.Size = new System.Drawing.Size(224, 35);
            this.dtpicker_from.TabIndex = 76;
            // 
            // cb_num
            // 
            this.cb_num.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_num.FormattingEnabled = true;
            this.cb_num.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16"});
            this.cb_num.Location = new System.Drawing.Point(598, 3);
            this.cb_num.Name = "cb_num";
            this.cb_num.Size = new System.Drawing.Size(64, 29);
            this.cb_num.TabIndex = 87;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(526, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(66, 40);
            this.label10.TabIndex = 88;
            this.label10.Text = "编号";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label16.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label16.Location = new System.Drawing.Point(233, 40);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(70, 40);
            this.label16.TabIndex = 89;
            this.label16.Text = "NG码";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label15.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label15.Location = new System.Drawing.Point(381, 40);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(49, 40);
            this.label15.TabIndex = 84;
            this.label15.Text = "电脑";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cb_ws
            // 
            this.cb_ws.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_ws.FormattingEnabled = true;
            this.cb_ws.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3"});
            this.cb_ws.Location = new System.Drawing.Point(436, 3);
            this.cb_ws.Name = "cb_ws";
            this.cb_ws.Size = new System.Drawing.Size(84, 29);
            this.cb_ws.TabIndex = 79;
            // 
            // cb_pc
            // 
            this.cb_pc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_pc.FormattingEnabled = true;
            this.cb_pc.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8"});
            this.cb_pc.Location = new System.Drawing.Point(436, 43);
            this.cb_pc.Name = "cb_pc";
            this.cb_pc.Size = new System.Drawing.Size(84, 29);
            this.cb_pc.TabIndex = 83;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label13.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(381, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(49, 40);
            this.label13.TabIndex = 80;
            this.label13.Text = "工站";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(233, 3);
            this.label9.Margin = new System.Windows.Forms.Padding(3);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 34);
            this.label9.TabIndex = 78;
            this.label9.Text = "二维码";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label14.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.Location = new System.Drawing.Point(526, 40);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(66, 40);
            this.label14.TabIndex = 82;
            this.label14.Text = "工装";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cb_testbox
            // 
            this.cb_testbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_testbox.FormattingEnabled = true;
            this.cb_testbox.Items.AddRange(new object[] {
            "0",
            "1"});
            this.cb_testbox.Location = new System.Drawing.Point(598, 43);
            this.cb_testbox.Name = "cb_testbox";
            this.cb_testbox.Size = new System.Drawing.Size(64, 29);
            this.cb_testbox.TabIndex = 81;
            // 
            // tb_barcode
            // 
            this.tb_barcode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_barcode.Location = new System.Drawing.Point(309, 3);
            this.tb_barcode.Name = "tb_barcode";
            this.tb_barcode.Size = new System.Drawing.Size(66, 32);
            this.tb_barcode.TabIndex = 77;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 7;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 72F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel1.Controls.Add(this.cb_res, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.dtpicker_from, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dtpicker_end, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cb_num, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this.label9, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cb_testbox, 6, 1);
            this.tableLayoutPanel1.Controls.Add(this.label10, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.label14, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this.label15, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.cb_pc, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.cb_ws, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.label16, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tb_barcode, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label13, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(665, 80);
            this.tableLayoutPanel1.TabIndex = 93;
            // 
            // cb_res
            // 
            this.cb_res.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_res.FormattingEnabled = true;
            this.cb_res.Items.AddRange(new object[] {
            "",
            "未测",
            "OK",
            "NG",
            "%266",
            "266"});
            this.cb_res.Location = new System.Drawing.Point(309, 43);
            this.cb_res.Name = "cb_res";
            this.cb_res.Size = new System.Drawing.Size(66, 29);
            this.cb_res.TabIndex = 93;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 181F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(904, 86);
            this.tableLayoutPanel2.TabIndex = 94;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.lb_data_status, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(726, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(175, 80);
            this.tableLayoutPanel3.TabIndex = 94;
            // 
            // lb_data_status
            // 
            this.lb_data_status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_data_status.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_data_status.ForeColor = System.Drawing.Color.Gray;
            this.lb_data_status.Location = new System.Drawing.Point(3, 0);
            this.lb_data_status.Name = "lb_data_status";
            this.lb_data_status.Size = new System.Drawing.Size(169, 32);
            this.lb_data_status.TabIndex = 97;
            this.lb_data_status.Text = "T20181023...2,000ms\r\n00000";
            this.lb_data_status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.btn_clear, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.btn_select, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 35);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(169, 42);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // btn_clear
            // 
            this.btn_clear.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_clear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_clear.Font = new System.Drawing.Font("宋体", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_clear.Location = new System.Drawing.Point(87, 3);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(79, 36);
            this.btn_clear.TabIndex = 96;
            this.btn_clear.Text = "清除";
            this.btn_clear.UseVisualStyleBackColor = false;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // btn_select
            // 
            this.btn_select.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_select.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_select.Font = new System.Drawing.Font("宋体", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_select.Location = new System.Drawing.Point(3, 3);
            this.btn_select.Name = "btn_select";
            this.btn_select.Size = new System.Drawing.Size(78, 36);
            this.btn_select.TabIndex = 95;
            this.btn_select.Text = "查询";
            this.btn_select.UseVisualStyleBackColor = false;
            this.btn_select.Click += new System.EventHandler(this.btn_select_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbtn_RetProduct);
            this.panel1.Controls.Add(this.rbtn_NorProduct);
            this.panel1.Controls.Add(this.rbtn_AllProduct);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(674, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(46, 80);
            this.panel1.TabIndex = 95;
            // 
            // rbtn_RetProduct
            // 
            this.rbtn_RetProduct.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbtn_RetProduct.FlatAppearance.CheckedBackColor = System.Drawing.Color.Yellow;
            this.rbtn_RetProduct.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbtn_RetProduct.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtn_RetProduct.ForeColor = System.Drawing.Color.Black;
            this.rbtn_RetProduct.Location = new System.Drawing.Point(1, 55);
            this.rbtn_RetProduct.Name = "rbtn_RetProduct";
            this.rbtn_RetProduct.Size = new System.Drawing.Size(45, 22);
            this.rbtn_RetProduct.TabIndex = 128;
            this.rbtn_RetProduct.Text = "复测";
            this.rbtn_RetProduct.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.rbtn_RetProduct.UseVisualStyleBackColor = true;
            // 
            // rbtn_NorProduct
            // 
            this.rbtn_NorProduct.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbtn_NorProduct.FlatAppearance.CheckedBackColor = System.Drawing.Color.Yellow;
            this.rbtn_NorProduct.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbtn_NorProduct.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtn_NorProduct.ForeColor = System.Drawing.Color.Black;
            this.rbtn_NorProduct.Location = new System.Drawing.Point(1, 28);
            this.rbtn_NorProduct.Name = "rbtn_NorProduct";
            this.rbtn_NorProduct.Size = new System.Drawing.Size(45, 22);
            this.rbtn_NorProduct.TabIndex = 127;
            this.rbtn_NorProduct.Text = "正常";
            this.rbtn_NorProduct.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbtn_NorProduct.UseVisualStyleBackColor = true;
            // 
            // rbtn_AllProduct
            // 
            this.rbtn_AllProduct.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbtn_AllProduct.Checked = true;
            this.rbtn_AllProduct.FlatAppearance.CheckedBackColor = System.Drawing.Color.Yellow;
            this.rbtn_AllProduct.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbtn_AllProduct.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtn_AllProduct.ForeColor = System.Drawing.Color.Black;
            this.rbtn_AllProduct.Location = new System.Drawing.Point(1, 1);
            this.rbtn_AllProduct.Name = "rbtn_AllProduct";
            this.rbtn_AllProduct.Size = new System.Drawing.Size(45, 22);
            this.rbtn_AllProduct.TabIndex = 126;
            this.rbtn_AllProduct.TabStop = true;
            this.rbtn_AllProduct.Text = "全部";
            this.rbtn_AllProduct.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.rbtn_AllProduct.UseVisualStyleBackColor = true;
            // 
            // SQLSelector
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "SQLSelector";
            this.Size = new System.Drawing.Size(904, 86);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpicker_end;
        private System.Windows.Forms.DateTimePicker dtpicker_from;
        private System.Windows.Forms.ComboBox cb_num;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cb_ws;
        private System.Windows.Forms.ComboBox cb_pc;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cb_testbox;
        private System.Windows.Forms.TextBox tb_barcode;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label lb_data_status;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.ComboBox cb_res;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.Button btn_select;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.RadioButton rbtn_AllProduct;
        public System.Windows.Forms.RadioButton rbtn_RetProduct;
        public System.Windows.Forms.RadioButton rbtn_NorProduct;
    }
}
