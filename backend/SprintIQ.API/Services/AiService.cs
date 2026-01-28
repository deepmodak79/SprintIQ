using Microsoft.EntityFrameworkCore;
using SprintIQ.API.Data;
using SprintIQ.API.DTOs;
using SprintIQ.API.Models;
using System.Text.Json;
using TaskStatus = SprintIQ.API.Models.TaskStatus;

namespace SprintIQ.API.Services;

/// <summary>
/// AI Service - Provides intelligent analytics and predictions for Sprint Management
/// SprintIQ 2.0: Enhanced with blocker prediction, risk analysis, and team health metrics
/// </summary>
public class AiService : IAiService
{
    private readonly SprintIQDbContext _context;
    private readonly ILogger<AiService> _logger;

    public AiService(SprintIQDbContext context, ILogger<AiService> logger)
    {
        _context = context;
        _logger = logger;
    }
    public Task<string> GenerateStandupSummaryAsync(List<StandupDto> standups)
    {
        if (!standups.Any())
        {
            return Task.FromResult("No standups submitted yet for this period.");
        }

        var totalSubmissions = standups.Count;
        var avgMood = standups.Where(s => s.Mood.HasValue).DefaultIfEmpty().Average(s => s?.Mood ?? 3);
        var avgConfidence = standups.Where(s => s.Confidence.HasValue).DefaultIfEmpty().Average(s => s?.Confidence ?? 3);

        var blockerCount = standups.Count(s => !string.IsNullOrEmpty(s.Blockers));
        var blockerList = standups
            .Where(s => !string.IsNullOrEmpty(s.Blockers))
            .Select(s => $"• {s.UserName}: {s.Blockers}")
            .ToList();

        var todayPlans = standups
            .Where(s => !string.IsNullOrEmpty(s.Today))
            .Select(s => $"• {s.UserName}: {s.Today}")
            .Take(5)
            .ToList();

        var moodEmoji = avgMood switch
        {
            >= 4.5 => "🌟",
            >= 3.5 => "😊",
            >= 2.5 => "😐",
            >= 1.5 => "😟",
            _ => "😰"
        };

        var summary = $"""
            📊 **Daily Standup Summary**
            
            **Participation:** {totalSubmissions} team member(s) submitted updates
            **Team Mood:** {moodEmoji} {avgMood:F1}/5
            **Sprint Confidence:** {avgConfidence:F1}/5
            
            """;

        if (blockerCount > 0)
        {
            summary += $"""
                
                ⚠️ **Blockers Reported ({blockerCount}):**
                {string.Join("\n", blockerList)}
                
                """;
        }
        else
        {
            summary += "\n✅ **No blockers reported** - Team is moving smoothly!\n";
        }

        if (todayPlans.Any())
        {
            summary += $"""
                
                📋 **Today's Focus:**
                {string.Join("\n", todayPlans)}
                """;
        }

        // Add recommendations based on data
        var recommendations = new List<string>();

        if (avgMood < 3)
        {
            recommendations.Add("Team morale is low - consider a quick team sync or support session");
        }

        if (avgConfidence < 3)
        {
            recommendations.Add("Sprint confidence is low - review scope and priorities");
        }

        if (blockerCount > 2)
        {
            recommendations.Add("Multiple blockers reported - prioritize blocker resolution today");
        }

        if (recommendations.Any())
        {
            summary += $"""
                
                💡 **AI Recommendations:**
                {string.Join("\n", recommendations.Select(r => $"• {r}"))}
                """;
        }

        return Task.FromResult(summary);
    }

