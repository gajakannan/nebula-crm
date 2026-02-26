import type { OpportunityColorGroup } from '../types';

const bgClasses: Record<OpportunityColorGroup, string> = {
  intake: 'bg-opportunity-intake',
  triage: 'bg-opportunity-triage',
  waiting: 'bg-opportunity-waiting',
  review: 'bg-opportunity-review',
  decision: 'bg-opportunity-decision',
};

const textClasses: Record<OpportunityColorGroup, string> = {
  intake: 'text-opportunity-intake',
  triage: 'text-opportunity-triage',
  waiting: 'text-opportunity-waiting',
  review: 'text-opportunity-review',
  decision: 'text-opportunity-decision',
};

const borderClasses: Record<OpportunityColorGroup, string> = {
  intake: 'border-opportunity-intake',
  triage: 'border-opportunity-triage',
  waiting: 'border-opportunity-waiting',
  review: 'border-opportunity-review',
  decision: 'border-opportunity-decision',
};

export function opportunityBg(group: OpportunityColorGroup): string {
  return bgClasses[group];
}

export function opportunityText(group: OpportunityColorGroup): string {
  return textClasses[group];
}

export function opportunityBorder(group: OpportunityColorGroup): string {
  return borderClasses[group];
}

const hexColors: Record<OpportunityColorGroup, string> = {
  intake: '#6b7280',
  triage: '#4f7ecf',
  waiting: '#c18a2f',
  review: '#7664c7',
  decision: '#2f7f92',
};

const hexColorsLight: Record<OpportunityColorGroup, string> = {
  intake: '#9ca3af',
  triage: '#7fa5e8',
  waiting: '#ddb15b',
  review: '#a190e2',
  decision: '#5ba8b8',
};

export function opportunityHex(group: OpportunityColorGroup): string {
  return hexColors[group];
}

export function opportunityHexLight(group: OpportunityColorGroup): string {
  return hexColorsLight[group];
}
