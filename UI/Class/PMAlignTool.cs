using Cognex.VisionPro;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro.Display;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cognex.VisionPro.DSCameraSetup.Implementation.Internal;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using MotionCtrl;


namespace UI
{
    public enum VisionToolRegionShape
    {
        [Description("全图像")]
        EntireImage,
        [Description("圆")]
        Circle,
        [Description("椭圆")]
        Ellipse,
        [Description("矩形")]
        Rectangle,
        [Description("仿射矩形")]
        RectangleAffine,
        [Description("圆环")]
        CircularAnnulusSection,
        [Description("椭圆环")]
        EllipticalAnnulusSection
    }
    public class PMAlignTool
    {
        public struct param
        {
            public double  High;
            public double  Low;
            public double Nominal;
            public CogPMAlignZoneConstants Cfg;
        }
        private CogPMAlignTool m_CogPMAlignTool = null;
        private VisionToolRegionShape M_Search;
        private VisionToolRegionShape M_Train;
        private ICogRegion m_ICogRegionTrain = null;
        private ICogRegion m_ICogRegionSearch = null;
        private CogImage8Grey m_ICogImgMask = null;
        public CogCoordinateAxes m_CogCoordinateAxes = new CogCoordinateAxes();
        public ICogGraphicInteractive m_ICogGraphicInteractive;
        private double m_dAcceptThreshold = 0;
        private double m_dContrastThreshold = 0;
        public  param m_Angle=new param();
        public  param m_Scale=new param();
        private int m_nNumToFind = 0;
        private bool m_IsUsingClutter = true;
        private bool m_IsIgnorePolarity = true;
        private bool m_bFound = false;
        private int m_nFoundCount = 0;
        private double m_dScore = 0;
        private double m_dTranslationX = 0;
        private double m_dTranslationY = 0;
        private double m_dAngle = 0;
        private double m_dScale = 0;
        private bool m_bResult = false;
        private bool m_bTrained = false;
        public double m_dMoveSearchRegionX = 0;
        public double m_dMoveSearchRegionY = 0;
        public double m_dInitSearchRegionX = 0;
        public double m_dInitSearchRegionY = 0;

        public PMAlignTool()
        {
        }
        public void InitPMAlignTool(CogPMAlignTool m_CogPMAlignTool)
        {
           this.m_CogPMAlignTool = m_CogPMAlignTool;
            GetPMAlignParam();
        }

        public ICogGraphicInteractive GetInteractiveGraphics(CogRecordDisplay display, string strItemName)
        {
            for (int i = 0; i < display.InteractiveGraphics.Count; ++i)
            {
                if (display.InteractiveGraphics[i].TipText == strItemName)
                {
                    return display.InteractiveGraphics[i];
                }
            }

            return null;
        }

        public CogImage8Grey GetImage8Grey(CogRecordDisplay display)
        {
            if (display.Image == null)
                return null;

            return CogImageConvert.GetIntensityImage(display.Image, 0, 0, display.Image.Width, display.Image.Height);
        }

        public void ClearAll(CogRecordDisplay display)
        {
            display.Image = null;
            display.InteractiveGraphics.Clear();
            display.StaticGraphics.Clear();
        }

        public  void GetPMAlignParam()
        {
            m_ICogRegionTrain = m_CogPMAlignTool.Pattern.TrainRegion;
            m_ICogRegionSearch = m_CogPMAlignTool.SearchRegion;
            m_ICogImgMask = m_CogPMAlignTool.RunParams.SearchImageMask;
            m_dAcceptThreshold = m_CogPMAlignTool.RunParams.AcceptThreshold;
            m_nNumToFind = m_CogPMAlignTool.RunParams.ApproximateNumberToFind;
            m_dContrastThreshold = m_CogPMAlignTool.RunParams.ContrastThreshold;
            m_IsUsingClutter = m_CogPMAlignTool.RunParams.ScoreUsingClutter;
            m_IsIgnorePolarity = m_CogPMAlignTool.Pattern.IgnorePolarity;
            m_Angle.High = CogMisc.RadToDeg(m_CogPMAlignTool.RunParams.ZoneAngle.High);
            m_Angle.Low = CogMisc.RadToDeg(m_CogPMAlignTool.RunParams.ZoneAngle.Low);
            m_Angle.Nominal= CogMisc.RadToDeg(m_CogPMAlignTool.RunParams.ZoneAngle.Nominal);
            m_Angle.Cfg = m_CogPMAlignTool.RunParams.ZoneAngle.Configuration;
            m_Scale.High = m_CogPMAlignTool.RunParams.ZoneScale.High;
            m_Scale.Low = m_CogPMAlignTool.RunParams.ZoneScale.Low;
            m_Scale.Nominal = m_CogPMAlignTool.RunParams.ZoneScale.Nominal;
            m_Scale.Cfg = m_CogPMAlignTool.RunParams.ZoneScale.Configuration;
            m_bTrained = m_CogPMAlignTool.Pattern.Trained;
            //SetInitSearchRegion();
        }

        public void SetPMAlignParam()
        {
            m_nFoundCount = 0;

            if (m_CogPMAlignTool != null)
            {
                m_CogPMAlignTool.RunParams.SaveMatchInfo = true;
                m_CogPMAlignTool.RunParams.AcceptThreshold = m_dAcceptThreshold;
                m_CogPMAlignTool.RunParams.ApproximateNumberToFind = m_nNumToFind;
                m_CogPMAlignTool.RunParams.ContrastThreshold = m_dContrastThreshold;
                m_CogPMAlignTool.RunParams.ScoreUsingClutter = m_IsUsingClutter;
                m_CogPMAlignTool.RunParams.ZoneAngle.Configuration = m_Angle.Cfg;
                m_CogPMAlignTool.RunParams.ZoneAngle.High = CogMisc.DegToRad(m_Angle.High);
                m_CogPMAlignTool.RunParams.ZoneAngle.Low = CogMisc.DegToRad(m_Angle.Low);
                m_CogPMAlignTool.RunParams.ZoneAngle.Nominal = CogMisc.DegToRad(m_Angle.Nominal);
                m_CogPMAlignTool.RunParams.ZoneScale.Configuration = m_Scale.Cfg;
                m_CogPMAlignTool.RunParams.ZoneScale.High = m_Scale.High;
                m_CogPMAlignTool.RunParams.ZoneScale.Low = m_Scale.Low;
                m_CogPMAlignTool.RunParams.ZoneScale.Nominal = m_Scale.Nominal;
                m_CogPMAlignTool.Pattern.IgnorePolarity = m_IsIgnorePolarity;

                //if (m_ICogRegionTrain != null)
                //{
                //    m_CogPMAlignTool.Pattern.TrainRegion = m_ICogRegionTrain;
                //}
                //if (m_ICogRegionSearch != null)
                //{
                //    m_CogPMAlignTool.SearchRegion = m_ICogRegionSearch;
                //}
            }
        }

