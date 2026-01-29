# Frontend Developer Agent

Complete specification and resources for the Frontend Developer builder agent role.

## Overview

The Frontend Developer Agent is responsible for implementing React/TypeScript user interfaces during Phase C (Implementation Mode). This agent creates production-quality frontend code that integrates with backend APIs and provides an excellent user experience.

**Key Principle:** Component-Driven Development with TypeScript. Accessibility First. Performance Matters.

---

## Quick Start

### 1. Activate the Agent

When ready to implement UI:

```bash
# Read the agent specification
cat agents/frontend-developer/SKILL.md

# Review screen specifications
cat planning-mds/INCEPTION.md  # Section 3.5

# Review API contracts
cat planning-mds/INCEPTION.md  # Section 4.5
```

### 2. Review Specifications

Understand requirements before coding:
- Section 3.5: Screen specifications (layouts, fields, interactions)
- Section 4.5: API contracts (endpoints, request/response models)
- Section 3.4: User stories (acceptance criteria)
- Section 4.4: Authorization model and token storage strategy
- Section 4.6: Security requirements (NFRs)
- ADRs: `planning-mds/architecture/decisions/` (especially auth-related ADRs)

### 3. Load References

```bash
# Frontend best practices
cat agents/frontend-developer/references/react-best-practices.md

# TypeScript patterns
cat agents/frontend-developer/references/typescript-patterns.md

# Accessibility guide
cat agents/frontend-developer/references/accessibility-guide.md
```

### 4. Follow the Workflow

See "Workflow Example" section in `SKILL.md` for step-by-step implementation guidance.

---

## Agent Structure

```
frontend-developer/
├── SKILL.md                          # Main agent specification
├── README.md                         # This file
├── references/                       # Best practices and patterns
│   ├── react-best-practices.md
│   ├── typescript-patterns.md
│   ├── tanstack-query-guide.md
│   ├── form-handling-guide.md
│   ├── accessibility-guide.md
│   └── testing-guide.md
└── scripts/                          # Development scripts
    ├── README.md
    ├── scaffold-component.py
    ├── scaffold-page.py
    └── run-tests.sh
```

---

## Core Responsibilities

### 1. Component Development
- Build reusable React components
- Use shadcn/ui for common patterns
- Implement responsive layouts
- Follow component composition patterns

### 2. API Integration
- Use TanStack Query for data fetching
- Define TypeScript types for API data
- Handle loading, error, and success states
- Implement caching and invalidation

### 3. Form Implementation
- Use React Hook Form for form management
- Implement Zod schemas for validation
- Match backend validation rules
- Provide clear error messages

### 4. Authentication & Authorization
- Integrate with Keycloak (OIDC)
- Implement protected routes
- Handle token management
- Show/hide UI based on permissions

### 5. Accessibility
- Implement ARIA labels and roles
- Ensure keyboard navigation
- Manage focus properly
- Test with screen readers

### 6. Testing
- Write component tests (React Testing Library)
- Test user interactions
- Mock API calls
- Cover edge cases and errors

---

## Technology Stack

### Core Technologies
- **Language:** TypeScript 5
- **Framework:** React 18
- **Build Tool:** Vite
- **Styling:** Tailwind CSS + shadcn/ui
- **State Management:** TanStack Query (server), Context API (global)
- **Forms:** React Hook Form + Zod
- **Routing:** React Router
- **Testing:** Vitest + React Testing Library
- **HTTP Client:** Axios

### Authentication
- **AuthN:** Keycloak (OIDC/OAuth2)
- **Token Storage:** Secure httpOnly cookies or sessionStorage

---

## Project Structure

```
src/
├── api/                    # API client and functions
│   ├── client.ts          # Axios instance with interceptors
│   ├── brokers.ts         # Broker API functions
│   └── ...
├── components/            # Reusable components
│   ├── ui/               # shadcn/ui components
│   ├── brokers/          # Broker-specific components
│   └── ...
├── pages/                 # Page components
│   ├── BrokersPage.tsx
│   └── ...
├── hooks/                 # Custom React hooks
│   ├── useAuth.ts
│   └── ...
├── types/                 # TypeScript types
│   ├── broker.ts
│   └── ...
├── lib/                   # Utilities
│   └── utils.ts
├── styles/                # Global styles
│   └── globals.css
└── App.tsx                # Root component

tests/
└── components/
    └── *.test.tsx
```

---

## Key Resources

### References (Frontend-Specific)

