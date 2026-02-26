import { DashboardLayout } from '@/components/layout/DashboardLayout';
import { NudgeCardsSection } from '@/features/nudges';
import { KpiCardsRow } from '@/features/kpis';
import { OpportunitiesSummary } from '@/features/opportunities';
import { MyTasksWidget } from '@/features/tasks';
import { ActivityFeed } from '@/features/timeline';

export default function DashboardPage() {
  return (
    <DashboardLayout title="Dashboard">
      <div className="space-y-6">
        <p className="text-sm text-text-muted">Your opportunities at a glance</p>
        <NudgeCardsSection />
        <KpiCardsRow />

        <div className="grid grid-cols-1 gap-6 xl:grid-cols-[minmax(0,1fr)_17.5rem] xl:items-stretch 2xl:grid-cols-[minmax(0,1fr)_18.5rem]">
          <div className="xl:h-full xl:min-h-0">
            <OpportunitiesSummary />
          </div>
          <div className="xl:h-full xl:min-h-0">
            <ActivityFeed />
          </div>
        </div>

        <MyTasksWidget />
      </div>
    </DashboardLayout>
  );
}
