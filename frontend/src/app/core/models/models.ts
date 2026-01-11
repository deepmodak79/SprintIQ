// Auth Models
export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  userId: number;
  fullName: string;
  email: string;
  role: string;
  token: string;
  expiresAt: Date;
}

export interface User {
  id: number;
  fullName: string;
  email: string;
  role?: string;
  avatarUrl?: string;
  totalPoints: number;
  currentStreak: number;
  longestStreak: number;
  lastActiveAt?: Date;
  badges: Badge[];
}

export interface Badge {
  id: number;
  name: string;
  description?: string;
  icon: string;
  color: string;
  earnedAt: Date;
  count: number;
}

// Team Models
export interface Team {
  id: number;
  name: string;
  description?: string;
  createdAt: Date;
  memberCount: number;
  activeSprintCount: number;
  members: TeamMember[];
}

export interface TeamMember {
  id: number;
  userId: number;
  userName: string;
  userEmail?: string;
  avatarUrl?: string;
  role: string;
  joinedAt: Date;
  totalPoints: number;
}

// Sprint Models
export enum SprintStatus {
  Planning = 0,
  Active = 1,
  Completed = 2,
  Cancelled = 3
}

export interface Sprint {
  id: number;
  name: string;
  goal?: string;
  teamId: number;
  teamName: string;
  startDate: Date;
  endDate: Date;
  status: SprintStatus;
  totalStoryPoints: number;
  completedStoryPoints: number;
  totalTasks: number;
  completedTasks: number;
  progressPercentage: number;
  daysRemaining: number;
  tasks: SprintTask[];
  burndownData: BurndownData[];
}

export interface SprintSummary {
  id: number;
  name: string;
  goal?: string;
  status: SprintStatus;
  startDate: Date;
  endDate: Date;
  totalTasks: number;
  completedTasks: number;
  progressPercentage: number;
  daysRemaining: number;
}

export interface BurndownData {
  date: Date;
  remainingPoints: number;
  idealRemainingPoints: number;
  completedPoints: number;
}

// Task Models
export enum TaskStatus {
  Todo = 0,
  InProgress = 1,
  InReview = 2,
  Done = 3
}

export enum TaskPriority {
  Low = 0,
  Medium = 1,
  High = 2,
  Critical = 3
}

export interface SprintTask {
  id: number;
  title: string;
  description?: string;
  sprintId: number;
  assigneeId?: number;
  assigneeName?: string;
  assigneeAvatar?: string;
  status: TaskStatus;
  priority: TaskPriority;
  storyPoints: number;
  orderIndex: number;
  isBlocked: boolean;
  blockedReason?: string;
  createdAt: Date;
  startedAt?: Date;
  completedAt?: Date;
  blockerCount: number;
}

export interface KanbanBoard {
  todo: SprintTask[];
  inProgress: SprintTask[];
  inReview: SprintTask[];
  done: SprintTask[];
}

export interface CreateTask {
  title: string;
  description?: string;
  sprintId: number;
  assigneeId?: number;
  priority: TaskPriority;
  storyPoints: number;
}

// Standup Models
export interface DailyStandup {
  id: number;
  userId: number;
  userName: string;
  userAvatar?: string;
  sprintId: number;
  date: Date;
  yesterday?: string;
  today?: string;
  blockers?: string;
  mood?: number;
  confidence?: number;
  submittedAt: Date;
  aiSummary?: string;
  pointsEarned: number;
}

export interface CreateStandup {
  sprintId: number;
  yesterday?: string;
  today?: string;
  blockers?: string;
  mood?: number;
  confidence?: number;
}

export interface TeamStandupSummary {
  date: Date;
  totalMembers: number;
  submittedCount: number;
  averageMood: number;
  averageConfidence: number;
  standups: DailyStandup[];
  aiTeamSummary?: string;
  commonBlockers: string[];
}

// Blocker Models
export enum BlockerStatus {
  Open = 0,
  InProgress = 1,
  Resolved = 2,
  Escalated = 3
}

export enum BlockerSeverity {
  Low = 0,
  Medium = 1,
  High = 2,
  Critical = 3
}

export interface Blocker {
  id: number;
  description: string;
  taskId?: number;
  taskTitle?: string;
  reportedById: number;
  reportedByName: string;
  assignedToId?: number;
  assignedToName?: string;
  sprintId: number;
  status: BlockerStatus;
  severity: BlockerSeverity;
  resolution?: string;
  createdAt: Date;
  resolvedAt?: Date;
  aiSuggestion?: string;
  daysOpen: number;
}

// Leaderboard Models
export interface LeaderboardEntry {
  rank: number;
  userId: number;
  userName: string;
  avatarUrl?: string;
  totalPoints: number;
  tasksCompleted: number;
  storyPointsDelivered: number;
  currentStreak: number;
  blockersResolved: number;
  badgeCount: number;
  topBadgeIcon: string;
}

export interface TeamLeaderboard {
  teamId: number;
  teamName: string;
  members: LeaderboardEntry[];
  totalTeamPoints: number;
  averagePoints: number;
}

export interface DashboardStats {
  totalTasksCompleted: number;
  totalStoryPoints: number;
  currentStreak: number;
  totalPoints: number;
  rank: number;
  totalUsers: number;
  badgesEarned: number;
  blockersResolved: number;
  recentActivities: RecentActivity[];
  recentBadges: Badge[];
}

export interface RecentActivity {
  type: string;
  description: string;
  points: number;
  timestamp: Date;
}

export interface TeamDashboard {
  activeSprintId: number;
  activeSprintName: string;
  sprintProgress: number;
  daysRemaining: number;
  teamMemberCount: number;
  todayStandupCount: number;
  openBlockers: number;
  teamVelocity: number;
  averageTeamMood: number;
  averageTeamConfidence: number;
  sprintHealth: SprintHealth;
}

export interface SprintHealth {
  status: string;
  color: string;
  warnings: string[];
  recommendations: string[];
}
