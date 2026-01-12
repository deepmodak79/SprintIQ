import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';
import { AuthService } from '../../core/services/auth.service';
import { TeamService } from '../../core/services/team.service';
import { SprintService } from '../../core/services/sprint.service';
import { User, Team, Sprint, SprintStatus } from '../../core/models/models';

@Component({
  selector: 'app-teams',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent, RouterLink],
  template: `
    <div class="layout">
      <app-sidebar [user]="user"></app-sidebar>
      
      <main class="main-content">
        <header class="page-header">
          <div>
            <h1>Teams 👥</h1>
            <p>Manage your teams and sprints</p>
          </div>
          <button class="btn btn-primary" (click)="openCreateTeamModal()">
            + New Team
          </button>
        </header>

        <div class="teams-grid">
          @for (team of teams; track team.id) {
            <div class="card team-card animate-fade-in" (click)="selectTeam(team)">
              <div class="team-header">
                <div class="team-icon">{{ team.name.charAt(0) }}</div>
                <div class="team-info">
                  <h3>{{ team.name }}</h3>
                  <p>{{ team.description || 'No description' }}</p>
                </div>
              </div>

              <div class="team-stats">
                <div class="stat">
                  <span class="stat-value">{{ team.memberCount }}</span>
                  <span class="stat-label">Members</span>
                </div>
                <div class="stat">
                  <span class="stat-value">{{ team.activeSprintCount }}</span>
                  <span class="stat-label">Active Sprints</span>
                </div>
              </div>

              <div class="team-members">
                @for (member of team.members.slice(0, 5); track member.id) {
                  <div class="avatar avatar-sm" [title]="member.userName">
                    {{ member.userName.charAt(0) }}
                  </div>
                }
                @if (team.members.length > 5) {
                  <div class="avatar avatar-sm more">+{{ team.members.length - 5 }}</div>
                }
              </div>

              <div class="team-actions">
                <button class="btn btn-secondary btn-sm" (click)="openSprintModal(team); $event.stopPropagation()">
                  + Sprint
                </button>
                <button class="btn btn-ghost btn-sm" (click)="openMemberModal(team); $event.stopPropagation()">
                  + Member
                </button>
              </div>
            </div>
          }

          @if (teams.length === 0) {
            <div class="empty-state">
              <div class="empty-icon">👥</div>
              <h3>No Teams Yet</h3>
              <p>Create your first team to start managing sprints</p>
              <button class="btn btn-primary mt-md" (click)="openCreateTeamModal()">
                Create Team
              </button>
            </div>
          }
        </div>

        <!-- Selected Team Details -->
        @if (selectedTeam) {
          <div class="card team-details animate-fade-in">
            <div class="card-header">
              <h3 class="card-title">{{ selectedTeam.name }} - Sprints</h3>
              <button class="btn btn-primary btn-sm" (click)="openSprintModal(selectedTeam)">
                + New Sprint
              </button>
            </div>

            <div class="sprints-list">
              @for (sprint of sprints; track sprint.id) {
                <div class="sprint-item" [class.active]="sprint.status === SprintStatus.Active">
                  <div class="sprint-info">
                    <h4>{{ sprint.name }}</h4>
                    <p>{{ sprint.goal || 'No goal set' }}</p>
                    <div class="sprint-dates">
                      {{ sprint.startDate | date:'mediumDate' }} - {{ sprint.endDate | date:'mediumDate' }}
                    </div>
                  </div>
                  <div class="sprint-meta">
                    <span class="badge" [class]="'badge-' + getStatusClass(sprint.status)">
                      {{ getStatusLabel(sprint.status) }}
                    </span>
                    <div class="sprint-progress">
                      <div class="progress" style="width: 100px;">
                        <div class="progress-bar" [style.width.%]="sprint.progressPercentage"></div>
                      </div>
                      <span>{{ sprint.progressPercentage }}%</span>
                    </div>
                  </div>
                  <div class="sprint-actions">
                    @if (sprint.status === SprintStatus.Planning) {
                      <button class="btn btn-success btn-sm" (click)="startSprint(sprint)">
                        Start Sprint
                      </button>
                    }
                    @if (sprint.status === SprintStatus.Active) {
                      <a [routerLink]="['/sprint', sprint.id]" class="btn btn-primary btn-sm">
                        Open Board
                      </a>
                    }
                  </div>
                </div>
              }

              @if (sprints.length === 0) {
                <div class="empty-state">
                  <p>No sprints yet. Create one to get started!</p>
                </div>
              }
            </div>
          </div>
        }

        <!-- Create Team Modal -->
        @if (createTeamModal) {
          <div class="modal-overlay" (click)="createTeamModal = false">
            <div class="modal" (click)="$event.stopPropagation()">
              <div class="modal-header">
                <h3>Create New Team</h3>
                <button class="btn btn-ghost btn-icon" (click)="createTeamModal = false">✕</button>
              </div>
              
              <div class="form-group">
                <label class="form-label">Team Name *</label>
                <input type="text" class="form-input" [(ngModel)]="newTeam.name" placeholder="Engineering Team">
              </div>
              
              <div class="form-group">
                <label class="form-label">Description</label>
                <textarea class="form-input" [(ngModel)]="newTeam.description" placeholder="Team description..."></textarea>
              </div>
              
              <div class="modal-footer">
                <button class="btn btn-secondary" (click)="createTeamModal = false">Cancel</button>
                <button class="btn btn-primary" (click)="createTeam()" [disabled]="!newTeam.name">
                  Create Team
                </button>
              </div>
            </div>
          </div>
        }

        <!-- Create Sprint Modal -->
        @if (createSprintModal && sprintTeam) {
          <div class="modal-overlay" (click)="createSprintModal = false">
            <div class="modal" (click)="$event.stopPropagation()">
              <div class="modal-header">
                <h3>Create Sprint for {{ sprintTeam.name }}</h3>
                <button class="btn btn-ghost btn-icon" (click)="createSprintModal = false">✕</button>
              </div>
              
              <div class="form-group">
                <label class="form-label">Sprint Name *</label>
                <input type="text" class="form-input" [(ngModel)]="newSprint.name" placeholder="Sprint 1">
              </div>
              
              <div class="form-group">
                <label class="form-label">Sprint Goal</label>
                <textarea class="form-input" [(ngModel)]="newSprint.goal" placeholder="What do we want to achieve?"></textarea>
              </div>
              
              <div class="form-row">
                <div class="form-group">
                  <label class="form-label">Start Date *</label>
                  <input type="date" class="form-input" [(ngModel)]="newSprint.startDate">
                </div>
                <div class="form-group">
                  <label class="form-label">End Date *</label>
                  <input type="date" class="form-input" [(ngModel)]="newSprint.endDate">
                </div>
              </div>
              
              <div class="modal-footer">
                <button class="btn btn-secondary" (click)="createSprintModal = false">Cancel</button>
                <button class="btn btn-primary" (click)="createSprint()" [disabled]="!newSprint.name || !newSprint.startDate || !newSprint.endDate">
                  Create Sprint
                </button>
              </div>
            </div>
          </div>
        }

        <!-- Add Member Modal -->
        @if (addMemberModal && memberTeam) {
          <div class="modal-overlay" (click)="addMemberModal = false">
            <div class="modal" (click)="$event.stopPropagation()">
              <div class="modal-header">
                <h3>Add Member to {{ memberTeam.name }}</h3>
                <button class="btn btn-ghost btn-icon" (click)="addMemberModal = false">✕</button>
              </div>
              
              <div class="form-group">
                <label class="form-label">Select User</label>
                <select class="form-input" [(ngModel)]="selectedUserId">
                  <option [ngValue]="null">Select a user...</option>
                  @for (u of allUsers; track u.id) {
                    <option [value]="u.id">{{ u.fullName }} ({{ u.email }})</option>
                  }
                </select>
              </div>
              
              <div class="form-group">
                <label class="form-label">Role</label>
                <select class="form-input" [(ngModel)]="selectedRole">
                  <option value="Member">Member</option>
                  <option value="Lead">Team Lead</option>
                </select>
              </div>
              
              <div class="modal-footer">
                <button class="btn btn-secondary" (click)="addMemberModal = false">Cancel</button>
                <button class="btn btn-primary" (click)="addMember()" [disabled]="!selectedUserId">
                  Add Member
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
      
      p {
        color: var(--text-secondary);
      }
    }
    
    .teams-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
      gap: var(--space-lg);
      margin-bottom: var(--space-xl);
    }
    
    .team-card {
      cursor: pointer;
      transition: all 0.3s ease;
      
      &:hover {
        transform: translateY(-4px);
        box-shadow: var(--shadow-glow);
      }
    }
    
    .team-header {
      display: flex;
      gap: var(--space-md);
      margin-bottom: var(--space-md);
    }
    
    .team-icon {
      width: 50px;
      height: 50px;
      background: linear-gradient(135deg, var(--accent-primary), var(--accent-secondary));
      border-radius: var(--radius-md);
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.5rem;
      font-weight: 700;
      color: white;
    }
    
    .team-info {
      flex: 1;
      
      h3 {
        margin-bottom: var(--space-xs);
      }
      
      p {
        font-size: 0.9rem;
        color: var(--text-muted);
      }
    }
    
    .team-stats {
      display: flex;
      gap: var(--space-lg);
      padding: var(--space-md);
      background: var(--bg-tertiary);
      border-radius: var(--radius-md);
      margin-bottom: var(--space-md);
    }
    
    .stat {
      text-align: center;
      
      .stat-value {
        display: block;
        font-size: 1.5rem;
        font-weight: 700;
        color: var(--accent-primary);
      }
      
      .stat-label {
        font-size: 0.8rem;
        color: var(--text-muted);
      }
    }
    
    .team-members {
      display: flex;
      gap: -8px;
      margin-bottom: var(--space-md);
      
      .avatar {
        margin-left: -8px;
        border: 2px solid var(--bg-card);
        
        &:first-child {
          margin-left: 0;
        }
        
        &.more {
          background: var(--bg-tertiary);
          font-size: 0.7rem;
          color: var(--text-muted);
        }
      }
    }
    
    .team-actions {
      display: flex;
      gap: var(--space-sm);
    }
    
    .team-details {
      margin-top: var(--space-lg);
    }
    
    .sprints-list {
      display: flex;
      flex-direction: column;
      gap: var(--space-md);
    }
    
    .sprint-item {
      display: flex;
      align-items: center;
      gap: var(--space-lg);
      padding: var(--space-md);
      background: var(--bg-tertiary);
      border-radius: var(--radius-md);
      border-left: 3px solid var(--border-color);
      
      &.active {
        border-left-color: var(--success);
        background: rgba(16, 185, 129, 0.05);
      }
    }
    
    .sprint-info {
      flex: 1;
      
      h4 {
        margin-bottom: var(--space-xs);
      }
      
      p {
        font-size: 0.9rem;
        color: var(--text-muted);
        margin-bottom: var(--space-xs);
      }
      
      .sprint-dates {
        font-size: 0.8rem;
        color: var(--text-muted);
      }
    }
    
    .sprint-meta {
      display: flex;
      flex-direction: column;
      align-items: flex-end;
      gap: var(--space-sm);
    }
    
    .sprint-progress {
      display: flex;
      align-items: center;
      gap: var(--space-sm);
      
      span {
        font-size: 0.85rem;
        font-weight: 600;
      }
    }
    
    .form-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--space-md);
    }
  `]
})
export class TeamsComponent implements OnInit {
  user: User | null = null;
  teams: Team[] = [];
  selectedTeam: Team | null = null;
  sprints: any[] = [];
  allUsers: User[] = [];
  
