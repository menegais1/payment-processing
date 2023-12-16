namespace PaymentProcessing;

public interface IOrganizationRepository
{
    public Task<bool> IsOrganizationValid(string orgId, string orgSecretKey);

    public Task<Organization> CreateOrganization(string orgId, string orgName, string orgSecretKey);
}