        public void SetOriginXY(double dX, double dY)
        {
            m_CogPMAlignTool.Pattern.Origin.TranslationX = dX;
            m_CogPMAlignTool.Pattern.Origin.TranslationY = dY;
        }

        public bool GrabTrainRegion(CogRecordDisplay display, VisionToolRegionShape EnumRegionShape,bool Clear=true)
        {
            double dScaling, dAspect, dRotation, dSkew, dTranslationX, dTranslationY;
            try
            {
                if (Clear) display.InteractiveGraphics.Clear();
                m_CogPMAlignTool.Pattern.Origin.GetScalingAspectRotationSkewTranslation(out dScaling, out dAspect, out dRotation, out dSkew, out dTranslationX, out dTranslationY);
                m_CogCoordinateAxes.SetOriginLengthAspectRotationSkew(dTranslationX, dTranslationY, dScaling, dAspect, dRotation, dSkew);
                m_CogCoordinateAxes.Interactive = true;
                m_CogCoordinateAxes.GraphicDOFEnable = CogCoordinateAxesDOFConstants.Position;
                m_CogCoordinateAxes.TipText = "Pattern Origin";
                display.InteractiveGraphics.Add(m_CogCoordinateAxes as ICogGraphicInteractive, "", false);
                ICogRegion region = m_CogPMAlignTool.Pattern.TrainRegion;
                CogImage8Grey img = GetImage8Grey(display);
                switch (EnumRegionShape)
                {
                      
                    case VisionToolRegionShape.Circle:
                        {
                            CogCircle circle = region as CogCircle;
                            if (circle == null)
                            {
                                circle = new CogCircle();                             
                                circle.FitToImage(img, 0.3, 0.3);
                            }
                            m_ICogGraphicInteractive = circle as ICogGraphicInteractive;
                        }
                        break;
                    case VisionToolRegionShape.Ellipse:
                        {
                            CogEllipse ellipse = region as CogEllipse;
                            if (ellipse == null)
                            {
                                ellipse = new CogEllipse();                     
                                ellipse.FitToImage(img, 0.3, 0.3);
                            }
                            m_ICogGraphicInteractive = ellipse as ICogGraphicInteractive;
                        }
                        break;
                    case VisionToolRegionShape.Rectangle:
                        {
                            CogRectangle rectangle = region as CogRectangle;
                            if (rectangle == null)
                            {
                                rectangle = new CogRectangle();
                                rectangle.FitToImage(img, 0.3, 0.3);
                            }
                            m_ICogGraphicInteractive = rectangle as ICogGraphicInteractive;
                        }
                        break;
                    case VisionToolRegionShape.RectangleAffine:
                        {
                            CogRectangleAffine rectangleAffine = region as CogRectangleAffine;
                            if (rectangleAffine == null)
                            {
                                rectangleAffine = new CogRectangleAffine();
                                rectangleAffine.FitToImage(img, 0.3, 0.3);
                            }
                            m_ICogGraphicInteractive = rectangleAffine as ICogGraphicInteractive;
                        }
                        break;
                    case VisionToolRegionShape.CircularAnnulusSection:
                        {
                            CogCircularAnnulusSection circularAnnuluns = region as CogCircularAnnulusSection;
                            if (circularAnnuluns == null)
                            {
                                circularAnnuluns = new CogCircularAnnulusSection();
                                circularAnnuluns.FitToImage(img, 0.3, 0.3);
                            }
                            m_ICogGraphicInteractive = circularAnnuluns as ICogGraphicInteractive;
                        }
                        break;
                    case VisionToolRegionShape.EllipticalAnnulusSection:
                        {
                            CogEllipticalAnnulusSection ellipticalAnnuluns = region as CogEllipticalAnnulusSection;
                            if (ellipticalAnnuluns == null)
                            {
                                ellipticalAnnuluns = new CogEllipticalAnnulusSection();
                                ellipticalAnnuluns.FitToImage(img, 0.3, 0.3);
                            }
                            m_ICogGraphicInteractive = ellipticalAnnuluns as ICogGraphicInteractive;
                        }
                        break;
                    case VisionToolRegionShape.EntireImage:
                        {
                            m_ICogGraphicInteractive = GetInteractiveGraphics(display, "Region of Interest");
                            if (m_ICogGraphicInteractive != null)
                            {
                                display.InteractiveGraphics.RemoveItem(m_ICogGraphicInteractive);
                            }
                            m_ICogRegionTrain = null;
                            m_ICogGraphicInteractive = null;
                        }
                        break;

                    default:
                        m_ICogRegionTrain = null;

                        return false;
                }
                if (m_ICogGraphicInteractive != null)
                {
                    m_ICogGraphicInteractive.GraphicDOFEnableBase = CogGraphicDOFConstants.All;
                    m_ICogGraphicInteractive.Interactive = true;
                    m_ICogGraphicInteractive.Color = CogColorConstants.Blue;
                    m_ICogGraphicInteractive.LineWidthInScreenPixels = 2;
                    m_ICogGraphicInteractive.SelectedColor = CogColorConstants.Green;
                    m_ICogGraphicInteractive.SelectedLineWidthInScreenPixels = 2;
                    m_ICogGraphicInteractive.TipText = "Region of Interest";
                    display.InteractiveGraphics.Add(m_ICogGraphicInteractive as ICogGraphicInteractive, "", false);
                }
                return true;
            }
            catch (Exception e)
            {

                return false;
            }
            
        }

