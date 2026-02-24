import { useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { DashboardLayout } from '@/components/layout/DashboardLayout';
import { Card } from '@/components/ui/Card';
import { Skeleton } from '@/components/ui/Skeleton';
import { ErrorFallback } from '@/components/ui/ErrorFallback';
import { Tabs } from '@/components/ui/Tabs';
import { BrokerProfileHeader } from '@/components/broker/BrokerProfileHeader';
import { BrokerProfileTab } from '@/components/broker/BrokerProfileTab';
import { BrokerContactsTab } from '@/components/broker/BrokerContactsTab';
import { BrokerTimelineTab } from '@/components/broker/BrokerTimelineTab';
import { EditBrokerModal } from '@/components/broker/EditBrokerModal';
import { DeactivateAction } from '@/components/broker/DeactivateAction';
import { DeleteBrokerAction } from '@/components/broker/DeleteBrokerAction';
import { ContactFormModal } from '@/components/broker/ContactFormModal';
import { DeleteContactAction } from '@/components/broker/DeleteContactAction';
import { useBroker } from '@/hooks/useBroker';
import { ApiError } from '@/services/api';
import type { ContactDto } from '@/types';

const TABS = ['Profile', 'Contacts', 'Timeline'];

export default function BrokerDetailPage() {
  const { brokerId } = useParams<{ brokerId: string }>();
  const { data: broker, isLoading, error, refetch } = useBroker(brokerId!);

  const [activeTab, setActiveTab] = useState('Profile');
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDeactivateDialog, setShowDeactivateDialog] = useState(false);
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);
  const [showContactForm, setShowContactForm] = useState(false);
  const [editingContact, setEditingContact] = useState<ContactDto | null>(null);
  const [deletingContact, setDeletingContact] = useState<ContactDto | null>(null);

  if (isLoading) {
    return (
      <DashboardLayout>
        <div className="space-y-4">
          <Skeleton className="h-16 w-full" />
          <Skeleton className="h-64 w-full" />
        </div>
      </DashboardLayout>
    );
  }

  if (error) {
    const apiError = error instanceof ApiError ? error : null;

    if (apiError?.status === 404) {
      return (
        <DashboardLayout>
          <div className="flex flex-col items-center justify-center py-16 text-center">
            <p className="text-sm text-zinc-400">Broker not found.</p>
            <Link
              to="/brokers"
              className="mt-3 text-sm text-nebula-violet hover:underline"
            >
              Back to broker list
            </Link>
          </div>
        </DashboardLayout>
      );
    }

    if (apiError?.status === 403) {
      return (
        <DashboardLayout>
          <div className="flex flex-col items-center justify-center py-16 text-center">
            <p className="text-sm text-zinc-400">
              You don't have permission to view this broker.
            </p>
            <Link
              to="/brokers"
              className="mt-3 text-sm text-nebula-violet hover:underline"
            >
              Back to broker list
            </Link>
          </div>
        </DashboardLayout>
      );
    }

    return (
      <DashboardLayout>
        <ErrorFallback message="Unable to load broker." onRetry={() => refetch()} />
      </DashboardLayout>
    );
  }

  if (!broker) return null;

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

        <Card>
          <BrokerProfileHeader
            broker={broker}
            onEdit={() => setShowEditModal(true)}
            onDeactivate={() => setShowDeactivateDialog(true)}
            onDelete={() => setShowDeleteDialog(true)}
          />
        </Card>

        <Card>
          <Tabs tabs={TABS} activeTab={activeTab} onTabChange={setActiveTab}>
            {activeTab === 'Profile' && <BrokerProfileTab broker={broker} />}
            {activeTab === 'Contacts' && (
              <BrokerContactsTab
                brokerId={broker.id}
                brokerStatus={broker.status}
                onAddContact={() => {
                  setEditingContact(null);
                  setShowContactForm(true);
                }}
                onEditContact={(contact) => {
                  setEditingContact(contact);
                  setShowContactForm(true);
                }}
                onDeleteContact={(contact) => setDeletingContact(contact)}
              />
            )}
            {activeTab === 'Timeline' && <BrokerTimelineTab brokerId={broker.id} />}
          </Tabs>
        </Card>
      </div>

      <EditBrokerModal
        broker={broker}
        open={showEditModal}
        onClose={() => setShowEditModal(false)}
      />

      <DeactivateAction
        broker={broker}
        open={showDeactivateDialog}
        onClose={() => setShowDeactivateDialog(false)}
      />

      <DeleteBrokerAction
        brokerId={broker.id}
        brokerName={broker.legalName}
        open={showDeleteDialog}
        onClose={() => setShowDeleteDialog(false)}
      />

      <ContactFormModal
        brokerId={broker.id}
        contact={editingContact}
        open={showContactForm}
        onClose={() => {
          setShowContactForm(false);
          setEditingContact(null);
        }}
      />

      <DeleteContactAction
        contact={deletingContact}
        brokerId={broker.id}
        open={!!deletingContact}
        onClose={() => setDeletingContact(null)}
      />
    </DashboardLayout>
  );
}
