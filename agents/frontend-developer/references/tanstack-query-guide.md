# TanStack Query Guide

**Version:** 1.0
**Last Updated:** 2026-01-29
**Applies To:** Frontend Developer

---

## Overview

TanStack Query (formerly React Query) is our solution for managing server state in Nebula. It handles data fetching, caching, synchronization, and updates with minimal boilerplate.

**Core Philosophy:** Server state is fundamentally different from client state. TanStack Query treats it as a cache that can become stale and needs revalidation.

---

## Installation & Setup

```bash
npm install @tanstack/react-query @tanstack/react-query-devtools
```

### Query Client Setup

```tsx
// src/lib/queryClient.ts
import { QueryClient } from '@tanstack/react-query';

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // 5 minutes
      gcTime: 1000 * 60 * 30, // 30 minutes (formerly cacheTime)
      retry: 1,
      refetchOnWindowFocus: false,
    },
    mutations: {
      retry: 0,
    },
  },
});
```

### Provider Setup

```tsx
// src/App.tsx
import { QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { queryClient } from './lib/queryClient';

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <Router />
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  );
}
```

---

## Queries (Fetching Data)

### Basic Query

```tsx
import { useQuery } from '@tanstack/react-query';
import { getBrokers } from '@/api/brokers';

function BrokerList() {
  const { data, isLoading, error } = useQuery({
    queryKey: ['brokers'],
    queryFn: getBrokers,
  });

  if (isLoading) return <Skeleton />;
  if (error) return <ErrorAlert error={error} />;

  return (
    <div>
      {data.map(broker => (
        <BrokerCard key={broker.id} broker={broker} />
      ))}
    </div>
  );
}
```

### Query with Parameters

```tsx
function BrokerDetails({ brokerId }: { brokerId: string }) {
  const { data: broker, isLoading } = useQuery({
    queryKey: ['brokers', brokerId], // Include params in key
    queryFn: () => getBroker(brokerId),
    enabled: !!brokerId, // Only run if brokerId exists
  });

  // ...
}
```

### Query Key Patterns

```tsx
// ✅ GOOD - Hierarchical, specific keys
const queryKeys = {
  // All brokers queries
  all: ['brokers'] as const,

  // Broker lists (with filters)
  lists: () => [...queryKeys.all, 'list'] as const,
  list: (filters: BrokerFilters) => [...queryKeys.lists(), filters] as const,

  // Individual brokers
  details: () => [...queryKeys.all, 'detail'] as const,
  detail: (id: string) => [...queryKeys.details(), id] as const,

  // Broker submissions
  submissions: (brokerId: string) => [...queryKeys.detail(brokerId), 'submissions'] as const,
};

// Usage
useQuery({
  queryKey: queryKeys.detail(brokerId),
  queryFn: () => getBroker(brokerId),
});

// ❌ BAD - Inconsistent, hard to invalidate
useQuery({ queryKey: ['broker', brokerId], ... });
useQuery({ queryKey: [brokerId, 'broker'], ... }); // Different order!
```

### Dependent Queries

```tsx
function SubmissionDetails({ submissionId }: { submissionId: string }) {
  // First query: Get submission
  const { data: submission } = useQuery({
    queryKey: ['submissions', submissionId],
    queryFn: () => getSubmission(submissionId),
  });

  // Second query: Get broker (depends on first query)
  const { data: broker } = useQuery({
    queryKey: ['brokers', submission?.brokerId],
    queryFn: () => getBroker(submission!.brokerId),
    enabled: !!submission?.brokerId, // Only run after submission loads
  });

  // ...
}
```

### Parallel Queries

```tsx
function Dashboard() {
  const brokers = useQuery({
    queryKey: ['brokers'],
    queryFn: getBrokers,
  });

  const submissions = useQuery({
    queryKey: ['submissions'],
    queryFn: getSubmissions,
  });

  const renewals = useQuery({
    queryKey: ['renewals'],
    queryFn: getRenewals,
  });

  // All three queries run in parallel
  if (brokers.isLoading || submissions.isLoading || renewals.isLoading) {
    return <DashboardSkeleton />;
  }

  return (
    <div>
      <BrokersWidget data={brokers.data} />
      <SubmissionsWidget data={submissions.data} />
      <RenewalsWidget data={renewals.data} />
    </div>
  );
}
```

### Paginated Queries

```tsx
function BrokerList() {
  const [page, setPage] = useState(1);

  const { data, isLoading, isPlaceholderData } = useQuery({
    queryKey: ['brokers', { page }],
    queryFn: () => getBrokers({ page, pageSize: 20 }),
    placeholderData: (previousData) => previousData, // Keep old data while loading new
  });

  return (
    <div>
      {data?.items.map(broker => <BrokerCard key={broker.id} broker={broker} />)}

      <Pagination
        currentPage={page}
        totalPages={data?.totalPages}
        onPageChange={setPage}
        isLoading={isPlaceholderData}
      />
    </div>
  );
}
```

