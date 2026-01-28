using Microsoft.EntityFrameworkCore;
using SprintIQ.API.Data;
using SprintIQ.API.DTOs;
using SprintIQ.API.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TaskStatus = SprintIQ.API.Models.TaskStatus;

namespace SprintIQ.API.Services;

public class JiraService : IJiraService
{
    private readonly SprintIQDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<JiraService> _logger;

    public JiraService(SprintIQDbContext context, IHttpClientFactory httpClientFactory, ILogger<JiraService> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<JiraWorkspaceResponse> ConnectJiraWorkspaceAsync(ConnectJiraRequest request)
    {
        // Check if workspace already exists for this team
        var existing = await _context.JiraWorkspaces
            .FirstOrDefaultAsync(w => w.TeamId == request.TeamId);

        if (existing != null)
        {
            // Update existing
            existing.JiraUrl = request.JiraUrl;
            existing.JiraEmail = request.JiraEmail;
            existing.JiraApiToken = request.JiraApiToken; // TODO: Encrypt this
            existing.ProjectKey = request.ProjectKey;
            existing.IsActive = true;
            _context.JiraWorkspaces.Update(existing);
        }
        else
        {
            // Create new
            existing = new JiraWorkspace
            {
                TeamId = request.TeamId,
                JiraUrl = request.JiraUrl,
                JiraEmail = request.JiraEmail,
                JiraApiToken = request.JiraApiToken, // TODO: Encrypt this
                ProjectKey = request.ProjectKey,
                IsActive = true,
                LastSyncDate = DateTime.UtcNow
            };
            _context.JiraWorkspaces.Add(existing);
        }

        await _context.SaveChangesAsync();

        // Test connection
        var isValid = await TestJiraConnectionAsync(existing.Id);
        if (!isValid)
        {
            throw new Exception("Invalid Jira credentials or connection failed");
        }

        return new JiraWorkspaceResponse
        {
            Id = existing.Id,
            TeamId = existing.TeamId,
            JiraUrl = existing.JiraUrl,
            JiraEmail = existing.JiraEmail,
            ProjectKey = existing.ProjectKey,
            LastSyncDate = existing.LastSyncDate,
            IsActive = existing.IsActive
        };
    }

    public async Task<bool> TestJiraConnectionAsync(int workspaceId)
    {
        var workspace = await _context.JiraWorkspaces.FindAsync(workspaceId);
        if (workspace == null) return false;

        try
        {
            var client = CreateJiraHttpClient(workspace);
            var response = await client.GetAsync($"/rest/api/3/project/{workspace.ProjectKey}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Jira connection test failed for workspace {WorkspaceId}", workspaceId);
            return false;
        }
    }

    public async Task<SyncStatusResponse> SyncFromJiraAsync(int workspaceId)
    {
        var workspace = await _context.JiraWorkspaces.FindAsync(workspaceId);
        if (workspace == null)
        {
            return new SyncStatusResponse { IsSyncing = false, ErrorMessage = "Workspace not found" };
        }

        var syncRecord = new JiraSync
        {
            JiraWorkspaceId = workspaceId,
            SyncType = "Full",
            SyncStartedAt = DateTime.UtcNow,
            Status = "In Progress"
        };
        _context.JiraSyncs.Add(syncRecord);
        await _context.SaveChangesAsync();

        try
        {
            int totalSynced = 0;

            // Sync Sprints
            var sprints = await FetchJiraSprintsAsync(workspaceId);
            totalSynced += await SyncSprintsAsync(workspace, sprints);

            // Sync Issues
            var issues = await FetchJiraIssuesAsync(workspaceId);
            totalSynced += await SyncIssuesAsync(workspace, issues);

            syncRecord.Status = "Completed";
            syncRecord.SyncCompletedAt = DateTime.UtcNow;
            syncRecord.ItemsSynced = totalSynced;

            workspace.LastSyncDate = DateTime.UtcNow;
            _context.JiraWorkspaces.Update(workspace);
            _context.JiraSyncs.Update(syncRecord);
            await _context.SaveChangesAsync();

            return new SyncStatusResponse
            {
                IsSyncing = false,
                LastSyncDate = workspace.LastSyncDate,
                TotalItemsSynced = totalSynced
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Jira sync failed for workspace {WorkspaceId}", workspaceId);
            syncRecord.Status = "Failed";
            syncRecord.ErrorMessage = ex.Message;
            syncRecord.SyncCompletedAt = DateTime.UtcNow;
            _context.JiraSyncs.Update(syncRecord);
            await _context.SaveChangesAsync();

            return new SyncStatusResponse
            {
                IsSyncing = false,
                LastSyncDate = workspace.LastSyncDate,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<List<JiraIssueResponse>> FetchJiraIssuesAsync(int workspaceId, string? sprintId = null)
    {
        var workspace = await _context.JiraWorkspaces.FindAsync(workspaceId);
        if (workspace == null) return new List<JiraIssueResponse>();

        try
        {
            var client = CreateJiraHttpClient(workspace);
            var jql = $"project={workspace.ProjectKey}";
            if (!string.IsNullOrEmpty(sprintId))
            {
                jql += $" AND sprint={sprintId}";
            }

            var response = await client.GetAsync($"/rest/api/3/search?jql={Uri.EscapeDataString(jql)}&maxResults=1000");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JiraSearchResponse>();
            return result?.Issues ?? new List<JiraIssueResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch Jira issues for workspace {WorkspaceId}", workspaceId);
            return new List<JiraIssueResponse>();
        }
    }

    public async Task<List<JiraSprint>> FetchJiraSprintsAsync(int workspaceId)
    {
        var workspace = await _context.JiraWorkspaces.FindAsync(workspaceId);
        if (workspace == null) return new List<JiraSprint>();

        try
        {
            var client = CreateJiraHttpClient(workspace);
            
            // First, get the board ID for the project
            var boardResponse = await client.GetAsync($"/rest/agile/1.0/board?projectKeyOrId={workspace.ProjectKey}");
            boardResponse.EnsureSuccessStatusCode();
            var boardResult = await boardResponse.Content.ReadFromJsonAsync<JiraBoardResponse>();
            
            if (boardResult?.Values == null || !boardResult.Values.Any())
            {
                return new List<JiraSprint>();
            }

            var boardId = boardResult.Values.First().Id;

            // Now fetch sprints for this board
            var sprintResponse = await client.GetAsync($"/rest/agile/1.0/board/{boardId}/sprint");
            sprintResponse.EnsureSuccessStatusCode();
            var sprintResult = await sprintResponse.Content.ReadFromJsonAsync<JiraSprintResponse>();
            
            return sprintResult?.Values ?? new List<JiraSprint>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch Jira sprints for workspace {WorkspaceId}", workspaceId);
            return new List<JiraSprint>();
        }
    }

    public async Task<JiraWorkspaceResponse?> GetJiraWorkspaceAsync(int teamId)
    {
        var workspace = await _context.JiraWorkspaces
            .FirstOrDefaultAsync(w => w.TeamId == teamId && w.IsActive);

        if (workspace == null) return null;

        return new JiraWorkspaceResponse
        {
            Id = workspace.Id,
            TeamId = workspace.TeamId,
            JiraUrl = workspace.JiraUrl,
            JiraEmail = workspace.JiraEmail,
            ProjectKey = workspace.ProjectKey,
            LastSyncDate = workspace.LastSyncDate,
            IsActive = workspace.IsActive
        };
    }

    private HttpClient CreateJiraHttpClient(JiraWorkspace workspace)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(workspace.JiraUrl);
        
        // Jira uses Basic Auth with email and API token
        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{workspace.JiraEmail}:{workspace.JiraApiToken}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        return client;
    }

    private async Task<int> SyncSprintsAsync(JiraWorkspace workspace, List<JiraSprint> jiraSprints)
    {
        int synced = 0;
        foreach (var jiraSprint in jiraSprints)
        {
            var existingSprint = await _context.Sprints
                .FirstOrDefaultAsync(s => s.TeamId == workspace.TeamId && s.Name == jiraSprint.Name);

            if (existingSprint == null)
            {
                var sprint = new Sprint
                {
                    Name = jiraSprint.Name,
                    TeamId = workspace.TeamId,
                    StartDate = jiraSprint.StartDate ?? DateTime.UtcNow,
                    EndDate = jiraSprint.EndDate ?? DateTime.UtcNow.AddDays(14),
                    Goal = $"Synced from Jira - {jiraSprint.Name}",
                    Status = jiraSprint.State == "active" ? SprintStatus.Active : jiraSprint.State == "closed" ? SprintStatus.Completed : SprintStatus.Planning
                };
                _context.Sprints.Add(sprint);
                synced++;
            }
        }
        await _context.SaveChangesAsync();
        return synced;
    }

    private async Task<int> SyncIssuesAsync(JiraWorkspace workspace, List<JiraIssueResponse> jiraIssues)
    {
        int synced = 0;
        foreach (var issue in jiraIssues)
        {
            // Find the sprint for this task
            var sprintName = issue.Fields.Sprint?.Name;
            Sprint? sprint = null;
            if (!string.IsNullOrEmpty(sprintName))
            {
                sprint = await _context.Sprints
                    .FirstOrDefaultAsync(s => s.TeamId == workspace.TeamId && s.Name == sprintName);
            }

            if (sprint == null) continue; // Skip issues not in a sprint

            // Find or create user
            var assigneeEmail = issue.Fields.Assignee?.EmailAddress;
            User? assignedUser = null;
            if (!string.IsNullOrEmpty(assigneeEmail))
            {
                assignedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == assigneeEmail);
            }

            // Check if task already exists (by Jira key)
            var existingTask = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Title.Contains(issue.Key));

            if (existingTask == null)
            {
                var status = MapJiraStatusToSprintIQStatus(issue.Fields.Status.Name);
                var priority = MapJiraPriorityToSprintIQPriority(issue.Fields.Priority?.Name ?? "Medium");
                var createdAt = issue.Fields.Created ?? DateTime.UtcNow;
                var updatedAt = issue.Fields.Updated ?? DateTime.UtcNow;
                
                var task = new SprintTask
                {
                    SprintId = sprint.Id,
                    Title = $"[{issue.Key}] {issue.Fields.Summary}",
                    Description = issue.Fields.Description ?? string.Empty,
                    Status = status,
                    Priority = priority,
                    AssigneeId = assignedUser?.Id,
                    CreatedAt = createdAt,
                    StartedAt = (status == TaskStatus.InProgress || status == TaskStatus.InReview || status == TaskStatus.Done) ? updatedAt : null,
                    CompletedAt = status == TaskStatus.Done ? updatedAt : null
                };
                _context.Tasks.Add(task);
                synced++;
            }
        }
        await _context.SaveChangesAsync();
        return synced;
    }

    private TaskStatus MapJiraStatusToSprintIQStatus(string jiraStatus)
    {
        return jiraStatus.ToLower() switch
        {
            "to do" or "todo" => TaskStatus.Todo,
            "in progress" => TaskStatus.InProgress,
            "in review" or "review" => TaskStatus.InReview,
            "done" or "completed" => TaskStatus.Done,
            "closed" => TaskStatus.Done,
            _ => TaskStatus.Todo
        };
    }

    private TaskPriority MapJiraPriorityToSprintIQPriority(string jiraPriority)
    {
        return jiraPriority.ToLower() switch
        {
            "critical" or "highest" => TaskPriority.Critical,
            "high" or "major" => TaskPriority.High,
            "medium" or "normal" => TaskPriority.Medium,
            "low" or "minor" or "trivial" => TaskPriority.Low,
            _ => TaskPriority.Medium
        };
    }

    // Helper classes for Jira API responses
    private class JiraSearchResponse
    {
        public List<JiraIssueResponse> Issues { get; set; } = new();
    }

    private class JiraBoardResponse
    {
        public List<JiraBoard> Values { get; set; } = new();
    }

    private class JiraBoard
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private class JiraSprintResponse
    {
        public List<JiraSprint> Values { get; set; } = new();
    }
}
