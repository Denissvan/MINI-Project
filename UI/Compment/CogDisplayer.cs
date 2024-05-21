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
using System.IO;

namespace UI
{    
    public partial class CogDisplayer : UserControl
    {
        public List<Cam> list_cam = new List<Cam>();
        public string curImgFile = "";
        Cam mcurCam = null;
        public bool TestOrGo = false;
        /// <summary>
        /// 获取当前相机
        /// </summary>
        public Cam curCam
        {
            get
            {
                if (cb_cam.SelectedIndex >= 0 && cb_cam.SelectedIndex < list_cam.Count)
                {
                    mcurCam = list_cam.ElementAt(cb_cam.SelectedIndex);

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
                if (cb_flow.SelectedIndex >= 0 && cb_flow.SelectedIndex < mcurCam.List_vs_task.Count)
                {
                    return mcurTask = mcurCam.List_vs_task.ElementAt(cb_flow.SelectedIndex);
                }
                return mcurTask =null;
            }
        }
        
        public CogDisplayer()
        {
            InitializeComponent();
            statusbar.Display = cogRecordDisplay;
            cogRecordDisplay.PopupMenu = false;
        }

        public void UpdateComBox()
        {
            string cur_sel = cb_cam.Text;
            cb_cam.Items.Clear();
            foreach (Cam cam in list_cam) cb_cam.Items.Add(VAR.IsChinese?cam.disc:cam.englishdisc);
            if (cb_cam.Items.Count > 0)
            {
                int idx = cb_cam.Items.IndexOf(cur_sel);
                if (idx < 0) idx = 0;
                cb_cam.SelectedIndex = idx;
            }
            mcurCam = curCam;
            cb_cam_SelectedIndexChanged(null, null);
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
                    AddCam(cam);
                }
            }
        }

        public void PmsEn(bool En=false)
        {
            btn_show_tool.Enabled = En;
        }

        private void cb_cam_SelectedIndexChanged(object sender, EventArgs e)
        {
            mcurCam = curCam;
            if (mcurCam == null) return;

            cb_flow.Items.Clear();
            foreach (Cam.VisionTask task in mcurCam.List_vs_task)
            {
                cb_flow.Items.Add(task.TaskName);
            }
            if (cb_flow.Items.Count > 0)
                cb_flow.SelectedIndex = 0;

            //update capcfg
            //if (ts_param.Visible)
            {
                stb_exposure.Text = curCam.isLive ? curCam.liveCapCfg.Exposure.ToString("F3") : curCam.curCapCfg.Exposure.ToString("F3");
                stb_brightness.Text = curCam.curCapCfg.Brightness.ToString("F3");
                stb_contrast.Text = curCam.curCapCfg.Contrast.ToString("F3");
                tsb_continue.BackColor = curCam.bcontinue ? Color.Gold : SystemColors.ButtonFace;
                TestOrGo = false;
                tsb_TestOrGo.Text = VAR.IsChinese?"定位模式": "Positioning";
                tsb_TestOrGo.BackColor = Color.Transparent;

            }
        }

        private void btn_live_Click(object sender, EventArgs e)
        {
            tsb_continue.BackColor = curCam.bcontinue ? Color.Gold : SystemColors.ButtonFace;
            if (mcurCam == null)
            {
                MessageBox.Show(VAR.IsChinese?"相机为空，请先选择相机!": "Camera is empty, please select a camera first!\r\n相机为空，请先选择相机!");
                return;
            }
            mcurCam.Live(cogRecordDisplay, !curCam.isLive, true);
            if(curCam.isLive)
            {
                btn_live.BackColor = Color.Gold;
                btn_img.BackColor = SystemColors.ButtonFace;
            }
            else
            {
                btn_live.BackColor = SystemColors.ButtonFace;
            }
        }

