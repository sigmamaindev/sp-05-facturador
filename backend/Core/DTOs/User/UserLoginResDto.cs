using Core.DTOs.Business;
using Core.DTOs.EmissionPoint;
using Core.DTOs.Establishment;

namespace Core.DTOs.User;

public class UserLoginResDto
{
    public int Id { get; set; }
    public string Document { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public BusinessResDto? Business { get; set; }
    public List<EstablishmentResDto> Establishment { get; set; } = [];
    public List<EmissionPointResDto> EmissionPoint { get; set; } = [];
}
