import { useQuery } from '@tanstack/react-query';
import { api } from '@/services/api';
import type { AccountReferenceDto, ProgramReferenceDto } from '../types';

export function useAccounts() {
  return useQuery({
    queryKey: ['referenceData', 'accounts'],
    queryFn: () => api.get<AccountReferenceDto[]>('/accounts'),
  });
}

export function usePrograms() {
  return useQuery({
    queryKey: ['referenceData', 'programs'],
    queryFn: () => api.get<ProgramReferenceDto[]>('/programs'),
  });
}
