using System;
using System.IO;
using System.Threading;
using MotionCtrl;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using DevReport;
using UI.Class;
using static UI.Cam;
using Sunny.UI;
using IniFile = MotionCtrl.IniFile;

namespace UI
{
    public  class XT
    {
        public bool Enable = true;
        int PickTrayChkErrCnt = 0;//取料后复检失败次数用于报警显示
        int PickTrayChkErrCntLimit = 7;//取料后复检失败次数用于报警显示
        #region 硬件
        /// <summary>
        ///吸头对应下相机
        /// </summary>
        public Cam dwcam = null;
        /// <summary>
        ///吸头对应上相机
        /// </summary>
        public Cam upcam = null;
        /// <summary>
        ///  吸头对应的上料仓储
        /// </summary>
        public TrayBox traybox_fd  = COM.traybox_fd;
        /// <summary>
        ///  吸头对应的NG仓储
        /// </summary>
        public TrayBox traybox_ok = COM.traybox_ok;
        /// <summary>
        ///  吸头对应的NG仓储
        /// </summary>
        public TrayBox traybox_ng = COM.traybox_ng;
        /// <summary>
        /// 上料X轴
        /// </summary>
        public AXIS ax_x = null;
        /// <summary>
        /// 上料Y轴
        /// </summary>
        public  AXIS ax_y = null;
        /// <summary>
        /// 上料Z轴
        /// </summary>
        public  AXIS ax_z = null;
        /// <summary>
        /// 上料U轴
        /// </summary>
        public  AXIS ax_u = null;
        /// <summary>
        /// 吸头气缸
        /// </summary>
        public Cylinder cy_zk = null;
        /// <summary>
        /// 吸头破真空
        /// </summary>
        public  GPIO gpio_pzk = null;

      
        /// <summary>
        /// 可以马达扫二维码
        /// </summary>
        public bool bCanMotoScan =>PT_SET.bmotorphoto&&XtMd!=null;
        #endregion

        #region 枚举--(拍照流程)
        /// <summary>
        /// 拍照流程
        /// </summary>
        public enum EM_XTFLOW
        {
            [Description("取模组")]
            PICKMOD,
            [Description("放模组")]
            PLACEMOD,
        }
        #endregion

        #region 吸头参数
        public int id;                  //吸头id
        public bool bDemo;
        public UpDownLoad Parent;
        public const ushort OFS_NUM_PER_XT = 2;
        public EM_XTFLOW xt_cam_flow = EM_XTFLOW.PICKMOD;
        public double cap_spd;          //拍照速度
        public ST_XYZ st_cap_pos;   //拍照坐标(下相机)
        public ST_XYZA xt_pos_pick_mod;//取料位置
        public double xt_pos_pick_tray_x;//取料料盘位置
        public double[] fly_offset = new double[CONST.FLY_COUNT];       //飞拍偏移
        public Product.MdDat XtMd = new Product.MdDat();
        //视觉校正参数
        //public  ST_XYZ st_pos_unit_pick;     //单位校准，吸头取料位置
        public ST_XYZ st_pos_unit_place;    //单位校准，吸头放料位
        public ST_XYZ st_pos_unit_upcam_cap;//单位校准，上相机拍照位
        public ST_XY[] st_pos_samevs = new ST_XY[2];//视觉同一位置下两个模块的坐标
        public double pos_unit_upcam_step;  //单位校准步长
        public ST_XYA st_vs_pos_xtshp = new ST_XYA();      //空吸头拍照坐标

        public bool bAdj_UpCam = false;
        public bool bAdj_DwCam = false;

        public ST_XY st_pos_affine_upcam_cap;  //仿射校准，上相机拍照位
        public ST_XY st_pos_affine_dwcam_cap;  //仿射校准，下相机拍照位
        public ST_XYZA st_pos_affine_place;      //仿射校准，吸头放料位置
        //public ST_XY st_pos_affine_dwtoup_cam1_cap;    //仿射校准，上相机拍照位(下相机->上相机1)
        //public ST_XY st_pos_affine_uptoup_cam1_cap;    //仿射校准，上相机拍照位(上相机2->上相机1)
        //public  ST_XYZ st_pos_affine_downcam_cap;  //仿射校准，下相机拍照位
        //offset 数据
        public ST_XYA[] vs_offset = new ST_XYA[OFS_NUM_PER_XT];
        public double[] vs_up_ref_angle = new double[OFS_NUM_PER_XT];
        public double[] vs_down_ref_angle = new double[OFS_NUM_PER_XT];
        //旋转中心
        public double rol_cap_befrcali;     //旋转校正前位置
        public ST_XYZ st_rol_cap;      //拍照位置
        public ST_XYZ st_rol;           //旋转中心 
       
        public double cap_offset; //拍照偏移
        public double DwCapQrCodeoffset; //拍二维码偏移

        public double xt_cap_ofs;//吸头飞拍OFS
        public double ws_cap_near_ofs; //工站近端飞拍偏差
        public double ws_cap_far_ofs; //工站远端飞拍偏差


        #endregion

        #region 描述符
        public string disc
        {
            get
            {
                return VAR.IsChinese?string.Format("吸头{0}", id + 1): string.Format("XT{0}", id + 1);
            }
        }
        #endregion

        #region 初始化
        public XT()
        { }
        public XT(int xt_id, ref AXIS ax_x, ref AXIS ax_y, ref AXIS ax_z, ref AXIS ax_u,ref Cam upcam,ref Cam dwcam, Cylinder cy_zk, GPIO gpio_pzk)
        {
            id = xt_id;
            this.ax_x = ax_x;
            this.ax_y = ax_y;
            this.ax_z = ax_z;
            this.ax_u = ax_u;
            this.upcam = upcam;
            this.dwcam = dwcam;
            this.cy_zk = cy_zk;
            this.gpio_pzk = gpio_pzk;
            XtMd = null;
        }
        #endregion

        #region 参数存取
        public EM_RES LoadCfg(string productname)
        {
            //check
            if (productname.Length < 3)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese?string.Format("{0}初始化对应产品名{1}异常(<3个字符)!", disc, productname): string.Format("{0} Initialize the corresponding product name ({1}) err:Less than three characters!          ({0}初始化对应产品名{1}异常(<3个字符)!)", disc, productname), DReport.EmErrCode.GetParamError, (int)DReport.EmHareware.Nozzle +id+1);
                return EM_RES.PARA_ERR;
            }

            //产品参数
            string filename = string.Format("{0}\\product\\{1}\\xtcfg.ini", Path.GetFullPath(".."), productname);

