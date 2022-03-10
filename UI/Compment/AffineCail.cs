using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MotionCtrl;

namespace UI.Compment
{
    public partial class AffineCail : UserControl
    {
        public XT xt;
        public Cam Frcam; //源头相机
        public Cam Tocam;//目标相机
        ST_XYZA st_pos_place;
        ST_XY st_pos_Tocam_cap;
        ST_XY st_pos_Frcam_cap;
        ST_XYA st_stransform;
        // bool IsDwToUp;
        public UpDownLoad udl=new UpDownLoad();
        public List<ST_XY> list_affine_vsdat_Frcam = new List<ST_XY>();  //源头相机仿射校准，拍照数据
        public List<ST_XY> list_affine_vsdat_Tocam = new List<ST_XY>();  //目标相机仿射校准，拍照数据
        public AffineCail()
        {
            InitializeComponent();
        }

        public void Init(ref UpDownLoad udl)
        {
            this.udl = udl;
            this.xt = udl.list_xt[0];
            this.Frcam = udl.dwcam;
            this.Tocam = udl.upcam;
            Frcam.GetAllCaliToolXYAScale(ref st_stransform);
            //this.IsDwToUp = IsDwToUp;
            lbl_affine_place.Text = xt.disc+ "放料位:";
            lbl_affine_upcam.Text = Tocam.disc+"拍照位:";
            lbl_affine_downcam.Text = Frcam.disc+"拍照位:";
            if (!VAR.IsChinese)
            {
                lbl_affine_place.Text = udl == COM.UDLoad1?"XT1 Place":"XT3 Place";
                lbl_affine_upcam.Text = udl == COM.UDLoad1 ? "UpCam1" : "UpCam2";
                lbl_affine_downcam.Text = udl == COM.UDLoad1 ? "DwCam1" : "DwCam2";
            }
            //if (IsDwToUp)
            //{
            //    lbl_affine_downcam.Text = "下相机拍照位:";
            //}
            //else
            //{
            //    lbl_affine_downcam.Text = "上相机2拍照位:";
            //}
            ShowData(true);
        }
        public void PmsEn(User.PERMISSION pms)
        {
            if (pms <= User.PERMISSION.Engineer)
            {
                this.Enabled = false;
            }
            else
            {
                this.Enabled = true;
            }
        }

        public void ShowData(bool bload = false, double rmse = -1)
        {
            if (bload)
            {
                st_pos_place = xt.st_pos_affine_place;
                st_pos_Frcam_cap = xt.st_pos_affine_dwcam_cap;
                st_pos_Tocam_cap = xt.st_pos_affine_upcam_cap;
                //if (IsDwToUp)
                //{                   
                //    st_pos_Frcam_cap = xt.st_pos_affine_downcam_cap;
                //    st_pos_Tocam_cap=xt.st_pos_affine_dwtoup_cam1_cap;
                //}
                //else
                //{
                //    st_pos_Frcam_cap = xt.st_pos_affine_upcam2_cap.ToXYZ();
                //    st_pos_Tocam_cap = xt.st_pos_affine_uptoup_cam1_cap;
                //}
            }
            nud_affine_place_x.Value = (decimal)st_pos_place.x;
            nud_affine_place_y.Value = (decimal)st_pos_place.y;
            nud_affine_place_z.Value = (decimal)st_pos_place.z;
            nud_affine_place_fdx.Value = (decimal)st_pos_place.a;

            nud_affine_downcam_x.Value = (decimal)st_pos_Frcam_cap.x;
            nud_affine_downcam_y.Value = (decimal)st_pos_Frcam_cap.y;
            nud_affine_downcam_z.Value = 0;

            nud_affine_upcam_x.Value = (decimal)st_pos_Tocam_cap.x;
            nud_affine_upcam_y.Value = (decimal)st_pos_Tocam_cap.y;
            nud_affine_upcam_z.Value = 0;
            tb_result.Text = string.Format("Xs={0:F3} Ys={1:F3}  A={2:F3} {3} ", st_stransform.x, st_stransform.y, st_stransform.a, rmse > -1 ? string.Format("RMSE={0:F3}", rmse) : "");
        }

