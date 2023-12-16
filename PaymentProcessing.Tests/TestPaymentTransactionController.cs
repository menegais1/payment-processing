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
    private static string OrgId = "SampleOrgId";

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
        ((ClaimsIdentity)_controller.HttpContext.User.Identity).AddClaim(new Claim("orgId", OrgId));
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
        var transaction = new Transaction() { Id = id, PayeeAccount = "123", PayerAccount = "321", Amount = 10 };
        _transactionRepositoryMock.Setup(x => x.SaveTransaction(It.IsAny<TransactionSave>()))
            .ReturnsAsync(transaction);

        var result = await _controller.CreatePaymentTransaction(new CreatePaymentTransactionRequest()
        {
            PayeeAccount = transaction.PayeeAccount, PayerAccount = transaction.PayerAccount,
            Amount = transaction.Amount
        });
        CreatePaymentTransactionResponse returnValue = result.Value;
        Assert.That(expectedResponse.ToJson(), Is.EqualTo(returnValue.ToJson()));
        _transactionRepositoryMock.Verify(x => x.SaveTransaction(It.IsAny<TransactionSave>()), Times.Once());
        _publisherQueueMock.Verify(x => x.PublishMessage(It.IsAny<PaymentTransactionMessagePayload>()), Times.Once());
    }

    [Test]
    public async Task CreatePaymentTransaction_WhenCalledTwice_IdempotencyCheck()
    {
        var id = Guid.NewGuid();
        var expectedResponse = new CreatePaymentTransactionResponse()
        {
            Status = TransactionStatus.Created,
            Id = id
        };
        var transaction = new Transaction() { Id = id, PayeeAccount = "123", PayerAccount = "321", Amount = 10,CustomerKey = "idempotency_key" };
        _transactionRepositoryMock.Setup(x => x.SaveTransaction(It.IsAny<TransactionSave>()))
            .ReturnsAsync(transaction);
        var transactionRequest = new CreatePaymentTransactionRequest()
        {
            PayeeAccount = transaction.PayeeAccount, PayerAccount = transaction.PayerAccount,
            Amount = transaction.Amount,
            CustomerKey = transaction.CustomerKey
            
        };
        
        
        var result = await _controller.CreatePaymentTransaction(transactionRequest);
        CreatePaymentTransactionResponse returnValue = result.Value;
        Assert.That(expectedResponse.ToJson(), Is.EqualTo(returnValue.ToJson()));
        _transactionRepositoryMock.Verify(x => x.SaveTransaction(It.IsAny<TransactionSave>()), Times.Once());
        _publisherQueueMock.Verify(x => x.PublishMessage(It.IsAny<PaymentTransactionMessagePayload>()), Times.Once());
        
        _transactionRepositoryMock.Invocations.Clear();
        _publisherQueueMock.Invocations.Clear();
        _transactionRepositoryMock
            .Setup(x => x.GetTransactionByCustomerKey(It.Is<string>(s => s == transaction.CustomerKey)))
            .ReturnsAsync(transaction);
        
        result = await _controller.CreatePaymentTransaction(transactionRequest);
        returnValue = result.Value;
        Assert.That(expectedResponse.ToJson(), Is.EqualTo(returnValue.ToJson()));
        _transactionRepositoryMock.Verify(x => x.SaveTransaction(It.IsAny<TransactionSave>()), Times.Never());
        _publisherQueueMock.Verify(x => x.PublishMessage(It.IsAny<PaymentTransactionMessagePayload>()), Times.Never());


    }
    
    
    [Test]
    public async Task GetTransaction_WhenCalled_ReturnsCorrectResponse()
    {
        var transactionId = Guid.NewGuid();
        var expectedTransaction = new Transaction() { OrganizationId = OrgId };

        _transactionRepositoryMock.Setup(x => x.GetTransaction(transactionId))
            .ReturnsAsync(expectedTransaction);

        var result = await _controller.GetTransaction(transactionId);

        var returnValue = result.Value;

        Assert.That(expectedTransaction.ToJson(), Is.EqualTo(returnValue.ToJson()));

        _transactionRepositoryMock.Verify(x => x.GetTransaction(transactionId), Times.Once());
    }

    [Test]
    public async Task CancelTransaction_CancelsExistingTransactionSuccessfully()
    {
        var transactionId = Guid.NewGuid();
        var payload = transactionId.ToJson();
        var fakeTransaction = new Transaction
            { Id = transactionId, Status = TransactionStatus.Created, OrganizationId = OrgId };

        _transactionRepositoryMock.Setup(a => a.GetTransaction(transactionId)).ReturnsAsync(fakeTransaction);

        var result = await _controller.CancelTransaction(transactionId);

        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }
}