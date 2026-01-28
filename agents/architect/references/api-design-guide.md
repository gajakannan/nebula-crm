# API Design Guide

Focused guide for designing RESTful APIs for BrokerHub.

## REST Principles

### Resource Naming
- Use nouns, not verbs: `/brokers` not `/getBrokers`
- Use plurals: `/brokers` not `/broker`
- Use hierarchical URLs: `/brokers/{id}/contacts`
- Lowercase with hyphens: `/broker-contacts`

### HTTP Methods
- **GET:** Retrieve resource(s)
- **POST:** Create new resource
- **PUT:** Update entire resource
- **PATCH:** Partial update
- **DELETE:** Remove resource

### Status Codes
- **200 OK:** Successful GET, PUT, PATCH
- **201 Created:** Successful POST
- **204 No Content:** Successful DELETE
- **400 Bad Request:** Validation error
- **401 Unauthorized:** Missing/invalid auth
- **403 Forbidden:** Insufficient permissions
- **404 Not Found:** Resource doesn't exist
- **409 Conflict:** Business rule violation
- **500 Internal Server Error:** Unexpected error

## Standard Patterns

### List/Search Endpoint
```
GET /api/brokers?page=1&pageSize=20&search=acme&sortBy=name&sortOrder=asc
```

Response:
```json
{
  "data": [...],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 150,
    "totalPages": 8
  }
}
```

### Error Response
```json
{
  "code": "VALIDATION_ERROR",
  "message": "Invalid request data",
  "details": [
    {"field": "name", "message": "Name is required"}
  ],
  "traceId": "uuid"
}
```

See `agents/templates/api-contract-template.yaml` for complete OpenAPI template.
