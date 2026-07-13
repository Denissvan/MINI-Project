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
    public partial class Station : UserControl
    {
        public WS ws = new WS (0,null,null,null,null,null,null,null);
        bool bflash = false;

        #region 属性
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
        private Color _cl_un = Color.Gray;
        [DefaultValue(typeof(Color), "Brushes.Gray")]
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
        #endregion
        public void UpdateShow()
        {
            chk_f_on.Checked = false;
            chk_f_close.Checked = false;
            if (ws.list_cld_fr != null && ws.list_cld_fr.Count > 0)
            {
                if (ws.list_cld_fr.ElementAt(0).io_out1 != null && ws.list_cld_fr.ElementAt(0).io_out2 != null && ws.list_cld_fr.ElementAt(0).io_out1.isON && ws.list_cld_fr.ElementAt(0).io_out2.isOFF) chk_f_on.Checked = true;
                if (ws.list_cld_fr.ElementAt(0).io_sen_on != null && ws.list_cld_fr.ElementAt(0).io_sen_on.isON) chk_f_close.Checked = true;
            }

            chk_b_on.Checked = false;
            chk_b_close.Checked = false;
            if (ws.list_cld_bk != null && ws.list_cld_bk.Count > 0)
            {
                if (ws.list_cld_bk.ElementAt(0).io_out1 != null && ws.list_cld_bk.ElementAt(0).io_out2 != null && ws.list_cld_bk.ElementAt(0).io_out1.isON && ws.list_cld_bk.ElementAt(0).io_out2.isOFF) chk_b_on.Checked = true;
                if (ws.list_cld_bk.ElementAt(0).io_sen_on != null && ws.list_cld_bk.ElementAt(0).io_sen_on.isON) chk_b_close.Checked = true;
            }



            bflash = !bflash;
            switch (ws.Status)
            {
                case WS.EM_STA.UPLOAD:
                case WS.EM_STA.DOWNLOAD:
                case WS.EM_STA.TEST:
                    if (bflash) lb_status.BackColor = Color.Blue;
                    else lb_status.BackColor = BackColor;
                    break;

                case WS.EM_STA.UNKNOWN:
                case WS.EM_STA.HOME:
                case WS.EM_STA.LINKERR:
                    if (bflash) lb_status.BackColor = Color.Gold;
                    else lb_status.BackColor = BackColor;
                    break;

                case WS.EM_STA.ERR:
                    if (bflash) lb_status.BackColor = Color.Red;
                    else lb_status.BackColor = BackColor;
                    break;

                case WS.EM_STA.REDAY:
                default:
                    lb_status.BackColor = BackColor;
                    break;
            }
            lb_pos_idx.Text = string.Format("[{0}]", ws.pos_idx + 1);
            lb_status.Text = Utility.GetDescription(ws.Status, VAR.IsChinese);
            pnl_ws.Refresh();
        }

        public Station()
        {
            InitializeComponent();
        }
        private void pnl_ws_Paint(object sender, PaintEventArgs e)
        {
            //if (ws != null && ws.list_test != null && ws.list_test.Count > 0 && ws.list_test[0].Count > 0)
            //{
            //    //get buf
            //    BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
            //    BufferedGraphics myBuffer = currentContext.Allocate(e.Graphics, e.ClipRectangle);
            //    Graphics gg = myBuffer.Graphics;
            //    gg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            //    gg.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
            //    gg.Clear(BackColor);

            //    Pen p = new Pen(Bordercolor, 2);
            //    SolidBrush br = new SolidBrush(OKcolor);
            //    Font ft = new Font("宋体", 12);

            //    //工站间隔
            //    float ws_gap = 4;
            //    //每行工站数
            //    int ws_per_row = 3;
            //    //工站每行显示产品数
            //    int cm_per_row = 2;

            //    //名称
            //    int yofs = (int)(gg.MeasureString("T", ft).Height * 1.5 + 0.9);
            //    //工站size
            //    int ws_row_cnt = (ws.list_test.Count / ws_per_row);
            //    float ws_w = (float)(e.ClipRectangle.Width - ws_per_row * ws_gap) / (float)ws_per_row;
            //    float ws_h = (float)(e.ClipRectangle.Height - ws_row_cnt * (ws_gap+ yofs)) / (float)ws_row_cnt;
            //    //模组size
            //    float cm_w = ws_w / (float)(cm_per_row);
            //    float cm_h = ws_h / (float)(ws.list_test[0].Count / cm_per_row>0? ws.list_test[0].Count / cm_per_row:1);

            //    foreach (List<MCInf.TestResult> ls_res in ws.list_test)
            //    {
            //        int test_idx = ws.list_test.IndexOf(ls_res);
            //        int ws_x = (int)((test_idx % ws_per_row) * (ws_w + ws_gap) + 0.5);
            //        int ws_y = (int)((test_idx / ws_per_row ) * (ws_h + ws_gap ) + (test_idx / ws_per_row +1 )*yofs+ 0.5);

            //        gg.DrawString(ls_res.ElementAt(0).TestFunc, ft, Brushes.DarkGray, new PointF(ws_x, ws_y - gg.MeasureString("T", ft).Height - 4));
            //        //gg.DrawRectangle(p, new Rectangle((int)((test_idx * 4) * w)+test_idx * 3, 0,(int)w*4, (int)h * (ws.cm_row * ws.cm_col / 4)));
            //        foreach (MCInf.TestResult res in ls_res)
            //        {
            //            //set color
            //            switch (res.NGCode)
            //            {
            //                case 0:
            //                    br.Color = UTcolor;
            //                    break;
            //                case -1:
            //                    br.Color = UNcolor;
            //                    break;
            //                case 1:
            //                    br.Color = OKcolor;
            //                    break;
            //                default:
            //                    br.Color = NGcolor;
            //                    break;
            //            }
            //            int res_idx = ls_res.IndexOf(res);

            //            //set rect
            //            Rectangle rect = new Rectangle();
            //            rect.X = (int)((res_idx % cm_per_row) * cm_w + 0.5) + ws_x;
            //            rect.Y = (int)((res_idx / cm_per_row) * cm_h + 0.5) + ws_y;
            //            rect.Width = (int)cm_w;
            //            rect.Height = (int)cm_h;

            //            gg.DrawRectangle(p, rect);
            //            gg.FillRectangle(br, rect);
            //            //gg.DrawString(cm.index.ToString(), ft, Brushes.White, new PointF(rect.X + rect.Width / 2 - gg.MeasureString(cm.index.ToString(), ft).Width / 2, rect.Y + rect.Height / 2 - gg.MeasureString(cm.index.ToString(), ft).Height / 2));
            //        }
            //    }
            //    //show buf, then dispose
            //    myBuffer.Render(e.Graphics);
            //    gg.Dispose();
            //    myBuffer.Dispose();
            //}
        }
    }
}
