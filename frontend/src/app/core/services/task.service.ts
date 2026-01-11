import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { SprintTask, KanbanBoard, CreateTask, TaskStatus, TaskPriority } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  constructor(private api: ApiService) {}

  createTask(data: CreateTask): Observable<SprintTask> {
    return this.api.post<SprintTask>('/api/tasks', data);
  }

  getTask(id: number): Observable<SprintTask> {
    return this.api.get<SprintTask>(`/api/tasks/${id}`);
  }

  getTasksBySprint(sprintId: number): Observable<SprintTask[]> {
    return this.api.get<SprintTask[]>(`/api/tasks/sprint/${sprintId}`);
  }

  getKanbanBoard(sprintId: number): Observable<KanbanBoard> {
    return this.api.get<KanbanBoard>(`/api/tasks/sprint/${sprintId}/kanban`);
  }

  updateTask(id: number, data: {
    title?: string;
    description?: string;
    assigneeId?: number;
    status?: TaskStatus;
    priority?: TaskPriority;
    storyPoints?: number;
    orderIndex?: number;
    isBlocked?: boolean;
    blockedReason?: string;
  }): Observable<SprintTask> {
    return this.api.put<SprintTask>(`/api/tasks/${id}`, data);
  }

  moveTask(id: number, newStatus: TaskStatus, newOrderIndex?: number): Observable<SprintTask> {
    return this.api.put<SprintTask>(`/api/tasks/${id}/move`, {
      newStatus,
      newOrderIndex
    });
  }

  deleteTask(id: number): Observable<void> {
    return this.api.delete<void>(`/api/tasks/${id}`);
  }
}
