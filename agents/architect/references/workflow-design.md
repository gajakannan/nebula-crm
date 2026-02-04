# Workflow Design Guide

Comprehensive guide for designing workflows and state machines for the Nebula insurance CRM. This guide covers workflow fundamentals, Temporal integration, submission workflow patterns, renewal workflows, and event sourcing.

---

## 1. Workflow Fundamentals

### 1.1 State Machine Components

**State Machine** is a model of computation with defined states and transitions.

**Components:**
- **States**: Discrete stages in a workflow (Received, Triaging, InReview, Quoted, Bound)
- **Transitions**: Allowed state changes (Triaging → ReadyForUWReview)
- **Events**: Triggers for transitions (user action, time-based, external system)
- **Guards**: Conditions that must be true for transition to occur (required fields populated)
- **Actions**: Side effects executed on transition (send email, log event)

---

### 1.2 State Types

**Initial State:**
- Entry point of workflow (e.g., Received for submissions)
- Created automatically when entity created

**Active States:**
- Work-in-progress states (Triaging, InReview, Quoted)
- Can transition to other states

**Terminal States:**
- End states, no further transitions allowed (Bound, Declined, Withdrawn)
- Workflow complete

**Example:**
```
Received (initial)
    ↓
Triaging (active)
    ↓
ReadyForUWReview (active)
    ↓
InReview (active)
    ↓
Quoted (active)
    ↓
Bound (terminal)
```

---

### 1.3 Transition Rules

**Allowed Transitions:** Define valid state changes

**Blocked Transitions:** Invalid state changes (return 409 Conflict)

**Example:**
```csharp
private static readonly Dictionary<string, List<string>> AllowedTransitions = new()
{
    ["Triaging"] = new() { "WaitingOnBroker", "ReadyForUWReview", "Declined", "Withdrawn" },
    ["ReadyForUWReview"] = new() { "InReview", "WaitingOnBroker", "Declined", "Withdrawn" },
    ["InReview"] = new() { "Quoted", "WaitingOnBroker", "Declined", "Withdrawn" },
    ["Quoted"] = new() { "BindRequested", "Declined", "Withdrawn" },
    ["BindRequested"] = new() { "Bound", "Declined" },
    ["Bound"] = new(), // Terminal, no transitions
    ["Declined"] = new(), // Terminal
    ["Withdrawn"] = new() // Terminal
};

public static bool IsValidTransition(string fromStatus, string toStatus)
{
    return AllowedTransitions.TryGetValue(fromStatus, out var allowed) && allowed.Contains(toStatus);
}
```

---

### 1.4 Workflow Events (Triggers)

**User-Initiated Events:**
- User clicks "Submit for Review" button → transition to ReadyForUWReview
- Underwriter clicks "Generate Quote" → transition to Quoted

**Time-Based Events:**
- 120 days before policy expiration → trigger renewal reminder
- 30 days before renewal due → escalate to manager

**System Events:**
- Payment confirmed → transition to Bound
- External rating API returns quote → transition to Quoted

---

### 1.5 Compensation (Rollback)

**Compensation** is the process of undoing a workflow step when a later step fails.

**Example: Bind Request Fails**
- Transition to BindRequested
- Call payment API → fails
- Compensate: Transition back to Quoted
- Log compensation event

**Saga Pattern:** Sequence of local transactions with compensating transactions for rollback.

---

### 1.6 Idempotency

**Idempotent Workflows** can be safely retried without side effects.

**Design Principle:**
- Store workflow state (current status)
- If transition request received for current state → no-op (already transitioned)
- If transition request for different state → validate and execute

**Example:**
```csharp
public async Task TransitionAsync(Guid submissionId, string toStatus)
{
    var submission = await _context.Submissions.FindAsync(submissionId);

    // Idempotency check
    if (submission.Status == toStatus)
        return; // Already in target state, no-op

    // Validate and execute transition
    if (!IsValidTransition(submission.Status, toStatus))
        throw new InvalidTransitionException();

    submission.Status = toStatus;
    await _context.SaveChangesAsync();
}
```

---

## 2. Temporal Integration

### 2.1 What is Temporal?

**Temporal** is a durable workflow orchestration engine.

