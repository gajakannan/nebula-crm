# Screen Specification Examples

Use these examples when creating screen specifications for the insurance CRM. Screen specs should be stored in `planning-mds/BLUEPRINT.md` Section 3.5 or as separate files if needed.

**Best Practice:** Define 5-10 key screens for MVP, covering primary user workflows.

---

## Screen 0: Dashboard

**Screen ID:** SCR-DASH-001
**Screen Name:** Dashboard
**Screen Type:** Widget-based landing page
**Route:** `/dashboard` (default after login)
**Parent Navigation:** Main navigation → Dashboard (home icon)

### Purpose
Provide an at-a-glance operational command center on login. Surfaces the most urgent items, key performance indicators, pipeline health, assigned tasks, and recent broker activity — all scoped to the logged-in user's authorization. This is the first screen every user sees after authentication.

### Target Users
- **Primary:** Distribution User (intake/triage), Relationship Manager (broker context)
- **Secondary:** Underwriter (pipeline/submission focus), Program Manager (program-level view)
- **Admin:** Sees unscoped data across all entities

### Layout & Structure — Full Desktop Wireframe (>1200px)

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│  ☰  Nebula          [🔍 Global Search...              ]    🔔 3    👤 Sarah ▾  │
├──────┬──────────────────────────────────────────────────────────────────────────┤
│      │                                                                          │
│ 🏠   │  NEEDS YOUR ATTENTION                                            [Hide] │
│ Dash │  ┌──────────────────────┐ ┌──────────────────────┐ ┌──────────────────┐ │
│      │  │ 🔴 Overdue Task    ✕ │ │ 🟠 Stale Sub       ✕ │ │ 🔵 Renewal Due ✕ │ │
│ 👥   │  │                      │ │                      │ │                  │ │
│ Brkr │  │ Follow up with Acme  │ │ Sub #2044 stuck in   │ │ Atlas Ins renews │ │
│      │  │ Insurance — 3 days   │ │ WaitingOnBroker for  │ │ in 8 days, still │ │
│ 📋   │  │ overdue              │ │ 7 days               │ │ in Created stage │ │
│ Subs │  │                      │ │                      │ │                  │ │
│      │  │  [ Review Now  ]     │ │  [ Take Action ]     │ │ [Start Outreach] │ │
│ 🔄   │  └──────────────────────┘ └──────────────────────┘ └──────────────────┘ │
│ Rnwl │                                                                          │
│      │  ┌───────────┐ ┌───────────┐ ┌───────────┐ ┌───────────┐               │
│ ✅   │  │  Active   │ │   Open    │ │  Renewal  │ │   Avg     │               │
│ Tasks│  │  Brokers  │ │   Subs    │ │   Rate    │ │ Turnaround│               │
│      │  │           │ │           │ │           │ │           │               │
│ ⚙️   │  │    142    │ │    38     │ │   72%     │ │  4.2 d    │               │
│ Admin│  │           │ │           │ │           │ │           │               │
│      │  └───────────┘ └───────────┘ └───────────┘ └───────────┘               │
│      │                                                                          │
│      │  SUBMISSIONS PIPELINE                                                    │
│      │  ┌────────┐   ┌────────┐   ┌────────┐   ┌────────┐   ┌────────┐       │
│      │  │Received│──▶│Triaging│──▶│WaitOn  │──▶│Ready   │──▶│InReview│──...   │
│      │  │   12   │   │    8   │   │Broker 5│   │ForUW  7│   │    7   │       │
│      │  └────────┘   └────────┘   └───┬────┘   └────────┘   └────────┘       │
│      │                                 │ ▼ (expanded on click)                  │
│      │                    ┌─────────────────────────────┐                       │
│      │                    │ WaitingOnBroker (5)         │                       │
│      │                    │ ┌─────────────────────────┐ │                       │
│      │                    │ │ Acme Corp  $45K   12d JM│ │                       │
│      │                    │ │ Beta LLC   $22K    9d SC│ │                       │
│      │                    │ │ Coral Inc  $18K    7d MR│ │                       │
│      │                    │ │ Delta Co   $15K    6d JM│ │                       │
│      │                    │ │ Echo Grp   $12K    5d SC│ │                       │
│      │                    │ ├─────────────────────────┤ │                       │
│      │                    │ │      View all 5 →       │ │                       │
│      │                    │ └─────────────────────────┘ │                       │
│      │                    └─────────────────────────────┘                       │
│      │                                                                          │
│      │  RENEWALS PIPELINE                                                       │
│      │  ┌────────┐   ┌────────┐   ┌────────┐   ┌────────┐   ┌────────┐       │
│      │  │Created │──▶│ Early  │──▶│Outreach│──▶│InReview│──▶│ Quoted │       │
│      │  │    4   │   │    6   │   │Started 3│  │    2   │   │    1   │       │
│      │  └────────┘   └────────┘   └────────┘   └────────┘   └────────┘       │
│      │                                                                          │
│      │  ┌──────────────────────────────────┐ ┌────────────────────────────────┐ │
│      │  │ MY TASKS & REMINDERS             │ │ BROKER ACTIVITY FEED           │ │
│      │  │                                  │ │                                │ │
│      │  │ 🔴 Follow up with Acme Ins       │ │ 🟢 Sarah added broker          │ │
│      │  │    Due: Jan 20 (3d overdue)  [→] │ │   "Pacific Reinsurance"        │ │
│      │  │                                  │ │    2 hours ago             [→] │ │
│      │  │ 🟠 Review submission #2044       │ │                                │ │
│      │  │    Due: Feb 15 (tomorrow)    [→] │ │ 🔵 License updated for         │ │
│      │  │                                  │ │   "Atlas Insurance Group"       │ │
│      │  │ ⚪ Renewal outreach — Atlas      │ │    4 hours ago             [→] │ │
│      │  │    Due: Feb 18 (4 days)      [→] │ │                                │ │
│      │  │                                  │ │ 🔵 Contact added to            │ │
│      │  │ ⚪ Prep for broker QBR           │ │   "Meridian Brokers"            │ │
│      │  │    Due: Feb 20 (6 days)      [→] │ │    yesterday               [→] │ │
│      │  │                                  │ │                                │ │
│      │  │ ⚪ Update program terms           │ │ 🟢 New submission via          │ │
│      │  │    Due: Feb 22 (8 days)      [→] │ │   "Coastal MGA Partners"       │ │
│      │  │                                  │ │    yesterday               [→] │ │
│      │  │         View all tasks →         │ │                                │ │
│      │  └──────────────────────────────────┘ │ 🔵 Broker "Summit Re" status   │ │
│      │                                       │   changed to Active             │ │
│      │                                       │    2 days ago              [→] │ │
│      │                                       │                                │ │
│      │                                       │ ... (20 items max)             │ │
│      │                                       └────────────────────────────────┘ │
│      │                                                                          │
└──────┴──────────────────────────────────────────────────────────────────────────┘
```

### Layout — Tablet Wireframe (768px – 1200px)

```
┌─────────────────────────────────────────────┐
│  ☰  Nebula    [🔍 Search...  ]  🔔  👤     │
├─────────────────────────────────────────────┤
│                                             │
│ NEEDS YOUR ATTENTION                        │
│ ┌─────────────────────┐ ┌─────────────────┐│
│ │ 🔴 Follow up Acme ✕ │ │ 🟠 Sub #2044  ✕ ││
│ │  3 days overdue      │ │  7 days stuck   ││
│ │  [ Review Now ]      │ │  [Take Action]  ││
│ └─────────────────────┘ └─────────────────┘│
│ ┌─────────────────────┐                    │
│ │ 🔵 Atlas renewal  ✕ │                    │
│ │  8 days remaining    │                    │
│ │  [Start Outreach]   │                    │
│ └─────────────────────┘                    │
│                                             │
│ ┌──────────┐┌──────────┐┌──────────┐┌─────┐│
│ │ Active   ││  Open    ││ Renewal  ││ Avg ││
│ │ Brokers  ││  Subs    ││  Rate    ││Turn ││
│ │   142    ││   38     ││  72%     ││4.2d ││
│ └──────────┘└──────────┘└──────────┘└─────┘│
│                                             │
│ SUBMISSIONS PIPELINE                        │
│ ┌─────┐┌─────┐┌─────┐┌─────┐┌─────┐┌────┐│
│ │Rcvd ││Tria ││Wait ││Rdy  ││InRv ││Quot││
│ │ 12  ││  8  ││  5  ││  7  ││  7  ││ 4  ││
│ └─────┘└─────┘└─────┘└─────┘└─────┘└────┘│
│                                             │
│ RENEWALS PIPELINE                           │
│ ┌──────┐┌──────┐┌──────┐┌──────┐┌──────┐  │
│ │Crtd  ││Early ││Outrch││InRev ││Quoted│  │
│ │  4   ││  6   ││  3   ││  2   ││  1   │  │
│ └──────┘└──────┘└──────┘└──────┘└──────┘  │
│                                             │
│ MY TASKS & REMINDERS                        │
│ ┌─────────────────────────────────────────┐ │
│ │ 🔴 Follow up with Acme  Jan 20 (3d) [→]│ │
│ │ 🟠 Review sub #2044     Feb 15 (1d) [→]│ │
│ │ ⚪ Renewal — Atlas       Feb 18 (4d) [→]│ │
│ │          View all tasks →               │ │
│ └─────────────────────────────────────────┘ │
│                                             │
│ BROKER ACTIVITY FEED                        │
│ ┌─────────────────────────────────────────┐ │
│ │ 🟢 Pacific Re added        2h ago  [→] │ │
│ │ 🔵 Atlas license updated   4h ago  [→] │ │
│ │ 🔵 Meridian contact added  ytd     [→] │ │
│ │ 🟢 Coastal MGA submission  ytd     [→] │ │
│ └─────────────────────────────────────────┘ │
│                                             │
└─────────────────────────────────────────────┘
```

### Layout — Mobile Wireframe (<768px)

```
┌─────────────────────────────┐
│  ☰  Nebula          🔔  👤 │
├─────────────────────────────┤
│                             │
│ NEEDS YOUR ATTENTION        │
│ ┌─────────────────────────┐ │
│ │ 🔴 Follow up Acme     ✕ │ │
│ │   3 days overdue         │ │
│ │   [ Review Now ]         │ │
│ └─────────────────────────┘ │
│ ┌─────────────────────────┐ │
│ │ 🟠 Sub #2044 stuck    ✕ │ │
│ │   7 days in WaitBroker   │ │
│ │   [ Take Action ]        │ │
│ └─────────────────────────┘ │
│ ┌─────────────────────────┐ │
│ │ 🔵 Atlas renewal due  ✕ │ │
│ │   8 days remaining       │ │
│ │   [ Start Outreach ]     │ │
│ └─────────────────────────┘ │
│                             │
│ ┌────────────┐┌────────────┐│
│ │Active Brkrs││ Open Subs  ││
│ │    142     ││    38      ││
│ └────────────┘└────────────┘│
│ ┌────────────┐┌────────────┐│
│ │Renewal Rate││ Avg Turn   ││
│ │   72%      ││   4.2 d    ││
│ └────────────┘└────────────┘│
│                             │
│ PIPELINE          [▾ Subs ] │
│ ┌─────┐┌─────┐┌─────┐      │
│ │Rcvd ││Tria ││Wait │ >>>  │
│ │ 12  ││  8  ││  5  │scroll│
│ └─────┘└─────┘└─────┘      │
│                             │
│ MY TASKS          View all →│
│ ┌─────────────────────────┐ │
│ │🔴 Follow up Acme    3d  │ │
│ │🟠 Review #2044      1d  │ │
│ │⚪ Renewal Atlas      4d  │ │
│ └─────────────────────────┘ │
│                             │
│ ACTIVITY          View all →│
│ ┌─────────────────────────┐ │
│ │🟢 Pacific Re added  2h  │ │
│ │🔵 Atlas updated     4h  │ │
│ │🔵 Meridian contact  ytd │ │
│ └─────────────────────────┘ │
│                             │
└─────────────────────────────┘
```

### Widget Specifications

#### Widget 1: Nudge Cards ("Needs Your Attention")

**Position:** Top of content area, above KPI cards
**Visibility:** Only rendered when at least 1 nudge qualifies; hidden entirely when none exist

**Card Anatomy:**
```
┌──────────────────────────────┐
│ 🔴 [Type Icon]  [Title]   ✕ │   ← Dismiss button (top-right)
│                              │
│ [Description — 1-2 lines     │   ← Contextual detail
│  with urgency indicator]     │
│                              │
│  [ CTA Button Label ]        │   ← Primary action button
└──────────────────────────────┘
```

**Fields per card:**

| Field | Type | Source | Example |
|-------|------|--------|---------|
| Type Icon | Icon + color | NudgeType | 🔴 (overdue), 🟠 (stale), 🔵 (upcoming) |
| Title | Text, bold | Task/Submission/Renewal name | "Follow up with Acme Insurance" |
| Description | Text, secondary | Computed urgency string | "3 days overdue" / "7 days in WaitingOnBroker" |
| Linked Entity | Text, tertiary | EntityName | "Broker: Acme Insurance" |
| CTA Label | Button text | NudgeType mapping | "Review Now" / "Take Action" / "Start Outreach" |
| Dismiss (✕) | Icon button | Client-side | Hides card for session |

**Color mapping:**

| Nudge Type | Icon Color | Background Tint | CTA Style |
|------------|-----------|------------------|-----------|
| Overdue Task | Red (destructive-500) | Red-50 | Primary/Red |
| Stale Submission | Amber (warning-500) | Amber-50 | Primary/Amber |
| Upcoming Renewal | Blue (info-500) | Blue-50 | Primary/Blue |

**Behavior:**
- Max 3 cards displayed simultaneously
- Priority fill order: overdue tasks → stale submissions → upcoming renewals
- Dismiss (✕) hides card for current session; replaced by next eligible if available
- CTA click navigates to linked entity detail screen
- If all dismissed or none qualify, entire section collapses with smooth animation
- "[Hide]" link in section header collapses all nudges for the session

#### Widget 2: KPI Metrics Cards

**Position:** Below nudge cards (or at top if no nudges)

**Card Anatomy:**
```
┌───────────────┐
│  [Label]      │   ← Secondary text, muted
│               │
│   [Value]     │   ← Large text, bold, primary color
│               │
│  [Unit]       │   ← Small text, muted (e.g., "count", "%", "days")
└───────────────┘
```

**Fields per card:**

| Card | Label | Value Source | Unit | Computation |
|------|-------|-------------|------|-------------|
| Active Brokers | "Active Brokers" | Broker table | count | COUNT WHERE Status = Active, ABAC-scoped |
| Open Submissions | "Open Submissions" | Submission table | count | COUNT WHERE CurrentStatus NOT IN (Bound, Declined, Withdrawn), ABAC-scoped |
| Renewal Rate | "Renewal Rate" | Renewal table | % | (Bound / (Bound + Lost + Lapsed)) * 100, trailing 90 days, ABAC-scoped |
| Avg Turnaround | "Avg Turnaround" | Submission + WorkflowTransition | days | AVG(first terminal transition OccurredAt - Submission.CreatedAt), trailing 90 days |

**Layout:** 4 cards in a horizontal row, equal width. Responsive: 2x2 grid on tablet, 2x2 on mobile.

**States:**
- Loading: Skeleton pulse animation per card
- Data available: Show value
- No data / insufficient data: Display "—"
- Query failure: Display "—" and log error; do not block other widgets

#### Widget 3: Pipeline Summary (Mini-Kanban)

**Position:** Below KPI cards, full width
**Sub-sections:** Submissions Pipeline (top), Renewals Pipeline (below)

**Collapsed pill anatomy:**
```
┌────────────┐
│ [Label]    │   ← Status name, truncated if needed
│   [Count]  │   ← Bold count badge
└────────────┘
```

**Expanded popover anatomy (on hover/click):**
```
┌─────────────────────────────────┐
│ [StatusLabel] ([Count])         │   ← Header
│ ┌─────────────────────────────┐ │
│ │ [Name]   [Amount] [Days] [A]│ │   ← Mini-card row
│ │ [Name]   [Amount] [Days] [A]│ │   ← Entity name, $amount, days-in-status, avatar
│ │ [Name]   [Amount] [Days] [A]│ │
│ │ [Name]   [Amount] [Days] [A]│ │
│ │ [Name]   [Amount] [Days] [A]│ │
│ ├─────────────────────────────┤ │
│ │       View all N →          │ │   ← Link to filtered list
│ └─────────────────────────────┘ │
└─────────────────────────────────┘
```

**Pill color mapping:**

| Stage Group | Statuses | Color |
|-------------|----------|-------|
| Intake | Received, Created | Slate/Gray |
| Triage | Triaging, Early | Blue |
| Waiting | WaitingOnBroker, OutreachStarted | Amber |
| Review | ReadyForUWReview, InReview | Indigo |
| Decision | Quoted, BindRequested | Green |

**Mini-card fields:**

| Field | Type | Format | Example |
|-------|------|--------|---------|
| Entity Name | Text, bold | Account/Broker name, max 20 chars + ellipsis | "Acme Corp" |
| Amount | Currency, right-aligned | $NNK (thousands) or $N.NM (millions) | "$45K" |
| Days in Status | Chip/badge | Nd | "12d" |
| Assigned User | Avatar circle | 2-char initials | "JM" |

**Behavior:**
- Pills are horizontally scrollable if they overflow (mobile/tablet)
- Hover (desktop) or tap (mobile) expands popover below the pill
- Only one popover open at a time; opening another closes the previous
- Popover shows top 5 items sorted by DaysInStatus descending
- Mini-card click → entity detail screen
- "View all N →" link → filtered list screen
- Popover repositions to stay within viewport (flip above if near bottom)
- Mini-card data is lazy-loaded (fetched on expand, not on dashboard load)

#### Widget 4: My Tasks & Reminders

**Position:** Bottom-left (desktop), full-width stacked (tablet/mobile)

**Task row anatomy:**
```
│ [●] [Task Title]                [Due Date] [→] │
│     [Linked Entity Name]        [Relative]      │
```

**Fields per task row:**

| Field | Type | Format | Example |
|-------|------|--------|---------|
| Status indicator | Colored dot | 🔴 overdue, 🟠 due today/tomorrow, ⚪ future | 🔴 |
| Task Title | Text, bold | Max 40 chars + ellipsis | "Follow up with Acme Insurance" |
| Linked Entity | Text, secondary | EntityType: EntityName | "Broker: Acme Insurance" |
| Due Date | Date | MMM DD | "Jan 20" |
| Relative Due | Text, muted | Relative indicator | "(3d overdue)" / "(tomorrow)" / "(4 days)" |
| Navigate arrow | Icon button | [→] | Navigates to entity or Task Center |

**Color coding:**

| Condition | Indicator | Row Style |
|-----------|-----------|-----------|
| DueDate < today | 🔴 Red dot | Red-50 background, red left border |
| DueDate = today or tomorrow | 🟠 Amber dot | Amber-50 background |
| DueDate > tomorrow | ⚪ Gray dot | No highlight |
| DueDate is null | ⚪ Gray dot | "No due date" in date column |

**Behavior:**
- Max 10 rows displayed; "View all tasks →" link if more exist
- Sorted by DueDate ascending (soonest/most overdue first), nulls last
- Only Open and InProgress tasks shown (Done excluded)
- Click row or [→] → linked entity detail or Task Center
- Empty state: "No tasks assigned. You're all caught up." with a subtle checkmark illustration

#### Widget 5: Broker Activity Feed

**Position:** Bottom-right (desktop), below tasks (tablet/mobile)

**Feed item anatomy:**
```
│ [●] [Event Description]                        │
│     [Broker Name]            [Timestamp]   [→]  │
```

**Fields per feed item:**

| Field | Type | Format | Example |
|-------|------|--------|---------|
| Event icon | Colored dot | 🟢 created, 🔵 updated, 🟡 status change | 🟢 |
| Event Description | Text, bold | Human-readable from EventType + payload | "New broker added" |
| Actor | Text, inline | "by [DisplayName]" | "by Sarah Chen" |
| Broker Name | Text, secondary, link-styled | Broker.LegalName | "Pacific Reinsurance" |
| Timestamp | Text, muted, right-aligned | Relative time | "2 hours ago" |
| Navigate arrow | Icon button | [→] | Navigates to Broker 360 |

**Event icon mapping:**

| EventType | Icon/Color | Description Template |
|-----------|-----------|---------------------|
| BrokerCreated | 🟢 Green | "New broker added by [Actor]" |
| BrokerUpdated | 🔵 Blue | "Broker updated by [Actor]" |
| BrokerStatusChanged | 🟡 Yellow | "Status changed to [NewStatus] by [Actor]" |
| ContactAdded | 🔵 Blue | "Contact added by [Actor]" |
| ContactUpdated | 🔵 Blue | "Contact updated by [Actor]" |
| SubmissionCreated | 🟢 Green | "New submission received via [BrokerName]" |
| LicenseUpdated | 🔵 Blue | "License updated by [Actor]" |

**Behavior:**
- Max 20 items, sorted by OccurredAt descending
- Click broker name or [→] → Broker 360 view
- Actor resolved from UserProfile.DisplayName; shows "Unknown User" if user deactivated
- Relative timestamps computed client-side; absolute time on hover tooltip
- Empty state: "No recent broker activity."
- No pagination or "load more" in MVP

### User Actions (Dashboard-Level)

**1. Interact with Nudge Card**
- **Trigger:** Click CTA button on nudge card
- **Permission:** Read access to linked entity (ABAC)
- **Navigation:** → Entity detail screen (Broker 360, Submission Detail, Renewal Detail, or Task Center)

**2. Dismiss Nudge Card**
- **Trigger:** Click ✕ on nudge card
- **Permission:** None (client-side action)
- **Behavior:** Card slides out, replaced by next eligible nudge (if any); state stored in session

**3. Expand Pipeline Pill**
- **Trigger:** Hover (desktop) or tap (touch) on a pipeline status pill
- **Permission:** Read access to submissions/renewals (ABAC)
- **Behavior:** Lazy-load mini-cards for that status; display popover below pill

**4. Navigate from Pipeline Mini-Card**
- **Trigger:** Click a mini-card in expanded popover
- **Permission:** Read access to specific submission/renewal (ABAC)
- **Navigation:** → Submission Detail or Renewal Detail screen

**5. Navigate from Pipeline "View All"**
- **Trigger:** Click "View all N →" in popover
- **Permission:** Read access to list (ABAC)
- **Navigation:** → Submission List or Renewal List, pre-filtered by status

**6. Navigate from Task Row**
- **Trigger:** Click task row or [→] arrow
- **Permission:** Task must be assigned to user
- **Navigation:** → Linked entity detail or Task Center

**7. Navigate from Activity Feed Item**
- **Trigger:** Click broker name or [→] arrow
- **Permission:** Read access to broker (ABAC)
- **Navigation:** → Broker 360 view for that broker

**8. View All Tasks**
- **Trigger:** Click "View all tasks →" link
- **Permission:** Authenticated user
- **Navigation:** → Task Center screen

### Error States & Messages

**Widget-Level Failures (Isolated):**
Each widget fails independently. A failed widget must not prevent other widgets from rendering.

| Widget | Failure Display | Log Action |
|--------|----------------|------------|
| Nudge Cards | Section not rendered (silent) | Log error with correlation ID |
| KPI Cards | Individual card shows "—" | Log error per failed metric |
| Pipeline Summary | "Unable to load pipeline data" in widget area | Log error |
| My Tasks | "Unable to load tasks" in widget area | Log error |
| Activity Feed | "Unable to load activity feed" in widget area | Log error |

**Full Page Errors:**

**Authentication Expired:**
- **Condition:** JWT expired during page load
- **Message:** "Your session has expired. Please log in again."
- **Action:** Redirect to Keycloak login

**Network Error:**
- **Condition:** All API calls fail (network down)
- **Message:** "Unable to connect to Nebula. Check your connection and try again."
- **Action:** [Retry] button refreshes all widgets

**Permission Denied:**
- **Condition:** User lacks dashboard access (edge case — all internal users should have access)
- **Message:** "You don't have permission to view the dashboard. Contact your administrator."

**Loading State:**
- **Condition:** Initial page load
- **Display:** Skeleton layout matching widget structure: 3 card skeletons for nudges, 4 card skeletons for KPIs, pill-shaped skeletons for pipeline, list skeletons for tasks and feed
- **Duration:** Typically < 1s; skeleton shown until all widget data arrives or 2s timeout

### Responsive Behavior

**Desktop (>1200px):**
- Left sidebar navigation (240px) + content area
- Nudge cards: 3 across in one row
- KPI cards: 4 across in one row
- Pipeline: full-width horizontal pills, popover below
- Tasks & Activity Feed: 2-column layout (50/50 split)

**Tablet (768px – 1200px):**
- Collapsed sidebar (icon-only, 64px) or hamburger menu
- Nudge cards: 2 across, third wraps to second row
- KPI cards: 4 across (compressed)
- Pipeline: abbreviated status labels (e.g., "Rcvd", "Tria", "Wait")
- Tasks & Activity Feed: stacked vertically (full width each)

**Mobile (<768px):**
- Bottom tab navigation (no sidebar)
- Nudge cards: stacked vertically (1 per row)
- KPI cards: 2x2 grid
- Pipeline: horizontal scroll with pill overflow (swipe to see all statuses); toggle between Submissions/Renewals via dropdown
- Tasks: condensed rows (title + due only), max 5 shown
- Activity Feed: condensed rows (description + time only), max 5 shown
- Each section has a "View all →" link

### Accessibility

- **Keyboard Navigation:**
  - Tab order: Nudge cards → KPI cards → Pipeline pills → Tasks → Activity Feed
  - Enter/Space on pipeline pill opens popover; Escape closes it
  - Arrow keys navigate within popover mini-cards
  - Enter on task/activity row navigates to detail
- **Screen Reader:**
  - Dashboard page announced as "Dashboard — Nebula CRM"
  - Each widget section has an aria-label (e.g., "Key Performance Indicators", "Submissions Pipeline")
  - KPI cards: aria-label="Active Brokers: 142"
  - Pipeline pills: aria-label="WaitingOnBroker: 5 submissions"
  - Nudge cards: role="alert" for overdue items, role="status" for informational
  - Popover: role="dialog", aria-labelledby referencing status label
- **Focus Indicators:** Visible focus ring (2px blue outline) on all interactive elements
- **Color Independence:** All status indicators use icons/shapes in addition to color (colorblind-safe)
- **Motion:** Respect prefers-reduced-motion; disable slide/collapse animations if set

### Performance Requirements

| Metric | Target | Notes |
|--------|--------|-------|
| Dashboard full load (all widgets) | p95 < 2s | From authenticated page load to all widgets rendered |
| KPI card data | p95 < 500ms | Server-side aggregation; may use pre-computed counts |
| Pipeline pill counts | p95 < 500ms | Grouped COUNT query per entity type |
| Pipeline popover (mini-cards) | p95 < 300ms | Lazy-loaded on hover/click; indexed query |
| Tasks widget | p95 < 300ms | Filtered by AssignedTo with DueDate sort |
| Activity feed | p95 < 300ms | Indexed on (EntityType, OccurredAt DESC) |
| Nudge card computation | p95 < 500ms | Parallel queries across tasks, submissions, renewals |
| Nudge dismiss animation | < 200ms | Client-side CSS transition |

### Data Refresh

- **On page load:** All widgets fetch fresh data
- **No auto-refresh in MVP:** Data is static until page reload or manual navigation back to dashboard
- **Future:** Consider polling (every 60s) or WebSocket push for real-time updates

---

## Screen 1: Broker List

**Screen ID:** SCR-BR-001
**Screen Name:** Broker List
**Screen Type:** List/Table View with Search and Filters
**Route:** `/brokers`
**Parent Navigation:** Main navigation → Brokers

### Purpose
Allow Distribution users to view, search, filter, and navigate to all broker records in the system. This is the primary entry point for broker management.

### Target Users
- **Primary:** Distribution & Marketing Manager (Sarah)
- **Secondary:** Broker Relationship Coordinator (Jennifer)
- **Read-only:** Underwriters (Marcus) - can view broker details but not manage

### Layout & Structure

**Page Header:**
- Title: "Brokers"
- Breadcrumb: Home > Brokers
- Action Buttons: [Add New Broker] (primary), [Export List] (secondary), [Import Brokers] (secondary, Phase 1)

**Search Bar:**
- Placeholder: "Search brokers by name, license number, or state..."
- Real-time search (debounced, 300ms delay)
- Minimum 2 characters to trigger search
- Search icon on left, clear icon (X) on right when text entered

**Filters Section (Collapsible):**
- Status: Dropdown [All, Active, Inactive, Pending Approval]
- State: Multi-select dropdown [All states]
- License Type: Dropdown [All, Retail, MGA, Surplus Lines]
- Production Tier: Dropdown [All, Platinum (>$5M), Gold ($1M-$5M), Silver ($250K-$1M), Bronze (<$250K)]
- Last Activity: Date range picker [Last 7 days, Last 30 days, Last 90 days, Custom Range]

**Results Table:**
Columns (sortable):
1. Broker Name (text, link to Broker 360, sortable)
2. License Number (text, sortable)
3. State (2-letter code, badge, sortable)
4. Status (Active/Inactive, colored badge, filterable, sortable)
5. Production Tier (Platinum/Gold/Silver/Bronze, badge with icon, sortable)
6. Premium YTD (currency, right-aligned, sortable, default sort DESC)
7. Last Activity Date (date, relative time on hover, sortable)
8. Actions (kebab menu: View, Edit, Deactivate)

**Pagination:**
- Show 25 entries per page (configurable: 10, 25, 50, 100)
- Page numbers with prev/next
- Total count: "Showing 1-25 of 143 brokers"

### Data Fields Detail

| Field Name | Type | Format | Validation | Source |
|------------|------|--------|------------|--------|
| Broker Name | Text | Full legal name | Required, max 200 chars | Broker.Name |
| License Number | Text | State-specific format | Required, unique | Broker.LicenseNumber |
| State | Dropdown | 2-letter code (CA, NY, etc.) | Required, valid US state | Broker.State |
| Status | Badge | Active (green), Inactive (gray), Pending (yellow) | Required | Broker.Status |
| Production Tier | Badge | Based on Premium YTD | Calculated field | Calculated from Policy.Premium |
| Premium YTD | Currency | $X,XXX,XXX (no cents for >$1M) | Auto-calculated | Sum(Policy.Premium WHERE Year=Current) |
| Last Activity Date | Date | MM/DD/YYYY, hover shows time | Auto-updated | Max(Activity.Timestamp) |

### User Actions

**1. View Broker Detail**
- **Trigger:** Click on broker name (anywhere in row)
- **Permission:** ReadBroker
- **Navigation:** → Broker 360 screen (SCR-BR-002)
- **State:** Opens in same tab

**2. Search Brokers**
- **Trigger:** Type in search box (min 2 chars)
- **Permission:** ReadBroker
- **Behavior:** Filters results in real-time; searches Name, License Number, and State fields
- **Feedback:** Show "Searching..." spinner if query takes >500ms

**3. Filter Brokers**
- **Trigger:** Select filter options
- **Permission:** ReadBroker
- **Behavior:** Apply AND logic to all filters; update results immediately
- **Feedback:** Show active filter chips above table; "Clear All Filters" button if >0 filters active

**4. Sort Table**
- **Trigger:** Click column header
- **Permission:** ReadBroker
- **Behavior:** Toggle ASC/DESC; show arrow indicator in header
- **Default Sort:** Premium YTD DESC (highest producers first)

**5. Add New Broker**
- **Trigger:** Click "Add New Broker" button
- **Permission:** CreateBroker
- **Navigation:** → Create Broker Form screen (SCR-BR-003)
- **State:** Opens as modal dialog (overlay) or new page (TBD - UX decision)

**6. Export List**
- **Trigger:** Click "Export List" button
- **Permission:** ExportData
- **Behavior:** Download current filtered results as CSV file (all fields + hidden metadata)
- **Feedback:** "Preparing export..." toast → "Download started" toast
- **Filename:** `brokers-export-YYYY-MM-DD-HHMMSS.csv`

**7. Edit Broker**
- **Trigger:** Click "Edit" in actions menu (kebab)
- **Permission:** UpdateBroker
- **Navigation:** → Edit Broker Form screen (SCR-BR-004)
- **State:** Opens as modal or new page

**8. Deactivate Broker**
- **Trigger:** Click "Deactivate" in actions menu
- **Permission:** UpdateBroker
- **Behavior:** Show confirmation dialog → Update Status to Inactive → Refresh list
- **Validation:** Cannot deactivate if broker has active submissions or renewals (show error)

### Error States & Messages

**No Results:**
- **Condition:** Search or filter returns 0 results
- **Message:** "No brokers found matching your criteria. Try adjusting your filters or search terms."
- **Action:** [Clear All Filters] button

**Search Error:**
- **Condition:** Search API fails (500 error)
- **Message:** "Unable to search brokers. Please try again. If the problem persists, contact support."
- **Action:** [Retry] button

**Permission Denied:**
- **Condition:** User lacks ReadBroker permission
- **Message:** "You don't have permission to view brokers. Contact your administrator."
- **Action:** None (show message in place of table)

**Loading State:**
- **Condition:** Initial data load or filter change
- **Display:** Skeleton table with 10 rows (shimmer effect)
- **Duration:** Typically <500ms; show spinner if >1 second

**Network Error:**
- **Condition:** API request fails (network timeout, 503)
- **Message:** "Unable to load brokers. Check your connection and try again."
- **Action:** [Retry] button

### Responsive Behavior

**Desktop (>1200px):**
- Show all 8 columns
- Filters expanded by default
- Pagination at bottom

**Tablet (768px - 1200px):**
- Hide "Production Tier" and "Last Activity" columns (available in detail view)
- Show 6 columns
- Filters collapsed by default (expand on click)

**Mobile (<768px):**
- Card-based layout (stacked, not table)
- Show: Broker Name, State badge, Status badge, Premium YTD
- Tap card → Broker 360
- Search bar full-width
- Filters in slide-out panel (hamburger icon)

### Accessibility

- **Keyboard Navigation:** Tab through search, filters, table rows; Enter to open Broker 360
- **Screen Reader:** Announce filter changes ("Filtered by Status: Active, 87 results"); table headers labeled
- **Focus Indicators:** Clear blue outline on focused elements
- **ARIA Labels:** data-testid attributes on key elements for automated testing

### Performance Requirements

- **Initial Load:** <1 second for first 25 records
- **Search Response:** <300ms for filtered results (local or API)
- **Sort/Filter:** <200ms (client-side if <500 records; server-side if >500)
- **Export:** Start download within 2 seconds for <5,000 records

---

## Screen 2: Broker 360 (Detail View)

**Screen ID:** SCR-BR-002
**Screen Name:** Broker 360 View
**Screen Type:** Detail View with Tabbed Sections
**Route:** `/brokers/{brokerId}`
**Parent Navigation:** Broker List → Broker Detail

### Purpose
Provide a comprehensive, single-screen view of all broker information, relationships, activity, and performance metrics. This is the "golden record" for broker data.

### Target Users
- **Primary:** Distribution & Marketing Manager (Sarah), Underwriters (Marcus)
- **Secondary:** Broker Relationship Coordinator (Jennifer)

### Layout & Structure

**Page Header:**
- **Breadcrumb:** Home > Brokers > [Broker Name]
- **Title:** Broker legal name (H1)
- **Status Badge:** Active/Inactive/Pending (colored, right side of header)
- **Action Buttons:** [Edit Broker], [View Submissions], [New Submission], [Deactivate] (in dropdown)

**Overview Card (Top Section):**
- **Left Column:**
  - License Number
  - License Type (Retail, MGA, Surplus Lines)
  - State(s) licensed in (comma-separated or "See all" if >3)
  - Primary Contact (name, phone, email with mailto link)
- **Right Column:**
  - Production Tier badge (Platinum/Gold/Silver/Bronze)
  - Premium YTD (large, bold)
  - Submission Count YTD
  - Quote-to-Bind Ratio YTD (%)
  - Last Activity Date

**Tabbed Content Sections:**
1. **Overview Tab** (default)
2. **Accounts Tab**
3. **Submissions Tab**
4. **Activity Timeline Tab**
5. **Contacts Tab**
6. **Documents Tab**
7. **Notes Tab**

### Tab 1: Overview

**Broker Information:**
- Legal Name
- DBA (if different)
- License Number(s)
- License Type
- States Licensed
- Tax ID / EIN
- Website (clickable link)
- Main Office Address
- Main Phone Number

**Relationship Details:**
- Assigned Distribution Manager (Sarah)
- Secondary Contact (Coordinator)
- Account Manager at Broker (name, title, email, phone)
- Relationship Status (Active, Inactive, Probation, Terminated)
- Start Date (when relationship began)
- Contract Expiration Date (if applicable)

**Performance Metrics (Current Year):**
- Premium Written YTD (bar chart, compare to goal)
- Submission Count (by month, line chart)
- Quote Rate (%)
- Bind Rate (%)
- Average Policy Premium
- Top Lines of Business (pie chart: GL 40%, Property 30%, WC 20%, Auto 10%)

**Quick Stats (Lifetime):**
- Total Premium Written (all time)
- Total Policies Bound
- Years as Partner
- Net Promoter Score (if available)

### Tab 2: Accounts

**Purpose:** List all accounts (insureds) submitted through this broker

**Table Columns:**
1. Account Name (link to Account 360)
2. Account Type (Individual, Business, Public Entity)
3. Lines of Business (badges: GL, Property, WC, etc.)
4. Total Premium (current year)
5. Policy Count (active policies)
6. Last Activity Date
7. Status (Active, Lapsed, Cancelled)

**Actions:**
- Click account name → Navigate to Account 360
- Filter by Status, Line of Business
- Sort by Premium (DESC default)

### Tab 3: Submissions

**Purpose:** List all submissions from this broker (new business and renewals)

**Table Columns:**
1. Submission ID (link to Submission Detail)
2. Account Name
3. Line of Business (badge)
4. Submission Date
5. Status (Intake, Triaging, Quoted, Bound, Declined, Expired)
6. Assigned Underwriter (if applicable)
7. Requested Premium
8. Quote Premium (if quoted)
9. Days in Pipeline

**Filters:**
- Status (dropdown multi-select)
- Line of Business (dropdown multi-select)
- Date Range (last 30/90/365 days, custom)
- Underwriter (if Distribution Manager has visibility)

**Actions:**
- Click Submission ID → Submission Detail screen
- [New Submission] button → Create Submission form

### Tab 4: Activity Timeline

**Purpose:** Chronological feed of all broker-related events (audit trail + activity log)

**Event Types:**
- Broker record created/updated (who, when, what changed)
- New submission received
- Submission status changed
- Policy bound
- Meeting scheduled/completed
- Note added
- Document uploaded
- Email sent/received (if integrated)
- Phone call logged

**Display Format:**
- Reverse chronological (newest first)
- Left timeline indicator with icon for event type
- Event description (e.g., "Submission SUB-00123 created by Sarah Chen")
- Timestamp (relative time: "2 hours ago", absolute on hover: "Jan 31, 2026 2:15 PM")
- User avatar if action performed by user
- Expandable details for complex events

**Filters:**
- Event Type (dropdown multi-select)
- Date Range
- User (who performed action)

### Tab 5: Contacts

**Purpose:** List all contacts (people) at the broker organization

**Table Columns:**
1. Name (First Last)
2. Title/Role
3. Email (mailto link)
4. Phone (tel link)
5. Mobile (tel link)
6. Preferred Contact Method (Email, Phone, SMS)
7. Primary Contact (checkbox, only one can be primary)
8. Active (checkbox)

**Actions:**
- [Add Contact] button → Contact form modal
- Click row → Edit contact modal
- [Delete] icon (soft delete, requires confirmation)

### Tab 6: Documents

**Purpose:** Store and retrieve all broker-related documents

**Document Categories:**
- Licenses & Certifications
- Contracts & Agreements
- Marketing Materials
- Performance Reports
- Correspondence
- Other

**Display:**
- Grouped by category (expandable sections)
- File name, type (icon), size, uploaded by, upload date
- Download, View (if previewable), Delete (if permission)

**Actions:**
- [Upload Document] button → File picker + category selector
- Drag & drop to upload
- Click file name → Download or preview in modal

### Tab 7: Notes

**Purpose:** Internal notes about the broker (not visible to broker)

**Display:**
- Reverse chronological list
- Note text (rich text: bold, italic, bullets, links)
- Created by (user name, avatar)
- Created date/time
- Edited indicator if modified (show edit history on hover)

**Actions:**
- [Add Note] button → Rich text editor
- Edit own notes (pencil icon)
- Delete own notes (requires confirmation)
- Pin important notes to top (pin icon)

### Navigation & Actions

**Top-Level Actions:**

**1. Edit Broker**
- **Trigger:** Click [Edit Broker] button
- **Permission:** UpdateBroker
- **Navigation:** → Edit Broker Form (SCR-BR-004) or inline editing
- **State:** Modal overlay or replace content

**2. View Submissions**
- **Trigger:** Click [View Submissions] button
- **Permission:** ReadSubmission
- **Behavior:** Activate "Submissions" tab (shortcut)

**3. New Submission**
- **Trigger:** Click [New Submission] button
- **Permission:** CreateSubmission
- **Navigation:** → Create Submission form (SCR-SUB-001) with broker pre-filled
- **State:** New page or modal

**4. Deactivate Broker**
- **Trigger:** Click [Deactivate] in dropdown menu
- **Permission:** UpdateBroker
- **Behavior:** Confirmation dialog → Update status → Refresh view
- **Validation:** Warn if active submissions/policies exist; require reason for deactivation

### Error States

**Broker Not Found:**
- **Condition:** Invalid brokerId in URL
- **Message:** "Broker not found. It may have been deleted or you don't have permission to view it."
- **Action:** [Back to Broker List] button

**Permission Denied:**
- **Condition:** User lacks ReadBroker permission for this specific broker
- **Message:** "You don't have permission to view this broker."
- **Action:** [Back to Broker List]

**Data Load Error:**
- **Condition:** API fails to load broker data
- **Message:** "Unable to load broker details. Please try again."
- **Action:** [Retry] button

### Responsive Behavior

**Desktop (>1200px):**
- Overview card + tabs side-by-side (70/30 split)
- All tabs visible in tab bar

**Tablet (768px-1200px):**
- Overview card full-width (collapsible)
- Tabs below overview
- Hide less-used tabs in "More" dropdown

**Mobile (<768px):**
- Stacked layout (Overview → Tabs)
- Tabs as accordion sections (expandable)
- Action buttons in floating action button (FAB) menu

---

## Screen 3: Create Broker Form

**Screen ID:** SCR-BR-003
**Screen Name:** Create Broker
**Screen Type:** Form (Multi-Step Wizard)
**Route:** `/brokers/new`
**Parent Navigation:** Broker List → Add New Broker

### Purpose
Allow Distribution users to create a new broker record with required information. Form validates data and creates audit trail entry.

### Target Users
- **Primary:** Distribution & Marketing Manager (Sarah)
- **Secondary:** Broker Relationship Coordinator (Jennifer)

### Layout & Structure

**Form Type:** Multi-step wizard (3 steps)

**Step Indicator (Top):**
```
[1] Basic Information  →  [2] Licensing & Contacts  →  [3] Review & Create
```

**Step 1: Basic Information**

**Fields:**
1. **Legal Name** (required)
   - Type: Text input
   - Validation: Max 200 chars, min 3 chars
   - Help text: "Full legal business name as it appears on license"
   - Error: "Legal name is required"

2. **DBA / Doing Business As** (optional)
   - Type: Text input
   - Validation: Max 200 chars
   - Help text: "If different from legal name"

3. **Broker Type** (required)
   - Type: Radio buttons
   - Options: Retail Broker, MGA (Managing General Agent), Surplus Lines Broker, Reinsurance Broker
   - Default: Retail Broker

4. **Primary State** (required)
   - Type: Dropdown (searchable)
   - Options: All 50 US states + DC
   - Validation: Must select one
   - Help text: "State where broker is primarily licensed"

5. **Main Office Address** (required)
   - Street Address (required)
   - City (required)
   - State (pre-filled from Primary State, editable)
   - ZIP Code (required, format validation: 12345 or 12345-6789)

6. **Main Phone Number** (required)
   - Type: Phone input with formatting
   - Validation: (XXX) XXX-XXXX format
   - Error: "Valid phone number required"

7. **Website** (optional)
   - Type: URL input
   - Validation: Valid URL format (https://...)
   - Help text: "Broker's website (if available)"

**Step 1 Actions:**
- [Cancel] button → Confirmation dialog if data entered → Return to Broker List
- [Next] button → Validate Step 1 fields → Proceed to Step 2

**Step 2: Licensing & Contacts**

**Fields:**
8. **License Number** (required)
   - Type: Text input
   - Validation: Alphanumeric, max 50 chars, unique (check against existing)
   - Error: "License number is required and must be unique"
   - Help text: "Primary license number"

9. **Additional States Licensed** (optional)
   - Type: Multi-select dropdown (searchable)
   - Options: All 50 US states + DC (excluding Primary State)
   - Help text: "Select all states where broker holds active licenses"

10. **Tax ID / EIN** (required)
    - Type: Text input with formatting
    - Validation: XX-XXXXXXX format
    - Error: "Valid EIN required (XX-XXXXXXX format)"

11. **Primary Contact Name** (required)
    - Type: Text input
    - Validation: Min 3 chars
    - Help text: "Main point of contact at broker organization"

12. **Primary Contact Email** (required)
    - Type: Email input
    - Validation: Valid email format
    - Error: "Valid email address required"

13. **Primary Contact Phone** (required)
    - Type: Phone input
    - Validation: Phone format
    - Can be same as main phone number

14. **Primary Contact Title** (optional)
    - Type: Text input
    - Example: "Vice President", "Account Executive"

**Step 2 Actions:**
- [Back] button → Return to Step 1 (preserve data)
- [Cancel] button → Confirmation dialog → Discard and return to list
- [Next] button → Validate Step 2 → Proceed to Step 3

**Step 3: Review & Create**

**Display:**
- Read-only summary of all entered data
- Grouped by section (Basic Information, Licensing, Contacts)
- [Edit] links next to each section → Return to relevant step

**Final Fields:**
15. **Assigned Distribution Manager** (required)
    - Type: Dropdown (searchable)
    - Options: All users with Distribution Manager role
    - Default: Current user
    - Help text: "Who will manage this broker relationship?"

16. **Initial Status** (required)
    - Type: Radio buttons
    - Options: Active (default), Pending Approval, Inactive
    - Help text: "Brokers marked 'Pending' require approval before activating"

17. **Notes** (optional)
    - Type: Textarea
    - Placeholder: "Add any initial notes about this broker..."
    - Max 1000 chars

**Step 3 Actions:**
- [Back] button → Return to Step 2
- [Cancel] button → Confirmation dialog → Discard
- [Create Broker] button (primary, prominent) → Validate all → Submit → Create record

### Form Validation

**Field-Level Validation:**
- Triggered on blur (lose focus)
- Show inline error messages below field
- Red border on invalid field
- Prevent advancing to next step if current step has errors

**Form-Level Validation:**
- Duplicate license number check (real-time API call on blur)
- Email format validation
- Phone format validation
- Required field checks

**Error Summary:**
- If submit fails, show error summary at top of form
- List all validation errors with links to jump to field
- Scroll to first error field

### Success Flow

**On Successful Creation:**
1. Show success toast: "Broker [Name] created successfully"
2. Create audit trail entry: "Broker created by [User] on [Date]"
3. Navigate to Broker 360 view of newly created broker
4. Auto-expand "Overview" tab

### Error States

**Duplicate License Number:**
- **Condition:** License number already exists in system
- **Message:** "A broker with this license number already exists. [View Broker]"
- **Action:** Link to existing broker record

**Network Error:**
- **Condition:** Submit API call fails
- **Message:** "Unable to create broker. Check your connection and try again."
- **Action:** [Retry] button (preserves form data)

**Validation Error:**
- **Condition:** Server-side validation fails
- **Message:** "Please correct the errors below and try again"
- **Action:** Show error summary + highlight invalid fields

**Permission Denied:**
- **Condition:** User loses CreateBroker permission mid-session
- **Message:** "You no longer have permission to create brokers. Contact your administrator."
- **Action:** [Back to Broker List]

### Auto-Save & Data Loss Prevention

**Auto-Save to Local Storage:**
- Save form data to browser local storage every 10 seconds
- On page reload, check for saved data → Offer to restore: "You have unsaved changes. Restore them?"

**Navigation Warning:**
- If user tries to leave page with unsaved data:
- Show browser confirmation: "You have unsaved changes. Are you sure you want to leave?"

### Accessibility

- All fields have associated labels (not just placeholders)
- Tab order follows logical flow (top to bottom, left to right)
- Required fields marked with asterisk and aria-required
- Error messages associated with fields via aria-describedby
- Focus moves to first error on validation failure
- Keyboard shortcuts: Ctrl+Enter to submit (from any field)

### Performance

- Form loads in <500ms
- Validation feedback within 100ms of blur event
- Submit processing <1 second for successful creation
- License number uniqueness check <300ms

---

## How to Use These Screen Specs

### During Story Writing:
- Reference screen IDs in acceptance criteria: "Then navigate to SCR-BR-001 (Broker List)"
- Use field names consistently: "When I enter Broker Name..."
- Link user stories to specific screens/actions

### During Design/Wireframing:
- Use these specs as requirements for mockups
- Don't deviate from specified fields/validation without PM approval
- Design mobile-first, then scale up to tablet/desktop

### During Development:
- Implement all specified validations
- Follow responsive behavior guidelines
- Test all error states
- Implement accessibility requirements

### During QA/Testing:
- Test all user actions listed in spec
- Verify all error states appear correctly
- Test responsive behavior on all screen sizes
- Validate accessibility with screen reader

---

**Last Updated:** 2026-01-31
**Version:** 2.0
