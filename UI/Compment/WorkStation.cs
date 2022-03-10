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

namespace UI
{
    public partial class WorkStation : UserControl
    {
        public WS ws = new WS (0,null,null,null,null,null,null,null);
        bool bflash = false;
        bool bshowid = false;

        private Color _bordercolor = Color.DodgerBlue;
        [DefaultValue(typeof(Color), "Color.DodgerBlue")]
        [Description("描边色")]
        public Color Bordercolor
        {
            get { return _bordercolor; }
            set
            {
                _bordercolor = value;
                base.Invalidate(true);
            }
        }
        private Color _cl_ng = Color.Red;
        [DefaultValue(typeof(Color), "Brushes.Red")]
        [Description("NG颜色")]
        public Color NGcolor
        {
            get { return _cl_ng; }
            set
            {
                _cl_ng = value;
                base.Invalidate(true);
            }
        }
        private Color _cl_ok = Color.Lime;
        [DefaultValue(typeof(Color), "Brushes.Lime")]
        [Description("OK颜色")]
        public Color OKcolor
        {
            get { return _cl_ok; }
            set
            {
                _cl_ok = value;
                base.Invalidate(true);
            }
        }
        private Color _cl_un = Color.Silver;
        [DefaultValue(typeof(Color), "Brushes.Silver")]
        [Description("无料颜色")]
        public Color UNcolor
        {
            get { return _cl_un; }
            set
            {
                _cl_un = value;
                base.Invalidate(true);
            }
        }
        private Color _cl_ut = Color.SkyBlue;
        [DefaultValue(typeof(Color), "Brushes.SkyBlue")]
        [Description("待测颜色")]
        public Color UTcolor
        {
            get { return _cl_ut; }
            set
            {
                _cl_ut = value;
                base.Invalidate(true);
            }
        }
        private Color _cl_err = Color.Gold;
        [DefaultValue(typeof(Color), "Brushes.Gold")]
        [Description("异常颜色")]
        public Color ERRcolor
        {
            get { return _cl_err; }
            set
            {
                _cl_err = value;
                base.Invalidate(true);
            }
        }

        [DefaultValue(typeof(string), "名称")]
        [Description("名称")]
        public string WorkStationName
        {
            get { return lb_disc.Text; }
            set
            {
                lb_disc.Text = value;
                base.Invalidate(true);
            }
        }

