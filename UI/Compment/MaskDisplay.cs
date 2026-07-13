using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;

namespace UI.Compment
{
    public partial class MaskDisplay : UserControl
    {
        private CogImage8Grey m_MaskedImage = null;
        private CogMaskGraphic m_MaskedGraphic = null;
        public bool m_IsApplied = false;
        public CogRecordDisplay disy;
        public PMAlignTool maskpt;
        public MaskDisplay()
        {
            InitializeComponent();
            this.SendToBack();
            //tableLayoutPanel1.Location = new Point(this.Width- tableLayoutPanel1.Width-10, 2);
        }

        public void MaskImages(ICogImage image, CogImage8Grey mask, CogRecordDisplay disy, PMAlignTool pt)
        {
            this.disy = disy;
            this.maskpt = pt;
            CogImageMask.Image = image;
            CogDisplay displayInMaskEditor = FindControl(CogImageMask.Controls, typeof(CogDisplay)) as CogDisplay; //Find the CogDisplay, check for null 

            displayInMaskEditor.Fit();  //Fit the image in Display
            if (mask != null)
            {

                CogImageMask.MaskImage = mask;
                m_MaskedImage = CogImageMask.MaskImage;
                m_MaskedGraphic = new CogMaskGraphic();
                m_MaskedGraphic.Image = m_MaskedImage;

            }
        }

        private Control FindControl(System.Windows.Forms.Control.ControlCollection controlCollection, Type controlType)
        {
            foreach (Control c in controlCollection)
            {
                if (controlType.Equals(c.GetType()))
                    return c;
                else if (c.Controls != null && c.Controls.Count > 0)
                    return FindControl(c.Controls, controlType);
            }
            return null;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (m_MaskedGraphic == null)
                m_MaskedGraphic = new CogMaskGraphic();

            m_MaskedImage = CogImageMask.MaskImage;
            m_MaskedGraphic.Image = m_MaskedImage;
            m_IsApplied = true;
          //  maskpt.GetTool().Pattern.TrainImageMask = m_MaskedGraphic.Image;
           // maskpt.Untrain(disy);

           // VisionRun.VisionPMAlignTool.ShowTrainImage(VisionRun.Display, true, false);
            disy.StaticGraphics.Clear();
            disy.StaticGraphics.Add(m_MaskedGraphic as ICogGraphic, "");
            //VisionRun.VisionPMAlignTool.GrabTrainRegion(VisionRun.Display, VisionToolRegionShape.Circle, false);
            //VisionRun.VisionPMAlignTool.GrabSearchRegion(VisionRun.Display, VisionToolRegionShape.RectangleAffine, false);
            this.SendToBack();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            m_IsApplied = false;
            this.SendToBack();
        }

        public CogMaskGraphic MaskedGraphic
        {
            get { return m_MaskedGraphic; }
        }

        public CogImage8Grey MaskedImage
        {
            get { return m_MaskedImage; }
        }

        public bool MaskApplied
        {
            get { return m_IsApplied; }
        }
    }
}
