namespace HospitalCare.Domain.Entities;

public class RoleClaim : Entity
{
    public Guid RoleId { get; private set; }
    public Guid ClaimId { get; private set; }

    private RoleClaim() { }

    public RoleClaim(Guid roleId, Guid claimId)
    {
        RoleId = roleId;
        ClaimId = claimId;
    }
}
