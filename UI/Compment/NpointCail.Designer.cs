namespace UI.Compment
{
    partial class NpointCail
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
            this.lbl_unit_place = new System.Windows.Forms.Label();
            this.nud_unit_place_x = new System.Windows.Forms.NumericUpDown();
            this.nud_unit_place_z = new System.Windows.Forms.NumericUpDown();
            this.nud_unit_place_y = new System.Windows.Forms.NumericUpDown();
            this.Btn_unit_place_getpos = new System.Windows.Forms.Button();
            this.label52 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.label74 = new System.Windows.Forms.Label();
            this.Btn_unit_place_movpos = new System.Windows.Forms.Button();
            this.lbl_unit_cam = new System.Windows.Forms.Label();
            this.btn_unit_cam_x = new System.Windows.Forms.NumericUpDown();
            this.btn_unit_cam_y = new System.Windows.Forms.NumericUpDown();
            this.btn_unit_cam_z = new System.Windows.Forms.NumericUpDown();
            this.btn_unit_cam_getpos = new System.Windows.Forms.Button();
            this.btn_unit_cam_movpos = new System.Windows.Forms.Button();
            this.nud_unit_step = new System.Windows.Forms.NumericUpDown();
            this.btn_unit_cali_action = new System.Windows.Forms.Button();
            this.btn_save_action = new System.Windows.Forms.Button();
            this.Btn_unit_place_action = new System.Windows.Forms.Button();
            this.npointres = new System.Windows.Forms.Label();
            this.tb_result = new System.Windows.Forms.TextBox();
            this.btn_zk_ctrl = new System.Windows.Forms.Button();
            this.npointlabel56 = new System.Windows.Forms.Label();
            this.btn_cam = new System.Windows.Forms.Button();
            this.Btn_Live = new System.Windows.Forms.Button();
            this.cb_UDload = new System.Windows.Forms.ComboBox();
            this.npointmk = new System.Windows.Forms.Label();
            this.npointlabel2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nud_unit_place_x)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_unit_place_z)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_unit_place_y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_unit_cam_x)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_unit_cam_y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_unit_cam_z)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_unit_step)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_unit_place
            // 
            this.lbl_unit_place.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_unit_place.Location = new System.Drawing.Point(-4, 39);
            this.lbl_unit_place.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_unit_place.Name = "lbl_unit_place";
            this.lbl_unit_place.Size = new System.Drawing.Size(152, 24);
            this.lbl_unit_place.TabIndex = 249;
            this.lbl_unit_place.Text = "吸头1放料位:";
            this.lbl_unit_place.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nud_unit_place_x
            // 
            this.nud_unit_place_x.DecimalPlaces = 2;
            this.nud_unit_place_x.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_unit_place_x.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nud_unit_place_x.Location = new System.Drawing.Point(146, 33);
            this.nud_unit_place_x.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nud_unit_place_x.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.nud_unit_place_x.Name = "nud_unit_place_x";
            this.nud_unit_place_x.Size = new System.Drawing.Size(106, 35);
            this.nud_unit_place_x.TabIndex = 250;
            // 
            // nud_unit_place_z
            // 
            this.nud_unit_place_z.DecimalPlaces = 2;
            this.nud_unit_place_z.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_unit_place_z.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nud_unit_place_z.Location = new System.Drawing.Point(378, 33);
            this.nud_unit_place_z.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nud_unit_place_z.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            -2147483648});
            this.nud_unit_place_z.Name = "nud_unit_place_z";
            this.nud_unit_place_z.Size = new System.Drawing.Size(106, 35);
            this.nud_unit_place_z.TabIndex = 251;
            // 
            // nud_unit_place_y
            // 
            this.nud_unit_place_y.DecimalPlaces = 2;
            this.nud_unit_place_y.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_unit_place_y.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nud_unit_place_y.Location = new System.Drawing.Point(262, 33);
            this.nud_unit_place_y.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nud_unit_place_y.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.nud_unit_place_y.Name = "nud_unit_place_y";
            this.nud_unit_place_y.Size = new System.Drawing.Size(106, 35);
            this.nud_unit_place_y.TabIndex = 252;
            // 
            // Btn_unit_place_getpos
            // 
            this.Btn_unit_place_getpos.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Btn_unit_place_getpos.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.Btn_unit_place_getpos.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Btn_unit_place_getpos.ForeColor = System.Drawing.Color.Black;
            this.Btn_unit_place_getpos.Location = new System.Drawing.Point(492, 26);
            this.Btn_unit_place_getpos.Name = "Btn_unit_place_getpos";
            this.Btn_unit_place_getpos.Size = new System.Drawing.Size(80, 42);
            this.Btn_unit_place_getpos.TabIndex = 365;
            this.Btn_unit_place_getpos.Text = "学习";
            this.Btn_unit_place_getpos.UseVisualStyleBackColor = false;
            this.Btn_unit_place_getpos.Click += new System.EventHandler(this.Btn_unit_getpos_Click);
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label52.Location = new System.Drawing.Point(274, 1);
            this.label52.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(66, 24);
            this.label52.TabIndex = 366;
            this.label52.Text = "Y(mm)";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label42.Location = new System.Drawing.Point(390, 1);
            this.label42.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(66, 24);
            this.label42.TabIndex = 367;
            this.label42.Text = "Z(mm)";
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label74.Location = new System.Drawing.Point(158, 1);
            this.label74.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(68, 24);
            this.label74.TabIndex = 368;
            this.label74.Text = "X(mm)";
            // 
            // Btn_unit_place_movpos
            // 
            this.Btn_unit_place_movpos.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Btn_unit_place_movpos.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.Btn_unit_place_movpos.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Btn_unit_place_movpos.ForeColor = System.Drawing.Color.Black;
            this.Btn_unit_place_movpos.Location = new System.Drawing.Point(586, 26);
            this.Btn_unit_place_movpos.Name = "Btn_unit_place_movpos";
            this.Btn_unit_place_movpos.Size = new System.Drawing.Size(80, 42);
            this.Btn_unit_place_movpos.TabIndex = 369;
            this.Btn_unit_place_movpos.Text = "定位";
            this.Btn_unit_place_movpos.UseVisualStyleBackColor = false;
            this.Btn_unit_place_movpos.Click += new System.EventHandler(this.Btn_unit_movpos_Click);
            // 
            // lbl_unit_cam
            // 
            this.lbl_unit_cam.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_unit_cam.Location = new System.Drawing.Point(1, 89);
            this.lbl_unit_cam.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_unit_cam.Name = "lbl_unit_cam";
            this.lbl_unit_cam.Size = new System.Drawing.Size(148, 24);
            this.lbl_unit_cam.TabIndex = 370;
            this.lbl_unit_cam.Text = "上相机1拍照位:";
            this.lbl_unit_cam.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btn_unit_cam_x
            // 
            this.btn_unit_cam_x.DecimalPlaces = 2;
            this.btn_unit_cam_x.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_unit_cam_x.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.btn_unit_cam_x.Location = new System.Drawing.Point(146, 85);
            this.btn_unit_cam_x.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.btn_unit_cam_x.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.btn_unit_cam_x.Name = "btn_unit_cam_x";
            this.btn_unit_cam_x.Size = new System.Drawing.Size(106, 35);
            this.btn_unit_cam_x.TabIndex = 371;
            // 
            // btn_unit_cam_y
            // 
            this.btn_unit_cam_y.DecimalPlaces = 2;
            this.btn_unit_cam_y.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_unit_cam_y.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.btn_unit_cam_y.Location = new System.Drawing.Point(262, 85);
            this.btn_unit_cam_y.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.btn_unit_cam_y.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.btn_unit_cam_y.Name = "btn_unit_cam_y";
            this.btn_unit_cam_y.Size = new System.Drawing.Size(106, 35);
            this.btn_unit_cam_y.TabIndex = 372;
            // 
            // btn_unit_cam_z
            // 
            this.btn_unit_cam_z.DecimalPlaces = 2;
            this.btn_unit_cam_z.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_unit_cam_z.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.btn_unit_cam_z.Location = new System.Drawing.Point(378, 85);
            this.btn_unit_cam_z.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.btn_unit_cam_z.Name = "btn_unit_cam_z";
            this.btn_unit_cam_z.Size = new System.Drawing.Size(106, 35);
            this.btn_unit_cam_z.TabIndex = 373;
            // 
            // btn_unit_cam_getpos
            // 
            this.btn_unit_cam_getpos.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_unit_cam_getpos.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_unit_cam_getpos.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_unit_cam_getpos.ForeColor = System.Drawing.Color.Black;
            this.btn_unit_cam_getpos.Location = new System.Drawing.Point(493, 82);
            this.btn_unit_cam_getpos.Name = "btn_unit_cam_getpos";
            this.btn_unit_cam_getpos.Size = new System.Drawing.Size(80, 42);
            this.btn_unit_cam_getpos.TabIndex = 374;
            this.btn_unit_cam_getpos.Text = "学习";
            this.btn_unit_cam_getpos.UseVisualStyleBackColor = false;
            this.btn_unit_cam_getpos.Click += new System.EventHandler(this.Btn_unit_getpos_Click);
            // 
            // btn_unit_cam_movpos
            // 
            this.btn_unit_cam_movpos.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_unit_cam_movpos.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_unit_cam_movpos.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_unit_cam_movpos.ForeColor = System.Drawing.Color.Black;
            this.btn_unit_cam_movpos.Location = new System.Drawing.Point(586, 82);
            this.btn_unit_cam_movpos.Name = "btn_unit_cam_movpos";
            this.btn_unit_cam_movpos.Size = new System.Drawing.Size(80, 42);
            this.btn_unit_cam_movpos.TabIndex = 375;
            this.btn_unit_cam_movpos.Text = "定位";
            this.btn_unit_cam_movpos.UseVisualStyleBackColor = false;
            this.btn_unit_cam_movpos.Click += new System.EventHandler(this.Btn_unit_movpos_Click);
            // 
            // nud_unit_step
            // 
            this.nud_unit_step.DecimalPlaces = 2;
            this.nud_unit_step.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_unit_step.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nud_unit_step.Location = new System.Drawing.Point(145, 139);
            this.nud_unit_step.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nud_unit_step.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.nud_unit_step.Name = "nud_unit_step";
            this.nud_unit_step.Size = new System.Drawing.Size(106, 35);
            this.nud_unit_step.TabIndex = 376;
            // 
            // btn_unit_cali_action
            // 
            this.btn_unit_cali_action.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.btn_unit_cali_action.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_unit_cali_action.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_unit_cali_action.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_unit_cali_action.ForeColor = System.Drawing.Color.White;
            this.btn_unit_cali_action.Location = new System.Drawing.Point(683, 88);
            this.btn_unit_cali_action.Name = "btn_unit_cali_action";
            this.btn_unit_cali_action.Size = new System.Drawing.Size(89, 44);
            this.btn_unit_cali_action.TabIndex = 378;
            this.btn_unit_cali_action.Text = "2.校准";
            this.btn_unit_cali_action.UseVisualStyleBackColor = false;
            this.btn_unit_cali_action.Click += new System.EventHandler(this.btn_unit_cali_action_Click);
            // 
            // btn_save_action
            // 
            this.btn_save_action.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.btn_save_action.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_save_action.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_save_action.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_save_action.ForeColor = System.Drawing.Color.White;
            this.btn_save_action.Location = new System.Drawing.Point(683, 151);
            this.btn_save_action.Name = "btn_save_action";
            this.btn_save_action.Size = new System.Drawing.Size(89, 44);
            this.btn_save_action.TabIndex = 379;
            this.btn_save_action.Text = "3.保存";
            this.btn_save_action.UseVisualStyleBackColor = false;
            this.btn_save_action.Click += new System.EventHandler(this.btn_save_action_Click);
            // 
            // Btn_unit_place_action
            // 
            this.Btn_unit_place_action.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.Btn_unit_place_action.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.Btn_unit_place_action.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Btn_unit_place_action.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Btn_unit_place_action.ForeColor = System.Drawing.Color.White;
            this.Btn_unit_place_action.Location = new System.Drawing.Point(683, 25);
            this.Btn_unit_place_action.Name = "Btn_unit_place_action";
            this.Btn_unit_place_action.Size = new System.Drawing.Size(89, 44);
            this.Btn_unit_place_action.TabIndex = 380;
            this.Btn_unit_place_action.Text = "1.放料";
            this.Btn_unit_place_action.UseVisualStyleBackColor = false;
            this.Btn_unit_place_action.Click += new System.EventHandler(this.Btn_unit_place_action_Click);
            // 
            // npointres
            // 
            this.npointres.AutoSize = true;
            this.npointres.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.npointres.ForeColor = System.Drawing.Color.Black;
            this.npointres.Location = new System.Drawing.Point(60, 253);
            this.npointres.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.npointres.Name = "npointres";
            this.npointres.Size = new System.Drawing.Size(91, 24);
            this.npointres.TabIndex = 382;
            this.npointres.Text = "校准结果:";
            // 
            // tb_result
            // 
            this.tb_result.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(225)))), ((int)(((byte)(232)))));
            this.tb_result.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_result.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_result.ForeColor = System.Drawing.Color.Black;
            this.tb_result.Location = new System.Drawing.Point(147, 253);
            this.tb_result.Name = "tb_result";
            this.tb_result.ReadOnly = true;
            this.tb_result.Size = new System.Drawing.Size(454, 26);
            this.tb_result.TabIndex = 381;
            this.tb_result.Text = "Xs=-0.0000 Ys=-0.0000 A=-000.000 RMSE=0.0";
            // 
            // btn_zk_ctrl
            // 
            this.btn_zk_ctrl.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_zk_ctrl.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_zk_ctrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_zk_ctrl.ForeColor = System.Drawing.Color.Black;
            this.btn_zk_ctrl.Location = new System.Drawing.Point(443, 193);
            this.btn_zk_ctrl.Name = "btn_zk_ctrl";
            this.btn_zk_ctrl.Size = new System.Drawing.Size(116, 44);
            this.btn_zk_ctrl.TabIndex = 384;
            this.btn_zk_ctrl.Text = "真空";
            this.btn_zk_ctrl.UseVisualStyleBackColor = false;
            this.btn_zk_ctrl.Click += new System.EventHandler(this.btn_zk_ctrl_Click);
            // 
            // npointlabel56
            // 
            this.npointlabel56.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.npointlabel56.Location = new System.Drawing.Point(-3, 145);
            this.npointlabel56.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.npointlabel56.Name = "npointlabel56";
            this.npointlabel56.Size = new System.Drawing.Size(153, 24);
            this.npointlabel56.TabIndex = 377;
            this.npointlabel56.Text = "步进(mm):";
            this.npointlabel56.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btn_cam
            // 
            this.btn_cam.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_cam.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_cam.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_cam.ForeColor = System.Drawing.Color.Black;
            this.btn_cam.Location = new System.Drawing.Point(293, 193);
            this.btn_cam.Name = "btn_cam";
            this.btn_cam.Size = new System.Drawing.Size(116, 44);
            this.btn_cam.TabIndex = 385;
            this.btn_cam.Text = "拍照";
            this.btn_cam.UseVisualStyleBackColor = false;
            this.btn_cam.Click += new System.EventHandler(this.btn_cam_Click);
            // 
            // Btn_Live
            // 
            this.Btn_Live.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Btn_Live.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.Btn_Live.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Btn_Live.ForeColor = System.Drawing.Color.Black;
            this.Btn_Live.Location = new System.Drawing.Point(147, 192);
            this.Btn_Live.Name = "Btn_Live";
            this.Btn_Live.Size = new System.Drawing.Size(116, 44);
            this.Btn_Live.TabIndex = 383;
            this.Btn_Live.Text = "实时";
            this.Btn_Live.UseVisualStyleBackColor = false;
            // 
            // cb_UDload
            // 
            this.cb_UDload.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_UDload.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cb_UDload.FormattingEnabled = true;
            this.cb_UDload.Location = new System.Drawing.Point(358, 142);
            this.cb_UDload.Name = "cb_UDload";
            this.cb_UDload.Size = new System.Drawing.Size(126, 27);
            this.cb_UDload.TabIndex = 428;
            this.cb_UDload.SelectedIndexChanged += new System.EventHandler(this.cb_cam_SelectedIndexChanged);
            // 
            // npointmk
            // 
            this.npointmk.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.npointmk.Location = new System.Drawing.Point(261, 144);
            this.npointmk.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.npointmk.Name = "npointmk";
            this.npointmk.Size = new System.Drawing.Size(97, 24);
            this.npointmk.TabIndex = 427;
            this.npointmk.Text = "模块选取:";
            // 
            // npointlabel2
            // 
            this.npointlabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.npointlabel2.ForeColor = System.Drawing.Color.Red;
            this.npointlabel2.Location = new System.Drawing.Point(503, 127);
            this.npointlabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.npointlabel2.Name = "npointlabel2";
            this.npointlabel2.Size = new System.Drawing.Size(174, 64);
            this.npointlabel2.TabIndex = 427;
            this.npointlabel2.Text = "注意：请确保所有相机的九点校正是在校正片位置不变的情况下进行";
            // 
            // NpointCail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(225)))), ((int)(((byte)(232)))));
            this.Controls.Add(this.cb_UDload);
            this.Controls.Add(this.npointlabel2);
            this.Controls.Add(this.npointmk);
            this.Controls.Add(this.btn_cam);
            this.Controls.Add(this.btn_zk_ctrl);
            this.Controls.Add(this.Btn_Live);
            this.Controls.Add(this.tb_result);
            this.Controls.Add(this.npointres);
            this.Controls.Add(this.Btn_unit_place_action);
            this.Controls.Add(this.btn_save_action);
            this.Controls.Add(this.btn_unit_cali_action);
            this.Controls.Add(this.nud_unit_step);
            this.Controls.Add(this.npointlabel56);
            this.Controls.Add(this.btn_unit_cam_movpos);
            this.Controls.Add(this.btn_unit_cam_getpos);
            this.Controls.Add(this.btn_unit_cam_z);
            this.Controls.Add(this.btn_unit_cam_y);
            this.Controls.Add(this.btn_unit_cam_x);
            this.Controls.Add(this.lbl_unit_cam);
            this.Controls.Add(this.Btn_unit_place_movpos);
            this.Controls.Add(this.label74);
            this.Controls.Add(this.label42);
            this.Controls.Add(this.label52);
            this.Controls.Add(this.Btn_unit_place_getpos);
            this.Controls.Add(this.nud_unit_place_y);
            this.Controls.Add(this.nud_unit_place_z);
            this.Controls.Add(this.nud_unit_place_x);
            this.Controls.Add(this.lbl_unit_place);
            this.Name = "NpointCail";
            this.Size = new System.Drawing.Size(794, 289);
            ((System.ComponentModel.ISupportInitialize)(this.nud_unit_place_x)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_unit_place_z)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_unit_place_y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_unit_cam_x)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_unit_cam_y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_unit_cam_z)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_unit_step)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_unit_place;
        private System.Windows.Forms.NumericUpDown nud_unit_place_x;
        private System.Windows.Forms.NumericUpDown nud_unit_place_z;
        private System.Windows.Forms.NumericUpDown nud_unit_place_y;
        private System.Windows.Forms.Button Btn_unit_place_getpos;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label74;
        private System.Windows.Forms.Button Btn_unit_place_movpos;
        private System.Windows.Forms.NumericUpDown btn_unit_cam_x;
        private System.Windows.Forms.NumericUpDown btn_unit_cam_y;
        private System.Windows.Forms.NumericUpDown btn_unit_cam_z;
        private System.Windows.Forms.Button btn_unit_cam_getpos;
        private System.Windows.Forms.Button btn_unit_cam_movpos;
        private System.Windows.Forms.NumericUpDown nud_unit_step;
        private System.Windows.Forms.Button btn_unit_cali_action;
        private System.Windows.Forms.Button btn_save_action;
        private System.Windows.Forms.Button Btn_unit_place_action;
        private System.Windows.Forms.Label npointres;
        protected System.Windows.Forms.TextBox tb_result;
        private System.Windows.Forms.Button btn_zk_ctrl;
        private System.Windows.Forms.Label npointlabel56;
        private System.Windows.Forms.Button btn_cam;
        private System.Windows.Forms.Button Btn_Live;
        private System.Windows.Forms.Label npointmk;
        private System.Windows.Forms.Label npointlabel2;
        public System.Windows.Forms.ComboBox cb_UDload;
        public System.Windows.Forms.Label lbl_unit_cam;
    }
}
