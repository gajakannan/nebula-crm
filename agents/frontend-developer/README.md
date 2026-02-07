# Frontend Developer Agent

Generic specification for the Frontend Developer role.

## Overview

The Frontend Developer implements **user interfaces** based on product and architecture specifications. All solution-specific screen specs live in `planning-mds/`.

## Quick Start

```bash
cat agents/frontend-developer/SKILL.md
cat planning-mds/INCEPTION.md
cat planning-mds/architecture/SOLUTION-PATTERNS.md
```

## Core Workflow (Summary)

1) Read `planning-mds/INCEPTION.md` (Sections 3.x and 4.x)
2) Review screen specifications in `planning-mds/screens/`
3) Review API contracts in `planning-mds/api/`
4) Review or create JSON Schemas in `planning-mds/schemas/` (shared with backend)
5) Implement React components with TypeScript
6) Build forms with React Hook Form + AJV validation
7) Integrate APIs using TanStack Query
8) Add accessibility (WCAG AA compliance)
9) Write tests (Vitest + React Testing Library)
10) Optimize performance (code splitting, lazy loading)

## Tech Stack

- **Framework:** React 18 + TypeScript
- **Build Tool:** Vite
- **Styling:** Tailwind CSS
- **Component Library:** shadcn/ui
- **State Management:** TanStack Query (React Query)
- **Forms:**
  - **Manual forms:** React Hook Form + AJV (JSON Schema validation)
  - **Dynamic forms:** RJSF (React JSON Schema Form) - auto-generates forms from schemas
- **Routing:** React Router v6
- **Testing:** Vitest + React Testing Library + jest-axe
- **E2E Testing:** Playwright (Quality Engineer owns)

