using SprintIQ.API.DTOs;
using SprintIQ.API.Models;

namespace SprintIQ.API.Services;

public interface IJiraService
{
    Task<JiraWorkspaceResponse> ConnectJiraWorkspaceAsync(ConnectJiraRequest request);
    Task<bool> TestJiraConnectionAsync(int workspaceId);
    Task<SyncStatusResponse> SyncFromJiraAsync(int workspaceId);
    Task<List<JiraIssueResponse>> FetchJiraIssuesAsync(int workspaceId, string? sprintId = null);
    Task<List<JiraSprint>> FetchJiraSprintsAsync(int workspaceId);
    Task<JiraWorkspaceResponse?> GetJiraWorkspaceAsync(int teamId);
}