    public Task<string> GenerateBlockerSuggestionAsync(string blockerDescription)
    {
        var lowerDesc = blockerDescription.ToLower();

        var suggestions = new List<string>();

        // Common blocker patterns and suggestions
        if (lowerDesc.Contains("api") || lowerDesc.Contains("endpoint") || lowerDesc.Contains("backend"))
        {
            suggestions.Add("Check API documentation for correct endpoint usage");
            suggestions.Add("Verify authentication tokens are valid");
            suggestions.Add("Review API response codes and error messages");
        }

        if (lowerDesc.Contains("deploy") || lowerDesc.Contains("build") || lowerDesc.Contains("pipeline"))
        {
            suggestions.Add("Check CI/CD pipeline logs for detailed error messages");
            suggestions.Add("Verify all environment variables are correctly set");
            suggestions.Add("Ensure all dependencies are properly versioned");
        }

        if (lowerDesc.Contains("test") || lowerDesc.Contains("failing"))
        {
            suggestions.Add("Review test output for specific failure reasons");
            suggestions.Add("Check if recent code changes affected test dependencies");
            suggestions.Add("Consider running tests in isolation to identify issues");
        }

        if (lowerDesc.Contains("permission") || lowerDesc.Contains("access") || lowerDesc.Contains("auth"))
        {
            suggestions.Add("Verify user roles and permissions in the system");
            suggestions.Add("Check if required access has been granted");
            suggestions.Add("Contact team lead or admin for access requests");
        }

        if (lowerDesc.Contains("wait") || lowerDesc.Contains("depend") || lowerDesc.Contains("blocked by"))
        {
            suggestions.Add("Identify the dependency owner and communicate urgency");
            suggestions.Add("Check if there's alternative work that can be done meanwhile");
            suggestions.Add("Escalate if dependency is critical path");
        }

        if (lowerDesc.Contains("unclear") || lowerDesc.Contains("requirement") || lowerDesc.Contains("spec"))
        {
            suggestions.Add("Schedule a quick clarification call with stakeholders");
            suggestions.Add("Document assumptions and get them validated");
            suggestions.Add("Break down the task into smaller, clearer items");
        }

        // Default suggestions if no pattern matched
        if (!suggestions.Any())
        {
            suggestions.Add("Break down the blocker into smaller, actionable items");
            suggestions.Add("Discuss with team members who might have faced similar issues");
            suggestions.Add("Document the issue thoroughly for faster resolution");
            suggestions.Add("Consider pairing with a teammate to troubleshoot");
        }

        var suggestion = $"""
            🤖 **AI Suggestions for this blocker:**
            
            {string.Join("\n", suggestions.Select(s => $"• {s}"))}
            
            💬 *Tip: Share this blocker in your daily standup for team visibility.*
            """;

        return Task.FromResult(suggestion);
    }

    public Task<SprintHealthDto> AnalyzeSprintHealthAsync(int sprintId)
    {
        // This would typically fetch sprint data and analyze it
        // For now, returning a placeholder
        return Task.FromResult(new SprintHealthDto
        {
            Status = "Healthy",
            Color = "#4CAF50",
            Warnings = new List<string>(),
            Recommendations = new List<string> { "Continue maintaining current pace", "Keep blocker count low" }
        });
    }

    // ==================== SprintIQ 2.0: Advanced AI Features ====================

