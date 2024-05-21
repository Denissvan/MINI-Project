using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace UI
{

    public partial class SysTimeBarChart : UserControl
    {
        
        public EventHandler HandlerSelect = null;
        public List<SysTimeCnt> ListAllTime=new List<SysTimeCnt>();
        public DataTable ListAllTimeTable = new DataTable();
        public SysTimeBarChart()
        {
            InitializeComponent();
            this.chart1.Series.Clear();
            
        }    
        public DateTime timeStart
        { 
            get{
                DateTime Time1 = this.dateTimePickerLast.Value;
                DateTime Time2 = this.dateTimePickerStart.Value;
                return Time1 >= Time2? Time2 : Time1;
            }   
        }
        public DateTime timeEnd
        {
            get
            {
                DateTime Time1 = this.dateTimePickerLast.Value;
                DateTime Time2 = this.dateTimePickerStart.Value;
                return Time1 > Time2 ? Time1 : Time2;
            }
        }
        public int Hours
        {
            get
            {
                TimeSpan ts1 = new TimeSpan(timeStart.Ticks);
                TimeSpan ts2 = new TimeSpan(timeEnd.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                return ts.Hours;
            }
        }
        public int Days
        {
            get
            {
                TimeSpan ts1 = new TimeSpan(timeStart.Ticks);
                TimeSpan ts2 = new TimeSpan(timeEnd.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                return ts.Days;
            }
        }
        private void btn_GetChart_Click(object sender, EventArgs e)
        {
            var bSuccessd = InitDataList();
            if(bSuccessd)
            {
                if (HandlerSelect != null)
                  HandlerSelect(sender, e);
                else
                    MessageBox.Show("数据未更新！仅显示初始数据", "警告", MessageBoxButtons.OK);
                ShowData();
            }
            
        }
        public void ShowData()
        {
            // 判断时间设置
            if(timeEnd>DateTime.Now)
                MessageBox.Show("查询时间大于当前时间，请重新设置时间", "警告", MessageBoxButtons.OK);
            if (Hours == 0)
                MessageBox.Show("时间设置间隔小于1小时，请重新设置时间", "警告", MessageBoxButtons.OK);
            else if (Days != 0)
                MessageBox.Show("时间间隔大于24小时，请重新设置时间", "警告", MessageBoxButtons.OK);            
            else
                Plot();
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            DataClear();
        }
        public void DataClear()
        {
            this.chart1.Series.Clear();
            this.chart1.Legends.Clear();
            this.chart1.ChartAreas.Clear();
            ListAllTime.Clear();
        }
        /// <summary>
        ///  图表绘制
        /// </summary>
        private void Plot()
        {
            this.chart1.Series.Clear();                  // 清空图表
            this.chart1.Legends.Clear();
            this.chart1.ChartAreas.Clear();
            if (ListAllTime == null || ListAllTime.Count == 0)
            { MessageBox.Show("数据为空"); return; }
            // 辅助设置
            AxiesSet();
            LegendSet();
            // 表格绘制
            if (this.rbt_runTime.Checked)
                ChartPlot(TimeType.RunTime);
            else if (this.rbt_StopTime.Checked)
                ChartPlot(TimeType.StopTime);
            else if (this.rbt_almtime.Checked)
                ChartPlot(TimeType.AlmTime);
            else if (this.rbt_runrate.Checked)
                ChartPlot(TimeType.RunRate);
            else
            {
                ChartPlot(TimeType.RunTime);
                ChartPlot(TimeType.StopTime);
                ChartPlot(TimeType.AlmTime);
                ChartPlot(TimeType.RunRate);
            }
        }
        enum TimeType
        {
            RunTime,
            AlmTime,
            StopTime,
            RunRate
        }
        /// <summary>
        ///  数据绘制
        /// </summary>
        /// <param name="seriesName">数据名字</param>
        /// <param name="data">数据</param>
        /// <param name="chartType">chart类型</param>
        /// <param name="isPrimary">是否为主轴数据</param>
        private void ChartPlot(TimeType type)
        {
            string seriesName = "";
            SeriesChartType chartType = SeriesChartType.StackedColumn;
            bool isPrimary = true;
            switch (type)
            {
                case TimeType.RunTime:
                    seriesName = "运行时间"; break;
                case TimeType.StopTime:
                    seriesName = "待机时间"; break;
                case TimeType.AlmTime:
                    seriesName = "报警时间"; break;
                case TimeType.RunRate:
                    seriesName = "稼动率";
                    chartType = SeriesChartType.Line;
                    isPrimary = false;//非主轴，右边轴
                    break;
                default: break;
            }
            Series series = this.chart1.Series.Add(seriesName);
            series.ChartType = chartType;       // 图表类型
            series.YAxisType = isPrimary ? AxisType.Primary : AxisType.Secondary;
            series.BorderWidth = 2;
            series.Label = isPrimary ? "#VAL" : "#VAL{P}";
            int i = 0;
            foreach (var m in ListAllTime)
            {
                double mvalue = 0;
                switch (type)
                {
                    case TimeType.RunTime:
                        mvalue = m.RunTime; break;
                    case TimeType.StopTime:
                        mvalue = m.StopTime; break;
                    case TimeType.AlmTime:
                        mvalue = m.AlmTime; break;
                    case TimeType.RunRate:
                        mvalue = m.RunRate; break;
                    default: break;
                }
                series.Points.AddXY(i, mvalue);
                i++;
            }
        }

        /// <summary>
        /// 坐标轴设置
        /// </summary>
        private void AxiesSet()
        {
            ChartArea chartAreas = this.chart1.ChartAreas.Add("ChartAreas");
            chartAreas.AxisX.MajorGrid.Enabled = false;             // 坐标轴
            chartAreas.AxisY.MajorGrid.Enabled = false;             // Y轴主轴
            chartAreas.AxisY.Maximum = 60;
            chartAreas.AxisY2.MajorGrid.Enabled = false;            // Y轴次轴
            chartAreas.AxisY2.Enabled = AxisEnabled.True;
            chartAreas.AxisY2.LabelStyle.Format = "0%";
            chartAreas.AxisY2.Maximum = 1;
        }

        /// <summary>
        ///  标签设置
        /// </summary>
        private void LegendSet()
        {
            Legend legend = this.chart1.Legends.Add("Legend");
            legend.Alignment = StringAlignment.Center;              // 标签居中
            legend.Docking = Docking.Top;                           //     上方
        }
        /// <summary>
        /// 初始化数据列，默认全部停止时间
        /// </summary>
        /// <param name="brandom"></param>
        /// <returns></returns>
        public bool InitDataList(bool brandom=false)
        {
            if (Days != 0)
            { MessageBox.Show("时间设置超过24小时，请重新设置"); return false; }
            if(timeStart.ToString("yyyy-MM")!=timeEnd.ToString("yyyy-MM"))
            { MessageBox.Show("不能夸月查询，请重新设置"); return false; }
            var startTime = timeStart;
            ListAllTime.Clear();
            Random rd = new Random();
            for (int i = 0; i <= Hours; i++)
            {

                var num = rd.Next(0, 59);
                SysTimeCnt mCnt = new SysTimeCnt()
                {
                    InSertTime = string.Format($"{startTime.Day}99{startTime.Hour}"),
                    RunTime = brandom ? num : 0,
                    Time = startTime,
                    AlmTime = brandom ? rd.Next(0, 60 - num):0
                };
                ListAllTime.Add(mCnt);
                startTime= startTime.AddHours(1);
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var bSuccessd = InitDataList();
            ListAllTimeTable = new DataTable();
            if (bSuccessd)
            {
                if (HandlerSelect != null)
                {
                    HandlerSelect(sender, e);
                    ShowData();
                    if (ListAllTimeTable != null)
                    {
                        DatatableToCSV(ListAllTimeTable);
                    }
                }
                else
                    MessageBox.Show("数据未更新！仅显示初始数据", "警告", MessageBoxButtons.OK);
               
            }
        }



        public static bool DatatableToCSV(DataTable dt)
        {
            bool createFLAG = false;

            try
            {
                StringBuilder sb = new StringBuilder();
                string line = "";

                if (dt != null && dt.Rows.Count > 0)
                {
                    //table head
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        line += string.Format("\"{0}\",", dt.Columns[i].ColumnName);
                    }

                    line = line.TrimEnd(',');
                    sb.AppendLine(line);
                    //every row
                    foreach (DataRow row in dt.Rows)
                    {
                        line = "";
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            line += string.Format("\"{0}\",", row[j].ToString().Replace("\"", "\"\""));
                        }

                        line = line.TrimEnd(',');
                        sb.AppendLine(line);
                    }
                  
                        SaveFileDialog mopen = new SaveFileDialog();
                    mopen.Title = "保存csv文件";
                    mopen.Filter = "csv文件(*.csv)|*.csv";
                    if (mopen.ShowDialog() == DialogResult.OK)
                    {
                        //write file
                        //日志文件夹路径
                        var path = mopen.FileName;



                        //日志TXT文件
                        string csvName = path;

                        File.WriteAllText(csvName, sb.ToString(), Encoding.UTF8);
                    }
                }else
                {
                    MessageBox.Show("数据为空,保存失败");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return createFLAG;
        }


    }


    public class SysTimeCnt
    {
        public string InSertTime ;
        public void UpdateInSertTime()
        {
          var startTime=  DateTime.Now;
            InSertTime = string.Format($"{startTime.Day}99{startTime.Hour}");
            Time = startTime;
        }
        public double RunTime;
        public double AlmTime;
        public DateTime Time;
        public double StopTime
        {
            get { return 60 - RunTime - AlmTime; }

        }
        public double RunRate
        {
            get { return Math.Round(Convert.ToDouble(this.RunTime / 60), 2); }
        }


    }
}
