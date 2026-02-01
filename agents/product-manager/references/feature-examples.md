# Feature Examples

**Note:** Use these examples when defining features for the insurance CRM. Features should be stored in `planning-mds/features/` directory.

---

## Feature F1: Broker & MGA Relationship Management

**Feature Statement:**
As a Distribution user, I want to manage broker relationships so that I can grow premium and track broker performance.

**Business Objective:**
Centralize broker data, activity history, and relationship management to improve broker engagement and submission quality.

**Success Criteria:**
- Reduce time to find broker information from 5 minutes to 30 seconds
- Increase broker submission quality score by 20%
- Enable tracking of all broker interactions in one place

**In Scope:**
- Broker CRUD operations
- Broker 360 view (all broker data in one place)
- Broker hierarchy management (parent/child relationships)
- Broker contact management
- Broker activity timeline

**Out of Scope:**
- External broker self-service portal (Phase 1)
- Broker performance analytics/dashboards (Phase 1)
- Automated broker onboarding workflows (Future)

**Related Stories:**
- S1: Create Broker
- S2: Read Broker (360 View)
- S3: Update Broker
- S4: Delete Broker
- S5: Manage Broker Hierarchy
- S6: Manage Broker Contacts
- S7: View Broker Activity Timeline

---

## Feature F2: Account 360 & Activity Timeline

**Feature Statement:**
As an Underwriter, I want to see all account information and activity in one place so that I can make informed underwriting decisions.

**Business Objective:**
Provide complete account context including submissions, renewals, communications, and documents to improve underwriting efficiency and quality.

**Success Criteria:**
- Reduce time to gather account information from 10 minutes to 1 minute
- Improve underwriter decision quality (measured by quote-to-bind ratio)
- Enable complete audit trail of all account interactions

**In Scope:**
- Account overview dashboard
- Activity timeline (all events chronologically)
- Related submissions and renewals
- Document repository
- Communication history

**Out of Scope:**
- Predictive analytics (Future)
- Automated risk assessment (Future)
- Integration with third-party data providers (Phase 1)

**Related Stories:**
- S8: View Account Overview
- S9: View Activity Timeline
- S10: View Related Submissions
- S11: Access Account Documents

---

## Feature F3: Submission Intake Workflow

**Feature Statement:**
As a Distribution user, I want to efficiently intake and triage new submissions so that they reach the right underwriter quickly.

**Business Objective:**
Streamline submission intake process to reduce time-to-quote and improve submission quality through standardized data collection.

**Success Criteria:**
- Reduce submission intake time from 15 minutes to 5 minutes
- Reduce incomplete submissions by 40%
- Route 95% of submissions to correct underwriter automatically

**In Scope:**
- Submission creation form
- Document upload
- Automated routing rules
- Status tracking
- Underwriter assignment

**Out of Scope:**
- Pre-fill from external sources (Future)
- AI-powered risk assessment (Future)
- Automated pricing (Phase 1)

**Related Stories:**
- S12: Create Submission
- S13: Upload Submission Documents
- S14: Route Submission to Underwriter
- S15: Track Submission Status

---

## Best Practices for Feature Definitions

1. **Clear Business Value:** Every feature should have measurable business objectives and success criteria
2. **Appropriate Scope:** Features should be decomposable into 5-10 user stories
3. **User-Centric:** Written from user perspective, not technical implementation
4. **Bounded:** Clear in-scope and out-of-scope items to prevent scope creep
5. **Traceable:** Link to related user stories that implement the feature
