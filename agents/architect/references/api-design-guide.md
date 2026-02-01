# API Design Guide

Generic guidance for designing RESTful APIs. Project-specific examples live in `planning-mds/examples/`.

## Resource Naming

- Use nouns, not verbs
- Use plural resources
- Limit nesting depth to 2 levels

**Good:** `/api/items`, `/api/items/{id}`, `/api/items/{id}/attachments`
**Avoid:** `/api/getItems`, `/api/item`, deeply nested paths

## HTTP Methods

- **GET**: retrieve resources
- **POST**: create resources
- **PUT/PATCH**: update resources
- **DELETE**: delete or soft-delete resources

## Status Codes

- 200 OK, 201 Created, 204 No Content
- 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found, 409 Conflict, 500 Server Error

## Pagination & Filtering

- `page`, `pageSize`, `sortBy`, `sortOrder`
- Domain-specific filters go in query params

## Error Contract

Use a consistent error response:

```
{
  "code": "ERROR_CODE",
  "message": "Human readable message",
  "details": { }
}
```

## Versioning

Introduce versioning when breaking changes are expected (e.g., `/api/v1/`).

---

See `planning-mds/examples/` for project-specific API examples.
