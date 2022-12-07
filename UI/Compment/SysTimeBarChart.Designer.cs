
namespace UI
{
    partial class SysTimeBarChart
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dateTimePickerLast = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.rbt_runrate = new System.Windows.Forms.RadioButton();
            this.rbt_almtime = new System.Windows.Forms.RadioButton();
            this.rbt_runTime = new System.Windows.Forms.RadioButton();
            this.rbt_StopTime = new System.Windows.Forms.RadioButton();
            this.rbt_allTime = new System.Windows.Forms.RadioButton();
            this.btn_clear = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btn_GetChart = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(3, 3);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(1276, 524);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // dateTimePickerLast
            // 
            this.dateTimePickerLast.CustomFormat = "yyyy-MM-dd HH:00 ";
            this.dateTimePickerLast.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dateTimePickerLast.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dateTimePickerLast.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerLast.Location = new System.Drawing.Point(3, 40);
            this.dateTimePickerLast.Name = "dateTimePickerLast";
            this.dateTimePickerLast.ShowUpDown = true;
            this.dateTimePickerLast.Size = new System.Drawing.Size(144, 23);
            this.dateTimePickerLast.TabIndex = 3;
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.CustomFormat = "yyyy-MM-dd HH:00 ";
            this.dateTimePickerStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dateTimePickerStart.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dateTimePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerStart.Location = new System.Drawing.Point(3, 3);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.ShowUpDown = true;
            this.dateTimePickerStart.Size = new System.Drawing.Size(144, 23);
            this.dateTimePickerStart.TabIndex = 2;
            // 
            // rbt_runrate
            // 
            this.rbt_runrate.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbt_runrate.AutoSize = true;
            this.rbt_runrate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbt_runrate.Location = new System.Drawing.Point(153, 40);
            this.rbt_runrate.Name = "rbt_runrate";
            this.rbt_runrate.Size = new System.Drawing.Size(74, 31);
            this.rbt_runrate.TabIndex = 4;
            this.rbt_runrate.TabStop = true;
            this.rbt_runrate.Text = "稼动率";
            this.rbt_runrate.UseVisualStyleBackColor = true;
            // 
            // rbt_almtime
            // 
            this.rbt_almtime.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbt_almtime.AutoSize = true;
            this.rbt_almtime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbt_almtime.Location = new System.Drawing.Point(233, 3);
            this.rbt_almtime.Name = "rbt_almtime";
            this.rbt_almtime.Size = new System.Drawing.Size(74, 31);
            this.rbt_almtime.TabIndex = 2;
            this.rbt_almtime.TabStop = true;
            this.rbt_almtime.Text = "报警时间";
            this.rbt_almtime.UseVisualStyleBackColor = true;
            // 
            // rbt_runTime
            // 
            this.rbt_runTime.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbt_runTime.AutoSize = true;
            this.rbt_runTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbt_runTime.Location = new System.Drawing.Point(153, 3);
            this.rbt_runTime.Name = "rbt_runTime";
            this.rbt_runTime.Size = new System.Drawing.Size(74, 31);
            this.rbt_runTime.TabIndex = 1;
            this.rbt_runTime.TabStop = true;
            this.rbt_runTime.Text = "运行时间";
            this.rbt_runTime.UseVisualStyleBackColor = true;
            // 
            // rbt_StopTime
            // 
            this.rbt_StopTime.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbt_StopTime.AutoSize = true;
            this.rbt_StopTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbt_StopTime.Location = new System.Drawing.Point(313, 3);
            this.rbt_StopTime.Name = "rbt_StopTime";
            this.rbt_StopTime.Size = new System.Drawing.Size(74, 31);
            this.rbt_StopTime.TabIndex = 0;
            this.rbt_StopTime.TabStop = true;
            this.rbt_StopTime.Text = "待机时间";
            this.rbt_StopTime.UseVisualStyleBackColor = true;
            // 
            // rbt_allTime
            // 
            this.rbt_allTime.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbt_allTime.AutoSize = true;
            this.rbt_allTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbt_allTime.Location = new System.Drawing.Point(233, 40);
            this.rbt_allTime.Name = "rbt_allTime";
            this.rbt_allTime.Size = new System.Drawing.Size(74, 31);
            this.rbt_allTime.TabIndex = 3;
            this.rbt_allTime.TabStop = true;
            this.rbt_allTime.Text = "汇总";
            this.rbt_allTime.UseVisualStyleBackColor = true;
            // 
            // btn_clear
            // 
            this.btn_clear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_clear.Location = new System.Drawing.Point(1159, 40);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(114, 31);
            this.btn_clear.TabIndex = 2;
            this.btn_clear.Text = "清除";
            this.btn_clear.UseVisualStyleBackColor = true;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.chart1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1282, 610);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 6;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel2.Controls.Add(this.dateTimePickerLast, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.rbt_runrate, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.dateTimePickerStart, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.rbt_runTime, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.rbt_allTime, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.rbt_StopTime, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.rbt_almtime, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.btn_clear, 5, 1);
            this.tableLayoutPanel2.Controls.Add(this.btn_GetChart, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.button1, 3, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 533);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1276, 74);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // btn_GetChart
            // 
            this.btn_GetChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_GetChart.Location = new System.Drawing.Point(1159, 3);
            this.btn_GetChart.Name = "btn_GetChart";
            this.btn_GetChart.Size = new System.Drawing.Size(114, 31);
            this.btn_GetChart.TabIndex = 5;
            this.btn_GetChart.Text = "查询";
            this.btn_GetChart.UseVisualStyleBackColor = true;
            this.btn_GetChart.Click += new System.EventHandler(this.btn_GetChart_Click);
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(313, 40);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(74, 31);
            this.button1.TabIndex = 6;
            this.button1.Text = "导出文件";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SysTimeBarChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SysTimeBarChart";
            this.Size = new System.Drawing.Size(1282, 610);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DateTimePicker dateTimePickerLast;
        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.RadioButton rbt_runrate;
        private System.Windows.Forms.RadioButton rbt_almtime;
        private System.Windows.Forms.RadioButton rbt_runTime;
        private System.Windows.Forms.RadioButton rbt_StopTime;
        private System.Windows.Forms.RadioButton rbt_allTime;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btn_GetChart;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}
