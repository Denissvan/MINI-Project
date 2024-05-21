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
    public partial class NGcode : UserControl
    {
        //public NGCodeDef NGDef = null;
        Product.Tray tray_show = new Product.Tray(10, 10, 0);

        public Product.Tray TrayDat
        {
            get
            {
                return tray_show;
            }
            set
            {
                tray_show = value;
                tray_ngcode.bNgCfgMode = true;
                tray_ngcode.tray_dat = tray_show;
            }
        }

        public NGcode()
        {
            InitializeComponent();
            dgv_ngcode.DataError += delegate (object sender, DataGridViewDataErrorEventArgs e) { };
            tray_ngcode.tray_dat = tray_show;
        }

        DataGridViewCheckBoxColumn NewDgvChkColumn(string name, int width, int fillWeight = 0)
        {
            DataGridViewCheckBoxColumn cb = new DataGridViewCheckBoxColumn(false);
            cb.DataPropertyName = name;
            cb.Name = name;
            cb.HeaderText = name;
            cb.Width = width;
            cb.FlatStyle = FlatStyle.Flat;
            cb.ThreeState = false;
            //cb.TrueValue = "v";
            //cb.FalseValue = "x";
            //cb.ValueType = typeof(string);
            if (fillWeight > 0)
            {
                cb.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                cb.FillWeight = fillWeight;
            }
            return cb;
        }
        DataGridViewButtonColumn NewDgvBtnColumn(string name, int width, int fillWeight = 0)
        {
            DataGridViewButtonColumn cb = new DataGridViewButtonColumn();
            cb.DataPropertyName = name;
            cb.Name = name;
            cb.HeaderText = name;
            cb.Width = width;
            cb.FlatStyle = FlatStyle.Flat;
            if (fillWeight > 0)
            {
                cb.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                cb.FillWeight = fillWeight;
            }
            return cb;
        }
        DataGridViewTextBoxColumn NewDgvTextColumn(string name, int width, int fillWeight = 0)
        {
            DataGridViewTextBoxColumn cb = new DataGridViewTextBoxColumn();
            cb.DataPropertyName = name;
            cb.Name = name;
            cb.HeaderText = name;
            cb.Width = width;
            if (fillWeight > 0)
            {
                cb.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                cb.FillWeight = fillWeight;
                cb.MinimumWidth = width;
            }
            return cb;
        }

        DataGridViewComboBoxColumn NewDgvComboxColumn(string name, List<string> list_item, int width, int fillWeight = 0)
        {
            DataGridViewComboBoxColumn cb = new DataGridViewComboBoxColumn();
            cb.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            cb.FlatStyle = FlatStyle.Popup;
            cb.DataPropertyName = name;
            cb.Name = name;
            cb.HeaderText = name;
            cb.Width = width;
            cb.DefaultCellStyle.NullValue = " ";
            DataTable dt = new DataTable();
            dt.Columns.Add("value", typeof(string));
            dt.Columns.Add("disc", typeof(string));            
            foreach (string str in list_item)
            {
                dt.Rows.Add(string.Format("分区{0}",list_item.IndexOf(str)),str);
            }
            cb.DataSource = dt;
            cb.ValueMember = "value";
            cb.DisplayMember = "disc";
            if (fillWeight > 0)
            {
                cb.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                cb.FillWeight = fillWeight;
                cb.MinimumWidth = width;
            }
            //cb.Items.Clear();
            //foreach (string str in list_item)
            //    cb.Items.Add(str);

            //cb.DataSource = list_item;
            return cb;
        }

        void UpdateNGCodeDgv(DataTable dt, DataGridView dgv,bool brenew=false)
        {
            //初始化dgv
            if (dgv.Columns.Count == 0 || brenew)
            {
                dgv_ngcode.Columns.Clear();

                dgv_ngcode.Columns.Add(NewDgvTextColumn("编号", 80));
                dgv_ngcode.Columns.Add(NewDgvTextColumn("名称", 160, 80));
                dgv_ngcode.Columns.Add(NewDgvTextColumn("代码", 80, 20));

                dgv_ngcode.Columns.Add(NewDgvComboxColumn("分区放置", tray_show.NGDef.GetZoneNameList(), 160));//new List<string> { " ", "NG类1", "NG类2", "NG类3", "分区4", "分区5", "分区6"}

                dgv_ngcode.Columns.Add(NewDgvChkColumn("复测", 80));
                dgv_ngcode.Columns.Add(NewDgvChkColumn("工装", 80));
                dgv_ngcode.Columns.Add(NewDgvChkColumn("夹具", 80));
                dgv_ngcode.Columns.Add(NewDgvChkColumn("光箱", 80));
                dgv_ngcode.Columns.Add(NewDgvChkColumn("软件", 80));
            }

            if (dt.Columns.Count == 0)
            {
                dt.Columns.Add("编号", typeof(int));
                dt.Columns.Add("代码", typeof(int));
                dt.Columns.Add("名称", typeof(string));
                dt.Columns.Add("分区放置", typeof(string));
                dt.Columns.Add("复测", typeof(bool));
                dt.Columns.Add("工装", typeof(bool));
                dt.Columns.Add("夹具", typeof(bool));
                dt.Columns.Add("光箱", typeof(bool));
                dt.Columns.Add("软件", typeof(bool));
                dt.Columns["复测"].DefaultValue = false;
                dt.Columns["工装"].DefaultValue = false;
                dt.Columns["夹具"].DefaultValue = false;
                dt.Columns["光箱"].DefaultValue = false;
                dt.Columns["工装"].DefaultValue = false;
                dt.Columns["软件"].DefaultValue = false;
            }

            bds_ngcode.DataSource = dt;
            dgv_ngcode.DataSource = bds_ngcode;

            ds_ngcode.Tables.Clear();
            ds_ngcode.Tables.Add(dt);
            bds_ngcode.DataSource = ds_ngcode.Tables[0];
            dgv_ngcode.DataSource = bds_ngcode;
            dgv_ngcode.Refresh();
        }

        private void btn_ngcode_import_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            if (DialogResult.OK != openFileDlg.ShowDialog()) return;
            DataTable dt = Utility.CsvToDataTable(openFileDlg.FileName);
            if (dt == null || dt.Rows.Count == 0) return;

            if (ds_ngcode.Tables.Count == 0) ds_ngcode.Tables.Add(new DataTable());
            UpdateNGCodeDgv(ds_ngcode.Tables[0], dgv_ngcode);
            int n = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (ds_ngcode.Tables[0].Select(string.Format("代码 = '{0}'", row[0])).Count() > 0) continue;
                if (row[1].ToString().Contains("预留")) continue;

                DataRow r = ds_ngcode.Tables[0].NewRow();
                r[0] = ds_ngcode.Tables[0].Rows.Count;
                r[1] = row[0];
                r[2] = row[1];
                ds_ngcode.Tables[0].Rows.Add(r);
                n++;
            }

            UpdateNGCodeDgv(ds_ngcode.Tables[0], dgv_ngcode);
            MessageBox.Show(VAR.IsChinese?string.Format("导入完成!\r\n本次导入 {0} 个NG代码!", n): string.Format("Import complete!\r\nThis time import {0} NG codes!\r\n导入完成!\r\n本次导入 {0} 个NG代码!", n), "", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        public bool ShowMsg = true;
        private void btn_ngcode_load_Click(object sender, EventArgs e)
        {
            bool chgdt = false;
            DataTable dt = Utility.ReadFromXml(VAR.gsys_set.GetCurProductPath + "ngcode.xml");
            tray_show.NGDef.LoadCfg(rbtn_retest.Checked);
            //tray_ngcode.tray_dat = tray_show;
            tray_ngcode.UpdateShow();
            UpdatezoneCfg();
            foreach (DataRow r in dt.Rows)
            {
                chgdt = false;
                foreach (NGCodeDef.Zone z in tray_show.NGDef.ListZone)
                {
                    if (z.ListNGcode.Contains(Convert.ToInt32(r["代码"].ToString())))
                    {
                        dt.Rows[dt.Rows.IndexOf(r)]["分区放置"] = z.Zname;
                        chgdt = true;
                        break;
                    }                  
                }
                if(!chgdt) dt.Rows[dt.Rows.IndexOf(r)]["分区放置"] ="";
            }
            UpdateNGCodeDgv(dt, dgv_ngcode, true);
            if(ShowMsg)
            MessageBox.Show(VAR.IsChinese?"加载完成！": "Loading completed!\r\n加载完成！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowMsg = true;
        }

        List<NGCodeDef.Zone> GetzoneCfg()
        {
            if (tray_show == null || tray_show.NGDef == null) return null;
            Control[] col;
            foreach(NGCodeDef.Zone zone in tray_show.NGDef.ListZone)
            {
                zone.ListNGcode = new List<int>();
                col = tbp_zone_def.Controls.Find(string.Format("tb_zonename{0}", tray_show.NGDef.ListZone.IndexOf(zone)), true);
                if (col == null|| col.Count()==0) continue;
                
                zone.Zname = ((TextBox)col[0]).Text;
                zone.Zcolor = ((TextBox)col[0]).BackColor;

                //col = tbp_zone_def.Controls.Find(string.Format("tb_zone{0}", n), true);
                //if (col == null || col.Count() == 0) continue;
                //int[] ngcode = Array.ConvertAll<string, int>(((TextBox)col[0]).Text.Split(','), s => { if (s.Length > 0) return int.Parse(s); else return -10000; });
                //zone.ListNGcode.Clear();
                //for (int i = 0; i < ngcode.Count(); i++)
                //{
                //    if (ngcode[i] > -10000) zone.ListNGcode.Add(ngcode[i]);
                //}

                DataRow[] rc = ds_ngcode.Tables[0].Select(string.Format("分区放置 = '{0}'", zone.Zname));
                zone.ListNGcode.Clear();
                foreach (DataRow row in rc)
                {
                    int ng = -1;
                    if (int.TryParse(row["代码"].ToString(), out ng))
                        zone.ListNGcode.Add(ng);
                }
            }

            return tray_show.NGDef.ListZone;
        }
        void UpdatezoneCfg()
        {
            Control[] col;
            for (int n = 0; tray_show.NGDef != null && n < tray_show.NGDef.MAX_DEF && n < tray_show.NGDef.ListZone.Count; n++)
            {
                col = this.tbp_zone_def.Controls.Find(string.Format("tb_zonename{0}", n), true);
                if (col == null || col.Count() == 0) continue;
                ((TextBox)col[0]).Text = tray_show.NGDef.ListZone.ElementAt(n).Zname;
                //((TextBox)col[0]).BackColor = zone.Zcolor;

                col = this.tbp_zone_def.Controls.Find(string.Format("tb_zone{0}", n), true);
                if (col == null || col.Count() == 0) continue;
                ((TextBox)col[0]).Text = tray_show.NGDef.ListZone.ElementAt(n).StrNgCode;
            }
        }

        private void btn_ngcode_save_Click(object sender, EventArgs e)
        {
            try
            {
                dgv_ngcode.EndEdit();
                ds_ngcode.AcceptChanges();
                if (Utility.WriteToXml(ds_ngcode.Tables[0], VAR.gsys_set.GetCurProductPath + "ngcode.xml"))
                {
                    DataTable dt = Utility.ReadFromXml(VAR.gsys_set.GetCurProductPath + "ngcode.xml");                    
                    UpdateNGCodeDgv(dt, dgv_ngcode);
                    tray_show.NGDef.ListZone = GetzoneCfg();
                    tray_show.NGDef.SaveCfg(rbtn_retest.Checked);

                    //reload
                    UpdateNGCodeDgv(dt, dgv_ngcode, true);
                    tray_show.NGDef.LoadCfg(rbtn_retest.Checked);
                    UpdatezoneCfg();
                    // MessageBox.Show("保存完成！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (DialogResult.No == MessageBox.Show(VAR.IsChinese?"保存成功!\r\n注:如需修改的参数立即生效,请按'是'键更换NG料仓,并拿掉NG料轴上的料盘!": "Saved successfully! \r\n Note: If the parameters to be modified take effect immediately, please press the 'Yes' button to replace the NG silo and remove the tray on the NG feed shaft!\r\n保存成功!\r\n注:如需修改的参数立即生效,请按'是'键更换NG料仓,并拿掉NG料轴上的料盘!", VAR.IsChinese?"提示": "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Information)) return;
                    EM_RES res = COM.traybox_ng.NewBox();
                    if (res == EM_RES.OK) COM.traybox_ng.IsReady = true;
                    COM.traybox_ng.tray_idx = 0;
                    COM.traybox_ng.tray_cur = null;
                }
                else
                {
                    MessageBox.Show(VAR.IsChinese?"保存失败！": "Save failed!\r\n保存失败！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(VAR.IsChinese?"保存失败！\r\n" + ex.Message: "Save failed!\r\n"+ ex.Message+ "\r\n保存失败!\r\n"+ ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgv_ngcode_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            //e.CellStyle.BackColor = SystemColors.ControlLightLight;
            if (e.ColumnIndex == 3)
            {
                Color cl = e.CellStyle.BackColor;
                if (tray_show.NGDef != null && tray_show.NGDef.GetColorByName(e.Value.ToString(), ref cl) == EM_RES.OK)
                    e.CellStyle.BackColor = cl;
                else if ((e.RowIndex & 1) == 1)
                    e.CellStyle.BackColor = SystemColors.ControlLight;
            }
            else if ((e.RowIndex & 1) == 1)
            {
                e.CellStyle.BackColor = SystemColors.ControlLight;
            }
        }

        private void pnl_tray_Paint(object sender, PaintEventArgs e)
        {
            ////get buf
            //BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
            //BufferedGraphics myBuffer = currentContext.Allocate(e.Graphics, e.ClipRectangle);
            //Graphics gg = myBuffer.Graphics;
            //gg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            //gg.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
            //gg.Clear(BackColor);

            ////draw at buf
            //Pen p = new Pen(Bordercolor, 2);

            //SolidBrush br = new SolidBrush(OKcolor);
            //Font ft = new Font("宋体", 12);

            //if (tray_dat != null && tray_dat.col != 0 && tray_dat.row != 0)
            //{
            //    float w = (float)e.ClipRectangle.Width / (float)tray_dat.col;
            //    float h = (float)e.ClipRectangle.Height / (float)tray_dat.row;

            //    list_rect.Clear();
            //    int okcnt = 0;
            //    int ngcnt = 0;
            //    int maskcnt = 0;
            //    for (int n = 0; n < tray_dat.list_cam.Count; n++)
            //    {
            //        //set color
            //        switch (tray_dat.list_cam[n].res)
            //        {
            //            case Product.EM_CM_RES.UNTEST:
            //                br.Color = UTcolor;
            //                break;
            //            case Product.EM_CM_RES.NONE:
            //                br.Color = UNcolor;
            //                break;
            //            case Product.EM_CM_RES.OK:
            //                br.Color = OKcolor;
            //                okcnt++;
            //                break;
            //            case Product.EM_CM_RES.ERR:
            //                br.Color = ERRcolor;
            //                break;
            //            case Product.EM_CM_RES.NG:
            //            default:
            //                ngcnt++;
            //                br.Color = NGcolor;
            //                break;
            //        }

            //        //set rect
            //        Rectangle rect = new Rectangle();
            //        rect.X = (int)(tray_dat.list_cam[n].col * w + 0.5);
            //        rect.Y = (int)(tray_dat.list_cam[n].row * h + 0.5);
            //        rect.Width = (int)(w + 0.5);
            //        rect.Height = (int)(h + 0.5);
            //        list_rect.Add(rect);
            //        gg.DrawRectangle(p, rect);
            //        gg.FillRectangle(br, rect);

            //        //mask
            //        if (tray_dat.list_mask[n])
            //        {
            //            maskcnt++;
            //            if (br.Color == Color.Red)
            //            {
            //                p.Color = Color.White;
            //            }
            //            else
            //            {
            //                p.Color = Color.Red;
            //            }
            //            gg.DrawLine(p, rect.Location, new Point(rect.X + rect.Width, rect.Y + rect.Height));
            //            gg.DrawLine(p, new Point(rect.X + rect.Width, rect.Y), new Point(rect.X, rect.Y + rect.Height));
            //            p.Color = Bordercolor;
            //        }
            //        //idx
            //        string str = tray_dat.list_cam[n].index.ToString();
            //        float ft_w = gg.MeasureString(str, ft).Width;
            //        float ft_h = gg.MeasureString(str, ft).Height;
            //        gg.DrawString(str, ft, Brushes.White, new PointF(rect.X + rect.Width / 2 - ft_w / 2, rect.Y + rect.Height / 2 - ft_h / 2));
            //    }

            //    if (ben_edit)
            //    {
            //        Rectangle rect = new Rectangle();
            //        rect.X = 0;
            //        rect.Y = 0;
            //        rect.Width = e.ClipRectangle.Width - 2;
            //        rect.Height = e.ClipRectangle.Height - 2;
            //        p.Color = Color.FromArgb(23, 169, 254);
            //        p.Width = 3;
            //        gg.DrawRectangle(p, rect);
            //    }
            //}

            ////show buf, then dispose
            //myBuffer.Render(e.Graphics);
            //gg.Dispose();
            //myBuffer.Dispose();
        }

        private void tb_zone0_Enter(object sender, EventArgs e)
        {
            if (tray_show.NGDef == null) return;
            tray_show.NGDef.CurZone = tray_show.NGDef.ListZone[0];
            for (int n = 0;n < tray_show.NGDef.MAX_DEF;n++)
            {
                if(((TextBox)sender).Name.Contains(n.ToString()))
                {
                    tray_show.NGDef.CurZone = tray_show.NGDef.ListZone[n];
                }
            }            
        }

        private void rbtn_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtn_normal.Checked) VAR.Isretestzone = false;
            else VAR.Isretestzone = true;
            ShowMsg = false;
             btn_ngcode_load_Click(null, null);
            if (DialogResult.No == MessageBox.Show(VAR.IsChinese?"切换成功!\r\n注:如需修改的参数立即生效,请按'是'键更换NG料仓,并拿掉NG料轴上的料盘!": "Switch successfully! \r\n Note: If the parameters to be modified take effect immediately, please press the 'Yes' button to replace the NG silo and remove the tray on the NG feed shaft!\r\n切换成功!\r\n注:如需修改的参数立即生效,请按'是'键更换NG料仓,并拿掉NG料轴上的料盘!", VAR.IsChinese?"提示": "Prompt", MessageBoxButtons.YesNo, MessageBoxIcon.Information)) return;
            EM_RES res = COM.traybox_ng.NewBox();
            if (res == EM_RES.OK) COM.traybox_ng.IsReady = true;
            COM.traybox_ng.tray_idx = 0;
            COM.traybox_ng.tray_cur = null;
        }
    }
}
