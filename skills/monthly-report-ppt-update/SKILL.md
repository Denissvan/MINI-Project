---
name: monthly-report-ppt-update
description: "Use when creating or editing monthly report PowerPoint content in an existing deck, especially under E:\\电气课室月报\\... . Use for tasks like updating one slide only, modifying only a specified half-page or region, preserving the existing monthly-report style, converting a completed engineering/debug task into leader-facing summary content, adding simple flow diagrams or log screenshots, and exporting a preview image to verify layout before handoff."
---

# Monthly Report PPT Update

Edit existing monthly-report PPT files surgically. Preserve the deck's established visual language and modify only the user-specified slide and region.

## Core Rule

- Read the target PPT first.
- Identify the exact slide and the exact editable region.
- Do not redesign the whole page unless the user explicitly asks for redesign.
- Keep untouched content untouched.

## Default Workflow

1. Locate the target `.ppt` or `.pptx`.
2. Read the target slide's shapes:
   - title bar
   - left/right content regions
   - text boxes
   - divider lines
   - existing pictures
3. Determine the editable area from actual shape positions, not assumptions.
4. Replace or add content only inside the allowed region.
5. Prefer concise management-facing copy:
   - background
   - progress
   - root cause
   - fix
   - result
6. If the page feels too text-heavy, add one small visual:
   - abnormal flow
   - fixed flow
   - before/after comparison
   - log screenshot block
7. Export the edited slide to PNG and visually inspect it before finishing.

## Style Rules

- Follow the existing month-report style in the file or the nearest recent file from the same year.
- On content pages, keep the usual white background and structured text/image layout unless the file clearly uses a different pattern.
- Reuse existing heading style, colors, divider lines, and alignment.
- Keep the new work item as a sub-block of the page if the user says it is only one small part of the month.
- When only one side of the page is allowed, keep all new elements strictly inside that half.

## Content Rules

- Write for leader review, not for debugging replay.
- Compress the engineering process into a small number of points:
  - what problem appeared on site
  - what was analyzed
  - what root cause was confirmed
  - what was changed
  - what effect/risk reduction was achieved
- Do not paste large raw logs into the PPT.
- If logs need to appear, use:
  - a screenshot provided by the user, or
  - one short extracted evidence snippet, or
  - a simplified diagram derived from the logs

## Diagram Rules

For DLL / communication / workflow fixes, prefer small diagrams like:

- abnormal chain:
  - `StartFlow -> ACK -> stale result -> sta=0,n=0 / NG->-1`
- fixed chain:
  - `StartFlow -> save consumedStatus -> reset result area -> clear inf.status`
- comparison blocks:
  - `修复前`
  - `修复后`

Keep diagrams simple:

- 3 to 5 nodes
- short labels
- red for problem path
- green for fixed path
- avoid dense technical text inside nodes

## PowerPoint Editing Path

If the `slides`/artifacts runtime is unavailable, use local PowerPoint COM automation.

Recommended process:

1. Open the target file with PowerPoint COM.
2. Inspect slide shapes and coordinates.
3. Update text boxes or add shapes in the specified region.
4. Save the file.
5. Export the edited slide as PNG for QA.

## QA Checklist

Before handing off:

- Confirm the edit stays inside the requested region.
- Confirm no overlap with untouched content.
- Confirm heading hierarchy is clear.
- Confirm text is readable at presentation distance.
- Confirm any diagram is visually simpler than the text it replaces.
- Confirm the preview PNG matches the intended result.

## Good Output Pattern

For a half-page engineering item, aim for:

- one short background block
- one short progress block
- one compact flow diagram or one log screenshot

Avoid:

- full-page takeover
- long debugging prose
- multiple dense screenshots
- changing the page theme unnecessarily
