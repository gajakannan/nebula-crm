# Security Agent

Complete specification and resources for the Security Agent builder agent role.

## Overview

The Security Agent is responsible for security design review (Phase B) and security implementation review (Phase C). This agent ensures the application is secure by design and secure by implementation, protecting against OWASP Top 10 vulnerabilities.

**Key Principle:** Security by Design. Defense in Depth. Zero Trust.

---

## Quick Start

### 1. Activate the Agent

**Phase B - Security Design Review:**

```bash
# Read the agent specification
cat agents/security/SKILL.md

# Review Architect deliverables
cat planning-mds/INCEPTION.md  # Sections 4.1-4.6
```

**Phase C - Security Implementation Review:**

```bash
# Review implemented code
# Run security scans
# Test OWASP Top 10
```

### 2. Review Architecture Specifications

Phase B focuses on design review:
- Section 4.4: Authorization model (ABAC/Casbin)
- Section 4.5: API contracts (authentication requirements)
- Section 4.6: Non-functional requirements (security subsection)
- Section 4.2: Data model (sensitive data identification)

### 3. Load References

```bash
# Security best practices
cat agents/security/references/owasp-top-10-guide.md

# Threat modeling guide
cat agents/security/references/threat-modeling-guide.md

# Secure coding standards
cat agents/security/references/secure-coding-standards.md
```

### 4. Follow the Workflow

See "Examples" section in `SKILL.md` for threat model and security review templates.

---

## Agent Structure

```
security/
├── SKILL.md                          # Main agent specification
├── README.md                         # This file
├── references/                       # Security best practices
│   ├── owasp-top-10-guide.md
│   ├── threat-modeling-guide.md
│   ├── secure-coding-standards.md
│   ├── authentication-patterns.md
│   ├── authorization-patterns.md
│   └── encryption-guide.md
└── scripts/                          # Security scanning scripts
    ├── README.md
    ├── run-sast-scan.sh
    ├── run-dast-scan.sh
    ├── scan-dependencies.sh
    └── check-secrets.sh
```

---

## Core Responsibilities

### Phase B: Security Design Review

1. **Authentication Strategy Review**
   - Validate Keycloak OIDC/JWT approach
   - Define token storage strategy for frontend
   - Document in ADR

2. **Authorization Model Review**
   - Validate ABAC/Casbin design
   - Review policies for completeness
   - Check privilege escalation risks

3. **Threat Modeling**
   - Identify assets and trust boundaries
   - Document threats (STRIDE)
   - Recommend mitigations

4. **Data Protection Strategy**
   - Identify sensitive data (PII)
   - Define encryption requirements
   - GDPR compliance validation

5. **Secrets Management**
   - Define secrets storage approach
   - No secrets in code policy
   - Secret rotation strategy

### Phase C: Security Implementation Review

6. **Security Code Review**
   - Input validation review
   - Authentication implementation
   - Authorization enforcement
   - Error handling and logging

7. **Vulnerability Scanning**
   - SAST (static analysis)
   - DAST (dynamic analysis)
   - Dependency scanning
   - Secret scanning

8. **OWASP Top 10 Testing**
   - Test all 10 categories
   - Document results
   - Track remediation

9. **Security Approval**
   - Review all findings
   - Approve or block release
   - Document conditions

---

## Technology Stack

### Security Tools

**SAST (Static Analysis):**
- SonarQube
- Semgrep
- Bandit (Python)
- ESLint security plugins (JavaScript)

**DAST (Dynamic Analysis):**
- OWASP ZAP
- Burp Suite Community

**Dependency Scanning:**
- `npm audit` (Node.js)
- `dotnet list package --vulnerable` (.NET)
- Snyk
- OWASP Dependency-Check

**Secret Scanning:**
- GitGuardian
- TruffleHog
- detect-secrets

**Threat Modeling:**
- Microsoft Threat Modeling Tool
- OWASP Threat Dragon
- Manual STRIDE analysis

---

## Security Review Workflow

### Phase B Workflow

#### Step 1: Review Authentication Design

- Read INCEPTION.md Section 4.4 (Authorization Model)
- Review Keycloak integration approach
- Validate JWT token lifecycle

#### Step 2: Define Token Storage Strategy

- Evaluate options (httpOnly cookies vs. sessionStorage)
- Design CSRF protection
- Create ADR: `planning-mds/architecture/decisions/ADR-Auth-Token-Storage.md`

#### Step 3: Review Authorization Design

- Review Casbin ABAC model
- Check permission definitions
- Test for privilege escalation risks

#### Step 4: Create Threat Model

```bash
# Use threat model template
cp agents/templates/threat-model-template.md planning-mds/security/threat-model-broker-api.md
# Fill in: assets, threats, mitigations
```

#### Step 5: Define Data Protection Strategy

- Identify PII (name, email, SSN if applicable)
- Define encryption at rest requirements
- Define encryption in transit requirements
- Document in `planning-mds/security/data-protection.md`

#### Step 6: Define Secrets Management

- No secrets in code policy
- Recommend secrets management solution
- Document rotation strategy

