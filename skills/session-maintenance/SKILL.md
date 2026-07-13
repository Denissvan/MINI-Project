---
name: session-maintenance
description: Use when creating, updating, splitting, or reviewing project session notes under sessions/, especially to preserve progress across account or tool switching.
---

# Session Maintenance

Use this skill when the task is to maintain session records, not to analyze the underlying bug itself.

## Goal

Keep `sessions/` as the durable progress layer for each demand track.

## Required Fields

Each session note should keep:

1. workshop
2. demand track
3. date
4. related logs or scope
5. software version identifier
6. versioned change summary
7. observed symptom
8. current hypothesis
9. code changes made
10. validation result
11. next action

Use `sessions/session-template.md` as the base shape.

## Update Rule

- Prefer updating the current day's note for the same demand before creating another file.
- Create a new dated file when the work moves to a new day or needs a clean branch.
- Keep conclusions factual and concise.
- Record what was verified separately from what is only suspected.
- For every confirmed code modification, append a version trace entry:
  - explicit version identifier
  - short summary of the change
  - whether the related field log/software has confirmed that version
- Do not rely on “latest code” as a version description.

## Split Rule

Split into separate session notes when:

- workshops differ
- symptoms differ materially
- validation paths differ
- one note starts mixing workflow governance with production debugging

## Cross-Account Persistence Rule

- Do not depend on chat history as the only record.
- If a conclusion matters later, write it into `sessions/`.
- If the content is reusable process knowledge rather than issue progress, move it into a skill instead.

## Quality Bar

A good session note should let a new chat resume with minimal ambiguity:

- what problem is being tracked
- what evidence was checked
- which software/log version the evidence belongs to
- what changed
- what to do next

## Related Files

- `sessions/README.md`
- `sessions/session-template.md`
- `skills/workflow-demand-governance/SKILL.md`
