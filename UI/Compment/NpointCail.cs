using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MotionCtrl;
using System.IO;



namespace UI.Compment
{
    public partial class NpointCail : UserControl
    {
      
       public ST_XYZ st_pos_place;
       public  ST_XYZ st_pos_up_cap;
       public double cap_step;
       public ST_XYA st_stransform;
        public List<UpDownLoad> list_UDLoad= new List<UpDownLoad>();
        public XT xt= new XT();
        UpDownLoad mcurUDLoad = new UpDownLoad();
        public Cam mcurCam = null;

        public NpointCail()
        {
            InitializeComponent();
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

        public UpDownLoad curUDload
        {
            get
            {
                if (cb_UDload.SelectedIndex >= 0 && cb_UDload.SelectedIndex < list_UDLoad.Count)
                {
                    mcurUDLoad = list_UDLoad.ElementAt(cb_UDload.SelectedIndex);
                    mcurCam = mcurUDLoad.upcam;
                    xt = mcurUDLoad.list_xt[0];
                }
                return mcurUDLoad;
            }
        }

        public void UpdateComBox()
        {
            string cur_sel = cb_UDload.Text;
            cb_UDload.Items.Clear();
            foreach (UpDownLoad ud in list_UDLoad)
            {
                cb_UDload.Items.Add(VAR.IsChinese?ud.disc:ud.englishdisc);
            }
                           
            if (cb_UDload.Items.Count > 0)
            {
                int idx = cb_UDload.Items.IndexOf(cur_sel);
                if (idx < 0) 
                cb_UDload.SelectedIndex = 0;
            }
            mcurUDLoad = curUDload;
            cb_cam_SelectedIndexChanged(null,null);
        }

        public void AddUDLoad(UpDownLoad ud)
        {
            bool IsDif = false;
            if (ud != null)
            {
                foreach (UpDownLoad _ud in list_UDLoad)
                {
                    if (ud.id == _ud.id)
                        IsDif = true;
                }
                if(!IsDif)
                list_UDLoad.Add(ud);
            }           
        }

        public void AddUDLoad(List<UpDownLoad> list_UDLoad)
        {
            if (list_UDLoad != null)
            {
                foreach (UpDownLoad _ud in list_UDLoad)
                {
                    AddUDLoad(_ud);
                }
                UpdateComBox();
            }
         }

        public void ShowData(bool bload = false,double rmse = -1)
        {
            if (bload)
            {               
                st_pos_place = COM.xt1.st_pos_unit_place;
                st_pos_up_cap = xt.st_pos_unit_upcam_cap;
                cap_step = xt.pos_unit_upcam_step;                                
            }

            nud_unit_place_x.Value = (decimal)st_pos_place.x;
            nud_unit_place_y.Value = (decimal)st_pos_place.y;
            nud_unit_place_z.Value = (decimal)st_pos_place.z;

            btn_unit_cam_x.Value = (decimal)st_pos_up_cap.x;
            btn_unit_cam_y.Value = (decimal)st_pos_up_cap.y;
            btn_unit_cam_z.Value = (decimal)st_pos_up_cap.z;

            nud_unit_step.Value = (decimal)cap_step;

            tb_result.Text = string.Format("Xs={0:F3} Ys={1:F3}  A={2:F3} {3} ", st_stransform.x, st_stransform.y, st_stransform.a,rmse>-1?string.Format("RMSE={0:F3}",rmse):"");
        }

        public void GetData(bool bload=false)
        {
            st_pos_place.x = (double)nud_unit_place_x.Value;
            st_pos_place.y = (double)nud_unit_place_y.Value;
            st_pos_place.z = (double)nud_unit_place_z.Value;
            st_pos_up_cap.x = (double)btn_unit_cam_x.Value;
            st_pos_up_cap.y = (double)btn_unit_cam_y.Value;
            //st_pos_up_cap.z = (double)btn_unit_cam_z.Value;
             cap_step = (double)nud_unit_step.Value;
            if (bload)
            {
                 COM.xt1.st_pos_unit_place= st_pos_place;
                 xt.st_pos_unit_upcam_cap= st_pos_up_cap;
                 xt.pos_unit_upcam_step= cap_step;
            }
        }

        private void Btn_unit_getpos_Click(object sender, EventArgs e)
        {
            string name = ((Button)sender).Name;
            switch (name)
            {
                case "Btn_unit_place_getpos": //放料位置学习
                    st_pos_place.x = COM.xt1.ax_x.fenc_pos;
                    st_pos_place.y = COM.xt1.ax_y.fenc_pos;
                    st_pos_place.z = COM.xt1.ax_z.fenc_pos;
                    break;
                case "btn_unit_cam_getpos"://拍照位置学习
                    st_pos_up_cap.x = xt.ax_x.fenc_pos;
                    st_pos_up_cap.y = xt.ax_y.fenc_pos;
                    //st_pos_up_cap.z = mcurUDLoad.traybox_fd.ax_x.fenc_pos;
                    break;
            }
            ShowData();
        }

        #region 九点校正 CamXYTCalb
        public  EM_RES CamXYTCalb(ref bool bquit,Cam cam, Cam.VisionTask task, double step)
        {
            EM_RES ret;
            VAR.gsys_set.bquit = false;
            Cam.AffineTransform NpointAF = new Cam.AffineTransform();
            ST_XY st_firstpos = new ST_XY();
            List<ST_XY> list_pos = new List<ST_XY>();
            ST_XY[] st_pos_p = new ST_XY[9];
            ST_XY[] st_pos_a = new ST_XY[9];
            AXIS ax = xt.ax_x;
            AXIS ay = xt.ax_y;
           
            st_firstpos.x = ax.fenc_pos;
            st_firstpos.y = ay.fenc_pos;
            list_pos = cam.MakeNineArray(st_firstpos, step);
            for (int n = 0; n < list_pos.Count;n++)
            {
                if (bquit) return EM_RES.QUIT;
                //定位
                 ret = MT.Move(ref bquit, ref ax, list_pos[n].x, ref ay, list_pos[n].y);
                if (ret != EM_RES.OK) return ret;
                Thread.Sleep(1000);
                Application.DoEvents();
                //拍照
                ret = task.TriAndWaitResult(ref bquit);
                if (ret != EM_RES.OK) return ret;
                //更新点位数据
                st_pos_p[n].x = task.ResData.PosPix.x;
                st_pos_p[n].y = task.ResData.PosPix.y;
                st_pos_a[n].x = ax.fenc_pos - st_firstpos.x;
                st_pos_a[n].y = ay.fenc_pos - st_firstpos.y;
            }
            //回到初始位置
            ret = MT.Move(ref bquit, ref ax, st_firstpos.x, ref ay, st_firstpos.y);
            if (ret != EM_RES.OK) return ret;

            ret= NpointAF.BuildTransform(st_pos_p, st_pos_a, 0.5);
            if (ret != EM_RES.OK) return ret;

            //save
            string path = string.Format("{0}\\syscfg\\Calibration\\{1}\\{2}_NPoint.vpp", Path.GetFullPath(".."),cam.mName,cam.mName);
          // NpointAF.Save(path);


            return EM_RES.OK;
        }
        #endregion

        private void Btn_unit_movpos_Click(object sender, EventArgs e)
        {
            EM_RES ret=EM_RES.OK;
            VAR.gsys_set.bquit = false;
            GetData();
            MT.SetAllAxToManualSpd();
            //if(Math.Abs(UploadModle.ax_z.fenc_pos)>0.5)
            //{
            //    ret = UploadModle.ZHome(ref VAR.gsys_set.bquit);
            //    if (ret != EM_RES.OK) return;
            //}         
           // ret = MT.Move(ref VAR.gsys_set.bquit, ref MT.AXIS_UDL1_Z, 0, ref MT.AXIS_UDL2_Z,0);
            if (ret != EM_RES.OK) return ;
            string name = ((Button)sender).Name;           
            switch (name)
            {
                case "Btn_unit_place_movpos": //放料位置定位
                    ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref COM.xt1.ax_x, st_pos_place.x, ref COM.xt1.ax_y, st_pos_place.y, true);
                    if (ret != EM_RES.OK) return;
                    break;
                case "btn_unit_cam_movpos"://拍照位置定位
                    ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref xt.ax_x, st_pos_up_cap.x, ref xt.ax_y, st_pos_up_cap.y, ref  mcurUDLoad.traybox_fd.ax_x, st_pos_up_cap.z,true);
                    if (ret != EM_RES.OK) return;
                    break;
            }
        }

