using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MotionCtrl;
using System.IO;
using System.Drawing;
using System.ComponentModel;

namespace UI
{
    public class Product
    {
        public string name = "";

        public enum EM_CM_RES
        {
            [Description("视觉NG")]
            CAMERR = -4,
            [Description("随机")]
            RANDOM = -3,
            [Description("空料")]
            EMPTY = -2,
            [Description("待测")]
            UNTEST =-1,            
            [Description("OK")]
            OK =0,
            [Description("NG")]
            NG =1
        }

        public class MdDat
        {
            //编号
            public int Num;
            //放料位置
            public ST_XYZ st_pos;
            //工站编号
            public int WS_ID;
            //对应测试电脑
            public int PC_ID;
            public string PC_IP;
            //当前测试位置
            public int test_idx;
            //工装编号
            public int TestBox_ID;
            //序号
            public int SN;
            //结果
            public int res;
            //用时
            public int ct;
            //二维码
            public string bardcode;
            //马达二维码
            public string motor_barcode;
            //使用
            public bool benable;

            //对应tray的位置编号
            public int idx;
            //对应tray的二维码
            public string tray_barcode;
            //对应tray的坐标
            public ST_XYZA pos;
            //马达是否已经扫码
            public bool bhaveMotoScan;
            public MdDat Clone()
            {
                MdDat md = new MdDat();
                md.st_pos = st_pos.clone();
                md.pos = pos.clone();
                //md.pos = new ST_XYZA();
                //md.pos.x = pos.x;
                //md.pos.y = pos.y;
                //md.pos.z = pos.z;
                //md.pos.a = pos.a;

                md.Num = Num;
                md.WS_ID = WS_ID;
                md.PC_ID = PC_ID;
                md.PC_IP = PC_IP;
                md.test_idx = test_idx;
                md.TestBox_ID = TestBox_ID;
                md.SN = SN;
                md.bhaveMotoScan = bhaveMotoScan;
                md.res = res;
                md.ct = ct;
                md.bardcode = bardcode;
                md.benable = benable;

                md.idx = idx;
                md.tray_barcode = tray_barcode;

                return md;
            }
        };

        //public class CamModule
        //{
        //    public string barcode;
        //    public TestRes cur_res;
        //    public List<TestRes> list_res = new List<TestRes> ();
        //    public int index;
        //    public int col;
        //    public int row;
        //    public ST_XYZA posInf;
        //    public string tray_barcode;
        //    public EM_CM_RES res
        //    {
        //        get
        //        {                    
        //            if (list_res == null) return EM_CM_RES.NONE;
        //            EM_CM_RES ng = EM_CM_RES.OK;
        //            foreach (TestRes r in list_res)
        //            {
        //                if (r.res != EM_CM_RES.OK) ng = r.res;
        //            }
        //            //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG,ng.ToString());
        //            return ng;
        //        }
        //    }
        //    public CamModule(string barcode, string tray_barcode, int idx,int row,int col, ST_XYZA posInf)
        //    {
        //        this.barcode = barcode;
        //        this.tray_barcode = tray_barcode;
        //        this.index = idx;
        //        this.row = row;
        //        this.col = col;
        //        this.posInf = posInf;
        //        list_res = new List<TestRes>();
        //        cur_res = null;
        //    }

        //    public EM_RES AddResult(TestRes res)
        //    {
        //        cur_res = res;
        //        list_res.Add(cur_res);
        //        return EM_RES.OK;
        //    }
        //    //从服务器获取数据
        //    int GetResFrSvr()
        //    {
        //        return 0;
        //    }
        //    //数据上传服务器
        //}
        public class Tray
        {
            #region 参数
            public const int cnt = 2;
            public string strCfgPath ="";
            public NGCodeDef NGDef = null;
            //public event EventHandler DataChanged;
            public string barcode = "BARCODE";
            public int row = 6;
            public int col = 8;
            /// <summary>
            /// 左上角
            /// </summary>
            public ST_XYZA[] tl = new ST_XYZA[cnt];
            /// <summary>
            /// 左下角
            /// </summary>
            public ST_XYZA[] bl = new ST_XYZA[cnt];
            /// <summary>
            /// 右上角
            /// </summary>
            public ST_XYZA[] tr = new ST_XYZA[cnt];
            /// <summary>
            /// 左上角
            /// </summary>
            public ST_XYZA[] AddQrcodeTL = new ST_XYZA[cnt];
            /// <summary>
            /// 左下角
            /// </summary>
            public ST_XYZA[] AddQrcodeBL = new ST_XYZA[cnt];
            /// <summary>
            /// 右上角
            /// </summary>
            public ST_XYZA[] AddQrcodeTR = new ST_XYZA[cnt];
            /// <summary>
            /// 视觉检测
            /// </summary>
            public ST_XYZA[] TrayVsPos = new ST_XYZA[cnt];
            public class PosInf
            {
                public ST_XYZA[] Pos=new ST_XYZA[cnt];
                public int idx;
                public int Row;
                public int Col;
                public bool isEmpty;
                public bool isDisable;
                public bool isDisableTemp;
                //绘图用
                public Rectangle rect;
                //对应模组
                public MdDat md;

