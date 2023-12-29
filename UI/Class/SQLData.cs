using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using DevReport;
using MotionCtrl;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Flurl.Http;
using UI.Class;
using System.Collections;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using Sunny.UI.Win32;

namespace UI
{
    public static class SQLData
    {
        public static readonly Object LockObj = new object();

        public static readonly Object AlarmLockObj = new object();
        public static readonly Object SysTimeCntLock = new object();

        public static string TestDataSource(string file = "")
        {
            string filename = string.Format("{0}\\product\\{1}\\DataBase", Path.GetFullPath(".."),
                VAR.gsys_set.cur_product_name, file);
            if (!Directory.Exists(filename))
            {
                try
                {
                    Directory.CreateDirectory(filename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(VAR.IsChinese ? "建立文件夹出错：" : "Error creating folder:\r\n建立文件夹出错：" + ex.Message + "\r\n" + filename);
                }
            }


            if (file == "") file = DateTime.Now.ToString("yyyy_MM");
            filename = string.Format("{0}\\product\\{1}\\DataBase\\{2}.db", Path.GetFullPath(".."),
               VAR.gsys_set.cur_product_name, file);
            //if (File.Exists(filename))
            return string.Format("data source={0}", filename);

            // return "";
        }

        public static string TestTimeDataSource(string file = "")
        {
            string filename = string.Format("{0}\\product\\TestimeDataBase", Path.GetFullPath(".."),file);
           // string filename = string.Format("{0}\\product\\{1}\\TestimeDataBase", Path.GetFullPath(".."),VAR.gsys_set.cur_product_name, file);
            if (!Directory.Exists(filename))
            {
                try
                {
                    Directory.CreateDirectory(filename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(VAR.IsChinese ? "建立文件夹出错：" : "Error creating folder:\r\n建立文件夹出错：" + ex.Message + "\r\n" + filename);
                }
            }


            if (file == "") file = DateTime.Now.ToString("yyyy_MM");
            filename = string.Format("{0}\\product\\TestimeDataBase\\{1}.db", Path.GetFullPath(".."), file);
            //filename = string.Format("{0}\\product\\{1}\\TestimeDataBase\\{2}.db", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name, file);
            return string.Format("data source={0}", filename);

            // return "";
        }
        public static string AlarmTestDataSource(string file = "")
        {
            string filename = string.Format("{0}\\product\\{1}\\AlarmDataBase", Path.GetFullPath(".."),
                VAR.gsys_set.cur_product_name, file);
            if (!Directory.Exists(filename))
            {
                try
                {
                    Directory.CreateDirectory(filename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(VAR.IsChinese ? "建立文件夹出错：" : "Error creating folder:\r\n建立文件夹出错：" + ex.Message + "\r\n" + filename);
                }
            }


            if (file == "") file = DateTime.Now.ToString("yyyy_MM");
            filename = string.Format("{0}\\product\\{1}\\AlarmDataBase\\{2}.db", Path.GetFullPath(".."),
               VAR.gsys_set.cur_product_name, file);
            //if (File.Exists(filename))
            return string.Format("data source={0}", filename);

            // return "";
        }
        public static string SysTimeDataSource(string file = "")
        {
            string filename = string.Format("{0}\\product\\{1}\\SysTimeBase", Path.GetFullPath(".."),
                VAR.gsys_set.cur_product_name);
            if (!Directory.Exists(filename))
            {
                try
                {
                    Directory.CreateDirectory(filename);
                    
                    


                }
                catch (Exception ex)
                {
                    MessageBox.Show(VAR.IsChinese ? "建立文件夹出错：" : "Error creating folder:\r\n建立文件夹出错：" + ex.Message + "\r\n" + filename);
                }
            }


            if (file == "") file = DateTime.Now.ToString("yyyy_MM");
            filename = string.Format("{0}\\product\\{1}\\SysTimeBase\\{2}.db", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name, file);
            //if (File.Exists(filename))
            return string.Format("data source={0}", filename);

            // return "";
        }
        public static string TestDataTable(SQLiteHelper sh, string tablename = "", bool bnew = true, string dbName = "")
        {
            tablename = tablename.Length > 0 ? tablename : DateTime.Now.ToString("TyyyyMMdd");
            DataTable dt = new DataTable();
            try
            {
                //dt = sh.Select($"select * from {(dbName.Length > 0 ? $"[{dbName}]." : "")}sqlite_master where name = '{tablename}'");
                string str = string.Format("select * from {0}sqlite_master where name = '{1}'", dbName.Length > 0 ? "[" + dbName + "]." : "", tablename);
                dt = sh.Select(str);
            }
            catch (Exception e)
            {
                return "";
            }

            if (dt.Rows.Count == 0)
            {
                if (bnew)
                {
                    SQLiteTable tb = new SQLiteTable(tablename);
                    tb.Columns.Add(new SQLiteColumn("ID", ColType.Integer, true, true, true, "0"));
                    tb.Columns.Add(new SQLiteColumn("TIME", ColType.DateTime, false, false, false,
                        DateTime.Now.ToString("s"))); //yyyy/MM/dd HH:mm:ss
                    tb.Columns.Add(new SQLiteColumn("BARCODE", ColType.Text, false, false, false, "NOBARCODE"));
                    tb.Columns.Add(new SQLiteColumn("NUM", ColType.Integer, false, false, false, "0"));
                    tb.Columns.Add(new SQLiteColumn("WS_ID", ColType.Integer, false, false, false, "0"));
                    tb.Columns.Add(new SQLiteColumn("PC_ID", ColType.Integer, false, false, false, "0"));
                    tb.Columns.Add(new SQLiteColumn("BOX_ID", ColType.Integer, false, false, false, "0"));
                    tb.Columns.Add(new SQLiteColumn("RES", ColType.Integer, false, false, false, "0"));
                    tb.Columns.Add(new SQLiteColumn("CT", ColType.Integer, false, false, false, "0"));
                    tb.Columns.Add(new SQLiteColumn("PUD", ColType.Text, false, false, false, "正常"));
                    lock (LockObj)
                    {
                        sh.DropTable(tb.TableName);
                        sh.CreateTable(tb);
                    }
                }
                else return "";
            }

            return tablename;
        }

        public static string AlarmTestDataTable(SQLiteHelper sh, string tablename = "", bool bnew = true, string dbName = "")
        {
            tablename = tablename.Length > 0 ? tablename : DateTime.Now.ToString("TyyyyMMdd");
            DataTable dt = new DataTable();
            try
            {
                //dt = sh.Select($"select * from {(dbName.Length > 0 ? $"[{dbName}]." : "")}sqlite_master where name = '{tablename}'");
                string str = string.Format("select * from {0}sqlite_master where name = '{1}'", dbName.Length > 0 ? "[" + dbName + "]." : "", tablename);
                dt = sh.Select(str);
            }
            catch (Exception e)
            {
                return "";
            }

            if (dt.Rows.Count == 0)
            {
                if (bnew)
                {
                    SQLiteTable tb = new SQLiteTable(tablename);
                    tb.Columns.Add(new SQLiteColumn("ID", ColType.Integer, true, true, true, "0"));
                    tb.Columns.Add(new SQLiteColumn("ALARMID", ColType.Integer, false, false, false, "0"));
                    tb.Columns.Add(new SQLiteColumn("INTERVAL", ColType.Integer, false, false, false, "0"));
                    tb.Columns.Add(new SQLiteColumn("TIME", ColType.DateTime, false, false, false,
    DateTime.Now.ToString("s"))); //yyyy/MM/dd HH:mm:ss

                    lock (AlarmLockObj)
                    {
                        sh.DropTable(tb.TableName);
                        sh.CreateTable(tb);
                    }
                }
                else return "";
            }

            return tablename;
        }
        public static string TestTimeDataTable(SQLiteHelper sh, string tablename = "", bool bnew = true, string dbName = "")
        {
            tablename = tablename.Length > 0 ? tablename : DateTime.Now.ToString("TyyyyMMdd");
            DataTable dt = new DataTable();
            try
            {
                //dt = sh.Select($"select * from {(dbName.Length > 0 ? $"[{dbName}]." : "")}sqlite_master where name = '{tablename}'");
                string str = string.Format("select * from {0}sqlite_master where name = '{1}'", dbName.Length > 0 ? "[" + dbName + "]." : "", tablename);
                dt = sh.Select(str);
            }
            catch (Exception e)
            {
                return "";
            }

            if (dt.Rows.Count == 0)
            {
                if (bnew)
                {
                    SQLiteTable tb = new SQLiteTable(tablename);
                    tb.Columns.Add(new SQLiteColumn("ID", ColType.Integer, true, true, true, "0"));
                    tb.Columns.Add(new SQLiteColumn("TIME", ColType.DateTime, false, false, false, DateTime.Now.ToString("s"))); //yyyy/MM/dd HH:mm:ss
                    tb.Columns.Add(new SQLiteColumn("WSID", ColType.Integer, false, false, false, "0"));
                    tb.Columns.Add(new SQLiteColumn("LBID", ColType.Integer, false, false, false, "0"));
                    tb.Columns.Add(new SQLiteColumn("TESTTIME", ColType.Decimal, false, false, false, "0"));

                    lock (AlarmLockObj)
                    {
                        sh.DropTable(tb.TableName);
                        sh.CreateTable(tb);
                    }
                }
                else return "";
            }

            return tablename;
        }

        public static string SysTimeCntDataTable(SQLiteHelper sh, string tablename = "", bool bnew = true, string dbName = "")
        {
            tablename = tablename.Length > 0 ? tablename : DateTime.Now.ToString("TyyyyMM");
            DataTable dt = new DataTable();
            try
            {
                //dt = sh.Select($"select * from {(dbName.Length > 0 ? $"[{dbName}]." : "")}sqlite_master where name = '{tablename}'");
                string str = string.Format("select * from {0}sqlite_master where name = '{1}'", dbName.Length > 0 ? "[" + dbName + "]." : "", tablename);
                dt = sh.Select(str);
            }
            catch (Exception e)
            {
                return "";
            }

            if (dt.Rows.Count == 0)
            {
                if (bnew)
                {
                    SQLiteTable tb = new SQLiteTable(tablename);
                    tb.Columns.Add(new SQLiteColumn("InSertTime", ColType.Text, true, true, true, ""));
                    tb.Columns.Add(new SQLiteColumn("RunTime", ColType.Text, false, false, false, "0"));
                    tb.Columns.Add(new SQLiteColumn("AlmTime", ColType.Text, false, false, false, "0"));
                    tb.Columns.Add(new SQLiteColumn("TIME", ColType.DateTime, false, false, false,
    DateTime.Now.ToString("s"))); //yyyy/MM/dd HH:mm:ss

                    lock (SysTimeCntLock)
                    {
                        sh.DropTable(tb.TableName);
                        sh.CreateTable(tb);
                    }
                }
                else return "";
            }

            return tablename;
        }

        public static bool ConnectionChk(string file = "")
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(TestDataSource(file)))
                {
                    conn.Open();
                    conn.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, ex.Message, DReport.EmErrCode.ConnectFailed);
                return false;
            }
        }

