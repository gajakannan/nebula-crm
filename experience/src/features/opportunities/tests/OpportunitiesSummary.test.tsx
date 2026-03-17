/**
 * @vitest-environment jsdom
 */

import { describe, it, expect, vi, beforeEach } from 'vitest';
import { fireEvent, render, screen, waitFor } from '@testing-library/react';

const mockUseOpportunityFlow = vi.fn();
vi.mock('../hooks/useOpportunityFlow', () => ({
  useOpportunityFlow: (...args: unknown[]) => mockUseOpportunityFlow(...args),
}));

const mockUseOpportunityOutcomes = vi.fn();
vi.mock('../hooks/useOpportunityOutcomes', () => ({
  useOpportunityOutcomes: (...args: unknown[]) => mockUseOpportunityOutcomes(...args),
}));

const mockUseOpportunityAging = vi.fn();
vi.mock('../hooks/useOpportunityAging', () => ({
  useOpportunityAging: (...args: unknown[]) => mockUseOpportunityAging(...args),
}));

const mockUseOpportunityBreakdown = vi.fn();
vi.mock('../hooks/useOpportunityBreakdown', () => ({
  useOpportunityBreakdown: (...args: unknown[]) => mockUseOpportunityBreakdown(...args),
}));

const mockUseDashboardKpis = vi.fn();
vi.mock('@/features/kpis/hooks/useDashboardKpis', () => ({
  useDashboardKpis: (...args: unknown[]) => mockUseDashboardKpis(...args),
}));

import { OpportunitiesSummary } from '../components/OpportunitiesSummary';

const flowDto = {
  entityType: 'submission' as const,
  periodDays: 180,
  windowStartUtc: '2026-01-01T00:00:00Z',
  windowEndUtc: '2026-03-01T00:00:00Z',
  nodes: [
    {
      status: 'Received',
      label: 'Received',
      isTerminal: false,
      displayOrder: 1,
      colorGroup: 'intake' as const,
      currentCount: 10,
      inflowCount: 0,
      outflowCount: 7,
      avgDwellDays: 2.1,
      emphasis: 'normal' as const,
    },
    {
      status: 'Triaging',
      label: 'Triaging',
      isTerminal: false,
      displayOrder: 2,
      colorGroup: 'triage' as const,
      currentCount: 7,
      inflowCount: 8,
      outflowCount: 5,
      avgDwellDays: 5.4,
      emphasis: 'bottleneck' as const,
    },
    {
      status: 'InReview',
      label: 'In Review',
      isTerminal: false,
      displayOrder: 3,
      colorGroup: 'review' as const,
      currentCount: 4,
      inflowCount: 5,
      outflowCount: 4,
      avgDwellDays: 8.0,
      emphasis: 'blocked' as const,
    },
    {
      status: 'Bound',
      label: 'Bound',
      isTerminal: true,
      displayOrder: 4,
      colorGroup: 'decision' as const,
      currentCount: 15,
      inflowCount: 5,
      outflowCount: 0,
      avgDwellDays: null,
      emphasis: null,
    },
  ],
  links: [
    { sourceStatus: 'Received', targetStatus: 'Triaging', count: 12 },
    { sourceStatus: 'Triaging', targetStatus: 'InReview', count: 8 },
  ],
};

const outcomesDto = {
  periodDays: 180,
  totalExits: 20,
  outcomes: [
    {
      key: 'bound',
      label: 'Bound',
      branchStyle: 'solid' as const,
      count: 12,
      percentOfTotal: 60,
      averageDaysToExit: 7.2,
    },
    {
      key: 'declined',
      label: 'Declined',
      branchStyle: 'red_dashed' as const,
      count: 8,
      percentOfTotal: 40,
      averageDaysToExit: 5.8,
    },
  ],
};

