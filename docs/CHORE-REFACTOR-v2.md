# Comprehensive Refactoring Plan: Separate Generic Agents from Solution-Specific Content (v2.0)

**Execute this plan to cleanly separate reusable agent roles from Nebula CRM-specific content.**

This refactoring ensures `agents/` can be copied to any new project (underwriting workbench, claims system, etc.) while all Nebula-specific content lives in `planning-mds/`.

---

## Objective

Transform the repository from:
- ❌ Mixed: Generic + Nebula-specific content in `agents/`
- ✅ To: Clean separation with `agents/` = 100% generic, `planning-mds/` = 100% Nebula-specific

---

## Prerequisites

**Current working directory:** `C:\Users\gajap\sandbox\insurance-crm`

**Safety:** Create a branch for this refactoring:
```bash
git checkout -b refactor/separate-generic-from-solution
```

---

## PHASE 0: DISCOVERY & INVENTORY

Before moving any files, understand what exists and what needs to be separated.

### Step 0.1: Inventory All Agent Files

```bash
# List all files in each agent directory
find agents -type f -name "*.md" | sort

# Count files in each agent
echo "=== Product Manager ===" && ls -la agents/product-manager/references/ | wc -l
echo "=== Architect ===" && ls -la agents/architect/references/ | wc -l
echo "=== DevOps ===" && ls -la agents/devops/ | wc -l
echo "=== Templates ===" && ls -la agents/templates/ | wc -l
```

**Document findings in:** `docs/inventory-agents.md`

---

### Step 0.2: Scan for Nebula-Specific Terms

Create comprehensive search pattern:

```bash
# Create comprehensive Nebula-specific term list
cat > /tmp/nebula-terms.txt <<'EOF'
Broker
Submission
Sarah Chen
Marcus
Jennifer
Nebula
MGA
Account
Renewal
Binder
Commission
F1\b
F2\b
F3\b
F4\b
F5\b
F6\b
S1:
S2:
S3:
S4:
S5:
S6:
S7:
insurance CRM
broker hierarchy
submission workflow
renewal pipeline
distribution manager
underwriter persona
coordinator persona
Applied Epic
Vertafore
Duck Creek
EOF

# Search all agent files for Nebula-specific terms
echo "=== Searching agents/ for Nebula-specific content ===" > docs/nebula-content-scan.md
for term in $(cat /tmp/nebula-terms.txt); do
  echo "" >> docs/nebula-content-scan.md
  echo "## Term: $term" >> docs/nebula-content-scan.md
  grep -rn "$term" agents/ --include="*.md" >> docs/nebula-content-scan.md 2>/dev/null || echo "No matches" >> docs/nebula-content-scan.md
done

# Review the scan results
cat docs/nebula-content-scan.md
```

**Action:** Review `docs/nebula-content-scan.md` to identify all files with Nebula content.

---

### Step 0.3: Analyze Scripts for Solution-Specific Code

```bash
# List all scripts
find agents -type f -name "*.py" -o -name "*.sh" -o -name "*.js"

# Review each script for Nebula-specific content
cat agents/product-manager/scripts/generate-story-index.py
cat agents/product-manager/scripts/validate-stories.py
# (Check if scripts reference Nebula entities, paths, or business rules)
```

**Document findings:** Are scripts generic or do they hard-code Nebula assumptions?

---

### Step 0.4: Review Templates for Examples

```bash
# Check templates for embedded examples
cat agents/templates/story-template.md | grep -i "broker\|submission\|nebula\|sarah"
cat agents/templates/feature-template.md | grep -i "broker\|submission\|nebula\|sarah"
cat agents/templates/persona-template.md | grep -i "broker\|submission\|nebula\|sarah"
cat agents/templates/screen-spec-template.md | grep -i "broker\|submission\|nebula\|sarah"
cat agents/templates/adr-template.md | grep -i "broker\|submission\|nebula\|sarah"
```

**Action:** If templates contain Nebula examples, they must be replaced with generic placeholders.

---

### Step 0.5: Check Other Agents

```bash
# Check if DevOps agent exists and has solution-specific content
cat agents/devops/SKILL.md | grep -i "nebula\|broker\|submission"
cat agents/devops/README.md | grep -i "nebula\|broker\|submission" 2>/dev/null

# Check for any other agents
ls -la agents/
```

**Document:** Which agents exist and which have solution-specific content?

---

### Step 0.6: Review INCEPTION.md Structure

```bash
# Check current INCEPTION.md structure
grep "^##" planning-mds/INCEPTION.md | head -20
```

**Action:** Identify where to insert "2.0 Reference Documentation" section without breaking existing structure.

---

### Step 0.7: Create Inventory Summary

**File:** `docs/refactoring-inventory.md`

**Content:**
```markdown
# Refactoring Inventory Summary

**Date:** 2026-02-01

## Files to Move to planning-mds/

### Product Manager
- [ ] agents/product-manager/references/insurance-domain-glossary.md → planning-mds/domain/insurance-glossary.md
- [ ] agents/product-manager/references/crm-competitive-analysis.md → planning-mds/domain/crm-competitive-analysis.md
- [ ] agents/product-manager/references/persona-examples.md → planning-mds/examples/personas/nebula-personas.md
- [ ] agents/product-manager/references/feature-examples.md → planning-mds/examples/features/nebula-features.md
- [ ] agents/product-manager/references/story-examples.md → planning-mds/examples/stories/nebula-stories.md
- [ ] agents/product-manager/references/screen-spec-examples.md → planning-mds/examples/screens/nebula-screens.md

### Architect
- [ ] agents/architect/references/insurance-crm-architecture-patterns.md → planning-mds/domain/crm-architecture-patterns.md
- [ ] agents/architect/references/architecture-examples.md → planning-mds/examples/architecture/nebula-architecture.md

### Other (TBD based on scan)
- [ ] [Add files discovered in scan]

## Files to Create (Generic Replacements)

### Product Manager
- [ ] agents/product-manager/references/persona-examples.md (NEW - B2B SaaS, e-commerce, healthcare)
- [ ] agents/product-manager/references/feature-examples.md (NEW - task mgmt, e-commerce, scheduling)
- [ ] agents/product-manager/references/story-examples.md (NEW - generic stories)

### Architect
- [ ] agents/architect/references/architecture-examples.md (NEW - e-commerce, CMS, SaaS)

## Templates to Review
- [ ] agents/templates/story-template.md - Remove Nebula examples if any
- [ ] agents/templates/feature-template.md - Remove Nebula examples if any
- [ ] agents/templates/persona-template.md - Remove Nebula examples if any
- [ ] agents/templates/screen-spec-template.md - Remove Nebula examples if any
- [ ] agents/templates/adr-template.md - Remove Nebula examples if any

## Scripts to Review
- [ ] agents/product-manager/scripts/generate-story-index.py - Generic or solution-specific?
- [ ] agents/product-manager/scripts/validate-stories.py - Generic or solution-specific?

## Documentation to Create
- [ ] planning-mds/README.md
- [ ] planning-mds/domain/README.md
- [ ] planning-mds/examples/README.md
- [ ] planning-mds/examples/personas/README.md
- [ ] planning-mds/examples/features/README.md
- [ ] planning-mds/examples/stories/README.md
- [ ] planning-mds/examples/screens/README.md
- [ ] planning-mds/examples/architecture/README.md
- [ ] planning-mds/BOUNDARY-POLICY.md
- [ ] agents/README.md

## ADRs to Create
- [ ] planning-mds/examples/architecture/adrs/ADR-001-modular-monolith.md
- [ ] planning-mds/examples/architecture/adrs/ADR-002-ef-core.md
- [ ] planning-mds/examples/architecture/adrs/ADR-003-casbin-abac.md

## Files to Update
- [ ] agents/product-manager/SKILL.md - Add generic/solution resource separation
- [ ] agents/product-manager/README.md - Add generic/solution resource separation
- [ ] agents/architect/SKILL.md - Add generic/solution resource separation
- [ ] agents/architect/README.md - Add generic/solution resource separation
- [ ] agents/devops/SKILL.md (if needed)
- [ ] agents/AGENT-STATUS.md - Update status
- [ ] planning-mds/INCEPTION.md - Add reference documentation section
```

---

## PHASE 1: CREATE NEW DIRECTORY STRUCTURE

### Step 1.1: Create planning-mds/ Directories

```bash
# Create domain directory
mkdir -p planning-mds/domain

# Create examples directories
mkdir -p planning-mds/examples/personas
mkdir -p planning-mds/examples/features
mkdir -p planning-mds/examples/stories
mkdir -p planning-mds/examples/screens
mkdir -p planning-mds/examples/architecture/adrs

# Verify structure
tree planning-mds -L 3
```

**Expected output:**
```
planning-mds/
├── domain/
├── examples/
│   ├── personas/
│   ├── features/
│   ├── stories/
│   ├── screens/
│   └── architecture/
│       └── adrs/
```

---

## PHASE 2: PRESERVE NEBULA-SPECIFIC CONTENT

**Important:** Before moving files, read and verify Nebula content is preserved.

### Step 2.1: Move Product Manager Domain Files

```bash
# Move insurance domain glossary (if exists)
if [ -f "agents/product-manager/references/insurance-domain-glossary.md" ]; then
  mv agents/product-manager/references/insurance-domain-glossary.md planning-mds/domain/insurance-glossary.md
  echo "✅ Moved insurance-domain-glossary.md"
else
  echo "⚠️  File not found: insurance-domain-glossary.md"
fi

# Move CRM competitive analysis (if exists)
if [ -f "agents/product-manager/references/crm-competitive-analysis.md" ]; then
  mv agents/product-manager/references/crm-competitive-analysis.md planning-mds/domain/crm-competitive-analysis.md
  echo "✅ Moved crm-competitive-analysis.md"
else
  echo "⚠️  File not found: crm-competitive-analysis.md"
fi
```

**Rationale:** These files contain Nebula CRM-specific domain knowledge (brokers, MGAs, submissions, renewals). Not applicable to underwriting workbench or claims system.

---

### Step 2.2: Move Architect Domain Files

```bash
# Move insurance CRM architecture patterns (if exists)
if [ -f "agents/architect/references/insurance-crm-architecture-patterns.md" ]; then
  mv agents/architect/references/insurance-crm-architecture-patterns.md planning-mds/domain/crm-architecture-patterns.md
  echo "✅ Moved insurance-crm-architecture-patterns.md"
else
  echo "⚠️  File not found: insurance-crm-architecture-patterns.md"
fi
```

**Rationale:** This file contains Nebula CRM-specific architectural patterns (broker hierarchy, submission workflow, renewal pipeline). Not generic.

---

### Step 2.3: Move Product Manager Examples

**Important:** These files may contain Nebula examples. Read first, then move to preserve content.

