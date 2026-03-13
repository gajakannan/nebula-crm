import { useQuery } from '@tanstack/react-query';
import { api } from '@/services/api';
import type { OpportunityItemsDto } from '../types';

export function useOpportunityOutcomeItems(
  outcomeKey: string,
  periodDays: number,
  enabled: boolean,
) {
  return useQuery({
    queryKey: ['dashboard', 'opportunities', 'outcomes', outcomeKey, 'items', periodDays],
    queryFn: () =>
      api.get<OpportunityItemsDto>(
        `/dashboard/opportunities/outcomes/${encodeURIComponent(outcomeKey)}/items?periodDays=${periodDays}`,
      ),
    enabled,
  });
}