        public bool GrabSearchRegion(CogRecordDisplay display, VisionToolRegionShape EnumRegionShape,bool Clear= true)
        {
            if(Clear)display.InteractiveGraphics.Clear();
            ICogGraphicInteractive icogGraphInteractive = null;
            ICogRegion region = m_CogPMAlignTool.SearchRegion;
            //region.FitToImage(display.GetImage8Grey(), 0.2, 0.2);           
            CogImage8Grey img = GetImage8Grey(display);
            switch (EnumRegionShape)
            {
                case VisionToolRegionShape.Circle:
                    {
                        CogCircle circle = region as CogCircle;
                        if (circle == null)
                        {
                            circle = new CogCircle();                          
                            circle.FitToImage(img, 0.5, 0.5);
                        }
                        icogGraphInteractive = circle as ICogGraphicInteractive;
                    }
                    break;
                case VisionToolRegionShape.Ellipse:
                    {
                        CogEllipse ellipse = region as CogEllipse;
                        if (ellipse == null)
                        {
                            ellipse = new CogEllipse();
                            ellipse.FitToImage(img, 0.5, 0.5);
                        }
                        icogGraphInteractive = ellipse as ICogGraphicInteractive;
                    }
                    break;
                case VisionToolRegionShape.Rectangle:
                    {
                        CogRectangle rectangle = region as CogRectangle;
                        if (rectangle == null)
                        {
                            rectangle = new CogRectangle();                          
                            rectangle.FitToImage(img, 0.5, 0.5);
                        }
                        icogGraphInteractive = rectangle as ICogGraphicInteractive;
                    }
                    break;
                case VisionToolRegionShape.RectangleAffine:
                    {
                        CogRectangleAffine rectangleAffine = region as CogRectangleAffine;
                        if (rectangleAffine == null)
                        {
                            rectangleAffine = new CogRectangleAffine();
                            rectangleAffine.FitToImage(img, 0.5, 0.5);
                        }
                        icogGraphInteractive = rectangleAffine as ICogGraphicInteractive;
                    }
                    break;
                case VisionToolRegionShape.CircularAnnulusSection:
                    {
                        CogCircularAnnulusSection circularAnnuluns = region as CogCircularAnnulusSection;
                        if (circularAnnuluns == null)
                        {
                            circularAnnuluns = new CogCircularAnnulusSection();
                            circularAnnuluns.FitToImage(img, 0.5, 0.5);
                        }
                        icogGraphInteractive = circularAnnuluns as ICogGraphicInteractive;
                    }
                    break;
                case VisionToolRegionShape.EllipticalAnnulusSection:
                    {
                        CogEllipticalAnnulusSection ellipticalAnnuluns = region as CogEllipticalAnnulusSection;
                        if (ellipticalAnnuluns == null)
                        {
                            ellipticalAnnuluns = new CogEllipticalAnnulusSection();
                            ellipticalAnnuluns.FitToImage(img, 0.5, 0.5);
                        }
                        icogGraphInteractive = ellipticalAnnuluns as ICogGraphicInteractive;
                    }
                    break;
                case VisionToolRegionShape.EntireImage:
                    {
                        m_ICogGraphicInteractive = GetInteractiveGraphics(display, "Region To Search");
                        if (m_ICogGraphicInteractive != null)
                        {
                           display.InteractiveGraphics.RemoveItem(m_ICogGraphicInteractive);
                        }
                        m_ICogRegionSearch = null;
                    }
                    break;

                default:
                    m_ICogRegionSearch = null;
                    return false;
            }
            if (icogGraphInteractive != null)
            {
                icogGraphInteractive.GraphicDOFEnableBase = CogGraphicDOFConstants.All;
                icogGraphInteractive.Interactive = true;
                icogGraphInteractive.TipText = "Region To Search";
                icogGraphInteractive.Color = CogColorConstants.Magenta;
                icogGraphInteractive.LineWidthInScreenPixels = 2;
                icogGraphInteractive.SelectedColor = CogColorConstants.Cyan;
                icogGraphInteractive.SelectedLineWidthInScreenPixels = 2;
                display.InteractiveGraphics.Add(icogGraphInteractive as ICogGraphicInteractive, "", false);
            }
            return true;
        }

        public void SetSearchRegion(CogRecordDisplay display)
        {

            ICogGraphicInteractive icogGraphInteractive = GetInteractiveGraphics(display, "Region To Search");
            if (icogGraphInteractive != null)
            {
                icogGraphInteractive.GraphicDOFEnableBase = CogGraphicDOFConstants.None;
                icogGraphInteractive.Interactive = false;
            }
            m_CogPMAlignTool.SearchRegion = icogGraphInteractive as ICogRegion;
            m_ICogRegionSearch = m_CogPMAlignTool.SearchRegion;

        }

        public void MoveSearchRegion(double x, double y)
        {
            m_dMoveSearchRegionX = x;
            m_dMoveSearchRegionY = y;
            ICogRegion region = m_CogPMAlignTool.SearchRegion;
            if (region is CogCircle)
            {
                CogCircle circle = region as CogCircle;

                circle.CenterX = m_dInitSearchRegionX + x;
                circle.CenterY = m_dInitSearchRegionY + y;
            }
            else if (region is CogEllipse)
            {
                CogEllipse ellipse = region as CogEllipse;
                ellipse.CenterX = m_dInitSearchRegionX + x;
                ellipse.CenterY = m_dInitSearchRegionY + y;
            }
            else if (region is CogRectangle)
            {
                CogRectangle rectangle = region as CogRectangle;
                rectangle.X = m_dInitSearchRegionX + x;
                rectangle.Y = m_dInitSearchRegionY + y;
            }
            else if (region is CogRectangleAffine)
            {
                CogRectangleAffine rectangleAffine = region as CogRectangleAffine;
                rectangleAffine.CenterX = m_dInitSearchRegionX + x;
                rectangleAffine.CenterY = m_dInitSearchRegionY + y;
            }
            else if (region is CogCircularAnnulusSection)
            {
                CogCircularAnnulusSection circularAnnuluns = region as CogCircularAnnulusSection;
                circularAnnuluns.CenterX = m_dInitSearchRegionX + x;
                circularAnnuluns.CenterY = m_dInitSearchRegionY + y;
            }
            else if (region is CogEllipticalAnnulusSection)
            {
                CogEllipticalAnnulusSection ellipticalAnnuluns = region as CogEllipticalAnnulusSection;
                ellipticalAnnuluns.CenterX = m_dInitSearchRegionX + x;
                ellipticalAnnuluns.CenterY = m_dInitSearchRegionY + y;
            }
        }