```bash
# Read current persona-examples.md to verify Nebula content
echo "=== Current persona-examples.md content ===" > /tmp/persona-backup.md
cat agents/product-manager/references/persona-examples.md >> /tmp/persona-backup.md 2>/dev/null || echo "File not found"

# Move to planning-mds (if exists and has Nebula content)
if [ -f "agents/product-manager/references/persona-examples.md" ]; then
  # Check if it contains Nebula-specific content
  if grep -qi "broker\|sarah chen\|nebula" agents/product-manager/references/persona-examples.md; then
    mv agents/product-manager/references/persona-examples.md planning-mds/examples/personas/nebula-personas.md
    echo "✅ Moved persona-examples.md (contained Nebula content)"
  else
    echo "⚠️  persona-examples.md appears generic, skipping move"
  fi
else
  echo "⚠️  File not found: persona-examples.md"
fi

# Repeat for feature-examples.md
if [ -f "agents/product-manager/references/feature-examples.md" ]; then
  if grep -qi "broker\|submission\|nebula\|F1\|F2\|F3" agents/product-manager/references/feature-examples.md; then
    mv agents/product-manager/references/feature-examples.md planning-mds/examples/features/nebula-features.md
    echo "✅ Moved feature-examples.md (contained Nebula content)"
  else
    echo "⚠️  feature-examples.md appears generic, skipping move"
  fi
else
  echo "⚠️  File not found: feature-examples.md"
fi

# Repeat for story-examples.md
if [ -f "agents/product-manager/references/story-examples.md" ]; then
  if grep -qi "broker\|submission\|nebula\|S1:\|S2:" agents/product-manager/references/story-examples.md; then
    mv agents/product-manager/references/story-examples.md planning-mds/examples/stories/nebula-stories.md
    echo "✅ Moved story-examples.md (contained Nebula content)"
  else
    echo "⚠️  story-examples.md appears generic, skipping move"
  fi
else
  echo "⚠️  File not found: story-examples.md"
fi

# Repeat for screen-spec-examples.md
if [ -f "agents/product-manager/references/screen-spec-examples.md" ]; then
  if grep -qi "broker\|submission\|nebula" agents/product-manager/references/screen-spec-examples.md; then
    mv agents/product-manager/references/screen-spec-examples.md planning-mds/examples/screens/nebula-screens.md
    echo "✅ Moved screen-spec-examples.md (contained Nebula content)"
  else
    echo "⚠️  screen-spec-examples.md appears generic, skipping move"
  fi
else
  echo "⚠️  File not found: screen-spec-examples.md"
fi
```

**Rationale:** All examples reference Nebula entities (Broker, Submission, Account), Nebula personas (Sarah Chen, Marcus, Jennifer), and Nebula features (F1-F6). Not generic.

---

### Step 2.4: Move Architect Examples

```bash
# Move architecture-examples.md (if exists and has Nebula content)
if [ -f "agents/architect/references/architecture-examples.md" ]; then
  if grep -qi "broker\|submission\|account\|nebula" agents/architect/references/architecture-examples.md; then
    mv agents/architect/references/architecture-examples.md planning-mds/examples/architecture/nebula-architecture.md
    echo "✅ Moved architecture-examples.md (contained Nebula content)"
  else
    echo "⚠️  architecture-examples.md appears generic, skipping move"
  fi
else
  echo "⚠️  File not found: architecture-examples.md"
fi
```

**Rationale:** All examples reference Nebula entities (Broker, Submission, Account) and Nebula workflows. Not generic.

---

### Step 2.5: Verify No Nebula Content Lost

```bash
# List all moved files
echo "=== Files moved to planning-mds/ ===" > docs/moved-files.log
find planning-mds -type f -name "*.md" >> docs/moved-files.log

# Verify Nebula content is preserved
cat docs/moved-files.log
```

---

## PHASE 3: CREATE GENERIC EXAMPLES IN agents/

Now create truly generic examples to replace the moved Nebula-specific ones.

### Step 3.1: Create Generic Persona Examples

**File:** `agents/product-manager/references/persona-examples.md`

```bash
cat > agents/product-manager/references/persona-examples.md <<'EOF'
# Persona Examples

This document provides generic persona examples across different domains. Use these as templates when creating personas for your specific solution.

---

## Example 1: B2B SaaS - Sales Representative

**Name:** Alex Rivera
**Role:** Enterprise Sales Representative
**Company Type:** B2B SaaS Platform
**Experience:** 5 years in enterprise sales

**Demographics:**
- Age: 32
- Location: Remote (US-based)
- Team: 8-person sales team

**Goals:**
- Close 10 enterprise deals per quarter ($50K+ ACV)
- Reduce sales cycle from 90 to 60 days
- Improve lead qualification accuracy

**Pain Points:**
- Manually tracking leads across CRM, email, and LinkedIn
- No visibility into prospect engagement with marketing materials
- Difficulty identifying decision-makers in complex org structures
- Spending 40% of time on administrative tasks (data entry, status updates)

**Jobs-to-be-Done:**
- When prospecting new accounts, I want to quickly identify decision-makers so I can reach the right people faster
- When following up with leads, I want to see their engagement history so I can personalize outreach
- When preparing for demos, I want to understand prospect pain points so I can tailor my pitch

**Behavioral Patterns:**
- Checks CRM 5-10 times daily
- Prefers mobile access for on-the-go updates
- Uses LinkedIn Sales Navigator heavily
- Relies on email for prospect communication

---

## Example 2: E-commerce - Customer Support Agent

**Name:** Maria Santos
**Role:** Tier 1 Customer Support Agent
**Company Type:** E-commerce Fashion Retailer
**Experience:** 2 years in customer support

**Demographics:**
- Age: 26
- Location: Office-based, Manila
- Team: 50-person support team

**Goals:**
- Resolve 30 tickets per day with 95% CSAT
- Reduce average handling time to under 5 minutes
- Escalate less than 10% of tickets to Tier 2

**Pain Points:**
- No unified view of customer order history across channels
- Switching between 5 different tools to find information
- Difficulty finding answers in outdated knowledge base
- Customers angry about having to repeat information

**Jobs-to-be-Done:**
- When a customer contacts us, I want to see their complete history so I don't ask them to repeat information
- When troubleshooting an order issue, I want to see real-time shipping status so I can give accurate updates
- When I don't know the answer, I want to quickly find relevant KB articles so I can resolve tickets faster

**Behavioral Patterns:**
- Handles 30-40 tickets daily via chat, email, phone
- Uses keyboard shortcuts extensively
- Keeps 3 monitors open simultaneously
- References knowledge base for 60% of inquiries

---

## Example 3: Healthcare - Clinical Nurse

**Name:** David Chen
**Role:** Registered Nurse (RN)
**Department:** Emergency Department
**Experience:** 8 years in emergency medicine

**Demographics:**
- Age: 35
- Location: Urban hospital, 400 beds
- Shift: 12-hour shifts (7am-7pm or 7pm-7am)

**Goals:**
- Provide safe, high-quality patient care
- Document accurately without sacrificing patient time
- Coordinate effectively with physicians and specialists
- Maintain situational awareness of all assigned patients

**Pain Points:**
- Clunky EMR requires 20+ clicks for common tasks
- Medication administration workflow interrupts patient care
- Difficult to track which tasks are pending vs completed
- Alerts and notifications are overwhelming (90% false positives)

**Jobs-to-be-Done:**
- When admitting a patient, I want to quickly review allergies and contraindications so I can administer medications safely
- When a physician orders a new medication, I want to see administration instructions immediately so I don't delay treatment
- When handing off to the next shift, I want to see a summary of pending tasks so nothing falls through the cracks

**Behavioral Patterns:**
- Spends 40% of shift on documentation
- Uses mobile workstations (COWs - Computers on Wheels)
- Frequently interrupted (every 6 minutes on average)
- Relies on verbal handoffs and written notes

---

## How to Use These Examples

1. **Select a domain** relevant to your solution (or create your own)
2. **Adapt demographics** to match your target users
3. **Customize goals and pain points** based on your research
4. **Map to your features** - ensure each persona's JTBD aligns with your feature set
5. **Keep it real** - base personas on actual user research, not assumptions

---

## Version History

**Version 2.0** - 2026-02-01 - Generic examples (separated from solution-specific)
**Version 1.0** - Initial version
EOF

echo "✅ Created generic persona-examples.md"
```

---

### Step 3.2: Create Generic Feature Examples

**File:** `agents/product-manager/references/feature-examples.md`

```bash
cat > agents/product-manager/references/feature-examples.md <<'EOF'
# Feature Examples

This document provides generic feature examples across different domains. Use these as templates when defining features for your specific solution.

---

## Example 1: Task Management System

**Feature ID:** F1
**Feature Name:** Task Organization & Prioritization
**Domain:** Productivity SaaS

**Feature Statement:**
As a project manager, I want to organize and prioritize tasks across multiple projects so that my team focuses on high-impact work.

**Business Objective:**
- **Goal:** Increase team productivity by 20%
- **Metric:** Task completion rate, time-to-completion
- **Alignment:** Company OKR - Improve operational efficiency

**Problem Statement:**
- **Current State:** Teams manually track tasks in spreadsheets, email, and Slack, leading to missed deadlines and duplicated effort
- **Desired State:** Centralized task management with automatic prioritization and team visibility
- **Impact:** 15 hours/week wasted on task coordination per project manager

**Scope & Boundaries:**
- **In Scope:** Task CRUD, assignment, priority levels, due dates, filtering, search
- **Out of Scope:** Time tracking, billing, resource allocation (deferred to Phase 2)

**User Stories:**
- S1: Create task with title, description, assignee, due date
- S2: Update task status (To Do, In Progress, Done, Blocked)
- S3: Assign priority level (Critical, High, Medium, Low)
- S4: Filter tasks by assignee, status, priority, due date
- S5: Search tasks by title or description

---

## Example 2: E-commerce Order Fulfillment

**Feature ID:** F2
**Feature Name:** Order Processing & Shipping
**Domain:** E-commerce Platform

**Feature Statement:**
As a warehouse manager, I want to process orders efficiently and generate shipping labels so that customers receive orders within SLA.

**Business Objective:**
- **Goal:** Reduce order fulfillment time from 48 hours to 24 hours
- **Metric:** Average fulfillment time, on-time shipping rate
- **Alignment:** Company goal - Best-in-class customer experience

**Problem Statement:**
- **Current State:** Manual order processing, printing packing slips, handwriting shipping labels
- **Desired State:** Automated order queue, one-click label generation, carrier integration
- **Impact:** 100+ orders/day delayed due to manual processes

**Scope & Boundaries:**
- **In Scope:** Order queue, pick list generation, shipping label printing, carrier API integration
- **Out of Scope:** Inventory management, returns processing (separate features)

**User Stories:**
- S1: View order queue sorted by order date
- S2: Generate pick list for batch fulfillment
- S3: Mark order as picked
- S4: Generate shipping label via carrier API (USPS, UPS, FedEx)
- S5: Mark order as shipped with tracking number

---

## Example 3: Patient Appointment Scheduling

**Feature ID:** F3
**Feature Name:** Appointment Scheduling & Reminders
**Domain:** Healthcare SaaS

**Feature Statement:**
As a medical office administrator, I want to schedule patient appointments and send automated reminders so that we reduce no-shows and maximize provider utilization.

**Business Objective:**
- **Goal:** Reduce no-show rate from 15% to 5%
- **Metric:** No-show rate, provider schedule utilization
- **Alignment:** Practice revenue optimization

**Problem Statement:**
- **Current State:** Phone-based scheduling, paper calendars, manual reminder calls
- **Desired State:** Online scheduling, automated SMS/email reminders, waitlist management
- **Impact:** 15% no-show rate = $200K annual revenue loss

**Scope & Boundaries:**
- **In Scope:** Appointment booking, calendar integration, automated reminders (SMS/email), waitlist
- **Out of Scope:** Billing, insurance verification, telemedicine (separate features)

**User Stories:**
- S1: Book appointment for patient (select provider, date/time, reason)
- S2: Send automated reminder 24 hours before appointment
- S3: Allow patient to confirm/cancel via SMS link
- S4: Add patient to waitlist if no slots available
- S5: Auto-fill waitlist slot when cancellation occurs

---

## How to Use These Examples

1. **Select a domain** close to your solution or use as inspiration
2. **Follow the structure** (Feature Statement, Business Objective, Problem Statement, Scope, Stories)
3. **Customize metrics** based on your business goals
4. **Keep features focused** - one feature should deliver one cohesive capability
5. **Link to stories** - ensure 5-10 user stories per feature

---

## Version History

**Version 2.0** - 2026-02-01 - Generic examples (separated from solution-specific)
**Version 1.0** - Initial version
EOF

echo "✅ Created generic feature-examples.md"
```

