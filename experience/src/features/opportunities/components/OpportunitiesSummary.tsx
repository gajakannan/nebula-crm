import { useState } from 'react';
import { useDashboardOpportunities } from '../hooks/useDashboardOpportunities';
import { Card, CardHeader, CardTitle } from '@/components/ui/Card';
import { Skeleton } from '@/components/ui/Skeleton';
import { ErrorFallback } from '@/components/ui/ErrorFallback';
import { OpportunityChart } from './OpportunityChart';

const FLOW_WINDOWS = [30, 90, 180, 365] as const;

export function OpportunitiesSummary() {
  const [periodDays, setPeriodDays] = useState<(typeof FLOW_WINDOWS)[number]>(180);
  const { data, isLoading, isError, refetch } = useDashboardOpportunities();

  return (
    <Card className="flex h-full min-h-0 flex-col overflow-hidden">
      <CardHeader>
        <CardTitle>Opportunities</CardTitle>
        <div
          className="inline-flex items-center gap-1 rounded-lg border border-border-muted bg-surface-panel p-1"
          role="tablist"
          aria-label="Opportunity flow window"
        >
          {FLOW_WINDOWS.map((days) => {
            const active = days === periodDays;
            return (
              <button
                key={days}
                type="button"
                role="tab"
                aria-selected={active}
                onClick={() => setPeriodDays(days)}
                className={
                  active
                    ? 'rounded-full border border-nebula-violet/45 bg-nebula-violet/15 px-2.5 py-1 text-xs font-semibold text-nebula-violet shadow-sm'
                    : 'rounded-full px-2.5 py-1 text-xs text-text-muted hover:text-text-secondary'
                }
              >
                {days}d
              </button>
            );
          })}
        </div>
      </CardHeader>

      {isLoading && (
        <div className="min-h-0 flex-1 space-y-4 overflow-y-auto pr-1">
          <Skeleton className="h-8 w-full" />
          <Skeleton className="h-8 w-full" />
        </div>
      )}

      {isError && (
        <div className="min-h-0 flex-1 overflow-y-auto pr-1">
          <ErrorFallback
            message="Unable to load opportunities data"
            onRetry={() => refetch()}
          />
        </div>
      )}

      {data && (
        <div className="timeline-scrollbar min-h-0 flex-1 space-y-8 overflow-y-auto pr-1">
          <OpportunityChart
            label="Submissions"
            entityType="submission"
            statuses={data.submissions}
            periodDays={periodDays}
          />
          <OpportunityChart
            label="Renewals"
            entityType="renewal"
            statuses={data.renewals}
            periodDays={periodDays}
          />
        </div>
      )}
    </Card>
  );
}
