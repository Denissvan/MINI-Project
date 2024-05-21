namespace MotionCtrl
{
    partial class XYZ
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
            this.nud_z = new System.Windows.Forms.NumericUpDown();
            this.lb_z = new System.Windows.Forms.Label();
            this.nud_y = new System.Windows.Forms.NumericUpDown();
            this.lb_y = new System.Windows.Forms.Label();
            this.nud_x = new System.Windows.Forms.NumericUpDown();
            this.lb_x = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nud_z)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_x)).BeginInit();
            this.SuspendLayout();
            // 
            // nud_z
            // 
            this.nud_z.DecimalPlaces = 3;
            this.nud_z.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_z.Location = new System.Drawing.Point(223, 17);
            this.nud_z.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nud_z.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nud_z.Name = "nud_z";
            this.nud_z.Size = new System.Drawing.Size(108, 32);
            this.nud_z.TabIndex = 39;
            this.nud_z.Value = new decimal(new int[] {
            123,
            0,
            0,
            0});
            this.nud_z.ValueChanged += new System.EventHandler(this.nud_x_ValueChanged);
            // 
            // lb_z
            // 
            this.lb_z.AutoSize = true;
            this.lb_z.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_z.Location = new System.Drawing.Point(220, 0);
            this.lb_z.Name = "lb_z";
            this.lb_z.Size = new System.Drawing.Size(15, 16);
            this.lb_z.TabIndex = 38;
            this.lb_z.Text = "Z";
            // 
            // nud_y
            // 
            this.nud_y.DecimalPlaces = 3;
            this.nud_y.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_y.Location = new System.Drawing.Point(113, 17);
            this.nud_y.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nud_y.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nud_y.Name = "nud_y";
            this.nud_y.Size = new System.Drawing.Size(108, 32);
            this.nud_y.TabIndex = 37;
            this.nud_y.Value = new decimal(new int[] {
            123,
            0,
            0,
            0});
            this.nud_y.ValueChanged += new System.EventHandler(this.nud_x_ValueChanged);
            // 
            // lb_y
            // 
            this.lb_y.AutoSize = true;
            this.lb_y.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_y.Location = new System.Drawing.Point(110, 0);
            this.lb_y.Name = "lb_y";
            this.lb_y.Size = new System.Drawing.Size(15, 16);
            this.lb_y.TabIndex = 36;
            this.lb_y.Text = "Y";
            // 
            // nud_x
            // 
            this.nud_x.DecimalPlaces = 3;
            this.nud_x.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_x.Location = new System.Drawing.Point(3, 17);
            this.nud_x.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nud_x.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.nud_x.Name = "nud_x";
            this.nud_x.Size = new System.Drawing.Size(108, 32);
            this.nud_x.TabIndex = 35;
            this.nud_x.Value = new decimal(new int[] {
            123,
            0,
            0,
            0});
            this.nud_x.ValueChanged += new System.EventHandler(this.nud_x_ValueChanged);
            // 
            // lb_x
            // 
            this.lb_x.AutoSize = true;
            this.lb_x.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_x.Location = new System.Drawing.Point(3, 0);
            this.lb_x.Name = "lb_x";
            this.lb_x.Size = new System.Drawing.Size(15, 16);
            this.lb_x.TabIndex = 34;
            this.lb_x.Text = "X";
            // 
            // XYZ
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.nud_z);
            this.Controls.Add(this.lb_z);
            this.Controls.Add(this.nud_y);
            this.Controls.Add(this.lb_y);
            this.Controls.Add(this.nud_x);
            this.Controls.Add(this.lb_x);
            this.Name = "XYZ";
            this.Size = new System.Drawing.Size(343, 53);
            ((System.ComponentModel.ISupportInitialize)(this.nud_z)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_x)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        //private System.Windows.Forms.NumericUpDown nud_u;
        //private System.Windows.Forms.Label lb_u;
        private System.Windows.Forms.NumericUpDown nud_z;
        private System.Windows.Forms.Label lb_z;
        private System.Windows.Forms.NumericUpDown nud_y;
        private System.Windows.Forms.Label lb_y;
        private System.Windows.Forms.NumericUpDown nud_x;
        private System.Windows.Forms.Label lb_x;
    }
}
