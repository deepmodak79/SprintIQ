import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { User } from '../../../core/models/models';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  template: `
    <aside class="sidebar">
      <div class="sidebar-header">
        <a routerLink="/dashboard" class="logo">
          <span class="logo-icon">⚡</span>
          <span class="logo-text">SprintIQ</span>
        </a>
      </div>

      <nav class="sidebar-nav">
        <a routerLink="/dashboard" routerLinkActive="active" class="nav-item">
          <span class="nav-icon">📊</span>
          <span class="nav-label">Dashboard</span>
        </a>
        <a routerLink="/teams" routerLinkActive="active" class="nav-item">
          <span class="nav-icon">👥</span>
          <span class="nav-label">Teams</span>
        </a>
        <a routerLink="/standup" routerLinkActive="active" class="nav-item">
          <span class="nav-icon">📋</span>
          <span class="nav-label">Daily Standup</span>
        </a>
        <a routerLink="/leaderboard" routerLinkActive="active" class="nav-item">
          <span class="nav-icon">🏆</span>
          <span class="nav-label">Leaderboard</span>
        </a>
      </nav>

      <div class="sidebar-footer">
        @if (user) {
          <div class="user-info">
            <div class="avatar">
              {{ user.fullName.charAt(0) }}
            </div>
            <div class="user-details">
              <span class="user-name">{{ user.fullName }}</span>
              <span class="user-points">{{ user.totalPoints }} points</span>
            </div>
          </div>
        }
        <button class="btn btn-ghost btn-sm w-full" (click)="logout()">
          <span>🚪</span> Logout
        </button>
      </div>
    </aside>
  `,
  styles: [`
    .sidebar {
      width: 260px;
      height: 100vh;
      background: var(--bg-secondary);
      border-right: 1px solid var(--border-color);
      display: flex;
      flex-direction: column;
      position: fixed;
      left: 0;
      top: 0;
      z-index: 100;
    }
    
    .sidebar-header {
      padding: var(--space-lg);
      border-bottom: 1px solid var(--border-color);
    }
    
    .logo {
      display: flex;
      align-items: center;
      gap: var(--space-sm);
      text-decoration: none;
      
      .logo-icon {
        font-size: 1.75rem;
      }
      
      .logo-text {
        font-size: 1.5rem;
        font-weight: 700;
        background: linear-gradient(135deg, var(--accent-primary), var(--accent-tertiary));
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
        background-clip: text;
      }
    }
    
    .sidebar-nav {
      flex: 1;
      padding: var(--space-md);
      display: flex;
      flex-direction: column;
      gap: var(--space-xs);
    }
    
    .nav-item {
      display: flex;
      align-items: center;
      gap: var(--space-md);
      padding: var(--space-md);
      border-radius: var(--radius-md);
      color: var(--text-secondary);
      text-decoration: none;
      transition: all 0.2s ease;
      
      &:hover {
        background: var(--bg-tertiary);
        color: var(--text-primary);
      }
      
      &.active {
        background: var(--accent-glow);
        color: var(--accent-primary);
        
        .nav-icon {
          transform: scale(1.1);
        }
      }
      
      .nav-icon {
        font-size: 1.25rem;
        transition: transform 0.2s ease;
      }
      
      .nav-label {
        font-weight: 500;
      }
    }
    
    .sidebar-footer {
      padding: var(--space-md);
      border-top: 1px solid var(--border-color);
    }
    
    .user-info {
      display: flex;
      align-items: center;
      gap: var(--space-md);
      padding: var(--space-md);
      margin-bottom: var(--space-md);
      background: var(--bg-tertiary);
      border-radius: var(--radius-md);
    }
    
    .user-details {
      display: flex;
      flex-direction: column;
      
      .user-name {
        font-weight: 600;
        font-size: 0.9rem;
      }
      
      .user-points {
        font-size: 0.8rem;
        color: var(--accent-primary);
      }
    }
  `]
})
export class SidebarComponent {
  @Input() user: User | null = null;

  constructor(private authService: AuthService) {}

  logout(): void {
    this.authService.logout();
  }
}
