using FluentAssertions;
using Nebula.Domain.Entities;

namespace Nebula.Tests;

public class SmokeTests
{
    [Fact]
    public void Domain_BaseEntity_HasRequiredAuditFields()
    {
        var properties = typeof(BaseEntity).GetProperties();
        var names = properties.Select(p => p.Name).ToList();

        names.Should().Contain("Id");
        names.Should().Contain("CreatedAt");
        names.Should().Contain("CreatedBy");
        names.Should().Contain("UpdatedAt");
        names.Should().Contain("UpdatedBy");
        names.Should().Contain("IsDeleted");
        names.Should().Contain("DeletedAt");
        names.Should().Contain("DeletedBy");
        names.Should().Contain("RowVersion");
    }

    [Fact]
    public void Domain_BaseEntity_GeneratesNewGuidById()
    {
        var entity = new TestEntity();
        entity.Id.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(typeof(Account), new[] { "Name", "Industry", "PrimaryState", "Region", "Status" })]
    [InlineData(typeof(MGA), new[] { "Name", "ExternalCode", "Status" })]
    [InlineData(typeof(Nebula.Domain.Entities.Program), new[] { "Name", "ProgramCode", "MgaId", "ManagedBySubject" })]
    [InlineData(typeof(Broker), new[] { "LegalName", "LicenseNumber", "State", "Status", "Email", "Phone", "ManagedBySubject", "MgaId", "PrimaryProgramId" })]
    [InlineData(typeof(Contact), new[] { "BrokerId", "AccountId", "FullName", "Email", "Phone", "Role" })]
    [InlineData(typeof(Submission), new[] { "AccountId", "BrokerId", "ProgramId", "CurrentStatus", "EffectiveDate", "PremiumEstimate", "AssignedTo" })]
    [InlineData(typeof(Renewal), new[] { "AccountId", "BrokerId", "SubmissionId", "CurrentStatus", "RenewalDate", "AssignedTo" })]
    [InlineData(typeof(TaskItem), new[] { "Title", "Description", "Status", "Priority", "DueDate", "AssignedTo", "LinkedEntityType", "LinkedEntityId", "CompletedAt" })]
    public void Domain_BaseEntityDescendant_HasExpectedProperties(Type entityType, string[] expectedProperties)
    {
        entityType.Should().BeDerivedFrom<BaseEntity>();
        var names = entityType.GetProperties().Select(p => p.Name).ToList();
        foreach (var prop in expectedProperties)
        {
            names.Should().Contain(prop, because: $"{entityType.Name} should have property {prop}");
        }
    }

    [Theory]
    [InlineData(typeof(ActivityTimelineEvent), new[] { "Id", "EntityType", "EntityId", "EventType", "EventPayloadJson", "EventDescription", "ActorSubject", "ActorDisplayName", "OccurredAt" })]
    [InlineData(typeof(WorkflowTransition), new[] { "Id", "WorkflowType", "EntityId", "FromState", "ToState", "Reason", "ActorSubject", "OccurredAt" })]
    public void Domain_AppendOnlyEntity_DoesNotInheritBaseEntity(Type entityType, string[] expectedProperties)
    {
        entityType.Should().NotBeDerivedFrom<BaseEntity>();
        var names = entityType.GetProperties().Select(p => p.Name).ToList();
        foreach (var prop in expectedProperties)
        {
            names.Should().Contain(prop, because: $"{entityType.Name} should have property {prop}");
        }
    }

    [Fact]
    public void Domain_UserProfile_HasOwnAuditFieldsNotBaseEntity()
    {
        typeof(UserProfile).Should().NotBeDerivedFrom<BaseEntity>();
        var names = typeof(UserProfile).GetProperties().Select(p => p.Name).ToList();
        names.Should().Contain("Subject");
        names.Should().Contain("Email");
        names.Should().Contain("DisplayName");
        names.Should().Contain("Department");
        names.Should().Contain("RegionsJson");
        names.Should().Contain("RolesJson");
        names.Should().Contain("IsActive");
        names.Should().Contain("CreatedAt");
        names.Should().Contain("UpdatedAt");
    }

    [Fact]
    public void Domain_BrokerRegion_HasCompositePKProperties()
    {
        typeof(BrokerRegion).Should().NotBeDerivedFrom<BaseEntity>();
        var names = typeof(BrokerRegion).GetProperties().Select(p => p.Name).ToList();
        names.Should().Contain("BrokerId");
        names.Should().Contain("Region");
    }

    [Theory]
    [InlineData(typeof(ReferenceTaskStatus), new[] { "Code", "DisplayName", "DisplayOrder" })]
    [InlineData(typeof(ReferenceSubmissionStatus), new[] { "Code", "DisplayName", "Description", "IsTerminal", "DisplayOrder", "ColorGroup" })]
    [InlineData(typeof(ReferenceRenewalStatus), new[] { "Code", "DisplayName", "Description", "IsTerminal", "DisplayOrder", "ColorGroup" })]
    public void Domain_ReferenceEntity_HasCodeAndDisplayFields(Type entityType, string[] expectedProperties)
    {
        entityType.Should().NotBeDerivedFrom<BaseEntity>();
        var names = entityType.GetProperties().Select(p => p.Name).ToList();
        foreach (var prop in expectedProperties)
        {
            names.Should().Contain(prop, because: $"{entityType.Name} should have property {prop}");
        }
    }

    private class TestEntity : BaseEntity { }
}
