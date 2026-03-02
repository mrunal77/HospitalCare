using System.Security.Cryptography;
using AutoMapper;
using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;

namespace HospitalCare.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public DoctorService(
        IDoctorRepository doctorRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IMapper mapper)
    {
        _doctorRepository = doctorRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<DoctorDto?> GetByIdAsync(Guid id)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        return doctor is null ? null : _mapper.Map<DoctorDto>(doctor);
    }

    public async Task<IEnumerable<DoctorDto>> GetAllAsync()
    {
        var doctors = await _doctorRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
    }

    public async Task<DoctorDto> CreateAsync(CreateDoctorDto dto)
    {
        var doctor = new Doctor(
            dto.FirstName,
            dto.LastName,
            dto.Specialization,
            dto.Email,
            dto.Phone,
            dto.LicenseNumber
        );

        var created = await _doctorRepository.AddAsync(doctor);

        var role = await _roleRepository.GetByNameAsync("Doctor");
        if (role is null)
        {
            role = await _roleRepository.GetByNameAsync("HospitalEmployee");
        }
        
        if (role is not null)
        {
            var user = new User(
                dto.Email,
                HashPassword("Pass@123"),
                dto.FirstName,
                dto.LastName,
                role.Id
            );
            await _userRepository.AddAsync(user);
        }

        return _mapper.Map<DoctorDto>(created);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor is null) return false;

        await _doctorRepository.DeleteAsync(id);
        return true;
    }

    public async Task<IEnumerable<DoctorDto>> GetBySpecializationAsync(string specialization)
    {
        var doctors = await _doctorRepository.GetBySpecializationAsync(specialization);
        return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
    }

    private static string HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[16];
        rng.GetBytes(salt);

        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, 32);
        var hashBytes = new byte[48];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }
}
