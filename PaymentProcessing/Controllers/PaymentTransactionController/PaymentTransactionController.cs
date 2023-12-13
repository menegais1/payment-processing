using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using PaymentProcessing.Exceptions;
using PaymentProcessing.Schemas;

namespace PaymentProcessing
{
    [Route("transaction")]
    [ApiController]
    public class PaymentTransactionController : ControllerBase
    {
        private IAsyncPaymentTransactionPublisherQueue _publisherQueue;
        private ITransactionRepository _transactionRepository;

        public PaymentTransactionController(IAsyncPaymentTransactionPublisherQueue publisherQueue,
            ITransactionRepository transactionRepository)
        {
            _publisherQueue = publisherQueue;
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CreatePaymentTransactionResponse>> CreatePaymentTransaction(
            CreatePaymentTransactionRequest request)
        {
            if (request.PayerAccount.IsNullOrEmpty() || request.Amount < 0)
            {
                return ValidationProblem("Payer account cannot be null or empty.");
            }

            if (request.PayeeAccount.IsNullOrEmpty())
            {
                return ValidationProblem("Payee account cannot be null or empty.");
            }

            if (request.Amount < 0)
            {
                return ValidationProblem("Request amount cannot be negative.");
            }

            if (request.CustomerKey is not null)
            {
                var prevTransaction = await _transactionRepository.GetTransactionByCustomerKey(request.CustomerKey);
                if (prevTransaction is not null)
                    return new CreatePaymentTransactionResponse()
                    {
                        Id = prevTransaction.Id,
                        Status = prevTransaction.Status
                    };
            }

            // We store the transaction in the DB so we have the idempotency check in place, and if the queue is offline so we can replay it later
            var transaction = await _transactionRepository.SaveTransaction(
                new TransactionSave(request.PayerAccount, request.PayeeAccount, request.Amount)
                {
                    Description = request.Description,
                    CustomerKey = request.CustomerKey,
                    Fee = request.Fee ?? 0,
                });


            _publisherQueue.PublishMessage(
                new PaymentTransactionMessagePayload(PaymentTransactionTaskType.Process, transaction.Id.ToJson()));

            return new CreatePaymentTransactionResponse()
            {
                Id = transaction.Id,
                Status = transaction.Status
            };
        }

        [HttpGet]
        public async Task<ActionResult<List<string>>> GetTransaction()
        {
            List<string> response = ["bananas"];
            return response;
        }
    }
}