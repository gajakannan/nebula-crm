# Insurance Domain Glossary

Essential insurance terminology for BrokerHub product specifications.

## Purpose

This glossary ensures consistent use of insurance terms in product requirements and helps the PM agent understand domain concepts without inventing definitions.

**Rule:** If a term is not in this glossary and you're unsure of its meaning, ASK rather than assume.

---

## Core Entities

### Account
**Definition:** A business entity (insured) seeking or holding insurance coverage
**Also Known As:** Insured, Policyholder (when bound), Prospect (when shopping)
**Example:** "ABC Manufacturing Company is an account seeking general liability coverage"
**In BrokerHub:** Central entity linking submissions, renewals, and broker relationships

### Broker
**Definition:** Licensed insurance intermediary who represents insureds and places business with carriers
**Types:**
- **Retail Broker:** Works directly with insureds
- **Wholesale Broker:** Works with retail brokers, specializes in hard-to-place risk
**Example:** "Jones Insurance Brokers represents 50+ commercial accounts"
**In BrokerHub:** Primary relationship management entity

### MGA (Managing General Agent)
**Definition:** Insurance intermediary with underwriting authority delegated by carriers
**Responsibilities:** Can quote, bind, and issue policies on behalf of carriers
**Example:** "Coastal MGA Program specializes in coastal property risks"
**In BrokerHub:** Partners with brokers to place specialty business

### Program
**Definition:** A specialized insurance product designed for a specific industry or risk type
**Managed By:** MGAs or program managers
**Example:** "Restaurant Liability Program" or "Cyber Insurance Program"
**In BrokerHub:** Associates with MGAs and defines available coverages

---

## Insurance Process Terms

### Submission
**Definition:** A request for insurance coverage presented to a carrier or MGA
**Contains:** Application, exposure information, loss history
**Outcome:** Quoted, Declined, or Withdrawn
**Example:** "Broker submitted a general liability submission for ABC Corp"
**In BrokerHub:** Core workflow entity tracking quote requests

### Quote
**Definition:** A carrier's offer to provide insurance coverage at specified terms and pricing
**Components:** Coverage limits, premium, deductible, terms & conditions
**Status:** May be accepted (bound) or rejected by insured
**Example:** "$5,000 annual premium for $1M/$2M general liability"
**In BrokerHub:** Output of successful submission underwriting

### Bind
**Definition:** The act of accepting a quote and committing to the insurance contract
**Trigger:** Insured accepts terms and provides payment (or payment agreement)
**Result:** Policy is issued
**Example:** "ABC Corp bound the quoted policy on 2024-01-15"
**In BrokerHub:** Final successful state of submission workflow

### Policy
**Definition:** The legal contract of insurance coverage
**Effective Dates:** Policy period (typically 12 months)
**Deliverable:** Physical or electronic policy document
**Example:** "Policy #POL-2024-12345, effective 2024-02-01 to 2025-02-01"
**In BrokerHub:** Not managed in Phase 0 (focus is pre-bind workflows)

### Renewal
**Definition:** The process of continuing insurance coverage for another term
**Timing:** Typically starts 90-120 days before expiration
**Outcome:** Renewed (bound), Lost (to competitor), or Lapsed (not renewed)
**Example:** "ABC Corp's policy renews on 2025-02-01"
**In BrokerHub:** Core workflow for retention management

---

## Coverage & Risk Terms

### Premium
**Definition:** The price paid for insurance coverage
**Components:** Base rate × exposure × modifiers
**Types:** Annual premium, installment premium
**Example:** "$5,000 annual premium, paid quarterly"
**In BrokerHub:** Key metric tracked per submission and renewal

### Exposure
**Definition:** The unit of measure for risk pricing
**Examples:**
- General Liability: Payroll, revenue, square footage
- Commercial Auto: Number of vehicles
- Workers Comp: Payroll by class code
**In BrokerHub:** Captured in submission details

### Line of Business (LOB)
**Definition:** Category of insurance coverage
**Common LOBs:**
- **General Liability (GL):** Bodily injury and property damage
- **Commercial Property:** Buildings and contents
- **Workers Compensation (WC):** Employee injury coverage
- **Commercial Auto:** Vehicle coverage
- **Professional Liability (E&O):** Errors & Omissions
- **Cyber:** Data breach and cyber risk
**In BrokerHub:** Used to categorize submissions and programs

