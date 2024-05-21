using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using MotionCtrl;
using System.IO;
using Cognex.VisionPro;

namespace UI
{
    public partial class CogBlockEidt : UserControl
    {
        Cam mCam;
        public Cam Camera
        {
            get
            {
                return mCam;
            }
            set
            {
                mCam = value;
                if (mCam.curTask != null) VsTask = mCam.curTask;
            }
        }
        Cam.VisionTask mVsTask;
        public Cam.VisionTask VsTask
        {
            get
            {
                return mVsTask;
            }
            set
            {
                mVsTask =value;
                if(mVsTask!=null && mVsTask.Block !=null )Editor.Subject = mVsTask.Block;
            }
        }
        public CogBlockEidt()
        {
            InitializeComponent();
        }

        private void btn_triger_Click(object sender, EventArgs e)
        {
            if (mCam != null)
                mCam.Triger();
        }

        private void btn_save_Click(object sender, EventArgs e)
        {            
            if (mVsTask != null)
            {
                if (DialogResult.Yes != MessageBox.Show(VAR.IsChinese?"确定要保存?": "Are you sure you want to save?\r\n确定要保存?", VAR.IsChinese?"提示": "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;
                mVsTask.Save();
                Thread.Sleep(500);
                mVsTask.Load();
            }
        }

        private void btn_load_Click(object sender, EventArgs e)
        {            
            if (mVsTask != null)
            {
                if (DialogResult.Yes != MessageBox.Show(VAR.IsChinese ? "确定要重新加载?": "Are you sure you want to reload?\r\n确定要重新加载?", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) return;
                mVsTask.Load();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (mVsTask == null) return;

            if (DialogResult.OK != openFileDlg.ShowDialog())
                return;
            string ImgFileName = openFileDlg.FileName;

            if (ImgFileName.Length < 3 || !File.Exists(ImgFileName))
                return;


            Bitmap bmp = new Bitmap(ImgFileName);
            CogImage8Grey Image = new CogImage8Grey(bmp);
            ICogImage Image2 = Image;


            mVsTask.RunImage(Image2);
            var img = mVsTask.ResData.OutputImg;

            mVsTask.Block.Inputs["InputImage"].Value = img;
        }
    }
}
