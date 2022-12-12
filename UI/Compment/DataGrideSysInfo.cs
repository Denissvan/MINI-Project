
using MotionCtrl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class DataGrideSysInfo : UserControl
    {
         public  delegate void ShowErrMsg(string str);
        public ShowErrMsg mShowErrMsg;
        public DataGrideSysInfo()
        {
            InitializeComponent();
        }
        public static string GetEnumDescrip(FieldInfo field)
        {

            DescriptionAttribute descAttr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (descAttr == null)
                return string.Empty;
            return descAttr.Description;
        }
        public void ShowDataToGride()
        {
            try
            {
                if (NewSysInf.NoneRunPosInfo == null)
                    NewSysInf.NoneRunPosInfo = new NewSysInf.NoneRunAll();
                var obj = Newtonsoft.Json.Linq.JObject.Parse(JsonConvert.SerializeObject(NewSysInf.NoneRunPosInfo));
                Type t = typeof(NewSysInf.NoneRunAll);
                FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                MyDge.Rows.Clear();
                int InsertClassId = MyDge.Rows.Add(new DataGridViewGroupCell(), new DataGridViewTextBoxCell(), new DataGridViewButtonCell());
                MyDge.Rows[InsertClassId].Cells[0].Value = t.Name;
                MyDge.Rows[InsertClassId].Height = 50;
                MyDge.Rows[InsertClassId].Cells[0].Style.BackColor = Color.Gray;
                MyDge.Rows[InsertClassId].Cells[1].Value = "";
                MyDge.Rows[InsertClassId].Cells[2].Value = "";
                foreach (FieldInfo field in fields)
                {
                    var fieldTypeName = field.FieldType.Name;
                    var fieldName = field.Name;
                    int InsertFieldId = MyDge.Rows.Add(new DataGridViewGroupCell(), new DataGridViewTextBoxCell(), new DataGridViewButtonCell());
                    MyDge.Rows[InsertFieldId].Cells[0].Value = fieldName;
                    MyDge.Rows[InsertFieldId].Height = 45;
                    MyDge.Rows[InsertFieldId].Cells[0].Style.BackColor = Color.Gray;
                    MyDge.Rows[InsertFieldId].Cells[1].Value = GetEnumDescrip(field);
                    MyDge.Rows[InsertFieldId].Cells[2].Value = "";
                    ((DataGridViewGroupCell)MyDge.Rows[InsertClassId].Cells[0]).AddChildCell((DataGridViewGroupCell)MyDge.Rows[InsertFieldId].Cells[0]);


                    string StrDataobj = obj[field.Name].ToString();
                    Type temp = field.FieldType;
                    var mtype = field.FieldType;
                    object curobj = new object();


                    if (fieldTypeName == "UserSet")
                    {
                        curobj = JsonConvert.DeserializeObject<NewSysInf.UserSet>(StrDataobj);
                    }
                    else
                   if (fieldTypeName == "NoneRunAa")
                    {
                        curobj = JsonConvert.DeserializeObject<NewSysInf.NoneRunAa>(StrDataobj);
                    }
                    else if (fieldTypeName == "NoneRunSenUpDownLens")
                    {
                        curobj = JsonConvert.DeserializeObject<NewSysInf.NoneRunSenUpDownLens>(StrDataobj);
                    }
                    else if (fieldTypeName == "NoneRunSenUpDownSens")
                    {
                        curobj = JsonConvert.DeserializeObject<NewSysInf.NoneRunSenUpDownSens>(StrDataobj);
                    }
                    else if (fieldTypeName == "NoneRuntrayUnit")
                    {
                        curobj = JsonConvert.DeserializeObject<NewSysInf.NoneRuntrayUnit>(StrDataobj);
                    }
                    else
                    {
                        continue;
                    }
                    var mobj = Newtonsoft.Json.Linq.JObject.Parse(JsonConvert.SerializeObject(curobj));
                    FieldInfo[] mfields = temp.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                    foreach (FieldInfo mfield in mfields)
                    {
                        int mInsertFieldId = MyDge.Rows.Add(new DataGridViewGroupCell(), new DataGridViewTextBoxCell(), new DataGridViewButtonCell());
                        MyDge.Rows[mInsertFieldId].Cells[0].Value = fieldName + "-" + mfield.Name;
                        MyDge.Rows[mInsertFieldId].Cells[1].Value = GetEnumDescrip(mfield);
                        MyDge.Rows[mInsertFieldId].Cells[2].Value = mobj[mfield.Name].ToString();
                        ((DataGridViewGroupCell)MyDge.Rows[InsertFieldId].Cells[0]).AddChildCell((DataGridViewGroupCell)MyDge.Rows[mInsertFieldId].Cells[0]);
                    }
                }
                MyDge.Update();
            }
            catch (Exception ee)
            {
                if(mShowErrMsg!=null)
                mShowErrMsg("showDataToGride显示空跑位置数据异常:" + ee.ToString());
            }
        }
        public bool  GetDataFromGride( bool bSave=true )
        {
            try
            {
                if (NewSysInf.NoneRunPosInfo == null)
                    NewSysInf.NoneRunPosInfo = new NewSysInf.NoneRunAll();
                var obj = Newtonsoft.Json.Linq.JObject.Parse(JsonConvert.SerializeObject(NewSysInf.NoneRunPosInfo));
                Type t = typeof(NewSysInf.NoneRunAll);
                FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                foreach (FieldInfo field in fields)
                {
                    var fieldTypeName = field.FieldType.Name;
                    var fieldName = field.Name;

                    string StrDataobj = obj[field.Name].ToString();
                    Type temp = field.FieldType;
                    var mtype = field.FieldType;
                    object curobj = new object();
                    if (fieldTypeName == "UserSet")
                    {
                        curobj = JsonConvert.DeserializeObject<NewSysInf.UserSet>(StrDataobj);
                    }
                    else
                   if (fieldTypeName == "NoneRunAa")
                    {
                        curobj = JsonConvert.DeserializeObject<NewSysInf.NoneRunAa>(StrDataobj);
                    }
                    else if (fieldTypeName == "NoneRunSenUpDownLens")
                    {
                        curobj = JsonConvert.DeserializeObject<NewSysInf.NoneRunSenUpDownLens>(StrDataobj);
                    }
                    else if (fieldTypeName == "NoneRunSenUpDownSens")
                    {
                        curobj = JsonConvert.DeserializeObject<NewSysInf.NoneRunSenUpDownSens>(StrDataobj);
                    }
                    else if (fieldTypeName == "NoneRuntrayUnit")
                    {
                        curobj = JsonConvert.DeserializeObject<NewSysInf.NoneRuntrayUnit>(StrDataobj);
                    }
                    else
                    {
                        continue;
                    }
                    var mobj = Newtonsoft.Json.Linq.JObject.Parse(JsonConvert.SerializeObject(curobj));
                    FieldInfo[] mfields = temp.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

                    foreach (FieldInfo mfield in mfields)
                    {

                        for (int i = 0; i < MyDge.Rows.Count; i++)
                        {
                            var row = MyDge.Rows[i];
                            if (row.Cells[0].Value.ToString() == fieldName + "-" + mfield.Name)
                            {
                                mobj[mfield.Name] = row.Cells[2].Value.ToString();

                            }
                        }
                    }
                    obj[field.Name] = JsonConvert.SerializeObject(mobj);
                }
              
                string ss = obj.ToString();
                ss = ss.Replace("\\r\\n", "");
                ss = ss.Replace("\r\n", "");
                ss = ss.Replace("\\", "");
                ss = ss.Replace(": \"{", ":{");
                ss = ss.Replace("}\"", "}");

                NewSysInf.NoneRunPosInfo = JsonConvert.DeserializeObject<NewSysInf.NoneRunAll>(ss);
                if (bSave)
                    return NewSysInf.SaveSysInf(out var msg);
                return true;
            }
            catch (Exception ee)
            {
                if (mShowErrMsg != null)
                    mShowErrMsg("getDataFromGride加载空跑位置数据异常:" + ee.ToString());
                return false;
            }

        }

        private void MyDge_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var mcell = MyDge.SelectedCells[0];
            if (mcell.ColumnIndex == 0)
            {
                var newCell = (DataGridViewGroupCell)mcell;
                if (newCell.ChildCells.Length > 0)
                {
                    newCell.UpdataShowChild();
                }
            }

        }
    }

    public class NewSysInf
    {
        public class NoneRunAa
        {
            [Description("Aa大Xx位置")]
            public double AaXx;
            [Description("Aa1小x位置")]
            public double x1;
            [Description("Aa1小y位置")]
            public double y1;
            [Description("Aa2小x位置")]
            public double x2;
            [Description("Aa2小y位置")]
            public double y2;
            [Description("Aa小z位置")]
            public double z;
            [Description("AaU位置")]
            public double u;
            [Description("AaV位置")]
            public double v;
        }
        public class NoneRunSenUpDownSens
        {
            [Description("Y拍照位置")]
            public double YPhoto;
            [Description("吸头Z1下降位")]
            public double Z1;
            [Description("吸头Z2下降位")]
            public double Z2;
            [Description("Y取料位置")]
            public double YPick;
        }
        public class NoneRunSenUpDownLens
        {
            [Description("Y拍照位置")]
            public double YPhoto;
            [Description("吸头Z1下降位")]
            public double Z1;
            [Description("吸头Z2下降位")]
            public double Z2;
            [Description("Y取料位置")]
            public double YPick;
        }
        public class NoneRuntrayUnit
        {
            [Description("大Xx工作位置")]
            public double XxUp;
            [Description("大Xx下料盘位置")]
            public double XxDown;
            [Description("小x上料工作位置")]
            public double x;
            [Description("料盘z上升位置")]
            public double z;
        }
        public class UserSet
        {
            [Description("红灯亮蜂鸣响")]
            public bool RedLightSund;

        }

        public class NoneRunAll
        {
            [Description("常见用户设置")]
            public UserSet UserNormalSet = new UserSet();
            //[Description("Raa工作位置")]
            //public NoneRunAa Raa = new NoneRunAa();
            //[Description("A左Lens上下料单元")]
            //public NoneRunSenUpDownLens AlLens = new NoneRunSenUpDownLens();
            //[Description("A右Lens上下料单元")]
            //public NoneRunSenUpDownLens ARLens = new NoneRunSenUpDownLens();
            //[Description("B左Lens上下料单元")]
            //public NoneRunSenUpDownLens BRLens = new NoneRunSenUpDownLens();
            //[Description("B右Lens上下料单元")]
            //public NoneRunSenUpDownLens BLLens = new NoneRunSenUpDownLens();

            //[Description("A左Sens上下料单元")]
            //public NoneRunSenUpDownSens AlSens = new NoneRunSenUpDownSens();
            //[Description("A右Sens上下料单元")]
            //public NoneRunSenUpDownSens ARSens = new NoneRunSenUpDownSens();
            //[Description("B左Sens上下料单元")]
            //public NoneRunSenUpDownSens BRSens = new NoneRunSenUpDownSens();
            //[Description("B右Sens上下料单元")]
            //public NoneRunSenUpDownSens BLSens = new NoneRunSenUpDownSens();

            //[Description("tray单元1")]
            //public NoneRuntrayUnit uint1 = new NoneRuntrayUnit();
            //[Description("tray单元2")]
            //public NoneRuntrayUnit uint2 = new NoneRuntrayUnit();
            //[Description("tray单元3")]
            //public NoneRuntrayUnit uint3 = new NoneRuntrayUnit();
            //[Description("tray单元4")]
            //public NoneRuntrayUnit uint4 = new NoneRuntrayUnit();
        }

        [Description("空跑位置参数")]
        public static NoneRunAll NoneRunPosInfo = new NoneRunAll();


        public static bool LoadSysInfCfg( out string errMsg)
        {
            errMsg = "";
            try
            {
                //产品参数
                string filename = string.Format("{0}\\product\\NewSysInf.ini", Path.GetFullPath(".."));

                if (!File.Exists(filename))
                {
                    errMsg = string.Format("{0}加载NewSysInf配置文件不存在!", "LoadSysInfCfg");
                    return false;
                }

                IniFile inf = new IniFile(filename);

                var AllPosStr = inf.ReadString("OTHER_SET", "NoneRunPosInfo", "");
                if (AllPosStr.Length > 10)
                {
                    NewSysInf.NoneRunPosInfo = JsonConvert.DeserializeObject<NewSysInf.NoneRunAll>(AllPosStr);
                    return true;
                }

                return false;
            }
            catch (Exception ee)
            {
                return false;
            }
        }

        public static bool SaveSysInf( out string errmsg)
        {
            errmsg = "";
            try
            {
                EM_RES res = EM_RES.OK;
                //产品参数
                string filename = string.Format("{0}\\product\\NewSysInf.ini", Path.GetFullPath(".."));
            
                if (!File.Exists(filename))
                {
                     new FileStream(filename, FileMode.Create);
                }
               
                string[] backup = File.ReadAllLines(filename);
                bool ischange = false;
                //if (!File.Exists(filename))
                //{
                //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0}保存异常对应产品名{1}配置文件不存在!", "SavePtCfg", productname));
                //    return EM_RES.PARA_ERR;
                //}
                IniFile inf = new IniFile(filename);
                //门禁

                var EqpPos = JsonConvert.SerializeObject(NewSysInf.NoneRunPosInfo);
                inf.WriteString("OTHER_SET", "NoneRunPosInfo", EqpPos, ref ischange, true, filename);
                if (ischange)
                {
                    //创建backup
                    string backup_filename = string.Format("{0}\\product\\backup", Path.GetFullPath(".."));
                    res = SYS_PUD.CopyFile2(backup_filename);
                    if (res != EM_RES.OK) return false;
                    res = SYS_PUD.FileWriteLine("NewSysInf.ini", backup, backup_filename);
                    if (res != EM_RES.OK) return false;
                }

                return true;
            }catch(Exception ee)
            {
                return false;
            }
        }
    }

}
