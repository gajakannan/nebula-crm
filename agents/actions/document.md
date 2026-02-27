# Action: Document

## User Intent

Generate comprehensive technical documentation including API documentation, README files, runbooks, and developer guides.

## Agent Flow

```
Technical Writer
```

**Flow Type:** Single agent

## Prerequisites

- [ ] Implementation code exists (backend and/or frontend)
- [ ] Architecture artifacts available in `planning-mds/`
- [ ] API endpoints are stable and tested
- [ ] Application is deployable

## Inputs

### From Planning
- `planning-mds/BLUEPRINT.md` (overview, architecture)
- `planning-mds/architecture/` (data model, ADRs, architecture notes)
- `planning-mds/api/` (OpenAPI/API contracts)
- User stories and features

### From Codebase
- API controllers and endpoints
- Database schema (migrations)
- Configuration files
- Docker setup (Dockerfile, docker-compose.yml)
- Environment variables

### From User
- Documentation scope (API, README, runbooks, all)
- Target audience (developers, operators, end users)
- Specific areas requiring documentation

## Outputs

### API Documentation
- API reference documentation (OpenAPI/Swagger)
- Endpoint descriptions
- Request/response examples
- Authentication/authorization guide
- Error codes and handling
- Rate limiting and pagination

### README Files
- **Root README.md:**
  - Project overview
  - Tech stack
  - Prerequisites
  - Quick start guide
  - Project structure
  - Development setup
  - Testing instructions
  - Deployment guide
  - Contributing guidelines
  - License

- **Component READMEs:**
  - Backend README (API setup, structure)
  - Frontend README (UI setup, structure)
  - Database README (schema, migrations)

### Runbooks
- Deployment runbook
- Operational runbook (monitoring, troubleshooting)
- Database migration runbook
- Backup and recovery runbook
- Incident response runbook

### Developer Guides
- Architecture overview
- Code organization guide
- Development workflow
- Testing guide
- Debugging guide
- Common tasks guide

## Agent Responsibilities

### Technical Writer
1. **API Documentation:**
   - Generate OpenAPI specification from code
   - Write endpoint descriptions
   - Add request/response examples
   - Document authentication flow
   - Document error responses
   - Add usage examples

2. **README Creation:**
   - Write project overview
   - Document tech stack and prerequisites
   - Create quick start guide
   - Document project structure
   - Write development setup instructions
   - Document testing procedures
   - Create deployment guide

3. **Runbook Development:**
   - Document deployment procedures
   - Create troubleshooting guides
   - Document common operations
   - Add monitoring guidance
   - Include rollback procedures

4. **Developer Guide Creation:**
   - Explain architecture patterns
   - Document code organization
   - Describe development workflow
   - Provide debugging tips
   - Document common tasks

5. **Quality Assurance:**
   - Validate documentation accuracy
   - Check for completeness
   - Ensure clarity and readability
   - Test all examples and commands

## Validation Criteria

### API Documentation Complete
- [ ] All endpoints documented
- [ ] Request/response schemas defined
- [ ] Authentication documented
- [ ] Error codes listed
- [ ] Examples provided
- [ ] OpenAPI spec generated

### README Complete
- [ ] Project overview clear
- [ ] Prerequisites listed
- [ ] Quick start works
- [ ] Project structure explained
- [ ] Development setup documented
- [ ] Testing instructions clear
- [ ] Deployment guide complete

### Runbooks Complete
- [ ] Deployment steps clear
- [ ] Troubleshooting guide helpful
- [ ] Common operations documented
- [ ] Monitoring setup explained
- [ ] Rollback procedures defined

### Developer Guides Complete
- [ ] Architecture explained
- [ ] Code organization clear
- [ ] Development workflow documented
- [ ] Common tasks covered
- [ ] Debugging guide helpful

## Example Usage

### Scenario 1: API Documentation
```
User: "Document the customer management API"

Document Action:
  ↓
Technical Writer:
  - Reviews customer API endpoints
  - Generates OpenAPI spec

  Endpoints Documented:
    - POST /api/customers
    - GET /api/customers
    - GET /api/customers/:id
    - PUT /api/customers/:id
    - DELETE /api/customers/:id

  For Each Endpoint:
    - Description
    - Parameters
    - Request body schema
    - Response schema
    - Example requests/responses
    - Error codes

  Output:
    - docs/api/customers.md
    - openapi.yaml (updated)
```

### Scenario 2: Complete Documentation
```
User: "Generate all documentation for the application"

Document Action:
  ↓
Technical Writer:
  API Documentation:
    - Generates full OpenAPI spec
    - Documents all endpoints
    - Adds examples and errors

  README:
    - Root README.md
    - backend/README.md
    - frontend/README.md

  Runbooks:
    - docs/runbooks/deployment.md
    - docs/runbooks/operations.md
    - docs/runbooks/migrations.md

  Developer Guides:
    - docs/guides/architecture.md
    - docs/guides/development.md
    - docs/guides/testing.md

  Output:
    - 12 documentation files created
    - All examples tested and verified
```

### Scenario 3: README Update
```
User: "Update README with new deployment instructions"

Document Action:
  ↓
Technical Writer:
  - Reviews current README
  - Reads new deployment setup (Docker, docker-compose)
  - Updates deployment section

  Updates:
    - Added Docker prerequisites
    - Updated environment setup
    - Added docker-compose instructions
    - Updated troubleshooting section

  Output:
    - README.md updated
    - Deployment section verified
```

## Documentation Best Practices

### API Documentation
- ✅ Use OpenAPI/Swagger standard
- ✅ Include realistic examples
- ✅ Document error scenarios
- ✅ Keep in sync with code
- ❌ Don't use generic descriptions
- ❌ Don't skip error documentation

### README Files
- ✅ Start with quick start
- ✅ Use clear headings
- ✅ Include working examples
- ✅ Keep it up-to-date
- ❌ Don't assume prior knowledge
- ❌ Don't skip prerequisites

### Runbooks
- ✅ Use step-by-step format
- ✅ Include verification steps
- ✅ Add troubleshooting tips
- ✅ Document rollback procedures
- ❌ Don't skip edge cases
- ❌ Don't use ambiguous language

### Developer Guides
- ✅ Explain the "why" not just "how"
- ✅ Use diagrams and examples
- ✅ Link to related docs
- ✅ Keep guides focused
- ❌ Don't duplicate README
- ❌ Don't overwhelm with detail

## Post-Documentation Next Steps

After document action completes:
1. Review documentation for accuracy
2. Test all examples and commands
3. Share with team for feedback
4. Publish to documentation site (if applicable)
5. Keep documentation updated with code changes

## Related Actions

- **After:** [build action](./build.md) or [feature action](./feature.md) - Document after building
- **With:** [blog action](./blog.md) - Blog about new features
- **Continuous:** Update docs when code changes

## Notes

- Documentation should be versioned with code
- Keep docs close to code (co-located when possible)
- Automate API doc generation from code
- Test all commands and examples in docs
- Use documentation as onboarding tool
- Update docs in same PR as code changes
- Consider documentation as part of Definition of Done
