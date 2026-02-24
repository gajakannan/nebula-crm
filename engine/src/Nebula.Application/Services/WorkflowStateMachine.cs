namespace Nebula.Application.Services;

public static class WorkflowStateMachine
{
    private static readonly Dictionary<string, HashSet<string>> SubmissionTransitions = new()
    {
        ["Received"] = ["Triaging"],
        ["Triaging"] = ["WaitingOnBroker", "ReadyForUWReview"],
        ["WaitingOnBroker"] = ["ReadyForUWReview"],
        ["ReadyForUWReview"] = ["InReview"],
        ["InReview"] = ["Quoted", "Declined"],
        ["Quoted"] = ["BindRequested", "Withdrawn"],
        ["BindRequested"] = ["Bound", "Declined"],
    };

    private static readonly HashSet<string> SubmissionTerminalStates = ["Bound", "Declined", "Withdrawn"];

    private static readonly Dictionary<string, HashSet<string>> RenewalTransitions = new()
    {
        ["Created"] = ["Early"],
        ["Early"] = ["OutreachStarted"],
        ["OutreachStarted"] = ["InReview"],
        ["InReview"] = ["Quoted", "Lost"],
        ["Quoted"] = ["Bound", "Lapsed"],
    };

    private static readonly HashSet<string> RenewalTerminalStates = ["Bound", "Lost", "Lapsed"];

    public static bool IsValidTransition(string workflowType, string from, string to) =>
        workflowType switch
        {
            "Submission" => SubmissionTransitions.TryGetValue(from, out var targets) && targets.Contains(to),
            "Renewal" => RenewalTransitions.TryGetValue(from, out var targets) && targets.Contains(to),
            _ => false
        };

    public static bool IsTerminalState(string workflowType, string state) =>
        workflowType switch
        {
            "Submission" => SubmissionTerminalStates.Contains(state),
            "Renewal" => RenewalTerminalStates.Contains(state),
            _ => false
        };
}
