# Generic Agent Roles - Agent-Driven Builder Framework

## Purpose

This directory contains **generic, reusable** agent role definitions for building software using an agent-driven builder methodology. Agents are designed to be copied across projects unchanged.

## How to Use

1) Copy `agents/` into a new repo
2) Create a fresh `planning-mds/` for the new solution
3) Use the agents as-is; all solution-specific content must live in `planning-mds/`

## Single Source of Truth

All agents read requirements from `planning-mds/INCEPTION.md` and related planning artifacts.

## Tech Stack Assumptions

The framework is opinionated about delivery practices and provides stack-specific references in some agent guides. In this repo, the default references assume a modern .NET + React + PostgreSQL stack. If you adopt a different stack, keep the agent roles but replace the stack-specific reference guides and examples with your own.

See `agents/TECH-STACK-ADAPTATION.md` for a concise adaptation guide.

---

If you’re starting a new project, see `planning-mds/README.md` for a minimal setup checklist.
