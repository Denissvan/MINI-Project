# 2026-05-07 测试端断链/重启通信闭环修复

## 背景

产线复现测试电脑手动断开、测试软件重新登录后，电控侧可能遇到某台 PC 的旧测试会话丢失，结果区表现为 `mark=4, sta=0, num=0`，同时其他 PC 仍处于测试中。旧逻辑把这类情况按普通 `WaitTestResult` 错误处理，容易导致整站测试异常，或后续 `StartFlow` 命中“上一轮结果未闭环”。

## 本次修改

- DLL 增加返回码：`RES_ERR_SESSION_LOST=-7`、`RES_ERR_LINK_LOST=-8`。
- 2026-05-07 追加编译兜底：在 `myfunc.c` include 后增加 `#ifndef` 宏定义，避免 VS 预编译头/旧包含路径导致 `RES_ERR_SESSION_LOST`、`RES_ERR_LINK_LOST` 未定义。
- DLL 将联机心跳计数从全局 `Tick/TickCnt` 改为按 PC 维度统计，避免某一台测试电脑异常影响其他电脑的断链判断。
- 2026-05-07 现场追加：优先修复“PC2 断链/重启导致其他 PC 也重连”的公共 server 隔离问题。电控端 DLL 的 `ThreadListen` 原来所有测试 PC 共用一个 select 主循环，断链 socket 要累计多次后才关闭，并且断链路径里有 `Sleep(1000)`；这会阻塞其他 PC 的 ACK 处理，导致其他测试端 DLL 在 `SetDeviceStatus`/`ThreadStatus` 中一起触发重连。
- 电控端 DLL 将 `modbus_tcp_listen(Dev.mb, 3)` 改为 `modbus_tcp_listen(Dev.mb, SLAVE_BUF)`，避免 8 台 PC 同时重连时监听队列过小。
- 电控端 DLL 在 socket 断链时立即 `FD_CLR + closesocket`，不再在 server 主循环中延迟关闭和睡眠，避免单 PC 断链拖住其他 PC 通信。
- 电控端 DLL 在 `SlaveDat` 满槽清理最旧连接时，同步从 `select` 集合移除并关闭旧 socket，避免遗留脏 fd。
- 电控端 DLL 在同一个 `slaveid` 重新连接时，清理旧 socket 槽位，确保 `GetSlaveDatByID(id)` 不再优先命中旧连接。
- DLL 在 `StartTestFlow Ack` 后记录当前 PC 有活动测试会话；如果后续 `WaitTestResultA` 看到活动会话的最终区变成 `sta=0,num=0`，且结果区 `param` 已不同于本轮启动/中间测试状态并持续多次，返回 `SESSION_LOST`，避免把 StartFlow 后短暂旧空结果误判为断链。
- C# `WS.WaitTestResult` 收到 `SESSION_LOST` 时，仅将当前 PC 覆盖工位按 `NG_OS(270)` 闭环并调用 `NextTest(pc,-1)` 关闭该 PC 会话；同工站另一台 PC 继续走原测试流程。
- C# 收到 `LINK_LOST` 时，仍将当前 PC 标记为 `LINK_ERR` 并按工站测试异常报警停机。

## 2026-05-08 log508 复盘与修正

