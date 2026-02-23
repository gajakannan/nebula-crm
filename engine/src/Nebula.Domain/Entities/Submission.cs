namespace Nebula.Domain.Entities;

public class Submission : BaseEntity
{
    public Guid AccountId { get; set; }
    public Guid BrokerId { get; set; }
    public Guid? ProgramId { get; set; }
    public string CurrentStatus { get; set; } = "Received";
    public DateTime EffectiveDate { get; set; }
    public decimal PremiumEstimate { get; set; }
    public string AssignedTo { get; set; } = default!;

    public Account Account { get; set; } = default!;
    public Broker Broker { get; set; } = default!;
    public Program? Program { get; set; }
}