        private void btn_triger_Click(object sender, EventArgs e)
        {
            tsb_continue.BackColor = curCam.bcontinue ? Color.Gold : SystemColors.ButtonFace;
            if (mcurCam == null)
            {
                MessageBox.Show(VAR.IsChinese?"相机为空，请先选择相机!": "Camera is empty, please select a camera first!\r\n相机为空，请先选择相机!");
                return;
            }            
            if(curTask == null)
            {
                MessageBox.Show(VAR.IsChinese?"流程为空，请先选择流程!": "The process is empty, please select the process first!\r\n流程为空，请先选择流程!");
                return;
            }
            curTask.ResData.Clear();
            mcurCam.List_vs_task_cur.Clear();
            mcurCam.inputImageCnt = 0;
            //curTask.ListCaliTool = new List<Cam.CaliTool>();
            mcurCam.List_vs_task_cur.Add(curTask);
            if (mcurCam.mAcqFifo != null && btn_img.BackColor != Color.Gold)
            {
                //get cap config
                double val = Convert.ToDouble(stb_exposure.Text);
                if (curCam != null && val > 0 && val < 1000)
                {
                    curCam.curCapCfg.Exposure = val;
                }
                else MessageBox.Show(VAR.IsChinese ? "曝光参数数据异常!\r\n请检查数据格式及大小范围!": "The exposure parameter data is abnormal!\r\nPlease check the data format and size range!\r\n曝光参数数据异常!\r\n请检查数据格式及大小范围!");

                val = Convert.ToDouble(stb_brightness.Text);
                if (curCam != null && val >= 0 && val < 1)
                {
                    curCam.curCapCfg.Brightness = val;
                }
                else MessageBox.Show(VAR.IsChinese ? "亮度参数数据异常!\r\n请检查数据格式及大小范围!": "The brightness parameter data is abnormal!\r\nPlease check the data format and size range!");

                val = Convert.ToDouble(stb_contrast.Text);
                if (curCam != null && val >= 0 && val < 1)
                {
                    curCam.curCapCfg.Contrast = val;
                }
                else MessageBox.Show(VAR.IsChinese ? "对比参数数据异常!\r\n请检查数据格式及大小范围!": "The comparison parameter data is abnormal!\r\nPlease check the data format and size range!");

                mcurCam.mAcqFifo.Prepare();
                mcurCam.Live(cogRecordDisplay, false, true);
                mcurCam.Triger(mcurCam.bcontinue);

                btn_img.BackColor = SystemColors.ButtonFace;
                if (curCam.isLive)
                {
                    btn_live.BackColor = Color.Gold;
                }
                else
                {
                    btn_live.BackColor = SystemColors.ButtonFace;
                }
            }
            else
            {
                try
                {
                    if (curImgFile.Length < 3 || !Directory.Exists(Path.GetDirectoryName(curImgFile))) return;
                    string[] files = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(curImgFile), "*.bmp");
                    for (int n = 0; n < files.Count(); n++)
                    {
                        if (files[n] == curImgFile)
                        {
                            n++;
                            if (n >= files.Count()) n = 0;
                            curImgFile = files[n];
                            mcurCam.TrigerByLoadImg(curImgFile);
                            break;
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(VAR.IsChinese?string.Format("{0}触发{1}异常，{2}", mcurCam.disc, mcurTask.TaskName, ex.Message)
                        : string.Format("{0} trigger {2} err, {3}\r\n({1}触发{2}异常，{3})",mcurCam.englishdisc, mcurCam.disc, mcurTask.TaskName, ex.Message));
                }
            }
            //wait for done
            int timeout = 0;
            while(timeout++<20)
            {
                System.Threading.Thread.Sleep(50);
                Application.DoEvents();
                if(curTask.ResData.bUpdate)
                {
                    //string str = string.Format("{0}/{1}:{2},X:{3:F3},Y{4:F3},A{5:F3},T{6:F3}", curCam.disc,curTask.TaskName, curTask.ResData.bOK?"OK":"NG", 
                    //    curTask.ResData.PosMM.x, curTask.ResData.PosMM.y, curTask.ResData.PosMM.a, curTask.ResData.CTms);
                    //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, str);
                    break;
                }
            }
            if(timeout >=20) VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese?string.Format("{0}/{1} 超时！", curCam.disc, curTask.TaskName): string.Format("{0}/{2} timeout!            ({1}/{2} 超时！)",curCam.englishdisc, curCam.disc, curTask.TaskName));
        }

        private void btn_img_Click(object sender, EventArgs e)
        {
            tsb_continue.BackColor = curCam.bcontinue ? Color.Gold : SystemColors.ButtonFace;
            if (curCam == null || curTask == null) return;            
            if (((Button)sender).BackColor == SystemColors.ButtonFace)
            {               
                if (DialogResult.OK != openFileDlg.ShowDialog()) return;
                curImgFile = openFileDlg.FileName;
                ((Button)sender).BackColor = Color.Gold;
                mcurCam.TrigerByLoadImg(curImgFile);
            }
            else
            {
                ((Button)sender).BackColor = SystemColors.ButtonFace;
            }

            if (curCam.isLive)
            {
                btn_live.BackColor = Color.Gold;
            }
            else
            {
                btn_live.BackColor = SystemColors.ButtonFace;
            }
        }

