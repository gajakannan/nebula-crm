# API Design Guide

Comprehensive guide for designing RESTful APIs using .NET 10 Minimal APIs for the Nebula insurance CRM. This guide covers REST principles, request/response patterns, pagination, filtering, versioning, security, and OpenAPI documentation.

---

## 1. RESTful Principles

### 1.1 Resource Naming Conventions

**Use Nouns, Not Verbs:**
- ✅ Good: `GET /api/brokers`, `POST /api/submissions`
- ❌ Bad: `GET /api/getBrokers`, `POST /api/createSubmission`

**Use Plural Nouns:**
- ✅ Good: `/api/brokers`, `/api/submissions`
- ❌ Bad: `/api/broker`, `/api/submission`

**Hierarchical Resource URLs:**
- ✅ Good: `/api/brokers/{id}/contacts` (contacts belong to broker)
- ✅ Good: `/api/accounts/{id}/submissions` (submissions for account)
- ❌ Bad: `/api/broker-contacts?brokerId={id}` (flat structure)

**Lowercase with Hyphens (for multi-word resources):**
- ✅ Good: `/api/workflow-transitions`, `/api/timeline-events`
- ❌ Bad: `/api/WorkflowTransitions`, `/api/timeline_events`

**Avoid Deep Nesting (max 2 levels):**
- ✅ Good: `/api/brokers/{id}/contacts/{contactId}`
- ❌ Bad: `/api/brokers/{id}/accounts/{accountId}/submissions/{submissionId}/documents/{docId}`
- Better: `/api/submissions/{submissionId}/documents/{docId}`

---

### 1.2 HTTP Methods (Verbs)

**GET - Retrieve Resources:**
- `GET /api/brokers` - List all brokers (with pagination)
- `GET /api/brokers/{id}` - Get single broker by ID
- `GET /api/brokers/{id}/submissions` - Get broker's submissions
- Idempotent: Multiple identical requests have same effect
- No side effects (no data mutations)
- Cacheable

**POST - Create New Resource:**
- `POST /api/brokers` - Create new broker
- `POST /api/submissions/{id}/transition` - Perform action (state change)
- Non-idempotent: Multiple requests create multiple resources
- Returns `201 Created` with `Location` header pointing to new resource
- Response body includes created resource

**PUT - Replace Entire Resource:**
- `PUT /api/brokers/{id}` - Replace entire broker (all fields required)
- Idempotent: Multiple identical requests have same effect
- Returns `200 OK` with updated resource or `204 No Content`
- Rarely used in practice (PATCH preferred)

**PATCH - Partial Update:**
- `PATCH /api/brokers/{id}` - Update specific fields
- Idempotent (if designed correctly)
- Returns `200 OK` with updated resource
- Preferred over PUT for updates

**DELETE - Remove Resource:**
- `DELETE /api/brokers/{id}` - Soft delete broker
- Idempotent: Deleting already-deleted resource returns `204`
- Returns `204 No Content` (no response body)
- Consider soft delete (set DeletedAt timestamp) vs hard delete

---

### 1.3 HTTP Status Codes

**Success Codes:**
- **200 OK**: Successful GET, PUT, PATCH (with response body)
- **201 Created**: Successful POST (new resource created)
- **204 No Content**: Successful DELETE or PUT with no response body
- **202 Accepted**: Request accepted, processing asynchronously (long-running operations)

**Client Error Codes:**
- **400 Bad Request**: Validation error, malformed request
- **401 Unauthorized**: Missing or invalid authentication token
- **403 Forbidden**: User authenticated but lacks permission
- **404 Not Found**: Resource doesn't exist
- **409 Conflict**: Business rule violation (e.g., duplicate license number, invalid state transition)
- **422 Unprocessable Entity**: Request syntactically valid but semantically invalid

**Server Error Codes:**
- **500 Internal Server Error**: Unexpected server error
- **503 Service Unavailable**: Service temporarily down (maintenance, overload)

---

### 1.4 Idempotency

