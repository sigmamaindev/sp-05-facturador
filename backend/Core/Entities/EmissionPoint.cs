using System;

namespace Core.Entities;

public class EmissionPoint : BaseEntity
{
    public required string Code { get; set; }
    public required string Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int EstablishmentId { get; set; }
    public Establishment? Establishment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<UserEmissionPoint> UserEmissionPoints { get; set; } = [];
    public ICollection<Invoice> Invoices { get; set; } = [];
}
