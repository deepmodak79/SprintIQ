import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="auth-container">
      <div class="auth-card animate-fade-in">
        <div class="auth-header">
          <div class="logo">
            <span class="logo-icon">⚡</span>
            <span class="logo-text">SprintIQ</span>
          </div>
          <h1>Welcome Back</h1>
          <p>Sign in to continue to your dashboard</p>
        </div>

        <form (ngSubmit)="onSubmit()" class="auth-form">
          <div class="form-group">
            <label class="form-label">Email</label>
            <input
              type="email"
              class="form-input"
              [(ngModel)]="email"
              name="email"
              placeholder="you@example.com"
              required
            />
          </div>

          <div class="form-group">
            <label class="form-label">Password</label>
            <input
              type="password"
              class="form-input"
              [(ngModel)]="password"
              name="password"
              placeholder="••••••••"
              required
            />
          </div>

          @if (error) {
            <div class="error-message">
              {{ error }}
            </div>
          }

          <button type="submit" class="btn btn-primary btn-lg w-full" [disabled]="loading">
            @if (loading) {
              <span class="spinner"></span>
            } @else {
              Sign In
            }
          </button>
        </form>

        <div class="auth-footer">
          <p>Don't have an account? <a routerLink="/register">Sign up</a></p>
        </div>

        <div class="demo-credentials">
          <p>Demo: Create a new account or use the register page</p>
        </div>
      </div>

      <div class="auth-decoration">
        <div class="decoration-content">
          <h2>Sprint Visual Management</h2>
          <p>Track your team's progress, manage daily standups, and compete on the leaderboard.</p>
          <div class="features">
            <div class="feature">📊 Kanban Board</div>
            <div class="feature">📋 Daily Standups</div>
            <div class="feature">🏆 Leaderboard</div>
            <div class="feature">🤖 AI Summaries</div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .auth-container {
      min-height: 100vh;
      display: grid;
      grid-template-columns: 1fr 1fr;
      
      @media (max-width: 900px) {
        grid-template-columns: 1fr;
      }
    }
    
    .auth-card {
      display: flex;
      flex-direction: column;
      justify-content: center;
      padding: var(--space-2xl);
      max-width: 450px;
      margin: 0 auto;
      width: 100%;
    }
    
    .auth-header {
      text-align: center;
      margin-bottom: var(--space-xl);
      
      h1 {
        margin-top: var(--space-lg);
        margin-bottom: var(--space-sm);
      }
      
      p {
        color: var(--text-secondary);
      }
    }
    
    .logo {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: var(--space-sm);
      
      .logo-icon {
        font-size: 2.5rem;
      }
      
      .logo-text {
        font-size: 2rem;
        font-weight: 700;
        background: linear-gradient(135deg, var(--accent-primary), var(--accent-tertiary));
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
        background-clip: text;
      }
    }
    
    .auth-form {
      margin-bottom: var(--space-xl);
    }
    
    .error-message {
      padding: var(--space-md);
      background: rgba(239, 68, 68, 0.1);
      border: 1px solid rgba(239, 68, 68, 0.3);
      border-radius: var(--radius-md);
      color: var(--danger);
      margin-bottom: var(--space-md);
      font-size: 0.9rem;
    }
    
    .auth-footer {
      text-align: center;
      color: var(--text-secondary);
      
      a {
        color: var(--accent-primary);
        font-weight: 500;
      }
    }
    
    .demo-credentials {
      margin-top: var(--space-lg);
      padding: var(--space-md);
      background: var(--bg-tertiary);
      border-radius: var(--radius-md);
      text-align: center;
      font-size: 0.85rem;
      color: var(--text-muted);
    }
    
    .auth-decoration {
      background: linear-gradient(135deg, var(--accent-primary), var(--accent-tertiary));
      display: flex;
      align-items: center;
      justify-content: center;
      padding: var(--space-2xl);
      
      @media (max-width: 900px) {
        display: none;
      }
    }
    
    .decoration-content {
      text-align: center;
      color: white;
      max-width: 400px;
      
      h2 {
        font-size: 2.5rem;
        margin-bottom: var(--space-md);
        color: white;
      }
      
      p {
        font-size: 1.1rem;
        opacity: 0.9;
        margin-bottom: var(--space-xl);
        color: white;
      }
    }
    
    .features {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--space-md);
    }
    
    .feature {
      background: rgba(255, 255, 255, 0.15);
      padding: var(--space-md);
      border-radius: var(--radius-md);
      font-weight: 500;
    }
  `]
})
export class LoginComponent {
  email = '';
  password = '';
  loading = false;
  error = '';

  constructor(private authService: AuthService, private router: Router) {
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/dashboard']);
    }
  }

  onSubmit(): void {
    if (!this.email || !this.password) {
      this.error = 'Please fill in all fields';
      return;
    }

    this.loading = true;
    this.error = '';

    this.authService.login({ email: this.email, password: this.password }).subscribe({
      next: () => {
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Invalid email or password';
      }
    });
  }
}
