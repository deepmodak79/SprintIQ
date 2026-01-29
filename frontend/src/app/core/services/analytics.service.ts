import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface SprintRisk {
  sprintId: number;
  sprintName: string;
  riskLevel: 'Low' | 'Medium' | 'High' | 'Critical';
  completionProbability: number;
  predictedUnfinishedTasks: number;
  riskFactors: string[];
  recommendations: string[];
  analyzedAt: string;
}

export interface BlockerPrediction {
  taskId: number;
  taskTitle: string;
  blockerProbability: number;
  reasons: string[];
  suggestedActions: string[];
}

export interface TeamHealth {
  teamId: number;
  teamName: string;
  overallHealthScore: number;
  moraleScore: number;
  workloadBalance: number;
  collaborationScore: number;
  burnoutRisk: number;
  velocityTrend: number;
  insights: HealthInsight[];
  calculatedAt: string;
}

export interface HealthInsight {
  type: string;
  message: string;
  severity: 'Info' | 'Warning' | 'Critical';
}

export interface SmartStandup {
  standupId: number;
  date: string;
  teamName: string;
  userSummaries: UserStandupSummary[];
  insights: StandupInsight[];
  totalBlockers: number;
  tasksCompleted: number;
  tasksInProgress: number;
}

export interface UserStandupSummary {
  userId: number;
  userName: string;
  completedYesterday: string[];
  plannedToday: string[];
  blockers: any[];
  hasBlockers: boolean;
  sentimentIndicator: string;
}

export interface StandupInsight {
  insightType: string;
  title: string;
  description: string;
  priority: string;
}

@Injectable({
  providedIn: 'root'
})
export class AnalyticsService {
  private get apiUrl() {
    return 'http://localhost:5555/api';
  }

  constructor(private http: HttpClient) {}

  getSprintRisk(sprintId: number): Observable<SprintRisk> {
    return this.http.get<SprintRisk>(`${this.apiUrl}/analytics/sprint-risk/${sprintId}`);
  }

  getBlockerPredictions(sprintId: number): Observable<BlockerPrediction[]> {
    return this.http.get<BlockerPrediction[]>(`${this.apiUrl}/analytics/blocker-predictions/${sprintId}`);
  }

  getTeamHealth(teamId: number): Observable<TeamHealth> {
    return this.http.get<TeamHealth>(`${this.apiUrl}/analytics/team-health/${teamId}`);
  }

  getSmartStandup(teamId: number, date?: string): Observable<SmartStandup> {
    const dateParam = date || new Date().toISOString().split('T')[0];
    return this.http.get<SmartStandup>(`${this.apiUrl}/analytics/smart-standup/${teamId}?date=${dateParam}`);
  }

  generateAIInsights(sprintId: number): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/analytics/ai-insights/${sprintId}`);
  }
}
