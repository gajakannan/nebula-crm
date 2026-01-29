# Security Best Practices

Guidance for Security agent work. Use OWASP and internal standards.

## Core Practices
- Authenticate all endpoints (except health)
- Authorize all mutations with least privilege
- Validate all inputs server-side
- Encrypt sensitive data at rest and in transit
- Log security-relevant events without leaking sensitive data

## Reviews
- Threat modeling for new features
- Token storage ADR for auth changes
- OWASP Top 10 coverage per release