---

### Step 3.3: Create Generic Story Examples

**File:** `agents/product-manager/references/story-examples.md`

```bash
cat > agents/product-manager/references/story-examples.md <<'EOF'
# Story Examples

This document provides generic user story examples across different domains. Use these as templates when writing stories for your specific solution.

---

## Example 1: Task Management - Create Task

**Story ID:** S1
**Feature:** F1 - Task Organization
**Domain:** Productivity SaaS

**User Story:**
As a project manager,
I want to create a task with title, description, assignee, and due date,
So that my team knows what to work on and when it's due.

**Context & Background:**
Project managers currently track tasks in spreadsheets, leading to version control issues and lack of visibility. This story enables centralized task creation with all essential metadata.

**Acceptance Criteria:**

**Given** I am on the Tasks page
**When** I click "Create Task"
**Then** I see a form with fields: Title (required), Description (optional), Assignee (dropdown), Due Date (date picker), Priority (dropdown)

**Given** I fill in Title = "Design landing page" and Assignee = "John Doe"
**When** I click "Save"
**Then** The task is created and appears in the task list
**And** John Doe receives an email notification

**Given** I try to save a task without a Title
**When** I click "Save"
**Then** I see an error message "Title is required"
**And** The task is not created

**Checklist:**
- [ ] Title is required (max 255 characters)
- [ ] Description is optional (max 2000 characters)
- [ ] Assignee dropdown shows all active team members
- [ ] Due date defaults to 7 days from now
- [ ] Priority defaults to "Medium"
- [ ] Task appears in assignee's task list immediately
- [ ] Email notification sent to assignee

**Edge Cases & Error Scenarios:**
- User tries to assign task to inactive team member → Show error "Cannot assign to inactive user"
- User selects due date in the past → Show warning "Due date is in the past, are you sure?"
- Network error during save → Show error "Failed to create task, please retry"

**Audit & Timeline Requirements:**
- Log TaskCreated event with timestamp, created by user, task details
- Capture in activity timeline: "Sarah created task 'Design landing page'"

**Data Requirements:**
- Task entity: Id (GUID), Title (string), Description (string), AssigneeId (GUID), DueDate (DateTime), Priority (enum), Status (enum), CreatedAt, CreatedBy, UpdatedAt, UpdatedBy

**Dependencies:**
- Requires User entity (for assignee dropdown)
- Requires Email service (for notifications)

**Out of Scope:**
- Recurring tasks (deferred to Phase 2)
- Task templates (deferred to Phase 2)
- Bulk task creation (deferred to Phase 2)

**Definition of Done:**
- [ ] Code complete and peer reviewed
- [ ] Unit tests written (>80% coverage)
- [ ] Integration tests written
- [ ] Manual QA completed
- [ ] Accessible (WCAG 2.1 AA compliant)
- [ ] Documentation updated
- [ ] Deployed to staging and verified

---

## Example 2: E-commerce - Add to Cart

**Story ID:** S2
**Feature:** F2 - Shopping Cart
**Domain:** E-commerce

**User Story:**
As a customer,
I want to add a product to my cart,
So that I can purchase it later.

**Context & Background:**
Customers browse products and want to save items for purchase. Cart persists across sessions so customers can return later.

**Acceptance Criteria:**

**Given** I am viewing a product detail page
**When** I click "Add to Cart"
**Then** The product is added to my cart
**And** I see a confirmation message "Product added to cart"
**And** The cart icon badge updates to show cart count

**Given** I add a product that's already in my cart
**When** I click "Add to Cart"
**Then** The quantity increments by 1
**And** I see "Cart updated: 2x Product Name"

**Given** I add a product that's out of stock
**When** I click "Add to Cart"
**Then** I see an error "Product is out of stock"
**And** The product is not added to cart

**Checklist:**
- [ ] Product added with quantity = 1 (if new)
- [ ] Quantity incremented (if already in cart)
- [ ] Cart persists across browser sessions (cookie or local storage)
- [ ] Cart syncs to server for logged-in users
- [ ] Out of stock products cannot be added
- [ ] Cart icon badge shows total item count
- [ ] Confirmation message displays for 3 seconds

**Edge Cases & Error Scenarios:**
- Product becomes out of stock between view and add → Show error, refresh inventory
- User's cart reaches max items (100) → Show error "Cart is full"
- Network error during add → Retry 3 times, then show error

**Audit & Timeline Requirements:**
- Log ProductAddedToCart event (productId, quantity, userId, timestamp)
- Used for abandoned cart analysis

**Data Requirements:**
- Cart entity: UserId (GUID or session ID), ProductId (GUID), Quantity (int), AddedAt (DateTime)
- Product entity: Id, Name, Price, StockQuantity

**Dependencies:**
- Product catalog (must exist)
- Session management (for anonymous users)

**Out of Scope:**
- Cart recommendations (deferred to Phase 2)
- Save for later (deferred to Phase 2)

**Definition of Done:**
- [ ] Code complete and peer reviewed
- [ ] Unit tests written
- [ ] E2E tests written (Playwright)
- [ ] Performance tested (add to cart < 200ms)
- [ ] Analytics tracking verified
- [ ] Deployed to production

---

## How to Use These Examples

1. **Follow the template structure** exactly (User Story, Context, Acceptance Criteria, etc.)
2. **Use Given/When/Then** format for acceptance criteria (Gherkin syntax)
3. **Include checklists** for implementation details
4. **Document edge cases** explicitly (don't leave to developer imagination)
5. **Define DoD** - what "done" means for your team

---

## Version History

**Version 2.0** - 2026-02-01 - Generic examples (separated from solution-specific)
**Version 1.0** - Initial version
EOF

echo "✅ Created generic story-examples.md"
```

---

### Step 3.4: Create Generic Architecture Examples

**File:** `agents/architect/references/architecture-examples.md`

```bash
cat > agents/architect/references/architecture-examples.md <<'EOF'
# Architecture Examples

Real-world architecture examples across different domains. Use these as templates when designing your specific solution.

---

## Example 1: E-commerce Order Management System

### Order Entity Specification

**Table Name:** `Orders`

**Description:** Represents a customer order with line items.

#### Fields

| Field | Type | Constraints | Default | Description |
|-------|------|-------------|---------|-------------|
| Id | Guid | PK, NOT NULL | NewGuid() | Unique identifier |
| OrderNumber | string(50) | NOT NULL, UNIQUE | - | Human-readable order number (ORD-2026-001234) |
| CustomerId | Guid | FK → Customers, NOT NULL | - | Customer who placed order |
| Status | string(20) | NOT NULL | 'Pending' | Pending, Processing, Shipped, Delivered, Cancelled |
| OrderDate | DateTime | NOT NULL | UtcNow | When order was placed |
| TotalAmount | decimal(18,2) | NOT NULL | - | Order total (sum of line items) |
| ShippingAddress | string(500) | NOT NULL | - | Delivery address |
| PaymentMethod | string(50) | NOT NULL | - | CreditCard, PayPal, BankTransfer |
| PaymentStatus | string(20) | NOT NULL | 'Pending' | Pending, Paid, Refunded |
| CreatedAt | DateTime | NOT NULL | UtcNow | UTC timestamp |
| CreatedBy | Guid | NOT NULL | - | System or user who created |
| UpdatedAt | DateTime | NOT NULL | UtcNow | UTC timestamp |
| UpdatedBy | Guid | NOT NULL | - | User who last updated |

#### Relationships

- **One-to-Many:** Order → OrderItems (cascade delete)
- **Many-to-One:** Order → Customer (restrict delete if orders exist)

#### Indexes

- `IX_Orders_OrderNumber` (UNIQUE)
- `IX_Orders_CustomerId`
- `IX_Orders_Status`
- `IX_Orders_OrderDate`

#### Audit Requirements

- All mutations create `ActivityTimelineEvent`
- Events: OrderCreated, OrderStatusChanged, OrderCancelled
- Timeline includes before/after state for updates

### Order Workflow State Machine

**States:**
- Pending (initial) → Processing → Shipped → Delivered (terminal)
- Pending → Cancelled (terminal)

**Allowed Transitions:**

| From | To | Prerequisites | Authorization |
|------|----|---------------|---------------|
| Pending | Processing | Payment confirmed | System, Admin |
| Processing | Shipped | Items picked, label generated | WarehouseStaff, Admin |
| Shipped | Delivered | Carrier confirms delivery | System |
| Pending | Cancelled | No payment or customer request | Customer, Admin |
| Processing | Cancelled | Customer request before shipment | Admin only |

**Validation Rules:**

**Processing → Shipped:**
- All order items must be picked
- Shipping label must be generated
- Carrier tracking number must exist

**Error Responses:**

```json
{
  "code": "INVALID_TRANSITION",
  "message": "Cannot ship order. Items not yet picked.",
  "details": {
    "currentStatus": "Processing",
    "attemptedStatus": "Shipped",
    "missingRequirements": ["Items not picked"]
  }
}
```

### Order API Contract

```yaml
openapi: 3.0.0
paths:
  /api/orders:
    post:
      summary: Create a new order
      operationId: createOrder
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              required: [customerId, items, shippingAddress]
              properties:
                customerId:
                  type: string
                  format: uuid
                items:
                  type: array
                  items:
                    type: object
                    properties:
                      productId:
                        type: string
                        format: uuid
                      quantity:
                        type: integer
                        minimum: 1
                      price:
                        type: number
                        format: decimal
                shippingAddress:
                  type: string
                paymentMethod:
                  type: string
                  enum: [CreditCard, PayPal, BankTransfer]
      responses:
        '201':
          description: Order created successfully
        '400':
          description: Validation error
        '402':
          description: Payment required
        '409':
          description: Inventory conflict (out of stock)

  /api/orders/{id}/status:
    put:
      summary: Update order status
      parameters:
        - name: id
          in: path
          required: true
          schema:
            type: string
            format: uuid
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              required: [status]
              properties:
                status:
                  type: string
                  enum: [Processing, Shipped, Delivered, Cancelled]
                trackingNumber:
                  type: string
                  description: Required when transitioning to Shipped
      responses:
        '200':
          description: Status updated successfully
        '409':
          description: Invalid status transition
