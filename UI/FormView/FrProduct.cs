using System;
using System.Drawing;
using System.Windows.Forms;
using MotionCtrl;
using System.IO;
using System.Threading;
using Win32Lib;

namespace UI
{
    public partial class FrProduct : Form
    {
        private sealed class TrayCfgBinding
        {
            public Product.Tray Tray;
            public TrayBox TrayBox;
            public string TrayAxisName;
        }

        public Product.Tray tray = new Product.Tray();
        public Product.Tray tray_fd;
        public Product.Tray tray_ok;
        public Product.Tray tray_ng;
        public TrayBox traybox = new TrayBox();
        public bool gettraydata = false;
        public bool bPosUpdate = false;
        

        public FrProduct()
        {
            InitializeComponent();
            //PmsfrProduct(User.PERMISSION.Operator);
            tray_map.bShowMode = true;
            if (FrMain.frsuser != null)
                PmsfrProduct(FrMain.frsuser.user1.cur_user.pms);
            else
                PmsfrProduct(User.PERMISSION.Operator);
        }

        private bool IsTrayUiSwapped => RuntimeMachineMode.IsTrayBoxSwapped;

        private TrayCfgBinding GetTrayCfgBinding(string tabName)
        {
            switch (tabName)
            {
                case "ctp_tray_fd":
                    return new TrayCfgBinding
                    {
                        Tray = tray_fd,
                        TrayBox = COM.traybox_fd,
                        TrayAxisName = VAR.IsChinese ? "供料轴" : "Feed tray"
                    };
                case "ctp_tray_ok":
                    return new TrayCfgBinding { Tray = tray_ok, TrayBox = COM.traybox_ok, TrayAxisName = VAR.IsChinese ? "OK料轴" : "Ok tray" };
                case "ctp_tray_ng":
                    return new TrayCfgBinding { Tray = tray_ng, TrayBox = COM.traybox_ng, TrayAxisName = VAR.IsChinese ? "NG料轴" : "Ng tray" };
                default:
                    return new TrayCfgBinding
                    {
                        Tray = tray_fd,
                        TrayBox = COM.traybox_fd,
                        TrayAxisName = VAR.IsChinese ? "供料轴" : "Feed tray"
                    };
            }
        }

        private TrayCfgBinding GetCurrentTrayCfgBinding()
        {
            return GetTrayCfgBinding(ctb_tray_cfg.SelectedTab != null ? ctb_tray_cfg.SelectedTab.Name : "ctp_tray_fd");
        }

        private void UpdateTrayCfgTabText()
        {
            ctp_tray_ok.Text = "  OK料盘  ";
            ctp_tray_ng.Text = "  NG料盘  ";

            ctb_tray_cfg.TabPages.Remove(ctp_tray_ok);
            ctb_tray_cfg.TabPages.Remove(ctp_tray_ng);
            if (IsTrayUiSwapped)
            {
                ctb_tray_cfg.TabPages.Add(ctp_tray_ng);
                ctb_tray_cfg.TabPages.Add(ctp_tray_ok);
            }
            else
            {
                ctb_tray_cfg.TabPages.Add(ctp_tray_ok);
                ctb_tray_cfg.TabPages.Add(ctp_tray_ng);
            }
        }

        public bool bupdate
        {
            get { return tmr_update.Enabled; }
            set { tmr_update.Enabled = value; }
        }

        private void PmsEn(User.PERMISSION pms)
        {
            User.PERMISSION userPms = User.PERMISSION.Engineer;
            btn_new.Enabled = (pms > userPms) ? true : false;
            btn_change.Enabled = (pms > userPms) ? true : false;
            btn_del.Enabled = (pms > userPms) ? true : false;
            btn_rename.Enabled = (pms > userPms) ? true : false;
            //料盘信息
            nud_tray_col.Enabled = (pms > userPms) ? true : false;
            nud_tray_row.Enabled = (pms > userPms) ? true : false;
            xyzu_trayvs.Enabled= (pms > userPms) ? true : false;
            xyzu_tray_study.Enabled = (pms > userPms) ? true : false;
            btn_tray_vspos_get.Enabled= (pms > userPms) ? true : false;
            btn_tray_vspos_goto.Enabled = (pms > userPms) ? true : false;
            btn_tray_vspos_gocenter.Enabled= (pms > userPms) ? true : false;
            btn_tray_get.Enabled = (pms > userPms) ? true : false;
            btn_tray_goto.Enabled = (pms >= userPms) ? true : false;
            btn_tray_make_array.Enabled = (pms > userPms) ? true : false;
            btn_tray_copy.Enabled = (pms > userPms) ? true : false;
            btn_tray_save.Enabled = (pms > userPms) ? true : false;
            btn_VsCalc.Enabled = (pms > userPms) ? true : false;
            btn_Vsgopos.Enabled = (pms > userPms) ? true : false;
            gettraydata= (pms > userPms) ? true : false;
            dgv_tray_pos.Columns[0].ReadOnly = true;
            dgv_tray_pos.Columns[1].ReadOnly = (pms <= userPms) ? true : false;
            dgv_tray_pos.Columns[2].ReadOnly = (pms <= userPms) ? true : false; 
            dgv_tray_pos.Columns[3].ReadOnly = (pms <= userPms) ? true : false; 
            dgv_tray_pos.Columns[4].ReadOnly = (pms <= userPms) ? true : false;
            dgv_tray_pos.Columns[5].ReadOnly = (pms <= userPms) ? true : false;
            //仓储参数
            nud_frist_tray_zpos.Enabled = (pms > userPms) ? true : false;
            nud_tray_heigh.Enabled = (pms > userPms) ? true : false;
            nud_tray_feed_ofs_h.Enabled = (pms > userPms) ? true : false;
            nud_tray_feed_x.Enabled = (pms > userPms) ? true : false;
            nud_tray_feed_safe_x.Enabled = (pms > userPms) ? true : false;
            nud_tray_openCy_x.Enabled = (pms > userPms) ? true : false;
            nud_tray_cnt.Enabled = (pms > userPms) ? true : false;
            btn_traybox_save.Enabled = (pms > userPms) ? true : false;
            nud_tray_Chk_High_z.Enabled = (pms > userPms) ? true : false;
            nud_tray_barcode_x1.Enabled= (pms > userPms) ? true : false;
            nud_tray_barcode_y1.Enabled = (pms > userPms) ? true : false;
            nud_tray_barcode_x2.Enabled = (pms > userPms) ? true : false;
            nud_tray_barcode_y2.Enabled = (pms > userPms) ? true : false;
            nud_tray_barcode_a1.Enabled = (pms > userPms) ? true : false;
            nud_tray_barcode_a2.Enabled = (pms > userPms) ? true : false;
            //下料参数
            btn_ud1_hightest.Enabled = (pms > userPms) ? true : false;
            btn_ud_hightest.Enabled = (pms > userPms) ? true : false;
            btn_ud2_hightest.Enabled= (pms > userPms) ? true : false;
            nud_PosCapLZ_y.Enabled = (pms > userPms) ? true : false;
            nud_PosCapLZ_x.Enabled = (pms > userPms) ? true : false;
            nud_PosCapLZ_Limit.Enabled= (pms > userPms) ? true : false;
            btn_vs_calpos.Enabled = (pms > userPms) ? true : false;
            btn_Pos_LiZhu_get.Enabled= (pms > userPms) ? true : false;
            btn_Pos_LiZhu_goto.Enabled= (pms > userPms) ? true : false;
            cogDisplayer_product.PmsEn((pms > userPms) ? true : false);
        }

        public void PmsfrProduct(User.PERMISSION pms)
        {
           lightBoxDef.PmsEn(pms);
           wsModuelDef.PmsEn(pms);
           xtOfsAdj1.PmsEn(pms);
           pmAlignEdit1.PmsEn(pms);
           PmsEn(pms);
        }
        private void FrProduct_Load(object sender, EventArgs e)
        {
            COM.product.LoadProductList(ltb_product);
            edt_cur_product.Text = VAR.gsys_set.cur_product_name;
            ctb_lightbox.SelectedIndex = 0;
            lightBoxDef.lightbox = COM.LeftLightBox;
            lightBoxDef.UpdateShow();

            cmb_tt_pos.Items.Clear();
            cmb_tt_pos.Items.Add(Utility.GetDescription(Turntable.EM_STA.POS0, VAR.IsChinese));
            cmb_tt_pos.Items.Add(Utility.GetDescription(Turntable.EM_STA.POS1, VAR.IsChinese));
            cmb_tt_pos.Items.Add(Utility.GetDescription(Turntable.EM_STA.POS2, VAR.IsChinese));
            cmb_tt_pos.Items.Add(Utility.GetDescription(Turntable.EM_STA.POS3, VAR.IsChinese));
            cmb_tt_pos.Items.Add(Utility.GetDescription(Turntable.EM_STA.MOVING, VAR.IsChinese));
            cmb_tt_pos.Items.Add(Utility.GetDescription(Turntable.EM_STA.ERR, VAR.IsChinese));
            cmb_tt_pos.Items.Add(Utility.GetDescription(Turntable.EM_STA.ALM, VAR.IsChinese));
            cmb_tt_pos.Items.Add(Utility.GetDescription(Turntable.EM_STA.UNKNOW, VAR.IsChinese));
            cogDisplayer_product.list_cam.Clear();
            cogDisplayer_product.AddCam(COM.ListCam);
            pmAlignEdit1.InitDisPlay(cogDisplayer_product.cogRecordDisplay, maskDisplay1);
            pmAlignEdit1.list_cam.Clear();
            pmAlignEdit1.AddCam(COM.ListCam);
            
            tray_fd = COM.tray_fd;
            tray_ok = COM.tray_ok;
            tray_ng = COM.tray_ng;

            tray = tray_fd;
            traybox = COM.traybox_fd;
            UpdateTrayCfgTabText();
            ctb_tray_cfg_SelectedIndexChanged(ctb_tray_cfg, EventArgs.Empty);
            xyzu_tray_study.XYZUVisible = "True,True,True,True";
            xyzu_trayvs.XYZUVisible = "True,True,True,False";
			
			COM.LeftLightBox.LoadCfg();
            //FrMain.frproduct.lightBoxDef.serialPort1.ReceivedBytesThreshold = 1;
            //FrMain.frproduct.lightBoxDef.serialPort1.PortName = "COM2";
            //FrMain.frproduct.lightBoxDef.serialPort1.BaudRate = 9600;
            //FrMain.frproduct.lightBoxDef.serialPort1.Close();
            //FrMain.frproduct.lightBoxDef.serialPort1.Open();
            //FrMain.frproduct.lightBoxDef.serialPort1.DataReceived +=
            //    FrMain.frproduct.lightBoxDef.serialPort1_DataReceived;
        }