  createTeamModal = false;
  createSprintModal = false;
  addMemberModal = false;
  
  sprintTeam: Team | null = null;
  memberTeam: Team | null = null;
  
  newTeam = { name: '', description: '' };
  newSprint = { name: '', goal: '', startDate: '', endDate: '' };
  selectedUserId: number | null = null;
  selectedRole = 'Member';
  
  SprintStatus = SprintStatus;

  constructor(
    private router: Router,
    private authService: AuthService,
    private teamService: TeamService,
    private sprintService: SprintService
  ) {}

  ngOnInit(): void {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }

    this.authService.currentUser$.subscribe(user => this.user = user);
    this.loadTeams();
    this.loadAllUsers();
  }

  loadTeams(): void {
    this.teamService.getMyTeams().subscribe({
      next: teams => {
        this.teams = teams;
        if (teams.length > 0 && !this.selectedTeam) {
          this.selectTeam(teams[0]);
        }
      }
    });
  }

  loadAllUsers(): void {
    this.teamService.getAllUsers().subscribe({
      next: users => this.allUsers = users
    });
  }

  selectTeam(team: Team): void {
    this.selectedTeam = team;
    this.loadSprints(team.id);
  }

  loadSprints(teamId: number): void {
    this.sprintService.getSprintsByTeam(teamId).subscribe({
      next: sprints => this.sprints = sprints
    });
  }

  openCreateTeamModal(): void {
    this.newTeam = { name: '', description: '' };
    this.createTeamModal = true;
  }

  createTeam(): void {
    if (!this.newTeam.name) return;

    this.teamService.createTeam(this.newTeam).subscribe({
      next: team => {
        this.teams.push(team);
        this.createTeamModal = false;
        this.selectTeam(team);
      }
    });
  }

  openSprintModal(team: Team): void {
    this.sprintTeam = team;
    this.newSprint = { name: '', goal: '', startDate: '', endDate: '' };
    this.createSprintModal = true;
  }

  createSprint(): void {
    if (!this.sprintTeam || !this.newSprint.name) return;

    this.sprintService.createSprint({
      name: this.newSprint.name,
      goal: this.newSprint.goal,
      teamId: this.sprintTeam.id,
      startDate: new Date(this.newSprint.startDate),
      endDate: new Date(this.newSprint.endDate)
    }).subscribe({
      next: () => {
        this.createSprintModal = false;
        if (this.selectedTeam) {
          this.loadSprints(this.selectedTeam.id);
          this.loadTeams();
        }
      }
    });
  }

  openMemberModal(team: Team): void {
    this.memberTeam = team;
    this.selectedUserId = null;
    this.selectedRole = 'Member';
    this.addMemberModal = true;
  }

  addMember(): void {
    if (!this.memberTeam || !this.selectedUserId) return;

    this.teamService.addMember(this.memberTeam.id, this.selectedUserId, this.selectedRole).subscribe({
      next: () => {
        this.addMemberModal = false;
        this.loadTeams();
      }
    });
  }

  startSprint(sprint: any): void {
    this.sprintService.updateSprint(sprint.id, { status: SprintStatus.Active }).subscribe({
      next: () => {
        if (this.selectedTeam) {
          this.loadSprints(this.selectedTeam.id);
        }
      }
    });
  }

  getStatusLabel(status: SprintStatus): string {
    switch (status) {
      case SprintStatus.Planning: return 'Planning';
      case SprintStatus.Active: return 'Active';
      case SprintStatus.Completed: return 'Completed';
      case SprintStatus.Cancelled: return 'Cancelled';
    }
  }

  getStatusClass(status: SprintStatus): string {
    switch (status) {
      case SprintStatus.Planning: return 'info';
      case SprintStatus.Active: return 'success';
      case SprintStatus.Completed: return 'primary';
      case SprintStatus.Cancelled: return 'danger';
    }
  }
}
