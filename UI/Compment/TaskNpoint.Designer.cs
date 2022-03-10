namespace UI.Compment
{
    partial class TaskNpoint
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
            this.lbl_cam_pos = new System.Windows.Forms.Label();
            this.label74 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.label52 = new System.Windows.Forms.Label();
            this.btn_task_cam_movpos = new System.Windows.Forms.Button();
            this.btn_task_cam_getpos = new System.Windows.Forms.Button();
            this.nud_task_cam_z = new System.Windows.Forms.NumericUpDown();
            this.nud_task_cam_y = new System.Windows.Forms.NumericUpDown();
            this.nud_task_cam_x = new System.Windows.Forms.NumericUpDown();
            this.btn_task_cali_action = new System.Windows.Forms.Button();
            this.nud_task_step = new System.Windows.Forms.NumericUpDown();
            this.label56 = new System.Windows.Forms.Label();
            this.cb_upcamload = new System.Windows.Forms.ComboBox();
            this.tn_lbl_upcam = new System.Windows.Forms.Label();
            this.tn_lbl_task = new System.Windows.Forms.Label();
            this.cb_taskload = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.nud_task_cam_z)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_task_cam_y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_task_cam_x)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_task_step)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_cam_pos
            // 
            this.lbl_cam_pos.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_cam_pos.Location = new System.Drawing.Point(15, 65);
            this.lbl_cam_pos.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_cam_pos.Name = "lbl_cam_pos";
            this.lbl_cam_pos.Size = new System.Drawing.Size(113, 24);
            this.lbl_cam_pos.TabIndex = 371;
            this.lbl_cam_pos.Text = "拍照位:";
            this.lbl_cam_pos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label74.Location = new System.Drawing.Point(151, 22);
            this.label74.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(68, 24);
            this.label74.TabIndex = 416;
            this.label74.Text = "X(mm)";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label42.Location = new System.Drawing.Point(383, 22);
            this.label42.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(66, 24);
            this.label42.TabIndex = 415;
            this.label42.Text = "Z(mm)";
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label52.Location = new System.Drawing.Point(267, 22);
            this.label52.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(66, 24);
            this.label52.TabIndex = 414;
            this.label52.Text = "Y(mm)";
            // 
            // btn_task_cam_movpos
            // 
            this.btn_task_cam_movpos.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_task_cam_movpos.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_task_cam_movpos.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_task_cam_movpos.ForeColor = System.Drawing.Color.Black;
            this.btn_task_cam_movpos.Location = new System.Drawing.Point(584, 56);
            this.btn_task_cam_movpos.Name = "btn_task_cam_movpos";
            this.btn_task_cam_movpos.Size = new System.Drawing.Size(80, 42);
            this.btn_task_cam_movpos.TabIndex = 421;
            this.btn_task_cam_movpos.Text = "定位";
            this.btn_task_cam_movpos.UseVisualStyleBackColor = false;
            this.btn_task_cam_movpos.Click += new System.EventHandler(this.btn_task_cam_movpos_Click);
            // 
            // btn_task_cam_getpos
            // 
            this.btn_task_cam_getpos.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_task_cam_getpos.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_task_cam_getpos.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_task_cam_getpos.ForeColor = System.Drawing.Color.Black;
            this.btn_task_cam_getpos.Location = new System.Drawing.Point(491, 56);
            this.btn_task_cam_getpos.Name = "btn_task_cam_getpos";
            this.btn_task_cam_getpos.Size = new System.Drawing.Size(80, 42);
            this.btn_task_cam_getpos.TabIndex = 420;
            this.btn_task_cam_getpos.Text = "学习";
            this.btn_task_cam_getpos.UseVisualStyleBackColor = false;
            this.btn_task_cam_getpos.Click += new System.EventHandler(this.btn_task_cam_getpos_Click);
            // 
            // nud_task_cam_z
            // 
            this.nud_task_cam_z.DecimalPlaces = 2;
            this.nud_task_cam_z.Enabled = false;
            this.nud_task_cam_z.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_task_cam_z.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nud_task_cam_z.Location = new System.Drawing.Point(376, 59);
            this.nud_task_cam_z.Maximum = new decimal(new int[] {
            70,
            0,
            0,
            0});
            this.nud_task_cam_z.Minimum = new decimal(new int[] {
            70,
            0,
            0,
            -2147483648});
            this.nud_task_cam_z.Name = "nud_task_cam_z";
            this.nud_task_cam_z.Size = new System.Drawing.Size(106, 35);
            this.nud_task_cam_z.TabIndex = 419;
            // 
            // nud_task_cam_y
            // 
            this.nud_task_cam_y.DecimalPlaces = 2;
            this.nud_task_cam_y.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_task_cam_y.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nud_task_cam_y.Location = new System.Drawing.Point(255, 59);
            this.nud_task_cam_y.Maximum = new decimal(new int[] {
            1999,
            0,
            0,
            0});
            this.nud_task_cam_y.Minimum = new decimal(new int[] {
            1999,
            0,
            0,
            -2147483648});
            this.nud_task_cam_y.Name = "nud_task_cam_y";
            this.nud_task_cam_y.Size = new System.Drawing.Size(106, 35);
            this.nud_task_cam_y.TabIndex = 418;
            // 
            // nud_task_cam_x
            // 
            this.nud_task_cam_x.DecimalPlaces = 2;
            this.nud_task_cam_x.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_task_cam_x.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nud_task_cam_x.Location = new System.Drawing.Point(135, 59);
            this.nud_task_cam_x.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nud_task_cam_x.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.nud_task_cam_x.Name = "nud_task_cam_x";
            this.nud_task_cam_x.Size = new System.Drawing.Size(106, 35);
            this.nud_task_cam_x.TabIndex = 417;
            // 
            // btn_task_cali_action
            // 
            this.btn_task_cali_action.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.btn_task_cali_action.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_task_cali_action.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_task_cali_action.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_task_cali_action.ForeColor = System.Drawing.Color.White;
            this.btn_task_cali_action.Location = new System.Drawing.Point(684, 55);
            this.btn_task_cali_action.Name = "btn_task_cali_action";
            this.btn_task_cali_action.Size = new System.Drawing.Size(89, 44);
            this.btn_task_cali_action.TabIndex = 422;
            this.btn_task_cali_action.Text = "校准";
            this.btn_task_cali_action.UseVisualStyleBackColor = false;
            this.btn_task_cali_action.Click += new System.EventHandler(this.btn_task_cali_action_Click);
            // 
            // nud_task_step
            // 
            this.nud_task_step.DecimalPlaces = 2;
            this.nud_task_step.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_task_step.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nud_task_step.Location = new System.Drawing.Point(134, 123);
            this.nud_task_step.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nud_task_step.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.nud_task_step.Name = "nud_task_step";
            this.nud_task_step.Size = new System.Drawing.Size(107, 35);
            this.nud_task_step.TabIndex = 423;
            this.nud_task_step.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label56
            // 
            this.label56.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label56.Location = new System.Drawing.Point(24, 128);
            this.label56.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(104, 24);
            this.label56.TabIndex = 424;
            this.label56.Text = "步进(mm):";
            this.label56.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cb_upcamload
            // 
            this.cb_upcamload.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_upcamload.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cb_upcamload.FormattingEnabled = true;
            this.cb_upcamload.Location = new System.Drawing.Point(364, 127);
            this.cb_upcamload.Name = "cb_upcamload";
            this.cb_upcamload.Size = new System.Drawing.Size(137, 27);
            this.cb_upcamload.TabIndex = 430;
            this.cb_upcamload.SelectedIndexChanged += new System.EventHandler(this.cb_upcamload_SelectedIndexChanged);
            // 
            // tn_lbl_upcam
            // 
            this.tn_lbl_upcam.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tn_lbl_upcam.Location = new System.Drawing.Point(248, 128);
            this.tn_lbl_upcam.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tn_lbl_upcam.Name = "tn_lbl_upcam";
            this.tn_lbl_upcam.Size = new System.Drawing.Size(114, 24);
            this.tn_lbl_upcam.TabIndex = 429;
            this.tn_lbl_upcam.Text = "上相机选取:";
            // 
            // tn_lbl_task
            // 
            this.tn_lbl_task.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tn_lbl_task.Location = new System.Drawing.Point(516, 128);
            this.tn_lbl_task.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tn_lbl_task.Name = "tn_lbl_task";
            this.tn_lbl_task.Size = new System.Drawing.Size(97, 24);
            this.tn_lbl_task.TabIndex = 431;
            this.tn_lbl_task.Text = "任务选取:";
            // 
            // cb_taskload
            // 
            this.cb_taskload.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_taskload.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cb_taskload.FormattingEnabled = true;
            this.cb_taskload.Location = new System.Drawing.Point(614, 125);
            this.cb_taskload.Name = "cb_taskload";
            this.cb_taskload.Size = new System.Drawing.Size(159, 27);
            this.cb_taskload.TabIndex = 432;
            // 
            // TaskNpoint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(225)))), ((int)(((byte)(232)))));
            this.Controls.Add(this.cb_taskload);
            this.Controls.Add(this.tn_lbl_task);
            this.Controls.Add(this.cb_upcamload);
            this.Controls.Add(this.tn_lbl_upcam);
            this.Controls.Add(this.nud_task_step);
            this.Controls.Add(this.label56);
            this.Controls.Add(this.btn_task_cali_action);
            this.Controls.Add(this.btn_task_cam_movpos);
            this.Controls.Add(this.btn_task_cam_getpos);
            this.Controls.Add(this.nud_task_cam_z);
            this.Controls.Add(this.nud_task_cam_y);
            this.Controls.Add(this.nud_task_cam_x);
            this.Controls.Add(this.label74);
            this.Controls.Add(this.label42);
            this.Controls.Add(this.label52);
            this.Controls.Add(this.lbl_cam_pos);
            this.Font = new System.Drawing.Font("宋体", 9F);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "TaskNpoint";
            this.Size = new System.Drawing.Size(794, 289);
            ((System.ComponentModel.ISupportInitialize)(this.nud_task_cam_z)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_task_cam_y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_task_cam_x)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_task_step)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_cam_pos;
        private System.Windows.Forms.Label label74;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.Button btn_task_cam_movpos;
        private System.Windows.Forms.Button btn_task_cam_getpos;
        private System.Windows.Forms.NumericUpDown nud_task_cam_z;
        private System.Windows.Forms.NumericUpDown nud_task_cam_y;
        private System.Windows.Forms.NumericUpDown nud_task_cam_x;
        private System.Windows.Forms.Button btn_task_cali_action;
        private System.Windows.Forms.NumericUpDown nud_task_step;
        private System.Windows.Forms.Label label56;
        private System.Windows.Forms.Label tn_lbl_upcam;
        private System.Windows.Forms.Label tn_lbl_task;
        public System.Windows.Forms.ComboBox cb_upcamload;
        public System.Windows.Forms.ComboBox cb_taskload;
    }
}
