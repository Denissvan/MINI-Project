# Session Records

Use this folder to separate workshop-specific issue history.

## Purpose

- Prevent different workshop problems from being mixed in one conversation.
- Preserve the latest hypothesis, code changes, and validation result for each demand track.
- Make it easy to continue in a new Codex conversation by pointing to one session file.

## Folder Layout

Recommended layout:

```text
sessions/
  workshop-a/
    tray-version-switch/
      2026-04-10.md
    401-extra-test-delay-unload/
      2026-04-10.md
  workshop-b/
    full-row-ng-no-unload/
      2026-04-10.md
```

## Demand Track Names

- `tray-version-switch`
- `401-extra-test-delay-unload`
- `full-row-ng-no-unload`

## Update Rule

Each session note should record:

1. Workshop
2. Demand track
3. Related logs
4. Observed symptom
5. Current root-cause hypothesis
6. Code changes made
7. Validation result
8. Next action
