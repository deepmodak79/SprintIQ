import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { DailyStandup, CreateStandup, TeamStandupSummary } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class StandupService {
  constructor(private api: ApiService) {}

  createOrUpdateStandup(data: CreateStandup): Observable<DailyStandup> {
    return this.api.post<DailyStandup>('/api/standups', data);
  }

  getStandup(id: number): Observable<DailyStandup> {
    return this.api.get<DailyStandup>(`/api/standups/${id}`);
  }

  getTodayStandup(sprintId: number): Observable<DailyStandup> {
    return this.api.get<DailyStandup>(`/api/standups/today?sprintId=${sprintId}`);
  }

  getStandupsBySprint(sprintId: number, date?: Date): Observable<DailyStandup[]> {
    let url = `/api/standups/sprint/${sprintId}`;
    if (date) {
      url += `?date=${date.toISOString()}`;
    }
    return this.api.get<DailyStandup[]>(url);
  }

  getTeamStandupSummary(sprintId: number, date?: Date): Observable<TeamStandupSummary> {
    let url = `/api/standups/sprint/${sprintId}/summary`;
    if (date) {
      url += `?date=${date.toISOString()}`;
    }
    return this.api.get<TeamStandupSummary>(url);
  }

  getMyStandups(limit?: number): Observable<DailyStandup[]> {
    let url = '/api/standups/my-standups';
    if (limit) {
      url += `?limit=${limit}`;
    }
    return this.api.get<DailyStandup[]>(url);
  }

  generateAiSummary(sprintId: number, date: Date): Observable<{ summary: string }> {
    return this.api.post<{ summary: string }>('/api/standups/generate-ai-summary', {
      sprintId,
      date
    });
  }
}
