---
template: screen-specification
version: 1.0
applies_to: product-manager
---

# Screen Specification Template

Use this template to define screen layouts, data, and interactions from a product perspective (non-technical).

## Screen Header

**Screen ID:** [Unique identifier, e.g., SCR-001]
**Screen Name:** [User-facing name, e.g., "Broker 360 View"]
**Screen Type:** [List | Detail | Form | Dashboard | Modal | Drawer]
**Parent Screen:** [Screen ID of parent, if nested navigation]
**Route/URL:** [Logical route, e.g., `/brokers/:id` - non-technical]

---

## Purpose & Context

**Primary Purpose:** [What is this screen for?]

**User Goal:** [What does the user want to accomplish on this screen?]

**When Used:** [Context or trigger for accessing this screen]

**Personas Who Use This:**
- [Persona 1]: [What they do here]
- [Persona 2]: [What they do here]

---

## Screen Layout (Conceptual)

Describe the layout in sections, not technical components.

### Header Section
**Content:**
- [Element 1, e.g., "Broker name as page title"]
- [Element 2, e.g., "Edit and Delete action buttons"]
- [Element 3, e.g., "Status badge showing broker status"]

### Main Content Section
**Content:**
- [Element 1, e.g., "Broker details card with key information"]
- [Element 2, e.g., "Activity timeline showing recent events"]
- [Element 3, e.g., "Related submissions list"]

### Sidebar Section (if applicable)
**Content:**
- [Element 1, e.g., "Quick stats: submission count, premium total"]
- [Element 2, e.g., "Related contacts list"]

### Footer Section (if applicable)
**Content:**
- [Element 1, e.g., "Audit information: created by, last modified"]

---

## Data Fields

List all data displayed on this screen. Describe fields from business perspective, not database columns.

### Primary Data (Always Visible)
| Field Name | Description | Format/Type | Required | Editable |
|------------|-------------|-------------|----------|----------|
| Broker Name | Legal business name | Text | Yes | Yes |
| License Number | State license ID | Text | Yes | Yes |
| State | Licensed state | Dropdown | Yes | Yes |
| Status | Active/Inactive/Suspended | Badge | Yes | Yes |
| Email | Primary contact email | Email | No | Yes |
| Phone | Primary contact phone | Phone | No | Yes |

### Calculated/Derived Fields
| Field Name | Description | How Calculated |
|------------|-------------|----------------|
| Total Submissions | Count of submissions | Sum of related submissions |
| Last Activity Date | Most recent event | Max date from timeline |
| Premium YTD | Year-to-date premium | Sum of bound premiums |

### Related Data (Sections)
**Activity Timeline:**
- Event type, timestamp, user, description
- Display: Most recent 20 events, paginated

**Related Submissions:**
- Submission ID, insured name, status, date
- Display: Most recent 10, link to view all

**Contacts:**
- Contact name, title, email, phone
- Display: All contacts for this broker

---

## User Actions

List all actions users can perform on this screen.

### Primary Actions
| Action | Button/Link | Behavior | Permission Required |
|--------|-------------|----------|---------------------|
| Edit Broker | "Edit" button | Navigate to edit form | UpdateBroker |
| Delete Broker | "Delete" button | Show confirmation modal, then delete | DeleteBroker |
| Add Submission | "+ New Submission" button | Navigate to submission form with broker pre-filled | CreateSubmission |
| Add Contact | "+ Add Contact" button | Open contact form drawer | ManageContacts |

### Secondary Actions
| Action | Button/Link | Behavior | Permission Required |
|--------|-------------|----------|---------------------|
| View Hierarchy | "View Hierarchy" link | Navigate to broker hierarchy view | ViewBroker |
| Export Data | "Export" icon | Download broker data as CSV | ViewBroker |
| View Audit Log | "Audit" icon | Navigate to full audit log | ViewAuditLog |

### Contextual Actions (Conditional)
| Action | When Visible | Behavior |
|--------|--------------|----------|
| Activate Broker | When status is Inactive | Change status to Active, log event |
| Suspend Broker | When status is Active | Change status to Suspended, require reason |

---

## Navigation & Flow

### Entry Points (How Users Get Here)
- From Broker List: Click on broker name in table
- From Submission Detail: Click on broker name link
- From Search: Select broker from global search results
- From URL: Direct navigation via bookmark or link

### Exit Points (Where Users Go From Here)
- Back to Broker List: Breadcrumb or back button
- To Edit Broker: Edit button
- To Submission Detail: Click submission in related list
- To Contact Detail: Click contact name in contacts list

---

## Interactions & Behaviors

### On Page Load
1. [Action 1, e.g., "Fetch broker data by ID from URL"]
2. [Action 2, e.g., "Load recent activity timeline events"]
3. [Action 3, e.g., "Display loading state while data fetches"]

