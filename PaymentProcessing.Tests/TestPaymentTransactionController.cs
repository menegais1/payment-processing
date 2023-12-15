// PaymentTransactionControllerTests.cs

using Moq;
using PaymentProcessing;
using PaymentProcessing.Schemas;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using Transaction = PaymentProcessing.Transaction;

[TestFixture]
public class PaymentTransactionControllerTests
{
    private Mock<ITransactionRepository> _transactionRepositoryMock;
    private Mock<IAsyncPaymentTransactionPublisherQueue> _publisherQueueMock;
    private PaymentTransactionController _controller;

    [SetUp]
    public void SetUp()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _publisherQueueMock = new Mock<IAsyncPaymentTransactionPublisherQueue>();
        _controller = new PaymentTransactionController(_publisherQueueMock.Object, _transactionRepositoryMock.Object);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
        };
        ((ClaimsIdentity)_controller.HttpContext.User.Identity).AddClaim(new Claim("orgId", "SampleOrgId"));
    }

    [Test]
    public async Task CreatePaymentTransaction_WhenCalled_ReturnsCorrectResponse()
    {
        var id = Guid.NewGuid();
        var expectedResponse = new CreatePaymentTransactionResponse()
        {
            Status = TransactionStatus.Created,
            Id = id
        };
        _transactionRepositoryMock.Setup(x => x.SaveTransaction(It.IsAny<TransactionSave>()))
            .ReturnsAsync(new Transaction() { Id = id, PayeeAccount = "123", PayerAccount = "321", Amount = 10 });

        var result = await _controller.CreatePaymentTransaction(new CreatePaymentTransactionRequest()
        {
            PayeeAccount = "123", PayerAccount = "321", Amount = 10
        });
        CreatePaymentTransactionResponse returnValue = result.Value;
        Assert.That(expectedResponse.ToJson(), Is.EqualTo(returnValue.ToJson()));
        _transactionRepositoryMock.Verify(x => x.SaveTransaction(It.IsAny<TransactionSave>()), Times.Once());
        _publisherQueueMock.Verify(x => x.PublishMessage(It.IsAny<PaymentTransactionMessagePayload>()), Times.Once());
    }

    [Test]
    public async Task GetTransaction_WhenCalled_ReturnsCorrectResponse()
    {
        var transactionId = Guid.NewGuid();
        var expectedTransaction = new Transaction() { OrganizationId = "SampleOrgId" };

        _transactionRepositoryMock.Setup(x => x.GetTransaction(transactionId))
            .ReturnsAsync(expectedTransaction);

        var result = await _controller.GetTransaction(transactionId);

        var returnValue = result.Value;
        Console.Out.WriteLine(returnValue.ToJson());
        Assert.That(expectedTransaction.ToJson(), Is.EqualTo(returnValue.ToJson()));

        _transactionRepositoryMock.Verify(x => x.GetTransaction(transactionId), Times.Once());
    }

    // you can write similar test for CancelTransaction method...
}