# SprintIQ 🚀

**Sprint Visual Management & Daily Management Dashboard with Leaderboard**

A modern, full-stack application for agile teams to manage sprints, track daily standups, and boost productivity through gamification.

![SprintIQ](https://img.shields.io/badge/SprintIQ-v1.0-6366f1?style=for-the-badge)
![Angular](https://img.shields.io/badge/Angular-18-dd0031?style=for-the-badge&logo=angular)
![.NET](https://img.shields.io/badge/.NET-9.0-512bd4?style=for-the-badge&logo=dotnet)

## ✨ Features

### 📊 Sprint Dashboard
- **Kanban Board** - Drag-and-drop task management across To Do, In Progress, In Review, Done
- **Burndown Chart** - Real-time visualization of sprint progress
- **Sprint Health Indicator** - AI-powered risk detection and recommendations

### 📋 Daily Standup Management
- **Quick Standup Logger** - Submit daily updates in < 30 seconds
- **Team Overview** - View all team standups in one place
- **Mood & Confidence Tracking** - Track team health metrics
- **AI-Generated Summaries** - Automatic daily standup summaries for managers

### 🏆 Gamification & Leaderboard
- **Points System** - Earn points for completing tasks, submitting standups, resolving blockers
- **Achievement Badges** - Unlock badges like "Task Master", "Streak Legend", "Blocker Buster"
- **Streak Tracking** - Maintain daily standup streaks for bonus points
- **Team Rankings** - Compete with teammates for the top spot

### 🚫 Blocker Management
- **Blocker Tracking** - Report, assign, and resolve blockers
- **AI Suggestions** - Get smart suggestions for common blocker patterns
- **Severity Levels** - Prioritize blockers by severity

## 🛠️ Tech Stack

| Layer | Technology |
|-------|------------|
| **Frontend** | Angular 18, TypeScript, SCSS |
| **Backend** | ASP.NET Core 9 Web API (C#) |
| **Database** | SQLite (EF Core) |
| **Charts** | Chart.js + ng2-charts |
| **Drag & Drop** | Angular CDK |
| **Auth** | JWT Authentication |

## 🚀 Quick Start

### Prerequisites
- Node.js 18+ & npm
- .NET 9 SDK
- Git

### Backend Setup
```bash
# Navigate to backend
cd backend/SprintIQ.API

# Run the API
dotnet run

# API will be available at http://localhost:5000
# Swagger docs at http://localhost:5000/swagger
```

### Frontend Setup
```bash
# Navigate to frontend
cd frontend

# Install dependencies
npm install

# Start dev server
npm start

# App will be available at http://localhost:4200
```

## 📁 Project Structure

```
SprintIQ/
├── backend/
│   └── SprintIQ.API/
│       ├── Controllers/     # API endpoints
│       ├── Services/        # Business logic
│       ├── Models/          # Entity models
│       ├── DTOs/            # Data transfer objects
│       └── Data/            # EF Core DbContext
├── frontend/
│   └── src/
│       ├── app/
│       │   ├── core/        # Services, models
│       │   ├── pages/       # Page components
│       │   └── shared/      # Shared components
│       └── styles.scss      # Global styles
└── README.md
```

## 📱 Screenshots

### Dashboard
- Quick stats (Tasks, Points, Streak)
- Team overview
- Quick actions

### Sprint Board
- Drag-and-drop Kanban
- Task cards with priority indicators
- Real-time progress tracking

### Leaderboard
- Top 3 podium display
- Full rankings table
- Personal stats

## 🎯 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user
- `GET /api/auth/me` - Get current user

### Teams
- `GET /api/teams` - Get all teams
- `POST /api/teams` - Create team
- `POST /api/teams/{id}/members` - Add team member

### Sprints
- `GET /api/sprints/{id}` - Get sprint details
- `POST /api/sprints` - Create sprint
- `GET /api/sprints/{id}/burndown` - Get burndown data

### Tasks
- `GET /api/tasks/sprint/{id}/kanban` - Get Kanban board
- `POST /api/tasks` - Create task
- `PUT /api/tasks/{id}/move` - Move task between columns

### Standups
- `POST /api/standups` - Submit daily standup
- `GET /api/standups/sprint/{id}/summary` - Get team summary
- `POST /api/standups/generate-ai-summary` - Generate AI summary

### Leaderboard
- `GET /api/leaderboard` - Get global leaderboard
- `GET /api/leaderboard/my-stats` - Get personal stats

## 🎨 Design System

The app uses a dark futuristic theme with:
- **Primary Color**: Indigo (#6366f1)
- **Background**: Dark slate (#0a0a0f)
- **Font**: Space Grotesk

## 🏅 Badge Types

| Badge | Criteria | Points |
|-------|----------|--------|
| 🎯 First Steps | Complete first task | 25 |
| ⚡ Task Master | Complete 10 tasks | 100 |
| 🔥 Consistency King | 7-day standup streak | 150 |
| 👑 Streak Legend | 30-day standup streak | 500 |
| 🔓 Blocker Buster | Resolve 5 blockers | 100 |
| 🏆 Sprint Champion | 100% sprint completion | 300 |

## 📄 License

MIT License - feel free to use this project for learning or production!

## 🤝 Contributing

Contributions welcome! Please feel free to submit a Pull Request.

---

Built with ❤️ by Deep Modak