```

### Authorization Policies

**Casbin Policies:**

```
# Customers can create orders and view their own orders
p, Customer, Order, Create, allow
p, Customer, Order, Read, allow, sub.userId == res.customerId

# WarehouseStaff can update order status (Processing → Shipped)
p, WarehouseStaff, Order, Update, allow, res.status in ["Processing", "Shipped"]

# Admins can do everything
p, Admin, Order, *, allow
```

---

## Example 2: Content Management System - Article Workflow

### Article Entity Specification

**Table Name:** `Articles`

**Fields:**

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK | Unique identifier |
| Title | string(255) | NOT NULL | Article title |
| Slug | string(255) | UNIQUE, NOT NULL | URL-friendly slug |
| Content | text | NULL | Article body (markdown) |
| AuthorId | Guid | FK → Users | Author |
| Status | string(20) | NOT NULL | Draft, InReview, Published, Archived |
| PublishedAt | DateTime | NULL | When published |
| CreatedAt | DateTime | NOT NULL | Creation timestamp |
| UpdatedAt | DateTime | NOT NULL | Last update timestamp |

### Article Workflow

**States:**
- Draft (initial) → InReview → Published (terminal)
- Draft → Archived (terminal)

**Allowed Transitions:**

| From | To | Prerequisites |
|------|----|---------------|
| Draft | InReview | Title and content populated |
| InReview | Draft | Editor requests changes |
| InReview | Published | Editor approval |
| Published | Archived | Author or editor archives |

---

## Example 3: SaaS Subscription Management

### Subscription Entity

**Table Name:** `Subscriptions`

**Fields:**

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK | Subscription ID |
| CustomerId | Guid | FK → Customers | Customer |
| PlanId | Guid | FK → Plans | Subscription plan (Starter, Pro, Enterprise) |
| Status | string(20) | NOT NULL | Active, Suspended, Cancelled, Expired |
| BillingCycle | string(20) | NOT NULL | Monthly, Yearly |
| StartDate | DateTime | NOT NULL | Subscription start |
| EndDate | DateTime | NULL | Subscription end (null = ongoing) |
| NextBillingDate | DateTime | NOT NULL | Next charge date |
| MRR | decimal(18,2) | NOT NULL | Monthly Recurring Revenue |

### Subscription Workflow

**States:**
- Active → Suspended (payment failed) → Active (payment recovered)
- Active → Cancelled → Expired (terminal)

**Business Rules:**
- Cannot cancel if in trial period (trial cancels automatically)
- Cannot downgrade mid-cycle (takes effect next billing date)
- Suspend after 3 failed payment attempts
- Expire 30 days after cancellation (grace period)

---

## How to Use These Examples

1. Select a domain relevant to your solution (or create your own)
2. Follow the structure (Entity → Workflow → API → Authorization)
3. Adapt field names and relationships to your domain
4. Keep patterns consistent across all entities in your system
5. Reference these examples when designing new modules

---

## Version History

**Version 2.0** - 2026-02-01 - Generic examples (separated from solution-specific)
**Version 1.0** - Initial version
EOF

echo "✅ Created generic architecture-examples.md"
```

---

### Step 3.5: Verify Generic Content

```bash
# Verify NO Nebula-specific terms in new generic files
echo "=== Checking generic files for Nebula-specific content ===" > docs/generic-verification.log

for file in \
  agents/product-manager/references/persona-examples.md \
  agents/product-manager/references/feature-examples.md \
  agents/product-manager/references/story-examples.md \
  agents/architect/references/architecture-examples.md
do
  echo "" >> docs/generic-verification.log
  echo "## Checking: $file" >> docs/generic-verification.log
  grep -in "broker\|submission\|nebula\|sarah chen\|marcus\|jennifer\|MGA\|F1\|F2\|F3\|F4\|F5\|F6\|S1:\|S2:\|insurance CRM" "$file" >> docs/generic-verification.log 2>&1 || echo "✅ No Nebula content found" >> docs/generic-verification.log
done

cat docs/generic-verification.log
```

**Expected:** All files should show "✅ No Nebula content found"

---

## PHASE 4: REVIEW AND CLEAN TEMPLATES

Templates should contain only generic placeholders, not solution-specific examples.

### Step 4.1: Review Story Template

```bash
# Check for Nebula-specific examples in story template
cat agents/templates/story-template.md | grep -i "broker\|submission\|nebula\|sarah\|insurance"
```

**If found:** Replace with generic placeholders (e.g., `[Entity Name]`, `[User Name]`, `[System Name]`)

---

### Step 4.2: Review Feature Template

```bash
# Check for Nebula-specific examples in feature template
cat agents/templates/feature-template.md | grep -i "broker\|submission\|nebula\|sarah\|insurance"
```

**If found:** Replace with generic placeholders

---

### Step 4.3: Review Persona Template

```bash
# Check for Nebula-specific examples in persona template
cat agents/templates/persona-template.md | grep -i "broker\|submission\|nebula\|sarah\|insurance"
```

**If found:** Replace with generic placeholders

---

### Step 4.4: Review Screen Spec Template

```bash
# Check for Nebula-specific examples in screen spec template
cat agents/templates/screen-spec-template.md | grep -i "broker\|submission\|nebula\|sarah\|insurance"
```

**If found:** Replace with generic placeholders

---

### Step 4.5: Review ADR Template

```bash
# Check for Nebula-specific examples in ADR template
cat agents/templates/adr-template.md | grep -i "broker\|submission\|nebula\|sarah\|insurance"
```

**If found:** Replace with generic placeholders

---

## PHASE 5: UPDATE AGENT DOCUMENTATION

Update SKILL.md and README.md files to separate generic vs solution-specific resources.

### Step 5.1: Update Product Manager SKILL.md

**File:** `agents/product-manager/SKILL.md`

Find the "Required Resources" section and replace with:

```markdown
**Required Resources:**

**Generic Resources (reusable across projects):**
- `agents/templates/story-template.md` - Story specification format
- `agents/templates/feature-template.md` - Feature specification format
- `agents/templates/persona-template.md` - Persona specification format
- `agents/templates/screen-spec-template.md` - Screen specification format
- `agents/product-manager/references/pm-best-practices.md` - INVEST criteria, acceptance criteria patterns
- `agents/product-manager/references/vertical-slicing-guide.md` - Feature decomposition strategies
- `agents/product-manager/references/persona-examples.md` - Generic persona examples (B2B SaaS, e-commerce, healthcare)
- `agents/product-manager/references/feature-examples.md` - Generic feature examples (task mgmt, e-commerce, scheduling)
- `agents/product-manager/references/story-examples.md` - Generic story examples with full acceptance criteria

**Solution-Specific Resources (for current project):**
- `planning-mds/INCEPTION.md` - Master specification for this project
- `planning-mds/domain/` - Domain knowledge (terminology, competitive analysis, architecture patterns)
- `planning-mds/examples/personas/` - Project-specific persona examples
- `planning-mds/examples/features/` - Project-specific feature examples
- `planning-mds/examples/stories/` - Project-specific story examples
- `planning-mds/examples/screens/` - Project-specific screen specifications

**Note:** When starting a new project, copy `agents/` directory wholesale and create new `planning-mds/` with project-specific content.
```

---

### Step 5.2: Update Product Manager README.md

**File:** `agents/product-manager/README.md`

Find the "Key Resources" section and replace with:

```markdown
### Key Resources

**Generic Resources (in agents/):**

| Resource | Purpose |
|----------|---------|
| `SKILL.md` | Complete PM agent specification |
| `references/pm-best-practices.md` | INVEST criteria, acceptance criteria patterns |
| `references/vertical-slicing-guide.md` | Feature decomposition strategies |
| `references/persona-examples.md` | Generic persona templates (B2B, e-commerce, healthcare) |
| `references/feature-examples.md` | Generic feature templates |
| `references/story-examples.md` | Generic story templates with full AC |
| `../templates/story-template.md` | User story template structure |
| `../templates/feature-template.md` | Feature template structure |

**Solution-Specific Resources (in planning-mds/):**

| Resource | Purpose |
|----------|---------|
| `../../planning-mds/INCEPTION.md` | Project master specification |
| `../../planning-mds/domain/` | Domain glossary, competitive analysis, architecture patterns |
| `../../planning-mds/examples/personas/` | Project-specific personas |
| `../../planning-mds/examples/features/` | Project-specific features |
| `../../planning-mds/examples/stories/` | Project-specific stories |
| `../../planning-mds/examples/screens/` | Project-specific screen specs |
```

---

### Step 5.3: Update Architect SKILL.md

**File:** `agents/architect/SKILL.md`

Find the "Required Resources" section and replace with:

```markdown
**Required Resources:**

**Generic Resources (reusable across projects):**
- `agents/templates/api-contract-template.yaml` - OpenAPI template structure
- `agents/templates/entity-model-template.md` - Entity specification format
- `agents/templates/adr-template.md` - Architecture Decision Record format
- `agents/architect/references/architecture-best-practices.md` - SOLID, DDD, Clean Architecture principles
- `agents/architect/references/architecture-examples.md` - Generic architecture examples (e-commerce, CMS, SaaS)
- `agents/architect/references/api-design-guide.md` - REST API design patterns (.NET 10 Minimal APIs)
- `agents/architect/references/data-modeling-guide.md` - EF Core 10 & PostgreSQL patterns
- `agents/architect/references/authorization-patterns.md` - ABAC with Casbin, Keycloak integration
- `agents/architect/references/service-architecture-patterns.md` - Modular monolith, Clean Architecture, DDD
- `agents/architect/references/security-architecture-guide.md` - Authentication, encryption, OWASP Top 10
- `agents/architect/references/performance-design-guide.md` - Database optimization, caching, monitoring
- `agents/architect/references/workflow-design.md` - State machines, Temporal workflows

**Solution-Specific Resources (for current project):**
- `planning-mds/INCEPTION.md` - Project master specification
- `planning-mds/domain/` - Domain-specific architecture patterns
- `planning-mds/examples/architecture/` - Project-specific architecture examples
- `planning-mds/examples/architecture/adrs/` - Project Architecture Decision Records

**Note:** When starting a new project, copy `agents/` directory wholesale and create new `planning-mds/` with project-specific content.
```

