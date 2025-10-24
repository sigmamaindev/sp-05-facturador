namespace Core.Entities;

public class UserEmissionPoint : BaseEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }
    public int EmissionPointId { get; set; }
    public EmissionPoint? EmissionPoint { get; set; }
}
