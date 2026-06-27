namespace SunnyPrjTemplate.Controls
{
    partial class ScanCode
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
            this.NICcomboBox = new System.Windows.Forms.ComboBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.DataText = new System.Windows.Forms.TextBox();
            this.liveviewForm1 = new Keyence.AutoID.SDK.LiveviewForm();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.SchBtn = new System.Windows.Forms.Button();
            this.SctBtn = new System.Windows.Forms.CheckBox();
            this.TgrBtn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // NICcomboBox
            // 
            this.NICcomboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NICcomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NICcomboBox.FormattingEnabled = true;
            this.NICcomboBox.Location = new System.Drawing.Point(3, 3);
            this.NICcomboBox.Name = "NICcomboBox";
            this.NICcomboBox.Size = new System.Drawing.Size(176, 20);
            this.NICcomboBox.TabIndex = 15;
            // 
            // comboBox1
            // 
            this.comboBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(3, 36);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(176, 20);
            this.comboBox1.TabIndex = 12;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(370, 387);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.DataText, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.liveviewForm1, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 119);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(364, 265);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // DataText
            // 
            this.DataText.BackColor = System.Drawing.SystemColors.Control;
            this.DataText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataText.Location = new System.Drawing.Point(3, 241);
            this.DataText.MaxLength = 10;
            this.DataText.Name = "DataText";
            this.DataText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.DataText.Size = new System.Drawing.Size(358, 21);
            this.DataText.TabIndex = 11;
            // 
            // liveviewForm1
            // 
            this.liveviewForm1.BackColor = System.Drawing.Color.Black;
            this.liveviewForm1.BinningType = Keyence.AutoID.SDK.LiveviewForm.ImageBinningType.OneQuarter;
            this.liveviewForm1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.liveviewForm1.ImageFormat = Keyence.AutoID.SDK.LiveviewForm.ImageFormatType.Jpeg;
            this.liveviewForm1.ImageQuality = 5;
            this.liveviewForm1.IpAddress = "192.168.100.100";
            this.liveviewForm1.Location = new System.Drawing.Point(3, 3);
            this.liveviewForm1.Name = "liveviewForm1";
            this.liveviewForm1.PullTimeSpan = 100;
            this.liveviewForm1.Size = new System.Drawing.Size(358, 232);
            this.liveviewForm1.TabIndex = 16;
            this.liveviewForm1.TimeoutMs = 2000;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.NICcomboBox, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.SchBtn, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.SctBtn, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.TgrBtn, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.comboBox1, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(364, 110);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // SchBtn
            // 
            this.SchBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SchBtn.Location = new System.Drawing.Point(185, 3);
            this.SchBtn.Name = "SchBtn";
            this.SchBtn.Size = new System.Drawing.Size(176, 27);
            this.SchBtn.TabIndex = 13;
            this.SchBtn.Text = "Search";
            this.SchBtn.UseVisualStyleBackColor = true;
            this.SchBtn.Click += new System.EventHandler(this.SchBtn_Click);
            // 
            // SctBtn
            // 
            this.SctBtn.Appearance = System.Windows.Forms.Appearance.Button;
            this.SctBtn.AutoSize = true;
            this.SctBtn.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.SctBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SctBtn.Enabled = false;
            this.SctBtn.Location = new System.Drawing.Point(185, 36);
            this.SctBtn.Name = "SctBtn";
            this.SctBtn.Size = new System.Drawing.Size(176, 27);
            this.SctBtn.TabIndex = 14;
            this.SctBtn.Text = "ReaderSelect";
            this.SctBtn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.SctBtn.UseVisualStyleBackColor = true;
            this.SctBtn.CheckedChanged += new System.EventHandler(this.SctBtn_CheckedChanged);
            // 
            // TgrBtn
            // 
            this.TgrBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TgrBtn.Enabled = false;
            this.TgrBtn.Location = new System.Drawing.Point(3, 69);
            this.TgrBtn.Name = "TgrBtn";
            this.TgrBtn.Size = new System.Drawing.Size(176, 38);
            this.TgrBtn.TabIndex = 10;
            this.TgrBtn.Text = "Trigger On";
            this.TgrBtn.UseVisualStyleBackColor = true;
            this.TgrBtn.Click += new System.EventHandler(this.TgrBtn_Click);
            // 
            // ScanCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ScanCode";
            this.Size = new System.Drawing.Size(370, 387);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox NICcomboBox;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button TgrBtn;
        private Keyence.AutoID.SDK.LiveviewForm liveviewForm1;
        private System.Windows.Forms.TextBox DataText;
        private System.Windows.Forms.Button SchBtn;
        private System.Windows.Forms.CheckBox SctBtn;
    }
}