describe('OpportunitiesSummary', () => {
  beforeEach(() => {
    vi.clearAllMocks();

    mockUseOpportunityFlow.mockReturnValue({
      data: flowDto,
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    });
    mockUseOpportunityOutcomes.mockReturnValue({
      data: outcomesDto,
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    });
    mockUseOpportunityAging.mockReturnValue({
      data: {
        entityType: 'submission',
        periodDays: 180,
        statuses: [
          {
            status: 'Triaging',
            label: 'Triaging',
            colorGroup: 'triage',
            displayOrder: 2,
            sla: {
              warningDays: 2,
              targetDays: 5,
              totalCount: 7,
              onTimeCount: 3,
              approachingCount: 2,
              overdueCount: 2,
            },
            buckets: [],
            total: 7,
          },
        ],
      },
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    });
    mockUseOpportunityBreakdown.mockReturnValue({
      data: {
        entityType: 'submission',
        status: 'Received',
        groupBy: 'lineOfBusiness',
        periodDays: 180,
        groups: [
          { key: 'property', label: 'Property', count: 6 },
          { key: 'casualty', label: 'Casualty', count: 4 },
        ],
        total: 10,
      },
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    });
    mockUseDashboardKpis.mockReturnValue({
      data: {
        activeBrokers: 10,
        openSubmissions: 7,
        renewalRate: 0.42,
        avgTurnaroundDays: 8.3,
      },
      isLoading: false,
      isError: false,
    });
  });

  it('renders chapter controls and connected flow canvas', () => {
    render(<OpportunitiesSummary />);

    expect(screen.getByRole('tab', { name: 'Flow' })).toBeTruthy();
    expect(screen.getByRole('tab', { name: 'Friction' })).toBeTruthy();
    expect(screen.getByRole('tab', { name: 'Outcomes' })).toBeTruthy();
    expect(screen.queryByRole('tab', { name: 'Aging' })).toBeNull();
    expect(screen.queryByRole('tab', { name: 'Mix' })).toBeNull();

    expect(screen.getByRole('button', { name: 'Received stage, 10 opportunities' })).toBeTruthy();
    expect(screen.getByRole('button', { name: /Bound outcome, 12 exits/i })).toBeTruthy();
    expect(screen.getByText(/2d warning/i)).toBeTruthy();
  });

  it('passes periodDays to KPI hook', () => {
    render(<OpportunitiesSummary />);

    expect(mockUseDashboardKpis).toHaveBeenCalledWith(180);
    fireEvent.click(screen.getByRole('tab', { name: '30d' }));
    expect(mockUseDashboardKpis).toHaveBeenLastCalledWith(30);
  });

  it('supports keyboard chapter navigation with arrow keys', () => {
    render(<OpportunitiesSummary />);

    const flowTab = screen.getByRole('tab', { name: 'Flow' });
    flowTab.focus();
    fireEvent.keyDown(flowTab, { key: 'ArrowRight' });
    expect(screen.getByRole('tab', { name: 'Friction' })).toHaveAttribute('aria-selected', 'true');

    const frictionTab = screen.getByRole('tab', { name: 'Friction' });
    fireEvent.keyDown(frictionTab, { key: 'ArrowRight' });
    expect(screen.getByRole('tab', { name: 'Outcomes' })).toHaveAttribute('aria-selected', 'true');

    const outcomesTab = screen.getByRole('tab', { name: 'Outcomes' });
    fireEvent.keyDown(outcomesTab, { key: 'ArrowLeft' });
    expect(screen.getByRole('tab', { name: 'Friction' })).toHaveAttribute('aria-selected', 'true');
  });

  it('supports arrow-key navigation across stage nodes', () => {
    render(<OpportunitiesSummary />);

    const received = screen.getByRole('button', { name: /Received stage, 10 opportunities/i });
    const triaging = screen.getByRole('button', { name: /Triaging stage, 7 opportunities/i });
    received.focus();

    fireEvent.keyDown(received, { key: 'ArrowDown' });
    expect(triaging).toHaveFocus();

    fireEvent.keyDown(triaging, { key: 'ArrowUp' });
    expect(received).toHaveFocus();
  });

  it('hides per-stage alternate toggles during chapter override and restores in flow', async () => {
    render(<OpportunitiesSummary />);
    expect(screen.getByRole('button', { name: /cycle received mini visualization/i })).toBeTruthy();

    fireEvent.click(screen.getByRole('tab', { name: 'Friction' }));
    expect(screen.queryByRole('button', { name: /cycle received mini visualization/i })).toBeNull();

    fireEvent.click(screen.getByRole('tab', { name: 'Outcomes' }));
    expect(screen.queryByRole('button', { name: /cycle received mini visualization/i })).toBeNull();

    fireEvent.click(screen.getByRole('tab', { name: 'Flow' }));

    await waitFor(() => {
      expect(screen.getByRole('button', { name: /cycle received mini visualization/i })).toBeTruthy();
    });
  });

  it('restores the previously selected flow mini-view after chapter overrides', async () => {
    render(<OpportunitiesSummary />);

    const cycleButton = screen.getByRole('button', { name: /cycle received mini visualization/i });
    fireEvent.click(cycleButton);
    expect(screen.getByText('Top brokers')).toBeTruthy();

    fireEvent.click(screen.getByRole('tab', { name: 'Friction' }));
    fireEvent.click(screen.getByRole('tab', { name: 'Outcomes' }));
    fireEvent.click(screen.getByRole('tab', { name: 'Flow' }));

    await waitFor(() => {
      expect(screen.getByText('Top brokers')).toBeTruthy();
    });
  });

  it('enables breakdown requests lazily for the selected stage mini-view', async () => {
    render(<OpportunitiesSummary />);

    await waitFor(() => {
      const receivedLobEnabled = mockUseOpportunityBreakdown.mock.calls.some((call) => {
        const [, status, groupBy, , options] = call;
        return status === 'Received' && groupBy === 'lineOfBusiness' && (options as { enabled?: boolean }).enabled === true;
      });
      expect(receivedLobEnabled).toBe(true);
    });

    const receivedBrokerEnabledBeforeCycle = mockUseOpportunityBreakdown.mock.calls.some((call) => {
      const [, status, groupBy, , options] = call;
      return status === 'Received' && groupBy === 'broker' && (options as { enabled?: boolean }).enabled === true;
    });
    expect(receivedBrokerEnabledBeforeCycle).toBe(false);

    fireEvent.click(screen.getByRole('button', { name: /cycle received mini visualization/i }));

    await waitFor(() => {
      const receivedBrokerEnabled = mockUseOpportunityBreakdown.mock.calls.some((call) => {
        const [, status, groupBy, , options] = call;
        return status === 'Received' && groupBy === 'broker' && (options as { enabled?: boolean }).enabled === true;
      });
      expect(receivedBrokerEnabled).toBe(true);
    });
  });

  it('shows flow error fallback', () => {
    mockUseOpportunityFlow.mockReturnValue({
      data: undefined,
      isLoading: false,
      isError: true,
      refetch: vi.fn(),
    });

    render(<OpportunitiesSummary />);

    expect(screen.getByText('Unable to load opportunity flow')).toBeTruthy();
  });

  it('renders empty timeline state with no popovers when all stage counts are zero', () => {
    mockUseOpportunityFlow.mockReturnValue({
      data: {
        ...flowDto,
        nodes: flowDto.nodes.map((node) => ({
          ...node,
          currentCount: 0,
          inflowCount: 0,
          outflowCount: 0,
        })),
        links: flowDto.links.map((link) => ({ ...link, count: 0 })),
      },
      isLoading: false,
      isError: false,
      refetch: vi.fn(),
    });

    render(<OpportunitiesSummary />);

    expect(screen.getByText('No activity in period')).toBeTruthy();
    const received = screen.getByRole('button', { name: /Received stage, 0 opportunities/i });
    fireEvent.click(received);
    expect(screen.queryByRole('dialog')).toBeNull();
  });
});