### Infinite Queries

```tsx
function SubmissionFeed() {
  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
  } = useInfiniteQuery({
    queryKey: ['submissions', 'feed'],
    queryFn: ({ pageParam = 1 }) => getSubmissions({ page: pageParam }),
    getNextPageParam: (lastPage) => lastPage.nextPage ?? undefined,
    initialPageParam: 1,
  });

  return (
    <div>
      {data?.pages.map((page) =>
        page.items.map((submission) => (
          <SubmissionCard key={submission.id} submission={submission} />
        ))
      )}

      {hasNextPage && (
        <Button
          onClick={() => fetchNextPage()}
          disabled={isFetchingNextPage}
        >
          {isFetchingNextPage ? 'Loading...' : 'Load More'}
        </Button>
      )}
    </div>
  );
}
```

---

## Mutations (Modifying Data)

### Basic Mutation

```tsx
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { createBroker } from '@/api/brokers';

function CreateBrokerDialog() {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: createBroker,
    onSuccess: () => {
      // Invalidate and refetch broker list
      queryClient.invalidateQueries({ queryKey: ['brokers'] });
      toast.success('Broker created successfully');
    },
    onError: (error) => {
      toast.error(error.message);
    },
  });

  const handleSubmit = (data: CreateBrokerRequest) => {
    mutation.mutate(data);
  };

  return (
    <Dialog>
      <BrokerForm
        onSubmit={handleSubmit}
        isSubmitting={mutation.isPending}
      />
    </Dialog>
  );
}
```

### Optimistic Updates

```tsx
function UpdateBrokerName({ brokerId }: { brokerId: string }) {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: (newName: string) => updateBroker(brokerId, { name: newName }),

    // Optimistic update: update UI immediately
    onMutate: async (newName) => {
      // Cancel outgoing refetches
      await queryClient.cancelQueries({ queryKey: ['brokers', brokerId] });

      // Snapshot previous value
      const previousBroker = queryClient.getQueryData(['brokers', brokerId]);

      // Optimistically update
      queryClient.setQueryData(['brokers', brokerId], (old: Broker) => ({
        ...old,
        name: newName,
      }));

      // Return context with snapshot
      return { previousBroker };
    },

    // Rollback on error
    onError: (err, newName, context) => {
      queryClient.setQueryData(
        ['brokers', brokerId],
        context?.previousBroker
      );
      toast.error('Failed to update broker name');
    },

    // Always refetch after success or error
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['brokers', brokerId] });
    },
  });

  return (
    <Input
      defaultValue={broker.name}
      onBlur={(e) => mutation.mutate(e.target.value)}
    />
  );
}
```

### Mutation with Cache Updates

```tsx
function DeleteBroker({ brokerId }: { brokerId: string }) {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: () => deleteBroker(brokerId),
    onSuccess: () => {
      // Option 1: Invalidate (refetch from server)
      queryClient.invalidateQueries({ queryKey: ['brokers'] });

      // Option 2: Update cache directly (faster, no refetch)
      queryClient.setQueryData(['brokers'], (old: Broker[]) =>
        old.filter(b => b.id !== brokerId)
      );

      // Remove individual broker from cache
      queryClient.removeQueries({ queryKey: ['brokers', brokerId] });
    },
  });

  return (
    <AlertDialog>
      <AlertDialogAction onClick={() => mutation.mutate()}>
        Delete
      </AlertDialogAction>
    </AlertDialog>
  );
}
```

---

## Error Handling

### Global Error Boundary

```tsx
import { QueryErrorResetBoundary } from '@tanstack/react-query';
import { ErrorBoundary } from 'react-error-boundary';

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <QueryErrorResetBoundary>
        {({ reset }) => (
          <ErrorBoundary
            onReset={reset}
            fallbackRender={({ error, resetErrorBoundary }) => (
              <div>
                <h1>Something went wrong</h1>
                <p>{error.message}</p>
                <Button onClick={resetErrorBoundary}>Try again</Button>
              </div>
            )}
          >
            <Router />
          </ErrorBoundary>
        )}
      </QueryErrorResetBoundary>
    </QueryClientProvider>
  );
}
```

### Query-Level Error Handling

```tsx
function BrokerDetails({ brokerId }: { brokerId: string }) {
  const { data, error, isError, refetch } = useQuery({
    queryKey: ['brokers', brokerId],
    queryFn: () => getBroker(brokerId),
    retry: (failureCount, error) => {
      // Don't retry on 404
      if (error.response?.status === 404) return false;
      // Retry up to 3 times for other errors
      return failureCount < 3;
    },
  });

  if (isError) {
    if (error.response?.status === 404) {
      return <NotFoundAlert message="Broker not found" />;
    }
    return (
      <ErrorAlert
        error={error}
        onRetry={refetch}
      />
    );
  }

  // ...
}
```

