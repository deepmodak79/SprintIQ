import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Team, User } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class TeamService {
  constructor(private api: ApiService) {}

  createTeam(data: { name: string; description?: string }): Observable<Team> {
    return this.api.post<Team>('/api/teams', data);
  }

  getTeam(id: number): Observable<Team> {
    return this.api.get<Team>(`/api/teams/${id}`);
  }

  getAllTeams(): Observable<Team[]> {
    return this.api.get<Team[]>('/api/teams');
  }

  getMyTeams(): Observable<Team[]> {
    return this.api.get<Team[]>('/api/teams/my-teams');
  }

  updateTeam(id: number, data: { name?: string; description?: string }): Observable<Team> {
    return this.api.put<Team>(`/api/teams/${id}`, data);
  }

  deleteTeam(id: number): Observable<void> {
    return this.api.delete<void>(`/api/teams/${id}`);
  }

  addMember(teamId: number, userId: number, role: string = 'Member'): Observable<Team> {
    return this.api.post<Team>(`/api/teams/${teamId}/members`, { userId, role });
  }

  removeMember(teamId: number, userId: number): Observable<void> {
    return this.api.delete<void>(`/api/teams/${teamId}/members/${userId}`);
  }

  getAllUsers(): Observable<User[]> {
    return this.api.get<User[]>('/api/auth/users');
  }
}
