import { BrokerStatusBadge } from './BrokerStatusBadge';
import type { BrokerDto } from '../types';

interface BrokerProfileHeaderProps {
  broker: BrokerDto;
  onEdit: () => void;
  onDeactivate: () => void;
  onDelete: () => void;
}

export function BrokerProfileHeader({
  broker,
  onEdit,
  onDeactivate,
  onDelete,
}: BrokerProfileHeaderProps) {
  const isInactive = broker.status === 'Inactive';

  return (
    <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <div className="flex items-center gap-3">
          <h1 className="text-lg font-semibold text-text-primary">{broker.legalName}</h1>
          <BrokerStatusBadge status={broker.status} />
        </div>
        <p className="mt-1 font-mono text-xs text-text-muted">{broker.licenseNumber}</p>
      </div>
      <div className="flex gap-2">
        <button
          onClick={onEdit}
          className="rounded-lg border border-surface-border bg-surface-card px-3 py-1.5 text-xs font-medium text-text-secondary transition-colors hover:bg-surface-card-hover hover:text-text-primary"
        >
          Edit
        </button>
        <button
          onClick={onDeactivate}
          className="rounded-lg border border-surface-border bg-surface-card px-3 py-1.5 text-xs font-medium text-text-secondary transition-colors hover:bg-surface-card-hover hover:text-text-primary"
        >
          {isInactive ? 'Activate' : 'Deactivate'}
        </button>
        <button
          onClick={onDelete}
          className="rounded-lg bg-status-error/15 px-3 py-1.5 text-xs font-medium text-status-error transition-colors hover:bg-status-error/25"
        >
          Delete
        </button>
      </div>
    </div>
  );
}
