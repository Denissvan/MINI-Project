using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using MotionCtrl;

namespace UI
{
    public class NGCodeDef
    {
        public int MAX_DEF = 6;
        public Zone CurZone = null;
        public class Zone
        {
            public string Zname;
            public Color Zcolor;
            public List<int> ListTrayPosIdx;
            public List<int> ListNGcode;
            public string StrNgCode
            {
                get
                {
                    string str = "";
                    if(ListNGcode!=null)
                        foreach (int i in ListNGcode) str += string.Format("{0},",i);
                    return str;
                }
            }
            public string StrPosIdx
            {
                get
                {
                    string str = "";
                    if (ListTrayPosIdx != null)
                        foreach (int i in ListTrayPosIdx) str += string.Format("{0},", i);
                    return str;
                }
            }
        }
        public void InvertTrayIdx(int idx)
        {
            if (CurZone == null) return;
            if (CurZone.ListTrayPosIdx == null) return;

            if (CurZone.ListTrayPosIdx.Contains(idx))
            {
                CurZone.ListTrayPosIdx.Remove(idx);
            }
            else
            {
                //remove first
                foreach(Zone z in ListZone)
                {
                    z.ListTrayPosIdx.Remove(idx);
                }
                //add
                CurZone.ListTrayPosIdx.Add(idx);
            }
        }
        public List<Zone> ListZone = new List<Zone>();

        /// <summary>
        /// 参数保存
        /// </summary>
        /// <param name="filename">配置文件路径，为空时默认为..\\syscfg\\syscfg.ini</param>
        /// <returns></returns>
        public EM_RES SaveCfg(bool bReTest= false, string filename = "")
        {
            if (filename.Length < 3)
                filename = VAR.gsys_set.GetCurProductPath + "ZoneDef.ini";
            bool ischange = false;
            IniFile inf = new IniFile(filename);
            string str_section = "";
            string str_temp = "";
            //intArray = Array.ConvertAll<string, int>(strArray, s => int.Parse(s));
            foreach (Zone z in ListZone)
            {
                str_section =string.Format("ZONE{0}", ListZone.IndexOf(z),false);
                inf.WriteString(str_section, "Name", z.Zname,ref ischange,false);
                inf.WriteInteger(str_section, "Color", z.Zcolor.ToArgb(), ref ischange, false);

                str_temp = "";
                foreach (int n in z.ListNGcode) str_temp += string.Format("{0},",n);
                inf.WriteString(str_section, !bReTest ? "ListNGCode" : "ListNGCode_RT", str_temp, ref ischange, false);

                str_temp = "";
                foreach (int n in z.ListTrayPosIdx) str_temp += string.Format("{0},", n);
                inf.WriteString(str_section, !bReTest ? "ListTrayPosIdx" : "ListTrayPosIdx_RT", str_temp, ref ischange, false);
            }

            return EM_RES.OK;
        }
        /// <summary>
        /// 参数加载
        /// </summary>
        /// <param name="filename">配置文件路径，为空时默认为..\\syscfg\\syscfg.ini</param>
        /// <returns></returns>
        public EM_RES LoadCfg(bool bReTest = false,string filename = "")
        {
            if (filename.Length < 3)
                filename = VAR.gsys_set.GetCurProductPath + "ZoneDef.ini";            

            if (!Directory.Exists(Path.GetDirectoryName(filename))) return EM_RES.PARA_ERR;

            IniFile inf = new IniFile(filename);
            string str_section = "";
            string str_temp = "";
            int def_color = 0;
            ListZone.Clear();
            for (int n =0;n<6;n++)
            {
                Zone z = new Zone();
                z.ListNGcode = new List<int>();
                z.ListTrayPosIdx = new List<int>();
                str_section =  string.Format("ZONE{0}", n);
                z.Zname = inf.ReadString(str_section, "Name", string.Format("分区{0}", n+1));
                if (z.Zname == "") continue;
                switch (n)
                {
                    case 0:
                        def_color = -7876885;
                        break;
                    case 1:
                        def_color = -989556;
                        break;
                    case 2:
                        def_color = -6632142;
                        break;
                    case 3:
                        def_color = -16181;
                        break;
                    case 4:
                        def_color = -23296;
                        break;
                    case 5:
                        def_color = -32944;
                        break;
                }
                z.Zcolor =Color.FromArgb(inf.ReadInteger(str_section, "Color", def_color));

                str_temp = inf.ReadString(str_section, !bReTest?"ListNGCode": "ListNGCode_RT", "");
                z.ListNGcode.Clear();
                if (str_temp.Length > 0)
                {
                    int[] ngcode = Array.ConvertAll<string, int>(str_temp.Split(','), s => { if (s.Length > 0) return int.Parse(s); else return -10000; });                    
                    for (int i = 0; i < ngcode.Count(); i++)
                    {
                        if (ngcode[i] > -10000) z.ListNGcode.Add(ngcode[i]);
                    }
                }

                str_temp = inf.ReadString(str_section, !bReTest ? "ListTrayPosIdx" : "ListTrayPosIdx_RT", "");
                z.ListTrayPosIdx.Clear();
                if (str_temp.Length > 0)
                {
                    int[] posidx = Array.ConvertAll<string, int>(str_temp.Split(','), s => { if (s.Length > 0) return int.Parse(s); else return -10000; });                    
                    for (int i = 0; i < posidx.Count(); i++)
                    {
                        if (posidx[i] > -10000) z.ListTrayPosIdx.Add(posidx[i]);
                    }
                }

                ListZone.Add(z);
            }

            return EM_RES.OK;
        }

