import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';
import { JiraService, JiraWorkspace, JiraProject } from '../../core/services/jira.service';
import { AuthService } from '../../core/services/auth.service';
import { TeamService } from '../../core/services/team.service';

@Component({
  selector: 'app-jira-integration',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent],
  template: `
    <div class="layout">
      <app-sidebar [user]="user"></app-sidebar>
      
      <main class="main-content">
        <header class="page-header">
          <div>
            <h1>🔗 Jira Integration</h1>
            <p>Connect your Jira workspace to sync projects and sprints</p>
          </div>
        </header>

        @if (!isConnected) {
          <!-- Connection Form -->
          <div class="card animate-fade-in">
            <div class="card-header">
              <h3>Connect to Jira</h3>
              <p class="text-muted">Enter your Jira workspace details to get started</p>
            </div>
            <div class="card-body">
              <form (ngSubmit)="connectJira()" class="form-grid">
                <div class="form-group">
                  <label>Jira Base URL</label>
                  <input 
                    type="url" 
                    [(ngModel)]="connectionForm.jiraBaseUrl" 
                    name="jiraBaseUrl"
                    placeholder="https://your-company.atlassian.net"
                    required>
                  <small>Your Jira cloud instance URL</small>
                </div>
                <div class="form-group">
                  <label>Email Address</label>
                  <input 
                    type="email" 
                    [(ngModel)]="connectionForm.jiraEmail" 
                    name="jiraEmail"
                    placeholder="your-email@company.com"
                    required>
                </div>
                <div class="form-group">
                  <label>API Token</label>
                  <input 
                    type="password" 
                    [(ngModel)]="connectionForm.apiToken" 
                    name="apiToken"
                    placeholder="Your Jira API token"
                    required>
                  <small>
                    <a href="https://id.atlassian.com/manage-profile/security/api-tokens" target="_blank">
                      Get API Token from Atlassian
                    </a>
                  </small>
                </div>
                <div class="form-actions">
                  <button type="submit" class="btn btn-primary" [disabled]="isConnecting">
                    {{ isConnecting ? 'Connecting...' : 'Connect to Jira' }}
                  </button>
                </div>
              </form>
            </div>
          </div>
        } @else {
          <!-- Connected State -->
          <div class="card success-card animate-fade-in">
            <div class="card-body">
              <div class="success-content">
                <span class="success-icon">✅</span>
                <div>
                  <h3>Connected to Jira</h3>
                  <p>{{ workspace?.jiraBaseUrl }}</p>
                  @if (workspace?.lastSyncDate) {
                    <small>Last synced: {{ workspace?.lastSyncDate | date:'medium' }}</small>
                  }
                </div>
                <button class="btn btn-outline-danger" (click)="disconnectJira()">
                  Disconnect
                </button>
              </div>
            </div>
          </div>

          <!-- Projects -->
          <div class="card animate-fade-in" style="animation-delay: 0.1s;">
            <div class="card-header">
              <h3>Jira Projects</h3>
              <button class="btn btn-primary" (click)="loadProjects()" [disabled]="isLoading">
                🔄 Refresh Projects
              </button>
            </div>
            <div class="card-body">
              @if (isLoading) {
                <div class="loading-spinner">Loading projects...</div>
              } @else if (projects.length === 0) {
                <div class="empty-state">
                  <span class="empty-icon">📦</span>
                  <p>No projects found</p>
                  <button class="btn btn-secondary" (click)="loadProjects()">Load Projects</button>
                </div>
              } @else {
                <div class="projects-grid">
                  @for (project of projects; track project.id) {
                    <div class="project-card" [class.selected]="selectedProjects.has(project.key)">
                      <input 
                        type="checkbox" 
                        [checked]="selectedProjects.has(project.key)"
                        (change)="toggleProject(project.key)">
                      <div class="project-info">
                        <h4>{{ project.name }}</h4>
                        <span class="project-key">{{ project.key }}</span>
                      </div>
                    </div>
                  }
                </div>
                <div class="form-actions">
                  <button 
                    class="btn btn-primary btn-lg" 
                    (click)="syncSelectedProjects()"
                    [disabled]="selectedProjects.size === 0 || isSyncing">
                    {{ isSyncing ? 'Syncing...' : 'Sync ' + selectedProjects.size + ' Project(s)' }}
                  </button>
                </div>
              }
            </div>
          </div>

          <!-- Sync Status -->
          @if (syncStatus) {
            <div class="card animate-fade-in" style="animation-delay: 0.2s;">
              <div class="card-header">
                <h3>Sync Status</h3>
              </div>
              <div class="card-body">
                <div class="sync-stats">
                  <div class="sync-stat">
                    <span class="stat-value">{{ syncStatus.projectsSynced }}</span>
                    <span class="stat-label">Projects</span>
                  </div>
                  <div class="sync-stat">
                    <span class="stat-value">{{ syncStatus.sprintsSynced }}</span>
                    <span class="stat-label">Sprints</span>
                  </div>
                  <div class="sync-stat">
                    <span class="stat-value">{{ syncStatus.issuesSynced }}</span>
                    <span class="stat-label">Issues</span>
                  </div>
                </div>
              </div>
            </div>
          }
        }
      </main>
    </div>
  `,
  styles: [`
    .layout {
      display: flex;
      min-height: 100vh;
    }

    .main-content {
      flex: 1;
      margin-left: 260px;
      padding: 2rem;
      max-width: calc(100% - 260px);
    }

    .page-header {
      margin-bottom: 2rem;
    }

    .page-header h1 {
      font-size: 2rem;
      font-weight: 700;
      margin-bottom: 0.5rem;
      color: var(--text-primary);
    }

    .page-header p {
      color: var(--text-secondary);
    }

    .card {
      background: var(--bg-card);
      border: 1px solid var(--border-color);
      border-radius: var(--radius-lg);
      padding: 1.5rem;
      margin-bottom: 1.5rem;
    }

    .card-header {
      margin-bottom: 1.5rem;
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding-bottom: 1rem;
      border-bottom: 1px solid var(--border-color);
    }

    .card-header h3 {
      margin: 0;
      font-size: 1.25rem;
      font-weight: 600;
      color: var(--text-primary);
    }

    .card-header p {
      margin: 0.5rem 0 0 0;
      color: var(--text-secondary);
      font-size: 0.875rem;
    }

    .text-muted {
      color: var(--text-secondary);
    }

    .card-body {
      padding: 0;
    }

    .form-grid {
      display: grid;
      gap: 1.5rem;
      max-width: 600px;
    }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .form-group label {
      font-weight: 500;
      color: var(--text-primary);
      font-size: 0.9rem;
    }

    .form-group input {
      width: 100%;
      padding: 0.75rem;
      background: var(--bg-tertiary);
      border: 1px solid var(--border-color);
      border-radius: var(--radius-md);
      font-size: 0.875rem;
      color: var(--text-primary);
      font-family: var(--font-primary);
    }

    .form-group input::placeholder {
      color: var(--text-muted);
    }

    .form-group input:focus {
      outline: none;
      border-color: var(--accent-primary);
      box-shadow: 0 0 0 3px var(--accent-glow);
    }

    .form-group small {
      color: var(--text-secondary);
      font-size: 0.8125rem;
    }

    .form-group small a {
      color: var(--accent-primary);
      text-decoration: none;
    }

    .form-group small a:hover {
      text-decoration: underline;
    }

    .form-actions {
      margin-top: 1.5rem;
    }

    .success-card {
      background: linear-gradient(135deg, var(--accent-primary), var(--accent-secondary));
      color: white;
      border: none;
    }

    .success-content {
      display: flex;
      align-items: center;
      gap: 1.5rem;
    }

    .success-icon {
      font-size: 3rem;
    }

    .success-content h3 {
      margin: 0 0 0.5rem 0;
    }

    .success-content p {
      margin: 0;
      opacity: 0.9;
    }

    .success-content small {
      opacity: 0.8;
      font-size: 0.8125rem;
    }

    .projects-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
      gap: 1rem;
      margin-bottom: 1.5rem;
    }

    .project-card {
      display: flex;
      gap: 1rem;
      padding: 1rem;
      border: 2px solid var(--border-color);
      border-radius: var(--radius-md);
      cursor: pointer;
      transition: all 0.2s;
      background: var(--bg-tertiary);
    }

    .project-card:hover {
      border-color: var(--accent-primary);
      transform: translateY(-2px);
    }

    .project-card.selected {
      border-color: var(--accent-primary);
      background: var(--accent-glow);
    }

    .project-card input[type="checkbox"] {
      cursor: pointer;
      width: 18px;
      height: 18px;
    }

    .project-info {
      flex: 1;
    }

    .project-info h4 {
      margin: 0 0 0.25rem 0;
      font-size: 1rem;
      color: var(--text-primary);
    }

    .project-key {
      display: inline-block;
      padding: 0.25rem 0.5rem;
      background: var(--bg-secondary);
      border-radius: var(--radius-sm);
      font-size: 0.75rem;
      font-weight: 600;
      color: var(--text-secondary);
    }

    .sync-stats {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: 2rem;
    }

    .sync-stat {
      text-align: center;
    }

    .sync-stat .stat-value {
      display: block;
      font-size: 2rem;
      font-weight: 700;
      color: var(--accent-primary);
    }

    .sync-stat .stat-label {
      display: block;
      font-size: 0.875rem;
      color: var(--text-secondary);
      margin-top: 0.5rem;
    }

    .loading-spinner {
      text-align: center;
      padding: 2rem;
      color: var(--text-secondary);
    }

    .empty-state {
      text-align: center;
      padding: 3rem;
    }

    .empty-icon {
      font-size: 4rem;
      display: block;
      margin-bottom: 1rem;
    }

    .empty-state p {
      color: var(--text-secondary);
      margin-bottom: 1rem;
    }

    .animate-fade-in {
      animation: fadeIn 0.3s ease-in;
    }

    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(10px); }
      to { opacity: 1; transform: translateY(0); }
    }
  `]
})
export class JiraIntegrationComponent implements OnInit {
  user: any;
  isConnected = false;
  isConnecting = false;
  isLoading = false;
  isSyncing = false;
  
