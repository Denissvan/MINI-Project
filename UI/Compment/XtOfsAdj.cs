using System;
using System.Collections.Generic;
using System.Drawing;
//using System.ComponentModel;
//using System.Drawing;
//using System.Data;
//using System.Linq;
//using System.Runtime.InteropServices.WindowsRuntime;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using MotionCtrl;
using System.Threading;
//using Cognex.VisionPro;

namespace UI.Compment
{
    public partial class XtOfsAdj : UserControl
    {
        Cam dwcam = COM.CamDw1;
        Cam upcam = COM.CamUp1;
        XT xt= COM.xt1;
         XT.EM_XTFLOW CamMod = XT.EM_XTFLOW.PLACEMOD;
        ST_XYA st_cam_dw_vs = new ST_XYA();
        ST_XYA st_cam_up_vs = new ST_XYA();
        ST_XYA st_cam_up_pos = new ST_XYA();
        ST_XYA st_cam_up_endpos = new ST_XYA();
        ST_XYA st_ShapeOrgOffset = new ST_XYA();
        Cam.ComShape dwshp =new Cam.ComShape();
        Cam.ComShape dwshp_temp = new Cam.ComShape();
        public Product.Tray tray = new Product.Tray();
        string upcam_flow= "WsUp_Shp";
        string dwcam_flow = "ModDw1_Shp";
        ST_XY st_pos_upcam_temp = new ST_XY(25, 139);
        double ZHigh = 7;
        int ofs_id;
        bool UpdataCurCfg = true;
        //XT xt2 = COM.xt2;
        public XtOfsAdj()
        {
            InitializeComponent();
           // GetCurCfg();
        }

        public void PmsEn(User.PERMISSION pms)
        {
            if (pms <= User.PERMISSION.Engineer)
            {
                this.Enabled = false;
            }
            else
            {
                if (UpdataCurCfg)
                {
                    GetCurCfg();
                    UpdataCurCfg = false;
                }
                this.Enabled = true;
            }
        }

        public void GetCurCfg()
        {
            
            if (rbtn_xt1.Checked) xt = COM.xt1;
            else if (rbtn_xt2.Checked) xt = COM.xt2;
            else if (rbtn_xt3.Checked) xt = COM.xt3;
            else if (rbtn_xt4.Checked) xt = COM.xt4;
            else
            {
                rbtn_xt1.Checked = true;
                xt = COM.xt1;
            }

            if (rbtn_palce.Checked)
            {
                CamMod = XT.EM_XTFLOW.PLACEMOD;
                upcam_flow = "WsUp_Shp";
                dwcam_flow = string.Format("ModDw{0}_Shp",xt.id%2+1);
                WS ws = Turntable.GetWSOnFeedPos;
                if (ws != null)
                {
                    WS.MdDat MdDat = new WS.MdDat();
                    MdDat = ws.list_md.Find(s => s.Num.Equals((int)nud_wsnum.Value));
                    nub_Zhigh.Value = (decimal)MdDat.st_pos[xt.Parent.id].z;
                }
            }
            else if (rbtn_pick.Checked)
            {
                CamMod = XT.EM_XTFLOW.PICKMOD;
                upcam_flow = "ModUp_Shp";
                dwcam_flow = string.Format("Xt{0}_Shp", xt.id % 2+1);
                nub_Zhigh.Value = (decimal)COM.tray_fd.tl[xt.Parent.id].z;
            }
            else
            {
                rbtn_palce.Checked = true;
                CamMod = XT.EM_XTFLOW.PLACEMOD;
                upcam_flow = "WsUp_Shp";
                dwcam_flow = string.Format("ModDw{0}_Shp", xt.id % 2+1);
            }
            upcam = xt.upcam;
            dwcam = xt.dwcam;
            xt.bAdj_DwCam = false;
            xt.bAdj_UpCam = false;
            nud_xt_offset_x.Text = xt.vs_offset[(int)CamMod].x.ToString("f3");
            nud_xt_offset_y.Text = xt.vs_offset[(int)CamMod].y.ToString("f3");
            nud_xt_offset_a.Text = xt.vs_offset[(int)CamMod].a.ToString("f3");

        }

