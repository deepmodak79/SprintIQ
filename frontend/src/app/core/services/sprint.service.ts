import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Sprint, SprintSummary, BurndownData, SprintStatus } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class SprintService {
  constructor(private api: ApiService) {}

  createSprint(data: {
    name: string;
    goal?: string;
    teamId: number;
    startDate: Date;
    endDate: Date;
  }): Observable<Sprint> {
    return this.api.post<Sprint>('/api/sprints', data);
  }

  getSprint(id: number): Observable<Sprint> {
    return this.api.get<Sprint>(`/api/sprints/${id}`);
  }

  getSprintsByTeam(teamId: number): Observable<SprintSummary[]> {
    return this.api.get<SprintSummary[]>(`/api/sprints/team/${teamId}`);
  }

  getActiveSprintByTeam(teamId: number): Observable<Sprint> {
    return this.api.get<Sprint>(`/api/sprints/team/${teamId}/active`);
  }

  updateSprint(id: number, data: {
    name?: string;
    goal?: string;
    startDate?: Date;
    endDate?: Date;
    status?: SprintStatus;
  }): Observable<Sprint> {
    return this.api.put<Sprint>(`/api/sprints/${id}`, data);
  }

  deleteSprint(id: number): Observable<void> {
    return this.api.delete<void>(`/api/sprints/${id}`);
  }

  getBurndownData(sprintId: number): Observable<BurndownData[]> {
    return this.api.get<BurndownData[]>(`/api/sprints/${sprintId}/burndown`);
  }
}