Located in `agents/frontend-developer/references/`:

| Reference | Purpose | When to Use |
|-----------|---------|-------------|
| `react-best-practices.md` | React patterns and conventions | Daily coding reference |
| `typescript-patterns.md` | TypeScript best practices | Type definitions |
| `tanstack-query-guide.md` | Data fetching patterns | API integration |
| `form-handling-guide.md` | Form implementation patterns | Building forms |
| `accessibility-guide.md` | A11y implementation | Making UI accessible |
| `testing-guide.md` | Component testing patterns | Writing tests |

### Scripts

Located in `agents/frontend-developer/scripts/`:

| Script | Purpose | Usage |
|--------|---------|-------|
| `scaffold-component.py` | Generate component boilerplate | `python scaffold-component.py BrokerCard` |
| `scaffold-page.py` | Generate page boilerplate | `python scaffold-page.py BrokersPage` |
| `run-tests.sh` | Run tests with coverage | `./run-tests.sh` |

---

## Development Workflow

### Step 1: Review User Story and Screen Spec

- Read acceptance criteria
- Understand UI requirements
- Identify API endpoints needed
- Check authorization requirements

### Step 2: Review API Contracts

- Check available endpoints
- Review request/response models
- Understand error responses
- Plan API integration

### Step 3: Create API Integration

Create API functions:
```bash
# Create API module for feature
touch src/api/brokers.ts
```

Define types and functions:
- Type definitions from API contract
- Fetch functions
- Create/Update/Delete functions
- Error handling

### Step 4: Create Components

Scaffold component:
```bash
python agents/frontend-developer/scripts/scaffold-component.py BrokerCard
```

Implement component:
- Props interface with JSDoc
- Component logic
- Tailwind styling
- Accessibility attributes

### Step 5: Create Page

Scaffold page:
```bash
python agents/frontend-developer/scripts/scaffold-page.py BrokersPage
```

Implement page:
- Layout structure
- Integrate components
- Add routing
- Implement data fetching

### Step 6: Implement Forms (if needed)

Create form component:
- Define Zod schema
- Use React Hook Form
- Add validation error messages
- Handle submission

### Step 7: Write Tests

Create test file:
```bash
touch src/components/brokers/__tests__/BrokerCard.test.tsx
```

Write tests:
- Rendering tests
- Interaction tests
- API integration tests (mocked)
- Error state tests

### Step 8: Run Tests

```bash
npm test
# or
./agents/frontend-developer/scripts/run-tests.sh
```

### Step 9: Manual Testing

```bash
npm run dev
```

Test in browser:
- Happy path
- Error scenarios
- Loading states
- Keyboard navigation
- Screen reader (if possible)

### Step 10: Validate Completeness

Check Definition of Done:
- [ ] TypeScript compiles (zero errors)
- [ ] No ESLint errors
- [ ] Tests pass
- [ ] UI matches spec
- [ ] API integration works
- [ ] Accessibility implemented

### Step 11: Commit and Hand Off

```bash
git add .
git commit -m "feat: Implement [feature name]

[Description]

Co-Authored-By: Claude (claude-sonnet-4-5) <noreply@anthropic.com>"
```

---

## Quality Standards

### Code Quality Checklist

- [ ] TypeScript strict mode enabled
- [ ] No `any` types (use `unknown` if necessary)
- [ ] All props interfaces documented with JSDoc
- [ ] Components under 300 lines
- [ ] Hooks follow naming convention (use*)
- [ ] No console.log statements
- [ ] No unused imports
- [ ] Follows ESLint rules

### Component Quality Checklist

- [ ] Single responsibility principle
- [ ] Props interface defined
- [ ] Default props where appropriate
- [ ] Proper event handlers (on* naming)
- [ ] Memoization where needed (useMemo, useCallback)
- [ ] Error boundary implemented per route

### API Integration Checklist

- [ ] TanStack Query for data fetching
- [ ] Loading states shown
- [ ] Error states handled
- [ ] Types match API contract
- [ ] Query keys properly structured
- [ ] Cache invalidation after mutations

### Form Quality Checklist

- [ ] React Hook Form used
- [ ] Zod schema for validation
- [ ] Client validation matches backend
- [ ] Clear error messages
- [ ] Proper field labeling
- [ ] Disabled state during submission

### Accessibility Checklist

