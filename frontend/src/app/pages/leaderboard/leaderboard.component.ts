import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';
import { AuthService } from '../../core/services/auth.service';
import { LeaderboardService } from '../../core/services/leaderboard.service';
import { User, LeaderboardEntry, DashboardStats } from '../../core/models/models';

@Component({
  selector: 'app-leaderboard',
  standalone: true,
  imports: [CommonModule, SidebarComponent],
  template: `
    <div class="layout">
      <app-sidebar [user]="user"></app-sidebar>
      
      <main class="main-content">
        <header class="page-header">
          <div>
            <h1>Leaderboard 🏆</h1>
            <p>See how you rank among your peers</p>
          </div>
        </header>

        <div class="leaderboard-grid">
          <!-- Top 3 Podium -->
          <div class="podium-section animate-fade-in">
            @if (leaderboard.length >= 3) {
              <div class="podium">
                <!-- Second Place -->
                <div class="podium-item second">
                  <div class="avatar avatar-lg">
                    {{ leaderboard[1].userName.charAt(0) }}
                  </div>
                  <span class="position">🥈</span>
                  <h4>{{ leaderboard[1].userName }}</h4>
                  <span class="points">{{ leaderboard[1].totalPoints }} pts</span>
                </div>
                
                <!-- First Place -->
                <div class="podium-item first">
                  <div class="crown">👑</div>
                  <div class="avatar avatar-xl">
                    {{ leaderboard[0].userName.charAt(0) }}
                  </div>
                  <span class="position">🥇</span>
                  <h4>{{ leaderboard[0].userName }}</h4>
                  <span class="points">{{ leaderboard[0].totalPoints }} pts</span>
                </div>
                
                <!-- Third Place -->
                <div class="podium-item third">
                  <div class="avatar avatar-lg">
                    {{ leaderboard[2].userName.charAt(0) }}
                  </div>
                  <span class="position">🥉</span>
                  <h4>{{ leaderboard[2].userName }}</h4>
                  <span class="points">{{ leaderboard[2].totalPoints }} pts</span>
                </div>
              </div>
            }
          </div>

          <!-- Full Leaderboard -->
          <div class="card leaderboard-table">
            <div class="card-header">
              <h3 class="card-title">Rankings</h3>
            </div>

            <div class="table-container">
              <table>
                <thead>
                  <tr>
                    <th>Rank</th>
                    <th>User</th>
                    <th>Points</th>
                    <th>Tasks</th>
                    <th>Story Points</th>
                    <th>Streak</th>
                    <th>Badges</th>
                  </tr>
                </thead>
                <tbody>
                  @for (entry of leaderboard; track entry.userId) {
                    <tr [class.current-user]="entry.userId === user?.id">
                      <td class="rank-cell">
                        @if (entry.rank <= 3) {
                          <span class="rank-medal">{{ getRankMedal(entry.rank) }}</span>
                        } @else {
                          <span class="rank-number">#{{ entry.rank }}</span>
                        }
                      </td>
                      <td class="user-cell">
                        <div class="avatar avatar-sm">{{ entry.userName.charAt(0) }}</div>
                        <span>{{ entry.userName }}</span>
                        @if (entry.userId === user?.id) {
                          <span class="you-badge">You</span>
                        }
                      </td>
                      <td class="points-cell">
                        <span class="points-value">{{ entry.totalPoints }}</span>
                      </td>
                      <td>{{ entry.tasksCompleted }}</td>
                      <td>{{ entry.storyPointsDelivered }}</td>
                      <td>
                        @if (entry.currentStreak > 0) {
                          <span class="streak-badge">🔥 {{ entry.currentStreak }}</span>
                        } @else {
                          <span class="text-muted">-</span>
                        }
                      </td>
                      <td>
                        <span class="badge-icon">{{ entry.topBadgeIcon }}</span>
                        <span class="badge-count">{{ entry.badgeCount }}</span>
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          </div>

          <!-- Your Stats -->
          <div class="card your-stats">
            <div class="card-header">
              <h3 class="card-title">Your Stats</h3>
            </div>

            @if (myStats) {
              <div class="stats-grid">
                <div class="stat-item">
                  <span class="stat-icon">🎯</span>
                  <div class="stat-content">
                    <span class="stat-value">{{ myStats.totalTasksCompleted }}</span>
                    <span class="stat-label">Tasks Done</span>
                  </div>
                </div>
                <div class="stat-item">
                  <span class="stat-icon">📈</span>
                  <div class="stat-content">
                    <span class="stat-value">{{ myStats.totalStoryPoints }}</span>
                    <span class="stat-label">Story Points</span>
                  </div>
                </div>
                <div class="stat-item">
                  <span class="stat-icon">🔥</span>
                  <div class="stat-content">
                    <span class="stat-value">{{ myStats.currentStreak }}</span>
                    <span class="stat-label">Day Streak</span>
                  </div>
                </div>
                <div class="stat-item">
                  <span class="stat-icon">🔓</span>
                  <div class="stat-content">
                    <span class="stat-value">{{ myStats.blockersResolved }}</span>
                    <span class="stat-label">Blockers Solved</span>
                  </div>
                </div>
              </div>

              <div class="rank-progress">
                <div class="current-rank">
                  <span class="rank-label">Your Rank</span>
                  <span class="rank-value">#{{ myStats.rank }}</span>
                  <span class="rank-total">of {{ myStats.totalUsers }}</span>
                </div>
              </div>

              @if (myStats.recentBadges && myStats.recentBadges.length > 0) {
                <div class="recent-badges">
                  <h4>Recent Badges</h4>
                  <div class="badges-list">
                    @for (badge of myStats.recentBadges; track badge.id) {
                      <div class="badge-item" [title]="badge.description || ''">
                        <span class="badge-icon-lg">{{ badge.icon }}</span>
                        <span class="badge-name">{{ badge.name }}</span>
                      </div>
                    }
                  </div>
                </div>
              }
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
    
    .leaderboard-grid {
      display: grid;
      grid-template-columns: 1fr 350px;
      grid-template-rows: auto 1fr;
      gap: var(--space-lg);
      
      @media (max-width: 1200px) {
        grid-template-columns: 1fr;
      }
    }
    
    .podium-section {
      grid-column: 1 / -1;
      padding: var(--space-xl);
      background: linear-gradient(135deg, var(--bg-card), var(--bg-secondary));
      border: 1px solid var(--border-color);
      border-radius: var(--radius-lg);
    }
    
    .podium {
      display: flex;
      justify-content: center;
      align-items: flex-end;
      gap: var(--space-lg);
      padding-top: var(--space-xl);
    }
    
    .podium-item {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: var(--space-lg);
      background: var(--bg-tertiary);
      border-radius: var(--radius-lg);
      text-align: center;
      transition: transform 0.3s ease;
      
      &:hover {
        transform: translateY(-5px);
      }
      
      .position {
        font-size: 2rem;
        margin: var(--space-sm) 0;
      }
      
      h4 {
        margin-bottom: var(--space-xs);
      }
      
      .points {
        color: var(--accent-primary);
        font-weight: 600;
      }
      
      &.first {
        padding-bottom: var(--space-xl);
        background: linear-gradient(135deg, rgba(255, 215, 0, 0.1), var(--bg-tertiary));
        border: 1px solid rgba(255, 215, 0, 0.3);
        
        .crown {
          font-size: 2rem;
          animation: bounce 1s infinite;
        }
      }
      
      &.second {
        margin-bottom: 20px;
        background: linear-gradient(135deg, rgba(192, 192, 192, 0.1), var(--bg-tertiary));
      }
      
      &.third {
        margin-bottom: 40px;
        background: linear-gradient(135deg, rgba(205, 127, 50, 0.1), var(--bg-tertiary));
      }
    }
    
    @keyframes bounce {
      0%, 100% { transform: translateY(0); }
      50% { transform: translateY(-5px); }
    }
    
    .leaderboard-table {
      overflow: hidden;
    }
    
    .table-container {
      overflow-x: auto;
    }
    
    table {
      width: 100%;
      border-collapse: collapse;
      
      th, td {
        padding: var(--space-md);
        text-align: left;
        border-bottom: 1px solid var(--border-color);
      }
      
      th {
        font-size: 0.85rem;
        font-weight: 600;
        color: var(--text-muted);
        text-transform: uppercase;
        letter-spacing: 0.05em;
      }
      
      tbody tr {
        transition: background 0.2s ease;
        
        &:hover {
          background: var(--bg-hover);
        }
        
        &.current-user {
          background: var(--accent-glow);
        }
      }
    }
    
    .rank-cell {
      width: 60px;
      
      .rank-medal {
        font-size: 1.5rem;
      }
      
      .rank-number {
        font-weight: 600;
        color: var(--text-muted);
      }
    }
    
    .user-cell {
      display: flex;
      align-items: center;
      gap: var(--space-sm);
      
      .you-badge {
        font-size: 0.7rem;
        padding: 2px 6px;
        background: var(--accent-primary);
        color: white;
        border-radius: var(--radius-sm);
        margin-left: var(--space-sm);
      }
    }
    
    .points-cell {
      .points-value {
        font-weight: 700;
        font-size: 1.1rem;
        color: var(--accent-primary);
      }
    }
    
    .streak-badge {
      display: inline-flex;
      align-items: center;
      gap: var(--space-xs);
      padding: var(--space-xs) var(--space-sm);
      background: rgba(239, 68, 68, 0.1);
      border-radius: var(--radius-sm);
      font-weight: 600;
      color: var(--warning);
    }
    
    .badge-count {
      margin-left: var(--space-xs);
      color: var(--text-muted);
    }
    
    .your-stats {
      height: fit-content;
    }
    
    .stats-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--space-md);
      margin-bottom: var(--space-lg);
    }
    
    .stat-item {
      display: flex;
      align-items: center;
      gap: var(--space-md);
      padding: var(--space-md);
      background: var(--bg-tertiary);
      border-radius: var(--radius-md);
      
      .stat-icon {
        font-size: 1.5rem;
      }
      
      .stat-content {
        display: flex;
        flex-direction: column;
        
        .stat-value {
          font-size: 1.25rem;
          font-weight: 700;
        }
        
        .stat-label {
          font-size: 0.8rem;
          color: var(--text-muted);
        }
      }
    }
    
    .rank-progress {
      padding: var(--space-lg);
      background: linear-gradient(135deg, var(--accent-glow), var(--bg-tertiary));
      border-radius: var(--radius-md);
      text-align: center;
      margin-bottom: var(--space-lg);
    }
    
    .current-rank {
      .rank-label {
        display: block;
        font-size: 0.85rem;
        color: var(--text-muted);
        margin-bottom: var(--space-xs);
      }
      
      .rank-value {
        font-size: 3rem;
        font-weight: 700;
        background: linear-gradient(135deg, var(--accent-primary), var(--accent-tertiary));
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
        background-clip: text;
      }
      
      .rank-total {
        display: block;
        color: var(--text-muted);
      }
    }
    
    .recent-badges {
      h4 {
        font-size: 0.9rem;
        color: var(--text-muted);
        margin-bottom: var(--space-md);
      }
    }
    
    .badges-list {
      display: flex;
      flex-wrap: wrap;
      gap: var(--space-sm);
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
      
      .badge-icon-lg {
        font-size: 1.75rem;
      }
      
      .badge-name {
        font-size: 0.7rem;
        color: var(--text-muted);
        text-align: center;
      }
    }
  `]
})
export class LeaderboardComponent implements OnInit {
  user: User | null = null;
  leaderboard: LeaderboardEntry[] = [];
  myStats: DashboardStats | null = null;

  constructor(
    private router: Router,
    private authService: AuthService,
    private leaderboardService: LeaderboardService
  ) {}

  ngOnInit(): void {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }

    this.authService.currentUser$.subscribe(user => this.user = user);
    this.loadLeaderboard();
  }

  loadLeaderboard(): void {
    this.leaderboardService.getGlobalLeaderboard(20).subscribe({
      next: data => this.leaderboard = data
    });

    this.leaderboardService.getMyStats().subscribe({
      next: stats => this.myStats = stats
    });
  }

  getRankMedal(rank: number): string {
    switch (rank) {
      case 1: return '🥇';
      case 2: return '🥈';
      case 3: return '🥉';
      default: return '';
    }
  }
}
