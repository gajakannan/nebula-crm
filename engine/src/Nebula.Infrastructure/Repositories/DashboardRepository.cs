using Microsoft.EntityFrameworkCore;
using Nebula.Application.DTOs;
using Nebula.Application.Interfaces;
using Nebula.Infrastructure.Persistence;

namespace Nebula.Infrastructure.Repositories;

public class DashboardRepository(AppDbContext db) : IDashboardRepository
{
    private static readonly string[] TerminalSubmissionStatuses = ["Bound", "Declined", "Withdrawn"];
    private static readonly string[] TerminalRenewalStatuses = ["Bound", "Lost", "Lapsed"];

    public async Task<DashboardKpisDto> GetKpisAsync(CancellationToken ct = default)
    {
        var activeBrokers = await db.Brokers.CountAsync(b => b.Status == "Active", ct);
        var openSubmissions = await db.Submissions
            .CountAsync(s => !TerminalSubmissionStatuses.Contains(s.CurrentStatus), ct);

        var ninetyDaysAgo = DateTime.UtcNow.AddDays(-90);

        // Renewal rate: % of renewals reaching Bound out of all that exited pipeline in 90 days
        var exitedRenewals = await db.Renewals
            .Where(r => TerminalRenewalStatuses.Contains(r.CurrentStatus) && r.UpdatedAt >= ninetyDaysAgo)
            .ToListAsync(ct);

        double? renewalRate = exitedRenewals.Count > 0
            ? Math.Round(exitedRenewals.Count(r => r.CurrentStatus == "Bound") * 100.0 / exitedRenewals.Count, 1)
            : null;

        // Avg turnaround: mean days from Submission.CreatedAt to first terminal transition
        var terminalTransitions = await db.WorkflowTransitions
            .Where(wt => wt.WorkflowType == "Submission" && TerminalSubmissionStatuses.Contains(wt.ToState)
                && wt.OccurredAt >= ninetyDaysAgo)
            .GroupBy(wt => wt.EntityId)
            .Select(g => new { EntityId = g.Key, FirstTerminal = g.Min(wt => wt.OccurredAt) })
            .ToListAsync(ct);

        double? avgTurnaroundDays = null;
        if (terminalTransitions.Count > 0)
        {
            var submissionIds = terminalTransitions.Select(t => t.EntityId).ToList();
            var submissions = await db.Submissions.IgnoreQueryFilters()
                .Where(s => submissionIds.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, s => s.CreatedAt, ct);

            var turnarounds = terminalTransitions
                .Where(t => submissions.ContainsKey(t.EntityId))
                .Select(t => (t.FirstTerminal - submissions[t.EntityId]).TotalDays)
                .ToList();

            if (turnarounds.Count > 0)
                avgTurnaroundDays = Math.Round(turnarounds.Average(), 1);
        }

        return new DashboardKpisDto(activeBrokers, openSubmissions, renewalRate, avgTurnaroundDays);
    }

    public async Task<DashboardPipelineDto> GetPipelineAsync(CancellationToken ct = default)
    {
        var submissionStatuses = await db.ReferenceSubmissionStatuses
            .Where(s => !s.IsTerminal)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(ct);

        var submissionCounts = await db.Submissions
            .Where(s => !TerminalSubmissionStatuses.Contains(s.CurrentStatus))
            .GroupBy(s => s.CurrentStatus)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Status, g => g.Count, ct);

        var submissionPipeline = submissionStatuses.Select(s =>
            new PipelineStatusCountDto(s.Code, submissionCounts.GetValueOrDefault(s.Code, 0), s.ColorGroup ?? "intake"))
            .ToList();

        var renewalStatuses = await db.ReferenceRenewalStatuses
            .Where(s => !s.IsTerminal)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(ct);