        public void SetInitSearchRegion(double dX, double dY)
        {
            m_dInitSearchRegionX = dX;
            m_dInitSearchRegionY = dY;

            ICogRegion region = m_CogPMAlignTool.SearchRegion;

            if (region is CogCircle)
            {
                CogCircle circle = region as CogCircle;

                circle.CenterX = m_dInitSearchRegionX;
                circle.CenterY = m_dInitSearchRegionY;
            }
            else if (region is CogEllipse)
            {
                CogEllipse ellipse = region as CogEllipse;
                ellipse.CenterX = m_dInitSearchRegionX;
                ellipse.CenterY = m_dInitSearchRegionY;
            }
            else if (region is CogRectangle)
            {
                CogRectangle rectangle = region as CogRectangle;
                rectangle.X = m_dInitSearchRegionX;
                rectangle.Y = m_dInitSearchRegionY;
            }
            else if (region is CogRectangleAffine)
            {
                CogRectangleAffine rectangleAffine = region as CogRectangleAffine;
                rectangleAffine.CenterX = m_dInitSearchRegionX;
                rectangleAffine.CenterY = m_dInitSearchRegionY;
            }
            else if (region is CogCircularAnnulusSection)
            {
                CogCircularAnnulusSection circularAnnuluns = region as CogCircularAnnulusSection;
                circularAnnuluns.CenterX = m_dInitSearchRegionX;
                circularAnnuluns.CenterY = m_dInitSearchRegionY;
            }
            else if (region is CogEllipticalAnnulusSection)
            {
                CogEllipticalAnnulusSection ellipticalAnnuluns = region as CogEllipticalAnnulusSection;
                ellipticalAnnuluns.CenterX = m_dInitSearchRegionX;
                ellipticalAnnuluns.CenterY = m_dInitSearchRegionY;
            }
        }
        public void SetInitSearchRegion()
        {
            ICogRegion region = m_CogPMAlignTool.SearchRegion;

            if (region is CogCircle)
            {
                CogCircle circle = region as CogCircle;

                m_dInitSearchRegionX = circle.CenterX - m_dMoveSearchRegionX;
                m_dInitSearchRegionY = circle.CenterY - m_dMoveSearchRegionY;
            }
            else if (region is CogEllipse)
            {
                CogEllipse ellipse = region as CogEllipse;
                m_dInitSearchRegionX = ellipse.CenterX - m_dMoveSearchRegionX;
                m_dInitSearchRegionY = ellipse.CenterY - m_dMoveSearchRegionY;
            }
            else if (region is CogRectangle)
            {
                CogRectangle rectangle = region as CogRectangle;
                m_dInitSearchRegionX = rectangle.X - m_dMoveSearchRegionX;
                m_dInitSearchRegionY = rectangle.Y - m_dMoveSearchRegionY;
            }
            else if (region is CogRectangleAffine)
            {
                CogRectangleAffine rectangleAffine = region as CogRectangleAffine;
                m_dInitSearchRegionX = rectangleAffine.CenterX - m_dMoveSearchRegionX;
                m_dInitSearchRegionY = rectangleAffine.CenterY - m_dMoveSearchRegionY;
            }
            else if (region is CogCircularAnnulusSection)
            {
                CogCircularAnnulusSection circularAnnuluns = region as CogCircularAnnulusSection;
                m_dInitSearchRegionX = circularAnnuluns.CenterX - m_dMoveSearchRegionX;
                m_dInitSearchRegionY = circularAnnuluns.CenterY - m_dMoveSearchRegionY;
            }
            else if (region is CogEllipticalAnnulusSection)
            {
                CogEllipticalAnnulusSection ellipticalAnnuluns = region as CogEllipticalAnnulusSection;
                m_dInitSearchRegionX = ellipticalAnnuluns.CenterX - m_dMoveSearchRegionX;
                m_dInitSearchRegionY = ellipticalAnnuluns.CenterY - m_dMoveSearchRegionY;
            }

        }

        public void SetTrainRegion(CogRecordDisplay display)
        {
            m_ICogGraphicInteractive = GetInteractiveGraphics(display,"Region of Interest");
            if (m_ICogGraphicInteractive != null)
            {
                m_ICogGraphicInteractive.GraphicDOFEnableBase = CogGraphicDOFConstants.None;
                m_ICogGraphicInteractive.Interactive = false;
                ICogGraphicInteractive axes = GetInteractiveGraphics(display,"Pattern Origin");
                if (axes != null)
                {
                    axes.GraphicDOFEnableBase = CogGraphicDOFConstants.None;
                    axes.Interactive = false;
                    double dScaling, dAspect, dRotation, dSkew, dTranslationX, dTranslationY;

                    m_CogCoordinateAxes.GetOriginLengthAspectRotationSkew(out dTranslationX, out dTranslationY, out dScaling, out dAspect, out dRotation, out dSkew);
                    m_CogPMAlignTool.Pattern.Origin.SetScalingAspectRotationSkewTranslation(dScaling, dAspect, dRotation, dSkew, dTranslationX, dTranslationY);
                }
            }
            m_CogPMAlignTool.Pattern.TrainRegion = m_ICogGraphicInteractive as ICogRegion;
            m_ICogRegionTrain = m_CogPMAlignTool.Pattern.TrainRegion;
        }

        public void Untrain(CogRecordDisplay displayPattern)
        {
            if (m_bTrained == true)
            {
              m_CogPMAlignTool.Pattern.Untrain();
               // displayPattern.ClearAll();

                m_bTrained = false;
            }
        }

        public bool Train(CogRecordDisplay display, CogImage8Grey imageMask)
        {
            CogImage8Grey inputImage = GetImage8Grey(display);

            if (inputImage == null)
            {
                m_bTrained = false;
            }
            else
            {
                if (imageMask != null)
                {
                    m_CogPMAlignTool.Pattern.TrainImageMask = imageMask;
                }
                try
                {
                    SetSearchRegion(display);
                    SetTrainRegion(display);
                    m_CogPMAlignTool.Pattern.TrainImage = inputImage;                 
                    m_CogPMAlignTool.Pattern.Train();
                   
                    //m_ICogRegionTrain= m_CogPMAlignTool.Pattern.TrainRegion;

                    ICogGraphicInteractive axes = GetInteractiveGraphics(display, "Pattern Origin");
                    if (axes != null)
                    {
                        //axes.GraphicDOFEnableBase = CogGraphicDOFConstants.None;
                        //axes.Interactive = false;
                        double dScaling, dAspect, dRotation, dSkew, dTranslationX, dTranslationY;

                        m_CogCoordinateAxes.GetOriginLengthAspectRotationSkew(out dTranslationX, out dTranslationY, out dScaling, out dAspect, out dRotation, out dSkew);
                        m_CogPMAlignTool.Pattern.Origin.SetScalingAspectRotationSkewTranslation(dScaling, dAspect, dRotation, dSkew, dTranslationX, dTranslationY);
                    }
                    display.InteractiveGraphics.Clear();
                    m_bTrained = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                };

            }
            return m_bTrained;
        }

        /// <summary>
        /// 原点操作
        /// </summary>
        /// <param name="display">
        public void SetOrigin(CogRecordDisplay display,double x,double y)
        {
            ICogGraphicInteractive axes = GetInteractiveGraphics(display, "Pattern Origin");
            if (axes != null)
            {
                double dScaling, dAspect, dRotation, dSkew, dTranslationX, dTranslationY;
                m_CogCoordinateAxes.GetOriginLengthAspectRotationSkew(out dTranslationX, out dTranslationY, out dScaling, out dAspect, out dRotation, out dSkew);
                display.InteractiveGraphics.RemoveItem(m_CogCoordinateAxes);
                m_CogCoordinateAxes.SetOriginLengthAspectRotationSkew(x, y, dScaling, dAspect, dRotation, dSkew);
                m_CogPMAlignTool.Pattern.Origin.SetScalingAspectRotationSkewTranslation(dScaling, dAspect, dRotation, dSkew, x, y);
                m_CogCoordinateAxes.Interactive = true;
                m_CogCoordinateAxes.GraphicDOFEnable = CogCoordinateAxesDOFConstants.Position;
                m_CogCoordinateAxes.TipText = "Pattern Origin";                
                display.InteractiveGraphics.Add(m_CogCoordinateAxes as ICogGraphicInteractive, "", false);
            }
        }

