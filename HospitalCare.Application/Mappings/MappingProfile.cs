using AutoMapper;
using HospitalCare.Application.DTOs;
using HospitalCare.Domain.Entities;

namespace HospitalCare.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Patient, PatientDto>();
        CreateMap<CreatePatientDto, Patient>()
            .ConstructUsing(dto => new Patient(dto.FirstName, dto.LastName, dto.DateOfBirth, dto.Email, dto.Phone, dto.Address));

        CreateMap<Doctor, DoctorDto>();
        CreateMap<CreateDoctorDto, Doctor>()
            .ConstructUsing(dto => new Doctor(dto.FirstName, dto.LastName, dto.Specialization, dto.Email, dto.Phone, dto.LicenseNumber));

        CreateMap<Role, RoleDto>();
        CreateMap<CreateRoleDto, Role>()
            .ConstructUsing(dto => new Role(dto.Name, dto.Description, dto.Permission));

        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.PatientName, opt => opt.Ignore())
            .ForMember(dest => dest.DoctorName, opt => opt.Ignore());

        CreateMap<Claim, ClaimDto>();
        CreateMap<CreateClaimDto, Claim>()
            .ConstructUsing(dto => new Claim(dto.Name, dto.Description, dto.Category));

        CreateMap<UserClaim, UserClaimDto>();
        CreateMap<RoleClaim, RoleClaimDto>();
    }
}
