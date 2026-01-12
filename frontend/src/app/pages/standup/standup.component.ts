import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';
import { AuthService } from '../../core/services/auth.service';
import { StandupService } from '../../core/services/standup.service';
import { TeamService } from '../../core/services/team.service';
import { SprintService } from '../../core/services/sprint.service';
import { User, DailyStandup, Team, Sprint, TeamStandupSummary } from '../../core/models/models';

@Component({
  selector: 'app-standup',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent, RouterLink],
  template: `
    <div class="layout">
      <app-sidebar [user]="user"></app-sidebar>
      
      <main class="main-content">
        <header class="page-header">
          <div>
            <h1>Daily Standup 📋</h1>
            <p>Share your progress and blockers with your team</p>
          </div>
        </header>

        <div class="standup-grid">
          <!-- Standup Form -->
          <div class="card standup-form animate-fade-in">
            <div class="card-header">
              <h3 class="card-title">Today's Update</h3>
              <span class="date-badge">{{ today | date:'EEEE, MMMM d' }}</span>
            </div>

            @if (!activeSprint) {
              <div class="empty-state">
                <div class="empty-icon">🏃</div>
                <h3>No Active Sprint</h3>
                <p>Join a team with an active sprint to submit standups</p>
                <a routerLink="/teams" class="btn btn-primary mt-md">View Teams</a>
              </div>
            } @else {
              <form (ngSubmit)="submitStandup()">
                <div class="form-group">
                  <label class="form-label">
                    <span class="label-icon">⬅️</span>
                    What did you work on yesterday?
                  </label>
                  <textarea 
                    class="form-input" 
                    [(ngModel)]="standup.yesterday" 
                    name="yesterday"
                    placeholder="Describe your completed tasks..."
                    rows="3"
                  ></textarea>
                </div>

                <div class="form-group">
                  <label class="form-label">
                    <span class="label-icon">➡️</span>
                    What will you work on today?
                  </label>
                  <textarea 
                    class="form-input" 
                    [(ngModel)]="standup.today" 
                    name="today"
                    placeholder="What are you planning to accomplish..."
                    rows="3"
                  ></textarea>
                </div>

                <div class="form-group">
                  <label class="form-label">
                    <span class="label-icon">🚫</span>
                    Any blockers or impediments?
                  </label>
                  <textarea 
                    class="form-input" 
                    [(ngModel)]="standup.blockers" 
                    name="blockers"
                    placeholder="Describe any blockers or leave empty if none..."
                    rows="2"
                  ></textarea>
                </div>

                <div class="mood-section">
                  <div class="form-group">
                    <label class="form-label">How are you feeling today?</label>
                    <div class="mood-selector">
                      @for (m of [1,2,3,4,5]; track m) {
                        <button 
                          type="button" 
                          class="mood-btn"
                          [class.active]="standup.mood === m"
                          (click)="standup.mood = m"
                        >
                          {{ getMoodEmoji(m) }}
                        </button>
                      }
                    </div>
                  </div>

                  <div class="form-group">
                    <label class="form-label">Sprint confidence level?</label>
                    <div class="mood-selector">
                      @for (c of [1,2,3,4,5]; track c) {
                        <button 
                          type="button" 
                          class="mood-btn"
                          [class.active]="standup.confidence === c"
                          (click)="standup.confidence = c"
                        >
                          {{ c }}
                        </button>
                      }
                    </div>
                  </div>
                </div>

                <button 
                  type="submit" 
                  class="btn btn-primary btn-lg w-full"
                  [disabled]="submitting"
                >
                  @if (submitting) {
                    <span class="spinner"></span>
                  } @else if (hasSubmittedToday) {
                    Update Standup
                  } @else {
                    Submit Standup (+10 points)
                  }
                </button>

                @if (submitSuccess) {
                  <div class="success-message animate-fade-in">
                    ✅ Standup {{ hasSubmittedToday ? 'updated' : 'submitted' }} successfully!
                  </div>
                }
              </form>
            }
          </div>

          <!-- Team Standups -->
          <div class="card team-standups">
            <div class="card-header">
              <h3 class="card-title">Team Updates</h3>
              <button class="btn btn-ghost btn-sm" (click)="generateAiSummary()" [disabled]="!summary">
                🤖 AI Summary
              </button>
            </div>

            @if (aiSummary) {
              <div class="ai-summary animate-fade-in">
                <div class="ai-header">
                  <span>🤖</span>
                  <span>AI-Generated Summary</span>
                </div>
                <div class="ai-content" [innerHTML]="aiSummary"></div>
              </div>
            }

            @if (summary) {
              <div class="summary-stats">
                <div class="summary-stat">
                  <span class="stat-value">{{ summary.submittedCount }}/{{ summary.totalMembers }}</span>
                  <span class="stat-label">Submitted</span>
                </div>
                <div class="summary-stat">
                  <span class="stat-value">{{ getMoodEmoji(Math.round(summary.averageMood || 3)) }}</span>
                  <span class="stat-label">Avg Mood</span>
                </div>
                <div class="summary-stat">
                  <span class="stat-value">{{ (summary.averageConfidence || 3).toFixed(1) }}</span>
                  <span class="stat-label">Confidence</span>
                </div>
              </div>

              <div class="standups-list">
                @for (s of summary.standups; track s.id) {
                  <div class="standup-item">
                    <div class="standup-header">
                      <div class="avatar avatar-sm">{{ s.userName.charAt(0) }}</div>
                      <div class="standup-user">
                        <span class="user-name">{{ s.userName }}</span>
                        <span class="submitted-time">{{ s.submittedAt | date:'shortTime' }}</span>
                      </div>
                      @if (s.mood) {
                        <span class="mood-display">{{ getMoodEmoji(s.mood) }}</span>
                      }
                    </div>
                    
                    @if (s.today) {
                      <div class="standup-section">
                        <span class="section-icon">➡️</span>
                        <p>{{ s.today }}</p>
                      </div>
                    }
                    
                    @if (s.blockers) {
                      <div class="standup-section blocker">
                        <span class="section-icon">🚫</span>
                        <p>{{ s.blockers }}</p>
                      </div>
                    }
                  </div>
                }

                @if (summary.standups.length === 0) {
                  <div class="empty-state">
                    <p>No standups submitted yet today</p>
                  </div>
                }
              </div>
            }
          </div>
        </div>
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
      margin-bottom: var(--space-xl);
      
      p {
        color: var(--text-secondary);
      }
    }
    
    .standup-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--space-lg);
      
      @media (max-width: 1000px) {
        grid-template-columns: 1fr;
      }
    }
    
    .standup-form {
      .card-header {
        margin-bottom: var(--space-lg);
      }
      
      .date-badge {
        font-size: 0.9rem;
        color: var(--accent-primary);
      }
    }
    
    .form-label {
      display: flex;
      align-items: center;
      gap: var(--space-sm);
      
      .label-icon {
        font-size: 1.1rem;
      }
    }
    
    .mood-section {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--space-lg);
      margin-bottom: var(--space-lg);
    }
    
    .mood-selector {
      display: flex;
      gap: var(--space-sm);
    }
    
    .mood-btn {
      width: 48px;
      height: 48px;
      border: 1px solid var(--border-color);
      border-radius: var(--radius-md);
      background: var(--bg-tertiary);
      font-size: 1.5rem;
      cursor: pointer;
      transition: all 0.2s ease;
      
      &:hover {
        border-color: var(--accent-primary);
        transform: scale(1.1);
      }
      
      &.active {
        border-color: var(--accent-primary);
        background: var(--accent-glow);
        transform: scale(1.1);
      }
    }
    
    .success-message {
      margin-top: var(--space-md);
      padding: var(--space-md);
      background: rgba(16, 185, 129, 0.1);
      border: 1px solid rgba(16, 185, 129, 0.3);
      border-radius: var(--radius-md);
      color: var(--success);
      text-align: center;
    }
    
    .team-standups {
      max-height: calc(100vh - 200px);
      overflow-y: auto;
    }
    
    .ai-summary {
      margin-bottom: var(--space-lg);
      padding: var(--space-md);
      background: var(--bg-tertiary);
      border-radius: var(--radius-md);
      border-left: 3px solid var(--accent-primary);
      
      .ai-header {
        display: flex;
        align-items: center;
        gap: var(--space-sm);
        font-weight: 600;
        margin-bottom: var(--space-sm);
        color: var(--accent-primary);
      }
      
      .ai-content {
        font-size: 0.9rem;
        line-height: 1.6;
        color: var(--text-secondary);
        white-space: pre-wrap;
      }
    }
    
    .summary-stats {
      display: flex;
      gap: var(--space-md);
      margin-bottom: var(--space-lg);
    }
    
    .summary-stat {
      flex: 1;
      text-align: center;
      padding: var(--space-md);
      background: var(--bg-tertiary);
      border-radius: var(--radius-md);
      
      .stat-value {
        display: block;
        font-size: 1.5rem;
        font-weight: 700;
      }
      
      .stat-label {
        font-size: 0.8rem;
        color: var(--text-muted);
      }
    }
    
    .standups-list {
      display: flex;
      flex-direction: column;
      gap: var(--space-md);
    }
    
    .standup-item {
      padding: var(--space-md);
      background: var(--bg-tertiary);
      border-radius: var(--radius-md);
    }
    
    .standup-header {
      display: flex;
      align-items: center;
      gap: var(--space-sm);
      margin-bottom: var(--space-sm);
    }
    
    .standup-user {
      flex: 1;
      
      .user-name {
        display: block;
        font-weight: 600;
      }
      
      .submitted-time {
        font-size: 0.8rem;
        color: var(--text-muted);
      }
    }
    
    .mood-display {
      font-size: 1.25rem;
    }
    
    .standup-section {
      display: flex;
      gap: var(--space-sm);
      padding: var(--space-sm);
      margin-top: var(--space-sm);
      background: var(--bg-card);
      border-radius: var(--radius-sm);
      
      .section-icon {
        flex-shrink: 0;
      }
      
      p {
        font-size: 0.9rem;
        color: var(--text-secondary);
        margin: 0;
      }
      
      &.blocker {
        background: rgba(239, 68, 68, 0.1);
        border-left: 2px solid var(--danger);
      }
    }
  `]
})
export class StandupComponent implements OnInit {
  user: User | null = null;
  teams: Team[] = [];
  activeSprint: Sprint | null = null;
  summary: TeamStandupSummary | null = null;
  
