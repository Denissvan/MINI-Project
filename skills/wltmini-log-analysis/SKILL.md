---
name: wltmini-log-analysis
description: Use when analyzing logs, workstation state, testing flow, or updownload behavior for the WLTmini software in this repository. Prioritize correct log decoding: read `.log` files with GBK/GB2312/OEM before trusting Chinese text, and only fall back to UTF-8 if the content is clearly valid.
---

# WLTmini Log Analysis

Use this skill for:
- `2026-*.log` analysis
- workstation state tracing
- test flow / `StartFlow` / `WaitTestResult` / `NextTest` analysis
- updownload, pause, second pickup, repeated unload, and stale-result investigations

## Log Decoding Rule

- Do not trust Chinese log text read with default `UTF-8`.
- For this project, `.log` files should be read with `GBK`, `GB2312`, or `OEM` first.
- If the line contains mojibake such as `���`, `����`, or broken Chinese, retry with `GBK`/`GB2312`/`OEM` before analyzing semantics.
- In PowerShell, `OEM` or `[System.Text.Encoding]::GetEncoding('gbk')` are the preferred options.

## Source File Encoding Rule

- Do not apply the log decoding rule to source files.
- Before editing a `.cs`, `.Designer.cs`, `.resx`, `.c`, or `.h` file, first detect or confirm the file's existing encoding.
- Preserve the original source-file encoding when writing changes back.
- Do not use PowerShell `Set-Content` / `Out-File` / redirection blindly on source files unless the encoding is explicitly set to match the file.
- If an edit causes Chinese comments or string literals to become mojibake, restore the file before continuing and switch to an encoding-preserving edit path.

## Build Rule

- For this project, do not compile by default after demand changes or code edits.
- The user will handle compilation manually.
- Only run build or compile steps when the user explicitly asks for them.

## Working Rule

When a user references a log timestamp:
1. Read the exact line with `GBK`/`GB2312`/`OEM`.
2. Read nearby lines in the same encoding.
3. Only then infer the workflow state and root cause.

## Checks To Perform

- Confirm whether the station actually ran `reset -> StartFlow`.
- Check whether `TestStatus`, `FeedStatus`, and `bfeed` are consistent with the observed action.
- Distinguish:
  - stale-result / repeated-updownload issues
  - communication/test-state issues
  - tray/feed/end-of-material issues
  - operator pause / resume intervention

## Useful Keywords

- `下料判定快照`
- `WaitTestResult返回快照`
- `StartFlow`
- `NextTest`
- `完成工站`
- `---暂停键按下---`
- `---开始键按下---`


## Multi-Workshop Session Rule

This repository is used for multiple workshops in parallel. Do not assume all issues in one conversation belong to the same workshop or the same root cause.

When the user describes multiple workshop issues:
1. Separate them into distinct demand tracks.
2. Keep each track independent unless the user explicitly merges them.
3. When continuing in a new conversation, first identify which track is being discussed before reusing conclusions.

Current baseline demand tracks:
- `workflow-demand-management`
  Manage demand taxonomy, session records, new-conversation matching, and continuation workflow.
- `tray-version-switch`
  Startup selects different machine versions and maps to different OK/NG tray-box modes.
- `401-extra-test-delay-unload`
  The final `401` add-on test causes the station to reach unload position but unload only after one more turn.
- `full-row-ng-no-unload`
  One PC returns a full row of NG results and later the station does not unload or rereads stale results.

## Session Record Workflow

Use project-local session records to avoid mixing workshop contexts across turns.

Recommended process:
1. Create or update one session note per workshop issue under `sessions/`.
2. Name files with workshop + demand track + date.
3. Record only high-signal context:
   - workshop identifier
   - demand track
   - logs analyzed
   - current hypothesis
   - code changes made
   - validation result
   - next action
4. When starting a new conversation, first read the matching session note before extending analysis.

Suggested filename format:
- `sessions/<workshop>/<demand-track>/<yyyy-mm-dd>.md`

## Current Conversation Rule

If the user says the current conversation should focus on one demand only, keep all new analysis and code changes constrained to that demand until the user redirects.

For a brand-new empty Codex conversation:
- The user may describe the symptom only, without naming the workshop or demand track.
- First try to classify the issue into an existing demand track by symptom and code/log context.
- If one existing track is clearly the best match, confirm that mapping briefly and continue.
- Only ask whether to create a new demand when the issue cannot be cleanly mapped to an existing track.

For a `codex-resume` continuation:
- Do not re-ask for demand-track confirmation by default.
- Assume the resumed conversation should continue the existing track unless the user explicitly redirects or re-emphasizes a different demand.

At the time of writing:
- current focus demand: `full-row-ng-no-unload`
