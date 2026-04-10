# Cross-Account Persistence Plan

## Goal

Build a stable workflow so that:

- project demand/session records stay inside each project repository
- reusable skills stay outside repositories in one shared folder on `E:`
- switching ChatGPT accounts or switching to another coding tool does not lose core process knowledge

## Current State

- Project-local `sessions/` already exists and is the right place for demand progress.
- Project-local `skills/` currently contains `wltmini-log-analysis`.
- The missing layer is a shared skill library and a clear rule for what belongs in project folders vs. shared folders.

## Recommended Architecture

Use three layers.

### Layer 1: Global workspace on `E:`

Recommended root:

```text
E:\AI-Workspace\
```

Recommended structure:

```text
E:\AI-Workspace\
  skills\
    shared\
      log-analysis-common\
      workflow-demand-governance\
      session-maintenance\
    projects\
      wltmini\
        wltmini-log-analysis\
        wltmini-workflow-governance\
      other-project-a\
      other-project-b\
  templates\
    session-template.md
    demand-template.md
    skill-template\
  standards\
    session-writing-rule.md
    demand-classification-rule.md
    skill-boundary-rule.md
  backups\
    skills\
  index\
    skills-index.md
    projects-index.md
```

Purpose:

- `skills\shared\`: reusable cross-project skills
- `skills\projects\`: project-specific skills that should survive account switching
- `templates\`: unified templates
- `standards\`: governance documents, not bound to one repo
- `backups\`: exported backups of important skills
- `index\`: one place to see what exists

### Layer 2: Project repository

Keep only project-specific runtime context inside each repo:

```text
<repo>\
  AGENTS.md
  sessions\
  skills\
```

Rule:

- `sessions\` stores demand progress and workshop-isolated conclusions
- `skills\` stores project-local wrapper skills or copies required for this repo to work independently
- source code and logs remain in the repo as usual

### Layer 3: Tool adapter layer

Different tools may not read the same skill folder automatically.

So the stable strategy is:

1. treat `E:\AI-Workspace\skills\...` as the source of truth
2. keep project-local `skills\` as a lightweight mirror or wrapper
3. when switching tools, only adapt the loading path, not the skill content itself

## Ownership Boundary

Use this boundary to avoid future confusion.

### Put content in `E:\AI-Workspace\skills\shared\` when

- the workflow applies to multiple projects
- the skill is methodology-focused
- the skill does not depend on one repo's code structure

Examples:

- generic log triage workflow
- session note governance
- demand classification workflow
- cross-chat continuation checklist

### Put content in `E:\AI-Workspace\skills\projects\<project>\` when

- the skill is specific to one product line or one codebase
- file paths, log keywords, station states, or business rules are project-specific

Examples:

- `wltmini-log-analysis`
- tray version mapping rules
- updownload and test-state trace rules

### Keep content only in project `sessions\` when

- it is issue progress
- it is workshop-specific
- it is a temporary hypothesis or validation result

Examples:

- root-cause hypothesis
- logs already checked
- current workaround
- next validation action

## Minimal Skill Set To Build

The current gap is not “more sessions”, but “stable reusable skills”. Start with these.

### 1. `workflow-demand-governance`

Goal:

- classify a new user report into an existing demand track
- decide whether to create a new track
- enforce workshop isolation
- define continuation rules for new chats

### 2. `session-maintenance`

Goal:

- create/update session notes in a uniform format
- enforce high-signal notes only
- prevent mixing two demands in one session note

### 3. `wltmini-workflow-governance`

Goal:

- hold WLTmini-specific demand taxonomy
- define known active demand tracks
- specify when to reuse an old conclusion vs. start a fresh branch

### 4. `wltmini-log-analysis`

Goal:

- remain the execution skill for logs, workstation state, and timeline tracing
- move to shared `E:` storage as source of truth
- leave a project-local mirror if needed

## Migration Strategy

Do this in phases.

### Phase 1: Freeze the standard

- choose `E:\AI-Workspace\` as the only shared root
- define naming rules for skills and standards
- define which documents are source-of-truth vs. mirrors

### Phase 2: Move skills out of single-account directories

- stop relying only on `C:\Users\<user>\.codex\skills\...`
- copy project-relevant skills into `E:\AI-Workspace\skills\...`
- keep an index file with skill name, purpose, and owner

### Phase 3: Keep project-local wrappers

- keep a small `skills\` folder in each repo
- project-local skill can either:
  - duplicate the shared skill
  - or contain a short wrapper note pointing to the shared canonical version

For reliability, duplication is safer than indirection when tools differ.

### Phase 4: Build the governance skills

- create `workflow-demand-governance`
- create `session-maintenance`
- create `wltmini-workflow-governance`

### Phase 5: Add backup discipline

- back up `E:\AI-Workspace\skills\` regularly
- export key standards and templates with date versions
- do not depend on chat history as the only carrier of process knowledge

## Recommended Conventions

### Naming

- shared skills: `<domain>-<purpose>`
- project skills: `<project>-<purpose>`
- demand tracks: short, stable, symptom-oriented

### Versioning

- skills should have a small `Last updated` section in `SKILL.md`
- large governance changes should also be recorded in the related session track

### Source of Truth

- session progress: project `sessions\`
- reusable workflow knowledge: `E:\AI-Workspace\skills\`
- project-specific operating rules: both project `AGENTS.md` and project skills

## Recommended Execution Order For Your Case

1. Establish `E:\AI-Workspace\` as the permanent shared root.
2. Move or copy the current WLTmini skill into `E:\AI-Workspace\skills\projects\wltmini\`.
3. Keep this repo's `skills\wltmini-log-analysis\SKILL.md` as a mirror for local continuity.
4. Add two new governance skills:
   - `workflow-demand-governance`
   - `session-maintenance`
5. Later, add `wltmini-workflow-governance` to hold project-specific demand taxonomy.

## Conclusion

For your current setup, the judgment is:

- `sessions\` is already doing its job
- the missing capability is a stable, shared, account-independent skill library
- the right design is not to move sessions out of the repo
- the right design is to move reusable skills and governance standards to `E:\AI-Workspace\`, while keeping project-local mirrors for tool compatibility
