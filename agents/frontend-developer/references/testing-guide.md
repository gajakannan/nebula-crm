# Testing Guide (React Testing Library)

**Version:** 1.0
**Last Updated:** 2026-01-29
**Applies To:** Frontend Developer

---

## Overview

This guide covers frontend testing in Nebula using **React Testing Library** and **Vitest**. We follow the principle: **"Test behavior, not implementation."**

**Philosophy:** Write tests that interact with components the same way users do - through the rendered UI, not internal implementation details.

---

## Installation

```bash
npm install -D vitest @testing-library/react @testing-library/jest-dom @testing-library/user-event @vitejs/plugin-react
```

### Vitest Configuration

```ts
// vitest.config.ts
import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: './src/test/setup.ts',
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html'],
      exclude: [
        'node_modules/',
        'src/test/',
        '**/*.d.ts',
        '**/*.config.*',
        '**/mockData',
      ],
    },
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
});
```

### Test Setup File

```ts
// src/test/setup.ts
import '@testing-library/jest-dom';
import { cleanup } from '@testing-library/react';
import { afterEach } from 'vitest';

// Cleanup after each test
afterEach(() => {
  cleanup();
});

// Mock window.matchMedia
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: vi.fn().mockImplementation((query) => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: vi.fn(),
    removeListener: vi.fn(),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
  })),
});
```

---

## Basic Component Testing

### Simple Component Test

```tsx
// BrokerCard.test.tsx
import { render, screen } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import { BrokerCard } from './BrokerCard';

describe('BrokerCard', () => {
  const mockBroker = {
    id: '123',
    name: 'ABC Insurance Brokers',
    licenseNumber: 'CA0123456',
    email: 'contact@abc.com',
  };

  it('should render broker name', () => {
    render(<BrokerCard broker={mockBroker} />);

    expect(screen.getByText('ABC Insurance Brokers')).toBeInTheDocument();
  });

  it('should render license number', () => {
    render(<BrokerCard broker={mockBroker} />);

    expect(screen.getByText(/CA0123456/)).toBeInTheDocument();
  });

  it('should render email as a link', () => {
    render(<BrokerCard broker={mockBroker} />);

    const emailLink = screen.getByRole('link', { name: /contact@abc.com/i });
    expect(emailLink).toHaveAttribute('href', 'mailto:contact@abc.com');
  });
});
```

---

## Querying Elements

### Query Priority (Use in this order)

1. **Accessible Queries (Preferred)**
```tsx
// By role (best for accessibility)
screen.getByRole('button', { name: /submit/i });
screen.getByRole('heading', { name: /broker details/i });
screen.getByRole('textbox', { name: /email/i });
screen.getByRole('link', { name: /view details/i });

// By label text (forms)
screen.getByLabelText(/broker name/i);

// By placeholder
screen.getByPlaceholderText(/search brokers/i);

// By text content
screen.getByText(/no brokers found/i);
```

2. **Semantic Queries**
```tsx
// By display value (inputs with values)
screen.getByDisplayValue(/abc insurance/i);

// By alt text (images)
screen.getByAltText(/broker logo/i);

// By title
screen.getByTitle(/close dialog/i);
```

3. **Test IDs (Last resort)**
```tsx
screen.getByTestId('broker-card');
```

### Query Variants

```tsx
// getBy* - Throws error if not found (use for elements that should exist)
screen.getByRole('button');

// queryBy* - Returns null if not found (use for elements that shouldn't exist)
expect(screen.queryByRole('button')).not.toBeInTheDocument();

// findBy* - Returns promise, waits for element (use for async)
await screen.findByRole('button');

// *AllBy* - Returns array of elements
const buttons = screen.getAllByRole('button');
```

---

## User Interactions

### Using userEvent (Recommended)

