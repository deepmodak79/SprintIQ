# SprintIQ 2.0 - Complete Technical & Non-Technical Documentation

---

## 📋 Table of Contents

1. [Executive Summary](#executive-summary)
2. [Project Overview](#project-overview)
3. [Features & Capabilities](#features--capabilities)
4. [Technical Architecture](#technical-architecture)
5. [Backend Documentation](#backend-documentation)
6. [Frontend Documentation](#frontend-documentation)
7. [Database Schema](#database-schema)
8. [Authentication & Authorization](#authentication--authorization)
9. [API Documentation](#api-documentation)
10. [Jira Integration](#jira-integration)
11. [AI Analytics Engine](#ai-analytics-engine)
12. [Setup & Installation](#setup--installation)
13. [Development Workflow](#development-workflow)
14. [Deployment Guide](#deployment-guide)
15. [Best Practices](#best-practices)
16. [Future Roadmap](#future-roadmap)

---

## 1. Executive Summary

**SprintIQ 2.0** is an advanced Visual Management and Daily Management (VMDM) platform that integrates seamlessly with Jira to provide AI-powered insights for agile teams. Unlike traditional project management tools, SprintIQ focuses on real-time team health, predictive analytics, and intelligent automation to improve sprint outcomes.

### Key Differentiators:
- **Jira-Powered**: Integrates with existing Jira workflows instead of replacing them
- **AI-Driven**: Uses machine learning for blocker prediction, risk analysis, and team health monitoring
- **Real-Time Insights**: Live sprint analytics and team performance metrics
- **Gamification**: Engaging leaderboard system with badges and achievements
- **Developer-Friendly**: Modern tech stack (Angular 18 + .NET 9 + SQLite)

---

## 2. Project Overview

### Vision
Transform sprint management from reactive to proactive by providing teams with AI-powered insights that predict problems before they occur.

### Target Audience
- Agile Development Teams
- Scrum Masters & Product Owners
- Engineering Managers
- DevOps Teams

### Problem Statement
Traditional sprint management tools are reactive - teams discover problems too late in the sprint. SprintIQ 2.0 uses AI to predict blockers, identify risks, and optimize team capacity **before** issues impact delivery.

### Solution
A lightweight VMDM platform that:
1. Connects to your existing Jira workspace
2. Analyzes historical sprint data with AI
3. Provides real-time predictions and recommendations
4. Gamifies team engagement and performance

---

## 3. Features & Capabilities

### 3.1 Core Features (MVP)

#### 🎯 Sprint Management
- **Kanban Board**: Visual task management with drag-and-drop
- **Sprint Planning**: Create and configure sprints with goals
- **Burndown Charts**: Real-time progress tracking
- **Task Management**: Create, assign, and track tasks

#### 👥 Team Collaboration
- **Daily Standup Logger**: Quick status updates (Yesterday/Today/Blockers)
- **Team Management**: Create teams, invite members, assign roles
- **Blocker Tracking**: Identify and resolve impediments
- **Activity Feed**: Real-time team activity updates

#### 🏆 Gamification & Leaderboard
- **Points System**: Earn points for completing tasks, standups, resolving blockers
- **Badges & Achievements**: 10+ badges (Task Master, Streak Legend, Blocker Buster, etc.)
- **Team Rankings**: Weekly/monthly leaderboards
- **Streak Tracking**: Encourage consistent daily participation

### 3.2 SprintIQ 2.0 Features (Advanced)

#### 🔌 Jira Integration
- **OAuth Authentication**: Secure connection to Jira workspaces
- **Bi-Directional Sync**: Pull Jira issues, push updates back
- **Real-Time Webhooks**: Instant synchronization of changes
- **Multi-Project Support**: Sync multiple Jira projects simultaneously
- **User Mapping**: Link Jira users to SprintIQ team members

#### 🤖 AI-Powered Analytics

**Sprint Risk Analysis**
- Predicts sprint completion probability
- Identifies at-risk user stories
- Calculates sprint health score (0-100)
- Provides actionable recommendations

**Blocker Prediction**
- Uses ML to identify issues likely to become blocked
- Analyzes patterns: stuck time, comment frequency, status changes
- Provides early warning alerts

**Team Health Metrics**
- Workload distribution analysis
- Burnout risk detection
- Velocity trend tracking
- Capacity utilization monitoring

**Smart Daily Standup**
- Auto-generates standup summaries from Jira activity
- Detects sentiment trends in team updates
- Highlights potential issues early

**Dependency Graphs**
- Visualizes task dependencies
- Identifies critical path
- Detects circular dependencies

#### 📊 Advanced Visualizations
- Team Capacity Heatmaps
- Sprint Velocity Trends
- Work Distribution Fairness Charts
- Burnout Risk Indicators
- Predictive Sprint Completion Timeline

---

## 4. Technical Architecture

### 4.1 System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        CLIENT LAYER                          │
│  ┌─────────────────────────────────────────────────────┐    │
│  │   Angular 18 SPA (TypeScript)                       │    │
│  │   - Standalone Components                           │    │
│  │   - Lazy Loading                                    │    │
│  │   - RxJS State Management                           │    │
│  │   - Angular Material UI                             │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                              ↕ HTTP/HTTPS
┌─────────────────────────────────────────────────────────────┐
│                      API GATEWAY LAYER                       │
│  ┌─────────────────────────────────────────────────────┐    │
│  │   ASP.NET Core 9 Web API                            │    │
│  │   - RESTful APIs                                    │    │
│  │   - JWT Authentication                              │    │
│  │   - Swagger/OpenAPI                                 │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                              ↕
┌─────────────────────────────────────────────────────────────┐
│                     SERVICE LAYER                            │
│  ┌───────────┐ ┌───────────┐ ┌───────────┐ ┌──────────┐    │
│  │  Auth     │ │  Team     │ │  Sprint   │ │  Task    │    │
│  │  Service  │ │  Service  │ │  Service  │ │  Service │    │
│  └───────────┘ └───────────┘ └───────────┘ └──────────┘    │
│  ┌───────────┐ ┌───────────┐ ┌───────────┐ ┌──────────┐    │
│  │  Standup  │ │  Blocker  │ │Leaderboard│ │   AI     │    │
│  │  Service  │ │  Service  │ │  Service  │ │  Service │    │
│  └───────────┘ └───────────┘ └───────────┘ └──────────┘    │
│  ┌───────────┐                                               │
│  │   Jira    │                                               │
│  │  Service  │                                               │
│  └───────────┘                                               │
└─────────────────────────────────────────────────────────────┘
                              ↕
┌─────────────────────────────────────────────────────────────┐
│                    DATA ACCESS LAYER                         │
│  ┌─────────────────────────────────────────────────────┐    │
│  │   Entity Framework Core 9                           │    │
│  │   - Code-First Migrations                           │    │
│  │   - DbContext                                       │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                              ↕
┌─────────────────────────────────────────────────────────────┐
│                      DATABASE LAYER                          │
│  ┌─────────────────────────────────────────────────────┐    │
│  │   SQLite (Development/Production)                   │    │
│  │   - Lightweight & Portable                          │    │
│  │   - No separate server required                     │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                              ↕
┌─────────────────────────────────────────────────────────────┐
│                  EXTERNAL INTEGRATIONS                       │
│  ┌───────────┐ ┌───────────┐                                │
│  │   Jira    │ │  OpenAI   │                                │
│  │   REST    │ │   API     │                                │
│  │   API     │ │           │                                │
│  └───────────┘ └───────────┘                                │
└─────────────────────────────────────────────────────────────┘
```

### 4.2 Technology Stack

#### Frontend
- **Framework**: Angular 18 (Latest)
- **Language**: TypeScript 5.x
- **UI Library**: Angular Material + Custom CSS/Tailwind
- **State Management**: RxJS (BehaviorSubject, Observables)
- **Charts**: ng2-charts (Chart.js wrapper)
- **HTTP Client**: Angular HttpClient
- **Routing**: Angular Router with Lazy Loading
- **Build Tool**: Angular CLI, Webpack

#### Backend
- **Framework**: ASP.NET Core 9 Web API
- **Language**: C# 12
- **ORM**: Entity Framework Core 9
- **Database**: SQLite
- **Authentication**: JWT (JSON Web Tokens)
- **API Documentation**: Swagger/OpenAPI (Swashbuckle)
- **Dependency Injection**: Built-in ASP.NET Core DI
- **Logging**: ILogger (Microsoft.Extensions.Logging)

#### DevOps & Tools
- **Version Control**: Git + GitHub
- **IDE**: Visual Studio Code / Visual Studio 2022
- **API Testing**: Swagger UI, Postman
- **Package Managers**: npm (frontend), NuGet (backend)

---

## 5. Backend Documentation

### 5.1 Project Structure

```
backend/SprintIQ.API/
├── Controllers/          # API Controllers
│   ├── AuthController.cs
│   ├── TeamsController.cs
│   ├── SprintsController.cs
│   ├── TasksController.cs
│   ├── StandupsController.cs
│   ├── BlockersController.cs
│   ├── LeaderboardController.cs
│   ├── JiraController.cs
│   └── AnalyticsController.cs
├── Models/              # Domain Models
│   ├── User.cs
│   ├── Team.cs
│   ├── Sprint.cs
│   ├── SprintTask.cs
│   ├── DailyStandup.cs
│   ├── Blocker.cs
│   ├── Badge.cs
│   ├── UserBadge.cs
│   ├── JiraIntegration.cs
│   └── Enums.cs
├── DTOs/                # Data Transfer Objects
│   ├── AuthDTOs.cs
│   ├── TeamDTOs.cs
│   ├── SprintDTOs.cs
│   ├── TaskDTOs.cs
│   ├── StandupDTOs.cs
│   ├── BlockerDTOs.cs
│   ├── LeaderboardDTOs.cs
│   ├── JiraDTOs.cs
│   └── AiAnalyticsDTOs.cs
├── Services/            # Business Logic
│   ├── IAuthService.cs / AuthService.cs
│   ├── ITeamService.cs / TeamService.cs
│   ├── ISprintService.cs / SprintService.cs
│   ├── ITaskService.cs / TaskService.cs
│   ├── IStandupService.cs / StandupService.cs
│   ├── IBlockerService.cs / BlockerService.cs
│   ├── ILeaderboardService.cs / LeaderboardService.cs
│   ├── IJiraService.cs / JiraService.cs
│   └── IAiService.cs / AiService.cs
├── Data/                # Database Context
│   └── SprintIQDbContext.cs
├── Migrations/          # EF Core Migrations
├── Program.cs           # App Entry Point
└── appsettings.json     # Configuration
```

### 5.2 Key Services

#### AuthService
**Responsibilities:**
- User registration and login
- Password hashing (BCrypt)
- JWT token generation and validation
- User authentication

**Key Methods:**
```csharp
Task<User?> RegisterAsync(RegisterDto dto)
Task<string?> LoginAsync(LoginDto dto)
Task<User?> GetUserByEmailAsync(string email)
```

#### JiraService
**Responsibilities:**
- OAuth authentication with Jira
- Sync projects and sprints from Jira
- Sync issues and map to SprintIQ tasks
- Handle webhook events from Jira
- Bi-directional data synchronization

**Key Methods:**
```csharp
Task<string> GetOAuthRequestTokenAsync(...)
Task<JiraOAuthAccessTokenResponse> GetOAuthAccessTokenAsync(...)
Task<List<JiraProjectDto>> GetJiraProjectsAsync(int teamId)
Task<bool> SyncJiraProjectsAndSprintsAsync(...)
Task<bool> HandleJiraWebhookAsync(JiraWebhookPayload payload)
```

#### AiService
**Responsibilities:**
- Generate AI-powered standup summaries
- Predict blocker risks using ML patterns
- Analyze sprint health and calculate risk scores
- Generate team capacity reports
- Detect sentiment trends

**Key Methods:**
```csharp
Task<string> GenerateStandupSummaryAsync(List<StandupDto> standups)
Task<SprintHealthDto> AnalyzeSprintHealthAsync(int sprintId)
Task<List<JiraIssueDto>> PredictBlockerRiskAsync(...)
Task<TeamCapacityReportDto> GenerateTeamCapacityReportAsync(...)
```

### 5.3 Dependency Injection Setup

```csharp
// Program.cs
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<ISprintService, SprintService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IStandupService, StandupService>();
builder.Services.AddScoped<IBlockerService, BlockerService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<IAiService, AiService>();
builder.Services.AddHttpClient<IJiraService, JiraService>();
builder.Services.AddScoped<IJiraService, JiraService>();
```

### 5.4 CORS Configuration

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

app.UseCors("AllowFrontend");
```

---

## 6. Frontend Documentation

### 6.1 Project Structure

```
frontend/src/
├── app/
│   ├── core/                    # Core Module
│   │   ├── guards/
│   │   │   └── auth.guard.ts
│   │   ├── interceptors/
│   │   │   └── auth.interceptor.ts
│   │   └── services/
│   │       ├── api.service.ts
│   │       ├── auth.service.ts
│   │       ├── jira.service.ts
│   │       └── analytics.service.ts
│   ├── shared/                  # Shared Module
│   │   └── components/
│   │       ├── sidebar/
│   │       └── header/
│   ├── pages/                   # Feature Pages
│   │   ├── dashboard/
│   │   ├── auth/
│   │   │   ├── login/
│   │   │   └── register/
│   │   ├── teams/
│   │   ├── sprints/
│   │   ├── kanban/
│   │   ├── standup/
│   │   ├── blockers/
│   │   ├── leaderboard/
│   │   ├── jira-integration/
│   │   └── ai-insights/
│   ├── app.component.ts         # Root Component
│   ├── app.routes.ts            # Routing Configuration
│   └── app.config.ts            # App Configuration
├── environments/
│   ├── environment.ts           # Dev Environment
│   └── environment.prod.ts      # Prod Environment
├── styles.scss                  # Global Styles
└── index.html                   # Entry HTML
```

### 6.2 Key Services

#### ApiService
**Responsibilities:**
- Base HTTP service
- Centralized API calls
- Error handling
- Request/Response interceptors

```typescript
export class ApiService {
  baseUrl = 'http://localhost:5555/api';
  
  get<T>(endpoint: string): Observable<T>
  post<T>(endpoint: string, data: any): Observable<T>
  put<T>(endpoint: string, data: any): Observable<T>
  delete<T>(endpoint: string): Observable<T>
}
```

#### AuthService
**Responsibilities:**
- User authentication state management
- Login/Register/Logout
- Token storage (localStorage)
- Auth state broadcasting (BehaviorSubject)

```typescript
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSubject.asObservable();
  
  login(credentials: LoginDto): Observable<LoginResponse>
  register(data: RegisterDto): Observable<User>
  logout(): void
  isAuthenticated(): boolean
}
```

#### JiraService
**Responsibilities:**
- Connect to Jira workspace
- Fetch Jira projects
- Trigger sync operations
- Handle OAuth flow

```typescript
export class JiraService {
  connectJira(config: JiraConnectDto): Observable<any>
  getProjects(teamId: number): Observable<JiraProject[]>
  syncJiraData(request: JiraSyncRequestDto): Observable<any>
}
```

#### AnalyticsService
**Responsibilities:**
- Fetch AI-powered insights
- Get sprint health metrics
- Retrieve blocker predictions
- Generate team capacity reports

```typescript
export class AnalyticsService {
  getSprintHealth(sprintId: number): Observable<SprintHealth>
  getBlockerPredictions(teamId: number): Observable<BlockerPrediction[]>
  getTeamCapacity(teamId: number): Observable<TeamCapacityReport>
}
```

### 6.3 Routing Configuration

```typescript
export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [authGuard]
  },
  {
    path: 'teams',
    component: TeamsComponent,
    canActivate: [authGuard]
  },
  // ... other routes
  {
    path: 'jira-integration',
    component: JiraIntegrationComponent,
    canActivate: [authGuard]
  },
  {
    path: 'ai-insights',
    component: AiInsightsComponent,
    canActivate: [authGuard]
  }
];
```

### 6.4 State Management Pattern

SprintIQ uses **RxJS BehaviorSubject** for state management:

```typescript
// Example: Auth State
private currentUserSubject = new BehaviorSubject<User | null>(null);
currentUser$ = this.currentUserSubject.asObservable();

// Components subscribe to state
this.authService.currentUser$.subscribe(user => {
  this.currentUser = user;
});

// Update state
this.currentUserSubject.next(newUser);
```

---

## 7. Database Schema

### 7.1 Entity Relationship Diagram

```
┌─────────────┐       ┌──────────────┐       ┌─────────────┐
│    User     │───────│  TeamMember  │───────│    Team     │
└─────────────┘       └──────────────┘       └─────────────┘
      │                                              │
      │                                              │
      │                                              │
      ├──────────────────────────────────────────────┤
      │                                              │
┌─────────────┐       ┌──────────────┐       ┌─────────────┐
│  UserBadge  │───────│    Badge     │       │   Sprint    │
└─────────────┘       └──────────────┘       └─────────────┘
                                                    │
                                                    │
      ┌─────────────────────────────────────────────┼──────────┐
      │                                             │          │
┌─────────────┐       ┌──────────────┐       ┌─────────────┐ │
│ DailyStandup│       │   Blocker    │       │ SprintTask  │ │
└─────────────┘       └──────────────┘       └─────────────┘ │
                                                              │
                                             ┌────────────────┘
                                             │
                                       ┌─────────────┐
                                       │SprintBurndown
                                       └─────────────┘

┌─────────────┐       ┌──────────────┐       ┌─────────────┐
│JiraSettings │       │ JiraProject  │       │  JiraUser   │
└─────────────┘       └──────────────┘       └─────────────┘

┌─────────────────────────┐
│ JiraWebhookSubscription │
└─────────────────────────┘
```

### 7.2 Core Tables

#### Users
```sql
CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FullName TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    TotalPoints INTEGER DEFAULT 0
);
```

#### Teams
```sql
CREATE TABLE Teams (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Description TEXT,
    CreatedById INTEGER NOT NULL,
    CreatedAt DATETIME NOT NULL,
    FOREIGN KEY (CreatedById) REFERENCES Users(Id)
);
```

#### Sprints
```sql
CREATE TABLE Sprints (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Goal TEXT,
    TeamId INTEGER NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    Status TEXT NOT NULL, -- Planning/Active/Completed
    FOREIGN KEY (TeamId) REFERENCES Teams(Id)
);
```

#### SprintTasks
```sql
CREATE TABLE Tasks (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    Description TEXT,
    SprintId INTEGER NOT NULL,
    AssigneeId INTEGER,
    Status TEXT NOT NULL, -- Todo/InProgress/InReview/Done
    Priority TEXT NOT NULL, -- Low/Medium/High/Critical
    StoryPoints INTEGER,
    CreatedAt DATETIME NOT NULL,
    FOREIGN KEY (SprintId) REFERENCES Sprints(Id),
    FOREIGN KEY (AssigneeId) REFERENCES Users(Id)
);
```

#### DailyStandups
```sql
CREATE TABLE DailyStandups (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    SprintId INTEGER NOT NULL,
    StandupDate DATETIME NOT NULL,
    YesterdayWork TEXT NOT NULL,
    TodayPlan TEXT NOT NULL,
    BlockersText TEXT,
    AiSummary TEXT,
    CreatedAt DATETIME NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (SprintId) REFERENCES Sprints(Id)
);
```

#### Blockers
```sql
CREATE TABLE Blockers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    Description TEXT NOT NULL,
    TaskId INTEGER,
    SprintId INTEGER NOT NULL,
    ReportedById INTEGER NOT NULL,
    AssignedToId INTEGER,
    Severity TEXT NOT NULL, -- Low/Medium/High/Critical
    Status TEXT NOT NULL, -- Open/InProgress/Resolved
    CreatedAt DATETIME NOT NULL,
    ResolvedAt DATETIME,
    FOREIGN KEY (TaskId) REFERENCES Tasks(Id),
    FOREIGN KEY (SprintId) REFERENCES Sprints(Id),
    FOREIGN KEY (ReportedById) REFERENCES Users(Id),
    FOREIGN KEY (AssignedToId) REFERENCES Users(Id)
);
```

#### Badges
```sql
CREATE TABLE Badges (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Description TEXT NOT NULL,
    Icon TEXT NOT NULL,
    Color TEXT NOT NULL,
    Type TEXT NOT NULL, -- TaskCompletion/StandupStreak/BlockerBuster/etc.
    CriteriaValue INTEGER NOT NULL,
    PointValue INTEGER NOT NULL
);
```

#### JiraSettings
```sql
CREATE TABLE JiraSettings (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    TeamId INTEGER NOT NULL UNIQUE,
    JiraBaseUrl TEXT NOT NULL,
    ConsumerKey TEXT NOT NULL,
    PrivateKey TEXT NOT NULL,
    AccessToken TEXT NOT NULL,
    AccessTokenSecret TEXT NOT NULL,
    LastSyncDate DATETIME NOT NULL,
    FOREIGN KEY (TeamId) REFERENCES Teams(Id)
);
```

---

## 8. Authentication & Authorization

### 8.1 Authentication Flow

```
┌─────────┐                          ┌─────────┐
│ Client  │                          │  Server │
└────┬────┘                          └────┬────┘
     │                                    │
     │  POST /api/auth/login              │
     │  { email, password }               │
     ├───────────────────────────────────>│
     │                                    │
     │                        Validate    │
     │                        Credentials │
     │                                    │
     │                        Generate    │
     │                        JWT Token   │
     │                                    │
     │  200 OK                            │
     │  { token, user }                   │
     │<───────────────────────────────────┤
     │                                    │
     │  Store token in localStorage       │
     │                                    │
     │  GET /api/teams                    │
     │  Header: Authorization: Bearer ... │
     ├───────────────────────────────────>│
     │                                    │
     │                        Validate    │
     │                        JWT Token   │
     │                                    │
     │  200 OK                            │
     │  [ teams... ]                      │
     │<───────────────────────────────────┤
     │                                    │
```

### 8.2 JWT Token Structure

**Payload:**
```json
{
  "sub": "user@example.com",
  "userId": "123",
  "name": "John Doe",
  "iat": 1234567890,
  "exp": 1234654290
}
```

**Token Generation (Backend):**
```csharp
private string GenerateJwtToken(User user)
{
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim("userId", user.Id.ToString()),
        new Claim("name", user.FullName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: "SprintIQ",
        audience: "SprintIQ",
        claims: claims,
        expires: DateTime.UtcNow.AddDays(7),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

**Token Validation (Frontend):**
```typescript
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    return true;
  }

  router.navigate(['/login']);
  return false;
};
```

### 8.3 Password Security

- **Hashing Algorithm**: BCrypt (Cost Factor: 12)
- **Salt**: Automatically generated per password
- **No Plain Text Storage**: Only hashed passwords in database

```csharp
// Hash password during registration
user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

// Verify password during login
if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
{
    return null; // Invalid credentials
}
```

---

## 9. API Documentation

### 9.1 Authentication Endpoints

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "fullName": "John Doe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}

Response: 201 Created
{
  "id": 1,
  "fullName": "John Doe",
  "email": "john@example.com",
  "createdAt": "2024-01-30T10:00:00Z",
  "totalPoints": 0
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecurePass123!"
}

Response: 200 OK
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "fullName": "John Doe",
    "email": "john@example.com"
  }
}
```

### 9.2 Jira Integration Endpoints

#### Connect Jira Workspace
```http
POST /api/jira/connect
Authorization: Bearer {token}
Content-Type: application/json

{
  "jiraBaseUrl": "https://yourcompany.atlassian.net",
  "consumerKey": "your-consumer-key",
  "privateKey": "-----BEGIN RSA PRIVATE KEY-----\n..."
}

Response: 200 OK
{
  "authUrl": "https://yourcompany.atlassian.net/plugins/servlet/oauth/authorize?oauth_token=..."
}
```

#### Sync Jira Data
```http
POST /api/jira/sync
Authorization: Bearer {token}
Content-Type: application/json

{
  "teamId": 1,
  "projectKeys": ["PROJ", "DEV"]
}

Response: 200 OK
{
  "message": "Jira data sync initiated"
}
```

### 9.3 Analytics Endpoints

#### Get Sprint Health
```http
GET /api/analytics/sprint-health/5
Authorization: Bearer {token}

Response: 200 OK
{
  "sprintId": 5,
  "healthScore": 78.5,
  "riskLevel": "Medium",
  "completionProbability": 0.85,
  "atRiskTasks": 3,
  "warnings": ["Sprint is 60% through time but only 45% complete"],
  "recommendations": ["Consider extending sprint or reducing scope"]
}
```

#### Get Blocker Predictions
```http
GET /api/analytics/blocker-prediction/1
Authorization: Bearer {token}

Response: 200 OK
[
  {
    "issueKey": "PROJ-123",
    "title": "Implement user authentication",
    "riskScore": 0.87,
    "reasons": ["Stuck in progress for 5 days", "High comment frequency"],
    "suggestedActions": ["Escalate to team lead", "Break into smaller tasks"]
  }
]
```

### 9.4 Complete API Reference

See Swagger UI at: `http://localhost:5555/swagger`

---

## 10. Jira Integration

### 10.1 OAuth Flow

```
1. User clicks "Connect Jira" in SprintIQ
2. SprintIQ sends request to Jira for request token
3. User is redirected to Jira authorization page
4. User approves access in Jira
5. Jira redirects back with OAuth verifier
6. SprintIQ exchanges verifier for access token
7. Access token stored securely in database
8. SprintIQ can now make authenticated Jira API calls
```

### 10.2 Data Synchronization

**Pull from Jira:**
- Projects
- Boards
- Sprints
- Issues (User Stories, Tasks, Bugs)
- Users
- Comments
- Status Changes

**Push to Jira:**
- Task status updates
- Assignee changes
- Comments from SprintIQ
- Story point updates

**Sync Frequency:**
- Manual: On-demand via UI
- Automatic: Every 15 minutes (configurable)
- Real-time: Via webhooks for instant updates

### 10.3 Webhook Configuration

SprintIQ can receive real-time updates from Jira via webhooks:

**Supported Events:**
- `jira:issue_created`
- `jira:issue_updated`
- `jira:issue_deleted`
- `sprint_created`
- `sprint_started`
- `sprint_closed`

**Webhook Endpoint:**
```http
POST /api/jira/webhook
Content-Type: application/json

{
  "webhookEvent": "jira:issue_updated",
  "issue": { ... },
  "user": { ... },
  "changelog": { ... }
}
```

### 10.4 Field Mapping

| Jira Field | SprintIQ Field |
|------------|----------------|
| Summary | Task Title |
| Description | Task Description |
| Status | Task Status |
| Assignee | Assignee |
| Story Points | Story Points |
| Priority | Priority |
| Issue Type | Task Type |
| Sprint | Sprint |
| Created | Created At |
| Updated | Updated At |

---

## 11. AI Analytics Engine

### 11.1 Sprint Risk Analysis

**Algorithm:**
1. Calculate sprint progress ratio: `completed_points / total_points`
2. Calculate time progress ratio: `elapsed_days / total_days`
3. Calculate velocity trend from past 3 sprints
4. Identify blocked/stuck tasks (no updates in 3+ days)
5. Calculate health score: weighted average of above factors

**Risk Levels:**
- **Low (80-100)**: On track, healthy progress
- **Medium (60-79)**: Minor concerns, needs attention
- **High (40-59)**: Significant risks, intervention needed
- **Critical (0-39)**: Sprint in danger, immediate action required

### 11.2 Blocker Prediction

**ML Features Analyzed:**
- Time in current status (stuck indicator)
- Comment frequency (discussion intensity)
- Number of status changes (churn indicator)
- Assignee change frequency
- Story point complexity
- Historical blocker patterns
- Team member workload

**Prediction Output:**
- Risk score (0.0 - 1.0)
- Confidence level
- Contributing factors
- Recommended actions

### 11.3 Team Health Metrics

**Metrics Calculated:**
- **Workload Distribution**: Standard deviation of assigned points
- **Velocity Trend**: 3-sprint moving average
- **Burnout Risk**: Based on overtime patterns, weekend work
- **Collaboration Score**: Based on comments, pair programming
- **Engagement Score**: Based on standup participation, activity

### 11.4 AI-Generated Summaries

**Daily Standup Summary:**
```
Team Progress: 15 tasks completed, 8 in progress
Key Highlights:
  - Authentication module completed (John, Sarah)
  - API integration 80% done (Mike)
Blockers Detected:
  - Database migration issue (Sarah - needs DBA support)
  - Dependency on external API (Mike - vendor delayed)
Action Items:
  - Schedule DBA review for database migration
  - Escalate vendor delay to product owner
Overall Sentiment: Positive with minor concerns
```

---

## 12. Setup & Installation

### 12.1 Prerequisites

**Required:**
- Node.js v20.19.0 or higher
- .NET 9 SDK
- Git
- Visual Studio Code (recommended)

**Optional:**
- Visual Studio 2022
- SQL Server Management Studio (for advanced DB management)
- Postman (for API testing)

### 12.2 Backend Setup

```bash
# Navigate to backend directory
cd backend/SprintIQ.API

# Restore NuGet packages
dotnet restore

# Apply database migrations
dotnet ef database update

# Run the backend
dotnet run
```

Backend will start at: `http://localhost:5555`
Swagger UI: `http://localhost:5555/swagger`

### 12.3 Frontend Setup

```bash
# Navigate to frontend directory
cd frontend

# Install npm packages
npm install

# Start development server
npm start
```

Frontend will start at: `http://localhost:3000`

### 12.4 Environment Configuration

**Backend: `appsettings.json`**
```json
{
  "Jwt": {
    "Secret": "YourSecretKey_AtLeast32Chars_ForProduction",
    "Issuer": "SprintIQ",
    "Audience": "SprintIQ"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=sprintiq.db"
  }
}
```

**Frontend: `environment.ts`**
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5555/api'
};
```

### 12.5 Database Initialization

Database is automatically created on first run with seeded data:
- 10 default badges
- Sample admin user (optional)

---

## 13. Development Workflow

### 13.1 Git Workflow

```bash
# Create feature branch
git checkout -b feature/jira-integration

# Make changes and commit
git add .
git commit -m "feat: Add Jira OAuth integration"

# Push to remote
git push origin feature/jira-integration

# Create Pull Request on GitHub
```

### 13.2 Commit Message Convention

```
feat: Add new feature
fix: Fix bug
docs: Update documentation
refactor: Code refactoring
test: Add tests
chore: Maintenance tasks
```

### 13.3 Code Quality

**Backend:**
- Follow C# coding conventions
- Use async/await for all I/O operations
- Implement proper error handling
- Add XML documentation comments

**Frontend:**
- Follow Angular style guide
- Use TypeScript strict mode
- Implement reactive patterns with RxJS
- Add JSDoc comments for complex logic

---

## 14. Deployment Guide

### 14.1 Production Checklist

- [ ] Update JWT secret to strong random string
- [ ] Set `environment.production = true`
- [ ] Configure CORS for production domain
- [ ] Enable HTTPS
- [ ] Set up error logging (e.g., Sentry)
- [ ] Configure database backups
- [ ] Set up monitoring (e.g., Application Insights)
- [ ] Create deployment scripts
- [ ] Document environment variables

### 14.2 Deployment Options

**Option 1: Azure App Service**
```bash
# Backend
az webapp create --name sprintiq-api --plan myplan --resource-group mygroup
dotnet publish -c Release
az webapp deployment source config-zip --src ./publish.zip

# Frontend
ng build --configuration production
az storage blob upload-batch -d '$web' -s ./dist/frontend
```

**Option 2: Docker**
```dockerfile
# Backend Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY ./publish .
ENTRYPOINT ["dotnet", "SprintIQ.API.dll"]

# Frontend Dockerfile
FROM nginx:alpine
COPY ./dist/frontend /usr/share/nginx/html
```

**Option 3: Traditional Hosting (IIS)**
- Publish backend to IIS
- Host frontend on static file server

---

## 15. Best Practices

### 15.1 Security
- Never commit secrets to Git
- Use environment variables for sensitive data
- Implement rate limiting on API endpoints
- Validate and sanitize all user inputs
- Keep dependencies updated

### 15.2 Performance
- Use lazy loading for Angular modules
- Implement caching strategies
- Optimize database queries (use indexes)
- Compress HTTP responses
- Use CDN for static assets

### 15.3 Code Organization
- Keep components small and focused
- Use dependency injection
- Follow SOLID principles
- Write unit tests for critical logic
- Document complex algorithms

---

## 16. Future Roadmap

### Phase 1 (Completed ✅)
- Core sprint management
- Kanban board
- Daily standup logger
- Leaderboard & gamification
- Jira integration
- AI analytics

### Phase 2 (Planned 🔮)
- Real-time collaboration (SignalR)
- Advanced AI features:
  - Sprint velocity prediction
  - Optimal task assignment
  - Sentiment analysis
- Mobile app (React Native)
- Slack/Teams integration
- Advanced reporting
- Custom dashboards

### Phase 3 (Future 🚀)
- Multi-tenancy support
- Enterprise SSO (SAML, OAuth)
- Advanced permissions system
- Audit logging
- API webhooks for 3rd party integrations
- Marketplace for plugins

---

## 17. Support & Contact

**Documentation**: This file
**API Documentation**: http://localhost:5555/swagger
**GitHub Repository**: https://github.com/deepmodak79/SprintIQ
**Issues**: https://github.com/deepmodak79/SprintIQ/issues

---

## 18. License

MIT License - See LICENSE file for details

---

## 19. Acknowledgments

- Angular Team for the amazing framework
- Microsoft for .NET and Entity Framework Core
- Atlassian for Jira API documentation
- OpenAI for AI capabilities inspiration
- Chart.js for visualization library

---

**Last Updated**: January 30, 2026
**Version**: 2.0.0
**Author**: Deep Modak

---

**End of Documentation**
