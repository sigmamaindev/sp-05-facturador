using Core.DTOs;
using Core.DTOs.PersonLookupDto;

namespace Core.Interfaces.Repository;

public interface IPersonLookupRepository
{
    Task<ApiResponse<PersonLookupResDto>> LookupByDocumentAsync(string document);
}
