import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface JiraWorkspace {
  id: number;
  teamId: number;
  jiraBaseUrl: string;
  jiraEmail: string;
  isConnected: boolean;
  lastSyncDate?: string;
}

export interface JiraProject {
  id: string;
  key: string;
  name: string;
}

export interface JiraSyncStatus {
  isRunning: boolean;
  lastSync?: string;
  projectsSynced: number;
  issuesSynced: number;
  sprintsSynced: number;
}

@Injectable({
  providedIn: 'root'
})
export class JiraService {
  private get apiUrl() {
    return 'http://localhost:5555/api';
  }

  constructor(private http: HttpClient) {}

  connectJira(teamId: number, jiraBaseUrl: string, jiraEmail: string, apiToken: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/jira/connect`, {
      teamId,
      jiraBaseUrl,
      jiraEmail,
      apiToken
    });
  }

  getJiraWorkspace(teamId: number): Observable<JiraWorkspace> {
    return this.http.get<JiraWorkspace>(`${this.apiUrl}/jira/workspace/${teamId}`);
  }

  getJiraProjects(teamId: number): Observable<JiraProject[]> {
    return this.http.get<JiraProject[]>(`${this.apiUrl}/jira/projects/${teamId}`);
  }

  syncJiraData(teamId: number, projectKeys: string[]): Observable<any> {
    return this.http.post(`${this.apiUrl}/jira/sync`, {
      teamId,
      projectKeys
    });
  }

  getSyncStatus(teamId: number): Observable<JiraSyncStatus> {
    return this.http.get<JiraSyncStatus>(`${this.apiUrl}/jira/sync-status/${teamId}`);
  }

  disconnectJira(teamId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/jira/disconnect/${teamId}`);
  }
}
