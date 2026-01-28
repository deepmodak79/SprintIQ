namespace SprintIQ.API.Models;

public class JiraWorkspace
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public Team? Team { get; set; }
    public string JiraUrl { get; set; } = string.Empty; // e.g., https://yourcompany.atlassian.net
    public string JiraEmail { get; set; } = string.Empty;
    public string JiraApiToken { get; set; } = string.Empty; // Encrypted
    public string ProjectKey { get; set; } = string.Empty; // e.g., PROJ
    public DateTime LastSyncDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class JiraSync
{
    public int Id { get; set; }
    public int JiraWorkspaceId { get; set; }
    public JiraWorkspace? JiraWorkspace { get; set; }
    public string SyncType { get; set; } = string.Empty; // Sprint, Task, User, Comment
    public DateTime SyncStartedAt { get; set; }
    public DateTime? SyncCompletedAt { get; set; }
    public string Status { get; set; } = "In Progress"; // In Progress, Completed, Failed
    public int ItemsSynced { get; set; }
    public string? ErrorMessage { get; set; }
}

// AI Analysis Models
public class SprintRiskAnalysis
{
    public int Id { get; set; }
    public int SprintId { get; set; }
    public Sprint? Sprint { get; set; }
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
    public string RiskLevel { get; set; } = "Low"; // Low, Medium, High, Critical
    public decimal CompletionProbability { get; set; } // 0-100%
    public int PredictedUnfinishedTasks { get; set; }
    public string RiskFactors { get; set; } = string.Empty; // JSON array of risk factors
    public string Recommendations { get; set; } = string.Empty; // AI recommendations
}

public class BlockerPrediction
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public SprintTask? Task { get; set; }
    public DateTime PredictedAt { get; set; } = DateTime.UtcNow;
    public decimal BlockerProbability { get; set; } // 0-100%
    public string PredictionReason { get; set; } = string.Empty;
    public bool IsActualBlocker { get; set; } = false; // For ML feedback
    public DateTime? ConfirmedAt { get; set; }
}

public class TeamHealthMetrics
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public Team? Team { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    public decimal OverallHealthScore { get; set; } // 0-100
    public decimal MoraleScore { get; set; } // Sentiment analysis from comments
    public decimal WorkloadBalance { get; set; } // How evenly distributed work is
    public decimal CollaborationScore { get; set; } // Team interaction level
    public decimal BurnoutRisk { get; set; } // 0-100, higher = more risk
    public int ActiveBlockers { get; set; }
    public decimal VelocityTrend { get; set; } // % change from previous sprint
    public string Insights { get; set; } = string.Empty; // JSON array of insights
}

public class StandupInsight
{
    public int Id { get; set; }
    public int StandupId { get; set; }
    public DailyStandup? Standup { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public string InsightType { get; set; } = string.Empty; // BlockerAlert, VelocityWarning, PraiseWin, etc.
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = "Low"; // Low, Medium, High
    public bool IsAcknowledged { get; set; } = false;
}
