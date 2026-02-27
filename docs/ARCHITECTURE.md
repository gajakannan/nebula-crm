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
│  │ • 11 Roles   │         │ • Nebula CRM Specs       │   │
│  │ • Templates  │         │ • Insurance Domain       │   │
│  │ • References │         │ • Features/Stories       │   │
│  └──────────────┘         └──────────────────────────┘   │
│         │                            │                  │
│         │                            │                  │
│         ▼                            ▼                  │
│  ┌──────────────┐         ┌──────────────────────────┐   │
│  │ blueprint-   │         │ engine/experience/neuron/│   │
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
- `blueprint-setup/` provides bootstrap guidance and examples.
- `engine/experience/neuron/` are the implementation layers for the current solution.
- Architect orchestrates app assembly from planning artifacts into implementation layers.
