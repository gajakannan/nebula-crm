# Security Architecture Guide

Comprehensive security architecture guide for the Nebula insurance CRM. This guide covers security principles, authentication, authorization, data protection, audit & compliance, and OWASP Top 10 mitigation strategies.

---

## 1. Security Principles

### 1.1 Defense in Depth

**Multiple layers of security** so that if one layer is breached, others provide protection.

**Layers in Nebula:**
1. **Network Layer**: HTTPS/TLS 1.3, firewall rules, VPN for internal access
2. **Authentication Layer**: Keycloak OIDC/JWT, MFA enforcement
3. **Authorization Layer**: Casbin ABAC policies enforced on every request
4. **Application Layer**: Input validation, parameterized queries, output encoding
5. **Data Layer**: Database TDE (Transparent Data Encryption), encrypted backups
6. **Audit Layer**: Immutable audit logs, all mutations logged

**No single point of failure** in security.

---

### 1.2 Least Privilege

**Users and services get minimum permissions needed** to perform their tasks, nothing more.

**Implementation:**
- New users start with no permissions (explicit grant required)
- Roles define permissions (Distribution, Underwriter, Admin)
- ABAC policies further restrict based on attributes (assigned underwriter, broker region)
- Service accounts have limited scopes (e.g., renewal workflow can only read policies, not delete)

**Example:**
- Underwriter can **read** all submissions
- Underwriter can **update** only submissions **assigned to them**
- Underwriter **cannot delete** any submission

---

### 1.3 Secure by Default

**Security is the default state**, not an opt-in feature.

**Defaults in Nebula:**
- All API endpoints require authentication (no public endpoints except health check)
- All mutations require authorization (Casbin check on every write)
- HTTPS enforced (HTTP redirects to HTTPS)
- Cookies have Secure and HttpOnly flags
- CORS restricted to known origins only
- Passwords must meet complexity requirements (enforced by Keycloak)

**Never:**
- Open endpoints by default and secure later
- Allow unauthenticated access "temporarily"

---

### 1.4 Fail Securely

**When errors occur, fail in a secure state** (deny access, don't expose data).

**Implementation:**
- If JWT validation fails → return 401, don't process request
- If authorization check fails → return 403, don't show sensitive error details
- If database query fails → return generic error to client, log detailed error server-side
- If encryption fails → reject data, don't store in plaintext

**Example:**
```csharp
try
{
    var allowed = await _enforcer.EnforceAsync(user, resource, action);
    if (!allowed)
        return Results.Forbid(); // Fail securely: deny access
}
catch (Exception ex)
{
    _logger.LogError(ex, "Authorization check failed");
    return Results.Forbid(); // Fail securely: deny on error
}
```

---

### 1.5 Separation of Duties

**No single person or system should have complete control** over critical operations.

**Implementation:**
- Code changes require PR review (developer cannot merge own PR)
- Database migrations require approval (DBA review)
- Production deployments require two approvals
- Admin operations logged and reviewed

---

## 2. Authentication Architecture

### 2.1 Keycloak OIDC Integration

**Keycloak** handles all authentication (login, logout, password management, MFA).

**Flow:**
1. User visits Nebula frontend (https://app.nebula.com)
2. Frontend redirects to Keycloak login page
3. User enters credentials (+ MFA if enabled)
4. Keycloak validates credentials and issues JWT token
5. Frontend stores token (in memory or httpOnly cookie)
6. Frontend includes token in API requests (`Authorization: Bearer <token>`)
7. Nebula API validates token and extracts user claims

**Configuration:**
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://keycloak.example.com/realms/nebula";
        options.Audience = "nebula-api";
        options.RequireHttpsMetadata = true; // HTTPS only

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://keycloak.example.com/realms/nebula",
            ValidateAudience = true,
            ValidAudience = "nebula-api",
            ValidateLifetime = true, // Check exp claim
            ValidateIssuerSigningKey = true, // Verify signature
            ClockSkew = TimeSpan.FromMinutes(5) // Allow 5 min clock drift
        };
    });
