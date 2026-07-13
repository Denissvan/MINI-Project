using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace MotionCtrl
{
    public partial class AxisTable : UserControl
    {
        public List<AXIS> list_ax = new List<AXIS>();
        private static readonly string[] WsRotateDiscs = new string[] { "工站1旋转", "工站2旋转", "工站3旋转", "工站4旋转" };
        private static readonly string[] WsRotateEnglishDiscs = new string[] { "WS1 Rotate", "WS2 Rotate", "WS3 Rotate", "WS4 Rotate" };
        private readonly Dictionary<string, string> wsRotateSelections = new Dictionary<string, string>();

        public AxisTable()
        {
            InitializeComponent();
            list_ax.Clear();
        }
        
        private void FillTableWithAxisInf(AXIS ax, int row = -2)
        {
            if (ax == null || ax.mt_type == AXIS.MT_TYPE.NULL) return;
            //if empty or add mode then add
            //if (dgv.Rows.Count == 0|| row == -2) row = dgv.Rows.Add();
            if (dgv.Rows.Count == 0 || row < 0 || row >= dgv.Rows.Count) row = dgv.Rows.Add();
            //the last row
            else if (row < 0) row = dgv.Rows.Count - 1;

                dgv.Rows[row].Cells[0].Value = ax.disc;
                dgv.Rows[row].Cells[1].Value = ax.str_status;
                dgv.Rows[row].Cells[2].Value = ax.fcmd_pos.ToString("F3");
                dgv.Rows[row].Cells[3].Value = ax.fenc_pos.ToString("F3");
                dgv.Rows[row].Cells[4].Value = ax.isORG;
                dgv.Rows[row].Cells[5].Value = ax.isELN;
                dgv.Rows[row].Cells[6].Value = ax.isELP;
                dgv.Rows[row].Cells[7].Value = ax.isSLN;
                dgv.Rows[row].Cells[8].Value = ax.isSLP;
                dgv.Rows[row].Cells[9].Value = ax.isINP;
                dgv.Rows[row].Cells[10].Value = ax.isALM;
                dgv.Rows[row].Cells[11].Value = ax.isSVRON;
                UpdateTargetPosCell(ax, row);

        }

        private bool IsWorkstationRotateAxis(AXIS ax)
        {
            if (ax == null) return false;
            return WsRotateDiscs.Contains(ax.disc) || WsRotateEnglishDiscs.Contains(ax.english_disc);
        }

        private string[] GetWsRotatePosOptions()
        {
            return VAR.IsChinese
                ? new string[] { "位置0", "位置1", "位置2" }
                : new string[] { "Pos0", "Pos1", "Pos2" };
        }

        private void UpdateTargetPosCell(AXIS ax, int row)
        {
            if (row < 0 || row >= dgv.Rows.Count) return;

            if (IsWorkstationRotateAxis(ax))
            {
                DataGridViewComboBoxCell cell = dgv.Rows[row].Cells[12] as DataGridViewComboBoxCell;
                string[] options = GetWsRotatePosOptions();
                string currentValue = "";

                if (wsRotateSelections.ContainsKey(ax.disc))
                    currentValue = wsRotateSelections[ax.disc];
                else if (dgv.Rows[row].Cells[12].Value != null)
                    currentValue = dgv.Rows[row].Cells[12].Value.ToString();

                if (cell == null)
                {
                    cell = new DataGridViewComboBoxCell();
                    cell.FlatStyle = FlatStyle.Flat;
                    cell.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                    dgv.Rows[row].Cells[12] = cell;
                }

                cell.Items.Clear();
                cell.Items.AddRange(options);
                cell.ValueType = typeof(string);
                if (!options.Contains(currentValue)) currentValue = options[0];
                wsRotateSelections[ax.disc] = currentValue;
                cell.Value = currentValue;
            }
            else if (!(dgv.Rows[row].Cells[12] is DataGridViewTextBoxCell))
            {
                object currentValue = dgv.Rows[row].Cells[12].Value;
                dgv.Rows[row].Cells[12] = new DataGridViewTextBoxCell();
                dgv.Rows[row].Cells[12].Value = currentValue;
            }
        }

        private bool TryGetSelectedWsRotatePos(AXIS ax, int row, out byte sel)
        {
            sel = 0;
            if (!IsWorkstationRotateAxis(ax)) return false;
            if (row < 0 || row >= dgv.Rows.Count) return false;

            object value = dgv.Rows[row].Cells[12].Value;
            if (value == null) return false;

            string str = value.ToString().Trim();
            if (str.EndsWith("0")) { sel = 0; return true; }
            if (str.EndsWith("1")) { sel = 1; return true; }
            if (str.EndsWith("2")) { sel = 2; return true; }
            return false;
        }

        private void LogWsRotateUi(AXIS ax, string action, string detail)
        {
            if (!IsWorkstationRotateAxis(ax)) return;
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG,
                VAR.IsChinese
                    ? string.Format("{0} AxisTable-{1}: {2}", ax.disc, action, detail)
                    : string.Format("{0} AxisTable-{1}: {2}    ({3} AxisTable-{1}: {2})", ax.english_disc, action, detail, ax.disc));
        }

        public void AddAxis(AXIS ax)
        {
            if (true)//if (ax.isInit)
            {
                if (list_ax.Contains(ax) == false)
                {
                    list_ax.Add(ax);
                    FillTableWithAxisInf(ax);
                }
            }
            else
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, String.Format("轴状态列表,{0} 未初始化！", ax.disc));
                return;
            }
        }

        public void AddAxis(List<AXIS> list_ax)
        {
            foreach (AXIS ax in list_ax)
            {
                if(ax!=null)
                AddAxis(ax);
            }
        }

        public void ClearAxis()
        {
            list_ax.Clear();
        }

        public void AutoUpdate(int intv_ms = 300)
        {
            if (intv_ms > 0)
            {
                tmr_update.Interval = intv_ms;
                tmr_update.Enabled = true;
                tmr_update.Start();
            }
            else
            {
                tmr_update.Enabled = false;
                tmr_update.Stop();
            }
        }

        public void UpdateShow()
        {
            if (dgv.Rows.Count != list_ax.Count) dgv.Rows.Clear();
            for (int r = 0; r < list_ax.Count; r++)
            {
                FillTableWithAxisInf(list_ax.ElementAt(r), r);
                Thread.Sleep(10);
                Application.DoEvents();
            }
            dgv.Update();
        }

        private void tmr_update_Tick(object sender, EventArgs e)
        {
            UpdateShow();
        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            EM_RES ret;
            if (e.RowIndex < 0 || e.RowIndex > list_ax.Count) return;
            if (list_ax.ElementAt(e.RowIndex).disc != dgv.Rows[e.RowIndex].Cells[0].Value.ToString())
            {
                MessageBox.Show(VAR.IsChinese?"轴列表异常，请重新启动软件!": "The axis list is abnormal. Please restart the software!           \r\n(轴列表异常，请重新启动软件!)");
                return;
            }

            AXIS ax = list_ax.ElementAt(e.RowIndex);

            //负向
            if (e.ColumnIndex == 14)
            {
                ret = ax.JOG_Step(ref VAR.gsys_set.bquit, AXIS.AX_DIR.N);
                if (ret != EM_RES.OK) MessageBox.Show(VAR.IsChinese?ax.disc + "负向移动异常!":ax.disc + " Negative movement is abnormal!       \r\n("+ax.disc+ "负向移动异常!");
            }
            //正向
            else if (e.ColumnIndex == 15)
            {
                ret = ax.JOG_Step(ref VAR.gsys_set.bquit, AXIS.AX_DIR.P);
                if (ret != EM_RES.OK) MessageBox.Show(VAR.IsChinese?ax.disc + "正向移动异常!": ax.disc + " Positive movement is abnormal!      \r\n("+ax.disc+ "正向移动异常!");
            }
            //定位
            else if (e.ColumnIndex == 13)
            {
                bool bquit = false;
                if (IsWorkstationRotateAxis(ax))
                {
                    byte sel;
                    if (!TryGetSelectedWsRotatePos(ax, e.RowIndex, out sel))
                    {
                        MessageBox.Show(VAR.IsChinese ? ax.disc + "定位档位选择异常!" : ax.disc + " Position selection is abnormal!      \r\n(" + ax.disc + "定位档位选择异常!)");
                        return;
                    }
                    LogWsRotateUi(ax, "MoveClick", string.Format("selected={0}, sel={1}", dgv.Rows[e.RowIndex].Cells[12].Value, sel));
                    ret = ax.MoveToSelPos(ref VAR.gsys_set.bquit, sel);
                    LogWsRotateUi(ax, "MoveClick", string.Format("result={0}, selected={1}, sel={2}", ret, dgv.Rows[e.RowIndex].Cells[12].Value, sel));
                    if (ret != EM_RES.OK) MessageBox.Show(VAR.IsChinese ? ax.disc + "定位异常!" : ax.disc + " Positioning is abnormal!        \r\n(" + ax.disc + "定位异常!)");
                }
                else
                {
                    double pos = double.MaxValue;
                    try
                    {
                        if (dgv.Rows[e.RowIndex].Cells[12].Value == null || dgv.Rows[e.RowIndex].Cells[12].Value.ToString().Length == 0) return;
                        pos = Convert.ToDouble(dgv.Rows[e.RowIndex].Cells[12].Value);
                    }
                    catch
                    {
                        MessageBox.Show(VAR.IsChinese ? ax.disc + "获取定位坐标异常，请确保定位栏坐标输入正常!" : ax.disc + " The positioning coordinates are abnormal, please make sure that the coordinates of the positioning bar are entered normally!           \r\n(" + ax.disc + "获取定位坐标异常，请确保定位栏坐标输入正常!)");
                        return;
                    }
                    ret = ax.SetToManualHighSpd();
                    if (ret != EM_RES.OK) MessageBox.Show(VAR.IsChinese ? ax.disc + "速度设置异常!" : ax.disc + " Speed setting is abnormal!         \r\n(" + ax.disc + "速度设置异常!)");
                    ret = ax.MoveTo(ref bquit, pos, 25000, true);
                    if (ret != EM_RES.OK) MessageBox.Show(VAR.IsChinese ? ax.disc + "定位异常!" : ax.disc + " Positioning is abnormal!        \r\n(" + ax.disc + "定位异常!)");
                }
            }
            else if (e.ColumnIndex == 16)
            {
                if (DialogResult.Yes != MessageBox.Show(VAR.IsChinese?"是否执行回零操作!": "Whether to perform the zero return operation!       \r\n(是否执行回零操作!)", VAR.IsChinese?"警告":"Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {
                    return;
                }
                VAR.gsys_set.bquit = false;
                ax.HomeTask(25000);
                while (true)
                {
                    if (ax.HomeTaskisEnd) break;
                    Thread.Sleep(10);
                    Application.DoEvents();
                }
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0} end...", ax.disc));
                if (ax.HomeTaskRet != EM_RES.OK)
                {
                    ax.Stop();
                    MessageBox.Show(VAR.IsChinese?ax.disc + "回零异常!": ax.disc + "Home err!        \r\n("+ax.disc+ "回零异常!)");
                }
                else
                    MessageBox.Show(VAR.IsChinese?ax.disc + "回零成功!":ax.disc + "Home successfully!        \r\n("+ax.disc+ "回零成功!");
            }
            //使能
            else if (e.ColumnIndex == 11)
            {
                if (ax.SVRON && DialogResult.Yes == MessageBox.Show(VAR.IsChinese?"是否松开电机?\r\n松开后需要归零操作!": "Do you want to release the motor? \r\n Zeroing is required after releasing!      \r\n      (是否松开电机?松开后需要归零操作!)", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {
                    ax.SVRON = false;
                    VAR.gsys_set.status = EM_SYS_STA.UNKOWN;
                }
                else ax.SVRON = true;
            }
        }

        private void dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if ((e.RowIndex & 1) == 1) e.CellStyle.BackColor = SystemColors.ButtonFace;
        }

        private void dgv_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = false;
        }

        private void dgv_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgv.IsCurrentCellDirty && dgv.CurrentCell is DataGridViewComboBoxCell)
            {
                dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= list_ax.Count) return;
            if (e.ColumnIndex != 12) return;

            AXIS ax = list_ax.ElementAt(e.RowIndex);
            if (!IsWorkstationRotateAxis(ax)) return;

            object value = dgv.Rows[e.RowIndex].Cells[12].Value;
            if (value == null) return;
            wsRotateSelections[ax.disc] = value.ToString();
            LogWsRotateUi(ax, "SelectionChanged", string.Format("selected={0}", value));
        }


        public void ChangeColumn()
        { //axis status cmd_pos enc_pos org eln elp sln inp alm svron tg_pos btn_go btn_n btn_p btn_home
            if (VAR.IsChinese)
            {
                axis.HeaderText = "    轴";
                status.HeaderText = "状态";
                cmd_pos.HeaderText = "命令位置";
                enc_pos.HeaderText = "反馈位置";
                org.HeaderText = "ORG";
                eln.HeaderText = "EL-";
                elp.HeaderText = "EL+";
                slp.HeaderText = "SL+";
                sln.HeaderText = "SL-";
                inp.HeaderText = "INP";
                alm.HeaderText = "ALM";
                svron.HeaderText = "ON";
                tg_pos.HeaderText = "定位";
                btn_go.Text = "定位";
                btn_n.Text = "负向";
                btn_p.Text = "正向";
                btn_home.Text = "回零";
            }
            else
            {
                axis.HeaderText = "Axis";
                status.HeaderText = "Status";
                cmd_pos.HeaderText = "Cmd_Pos";
                enc_pos.HeaderText = "Enc_Pos";
                org.HeaderText = "ORG";
                eln.HeaderText = "EL-";
                elp.HeaderText = "EL+";
                slp.HeaderText = "SL+";
                sln.HeaderText = "SL-";
                inp.HeaderText = "INP";
                alm.HeaderText = "ALM";
                svron.HeaderText = "ON";
                tg_pos.HeaderText = "Tg_Pos";
                btn_go.Text = "Move";
                btn_n.Text = "-";
                btn_p.Text = "+";
                btn_home.Text = "Home";
            }

            for (int row = 0; row < list_ax.Count; row++)
            {
                UpdateTargetPosCell(list_ax.ElementAt(row), row);
            }
        }
    }
}