  workspace?: JiraWorkspace;
  projects: JiraProject[] = [];
  selectedProjects = new Set<string>();
  syncStatus: any;

  connectionForm = {
    jiraBaseUrl: '',
    jiraEmail: '',
    apiToken: ''
  };

  constructor(
    private jiraService: JiraService,
    private authService: AuthService,
    private teamService: TeamService
  ) {
    this.user = this.authService.getCurrentUser();
  }

  ngOnInit() {
    this.checkConnection();
  }

  checkConnection() {
    const teamId = 1; // TODO: Get from current team context
    this.jiraService.getJiraWorkspace(teamId).subscribe({
      next: (workspace) => {
        this.workspace = workspace;
        this.isConnected = workspace.isConnected;
        if (this.isConnected) {
          this.loadProjects();
        }
      },
      error: () => {
        this.isConnected = false;
      }
    });
  }

  connectJira() {
    this.isConnecting = true;
    const teamId = 1; // TODO: Get from current team context
    
    this.jiraService.connectJira(
      teamId,
      this.connectionForm.jiraBaseUrl,
      this.connectionForm.jiraEmail,
      this.connectionForm.apiToken
    ).subscribe({
      next: () => {
        this.isConnected = true;
        this.isConnecting = false;
        this.checkConnection();
      },
      error: (error) => {
        console.error('Connection failed:', error);
        alert('Failed to connect to Jira. Please check your credentials.');
        this.isConnecting = false;
      }
    });
  }

