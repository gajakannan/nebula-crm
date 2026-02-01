# Repository Architecture (Conceptual)

This diagram shows how the repository is structured and how the reusable framework relates to solution-specific artifacts.

```
┌─────────────────────────────────────────────────────────┐
│                  REPOSITORY STRUCTURE                    │
├─────────────────────────────────────────────────────────┤
│                                                          │
│  ┌──────────────┐         ┌──────────────────────────┐   │
│  │   agents/    │         │    planning-mds/         │   │
│  │  (GENERIC)   │────────▶│  (SOLUTION-SPECIFIC)     │   │
│  │              │         │                          │   │
│  │ • 10 Roles   │         │ • Nebula CRM Specs       │   │
│  │ • Templates  │         │ • Insurance Domain       │   │
│  │ • References │         │ • Features/Stories       │   │
│  └──────────────┘         └──────────────────────────┘   │
│         │                            │                  │
│         │                            │                  │
│         ▼                            ▼                  │
│  ┌──────────────┐         ┌──────────────────────────┐   │
│  │ inception-   │         │  engine/experience/soma/ │   │
│  │   setup/     │         │    (IMPLEMENTATION)      │   │
│  │ (BOOTSTRAP)  │         │                          │   │
│  └──────────────┘         └──────────────────────────┘   │
│                                                          │
└─────────────────────────────────────────────────────────┘

COPY THIS ────────┐
   (for new       │
    projects)     │
                  ▼
              Your New
              Project
```

## Notes

- `agents/` is reusable across projects and should be copied as-is.
- `planning-mds/` is replaced for each new project.
- `inception-setup/` provides bootstrap guidance and examples.
- `engine/experience/soma/` are the implementation layers for the current solution.
