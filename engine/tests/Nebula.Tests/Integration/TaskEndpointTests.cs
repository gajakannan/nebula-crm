using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nebula.Application.DTOs;
using Nebula.Infrastructure.Persistence;

namespace Nebula.Tests.Integration;

[Collection(IntegrationTestCollection.Name)]
public class TaskEndpointTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();

    public async Task InitializeAsync()
    {
        // Clean tasks and timeline events before each test to prevent data accumulation
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.ExecuteSqlRawAsync(
            "DELETE FROM \"ActivityTimelineEvents\" WHERE \"EntityType\" = 'Task'; " +
            "DELETE FROM \"Tasks\";");
    }

    public Task DisposeAsync()
    {
        TestAuthHandler.TestSubject = "test-user-001";
        TestAuthHandler.TestRole = "Admin";
        TestAuthHandler.TestDisplayName = "Test User";
        TestAuthHandler.ResetF0009Overrides();
        return Task.CompletedTask;
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  S0001: POST /tasks — Create Task
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateTask_WithValidData_Returns201()
    {
        var userId = await GetCurrentUserId();
        var dto = new TaskCreateRequestDto("Follow up with broker", "Call broker re: renewal",
            "High", DateTime.UtcNow.AddDays(7), userId, null, null);

        var response = await _client.PostAsJsonAsync("/tasks", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<TaskDto>();
        result.Should().NotBeNull();
        result!.Title.Should().Be("Follow up with broker");
        result.Status.Should().Be("Open");
        result.Priority.Should().Be("High");
        result.AssignedToUserId.Should().Be(userId);
    }

    [Fact]
    public async Task CreateTask_DefaultPriority_ReturnsNormal()
    {
        var userId = await GetCurrentUserId();
        var dto = new TaskCreateRequestDto("Default priority task", null, null, null, userId, null, null);

        var response = await _client.PostAsJsonAsync("/tasks", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<TaskDto>();
        result!.Priority.Should().Be("Normal");
    }

    [Fact]
    public async Task CreateTask_SelfAssignmentViolation_Returns403()
    {
        // Non-manager roles cannot assign tasks to other users
        TestAuthHandler.TestSubject = "test-user-nonadmin-selfassign";
        TestAuthHandler.TestRole = "DistributionUser";
        TestAuthHandler.TestNebulaRoles = ["DistributionUser"];
        TestAuthHandler.TestDisplayName = "Non-Manager User";

        var userId = await GetCurrentUserId();
        var otherUserId = Guid.NewGuid();
        var dto = new TaskCreateRequestDto("Bad assignment", null, null, null, otherUserId, null, null);

        var response = await _client.PostAsJsonAsync("/tasks", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateTask_MissingTitle_Returns400()
    {
        var userId = await GetCurrentUserId();
        var json = JsonSerializer.Serialize(new { assignedToUserId = userId });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/tasks", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTask_TitleTooLong_Returns400()
    {
        var userId = await GetCurrentUserId();
        var dto = new TaskCreateRequestDto(new string('A', 256), null, null, null, userId, null, null);

        var response = await _client.PostAsJsonAsync("/tasks", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTask_InvalidPriority_Returns400()
    {
        var userId = await GetCurrentUserId();
        var dto = new TaskCreateRequestDto("Test", null, "Critical", null, userId, null, null);

        var response = await _client.PostAsJsonAsync("/tasks", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTask_InvalidLinkedEntityType_Returns400()
    {
        var userId = await GetCurrentUserId();
        var dto = new TaskCreateRequestDto("Test", null, null, null, userId, "Unknown", Guid.NewGuid());

        var response = await _client.PostAsJsonAsync("/tasks", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTask_LinkedEntityTypeMissingId_Returns400()
    {
        var userId = await GetCurrentUserId();
        var dto = new TaskCreateRequestDto("Test", null, null, null, userId, "Broker", null);

        var response = await _client.PostAsJsonAsync("/tasks", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTask_WithLinkedEntity_Returns201()
    {
        var userId = await GetCurrentUserId();
        var linkedId = Guid.NewGuid();
        var dto = new TaskCreateRequestDto("Linked task", null, null, null, userId, "Submission", linkedId);

        var response = await _client.PostAsJsonAsync("/tasks", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<TaskDto>();
        result!.LinkedEntityType.Should().Be("Submission");
        result.LinkedEntityId.Should().Be(linkedId);
    }

    [Fact]
    public async Task CreateTask_ExternalUser_Returns403()
    {
        TestAuthHandler.TestRole = "ExternalUser";
        TestAuthHandler.TestNebulaRoles = [];
        var dto = new TaskCreateRequestDto("Bad task", null, null, null, Guid.NewGuid(), null, null);

        var response = await _client.PostAsJsonAsync("/tasks", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  S0002: PUT /tasks/{taskId} — Update Task
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task UpdateTask_ChangeTitle_Returns200()
    {
        var taskId = await CreateTestTask();
        var json = JsonSerializer.Serialize(new { title = "Updated Title" });

        var response = await PutJsonAsync($"/tasks/{taskId}", json);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TaskDto>();
        result!.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task UpdateTask_StatusOpenToInProgress_Returns200()
    {
        var taskId = await CreateTestTask();
        var json = JsonSerializer.Serialize(new { status = "InProgress" });

        var response = await PutJsonAsync($"/tasks/{taskId}", json);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TaskDto>();
        result!.Status.Should().Be("InProgress");
    }

    [Fact]
    public async Task UpdateTask_StatusInProgressToDone_SetsCompletedAt()
    {
        var taskId = await CreateTestTask();
        // First transition to InProgress
        await PutJsonAsync($"/tasks/{taskId}", JsonSerializer.Serialize(new { status = "InProgress" }));
        // Then to Done
        var response = await PutJsonAsync($"/tasks/{taskId}", JsonSerializer.Serialize(new { status = "Done" }));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TaskDto>();
        result!.Status.Should().Be("Done");
        result.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateTask_StatusOpenToDone_Returns409()
    {
        var taskId = await CreateTestTask();
        var json = JsonSerializer.Serialize(new { status = "Done" });

        var response = await PutJsonAsync($"/tasks/{taskId}", json);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateTask_StatusDoneToOpen_ClearsCompletedAt()
    {
        var taskId = await CreateTestTask();
        await PutJsonAsync($"/tasks/{taskId}", JsonSerializer.Serialize(new { status = "InProgress" }));
        await PutJsonAsync($"/tasks/{taskId}", JsonSerializer.Serialize(new { status = "Done" }));

        var response = await PutJsonAsync($"/tasks/{taskId}", JsonSerializer.Serialize(new { status = "Open" }));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TaskDto>();
        result!.Status.Should().Be("Open");
        result.CompletedAt.Should().BeNull();
    }

    [Fact]
    public async Task UpdateTask_EmptyPayload_Returns400()
    {
        var taskId = await CreateTestTask();

        var response = await PutJsonAsync($"/tasks/{taskId}", "{}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTask_NotFound_Returns404()
    {
        var json = JsonSerializer.Serialize(new { title = "X" });

        // Pass a dummy rowVersion since the task doesn't exist (can't auto-fetch)
        var response = await PutJsonAsync($"/tasks/{Guid.NewGuid()}", json, rowVersion: 1);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTask_MissingIfMatch_Returns428()
    {
        var taskId = await CreateTestTask();
        var json = JsonSerializer.Serialize(new { title = "X" });

        // Send PUT without If-Match header
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/tasks/{taskId}", content);

        ((int)response.StatusCode).Should().Be(428);
    }

    [Fact]
    public async Task UpdateTask_InvalidStatus_Returns400()
    {
        var taskId = await CreateTestTask();
        var json = JsonSerializer.Serialize(new { status = "Cancelled" });

        var response = await PutJsonAsync($"/tasks/{taskId}", json);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTask_TitleTooLong_Returns400()
    {
        var taskId = await CreateTestTask();
        var json = JsonSerializer.Serialize(new { title = new string('A', 256) });

        var response = await PutJsonAsync($"/tasks/{taskId}", json);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTask_ExternalUser_Returns404()
    {
        var taskId = await CreateTestTask();
        TestAuthHandler.TestRole = "ExternalUser";
        TestAuthHandler.TestNebulaRoles = [];

        var response = await PutJsonAsync($"/tasks/{taskId}", JsonSerializer.Serialize(new { title = "X" }));

        // IDOR normalization: ExternalUser gets 404 (not 403) to prevent entity existence leakage
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  S0003: DELETE /tasks/{taskId} — Delete Task
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task DeleteTask_OwnTask_Returns204()
    {
        var taskId = await CreateTestTask();

        var response = await _client.DeleteAsync($"/tasks/{taskId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTask_ThenGetReturns404()
    {
        var taskId = await CreateTestTask();
        await _client.DeleteAsync($"/tasks/{taskId}");

        var response = await _client.GetAsync($"/tasks/{taskId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTask_AlreadyDeleted_Returns404()
    {
        var taskId = await CreateTestTask();
        await _client.DeleteAsync($"/tasks/{taskId}");

        var response = await _client.DeleteAsync($"/tasks/{taskId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTask_NotFound_Returns404()
    {
        var response = await _client.DeleteAsync($"/tasks/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTask_CompletedTask_Returns204()
    {
        var taskId = await CreateTestTask();
        await PutJsonAsync($"/tasks/{taskId}", JsonSerializer.Serialize(new { status = "InProgress" }));
        await PutJsonAsync($"/tasks/{taskId}", JsonSerializer.Serialize(new { status = "Done" }));

        var response = await _client.DeleteAsync($"/tasks/{taskId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTask_ExternalUser_Returns404()
    {
        var taskId = await CreateTestTask();
        TestAuthHandler.TestRole = "ExternalUser";
        TestAuthHandler.TestNebulaRoles = [];

        var response = await _client.DeleteAsync($"/tasks/{taskId}");

        // IDOR normalization: ExternalUser gets 404 (not 403) to prevent entity existence leakage
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  S0001: Additional Create edge cases
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateTask_DescriptionTooLong_Returns400()
    {
        var userId = await GetCurrentUserId();
        var dto = new TaskCreateRequestDto("Test", new string('B', 2001), null, null, userId, null, null);

        var response = await _client.PostAsJsonAsync("/tasks", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTask_PastDueDate_Returns201()
    {
        var userId = await GetCurrentUserId();
        var pastDate = DateTime.UtcNow.AddDays(-7);
        var dto = new TaskCreateRequestDto("Overdue task", null, null, pastDate, userId, null, null);

        var response = await _client.PostAsJsonAsync("/tasks", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<TaskDto>();
        result!.DueDate.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateTask_ThenAppearsInMyTasks()
    {
        var userId = await GetCurrentUserId();
        var uniqueTitle = $"Visible-{Guid.NewGuid():N}";
        var dto = new TaskCreateRequestDto(uniqueTitle, null, null, null, userId, null, null);
        await _client.PostAsJsonAsync("/tasks", dto);

        var response = await _client.GetAsync("/my/tasks?limit=100");
        var body = await response.Content.ReadAsStringAsync();

        body.Should().Contain(uniqueTitle);
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  S0002: Additional Update edge cases
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task UpdateTask_DescriptionTooLong_Returns400()
    {
        var taskId = await CreateTestTask();
        var json = JsonSerializer.Serialize(new { description = new string('X', 2001) });

        var response = await PutJsonAsync($"/tasks/{taskId}", json);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTask_InvalidPriority_Returns400()
    {
        var taskId = await CreateTestTask();
        var json = JsonSerializer.Serialize(new { priority = "Critical" });

        var response = await PutJsonAsync($"/tasks/{taskId}", json);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTask_StatusDoneToInProgress_ClearsCompletedAt()
    {
        var taskId = await CreateTestTask();
        await PutJsonAsync($"/tasks/{taskId}", JsonSerializer.Serialize(new { status = "InProgress" }));
        await PutJsonAsync($"/tasks/{taskId}", JsonSerializer.Serialize(new { status = "Done" }));

        var response = await PutJsonAsync($"/tasks/{taskId}", JsonSerializer.Serialize(new { status = "InProgress" }));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TaskDto>();
        result!.Status.Should().Be("InProgress");
        result.CompletedAt.Should().BeNull();
    }

    [Fact]
    public async Task UpdateTask_StatusInProgressToOpen_Returns200()
    {
        var taskId = await CreateTestTask();
        await PutJsonAsync($"/tasks/{taskId}", JsonSerializer.Serialize(new { status = "InProgress" }));

        var response = await PutJsonAsync($"/tasks/{taskId}", JsonSerializer.Serialize(new { status = "Open" }));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TaskDto>();
        result!.Status.Should().Be("Open");
    }

    [Fact]
    public async Task UpdateTask_ClearDueDate_SetsNull()
    {
        var userId = await GetCurrentUserId();
        var dto = new TaskCreateRequestDto("Due date task", null, null, DateTime.UtcNow.AddDays(5), userId, null, null);
        var createResp = await _client.PostAsJsonAsync("/tasks", dto);
        var created = await createResp.Content.ReadFromJsonAsync<TaskDto>();

        // Send explicit null for dueDate
        var response = await PutJsonAsync($"/tasks/{created!.Id}",
            "{\"dueDate\": null}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TaskDto>();
        result!.DueDate.Should().BeNull();
    }

    [Fact]
    public async Task UpdateTask_ClearDescription_SetsNull()
    {
        var userId = await GetCurrentUserId();
        var dto = new TaskCreateRequestDto("Desc task", "Original description", null, null, userId, null, null);
        var createResp = await _client.PostAsJsonAsync("/tasks", dto);
        var created = await createResp.Content.ReadFromJsonAsync<TaskDto>();

        var response = await PutJsonAsync($"/tasks/{created!.Id}",
            "{\"description\": null}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TaskDto>();
        result!.Description.Should().BeNull();
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  S0003: Additional Delete edge cases
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task DeleteTask_ThenExcludedFromMyTasks()
    {
        var uniqueTitle = $"DeleteMe-{Guid.NewGuid():N}";
        var taskId = await CreateTestTask(uniqueTitle);
        await _client.DeleteAsync($"/tasks/{taskId}");

        var response = await _client.GetAsync("/my/tasks?limit=100");
        var body = await response.Content.ReadAsStringAsync();

        body.Should().NotContain(uniqueTitle);
    }

    [Fact]
    public async Task DeleteTask_InProgressTask_Returns204()
    {
        var taskId = await CreateTestTask();
        await PutJsonAsync($"/tasks/{taskId}", JsonSerializer.Serialize(new { status = "InProgress" }));

        var response = await _client.DeleteAsync($"/tasks/{taskId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  Full Lifecycle
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task FullLifecycle_CreateUpdateCompletionReopenDelete()
    {
        // Create
        var userId = await GetCurrentUserId();
        var createDto = new TaskCreateRequestDto("Lifecycle task", "Test full lifecycle", "Normal",
            DateTime.UtcNow.AddDays(3), userId, null, null);
        var createResp = await _client.PostAsJsonAsync("/tasks", createDto);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var task = await createResp.Content.ReadFromJsonAsync<TaskDto>();

        // Update title
        var updateResp = await PutJsonAsync($"/tasks/{task!.Id}", JsonSerializer.Serialize(new { title = "Updated lifecycle" }));
        updateResp.StatusCode.Should().Be(HttpStatusCode.OK);

        // Open → InProgress
        var progressResp = await PutJsonAsync($"/tasks/{task.Id}", JsonSerializer.Serialize(new { status = "InProgress" }));
        progressResp.StatusCode.Should().Be(HttpStatusCode.OK);

        // InProgress → Done
        var doneResp = await PutJsonAsync($"/tasks/{task.Id}", JsonSerializer.Serialize(new { status = "Done" }));
        doneResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var done = await doneResp.Content.ReadFromJsonAsync<TaskDto>();
        done!.CompletedAt.Should().NotBeNull();

        // Reopen: Done → Open
        var reopenResp = await PutJsonAsync($"/tasks/{task.Id}", JsonSerializer.Serialize(new { status = "Open" }));
        reopenResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var reopened = await reopenResp.Content.ReadFromJsonAsync<TaskDto>();
        reopened!.CompletedAt.Should().BeNull();

        // Delete
        var deleteResp = await _client.DeleteAsync($"/tasks/{task.Id}");
        deleteResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify gone
        var getResp = await _client.GetAsync($"/tasks/{task.Id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  F0004: GET /tasks — Task List
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task GetTasks_MyWorkView_ReturnsOwnTasks()
    {
        // Create a task assigned to the current user, then verify it appears in myWork view
        var userId = await GetCurrentUserId();
        var uniqueTitle = $"MyWork-{Guid.NewGuid():N}";
        var dto = new TaskCreateRequestDto(uniqueTitle, null, null, null, userId, null, null);
        await _client.PostAsJsonAsync("/tasks", dto);

        var response = await _client.GetAsync("/tasks?view=myWork");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(uniqueTitle);
        var result = await response.Content.ReadFromJsonAsync<TaskListResponseDto>();
        result.Should().NotBeNull();
        result!.Page.Should().Be(1);
    }

    [Fact]
    public async Task GetTasks_MyWorkDefault_WhenNoViewParam()
    {
        // Without ?view=, should default to myWork
        var userId = await GetCurrentUserId();
        var uniqueTitle = $"DefaultView-{Guid.NewGuid():N}";
        var dto = new TaskCreateRequestDto(uniqueTitle, null, null, null, userId, null, null);
        await _client.PostAsJsonAsync("/tasks", dto);

        var response = await _client.GetAsync("/tasks");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(uniqueTitle);
    }

    [Fact]
    public async Task GetTasks_AssignedByMeView_Admin_ReturnsDelegatedTasks()
    {
        // Admin (user-001) creates a task assigned to user-002;
        // then queries assignedByMe — should see that delegated task
        TestAuthHandler.TestSubject = "test-user-001";
        TestAuthHandler.TestRole = "Admin";
        TestAuthHandler.TestDisplayName = "Admin User";

        var user002Id = await GetOrCreateUserId("test-user-002", "Assignee User", "DistributionUser");
        var user001Id = await GetCurrentUserId();

        var uniqueTitle = $"Delegated-{Guid.NewGuid():N}";
        var dto = new TaskCreateRequestDto(uniqueTitle, null, null, null, user002Id, null, null);
        var createResp = await _client.PostAsJsonAsync("/tasks", dto);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await _client.GetAsync("/tasks?view=assignedByMe");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(uniqueTitle);
    }

    [Fact]
    public async Task GetTasks_AssignedByMeView_NonManager_Returns403()
    {
        TestAuthHandler.TestSubject = "test-user-nonadmin-001";
        TestAuthHandler.TestRole = "DistributionUser";
        TestAuthHandler.TestNebulaRoles = ["DistributionUser"];
        TestAuthHandler.TestDisplayName = "Distribution User";

        var response = await _client.GetAsync("/tasks?view=assignedByMe");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetTasks_Pagination_ReturnsCorrectPage()
    {
        var userId = await GetCurrentUserId();
        // Create several tasks to exercise pagination
        for (var i = 0; i < 3; i++)
        {
            var dto = new TaskCreateRequestDto($"Paginated-{Guid.NewGuid():N}", null, null, null, userId, null, null);
            await _client.PostAsJsonAsync("/tasks", dto);
        }

        var response = await _client.GetAsync("/tasks?view=myWork&page=1&pageSize=2");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TaskListResponseDto>();
        result.Should().NotBeNull();
        result!.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.TotalCount.Should().BeGreaterThanOrEqualTo(3);
        result.TotalPages.Should().BeGreaterThanOrEqualTo(2);
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTasks_EmptyResult_ReturnsEmptyArray()
    {
        // Use a unique subject that has never had tasks created
        TestAuthHandler.TestSubject = $"test-user-empty-{Guid.NewGuid():N}";
        TestAuthHandler.TestRole = "DistributionUser";
        TestAuthHandler.TestNebulaRoles = ["DistributionUser"];
        TestAuthHandler.TestDisplayName = "Empty Tasks User";
        // Trigger UserProfile creation
        await _client.GetAsync("/my/tasks");

        var response = await _client.GetAsync("/tasks?view=myWork");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TaskListResponseDto>();
        result.Should().NotBeNull();
        result!.Data.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  F0004: GET /users — User Search
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task SearchUsers_ValidQuery_ReturnsMatches()
    {
        // Ensure at least the current user's profile is created by authenticating first
        TestAuthHandler.TestSubject = "test-user-search-001";
        TestAuthHandler.TestRole = "Admin";
        TestAuthHandler.TestDisplayName = "Searchable Admin";
        await _client.GetAsync("/my/tasks"); // trigger UserProfile upsert

        // Search by partial display name — should return at least our test user
        var response = await _client.GetAsync("/users?q=Searchable");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserSearchResponseDto>();
        result.Should().NotBeNull();
        result!.Users.Should().NotBeEmpty();
        result.Users.Should().Contain(u => u.DisplayName.Contains("Searchable", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchUsers_QueryTooShort_Returns400()
    {
        TestAuthHandler.TestSubject = "test-user-001";
        TestAuthHandler.TestRole = "Admin";

        var response = await _client.GetAsync("/users?q=a");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchUsers_ExternalUser_Returns403()
    {
        TestAuthHandler.TestRole = "ExternalUser";
        TestAuthHandler.TestNebulaRoles = [];

        var response = await _client.GetAsync("/users?q=test");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  F0004: Cross-User Assignment
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateTask_ManagerAssignsToOther_Returns201()
    {
        // Set up admin user (001) and create a target assignee (002)
        TestAuthHandler.TestSubject = "test-user-001";
        TestAuthHandler.TestRole = "Admin";
        TestAuthHandler.TestDisplayName = "Admin User";

        var assigneeId = await GetOrCreateUserId("test-user-002", "Assignee User", "DistributionUser");
        var creatorId = await GetCurrentUserId();

        var dto = new TaskCreateRequestDto(
            $"Cross-Assign-{Guid.NewGuid():N}", null, null, null, assigneeId, null, null);

        var response = await _client.PostAsJsonAsync("/tasks", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<TaskDto>();
        result.Should().NotBeNull();
        result!.AssignedToUserId.Should().Be(assigneeId);
        result.CreatedByUserId.Should().Be(creatorId);
        result.AssignedToUserId.Should().NotBe(result.CreatedByUserId);
    }

    [Fact]
    public async Task CreateTask_NonManagerAssignsToOther_Returns403()
    {
        // Use a non-manager role
        TestAuthHandler.TestSubject = "test-user-dist-001";
        TestAuthHandler.TestRole = "DistributionUser";
        TestAuthHandler.TestNebulaRoles = ["DistributionUser"];
        TestAuthHandler.TestDisplayName = "Distribution User";

        var otherUserId = Guid.NewGuid(); // someone else's ID (doesn't need to exist)
        var dto = new TaskCreateRequestDto("Forbidden assign", null, null, null, otherUserId, null, null);

        var response = await _client.PostAsJsonAsync("/tasks", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateTask_ManagerSelfAssign_Returns201()
    {
        TestAuthHandler.TestSubject = "test-user-001";
        TestAuthHandler.TestRole = "Admin";
        TestAuthHandler.TestDisplayName = "Admin Self Assign";

        var selfId = await GetCurrentUserId();
        var dto = new TaskCreateRequestDto($"ManagerSelf-{Guid.NewGuid():N}", null, null, null, selfId, null, null);

        var response = await _client.PostAsJsonAsync("/tasks", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<TaskDto>();
        result!.AssignedToUserId.Should().Be(selfId);
        result.CreatedByUserId.Should().Be(selfId);
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  F0004: Creator-Based Update
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task UpdateTask_CreatorEditsTitle_Returns200()
    {
        // Admin (user-001) creates a task for user-002, then edits its title as creator
        TestAuthHandler.TestSubject = "test-user-001";
        TestAuthHandler.TestRole = "Admin";
        TestAuthHandler.TestDisplayName = "Admin Creator";

        var assigneeId = await GetOrCreateUserId("test-user-002", "Assignee User", "DistributionUser");

        var uniqueTitle = $"CreatorEdit-{Guid.NewGuid():N}";
        var dto = new TaskCreateRequestDto(uniqueTitle, null, null, null, assigneeId, null, null);
        var createResp = await _client.PostAsJsonAsync("/tasks", dto);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResp.Content.ReadFromJsonAsync<TaskDto>();

        // Edit the title as the creator (still user-001)
        var response = await PutJsonAsync($"/tasks/{created!.Id}",
            JsonSerializer.Serialize(new { title = "Creator Updated Title" }));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TaskDto>();
        result!.Title.Should().Be("Creator Updated Title");
    }

    [Fact]
    public async Task UpdateTask_CreatorChangesStatus_Returns403StatusChangeRestricted()
    {
        // Admin (user-001) creates a task for user-002, then attempts to change status as creator
        TestAuthHandler.TestSubject = "test-user-001";
        TestAuthHandler.TestRole = "Admin";
        TestAuthHandler.TestDisplayName = "Admin Status Test";

        var assigneeId = await GetOrCreateUserId("test-user-002", "Assignee User", "DistributionUser");

        var dto = new TaskCreateRequestDto($"StatusChange-{Guid.NewGuid():N}", null, null, null, assigneeId, null, null);
        var createResp = await _client.PostAsJsonAsync("/tasks", dto);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResp.Content.ReadFromJsonAsync<TaskDto>();

        // Attempt status change as creator (not assignee) — should be 403
        var response = await PutJsonAsync($"/tasks/{created!.Id}",
            JsonSerializer.Serialize(new { status = "InProgress" }));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateTask_AssigneeChangesStatus_Returns200()
    {
        // Admin (user-001) creates a task for user-002; user-002 changes status
        TestAuthHandler.TestSubject = "test-user-001";
        TestAuthHandler.TestRole = "Admin";
        TestAuthHandler.TestDisplayName = "Admin Assigner";

        var assigneeId = await GetOrCreateUserId("test-user-002", "Assignee User", "DistributionUser");

        var dto = new TaskCreateRequestDto($"AssigneeStatus-{Guid.NewGuid():N}", null, null, null, assigneeId, null, null);
        var createResp = await _client.PostAsJsonAsync("/tasks", dto);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResp.Content.ReadFromJsonAsync<TaskDto>();

        // Switch to user-002 (the assignee) and change status
        TestAuthHandler.TestSubject = "test-user-002";
        TestAuthHandler.TestRole = "DistributionUser";
        TestAuthHandler.TestNebulaRoles = ["DistributionUser"];
        TestAuthHandler.TestDisplayName = "Assignee User";

        var response = await PutJsonAsync($"/tasks/{created!.Id}",
            JsonSerializer.Serialize(new { status = "InProgress" }));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TaskDto>();
        result!.Status.Should().Be("InProgress");
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  F0004: Creator-Based Delete
    // ═══════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task DeleteTask_Creator_Returns204()
    {
        // Admin (user-001) creates a task for user-002, then deletes it as creator
        TestAuthHandler.TestSubject = "test-user-001";
        TestAuthHandler.TestRole = "Admin";
        TestAuthHandler.TestDisplayName = "Admin Deleter";

        var assigneeId = await GetOrCreateUserId("test-user-002", "Assignee User", "DistributionUser");

        var dto = new TaskCreateRequestDto($"CreatorDelete-{Guid.NewGuid():N}", null, null, null, assigneeId, null, null);
        var createResp = await _client.PostAsJsonAsync("/tasks", dto);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResp.Content.ReadFromJsonAsync<TaskDto>();

        // Delete as creator (still user-001)
        var response = await _client.DeleteAsync($"/tasks/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify it is gone
        var getResp = await _client.GetAsync($"/tasks/{created.Id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  Helpers
    // ═══════════════════════════════════════════════════════════════════════

    private async Task<Guid> GetCurrentUserId()
    {
        // Create a task and read back the assignedToUserId to discover the test user's internal ID
        var tempDto = new TaskCreateRequestDto("temp", null, null, null, Guid.Empty, null, null);

        // We need to figure out the user ID. The HttpCurrentUserService maps (iss, sub) to a UserProfile.
        // For tests, we can use a trick: attempt with a known Guid and see if it works,
        // or just create via the list endpoint.
        // Simpler: read the /my/tasks endpoint which doesn't need a task ID.
        // But we need the user's internal ID for self-assignment.
        // The test auth handler uses subject "test-user-001" with issuer "http://test.local/application/o/nebula/".
        // HttpCurrentUserService does an upsert — the first call creates the UserProfile.
        // We can trigger this by calling any auth'd endpoint, then query the DB.
        // Simplest approach: call GET /my/tasks to trigger UserProfile creation, then use the factory to query.
        await _client.GetAsync("/my/tasks");

        // Now get the user ID from the DB
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Nebula.Infrastructure.Persistence.AppDbContext>();
        const string issuer = "http://test.local/application/o/nebula/";
        var subject = TestAuthHandler.TestSubject;
        var profile = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            db.Set<Nebula.Domain.Entities.UserProfile>()
                .Where(u => u.IdpIssuer == issuer && u.IdpSubject == subject));
        return profile!.Id;
    }

    /// <summary>
    /// Switches to the specified user, triggers UserProfile upsert, queries the internal ID,
    /// then restores the original auth handler state.
    /// </summary>
    private async Task<Guid> GetOrCreateUserId(string subject, string displayName, string role)
    {
        var prevSubject = TestAuthHandler.TestSubject;
        var prevRole = TestAuthHandler.TestRole;
        var prevName = TestAuthHandler.TestDisplayName;
        var prevNebulaRoles = TestAuthHandler.TestNebulaRoles;

        TestAuthHandler.TestSubject = subject;
        TestAuthHandler.TestRole = role;
        TestAuthHandler.TestDisplayName = displayName;
        TestAuthHandler.TestNebulaRoles = [role];

        // Trigger UserProfile upsert
        await _client.GetAsync("/my/tasks");

        TestAuthHandler.TestSubject = prevSubject;
        TestAuthHandler.TestRole = prevRole;
        TestAuthHandler.TestDisplayName = prevName;
        TestAuthHandler.TestNebulaRoles = prevNebulaRoles;

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Nebula.Infrastructure.Persistence.AppDbContext>();
        const string issuer = "http://test.local/application/o/nebula/";
        var profile = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            db.Set<Nebula.Domain.Entities.UserProfile>()
                .Where(u => u.IdpIssuer == issuer && u.IdpSubject == subject));
        return profile!.Id;
    }

    private async Task<Guid> CreateTestTask(string title = "Test Task")
    {
        var task = await CreateTestTaskFull(title);
        return task.Id;
    }

    private async Task<TaskDto> CreateTestTaskFull(string title = "Test Task")
    {
        var userId = await GetCurrentUserId();
        var dto = new TaskCreateRequestDto(title, null, null, null, userId, null, null);
        var response = await _client.PostAsJsonAsync("/tasks", dto);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TaskDto>())!;
    }

    /// <summary>
    /// PUT with If-Match header. Pass rowVersion from the last read/create/update response.
    /// If rowVersion is 0, fetches the current RowVersion from GET /tasks/{id} first.
    /// </summary>
    private async Task<HttpResponseMessage> PutJsonAsync(string url, string jsonBody, uint rowVersion = 0)
    {
        // If caller didn't pass a rowVersion, try to fetch the current one
        if (rowVersion == 0)
        {
            // Extract task ID from URL pattern /tasks/{guid}
            var segments = url.Split('/');
            if (segments.Length >= 3 && Guid.TryParse(segments[^1], out _))
            {
                var getResp = await _client.GetAsync(url);
                if (getResp.IsSuccessStatusCode)
                {
                    var existing = await getResp.Content.ReadFromJsonAsync<TaskDto>();
                    rowVersion = existing!.RowVersion;
                }
            }
        }

        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Put, url) { Content = content };
        request.Headers.TryAddWithoutValidation("If-Match", $"\"{rowVersion}\"");
        return await _client.SendAsync(request);
    }
}