            if (!File.Exists(filename))
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}初始化对应产品名{1}配置文件不存在!", disc, productname): string.Format("{0} Initialize the corresponding product name ({1}),the configuration file does not exist!          ({0}初始化对应产品名{1}配置文件不存在!)", disc, productname), DReport.EmErrCode.GetParamError, (int)DReport.EmHareware.Nozzle + id + 1);
                return EM_RES.PARA_ERR;
            }

            IniFile inf = new IniFile(filename);
            string section = string.Format("XT{0}", id);

            //贴附offset 数据
            for(int i=0;i<OFS_NUM_PER_XT;i++)
            {
                vs_offset[i].x = inf.ReadDouble(section, "VS_OFS_X_" + i.ToString(), 0);
                vs_offset[i].y = inf.ReadDouble(section, "VS_OFS_Y_" + i.ToString(), 0);
                vs_offset[i].a = inf.ReadDouble(section, "VS_OFS_A_" + i.ToString(), 0);

                vs_down_ref_angle[i] = inf.ReadDouble(section, "VS_DOWN_REF_ANGLE_" + i.ToString(), 0);
                vs_up_ref_angle[i] = inf.ReadDouble(section, "VS_UP_REF_ANGLE_" + i.ToString(), 0);
            }

            //空吸头拍照坐标
            st_vs_pos_xtshp.x = inf.ReadDouble(section, "VSPOS_XT_SHP_X", 0);
            st_vs_pos_xtshp.y = inf.ReadDouble(section, "VSPOS_XT_SHP_Y", 0);
            st_vs_pos_xtshp.a = inf.ReadDouble(section, "VSPOS_XT_SHP_A", 0);

            //硬件参数
            filename = Path.GetFullPath("..") + "\\syscfg\\syscfg.ini";
            inf = new IniFile(filename);

            //飞拍速度 
            cap_spd = inf.ReadDouble(section, "CAP_SPD", 0);
            //飞拍偏移补偿
            for (int i = 0; i < CONST.FLY_COUNT; i++)
            {
                fly_offset[i] = inf.ReadDouble(section, "FLY_OFS" + i.ToString(), 0);
            }
            //单位校准 吸头1取料位置
            //st_pos_unit_pick.x = inf.ReadDouble(section, "UNIT_PICK_X", 0.000);
            //st_pos_unit_pick.y = inf.ReadDouble(section, "UNIT_PICK_Y", 0.000);
            //st_pos_unit_pick.z = inf.ReadDouble(section, "UNIT_PICK_Z", 0.000);
            //单位校准 吸头1放料位置
            st_pos_unit_place.x = inf.ReadDouble(section, "UNIT_PLACE_X", 0.000);
            st_pos_unit_place.y = inf.ReadDouble(section, "UNIT_PLACE_Y", 0.000);
            st_pos_unit_place.z = inf.ReadDouble(section, "UNIT_PLACE_Z", 0.000);
            //单位校准 上相机拍照位
            st_pos_unit_upcam_cap.x = inf.ReadDouble(section, "UNIT_UPCAM_X", 0.000);
            st_pos_unit_upcam_cap.y = inf.ReadDouble(section, "UNIT_UPCAM_Y", 0.000);
            st_pos_unit_upcam_cap.z = 200;// inf.ReadDouble(section, "UNIT_UPCAM_Z", 0.000);
            //视觉同一位置两个模块的坐标
            for (int i= 0; i <2; i++)
            {
                st_pos_samevs[i].x = inf.ReadDouble(section, "SAMEVS_X" + i.ToString(), 0.000);
                st_pos_samevs[i].y = inf.ReadDouble(section, "SAMEVS_Y" + i.ToString(), 0.000);
            }
            //单位校准步长
            pos_unit_upcam_step = inf.ReadDouble(section, "UNIT_STEP", 0.000);
            //仿射校准，上相机的拍照位置
            st_pos_affine_upcam_cap.x = inf.ReadDouble(section, "AFFINE_UPCAM_X", 0.000);
            st_pos_affine_upcam_cap.y = inf.ReadDouble(section, "AFFINE_UPCAM_Y", 0.000);
            //仿射校准，下相机的拍照位置
            st_pos_affine_dwcam_cap.x = inf.ReadDouble(section, "AFFINE_DWCAM_X", 0.000);
            st_pos_affine_dwcam_cap.y = inf.ReadDouble(section, "AFFINE_DWCAM_Y", 0.000);
            //仿射校准，吸头放料位置(相机相同共用)
            st_pos_affine_place.x = inf.ReadDouble(section, "AFFINE_PLACE_X", 0.000);
            st_pos_affine_place.y = inf.ReadDouble(section, "AFFINE_PLACE_Y", 0.000);
            st_pos_affine_place.z = inf.ReadDouble(section, "AFFINE_PLACE_Z", 0.000);
            st_pos_affine_place.a = inf.ReadDouble(section, "AFFINE_PLACE_A", 0.000);
            ////仿射校准，上相机1拍照位(下相机->上相机1)
            //st_pos_affine_dwtoup_cam1_cap.x = inf.ReadDouble(section, "AFFINE_DWTOUP_CAM1_X", 0.000);
            //st_pos_affine_dwtoup_cam1_cap.y = inf.ReadDouble(section, "AFFINE_DWTOUP_CAM1_Y", 0.000);
            ////仿射校准，上相机1拍照位(上相机2->上相机1)
            //st_pos_affine_uptoup_cam1_cap.x = inf.ReadDouble(section, "AFFINE_UPTOUP_CAM1_X", 0.000);
            //st_pos_affine_uptoup_cam1_cap.y = inf.ReadDouble(section, "AFFINE_UPTOUP_CAM1_Y", 0.000);
            ////仿射校准，下相机拍照位(相机相同共用)
            //st_pos_affine_downcam_cap.x = inf.ReadDouble(section, "AFFINE_DOWNCAM_X", 0.000);
            //st_pos_affine_downcam_cap.y = inf.ReadDouble(section, "AFFINE_DOWNCAM_Y", 0.000);
            //st_pos_affine_downcam_cap.z = inf.ReadDouble(section, "AFFINE_DOWNCAM_Z", 0.000);
            //旋转中心校正前拍照学习的拍照位置
            rol_cap_befrcali = inf.ReadDouble(section, "ROC_CAP_BEFRCAIL_Y", 0.000);
            //下相机拍照位置
            st_rol_cap.x = inf.ReadDouble(section, "ROL_CAP_X", 0);
            st_rol_cap.y = inf.ReadDouble(section, "ROL_CAP_Y", 0);
            //旋转中心 
            st_rol.x = inf.ReadDouble(section, "ROL_X", 0);
            st_rol.y = inf.ReadDouble(section, "ROL_Y", 0);
            st_rol.z = inf.ReadDouble(section, "ROL_Z", 0);

            cap_offset = inf.ReadDouble(section, "CAP_OFS", 0);
            DwCapQrCodeoffset = inf.ReadDouble(section, "DwCapQrCodeoffset", 0);
           

            //屏蔽功能
            Enable = inf.ReadBool(section, "ENABLE", true);
            //飞拍位置
            st_cap_pos.y = st_rol_cap.y+ cap_offset;
            ax_y.spd_cap = cap_spd;

            
            //飞拍偏差
            xt_cap_ofs = inf.ReadDouble(section, "XT_CAP_OFS", 0);
            ws_cap_near_ofs = inf.ReadDouble(section, "WS_CAP_NEAR_OFS", 0);
            ws_cap_far_ofs = inf.ReadDouble(section, "WS_CAP_FAR_OFS", 0);
            return EM_RES.OK;
        }

        public EM_RES LoadOrSaveCapOfsCfg(bool bSave=true)
        {
            //硬件参数
            string filename = Path.GetFullPath("..") + "\\syscfg\\syscfg.ini";
            IniFile inf = new IniFile(filename);
            string section = string.Format("XT{0}", id);
            if (bSave)
            {
                string[] backup = File.ReadAllLines(filename);
                bool ischange = false;
                
                inf.WriteDouble(section, "CAP_OFS", cap_offset,ref ischange, true, filename);
                inf.WriteDouble(section, "DwCapQrCodeoffset", DwCapQrCodeoffset, ref ischange, true, filename);
                if (ischange)
                {
                    //创建backup
                    //第一层
                    string backup_filename = string.Format("{0}\\syscfg\\backup", Path.GetFullPath(".."));
                    SYS_PUD.CopyFile2(backup_filename);
                    //文件
                    string[] str = filename.Split('\\');
                    SYS_PUD.FileWriteLine(string.Format(str[str.Length - 1]),backup, backup_filename);
                }
            }
            else
            {
                cap_offset = inf.ReadDouble(section, "CAP_OFS", 0);
                DwCapQrCodeoffset = inf.ReadDouble(section, "DwCapQrCodeoffset", 0);
                //飞拍位置
                st_cap_pos.y = st_rol_cap.y+ cap_offset;
            }
            return EM_RES.OK;
        }

        public EM_RES SaveCfg(string productname)
        {
            EM_RES res = EM_RES.OK;
            //check
            if (productname.Length < 3)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese?string.Format("{0}初始化对应产品名{1}异常(<3个字符)!", disc, productname) : string.Format("{0} Initialize the corresponding product name ({1}) err:Less than three characters!          ({0}初始化对应产品名{1}异常(<3个字符)!)", disc, productname), DReport.EmErrCode.GetParamError, (int)DReport.EmHareware.Nozzle + id + 1);
                return  EM_RES.PARA_ERR;
            }

            //产品参数
            string filename = string.Format("{0}\\product\\{1}\\xtcfg.ini", Path.GetFullPath(".."), productname);
            string[] backup = File.ReadAllLines(filename);
            bool ischange = false;
            
            IniFile inf = new IniFile(filename);
            string section = string.Format("XT{0}", id);
            inf.WriteString(section, "DISC", disc,ref ischange,false);
            //空吸头拍照坐标
            inf.WriteDouble(section, "VSPOS_XT_SHP_X", st_vs_pos_xtshp.x, ref ischange, true, filename);
            inf.WriteDouble(section, "VSPOS_XT_SHP_Y", st_vs_pos_xtshp.y, ref ischange, true, filename);
            inf.WriteDouble(section, "VSPOS_XT_SHP_A", st_vs_pos_xtshp.a, ref ischange, true, filename);
            if (ischange)
            {
                //创建backup
                //第一层
                string backup_filename = string.Format("{0}\\product\\{1}\\backup", Path.GetFullPath(".."), productname);
                res = SYS_PUD.CopyFile2(backup_filename);
                if (res != EM_RES.OK) return res;
                //文件
                string[] str = filename.Split('\\');
                res = SYS_PUD.FileWriteLine(string.Format(str[str.Length - 1]),backup, backup_filename);
                if (res != EM_RES.OK) return res;
            }
            return EM_RES.OK;
        }
        public void SaveHwCfg(string filename = "")
        {
            //硬件参数
            //if(filename.Length<3)
            filename = Path.GetFullPath("..") + "\\syscfg\\syscfg.ini";
            string[] backup = File.ReadAllLines(filename);
            bool ischange = false;
           
            IniFile inf = new IniFile(filename);
            string section = string.Format("XT{0}", id);
            inf.WriteString(section, "DISC", disc,ref ischange,false);

            string str_xt = string.Format("[{0}]", section);


            //贴附offset 数据
            for (int i = 0; i < OFS_NUM_PER_XT;i++ )
            {
                inf.WriteDouble(section, "VS_OFS_X_" + i.ToString(), vs_offset[i].x,ref ischange, true, filename);
                inf.WriteDouble(section, "VS_OFS_Y_" + i.ToString(), vs_offset[i].y, ref ischange, true, filename);
                inf.WriteDouble(section, "VS_OFS_A_" + i.ToString(), vs_offset[i].a, ref ischange, true, filename);

                inf.WriteDouble(section, "VS_DOWN_REF_ANGLE_" + i.ToString(), vs_down_ref_angle[i], ref ischange, true, filename);
                inf.WriteDouble(section, "VS_UP_REF_ANGLE_" + i.ToString(), vs_up_ref_angle[i], ref ischange, true, filename);
            }
              

            //飞拍速度 
            inf.WriteDouble(section, "CAP_SPD", cap_spd, ref ischange, true, filename);
            //飞拍偏移补偿  
            for (int i = 0; i < CONST.FLY_COUNT; i++)
            {
                inf.WriteDouble(section, "FLY_OFS" + i.ToString(), fly_offset[i], ref ischange, true, filename);
            }
            //单位校准 吸头1取料位置
            //inf.WriteDouble(section, "UNIT_PICK_X", st_pos_unit_pick.x);
            //inf.WriteDouble(section, "UNIT_PICK_Y", st_pos_unit_pick.y);
            //inf.WriteDouble(section, "UNIT_PICK_Z", st_pos_unit_pick.z);
            //单位校准 吸头1放料位置
            inf.WriteDouble(section, "UNIT_PLACE_X", st_pos_unit_place.x, ref ischange, true, filename);
            inf.WriteDouble(section, "UNIT_PLACE_Y", st_pos_unit_place.y, ref ischange, true, filename);
            inf.WriteDouble(section, "UNIT_PLACE_Z", st_pos_unit_place.z, ref ischange, true, filename);
            //单位校准 上相机拍照位(对中)
            inf.WriteDouble(section, "UNIT_UPCAM_X", st_pos_unit_upcam_cap.x, ref ischange, true, filename);
            inf.WriteDouble(section, "UNIT_UPCAM_Y", st_pos_unit_upcam_cap.y, ref ischange, true, filename);
            inf.WriteDouble(section, "UNIT_UPCAM_Z", st_pos_unit_upcam_cap.z, ref ischange, true, filename);
            //视觉同一位置两个模块的坐标
            for (int i = 0; i < 2; i++)
            {
                inf.WriteDouble(section, "SAMEVS_X" + i.ToString(), st_pos_samevs[i].x, ref ischange, true, filename);
                inf.WriteDouble(section, "SAMEVS_Y" + i.ToString(), st_pos_samevs[i].y, ref ischange, true, filename);
            }
            //单位校准步长
            inf.WriteDouble(section, "UNIT_STEP", pos_unit_upcam_step, ref ischange, true, filename);
            //上相机仿射校准，拍照位置
            inf.WriteDouble(section, "AFFINE_UPCAM_X", st_pos_affine_upcam_cap.x, ref ischange, true, filename);
            inf.WriteDouble(section, "AFFINE_UPCAM_Y", st_pos_affine_upcam_cap.y, ref ischange, true, filename);
            //下相机仿射校准，拍照位置
            inf.WriteDouble(section, "AFFINE_DWCAM_X", st_pos_affine_dwcam_cap.x, ref ischange, true, filename);
            inf.WriteDouble(section, "AFFINE_DWCAM_Y", st_pos_affine_dwcam_cap.y, ref ischange, true, filename);
            //吸头放料位置(相机相同共用)
            inf.WriteDouble(section, "AFFINE_PLACE_X", st_pos_affine_place.x, ref ischange, true, filename);
            inf.WriteDouble(section, "AFFINE_PLACE_Y", st_pos_affine_place.y, ref ischange, true, filename);
            inf.WriteDouble(section, "AFFINE_PLACE_Z", st_pos_affine_place.z, ref ischange, true, filename);
            inf.WriteDouble(section, "AFFINE_PLACE_A", st_pos_affine_place.a, ref ischange, true, filename);
            ////仿射校准，上相机1拍照位(下相机->上相机1)
            //inf.WriteDouble(section, "AFFINE_DWTOUP_CAM1_X",st_pos_affine_dwtoup_cam1_cap.x);
            //inf.WriteDouble(section, "AFFINE_DWTOUP_CAM1_Y",st_pos_affine_dwtoup_cam1_cap.y);
            ////仿射校准，上相机1拍照位(上相机2->上相机1)
            //inf.WriteDouble(section, "AFFINE_UPTOUP_CAM1_X", st_pos_affine_uptoup_cam1_cap.x);
            //inf.WriteDouble(section, "AFFINE_UPTOUP_CAM1_Y", st_pos_affine_uptoup_cam1_cap.y);
            ////仿射校准，下相机拍照位(相机相同共用)
            //inf.WriteDouble(section, "AFFINE_DOWNCAM_X", st_pos_affine_downcam_cap.x);
            //inf.WriteDouble(section, "AFFINE_DOWNCAM_Y", st_pos_affine_downcam_cap.y);
            //inf.WriteDouble(section, "AFFINE_DOWNCAM_Z", st_pos_affine_downcam_cap.z);
            //旋转中心校正前拍照学习的拍照位置
            inf.WriteDouble(section, "ROC_CAP_BEFRCAIL_Y", rol_cap_befrcali, ref ischange, true, filename);
            //旋转中心拍照位置
            inf.WriteDouble(section, "ROL_CAP_X", st_rol_cap.x, ref ischange, true, filename);
            inf.WriteDouble(section, "ROL_CAP_Y", st_rol_cap.y, ref ischange, true, filename);
            //旋转中心 
            inf.WriteDouble(section, "ROL_X", st_rol.x, ref ischange, true, filename);
            inf.WriteDouble(section, "ROL_Y", st_rol.y, ref ischange, true, filename);
            
            inf.WriteDouble(section, "ROL_Z", st_rol.z, ref ischange, true, filename);
            //拍照偏差
            inf.WriteDouble(section, "DwCapQrCodeoffset", DwCapQrCodeoffset, ref ischange, true, filename);
            inf.WriteDouble(section, "CAP_OFS", cap_offset, ref ischange, true, filename);
            //飞拍偏差
            inf.WriteDouble(section, "XT_CAP_OFS", xt_cap_ofs, ref ischange, true, filename);
            inf.WriteDouble(section, "WS_CAP_NEAR_OFS", ws_cap_near_ofs, ref ischange, true, filename);
            inf.WriteDouble(section, "WS_CAP_FAR_OFS", ws_cap_far_ofs, ref ischange, true, filename);

            //屏蔽功能
             inf.WriteBool(section, "ENABLE", Enable, ref ischange, true, filename);
            if (ischange)
            {
                //创建backup
                //第一层
                string backup_filename = string.Format("{0}\\syscfg\\backup", Path.GetFullPath(".."));
                SYS_PUD.CopyFile2(backup_filename);
                //文件
                string[] str = filename.Split('\\');
                SYS_PUD.FileWriteLine(string.Format(str[str.Length - 1]),backup, backup_filename);
            }
        }
        public void SaveOffset(string productname)
        {
            string filename = string.Format("{0}\\product\\{1}\\xtcfg.ini", Path.GetFullPath(".."), productname);
            string[] backup = File.ReadAllLines(filename);
            bool ischange = false;
            
            IniFile inf = new IniFile(filename);
            string section = string.Format("XT{0}", id);
            inf.WriteString(section, "DISC", disc, ref ischange, false);

            //贴附offset 数据
            for (int i = 0; i < OFS_NUM_PER_XT; i++)
            {
                inf.WriteDouble(section, "VS_OFS_X_" + i.ToString(), vs_offset[i].x, ref ischange, true, filename);
                inf.WriteDouble(section, "VS_OFS_Y_" + i.ToString(), vs_offset[i].y, ref ischange, true, filename);
                inf.WriteDouble(section, "VS_OFS_A_" + i.ToString(), vs_offset[i].a, ref ischange, true, filename);

                inf.WriteDouble(section, "VS_DOWN_REF_ANGLE_" + i.ToString(), vs_down_ref_angle[i], ref ischange, true, filename);
                inf.WriteDouble(section, "VS_UP_REF_ANGLE_" + i.ToString(), vs_up_ref_angle[i], ref ischange, true, filename);
            }

            if (ischange)
            {
                //创建backup
                //第一层
                string backup_filename = string.Format("{0}\\product\\{1}\\backup", Path.GetFullPath(".."), productname);
                SYS_PUD.CopyFile2(backup_filename);
                //文件
                string[] str = filename.Split('\\');
                SYS_PUD.FileWriteLine(string.Format(str[str.Length - 1]),backup, backup_filename);
            }
        }
        #endregion           

        #region 相机拍照
        /// <summary>
        /// 相机拍照(没有料盘移动)
        /// </summary>
        /// <param name="pos_mod_upcam">上相机的拍照位置</param>
        /// <param name="upcam">上相机</param>
        /// <param name="CapFlow">拍照流程</param>
        /// <returns></returns>
        public EM_RES UpCam(ref bool bquit,ST_XY pos_mod_upcam,string CapFlow,ref ST_XYA vs_data,bool Demo=false)
        {

            EM_RES ret;
            //移到拍照位
            ret=MT.ZupMove(ref bquit, ref ax_x, pos_mod_upcam.x, ref ax_y, pos_mod_upcam.y);
            if (ret != EM_RES.OK) return ret;
            //拍照
            ret = upcam.FindTaskTriAndWait(CapFlow,Demo);
            if (ret != EM_RES.OK) return ret;
            
            vs_data=upcam.curTask.ResData.PosMM;
            return EM_RES.OK;
        }

        /// <summary>
        /// 相机拍照(没有料盘移动)
        /// </summary>
        /// <param name="pos_mod_upcam">上相机的拍照位置</param>
        /// <param name="upcam">上相机</param>
        /// <param name="CapFlow">拍照流程</param>
        /// <returns></returns>
        public EM_RES UpCam(ref bool bquit, ST_XY pos_mod_upcam, string CapFlow, ref VisionOutPutData ResData, bool Demo = false)
        {

            EM_RES ret;
            //移到拍照位
            ret = MT.ZupMove(ref bquit, ref ax_x, pos_mod_upcam.x, ref ax_y, pos_mod_upcam.y);
            if (ret != EM_RES.OK) return ret;
            //拍照
            ret = upcam.FindTaskTriAndWait(CapFlow, Demo);
            if (ret != EM_RES.OK) return ret;

            ResData = upcam.curTask.ResData;
            return EM_RES.OK;
        }

        public EM_RES UpCamMod(ref bool bquit,out string barcode, ST_XYZA pos_mod_upcam, string CapFlow, ref ST_XYA vs_data,bool Demo=false)
        {
            barcode = "";
            EM_RES ret;
            //移到拍照位
            
            ret = MT.ZupMove(ref bquit, ref ax_x, pos_mod_upcam.x, ref ax_y, pos_mod_upcam.y,ref traybox_fd.ax_x,pos_mod_upcam.a);
            if (ret != EM_RES.OK) return ret;

            if (PT_SET.OvenCheck)
            {
                 VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, "检测探针是否断裂:");
                ret = upcam.FindTaskTriAndWait(CONST.OvenCheckFw, Demo);
                if (ret == EM_RES.OK)
                {
                    //return EM_RES.CAM_ERR;
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, "检测探针是否脱落未通过:");

                    MT.ST_WARN st_warn = new MT.ST_WARN();
                    warning fr_warn = new warning();
                    st_warn.ok_txt = VAR.IsChinese ? "继续运行" : "Give up";
                    st_warn.abort_txt = VAR.IsChinese ? "确定" : "Try again";
                    st_warn.cancle_txt = VAR.IsChinese ? "停止运行" : "Stop running";
                    st_warn.title = VAR.IsChinese ? "提示:检测探针是否脱落未通过!" : "Tip: Abnormal suction!";
                    st_warn.msg = "请注意：：：检测探针是否脱落未通过！！！";
                    st_warn.lb_msg = "提示:" + st_warn.msg + "请人工确认探针是否脱落!\r\n  1.点击继续运行将继续运行!\r\n  " +
                        "2.点击确定或者停止运行将停止运行!\r\n  ";

                    DialogResult logres = MT.Display_frwarn(fr_warn, st_warn, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                    if (DialogResult.OK == logres)
                    {

                    }
                    else
                    {
                        bquit = true;
                        return EM_RES.QUIT;
                    }
                }
            }

            //拍照
            ret = upcam.FindTaskTriAndWait(CapFlow,Demo);
            if (ret != EM_RES.OK) return ret;

            ST_XYA st_mm = upcam.curTask.ResData.PosMM;
            barcode = upcam.curTask.ResData.BarCode;
            vs_data = st_mm;
            return EM_RES.OK;
        }

        public EM_RES DwCam(ref bool bquit, string CapFlow, ref VisionOutPutData ResData, bool Demo = false)
        {
            EM_RES ret;
            //移到拍照位
            if(CapFlow == CONST.DwFindQrCodeFw[0]|| CapFlow == CONST.DwFindQrCodeFw[1])
            {
                double postemp = st_cap_pos.y + DwCapQrCodeoffset;
                ret = MT.ZupMove(ref bquit, ref ax_y, postemp);
                if (ret != EM_RES.OK) return ret;
            }
            else
            {
                ret = MT.ZupMove(ref bquit, ref ax_y, st_cap_pos.y);
                if (ret != EM_RES.OK) return ret;
            }
            
            //拍照
            ret = dwcam.FindTaskTriAndWait(CapFlow, Demo);
            if (ret != EM_RES.OK) return ret;
            ResData = dwcam.curTask.ResData;
            return EM_RES.OK;
        }

        public EM_RES DwCam(ref bool bquit, string CapFlow,ref ST_XYA vs_data,bool Demo=false)
        {

            EM_RES ret;
            //移到拍照位
            ret = MT.ZupMove(ref bquit,ref ax_y, st_cap_pos.y);
            if (ret != EM_RES.OK) return ret;
            //拍照
            ret = dwcam.FindTaskTriAndWait(CapFlow, Demo);
            if (ret != EM_RES.OK) return ret;

            vs_data = dwcam.curTask.ResData.PosMM;
            return EM_RES.OK;
        }
        #endregion

        #region 根椐相机数据取放料


        public ST_XYZA CaliPos(ST_XYZA xyza,double x=0)
        {
            ST_XYZA pos = xyza.clone();
            double xpos = 0;
            if (Math.Abs(ax_x.fenc_pos) < 5 && Math.Abs(x) > 3)
            {
                xpos = x;
            }              
            else if (Math.Abs(ax_x.fenc_pos) < 5 && Math.Abs(x) < 3 && Math.Abs(xyza.x) > 5)
            {
                if (Parent.id == 0) xpos = 50;
                else xpos = -50;
            }                
            else
                xpos = ax_x.fenc_pos;
             pos.a = -pos.x + xpos + pos.a;
            if (pos.a < 0)
            {
                pos.x = xpos - pos.a;
                pos.a = 0;
            }
            else
            {
                pos.x = xpos;
            }

            return pos;
        }
        /// <summary>
        /// 上相机定点拍照，吸头取物料
        /// </summary>
        /// <param name="upcam">上相机</param>
        /// <param name="pos_mod_upcam">上相机拍照位置</param>
        /// <param name="UpCamFlow">上相机拍照流程</param>
        /// <param name="IsPick">true 取  false 放</param>
        /// <param name="DwCamFlow">下相机拍照流程</param>
        /// <param name="cam_dw_vs">下相机拍照模组坐标</param>
        /// <returns></returns>
        public EM_RES XtCapMovMod(ref bool bquit,out string barcode, ST_XYZA pos_mod_upcam, string UpCamFlow, double ws_xpos=0, EM_XTFLOW xt_flow = EM_XTFLOW.PICKMOD, bool IsPick = true,bool Demo=false, string DwCamFlow = "", int trycnt = 2, ST_XYA cam_dw_vs = new ST_XYA(), ST_XYZA capQrcodePos = new ST_XYZA())
        {
            EM_RES ret = EM_RES.OK;
            ST_XYA st_cam_up_vs = new ST_XYA();
            ST_XYA st_cam_dw_vs = new ST_XYA();
            ST_XYZA st_pos_place;
            ST_XYZA st_pos_cam;
            ST_XYZA st_pos_qrcodecam = new ST_XYZA();
            barcode = "";
            if (DwCamFlow != "")
            {
                st_cam_dw_vs = cam_dw_vs;
            }
            else
            {
                st_cam_dw_vs = st_vs_pos_xtshp;
            }

            #region oldprogram
            //st_pos_cam.a = -pos_mod_upcam.x + ax_x.fenc_pos + pos_mod_upcam.a;
            //if (st_pos_cam.a < 0)
            //{
            //    st_pos_cam.x = ax_x.fenc_pos - st_pos_cam.a;
            //    st_pos_cam.y = pos_mod_upcam.y;
            //    st_pos_cam.z = pos_mod_upcam.z;
            //    st_pos_cam.a = 0;

            //}
            //else
            //{
            //    st_pos_cam.x = ax_x.fenc_pos;
            //    st_pos_cam.y = pos_mod_upcam.y;
            //    st_pos_cam.z = pos_mod_upcam.z;
            //}
            #endregion

            st_pos_cam = CaliPos(pos_mod_upcam,ws_xpos);
            if(PT_SET.bAddCapQrcode && (capQrcodePos.x!=0 || capQrcodePos.y != 0 || capQrcodePos.z != 0 || capQrcodePos.a != 0))
            {
                st_pos_qrcodecam = CaliPos(capQrcodePos, ws_xpos);
            }

            if(PT_SET.bAddCapQrcode)
            {
                //上拍照
                for (int i = 0; i < trycnt; i++)
                {
                    //拍照NG偏移
                    if (i == 1)
                    {
                        st_pos_cam.x = Parent.id == COM.UDLoad1.id ? st_pos_cam.x - 2 : st_pos_cam.x + 2;
                        st_pos_qrcodecam.x = Parent.id == COM.UDLoad1.id ? st_pos_qrcodecam.x - 2 : st_pos_qrcodecam.x + 2;
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}拍照失败,X负方向偏移2mm重拍!", upcam.disc) : string.Format("{0} The photo failed, and the negative X direction was offset by 2mm!              ({1}拍照失败,X负方向偏移2mm重拍!)", upcam.englishdisc, upcam.disc));
                    }
                    else if (i == 2)
                    {
                        st_pos_cam.x = Parent.id == COM.UDLoad1.id ? st_pos_cam.x + 4 : st_pos_cam.x - 4;
                        st_pos_qrcodecam.x = Parent.id == COM.UDLoad1.id ? st_pos_qrcodecam.x + 4 : st_pos_qrcodecam.x - 4;
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}拍照失败,X正方向偏移2mm重拍!", upcam.disc) : string.Format("{0} The photo failed, and the positive X direction was offset by 2mm!              ({1}拍照失败,X正方向偏移2mm重拍!)", upcam.englishdisc, upcam.disc));
                    }
                    try
                    {
                        ret = MT.ZupMove(ref bquit, ref ax_x, st_pos_cam.x, ref ax_y, st_pos_cam.y);
                        if (ret != EM_RES.OK) return ret;
                        int n = 0;
                        while (traybox_fd.runin || !traybox_fd.ax_x.isStop)
                        {
                            if (n++ > 500) break;
                            if (bquit) return EM_RES.QUIT;
                            Thread.Sleep(10);
                        }
                        traybox_fd.MoveToLastPos[this.Parent.id] = true;
                        if (n > 500)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}定位超时5S", traybox_fd.disc) : string.Format("{0} move timeout (5s)        ({1}定位超时5S)", traybox_fd.name, traybox_fd.disc), DReport.EmErrCode.Timeout, (int)DReport.EmHareware.Nozzle + id + 1, ERR_ALM.EmErrItem.TimeOut);
                            return EM_RES.ERR;
                        }
                    }
                    finally
                    {
                        traybox_fd.runin = false;
                    }

                    ret = UpCamMod(ref bquit,out barcode, st_pos_cam, UpCamFlow, ref st_cam_up_vs, Demo);
                    if (ret == EM_RES.OK)
                    {
                        //break;
                    }
                    else if (ret != EM_RES.ABORT || ret != EM_RES.CAM_ERR)
                    {
                        return ret;
                    }

                    try
                    {
                        ret = MT.ZupMove(ref bquit, ref ax_x, st_pos_qrcodecam.x, ref ax_y, st_pos_qrcodecam.y);

                        if (ret != EM_RES.OK) return ret;
                        int n = 0;
                        while (traybox_fd.runin || !traybox_fd.ax_x.isStop)
                        {
                            if (n++ > 500) break;
                            if (bquit) return EM_RES.QUIT;
                            Thread.Sleep(10);
                        }
                        traybox_fd.MoveToLastPos[this.Parent.id] = true;
                        if (n > 500)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}定位超时5S", traybox_fd.disc) : string.Format("{0} move timeout (5s)        ({1}定位超时5S)", traybox_fd.name, traybox_fd.disc), DReport.EmErrCode.Timeout, (int)DReport.EmHareware.Nozzle + id + 1, ERR_ALM.EmErrItem.TimeOut);
                            return EM_RES.ERR;
                        }
                    }
                    finally
                    {
                        traybox_fd.runin = false;
                    }
                    ST_XYA temp = new ST_XYA();
                    ret = UpCamMod(ref bquit,out barcode, st_pos_qrcodecam, CONST.FindQrCodeFw, ref temp, Demo);
                    if (ret == EM_RES.OK)
                    {
                        break;
                    }
                    else if (ret != EM_RES.ABORT || ret != EM_RES.CAM_ERR)
                    {
                        return ret;
                    }

                }
                if (ret == EM_RES.CAM_ERR) return ret;
            }
            else
            {
                //上拍照
                for (int i = 0; i < trycnt; i++)
                {
                    //拍照NG偏移
                    if (i == 1)
                    {
                        st_pos_cam.x = Parent.id == COM.UDLoad1.id ? st_pos_cam.x - 2 : st_pos_cam.x + 2;
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}拍照失败,X负方向偏移2mm重拍!", upcam.disc) : string.Format("{0} The photo failed, and the negative X direction was offset by 2mm!              ({1}拍照失败,X负方向偏移2mm重拍!)", upcam.englishdisc, upcam.disc));
                    }
                    else if (i == 2)
                    {
                        st_pos_cam.x = Parent.id == COM.UDLoad1.id ? st_pos_cam.x + 4 : st_pos_cam.x - 4;
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}拍照失败,X正方向偏移2mm重拍!", upcam.disc) : string.Format("{0} The photo failed, and the positive X direction was offset by 2mm!              ({1}拍照失败,X正方向偏移2mm重拍!)", upcam.englishdisc, upcam.disc));
                    }
                    try
                    {
                        //if (st_pos_cam.y - ax_y.fenc_pos < 150)
                        //{
                        //    ret = ax_y.SetToWorkSpd(0.4);
                        //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0}速度降为原来的40%:", ax_y.disc));
                        //}
                        ret = MT.ZupMove(ref bquit, ref ax_x, st_pos_cam.x, ref ax_y, st_pos_cam.y);
                        //ax_y.SetToWorkSpd();
                        if (ret != EM_RES.OK) return ret;
                        int n = 0;
                        while (traybox_fd.runin || !traybox_fd.ax_x.isStop)
                        {
                            if (n++ > 500) break;
                            if (bquit) return EM_RES.QUIT;
                            Thread.Sleep(10);
                        }
                        traybox_fd.MoveToLastPos[this.Parent.id] = true;
                        if (n > 500)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}定位超时5S", traybox_fd.disc) : string.Format("{0} move timeout (5s)        ({1}定位超时5S)", traybox_fd.name, traybox_fd.disc), DReport.EmErrCode.Timeout, (int)DReport.EmHareware.Nozzle + id + 1, ERR_ALM.EmErrItem.TimeOut);
                            return EM_RES.ERR;
                        }
                    }
                    finally
                    {
                        traybox_fd.runin = false;
                    }

                    ret = UpCamMod(ref bquit,out barcode, st_pos_cam, UpCamFlow, ref st_cam_up_vs, Demo);
                    if (ret == EM_RES.OK)
                    {
                        break;
                    }
                    else if (ret != EM_RES.ABORT || ret != EM_RES.CAM_ERR)
                    {
                        return ret;
                    }

                }
                if (ret == EM_RES.CAM_ERR) return ret;
            }
            


            //清空视觉数据
            if (!upcam.FlushOk && !upcam.FlushUpdate)
            {
                Task tak = new Task(() =>
                {
                    upcam.FlushUpdate = true;
                    upcam.mAcqFifo.Flush();
                    upcam.FlushOk = true;
                    upcam.FlushUpdate = false;
                });
                tak.Start();
            }
            //计算
            st_pos_place = XtCalPos(st_pos_cam.ToXY(), st_cam_up_vs, st_cam_dw_vs, xt_flow).ToXYZA();

            st_pos_place.z = (id % 2 == 0) ? pos_mod_upcam.z : -pos_mod_upcam.z;

            if (IsPick)
            {
                xt_pos_pick_tray_x = traybox_fd.ax_x.fenc_pos;
                xt_pos_pick_mod = st_pos_place;
            }
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese?string.Format("{0}拍照,{1},Tx{2}, vs:{3}", upcam.disc, pos_mod_upcam.ToString(), traybox_fd.ax_x.fenc_pos, st_cam_up_vs.ToString()): string.Format("{0} photograph ,{2},Tx{3}, vs:{4}              ({1}拍照,{2},Tx{3}, vs:{4})", upcam.englishdisc,upcam.disc, pos_mod_upcam.ToString(), traybox_fd.ax_x.fenc_pos, st_cam_up_vs.ToString()));
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0}拍照,Y:{1} ,vs:{2}", dwcam.disc, st_cap_pos.y, st_cam_dw_vs.ToString()): string.Format("{0} photograph,Y:{2} ,ws:{3}          ({1}拍照,Y:{2} ,vs:{3})",dwcam.englishdisc, dwcam.disc, st_cap_pos.y, st_cam_dw_vs.ToString()));
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0},Vs_Offset{1},vs_up_ref_angle:{2:f3}, vs_down_ref_angle{3:f3}) ", upcam.disc, vs_offset[(int)xt_flow].ToString(), vs_up_ref_angle[(int)xt_flow], vs_down_ref_angle[(int)xt_flow]));
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0},{1},{2}", disc, IsPick == true ? "取料" : "放料", st_pos_place.ToString()): string.Format(" {0},{1},{3}            ({0},{2},{3})", disc, IsPick == true ? "PICK" : "PLACE", IsPick == true ? "取料" : "放料", st_pos_place.ToString()));
            //barcode = upcam.curTask.ResData.BarCode;
            //取料
            ret = PickOrPlace(ref bquit, st_pos_place, IsPick,Demo,true);
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0},{1}结束", disc, IsPick == true ? "取料" : "放料"): string.Format("{0},{1} end           ({0},{2}结束)", disc, IsPick == true ? "PICK" : "PLACE", IsPick == true ? "取料" : "放料"));
            if (ret != EM_RES.OK) return ret;
            MT.Move(ref bquit, ref ax_u, 0);
            return EM_RES.OK;
         }

        /// <summary>
        /// 根据视觉计算吸头定位位置
        /// </summary>
        /// <param name="cam_flow">照流程:0:取模组 ,1:放模组</param>
        /// <param name="st_cam_up_pos">上相机拍照位置</param>
        /// <param name="st_cam_up_vs">上相机拍模组坐标</param>
        /// <param name="st_cam_dw_vs">下相机拍模组坐标</param>
        /// <returns></returns>
        public ST_XYA XtCalPos(ST_XY st_cam_up_pos, ST_XYA st_cam_up_vs, ST_XYA st_cam_dw_vs, EM_XTFLOW xt_flow = EM_XTFLOW.PICKMOD)
        {
            ST_XYA cali_ofs = st_pos_affine_place.ToXYA() - st_pos_affine_dwcam_cap.ToXYA() - st_pos_affine_upcam_cap.ToXYA();
            ST_XYA st_place = Utility.CalcPlacePos(st_cam_up_pos, st_cam_up_vs, st_cap_pos.ToXY(), st_cam_dw_vs, cali_ofs, st_rol.ToXY(), st_rol_cap, vs_offset[(int)xt_flow], vs_up_ref_angle[(int)xt_flow], vs_down_ref_angle[(int)xt_flow]);
            return st_place;
        }

        /// <summary>
        /// 根据上相机位置确认吸头位置
        /// </summary>
        /// <returns></returns>
        public ST_XY XtCalPos(ST_XY pos)
        {
            ST_XYA cali_ofs = st_pos_affine_place.ToXYA() - st_pos_affine_dwcam_cap.ToXYA() - st_pos_affine_upcam_cap.ToXYA();
            ST_XY st_pos = cali_ofs.ToXY() + st_rol_cap.ToXY() + pos;
            return st_pos;
        }

        /// <summary>
        /// 根据上相机位置确认拍照位置
        /// </summary>
        /// <returns></returns>
        public ST_XY XtCamPos(ST_XY xtplace)
        {
            ST_XYA cali_ofs = st_pos_affine_place.ToXYA() - st_pos_affine_dwcam_cap.ToXYA() - st_pos_affine_upcam_cap.ToXYA();
            ST_XY st_pos =xtplace-cali_ofs.ToXY() - st_rol_cap.ToXY();
            return st_pos;
        }
        /// <summary>
        /// 根据上下相机数据取放料
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="upcam"></param>
        /// <param name="pos_mod_upcam"></param>
        /// <param name="UpCamFlow"></param>
        /// <param name="IsPick"></param>
        /// <returns></returns>
        public EM_RES XtPickOrPlaceMod(ref bool bquit, ST_XY pos_mod_upcam, ST_XYA cam_up_vs, ST_XYA cam_dw_vs, double zpos, bool IsDemo = false, EM_XTFLOW xt_flow = EM_XTFLOW.PICKMOD, bool IsPick = true, bool bPasteUp = false,bool bdismove=false)
        {
            ST_XYZA st_pos_place;
            //如果下相机数据为0,数据为空吸头数据
            if (cam_dw_vs.x == -10000 && cam_dw_vs.y == -10000)
            {
                cam_dw_vs = st_vs_pos_xtshp;
            }

            //计算
            st_pos_place = XtCalPos(pos_mod_upcam, cam_up_vs, cam_dw_vs, xt_flow).ToXYZA();
            st_pos_place.z = (id % 2 == 0) ? zpos : -zpos;
            //取模组不带角度取
            if (IsPick)
            {
                xt_pos_pick_tray_x = traybox_fd.ax_x.fenc_pos;
                xt_pos_pick_mod = st_pos_place;
            }
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese?string.Format("{0}拍照,{1},Tx:{2} vs:{3}", upcam.disc, pos_mod_upcam.ToString(), COM.traybox_fd.ax_x.fenc_pos, cam_up_vs.ToString()): string.Format("{0} photograph ,{2},Tx:{3} vs:{4}         ({1}拍照,{2},Tx:{3} vs:{4})",upcam.englishdisc, upcam.disc, pos_mod_upcam.ToString(), COM.traybox_fd.ax_x.fenc_pos, cam_up_vs.ToString()));
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0}拍照,Y:{1} ,vs:{2}", dwcam.disc, st_cap_pos.y, cam_dw_vs.ToString()): string.Format("{0} photograph ,Y:{2},vs:{3}         ({1}拍照,Y:{2} ,vs:{3})",dwcam.englishdisc, dwcam.disc, st_cap_pos.y, cam_dw_vs.ToString()));
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0},Vs_Offset{1},vs_up_ref_angle:{2:f3}, vs_down_ref_angle{3:f3}) ", upcam.disc, vs_offset[(int)xt_flow].ToString(), vs_up_ref_angle[(int)xt_flow], vs_down_ref_angle[(int)xt_flow]));
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0}{1},{2}", disc, IsPick == true ? "取料" : "放料", st_pos_place.ToString()): string.Format("{0}{1},{3}          ({0}{2},{3})", disc, IsPick == true ? "PICK" : "PLACE", IsPick == true ? "取料" : "放料", st_pos_place.ToString()));

            //string filename= string.Format("VAR.gsys_set.GetCurProductPath + //CsvData//{0}{1}{2}data.csv",VAR.gsys_set.GetCurProductPath,disc ,IsPick?"Pick":"Place");
            //Utility.WriteStrToCSV(filename, string.Format("{0},{1:F3},{2:F3},{3:F3},{4:F3}", DateTime.Now.ToString("hh:mm:ss:fff"), st_pos_place.x, st_pos_place.y, st_pos_place.z, st_pos_place.a));
            //取料
            EM_RES ret = PickOrPlace(ref bquit, st_pos_place, IsPick,IsDemo,true, bPasteUp, bdismove);
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0},{1}结束", disc, IsPick == true ? "取料" : "放料"): string.Format("{0},{1} end           ({0},{2}结束)", disc, IsPick == true ? "PICK" : "PLACE", IsPick == true ? "取料" : "放料"));
            if (ret != EM_RES.OK) return ret;
            MT.Move(ref bquit, ref ax_u, 0);
            return EM_RES.OK;
        }

        #endregion

        #region 物料取放

        /// <summary>
        /// /吸头取料
        /// </summary>
        /// <param name="bquit">系统退出标志</param>
        /// <param name="st_pos">取料坐标</param>
        /// <param name="bpick">取:true 放:false</param>
        /// <returns></returns>
        public EM_RES PickOrPlace(ref bool bquit, ST_XYZA st_pos, bool bpick = true, bool IsDemo = false,bool DwMov=false,bool bPasteUp=false,bool bDisMove=false)
        {
            EM_RES res=EM_RES.OK;
            EM_RES res1 = EM_RES.OK;         
            if (!IsDemo)
            {
                if (bpick)
                {
                    //有料直接返回
                    if (cy_zk.isONByChkSen)
                    {
                        if (XtMd != null)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese?string.Format("{0}取料前已检测到模组返回!", disc): string.Format("{0} Module return has been detected before picking           ({0}取料前已检测到模组返回!)", disc));
                            return EM_RES.OK;
                        }
                        else
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}取料前真空阀值已达要求,但系统标识无模组,请检真空阀值设定是否OK!", disc): string.Format("{0}The vacuum threshold has been met before the material is taken, but there is no module in the system identification. Please check whether the vacuum threshold setting is OK!            ({0}取料前真空阀值已达要求,但系统标识无模组,请检真空阀值设定是否OK!)", disc), DReport.EmErrCode.PickFailed, (int)DReport.EmHareware.Nozzle + id + 1, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                            return EM_RES.ERR;
                        }
                    }
                  
                }
                else
                {
                    //无料直接返回
                    if (!cy_zk.isONByChkSen)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR,VAR.IsChinese? (disc + "真空达不到要求返回!"): (disc + "Vacuum does not meet requirements to return            (真空达不到要求返回!)"), DReport.EmErrCode.PlaceFailed, (int)DReport.EmHareware.Nozzle + id + 1, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                        //cy_zk.SetOff();
                        return EM_RES.ERR;
                    }
                }
            }
           
            try
            {
                //move to posInf
                if (Math.Abs(ax_z.fenc_pos) > 0.5)
                {
                    res = MT.Move(ref bquit, ref ax_z, 0);
                    if (res != EM_RES.OK) return res;
                }
                if( bDisMove)
                {
                    res = MT.Move(ref bquit, ref ax_x, st_pos.x, ref ax_y, st_pos.y, ref ax_u, st_pos.a);
                    if (res != EM_RES.OK) return res;
                    res = disMove(ref bquit, st_pos, bpick);
                }
                else
                {
                    res = MT.Move(ref bquit, ref ax_x, st_pos.x, ref ax_y, st_pos.y, ref ax_u, st_pos.a);
                }
               
                //ax_y.SetToWorkSpd();
                if (res != EM_RES.OK) return res;

                if (bpick) cy_zk.SetOn();

                //空跑模式
                if (IsDemo)
                {
                    if (st_pos.z < 0) st_pos.z += 5;
                    else if (st_pos.z > 0) st_pos.z -= 5;
                }
               
                res = MT.Move(ref bquit, ref ax_z, st_pos.z);
                if (res != EM_RES.OK) return res;
                
                //Thread.Sleep(10);

                //等待真空
                if (!IsDemo)
                {
                    if (bpick)
                    {
                        // VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "打开"+ disc+"真空");
                        for (int i = 1; i < 3; i++)
                        {
                            res = cy_zk.SetOn(ref bquit, 500);
                            if (res == EM_RES.OK || !DwMov) break;
                            else if (res == EM_RES.TIMEOUT)
                            {
                                res1 = MT.Move(ref bquit, ref ax_z,id%2==0? st_pos.z + i : st_pos.z - i);
                                if (res1 != EM_RES.OK) break;
                            }

                        }

                        if (res != EM_RES.OK)
                        {
                            if (!cy_zk.isONByChkSen)
                            {
                                cy_zk.SetOff();
                                //破真空避免带起
                                gpio_pzk.SetOn();
                            }
                            if (res != EM_RES.QUIT)  res= EM_RES.PICK_ERR;
                           // else  return EM_RES.QUIT;
                        }
                        
                        

                    }
                    else
                    {
                        //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "关闭" + disc + "真空");
                       
                        cy_zk.SetOff();
                        //Thread.Sleep(100);
                        // VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, gpio_pzk.disc+"打开");
                        gpio_pzk.SetOn();
                        //Application.DoEvents();
                        Thread.Sleep(50);
                        if (bPasteUp)
                        {
                            Thread.Sleep(PT_SET.PlaceDly-40);
                            res = MT.Move(ref bquit, ref ax_z, id % 2 == 0 ? st_pos.z-1 : st_pos.z+1);
                           // if (res != EM_RES.OK) return res;
                            Thread.Sleep(PT_SET.MovUpDly);
                        }
                        //// VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, gpio_pzk.disc + "关闭");
                        //gpio_pzk.SetOff();

                    }
                }
                else
                {
                    if (bpick)
                    {
                         cy_zk.SetOn();                    

                    }
                    else
                    {
                      

                        cy_zk.SetOff();
                        gpio_pzk.SetOn();
                        Thread.Sleep(60);

                    }
                }
               
            }
            finally
            {
                bool b = false;
                res1 = MT.Move(ref b, ref ax_z, 0);
                if(cy_zk.isOFF||gpio_pzk.isON)
                 gpio_pzk.SetOff();
                //rechk zk
                if (res == EM_RES.OK && !cy_zk.isONByChkSen && !IsDemo && bpick)
                {
                    Thread.Sleep(20);
                    if (!cy_zk.isONByChkSen)
                    {
                        PickTrayChkErrCnt++;
                        if(PickTrayChkErrCnt>PickTrayChkErrCntLimit)
                        {
                            PickTrayChkErrCnt = 0;
                            res = PickTrayChkErrShow();                  
                        }
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese?string.Format("{0} 取料后复检失败!", disc): string.Format("{0} Failure to recheck after picking up!        ({0} 取料后复检失败!)", disc));
                        if (res == EM_RES.OK)
                        res = EM_RES.PICK_ERR;
                    }
                  
                }                
            }

            if (res1 == EM_RES.OK) return res;
            else return res1;
        }

        #endregion

        #region 物料取放(单独取放函数)
        /// <summary>
        /// 偏移运动
        /// </summary>
        /// <param name="bquit"></param>
        /// <param name="pos"></param>
        /// <param name="bpick"></param>
        /// <param name="bAfterDisMove">先定位到位再偏移</param>
        /// <returns></returns>
        private EM_RES disMove(ref bool bquit, ST_XYZA pos, bool bpick=true,bool bAfterDisMove=true)
        {
            if (bquit) return EM_RES.QUIT;
            string newdesc = disc +( bpick ? "工站取料" : "工站放料");
            if (bAfterDisMove) // 先定位到位再偏移
            {
                if (Math.Abs(ax_x.fenc_pos - pos.x) > 2 || Math.Abs(ax_y.fenc_pos - pos.y) > 2)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, newdesc + "前偏移当前位置和目标位置偏差大:" + pos.ToString());
                    return EM_RES.ERR;
                }
            }
            var dispos =bpick? new ST_XYZ() { x = NewSysInf.UserParams.PickWsDisX, y = NewSysInf.UserParams.PickWsDisY, z = NewSysInf.UserParams.PickWsDisZ }:
                new ST_XYZ() { x = NewSysInf.UserParams.PlaceWsDisX, y = NewSysInf.UserParams.PlaceWsDisY, z = NewSysInf.UserParams.PlaceWsDisZ }; ;
            if (Math.Abs(dispos.z) > 30 || Math.Abs(dispos.y) > 20 || Math.Abs(dispos.x) > 20)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, newdesc + "前偏移超过20mm请重新设置，偏移量:" + dispos.ToString());
                return EM_RES.ERR;
            }
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, newdesc + "前偏移量:" + dispos.ToString());
            if (bAfterDisMove)
            {
                var res = MT.Move(ref bquit, ref ax_z, id % 2 == 0 ? pos.z - dispos.z : pos.z + dispos.z);
                if (res != EM_RES.OK) return res;
                res = MT.Move(ref bquit, ref ax_x, pos.x + dispos.x, ref ax_y, pos.y + dispos.y, bchkps: false);
                if (res != EM_RES.OK)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, newdesc + "前偏移定位失败，偏移量:" + dispos.ToString() + "结果res:" + res.ToString());
                    return res;
                }
                return res;
            }else
            {
                var res = MT.Move(ref bquit, ref ax_x, pos.x + dispos.x, ref ax_y, pos.y + dispos.y);
                if (res != EM_RES.OK)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, newdesc + "前偏移定位失败，偏移量:" + dispos.ToString() + "结果res:" + res.ToString());
                    return res;
                }
                 res = MT.Move(ref bquit, ref ax_z, id % 2 == 0 ? pos.z - dispos.z : pos.z + dispos.z);
                if (res != EM_RES.OK) return res;

                res = MT.Move(ref bquit, ref ax_x, pos.x, ref ax_y, pos.y, ref ax_u, pos.a,bchkps:false);
                if (res != EM_RES.OK) return res;
                return res;
            }
          
          
        }
        /// <summary>
        /// 取料
        /// </summary>
        /// <param name="bquit">系统退出标志</param>
        /// <param name="pos">取料坐标</param>
        /// <param name="bcheck">取:true 放:false</param>
        ///   /// <param name="bMoveDis">取起来前偏移部分</param>
        /// <returns></returns>
        public EM_RES Pick(ref bool bquit, ST_XYZA pos, bool bcheck = true,bool IsDemo=false, bool DwMov = false,bool bMoveDis=false)
        {
            EM_RES res = EM_RES.OK;
            EM_RES res1 = EM_RES.OK;
            //演示模式不检查真空
            if (IsDemo) bcheck = false;
           
            //check zk
            if (bcheck && cy_zk.isONByChkSen)
            {
                if(XtMd!=null)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, VAR.IsChinese?string.Format("{0}取料前已检测到模组返回!", disc): string.Format("{0} Module return has been detected before picking!          ({0}取料前已检测到模组返回!)", disc));
                    return EM_RES.OK;
                }
                else
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0}取料前真空阀值已达要求,但系统标识无模组,请检真空阀值设定是否OK!", disc): string.Format("The vacuum threshold has been met before the material is taken, but there is no module in the system identification. Please check whether the vacuum threshold setting is OK!        ({0}取料前真空阀值已达要求,但系统标识无模组,请检真空阀值设定是否OK!)", disc), DReport.EmErrCode.PickFailed, (int)DReport.EmHareware.Nozzle + id + 1, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                    return EM_RES.ERR;
                }
            }

            try
            {
                //move to posIn
                //if (pos.y - ax_y.fenc_pos < 150)
                //{
                //    res = ax_y.SetToWorkSpd(0.4);
                //    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0}速度降为原来的40%:", ax_y.disc));
                //}
                res = MT.ZupMove(ref bquit, ref ax_x, pos.x, ref ax_y, pos.y, ref ax_u, pos.a);
                //ax_y.SetToWorkSpd();
                if (res != EM_RES.OK) return res;

                cy_zk.SetOn();
                //down
                res = MT.Move(ref bquit, ref ax_z, pos.z);
                if (res != EM_RES.OK) return res;

                //zk on and wait
                for (int i = 1; i < 3; i++)
                {
                    res = cy_zk.SetOn(ref bquit, bcheck ? 500 : 100);
                    if (res == EM_RES.OK || !DwMov ||(!bcheck && res == EM_RES.TIMEOUT)) break;
                    else if (res == EM_RES.TIMEOUT)
                    {
                        res1 = MT.Move(ref bquit, ref ax_z, id % 2 == 0 ? pos.z + i*0.5 : pos.z - i*0.5);
                        if (res1 != EM_RES.OK) return res;
                    }

                }
                //res = cy_zk.SetOn(ref bquit, bcheck ? 1000 : 100);
                if (!bcheck && res == EM_RES.TIMEOUT) return EM_RES.OK;
                else if (res != EM_RES.OK)
                {
                    if (!cy_zk.isONByChkSen)
                    {
                        cy_zk.SetOff();
                        //破真空避免带起
                        gpio_pzk.SetOn();
                    }
                    if (res != EM_RES.QUIT) return EM_RES.PICK_ERR;
                    else return EM_RES.QUIT;
                }
                //工站取料前偏移
             

            }
            finally
            {
                if (bMoveDis)
                {
                   res = disMove(ref bquit,pos);
                }
                //z up
                bool b = false;
           
                MT.Move(ref b, ref ax_z, 0);
                MT.Move(ref b, ref ax_u, 0);
                if (gpio_pzk.isON)
                gpio_pzk.SetOff();

                //rechk zk
                if (res == EM_RES.OK && !cy_zk.isONByChkSen && !IsDemo)
                {
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.WAR, VAR.IsChinese?string.Format("{0} 取料后复检失败!",disc): string.Format("{0} Failure to recheck after picking!        ({0} 取料后复检失败!)", disc));
                    res = EM_RES.PICK_ERR;
                }
            }

            return res;
        }
        /// <summary>
        /// 放料
        /// </summary>
        /// <param name="bquit">系统退出标志</param>
        /// <param name="pos">取料坐标</param>
        /// <param name="bcheck">取:true 放:false</param>
        /// <returns></returns>
        public EM_RES Place(ref bool bquit, ST_XYZA pos, bool bcheck = true,bool IsDemo=false)
        {
            EM_RES res;

            //演示模式不检查真空
            if (IsDemo) bcheck = false;

            //check zk
            if (bcheck && !cy_zk.isONByChkSen)
            {
                VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese?string.Format("{0} 放料前真空失效!", disc): string.Format("{0} Vacuum failure before unloading!         ({0} 放料前真空失效!)", disc), DReport.EmErrCode.PlaceFailed, (int)DReport.EmHareware.Nozzle + id + 1, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                return EM_RES.PLACE_ERR;
            }

            try
            {
 
                res = MT.ZupMove(ref bquit, ref ax_x, pos.x, ref ax_y, pos.y, ref ax_u, pos.a);
                if (res != EM_RES.OK) return res;

                //down
                res = MT.Move(ref bquit, ref ax_z, pos.z);
                if (res != EM_RES.OK) return res;

                //zk off
                cy_zk.SetOff();
                gpio_pzk.SetOn();
                Thread.Sleep(50);
            }
            finally
            {
                //z up
                bool b = false;
                res = MT.Move(ref b, ref ax_z, 0);
                gpio_pzk.SetOff();
            }

            return res;
        }

        #endregion

        #region 吸头放料于料盘
        public EM_RES PlaceMd(ref bool bquit, TrayBox traybox,WS ws,bool IsDemo=false)
        {
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, $"{disc} 吸头放料于料盘");
            EM_RES res;
            List<Product.Tray.PosInf> placepos = new List<Product.Tray.PosInf>();
            List<Product.Tray.PosInf> placepos_temp = new List<Product.Tray.PosInf>();
            ST_XYZA pos = new ST_XYZA();
            ST_XYZA enpos = new ST_XYZA();
            ST_XYA VsCmp = new ST_XYA();
            if (XtMd == null || (XtMd != null && XtMd.res < 0)) return EM_RES.OK;
            placepos_temp = traybox.tray_cur.GetPosList(Product.EM_CM_RES.EMPTY);
            if (traybox.disc == traybox_ok.disc)
                placepos = traybox.tray_cur.GetPosList(Product.EM_CM_RES.EMPTY);
            else if (traybox.disc == traybox_ng.disc)
                placepos = traybox.tray_cur.GetPosList(Product.EM_CM_RES.EMPTY, XtMd.res);
               // placepos = traybox.tray_cur.GetPosList(Product.EM_CM_RES.EMPTY);
            if (placepos.Count > 0)
            {
                bool check=true;
                foreach (XT xt in Parent.list_xt)
                {
                    if (xt.XtMd != null)
                    {
                        if (xt.XtMd.res == 3342 || xt.XtMd.res == 2222)
                        {
                            VAR.msg.AddMsg(Msg.EM_MSGTYPE.SYS, $"{disc} 存在3342或者2222，不在检测二维码");
                            //VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, VAR.IsChinese ? string.Format("{0} 存在3342，不在检测二维码!", disc) : string.Format("{0} Vacuum failure before unloading!         ({0} 放料前真空失效!)", disc), DReport.EmErrCode.PlaceFailed, (int)DReport.EmHareware.Nozzle + id + 1, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                            check = false;
                        }
                        
                    }

                }

                res = Parent.FlyCamToTray(ref bquit, ws, traybox,check ,ref enpos);
                if (res != EM_RES.OK && res != EM_RES.NEXT && res != EM_RES.RETRY) return res;

                if (placepos_temp.Count == traybox.tray_cur.list_pos.Count && PT_SET.bEnVsTray && traybox.disc != traybox_fd.disc)
                {
                    //if (PT_SET.isopen_degree)
                    //{

                    //}
                    res = MT.ZupMove(ref bquit, ref ax_x, enpos.x, ref ax_y, enpos.y, ref ax_u, 0, ref traybox.ax_x, enpos.a);
                    if (res != EM_RES.OK) return res;
                    int i=0;
                    for (; i < 3;i++ )
                    {
                        res = upcam.FindTaskTriAndWait(CONST.TrayUpFw);
                        if (res != EM_RES.OK)
                        {
                            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, VAR.IsChinese ? "拍照错误!" : "Cam ERR", 20, true);
                            MT.ST_WARN warn = new MT.ST_WARN();//增加语言
                            warning fr_warn = new warning();
                            warn.ok_txt = MultiLanguage.TxtSelct("更换料盘", "Replace tray", "Thay khay");
                            warn.cancle_txt = MultiLanguage.TxtSelct("重新拍照", "Take a photo", "Chụp ảnh");
                            warn.abort_txt = MultiLanguage.TxtSelct("停止运行", "Stop running", "Ngừng chạy");
                            warn.ws = null;
                            warn.title = MultiLanguage.TxtSelct("提示:拍照错误!", "Tip: Failed to take a picture!", "Mẹo: Không chụp được ảnh!");

                            string tray = traybox.disc == traybox_ok.disc ? "OK料盘" : "NG料盘";
                            warn.msg = MultiLanguage.TxtSelct($"{upcam.disc}拍{tray}失败!", $"{upcam.englishdisc} failed to shoot {tray}!", $"{upcam.disc} không bắn được {tray}!");
                            warn.lb_msg = MultiLanguage.TxtSelct(
                                $"提示:{upcam.disc}拍{tray}失败! 请确认!\r\n" +
                                $"1.如果料盘放反或拍照位有异物,请按更换料盘按键\r\n" +
                                $"2.如需重拍确认,请按重新拍照键\r\n" +
                                $"3.如需停止,请按停止运行键",

                                $"Tip:{upcam.englishdisc} failed to shoot {tray}!,please comfirm!\r\n" +
                                $"1.If the tray is turned upside down or there is a foreign object in the photo position, press 'Replace tray' button\r\n" +
                                $"2.If you want to take a photo again, press the 'Take a photo' key\r\n" +
                                $"3. If you want to stop, press the stop running key",

                                $"{upcam.disc} failed to shoot {tray}!\r\n" +
                                $"1.Nếu khay bị lật ngược hoặc có vật thể lạ ở vị trí ảnh chụp, hãy nhấn nút 'Thay khay'\r\n" +
                                $"2.Nếu bạn muốn chụp ảnh lại, hãy nhấn phím 'Chụp ảnh'\r\n" +
                                $"3. Nếu bạn muốn dừng, hãy bấm phím dừng chạy");
                            DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.CaptureAbnomal);
                            if (DialogResult.OK == logres)
                            {
                                VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                                return EM_RES.END;
                            }
                            else if (DialogResult.Abort == logres)
                            {
                                return EM_RES.ERR;
                            }
                            VAR.sys_inf.Set(EM_ALM_STA.NOR_GREEN, VAR.IsChinese ? "运行" : "RUN", 0, true);
                        }
                        else break;
                    }
                    if(i>=3)
                    {
                        VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese ? string.Format("{0}拍{1}失败次数达3次!",upcam.disc,traybox.disc==traybox_ok.disc?"OK料盘":"NG料盘"): string.Format("{0} failed to take pictures of {1} 3 times      ({2}拍{3}失败次数达3次!)", upcam.englishdisc, traybox.disc == traybox_ok.disc ? "OK tray" : "NG tray", upcam.disc, traybox.disc == traybox_ok.disc ? "OK料盘" : "NG料盘"), DReport.EmErrCode.CaptureFailed, (int)DReport.EmHareware.UpDownLoad, ERR_ALM.EmErrItem.UpDownLoadAbnormal);
                        return EM_RES.ERR;
                    }
                    if(traybox_ok.disc==traybox.disc) UpDownLoad.Vs_TrayOK = upcam.curTask.ResData.PosMM;
                    else UpDownLoad.Vs_TrayNg = upcam.curTask.ResData.PosMM;                    
                }
                //位置确认
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, "进行位置确认");
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG,  string.Format("{0}放料，计算前位置{1}", traybox.disc,placepos[0].Pos[Parent.id].ToXY().ToString()));
                pos = CaliPos(placepos[0].Pos[Parent.id]);
                if (PT_SET.bEnVsTray && traybox.disc != traybox_fd.disc)
                {
                    if (traybox_ok.disc == traybox.disc) VsCmp = UpDownLoad.Vs_TrayOK;
                    else VsCmp = UpDownLoad.Vs_TrayNg;  
                }

                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0}放料，计算后位置{1}", traybox.disc, pos.ToXY().ToString()));
                pos.x = pos.x + st_rol_cap.x - Parent.list_xt[0].st_rol_cap.x + st_vs_pos_xtshp.x- VsCmp.x;
                pos.y = pos.y + st_cap_pos.y - Parent.list_xt[0].st_rol_cap.y + st_vs_pos_xtshp.y- VsCmp.y;
                pos.z = (id % 2 == 0) ? pos.z : -pos.z;
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0}放料，st_rol_cap.x{1},st_vs_pos_xtshp.x{2},VsCmp.x{3}", traybox.disc, st_rol_cap.x, st_vs_pos_xtshp.x, VsCmp.x));
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0}放料，st_rol_cap.y{1},st_vs_pos_xtshp.y{2},VsCmp.y{3}", traybox.disc, st_rol_cap.y, st_vs_pos_xtshp.y, VsCmp.y));
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0}放料，最终结果X{1},Y{2}", traybox.disc, pos.x, pos.y));
                //定位
                //VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, string.Format("{0}放料{1}: X:{2} Y:{3} X1:{4}", disc, traybox.disc, pos.x, pos.y, pos.a));
                //if (PT_SET.isopen_degree)
                //{
                //    res = MT.ZupMove(ref bquit, ref ax_x, pos.x, ref ax_y, pos.y, ref ax_u, -PT_SET.degree, ref traybox.ax_x, pos.a);
                //}
                //else
                //{
                res = MT.ZupMove(ref bquit, ref ax_x, pos.x, ref ax_y, pos.y, ref ax_u, PT_SET.isopen_degree ? -PT_SET.degree : 0, ref traybox.ax_x, pos.a);
                //}
                if (res != EM_RES.OK) return res;
                //零角度放料
                //pos.a = 0;
                if (PT_SET.isopen_degree/*&& traybox.IsNg*/)
                {
                    pos.a = -PT_SET.degree;
                }
                else
                {
                    pos.a = 0;
                }
                //放料
                res = Place(ref bquit, pos,true,IsDemo);
                if (res == EM_RES.OK)
                {
                    traybox.tray_cur.Push(XtMd, placepos[0].idx);                    
                    VAR.msg.AddMsg(Msg.EM_MSGTYPE.DBG, VAR.IsChinese?string.Format("{0}放料: 仓储:{1} 料盘:{2} 位置:{3} 模组结果{4} BC:{5}",disc, traybox.disc, traybox.tray_idx, placepos[0].idx, placepos[0].md.res, placepos[0].md.bardcode): string.Format("{0}Place : Traybox:{1} Tray:{3} Tray_idx:{4} Md_res:{5} Md_BarCode:{6}                 ({0}放料: 仓储:{2} 料盘:{3} 位置:{4} 模组结果{5} BC:{6})", disc,traybox.name, traybox.disc, traybox.tray_idx, placepos[0].idx, placepos[0].md.res, placepos[0].md.bardcode));
                    if (XtMd != null)
                    {
                        XtMd.bardcode = "No Code";
                    }
                    XtMd = null;
                }
                else return res;

                if (placepos.Count == 1)
                    return EM_RES.END;
            }
            else
            {
                return EM_RES.PARA_OUTOFRANG;
            }
            return EM_RES.OK;
        }


        #endregion

        #region 真空检测
        public EM_RES MovAndChkSensor(ref bool bquit,double stop_pos,double step,double speed,bool bwaiton)
        {
            cy_zk.SetOff();
            cy_zk.SetOn();
            EM_RES res = EM_RES.OK;
            int n = 0;
            do
            {
                if(bquit)res=EM_RES.ABORT;
                if(Math.Abs(stop_pos)>Math.Abs(ax_z.fenc_pos))
                 res = EM_RES.PARA_OUTOFRANG;
                if(bwaiton)
                {
                  if(cy_zk.isONByChkSen)
                  {
                       Thread.Sleep(30);
                      if(cy_zk.isONByChkSen)
                      {
                          res=EM_RES.OK;
                           break;
                      }
                  }                                        
                }
                else
                {
                     if(cy_zk.isOFFByChkSen)
                  {
                       Thread.Sleep(30);
                      if(cy_zk.isOFFByChkSen)
                      {
                          res=EM_RES.OK;
                           break;
                      }
                  }            
                }

                res = ax_z.JOG_Step(ref bquit, step > 0 ? AXIS.AX_DIR.P : AXIS.AX_DIR.N, Math.Abs(step), speed);
                double a = ax_z.fenc_pos;
                if(res!=EM_RES.OK)break;
                if(Math.Abs(step)>2)Thread.Sleep(20);
                else Thread.Sleep(50);

                Thread.Sleep(10);
                Application.DoEvents();   
                if(n++>2000)
                {
                    MessageBox.Show(VAR.IsChinese?"下压检测真空超时>20S!": "Vacuum detection timeout> 20S!\r\n下压检测真空超时>20S!");
                    res = EM_RES.ERR;
                    break;
                }
            }
            while (true);
            return res;
        }

        public EM_RES CaliZrefByZK(ref bool bquit,ST_XY pos, ref double zref, int count = 3)
        {
            EM_RES res=EM_RES.OK;
            ST_XY gopos=new ST_XY();
            int xtnum = id % 2;
            if(pos.x!=-10000&&pos.y!=-10000)
            {
                gopos = pos - this.Parent.list_xt[0].st_rol_cap.ToXY() + st_rol_cap.ToXY();
                //move
                res = MT.ZupMove(ref bquit, ref ax_x, gopos.x, ref ax_y, gopos.y);
                if (res != EM_RES.OK) return res;
            }
         
            //check zk
            cy_zk.SetOn();
            Thread.Sleep(100);
             if(cy_zk.isONByChkSen)
                {
                    Thread.Sleep(30);
                    if(cy_zk.isONByChkSen)
                    {
                       MessageBox.Show(VAR.IsChinese?"未开始校准已经感应到真空!\r\n 1.确认真空感应是否正常。\r\n 2.确认吸头是否粘有物料。": "A vacuum has been sensed without calibration! \r\n1. Make sure the vacuum sense is normal.\r\n 2. Confirm whether the tip is stuck with material.\r\n未开始校准已经感应到真空!\r\n 1.确认真空感应是否正常。\r\n 2.确认吸头是否粘有物料。");
                       cy_zk.SetOff();
                       return EM_RES.ERR;
                    }
                }                

            //check zref
            if (zref == 0) zref = ax_z.slp;

            //up till zk sensor on
            
            res = MovAndChkSensor(ref bquit, zref,xtnum==0?1:-1, 50, true);
            if (res != EM_RES.OK)
            {
                MessageBox.Show(VAR.IsChinese?"下探寻找真空感应点失败!\r\n 1.确认真空感应是否正常。\r\n 2.确认吸头正下方对准平台。\r\n3.修改初始基准值，尽量接近实际值。": "Failed to find the vacuum sensing point! \r\n 1. Check if the vacuum sensing is normal. \r\n 2. Make sure the tip is aligned directly below the platform. \r\n3. Modify the initial reference value as close to the actual value as possible.\r\n下探寻找真空感应点失败!\r\n 1.确认真空感应是否正常。\r\n 2.确认吸头正下方对准平台。\r\n3.修改初始基准值，尽量接近实际值。");
                MT.Move(ref bquit, ref ax_z, 0);
                cy_zk.SetOff();
                return res;
            }

            List<double> list_zref = new List<double>();
            list_zref.Clear();
            for (; count > 0; count--)
            {
                //up till zk sensor off
                res = MovAndChkSensor(ref bquit, 0, xtnum == 0 ? -0.1 : 0.1, 50, false);
                if (res != EM_RES.OK)
                {
                    MessageBox.Show(VAR.IsChinese?"抬升寻找真空感应点失败!\r\n 1.确认真空感应是否正常。\r\n 2.确认吸头是否粘有物料。": "Lifting failed to find the vacuum sensing point! \r\n  1. Check if the vacuum sensing is normal. \r\n  2. Check if the tip is sticky.\r\n抬升寻找真空感应点失败!\r\n 1.确认真空感应是否正常。\r\n 2.确认吸头是否粘有物料。");
                    MT.Move(ref bquit, ref ax_z, 0);
                    cy_zk.SetOff();
                    return res;
                }

                //down till zk sensor on
                res = MovAndChkSensor(ref bquit, zref, xtnum == 0 ? 0.05 : -0.05, 50, true);
                if (res != EM_RES.OK)
                {
                    MessageBox.Show(VAR.IsChinese?"下降寻找真空感应点失败!\r\n 1.确认真空感应是否正常。\r\n 2.确认吸头正下方对准载台或飞达平台。": "Failed to find the vacuum sensing point when descending! \r\n 1. Check if the vacuum sensing is normal. \r\n 2. Make sure that the suction head is aligned with the carrier or the platform directly below.\r\n下降寻找真空感应点失败!\r\n 1.确认真空感应是否正常。\r\n 2.确认吸头正下方对准载台或飞达平台。");
                    MT.Move(ref bquit, ref ax_z, 0);
                    cy_zk.SetOff();
                    return res;
                }
                list_zref.Add(ax_z.fenc_pos);
            }
            MT.Move(ref bquit, ref ax_z, 0);
            cy_zk.SetOff();
            
            zref = list_zref.Average();
            MessageBox.Show(VAR.IsChinese?string.Format("测高结果:{0}", zref): string.Format("Altimetry results:{0}\r\n测高结果:{0}", zref));
            return EM_RES.OK;
        }

        public EM_RES CaliZrefWLByZK(ref bool bquit, ST_XY pos, ref double zref, bool showmsg = true, int count = 3)
        {
            EM_RES res = EM_RES.OK;
            ST_XY gopos = new ST_XY();
            int xtnum = id % 2;
            if (pos.x != -10000 && pos.y != -10000)
            {
                gopos = pos - this.Parent.list_xt[0].st_rol_cap.ToXY() + st_rol_cap.ToXY();
                //move
                res = MT.ZupMove(ref bquit, ref ax_x, gopos.x, ref ax_y, gopos.y);
                if (res != EM_RES.OK) return res;
            }

            //check zk
            cy_zk.SetOn();
            Thread.Sleep(100);
            if (cy_zk.isONByChkSen)
            {
                Thread.Sleep(30);
                if (cy_zk.isONByChkSen)
                {
                    MessageBox.Show(VAR.IsChinese?"未开始校准已经感应到真空!\r\n 1.确认真空感应是否正常。\r\n 2.确认吸头是否粘有物料。": "A vacuum has been sensed without starting calibration! \r\n 1. Make sure the vacuum sense is normal. \r\n 2. Check if the tip is sticky.\r\n未开始校准已经感应到真空!\r\n 1.确认真空感应是否正常。\r\n 2.确认吸头是否粘有物料。");
                    cy_zk.SetOff();
                    return EM_RES.ERR;
                }
            }

            //check zref
            if (zref == 0) zref = ax_z.slp;

            //up till zk sensor on

            res = MovAndChkSensor(ref bquit, zref, xtnum == 0 ? 1 : -1, 50, true);
            if (res != EM_RES.OK)
            {
                MessageBox.Show(VAR.IsChinese?"下探寻找真空感应点失败!\r\n 1.确认真空感应是否正常。\r\n 2.确认吸头正下方对准平台。\r\n3.修改初始基准值，尽量接近实际值。": "Failed to find the vacuum sensing point! \r\n 1. Check if the vacuum sensing is normal. \r\n 2. Make sure the tip is aligned directly below the platform. \r\n3. Modify the initial reference value as close to the actual value as possible.\r\n下探寻找真空感应点失败!\r\n 1.确认真空感应是否正常。\r\n 2.确认吸头正下方对准平台。\r\n3.修改初始基准值，尽量接近实际值。");
                MT.Move(ref bquit, ref ax_z, 0);
                cy_zk.SetOff();
                return res;
            }

            List<double> list_zref = new List<double>();
            list_zref.Clear();
            for (; count > 0; count--)
            {
                ////up till zk sensor off
                //res = MovAndChkSensor(ref bquit, 0, xtnum == 0 ? -0.1 : 0.1, 50, false);
                //if (res != EM_RES.OK)
                //{
                //    MessageBox.Show("抬升寻找真空感应点失败!\r\n 1.确认真空感应是否正常。\r\n 2.确认吸头是否粘有物料。");
                //    MT.Move(ref bquit, ref ax_z, 0);
                //    cy_zk.SetOff();
                //    return res;
                //}
                cy_zk.SetOff();
                Thread.Sleep(200);
                MT.Move(ref bquit, ref ax_z, xtnum == 0 ? ax_z.fenc_pos - 1 : ax_z.fenc_pos + 1);
                
                //down till zk sensor on
                res = MovAndChkSensor(ref bquit, zref, xtnum == 0 ? 0.05 : -0.05, 50, true);
                if (res != EM_RES.OK)
                {
                    MessageBox.Show(VAR.IsChinese?"下降寻找真空感应点失败!\r\n 1.确认真空感应是否正常。\r\n 2.确认吸头正下方对准载台或飞达平台。": "Failed to find the vacuum sensing point when descending! \r\n 1. Check if the vacuum sensing is normal. \r\n 2. Make sure that the suction head is aligned with the carrier or the platform directly below.\r\n下降寻找真空感应点失败!\r\n 1.确认真空感应是否正常。\r\n 2.确认吸头正下方对准载台或飞达平台。");
                    MT.Move(ref bquit, ref ax_z, 0);
                    cy_zk.SetOff();
                    return res;
                }
                list_zref.Add(ax_z.fenc_pos);
            }
            MT.Move(ref bquit, ref ax_z, 0);
            cy_zk.SetOff();

            zref = list_zref.Average();
            if(showmsg) MessageBox.Show(VAR.IsChinese?string.Format("测高结果:{0}", zref): string.Format("Altimetry results:{0}\r\n测高结果:{0}", zref));
            return EM_RES.OK;
        }

         EM_RES PickTrayChkErrShow()
        {
            MT.ST_WARN warn = new MT.ST_WARN();
            warning fr_warn = new warning();
            warn.ok_txt = VAR.IsChinese ? "继续" : "GoOn";
            warn.cancle_txt = VAR.IsChinese ? "停止" : "Stop";
            warn.title = VAR.IsChinese ? "提示:吸头真空异常" : "Tip: xt State Error";
            VAR.sys_inf.Set(EM_ALM_STA.WAR_YELLOW_FLASH, warn.title, 10, true);
            warn.msg = warn.title;
            warn.lb_msg = string.Format(  $"当前吸头名字：{disc} ，取料后复检真空异常，请确认真空气压阈值，检查吸头有无残胶等" +
                $"\r\n " + "按继续则运行。否者按停止则停机");
            DialogResult logres = MT.Display_frwarn(fr_warn, warn, ERR_ALM.EmErrItem.MaterialPosErr);
            if (logres == DialogResult.OK)
            {
                 return EM_RES.OK; 
            }
            else return EM_RES.ERR;
        }
        #endregion

        //飞拍(相机1任务列表/位置列表，相机2任务列表/位置列表，相机3任务列表/位置列表，终点位置)

        //根据取放位置计算上相机位置

    }
}