# Test Case Mapping Guide

Map each acceptance criterion to one or more tests.

## Mapping Rules
- Each acceptance criterion must map to at least one test
- Happy path → E2E or integration test
- Error paths → API or integration test
- UI behavior → component test

## Example
**AC:** "Duplicate license number shows error"
- API test: POST /brokers duplicate → 409
- UI test: error message displayed
