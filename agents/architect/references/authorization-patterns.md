# Authorization Patterns (ABAC/RBAC)

Generic guidance for authorization design. Project-specific policies live in `planning-mds/architecture/decisions/` or `planning-mds/security/`.

## RBAC vs ABAC

- **RBAC:** Roles grant fixed permissions (e.g., "Admin can edit all records")
- **ABAC:** Policies evaluate attributes (e.g., "User can update records assigned to them")

## Common Patterns

1) **Ownership-based access**
- Users can read/write records they own

2) **Assignment-based access**
- Users can update records assigned to them

3) **Team/Group-based access**
- Managers can access records assigned to their team

4) **State-based access**
- Only allow transitions when record is in a specific state

## Policy Design Tips

- Default deny if no policy matches
- Keep policies readable and version-controlled
- Test policies with unit/integration tests

## Example (Pseudo-Policy)

- Allow if user.role == "Manager"
- Allow if record.assignedTo == user.id
- Deny otherwise

---

See `planning-mds/examples/architecture/` for project-specific policy examples.
