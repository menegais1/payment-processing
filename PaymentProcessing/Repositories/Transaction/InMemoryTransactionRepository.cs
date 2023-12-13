using PaymentProcessing.Schemas;

namespace PaymentProcessing;

public class InMemoryTransactionRepository : ITransactionRepository
{
    private static List<Transaction> _transactions = [];
    private static List<string> _uniqueCustomerKeys = [];
    private static List<Guid> _uniqueIds = [];

    public async Task<Transaction> SaveTransaction(TransactionSave transactionSave)
    {
        lock (_uniqueCustomerKeys)
        {
            var transaction = new Transaction()
            {
                Id = Guid.NewGuid(),
                PayeeAccount = transactionSave.PayeeAccount,
                PayerAccount = transactionSave.PayerAccount,
                Amount = transactionSave.Amount,
                Fee = transactionSave.Fee,
                CreatedAt = DateTime.UtcNow,
                Description = transactionSave.Description,
                CustomerKey = transactionSave.CustomerKey,
            };

            _transactions.Add(transaction);
            return transaction;
        }
    }


    public async Task<Transaction?> GetTransaction(Guid transactionId)
    {
        var query = from trs in _transactions where trs.Id == transactionId select trs;
        return query.FirstOrDefault();
    }

    public async Task<Transaction?> GetTransactionByCustomerKey(string customerKey)
    {
        var query = from trs in _transactions where trs.CustomerKey == customerKey select trs;
        return query.FirstOrDefault();
    }

    public async Task<Transaction> UpdateTransaction(Guid transactionId, TransactionUpdate transactionUpdate)
    {
        var query = from trs in _transactions where trs.Id == transactionId select trs;
        var curTransaction = query.FirstOrDefault();
        if (curTransaction is null)
        {
            throw new Exception("Transaction doesn't exists in the database");
        }

        curTransaction.Status = transactionUpdate.Status ?? curTransaction.Status;
        curTransaction.ApprovedAt = transactionUpdate.ApprovedAt ?? curTransaction.ApprovedAt;
        curTransaction.CancelledAt = transactionUpdate.CancelledAt ?? curTransaction.CancelledAt;
        curTransaction.FailedAt = transactionUpdate.FailedAt ?? curTransaction.FailedAt;
        return curTransaction;
    }
}