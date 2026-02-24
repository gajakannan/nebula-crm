import type { BrokerDto } from '@/types';

interface BrokerProfileTabProps {
  broker: BrokerDto;
}

export function BrokerProfileTab({ broker }: BrokerProfileTabProps) {
  return (
    <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <DetailField label="Legal Name" value={broker.legalName} />
      <DetailField label="License Number" value={broker.licenseNumber} />
      <DetailField label="State" value={broker.state} />
      <DetailField label="Status" value={broker.status} />
      <DetailField
        label="Email"
        value={broker.email}
        masked={broker.status === 'Inactive' && !broker.email}
      />
      <DetailField
        label="Phone"
        value={broker.phone}
        masked={broker.status === 'Inactive' && !broker.phone}
      />
    </div>
  );
}

function DetailField({
  label,
  value,
  masked = false,
}: {
  label: string;
  value: string | null;
  masked?: boolean;
}) {
  return (
    <div>
      <dt className="text-xs font-medium text-zinc-500">{label}</dt>
      <dd className="mt-1 text-sm text-zinc-200">
        {value ? (
          value
        ) : masked ? (
          <span className="text-zinc-600 italic">Masked</span>
        ) : (
          <span className="text-zinc-600">—</span>
        )}
      </dd>
    </div>
  );
}