        bool btsk = false;        
        public void UpdateShow()
        {
            //异步更新IO
            if (btsk == false)
            {
                Task tsk = new Task(() =>
                {
                    try
                    {
                        btsk = true;
                        ws._isFrUp = ws.isFrUp;
                        ws._isBkUp = ws.isBkUp;

                        //ws.ax_fr._fenc_pos = ws.ax_fr.fenc_pos;
                        //ws.ax_bk._fenc_pos = ws.ax_bk.fenc_pos;
                    }
                    finally
                    {
                        btsk = false;
                    }
                });
                tsk.Start();
            }

            chk_f_on.Checked = !ws._isFrUp;            
            chk_b_on.Checked = !ws._isBkUp;

            //chk_f_close.Checked = !(Math.Abs(ws.ax_fr._fenc_pos) < 1);
            //chk_b_close.Checked = !(Math.Abs(ws.ax_bk._fenc_pos) < 1);

            bflash = !bflash;
            WS.EM_PC_STA sta = ws.GetStatus;

            switch (sta)
            {
                case WS.EM_PC_STA.WAIT:
                case WS.EM_PC_STA.TEST:
                    if (bflash) lb_status.BackColor = Color.LightSkyBlue;
                    else lb_status.BackColor = BackColor;
                    break;

                case WS.EM_PC_STA.NOT_SAME_TESTIDX:
                case WS.EM_PC_STA.LINK_ERR:
                    if (bflash) lb_status.BackColor = Color.Gold;
                    else lb_status.BackColor = BackColor;
                    break;

                case WS.EM_PC_STA.DISABLE:
                    if (bflash) lb_status.BackColor = Color.Red;
                    else lb_status.BackColor = BackColor;
                    break;

                case WS.EM_PC_STA.READY:
                    if(ws.pos_idx == (int)Turntable.EM_STA.POS0 && (ws.Status == WS.EM_STA.UPLOAD|| ws.Status == WS.EM_STA.DOWNLOAD))
                    {
                        sta = WS.EM_PC_STA.UP_DOWN_LOAD;
                    }
                    else 
                        lb_status.BackColor = BackColor;
                    break;
                default:
                    lb_status.BackColor = BackColor;
                    break;
            }
            
            lb_pos_idx.Text = string.Format("{0} [{1}]  {2}", Utility.GetDescription((Turntable.EM_STA)ws.pos_idx, VAR.IsChinese), ws.list_md.Count > 0 ? ws.list_md[0].test_idx : 0, ws.TestStatus == WS.EM_TEST_STA.NEXT ? ((Environment.TickCount - ws.tmr) / 1000.0).ToString("F1") : "");
            lb_status.Text = Utility.GetDescription(sta, VAR.IsChinese);
            double ng_cnt = COUNT_DATA.ngcnt[ws.num];
            double all_cnt = ng_cnt + COUNT_DATA.okcnt[ws.num];
            double ng = ng_cnt / (all_cnt == 0 ? 1 : all_cnt)*100;
            if(ng<=5.0) lb_NG.BackColor=Color.Lime;
            else if(ng>5.0 && ng<=10.0) lb_NG.BackColor = Color.Orange;
            else if (ng > 10) lb_NG.BackColor = Color.Red;
            lb_NG.Text ="NG:"+ng.ToString("f1")+"%";
            if (ws.breschanged)
            {
                ws.breschanged = false;
                pnl_ws.Refresh();
            }            
        }

        public WorkStation()
        {
            InitializeComponent();
        }
        #region paint
        //private void pnl_ws_Paint(object sender, PaintEventArgs e)
        //{
        //    if (ws != null && ws.list_test != null && ws.list_test.Count>=0)
        //    {
        //        //get buf
        //        BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
        //        BufferedGraphics myBuffer = currentContext.Allocate(e.Graphics, e.ClipRectangle);
        //        Graphics gg = myBuffer.Graphics;
        //        gg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
        //        gg.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
        //        gg.Clear(BackColor);

        //        Pen p = new Pen(Bordercolor, 2);
        //        SolidBrush br = new SolidBrush(OKcolor);
        //        Font ft = new Font("宋体", 10);

        //        int yofs = (int)(gg.MeasureString("T", ft).Height +0.9);
        //        float w = (float)(e.ClipRectangle.Width - ws.list_test.Count * 4) / (float)(ws.list_test.Count * 4.0);
        //        float h = (float)(e.ClipRectangle.Height - yofs) / (float)(ws.cm_row * ws.cm_col / 4.0);

        //        foreach (List<MCInf.TestResult> ls_res in ws.list_test)
        //        {
        //            int test_idx = ws.list_test.IndexOf(ls_res);

        //            gg.DrawString(ls_res.ElementAt(0).TestFunc, ft, Brushes.DarkGray, new PointF((int)((4 * w - gg.MeasureString(ls_res.ElementAt(0).TestFunc, ft).Width) / 2 + (test_idx * 4) * w)+ test_idx * 4, 0));
        //            //gg.DrawRectangle(p, new Rectangle((int)((test_idx * 4) * w)+test_idx * 3, 0,(int)w*4, (int)h * (ws.cm_row * ws.cm_col / 4)));
        //            foreach (MCInf.TestResult res in ls_res)
        //            {
        //                //set color
        //                switch (res.NGCode)
        //                {
        //                    case 0:
        //                        br.Color = UTcolor;
        //                        break;
        //                    case -1:
        //                        br.Color = UNcolor;
        //                        break;
        //                    case 1:
        //                        br.Color = OKcolor;
        //                        break;
        //                    default:
        //                        br.Color = NGcolor;
        //                        break;
        //                }
        //                int res_idx = ls_res.IndexOf(res);

