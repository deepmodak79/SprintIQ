import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./pages/auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent)
  },
  {
    path: 'sprint/:id',
    loadComponent: () => import('./pages/sprint-board/sprint-board.component').then(m => m.SprintBoardComponent)
  },
  {
    path: 'standup',
    loadComponent: () => import('./pages/standup/standup.component').then(m => m.StandupComponent)
  },
  {
    path: 'leaderboard',
    loadComponent: () => import('./pages/leaderboard/leaderboard.component').then(m => m.LeaderboardComponent)
  },
  {
    path: 'teams',
    loadComponent: () => import('./pages/teams/teams.component').then(m => m.TeamsComponent)
  },
  {
    path: 'jira-integration',
    loadComponent: () => import('./pages/jira-integration/jira-integration.component').then(m => m.JiraIntegrationComponent)
  },
  {
    path: 'ai-insights',
    loadComponent: () => import('./pages/ai-insights/ai-insights.component').then(m => m.AiInsightsComponent)
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