        private void Btn_unit_place_action_Click(object sender, EventArgs e)
        {
            VAR.gsys_set.bquit = false;
            GetData();
            MT.SetAllAxToManualSpd();
            xt.PickOrPlace(ref VAR.gsys_set.bquit, st_pos_place.ToXYZA(),false);
        }

        private void btn_unit_cali_action_Click(object sender, EventArgs e)
        {
            EM_RES ret = EM_RES.OK;
            VAR.gsys_set.bquit = false;
            ST_XYZA movpos = new ST_XYZA();
            Cam.AffineTransform NPointTool = new Cam.AffineTransform() ;
            VAR.gsys_set.bquit = false;
            GetData();
            //拍照任务
            mcurCam.List_vs_task_cur.Clear();
            mcurCam.inputImageCnt = 0;
            Cam.VisionTask task = mcurCam.List_vs_task.Find(s => s.TaskName.Equals("NpointTool"));
            mcurCam.List_vs_task_cur.Add(task);
            //移动到上相机拍照位置
            MT.SetAllAxToManualSpd();
           
            //ret = MT.Move(ref VAR.gsys_set.bquit, ref MT.AXIS_UDL1_Z, 0,ref MT.AXIS_UDL2_Z,0);
            //if (ret != EM_RES.OK) return ; 
            ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref xt.ax_x, st_pos_up_cap.x, ref xt.ax_y, st_pos_up_cap.y, ref  mcurUDLoad.traybox_fd.ax_x, st_pos_up_cap.z, true);
            if (ret != EM_RES.OK) return;


