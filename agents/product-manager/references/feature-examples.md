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
- **Out of Scope:** Billing, prescription management, telemedicine (separate features)

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

## For Project-Specific Features

See your project's `planning-mds/examples/features/` directory for feature examples specific to your solution.

---

## Version History

**Version 2.0** - 2026-02-01 - Generic examples (separated from solution-specific content)
**Version 1.0** - Initial version
