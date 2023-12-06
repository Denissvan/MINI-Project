using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cognex.VisionPro.Exceptions;
using System.IO.Ports;
using System.Threading;
using MotionCtrl;
namespace UI.Compment
{
    public partial class LightBoxDef : UserControl
    {
        public LightBox lightbox = null;
        public SerialPort serialPort1 = new SerialPort();
        byte[] read = { 0x02, 0x43, 0xb0, 0x01, 0x03, 0xf2 };
        byte[] origin = { 0x02, 0x43, 0xa1, 0x00, 0x03, 0xe2 };
        byte[] ini = { 0x02, 0x43, 0xa1, 0x01, 0x03, 0xe3 };
        double temp;
        double cali;
        double tempx1;
        double x1;
        double d = 0;
        public LightBoxDef()
        {
            InitializeComponent();
        }

        public void PmsEn(User.PERMISSION pms)
        {
            if (pms <= User.PERMISSION.Operator)
            {
                this.Enabled = false;
            }
            else
            {
                this.Enabled = true;
                if (pms >= User.PERMISSION.Engineer)
                {

                    for (int i = 0; i < dgv.ColumnCount; i++)
                        dgv.Columns[i].ReadOnly = true;
                    dgv.Columns["XX1"].ReadOnly = false;
                    dgv.Columns["X2"].ReadOnly = false;
                    dgv.Columns["Y1"].ReadOnly = false;
                    dgv.Columns["Z1"].ReadOnly = false;
                    dgv.Columns["Z2"].ReadOnly = false;
                    light_btn_del.Enabled = false;
                    light_btn_add.Enabled = false;
                    light_btn_load.Enabled = true;
                    light_btn_save.Enabled = true;
                    light_btn_stop.Enabled = true;
                }

                if (pms >= User.PERMISSION.Admin)
                {
                    for (int i = 0; i < dgv.ColumnCount; i++)
                        dgv.Columns[i].ReadOnly = false;
                    light_btn_del.Enabled = true;
                    light_btn_add.Enabled = true;
                }
            }
        }

        public void Add(LightBox.PosDef PosDef = null, int idx = -1)
        {
            if (lightbox == null) return;
            if (lightbox.ListPos == null) lightbox.ListPos = new List<LightBox.PosDef>();
            if (PosDef == null)
            {
                PosDef = new LightBox.PosDef();
                LightBox.PosDef maxid = lightbox.ListPos.Count > 0
                    ? lightbox.ListPos.OrderByDescending(t => t.ID).First()
                    : null;
                PosDef.ID = maxid != null ? maxid.ID + 1 : 1;
                PosDef.Name = "名称" + dgv.Rows.Count;
                PosDef.X1 = lightbox.ax_x1 != null ? lightbox.ax_x1.fenc_pos : double.MaxValue;
                PosDef.X2 = lightbox.ax_x2 != null ? lightbox.ax_x2.fenc_pos : double.MaxValue;
                PosDef.Y1 = lightbox.ax_y1 != null ? lightbox.ax_y1.fenc_pos : double.MaxValue;
                PosDef.Z1 = lightbox.ax_z1 != null ? lightbox.ax_z1.fenc_pos : double.MaxValue;
                PosDef.Z2 = lightbox.ax_z2 != null ? lightbox.ax_z2.fenc_pos : double.MaxValue;
                PosDef.Channel = int.MaxValue;
                PosDef.Delay = 0;
            }

            if (null == lightbox.ListPos.Find(s => s.ID == PosDef.ID))
            {
                if (idx == -1 && dgv.CurrentRow != null) idx = dgv.CurrentRow.Index;
                if (idx >= 0 && (idx + 1) < lightbox.ListPos.Count)
                    lightbox.ListPos.Insert(idx + 1, PosDef);
                else lightbox.ListPos.Add(PosDef);
                UpdateShow();

                //select add row
                if (idx > 0 && dgv.Rows != null && (idx + 1) < dgv.Rows.Count)
                    dgv.Rows[idx + 1].Selected = true;
            }
        }

