 Comprehensive Refactoring Prompt: Separate Generic Agents from Solution-Specific Content

  Execute this prompt to cleanly separate reusable agent roles from Nebula CRM-specific content. This refactoring ensures agents/ can be copied to any new
  project (underwriting workbench, claims system, etc.) while all Nebula-specific content lives in planning-mds/.

  ---
  Objective

  Transform the repository from:
  - ❌ Mixed: Generic + Nebula-specific content in agents/
  - ✅ To: Clean separation with agents/ = 100% generic, planning-mds/ = 100% Nebula-specific

  Prerequisites

  Current working directory: C:\Users\gajap\sandbox\insurance-crm

  ---
  PHASE 1: Create New Directory Structure

  Step 1.1: Create planning-mds/domain/ for Domain Knowledge

  mkdir -p planning-mds/domain

  Purpose: Store Nebula CRM-specific domain knowledge (currently polluting agents/)

  ---
  Step 1.2: Create planning-mds/examples/ for Solution Examples

  mkdir -p planning-mds/examples/personas
  mkdir -p planning-mds/examples/features
  mkdir -p planning-mds/examples/stories
  mkdir -p planning-mds/examples/screens
  mkdir -p planning-mds/examples/architecture
  mkdir -p planning-mds/examples/architecture/adrs

  Purpose: Store Nebula-specific examples (currently in agents/*/references/)

  ---
  PHASE 2: Move Solution-Specific Files from agents/ to planning-mds/

  Step 2.1: Move Product Manager Domain Files

  Move insurance-domain-glossary.md:
  mv agents/product-manager/references/insurance-domain-glossary.md planning-mds/domain/insurance-glossary.md

  Move crm-competitive-analysis.md:
  mv agents/product-manager/references/crm-competitive-analysis.md planning-mds/domain/crm-competitive-analysis.md

  Rationale: These files contain Nebula CRM-specific domain knowledge (brokers, MGAs, submissions, renewals). Not applicable to underwriting workbench or
  claims system.

  ---
  Step 2.2: Move Architect Domain Files

  Move insurance-crm-architecture-patterns.md:
  mv agents/architect/references/insurance-crm-architecture-patterns.md planning-mds/domain/crm-architecture-patterns.md

  Rationale: This file contains Nebula CRM-specific architectural patterns (broker hierarchy, submission workflow, renewal pipeline). Not generic.

  ---
  Step 2.3: Move Product Manager Examples

  Move persona-examples.md:
  mv agents/product-manager/references/persona-examples.md planning-mds/examples/personas/nebula-personas.md

  Move feature-examples.md:
  mv agents/product-manager/references/feature-examples.md planning-mds/examples/features/nebula-features.md

  Move story-examples.md:
  mv agents/product-manager/references/story-examples.md planning-mds/examples/stories/nebula-stories.md

  Move screen-spec-examples.md:
  mv agents/product-manager/references/screen-spec-examples.md planning-mds/examples/screens/nebula-screens.md

  Rationale: All examples reference Nebula entities (Broker, Submission, Account), Nebula personas (Sarah Chen), and Nebula features (F1-F6). Not generic.

  ---
  Step 2.4: Move Architect Examples

  Move architecture-examples.md:
  mv agents/architect/references/architecture-examples.md planning-mds/examples/architecture/nebula-architecture.md

  Rationale: All examples reference Nebula entities (Broker, Submission, Account) and Nebula workflows. Not generic.

  ---
 PHASE 3: Create Generic Examples in agents/

  Step 3.1: Create Generic Persona Examples

  File: agents/product-manager/references/persona-examples.md

  Content:
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

  **Version 1.0** - 2026-01-31 - Initial generic persona examples

  ---
  PHASE 4: Update All References + Enforce Boundary

  Step 4.1: Update agent references

  - Update all SKILL.md and README.md files to point to the new planning-mds paths for domain + Nebula-specific examples.
  - Ensure agent docs only reference generic examples in agents/ and solution-specific examples in planning-mds/.
  - Add a clear note in each agent doc: "Solution-specific references live in planning-mds/."

  ---
  Step 4.2: Add a Boundary Policy

  Create a short policy document:

  File: planning-mds/BOUNDARY-POLICY.md

  Content (minimum):
  - agents/ is generic and reusable across projects
  - planning-mds/ is the single source of solution-specific truth
  - agents must not invent or embed solution requirements; they only consume planning-mds/

  ---
  Step 4.3: Align planning-mds references

  - Add a short README in planning-mds/references/ that lists available solution-specific references.
  - Ensure all moved files are referenced from planning-mds (INCEPTION.md or README).

  ---
  PHASE 5: Verification

  Step 5.1: Verify structure

  - Print a tree of agents/ and planning-mds/ to confirm the hard boundary.
  - List all files moved.

  ---
  Step 5.2: Verify references

  - Search for stale paths (old agents/*/references/ paths to Nebula content).
  - Confirm all references resolve to existing files.

  Step 3.2: Create Generic Feature Examples

  File: agents/product-manager/references/feature-examples.md

  Content:
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
  As a medical office administrator, I want to schedule patient appointments and send automated reminders so that we reduce no-shows and maximize provider
  utilization.

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

  **Version 1.0** - 2026-01-31 - Initial generic feature examples

  ---
  Step 3.3: Create Generic Story Examples

  File: agents/product-manager/references/story-examples.md

  Content:
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
  Project managers currently track tasks in spreadsheets, leading to version control issues and lack of visibility. This story enables centralized task
  creation with all essential metadata.

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
  - Task entity: Id (GUID), Title (string), Description (string), AssigneeId (GUID), DueDate (DateTime), Priority (enum), Status (enum), CreatedAt,
  CreatedBy, UpdatedAt, UpdatedBy

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

  **Version 1.0** - 2026-01-31 - Initial generic story examples

  ---
  Step 3.4: Create Generic Architecture Examples

  File: agents/architect/references/architecture-examples.md

  Content:
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

  Order API Contract

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
            content:
              application/json:
                schema:
                  $ref: '#/components/schemas/OrderResponse'
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

  Authorization Policies

  Casbin Policies:

  # Customers can create orders and view their own orders
  p, Customer, Order, Create, allow
  p, Customer, Order, Read, allow, sub.userId == res.customerId

  # WarehouseStaff can update order status (Processing → Shipped)
  p, WarehouseStaff, Order, Update, allow, res.status in ["Processing", "Shipped"]

  # Admins can do everything
  p, Admin, Order, *, allow

  ---
  Example 2: Content Management System - Article Workflow

  Article Entity Specification

  Table Name: Articles

  Fields
  ┌─────────────┬─────────────┬──────────────────┬──────────────────────────────────────┐
  │    Field    │    Type     │   Constraints    │             Description              │
  ├─────────────┼─────────────┼──────────────────┼──────────────────────────────────────┤
  │ Id          │ Guid        │ PK               │ Unique identifier                    │
  ├─────────────┼─────────────┼──────────────────┼──────────────────────────────────────┤
  │ Title       │ string(255) │ NOT NULL         │ Article title                        │
  ├─────────────┼─────────────┼──────────────────┼──────────────────────────────────────┤
  │ Slug        │ string(255) │ UNIQUE, NOT NULL │ URL-friendly slug                    │
  ├─────────────┼─────────────┼──────────────────┼──────────────────────────────────────┤
  │ Content     │ text        │ NULL             │ Article body (markdown)              │
  ├─────────────┼─────────────┼──────────────────┼──────────────────────────────────────┤
  │ AuthorId    │ Guid        │ FK → Users       │ Author                               │
  ├─────────────┼─────────────┼──────────────────┼──────────────────────────────────────┤
  │ Status      │ string(20)  │ NOT NULL         │ Draft, InReview, Published, Archived │
  ├─────────────┼─────────────┼──────────────────┼──────────────────────────────────────┤
  │ PublishedAt │ DateTime    │ NULL             │ When published                       │
  ├─────────────┼─────────────┼──────────────────┼──────────────────────────────────────┤
  │ CreatedAt   │ DateTime    │ NOT NULL         │ Creation timestamp                   │
  ├─────────────┼─────────────┼──────────────────┼──────────────────────────────────────┤
  │ UpdatedAt   │ DateTime    │ NOT NULL         │ Last update timestamp                │
  └─────────────┴─────────────┴──────────────────┴──────────────────────────────────────┘
  Article Workflow

  States:
  - Draft (initial) → InReview → Published (terminal)
  - Draft → Archived (terminal)

  Allowed Transitions:
  ┌───────────┬───────────┬─────────────────────────────┐
  │   From    │    To     │        Prerequisites        │
  ├───────────┼───────────┼─────────────────────────────┤
  │ Draft     │ InReview  │ Title and content populated │
  ├───────────┼───────────┼─────────────────────────────┤
  │ InReview  │ Draft     │ Editor requests changes     │
  ├───────────┼───────────┼─────────────────────────────┤
  │ InReview  │ Published │ Editor approval             │
  ├───────────┼───────────┼─────────────────────────────┤
  │ Published │ Archived  │ Author or editor archives   │
  └───────────┴───────────┴─────────────────────────────┘
  ---
  Example 3: SaaS Subscription Management

  Subscription Entity

  Table Name: Subscriptions

  Fields
  ┌─────────────────┬───────────────┬────────────────┬──────────────────────────────────────────────┐
  │      Field      │     Type      │  Constraints   │                 Description                  │
  ├─────────────────┼───────────────┼────────────────┼──────────────────────────────────────────────┤
  │ Id              │ Guid          │ PK             │ Subscription ID                              │
  ├─────────────────┼───────────────┼────────────────┼──────────────────────────────────────────────┤
  │ CustomerId      │ Guid          │ FK → Customers │ Customer                                     │
  ├─────────────────┼───────────────┼────────────────┼──────────────────────────────────────────────┤
  │ PlanId          │ Guid          │ FK → Plans     │ Subscription plan (Starter, Pro, Enterprise) │
  ├─────────────────┼───────────────┼────────────────┼──────────────────────────────────────────────┤
  │ Status          │ string(20)    │ NOT NULL       │ Active, Suspended, Cancelled, Expired        │
  ├─────────────────┼───────────────┼────────────────┼──────────────────────────────────────────────┤
  │ BillingCycle    │ string(20)    │ NOT NULL       │ Monthly, Yearly                              │
  ├─────────────────┼───────────────┼────────────────┼──────────────────────────────────────────────┤
  │ StartDate       │ DateTime      │ NOT NULL       │ Subscription start                           │
  ├─────────────────┼───────────────┼────────────────┼──────────────────────────────────────────────┤
  │ EndDate         │ DateTime      │ NULL           │ Subscription end (null = ongoing)            │
  ├─────────────────┼───────────────┼────────────────┼──────────────────────────────────────────────┤
  │ NextBillingDate │ DateTime      │ NOT NULL       │ Next charge date                             │
  ├─────────────────┼───────────────┼────────────────┼──────────────────────────────────────────────┤
  │ MRR             │ decimal(18,2) │ NOT NULL       │ Monthly Recurring Revenue                    │
  └─────────────────┴───────────────┴────────────────┴──────────────────────────────────────────────┘
  Subscription Workflow

  States:
  - Active → Suspended (payment failed) → Active (payment recovered)
  - Active → Cancelled → Expired (terminal)

  Business Rules:
  - Cannot cancel if in trial period (trial cancels automatically)
  - Cannot downgrade mid-cycle (takes effect next billing date)
  - Suspend after 3 failed payment attempts
  - Expire 30 days after cancellation (grace period)

  ---
  How to Use These Examples

  1. Select a domain relevant to your solution (or create your own)
  2. Follow the structure (Entity → Workflow → API → Authorization)
  3. Adapt field names and relationships to your domain
  4. Keep patterns consistent across all entities in your system
  5. Reference these examples when designing new modules

  ---
  Version History

  Version 1.0 - 2026-01-31 - Initial generic architecture examples

  ---

  ## PHASE 4: Update Agent References to Point to New Locations

  ### Step 4.1: Update Product Manager SKILL.md

  **File:** `agents/product-manager/SKILL.md`

  **Find and replace:**

  **OLD (lines ~125-130):**
  ```markdown
  **Required Resources:**
  - `planning-mds/INCEPTION.md` - Master specification (sections 3.x from PM)
  - `agents/templates/story-template.md` - Story specification format
  - `agents/templates/persona-template.md` - Persona specification format
  - `agents/templates/screen-spec-template.md` - Screen specification format
  - `agents/product-manager/references/` - PM best practices

  NEW:
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

  **Solution-Specific Resources (Nebula CRM):**
  - `planning-mds/INCEPTION.md` - Nebula CRM master specification
  - `planning-mds/domain/insurance-glossary.md` - Insurance domain terminology
  - `planning-mds/domain/crm-competitive-analysis.md` - CRM competitive landscape
  - `planning-mds/examples/personas/nebula-personas.md` - Nebula-specific personas (Sarah Chen, Marcus, Jennifer)
  - `planning-mds/examples/features/nebula-features.md` - Nebula features (F1-F6: Broker Mgmt, Account 360, etc.)
  - `planning-mds/examples/stories/nebula-stories.md` - Nebula story examples (S1: Create Broker, etc.)
  - `planning-mds/examples/screens/nebula-screens.md` - Nebula screen specs (Broker List, Broker 360, etc.)

  ---
  Step 4.2: Update Product Manager README.md

  File: agents/product-manager/README.md

  Find section "Key Resources" (around line 35-45):

  OLD:
  ### Key Resources

  | Resource | Purpose |
  |----------|---------|
  | `SKILL.md` | Complete agent specification |
  | `references/pm-best-practices.md` | INVEST criteria, slicing strategies |
  | `references/insurance-domain-glossary.md` | Insurance terminology |
  | `references/crm-competitive-analysis.md` | CRM baseline features |
  | `templates/story-template.md` | User story format |

  NEW:
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
  | `../../planning-mds/INCEPTION.md` | Nebula CRM master specification |
  | `../../planning-mds/domain/insurance-glossary.md` | Insurance domain terms (broker, MGA, submission, etc.) |
  | `../../planning-mds/domain/crm-competitive-analysis.md` | CRM competitive landscape (Salesforce, Applied Epic, etc.) |
  | `../../planning-mds/examples/personas/` | Nebula personas (Sarah Chen, Marcus, Jennifer) |
  | `../../planning-mds/examples/features/` | Nebula features (F1-F6) |
  | `../../planning-mds/examples/stories/` | Nebula stories (S1-S7) |
  | `../../planning-mds/examples/screens/` | Nebula screen specs |

  ---
  Step 4.3: Update Architect SKILL.md

  File: agents/architect/SKILL.md

  Find section "Required Resources" (around line 125-130):

  OLD:
  **Required Resources:**
  - `planning-mds/INCEPTION.md` - Master specification (sections 3.x from PM)
  - `agents/templates/api-contract-template.yaml` - OpenAPI template
  - `agents/templates/entity-model-template.md` - Entity specification format
  - `agents/templates/adr-template.md` - Architecture Decision Record format
  - `agents/architect/references/` - Architecture best practices

  NEW:
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

  **Solution-Specific Resources (Nebula CRM):**
  - `planning-mds/INCEPTION.md` - Nebula CRM master specification (sections 3.x from PM)
  - `planning-mds/domain/crm-architecture-patterns.md` - Insurance CRM architectural patterns (broker hierarchy, submission workflow, renewal pipeline)
  - `planning-mds/examples/architecture/nebula-architecture.md` - Nebula-specific architecture examples (Broker module, Submission workflow, Account 360)
  - `planning-mds/examples/architecture/adrs/` - Nebula Architecture Decision Records (ADR-001: Modular Monolith, ADR-002: EF Core, ADR-003: Casbin ABAC)

  ---
  Step 4.4: Update Architect README.md

  File: agents/architect/README.md

  Find section "Key Resources" (around line 35-50):

  OLD:
  ### Key Resources

  | Resource | Purpose |
  |----------|---------|
  | `SKILL.md` | Complete agent specification |
  | `references/architecture-best-practices.md` | SOLID, DDD, Clean Architecture |
  | `references/architecture-examples.md` | Real-world examples |
  | `references/api-design-guide.md` | REST API patterns |
  | `templates/adr-template.md` | ADR format |

  NEW:
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
  | `../../planning-mds/INCEPTION.md` | Nebula CRM master specification |
  | `../../planning-mds/domain/crm-architecture-patterns.md` | Insurance CRM patterns (broker hierarchy, submission workflow, renewal pipeline, commission
  tracking) |
  | `../../planning-mds/examples/architecture/nebula-architecture.md` | Nebula architecture examples (Broker, Submission, Account 360, complete ADRs) |

  ---
  PHASE 5: Update INCEPTION.md References

  Step 5.1: Add Domain Knowledge Section to INCEPTION.md

  File: planning-mds/INCEPTION.md

  Find section "## 2. Product Context" (around line 50-100):

  Add this new section AFTER "## 2. Product Context" heading:

  ### 2.0 Reference Documentation

  This section provides links to domain knowledge and examples specific to Nebula CRM.

  **Domain Knowledge:**
  - **Insurance Glossary:** See `domain/insurance-glossary.md` for insurance industry terminology (broker, MGA, submission, binder, renewal, etc.)
  - **CRM Competitive Analysis:** See `domain/crm-competitive-analysis.md` for CRM landscape (Salesforce, Applied Epic, Vertafore, Duck Creek)
  - **CRM Architecture Patterns:** See `domain/crm-architecture-patterns.md` for insurance CRM-specific architectural patterns

  **Examples Library:**

  *Product Examples:*
  - **Personas:** See `examples/personas/nebula-personas.md` for Nebula user personas (Sarah Chen - Distribution Manager, Marcus - Underwriter, Jennifer -
  Coordinator)
  - **Features:** See `examples/features/nebula-features.md` for Nebula feature examples (F1: Broker Management, F2: Account 360, F3: Submission Intake,
  etc.)
  - **Stories:** See `examples/stories/nebula-stories.md` for Nebula story examples (S1: Create New Broker, etc.)
  - **Screens:** See `examples/screens/nebula-screens.md` for Nebula screen specifications (Broker List, Broker 360, Create Broker)

  *Architecture Examples:*
  - **Modules:** See `examples/architecture/nebula-architecture.md` for complete module examples (Broker, Submission, Account 360)
  - **ADRs:** See `examples/architecture/adrs/` for Architecture Decision Records (Modular Monolith, EF Core, Casbin ABAC)

  **Generic Resources:**

  For generic best practices and templates (applicable to any solution, not just Nebula):
  - See `../agents/product-manager/references/` for PM best practices, generic persona/feature/story examples
  - See `../agents/architect/references/` for architecture best practices, generic architecture examples
  - See `../agents/templates/` for all templates (story, feature, persona, ADR, API contract, etc.)

  ---

  ---
  PHASE 6: Update AGENT-STATUS.md

  Step 6.1: Update Product Manager Section

  File: agents/AGENT-STATUS.md

  Find Product Manager section (around line 24-43):

  OLD:
  #### 1. 🚧 Product Manager
  - **Status:** IN PROGRESS
  - **Artifacts:**
    - SKILL.md ✅
    - README.md ✅
    - references/ (9 files) ✅
      - pm-best-practices.md
      - crm-competitive-analysis.md
      - inception-requirements.md
      - vertical-slicing-guide.md
      - insurance-domain-glossary.md
      - story-examples.md
      - feature-examples.md ⭐ *renamed from epic-examples.md, updated to Feature terminology*
      - persona-examples.md
      - screen-spec-examples.md
    - scripts/ (2 files) ⚠️
      - validate-stories.py
      - generate-story-index.py
  - **Pending:** Script validation, workflow testing
  - **Notes:** Most comprehensive reference set; competitive analysis provides CRM baseline

  NEW:
  #### 1. ✅ Product Manager
  - **Status:** COMPLETE (95%) - Generic/Solution Separation Complete
  - **Artifacts:**
    - SKILL.md ✅ (updated with generic/solution resource separation)
    - README.md ✅ (updated with generic/solution resource separation)
    - references/ (6 generic files) ✅
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
    - insurance-domain-glossary.md → planning-mds/domain/insurance-glossary.md
    - crm-competitive-analysis.md → planning-mds/domain/crm-competitive-analysis.md
    - Nebula persona examples → planning-mds/examples/personas/nebula-personas.md
    - Nebula feature examples → planning-mds/examples/features/nebula-features.md
    - Nebula story examples → planning-mds/examples/stories/nebula-stories.md
    - Nebula screen specs → planning-mds/examples/screens/nebula-screens.md
  - **Completed:** 2026-01-31 - Generic agent role complete, ready for reuse in other projects
  - **Notes:** Agent is now 100% generic and reusable. All Nebula-specific content in planning-mds/

  ---
  Step 6.2: Update Architect Section

  File: agents/AGENT-STATUS.md

  Already updated in previous task, but add note about solution separation:

  Find Architect section and ADD this note at the end:

  - **Solution-Specific Content Moved to planning-mds/:**
    - insurance-crm-architecture-patterns.md → planning-mds/domain/crm-architecture-patterns.md
    - Nebula architecture examples → planning-mds/examples/architecture/nebula-architecture.md
  - **Notes:** Agent is now 100% generic and reusable. All Nebula-specific content in planning-mds/. Reference depth: 5,440 lines (generic patterns only).

  ---
  PHASE 7: Create planning-mds/README.md Documentation

  Step 7.1: Create README for planning-mds/

  File: planning-mds/README.md

  Content:
  # Nebula CRM Planning Documents

  This directory contains all solution-specific planning documents, domain knowledge, and examples for the Nebula Insurance CRM project.

  ---

  ## Directory Structure

  planning-mds/
  ├── INCEPTION.md                     # Master specification (single source of truth)
  │
  ├── domain/                          # Domain knowledge (insurance & CRM)
  │   ├── insurance-glossary.md        # Insurance terminology (broker, MGA, submission, binder, etc.)
  │   ├── crm-competitive-analysis.md  # CRM competitive landscape (Salesforce, Applied Epic, etc.)
  │   └── crm-architecture-patterns.md # Insurance CRM architectural patterns
  │
  ├── examples/                        # Nebula-specific examples
  │   ├── personas/
  │   │   └── nebula-personas.md       # Sarah Chen, Marcus, Jennifer personas
  │   ├── features/
  │   │   └── nebula-features.md       # F1-F6 feature examples
  │   ├── stories/
  │   │   └── nebula-stories.md        # S1-S7 story examples
  │   ├── screens/
  │   │   └── nebula-screens.md        # Broker List, Broker 360, Create Broker screens
  │   └── architecture/
  │       ├── nebula-architecture.md   # Broker module, Submission workflow, Account 360
  │       └── adrs/
  │           ├── ADR-001-modular-monolith.md
  │           ├── ADR-002-ef-core.md
  │           └── ADR-003-casbin-abac.md
  │
  ├── features/                        # Actual Nebula features (F1-F6)
  ├── stories/                         # Actual Nebula user stories (organized by feature)
  ├── architecture/                    # Architecture decisions and specs
  │   └── decisions/                   # ADRs (when written)
  ├── api/                             # API contracts
  │   └── nebula-api.yaml
  ├── security/                        # Security artifacts
  └── workflows/                       # Workflow specifications

  ---

  ## What's in This Directory

  ### Domain Knowledge

  **Purpose:** Insurance and CRM domain-specific knowledge that Product Managers and Architects need to understand Nebula.

  - **insurance-glossary.md:** Defines insurance terminology (broker, MGA, submission, binder, renewal, etc.)
  - **crm-competitive-analysis.md:** CRM landscape analysis (Salesforce, Applied Epic, Vertafore, Duck Creek)
  - **crm-architecture-patterns.md:** Insurance CRM architectural patterns (broker hierarchy, submission workflow, renewal pipeline, commission tracking)

  **When to use:** Reference during Phase A (Product Manager) and Phase B (Architect) to understand domain context.

  ---

  ### Examples Library

  **Purpose:** Real Nebula CRM examples showing how to apply generic templates to this specific solution.

  **Product Examples:**
  - **personas/:** Nebula user personas (Sarah Chen - Distribution Manager, Marcus - Underwriter, Jennifer - Coordinator)
  - **features/:** Nebula features (F1: Broker Management, F2: Account 360, F3: Submission Intake, F4: Renewal Pipeline, F5: Task Center, F6: Broker
  Insights)
  - **stories/:** Nebula user stories (S1: Create New Broker, S2: Update Broker, etc.)
  - **screens/:** Nebula screen specifications (Broker List, Broker 360, Create Broker)

  **Architecture Examples:**
  - **architecture/nebula-architecture.md:** Complete architecture examples for Nebula (Broker module, Submission workflow, Account 360 view)
  - **architecture/adrs/:** Architecture Decision Records explaining key technical choices (Modular Monolith vs Microservices, EF Core as ORM, Casbin for
  ABAC)

  **When to use:** Reference when writing actual features, stories, and architecture for Nebula. Use as templates showing "here's how the generic template
  was applied to our solution."

  ---

  ## Relationship to agents/

  **agents/ = Generic (reusable across projects)**
  - Contains agent role definitions (SKILL.md, README.md)
  - Contains generic best practices (pm-best-practices.md, architecture-best-practices.md)
  - Contains generic examples (B2B SaaS, e-commerce, healthcare examples)
  - Contains generic templates (story-template, feature-template, ADR-template)
  - **Can be copied wholesale to a new project** (underwriting workbench, claims system, policy admin)

  **planning-mds/ = Solution-Specific (Nebula CRM only)**
  - Contains Nebula CRM domain knowledge (insurance glossary, CRM competitive analysis)
  - Contains Nebula-specific examples (Sarah Chen persona, Broker Management feature, Create Broker story)
  - Contains actual Nebula requirements (INCEPTION.md, features/, stories/, architecture/)
  - **Unique to Nebula CRM** - would be replaced for underwriting workbench or claims system

  ---

  ## How to Use

  **For Product Managers:**
  1. Read `domain/insurance-glossary.md` to understand insurance terminology
  2. Read `domain/crm-competitive-analysis.md` to understand CRM landscape
  3. Reference `examples/personas/`, `examples/features/`, `examples/stories/` to see how to write Nebula specs
  4. Use `../agents/templates/` for generic templates
  5. Write actual features/stories in `features/` and `stories/` directories

  **For Architects:**
  1. Read `domain/crm-architecture-patterns.md` to understand insurance CRM patterns
  2. Reference `examples/architecture/nebula-architecture.md` to see complete module examples
  3. Use `../agents/templates/adr-template.md` for ADR structure
  4. Write actual architecture decisions in `architecture/decisions/`
  5. Write actual API contracts in `api/`

  ---

  ## Version History

  **Version 1.0** - 2026-01-31 - Initial planning-mds structure with domain knowledge and examples separated from generic agents

  ---
  PHASE 8: Create planning-mds/domain/README.md

  Step 8.1: Create README for Domain Directory

  File: planning-mds/domain/README.md

  Content:
  # Nebula CRM Domain Knowledge

  This directory contains domain-specific knowledge for the Nebula Insurance CRM project.

  ---

  ## Files

  ### insurance-glossary.md

  **Purpose:** Defines insurance industry terminology specific to commercial property & casualty insurance and broker/MGA relationships.

  **Key Terms Covered:**
  - **Core Entities:** Account, Broker, MGA, Program, Submission, Binder, Policy, Renewal
  - **Insurance Process Terms:** Underwriting, Rating, Premium, Commission, Endorsement, Loss Run
  - **CRM-Specific:** Producer, Insured, Effective Date, Expiration Date

  **When to use:** Reference when writing user stories, features, or architecture to ensure correct terminology.

  ---

  ### crm-competitive-analysis.md

  **Purpose:** Provides competitive landscape analysis of CRM systems used in insurance.

  **Competitors Covered:**
  - **General CRMs:** Salesforce (Sales Cloud, Financial Services Cloud), Microsoft Dynamics 365, HubSpot
  - **Insurance-Specific CRMs:** Applied Epic, Vertafore AMS360, Duck Creek Distribution Management

  **Insights:**
  - Table-stakes features (Account/Contact Management, Activity Timeline, Task Center, Search/Filter, Reports, Permissions)
  - Insurance-specific features (Broker/MGA Hierarchy, Submission Workflow, Renewal Pipeline, Policy Tracking, Producer Assignment, Document Management,
  Commission Tracking)
  - UI/UX patterns (360-degree view, pipeline/Kanban boards, business process flow)

  **When to use:** Reference during Phase A to understand competitive baseline and ensure Nebula has feature parity.

  ---

  ### crm-architecture-patterns.md

  **Purpose:** Architectural patterns specific to insurance CRM systems.

  **Patterns Covered:**
  - **Broker/MGA Hierarchy Architecture:** Self-referencing relationships, recursive queries
  - **Multi-Policy Account Architecture:** One-to-many relationships, aggregations
  - **Submission Workflow Architecture:** State machines, validation gates, side effects
  - **Renewal Pipeline Architecture:** Temporal scheduling, automated reminders
  - **Commission Tracking Architecture:** Calculation rules, splits, receivables
  - **Document Management Architecture:** Blob storage, metadata, versioning

  **Anti-Patterns:**
  - Mutable audit trails, missing authorization checks, N+1 query problems, god entities, missing indexes, ignoring soft delete, hard-coding business rules,
   missing state transition validation

  **When to use:** Reference during Phase B (Architect) to understand CRM-specific architectural challenges and solutions.

  ---

  ## Version History

  **Version 1.0** - 2026-01-31 - Initial domain knowledge directory

  ---
  PHASE 9: Validation & Testing

  Step 9.1: Verify File Movements

  Run:
  # Verify files were moved correctly
  ls -la planning-mds/domain/
  ls -la planning-mds/examples/personas/
  ls -la planning-mds/examples/features/
  ls -la planning-mds/examples/stories/
  ls -la planning-mds/examples/screens/
  ls -la planning-mds/examples/architecture/

  # Verify old files are gone from agents/
  ls -la agents/product-manager/references/ | grep -E "(insurance-domain|crm-competitive|persona-examples|feature-examples|story-examples|screen-spec)"
  ls -la agents/architect/references/ | grep -E "(insurance-crm-architecture|architecture-examples)"

  Expected: No output from second set of commands (files should be gone from agents/).

  ---
  Step 9.2: Verify Generic Examples Were Created

  Run:
  # Verify generic examples exist
  cat agents/product-manager/references/persona-examples.md | head -20
  cat agents/product-manager/references/feature-examples.md | head -20
  cat agents/product-manager/references/story-examples.md | head -20
  cat agents/architect/references/architecture-examples.md | head -20

  Expected: Should show generic examples (B2B SaaS, e-commerce, healthcare, etc.), NOT Nebula-specific content (Broker, Sarah Chen, etc.).

  ---
  Step 9.3: Verify SKILL.md and README.md References Updated

  Run:
  # Check Product Manager SKILL.md references
  grep -n "Generic Resources\|Solution-Specific Resources" agents/product-manager/SKILL.md

  # Check Architect SKILL.md references
  grep -n "Generic Resources\|Solution-Specific Resources" agents/architect/SKILL.md

  # Check Product Manager README.md references
  grep -n "Generic Resources\|Solution-Specific Resources" agents/product-manager/README.md

  # Check Architect README.md references
  grep -n "Generic Resources\|Solution-Specific Resources" agents/architect/README.md

  Expected: Should find both "Generic Resources" and "Solution-Specific Resources" sections in all 4 files.

  ---
  Step 9.4: Verify INCEPTION.md Updated

  Run:
  # Check INCEPTION.md has reference documentation section
  grep -n "### 2.0 Reference Documentation" planning-mds/INCEPTION.md
  grep -n "Domain Knowledge:" planning-mds/INCEPTION.md
  grep -n "Examples Library:" planning-mds/INCEPTION.md

  Expected: Should find new section with links to domain/ and examples/.

  ---
  Step 9.5: Test Reusability - Simulate New Project

  Run:
  # Simulate creating a new project (underwriting workbench)
  cd ..
  mkdir -p underwriting-workbench
  cp -r insurance-crm/agents underwriting-workbench/

  # Verify generic agents copied successfully
  ls -la underwriting-workbench/agents/product-manager/references/
  ls -la underwriting-workbench/agents/architect/references/

  # Verify NO Nebula-specific content in copied agents/
  grep -r "Broker\|Submission\|Sarah Chen\|Nebula" underwriting-workbench/agents/product-manager/references/persona-examples.md
  grep -r "Broker\|Submission\|Account" underwriting-workbench/agents/architect/references/architecture-examples.md

  Expected:
  - Files should be copied successfully
  - No grep matches for Nebula-specific terms (agents/ should be 100% generic)

  ---
  PHASE 10: Final Documentation Update

  Step 10.1: Update Root README.md (if exists)

  File: README.md (in root of insurance-crm)

  If README.md exists, add this section at the top:

  ## Repository Structure

  This repository follows a clear separation between **generic agent roles** and **solution-specific content**:

  ### agents/ - Generic (Reusable)

  The `agents/` directory contains generic agent role definitions and best practices that can be reused across **any** insurance software project
  (underwriting workbench, claims system, policy admin, etc.).

  **To start a new project:** Simply copy the entire `agents/` directory to your new project.

  **Contents:**
  - Agent role definitions (SKILL.md, README.md for each agent)
  - Generic best practices (SOLID, DDD, INVEST, vertical slicing, etc.)
  - Generic examples (B2B SaaS, e-commerce, healthcare examples)
  - Generic templates (story, feature, persona, ADR, API contract, etc.)

  ### planning-mds/ - Solution-Specific (Nebula CRM)

  The `planning-mds/` directory contains **Nebula CRM-specific** content:

  **Contents:**
  - `domain/` - Insurance & CRM domain knowledge (glossary, competitive analysis, architectural patterns)
  - `examples/` - Nebula-specific examples (Sarah Chen persona, Broker Management feature, Create Broker story, Broker module architecture)
  - `features/`, `stories/`, `architecture/` - Actual Nebula requirements and specifications
  - `INCEPTION.md` - Nebula CRM master specification

  **For a new project:** Replace `planning-mds/` with project-specific domain knowledge and examples.

  ---

  ---
  Step 10.2: Create agents/README.md

  File: agents/README.md

  Content:
  # Generic Agent Roles

  This directory contains **generic, reusable** agent role definitions and best practices for building software with AI agents.

  ## ✅ 100% Generic - Ready for Reuse

  All content in this directory is **domain-agnostic** and can be applied to any software project:
  - ✅ Insurance CRM (Nebula)
  - ✅ Underwriting Workbench
  - ✅ Claims Management System
  - ✅ Policy Administration System
  - ✅ E-commerce Platform
  - ✅ Healthcare SaaS
  - ✅ B2B SaaS
  - ✅ Any other software project

  ## Directory Structure

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
  │   └── ... (all templates are generic)
  │
  ├── README.md                         # This file
  └── ROLES.md                          # Agent roles quick reference

  ## How to Use for a New Project

  ### Step 1: Copy agents/ Directory

  ```bash
  # Example: Creating an Underwriting Workbench
  cp -r insurance-crm/agents underwriting-workbench/agents

  Result: You now have all agent roles, best practices, and templates ready to use.

  ---
  Step 2: Create Solution-Specific planning-mds/

  mkdir -p underwriting-workbench/planning-mds/{domain,examples,features,stories,architecture}

  ---
  Step 3: Create Domain Knowledge

  Create files in planning-mds/domain/:

  underwriting-workbench/planning-mds/domain/underwriting-glossary.md:
  - Define underwriting-specific terminology (risk assessment, binding authority, loss ratio, etc.)

  underwriting-workbench/planning-mds/domain/underwriting-competitive-analysis.md:
  - Analyze competitive landscape (Guidewire, Duck Creek, Insurity, etc.)

  underwriting-workbench/planning-mds/domain/underwriting-architecture-patterns.md:
  - Document underwriting-specific patterns (risk assessment workflow, pricing engine integration, etc.)

  ---
  Step 4: Create Solution-Specific Examples

  Create files in planning-mds/examples/:

  personas/:
  - Create personas for your solution (e.g., "Emma the Underwriter", "David the Risk Analyst")

  features/:
  - Create feature examples for your solution (e.g., "F1: Risk Assessment", "F2: Pricing Engine")

  stories/:
  - Create story examples for your solution (e.g., "S1: Assess Property Risk")

  architecture/:
  - Create architecture examples for your solution (e.g., "Risk Assessment Module", "Pricing Calculation Workflow")

  ---
  Step 5: Create INCEPTION.md

  underwriting-workbench/planning-mds/INCEPTION.md:
  - Use ../agents/templates/ as structure reference
  - Reference your domain knowledge in Section 2.0
  - Define your vision, personas, features, stories

  ---
  Step 6: Start Building

  You're now ready to execute:
  - Phase A: Use Product Manager agent to define requirements
  - Phase B: Use Architect agent to design technical architecture
  - Phase C: Use implementation agents to build

  All generic best practices, templates, and patterns are ready to use from agents/.

  ---
  What's NOT in This Directory

  Solution-specific content does NOT belong here:
  - ❌ Domain-specific terminology (insurance, underwriting, claims, etc.)
  - ❌ Competitive analysis for specific markets
  - ❌ Solution-specific examples (personas, features, stories referencing your domain entities)
  - ❌ Solution-specific architecture examples (modules/workflows specific to your domain)

  That content belongs in planning-mds/ (or equivalent) for each solution.

  ---
  Version History

  Version 2.0 - 2026-01-31 - Separated generic agents from solution-specific content (Nebula CRM)
  Version 1.0 - 2026-01-26 - Initial agent roles created

  ---

  ## PHASE 11: Final Commit

  ### Step 11.1: Commit All Changes

  ```bash
  cd C:\Users\gajap\sandbox\insurance-crm

  # Stage all changes
  git add -A

  # Commit with detailed message
  git commit -m "$(cat <<'EOF'
  refactor: Separate generic agent roles from Nebula CRM-specific content

  BREAKING CHANGE: Repository structure refactored to enable agent reusability

  ## Changes Made

  ### File Movements (agents/ → planning-mds/)

  **Product Manager:**
  - Moved insurance-domain-glossary.md → planning-mds/domain/insurance-glossary.md
  - Moved crm-competitive-analysis.md → planning-mds/domain/crm-competitive-analysis.md
  - Moved persona-examples.md → planning-mds/examples/personas/nebula-personas.md
  - Moved feature-examples.md → planning-mds/examples/features/nebula-features.md
  - Moved story-examples.md → planning-mds/examples/stories/nebula-stories.md
  - Moved screen-spec-examples.md → planning-mds/examples/screens/nebula-screens.md

  **Architect:**
  - Moved insurance-crm-architecture-patterns.md → planning-mds/domain/crm-architecture-patterns.md
  - Moved architecture-examples.md → planning-mds/examples/architecture/nebula-architecture.md

  ### New Generic Examples Created

  **Product Manager:**
  - Created generic persona-examples.md (B2B SaaS, e-commerce, healthcare)
  - Created generic feature-examples.md (task mgmt, e-commerce, scheduling)
  - Created generic story-examples.md (task creation, add to cart, appointment scheduling)

  **Architect:**
  - Created generic architecture-examples.md (e-commerce order mgmt, CMS, SaaS subscriptions)

  ### Documentation Updates

  - Updated agents/product-manager/SKILL.md with generic/solution resource separation
  - Updated agents/product-manager/README.md with generic/solution resource separation
  - Updated agents/architect/SKILL.md with generic/solution resource separation
  - Updated agents/architect/README.md with generic/solution resource separation
  - Updated planning-mds/INCEPTION.md with reference documentation section
  - Updated agents/AGENT-STATUS.md to reflect refactoring completion
  - Created planning-mds/README.md documenting structure
  - Created planning-mds/domain/README.md documenting domain knowledge
  - Created agents/README.md documenting generic agent reusability

  ## Result

  - ✅ agents/ is now 100% generic and reusable across any project
  - ✅ planning-mds/ contains all Nebula CRM-specific content
  - ✅ Clear boundary established between generic and solution-specific
  - ✅ Ready to copy agents/ to new projects (underwriting workbench, claims system, etc.)

  Co-Authored-By: Claude Opus 4.5 <noreply@anthropic.com>
  EOF
  )"

  ---
  SUCCESS CRITERIA

  After executing this prompt, verify:

  1. ✅ No Nebula-specific content in agents/:
    - No references to Broker, Submission, Account, Renewal entities
    - No references to Sarah Chen, Marcus, Jennifer personas
    - No references to F1-F6 features
    - All examples are generic (B2B SaaS, e-commerce, healthcare, etc.)
  2. ✅ All Nebula-specific content in planning-mds/:
    - Domain knowledge in planning-mds/domain/
    - Examples in planning-mds/examples/
    - Actual requirements in planning-mds/{features,stories,architecture}/
  3. ✅ References updated:
    - SKILL.md and README.md files reference both generic and solution-specific resources
    - INCEPTION.md has reference documentation section
    - All paths are correct and files can be found
  4. ✅ Reusability test passes:
    - Can copy agents/ to a new project directory
    - No Nebula-specific content in copied agents/
    - New project can create its own planning-mds/ with different domain knowledge

  ---