```

---

### 2.2 JWT Token Validation

**Validate JWT on Every Request:**

**Checks Performed:**
1. **Signature**: Verify token signed by Keycloak (using public key from JWKS endpoint)
2. **Issuer**: Verify `iss` claim matches Keycloak realm
3. **Audience**: Verify `aud` claim matches nebula-api
4. **Expiration**: Verify `exp` claim is in future (token not expired)
5. **Not Before**: Verify `nbf` claim is in past (token is valid now)

**JWT Structure:**
```json
{
  "iss": "https://keycloak.example.com/realms/nebula",
  "aud": "nebula-api",
  "sub": "550e8400-e29b-41d4-a716-446655440000",
  "exp": 1706749200,
  "nbf": 1706745600,
  "iat": 1706745600,
  "jti": "unique-token-id",
  "email": "sarah.chen@example.com",
  "realm_access": {
    "roles": ["Distribution"]
  }
}
```

---

### 2.3 Refresh Token Flow

**Access tokens are short-lived** (15 minutes), refresh tokens are long-lived (7 days).

**Flow:**
1. User logs in, Keycloak returns access token (15 min) + refresh token (7 days)
2. Frontend uses access token for API requests
3. When access token expires, frontend sends refresh token to Keycloak
4. Keycloak issues new access token (no re-login required)
5. After 7 days, refresh token expires and user must re-login

**Security:**
- Access token stolen → limited damage (expires in 15 min)
- Refresh token stolen → revoke via Keycloak admin console

---

### 2.4 Session Management

**Stateless Sessions** (JWT-based, no server-side session storage).

**Benefits:**
- Scalable (no session affinity required)
- Simpler (no session store to manage)
- Works across multiple API servers

**Tradeoffs:**
- Cannot revoke access tokens instantly (must wait for expiration)
- Solution: Keep access token lifetime short (15 min) + refresh token revocation

---

### 2.5 Multi-Factor Authentication (MFA)

**Keycloak enforces MFA** (TOTP, SMS, email).

**Configuration:**
- Admins require MFA (mandatory)
- Underwriters require MFA (mandatory)
- Distribution users optional MFA (recommended)

**Nebula does not implement MFA** (delegated to Keycloak).

---

### 2.6 Password Policies

**Keycloak enforces password policies:**
- Minimum 12 characters
- Must include uppercase, lowercase, number, special character
- Cannot be username or email
- Cannot be common password (dictionary check)
- Cannot reuse last 5 passwords
- Expire after 90 days (require password change)

---

## 3. Authorization Architecture

### 3.1 ABAC with Casbin

**Authorization is enforced on every API request** using Casbin ABAC policies.

**Middleware:**
```csharp
public class AuthorizationMiddleware
{
    public async Task InvokeAsync(HttpContext context, IEnforcer enforcer)
    {
        var user = context.User;
        var resource = DetermineResource(context.Request.Path);
        var action = DetermineAction(context.Request.Method);

        var allowed = await enforcer.EnforceAsync(
            user.FindFirstValue(ClaimTypes.Role),
            resource,
            action
        );

        if (!allowed)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new { error = "Forbidden" });
            return;
        }

        await _next(context);
    }
}
```

**Enforcement Point:** Every API request passes through authorization middleware.

---

### 3.2 Resource-Level Permissions

**Check permissions at resource level**, not just endpoint level.

**Example:**
- User can access `/api/submissions/{id}` endpoint
- But can they access **this specific** submission?
- Check: Is user assigned underwriter? Is user's broker associated with submission?

```csharp
public async Task<Submission> GetSubmissionAsync(Guid submissionId, ClaimsPrincipal user)
{
    var submission = await _context.Submissions.FindAsync(submissionId);

    // Check resource-level permission
    var allowed = await _enforcer.EnforceAsync(
        new { role = user.FindFirstValue(ClaimTypes.Role), userId = GetUserId(user) },
        new { type = "Submission", id = submissionId, assignedTo = submission.AssignedUnderwriterId },
        "Read"
    );

    if (!allowed)
        throw new ForbiddenException();

    return submission;
}
```

---

### 3.3 Operation-Level Permissions

**Different operations require different permissions:**

**Example: Submission Operations**
- **Read**: Underwriter can read all submissions
- **Update**: Underwriter can update only assigned submissions
- **Transition**: Distribution can transition Triaging→ReadyForUWReview, Underwriter can transition InReview→Quoted
- **Delete**: Only Admin can delete

---

### 3.4 Field-Level Security

**Hide sensitive fields based on user role:**

**Example:**
```csharp
public class BrokerResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    [InternalOnly] // Custom attribute
    public string? InternalNotes { get; set; }

    [InternalOnly]
    public decimal? CommissionRate { get; set; }
}

