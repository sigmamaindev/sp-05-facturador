namespace Core.Entities;

public class Role : BaseEntity
{
    public required string Name { get; set; }
    public ICollection<UserRole> UserRole { get; set; } = [];
}
