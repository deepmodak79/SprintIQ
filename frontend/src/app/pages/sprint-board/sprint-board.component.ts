import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CdkDragDrop, CdkDrag, CdkDropList, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';
import { AuthService } from '../../core/services/auth.service';
import { SprintService } from '../../core/services/sprint.service';
import { TaskService } from '../../core/services/task.service';
import { TeamService } from '../../core/services/team.service';
import { Sprint, SprintTask, KanbanBoard, TaskStatus, TaskPriority, User, BurndownData } from '../../core/models/models';

@Component({
  selector: 'app-sprint-board',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent, RouterLink, CdkDrag, CdkDropList],
  template: `
    <div class="layout">
      <app-sidebar [user]="user"></app-sidebar>
      
      <main class="main-content">
        @if (sprint) {
          <header class="page-header">
            <div>
              <div class="breadcrumb">
                <a routerLink="/dashboard">Dashboard</a>
                <span>/</span>
                <span>{{ sprint.name }}</span>
              </div>
              <h1>{{ sprint.name }}</h1>
              <p>{{ sprint.goal || 'Sprint Goal' }}</p>
            </div>
            <div class="header-actions">
              <button class="btn btn-secondary" (click)="showBurndown = !showBurndown">
                📈 {{ showBurndown ? 'Hide' : 'Show' }} Burndown
              </button>
              <button class="btn btn-primary" (click)="openCreateTaskModal()">
                + Add Task
              </button>
            </div>
          </header>

          <!-- Sprint Stats -->
          <div class="sprint-stats-bar">
            <div class="stat-item">
              <span class="stat-label">Progress</span>
              <div class="progress" style="width: 150px;">
                <div class="progress-bar" [style.width.%]="sprint.progressPercentage"></div>
              </div>
              <span class="stat-value">{{ sprint.progressPercentage }}%</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">Tasks</span>
              <span class="stat-value">{{ sprint.completedTasks }}/{{ sprint.totalTasks }}</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">Story Points</span>
              <span class="stat-value">{{ sprint.completedStoryPoints }}/{{ sprint.totalStoryPoints }}</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">Days Left</span>
              <span class="stat-value" [class.text-danger]="sprint.daysRemaining < 3">{{ sprint.daysRemaining }}</span>
            </div>
          </div>

          <!-- Burndown Chart (Simple CSS version) -->
          @if (showBurndown && burndownData.length > 0) {
            <div class="card burndown-chart animate-fade-in">
              <h3>Sprint Burndown</h3>
              <div class="burndown-visual">
                <div class="chart-container">
                  @for (point of burndownData; track point.date; let i = $index) {
                    <div class="chart-bar">
                      <div class="bar-wrapper">
                        <div class="bar ideal" [style.height.%]="getBarHeight(point.idealRemainingPoints)" title="Ideal: {{point.idealRemainingPoints}}"></div>
                        <div class="bar actual" [style.height.%]="getBarHeight(point.remainingPoints)" title="Actual: {{point.remainingPoints}}"></div>
                      </div>
                      <span class="bar-label">D{{ i + 1 }}</span>
                    </div>
                  }
                </div>
                <div class="chart-legend">
                  <span class="legend-item"><span class="dot ideal"></span> Ideal</span>
                  <span class="legend-item"><span class="dot actual"></span> Actual</span>
                </div>
              </div>
            </div>
          }

          <!-- Kanban Board -->
          <div class="kanban-board">
            <div class="kanban-column"
                 cdkDropList
                 #todoList="cdkDropList"
                 [cdkDropListData]="board.todo"
                 [cdkDropListConnectedTo]="[inProgressList, inReviewList, doneList]"
                 (cdkDropListDropped)="drop($event, TaskStatus.Todo)">
              <div class="column-header">
                <h3>📋 To Do</h3>
                <span class="task-count">{{ board.todo.length }}</span>
              </div>
              <div class="tasks-container">
                @for (task of board.todo; track task.id) {
                  <div class="task-card" cdkDrag [class.blocked]="task.isBlocked" (click)="openTaskModal(task)">
                    <div class="task-header">
                      <span class="task-priority" [class]="'priority-' + task.priority">
                        {{ getPriorityIcon(task.priority) }}
                      </span>
                      @if (task.isBlocked) {
                        <span class="blocked-badge">🚫 Blocked</span>
                      }
                    </div>
                    <h4 class="task-title">{{ task.title }}</h4>
                    <div class="task-footer">
                      @if (task.assigneeName) {
                        <div class="avatar avatar-sm">{{ task.assigneeName.charAt(0) }}</div>
                      }
                      <span class="story-points">{{ task.storyPoints }} SP</span>
                    </div>
                  </div>
                }
              </div>
            </div>

            <div class="kanban-column"
                 cdkDropList
                 #inProgressList="cdkDropList"
                 [cdkDropListData]="board.inProgress"
                 [cdkDropListConnectedTo]="[todoList, inReviewList, doneList]"
                 (cdkDropListDropped)="drop($event, TaskStatus.InProgress)">
              <div class="column-header">
                <h3>🔄 In Progress</h3>
                <span class="task-count">{{ board.inProgress.length }}</span>
              </div>
              <div class="tasks-container">
                @for (task of board.inProgress; track task.id) {
                  <div class="task-card" cdkDrag [class.blocked]="task.isBlocked" (click)="openTaskModal(task)">
                    <div class="task-header">
                      <span class="task-priority" [class]="'priority-' + task.priority">
                        {{ getPriorityIcon(task.priority) }}
                      </span>
                      @if (task.isBlocked) {
                        <span class="blocked-badge">🚫 Blocked</span>
                      }
                    </div>
                    <h4 class="task-title">{{ task.title }}</h4>
                    <div class="task-footer">
                      @if (task.assigneeName) {
                        <div class="avatar avatar-sm">{{ task.assigneeName.charAt(0) }}</div>
                      }
                      <span class="story-points">{{ task.storyPoints }} SP</span>
                    </div>
                  </div>
                }
              </div>
            </div>

            <div class="kanban-column"
                 cdkDropList
                 #inReviewList="cdkDropList"
                 [cdkDropListData]="board.inReview"
                 [cdkDropListConnectedTo]="[todoList, inProgressList, doneList]"
                 (cdkDropListDropped)="drop($event, TaskStatus.InReview)">
              <div class="column-header">
                <h3>👁️ In Review</h3>
                <span class="task-count">{{ board.inReview.length }}</span>
              </div>
              <div class="tasks-container">
                @for (task of board.inReview; track task.id) {
                  <div class="task-card" cdkDrag [class.blocked]="task.isBlocked" (click)="openTaskModal(task)">
                    <div class="task-header">
                      <span class="task-priority" [class]="'priority-' + task.priority">
                        {{ getPriorityIcon(task.priority) }}
                      </span>
                      @if (task.isBlocked) {
                        <span class="blocked-badge">🚫 Blocked</span>
                      }
                    </div>
                    <h4 class="task-title">{{ task.title }}</h4>
                    <div class="task-footer">
                      @if (task.assigneeName) {
                        <div class="avatar avatar-sm">{{ task.assigneeName.charAt(0) }}</div>
                      }
                      <span class="story-points">{{ task.storyPoints }} SP</span>
                    </div>
                  </div>
                }
              </div>
            </div>

            <div class="kanban-column done-column"
                 cdkDropList
                 #doneList="cdkDropList"
                 [cdkDropListData]="board.done"
                 [cdkDropListConnectedTo]="[todoList, inProgressList, inReviewList]"
                 (cdkDropListDropped)="drop($event, TaskStatus.Done)">
              <div class="column-header">
                <h3>✅ Done</h3>
                <span class="task-count">{{ board.done.length }}</span>
              </div>
              <div class="tasks-container">
                @for (task of board.done; track task.id) {
                  <div class="task-card completed" cdkDrag (click)="openTaskModal(task)">
                    <div class="task-header">
                      <span class="task-priority" [class]="'priority-' + task.priority">
                        {{ getPriorityIcon(task.priority) }}
                      </span>
                    </div>
                    <h4 class="task-title">{{ task.title }}</h4>
                    <div class="task-footer">
                      @if (task.assigneeName) {
                        <div class="avatar avatar-sm">{{ task.assigneeName.charAt(0) }}</div>
                      }
                      <span class="story-points">{{ task.storyPoints }} SP</span>
                    </div>
                  </div>
                }
              </div>
            </div>
          </div>
        } @else {
          <div class="loading-state">
            <div class="spinner"></div>
            <p>Loading sprint...</p>
          </div>
        }

        <!-- Create Task Modal -->
        @if (createTaskModal) {
          <div class="modal-overlay" (click)="createTaskModal = false">
            <div class="modal" (click)="$event.stopPropagation()">
              <div class="modal-header">
                <h3>Create New Task</h3>
                <button class="btn btn-ghost btn-icon" (click)="createTaskModal = false">✕</button>
              </div>
              
              <div class="form-group">
                <label class="form-label">Title *</label>
                <input type="text" class="form-input" [(ngModel)]="newTask.title" placeholder="Task title">
              </div>
              
              <div class="form-group">
                <label class="form-label">Description</label>
                <textarea class="form-input" [(ngModel)]="newTask.description" placeholder="Task description..."></textarea>
              </div>
              
              <div class="form-row">
                <div class="form-group">
                  <label class="form-label">Priority</label>
                  <select class="form-input" [(ngModel)]="newTask.priority">
                    <option [value]="TaskPriority.Low">Low</option>
                    <option [value]="TaskPriority.Medium">Medium</option>
                    <option [value]="TaskPriority.High">High</option>
                    <option [value]="TaskPriority.Critical">Critical</option>
                  </select>
                </div>
                <div class="form-group">
                  <label class="form-label">Story Points</label>
                  <input type="number" class="form-input" [(ngModel)]="newTask.storyPoints" min="0" max="21">
                </div>
              </div>
              
              <div class="form-group">
                <label class="form-label">Assignee</label>
                <select class="form-input" [(ngModel)]="newTask.assigneeId">
                  <option [ngValue]="null">Unassigned</option>
                  @for (member of teamMembers; track member.id) {
                    <option [value]="member.id">{{ member.fullName }}</option>
                  }
                </select>
              </div>
              
              <div class="modal-footer">
                <button class="btn btn-secondary" (click)="createTaskModal = false">Cancel</button>
                <button class="btn btn-primary" (click)="createTask()" [disabled]="!newTask.title">
                  Create Task
                </button>
              </div>
            </div>
          </div>
        }

        <!-- Task Detail Modal -->
        @if (selectedTask) {
          <div class="modal-overlay" (click)="selectedTask = null">
            <div class="modal task-modal" (click)="$event.stopPropagation()">
              <div class="modal-header">
                <h3>{{ selectedTask.title }}</h3>
                <button class="btn btn-ghost btn-icon" (click)="selectedTask = null">✕</button>
              </div>
              
              <div class="task-detail-content">
                <div class="task-meta">
                  <span class="badge" [class]="'badge-' + getPriorityClass(selectedTask.priority)">
                    {{ getPriorityLabel(selectedTask.priority) }}
                  </span>
                  <span class="badge badge-info">{{ selectedTask.storyPoints }} SP</span>
                  @if (selectedTask.isBlocked) {
                    <span class="badge badge-danger">Blocked</span>
                  }
                </div>
                
                @if (selectedTask.description) {
                  <div class="task-description">
                    <h4>Description</h4>
                    <p>{{ selectedTask.description }}</p>
                  </div>
                }
                
                <div class="task-info-grid">
                  <div class="info-item">
                    <span class="info-label">Status</span>
                    <span class="info-value">{{ getStatusLabel(selectedTask.status) }}</span>
                  </div>
                  <div class="info-item">
                    <span class="info-label">Assignee</span>
                    <span class="info-value">{{ selectedTask.assigneeName || 'Unassigned' }}</span>
                  </div>
                  <div class="info-item">
                    <span class="info-label">Created</span>
                    <span class="info-value">{{ selectedTask.createdAt | date:'short' }}</span>
                  </div>
                  @if (selectedTask.completedAt) {
                    <div class="info-item">
                      <span class="info-label">Completed</span>
                      <span class="info-value">{{ selectedTask.completedAt | date:'short' }}</span>
                    </div>
                  }
                </div>
              </div>
              
              <div class="modal-footer">
                <button class="btn btn-danger" (click)="deleteTask(selectedTask.id)">Delete</button>
                <button class="btn btn-secondary" (click)="selectedTask = null">Close</button>
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
      overflow-x: auto;
    }
    
    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: var(--space-lg);
    }
    
    .breadcrumb {
      display: flex;
      align-items: center;
      gap: var(--space-sm);
      margin-bottom: var(--space-sm);
      font-size: 0.9rem;
      color: var(--text-muted);
      
      a {
        color: var(--text-secondary);
        
        &:hover {
          color: var(--accent-primary);
        }
      }
    }
    
    .header-actions {
      display: flex;
      gap: var(--space-md);
    }
    
    .sprint-stats-bar {
      display: flex;
      gap: var(--space-xl);
      padding: var(--space-md) var(--space-lg);
      background: var(--bg-card);
      border: 1px solid var(--border-color);
      border-radius: var(--radius-lg);
      margin-bottom: var(--space-lg);
    }
    
    .stat-item {
      display: flex;
      align-items: center;
      gap: var(--space-sm);
      
      .stat-label {
        color: var(--text-muted);
        font-size: 0.85rem;
      }
      
      .stat-value {
        font-weight: 600;
        color: var(--text-primary);
      }
    }
    
    .burndown-chart {
      margin-bottom: var(--space-lg);
      padding: var(--space-lg);
      
      h3 {
        margin-bottom: var(--space-md);
      }
    }
    
    .burndown-visual {
      padding: var(--space-md);
    }
    
    .chart-container {
      display: flex;
      align-items: flex-end;
      justify-content: space-around;
      height: 200px;
      padding: var(--space-md);
      background: var(--bg-tertiary);
      border-radius: var(--radius-md);
      gap: var(--space-sm);
    }
    
    .chart-bar {
      display: flex;
      flex-direction: column;
      align-items: center;
      flex: 1;
      max-width: 60px;
    }
    
    .bar-wrapper {
      display: flex;
      gap: 4px;
      height: 150px;
      align-items: flex-end;
    }
    
    .bar {
      width: 20px;
      border-radius: 4px 4px 0 0;
      transition: height 0.3s ease;
      
      &.ideal {
        background: var(--text-muted);
        opacity: 0.5;
      }
      
      &.actual {
        background: linear-gradient(180deg, var(--accent-primary), var(--accent-secondary));
      }
    }
    
    .bar-label {
      margin-top: var(--space-sm);
      font-size: 0.75rem;
      color: var(--text-muted);
    }
    
    .chart-legend {
      display: flex;
      justify-content: center;
      gap: var(--space-lg);
      margin-top: var(--space-md);
    }
    
    .legend-item {
      display: flex;
      align-items: center;
      gap: var(--space-sm);
      font-size: 0.85rem;
      color: var(--text-secondary);
      
      .dot {
        width: 12px;
        height: 12px;
        border-radius: 3px;
        
        &.ideal {
          background: var(--text-muted);
          opacity: 0.5;
        }
        
        &.actual {
          background: var(--accent-primary);
        }
      }
    }
    
    .kanban-board {
      display: grid;
      grid-template-columns: repeat(4, 1fr);
      gap: var(--space-md);
      min-height: 500px;
      
      @media (max-width: 1200px) {
        grid-template-columns: repeat(2, 1fr);
      }
      
      @media (max-width: 768px) {
        grid-template-columns: 1fr;
      }
    }
    
    .kanban-column {
      background: var(--bg-secondary);
      border-radius: var(--radius-lg);
      padding: var(--space-md);
      min-height: 400px;
    }
    
    .done-column {
      background: rgba(16, 185, 129, 0.05);
    }
    
    .column-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: var(--space-md);
      padding-bottom: var(--space-md);
      border-bottom: 1px solid var(--border-color);
      
      h3 {
        font-size: 1rem;
        font-weight: 600;
      }
      
      .task-count {
        background: var(--bg-tertiary);
        padding: var(--space-xs) var(--space-sm);
        border-radius: var(--radius-sm);
        font-size: 0.85rem;
        font-weight: 600;
      }
    }
    
    .tasks-container {
      display: flex;
      flex-direction: column;
      gap: var(--space-sm);
      min-height: 100px;
    }
    
    .task-card {
      background: var(--bg-card);
      border: 1px solid var(--border-color);
      border-radius: var(--radius-md);
      padding: var(--space-md);
      cursor: pointer;
      transition: all 0.2s ease;
      
      &:hover {
        border-color: var(--accent-primary);
        transform: translateY(-2px);
        box-shadow: var(--shadow-md);
      }
      
      &.blocked {
        border-left: 3px solid var(--danger);
        background: rgba(239, 68, 68, 0.05);
      }
      
      &.completed {
        opacity: 0.7;
        
        .task-title {
          text-decoration: line-through;
        }
      }
    }
    
    .task-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: var(--space-sm);
    }
    
    .task-priority {
      font-size: 0.9rem;
      
      &.priority-0 { color: var(--text-muted); }
      &.priority-1 { color: var(--info); }
      &.priority-2 { color: var(--warning); }
      &.priority-3 { color: var(--danger); }
    }
    
    .blocked-badge {
      font-size: 0.7rem;
      color: var(--danger);
    }
    
    .task-title {
      font-size: 0.95rem;
      font-weight: 500;
      margin-bottom: var(--space-sm);
      line-height: 1.4;
    }
    
    .task-footer {
      display: flex;
      justify-content: space-between;
      align-items: center;
      
      .story-points {
        font-size: 0.8rem;
        color: var(--text-muted);
        background: var(--bg-tertiary);
        padding: var(--space-xs) var(--space-sm);
        border-radius: var(--radius-sm);
      }
    }
    
    .loading-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 400px;
      gap: var(--space-md);
      color: var(--text-muted);
    }
    
    .form-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--space-md);
    }
    
    .task-modal {
      max-width: 600px;
    }
    
    .task-meta {
      display: flex;
      gap: var(--space-sm);
      margin-bottom: var(--space-lg);
    }
    
    .task-description {
      margin-bottom: var(--space-lg);
      
      h4 {
        font-size: 0.9rem;
        color: var(--text-muted);
        margin-bottom: var(--space-sm);
      }
      
      p {
        color: var(--text-primary);
      }
    }
    
    .task-info-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: var(--space-md);
    }
    
    .info-item {
      .info-label {
        display: block;
        font-size: 0.8rem;
        color: var(--text-muted);
        margin-bottom: var(--space-xs);
      }
      
      .info-value {
        font-weight: 500;
      }
    }
    
    // CDK Drag styles
    .cdk-drag-preview {
      box-sizing: border-box;
      border-radius: var(--radius-md);
      box-shadow: var(--shadow-lg);
    }
    
    .cdk-drag-placeholder {
      opacity: 0.3;
    }
    
    .cdk-drag-animating {
      transition: transform 250ms cubic-bezier(0, 0, 0.2, 1);
    }
    
    .tasks-container.cdk-drop-list-dragging .task-card:not(.cdk-drag-placeholder) {
      transition: transform 250ms cubic-bezier(0, 0, 0.2, 1);
    }
  `]
})
export class SprintBoardComponent implements OnInit {
  user: User | null = null;
  sprint: Sprint | null = null;
  board: KanbanBoard = { todo: [], inProgress: [], inReview: [], done: [] };
  teamMembers: User[] = [];
  burndownData: BurndownData[] = [];
  maxPoints = 0;
  