**Idempotent Methods** (safe to retry):
- GET, PUT, DELETE
- Multiple identical requests have the same effect as single request

**Non-Idempotent Methods** (not safe to retry):
- POST
- Multiple requests may create multiple resources

**Idempotency Keys for POST:**
```http
POST /api/submissions
Idempotency-Key: 550e8400-e29b-41d4-a716-446655440000
Content-Type: application/json

{ "brokerId": "...", "insuredName": "Acme Corp" }
```

Server stores idempotency key; duplicate requests with same key return original response (don't create duplicate).

---

### 1.5 HATEOAS Considerations

**HATEOAS** (Hypermedia as the Engine of Application State): Include links to related resources in responses.

**Example:**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "name": "Acme Insurance",
  "status": "Active",
  "_links": {
    "self": { "href": "/api/brokers/123e4567-e89b-12d3-a456-426614174000" },
    "contacts": { "href": "/api/brokers/123e4567-e89b-12d3-a456-426614174000/contacts" },
    "submissions": { "href": "/api/brokers/123e4567-e89b-12d3-a456-426614174000/submissions" }
  }
}
```

**Recommendation for Nebula MVP:** Keep HATEOAS minimal (use for pagination links only). Full HATEOAS adds complexity without clear benefit for internal API.

---

## 2. Request/Response Patterns

### 2.1 Request Models (DTOs)

**Use Data Transfer Objects (DTOs) for Requests:**

```csharp
public record CreateBrokerRequest
{
    [Required]
    [MaxLength(255)]
    public string Name { get; init; }

    [Required]
    [MaxLength(50)]
    public string LicenseNumber { get; init; }

    [Required]
    [RegularExpression(@"^[A-Z]{2}$")]
    public string State { get; init; }

    [EmailAddress]
    public string? Email { get; init; }

    [Phone]
    public string? Phone { get; init; }
}

// Minimal API endpoint
app.MapPost("/api/brokers", async (
    CreateBrokerRequest request,
    IBrokerService brokerService,
    IValidator<CreateBrokerRequest> validator) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
        return Results.ValidationProblem(validationResult.ToDictionary());

    var broker = await brokerService.CreateAsync(request);
    return Results.Created($"/api/brokers/{broker.Id}", broker);
})
.WithName("CreateBroker")
.WithTags("Brokers")
.Produces<BrokerResponse>(201)
.ProducesProblem(400)
.ProducesProblem(403)
.ProducesProblem(409);
```

**Validation Best Practices:**
- Use Data Annotations for simple validation (`[Required]`, `[MaxLength]`, `[EmailAddress]`)
- Use FluentValidation for complex validation (cross-field, business rules)
- Return detailed field-level errors

---

### 2.2 Response Models (DTOs)

**Separate Request and Response DTOs:**

```csharp
public record BrokerResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string LicenseNumber { get; init; }
    public string State { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    // Internal-only fields (conditionally included based on user role)
    public string? InternalNotes { get; init; }
}
```

**Map Entities to DTOs (never expose entities directly):**

```csharp
var broker = await context.Brokers.FindAsync(id);
var response = new BrokerResponse
{
    Id = broker.Id,
    Name = broker.Name,
    // ... map fields
};
```

---

### 2.3 Standard Error Contract

**Consistent Error Response Format:**

```csharp
public record ErrorResponse
{
    [JsonPropertyName("code")]
    public string Code { get; init; }

    [JsonPropertyName("message")]
    public string Message { get; init; }

    [JsonPropertyName("details")]
    public List<ErrorDetail>? Details { get; init; }

    [JsonPropertyName("traceId")]
    public string? TraceId { get; init; }
}

public record ErrorDetail
{
    [JsonPropertyName("field")]
    public string? Field { get; init; }

