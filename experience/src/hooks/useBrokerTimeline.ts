import { useQuery } from '@tanstack/react-query';
import { api } from '@/services/api';
import type { TimelineEventDto } from '@/types';

export function useBrokerTimeline(brokerId: string) {
  return useQuery({
    queryKey: ['timeline', 'broker', brokerId],
    queryFn: () =>
      api.get<TimelineEventDto[]>(
        `/timeline/events?entityType=Broker&entityId=${brokerId}&limit=50`,
      ),
    enabled: !!brokerId,
  });
}
