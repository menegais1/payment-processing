using Microsoft.EntityFrameworkCore;
using PaymentProcessing.Database;

namespace PaymentProcessing;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly PaymentProcessingContext _context;

    public OrganizationRepository(PaymentProcessingContext context)
    {
        _context = context;
    }

    public async Task<bool> IsOrganizationValid(string orgId, string orgSecretKey)
    {
        return await _context.Organizations.AnyAsync(o => o.Id == orgId && o.SecretKey == orgSecretKey);
    }

    public async Task<Organization> CreateOrganization(string orgId, string orgName, string orgSecretKey)
    {
        var organization = new Organization
        (
            id: orgId,
            name: orgName,
            secretKey: orgSecretKey
        );
        await _context.Organizations.AddAsync(organization);
        await _context.SaveChangesAsync();
        return organization;
    }
}