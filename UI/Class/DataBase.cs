using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    class database
    {
       
        
        public static bool IsExitIndex(string sharkID,out string msg )
        {
            try
            {
                if (sharkID.Length < 5 || sharkID.Length > 25)
                {
                    msg = "数据长度错误";
                    return false;
                }
                //mysql.StopConnect();
                mysql.Connection();
                var ret = mysql.IsExitIndex("shark", "sharkID", sharkID);
                if (ret)
                {
                    msg = "找到数据";
                    return true;
                }
                else
                {
                    msg = "未找到数据";
                    return false;
                }
            }
            catch(Exception ee)
            {
                msg = "异常错误" + ee.ToString();
                return false;
            }
            finally
            {
               // mysql.StopConnect();
            }
        }
        public static bool DeleteIndex(string sharkID, out string msg)
        {
            try
            {
                var ret = IsExitIndex(sharkID, out msg);
                if (!ret)
                {                   
                    return true;
                }
               // mysql.StopConnect();
                mysql.Connection();
                ret = mysql.DeltRow("shark", "sharkID", sharkID);
                if (ret)
                {
                    msg = "删除成功";
                    return true;
                }
                else
                {
                    msg = "删除失败";
                    return false;
                }
            }
            catch(Exception ee)
            {
                msg = "异常错误"+ee.ToString();
                return false;

            }finally
                {
             //   mysql.StopConnect();
            }

        }
        public static bool InsertIndex(string sharkID, out string msg)
        {
            try
            {
                var ret = IsExitIndex(sharkID, out msg);
                if (ret)
                {
                    ret = DeleteIndex(sharkID, out msg);
                    if (!ret)
                    {
                        return false;
                    }
                }
              //  mysql.StopConnect();
                mysql.Connection();
                if (sharkID.Length < 5 || sharkID.Length > 19)
                {
                    msg = "数据长度错误";
                    return false;
                }
                 ret = mysql.InsertData("shark", "sharkID", sharkID, "CreateTime", DateTime.Now.ToString());
                if(ret)
                    msg = "插入数据成功";
                else
                    msg = "插入数据失败";
                return ret;
            }
            catch(Exception ee)
            {
                msg = "发生异常"+ee.ToString();
                return false;
            }
            finally
            {
               // mysql.StopConnect();
            }
        }
    }
}
