using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using MotionCtrl;
using UI.Compment;


namespace UI
{
    public enum enumLanguage
    {
        Chinese,
        English,
        Vietnamese,
    }

    static class MultiLanguage
    {
        //当前默认语言
        public static string DefaultLanguage = "ChineseSimplified";
        public static enumLanguage CurrentLanguage { set; get; } = enumLanguage.Chinese;
        private static List<string> ListMenu = new List<string>();
        private static Dictionary<string, ToolStripMenuItem> DicMenu = new Dictionary<string, ToolStripMenuItem>();
        public static string strRead = "\\Languages\\" + "readme" + ".xml";
        public static string strFile = System.Windows.Forms.Application.StartupPath + strRead;
        /// <summary>
        /// 读取当前默认语言
        /// </summary>
        /// <returns>当前默认语言</returns>
        public static string GetDefaultLanguage()
        {
            string defaultLanguage = "ChineseSimplified";

            XDocument document = new XDocument();

            if (!System.IO.File.Exists(strFile))
            {
                defaultLanguage = string.Empty;
                return defaultLanguage;
            }

            document = XDocument.Load(strFile);

            XElement root1 = document.Root;
            defaultLanguage = root1.FirstAttribute.Value;


            return defaultLanguage;
        }

        /// <summary>
        /// 修改默认语言
        /// </summary>
        /// <param name="lang">待设置默认语言</param>
        public static void SetDefaultLanguage(string lang)
        {
            DataSet ds = new DataSet();

            XDocument document = new XDocument();
            document = XDocument.Load(strFile);
            XElement root = document.Root;
            root.FirstAttribute.Value = lang;
            document.Save(strFile);

        }

        private static void EnumerateMenu(ToolStripMenuItem item)
        {
            foreach (ToolStripMenuItem subItem in item.DropDownItems)
            {
                ListMenu.Add(subItem.Name);
                DicMenu.Add(subItem.Name, subItem);
                EnumerateMenu(subItem);
            }
        }

        /// <summary>
        /// 加载语言
        /// </summary>
        /// <param name="form">加载语言的窗口</param>
        public static bool LoadLanguage(Form form, string language)
        {
            if (form == null || form.IsDisposed)
            {
                return false;
            }
            if (string.IsNullOrEmpty(language))
            {
                return false;
            }
            //根据用户选择的语言获得表的显示文字 
            Hashtable hashText = ReadXMLText(form.Name, language);
            Hashtable hashHeaderText = ReadXMLHeaderText(form.Name, language);
            if (hashText == null)
            {
                return false;
            }
            //获取当前窗口的所有控件
            Control.ControlCollection sonControls = form.Controls;

            try
            {
                DicMenu.Clear();
                ListMenu.Clear();
                MenuStrip menu = form.MainMenuStrip;
                if (menu != null)
                {
                    foreach (ToolStripMenuItem item in menu.Items)
                    {
                        ListMenu.Add(item.Name);
                        DicMenu.Add(item.Name, item);
                        EnumerateMenu(item);
                    }
                }

                var result = from pair in DicMenu orderby pair.Key select pair;
                foreach (KeyValuePair<string, ToolStripMenuItem> pair in result)
                {
                    if (hashText.Contains(pair.Key))
                    {
                        pair.Value.Text = (string)hashText[pair.Key];
                    }
                }


                //遍历所有控件
                foreach (Control control in sonControls)
                {
                    if (control.GetType() == typeof(Panel))     //Panel
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(GroupBox))     //GroupBox
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(TabControl))       //TabControl
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(TabPage))      //TabPage
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }

