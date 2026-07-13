using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MotionCtrl;
using System.IO;

namespace UI.Compment
{
    public partial class TaskNpoint : UserControl
    {
        public TaskNpoint()
        {
            InitializeComponent();
        }

        public ST_XY st_pos_up_cap;
        public double cap_step=5.00;
        public List<Cam> list_cam = new List<Cam>();
        
        Cam mcurCam = null;
        private UpDownLoad udmod=null;
        /// <summary>
        /// 获取当前相机
        /// </summary>
        public Cam curCam
        {
            get
            {
                if (cb_upcamload.SelectedIndex >= 0 && cb_upcamload.SelectedIndex < list_cam.Count)
                {
                    mcurCam = list_cam.ElementAt(cb_upcamload.SelectedIndex);
                    udmod = COM.List_UDLoad.Find(s => s.upcam.mName.Equals(mcurCam.mName));

                }
                return mcurCam;
            }
        }

        Cam.VisionTask mcurTask;
        /// <summary>
        /// 获取当前视觉任务
        /// </summary>
        public Cam.VisionTask curTask
        {
            get
            {
                mcurCam = curCam;
                if (mcurCam == null) return mcurTask = null;
                if (cb_taskload.SelectedIndex >= 0 && cb_taskload.SelectedIndex < mcurCam.List_vs_task.Count)
                {
                    return mcurTask = mcurCam.List_vs_task.ElementAt(cb_taskload.SelectedIndex);
                }
                return mcurTask = null;
            }
        }

        public void UpdateComBox()
        {
            string cur_sel = cb_upcamload.Text;
            cb_upcamload.Items.Clear();
            foreach (Cam cam in list_cam) cb_upcamload.Items.Add(VAR.IsChinese?cam.disc:cam.englishdisc);
            if (cb_upcamload.Items.Count > 0)
            {
                int idx = cb_upcamload.Items.IndexOf(cur_sel);
                if (idx < 0) idx = 0;
                cb_upcamload.SelectedIndex = idx;
            }
            mcurCam = curCam;
            cb_upcamload_SelectedIndexChanged(null, null);
        }


        public void AddCam(Cam camera)
        {
         
            if (camera != null)
            {
                foreach (Cam cam in list_cam)
                {
                    if (cam.mName == camera.mName) return;
                }

                list_cam.Add(camera);
            }

            UpdateComBox();
        }

        public void AddCam(List<Cam> listCam)
        {
            if (listCam != null)
            {
                foreach (Cam cam in listCam)
                {
                    if(cam.mName.Contains("CamUp"))
                    AddCam(cam);
                }
            }
        }

        public void ShowData()
        {
            nud_task_cam_x.Value = (decimal)st_pos_up_cap.x;
            nud_task_cam_y.Value = (decimal)st_pos_up_cap.y;
        }

        public void GetData()
        {
            st_pos_up_cap.x=(double)nud_task_cam_x.Value;
            st_pos_up_cap.y = (double) nud_task_cam_y.Value;
            cap_step = (double) nud_task_step.Value;
        }

        private void btn_task_cali_action_Click(object sender, EventArgs e)
        {
            EM_RES ret = EM_RES.OK;
            VAR.gsys_set.bquit = false;
            ST_XYZA movpos = new ST_XYZA();
            ST_XY st_cenpos = new ST_XY();
            Cam.AffineTransform NPointTool = new Cam.AffineTransform();
            Cam.VisionTask tsk=new Cam.VisionTask();
            VAR.gsys_set.bquit = false;
            
            GetData();
            //移动到上相机拍照位置
            MT.SetAllAxToManualSpd();

            ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref udmod.ax_x, st_pos_up_cap.x, ref udmod.ax_y, st_pos_up_cap.y,true);
            if (ret != EM_RES.OK) return;
            // movpos.x = st_pos_up_cap.x;
            movpos = st_pos_up_cap.ToXYZA();
            tsk = curTask;
            ret = mcurCam.NinePointCaliAction(ref VAR.gsys_set.bquit, ref NPointTool, ref st_cenpos, movpos, ref tsk,tsk.TaskName+"_Npoint", cap_step,Cam.TaskNpointCail);
            if (ret == EM_RES.OK)
            {
                MessageBox.Show(VAR.IsChinese?string.Format("九点校正成功,RMSE:{0:f3}!", NPointTool.NPointToNPointTool.Calibration.ComputedRMSError): string.Format("Nine-point calibration successful,RMSE:{0:f3}!\r\n九点校正成功,RMSE:{0:f3}!", NPointTool.NPointToNPointTool.Calibration.ComputedRMSError), VAR.IsChinese?"提示": "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }

            else
            {
                MessageBox.Show(VAR.IsChinese ? string.Format("九点校正失败,RMSE:{0:f3}!", NPointTool.NPointToNPointTool.Calibration.ComputedRMSError): string.Format("Nine-point calibration failed,RMSE:{0:f3}!\r\n九点校正失败,RMSE:{0:f3}!", NPointTool.NPointToNPointTool.Calibration.ComputedRMSError), VAR.IsChinese?"警告":"Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            mcurCam.graphics_other.Clear();
        }

        private void cb_upcamload_SelectedIndexChanged(object sender, EventArgs e)
        {
            mcurCam = curCam;
            if (mcurCam == null) return;

            cb_taskload.Items.Clear();
            foreach (Cam.VisionTask task in mcurCam.List_vs_task)
            {
                cb_taskload.Items.Add(task.TaskName);
            }

            if (cb_taskload.Items.Count > 0)
                cb_taskload.SelectedIndex = 0;
        }

        private void btn_task_cam_getpos_Click(object sender, EventArgs e)
        {
            st_pos_up_cap.x = udmod.ax_x.fenc_pos;
            st_pos_up_cap.y = udmod.ax_y.fenc_pos;
            ShowData();
        }

        private void btn_task_cam_movpos_Click(object sender, EventArgs e)
        {
            EM_RES ret = EM_RES.OK;
            VAR.gsys_set.bquit = false;
            GetData();
            MT.SetAllAxToManualSpd();
            ret = MT.ZupMove(ref VAR.gsys_set.bquit, ref udmod.ax_x, st_pos_up_cap.x, ref udmod.ax_y, st_pos_up_cap.y,true);
            if (ret != EM_RES.OK) return;
         }
    }
}