        public List<string> GetZoneNameList()
        {
            List<string> ls = new List<string>();
            ls.Add("");
            if (ListZone != null)
            {
                foreach (Zone z in ListZone)
                {
                    ls.Add(z.Zname);
                }
            }
            return ls;
        }

        public EM_RES GetColorByName(string name, ref Color color)
        {
            Zone z = ListZone.Find(s => { return s.Zname == name; });
            if(z != null)
            {
                color =  z.Zcolor;
                return EM_RES.OK;
            }
            return EM_RES.ERR;
        }
        public EM_RES GetColorByIdx(int idx, ref Color color)
        {
            foreach (Zone z in ListZone)
            {         
                //第一个未设置区域为其他默认色
                if ((z.ListTrayPosIdx == null || z.ListTrayPosIdx.Count == 0) && color != Color.Transparent) color = z.Zcolor;

                if (z.ListTrayPosIdx == null) continue;
                if(z.ListTrayPosIdx.Contains(idx))
                {
                    color = z.Zcolor;
                    return EM_RES.OK;
                }
            }
            return EM_RES.OK;
        }

        public EM_RES GetColorByNGCode(int code, ref Color color)
        {
            foreach (Zone z in ListZone)
            {
                //第一个未设置区域为其他默认色
                if ((z.ListNGcode == null || z.ListNGcode.Count == 0) && color != Color.Transparent) color = z.Zcolor;

                if (z.ListNGcode == null) continue;
                if (z.ListNGcode.Contains(code))
                {
                    color = z.Zcolor;
                    return EM_RES.OK;
                }
            }
            return EM_RES.OK;
        }

        /// <summary>
        /// 获取包含指定NG代码的位置列表
        /// </summary>
        /// <param name="ngcode"></param>
        /// <returns></returns>
        public List<int> GetPosIdxListByNGCode(int ngcode)
        {
            List<int> ls = new List<int>();
            if (ListZone == null) return ls;
            foreach (Zone z in ListZone)
            {
                if (z.ListNGcode != null && z.ListNGcode.Contains(ngcode))
                {
                    if (z.ListTrayPosIdx == null) continue;
                    foreach (int idx in z.ListTrayPosIdx)
                    {
                        ls.Add(idx);
                    }
                }
            }
            return ls;
        }

        /// <summary>
        /// 获取所有被NG代码定义的位置列表
        /// </summary>
        /// <returns></returns>
        public List<int> GetPosIdxListDefByNGCode()
        {
            List<int> ls = new List<int>();
            if (ListZone == null) return ls;
            foreach (Zone z in ListZone)
            {
                if (z.ListNGcode != null && z.ListNGcode.Count > 0)
                {
                    if (z.ListTrayPosIdx == null) continue;
                    foreach (int idx in z.ListTrayPosIdx)
                    {
                        ls.Add(idx);
                    }
                }
            }
            return ls;
        }
    }
}