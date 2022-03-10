using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySql;
using System.Windows.Forms;
namespace UI
{
    class mysql
    {
       
        public static List<string> list_tblName, coltype;
        static String connetStr = "server=127.0.0.1;port=3306;user=root;password=root123; database=shark;";
       // static String connetStr = "server=127.0.0.1;port=3306;user=root;password=mysql123;";
        // server=127.0.0.1/localhost 代表本机，端口号port默认是3306可以不写
        static MySqlConnection conn = new MySqlConnection(connetStr);
        public static void Connection()
        {
            try
            {
                if (conn.State == ConnectionState.Open)
                    return;
               
                conn.Open();//打开通道，建立连接，可能出现异常,使用try catch语句
                Console.WriteLine("已经建立连接");
                

                //在这里使用代码对数据库进行增删查改
            }
            catch (MySqlException ex)
            {
               // throw (ex);
                if (conn.State == ConnectionState.Open)
                    return;

                conn.Open();//打开通道，建立连接，可能出现异常,使用try catch语句
            }
            finally
            {

            }
        }
        public static void StopConnect()
        {
            try
            {
                conn.Close();//打开通道，建立连接，可能出现异常,使用try catch语句
                Console.WriteLine("已经关闭连接");
               // main.AddMsgSys("已经关闭连接");
                //在这里使用代码对数据库进行增删查改
            }
            catch (MySqlException ex)
            {
                //  main.AddMsgErr(ex.Message);
                throw (ex);
            }

        }
        public static bool GetTableName()
        {
            string sql = "show tables;";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = null;
            try
            {
                //if (!openconn()) return false;
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    list_tblName = new List<string>();
                    while (reader.Read())
                    {
                        string t = reader.GetString(0);
                      //  main.AddMsgSys("查询到表："+t);
                        list_tblName.Add(t);
                    }
                }
                reader.Close();
         //       closeconn();
                return true;
            }
            catch (Exception ex)
            {
                //  main.AddMsgErr(ex.Message);
                throw (ex);
                reader.Close();
            //    closeconn();
                return false;
            }
        }       
        public static bool GetColNameType(string _params="mytable")
        {
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;
            List<string> list_ColName = new List<string>();
            List<Type> list_ColType = new List<Type>();
            coltype = new List<string>();
            string sql = "show columns from " + _params + " ;";
          //  if (!openconn()) { return false; }
            cmd = new MySqlCommand(sql, conn);
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string t = reader.GetString(0);
                        Type tt = reader.GetValue(1) as Type;
                        string ttt = reader.GetString(1);
                       // main.AddMsgSys("列名："+t+";数据类型："+ttt);
                        list_ColName.Add(t);              
                        list_ColType.Add(tt);
                        coltype.Add(ttt);
                   //     string qq = reader.GetString(t);
                      //  main.AddMsgSys("获取内容" + t + ":" + qq);


                    }
                }
                reader.Close();
               
                return true;
            }
            catch (Exception ex) {
                // main.AddMsgErr(ex.Message);
                throw (ex);
                return false; }
        }
        public static void GetAllLine(string tablename = "mytable")
        {
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;
            MySqlDataReader reader1 = null;
            List<string> list_ColName = new List<string>();
            string sql = "show columns from " + tablename + " ;";
            cmd = new MySqlCommand(sql, conn);
            //  if (!openconn()) { return false; }          
            try
            {
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string t = reader.GetString(0);
                        Type tt = reader.GetValue(1) as Type;
                        string ttt = reader.GetString(1);
                   //     main.AddMsgSys("列名：" + t + ";数据类型：" + ttt);
                        list_ColName.Add(t);                     
                    }
                }
                reader.Close();
                cmd = new MySqlCommand("select * from " + tablename + " ;", conn);
                reader1 = cmd.ExecuteReader();
                if (reader1.HasRows)
                {                                 
                        // 逐行读取数据
                        while (reader1.Read())
                        {
                            foreach (var t in list_ColName)
                            {
                                string qq = reader1.GetString(t);
                               // main.AddMsgSys("获取列：" + t + "内容:" + qq);
                            }
                        }
                    
                }
                reader1.Close();

            }
            catch (Exception ex)
            {
                reader1.Close();
                reader.Close();
                throw (ex);
                // main.AddMsgErr(ex.Message);
            }


        }
        public static bool InsertData(string tableName, string ColName1, string Data1,
            string ColName2, string Data2)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("insert into " + tableName + " set " + ColName1 + " = '" + Data1 + "'"
                    + "," + ColName2 + "= '" + Data2 + "'", conn);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
              
                return false;
                throw (ex);
            }

        }
      
        public static void DeltRow(string tableName, int RowId)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("delete from " + tableName + " where id = @id", conn);
                cmd.Parameters.AddWithValue("id", RowId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                throw (ex);
                //main.AddMsgErr(ex.Message);
            }
        }
        public static bool DeltRow(string tableName,string rowName, string RowValue)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("delete from " + tableName + " where"+"(" + rowName  +" = '"+RowValue+"')" , conn);             
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw (ex); 
                
            }
        }
        public static void AlterRow(string tableName, string RowId, string ColName1, string Data1)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("update " + tableName + " set " + ColName1 + " = @pwd where id = 2" , conn);
                cmd.Parameters.AddWithValue("pwd", Data1);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw (ex);
               // main.AddMsgErr(ex.Message);
            }
        }
        public static bool IsExitIndex(string tableName, string ColName1,string GoalStr = "sd")
        {
           
            //欲插入的用户编号
            
            MySqlDataReader reader = null;
            MySqlCommand cmd = null;
            try {
                //查询此编号是否存在
                cmd = new MySqlCommand("select * from " + tableName + " where locate ('" + GoalStr + "' , " + ColName1 + ")", conn);
                //   cmd = new MySqlCommand("select name from mytable where name='www';", conn);

                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    //已经有记录使用此编号
                    return true;
                }
                else
                {
                    //此编号未被使用  
                    return false;
                }

            }
            finally
            {
                reader.Close();
            }
           
           
        }
        public static DataTable GetMessage(DataGridView  dt)
        {
         
            string P_Str_SqlStr = string.Format("SELECT id,name FROM mytable");
            MySqlDataAdapter adapter = new MySqlDataAdapter(P_Str_SqlStr, connetStr);
            DataTable P_dt = new DataTable();
            adapter.Fill(P_dt);
            dt.DataSource = P_dt;
            return P_dt;
        }
    }
}
