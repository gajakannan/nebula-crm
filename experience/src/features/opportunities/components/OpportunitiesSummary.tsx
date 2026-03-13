import { useState } from 'react';
import { useDashboardOpportunities } from '../hooks/useDashboardOpportunities';
import { useOpportunityOutcomes } from '../hooks/useOpportunityOutcomes';
import { Card, CardHeader, CardTitle } from '@/components/ui/Card';
import { Skeleton } from '@/components/ui/Skeleton';
import { ErrorFallback } from '@/components/ui/ErrorFallback';
import { OpportunityPipelineBoard } from './OpportunityPipelineBoard';
import { OpportunityHeatmap } from './OpportunityHeatmap';
import { OpportunityTreemap } from './OpportunityTreemap';
import { OpportunitySunburst } from './OpportunitySunburst';
import { OpportunityOutcomesRail } from './OpportunityOutcomesRail';

const FLOW_WINDOWS = [30, 90, 180, 365] as const;
const MINI_VIEWS = [
  { key: 'aging', label: 'Aging' },
  { key: 'hierarchy', label: 'Hierarchy' },
  { key: 'radial', label: 'Radial' },
] as const;

type MiniView = (typeof MINI_VIEWS)[number]['key'];

export function OpportunitiesSummary() {
  const [periodDays, setPeriodDays] = useState<(typeof FLOW_WINDOWS)[number]>(180);
  const [miniView, setMiniView] = useState<MiniView>('aging');
  const { data, isLoading, isError, refetch } = useDashboardOpportunities(periodDays);
  const {
    data: outcomesData,
    isLoading: outcomesLoading,
    isError: outcomesError,
    refetch: refetchOutcomes,
  } = useOpportunityOutcomes(periodDays);

  return (
    <Card className="flex flex-col overflow-hidden">
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
        <div className="space-y-4 px-4 pb-4">
          <Skeleton className="h-8 w-full" />
          <Skeleton className="h-8 w-full" />
          <Skeleton className="h-28 w-full" />
        </div>
      )}

      {isError && (
        <div className="px-4 pb-4">
          <ErrorFallback
            message="Unable to load opportunities data"
            onRetry={() => refetch()}
          />
        </div>
      )}

      {data && (
        <div className="space-y-4 px-4 pb-4">
          <div className="grid gap-4 xl:grid-cols-[2fr_1fr]">
            <div className="space-y-5 rounded-lg border border-border-muted bg-surface-main/45 p-3">
              <OpportunityPipelineBoard
                label="Submissions"
                entityType="submission"
                statuses={data.submissions}
              />
              <OpportunityPipelineBoard
                label="Renewals"
                entityType="renewal"
                statuses={data.renewals}
              />
            </div>
            <div>
              {outcomesLoading && <Skeleton className="h-48 w-full" />}
              {outcomesError && (
                <ErrorFallback
                  message="Unable to load terminal outcomes"
                  onRetry={() => refetchOutcomes()}
                />
              )}
              {outcomesData && (
                <OpportunityOutcomesRail
                  outcomes={outcomesData.outcomes}
                  periodDays={periodDays}
                />
              )}
            </div>
          </div>

          <section
            aria-label="Secondary opportunity insights"
            className="rounded-lg border border-border-muted bg-surface-main/45 p-3"
          >
            <div className="mb-3 flex flex-wrap items-center gap-2">
              {MINI_VIEWS.map((view) => {
                const active = miniView === view.key;
                return (
                  <button
                    key={view.key}
                    type="button"
                    aria-pressed={active}
                    onClick={() => setMiniView(view.key)}
                    className={
                      active
                        ? 'rounded-md border border-nebula-violet/45 bg-nebula-violet/15 px-3 py-1.5 text-xs font-semibold text-nebula-violet'
                        : 'rounded-md border border-border-muted px-3 py-1.5 text-xs text-text-muted hover:text-text-secondary'
                    }
                  >
                    {view.label}
                  </button>
                );
              })}
            </div>

            {miniView === 'aging' && (
              <div className="space-y-4">
                <p className="text-xs text-text-muted">Aging insights</p>
                <OpportunityHeatmap
                  entityType="submission"
                  periodDays={periodDays}
                  label="Submissions"
                />
                <OpportunityHeatmap
                  entityType="renewal"
                  periodDays={periodDays}
                  label="Renewals"
                />
              </div>
            )}

            {miniView === 'hierarchy' && (
              <div className="space-y-2">
                <p className="text-xs text-text-muted">Hierarchy insights</p>
                <OpportunityTreemap periodDays={periodDays} />
              </div>
            )}

            {miniView === 'radial' && (
              <div className="space-y-2">
                <p className="text-xs text-text-muted">Radial insights</p>
                <OpportunitySunburst periodDays={periodDays} />
              </div>
            )}
          </section>
        </div>
      )}
    </Card>
  );
}
