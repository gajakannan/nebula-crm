import { useDashboardKpis } from '../hooks/useDashboardKpis';
import { formatPercent } from '@/lib/format';
import { KpiCard } from './KpiCard';

export function KpiCardsRow() {
  const { data, isLoading, isError } = useDashboardKpis();

  if (isError) {
    // Show dashes for all KPIs on error — silent degradation per plan
  }

  const kpis = [
    {
      label: 'Active Brokers',
      value: data ? String(data.activeBrokers) : null,
    },
    {
      label: 'Open Submissions',
      value: data ? String(data.openSubmissions) : null,
    },
    {
      label: 'Renewal Rate',
      value: data?.renewalRate != null ? formatPercent(data.renewalRate) : null,
    },
    {
      label: 'Avg Turnaround',
      value:
        data?.avgTurnaroundDays != null
          ? `${data.avgTurnaroundDays.toFixed(1)} days`
          : null,
    },
  ];

  return (
    <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
      {kpis.map((kpi) => (
        <KpiCard
          key={kpi.label}
          label={kpi.label}
          value={kpi.value}
          isLoading={isLoading}
        />
      ))}
    </div>
  );
}
