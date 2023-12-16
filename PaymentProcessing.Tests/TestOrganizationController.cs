using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaymentProcessing;
using PaymentProcessing.Exceptions;
using Moq;

public class OrganizationControllerTests
{
    private Mock<IOrganizationRepository> _repository;
    private OrganizationController _controller;

    [SetUp]
    public void Setup()
    {
        _repository = new Mock<IOrganizationRepository>();
        _controller = new OrganizationController(_repository.Object);
    }

    [Test]
    public async Task Register_ReturnsClientFacingOrganization_WhenOrganizationCreated()
    {
        // Arrange
        var orgName = "TestOrganization";
        var secretKey = "sk_key";
        var orgId = "org_id";

        var organization = new Organization(orgId, orgName, secretKey);

        _repository.Setup(r => r.CreateOrganization(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(organization);

        // Act 
        var result = await _controller.Register(new RegisterRequest(orgName));

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(orgName, result.Value.Name);
        Assert.AreEqual(secretKey, result.Value.SecretKey);
        Assert.AreEqual(orgId, result.Value.Id);
    }

    [Test]
    public async Task Register_ReturnsBadRequest_WhenOrganizationAlreadyExists()
    {
        // Arrange
        var orgName = "TestOrganization";

        _repository.Setup(r => r.CreateOrganization(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new OrganizationAlreadyExistsException());

        // Act 
        var result = await _controller.Register(new RegisterRequest(orgName));

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Result is BadRequestObjectResult);
    }

    [Test]
    public async Task Authenticate_ReturnsToken_WhenOrganizationIsValid()
    {
        // Arrange
        var orgId = "org_id";
        var orgSecretKey = "sk_key";

        _repository.Setup(r => r.IsOrganizationValid(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act 
        var result = await _controller.Authenticate(new AuthenticateRequest(orgId, orgSecretKey));

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(!string.IsNullOrEmpty(result.Value));
    }

    [Test]
    public async Task Authenticate_ReturnsUnauthorized_WhenOrganizationIsInValid()
    {
        // Arrange
        var orgId = "org_id";
        var orgSecretKey = "sk_key";

        _repository.Setup(r => r.IsOrganizationValid(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act 
        var result = await _controller.Authenticate(new AuthenticateRequest(orgId, orgSecretKey));

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Result is UnauthorizedObjectResult);
    }
}