**Why JSON Schema + AJV?**
- Schema sharing: Same validation on frontend (AJV/RJSF) and backend (C# JSON Schema validator)
- Language-agnostic: JSON Schema works across TypeScript, C#, Python
- Type generation: TypeScript types can be generated from JSON Schema
- Consistency: Single source of truth for validation rules
- Dynamic forms: RJSF auto-generates UI from schemas (great for admin panels, configurable forms)

## Templates

- `agents/templates/screen-spec-template.md` - Screen specifications
- `agents/templates/api-contract-template.yaml` - API endpoint contracts
- Generic component templates (to be added)

## References

### Generic References (in agents/frontend-developer/references/)
- `react-best-practices.md` - React component patterns and best practices
- `typescript-patterns.md` - TypeScript typing and conventions
- `accessibility-guide.md` - WCAG compliance guide
- `ux-principles.md` - UX design principles
- `form-handling-guide.md` - Form handling patterns
- `json-schema-forms-guide.md` - JSON Schema form validation guide
- `tanstack-query-guide.md` - TanStack Query patterns
- `testing-guide.md` - Frontend testing approaches
- `design-inspiration.md` - Design inspiration and references

### Solution-Specific References
- `planning-mds/architecture/SOLUTION-PATTERNS.md` - Frontend patterns section
- `planning-mds/screens/` - Screen specifications for this project
- `planning-mds/api/` - API contracts (OpenAPI specs)

## Scripts

- `agents/frontend-developer/scripts/scaffold-component.py` - Generate React component module (TSX/types/index + optional tests/styles)
- `agents/frontend-developer/scripts/scaffold-page.py` - Generate page module (TSX/types/index + optional tests + route metadata)
- `agents/frontend-developer/scripts/run-tests.sh` - Run frontend tests (uses `FRONTEND_TEST_CMD` or `npm test`)

### Usage Examples

```bash
# Scaffold a shared component with tests and CSS module
python3 agents/frontend-developer/scripts/scaffold-component.py CustomerCard \
  --type shared \
  --with-tests \
  --with-styles

# Scaffold a page with route metadata + tests
python3 agents/frontend-developer/scripts/scaffold-page.py CustomerDetails \
  --route /customers/:id \
  --with-tests

# Patch an existing route registry file (requires scaffold markers)
python3 agents/frontend-developer/scripts/scaffold-page.py Orders \
  --route /orders \
  --routes-file experience/src/routes/index.tsx

# Run tests
FRONTEND_TEST_CMD="npm test" sh agents/frontend-developer/scripts/run-tests.sh
```

Potential future scripts:
- `check-accessibility.sh` - Run accessibility tests (jest-axe)
- `analyze-bundle.sh` - Analyze bundle size (vite-bundle-visualizer)

## Common Tasks

### Set Up Project
```bash
cd experience/
npm install
npm run dev
```

### Run Tests
```bash
npm test                    # Run all tests
npm run test:watch         # Watch mode
npm run test:coverage      # Coverage report
npm run test:a11y          # Accessibility tests
```

### Build for Production
```bash
npm run build              # Build optimized bundle
npm run preview            # Preview production build
npm run analyze            # Analyze bundle size
```

### Code Quality
```bash
npm run lint               # ESLint
npm run type-check         # TypeScript check
npm run format             # Prettier
```

## Development Guidelines

### Component File Structure
```
components/
├── ui/                    # shadcn/ui components
│   ├── button.tsx
│   ├── input.tsx
│   └── ...
├── forms/                 # Form components
│   ├── CustomerForm.tsx
│   └── AccountForm.tsx
├── layouts/               # Layout components
│   ├── AppLayout.tsx
│   └── DashboardLayout.tsx
└── shared/                # Shared business components
    ├── CustomerCard.tsx
    └── ActivityTimeline.tsx
```

### Naming Conventions
- **Components:** PascalCase (`CustomerForm.tsx`)
- **Hooks:** camelCase with `use` prefix (`useAuth.ts`)
- **Utilities:** camelCase (`formatDate.ts`)
- **Types:** PascalCase (`Customer`, `CustomerFormData`)
- **Constants:** UPPER_SNAKE_CASE (`MAX_FILE_SIZE`)

### Import Order
```tsx
// 1. External libraries
import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';

// 2. Internal libraries/components
import { Button } from '@/components/ui/button';
import { useAuth } from '@/hooks/useAuth';

// 3. Types
import type { Customer } from '@/types/customer';

// 4. Relative imports
import { formatCustomerName } from './utils';
```

### TypeScript Guidelines
- Use `interface` for object shapes (can match JSON Schema structure)
- Use `type` for unions, intersections, or mapped types
- Avoid `any` - use `unknown` if type is truly unknown
- Generate TypeScript types from JSON Schema using `json-schema-to-typescript`
- Manually define types for form data that match JSON Schema structure
- Export types that are used across files

### Form Pattern

**Choose the right approach:**
- **Manual forms (React Hook Form):** Static forms, custom layouts, complex UX
- **Dynamic forms (RJSF):** Admin interfaces, schema-driven forms, configurable forms

**Manual forms should:**
1. Use JSON Schema for validation (shared with backend in `planning-mds/schemas/` or `experience/src/schemas/`)
2. Use React Hook Form with ajvResolver
3. Configure AJV with `ajv-errors` for user-friendly error messages
4. Display field-level errors
5. Handle loading/submitting states
6. Show success/error messages
7. Include accessibility labels (htmlFor, aria-labels)

**Dynamic forms (RJSF) should:**
1. Load JSON Schema from shared location
2. Customize with UI Schema for field-specific widgets
3. Use custom widgets (shadcn/ui components) for consistent styling
4. Handle form submission and validation automatically
5. Display errors inline (RJSF handles this)

### API Integration Pattern
Every API integration should:
1. Define TypeScript types for request/response
2. Use TanStack Query (useQuery for GET, useMutation for POST/PUT/DELETE)
3. Handle loading, error, empty states
4. Implement proper caching strategy
5. Invalidate related queries on mutations

### Testing Requirements
- Unit test all business logic
- Integration test user flows (form submission, navigation)
- Accessibility test with jest-axe
- ≥80% coverage for business logic components
- Mock API calls using MSW (Mock Service Worker)

## Validation Checklist

Before marking work as complete:

- [ ] All screens implemented per specifications
- [ ] TypeScript errors resolved (no `any` types)
- [ ] Forms include validation and error handling
- [ ] API integration complete with proper error handling
- [ ] Loading/error/empty states handled
- [ ] Responsive design (mobile, tablet, desktop tested)
- [ ] Accessibility: keyboard navigation works
- [ ] Accessibility: screen reader tested
- [ ] Accessibility: color contrast meets WCAG AA
- [ ] Tests passing (≥80% coverage)
- [ ] No console errors or warnings
- [ ] Code follows SOLUTION-PATTERNS.md conventions
- [ ] Bundle size acceptable (<500KB initial load)

## Troubleshooting

### Common Issues

**TypeScript errors after schema change:**
```bash
npm run type-check
# Fix type mismatches in components
```

**Tests failing after API change:**
```bash
# Update MSW mocks in src/mocks/handlers.ts
# Re-run tests
npm test
```

**Bundle size too large:**
```bash
npm run analyze
# Identify large dependencies
# Add lazy loading for heavy components
```

**Accessibility violations:**
```bash
npm run test:a11y
# Fix ARIA labels, semantic HTML, keyboard nav
```

## Best Practices

1. **Type Safety** - Leverage TypeScript, avoid `any`
2. **Component Composition** - Small, reusable components
3. **Accessibility First** - WCAG AA compliance from the start
4. **Performance** - Code splitting, memoization, lazy loading
5. **Testing** - Test user behavior, not implementation details
6. **Error Handling** - User-friendly messages, recovery options
7. **Security** - XSS prevention, token security, input validation
8. **Consistency** - Follow established patterns in SOLUTION-PATTERNS.md

## Learning Resources

- [React Documentation](https://react.dev)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- [TanStack Query Docs](https://tanstack.com/query/latest)
- [React Hook Form Docs](https://react-hook-form.com/)
- [AJV Documentation](https://ajv.js.org/)
- [JSON Schema Specification](https://json-schema.org/)
- [RJSF Documentation](https://rjsf-team.github.io/react-jsonschema-form/)
- [Tailwind CSS Docs](https://tailwindcss.com/docs)
- [shadcn/ui Components](https://ui.shadcn.com/)
- [WCAG Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