        public void ShowTrayData(Product.Tray tray, bool bposdata = true)
        {
        int id = 0;
        AXIS ax_z = null;
        if (ctb_udload_cfg.SelectedIndex > 1 || ctb_udload_cfg.SelectedIndex < 0)
        {
            ctb_udload_cfg.SelectedIndex = 0;
        }
        id = ctb_udload_cfg.SelectedIndex;

            try
            {
                bPosUpdate = true;
                ax_z = COM.List_UDLoad[id].ax_z;
                if (Math.Abs(ax_z.fenc_pos) > 1)
                {
                    btn_dwup.Text = VAR.IsChinese?"抬起":"UP";
                }
                else
                {
                    btn_dwup.Text = VAR.IsChinese ? "下降":"DOWN";
                }

                nud_tray_col.Value = tray.col;
                nud_tray_row.Value = tray.row;
                cbouttray.Checked = tray.IsoutTray;

                xyzu_trayvs.Value = tray.TrayVsPos[id];

                if (rbtn_tray_tl.Checked)
                {
                    xyzu_tray_study.Value = tray.tl[id];
                    tray.AddQrcodeTL[id].x = tray.tl[id].x;
                    tray.AddQrcodeTL[id].a = tray.tl[id].a;
                    nud_AddCapQrcodePosX.Value = (decimal)tray.AddQrcodeTL[id].x;
                    nud_AddCapQrcodePosY.Value = (decimal)tray.AddQrcodeTL[id].y;
                    nud_AddCapQrcodePosTX.Value = (decimal)tray.AddQrcodeTL[id].a;
                }
                else if (rbtn_tray_bl.Checked)
                {
                    xyzu_tray_study.Value = tray.bl[id];
                    tray.AddQrcodeBL[id].x = tray.bl[id].x;
                    tray.AddQrcodeBL[id].a = tray.bl[id].a;
                    nud_AddCapQrcodePosX.Value = (decimal)tray.AddQrcodeBL[id].x;
                    nud_AddCapQrcodePosY.Value = (decimal)tray.AddQrcodeBL[id].y;
                    nud_AddCapQrcodePosTX.Value = (decimal)tray.AddQrcodeBL[id].a;
                }
                else if (rbtn_tray_tr.Checked)
                {
                    xyzu_tray_study.Value = tray.tr[id];
                    tray.AddQrcodeTR[id].x = tray.tr[id].x;
                    tray.AddQrcodeTR[id].a = tray.tr[id].a;
                    nud_AddCapQrcodePosX.Value = (decimal)tray.AddQrcodeTR[id].x;
                    nud_AddCapQrcodePosY.Value = (decimal)tray.AddQrcodeTR[id].y;
                    nud_AddCapQrcodePosTX.Value = (decimal)tray.AddQrcodeTR[id].a;
                }

                if (!bposdata) return;

                tray_map.tray_dat = tray;
                tray_map.UpdateShow();
                tray_map.Refresh();
                dgv_tray_pos.Rows.Clear();
                foreach (Product.Tray.PosInf pos in tray.list_pos)
                {
                    int idx = dgv_tray_pos.Rows.Add();
                    dgv_tray_pos.Rows[idx].Cells["编号"].Value = pos.idx;
                    dgv_tray_pos.Rows[idx].Cells["X"].Value = pos.Pos[id].x.ToString("F3");
                    dgv_tray_pos.Rows[idx].Cells["Y"].Value = pos.Pos[id].y.ToString("F3");
                    dgv_tray_pos.Rows[idx].Cells["Z"].Value = pos.Pos[id].z.ToString("F3");
                    dgv_tray_pos.Rows[idx].Cells["U"].Value = pos.Pos[id].a.ToString("F3");
                    dgv_tray_pos.Rows[idx].Cells["chk_use"].Value = !pos.isDisable;
                }


                if (ctb_tray_cfg.SelectedIndex > 2 || ctb_tray_cfg.SelectedIndex < 0)
                {
                    ctb_tray_cfg.SelectedIndex = 0;
                }

                id = ctb_tray_cfg.SelectedIndex;
                if (id == 0)
                {
                    btn_VsCalc.Visible = false;
                    btn_Vsgopos.Visible = false;
                }
                else
                {
                    btn_VsCalc.Visible = true;
                    btn_Vsgopos.Visible = true;
            }
            }
        finally
        {
                bPosUpdate = false;
        }
            
    }

         public  void ShowTrayBoxData(TrayBox traybox)
        {
            nud_frist_tray_zpos.Value = (decimal)traybox.st_first_tray_pos.z;
            nud_tray_heigh.Value = (decimal)traybox.tray_heigh;
            nud_tray_feed_ofs_h.Value= (decimal)traybox.tray_feed_ofs_h;
            nud_tray_feed_x.Value = (decimal)traybox.fd_pos_x;
            nud_tray_feed_safe_x.Value = (decimal)traybox.fd_safe_x;
            nud_tray_openCy_x.Value= (decimal)traybox.fd_openCy_pos_x;
            nud_tray_cnt.Value = (decimal)traybox.tray_cnt;
            nud_tray_Chk_High_z.Value = (decimal)traybox.fd_chk_high_z;
            nud_tray_barcode_x1.Value = (decimal)COM.traybox_fd.tray_barcode_x1;
            nud_tray_barcode_y1.Value = (decimal)COM.traybox_fd.tray_barcode_y1;
            nud_tray_barcode_x2.Value = (decimal)COM.traybox_fd.tray_barcode_x2;
            nud_tray_barcode_y2.Value = (decimal)COM.traybox_fd.tray_barcode_y2;
            nud_tray_barcode_a1.Value = (decimal)COM.traybox_fd.tray_barcode_a1;
            nud_tray_barcode_a2.Value = (decimal)COM.traybox_fd.tray_barcode_a2;
            nud_motor_y1.Value = (decimal)COM.traybox_fd.motor_barcode_pos[0];
            nud_motor_y2.Value = (decimal)COM.traybox_fd.motor_barcode_pos[1];
            nud_motor_y3.Value = (decimal)COM.traybox_fd.motor_barcode_pos[2];
            nud_motor_y4.Value = (decimal)COM.traybox_fd.motor_barcode_pos[3];
        }

        void GetTrayData(Product.Tray tray,bool bgetpos = true)
        {
            int id = 0;
            string str;
            if (ctb_udload_cfg.SelectedIndex > 1 || ctb_udload_cfg.SelectedIndex < 0)
            {
                ctb_udload_cfg.SelectedIndex = 0;
            }
            id = ctb_udload_cfg.SelectedIndex;
            tray.col= (int)nud_tray_col.Value ;
            tray.row = (int)nud_tray_row.Value;
            tray.TrayVsPos[id] = xyzu_trayvs.Value;
            tray.IsoutTray  = cbouttray.Checked;
            if (rbtn_tray_tl.Checked) tray.tl[id] = xyzu_tray_study.Value;
            else if (rbtn_tray_bl.Checked) tray.bl[id] = xyzu_tray_study.Value;
            else if (rbtn_tray_tr.Checked) tray.tr[id] = xyzu_tray_study.Value;           
            foreach (DataGridViewRow row in dgv_tray_pos.Rows)
            {
                str = row.Cells["chk_use"].Value == null ? "" : row.Cells["chk_use"].Value.ToString();
                tray.list_pos[dgv_tray_pos.Rows.IndexOf(row)].isDisable = !(str.Length > 0 ? Convert.ToBoolean(str) : true);
                if (bgetpos)
                {
                    Product.Tray.PosInf posInf = new Product.Tray.PosInf();                  
                    posInf.idx = int.Parse(row.Cells["编号"].Value.ToString());
                    posInf.Pos[id].x = double.Parse(row.Cells["X"].Value.ToString());
                    posInf.Pos[id].y = double.Parse(row.Cells["Y"].Value.ToString());
                    posInf.Pos[id].z = double.Parse(row.Cells["Z"].Value.ToString());
                    posInf.Pos[id].a = double.Parse(row.Cells["U"].Value.ToString());
                }
            }
        }

        void GetAddQrcodeCapPosData(Product.Tray tray)
        {
            int id = 0;
            if (ctb_udload_cfg.SelectedIndex > 1 || ctb_udload_cfg.SelectedIndex < 0)
            {
                ctb_udload_cfg.SelectedIndex = 0;
            }
            id = ctb_udload_cfg.SelectedIndex;
            tray.col = (int)nud_tray_col.Value;
            tray.row = (int)nud_tray_row.Value;
            if (rbtn_tray_tl.Checked) tray.AddQrcodeTL[id] = new ST_XYZA((double)nud_AddCapQrcodePosX.Value, (double)nud_AddCapQrcodePosY.Value, 0, (double)nud_AddCapQrcodePosTX.Value);
            else if (rbtn_tray_bl.Checked) tray.AddQrcodeBL[id] = new ST_XYZA((double)nud_AddCapQrcodePosX.Value, (double)nud_AddCapQrcodePosY.Value, 0, (double)nud_AddCapQrcodePosTX.Value);
            else if (rbtn_tray_tr.Checked) tray.AddQrcodeTR[id] = new ST_XYZA((double)nud_AddCapQrcodePosX.Value, (double)nud_AddCapQrcodePosY.Value, 0, (double)nud_AddCapQrcodePosTX.Value);
        }

        void GetTrayBoxData(TrayBox traybox)
        {
            traybox.st_first_tray_pos.z=(double)nud_frist_tray_zpos.Value;
            traybox.tray_heigh=(double)nud_tray_heigh.Value;
            traybox.tray_feed_ofs_h =(double)nud_tray_feed_ofs_h.Value;
            traybox.fd_pos_x=(double)nud_tray_feed_x.Value;
            traybox.fd_safe_x = (double)nud_tray_feed_safe_x.Value;
            traybox.fd_chk_high_z = (double)nud_tray_Chk_High_z.Value;
            traybox.fd_openCy_pos_x = (double) nud_tray_openCy_x.Value;
            traybox.tray_cnt = (int)nud_tray_cnt.Value;
            COM.traybox_fd.tray_barcode_x1 = (double)nud_tray_barcode_x1.Value;
            COM.traybox_fd.tray_barcode_y1 = (double)nud_tray_barcode_y1.Value;
            COM.traybox_fd.tray_barcode_x2 = (double)nud_tray_barcode_x2.Value;
            COM.traybox_fd.tray_barcode_y2 = (double)nud_tray_barcode_y2.Value;
            COM.traybox_fd.tray_barcode_a1 = (double)nud_tray_barcode_a1.Value;
            COM.traybox_fd.tray_barcode_a2 = (double)nud_tray_barcode_a2.Value;
            COM.traybox_fd.motor_barcode_pos[0] = (double)nud_motor_y1.Value;
            COM.traybox_fd.motor_barcode_pos[1] = (double)nud_motor_y2.Value;
            COM.traybox_fd.motor_barcode_pos[2] = (double)nud_motor_y3.Value;
            COM.traybox_fd.motor_barcode_pos[3] = (double)nud_motor_y4.Value;
        }



