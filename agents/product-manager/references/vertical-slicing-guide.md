# Vertical Slicing Guide

Comprehensive guide to breaking features into thin vertical slices that deliver end-to-end value.

## What is Vertical Slicing?

**Vertical Slice:** A feature increment that cuts through all architectural layers (UI → API → Business Logic → Database) and delivers complete user value.

**Horizontal Slice (Anti-pattern):** Building one layer at a time (all UI, then all backend, then all database).

---

## Why Vertical Slicing?

### Benefits

1. **Delivers Value Incrementally**
   - Each slice is a shippable, testable unit
   - Users get value sooner
   - Enables early feedback

2. **Reduces Integration Risk**
   - Integration happens continuously, not at the end
   - Problems discovered early when they're cheaper to fix
   - No "big bang" integration phase

3. **Enables Parallel Development**
   - Teams can work on different slices simultaneously
   - Clear boundaries reduce conflicts
   - Better utilization of team capacity

4. **Improves Estimation Accuracy**
   - Slices are small and concrete
   - Less uncertainty than large features
   - Velocity becomes more predictable

5. **Facilitates Testing**
   - Each slice is independently testable
   - QA can start testing slice 1 while slice 2 is being built
   - Faster feedback loops

---

## Vertical vs Horizontal Slicing

### Example: Broker Management Feature

#### ❌ Horizontal Slicing (DON'T DO THIS)

**Sprint 1: All UI**
- Build broker list screen
- Build broker detail screen
- Build broker create form
- Build broker edit form
- **Problem:** Nothing works without backend!

**Sprint 2: All APIs**
- Build GET /brokers
- Build GET /brokers/:id
- Build POST /brokers
- Build PUT /brokers/:id
- Build DELETE /brokers/:id
- **Problem:** Still can't test end-to-end!

**Sprint 3: All Database**
- Create broker table
- Create indexes
- Create migrations
- **Problem:** Integration hell begins!

**Sprint 4: Integration**
- Connect UI to API
- Debug issues
- Fix mismatches
- **Problem:** Surprises everywhere!

**Result:** 4 sprints, no working feature until the end

---

#### ✅ Vertical Slicing (DO THIS)

**Sprint 1, Slice 1: View Broker List (Read-Only)**
- UI: Broker list screen (table, pagination)
- API: GET /brokers with filtering
- DB: Broker table + sample data
- **Value:** Users can see brokers!
- **Testable:** End-to-end integration test

**Sprint 1, Slice 2: Create Broker**
- UI: Create broker form (name, license, state)
- API: POST /brokers with validation
- DB: Insert broker record
- Timeline: Log "Broker Created" event
- **Value:** Users can add new brokers!
- **Testable:** Full create flow works

**Sprint 2, Slice 3: View Broker 360**
- UI: Broker detail screen
- API: GET /brokers/:id
- DB: Join with related entities
- Timeline: Display timeline events
- **Value:** Users can see broker details!
- **Testable:** Navigation from list to detail works

**Sprint 2, Slice 4: Update Broker**
- UI: Edit broker form (pre-filled)
- API: PUT /brokers/:id
- DB: Update broker record
- Timeline: Log "Broker Updated" event with changes
- **Value:** Users can fix incorrect data!
- **Testable:** Full update flow works

**Sprint 3, Slice 5: Delete Broker (Soft Delete)**
- UI: Delete button with confirmation
- API: DELETE /brokers/:id (soft delete)
- DB: Update deleted_at timestamp
- Timeline: Log "Broker Deleted" event
- **Value:** Users can remove invalid brokers!
- **Testable:** Delete flow works, data preserved

**Result:** Working features delivered incrementally every sprint

---

## How to Slice Features

### Strategy 1: CRUD Operations

Most entities can be sliced by CRUD operations:

1. **Read (List):** View list of entities
2. **Create:** Add new entity
3. **Read (Detail):** View single entity details
4. **Update:** Modify existing entity
5. **Delete:** Remove entity

**Example: Account Management**
- Slice 1: View account list
- Slice 2: Create new account
- Slice 3: View account 360
- Slice 4: Update account information
- Slice 5: Delete account

---

### Strategy 2: Workflow States

For workflows, slice by state transitions:

**Example: Submission Workflow**

States: Received → Triaging → ReadyForUWReview → InReview → Quoted → Bound

**Slices:**
- Slice 1: Receive submission (Received state)
- Slice 2: Triage submission (Received → Triaging → ReadyForUWReview)
- Slice 3: Underwriter reviews (ReadyForUWReview → InReview)
- Slice 4: Generate quote (InReview → Quoted)
- Slice 5: Bind policy (Quoted → Bound)
- Slice 6: Handle declination (any state → Declined)
- Slice 7: Handle withdrawal (any state → Withdrawn)

---

### Strategy 3: Complexity Layers

Start simple, add complexity incrementally:

**Example: Broker Search**

- **Slice 1 (Simple):** Search by exact broker name
- **Slice 2 (Better):** Search by partial name (contains)
- **Slice 3 (Advanced):** Add license number search
- **Slice 4 (Filters):** Add state filter
- **Slice 5 (Polish):** Add status filter, sorting

