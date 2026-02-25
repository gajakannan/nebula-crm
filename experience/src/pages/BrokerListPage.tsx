import { useState } from 'react';
import { Link } from 'react-router-dom';
import { DashboardLayout } from '@/components/layout/DashboardLayout';
import { Card, CardHeader, CardTitle } from '@/components/ui/Card';
import { ErrorFallback } from '@/components/ui/ErrorFallback';
import { Skeleton } from '@/components/ui/Skeleton';
import { BrokerStatusBadge } from '@/components/broker/BrokerStatusBadge';
import { useBrokers } from '@/hooks/useBrokers';
import { useDebounce } from '@/hooks/useDebounce';
import type { BrokerDto, BrokerStatus } from '@/types';

const STATUS_OPTIONS = ['All', 'Active', 'Inactive', 'Pending'] as const;

export default function BrokerListPage() {
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('All');
  const [page, setPage] = useState(1);
  const debouncedSearch = useDebounce(search);

  const { data, isLoading, isError, refetch } = useBrokers({
    q: debouncedSearch,
    status: statusFilter,
    page,
  });

  return (
    <DashboardLayout>
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <h1 className="text-lg font-semibold text-zinc-200">Brokers</h1>
          <Link
            to="/brokers/new"
            className="rounded-lg bg-nebula-violet px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-nebula-violet/90"
          >
            New Broker
          </Link>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>Broker Directory</CardTitle>
          </CardHeader>

          <div className="mb-4 flex flex-col gap-3 sm:flex-row">
            <input
              type="text"
              placeholder="Search by name or license..."
              value={search}
              onChange={(e) => {
                setSearch(e.target.value);
                setPage(1);
              }}
              className="flex-1 rounded-lg border border-zinc-700 bg-zinc-950 px-3 py-2 text-sm text-zinc-200 placeholder-zinc-600 focus:border-nebula-violet focus:outline-none focus:ring-1 focus:ring-nebula-violet"
            />
            <select
              value={statusFilter}
              onChange={(e) => {
                setStatusFilter(e.target.value);
                setPage(1);
              }}
              className="rounded-lg border border-zinc-700 bg-zinc-950 px-3 py-2 text-sm text-zinc-200 focus:border-nebula-violet focus:outline-none focus:ring-1 focus:ring-nebula-violet"
            >
              {STATUS_OPTIONS.map((s) => (
                <option key={s} value={s}>
                  {s}
                </option>
              ))}
            </select>
          </div>

          {isLoading && <BrokerListSkeleton />}
          {isError && (
            <ErrorFallback
              message="Unable to load brokers."
              onRetry={() => refetch()}
            />
          )}
          {data && data.data.length === 0 && (
            <div className="py-8 text-center text-sm text-zinc-500">
              No brokers found.
              {(debouncedSearch || statusFilter !== 'All') && (
                <button
                  onClick={() => {
                    setSearch('');
                    setStatusFilter('All');
                    setPage(1);
                  }}
                  className="ml-2 text-nebula-violet hover:underline"
                >
                  Clear filters
                </button>
              )}
            </div>
          )}
          {data && data.data.length > 0 && (
            <>
              {/* Desktop table */}
              <div className="hidden md:block">
                <table className="w-full text-sm">
                  <thead>
                    <tr className="border-b border-zinc-800 text-left text-xs font-medium uppercase tracking-wider text-zinc-500">
                      <th className="pb-3 pr-4">Name</th>
                      <th className="pb-3 pr-4">License</th>
                      <th className="pb-3 pr-4">State</th>
                      <th className="pb-3 pr-4">Status</th>
                      <th className="pb-3 pr-4">Email</th>
                      <th className="pb-3">Phone</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-zinc-800">
                    {data.data.map((broker) => (
                      <BrokerRow key={broker.id} broker={broker} />
                    ))}
                  </tbody>
                </table>
              </div>

              {/* Mobile cards */}
              <div className="space-y-3 md:hidden">
                {data.data.map((broker) => (
                  <BrokerMobileCard key={broker.id} broker={broker} />
                ))}
              </div>

              {/* Pagination */}
              {data.totalPages > 1 && (
                <div className="mt-4 flex items-center justify-between border-t border-zinc-800 pt-4">
                  <button
                    onClick={() => setPage((p) => Math.max(1, p - 1))}
                    disabled={page <= 1}
                    className="rounded-lg bg-zinc-800 px-3 py-1.5 text-xs font-medium text-zinc-300 transition-colors hover:bg-zinc-700 disabled:opacity-50 disabled:hover:bg-zinc-800"
                  >
                    Previous
                  </button>
                  <span className="text-xs text-zinc-500">
                    Page {data.page} of {data.totalPages}
                  </span>
                  <button
                    onClick={() => setPage((p) => Math.min(data.totalPages, p + 1))}
                    disabled={page >= data.totalPages}
                    className="rounded-lg bg-zinc-800 px-3 py-1.5 text-xs font-medium text-zinc-300 transition-colors hover:bg-zinc-700 disabled:opacity-50 disabled:hover:bg-zinc-800"
                  >
                    Next
                  </button>
                </div>
              )}
            </>
          )}
        </Card>
      </div>
    </DashboardLayout>
  );
}

function BrokerRow({ broker }: { broker: BrokerDto }) {
  return (
    <tr className="text-zinc-300">
      <td className="py-3 pr-4">
        <Link
          to={`/brokers/${broker.id}`}
          className="font-medium text-zinc-200 hover:text-nebula-violet"
        >
          {broker.legalName}
        </Link>
      </td>
      <td className="py-3 pr-4 font-mono text-xs">{broker.licenseNumber}</td>
      <td className="py-3 pr-4">{broker.state}</td>
      <td className="py-3 pr-4">
        <BrokerStatusBadge status={broker.status} />
      </td>
      <td className="py-3 pr-4">
        <MaskedField value={broker.email} status={broker.status} />
      </td>
      <td className="py-3">
        <MaskedField value={broker.phone} status={broker.status} />
      </td>
    </tr>
  );
}

function BrokerMobileCard({ broker }: { broker: BrokerDto }) {
  return (
    <Link
      to={`/brokers/${broker.id}`}
      className="block rounded-lg border border-zinc-800 p-4 transition-colors hover:border-zinc-700"
    >
      <div className="flex items-start justify-between">
        <div>
          <p className="font-medium text-zinc-200">{broker.legalName}</p>
          <p className="mt-0.5 font-mono text-xs text-zinc-500">{broker.licenseNumber}</p>
        </div>
        <BrokerStatusBadge status={broker.status} />
      </div>
      <div className="mt-2 flex gap-4 text-xs text-zinc-400">
        <span>{broker.state}</span>
        <MaskedField value={broker.email} status={broker.status} />
      </div>
    </Link>
  );
}

function MaskedField({ value, status }: { value: string | null; status: BrokerStatus }) {
  if (value) return <span>{value}</span>;
  if (status === 'Inactive') return <span className="text-zinc-600 italic">Masked</span>;
  return <span className="text-zinc-600">—</span>;
}

function BrokerListSkeleton() {
  return (
    <div className="space-y-3">
      {Array.from({ length: 5 }).map((_, i) => (
        <Skeleton key={i} className="h-12 w-full" />
      ))}
    </div>
  );
}