// Mapper filters fields based on role
public BrokerResponse ToBrokerResponse(Broker broker, ClaimsPrincipal user)
{
    var response = new BrokerResponse
    {
        Id = broker.Id,
        Name = broker.Name
    };

    // Include internal fields only for internal users
    if (user.IsInRole("Admin") || user.IsInRole("Distribution") || user.IsInRole("Underwriter"))
    {
        response.InternalNotes = broker.InternalNotes;
        response.CommissionRate = broker.CommissionRate;
    }

    return response;
}
```

---

## 4. Data Protection

### 4.1 Encryption at Rest

**Database Encryption:**
- PostgreSQL **Transparent Data Encryption (TDE)** enabled
- Encrypts all data files on disk
- Decrypts only when data loaded into memory

**Backup Encryption:**
- Database backups encrypted with AES-256
- Encryption keys stored in AWS KMS or Azure Key Vault (not with backups)

**Sensitive Fields:**
- Tax ID (EIN) encrypted at application level (before storing in database)
- SSN (if stored) encrypted with AES-256

---

### 4.2 Encryption in Transit

**HTTPS/TLS 1.3:**
- All API traffic over HTTPS (TLS 1.3 preferred, TLS 1.2 minimum)
- HTTP requests automatically redirected to HTTPS
- HSTS (HTTP Strict Transport Security) header enabled

**Configuration:**
```csharp
app.UseHsts(); // HSTS header
app.UseHttpsRedirection(); // Redirect HTTP → HTTPS

// Configure Kestrel for HTTPS
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(https =>
    {
        https.SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12;
    });
});
```

**Database Connections:**
- PostgreSQL connection uses SSL/TLS
- Connection string: `sslmode=require`

---

### 4.3 Key Management

**Secrets Managed Externally** (not hardcoded in code or config files).

**Options:**
1. **Environment Variables** (development, testing)
2. **Azure Key Vault** (production on Azure)
3. **AWS Secrets Manager** (production on AWS)
4. **Keycloak Secrets** (for Keycloak-specific secrets)

**Example:**
```csharp
// WRONG: Hardcoded secret
var connectionString = "Host=localhost;Database=nebula;Username=admin;Password=secret123";

// RIGHT: Secret from environment variable
var connectionString = builder.Configuration.GetConnectionString("NebulaDatabse");
// Connection string stored in Azure Key Vault or AWS Secrets Manager
```

---

### 4.4 PII Handling

**Personally Identifiable Information (PII)** requires special handling.

**PII Fields in Nebula:**
- SSN (Social Security Number)
- Tax ID (EIN)
- Email addresses
- Phone numbers
- Physical addresses

**Protection:**
- Encrypt at rest (database TDE + application-level encryption for SSN/Tax ID)
- Encrypt in transit (HTTPS)
- Mask in logs (never log SSN, Tax ID)
- Access logging (log who accessed PII, when)
- Data retention policies (delete after 7 years per regulations)

---

### 4.5 Data Masking

**Mask Sensitive Data in Logs and UI:**

**Example:**
```csharp
public string MaskSSN(string ssn)
{
    if (string.IsNullOrEmpty(ssn) || ssn.Length < 4)
        return "***";

    return $"***-**-{ssn.Substring(ssn.Length - 4)}"; // Show last 4 digits only
}

