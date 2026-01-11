import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
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
          <h1>Create Account</h1>
          <p>Join SprintIQ and boost your team's productivity</p>
        </div>

        <form (ngSubmit)="onSubmit()" class="auth-form">
          <div class="form-group">
            <label class="form-label">Full Name</label>
            <input
              type="text"
              class="form-input"
              [(ngModel)]="fullName"
              name="fullName"
              placeholder="John Doe"
              required
            />
          </div>

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
              minlength="6"
            />
            <small class="text-muted">Minimum 6 characters</small>
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
              Create Account
            }
          </button>
        </form>

        <div class="auth-footer">
          <p>Already have an account? <a routerLink="/login">Sign in</a></p>
        </div>
      </div>

      <div class="auth-decoration">
        <div class="decoration-content">
          <h2>Start Your Journey</h2>
          <p>Create an account and start managing your sprints like a pro.</p>
          <div class="stats">
            <div class="stat">
              <span class="stat-value">10x</span>
              <span class="stat-label">Faster Standups</span>
            </div>
            <div class="stat">
              <span class="stat-value">50%</span>
              <span class="stat-label">Better Visibility</span>
            </div>
            <div class="stat">
              <span class="stat-value">∞</span>
              <span class="stat-label">Motivation</span>
            </div>
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
      
      small {
        display: block;
        margin-top: var(--space-xs);
        font-size: 0.8rem;
      }
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
    
    .auth-decoration {
      background: linear-gradient(135deg, var(--accent-secondary), var(--accent-tertiary));
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
    
    .stats {
      display: flex;
      gap: var(--space-lg);
      justify-content: center;
    }
    
    .stat {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: var(--space-md);
      background: rgba(255, 255, 255, 0.15);
      border-radius: var(--radius-md);
      min-width: 100px;
      
      .stat-value {
        font-size: 1.75rem;
        font-weight: 700;
      }
      
      .stat-label {
        font-size: 0.8rem;
        opacity: 0.9;
      }
    }
  `]
})
export class RegisterComponent {
  fullName = '';
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
    if (!this.fullName || !this.email || !this.password) {
      this.error = 'Please fill in all fields';
      return;
    }

    if (this.password.length < 6) {
      this.error = 'Password must be at least 6 characters';
      return;
    }

    this.loading = true;
    this.error = '';

    this.authService.register({
      fullName: this.fullName,
      email: this.email,
      password: this.password
    }).subscribe({
      next: () => {
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Registration failed. Please try again.';
      }
    });
  }
}