---

## Cache Invalidation Strategies

### After Mutations

```tsx
// Invalidate specific query
queryClient.invalidateQueries({ queryKey: ['brokers', brokerId] });

// Invalidate all broker queries
queryClient.invalidateQueries({ queryKey: ['brokers'] });

// Invalidate queries starting with 'brokers'
queryClient.invalidateQueries({ queryKey: ['brokers'], exact: false });
```

### Time-Based Invalidation

```tsx
useQuery({
  queryKey: ['brokers'],
  queryFn: getBrokers,
  staleTime: 1000 * 60 * 5, // Data is fresh for 5 minutes
  gcTime: 1000 * 60 * 30, // Cache for 30 minutes
});
```

### On Focus/Reconnect

```tsx
useQuery({
  queryKey: ['submissions'],
  queryFn: getSubmissions,
  refetchOnWindowFocus: true, // Refetch when window regains focus
  refetchOnReconnect: true, // Refetch when network reconnects
});
```

---

## Loading States

### Skeleton Screens

```tsx
function BrokerList() {
  const { data, isLoading } = useQuery({
    queryKey: ['brokers'],
    queryFn: getBrokers,
  });

  if (isLoading) {
    return (
      <div className="space-y-4">
        {Array.from({ length: 5 }).map((_, i) => (
          <Skeleton key={i} className="h-24 w-full" />
        ))}
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {data.map(broker => <BrokerCard key={broker.id} broker={broker} />)}
    </div>
  );
}
```

### Suspense (Experimental)

```tsx
import { useSuspenseQuery } from '@tanstack/react-query';

function BrokerDetails({ brokerId }: { brokerId: string }) {
  // No isLoading check needed - Suspense handles it
  const { data: broker } = useSuspenseQuery({
    queryKey: ['brokers', brokerId],
    queryFn: () => getBroker(brokerId),
  });

  return <BrokerCard broker={broker} />;
}

// Wrap with Suspense
function BrokerPage({ brokerId }: { brokerId: string }) {
  return (
    <Suspense fallback={<BrokerDetailsSkeleton />}>
      <BrokerDetails brokerId={brokerId} />
    </Suspense>
  );
}
```

---

## Prefetching

### On Hover

```tsx
function BrokerListItem({ broker }: { broker: Broker }) {
  const queryClient = useQueryClient();

  const handleMouseEnter = () => {
    queryClient.prefetchQuery({
      queryKey: ['brokers', broker.id],
      queryFn: () => getBroker(broker.id),
    });
  };

  return (
    <Link
      to={`/brokers/${broker.id}`}
      onMouseEnter={handleMouseEnter}
    >
      {broker.name}
    </Link>
  );
}
```

### On Route Load

```tsx
import { queryClient } from '@/lib/queryClient';
import { getBroker } from '@/api/brokers';

// In router loader
export const brokerLoader = async ({ params }: LoaderParams) => {
  await queryClient.ensureQueryData({
    queryKey: ['brokers', params.brokerId],
    queryFn: () => getBroker(params.brokerId!),
  });
  return null;
};
```

---

## TypeScript Integration

### Typed Query Hooks

```tsx
import { useQuery, UseQueryResult } from '@tanstack/react-query';

interface Broker {
  id: string;
  name: string;
  licenseNumber: string;
}

function useBroker(brokerId: string): UseQueryResult<Broker, Error> {
  return useQuery({
    queryKey: ['brokers', brokerId],
    queryFn: () => getBroker(brokerId),
  });
}

// Usage
function BrokerDetails({ brokerId }: { brokerId: string }) {
  const { data } = useBroker(brokerId); // data is typed as Broker | undefined
  // ...
}
```

### Query Key Factory with Types

```tsx
// src/lib/queryKeys.ts
export const brokerKeys = {
  all: ['brokers'] as const,
  lists: () => [...brokerKeys.all, 'list'] as const,
  list: (filters: BrokerFilters) => [...brokerKeys.lists(), filters] as const,
  details: () => [...brokerKeys.all, 'detail'] as const,
  detail: (id: string) => [...brokerKeys.details(), id] as const,
};

// TypeScript infers correct types
const key = brokerKeys.detail('123'); // readonly ['brokers', 'detail', '123']
```

---

## Best Practices

### 1. Use Query Keys Factories

✅ **GOOD:**
```tsx
const brokerKeys = {
  all: ['brokers'] as const,
  detail: (id: string) => [...brokerKeys.all, id] as const,
};

useQuery({ queryKey: brokerKeys.detail(brokerId), ... });
```

