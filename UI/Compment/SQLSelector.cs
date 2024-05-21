using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class SQLSelector : UserControl
    {
        public EventHandler HandlerSelect = null;
        public SQLSelector()
        {
            InitializeComponent();
            dtpicker_from.Value = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
            dtpicker_end.Value = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));
        }

        public void ClearCondition()
        {
            tb_barcode.Clear();
            cb_ws.Text = "";
            cb_testbox.Text = "";
            cb_pc.Text = "";
            cb_num.Text = "";
            cb_res.Text = "";

            dtpicker_from.Value = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
            dtpicker_end.Value = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));
        }


        public DateTime DateTimeForm
        {
            get
            {
                return dtpicker_from.Value;
            }
        }
        public DateTime DateTimeEnd
        {
            get
            {
                return dtpicker_end.Value;
            }
        }


        public string Condition
        {
            get
            {
                string select = "";
                if (tb_barcode.Text.Length > 0) select = string.Format("{0}BARCODE = '{1}'", select.Length == 0 ? "" : " and ", tb_barcode.Text.Length == 0 ? "" : tb_barcode.Text);
                if (cb_ws.Text.Length > 0) select += string.Format("{0}WS_ID = {1}", select.Length == 0 ? "" : " and ", cb_ws.Text);
                if (cb_pc.Text.Length > 0) select += string.Format("{0}PC_ID = {1}", select.Length == 0 ? "" : " and ", cb_pc.Text);
                if (cb_testbox.Text.Length > 0) select += string.Format("{0}BOX_ID = {1}", select.Length == 0 ? "" : " and ", cb_testbox.Text);
                if (cb_num.Text.Length > 0) select += string.Format("{0}NUM = {1}", select.Length == 0 ? "" : " and ", cb_num.Text);
                if (cb_res.Text.Length > 0)
                {
                    if(cb_res.Text == "OK") select += string.Format("{0}RES = 0", select.Length == 0 ? "" : " and ", cb_res.Text);
                    else if (cb_res.Text == "NG") select += string.Format("{0}RES > 0", select.Length == 0 ? "" : " and ", cb_res.Text);
                    else if (cb_res.Text == "未测") select += string.Format("{0}RES = -1", select.Length == 0 ? "" : " and ", cb_res.Text);
                    else select += string.Format("{0}RES like '{1}'", select.Length == 0 ? "" : " and ", cb_res.Text);
                }
                if(rbtn_NorProduct.Checked) select += string.Format("{0}PUD = '正常'", select.Length == 0 ? "" : " and ");
                else if(rbtn_RetProduct.Checked) select += string.Format("{0}PUD = '复测'", select.Length == 0 ? "" : " and ");
                return select;
            }
        }
        public string Lable
        {
            get
            {
                return lb_data_status.Text;
            }
            set
            {
                lb_data_status.Text = value;
            }            
        }

        private void btn_select_Click(object sender, EventArgs e)
        {
            btn_select.Enabled = false;
            HandlerSelect(sender, e);
            Application.DoEvents();
            btn_select.Enabled = true;
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            ClearCondition();
            HandlerSelect(sender, e);
        }
    }
}
