# planning-mds (Solution-Specific)

This directory contains **all solution-specific requirements and references**. It should be created from scratch for each new project.

**Note:** The current content (domain/, examples/, features/, screens/, etc.) is for **Nebula CRM** (insurance) and serves as a reference example. When starting a new project, replace all content with your own domain knowledge and requirements.

If you are starting a new project, see `blueprint-setup/README.md` for bootstrap steps.

## Minimal Folder Scaffold

```bash
mkdir -p planning-mds/{domain,examples,features,screens,workflows,architecture,api,security,testing,operations}
mkdir -p planning-mds/features/archive
mkdir -p planning-mds/examples/{personas,features,stories,screens,architecture,architecture/adrs}
mkdir -p planning-mds/security/reviews
```

Story convention:
- Keep one story per markdown file inside its feature folder: `planning-mds/features/F{NNNN}-{slug}/F{NNNN}-S{NNNN}-{slug}.md`.

## Rule of Thumb

If it’s **project-specific**, it belongs here. Agents should never embed it directly.
