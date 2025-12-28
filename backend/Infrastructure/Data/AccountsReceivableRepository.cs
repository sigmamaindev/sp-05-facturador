using Microsoft.EntityFrameworkCore;
using Core.Interfaces.Services.IUtilService;
using Core.Interfaces.Services.IARService;
using Core.Interfaces.Repository;
using Core.DTOs;
using Core.DTOs.ARDto;

namespace Infrastructure.Data;

public class AccountsReceivableRepository(
    StoreContext context,
    IUserContextService currentUser,
    IARDtoFactory aRDtoFactory) : IAccountsReceivableRepository
{
    public async Task<ApiResponse<ARComplexResDto>> GetAccountsReceivableByIdAsync(int id)
    {
        var response = new ApiResponse<ARComplexResDto>();

        try
        {
            if (currentUser.BusinessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no asociado a esta usuario";
                response.Error = "Error de asociación";

                return response;
            }

            var existingAR = await context.AccountsReceivables
            .Include(ar => ar.Customer)
            .Include(ar => ar.Invoice).ThenInclude(i => i!.Business)
            .Include(ar => ar.Invoice).ThenInclude(i => i!.Establishment)
            .Include(ar => ar.Invoice).ThenInclude(i => i!.EmissionPoint)
            .Include(ar => ar.Invoice).ThenInclude(i => i!.User)
            .Include(ar => ar.Transactions)
            .FirstOrDefaultAsync(
                ar =>
                ar.Id == id);

            if (existingAR == null)
            {
                response.Success = false;
                response.Message = "Cuenta por cobrar no encontrada";
                response.Error = "No existe una Cuenta por cobrar con el ID especificado";

                return response;
            }

            response.Success = true;
            response.Message = "Cuenta por cobrar obtenida correctamente";
            response.Data = aRDtoFactory.ARComplexRes(existingAR);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener la cuenta por cobrar";
            response.Error = $"{ex.Message} | {ex.StackTrace}";
        }

        return response;
    }

    public async Task<ApiResponse<List<ARSimpleResDto>>> GetAccountsReceivablesAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<ARSimpleResDto>>();

        try
        {
            if (currentUser.BusinessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no asociado a esta usuario";
                response.Error = "Error de asociación";

                return response;
            }

            var query = context.AccountsReceivables
            .Include(ar => ar.Customer)
            .Include(ar => ar.Invoice).ThenInclude(i => i!.Business)
            .Include(ar => ar.Invoice).ThenInclude(i => i!.Establishment)
            .Include(ar => ar.Invoice).ThenInclude(i => i!.EmissionPoint)
            .Include(ar => ar.Invoice).ThenInclude(i => i!.User)
            .Where(
                ar =>
                ar.BusinessId == currentUser.BusinessId &&
                ar.Invoice != null &&
                ar.Invoice.EstablishmentId == currentUser.EstablishmentId &&
                ar.Invoice.EmissionPointId == currentUser.EmissionPointId &&
                ar.Invoice.UserId == currentUser.UserId);

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();

                query = query.Where(ar =>
                    (ar.Invoice != null && EF.Functions.ILike(ar.Invoice.Sequential, $"%{keyword}%")) ||
                    (ar.Customer != null && (
                        EF.Functions.ILike(ar.Customer.Name, $"%{keyword}%") ||
                        EF.Functions.ILike(ar.Customer.Document, $"%{keyword}%")
                    )) ||
                    EF.Functions.ILike(ar.Status, $"%{keyword}%")
                );
            }

            var total = await query.CountAsync();
            var skip = (page - 1) * limit;

            var arList = await query
                .OrderByDescending(ar => ar.IssueDate)
                .Skip(skip)
                .Take(limit)
                .ToListAsync();

            var accountsReceivables = arList.Select(aRDtoFactory.ARSimpleRes).ToList();

            response.Success = true;
            response.Message = "Listado de cuentas por cobrar obtenido correctamente.";
            response.Data = accountsReceivables;
            response.Pagination = new Pagination
            {
                Total = total,
                Page = page,
                Limit = limit
            };

            return response;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener las cuentas por cobrar";
            response.Error = ex.Message;

            return response;
        }
    }
}

