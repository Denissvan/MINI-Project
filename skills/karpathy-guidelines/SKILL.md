---
name: karpathy-guidelines
description: Behavioral guidelines to reduce common LLM coding mistakes. Use when writing, reviewing, or refactoring code to avoid overcomplication, make surgical changes, surface assumptions, and define verifiable success criteria.
license: MIT
---

# Karpathy Guidelines

Behavioral guidelines to reduce common LLM coding mistakes, derived from Andrej Karpathy's observations on LLM coding pitfalls.

Tradeoff:
- These guidelines bias toward caution over speed.
- For trivial tasks, use judgment.

## 1. Think Before Coding

Do not assume silently.

Before implementing:
- State assumptions explicitly.
- If multiple interpretations exist, present them instead of picking one silently.
- If a simpler approach exists, say so.
- If something is unclear, stop and name the uncertainty.

## 2. Simplicity First

Use the minimum code that solves the problem.

- No features beyond what was asked.
- No speculative abstractions.
- No configurability that was not requested.
- No unnecessary error handling for impossible scenarios.
- If the solution is clearly overbuilt, simplify it.

## 3. Surgical Changes

Touch only what is needed for the current request.

When editing existing code:
- Do not clean up unrelated code.
- Do not refactor adjacent areas unless required.
- Match the existing style unless the user asks otherwise.
- If unrelated dead code is noticed, mention it instead of deleting it.

When your changes create unused code:
- Remove only the imports, variables, or functions made unused by your own change.

## 4. Goal-Driven Execution

Define concrete success criteria before implementing.

For multi-step tasks:
1. State a short plan.
2. Attach a verification check to each step.
3. Verify outcomes instead of assuming success.

## Project Trigger Rule

In this repository, use this skill when the user says phrases such as:

- `仔细思考`
- `认真思考`
- `深入思考`
- `think carefully`

When triggered:
- surface assumptions and tradeoffs before editing
- prefer the smallest valid implementation
- make only request-scoped changes
- define a clear verification target before coding