  showBurndown = false;
  createTaskModal = false;
  selectedTask: SprintTask | null = null;
  
  newTask = {
    title: '',
    description: '',
    priority: TaskPriority.Medium,
    storyPoints: 1,
    assigneeId: null as number | null
  };

  TaskStatus = TaskStatus;
  TaskPriority = TaskPriority;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private sprintService: SprintService,
    private taskService: TaskService,
    private teamService: TeamService
  ) {}

  ngOnInit(): void {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }

    this.authService.currentUser$.subscribe(user => this.user = user);

    this.route.params.subscribe(params => {
      const sprintId = +params['id'];
      if (sprintId) {
        this.loadSprint(sprintId);
      }
    });
  }

  loadSprint(sprintId: number): void {
    this.sprintService.getSprint(sprintId).subscribe({
      next: sprint => {
        this.sprint = sprint;
        this.loadKanbanBoard(sprintId);
        this.loadBurndownData(sprintId);
        this.loadTeamMembers(sprint.teamId);
      },
      error: () => this.router.navigate(['/dashboard'])
    });
  }

  loadKanbanBoard(sprintId: number): void {
    this.taskService.getKanbanBoard(sprintId).subscribe({
      next: board => this.board = board
    });
  }

  loadBurndownData(sprintId: number): void {
    this.sprintService.getBurndownData(sprintId).subscribe({
      next: data => {
        this.burndownData = data;
        if (data.length > 0) {
          this.maxPoints = Math.max(...data.map(d => Math.max(d.remainingPoints, d.idealRemainingPoints)));
        }
      }
    });
  }

  loadTeamMembers(teamId: number): void {
    this.teamService.getTeam(teamId).subscribe({
      next: team => {
        this.teamMembers = team.members.map(m => ({
          id: m.userId,
          fullName: m.userName,
          email: m.userEmail || '',
          totalPoints: m.totalPoints,
          currentStreak: 0,
          longestStreak: 0,
          badges: []
        }));
      }
    });
  }

  getBarHeight(points: number): number {
    if (this.maxPoints === 0) return 0;
    return (points / this.maxPoints) * 100;
  }

  drop(event: CdkDragDrop<SprintTask[]>, newStatus: TaskStatus): void {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );
      
      const task = event.container.data[event.currentIndex];
      this.taskService.moveTask(task.id, newStatus, event.currentIndex).subscribe({
        next: updatedTask => {
          Object.assign(task, updatedTask);
          if (this.sprint) {
            this.loadSprint(this.sprint.id);
          }
        }
      });
    }
  }

  openCreateTaskModal(): void {
    this.newTask = {
      title: '',
      description: '',
      priority: TaskPriority.Medium,
      storyPoints: 1,
      assigneeId: null
    };
    this.createTaskModal = true;
  }

  createTask(): void {
    if (!this.sprint || !this.newTask.title) return;

    this.taskService.createTask({
      title: this.newTask.title,
      description: this.newTask.description,
      sprintId: this.sprint.id,
      assigneeId: this.newTask.assigneeId || undefined,
      priority: this.newTask.priority,
      storyPoints: this.newTask.storyPoints
    }).subscribe({
      next: task => {
        this.board.todo.push(task);
        this.createTaskModal = false;
        if (this.sprint) {
          this.loadSprint(this.sprint.id);
        }
      }
    });
  }

  openTaskModal(task: SprintTask): void {
    this.selectedTask = task;
  }

  deleteTask(taskId: number): void {
    this.taskService.deleteTask(taskId).subscribe({
      next: () => {
        this.selectedTask = null;
        if (this.sprint) {
          this.loadKanbanBoard(this.sprint.id);
          this.loadSprint(this.sprint.id);
        }
      }
    });
  }

  getPriorityIcon(priority: TaskPriority): string {
    switch (priority) {
      case TaskPriority.Critical: return '🔴';
      case TaskPriority.High: return '🟠';
      case TaskPriority.Medium: return '🟡';
      default: return '🟢';
    }
  }

  getPriorityLabel(priority: TaskPriority): string {
    switch (priority) {
      case TaskPriority.Critical: return 'Critical';
      case TaskPriority.High: return 'High';
      case TaskPriority.Medium: return 'Medium';
      default: return 'Low';
    }
  }

  getPriorityClass(priority: TaskPriority): string {
    switch (priority) {
      case TaskPriority.Critical: return 'danger';
      case TaskPriority.High: return 'warning';
      case TaskPriority.Medium: return 'info';
      default: return 'primary';
    }
  }

  getStatusLabel(status: TaskStatus): string {
    switch (status) {
      case TaskStatus.Todo: return 'To Do';
      case TaskStatus.InProgress: return 'In Progress';
      case TaskStatus.InReview: return 'In Review';
      case TaskStatus.Done: return 'Done';
    }
  }
}
