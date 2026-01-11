import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';
import { AuthService } from '../../core/services/auth.service';
import { LeaderboardService } from '../../core/services/leaderboard.service';
import { TeamService } from '../../core/services/team.service';
import { SprintService } from '../../core/services/sprint.service';
import { User, DashboardStats, Team, Sprint } from '../../core/models/models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent, RouterLink],
  template: `
    <div class="layout">
      <app-sidebar [user]="user"></app-sidebar>
      
      <main class="main-content">
        <header class="page-header">
          <div>
            <h1>Welcome back, {{ getFirstName() }}! 👋</h1>
            <p>Here's what's happening with your sprints today.</p>
          </div>
          <div class="header-actions">
            <button class="btn btn-primary" (click)="createTeamModal = true">
              <span>+</span> New Team
            </button>
          </div>
        </header>

        <!-- Stats Cards -->
        <div class="stats-grid animate-fade-in">
          <div class="stat-card">
            <div class="stat-icon">🎯</div>
            <div class="stat-content">
              <span class="stat-value">{{ stats?.totalTasksCompleted || 0 }}</span>
              <span class="stat-label">Tasks Completed</span>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon">📈</div>
            <div class="stat-content">
              <span class="stat-value">{{ stats?.totalStoryPoints || 0 }}</span>
              <span class="stat-label">Story Points</span>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon">🔥</div>
            <div class="stat-content">
              <span class="stat-value">{{ stats?.currentStreak || 0 }}</span>
              <span class="stat-label">Day Streak</span>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon">⭐</div>
            <div class="stat-content">
              <span class="stat-value">{{ stats?.totalPoints || 0 }}</span>
              <span class="stat-label">Total Points</span>
            </div>
          </div>
        </div>

        <div class="dashboard-grid">
          <!-- Teams Section -->
          <section class="card teams-section">
            <div class="card-header">
              <h3 class="card-title">Your Teams</h3>
              <a routerLink="/teams" class="text-accent">View All →</a>
            </div>
            
            @if (teams.length === 0) {
              <div class="empty-state">
                <div class="empty-icon">👥</div>
                <h3>No Teams Yet</h3>
                <p>Create a team to start managing sprints</p>
                <button class="btn btn-primary mt-md" (click)="createTeamModal = true">
                  Create Team
                </button>
              </div>
            } @else {
              <div class="teams-list">
                @for (team of teams; track team.id) {
                  <div class="team-item" (click)="selectTeam(team)">
                    <div class="team-icon">{{ team.name.charAt(0) }}</div>
                    <div class="team-info">
                      <span class="team-name">{{ team.name }}</span>
                      <span class="team-meta">{{ team.memberCount }} members</span>
                    </div>
                    <span class="badge badge-primary">{{ team.activeSprintCount }} sprints</span>
                  </div>
                }
              </div>
            }
          </section>

          <!-- Rank & Badges -->
          <section class="card rank-section">
            <div class="card-header">
              <h3 class="card-title">Your Rank</h3>
            </div>
            
            <div class="rank-display">
              <div class="rank-number">#{{ stats?.rank || '-' }}</div>
              <p>out of {{ stats?.totalUsers || 0 }} users</p>
            </div>

            <div class="badges-section">
              <h4>Recent Badges</h4>
              @if (stats && stats.recentBadges && stats.recentBadges.length > 0) {
                <div class="badges-grid">
                  @for (badge of stats!.recentBadges; track badge.id) {
                    <div class="badge-item" [title]="badge.description || ''">
                      <span class="badge-icon">{{ badge.icon }}</span>
                      <span class="badge-name">{{ badge.name }}</span>
                    </div>
                  }
                </div>
              } @else {
                <p class="text-muted">Complete tasks to earn badges!</p>
              }
            </div>
          </section>

          <!-- Active Sprint -->
          @if (activeSprint) {
            <section class="card sprint-section">
              <div class="card-header">
                <h3 class="card-title">{{ activeSprint.name }}</h3>
                <span class="badge badge-success">Active</span>
              </div>
              
              <div class="sprint-progress">
                <div class="progress-header">
                  <span>Progress</span>
                  <span>{{ activeSprint.progressPercentage }}%</span>
                </div>
                <div class="progress">
                  <div class="progress-bar" [style.width.%]="activeSprint.progressPercentage"></div>
                </div>
              </div>

              <div class="sprint-stats">
                <div class="sprint-stat">
                  <span class="stat-value">{{ activeSprint.completedTasks }}/{{ activeSprint.totalTasks }}</span>
                  <span class="stat-label">Tasks</span>
                </div>
                <div class="sprint-stat">
                  <span class="stat-value">{{ activeSprint.daysRemaining }}</span>
                  <span class="stat-label">Days Left</span>
                </div>
              </div>

              <button class="btn btn-primary w-full mt-md" [routerLink]="['/sprint', activeSprint.id]">
                Open Sprint Board →
              </button>
            </section>
          }

          <!-- Quick Actions -->
          <section class="card actions-section">
            <div class="card-header">
              <h3 class="card-title">Quick Actions</h3>
            </div>
            
            <div class="actions-grid">
              <a routerLink="/standup" class="action-item">
                <span class="action-icon">📋</span>
                <span class="action-label">Submit Standup</span>
              </a>
              <a routerLink="/leaderboard" class="action-item">
                <span class="action-icon">🏆</span>
                <span class="action-label">Leaderboard</span>
              </a>
              <a routerLink="/teams" class="action-item">
                <span class="action-icon">👥</span>
                <span class="action-label">Manage Teams</span>
              </a>
              @if (activeSprint) {
                <a [routerLink]="['/sprint', activeSprint.id]" class="action-item">
                  <span class="action-icon">📊</span>
                  <span class="action-label">Sprint Board</span>
                </a>
              }
            </div>
          </section>
        </div>

        <!-- Create Team Modal -->
        @if (createTeamModal) {
          <div class="modal-overlay" (click)="createTeamModal = false">
            <div class="modal" (click)="$event.stopPropagation()">
              <div class="modal-header">
                <h3>Create New Team</h3>
                <button class="btn btn-ghost btn-icon" (click)="createTeamModal = false">✕</button>
              </div>
              
              <div class="form-group">
                <label class="form-label">Team Name</label>
                <input type="text" class="form-input" [(ngModel)]="newTeamName" placeholder="Engineering Team">
              </div>
              
              <div class="form-group">
                <label class="form-label">Description (optional)</label>
                <textarea class="form-input" [(ngModel)]="newTeamDescription" placeholder="Team description..."></textarea>
              </div>
              
              <div class="modal-footer">
                <button class="btn btn-secondary" (click)="createTeamModal = false">Cancel</button>
                <button class="btn btn-primary" (click)="createTeam()" [disabled]="!newTeamName">
                  Create Team
                </button>
              </div>
            </div>
          </div>
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
      padding: var(--space-xl);
    }
    
    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: var(--space-xl);
      
      h1 {
        margin-bottom: var(--space-xs);
      }
      
      p {
        color: var(--text-secondary);
      }
    }
    
    .stats-grid {
      display: grid;
      grid-template-columns: repeat(4, 1fr);
      gap: var(--space-md);
      margin-bottom: var(--space-xl);
      
      @media (max-width: 1200px) {
        grid-template-columns: repeat(2, 1fr);
      }
      
      @media (max-width: 600px) {
        grid-template-columns: 1fr;
      }
    }
    
    .stat-card {
      background: var(--bg-card);
      border: 1px solid var(--border-color);
      border-radius: var(--radius-lg);
      padding: var(--space-lg);
      display: flex;
      align-items: center;
      gap: var(--space-md);
      transition: all 0.3s ease;
      
      &:hover {
        border-color: var(--accent-primary);
        transform: translateY(-2px);
        box-shadow: var(--shadow-glow);
      }
    }
    
    .stat-icon {
      font-size: 2rem;
      width: 60px;
      height: 60px;
      display: flex;
      align-items: center;
      justify-content: center;
      background: var(--bg-tertiary);
      border-radius: var(--radius-md);
    }
    
    .stat-content {
      display: flex;
      flex-direction: column;
      
      .stat-value {
        font-size: 1.75rem;
        font-weight: 700;
        color: var(--text-primary);
      }
      
      .stat-label {
        font-size: 0.9rem;
        color: var(--text-secondary);
      }
    }
    
    .dashboard-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: var(--space-lg);
      
      @media (max-width: 1000px) {
        grid-template-columns: 1fr;
      }
    }
    
    .teams-list {
      display: flex;
      flex-direction: column;
      gap: var(--space-sm);
    }
    
    .team-item {
      display: flex;
      align-items: center;
      gap: var(--space-md);
      padding: var(--space-md);
      background: var(--bg-tertiary);
      border-radius: var(--radius-md);
      cursor: pointer;
      transition: all 0.2s ease;
      
      &:hover {
        background: var(--bg-hover);
      }
    }
    
    .team-icon {
      width: 40px;
      height: 40px;
      background: linear-gradient(135deg, var(--accent-primary), var(--accent-secondary));
      border-radius: var(--radius-md);
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: 700;
      font-size: 1.1rem;
    }
    
    .team-info {
      flex: 1;
      display: flex;
      flex-direction: column;
      
      .team-name {
        font-weight: 600;
      }
      
      .team-meta {
        font-size: 0.85rem;
        color: var(--text-muted);
      }
    }
    
    .rank-section {
      text-align: center;
    }
    
    .rank-display {
      padding: var(--space-lg) 0;
      
      .rank-number {
        font-size: 4rem;
        font-weight: 700;
        background: linear-gradient(135deg, var(--accent-primary), var(--accent-tertiary));
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
        background-clip: text;
      }
      
      p {
        color: var(--text-secondary);
      }
    }
    
    .badges-section {
      padding-top: var(--space-md);
      border-top: 1px solid var(--border-color);
      
      h4 {
        margin-bottom: var(--space-md);
        color: var(--text-secondary);
        font-size: 0.9rem;
      }
    }
    
    .badges-grid {
      display: flex;
      flex-wrap: wrap;
      gap: var(--space-sm);
      justify-content: center;
    }
    
    .badge-item {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: var(--space-xs);
      padding: var(--space-sm);
      background: var(--bg-tertiary);
      border-radius: var(--radius-md);
      min-width: 70px;
      
      .badge-icon {
        font-size: 1.5rem;
      }
      
      .badge-name {
        font-size: 0.7rem;
        color: var(--text-muted);
      }
    }
    
    .sprint-progress {
      margin-bottom: var(--space-md);
      
      .progress-header {
        display: flex;
        justify-content: space-between;
        margin-bottom: var(--space-sm);
        font-size: 0.9rem;
        color: var(--text-secondary);
      }
    }
    
    .sprint-stats {
      display: flex;
      gap: var(--space-lg);
      
      .sprint-stat {
        flex: 1;
        text-align: center;
        padding: var(--space-md);
        background: var(--bg-tertiary);
        border-radius: var(--radius-md);
        
        .stat-value {
          display: block;
          font-size: 1.5rem;
          font-weight: 700;
          color: var(--accent-primary);
        }
        
        .stat-label {
          font-size: 0.85rem;
          color: var(--text-muted);
        }
      }
    }
    
    .actions-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: var(--space-md);
    }
    
    .action-item {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: var(--space-sm);
      padding: var(--space-lg);
      background: var(--bg-tertiary);
      border-radius: var(--radius-md);
      text-decoration: none;
      transition: all 0.2s ease;
      
      &:hover {
        background: var(--bg-hover);
        transform: translateY(-2px);
        
        .action-icon {
          transform: scale(1.1);
        }
      }
      
      .action-icon {
        font-size: 2rem;
        transition: transform 0.2s ease;
      }
      
      .action-label {
        font-weight: 500;
        color: var(--text-primary);
      }
    }
  `]
})
export class DashboardComponent implements OnInit {
  user: User | null = null;
  stats: DashboardStats | null = null;
  teams: Team[] = [];
  activeSprint: Sprint | null = null;
  
