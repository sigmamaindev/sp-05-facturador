using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Core.Entities;
using Core.Interfaces;
using Core.DTOs;
using Core.DTOs.User;
using Core.DTOs.Business;
using Core.DTOs.Establishment;
using Core.DTOs.EmissionPoint;
using Core.DTOs.Role;

namespace Infrastructure.Data;

public class UserRepository(StoreContext context, IConfiguration config, IHttpContextAccessor httpContextAccessor) : IUserRepository
{
    public async Task<ApiResponse<UserResDto>> CreateUserAsync(UserCreateReqDto userCreateReqDto, List<int> rolIds, int establishmentId, int emissionPointId)
    {
        var response = new ApiResponse<UserResDto>();

        try
        {
            var businessId = GetBusinessIdFromToken();

            if (businessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no asociado a esta usuario";
                response.Error = "Error de asociación";

                return response;
            }

            var establishment = await context.Establishments
            .FirstOrDefaultAsync(e => e.Id == establishmentId && e.BusinessId == businessId);

            if (establishment == null)
            {
                response.Success = false;
                response.Message = "El establecimiento no pertenece al negocio asociado";
                response.Error = "Error de relación";

                return response;
            }

            var emissionPoint = await context.EmissionPoints
            .FirstOrDefaultAsync(ep => ep.Id == emissionPointId && ep.EstablishmentId == establishmentId);

            if (emissionPoint == null)
            {
                response.Success = false;
                response.Message = "El punto de emisión no pertenece al establecimiento seleccionado";
                response.Error = "Error de relación";

                return response;
            }

            var existingUser = await context.Users
            .Include(u => u.UserBusiness)
            .FirstOrDefaultAsync(u =>
            (u.Email == userCreateReqDto.Email ||
            u.Username == userCreateReqDto.Username) &&
            u.UserBusiness.Any(ub => ub.BusinessId == businessId));

            if (existingUser != null)
            {
                response.Success = false;
                response.Message = "El usuario ya está registrado en este negocio";
                response.Error = "Error de duplicación";

                return response;
            }

            var user = new User
            {
                Document = userCreateReqDto.Document,
                Email = userCreateReqDto.Email,
                FullName = userCreateReqDto.FullName,
                Username = userCreateReqDto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(userCreateReqDto.Password),
                Sequence = userCreateReqDto.Sequence,
                Address = userCreateReqDto.Address,
                Cellphone = userCreateReqDto.Cellphone,
                Telephone = userCreateReqDto.Telephone,
                ImageUrl = userCreateReqDto.ImageUrl,
                CompanyName = userCreateReqDto.CompanyName
            };

            context.Users.Add(user);

            await context.SaveChangesAsync();

            user.UserRole =
            [.. rolIds.Select(rid =>
            new UserRole
            {
                RoleId = rid,
                UserId = user.Id
            })];

            user.UserBusiness =
            [
                new UserBusiness
                {
                    UserId = user.Id,
                    BusinessId = businessId
                }
            ];

            user.UserEstablishment =
            [
                new UserEstablishment
                {
                    UserId = user.Id,
                    EstablishmentId = establishmentId
                }
            ];

            user.UserEmissionPoint = [
                new UserEmissionPoint
                {
                    UserId = user.Id,
                    EmissionPointId = emissionPointId
                }
            ];

            await context.SaveChangesAsync();

            var createdUser = await context.Users
            .Include(u => u.UserRole).ThenInclude(ur => ur.Role)
            .Include(u => u.UserBusiness).ThenInclude(ub => ub.Business)
            .FirstAsync(u => u.Id == user.Id);

            var userResDto = new UserResDto
            {
                Id = createdUser.Id,
                Document = createdUser.Document,
                Username = createdUser.Username,
                FullName = createdUser.FullName,
                Email = createdUser.Email,
                Address = user.Address,
                Telephone = user.Telephone,
                Cellphone = user.Cellphone,
                ImageUrl = user.ImageUrl,
                CompanyName = user.CompanyName,
                Sequence = user.Sequence,
                IsActive = user.IsActive,
                Roles = [.. user.UserRole.Select(ur => new RolResDto
                {
                    Id = ur.Role!.Id,
                    Name = ur.Role.Name
                })],
                Business = user.UserBusiness.Select(ub => new BusinessResDto
                {
                    Id = ub.Business!.Id,
                    Document = ub.Business.Document,
                    Name = ub.Business.Name,
                    Address = ub.Business.Address,
                    IsActive = ub.Business.IsActive,
                    CreatedAt = ub.Business.CreatedAt
                }).FirstOrDefault(),
                Establishment = [..user.UserEstablishment.Select(ue => new EstablishmentResDto
                {
                    Id = ue.Establishment!.Id,
                    Code = ue.Establishment.Code,
                    Name = ue.Establishment.Name,
                    IsActive = ue.Establishment.IsActive
                })],
                EmissionPoint = [..user.UserEmissionPoint.Select(ep=>new EmissionPointResDto
                {
                    Id = ep.EmissionPoint!.Id,
                    Code = ep.EmissionPoint.Code,
                    Description = ep.EmissionPoint.Description,
                    IsActive = ep.EmissionPoint.IsActive
                })]
            };

            response.Success = true;
            response.Message = "Usuario creado correctamente";
            response.Data = userResDto;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al crear el usuario";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<UserResDto>> GetUserByIdAsync(int id)
    {
        var response = new ApiResponse<UserResDto>();

        try
        {
            var businessId = GetBusinessIdFromToken();

            if (businessId == 0)
            {
                response.Success = false;
                response.Message = "Negocios no asociado a esta usuario";
                response.Error = "Error de asociación";

                return response;
            }

            var user = await context.Users
            .Include(u => u.UserRole).ThenInclude(u => u.Role)
            .Include(u => u.UserBusiness).ThenInclude(u => u.Business)
            .Include(u => u.UserEstablishment).ThenInclude(u => u.Establishment)
            .Include(u => u.UserEmissionPoint).ThenInclude(u => u.EmissionPoint)
            .Where(u =>
            !u.UserRole.Any(ur => ur.Role!.Name == "SuperAdmin") &&
            u.UserBusiness.Any(ub => ub.BusinessId == businessId))
            .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                response.Success = false;
                response.Message = "Usuario no encontrado";
                response.Error = "No existe un usuario con el ID especificado";

                return response;
            }

            var existingUserDto = new UserResDto
            {
                Id = user.Id,
                Document = user.Document,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                Address = user.Address,
                Telephone = user.Telephone,
                Cellphone = user.Cellphone,
                ImageUrl = user.ImageUrl,
                CompanyName = user.CompanyName,
                Sequence = user.Sequence,
                IsActive = user.IsActive,
                Roles = [.. user.UserRole.Select(ur => new RolResDto
                {
                    Id = ur.Role!.Id,
                    Name = ur.Role.Name
                })],
                Business = user.UserBusiness.Select(ub => new BusinessResDto
                {
                    Id = ub.Business!.Id,
                    Document = ub.Business.Document,
                    Name = ub.Business.Name,
                    Address = ub.Business.Address,
                    IsActive = ub.Business.IsActive
                }).FirstOrDefault(),
                Establishment = [..user.UserEstablishment.Select(ue => new EstablishmentResDto
                {
                    Id = ue.Establishment!.Id,
                    Code = ue.Establishment.Code,
                    Name = ue.Establishment.Name,
                    IsActive = ue.Establishment.IsActive
                })],
                EmissionPoint = [..user.UserEmissionPoint.Select(ep=>new EmissionPointResDto{
                    Id = ep.EmissionPoint!.Id,
                    Code = ep.EmissionPoint.Code,
                    Description = ep.EmissionPoint.Description,
                    IsActive = ep.EmissionPoint.IsActive
                })]
            };

            response.Success = true;
            response.Message = "Usuario obtenido correctamente";
            response.Data = existingUserDto;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener el usuario";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<List<UserResDto>>> GetUsersAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<UserResDto>>();

        try
        {
            var businessId = GetBusinessIdFromToken();

            if (businessId == 0)
            {
                response.Success = false;
                response.Message = "Negocios no asociados al contexto actual";
                response.Error = "Error de asociación";

                return response;
            }

            var query = context.Users
            .Include(u => u.UserBusiness).ThenInclude(u => u.Business)
            .Include(u => u.UserEstablishment).ThenInclude(u => u.Establishment)
            .Include(u => u.UserEmissionPoint).ThenInclude(u => u.EmissionPoint)
            .Where(u =>
            !u.UserRole.Any(ur => ur.Role!.Name == "SuperAdmin") &&
            u.UserBusiness.Any(ub => ub.BusinessId == businessId));

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(
                    u =>
                    u.FullName.Contains(keyword) ||
                    u.Username.Contains(keyword) ||
                    u.Document.Contains(keyword) ||
                    u.Email.Contains(keyword));
            }

            var total = await query.CountAsync();

            var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(u => new UserResDto
            {
                Id = u.Id,
                Document = u.Document,
                Username = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                Address = u.Address,
                Cellphone = u.Cellphone,
                Telephone = u.Telephone,
                ImageUrl = u.ImageUrl,
                CompanyName = u.CompanyName,
                Sequence = u.Sequence,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                Roles = u.UserRole
                .Select(ur => new RolResDto
                {
                    Id = ur.Role!.Id,
                    Name = ur.Role.Name
                }).ToList(),
                Business = u.UserBusiness
                .Select(ub => new BusinessResDto
                {
                    Id = ub.Business!.Id,
                    Document = ub.Business.Document,
                    Name = ub.Business.Name,
                    Address = ub.Business.Address,
                    IsActive = ub.Business.IsActive,
                    CreatedAt = ub.Business.CreatedAt
                }).FirstOrDefault(),
                Establishment = u.UserEstablishment
                .Select(ue => new EstablishmentResDto
                {
                    Id = ue.Establishment!.Id,
                    Code = ue.Establishment.Code,
                    Name = ue.Establishment.Name,
                    IsActive = ue.Establishment.IsActive
                }).ToList(),
                EmissionPoint = u.UserEmissionPoint
                .Select(uep => new EmissionPointResDto
                {
                    Id = uep.EmissionPoint!.Id,
                    Code = uep.EmissionPoint.Code,
                    Description = uep.EmissionPoint.Description,
                    IsActive = uep.EmissionPoint.IsActive
                }).ToList()
            }).ToListAsync();

            response.Success = true;
            response.Message = "Usuarios obtenidos correctamente";
            response.Data = users;
            response.Pagination = new Pagination
            {
                Total = total,
                Page = page,
                Limit = limit
            };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener los usuarios";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<UserLoginResDto>> LoginAsync(UserLoginReqDto userLoginReqDto)
    {
        var response = new ApiResponse<UserLoginResDto>();

        try
        {
            var user = await context.Users
            .Include(u => u.UserRole).ThenInclude(u => u.Role)
            .Include(u => u.UserBusiness).ThenInclude(u => u.Business)
            .Include(u => u.UserEstablishment).ThenInclude(u => u.Establishment)
            .Include(u => u.UserEmissionPoint).ThenInclude(u => u.EmissionPoint)
            .FirstOrDefaultAsync(u =>
            u.Username == userLoginReqDto.Username &&
            u.IsActive == true);

            if (user == null || !BCrypt.Net.BCrypt.Verify(userLoginReqDto.Password, user.Password))
            {
                response.Success = false;
                response.Message = "Nombre de usuario o contraseña incorrectos";
                response.Error = "Credenciales inválidas";

                return response;
            }

            if (user == null || !user.IsActive)
            {
                response.Success = false;
                response.Message = "El usuario no está activo, consulte a su administrador";
                response.Error = "Error de activación";

                return response;
            }

            var business = user.UserBusiness.FirstOrDefault()?.Business;

            if (business == null)
            {
                response.Success = false;
                response.Message = "No existe una empresa asociada a este usuario";
                response.Error = "Negocio no encontrado";

                return response;
            }

            var establishment = user.UserEstablishment.FirstOrDefault()?.Establishment;

            if (establishment == null)
            {
                response.Success = false;
                response.Message = "No existe un establecimiento asociada a este usuario";
                response.Error = "Establecimiento no encontrado";

                return response;
            }

            var emissionPoint = user.UserEmissionPoint.FirstOrDefault()?.EmissionPoint;

            if (emissionPoint == null)
            {
                response.Success = false;
                response.Message = "No existe un punto de emision asociada a este usuario";
                response.Error = "Punto de emision no encontrado";

                return response;
            }

            var token = GenerateJwtToken(user, business, establishment, emissionPoint);

            response.Success = true;
            response.Message = "Inicio de sesión exitoso";
            response.Data = new UserLoginResDto
            {
                Id = user.Id,
                Document = user.Document,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                Token = token,
                Roles = [.. user.UserRole.Select(ur => ur.Role!.Name)],
                Business = user.UserBusiness
                .Where(ub => ub.UserId == user.Id)
                .Select(ub => new BusinessResDto
                {
                    Id = ub.Business!.Id,
                    Document = ub.Business.Document,
                    Name = ub.Business.Name,
                    Address = ub.Business.Address,
                    IsActive = ub.Business.IsActive
                }).FirstOrDefault(),
                Establishment = [..user.UserEstablishment.Select(ue => new EstablishmentResDto
                {
                    Id = ue.Establishment!.Id,
                    Code = ue.Establishment.Code,
                    Name = ue.Establishment.Name,
                    IsActive = ue.Establishment.IsActive
                })],
                EmissionPoint = [..user.UserEmissionPoint.Select(ep=>new EmissionPointResDto
                {
                    Id = ep.EmissionPoint!.Id,
                    Code = ep.EmissionPoint.Code,
                    Description = ep.EmissionPoint.Description,
                    IsActive = ep.EmissionPoint.IsActive
                })]
            };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener usuario";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<UserResDto>> UpdateUserAsync(int userId, UserUpdateReqDto userUpdateReqDto, List<int> rolIds, int establishmentId, int emissionPointId)
    {
        var response = new ApiResponse<UserResDto>();

        try
        {
            var businessId = GetBusinessIdFromToken();

            if (businessId == 0)
            {
                response.Success = false;
                response.Message = "Error de asociación";
                response.Error = "Negocios no asociado a esta usuario";

                return response;
            }

            var existingUser = await context.Users
            .Include(u => u.UserRole).ThenInclude(ur => ur.Role)
            .Include(u => u.UserBusiness).ThenInclude(ub => ub.Business)
            .Include(u => u.UserEstablishment).ThenInclude(ue => ue.Establishment)
            .Include(u => u.UserEmissionPoint).ThenInclude(uep => uep.EmissionPoint)
            .FirstOrDefaultAsync(u => u.Id == userId);

            if (existingUser == null)
            {
                response.Success = false;
                response.Message = "Usuario no encontrado";
                response.Error = "No existe el usuario especificado";

                return response;
            }

            var isUserInBusiness = existingUser.UserBusiness
            .Any(ub => ub.BusinessId == businessId);

            if (!isUserInBusiness)
            {
                response.Success = false;
                response.Message = "Error de asociación";
                response.Error = "El usuario no pertenece al negocio actual";

                return response;
            }

            var establishment = await context.Establishments
            .FirstOrDefaultAsync(e => e.Id == establishmentId && e.BusinessId == businessId);

            if (establishment == null)
            {
                response.Success = false;
                response.Message = "Relación inválida";
                response.Error = "El establecimiento no pertenece al negocio";

                return response;
            }

            var emissionPoint = await context.EmissionPoints
            .FirstOrDefaultAsync(ep => ep.Id == emissionPointId && ep.EstablishmentId == establishmentId);

            if (emissionPoint == null)
            {
                response.Success = false;
                response.Message = "Relación inválida";
                response.Error = "El punto de emisión no pertenece al establecimiento";

                return response;
            }

            var validRoles = await context.Roles
            .Where(r => rolIds.Contains(r.Id))
            .ToListAsync();

            if (validRoles.Count != rolIds.Count)
            {
                response.Success = false;
                response.Message = "Rol no encontrado";
                response.Error = "Uno o más roles no existen o no están disponibles";

                return response;
            }

            existingUser.Username = userUpdateReqDto.Username;
            existingUser.Document = userUpdateReqDto.Document;
            existingUser.FullName = userUpdateReqDto.FullName;
            existingUser.Email = userUpdateReqDto.Email;
            existingUser.Address = userUpdateReqDto.Address;
            existingUser.Cellphone = userUpdateReqDto.Cellphone;
            existingUser.Telephone = userUpdateReqDto.Telephone;
            existingUser.Sequence = userUpdateReqDto.Sequence;
            existingUser.ImageUrl = userUpdateReqDto.ImageUrl;
            existingUser.UpdatedAt = DateTime.UtcNow;

            context.UserRoles.RemoveRange(existingUser.UserRole);
            existingUser.UserRole = [.. rolIds.Select(rid => new UserRole
            {
                UserId = existingUser.Id,
                RoleId = rid
            })];

            context.UserEstablishments.RemoveRange(existingUser.UserEstablishment);
            existingUser.UserEstablishment = [new UserEstablishment
            {
                UserId = existingUser.Id,
                EstablishmentId = establishmentId
            }];

            context.UserEmissionPoints.RemoveRange(existingUser.UserEmissionPoint);
            existingUser.UserEmissionPoint = [new UserEmissionPoint
            {
                UserId = existingUser.Id,
                EmissionPointId = emissionPointId
            }];

            await context.SaveChangesAsync();

            var userResDto = new UserResDto
            {
                Id = existingUser.Id,
                Document = existingUser.Document,
                FullName = existingUser.FullName,
                Email = existingUser.Email,
                Username = existingUser.Username,
                Address = existingUser.Address,
                Cellphone = existingUser.Cellphone,
                Telephone = existingUser.Telephone,
                ImageUrl = existingUser.ImageUrl,
                Sequence = existingUser.Sequence,
                CompanyName = existingUser.CompanyName,
                CreatedAt = existingUser.CreatedAt,
                IsActive = existingUser.IsActive,
                Roles = [.. existingUser.UserRole.Select(ur => new RolResDto
                {
                    Id = ur.Role!.Id,
                    Name = ur.Role.Name
                })],
                Business = existingUser.UserBusiness
                .Select(ub => new BusinessResDto
                {
                    Id = ub.Business!.Id,
                    Name = ub.Business.Name,
                    Document = ub.Business.Document,
                    Address = ub.Business.Address,
                    IsActive = ub.Business.IsActive,
                    CreatedAt = ub.Business.CreatedAt
                })
                .FirstOrDefault(),
                Establishment = [..existingUser.UserEstablishment.Select(ue => new EstablishmentResDto
                {
                    Id = ue.Establishment!.Id,
                    Code = ue.Establishment.Code,
                    Name = ue.Establishment.Name,
                    IsActive = ue.Establishment.IsActive
                })],
                EmissionPoint = [..existingUser.UserEmissionPoint.Select(ep=>new EmissionPointResDto
                {
                    Id = ep.EmissionPoint!.Id,
                    Code = ep.EmissionPoint.Code,
                    Description = ep.EmissionPoint.Description,
                    IsActive = ep.EmissionPoint.IsActive
                })]
            };

            response.Success = true;
            response.Message = "Usuario actualizado correctamente";
            response.Data = userResDto;

        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al actualizar el usuario";
            response.Error = ex.Message;
        }

        return response;
    }

    private string GenerateJwtToken(User user, Business? business, Establishment? establishment, EmissionPoint? emissionPoint)
    {
        var jwtSection = config.GetSection("Jwt");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email ?? ""),
            new("BusinessId", business?.Id.ToString() ?? ""),
            new("EstablishmentId", establishment?.Id.ToString()??""),
            new("EmissionPointId", emissionPoint?.Id.ToString()??"")
        };

        foreach (var role in user.UserRole.Select(ur => ur.Role!.Name))
            claims.Add(new(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int GetBusinessIdFromToken()
    {
        var businessIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("BusinessId")?.Value;
        return int.TryParse(businessIdClaim, out var id) ? id : 0;
    }
}
