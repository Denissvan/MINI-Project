using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MotionCtrl;

namespace UI
{
    public partial class warning : Form
    {
         public WS ws = null;
         public List<int> ws_num =new List<int>();
         public string title="";
         
        public warning()
        {
            InitializeComponent();
            //增加语言
            btn_cancle.Text = MultiLanguage.TxtSelct("取消", "Cancel", "Hủy bỏ");
            btn_ok.Text = MultiLanguage.TxtSelct("确定", "OK", "Đảm bảo");
            btn_abort.Text = MultiLanguage.TxtSelct("放弃", "Abort", "từ bỏ");
            btn_cancle.Visible = false;
            btn_abort.Visible = false;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            DRpt.Report_Opration(1000, 0, title+"  处理:"+btn_ok.Text+"按键按下!");
            this.Close();
        }

        private void btn_cancle_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            DRpt.Report_Opration(1000, 0, title + "  处理:" + btn_cancle.Text + "按键按下!");
            this.Close();
        }

        private void btn_abort_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            DRpt.Report_Opration(1000, 0, title + "  处理:" + btn_abort.Text + "按键按下!");
            this.Close();
        }

        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int i = Convert.ToInt16(btn.Text);
            WS.MdDat md=ws.list_md.Find(s => s.Num.Equals(i));
            if (md.res >= 0 && btn.BackColor != Color.Silver)
            {
                //  md.res = -2;
                btn.BackColor = Color.Silver;
            }
            else if(md.res >= 0 && btn.BackColor == Color.Silver)
            {
                if (md.res == 0) ((Button)btn).BackColor = Color.Lime;
                else if (md.res > 0) ((Button)btn).BackColor = Color.Red;
            }
        }
        public void LoadWs()
        {
          
            if (pnl_ws.Visible)
               {
                   foreach (WS.MdDat md in ws.list_md)
                   {
                       object btn = this.Controls.Find("btn" + md.Num, true)[0];
                       if (btn != null)
                       {
                           if (md.res == -2) ((Button) btn).BackColor = Color.Silver;
                           else if (md.res == -1) ((Button) btn).BackColor = Color.SkyBlue;
                           else if (md.res == 0) ((Button) btn).BackColor = Color.Lime;
                           else if (md.res > 0) ((Button) btn).BackColor = Color.Red;
                       }
                   }
               }
               else if (pnl_wspos.Visible && ws_num.Count>0)
               {
                   for (int i = 1; i < 17; i++)
                   {
                       object btn = this.Controls.Find("btn_pos" + i, true)[0];
                       if (btn != null)
                       {
                           if(ws_num.Contains(i)) ((Button)btn).BackColor = Color.Red;
                           else ((Button)btn).BackColor = Color.Lime;
                       }                      
                    }                   
               }        
        
        }
        private void warning_Load(object sender, EventArgs e)
        {
            if (VAR.IsChinese)
            {
                lb_msg.Font = new System.Drawing.Font("宋体", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            }
            else
            {
                lb_msg.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            }
            if (pnl_ws.Visible)
            {
                if (ws == null) return;
                pnl_un.BackColor = Color.Silver;
                pnl_ut.BackColor = Color.SkyBlue;
                pnl_ok.BackColor = Color.Lime;
                pnl_ng.BackColor = Color.Red;
            }
            else if (pnl_wspos.Visible)
            {
                pnl_posok.BackColor = Color.Lime;
                pnl_posng.BackColor = Color.Red;
            }          
            LoadWs();            
        }

        private void btn_Allun_Click(object sender, EventArgs e)
        {
            if (ws == null) return;
            foreach (WS.MdDat md in ws.list_md)
            {
                object btn = this.Controls.Find("btn" + md.Num, true)[0];
                if (btn != null&& md.res>-1)
                {
                    ((Button)btn).BackColor = Color.Silver;
                }
            }
        }

        private void btn_recovery_Click(object sender, EventArgs e)
        {
            if (ws == null) return;
            LoadWs();
        }

        private void btn_currun_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Retry;
            foreach (WS.MdDat md in ws.list_md)
            {
                object btn = this.Controls.Find("btn" + md.Num, true)[0];
                if (((Button)btn).BackColor == Color.Silver && md.res>=0)
                    md.res = -2;
            }
            this.Close();
        }     
    }
}
