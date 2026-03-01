using FluentValidation;
using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Validators;

public class UpdateProfilePictureValidator : AbstractValidator<UpdateProfilePictureDto>
{
    private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public UpdateProfilePictureValidator()
    {
        RuleFor(x => x.ImageData)
            .NotEmpty()
            .WithMessage("Image data is required");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("File name is required")
            .Must(BeValidImageExtension)
            .WithMessage("Invalid image file type. Allowed types: JPEG, PNG, GIF, WebP");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("Content type is required")
            .Must(x => AllowedContentTypes.Contains(x.ToLowerInvariant()))
            .WithMessage("Invalid content type. Allowed types: image/jpeg, image/png, image/gif, image/webp");

        RuleFor(x => x.ImageData.Length)
            .Must(length => length <= MaxFileSize)
            .WithMessage($"Image size cannot exceed {MaxFileSize / (1024 * 1024)}MB");
    }

    private static bool BeValidImageExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        return extension is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp";
    }
}