### Coverage Limit
**Definition:** Maximum amount an insurer will pay for a covered loss
**Format:** Often expressed as occurrence/aggregate (e.g., $1M/$2M)
**Example:** "$1M per occurrence, $2M aggregate" for general liability
**In BrokerHub:** Specified in quote terms

### Deductible
**Definition:** Amount insured must pay before insurance coverage applies
**Types:** Per occurrence, annual aggregate
**Example:** "$5,000 per occurrence deductible"
**In BrokerHub:** Part of quote terms

---

## Market & Regulatory Terms

### Surplus Lines
**Definition:** Insurance placed with non-admitted carriers for hard-to-place risks
**Also Known As:** Excess & Surplus (E&S)
**Regulation:** State-specific rules, often requires diligent search
**Example:** "High-risk restaurant placed in surplus lines market"
**In BrokerHub:** Primary market focus for Phase 0

### Non-Admitted Carrier
**Definition:** Insurance company not licensed in a state but allowed to write surplus lines
**Advantage:** More flexibility in rates and terms
**Requirement:** Broker must document that admitted market was insufficient
**Example:** "Lloyd's of London writing surplus lines coverage"

### Admitted Carrier
**Definition:** Insurance company licensed and regulated in a state
**Advantages:** State guaranty fund protection, standard rates
**Example:** "Hartford writing admitted general liability"
**In BrokerHub:** May expand to admitted in Phase 1

### Diligent Search
**Definition:** Documented effort to find coverage in admitted market before using surplus lines
**Requirement:** Typically 3 declinations from admitted carriers
**Documentation:** Required for regulatory compliance
**In BrokerHub:** May track in submission workflow (future phase)

---

## Commercial Lines (vs Personal Lines)

### Commercial Lines
**Definition:** Insurance for businesses and organizations
**Examples:** General liability, workers comp, commercial property
**In BrokerHub:** Exclusive focus

### Personal Lines
**Definition:** Insurance for individuals and families
**Examples:** Auto, homeowners, life insurance
**In BrokerHub:** Out of scope

---

## P&C (Property & Casualty)

**Property Insurance:** Coverage for buildings, contents, and property
**Casualty Insurance:** Liability coverage (injury to others, property damage to others)
**Combined:** "Property & Casualty" or "P&C" insurance

**In BrokerHub:** Focus is Commercial P&C, particularly surplus lines

---

## Workflow & Status Terms

### Triaging
**Definition:** Initial review of submission to assess completeness and assignability
**Decision:** Route to underwriter, send back to broker, or decline
**In BrokerHub:** First active state after submission received

### Underwriting
**Definition:** Process of evaluating risk and determining coverage terms
**Performed By:** Underwriter or MGA underwriting team
**Output:** Quote or declination
**In BrokerHub:** "InReview" status during underwriting

### Declination (Decline)
**Definition:** Carrier's decision not to offer coverage
**Reasons:** Risk doesn't meet appetite, insufficient information, unacceptable loss history
**In BrokerHub:** Terminal state for submission workflow

### Withdrawal
**Definition:** Broker or insured cancels submission before quote is bound
**Reasons:** Found coverage elsewhere, decided not to purchase, changed requirements
**In BrokerHub:** Terminal state for submission workflow

### Lapse
**Definition:** Policy expires without renewal
**Reasons:** Insured chose not to renew, found coverage elsewhere
**In BrokerHub:** Terminal state for renewal workflow

---

## Parties & Roles

### Insured
**Definition:** The party covered by the insurance policy
**Also Known As:** Policyholder (when policy is in force)
**Example:** "ABC Manufacturing Company"
**In BrokerHub:** Represented by Account entity

### Carrier
**Definition:** Insurance company that underwrites and issues policies
**Examples:** Lloyd's, AIG, Zurich, specialty surplus lines carriers
**In BrokerHub:** May be tracked per program or submission (future phase)

### Underwriter
**Definition:** Insurance professional who evaluates risk and determines coverage terms
**Employer:** Carrier or MGA
**In BrokerHub:** Internal user persona who reviews submissions

### Distribution & Marketing
**Definition:** Team responsible for broker relationships and business development
**Responsibilities:** Onboard brokers, manage relationships, track production
**In BrokerHub:** Primary user persona