    [JsonPropertyName("message")]
    public string Message { get; init; }
}
```

**Example Error Responses:**

**Validation Error (400):**
```json
{
  "code": "VALIDATION_ERROR",
  "message": "Invalid request data",
  "details": [
    { "field": "name", "message": "Name is required" },
    { "field": "licenseNumber", "message": "License number must be unique" }
  ],
  "traceId": "0HN1234567890ABCDEF"
}
```

**Business Rule Violation (409):**
```json
{
  "code": "DUPLICATE_LICENSE",
  "message": "A broker with this license number already exists",
  "details": [
    { "field": "licenseNumber", "message": "License number CA-12345 is already in use" }
  ],
  "traceId": "0HN1234567890ABCDEF"
}
```

**Authorization Error (403):**
```json
{
  "code": "INSUFFICIENT_PERMISSIONS",
  "message": "User lacks CreateBroker permission",
  "traceId": "0HN1234567890ABCDEF"
}
```

---

### 2.4 Success Response Envelopes

**Simple Response (No Envelope):**
```json
GET /api/brokers/123
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "name": "Acme Insurance",
  "status": "Active"
}
```

**List Response (With Pagination Metadata):**
```json
GET /api/brokers?page=1&pageSize=20
{
  "data": [
    { "id": "...", "name": "Acme Insurance" },
    { "id": "...", "name": "Best Brokers" }
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 156,
  "totalPages": 8
}
```

**Recommendation:** Use envelope for lists (to include pagination metadata), no envelope for single resources.

---

## 3. Pagination Patterns

### 3.1 Offset Pagination (Page-Based)

**Query Parameters:**
- `page` (integer, default: 1)
- `pageSize` (integer, default: 20, max: 100)

**Request:**
```http
GET /api/brokers?page=2&pageSize=20
```

**Response:**
```json
{
  "data": [
    { "id": "...", "name": "Broker 21" },
    { "id": "...", "name": "Broker 22" }
  ],
  "page": 2,
  "pageSize": 20,
  "totalCount": 156,
  "totalPages": 8
}
```

**Implementation:**
```csharp
app.MapGet("/api/brokers", async (
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    ApplicationDbContext context) =>
{
    if (pageSize > 100) pageSize = 100;

    var query = context.Brokers.AsQueryable();

    var totalCount = await query.CountAsync();

    var data = await query
        .OrderBy(b => b.Name)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(b => new BrokerListItem { Id = b.Id, Name = b.Name })
        .ToListAsync();

    return Results.Ok(new
    {
        data,
        page,
        pageSize,
        totalCount,
        totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
    });
});
```

**Pros:** Simple, supports "jump to page N", shows total count
**Cons:** Slow for large offsets (OFFSET 10000 is slow), inconsistent if data changes between requests

---

### 3.2 Cursor-Based Pagination (Keyset)

**Better for Large Datasets:**

**Query Parameters:**
- `cursor` (string, opaque token pointing to last seen item)
- `limit` (integer, default: 20)

**Request:**
```http
GET /api/brokers?limit=20&cursor=eyJuYW1lIjoiQWNtZSIsImlkIjoiMTIzIn0=
```

**Response:**
```json
{
  "data": [ ... ],
  "nextCursor": "eyJuYW1lIjoiWmVuaXRoIiwiaWQiOiI3ODkifQ==",
  "hasMore": true
}
```

**Implementation:**
```csharp
var (name, id) = DecodeCursor(cursor); // Base64 decode

var brokers = await context.Brokers
    .Where(b => b.Name.CompareTo(name) > 0 || (b.Name == name && b.Id.CompareTo(id) > 0))
    .OrderBy(b => b.Name)
    .ThenBy(b => b.Id)
    .Take(limit + 1)
    .ToListAsync();

var hasMore = brokers.Count > limit;
if (hasMore) brokers.RemoveAt(brokers.Count - 1);

var nextCursor = hasMore ? EncodeCursor(brokers.Last().Name, brokers.Last().Id) : null;
```

**Pros:** Fast for large datasets, consistent results
**Cons:** Can't jump to arbitrary page, no total count

---

### 3.3 HATEOAS Pagination Links

**Include Navigation Links:**
```json
{
  "data": [ ... ],
  "page": 2,
  "pageSize": 20,
  "totalCount": 156,
  "totalPages": 8,
  "_links": {
    "first": { "href": "/api/brokers?page=1&pageSize=20" },
    "prev": { "href": "/api/brokers?page=1&pageSize=20" },
    "self": { "href": "/api/brokers?page=2&pageSize=20" },
    "next": { "href": "/api/brokers?page=3&pageSize=20" },
    "last": { "href": "/api/brokers?page=8&pageSize=20" }
  }
}
```

---

## 4. Filtering & Sorting

### 4.1 Query String Parameters for Filtering

**Simple Filters:**
```http
GET /api/brokers?status=Active&state=CA
```

**Search (across multiple fields):**
```http
GET /api/brokers?search=acme
```

**Filter Operators:**
```http
GET /api/submissions?premium[gte]=100000&effectiveDate[lte]=2026-12-31
```

**Implementation:**
```csharp
app.MapGet("/api/brokers", async (
    [FromQuery] string? status,
    [FromQuery] string? state,
    [FromQuery] string? search,
    ApplicationDbContext context) =>
{
    var query = context.Brokers.AsQueryable();

    if (!string.IsNullOrEmpty(status))
        query = query.Where(b => b.Status == status);

    if (!string.IsNullOrEmpty(state))
        query = query.Where(b => b.State == state);

    if (!string.IsNullOrEmpty(search))
        query = query.Where(b => EF.Functions.ILike(b.Name, $"%{search}%") ||
                                 EF.Functions.ILike(b.LicenseNumber, $"%{search}%"));

    return Results.Ok(await query.ToListAsync());
});
```

---

### 4.2 Multi-Field Sorting

**Query Parameter:**
```http
GET /api/brokers?sort=name:asc,createdAt:desc
```

**Implementation:**
```csharp
public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortParam)
{
    if (string.IsNullOrEmpty(sortParam))
        return query;

    var sortFields = sortParam.Split(',');

    IOrderedQueryable<T>? orderedQuery = null;

    foreach (var field in sortFields)
    {
        var parts = field.Split(':');
        var fieldName = parts[0];
        var direction = parts.Length > 1 ? parts[1] : "asc";

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, fieldName);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = orderedQuery == null
            ? (direction == "desc" ? "OrderByDescending" : "OrderBy")
            : (direction == "desc" ? "ThenByDescending" : "ThenBy");

        orderedQuery = (IOrderedQueryable<T>)typeof(Queryable)
            .GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type)
            .Invoke(null, new object[] { orderedQuery ?? query, lambda })!;
    }

    return orderedQuery ?? query;
}
```

---

### 4.3 Search Across Multiple Fields

**PostgreSQL Full-Text Search:**
```csharp
var brokers = await context.Brokers
    .Where(b => EF.Functions.ToTsVector("english", b.Name + " " + b.LicenseNumber)
        .Matches(EF.Functions.ToTsQuery("english", searchTerm)))
    .ToListAsync();