        private void btn_show_tool_Click(object sender, EventArgs e)
        {
            ts_param.Visible = !ts_param.Visible;
            if (curCam != null && ts_param.Visible)
            {
                stb_exposure.Text = curCam.isLive ? curCam.liveCapCfg.Exposure.ToString("F3") : curCam.curCapCfg.Exposure.ToString("F3");
                stb_brightness.Text = curCam.curCapCfg.Brightness.ToString("F3");
                stb_contrast.Text = curCam.curCapCfg.Contrast.ToString("F3");
                tsb_continue.BackColor = curCam.bcontinue ? Color.Gold : SystemColors.ButtonFace;
            }
        }

        private void cogRecordDisplay_Click(object sender, EventArgs e)
        {
            ts_param.Visible = false;
           
        }

        private void tsb_block_edit_Click(object sender, EventArgs e)
        {
            if(curCam == null || curTask == null)
            {
                MessageBox.Show(VAR.IsChinese?"找不到相机或流程!\r\n请先选择相机，并选好要编辑的流程!": "Can't find the camera or process! \r\nPlease select the camera first and choose the process you want to edit!");
                return;
            }
            curTask.Load();
            using (Form f = new Form())
            { 
                CogBlockEidt fr = new CogBlockEidt();
                fr.Camera = curCam;
                fr.VsTask = curTask;
                fr.Dock = DockStyle.Fill;
                f.Controls.Add(fr);
                f.Size = new Size(800,600);
                f.Text = string.Format("{0}任务{1} {2}",curTask.Camera==null?"": curTask.Camera.disc,curTask.TaskName,curTask.path);
                f.ShowDialog();
            }
        }

        private void tsb_save_Click(object sender, EventArgs e)
        {
            if (curCam == null) return;
            try
            {
                Cam.ST_CAP_CFG cap_cfg = curCam.curCapCfg;
                double val = Convert.ToDouble(stb_exposure.Text);
                if (curCam != null && val > 0 && val < 1000)
                {
                    cap_cfg.Exposure = val;
                }
                else MessageBox.Show(VAR.IsChinese?"曝光参数数据异常!\r\n请检查数据格式及大小范围!": "The exposure parameter data is abnormal! \r\nPlease check the data format and size range!");
                val = Convert.ToDouble(stb_brightness.Text);
                if (curCam != null && val >= 0 && val < 1)
                {
                    cap_cfg.Brightness = val;
                }
                else MessageBox.Show(VAR.IsChinese?"亮度参数数据异常!\r\n请检查数据格式及大小范围!": "The brightness parameter data is abnormal! \r\nPlease check the data format and size range!");

                val = Convert.ToDouble(stb_contrast.Text);
                if (curCam != null && val >= 0 && val < 1)
                {
                    cap_cfg.Contrast = val;
                }
                else MessageBox.Show(VAR.IsChinese?"对比参数数据异常!\r\n请检查数据格式及大小范围!": "The comparison parameter data is abnormal! \r\nPlease check the data format and size range!");
                //save
                if (curCam.isLive) curCam.liveCapCfg = cap_cfg;
                else curCam.curCapCfg = cap_cfg;
                curCam.SaveCfg();
                MessageBox.Show(VAR.IsChinese ? "参数保存完成！": "Parameters are saved!\r\n参数保存完成！");
            }
            catch(Exception ex)
            {
                MessageBox.Show(VAR.IsChinese?"参数数据异常!\r\n请检查数据格式及大小范围!": "Parameter data is abnormal!\r\nPlease check the data format and size range!\r\n参数数据异常!\r\n请检查数据格式及大小范围!", ex.Message);
            }
        }

