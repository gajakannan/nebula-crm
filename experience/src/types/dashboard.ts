// Dashboard DTO types matching backend responses

export interface DashboardKpisDto {
  activeBrokers: number;
  openSubmissions: number;
  renewalRate: number | null;
  avgTurnaroundDays: number | null;
}

export interface PipelineStatusCountDto {
  status: string;
  count: number;
  colorGroup: PipelineColorGroup;
}

export type PipelineColorGroup =
  | 'intake'
  | 'triage'
  | 'waiting'
  | 'review'
  | 'decision';

export interface DashboardPipelineDto {
  submissions: PipelineStatusCountDto[];
  renewals: PipelineStatusCountDto[];
}

export type PipelineEntityType = 'submission' | 'renewal';

export interface PipelineMiniCardDto {
  entityId: string;
  entityName: string;
  amount: number | null;
  daysInStatus: number;
  assignedUserInitials: string | null;
  assignedUserDisplayName: string | null;
}

export interface PipelineItemsDto {
  items: PipelineMiniCardDto[];
  totalCount: number;
}

export interface NudgeCardDto {
  nudgeType: NudgeType;
  title: string;
  description: string;
  linkedEntityType: string;
  linkedEntityId: string;
  linkedEntityName: string;
  urgencyValue: number;
  ctaLabel: string;
}

export type NudgeType = 'OverdueTask' | 'StaleSubmission' | 'UpcomingRenewal';

export interface NudgesResponseDto {
  nudges: NudgeCardDto[];
}

export interface TaskSummaryDto {
  id: string;
  title: string;
  status: TaskStatus;
  dueDate: string | null;
  linkedEntityType: string | null;
  linkedEntityId: string | null;
  linkedEntityName: string | null;
  isOverdue: boolean;
}

export type TaskStatus = 'Open' | 'InProgress' | 'Done';

export interface MyTasksResponseDto {
  tasks: TaskSummaryDto[];
  totalCount: number;
}

export interface TimelineEventDto {
  id: string;
  entityType: string;
  entityId: string;
  eventType: string;
  eventDescription: string | null;
  entityName: string | null;
  actorDisplayName: string | null;
  occurredAt: string;
}
