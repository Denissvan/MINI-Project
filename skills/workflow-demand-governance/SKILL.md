---
name: workflow-demand-governance
description: Use when a user is discussing workflow optimization, demand classification, cross-workshop issue separation, continuation rules, or deciding whether a new request belongs to an existing demand track.
---

# Workflow Demand Governance

Use this skill when the task is about managing demands rather than fixing one runtime bug.

## Goals

- classify new reports into the right demand track
- keep workshop contexts isolated
- decide when to reuse an existing session note
- decide when to create a new demand track

## Working Rule

Treat these as separate concerns:

- production bug analysis
- workflow governance
- session record maintenance
- skill capability design

Do not merge them unless the user explicitly wants one track to absorb another.

## Classification Flow

When a new report arrives:

1. Identify the workshop, if it is known.
2. Identify whether the request is:
   - a production symptom
   - a workflow/process demand
   - a skill/capability demand
3. Compare the symptom or intent with existing demand tracks.
4. Reuse the existing track only if the match is clearly stronger than all others.
5. Otherwise create a new demand track with a short symptom-oriented name.

## Workshop Isolation Rule

- Never assume two workshops share one root cause.
- Reuse conclusions across workshops only when the user explicitly merges them or the evidence is strong and concrete.
- If one conversation contains multiple workshops, split them into distinct session notes.

## Continuation Rule

For a new conversation:

1. Check `sessions/` for the closest matching demand track.
2. Reuse only the latest relevant hypothesis, validation result, and next action.
3. Ignore stale assumptions that were not validated.

For a resumed conversation:

- continue the existing track by default
- only reclassify when new evidence conflicts with the old track

## Output Rule

When updating governance, keep notes high-signal:

- workshop
- demand track
- current scope
- current rule or hypothesis
- changes made
- next action

## Related Files

- `sessions/README.md`
- `sessions/session-template.md`
- `docs/workflow/cross-account-persistence-plan.md`
