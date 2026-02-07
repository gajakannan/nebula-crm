# Orchestration Contract

This document defines the minimum contract an orchestrator must satisfy to execute this framework.

The framework is orchestrator-agnostic and model-agnostic.
It can be used with any agent runtime that can read markdown contracts and follow file-based workflows.

## 1. Action Discovery

- Discover available user-facing actions from `agents/actions/README.md`.
- Each action definition lives in `agents/actions/<action>.md`.

## 2. Role Activation

- When an action requires a role, load `agents/<role>/SKILL.md`.
- Use role scope/boundaries exactly as specified.
- Do not merge role responsibilities unless the action explicitly does so.

## 3. Intent Routing

- Map user intent to an action definition.
- If intent is ambiguous, request clarification before execution.
- Execute action steps in defined order, including sequential and parallel stages.

## 4. Inputs and Outputs

- Required planning inputs come from `planning-mds/`.
- Generic templates and references come from `agents/templates/` and `agents/**/references/`.
- Output artifacts must be written to the paths defined by each action.

## 5. Approval and Review Gates

- For each approval gate, present the gate summary and capture an explicit user decision.
- Route execution based on that decision (`approve`, `fix`, `reject`, etc.).
- Never skip required gates.

## 6. Boundary Enforcement

- Treat `agents/` as generic framework content.
- Treat `planning-mds/` as solution-specific content.
- Do not introduce solution-specific requirements into `agents/`.

## 7. Failure Handling

- On step failure, report:
  - failed step
  - impacted artifacts
  - suggested remediation
- Follow action-defined retry paths.
- Escalate to user when unresolved after retry limits.

## 8. Auditability

- Keep execution traceable by referencing:
  - which action was run
  - which roles were activated
  - which artifacts were read/written
  - which approval decisions were made

## 9. Runtime Independence

This repository does not require a single vendor-specific orchestrator file to function.
Any orchestrator is compatible if it honors this contract and role/action definitions.
