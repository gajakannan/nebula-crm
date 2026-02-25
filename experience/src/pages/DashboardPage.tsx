import { DashboardLayout } from '@/components/layout/DashboardLayout';
import { NudgeCardsSection } from '@/components/dashboard/NudgeCardsSection';
import { KpiCardsRow } from '@/components/dashboard/KpiCardsRow';
import { PipelineSummary } from '@/components/dashboard/PipelineSummary';
import { MyTasksWidget } from '@/components/dashboard/MyTasksWidget';
import { ActivityFeed } from '@/components/dashboard/ActivityFeed';

export default function DashboardPage() {
  return (
    <DashboardLayout>
      <div className="space-y-6">
        <div>
          <h1 className="text-3xl font-bold tracking-tight" style={{ color: 'var(--text-primary)' }}>Dashboard</h1>
          <p className="mt-1 text-sm" style={{ color: 'var(--text-muted)' }}>Your pipeline at a glance</p>
        </div>
        <NudgeCardsSection />
        <KpiCardsRow />

        <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
          <div className="space-y-6 lg:col-span-2">
            <PipelineSummary />
            <MyTasksWidget />
          </div>
          <div>
            <ActivityFeed />
          </div>
        </div>
      </div>
    </DashboardLayout>
  );
}
