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
