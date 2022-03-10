namespace UI
{
    partial class CogDisplayer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CogDisplayer));
            this.openFileDlg = new System.Windows.Forms.OpenFileDialog();
            this.tbp_status = new System.Windows.Forms.TableLayoutPanel();
            this.btn_img = new System.Windows.Forms.Button();
            this.statusbar = new Cognex.VisionPro.CogDisplayStatusBarV2();
            this.btn_live = new System.Windows.Forms.Button();
            this.btn_triger = new System.Windows.Forms.Button();
            this.cb_flow = new System.Windows.Forms.ComboBox();
            this.cb_cam = new System.Windows.Forms.ComboBox();
            this.btn_show_tool = new System.Windows.Forms.Button();
            this.tbp_main = new System.Windows.Forms.TableLayoutPanel();
            this.pnl_display = new System.Windows.Forms.Panel();
            this.ts_param = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.tsc_show_mode = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_cam_list = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_block_edit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.stb_exposure = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.stb_brightness = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.stb_contrast = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_save = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_TestOrGo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_GoCenter = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_save_screen = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator28 = new System.Windows.Forms.ToolStripSeparator();
            this.tsb_continue = new System.Windows.Forms.ToolStripButton();
            this.cogRecordDisplay = new Cognex.VisionPro.CogRecordDisplay();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.tbp_status.SuspendLayout();
            this.tbp_main.SuspendLayout();
            this.pnl_display.SuspendLayout();
            this.ts_param.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecordDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDlg
            // 
            this.openFileDlg.FileName = "openFileDialog1";
            this.openFileDlg.Filter = "单色位图(*.bmp)|*.bmp";
            // 
            // tbp_status
            // 
            this.tbp_status.ColumnCount = 7;
            this.tbp_status.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tbp_status.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tbp_status.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tbp_status.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tbp_status.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tbp_status.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tbp_status.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tbp_status.Controls.Add(this.btn_img, 3, 0);
            this.tbp_status.Controls.Add(this.statusbar, 0, 0);
            this.tbp_status.Controls.Add(this.btn_live, 5, 0);
            this.tbp_status.Controls.Add(this.btn_triger, 4, 0);
            this.tbp_status.Controls.Add(this.cb_flow, 2, 0);
            this.tbp_status.Controls.Add(this.cb_cam, 1, 0);
            this.tbp_status.Controls.Add(this.btn_show_tool, 6, 0);
            this.tbp_status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbp_status.Location = new System.Drawing.Point(0, 468);
            this.tbp_status.Margin = new System.Windows.Forms.Padding(0);
            this.tbp_status.Name = "tbp_status";
            this.tbp_status.RowCount = 1;
            this.tbp_status.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tbp_status.Size = new System.Drawing.Size(596, 26);
            this.tbp_status.TabIndex = 1;
            // 
            // btn_img
            // 
            this.btn_img.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_img.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_img.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_img.Location = new System.Drawing.Point(396, 0);
            this.btn_img.Margin = new System.Windows.Forms.Padding(0);
            this.btn_img.Name = "btn_img";
            this.btn_img.Size = new System.Drawing.Size(50, 26);
            this.btn_img.TabIndex = 7;
            this.btn_img.Text = "图片";
            this.btn_img.UseVisualStyleBackColor = false;
            this.btn_img.Click += new System.EventHandler(this.btn_img_Click);
            // 
            // statusbar
            // 
            this.statusbar.CoordinateSpaceName = "*\\#";
            this.statusbar.CoordinateSpaceName3D = "*\\#";
            this.statusbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusbar.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.statusbar.Location = new System.Drawing.Point(0, 0);
            this.statusbar.Margin = new System.Windows.Forms.Padding(0);
            this.statusbar.Name = "statusbar";
            this.statusbar.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.statusbar.ShowPixelValuePane = false;
            this.statusbar.ShowZoomPane = false;
            this.statusbar.Size = new System.Drawing.Size(206, 26);
            this.statusbar.TabIndex = 0;
            this.statusbar.Use3DCoordinateSpaceTree = false;
            // 
            // btn_live
            // 
            this.btn_live.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_live.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_live.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_live.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_live.Location = new System.Drawing.Point(496, 0);
            this.btn_live.Margin = new System.Windows.Forms.Padding(0);
            this.btn_live.Name = "btn_live";
            this.btn_live.Size = new System.Drawing.Size(60, 26);
            this.btn_live.TabIndex = 2;
            this.btn_live.Text = "实况";
            this.btn_live.UseVisualStyleBackColor = false;
            this.btn_live.Click += new System.EventHandler(this.btn_live_Click);
            // 
            // btn_triger
            // 
            this.btn_triger.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_triger.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_triger.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_triger.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_triger.Location = new System.Drawing.Point(447, 0);
            this.btn_triger.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.btn_triger.Name = "btn_triger";
            this.btn_triger.Size = new System.Drawing.Size(49, 26);
            this.btn_triger.TabIndex = 5;
            this.btn_triger.Text = "触发";
            this.btn_triger.UseVisualStyleBackColor = false;
            this.btn_triger.Click += new System.EventHandler(this.btn_triger_Click);
            // 
            // cb_flow
            // 
            this.cb_flow.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.cb_flow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_flow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_flow.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cb_flow.FormattingEnabled = true;
            this.cb_flow.Items.AddRange(new object[] {
            "未加载"});
            this.cb_flow.Location = new System.Drawing.Point(287, 0);
            this.cb_flow.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.cb_flow.Name = "cb_flow";
            this.cb_flow.Size = new System.Drawing.Size(108, 24);
            this.cb_flow.TabIndex = 3;
            this.cb_flow.SelectedIndexChanged += new System.EventHandler(this.cb_flow_SelectedIndexChanged);
            // 
            // cb_cam
            // 
            this.cb_cam.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.cb_cam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cb_cam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_cam.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cb_cam.FormattingEnabled = true;
            this.cb_cam.Items.AddRange(new object[] {
            "未加载"});
            this.cb_cam.Location = new System.Drawing.Point(207, 0);
            this.cb_cam.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.cb_cam.Name = "cb_cam";
            this.cb_cam.Size = new System.Drawing.Size(78, 24);
            this.cb_cam.TabIndex = 4;
            this.cb_cam.SelectedIndexChanged += new System.EventHandler(this.cb_cam_SelectedIndexChanged);
            // 
            // btn_show_tool
            // 
            this.btn_show_tool.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn_show_tool.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_show_tool.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_show_tool.Location = new System.Drawing.Point(556, 0);
            this.btn_show_tool.Margin = new System.Windows.Forms.Padding(0);
            this.btn_show_tool.Name = "btn_show_tool";
            this.btn_show_tool.Size = new System.Drawing.Size(40, 26);
            this.btn_show_tool.TabIndex = 8;
            this.btn_show_tool.Text = ">>";
            this.btn_show_tool.UseVisualStyleBackColor = false;
            this.btn_show_tool.Click += new System.EventHandler(this.btn_show_tool_Click);
            // 
            // tbp_main
            // 
            this.tbp_main.ColumnCount = 1;
            this.tbp_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tbp_main.Controls.Add(this.tbp_status, 0, 1);
            this.tbp_main.Controls.Add(this.pnl_display, 0, 0);
            this.tbp_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbp_main.Location = new System.Drawing.Point(0, 0);
            this.tbp_main.Margin = new System.Windows.Forms.Padding(0);
            this.tbp_main.Name = "tbp_main";
            this.tbp_main.RowCount = 2;
            this.tbp_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tbp_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tbp_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tbp_main.Size = new System.Drawing.Size(596, 494);
            this.tbp_main.TabIndex = 0;
            // 
            // pnl_display
            // 
            this.pnl_display.Controls.Add(this.ts_param);
            this.pnl_display.Controls.Add(this.cogRecordDisplay);
            this.pnl_display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_display.Location = new System.Drawing.Point(3, 3);
            this.pnl_display.Name = "pnl_display";
            this.pnl_display.Size = new System.Drawing.Size(590, 462);
            this.pnl_display.TabIndex = 2;
            // 
            // ts_param
            // 
            this.ts_param.AutoSize = false;
            this.ts_param.BackColor = System.Drawing.SystemColors.Control;
            this.ts_param.Dock = System.Windows.Forms.DockStyle.Right;
            this.ts_param.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ts_param.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ts_param.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel4,
            this.tsc_show_mode,
            this.toolStripSeparator27,
            this.tsb_cam_list,
            this.toolStripSeparator2,
            this.tsb_block_edit,
            this.toolStripSeparator14,
            this.toolStripLabel5,
            this.stb_exposure,
            this.toolStripSeparator15,
            this.toolStripLabel2,
            this.stb_brightness,
            this.toolStripSeparator16,
            this.toolStripLabel3,
            this.stb_contrast,
            this.toolStripSeparator1,
            this.tsb_save,
            this.toolStripSeparator3,
            this.tsb_TestOrGo,
            this.toolStripSeparator5,
            this.tsb_GoCenter,
            this.toolStripSeparator4,
            this.tsb_save_screen,
            this.toolStripSeparator28,
            this.tsb_continue});
            this.ts_param.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.ts_param.Location = new System.Drawing.Point(483, 0);
            this.ts_param.Name = "ts_param";
            this.ts_param.Size = new System.Drawing.Size(107, 462);
            this.ts_param.TabIndex = 2;
            this.ts_param.Visible = false;
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(105, 17);
            this.toolStripLabel4.Text = "显示模式";
            this.toolStripLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsc_show_mode
            // 
            this.tsc_show_mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tsc_show_mode.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.tsc_show_mode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsc_show_mode.Items.AddRange(new object[] {
            "当前相机",
            "当前流程",
            "所有相机",
            "所有流程"});
            this.tsc_show_mode.Name = "tsc_show_mode";
            this.tsc_show_mode.Size = new System.Drawing.Size(103, 25);
            // 
            // toolStripSeparator27
            // 
            this.toolStripSeparator27.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripSeparator27.Name = "toolStripSeparator27";
            this.toolStripSeparator27.Size = new System.Drawing.Size(105, 6);
            // 
            // tsb_cam_list
            // 
            this.tsb_cam_list.AutoSize = false;
            this.tsb_cam_list.BackColor = System.Drawing.SystemColors.Control;
            this.tsb_cam_list.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsb_cam_list.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tsb_cam_list.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsb_cam_list.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsb_cam_list.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_cam_list.Name = "tsb_cam_list";
            this.tsb_cam_list.Size = new System.Drawing.Size(80, 26);
            this.tsb_cam_list.Text = "相机列表";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(105, 6);
            // 
            // tsb_block_edit
            // 
            this.tsb_block_edit.AutoSize = false;
            this.tsb_block_edit.BackColor = System.Drawing.SystemColors.Control;
            this.tsb_block_edit.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsb_block_edit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tsb_block_edit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsb_block_edit.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsb_block_edit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_block_edit.Name = "tsb_block_edit";
            this.tsb_block_edit.Size = new System.Drawing.Size(80, 26);
            this.tsb_block_edit.Text = "流程编辑";
            this.tsb_block_edit.Click += new System.EventHandler(this.tsb_block_edit_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(105, 6);
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(105, 17);
            this.toolStripLabel5.Text = "曝光(ms)";
            this.toolStripLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // stb_exposure
            // 
            this.stb_exposure.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.stb_exposure.Name = "stb_exposure";
            this.stb_exposure.Size = new System.Drawing.Size(103, 24);
            this.stb_exposure.Text = "1.000";
            this.stb_exposure.TextChanged += new System.EventHandler(this.stb_exposure_TextChanged);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(105, 6);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(105, 17);
            this.toolStripLabel2.Text = "亮度(0-1)";
            this.toolStripLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // stb_brightness
            // 
            this.stb_brightness.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.stb_brightness.Name = "stb_brightness";
            this.stb_brightness.Size = new System.Drawing.Size(103, 24);
            this.stb_brightness.Text = "1.000";
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(105, 6);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(105, 17);
            this.toolStripLabel3.Text = "对比度(0-1)";
            this.toolStripLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // stb_contrast
            // 
            this.stb_contrast.BackColor = System.Drawing.SystemColors.Window;
            this.stb_contrast.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.stb_contrast.Name = "stb_contrast";
            this.stb_contrast.Size = new System.Drawing.Size(103, 24);
            this.stb_contrast.Text = "1.000";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(105, 6);
            // 
            // tsb_save
            // 
            this.tsb_save.AutoSize = false;
            this.tsb_save.BackColor = System.Drawing.SystemColors.Control;
            this.tsb_save.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsb_save.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tsb_save.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsb_save.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsb_save.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_save.Name = "tsb_save";
            this.tsb_save.Size = new System.Drawing.Size(80, 26);
            this.tsb_save.Text = "保存参数";
            this.tsb_save.Click += new System.EventHandler(this.tsb_save_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(105, 6);
            // 
            // tsb_TestOrGo
            // 
            this.tsb_TestOrGo.AutoSize = false;
            this.tsb_TestOrGo.BackColor = System.Drawing.Color.Transparent;
            this.tsb_TestOrGo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsb_TestOrGo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tsb_TestOrGo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsb_TestOrGo.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsb_TestOrGo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_TestOrGo.Name = "tsb_TestOrGo";
            this.tsb_TestOrGo.Size = new System.Drawing.Size(80, 26);
            this.tsb_TestOrGo.Text = "定位模式";
            this.tsb_TestOrGo.Click += new System.EventHandler(this.tsb_TestOrGo_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(105, 6);
            // 
            // tsb_GoCenter
            // 
            this.tsb_GoCenter.AutoSize = false;
            this.tsb_GoCenter.BackColor = System.Drawing.SystemColors.Control;
            this.tsb_GoCenter.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsb_GoCenter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tsb_GoCenter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsb_GoCenter.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsb_GoCenter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_GoCenter.Name = "tsb_GoCenter";
            this.tsb_GoCenter.Size = new System.Drawing.Size(80, 26);
            this.tsb_GoCenter.Text = "图像对中";
            this.tsb_GoCenter.Click += new System.EventHandler(this.tsb_GoCenter_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(105, 6);
            // 
            // tsb_save_screen
            // 
            this.tsb_save_screen.AutoSize = false;
            this.tsb_save_screen.BackColor = System.Drawing.SystemColors.Control;
            this.tsb_save_screen.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsb_save_screen.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tsb_save_screen.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsb_save_screen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsb_save_screen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_save_screen.Name = "tsb_save_screen";
            this.tsb_save_screen.Size = new System.Drawing.Size(80, 26);
            this.tsb_save_screen.Text = "保存图片";
            this.tsb_save_screen.Click += new System.EventHandler(this.tsb_save_pic_Click);
            // 
            // toolStripSeparator28
            // 
            this.toolStripSeparator28.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripSeparator28.Name = "toolStripSeparator28";
            this.toolStripSeparator28.Size = new System.Drawing.Size(105, 6);
            // 
            // tsb_continue
            // 
            this.tsb_continue.AutoSize = false;
            this.tsb_continue.BackColor = System.Drawing.SystemColors.Control;
            this.tsb_continue.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsb_continue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tsb_continue.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsb_continue.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsb_continue.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_continue.Name = "tsb_continue";
            this.tsb_continue.Size = new System.Drawing.Size(80, 26);
            this.tsb_continue.Text = "连续测量";
            this.tsb_continue.Click += new System.EventHandler(this.tsb_continue_Click);
            // 
            // cogRecordDisplay
            // 
            this.cogRecordDisplay.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.cogRecordDisplay.ColorMapLowerRoiLimit = 0D;
            this.cogRecordDisplay.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.cogRecordDisplay.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.cogRecordDisplay.ColorMapUpperRoiLimit = 1D;
            this.cogRecordDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogRecordDisplay.DoubleTapZoomCycleLength = 2;
            this.cogRecordDisplay.DoubleTapZoomSensitivity = 2.5D;
            this.cogRecordDisplay.Location = new System.Drawing.Point(0, 0);
            this.cogRecordDisplay.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogRecordDisplay.MouseWheelSensitivity = 1D;
            this.cogRecordDisplay.Name = "cogRecordDisplay";
            this.cogRecordDisplay.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogRecordDisplay.OcxState")));
            this.cogRecordDisplay.Size = new System.Drawing.Size(590, 462);
            this.cogRecordDisplay.TabIndex = 1;
            this.cogRecordDisplay.DoubleClick += new System.EventHandler(this.cogRecordDisplay_DoubleClick);
            this.cogRecordDisplay.Click += new System.EventHandler(this.cogRecordDisplay_Click);
            this.cogRecordDisplay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cogRecordDisplay_MouseDown);
            this.cogRecordDisplay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cogRecordDisplay_MouseUp);
            this.cogRecordDisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cogRecordDisplay_MouseMove);
            // 
            // CogDisplayer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tbp_main);
            this.Name = "CogDisplayer";
            this.Size = new System.Drawing.Size(596, 494);
            this.tbp_status.ResumeLayout(false);
            this.tbp_main.ResumeLayout(false);
            this.pnl_display.ResumeLayout(false);
            this.ts_param.ResumeLayout(false);
            this.ts_param.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecordDisplay)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDlg;
        private System.Windows.Forms.TableLayoutPanel tbp_status;
        private Cognex.VisionPro.CogDisplayStatusBarV2 statusbar;
        public Cognex.VisionPro.CogRecordDisplay cogRecordDisplay;
        private System.Windows.Forms.Button btn_show_tool;
        public System.Windows.Forms.Button btn_live;
        public System.Windows.Forms.Button btn_triger;
        public System.Windows.Forms.ToolStripComboBox tsc_show_mode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator27;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripTextBox stb_exposure;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripTextBox stb_brightness;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripTextBox stb_contrast;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator28;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        public System.Windows.Forms.ComboBox cb_cam;
        public System.Windows.Forms.ComboBox cb_flow;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        public System.Windows.Forms.Button btn_img;
        public System.Windows.Forms.TableLayoutPanel tbp_main;
        public System.Windows.Forms.Panel pnl_display;
        public System.Windows.Forms.ToolStrip ts_param;
        public System.Windows.Forms.ToolStripButton tsb_cam_list;
        public System.Windows.Forms.ToolStripButton tsb_block_edit;
        public System.Windows.Forms.ToolStripLabel toolStripLabel2;
        public System.Windows.Forms.ToolStripLabel toolStripLabel3;
        public System.Windows.Forms.ToolStripButton tsb_save;
        public System.Windows.Forms.ToolStripLabel toolStripLabel4;
        public System.Windows.Forms.ToolStripLabel toolStripLabel5;
        public System.Windows.Forms.ToolStripButton tsb_save_screen;
        public System.Windows.Forms.ToolStripButton tsb_continue;
        public System.Windows.Forms.ToolStripButton tsb_TestOrGo;
        public System.Windows.Forms.ToolStripButton tsb_GoCenter;
    }
}
