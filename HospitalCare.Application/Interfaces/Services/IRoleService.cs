namespace HospitalCare.Application.Interfaces.Services;

using HospitalCare.Application.DTOs;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllAsync();
    Task<RoleDto?> GetByIdAsync(Guid id);
    Task<RoleDto> CreateAsync(CreateRoleDto dto);
    Task<RoleDto> UpdateAsync(Guid id, UpdateRoleDto dto);
    Task DeleteAsync(Guid id);
    Task<RoleDto> ActivateAsync(Guid id);
    Task<RoleDto> DeactivateAsync(Guid id);
}
