using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MotionCtrl;

namespace UI
{
    public partial class tray : UserControl
    {
        public bool bNgCfgMode = false;
        public bool bShowMode = false;
        public Product.Tray tray_dat = new Product.Tray(6,8);
        List<Rectangle> list_rect = new List<Rectangle>();
        private Rectangle rect_posinf = new Rectangle();
        Point m_start;
        bool bm_down = false;
        bool ben_edit = false;
        int tmr = 0;
        int chksum = 0;

        public class TrayClikEventArgs : EventArgs
        {
            public Product.Tray.PosInf PosInf;
        }
        public event EventHandler CellClik = null;

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
        public string TrayName
        {
            get { return lb_disc.Text; }
            set
            {
                lb_disc.Text = value;
                base.Invalidate(true);
            }
        }
        public Color TrayColor
        {
            get { return lb_disc.BackColor; }
            set
            {
                lb_disc.BackColor = value;
                base.Invalidate(true);
            }
        }
        public tray()
        {
            InitializeComponent();
        }

        public void UpdateShow()
        {
            int temp = tray_dat == null ?0:1;
            int tray_dat_cout=0;
            string traybarcode;
            if (tray_dat != null && tray_dat.col != 0 && tray_dat.row != 0)
            {
                if (lb_disc.BackColor == Color.DarkOrange) tray_dat_cout = tray_dat.count_md_untest;
                else if (lb_disc.BackColor == Color.Lime) tray_dat_cout = tray_dat.count_md_ok;
                else if (lb_disc.BackColor == Color.Red) tray_dat_cout = tray_dat.count_md_ng;
                traybarcode = tray_dat.barcode;
                if (tray_dat.barcode == "BARCODE")
                {
                    traybarcode = "";
                }
                lb_status.Text = string.Format("{0}  {1}/{2}", traybarcode, tray_dat_cout, tray_dat.count_pos_all);
            }
            else
            {
                lb_status.Text = string.Format("NO TRAY");
            }                     
            
            // 3sec
            if (tmr++ > 5)
            {
                tmr = 0;
                ben_edit = false;
            }

           // if(chksum != temp || ben_edit || bNgCfgMode|| bShowMode)
            {
                pnl_tray.Refresh();
                chksum = temp;
            }
        }

        private void pnl_tray_Paint(object sender, PaintEventArgs e)
        {
            //get buf
            BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
            BufferedGraphics myBuffer = currentContext.Allocate(e.Graphics, e.ClipRectangle);
            Graphics gg = myBuffer.Graphics;
            gg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            gg.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
            gg.Clear(BackColor);

            //draw at buf
            Pen p = new Pen(Bordercolor, 2);

            SolidBrush br = new SolidBrush(OKcolor);
            Font ft = new Font("宋体", 12);

            
            if (tray_dat != null && tray_dat.col > 0 && tray_dat.row > 0)
            {
                bool bswRowCol = tray_dat.list_pos.Count > 1 && Math.Abs(tray_dat.list_pos[0].Pos[0].x - tray_dat.list_pos[1].Pos[0].x) <
                    Math.Abs(tray_dat.list_pos[0].Pos[0].y - tray_dat.list_pos[1].Pos[0].y);

                float w = e.ClipRectangle.Width / (float)tray_dat.col;
                float h = e.ClipRectangle.Height / (float)tray_dat.row;

                foreach (Product.Tray.PosInf pos in  tray_dat.list_pos)
                {
                    if (bNgCfgMode &&tray_dat.NGDef != null)
                    {
                        Color cl = br.Color;
                        tray_dat.NGDef.GetColorByIdx(pos.idx, ref cl);
                        p.Color = cl;
                        if (bNgCfgMode || pos.md != null) br.Color = cl;
                        else br.Color = UNcolor;
                    }
                    else
                    {
                        if (pos.md == null || pos.md.res < -1) br.Color = UNcolor;
                        else if (pos.md.res == -1) br.Color = UTcolor;
                        else if (pos.md.res == 0) br.Color = OKcolor;
                        else if (pos.md.res >0 ) br.Color = ERRcolor;
                    }
                    
                    //set rect
                    Rectangle rect = new Rectangle();
                    if (!bswRowCol)
                    {
                        rect.X = (int) (pos.Col * w + 0.5);
                        rect.Y = (int) (pos.Row * h + 0.5);
                    }
                    else
                    {
                        rect.X = (int)(pos.Row * w + 0.5);
                        rect.Y = (int)(pos.Col * h + 0.5);
                    }

                    rect.Width = (int)(w + 0.5);
                    rect.Height = (int)(h + 0.5);
                    pos.rect = rect;
                    gg.DrawRectangle(p, rect);
                    gg.FillRectangle(br, rect);

                    //mask
                    if (!bNgCfgMode &&(pos.isDisable||pos.isDisableTemp))
                    {
                        if (br.Color == Color.Red)
                        {
                            p.Color = Color.White;
                        }
                        else
                        {
                            p.Color = Color.Red;
                        }
                        gg.DrawLine(p, rect.Location, new Point(rect.X + rect.Width, rect.Y + rect.Height));
                        gg.DrawLine(p, new Point(rect.X + rect.Width, rect.Y), new Point(rect.X, rect.Y + rect.Height));
                        p.Color = Bordercolor;
                    }
                    //idx
                    string str = pos.idx.ToString();
                    float ft_w = gg.MeasureString(str, ft).Width;
                    float ft_h = gg.MeasureString(str, ft).Height;
                    gg.DrawString(str, ft, Brushes.White, new PointF(rect.X + rect.Width / 2 - ft_w / 2, rect.Y + rect.Height / 2 - ft_h / 2));
                }
                
                if (ben_edit)
                {
                    //Rectangle rect = new Rectangle();
                    //rect.X = 0;
                    //rect.Y = 0;
                    //rect.Width = e.ClipRectangle.Width - 2;
                    //rect.Height = e.ClipRectangle.Height - 2;
                    p.Color = Color.BlueViolet;
                    p.DashStyle = DashStyle.DashDot;
                    p.Width = 2;
                    gg.DrawRectangle(p, rect_posinf);
                }
            }

            //show buf, then dispose
            myBuffer.Render(e.Graphics);
            gg.Dispose();
            myBuffer.Dispose();
        }

