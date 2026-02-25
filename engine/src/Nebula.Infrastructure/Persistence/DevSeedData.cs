using Microsoft.EntityFrameworkCore;
using Nebula.Domain.Entities;

namespace Nebula.Infrastructure.Persistence;

public static class DevSeedData
{
    public static async Task SeedDevDataAsync(AppDbContext db)
    {
        if (await db.UserProfiles.AnyAsync()) return; // already seeded

        var now = DateTime.UtcNow;
        var user1Sub = "dev-user-001";
        var user2Sub = "dev-user-002";
        var user3Sub = "dev-user-003";

        // UserProfiles
        var profiles = new[]
        {
            new UserProfile { Subject = user1Sub, Email = "sarah.chen@nebula.local", DisplayName = "Sarah Chen", Department = "Distribution", RegionsJson = "[\"West\"]", RolesJson = "[\"DistributionManager\"]", CreatedAt = now, UpdatedAt = now },
            new UserProfile { Subject = user2Sub, Email = "john.miller@nebula.local", DisplayName = "John Miller", Department = "Underwriting", RegionsJson = "[\"East\"]", RolesJson = "[\"Underwriter\"]", CreatedAt = now, UpdatedAt = now },
            new UserProfile { Subject = user3Sub, Email = "lisa.wong@nebula.local", DisplayName = "Lisa Wong", Department = "Distribution", RegionsJson = "[\"West\",\"Central\"]", RolesJson = "[\"DistributionUser\"]", CreatedAt = now, UpdatedAt = now },
        };
        db.UserProfiles.AddRange(profiles);

        // MGA + Program (needed as FK targets)
        var mga = new MGA { Name = "Acme MGA", ExternalCode = "ACME-001", Status = "Active", CreatedAt = now, UpdatedAt = now, CreatedBy = user1Sub, UpdatedBy = user1Sub };
        db.MGAs.Add(mga);
        await db.SaveChangesAsync();

        var program = new Nebula.Domain.Entities.Program { Name = "General Liability", ProgramCode = "GL-001", MgaId = mga.Id, ManagedBySubject = user1Sub, CreatedAt = now, UpdatedAt = now, CreatedBy = user1Sub, UpdatedBy = user1Sub };
        db.Programs.Add(program);
        await db.SaveChangesAsync();

        // Accounts
        var accounts = new[]
        {
            new Account { Name = "Acme Corp", Industry = "Manufacturing", PrimaryState = "CA", Region = "West", Status = "Active", CreatedAt = now, UpdatedAt = now, CreatedBy = user1Sub, UpdatedBy = user1Sub },
            new Account { Name = "Pacific Reinsurance", Industry = "Insurance", PrimaryState = "WA", Region = "West", Status = "Active", CreatedAt = now, UpdatedAt = now, CreatedBy = user1Sub, UpdatedBy = user1Sub },
            new Account { Name = "Midwest Manufacturing", Industry = "Manufacturing", PrimaryState = "IL", Region = "Central", Status = "Active", CreatedAt = now, UpdatedAt = now, CreatedBy = user3Sub, UpdatedBy = user3Sub },
        };
        db.Accounts.AddRange(accounts);
        await db.SaveChangesAsync();

        // Brokers
        var brokers = new[]
        {
            new Broker { LegalName = "Western Insurance Group", LicenseNumber = "WIG-2024-001", State = "CA", Status = "Active", Email = "info@western-ins.com", Phone = "+14155551234", ManagedBySubject = user1Sub, MgaId = mga.Id, PrimaryProgramId = program.Id, CreatedAt = now, UpdatedAt = now, CreatedBy = user1Sub, UpdatedBy = user1Sub },
            new Broker { LegalName = "Eastern Brokerage LLC", LicenseNumber = "EBL-2024-002", State = "NY", Status = "Active", Email = "contact@eastern-brokerage.com", Phone = "+12125559876", ManagedBySubject = user2Sub, CreatedAt = now, UpdatedAt = now, CreatedBy = user2Sub, UpdatedBy = user2Sub },
            new Broker { LegalName = "Legacy Partners", LicenseNumber = "LP-2024-003", State = "TX", Status = "Inactive", Email = "legacy@partners.com", Phone = "+12145550000", CreatedAt = now, UpdatedAt = now, CreatedBy = user1Sub, UpdatedBy = user1Sub },
        };
        db.Brokers.AddRange(brokers);
        await db.SaveChangesAsync();

        // BrokerRegions
        db.BrokerRegions.AddRange(
            new BrokerRegion { BrokerId = brokers[0].Id, Region = "West" },
            new BrokerRegion { BrokerId = brokers[0].Id, Region = "Central" },
            new BrokerRegion { BrokerId = brokers[1].Id, Region = "East" },
            new BrokerRegion { BrokerId = brokers[2].Id, Region = "South" }
        );

        // Contacts
        var contacts = new[]
        {
            new Contact { BrokerId = brokers[0].Id, FullName = "Alice Thompson", Email = "alice@western-ins.com", Phone = "+14155551111", Role = "Primary", CreatedAt = now, UpdatedAt = now, CreatedBy = user1Sub, UpdatedBy = user1Sub },
            new Contact { BrokerId = brokers[0].Id, FullName = "Bob Martinez", Email = "bob@western-ins.com", Phone = "+14155552222", Role = "Accounting", CreatedAt = now, UpdatedAt = now, CreatedBy = user1Sub, UpdatedBy = user1Sub },
            new Contact { BrokerId = brokers[1].Id, FullName = "Carol Davis", Email = "carol@eastern-brokerage.com", Phone = "+12125553333", Role = "Primary", CreatedAt = now, UpdatedAt = now, CreatedBy = user2Sub, UpdatedBy = user2Sub },
            new Contact { BrokerId = brokers[2].Id, FullName = "Dan Wilson", Email = "dan@legacy.com", Phone = "+12145554444", Role = "Primary", CreatedAt = now, UpdatedAt = now, CreatedBy = user1Sub, UpdatedBy = user1Sub },
        };
        db.Contacts.AddRange(contacts);

        // Submissions
        var submissions = new[]
        {
            new Submission { AccountId = accounts[0].Id, BrokerId = brokers[0].Id, ProgramId = program.Id, CurrentStatus = "Received", EffectiveDate = now.AddDays(30), PremiumEstimate = 45000m, AssignedTo = user1Sub, CreatedAt = now.AddDays(-10), UpdatedAt = now.AddDays(-10), CreatedBy = user1Sub, UpdatedBy = user1Sub },
            new Submission { AccountId = accounts[0].Id, BrokerId = brokers[0].Id, ProgramId = program.Id, CurrentStatus = "Triaging", EffectiveDate = now.AddDays(45), PremiumEstimate = 78000m, AssignedTo = user1Sub, CreatedAt = now.AddDays(-8), UpdatedAt = now.AddDays(-6), CreatedBy = user1Sub, UpdatedBy = user1Sub },
            new Submission { AccountId = accounts[1].Id, BrokerId = brokers[1].Id, CurrentStatus = "WaitingOnBroker", EffectiveDate = now.AddDays(60), PremiumEstimate = 120000m, AssignedTo = user2Sub, CreatedAt = now.AddDays(-15), UpdatedAt = now.AddDays(-7), CreatedBy = user2Sub, UpdatedBy = user2Sub },
            new Submission { AccountId = accounts[1].Id, BrokerId = brokers[1].Id, CurrentStatus = "InReview", EffectiveDate = now.AddDays(20), PremiumEstimate = 55000m, AssignedTo = user2Sub, CreatedAt = now.AddDays(-20), UpdatedAt = now.AddDays(-3), CreatedBy = user2Sub, UpdatedBy = user2Sub },
            new Submission { AccountId = accounts[2].Id, BrokerId = brokers[0].Id, ProgramId = program.Id, CurrentStatus = "Quoted", EffectiveDate = now.AddDays(15), PremiumEstimate = 92000m, AssignedTo = user3Sub, CreatedAt = now.AddDays(-25), UpdatedAt = now.AddDays(-2), CreatedBy = user3Sub, UpdatedBy = user3Sub },
            new Submission { AccountId = accounts[0].Id, BrokerId = brokers[0].Id, CurrentStatus = "Bound", EffectiveDate = now.AddDays(-30), PremiumEstimate = 67000m, AssignedTo = user1Sub, CreatedAt = now.AddDays(-60), UpdatedAt = now.AddDays(-30), CreatedBy = user1Sub, UpdatedBy = user1Sub },
            new Submission { AccountId = accounts[1].Id, BrokerId = brokers[1].Id, CurrentStatus = "Declined", EffectiveDate = now.AddDays(-15), PremiumEstimate = 35000m, AssignedTo = user2Sub, CreatedAt = now.AddDays(-45), UpdatedAt = now.AddDays(-15), CreatedBy = user2Sub, UpdatedBy = user2Sub },
        };
        db.Submissions.AddRange(submissions);
        await db.SaveChangesAsync();

        // Renewals
        var renewals = new[]
        {
            new Renewal { AccountId = accounts[0].Id, BrokerId = brokers[0].Id, SubmissionId = submissions[5].Id, CurrentStatus = "Created", RenewalDate = now.AddDays(10), AssignedTo = user1Sub, CreatedAt = now.AddDays(-5), UpdatedAt = now.AddDays(-5), CreatedBy = user1Sub, UpdatedBy = user1Sub },
            new Renewal { AccountId = accounts[0].Id, BrokerId = brokers[0].Id, CurrentStatus = "OutreachStarted", RenewalDate = now.AddDays(7), AssignedTo = user3Sub, CreatedAt = now.AddDays(-20), UpdatedAt = now.AddDays(-4), CreatedBy = user3Sub, UpdatedBy = user3Sub },
            new Renewal { AccountId = accounts[1].Id, BrokerId = brokers[1].Id, CurrentStatus = "InReview", RenewalDate = now.AddDays(30), AssignedTo = user2Sub, CreatedAt = now.AddDays(-30), UpdatedAt = now.AddDays(-2), CreatedBy = user2Sub, UpdatedBy = user2Sub },
            new Renewal { AccountId = accounts[2].Id, BrokerId = brokers[0].Id, CurrentStatus = "Bound", RenewalDate = now.AddDays(-10), AssignedTo = user1Sub, CreatedAt = now.AddDays(-50), UpdatedAt = now.AddDays(-10), CreatedBy = user1Sub, UpdatedBy = user1Sub },
        };
        db.Renewals.AddRange(renewals);
        await db.SaveChangesAsync();

        // Tasks
        var tasks = new[]
        {
            new TaskItem { Title = "Follow up with Western Insurance", Description = "Check on pending submission documents", Status = "Open", Priority = "High", DueDate = now.AddDays(-2), AssignedTo = user1Sub, LinkedEntityType = "Broker", LinkedEntityId = brokers[0].Id, CreatedAt = now.AddDays(-5), UpdatedAt = now.AddDays(-5), CreatedBy = user1Sub, UpdatedBy = user1Sub },
            new TaskItem { Title = "Review Acme Corp policy limits", Status = "Open", Priority = "Normal", DueDate = now.AddDays(-1), AssignedTo = user1Sub, LinkedEntityType = "Submission", LinkedEntityId = submissions[0].Id, CreatedAt = now.AddDays(-3), UpdatedAt = now.AddDays(-3), CreatedBy = user1Sub, UpdatedBy = user1Sub },
            new TaskItem { Title = "Prepare renewal quote for Pacific Re", Status = "InProgress", Priority = "Urgent", DueDate = now.AddDays(3), AssignedTo = user2Sub, LinkedEntityType = "Renewal", LinkedEntityId = renewals[2].Id, CreatedAt = now.AddDays(-2), UpdatedAt = now.AddDays(-1), CreatedBy = user2Sub, UpdatedBy = user2Sub },
            new TaskItem { Title = "Update broker contact information", Status = "Open", Priority = "Low", DueDate = now.AddDays(7), AssignedTo = user3Sub, LinkedEntityType = "Broker", LinkedEntityId = brokers[1].Id, CreatedAt = now.AddDays(-1), UpdatedAt = now.AddDays(-1), CreatedBy = user3Sub, UpdatedBy = user3Sub },
            new TaskItem { Title = "Schedule quarterly review", Status = "Open", Priority = "Normal", DueDate = now.AddDays(14), AssignedTo = user1Sub, CreatedAt = now, UpdatedAt = now, CreatedBy = user1Sub, UpdatedBy = user1Sub },
            new TaskItem { Title = "Complete underwriting checklist", Status = "Done", Priority = "High", DueDate = now.AddDays(-5), AssignedTo = user2Sub, CompletedAt = now.AddDays(-3), LinkedEntityType = "Submission", LinkedEntityId = submissions[3].Id, CreatedAt = now.AddDays(-10), UpdatedAt = now.AddDays(-3), CreatedBy = user2Sub, UpdatedBy = user2Sub },
        };
        db.Tasks.AddRange(tasks);

        // Workflow transitions
        var transitions = new[]
        {
            new WorkflowTransition { WorkflowType = "Submission", EntityId = submissions[1].Id, FromState = "Received", ToState = "Triaging", ActorSubject = user1Sub, OccurredAt = now.AddDays(-6) },
            new WorkflowTransition { WorkflowType = "Submission", EntityId = submissions[2].Id, FromState = "Received", ToState = "Triaging", ActorSubject = user2Sub, OccurredAt = now.AddDays(-12) },
            new WorkflowTransition { WorkflowType = "Submission", EntityId = submissions[2].Id, FromState = "Triaging", ToState = "WaitingOnBroker", ActorSubject = user2Sub, OccurredAt = now.AddDays(-7) },
            new WorkflowTransition { WorkflowType = "Submission", EntityId = submissions[3].Id, FromState = "Received", ToState = "Triaging", ActorSubject = user2Sub, OccurredAt = now.AddDays(-18) },
            new WorkflowTransition { WorkflowType = "Submission", EntityId = submissions[3].Id, FromState = "Triaging", ToState = "ReadyForUWReview", ActorSubject = user2Sub, OccurredAt = now.AddDays(-10) },
            new WorkflowTransition { WorkflowType = "Submission", EntityId = submissions[3].Id, FromState = "ReadyForUWReview", ToState = "InReview", ActorSubject = user2Sub, OccurredAt = now.AddDays(-3) },
            new WorkflowTransition { WorkflowType = "Submission", EntityId = submissions[5].Id, FromState = "Received", ToState = "Triaging", ActorSubject = user1Sub, OccurredAt = now.AddDays(-55) },
            new WorkflowTransition { WorkflowType = "Submission", EntityId = submissions[5].Id, FromState = "Quoted", ToState = "BindRequested", ActorSubject = user1Sub, OccurredAt = now.AddDays(-35) },
            new WorkflowTransition { WorkflowType = "Submission", EntityId = submissions[5].Id, FromState = "BindRequested", ToState = "Bound", ActorSubject = user1Sub, OccurredAt = now.AddDays(-30) },
            new WorkflowTransition { WorkflowType = "Submission", EntityId = submissions[6].Id, FromState = "InReview", ToState = "Declined", ActorSubject = user2Sub, OccurredAt = now.AddDays(-15) },
            new WorkflowTransition { WorkflowType = "Renewal", EntityId = renewals[1].Id, FromState = "Created", ToState = "Early", ActorSubject = user3Sub, OccurredAt = now.AddDays(-15) },
            new WorkflowTransition { WorkflowType = "Renewal", EntityId = renewals[1].Id, FromState = "Early", ToState = "OutreachStarted", ActorSubject = user3Sub, OccurredAt = now.AddDays(-4) },
            new WorkflowTransition { WorkflowType = "Renewal", EntityId = renewals[3].Id, FromState = "Quoted", ToState = "Bound", ActorSubject = user1Sub, OccurredAt = now.AddDays(-10) },
        };
        db.WorkflowTransitions.AddRange(transitions);

        // Timeline events
        var events = new[]
        {
            new ActivityTimelineEvent { EntityType = "Broker", EntityId = brokers[0].Id, EventType = "BrokerCreated", EventDescription = "New broker \"Western Insurance Group\" added", ActorSubject = user1Sub, ActorDisplayName = "Sarah Chen", OccurredAt = now.AddDays(-30) },
            new ActivityTimelineEvent { EntityType = "Broker", EntityId = brokers[1].Id, EventType = "BrokerCreated", EventDescription = "New broker \"Eastern Brokerage LLC\" added", ActorSubject = user2Sub, ActorDisplayName = "John Miller", OccurredAt = now.AddDays(-28) },
            new ActivityTimelineEvent { EntityType = "Broker", EntityId = brokers[2].Id, EventType = "BrokerCreated", EventDescription = "New broker \"Legacy Partners\" added", ActorSubject = user1Sub, ActorDisplayName = "Sarah Chen", OccurredAt = now.AddDays(-25) },
            new ActivityTimelineEvent { EntityType = "Broker", EntityId = brokers[2].Id, EventType = "BrokerStatusChanged", EventDescription = "Broker \"Legacy Partners\" status changed from Active to Inactive", ActorSubject = user1Sub, ActorDisplayName = "Sarah Chen", OccurredAt = now.AddDays(-5) },
            new ActivityTimelineEvent { EntityType = "Broker", EntityId = brokers[0].Id, EventType = "ContactCreated", EventDescription = "Contact \"Alice Thompson\" added to broker \"Western Insurance Group\"", ActorSubject = user1Sub, ActorDisplayName = "Sarah Chen", OccurredAt = now.AddDays(-29) },
            new ActivityTimelineEvent { EntityType = "Broker", EntityId = brokers[0].Id, EventType = "BrokerUpdated", EventDescription = "Broker \"Western Insurance Group\" updated", ActorSubject = user3Sub, ActorDisplayName = "Lisa Wong", OccurredAt = now.AddDays(-2) },
            new ActivityTimelineEvent { EntityType = "Submission", EntityId = submissions[3].Id, EventType = "SubmissionTransitioned", EventDescription = "Submission transitioned from ReadyForUWReview to InReview", ActorSubject = user2Sub, ActorDisplayName = "John Miller", OccurredAt = now.AddDays(-3) },
        };
        db.ActivityTimelineEvents.AddRange(events);

        await db.SaveChangesAsync();
    }
}
