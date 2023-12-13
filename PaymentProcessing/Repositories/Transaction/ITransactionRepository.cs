using PaymentProcessing.Schemas;

namespace PaymentProcessing;

public interface ITransactionRepository
{
    public Task<Transaction> SaveTransaction(TransactionSave transactionSave);

    public Task<Transaction?> GetTransaction(Guid transactionId, string orgId);
    
    
    public Task<Transaction?> GetTransactionByCustomerKey(string customerKey);

    public Task<Transaction> UpdateTransaction(Guid transactionId, TransactionUpdate transactionUpdate);
}