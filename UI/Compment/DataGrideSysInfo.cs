
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
using UI.Class;

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
                    {
                        continue;
                    }
                    var mobj = Newtonsoft.Json.Linq.JObject.Parse(JsonConvert.SerializeObject(curobj));
                    FieldInfo[] mfields = temp.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                    foreach (FieldInfo mfield in mfields)
                    {
                        bool bbool = mfield.FieldType.Name.Contains("ool");
                        
                        int mInsertFieldId = 0;

                        if (bbool)
                        {
                            mInsertFieldId = MyDge.Rows.Add(new DataGridViewGroupCell(), new DataGridViewTextBoxCell(), new DataGridViewCheckBoxCell());
                        }
                        else
                        {
                            mInsertFieldId = MyDge.Rows.Add(new DataGridViewGroupCell(), new DataGridViewTextBoxCell(), new DataGridViewTextBoxCell());
                        }
                        MyDge.Rows[mInsertFieldId].Cells[0].Value = fieldName + "-" + mfield.Name;
                        MyDge.Rows[mInsertFieldId].Cells[1].Value = GetEnumDescrip(mfield);
                      
                        if (bbool)                  
                        {
                            MyDge.Rows[mInsertFieldId].Cells[2] = new DataGridViewCheckBoxCell() { Value = mobj[mfield.Name].ToString().Contains("rue") };
                           
                        }else
                        {
                            MyDge.Rows[mInsertFieldId].Cells[2].Value = mobj[mfield.Name].ToString();
                        }
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


        private void PasteData()
        {
            string clipboardText = Clipboard.GetText(); //获取剪贴板中的内容
            if (clipboardText.Substring(clipboardText.Length - 1, 1) != "\n")
            {
                clipboardText += "\n";
            }
            if (string.IsNullOrEmpty(clipboardText))
            {
                return;
            }
            int colnum = 0;
            int rownum = 0;
            for (int i = 0; i < clipboardText.Length; i++)
            {
                if (clipboardText.Substring(i, 1) == "\t")
                {
                    colnum++;
                }
                if (clipboardText.Substring(i, 1) == "\n")
                {
                    rownum++;
                }
            }
            colnum = colnum / rownum + 1;
            int selectedRowIndex, selectedColIndex;
            selectedRowIndex = this.MyDge.CurrentRow.Index;
            selectedColIndex = this.MyDge.CurrentCell.ColumnIndex;
            if (selectedRowIndex + rownum > MyDge.RowCount || selectedColIndex + colnum > MyDge.ColumnCount)
            {
                MessageBox.Show("粘贴区域大小不一致");
                return;
            }
            String[][] temp = new String[rownum][];
            for (int i = 0; i < rownum; i++)
            {
                temp[i] = new String[colnum];
            }
            int m = 0, n = 0, len = 0;
            while (len != clipboardText.Length)
            {
                String str = clipboardText.Substring(len, 1);
                if (str == "\t")
                {
                    n++;
                }
                else if (str == "\n")
                {
                    m++;
                    n = 0;
                }
                else
                {
                    temp[m][n] += str;
                }
                len++;
            }
            for (int i = selectedRowIndex; i < selectedRowIndex + rownum; i++)
            {
                for (int j = selectedColIndex; j < selectedColIndex + colnum; j++)
                {
                    this.MyDge.Rows[i].Cells[j].Value = temp[i - selectedRowIndex][j - selectedColIndex];
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //在检测到按Ctrl+V键后，系统无法复制多单元格数据，重写ProcessCmdKey方法，屏蔽系统粘贴事件，使用自定义粘贴事件，在事件中对剪贴板的HTML格式进行处理，获取表数据，更新DataGrid控件内容
            if (keyData == (Keys.V | Keys.Control))  // &&
            {
                IDataObject idataObject = Clipboard.GetDataObject();
                string[] s = idataObject.GetFormats();
                string data;
                if (!s.Any(f => f == "OEMText"))
                {
                    if (!s.Any(f => f == "HTML Format"))
                    {
                    }
                    else
                    {
                        //data = idataObject.GetData("HTML Format").ToString();//多个单元格
                        //copyClipboardHtmltoGrid(data);
                        PasteData();
                        //msg = Message.;
                        msg = new Message();
                        return base.ProcessCmdKey(ref msg, Keys.Control);
                    }
                }
                else
                    data = idataObject.GetData("OEMText").ToString();//单个单元格,使用系统功能，无需处理
            }
            MyDge.Update();
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

   

}
