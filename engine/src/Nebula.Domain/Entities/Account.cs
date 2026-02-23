namespace Nebula.Domain.Entities;

public class Account : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Industry { get; set; } = default!;
    public string PrimaryState { get; set; } = default!;
    public string Region { get; set; } = default!;
    public string Status { get; set; } = default!;
}
