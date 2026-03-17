# F0013-S0005: Ensure Responsive, Accessibility, and Performance Parity for Framed Storytelling Canvas

**Story ID:** F0013-S0005
**Feature:** F0013 — Dashboard Framed Storytelling Canvas
**Title:** Ensure responsive, accessibility, and performance parity for framed storytelling canvas
**Priority:** Medium
**Phase:** MVP

## User Story

**As a** dashboard user on any device or using assistive technology
**I want** the framed storytelling canvas to adapt gracefully across desktop, tablet, and phone, remain fully keyboard and screen-reader accessible, and meet performance budgets
**So that** the dashboard experience is consistent and inclusive regardless of device or ability

## Context & Background

The framed storytelling canvas introduces new visualization components (timeline bar, radial popovers, three-layer visual hierarchy) that need responsive adaptation, accessibility compliance, and performance validation. The timeline bar must adapt to narrower viewports, radial popovers must work with touch, and glass-card panels must maintain their depth across themes and screen sizes.

## Acceptance Criteria

**Happy Path:**
- **Given** the framed storytelling canvas is fully implemented (S0001–S0004)
- **When** the user accesses the dashboard from any device or with assistive technology
- **Then** the canvas adapts to viewport width, maintains keyboard/screen-reader accessibility, and meets performance budgets as specified below

**Alternative Flows / Edge Cases:**
- Very slow network (>3s API response) → Skeleton placeholders render for timeline, radial areas, and glass-card panels; no layout shift when data arrives
- User resizes browser from desktop to phone width → Layout transitions smoothly without jarring reflows
- All stages have count = 0 → Timeline renders as empty nodes with "No activity in period" message; no radial popovers shown
- `prefers-reduced-motion: reduce` → Glow pulse animations disabled; static soft shadow retained for depth hierarchy
- Screen reader + keyboard-only user → Full navigation flow works: Tab → chapter controls → stage nodes → popover → dismiss → next node

### Responsive Layout

**Desktop (1280px+):**
- [ ] Timeline bar renders horizontally with all stage nodes visible without scrolling
- [ ] Radial popovers render above/below stage nodes with full detail
- [ ] Nudge cards, Activity, and Tasks panels render as glass-card panels with depth
- [ ] Collapsible left nav and right Neuron rail work with adaptive canvas width

**Tablet Landscape (1024px):**
- [ ] Timeline bar renders horizontally, nodes may be more compact
- [ ] Radial popovers remain functional on tap
- [ ] Rails default to collapsed; canvas takes full width

**Tablet Portrait (768px):**
- [ ] Timeline bar may switch to a compact/condensed mode (fewer label characters, tighter spacing)
- [ ] Radial popovers render as modal overlays instead of inline popovers (to avoid clipping)
- [ ] Glass-card panels stack vertically with maintained depth

**Phone (375px):**
- [ ] Timeline bar switches to a vertical/stacked layout (stages top-to-bottom)
- [ ] Radial popovers render as bottom-sheet overlays on tap
- [ ] Glass-card panels stack vertically, full-width
- [ ] Chapter controls render as a horizontally scrollable pill group
- [ ] Period selector renders as a compact dropdown

### Accessibility

- [ ] Timeline stage nodes are keyboard-navigable (Tab to enter timeline, Arrow keys between nodes, Enter/Space to open popover)
- [ ] Radial chart popovers have `role="dialog"` with `aria-label` describing stage and composition
- [ ] Popover dismisses on Escape key
- [ ] Chapter controls have `role="tablist"` with `aria-selected` on active chapter
- [ ] Glass-card panels maintain WCAG AA text contrast in both dark and light themes
- [ ] Glow effects respect `prefers-reduced-motion` (disable glow animations, keep static visual indicators)
- [ ] Focus ring is visible on all interactive elements in both themes
- [ ] Screen reader announces: stage label, count, and composition summary when popover opens

### Performance

- [ ] Dashboard page interactive (LCP) < 2.5s on desktop
- [ ] Timeline bar SVG render < 200ms after data arrival
- [ ] Radial popover appears < 100ms after hover/tap
- [ ] Chapter switch visual update < 150ms
- [ ] No lazy chapter data loads — all chapter data eagerly loaded at mount
- [ ] Initial page fires no more than 6 parallel API requests (nudges + KPIs + flow + outcomes + tasks + activity)
- [ ] Total JavaScript bundle for dashboard route < 150kB gzipped
- [ ] No layout shift from data loading (skeleton placeholders for initial load)

### Collapsible Rails

- [ ] 4 rail states work correctly: both expanded, left collapsed, right collapsed, both collapsed
- [ ] Canvas width adapts smoothly (CSS custom properties `--sidebar-width`, `--chat-panel-width`)
- [ ] Timeline bar nodes scale proportionally with available width (no overflow/scroll)
- [ ] Glass-card panels maintain their depth in all rail states

## Data Requirements

No new data requirements. This story validates existing data flows across devices and interaction modes.

## Role-Based Visibility

All roles see the same responsive/accessible behavior.

## Non-Functional Expectations

See Acceptance Criteria above — this story IS the non-functional validation story.

## Dependencies

**Depends On:**
- F0013-S0001 — Three-layer visual hierarchy must be established
- F0013-S0002 — Timeline bar must be built
- F0013-S0003 — Radial popovers must be built
- F0013-S0004 — Chapter controls must be connected

**Related Stories:**
- All prior F0013 stories — this is the cross-cutting validation

## Out of Scope

- Native mobile app (this is responsive web only)
- Offline support
- PWA installation

## UI/UX Notes

- Phone timeline: vertical layout mirrors the horizontal timeline rotated 90 degrees. Stage nodes stack top-to-bottom. Flow ribbons render as vertical connectors.
- Radial popovers on phone: render as a bottom-sheet with the radial chart centered and detail fields below it.
- `prefers-reduced-motion`: disable glow pulse/transition animations. Keep static glow (always-on soft shadow) as a fallback so the depth hierarchy is still visible.

## Questions & Assumptions

**Open Questions:**
- [ ] Should the phone layout hide chapter controls behind a "More" menu, or keep all 3 visible in a scrollable pill group? (Assumption: scrollable pill group — 3 pills fit comfortably)

**Assumptions:**
- SVG timeline bar scales naturally with viewport width via `viewBox` — no JavaScript resize logic needed
- Radial popovers can reuse the existing popover/modal component infrastructure (shadcn Popover / Dialog)

## Definition of Done

- [ ] Acceptance criteria met across all 4 breakpoints
- [ ] Accessibility checklist passed
- [ ] Performance budgets met
- [ ] Rail collapse states verified
- [ ] Both dark and light themes verified
- [ ] Tests pass (component + E2E across breakpoints)
- [ ] Story filename matches Story ID prefix
- [ ] Story index regenerated
