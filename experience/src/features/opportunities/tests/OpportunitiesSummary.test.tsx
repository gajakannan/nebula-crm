/**
 * Tests for OpportunitiesSummary (F0011)
 *
 * Covers:
 *   1. Renders connected opportunities flow and terminal outcomes rail
 *   2. Period selector buttons are present
 *   3. Shows loading skeleton while fetching
 *   4. Shows error fallback on fetch failure
 *   5. Stage cards and outcomes have accessible labels
 *   6. Secondary mini-view buttons switch details panel
 *
 * @vitest-environment jsdom
 */

import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, fireEvent, within } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import type { ReactNode } from 'react';

// Mock the hooks
const mockUseDashboardOpportunities = vi.fn();
vi.mock('../hooks/useDashboardOpportunities', () => ({
  useDashboardOpportunities: () => mockUseDashboardOpportunities(),
}));

const mockUseOpportunityAging = vi.fn();
vi.mock('../hooks/useOpportunityAging', () => ({
  useOpportunityAging: () => mockUseOpportunityAging(),
}));

const mockUseOpportunityHierarchy = vi.fn();
vi.mock('../hooks/useOpportunityHierarchy', () => ({
  useOpportunityHierarchy: () => mockUseOpportunityHierarchy(),
}));

const mockUseOpportunityOutcomes = vi.fn();
vi.mock('../hooks/useOpportunityOutcomes', () => ({
  useOpportunityOutcomes: () => mockUseOpportunityOutcomes(),
}));

// Mock child components that fetch their own data
vi.mock('./OpportunityPopover', () => ({
  OpportunityPopoverContent: () => <div data-testid="popover-content">Popover</div>,
}));
vi.mock('./OpportunityOutcomePopover', () => ({
  OpportunityOutcomePopoverContent: () => <div data-testid="outcome-popover-content">Outcome Popover</div>,
}));

import { OpportunitiesSummary } from '../components/OpportunitiesSummary';

function wrapper({ children }: { children: ReactNode }) {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  return (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );
}

const mockSummaryData = {
  submissions: [
    { status: 'Received', count: 5, colorGroup: 'intake' as const },
    { status: 'Triaging', count: 3, colorGroup: 'triage' as const },
  ],
  renewals: [
    { status: 'Created', count: 2, colorGroup: 'intake' as const },
  ],
};

const mockOutcomesData = {
  periodDays: 180,
  totalExits: 20,
  outcomes: [
    {
      key: 'bound',
      label: 'Bound',
      branchStyle: 'solid' as const,
      count: 10,
      percentOfTotal: 50,
      averageDaysToExit: 11.2,
    },
    {
      key: 'declined',
      label: 'Declined',
      branchStyle: 'red_dashed' as const,
      count: 5,
      percentOfTotal: 25,
      averageDaysToExit: 7.5,
    },
    {
      key: 'expired',
      label: 'Expired',
      branchStyle: 'gray_dotted' as const,
      count: 5,
      percentOfTotal: 25,
      averageDaysToExit: 16.4,
    },
  ],
};

describe('OpportunitiesSummary', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockUseDashboardOpportunities.mockReturnValue({
      data: mockSummaryData,
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    });
    mockUseOpportunityOutcomes.mockReturnValue({
      data: mockOutcomesData,
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    });
    mockUseOpportunityAging.mockReturnValue({
      data: null,
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    });
    mockUseOpportunityHierarchy.mockReturnValue({
      data: null,
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    });
  });

  it('renders opportunities flow and outcomes rail', () => {
    render(<OpportunitiesSummary />, { wrapper });

    expect(screen.getByText('Submissions')).toBeDefined();
    expect(screen.getByText('Renewals')).toBeDefined();
    expect(screen.getByText('Terminal Outcomes')).toBeDefined();
    expect(screen.getByText('Bound')).toBeDefined();
  });

  it('renders period selector buttons', () => {
    render(<OpportunitiesSummary />, { wrapper });

    expect(screen.getByText('30d')).toBeDefined();
    expect(screen.getByText('90d')).toBeDefined();
    expect(screen.getByText('180d')).toBeDefined();
    expect(screen.getByText('365d')).toBeDefined();
  });

  it('shows secondary insights section by default', () => {
    render(<OpportunitiesSummary />, { wrapper });

    expect(screen.getByText('Aging insights')).toBeDefined();
  });

  it('shows loading skeleton when data is loading', () => {
    mockUseDashboardOpportunities.mockReturnValue({
      data: undefined,
      isLoading: true,
      isError: false,
      refetch: vi.fn(),
    });

    render(<OpportunitiesSummary />, { wrapper });

    expect(screen.queryByText('Submissions')).toBeNull();
  });

  it('shows error fallback on fetch failure', () => {
    mockUseDashboardOpportunities.mockReturnValue({
      data: undefined,
      isLoading: false,
      isError: true,
      refetch: vi.fn(),
    });

    render(<OpportunitiesSummary />, { wrapper });

    expect(screen.getByText('Unable to load opportunities data')).toBeDefined();
  });

  it('pipeline board stage cards have accessible labels', () => {
    render(<OpportunitiesSummary />, { wrapper });

    const submissionsRegion = screen.getByRole('region', {
      name: 'Submissions opportunities flow',
    });
    const receivedCards = within(submissionsRegion).getAllByLabelText(
      'Received: 5 opportunities',
    );
    expect(receivedCards.length).toBeGreaterThan(0);
  });

  it('outcome rows have accessible labels', () => {
    render(<OpportunitiesSummary />, { wrapper });

    const boundOutcome = screen.getByLabelText('Bound: 10 exits, 50% of total');
    expect(boundOutcome).toBeDefined();
  });

  it('switches secondary mini-view panel', () => {
    render(<OpportunitiesSummary />, { wrapper });

    fireEvent.click(screen.getByRole('button', { name: 'Hierarchy' }));

    expect(screen.getByText('Hierarchy insights')).toBeDefined();
  });
});