### Relationship Manager
**Definition:** Role focused on managing broker and MGA partnerships
**Responsibilities:** Broker performance, strategic relationships, program development
**In BrokerHub:** User persona

---

## Dates & Timing

### Effective Date
**Definition:** Date coverage begins
**Example:** "Policy effective 2024-02-01"
**In BrokerHub:** Key date tracked for policies and renewals

### Expiration Date
**Definition:** Date coverage ends
**Example:** "Policy expires 2025-02-01"
**In BrokerHub:** Trigger for renewal workflow

### Inception Date
**Definition:** Original start date of first policy (for renewal purposes)
**Example:** "Account inception 2020-02-01, now on 5th renewal"
**In BrokerHub:** Historical tracking (future phase)

### Renewal Date
**Definition:** Date renewal coverage begins (typically = expiration date of expiring policy)
**Example:** "Renewal date 2025-02-01"
**In BrokerHub:** Key date for renewal workflow

---

## Financial Terms

### Commission
**Definition:** Percentage of premium paid to broker for placing business
**Typical Range:** 10-20% for commercial lines
**Example:** "15% commission on $10,000 premium = $1,500"
**In BrokerHub:** May track in future phase for broker performance

### GWP (Gross Written Premium)
**Definition:** Total premium before commissions and other deductions
**Use:** Key metric for production and performance
**Example:** "$500K GWP in Q1"
**In BrokerHub:** May aggregate for broker insights (future phase)

### Installment
**Definition:** Partial premium payment (e.g., quarterly, monthly)
**Example:** "$5,000 annual premium paid in 4 quarterly installments of $1,250"
**In BrokerHub:** Not managed in Phase 0 (carrier billing systems handle this)

---

## Document Types

### Application
**Definition:** Form completed by insured providing exposure and risk information
**Format:** Often PDF or online form
**In BrokerHub:** Attached to submission

### ACORD Form
**Definition:** Standardized insurance industry forms
**Examples:** ACORD 125 (commercial insurance application), ACORD 140 (property section)
**In BrokerHub:** May accept as submission documents

### Binder
**Definition:** Temporary evidence of insurance coverage before policy is issued
**Duration:** Usually 30-90 days
**In BrokerHub:** Generated after bind (future phase)

### Certificate of Insurance (COI)
**Definition:** Document proving insurance coverage exists
**Requested By:** Landlords, general contractors, customers
**In BrokerHub:** Generated from bound policies (future phase)

### Loss Runs
**Definition:** Historical claims report for an insured
**Use:** Underwriting uses to assess risk
**Timeframe:** Typically 5 years
**In BrokerHub:** Attached to submission as supporting document

---

## BrokerHub-Specific Terms

### Activity Timeline Event
**Definition:** Immutable audit log entry recording a system action or status change
**Contains:** Timestamp, user, action type, entity, details
**In BrokerHub:** Core auditability requirement

### Workflow Transition
**Definition:** Immutable log of status change for submission or renewal
**Contains:** From status, to status, timestamp, user, reason
**In BrokerHub:** Append-only table for workflow history

### Broker 360
**Definition:** Comprehensive view of broker relationship including contacts, submissions, renewals, timeline
**In BrokerHub:** Key screen for relationship management

### Task Center
**Definition:** Centralized list of reminders, follow-ups, and pending actions
**In BrokerHub:** Planned for MVP or Phase 1

---

## Terms to AVOID (Ambiguous or Overloaded)

### "Client"
**Problem:** Could mean broker (customer of carrier) or insured (customer of broker)
**Use Instead:** Be specific - "broker" or "insured"

### "Customer"
**Problem:** Ambiguous - is it broker or insured?
**Use Instead:** "broker" or "insured"

### "Producer"
**Problem:** Could mean broker or internal sales rep
**Use Instead:** Be specific based on context

### "Agent"
**Problem:** Could mean broker, MGA, or carrier representative
**Use Instead:** Be specific - "broker" or "MGA"

---

## Questions? Add to Glossary!

If you encounter an insurance term not defined here:
1. **ASK the user** for clarification
2. **Document the answer** in this glossary
3. **Use consistently** throughout all requirements

---

## Version History

**Version 1.0** - 2026-01-26 - Initial glossary