  createTeamModal = false;
  newTeamName = '';
  newTeamDescription = '';

  constructor(
    private authService: AuthService,
    private leaderboardService: LeaderboardService,
    private teamService: TeamService,
    private sprintService: SprintService,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }

    this.authService.currentUser$.subscribe(user => {
      this.user = user;
    });

    this.loadDashboardData();
  }

  getFirstName(): string {
    if (this.user && this.user.fullName) {
      return this.user.fullName.split(' ')[0];
    }
    return 'User';
  }

  loadDashboardData(): void {
    this.leaderboardService.getMyStats().subscribe({
      next: stats => this.stats = stats,
      error: () => {}
    });

    this.teamService.getMyTeams().subscribe({
      next: teams => {
        this.teams = teams;
        if (teams.length > 0) {
          this.loadActiveSprint(teams[0].id);
        }
      },
      error: () => {}
    });
  }

  loadActiveSprint(teamId: number): void {
    this.sprintService.getActiveSprintByTeam(teamId).subscribe({
      next: sprint => this.activeSprint = sprint,
      error: () => {}
    });
  }

  selectTeam(team: Team): void {
    this.loadActiveSprint(team.id);
  }

  createTeam(): void {
    if (!this.newTeamName) return;

    this.teamService.createTeam({
      name: this.newTeamName,
      description: this.newTeamDescription
    }).subscribe({
      next: team => {
        this.teams.push(team);
        this.createTeamModal = false;
        this.newTeamName = '';
        this.newTeamDescription = '';
      },
      error: () => {}
    });
  }
}
