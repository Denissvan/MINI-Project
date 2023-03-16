using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using MotionCtrl;

namespace UI
{
    public partial class WsModuelDef : UserControl
    {
        public WS ws = null;
        private bool getdata = true;
        public WsModuelDef()
        {
            InitializeComponent();
        }

        public void PmsEn(User.PERMISSION pms)
        {
            if (pms <= User.PERMISSION.Operator)
            {
                getdata = false;
                this.Enabled = false;
            }
            else
            {
                this.Enabled =true ;               
                if (pms >= User.PERMISSION.Engineer)
                {
                    for (int i = 0; i < dgv.ColumnCount; i++)
                        dgv.Columns[i].ReadOnly = true;
                    dgv.Columns["enable"].ReadOnly = false;//使用
                    getdata = false;
                    nud_dx.Enabled = false;
                    nud_dy.Enabled = false;
                    nud_jig_X.Enabled = false;
                    nud_jig_Y.Enabled = false;
                    btn_array.Enabled = false;
                    btn_cali.Enabled = false;
                    btn_start_test_flow.Enabled = true;
                    btn_next_flow.Enabled = true;
                    btn_PosCopy.Enabled = true;
                    btn_save.Enabled = true;
                }

                if (pms >= User.PERMISSION.Admin)
                {
                    dgv.Columns["X1"].ReadOnly = false;
                    dgv.Columns["Y1"].ReadOnly = false;
                    dgv.Columns["Z1"].ReadOnly = false;
                    dgv.Columns["X2"].ReadOnly = false;
                    dgv.Columns["Y2"].ReadOnly = false;
                    dgv.Columns["Z2"].ReadOnly = false;
                    getdata = true;
                    nud_dx.Enabled = true;
                    nud_dy.Enabled = true;
                    nud_jig_X.Enabled = true;
                    nud_jig_Y.Enabled = true;
                    btn_array.Enabled = true;
                    btn_cali.Enabled = true;
                }

                if (pms >= User.PERMISSION.SuperAdmin)
                {
                   // dgv.Columns["pc"].ReadOnly = false;
                    //dgv.Columns["textbox"].ReadOnly = false;
                   // dgv.Columns["SN"].ReadOnly = false;
                }
            }
        }