```tsx
import { render, screen } from '@testing-library/react';
import { userEvent } from '@testing-library/user-event';

describe('CreateBrokerForm', () => {
  it('should submit form with valid data', async () => {
    const user = userEvent.setup();
    const onSubmit = vi.fn();

    render(<CreateBrokerForm onSubmit={onSubmit} />);

    // Type into inputs
    await user.type(screen.getByLabelText(/broker name/i), 'ABC Insurance');
    await user.type(screen.getByLabelText(/license number/i), 'CA0123456');
    await user.type(screen.getByLabelText(/email/i), 'test@abc.com');

    // Click button
    await user.click(screen.getByRole('button', { name: /submit/i }));

    // Assert
    expect(onSubmit).toHaveBeenCalledWith({
      name: 'ABC Insurance',
      licenseNumber: 'CA0123456',
      email: 'test@abc.com',
    });
  });
});
```

### Common User Interactions

```tsx
const user = userEvent.setup();

// Typing
await user.type(input, 'text to type');
await user.clear(input);

// Clicking
await user.click(button);
await user.dblClick(button);

// Selecting
await user.selectOptions(select, 'option-value');
await user.deselectOptions(select, 'option-value');

// Checkboxes/Radio
await user.click(checkbox); // Toggle

// Keyboard
await user.keyboard('{Enter}');
await user.keyboard('{Escape}');
await user.tab(); // Focus next element

// Hover
await user.hover(element);
await user.unhover(element);

// Upload files
await user.upload(fileInput, file);
```

---

## Testing Forms

### Form Validation

```tsx
describe('BrokerForm validation', () => {
  it('should show error for invalid email', async () => {
    const user = userEvent.setup();
    render(<BrokerForm />);

    const emailInput = screen.getByLabelText(/email/i);
    await user.type(emailInput, 'invalid-email');
    await user.tab(); // Blur field

    expect(await screen.findByText(/invalid email address/i)).toBeInTheDocument();
  });

  it('should show error for required field', async () => {
    const user = userEvent.setup();
    render(<BrokerForm />);

    const submitButton = screen.getByRole('button', { name: /submit/i });
    await user.click(submitButton);

    expect(await screen.findByText(/broker name is required/i)).toBeInTheDocument();
  });

  it('should clear errors when valid input is entered', async () => {
    const user = userEvent.setup();
    render(<BrokerForm />);

    const emailInput = screen.getByLabelText(/email/i);

    // Enter invalid email
    await user.type(emailInput, 'invalid');
    await user.tab();
    expect(await screen.findByText(/invalid email/i)).toBeInTheDocument();

    // Fix email
    await user.clear(emailInput);
    await user.type(emailInput, 'valid@example.com');
    await user.tab();

    expect(screen.queryByText(/invalid email/i)).not.toBeInTheDocument();
  });
});
```

### Form Submission

```tsx
it('should submit form and show success message', async () => {
  const user = userEvent.setup();
  const mockCreateBroker = vi.fn().mockResolvedValue({ id: '123' });

  render(<CreateBrokerForm createBroker={mockCreateBroker} />);

  // Fill form
  await user.type(screen.getByLabelText(/broker name/i), 'ABC Insurance');
  await user.type(screen.getByLabelText(/license number/i), 'CA0123456');
  await user.type(screen.getByLabelText(/email/i), 'test@abc.com');

  // Submit
  await user.click(screen.getByRole('button', { name: /submit/i }));

  // Assert API was called
  expect(mockCreateBroker).toHaveBeenCalledWith({
    name: 'ABC Insurance',
    licenseNumber: 'CA0123456',
    email: 'test@abc.com',
  });

  // Assert success message
  expect(await screen.findByText(/broker created successfully/i)).toBeInTheDocument();
});
```

---

## Testing Async Operations

### Waiting for Elements