---

### Strategy 4: Persona-Based

Slice by user role or persona:

**Example: Broker 360 Screen**

- **Slice 1:** Distribution user view (basic broker info + timeline)
- **Slice 2:** Underwriter view (add submission history)
- **Slice 3:** Relationship Manager view (add performance metrics)
- **Slice 4:** Admin view (add audit log, delete capability)

---

### Strategy 5: Happy Path First

Build happy path, then error cases:

**Example: Create Broker**

- **Slice 1:** Create broker with valid data (happy path)
- **Slice 2:** Validation errors (required fields, format checks)
- **Slice 3:** Business rule errors (duplicate license)
- **Slice 4:** Permission errors (unauthorized user)
- **Slice 5:** System errors (database unavailable)

---

## Slicing Patterns

### Pattern 1: Walking Skeleton

**Definition:** Thinnest possible slice that connects all architectural layers

**Purpose:** Prove integration works before building full features

**Example: Broker Management Walking Skeleton**
- UI: Display "Hello, Brokers" text
- API: GET /brokers returns `{"message": "Hello"}`
- DB: Simple health check query

**Value:** Confirms deployment pipeline, authentication, authorization all work

---

### Pattern 2: Defer Complexity

**Start Simple:**
- Hardcode values
- Skip optional features
- Use stub implementations

**Add Complexity Later:**
- Make it configurable
- Add optional features
- Implement full logic

**Example: Broker Status**
- Slice 1: All brokers are "Active" (hardcoded)
- Slice 2: Add "Inactive" status with toggle
- Slice 3: Add "Suspended" status with reason field
- Slice 4: Add automated status changes based on rules

---

### Pattern 3: Tracer Bullet

**Definition:** Build one specific use case end-to-end

**Purpose:** Prove the approach works for a concrete scenario

**Example: Submission Workflow**
- Tracer Bullet: One specific submission (General Liability for Restaurant)
- Build full flow for this case
- Then generalize to other coverage types

---

## Common Slicing Mistakes

### Mistake 1: Slicing by Technology Layer

❌ **Bad:**
- Story 1: Build React components
- Story 2: Build REST APIs
- Story 3: Build database schema

✅ **Good:**
- Story 1: View broker list (UI + API + DB)
- Story 2: Create broker (UI + API + DB)

---

### Mistake 2: Making Slices Too Large

❌ **Bad:**
- Story 1: Complete broker management system

✅ **Good:**
- Story 1: View broker list
- Story 2: Create broker
- Story 3: View broker details
- Story 4: Update broker
- Story 5: Delete broker

**Rule of Thumb:** If a story takes more than 1-2 weeks, it's too large

---

### Mistake 3: Slicing by Component

❌ **Bad:**
- Story 1: Broker header component
- Story 2: Broker form component
- Story 3: Broker table component

✅ **Good:**
- Story 1: View broker list (includes table component)
- Story 2: Create broker (includes form component)
- Story 3: View broker 360 (includes header component)

---

### Mistake 4: UI-Only or API-Only Stories

❌ **Bad:**
- Story 1: Implement broker API endpoints
- Story 2: Build broker UI screens

✅ **Good:**
- Story 1: View broker list (UI + API + DB)
- Story 2: Create broker (UI + API + DB)

**Exception:** Infrastructure or refactoring stories may be horizontal, but feature stories should always be vertical

---

## Slicing Workshop Exercise

### Original Requirement

> "As a Distribution Manager, I want to manage broker relationships including contacts, hierarchy, submissions, and performance metrics, so that I can optimize our broker network."

### Step 1: Identify Core Entities
- Broker
- Contact
- Hierarchy
- Submission
- Performance Metrics

### Step 2: Apply CRUD Pattern

**Broker:**
- S1: View broker list
- S2: Create broker
- S3: View broker 360
- S4: Update broker
- S5: Delete broker

**Contact (Separate Epic):**
- S6: View contacts for broker
- S7: Add contact to broker
- S8: Update contact
- S9: Remove contact

**Hierarchy (Separate Epic):**
- S10: View broker hierarchy
- S11: Assign parent broker
- S12: View sub-brokers

**Submissions (Separate Epic):**
- S13: View submissions for broker
- S14: Link submission to broker

**Performance Metrics (Future Phase):**
- Deferred - not MVP

### Step 3: Prioritize for MVP

**MVP (Must Have):**
- S1: View broker list ✓
- S2: Create broker ✓
- S3: View broker 360 ✓
- S4: Update broker ✓

**Phase 1 (Should Have):**
- S5: Delete broker
- S6: View contacts
- S7: Add contact
- S10: View hierarchy

**Future (Nice to Have):**
- S11-S14: Advanced features
- Performance metrics

---

## Real-World Example: Broker CRUD

### Slice 1: View Broker List (Read)

**What's Included:**
- UI: Broker list screen with table
- API: GET /brokers endpoint
- DB: Broker table with sample data
- Features: Pagination, sorting by name
- Test: Integration test for list retrieval

