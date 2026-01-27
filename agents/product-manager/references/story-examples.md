# User Story Examples

Complete examples of well-written user stories for BrokerHub, following best practices.

## Story S1: Create New Broker

**Epic:** E1 - Broker & MGA Relationship Management
**Priority:** Critical
**Phase:** MVP

---

### User Story

**As a** Distribution & Marketing Manager
**I want** to create a new broker record with basic information
**So that** I can start tracking relationship activity and submissions for this broker

---

### Context & Background

Currently, brokers are tracked in spreadsheets, making it difficult to manage relationships, track submissions, and maintain data quality. This story establishes the foundation for broker relationship management in BrokerHub.

This is the first story in the Broker CRUD vertical slice. It depends on the Broker table being created (part of Phase 0 infrastructure), but has no other dependencies.

---

### Acceptance Criteria

#### Happy Path

**Navigation to Form:**
- **Given** I'm logged in as a Distribution user
- **And** I'm on the Broker List screen
- **When** I click the "Add New Broker" button
- **Then** I'm navigated to the Create Broker form
- **And** the form displays empty fields except for default values

**Successful Creation:**
- **Given** I'm on the Create Broker form
- **When** I fill in the following valid data:
  - Name: "Acme Insurance Brokers"
  - License Number: "CA-12345"
  - State: "California"
  - Email: "contact@acmeins.com"
  - Phone: "+1-555-0100"
- **And** I click the "Save" button
- **Then** the broker is created in the database
- **And** I see a success toast message: "Broker created successfully"
- **And** I'm redirected to the Broker 360 view for the newly created broker
- **And** the broker appears in the Broker List

---

#### Validation Errors

**Missing Required Fields:**
- **Given** I'm on the Create Broker form
- **When** I leave the Name field blank
- **And** I click "Save"
- **Then** I see an inline error message under Name field: "Name is required"
- **And** the form is not submitted
- **And** the broker is not created

**Duplicate License Number:**
- **Given** a broker with License Number "CA-12345" already exists
- **When** I try to create a new broker with License Number "CA-12345"
- **And** I click "Save"
- **Then** I see an error message: "A broker with this license number already exists"
- **And** the form is not submitted
- **And** the broker is not created

**Invalid Email Format:**
- **Given** I'm on the Create Broker form
- **When** I enter "invalid-email" in the Email field
- **And** I click "Save"
- **Then** I see an inline error: "Please enter a valid email address"
- **And** the form is not submitted

**Invalid Phone Format:**
- **Given** I'm on the Create Broker form
- **When** I enter "abc" in the Phone field
- **And** I click "Save"
- **Then** I see an inline error: "Please enter a valid phone number"
- **And** the form is not submitted

---

#### Permission Errors

**Unauthorized User:**
- **Given** I'm logged in as a user without "CreateBroker" permission
- **When** I try to access the Create Broker form at /brokers/new
- **Then** I see a 403 Forbidden error
- **And** the error message is: "You don't have permission to create brokers"
- **And** I'm not able to access the form

---

#### Audit & Timeline Requirements

**Timeline Event Creation:**
- **Given** I successfully create a broker
- **Then** an immutable timeline event is created with:
  - Event Type: "BrokerCreated"
  - Timestamp: Current UTC time in ISO 8601 format
  - User ID: My user ID
  - Entity ID: The new broker's ID
  - Details: Broker name
- **And** the timeline event is visible in the Broker 360 view

---

### Data Requirements

#### Required Fields
- **Name:** Broker's legal business name (Max 255 characters)
- **License Number:** State insurance license number (Max 50 characters)
- **State:** US state where broker is licensed (Dropdown from 50 states)

#### Optional Fields
- **Email:** Primary contact email address (Valid email format)
- **Phone:** Primary contact phone number (US phone format)
- **Website:** Broker's website URL (Valid URL format)
- **Notes:** Internal notes about broker (Text area, max 2000 characters)

#### System-Generated Fields
- **ID:** Auto-generated UUID
- **Status:** Defaults to "Active"
- **Created At:** Current timestamp
- **Created By:** Current user ID
- **Updated At:** Current timestamp
- **Updated By:** Current user ID

---

### Role-Based Visibility

**Who Can Create Brokers:**
- Distribution & Marketing Manager ✓
- Relationship Manager ✓
- Admin ✓

**Who Cannot:**
- Underwriter ✗ (read-only access)
- External broker users ✗ (not in Phase 0)

---

### Dependencies

