namespace Core.Entities;

public class UserEstablishment : BaseEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }
    public int EstablishmentId { get; set; }
    public Establishment? Establishment { get; set; }
}
