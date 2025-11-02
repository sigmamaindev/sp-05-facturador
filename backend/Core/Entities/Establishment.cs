using System;

namespace Core.Entities;

public class Establishment : BaseEntity
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; } = true;
    public int BusinessId { get; set; }
    public Business? Business { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<EmissionPoint> EmissionPoints { get; set; } = [];
    public ICollection<UserEstablishment> UserEstablishments { get; set; } = [];
}