        private void ctb_lightbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(((CTabControl)sender).SelectedTab.Name)
            {
                default:
                case "tp_lightbox_left":
                    lightBoxDef.lightbox = COM.LeftLightBox;
                    lightBoxDef.UpdateShow(distance:PT_SET.AFC_distance_check_open,lux:PT_SET.AFC_luxcct_check_open);
                    break;
                case "tp_lightbox_right":
                    lightBoxDef.lightbox = COM.RightLightBox;
                    lightBoxDef.UpdateShow(distance: PT_SET.DCC_distance_check_open, lux: PT_SET.DCC_luxcct_check_open);
                    break;
                case "tp_lightbox_otp":
                    lightBoxDef.lightbox = COM.OTPLightBox;
                    lightBoxDef.UpdateShow(distance: PT_SET.OTP_distance_check_open, lux: PT_SET.OTP_luxcct_check_open);
                    break;
            }
        }

        private void ctb_gz_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (((CTabControl)sender).SelectedTab.Name)
            {
                default:
                case "tp_gz1":
                    ws_status.ws = COM.ws1;
                    break;
                case "tp_gz2":
                    ws_status.ws = COM.ws2;
                    break;
                case "tp_gz3":
                    ws_status.ws = COM.ws3;
                    break;
                case "tp_gz4":
                    ws_status.ws = COM.ws4;
                    break;
            }
            wsModuelDef.ws = ws_status.ws;
            wsModuelDef.UpdateShow();
            wsModuelDef.ShowOtherData();
            ws_status.UpdateShow();
            //if(ws_status.ws!=null && ws_status.ws.ax_u!=null)
            //    btn_free.Text = ws_status.ws.ax_u.CurretOn ? "旋转励磁关" : "旋转励磁开";
        }

        private void btn_zp_cw_Click(object sender, EventArgs e)
        {
            try
            {
                if (COM.bhomeing)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese?string.Format("系统回零中,转盘不能动!"): "When the system is returning to zero, the turntable cannot move!        (系统回零中,转盘不能动!)");
                    return;
                }
                WS unsafeWs = COM.list_ws.Find(delegate (WS x) { return x != null && !x.isInTestPos; });
                if (unsafeWs != null)
                {
                    MessageBox.Show(VAR.IsChinese ? string.Format("{0}不在测试位，禁止手动旋转转盘!", unsafeWs.disc) : string.Format("{0} is not in test position, manual turntable rotation is forbidden!\r\n{1}不在测试位，禁止手动旋转转盘!", unsafeWs.disc, unsafeWs.disc), VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                btn_zp_ccw.Enabled = false;
                btn_zp_cw.Enabled = false;
                VAR.gsys_set.bquit = false;
                EM_RES res = Turntable.MoveToPre(ref VAR.gsys_set.bquit);
                if (res != EM_RES.OK)
                {
                    MessageBox.Show(VAR.IsChinese?"转盘定位失败!": "Turntable positioning failed!\r\n转盘定位失败!", VAR.IsChinese?"提示": "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    MessageBox.Show(VAR.IsChinese ? "转盘定位成功!": "Turntable positioning successful!\r\n转盘定位成功!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            finally
            {
                btn_zp_ccw.Enabled = true;
                btn_zp_cw.Enabled = true;
            }
          
        }

        //private void btn_free_Click(object sender, EventArgs e)
        //{
        //    if (ws_status.ws == null) return;
        //    if (ws_status.ws.isInTestPos)
        //    {
        //        if (DialogResult.Yes != MessageBox.Show(string.Format("{0}不在测试位，旋转励磁【关】可能反方向掉落！\r\n确定要【关】旋转励磁?", ws_status.ws.disc), "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) return;
        //    }
        //    else
        //    {
        //        if (DialogResult.Yes != MessageBox.Show(string.Format("{0}确定要【关】旋转励磁?", ws_status.ws.disc), "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) return;
        //    }

        //    ws_status.ws.ax_u.CurretOn = !ws_status.ws.ax_u.CurretOn;
        //    ((Button)sender).Text = ws_status.ws.ax_u.CurretOn ? "旋转励磁关" : "旋转励磁开";
        //}
        private void btn_free_Click(object sender, EventArgs e)
        {
            if (ws_status.ws == null) return;
            if (ws_status.ws.isInTestPos)
            {
                if (DialogResult.Yes != MessageBox.Show(VAR.IsChinese?string.Format("{0}不在测试位，旋转励磁【关】可能反方向掉落！\r\n确定要【关】旋转励磁?", ws_status.ws.disc): string.Format("{0} is not in the test position, the rotary excitation [OFF] may drop in the opposite direction! \r\nAre you sure to [OFF] spin excitation?\r\n{0}不在测试位，旋转励磁【关】可能反方向掉落！\r\n确定要【关】旋转励磁?", ws_status.ws.disc), VAR.IsChinese?"提示":"Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) return;
            }
            else
            {
                if (DialogResult.Yes != MessageBox.Show(VAR.IsChinese ? string.Format("{0}确定要【关】旋转励磁?", ws_status.ws.disc): string.Format("{0} Are you sure you want to [OFF] spin excitation?\r\n{0}确定要【关】旋转励磁?", ws_status.ws.disc), VAR.IsChinese?"提示":"Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) return;
            }

            ws_status.ws.ax_u.CurretOn = !ws_status.ws.ax_u.CurretOn;            
            ((Button)sender).Text = ws_status.ws.ax_u.CurretOn?"旋转励磁关": "旋转励磁开";
        }

        private void ctb_product_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (((CTabControl)sender).SelectedTab.Name)
            {

                default:
                case "tp_ws":
                    //if (ws_status.ws != null && ws_status.ws.ax_u != null)
                    //    btn_free.Text = ws_status.ws.ax_u.CurretOn ? "旋转励磁关" : "旋转励磁开";

                    ctb_gz_SelectedIndexChanged(ctb_gz, null);
                    break;
                case "tp_tray":
                    ShowTrayData(tray);
                    ShowTrayBoxData(traybox);
                    break;
            }
            lightBoxDef.UpdateShow();
        }

        private void tmr_update_Tick(object sender, EventArgs e)
        {
            switch (ctb_product.SelectedTab.Name)
            {
                default:
                case "tp_ws":
                    ws_status.UpdateShow();
                    int idx = (int)Turntable.GetCurSta;
                    cmb_tt_pos.SelectedIndex = idx>=0 && idx < cmb_tt_pos.Items.Count?idx:0;
                    break;
                case "tp_light_box":
                    LightBox.PosDef posdef = lightBoxDef.lightbox.CurPosDef;
                    lb_cur_pos_idx.Text = VAR.IsChinese?string.Format("当前位置:{0}", posdef != null ? posdef.ID.ToString() : "未知"): string.Format("Cur Pos:{0}", posdef != null ? posdef.ID.ToString() : "Unknown");
                    lb_cur_pos.Text = lightBoxDef.lightbox.StrOfPos;
                    break;
            }
            ctb_tray_cfg.Refresh();
            //lightBoxDef.light_btn_del.Text = VAR.IsChinese ? "删除" : "Delete";
            //lightBoxDef.light_btn_add.Text = VAR.IsChinese ? "增加" : "Add";
            //lightBoxDef.light_btn_load.Text = VAR.IsChinese ? "加载" : "Load";
            //lightBoxDef.light_btn_save.Text = VAR.IsChinese ? "保存" : "Save";
            //lightBoxDef.light_btn_stop.Text = VAR.IsChinese ? "停止" : "Stop";
            //cogDisplayer_product.btn_img.Text = VAR.IsChinese ? "图片" : "Img";
            //cogDisplayer_product.btn_live.Text = VAR.IsChinese ? "实况" : "Live";
            //cogDisplayer_product.btn_triger.Text = VAR.IsChinese ? "触发" : "Trigger";
            wsModuelDef.Changecolumn();
            lightBoxDef.ChangeColumn();
            btn_get.Text = VAR.IsChinese ? "学习" : "Study";
            btn_goto.Text = VAR.IsChinese ? "定位" : "Move";
            tmr_update.Interval = 500;
        }

        private void btn_zp_ccw_Click(object sender, EventArgs e)
        {
            try
            {
                if (COM.bhomeing)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("系统回零中,转盘不能动!") : "When the system is returning to zero, the turntable cannot move!        (系统回零中,转盘不能动!)");
                    return;
                }
                WS unsafeWs = COM.list_ws.Find(delegate (WS x) { return x != null && !x.isInTestPos; });
                if (unsafeWs != null)
                {
                    MessageBox.Show(VAR.IsChinese ? string.Format("{0}不在测试位，禁止手动旋转转盘!", unsafeWs.disc) : string.Format("{0} is not in test position, manual turntable rotation is forbidden!\r\n{1}不在测试位，禁止手动旋转转盘!", unsafeWs.disc, unsafeWs.disc), VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                btn_zp_ccw.Enabled = false;
                btn_zp_cw.Enabled = false;
                VAR.gsys_set.bquit = false;
                EM_RES res = Turntable.MoveToNext(ref VAR.gsys_set.bquit);
                if (res != EM_RES.OK) MessageBox.Show(VAR.IsChinese?"转盘定位失败!": "Turntable positioning failed!", VAR.IsChinese?"提示":"Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else MessageBox.Show(VAR.IsChinese?"转盘定位成功!": "Turntable positioning successful!\r\n转盘定位成功!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                btn_zp_cw.Enabled = true;
                btn_zp_ccw.Enabled = true;
            }
           
        }

        public void GetPos(bool IsChange = false, bool buofs = true)
        {
            VAR.gsys_set.bquit = false;
            ST_XYZA pos = new ST_XYZA();
            ST_XY pos_temp = new ST_XY();
            ST_XY Pickpos = new ST_XY();
            int id = 0;
            if (ctb_udload_cfg.SelectedIndex > 1 || ctb_udload_cfg.SelectedIndex < 0)
            {
                ctb_udload_cfg.SelectedIndex = 0;
            }
            id = ctb_udload_cfg.SelectedIndex;
            DialogResult result = MessageBox.Show(VAR.IsChinese?string.Format("位置学习说明:\n 1.按'是'键学习轴的当前位置(如是OK与NG料盘位置为当前位置-吸头偏差)。\n 2.按'否'键通{0}的位置计算得到。\n 3.按'取消'键退出位置学习", COM.List_UDLoad[(id + 1) % 2].disc): string.Format("Position learning instructions: \n 1. Press 'Yes' to learn the current position of the axis (if tray is OK and NG tray, position is the current position-tip deviation). \n 2.Press 'No' key to calculate the position of {0}. \n 3.Press the 'Cancel' key to exit position learning\r\n位置学习说明:\n 1.按'是'键学习轴的当前位置(如是OK与NG料盘位置为当前位置-吸头偏差)。\n 2.按'否'键通{0}的位置计算得到。\n 3.按'取消'键退出位置学习", COM.List_UDLoad[(id + 1) % 2].disc), VAR.IsChinese?"提示":"Prompt", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                TrayCfgGetPos(ref pos, buofs);

            }
            else if (result == DialogResult.No)
            {
                if (!IsChange)
                {
                    if (rbtn_tray_tl.Checked) pos = tray.tl[(id + 1) % 2];
                    else if (rbtn_tray_bl.Checked) pos = tray.bl[(id + 1) % 2];
                    else if (rbtn_tray_tr.Checked) pos = tray.tr[(id + 1) % 2];
                    //pos = COM.List_UDLoad[(id + 1) % 2];
                    if (ctb_tray_cfg.SelectedIndex!=0)
                    {
                        Pickpos = COM.List_UDLoad[(id + 1) % 2].list_xt[0].XtCamPos(pos.ToXY());
                        pos_temp = Pickpos - COM.List_UDLoad[(id + 1) % 2].list_xt[0].st_pos_samevs[(id + 1) % 2] +
                                   COM.List_UDLoad[id].list_xt[0].st_pos_samevs[id];
                        Pickpos = COM.List_UDLoad[id].list_xt[0].XtCalPos(pos_temp);
                    }
                    else
                    {
                        Pickpos = pos.ToXY() - COM.List_UDLoad[(id + 1) % 2].list_xt[0].st_pos_samevs[(id + 1) % 2] +
                                 COM.List_UDLoad[id].list_xt[0].st_pos_samevs[id];
                    }
                    pos.x = Pickpos.x;
                    pos.y = Pickpos.y;
                }
                else
                {
                    Pickpos = tray.TrayVsPos[(id + 1) % 2].ToXY();
                    pos_temp = Pickpos - COM.List_UDLoad[(id + 1) % 2].list_xt[0].st_pos_samevs[(id + 1) % 2] +
                              COM.List_UDLoad[id].list_xt[0].st_pos_samevs[id];
                    pos.x = pos_temp.x;
                    pos.y = pos_temp.y;
                    pos.a = tray.TrayVsPos[(id + 1) % 2].z;
                }
                
            }
            else return;
            if(IsChange)  
            {
                pos.z = pos.a;
                pos.a = 0;
                xyzu_trayvs.Value = pos;
            }
            else xyzu_tray_study.Value = pos;
          //  ShowTrayData(tray);
        }

        private void btn_tray_get_Click(object sender, EventArgs e)
        {
            GetPos();
            DialogResult result = MessageBox.Show(VAR.IsChinese ? "是否保存数据?" : "Do you want to save the data?\r\n是否保存数据?", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                btn_tray_save_Click(null, null);
            }
        }

        private void btn_tray_make_array_Click(object sender, EventArgs e)
        {
            GetTrayData(tray,false);
            tray.CreatePos();
            ShowTrayData(tray);
            btn_tray_save_Click(null, null);
        }

        private void btn_tray_save_Click(object sender, EventArgs e)
        {
            string trayname = string.Empty;
            GetTrayData(tray, true);
            tray.SaveDat();
            tray.LoadDat();
            ShowTrayData(tray);
            if (ctb_tray_cfg.SelectedIndex == 0) trayname = VAR.IsChinese?"供料轴":"Feed tray";
            else if (ctb_tray_cfg.SelectedIndex == 1) trayname = VAR.IsChinese ? "OK料轴":"Ok tray";
            else if (ctb_tray_cfg.SelectedIndex == 2) trayname = VAR.IsChinese ? "NG料轴":"Ng tray";
            if (DialogResult.No == MessageBox.Show(VAR.IsChinese?string.Format("保存成功!\r\n注:如需修改的参数立即生效,请按'是'键更换{0},并拿掉{1}上的料盘!",traybox.disc, trayname): string.Format("Saved successfully! \r\n Note: If the parameters to be modified take effect immediately, press the 'Yes' button to replace {0}, and remove the tray on {2}!\r\n保存成功!\r\n注:如需修改的参数立即生效,请按'是'键更换{1},并拿掉{2}上的料盘!", traybox.name, traybox.disc, trayname), VAR.IsChinese?"提示": "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Information)) return;
            EM_RES res = traybox.NewBox();
            if (res == EM_RES.OK) traybox.IsReady = true;
            traybox.tray_idx = 0;
            traybox.tray_cur = null;
        }

        private void xyzu_tray_study_DataChanged(object sender, EventArgs e)
        {
            if (bPosUpdate) return;
            GetTrayData(tray,false);
        }

        private void rbtn_tray_tl_CheckedChanged(object sender, EventArgs e)
        {
            if(((RadioButton)sender).Checked)
                ShowTrayData(tray);
        }

        private void dgv_tray_pos_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if ((e.RowIndex & 1) == 1) e.CellStyle.BackColor = Color.WhiteSmoke;
        }

        private void ctb_tray_cfg_SelectedIndexChanged(object sender, EventArgs e)
        {
            TrayCfgBinding binding = GetCurrentTrayCfgBinding();
            tray = binding.Tray;
            traybox = binding.TrayBox;
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, string.Format("TrayCfg页签={0}, 绑定料仓={1}, 列={2}, 行={3}",
                ((CTabControl)sender).SelectedTab.Name, traybox.disc, tray.col, tray.row));
            bool isFeedTray = ((CTabControl)sender).SelectedTab.Name == "ctp_tray_fd";
            nud_AddCapQrcodePosY.Enabled = isFeedTray;
            btn_GetAddCapQrcodePos.Enabled = isFeedTray;
            btn_GotoAddCapQrcodePos.Enabled = isFeedTray;
            btn_MakeAddCapQrcodePosArray.Enabled = isFeedTray;
            btn_SaveAddCapQrcodePos.Enabled = isFeedTray;
            ShowTrayData(tray);
            ShowTrayBoxData(traybox);
        }

        private void btn_tray_copy_Click(object sender, EventArgs e)
        {
            ST_XYZA pos = new ST_XYZA();
            ST_XY pos_temp = new ST_XY();
            ST_XY Pickpos = new ST_XY();
            int id = 0;
            if (ctb_udload_cfg.SelectedIndex > 1 || ctb_udload_cfg.SelectedIndex < 0)
            {
                ctb_udload_cfg.SelectedIndex = 0;
            }
            id = ctb_udload_cfg.SelectedIndex;
            DialogResult result = MessageBox.Show(VAR.IsChinese?string.Format("位置学习说明:\n 1.按'是'键以左上角位置学习供料仓上下料1位置间隔。\n 2.按'否'键通过{0}的位置计算得到。\n 3.按'取消'键退出位置学习", COM.List_UDLoad[(id + 1) % 2].disc): string.Format("Instructions for position learning: \n 1. Press the 'Yes' key to learn the position interval by feed tray of updownload 1. \n 2.Press 'No' key to calculate the position of {0}. \n 3.Press the 'Cancel' key to exit position learning \r\n位置学习说明:\n 1.按'是'键以左上角位置学习供料仓上下料1位置间隔。\n 2.按'否'键通过{1}的位置计算得到。\n 3.按'取消'键退出位置学习", COM.List_UDLoad[(id + 1) % 2].englishdisc,COM.List_UDLoad[(id + 1) % 2].disc), VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                tray.bl[id].x = tray.tl[id].x + tray_fd.bl[0].x - tray_fd.tl[0].x;
                tray.bl[id].y = tray.tl[id].y + tray_fd.bl[0].y - tray_fd.tl[0].y;
                tray.bl[id].a = tray.tl[id].a + tray_fd.bl[0].a - tray_fd.tl[0].a;
                tray.tr[id].x = tray.tl[id].x + tray_fd.tr[0].x - tray_fd.tl[0].x;
                tray.tr[id].y = tray.tl[id].y + tray_fd.tr[0].y - tray_fd.tl[0].y;
                tray.tr[id].a = tray.tl[id].a + tray_fd.tr[0].a - tray_fd.tl[0].a;
              
            }
            else if (result == DialogResult.No)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (i==0) pos = tray.tl[(id + 1) % 2];
                    else if (i == 1) pos = tray.bl[(id + 1) % 2];
                    else if (i == 2) pos = tray.tr[(id + 1) % 2];
                    //pos = COM.List_UDLoad[(id + 1) % 2];
                    if (ctb_tray_cfg.SelectedIndex != 0)
                    {
                        Pickpos = COM.List_UDLoad[(id + 1) % 2].list_xt[0].XtCamPos(pos.ToXY());
                        pos_temp = Pickpos - COM.List_UDLoad[(id + 1) % 2].list_xt[0].st_pos_samevs[(id + 1) % 2] +
                                   COM.List_UDLoad[id].list_xt[0].st_pos_samevs[id];
                        Pickpos = COM.List_UDLoad[id].list_xt[0].XtCalPos(pos_temp);
                    }
                    else
                    {
                        Pickpos = pos.ToXY() - COM.List_UDLoad[(id + 1) % 2].list_xt[0].st_pos_samevs[(id + 1) % 2] +
                                  COM.List_UDLoad[id].list_xt[0].st_pos_samevs[id];
                    }

                    if (i == 0)
                    {
                        tray.tl[id].x = Pickpos.x;
                        tray.tl[id].y = Pickpos.y;
                        tray.tl[id].a = tray.tl[(id + 1) % 2].a;
                    }
                    else if (i == 1)
                    {
                        tray.bl[id].x = Pickpos.x;
                        tray.bl[id].y = Pickpos.y;
                        tray.bl[id].a = tray.bl[(id + 1) % 2].a;
                    }
                    else if (i == 2)
                    {
                        tray.tr[id].x = Pickpos.x;
                        tray.tr[id].y = Pickpos.y;
                        tray.tr[id].a = tray.tr[(id + 1) % 2].a;
                    }
                }
               
            }
            else return;
            tray.SaveDat();
            tray.LoadDat();
            ShowTrayData(tray);

        }

        EM_RES TrayCfgGotoPos(ST_XYZA st_pos,bool buofs=true)
        {
            if (DialogResult.No == MessageBox.Show(string.Format("确认是否定位到当前坐标X:{0},Y:{1},X_TR:{2}。\n 注;如果是OK与NG料盘定位为定位坐标＋吸头偏差。", st_pos.x, st_pos.y, st_pos.a), VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return EM_RES.ABORT;
            int id = 0;
            bool gocap = false;
            if (ctb_udload_cfg.SelectedIndex > 1 || ctb_udload_cfg.SelectedIndex < 0)
            {
                ctb_udload_cfg.SelectedIndex = 0;
            }
            id = ctb_udload_cfg.SelectedIndex;
            AXIS ax_x = null;
            AXIS ax_y = null;
            AXIS ax_z = null;
            AXIS ax_u = null;
            ax_x = COM.List_UDLoad[id].ax_x;
            ax_y = COM.List_UDLoad[id].ax_y;
            ax_z = COM.List_UDLoad[id].ax_z;
            TrayCfgBinding binding = GetCurrentTrayCfgBinding();
            tray = binding.Tray;
            ax_u = binding.TrayBox.ax_x;
            gocap = ctb_tray_cfg.SelectedTab.Name == "ctp_tray_fd";
            //EM_RES res = MT.Move(ref VAR.gsys_set.bquit, ref ax_z,0);
            //if (res != EM_RES.OK)
            //{
            //    //MessageBox.Show("定位失败!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return res;
            //}
            if (ctb_tray_cfg.SelectedIndex != 0 && buofs)
            {
                st_pos.x = st_pos.x + COM.List_UDLoad[id].list_xt[0].st_vs_pos_xtshp.x;
                st_pos.y = st_pos.y + COM.List_UDLoad[id].list_xt[0].st_vs_pos_xtshp.y;
            }
                EM_RES res = MT.ZupMove(ref VAR.gsys_set.bquit, ref ax_x, st_pos.x, ref ax_y, st_pos.y, ref ax_u, st_pos.a,true);
            if (res != EM_RES.OK)
            {
                //MessageBox.Show("定位失败!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return res;
            }

            if (gocap)
            {
                ctb_product.SelectedIndex = 2;
                ctb_cali.SelectedIndex = 0;
                Thread.Sleep(300);
                COM.List_UDLoad[id].upcam.FindTaskTriAndWait(CONST.ModUpFw, false);
            }
            return EM_RES.OK;
        }

        EM_RES GotoAddCapQrcodePos(ST_XYZA st_pos)
        {
            if (DialogResult.No == MessageBox.Show(VAR.IsChinese ? string.Format("确认是否定位到当前坐标X:{0},Y:{1},X_TR:{2}。\n ", st_pos.x, st_pos.y, st_pos.a) : string.Format("Confirm whether to locate to the current coordinatesX:{0},Y:{1},X_TR:{2}。\nNote: if it is OK or NG tray ,positioning = positioning coordinate + tip deviation\r\n确认是否定位到当前坐标X:{0},Y:{1},X_TR:{2}。\n 注;如果是OK与NG料盘定位为定位坐标＋吸头偏差。", st_pos.x, st_pos.y, st_pos.a), VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return EM_RES.ABORT;
            int id = 0;
            bool gocap = false;
            if (ctb_udload_cfg.SelectedIndex > 1 || ctb_udload_cfg.SelectedIndex < 0)
            {
                ctb_udload_cfg.SelectedIndex = 0;
            }
            id = ctb_udload_cfg.SelectedIndex;
            AXIS ax_x = null;
            AXIS ax_y = null;
            AXIS ax_z = null;
            AXIS ax_u = null;
            ax_x = COM.List_UDLoad[id].ax_x;
            ax_y = COM.List_UDLoad[id].ax_y;
            ax_z = COM.List_UDLoad[id].ax_z;
            TrayCfgBinding binding = GetCurrentTrayCfgBinding();
            tray = binding.Tray;
            ax_u = binding.TrayBox.ax_x;
            gocap = ctb_tray_cfg.SelectedTab.Name == "ctp_tray_fd";
            EM_RES res = MT.ZupMove(ref VAR.gsys_set.bquit, ref ax_x, st_pos.x, ref ax_y, st_pos.y, ref ax_u, st_pos.a, true);
            if (res != EM_RES.OK)
            {
                MessageBox.Show("定位失败!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return res;
            }

            if (gocap)
            {
                ctb_product.SelectedIndex = 2;
                ctb_cali.SelectedIndex = 0;
                Thread.Sleep(300);
                COM.List_UDLoad[id].upcam.FindTaskTriAndWait(CONST.FindQrCodeFw, false);
            }
            return EM_RES.OK;
        }

        EM_RES TrayCfgGetPos(ref ST_XYZA st_pos,bool buofs=true)
        {
            int id = 0;
            if (ctb_udload_cfg.SelectedIndex > 1 || ctb_udload_cfg.SelectedIndex < 0)
            {
                ctb_udload_cfg.SelectedIndex = 0;
            }
            id = ctb_udload_cfg.SelectedIndex;
            AXIS ax_x = null;
            AXIS ax_y = null;
            AXIS ax_z = null;
            AXIS ax_u = null;
            ax_x = COM.List_UDLoad[id].ax_x;
            ax_y = COM.List_UDLoad[id].ax_y;
            //ax_z = COM.List_UDLoad[id].ax_z;
            TrayCfgBinding binding = GetCurrentTrayCfgBinding();
            tray = binding.Tray;
            ax_u = binding.TrayBox.ax_x;
            st_pos.x = ax_x != null ? ax_x.fenc_pos : 0;
            st_pos.y = ax_y != null ? ax_y.fenc_pos : 0;
            st_pos.z = ax_z != null ? ax_z.fenc_pos : 0;
            st_pos.a = ax_u != null ? ax_u.fenc_pos : 0;
            if (ctb_tray_cfg.SelectedIndex != 0 && buofs)
            {
                st_pos.x = st_pos.x - COM.List_UDLoad[id].list_xt[0].st_vs_pos_xtshp.x;
                st_pos.y = st_pos.y - COM.List_UDLoad[id].list_xt[0].st_vs_pos_xtshp.y;
            }
            return EM_RES.OK;
        }

        private void btn_tray_goto_Click(object sender, EventArgs e)
        {
            EM_RES ret;
            VAR.gsys_set.bquit=false;
            ST_XYZA pos = new ST_XYZA();
            pos = xyzu_tray_study.Value;
            EM_RES res = TrayCfgGotoPos(pos);
            if (res != EM_RES.OK)
             MessageBox.Show(VAR.IsChinese?"定位失败!": "Positioning failed!\r\n定位失败!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void dgv_tray_pos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //定位
            if (e.ColumnIndex == 7)
            {
                try
                {
                    ST_XYZA pos = new ST_XYZA();
                    pos.x = double.Parse(dgv_tray_pos.Rows[e.RowIndex].Cells["X"].Value.ToString());
                    pos.y = double.Parse(dgv_tray_pos.Rows[e.RowIndex].Cells["Y"].Value.ToString());
                    pos.z = double.Parse(dgv_tray_pos.Rows[e.RowIndex].Cells["Z"].Value.ToString());
                    pos.a = double.Parse(dgv_tray_pos.Rows[e.RowIndex].Cells["U"].Value.ToString());
                    //if (DialogResult.No == MessageBox.Show(string.Format("确认是否定位到当前坐标X:{0},Y:{1},TR_X:{2}", pos.x, pos.y, pos.a), "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;
                    EM_RES res = TrayCfgGotoPos(pos);
                    if(res!=EM_RES.OK)
                    MessageBox.Show(VAR.IsChinese ? "定位失败!" : "Positioning failed!\r\n定位失败!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //学习位置
            else if (e.ColumnIndex == 6)
            {
                try
                {
                    if(!gettraydata)return;
                    ST_XYZA pos = new ST_XYZA();
                    TrayCfgGetPos(ref pos);
                    dgv_tray_pos.Rows[e.RowIndex].Cells["X"].Value = pos.x;
                    dgv_tray_pos.Rows[e.RowIndex].Cells["Y"].Value = pos.y;
                    //dgv_tray_pos.Rows[e.RowIndex].Cells["Z"].Value = posInf.z;
                    dgv_tray_pos.Rows[e.RowIndex].Cells["U"].Value = pos.a;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btn_traybox_save_Click(object sender, EventArgs e)
        {
            GetTrayBoxData(traybox);
            traybox.SaveCfg();
            traybox.LoadCfg();
            ShowTrayBoxData(traybox);
        }

        private void dgv_tray_pos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (tray_map.tray_dat != null && tray_map.tray_dat.list_pos != null)
            {
                tray_map.tray_dat.list_pos.Find(s =>
                {
                    s.md = null;
                    return false;
                });
                if (e.RowIndex >= 0 && e.RowIndex < tray_map.tray_dat.list_pos.Count)
                {
                    tray_map.tray_dat.list_pos[e.RowIndex].md = new Product.MdDat();
                    tray_map.tray_dat.list_pos[e.RowIndex].md.res = 1;
                }

                tray_map.UpdateShow();
            }
        }

        private void tray_map_CellClik(object sender, EventArgs e)
        {
            tray_map.tray_dat.list_pos.Find(s =>
            {
                s.md = null;
                return false;
            });
            ((tray.TrayClikEventArgs)e).PosInf.md = new Product.MdDat();
            ((tray.TrayClikEventArgs)e).PosInf.md.res = 1;
            int rowidx = ((tray.TrayClikEventArgs)e).PosInf.idx;
            if (rowidx >= 0 && rowidx < dgv_tray_pos.Rows.Count)
            {
                dgv_tray_pos.FirstDisplayedScrollingRowIndex = rowidx;
                dgv_tray_pos.Rows[rowidx].Selected = true;
            }

        }

        private void ctb_udload_cfg_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowTrayData(tray);
        }


        private void btn_dwup_Click(object sender, EventArgs e)
        {
            // UploadModle.ax_z.SetSpdRadio(0.5);
            VAR.gsys_set.bquit = false;
            ST_XYZA pos = new ST_XYZA();
            pos = xyzu_tray_study.Value;
            int id = 0;
            if (ctb_udload_cfg.SelectedIndex > 1 || ctb_udload_cfg.SelectedIndex < 0)
            {
                ctb_udload_cfg.SelectedIndex = 0;
            }
            id = ctb_udload_cfg.SelectedIndex;

            AXIS ax_z = null;
            ax_z = COM.List_UDLoad[id].ax_z;
            if (Math.Abs(ax_z.fenc_pos) > 1)
            {
                btn_dwup.Text = VAR.IsChinese?"下降":"Down";
                MT.Move(ref VAR.gsys_set.bquit, ref ax_z, 0);
            }
            else
            {
                btn_dwup.Text = VAR.IsChinese ? "抬起":"Up";
                MT.Move(ref VAR.gsys_set.bquit, ref ax_z, pos.z);
            }
        }

        private void btn_VsCalc_Click(object sender, EventArgs e)
        {
            VAR.gsys_set.bquit = false;
            ST_XYZA pos = new ST_XYZA();
            ST_XY Pickpos = new ST_XY();
              int id = 0;
            if (ctb_udload_cfg.SelectedIndex > 1 || ctb_udload_cfg.SelectedIndex < 0)
            {
                ctb_udload_cfg.SelectedIndex = 0;
            }
            id = ctb_udload_cfg.SelectedIndex;            
            pos = xyzu_tray_study.Value;
            Pickpos = COM.List_UDLoad[id].list_xt[0].XtCalPos(new ST_XY(COM.List_UDLoad[id].list_xt[0].ax_x.fenc_pos, COM.List_UDLoad[id].list_xt[0].ax_y.fenc_pos));
            pos.x = Pickpos.x;
            pos.y = Pickpos.y;
            xyzu_tray_study.Value=pos;
        }

    
        private void btn_Vsgopos_Click(object sender, EventArgs e)
        {
            VAR.gsys_set.bquit = false;
            ST_XYZA pos = new ST_XYZA();
            ST_XY Pickpos = new ST_XY();
              int id = 0;
            if (ctb_udload_cfg.SelectedIndex > 1 || ctb_udload_cfg.SelectedIndex < 0)
            {
                ctb_udload_cfg.SelectedIndex = 0;
            }
            id = ctb_udload_cfg.SelectedIndex;            
            pos = xyzu_tray_study.Value;
            Pickpos = COM.List_UDLoad[id].list_xt[0].XtCamPos(pos.ToXY());
            pos.x = Pickpos.x;
            pos.y = Pickpos.y;
            EM_RES res = TrayCfgGotoPos(pos,false);
            if (res == EM_RES.ABORT) return;
            if (res != EM_RES.OK ) MessageBox.Show(VAR.IsChinese?"定位失败!": "Positioning failed!\r\n定位失败!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                ctb_product.SelectedIndex = 2;
                ctb_cali.SelectedIndex = 0;
                Thread.Sleep(300);
                COM.List_UDLoad[id].upcam.FindTaskTriAndWait(CONST.ModUpFw, false);
            }
        }

        private void btn_vs_calpos_Click(object sender, EventArgs e)
        {
            VAR.gsys_set.bquit = false;
            ST_XY Pickpos = new ST_XY();
            WS.MdDat md_dat=new WS.MdDat();
            bool ret=false;
            int id = 0;
            DialogResult result = MessageBox.Show(VAR.IsChinese?"请选择要计算的模块:\n 1.按'是'键选择上下料模块<1>.\n 2.按'否'键选择上下料模块<2>.\n 3.按'取消'键退出.": "Please select the module to be calculated: \n 1. Press 'Yes' to select the updownload 1. \n 2. Press 'No' to select the updownload 2. \n 3. Press 'Cancel' drop out.\r\n请选择要计算的模块：\n 1.按'是'键选择上下料模块<1>。\n 2.按'否'键选择上下料模块<2>。\n 3.按'取消'键 退出。", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Yes) id = 0;
            else if (result == DialogResult.No) id = 1;
            else return;
            Pickpos = COM.List_UDLoad[id].list_xt[0].XtCalPos(new ST_XY(COM.List_UDLoad[id].list_xt[0].ax_x.fenc_pos, COM.List_UDLoad[id].list_xt[0].ax_y.fenc_pos));
            nud_cal_x.Value = (decimal) Pickpos.x;
            nud_cal_y.Value = (decimal) Pickpos.y;
            result = MessageBox.Show(VAR.IsChinese?"是否加载数据并保存?": "Do you want to load and save the data\r\n是否加载数据并保存?", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if(result==DialogResult.No)return;
            wsModuelDef.rbtn_pickpos.Checked = true;
            wsModuelDef.rbtn_campos.Checked = false;
            Thread.Sleep(30);
            ret=!PT_SET.bNonparallel?wsModuelDef.GetRowData(0, ref md_dat): wsModuelDef.GetRowData(1, ref md_dat);
            if(!ret)return;
            md_dat.st_pickpos[id].x = Pickpos.x;
            md_dat.st_pickpos[id].y = Pickpos.y;
            ret = !PT_SET.bNonparallel ? wsModuelDef.SetRowData(0, md_dat): wsModuelDef.SetRowData(1, md_dat);
            if (!ret) return;
            wsModuelDef.btn_save.PerformClick();
            result = MessageBox.Show(VAR.IsChinese?"是否进行阵列?": "Do you perform the array?\r\n是否进行阵列?", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.No) return;
            Thread.Sleep(30);
            wsModuelDef.btn_array.PerformClick();
        }

        private void btn_tray_vspos_get_Click(object sender, EventArgs e)
        {
           GetPos(true,false);
        }

        private void btn_tray_vspos_goto_Click(object sender, EventArgs e)
        {
            VAR.gsys_set.bquit = false;
            Button btn = (Button) sender;
            
            ST_XYZA pos = new ST_XYZA();
            pos = xyzu_trayvs.Value;
            pos.a = pos.z;
            pos.z = 0;
            EM_RES res = TrayCfgGotoPos(pos,btn.Name == "btn_tray_vspos_goto" ? false:true );
            if (res != EM_RES.OK)
                MessageBox.Show(VAR.IsChinese?"定位失败!": "Positioning failed!\r\n定位失败!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btn_tray_vspos_gocenter_Click(object sender, EventArgs e)
        {
            EM_RES res;
            VAR.gsys_set.bquit = false;
            int id = 0;
            ST_XYZA pos = new ST_XYZA();
            ST_XYZA pos_temp = new ST_XYZA();
            if (ctb_udload_cfg.SelectedIndex > 1 || ctb_udload_cfg.SelectedIndex < 0)
            {
                ctb_udload_cfg.SelectedIndex = 0;
            }
            id = ctb_udload_cfg.SelectedIndex;    
            //定位
            pos = xyzu_trayvs.Value;
            pos.a = pos.z;
            pos.z = 0;
            res = TrayCfgGotoPos(pos);
            if (res != EM_RES.OK)
            {
                MessageBox.Show(VAR.IsChinese ? "定位失败!" : "Positioning failed!\r\n定位失败!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } 
            Cam cam_up= COM.List_UDLoad[id].upcam;
            cam_up.List_vs_task_cur.Clear();
            cam_up.inputImageCnt = 0;
            Cam.VisionTask VsTask = cam_up.List_vs_task.Find(s => s.TaskName.Equals(CONST.TrayUpFw));
            cam_up.List_vs_task_cur.Add(VsTask);
            pos_temp = pos;
            res = cam_up.MoveToImgCenter(ref VAR.gsys_set.bquit, ref pos_temp, VsTask, cam_up.ListCaliTool);
          
            if (res != EM_RES.OK) return;
            pos.x = pos_temp.x;
            pos.y = pos_temp.y;
            pos.z = pos.a;
            pos.a = 0;
            xyzu_trayvs.Value = pos;
            MessageBox.Show(VAR.IsChinese?"对中成功!": "Centered successfully!\r\n对中成功!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btn_ud1_hightest_Click(object sender, EventArgs e)
        {
            try
            {
                btn_ud1_hightest.Enabled = false;
                btn_ud2_hightest.Enabled = false;
                btn_ud_hightest.Enabled = false;
                int id = 0;
                double zref = 0;
                DialogResult result = MessageBox.Show(VAR.IsChinese?"选择当前位置测高的吸头?\n 1.按'是'键吸头1测高。\n 2.按'否'键吸头2测高。\n 3.按‘取消’退出。": "Select the tip for height measurement at the current position? \n 1. Press the 'Yes' button to measure the height of tip 1. \n 2.Press the 'No' button to measure the height of tip 2. \n 3. Press 'Cancel' to exit.\r\n选择当前位置测高的吸头?\n 1.按'是'键吸头1测高。\n 2.按'否'键吸头2测高。\n 3.按‘取消’退出。", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (result == DialogResult.Yes) id = 0;
                else if (result == DialogResult.No) id = 1;
                else return;
                VAR.gsys_set.bquit = false;
                COM.UDLoad1.list_xt[id].CaliZrefWLByZK(ref VAR.gsys_set.bquit, new ST_XY(-10000, -10000), ref zref);
            }
            finally 
            {
                btn_ud1_hightest.Enabled = true;
                btn_ud2_hightest.Enabled = true;
                btn_ud_hightest.Enabled = true;
            }
          
        }

        private void btn_ud2_hightest_Click(object sender, EventArgs e)
        {
            try
            {
                btn_ud1_hightest.Enabled = false;
                btn_ud2_hightest.Enabled = false;
                btn_ud_hightest.Enabled = false;
                int id = 0;
                double zref = 0;
                DialogResult result = MessageBox.Show(VAR.IsChinese?"选择当前位置测高的吸头?\n 1.按'是'键吸头3测高。\n 2.按'否'键吸头4测高。\n 3.按‘取消’退出。": "Select the tip for height measurement at the current position? \n 1. Press the 'Yes' key to measure the height of tip 3. \n 2.Press 'No' button to measure height of tip 4. \n 3. Press 'Cancel' to exit.\r\n选择当前位置测高的吸头?\n 1.按'是'键吸头3测高。\n 2.按'否'键吸头4测高。\n 3.按‘取消’退出。", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (result == DialogResult.Yes) id = 0;
                else if (result == DialogResult.No) id = 1;
                else return;
                VAR.gsys_set.bquit = false;
                COM.UDLoad2.list_xt[id].CaliZrefWLByZK(ref VAR.gsys_set.bquit, new ST_XY(-10000, -10000), ref zref);
            }
            finally 
            {
                btn_ud1_hightest.Enabled = true;
                btn_ud2_hightest.Enabled = true;
                btn_ud_hightest.Enabled = true;
            }
           
        }

        private void btn_Pos_LiZhu_goto_Click(object sender, EventArgs e)
        {
            ST_XY pos = new ST_XY();
            WS ws = Turntable.GetWSOnFeedPos;
            if (ws == null)
            {
                MessageBox.Show(VAR.IsChinese?"没有找到当前上料位工站！": "No current loading station was found!\r\n没有找到当前上料位工站！");
                return;
            }

            if (ws.disc != ws_status.ws.disc)
            {
                MessageBox.Show(VAR.IsChinese ? string.Format("当前上料位工站为{0}，选择工站为{1},请选对工站或定位到所选工站！",ws.disc,ws_status.ws.disc): string.Format("The current loading station is {0},but the selected station is {1}, please choose the right station or locate the selected station!\r\n当前上料位工站为{0}，选择工站为{1},请选对工站或定位到所选工站！", ws.disc, ws_status.ws.disc));
                return;
            }
            VAR.gsys_set.bquit = false;
            pos.x = (double)nud_PosCapLZ_x.Value;
            pos.y = (double)nud_PosCapLZ_y.Value;
            MT.ZupMove(ref VAR.gsys_set.bquit, ref COM.UDLoad1.ax_x, pos.x, ref COM.UDLoad1.ax_y, pos.y);
            ctb_product.SelectedIndex = 2;
            ctb_cali.SelectedIndex = 0;
            Thread.Sleep(300);
            COM.UDLoad1.upcam.FindTaskTriAndWait(CONST.LiZhuFw, false);
        }

        private void btn_Pos_LiZhu_get_Click(object sender, EventArgs e)
        {
            EM_RES res;
            VAR.gsys_set.bquit = false;
            int id = 0;
            ST_XY pos = new ST_XY();
            ST_XYZA pos_temp = new ST_XYZA();
            WS ws = Turntable.GetWSOnFeedPos;
            if (ws == null)
            {
                MessageBox.Show(VAR.IsChinese?"没有找到当前上料位工站！": "No current loading station was found!\r\n没有找到当前上料位工站！");
                return;
            }

            if (ws.disc != ws_status.ws.disc)
            {
                MessageBox.Show(VAR.IsChinese ? string.Format("当前上料位工站为{0}，选择工站为{1},请选对工站或定位到所选工站！", ws.disc, ws_status.ws.disc) : string.Format("The current loading station is {0},but the selected station is {1}, please choose the right station or locate the selected station!\r\n当前上料位工站为{0}，选择工站为{1},请选对工站或定位到所选工站！", ws.disc, ws_status.ws.disc));
                return;
            }

            DialogResult result = MessageBox.Show(VAR.IsChinese?"学习方法说明:\n 1.按'是'键对中后学习坐标。\n 2.按'否'学习当前坐标!\n 3.按‘取消’退出。": "Description: \n 1. Press 'Yes' key to learn the coordinates after centering. \n 2. Press 'No' to learn the current coordinates! \n 3. Press 'Cancel' to exit.\r\n学习方法说明:\n 1.按'是'键对中后学习坐标。\n 2.按'否'学习当前坐标!\n 3.按‘取消’退出。", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if(result== DialogResult.Cancel)return;
            else if (result == DialogResult.Yes)
            {
                //定位
                pos.x = (double) nud_PosCapLZ_x.Value;
                pos.y = (double) nud_PosCapLZ_y.Value;
                res = MT.ZupMove(ref VAR.gsys_set.bquit, ref COM.UDLoad1.ax_x, pos.x, ref COM.UDLoad1.ax_y, pos.y);
                if (res != EM_RES.OK)
                {
                    MessageBox.Show(VAR.IsChinese?"定位失败!": "Positioning failed!\r\n定位失败!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Cam cam_up = COM.UDLoad1.upcam;
                cam_up.List_vs_task_cur.Clear();
                cam_up.inputImageCnt = 0;
                Cam.VisionTask VsTask = cam_up.List_vs_task.Find(s => s.TaskName.Equals(CONST.LiZhuFw));
                cam_up.List_vs_task_cur.Add(VsTask);
                pos_temp = pos.ToXYZA();
                res = cam_up.MoveToImgCenter(ref VAR.gsys_set.bquit, ref pos_temp, VsTask, cam_up.ListCaliTool);
                if (res != EM_RES.OK) return;
                nud_PosCapLZ_x.Value = (decimal) pos_temp.x;
                nud_PosCapLZ_y.Value = (decimal) pos_temp.y;
                MessageBox.Show(VAR.IsChinese?"对中学习成功!": "Center learning successfully!\r\n对中学习成功!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (result == DialogResult.No)
            {
                nud_PosCapLZ_x.Value = (decimal)COM.UDLoad1.ax_x.fenc_pos;
                nud_PosCapLZ_y.Value = (decimal)COM.UDLoad1.ax_y.fenc_pos;
            }
        }

        private void btn_new_Click(object sender, EventArgs e)
        {
            EM_RES ret;
            if (edt_new_product.Text.Length < 3)
            {
                MessageBox.Show(VAR.IsChinese?"新建产品名字不能少于3字符， " + edt_new_product.Text: "New product name must be at least 3 characters\r\n新建产品名字不能少于3字符\r\n"+ edt_new_product.Text, VAR.IsChinese?"警告":"Warning");
                return;
            }
            if (ltb_product.Text == "Calibration")
            {
                MessageBox.Show(VAR.IsChinese ? string.Format("不能以校正程序({0})建立产品", ltb_product.Text): string.Format("Cannot build product with ({0}) as template\r\n不能以校正程序({0})建立产品", ltb_product.Text), VAR.IsChinese ? "警告" : "Warning");
                return;
            }
            if (ltb_product.Text == "")
            {
                MessageBox.Show(VAR.IsChinese?"请先选择新建品目依据的模板！": "Please select the template for the new item first!\r\n请先选择新建品目依据的模板！", VAR.IsChinese ? "警告" : "Warning");
            }
            if (DialogResult.Cancel == MessageBox.Show(VAR.IsChinese ? "确定以 " + ltb_product.Text + " 为模板，建立新产品 " + edt_new_product.Text + " ？\r\n提示：产品列表当前选中的产品为模板。":"Are you sure to use "+ ltb_product.Text+ "as a template to create a new product "+ edt_new_product.Text+ "?\r\n Tip: The currently selected product in the product list is a template.\r\n确定以 " + ltb_product.Text + " 为模板，建立新产品 " + edt_new_product.Text + " ？\r\n提示：产品列表当前选中的产品为模板。", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                return;
            ret = SYS_PUD.AddProduct(edt_new_product.Text, ltb_product.Text);
            if (ret != EM_RES.OK) return;
            ltb_product.Items.Add(edt_new_product.Text);
            FrMain.frrun.cb_product_list.Items.Add(edt_new_product.Text);
            MessageBox.Show(VAR.IsChinese?"新建产品" + edt_new_product.Text + "配置完成!": "New product " + edt_new_product.Text + ",configuration is complete!\r\n新建产品" + edt_new_product.Text + "配置完成!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK,MessageBoxIcon.Information);
            if (MessageBox.Show(VAR.IsChinese ? "切换到新产品?": "Are you sure to switch to a new product?\r\n切换到新产品?", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel) return;
            ltb_product.SelectedIndex = ltb_product.Items.IndexOf(edt_new_product.Text);
            btn_change_Click(sender, e);
        }

        private void btn_change_Click(object sender, EventArgs e)
        {
            if (ltb_product.Text == VAR.gsys_set.cur_product_name)
            {
                MessageBox.Show(VAR.IsChinese?"已是当前型号!": "Already the current model!\r\n已是当前型号!", VAR.IsChinese?"切换信息": "Switch information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(VAR.IsChinese ? "切换到产品：" + ltb_product.Text + " ？": "Switch to product: "+ ltb_product.Text+ "?\r\n切换到产品：" + ltb_product.Text + " ？", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel) return;

            btn_change.Text = VAR.IsChinese?"正在切换...": "Switching ...";
            btn_change.Enabled = false;
            try
            {
                string last_product_name = VAR.gsys_set.cur_product_name;
                EM_RES ret = BaseAction.LoadProductCfg(ltb_product.Text);
                if (ret != EM_RES.OK)
                {
                    ret = BaseAction.LoadProductCfg(last_product_name);
                    if (ret != EM_RES.OK)
                    {
                        MessageBox.Show(VAR.IsChinese?"切换产品失败，且恢复之前产品失败!": "Product switching failed, and product failed before recovery!\r\n切换产品失败，且恢复之前产品失败!", VAR.IsChinese?"错误":"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else MessageBox.Show(VAR.IsChinese ? "切换产品失败，恢复之前产品成功!": "Switching products failed, the product was successful before recovery!\r\n切换产品失败，恢复之前产品成功!", VAR.IsChinese?"警告":"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    edt_cur_product.Text = ltb_product.Text;
                    FrMain.frrun.cb_product_list.Text = edt_cur_product.Text;

                    Msg.secsManager.Send(new BaseInfo() { Id = 2, Value = FrMain.frrun.cb_product_list.Text });
                    Msg.secsManager.Send(new BaseInfo() { Id = 2 }, TypeId: 2);
                    

                    MessageBox.Show(VAR.IsChinese ? "切换产品成功，当前产品为 " + VAR.gsys_set.cur_product_name: "Product switching succeeded, the current product is " + VAR.gsys_set.cur_product_name+ "\r\n切换产品成功，当前产品为 " + VAR.gsys_set.cur_product_name);
                }
            }
            finally
            {
                btn_change.Enabled = true;
                btn_change.Text = VAR.IsChinese?"切换产品": "Switch products";
            }
        }

        private void btn_del_Click(object sender, EventArgs e)
        {
            //判断是否已选择产品
            if (ltb_product.SelectedIndex < 0)
            {
                MessageBox.Show(VAR.IsChinese?"请在列表中选择需要删除的产品型号": "Please select the product model to be deleted from the list\r\n请在列表中选择需要删除的产品型号", VAR.IsChinese?"警告":"Warning");
                return;
            }
            //判断是否选择删除当前产品
            if (ltb_product.Text == VAR.gsys_set.cur_product_name || ltb_product.Text == "Calibration")
            {
                MessageBox.Show(VAR.IsChinese ? "不能删除当前运行的产品型号或校正程序Calibration": "Cannot delete the currently running product model or calibration procedure\r\n不能删除当前运行的产品型号或校正程序Calibration", VAR.IsChinese?"警告":"Warning");
                return;
            }
            ////判断是删除当前切换完成的产品型号
            //if (ltb_product.Text == edt_cur_product.Text)
            //{
            //    edt_cur_product.Text = VAR.gst_sys_set.cur_product_name;
            //}
            //提示确认
            if (MessageBox.Show(VAR.IsChinese?"请确认是否删除该产品?": "Please confirm whether to delete the product?\r\n请确认是否删除该产品?", VAR.IsChinese?"警告":"Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                return;
            //判断文件否存在  
            DirectoryInfo dir = new DirectoryInfo(Path.GetFullPath("..") + "\\product\\" + ltb_product.Text);
            if (dir.Exists)
            {
                dir.Delete(true);
            }

            FrMain.frrun.cb_product_list.Items.Remove(ltb_product.Text);
            ltb_product.Items.Remove(ltb_product.Text);
            ltb_product.Text = edt_cur_product.Text;
            MessageBox.Show(VAR.IsChinese?"删除成功！": "Successfully deleted!");
        }


        private void btn_ud_hightest_Click(object sender, EventArgs e)
        {
            try
            {
                btn_ud1_hightest.Enabled = false;
                btn_ud2_hightest.Enabled = false;
                btn_ud_hightest.Enabled = false;
                int id = 0;
                double[] zref = new double[2];
                EM_RES res = EM_RES.OK;
                ST_XY pos = new ST_XY();
                DialogResult result = MessageBox.Show(VAR.IsChinese?"选择测高模块?\n 1.按'是'键上下料模块1测高。\n 2.按'否'键上下料模块2测高。\n 3.按‘取消’退出。\n注意:请保证两个吸头下的基准面都是在同一高度上,否则异常!": "Select height measurement module? \n 1. Press 'Yes' to measure the height of updownload 1. \n 2.Press 'No' to measure the height of updownload 2. \n 3. Press 'Cancel' to exit. \nNote: Please make sure that the reference planes under the two tips are at the same height, otherwise it will be abnormal!\r\n选择测高模块?\n 1.按'是'键上下料模块1测高。\n 2.按'否'键上下料模块2测高。\n 3.按‘取消’退出。\n注意:请保证两个吸头下的基准面都是在同一高度上,否则异常!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (result == DialogResult.Yes) id = 0;
                else if (result == DialogResult.No) id = 1;
                else return;
                VAR.gsys_set.bquit = false;
                pos.x = COM.List_UDLoad[id].ax_x.fenc_pos;
                pos.y = COM.List_UDLoad[id].ax_y.fenc_pos;
                foreach (XT xt in COM.List_UDLoad[id].list_xt)
                {
                    res = xt.CaliZrefWLByZK(ref VAR.gsys_set.bquit, new ST_XY(-10000, -10000), ref zref[COM.List_UDLoad[id].list_xt.IndexOf(xt)],false);
                    if (res != EM_RES.OK) break;
                }

                if (res == EM_RES.OK)
                {
                    MessageBox.Show(VAR.IsChinese?string.Format("{0}测试数据如下:\n 1.{1}高度:{2:f2}.\n 2.{3}高度:{4:f2}.\n 3.吸头高度差:{5}", COM.List_UDLoad[id].disc, COM.List_UDLoad[id].list_xt[0].disc,
                        zref[0], COM.List_UDLoad[id].list_xt[1].disc, zref[1], (zref[0] + zref[1]) / 2): string.Format("{0}The test data is as follows:\n 1.{2} height:{3:f2}.\n 2.{4}height:{5:f2}.\n 3.Tip height difference:{6}\r\n{1}测试数据如下:\n 1.{2}高度:{3:f2}.\n 2.{4}高度:{5:f2}.\n 3.吸头高度差:{6}", COM.List_UDLoad[id].englishdisc, COM.List_UDLoad[id].disc, COM.List_UDLoad[id].list_xt[0].disc,
                        zref[0], COM.List_UDLoad[id].list_xt[1].disc, zref[1], (zref[0] + zref[1]) / 2), VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(VAR.IsChinese?string.Format("{0}测高失败！", COM.List_UDLoad[id].disc): string.Format("{0}Failed to measure height!\r\n{1}测高失败！", COM.List_UDLoad[id].englishdisc, COM.List_UDLoad[id].disc), VAR.IsChinese?"警告":"Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally 
            {
                btn_ud1_hightest.Enabled = true;
                btn_ud2_hightest.Enabled = true;
                btn_ud_hightest.Enabled = true;
            }
           
        }

        private void btn_traybox_study_Click(object sender, EventArgs e)
        {
            int id = 0;
            DialogResult res = MessageBox.Show("1.按'是'键选择料盘扫码学习。\n 2.按'否'键选择马达扫码学习。", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (res == DialogResult.Yes)
            {
                DialogResult result = MessageBox.Show("选择学习模块?\n 1.按'是'键上下料模块1学习。\n 2.按'否'键上下料模块2学习。", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes) id = 0;
                else if (result == DialogResult.No) id = 1;

                if (id == 0)
                {
                    COM.traybox_fd.tray_barcode_x1 = COM.UDLoad1.ax_x.fenc_pos;
                    COM.traybox_fd.tray_barcode_y1 = COM.UDLoad1.ax_y.fenc_pos;
                    COM.traybox_fd.tray_barcode_a1 = MT.AXIS_UDL_FD_X.fenc_pos;
                    nud_tray_barcode_x1.Value = (decimal)COM.traybox_fd.tray_barcode_x1;
                    nud_tray_barcode_y1.Value = (decimal)COM.traybox_fd.tray_barcode_y1;
                    nud_tray_barcode_a1.Value = (decimal)COM.traybox_fd.tray_barcode_a1;
                }
                else
                {
                    COM.traybox_fd.tray_barcode_x2 = COM.UDLoad2.ax_x.fenc_pos;
                    COM.traybox_fd.tray_barcode_y2 = COM.UDLoad2.ax_y.fenc_pos;
                    COM.traybox_fd.tray_barcode_a2 = MT.AXIS_UDL_FD_X.fenc_pos;
                    nud_tray_barcode_x2.Value = (decimal)COM.traybox_fd.tray_barcode_x2;
                    nud_tray_barcode_y2.Value = (decimal)COM.traybox_fd.tray_barcode_y2;
                    nud_tray_barcode_a2.Value = (decimal)COM.traybox_fd.tray_barcode_a2;
                }
            }
            else
            {
                DialogResult ret = MessageBox.Show("1.按'是'键选择模块1学习。\n 2.按'否'键模块2学习。", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (ret == DialogResult.Yes) id = 0;
                else if (ret == DialogResult.No) id = 1;
                if (id == 0)
                {
                    DialogResult ret2 = MessageBox.Show("1.按'是'键选吸头1学习。\n 2.按'否'键选择吸头2学习。", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (ret2 == DialogResult.Yes) id = 0;
                    else if (ret2 == DialogResult.No) id = 1;
                    if (id == 0)
                    {
                        COM.traybox_fd.motor_barcode_pos[0] = COM.UDLoad1.ax_y.fenc_pos;
                        nud_motor_y1.Value = (decimal)COM.traybox_fd.motor_barcode_pos[0];
                    }else
                    {
                        COM.traybox_fd.motor_barcode_pos[1] = COM.UDLoad1.ax_y.fenc_pos;
                        nud_motor_y2.Value = (decimal)COM.traybox_fd.motor_barcode_pos[1];
                    }
                }
                else
                {
                   
                    DialogResult ret2 = MessageBox.Show("1.按'是'键选吸头3学习。\n 2.按'否'键选择吸头4学习。", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (ret2 == DialogResult.Yes) id = 0;
                    else if (ret2 == DialogResult.No) id = 1;
                    if (id == 0)
                    {
                        COM.traybox_fd.motor_barcode_pos[2] = COM.UDLoad2.ax_y.fenc_pos;
                        nud_motor_y3.Value = (decimal)COM.traybox_fd.motor_barcode_pos[3];
                    }
                    else
                    {
                        COM.traybox_fd.motor_barcode_pos[3] = COM.UDLoad2.ax_y.fenc_pos;
                        nud_motor_y4.Value = (decimal)COM.traybox_fd.motor_barcode_pos[4];
                    }
                }
            }
        }

        private void btn_GetAddCapQrcodePos_Click(object sender, EventArgs e)
        {
            VAR.gsys_set.bquit = false;
            ST_XYA pos = new ST_XYA();
            int id = 0;
            if (ctb_udload_cfg.SelectedIndex > 1 || ctb_udload_cfg.SelectedIndex < 0)
            {
                ctb_udload_cfg.SelectedIndex = 0;
            }
            id = ctb_udload_cfg.SelectedIndex;
            var ppos = new ST_XYZA();
            DialogResult result = MessageBox.Show(string.Format("位置学习说明:\n 1.按'是'键学习轴的当前位置(如是OK与NG料盘位置为当前位置-吸头偏差)。\n 2.按'否'键退出位置学习", COM.List_UDLoad[(id + 1) % 2].disc), VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                TrayCfgGetPos(ref ppos, false);
                pos = ppos.ToXYA();
            }
            else return;
            //nud_AddCapQrcodePosX.Value = (decimal)pos.x;
            nud_AddCapQrcodePosY.Value = (decimal)pos.y;
            //nud_AddCapQrcodePosTX.Value = (decimal)pos.a;
        }

        private void btn_GotoAddCapQrcodePos_Click(object sender, EventArgs e)
        {
            VAR.gsys_set.bquit = false;
            Button btn = (Button)sender;

            ST_XYZA pos = new ST_XYZA((double)nud_AddCapQrcodePosX.Value, (double)nud_AddCapQrcodePosY.Value,0, (double)nud_AddCapQrcodePosTX.Value);
            pos.z = 0;
            EM_RES res = GotoAddCapQrcodePos(pos);
            if (res != EM_RES.OK)
                MessageBox.Show(VAR.IsChinese ? "定位失败!" : "Positioning failed!\r\n定位失败!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btn_MakeAddCapQrcodePosArray_Click(object sender, EventArgs e)
        {
            GetAddQrcodeCapPosData(tray);
            tray.CreateCapQrcodePos();
            btn_SaveAddCapQrcodePos_Click(null, null);
        }

        private void btn_SaveAddCapQrcodePos_Click(object sender, EventArgs e)
        {
            string trayname = string.Empty;
            GetAddQrcodeCapPosData(tray);
            tray.SaveDat();
            tray.LoadDat();
            ShowTrayData(tray);
            trayname = GetCurrentTrayCfgBinding().TrayAxisName;
            if (DialogResult.No == MessageBox.Show(VAR.IsChinese ? string.Format("保存成功!\r\n注:如需修改的参数立即生效,请按'是'键更换{0},并拿掉{1}上的料盘!", traybox.disc, trayname) : string.Format("Saved successfully! \r\n Note: If the parameters to be modified take effect immediately, press the 'Yes' button to replace {0}, and remove the tray on {2}!\r\n保存成功!\r\n注:如需修改的参数立即生效,请按'是'键更换{1},并拿掉{2}上的料盘!", traybox.name, traybox.disc, trayname), VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Information)) return;
            EM_RES res = traybox.NewBox();
            if (res == EM_RES.OK) traybox.IsReady = true;
            traybox.tray_idx = 0;
            traybox.tray_cur = null;
        }

        private void cTabControl2_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if(ctb_tray_cfg.SelectedIndex != 0 && e.TabPageIndex !=0)
            {
                e.Cancel = true;
            }
        }

        private void cbouttray_CheckedChanged(object sender, EventArgs e)
        {
            tray.IsoutTray = cbouttray.Checked;
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}