    public async Task<SprintRiskResponse> AnalyzeSprintRiskAsync(int sprintId)
    {
        var sprint = await _context.Sprints
            .Include(s => s.Tasks)
            .ThenInclude(t => t.Assignee)
            .FirstOrDefaultAsync(s => s.Id == sprintId);

        if (sprint == null)
        {
            throw new Exception("Sprint not found");
        }

        var totalTasks = sprint.Tasks!.Count;
        var completedTasks = sprint.Tasks!.Count(t => t.Status == TaskStatus.Done);
        var inProgressTasks = sprint.Tasks!.Count(t => t.Status == TaskStatus.InProgress);
        var todoTasks = sprint.Tasks!.Count(t => t.Status == TaskStatus.Todo);
        
        var daysRemaining = (sprint.EndDate - DateTime.UtcNow).TotalDays;
        var daysTotal = (sprint.EndDate - sprint.StartDate).TotalDays;
        var daysElapsed = daysTotal - daysRemaining;
        var progressPercentage = daysTotal > 0 ? (daysElapsed / daysTotal) * 100 : 0;
        
        var completionRate = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0;
        
        // Risk calculation logic
        var riskFactors = new List<string>();
        var recommendations = new List<string>();
        string riskLevel;
        decimal completionProbability;

        // Behind schedule detection
        if (completionRate < progressPercentage - 20)
        {
            riskFactors.Add($"Significantly behind schedule: {completionRate:F0}% complete vs {progressPercentage:F0}% time elapsed");
            recommendations.Add("Consider descoping non-critical tasks");
            recommendations.Add("Schedule team capacity review meeting");
        }
        else if (completionRate < progressPercentage - 10)
        {
            riskFactors.Add($"Slightly behind schedule: {completionRate:F0}% complete vs {progressPercentage:F0}% time elapsed");
            recommendations.Add("Monitor daily progress closely");
        }

        // High workload detection
        var tasksPerDay = daysRemaining > 0 ? (todoTasks + inProgressTasks) / daysRemaining : 0;
        if (tasksPerDay > 3)
        {
            riskFactors.Add($"High remaining workload: {tasksPerDay:F1} tasks per day needed");
            recommendations.Add("Consider extending sprint or reducing scope");
        }

        // Blocker detection
        var activeBlockers = await _context.Blockers
            .Where(b => b.Task!.SprintId == sprintId && b.Status != BlockerStatus.Resolved)
            .CountAsync();
        
        if (activeBlockers > 0)
        {
            riskFactors.Add($"{activeBlockers} active blocker(s) detected");
            recommendations.Add("Prioritize blocker resolution immediately");
        }

        // Unassigned tasks detection
        var unassignedTasks = sprint.Tasks!.Count(t => t.AssigneeId == null && t.Status != TaskStatus.Done);
        if (unassignedTasks > 0)
        {
            riskFactors.Add($"{unassignedTasks} task(s) are unassigned");
            recommendations.Add("Assign all tasks to team members");
        }

        // Calculate risk level and completion probability
        if (riskFactors.Count >= 3 || completionRate < progressPercentage - 20)
        {
            riskLevel = "Critical";
            completionProbability = Math.Max(30, (decimal)completionRate);
        }
        else if (riskFactors.Count == 2 || completionRate < progressPercentage - 10)
        {
            riskLevel = "High";
            completionProbability = Math.Max(50, (decimal)completionRate);
        }
        else if (riskFactors.Count == 1)
        {
            riskLevel = "Medium";
            completionProbability = Math.Max(70, (decimal)completionRate);
        }
        else
        {
            riskLevel = "Low";
            completionProbability = Math.Min(95, Math.Max((decimal)completionRate, 75));
            recommendations.Add("Sprint is on track - maintain current momentum");
        }

        var predictedUnfinished = (int)Math.Ceiling((100 - completionProbability) / 100 * totalTasks);

        // Store analysis in database
        var analysis = new SprintRiskAnalysis
        {
            SprintId = sprintId,
            AnalyzedAt = DateTime.UtcNow,
            RiskLevel = riskLevel,
            CompletionProbability = completionProbability,
            PredictedUnfinishedTasks = predictedUnfinished,
            RiskFactors = JsonSerializer.Serialize(riskFactors),
            Recommendations = JsonSerializer.Serialize(recommendations)
        };
        _context.SprintRiskAnalyses.Add(analysis);
        await _context.SaveChangesAsync();

        return new SprintRiskResponse
        {
            SprintId = sprintId,
            SprintName = sprint.Name,
            RiskLevel = riskLevel,
            CompletionProbability = completionProbability,
            PredictedUnfinishedTasks = predictedUnfinished,
            RiskFactors = riskFactors,
            Recommendations = recommendations,
            AnalyzedAt = DateTime.UtcNow
        };
    }

