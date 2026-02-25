/**
 * Route registry for navigation degradation.
 * Add entries here as new pages are implemented.
 */
const REGISTERED_ROUTES: Record<string, (id: string) => string> = {
  'Broker': (id) => `/brokers/${id}`,
};

export function canNavigateTo(entityType: string): boolean {
  return entityType in REGISTERED_ROUTES;
}

export function getEntityPath(entityType: string, entityId: string): string | null {
  const builder = REGISTERED_ROUTES[entityType];
  return builder ? builder(entityId) : null;
}
