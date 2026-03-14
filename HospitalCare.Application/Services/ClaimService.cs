using AutoMapper;
using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;

namespace HospitalCare.Application.Services;

public class ClaimService : IClaimService
{
    private readonly IClaimRepository _claimRepository;
    private readonly IMapper _mapper;

    public ClaimService(IClaimRepository claimRepository, IMapper mapper)
    {
        _claimRepository = claimRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ClaimDto>> GetAllAsync()
    {
        var claims = await _claimRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ClaimDto>>(claims);
    }

    public async Task<ClaimDto?> GetByIdAsync(Guid id)
    {
        var claim = await _claimRepository.GetByIdAsync(id);
        return claim is null ? null : _mapper.Map<ClaimDto>(claim);
    }

    public async Task<ClaimDto> CreateAsync(CreateClaimDto dto)
    {
        var exists = await _claimRepository.NameExistsAsync(dto.Name);
        if (exists)
        {
            throw new InvalidOperationException($"Claim with name '{dto.Name}' already exists");
        }

        var claim = new Claim(dto.Name, dto.Description, dto.Category);
        var created = await _claimRepository.AddAsync(claim);
        return _mapper.Map<ClaimDto>(created);
    }

    public async Task<ClaimDto> UpdateAsync(Guid id, UpdateClaimDto dto)
    {
        var claim = await _claimRepository.GetByIdAsync(id);
        if (claim is null)
        {
            throw new InvalidOperationException($"Claim with id '{id}' not found");
        }

        var exists = await _claimRepository.NameExistsAsync(dto.Name);
        if (exists && claim.Name != dto.Name)
        {
            throw new InvalidOperationException($"Claim with name '{dto.Name}' already exists");
        }

        claim.Update(dto.Name, dto.Description, dto.Category);
        await _claimRepository.UpdateAsync(claim);
        return _mapper.Map<ClaimDto>(claim);
    }

    public async Task DeleteAsync(Guid id)
    {
        var claim = await _claimRepository.GetByIdAsync(id);
        if (claim is null)
        {
            throw new InvalidOperationException($"Claim with id '{id}' not found");
        }

        await _claimRepository.DeleteAsync(id);
    }

    public async Task<ClaimDto> ActivateAsync(Guid id)
    {
        var claim = await _claimRepository.GetByIdAsync(id);
        if (claim is null)
        {
            throw new InvalidOperationException($"Claim with id '{id}' not found");
        }

        claim.Activate();
        await _claimRepository.UpdateAsync(claim);
        return _mapper.Map<ClaimDto>(claim);
    }

    public async Task<ClaimDto> DeactivateAsync(Guid id)
    {
        var claim = await _claimRepository.GetByIdAsync(id);
        if (claim is null)
        {
            throw new InvalidOperationException($"Claim with id '{id}' not found");
        }

        claim.Deactivate();
        await _claimRepository.UpdateAsync(claim);
        return _mapper.Map<ClaimDto>(claim);
    }

    public async Task<IEnumerable<ClaimDto>> GetActiveAsync()
    {
        var claims = await _claimRepository.GetActiveAsync();
        return _mapper.Map<IEnumerable<ClaimDto>>(claims);
    }

    public async Task<IEnumerable<ClaimDto>> GetByCategoryAsync(string category)
    {
        var claims = await _claimRepository.GetByCategoryAsync(category);
        return _mapper.Map<IEnumerable<ClaimDto>>(claims);
    }
}