// Logging
_logger.LogInformation("User accessed SSN for account {AccountId}", accountId);
// Never log: _logger.LogInformation("SSN: {SSN}", ssn); ❌
```

---

## 5. Audit & Compliance

### 5.1 Audit Trail Requirements

**Every mutation must be logged** (who, what, when, why).

**Audit Log Fields:**
- **Who**: UserId (from JWT token)
- **What**: Entity type + entity ID + operation (Created, Updated, Deleted, Transitioned)
- **When**: Timestamp (UTC)
- **Why**: Reason (optional, required for Declined/Withdrawn)
- **Before/After**: Before and after state (for updates)

**Implementation:**
```csharp
public async Task<Broker> UpdateBrokerAsync(Guid brokerId, UpdateBrokerRequest request, Guid userId)
{
    var broker = await _context.Brokers.FindAsync(brokerId);
    var beforeState = JsonSerializer.Serialize(broker);

    // Apply update
    broker.Name = request.Name;
    broker.Email = request.Email;

    var afterState = JsonSerializer.Serialize(broker);

    // Log audit event
    await _context.ActivityTimelineEvents.AddAsync(new ActivityTimelineEvent
    {
        EntityType = "Broker",
        EntityId = brokerId,
        EventType = "BrokerUpdated",
        UserId = userId,
        Timestamp = DateTime.UtcNow,
        Payload = new
        {
            before = beforeState,
            after = afterState,
            changedFields = new[] { "Name", "Email" }
        }
    });

    await _context.SaveChangesAsync();
    return broker;
}
```

---

### 5.2 Immutable Audit Logs

**Audit logs are append-only** (no updates or deletes).

**Database Constraints:**
```sql
-- Prevent updates on ActivityTimelineEvents table
CREATE TRIGGER prevent_timeline_update
BEFORE UPDATE ON activity_timeline_events
FOR EACH ROW
EXECUTE FUNCTION prevent_update();

CREATE FUNCTION prevent_update() RETURNS TRIGGER AS $$
BEGIN
    RAISE EXCEPTION 'Updates not allowed on audit table';
END;
$$ LANGUAGE plpgsql;
```

**Application Constraint:**
```csharp
// Repository for timeline events
public class TimelineEventRepository
{
    public async Task AddAsync(ActivityTimelineEvent evt)
    {
        await _context.ActivityTimelineEvents.AddAsync(evt);
    }

    // No Update or Delete methods (compile-time prevention)
}
```

---

### 5.3 Log Retention Policies

**Regulatory Compliance:**
- **Financial records**: Retain 7 years (Sarbanes-Oxley, IRS)
- **Insurance records**: Retain per state regulations (typically 5-7 years)
- **Audit logs**: Retain 7 years minimum

**Implementation:**
- Archive old audit logs to cold storage (S3 Glacier, Azure Archive)
- Keep recent logs (< 1 year) in hot database for fast queries
- Automated archival process runs monthly

---

### 5.4 Compliance Reporting

**Generate Compliance Reports:**

**SOC 2 (System and Organization Controls):**
- Audit log completeness (all mutations logged)
- Access control effectiveness (authorization enforced)
- Data encryption (at rest and in transit)
- Incident response (security event logging)

**GDPR (if applicable):**
- Right to access (export user data)
- Right to erasure (delete user data)
- Data breach notification (within 72 hours)

---

## 6. OWASP Top 10 Mitigation

### 6.1 Injection Prevention

**SQL Injection:**
- Use parameterized queries (EF Core does this automatically)
- Never concatenate user input into SQL queries

```csharp
// WRONG: SQL injection vulnerability
var query = $"SELECT * FROM Brokers WHERE Name = '{userInput}'";

