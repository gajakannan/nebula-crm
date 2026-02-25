import { useDashboardPipeline } from '@/hooks/useDashboardPipeline';
import { Card, CardHeader, CardTitle } from '@/components/ui/Card';
import { Skeleton } from '@/components/ui/Skeleton';
import { ErrorFallback } from '@/components/ui/ErrorFallback';
import { PipelineChart } from './PipelineChart';

export function PipelineSummary() {
  const { data, isLoading, isError, refetch } = useDashboardPipeline();

  return (
    <Card>
      <CardHeader>
        <CardTitle>Pipeline</CardTitle>
      </CardHeader>

      {isLoading && (
        <div className="space-y-4">
          <Skeleton className="h-8 w-full" />
          <Skeleton className="h-8 w-full" />
        </div>
      )}

      {isError && (
        <ErrorFallback
          message="Unable to load pipeline data"
          onRetry={() => refetch()}
        />
      )}

      {data && (
        <div className="space-y-8">
          <PipelineChart
            label="Submissions"
            entityType="submission"
            statuses={data.submissions}
          />
          <PipelineChart
            label="Renewals"
            entityType="renewal"
            statuses={data.renewals}
          />
        </div>
      )}
    </Card>
  );
}
