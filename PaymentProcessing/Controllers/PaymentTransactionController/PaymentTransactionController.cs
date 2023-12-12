using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using PaymentProcessing.Schemas;

namespace PaymentProcessing
{
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
        [Authorize]
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