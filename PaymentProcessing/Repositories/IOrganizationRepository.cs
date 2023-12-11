namespace PaymentProcessing;

public interface IOrganizationRepository
{
    public bool IsOrganizationValid(string orgId, string orgSecretKey);

    public Organization CreateOrganization(string orgId, string orgName, string orgSecretKey);
}