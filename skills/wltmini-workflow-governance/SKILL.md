---
name: wltmini-workflow-governance
description: Use when organizing WLTmini-specific demand tracks, deciding whether a WLTmini issue matches an existing session, or maintaining project-specific workflow rules for this repository.
---

# WLTmini Workflow Governance

Use this skill for WLTmini-specific workflow management.

## Scope

This skill manages:

- WLTmini demand taxonomy
- project-specific continuation rules
- relationship between WLTmini production issues and workflow-governance tracks

This skill does not replace `wltmini-log-analysis`; it routes work to the right track first.

## Current Demand Tracks

- `workflow-demand-management`
  Workflow governance, session structure, continuation rules, and skill planning.
- `tray-version-switch`
  Startup machine-version selection and OK/NG tray mode mapping.
- `401-extra-test-delay-unload`
  Final `401` add-on test causes unload to wait one extra cycle.
- `full-row-ng-no-unload`
  One PC returns a full row of NG and later does not unload or rereads stale results.

## Matching Rule

When a user describes a WLTmini issue:

1. Determine whether it is governance or runtime behavior.
2. If runtime behavior, match by symptom, not by vague similarity.
3. If the issue touches logs, workstation state, or test/updownload flow, also use `wltmini-log-analysis`.
4. If no existing track matches cleanly, create a new track under `sessions/<workshop>/`.

## Cross-Workshop Rule

- `general` is allowed only for workflow governance or when the workshop is genuinely unknown.
- Once the workshop is known, prefer moving future updates to that workshop path.
- Do not copy one workshop conclusion into another workshop note without evidence.

## Cross-Tool Persistence Rule

- project-specific active demand taxonomy must remain visible in the repository
- reusable methodology can live in the shared `E:\AI-Workspace\skills\projects\wltmini\` library
- this local skill should remain available as the repository-side mirror

## Build Rule

- Requirement changes and code edits do not require agent-side compilation by default.
- The user compiles this project manually.
- Do not add build/compile as a routine completion step unless the user explicitly requests it.

## Version Trace Rule

- Every confirmed code change must be associated with a traceable software version identifier.
- Record the version identifier in a durable project record before or together with the change summary.
- The version identifier may be a package version, handoff version tag, release label, dated patch label, or another user-recognized version string, but it must be explicit and stable enough to compare against field logs later.
- Record a short change summary for that version:
  - which files or communication path changed
  - what behavior changed
  - what symptom the change targets
- When analyzing logs, compare the log timestamp/version in use against the recorded version note before reusing old conclusions.
- If the current software on site cannot be mapped to a recorded version identifier, call that out explicitly instead of assuming the latest local code matches the field software.

## Related Files

- `skills/wltmini-log-analysis/SKILL.md`
- `sessions/general/workflow-demand-management/2026-04-10.md`
- `docs/workflow/cross-account-persistence-plan.md`
