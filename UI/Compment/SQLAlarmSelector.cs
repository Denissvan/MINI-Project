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
    public partial class SQLAlarmSelector : UserControl
    {
        public EventHandler HandlerSelect = null;
        public SQLAlarmSelector()
        {
            InitializeComponent();
            dtpicker_from.Value = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
            dtpicker_end.Value = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));
        }

        public void ClearCondition()
        {
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