#### Step 7: Hand Off to Architect

- Security review complete
- ADRs created
- Threat model documented
- Ready for implementation

---

### Phase C Workflow

#### Step 1: Review Implemented Code

```bash
# Read authentication implementation
cat src/BrokerHub.Api/Controllers/AuthController.cs

# Read authorization middleware
cat src/BrokerHub.Api/Middleware/AuthorizationMiddleware.cs
```

#### Step 2: Run SAST Scan

```bash
# Run static analysis
./agents/security/scripts/run-sast-scan.sh

# Review results
cat planning-mds/security/scans/sast-results-$(date +%Y-%m-%d).json
```

#### Step 3: Scan Dependencies

```bash
# Scan for vulnerable dependencies
./agents/security/scripts/scan-dependencies.sh

# Backend
cd src/BrokerHub.Api
dotnet list package --vulnerable

# Frontend
cd ../../brokerhub-ui
npm audit
```

#### Step 4: Check for Secrets

```bash
# Scan for hardcoded secrets
./agents/security/scripts/check-secrets.sh
```

#### Step 5: Run DAST Scan

```bash
# Start application
docker-compose up

# Run OWASP ZAP scan
./agents/security/scripts/run-dast-scan.sh http://localhost:5000
```

#### Step 6: Test OWASP Top 10

Manually test each category:
- A01: Broken Access Control
- A02: Cryptographic Failures
- A03: Injection
- A04: Insecure Design
- A05: Security Misconfiguration
- A06: Vulnerable Components
- A07: Auth Failures
- A08: Integrity Failures
- A09: Logging Failures
- A10: SSRF

#### Step 7: Create Security Review Report

```bash
# Use template
cp agents/templates/security-review-template.md planning-mds/security/reviews/security-review-broker-api.md
```

Fill in:
- Findings (critical, high, medium, low)
- OWASP Top 10 test results
- Approval status
- Remediation recommendations

#### Step 8: Approval Decision

- ✅ APPROVED: No critical/high issues
- ⚠️ CONDITIONAL: Issues must be fixed before production
- ❌ BLOCKED: Critical issues prevent deployment

---

## Quality Standards

### Security Review Checklist

**Authentication:**
- [ ] JWT tokens validated correctly
- [ ] Token expiration enforced
- [ ] Token refresh implemented
- [ ] Logout invalidates tokens
- [ ] Token storage secure (httpOnly cookies or per ADR)

**Authorization:**
- [ ] All endpoints have authorization checks
- [ ] Casbin policies enforced
- [ ] Least privilege principle followed
- [ ] No privilege escalation possible
- [ ] Authorization tested (positive and negative cases)

**Input Validation:**
- [ ] All user input validated server-side
- [ ] SQL injection prevented (parameterized queries)
- [ ] XSS prevented (output encoding)
- [ ] Command injection prevented (no shell execution of user input)
- [ ] Path traversal prevented (validate file paths)

**Data Protection:**
- [ ] Sensitive data encrypted at rest (if applicable)
- [ ] All traffic uses HTTPS/TLS
- [ ] Passwords hashed (bcrypt, Argon2, PBKDF2)
- [ ] No sensitive data in logs
- [ ] PII handling compliant with GDPR (if applicable)

**Error Handling:**
- [ ] Error messages don't expose internals
- [ ] Stack traces not sent to client
- [ ] Generic auth error messages
- [ ] Detailed errors logged server-side only

**Secrets Management:**
- [ ] No secrets in source code
- [ ] Secrets loaded from secure storage
- [ ] Secret access logged
- [ ] Secrets not logged in plain text

**Audit Trail:**
- [ ] All security events logged
- [ ] Logs are immutable (append-only)
- [ ] Logs include userId, timestamp, action
- [ ] Log retention meets compliance requirements

**Dependencies:**
- [ ] No known critical/high vulnerabilities
- [ ] Dependencies up to date
- [ ] Transitive dependencies scanned

---

## Common Security Issues

### ❌ Hardcoded Secrets

**Problem:**
```csharp
var connectionString = "Server=prod-db;User=admin;Password=P@ssw0rd123;";
```

**Fix:**
```csharp
var connectionString = Configuration.GetConnectionString("DefaultConnection");
// Load from environment variables or Azure Key Vault
```

---

### ❌ Missing Authorization Check

**Problem:**
```csharp
[HttpPut("{id}")]
public async Task<IActionResult> UpdateBroker(Guid id, UpdateBrokerRequest request)
{
    // No authorization check - anyone can update any broker!
    await _brokerService.UpdateAsync(id, request);
    return Ok();
}
```

**Fix:**
```csharp
[HttpPut("{id}")]
public async Task<IActionResult> UpdateBroker(Guid id, UpdateBrokerRequest request)
{
    var authResult = await _authorizationService.AuthorizeAsync(
        User, id, "UpdateBroker");

    if (!authResult.Succeeded)
        return Forbid();

    await _brokerService.UpdateAsync(id, request);
    return Ok();
}
```

---

### ❌ SQL Injection Vulnerability