    public async Task<List<BlockerPredictionResponse>> PredictBlockersAsync(int sprintId)
    {
        var tasks = await _context.Tasks
            .Where(t => t.SprintId == sprintId && t.Status != TaskStatus.Done)
            .Include(t => t.Blockers)
            .ToListAsync();

        var predictions = new List<BlockerPredictionResponse>();

        foreach (var task in tasks)
        {
            decimal blockerProbability = 0;
            var reasons = new List<string>();
            var actions = new List<string>();

            // Task stuck in same status for too long
            var lastUpdateDate = task.StartedAt ?? task.CreatedAt;
            var daysSinceUpdate = (DateTime.UtcNow - lastUpdateDate).TotalDays;
            if (daysSinceUpdate > 3 && task.Status == TaskStatus.InProgress)
            {
                blockerProbability += 40;
                reasons.Add($"Task hasn't been updated in {daysSinceUpdate:F0} days");
                actions.Add("Check in with assignee about progress");
            }

            // Task is unassigned
            if (task.AssigneeId == null)
            {
                blockerProbability += 30;
                reasons.Add("Task is unassigned");
                actions.Add("Assign task to a team member immediately");
            }

            // Task has high story points (complexity indicator)
            if (task.StoryPoints > 8)
            {
                blockerProbability += 20;
                reasons.Add($"High complexity task ({task.StoryPoints} story points)");
                actions.Add("Consider breaking down into smaller subtasks");
            }

            // Task already has active blockers
            var activeBlockers = task.Blockers?.Count(b => b.Status != BlockerStatus.Resolved) ?? 0;
            if (activeBlockers > 0)
            {
                blockerProbability += 30;
                reasons.Add($"Already has {activeBlockers} active blocker(s)");
                actions.Add("Escalate existing blockers for resolution");
            }

            // Task potentially overdue (check against sprint end date)
            var sprint = await _context.Sprints.FindAsync(task.SprintId);
            if (sprint != null && sprint.EndDate < DateTime.UtcNow && task.Status != TaskStatus.Done)
            {
                blockerProbability += 25;
                reasons.Add("Task is past sprint end date");
                actions.Add("Re-prioritize or move to next sprint");
            }

            blockerProbability = Math.Min(blockerProbability, 100);

            if (blockerProbability >= 50)
            {
                // Store prediction
                var prediction = new BlockerPrediction
                {
                    TaskId = task.Id,
                    PredictedAt = DateTime.UtcNow,
                    BlockerProbability = blockerProbability,
                    PredictionReason = string.Join("; ", reasons)
                };
                _context.BlockerPredictions.Add(prediction);

                predictions.Add(new BlockerPredictionResponse
                {
                    TaskId = task.Id,
                    TaskTitle = task.Title,
                    BlockerProbability = blockerProbability,
                    PredictionReason = string.Join(", ", reasons),
                    RecommendedAction = string.Join("; ", actions)
                });
            }
        }

        if (predictions.Any())
        {
            await _context.SaveChangesAsync();
        }

        return predictions.OrderByDescending(p => p.BlockerProbability).ToList();
    }

    public async Task<TeamHealthResponse> AnalyzeTeamHealthAsync(int teamId)
    {
        var team = await _context.Teams
            .Include(t => t.Members)
            .Include(t => t.Sprints)
                .ThenInclude(s => s.Tasks)
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team == null)
        {
            throw new Exception("Team not found");
        }

        var activeSprint = team.Sprints?
            .FirstOrDefault(s => s.Status == SprintStatus.Active);

        if (activeSprint == null)
        {
            return new TeamHealthResponse
            {
                TeamId = teamId,
                TeamName = team.Name,
                OverallHealthScore = 75,
                Insights = new List<HealthInsight>
                {
                    new() { Type = "Info", Message = "No active sprint to analyze", Severity = "Info" }
                }
            };
        }

        // Calculate metrics
        var tasks = activeSprint.Tasks ?? new List<SprintTask>();
        var totalTasks = tasks.Count;
        var completedTasks = tasks.Count(t => t.Status == TaskStatus.Done);
        
        // Workload balance
        var tasksPerMember = team.Members?
            .Select(m => tasks.Count(t => t.AssigneeId == m.UserId))
            .ToList() ?? new List<int>();
        
        var avgTasksPerMember = tasksPerMember.Any() ? tasksPerMember.Average() : 0;
        var maxDeviation = tasksPerMember.Any() ? tasksPerMember.Max() - avgTasksPerMember : 0;
        var workloadBalance = maxDeviation <= 2 ? 100 : Math.Max(0, 100 - (int)(maxDeviation * 10));

        // Velocity trend
        var previousSprints = team.Sprints?
            .Where(s => s.Status == SprintStatus.Completed)
            .OrderByDescending(s => s.EndDate)
            .Take(3)
            .ToList() ?? new List<Sprint>();

        decimal velocityTrend = 0;
        if (previousSprints.Any())
        {
            var prevCompletionRates = previousSprints
                .Select(s => s.Tasks!.Any() ? (decimal)s.Tasks!.Count(t => t.Status == TaskStatus.Done) / s.Tasks!.Count * 100 : 0)
                .ToList();
            var currentCompletionRate = totalTasks > 0 ? (decimal)completedTasks / totalTasks * 100 : 0;
            var avgPrevRate = prevCompletionRates.Average();
            velocityTrend = avgPrevRate > 0 ? ((currentCompletionRate - avgPrevRate) / avgPrevRate) * 100 : 0;
        }

        // Active blockers
        var activeBlockers = await _context.Blockers
            .Where(b => b.Task!.SprintId == activeSprint.Id && b.Status != BlockerStatus.Resolved)
            .CountAsync();

        // Morale score (from recent standups)
        var recentStandups = await _context.DailyStandups
            .Include(ds => ds.Sprint)
            .Where(ds => ds.Sprint.TeamId == teamId && ds.Date >= DateTime.UtcNow.AddDays(-7))
            .ToListAsync();