```tsx
import { waitFor } from '@testing-library/react';

it('should load and display brokers', async () => {
  const mockBrokers = [
    { id: '1', name: 'ABC Insurance' },
    { id: '2', name: 'XYZ Brokers' },
  ];

  render(<BrokerList />);

  // Wait for brokers to load
  expect(await screen.findByText('ABC Insurance')).toBeInTheDocument();
  expect(await screen.findByText('XYZ Brokers')).toBeInTheDocument();
});

// Or use waitFor for custom conditions
it('should update status after transition', async () => {
  render(<SubmissionCard />);

  const button = screen.getByRole('button', { name: /approve/i });
  await userEvent.click(button);

  await waitFor(() => {
    expect(screen.getByText(/approved/i)).toBeInTheDocument();
  });
});
```

### Mocking API Calls (with MSW)

```tsx
// src/test/mocks/handlers.ts
import { http, HttpResponse } from 'msw';

export const handlers = [
  http.get('/api/brokers', () => {
    return HttpResponse.json([
      { id: '1', name: 'ABC Insurance', licenseNumber: 'CA0123456' },
      { id: '2', name: 'XYZ Brokers', licenseNumber: 'NY9876543' },
    ]);
  }),

  http.post('/api/brokers', async ({ request }) => {
    const body = await request.json();
    return HttpResponse.json(
      { id: '123', ...body },
      { status: 201 }
    );
  }),

  http.delete('/api/brokers/:id', () => {
    return HttpResponse.json(null, { status: 204 });
  }),
];

// src/test/mocks/server.ts
import { setupServer } from 'msw/node';
import { handlers } from './handlers';

export const server = setupServer(...handlers);

// src/test/setup.ts
import { server } from './mocks/server';
import { beforeAll, afterEach, afterAll } from 'vitest';

beforeAll(() => server.listen());
afterEach(() => server.resetHandlers());
afterAll(() => server.close());
```

### Using MSW in Tests

```tsx
import { server } from '@/test/mocks/server';
import { http, HttpResponse } from 'msw';

describe('BrokerList', () => {
  it('should handle API error', async () => {
    // Override default handler for this test
    server.use(
      http.get('/api/brokers', () => {
        return HttpResponse.json(
          { message: 'Internal Server Error' },
          { status: 500 }
        );
      })
    );

    render(<BrokerList />);

    expect(await screen.findByText(/failed to load brokers/i)).toBeInTheDocument();
  });

  it('should show empty state when no brokers', async () => {
    server.use(
      http.get('/api/brokers', () => {
        return HttpResponse.json([]);
      })
    );

    render(<BrokerList />);

    expect(await screen.findByText(/no brokers found/i)).toBeInTheDocument();
  });
});
```

---

## Testing with TanStack Query

### Wrapper Setup

```tsx
// src/test/utils.tsx
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactNode } from 'react';

export function createTestQueryClient() {
  return new QueryClient({
    defaultOptions: {
      queries: {
        retry: false, // Don't retry in tests
        gcTime: Infinity, // Don't garbage collect
      },
    },
  });
}

export function renderWithQueryClient(ui: ReactNode) {
  const testQueryClient = createTestQueryClient();

  return render(
    <QueryClientProvider client={testQueryClient}>
      {ui}
    </QueryClientProvider>
  );
}
```

### Testing Queries

```tsx
import { renderWithQueryClient } from '@/test/utils';

describe('BrokerDetails', () => {
  it('should load and display broker', async () => {
    renderWithQueryClient(<BrokerDetails brokerId="123" />);

    // Loading state
    expect(screen.getByText(/loading/i)).toBeInTheDocument();

    // Data loaded
    expect(await screen.findByText('ABC Insurance')).toBeInTheDocument();
    expect(screen.getByText('CA0123456')).toBeInTheDocument();
  });

  it('should show error when broker not found', async () => {
    server.use(
      http.get('/api/brokers/:id', () => {
        return HttpResponse.json(
          { message: 'Not found' },
          { status: 404 }
        );
      })
    );

    renderWithQueryClient(<BrokerDetails brokerId="999" />);

    expect(await screen.findByText(/broker not found/i)).toBeInTheDocument();
  });
});
```

### Testing Mutations

