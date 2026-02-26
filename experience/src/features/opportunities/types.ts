export interface OpportunityStatusCountDto {
  status: string;
  count: number;
  colorGroup: OpportunityColorGroup;
}

export type OpportunityColorGroup =
  | 'intake'
  | 'triage'
  | 'waiting'
  | 'review'
  | 'decision';

export interface DashboardOpportunitiesDto {
  submissions: OpportunityStatusCountDto[];
  renewals: OpportunityStatusCountDto[];
}

export type OpportunityEntityType = 'submission' | 'renewal';

export interface OpportunityFlowNodeDto {
  status: string;
  label: string;
  isTerminal: boolean;
  displayOrder: number;
  colorGroup: OpportunityColorGroup;
  currentCount: number;
  inflowCount: number;
  outflowCount: number;
}

export interface OpportunityFlowLinkDto {
  sourceStatus: string;
  targetStatus: string;
  count: number;
}

export interface OpportunityFlowDto {
  entityType: OpportunityEntityType;
  periodDays: number;
  windowStartUtc: string;
  windowEndUtc: string;
  nodes: OpportunityFlowNodeDto[];
  links: OpportunityFlowLinkDto[];
}

export interface OpportunityMiniCardDto {
  entityId: string;
  entityName: string;
  amount: number | null;
  daysInStatus: number;
  assignedUserInitials: string | null;
  assignedUserDisplayName: string | null;
}

export interface OpportunityItemsDto {
  items: OpportunityMiniCardDto[];
  totalCount: number;
}