---

### Step 5.4: Update Architect README.md

**File:** `agents/architect/README.md`

Find the "Key Resources" section and replace with:

```markdown
### Key Resources

**Generic Resources (in agents/):**

| Resource | Purpose |
|----------|---------|
| `SKILL.md` | Complete architect agent specification |
| `references/architecture-best-practices.md` | SOLID, DDD, Clean Architecture principles |
| `references/architecture-examples.md` | Generic examples (e-commerce order mgmt, CMS, SaaS subscriptions) |
| `references/api-design-guide.md` | REST API design with .NET 10 Minimal APIs |
| `references/data-modeling-guide.md` | EF Core 10, PostgreSQL, Clean Architecture data layer |
| `references/authorization-patterns.md` | ABAC with Casbin, Keycloak OIDC/JWT integration |
| `references/service-architecture-patterns.md` | Modular monolith, DDD aggregates, integration patterns |
| `references/security-architecture-guide.md` | Authentication, authorization, data protection, OWASP |
| `references/performance-design-guide.md` | Database optimization, caching, load testing |
| `references/workflow-design.md` | State machines, Temporal workflows, event sourcing |
| `../templates/adr-template.md` | ADR template structure |
| `../templates/api-contract-template.yaml` | OpenAPI template |

**Solution-Specific Resources (in planning-mds/):**

| Resource | Purpose |
|----------|---------|
| `../../planning-mds/INCEPTION.md` | Project master specification |
| `../../planning-mds/domain/` | Domain-specific architecture patterns |
| `../../planning-mds/examples/architecture/` | Project-specific architecture examples and ADRs |
```

---

### Step 5.5: Check DevOps Agent (If Exists)

```bash
# Check if DevOps SKILL.md references Nebula-specific content
if [ -f "agents/devops/SKILL.md" ]; then
  echo "=== Checking DevOps SKILL.md for Nebula content ===" > docs/devops-check.log
  grep -in "nebula\|broker\|submission\|insurance" agents/devops/SKILL.md >> docs/devops-check.log || echo "✅ No Nebula content" >> docs/devops-check.log
  cat docs/devops-check.log

  # If Nebula content found, update DevOps SKILL.md similarly
fi
```

---

## PHASE 6: CREATE PLANNING-MDS DOCUMENTATION

Create comprehensive documentation for planning-mds/ directory.

### Step 6.1: Create planning-mds/BOUNDARY-POLICY.md

**File:** `planning-mds/BOUNDARY-POLICY.md`

```bash
cat > planning-mds/BOUNDARY-POLICY.md <<'EOF'
# Boundary Policy: Generic Agents vs Solution-Specific Content

**Date:** 2026-02-01
**Status:** Active
**Owner:** Architecture Team

---

## Purpose

This policy defines the boundary between generic, reusable agent roles (`agents/`) and solution-specific content (`planning-mds/`).

---

## Policy Rules

### Rule 1: agents/ is Generic and Reusable

**Principle:** Everything in `agents/` must be applicable to **any** software project, regardless of domain.

**What belongs in agents/:**
- ✅ Agent role definitions (SKILL.md, README.md)
- ✅ Generic best practices (SOLID, DDD, INVEST, vertical slicing, etc.)
- ✅ Generic examples from multiple domains (B2B SaaS, e-commerce, healthcare, etc.)
- ✅ Generic templates (story, feature, persona, ADR, API contract, etc.)
- ✅ Generic scripts and tools (validation, linting, formatting)

**What does NOT belong in agents/:**
- ❌ Domain-specific terminology (insurance, underwriting, claims, etc.)
- ❌ Competitive analysis for specific markets
- ❌ Solution-specific examples (personas, features, stories referencing project entities)
- ❌ Solution-specific architecture patterns
- ❌ Project-specific business rules or workflows

---

### Rule 2: planning-mds/ is Solution-Specific

**Principle:** Everything in `planning-mds/` is specific to the current project and would be replaced for a new project.

**What belongs in planning-mds/:**
- ✅ Project master specification (INCEPTION.md)
- ✅ Domain knowledge (glossary, competitive analysis, domain-specific patterns)
- ✅ Project-specific examples (personas, features, stories, architecture)
- ✅ Actual project requirements (features/, stories/, architecture/)
- ✅ Project-specific ADRs and design decisions

**What does NOT belong in planning-mds/:**
- ❌ Generic best practices (those go in agents/)
- ❌ Generic examples from other domains
- ❌ Reusable templates (those go in agents/templates/)

---

### Rule 3: Agents Must Not Invent Requirements

**Principle:** Agent roles consume requirements from `planning-mds/`; they do not create or embed solution requirements.

**Implementation:**
- Agents read from `planning-mds/INCEPTION.md` and `planning-mds/domain/` to understand project context
- Agents reference `planning-mds/examples/` to see how generic patterns apply to this project
- Agents generate deliverables based on templates in `agents/templates/` and requirements in `planning-mds/`
- Agents never hard-code project-specific business logic in their role definitions

---

### Rule 4: Starting a New Project

**Principle:** Reusing agents for a new project should be as simple as copying `agents/` and creating new `planning-mds/`.

**Process:**
1. Copy entire `agents/` directory to new project
2. Create new `planning-mds/` directory structure
3. Write new domain knowledge in `planning-mds/domain/`
4. Create new project-specific examples in `planning-mds/examples/`
5. Write new INCEPTION.md for the new project
6. Agents are immediately ready to use with new project context

---

## Enforcement

### Pre-Commit Checks

Run validation before committing changes to `agents/`:

```bash
# Check for Nebula-specific (or other project-specific) terms
./scripts/validate-generic-boundary.sh
```

### Code Review Checklist

When reviewing PRs that modify `agents/`:
- [ ] No project-specific terminology in agent files
- [ ] Examples are generic and span multiple domains
- [ ] No hard-coded business rules or domain logic
- [ ] All project-specific content belongs in `planning-mds/`

When reviewing PRs that modify `planning-mds/`:
- [ ] Content is specific to current project
- [ ] No generic best practices (those belong in `agents/`)
- [ ] References to `agents/` resources are correct

---

## Examples

### ✅ GOOD: Generic Example

**File:** `agents/product-manager/references/persona-examples.md`

```markdown
## Example: B2B SaaS - Sales Representative

**Name:** Alex Rivera
**Role:** Enterprise Sales Representative
**Goals:** Close enterprise deals, reduce sales cycle, improve lead qualification
```

**Why good:** Uses a generic domain (B2B SaaS), generic persona name (Alex Rivera), no project-specific terms.

---

### ❌ BAD: Project-Specific in agents/

**File:** `agents/product-manager/references/persona-examples.md` (WRONG)

```markdown
## Example: Insurance CRM - Distribution Manager

**Name:** Sarah Chen
**Role:** Distribution Manager
**Goals:** Manage broker relationships, track submissions, process renewals
```

**Why bad:** References insurance domain, project-specific persona (Sarah Chen), project-specific entities (broker, submissions, renewals). This belongs in `planning-mds/examples/personas/`.

---

### ✅ GOOD: Solution-Specific in planning-mds/

**File:** `planning-mds/examples/personas/nebula-personas.md`

```markdown
## Persona: Distribution Manager

**Name:** Sarah Chen
**Role:** Distribution Manager at Nebula Insurance
**Goals:** Manage broker relationships, track submissions, process renewals
```

**Why good:** Project-specific persona in the correct location (`planning-mds/`).

---

## Version History

**Version 1.0** - 2026-02-01 - Initial boundary policy
EOF

echo "✅ Created BOUNDARY-POLICY.md"
```

---

### Step 6.2: Create planning-mds/README.md

**File:** `planning-mds/README.md`

```bash
cat > planning-mds/README.md <<'EOF'
# Project Planning Documents

This directory contains all solution-specific planning documents, domain knowledge, and examples for this project.

---

## Directory Structure

```
planning-mds/
├── INCEPTION.md                     # Master specification (single source of truth)
├── BOUNDARY-POLICY.md               # Policy defining agents/ vs planning-mds/ boundary
│
├── domain/                          # Domain knowledge
│   ├── README.md                    # Domain knowledge overview
│   └── [domain-specific files]      # Glossary, competitive analysis, patterns
│
├── examples/                        # Project-specific examples
│   ├── README.md                    # Examples overview
│   ├── personas/                    # Project personas
│   ├── features/                    # Project features
│   ├── stories/                     # Project user stories
│   ├── screens/                     # Project screen specs
│   └── architecture/                # Project architecture examples
│       └── adrs/                    # Architecture Decision Records
│
├── features/                        # Actual project features
├── stories/                         # Actual project user stories
├── architecture/                    # Architecture decisions and specs
│   └── decisions/                   # ADRs (when written)
├── api/                             # API contracts
├── security/                        # Security artifacts
└── workflows/                       # Workflow specifications
```

---

## What's in This Directory

### Domain Knowledge (`domain/`)

**Purpose:** Domain-specific knowledge that Product Managers and Architects need to understand this project.

**Contents:**
- Domain glossary (terminology, definitions)
- Competitive analysis (market landscape, baseline features)
- Domain-specific architecture patterns

**When to use:** Reference during requirements definition and architecture design to understand domain context.

---

### Examples Library (`examples/`)

**Purpose:** Real project examples showing how to apply generic templates to this specific solution.

**Product Examples:**
- `personas/` - Project user personas
- `features/` - Project feature examples
- `stories/` - Project user story examples
- `screens/` - Project screen specifications

**Architecture Examples:**
- `architecture/` - Complete architecture examples for this project
- `architecture/adrs/` - Architecture Decision Records explaining key technical choices

**When to use:** Reference when writing actual features, stories, and architecture. Use as templates showing "here's how the generic template was applied to our solution."

---

## Relationship to agents/

**agents/ = Generic (reusable across projects)**
- Contains agent role definitions (SKILL.md, README.md)
- Contains generic best practices
- Contains generic examples from multiple domains
- Contains generic templates
- **Can be copied wholesale to a new project**

**planning-mds/ = Solution-Specific (this project only)**
- Contains project domain knowledge
- Contains project-specific examples
- Contains actual project requirements
- **Unique to this project** - would be replaced for a new project

**See:** `BOUNDARY-POLICY.md` for detailed rules on what belongs where.

---

## How to Use

**For Product Managers:**
1. Read `domain/` to understand domain terminology and competitive landscape
2. Reference `examples/personas/`, `examples/features/`, `examples/stories/` to see how to write project specs
3. Use `../agents/templates/` for generic templates
4. Write actual features/stories in `features/` and `stories/` directories

**For Architects:**
1. Read `domain/` to understand domain-specific architecture patterns
2. Reference `examples/architecture/` to see complete module examples
3. Use `../agents/templates/adr-template.md` for ADR structure
4. Write actual architecture decisions in `architecture/decisions/`
5. Write actual API contracts in `api/`

---

## Version History

**Version 1.0** - 2026-02-01 - Initial planning-mds structure with domain knowledge and examples separated from generic agents
EOF

echo "✅ Created planning-mds/README.md"
```

