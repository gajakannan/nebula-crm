using Nebula.Application.Interfaces;

namespace Nebula.Infrastructure.Authorization;

/// <summary>
/// Implements ABAC authorization matching the Casbin model.conf and policy.csv semantics.
/// Evaluates: role == policy.sub AND resourceType == policy.obj AND action == policy.act AND eval(policy.cond)
/// </summary>
public class PolicyAuthorizationService : IAuthorizationService
{
    private static readonly List<PolicyRule> Rules = LoadPolicies();

    public Task<bool> AuthorizeAsync(
        string userRole, string resourceType, string action,
        IDictionary<string, object>? resourceAttributes = null)
    {
        foreach (var rule in Rules)
        {
            if (rule.Role == userRole && rule.Resource == resourceType && rule.Action == action)
            {
                if (EvaluateCondition(rule.Condition, resourceAttributes))
                    return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }

    private static bool EvaluateCondition(string condition, IDictionary<string, object>? attrs)
    {
        if (condition == "true") return true;

        // r.obj.assignee == r.sub.id — task ownership check
        if (condition == "r.obj.assignee == r.sub.id")
        {
            if (attrs is null) return false;
            return attrs.TryGetValue("assignee", out var assignee)
                && attrs.TryGetValue("subjectId", out var subjectId)
                && string.Equals(assignee?.ToString(), subjectId?.ToString(), StringComparison.Ordinal);
        }

        return false;
    }

    private static List<PolicyRule> LoadPolicies() =>
    [
        // Broker policies
        new("DistributionUser", "broker", "create"),
        new("DistributionUser", "broker", "read"),
        new("DistributionUser", "broker", "search"),
        new("DistributionUser", "broker", "update"),
        new("DistributionUser", "broker", "delete"),
        new("DistributionManager", "broker", "create"),
        new("DistributionManager", "broker", "read"),
        new("DistributionManager", "broker", "search"),
        new("DistributionManager", "broker", "update"),
        new("DistributionManager", "broker", "delete"),
        new("Underwriter", "broker", "read"),
        new("RelationshipManager", "broker", "create"),
        new("RelationshipManager", "broker", "read"),
        new("RelationshipManager", "broker", "search"),
        new("RelationshipManager", "broker", "update"),
        new("ProgramManager", "broker", "read"),
        new("Admin", "broker", "create"),
        new("Admin", "broker", "read"),
        new("Admin", "broker", "search"),
        new("Admin", "broker", "update"),
        new("Admin", "broker", "delete"),

        // Contact policies
        new("DistributionUser", "contact", "create"),
        new("DistributionUser", "contact", "read"),
        new("DistributionUser", "contact", "update"),
        new("DistributionManager", "contact", "create"),
        new("DistributionManager", "contact", "read"),
        new("DistributionManager", "contact", "update"),
        new("DistributionManager", "contact", "delete"),
        new("Underwriter", "contact", "read"),
        new("RelationshipManager", "contact", "create"),
        new("RelationshipManager", "contact", "read"),
        new("RelationshipManager", "contact", "update"),
        new("ProgramManager", "contact", "read"),
        new("Admin", "contact", "create"),
        new("Admin", "contact", "read"),
        new("Admin", "contact", "update"),
        new("Admin", "contact", "delete"),

        // Submission policies
        new("DistributionUser", "submission", "read"),
        new("DistributionUser", "submission", "transition"),
        new("DistributionManager", "submission", "read"),
        new("DistributionManager", "submission", "transition"),
        new("Underwriter", "submission", "read"),
        new("Underwriter", "submission", "transition"),
        new("RelationshipManager", "submission", "read"),
        new("ProgramManager", "submission", "read"),
        new("Admin", "submission", "read"),
        new("Admin", "submission", "transition"),

        // Renewal policies
        new("DistributionUser", "renewal", "read"),
        new("DistributionUser", "renewal", "transition"),
        new("DistributionManager", "renewal", "read"),
        new("DistributionManager", "renewal", "transition"),
        new("Underwriter", "renewal", "read"),
        new("Underwriter", "renewal", "transition"),
        new("RelationshipManager", "renewal", "read"),
        new("ProgramManager", "renewal", "read"),
        new("Admin", "renewal", "read"),
        new("Admin", "renewal", "transition"),

        // Dashboard policies (all internal roles can read)
        new("DistributionUser", "dashboard_kpi", "read"),
        new("DistributionManager", "dashboard_kpi", "read"),
        new("Underwriter", "dashboard_kpi", "read"),
        new("RelationshipManager", "dashboard_kpi", "read"),
        new("ProgramManager", "dashboard_kpi", "read"),
        new("Admin", "dashboard_kpi", "read"),
        new("DistributionUser", "dashboard_pipeline", "read"),
        new("DistributionManager", "dashboard_pipeline", "read"),
        new("Underwriter", "dashboard_pipeline", "read"),
        new("RelationshipManager", "dashboard_pipeline", "read"),
        new("ProgramManager", "dashboard_pipeline", "read"),
        new("Admin", "dashboard_pipeline", "read"),
        new("DistributionUser", "dashboard_nudge", "read"),
        new("DistributionManager", "dashboard_nudge", "read"),
        new("Underwriter", "dashboard_nudge", "read"),
        new("RelationshipManager", "dashboard_nudge", "read"),
        new("ProgramManager", "dashboard_nudge", "read"),
        new("Admin", "dashboard_nudge", "read"),

        // Task policies (ownership-based)
        new("DistributionUser", "task", "create", "r.obj.assignee == r.sub.id"),
        new("DistributionUser", "task", "read", "r.obj.assignee == r.sub.id"),
        new("DistributionUser", "task", "update", "r.obj.assignee == r.sub.id"),
        new("DistributionUser", "task", "delete", "r.obj.assignee == r.sub.id"),
        new("DistributionManager", "task", "create", "r.obj.assignee == r.sub.id"),
        new("DistributionManager", "task", "read", "r.obj.assignee == r.sub.id"),
        new("DistributionManager", "task", "update", "r.obj.assignee == r.sub.id"),
        new("DistributionManager", "task", "delete", "r.obj.assignee == r.sub.id"),
        new("Underwriter", "task", "create", "r.obj.assignee == r.sub.id"),
        new("Underwriter", "task", "read", "r.obj.assignee == r.sub.id"),
        new("Underwriter", "task", "update", "r.obj.assignee == r.sub.id"),
        new("Underwriter", "task", "delete", "r.obj.assignee == r.sub.id"),
        new("RelationshipManager", "task", "create", "r.obj.assignee == r.sub.id"),
        new("RelationshipManager", "task", "read", "r.obj.assignee == r.sub.id"),
        new("RelationshipManager", "task", "update", "r.obj.assignee == r.sub.id"),
        new("RelationshipManager", "task", "delete", "r.obj.assignee == r.sub.id"),
        new("ProgramManager", "task", "create", "r.obj.assignee == r.sub.id"),
        new("ProgramManager", "task", "read", "r.obj.assignee == r.sub.id"),
        new("ProgramManager", "task", "update", "r.obj.assignee == r.sub.id"),
        new("ProgramManager", "task", "delete", "r.obj.assignee == r.sub.id"),
        new("Admin", "task", "create", "r.obj.assignee == r.sub.id"),
        new("Admin", "task", "read", "r.obj.assignee == r.sub.id"),
        new("Admin", "task", "update", "r.obj.assignee == r.sub.id"),
        new("Admin", "task", "delete", "r.obj.assignee == r.sub.id"),

        // Timeline event policies
        new("DistributionUser", "timeline_event", "read"),
        new("DistributionManager", "timeline_event", "read"),
        new("Underwriter", "timeline_event", "read"),
        new("RelationshipManager", "timeline_event", "read"),
        new("ProgramManager", "timeline_event", "read"),
        new("Admin", "timeline_event", "read"),
    ];

    private record PolicyRule(string Role, string Resource, string Action, string Condition = "true");
}
