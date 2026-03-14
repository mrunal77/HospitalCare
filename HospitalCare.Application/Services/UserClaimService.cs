using AutoMapper;
using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;

namespace HospitalCare.Application.Services;

public class UserClaimService : IUserClaimService
{
    private readonly IUserClaimRepository _userClaimRepository;
    private readonly IClaimRepository _claimRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly IMapper _mapper;

    public UserClaimService(
        IUserClaimRepository userClaimRepository,
        IClaimRepository claimRepository,
        IUserRepository userRepository,
        IRoleClaimRepository roleClaimRepository,
        IMapper mapper)
    {
        _userClaimRepository = userClaimRepository;
        _claimRepository = claimRepository;
        _userRepository = userRepository;
        _roleClaimRepository = roleClaimRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserClaimDto>> GetByUserIdAsync(Guid userId)
    {
        var userClaims = await _userClaimRepository.GetByUserIdAsync(userId);
        var result = new List<UserClaimDto>();

        foreach (var uc in userClaims)
        {
            var claim = await _claimRepository.GetByIdAsync(uc.ClaimId);
            result.Add(new UserClaimDto(
                uc.Id,
                uc.UserId,
                uc.ClaimId,
                claim?.Name,
                claim?.Description,
                claim?.Category,
                uc.CreatedAt
            ));
        }

        return result;
    }

    public async Task<UserClaimDto> CreateAsync(CreateUserClaimDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId);
        if (user is null)
        {
            throw new InvalidOperationException($"User with id '{dto.UserId}' not found");
        }

        var claim = await _claimRepository.GetByIdAsync(dto.ClaimId);
        if (claim is null)
        {
            throw new InvalidOperationException($"Claim with id '{dto.ClaimId}' not found");
        }

        var exists = await _userClaimRepository.ExistsAsync(dto.UserId, dto.ClaimId);
        if (exists)
        {
            throw new InvalidOperationException($"User claim already exists");
        }

        var userClaim = new UserClaim(dto.UserId, dto.ClaimId);
        var created = await _userClaimRepository.AddAsync(userClaim);

        return new UserClaimDto(
            created.Id,
            created.UserId,
            created.ClaimId,
            claim.Name,
            claim.Description,
            claim.Category,
            created.CreatedAt
        );
    }

    public async Task DeleteAsync(Guid userId, Guid claimId)
    {
        var exists = await _userClaimRepository.ExistsAsync(userId, claimId);
        if (!exists)
        {
            throw new InvalidOperationException($"User claim not found");
        }

        await _userClaimRepository.DeleteAsync(userId, claimId);
    }

    public async Task UpdateUserClaimsAsync(UpdateUserClaimsDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId);
        if (user is null)
        {
            throw new InvalidOperationException($"User with id '{dto.UserId}' not found");
        }

        await _userClaimRepository.DeleteByUserIdAsync(dto.UserId);

        foreach (var claimId in dto.ClaimIds)
        {
            var claim = await _claimRepository.GetByIdAsync(claimId);
            if (claim is null)
            {
                throw new InvalidOperationException($"Claim with id '{claimId}' not found");
            }

            var userClaim = new UserClaim(dto.UserId, claimId);
            await _userClaimRepository.AddAsync(userClaim);
        }
    }

    public async Task<IEnumerable<ClaimDto>> GetEffectiveClaimsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            throw new InvalidOperationException($"User with id '{userId}' not found");
        }

        var roleClaims = await _roleClaimRepository.GetClaimsByRoleIdAsync(user.RoleId);
        var userClaims = await _userClaimRepository.GetClaimsByUserIdAsync(userId);

        var allClaims = roleClaims.Union(userClaims, new ClaimComparer());
        return _mapper.Map<IEnumerable<ClaimDto>>(allClaims);
    }

    private class ClaimComparer : IEqualityComparer<Claim>
    {
        public bool Equals(Claim? x, Claim? y) => x?.Id == y?.Id;
        public int GetHashCode(Claim obj) => obj.Id.GetHashCode();
    }
}
