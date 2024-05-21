namespace UI.Compment
{
    partial class RotAndFly
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
            this.btn_rot_downcam_movpos = new System.Windows.Forms.Button();
            this.label74 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.label52 = new System.Windows.Forms.Label();
            this.btn_rot_downcam_getpos = new System.Windows.Forms.Button();
            this.nud_rot_downcam_y = new System.Windows.Forms.NumericUpDown();
            this.nud_rot_downcam_z = new System.Windows.Forms.NumericUpDown();
            this.nud_rot_downcam_x = new System.Windows.Forms.NumericUpDown();
            this.lbl_rot_downcam = new System.Windows.Forms.Label();
            this.txb_rot_y = new System.Windows.Forms.TextBox();
            this.txb_rot_x = new System.Windows.Forms.TextBox();
            this.txb_rot_rmse = new System.Windows.Forms.TextBox();
            this.lbl_rotrmse = new System.Windows.Forms.Label();
            this.lbl_rotx = new System.Windows.Forms.Label();
            this.lbl_roty = new System.Windows.Forms.Label();
            this.lbl_xt = new System.Windows.Forms.Label();
            this.cb_xt = new System.Windows.Forms.ComboBox();
            this.btn_rot_action = new System.Windows.Forms.Button();
            this.lbl_flycam_test = new System.Windows.Forms.Label();
            this.lbl_xt1_cap = new System.Windows.Forms.Label();
            this.tb_ws_cap_before = new System.Windows.Forms.TextBox();
            this.tb_xt2_cap = new System.Windows.Forms.TextBox();
            this.tb_xt1_cap = new System.Windows.Forms.TextBox();
            this.btn_fly_action = new System.Windows.Forms.Button();
            this.btn_cam = new System.Windows.Forms.Button();
            this.btn_zk_ctrl = new System.Windows.Forms.Button();
            this.Btn_Live = new System.Windows.Forms.Button();
            this.lbl_rot_res = new System.Windows.Forms.Label();
            this.tb_ws_cap_after = new System.Windows.Forms.TextBox();
            this.lbl_xt2_cap = new System.Windows.Forms.Label();
            this.lbl_frontws = new System.Windows.Forms.Label();
            this.lbl_behindws = new System.Windows.Forms.Label();
            this.lbl_flycam_offset = new System.Windows.Forms.Label();
            this.tb_xt1_ofs = new System.Windows.Forms.TextBox();
            this.tb_xt2_ofs = new System.Windows.Forms.TextBox();
            this.tb_ws_ofs_before = new System.Windows.Forms.TextBox();
            this.tb_ws_ofs_after = new System.Windows.Forms.TextBox();
            this.lbl_mk = new System.Windows.Forms.Label();
            this.cb_UDload = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.nud_rot_downcam_y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_rot_downcam_z)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_rot_downcam_x)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_rot_downcam_movpos
            // 
            this.btn_rot_downcam_movpos.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_rot_downcam_movpos.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_rot_downcam_movpos.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_rot_downcam_movpos.ForeColor = System.Drawing.Color.Black;
            this.btn_rot_downcam_movpos.Location = new System.Drawing.Point(586, 29);
            this.btn_rot_downcam_movpos.Name = "btn_rot_downcam_movpos";
            this.btn_rot_downcam_movpos.Size = new System.Drawing.Size(80, 42);
            this.btn_rot_downcam_movpos.TabIndex = 414;
            this.btn_rot_downcam_movpos.Text = "定位";
            this.btn_rot_downcam_movpos.UseVisualStyleBackColor = false;
            this.btn_rot_downcam_movpos.Click += new System.EventHandler(this.btn_rot_downcam_movpos_Click);
            // 
            // label74
            // 
            this.label74.AutoSize = true;
            this.label74.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label74.Location = new System.Drawing.Point(158, 10);
            this.label74.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label74.Name = "label74";
            this.label74.Size = new System.Drawing.Size(68, 24);
            this.label74.TabIndex = 413;
            this.label74.Text = "X(mm)";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label42.Location = new System.Drawing.Point(390, 10);
            this.label42.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(66, 24);
            this.label42.TabIndex = 412;
            this.label42.Text = "Z(mm)";
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label52.Location = new System.Drawing.Point(274, 10);
            this.label52.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(66, 24);
            this.label52.TabIndex = 411;
            this.label52.Text = "Y(mm)";
            // 
            // btn_rot_downcam_getpos
            // 
            this.btn_rot_downcam_getpos.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_rot_downcam_getpos.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_rot_downcam_getpos.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_rot_downcam_getpos.ForeColor = System.Drawing.Color.Black;
            this.btn_rot_downcam_getpos.Location = new System.Drawing.Point(492, 29);
            this.btn_rot_downcam_getpos.Name = "btn_rot_downcam_getpos";
            this.btn_rot_downcam_getpos.Size = new System.Drawing.Size(80, 42);
            this.btn_rot_downcam_getpos.TabIndex = 410;
            this.btn_rot_downcam_getpos.Text = "学习";
            this.btn_rot_downcam_getpos.UseVisualStyleBackColor = false;
            this.btn_rot_downcam_getpos.Click += new System.EventHandler(this.btn_rot_downcam_getpos_Click);
            // 
            // nud_rot_downcam_y
            // 
            this.nud_rot_downcam_y.DecimalPlaces = 2;
            this.nud_rot_downcam_y.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_rot_downcam_y.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nud_rot_downcam_y.Location = new System.Drawing.Point(262, 36);
            this.nud_rot_downcam_y.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nud_rot_downcam_y.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.nud_rot_downcam_y.Name = "nud_rot_downcam_y";
            this.nud_rot_downcam_y.Size = new System.Drawing.Size(106, 35);
            this.nud_rot_downcam_y.TabIndex = 409;
            // 
            // nud_rot_downcam_z
            // 
            this.nud_rot_downcam_z.DecimalPlaces = 2;
            this.nud_rot_downcam_z.Enabled = false;
            this.nud_rot_downcam_z.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_rot_downcam_z.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nud_rot_downcam_z.Location = new System.Drawing.Point(378, 36);
            this.nud_rot_downcam_z.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nud_rot_downcam_z.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            -2147483648});
            this.nud_rot_downcam_z.Name = "nud_rot_downcam_z";
            this.nud_rot_downcam_z.Size = new System.Drawing.Size(106, 35);
            this.nud_rot_downcam_z.TabIndex = 408;
            // 
            // nud_rot_downcam_x
            // 
            this.nud_rot_downcam_x.DecimalPlaces = 2;
            this.nud_rot_downcam_x.Enabled = false;
            this.nud_rot_downcam_x.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nud_rot_downcam_x.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nud_rot_downcam_x.Location = new System.Drawing.Point(146, 36);
            this.nud_rot_downcam_x.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nud_rot_downcam_x.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
            this.nud_rot_downcam_x.Name = "nud_rot_downcam_x";
            this.nud_rot_downcam_x.Size = new System.Drawing.Size(106, 35);
            this.nud_rot_downcam_x.TabIndex = 407;
            // 
            // lbl_rot_downcam
            // 
            this.lbl_rot_downcam.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_rot_downcam.Location = new System.Drawing.Point(0, 38);
            this.lbl_rot_downcam.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_rot_downcam.Name = "lbl_rot_downcam";
            this.lbl_rot_downcam.Size = new System.Drawing.Size(147, 24);
            this.lbl_rot_downcam.TabIndex = 406;
            this.lbl_rot_downcam.Text = "下相机拍照位:";
            this.lbl_rot_downcam.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txb_rot_y
            // 
            this.txb_rot_y.BackColor = System.Drawing.Color.White;
            this.txb_rot_y.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txb_rot_y.Enabled = false;
            this.txb_rot_y.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txb_rot_y.ForeColor = System.Drawing.Color.Black;
            this.txb_rot_y.Location = new System.Drawing.Point(263, 94);
            this.txb_rot_y.Name = "txb_rot_y";
            this.txb_rot_y.Size = new System.Drawing.Size(92, 26);
            this.txb_rot_y.TabIndex = 418;
            this.txb_rot_y.Text = "-00.000";
            // 
            // txb_rot_x
            // 
            this.txb_rot_x.BackColor = System.Drawing.Color.White;
            this.txb_rot_x.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txb_rot_x.Enabled = false;
            this.txb_rot_x.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txb_rot_x.ForeColor = System.Drawing.Color.Black;
            this.txb_rot_x.Location = new System.Drawing.Point(148, 94);
            this.txb_rot_x.Name = "txb_rot_x";
            this.txb_rot_x.Size = new System.Drawing.Size(89, 26);
            this.txb_rot_x.TabIndex = 416;
            this.txb_rot_x.Text = "-00.000";
            // 
            // txb_rot_rmse
            // 
            this.txb_rot_rmse.BackColor = System.Drawing.Color.White;
            this.txb_rot_rmse.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txb_rot_rmse.Enabled = false;
            this.txb_rot_rmse.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txb_rot_rmse.ForeColor = System.Drawing.Color.Black;
            this.txb_rot_rmse.Location = new System.Drawing.Point(379, 94);
            this.txb_rot_rmse.Name = "txb_rot_rmse";
            this.txb_rot_rmse.Size = new System.Drawing.Size(92, 26);
            this.txb_rot_rmse.TabIndex = 421;
            this.txb_rot_rmse.Text = "-00.000";
            // 
            // lbl_rotrmse
            // 
            this.lbl_rotrmse.AutoSize = true;
            this.lbl_rotrmse.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_rotrmse.Location = new System.Drawing.Point(395, 73);
            this.lbl_rotrmse.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_rotrmse.Name = "lbl_rotrmse";
            this.lbl_rotrmse.Size = new System.Drawing.Size(44, 20);
            this.lbl_rotrmse.TabIndex = 422;
            this.lbl_rotrmse.Text = "rmse";
            // 
            // lbl_rotx
            // 
            this.lbl_rotx.AutoSize = true;
            this.lbl_rotx.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_rotx.Location = new System.Drawing.Point(173, 73);
            this.lbl_rotx.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_rotx.Name = "lbl_rotx";
            this.lbl_rotx.Size = new System.Drawing.Size(16, 20);
            this.lbl_rotx.TabIndex = 423;
            this.lbl_rotx.Text = "x";
            // 
            // lbl_roty
            // 
            this.lbl_roty.AutoSize = true;
            this.lbl_roty.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_roty.Location = new System.Drawing.Point(291, 72);
            this.lbl_roty.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_roty.Name = "lbl_roty";
            this.lbl_roty.Size = new System.Drawing.Size(16, 20);
            this.lbl_roty.TabIndex = 424;
            this.lbl_roty.Text = "y";
            // 
            // lbl_xt
            // 
            this.lbl_xt.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_xt.Location = new System.Drawing.Point(615, 86);
            this.lbl_xt.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_xt.Name = "lbl_xt";
            this.lbl_xt.Size = new System.Drawing.Size(171, 24);
            this.lbl_xt.TabIndex = 425;
            this.lbl_xt.Text = "旋转中心吸头选取:";
            // 
            // cb_xt
            // 
            this.cb_xt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_xt.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cb_xt.FormattingEnabled = true;
            this.cb_xt.Location = new System.Drawing.Point(625, 117);
            this.cb_xt.Name = "cb_xt";
            this.cb_xt.Size = new System.Drawing.Size(151, 27);
            this.cb_xt.TabIndex = 426;
            this.cb_xt.SelectedIndexChanged += new System.EventHandler(this.cb_xt_SelectedIndexChanged);
            // 
            // btn_rot_action
            // 
            this.btn_rot_action.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.btn_rot_action.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_rot_action.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_rot_action.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_rot_action.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btn_rot_action.Location = new System.Drawing.Point(677, 32);
            this.btn_rot_action.Name = "btn_rot_action";
            this.btn_rot_action.Size = new System.Drawing.Size(106, 44);
            this.btn_rot_action.TabIndex = 427;
            this.btn_rot_action.Text = "旋转中心";
            this.btn_rot_action.UseVisualStyleBackColor = false;
            this.btn_rot_action.Click += new System.EventHandler(this.btn_rot_action_Click);
            // 
            // lbl_flycam_test
            // 
            this.lbl_flycam_test.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_flycam_test.Location = new System.Drawing.Point(42, 145);
            this.lbl_flycam_test.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_flycam_test.Name = "lbl_flycam_test";
            this.lbl_flycam_test.Size = new System.Drawing.Size(105, 24);
            this.lbl_flycam_test.TabIndex = 428;
            this.lbl_flycam_test.Text = "飞拍测试:";
            this.lbl_flycam_test.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_xt1_cap
            // 
            this.lbl_xt1_cap.AutoSize = true;
            this.lbl_xt1_cap.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_xt1_cap.Location = new System.Drawing.Point(151, 126);
            this.lbl_xt1_cap.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_xt1_cap.Name = "lbl_xt1_cap";
            this.lbl_xt1_cap.Size = new System.Drawing.Size(86, 15);
            this.lbl_xt1_cap.TabIndex = 433;
            this.lbl_xt1_cap.Text = "吸头1位置偏差";
            // 
            // tb_ws_cap_before
            // 
            this.tb_ws_cap_before.BackColor = System.Drawing.Color.White;
            this.tb_ws_cap_before.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_ws_cap_before.Enabled = false;
            this.tb_ws_cap_before.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_ws_cap_before.ForeColor = System.Drawing.Color.Black;
            this.tb_ws_cap_before.Location = new System.Drawing.Point(378, 146);
            this.tb_ws_cap_before.Name = "tb_ws_cap_before";
            this.tb_ws_cap_before.Size = new System.Drawing.Size(92, 26);
            this.tb_ws_cap_before.TabIndex = 431;
            this.tb_ws_cap_before.Text = "-00.000";
            // 
            // tb_xt2_cap
            // 
            this.tb_xt2_cap.BackColor = System.Drawing.Color.White;
            this.tb_xt2_cap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_xt2_cap.Enabled = false;
            this.tb_xt2_cap.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_xt2_cap.ForeColor = System.Drawing.Color.Black;
            this.tb_xt2_cap.Location = new System.Drawing.Point(262, 146);
            this.tb_xt2_cap.Name = "tb_xt2_cap";
            this.tb_xt2_cap.Size = new System.Drawing.Size(92, 26);
            this.tb_xt2_cap.TabIndex = 430;
            this.tb_xt2_cap.Text = "-00.000";
            // 
            // tb_xt1_cap
            // 
            this.tb_xt1_cap.BackColor = System.Drawing.Color.White;
            this.tb_xt1_cap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_xt1_cap.Enabled = false;
            this.tb_xt1_cap.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_xt1_cap.ForeColor = System.Drawing.Color.Black;
            this.tb_xt1_cap.Location = new System.Drawing.Point(148, 146);
            this.tb_xt1_cap.Name = "tb_xt1_cap";
            this.tb_xt1_cap.Size = new System.Drawing.Size(89, 26);
            this.tb_xt1_cap.TabIndex = 429;
            this.tb_xt1_cap.Text = "-00.000";
            // 
            // btn_fly_action
            // 
            this.btn_fly_action.BackColor = System.Drawing.Color.DarkGoldenrod;
            this.btn_fly_action.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_fly_action.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_fly_action.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_fly_action.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btn_fly_action.Location = new System.Drawing.Point(677, 156);
            this.btn_fly_action.Name = "btn_fly_action";
            this.btn_fly_action.Size = new System.Drawing.Size(106, 44);
            this.btn_fly_action.TabIndex = 435;
            this.btn_fly_action.Text = "飞拍";
            this.btn_fly_action.UseVisualStyleBackColor = false;
            this.btn_fly_action.Click += new System.EventHandler(this.btn_fly_action_Click);
            // 
            // btn_cam
            // 
            this.btn_cam.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_cam.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_cam.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_cam.ForeColor = System.Drawing.Color.Black;
            this.btn_cam.Location = new System.Drawing.Point(289, 237);
            this.btn_cam.Name = "btn_cam";
            this.btn_cam.Size = new System.Drawing.Size(116, 44);
            this.btn_cam.TabIndex = 438;
            this.btn_cam.Text = "拍照";
            this.btn_cam.UseVisualStyleBackColor = false;
            this.btn_cam.Click += new System.EventHandler(this.btn_cam_Click);
            // 
            // btn_zk_ctrl
            // 
            this.btn_zk_ctrl.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_zk_ctrl.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_zk_ctrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_zk_ctrl.ForeColor = System.Drawing.Color.Black;
            this.btn_zk_ctrl.Location = new System.Drawing.Point(439, 237);
            this.btn_zk_ctrl.Name = "btn_zk_ctrl";
            this.btn_zk_ctrl.Size = new System.Drawing.Size(116, 44);
            this.btn_zk_ctrl.TabIndex = 437;
            this.btn_zk_ctrl.Text = "真空";
            this.btn_zk_ctrl.UseVisualStyleBackColor = false;
            this.btn_zk_ctrl.Click += new System.EventHandler(this.btn_zk_ctrl_Click);
            // 
            // Btn_Live
            // 
            this.Btn_Live.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Btn_Live.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
            this.Btn_Live.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Btn_Live.ForeColor = System.Drawing.Color.Black;
            this.Btn_Live.Location = new System.Drawing.Point(143, 236);
            this.Btn_Live.Name = "Btn_Live";
            this.Btn_Live.Size = new System.Drawing.Size(116, 44);
            this.Btn_Live.TabIndex = 436;
            this.Btn_Live.Text = "实时";
            this.Btn_Live.UseVisualStyleBackColor = false;
            // 
            // lbl_rot_res
            // 
            this.lbl_rot_res.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_rot_res.Location = new System.Drawing.Point(0, 94);
            this.lbl_rot_res.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_rot_res.Name = "lbl_rot_res";
            this.lbl_rot_res.Size = new System.Drawing.Size(147, 24);
            this.lbl_rot_res.TabIndex = 419;
            this.lbl_rot_res.Text = "旋转结果:";
            this.lbl_rot_res.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tb_ws_cap_after
            // 
            this.tb_ws_cap_after.BackColor = System.Drawing.Color.White;
            this.tb_ws_cap_after.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_ws_cap_after.Enabled = false;
            this.tb_ws_cap_after.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_ws_cap_after.ForeColor = System.Drawing.Color.Black;
            this.tb_ws_cap_after.Location = new System.Drawing.Point(492, 146);
            this.tb_ws_cap_after.Name = "tb_ws_cap_after";
            this.tb_ws_cap_after.Size = new System.Drawing.Size(92, 26);
            this.tb_ws_cap_after.TabIndex = 439;
            this.tb_ws_cap_after.Text = "-00.000";
            // 
            // lbl_xt2_cap
            // 
            this.lbl_xt2_cap.AutoSize = true;
            this.lbl_xt2_cap.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_xt2_cap.Location = new System.Drawing.Point(268, 126);
            this.lbl_xt2_cap.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_xt2_cap.Name = "lbl_xt2_cap";
            this.lbl_xt2_cap.Size = new System.Drawing.Size(86, 15);
            this.lbl_xt2_cap.TabIndex = 440;
            this.lbl_xt2_cap.Text = "吸头2位置偏差";
            // 
            // lbl_frontws
            // 
            this.lbl_frontws.AutoSize = true;
            this.lbl_frontws.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_frontws.Location = new System.Drawing.Point(384, 126);
            this.lbl_frontws.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_frontws.Name = "lbl_frontws";
            this.lbl_frontws.Size = new System.Drawing.Size(79, 15);
            this.lbl_frontws.TabIndex = 441;
            this.lbl_frontws.Text = "前排工站偏差";
            // 
            // lbl_behindws
            // 
            this.lbl_behindws.AutoSize = true;
            this.lbl_behindws.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_behindws.Location = new System.Drawing.Point(494, 126);
            this.lbl_behindws.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_behindws.Name = "lbl_behindws";
            this.lbl_behindws.Size = new System.Drawing.Size(79, 15);
            this.lbl_behindws.TabIndex = 442;
            this.lbl_behindws.Text = "后排工站偏差";
            // 
            // lbl_flycam_offset
            // 
            this.lbl_flycam_offset.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_flycam_offset.Location = new System.Drawing.Point(42, 195);
            this.lbl_flycam_offset.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_flycam_offset.Name = "lbl_flycam_offset";
            this.lbl_flycam_offset.Size = new System.Drawing.Size(105, 24);
            this.lbl_flycam_offset.TabIndex = 443;
            this.lbl_flycam_offset.Text = "飞拍偏差:";
            this.lbl_flycam_offset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tb_xt1_ofs
            // 
            this.tb_xt1_ofs.BackColor = System.Drawing.Color.White;
            this.tb_xt1_ofs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_xt1_ofs.Enabled = false;
            this.tb_xt1_ofs.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_xt1_ofs.ForeColor = System.Drawing.Color.Black;
            this.tb_xt1_ofs.Location = new System.Drawing.Point(146, 196);
            this.tb_xt1_ofs.Name = "tb_xt1_ofs";
            this.tb_xt1_ofs.Size = new System.Drawing.Size(89, 26);
            this.tb_xt1_ofs.TabIndex = 444;
            this.tb_xt1_ofs.Text = "-00.000";
            // 
            // tb_xt2_ofs
            // 
            this.tb_xt2_ofs.BackColor = System.Drawing.Color.White;
            this.tb_xt2_ofs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_xt2_ofs.Enabled = false;
            this.tb_xt2_ofs.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_xt2_ofs.ForeColor = System.Drawing.Color.Black;
            this.tb_xt2_ofs.Location = new System.Drawing.Point(262, 196);
            this.tb_xt2_ofs.Name = "tb_xt2_ofs";
            this.tb_xt2_ofs.Size = new System.Drawing.Size(92, 26);
            this.tb_xt2_ofs.TabIndex = 445;
            this.tb_xt2_ofs.Text = "-00.000";
            // 
            // tb_ws_ofs_before
            // 
            this.tb_ws_ofs_before.BackColor = System.Drawing.Color.White;
            this.tb_ws_ofs_before.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_ws_ofs_before.Enabled = false;
            this.tb_ws_ofs_before.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_ws_ofs_before.ForeColor = System.Drawing.Color.Black;
            this.tb_ws_ofs_before.Location = new System.Drawing.Point(379, 196);
            this.tb_ws_ofs_before.Name = "tb_ws_ofs_before";
            this.tb_ws_ofs_before.Size = new System.Drawing.Size(92, 26);
            this.tb_ws_ofs_before.TabIndex = 446;
            this.tb_ws_ofs_before.Text = "-00.000";
            // 
            // tb_ws_ofs_after
            // 
            this.tb_ws_ofs_after.BackColor = System.Drawing.Color.White;
            this.tb_ws_ofs_after.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_ws_ofs_after.Enabled = false;
            this.tb_ws_ofs_after.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_ws_ofs_after.ForeColor = System.Drawing.Color.Black;
            this.tb_ws_ofs_after.Location = new System.Drawing.Point(492, 196);
            this.tb_ws_ofs_after.Name = "tb_ws_ofs_after";
            this.tb_ws_ofs_after.Size = new System.Drawing.Size(92, 26);
            this.tb_ws_ofs_after.TabIndex = 447;
            this.tb_ws_ofs_after.Text = "-00.000";
            // 
            // lbl_mk
            // 
            this.lbl_mk.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_mk.Location = new System.Drawing.Point(615, 213);
            this.lbl_mk.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_mk.Name = "lbl_mk";
            this.lbl_mk.Size = new System.Drawing.Size(172, 24);
            this.lbl_mk.TabIndex = 448;
            this.lbl_mk.Text = "飞拍校正模块选取:";
            // 
            // cb_UDload
            // 
            this.cb_UDload.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_UDload.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cb_UDload.FormattingEnabled = true;
            this.cb_UDload.Location = new System.Drawing.Point(625, 247);
            this.cb_UDload.Name = "cb_UDload";
            this.cb_UDload.Size = new System.Drawing.Size(151, 27);
            this.cb_UDload.TabIndex = 426;
            this.cb_UDload.SelectedIndexChanged += new System.EventHandler(this.cb_UDload_SelectedIndexChanged);
            // 
            // RotAndFly
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(225)))), ((int)(((byte)(232)))));
            this.Controls.Add(this.lbl_mk);
            this.Controls.Add(this.tb_ws_ofs_after);
            this.Controls.Add(this.tb_ws_ofs_before);
            this.Controls.Add(this.tb_xt2_ofs);
            this.Controls.Add(this.tb_xt1_ofs);
            this.Controls.Add(this.lbl_flycam_offset);
            this.Controls.Add(this.lbl_behindws);
            this.Controls.Add(this.lbl_frontws);
            this.Controls.Add(this.lbl_xt2_cap);
            this.Controls.Add(this.tb_ws_cap_after);
            this.Controls.Add(this.btn_cam);
            this.Controls.Add(this.btn_zk_ctrl);
            this.Controls.Add(this.Btn_Live);
            this.Controls.Add(this.btn_fly_action);
            this.Controls.Add(this.lbl_xt1_cap);
            this.Controls.Add(this.tb_ws_cap_before);
            this.Controls.Add(this.tb_xt2_cap);
            this.Controls.Add(this.tb_xt1_cap);
            this.Controls.Add(this.lbl_flycam_test);
            this.Controls.Add(this.btn_rot_action);
            this.Controls.Add(this.cb_UDload);
            this.Controls.Add(this.cb_xt);
            this.Controls.Add(this.lbl_xt);
            this.Controls.Add(this.lbl_roty);
            this.Controls.Add(this.lbl_rotx);
            this.Controls.Add(this.lbl_rotrmse);
            this.Controls.Add(this.txb_rot_rmse);
            this.Controls.Add(this.lbl_rot_res);
            this.Controls.Add(this.txb_rot_y);
            this.Controls.Add(this.txb_rot_x);
            this.Controls.Add(this.btn_rot_downcam_movpos);
            this.Controls.Add(this.label74);
            this.Controls.Add(this.label42);
            this.Controls.Add(this.label52);
            this.Controls.Add(this.btn_rot_downcam_getpos);
            this.Controls.Add(this.nud_rot_downcam_y);
            this.Controls.Add(this.nud_rot_downcam_z);
            this.Controls.Add(this.nud_rot_downcam_x);
            this.Controls.Add(this.lbl_rot_downcam);
            this.Name = "RotAndFly";
            this.Size = new System.Drawing.Size(794, 289);
            ((System.ComponentModel.ISupportInitialize)(this.nud_rot_downcam_y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_rot_downcam_z)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_rot_downcam_x)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btn_rot_downcam_movpos;
        private System.Windows.Forms.Label label74;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.Button btn_rot_downcam_getpos;
        private System.Windows.Forms.NumericUpDown nud_rot_downcam_y;
        private System.Windows.Forms.NumericUpDown nud_rot_downcam_z;
        private System.Windows.Forms.NumericUpDown nud_rot_downcam_x;
        private System.Windows.Forms.Label lbl_rot_downcam;
        private System.Windows.Forms.TextBox txb_rot_y;
        private System.Windows.Forms.TextBox txb_rot_x;
        private System.Windows.Forms.TextBox txb_rot_rmse;
        private System.Windows.Forms.Label lbl_rotrmse;
        private System.Windows.Forms.Label lbl_rotx;
        private System.Windows.Forms.Label lbl_roty;
        private System.Windows.Forms.Label lbl_xt;
        private System.Windows.Forms.Button btn_rot_action;
        private System.Windows.Forms.Label lbl_flycam_test;
        private System.Windows.Forms.Label lbl_xt1_cap;
        private System.Windows.Forms.TextBox tb_ws_cap_before;
        private System.Windows.Forms.TextBox tb_xt2_cap;
        private System.Windows.Forms.TextBox tb_xt1_cap;
        private System.Windows.Forms.Button btn_fly_action;
        private System.Windows.Forms.Button btn_cam;
        private System.Windows.Forms.Button btn_zk_ctrl;
        private System.Windows.Forms.Button Btn_Live;
        private System.Windows.Forms.Label lbl_rot_res;
        private System.Windows.Forms.TextBox tb_ws_cap_after;
        private System.Windows.Forms.Label lbl_xt2_cap;
        private System.Windows.Forms.Label lbl_frontws;
        private System.Windows.Forms.Label lbl_behindws;
        private System.Windows.Forms.Label lbl_flycam_offset;
        private System.Windows.Forms.TextBox tb_xt1_ofs;
        private System.Windows.Forms.TextBox tb_xt2_ofs;
        private System.Windows.Forms.TextBox tb_ws_ofs_before;
        private System.Windows.Forms.TextBox tb_ws_ofs_after;
        private System.Windows.Forms.Label lbl_mk;
        public System.Windows.Forms.ComboBox cb_xt;
        public System.Windows.Forms.ComboBox cb_UDload;
    }
}
