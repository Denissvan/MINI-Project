# Project Notes

- When analyzing this software's `.log` files, do not assume `UTF-8`.
- Prefer `GBK`/`GB2312`/`OEM` decoding first. If Chinese text is garbled, retry with those encodings before drawing conclusions from the log content.
- Use the local skill `wltmini-log-analysis` for this project's logs, workstation state analysis, and test/updownload time-line tracing.