        public static bool ConnectionTimeChk(string file = "")
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(TestTimeDataSource(/*file*/)))
                {
                    conn.Open();
                    conn.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, ex.Message, DReport.EmErrCode.ConnectFailed);
                return false;
            }
        }

        public static bool AlarmConnectionChk(string file = "")
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(AlarmTestDataSource(file)))
                {
                    conn.Open();
                    conn.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, ex.Message, DReport.EmErrCode.ConnectFailed);
                return false;
            }
        }
        public static bool SysTimeCntConnectionChk(string file = "")
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(SysTimeDataSource(file)))
                {
                    conn.Open();
                    conn.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, ex.Message, DReport.EmErrCode.ConnectFailed);
                return false;
            }
        }

        /// <summary>
        /// 聚合时间范围内的表格，跨度限定31天
        /// </summary>
        /// <param name="dtFrom">起始时间</param>
        /// <param name="dtEnd">结束时间</param>
        /// <param name="sh"></param>
        /// <param name="strFirstTable">第一个表格名称</param>
        /// <param name="strTable">表格聚合字符串</param>
        /// <returns>返回表格数</returns>
        public static int AttachFileAndGetTable(DateTime dtFrom, DateTime dtEnd, ref SQLiteHelper sh,
            ref string strFirstTable, ref string strTable)
        {
            if ((dtEnd - dtFrom).Days > 31)
            {
                MessageBox.Show(VAR.IsChinese ? @"时间跨度不能超一个月" : "Time span cannot exceed one month!\r\n时间跨度不能超一个月", VAR.IsChinese ? @"提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return -1;
            }

            int dtCnt = 0;
            DateTime dateTemp = dtFrom;

            //跨月份
            for (int m = dtFrom.Month; m <= dtEnd.Month; m++, dateTemp = dateTemp.AddMonths(1))
            {
                // string filename =$"{Path.GetFullPath("..")}\\product\\{VAR.gsys_set.cur_product_name}\\DataBase\\{dateTemp:yyyy_MM}.db";
                string temp = dateTemp.ToString("yyyy_MM");
                string filename = string.Format("{0}\\product\\{1}\\DataBase\\{2}.db", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name, temp);
                if (File.Exists(filename))
                {
                    sh.AttachDatabase(filename, dateTemp.ToString("yyyy_MM"));
                }
            }

            //跨天
            dateTemp = dtFrom;
            strTable = "";
            strFirstTable = "";
            for (; dateTemp <= dtEnd.AddDays(1); dateTemp = dateTemp.AddDays(1))
            {
                //当月无数据
                // string filename = $"{Path.GetFullPath("..")}\\product\\{VAR.gsys_set.cur_product_name}\\DataBase\\{dateTemp:yyyy_MM}.db";
                string temp = dateTemp.ToString("yyyy_MM");
                string filename = string.Format("{0}\\product\\{1}\\DataBase\\{2}.db", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name, temp);
                if (!File.Exists(filename)) continue;

                //exist?
                string tablename = dateTemp.ToString("TyyyyMMdd");
                tablename = TestDataTable(sh, tablename, false, dateTemp.ToString("yyyy_MM"));
                if ("" == tablename) continue;
                if (strFirstTable == "") strFirstTable = tablename;
                if (strTable != "")
                {
                    strTable += " UNION ALL ";
                }

                strTable += string.Format("select * from {0}", dateTemp.ToString("TyyyyMMdd"));

                dtCnt++;
            }

            if (strTable.Length > 0)
                strTable = string.Format("({0})", strTable);

            return dtCnt;
        }
        public static int TesttimeAttachFileAndGetTable(DateTime dtFrom, DateTime dtEnd, ref SQLiteHelper sh,
   ref string strFirstTable, ref string strTable)
        {
            if ((dtEnd - dtFrom).Days > 31)
            {
                MessageBox.Show(VAR.IsChinese ? @"时间跨度不能超一个月" : "Time span cannot exceed one month!\r\n时间跨度不能超一个月", VAR.IsChinese ? @"提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return -1;
            }

            int dtCnt = 0;
            DateTime dateTemp = dtFrom;

            //跨月份
            for (int m = dtFrom.Month; m <= dtEnd.Month; m++, dateTemp = dateTemp.AddMonths(1))
            {
                // string filename =$"{Path.GetFullPath("..")}\\product\\{VAR.gsys_set.cur_product_name}\\DataBase\\{dateTemp:yyyy_MM}.db";
                string temp = dateTemp.ToString("yyyy_MM");
                string filename = string.Format("{0}\\product\\{1}\\TestimeDataBase\\{2}.db", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name, temp);
                if (File.Exists(filename))
                {
                    sh.AttachDatabase(filename, dateTemp.ToString("yyyy_MM"));
                }
            }

            //跨天
            dateTemp = dtFrom;
            strTable = "";
            strFirstTable = "";
            for (; dateTemp <= dtEnd.AddDays(1); dateTemp = dateTemp.AddDays(1))
            {
                //当月无数据
                // string filename = $"{Path.GetFullPath("..")}\\product\\{VAR.gsys_set.cur_product_name}\\DataBase\\{dateTemp:yyyy_MM}.db";
                string temp = dateTemp.ToString("yyyy_MM");
                string filename = string.Format("{0}\\product\\{1}\\TestimeDataBase\\{2}.db", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name, temp);
                if (!File.Exists(filename)) continue;

                //exist?
                string tablename = dateTemp.ToString("TyyyyMMdd");
                tablename = AlarmTestDataTable(sh, tablename, false, dateTemp.ToString("yyyy_MM"));
                if ("" == tablename) continue;
                if (strFirstTable == "") strFirstTable = tablename;
                if (strTable != "")
                {
                    strTable += " UNION ALL ";
                }

                strTable += string.Format("select * from {0}", dateTemp.ToString("TyyyyMMdd"));

                dtCnt++;
            }

            if (strTable.Length > 0)
                strTable = string.Format("({0})", strTable);

            return dtCnt;
        }

        public static int AlarmAttachFileAndGetTable(DateTime dtFrom, DateTime dtEnd, ref SQLiteHelper sh,
            ref string strFirstTable, ref string strTable)
        {
            if ((dtEnd - dtFrom).Days > 31)
            {
                MessageBox.Show(VAR.IsChinese ? @"时间跨度不能超一个月" : "Time span cannot exceed one month!\r\n时间跨度不能超一个月", VAR.IsChinese ? @"提示" : "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return -1;
            }

            int dtCnt = 0;
            DateTime dateTemp = dtFrom;

            //跨月份
            for (int m = dtFrom.Month; m <= dtEnd.Month; m++, dateTemp = dateTemp.AddMonths(1))
            {
                // string filename =$"{Path.GetFullPath("..")}\\product\\{VAR.gsys_set.cur_product_name}\\DataBase\\{dateTemp:yyyy_MM}.db";
                string temp = dateTemp.ToString("yyyy_MM");
                string filename = string.Format("{0}\\product\\{1}\\AlarmDataBase\\{2}.db", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name, temp);
                if (File.Exists(filename))
                {
                    sh.AttachDatabase(filename, dateTemp.ToString("yyyy_MM"));
                }
            }

            //跨天
            dateTemp = dtFrom;
            strTable = "";
            strFirstTable = "";
            for (; dateTemp <= dtEnd.AddDays(1); dateTemp = dateTemp.AddDays(1))
            {
                //当月无数据
                // string filename = $"{Path.GetFullPath("..")}\\product\\{VAR.gsys_set.cur_product_name}\\DataBase\\{dateTemp:yyyy_MM}.db";
                string temp = dateTemp.ToString("yyyy_MM");
                string filename = string.Format("{0}\\product\\{1}\\AlarmDataBase\\{2}.db", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name, temp);
                if (!File.Exists(filename)) continue;

                //exist?
                string tablename = dateTemp.ToString("TyyyyMMdd");
                tablename = AlarmTestDataTable(sh, tablename, false, dateTemp.ToString("yyyy_MM"));
                if ("" == tablename) continue;
                if (strFirstTable == "") strFirstTable = tablename;
                if (strTable != "")
                {
                    strTable += " UNION ALL ";
                }

                strTable += string.Format("select * from {0}", dateTemp.ToString("TyyyyMMdd"));

                dtCnt++;
            }

            if (strTable.Length > 0)
                strTable = string.Format("({0})", strTable);

            return dtCnt;
        }

        public static DataTable TestDataSelect(SQLSelector Selector, DataGridView dgv = null)
        {
            int ct = Environment.TickCount;
            int tablcecnt = 0;
            string tablecollection = "";
            string firstTbname = "";
            DataTable dt = new DataTable();

            using (SQLiteConnection conn = new SQLiteConnection(TestDataSource()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    tablcecnt = AttachFileAndGetTable(Selector.DateTimeForm, Selector.DateTimeEnd, ref sh,
                        ref firstTbname, ref tablecollection);

                    if (tablcecnt > 0)
                    {
                        string select = "";
                        if (Selector.Condition == "")
                        {
                            //select =$"select * from {tablecollection} where time between '{Selector.DateTimeForm:s}' and '{Selector.DateTimeEnd:s}'order by time;";
                            select = string.Format("select * from {0} where time between '{1:s}' and '{2:s}'order by time;", tablecollection, Selector.DateTimeForm, Selector.DateTimeEnd);
                        }
                        else
                        {
                            //select =$"select * from {tablecollection} where time between '{Selector.DateTimeForm:s}' and '{Selector.DateTimeEnd:s}' and {Selector.Condition} order by time;";
                            select = string.Format("select * from {0} where time between '{1:s}' and '{2:s}' and {3} order by time;", tablecollection, Selector.DateTimeForm, Selector.DateTimeEnd, Selector.Condition);
                        }

                        dt = sh.Select(select);
                        //renum
                        int id = 1;
                        foreach (DataRow row in dt.Rows)
                        {
                            row["ID"] = id++;
                        }
                    }
                }
            }

            if (dgv != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (VAR.IsChinese)
                    {
                        dgv.DataSource = dt;
                        dgv.Columns["ID"].HeaderText = "序号";
                        dgv.Columns["ID"].Width = 70;
                        dgv.Columns["TIME"].HeaderText = "时间";
                        dgv.Columns["TIME"].MinimumWidth = 160;
                        dgv.Columns["TIME"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgv.Columns["BARCODE"].HeaderText = "二维码";
                        dgv.Columns["BARCODE"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgv.Columns["BARCODE"].MinimumWidth = 160;
                        dgv.Columns["WS_ID"].HeaderText = "工站";
                        dgv.Columns["WS_ID"].Width = 80;
                        dgv.Columns["PC_ID"].HeaderText = "电脑";
                        dgv.Columns["PC_ID"].Width = 80;
                        dgv.Columns["BOX_ID"].HeaderText = "工装";
                        dgv.Columns["BOX_ID"].Width = 80;
                        dgv.Columns["NUM"].HeaderText = "编号";
                        dgv.Columns["NUM"].Width = 80;
                        dgv.Columns["RES"].HeaderText = "结果";
                        dgv.Columns["RES"].Width = 80;
                        dgv.Columns["CT"].HeaderText = "用时";
                        dgv.Columns["CT"].Width = 80;
                        dgv.Columns["PUD"].HeaderText = "类型";
                        dgv.Columns["PUD"].Width = 80;
                    }
                    else
                    {
                        dgv.DataSource = dt;
                        dgv.Columns["ID"].HeaderText = "SN";
                        dgv.Columns["ID"].Width = 70;
                        dgv.Columns["TIME"].HeaderText = "TIME";
                        dgv.Columns["TIME"].MinimumWidth = 160;
                        dgv.Columns["TIME"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgv.Columns["BARCODE"].HeaderText = "Barcode";
                        dgv.Columns["BARCODE"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgv.Columns["BARCODE"].MinimumWidth = 160;
                        dgv.Columns["WS_ID"].HeaderText = "WS";
                        dgv.Columns["WS_ID"].Width = 80;
                        dgv.Columns["PC_ID"].HeaderText = "PC";
                        dgv.Columns["PC_ID"].Width = 80;
                        dgv.Columns["BOX_ID"].HeaderText = "BOX_ID";
                        dgv.Columns["BOX_ID"].Width = 80;
                        dgv.Columns["NUM"].HeaderText = "NUM";
                        dgv.Columns["NUM"].Width = 80;
                        dgv.Columns["RES"].HeaderText = "RES";
                        dgv.Columns["RES"].Width = 80;
                        dgv.Columns["CT"].HeaderText = "CT";
                        dgv.Columns["CT"].Width = 80;
                        dgv.Columns["PUD"].HeaderText = "PUD";
                        dgv.Columns["PUD"].Width = 80;
                    }
                }
                else
                {
                    dt.Clear();
                    dgv.DataSource = dt;
                }

            }

            //Selector.Lable =$"{firstTbname}...{(tablcecnt > 0 ? tablcecnt.ToString() : "")} [{Environment.TickCount - ct}ms]\r\n[{dt?.Rows.Count:000000}]";
            Selector.Lable = string.Format("{0}_{1}[{2}ms]\r\n[{3}]", firstTbname, tablcecnt > 0 ? tablcecnt.ToString() : "", Environment.TickCount - ct, dt != null ? dt.Rows.Count : 000000);
            return dt;
        }

        class alarmData
        {
            public int AlarmId;
            public double TotalTime;
            public int CNT;
        }
        static Dictionary<int, string> keyValues = new Dictionary<int, string>()
        {
            { 0,"NULL"},
            { 1,"初始化错误"},
            { 2,"状态异常"},
            { 3,"获取参数出错"},
            { 4,"设置参数出错"},
            { 5,"运动出错"},
            { 6,"更新出错"},
            { 7,"显示出错"},
            { 8,"操作出错"},
            { 9,"测试出错"},
            { 10,"硬件保护"},
            { 11,"软件保护"},
            { 12,"连接失败"},
            { 13,"定位失败"},
            { 14,"超时错误"},
            { 15,"工具错误"},
            { 16,"结果NG"},
            { 17,"中途掉料"},
            { 18,"取料失败"},
            { 19,"放料失败"},
            { 20,"进料失败"},
            { 21,"出料失败"},
            { 22,"取图失败"},
            { 23,"保存失败"},
            { 24,"切换失败"},
            { 25,"硬件报警"},
            { 26,"视觉定位失败"},
            { 27,"二维码识别失败"},
            { 28,"通信超时"},
            { 29,"通信掉线"},
            { 30,"通信不同步"},
            { 31,"安全保护"},
            { 32,"急停"},
            { 33,"文件读取失败"},
            { 34,"文件写入失败"},
            { 35,"数据库操作失败"},

            { 36,"供料仓完成"},
            { 37,"OK料仓完成"},
            { 38,"NG料仓完成"},
            { 39,"供料X轴回零"},
            { 40,"OK料X轴回零异常"},
            { 41,"NG料X轴回零异常"},
            { 42,"连续出现相同NG代码"},
            { 43,"上下料模块（1或2）报警"},
            { 44,"左光箱X轴报警"},
            { 45,"右光箱X轴报警"},
            { 46,"左光箱定位出错"},
            { 47,"右光箱定位出错"},
            { 48,"转盘定位异常"},
            { 49,"气缸下压超时"},
            { 50,"上下料Z轴超正/负软限位"},
            { 51,"光幕感应异常"},
            { 52,"飞拍失败"},
            { 53,"吸头已达阀值，但系统识别无模组"},
            { 54,"供料仓进料感应已有物料"},
            { 55,"OK料仓进料感应已有物料"},
            { 56,"NG料仓进料感应已有物料"},
            { 57,"上料吸头真空感应等待ON超时(500ms)"},
            { 58,"测试线程未退出无法创建"},
            { 59,"飞拍失败，工站位置有异物"},
            { 60,"上下料参观门禁打开"},
            { 61,"转台门后门门禁打开"},
            { 62,"左光箱门禁打开"},
            { 63,"右光箱门禁打开"},
            { 64,"工站不在测试位，转盘禁止动作"},
            { 65,"转盘异常"},
            { 66,"NG3333，请更换订单号"},
            { 67,"等待结果超时"},
            { 68,"OK料X异常停止,负限位"},
            { 69,"供料X异常停止,负限位"},
            { 70,"NG料X异常停止,负限位"},
            { 71,"物料放偏"},
            { 72,"工站重拍失败,请检查对应的工站位置是否有异物"},
            { 73,"连续出现相同NG达到设定数量"},
            { 74,"连续出现相同NG达到设定数量"}
        };
        public static DataTable AlarmTestDataSelect(SQLAlarmSelector Selector, DataGridView dgv = null)
        {
            int ct = Environment.TickCount;
            int tablcecnt = 0;
            string tablecollection = "";
            string firstTbname = "";
            DataTable dt = new DataTable();

            DataTable dataTable = new DataTable();
            List<alarmData> alarmDatas = new List<alarmData>();
            using (SQLiteConnection conn = new SQLiteConnection(AlarmTestDataSource()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    tablcecnt = AlarmAttachFileAndGetTable(Selector.DateTimeForm, Selector.DateTimeEnd, ref sh,
                        ref firstTbname, ref tablecollection);

                    if (tablcecnt > 0)
                    {
                        string select = "";
                        select = string.Format("select * from {0} where time between '{1:s}' and '{2:s}'order by time;", tablecollection, Selector.DateTimeForm, Selector.DateTimeEnd);
                        dt = sh.Select(select);
                        foreach (DataRow row in dt.Rows)
                        {
                            if (int.Parse(row["ALARMID"].ToString()) != 9 && int.Parse(row["ALARMID"].ToString()) != 18 && int.Parse(row["ALARMID"].ToString()) != 27
                                && int.Parse(row["ALARMID"].ToString()) != 52 && int.Parse(row["ALARMID"].ToString()) != 71)
                                continue;
                            bool isExist = false;
                            foreach (alarmData ad in alarmDatas)
                            {
                                if (ad.AlarmId == int.Parse(row["ALARMID"].ToString()))
                                {
                                    isExist = true;
                                    ad.TotalTime += double.Parse(row["INTERVAL"].ToString());
                                    ad.CNT++;
                                }
                            }
                            if (!isExist) alarmDatas.Add(new alarmData() { AlarmId = int.Parse(row["ALARMID"].ToString()), TotalTime = double.Parse(row["INTERVAL"].ToString()), CNT = 1 });
                        }
                        dataTable.Columns.Add("ID");
                        dataTable.Columns.Add("ALARMID");
                        dataTable.Columns.Add("TOTALTIME");
                        dataTable.Columns.Add("CNT");
                        alarmDatas = alarmDatas.OrderByDescending(x => x.TotalTime).ToList();
                        foreach (alarmData ad in alarmDatas)
                        {
                            DataRow dr = dataTable.NewRow();
                            dr[0] = (alarmDatas.IndexOf(ad) + 1).ToString();
                            dr[1] = keyValues[ad.AlarmId].ToString();
                            dr[2] = ad.TotalTime.ToString();
                            dr[3] = ad.CNT.ToString();
                            dataTable.Rows.Add(dr);
                        }


                    }
                }
            }

            if (dgv != null)
            {
                if (dataTable.Rows.Count > 0)
                {
                    if (VAR.IsChinese)
                    {
                        dgv.DataSource = dataTable;
                        dgv.Columns["ID"].HeaderText = "序号";
                        dgv.Columns["ID"].Width = 70;
                        dgv.Columns["ALARMID"].HeaderText = "异常代码";
                        dgv.Columns["ALARMID"].MinimumWidth = 160;
                        dgv.Columns["ALARMID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgv.Columns["TOTALTIME"].HeaderText = "时长";
                        dgv.Columns["TOTALTIME"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgv.Columns["TOTALTIME"].MinimumWidth = 160;
                        dgv.Columns["CNT"].HeaderText = "次数";
                        dgv.Columns["CNT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgv.Columns["CNT"].MinimumWidth = 160;
                    }
                    else
                    {
                        dgv.DataSource = dataTable;
                        dgv.Columns["ID"].HeaderText = "SN";
                        dgv.Columns["ID"].Width = 70;
                        dgv.Columns["ALARMID"].HeaderText = "ALARMID";
                        dgv.Columns["ALARMID"].MinimumWidth = 160;
                        dgv.Columns["ALARMID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgv.Columns["TOTALTIME"].HeaderText = "TOTALTIME";
                        dgv.Columns["TOTALTIME"].MinimumWidth = 160;
                        dgv.Columns["TOTALTIME"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgv.Columns["CNT"].HeaderText = "CNT";
                        dgv.Columns["CNT"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dgv.Columns["CNT"].MinimumWidth = 160;
                    }
                }
                else
                {
                    dt.Clear();
                    dataTable.Clear();
                    dgv.DataSource = dataTable;
                }

            }

            //Selector.Lable =$"{firstTbname}...{(tablcecnt > 0 ? tablcecnt.ToString() : "")} [{Environment.TickCount - ct}ms]\r\n[{dt?.Rows.Count:000000}]";
            Selector.Lable = string.Format("{0}_{1}[{2}ms]\r\n[{3}]", firstTbname, tablcecnt > 0 ? tablcecnt.ToString() : "", Environment.TickCount - ct, dt != null ? dt.Rows.Count : 000000);
            return dt;
        }

        public static DataTable TesttimeDataSelect(SQLAlarmSelector Selector)
        {
            int ct = Environment.TickCount;
            int tablcecnt = 0;
            string tablecollection = "";
            string firstTbname = "";
            DataTable dt = new DataTable();

            using (SQLiteConnection conn = new SQLiteConnection(TestTimeDataSource()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    tablcecnt = TesttimeAttachFileAndGetTable(Selector.DateTimeForm, Selector.DateTimeEnd, ref sh, ref firstTbname, ref tablecollection);
                    int leftlbnum = 0;//左光箱数量
                    double lefttime = 0;  //左光箱时间
                    int rightnum = 0;//右光箱数量
                    double righttime = 0;  //右光箱时间
                    int otpnum = 0;//右光箱数量
                    double otptime = 0;  //右光箱时间
                    if (tablcecnt > 0)
                    {
                        string select = "";
                        select = string.Format("select * from {0} where time between '{1:s}' and '{2:s}'order by time;", tablecollection, Selector.DateTimeForm, Selector.DateTimeEnd);
                        dt = sh.Select(select);
                        foreach (DataRow row in dt.Rows)
                        {
                            if(row["LBID"].ToString()=="1")    //左光箱
                            {
                                leftlbnum++;
                                lefttime = lefttime + double.Parse(row["TESTTIME"].ToString());
                            }
                            if (row["LBID"].ToString() == "2")    //右2光箱
                            {
                                rightnum++;
                                righttime = righttime + double.Parse(row["TESTTIME"].ToString());
                            }
                            if (row["LBID"].ToString() == "3")    //OTP光箱
                            {
                                otpnum++;
                                otptime = otptime + double.Parse(row["TESTTIME"].ToString());
                            }
                        }

                    }

                    PT_SET.lefttimedb = lefttime / (leftlbnum == 0 ? 1 : leftlbnum);
                    PT_SET.righttimedb = righttime / (rightnum == 0 ? 1 : rightnum);
                    PT_SET.otptimedb = otptime / (otpnum == 0 ? 1 : otpnum);

                    double[] numbers = { PT_SET.lefttimedb, PT_SET.righttimedb, PT_SET.otptimedb };
                    double max = numbers.Max();

                    PT_SET.ratedb =(PT_SET.lefttimedb + PT_SET.righttimedb + PT_SET.otptimedb) / (max==0 ? 3 : max*3);
                }
            }

            //Selector.Lable =$"{firstTbname}...{(tablcecnt > 0 ? tablcecnt.ToString() : "")} [{Environment.TickCount - ct}ms]\r\n[{dt?.Rows.Count:000000}]";
            Selector.Lable = string.Format("{0}_{1}[{2}ms]\r\n[{3}]", firstTbname, tablcecnt > 0 ? tablcecnt.ToString() : "", Environment.TickCount - ct, dt != null ? dt.Rows.Count : 000000);
            return dt;
        }
        public static List<SysTimeCnt> SysTimeCntDataSelect(SysTimeBarChart Selector)
        {
            var TimeCnt = new SysTimeCnt()
            {
                Time = DateTime.Now,
                InSertTime = string.Format($"99"),
                RunTime = 0,
                AlmTime = 0
            };
            bool bexit = SysTimeCntDataChkExitTable(TimeCnt);
            if (!bexit)
            {
                SysTimeCntDataAdd(TimeCnt);
            }
            int ct = Environment.TickCount;
            DataTable dt = new DataTable();
            DataTable dataTable = new DataTable();
            List<SysTimeCnt> SysTimeCntList = new List<SysTimeCnt>();
           var fileName= Selector.timeStart.ToString("yyyy_MM");
            using (SQLiteConnection conn = new SQLiteConnection(SysTimeDataSource(fileName)))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    string select = "";
                   // select = string.Format("select * from {0} ;", SysTimeCnttableName);
                    select = string.Format("select * from {0} where TIME between '{1}' and '{2}'order by time;", SysTimeCnttableName, Selector.timeStart.ToString("yyyy-MM-dd HH"), Selector.timeEnd.AddHours(1).ToString("yyyy-MM-dd HH"));
                   // select = string.Format("select * from {0} where TIME between '2019-1-1 00:00:00' and '2028-1-1 00:00:00'order by time;", SysTimeCnttableName);
                    dt = sh.Select(select);
                    foreach (DataRow row in dt.Rows)
                    {
                        DateTime mm ;
                         DateTime.TryParse(row["TIME"].ToString(),out mm);
                        SysTimeCntList.Add(new SysTimeCnt()
                        {
                            InSertTime = row["InSertTime"].ToString(),
                            RunTime = double.Parse(row["RunTime"].ToString()),
                            AlmTime = double.Parse(row["AlmTime"].ToString()),
                            Time = mm
                        }) ;
                    }
                    Selector.ListAllTimeTable = dt;




                }
            }

            return SysTimeCntList;
        }

       
        /// <summary>
        /// 判断是否存在某月的表，工作时间记录
        /// </summary>
        /// <param name="Selector"></param>
        /// <returns></returns>
        public static bool SysTimeCntDataChkExitTable(SysTimeCnt Selector)
        {
           
            int ct = Environment.TickCount;
            try
            {
                DataTable dt = new DataTable();
                DataTable dataTable = new DataTable();
                List<SysTimeCnt> SysTimeCntList = new List<SysTimeCnt>();
                var fileName = Selector.Time.ToString("yyyy_MM");
                using (SQLiteConnection conn = new SQLiteConnection(SysTimeDataSource(fileName)))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        string select = "";
                        // select = string.Format("select * from {0} ;", SysTimeCnttableName);
                        select = "select name from sqlite_master where type='table' order by name;";
                        // select = string.Format("select * from {0} where TIME between '2019-1-1 00:00:00' and '2028-1-1 00:00:00'order by time;", SysTimeCnttableName);
                        dt = sh.Select(select);
                        if (dt.Rows.Count > 0)
                            return true;

                    }
                }
            }catch(Exception ee)
            {
                return false;
            }
            return false;
        }
        /// <summary>
        /// 判断是否存在对应插入时间的记录，系统运行时间查询
        /// </summary>
        /// <param name="Selector"></param>
        /// <returns></returns>
        public static List<SysTimeCnt> SysTimeCntDataChkExit(SysTimeCnt Selector)
        {
            bool bexit = SysTimeCntDataChkExitTable(Selector);
            if (!bexit)         
            {
                SysTimeCntDataAdd(Selector);
            }
            int ct = Environment.TickCount;
            DataTable dt = new DataTable();
            DataTable dataTable = new DataTable();
            List<SysTimeCnt> SysTimeCntList = new List<SysTimeCnt>();
            var fileName = Selector.Time.ToString("yyyy_MM");
            using (SQLiteConnection conn = new SQLiteConnection(SysTimeDataSource(fileName)))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    string select = "";
                    select = string.Format("select * from {0} where InSertTime = '{1}';", SysTimeCnttableName, Selector.InSertTime);
                    dt = sh.Select(select);
                    foreach (DataRow row in dt.Rows)
                    {
                        DateTime mm;
                        DateTime.TryParse(row["TIME"].ToString(), out mm);
                        SysTimeCntList.Add(new SysTimeCnt()
                        {
                            InSertTime = row["InSertTime"].ToString(),
                            RunTime = double.Parse(row["RunTime"].ToString()),
                            AlmTime = double.Parse(row["AlmTime"].ToString()),
                            Time = mm
                        });
                    }

                }
            }

            return SysTimeCntList;
        }
        public static bool SysTimeCntDataDelete(SysTimeCnt Selector)
        {
            int ct = Environment.TickCount;
            bool bexit = SysTimeCntDataChkExitTable(Selector);
            if (!bexit)
            {
                SysTimeCntDataAdd(Selector);
            }
            DataTable dt = new DataTable();
            DataTable dataTable = new DataTable();
            List<SysTimeCnt> SysTimeCntList = new List<SysTimeCnt>();
            var fileName = Selector.Time.ToString("yyyy_MM");
            using (SQLiteConnection conn = new SQLiteConnection(SysTimeDataSource(fileName)))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    string select = "";
                    // select = string.Format("select * from {0} ;", SysTimeCnttableName);
                    select = string.Format("delete from {0} where InSertTime = '{1}';", SysTimeCnttableName, Selector.InSertTime);
                    // select = string.Format("select * from {0} where TIME between '2019-1-1 00:00:00' and '2028-1-1 00:00:00'order by time;", SysTimeCnttableName);
                    sh.Execute(select);

                }
            }

            return true;
        }

        public static void TestNGCount(SQLSelector Selector, Chart chart = null)
        {
            DataTable dtall = null;
            int dt_cnt = 0;
            int count = 0;
            int ng_cnt = 0;
            double per = 0;
            int all_cnt = 0;
            int allcnt = 1;
            string tablecollection = "";
            string firsttablename = "No Data";
            int ct = Environment.TickCount;

            ConnectionChk();
            using (SQLiteConnection conn = new SQLiteConnection(TestDataSource()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    //聚合相关表格
                    int tablcecnt = AttachFileAndGetTable(Selector.DateTimeForm, Selector.DateTimeEnd, ref sh,
                        ref firsttablename, ref tablecollection);

                    //查询
                    if (tablecollection.Length > 0)
                    {
                        string select = "";
                        if (Selector.Condition.Length > 0) select = string.Format("select RES, count(*) as CNT from {0} where TIME between '{1:s}' and '{2:s}' and {3} group by RES order by CNT desc", tablecollection, Selector.DateTimeForm, Selector.DateTimeEnd, Selector.Condition);
                        else select = string.Format("select RES, count(*) as CNT from {0} where TIME between '{1:s}' and '{2:s}'  group by RES order by CNT desc", tablecollection, Selector.DateTimeForm, Selector.DateTimeEnd);

                        dtall = sh.Select(select);

                        string pud = string.Empty;
                        if (Selector.Condition.Contains("PUD = '正常'")) pud = "and PUD = '正常'";
                        else if (Selector.Condition.Contains("PUD = '复测'")) pud = "and PUD = '复测'";
                        string str = string.Format(
                            "select * from {0} where time between '{1:s}' and '{2:s}'{3} order by time;",
                            tablecollection, Selector.DateTimeForm, Selector.DateTimeEnd, pud);

                        DataTable dt = sh.Select(str);

                        if (dt.Rows.Count > 0)
                        {
                            allcnt = dt.Rows.Count;
                        }
                    }
                }
            }

            if (chart != null)
            {
                chart.ChartAreas[0].AxisX.LabelStyle.Enabled = true;
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = 0;
                chart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
                chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
                chart.ChartAreas[0].AxisX.IsMarginVisible = false;
                chart.ChartAreas[0].AxisY.IsMarginVisible = false;
                chart.Series.Clear();
                chart.Series.Add("NG");
                chart.Series["NG"].IsValueShownAsLabel = true;
                //  chart.Series["NG"].IsVisibleInLegend = false;
                chart.Series["NG"].Color = Color.Orange;
                chart.Series["NG"].Points.Clear();
                chart.Series["NG"].LegendText = string.Empty;
                DataTable dt = Utility.ReadFromXml(VAR.gsys_set.GetCurProductPath + "ngcode.xml");
                int other = 0;
                int res;
                if (dtall != null)
                {
                    foreach (DataRow row in dtall.Rows)
                    {
                        res = int.Parse(row["RES"].ToString());
                        //if (res > 0&&(res==2938||res==768||res==1024))
                        if (res > 0)
                        {

                            // if (chart.Series["NG"].Points.Count > 20)
                            if (chart.Series["NG"].Points.Count > 20)
                            {
                                other += int.Parse(row["CNT"].ToString());
                            }
                            else
                            {
                                chart.Series["NG"].Points.AddXY(row["RES"].ToString(), row["CNT"]);
                                if (chart.Series["NG"].Points.Count <= 10)
                                {
                                    string a = row["RES"].ToString();
                                    DataRow[] drs = dt.Select(string.Format("代码 = '{0}'", a));
                                    if (dtall.Rows.Count > 0)
                                    {
                                        try
                                        {
                                            ng_cnt = int.Parse(dtall.Compute("sum(CNT)", "RES=" + a).ToString());
                                            per = ng_cnt;
                                            if (allcnt > 0) per = per / allcnt * 100;
                                            else per = 0;
                                        }
                                        catch (Exception e)
                                        {
                                        }
                                    }
                                    if (drs.Length > 0)
                                        chart.Series["NG"].LegendText += chart.Series["NG"].Points.Count.ToString() + ":" + row["RES"].ToString() + "->" + drs[0]["名称"] + "(" + per.ToString("f2") + "%)\r\n";
                                }

                            }
                        }
                    }
                    if (other > 0)
                    {
                        chart.Series["NG"].Points.AddXY("other", other);
                    }
                    for (int n = chart.Series["NG"].Points.Count; n < 20; n++)
                    {
                        chart.Series["NG"].Points.AddXY(" ", "");
                    }

                    if (dtall.Rows.Count > 0)
                    {
                        try
                        {
                            count = int.Parse(dtall.Compute("sum(CNT)", "RES>0").ToString());
                        }
                        catch (Exception e)
                        {
                            count = -1;
                        }
                    }
                    else count = -1;
                }
            }
            //Selector.Lable =$"{firsttablename}...{dt_cnt} [{Environment.TickCount - ct}ms]\r\n[{count:000000}]   [{count * 100.0 / allcnt:F2}%]";
            Selector.Lable = string.Format("{0}_{1}[{2}ms]\r\n[{3:000000}]   [{4:f2}%]", firsttablename, dt_cnt, Environment.TickCount - ct, count, count * 100.0 / allcnt);
        }
        public static void TestDataProductCount(SQLSelector Selector, Chart chart = null)
        {
            DateTime date_temp = DateTime.Parse(Selector.DateTimeForm.ToString("yyyy-MM-dd HH:00:00"));
            if (chart != null)
            {
                chart.ChartAreas[0].AxisX.LabelStyle.Enabled = true;
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = -60;
                chart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
                chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
                chart.Series.Clear();
                //chart.Series.Add("总数");
                //chart.Series["总数"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                //chart.Series["总数"].IsValueShownAsLabel = true;
                for (int n = 1; n <= 4; n++)
                {
                    string s = VAR.IsChinese ? string.Format("工站{0}", n) : string.Format("WS{0}", n);
                    chart.Series.Add(s);
                    chart.Series[s].Points.Clear();
                    chart.Series[s].BorderWidth = 3;
                    chart.Series[s].ChartType = SeriesChartType.StackedColumn;
                }
            }
            int count = 0;
            int tablcecnt = 0;
            string table_temp = "";
            int ct = Environment.TickCount;
            string FirstTbname = "No Data";
            while ((Selector.DateTimeEnd - date_temp) > new TimeSpan(1, 0, 0))
            {
                //提取file
                string file = date_temp.ToString("yyyy_MM");
                string filename = string.Format("{0}\\product\\{1}\\DataBase\\{2}.db", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name, file);
                if (!File.Exists(filename))
                {
                    date_temp = date_temp.AddMonths(1);
                    continue;
                };

                ConnectionChk();
                using (SQLiteConnection conn = new SQLiteConnection(TestDataSource(file)))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        string s = VAR.IsChinese ? string.Format("工站{0}", 1) : string.Format("WS{0}", 1);
                        for (int h = date_temp.Hour; h < 24; h++, date_temp = date_temp.AddHours(1))
                        {
                            //提取table
                            if ((Selector.DateTimeEnd - date_temp) < new TimeSpan(1, 0, 0)) break;
                            SQLiteHelper sh = new SQLiteHelper(cmd);
                            string tablename = date_temp.ToString("TyyyyMMdd");
                            tablename = TestDataTable(sh, tablename, false);
                            if ("" == tablename) continue;
                            if (tablename != table_temp) tablcecnt++;
                            table_temp = tablename;
                            //提取data
                            string select = "";
                            if (Selector.Condition == "") select = string.Format("select time,ws_id, count(*) from {0} where time between '{1:s}' and '{2:s}' group by WS_ID; ", tablename, date_temp, (date_temp + new TimeSpan(1, 0, 0)));
                            else select = string.Format("select time,ws_id, count(*) from {0} where time between '{1:s}' and '{2:s}' and {3} group by WS_ID; ", tablename, date_temp, (date_temp + new TimeSpan(1, 0, 0)), Selector.Condition);
                            DataTable dt = sh.Select(select);

                            int ws_id = 0;
                            int num = 0;
                            int sum = 0;
                            bool bws1 = false;
                            bool bws2 = false;
                            bool bws3 = false;
                            bool bws4 = false;
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                if (FirstTbname == "No Data") FirstTbname = tablename;
                                foreach (DataRow row in dt.Rows)
                                {
                                    ws_id = int.Parse(row["WS_ID"].ToString());
                                    switch (ws_id)
                                    {
                                        case 0:
                                            bws1 = true;
                                            break;
                                        case 1:
                                            bws2 = true;
                                            break;
                                        case 2:
                                            bws3 = true;
                                            break;
                                        case 3:
                                            bws4 = true;
                                            break;
                                    }
                                    num = int.Parse(row["count(*)"].ToString());
                                    s = VAR.IsChinese ? string.Format("工站{0}", ws_id + 1) : string.Format("WS{0}", ws_id + 1);
                                    chart.Series[s].Points.AddXY(date_temp.ToString("HH:mm"), num);
                                    sum += num;
                                }
                                //补空
                                if (VAR.IsChinese)
                                {
                                    if (!bws1) chart.Series["工站1"].Points.AddXY(date_temp.ToString("HH:mm"), 0);
                                    if (!bws2) chart.Series["工站2"].Points.AddXY(date_temp.ToString("HH:mm"), 0);
                                    if (!bws3) chart.Series["工站3"].Points.AddXY(date_temp.ToString("HH:mm"), 0);
                                    if (!bws4) chart.Series["工站4"].Points.AddXY(date_temp.ToString("HH:mm"), 0);
                                }
                                else
                                {
                                    if (!bws1) chart.Series["WS1"].Points.AddXY(date_temp.ToString("HH:mm"), 0);
                                    if (!bws2) chart.Series["WS2"].Points.AddXY(date_temp.ToString("HH:mm"), 0);
                                    if (!bws3) chart.Series["WS3"].Points.AddXY(date_temp.ToString("HH:mm"), 0);
                                    if (!bws4) chart.Series["WS4"].Points.AddXY(date_temp.ToString("HH:mm"), 0);
                                }

                                count += sum;
                                //chart.Series["总数"].Points.AddXY(date_temp.ToString("HH:mm"), sum);
                            }

                        }
                    }
                }
                Selector.Lable = string.Format("{0}_{1}[{2}ms]\r\n[{3:000000}]", FirstTbname, tablcecnt > 0 ? tablcecnt.ToString() : "", Environment.TickCount - ct, count);
            }
        }

        public static void TestDataSoketCount(SQLSelector Selector, Chart chart = null)
        {

            int ct = Environment.TickCount;
            int tablcecnt = 0;
            string tablecollection = "";
            string firstTbname = "";
            int[,] xy = new int[4, 64];

            DataTable dt = new DataTable();
            ConnectionChk();
            int cnt_ttl = 1;
            int cnt_ng = 0;
            using (SQLiteConnection conn = new SQLiteConnection(TestDataSource()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    tablcecnt = AttachFileAndGetTable(Selector.DateTimeForm, Selector.DateTimeEnd, ref sh,
                        ref firstTbname, ref tablecollection);

                    if (tablcecnt > 0)
                    {
                        string select = "";
                        if (Selector.Condition.Length > 0)
                            select = string.Format(
                                "select WS_ID,NUM, count(*) as CNT from {0} where TIME between '{1}' and '{2}' and {3} group by WS_ID,NUM",
                                tablecollection, Selector.DateTimeForm.ToString("s"),
                                Selector.DateTimeEnd.ToString("s"), Selector.Condition);
                        else
                            select = string.Format(
                                "select WS_ID,NUM, count(*) as CNT from {0} where TIME between '{1}' and '{2}' group by WS_ID,NUM",
                                tablecollection, Selector.DateTimeForm.ToString("s"),
                                Selector.DateTimeEnd.ToString("s"));

                        dt = sh.Select(string.Format(
                            "select count(*) as CNT from {0} where TIME between '{1}' and '{2}'",
                            tablecollection, Selector.DateTimeForm.ToString("s"),
                            Selector.DateTimeEnd.ToString("s")));
                        if (dt.Rows.Count > 0)
                            cnt_ttl = int.Parse(dt.Rows[0]["CNT"].ToString());

                        dt = sh.Select(select);
                        if (dt.Rows.Count > 0)
                            cnt_ng = int.Parse(dt.Compute("sum(CNT)", "true").ToString());

                        foreach (DataRow row in dt.Rows)
                        {
                            int ws_id = int.Parse(row["WS_ID"].ToString());
                            int num = int.Parse(row["NUM"].ToString()) + ws_id * 16 - 1;
                            int cnt = int.Parse(row["CNT"].ToString());
                            xy[ws_id, num] += cnt;
                        }
                    }
                }
            }

            if (chart != null)
            {
                chart.ChartAreas[0].AxisX.LabelStyle.Enabled = false;
                chart.ChartAreas[0].AxisX.LabelStyle.IsEndLabelVisible = true;
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = 90;
                chart.ChartAreas[0].AxisX.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont;
                chart.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
                chart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
                chart.ChartAreas[0].AxisX.MajorGrid.Interval = 16;
                chart.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
                chart.ChartAreas[0].AxisX.MajorGrid.IntervalOffset = 0;
                chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
                chart.ChartAreas[0].AxisX.IsMarginVisible = true;
                chart.ChartAreas[0].AxisX.IsStartedFromZero = true;
                chart.ChartAreas[0].AxisX.IntervalOffset = 0;
                chart.Series.Clear();
                for (int n = 1; n <= 4; n++)
                {
                    string s = VAR.IsChinese ? string.Format("工站{0}", n) : string.Format("WS{0}", n);
                    chart.Series.Add(s);
                    chart.Series[s].Points.Clear();
                    chart.Series[s]["PointWidth"] = "2";
                    chart.Series[s].IsValueShownAsLabel = true;
                    for (int m = 0; m < (n - 1) * 16 + 16; m++)
                    {
                        if (m < (n - 1) * 16)//|| xy[n - 1, m]==0
                        {
                            chart.Series[s].Points.AddXY(m + 0, 0);
                            chart.Series[s].Points[m].IsEmpty = true;
                            continue;
                        }
                        chart.Series[s].Points.AddXY(m + 0, xy[n - 1, m]);
                        //chart.Series[s].Points[m].AxisLabel = string.Format("{0}", m%16);
                    }
                }
            }

            Selector.Lable = string.Format("{0}_{1}[{2}ms]\r\n[{3:000000}]  {4:F2}%", firstTbname,
                tablcecnt > 0 ? tablcecnt.ToString() : "", Environment.TickCount - ct,
                new DataTable() == null ? -1 : cnt_ng, cnt_ng * 100.0 / cnt_ttl);
        }
        public static void NGRateShow(string PosInfo, ref string objdata)
        {

            var resList = objdata.Split(',');
            if (resList.Length > 20)
            {
                resList = resList.Skip(resList.Length - 20).Take(20).ToArray();
                objdata = string.Join(",", resList);
            }
            List<int> listMres = new List<int>();
            foreach (var mres in resList)
            {
                listMres.Add(int.Parse(mres));
            }
            var ngMresList = listMres.FindAll(s => s > 0).ToList();
            string ngStr = string.Join(",", ngMresList);
            if (ngMresList.Count > PT_SET.CntWsNgRateShow)
            {
                var msg = string.Format($" \r\n 当前位置{PosInfo}NG比例超过设置, \r\n，" +
                    $"近20个模组中有以下ng类型 \r\n" +
                    $"{ngStr}，\r\n，请选择是否清除记录!");
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, msg);

                MT.ST_WARN warn = new MT.ST_WARN();
                warning fr_warn = new warning();
                warn.ok_txt = MultiLanguage.TxtSelct("清除", "ClearData", "ClearData");
                warn.cancle_txt = MultiLanguage.TxtSelct("不清除", "NotClearData", "NotClearDatay");
                warn.ws = null;//增加语言
                warn.title = "NG超比例";
                warn.msg = "NG超比例";
                warn.lb_msg = msg;
                VAR.sys_inf.Set(EM_ALM_STA.WAR_RED_FLASH, VAR.IsChinese ? "NG超比例!" : "NgRateOverLimit", 20, true);
                DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.MaterialPosErr);

                if (logres == DialogResult.OK)
                {
                    objdata = "";
                }
                VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行!" : "RUN", 0, true);
            }
        }
        // static List<string> DataSaveRecordList = new List<string>();
        //public static EM_RES TestDataAdd(List<WS.MdDat> ListMd)
      
        public static EM_RES TestDataAdd(WS ws)
        {
            ConnectionChk();
            using (SQLiteConnection conn = new SQLiteConnection(TestDataSource()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    int okcnt = 0;
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    string table = TestDataTable(sh);
                    lock (LockObj)
                    {
                        var dic = new Dictionary<string, object>();
                        sh.BeginTransaction();
                        bool bNgOrder = true;
                        bool bNgNeiCun = true;
                        int NgNeiCunCnt = 0;
                        foreach (WS.MdDat md in ws.list_md)
                        {
                            if (!md.benable) continue;
                            if (md.res < 0) continue;
                            if ((md.bardcode == null || md.bardcode.Length < 1) && PT_SET.BarcodeMode != (int)PT_SET.BAR_SCAN.NO_SCAN) continue;
                            if (PT_SET.bWsNgRateShow&& md.res > 0)
                            {
                                bool bNgRateBySet = NewSysInf.UserParams.bNgRateBySet;
                                var ngcodes = NewSysInf.UserParams.NgRateCodes;
                                VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "bNgRateBySet:" + bNgRateBySet + "ngcodes:" + ngcodes  );
                                try
                                {
                                   
                                    if (bNgRateBySet)
                                    {                                      
                                        string[] ngCodeList = new string[20];
                                        if (ngcodes.Length > 0)
                                            ngCodeList = ngcodes.Split('#');
                                        foreach (var code in ngCodeList)
                                        {
                                            var mdCode = md.res.ToString();
                                            if (mdCode == code)
                                            {
                                                if (md.cntNgRateFor20.Length > 0)
                                                    md.cntNgRateFor20 += ',' + mdCode;
                                                else
                                                    md.cntNgRateFor20 += mdCode;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (md.cntNgRateFor20.Length > 0)
                                            md.cntNgRateFor20 += ',' + md.res.ToString();
                                        else
                                            md.cntNgRateFor20 += md.res.ToString();
                                    }
                                    NGRateShow(ws.disc + "-" + md.Num, ref md.cntNgRateFor20);
                                }catch(Exception ee)
                                {

                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR,  "NG比例监控发生异常" + ee.ToString());
                                }
                            }
                            if (md.res > 0)
                            {


                                if (!VAR.Isnormal) COUNT_DATA.NgTwoTestCnt++;
                                if (PT_SET.bNgControl && md.res == PT_SET.ngCode) COUNT_DATA.ngctrlngcnt++;
                                 COUNT_DATA.ngcnt[ws.num]++;
                                if (Math.Round((double)COUNT_DATA.ngctrlngcnt / COUNT_DATA.ngctrlallcnt, 4) >=
                                    PT_SET.ngScale / 100)
                                {
                                    MessageBox.Show($"NG码{PT_SET.ngCode}的不良率超过限制，NG个数为{COUNT_DATA.ngctrlngcnt},总数为{COUNT_DATA.ngctrlallcnt}");
                                    COUNT_DATA.ngctrlallcnt = 0;
                                    COUNT_DATA.ngctrlngcnt = 0;
                                }
                                if (md.res == 266 || md.res == 274 || md.res == 270) COUNT_DATA.openimagerate++;
                                //gy0123增加NG类型统计
                                if (md.res == (int)WS.Md_RES.NG_EXPOSURE) COUNT_DATA.cnt_ng_exposure++;
                                else if (md.res == (int)WS.Md_RES.NG_IIC) COUNT_DATA.cnt_ng_iic++;
                                else if (md.res == (int)WS.Md_RES.NG_IMAGE || md.res == (int)WS.Md_RES.NG_IMAGE2) COUNT_DATA.cnt_ng_iic++;
                                else if (md.res == (int)WS.Md_RES.NG_MISS_PIX) COUNT_DATA.cnt_ng_miss_pix++;
                                else if (md.res == (int)WS.Md_RES.NG_OS) COUNT_DATA.cnt_ng_OS++;
                                else if (md.res == (int)WS.Md_RES.NG_ORDER && bNgOrder)
                                {
                                    bNgOrder = false;
                                    COUNT_DATA.cnt_ng_other++;
                                    var msg = string.Format("模组异常代码3333, \r\n 模组更新失败，请更换订单号!");
                                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, msg, ErrCode: ShowErrMsg.Change3333Code);
                                    VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, msg, 20, true);
                                    DialogResult dr = FrRun.Dialog(Color.Yellow, "警告", msg, "确定", "取消");
                                    VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                }
                                else if (md.res == (int)WS.Md_RES.NG_NeiCun)
                                {
                                    NgNeiCunCnt++;
                                    COUNT_DATA.cnt_ng_other++;
                                    if (NgNeiCunCnt > 3 && bNgNeiCun)
                                    {
                                        NgNeiCunCnt = 0;
                                        bNgNeiCun = false;
                                        var msg = string.Format(ws.disc + "模组异常代码257, \r\n 请停机重启测试软件!");
                                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, msg, ErrCode: ShowErrMsg.Change3333Code);
                                        VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, msg, 20, true);
                                        DialogResult dr = FrRun.Dialog(Color.Yellow, "警告", msg, "确定", "取消");
                                        VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                    }
                                }
                                else
                                    COUNT_DATA.cnt_ng_other++;
                                //NG数据上传
                                DRpt.Report_NgProduct(md.res, (ws.num + 1) * 100 + md.Num, md.bardcode, !VAR.Isnormal);
                            }
                            else if (md.res == 0)
                            {
                                okcnt++;
                                if (!VAR.Isnormal) COUNT_DATA.OkTwoTestCnt++;
                                COUNT_DATA.okcnt[ws.num]++;


                            }

                            if (PT_SET.bSameNGTip && VAR.bSameNGTip_Temp)
                            {
                                if (md.last_res == md.res && md.res > 0) md.NgSameRes_cnt++;
                                else md.NgSameRes_cnt = 1;
                                md.last_res = md.res;
                            }
                            dic["TIME"] = DateTime.Now.ToString("s");
                            dic["BARCODE"] = md.bardcode == null ? "" : md.bardcode ;
                            //dic["MOTORBARCODE"] = md.motor_barcode == null ? "" : md.motor_barcode;
                            dic["NUM"] = md.Num;
                            dic["WS_ID"] = md.WS_ID;
                            dic["PC_ID"] = md.PC_ID;
                            dic["BOX_ID"] = md.TestBox_ID;
                            dic["RES"] = md.res;
                            dic["CT"] = md.ct;
                            dic["PUD"] = md.IsNormal == true ? "正常" : "复测";
                            try
                            {
                                sh.Insert(table, dic);
                                Task update = TestDataAddRemote(dic);//远程查看数据
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                        //上传OK数量
                        DRpt.Report_OkProduct((ws.num + 1) * 100 + okcnt, !VAR.Isnormal);
                        sh.Commit();
                        conn.Close();
                    }
                }
            }
            return EM_RES.OK;
        }

        public static EM_RES TestDataAddTime(int id ,int lbid, double time)
        {

            ConnectionTimeChk();
            using (SQLiteConnection conn = new SQLiteConnection(TestTimeDataSource()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    string table = TestTimeDataTable(sh);
                    lock (AlarmLockObj)
                    {
                        var dic = new Dictionary<string, object>();
                        sh.BeginTransaction();
                      
                        dic["TIME"] = DateTime.Now.ToString("s");
                        dic["WSID"] = id;
                        dic["LBID"] = lbid;
                        dic["TESTTIME"] = time;
                        try
                        {
                            sh.Insert(table, dic);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        //上传OK数量
                        sh.Commit();
                        conn.Close();
                    }
                }
            }
        
            return EM_RES.OK;
        }

        public static EM_RES AlarmTestDataAdd(Alarm alarmInfo)
        {
            AlarmConnectionChk();
            using (SQLiteConnection conn = new SQLiteConnection(AlarmTestDataSource()))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    string table = AlarmTestDataTable(sh);
                    lock (AlarmLockObj)
                    {
                        var dic = new Dictionary<string, object>();
                        sh.BeginTransaction();
                        TimeSpan timeSpan = alarmInfo.EndTime - alarmInfo.StartTime;
                        dic["ALARMID"] = alarmInfo.Alarmid;
                        dic["INTERVAL"] = timeSpan.TotalSeconds;
                        dic["TIME"] = DateTime.Now.ToString("s");
                        try
                        {
                            sh.Insert(table, dic);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        //上传OK数量
                        sh.Commit();
                        conn.Close();
                    }
                }
            }
            return EM_RES.OK;
        }

        static string SysTimeCnttableName = "SysTimeCnt";
        /// <summary>
        /// 插入系统运行时间数据，每小时一个。
        /// </summary>
        /// <param name="timeCnt"></param>
        /// <returns></returns>
        public static EM_RES SysTimeCntDataAdd(SysTimeCnt timeCnt)
        {
            //bool bexit = SysTimeCntDataChkExitTable(timeCnt);
            //if (bexit)
            //{
            //    return EM_RES.OK ;
            //}
            var fileName = timeCnt.Time.ToString("yyyy_MM");
            SysTimeCntConnectionChk(fileName);          
            using (SQLiteConnection conn = new SQLiteConnection(SysTimeDataSource(fileName)))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    
              
                   var  table = SysTimeCntDataTable(sh, SysTimeCnttableName);
                    lock (AlarmLockObj)
                    {
                        var dic = new Dictionary<string, object>();
                        sh.BeginTransaction();
                     
                        dic["InSertTime"] = timeCnt.InSertTime;
                        dic["RunTime"] = timeCnt.RunTime;
                        dic["AlmTime"] = timeCnt.AlmTime;
                        dic["TIME"] = timeCnt.Time;
                        try
                        {
                            sh.Insert(SysTimeCnttableName, dic);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        //上传OK数量
                        sh.Commit();
                        conn.Close();
                    }
                }
            }
            return EM_RES.OK;
        }


        public static async Task TestDataAddRemote(Dictionary<string, object> dic)
        {
            //上传url
            var uploadUrl = $"http://10.20.10.6/api/minidevice/uploaddata";
            //准备上传的数据
            var uploadData = new MiniDataDto()
            {
                //表名，不存在新建表
                DeviceCode = PT_SET.EqpPos+PT_SET.EqpSN+"版本"+ System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),

                Time = Convert.ToDateTime(dic["TIME"]),
                Barcode = dic["BARCODE"].ToString(),
                Num = (int)dic["NUM"],
                Wsid = (int)dic["WS_ID"],
                Pcid = (int)dic["PC_ID"],
                Boxid = (int)dic["BOX_ID"],
                Res = (int)dic["RES"],
                Ct = (int)dic["CT"],
                Pud = dic["PUD"].ToString()
            };
            try
            {
                //进行上传
                var doUpload =
                   await uploadUrl
                        .WithHeader("Content-Type", "application/json")
                        .WithTimeout(30)
                        .PostStringAsync(JsonConvert.SerializeObject(uploadData))
                        .ReceiveJson<HttpResult>();

                Console.WriteLine($"上传操作结果 State:{doUpload.State}, Message:{doUpload.Message}");
            }
            catch (Exception ex)
            {
                var msg = $"服务器上传异常-{ex.Message}";
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, msg);
            }
        }
        //数据实体类
        public class MiniDataDto
        {
            public string DeviceCode { get; set; }
            public DateTime Time { get; set; }
            public string Barcode { get; set; }
            public int Num { get; set; }
            public int Wsid { get; set; }
            public int Pcid { get; set; }
            public int Boxid { get; set; }
            public int Res { get; set; }
            public int Ct { get; set; }
            public string Pud { get; set; }
        }

        //查询参数类
        public class QueryMiniDataDto
        {
            public string DeviceCode { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }

        //返回的实体类
        public class HttpResult
        {
            //返回状态，1成功，0失败
            public int State { get; set; }
            //消息内容
            public string Message { get; set; }
        }


        public class QueryHttpResult : HttpResult
        {
            public List<MiniDataDto> Data { get; set; }
        }
    }
}