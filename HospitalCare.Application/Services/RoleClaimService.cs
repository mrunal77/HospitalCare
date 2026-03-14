using AutoMapper;
using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;

namespace HospitalCare.Application.Services;

public class RoleClaimService : IRoleClaimService
{
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly IClaimRepository _claimRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public RoleClaimService(
        IRoleClaimRepository roleClaimRepository,
        IClaimRepository claimRepository,
        IRoleRepository roleRepository,
        IMapper mapper)
    {
        _roleClaimRepository = roleClaimRepository;
        _claimRepository = claimRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoleClaimDto>> GetByRoleIdAsync(Guid roleId)
    {
        var roleClaims = await _roleClaimRepository.GetByRoleIdAsync(roleId);
        var result = new List<RoleClaimDto>();

        foreach (var rc in roleClaims)
        {
            var claim = await _claimRepository.GetByIdAsync(rc.ClaimId);
            result.Add(new RoleClaimDto(
                rc.Id,
                rc.RoleId,
                rc.ClaimId,
                claim?.Name,
                claim?.Description,
                claim?.Category,
                rc.CreatedAt
            ));
        }

        return result;
    }

    public async Task<RoleClaimDto> CreateAsync(CreateRoleClaimDto dto)
    {
        var role = await _roleRepository.GetByIdAsync(dto.RoleId);
        if (role is null)
        {
            throw new InvalidOperationException($"Role with id '{dto.RoleId}' not found");
        }

        var claim = await _claimRepository.GetByIdAsync(dto.ClaimId);
        if (claim is null)
        {
            throw new InvalidOperationException($"Claim with id '{dto.ClaimId}' not found");
        }

        var exists = await _roleClaimRepository.ExistsAsync(dto.RoleId, dto.ClaimId);
        if (exists)
        {
            throw new InvalidOperationException($"Role claim already exists");
        }

        var roleClaim = new RoleClaim(dto.RoleId, dto.ClaimId);
        var created = await _roleClaimRepository.AddAsync(roleClaim);

        return new RoleClaimDto(
            created.Id,
            created.RoleId,
            created.ClaimId,
            claim.Name,
            claim.Description,
            claim.Category,
            created.CreatedAt
        );
    }

    public async Task DeleteAsync(Guid roleId, Guid claimId)
    {
        var exists = await _roleClaimRepository.ExistsAsync(roleId, claimId);
        if (!exists)
        {
            throw new InvalidOperationException($"Role claim not found");
        }

        await _roleClaimRepository.DeleteAsync(roleId, claimId);
    }

    public async Task UpdateRoleClaimsAsync(UpdateRoleClaimsDto dto)
    {
        var role = await _roleRepository.GetByIdAsync(dto.RoleId);
        if (role is null)
        {
            throw new InvalidOperationException($"Role with id '{dto.RoleId}' not found");
        }

        await _roleClaimRepository.DeleteByRoleIdAsync(dto.RoleId);

        foreach (var claimId in dto.ClaimIds)
        {
            var claim = await _claimRepository.GetByIdAsync(claimId);
            if (claim is null)
            {
                throw new InvalidOperationException($"Claim with id '{claimId}' not found");
            }

            var roleClaim = new RoleClaim(dto.RoleId, claimId);
            await _roleClaimRepository.AddAsync(roleClaim);
        }
    }

    public async Task<IEnumerable<ClaimDto>> GetClaimsByRoleIdAsync(Guid roleId)
    {
        var claims = await _roleClaimRepository.GetClaimsByRoleIdAsync(roleId);
        return _mapper.Map<IEnumerable<ClaimDto>>(claims);
    }
}