           // movpos.x = st_pos_up_cap.x;
            movpos = st_pos_up_cap.ToXYZA();
            ST_XY st_cenpos=new ST_XY();
            ret = mcurCam.NinePointCaliAction(ref VAR.gsys_set.bquit, ref NPointTool,ref st_cenpos, movpos,ref task,"mm", cap_step);
            if (ret == EM_RES.OK)
            {
                //保存对中坐标
                st_pos_up_cap.x = st_cenpos.x;
                st_pos_up_cap.y = st_cenpos.y;
                foreach (XT _xt in COM.ListXT)
                {
                    if (mcurUDLoad.id < _xt.st_pos_samevs.Length)
                        _xt.st_pos_samevs[mcurUDLoad.id] = st_cenpos;
                    _xt.SaveHwCfg();
                }                       
                COM.CamLoadCailTool();
                mcurCam.GetAllCaliToolXYAScale(ref st_stransform);              
                xt.LoadCfg(VAR.gsys_set.cur_product_name);
                ShowData(false, NPointTool.NPointToNPointTool.Calibration.ComputedRMSError);
                // ShowData(true);
                MessageBox.Show(VAR.IsChinese?"九点校正成功!": "Nine-point correction succeeded!\r\n九点校正成功!", VAR.IsChinese?"提示": "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
                
            else
            {
                MessageBox.Show(VAR.IsChinese ? "九点校正失败!": "Nine-point calibration failed!\r\n九点校正失败!", VAR.IsChinese?"警告":"Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            mcurCam.graphics_other.Clear();
        }

        private void btn_save_action_Click(object sender, EventArgs e)
        {
            GetData(true);
            if (xt.id != COM.xt1.id)
            {
                COM.xt1.SaveHwCfg();
                COM.xt1.LoadCfg(VAR.gsys_set.cur_product_name);
            }
            xt.SaveHwCfg();
            xt.LoadCfg(VAR.gsys_set.cur_product_name);
            ShowData(true);
        }

        private void btn_cam_Click(object sender, EventArgs e)
        {
            VAR.gsys_set.bquit = false;
            //拍照任务            
            mcurCam.List_vs_task_cur.Clear();
            mcurCam.inputImageCnt = 0;
            Cam.VisionTask task = mcurCam.List_vs_task.Find(s => s.TaskName.Equals("NpointTool"));
            mcurCam.List_vs_task_cur.Add(task);
            EM_RES ret = task.TriAndWaitResult(ref VAR.gsys_set.bquit);
            if (ret != EM_RES.OK) return;
        }

        private void btn_zk_ctrl_Click(object sender, EventArgs e)
        {
            if (COM.xt1.cy_zk.isOFF)
                COM.xt1.cy_zk.SetOn();
            else
                COM.xt1.cy_zk.SetOff();
        }

        private void cb_cam_SelectedIndexChanged(object sender, EventArgs e)
        {
            mcurUDLoad = curUDload;
            //lbl_unit_cam.Text = mcurUDLoad.upcam.disc + "拍照位:";
            mcurCam.GetAllCaliToolXYAScale(ref st_stransform);
            ShowData(true);
        }
    }
}
