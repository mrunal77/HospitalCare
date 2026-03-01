using AutoMapper;
using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;

namespace HospitalCare.Application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public RoleService(IRoleRepository roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<RoleDto?> GetByIdAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        return role is null ? null : _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto)
    {
        var exists = await _roleRepository.NameExistsAsync(dto.Name);
        if (exists)
        {
            throw new InvalidOperationException($"Role with name '{dto.Name}' already exists");
        }

        var role = new Role(dto.Name, dto.Description, dto.Permission);
        var created = await _roleRepository.AddAsync(role);
        return _mapper.Map<RoleDto>(created);
    }

    public async Task<RoleDto> UpdateAsync(Guid id, UpdateRoleDto dto)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role is null)
        {
            throw new InvalidOperationException($"Role with id '{id}' not found");
        }

        var exists = await _roleRepository.NameExistsAsync(dto.Name);
        if (exists && role.Name != dto.Name)
        {
            throw new InvalidOperationException($"Role with name '{dto.Name}' already exists");
        }

        role.Update(dto.Name, dto.Description, dto.Permission);
        await _roleRepository.UpdateAsync(role);
        return _mapper.Map<RoleDto>(role);
    }

    public async Task DeleteAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role is null)
        {
            throw new InvalidOperationException($"Role with id '{id}' not found");
        }

        await _roleRepository.DeleteAsync(id);
    }

    public async Task<RoleDto> ActivateAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role is null)
        {
            throw new InvalidOperationException($"Role with id '{id}' not found");
        }

        role.Activate();
        await _roleRepository.UpdateAsync(role);
        return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto> DeactivateAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role is null)
        {
            throw new InvalidOperationException($"Role with id '{id}' not found");
        }

        role.Deactivate();
        await _roleRepository.UpdateAsync(role);
        return _mapper.Map<RoleDto>(role);
    }
}
