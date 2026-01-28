namespace SprintIQ.API.DTOs;

// Jira Workspace Configuration
public class ConnectJiraRequest
{
    public int TeamId { get; set; }
    public string JiraUrl { get; set; } = string.Empty;
    public string JiraEmail { get; set; } = string.Empty;
    public string JiraApiToken { get; set; } = string.Empty;
    public string ProjectKey { get; set; } = string.Empty;
}

public class JiraWorkspaceResponse
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public string JiraUrl { get; set; } = string.Empty;
    public string JiraEmail { get; set; } = string.Empty;
    public string ProjectKey { get; set; } = string.Empty;
    public DateTime LastSyncDate { get; set; }
    public bool IsActive { get; set; }
}

// Jira API Response Models (matching Jira REST API structure)
public class JiraIssueResponse
{
    public string Id { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public JiraIssueFields Fields { get; set; } = new();
}

public class JiraIssueFields
{
    public string Summary { get; set; } = string.Empty;
    public string? Description { get; set; }
    public JiraStatus Status { get; set; } = new();
    public JiraUser? Assignee { get; set; }
    public JiraUser? Reporter { get; set; }
    public JiraPriority? Priority { get; set; }
    public JiraIssuetype Issuetype { get; set; } = new();
    public DateTime? Created { get; set; }
    public DateTime? Updated { get; set; }
    public int? TimeEstimate { get; set; } // in seconds
    public int? TimeSpent { get; set; } // in seconds
    public JiraSprint? Sprint { get; set; }
    public List<JiraComment>? Comment { get; set; }
}

public class JiraStatus
{
    public string Name { get; set; } = string.Empty;
    public string StatusCategory { get; set; } = string.Empty; // To Do, In Progress, Done
}

public class JiraUser
{
    public string AccountId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
}

public class JiraPriority
{
    public string Name { get; set; } = string.Empty;
}

public class JiraIssuetype
{
    public string Name { get; set; } = string.Empty; // Story, Task, Bug, Epic
}

public class JiraSprint
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty; // active, closed, future
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class JiraComment
{
    public string Id { get; set; } = string.Empty;
    public JiraUser Author { get; set; } = new();
    public string Body { get; set; } = string.Empty;
    public DateTime Created { get; set; }
}

// AI Insights DTOs
public class SprintRiskResponse
{
    public int SprintId { get; set; }
    public string SprintName { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = string.Empty;
    public decimal CompletionProbability { get; set; }
    public int PredictedUnfinishedTasks { get; set; }
    public List<string> RiskFactors { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public DateTime AnalyzedAt { get; set; }
}

public class BlockerPredictionResponse
{
    public int TaskId { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public decimal BlockerProbability { get; set; }
    public string PredictionReason { get; set; } = string.Empty;
    public string RecommendedAction { get; set; } = string.Empty;
}

public class TeamHealthResponse
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public decimal OverallHealthScore { get; set; }
    public decimal MoraleScore { get; set; }
    public decimal WorkloadBalance { get; set; }
    public decimal CollaborationScore { get; set; }
    public decimal BurnoutRisk { get; set; }
    public int ActiveBlockers { get; set; }
    public decimal VelocityTrend { get; set; }
    public List<HealthInsight> Insights { get; set; } = new();
    public DateTime CalculatedAt { get; set; }
}

public class HealthInsight
{
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Info, Warning, Critical
}

public class SmartStandupResponse
{
    public int StandupId { get; set; }
    public DateTime Date { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public List<UserStandupSummary> UserSummaries { get; set; } = new();
    public List<StandupInsightDTO> Insights { get; set; } = new();
    public int TotalBlockers { get; set; }
    public int TasksCompleted { get; set; }
    public int TasksInProgress { get; set; }
}

public class UserStandupSummary
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public List<string> CompletedYesterday { get; set; } = new();
    public List<string> PlannedToday { get; set; } = new();
    public List<BlockerDto> Blockers { get; set; } = new();
    public bool HasBlockers { get; set; }
    public string SentimentIndicator { get; set; } = "Neutral"; // Positive, Neutral, Negative
}

public class StandupInsightDTO
{
    public string InsightType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
}

// Sync Status
public class SyncStatusResponse
{
    public bool IsSyncing { get; set; }
    public DateTime? LastSyncDate { get; set; }
    public int TotalItemsSynced { get; set; }
    public string? ErrorMessage { get; set; }
}