**Use Cases:**
- Long-running workflows (renewal reminders spanning months)
- Scheduled tasks (send reminder 120 days before expiration)
- Reliable retries (retry failed external API calls)
- Workflow versioning (change workflow logic without breaking in-flight workflows)

---

### 2.2 Temporal Workflow Definitions (C# SDK)

**Workflow Definition:**

```csharp
using Temporalio.Workflows;

[Workflow]
public class RenewalReminderWorkflow
{
    [WorkflowRun]
    public async Task RunAsync(RenewalWorkflowInput input)
    {
        var policy = input.Policy;

        // Schedule reminder 120 days before expiration
        var delay120 = policy.ExpirationDate.AddDays(-120) - DateTime.UtcNow;
        if (delay120 > TimeSpan.Zero)
        {
            await Workflow.DelayAsync(delay120);
            await Workflow.ExecuteActivityAsync<SendRenewalReminderActivity>(
                new ReminderActivityInput { PolicyId = policy.Id, DaysOut = 120 });
        }

        // Schedule reminder 90 days before expiration
        await Workflow.DelayAsync(TimeSpan.FromDays(30));
        await Workflow.ExecuteActivityAsync<SendRenewalReminderActivity>(
            new ReminderActivityInput { PolicyId = policy.Id, DaysOut = 90 });

        // Schedule reminder 60 days before expiration
        await Workflow.DelayAsync(TimeSpan.FromDays(30));
        await Workflow.ExecuteActivityAsync<SendRenewalReminderActivity>(
            new ReminderActivityInput { PolicyId = policy.Id, DaysOut = 60 });

        // Check if renewal submission created
        var renewalCreated = await Workflow.ExecuteActivityAsync<CheckRenewalSubmissionActivity>(
            policy.Id);

        if (!renewalCreated)
        {
            // Escalate to manager if no renewal 30 days before expiration
            await Workflow.DelayAsync(TimeSpan.FromDays(30));
            await Workflow.ExecuteActivityAsync<EscalateRenewalActivity>(policy.Id);
        }
    }
}

// Workflow input
public record RenewalWorkflowInput(Guid PolicyId, Policy Policy);
```

---

### 2.3 Temporal Activities (Side Effects)

**Activities** perform side effects (send email, call API, write to database).

**Activity Definition:**

```csharp
using Temporalio.Activities;

[Activity]
public class SendRenewalReminderActivity
{
    private readonly IEmailService _emailService;

    [ActivityMethod]
    public async Task ExecuteAsync(ReminderActivityInput input)
    {
        var policy = await _policyRepository.GetByIdAsync(input.PolicyId);

        await _emailService.SendRenewalReminderAsync(
            to: policy.BrokerEmail,
            subject: $"Renewal Reminder - {input.DaysOut} Days",
            body: $"Your policy {policy.PolicyNumber} expires in {input.DaysOut} days.");
    }
}
```

**Activities are:**
- Retryable (automatic retry on failure)
- Timeout-enforced (fail if activity takes too long)
- Logged (execution history tracked)

---

### 2.4 Temporal Signals (External Events)

**Signals** send events to running workflows from external systems.

**Example: Renewal Submitted Signal**

```csharp
[Workflow]
public class RenewalReminderWorkflow
{
    private bool _renewalSubmitted = false;

    [WorkflowRun]
    public async Task RunAsync(RenewalWorkflowInput input)
    {
        // ... schedule reminders

        // Wait for renewal submission or expiration date
        await Workflow.WaitConditionAsync(() => _renewalSubmitted || DateTime.UtcNow >= input.Policy.ExpirationDate);

        if (_renewalSubmitted)
        {
            // Success, workflow complete
            return;
        }
        else
        {
            // Expiration reached, escalate
            await Workflow.ExecuteActivityAsync<EscalateRenewalActivity>(input.PolicyId);
        }
    }

    [WorkflowSignal]
    public async Task RenewalSubmittedAsync()
    {
        _renewalSubmitted = true;
    }
}

// External system signals workflow
await _temporalClient.SignalWorkflowAsync<RenewalReminderWorkflow>(
    workflowId: $"renewal-{policyId}",
    signal: wf => wf.RenewalSubmittedAsync());
```

---

### 2.5 Temporal Queries (Workflow Status)

**Queries** read workflow state without mutating it.

