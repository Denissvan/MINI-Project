# Project Skills

This folder keeps project-local skills that the repository can use directly, even if the global shared skill library is unavailable.

## Skill List

- `wltmini-log-analysis`
  Execution skill for log decoding, workstation-state tracing, and test/updownload flow analysis.
- `workflow-demand-governance`
  Governance skill for demand classification, workshop isolation, and new-conversation continuation rules.
- `session-maintenance`
  Governance skill for creating and updating `sessions/` notes consistently.
- `wltmini-workflow-governance`
  WLTmini-specific demand taxonomy and continuation rules.
- `karpathy-guidelines`
  Use when the user explicitly asks to think carefully before changing code or planning a solution.
- `monthly-report-ppt-update`
  Edit an existing monthly report PPT surgically, preserving style while updating one slide or one page region with leader-facing text, flow diagrams, and optional log screenshots.

## Source-of-Truth Rule

- Shared long-term skill assets should live under `E:\AI-Workspace\skills\`.
- This repository keeps local mirrors or wrappers for continuity and tool compatibility.