        private void btn_Dir_Click(object sender, EventArgs e)
        {
            if (dwshp == null) 
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese?"找不到轮廓!": "Can't find outline!         (找不到轮廓!)");
                MessageBox.Show(VAR.IsChinese ? "找不到轮廓!" : "Can't find outline!\r\n找不到轮廓!");
                return;
            }
            double mov_ofs = 0.1;
            if (rbtn_001.Checked)
                mov_ofs = 0.01;
            else if(rbtn_1.Checked)
                mov_ofs = 1;
            string name = ((Button)sender).Name;
            switch (name)
            {
                case "btn_left":
                   dwshp.Move(0, mov_ofs, 0, false, upcam.mCogRecDisplay);
                   break;
                case "btn_up":
                   dwshp.Move(-mov_ofs,0, 0, false, upcam.mCogRecDisplay);
                   break;
                case "btn_right":
                   dwshp.Move(0, -mov_ofs, 0, false, upcam.mCogRecDisplay);
                   break;
                case "btn_ccw":
                    if (PT_SET.isopen_degree && rbtn_pick.Checked)
                    {
                        mov_ofs = 90 * mov_ofs;
                    }
                    else if (rbtn_palce.Checked)
                    {
                        mov_ofs = 1 * mov_ofs;
                    }
                   dwshp.Move(0, 0, -mov_ofs, false, upcam.mCogRecDisplay);
                   break;
                case "btn_down":
                   dwshp.Move(mov_ofs, 0, 0, false, upcam.mCogRecDisplay);
                   break;
                case "btn_cw":
                    if (PT_SET.isopen_degree && rbtn_pick.Checked)
                    {
                        mov_ofs = 90 * mov_ofs;
                    }
                    else if (rbtn_palce.Checked)
                    {
                        mov_ofs = 1 * mov_ofs;
                    }
                    dwshp.Move(0, 0, mov_ofs, false, upcam.mCogRecDisplay);
                   break;
            }
            tb_shape_pos.Text = dwshp.GetPos().ToString();
              
        }
        ST_XY st_pos_upcam = new ST_XY(25, 139);
        private void btn_dwcam_Click(object sender, EventArgs e)
        {
            
            EM_RES ret;
            MT.SetAllAxToWorkSpd();
            VAR.gsys_set.bquit = false;
            //ret = MT.Move(ref VAR.gsys_set.bquit, ref xt.ax_z, 0);
            //if (ret != EM_RES.OK) return;
            ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref xt.ax_y, xt.st_cap_pos.y, ref xt.ax_u, 0,true);
            if (ret != EM_RES.OK) return ;
            ret = dwcam.FindTaskTriAndWait(dwcam_flow);
            if (ret != EM_RES.OK) return;
            Cam.VisionTask task = dwcam.curTask;
            st_cam_dw_vs = task.ResData.PosMM;
            dwshp.mShape = task.ResData.ShapeOutput;
            dwshp.mShape.ID = xt.id *10 + (int)CamMod;
            dwshp.Save();


            //保存吸头坐标
            if (dwcam_flow.Contains("Xt"))
            {
                //if (DialogResult.Yes == MessageBox.Show("是否保存当前吸头坐标?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                //{
 
                    xt.st_vs_pos_xtshp = st_cam_dw_vs;
                    xt.SaveCfg(VAR.gsys_set.cur_product_name);
                //}

            }
            tb_camdw_vs.Text = st_cam_dw_vs.ToString();
            xt.bAdj_DwCam = true;
        }
        
        private void btn_upcam_Click(object sender, EventArgs e)
        {
            EM_RES ret;
            ret = upcam.FindTaskTriAndWait(upcam_flow);
            if (ret != EM_RES.OK) return;
            st_cam_up_vs = upcam.curTask.ResData.PosMM;
            st_cam_up_pos.x = xt.ax_x.fcmd_pos;
            st_cam_up_pos.y = xt.ax_y.fcmd_pos;
            tb_camup_vs.Text = st_cam_up_vs.ToString();
            tb_camup_pos.Text = st_cam_up_pos.ToString();
            xt.bAdj_UpCam = true;
        }

        private void btn_shp_Click(object sender, EventArgs e)
        {

            //if (!xt.bAdj_DwCam)
            //{
            //    MessageBox.Show("下相机没有拍照,无法调出轮廓，请进行下相机拍照!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            if (!xt.bAdj_UpCam)
            {
                MessageBox.Show(VAR.IsChinese?"上相机没有拍照,没有对位基准，请进行上相机拍照!": "There is no picture on the camera, there is no alignment reference, please take a picture on the camera!\r\n上相机没有拍照,没有对位基准，请进行上相机拍照!", VAR.IsChinese?"错误":"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            upcam.mCogRecDisplay.InteractiveGraphics.Clear();
            dwshp.Load(xt.id *10 + (int)CamMod);
            dwshp.mShape.SelectedSpaceName = upcam.mCogRecDisplay.Image.SelectedSpaceName;
            upcam.mCogRecDisplay.InteractiveGraphics.Add(dwshp.mShape, dwshp.mShape.ToString(), true);
            Thread.Sleep(100);
            dwshp.Move(0, 0.01, 0, false, upcam.mCogRecDisplay);//防止拖动没有反应
            tb_shape_pos.Text = dwshp.GetPos().ToString();
        }

        private void btn_calc_ofs_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show(VAR.IsChinese ? "是否需要更改当前偏差?": "Do I need to change the current deviation?\r\n是否需要更改当前偏差?", VAR.IsChinese?"提示": "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Question))return;
            if (dwshp == null) 
            {
                MessageBox.Show(VAR.IsChinese?"找不到轮廓!": "No outline found!\r\n找不到轮廓!");
                return;
            }
            ST_XYA st_ofs=new ST_XYA();
           // ofs_id = upcam == COM.CamUp1 ? 0 : 1;
            st_cam_up_endpos.x= xt.ax_x.fcmd_pos;
            st_cam_up_endpos.y = xt.ax_y.fcmd_pos;
            st_ShapeOrgOffset = dwshp.GetPos();
            Utility.CalcOffset(st_cam_up_vs, st_cam_dw_vs, st_ShapeOrgOffset, ref st_ofs);
            st_ofs.x = (st_cam_up_endpos.x - st_cam_up_pos.x) + st_ofs.x;
            st_ofs.y = (st_cam_up_endpos.y - st_cam_up_pos.y) + st_ofs.y;
            st_ofs.a = st_ShapeOrgOffset.a;
            xt.vs_offset[(int)CamMod] = st_ofs;
            xt.vs_down_ref_angle[(int)CamMod] = st_cam_dw_vs.a;
            xt.vs_up_ref_angle[(int)CamMod] = st_cam_up_vs.a;
            tb_ofs.Text = st_ofs.ToString() + string.Format(",Auar={0:F3},Adar={1:F3}", xt.vs_up_ref_angle[(int)CamMod], xt.vs_down_ref_angle[(int)CamMod]);
        }

        private void btn_save_ofs_Click(object sender, EventArgs e)
        {
            xt.SaveOffset(VAR.gsys_set.cur_product_name);
            dwshp.Save();
            nud_xt_offset_x.Text = xt.vs_offset[(int)CamMod].x.ToString("f3");
            nud_xt_offset_y.Text = xt.vs_offset[(int)CamMod].y.ToString("f3");
            nud_xt_offset_a.Text = xt.vs_offset[(int)CamMod].a.ToString("f3");
            // cb_dwshp_SelectedIndexChanged(null, null);
        }

        private void btn_place_Click(object sender, EventArgs e)
        {
            ST_XYA st_place;
            VAR.gsys_set.bquit = false;
             ST_XYA cali_ofs;
            ST_XYZA pos_place;

            MT.SetAllAxToWorkSpd();
            if (dwcam_flow.Contains("Xt"))
            {
                st_cam_dw_vs = xt.st_vs_pos_xtshp;
            }

            cali_ofs = xt.st_pos_affine_place.ToXYA() - xt.st_pos_affine_dwcam_cap.ToXYA() - xt.st_pos_affine_upcam_cap.ToXYA();
            cali_ofs.a = 0;
            st_place = Utility.CalcPlacePos(st_cam_up_pos.ToXY(), st_cam_up_vs, xt.st_cap_pos.ToXY(), st_cam_dw_vs, cali_ofs, xt.st_rol.ToXY(), xt.st_rol_cap, xt.vs_offset[(int)CamMod], xt.vs_up_ref_angle[(int)CamMod], xt.vs_down_ref_angle[(int)CamMod]);
            pos_place = st_place.ToXYZA();
            ZHigh = (double)nub_Zhigh.Value;
            if (xt.id % 2 ==0)
            {
                pos_place.z = ZHigh;
            }
            else
            {
                pos_place.z = -ZHigh;
            }

            if (xt.cy_zk.isSenONActive)
            {
                xt.PickOrPlace(ref VAR.gsys_set.bquit, pos_place, false);
            }
            else
            {
                xt.PickOrPlace(ref VAR.gsys_set.bquit, pos_place, true);
            }
            tb_place.Text = st_place.ToString();
        }

        private void btn_center_Click(object sender, EventArgs e)
        {
            tray_map_ofs.Visible = !tray_map_ofs.Visible;
            tray_map_ofs.Enabled = !tray_map_ofs.Enabled;
            if (tray_map_ofs.Visible) btn_center.BackColor =Color.Lime;
            else btn_center.BackColor = SystemColors.ButtonFace;

            #region oldpro
            //EM_RES res;
            //ST_XYZA StMovPos = new ST_XYZA();
            //StMovPos = st_pos_upcam.ToXYZA();
            //StMovPos.x = xt.ax_x.fenc_pos;
            //StMovPos.y = xt.ax_y.fenc_pos;
            //VAR.gsys_set.bquit = false;
            //upcam.List_vs_task_cur.Clear();
            //Cam.VisionTask VsTask = upcam.List_vs_task.Find(s => s.TaskName.Equals(upcam_flow));
            //upcam.List_vs_task_cur.Add(VsTask);
            //res = upcam.MoveToImgCenter(ref VAR.gsys_set.bquit, ref StMovPos, VsTask, upcam.ListCaliTool);
            //if (res != EM_RES.OK) return;
            //st_pos_upcam.x = xt.ax_x.fenc_pos;
            //st_pos_upcam.y = xt.ax_y.fenc_pos;
            //txb_Upcam_x.Value = (decimal)xt.ax_x.fenc_pos;
            //txb_Upcam_y.Value = (decimal)xt.ax_y.fenc_pos;
            #endregion


        }

        private void btn_test_Click(object sender, EventArgs e)
        {
            //VAR.gsys_set.mode = EM_SYS_MODE.DEMO;
            //VAR.gsys_set.mode = EM_SYS_MODE.NORMAL;
            //MT.SetAllAxToWorkSpd();
            //VAR.gsys_set.bquit=false;
           
            //Task task1 = new Task(() =>
            //  {
            //    //UploadModle.UploadModleAct(ref VAR.gsys_set.bquit, COM.ws4);
            //    // DownloadModle.DownloadModleAct(ref VAR.gsys_set.bquit, COM.ws4);
            //      //把工位为空
            //      EM_RES res,res2;
            //      foreach (WS.MdDat md in COM.ws4.list_md)
            //      {
            //          md.res = -2;

            //      }
            //       COM.ws4.TestStatus = WS.EM_TEST_STA.EMPTY;
            //      int  cnt=0;
            //      while(true)
            //      {
            //             DownloadModle.RunTask();
            //             UploadModle.RunTask();
                        
            //            res = DownloadModle.WaitTask(ref VAR.gsys_set.bquit);
            //            if (res != EM_RES.OK)
            //            {
            //                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "下料异常!");
            //                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "下料异常!", 0, true);
            //                break;
            //            }
            //            res2 = UploadModle.WaitTask(ref VAR.gsys_set.bquit);
            //            if (res2 != EM_RES.OK)
            //            {
            //                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "上料异常!");
            //                VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, "上料异常!", 0, true);
            //                break;
            //            }
                        
            //            Thread.Sleep(1000);

            //           int i=0;
            //            foreach (WS.MdDat md in COM.ws4.list_md)
            //            {
            //                if (!md.benable||md.res == -1)
            //                    i++;
            //            }
            //            if (i == COM.ws4.list_md.Count)
            //            {
            //                foreach (WS.MdDat md in COM.ws4.list_md)
            //                {
            //                    if (md.Num % 2 == 0)
            //                        md.res = 0;
            //                    else
            //                        md.res = 1;
            //                }
            //                COM.ws4.TestStatus = WS.EM_TEST_STA.COMPLETED;
            //            }
                       
                       

            //      }
                 
            //  });
            //task1.Start();
           
            //if (COM.traybox_ok.tray_idx == 0 && COM.traybox_ok.tray_cur == null)
            //    COM.traybox_ok.Tray_InOut(ref VAR.gsys_set.bquit, TrayBox.EM_DIR.ONLY_OUT);
            //else
            //    COM.traybox_ok.Tray_InOut(ref VAR.gsys_set.bquit, TrayBox.EM_DIR.IN_OUT);
            
        }


        private void btn_cam_Click(object sender, EventArgs e)
        {
            EM_RES ret;
            MT.SetAllAxToWorkSpd();
            VAR.gsys_set.bquit = false;
           // ret = MT.Move(ref VAR.gsys_set.bquit, ref xt.ax_z, 0);
           // if (ret != EM_RES.OK) return;
            st_pos_upcam_temp.x= (double) txb_Upcam_x.Value;
            st_pos_upcam_temp.y = (double)txb_Upcam_y.Value;
            st_pos_upcam = st_pos_upcam_temp;
            ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref xt.ax_x, st_pos_upcam.x, ref xt.ax_y, st_pos_upcam.y, true);
            if (ret != EM_RES.OK) return;
            btn_upcam_Click(null, null);

        }

        private void btn_zup_Click(object sender, EventArgs e)
        {
            if (xt.cy_zk.isOFF)
            {
                xt.cy_zk.SetOn();
            }
            else
            {
                xt.cy_zk.SetOff();
                xt.gpio_pzk.SetOn();
                Thread.Sleep(100);
                xt.gpio_pzk.SetOff();
            }
        }

        
        private void btn_get_upcam_pos_Click(object sender, EventArgs e)
        {
            txb_Upcam_x.Value = (decimal)xt.ax_x.fenc_pos;
            txb_Upcam_y.Value = (decimal)xt.ax_y.fenc_pos;
            st_pos_upcam_temp.x = xt.ax_x.fenc_pos;
            st_pos_upcam_temp.y = xt.ax_y.fenc_pos;
            st_pos_upcam = st_pos_upcam_temp;
        }

        private void XtOfsAdj_Load(object sender, EventArgs e)
        {
            txb_Upcam_x.Value = (decimal)st_pos_upcam.x;
            txb_Upcam_y.Value = (decimal)st_pos_upcam.y;
            tray_map_ofs.Visible = false;
            tray_map_ofs.Enabled = false;
            tray = COM.tray_fd;
            tray_map_ofs.TrayName = "供料盘_位置选择";
            tray_map_ofs.tray_dat = tray;
            
            tray_map_ofs.bShowMode = true;
            //GetCurCfg();
            //cb_dwshp.Items.Clear();
            //foreach (Cam.VisionTask task in dwcam.List_vs_task)
            //{
            //    cb_dwshp.Items.Add(task.TaskName);
            //}
            //if (cb_dwshp.Items.Count > 0)
            //    cb_dwshp.SelectedIndex = 0;
            // nub_Zhigh.Value = (decimal)ZHigh;

        }

        public void trayUpdate(Product.Tray tray_fd)
        {
            tray = tray_fd;
            tray_map_ofs.tray_dat = tray;
        }

        private void cb_dwshp_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ofs_id = 0;
            //确定吸头
            //foreach (XT xt_temp in COM.ListXT)
            //{
            //    if (cb_dwshp.Text.Contains((xt_temp.id+1).ToString()))
            //    {
            //        xt = xt_temp;
            //        break;
            //    }
            //}

            //if (cb_dwshp.Text.Contains("Xt"))
            //{
            //    upcam = COM.CamUp1;
            //    upcam_flow = "ModUp_Shp";
            //    ofs_id = 0;

            //}
            //else if (cb_dwshp.Text.Contains("ModDw"))
            //{
            //    upcam = COM.CamUp2;
            //    upcam_flow = "Tray_Shp";
            //    ofs_id = 1;
            //}
            //nud_xt_offset_x.Text = xt.vs_offset[ofs_id].x.ToString("f3");
            //nud_xt_offset_y.Text = xt.vs_offset[ofs_id].y.ToString("f3");
            //nud_xt_offset_a.Text = xt.vs_offset[ofs_id].a.ToString("f3");
        }

        private void btn_PickWl_Click(object sender, EventArgs e)
        {

            MT.SetAllAxToWorkSpd();
           // UploadModle.ax_z.SetSpdRadio(0.5);
            ZHigh = (double)nub_Zhigh.Value;
            if (Math.Abs(xt.ax_z.fenc_pos) > 1)
            {
                btn_PickWl.Text = VAR.IsChinese?"取料":"Pick";
                MT.Move(ref VAR.gsys_set.bquit, ref xt.ax_z, 0);
            }
            else
            {
                btn_PickWl.Text = VAR.IsChinese?"抬起":"Up";
                if (xt == COM.xt1 || xt == COM.xt3)
                {
                   MT.Move(ref VAR.gsys_set.bquit, ref xt.ax_z, ZHigh);
                }
                else
                {
                   MT.Move(ref VAR.gsys_set.bquit, ref xt.ax_z, -ZHigh);
                }
            }
        }

        private void btn_goto_Click(object sender, EventArgs e)
        {
            ST_XYA st_place;
            VAR.gsys_set.bquit = false;
            ST_XYA cali_ofs;
            cali_ofs = xt.st_pos_affine_place.ToXYA() - xt.st_pos_affine_dwcam_cap.ToXYA() - xt.st_pos_affine_upcam_cap.ToXYA();      
            cali_ofs.a = 0;
            st_place = Utility.CalcPlacePos(st_cam_up_pos.ToXY(), st_cam_up_vs, xt.st_cap_pos.ToXY(), st_cam_dw_vs, cali_ofs, xt.st_rol.ToXY(), xt.st_rol_cap, xt.vs_offset[(int)CamMod], xt.vs_up_ref_angle[(int)CamMod], xt.vs_down_ref_angle[(int)CamMod]);
            MT.ZupMove(ref VAR.gsys_set.bquit, ref xt.ax_x, st_place.x, ref xt.ax_y, st_place.y, ref xt.ax_u, st_place.a,true);
            tb_place.Text = st_place.ToString();
        }

        private void rbtn_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton) sender;
            if (btn.Checked)
             GetCurCfg();
        }

        private void btn_pick_Click(object sender, EventArgs e)
        {

            if (COM.traybox_fd.tray_cur == null)
            {
                MessageBox.Show(VAR.IsChinese?"没有发现待测料盘，请进料盘！": "No tray to be tested was found, please feed the tray!\r\n没有发现待测料盘，请进料盘！");
                return;
            }

            List<Product.Tray.PosInf> ListPickMod = COM.traybox_fd.tray_cur.GetPosList();
            if (ListPickMod != null && ListPickMod.Count == 0)
            {
                MessageBox.Show(VAR.IsChinese ? "当前料盘没有发现物料，请重新进料盘！": "No material was found in the current tray, please re-feed the tray!\r\n当前料盘没有发现物料，请重新进料盘！");
                return;
            }

            VAR.gsys_set.bquit = false; string barcode;
            EM_RES res = xt.XtCapMovMod(ref VAR.gsys_set.bquit,out barcode, ListPickMod[0].Pos[xt.Parent.id], CONST.ModUpFw, xt.Parent.id == 0 ? 10 : -10,
                XT.EM_XTFLOW.PICKMOD, true, false);
            ListPickMod[0].md = null;
            if (res != EM_RES.OK)
            {
               
                MessageBox.Show(VAR.IsChinese?String.Format("{0}取料失败！",xt.disc): String.Format("{0}Pick mod failed!\r\n{0}取料失败！", xt.disc));
            }
             
        }

        private void btn_wsgoto_Click(object sender, EventArgs e)
        {
            WS ws = Turntable.GetWSOnFeedPos;
            if (ws == null)
            {
                MessageBox.Show(VAR.IsChinese ? "没有找到当前上料位工站！": "No current uploading station was found!\r\n没有找到当前上料位工站！");
                return;
            }
            WS.MdDat MdDat = new WS.MdDat();
            MdDat = ws.list_md.Find(s => s.Num.Equals((int) nud_wsnum.Value));
            if (MdDat == null)
            {
                MessageBox.Show(VAR.IsChinese ? string.Format("{0}没有找到编号为{1}的工位",ws.disc, (int)nud_wsnum.Value): string.Format("No station number {1} was found on the {0}\r\n{0}没有找到编号为{1}的工位", ws.disc, (int)nud_wsnum.Value));
                return;
            }
          EM_RES  res = MT.ZupMove(ref VAR.gsys_set.bquit, ref xt.ax_x, MdDat.st_pos[xt.id/2].x, ref xt.ax_y, MdDat.st_pos[xt.id / 2].y,true);
            if (res != EM_RES.OK)
            {
                MessageBox.Show(VAR.IsChinese ? string.Format("{0}定位{1}{2}失败",xt.disc ,ws.disc, (int)nud_wsnum.Value): string.Format("{0} move to {1} {2} failed!\r\n{0}定位{1}{2}失败", xt.disc, ws.disc, (int)nud_wsnum.Value));
            }
            btn_upcam_Click(null, null);
        }

        private void tray_map_ofs_CellClik(object sender, EventArgs e)
        {
            EM_RES ret=EM_RES.OK;
            int ud_id = 0;
            VAR.gsys_set.bquit = false;
            tray_map_ofs.tray_dat.list_pos.Find(s =>
            {
                s.md = null;
                return false;
            });
            ((tray.TrayClikEventArgs)e).PosInf.md = new Product.MdDat();
            ((tray.TrayClikEventArgs)e).PosInf.md.res = 1;
            int rowidx = ((tray.TrayClikEventArgs)e).PosInf.idx;
            tray_map_ofs.UpdateShow();
            if (xt.id >= COM.xt3.id) ud_id = 1;
            ST_XYZA gopos = tray_map_ofs.tray_dat.list_pos[rowidx].Pos[ud_id];
            if (DialogResult.No == MessageBox.Show(VAR.IsChinese?string.Format("确认是否定位到当前坐标X:{0},Y:{1},X_TR:{2}", gopos.x, gopos.y, gopos.a): string.Format("Confirm whether to locate to the current coordinates X:{0},Y:{1},X_TR:{2}\r\n确认是否定位到当前坐标X:{0},Y:{1},X_TR:{2}", gopos.x, gopos.y, gopos.a), VAR.IsChinese?"提示": "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;           
            ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref COM.List_UDLoad[ud_id].ax_x, gopos.x, ref COM.List_UDLoad[ud_id].ax_y, gopos.y, ref COM.List_UDLoad[ud_id].traybox_fd.ax_x, gopos.a);
            if (ret != EM_RES.OK)
            {
                MessageBox.Show(VAR.IsChinese?"定位失败!": "Positioning failed!\r\n定位失败!");
                return;
            }

            btn_upcam_Click(null, null);
            tray_map_ofs.Visible = false;
            tray_map_ofs.Enabled = false;
            btn_center.BackColor = SystemColors.ButtonFace;
            //tray_map_ofs.Refresh();
        }
    }
}
