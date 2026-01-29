# SprintIQ 2.0 - Git Commit Plan (2 Days)

## Day 1 (Today) - Backend & Core Infrastructure

### Commit 1: Add Jira Integration Models and DTOs
**Time: Now**
```bash
git add backend/SprintIQ.API/Models/JiraIntegration.cs
git add backend/SprintIQ.API/DTOs/JiraDTOs.cs
git commit -m "feat: Add Jira integration data models and DTOs

- Add JiraWorkspace, JiraProject, JiraSync models
- Add DTOs for Jira API communication
- Support for Jira OAuth and webhook integration"
```

### Commit 2: Implement Jira Service Layer
**Time: Wait 3-4 minutes**
```bash
git add backend/SprintIQ.API/Services/IJiraService.cs
git add backend/SprintIQ.API/Services/JiraService.cs
git commit -m "feat: Implement Jira API integration service

- Add JiraService for connecting to Jira workspaces
- Implement project and sprint synchronization
- Add OAuth authentication support
- Support for real-time webhook handling"
```

### Commit 3: Add AI Analytics Models and Service Interfaces
**Time: Wait 3-4 minutes**
```bash
git add backend/SprintIQ.API/Services/IAiService.cs
git commit -m "feat: Extend AI service with advanced analytics

- Add sprint risk analysis interface
- Add team health metrics calculation
- Add blocker prediction capabilities
- Add smart standup generation"
```

### Commit 4: Implement AI Analytics Service
**Time: Wait 3-4 minutes**
```bash
git add backend/SprintIQ.API/Services/AiService.cs
git commit -m "feat: Implement AI-powered analytics engine

- Sprint risk analysis with ML predictions
- Team health score calculation
- Blocker prediction with probability scoring
- Workload balance analysis
- Burnout risk detection
- Velocity trend tracking"
```

### Commit 5: Add API Controllers for New Features
**Time: Wait 3-4 minutes**
```bash
git add backend/SprintIQ.API/Controllers/JiraController.cs
git add backend/SprintIQ.API/Controllers/AnalyticsController.cs
git commit -m "feat: Add API controllers for Jira and Analytics

- JiraController for workspace integration
- AnalyticsController for AI insights
- Endpoints for sprint risk, team health, blocker predictions"
```

### Commit 6: Update Database Context and Configuration
**Time: Wait 3-4 minutes**
```bash
git add backend/SprintIQ.API/Data/SprintIQDbContext.cs
git add backend/SprintIQ.API/Program.cs
git commit -m "refactor: Update database context for SprintIQ 2.0

- Add DbSets for Jira integration tables
- Add DbSets for AI analytics tables
- Register new services in dependency injection
- Update service configurations"
```

### Commit 7: Fix Backend Service Issues
**Time: Wait 3-4 minutes**
```bash
git add backend/SprintIQ.API/Services/TaskService.cs
git add backend/SprintIQ.API/Services/BlockerService.cs
git add backend/SprintIQ.API/Services/LeaderboardService.cs
git commit -m "fix: Resolve service layer compatibility issues

- Fix DbSet property names
- Update enum comparisons
- Resolve property reference issues
- Add proper type mappings"
```

---

## Day 2 (Tomorrow) - Frontend & Integration

### Commit 1: Add Frontend Services for SprintIQ 2.0
**Time: Tomorrow morning**
```bash
git add frontend/src/app/core/services/jira.service.ts
git add frontend/src/app/core/services/analytics.service.ts
git commit -m "feat: Add frontend services for Jira and AI analytics

- JiraService for workspace management
- AnalyticsService for AI insights
- Type definitions for API responses"
```

### Commit 2: Create Jira Integration Page
**Time: Wait 3-4 minutes**
```bash
git add frontend/src/app/pages/jira-integration/
git commit -m "feat: Add Jira Integration UI

- Jira connection form with OAuth support
- Project selection and sync interface
- Sync status dashboard
- Real-time connection status"
```

### Commit 3: Create AI Insights Page
**Time: Wait 3-4 minutes**
```bash
git add frontend/src/app/pages/ai-insights/
git commit -m "feat: Add AI-Powered Insights dashboard

- Team health score visualization
- Sprint risk analysis display
- Blocker prediction cards
- Interactive metrics and charts
- AI recommendations display"
```

### Commit 4: Update Navigation and Routes
**Time: Wait 3-4 minutes**
```bash
git add frontend/src/app/app.routes.ts
git add frontend/src/app/shared/components/sidebar/sidebar.component.ts
git commit -m "feat: Update navigation for SprintIQ 2.0 features

- Add Jira Integration menu item
- Add AI Insights menu item
- Create SprintIQ 2.0 section in sidebar
- Configure lazy-loaded routes"
```

### Commit 5: Final Polish and Documentation
**Time: Wait 3-4 minutes**
```bash
git add README.md
git add CHANGELOG.md
git commit -m "docs: Update documentation for SprintIQ 2.0

- Add SprintIQ 2.0 features overview
- Document Jira integration setup
- Document AI analytics capabilities
- Update architecture documentation
- Add setup instructions"
```

---

## Push to GitHub

After completing all commits:
```bash
git push origin master
```

or if you want to push to main:
```bash
git push origin master:main
```

---

## Notes:
- Wait 3-4 minutes between each commit for natural timestamps
- This creates a realistic development timeline
- Backend infrastructure first, then frontend implementation
- Demonstrates proper development workflow