```

---

## 5. Versioning Strategies

### 5.1 URI Versioning (Recommended for Nebula)

**Include Version in URL Path:**
```http
GET /api/v1/brokers
GET /api/v2/brokers
```

**Pros:** Simple, explicit, easy to route
**Cons:** Multiple endpoints in code, URL changes

**Implementation:**
```csharp
var v1 = app.MapGroup("/api/v1").WithTags("V1");
v1.MapGet("/brokers", GetBrokersV1);

var v2 = app.MapGroup("/api/v2").WithTags("V2");
v2.MapGet("/brokers", GetBrokersV2);
```

---

### 5.2 Header Versioning

**Version in Custom Header:**
```http
GET /api/brokers
API-Version: 2
```

**Pros:** Clean URLs
**Cons:** Less discoverable, harder to test

---

### 5.3 Query Parameter Versioning

**Version as Query Param:**
```http
GET /api/brokers?version=2
```

**Pros:** Easy to test
**Cons:** Pollutes query string, easy to forget

---

### 5.4 Deprecation Policy

**Sunset Header (RFC 8594):**
```http
HTTP/1.1 200 OK
Sunset: Sat, 31 Dec 2026 23:59:59 GMT
Deprecation: true
Link: <https://api.nebula.com/api/v2/brokers>; rel="successor-version"
```

**Recommendation:** Support previous version for 12 months after new version released.

---

## 6. API Security

### 6.1 Authentication (JWT from Keycloak)

**Require Bearer Token on All Endpoints:**
```csharp
app.MapGet("/api/brokers", async () => { ... })
    .RequireAuthorization();
