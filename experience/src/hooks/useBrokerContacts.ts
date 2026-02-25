import { useQuery } from '@tanstack/react-query';
import { api } from '@/services/api';
import type { ContactDto } from '@/types';

export function useBrokerContacts(brokerId: string) {
  return useQuery({
    queryKey: ['contacts', brokerId],
    queryFn: () => api.get<ContactDto[]>(`/contacts?brokerId=${brokerId}`),
    enabled: !!brokerId,
  });
}
