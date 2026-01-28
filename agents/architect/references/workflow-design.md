# Workflow Design Guide

Guide for designing state machine workflows for BrokerHub.

## State Machine Fundamentals

### Components
- **States:** Discrete stages (Received, Triaging, InReview, Bound)
- **Transitions:** Allowed state changes
- **Events:** Triggers for transitions
- **Guards:** Conditions that must be true
- **Actions:** Side effects of transitions

## Design Checklist

- [ ] Define all states (initial, active, terminal)
- [ ] Define allowed transitions
- [ ] Define blocked transitions
- [ ] Specify prerequisites for each transition
- [ ] Define authorization rules
- [ ] Specify side effects (emails, notifications)
- [ ] Define error responses

## Implementation Pattern

```csharp
public async Task TransitionTo(SubmissionStatus newStatus, User user)
{
    // Validate transition is allowed
    if (!IsTransitionAllowed(Status, newStatus))
        throw new InvalidTransitionException();
    
    // Check prerequisites
    ValidatePrerequisites(newStatus);
    
    // Check authorization
    await _authz.CheckCanTransition(user, this, newStatus);
    
    // Perform transition
    var oldStatus = Status;
    Status = newStatus;
    
    // Log transition
    await _workflow.LogTransition(Id, oldStatus, newStatus, user.Id);
    
    // Side effects
    await ExecuteSideEffects(newStatus);
}
```

See `agents/templates/workflow-spec-template.md` for complete workflow specification format.