        public void Del(int idx = -1)
        {
            if (lightbox == null) return;
            if (lightbox.ListPos == null) lightbox.ListPos = new List<LightBox.PosDef>();
            if (idx == -1 && dgv.CurrentRow != null) idx = dgv.CurrentRow.Index;

            if (idx >= 0 && idx < lightbox.ListPos.Count)
            {
                lightbox.ListPos.RemoveAt(idx);
                UpdateShow();
                if (idx > 0 && dgv.Rows != null && idx < dgv.Rows.Count)
                    dgv.Rows[idx].Selected = true;
            }
        }

        bool GetRowData(int RowID, ref LightBox.PosDef PosDef)
        {
            if (RowID < 0 || dgv.Rows == null || RowID >= dgv.Rows.Count) return false;
            try
            {
                string str = dgv.Rows[RowID].Cells["Num"].Value == null
                    ? ""
                    : dgv.Rows[RowID].Cells["Num"].Value.ToString();
                PosDef.ID = str.Length > 0 ? Convert.ToInt16(str) : -1;

                str = dgv.Rows[RowID].Cells["Disc"].Value == null ? "" : dgv.Rows[RowID].Cells["Disc"].Value.ToString();
                PosDef.Name = str;

                str = dgv.Rows[RowID].Cells["isuse"].Value == null ? "" : dgv.Rows[RowID].Cells["isuse"].Value.ToString();
                PosDef.IsUse = str.Length > 0 ? Convert.ToBoolean(str) : false;

                str = dgv.Rows[RowID].Cells["XX1"].Value == null ? "" : dgv.Rows[RowID].Cells["XX1"].Value.ToString();
                PosDef.X1 = str.Length > 0 ? Convert.ToDouble(str) : double.MaxValue;

                str = dgv.Rows[RowID].Cells["X2"].Value == null ? "" : dgv.Rows[RowID].Cells["X2"].Value.ToString();
                PosDef.X2 = str.Length > 0 ? Convert.ToDouble(str) : double.MaxValue;

                str = dgv.Rows[RowID].Cells["Y1"].Value == null ? "" : dgv.Rows[RowID].Cells["Y1"].Value.ToString();
                PosDef.Y1 = str.Length > 0 ? Convert.ToDouble(str) : double.MaxValue;

                str = dgv.Rows[RowID].Cells["Z1"].Value == null ? "" : dgv.Rows[RowID].Cells["Z1"].Value.ToString();
                PosDef.Z1 = str.Length > 0 ? Convert.ToDouble(str) : double.MaxValue;

                str = dgv.Rows[RowID].Cells["Z2"].Value == null ? "" : dgv.Rows[RowID].Cells["Z2"].Value.ToString();
                PosDef.Z2 = str.Length > 0 ? Convert.ToDouble(str) : double.MaxValue;

                str = dgv.Rows[RowID].Cells["Channel"].Value == null
                    ? ""
                    : dgv.Rows[RowID].Cells["Channel"].Value.ToString();
                PosDef.Channel = str.Length > 0 ? Convert.ToInt32(str) : int.MaxValue;

                str = dgv.Rows[RowID].Cells["Delay"].Value == null
                    ? ""
                    : dgv.Rows[RowID].Cells["Delay"].Value.ToString();
                PosDef.Delay = str.Length > 0 ? Convert.ToInt32(str) : 0;

                //补偿数据对应选择行
                if (dgv.CurrentRow != null && RowID == dgv.CurrentRow.Index)
                {
                    for (int n = 0; n < PosDef.Cmp.Length; n++)
                    {
                        PosDef.Cmp[n] = new LightBox.PosCmpDef();
                        str = dgvcmp.Rows[n].Cells["cmp_x1"].Value == null
                            ? ""
                            : dgvcmp.Rows[n].Cells["cmp_x1"].Value.ToString();
                        PosDef.Cmp[n].X1 = str.Length > 0 ? Convert.ToDouble(str) : int.MaxValue;

                        str = dgvcmp.Rows[n].Cells["cmp_x2"].Value == null
                            ? ""
                            : dgvcmp.Rows[n].Cells["cmp_x2"].Value.ToString();
                        PosDef.Cmp[n].X2 = str.Length > 0 ? Convert.ToDouble(str) : int.MaxValue;

                        str = dgvcmp.Rows[n].Cells["cmp_y1"].Value == null
                            ? ""
                            : dgvcmp.Rows[n].Cells["cmp_y1"].Value.ToString();
                        PosDef.Cmp[n].Y1 = str.Length > 0 ? Convert.ToDouble(str) : int.MaxValue;

                        str = dgvcmp.Rows[n].Cells["cmp_z1"].Value == null
                            ? ""
                            : dgvcmp.Rows[n].Cells["cmp_z1"].Value.ToString();
                        PosDef.Cmp[n].Z1 = str.Length > 0 ? Convert.ToDouble(str) : int.MaxValue;

                        str = dgvcmp.Rows[n].Cells["cmp_z2"].Value == null
                            ? ""
                            : dgvcmp.Rows[n].Cells["cmp_z2"].Value.ToString();
                        PosDef.Cmp[n].Z2 = str.Length > 0 ? Convert.ToDouble(str) : int.MaxValue;
                    }
                }
                else
                {
                    //保存之前的补偿值
                    int id = PosDef.ID;
                    LightBox.PosDef postemp = lightbox.ListPos.Find(delegate(LightBox.PosDef x) { return x.ID == id; });
                    if (postemp != null)
                    {
                        PosDef.Cmp[0] = postemp.Cmp[0];
                        PosDef.Cmp[1] = postemp.Cmp[1];
                        PosDef.Cmp[2] = postemp.Cmp[2];
                        PosDef.Cmp[3] = postemp.Cmp[3];
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        bool SetRowData(int RowID, LightBox.PosDef PosDef)
        {
            if (RowID < 0 || dgv.Rows == null || RowID >= dgv.Rows.Count) return false;
            try
            {
                dgv.Rows[RowID].Cells["Num"].Value = PosDef.ID;
                dgv.Rows[RowID].Cells["Disc"].Value = PosDef.Name;
                dgv.Rows[RowID].Cells["isuse"].Value = PosDef.IsUse;
                dgv.Rows[RowID].Cells["XX1"].Value = Math.Abs(PosDef.X1) < 10000 ? PosDef.X1.ToString("F3") : "";
                dgv.Rows[RowID].Cells["X2"].Value = Math.Abs(PosDef.X2) < 10000 ? PosDef.X2.ToString("F3") : "";
                dgv.Rows[RowID].Cells["Y1"].Value = Math.Abs(PosDef.Y1) < 10000 ? PosDef.Y1.ToString("F3") : "";
                dgv.Rows[RowID].Cells["Z1"].Value = Math.Abs(PosDef.Z1) < 10000 ? PosDef.Z1.ToString("F3") : "";
                dgv.Rows[RowID].Cells["Z2"].Value = Math.Abs(PosDef.Z2) < 10000 ? PosDef.Z2.ToString("F3") : "";
                dgv.Rows[RowID].Cells["Channel"].Value =
                    Math.Abs(PosDef.Channel) < 100000 ? PosDef.Channel.ToString() : "";
                dgv.Rows[RowID].Cells["Delay"].Value = Math.Abs(PosDef.Delay) < 100000 ? PosDef.Delay.ToString() : "0";
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void FillTableWithPosDef(LightBox.PosDef PosDef, int row = -2)
        {
            if (lightbox == null || PosDef == null || PosDef.Name.Length < 1) return;
            //if empty or add mode then add
            //if (dgv.Rows.Count == 0|| row == -2) row = dgv.Rows.Add();
            if (dgv.Rows.Count == 0 || row < 0 || row >= dgv.Rows.Count) row = dgv.Rows.Add();
            //the last row
            else if (row < 0) row = dgv.Rows.Count - 1;

            SetRowData(row, PosDef);
        }

        public void UpdateShow(bool bdgvupdate = true)
        {

            if (lightbox == null || lightbox.ListPos.Count == 0)
            {
                dgv.Rows.Clear();
                return;
            }

            if (bdgvupdate)
            {
                if (dgv.Rows.Count != lightbox.ListPos.Count)
                    dgv.Rows.Clear();

                for (int r = 0; r < lightbox.ListPos.Count; r++)
                {
                    FillTableWithPosDef(lightbox.ListPos.ElementAt(r), r);
                }
            }

            //补偿
            if (dgv.CurrentRow != null && dgv.CurrentRow.Cells[0] != null && dgv.CurrentRow.Cells[0].Value != null)
            {
                int num = (int) dgv.CurrentRow.Cells["Num"].Value;
                LightBox.PosDef PosDef = lightbox.ListPos.Find(delegate(LightBox.PosDef x) { return x.ID == num; });
                if (PosDef != null)
                {
                    dgvcmp.Rows.Clear();
                    for (int n = 0; n < PosDef.Cmp.Length; n++)
                    {
                        int RowID = dgvcmp.Rows.Add();
                        dgvcmp.Columns[0].HeaderText =VAR.IsChinese? string.Format("位置[{0}]的补偿", PosDef.ID): string.Format("Pos[{0}]Offset", PosDef.ID);
                        dgvcmp.Rows[RowID].Cells["cmp_ws"].Value = VAR.IsChinese?string.Format("工站{0}", n): string.Format("POS{0}", n);
                        dgvcmp.Rows[RowID].Cells["cmp_x1"].Value = Math.Abs(PosDef.Cmp[n].X1) < 10000
                            ? PosDef.Cmp[n].X1.ToString("F3")
                            : "";
                        dgvcmp.Rows[RowID].Cells["cmp_x2"].Value = Math.Abs(PosDef.Cmp[n].X2) < 10000
                            ? PosDef.Cmp[n].X2.ToString("F3")
                            : "";
                        dgvcmp.Rows[RowID].Cells["cmp_y1"].Value = Math.Abs(PosDef.Cmp[n].Y1) < 10000
                            ? PosDef.Cmp[n].Y1.ToString("F3")
                            : "";
                        dgvcmp.Rows[RowID].Cells["cmp_z1"].Value = Math.Abs(PosDef.Cmp[n].Z1) < 10000
                            ? PosDef.Cmp[n].Z1.ToString("F3")
                            : "";
                        dgvcmp.Rows[RowID].Cells["cmp_z2"].Value = Math.Abs(PosDef.Cmp[n].Z2) < 10000
                            ? PosDef.Cmp[n].Z2.ToString("F3")
                            : "";
                    }
                }
            }

            dgv.Update();
        }

        private void btn_del_Click(object sender, EventArgs e)
        {
            Del();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes !=
                MessageBox.Show(VAR.IsChinese?"确定是否新增?": "Are you sure you want to add it?\r\n确定是否新增?", VAR.IsChinese?"提示": "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) return;
            Add();
            // btn_save_Click(null, null);
            EM_RES res = lightbox.SaveCfg();
            if (res != EM_RES.OK)
            {
                MessageBox.Show(VAR.IsChinese ? "保存失败!": "Save failed!\r\n保存失败!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            res = lightbox.LoadCfg();
            if (res != EM_RES.OK)
            {
                MessageBox.Show(VAR.IsChinese ? "保存后加载失败!": "Load failed after saving!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btn_load_Click(object sender, EventArgs e)
        {
            if (lightbox == null) return;
            EM_RES res = lightbox.LoadCfg();
            if (res != EM_RES.OK)
            {
                MessageBox.Show(VAR.IsChinese ? "加载失败!": "Failed to load!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UpdateShow();
			ShowTrayBoxData(lightbox);
            MessageBox.Show(VAR.IsChinese ? "加载成功!": "Loaded successfully!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            if (lightbox == null) return;
            List<LightBox.PosDef> listtemp = new List<LightBox.PosDef>();
            listtemp.Clear();
            foreach (DataGridViewRow row in dgv.Rows)
            {
                LightBox.PosDef PosDef = new LightBox.PosDef();
                if (GetRowData(row.Index, ref PosDef))
                {
                    listtemp.Add(PosDef);
                }
            }

            lightbox.ListPos = listtemp;
		    GetLaserData(lightbox);
            EM_RES res = lightbox.SaveCfg();
            if (res != EM_RES.OK)
            {
                MessageBox.Show(VAR.IsChinese?"保存失败!": "Save failed!\r\n保存失败!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            res = lightbox.LoadCfg();

            ShowTrayBoxData(lightbox);

            if (res != EM_RES.OK)
            {
                MessageBox.Show(VAR.IsChinese ? "保存后加载失败!": "Load failed after saving!\r\n保存后加载失败!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UpdateShow();
            MessageBox.Show(VAR.IsChinese ? "保存成功!": "Saved successfully!\r\n保存成功!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Information);

            BaseAction.LightBoxSendMesAll();
        
        
        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (lightbox == null || e.RowIndex < 0 || e.RowIndex > lightbox.ListPos.Count) return;
            //定位
            if (e.ColumnIndex == 11)
            {
                try
                {
                    LightBox.PosDef PosDef = new LightBox.PosDef();
                    if (!GetRowData(e.RowIndex, ref PosDef)) return;

                    //确保转台在位置
                    //确保对应测试工站在测试位置
                    WS ws = null;
                    switch (lightbox.disc)
                    {
                        case "左光箱":
                            ws = Turntable.GetWSOnPos(Turntable.EM_STA.POS1);
                            break;
                        case "OTP光箱":
                            ws = Turntable.GetWSOnPos(Turntable.EM_STA.POS2);
                            break;
                        case "右光箱":
                            ws = Turntable.GetWSOnPos(Turntable.EM_STA.POS3);
                            break;
                    }

                    if (ws == null)
                    {
                        MessageBox.Show(VAR.IsChinese ? "转台位置未知，禁止光箱定位！": "The position of the turntable is unknown, positioning of the light box is prohibited!\r\n转台位置未知，禁止光箱定位！", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        return;
                    }

                    if (!ws.isInTestPos)
                    {
                        MessageBox.Show(VAR.IsChinese ? string.Format("{0} 不在测试未知，禁止光箱定位！", ws.disc): string.Format("{0} is not in the test position, and light box positioning is prohibited! \r\n{0} 不在测试位置，禁止光箱定位！", ws.disc), VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Error);
                        return;
                    }

                    //补偿
                    int ws_idx = -1;
                    if (PosDef.GetCmp(ws.num) != null && DialogResult.Yes == MessageBox.Show(VAR.IsChinese ? "当前位置有补偿，是否加上补偿定位?": "The current position is compensated. Is compensation positioning added?\r\n当前位置有补偿，是否加上补偿定位?", VAR.IsChinese ? "提示" : "Prompt",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    {
                        ws_idx = ws.num;
                    }
                    else if (DialogResult.Yes !=
                             MessageBox.Show(VAR.IsChinese ? "确定要定位?": "Are you sure you want to target?\r\n", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    {
                        return;
                    }

                    EM_RES res = lightbox.MoveTo(ref VAR.gsys_set.bquit, PosDef, ws_idx);
                    if (res == EM_RES.OK)
                        MessageBox.Show(VAR.IsChinese ? "定位成功!": "Positioning success!\r\n定位成功!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show(VAR.IsChinese ? "定位失败!": "Positioning failed!\r\n定位失败!", VAR.IsChinese ? "提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //学习位置
            else if (e.ColumnIndex == 10)
            {
                try
                {
                    LightBox.PosDef PosDef = new LightBox.PosDef();
                    if (!GetRowData(e.RowIndex, ref PosDef)) return;
                    PosDef.X1 = lightbox.ax_x1 != null ? lightbox.ax_x1.fenc_pos : double.MaxValue;
                    PosDef.X2 = lightbox.ax_x2 != null ? lightbox.ax_x2.fenc_pos : double.MaxValue;
                    PosDef.Y1 = lightbox.ax_y1 != null ? lightbox.ax_y1.fenc_pos : double.MaxValue;
                    PosDef.Z1 = lightbox.ax_z1 != null ? lightbox.ax_z1.fenc_pos : double.MaxValue;
                    PosDef.Z2 = lightbox.ax_z2 != null ? lightbox.ax_z2.fenc_pos : double.MaxValue;
                    SetRowData(e.RowIndex, PosDef);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            if (lightbox != null)
                lightbox.Stop();
        }

        private void btn_cmp_Click(object sender, EventArgs e)
        {
            tp_dgv.RowStyles[1] = new RowStyle(SizeType.Absolute, tableLayoutPanel1.Width / 2);
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            UpdateShow(false);
        }

        private void dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if ((e.RowIndex & 1) == 1) e.CellStyle.BackColor = Color.WhiteSmoke;
        }

        private void dgvcmp_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if ((e.RowIndex & 1) == 1) e.CellStyle.BackColor = Color.WhiteSmoke;
        }

        private void LightBoxDef_Load(object sender, EventArgs e)
        {
            this.dgv.ShowCellToolTips = true;
        }

        private void dgv_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 || e.RowIndex < 0 || dgv.Rows.Count <= 0) return;
            string str = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null
                ? ""
                : dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            int val = str.Length > 0 ? Convert.ToInt32(str) : -10000;
            if (val >= 0)
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "0x" + Convert.ToString(val, 16);
        }

        public void GetLaserData(LightBox lightBox)
        {
            lightBox.laser_x1= (double)nud_x.Value;
            lightBox.laser_y1 = (double)nud_y.Value;
            lightBox.laser_z2 = (double)nud_z.Value;
            lightBox.md_h = (double)nud_h.Value;
            lightBox.fsm_h = (double)nud_fsmh.Value;
            lightBox.scale= (double)nud_scale.Value;
        }

        public void ShowTrayBoxData(LightBox lightBox)
        {
            nud_x.Value = (decimal)lightBox.laser_x1;
            nud_y.Value = (decimal)lightBox.laser_y1;
            nud_z.Value = (decimal)lightBox.laser_z2;
            nud_h.Value = (decimal)lightBox.md_h;
            nud_fsmh.Value = (decimal)lightBox.fsm_h;

        }

        private void btn_getpos_Click(object sender, EventArgs e)
        {
            nud_x.Value = (decimal)MT.AXIS_BOX_L_X1.fenc_pos;
            nud_y.Value = (decimal)MT.AXIS_BOX_L_Y1.fenc_pos;
            nud_z.Value = (decimal)MT.AXIS_BOX_L_Z2.fenc_pos;
            COM.LeftLightBox.laser_x1 = MT.AXIS_BOX_L_X1.fenc_pos;
            COM.LeftLightBox.laser_y1 = MT.AXIS_BOX_L_Y1.fenc_pos;
            COM.LeftLightBox.laser_z2 = MT.AXIS_BOX_L_Z2.fenc_pos;
        }

        private void btn_gotopos_Click(object sender, EventArgs e)
        {
            EM_RES res;
            MT.SetAllAxToManualSpd();

            COM.LeftLightBox.laser_x1 = Convert.ToDouble(nud_x.Value);
            COM.LeftLightBox.laser_y1= Convert.ToDouble(nud_y.Value);
            COM.LeftLightBox.laser_z2 = Convert.ToDouble(nud_z.Value);
            res =lightbox.MoveTo(ref VAR.gsys_set.bquit, COM.LeftLightBox.laser_x1, 0, 0, COM.LeftLightBox.laser_z2,
                COM.LeftLightBox.laser_y1);
            if (res != EM_RES.OK)
            MessageBox.Show("定位失败!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                MessageBox.Show("定位成功!", "提示", MessageBoxButtons.OK);
            }

        }

        private void btn_measure_Click(object sender, EventArgs e)
        {

            COM.LeftLightBox.md_h = Convert.ToDouble(nud_h.Value);
            COM.LeftLightBox.fsm_h= Convert.ToDouble(nud_fsmh.Value);
            serialPort1.Write(read, 0, 6);
            Thread.Sleep(2000);

        }


        public void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] array = new byte[serialPort1.BytesToRead];

            int num = serialPort1.Read(array, 0, serialPort1.BytesToRead);
            double res = 0;


            if (array[2] > 0x7f)
            {
                int ret = 0;
                ret = Convert.ToInt32(array[2] << 8) + Convert.ToInt32(array[3]) - 1;
                double[] a = new double[20];
                int i = 0;
                do
                {
                    a[i] = ret % 2;
                    ret = ret / 2;
                    i++;
                } while (ret != 0);
                double ret1 = ret;
                for (int j = 1; j < i; j++)
                {
                    if (a[j] == 0) a[j] = 1;
                    else a[j] = 0;
                }
                for (int j = i - 1; j >= 0; j--)
                {
                    ret1 = ret1 + a[j] * Math.Pow(2, j);
                }
                res = -(ret1);
            }
            else
            {
                res = Convert.ToInt32(array[2] << 8) + Convert.ToInt32(array[3]);
            }

            if (Math.Abs(res) >= 2500)
            {
                richTextBox1.BeginInvoke(new Action(() =>
                {
                    richTextBox1.Text = "超范围";
                }));
                return;
            }
            

            d=Math.Round(35+res/100-(COM.LeftLightBox.md_h + COM.LeftLightBox.fsm_h + 1.5), 2, MidpointRounding.AwayFromZero);


            richTextBox1.BeginInvoke(new Action(() =>
            {
                richTextBox1.Text = string.Format("{0:F2}", d);
            }));
            


        }

        private void btn_cali_Click(object sender, EventArgs e)
        {

            EM_RES res;
            MT.SetAllAxToManualSpd();

            COM.LeftLightBox.laser_x1 = Convert.ToDouble(nud_x.Value);
            COM.LeftLightBox.laser_y1 = Convert.ToDouble(nud_y.Value);
            COM.LeftLightBox.laser_z2 = Convert.ToDouble(nud_z.Value);


            res = lightbox.MoveTo(ref VAR.gsys_set.bquit, COM.LeftLightBox.laser_x1, 0, 0, COM.LeftLightBox.laser_z2,
                COM.LeftLightBox.laser_y1);
            Thread.Sleep(3000);
            tempx1 = MT.AXIS_BOX_L_X1.fenc_pos;
          
            serialPort1.Write(read, 0, 6);
            Thread.Sleep(2000);


            richTextBox1.BeginInvoke(new Action(() =>
            {
                if (richTextBox1.Text == "超范围")

                {
                    MessageBox.Show("超范围!请确认后重新计算比例。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }));
            temp = d;


            res = lightbox.MoveTo(ref VAR.gsys_set.bquit, COM.LeftLightBox.laser_x1 + 1, 0, 0, COM.LeftLightBox.laser_z2,
                COM.LeftLightBox.laser_y1);
            Thread.Sleep(3000);
            serialPort1.Write(read, 0, 6);
            x1 = MT.AXIS_BOX_L_X1.fenc_pos;
            Thread.Sleep(2000);
            richTextBox1.BeginInvoke(new Action(() =>
            {
                if (richTextBox1.Text == "超范围")
                {
                    MessageBox.Show("超范围!请确认后重新计算比例。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }));
            cali = d;
            COM.LeftLightBox.scale = Math.Round((x1 - tempx1) / (temp - cali), 3, MidpointRounding.AwayFromZero);
            if (COM.LeftLightBox.scale>1.05||COM.LeftLightBox.scale<0.95)
            {
                MessageBox.Show("比例有误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else MessageBox.Show("比例验证通过", "提示", MessageBoxButtons.OK);

            nud_scale.BeginInvoke(new Action(() =>
            {
                nud_scale.Value = (decimal)COM.LeftLightBox.scale;
            }));




        }


        public void ChangeColumn()
        {
            //axis status cmd_pos enc_pos org eln elp sln inp alm svron tg_pos btn_go btn_n btn_p btn_home
            btn_open.Text = VAR.IsChinese ? "学习" : "Study";
            btn_close.Text = VAR.IsChinese ? "定位" : "Move";
        }

        private void dgv_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                string str = dgv.Rows[0].Cells["isuse"].Value == null ? "" : dgv.Rows[0].Cells["isuse"].Value.ToString();
                if (str == "")
                {
                    MessageBox.Show(string.Format("第一格数据为空,不能复制"), "错误", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    return;
                }
            
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (str == "True" || str == "False") row.Cells["isuse"].Value = Convert.ToBoolean(str);
                    else row.Cells["isuse"].Value = Convert.ToDouble(str);

                }
            }
                
        }
    }
}
