namespace Nebula.Domain.Entities;

public class Renewal : BaseEntity
{
    public Guid AccountId { get; set; }
    public Guid BrokerId { get; set; }
    public Guid? SubmissionId { get; set; }
    public string CurrentStatus { get; set; } = "Created";
    public DateTime RenewalDate { get; set; }
    public string AssignedTo { get; set; } = default!;

    public Account Account { get; set; } = default!;
    public Broker Broker { get; set; } = default!;
    public Submission? Submission { get; set; }
}