❌ **BAD:**
```tsx
useQuery({ queryKey: ['brokers', brokerId], ... });
```

### 2. Handle Loading and Error States

✅ **GOOD:**
```tsx
const { data, isLoading, error } = useQuery(...);

if (isLoading) return <Skeleton />;
if (error) return <ErrorAlert error={error} />;
return <BrokerCard broker={data} />;
```

❌ **BAD:**
```tsx
const { data } = useQuery(...);
return <BrokerCard broker={data} />; // data could be undefined!
```

### 3. Invalidate After Mutations

✅ **GOOD:**
```tsx
useMutation({
  mutationFn: createBroker,
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: ['brokers'] });
  },
});
```

❌ **BAD:**
```tsx
useMutation({
  mutationFn: createBroker,
  // No invalidation - UI won't update!
});
```

### 4. Use Optimistic Updates for Instant Feedback

✅ **GOOD:**
```tsx
onMutate: async (newData) => {
  await queryClient.cancelQueries({ queryKey: ['item', id] });
  const previous = queryClient.getQueryData(['item', id]);
  queryClient.setQueryData(['item', id], newData);
  return { previous };
},
```

### 5. Set Appropriate Stale Times

✅ **GOOD:**
```tsx
// Static data: long stale time
useQuery({
  queryKey: ['states'],
  queryFn: getStates,
  staleTime: Infinity, // Never becomes stale
});

// Real-time data: short stale time
useQuery({
  queryKey: ['submissions'],
  queryFn: getSubmissions,
  staleTime: 1000 * 10, // Stale after 10 seconds
});
```

### 6. Use Enabled Option for Conditional Queries

✅ **GOOD:**
```tsx
useQuery({
  queryKey: ['broker', brokerId],
  queryFn: () => getBroker(brokerId),
  enabled: !!brokerId, // Only run when brokerId exists
});
```

---

## Common Patterns for Nebula

### Broker 360 View

```tsx
function Broker360({ brokerId }: { brokerId: string }) {
  const broker = useQuery({
    queryKey: brokerKeys.detail(brokerId),
    queryFn: () => getBroker(brokerId),
  });

  const submissions = useQuery({
    queryKey: ['submissions', { brokerId }],
    queryFn: () => getSubmissions({ brokerId }),
    enabled: broker.isSuccess, // Wait for broker to load
  });

  const contacts = useQuery({
    queryKey: ['contacts', { brokerId }],
    queryFn: () => getContacts({ brokerId }),
    enabled: broker.isSuccess,
  });

  if (broker.isLoading) return <Skeleton />;
  if (broker.error) return <ErrorAlert error={broker.error} />;

  return (
    <div>
      <BrokerHeader broker={broker.data} />

      <Tabs>
        <TabsContent value="submissions">
          {submissions.isLoading ? (
            <Skeleton />
          ) : (
            <SubmissionsTable data={submissions.data} />
          )}
        </TabsContent>

        <TabsContent value="contacts">
          {contacts.isLoading ? (
            <Skeleton />
          ) : (
            <ContactsList data={contacts.data} />
          )}
        </TabsContent>
      </Tabs>
    </div>
  );
}
```

### Workflow State Transitions

```tsx
function useSubmissionTransition(submissionId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (toState: SubmissionState) =>
      transitionSubmission(submissionId, toState),
    onMutate: async (toState) => {
      // Optimistic update
      await queryClient.cancelQueries({ queryKey: ['submissions', submissionId] });
      const previous = queryClient.getQueryData(['submissions', submissionId]);

      queryClient.setQueryData(['submissions', submissionId], (old: Submission) => ({
        ...old,
        state: toState,
      }));

      return { previous };
    },
    onError: (err, toState, context) => {
      // Rollback
      queryClient.setQueryData(['submissions', submissionId], context?.previous);
      toast.error(`Failed to transition to ${toState}`);
    },
    onSuccess: () => {
      // Invalidate related queries
      queryClient.invalidateQueries({ queryKey: ['submissions'] });
      queryClient.invalidateQueries({ queryKey: ['timeline', submissionId] });
      toast.success('Submission updated');
    },
  });
}
```

---

## Debugging

### DevTools

```tsx
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';

<QueryClientProvider client={queryClient}>
  <App />
  <ReactQueryDevtools initialIsOpen={false} position="bottom-right" />
</QueryClientProvider>
```

### Logging

```tsx
const queryClient = new QueryClient({
  logger: {
    log: console.log,
    warn: console.warn,
    error: console.error,
  },
});
```

---

## References

- [TanStack Query Docs](https://tanstack.com/query/latest)
- [React Query Best Practices](https://tkdodo.eu/blog/practical-react-query)
- [Query Key Factories](https://tkdodo.eu/blog/effective-react-query-keys)
