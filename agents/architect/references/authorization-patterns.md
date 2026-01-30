# Authorization Patterns

Guide for implementing ABAC with Casbin in Nebula.

## ABAC Components

### Subject Attributes
```csharp
sub.userId = "user-guid"
sub.roles = ["Distribution", "Underwriter"]
sub.region = "West"
```

### Resource Attributes
```csharp
res.type = "Broker"
res.id = "broker-guid"
res.status = "Active"
res.assignedTo = "user-guid"
```

### Actions
- CreateBroker, ReadBroker, UpdateBroker, DeleteBroker
- TransitionSubmission, AssignUnderwriter, etc.

## Casbin Policy Format

```csv
# Policy: [Subject], [Object], [Action], [Effect]
p, Distribution, Broker, *, allow
p, Underwriter, Broker, Read, allow
p, Underwriter, Submission, Update, allow, sub.userId == res.assignedTo
p, Admin, *, *, allow
```

## Implementation

```csharp
public async Task CheckPermission(User user, string action, Resource resource)
{
    var enforcer = await CasbinEnforcer.GetInstance();
    var allowed = await enforcer.EnforceAsync(
        user.Id,
        resource.Type,
        action,
        resource.Attributes
    );
    
    if (!allowed)
        throw new ForbiddenException($"User lacks {action} permission");
}
```

See Casbin documentation: https://casbin.org/docs/overview
