namespace HospitalCare.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(Guid userId, string email, string firstName, string lastName, string role);
    DateTime GetTokenExpiration();
}