**Depends On (Blockers):**
- Infrastructure: Broker table exists in database
- Infrastructure: Authentication and authorization are working
- Infrastructure: Timeline event logging is implemented

**Related Stories:**
- S2: View Broker List (user will see new broker here)
- S3: View Broker 360 (redirect destination after create)
- S4: Broker Timeline (timeline event will be displayed here)

---

### Out of Scope (Explicit Non-Goals)

**Not Included in This Story:**
- Contact management (deferred to S5: Manage Broker Contacts)
- Broker hierarchy assignment (deferred to S6: Broker Hierarchy)
- Bulk broker import (deferred to Phase 1)
- Document upload (deferred to Phase 1)
- Email notifications (deferred to Phase 1)
- Duplicate detection beyond license number (Future)

---

### UI/UX Notes

**Screen:** Create Broker Form

**Layout:**
- Form header: "Add New Broker"
- Form fields in single column layout
- Required fields marked with red asterisk (*)
- Save and Cancel buttons at bottom right
- Cancel button returns to Broker List

**Interactions:**
- Real-time inline validation (on blur or after first submit attempt)
- Save button disabled until required fields are filled
- Loading spinner on Save button while creating
- Success toast appears in top-right corner
- Redirect after 1 second delay to allow user to see success message

**Responsive:**
- Mobile: Full-width form with stacked fields
- Desktop: Centered form, max-width 600px

---

### Technical Constraints (PM Awareness)

- Must integrate with Keycloak for user authentication
- Must enforce ABAC authorization via Casbin
- Must log audit events to append-only ActivityTimelineEvent table
- Form validation must match backend API validation rules

---

### Questions & Assumptions

**Assumptions (Validated):**
- License number is unique per broker (confirmed with stakeholder)
- A broker can only be licensed in one state initially (multi-state in Phase 1)
- Status defaults to "Active" (no workflow needed for activation)

**Open Questions:**
- [ ] ~~Is there a maximum number of brokers we can create?~~ (Resolved: No limit for MVP)
- [ ] ~~Do we need duplicate name detection?~~ (Resolved: No, license number uniqueness is sufficient)

---

### Definition of Done

- [ ] All acceptance criteria are met
- [ ] Inline validation works for all field formats
- [ ] Duplicate license number check works
- [ ] Timeline event is created and visible in Broker 360
- [ ] Permission check works (403 for unauthorized users)
- [ ] Unit tests pass (form validation logic)
- [ ] Integration tests pass (create broker end-to-end)
- [ ] API tests pass (POST /brokers endpoint)
- [ ] Code review approved
- [ ] QA manual testing completed
- [ ] Deployed to staging environment
- [ ] Product Owner acceptance

---

## Story S2: Search Brokers by Name and License

**Epic:** E1 - Broker & MGA Relationship Management
**Priority:** High
**Phase:** MVP

---

### User Story

**As a** Distribution & Marketing Manager
**I want** to search brokers by name or license number
**So that** I can quickly find a specific broker without scrolling through the entire list

---

### Context & Background

With 100+ brokers in the system, the broker list becomes unwieldy. Users need a fast way to find brokers by name (most common) or license number (when referencing external documents).

This builds on S1 (View Broker List) by adding search functionality.

---

### Acceptance Criteria

#### Search by Name

**Partial Name Match:**
- **Given** the broker list contains:
  - "Acme Insurance Brokers"
  - "Beta Brokers Inc"
  - "Acme Specialty"
- **When** I type "acme" in the search box
- **Then** I see 2 results: "Acme Insurance Brokers" and "Acme Specialty"
- **And** "Beta Brokers Inc" is not shown

**Case-Insensitive Search:**
- **When** I type "ACME" or "acme" or "Acme"
- **Then** I see the same results (case-insensitive)

**Empty Results:**
- **Given** no brokers match the search term
- **When** I type "nonexistent"
- **Then** I see message: "No brokers found matching 'nonexistent'"
- **And** I see a "Clear Search" button

**Clear Search:**
- **Given** I have an active search filter
- **When** I click "Clear Search" or delete search text
- **Then** the full broker list is displayed again

---

#### Search by License Number

**Exact License Match:**
- **Given** a broker exists with License Number "CA-12345"
- **When** I type "CA-12345" in the search box
- **Then** I see 1 result: the broker with that license number

**Partial License Match:**
- **Given** brokers exist with licenses "CA-12345" and "CA-67890"
- **When** I type "CA-"
- **Then** I see 2 results (both California licenses)

---

#### Search Behavior