---

### Step 6.3: Create planning-mds/domain/README.md

**File:** `planning-mds/domain/README.md`

```bash
cat > planning-mds/domain/README.md <<'EOF'
# Domain Knowledge

This directory contains domain-specific knowledge for this project.

---

## Purpose

Domain knowledge helps Product Managers, Architects, and Developers understand the business context, terminology, competitive landscape, and domain-specific patterns.

---

## Files in This Directory

### Domain Glossary

**File:** `[domain]-glossary.md` (e.g., `insurance-glossary.md`, `healthcare-glossary.md`)

**Purpose:** Defines domain-specific terminology.

**Contents:**
- Core entities and their definitions
- Industry-specific process terms
- Relationships between entities
- Common abbreviations and acronyms

**When to use:** Reference when writing user stories, features, or architecture to ensure correct terminology.

---

### Competitive Analysis

**File:** `[domain]-competitive-analysis.md` (e.g., `crm-competitive-analysis.md`)

**Purpose:** Provides competitive landscape analysis.

**Contents:**
- Key competitors and their features
- Table-stakes features (must-haves for market parity)
- Domain-specific features
- UI/UX patterns common in the industry

**When to use:** Reference during requirements definition to ensure feature parity and understand industry standards.

---

### Domain-Specific Architecture Patterns

**File:** `[domain]-architecture-patterns.md` (e.g., `crm-architecture-patterns.md`)

**Purpose:** Architectural patterns specific to the domain.

**Contents:**
- Domain-specific data models and relationships
- Common workflow patterns
- Domain-specific state machines
- Performance and scalability patterns
- Anti-patterns to avoid

**When to use:** Reference during architecture design to understand domain-specific challenges and proven solutions.

---

## How to Create Domain Knowledge

### 1. Domain Glossary

```markdown
# [Domain] Glossary

## Core Entities

### [Entity Name]
**Definition:** [Clear, concise definition]
**Example:** [Real-world example]
**Relationships:** [Related entities]

## Process Terms

