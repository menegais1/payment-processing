using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using PaymentProcessing.Exceptions;

namespace PaymentProcessing
{
    [Route("organization")]
    [ApiController]
    public class OrganizationController(IOrganizationRepository organizationRepository) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<Organization>> Register(RegisterRequest request)
        {
            var secretKey = "sk_" + AuthenticationService.GenerateSecretKey();
            var orgId = "org_" + AuthenticationService.GenerateSecretKey();
            try
            {
                var organization =
                    organizationRepository.CreateOrganization(orgId: orgId, orgName: request.OrgName,
                        orgSecretKey: secretKey);
                return organization;
            }
            catch (OrganizationAlreadyExistsException e)
            {
                return BadRequest("Organization already exists");
            }
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<string>> Authenticate(AuthenticateRequest request)
        {
            if (!organizationRepository.IsOrganizationValid(request.OrgId, request.OrgSecretKey))
                return Unauthorized("The organization is not registered in the system.");
            return AuthenticationService.GenerateJwtToken(request.OrgId);
        }
    }
}