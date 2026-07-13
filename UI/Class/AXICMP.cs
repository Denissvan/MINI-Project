using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevReport;
using MotionCtrl;

namespace UI
{
    public class AXICMP
    {
        public bool enable = false;
        public double start_pos = 0;
        public double end_pos = 0;
        public double step = 1;
        public List<double> list_cmp = new List<double>();
        public string axis_disc = "";
        public AXICMP(string axis_disc)
        {
            this.axis_disc = axis_disc;
            ReadTextFileToList();
        }

        public void WriteListToTextFile()
        {
            //创建 
            string filename = Path.GetFullPath("..") + "\\syscfg\\" + axis_disc + ".cmp";
            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.Flush();
            //定位 
            sw.BaseStream.Seek(0, SeekOrigin.Begin);
            //表头
            sw.WriteLine(axis_disc);
            sw.WriteLine(start_pos.ToString("F3"));
            sw.WriteLine(step.ToString("F3"));
            sw.WriteLine(list_cmp.Count.ToString());
            sw.WriteLine("----------------");
            //数据
            for (int i = 0; i < list_cmp.Count; i++) sw.WriteLine(list_cmp[i].ToString("F3"));
            //关闭
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        public EM_RES ReadTextFileToList()
        {
            enable = false;
            string filename = Path.GetFullPath("..") + "\\syscfg\\" + axis_disc + ".cmp";
            if (!File.Exists(filename)) return EM_RES.OK;
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            //定位
            sr.BaseStream.Seek(0, SeekOrigin.Begin);

            //disc
            string tmp = sr.ReadLine();
            if (tmp != null && tmp.Length > 0)
            {
                if (tmp != axis_disc)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese?string.Format("{0}补偿文件,参数 axis_disc 异常,{1}", axis_disc, tmp): string.Format("{0}ERROR:axis_disc   ({0}补偿文件,参数 axis_disc 异常,{1})", axis_disc, tmp), DReport.EmErrCode.GetParamError);
                    return EM_RES.PARA_ERR;
                }
            }

            //start posInf
            tmp = sr.ReadLine();
            if (tmp != null && tmp.Length > 0) start_pos = Convert.ToDouble(tmp);
            else
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese?string.Format("{0}补偿文件,参数 start posInf 异常", axis_disc): string.Format("{0}ERROR:start posInf    ({0}补偿文件,参数 start posInf 异常)", axis_disc), DReport.EmErrCode.GetParamError);
                return EM_RES.PARA_ERR;
            }
            //step
            tmp = sr.ReadLine();
            if (tmp != null && tmp.Length > 0) step = Convert.ToDouble(tmp);
            else
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}补偿文件,参数 step 异常", axis_disc): string.Format("{0}ERROR:step    ({0}补偿文件,参数 step 异常)", axis_disc), DReport.EmErrCode.GetParamError);
                return EM_RES.PARA_ERR;
            }
            //cnt
            int count = 0;
            tmp = sr.ReadLine();
            if (tmp != null && tmp.Length > 0) count = Convert.ToInt16(tmp);
            else
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}补偿文件,参数 count 异常", axis_disc): string.Format("{0}ERROR:count   ({0}补偿文件,参数 count 异常)", axis_disc), DReport.EmErrCode.GetParamError);
                return EM_RES.PARA_ERR;
            }

            //----
            tmp = sr.ReadLine();

            //读取
            tmp = sr.ReadLine();
            list_cmp.Clear();
            while (tmp != null && tmp.Length > 0)
            {
                list_cmp.Add(Convert.ToDouble(tmp));
                tmp = sr.ReadLine();
            }

            if (list_cmp.Count != count)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese?string.Format("{0}补偿文件,数据数量应为{1}，实际数量为{2}", axis_disc, list_cmp.Count, count): string.Format("{0} compensation file, the amount of data should be{1} but Actual quantity is {2}    ({0}补偿文件,数据数量应为{1}，实际数量为{2})", axis_disc, list_cmp.Count, count), DReport.EmErrCode.GetParamError);
                return EM_RES.PARA_ERR;
            }

            //关闭
            sr.Close();
            fs.Close();

            end_pos = start_pos + step * count;
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, VAR.IsChinese?string.Format("{0}启用补偿,start={1:F3},end={2:F3},step={3:F3},count={4}", axis_disc, start_pos, end_pos, step, list_cmp.Count): string.Format("{0} Enable cmp,start={1:F3},end={2:F3},step={3:F3},count={4}     ({0}启用补偿,start={1:F3},end={2:F3},step={3:F3},count={4})", axis_disc, start_pos, end_pos, step, list_cmp.Count));
            enable = true;

            return EM_RES.OK;
        }
        public double Cmp(double pos)
        {
            if (enable == false || list_cmp == null || list_cmp.Count == 0) return pos;
            if (pos < start_pos || pos > (start_pos + list_cmp.Count * step)) return pos;
            int idx = (int)((pos - start_pos) / step);
            if (idx > list_cmp.Count || idx < 0) return pos;
            if (idx >= list_cmp.Count - 1) return (pos + list_cmp.Last());

            return pos - ((list_cmp[idx + 1] - list_cmp[idx]) * (pos - (start_pos + idx * step)) / step + list_cmp[idx]);
        }

        public double DeCmp(double pos)
        {
            if (enable == false) return pos;
            if (pos < (start_pos - list_cmp.First()) || pos > (start_pos + list_cmp.Count * step) - list_cmp.Last()) return pos;
            int idx = (int)((pos - start_pos) / step);
            if (idx > list_cmp.Count) return pos;
            if (idx == list_cmp.Count) return (pos - list_cmp.Last());

            //posInf = x + (list_cmp[idx + 1] - list_cmp[idx]) * (x - idx * step) / step + list_cmp[idx]
            return (pos + (list_cmp[idx + 1] - list_cmp[idx]) * idx - list_cmp[idx]) * step / (step + (list_cmp[idx + 1] - list_cmp[idx]));
        }
    }
}