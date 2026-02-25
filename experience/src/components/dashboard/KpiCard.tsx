import { Card } from '@/components/ui/Card';
import { Skeleton } from '@/components/ui/Skeleton';

interface KpiCardProps {
  label: string;
  value: string | null;
  isLoading: boolean;
}

export function KpiCard({ label, value, isLoading }: KpiCardProps) {
  return (
    <Card className="gradient-accent-top pt-6">
      <p className="text-xs font-medium uppercase tracking-wider text-zinc-500">{label}</p>
      {isLoading ? (
        <Skeleton className="mt-2 h-10 w-24" />
      ) : (
        <p className="mt-1 text-4xl font-bold tracking-tight text-zinc-50">
          {value ?? '—'}
        </p>
      )}
    </Card>
  );
}