        private void pnl_tray_MouseDown(object sender, MouseEventArgs e)
        {
            if (tray_dat == null || bShowMode) return;
            if(ben_edit==false)
            {
                if (DialogResult.OK == MessageBox.Show(this, VAR.IsChinese?"确定要修改料盘物料状态?": "Are you sure you want to modify the tray material status?\r\n确定要修改料盘物料状态?", VAR.IsChinese?"提示": "Prompt", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                {
                    ben_edit = true;
                    tmr = 0;
                    return;
                }
            }
            tmr = 0;
            m_start.X = e.X;
            m_start.Y = e.Y;
            bm_down = true;
        }

        private void pnl_tray_MouseUp(object sender, MouseEventArgs e)
        {
            if (bShowMode)
            {
                Product.Tray.PosInf posInf = tray_dat.list_pos.Find(s => { return s.rect.Contains(e.X, e.Y); });
                if (posInf != null)
                {
                    TrayClikEventArgs args = new TrayClikEventArgs();
                    args.PosInf = posInf;
                    if(CellClik!=null)
                     CellClik.Invoke(sender, args);
                    pnl_tray.Refresh();
                }
                return;
            }

            if (tray_dat == null) return;
            if (ben_edit)
            {
                if (Math.Abs(e.X - m_start.X) < 3 && Math.Abs(e.Y - m_start.Y) < 3)
                {
                    Product.Tray.PosInf posInf = tray_dat.list_pos.Find(s => { return s.rect.Contains(e.X, e.Y); });
                    if (posInf != null)
                    {
                        posInf.isDisableTemp = !posInf.isDisableTemp;
                        if (bNgCfgMode && tray_dat.NGDef != null && tray_dat.NGDef.CurZone != null)
                        {
                            tray_dat.NGDef.InvertTrayIdx(posInf.idx);
                        }
                        pnl_tray.Refresh();
                    }                    
                }
                rect_posinf = new Rectangle();
            }
            bm_down =false;
            
        }

        private void pnl_tray_MouseMove(object sender, MouseEventArgs e)
        {
            if (!bm_down || !ben_edit || tray_dat == null || bNgCfgMode) return;
            int w = e.X - m_start.X;
            int h = e.Y - m_start.Y;
            rect_posinf = new Rectangle(Math.Min(m_start.X, e.X), Math.Min(m_start.Y, e.Y), Math.Abs(w), Math.Abs(h));
            foreach (Product.Tray.PosInf posInf in tray_dat.list_pos)
            {
                if (posInf != null && (rect_posinf.Contains(posInf.rect.X + posInf.rect.Width / 2,
                        posInf.rect.Y + posInf.rect.Height  / 2)||rect_posinf.Contains(posInf.rect.X + posInf.rect.Width/8,
                        posInf.rect.Y + posInf.rect.Height*7/ 8)|| rect_posinf.Contains(posInf.rect.X + posInf.rect.Width*7 / 8,
                                           posInf.rect.Y + posInf.rect.Height/ 8) || rect_posinf.Contains(posInf.rect.X + posInf.rect.Width / 8,
                        posInf.rect.Y + posInf.rect.Height / 8) || rect_posinf.Contains(posInf.rect.X + posInf.rect.Width*7/ 8,
                                           posInf.rect.Y + posInf.rect.Height*7/ 8)))
                {
                    posInf.isDisableTemp = w < 0 || h < 0 ? false : true;
                   // pnl_tray.Refresh();
                }

            }
            //Product.Tray.PosInf posInf = tray_dat.list_pos.Find(s => { return rect.Contains(s.rect.X + s.rect.Width / 2, s.rect.Y + s.rect.Height / 2); });
            //if (posInf != null)
            //{
            //    posInf.isDisable = w < 0 || h < 0 ? false : true;
            //    pnl_tray.Refresh();
            //}
        }
    }
}