**What's Excluded (Defer):**
- Search/filtering (Slice 2)
- Bulk actions (Phase 1)
- Export (Phase 1)

**Acceptance Criteria:**
```
Given I navigate to /brokers
When the page loads
Then I see a table with columns: Name, License Number, State, Status
And I see 20 brokers per page
And I can navigate to next/previous pages
And I can sort by clicking column headers
```

**Estimate:** 3-5 days

---

### Slice 2: Create Broker (Create)

**What's Included:**
- UI: Create broker form (Name, License, State)
- API: POST /brokers with validation
- DB: Insert broker record
- Timeline: Log "Broker Created" event
- Test: Create flow end-to-end

**What's Excluded (Defer):**
- Contact management (Slice 6)
- Hierarchy assignment (Slice 10)
- Bulk import (Phase 1)

**Acceptance Criteria:**
```
Given I'm on the broker list
When I click "Add New Broker"
Then I see a create form with fields: Name, License Number, State
When I fill valid data and click Save
Then the broker is created
And I see success message "Broker created successfully"
And I'm redirected to Broker 360 view
And a timeline event "Broker Created" is logged
```

**Estimate:** 3-5 days

---

### Slice 3: View Broker 360 (Read Detail)

**What's Included:**
- UI: Broker detail screen with tabs
- API: GET /brokers/:id
- DB: Fetch broker with related data
- Timeline: Display recent timeline events
- Test: Navigation from list to detail

**What's Excluded (Defer):**
- Contacts tab (Slice 6)
- Submissions tab (Epic 3)
- Performance metrics (Future)

**Acceptance Criteria:**
```
Given I'm on the broker list
When I click on a broker name
Then I see the Broker 360 screen showing:
  - Broker name as page title
  - Basic info: License Number, State, Status
  - Activity timeline with recent events
  - Edit and Delete action buttons
```

**Estimate:** 3-5 days

---

### Slice 4: Update Broker (Update)

**What's Included:**
- UI: Edit broker form (pre-populated)
- API: PUT /brokers/:id
- DB: Update broker record
- Timeline: Log "Broker Updated" event with field changes
- Test: Update flow end-to-end

**What's Excluded (Defer):**
- Concurrent update handling (Phase 1)
- Version history (Future)
- Approval workflow (Future)

**Acceptance Criteria:**
```
Given I'm on Broker 360 screen
When I click Edit button
Then I see the edit form pre-filled with current values
When I change Name from "Acme" to "Acme Insurance"
And I click Save
Then the broker is updated
And I see success message "Broker updated successfully"
And I'm redirected to Broker 360 view
And a timeline event "Broker Updated" is logged showing:
  - Field: Name
  - Old Value: "Acme"
  - New Value: "Acme Insurance"
```

**Estimate:** 3-5 days

---

### Slice 5: Delete Broker (Delete)

**What's Included:**
- UI: Delete button with confirmation modal
- API: DELETE /brokers/:id (soft delete)
- DB: Set deleted_at timestamp
- Timeline: Log "Broker Deleted" event
- Test: Delete flow end-to-end

**What's Excluded (Defer):**
- Hard delete (Admin only, Phase 1)
- Restore deleted broker (Phase 1)
- Cascade delete prevention (Phase 1)

**Acceptance Criteria:**
```
Given I'm on Broker 360 screen
When I click Delete button
Then I see confirmation modal "Are you sure you want to delete this broker?"
When I click Confirm
Then the broker is soft deleted (deleted_at timestamp set)
And I see success message "Broker deleted successfully"
And I'm redirected to broker list
And the deleted broker no longer appears in the list
And a timeline event "Broker Deleted" is logged
```

**Estimate:** 2-3 days

---

## Checklist: Is This a Good Vertical Slice?

Use this checklist to validate your slices:

- [ ] **End-to-End:** Touches UI, API, and Database layers
- [ ] **User Value:** Delivers something a user can see and use
- [ ] **Independently Testable:** Can write integration tests for this slice alone
- [ ] **Small Enough:** Can be completed in 1-2 weeks
- [ ] **No Hidden Dependencies:** Doesn't require other incomplete work
- [ ] **Clear Scope:** Acceptance criteria are specific and testable
- [ ] **Deferrable Complexity:** Nice-to-haves are explicitly excluded
- [ ] **Shippable:** Could be deployed to production (if needed)

---

## Summary

**Key Principles:**
1. Each slice cuts through all layers (UI → API → DB)
2. Each slice delivers complete user value
3. Start simple, add complexity incrementally
4. Happy path first, error cases later
5. MVP essentials first, nice-to-haves later

**Benefits:**
- Faster feedback
- Reduced risk
- Better parallelization
- Clearer progress tracking
- Higher quality through continuous integration

**Remember:** A vertical slice should be **thin** (small scope) and **tall** (full stack), not **thick** (large scope) and **flat** (one layer).

---

## Version History

**Version 1.0** - 2026-01-26 - Initial vertical slicing guide
