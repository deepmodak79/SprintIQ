using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SprintIQ.API.DTOs;
using SprintIQ.API.Services;

namespace SprintIQ.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class JiraController : ControllerBase
{
    private readonly IJiraService _jiraService;
    private readonly IAiService _aiService;
    private readonly ILogger<JiraController> _logger;

    public JiraController(IJiraService jiraService, IAiService aiService, ILogger<JiraController> logger)
    {
        _jiraService = jiraService;
        _aiService = aiService;
        _logger = logger;
    }

    /// <summary>
    /// Connect a Jira workspace to a team
    /// </summary>
    [HttpPost("connect")]
    public async Task<ActionResult<JiraWorkspaceResponse>> ConnectJiraWorkspace([FromBody] ConnectJiraRequest request)
    {
        try
        {
            var workspace = await _jiraService.ConnectJiraWorkspaceAsync(request);
            return Ok(workspace);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect Jira workspace");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get Jira workspace for a team
    /// </summary>
    [HttpGet("workspace/team/{teamId}")]
    public async Task<ActionResult<JiraWorkspaceResponse>> GetJiraWorkspace(int teamId)
    {
        var workspace = await _jiraService.GetJiraWorkspaceAsync(teamId);
        if (workspace == null)
        {
            return NotFound(new { error = "No Jira workspace connected for this team" });
        }
        return Ok(workspace);
    }

    /// <summary>
    /// Test Jira connection
    /// </summary>
    [HttpPost("test/{workspaceId}")]
    public async Task<ActionResult<bool>> TestConnection(int workspaceId)
    {
        var isValid = await _jiraService.TestJiraConnectionAsync(workspaceId);
        return Ok(new { isValid, message = isValid ? "Connection successful" : "Connection failed" });
    }

    /// <summary>
    /// Sync data from Jira
    /// </summary>
    [HttpPost("sync/{workspaceId}")]
    public async Task<ActionResult<SyncStatusResponse>> SyncFromJira(int workspaceId)
    {
        var result = await _jiraService.SyncFromJiraAsync(workspaceId);
        return Ok(result);
    }

    /// <summary>
    /// Get sync status
    /// </summary>
    [HttpGet("sync/status/{workspaceId}")]
    public async Task<ActionResult<SyncStatusResponse>> GetSyncStatus(int workspaceId)
    {
        var result = await _jiraService.SyncFromJiraAsync(workspaceId);
        return Ok(result);
    }

    /// <summary>
    /// Fetch Jira issues
    /// </summary>
    [HttpGet("issues/{workspaceId}")]
    public async Task<ActionResult<List<JiraIssueResponse>>> GetJiraIssues(int workspaceId, [FromQuery] string? sprintId = null)
    {
        var issues = await _jiraService.FetchJiraIssuesAsync(workspaceId, sprintId);
        return Ok(issues);
    }

    /// <summary>
    /// Fetch Jira sprints
    /// </summary>
    [HttpGet("sprints/{workspaceId}")]
    public async Task<ActionResult<List<JiraSprint>>> GetJiraSprints(int workspaceId)
    {
        var sprints = await _jiraService.FetchJiraSprintsAsync(workspaceId);
        return Ok(sprints);
    }
}