        public bool GetRowData(int RowID, ref WS.MdDat MdDat)
        {
            double x, y, z;
            if (RowID < 0 || dgv.Rows == null || RowID >= dgv.Rows.Count) return false;
            try
            {
                string str = dgv.Rows[RowID].Cells["Num"].Value==null?"": dgv.Rows[RowID].Cells["Num"].Value.ToString();
                MdDat.Num = str.Length>0? Convert.ToInt16(str) : -1;


                for (int i = 0; i < 2; i++)
                {
                    str = dgv.Rows[RowID].Cells["X"+(i + 1).ToString()].Value == null ? "" : dgv.Rows[RowID].Cells["X" + (i + 1).ToString()].Value.ToString();
                    x= str.Length > 0 ? Convert.ToDouble(str) : double.MaxValue;

                    str = dgv.Rows[RowID].Cells["Y" + (i + 1).ToString()].Value == null ? "" : dgv.Rows[RowID].Cells["Y" + (i + 1).ToString()].Value.ToString();
                    y= str.Length > 0 ? Convert.ToDouble(str) : double.MaxValue;

                    str = dgv.Rows[RowID].Cells["Z" + (i + 1).ToString()].Value == null ? "" : dgv.Rows[RowID].Cells["Z" + (i + 1).ToString()].Value.ToString();
                    z= str.Length > 0 ? Convert.ToDouble(str) : double.MaxValue;

                    if (rbtn_pickpos.Checked)
                    {
                        MdDat.st_pickpos[i].x = x;
                        MdDat.st_pickpos[i].y = y;
                        MdDat.st_pickpos[i].z = z;
                    }
                    else if (rbtn_campos.Checked)
                    {
                        MdDat.st_pos[i].x = x;
                        MdDat.st_pos[i].y = y;
                        MdDat.st_pos[i].z = z;
                    }
                    else if (rbtn_WsQrcodePos.Checked)
                    {
                        MdDat.st_CapQrcodePos[i].x = x;
                        MdDat.st_CapQrcodePos[i].y = y;
                        MdDat.st_CapQrcodePos[i].z = z;
                    }
                    else
                    {
                        MdDat.st_jigpos[i].x = x;
                        MdDat.st_jigpos[i].y = y;
                        MdDat.st_jigpos[i].z = z;
                    }
                      
                }
               
                //str = dgv.Rows[RowID].Cells["U"].Value == null ? "" : dgv.Rows[RowID].Cells["U"].Value.ToString();
                //MdDat.st_pos.a = str.Length > 0 ? Convert.ToDouble(str) : double.MaxValue;

                str = dgv.Rows[RowID].Cells["pc"].Value == null ? "" : dgv.Rows[RowID].Cells["pc"].Value.ToString();
                MdDat.PC_ID = str.Length > 0 ? Convert.ToInt32(str) :int.MaxValue;

                str = dgv.Rows[RowID].Cells["textbox"].Value == null ? "" : dgv.Rows[RowID].Cells["textbox"].Value.ToString();
                MdDat.TestBox_ID = str.Length > 0 ? Convert.ToInt32(str) : int.MaxValue;

                str = dgv.Rows[RowID].Cells["SN"].Value == null ? "" : dgv.Rows[RowID].Cells["SN"].Value.ToString();
                MdDat.SN = str.Length > 0 ? Convert.ToInt32(str) : int.MaxValue;

                str = dgv.Rows[RowID].Cells["enable"].Value == null ? "" : dgv.Rows[RowID].Cells["enable"].Value.ToString();
                MdDat.benable = str.Length > 0 ? Convert.ToBoolean(str) : true;

                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public void GetOtherData()
        {
            ws.pos_CapLiZhu.x = (double)FrMain.frproduct.nud_PosCapLZ_x.Value;
            ws.pos_CapLiZhu.y = (double)FrMain.frproduct.nud_PosCapLZ_y.Value;
            ws.Cap_LiZhu_Limit = (double) FrMain.frproduct.nud_PosCapLZ_Limit.Value;
        }

        public void ShowOtherData()
        {
            FrMain.frproduct.nud_PosCapLZ_y.Value=(decimal)ws.pos_CapLiZhu.y ;
            FrMain.frproduct.nud_PosCapLZ_x.Value = (decimal)ws.pos_CapLiZhu.x;
            FrMain.frproduct.nud_PosCapLZ_Limit.Value = (decimal)ws.Cap_LiZhu_Limit;
        }
        

       public bool SetRowData(int RowID, WS.MdDat MdDat)
        {
            double x=0,y=0,z=0;
            if (RowID < 0 || dgv.Rows == null || RowID >= dgv.Rows.Count) return false;
            try
            {
                dgv.Rows[RowID].Cells["Num"].Value = MdDat.Num;
               
                for (int i = 0; i < 2; i++)
                {
                    if (rbtn_pickpos.Checked)
                    {
                        x = MdDat.st_pickpos[i].x;
                        y = MdDat.st_pickpos[i].y;
                        z = MdDat.st_pickpos[i].z;
                        dgv.Rows[RowID].Visible = true;
                    }
                    else if (rbtn_campos.Checked)
                    {
                        x = MdDat.st_pos[i].x;
                        y = MdDat.st_pos[i].y;
                        z = MdDat.st_pos[i].z;
                        dgv.Rows[RowID].Visible = true;
                    }
                    else if(rabt_jigpos.Checked)
                    {
                        x = MdDat.st_jigpos[i].x;
                        y = MdDat.st_jigpos[i].y;
                        z = MdDat.st_jigpos[i].z;
                        if(MdDat.Num%2==0)//偶数编号不可见从1开始gy0123
                        {
                            dgv.Rows[RowID].Visible = false;
                        }

                    }
                    else
                    {
                        x = MdDat.st_CapQrcodePos[i].x;
                        y = MdDat.st_CapQrcodePos[i].y;
                        z = MdDat.st_CapQrcodePos[i].z;
                        dgv.Rows[RowID].Visible = true;
                    }
                    dgv.Rows[RowID].Cells["X" + (i + 1).ToString()].Value = x.ToString("F3");
                    dgv.Rows[RowID].Cells["Y" + (i + 1).ToString()].Value = y.ToString("F3");
                    dgv.Rows[RowID].Cells["Z" + (i + 1).ToString()].Value = z.ToString("F3");
                    //dgv.Rows[RowID].Cells["U"].Value = MdDat.st_pos.a.ToString("F3");
                }
               
                
                dgv.Rows[RowID].Cells["pc"].Value = MdDat.PC_ID;
                dgv.Rows[RowID].Cells["textbox"].Value = MdDat.TestBox_ID;
                dgv.Rows[RowID].Cells["SN"].Value = MdDat.SN;
                dgv.Rows[RowID].Cells["enable"].Value = MdDat.benable;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void FillTableWithMdDat(WS.MdDat MdDat, int row = -2)
        {
            if (dgv.Rows.Count == 0 || row < 0 || row >= dgv.Rows.Count) row = dgv.Rows.Add();
            //the last row
            if (row < 0) row = dgv.Rows.Count - 1;

            SetRowData(row, MdDat);
        }
        public void UpdateShow()
        {
            if (ws == null || ws.list_md.Count == 0)
            {
                dgv.Rows.Clear();
                return;
            }

            if (dgv.Rows.Count != ws.list_md.Count)
                dgv.Rows.Clear();
            for (int r = 0; r < ws.list_md.Count; r++)
            {               
                FillTableWithMdDat(ws.list_md.ElementAt(r), r);
            }
            dgv.Update();

               
        }

       

        private void btn_save_Click(object sender, EventArgs e)
        {
            if (ws == null) return;
            List<WS.MdDat> listtemp = new List<WS.MdDat>();
            listtemp.Clear();
            GetOtherData();
            bool bCheck = false;//是否已经设置过夹具扫码
            foreach (DataGridViewRow row in dgv.Rows)
            {
                WS.MdDat MdDat = new WS.MdDat();
                MdDat = ws.list_md[dgv.Rows.IndexOf(row)];
               bool  benable = MdDat.benable;//用来记录更改之前的状态                
                if (GetRowData(row.Index, ref MdDat))
                {
                    listtemp.Add(MdDat);
                    if(!PT_SET.bJigSan) continue;
                    if (ws.bjigSan) continue;
                    //判断夹具使能状态改变gy0123  由屏蔽状态更改为使能状态时候
                    if(MdDat.benable&& (!benable) &&!bCheck)
                    {
                        ws.bjigSan = true;
                        bCheck = true;//只设置一次
                    }
                }
            }
            ws.list_md = listtemp;
            EM_RES res = ws.SaveCfg();
            if (res != EM_RES.OK)
            {
                MessageBox.Show(VAR.IsChinese?"保存失败!": "Save failed!\r\n保存失败!", VAR.IsChinese?"提示": "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            res = ws.LoadCfg();
            if (res != EM_RES.OK)
            {
                MessageBox.Show(VAR.IsChinese ? "保存后加载失败!": "Load failed after saving!\r\n保存后加载失败!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            UpdateShow();
            ShowOtherData();
            MessageBox.Show(VAR.IsChinese?"保存成功!": "Saved successfully!\r\n保存成功!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ws == null || e.RowIndex < 0 || e.RowIndex > ws.list_md.Count) return;
            int id = 0;
            bool ret = false;
            if (e.ColumnIndex == 12)
            {
                try
                {
                    EM_RES res;
                    WS.MdDat MdDat = new WS.MdDat();
                    if (!GetRowData(e.RowIndex, ref MdDat)) return;
                    //增加夹具拍照位置gy0123
                    String msgE = string.Format(@"Are you sure you want to position the {0}? \r\n1. Press the 'Yes' button to 
                        position the loading module <1>. \r\n2. Press the 'No' key to position the loading module <2>. \r\nNote:
                        The positioning of the picking position is equal to the picking coordinate + tip deviation \r\n确定要定位【{1}】
                    ?\n 1.按'是'键上下料模块<1>定位。\n 2.按'否'键上下料模块<2>定位。\n注:取料位定位等于取料坐标+吸头偏差", rbtn_campos.Checked ?
                        "Photographing position" : rbtn_pickpos.Checked ? "Retrieving position":"Jig Pthoto Pos", rbtn_campos.Checked ? "拍照位" : rbtn_pickpos.Checked ? "取料位":"夹具位");
                    //增加夹具拍照位置gy0123
                    String msgC = string.Format("确定要定位【{0}】?\n 1.按'是'键上下料模块<1>定位。\n 2.按'否'键上下料模块<2>定位。\n注:取料位定位等于取料坐标+吸头偏差", rbtn_campos.Checked ? "拍照位" : rbtn_pickpos.Checked ? "取料位" : "夹具位");
                    DialogResult result=MessageBox.Show(VAR.IsChinese? msgC : msgE, VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes) id = 0;
                    else if (result == DialogResult.No) id = 1;
                    else return;
                    if (rbtn_campos.Checked)
                    {
                        res = MT.ZupMove(ref VAR.gsys_set.bquit, ref COM.List_UDLoad[id].ax_x, MdDat.st_pos[id].x, ref COM.List_UDLoad[id].ax_y, MdDat.st_pos[id].y,true);
                        if (res == EM_RES.OK)
                        {
                            FrMain.frproduct.ctb_product.SelectedIndex = 2;
                            FrMain.frproduct.ctb_cali.SelectedIndex = 0;
                            Thread.Sleep(300);
                            COM.List_UDLoad[id].upcam.FindTaskTriAndWait(CONST.ModUpFw, false);
                        }
                    }                    
                    else if (rbtn_pickpos.Checked)
                    {
                        res = MT.ZupMove(ref VAR.gsys_set.bquit, ref COM.List_UDLoad[id].ax_x, MdDat.st_pickpos[id].x+ COM.List_UDLoad[id].list_xt[0].st_vs_pos_xtshp.x, ref COM.List_UDLoad[id].ax_y, MdDat.st_pickpos[id].y+ COM.List_UDLoad[id].list_xt[0].st_vs_pos_xtshp.y, true);
                    }
                    else if(rabt_jigpos.Checked)//夹具拍照位置gy0123
                    {
                        res = MT.ZupMove(ref VAR.gsys_set.bquit, ref COM.List_UDLoad[id].ax_x, MdDat.st_jigpos[id].x, ref COM.List_UDLoad[id].ax_y, MdDat.st_jigpos[id].y, true);
                    }
                    else
                    {
                        res = MT.ZupMove(ref VAR.gsys_set.bquit, ref COM.List_UDLoad[id].ax_x, MdDat.st_CapQrcodePos[id].x, ref COM.List_UDLoad[id].ax_y, MdDat.st_CapQrcodePos[id].y, true);
                    }
                    if (res == EM_RES.OK)
                        MessageBox.Show(VAR.IsChinese ? "定位成功!" :"Positioning success!\r\n定位成功!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show(VAR.IsChinese ? "定位失败!": "Positioning failed!\r\n定位失败!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (e.ColumnIndex == 11)
            {
                try
                {
                        if (!getdata) return;
                        WS.MdDat MdDat = new WS.MdDat();
                        if (!GetRowData(e.RowIndex, ref MdDat)) return;
                    //增加夹具拍照位置gy0123
                    String msgE = string.Format(@" Learn the 【{0}】  coordinate position! \n 1 .
                        Press 'Yes' to learn the coordinates of updownload1.
                        \n 2.Press 'No' to learn the coordinates of updownload2. 
                        \n 3.Press 'Cancel' to exit. \n Note: Reclaim Bit coordinate is equal to
                        current coordinate-tip deviation\r\n学习【{0}】坐标位置！
                        \n 1.按'是'键学习上下料模块<1>坐标。\n 2.按'否'键学习上下料模块<2>坐标。
                        \n 3.按‘取消’键退出。\n 注:取料位坐标等于当前坐标-吸头偏差",
                        rbtn_campos.Checked ? "Photography position" : rbtn_pickpos.Checked ? "Retrieving position" : "Jig Pthoto Pos",
                        rbtn_campos.Checked ?  "拍照位" : rbtn_pickpos.Checked ? "取料位" : "夹具位");
                    //增加夹具拍照位置gy0123
                    String msgC = string.Format(@"学习【{0}】坐标位置！\n 1.按'是'键学习上下料模块<1>坐标。
                               \n 2.按'否'键学习上下料模块<2>坐标。
                              \n 3.按‘取消’键退出。\n 注:取料位坐标等于当前坐标-吸头偏差",
                               rbtn_campos.Checked ? "拍照位" : rbtn_pickpos.Checked ? "取料位" : "夹具位");
                    DialogResult result = MessageBox.Show(VAR.IsChinese? msgC : msgE
                            , VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                       if (result == DialogResult.Yes) id = 0;
                       else if (result == DialogResult.No) id = 1;
                       else return;
                    if (rbtn_campos.Checked)
                    {
                        MdDat.st_pos[id].x = COM.List_UDLoad[id].ax_x.fenc_pos;
                        MdDat.st_pos[id].y = COM.List_UDLoad[id].ax_y.fenc_pos;
                        MdDat.st_pos[id].z = COM.List_UDLoad[id].ax_z.fenc_pos;
                    }
                    else if(rbtn_pickpos.Checked)
                    {
                        MdDat.st_pickpos[id].x = COM.List_UDLoad[id].ax_x.fenc_pos- COM.List_UDLoad[id].list_xt[0].st_vs_pos_xtshp.x;
                        MdDat.st_pickpos[id].y = COM.List_UDLoad[id].ax_y.fenc_pos- COM.List_UDLoad[id].list_xt[0].st_vs_pos_xtshp.y;
                        MdDat.st_pickpos[id].z = COM.List_UDLoad[id].ax_z.fenc_pos;
                    }
                    else if(rabt_jigpos.Checked)//增加夹具拍照位置gy0123
                    {
                        MdDat.st_jigpos[id].x = COM.List_UDLoad[id].ax_x.fenc_pos;
                        MdDat.st_jigpos[id].y = COM.List_UDLoad[id].ax_y.fenc_pos;
                        MdDat.st_jigpos[id].z = COM.List_UDLoad[id].ax_z.fenc_pos;
                    }
                    else
                    {
                        MdDat.st_CapQrcodePos[id].x = COM.List_UDLoad[id].ax_x.fenc_pos;
                        MdDat.st_CapQrcodePos[id].y = COM.List_UDLoad[id].ax_y.fenc_pos;
                        MdDat.st_CapQrcodePos[id].z = COM.List_UDLoad[id].ax_z.fenc_pos;
                    }
                    //MdDat.st_pos[0].a = MT.AXIS_UL_U1.fenc_pos;
                    ret=SetRowData(e.RowIndex, MdDat);
                    
                    if (ret==true)
                    {
                        Thread.Sleep(100);
                        result = MessageBox.Show(VAR.IsChinese?"是否保存数据?": "Do you want to save the data?\r\n", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (result == DialogResult.Yes) btn_save_Click(null, null);
                        if (rbtn_campos.Checked && e.RowIndex == 0 && id==0)
                        {
                            result = MessageBox.Show(VAR.IsChinese ? "是否要进行数据阵列?": "Do you want a data array?\r\n是否要进行数据阵列?", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (result == DialogResult.No) return;
                            //Thread.Sleep(100);
                            btn_array_Click(null, null);
                            //Thread.Sleep(100);
                            btn_cali_Click(null,null);                            
                        }
                    }
                                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btn_start_test_flow_Click(object sender, EventArgs e)
        {
            //int ret = TestPC.StartTestFlow(0, "ABC#123".ToArray(), 1);
            //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("StartFlow,{0}", ret));
            ws.StartTestFlow();
            //TestPC.StartTestFlow(0, "".ToArray(), 1);
        }

        private void btn_next_flow_Click(object sender, EventArgs e)
        {          
            int status = 0;
            VAR.gsys_set.bquit = false;
            ws.WaitTestResult(ref status, PT_SET.TestTime); 
            ws.NextTest(status);
      
        }

        private void dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if ((e.RowIndex & 1) == 1) e.CellStyle.BackColor = Color.WhiteSmoke;
        }

        private void btn_array_Click(object sender, EventArgs e)
     {
            ST_XYZ cur_pos=new ST_XYZ();
         int cur_id = 0;
         if (dgv.CurrentRow.Index >= 0 && dgv.CurrentRow.Index < dgv.Rows.Count)
             cur_id = dgv.CurrentRow.Index;
        int cur_row = cur_id / 8;
        int cur_col = cur_id % 8;
        int RunMod = -1;
          
         DialogResult result;
        if (rbtn_campos.Checked)
        {
            //result = MessageBox.Show(string.Format("工站拍照位置阵列【当前选定行号:{0}】说明:\n 1.按'是'键以选定行进行△X,△Y阵列。\n 2.按'否'键以选定行的上下料模块1当前坐标进行△X,△Y阵列。\n 3.按'取消'退出", cur_id), "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
             result = MessageBox.Show(VAR.IsChinese?string.Format("工站拍照位置阵列【当前选定行号:{0}】说明:\n 1.按'是'键以选定行进行△X,△Y阵列。\n 2.按'否'键以上下料模块2选定行进行模块2 △X,△Y阵列。\n 3.按'取消'退出", cur_id): string.Format("Array of photo positions at the station 【currently selected line number: {0}】 Description: \r\n1. Press the 'Yes' key to perform a △ X, △ Y array on the selected line. \r\n 2. Press the 'No' button to cut the selected row above updownload2 to perform updownload 2 △ X, △ Y array. \r\n 3.Press 'Cancel' to exit \r\n工站拍照位置阵列【当前选定行号:{0}】说明:\n 1.按'是'键以选定行进行△X,△Y阵列。\n 2.按'否'键以上下料模块2选定行进行模块2 △X,△Y阵列。\n 3.按'取消'退出", cur_id), VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
             if (result == DialogResult.Yes)
             {
                 RunMod = 0;
                 cur_pos = ws.list_md.ElementAt(cur_id).st_pos[0];
             }
             else if (result == DialogResult.No) 
            {
                RunMod = 1;
                cur_pos = ws.list_md.ElementAt(cur_id).st_pos[1];
             }
            else return;
        }
            else if (rbtn_pickpos.Checked)
            {
            result = MessageBox.Show(VAR.IsChinese ? string.Format("工站取料位置阵列【当前选定行号:{0}】说明:\n 1.按'是'键以选定行与拍照位数据对比后差值进行阵列。\n 2.按'否'键以选定行进行上下料模块1的△X,△Y阵列。\n 3.按'取消'键以选定行进行上下料模块2的△X,△Y阵列", cur_id) : string.Format("Array of station picking position 【currently selected line number: {0}】 Description: \r\n 1. Press the 'Yes' key to perform an array by comparing the difference between the selected line and the photographed position data. \r\n 2. Press the 'No' key to select the △ X, △ Y array of the updownload 1 for the selected row. \r\n 3.Press the 'Cancel' key to select the line for the △ X, △ Y array of updownload 2\r\n工站取料位置阵列【当前选定行号:{0}】说明:\n 1.按'是'键以选定行与拍照位数据对比后差值进行阵列。\n 2.按'否'键以选定行进行上下料模块1的△X,△Y阵列。\n 3.按'取消'键以选定行进行上下料模块2的△X,△Y阵列", cur_id), VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                RunMod = 2;

            }
            else if (result == DialogResult.No)
            {
                RunMod = 3;
                cur_pos = ws.list_md.ElementAt(cur_id).st_pickpos[0];
            }
            else if (result == DialogResult.Cancel)
            {
                RunMod = 4;
                cur_pos = ws.list_md.ElementAt(cur_id).st_pickpos[1];
            }  
         }else//gy0123  夹具阵列学习
            {
                btn_jig_lean.PerformClick();
                return;
            }
            if (RunMod<0 || RunMod >4)
                MessageBox.Show(VAR.IsChinese?"没有找到相应的阵列流程": "No corresponding array process found\r\n没有找到相应的阵列流程", VAR.IsChinese?"错误":"Error", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
            //cur_pos.z = 0;
            // cur_pos.a = 0;
            double dx = (double)nud_dx.Value;
        double dy = (double)nud_dy.Value;

        List<WS.MdDat> list_temp = new List<WS.MdDat>();
        foreach (WS.MdDat md in ws.list_md) list_temp.Add(md.Clone());
        for (int row = 0; row < 2; row++)                
        {
            for (int col = 0; col < 8; col++)
            {
                if ((row * 8 + col) < list_temp.Count)
                {
                    if (PT_SET.bNonparallel)
                    {
                        if (!PT_SET.issmall)
                        {
                            if (PT_SET.bitOpenMode == 1 && ((row * 8 + col) % 2) != 0)
                                continue;
                            else if (PT_SET.bitOpenMode == 2 && ((row * 8 + col) % 2) == 0)
                                continue;
                            else if (PT_SET.bitOpenMode == 3 && ((row * 8 + col) < 8))
                                continue;
                            else if (PT_SET.bitOpenMode == 4 && ((row * 8 + col) > 7))
                                continue;
                        }
                    }
                    if (RunMod == 0)
                    {
                        list_temp.ElementAt(row * 8 + col).st_pos[0] = cur_pos + new ST_XYZ(dx * (col-cur_col), dy * (row-cur_row));
                        list_temp.ElementAt(row * 8 + col).st_pos[0].z = cur_pos.z;
                        list_temp.ElementAt(row * 8 + col).st_pos[1] = cur_pos + new ST_XYZ(dx * (col - cur_col), dy * (row - cur_row)) - COM.xt1.st_pos_samevs[0].ToXYZ() + COM.xt1.st_pos_samevs[1].ToXYZ();
                        list_temp.ElementAt(row * 8 + col).st_pos[1].z = cur_pos.z;
                    }
                   else if (RunMod == 1)
                    {
                        list_temp.ElementAt(row * 8 + col).st_pos[1] = cur_pos + new ST_XYZ(dx * (col - cur_col), dy * (row - cur_row));
                        list_temp.ElementAt(row * 8 + col).st_pos[1].z = cur_pos.z;
                    }
                   else if (RunMod == 2)
                   {
                       if (row * 8 + col != cur_id)
                       {
                           list_temp.ElementAt(row * 8 + col).st_pickpos[0] = ws.list_md.ElementAt(row * 8 + col).st_pos[0] + (ws.list_md.ElementAt(cur_id).st_pickpos[0] - ws.list_md.ElementAt(cur_id).st_pos[0]);
                           list_temp.ElementAt(row * 8 + col).st_pickpos[0].z = ws.list_md.ElementAt(cur_id).st_pickpos[0].z;
                           list_temp.ElementAt(row * 8 + col).st_pickpos[1] = ws.list_md.ElementAt(row * 8 + col).st_pos[1] + (ws.list_md.ElementAt(cur_id).st_pickpos[1] - ws.list_md.ElementAt(cur_id).st_pos[1]);
                           list_temp.ElementAt(row * 8 + col).st_pickpos[1].z = ws.list_md.ElementAt(cur_id).st_pickpos[1].z;
                        }
                       else
                       {
                           list_temp.ElementAt(row * 8 + col).st_pickpos[0] = ws.list_md.ElementAt(row * 8 + col).st_pickpos[0];
                           list_temp.ElementAt(row * 8 + col).st_pickpos[1] = ws.list_md.ElementAt(row * 8 + col).st_pickpos[1];
                       }
                      
                    }
                    else if (RunMod == 3)
                    {
                        list_temp.ElementAt(row * 8 + col).st_pickpos[0] = cur_pos + new ST_XYZ(dx * (col - cur_col), dy * (row - cur_row));
                        list_temp.ElementAt(row * 8 + col).st_pickpos[0].z = cur_pos.z;
                        list_temp.ElementAt(row * 8 + col).st_pickpos[1] = ws.list_md.ElementAt(row * 8 + col).st_pickpos[1];
                        }
                    else if (RunMod == 4)
                    {
                        list_temp.ElementAt(row * 8 + col).st_pickpos[1] = cur_pos + new ST_XYZ(dx * (col - cur_col), dy * (row - cur_row));
                        list_temp.ElementAt(row * 8 + col).st_pickpos[1].z = cur_pos.z;
                        list_temp.ElementAt(row * 8 + col).st_pickpos[0] = ws.list_md.ElementAt(row * 8 + col).st_pickpos[0];
                     }

                    }                        
                else
                {
                    MessageBox.Show(VAR.IsChinese?"工站Sorket数量异常！": "The number of sorkets in the workstation is abnormal!\r\n工站Sorket数量异常！", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
        dgv.Rows.Clear();
        for (int r = 0; r < list_temp.Count; r++)
        {
            FillTableWithMdDat(list_temp.ElementAt(r), r);
        }
         result = MessageBox.Show(VAR.IsChinese ? "是否保存阵列数据!": "Do you want to save array data!\r\n是否保存阵列数据!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
         if (result == DialogResult.No) return;
          btn_save_Click(null,null);
        }

        private void btn_cali_Click(object sender, EventArgs e)
        {
            //视觉校正
            try
            {
                rbtn_campos.Enabled = false;
                rbtn_pickpos.Enabled = false;
                rabt_jigpos.Enabled = false;//夹具拍照位置gy0123
                EM_RES res;
                ST_XYZA StMovPos = new ST_XYZA();
                VAR.gsys_set.bquit = false;
                List<WS.MdDat> list_temp = new List<WS.MdDat>();
                int id = 0;
                DialogResult result = MessageBox.Show(VAR.IsChinese?"工站视觉校正!\n 1.按'是'键上下料模块<1>校正。\n 2.按'否'键上下料模块<2>校正。\n 3.按'取消'键退出!": "Station vision correction! \n 1. Press the 'Yes' button to to correct the updownload 1. \n 2.Press the 'No' button to correct the updownload 2. \n 3.Press the 'Cancel' key to exit!\r\n工站视觉校正!\n 1.按'是'键上下料模块<1>校正。\n 2.按'否'键上下料模块<2>校正。\n 3.按'取消'键退出!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes) id = 0;
                else if (result == DialogResult.No) id = 1;
                else return;
                RECAIL:
                foreach (WS.MdDat md in ws.list_md) list_temp.Add(md.Clone());
                for (int row = 0; row < 2; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        if ((row * 8 + col) < list_temp.Count)
                        {
                            int idx = row * 8 + col;
                            if (!list_temp.ElementAt(row * 8 + col).benable) continue;//gy0123增加屏蔽工位不校正
                         
                            if (!PT_SET.issmall)
                            {
                                if (PT_SET.bitOpenMode==1 && ((row * 8 + col) % 2) != 0)
                                    continue;
                                else if (PT_SET.bitOpenMode == 2 && ((row * 8 + col) % 2) == 0)
                                    continue;
                                else if(PT_SET.bitOpenMode == 3 && ((row * 8 + col) < 8 ))
                                    continue;
                                else if (PT_SET.bitOpenMode == 4 && ((row * 8 + col) > 7))
                                    continue;
                            }
                            res = MT.ZupMove(ref VAR.gsys_set.bquit, ref COM.List_UDLoad[id].ax_x, list_temp.ElementAt(row * 8 + col).st_pos[id].x, ref COM.List_UDLoad[id].ax_y, list_temp.ElementAt(row * 8 + col).st_pos[id].y);
                            if (res != EM_RES.OK)
                            {
                                MessageBox.Show(VAR.IsChinese?string.Format("{0}定位失败！", COM.List_UDLoad[id].disc): string.Format("{0}Positioning failed!{1}定位失败！", COM.List_UDLoad[id].englishdisc, COM.List_UDLoad[id].disc), VAR.IsChinese ? "错误" : "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            COM.List_UDLoad[id].upcam.List_vs_task_cur.Clear();
                            COM.List_UDLoad[id].upcam.inputImageCnt = 0;
                            StMovPos.x = list_temp.ElementAt(row * 8 + col).st_pos[id].x;
                            StMovPos.y = list_temp.ElementAt(row * 8 + col).st_pos[id].y;
                            Cam.VisionTask VsTask = COM.List_UDLoad[id].upcam.List_vs_task.Find(s => s.TaskName.Equals("WsUp_Shp"));
                            COM.List_UDLoad[id].upcam.List_vs_task_cur.Add(VsTask);
                            res = COM.List_UDLoad[id].upcam.MoveToImgCenter(ref VAR.gsys_set.bquit, ref StMovPos, VsTask, COM.List_UDLoad[id].upcam.ListCaliTool);
                            if (res != EM_RES.OK)
                            {
                                MessageBox.Show(VAR.IsChinese?"工站Sorket对中失败！": "Sorket alignment failed!\r\n工站Sorket对中失败！", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            list_temp.ElementAt(row * 8 + col).st_pos[id].x = COM.List_UDLoad[id].ax_x.fenc_pos;
                            list_temp.ElementAt(row * 8 + col).st_pos[id].y = COM.List_UDLoad[id].ax_y.fenc_pos;
                        }
                        else
                        {
                            MessageBox.Show(VAR.IsChinese?"工站Sorket数量异常！": "The number of sorkets in the station is abnormal!\r\n工站Sorket数量异常！", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                dgv.Rows.Clear();
                for (int r = 0; r < list_temp.Count; r++)
                {
                    FillTableWithMdDat(list_temp.ElementAt(r), r);
                }
                //MessageBox.Show("视觉校正成功", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btn_save_Click(null, null);
                if (id == 0)
                {
                    result = MessageBox.Show(VAR.IsChinese ? "是否进行模块2视觉校正!\n 1.按'是'进行模块2视觉校正！\n 2.按'否'退出": "Do you want to perform updownload 2 visual correction! \n 1. Press 'Yes' to perform updownload 2 visual correction! \n 2.Press 'No' to exit\r\n是否进行模块2视觉校正!\n 1.按'是'进行模块2视觉校正！\n 2.按'否'退出", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        id = 1;
                        list_temp.Clear();
                        goto RECAIL;
                    }
                }
            }
            finally
            {
                rbtn_campos.Enabled = true;
                rbtn_pickpos.Enabled = true;
                rabt_jigpos.Enabled = true;//夹具拍照位置gy0123
            }          
        }

        private void rbtn_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton) sender;
            if (btn.Checked)
                UpdateShow();
            btn_cali.Enabled = rbtn_campos.Checked;
            if (rbtn_WsQrcodePos.Checked)
            {
                nud_jig_X.Value = 0;
                nud_jig_X.Enabled = false;
            }
            else
            {
                nud_jig_X.Enabled = true;
            }

        }
        public bool dgvupdate=false;
        public void dgvCopyData(string ColName)
        {
            // int cur_id = dgv.CurrentRow.Index;
            dgvupdate = true;
            dgv.EndEdit();
            string str = dgv.Rows[0].Cells[ColName].Value == null ? "" : dgv.Rows[0].Cells[ColName].Value.ToString();
            dgvupdate = false;
            if (str == "")
            {
                MessageBox.Show(VAR.IsChinese?string.Format("{0}列第一格数据为空,不能复制", ColName): string.Format("The first column of ({0})column is empty and cannot be copied\r\n{0}列第一格数据为空,不能复制", ColName), VAR.IsChinese ? "错误" : "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                return;
            }
            if (DialogResult.Yes != MessageBox.Show(VAR.IsChinese ? string.Format("确认是否以{0}第一格数据【{1}】复制整列?", ColName, str): string.Format("Are you sure to copy the entire column with the first grid data [{1}] of {0}?\r\n确认是否以{0}第一格数据【{1}】复制整列?", ColName, str), VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) return;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if(str == "True" || str=="False") row.Cells[ColName].Value = Convert.ToBoolean(str);
                else row.Cells[ColName].Value = Convert.ToDouble(str);

            }
            
        }

        private void dgv_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                dgvCopyData("Z1");
            }
            else if (e.ColumnIndex == 6)
            {
                dgvCopyData("Z2");
            }
            else if (e.ColumnIndex == 10)
            {
                dgvCopyData("enable");
            }
        }

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvupdate == true)
            {
                this.Validate();
                dgvupdate = false;
            }
          
        }

        private void btn_PosCopy_Click(object sender, EventArgs e)
        {
            EM_RES res;
            VAR.gsys_set.bquit = false;
            List<WS.MdDat> list_temp = new List<WS.MdDat>();
            foreach (WS.MdDat md in ws.list_md) list_temp.Add(md.Clone());
            string Msg;
            if (rbtn_campos.Checked)
                Msg = VAR.IsChinese?"工站拍照位复制说明:\n 1.按'是'键->复制工站1的位置信息到当前工站,复制完成后记得视觉校正!\n 2.按'否'键退出!": "Instructions for copying the photo station of the station: \n 1. Press the 'Yes' key-> copy the position information of workstation 1 to the current station, remember to correct the vision after copying! \n 2. Press the 'No' key to exit!\r\n工站拍照位复制说明:\n 1.按'是'键->复制工站1的位置信息到当前工站,复制完成后记得视觉校正!\n 2.按'否'键退出!";
            else if (rbtn_pickpos.Checked)
                Msg = VAR.IsChinese ? "工站放料位复制说明:\n 1.按'是'键->以工站1位置信息计算得到当前工站，执行前确认完成本站视觉校正!\n 2.按'否'键退出!": "Description of the station loading position copy: \n 1. Press the 'Yes' key-> Calculate the current station based on the position information of workstation 1 and confirm the visual correction of this station before execution! \n 2. Press the 'No' key to exit !\r\n工站放料位复制说明:\n 1.按'是'键->以工站1位置信息计算得到当前工站，执行前确认完成本站视觉校正!\n 2.按'否'键退出!";
            else
                Msg = VAR.IsChinese ? "工站夹具位复制说明:\n 1.按'是'键->以工站1位置信息计算得到当前工站，执行前确认完成本站视觉校正!\n 2.按'否'键退出!" : "Description of the station loading position copy: \n 1. Press the 'Yes' key-> Calculate the current station based on the position information of workstation 1 and confirm the visual correction of this station before execution! \n 2. Press the 'No' key to exit !\r\n工站放料位复制说明:\n 1.按'是'键->以工站1位置信息计算得到当前工站，执行前确认完成本站视觉校正!\n 2.按'否'键退出!";
            if (DialogResult.No == MessageBox.Show(Msg, VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))return;    
                       
            for (int row = 0; row < 2; row++)
            { 
                for (int col = 0; col < 8; col++)
                {
                    if ((row * 8 + col) < list_temp.Count)
                    {
                        for (int i = 0; i < COM.List_UDLoad.Count; i++)
                        {
                            if (rbtn_campos.Checked)
                            {
                                list_temp.ElementAt(row * 8 + col).st_pos[i].x = COM.ws1.list_md[row * 8 + col].st_pos[i].x;
                                list_temp.ElementAt(row * 8 + col).st_pos[i].y = COM.ws1.list_md[row * 8 + col].st_pos[i].y;
                                list_temp.ElementAt(row * 8 + col).st_pos[i].z = COM.ws1.list_md[row * 8 + col].st_pos[i].z;
                            }
                            else if (rbtn_pickpos.Checked)
                            {
                                list_temp.ElementAt(row * 8 + col).st_pickpos[i].x = ws.list_md[row * 8 + col].st_pos[i].x- COM.ws1.list_md[row * 8 + col].st_pos[i].x + COM.ws1.list_md[row * 8 + col].st_pickpos[i].x;
                                list_temp.ElementAt(row * 8 + col).st_pickpos[i].y = ws.list_md[row * 8 + col].st_pos[i].y - COM.ws1.list_md[row * 8 + col].st_pos[i].y + COM.ws1.list_md[row * 8 + col].st_pickpos[i].y;
                                list_temp.ElementAt(row * 8 + col).st_pickpos[i].z = COM.ws1.list_md[row * 8 + col].st_pickpos[i].z;
                            }
                            else if(rabt_jigpos.Checked)//gy0123
                            {
                                list_temp.ElementAt(row * 8 + col).st_jigpos[i].x = COM.ws1.list_md[row * 8 + col].st_jigpos[i].x;
                                list_temp.ElementAt(row * 8 + col).st_jigpos[i].y = COM.ws1.list_md[row * 8 + col].st_jigpos[i].y;
                                list_temp.ElementAt(row * 8 + col).st_jigpos[i].z = COM.ws1.list_md[row * 8 + col].st_jigpos[i].z;
                            }
                            else
                            {
                                list_temp.ElementAt(row * 8 + col).st_CapQrcodePos[i].x = COM.ws1.list_md[row * 8 + col].st_CapQrcodePos[i].x;
                                list_temp.ElementAt(row * 8 + col).st_CapQrcodePos[i].y = COM.ws1.list_md[row * 8 + col].st_CapQrcodePos[i].y;
                                list_temp.ElementAt(row * 8 + col).st_CapQrcodePos[i].z = COM.ws1.list_md[row * 8 + col].st_CapQrcodePos[i].z;
                            }
                        }              
                    }
                    else
                    {
                        MessageBox.Show(VAR.IsChinese?"工站Sorket数量异常！": "The number of sorkets in the station is abnormal!\r\n工站Sorket数量异常！", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            dgv.Rows.Clear();
            for (int r = 0; r < list_temp.Count; r++)
            {
                FillTableWithMdDat(list_temp.ElementAt(r), r);
            }
            btn_save_Click(null, null);
        }

        public void Changecolumn()
        {
            if (VAR.IsChinese)
            {
                btn_get_pos.Text = "学习";
                btn_goto_pos.Text = "定位";
            }
            else
            {
                btn_get_pos.Text = "Study";
                btn_goto_pos.Text = "Move";
            }
        }

        private void btn_learn_Click(object sender, EventArgs e)
        {
            if (!rabt_jigpos.Checked && !rbtn_WsQrcodePos.Checked) return;
            //gy0123 夹具位置学习，从拍照位置直接偏移得到夹具二维码拍照位置
            double dx = (double)nud_jig_X.Value;
            double dy = (double)nud_jig_Y.Value;
            int id = 0;
            string msg = "按【是】进行相对拍照位偏移学习，按【否】取消学习";
            DialogResult result1 = MessageBox.Show(msg, VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (result1 != DialogResult.Yes)
            {
                return;

            }
            string msgC = "按【是】进行上下料模块1学习，按【否】进行上下料模块2学习，夹具位置由拍照位置偏移计算得到";
            DialogResult result = MessageBox.Show( msgC 
                           , VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Yes) id = 0;
            else if (result == DialogResult.No) id = 1;
            else return;
            List<WS.MdDat> list_temp = new List<WS.MdDat>();
            foreach (WS.MdDat md in ws.list_md)
            {
                if(rabt_jigpos.Checked)
                {
                    md.st_jigpos[id].x = md.st_pos[id].x + dx;
                    md.st_jigpos[id].y = md.st_pos[id].y + dy;
                    md.st_jigpos[id].z = md.st_pos[id].z;
                }
                else if(rbtn_WsQrcodePos.Checked)
                {
                    md.st_CapQrcodePos[id].x = md.st_pos[id].x + dx;
                    md.st_CapQrcodePos[id].y = md.st_pos[id].y + dy;
                    md.st_CapQrcodePos[id].z = md.st_pos[id].z;
                }
                list_temp.Add(md);
            }
            dgv.Rows.Clear();
            for (int r = 0; r < list_temp.Count; r++)
            {
                FillTableWithMdDat(list_temp.ElementAt(r), r);
            }
            ws.list_md = list_temp;
             msgC = "按【是】进行位置保存，按【否】取消保存";
             result = MessageBox.Show(msgC, VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                EM_RES res = ws.SaveCfg();
                if (res != EM_RES.OK)
                {
                    MessageBox.Show(VAR.IsChinese ? "保存失败!" : "Save failed!\r\n保存失败!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                res = ws.LoadCfg();
                if (res != EM_RES.OK)
                {
                    MessageBox.Show(VAR.IsChinese ? "保存后加载失败!" : "Load failed after saving!\r\n保存后加载失败!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                UpdateShow();
                ShowOtherData();
                MessageBox.Show(VAR.IsChinese ? "保存成功!" : "Saved successfully!\r\n保存成功!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            
        }
    }
}
