using Core.DTOs;
using Core.DTOs.User;
using Core.Entities;

namespace Core.Interfaces;

public interface IUserRepository
{
    Task<ApiResponse<UserLoginResDto>> LoginAsync(UserLoginReqDto loginUserReqDto);
    Task<ApiResponse<List<UserResDto>>> GetUsersAsync(string? keyword, int page, int limit);
    Task<ApiResponse<UserResDto>> GetUserByIdAsync(int id);
    Task<ApiResponse<UserResDto>> CreateUserAsync(UserCreateReqDto userCreateReqDto, List<int> rolIds, int establishmentId, int emissionPointId);
    Task<ApiResponse<UserResDto>> UpdateUserAsync(int userId, UserUpdateReqDto userUpdateReqDto, List<int> rolIds, int establishmentId, int emissionPointId);
}
