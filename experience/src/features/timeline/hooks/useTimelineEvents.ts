import { useQuery } from '@tanstack/react-query';
import { api } from '@/services/api';
import type { TimelineEventDto } from '@/contracts/timeline';

export function useTimelineEvents() {
  return useQuery({
    queryKey: ['timeline', 'events'],
    queryFn: () =>
      api.get<TimelineEventDto[]>('/timeline/events?entityType=Broker&limit=12'),
  });
}
