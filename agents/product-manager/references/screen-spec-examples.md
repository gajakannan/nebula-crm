# Screen Specification Examples

Use these examples when creating screen specifications for the insurance CRM. Screen specs should be stored in `planning-mds/INCEPTION.md` Section 3.5 or as separate files if needed.

**Best Practice:** Define 5-10 key screens for MVP, covering primary user workflows.

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
