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
    public partial class UpLoad : UserControl
    {
        public UpLoad()
        {
            InitializeComponent();
        }
        public void UpdateShow(UpDownLoad ud)
        {
            lb_name.Text = VAR.IsChinese?string.Format("上料[{0}]", ud.id + 1): string.Format("UpLoad[{0}]", ud.id + 1); 
            lb_sta.Text = string.Format("{0} [{1:f1}s]",Utility.GetDescription(ud.status_ud, VAR.IsChinese), ud.Ct_ud);
            lb_pos.Text = string.Format("X:{0:000.000}\nY:{1:000.000}\nZ:{2:000.000}\nU1:{3:000.000}\nU2:{4:000.000}", ud.ax_x._fenc_pos, ud.ax_y._fenc_pos, ud.ax_z._fenc_pos, ud.ax_u1._fenc_pos, ud.ax_u2._fenc_pos);
        }
    }
}