        public void GetOrigin(CogRecordDisplay display,ref double x,ref double y,ref double r)
        {
            ICogGraphicInteractive axes = GetInteractiveGraphics(display, "Pattern Origin");
            if (axes != null)
            {
                double dScaling, dAspect, dRotation, dSkew, dTranslationX, dTranslationY;
                m_CogCoordinateAxes.GetOriginLengthAspectRotationSkew(out dTranslationX, out dTranslationY, out dScaling, out dAspect, out dRotation, out dSkew);
                x = dTranslationX;
                y = dTranslationY;
                r = dRotation;
            }
            else
            {
                MessageBox.Show("没有找到原点坐标！");
            }
        }

        public void ViewLastTrainRegion(CogRecordDisplay display)
        {
            try
            {
                if (m_bTrained == true)
                {
                    ICogRegion region = m_ICogRegionTrain;
                    m_ICogGraphicInteractive = region as ICogGraphicInteractive;

                    if (m_ICogGraphicInteractive != null)
                    {
                        m_ICogGraphicInteractive.GraphicDOFEnableBase = CogGraphicDOFConstants.All;
                        m_ICogGraphicInteractive.Interactive = true;
                        display.InteractiveGraphics.Add(m_ICogGraphicInteractive, "", false);
                        double dScaling, dAspect, dRotation, dSkew, dTranslationX, dTranslationY;

                        CogTransform2DLinear Transform2DLinear = m_CogPMAlignTool.Pattern.Origin;
                        Transform2DLinear.GetScalingAspectRotationSkewTranslation(out dScaling, out dAspect, out dRotation, out dSkew, out dTranslationX, out dTranslationY);

                        if (Transform2DLinear != null)
                        {
                            m_CogCoordinateAxes.SetOriginLengthAspectRotationSkew(dTranslationX, dTranslationY, dScaling, dAspect, dRotation, dSkew);
                            ICogGraphicInteractive axesIntercative = m_CogCoordinateAxes as ICogGraphicInteractive;
                            axesIntercative.GraphicDOFEnableBase = CogGraphicDOFConstants.Position;
                            axesIntercative.Interactive = true;
                            axesIntercative.TipText = "Pattern Origin";
                            display.InteractiveGraphics.Add(axesIntercative, "", false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        public void ViewLastSearchRegion(CogRecordDisplay display)
        {
            ICogRegion region = m_CogPMAlignTool.SearchRegion;
            ICogGraphicInteractive icogGraphInteractive = region as ICogGraphicInteractive;

            try
            {
                if (icogGraphInteractive != null)
                {
                    icogGraphInteractive.GraphicDOFEnableBase = CogGraphicDOFConstants.All;
                    icogGraphInteractive.Interactive = true;
                    display.InteractiveGraphics.Add(icogGraphInteractive, "", false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
        }

        public void ShowPattern(CogRecordDisplay display,bool Clean=true)
        {
            if (m_bTrained == true)
            {
                try
                {
                    if(Clean) ClearAll(display);
                    display.Image = m_CogPMAlignTool.Pattern.GetTrainedPatternImage();                 
                    if (TrainMode == CogPMAlignTrainModeConstants.Image)
                    {
                        CogImage8Grey mask = m_CogPMAlignTool.Pattern.GetTrainedPatternImageMask();
                        if (mask != null)
                        {
                            CogMaskGraphic MaskGraphic = new CogMaskGraphic();
                            MaskGraphic.Image = mask;
                            display.StaticGraphics.Add(MaskGraphic as ICogGraphic, "");
                        }
                    }

                    ShowPatternOrigin(display);
                    ShowPatternRegion(display);
                   // ShowCoarse(display);
                     ShowFine(display);
                     //display.Fit(true);
                    //ShowTrainShapeModel(display);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        public void PatternEdit(CogRecordDisplay display, VisionToolRegionShape EnumSearchShape, VisionToolRegionShape EnumTrainShape,bool trainimag = true)
        {

            if (ShowTrainImage(display, true, false, false, false, trainimag))
            {
                GrabSearchRegion(display, EnumSearchShape, false);
                GrabTrainRegion(display, EnumTrainShape, false);
            }
        
        }


        public bool ShowTrainImage(CogRecordDisplay display, bool Clean = true,bool showpattern=true,bool showregion=true,bool showGrab=false,bool trainimag=true)
        {
            //if (m_bTrained == true)
            {
                try
                {
                    if (Clean && trainimag) ClearAll(display);
                    if (trainimag)
                    {
                        display.Image = m_CogPMAlignTool.Pattern.TrainImage;
                    }
                    else
                    {
                        
                        //if (m_CogPMAlignTool.InputImage != null) display.Image = m_CogPMAlignTool.InputImage;
                        //else
                        //{
                            if (display.Image != null) m_CogPMAlignTool.InputImage = display.Image;
                            else
                            {
                                MessageBox.Show(VAR.IsChinese?"没有找到输入图片": "No input picture found\r\n没有找到输入图片");
                                return false;
                            }
                            
                        //}
                       
                           
                            

                    }
                    if (display.Image != null)
                    {
                        display.StaticGraphics.Clear();
                        display.InteractiveGraphics.Clear();
                    }
                   
                    if (TrainMode == CogPMAlignTrainModeConstants.Image)
                    {
                        CogImage8Grey mask = m_CogPMAlignTool.Pattern.TrainImageMask;
                        if (mask != null)
                        {
                            CogMaskGraphic MaskGraphic = new CogMaskGraphic();
                            MaskGraphic.Image = mask;
                            display.StaticGraphics.Add(MaskGraphic as ICogGraphic, "");
                        }
                    }

                    if (showregion)
                    {
                        ShowSearchRegion(display);
                        ShowTrainRegion(display);
                    }

                    if (showGrab)
                    {

                        GrabTrainRegion(display, PatternShape, false);
                        GrabSearchRegion(display, SearchShape, false);
                    }

                    if (showpattern)
                    {
                        //ShowPatternOrigin(display);
                        //ShowPatternRegion(display);
                        // ShowCoarse(display);
                        ShowFine(display);
                        //ShowTrainShapeModel(display);
                    }
                    display.AutoFit = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
                return true;
            }
        }

        private void ShowPatternRegion(CogRecordDisplay display)
        {
            ICogRegion region = m_ICogRegionTrain;
            m_ICogGraphicInteractive = region as ICogGraphicInteractive;

            if (m_ICogGraphicInteractive != null)
            {
                m_ICogGraphicInteractive.GraphicDOFEnableBase = CogGraphicDOFConstants.All;
                m_ICogGraphicInteractive.Interactive = true;
                m_ICogGraphicInteractive.Color = CogColorConstants.Cyan;
                display.StaticGraphics.Add(m_ICogGraphicInteractive, "");
            }
        }

        private void ShowPatternOrigin(CogRecordDisplay display)
        {
            double dScaling, dAspect, dRotation, dSkew, dOriginX, dOriginY;

            CogTransform2DLinear Transform2DLinear = m_CogPMAlignTool.Pattern.Origin;
            Transform2DLinear.GetScalingAspectRotationSkewTranslation(out dScaling, out dAspect, out dRotation, out dSkew, out dOriginX, out dOriginY);

            CogCoordinateAxes coordinateAxes = new CogCoordinateAxes(m_CogCoordinateAxes);
            coordinateAxes.SetOriginLengthAspectRotationSkew(dOriginX, dOriginY, dScaling, dAspect, dRotation, dSkew);

            ICogGraphicInteractive icogGraphInteractive = coordinateAxes as ICogGraphicInteractive;
            icogGraphInteractive.Interactive = false;
            icogGraphInteractive.Color = CogColorConstants.Cyan;
            display.StaticGraphics.Add(icogGraphInteractive, "");
        }

        private void ShowCoarse(CogRecordDisplay display)
        {
            CogGraphicCollection GraphicCollection = m_CogPMAlignTool.Pattern.CreateGraphicsCoarse(CogColorConstants.Yellow);

            for (int i = 0; i < GraphicCollection.Count; i++)
            {
                display.StaticGraphics.Add(GraphicCollection[i] as ICogGraphic, "");
            }
        }

        private void ShowFine(CogRecordDisplay display)
        {
            CogGraphicCollection GraphicCollection = m_CogPMAlignTool.Pattern.CreateGraphicsFine(CogColorConstants.Green);

            for (int i = 0; i < GraphicCollection.Count; i++)
            {
                display.StaticGraphics.Add(GraphicCollection[i] as ICogGraphic, "");
            }
        }

        private void ShowTrainShapeModel(CogRecordDisplay display)
        {
            for (int i = 0; i < m_CogPMAlignTool.Pattern.GetTrainedPatternShapeModels().Count; i++)
            {

                display.StaticGraphics.Add(m_CogPMAlignTool.Pattern.GetTrainedPatternShapeModels().get_Item(i) as ICogGraphic, "");
            }
        }

        public void ShowTrainRegion(CogRecordDisplay display)
        {
            ShowPatternRegion(display);
        }

        public void ShowTrainOrigin(CogRecordDisplay display)
        {
            ShowPatternOrigin(display);
        }

        public void ShowSearchRegion(CogRecordDisplay display)
        {
            ICogRegion searchRegion = m_CogPMAlignTool.SearchRegion;
            ICogGraphicInteractive searchInteractive = searchRegion as ICogGraphicInteractive;

            if (searchInteractive != null)
            {
                //searchInteractive.GraphicDOFEnableBase = CogGraphicDOFConstants.None;
                //searchInteractive.Interactive = false;
                searchInteractive.GraphicDOFEnableBase = CogGraphicDOFConstants.All;
                searchInteractive.Interactive = true;
                searchInteractive.Color = CogColorConstants.Orange;
                searchInteractive.LineWidthInScreenPixels = 2;
                searchInteractive.SelectedLineWidthInScreenPixels = 2;
               // searchInteractive.Color = CogColorConstants.DarkGreen;
                display.InteractiveGraphics.Add(searchInteractive, "", false);
            }
        }

        private void ShowSearchedResult(CogRecordDisplay display, int nIndex)
        {
            CogPMAlignResult pmAlignResult = new CogPMAlignResult();
            pmAlignResult = m_CogPMAlignTool.Results[nIndex];
            if (pmAlignResult != null)
            {
                if (m_bFound == true)
                {
                    CogCompositeShape compositeShape = new CogCompositeShape();
                    compositeShape = pmAlignResult.CreateResultGraphics((CogPMAlignResultGraphicConstants)(1 + 64 + 32 + 16));
                    display.InteractiveGraphics.Add(compositeShape as ICogGraphicInteractive, "", false);
                }
            }
        }

        public void GetResult(int nIndex)
        {
            if (nIndex >= 0)
            {
                m_dTranslationX = m_CogPMAlignTool.Results[nIndex].GetPose().TranslationX;
                m_dTranslationY = m_CogPMAlignTool.Results[nIndex].GetPose().TranslationY;
                m_dScore = m_CogPMAlignTool.Results[nIndex].Score;
                m_dAngle = CogMisc.RadToDeg(m_CogPMAlignTool.Results[nIndex].GetPose().Rotation);
                m_dScale = m_CogPMAlignTool.Results[nIndex].GetPose().Scaling;

                if (m_dScore >= m_dAcceptThreshold)
                {
                    m_bResult = true;
                }
                else
                {
                    m_bResult = false;
                }
            }
            else
            {
                m_dTranslationX = 0;
                m_dTranslationY = 0;
                m_dScore = 0;
                m_dAngle = 0;
                m_dScale = 0;
                m_bResult = false;
            }
        }

        public void DrawTextToGCOut(CogGraphicCollection gc, int x, int y, CogColorConstants cl, string str)
        {
            CogGraphicLabel lable = new CogGraphicLabel();
            lable.Alignment = CogGraphicLabelAlignmentConstants.BaselineLeft;
            lable.SelectedSpaceName = "*";
            lable.Font = new Font("Arial", 12);
            lable.LineWidthInScreenPixels = 3;
            lable.Color = cl;
            lable.SetXYText(x, y, str);
            gc.Add(lable);
        }

        public void ShowResult(CogRecordDisplay display,int index)
        {
            display.StaticGraphics.Clear();
         
                int lineHeight = 24;
                int n = 0;
                CogColorConstants cl = m_bResult ? CogColorConstants.Green : CogColorConstants.Red;

                CogGraphicCollection gc = new CogGraphicCollection();
                DrawTextToGCOut(gc, 10, 30 + lineHeight * n++, cl, m_bResult ? "OK" : "NG");
                DrawTextToGCOut(gc, 10, 30 + lineHeight * n++, cl, string.Format("X:{0:000.000}", m_dTranslationX));
                DrawTextToGCOut(gc, 10, 30 + lineHeight * n++, cl, string.Format("Y:{0:000.000}", m_dTranslationY));
                DrawTextToGCOut(gc, 10, 30 + lineHeight * n++, cl, string.Format("A:{0:000.000}", m_dAngle));
                DrawTextToGCOut(gc, 10, 30 + lineHeight * n++, cl, string.Format("S: {0:0.000}", m_dScore));

            display.StaticGraphics.AddList(gc,"Result");
    }

        public int GetRun()
        {
            SetPMAlignParam();

            if (m_CogPMAlignTool != null)
            {
                try
                {
                    if (m_bTrained == true)
                    {
                        if (m_CogPMAlignTool.Results != null)
                        {
                            m_nFoundCount = m_CogPMAlignTool.Results.Count;
                            double score = m_dAcceptThreshold;

                            for (int i = 0; i < m_nFoundCount; i++)
                            {
                                if (m_CogPMAlignTool.Results[i].Score > score)
                                {
                                    score = m_CogPMAlignTool.Results[i].Score;
                                }
                            }
                            if (score > m_dAcceptThreshold)
                            {
                                m_dScore = score;
                                m_bFound = true;
                            }
                            else
                            {
                                m_dScore = 0;
                                m_bResult = false;
                                m_bFound = false;
                            }
                        }
                        else
                        {
                            m_bFound = false;
                        }
                    }
                }
                catch
                {
                }
            }

            return m_nFoundCount;
        }

        public int Run(CogRecordDisplay display)
        {
            SetPMAlignParam();

            if (m_CogPMAlignTool != null)
            {
                display.InteractiveGraphics.Clear();
                display.StaticGraphics.Clear();
                m_CogPMAlignTool.InputImage = GetImage8Grey(display);
               // m_CogPMAlignTool.InputImage = display.Image;
                try
                {
                    if (m_bTrained == true)
                    {
                        if (m_dAcceptThreshold > 0.5)
                            m_CogPMAlignTool.RunParams.AcceptThreshold = 0.5;

                        m_CogPMAlignTool.Run();
                        if (m_CogPMAlignTool.Results != null)
                        {
                            m_nFoundCount = m_CogPMAlignTool.Results.Count;
                            if (m_nFoundCount > 0)
                            {
                                double score = m_dAcceptThreshold;

                                for (int i = 0; i < m_nFoundCount; i++)
                                {
                                    if (m_CogPMAlignTool.Results[i].Score > score)
                                    {
                                        score = m_CogPMAlignTool.Results[i].Score;
                                    }
                                }
                                if (score > m_dAcceptThreshold)
                                {
                                    m_dScore = score;
                                    m_bFound = true;
                                }
                                else
                                {
                                    m_dScore = 0;
                                    m_bResult = false;
                                    m_bFound = false;
                                }
                            }
                            else
                            {
                                m_dScore = 0;
                                m_bResult = false;
                                m_bFound = false;
                            }
                        }
                        else
                        {
                            m_dScore = 0;
                            m_bResult = false;
                            m_bFound = false;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Run PMAlign Error");
                }
                finally
                {
                    m_CogPMAlignTool.RunParams.AcceptThreshold = m_dAcceptThreshold;
                }
            }

            return m_nFoundCount;
        }

        public void ShowResult(CogRecordDisplay display, bool bView)
        {
            int count = 0;

            if (m_nFoundCount > 0)
            {
                int index = 0;

                for (int i = 0; i < m_nFoundCount; i++)
                {
                    if (m_CogPMAlignTool.Results[i].Score == m_dScore)
                    {
                        index = i;
                        break;
                    }
                }

                GetResult(index);
              
                // ShowSearchedResult(display, 0);
            }

            if (bView)
            {
                if (m_nNumToFind <= m_nFoundCount)
                {
                    count = m_nNumToFind;
                }
                else
                {
                    count = m_nFoundCount;
                }
                display.InteractiveGraphics.Clear();
                for (int i = 0; i < count; i++)
                {
                    ShowSearchedResult(display, i);
                }
                ShowResult(display, 0);
                ShowSearchRegion(display);
            }
        }

        public void SavePattern(string strPtnName, CogPMAlignPattern pattern)
        {
            CogSerializer.SaveObjectToFile(pattern, strPtnName);
        }

        public bool LoadPattern(string strPtnName, CogRecordDisplay display)
        {
            if (File.Exists(strPtnName))
            {
                try
                {
                    CogPMAlignPattern pattern = CogSerializer.LoadObjectFromFile(strPtnName) as CogPMAlignPattern;

                    ClearAll(display);
                    display.Image = pattern.GetTrainedPatternImage();

                    CogImage8Grey mask = pattern.GetTrainedPatternImageMask();
                    if (mask != null)
                    {
                        CogMaskGraphic MaskGraphic = new CogMaskGraphic();
                        MaskGraphic.Image = mask;
                        display.StaticGraphics.Add(MaskGraphic as ICogGraphic, "");
                    }

                    ShowPatternOrigin(display);
                    ShowCoarse(display);
                    ShowFine(display);

                    return true;
                }
                catch { }
            }

            ClearAll(display);
            return false;
        }

        public CogImage8Grey InputImage
        {
            get {
                if (m_CogPMAlignTool == null) return null;                              
                return (CogImage8Grey)m_CogPMAlignTool.InputImage; }
            set { m_CogPMAlignTool.InputImage = value; }
        }

        public CogPMAlignTool GetTool()
        {
            return m_CogPMAlignTool;
        }

        public bool Trained
        {
            get { return m_bTrained; }
        }

        public double AcceptThreshold
        {
            get { return m_dAcceptThreshold; }
            set
            {
                m_dAcceptThreshold = (double)value;
                m_CogPMAlignTool.RunParams.AcceptThreshold = m_dAcceptThreshold;
            }
        }

        public double ContrastThreshold
        {
            get { return m_dContrastThreshold; }
            set
            {
                m_dContrastThreshold = (double)value;
                m_CogPMAlignTool.RunParams.ContrastThreshold = m_dContrastThreshold;
            }
        }

        public int NumberToFind
        {
            get { return m_nNumToFind; }
            set
            {
                m_nNumToFind = (int)value;
                m_CogPMAlignTool.RunParams.ApproximateNumberToFind = m_nNumToFind;
            }
        }

        public bool IsFound
        {
            get { return m_bFound; }
        }

        public double ZoneAngleHigh
        {
            get { return CogMisc.RadToDeg(m_Angle.High); }
            set
            {
                m_Angle.High = (double)CogMisc.DegToRad(value);
                m_CogPMAlignTool.RunParams.ZoneAngle.High= m_Angle.High;
            }
        }

        public double ZoneAngleLow
        {
            get { return CogMisc.RadToDeg(m_Angle.Low); }
            set
            {
                m_Angle.Low = (double)CogMisc.DegToRad(value);
                m_CogPMAlignTool.RunParams.ZoneAngle.Low = m_Angle.Low;
            }
        }

        public double ZoneScaleHigh
        {
            get { return m_Scale.High; }
            set
            {
                m_Scale.High = (double)value;
                m_CogPMAlignTool.RunParams.ZoneScale.High = m_Scale.High;
            }
        }

        public double ZoneScaleLow
        {
            get { return m_Scale.Low; }
            set
            {
                m_Scale.Low = (double)value;
                m_CogPMAlignTool.RunParams.ZoneScale.Low = m_Scale.Low;
            }
        }

        public bool UsingClutter
        {
            get { return m_IsUsingClutter; }
            set
            {
                m_IsUsingClutter = (bool)value;
                m_CogPMAlignTool.RunParams.ScoreUsingClutter = m_IsUsingClutter;
            }
        }

        public bool IgnorePolarity
        {
            get { return m_IsIgnorePolarity; }
            set
            {
                m_IsIgnorePolarity = (bool)value;
                m_CogPMAlignTool.Pattern.IgnorePolarity = m_IsIgnorePolarity;
            }
        }

        public int FoundCount
        {
            get { return m_nFoundCount; }
        }

        public double Score
        {
            get { return m_dScore; }
        }

        public bool Result
        {
            get { return m_bResult; }
        }

        public double X
        {
            get { return m_dTranslationX; }
        }

        public double Y
        {
            get { return m_dTranslationY; }
        }

        public double Angle
        {
            get { return m_dAngle; }
        }

        public double Scale
        {
            get { return m_dScale; }
        }

        public string Name
        {
            get { return m_CogPMAlignTool.Name; }
            set { m_CogPMAlignTool.Name = (string)value; }
        }

        public CogImage8Grey MaskImage
        {
            get { return m_ICogImgMask; }
        }

        public VisionToolRegionShape SearchShape
        {
            get
            {
                if (m_CogPMAlignTool.SearchRegion is CogCircle)
                {
                    return VisionToolRegionShape.Circle;
                }
                else if (m_CogPMAlignTool.SearchRegion is CogEllipse)
                {
                    return VisionToolRegionShape.Ellipse;
                }
                else if (m_CogPMAlignTool.SearchRegion is CogRectangle)
                {
                    return VisionToolRegionShape.Rectangle;
                }
                else if (m_CogPMAlignTool.SearchRegion is CogRectangleAffine)
                {
                    return VisionToolRegionShape.RectangleAffine;
                }
                else if (m_CogPMAlignTool.SearchRegion is CogCircularAnnulusSection)
                {
                    return VisionToolRegionShape.CircularAnnulusSection;
                }
                else if (m_CogPMAlignTool.SearchRegion is CogEllipticalAnnulusSection)
                {
                    return VisionToolRegionShape.EllipticalAnnulusSection;
                }
                else
                {
                    return VisionToolRegionShape.EntireImage;
                }
            }
        }

        public VisionToolRegionShape PatternShape
        {
            get
            {
                if (m_CogPMAlignTool.Pattern.TrainRegion is CogCircle)
                {
                    return VisionToolRegionShape.Circle;
                }
                else if (m_CogPMAlignTool.Pattern.TrainRegion is CogEllipse)
                {
                    return VisionToolRegionShape.Ellipse;
                }
                else if (m_CogPMAlignTool.Pattern.TrainRegion is CogRectangle)
                {
                    return VisionToolRegionShape.Rectangle;
                }
                else if (m_CogPMAlignTool.Pattern.TrainRegion is CogRectangleAffine)
                {
                    return VisionToolRegionShape.RectangleAffine;
                }
                else if (m_CogPMAlignTool.Pattern.TrainRegion is CogCircularAnnulusSection)
                {
                    return VisionToolRegionShape.CircularAnnulusSection;
                }
                else if (m_CogPMAlignTool.Pattern.TrainRegion is CogEllipticalAnnulusSection)
                {
                    return VisionToolRegionShape.EllipticalAnnulusSection;
                }
                else
                {
                    return VisionToolRegionShape.EntireImage;
                }
            }
        }

        // 2014.07.28
        public CogPMAlignTrainModeConstants TrainMode
        {
            get { return m_CogPMAlignTool.Pattern.TrainMode; }
            set { m_CogPMAlignTool.Pattern.TrainMode = (CogPMAlignTrainModeConstants)value; }
        }

        // 2014.07.28
        public ICogShapeModelCollection TrainShapeModels
        {
            get { return m_CogPMAlignTool.Pattern.TrainShapeModels; }
            set { m_CogPMAlignTool.Pattern.TrainShapeModels = value as ICogShapeModelCollection; }
        }

        // 2014.07.28
        public CogRegionModeConstants TrainRegionMode
        {
            get { return m_CogPMAlignTool.Pattern.TrainRegionMode; }
            set { m_CogPMAlignTool.Pattern.TrainRegionMode = (CogRegionModeConstants)value; }
        }

        // 2014.07.30
        public CogPMAlignLastRunRecordDiagConstants LastRunRecordDiagEnable
        {
            get { return m_CogPMAlignTool.LastRunRecordDiagEnable; }
            set { m_CogPMAlignTool.LastRunRecordDiagEnable = (CogPMAlignLastRunRecordDiagConstants)value; }
        }
      static  bool bShow = true;
        public bool LoadPattern(string filepath = "")
        {
            if (filepath.Length > 0)
            {
                try
                {
                    m_CogPMAlignTool.Pattern = CogSerializer.LoadObjectFromFile(filepath, typeof(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter), CogSerializationOptionsConstants.Minimum) as CogPMAlignPattern;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese?string.Format("{0}加载完成", m_CogPMAlignTool.Name): string.Format("{0} Loading completed      ({0}加载完成)", m_CogPMAlignTool.Name));
                    return true;
                }
                catch (Exception ex)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese ? string.Format("{0}加载失败！{1}", m_CogPMAlignTool.Name, ex.Message): string.Format("{0} Loading failed! {1}       ({0}加载失败！{1})", m_CogPMAlignTool.Name, ex.Message));
                  if(bShow)  MessageBox.Show(VAR.IsChinese ? string.Format("{0}加载失败!\r\n文件不存在，或配置文件遭到破坏！", Path.GetFileName(filepath)) :string.Format("{0}Load failed!\r\n The file does not exist or the configuration file is corrupted! \r\n{0}加载失败!\r\n文件不存在，或配置文件遭到破坏！", Path.GetFileName(filepath)), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    bShow = false;
                }
            }
            return false;
        }

        public bool Save(string filepath = "")
        {
            try
            {
                CogSerializer.SaveObjectToFile(m_CogPMAlignTool.Pattern, filepath, typeof(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter), CogSerializationOptionsConstants.Minimum);
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese?string.Format("{0}保存成功！{1}", m_CogPMAlignTool.Name, filepath): string.Format(" {0}  save successfully! {1}    ({0}保存成功！{1})", m_CogPMAlignTool.Name, filepath));
                MessageBox.Show(VAR.IsChinese?string.Format("{0}保存成功!\r\n{1}", m_CogPMAlignTool.Name, filepath): string.Format("{0}Saved successfully\r\n{1}\r\n{0}保存成功!\r\n{1}", m_CogPMAlignTool.Name, filepath), "信息", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return true;
            }
            catch (Exception ex)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese?string.Format("{0}保存失败！{1}", m_CogPMAlignTool.Name, ex.Message): string.Format("{0} save failed! {1}     ({0}保存失败！{1})", m_CogPMAlignTool.Name, ex.Message));
                MessageBox.Show(VAR.IsChinese ? string.Format("{0}保存失败!\r\n{1}", m_CogPMAlignTool.Name, filepath): string.Format("{0}Saved failed!\r\n{1}\r\n{0}保存失败!\r\n{1}", m_CogPMAlignTool.Name, filepath), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return false;
            }

        }
    }
}