```csharp
[Workflow]
public class RenewalReminderWorkflow
{
    private int _remindersSent = 0;

    [WorkflowQuery]
    public int GetRemindersSent() => _remindersSent;

    [WorkflowRun]
    public async Task RunAsync(RenewalWorkflowInput input)
    {
        await Workflow.ExecuteActivityAsync<SendRenewalReminderActivity>(...);
        _remindersSent++;
    }
}

// Query workflow
var remindersSent = await _temporalClient.QueryWorkflowAsync<RenewalReminderWorkflow, int>(
    workflowId: $"renewal-{policyId}",
    query: wf => wf.GetRemindersSent());
```

---

### 2.6 Temporal Best Practices

**Determinism:**
- Workflow code must be deterministic (same input → same output)
- No random numbers, no DateTime.Now (use Workflow.UtcNow)
- No direct I/O (use activities for I/O)

**Versioning:**
- Use workflow versioning to change logic without breaking in-flight workflows

```csharp
[Workflow]
public class RenewalReminderWorkflow
{
    [WorkflowRun]
    public async Task RunAsync(RenewalWorkflowInput input)
    {
        var version = Workflow.GetVersion("add-60-day-reminder", Workflow.DefaultVersion, 2);

        if (version >= 2)
        {
            // New logic: Send 60-day reminder
            await Workflow.DelayAsync(TimeSpan.FromDays(60));
            await Workflow.ExecuteActivityAsync<SendRenewalReminderActivity>(...);
        }
    }
}
```

**Child Workflows:**
- Break complex workflows into smaller child workflows
- Each child workflow can be retried independently

---

## 3. Submission Workflow Deep Dive

### 3.1 Complete State Machine

**States:**
1. Received (initial)
2. Triaging
3. WaitingOnBroker
4. ReadyForUWReview
5. InReview
6. Quoted
7. BindRequested
8. Bound (terminal)
9. Declined (terminal)
10. Withdrawn (terminal)

**Transition Matrix:**

| From | To | Prerequisites | Authorization | Side Effects |
|------|----|--------------|--------------|--------------|
| Received | Triaging | None | System | Log timeline event |
| Triaging | WaitingOnBroker | Missing info documented | Distribution, Admin | Email broker, log event |
| Triaging | ReadyForUWReview | All required fields | Distribution, Admin | Email underwriters, log event |
| Triaging | Declined | Decline reason | Distribution, Admin | Email broker, log event |
| WaitingOnBroker | ReadyForUWReview | Missing info received | Distribution, Admin | Email underwriters, log event |
| ReadyForUWReview | InReview | Underwriter assigned | Underwriter, Admin | Log event |
| InReview | Quoted | Quote generated | Underwriter, Admin | Email broker, log event |
| Quoted | BindRequested | Broker acceptance | Distribution, Admin | Call payment API, log event |
| BindRequested | Bound | Payment confirmed | Underwriter, Admin | Issue policy, email broker, log event |

---

### 3.2 Validation Rules Per Transition

**Triaging → ReadyForUWReview:**

```csharp
public async Task ValidateReadyForUWReview(Submission submission)
{
    var errors = new List<string>();

    if (string.IsNullOrEmpty(submission.InsuredName))
        errors.Add("Insured name is required");

    if (submission.CoverageType == null)
        errors.Add("Coverage type is required");

    if (submission.ProgramId == null)
        errors.Add("Program must be selected");

    if (submission.BrokerId == null)
        errors.Add("Broker must be assigned");

    if (submission.EffectiveDate == default)
        errors.Add("Effective date is required");

    if (submission.ExpirationDate == default)
        errors.Add("Expiration date is required");

    if (submission.EffectiveDate >= submission.ExpirationDate)
        errors.Add("Expiration date must be after effective date");

    if (errors.Any())
        throw new ValidationException("Cannot transition to ReadyForUWReview", errors);
}
```

---

### 3.3 Side Effects (Email Notifications, Timeline Events)

**On Transition:**

