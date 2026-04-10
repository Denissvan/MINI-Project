#include "stdafx.h"
#include "myfunc.h"
#include "modbus.h"
#include <stdlib.h>
#include <Ws2tcpip.h>
#include <io.h> 

#pragma comment(lib, "Ws2_32.lib")

#include <stdio.h>
#include <windows.h>
#include <process.h>
#include <tchar.h>  
#include <Shlwapi.h>  


void AppendDebugLog(const TCHAR *msg);

#define DebugStr2(fmt,var1,var2) {TCHAR sOut[2048];sprintf_s(sOut,2048,_T(fmt),var1,var2);OutputDebugString(sOut);AppendDebugLog(sOut);}  
#define DebugStr3(fmt,var1,var2,var3) {TCHAR sOut[2048];sprintf_s(sOut,2048,_T(fmt),var1,var2,var3);OutputDebugString(sOut);AppendDebugLog(sOut);}  
//#define DebugStr4(fmt,var1,var2,var3,var4) {TCHAR sOut[2048];sprintf_s(sOut,2048,_T(fmt),var1,var2,var3,var4);OutputDebugString(sOut);AppendDebugLog(sOut);}  
#define DebugStr5(fmt,var1,var2,var3,var4,var5) {TCHAR sOut[2048];sprintf_s(sOut,2048,_T(fmt),var1,var2,var3,var4,var5);OutputDebugString(sOut);AppendDebugLog(sOut);}  
#define BITS_ADDR 0
#define BITS_NB 0
#define INPUT_BITS_ADDR 0
#define INPUT_BITS_NB 0
#define INPUT_REG_ADDR 0
#define INPUT_REG_NB 0
#define INF_REG_ADDR 0
#define INF_CNT 16
#define INF_REG_NB (INF_CNT*32+4)
#define RESULT_REG_ADDR 1000
#define RESULT_REG_NB 256

#define REG_IDX_MARK 0
#define REG_IDX_STATUS 1
#define REG_IDX_PARAM 2
#define REG_IDX_DAT_LEN 3
#define REG_IDX_DAT 4

#define MARK_RESET 5
#define MARK_READY 1
#define MARK_ACK 2
#define MARK_NEXT 3
#define MARK_END 4
#define MARK_RESETTEST 6

#define SLAVE_BUF 32

typedef struct _Device
{
	BOOLEAN bcom;
	BOOLEAN bInit;
	char com[sizeof("COM10")];
	int baudrate;
	int data_bit;
	int stop_bit;
	char parity;
	char ip[sizeof("255.255.255.255")];
	int port;
	int s;
	int ns;
	int quit;
	int errcnt;
	HANDLE hListen;
	HANDLE hServer;
	BOOLEAN buse;
	BYTE slaveid;
	modbus_t *mb;
	uint8_t *query;
	modbus_mapping_t *mb_map_inf;
	modbus_mapping_t *mb_map_result;
	int header_length;
}Device;
Device Dev;

typedef struct _SlaveData
{
	int slaveid;
	char ip[20];
	int s;
	int status;
	int sec;
	modbus_mapping_t *map_inf;
	modbus_mapping_t *map_result;
}SlaveData;
SlaveData SlaveDat[SLAVE_BUF];

int Tick = 0;
int TickCnt = 0;
boolean IsFirstRun = TRUE;

SlaveData *GetSlaveDatByID(int id)
{
	for (int i = 0; i < SLAVE_BUF; i++)
	{
		if (SlaveDat[i].slaveid == id)
		{
			return &SlaveDat[i];
		}
	}
	return NULL;
}

//windows???????dll????
void GetDllPath(const char * filename, TCHAR *fullpath)
{
	MEMORY_BASIC_INFORMATION mbi;	
	GetModuleFileName(((VirtualQuery(GetDllPath, &mbi, sizeof(mbi)) != 0) ? (HMODULE)mbi.AllocationBase : NULL), fullpath, MAX_PATH);
	PathRemoveFileSpec(fullpath);
	PathAppend(fullpath, _T(filename));
}

DWORD g_lastNoDataLogTick[256] = { 0 };
int g_lastNoDataStatus[256] = { 0 };
int g_lastNoDataParam[256] = { 0 };
int g_lastReturnStatus[256] = { 0 };
int g_lastReturnNum[256] = { 0 };
int g_lastReturnMark[256] = { 0 };
int g_lastReturnParam[256] = { 0 };

