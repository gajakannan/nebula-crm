namespace Nebula.Domain.Workflow;

public sealed record WorkflowStatusDefinition(
    string Code,
    string DisplayName,
    string Description,
    bool IsTerminal,
    short DisplayOrder,
    string? ColorGroup);

public static class OpportunityStatusCatalog
{
    public static readonly IReadOnlyList<WorkflowStatusDefinition> SubmissionStatuses =
    [
        new("Received", "Received", "Initial state when submission is created", false, 1, "intake"),
        new("Triaging", "Triaging", "Initial triage and data validation", false, 2, "triage"),
        new("WaitingOnBroker", "Waiting on Broker", "Awaiting additional information from broker", false, 3, "waiting"),
        new("WaitingOnDocuments", "Waiting on Documents", "Awaiting required underwriting documents", false, 4, "waiting"),
        new("ReadyForUWReview", "Ready for UW Review", "All data received, queued for underwriter", false, 5, "review"),
        new("InReview", "In Review", "Under active underwriter review", false, 6, "review"),
        new("QuotePreparation", "Quote Preparation", "Preparing quote terms for broker", false, 7, "decision"),
        new("Quoted", "Quoted", "Quote issued, awaiting broker response", false, 8, "decision"),
        new("RequoteRequested", "Requote Requested", "Broker requested revised quote terms", false, 9, "decision"),
        new("BindRequested", "Bind Requested", "Broker accepted quote, bind in progress", false, 10, "decision"),
        new("Binding", "Binding", "Binding and issuance processing in progress", false, 11, "decision"),
        new("Bound", "Bound", "Policy bound and issued", true, 12, null),
        new("Declined", "Declined", "Submission declined by underwriter", true, 13, null),
        new("Withdrawn", "Withdrawn", "Broker or insured withdrew submission", true, 14, null),
        new("NotQuoted", "Not Quoted", "Submission closed without quote issued", true, 15, null),
        new("Lost", "Lost", "Opportunity lost to another market or strategy change", true, 16, null),
        new("Expired", "Expired", "Submission expired before disposition completed", true, 17, null),
    ];

    public static readonly IReadOnlyList<WorkflowStatusDefinition> RenewalStatuses =
    [
        new("Created", "Created", "Renewal record created from expiring policy", false, 1, "intake"),
        new("Early", "Early", "In early renewal window (90-120 days out)", false, 2, "intake"),
        new("DataReview", "Data Review", "Coverage and account data review before outreach", false, 3, "triage"),
        new("OutreachStarted", "Outreach Started", "Active broker/account outreach begun", false, 4, "waiting"),
        new("WaitingOnBroker", "Waiting on Broker", "Awaiting broker response or required renewal information", false, 5, "waiting"),
        new("InReview", "In Review", "Under underwriter review for renewal terms", false, 6, "review"),
        new("Quoted", "Quoted", "Renewal quote issued", false, 7, "decision"),
        new("Negotiation", "Negotiation", "Actively negotiating renewal terms", false, 8, "decision"),
        new("BindRequested", "Bind Requested", "Renewal bind requested", false, 9, "decision"),
        new("Bound", "Bound", "Policy renewed and bound", true, 10, null),
        new("NotRenewed", "Not Renewed", "Renewal closed without binding", true, 11, null),
        new("Lost", "Lost", "Lost to competitor", true, 12, null),
        new("Lapsed", "Lapsed", "Policy expired without renewal", true, 13, null),
        new("Withdrawn", "Withdrawn", "Renewal withdrawn by broker or insured", true, 14, null),
        new("Expired", "Expired", "Renewal workflow expired before completion", true, 15, null),
    ];

    public static readonly IReadOnlySet<string> SubmissionTerminalStatusCodes = SubmissionStatuses
        .Where(s => s.IsTerminal)
        .Select(s => s.Code)
        .ToHashSet(StringComparer.Ordinal);

    public static readonly IReadOnlySet<string> RenewalTerminalStatusCodes = RenewalStatuses
        .Where(s => s.IsTerminal)
        .Select(s => s.Code)
        .ToHashSet(StringComparer.Ordinal);
}