```tsx
it('should create broker and refetch list', async () => {
  const user = userEvent.setup();
  const testQueryClient = createTestQueryClient();

  render(
    <QueryClientProvider client={testQueryClient}>
      <BrokerList />
      <CreateBrokerDialog />
    </QueryClientProvider>
  );

  // Initially 2 brokers
  expect(await screen.findAllByRole('article')).toHaveLength(2);

  // Open dialog and create broker
  await user.click(screen.getByRole('button', { name: /create broker/i }));
  await user.type(screen.getByLabelText(/broker name/i), 'New Broker');
  await user.type(screen.getByLabelText(/license/i), 'TX1111111');
  await user.click(screen.getByRole('button', { name: /submit/i }));

  // List should refetch and show 3 brokers
  await waitFor(() => {
    expect(screen.getAllByRole('article')).toHaveLength(3);
  });
});
```

---

## Testing Accessibility

### ARIA Roles and Labels

```tsx
it('should have accessible button', () => {
  render(<DeleteBrokerButton />);

  const button = screen.getByRole('button', { name: /delete broker/i });
  expect(button).toBeInTheDocument();
});

it('should have accessible form fields', () => {
  render(<BrokerForm />);

  // Labels should be associated with inputs
  const nameInput = screen.getByLabelText(/broker name/i);
  expect(nameInput).toHaveAccessibleName('Broker Name');
});

it('should have accessible error messages', async () => {
  const user = userEvent.setup();
  render(<BrokerForm />);

  await user.click(screen.getByRole('button', { name: /submit/i }));

  const errorMessage = await screen.findByText(/broker name is required/i);
  expect(errorMessage).toHaveAccessibleDescription();
});
```

### Keyboard Navigation

```tsx
it('should be keyboard navigable', async () => {
  const user = userEvent.setup();
  render(<BrokerForm />);

  const nameInput = screen.getByLabelText(/broker name/i);
  const emailInput = screen.getByLabelText(/email/i);
  const submitButton = screen.getByRole('button', { name: /submit/i });

  // Tab through form
  await user.tab();
  expect(nameInput).toHaveFocus();

  await user.tab();
  expect(emailInput).toHaveFocus();

  await user.tab();
  expect(submitButton).toHaveFocus();

  // Submit with Enter
  await user.keyboard('{Enter}');
  // Assert form submission
});

it('should close dialog with Escape', async () => {
  const user = userEvent.setup();
  const onClose = vi.fn();

  render(<Dialog open onClose={onClose} />);

  await user.keyboard('{Escape}');
  expect(onClose).toHaveBeenCalled();
});
```

---

## Testing Hooks

### Custom Hook Testing

```tsx
import { renderHook, waitFor } from '@testing-library/react';

describe('useBroker', () => {
  it('should fetch broker data', async () => {
    const { result } = renderHook(() => useBroker('123'), {
      wrapper: ({ children }) => (
        <QueryClientProvider client={createTestQueryClient()}>
          {children}
        </QueryClientProvider>
      ),
    });

    // Initially loading
    expect(result.current.isLoading).toBe(true);

    // Wait for data
    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true);
    });

    expect(result.current.data).toEqual({
      id: '123',
      name: 'ABC Insurance',
    });
  });
});
```

---

## Snapshot Testing

### When to Use Snapshots

Use snapshots for:
- Static components (headers, footers)
- Error messages
- Icons and small UI elements

**Don't use for:**
- Complex components with lots of logic
- Components with dynamic data

```tsx
it('should match snapshot', () => {
  const { container } = render(<ErrorMessage message="Something went wrong" />);
  expect(container).toMatchSnapshot();
});

// Update snapshots with:
// npm test -- -u
```

---

## Best Practices

### 1. Test User Behavior, Not Implementation

✅ **GOOD:**
```tsx
it('should show error message on failed login', async () => {
  const user = userEvent.setup();
  render(<LoginForm />);

  await user.type(screen.getByLabelText(/email/i), 'wrong@example.com');
  await user.type(screen.getByLabelText(/password/i), 'wrongpass');
  await user.click(screen.getByRole('button', { name: /login/i }));

  expect(await screen.findByText(/invalid credentials/i)).toBeInTheDocument();
});
```

