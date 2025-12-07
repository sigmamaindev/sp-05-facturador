namespace Core.DTOs.UserDto;

public class UserUpdateReqDto
{
    public string Document { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Sequence { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Cellphone { get; set; }
    public string? Telephone { get; set; }
    public string? ImageUrl { get; set; }
    public string? CompanyName { get; set; }
    public List<int> RolIds { get; set; } = [];
    public int BusinessId { get; set; }
    public int EstablishmentId { get; set; }
    public int EmissionPointId { get; set; }
}