void AppendDebugLog(const TCHAR *msg)
{
	if (msg == NULL || msg[0] == 0)return;
	TCHAR logpath[MAX_PATH];
	TCHAR prefix[64];
	SYSTEMTIME st;
	GetLocalTime(&st);
	sprintf_s(prefix, 64, "[%04d-%02d-%02d %02d:%02d:%02d.%03d] ", st.wYear, st.wMonth, st.wDay, st.wHour, st.wMinute, st.wSecond, st.wMilliseconds);
	GetDllPath("dllforcomv8_debug.txt", logpath);
	HANDLE hFile = CreateFile(logpath, FILE_APPEND_DATA, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	if (hFile == INVALID_HANDLE_VALUE)return;
	DWORD written = 0;
	DWORD prefixLen = (DWORD)(_tcslen(prefix) * sizeof(TCHAR));
	DWORD writeLen = (DWORD)(_tcslen(msg) * sizeof(TCHAR));
	WriteFile(hFile, prefix, prefixLen, &written, NULL);
	WriteFile(hFile, msg, writeLen, &written, NULL);
	CloseHandle(hFile);
}


int GetConfig(const char * filename)
{
	DebugStr2("DevCfg Path: %s%s\n", filename, "");

	if (_access(filename, 0))
	{
		filename = "C:\\DevCfg.ini";
	}

	Dev.bcom = 1;
	int len = GetPrivateProfileString("CONFIG", "COM", "", Dev.com, sizeof(Dev.com), filename);
	if (len > 0)
	{
		Dev.baudrate = GetPrivateProfileInt("CONFIG", "BAUD", 9600, filename);
		Dev.data_bit = GetPrivateProfileInt("CONFIG", "DATABIT", 8, filename);
		Dev.stop_bit = GetPrivateProfileInt("CONFIG", "STOPBIT", 1, filename);
		Dev.parity = GetPrivateProfileInt("CONFIG", "PARITY", 'N', filename);
	}
	len = GetPrivateProfileString("CONFIG", "IP", "127.0.0.1", Dev.ip, sizeof(Dev.ip), filename);
	{
		Dev.bcom = 0;
		Dev.port = GetPrivateProfileInt("CONFIG", "PORT", 502, filename);
	}
	Dev.slaveid = GetPrivateProfileInt("CONFIG", "ID", 0, filename);

	if (strlen(Dev.com) < 3 && strlen(Dev.ip) < 3)
		return RES_ERR_PARAM;
	else
		return RES_OK;
}

//static void close_sigint(int dummy)
//{
//	if (Dev.s != -1) {
//		close(Dev.s);
//	}
//	modbus_free(Dev.mb);
//	modbus_mapping_free(Dev.mb_map_inf);
//	modbus_mapping_free(Dev.mb_map_result);
//	exit(dummy);
//}

//???????????
unsigned int __stdcall ThreadListen(PVOID pM)
{
	//COM??Master?????????
	if (Dev.bcom || Dev.slaveid != 0)return 0;

	int master_socket;
	int rc;
	fd_set refset;
	fd_set rdset;
	/* Maximum file descriptor number */
	int fdmax;

	//signal(SIGINT, close_sigint);

	/* Clear the reference set of socket */
	FD_ZERO(&refset);
	/* Add the server socket */
	FD_SET(Dev.s, &refset);

	/* Keep track of the max file descriptor */
	fdmax = Dev.s;

	DebugStr2("[DLL]ListThread ID=%4d,s=%d\n", GetCurrentThreadId(), Dev.s);

	//#define THREAD_NUM 10
	//HANDLE handle[THREAD_NUM];
	int th_idx = 0;
	int trycnt = 0;
	while (Dev.quit == 0)
	{
		rdset = refset;
		if (select(fdmax + 1, &rdset, NULL, NULL, NULL) == -1) 
		{
			perror("[DLL]Server select() failure.");
			DebugStr2("[DLL]Server sleect error max=%4d,s=%d\n", fdmax, Dev.s);
			//close_sigint(1);
		}

		/* Run through the existing connections looking for data to be
		* read */
		for (master_socket = 0; master_socket <= fdmax; master_socket++) {

			if (!FD_ISSET(master_socket, &rdset)) 
			{
				continue;
			}

			if (master_socket == Dev.s) 
			{
				/* A client is asking a new connection */
				int addrlen;
				struct sockaddr_in clientaddr;
				int newfd;

				/* Handle new connections */
				addrlen = sizeof(clientaddr);
				memset(&clientaddr, 0, sizeof(clientaddr));
				newfd = accept(Dev.s, (struct sockaddr *)&clientaddr, &addrlen);
				if (newfd == -1)
				{
					perror("Server accept() error");
					DebugStr2("[DLL]Server accept error %s,s=%d\n", "", Dev.s);
				}
				else 
				{
					FD_SET(newfd, &refset);
					if (newfd > fdmax) 
					{
						/* Keep track of the maximum */
						fdmax = newfd;
					}			

					char guest_ip[20];
					inet_ntop(AF_INET, &clientaddr.sin_addr, guest_ip, sizeof(guest_ip));
					DebugStr2("[DLL]New connect %s,%d\n", guest_ip, newfd);
					//printf("New connection from %s:%d on socket %d\n", guest_ip, ntohs((*(struct sockaddr_in *)&clientaddr).sin_port), newfd);
				}
			}
			else 
			{				
				struct sockaddr_in clientaddr;
				int addrlen = sizeof(clientaddr);
				memset(&clientaddr, 0, sizeof(clientaddr));				
				modbus_set_socket(Dev.mb, master_socket);
				rc = modbus_receive(Dev.mb, Dev.query);
				getpeername(master_socket, (SOCKADDR *)&clientaddr, &addrlen);
				char guest_ip[20];
				inet_ntop(AF_INET, &clientaddr.sin_addr, guest_ip, sizeof(guest_ip));

				//debug out
				char chtmp[1024] = { 0 };
				if (rc > Dev.header_length + 1)
				{
					int len = sprintf_s(chtmp, 8, "%02X", Dev.query[Dev.header_length]);
					for (int i = 1; i < rc - Dev.header_length && len <1024 ; i++)
					{
						len += sprintf_s(chtmp + len, 8, " %02X", Dev.query[Dev.header_length + i]);
					}
					
				}	
				DebugStr5("[DLL_RX_%02X]%s,%d,rc=%d,%s\n", Dev.query[Dev.header_length],guest_ip, master_socket, rc, chtmp);				

				if (rc > 0)
				{
					SlaveData *pdat = NULL;
					//???????????
					for (int i = 0; i < SLAVE_BUF; i++)
					{
						if (SlaveDat[i].s == master_socket)
						{
							pdat = &SlaveDat[i];
							break;
						}
					}
					//???????????????????
					if (pdat == NULL)
					{
						//????????????
						int cursec = (INT)time(NULL);
						int maxsec = 0;
						int maxId = -1;
						for (int i = 0; i < SLAVE_BUF; i++)
						{
							if (SlaveDat[i].s <= 0)
							{
								pdat = &SlaveDat[i];
								break;
							}
							//????????????
							if ((cursec - SlaveDat[i].sec) > maxsec)
							{
								maxsec = cursec - SlaveDat[i].sec;
								maxId = i;
							}
						}

						//??????????????????????????????
						if (pdat == NULL)
						{
							SlaveDat[maxId].slaveid = -1;
							SlaveDat[maxId].s = -1;
							SlaveDat[maxId].ip[0] = 0;
							pdat = &SlaveDat[maxId];
							DebugStr2("[DLL]Clear SlaveDat??ID=%d,T=%d\n", maxId, maxsec);
						}
					}

					if (pdat == NULL)
					{
						DebugStr2("[DLL]Data Full??%s,%d\n", guest_ip, master_socket);
						continue;
					}
					
					if (pdat->map_inf == NULL)
					{
						pdat->map_inf = modbus_mapping_new_start_address(BITS_ADDR, BITS_NB, INPUT_BITS_ADDR, INPUT_BITS_NB,
							INF_REG_ADDR, INF_REG_NB,
							INPUT_REG_ADDR, INPUT_REG_NB);
						if (pdat->map_inf == NULL)
						{
							//fprintf(stderr, "Failed to allocate the inf mapping: %s\n", modbus_strerror(errno));
							DebugStr2("[DLL]Failed to allocate the inf mapping %s%s\n", "", "");
							return -3;
						}
					}

					if (pdat->map_result == NULL)
					{
						pdat->map_result = modbus_mapping_new_start_address(BITS_ADDR, BITS_NB, INPUT_BITS_ADDR, INPUT_BITS_NB,
							RESULT_REG_ADDR, RESULT_REG_NB,
							INPUT_REG_ADDR, INPUT_REG_NB);
						if (pdat->map_result == NULL)
						{
							//fprintf(stderr, "Failed to allocate the inf mapping: %s\n", modbus_strerror(errno));
							DebugStr2("[DLL]Failed to allocate the result mapping %s%s\n", "", "");
							return -3;
						}
					}	
					
					/*if (MODBUS_GET_INT16_FROM_INT8(Dev.query, Dev.header_length + 1) < (Dev.mb_map_inf->start_registers + Dev.mb_map_inf->nb_registers))
						modbus_reply(Dev.mb, Dev.query, rc, Dev.mb_map_inf);
					else
						modbus_reply(Dev.mb, Dev.query, rc, Dev.mb_map_result);*/

					if (MODBUS_GET_INT16_FROM_INT8(Dev.query, Dev.header_length + 1) < (pdat->map_inf->start_registers + pdat->map_inf->nb_registers))
						rc=modbus_reply(Dev.mb, Dev.query, rc, pdat->map_inf);
					else
						rc=modbus_reply(Dev.mb, Dev.query, rc, pdat->map_result);

					pdat->s = master_socket;
					strcpy_s(pdat->ip, sizeof(guest_ip), guest_ip);
					pdat->slaveid = Dev.query[Dev.header_length - 1];
					pdat->sec = (INT)time(NULL);

				}
				else if (rc == -1) 
				{
					/* This example server in ended on connection closing or
					* any errors. */
					//printf("Connection closed on socket %d\n", master_socket);
					DebugStr2("[DLL]Connection closed on socket %d,%d\n", master_socket, trycnt++);
					if (trycnt>5)
					{
						trycnt = 0;
						closesocket(master_socket);
						Sleep(1000);
					}

					for (int i = 0; i < SLAVE_BUF; i++)
					{
						if (SlaveDat[i].s == master_socket)
						{
							SlaveDat[i].slaveid = -1;
							SlaveDat[i].s = -1;
							SlaveDat[i].ip[0] = 0;				

						    //??????????
							if (SlaveDat[i].map_result != NULL)
							{
								//SlaveDat[i].map_result->tab_registers[REG_IDX_MARK] = MARK_RESET;//mark
								SlaveDat[i].map_result->tab_registers[REG_IDX_DAT_LEN] = 0;//num
								int* pres = (int*)&SlaveDat[i].map_result->tab_registers[REG_IDX_DAT];
								for (int n = 0; n<INF_CNT; n++)
								{
									pres[n] = -1;
								}
							}					

							//break;
						}
					}

					/* Remove from reference set */
					FD_CLR(master_socket, &refset);

					if (master_socket == fdmax) {
						fdmax--;
					}
				}
			}
		}
	}
	DebugStr2("Thread ID=%4d,s=%d,End\n", GetCurrentThreadId(), Dev.s);
	Dev.hListen = NULL;
	return 0;
}

//?????
#define STA_READY 1
#define STA_TEST 2
#define STA_WAIT 3
#define STA_QUIT 4
UCHAR sta = STA_READY;
UCHAR idx = 0;
HANDLE hStatus;
int block;
#define BARCODEBUF (16 * 32 *2)
char barcode[BARCODEBUF]="";
unsigned int __stdcall ThreadStatus(PVOID pM)
{
	//slave ?????
	if (Dev.slaveid == 0)return 0;
	DebugStr2("[DLL]ThreadStatus Start.ID=%4d,s=%d\n", GetCurrentThreadId(), Dev.s);
	uint16_t temp;
	int trycnt = 0;
	while (Dev.quit == 0)
	{
		Sleep(100);
		if (block)continue;
		block = 1;
		temp = sta;
		temp = temp << 8 |idx++;
		int rc = modbus_write_register(Dev.mb, Dev.mb_map_result->start_registers + REG_IDX_PARAM, temp);
		block = 0;		
		DebugStr2("[DLL_TS]ThreadStatus, id=%d,rc=%d\n", Dev.slaveid, rc);
		if (rc == -1)
		{
			trycnt++;
			if (trycnt > 5)// about 5sec
			{
				trycnt = 0;
				Dev.errcnt = 0;
				DebugStr2("[DLL]Set Dev Reconnect(ThreadStatus) %d,%d\n", Dev.s, Dev.slaveid);
				//reconnect
				Sleep(1000);
				modbus_close(Dev.mb);
				Sleep(1000);
				modbus_connect(Dev.mb);
				Sleep(1000);
			}
		}
		Sleep(500);
	}
	DebugStr2("ThreadStatus ID=%4d,s=%d,End\n", GetCurrentThreadId(), Dev.s);
	hStatus = NULL;
	return 0;
}
int GetStatus(int id,int *sta)
{
	SlaveData *pSlaveDat = GetSlaveDatByID(id);
	if (pSlaveDat == NULL)return RES_ERR;
	if (sta != NULL && pSlaveDat->map_result != NULL)
	{
		*sta = pSlaveDat->map_result->tab_registers[REG_IDX_PARAM];
		return RES_OK;
	}
	else
	{
		if (sta != NULL)*sta = -1;
		return RES_ERR;
	}

}
//?????????
int Init(const char * filename)
{
	Dev.quit = FALSE;
	if (Dev.bInit == TRUE && Dev.slaveid >0 && hStatus == NULL)
	{
		hStatus = (HANDLE)_beginthreadex(NULL, 0, ThreadStatus, &Dev.s, 0, NULL);
	}

	if (Dev.bInit == TRUE)return RES_OK;

	TCHAR szAbsPathFile[MAX_PATH + 1] = { 0 };
	GetDllPath(filename, szAbsPathFile);
	int ret = GetConfig(szAbsPathFile);
	if (ret != 0)return ret;

	if (Dev.bcom)
	{
		Dev.mb = modbus_new_rtu(Dev.com, Dev.baudrate, Dev.parity, Dev.data_bit, Dev.stop_bit);
		Dev.query = (uint8_t *)malloc(MODBUS_RTU_MAX_ADU_LENGTH);
		memset(Dev.query, 0, MODBUS_RTU_MAX_ADU_LENGTH * sizeof(uint8_t));
	}
	else if (strlen(Dev.ip) > 3)
	{
		Dev.mb = modbus_new_tcp(Dev.ip, Dev.port);
		Dev.query = (uint8_t*)malloc(MODBUS_TCP_MAX_ADU_LENGTH);
		memset(Dev.query, 0, MODBUS_TCP_MAX_ADU_LENGTH * sizeof(uint8_t));
	}
	else return RES_ERR;

	if (Dev.mb == NULL)
	{
		DebugStr2("[DLL]new modbus error %s%s\n", "", "");
		return RES_ERR_PARAM;
	}

	Dev.header_length = modbus_get_header_length(Dev.mb);
	modbus_set_slave(Dev.mb, Dev.slaveid);

	Dev.mb_map_inf = modbus_mapping_new_start_address(BITS_ADDR, BITS_NB, INPUT_BITS_ADDR, INPUT_BITS_NB,
		INF_REG_ADDR, INF_REG_NB,
		INPUT_REG_ADDR, INPUT_REG_NB);
	if (Dev.mb_map_inf == NULL)
	{
		//fprintf(stderr, "Failed to allocate the inf mapping: %s\n", modbus_strerror(errno));
		DebugStr2("[DLL]Failed to allocate the inf mapping: %s%s\n", modbus_strerror(errno),"");
		modbus_free(Dev.mb);
		Dev.mb = NULL;
		return RES_ERR_PARAM;
	}

	Dev.mb_map_result = modbus_mapping_new_start_address(BITS_ADDR, BITS_NB, INPUT_BITS_ADDR, INPUT_BITS_NB,
		RESULT_REG_ADDR, RESULT_REG_NB,
		INPUT_REG_ADDR, INPUT_REG_NB);
	if (Dev.mb_map_result == NULL)
	{
		//fprintf(stderr, "Failed to allocate the result mapping: %s\n", modbus_strerror(errno));
		DebugStr2("[DLL]Failed to allocate the result mapping: %s%s\n", modbus_strerror(errno), "");
		modbus_free(Dev.mb);
		Dev.mb = NULL;
		return RES_ERR_PARAM;
	}

	//????????
	Dev.mb_map_result->tab_registers[REG_IDX_MARK] = MARK_RESETTEST;//mark
	Dev.mb_map_result->tab_registers[REG_IDX_STATUS] = MARK_RESETTEST;//status
	Dev.mb_map_result->tab_registers[REG_IDX_PARAM] = 0;//param
	Dev.mb_map_result->tab_registers[REG_IDX_DAT_LEN] = 0;//num
	int* pres = (int*)&Dev.mb_map_result->tab_registers[REG_IDX_DAT];
	for (int n = 0; n < SLAVE_BUF; n++)
	{
		pres[n] = -1;
	}

	if (!Dev.bcom && Dev.slaveid == 0)
	{
		Dev.s = modbus_tcp_listen(Dev.mb, 3);
		//???????????
		Dev.quit = 0;
		Dev.hListen = (HANDLE)_beginthreadex(NULL, 0, ThreadListen, &Dev.s, 0, NULL);
	}
	else
	{
		int rc = modbus_connect(Dev.mb);
		if (rc == -1)
		{
			DebugStr2("[DLL]Unable to connect, %s%s\n", modbus_strerror(errno),"");
			modbus_free(Dev.mb);
			Dev.mb = NULL;
			return RES_ERR;
		}
	}
	Dev.bInit = TRUE;
	Sleep(300);	
	Sleep(300);
	Sleep(300);
	return RES_OK;
}
//???
int MBClose()
{
	DebugStr2("[DLL]Close%s%s\n", "", "");
	Dev.quit = 1;
	Sleep(100);	
	modbus_close(Dev.mb);
	if(Dev.hListen!=NULL)
		WaitForSingleObject(Dev.hListen, INFINITE);
	if (Dev.hServer != NULL)
		WaitForSingleObject(Dev.hServer, INFINITE);

	if (Dev.mb != NULL)
	{
		modbus_free(Dev.mb);
		Dev.mb = NULL;
	}
	if (Dev.mb_map_inf != NULL)
	{
		modbus_mapping_free(Dev.mb_map_inf);
		Dev.mb_map_inf = NULL;
	}
	if (Dev.mb_map_result != NULL)
	{
		modbus_mapping_free(Dev.mb_map_result);
		Dev.mb_map_result = NULL;
	}
	for (int n = 0; n < SLAVE_BUF; n++)
	{
		if (SlaveDat[n].map_inf != NULL)
		{
			modbus_mapping_free(SlaveDat[n].map_inf);
			SlaveDat[n].map_inf = NULL;

		}
		if (SlaveDat[n].map_result != NULL)
		{
			modbus_mapping_free(SlaveDat[n].map_result);
			SlaveDat[n].map_result = NULL;
		}
	}
	return RES_OK;
}
//??????????????????
BOOL GetDeviceStatus(int *pnDeviceStatus)
{
	Init("DevCfg.ini");
	sta = STA_READY;
	if (block)Sleep(100);
	if (block)Sleep(100);
	if (block)Sleep(100);
	block = 1;
	//reset barcode
	for (int n =0; n < BARCODEBUF; n++)barcode[n] = 0;
	int t = 0;
	for (int i = 0; i < 10000; i++)
	{
		//??????????????????????????
		int temp = sta;
		if (++t > 10)
		{
			t = 0;
			temp = temp << 8 | idx++;
			DebugStr2("[DLL_TX]GetDeviceStatus, ID=%d,%d\n", Dev.slaveid, idx);
			Dev.mb_map_result->tab_registers[REG_IDX_MARK] = MARK_END;//mark
			Dev.mb_map_result->tab_registers[REG_IDX_PARAM] = temp;//param	

			// reset result
			if (Dev.mb_map_inf->tab_registers[REG_IDX_STATUS] == 0)
			{
				int* pres = (int*)&Dev.mb_map_result->tab_registers[REG_IDX_DAT];
				for (int n = 0; n < INF_CNT; n++)
				{
					pres[n] = -1;
				}
			}

			if (IsFirstRun)
			{
				DebugStr2("[DLL_TX]GetDeviceStatus FirstRun, ID=%d,%d\n", Dev.slaveid, idx);
				IsFirstRun = FALSE;
				Dev.mb_map_result->tab_registers[REG_IDX_STATUS] = 0;//status
				Dev.mb_map_result->tab_registers[REG_IDX_DAT_LEN] = 0;//num
				int* pres = (int*)&Dev.mb_map_result->tab_registers[REG_IDX_DAT];
				for (int n = 0; n<INF_CNT; n++)
				{
					pres[n] = -1;
				}
				//clear barcode
				for (int m = 0; m < BARCODEBUF; m++)barcode[m] = "";
				modbus_write_registers(Dev.mb, Dev.mb_map_result->start_registers, 4 + INF_CNT * 2, Dev.mb_map_result->tab_registers);
			}
			else
			{
				//modbus_write_registers(Dev.mb, Dev.mb_map_result->start_registers, 3, Dev.mb_map_result->tab_registers);
				modbus_write_registers(Dev.mb, Dev.mb_map_result->start_registers, 4 + INF_CNT * 2, Dev.mb_map_result->tab_registers);
			}			
		}

		//read status
		int rc = modbus_read_registers(Dev.mb, Dev.mb_map_inf->start_registers, 4, Dev.mb_map_inf->tab_registers);
		//??????????????????????????
		if(rc == -1)
		{
			if (++Dev.errcnt>30)
			{
				Dev.errcnt = 0;
				DebugStr2("[DLL_TX]Set Dev Reconnect(GetDeviceStatus) %d,%d\n", Dev.s, Dev.slaveid);
				//reconnect
				Sleep(1000);
				modbus_close(Dev.mb);
				Sleep(1000);
				modbus_connect(Dev.mb);
				Sleep(1000);
			}
		}

		//??????
		if (Dev.mb_map_inf->tab_registers[REG_IDX_MARK] == MARK_RESETTEST)
		{
			DebugStr2("[DLL_TX]GetDeviceStatus ResetTest %d,%d\n", Dev.s, Dev.slaveid);

			Dev.mb_map_result->tab_registers[REG_IDX_MARK] = MARK_RESET;
			Dev.mb_map_result->tab_registers[REG_IDX_STATUS] = 0;
			Dev.mb_map_result->tab_registers[REG_IDX_PARAM] = 0;
			Dev.mb_map_result->tab_registers[REG_IDX_DAT_LEN] = 0;
			int* pres = (int*)&Dev.mb_map_result->tab_registers[REG_IDX_DAT];
			for (int n = 0; n < INF_CNT; n++)
			{
				pres[n] = -1;
			}
			modbus_write_registers(Dev.mb, Dev.mb_map_result->start_registers, 4 + INF_CNT * 2, Dev.mb_map_result->tab_registers);

			Dev.mb_map_inf->tab_registers[REG_IDX_MARK] = MARK_ACK;
			modbus_write_register(Dev.mb, Dev.mb_map_inf->start_registers, Dev.mb_map_inf->tab_registers[REG_IDX_MARK]);
			Sleep(100);
			continue;
		}

		if (Dev.mb_map_inf->tab_registers[REG_IDX_MARK] == MARK_READY)
		{
			//????????
			if (rc == 4)
			{
				Dev.errcnt = 0;
				//read barcode
				int res_num = Dev.mb_map_inf->tab_registers[REG_IDX_DAT_LEN];
				int addr = 4;
				if (res_num > 0)
				{
					res_num = res_num * 16;
					for (; res_num > 0;)
					{
						int len = 125;
						if (res_num < 125)len = res_num;
						rc = modbus_read_registers(Dev.mb, Dev.mb_map_inf->start_registers + addr, len, &Dev.mb_map_inf->tab_registers[addr]);
						if (rc != len)
						{
							DebugStr2("[DLL_TX]Get Dev Barcode error %d,%d\n", rc, res_num);
							break;
						}
						res_num -= rc;
						addr += rc;
					}
				}
				//????????
				if (res_num > 0)
				{
					Sleep(100);
					continue;
				}
			}

			//reset mark
			Dev.mb_map_inf->tab_registers[REG_IDX_MARK] = MARK_ACK;//mark
			int rc = modbus_write_register(Dev.mb, Dev.mb_map_inf->start_registers, Dev.mb_map_inf->tab_registers[REG_IDX_MARK]);
			if (rc != 1)continue;

			//get status
			*pnDeviceStatus = Dev.mb_map_inf->tab_registers[REG_IDX_STATUS];
			//pnDeviceStatus++;

			//get barcode
			strcpy_s(barcode, BARCODEBUF, (char*)&Dev.mb_map_inf->tab_registers[REG_IDX_DAT]);
			/*char* pch =(char*) &Dev.mb_map_inf->tab_registers[REG_IDX_DAT];
			for (int n = 0; n < Dev.mb_map_inf->tab_registers[REG_IDX_DAT_LEN] * 16; n++)
			{
				if (*pch != '\0')
				{
					*pnDeviceStatus = *pch;
					pnDeviceStatus++;
				}
				else if(*(pnDeviceStatus-1)!='#')
				{
					*pnDeviceStatus = '#';
					pnDeviceStatus++;
					n++;
				}
				pch++;
			}*/

			DebugStr2("[DLL_TX]Get barcode, %d,%s\n", *pnDeviceStatus, barcode);

			//reset result
			if (Dev.mb_map_inf->tab_registers[REG_IDX_STATUS] == 0)
			{
				Dev.mb_map_result->tab_registers[REG_IDX_MARK] = MARK_RESET;//mark
				Dev.mb_map_result->tab_registers[REG_IDX_STATUS] = 0;//status
				Dev.mb_map_result->tab_registers[REG_IDX_PARAM] = 0;//param
				Dev.mb_map_result->tab_registers[REG_IDX_DAT_LEN] = 0;//num
				int* pres = (int*)&Dev.mb_map_result->tab_registers[REG_IDX_DAT];
				for (int n = 0; n<INF_CNT; n++)
				{
					pres[n] = -1;
				}

				rc=modbus_write_registers(Dev.mb, Dev.mb_map_result->start_registers, 4 + INF_CNT * 2, Dev.mb_map_result->tab_registers);
				if (rc == -1)
				{
					DebugStr2("[DLL_TX]Reset resutl err, %d,%s\n", rc, "");
					Sleep(200);
					modbus_write_registers(Dev.mb, Dev.mb_map_result->start_registers, 4 + INF_CNT * 2, Dev.mb_map_result->tab_registers);
				}
			}
			sta = STA_TEST;
			block = 0;
			return TRUE;
		}
		Sleep(100);
	}
	*pnDeviceStatus = -1;
	block = 0;
	return FALSE;
}
//??????????????/???????
BOOL SetDeviceStatus(int status, int ChN, int *testResult, DeviceStruct *pDeviceParam)
{
	//if (sta == STA_QUIT)return FALSE;

	sta = STA_WAIT;
	//check param
	if (ChN > 0  && testResult == NULL || ChN > 32)return FALSE;

	//debug out
	char chtmp[1024] = { 0 };
	if (ChN > 0)
	{
		//int len = sprintf_s(chtmp, 8, "%d", testResult[0]);
		int len = 0;
		for (int i = 0; i < ChN && len <1024 - 8; i++)
		{
			len += sprintf_s(chtmp + len, 8, " %d", testResult[i]);
		}
	}	
	DebugStr3("[DLL_TX]SetDeviceStatus,sta=%d,n=%d,%s\n", status, ChN, chtmp);

	//??????????????????0
	ChN = status == 0 ? ChN : 0;

	Init("DevCfg.ini");
	if (block)Sleep(100);
	if (block)Sleep(100);
	if (block)Sleep(100);
	block = 1;
	Dev.errcnt = 0;
	for (int n = 0; n < 10000; n++)
	{
		//fill data
		int temp = sta;
		temp = temp << 8 | idx++;

		Dev.mb_map_result->tab_registers[REG_IDX_MARK] = MARK_READY;//mark
		Dev.mb_map_result->tab_registers[REG_IDX_STATUS] = status;//status
		Dev.mb_map_result->tab_registers[REG_IDX_PARAM] = temp;//param
		Dev.mb_map_result->tab_registers[REG_IDX_DAT_LEN] = ChN;//num
		//result
		int* pres = (int*)&Dev.mb_map_result->tab_registers[REG_IDX_DAT];
		for (int n = 0; (n < ChN) && (testResult != NULL); n++)
		{
			pres[n] = testResult[n];
		}

		//send
		int rc = modbus_write_registers(Dev.mb, Dev.mb_map_result->start_registers, REG_IDX_DAT + ChN * 2, Dev.mb_map_result->tab_registers);
		if (rc == REG_IDX_DAT + ChN * 2)
		{
			//Dev.errcnt = 0;
			//wait return
			for (int n = 0; n < 2000; n++)
			{
				Sleep(100);

				//????????
				Dev.errcnt++;
				if (Dev.errcnt == 20)
				{
					//Dev.errcnt = 0;
					DebugStr2("[DLL_TX]Set Dev Reconnect(SetDeviceStatus,1) %d,%d\n", Dev.s, Dev.slaveid);
					//reconnect
					Sleep(1000);
					modbus_close(Dev.mb);
					Sleep(1000);
					modbus_flush(Dev.mb);
					Sleep(200);
					modbus_connect(Dev.mb);
					Sleep(1000);
					Dev.errcnt = 0;
					modbus_write_registers(Dev.mb, Dev.mb_map_result->start_registers, REG_IDX_DAT + ChN * 2, Dev.mb_map_result->tab_registers);
				}

				////???????
				//if (Dev.errcnt > 60)
				//{
				//	DebugStr2("Retry connect abort %d,%d\n", Dev.s, Dev.slaveid);
 				//		sta = STA_QUIT;
				//	block = 0;
				//	return FALSE;
				//}

				//????????????
				//if (status != 0)
				{
					rc = modbus_read_registers(Dev.mb, Dev.mb_map_inf->start_registers, 1, Dev.mb_map_inf->tab_registers);
					if (rc != -1)
					{
						if (Dev.mb_map_inf->tab_registers[REG_IDX_MARK] == MARK_RESETTEST)
						{
							DebugStr2("[DLL]SetDev ResetTest %d,%d\n", Dev.s, Dev.slaveid);
							sta = STA_QUIT;
							block = 0;
							return FALSE;
						}
					}
				}

				//????????????
				rc = modbus_read_registers(Dev.mb, Dev.mb_map_result->start_registers, 1, Dev.mb_map_result->tab_registers);
				if (rc == -1)continue;
				Dev.errcnt = 0;

				if (Dev.mb_map_result->tab_registers[REG_IDX_MARK] == MARK_ACK)
				{
					//???????????????????????
					Dev.mb_map_result->tab_registers[REG_IDX_MARK] = (status == 0 ? MARK_END : MARK_NEXT);//status==0??????????????????????
					rc = modbus_write_register(Dev.mb, Dev.mb_map_result->start_registers, Dev.mb_map_result->tab_registers[REG_IDX_MARK]);
					if (rc == -1)continue;
					Dev.errcnt = 0;

					if(status == 0) sta = STA_READY;
					else sta = STA_TEST;
					block = 0;
					DebugStr2("[DLL_TX]Send Next Cmd,sta=%d%s\n", status, "");
					return TRUE;
				}
				else if (Dev.mb_map_result->tab_registers[REG_IDX_MARK] == (status == 0 ? MARK_END : MARK_NEXT))//status==0??????????????????????
				{
					//?????????????????????????
					if (status == 0) sta = STA_READY;
					else sta = STA_TEST;
					block = 0;
					DebugStr2("[DLL_TX]Rece Next Cmd,sta=%d%s\n", status, "");
					return TRUE;
				}
				else if (Dev.mb_map_result->tab_registers[REG_IDX_MARK] == MARK_RESET)
				{
					DebugStr2("[DLL_TX]Rece Reset Cmd,%d,%d\n", Dev.s, Dev.slaveid);
					//???????
					sta = STA_READY;
					block = 0;
					return FALSE;
				}
				//Sleep(100);

				//??????
				if ((n & 0x07) == 0x07)
				{
					int temp = sta;
					temp = temp << 8 | idx++;
					modbus_write_register(Dev.mb, Dev.mb_map_result->start_registers + REG_IDX_PARAM, temp);
				}
			}
		}		
		if (rc == -1)
		{
			//????????
			Dev.errcnt++;
			if (Dev.errcnt == 20)
			{
				//Dev.errcnt = 0;
				DebugStr2("[DLL_TX]Set Dev Reconnect(SetDeviceStatus,2) %d,%d\n", Dev.s, Dev.slaveid);
				//reconnect
				Sleep(1000);
				modbus_close(Dev.mb);
				Sleep(1000);
				modbus_flush(Dev.mb);
				Sleep(200);
				modbus_connect(Dev.mb);
				Sleep(1000);
			}

			////???????
			//if (Dev.errcnt > 60)
			//{
			//	DebugStr2("Retry connect abort %d,%d\n", Dev.s, Dev.slaveid);
			//	sta = STA_QUIT;
			//	block = 0;
			//	return FALSE;
			//}
		}
		Sleep(100);
	}
	DebugStr2("[DLL_TX]Set Dev Timeout %d,%d\n", Dev.s, Dev.slaveid);
	block = 0;
	return FALSE;
}
//??????????????????
BOOL GetBarcode(char *pch)
{
	if (pch == NULL)return FALSE;

	int n = 0;
	for (; n < BARCODEBUF && barcode[n] != 0; n++)
	{
		pch[n] = barcode[n];
	}
	if (n == 0)return FALSE;
	else return TRUE;
}
BOOL GetFuncQR(char *pch)
{
	return GetBarcode(pch);
}

//????????????????????
int StartTestFlow(int id, int status,char* barcode)
{
	Init("DevCfg.ini");	
	DebugStr3("[DLL_FLOW]StartTestFlow InitOk,id=%d,dev_s=%d,slave_count=%d\n", id, Dev.s, SLAVE_BUF);

	SlaveData *pDat = GetSlaveDatByID(id);
	if (pDat == NULL)return RES_ERR;
	DebugStr5("[DLL_FLOW]StartTestFlow Enter,id=%d,req_sta=%d,result_mark=%d,result_sta=%d,inf_mark=%d\n",
		id, status,
		pDat->map_result != NULL ? pDat->map_result->tab_registers[REG_IDX_MARK] : -1,
		pDat->map_result != NULL ? pDat->map_result->tab_registers[REG_IDX_STATUS] : -1,
		pDat->map_inf != NULL ? pDat->map_inf->tab_registers[REG_IDX_MARK] : -1);

	if (pDat->map_inf->tab_registers[REG_IDX_MARK] == MARK_RESET && pDat->map_result->tab_registers[REG_IDX_STATUS] != 0)
	{
		DebugStr2("[DLL]StartTestFlow,MARK_RESET && STATUS!=0, %d,%d\n", Dev.s, Dev.slaveid);
	}

	// ??MARK_END????????????????????????
	if (pDat->map_result->tab_registers[REG_IDX_MARK] != MARK_END)
	{
		DebugStr5("[DLL_FLOW]StartTestFlow Skip,id=%d,req_sta=%d,result_mark=%d,result_sta=%d,result_num=%d\n",
			id, status,
			pDat->map_result->tab_registers[REG_IDX_MARK],
			pDat->map_result->tab_registers[REG_IDX_STATUS],
			pDat->map_result->tab_registers[REG_IDX_DAT_LEN]);
		return RES_OK;
	}

	//Dev.mb_map_inf->tab_registers[1] = status;//status
	//Dev.mb_map_inf->tab_registers[2] = 0;//param

	pDat->map_inf->tab_registers[REG_IDX_STATUS] = status;//status
	pDat->map_inf->tab_registers[REG_IDX_PARAM] = 0;//param

	//fill barcode
	int barcode_num = 0;
	char *pch = (char*)&pDat->map_inf->tab_registers[REG_IDX_DAT];
	for (int idx = 0; idx < ((pDat->map_inf->nb_registers - REG_IDX_DAT) * 2 - 1) && *barcode != '\0'; idx++)
	{
		*pch = *barcode;
		if (*barcode == '#')barcode_num++;
		pch++;
		barcode++;
	}

	//end of bardcode
	*pch = '\0';	
	pDat->map_inf->tab_registers[REG_IDX_DAT_LEN] = barcode_num;//num of barcode
	pDat->map_inf->tab_registers[REG_IDX_MARK] = MARK_READY;//mark
	DebugStr5("[DLL_FLOW]StartTestFlow Send,id=%d,req_sta=%d,barcode_num=%d,inf_mark=%d,result_mark=%d\n",
		id, status, barcode_num,
		pDat->map_inf->tab_registers[REG_IDX_MARK],
		pDat->map_result->tab_registers[REG_IDX_MARK]);

	//wait for start
	for (int n = 0; n < 100; n++)
	{
		if (pDat->map_inf->tab_registers[REG_IDX_MARK] == MARK_ACK)
		{
			////reset mark
			//pDat->map_inf->tab_registers[0] = 0;		
			DebugStr5("[DLL_FLOW]StartTestFlow Ack,id=%d,req_sta=%d,loop=%d,result_mark=%d,result_sta=%d\n",
				id, status, n,
				pDat->map_result->tab_registers[REG_IDX_MARK],
				pDat->map_result->tab_registers[REG_IDX_STATUS]);
			return RES_OK;
		}
		Sleep(10);
	}

	// reset result
	int* pres = (int*)&Dev.mb_map_result->tab_registers[REG_IDX_DAT];
	for (int n = 0; n < INF_CNT; n++)
	{
		pres[n] = -1;
	}
	pDat->map_inf->tab_registers[REG_IDX_MARK] = MARK_RESET;
	DebugStr5("[DLL_FLOW]StartTestFlow Timeout,id=%d,req_sta=%d,inf_mark=%d,result_mark=%d,result_sta=%d\n",
		id, status,
		pDat->map_inf->tab_registers[REG_IDX_MARK],
		pDat->map_result->tab_registers[REG_IDX_MARK],
		pDat->map_result->tab_registers[REG_IDX_STATUS]);
	return RES_ERR_TIMEOUT;
	DebugStr5("[DLL_FLOW]WaitTestResult Timeout,id=%d,result_mark=%d,result_sta=%d,inf_mark=%d,last_param=%d\n",
		id,
		pDat->map_result->tab_registers[REG_IDX_MARK],
		pDat->map_result->tab_registers[REG_IDX_STATUS],
		pDat->map_inf->tab_registers[REG_IDX_MARK],
		pDat->map_result->tab_registers[REG_IDX_PARAM]);
	////send
	//for (int n = 0; n < 10; n++)
	//{
	//	int rc = modbus_write_registers(Dev.mb, Dev.mb_map_inf->start_registers, idx, Dev.mb_map_inf->tab_registers);
	//	if (rc == idx)return 0;
	//	else
	//	{
	//		modbus_close(Dev.mb);
	//		Sleep(100);
	//		modbus_connect(Dev.mb);
	//		Sleep(100);
	//	}
	//}
	//return -1;
}

//????????????????
int WaitTestResult(int id, int *status, int *res, int *NumofResult, BOOL *bquit, int delay)
{
	return WaitTestResultA(id, status, res, NumofResult, NULL,bquit, delay, NULL);
}
int WaitTestResultA(int id, int *status, int *res, int *NumofResult, DeviceStruct *pDeviceParam, BOOL *bquit, int delay,char* barcode)
{	
	if (NumofResult != NULL)*NumofResult = 0;
	if (status != NULL)*status = -1;
	if (res != NULL)*res = -1;

	//check param
	if (status == NULL || NumofResult == NULL || res == NULL)return RES_ERR_PARAM;

	//init
	Init("DevCfg.ini");

	if (bquit != NULL&&*bquit)return RES_ERR_QUIT;

	//check link
	SlaveData *pDat = GetSlaveDatByID(id);
	if (pDat == NULL)
	{
		return RES_ERR_LINK;
	}

	//wait for mark
	for (; delay >0; delay-=10)
	{
		//?????????(10sec)
		int t = pDat->map_result->tab_registers[REG_IDX_PARAM];
		t = t & 0xFF;
		if (Tick != t) TickCnt = 0;
		else TickCnt++;
		Tick = t;
		if (TickCnt>1000)
		{		
			DebugStr2("[DLL]%s%s\n", "ERR_LINK","");			
			return RES_ERR_LINK;
		}

		//?????
		if (bquit != NULL&&*bquit)
		{
			return RES_ERR_QUIT;
		}
		//???
		if (pDat->map_inf->tab_registers[REG_IDX_MARK] == MARK_RESETTEST)
		{
			DebugStr2("[DLL]StartTestFlow,MARK_RESET && STATUS!=0, %d,%d\n", Dev.s, Dev.slaveid);
			DebugStr5("[DLL_FLOW]WaitTestResult QuitByReset,id=%d,result_mark=%d,result_sta=%d,inf_mark=%d,delay=%d\n",
				id,
				pDat->map_result->tab_registers[REG_IDX_MARK],
				pDat->map_result->tab_registers[REG_IDX_STATUS],
				pDat->map_inf->tab_registers[REG_IDX_MARK],
				delay);
			return RES_ERR_QUIT;
		}
		// 4???????????????????
		if (pDat->map_result->tab_registers[REG_IDX_MARK] == MARK_READY || pDat->map_result->tab_registers[REG_IDX_MARK] == MARK_END)
		{
			*status = pDat->map_result->tab_registers[REG_IDX_STATUS];			

			//???0????????
			if (*status == 0)
			{
				int final_num = pDat->map_result->tab_registers[REG_IDX_DAT_LEN];
				if (final_num <= 0)
				{
					DWORD nowTick = GetTickCount();
					int lastIdx = id & 0xFF;
					if (g_lastNoDataLogTick[lastIdx] == 0
						|| (nowTick - g_lastNoDataLogTick[lastIdx]) >= 1000
						|| g_lastNoDataStatus[lastIdx] != *status
						|| g_lastNoDataParam[lastIdx] != pDat->map_result->tab_registers[REG_IDX_PARAM])
					{
						DebugStr5("[DLL_FLOW]WaitTestResult FinalNoData,id=%d,result_mark=%d,status=%d,param=%d,remain=%d\n",
							id,
							pDat->map_result->tab_registers[REG_IDX_MARK],
							*status,
							pDat->map_result->tab_registers[REG_IDX_PARAM],
							delay);
						g_lastNoDataLogTick[lastIdx] = nowTick;
						g_lastNoDataStatus[lastIdx] = *status;
						g_lastNoDataParam[lastIdx] = pDat->map_result->tab_registers[REG_IDX_PARAM];
					}
					Sleep(10);
					continue;
				}

				*NumofResult = final_num;
				if (*NumofResult > INF_CNT)*NumofResult = INF_CNT;
				for (int i = 0; i < *NumofResult; i++)
				{
					res[i] = pDat->map_result->tab_registers[REG_IDX_DAT + i * 2 + 1] << 16 | pDat->map_result->tab_registers[REG_IDX_DAT + i * 2];
				}
				GetBarcode(barcode);
			}			
			*status = *status & 0x0FFF;
			{
				int lastIdx = id & 0xFF;
				int curMark = pDat->map_result->tab_registers[REG_IDX_MARK];
				int curParam = pDat->map_result->tab_registers[REG_IDX_PARAM];
				if (g_lastReturnStatus[lastIdx] != *status
					|| g_lastReturnNum[lastIdx] != *NumofResult
					|| g_lastReturnMark[lastIdx] != curMark
					|| g_lastReturnParam[lastIdx] != curParam)
				{
					DebugStr5("[DLL_FLOW]WaitTestResult Return,id=%d,result_mark=%d,status=%d,num=%d,param=%d\n",
						id,
						curMark,
						*status,
						*NumofResult,
						curParam);
					g_lastReturnStatus[lastIdx] = *status;
					g_lastReturnNum[lastIdx] = *NumofResult;
					g_lastReturnMark[lastIdx] = curMark;
					g_lastReturnParam[lastIdx] = curParam;
				}
			}
			//??????????
			if (pDeviceParam != NULL)
			{
				//int addr = 4 + Dev.mb_map_result->tab_registers[3] * 2;
				//pDeviceParam->angle = pDat->map_result->tab_registers[addr + 1] << 16 | pDat->map_result->tab_registers[addr + 0];
				//pDeviceParam->lenpitch = pDat->map_result->tab_registers[addr + 3] << 16 | pDat->map_result->tab_registers[addr + 2];
				//pDeviceParam->spaceAngle = pDat->map_result->tab_registers[addr + 5] << 16 | pDat->map_result->tab_registers[addr + 4];
			}

			//reset test result
			//pDat->map_result->tab_registers[0] = 2;
			return RES_OK;
		}		

		Sleep(10);
	}
	return RES_ERR_TIMEOUT;

	
	//for (int n = 0; n < 10; n++)
	//{
	//	int rc = modbus_read_registers(Dev.mb, Dev.mb_map_result->start_registers, 4 + *NumofResult * 2, Dev.mb_map_result->tab_registers);
	//	if (rc == 4 + *NumofResult * 2)
	//	{
	//		if (Dev.mb_map_result->tab_registers[3] > NumofResult)
	//		{
	//			rc = modbus_read_registers(Dev.mb, Dev.mb_map_result->start_registers, 4 + Dev.mb_map_result->tab_registers[3] * 2, Dev.mb_map_result->tab_registers);
	//		}

	//	}
	//	if (rc > 0)
	//	{
	//		*status = Dev.mb_map_result->tab_registers[1];

	//		//??????
	//		NumofResult = Dev.mb_map_result->tab_registers[3];
	//		for (int i = 0; n < *NumofResult * 2; n++)
	//		{
	//			res[i] = Dev.mb_map_result->tab_registers[4 + i * 2] << 16 | Dev.mb_map_result->tab_registers[4 + i * 2 + 1];
	//		}

	//		//??????????
	//		if (pDeviceParam != NULL)
	//		{
	//			int addr = 4 + Dev.mb_map_result->tab_registers[3] * 2;
	//			pDeviceParam->angle = Dev.mb_map_result->tab_registers[addr] << 16 | Dev.mb_map_result->tab_registers[addr + 1];
	//			pDeviceParam->lenpitch = Dev.mb_map_result->tab_registers[addr + 2] << 16 | Dev.mb_map_result->tab_registers[addr + 3];
	//			pDeviceParam->spaceAngle = Dev.mb_map_result->tab_registers[addr + 4] << 16 | Dev.mb_map_result->tab_registers[addr + 5];
	//		}
	//		return 0;
	//	}
	//	else
	//	{
	//		modbus_close(Dev.mb);
	//		Sleep(100);
	//		modbus_connect(Dev.mb);
	//		Sleep(100);
	//	}
	//}
	//return 0;
}

//???????????????????
int NextTest(int id ,int status, int delay, BOOL *bquit)
{		
	Init("DevCfg.ini");	

	//reset test
	if (status == -1)ResetTest(id);

	if (bquit != NULL&&*bquit)return RES_ERR_QUIT;

	SlaveData *pDat = GetSlaveDatByID(id);
	if (pDat == NULL)return RES_ERR;
	DebugStr5("[DLL_FLOW]NextTest Enter,id=%d,req_sta=%d,result_mark=%d,result_sta=%d,result_num=%d\n",
		id, status,
		pDat->map_result != NULL ? pDat->map_result->tab_registers[REG_IDX_MARK] : -1,
		pDat->map_result != NULL ? pDat->map_result->tab_registers[REG_IDX_STATUS] : -1,
		pDat->map_result != NULL ? pDat->map_result->tab_registers[REG_IDX_DAT_LEN] : -1);

	// 4???????????????????
	if (pDat->map_result->tab_registers[0] == MARK_END)
	{
		if (status == 0)
		{
			DebugStr3("[DLL_FLOW]NextTest FinalReset,id=%d,req_sta=%d,result_sta=%d\n",
				id, status, pDat->map_result->tab_registers[REG_IDX_STATUS]);
			return ResetTest(id);
		}

		DebugStr3("[DLL_FLOW]NextTest SkipEnd,id=%d,req_sta=%d,result_sta=%d\n",
			id, status, pDat->map_result->tab_registers[REG_IDX_STATUS]);
		return 0;
	}

	pDat->map_result->tab_registers[REG_IDX_STATUS] = status;//status
	pDat->map_result->tab_registers[REG_IDX_PARAM] = 0;//param
	pDat->map_result->tab_registers[REG_IDX_MARK] = MARK_ACK;//mark
	DebugStr5("[DLL_FLOW]NextTest Send,id=%d,req_sta=%d,result_mark=%d,result_sta=%d,delay=%d\n",
		id, status,
		pDat->map_result->tab_registers[REG_IDX_MARK],
		pDat->map_result->tab_registers[REG_IDX_STATUS],
		delay);

	//wait for accept
	for (; delay >0; delay -= 10)
	{
		if (bquit != NULL&&*bquit)return RES_ERR_QUIT;
		//?????????????????????????????
		if (pDat->map_result->tab_registers[0] != MARK_ACK)
		{
			DebugStr5("[DLL_FLOW]NextTest Acked,id=%d,req_sta=%d,result_mark=%d,result_sta=%d,remain=%d\n",
				id, status,
				pDat->map_result->tab_registers[REG_IDX_MARK],
				pDat->map_result->tab_registers[REG_IDX_STATUS],
				delay);
			return RES_OK;
		}
		Sleep(10);
	}
	DebugStr5("[DLL_FLOW]NextTest Timeout,id=%d,req_sta=%d,result_mark=%d,result_sta=%d,delay=%d\n",
		id, status,
		pDat->map_result->tab_registers[REG_IDX_MARK],
		pDat->map_result->tab_registers[REG_IDX_STATUS],
		delay);
	return RES_ERR_TIMEOUT;
}

//???????
int GetInfo(int id, InfoData *pdat)
{
	Init("DevCfg.ini");
	SlaveData *pSlaveDat = GetSlaveDatByID(id);
	if (pSlaveDat == NULL)return RES_ERR;
	if (pdat != NULL)
	{
		pdat->s = pSlaveDat->s;
		pdat->slaveid = pSlaveDat->slaveid;
		pdat->start_mark = pSlaveDat->map_inf != NULL ? pSlaveDat->map_inf->tab_registers[0] : 0;
		pdat->result_mark = pSlaveDat->map_result != NULL ? pSlaveDat->map_result->tab_registers[0] : 0;
		pdat->pos_idx = pSlaveDat->map_result != NULL ? pSlaveDat->map_result->tab_registers[1] : 0;
		//result
		if (pSlaveDat->map_result != NULL)
		{			
			//reset res
			for (int i = 0; i < SLAVE_BUF; i++)pdat->res[i] = -1;
			//fill res
			pdat->res_num = pSlaveDat->map_result->tab_registers[3];
			for (int i = 0; i < SLAVE_BUF && i < pdat->res_num && (4 + i * 2 + 1) < pSlaveDat->map_result->nb_registers; i++)
			{
				pdat->res[i] = pSlaveDat->map_result->tab_registers[4 + i * 2 + 1] << 16 | pSlaveDat->map_result->tab_registers[4 + i * 2];
			}
		}
		//barcode
		if (pSlaveDat->map_inf != NULL)
		{
			int barcode_num = 0;
			char *pch = (char*)&pSlaveDat->map_inf->tab_registers[4];
			for (int idx = 0; idx < ((pSlaveDat->map_inf->nb_registers - 4) * 2 - 1) && *pch != '\0'; idx++)
			{
				pdat->barcode[idx] = pch[idx];
			}
		}
		
		pdat->status = pSlaveDat->status;
		strcpy_s(pdat->ip, sizeof(pSlaveDat->ip),pSlaveDat->ip);
	}
	return RES_OK;
}

//????????
int ResetTest(int id)
{
	Init("DevCfg.ini");

	SlaveData *pDat = GetSlaveDatByID(id);
	if (pDat == NULL)return RES_ERR;	
	DebugStr5("[DLL_FLOW]ResetTest Enter,id=%d,result_mark=%d,result_sta=%d,inf_mark=%d,inf_sta=%d\n",
		id,
		pDat->map_result != NULL ? pDat->map_result->tab_registers[REG_IDX_MARK] : -1,
		pDat->map_result != NULL ? pDat->map_result->tab_registers[REG_IDX_STATUS] : -1,
		pDat->map_inf != NULL ? pDat->map_inf->tab_registers[REG_IDX_MARK] : -1,
		pDat->map_inf != NULL ? pDat->map_inf->tab_registers[REG_IDX_STATUS] : -1);

	for (int delay=0 ; delay <10; delay++)
	{
		pDat->map_inf->tab_registers[REG_IDX_MARK] = MARK_RESETTEST;
		Sleep(10);
	}
	DebugStr3("[DLL_FLOW]ResetTest Send,id=%d,inf_mark=%d,inf_sta=%d\n",
		id,
		pDat->map_inf->tab_registers[REG_IDX_MARK],
		pDat->map_inf->tab_registers[REG_IDX_STATUS]);
	return RES_OK;
}
