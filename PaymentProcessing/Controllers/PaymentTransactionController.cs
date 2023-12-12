using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace PaymentProcessing
{
    public class CreatePaymentTransactionRequest()
    {
        public string PayerAccount { get; set; }
        public string PayeeAccount { get; set; }
        public double Amount { get; set; }
    }

    [Route("transaction")]
    [ApiController]
    public class PaymentTransactionController : ControllerBase
    {
        private IAsyncPaymentTransactionPublisherQueue _publisherQueue;

        public PaymentTransactionController(IAsyncPaymentTransactionPublisherQueue publisherQueue)
        {
            _publisherQueue = publisherQueue;
        }

        [HttpPost]
        // [Authorize]
        public async Task<ActionResult<CreatePaymentTransactionRequest>> CreatePaymentTransaction(
            CreatePaymentTransactionRequest request)
        {
            _publisherQueue.PublishMessage(
                new PaymentTransactionMessagePayload(PaymentTransactionTaskType.Create, request.ToJson()));
            return request;
        }

        [HttpGet]
        public async Task<ActionResult<List<string>>> GetTransactions()
        {
            List<string> response = ["bananas"];
            return response;
        }
    }
}