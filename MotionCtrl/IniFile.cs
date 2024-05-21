using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MotionCtrl
{
    public class IniFile
    {
        #region ini文件读写API
        
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal,
            int size, string filePath);

        #endregion

        /// <summary>
        /// ini文件完整路径
        /// </summary>
        private readonly string _fileName;

        /// <summary>
        /// 类读写支持变量的类型
        /// </summary>
        private readonly List<string> _listType = new List<string>()
        {
            "Double",
            "Single",
            "Int16",
            "Int32",
            "Int64",
            "Char",
            "Byte",
            "Boolean",
            "UInt16",
            "UInt32",
            "UInt64",
            "UChar"
        };

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filename">ini完整路径</param>
        public IniFile(string filename)
        {
            _fileName = filename;
            //目录不存在则创建
            var path = Path.GetDirectoryName(_fileName);
            if(path!=null){
                if (path.Length>0 && !Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            
        }

        /// <summary>
        /// 写入字符串
        /// </summary>
        /// <param name="section">节名</param>
        /// <param name="ident">键名</param>
        /// <param name="value">值</param>
        /// <param name="check">是否记录变更</param>
        public void WriteString(string section, string ident, string value, ref bool ischange, bool check = true,string filename="",string username="",string productname = "")
        {
            if (check)
            {
                if (productname.Length < 3) productname = VAR.gsys_set.cur_product_name;
                if (username.Length < 3) username = VAR.CurUserName;
                string[] str;
                string fname = string.Empty;
                if (filename.Length>3)
                {                   
                    str = filename.Split('\\');
                    str = str[str.Length - 1].Split('.');
                    fname = str[0];
                }
                filename = fname;
                string info = string.Empty;
                var v = ReadString(section, ident, "EMPTY");
                // if (v != value) Logger.Param($"[{section}][{ident}]{v}->{value}");

                if (v != value)
                {
                    ischange = true;
                    info = string.Format("[产品:{0}][文件:{1}][用户:{2}][字段:{3}][参数:{4}:{5}->{6}]",productname,filename,username,section,ident,v,value);
                    DRpt.Report_Opration(1000, 0, info);
                    string ls=string.Format("{0:HH:mm:ss.fff}", DateTime.Now);
                    info=string.Format("[{0}]",ls)+info;
                    //info = $"[{string.Format("{0:HH:mm:ss.fff}", DateTime.Now)}]" + info;
                    Msg.SaveMsg(ref Msg.Log_Info, info);
                    Msg.Log_Info.Flush();
                }                   
                else return;
            }
            
            if (!WritePrivateProfileString(section, ident, value, _fileName))
            {
                throw (new ApplicationException("write inf file error"));
            }
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="section">节名</param>
        /// <param name="ident">键名</param>
        /// <param name="Default">默认值</param>
        /// <returns>返回指定位置的字符串</returns>
        public string ReadString(string section, string ident, string Default)
        {
            byte[] buffer = new byte[1024];
            int bufLen = GetPrivateProfileString(section, ident, Default, buffer, buffer.GetUpperBound(0), _fileName);
            //必须设定0（系统默认的代码页）的编码方式，否则无法支持中文
            string s = Encoding.GetEncoding(0).GetString(buffer);
            s = s.Substring(0, bufLen);
            return s.Trim('\0');
        }

        /// <summary>
        /// 读取整数
        /// </summary>
        /// <param name="section">节名</param>
        /// <param name="ident">键名</param>
        /// <param name="Default">默认值</param>
        /// <returns>返回指定位置的整数</returns>
        public int ReadInteger(string section, string ident, int Default)
        {
            string intStr = ReadString(section, ident, Convert.ToString(Default));
            try
            {
                return Convert.ToInt32(intStr);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Default;
            }
        }

        /// <summary>
        /// 写入整数
        /// </summary>
        /// <param name="section">节名</param>
        /// <param name="ident">键名</param>
        /// <param name="value">默认值</param>
        /// <param name="check">是否记录变更</param>
        public void WriteInteger(string section, string ident, int value, ref bool ischange, bool check = true, string filename = "", string username = "", string productname = "")
        {
            WriteString(section, ident, value.ToString(),ref ischange, check,filename, username, productname);
        }

        /// <summary>
        /// 读取布尔值
        /// </summary>
        /// <param name="section">节名</param>
        /// <param name="ident">键名</param>
        /// <param name="Default">默认值</param>
        /// <returns>返回指定位置的布尔值</returns>
        public bool ReadBool(string section, string ident, bool Default)
        {
            try
            {
                return Convert.ToBoolean(ReadString(section, ident, Convert.ToString(Default)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Default;
            }
        }

        /// <summary>
        /// 写入布尔值
        /// </summary>
        /// <param name="section">节名</param>
        /// <param name="ident">键名</param>
        /// <param name="value">默认值</param>
        /// <param name="check">是否记录变更</param>
        public void WriteBool(string section, string ident, bool value, ref bool ischange, bool check = true, string filename = "", string username = "", string productname = "")
        {
            WriteString(section, ident, Convert.ToString(value),ref ischange, check, filename, username, productname);
        }

        /// <summary>
        /// 读取浮点数
        /// </summary>
        /// <param name="section">节名</param>
        /// <param name="ident">键名</param>
        /// <param name="Default">默认值</param>
        /// <returns>返回指定位置的浮点数</returns>
        public double ReadDouble(string section, string ident, double Default)
        {
            string intStr = ReadString(section, ident, Convert.ToString(Default, CultureInfo.InvariantCulture));
            try
            {
                return Convert.ToDouble(intStr);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Default;
            }
        }

        /// <summary>
        /// 写入浮点数
        /// </summary>
        /// <param name="section">节名</param>
        /// <param name="ident">键名</param>
        /// <param name="value">默认值</param>
        /// <param name="check">是否记录变更</param>
        public void WriteDouble(string section, string ident, double value, ref bool ischange, bool check = true, string filename = "", string username = "", string productname = "")
        {
            WriteString(section, ident, Convert.ToString(value, CultureInfo.CurrentCulture), ref ischange, check, filename, username, productname);
        }
        /// <summary>
        /// 读取XYZ类型
        /// </summary>
        /// <param name="section"></param>
        /// <param name="identPrefix"></param>
        /// <param name="defaultX"></param>
        /// <param name="defaultY"></param>
        /// <param name="defaultZ"></param>
        /// <returns></returns>
        public ST_XYZ ReadXYZ(string section, string identPrefix, double defaultX = 0, double defaultY = 0, double defaultZ = 0)
        {
            // 读取x坐标值
            string xStr = ReadString(section, $"{identPrefix}X", defaultX.ToString(CultureInfo.CurrentCulture));
            double x = double.TryParse(xStr, NumberStyles.Any, CultureInfo.CurrentCulture, out double xVal)
                ? xVal
                : defaultX;

            // 读取y坐标值
            string yStr = ReadString(section, $"{identPrefix}Y", defaultY.ToString(CultureInfo.CurrentCulture));
            double y = double.TryParse(yStr, NumberStyles.Any, CultureInfo.CurrentCulture, out double yVal)
                ? yVal
                : defaultY;

            // 读取z坐标值
            string zStr = ReadString(section, $"{identPrefix}Z", defaultZ.ToString(CultureInfo.CurrentCulture));
            double z = double.TryParse(zStr, NumberStyles.Any, CultureInfo.CurrentCulture, out double zVal)
                ? zVal
                : defaultZ;

            // 创建并返回ST_XYZ对象
            return new ST_XYZ(x, y, z);
        }
        /// <summary>
        /// 将ST_XYZ对象的坐标值写入配置文件
        /// </summary>
        /// <param name="section">配置节名称</param>
        /// <param name="identPrefix">标识前缀，用于区分不同的XYZ组</param>
        /// <param name="xyz">要写入的ST_XYZ对象</param>
        /// <param name="ischange">是否有变更的标志（引用传递）</param>
        /// <param name="check">是否检查变更并记录日志</param>
        /// <param name="filename">文件名</param>
        /// <param name="username">用户名</param>
        /// <param name="productname">产品名称</param>
        public void WriteXYZ(string section, string identPrefix, ST_XYZ xyz, ref bool ischange,
                            bool check = true, string filename = "", string username = "", string productname = "")
        {
            
            //// 检查传入的对象是否为空
            //if (xyz.Equals(default(ST_XYZ)))
            //    throw new ArgumentNullException(nameof(xyz), "ST_XYZ对象不能为null");

            // 分别写入X、Y、Z坐标值，使用前缀+坐标名作为标识
            WriteDouble(section, $"{identPrefix}X", xyz.x, ref ischange, check, filename, username, productname);
            WriteDouble(section, $"{identPrefix}Y", xyz.y, ref ischange, check, filename, username, productname);
            WriteDouble(section, $"{identPrefix}Z", xyz.z, ref ischange, check, filename, username, productname);
        }

        /// <summary>
        /// 把类中变量值写入文件（支持类型在ListType列出）
        /// </summary>
        /// <param name="section">字段</param>
        /// <param name="obj">类对象</param>
        /// <param name="prefix">键名前缀</param>
        /// <returns></returns>
        public int WriteClass(string section, string prefix, object obj , ref bool ischange, bool check = true, string filename = "", string username = "", string productname = "")
        {
            int cnt = 0;
            var pArray = obj.GetType().GetFields();
            foreach (var p in pArray)
            {
                if (_listType.Contains(p.FieldType.Name) || p.FieldType.IsEnum)
                {
                    WriteString(section, string.Format("{0}{1}",prefix,p.Name), p.GetValue(obj).ToString(),ref ischange,check,filename,username,productname);
                    cnt++;
                }
            }

            return cnt;
        }

        /// <summary>
        /// 从文件读取类中变量值（支持类型在ListType列出）
        /// </summary>
        /// <param name="section">字段</param>
        /// <param name="obj">类对象</param>
        /// <param name="prefix">键名前缀</param>
        /// <returns>读取到的个数</returns>
        public int ReadClass(string section, object obj, string prefix = "")
        {
            int cnt = 0;
            FieldInfo[] pArray = obj.GetType().GetFields();
            foreach (FieldInfo p in pArray)
            {
                if (_listType.Contains(p.FieldType.Name) || p.FieldType.IsEnum)
                {
                    string str = ReadString(section, string.Format("{0}{1}", prefix, p.Name), "");
                    if (str == "") continue;

                    p.SetValue(obj,
                        p.FieldType.IsEnum ? Enum.Parse(p.FieldType, str) : Convert.ChangeType(str, p.FieldType));
                    cnt++;
                }
            }

            return cnt;
        }
    }
}