        //                //set rect
        //                Rectangle rect = new Rectangle();
        //                rect.X = (int)((res_idx % 4) * w + 0.5) + (int)( 4 * w + 0.5)* test_idx + test_idx * 4;
        //                rect.Y = (int)((res_idx / 4) * h + yofs + 0.5);
        //                rect.Width = (int)(w + 0.5);
        //                rect.Height = (int)(h + 0.5);

        //                gg.DrawRectangle(p, rect);
        //                gg.FillRectangle(br, rect);
        //                //gg.DrawString(cm.index.ToString(), ft, Brushes.White, new PointF(rect.X + rect.Width / 2 - gg.MeasureString(cm.index.ToString(), ft).Width / 2, rect.Y + rect.Height / 2 - gg.MeasureString(cm.index.ToString(), ft).Height / 2));
        //            }
        //        }                
        //        //show buf, then dispose
        //        myBuffer.Render(e.Graphics);
        //        gg.Dispose();
        //        myBuffer.Dispose();
        //    }
        //}
        #endregion
        #region paint
        private void pnl_ws_Paint(object sender, PaintEventArgs e)
        {
            if (ws != null && ws.list_md != null && ws.list_md.Count >= 0)
            {
                //get buf
                BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
                BufferedGraphics myBuffer = currentContext.Allocate(e.Graphics, e.ClipRectangle);
                Graphics gg = myBuffer.Graphics;
                gg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                gg.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                gg.Clear(BackColor);

                Pen p = new Pen(Bordercolor, 2);
                SolidBrush br = new SolidBrush(OKcolor);
                Font ft = new Font("宋体", 10);

                int yofs = 2;// (int)(gg.MeasureString("T", ft).Height + 0.9);
                float w = (float)(e.ClipRectangle.Width - 4) / 8;
                float h = (float)(e.ClipRectangle.Height - yofs*2) / 2;

                int test_idx = 0;
                int pc = -1;
                foreach (WS.MdDat md in ws.list_md)
                {
                    //set color
                    if (md.benable == false) br.Color = UNcolor;
                    else if (md.res == -2) br.Color = SystemColors.ButtonFace;
                    else if (md.res == 0) br.Color = OKcolor;
                    else if (md.res == -1) br.Color = UTcolor;
                    else br.Color = NGcolor;
                    if (pc == -1) pc = md.PC_ID;

                    if (pc == md.PC_ID) p.Color = Bordercolor;
                    else p.Color = Color.DarkOrange;

                    //set rect
                    Rectangle rect = new Rectangle();
                    rect.X = 2 + (int)((test_idx % 8) * w + 0.5);
                    rect.Y = (int)((test_idx / 8) * h + yofs + 0.9);
                    rect.Width = (int)(w + 0.5-3);
                    rect.Height = (int)(h + 0.5-3);

                    gg.DrawRectangle(p, rect);
                    gg.FillRectangle(br, rect);
                    if (md.res >= 0 || bshowid)
                    {
                        string str_res;
                        if (bshowid) str_res = string.Format("{0}:{1}/{2}",md.PC_ID,md.TestBox_ID,md.SN);
                        else str_res = md.res == 0 ? "OK" : md.res.ToString();
                        SizeF fs = gg.MeasureString(str_res, ft);
                        gg.DrawString(str_res, ft, Brushes.White, new PointF(rect.X + rect.Width / 2 - fs.Width / 2, rect.Y + rect.Height / 2 - fs.Height / 2));
                    }
                    test_idx++;
                }
                //show buf, then dispose
                myBuffer.Render(e.Graphics);
                gg.Dispose();
                myBuffer.Dispose();
            }
        }
        #endregion

        private void pnl_ws_DoubleClick(object sender, EventArgs e)
        {
            bshowid = !bshowid;
        }
    }
}
