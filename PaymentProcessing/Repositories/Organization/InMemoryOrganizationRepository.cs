using PaymentProcessing.Exceptions;

namespace PaymentProcessing;

public class InMemoryOrganizationRepository : IOrganizationRepository
{
    private static List<Organization> _organizations = [];

    public bool IsOrganizationValid(string orgId, string orgSecretKey)
    {
        var org = from organization in _organizations
            where organization.Id == orgId && organization.SecretKey == orgSecretKey
            select organization;
        return org.FirstOrDefault() is not null;
    }

    public Organization CreateOrganization(string orgId, string orgName, string orgSecretKey)
    {
        var org = from organization in _organizations
            where organization.Name == orgName
            select organization;
        if (org.FirstOrDefault() is not null) throw new OrganizationAlreadyExistsException();

        var newOrganization = new Organization(orgId, orgName, orgSecretKey);
        _organizations.Add(newOrganization);
        return newOrganization;
    }
}