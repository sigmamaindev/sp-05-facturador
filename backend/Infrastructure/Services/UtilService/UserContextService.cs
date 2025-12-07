using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Core.Interfaces.Services.IUtilService;

namespace Infrastructure.Services.UtilService;

public class UserContextService(IHttpContextAccessor http) : IUserContextService
{
    public int BusinessId => GetContextValue("BusinessId");

    public int EstablishmentId => GetContextValue("EstablishmentId");

    public int EmissionPointId => GetContextValue("EmissionPointId");

    public int UserId => GetContextValue(ClaimTypes.NameIdentifier);

    private int GetContextValue(string type)
    {
        var val = http.HttpContext?.User.FindFirst(type)?.Value;
        return int.TryParse(val, out var id) ? id : 0;
    }
}