        var moraleScore = recentStandups.Any() && recentStandups.Any(s => s.Mood.HasValue)
            ? (decimal)recentStandups.Where(s => s.Mood.HasValue).Average(s => s.Mood!.Value) * 20
            : 70;

        // Collaboration score (placeholder - would analyze comment interactions, pair programming, etc.)
        var collaborationScore = 75m;

        // Burnout risk
        var burnoutRisk = 0m;
        if (activeBlockers > 5) burnoutRisk += 30;
        if (workloadBalance < 60) burnoutRisk += 25;
        if (moraleScore < 50) burnoutRisk += 25;
        if (velocityTrend < -20) burnoutRisk += 20;

        // Overall health score
        var overallHealthScore = (moraleScore + workloadBalance + collaborationScore + Math.Max(0, 100 - burnoutRisk)) / 4;

        // Generate insights
        var insights = new List<HealthInsight>();
        if (overallHealthScore >= 80)
        {
            insights.Add(new HealthInsight
            {
                Type = "Praise",
                Message = "Team is performing excellently! Keep up the great work.",
                Severity = "Info"
            });
        }

        if (workloadBalance < 70)
        {
            insights.Add(new HealthInsight
            {
                Type = "WorkloadImbalance",
                Message = "Work distribution is uneven. Consider rebalancing tasks.",
                Severity = "Warning"
            });
        }

        if (activeBlockers > 3)
        {
            insights.Add(new HealthInsight
            {
                Type = "BlockerAlert",
                Message = $"{activeBlockers} active blockers detected. Prioritize resolution.",
                Severity = "Critical"
            });
        }

        if (moraleScore < 60)
        {
            insights.Add(new HealthInsight
            {
                Type = "MoraleConcern",
                Message = "Team morale is below target. Schedule a team check-in.",
                Severity = "Warning"
            });
        }

        if (velocityTrend < -15)
        {
            insights.Add(new HealthInsight
            {
                Type = "VelocityDrop",
                Message = $"Velocity decreased by {Math.Abs(velocityTrend):F0}%. Investigate root causes.",
                Severity = "Warning"
            });
        }

        // Store metrics
        var metrics = new TeamHealthMetrics
        {
            TeamId = teamId,
            CalculatedAt = DateTime.UtcNow,
            OverallHealthScore = overallHealthScore,
            MoraleScore = moraleScore,
            WorkloadBalance = workloadBalance,
            CollaborationScore = collaborationScore,
            BurnoutRisk = burnoutRisk,
            ActiveBlockers = activeBlockers,
            VelocityTrend = velocityTrend,
            Insights = JsonSerializer.Serialize(insights)
        };
        _context.TeamHealthMetrics.Add(metrics);
        await _context.SaveChangesAsync();