### Real-Time Updates
- [Behavior 1, e.g., "When submission status changes, update related submissions count"]
- [Behavior 2, e.g., "When new timeline event created, prepend to timeline"]

### User Interactions
- **Hover over timeline event:** Show full event details in tooltip
- **Click on submission ID:** Navigate to submission detail screen
- **Click "Load More" in timeline:** Fetch next 20 events

### Form Validation (if applicable)
- [Validation rule 1, e.g., "License number must be unique"]
- [Validation rule 2, e.g., "Email must be valid format"]

---

## States & Conditions

### Loading State
- Display: Skeleton loaders for content sections
- Behavior: Show while data is being fetched

### Empty State
- **When:** No related submissions exist
- **Display:** "No submissions yet. Create your first submission."
- **Action:** Button to create new submission

### Error State
- **When:** Broker not found or unauthorized access
- **Display:** "Broker not found" or "You don't have permission to view this broker"
- **Action:** Button to return to broker list

### Success State
- **When:** Action completed (e.g., broker updated)
- **Display:** Toast notification "Broker updated successfully"
- **Duration:** 3 seconds

---

## Responsive Behavior (Optional)

### Desktop (> 1024px)
- [Layout description, e.g., "Three-column layout with sidebar"]

### Tablet (768px - 1024px)
- [Layout description, e.g., "Two-column layout, sidebar collapses"]

### Mobile (< 768px)
- [Layout description, e.g., "Single column, actions in overflow menu"]

---

## Role-Based Visibility

Different roles see/do different things on this screen.

| Role | What They Can See | What They Can Do |
|------|-------------------|------------------|
| Distribution User | All data | View, Edit, Add Submission |
| Underwriter | All data | View, Add Submission |
| Relationship Manager | All data | View, Edit, Add Submission, Manage Contacts |
| Admin | All data + audit metadata | All actions |
| Broker (external) | Public data only (future) | View own data (future) |

---

## Integration Points

### Data Sources
- [Source 1, e.g., "Broker data from Broker service"]
- [Source 2, e.g., "Timeline events from ActivityTimeline service"]
- [Source 3, e.g., "Submissions from Submission service"]

### External Links/Integrations
- [Integration 1, e.g., "Link to state licensing board website"]
- [Integration 2, e.g., "Email client opens when clicking email address"]

---

## Accessibility Considerations

- [Consideration 1, e.g., "Page title should announce broker name for screen readers"]
- [Consideration 2, e.g., "Action buttons must have descriptive aria-labels"]
- [Consideration 3, e.g., "Timeline events must be keyboard navigable"]

---

## Edge Cases & Special Scenarios

### Scenario 1: Broker Has No Submissions
- Display empty state with call-to-action to create first submission

### Scenario 2: Broker Is Suspended
- Display warning banner: "This broker is suspended. Reason: [reason]"
- Disable "Add Submission" action

### Scenario 3: User Lacks Edit Permission
- Hide Edit and Delete buttons
- Show read-only view

### Scenario 4: Broker Was Deleted (Soft Delete)
- Display "This broker has been deleted" banner
- Show "Restore" button for admins

---

## Wireframe or Mockup (Optional)

[Link to Figma, Sketch, or embedded image]

**Placeholder Text Description:**
```
+--------------------------------------------------+
| [< Back to List]  Broker 360: Acme Insurance     |
|                                    [Edit] [Delete] |
+--------------------------------------------------+
| Broker Details Card                              |
| - Name: Acme Insurance                           |
| - License: CA-12345                              |
| - Status: Active                                 |
+--------------------------------------------------+
| Activity Timeline                                |
| - [Icon] Broker Created - 2024-01-15 by John D   |
| - [Icon] Contact Added - 2024-01-20 by Sarah M   |
| - [Icon] Submission Received - 2024-02-01        |
|                                   [Load More...] |
+--------------------------------------------------+
| Related Submissions (5)                          |
| - SUB-001 | ABC Corp | Quoted | 2024-01-25       |
| - SUB-002 | XYZ Inc  | Bound  | 2024-02-01       |
|                                    [View All...] |
+--------------------------------------------------+
```

---

## Open Questions

- [ ] [Question 1, e.g., "Should we display soft-deleted contacts in the contacts list?"]
- [ ] [Question 2, e.g., "What's the max number of timeline events to load initially?"]

---

## Related Screens

**Parent Screen:** Broker List (SCR-002)
**Child Screens:**
- Edit Broker Form (SCR-003)
- Broker Hierarchy View (SCR-004)

**Related Screens:**
- Submission Detail (SCR-010)
- Contact Detail (SCR-020)

---

## Version History

**Version 1.0** - [Date] - Initial screen specification
**Version 1.1** - [Date] - [Changes based on user feedback]

---

## Example Usage

See `product-manager/references/screen-spec-examples.md` for complete examples of detailed screen specifications.
