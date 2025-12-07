using Core.DTOs.BusinessDto;
using Core.DTOs.EmissionPointDto;
using Core.DTOs.EstablishmentDto;
using Core.DTOs.RoleDto;

namespace Core.DTOs.UserDto;

public class UserResDto
{
    public int Id { get; set; }
    public string Document { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Cellphone { get; set; }
    public string? Telephone { get; set; }
    public string? ImageUrl { get; set; }
    public string? CompanyName { get; set; }
    public string? Sequence { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<RolResDto> Roles { get; set; } = [];
    public BusinessResDto? Business { get; set; }
    public List<EstablishmentResDto> Establishment { get; set; } = [];
    public List<EmissionPointResDto> EmissionPoint { get; set; } = [];
}
