using Nebula.Domain.Workflow;

namespace Nebula.Application.Services;

public static class WorkflowStateMachine
{
    private static readonly IReadOnlySet<string> SubmissionTerminalStates = OpportunityStatusCatalog.SubmissionTerminalStatusCodes;
    private static readonly IReadOnlySet<string> RenewalTerminalStates = OpportunityStatusCatalog.RenewalTerminalStatusCodes;

    private static readonly Dictionary<string, HashSet<string>> SubmissionTransitions = new()
    {
        ["Received"] = ["Triaging"],
        ["Triaging"] = ["WaitingOnBroker", "WaitingOnDocuments", "ReadyForUWReview"],
        ["WaitingOnBroker"] = ["Triaging", "WaitingOnDocuments", "ReadyForUWReview"],
        ["WaitingOnDocuments"] = ["WaitingOnBroker", "ReadyForUWReview"],
        ["ReadyForUWReview"] = ["InReview", "QuotePreparation"],
        ["InReview"] = ["WaitingOnBroker", "WaitingOnDocuments", "QuotePreparation", "Quoted", "RequoteRequested"],
        ["QuotePreparation"] = ["Quoted", "RequoteRequested"],
        ["Quoted"] = ["RequoteRequested", "BindRequested"],
        ["RequoteRequested"] = ["InReview", "QuotePreparation", "Quoted"],
        ["BindRequested"] = ["Binding", "Bound"],
        ["Binding"] = ["Bound"],
    };

    private static readonly Dictionary<string, HashSet<string>> RenewalTransitions = new()
    {
        ["Created"] = ["Early", "DataReview"],
        ["Early"] = ["DataReview", "OutreachStarted"],
        ["DataReview"] = ["OutreachStarted", "WaitingOnBroker", "InReview"],
        ["OutreachStarted"] = ["WaitingOnBroker", "InReview", "Quoted"],
        ["WaitingOnBroker"] = ["DataReview", "InReview", "Quoted"],
        ["InReview"] = ["Quoted", "Negotiation"],
        ["Quoted"] = ["Negotiation", "BindRequested", "Bound"],
        ["Negotiation"] = ["Quoted", "BindRequested", "Bound"],
        ["BindRequested"] = ["Bound"],
    };

    public static bool IsValidTransition(string workflowType, string from, string to) =>
        workflowType switch
        {
            "Submission" => IsValidTransition(SubmissionTransitions, SubmissionTerminalStates, from, to),
            "Renewal" => IsValidTransition(RenewalTransitions, RenewalTerminalStates, from, to),
            _ => false,
        };

    public static bool IsTerminalState(string workflowType, string state) =>
        workflowType switch
        {
            "Submission" => SubmissionTerminalStates.Contains(state),
            "Renewal" => RenewalTerminalStates.Contains(state),
            _ => false,
        };

    private static bool IsValidTransition(
        IReadOnlyDictionary<string, HashSet<string>> transitions,
        IReadOnlySet<string> terminalStates,
        string from,
        string to)
    {
        if (terminalStates.Contains(from)) return false;
        if (terminalStates.Contains(to)) return true;

        return transitions.TryGetValue(from, out var targets) && targets.Contains(to);
    }
}
