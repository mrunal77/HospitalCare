namespace HospitalCare.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(Guid userId, string email, string firstName, string lastName, string role, IEnumerable<string>? claims = null);
    DateTime GetTokenExpiration();
}
