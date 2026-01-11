import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { LeaderboardEntry, TeamLeaderboard, DashboardStats, TeamDashboard } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class LeaderboardService {
  constructor(private api: ApiService) {}

  getGlobalLeaderboard(limit: number = 10): Observable<LeaderboardEntry[]> {
    return this.api.get<LeaderboardEntry[]>(`/api/leaderboard?limit=${limit}`);
  }

  getTeamLeaderboard(teamId: number): Observable<TeamLeaderboard> {
    return this.api.get<TeamLeaderboard>(`/api/leaderboard/team/${teamId}`);
  }

  getSprintLeaderboard(sprintId: number): Observable<any> {
    return this.api.get<any>(`/api/leaderboard/sprint/${sprintId}`);
  }

  getMyStats(): Observable<DashboardStats> {
    return this.api.get<DashboardStats>('/api/leaderboard/my-stats');
  }

  getTeamDashboard(teamId: number): Observable<TeamDashboard> {
    return this.api.get<TeamDashboard>(`/api/leaderboard/team/${teamId}/dashboard`);
  }
}