  today = new Date();
  hasSubmittedToday = false;
  submitting = false;
  submitSuccess = false;
  aiSummary = '';
  
  standup = {
    yesterday: '',
    today: '',
    blockers: '',
    mood: 3,
    confidence: 3
  };

  Math = Math;

  constructor(
    private router: Router,
    private authService: AuthService,
    private standupService: StandupService,
    private teamService: TeamService,
    private sprintService: SprintService
  ) {}

  ngOnInit(): void {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }

    this.authService.currentUser$.subscribe(user => this.user = user);
    this.loadData();
  }

  loadData(): void {
    this.teamService.getMyTeams().subscribe({
      next: teams => {
        this.teams = teams;
        if (teams.length > 0) {
          this.loadActiveSprint(teams[0].id);
        }
      }
    });
  }

  loadActiveSprint(teamId: number): void {
    this.sprintService.getActiveSprintByTeam(teamId).subscribe({
      next: sprint => {
        this.activeSprint = sprint;
        this.loadTodayStandup();
        this.loadTeamSummary();
      },
      error: () => {}
    });
  }

  loadTodayStandup(): void {
    if (!this.activeSprint) return;

    this.standupService.getTodayStandup(this.activeSprint.id).subscribe({
      next: standup => {
        this.hasSubmittedToday = true;
        this.standup = {
          yesterday: standup.yesterday || '',
          today: standup.today || '',
          blockers: standup.blockers || '',
          mood: standup.mood || 3,
          confidence: standup.confidence || 3
        };
      },
      error: () => {
        this.hasSubmittedToday = false;
      }
    });
  }

  loadTeamSummary(): void {
    if (!this.activeSprint) return;

    this.standupService.getTeamStandupSummary(this.activeSprint.id).subscribe({
      next: summary => this.summary = summary
    });
  }

  submitStandup(): void {
    if (!this.activeSprint) return;

    this.submitting = true;
    this.submitSuccess = false;

    this.standupService.createOrUpdateStandup({
      sprintId: this.activeSprint.id,
      yesterday: this.standup.yesterday,
      today: this.standup.today,
      blockers: this.standup.blockers,
      mood: this.standup.mood,
      confidence: this.standup.confidence
    }).subscribe({
      next: () => {
        this.submitting = false;
        this.submitSuccess = true;
        this.hasSubmittedToday = true;
        this.loadTeamSummary();
        
        setTimeout(() => this.submitSuccess = false, 3000);
      },
      error: () => {
        this.submitting = false;
      }
    });
  }

  generateAiSummary(): void {
    if (!this.activeSprint) return;

    this.standupService.generateAiSummary(this.activeSprint.id, new Date()).subscribe({
      next: response => {
        this.aiSummary = response.summary;
      }
    });
  }

  getMoodEmoji(mood: number): string {
    switch (mood) {
      case 1: return '😰';
      case 2: return '😟';
      case 3: return '😐';
      case 4: return '😊';
      case 5: return '🌟';
      default: return '😐';
    }
  }
}
