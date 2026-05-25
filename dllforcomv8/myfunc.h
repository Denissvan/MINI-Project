#ifndef MYFUNC_H
#define MYFUNC_H

#ifndef _MSC_VER
#include <stdint.h>
#else
#include "stdint.h"
#endif


//#if defined(_MSC_VER)
# if defined(DLLBUILD)
/* define DLLBUILD when building the DLL */
#  define MYFUNC_API __declspec(dllexport)
# else
#  define MYFUNC_API __declspec(dllimport)
# endif
//#else
//# define MYFUNC_API
//#endif

#ifdef  __cplusplus
# define _BEGIN_DECLS  extern "C" {
# define _END_DECLS    }
#else
# define _BEGIN_DECLS
# define _END_DECLS
#endif

_BEGIN_DECLS

//成功
#define RES_OK 0
//参数错误
#define RES_ERR -1
//参数错误
#define RES_ERR_PARAM -2
//取消
#define RES_ERR_QUIT -3
//超时
#define RES_ERR_TIMEOUT -4
//掉线
#define RES_ERR_LINK -5
//SESSION_LOST: test software restarted and current PC session was lost
#define RES_ERR_SESSION_LOST -7
//LINK_LOST: test PC communication stalled for a long time
#define RES_ERR_LINK_LOST -8

typedef struct _DeviceStruct
{
	int angle;		//下旋角度	【测试软件提供】			
	int lenpitch;	//螺距		【测试软件提供】			
	int spaceAngle; //半间隙角度  【测试软件提供】			
}DeviceStruct;

typedef struct _InfoData
{
	int slaveid;
	char ip[20];
	int s;
	int start_mark;
	int result_mark;
	int pos_idx;
	int res_num;
	uint16_t res[32];
	char barcode[32 * 32 + 32 + 1];
	int status;
}InfoData;

//测试软件使用
MYFUNC_API BOOL GetDeviceStatus(int *pnDeviceStatus);
MYFUNC_API BOOL SetDeviceStatus(int status, int ChN, int *testResult, DeviceStruct *pDeviceParam);
MYFUNC_API BOOL GetBarcode(char *pch);
MYFUNC_API BOOL GetFuncQR(char *pch);

//设备用
MYFUNC_API int Init(const char * filename);
MYFUNC_API int MBClose();
MYFUNC_API int StartTestFlow(int id, int status, char* barcode);
MYFUNC_API int WaitTestResult(int id, int *status, int *res, int *NumofResult, BOOL *bquit, int delay);
MYFUNC_API int WaitTestResultA(int id, int *status, int *res, int *NumofResult, DeviceStruct *pDeviceParam, BOOL *bquit, int delay,char* barcode);
MYFUNC_API int NextTest(int id, int status, int delay, BOOL *bquit);
MYFUNC_API int GetInfo(int id, InfoData *pdat);
MYFUNC_API int GetStatus(int id, int *sta);
MYFUNC_API int ResetTest(int id);

_END_DECLS

#endif  /* MYFUNC_H */
