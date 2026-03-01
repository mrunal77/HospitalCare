namespace HospitalCare.Application.DTOs;

public record UpdateProfilePictureDto(
    byte[] ImageData,
    string FileName,
    string ContentType
);

public record UpdateBioDto(
    string Bio
);

public record UpdatePreferencesDto(
    string Timezone,
    string Language
);

public record UpdateUsernameDto(
    string Username,
    string? DisplayName
);

public record UpdateProfileDto(
    string? FirstName,
    string? LastName,
    string? Bio,
    string? Timezone,
    string? Language
);
