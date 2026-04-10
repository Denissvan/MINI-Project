# AI Handoff Prompt

Use this prompt when switching to another AI tool and you want it to continue the current workflow immediately.

```text
你现在接手的是一个已有工作流规范的本地项目，请严格按以下规则工作，不要先按通用方式自行发挥。

项目信息：
- 项目路径：D:\MyProject\自动点检验证普通版本1218\自动点检验证普通版本
- 共享能力库：E:\AI-Workspace\
- 当前项目是 WLTmini 相关项目

你的首要目标：
- 先接管当前项目的工作流，再处理具体需求
- 不要依赖历史聊天记录
- 以项目内文件和 E 盘共享技能库为准

接管顺序：
1. 先读取项目根目录下的 `AGENTS.md`
2. 读取项目内 `skills/README.md`
3. 优先读取以下 skills：
   - `skills/wltmini-workflow-governance/SKILL.md`
   - `skills/wltmini-log-analysis/SKILL.md`
   - `skills/workflow-demand-governance/SKILL.md`
   - `skills/session-maintenance/SKILL.md`
4. 如果需要共享规范，再读取：
   - `E:\AI-Workspace\index\skills-index.md`
   - `E:\AI-Workspace\standards\session-writing-rule.md`
   - `E:\AI-Workspace\standards\demand-classification-rule.md`
   - `E:\AI-Workspace\standards\skill-boundary-rule.md`
5. 根据当前需求，到 `sessions/` 下找到匹配的 demand track，再继续工作

必须遵守的项目规则：
- 这是多车间并行项目，不同车间的问题不得默认混为同一根因
- `sessions/` 是需求进度和验证结论的唯一持久化层
- 可复用方法论和专用 skill 以 `E:\AI-Workspace\skills\` 为长期主库
- 分析 `.log` 文件时，不要先假定 UTF-8，优先尝试 `GBK`/`GB2312`/`OEM`
- 日志、工位状态、测试流、上下料时间线分析优先遵循 `wltmini-log-analysis`
- 如果只是需求改动、流程调整、代码修改，不需要你执行编译；编译由用户自行完成
- 除非用户明确要求，否则不要主动编译项目

工作方式要求：
- 先判断当前需求属于哪个 demand track
- 如果能映射到已有 track，就沿用该 track
- 如果不能清晰映射，再创建新的 track
- 每次产生重要结论时，更新对应 `sessions/` 文件
- workflow 治理类内容写入 skill 或 docs，不要混进生产问题 session

你的输出要求：
- 先简短说明你识别到的 demand track 和将读取的文件
- 然后开始分析/修改
- 不要在接管阶段输出泛泛的项目介绍
```

## Usage

- For a brand-new AI session, paste the full prompt first.
- Then append the current concrete demand in one short paragraph.
- If needed, also append the exact session file path that should be continued.