- [ ] Semantic HTML used
- [ ] ARIA labels on interactive elements
- [ ] Keyboard navigation works
- [ ] Focus management proper
- [ ] Color contrast meets WCAG AA
- [ ] Form fields have labels
- [ ] Error messages associated with fields

### Testing Checklist

- [ ] Component tests written
- [ ] Rendering tests
- [ ] Interaction tests
- [ ] API integration tests (mocked)
- [ ] Error state tests
- [ ] All tests pass
- [ ] No flaky tests

---

## Common Pitfalls

### ❌ Using `any` Type

**Problem:** Losing type safety

**Fix:**
```typescript
// Bad
const data: any = response.data;

// Good
const data: Broker[] = brokerSchema.array().parse(response.data);
```

### ❌ Not Handling Loading States

**Problem:** Blank screen while loading

**Fix:**
```tsx
if (isLoading) {
  return <Skeleton />;
}
```

### ❌ Not Handling Errors

**Problem:** Silent failures

**Fix:**
```tsx
if (isError) {
  return <Alert variant="destructive">{error.message}</Alert>;
}
```

### ❌ Missing Form Validation

**Problem:** No client-side validation

**Fix:**
```typescript
const schema = z.object({
  name: z.string().min(1, 'Name is required'),
  email: z.string().email('Invalid email'),
});
```

### ❌ Poor Accessibility

**Problem:** No keyboard navigation, no ARIA

**Fix:**
```tsx
<button
  onClick={handleClick}
  aria-label="Close dialog"
  onKeyDown={(e) => e.key === 'Escape' && handleClose()}
>
  Close
</button>
```

---

## Definition of Done

### Before Committing

- [ ] Code compiles (zero TypeScript errors)
- [ ] No ESLint errors
- [ ] All tests pass
- [ ] Manual testing done
- [ ] Accessibility tested (keyboard nav)
- [ ] No console logs or debug statements

### Before Handing Off

- [ ] Feature branch created and pushed
- [ ] All acceptance criteria met
- [ ] UI matches screen spec
- [ ] API integration works
- [ ] Loading and error states handled
- [ ] Forms validate correctly
- [ ] Authentication/authorization works
- [ ] Tests written and passing
- [ ] No TypeScript errors
- [ ] No ESLint errors

---

## Tools & Commands

### Common npm Commands

```bash
# Install dependencies
npm install

# Development server
npm run dev

# Build for production
npm run build

# Run tests
npm test

# Run tests in watch mode
npm test -- --watch

# Type check
npm run typecheck

# Lint
npm run lint

# Format code
npm run format
```

### Common Vite Commands

```bash
# Preview production build
npm run preview

# Build with source maps
npm run build -- --sourcemap

# Analyze bundle size
npm run build -- --mode analyze
```

---

## Handoff to Code Reviewer

### Handoff Checklist

- [ ] Code compiles (zero errors)
- [ ] All tests pass
- [ ] UI matches screen spec
- [ ] API integration works
- [ ] Authentication/authorization implemented
- [ ] Accessibility implemented
- [ ] Error handling implemented
- [ ] Code committed to feature branch
- [ ] Pull request created

### Handoff Artifacts

Provide to Code Reviewer:
1. Pull request with screenshots
2. Link to user story with acceptance criteria
3. Test results (all passing)
4. Manual testing checklist completed

---

## Troubleshooting

### Build Errors

**Problem:** TypeScript compilation errors

**Fix:** Check types, run `npm run typecheck` for details.

### Test Failures

**Problem:** Tests fail with "cannot find module"

**Fix:** Check import paths, ensure test setup is correct.

### API Integration Issues

**Problem:** 401 Unauthorized errors

**Fix:** Check token storage, ensure interceptor adds auth header.

### Accessibility Issues

**Problem:** Keyboard navigation not working

**Fix:** Ensure `tabIndex` is set, implement `onKeyDown` handlers.

---

## Version History

**Version 1.0** - 2026-01-28 - Initial Frontend Developer agent
- SKILL.md with complete agent specification
- Best practices guides (pending creation)
- Scaffolding scripts (pending creation)
- Testing guide (pending creation)

---

## Next Steps

Ready to implement frontend code?

1. Read `SKILL.md` thoroughly
2. Review screen specs and API contracts
3. Set up development environment (Node.js, npm)
4. Start with one screen (e.g., Broker List)
5. Follow the 11-step workflow
6. Write tests as you go
7. Validate before handoff

**Remember:** Your job is to build a great user experience with clean, type-safe, accessible code that integrates seamlessly with backend APIs.