        Cognex.VisionPro.CogRectangle rct = new Cognex.VisionPro.CogRectangle();
        Cognex.VisionPro.CogGraphicLabel lable = new Cognex.VisionPro.CogGraphicLabel();
        Cognex.VisionPro.ICogTransform2D mCalToPixel;
        bool bmouseup = true;
        private void cogRecordDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            if (TestOrGo) return;
            if (mCalToPixel != null && bmouseup == false && e.X - rct.X > 0 && e.Y - rct.Y > 0)
            {
                rct.SetXYWidthHeight(rct.X, rct.Y, e.X - rct.X, e.Y - rct.Y);

                double x1 = 0;
                double y1 = 0;
                mCalToPixel.MapPoint(rct.X, rct.Y, out x1, out y1);
                double x2 = 0;
                double y2 = 0;
                mCalToPixel.MapPoint(rct.X + rct.Width, rct.Y + rct.Height, out x2, out y2);
                double a = Cognex.VisionPro.CogMisc.RadToDeg(Cognex.VisionPro.CogMath.AnglePointPoint(rct.X, rct.Y, rct.X + rct.Width, rct.Y + rct.Height));
                lable.Text = string.Format("W{0:F3} H{1:F3} R{2:F3}", x2 - x1, y2 - y1, a);

                try
                {
                    cogRecordDisplay.StaticGraphics.Remove("Measure");
                }
                catch
                { }
                cogRecordDisplay.StaticGraphics.Add(rct, "Measure");
                cogRecordDisplay.StaticGraphics.Add(lable, "Measure");
            }
        }

        private void cogRecordDisplay_MouseDown(object sender, MouseEventArgs e)
        {
           // double x = 0, y = 0;
            EM_RES res=EM_RES.OK;
            ST_XYZA pos=new ST_XYZA();
            if (!TestOrGo)
                //init rectangle
            {
                rct.X = e.X;
                rct.Y = e.Y;
                rct.SelectedSpaceName = "*";
                //rct.Color = Cognex.VisionPro.CogColorConstants.Orange;
                //init lable
                lable.Alignment = Cognex.VisionPro.CogGraphicLabelAlignmentConstants.BaselineLeft;
                lable.SelectedSpaceName = "*";
                lable.Font = new Font("Arial", 12);
                lable.LineWidthInScreenPixels = 3;
                lable.X = e.X;
                lable.Y = e.Y - lable.Font.SizeInPoints;
                //lable.Color = Cognex.VisionPro.CogColorConstants.Orange;
                try
                {
                    cogRecordDisplay.StaticGraphics.Remove("Measure");
                }
                catch
                {
                }
            }

            //get transform
            try
            {
                string spaceName = "#";
                if (cogRecordDisplay.Image.SelectedSpaceName.Contains("mm")) spaceName = "mm";
                    mCalToPixel = cogRecordDisplay.GetTransform(spaceName, "*");
                if (TestOrGo)
                {
                    VAR.gsys_set.bquit = false;
                    AXIS ax_x = null;
                    AXIS ax_y = null;
                    if (mcurCam.mName == "CamUp1")
                    {
                        ax_x = COM.UDLoad1.ax_x;
                        ax_y = COM.UDLoad1.ax_y;
                    }
                    else if (mcurCam.mName == "CamUp2")
                    {
                        ax_x = COM.UDLoad2.ax_x;
                        ax_y = COM.UDLoad2.ax_y;
                    }
                     
                    mCalToPixel.MapPoint(e.X, e.Y, out pos.x, out pos.y);
                    pos.x = ax_x.fenc_pos-pos.x;
                    pos.y = ax_y.fenc_pos-pos.y;
                    //定位                  
                    res =curCam.MoveHandle(ref VAR.gsys_set.bquit, true, true, false, false, false, ref pos, curCam.mName, 0);
                    if (res != EM_RES.OK) return;
                    //拍照
                    btn_triger_Click(null, null);
                }
                    
                bmouseup = false;
            }
            catch
            {
                mCalToPixel = null;
            }            
        }

        private void cogRecordDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            bmouseup = true;
        }

        private void cogRecordDisplay_DoubleClick(object sender, EventArgs e)
        {
            cogRecordDisplay.Fit();
        }

        private void cb_flow_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tsb_save_rawpic_Click(object sender, EventArgs e)
        {
            //if (cogRecordDisplay.Image == null) return;

            //saveFileDialog.Filter = "Jpeg|*.jpeg|Bmp|*.bmp";
            //if (DialogResult.OK != saveFileDialog.ShowDialog()) return;
            //mcurCam.SaveOriginImage(cogRecordDisplay.Image, Path.GetDirectoryName(saveFileDialog.FileName), Path.GetFileName(saveFileDialog.FileName));
            //mcurCam.ComShapeSave(curTask.ResData.ShapeOutput, "comshape.shp");
            //mcurCam.ComShapeLoad("comshape.shp");
            //mcurCam.ComShapeMove(0, 0, 0);
            
        }

        private void tsb_save_pic_Click(object sender, EventArgs e)
        {
            if (cogRecordDisplay.Image == null) return;
            DialogResult dlgres = MessageBox.Show(VAR.IsChinese?"是否要保存显示画面?\n\nYES---保存显示画面\nNO---保存原始图片": "Do you want to save the display? \n\nYES --- Save the display \nNO --- Save the original picture\r\n是否要保存显示画面?\n\nYES---保存显示画面\nNO---保存原始图片", VAR.IsChinese?"问询": "Inquire", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (dlgres == DialogResult.Cancel) return;

            saveFileDialog.Filter = "Jpeg|*.jpeg|Bmp|*.bmp";
            if (DialogResult.OK != saveFileDialog.ShowDialog()) return;

            if (dlgres == DialogResult.OK)
                mcurCam.SaveDisplay(cogRecordDisplay, Path.GetDirectoryName(saveFileDialog.FileName), Path.GetFileName(saveFileDialog.FileName));
            else
                mcurCam.SaveOriginImage(cogRecordDisplay.Image, Path.GetDirectoryName(saveFileDialog.FileName), Path.GetFileName(saveFileDialog.FileName));
        }

        private void tsb_continue_Click(object sender, EventArgs e)
        {
            if (curCam != null)
            {
                curCam.bcontinue = !curCam.bcontinue;
                ((ToolStripButton)sender).BackColor = curCam.bcontinue ? Color.Gold : SystemColors.ButtonFace;
            }
        }

        private void stb_exposure_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (curCam == null) return;
            //    double val = Convert.ToDouble(stb_exposure.Text);
            //    if(val <= 0 && val >= 1000)
            //    {
            //        MessageBox.Show("曝光值超范围!");
            //        return;
            //    }
            //    if (curCam.isLive)
            //    {
            //        curCam.liveCapCfg.Exposure = val;
            //        curCam.CapCfg(curCam.liveCapCfg);
            //    }
            //    else
            //    {
            //        curCam.curCapCfg.Exposure = val;
            //        curCam.CapCfg(curCam.curCapCfg);
            //    }
            //}
            //catch
            //{

            //}
        }

        private void tsb_TestOrGo_Click(object sender, EventArgs e)
        {
            if(mcurCam.mName== "CamDw1"|| mcurCam.mName == "CamDw2")return;
            if (TestOrGo)
            {
                TestOrGo = false;
                tsb_TestOrGo.Text = VAR.IsChinese?"定位模式": "Positioning";
                tsb_TestOrGo.BackColor = Color.Transparent;
            }
            else
            {
                TestOrGo = true;
                tsb_TestOrGo.Text = VAR.IsChinese ? "测量模式": "Measurement";
                tsb_TestOrGo.BackColor = Color.Orange;
            }

        }

        private void tsb_GoCenter_Click(object sender, EventArgs e)
        {
            if (mcurCam.mName == "CamDw1" || mcurCam.mName == "CamDw2") return;
            EM_RES res = EM_RES.OK;
            ST_XYZA CurPos = new ST_XYZA();
            VAR.gsys_set.bquit = false;
            if (DialogResult.Yes == MessageBox.Show(VAR.IsChinese?"是否画面对中?": "Is the screen aligned?\r\n是否画面对中?", VAR.IsChinese?"提示": "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                AXIS ax_x = null;
                AXIS ax_y = null;
                if (mcurCam.mName == "CamUp1")
                {
                    ax_x = COM.UDLoad1.ax_x;
                    ax_y = COM.UDLoad1.ax_y;
                }
                else if (mcurCam.mName == "CamUp2")
                {
                    ax_x = COM.UDLoad2.ax_x;
                    ax_y = COM.UDLoad2.ax_y;
                }
                CurPos.x = ax_x.fenc_pos;
                CurPos.y = ax_y.fenc_pos;
                curCam.MoveToImgCenter(ref VAR.gsys_set.bquit, ref CurPos, curTask);
            }

        }
    }
}