  disconnectJira() {
    if (!confirm('Are you sure you want to disconnect from Jira?')) return;
    
    const teamId = 1;
    this.jiraService.disconnectJira(teamId).subscribe({
      next: () => {
        this.isConnected = false;
        this.workspace = undefined;
        this.projects = [];
        this.selectedProjects.clear();
      }
    });
  }

  loadProjects() {
    this.isLoading = true;
    const teamId = 1;
    
    this.jiraService.getJiraProjects(teamId).subscribe({
      next: (projects) => {
        this.projects = projects;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  toggleProject(projectKey: string) {
    if (this.selectedProjects.has(projectKey)) {
      this.selectedProjects.delete(projectKey);
    } else {
      this.selectedProjects.add(projectKey);
    }
  }

  syncSelectedProjects() {
    this.isSyncing = true;
    const teamId = 1;
    const projectKeys = Array.from(this.selectedProjects);
    
    this.jiraService.syncJiraData(teamId, projectKeys).subscribe({
      next: () => {
        alert('Sync started! This may take a few moments.');
        this.isSyncing = false;
        this.loadSyncStatus();
      },
      error: () => {
        alert('Sync failed. Please try again.');
        this.isSyncing = false;
      }
    });
  }

  loadSyncStatus() {
    const teamId = 1;
    this.jiraService.getSyncStatus(teamId).subscribe({
      next: (status) => {
        this.syncStatus = status;
      }
    });
  }
}
