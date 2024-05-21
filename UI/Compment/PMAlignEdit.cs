using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using MotionCtrl;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro.PMAlign;
using System.Reflection;
using System.Threading;


namespace UI.Compment
{
    public partial class PMAlignEdit : UserControl
    {
        public PMAlignEdit()
        {
            InitializeComponent();
            cmbTrainRegion.Items.Clear();
            cmbSearchRegion.Items.Clear();
            foreach (VisionToolRegionShape Shape in Enum.GetValues(typeof(VisionToolRegionShape)))
            {
                this.cmbTrainRegion.Items.Add(GetEnumDescription(Shape));
                this.cmbSearchRegion.Items.Add(GetEnumDescription(Shape));
            }

            EditEnable(false);
        }
        private PMAlignTool pt=new PMAlignTool();
        public CogRecordDisplay dipyMod;
        public MaskDisplay disyMask;
        
        public List<Cam> list_cam = new List<Cam>();
        public string curImgFile = "";
        Cam mcurCam = null;
   
        //初始化显示
        public void InitDisPlay(CogRecordDisplay dipyMod, MaskDisplay disyMask)
        {
            this.dipyMod = dipyMod;
            this.disyMask = disyMask;
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
        /// <summary>
        /// 获取当前相机
        /// </summary>
        public Cam curCam
        {
            get
            {
                if (cmb_CamSel.SelectedIndex >= 0 && cmb_CamSel.SelectedIndex < list_cam.Count)
                {
                    mcurCam = list_cam.ElementAt(cmb_CamSel.SelectedIndex);

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
                if (cmb_TskSel.SelectedIndex >= 0 && cmb_TskSel.SelectedIndex < mcurCam.List_vs_task.Count)
                {
                     mcurTask = mcurCam.List_vs_task.ElementAt(cmb_TskSel.SelectedIndex);
                   // cb_tsk_SelectedIndexChanged(null, null);
                    return mcurTask;
                }
                return mcurTask = null;
            }
        }

        CogPMAlignTool mcurPMAlign;
        private List<CogPMAlignTool> ListPMAlign = new List<CogPMAlignTool>();
        /// <summary>
        /// 获取当前视觉任务
        /// </summary>
        public CogPMAlignTool curPMAlign
        {
            get
            {
                mcurTask = curTask;
                if (mcurTask == null) return mcurPMAlign = null;
                if (cmb_PmaSel.SelectedIndex >= 0 && cmb_PmaSel.SelectedIndex < ListPMAlign.Count)
                {
                     mcurPMAlign = ListPMAlign.ElementAt(cmb_PmaSel.SelectedIndex);
                    
                     return mcurPMAlign;
                }
                return mcurPMAlign = null;
            }
        }

        public void UpdateComBox()
        {
            string cur_sel = cmb_CamSel.Text;
            cmb_CamSel.Items.Clear();
            foreach (Cam cam in list_cam) cmb_CamSel.Items.Add(cam.disc);
            if (cmb_CamSel.Items.Count > 0)
            {
                int idx = cmb_CamSel.Items.IndexOf(cur_sel);
                if (idx < 0) idx = 0;
                cmb_CamSel.SelectedIndex = idx;
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

        public void PmsEn(bool En = false)
        {
           // btn_show_tool.Enabled = En;
        }

        private void cb_cam_SelectedIndexChanged(object sender, EventArgs e)
        {
            mcurCam = curCam;
            if (mcurCam == null) return;

            cmb_TskSel.Items.Clear();
            foreach (Cam.VisionTask task in mcurCam.List_vs_task)
            {
                cmb_TskSel.Items.Add(task.TaskName);
            }
            if (cmb_TskSel.Items.Count > 0)
                cmb_TskSel.SelectedIndex = 0;
        }

        private void cb_tsk_SelectedIndexChanged(object sender, EventArgs e)
        {
            mcurTask = curTask;
            if (mcurTask == null) return;

            cmb_PmaSel.Items.Clear();
            ListPMAlign.Clear();
            for (int i = 0; i < mcurTask.Block.Tools.Count; i++)
            {
                if (mcurTask.Block.Tools[i].Name.Contains("CogPMAlignTool"))
                {
                    ListPMAlign.Add((CogPMAlignTool)mcurTask.Block.Tools[i]);
                    cmb_PmaSel.Items.Add(mcurTask.Block.Tools[i].Name);
                }              
            }   
            if (cmb_PmaSel.Items.Count > 0)
                cmb_PmaSel.SelectedIndex = 0;
        }

        private void cb_PMA_SelectedIndexChanged(object sender, EventArgs e)
        {
            CogPMAlignTool _mcurPMAlign = mcurPMAlign;
            mcurPMAlign = curPMAlign;
            if (mcurPMAlign != _mcurPMAlign)
            {
                pt.ClearAll(dipyMod);
                try
                {
                    if(pt.InputImage!=null)
                    dipyMod.Image = pt.InputImage;
                }
                catch 
                {
                }
                EditEnable(false);
                pt.InitPMAlignTool(mcurPMAlign);
                ShowPMAlignData(false);
                dipyMod.BringToFront();
            }
           
        }

        public string GetEnumDescription(Enum enumValue)
        {
            string value = enumValue.ToString();
            FieldInfo field = enumValue.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);    //获取描述属性
            if (objs == null || objs.Length == 0)    //当描述属性没有时，直接返回名称
                return value;
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
            return descriptionAttribute.Description;
        }

        public void ShowPMAlignData(bool IsDisMod=true)
        {

            this.chkPolarity.Checked = pt.IgnorePolarity;
            this.Chk_ScoreUsingClutter.Checked = pt.UsingClutter;
            this.NM_Find_Num.Value = pt.NumberToFind;
            this.NM_Accept_Threshold.Value = (decimal)pt.AcceptThreshold;
            NM_Angle_Max.Value =(decimal) pt.m_Angle.High;
            NM_Angle_Min.Value = (decimal)pt.m_Angle.Low;
            NM_Angle_Default.Value = (decimal) pt.m_Angle.Nominal;
            NM_Scale_Max.Value = (decimal)pt.m_Scale.High;
            NM_Scale_Min.Value = (decimal)pt.m_Scale.Low;
            NM_Scale_Default.Value = (decimal)pt.m_Scale.Nominal;
            nud_Origin_X.Value = (decimal)pt.GetTool().Pattern.Origin.TranslationX;
            nud_Origin_Y.Value = (decimal)pt.GetTool().Pattern.Origin.TranslationY;
            Btn_Angle.Text = pt.m_Angle.Cfg == CogPMAlignZoneConstants.Nominal? "<" : ">";
            Btn_Scale.Text = pt.m_Scale.Cfg == CogPMAlignZoneConstants.Nominal? "<" : ">";
            cmbTrainRegion.SelectedItem = GetEnumDescription(pt.PatternShape);
            cmbSearchRegion.SelectedItem = GetEnumDescription(pt.SearchShape);
            pt.ShowPattern(DisplayTrainedPatternImage);
            // VisionRun.Display.Image = mPMAlignTool.GetTool().Pattern.TrainImageMask;        
            if (IsDisMod)
            {
                dipyMod.BringToFront();
                pt.ShowTrainImage(dipyMod,true,true,false);
               // dipyMod.Fit(true);
            }
           
        }

        public void GetPMAlignData()
        {
            pt.IgnorePolarity=this.chkPolarity.Checked;
            pt.UsingClutter= this.Chk_ScoreUsingClutter.Checked;
            pt.NumberToFind=(int) this.NM_Find_Num.Value;
            pt.AcceptThreshold = (double)this.NM_Accept_Threshold.Value;
            pt.m_Angle.Nominal= (double)NM_Angle_Default.Value;
            pt.m_Angle.High=(double) NM_Angle_Max.Value;
            pt.m_Angle.Low=(double)NM_Angle_Min.Value;
            pt.m_Scale.Nominal = (double)NM_Scale_Default.Value;
            pt.m_Scale.High= (double)NM_Scale_Max.Value;
            pt.m_Scale.Low=(double)NM_Scale_Min.Value;
            //pt.GetTool().Pattern.Origin.TranslationX=(double) nud_Origin_X.Value;
            //pt.GetTool().Pattern.Origin.TranslationY= (double)nud_Origin_Y.Value;
            pt.m_Angle.Cfg = Btn_Angle.Text.Contains("<") ? CogPMAlignZoneConstants.Nominal : CogPMAlignZoneConstants.LowHigh;
            pt.m_Scale.Cfg = Btn_Scale.Text.Contains("<") ? CogPMAlignZoneConstants.Nominal : CogPMAlignZoneConstants.LowHigh;
            pt.SetPMAlignParam();
        }

        public void EditEnable(bool En = true)
        {
            Btn_Origin.Enabled = En;
            cmbSearchRegion.Enabled = En;
            cmbTrainRegion.Enabled = En;
            btn_mask.Enabled = En;
            btn_train.Enabled = En;
            //btn_Capture.Enabled = En;
           
        }

        private void Btn_AngleOrScale_Click(object sender, EventArgs e)
        {
            Button btn = (Button) sender;
            if (btn.Text.Contains("<"))
            {
                btn.Text = ">";
            }
            else btn.Text = "<";
        }

        private void CmbPatMaxRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if(!comboBox.Enabled)return;
            List<ICogGraphicInteractive> IcogInter = new List<ICogGraphicInteractive>();
            if (dipyMod.InteractiveGraphics != null && dipyMod.InteractiveGraphics.Count > 0)
            {
                for(int i=0;i< dipyMod.InteractiveGraphics.Count;i++)
                {
                  
                    IcogInter.Add(dipyMod.InteractiveGraphics[i]);
                }
            }
            dipyMod.InteractiveGraphics.Clear();
            
            switch (comboBox.Name)
            {
                case "cmbTrainRegion":
                    if (dipyMod.Image != null)
                    {
                       // if (!bInitialize) pt.Untrain(TrainPattermDisplay);
                        switch (cmbTrainRegion.SelectedIndex)
                        {
                            case 0:     
                                  pt.GrabTrainRegion(dipyMod, VisionToolRegionShape.EntireImage,false);                                
                                break;
                            case 1:
                                    pt.GrabTrainRegion(dipyMod, VisionToolRegionShape.Circle, false);
                                break;
                            case 2:
                                    pt.GrabTrainRegion(dipyMod, VisionToolRegionShape.Ellipse, false);
                                break;
                            case 3:
                                    pt.GrabTrainRegion(dipyMod, VisionToolRegionShape.Rectangle, false);
                                break;
                            case 4:
                                    pt.GrabTrainRegion(dipyMod, VisionToolRegionShape.RectangleAffine, false);
                                break;
                            case 5:
                                    pt.GrabTrainRegion(dipyMod, VisionToolRegionShape.CircularAnnulusSection, false);
                                break;
                            case 6:
                                    pt.GrabTrainRegion(dipyMod, VisionToolRegionShape.EllipticalAnnulusSection, false);
                                break;
                        }

                        ICogGraphicInteractive search = IcogInter.Find(s => s.TipText.Equals("Region To Search"));
                        if(search!=null )
                        dipyMod.InteractiveGraphics.Add(search, "", false);

                    }
                    break;
                case "cmbSearchRegion":
                    if (dipyMod.Image != null)
                    {
                        switch (cmbSearchRegion.SelectedIndex)
                        {
                            case (int)VisionToolRegionShape.EntireImage:
                                    pt.GrabSearchRegion(dipyMod, VisionToolRegionShape.EntireImage,false);
                                break;
                            case 1:
                                    pt.GrabSearchRegion(dipyMod, VisionToolRegionShape.Circle, false);
                                break;
                            case 2:
                                    pt.GrabSearchRegion(dipyMod, VisionToolRegionShape.Ellipse, false);
                                break;
                            case 3:
                                    pt.GrabSearchRegion(dipyMod, VisionToolRegionShape.Rectangle, false);
                                break;
                            case 4:
                                    pt.GrabSearchRegion(dipyMod, VisionToolRegionShape.RectangleAffine, false);
                                break;
                            case 5:
                                    pt.GrabSearchRegion(dipyMod, VisionToolRegionShape.CircularAnnulusSection, false);
                                break;
                            case 6:
                                    pt.GrabSearchRegion(dipyMod, VisionToolRegionShape.EllipticalAnnulusSection, false);
                                break;
                        }
                        ICogGraphicInteractive train = IcogInter.Find(s => s.TipText.Equals("Region of Interest"));
                        ICogGraphicInteractive origin= IcogInter.Find(s => s.TipText.Equals("Pattern Origin"));
                        if(origin != null)
                            dipyMod.InteractiveGraphics.Add(IcogInter.Find(s => s.TipText.Equals("Pattern Origin")), "", false);
                        if(train != null )
                            dipyMod.InteractiveGraphics.Add(IcogInter.Find(s => s.TipText.Equals("Region of Interest")), "", false);
                    }
                    break;
            }
        }

        private void btn_ImgEdit_Click(object sender, EventArgs e)
        {
            try
            {
                pt.PatternEdit(dipyMod, pt.SearchShape, pt.PatternShape);
                EditEnable();
            }
            catch { }


        }

        private void btn_mask_Click(object sender, EventArgs e)
        {
            if (dipyMod.Image != null)
            {
                //2014.07.28
                if (pt.TrainMode == CogPMAlignTrainModeConstants.Image)
                {

                    disyMask.BringToFront();
                    disyMask.MaskImages(dipyMod.Image, pt.GetTool().Pattern.TrainImageMask, dipyMod, pt);
                }
            }
        }

        private void btn_train_Click(object sender, EventArgs e)
        {

                pt.Train(dipyMod, disyMask.MaskedImage);
                pt.ShowPattern(DisplayTrainedPatternImage);
                pt.ShowTrainImage(dipyMod, true, true, false, true);
            //nud_Origin_X.Value = (decimal)pt.GetTool().Pattern.Origin.TranslationX;
            //nud_Origin_Y.Value = (decimal)pt.GetTool().Pattern.Origin.TranslationY;
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                if (mcurTask != null)
                {
                    if (DialogResult.Yes != MessageBox.Show(VAR.IsChinese?"确定要保存?": "Are you sure you want to save?\r\n确定要保存?", VAR.IsChinese?"提示": "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;
                    GetPMAlignData();
                    mcurTask.Save();
                    Thread.Sleep(300);
                     mcurTask.Load();
                    ShowPMAlignData();
                    EditEnable(false);
                    cb_tsk_SelectedIndexChanged(null, null);
                }
            }
            catch { }

        }

        private void btn_Capture_Click(object sender, EventArgs e)
        {

            try
            {
                pt.PatternEdit(dipyMod, pt.SearchShape, pt.PatternShape, false);
                EditEnable(true);
            }
            catch{ }
               
        }

        private void btn_vpp_Click(object sender, EventArgs e)
        {
            if (curCam == null || curTask == null)
            {
                MessageBox.Show(VAR.IsChinese?"找不到相机或流程!\r\n请先选择相机，并选好要编辑的流程!": "Can't find the camera or process! \r\n Please select the camera first and choose the process you want to edit!\r\n找不到相机或流程!\r\n请先选择相机，并选好要编辑的流程!");
                return;
            }
            using (Form f = new Form())
            {
                CogBlockEidt fr = new CogBlockEidt();
                fr.Camera = curCam;
                fr.VsTask = curTask;
                fr.Dock = DockStyle.Fill;
                f.Controls.Add(fr);
                f.Size = new Size(800, 600);
                f.Text = string.Format("{0}任务{1} {2}", curTask.Camera == null ? "" : curTask.Camera.disc, curTask.TaskName, curTask.path);
                f.ShowDialog();
            }
        }

        private void Btn_Origin_Click(object sender, EventArgs e)
        {
            double OriginX = 0, OriginY = 0;
            switch (pt.PatternShape)
            {
                case VisionToolRegionShape.Circle:
                    CogCircle Circle = (CogCircle)pt.GetTool().Pattern.TrainRegion;
                    OriginX = Circle.CenterX;
                    OriginY = Circle.CenterY;
                    break;
                case VisionToolRegionShape.Ellipse:
                    CogEllipse Ellipse = (CogEllipse)pt.GetTool().Pattern.TrainRegion;
                    OriginX = Ellipse.CenterX;
                    OriginY = Ellipse.CenterY;
                    break;
                case VisionToolRegionShape.Rectangle:
                    CogRectangle Rectangle = (CogRectangle)pt.GetTool().Pattern.TrainRegion;
                    OriginX = Rectangle.CenterX;
                    OriginY = Rectangle.CenterY;
                    break;
                case VisionToolRegionShape.RectangleAffine:
                    CogRectangleAffine RectangleAffine = (CogRectangleAffine)pt.GetTool().Pattern.TrainRegion;
                    OriginX = RectangleAffine.CenterX;
                    OriginY = RectangleAffine.CenterY;
                    break;
                case VisionToolRegionShape.CircularAnnulusSection:
                    CogCircularAnnulusSection CircularAnnulusSection = (CogCircularAnnulusSection)pt.GetTool().Pattern.TrainRegion;
                    OriginX = CircularAnnulusSection.CenterX;
                    OriginY = CircularAnnulusSection.CenterY;
                    break;
                case VisionToolRegionShape.EllipticalAnnulusSection:
                    CogEllipticalAnnulusSection EllipticalAnnulusSection = (CogEllipticalAnnulusSection)pt.GetTool().Pattern.TrainRegion;
                    OriginX = EllipticalAnnulusSection.CenterX;
                    OriginY = EllipticalAnnulusSection.CenterY;
                    break;
            }
            pt.SetOrigin(dipyMod, OriginX, OriginY);
            nud_Origin_X.Value = (decimal)pt.GetTool().Pattern.Origin.TranslationX;
            nud_Origin_Y.Value = (decimal)pt.GetTool().Pattern.Origin.TranslationY;
        }

        private void nud_Origin_ValueChanged(object sender, EventArgs e)
        {
           // pt.SetOrigin(dipyMod, (double)nud_Origin_X.Value,(double)nud_Origin_Y.Value);
        }

        private void Btn_FitToImage_Click(object sender, EventArgs e)
        {
            dipyMod.AutoFit = true;
        }

        private void btn_run_Click(object sender, EventArgs e)
        {
            try
            {
                if (dipyMod.Image != null && pt.GetTool().Pattern.Trained)
                {
                    pt.Run(dipyMod);
                    pt.ShowResult(dipyMod, true);
                    EditEnable(false);
                }
                else
                {
                    if(dipyMod.Image == null) MessageBox.Show(VAR.IsChinese?"没有找到输入图片": "No input picture found\r\n没有找到输入图片");
                    else MessageBox.Show(VAR.IsChinese ? "当前PMAlign图像未训练!": "The current PMAlign image is not trained!\r\n当前PMAlign图像未训练!");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
          
        }

        private void btn_SavePartten_Click(object sender, EventArgs e)
        {
            try
            {
                btn_SavePartten.Enabled = false;
                bool ret = false;
                SaveFileDlg.Filter = "vpp文件(*.vpp)|*.vpp";
                if (DialogResult.OK != SaveFileDlg.ShowDialog()) return;
                curImgFile = SaveFileDlg.FileName;
                ret = pt.Save(curImgFile);
                if (!ret) MessageBox.Show(VAR.IsChinese?"模板保存失败!": "Template save failed!\r\n模板保存失败");
            }
            finally 
            {
                btn_SavePartten.Enabled = true;
            }
           
        }

        private void btn_LoadPartten_Click(object sender, EventArgs e)
        {
            try
            {
                btn_LoadPartten.Enabled = false;
                bool ret = false;
                openFileDlg.Filter = "vpp文件|*.vpp|所有文件|*.*";
                if (DialogResult.OK != openFileDlg.ShowDialog()) return;
                curImgFile = openFileDlg.FileName;
                ret = pt.LoadPattern(curImgFile);
                if (ret) btn_save_Click(null, null);
                else MessageBox.Show(VAR.IsChinese?"模板加载失败!": "Template loading failed!\r\n模板加载失败!");
            }
            finally 
            {
                btn_LoadPartten.Enabled = true;
            }
          

        }
    }
}
