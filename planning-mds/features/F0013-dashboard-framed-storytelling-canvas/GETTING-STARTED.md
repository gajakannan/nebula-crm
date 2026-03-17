# F0013 — Dashboard Framed Storytelling Canvas — Getting Started

## Prerequisites

- [ ] F0012 backend changes deployed (periodDays on `/dashboard/kpis`, avgDwellDays + emphasis on flow nodes)
- [ ] F0010 opportunities refactor baseline available (Pipeline Board + aging/hierarchy endpoints)
- [ ] Docker services running (PostgreSQL, authentik, engine API)
- [ ] Frontend dependencies installed (`cd experience && pnpm install`)
- [ ] Seed data loaded (submissions, renewals, workflow transitions, brokers — for meaningful flow/radial data)

## Services to Run

```bash
# Start backend services
docker compose up -d

# Start frontend dev server
cd experience && pnpm dev
```

## Environment Variables

No new environment variables. F0013 uses the same configuration as F0012/F0010.

## Seed Data

Ensure sufficient seed data for meaningful radial popover visualization:
- **Submissions:** At least 5-10 across different workflow stages
- **Renewals:** At least 3-5 across different workflow stages (needed for entity type composition in Flow chapter radials)
- **Workflow transitions:** Historical transitions for avgDwellDays computation
- **Brokers:** Multiple brokers for Mix chapter radial segments
- **Tasks:** Overdue tasks for nudge card display

## How to Verify

1. Navigate to `http://localhost:5173/` (dashboard)
2. **Three-layer hierarchy check:**
   - Nudge cards should have glass-card depth and glow on hover
   - Activity panel should have glass-card depth and glow on hover
   - My Tasks panel should have glass-card depth and glow on hover
   - KPI band and story controls should be flat (no card borders)
3. **Timeline bar check:**
   - Opportunity flow should render as a horizontal timeline with connected stage nodes
   - Nodes should be connected by flow ribbons
   - Terminal outcomes should branch off the right end
4. **Editorial palette check:**
   - Dark mode background should be deep navy (#1a2332), not graphite (#0b0f18)
   - Primary accents (buttons, links) should be muted coral, not violet
   - Secondary accents should be steel blue, not fuchsia
   - Light mode background should be warm gray (#f0eded), not cool white (#f4f6fb)
5. **Radial popover check:**
   - Hover a stage node — a radial/donut chart popover should appear
   - Count should display in the center
   - Segments should show entity type breakdown (Flow chapter default)
6. **Chapter controls check:**
   - Switch to Friction — all mini-visuals become uniform dwell-time donuts, timeline nodes gain emphasis rings
   - Switch to Outcomes — terminal branches glow, stage mini-visuals dim
   - Switch to Flow — contextual defaults restore, per-stop toggle indicators reappear
7. **Theme check:**
   - Toggle between dark and light themes
   - Glass-card depth and glow should be visible in both themes
8. **Responsive check:**
   - Resize browser to tablet/phone widths
   - Timeline should adapt (compact on tablet, vertical on phone)
   - Radial popovers should adapt (modal on tablet, bottom-sheet on phone)

## Key Files

| Layer | Path | Purpose |
|-------|------|---------|
| Frontend | `experience/src/pages/DashboardPage.tsx` | Main dashboard layout with three-layer hierarchy |
| Frontend | `experience/src/features/opportunities/components/` | Timeline bar, radial popovers, chapter overlays |
| Design | `planning-mds/screens/design-tokens.md` | Glass-card, glow, accent color tokens |
| Design | `planning-mds/screens/handdrawn.jpeg` | Original wireframe showing the framed canvas vision |
| Design | `planning-mds/screens/S-DASH-002-framed-storytelling-canvas.md` | Screen specification |
| Spec | `planning-mds/schemas/opportunity-flow-node.schema.json` | Flow node schema (includes avgDwellDays, emphasis) |

## Notes

- This feature corrects F0012's over-flattening. The key insight: "infographic" applies to the **story canvas zone only**, not to operational panels (nudges, activity, tasks).
- Glass-card and glow classes already exist in `design-tokens.md` — S0001 restores their usage on dashboard components.
- No new backend endpoints — all data comes from existing F0010/F0012 endpoints.
- Radial chart segments are computed frontend-side by cross-referencing stage data with chapter-specific data sources.