❌ **BAD:**
```tsx
it('should call loginFailed action', () => {
  const { result } = renderHook(() => useLogin());
  result.current.loginFailed();
  expect(result.current.error).toBe('Invalid credentials');
});
```

### 2. Use Accessible Queries

✅ **GOOD:**
```tsx
screen.getByRole('button', { name: /submit/i });
screen.getByLabelText(/email/i);
```

❌ **BAD:**
```tsx
screen.getByTestId('submit-button');
screen.getByClassName('email-input');
```

### 3. Don't Test Third-Party Libraries

✅ **GOOD:**
```tsx
it('should submit form when valid', async () => {
  const onSubmit = vi.fn();
  render(<Form onSubmit={onSubmit} />);

  // Fill and submit
  await user.click(submitButton);

  expect(onSubmit).toHaveBeenCalled();
});
```

❌ **BAD:**
```tsx
it('should validate with Zod', () => {
  const schema = z.string().min(3);
  expect(() => schema.parse('ab')).toThrow();
});
```

### 4. Keep Tests Focused

✅ **GOOD:**
```tsx
it('should display broker name', () => {
  render(<BrokerCard broker={mockBroker} />);
  expect(screen.getByText('ABC Insurance')).toBeInTheDocument();
});

it('should display license number', () => {
  render(<BrokerCard broker={mockBroker} />);
  expect(screen.getByText('CA0123456')).toBeInTheDocument();
});
```

❌ **BAD:**
```tsx
it('should render broker card correctly', () => {
  // Tests 10 different things
});
```

### 5. Use Descriptive Test Names

✅ **GOOD:**
```tsx
it('should show validation error when email is invalid', () => {});
it('should disable submit button while form is submitting', () => {});
```

❌ **BAD:**
```tsx
it('works', () => {});
it('test form', () => {});
```

---

## Coverage Goals

Target coverage levels:
- **Statements:** > 80%
- **Branches:** > 75%
- **Functions:** > 80%
- **Lines:** > 80%

Run coverage report:
```bash
npm test -- --coverage
```

---

## Common Testing Patterns

### Loading States

```tsx
it('should show skeleton while loading', () => {
  render(<BrokerList />);
  expect(screen.getByTestId('skeleton')).toBeInTheDocument();
});

it('should show content after loading', async () => {
  render(<BrokerList />);
  expect(await screen.findByText('ABC Insurance')).toBeInTheDocument();
  expect(screen.queryByTestId('skeleton')).not.toBeInTheDocument();
});
```

### Empty States

```tsx
it('should show empty state when no data', async () => {
  server.use(
    http.get('/api/brokers', () => HttpResponse.json([]))
  );

  render(<BrokerList />);

  expect(await screen.findByText(/no brokers found/i)).toBeInTheDocument();
  expect(screen.getByRole('button', { name: /create broker/i })).toBeInTheDocument();
});
```

### Conditional Rendering

```tsx
it('should show edit button for admin users', () => {
  render(<BrokerCard broker={mockBroker} userRole="admin" />);
  expect(screen.getByRole('button', { name: /edit/i })).toBeInTheDocument();
});

it('should not show edit button for regular users', () => {
  render(<BrokerCard broker={mockBroker} userRole="user" />);
  expect(screen.queryByRole('button', { name: /edit/i })).not.toBeInTheDocument();
});
```

---

## References

- [React Testing Library Docs](https://testing-library.com/react)
- [Vitest Documentation](https://vitest.dev)
- [Common Testing Mistakes](https://kentcdodds.com/blog/common-mistakes-with-react-testing-library)
- [Testing Library Guiding Principles](https://testing-library.com/docs/guiding-principles/)
- [MSW (Mock Service Worker)](https://mswjs.io)