                    else if (control.GetType() == typeof(DataGridView))     //DataGridView
                    {
                        GetSetHeaderCell((DataGridView)control, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(Button))     //Button
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(ToolStripMenuItem))     //menu
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(CTabControl))     //CTabControl
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(AffineCail))     //AffineCail
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(RotAndFly))     //RotAndFly
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(TaskNpoint))     //TaskNpoint
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(NpointCail))     //NpointCail
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(traybox))     //traybox
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(LightBoxDef))     //LightBoxDef
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(XtOfsAdj))     //XtOfsAdj
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(CogDisplayer))     //CogDisplayer
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(Compment.AZD))     //AZD
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(WsModuelDef))     //WsModuelDef
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(RadioButton))     //RadioButton
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(FlowLayoutPanel))     //FlowLayoutPanel
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(TableLayoutPanel))     //TableLayoutPanel
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(SQLSelector))     //SQLSelector
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(NGcode))     //NGcode
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    //else if (control.GetType() == typeof(ToolStrip))     //ToolStrip
                    //{
                    //    GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    //}
                    //else if (control.GetType().Name == typeof(ToolStripLabel).Name + "Control")     //ToolStripLabel
                    //{
                    //    GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    //}
                    //else if (control.GetType().Name == typeof(ToolStripButton).Name + "Control")     //ToolStripButton
                    //{
                    //    GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    //}




                    if (hashText.Contains(control.Name))
                    {
                        control.Text = (string)hashText[control.Name];
                    }

                }
                //如果找到了控件，就将对应的名字赋值过去
                if (hashText.Contains(form.Name))
                {
                    form.Text = (string)hashText[form.Name];
                }
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
                return false;
            }
            return true;
        }


        /// <summary>
        /// 获取并设置控件中的子控件
        /// </summary>
        /// <param name="controls">父控件</param>
        /// <param name="hashResult">哈希表</param>
        private static void GetSetSubControls(Control.ControlCollection controls, Hashtable hashText, Hashtable hashHeaderText)
        {
            try
            {
                foreach (Control control in controls)
                {
                    if (control.GetType() == typeof(Panel))     //Panel
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(GroupBox))     //GroupBox
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }

                    else if (control.GetType() == typeof(TabControl))       //TabControl
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(TabPage))      //TabPage
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(DataGridView))
                    {
                        GetSetHeaderCell((DataGridView)control, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(Button))     //Button
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(CTabControl))     //CTabControl
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(AffineCail))     //AffineCail
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(RotAndFly))     //RotAndFly
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(TaskNpoint))     //TaskNpoint
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(NpointCail))     //NpointCail
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(traybox))     //traybox
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(LightBoxDef))     //LightBoxDef
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(XtOfsAdj))     //XtOfsAdj
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(CogDisplayer))     //CogDisplayer
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(Compment.AZD))     //AZD
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(WsModuelDef))     //WsModuelDef
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(RadioButton))     //RadioButton
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(FlowLayoutPanel))     //FlowLayoutPanel
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(TableLayoutPanel))     //TableLayoutPanel
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(SQLSelector))     //SQLSelector
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    else if (control.GetType() == typeof(NGcode))     //NGcode
                    {
                        GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    }
                    //else if (control.GetType() == typeof(ToolStrip))     //ToolStrip
                    //{
                    //    GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    //}
                    //else if (control.GetType().Name == typeof(ToolStripLabel).Name + "Control")     //ToolStripLabel
                    //{
                    //    GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    //}
                    //else if (control.GetType().Name == typeof(ToolStripButton).Name + "Control")     //ToolStripButton
                    //{
                    //    GetSetSubControls(control.Controls, hashText, hashHeaderText);
                    //}


                    if (hashText.Contains(control.Name))
                    {
                        control.Text = (string)hashText[control.Name];
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 从XML文件中读取需要修改Text的內容
        /// </summary>
        /// <param name="frmName">窗口名，用于获取对应窗口的那部分内容</param>
        /// <param name="xmlName">目标语言</param>
        /// <returns></returns>
        private static Hashtable ReadXMLText(string frmName, string xmlName)
        {
            try
            {
                Hashtable hashResult = new Hashtable();
                XmlReader reader = null;
                //判断是否存在该语言的配置文件
                if (!(new System.IO.FileInfo(System.Windows.Forms.Application.StartupPath + "\\Languages\\" + xmlName + ".xml")).Exists)
                {
                    return null;
                }
                else
                {
                    reader = new XmlTextReader(System.Windows.Forms.Application.StartupPath + "\\Languages\\" + xmlName + ".xml");
                }
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                XmlNode root = doc.DocumentElement;
                //获取XML文件中对应该窗口的内容
                XmlNodeList nodeList = root.SelectNodes("Form[Name='" + frmName + "']/Controls/Control");
                foreach (XmlNode node in nodeList)
                {
                    try
                    {
                        //修改内容为控件的Text值
                        XmlNode node1 = node.SelectSingleNode("@name");
                        XmlNode node2 = node.SelectSingleNode("@text");
                        if (node1 != null)
                        {
                            hashResult.Add(node1.InnerText, node2.InnerText);
                        }
                    }
                    catch (Exception ex)
                    {
                        string s = ex.ToString();
                    }
                }
                reader.Close();
                reader.Dispose();
                return hashResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 从XML文件中读取需要修改HeaderText的內容
        /// </summary>
        /// <param name="frmName">窗口名，用于获取对应窗口的那部分内容</param>
        /// <param name="xmlName">目标语言</param>
        /// <returns></returns>
        private static Hashtable ReadXMLHeaderText(string frmName, string xmlName)
        {
            try
            {
                Hashtable hashResult = new Hashtable();
                XmlReader reader = null;
                //判断是否存在该语言的配置文件
                if (!(new System.IO.FileInfo(System.Windows.Forms.Application.StartupPath + "\\Languages\\" + xmlName + ".xml")).Exists)
                {
                    return null;
                }
                else
                {
                    reader = new XmlTextReader(System.Windows.Forms.Application.StartupPath + "\\Languages\\" + xmlName + ".xml");
                }


                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                XmlNode root = doc.DocumentElement;
                //获取XML文件中对应该窗口的内容
                XmlNodeList nodeList = root.SelectNodes("Form[Name='" + frmName + "']/DataGridViewCells/DataGridViewCell");
                foreach (XmlNode node in nodeList)
                {
                    try
                    {
                        //修改内容为控件的Text值
                        XmlNode node1 = node.SelectSingleNode("@name");
                        XmlNode node2 = node.SelectSingleNode("@HeaderText");
                        if (node1 != null)
                        {
                            hashResult.Add(node1.InnerText.ToLower(), node2.InnerText);
                        }
                    }
                    catch { }
                }
                reader.Close();
                reader.Dispose();
                return hashResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 获取并设置DataGridView的列头
        /// </summary>
        /// <param name="dataGridView">DataGridView名</param>
        /// <param name="hashResult">哈希表</param>
        private static void GetSetHeaderCell(DataGridView dataGridView, Hashtable hashHeaderText)
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                if (hashHeaderText.Contains(column.Name.ToLower()))
                {
                    column.HeaderText = (string)hashHeaderText[column.Name.ToLower()];
                }
            }
        }

        public static string TxtSelct(string chinese = "", string english = "", string vietnamese = "")
        {
            var language = MultiLanguage.CurrentLanguage;
            if (language == enumLanguage.Chinese)
                return chinese;
            else if (language == enumLanguage.English)
                return english + "\n" + chinese;
            else
                return vietnamese + "\n" + chinese;
        }
    }
}
