using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI.Compment
{
    public partial class Turnplate : UserControl
    {
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
        private Color _cl_normal = Color.Lime;
        [DefaultValue(typeof(Color), "Color.Lime")]
        [Description("正常颜色")]
        public Color NormalColor
        {
            get { return _cl_normal; }
            set
            {
                _cl_normal = value;
                base.Invalidate(true);
            }
        }
        private Color _cl_err = Color.Red;
        [DefaultValue(typeof(Color), "Color.Red")]
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

        public Turnplate()
        {
            InitializeComponent();
        }

        public void UpdateShow()
        {
            pnl_sta.Refresh();
        }
        void DrawRect(WS ws, ref Graphics gg, int x,int y,int w,int h)
        {
            Rectangle rect = new Rectangle();
            rect.X = x - w / 2;
            rect.Y = y - h / 2;
            rect.Width = w;
            rect.Height = h;
            Pen p = new Pen(Bordercolor, 1);
            SolidBrush br = new SolidBrush(NormalColor);
            if (ws.Status == WS.EM_STA.ERR || ws.Status == WS.EM_STA.UNKNOWN) br.Color = ERRcolor;
            else if (ws.Status == WS.EM_STA.DOWNLOAD || ws.Status == WS.EM_STA.UPLOAD) br.Color = Color.Gold;
            gg.FillRectangle(br, rect);            
            //arrow
            if (ws.num == 0)
            {
                rect.X -= 2;
                rect.Y -= 2;
                rect.Width += 4;
                rect.Height += 4;
                p.Width = 2;
                gg.DrawRectangle(p, rect);
            }
            else gg.DrawRectangle(p, rect);

            //id
            Font ft = new Font("宋体", 12);
            string str = (ws.num + 1).ToString();
            float str_w = gg.MeasureString(str, ft).Width;
            float str_h = gg.MeasureString(str, ft).Height;
            gg.DrawString(str, ft, Brushes.DarkGray, new PointF(x - str_w / 2, y - str_h / 2));
            
        }
        private void pnl_sta_Paint(object sender, PaintEventArgs e)
        {
            if (e.ClipRectangle.Width == 0 || e.ClipRectangle.Height == 0) return;
            
            //get buf
            BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
            BufferedGraphics myBuffer = currentContext.Allocate(e.Graphics, e.ClipRectangle);
            Graphics gg = myBuffer.Graphics;
            gg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            gg.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
            gg.Clear(BackColor);

            if (COM.ws1.pos_idx < 0 || COM.ws1.pos_idx < 3) return;

            int deg = 360 / COM.list_ws.Count;

            int gg_h = (int)(e.ClipRectangle.Height * 0.8) / 2;
            int gg_w = (int)(e.ClipRectangle.Width * 0.8) / 2;
            int rect_h = (int)(gg_h * 0.4);
            int rect_w = (int)(gg_w * 0.4);

            gg.TranslateTransform(e.ClipRectangle.Width / 2, e.ClipRectangle.Height / 2);
            Pen p = new Pen(Bordercolor, 2);
            gg.DrawArc(p, -gg_w, -gg_h, gg_w * 2, gg_h * 2, 90, COM.ws1.pos_idx * -90);

            foreach (WS ws in COM.list_ws)
            {
                switch (ws.pos_idx)
                {
                    case 0:
                        //down
                        DrawRect(ws, ref gg, 0, gg_h, rect_w, rect_h);
                        break;
                    case 1:
                        //right
                        DrawRect(ws, ref gg, gg_w, 0, rect_h, rect_w);
                        break;
                    case 2:
                        //up
                        DrawRect(ws, ref gg, 0, -gg_h, rect_w, rect_h);
                        break;
                    case 3:
                        //left
                        DrawRect(ws, ref gg, -gg_w, 0, rect_h, rect_w);
                        break;
                }
            }

            //show buf, then dispose
            myBuffer.Render(e.Graphics);
            gg.Dispose();
            myBuffer.Dispose();
        }
    }
}