        public void GetData(bool bload = false)
        {
            st_pos_place.x = (double)nud_affine_place_x.Value;
            st_pos_place.y = (double)nud_affine_place_y.Value;
            st_pos_place.z = (double)nud_affine_place_z.Value;
            st_pos_place.a = (double)nud_affine_place_fdx.Value;

            st_pos_Frcam_cap.x = (double)nud_affine_downcam_x.Value;
            st_pos_Frcam_cap.y = (double)nud_affine_downcam_y.Value;
            //st_pos_Frcam_cap.z = (double)nud_affine_downcam_z.Value;

            st_pos_Tocam_cap.x = (double)nud_affine_upcam_x.Value;
            st_pos_Tocam_cap.y = (double)nud_affine_upcam_y.Value;
            if (bload)
            {
                foreach(XT Xt in  udl.list_xt)
                {
                    Xt.st_pos_affine_place= st_pos_place;
                    Xt.st_pos_affine_dwcam_cap = st_pos_Frcam_cap;
                    Xt.st_pos_affine_upcam_cap = st_pos_Tocam_cap;
                    //if (IsDwToUp)
                    //{
                    //    Xt.st_pos_affine_downcam_cap= st_pos_Frcam_cap;
                    //    Xt.st_pos_affine_dwtoup_cam1_cap = st_pos_Tocam_cap;
                    //}
                    //else
                    //{
                    //    Xt.st_pos_affine_upcam2_cap= st_pos_Frcam_cap.ToXY();
                    //    Xt.st_pos_affine_uptoup_cam1_cap = st_pos_Tocam_cap;
                    //}
                }
               
            }
        }
        private void btn_affine_movpos_Click(object sender, EventArgs e)
        {
            EM_RES ret = EM_RES.OK;
            VAR.gsys_set.bquit = false;
            GetData();
            MT.SetAllAxToManualSpd();
            //if (Math.Abs(UploadModle.ax_z.fenc_pos) > 0.5)
            //{
            //    ret = UploadModle.ZHome(ref VAR.gsys_set.bquit);
            //    if (ret != EM_RES.OK) return;
            //}
            //ret = MT.Move(ref VAR.gsys_set.bquit, ref xt.ax_z,0);
            if (ret != EM_RES.OK) return ;
            string name = ((Button)sender).Name;
            switch (name)
            {
                case "btn_affine_place_movpos": //放料位置定位
                    ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref xt.ax_x, st_pos_place.x, ref xt.ax_y, st_pos_place.y,ref udl.traybox_fd.ax_x, st_pos_place.a,true);
                    if (ret != EM_RES.OK) return;
                    break;
                case "btn_affine_downcam_movpos"://下相机(源相机)拍照位置定位
                   // ret = MT.Move(ref VAR.gsys_set.bquit, ref xt.ax_x, st_pos_Frcam_cap.x, ref xt.ax_y, st_pos_Frcam_cap.y);
                    ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref xt.ax_y, st_pos_Frcam_cap.y,true);
                    if (ret != EM_RES.OK) return;
                    break;
                case "btn_affine_upcam_movpos"://上相机(目标相机)拍照位置定位
                    ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref xt.ax_x, st_pos_Tocam_cap.x, ref xt.ax_y, st_pos_Tocam_cap.y,true);
                    if (ret != EM_RES.OK) return;
                    break;
            }
        }

        private void btn_affine_getpos_Click(object sender, EventArgs e)
        {
            string name = ((Button)sender).Name;
            switch (name)
            {
                case "btn_affine_place_getpos": //放料位置学习
                    st_pos_place.x = xt.ax_x.fenc_pos;
                    st_pos_place.y = xt.ax_y.fenc_pos;
                    st_pos_place.z = xt.ax_z.fenc_pos;
                    st_pos_place.a = udl.traybox_fd.ax_x.fenc_pos;
                    break;
                case "btn_affine_downcam_getpos"://下相机(源相机)拍照位置学习
                    st_pos_Frcam_cap.x = xt.ax_x.fenc_pos;
                    st_pos_Frcam_cap.y = xt.ax_y.fenc_pos;
                    //st_pos_Frcam_cap.y = xt.ax_y.fenc_pos;
                    
                    break;
                case "btn_affine_upcam_getpos"://上相机(目标相机)拍照位置学习
                    st_pos_Tocam_cap.x = xt.ax_x.fenc_pos;
                    st_pos_Tocam_cap.y = xt.ax_y.fenc_pos;
                    break;
            }
            ShowData();
        }

        public EM_RES MoveCam(Cam cam,ST_XY gopos,ref List<ST_XY> list_affine_vsdat,ref ST_XY cenpos)
        {
            EM_RES ret;
            ST_XYZA StMovPos = new ST_XYZA();
            //拍照任务   
            VAR.gsys_set.bquit = false;
            GetData();
            //定位         
            MT.SetAllAxToManualSpd();

            //ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref xt.ax_x, gopos.x, ref xt.ax_y, gopos.y,true);
            ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref xt.ax_y, gopos.y, true);
            if (ret != EM_RES.OK) return ret;
            //ret = MT.Move(ref VAR.gsys_set.bquit, ref xt.ax_z, gopos.z);
            //if (ret != EM_RES.OK) return ret;
            StMovPos = gopos.ToXYZA();
            list_affine_vsdat.Clear();
            ret= cam.TrigerAndGetNPoint(ref VAR.gsys_set.bquit,ref StMovPos, ref list_affine_vsdat);
            if (ret != EM_RES.OK) return ret;
            cenpos = StMovPos.ToXY();
            return EM_RES.OK;
        }

        private void Btn_affine_downcam_action_Click(object sender, EventArgs e)
        {
            ST_XY cenpos = new ST_XY();
            MoveCam(Frcam, st_pos_Frcam_cap, ref list_affine_vsdat_Frcam, ref cenpos);
        }

        private void btn_affine_place_action_Click(object sender, EventArgs e)
        {
            VAR.gsys_set.bquit = false;
            GetData();
            MT.SetAllAxToManualSpd();
            ST_XYZA gopos = st_pos_place;
            gopos.a = 0;
            EM_RES ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref udl.traybox_fd.ax_x, st_pos_place.a, true);
            if (ret != EM_RES.OK) return;
            xt.PickOrPlace(ref VAR.gsys_set.bquit, gopos, false);
        }

        public EM_RES affine_cail(ref Cam.AffineTransform AffineTransform,ref ST_XY cenpos)
        {
            //定位拍照
            EM_RES ret = MoveCam(Tocam, st_pos_Tocam_cap, ref list_affine_vsdat_Tocam,ref cenpos);
            if (ret != EM_RES.OK) return ret;

            ret = Frcam.AffineCailAction(list_affine_vsdat_Frcam, list_affine_vsdat_Tocam, ref AffineTransform,string.Format("{0}TO{1}",Frcam.mName,Tocam.mName));
            return ret;
        }
         
        private void btn_affine_cail_action_Click(object sender, EventArgs e)
        {
            EM_RES ret;
            ST_XY cenpos = new ST_XY();
            //定位拍照取数据
            Cam.AffineTransform AffineTransform = new Cam.AffineTransform();
            ret = affine_cail(ref AffineTransform,ref cenpos);
            if (ret == EM_RES.OK)
            {
                st_pos_Tocam_cap.x = cenpos.x;
                st_pos_Tocam_cap.y = cenpos.y;
                nud_affine_upcam_x.Value=(decimal)st_pos_Tocam_cap.x;
                nud_affine_upcam_y.Value = (decimal)st_pos_Tocam_cap.y;
                COM.CamLoadCailTool();
                Frcam.GetAllCaliToolXYAScale(ref st_stransform);
                ShowData(false, AffineTransform.NPointToNPointTool.Calibration.ComputedRMSError);
                btn_affine_save_action_Click(null, null);
                MessageBox.Show(VAR.IsChinese?Frcam.disc+"统一校正成功!": Frcam.englishdisc+ "Unified calibration succeeded!\r\n统一校正成功!", VAR.IsChinese?"提示": "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }

            else
            {
                MessageBox.Show(VAR.IsChinese ? Frcam.disc + "统一校正失败!": Frcam.englishdisc + "Unified calibration failed\r\n统一校正失败!", VAR.IsChinese ? "警告":"Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btn_affine_save_action_Click(object sender, EventArgs e)
        {
            GetData(true);
            foreach (XT Xt in udl.list_xt)
            {
                Xt.SaveHwCfg(VAR.gsys_set.cur_product_name);
                Xt.LoadCfg(VAR.gsys_set.cur_product_name);
            }
            ShowData(true);
        }

        private void btn_zk_ctrl_Click(object sender, EventArgs e)
        {
            if (xt.cy_zk.isOFF)
                xt.cy_zk.SetOn();
            else
                xt.cy_zk.SetOff();
        }
    }
}
