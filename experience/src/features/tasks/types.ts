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
