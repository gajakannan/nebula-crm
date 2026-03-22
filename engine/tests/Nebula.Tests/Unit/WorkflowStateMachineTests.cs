using FluentAssertions;
using Nebula.Application.Services;

namespace Nebula.Tests.Unit;

public class WorkflowStateMachineTests
{
    [Theory]
    [InlineData("Submission", "Received", "Triaging")]
    [InlineData("Submission", "Quoted", "Bound")]
    [InlineData("Renewal", "Created", "DataReview")]
    [InlineData("Renewal", "Quoted", "Bound")]
    public void IsValidTransition_WithAllowedTransition_ReturnsTrue(string workflowType, string from, string to)
    {
        WorkflowStateMachine.IsValidTransition(workflowType, from, to).Should().BeTrue();
    }

    [Theory]
    [InlineData("Submission", "Bound", "Quoted")]
    [InlineData("Submission", "Received", "WaitingOnBroker")]
    [InlineData("Renewal", "Bound", "Quoted")]
    [InlineData("Renewal", "Created", "Negotiation")]
    [InlineData("Unknown", "Open", "Closed")]
    public void IsValidTransition_WithDisallowedTransition_ReturnsFalse(string workflowType, string from, string to)
    {
        WorkflowStateMachine.IsValidTransition(workflowType, from, to).Should().BeFalse();
    }

    [Theory]
    [InlineData("Submission", "Bound", true)]
    [InlineData("Submission", "Received", false)]
    [InlineData("Renewal", "Bound", true)]
    [InlineData("Renewal", "Created", false)]
    [InlineData("Unknown", "Anything", false)]
    public void IsTerminalState_ReturnsExpectedValue(string workflowType, string state, bool expected)
    {
        WorkflowStateMachine.IsTerminalState(workflowType, state).Should().Be(expected);
    }
}
