namespace HospitalCare.Domain.Entities;

public class UserClaim : Entity
{
    public Guid UserId { get; private set; }
    public Guid ClaimId { get; private set; }

    private UserClaim() { }

    public UserClaim(Guid userId, Guid claimId)
    {
        UserId = userId;
        ClaimId = claimId;
    }
}
