namespace Core.Interfaces.Services.IUtilService;

public interface IUserContextService
{
    int BusinessId { get; }
    int EstablishmentId { get; }
    int EmissionPointId { get; }
    int UserId { get; }
}
