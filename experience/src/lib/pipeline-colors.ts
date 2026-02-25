import type { PipelineColorGroup } from '@/types';

const bgClasses: Record<PipelineColorGroup, string> = {
  intake: 'bg-pipeline-intake',
  triage: 'bg-pipeline-triage',
  waiting: 'bg-pipeline-waiting',
  review: 'bg-pipeline-review',
  decision: 'bg-pipeline-decision',
};

const textClasses: Record<PipelineColorGroup, string> = {
  intake: 'text-pipeline-intake',
  triage: 'text-pipeline-triage',
  waiting: 'text-pipeline-waiting',
  review: 'text-pipeline-review',
  decision: 'text-pipeline-decision',
};

const borderClasses: Record<PipelineColorGroup, string> = {
  intake: 'border-pipeline-intake',
  triage: 'border-pipeline-triage',
  waiting: 'border-pipeline-waiting',
  review: 'border-pipeline-review',
  decision: 'border-pipeline-decision',
};

export function pipelineBg(group: PipelineColorGroup): string {
  return bgClasses[group];
}

export function pipelineText(group: PipelineColorGroup): string {
  return textClasses[group];
}

export function pipelineBorder(group: PipelineColorGroup): string {
  return borderClasses[group];
}

const hexColors: Record<PipelineColorGroup, string> = {
  intake: '#64748b',
  triage: '#3b82f6',
  waiting: '#f59e0b',
  review: '#8b5cf6',
  decision: '#10b981',
};

const hexColorsLight: Record<PipelineColorGroup, string> = {
  intake: '#94a3b8',
  triage: '#60a5fa',
  waiting: '#fbbf24',
  review: '#a78bfa',
  decision: '#34d399',
};

export function pipelineHex(group: PipelineColorGroup): string {
  return hexColors[group];
}

export function pipelineHexLight(group: PipelineColorGroup): string {
  return hexColorsLight[group];
}