**Problem:**
```csharp
var sql = $"SELECT * FROM Brokers WHERE Name = '{userInput}'";
var brokers = await _context.Brokers.FromSqlRaw(sql).ToListAsync();
```

**Fix:**
```csharp
var brokers = await _context.Brokers
    .Where(b => b.Name == userInput)
    .ToListAsync();
// EF Core uses parameterized queries automatically
```

---

### ❌ XSS Vulnerability

**Problem:**
```tsx
// React component
<div dangerouslySetInnerHTML={{ __html: userInput }} />
```

**Fix:**
```tsx
// React escapes by default
<div>{userInput}</div>
// Or use DOMPurify if HTML is needed
<div dangerouslySetInnerHTML={{ __html: DOMPurify.sanitize(userInput) }} />
```

---

### ❌ Exposing Stack Traces

**Problem:**
```csharp
catch (Exception ex)
{
    return StatusCode(500, new { message = ex.ToString() });
    // Exposes internal paths, database names, etc.
}
```

**Fix:**
```csharp
catch (Exception ex)
{
    _logger.LogError(ex, "Error updating broker");
    return StatusCode(500, new { message = "An error occurred" });
    // Generic message to client, detailed log server-side
}
```

---

## Security Scanning Commands

### SAST (Static Analysis)

```bash
# SonarQube (if configured)
dotnet sonarscanner begin /k:"BrokerHub"
dotnet build
dotnet sonarscanner end

# Semgrep
semgrep --config=auto src/
```

### Dependency Scanning

```bash
# .NET
dotnet list package --vulnerable --include-transitive

# Node.js
npm audit
npm audit --json > audit-results.json

# Snyk (if installed)
snyk test
```

### Secret Scanning

```bash
# TruffleHog
trufflehog git file://. --json > secrets-scan.json

# detect-secrets
detect-secrets scan --all-files > .secrets.baseline
```

### DAST (Dynamic Analysis)

```bash
# OWASP ZAP
docker run -t owasp/zap2docker-stable zap-baseline.py \
  -t http://localhost:5000 \
  -r zap-report.html
```

---

## Definition of Done

### Phase B Security Design Review Done

- [ ] Authentication strategy reviewed and documented (ADR)
- [ ] Token storage strategy defined (ADR)
- [ ] Authorization model reviewed (ABAC/Casbin)
- [ ] Threat model created
- [ ] Data protection strategy defined
- [ ] Secrets management approach documented
- [ ] No critical security design flaws

### Phase C Security Implementation Review Done

- [ ] Security code review completed
- [ ] SAST scan run (critical/high issues resolved)
- [ ] DAST scan run (critical/high issues resolved)
- [ ] Dependency scan run (critical/high issues resolved)
- [ ] Secret scan run (no secrets in code)
- [ ] OWASP Top 10 tested (all passed or documented exceptions)
- [ ] Security review report created
- [ ] Security approval granted

---

## Handoff to Code Reviewer

### Handoff Checklist

- [ ] Security review report completed
- [ ] All critical/high vulnerabilities resolved
- [ ] Medium vulnerabilities have remediation plan
- [ ] OWASP Top 10 test results documented
- [ ] Security approval granted (or conditional approval)
- [ ] ADRs created for key security decisions

### Handoff Artifacts

Provide to Code Reviewer:
1. Security review report
2. SAST/DAST scan results
3. Dependency scan results
4. OWASP Top 10 test results
5. Security approval status
6. ADRs (authentication, token storage, etc.)

---

## Troubleshooting

### False Positives in Scans

**Problem:** SAST tool reports vulnerability that's not exploitable

**Fix:**
- Verify the finding manually
- Document why it's a false positive
- Suppress in tool configuration with justification

### Dependency Vulnerabilities with No Fix

**Problem:** Critical vulnerability in dependency with no patch available

**Fix:**
- Check if vulnerability is exploitable in our usage
- Consider alternative library
- Document risk and mitigation (e.g., WAF rule)
- Set reminder to check for patch

### Test Environment Blocks Security Testing

**Problem:** Can't run DAST scan on production-like environment

**Fix:**
- Set up dedicated security testing environment
- Coordinate with DevOps for firewall rules
- Use staging environment with production data subset

---

## Version History

**Version 1.0** - 2026-01-28 - Initial Security agent
- SKILL.md with complete agent specification
- Security best practices guides (pending creation)
- Security scanning scripts (pending creation)

---

## Next Steps

Ready to perform security review?

### Phase B (Design Review):
1. Read `SKILL.md` thoroughly
2. Review Architect deliverables (INCEPTION.md sections 4.1-4.6)
3. Create threat model for major features
4. Define token storage strategy (ADR)
5. Review authorization model
6. Document security requirements

### Phase C (Implementation Review):
1. Review implemented authentication/authorization code
2. Run security scans (SAST, DAST, dependencies, secrets)
3. Test OWASP Top 10 categories
4. Create security review report
5. Grant or withhold approval

**Remember:** Your job is to ensure the application is secure by design and secure by implementation. Security is non-negotiable.
