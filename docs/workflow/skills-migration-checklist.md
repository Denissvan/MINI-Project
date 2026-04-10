# Skills Migration Checklist

## Goal

Migrate skill capability from account-bound locations into a shared `E:\AI-Workspace\skills\` library without losing project-local usability.

## Checklist

1. Create `E:\AI-Workspace\skills\shared\`.
2. Create `E:\AI-Workspace\skills\projects\wltmini\`.
3. Copy or mirror:
   - `workflow-demand-governance`
   - `session-maintenance`
   - `wltmini-workflow-governance`
   - `wltmini-log-analysis`
4. Add `index\skills-index.md`.
5. Keep repository `skills\` as local mirrors.
6. When a skill changes materially, update both:
   - shared canonical copy
   - repository mirror