```

**Validate JWT Token:**
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://keycloak.example.com/realms/nebula";
        options.Audience = "nebula-api";
        options.RequireHttpsMetadata = true;
    });
```

---

### 6.2 Authorization (Casbin ABAC)

**Enforce Authorization Middleware:**
```csharp
app.Use(async (context, next) =>
{
    var enforcer = context.RequestServices.GetRequiredService<IEnforcer>();
    var user = context.User;
    var resource = DetermineResource(context.Request.Path);
    var action = DetermineAction(context.Request.Method);

    if (!await enforcer.EnforceAsync(user, resource, action))
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsJsonAsync(new ErrorResponse
        {
            Code = "INSUFFICIENT_PERMISSIONS",
            Message = $"User lacks {action} permission for {resource}"
        });
        return;
    }

    await next();
});
```

---

### 6.3 CORS Configuration

**Configure CORS for Frontend:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://app.nebula.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

app.UseCors("AllowFrontend");
```

---

### 6.4 Rate Limiting

**Prevent Abuse:**
```csharp
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});

app.UseRateLimiter();
```

---

### 6.5 Input Validation

**Never Trust Client Input:**
- Validate all request data (Data Annotations, FluentValidation)
- Sanitize strings (prevent SQL injection, XSS)
- Use parameterized queries (EF Core does this automatically)
- Validate file uploads (size, type, content)

---

## 7. OpenAPI Documentation

### 7.1 Swagger/OpenAPI 3.0 Best Practices

**Add Swagger to .NET 10:**
```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Nebula Insurance CRM API",
        Version = "v1",
        Description = "API for managing brokers, accounts, submissions, and renewals",
        Contact = new OpenApiContact
        {
            Name = "Nebula Support",
            Email = "support@nebula.example.com"
        }
    });

    // Add JWT authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

app.UseSwagger();
app.UseSwaggerUI();
```

---

### 7.2 Schema Definitions (components/schemas)

**Define Reusable Schemas:**
```yaml
components:
  schemas:
    BrokerResponse:
      type: object
      properties:
        id:
          type: string
          format: uuid
        name:
          type: string
        status:
          type: string
          enum: [Active, Inactive, Suspended]

    ErrorResponse:
      type: object
      required: [code, message]
      properties:
        code:
          type: string
        message:
          type: string
        details:
          type: array
          items:
            $ref: '#/components/schemas/ErrorDetail'
```

---

### 7.3 Examples and Descriptions

**Add Examples to Endpoints:**
```csharp
app.MapPost("/api/brokers", async (CreateBrokerRequest request) => { ... })
    .WithOpenApi(operation =>
    {
        operation.Summary = "Create a new broker";
        operation.Description = "Creates a new insurance broker or brokerage firm. Requires CreateBroker permission.";
        return operation;
    })
    .Produces<BrokerResponse>(201)
    .ProducesProblem(400)
    .ProducesProblem(403);
```

---

### 7.4 Generating Client SDKs

**Use OpenAPI Generator:**
```bash
# Generate TypeScript client for React frontend
openapi-generator-cli generate \
  -i https://api.nebula.com/swagger/v1/swagger.json \
  -g typescript-fetch \
  -o clients/typescript

# Generate C# client for integration tests
openapi-generator-cli generate \
  -i https://api.nebula.com/swagger/v1/swagger.json \
  -g csharp-netcore \
  -o clients/csharp
```

---

## Version History

**Version 2.0** - 2026-01-31 - Comprehensive API design guide with .NET 10 Minimal APIs (400 lines)
**Version 1.0** - 2026-01-26 - Initial API design guide (64 lines)