```csharp
public async Task OnTransitionedToReadyForUWReview(Submission submission)
{
    // 1. Create workflow transition event (immutable)
    await _context.WorkflowTransitions.AddAsync(new WorkflowTransition
    {
        SubmissionId = submission.Id,
        FromStatus = "Triaging",
        ToStatus = "ReadyForUWReview",
        TransitionedAt = DateTime.UtcNow,
        TransitionedBy = _currentUser.UserId
    });

    // 2. Create timeline event
    await _context.ActivityTimelineEvents.AddAsync(new ActivityTimelineEvent
    {
        EntityType = "Submission",
        EntityId = submission.Id,
        EventType = "SubmissionReadyForReview",
        UserId = _currentUser.UserId,
        Timestamp = DateTime.UtcNow,
        Payload = new { submissionNumber = submission.SubmissionNumber }
    });

    // 3. Send email notification to underwriting team
    await _emailService.SendSubmissionReadyEmailAsync(submission);

    await _context.SaveChangesAsync();
}
```

---

### 3.4 Error Handling (Invalid Transitions, Missing Data)

**Invalid Transition (409 Conflict):**

```json
{
  "code": "INVALID_TRANSITION",
  "message": "Cannot transition from Bound to Triaging",
  "details": {
    "currentStatus": "Bound",
    "attemptedStatus": "Triaging",
    "allowedStatuses": []
  }
}
```

**Missing Required Fields (400 Bad Request):**

```json
{
  "code": "MISSING_REQUIRED_FIELDS",
  "message": "Cannot transition to ReadyForUWReview. Missing required fields.",
  "details": [
    { "field": "insuredName", "message": "Insured name is required" },
    { "field": "program", "message": "Program must be selected" }
  ]
}
```

---

### 3.5 Compensation (Rollback if Bind Fails)

**Scenario:** Transition to BindRequested → Payment API fails → Rollback to Quoted

```csharp
public async Task TransitionToBindRequestedAsync(Guid submissionId)
{
    var submission = await _context.Submissions.FindAsync(submissionId);

    // Transition to BindRequested
    submission.Status = "BindRequested";
    await _context.SaveChangesAsync();

    try
    {
        // Call payment API
        var paymentResult = await _paymentService.ProcessPaymentAsync(submission);

        if (!paymentResult.Success)
            throw new PaymentFailedException(paymentResult.Error);
    }
    catch (Exception ex)
    {
        // Compensate: Rollback to Quoted
        submission.Status = "Quoted";

        await _context.WorkflowTransitions.AddAsync(new WorkflowTransition
        {
            SubmissionId = submissionId,
            FromStatus = "BindRequested",
            ToStatus = "Quoted",
            TransitionedAt = DateTime.UtcNow,
            TransitionedBy = _currentUser.UserId,
            Reason = $"Payment failed: {ex.Message}"
        });

        await _context.SaveChangesAsync();

        throw; // Rethrow to return error to client
    }
}
```

---

## 4. Renewal Workflow

### 4.1 Scheduled Reminders

**Temporal Workflow for Renewal Reminders:**

```csharp
[Workflow]
public class RenewalReminderWorkflow
{
    [WorkflowRun]
    public async Task RunAsync(RenewalWorkflowInput input)
    {
        var policy = input.Policy;

        // Reminder 1: 120 days before expiration
        var delay120 = policy.ExpirationDate.AddDays(-120) - Workflow.UtcNow;
        if (delay120 > TimeSpan.Zero)
        {
            await Workflow.DelayAsync(delay120);
            await Workflow.ExecuteActivityAsync<SendRenewalReminderActivity>(
                new ReminderActivityInput { PolicyId = policy.Id, DaysOut = 120 },
                new ActivityOptions { StartToCloseTimeout = TimeSpan.FromMinutes(5) });
        }

        // Reminder 2: 90 days before expiration
        await Workflow.DelayAsync(TimeSpan.FromDays(30));
        await Workflow.ExecuteActivityAsync<SendRenewalReminderActivity>(
            new ReminderActivityInput { PolicyId = policy.Id, DaysOut = 90 });

        // Reminder 3: 60 days before expiration
        await Workflow.DelayAsync(TimeSpan.FromDays(30));
        await Workflow.ExecuteActivityAsync<SendRenewalReminderActivity>(
            new ReminderActivityInput { PolicyId = policy.Id, DaysOut = 60 });

        // Final check: 30 days before expiration
        await Workflow.DelayAsync(TimeSpan.FromDays(30));

        var renewalCreated = await Workflow.ExecuteActivityAsync<CheckRenewalSubmissionActivity>(
            policy.Id);

        if (!renewalCreated)
        {
            // Escalate to manager
            await Workflow.ExecuteActivityAsync<EscalateRenewalActivity>(
                new EscalationActivityInput
                {
                    PolicyId = policy.Id,
                    Reason = "No renewal submission created 30 days before expiration"
                });
        }
    }
}
```

