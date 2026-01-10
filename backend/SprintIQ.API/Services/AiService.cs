using SprintIQ.API.DTOs;

namespace SprintIQ.API.Services;

/// <summary>
/// AI Service - Currently provides rule-based summaries.
/// Can be extended to integrate with OpenAI/Azure OpenAI for advanced AI features.
/// </summary>
public class AiService : IAiService
{
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
}