        return new TeamHealthResponse
        {
            TeamId = teamId,
            TeamName = team.Name,
            OverallHealthScore = overallHealthScore,
            MoraleScore = moraleScore,
            WorkloadBalance = workloadBalance,
            CollaborationScore = collaborationScore,
            BurnoutRisk = burnoutRisk,
            ActiveBlockers = activeBlockers,
            VelocityTrend = velocityTrend,
            Insights = insights,
            CalculatedAt = DateTime.UtcNow
        };
    }

    public async Task<SmartStandupResponse> GenerateSmartStandupAsync(int teamId, DateTime date)
    {
        var team = await _context.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team == null)
        {
            throw new Exception("Team not found");
        }

        // Get active sprint first (needed for standup)
        var activeSprint = await _context.Sprints
            .Include(s => s.Tasks)
                .ThenInclude(t => t.Assignee)
            .Include(s => s.Tasks)
                .ThenInclude(t => t.Blockers)
            .FirstOrDefaultAsync(s => s.TeamId == teamId && s.Status == SprintStatus.Active);

        if (activeSprint == null)
        {
            throw new Exception("No active sprint found for team");
        }

        // Get or create standup for today (standups are per sprint, not per team)
        var standup = await _context.DailyStandups
            .FirstOrDefaultAsync(ds => ds.SprintId == activeSprint.Id && ds.Date.Date == date.Date);

        if (standup == null)
        {
            // Create a placeholder standup - in real scenario, this would be per user
            // For now, we'll just use the first user's standup or create a team-level summary
            standup = new DailyStandup
            {
                SprintId = activeSprint.Id,
                UserId = team.Members?.FirstOrDefault()?.UserId ?? 0, // Use first member as placeholder
                Date = date.Date,
                AiSummary = "Auto-generated smart standup"
            };
            _context.DailyStandups.Add(standup);
            await _context.SaveChangesAsync();
        }

        var userSummaries = new List<UserStandupSummary>();
        var totalBlockers = 0;
        var tasksCompleted = 0;
        var tasksInProgress = 0;

        if (activeSprint != null && team.Members != null)
        {
            foreach (var member in team.Members)
            {
                var userTasks = activeSprint.Tasks?
                    .Where(t => t.AssigneeId == member.UserId)
                    .ToList() ?? new List<SprintTask>();

                var completedYesterday = userTasks
                    .Where(t => t.Status == TaskStatus.Done && t.CompletedAt.HasValue && t.CompletedAt.Value.Date >= date.AddDays(-1).Date)
                    .Select(t => t.Title)
                    .ToList();

                var plannedToday = userTasks
                    .Where(t => t.Status != TaskStatus.Done)
                    .Take(3)
                    .Select(t => t.Title)
                    .ToList();

                var blockers = userTasks
                    .SelectMany(t => t.Blockers ?? new List<Blocker>())
                    .Where(b => b.Status != BlockerStatus.Resolved)
                    .Select(b => new BlockerDto
                    {
                        Id = b.Id,
                        Description = b.Description,
                        Severity = b.Severity,
                        Status = b.Status
                    })
                    .ToList();

                tasksCompleted += completedYesterday.Count;
                tasksInProgress += plannedToday.Count;
                totalBlockers += blockers.Count;

                var user = await _context.Users.FindAsync(member.UserId);
                userSummaries.Add(new UserStandupSummary
                {
                    UserId = member.UserId,
                    UserName = user?.FullName ?? user?.Email ?? "Unknown",
                    CompletedYesterday = completedYesterday,
                    PlannedToday = plannedToday,
                    Blockers = blockers,
                    HasBlockers = blockers.Any(),
                    SentimentIndicator = blockers.Any() ? "Negative" : completedYesterday.Any() ? "Positive" : "Neutral"
                });
            }
        }

        // Generate insights
        var insights = new List<StandupInsightDTO>();
        if (totalBlockers > 0)
        {
            insights.Add(new StandupInsightDTO
            {
                InsightType = "BlockerAlert",
                Title = "Active Blockers Detected",
                Description = $"{totalBlockers} blocker(s) need immediate attention",
                Priority = "High"
            });
        }

        if (tasksCompleted == 0)
        {
            insights.Add(new StandupInsightDTO
            {
                InsightType = "VelocityWarning",
                Title = "No Tasks Completed Yesterday",
                Description = "Team velocity may be slowing. Check for impediments.",
                Priority = "Medium"
            });
        }

        return new SmartStandupResponse
        {
            StandupId = standup.Id,
            Date = date,
            TeamName = team.Name,
            UserSummaries = userSummaries,
            Insights = insights,
            TotalBlockers = totalBlockers,
            TasksCompleted = tasksCompleted,
            TasksInProgress = tasksInProgress
        };
    }

    public async Task<List<string>> GenerateAIInsightsAsync(int sprintId)
    {
        var sprint = await _context.Sprints
            .Include(s => s.Tasks)
            .ThenInclude(t => t.Assignee)
            .FirstOrDefaultAsync(s => s.Id == sprintId);

        if (sprint == null) return new List<string>();

        var insights = new List<string>();
        var tasks = sprint.Tasks ?? new List<SprintTask>();
        var totalTasks = tasks.Count;
        var completedTasks = tasks.Count(t => t.Status == TaskStatus.Done);

        if (totalTasks > 0)
        {
            var completionRate = (double)completedTasks / totalTasks * 100;
            insights.Add($"📊 Sprint completion: {completionRate:F0}% ({completedTasks}/{totalTasks} tasks)");
        }

        var mostProductiveUser = tasks
            .Where(t => t.Assignee != null && t.Status == TaskStatus.Done)
            .GroupBy(t => t.Assignee!.FullName ?? t.Assignee.Email)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        if (mostProductiveUser != null)
        {
            insights.Add($"⭐ Top performer: {mostProductiveUser.Key} ({mostProductiveUser.Count()} tasks completed)");
        }

        return insights;
    }
}
