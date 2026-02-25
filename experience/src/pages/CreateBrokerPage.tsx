import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { DashboardLayout } from '@/components/layout/DashboardLayout';
import { Card } from '@/components/ui/Card';
import { TextInput } from '@/components/ui/TextInput';
import { Select } from '@/components/ui/Select';
import { useCreateBroker } from '@/hooks/useCreateBroker';
import { validateBrokerCreate } from '@/lib/validation';
import { ApiError } from '@/services/api';
import { US_STATES } from '@/lib/us-states';
import type { BrokerCreateDto } from '@/types';

export default function CreateBrokerPage() {
  const navigate = useNavigate();
  const createBroker = useCreateBroker();

  const [form, setForm] = useState<Partial<BrokerCreateDto>>({
    legalName: '',
    licenseNumber: '',
    state: '',
    email: '',
    phone: '',
  });
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [serverError, setServerError] = useState('');

  function updateField(field: string, value: string) {
    setForm((prev) => ({ ...prev, [field]: value }));
    setErrors((prev) => {
      const next = { ...prev };
      delete next[field];
      return next;
    });
    setServerError('');
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();

    const validationErrors = validateBrokerCreate(form);
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    try {
      const broker = await createBroker.mutateAsync({
        legalName: form.legalName!.trim(),
        licenseNumber: form.licenseNumber!.trim(),
        state: form.state!,
        email: form.email?.trim() || undefined,
        phone: form.phone?.trim() || undefined,
      });
      navigate(`/brokers/${broker.id}`);
    } catch (err) {
      if (err instanceof ApiError && err.code === 'duplicate_license') {
        setErrors({ licenseNumber: 'A broker with this license number already exists.' });
      } else {
        setServerError('Unable to create broker. Please try again.');
      }
    }
  }

  return (
    <DashboardLayout>
      <div className="space-y-6">
        <Link
          to="/brokers"
          className="inline-flex items-center gap-1 text-xs text-zinc-500 hover:text-zinc-300"
        >
          <svg className="h-3 w-3" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M15 19l-7-7 7-7" />
          </svg>
          Brokers
        </Link>

        <h1 className="text-lg font-semibold text-zinc-200">New Broker</h1>

        <Card className="max-w-lg">
          <form onSubmit={handleSubmit} className="space-y-4">
            <TextInput
              label="Legal Name"
              required
              value={form.legalName ?? ''}
              onChange={(e) => updateField('legalName', e.target.value)}
              error={errors.legalName}
            />
            <TextInput
              label="License Number"
              required
              value={form.licenseNumber ?? ''}
              onChange={(e) => updateField('licenseNumber', e.target.value)}
              error={errors.licenseNumber}
            />
            <Select
              label="State"
              required
              value={form.state ?? ''}
              onChange={(e) => updateField('state', e.target.value)}
              error={errors.state}
              placeholder="Select state"
              options={US_STATES}
            />
            <TextInput
              label="Email"
              type="email"
              value={form.email ?? ''}
              onChange={(e) => updateField('email', e.target.value)}
              error={errors.email}
            />
            <TextInput
              label="Phone"
              type="tel"
              value={form.phone ?? ''}
              onChange={(e) => updateField('phone', e.target.value)}
              error={errors.phone}
              placeholder="+12025551234"
            />

            {serverError && (
              <p className="text-sm text-status-error">{serverError}</p>
            )}

            <div className="flex gap-3 pt-2">
              <button
                type="submit"
                disabled={createBroker.isPending}
                className="rounded-lg bg-nebula-violet px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-nebula-violet/90 disabled:opacity-50"
              >
                {createBroker.isPending ? 'Creating...' : 'Create Broker'}
              </button>
              <Link
                to="/brokers"
                className="rounded-lg bg-zinc-800 px-4 py-2 text-sm font-medium text-zinc-300 transition-colors hover:bg-zinc-700"
              >
                Cancel
              </Link>
            </div>
          </form>
        </Card>
      </div>
    </DashboardLayout>
  );
}