**Real-Time Search:**
- **When** I type in the search box
- **Then** results update after I stop typing for 300ms (debounced)
- **And** I see a loading indicator while searching

**Search Persistence:**
- **Given** I perform a search
- **When** I click on a broker to view Broker 360
- **And** I click Back to return to the list
- **Then** my search term is still present
- **And** the filtered results are still displayed

**Pagination with Search:**
- **Given** my search returns 50 results
- **Then** results are paginated (20 per page)
- **And** page count shows "Page 1 of 3"

---

### Data Requirements

**Search Fields:**
- Broker name (partial match)
- License number (partial match)

**Search Behavior:**
- Case-insensitive
- Searches both name and license number fields simultaneously
- Returns results matching either field

---

### Out of Scope

**Not Included:**
- Advanced filters (state, status) - deferred to S3: Advanced Filtering
- Search suggestions/autocomplete - deferred to Phase 1
- Search history - deferred to Phase 1
- Full-text search across all broker fields - deferred to Future

---

### Definition of Done

- [ ] Search works for name (partial, case-insensitive)
- [ ] Search works for license number (partial)
- [ ] Search is debounced (300ms delay)
- [ ] Empty state displays correctly
- [ ] Clear search works
- [ ] Search persists across navigation
- [ ] Integration tests pass
- [ ] Performance test: search returns results in < 500ms for 1000 brokers
- [ ] Code review approved
- [ ] Deployed

---

## Story S3: Submission Status Transition - Triage to Ready

**Epic:** E3 - Submission Intake Workflow
**Priority:** Critical
**Phase:** MVP

---

### User Story

**As a** Distribution & Marketing user
**I want** to move a submission from "Triaging" to "ReadyForUWReview" status
**So that** underwriters know which submissions are ready for their review

---

### Acceptance Criteria

#### Allowed Transition

**Successful Transition:**
- **Given** a submission exists in "Triaging" status
- **When** I click the "Ready for UW Review" button
- **Then** the submission status changes to "ReadyForUWReview"
- **And** a workflow transition event is logged with:
  - From Status: "Triaging"
  - To Status: "ReadyForUWReview"
  - Timestamp: Current UTC time
  - User ID: My user ID
  - Submission ID: The submission's ID
- **And** I see success message: "Submission moved to Ready for UW Review"
- **And** the submission appears in the Underwriter's queue

---

#### Blocked Transitions

**Invalid From-Status:**
- **Given** a submission is in "Bound" status
- **When** I try to move it to "ReadyForUWReview"
- **Then** I see a 409 Conflict error
- **And** the error message is: "Cannot transition from Bound to ReadyForUWReview"
- **And** the submission status remains "Bound"

**Missing Required Information:**
- **Given** a submission in "Triaging" status has incomplete data (missing insured name)
- **When** I try to move it to "ReadyForUWReview"
- **Then** I see a 400 Bad Request error
- **And** the error message lists missing fields: "Cannot proceed. Missing: Insured Name, Coverage Type"
- **And** the submission status remains "Triaging"

---

#### Permission Checks

**Authorized User:**
- Distribution users CAN transition from Triaging to ReadyForUWReview ✓

**Unauthorized User:**
- **Given** I'm logged in as an Underwriter
- **When** I try to transition a submission from Triaging to ReadyForUWReview
- **Then** I see a 403 Forbidden error
- **And** the error message is: "Only Distribution users can triage submissions"

---

### Out of Scope

**Not Included:**
- Bulk status transitions (Phase 1)
- Status transition comments (Phase 1)
- Email notifications on status change (Phase 1)
- Automated transitions based on rules (Future)

---

### Definition of Done

- [ ] Transition from Triaging to ReadyForUWReview works
- [ ] Workflow transition event is logged immutably
- [ ] Invalid transitions return 409 with descriptive error
- [ ] Missing data validations work
- [ ] Permission checks enforce role-based access
- [ ] Submission appears in underwriter queue after transition
- [ ] Integration tests cover all scenarios
- [ ] Code review approved
- [ ] Deployed

---

## Summary

These examples demonstrate:
- ✅ Clear persona and value statement
- ✅ Comprehensive acceptance criteria (happy path, errors, permissions)
- ✅ Given/When/Then format for scenarios
- ✅ Explicit audit trail requirements
- ✅ Clear data requirements
- ✅ Explicit out-of-scope items
- ✅ Testable definition of done

Use these as templates for writing your own stories!

---

## Version History

**Version 1.0** - 2026-01-26 - Initial story examples