// RIGHT: Parameterized query (EF Core)
var brokers = await _context.Brokers
    .Where(b => b.Name == userInput)
    .ToListAsync();
```

**NoSQL Injection:**
- Validate input before using in queries
- Use ORM/library features (don't build queries manually)

---

### 6.2 Broken Authentication

**Mitigations:**
- Use Keycloak for authentication (proven, secure)
- Enforce password complexity (Keycloak policy)
- Require MFA for admins and underwriters
- Use short-lived access tokens (15 min)
- Validate JWT signature, expiration, issuer, audience

---

### 6.3 Sensitive Data Exposure

**Mitigations:**
- Encrypt data at rest (database TDE)
- Encrypt data in transit (HTTPS/TLS 1.3)
- Never log sensitive data (SSN, Tax ID, passwords)
- Mask sensitive data in UI and logs
- Use field-level security (hide internal fields from external users)

---

### 6.4 XML External Entities (XXE)

**Mitigations:**
- Disable XML external entity processing in parsers
- Use JSON instead of XML where possible

```csharp
var settings = new XmlReaderSettings
{
    DtdProcessing = DtdProcessing.Prohibit, // Disable DTD processing
    XmlResolver = null // Disable external entity resolution
};
```

---

### 6.5 Broken Access Control

**Mitigations:**
- Enforce authorization on every API request (middleware)
- Check resource-level permissions (not just endpoint-level)
- Use ABAC for fine-grained control (Casbin)
- Log all authorization decisions (approved and denied)
- Default deny (no policy = deny access)

---

### 6.6 Security Misconfiguration

**Mitigations:**
- Secure defaults (HTTPS, authentication required, CORS restricted)
- Disable debug mode in production
- Remove unnecessary features (e.g., Swagger in production)
- Keep dependencies up-to-date (automated scanning)
- Review security headers (HSTS, CSP, X-Frame-Options)

```csharp
if (app.Environment.IsProduction())
{
    app.UseHsts();
    // Disable Swagger in production
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

---

### 6.7 Cross-Site Scripting (XSS)

**Mitigations:**
- Output encoding (React does this automatically)
- Content Security Policy (CSP) headers
- HttpOnly cookies (cannot be accessed by JavaScript)
- Validate and sanitize input

```csharp
// CSP header
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline';");
    await next();
});
```

---

### 6.8 Insecure Deserialization

**Mitigations:**
- Use `System.Text.Json` (secure by default)
- Avoid deserializing untrusted data
- Validate input before deserialization

```csharp
// Validate input
var request = await JsonSerializer.DeserializeAsync<CreateBrokerRequest>(requestBody);
var validationResult = await _validator.ValidateAsync(request);
if (!validationResult.IsValid)
    return Results.BadRequest(validationResult.Errors);
```

---

### 6.9 Using Components with Known Vulnerabilities

**Mitigations:**
- Automated dependency scanning (Dependabot, Snyk)
- Keep dependencies up-to-date
- Review security advisories
- Use `dotnet list package --vulnerable` to check for vulnerabilities

---

### 6.10 Insufficient Logging & Monitoring

**Mitigations:**
- Log all authentication attempts (success and failure)
- Log all authorization decisions (approved and denied)
- Log all security events (failed login, suspicious activity)
- Structured logging with correlation IDs
- Centralized logging (Elasticsearch, Splunk)
- Alerting for security events (failed login spikes, authorization failures)

```csharp
_logger.LogWarning("Failed login attempt for {Email} from {IpAddress}",
    email, context.Connection.RemoteIpAddress);

_logger.LogInformation("Authorization {Result}: {User} {Action} {Resource}",
    allowed ? "ALLOWED" : "DENIED",
    userId,
    action,
    resource);
```

---

## Version History

**Version 1.0** - 2026-01-31 - Initial security architecture guide (500 lines)