---

### 4.2 Renewal Status Tracking

**Renewal Entity:**

```csharp
public class Renewal : BaseEntity
{
    public Guid PolicyId { get; set; }
    public Policy Policy { get; set; }

    public Guid? RenewalSubmissionId { get; set; }
    public Submission? RenewalSubmission { get; set; }

    public string Status { get; set; } // Pending, Quoted, Renewed, NonRenewed, Declined

    public DateTime ExpirationDate { get; set; }
    public DateTime? QuotedAt { get; set; }
    public DateTime? RenewedAt { get; set; }
}
```

---

### 4.3 Escalation Rules

**Escalation Triggers:**
- No renewal submission created 30 days before expiration → escalate to manager
- Renewal pending 60 days before expiration → escalate to director

**Escalation Activity:**

```csharp
[Activity]
public class EscalateRenewalActivity
{
    [ActivityMethod]
    public async Task ExecuteAsync(EscalationActivityInput input)
    {
        var policy = await _policyRepository.GetByIdAsync(input.PolicyId);

        // Send email to manager
        await _emailService.SendEscalationEmailAsync(
            to: "manager@example.com",
            subject: $"Escalation: Renewal Pending for Policy {policy.PolicyNumber}",
            body: $"Policy {policy.PolicyNumber} expires in 30 days. No renewal submission created. Reason: {input.Reason}");

        // Log escalation event
        await _context.ActivityTimelineEvents.AddAsync(new ActivityTimelineEvent
        {
            EntityType = "Renewal",
            EntityId = policy.Id,
            EventType = "RenewalEscalated",
            UserId = Guid.Empty, // System user
            Timestamp = DateTime.UtcNow,
            Payload = new { policyId = policy.Id, reason = input.Reason }
        });

        await _context.SaveChangesAsync();
    }
}
```

---

## 5. Event Sourcing for Workflows

### 5.1 WorkflowTransition Events (Immutable)

**Store all state transitions as immutable events:**

```csharp
public class WorkflowTransition : BaseEntity
{
    public Guid SubmissionId { get; set; } // Or other entity with workflow
    public string FromStatus { get; set; }
    public string ToStatus { get; set; }
    public DateTime TransitionedAt { get; set; }
    public Guid TransitionedBy { get; set; }
    public string? Reason { get; set; } // Required for Declined/Withdrawn
}
```

**Benefits:**
- Complete audit trail of all state changes
- Can rebuild current state from events
- Can analyze workflow patterns (average time in each state, common paths)

---

### 5.2 Rebuilding Workflow State from Events

**Rebuild current state by replaying events:**

```csharp
public async Task<Submission> RebuildSubmissionFromEventsAsync(Guid submissionId)
{
    var transitions = await _context.WorkflowTransitions
        .Where(t => t.SubmissionId == submissionId)
        .OrderBy(t => t.TransitionedAt)
        .ToListAsync();

    var submission = new Submission { Id = submissionId, Status = "Received" };

    foreach (var transition in transitions)
    {
        submission.Status = transition.ToStatus;
    }

    return submission;
}
```

**Use Cases:**
- Debugging (what was the state at time X?)
- Analytics (how long do submissions spend in InReview?)
- Compliance (prove state was X at time Y)

---

### 5.3 Event Versioning

**Handle schema changes over time:**

**Event Payload with Version:**

```csharp
public class WorkflowTransition : BaseEntity
{
    public string EventType { get; set; } // "SubmissionTransitioned"
    public int EventVersion { get; set; } // 1, 2, 3...
    public string Payload { get; set; } // JSON

    // Deserialize based on version
    public T GetPayload<T>() where T : class
    {
        return EventVersion switch
        {
            1 => JsonSerializer.Deserialize<T>(Payload),
            2 => JsonSerializer.Deserialize<T>(Payload), // V2 schema
            _ => throw new NotSupportedException($"Event version {EventVersion} not supported")
        };
    }
}
```

---

## Version History

**Version 2.0** - 2026-01-31 - Comprehensive workflow design guide with Temporal integration (350 lines)
**Version 1.0** - 2026-01-26 - Initial workflow design guide (52 lines)