### [Process Name]
**Definition:** [What it is]
**Steps:** [Key steps if applicable]
**Stakeholders:** [Who's involved]
```

### 2. Competitive Analysis

```markdown
# [Domain] Competitive Analysis

## Competitors

### [Competitor Name]
**Market Position:** [Leader/Challenger/Niche]
**Key Features:** [Bulleted list]
**Strengths:** [What they do well]
**Gaps:** [What they're missing]

## Table-Stakes Features
- Feature 1
- Feature 2

## Domain-Specific Features
- Feature 1
- Feature 2
```

### 3. Architecture Patterns

```markdown
# [Domain] Architecture Patterns

## Pattern: [Pattern Name]

**Problem:** [What problem does it solve]
**Solution:** [How to implement]
**Example:** [Code/diagram/pseudocode]
**Trade-offs:** [Pros/cons]
```

---

## Version History

**Version 1.0** - 2026-02-01 - Initial domain knowledge directory
EOF

echo "✅ Created planning-mds/domain/README.md"
```

---

### Step 6.4: Create planning-mds/examples/README.md

**File:** `planning-mds/examples/README.md`

```bash
cat > planning-mds/examples/README.md <<'EOF'
# Project Examples

This directory contains project-specific examples showing how generic templates are applied to this solution.

---

## Purpose

Examples serve as:
1. **Templates** - Show how to apply generic patterns to this project
2. **Reference** - Provide context for new team members
3. **Validation** - Ensure consistency across similar deliverables

---

## Directory Structure

```
examples/
├── personas/           # Project user personas
├── features/           # Project feature examples
├── stories/            # Project user story examples
├── screens/            # Project screen specifications
└── architecture/       # Project architecture examples
    └── adrs/           # Architecture Decision Records
```

---

## How to Use Examples

### For Product Managers

**Personas:**
- See `personas/` for project-specific user personas
- Use generic template from `../../agents/templates/persona-template.md`
- Reference these examples when creating new personas

**Features:**
- See `features/` for project-specific feature examples
- Use generic template from `../../agents/templates/feature-template.md`
- Reference these examples when defining new features

**Stories:**
- See `stories/` for project-specific user story examples
- Use generic template from `../../agents/templates/story-template.md`
- Reference these examples when writing new stories

**Screens:**
- See `screens/` for project-specific screen specifications
- Use generic template from `../../agents/templates/screen-spec-template.md`
- Reference these examples when specifying new screens

### For Architects

**Architecture:**
- See `architecture/` for complete module/workflow examples
- Use generic patterns from `../../agents/architect/references/architecture-examples.md`
- Reference these examples when designing new modules

**ADRs:**
- See `architecture/adrs/` for Architecture Decision Record examples
- Use generic template from `../../agents/templates/adr-template.md`
- Reference these examples when documenting new architecture decisions

---

## Difference from agents/references/

**agents/references/ = Generic examples from multiple domains**
- Examples span B2B SaaS, e-commerce, healthcare, etc.
- Show how patterns work in different contexts
- Reusable across any project

**planning-mds/examples/ = Project-specific examples**
- Examples use project entities, personas, workflows
- Show how patterns apply to THIS project
- Unique to this solution

---

## Version History

**Version 1.0** - 2026-02-01 - Initial examples directory
EOF

echo "✅ Created planning-mds/examples/README.md"
```

---

### Step 6.5: Create Subdirectory READMEs

```bash
# Create personas README
cat > planning-mds/examples/personas/README.md <<'EOF'
# Project Personas

Project-specific user personas.

## Purpose
Define who will use this system, their goals, pain points, and behavioral patterns.

## Template
Use: `../../../agents/templates/persona-template.md`

## Generic Examples
See: `../../../agents/product-manager/references/persona-examples.md`

## Files in This Directory
List your project-specific persona files here (e.g., `primary-personas.md`, `secondary-personas.md`)
EOF

# Create features README
cat > planning-mds/examples/features/README.md <<'EOF'
# Project Features

Project-specific feature examples.

## Purpose
Show how features are defined for this project, following the generic feature template.

## Template
Use: `../../../agents/templates/feature-template.md`

## Generic Examples
See: `../../../agents/product-manager/references/feature-examples.md`

## Files in This Directory
List your project-specific feature example files here
EOF

# Create stories README
cat > planning-mds/examples/stories/README.md <<'EOF'
# Project User Stories

Project-specific user story examples.

## Purpose
Show how user stories are written for this project, with complete acceptance criteria.

## Template
Use: `../../../agents/templates/story-template.md`

## Generic Examples
See: `../../../agents/product-manager/references/story-examples.md`

## Files in This Directory
List your project-specific story example files here
EOF

# Create screens README
cat > planning-mds/examples/screens/README.md <<'EOF'
# Project Screen Specifications

Project-specific screen/UI specifications.

## Purpose
Define screen layouts, components, interactions, and validation rules for this project.

## Template
Use: `../../../agents/templates/screen-spec-template.md`

## Generic Examples
See: `../../../agents/product-manager/references/screen-spec-examples.md` (if exists)

## Files in This Directory
List your project-specific screen specification files here
EOF

# Create architecture README
cat > planning-mds/examples/architecture/README.md <<'EOF'
# Project Architecture Examples

Project-specific architecture examples and decisions.

## Purpose
Provide complete architecture examples (entities, workflows, APIs, authorization) for this project.

## Template
Use: `../../../agents/templates/adr-template.md` for ADRs

## Generic Examples
See: `../../../agents/architect/references/architecture-examples.md`

## Subdirectories
- `adrs/` - Architecture Decision Records

## Files in This Directory
List your project-specific architecture example files here
EOF

# Create ADRs README
cat > planning-mds/examples/architecture/adrs/README.md <<'EOF'
# Architecture Decision Records (ADRs)

Project-specific architecture decisions.

## Purpose
Document key technical decisions, rationale, alternatives considered, and consequences.

## Template
Use: `../../../../agents/templates/adr-template.md`

## Naming Convention
`ADR-XXX-short-title.md` (e.g., `ADR-001-modular-monolith.md`)

## Files in This Directory
List your ADR files here as they're created
EOF

echo "✅ Created subdirectory READMEs"
```

---

### Step 6.6: Create agents/README.md

**File:** `agents/README.md`

```bash
cat > agents/README.md <<'EOF'
# Generic Agent Roles

This directory contains **generic, reusable** agent role definitions and best practices for building software with AI agents.

## ✅ 100% Generic - Ready for Reuse

All content in this directory is **domain-agnostic** and can be applied to any software project:
- ✅ Insurance CRM
- ✅ Underwriting Workbench
- ✅ Claims Management System
- ✅ Policy Administration System
- ✅ E-commerce Platform
- ✅ Healthcare SaaS
- ✅ B2B SaaS
- ✅ Any other software project

## Directory Structure

```
agents/
├── product-manager/
│   ├── SKILL.md                      # PM agent role definition (generic)
│   ├── README.md                     # PM quick start guide (generic)
│   ├── references/
│   │   ├── pm-best-practices.md      # INVEST, slicing, AC patterns (generic)
│   │   ├── vertical-slicing-guide.md # Feature decomposition (generic)
│   │   ├── persona-examples.md       # B2B SaaS, e-commerce, healthcare examples
│   │   ├── feature-examples.md       # Task mgmt, e-commerce, scheduling examples
│   │   └── story-examples.md         # Generic story examples with full AC
│   └── scripts/                      # Generic validation scripts
│
├── architect/
│   ├── SKILL.md                      # Architect agent role definition (generic)
│   ├── README.md                     # Architect quick start guide (generic)
│   ├── references/
│   │   ├── architecture-best-practices.md  # SOLID, DDD, Clean Arch (generic)
│   │   ├── architecture-examples.md        # E-commerce, CMS, SaaS examples
│   │   ├── api-design-guide.md             # REST API patterns (generic)
│   │   ├── data-modeling-guide.md          # EF Core, PostgreSQL (generic)
│   │   ├── authorization-patterns.md       # ABAC, Casbin, Keycloak (generic)
│   │   ├── service-architecture-patterns.md # Modular monolith, DDD (generic)
│   │   ├── security-architecture-guide.md   # Auth, encryption, OWASP (generic)
│   │   ├── performance-design-guide.md      # Optimization, caching (generic)
│   │   └── workflow-design.md              # State machines, Temporal (generic)
│   └── scripts/                      # Generic validation scripts
│
├── templates/                        # Generic templates (all domains)
│   ├── story-template.md
│   ├── feature-template.md
│   ├── persona-template.md
│   ├── adr-template.md
│   ├── api-contract-template.yaml
│   └── [other generic templates]
│
├── README.md                         # This file
└── AGENT-STATUS.md                   # Agent development status
```

## How to Use for a New Project

### Step 1: Copy agents/ Directory

```bash
# Example: Creating an Underwriting Workbench
cp -r current-project/agents new-project/agents
```

**Result:** You now have all agent roles, best practices, and templates ready to use.

---

### Step 2: Create Solution-Specific planning-mds/

```bash
mkdir -p new-project/planning-mds/{domain,examples,features,stories,architecture}
```

---

### Step 3: Create Domain Knowledge

Create files in `planning-mds/domain/`:

- `[domain]-glossary.md` - Define domain-specific terminology
- `[domain]-competitive-analysis.md` - Analyze competitive landscape
- `[domain]-architecture-patterns.md` - Document domain-specific patterns

---

### Step 4: Create Solution-Specific Examples

Create files in `planning-mds/examples/`:

- `personas/` - Create personas for your solution
- `features/` - Create feature examples for your solution
- `stories/` - Create story examples for your solution
- `architecture/` - Create architecture examples for your solution

---

### Step 5: Create INCEPTION.md

`new-project/planning-mds/INCEPTION.md`:
- Use `../agents/templates/` as structure reference
- Reference your domain knowledge in Section 2.0
- Define your vision, personas, features, stories

---

### Step 6: Start Building

You're now ready to execute:
- **Phase A:** Use Product Manager agent to define requirements
- **Phase B:** Use Architect agent to design technical architecture
- **Phase C:** Use implementation agents to build

All generic best practices, templates, and patterns are ready to use from `agents/`.

---

## What's NOT in This Directory

Solution-specific content does NOT belong here:
- ❌ Domain-specific terminology (industry jargon, entity names)
- ❌ Competitive analysis for specific markets
- ❌ Solution-specific examples (personas, features, stories referencing your domain)
- ❌ Solution-specific architecture examples (modules/workflows specific to your domain)

**That content belongs in `planning-mds/` (or equivalent) for each solution.**

See: `../planning-mds/BOUNDARY-POLICY.md` for detailed rules.

---

## Version History

**Version 2.0** - 2026-02-01 - Separated generic agents from solution-specific content
**Version 1.0** - 2026-01-26 - Initial agent roles created
EOF

echo "✅ Created agents/README.md"
```

---

## PHASE 7: UPDATE INCEPTION.MD

Add reference documentation section to INCEPTION.md.

### Step 7.1: Check INCEPTION.md Structure

```bash
# Check current section structure
grep "^##" planning-mds/INCEPTION.md | head -20

# Identify where to insert "2.0 Reference Documentation"
```

**Manual Step:** Open `planning-mds/INCEPTION.md` and find "## 2. Product Context" section.

---

### Step 7.2: Add Reference Documentation Section

Insert this content after "## 2. Product Context" heading:

```markdown
### 2.0 Reference Documentation

This section provides links to domain knowledge and examples specific to this project.

**Domain Knowledge:**
- **Domain Glossary:** See `domain/[domain]-glossary.md` for domain terminology
- **Competitive Analysis:** See `domain/[domain]-competitive-analysis.md` for market landscape
- **Architecture Patterns:** See `domain/[domain]-architecture-patterns.md` for domain-specific architectural patterns

**Examples Library:**

*Product Examples:*
- **Personas:** See `examples/personas/` for project user personas
- **Features:** See `examples/features/` for project feature examples
- **Stories:** See `examples/stories/` for project user story examples
- **Screens:** See `examples/screens/` for project screen specifications

*Architecture Examples:*
- **Modules:** See `examples/architecture/` for complete module examples
- **ADRs:** See `examples/architecture/adrs/` for Architecture Decision Records

**Generic Resources:**

For generic best practices and templates (applicable to any solution, not just this project):
- See `../agents/product-manager/references/` for PM best practices, generic persona/feature/story examples
- See `../agents/architect/references/` for architecture best practices, generic architecture examples
- See `../agents/templates/` for all templates (story, feature, persona, ADR, API contract, etc.)

---
```

---

## PHASE 8: UPDATE AGENT-STATUS.MD

Update agent status to reflect refactoring completion.

### Step 8.1: Update Product Manager Section

**File:** `agents/AGENT-STATUS.md`

Find Product Manager section and update:

```markdown
#### 1. ✅ Product Manager
- **Status:** COMPLETE - Generic/Solution Separation Complete
- **Artifacts:**
  - SKILL.md ✅ (updated with generic/solution resource separation)
  - README.md ✅ (updated with generic/solution resource separation)
  - references/ (generic files) ✅
    - pm-best-practices.md (generic)
    - vertical-slicing-guide.md (generic)
    - inception-requirements.md (generic)
    - persona-examples.md ⭐ *rewritten with generic examples (B2B SaaS, e-commerce, healthcare)*
    - feature-examples.md ⭐ *rewritten with generic examples (task mgmt, e-commerce, scheduling)*
    - story-examples.md ⭐ *rewritten with generic examples*
  - scripts/ (2 files) ✅
    - validate-stories.py
    - generate-story-index.py
- **Solution-Specific Content Moved to planning-mds/:**
  - Domain glossary → planning-mds/domain/
  - Competitive analysis → planning-mds/domain/
  - Project persona examples → planning-mds/examples/personas/
  - Project feature examples → planning-mds/examples/features/
  - Project story examples → planning-mds/examples/stories/
  - Project screen specs → planning-mds/examples/screens/
- **Completed:** 2026-02-01 - Generic agent role complete, ready for reuse in other projects
- **Notes:** Agent is now 100% generic and reusable. All project-specific content in planning-mds/
```

---

### Step 8.2: Update Architect Section

**File:** `agents/AGENT-STATUS.md`

Find Architect section and update:

```markdown
#### 2. ✅ Architect
- **Status:** COMPLETE - Generic/Solution Separation Complete
- **Artifacts:**
  - SKILL.md ✅ (updated with generic/solution resource separation)
  - README.md ✅ (updated with generic/solution resource separation)
  - references/ (generic files) ✅
    - architecture-best-practices.md (generic)
    - architecture-examples.md ⭐ *rewritten with generic examples (e-commerce, CMS, SaaS)*
    - api-design-guide.md (generic)
    - data-modeling-guide.md (generic)
    - authorization-patterns.md (generic)
    - service-architecture-patterns.md (generic)
    - security-architecture-guide.md (generic)
    - performance-design-guide.md (generic)
    - workflow-design.md (generic)
- **Solution-Specific Content Moved to planning-mds/:**
  - Domain architecture patterns → planning-mds/domain/
  - Project architecture examples → planning-mds/examples/architecture/
- **Completed:** 2026-02-01 - Generic agent role complete, ready for reuse
- **Notes:** Agent is now 100% generic and reusable. All project-specific content in planning-mds/
```

---

## PHASE 9: COMPREHENSIVE VALIDATION

Verify the refactoring is complete and correct.

### Step 9.1: Validate File Movements

```bash
echo "=== Validating file movements ===" > docs/validation-report.md

# Check planning-mds/ has expected files
echo "" >> docs/validation-report.md
echo "## Files in planning-mds/:" >> docs/validation-report.md
find planning-mds -type f -name "*.md" | sort >> docs/validation-report.md

# Check agents/ no longer has project-specific files
echo "" >> docs/validation-report.md
echo "## Checking agents/ for removed files:" >> docs/validation-report.md
echo "(Should not find project-specific examples)" >> docs/validation-report.md
ls agents/product-manager/references/ | grep -E "(insurance|crm-competitive|nebula)" >> docs/validation-report.md 2>&1 || echo "✅ No project-specific files found" >> docs/validation-report.md

cat docs/validation-report.md
```

---

### Step 9.2: Comprehensive Nebula-Specific Term Scan

```bash
echo "=== Scanning agents/ for ALL project-specific terms ===" > docs/comprehensive-scan.log

# Comprehensive term list
terms=(
  "Broker" "Submission" "Sarah Chen" "Marcus" "Jennifer"
  "Nebula" "MGA" "Account" "Renewal" "Binder" "Commission"
  "F1\b" "F2\b" "F3\b" "F4\b" "F5\b" "F6\b"
  "S1:" "S2:" "S3:" "S4:" "S5:" "S6:" "S7:"
  "insurance CRM" "broker hierarchy" "submission workflow"
  "renewal pipeline" "distribution manager" "underwriter persona"
  "coordinator persona" "Applied Epic" "Vertafore" "Duck Creek"
)

for term in "${terms[@]}"; do
  echo "" >> docs/comprehensive-scan.log
  echo "## Scanning for: $term" >> docs/comprehensive-scan.log
  grep -rn "$term" agents/ --include="*.md" >> docs/comprehensive-scan.log 2>/dev/null || echo "✅ No matches" >> docs/comprehensive-scan.log
done

# Review scan results
cat docs/comprehensive-scan.log

# Count matches (should be 0)
match_count=$(grep -rc "Broker\|Submission\|Sarah Chen\|Marcus\|Jennifer\|Nebula\|MGA\|F1\|F2\|F3" agents/ --include="*.md" | grep -v ":0" | wc -l)
echo "" >> docs/comprehensive-scan.log
echo "=== Total files with matches: $match_count ===" >> docs/comprehensive-scan.log

if [ "$match_count" -eq 0 ]; then
  echo "✅ PASS: No project-specific terms found in agents/" >> docs/comprehensive-scan.log
else
  echo "❌ FAIL: Found $match_count files with project-specific terms" >> docs/comprehensive-scan.log
fi

cat docs/comprehensive-scan.log
```

**Expected:** 0 matches (all project-specific terms removed from agents/)

---

### Step 9.3: Validate Generic Examples

```bash
echo "=== Validating generic examples contain no project-specific content ===" > docs/generic-validation.log

generic_files=(
  "agents/product-manager/references/persona-examples.md"
  "agents/product-manager/references/feature-examples.md"
  "agents/product-manager/references/story-examples.md"
  "agents/architect/references/architecture-examples.md"
)

for file in "${generic_files[@]}"; do
  echo "" >> docs/generic-validation.log
  echo "## Validating: $file" >> docs/generic-validation.log

  if [ -f "$file" ]; then
    grep -in "broker\|submission\|nebula\|sarah\|marcus\|jennifer\|MGA\|insurance CRM" "$file" >> docs/generic-validation.log 2>&1 || echo "✅ No project-specific content" >> docs/generic-validation.log
  else
    echo "⚠️  File not found" >> docs/generic-validation.log
  fi
done

cat docs/generic-validation.log
```

**Expected:** All files should show "✅ No project-specific content"

---

### Step 9.4: Validate Cross-References

```bash
echo "=== Validating cross-references and links ===" > docs/link-validation.log

# Find all markdown links in agents/ and planning-mds/
echo "## Checking markdown links in agents/:" >> docs/link-validation.log
grep -rn "\[.*\](.*\.md)" agents/ --include="*.md" >> docs/link-validation.log 2>/dev/null

echo "" >> docs/link-validation.log
echo "## Checking markdown links in planning-mds/:" >> docs/link-validation.log
grep -rn "\[.*\](.*\.md)" planning-mds/ --include="*.md" >> docs/link-validation.log 2>/dev/null

# TODO: Manually verify links point to existing files
cat docs/link-validation.log
```

**Manual Step:** Review link-validation.log and verify all referenced files exist.

---

### Step 9.5: Test Reusability - Simulate New Project

```bash
# Create temporary directory for reusability test
cd ..
mkdir -p reusability-test/underwriting-workbench

# Copy agents/ directory
cp -r insurance-crm/agents reusability-test/underwriting-workbench/

echo "=== Reusability Test ===" > insurance-crm/docs/reusability-test.log

# Verify agents/ copied successfully
echo "## Files copied:" >> insurance-crm/docs/reusability-test.log
find reusability-test/underwriting-workbench/agents -type f -name "*.md" | wc -l >> insurance-crm/docs/reusability-test.log

# Scan copied agents/ for project-specific terms
echo "" >> insurance-crm/docs/reusability-test.log
echo "## Scanning copied agents/ for project-specific terms:" >> insurance-crm/docs/reusability-test.log
grep -r "Broker\|Submission\|Sarah Chen\|Nebula\|MGA\|F1\|F2\|F3" reusability-test/underwriting-workbench/agents/ --include="*.md" >> insurance-crm/docs/reusability-test.log 2>&1 || echo "✅ No project-specific terms found" >> insurance-crm/docs/reusability-test.log

# Cleanup
rm -rf reusability-test

cat insurance-crm/docs/reusability-test.log
cd insurance-crm
```

**Expected:** No project-specific terms found in copied agents/

---

### Step 9.6: Validate Documentation Completeness

```bash
echo "=== Validating documentation completeness ===" > docs/documentation-validation.log

required_docs=(
  "planning-mds/README.md"
  "planning-mds/BOUNDARY-POLICY.md"
  "planning-mds/domain/README.md"
  "planning-mds/examples/README.md"
  "planning-mds/examples/personas/README.md"
  "planning-mds/examples/features/README.md"
  "planning-mds/examples/stories/README.md"
  "planning-mds/examples/screens/README.md"
  "planning-mds/examples/architecture/README.md"
  "planning-mds/examples/architecture/adrs/README.md"
  "agents/README.md"
)

for doc in "${required_docs[@]}"; do
  if [ -f "$doc" ]; then
    echo "✅ $doc" >> docs/documentation-validation.log
  else
    echo "❌ MISSING: $doc" >> docs/documentation-validation.log
  fi
done

cat docs/documentation-validation.log
```

**Expected:** All documentation files exist

---

### Step 9.7: Final Validation Summary

```bash
cat > docs/REFACTORING-VALIDATION-SUMMARY.md <<'EOF'
# Refactoring Validation Summary

**Date:** 2026-02-01
**Status:** [PASS/FAIL]

---

## Validation Checks

### ✅ Phase 0: Discovery
- [ ] File inventory completed
- [ ] Nebula-specific content identified
- [ ] Scripts analyzed for solution-specific code
- [ ] Templates reviewed for embedded examples
- [ ] Other agents checked

### ✅ Phase 1: Directory Structure
- [ ] planning-mds/domain/ created
- [ ] planning-mds/examples/ subdirectories created
- [ ] Directory structure verified

### ✅ Phase 2: File Movements
- [ ] Product Manager domain files moved
- [ ] Architect domain files moved
- [ ] Product Manager examples moved
- [ ] Architect examples moved
- [ ] No Nebula content lost

### ✅ Phase 3: Generic Examples
- [ ] Generic persona-examples.md created
- [ ] Generic feature-examples.md created
- [ ] Generic story-examples.md created
- [ ] Generic architecture-examples.md created
- [ ] Generic files contain NO project-specific content

### ✅ Phase 4: Templates
- [ ] Story template reviewed and cleaned
- [ ] Feature template reviewed and cleaned
- [ ] Persona template reviewed and cleaned
- [ ] Screen spec template reviewed and cleaned
- [ ] ADR template reviewed and cleaned

### ✅ Phase 5: Agent Documentation
- [ ] Product Manager SKILL.md updated
- [ ] Product Manager README.md updated
- [ ] Architect SKILL.md updated
- [ ] Architect README.md updated
- [ ] DevOps agent checked (if exists)

### ✅ Phase 6: planning-mds/ Documentation
- [ ] BOUNDARY-POLICY.md created
- [ ] planning-mds/README.md created
- [ ] planning-mds/domain/README.md created
- [ ] planning-mds/examples/README.md created
- [ ] Subdirectory READMEs created
- [ ] agents/README.md created

### ✅ Phase 7: INCEPTION.md
- [ ] Reference documentation section added

### ✅ Phase 8: AGENT-STATUS.md
- [ ] Product Manager section updated
- [ ] Architect section updated

### ✅ Phase 9: Validation
- [ ] File movements validated
- [ ] Comprehensive term scan completed (0 matches)
- [ ] Generic examples validated (no project-specific content)
- [ ] Cross-references validated
- [ ] Reusability test passed
- [ ] Documentation completeness validated

---

## Results

**Project-Specific Terms in agents/:** [count]
**Expected:** 0

**Broken Links:** [count]
**Expected:** 0

**Missing Documentation:** [count]
**Expected:** 0

---

## Reusability Test

**Test:** Copy agents/ to new project, scan for project-specific terms
**Result:** [PASS/FAIL]
**Details:** [outcome]

---

## Final Status

**Overall Result:** [PASS/FAIL]

**If PASS:**
- ✅ agents/ is 100% generic and reusable
- ✅ planning-mds/ contains all project-specific content
- ✅ Clear boundary established
- ✅ Documentation complete
- ✅ Ready to copy agents/ to new projects

**If FAIL:**
- ❌ Issues found: [list issues]
- ❌ Remediation needed: [list actions]

EOF

echo "✅ Created validation summary template"
echo "📝 Review docs/REFACTORING-VALIDATION-SUMMARY.md and fill in results"
```

---

## PHASE 10: GIT COMMIT

Commit all changes with a comprehensive commit message.

### Step 10.1: Review Changes

```bash
# Review all changes
git status

# Review diffs (optional)
git diff agents/
git diff planning-mds/
```

---

### Step 10.2: Stage All Changes

```bash
git add -A
```

---

### Step 10.3: Commit with Detailed Message

```bash
git commit -m "$(cat <<'EOF'
refactor: Separate generic agent roles from solution-specific content

BREAKING CHANGE: Repository structure refactored to enable agent reusability

## Summary

Separated generic, reusable agent content (agents/) from solution-specific
content (planning-mds/) to enable copying agents/ to new projects.

## Changes Made

### File Movements (agents/ → planning-mds/)

**Product Manager:**
- Moved domain glossary → planning-mds/domain/
- Moved competitive analysis → planning-mds/domain/
- Moved project personas → planning-mds/examples/personas/
- Moved project features → planning-mds/examples/features/
- Moved project stories → planning-mds/examples/stories/
- Moved project screens → planning-mds/examples/screens/

**Architect:**
- Moved domain architecture patterns → planning-mds/domain/
- Moved project architecture examples → planning-mds/examples/architecture/

### New Generic Examples Created

**Product Manager:**
- Created generic persona-examples.md (B2B SaaS, e-commerce, healthcare)
- Created generic feature-examples.md (task mgmt, e-commerce, scheduling)
- Created generic story-examples.md (create task, add to cart)

**Architect:**
- Created generic architecture-examples.md (e-commerce, CMS, SaaS)

### Documentation Created

- planning-mds/BOUNDARY-POLICY.md - Defines generic vs solution-specific boundary
- planning-mds/README.md - planning-mds/ overview
- planning-mds/domain/README.md - Domain knowledge overview
- planning-mds/examples/README.md - Examples overview
- planning-mds/examples/*/README.md - Subdirectory documentation
- agents/README.md - Agent reusability guide

### Documentation Updated

- agents/product-manager/SKILL.md - Added generic/solution resource separation
- agents/product-manager/README.md - Added generic/solution resource separation
- agents/architect/SKILL.md - Added generic/solution resource separation
- agents/architect/README.md - Added generic/solution resource separation
- planning-mds/INCEPTION.md - Added reference documentation section
- agents/AGENT-STATUS.md - Updated completion status

### Templates Cleaned

- Reviewed all templates for project-specific examples
- Replaced with generic placeholders where needed

## Validation

✅ Zero project-specific terms in agents/
✅ All project-specific content in planning-mds/
✅ Cross-references validated
✅ Reusability test passed
✅ Documentation complete

## Result

- ✅ agents/ is 100% generic and reusable across any project
- ✅ planning-mds/ contains all project-specific content
- ✅ Clear boundary established via BOUNDARY-POLICY.md
- ✅ Ready to copy agents/ to new projects (underwriting, claims, policy admin, etc.)

## How to Reuse for New Project

1. Copy entire agents/ directory to new project
2. Create new planning-mds/ with project-specific domain knowledge and examples
3. Agents are immediately ready to use with new project context

See: agents/README.md for detailed reusability guide

Co-Authored-By: Claude Opus 4.5 <noreply@anthropic.com>
EOF
)"
```

---

### Step 10.4: Push to Remote (Optional)

```bash
# Push feature branch
git push origin refactor/separate-generic-from-solution

# Create pull request (if desired)
# Use your preferred PR creation method
```

---

## SUCCESS CRITERIA

After executing this plan, verify:

### 1. ✅ No Project-Specific Content in agents/
- No references to project entities (Broker, Submission, Account, etc.)
- No references to project personas (Sarah Chen, Marcus, Jennifer, etc.)
- No references to project features (F1-F6) or stories (S1-S7)
- All examples are generic (B2B SaaS, e-commerce, healthcare, etc.)

### 2. ✅ All Project-Specific Content in planning-mds/
- Domain knowledge in planning-mds/domain/
- Examples in planning-mds/examples/
- Actual requirements in planning-mds/{features,stories,architecture}/

### 3. ✅ References Updated
- SKILL.md and README.md files reference both generic and solution-specific resources
- INCEPTION.md has reference documentation section
- All paths are correct and files can be found

### 4. ✅ Documentation Complete
- BOUNDARY-POLICY.md explains separation rules
- README files at all levels explain purpose and usage
- agents/README.md provides reusability guide

### 5. ✅ Reusability Test Passes
- Can copy agents/ to a new project directory
- No project-specific content in copied agents/
- New project can create its own planning-mds/ with different domain knowledge

### 6. ✅ Validation Clean
- Comprehensive term scan shows 0 matches
- Generic examples contain no project-specific content
- Cross-references all valid
- No broken links

---

## Rollback Plan

If refactoring fails or introduces issues:

```bash
# Discard all changes on feature branch
git checkout refactor/separate-generic-from-solution
git reset --hard origin/feat/finish-agent-roles

# Or delete feature branch entirely
git checkout feat/finish-agent-roles
git branch -D refactor/separate-generic-from-solution
```

---

## Version History

**Version 2.0** - 2026-02-01 - Comprehensive refactoring plan addressing all gaps
**Version 1.0** - 2026-01-31 - Initial refactoring plan