                public PosInf Clone()
                {
                    PosInf posInf = new PosInf();
                    for (int i = 0; i < cnt; i++)
                        posInf.Pos[i] = Pos[i].clone();
                    posInf.idx = idx;
                    posInf.Row = Row;
                    posInf.Col = Col;
                    posInf.isEmpty = isEmpty;
                    posInf.isDisable = isDisable;
                    posInf.isDisableTemp = false;
                    posInf.rect = new Rectangle(rect.X,rect.Y,rect.Width,rect.Height);
                    posInf.md = md.Clone();
                    return posInf;
                }
            }
            public List<PosInf> list_pos = new List<PosInf>();
            public List<PosInf> list_AddCapQrcodepos = new List<PosInf>();
            #endregion
            #region 构造
            public Tray()
            { }
            /// <summary>
            /// 从文件加载料盘，并按指定状态填充模组
            /// </summary>
            /// <param name="res">模组状态 -3:随机生成，-2:为空，-1:待测，0:OK,>0:NG</param>
            public Tray(string filename,EM_CM_RES res =  EM_CM_RES.UNTEST)
            {
                if (filename.Length > 0) strCfgPath = filename;
                else filename = strCfgPath;
                LoadDat(filename);
                //添加模组
                foreach (PosInf pos in list_pos)
                {
                    MdDat md = new MdDat();
                    if (res ==  EM_CM_RES.RANDOM)
                    {
                        Random rdm = new Random();
                        res = (EM_CM_RES)rdm.Next(-2, 1);
                    }
                    if (res !=  EM_CM_RES.EMPTY) md.res = (int)res;
                    else md = null;
                    if (md != null)
                        md.idx = pos.idx;
                    pos.md = md;
                }
                foreach (PosInf pos in list_AddCapQrcodepos)
                {
                    MdDat md = new MdDat();
                    if (res == EM_CM_RES.RANDOM)
                    {
                        Random rdm = new Random();
                        res = (EM_CM_RES)rdm.Next(-2, 1);
                    }
                    if (res != EM_CM_RES.EMPTY) md.res = (int)res;
                    else md = null;
                    pos.md = md;
                }
            }
            /// <summary>
            /// 指定行列生成料盘，并按指定状态填充模组
            /// </summary>
            /// <param name="row">行数</param>
            /// <param name="col">列数</param>
            /// <param name="res">模组状态 -3:随机生成，-2:为空，-1:待测，0:OK,>0:NG</param>
            public Tray(int row , int col, EM_CM_RES res = EM_CM_RES.UNTEST)
            {
                this.row = row;
                this.col = col;
                CreatePos(row, col, new ST_XYZA[2], new ST_XYZA[2], new ST_XYZA[2]);

                //添加模组
                foreach (PosInf pos in list_pos)
                {
                    MdDat md = new MdDat();
                    if (res ==  EM_CM_RES.RANDOM)
                    {
                        Random rdm = new Random();
                        res = (EM_CM_RES)rdm.Next(-2, 1);
                    }
                    if (res !=  EM_CM_RES.EMPTY) md.res = (int)res;
                    else md = null;
                    if (md != null)
                        md.idx = pos.idx;
                    pos.md = md;
                }
                foreach (PosInf pos in list_AddCapQrcodepos)
                {
                    MdDat md = new MdDat();
                    if (res == EM_CM_RES.RANDOM)
                    {
                        Random rdm = new Random();
                        res = (EM_CM_RES)rdm.Next(-2, 1);
                    }
                    if (res != EM_CM_RES.EMPTY) md.res = (int)res;
                    else md = null;
                    if (md != null)
                        md.idx = pos.idx;
                    pos.md = md;
                }
            }
            public Tray Clone()
            {
                Tray new_tray = new Tray();

                new_tray.strCfgPath = strCfgPath;
                new_tray.NGDef = NGDef;
                new_tray.barcode = barcode;
                new_tray.row = row;
                new_tray.col = col;
                for (int i = 0; i < cnt; i++)
                {
                    new_tray.tl[i] = tl[i].clone();
                    new_tray.bl[i] = bl[i].clone();
                    new_tray.tr[i] = tr[i].clone();
                    new_tray.AddQrcodeTL[i] = AddQrcodeTL[i].clone();
                    new_tray.AddQrcodeBL[i] = AddQrcodeBL[i].clone();
                    new_tray.AddQrcodeTR[i] = AddQrcodeTR[i].clone();
                    new_tray.TrayVsPos[i] = TrayVsPos[i].clone();
                }                   
                new_tray.list_pos = new List<PosInf>();
                foreach(PosInf pos in list_pos)
                {
                    new_tray.list_pos.Add(pos.Clone());
                }
                new_tray.list_AddCapQrcodepos = new List<PosInf>();
                foreach (PosInf pos in list_AddCapQrcodepos)
                {
                    new_tray.list_AddCapQrcodepos.Add(pos.Clone());
                }
                return new_tray;
            }
            #endregion
            #region 位置生成
            public void CreatePos()
            {
                CreatePos(col, row, tl, bl, tr);
            }
            public void CreatePos(int row, int col, ST_XYZA[] tl, ST_XYZA[] bl, ST_XYZA[] tr)
            {
               list_pos.Clear();
                for (int i = 0; i < cnt; i++)
                {
                    ST_XYZA[][] PointArray = Utility.Array(tl[i], tr[i], bl[i], col, row);

                    for (int r = 0; r < PointArray.Length; r++)
                    {
                        for (int c = 0; c < PointArray[0].Length; c++)
                        {
                            if (i == 0)
                            {
                                PosInf p = new PosInf();
                                p.Pos[i] = PointArray[r][c];
                                p.Pos[i].z = tl[i].z;
                                p.Pos[i].a = tl[i].a;
                                p.Row = r;
                                p.Col = c;
                                p.idx = PointArray[0].Length * r + c;
                                p.isDisable = false;
                                p.isDisableTemp = false;
                                p.md = null;
                                list_pos.Add(p);
                            }
                            else
                            { 
                                if (list_pos.Count > 0)
                                {
                                    PosInf PI = list_pos.Find(delegate(PosInf x) { return (x.Col == c && x.Row == r);});
                                    if (PI != null)
                                    {
                                        PI.Pos[i] = PointArray[r][c];
                                        PI.Pos[i].z = tl[i].z;
                                        PI.Pos[i].a = tl[i].a;
                                    }
                                }
                            }
                           }                          
                      }
                   }               
            }
            public void CreateCapQrcodePos()
            {
                CreateCapQrcodePos(col, row, AddQrcodeTL, AddQrcodeBL, AddQrcodeTR);
            }
            public void CreateCapQrcodePos(int row, int col, ST_XYZA[] tl, ST_XYZA[] bl, ST_XYZA[] tr)
            {
                list_AddCapQrcodepos.Clear();
                for (int i = 0; i < cnt; i++)
                {
                    ST_XYZA[][] PointArray = Utility.Array(tl[i], tr[i], bl[i], col, row);

                    for (int r = 0; r < PointArray.Length; r++)
                    {
                        for (int c = 0; c < PointArray[0].Length; c++)
                        {
                            if (i == 0)
                            {
                                PosInf p = new PosInf();
                                p.Pos[i] = PointArray[r][c];
                                p.Pos[i].z = tl[i].z;
                                p.Pos[i].a = tl[i].a;
                                p.Row = r;
                                p.Col = c;
                                p.idx = PointArray[0].Length * r + c;
                                p.isDisable = false;
                                p.isDisableTemp = false;
                                p.md = null;
                                list_AddCapQrcodepos.Add(p);
                            }
                            else
                            {
                                if (list_AddCapQrcodepos.Count > 0)
                                {
                                    PosInf PI = list_AddCapQrcodepos.Find(delegate (PosInf x) { return (x.Col == c && x.Row == r); });
                                    if (PI != null)
                                    {
                                        PI.Pos[i] = PointArray[r][c];
                                        PI.Pos[i].z = tl[i].z;
                                        PI.Pos[i].a = tl[i].a;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region 参数存取
            public EM_RES LoadDat(string filename = "")
            {
                if (filename.Length < 3) filename = strCfgPath;

                if (filename.Length < 3) return EM_RES.PARA_ERR;
                if (!File.Exists(filename)) return EM_RES.PARA_ERR;

                IniFile inf = new IniFile(filename);

                //cofig
                string str_section = "TRAY";
                row = inf.ReadInteger(str_section, "ROW", 0);
                col = inf.ReadInteger(str_section, "COL", 0);
                for (int i = 0; i < cnt ; i++)
                {
                    tl[i].x = inf.ReadDouble(str_section, string.Format("TL_X[{0}]", i.ToString()), 0);
                    tl[i].y = inf.ReadDouble(str_section, string.Format("TL_Y[{0}]", i.ToString()), 0);
                    tl[i].z = inf.ReadDouble(str_section, string.Format("TL_Z[{0}]", i.ToString()), 0);
                    tl[i].a = inf.ReadDouble(str_section, string.Format("TL_A[{0}]", i.ToString()), 0);
 
                    tr[i].x = inf.ReadDouble(str_section, string.Format("TR_X[{0}]", i.ToString()), 0);
                    tr[i].y = inf.ReadDouble(str_section, string.Format("TR_Y[{0}]", i.ToString()), 0);
                    tr[i].z = inf.ReadDouble(str_section, string.Format("TR_Z[{0}]", i.ToString()), 0);
                    tr[i].a = inf.ReadDouble(str_section, string.Format("TR_A[{0}]", i.ToString()), 0);

                    bl[i].x = inf.ReadDouble(str_section, string.Format("BL_X[{0}]", i.ToString()), 0);
                    bl[i].y = inf.ReadDouble(str_section, string.Format("BL_Y[{0}]", i.ToString()), 0);
                    bl[i].z = inf.ReadDouble(str_section, string.Format("BL_Z[{0}]", i.ToString()), 0);
                    bl[i].a = inf.ReadDouble(str_section, string.Format("BL_A[{0}]", i.ToString()), 0);

                    AddQrcodeTL[i].x = inf.ReadDouble(str_section, string.Format("AddQrcodeTL_X[{0}]", i.ToString()), 0);
                    AddQrcodeTL[i].y = inf.ReadDouble(str_section, string.Format("AddQrcodeTL_Y[{0}]", i.ToString()), 0);
                    AddQrcodeTL[i].z = inf.ReadDouble(str_section, string.Format("AddQrcodeTL_Z[{0}]", i.ToString()), 0);
                    AddQrcodeTL[i].a = inf.ReadDouble(str_section, string.Format("AddQrcodeTL_A[{0}]", i.ToString()), 0);

                    AddQrcodeTR[i].x = inf.ReadDouble(str_section, string.Format("AddQrcodeTR_X[{0}]", i.ToString()), 0);
                    AddQrcodeTR[i].y = inf.ReadDouble(str_section, string.Format("AddQrcodeTR_Y[{0}]", i.ToString()), 0);
                    AddQrcodeTR[i].z = inf.ReadDouble(str_section, string.Format("AddQrcodeTR_Z[{0}]", i.ToString()), 0);
                    AddQrcodeTR[i].a = inf.ReadDouble(str_section, string.Format("AddQrcodeTR_A[{0}]", i.ToString()), 0);

                    AddQrcodeBL[i].x = inf.ReadDouble(str_section, string.Format("AddQrcodeBL_X[{0}]", i.ToString()), 0);
                    AddQrcodeBL[i].y = inf.ReadDouble(str_section, string.Format("AddQrcodeBL_Y[{0}]", i.ToString()), 0);
                    AddQrcodeBL[i].z = inf.ReadDouble(str_section, string.Format("AddQrcodeBL_Z[{0}]", i.ToString()), 0);
                    AddQrcodeBL[i].a = inf.ReadDouble(str_section, string.Format("AddQrcodeBL_A[{0}]", i.ToString()), 0);

                    TrayVsPos[i].x = inf.ReadDouble(str_section, string.Format("TRAY_VSPOS_X[{0}]", i.ToString()), 0);
                    TrayVsPos[i].y = inf.ReadDouble(str_section, string.Format("TRAY_VSPOS_Y[{0}]", i.ToString()), 0);
                    TrayVsPos[i].z = inf.ReadDouble(str_section, string.Format("TRAY_VSPOS_Z[{0}]", i.ToString()), 0);
                }

                //data
                list_pos.Clear();
                for (int n = 0; n < row * col; n++)
                {
                    str_section = string.Format("P{0}", n);
                    PosInf posInf = new PosInf();
                    for (int i = 0; i <cnt; i++)
                    {
                        posInf.Pos[i].x = inf.ReadDouble(str_section, string.Format("X[{0}]", i.ToString()), 0);
                        posInf.Pos[i].y = inf.ReadDouble(str_section, string.Format("Y[{0}]", i.ToString()), 0);
                        posInf.Pos[i].z = inf.ReadDouble(str_section, string.Format("Z[{0}]", i.ToString()), 0);
                        posInf.Pos[i].a = inf.ReadDouble(str_section, string.Format("A[{0}]", i.ToString()), 0);
                    }
                   

                    posInf.Col = inf.ReadInteger(str_section, "col", 0);
                    posInf.Row = inf.ReadInteger(str_section, "row", 0);
                    posInf.idx = inf.ReadInteger(str_section, "idx", 0);
                    posInf.isDisable = inf.ReadBool(str_section, "disable", false);
                    posInf.isDisableTemp = false;
                    list_pos.Add(posInf);
                }

                list_AddCapQrcodepos.Clear();
                for (int n = 0; n < row * col; n++)
                {
                    str_section = string.Format("P{0}", n);
                    PosInf posInf = new PosInf();
                    for (int i = 0; i < cnt; i++)
                    {
                        posInf.Pos[i].x = inf.ReadDouble(str_section, string.Format("CapQrcodePosX[{0}]", i.ToString()), 0);
                        posInf.Pos[i].y = inf.ReadDouble(str_section, string.Format("CapQrcodePosY[{0}]", i.ToString()), 0);
                        posInf.Pos[i].z = inf.ReadDouble(str_section, string.Format("CapQrcodePosZ[{0}]", i.ToString()), 0);
                        posInf.Pos[i].a = inf.ReadDouble(str_section, string.Format("CapQrcodePosA[{0}]", i.ToString()), 0);
                    }


                    posInf.Col = inf.ReadInteger(str_section, "col", 0);
                    posInf.Row = inf.ReadInteger(str_section, "row", 0);
                    posInf.idx = inf.ReadInteger(str_section, "idx", 0);
                    posInf.isDisable = inf.ReadBool(str_section, "disable", false);
                    posInf.isDisableTemp = false;
                    list_AddCapQrcodepos.Add(posInf);
                }
                return EM_RES.OK;
            }
            public EM_RES SaveDat(string filename = "")
            {
                EM_RES res = EM_RES.OK;
                if (filename.Length < 3) filename = strCfgPath;

                if (filename.Length < 3) return EM_RES.PARA_ERR;
                //if (!File.Exists(filename)) return EM_RES.PARA_ERR;
                string[] backup = File.ReadAllLines(filename);
                bool ischange = false;
                

                IniFile inf = new IniFile(filename);              
                //cofig
                string str_section = "TRAY";
                inf.WriteInteger(str_section, "ROW", row,ref ischange,true, filename);
                inf.WriteInteger(str_section, "COL", col, ref ischange, true, filename);
                for (int i = 0; i < cnt; i++)
                {
                    inf.WriteDouble(str_section, string.Format("TL_X[{0}]", i.ToString()), Math.Round(tl[i].x, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("TL_Y[{0}]", i.ToString()), Math.Round(tl[i].y, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("TL_Z[{0}]", i.ToString()), Math.Round(tl[i].z, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("TL_A[{0}]", i.ToString()), Math.Round(tl[i].a, 3), ref ischange, true, filename);

                    inf.WriteDouble(str_section, string.Format("TR_X[{0}]", i.ToString()), Math.Round(tr[i].x, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("TR_Y[{0}]", i.ToString()), Math.Round(tr[i].y, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("TR_Z[{0}]", i.ToString()), Math.Round(tr[i].z, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("TR_A[{0}]", i.ToString()), Math.Round(tr[i].a, 3), ref ischange, true, filename);
       
                    inf.WriteDouble(str_section, string.Format("BL_X[{0}]", i.ToString()), Math.Round(bl[i].x, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("BL_Y[{0}]", i.ToString()), Math.Round(bl[i].y, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("BL_Z[{0}]", i.ToString()), Math.Round(bl[i].z, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("BL_A[{0}]", i.ToString()), Math.Round(bl[i].a, 3), ref ischange, true, filename);

                    inf.WriteDouble(str_section, string.Format("AddQrcodeTL_X[{0}]", i.ToString()), Math.Round(AddQrcodeTL[i].x, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("AddQrcodeTL_Y[{0}]", i.ToString()), Math.Round(AddQrcodeTL[i].y, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("AddQrcodeTL_Z[{0}]", i.ToString()), Math.Round(AddQrcodeTL[i].z, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("AddQrcodeTL_A[{0}]", i.ToString()), Math.Round(AddQrcodeTL[i].a, 3), ref ischange, true, filename);

                    inf.WriteDouble(str_section, string.Format("AddQrcodeTR_X[{0}]", i.ToString()), Math.Round(AddQrcodeTR[i].x, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("AddQrcodeTR_Y[{0}]", i.ToString()), Math.Round(AddQrcodeTR[i].y, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("AddQrcodeTR_Z[{0}]", i.ToString()), Math.Round(AddQrcodeTR[i].z, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("AddQrcodeTR_A[{0}]", i.ToString()), Math.Round(AddQrcodeTR[i].a, 3), ref ischange, true, filename);

                    inf.WriteDouble(str_section, string.Format("AddQrcodeBL_X[{0}]", i.ToString()), Math.Round(AddQrcodeBL[i].x, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("AddQrcodeBL_Y[{0}]", i.ToString()), Math.Round(AddQrcodeBL[i].y, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("AddQrcodeBL_Z[{0}]", i.ToString()), Math.Round(AddQrcodeBL[i].z, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("AddQrcodeBL_A[{0}]", i.ToString()), Math.Round(AddQrcodeBL[i].a, 3), ref ischange, true, filename);

                    inf.WriteDouble(str_section, string.Format("TRAY_VSPOS_X[{0}]", i.ToString()), Math.Round(TrayVsPos[i].x, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("TRAY_VSPOS_Y[{0}]", i.ToString()), Math.Round(TrayVsPos[i].y, 3), ref ischange, true, filename);
                    inf.WriteDouble(str_section, string.Format("TRAY_VSPOS_Z[{0}]", i.ToString()), Math.Round(TrayVsPos[i].z, 3), ref ischange, true, filename);
                }

                //data
                PosInf posInf = new PosInf();
                PosInf addCapQrcodePosInf = new PosInf();
                for (int n = 0; n < row * col; n++)
                {
                    str_section = string.Format("P{0}", n);
                    if (n < list_pos.Count) posInf = list_pos.ElementAt(n);
                    else posInf = new PosInf();
                    if (n < list_AddCapQrcodepos.Count) addCapQrcodePosInf = list_AddCapQrcodepos.ElementAt(n);
                    else addCapQrcodePosInf = new PosInf();
                    inf.WriteInteger(str_section, "col", posInf.Col, ref ischange, true, filename);
                    inf.WriteInteger(str_section, "row", posInf.Row, ref ischange, true, filename);
                    inf.WriteInteger(str_section, "idx", posInf.idx, ref ischange, true, filename);
                    inf.WriteBool(str_section, "disable", posInf.isDisable, ref ischange, true, filename);
                    for (int i = 0; i < cnt; i++)
                    {
                        inf.WriteDouble(str_section, string.Format("X[{0}]", i.ToString()), Math.Round(posInf.Pos[i].x, 3), ref ischange, true, filename);
                        inf.WriteDouble(str_section, string.Format("Y[{0}]", i.ToString()), Math.Round(posInf.Pos[i].y, 3), ref ischange, true, filename);
                        inf.WriteDouble(str_section, string.Format("Z[{0}]", i.ToString()), Math.Round(posInf.Pos[i].z, 3), ref ischange, true, filename);
                        inf.WriteDouble(str_section, string.Format("A[{0}]", i.ToString()), Math.Round(posInf.Pos[i].a, 3), ref ischange, true, filename);

                    }
                    for (int i = 0; i < cnt; i++)
                    {
                        inf.WriteDouble(str_section, string.Format("CapQrcodePosX[{0}]", i.ToString()), Math.Round(addCapQrcodePosInf.Pos[i].x, 3), ref ischange, true, filename);
                        inf.WriteDouble(str_section, string.Format("CapQrcodePosY[{0}]", i.ToString()), Math.Round(addCapQrcodePosInf.Pos[i].y, 3), ref ischange, true, filename);
                        inf.WriteDouble(str_section, string.Format("CapQrcodePosZ[{0}]", i.ToString()), Math.Round(addCapQrcodePosInf.Pos[i].z, 3), ref ischange, true, filename);
                        inf.WriteDouble(str_section, string.Format("CapQrcodePosA[{0}]", i.ToString()), Math.Round(addCapQrcodePosInf.Pos[i].a, 3), ref ischange, true, filename);

                    }

                }
                for (int n = row * col; n < 20 * 20; n++)
                {
                    str_section = string.Format("P{0}", n);
                    inf.WriteString(str_section, null, null, ref ischange, false);
                }

                if (ischange)
                {
                    //创建backup
                    //第一层
                    string backup_filename = string.Format("{0}\\product\\{1}\\backup", Path.GetFullPath(".."), VAR.gsys_set.cur_product_name);
                    res = SYS_PUD.CopyFile2(backup_filename);
                    if (res != EM_RES.OK) return res;
                    //第二层
                    backup_filename = backup_filename + "\\TrayBoxCfg";
                    res = SYS_PUD.CopyFile2(backup_filename);
                    if (res != EM_RES.OK) return res;
                    //文件
                    string[] str = filename.Split('\\');
                    res = SYS_PUD.FileWriteLine(string.Format(str[str.Length - 1]),backup, backup_filename);
                    if (res != EM_RES.OK) return res;
                }
                return EM_RES.OK;
            }
            #endregion
            #region 统计
            //public int count_md (Tray tray)
            //{
            //    get
            //    {
            //        if(tray.)
            //        return list_pos.Count(s => { return s.md != null && s.md.res == 0 && !s.isDisable; });
            //    }
            //}

            public int count_md_ok
            {
                get
                {
                    return list_pos.Count(s => { return s.md != null && s.md.res == 0 && !s.isDisable && !s.isDisableTemp; });
                }
            }
            public int count_md_ng
            {
                get
                {
                    return list_pos.Count(s => { return s.md != null && s.md.res > 0 && !s.isDisable && !s.isDisableTemp; });
                }
            }
            public int count_md_untest
            {
                get
                {
                    return list_pos.Count(s => { return s.md != null && s.md.res == -1 && !s.isDisable && !s.isDisableTemp; });
                }
            }
            public int count_pos_empty
            {
                get
                {
                    return list_pos.Count(s => { return s.md == null; });
                }
            }
            public int count_pos_all
            {
                get
                {
                    return list_pos.Count(s => { return (!s.isDisable && !s.isDisableTemp); });
                }
            }
            public int count_pos_disable
            {
                get
                {
                    return list_pos.Count(s => { return (s.isDisable || s.isDisableTemp); });
                }
            }
            #endregion
            #region 提取模组
            /// <summary>
            /// 提取指定状态，指定位置的模组
            /// </summary>
            /// <param name="md">接受返回模组</param>
            /// <param name="idx">指定位置，默认不指定</param>
            /// <param name="res">指定状态，默认待测模组</param>
            /// <returns></returns>
            public EM_RES Pull(ref MdDat md, int idx = -1, EM_CM_RES res = EM_CM_RES.UNTEST)
            {
                PosInf posInf = list_pos.Find(s =>
                {
                    if (idx == -1 || s.idx == idx)
                    {
                        if (s.md != null && s.md.res == (int)res) return true;
                    }
                    return false;
                });

                if (posInf != null)
                {
                    md = posInf.md.Clone();
                    posInf.md = null;
                }
                else
                {
                    md = null;
                    return EM_RES.END;
                }

                return EM_RES.OK;
            }
            #endregion
            #region 放置模组
            /// <summary>
            /// 放置模组到指定位置
            /// </summary>
            /// <param name="md">要放置的模组</param>
            /// <param name="idx">指定位置</param>
            /// <returns></returns>
            public EM_RES Push(MdDat md,int idx = -1,int Modid=0)
            {
                //获取空位置
                List<PosInf> ls_pos = GetPosList(EM_CM_RES.EMPTY, md.res);
                if (ls_pos == null || ls_pos.Count == 0) return EM_RES.PARA_OUTOFRANG;
                //如果指定位置
                if (idx != -1)
                {
                    idx = ls_pos.FindIndex(s => { return s.idx == idx; });
                }
                else idx = 0;
                if(idx <0 || idx >= ls_pos.Count) return EM_RES.PARA_OUTOFRANG;

                //push
                md.idx = idx;
                md.pos = ls_pos.ElementAt(idx).Pos[Modid];
                md.tray_barcode = barcode;
                ls_pos.ElementAt(idx).md = md.Clone();

                //最后一个返回END
                if (list_pos.Count == 1) return EM_RES.END;
                else return EM_RES.OK;
            }
            #endregion
            #region 状态
            public List<PosInf> GetPosList(EM_CM_RES res = EM_CM_RES.UNTEST, int ngcode = -10000)
            {
                //NG分区
                List<int> ls_idx = new List<int>();
                List<int> ls_all_idx = new List<int>();
                if (ngcode != 0 && ngcode != -10000 && NGDef != null)
                {
                    ls_idx = NGDef.GetPosIdxListByNGCode(ngcode);
                    if (ls_idx.Count == 0) ls_all_idx = NGDef.GetPosIdxListDefByNGCode();
                }

                return list_pos.FindAll(s =>
                {
                if (s.isDisable||s.isDisableTemp) return false;

                switch (res)
                {
                    //视觉NG
                    case EM_CM_RES.CAMERR:
                        if (s.md != null && s.md.res == (int)EM_CM_RES.CAMERR) return true;
                            break;
                    //空位
                    case EM_CM_RES.EMPTY:
                            if (s.md == null)
                            {
                                //分区
                                if (ls_idx.Count != 0)
                                {
                                    if (ls_idx.Contains(s.idx)) return true;
                                }
                                //未被定义分区
                                else if (ls_all_idx.Count != 0)
                                {
                                   if(!ls_all_idx.Contains(s.idx)) return true;
                                   
                                }
                                else return true;
                            }
                            break;
                        //待测
                        case EM_CM_RES.UNTEST:
                            if (s.md != null && s.md.res == (int)EM_CM_RES.UNTEST) return true;
                            break;
                        //OK
                        case EM_CM_RES.OK:
                            if (s.md != null && s.md.res == (int)EM_CM_RES.OK) return true;
                            break;
                        //NG
                        case EM_CM_RES.NG:
                            if (s.md != null && s.md.res > 0)
                            {
                                //指定NG码
                                if (ngcode != -10000)
                                {
                                    if (s.md.res == ngcode)
                                        return true;
                                }
                                else return true;
                            }
                            break;
                    }
                    return false;
                });
            }
            #endregion
        }

        #region  加载
        public EM_RES LoadDat(string name ="")
        {
            if (name.Length < 3) name = this.name;
            else this.name = name;

            string filename= Path.GetFullPath("..") + "\\product\\" + name + "\\cfg.ini";
            if (!File.Exists(filename)) return EM_RES.PARA_ERR;

            IniFile inf = new IniFile(filename);
            return EM_RES.OK;
        }
        #endregion
        #region  保存
        public void SaveDat()
        {
        }
        #endregion
        #region 加载产品list
        //加载产品列表，列表里无当前产品时返回true
        public bool LoadProductList(System.Windows.Forms.ComboBox cb, string cur_product = "")
        {   
            cb.Items.Clear();
            DirectoryInfo dir = new DirectoryInfo(Path.GetFullPath("..") + "\\product\\");
            foreach (DirectoryInfo dChild in dir.GetDirectories("*"))
            {
                if (dChild.Name.Length > 2)
                    cb.Items.Add(dChild.Name.ToString());
            }
            int idx = cb.Items.IndexOf(cur_product);
            if (idx < 0) idx = 0;
            cb.SelectedIndex = idx;

            if(cur_product != cb.SelectedItem.ToString()) return true;
            else return false;
        }
        public bool LoadProductList(System.Windows.Forms.ListBox listbox, string cur_product = "")
        {
            listbox.Items.Clear();
            DirectoryInfo dir = new DirectoryInfo(Path.GetFullPath("..") + "\\product\\");
            foreach (DirectoryInfo dChild in dir.GetDirectories("*"))
            {
                if (dChild.Name.Length > 2)
                    listbox.Items.Add(dChild.Name.ToString());
            }
            int idx = listbox.Items.IndexOf(cur_product);
            if (idx < 0) idx = 0;
            listbox.SelectedIndex = idx;

            if (cur_product != listbox.SelectedItem.ToString()) return true;
            else return false;
        }
        #endregion
    }
}