- 现场日志目录：`C:\Users\gdfanyd\Downloads\4.23\log508`。
- 电控日志 `log\2026-05-08.log` 中，`00:22:38.372` 不是 PC2 重连点，而是工站1上下料完成；PC2 异常在更早的 `00:22:31.578` 已由电控报出：`工站2 PC2测试端长时间联机断开`。
- `00:22:39.599`、`00:22:39.710` 工站1的 PC1/PC5 StartFlow 当时成功；后续 `00:23:17` 手动重新开始后，才出现 PC1 `StartFlow命中上一轮结果未闭环`。
- 测试端 DLL 日志版本为 `EC_DLL_2026-05-04_002`，属于现场旧 DLL；事故窗口内未看到 1/3/4/5/6/7/8 同时重连，PC2 才是主要异常点。
- 根因调整：这次不是所有 PC 独立 DLL 同时断链，而是 PC2 `LINK_LOST` 被 C# 当成工站/全局测试异常，导致总测试线程退出；再次开始时，其他工站仍有未闭环 PC 测试会话，电控又重发 StartFlow，触发 DLL 的 `BusyLastResult/上一轮结果未闭环`。
- 业务策略调整：测试电脑重启/重连时，不再直接把当前 PC 覆盖料判 `NG_OS(270)`；该 PC 的料复位为 `UNTEST(-1)`，关闭当前 PC 会话，等待下一轮直接重测，不上传本轮结果。
- C# `WS.WaitTestResult` 收到 `SESSION_LOST` 或 `LINK_LOST` 后，均只隔离当前 PC：当前 PC 模组复位待测、PC 状态恢复 READY、调用 `NextTest(pc,-1)` 关闭会话，并继续处理同工站另一台 PC，避免返回 `ERR` 打断全局测试线程。
- C# 在 `StartTestFlow` 入口和工站测试线程恢复处增加未闭环会话保护：若当前工站已有 PC 或模组 `test_idx != 0`，说明旧测试仍在进行/等待闭环，跳过 StartFlow，直接进入等待结果，避免重启线程后误发新一轮 StartFlow。
- 2026-05-08 新文件夹0508 复验补充：手动关闭 PC2/PC4 后，重试后 DLL 可能先返回 `ERR_LINK(-5, 失联)`，不是 `LINK_LOST(-8)`；C# 也需要把 `ERR_LINK` 作为当前 PC 隔离处理，否则仍会进入普通错误分支并结束测试线程。
- 同次复验还暴露同排双 PC 结果一致性问题：当前 PC 复位待测后，同工站另一 PC 仍可能处于旧测试位，且 `list[0]` 可能是禁用/旧状态槽位，导致 `WaitTestResult,结果不一致,PC=2/4,idx=0`。修正为断链复位时清零该 PC 列表内所有槽位 `test_idx`，一致性检查只以启用模组为基准。
- 2026-05-08 新文件夹0508 二次复盘：PC4 断链后只清当前 PC 会导致同工站 PC8 仍保留旧 `sta/test_idx=206`，随后触发正常生产路径的同步/一致性校验，表现为电控无法继续运行；PC8 重新登录后，PC4 又可能反复返回 `sta=0,num=0` 的空最终结果，导致线程持续轮询。
- C# 修正策略调整为工站级断链恢复：`SESSION_LOST`、`LINK_LOST`、`ERR_LINK`、以及活动会话下的 `sta=0,num=0` 空结果，均关闭同工站所有 PC 当前会话、清零所有 PC/模组 `test_idx`，当前工站结果不判 NG，直接返回 `OK` 退出本轮 `WaitTestResult`，避免继续读取同工站另一台 PC 的旧状态并触发同步校验。
- 2026-05-08 9090 复验补充：PC4/PC8 在工站4测试 `sta=8` 时，PC8 `WaitTestResult` 失联后旧版本只清 PC8，PC4 仍保留 `sta=8`；再次启动后 PC8 返回 `sta=0,num=0` 被当作普通缺失，电控继续按 PC4 的 `sta=8` 定位并 `NextTest(8)` 给 PC8，随后电控侧 DLL 高频打印 `WaitTestResult FinalNoData,id=8`，表现为电控无反应。空结果恢复判断改为检查整工站是否仍有活动 PC 会话，而不是只检查当前 PC。
- 2026-05-09 `2攻占模拟掉线电控卡主` 复验补充：电控 `2026-05-08.log` 在 `17:55:17.419` 进入工站2 `WaitTestResult,ID=2`，`17:56:32.837` 只打印一次重发，之后工站2没有返回；电控侧 DLL 同期持续打印 `WaitTestResult FinalNoData,id=2,ws=2,result_mark=4,status=0,param=389~395`，未出现 `SessionLost/LinkStall`。原因是恢复流程跳过 `StartFlow` 后，C# 仍知道有未闭环会话，但 DLL 进程本地 `g_activeSession[id]` 可能已被清掉，旧判定必须依赖 `g_activeSession != 0` 才会把 `MARK_END/sta=0/num=0` 判为会话丢失。
- 本次追加修正：DLL 对 `MARK_END/sta=0/num=0` 增加 `SessionLostNoActive` 兜底，close-only 优先级不变；当 DLL 本地没有 active session 但空最终结果连续存在时，快速返回 `RES_ERR_SESSION_LOST`。C# `WaitTestResult` 的重试条件排除 `SESSION_LOST/LINK_LOST/ERR_LINK`，这些明确通信异常直接进入工站级断链恢复，不再多次重发。

## 回溯点

- DLL：`dllforcomv8/myfunc.c`、`dllforcomv8/myfunc.h`
- 电控 C#：`UI/Class/TestPC.cs`、`UI/Class/WS.cs`
- 关键日志：`WaitTestResult SessionLost`、`WaitTestResult SessionLostNoActive`、`WaitTestResult LinkStall`、`同工站PC会话按断链恢复处理`、`当前存在未闭环PC测试会话，恢复后跳过StartFlow直接等待结果`