        var renewalCounts = await db.Renewals
            .Where(r => !TerminalRenewalStatuses.Contains(r.CurrentStatus))
            .GroupBy(r => r.CurrentStatus)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Status, g => g.Count, ct);

        var renewalPipeline = renewalStatuses.Select(s =>
            new PipelineStatusCountDto(s.Code, renewalCounts.GetValueOrDefault(s.Code, 0), s.ColorGroup ?? "intake"))
            .ToList();

        return new DashboardPipelineDto(submissionPipeline, renewalPipeline);
    }

    public async Task<PipelineItemsDto> GetPipelineItemsAsync(
        string entityType, string status, CancellationToken ct = default)
    {
        if (entityType == "submission")
        {
            var query = db.Submissions
                .Include(s => s.Account)
                .Where(s => s.CurrentStatus == status);

            var totalCount = await query.CountAsync(ct);

            var lastTransitions = await db.WorkflowTransitions
                .Where(wt => wt.WorkflowType == "Submission" && wt.ToState == status)
                .GroupBy(wt => wt.EntityId)
                .Select(g => new { EntityId = g.Key, LastTransition = g.Max(wt => wt.OccurredAt) })
                .ToDictionaryAsync(g => g.EntityId, g => g.LastTransition, ct);

            var items = await query.Take(5).ToListAsync(ct);

            var userSubjects = items.Select(s => s.AssignedTo).Distinct().ToList();
            var users = await db.UserProfiles
                .Where(u => userSubjects.Contains(u.Subject))
                .ToDictionaryAsync(u => u.Subject, ct);

            var miniCards = items.Select(s =>
            {
                var daysInStatus = lastTransitions.TryGetValue(s.Id, out var transitionDate)
                    ? (int)(DateTime.UtcNow - transitionDate).TotalDays
                    : (int)(DateTime.UtcNow - s.CreatedAt).TotalDays;

                users.TryGetValue(s.AssignedTo, out var user);
                var initials = user?.DisplayName is { } dn ? string.Concat(dn.Split(' ').Select(w => w[..1])).ToUpper()[..Math.Min(2, string.Concat(dn.Split(' ').Select(w => w[..1])).Length)] : null;

                return new PipelineMiniCardDto(s.Id, s.Account.Name, (double)s.PremiumEstimate, daysInStatus, initials, user?.DisplayName);
            }).OrderByDescending(c => c.DaysInStatus).ToList();

            return new PipelineItemsDto(miniCards, totalCount);
        }
        else // renewal
        {
            var query = db.Renewals
                .Include(r => r.Account)
                .Where(r => r.CurrentStatus == status);

            var totalCount = await query.CountAsync(ct);

            var lastTransitions = await db.WorkflowTransitions
                .Where(wt => wt.WorkflowType == "Renewal" && wt.ToState == status)
                .GroupBy(wt => wt.EntityId)
                .Select(g => new { EntityId = g.Key, LastTransition = g.Max(wt => wt.OccurredAt) })
                .ToDictionaryAsync(g => g.EntityId, g => g.LastTransition, ct);

            var items = await query.Take(5).ToListAsync(ct);

            var userSubjects = items.Select(r => r.AssignedTo).Distinct().ToList();
            var users = await db.UserProfiles
                .Where(u => userSubjects.Contains(u.Subject))
                .ToDictionaryAsync(u => u.Subject, ct);

            var miniCards = items.Select(r =>
            {
                var daysInStatus = lastTransitions.TryGetValue(r.Id, out var transitionDate)
                    ? (int)(DateTime.UtcNow - transitionDate).TotalDays
                    : (int)(DateTime.UtcNow - r.CreatedAt).TotalDays;

                users.TryGetValue(r.AssignedTo, out var user);
                var initials = user?.DisplayName is { } dn ? string.Concat(dn.Split(' ').Select(w => w[..1])).ToUpper()[..Math.Min(2, string.Concat(dn.Split(' ').Select(w => w[..1])).Length)] : null;

                return new PipelineMiniCardDto(r.Id, r.Account.Name, null, daysInStatus, initials, user?.DisplayName);
            }).OrderByDescending(c => c.DaysInStatus).ToList();

            return new PipelineItemsDto(miniCards, totalCount);
        }
    }

    public async Task<IReadOnlyList<NudgeCardDto>> GetNudgesAsync(string userSubject, CancellationToken ct = default)
    {
        var nudges = new List<NudgeCardDto>();
        var today = DateTime.UtcNow.Date;

        // Priority 1: Overdue tasks
        var overdueTasks = await db.Tasks
            .Where(t => t.AssignedTo == userSubject && t.Status != "Done"
                && t.DueDate.HasValue && t.DueDate.Value < today)
            .OrderBy(t => t.DueDate)
            .Take(3)
            .ToListAsync(ct);

        foreach (var task in overdueTasks)
        {
            var daysOverdue = (int)(today - task.DueDate!.Value).TotalDays;
            nudges.Add(new NudgeCardDto(
                "OverdueTask", task.Title,
                $"{daysOverdue} day{(daysOverdue != 1 ? "s" : "")} overdue",
                task.LinkedEntityType ?? "Task", task.LinkedEntityId ?? task.Id,
                task.Title, daysOverdue, "Review Now"));
        }

        if (nudges.Count >= 3) return nudges.Take(3).ToList();

        // Priority 2: Stale submissions (>5 days in current status)
        var fiveDaysAgo = DateTime.UtcNow.AddDays(-5);
        var staleSubmissions = await db.Submissions
            .Include(s => s.Account)
            .Where(s => !TerminalSubmissionStatuses.Contains(s.CurrentStatus) && s.UpdatedAt < fiveDaysAgo)
            .OrderBy(s => s.UpdatedAt)
            .Take(3 - nudges.Count)
            .ToListAsync(ct);

        foreach (var sub in staleSubmissions)
        {
            var daysStuck = (int)(DateTime.UtcNow - sub.UpdatedAt).TotalDays;
            nudges.Add(new NudgeCardDto(
                "StaleSubmission", $"Follow up on {sub.Account.Name}",
                $"{daysStuck} days in {sub.CurrentStatus}",
                "Submission", sub.Id, sub.Account.Name, daysStuck, "Take Action"));
        }

        if (nudges.Count >= 3) return nudges.Take(3).ToList();

        // Priority 3: Upcoming renewals (within 14 days)
        var fourteenDaysFromNow = today.AddDays(14);
        var upcomingRenewals = await db.Renewals
            .Include(r => r.Account)
            .Where(r => !TerminalRenewalStatuses.Contains(r.CurrentStatus)
                && r.RenewalDate >= today && r.RenewalDate <= fourteenDaysFromNow)
            .OrderBy(r => r.RenewalDate)
            .Take(3 - nudges.Count)
            .ToListAsync(ct);

        foreach (var ren in upcomingRenewals)
        {
            var daysUntil = (int)(ren.RenewalDate - today).TotalDays;
            nudges.Add(new NudgeCardDto(
                "UpcomingRenewal", $"Renewal for {ren.Account.Name}",
                $"Due in {daysUntil} day{(daysUntil != 1 ? "s" : "")}",
                "Renewal", ren.Id, ren.Account.Name, daysUntil, "Start Outreach"));
        }

        return nudges.Take(3).ToList();
    }
}
